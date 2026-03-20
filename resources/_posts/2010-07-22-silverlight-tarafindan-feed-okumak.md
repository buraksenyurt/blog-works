---
layout: post
title: "Silverlight Tarafından Feed Okumak"
date: 2010-07-22 09:05:00 +0300
categories:
  - silverlight-4-0
  - wcf-eco-system
  - wcf-ria-services
tags:
  - silverlight-4-0
  - wcf-eco-system
  - wcf-ria-services
  - xml
  - csharp
  - dotnet
  - aspnet
  - linq
  - wcf
  - silverlight
  - xaml
  - http
  - iis
  - serialization
  - generics
  - visual-studio
---
Yeni bir maceraya hazır mısınız? Hureyyy dediğinizi duyar gibiyim. Bildiğiniz üzere Internet kaynaklarının takibinin kolay bir şekilde yapılabilmesi adına RSS veya Atom formatındaki Feed içeriklerinden sıklıkla yararlanmaktayız. Blog, Community, News Group ve benzeri pek çok internet kaynağı, güncel içeriklerini yayınlamak amacıyla global olarak standart hale getirilmiş olan bu formatları kullanmaktalar.

![blg176_Giris.jpg](/assets/images/2010/blg176_Giris.jpg)

Pek tabi yayınlanan bu içeriklerin takip edilebilmesi içinde çeşitli istemci programlar söz konusu. FeedReader bu uygulamalara örnek olarak verilebilecek Windows tabanlı iddialı programlardan birisi. Feed içerikleri zaman zaman internet siteleri üzerinde kontrol şeklinde de barındırılmaktadır. Söz gelimi pek çok blog içerisinde bu durum söz konusudur ve hatta hazır Widget'lar yardımıyla entegrasyonları son derece kolaydır. Peki maceramız nerede başlıyor? Özellikle ambulans resminin bu konu ile alakası nedir?

![Sealed](/assets/images/2010/smiley-sealed.gif)

