---
title: "Duplex Service için Silverlight İstemcisi Geliştirmek"
pubDate: 2010-07-05 01:00:00
categories:
  - Silverlight 4.0
  - WCF
tags:
  - Silverlight 4.0
  - WCF
  - XML
  - C#
  - .NET
  - ASP.NET
  - Silverlight
  - XAML
  - HTTP
  - IIS
  - Visual Studio
  - Rc
---

# Duplex Service için Silverlight İstemcisi Geliştirmek
Merhaba Arkadaşlar,

Hatırlayacağınız üzere [bir önceki yazımızda](Silverlight İstemcileri için Duplex Service Geliştirmek.md) Silverlight istemcilerinin kullanabileceği Duplex WCF Service uygulamalarının nasıl yazılabileceğini incelemeye çalışmıştık. Çok doğal olarak bu işin bir de istemci tarafı bulunmaktadır. İşte bu yazımızda söz konusu istemciyi geliştirmeye çalışacak ve bir önceki yazının yorgunluğunu üzerimizden atarcasına, basit bir şekilde ilerliyor olacağız. İlk olarak Visual Studio 2010 Ultimate RC ortamında Silverlight 4.0 tabanlı bir uygulama oluşturarak işe başlayabiliriz. Bu işlemin ardından Proxy tabanlı bir WCF servis kullanımı için Add Service Reference seçeneğine başvurmamız gerekecektir. Yine hatırlayacağınız üzere geliştirdiğimiz WorldWeatherService isimli servisi IIS üzerine Publish etmiştik. Bu sebepten ilgili servis referansına aşağıdaki şekilden de görüldüğü üzere http://localhost/WorldWeatherService/WeatherDuplexService.svc adresinden erişebiliriz.

![blg173_AddServiceReference.gif](images/blg173_AddServiceReference.gif)

İstemci tarafında çok basit olarak aşağıdaki XAML içeriğine sahip bir kontrol kullanıyor olacağız. Buna göre istemciler bir şehir adı girerek sunucudan anlık hava durumu bilgilerini alabilecekleri bir arayüze sahip olacaklar. Aslında alacaklar demek çok doğru bir tabir değil. Nitekim servisin kendisi, bağlı olan istemci üzerinde tetiklediği bir operasyona bu bilgileri parametre şeklinde gönderiyor olacak.

```xml
<UserControl x:Class="WeatherClientApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="230" d:DesignWidth="479">

    <Grid x:Name="LayoutRoot" Background="White">
        <Button Content="Start" Height="23" HorizontalAlignment="Left" Margin="207,27,0,0" Name="StartButton" VerticalAlignment="Top" Width="75" Click="StartButton_Click" />
        <TextBox Height="23" HorizontalAlignment="Left" Margin="25,27,0,0" Name="CityTextBox" VerticalAlignment="Top" Width="165" />
        <ListBox Height="142" HorizontalAlignment="Left" Margin="24,64,0,0" Name="WeatherStatusListBox" VerticalAlignment="Top" Width="434" />
    </Grid>
</UserControl>
```

İstemci tarafı kodlarına gelince;

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using WeatherClientApp.WeatherServiceReference;

namespace WeatherClientApp
{
    public partial class MainPage : UserControl
    {
        WeatherDuplexServiceClient proxy = null;

        public MainPage()
        {
            InitializeComponent();

            EndpointAddress address = new EndpointAddress("http://localhost/WorldWeatherService/WeatherDuplexService.svc");
            PollingDuplexHttpBinding binding = new PollingDuplexHttpBinding(PollingDuplexMode.MultipleMessagesPerPoll);
            proxy = new WeatherDuplexServiceClient(binding, address);

            proxy.SetCityCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(proxy_SetCityCompleted);
            proxy.NoticeReceived += new EventHandler<NoticeReceivedEventArgs>(proxy_NoticeReceived);
        }

        void proxy_NoticeReceived(object sender, NoticeReceivedEventArgs e)
        {
            WeatherStatus wStatus = e.weather;
            WeatherStatusListBox.Items.Add(String.Format("{0}({1} C){2}", wStatus.City, wStatus.Heat, wStatus.Summary));
        }

        void proxy_SetCityCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            WeatherStatusListBox.Items.Add("SetCity çağrısı tamamlandı");
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            WeatherStatusListBox.Items.Clear();
            proxy.SetCityAsync(CityTextBox.Text);
        }
    }
}
```

Dikkat edileceği üzere WeatherDuplexServiceClient tipinden olan proxy nesnesi örneklenirken iki önemli parametre bilgisi geçilmektedir. Bunlardan ilki PollingDuplexHttpBinding tipinden olan bağlayıcı tiptir (Binding Type). Diğeri ise servise erişilecek olan Endpoint adresidir. İstemci tarafı asenkron olarak SetCity metoduna erişebilir. Bu nedenle SetCityCompleted olay metodu yüklenmiştir. Dikkat çekici noktalardan birisi de NoticeReceived isimli bir olayın söz konusu olmasıdır. Bilinen Completed son eki yerine Received son ekinin gelmesinin de bir anlamı vardır elbette.

![Wink](images/smiley-wink.gif)

Bu, istemcinin servisten gelen Notice çağrısını takiben devreye girecek operasyon ile alakalıdır. Bir başka deyişle servis tarafı Notice metodunu çağırdıktan ve bu operasyon işleyişini tamamladıktan sonra istemci tarafında proxyNoticeReceived olay metodu devreye girecektir. Ayrıca, bu olay metodunun NoticeReceivedEventArgs tipinden olan parametresi üzerinden yakalanan weather özelliği yardımıyla, servisin gönderdiği WeatherStatus nesnesine ulaşılabilir. Sonuç olarak uygulamayı test ettiğimizde örnek olarak aşağıdakine benzer bir sonuç elde ettiğimizi görebiliriz.

![blg172_Runtime.gif](images/blg172_Runtime.gif)

Üç sonuç gelmesi tamamen servis tarafındaki zamanlama ayarları ile alakalı bir durumdur. Bu sürelerde oynayarak servisin istemci tarafına kaç kere bildirimde bulunacağını da ayarlayabilirsiniz. Önemli olan nokta servisin istemci üzerinde bir operasyon tetiklemesidir. Bunu yazdığımız istemci ile test etmiş olduk.

Tüm bu çalışma sırasında dikkat edilmesi gereken bir husus da, önceki yazımızda da değinmiş olduğumuz Client Access Policy kullanımıdır. Eğer IIS root klasörü altında ClientAccessPolicy.xml dosyası ve gerekli içeriği olmassa çalışma zamanında aşağıdaki hata mesajı ile karşılaşılacaktır.

![blg172_Exception.gif](images/blg172_Exception.gif)

Oysaki geliştirdiğimiz örnek Asp.Net Development Server üzerinden yayınlanmaktadır (http://localhost:22334/WeatherClientAppTestPage.aspx) ve sorunsuz bir şekilde IIS üzerindeki WorldWeatherService uygulamasına erişebilmektedir. Dolayısıyla Silverlight uygulamalarında sıkça rastladığımız Cross Domain sorunu yaşanmamaktadır. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WeatherClientApp.rar (540,12 kb)](assets/WeatherClientApp.rar) [Örnek Visual Studio 2010 RC sürümü üzerinde geliştirilmiş ve test edilmiştir]