Doğruyu söylemek gerekirse sıkıldığım bir ara ne yapayım diye düşünürken Silverlight 4.0 tabanlı olarak geliştirilen bir uygulamadan RSS içeriklerini nasıl okuyabileceğimi düşünmeye başladım. Daha önceden HTTP bazlı Get,Post,Put, Delete metodlarınaa cevap veren WCF tabanlı servislerin tüketilmesi için WebClient tipinden nasıl yararlanıldığını incelemiştim ([Silverlight Tarafında HTTP Bazli Servisleri Kullanmak](https://www.buraksenyurt.com/admin/app/editor/post/Silverlight-Tarafinda-HTTP-Bazli-Servisleri-Kullanmak) isimli yazıyı incelemenizi öneririm) Yine aynı şekilde devam ederek herhangibir RSS içeriğini örnek Silverlight uygulamama taşıyabileceğimi düşünerek kolları sıvadım ve heyecanlı bir şekilde aşağıdaki ekran görüntüsü ve XAML içeriğine sahip kontrolü oluşturdum.

![blg176_Design.gif](/assets/images/2010/blg176_Design.gif)

XAML içeriği;

```xml
<UserControl x:Class="RSSReaderim.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="337" d:DesignWidth="394" xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk">

    <Grid x:Name="LayoutRoot" Background="White">
        <Button Content="RSS Oku" Height="23" HorizontalAlignment="Left" Margin="313,76,0,0" Name="ReadRSSButton" VerticalAlignment="Top" Width="75" Click="ReadRSSButton_Click" />
        <sdk:Label Height="29" HorizontalAlignment="Left" Margin="8,12,0,0" Name="label1" VerticalAlignment="Top" Width="69" Content="RSS Adresi" FontSize="10" />
        <ListBox Height="180" HorizontalAlignment="Left" Margin="6,105,0,0" Name="RSSListBox" VerticalAlignment="Top" Width="382" ItemsSource="{Binding}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Title.Text}" Foreground="BlueViolet" />
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
        <TextBox Height="23" HorizontalAlignment="Left" Margin="8,47,0,0" Name="RSSTextBox" VerticalAlignment="Top" Width="380" />
        <sdk:Label Height="40" HorizontalAlignment="Left" Margin="8,297,0,0" Name="RSSInfoLabel" VerticalAlignment="Top" Width="380" FontSize="9" />
    </Grid>
</UserControl>
```

Aslında teori son derece basitti. Kullanıcı TextBox kontrolü üzerinden bir RSS adresi girecekti. Sonra düğmeye basarak içeriğin ListBox kontrolüne dolmasını seyredecekti. Son derece basit ve masumane bir talep öyle değil mi?

![Undecided](/assets/images/2010/smiley-undecided.gif)

Tabi bu işlemler için kod tarafını da, heyecanlı bir şekilde aşağıdaki gibi geliştirmeye çalıştım.

```csharp
using System;
using System.Net;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace RSSReaderim
{
    public partial class MainPage : UserControl
    {
        WebClient client;

        public MainPage()
        {
            InitializeComponent();
            // WebClient nesnesi örneklenir
            client = new WebClient();
            // RSS Adresinden okuma işlemi tamamlanınca devreye girecek olan olay metodu yüklenir
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null) // Eğer okuma işlemi sırasında bir hata oluşmadıysa
            {
                // RSS bilgisi e.Result üzerinden Stream şeklinde elde edilir ve XmlReader nesnesinin örneklenmesi için kullanılır. Bu gereklidir nitekim SyndicationFeed.Load metodu XmlReader tipi ile çalışmaktadır.
                XmlReader xReader = XmlReader.Create(e.Result);
                // System.ServiceModel.Syndication.dll Assembly' ının projeye referans edilmesi gerekmektedir.
                SyndicationFeed feed = SyndicationFeed.Load(xReader);
                // Items koleksiyonu ListBox bileşenine veri kaynağı olarak bağlanır
                RSSListBox.ItemsSource = feed.Items;
            }
            else if(e.Error!=null)
            {
                // Bir hata oluştuysa istisna mesajını Label kontrolünde göster
                RSSInfoLabel.Content = String.Format("Bir Sorun oluştu. {0}", e.Error);
            }
        }

        private void ReadRSSButton_Click(object sender, RoutedEventArgs e)
        {            
            // Okuma işlemini başlat.
            // Örnek RSS Adresi : http://www.buraksenyurt.com/syndication.axd?format=rss
            if (!String.IsNullOrEmpty(RSSTextBox.Text))
                client.OpenReadAsync(new Uri(RSSTextBox.Text));
            else
                RSSInfoLabel.Content = "Lütfen bir RSS Adresi giriniz";
        }
    }
}
```

Kod parçasından da görüldüğü üzere WebClient tipini kullanarak TextBox kontrolüne girilen adres için bir talepte bulunulmaktadır. Söz konusu talepin sonucu elde edildiğinde devreye giren olay metodu içerisinde ise, öncelikli olarak bir hata kontrolü yapılmaktadır. Eğer herhangibir hata söz konusu değilse SyndicationFeed tipinden yararlanılarak elde edilen Stream referansının Feed olarak ele alınabilmesi amacıyla gerekli işlemler yapılmaktadır. Son olarak söz konusu içerik nesnesi üzerinden ulaşılan Items koleksiyonu, ListBox kontrolüne bağlanır.

Şimdi blog girdimizin başında yer alan resmi açıklayalım. Bu kadar süratli araba kullanırsanız duvara toslamanız an meselesi olabilir. Aynen örneğimizde şu an tosladığımız gibi

![Undecided](/assets/images/2010/smiley-undecided.gif)

İşte duvara tosladığımız anda saniyenin milyonda birinde şişen hava yastığı içinden fırlayan Exception mesajımız.

![blg176_Exception.gif](/assets/images/2010/blg176_Exception.gif)

Hayda breeeee!!!

![Surprised](/assets/images/2010/smiley-surprised.gif)

İşte hızlı gitmenin doğal sonucu.

Aslında gözden kaçırdığımız çok önemli bir durum söz konusu. O da Silverlight tarafında önem arz eden konuların başında gelen Cross-Domain Policy vakası. Sonuç itibariyle RSS çıktısı için talepte bulunduğumuz Domain adresi ile örneği geliştirmekte olduğumuz Asp.Net Development Server'ın port numarası eşliğine açtığı Domain adresleri birbirlerinden farklı. Bu sebepten sunucu tarafının bir ClientAccessPolicy.xml dosyasına sahip olması ve içerisinde söz konusu talepler için gerekli garanti haklarını belirtmiş olması şart. Ancak bu senaryoya göre Silverlight istemcileri için Cross-Domain Policy desteği vermeyen hiç bir sunucudan RSS içeriğini okumamız mümkün değil. Peki öyleyse ne yapacağız? Çözüm olarak biraz dolambaçlı bir yol olsa da, aşağıdaki şekilde görülen planı izleyebiliriz.

![blg176_Plan.gif](/assets/images/2010/blg176_Plan.gif)

Biliyoruz ki, Silverlight uygulamaları Asp.Net gibi Web uygulamaları içerisinde host edilebilmektedir. Planımıza göre Cross-Domain Policy sorunu ile karşılaşmayacak olan WCF Service'lerinin, Silverlight istemcilerinin talep edeceği RSS içeriklerini ele alması söz konusudur. Buna göre Silverlight istemcileri, RSS çıktılarına doğrudan talepte bulunmak yerine söz konusu taleplerini önce arada Proxy görevini üstlenen bir WCF servisine iletecektir. Bu WCF servisi, ilgili adres bilgisini alarak Feed çıktısını talep edecek ve elde ettiği içeriği tekrardan Silverlight tarafına gönderecektir.

Şekildeki plana göre 1 ve 2 numaralı iki adet WCF servisi söz konusudur. Bunlardan hangisinin seçileceği tamamen tercihe bağlıdır. İstersek Silverlight uygulaması ile aynı Domain içerisinde yer alan bir WCF servisini, istersek IIS üzerinde konuşlandırılan ayrı bir WCF servisini kullanabiliriz. Tabi IIS üzerinde host edilen bir WCF Servisi söz konusu ise, Silverlight istemcisi ile olan iletişiminin güvenlik sorununa takılmaması için ClientAccessPolicy.xml kullanılması gerekecektir. Tercih tamamen geliştiriciye bağlıdır. Ancak aynı Web sunucusu üzerinde yer alan birden fazla Silverlight istemcisi söz konusu ise ilgili servisin IIS altında konuşlandırılması daha çok tercih edilebilir.

> Bu noktada hazır olarak Siverlight istemcilerine hizmette bulunabilen Feed servislerinden de yararlanabileceğimizi belirtmek isterim. [Reading data and RSS with Silverlight and no cross-domain policy](http://timheuer.com/blog/archive/2008/06/03/use-silverlight-with-any-feed-without-cross-domain-files.aspx) başlıklı yazıda Tim Heuer söz konusu servislerden bahsetmektedir.

Ben örneğimizde hız kesemeden devam edebilmek adına, aynı uygulamaya Silverlight destekli bir WCF servisini ekleyerek ilerlemeyi tercih ettim. İşte FeedReaderService isimli Silverlight servisinin kod içeriği.

> Silverlight destekli WCF Servicelerinin nasıl geliştirileceğini [Screencast - Silverlight Enabled WCF Services](https://www.buraksenyurt.com/post/Screencast-Silverlight-Enabled-WCF-Services) isimli görsel dersten takip edebilirsiniz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Syndication;
using System.Web.Services.Protocols;
using System.Xml;

namespace RSSReaderim.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class FeedReaderService
    {
        [OperationContract]
        public List<SyndItem> ReadRss(string address)
        {
            List<SyndItem> feedItems = null;           
            try
            {
                // XmlReader.Create metodu parametre olarak address bilgisini almaktadır. Elde edilen Xml içeriği Load metodu yardımıyla çekilir ve SyndicationItem tipinden olan Items koleksiyonu çekilir.                
                var syndicationItems =SyndicationFeed.Load(XmlReader.Create(address)).Items;
                // SyndicationItem örneklerinin her biri ele alınıp yeni bir SyndItem örneklenmesinde kullanılır.
                feedItems = (from syndicationItem in syndicationItems
                             select new SyndItem
                             {
                                 Title = syndicationItem.Title.Text,
                                 PublishDate = syndicationItem.PublishDate.DateTime,
                                 Summary = syndicationItem.Summary.Text,
                                 Link=syndicationItem.Links[0].Uri
                             }
                           ).ToList();
            }
            catch(Exception excp)
            {
                throw new SoapException("Bir hata oluştu", new XmlQualifiedName("RssReadError"), excp);
            }
            return feedItems;
        }
    }

    // SyndicationItem tipi serileştirme sorununa neden olduğundan araya bir Surrogate tip alınmıştır. Bu tip içerisinde Silverlight tarafı için gerekli temel Feed bilgileri yer almaktadır.
    public class SyndItem
    {
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public string Summary { get; set; }
        public Uri Link { get; set; }
        //TODO: Diğer bilgilerde getirilmelidir. Örneği yazar bilgisi, son güncellenme tarihi veya kategoriler.
    }
}
```

Servis kodunda dikkat edilmesi gereken en önemli noktalardan birisi, ReadRss metodunun geriye SyndItem tipinden generic bir List koleksiyonu döndürmesidir. Bu noktada akla şu soru gelebilir. Neden List gibi bir koleksiyon döndürmüyoruz?

![Wink](/assets/images/2010/smiley-wink.gif)

Aslında buradaki sorun SyndicationItem tipinin serileştirme işlemi sırasında çalışma zamanı hatasına neden olmasıdır. Serileştirmedeki bu sıkıntı bizi alternatif bir yola itmiştir. Bu sebepten örnekte bir Surrogate tip kullanılmaktadır. Bu işlemin ardından artık Silverlight tarafı için gerekli geliştirmeler yapılabilir. İlk etapta aynı Domain içerisindeki (bir başka deyişle aynı Solution içerisindeki) WCF Servisinin Silverlight projesine eklenmesi gerekmektedir.

![blg176_AddServiceRef.gif](/assets/images/2010/blg176_AddServiceRef.gif)

Sonrasında ise istemci tarafı için gerekli kodlar yazılabilir. Yeni örnekte XAML içeriği de aşağıdaki gibi düzenlenmiştir.

```xml
<UserControl x:Class="RSSReaderim.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="337" d:DesignWidth="394" xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk">

    <Grid x:Name="LayoutRoot" Background="White">
        <Button Content="RSS Oku" Height="23" HorizontalAlignment="Left" Margin="313,76,0,0" Name="ReadRSSButton" VerticalAlignment="Top" Width="75" Click="ReadRSSButton_Click" />
        <sdk:Label Height="29" HorizontalAlignment="Left" Margin="8,12,0,0" Name="label1" VerticalAlignment="Top" Width="69" Content="RSS Adresi" FontSize="10" />
        <ListBox Height="180" HorizontalAlignment="Left" Margin="6,105,0,0" Name="RSSListBox" VerticalAlignment="Top" Width="382" ItemsSource="{Binding}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Vertical" Margin="2" Background="Black">
                        <TextBlock Text="{Binding Title}" Foreground="Gold" />
                        <TextBlock Text="{Binding Link}" Foreground="LightCyan" />
                    </StackPanel>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
        <TextBox Height="23" HorizontalAlignment="Left" Margin="8,47,0,0" Name="RSSTextBox" VerticalAlignment="Top" Width="380" />
        <sdk:Label Height="40" HorizontalAlignment="Left" Margin="8,297,0,0" Name="RSSInfoLabel" VerticalAlignment="Top" Width="380" FontSize="9" />
    </Grid>
</UserControl>
```

Bu kez ListBox.ItemTemplate içerisinde hem Title hemde Link bilgilerinin gösterilmesi sağlanmıştır. Yeni örnekte SnydItem isimli bir Surrogate tip söz konusu olduğundan ve bu tipin Title özelliği String tipten tanımlandığından, bir önceki XAML kodunda yer alan {Binding Title.Text} eşitlemesi kullanılmamalıdır. Gelelim kodlarımıza;

```csharp
using System;
using System.Windows;
using System.Windows.Controls;
using RSSReaderim.FeedReaderServiceSpace;

namespace RSSReaderim
{
    public partial class MainPage : UserControl
    {
        FeedReaderServiceClient client = null;
        public MainPage()
        {
            InitializeComponent();
            client = new FeedReaderServiceClient();
            client.ReadRssCompleted += new EventHandler<ReadRssCompletedEventArgs>(client_ReadRssCompleted);
        }

        private void ReadRSSButton_Click(object sender, RoutedEventArgs e)
        {
            client.ReadRssAsync(RSSTextBox.Text);
        }
        void client_ReadRssCompleted(object sender, ReadRssCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                RSSInfoLabel.Content = e.Error;
            }
            else if (e.Cancelled)
            {
                RSSInfoLabel.Content = "İşlem iptal edildi";
            }
            else
            {
                RSSListBox.ItemsSource = e.Result;
            }            
        }
    }
}
```

Kod içeriğinden de görüldüğü üzere WCF servisine ait Proxy tipinden yararlanılarak Feed içeriğinin asenkron olarak ortama çekilmesi işlemi gerçekleştirilmektedir. İşte örnek çalışma zamanı çıktılarından birisi.

![blg176_Runtime.gif](/assets/images/2010/blg176_Runtime.gif)

ve diğer bir örnek;

![blg176_Runtime2.gif](/assets/images/2010/blg176_Runtime2.gif)

Görüldüğü üzere RSS içerikleri başarılı bir şekilde getirilebilmektedir. Elbetteki örnekte eksik olan bir çok kısım vardır. Söz gelimi RSS ile ilişkili olarak daha çok verinin getirilmesi daha iyi olacaktır. Söz gelimi Feed'in sahibi olan siteye ait bilgiler. Diğer yandan eksik kalan önemli noktalardan biriside ListBox'ta bir öğe seçildiğinde ilgili Feed adresine nasıl gidileceğidir. Sonuç itibariyle Silverlight uygulaması tarayıcı üzerinde çalışmaktadır ve ilgili Feed içeriğinin Content verisinin gösterilmesini herkes isteyecektir. İşte size güzel bir araştırma konusu ve ödev

![Smile](/assets/images/2010/smiley-smile.gif)

Benden buraya kadar. Bir süre dinlenmeye çalışacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[RSSReaderim_RTM.rar (1,48 mb)](/assets/files/2010/RSSReaderim_RTM.rar)[Örnek Visual Studio 2010 Ultimate RTM sürümünde geliştirilmiş ve test edilmiştir]
