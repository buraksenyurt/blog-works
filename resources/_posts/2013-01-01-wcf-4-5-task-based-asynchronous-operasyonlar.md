---
layout: post
title: "WCF 4.5–Task Based Asynchronous Operasyonlar"
date: 2013-01-01 13:51:00 +0300
categories:
  - wcf
  - wcf-4-5
tags:
  - windows-communication-foundation
  - task-parallel-library
  - task-based-asynchronous
  - async
  - await
  - .net-framework
---
Yaklaşık olarak 4 dakika 38 saniye…İzleyen yazıyı benim okuma hızım bu oldu. Aslında bu süre şu demek; Öğle arasına çıkmadan bir 4 dakika 38 saniye demek bu...Ya da geldikten sonra bir 4 dakika 38 saniye demek…Ya da sabah işe erken geldiğimizde ayırabileceğimiz bir 4 dakika 38 saniye demek...Ya da servisi/otobüsü/minibüsü beklerken ayırabileceğimiz 4 dakika 38 saniye demek. Hatta Facebook’ a, Twitter’ a, Linkedin’ e, Youtube’a bakmadan geçireceğimiz bir 4 dakika 38 saniye demek… E o halde ne duruyorsunuz?Ayırın işte o zamanı

[![kronometrei](/assets/images/2013/kronometrei_thumb.jpg)](/assets/images/2013/kronometrei.jpg)


![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_165.png)

Bilindiği üzere.Net Framework 4.5 ile birlikte altyapıya entegre olan async ve await anahtar kelimelerini kullanarak, task bazlı asenkron programlama teknikleri uygulanabilmektedir. Çok doğal olarak WCF 4.5 tarafında da bunun bir yansımasını görmekteyiz. Visual Studio arabirimi üzerinden herhangibir WCF servis referansını istemci uygulamaya eklemeye çalıştığımızda Task bazlı operasyon desteği varsayılan olarak etkinleştirilmekte ve proxy tipi içeriğinde buna uygun metodlara yer verilmektedir. Dolayısıyla WCF (Windows Communication Foundation) servislerini kullanan istemciler, operasyon çağrılarında async ve await anahtar kelimelerinden de yararlanabilirler.

Servislerin asenkron çağrılar ile yürütülmesi, özellikle User Experience’ ın önemli olduğu uygulamalarda, ön planda yer alan konular arasındadır. Gelin bu konuyu oldukça basit bir örnek üzerinden ele almaya çalışalım. İlk olarak.Net Framework 4.5 versiyonunda bir WCF Service Application oluşturup içerisine aşağıdaki sınıf diagramında (Class Diagram) görülen tipleri ilave edelim.

[![tbawcf_1](/assets/images/2013/tbawcf_1_thumb.png)](/assets/images/2013/tbawcf_1.png)

OptimizationService sembolik olarak uzun süren bir optimizasyon işlemini üstelenecek şekilde planlanmıştır. Senaryo gereği istemciden bir lokasyon bilgisi almakta olan servis, bu lokasyon için en ideal yolu çıkartmaktadır. Sadece hayal ediyoruz tabi

![Open-mouthed smile](/assets/images/2013/wlEmoticon-openmouthedsmile_38.png)

Amacımız uzun süren bir işlem ile servis tarafının istemciye geç cevap dönmesini sağlamak ve asenkronluğu devreye almaktır.

OptimizationService içerisinde yer alan GetBestRoot operasyonu Location tipinden bir parametre alırken, geriye de Root tipinden generic bir List koleksiyonu döndürmektedir. Servis uygulamasındaki tiplerin içerikleri ise aşağıdaki gibidir.

Servis sözleşmesi (Service Contract);

```csharp
using System.Collections.Generic; 
using System.ServiceModel;

namespace AzonServiceApp 
{ 
    [ServiceContract] 
    public interface IOptimizationService 
    { 
       [OperationContract] 
        List<Root> GetBestRoot(Location yourLocation); 
    } 
}
```

Servis sınıfı;

```csharp
using System.Collections.Generic; 
using System.Threading;

namespace AzonServiceApp 
{ 
    public class OptimizationService 
        : IOptimizationService 
    { 
        public List<Root> GetBestRoot(Location yourLocation) 
        { 
            List<Root> roots=FindBestRoot(yourLocation); 
            return roots; 
        }

        private List<Root> FindBestRoot(Location yourLocation) 
        { 
            Thread.Sleep(10000);

            return new List<Root>{ 
                new Root{RootId=1,Latitude=34.5,Longitude=43.2,Altitude=500.50,Title="4ncü cadde batı köşesi"}, 
                new Root{RootId=2,Latitude=34.85,Longitude=43.2,Altitude=450,Title="moda sahil yolu"}, 
                new Root{RootId=3,Latitude=22.5,Longitude=43.2,Altitude=100,Title="iskele caddesi durağı"}, 
                new Root{RootId=4,Latitude=44.90,Longitude=12.90,Altitude=0,Title="iskelenin kendisi"} 
            }; 
        } 
    } 
}
```

Location tipi;

```csharp
using System.Runtime.Serialization;

namespace AzonServiceApp 
{ 
    [DataContract] 
    public class Location 
    { 
        [DataMember] 
        public int LocationId { get; set; } 
        [DataMember] 
        public string Title { get; set; } 
    } 
}
```

Root tipi;

```csharp
using System.Runtime.Serialization;

namespace AzonServiceApp 
{ 
    [DataContract] 
    public class Root 
    { 
        [DataMember] 
        public double Altitude { get; set; } 
        [DataMember] 
        public double Longitude { get; set; } 
        [DataMember] 
        public double Latitude { get; set; } 
        [DataMember] 
        public int RootId { get; set; } 
        [DataMember] 
        public string Title { get; set; } 
    } 
}
```

Eğer servis uygulamasını bu haliyle çalıştırıp test edersek, WCF Test Client uygulamasında yaklaşık olarak 10 saniyelik bir gecikme ile Root listesini alabildiğimizi görürüz (Nitekim GetBestRoot servis operasyonu içerisinde çağırılan FindBestRoot metodunda, Thread.Sleep ile 10 saniyelik bir gecikme uygulanmıştır)

[![tbawcf_6](/assets/images/2013/tbawcf_6_thumb.png)](/assets/images/2013/tbawcf_6.png)

Tabi asıl konumuz bizim geliştireceğimiz istemci uygulamalardaki task bazlı operasyon desteğidir. İstemci tarafını bir WPF Application olarak geliştirdiğimizi düşünebiliriz. Uygulamaya Add Service Reference seçeneği ile servisimizi eklemek istediğimizde, Advanced sekmesinden ulaşacağımız arabirimde yer alan Generate Task-Based Operations kutucuğunun varsayılan olarak işaretli olduğunu fark edebiliriz.

[![tbawcf_2](/assets/images/2013/tbawcf_2_thumb.png)](/assets/images/2013/tbawcf_2.png)

Bu duruma göre referansı eklediğimizde, istemci uygulama tarafında aşağıdaki sınıf diagramında yer alan tiplerin üretildiğini görürüz.

[![tbawcf_3](/assets/images/2013/tbawcf_3_thumb.png)](/assets/images/2013/tbawcf_3.png)

Dikkat edileceği üzere OptimizationServiceClient sınıfı içerisinde, geriye Task tipinden referans döndüren bir operasyon yer almaktadır; GetBestRootAsync. Bu dönüş tipi nedeniyle ilgili metod çağrısı awaitable’ dır. Dolayısıyla async ile işaretlenmiş bir metod içerisindeyken await ile çağırılabilir. Dilerseniz bu durumu test etmeye çalışacak şekilde arayüzümüzü geliştirmeye devam edelim. Bu amaçla, WPF (Windows Presentation Foundation) uygulamamızda yer alan MainWindow öğesinin XAML (eXtensible Application Markup Language) içeriğini aşağıdaki gibi düzenleyelim.

[![tbawcf_4](/assets/images/2013/tbawcf_4_thumb.png)](/assets/images/2013/tbawcf_4.png)

```xml
<Window x:Class="WpfClientApp.MainWindow" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Title="MainWindow" Height="250" Width="460"> 
    <Grid> 
        <Grid.RowDefinitions> 
            <RowDefinition Height="auto"/> 
            <RowDefinition Height="*"/> 
            <RowDefinition Height="auto"/> 
        </Grid.RowDefinitions> 
        <StackPanel Orientation="Horizontal" Grid.Row="0"> 
            <Button x:Name="btnGetRoot" Content="En iyi yol bilgisini getir" 
                    HorizontalAlignment="Left" Margin="2,2,2,2" Click="btnGetRoot_Click_1"/> 
            <Label Content="Yana Birşeyler Yaz"/> 
            <TextBox Margin="3,3,3,3" Width="228"/> 
        </StackPanel> 
        <Label x:Name="lblStatus" Grid.Row="2" Content="Durum bilgisi"/> 
        <DataGrid x:Name="grdRoots" Grid.Row="1" ItemsSource="{Binding}" AutoGenerateColumns="False"> 
            <DataGrid.Columns> 
                <DataGridTextColumn Binding="{Binding Path=RootId}" Header=""/> 
                <DataGridTextColumn Binding="{Binding Path=Title}" Header="Yer"/> 
                <DataGridTextColumn Binding="{Binding Path=Longitude}" Header="Boylam"/> 
                <DataGridTextColumn Binding="{Binding Path=Latitude}" Header="Enlem"/> 
                <DataGridTextColumn Binding="{Binding Path=Altitude}" Header="Deniz Seviyesinden Yükseklik"/> 
            </DataGrid.Columns> 
        </DataGrid>        
    </Grid> 
</Window>
```

Burada yer alan DataGrid kontrolünün içeriğini, servis üzerinden yapacağımız çağrı sonrası gelen Root[] referansı ile doldurmaya çalışıyor olacağız. Bu sebepten bir data bind işlemi uyguladık ve DataGrid kontrolünün kolonlarında da Root tipine ait özelliklere yer verdik (RootId, Title, Longitude, Latitude ve Altitude) Gelelim yazımızın can alıcı noktasına

![Sarcastic smile](/assets/images/2013/wlEmoticon-sarcasticsmile_14.png)

Kod içeriğini aşağıdaki gibi düzenleyelim.

```csharp
using System.Windows; 
using WpfClientApp.AzonReference;

namespace WpfClientApp 
{ 
    public partial class MainWindow : Window 
    { 
        OptimizationServiceClient proxy = new OptimizationServiceClient();

        public MainWindow() 
        { 
            InitializeComponent(); 
        }

        private async void btnGetRoot_Click_1(object sender, RoutedEventArgs e) 
        { 
            lblStatus.Content = "Bilgiler çekiliyor..."; 
            Root[] roots = await proxy.GetBestRootAsync( 
                new Location { 
                    LocationId = 1 
                    , Title = "Hasanpaşa" 
                } 
                ); 
            grdRoots.DataContext = roots; 
            lblStatus.Content = "Bilgiler çekildi..."; 
        } 
    } 
}
```

İlk dikkat çekici nokta bntGetRootClick1 olay metodunun async anahtar kelimesi ile işaretlenmiş olmasıdır. Bu işaretleme nedeniyle, ilgili olay metodu içerisinde asenkron yürütülebilecek bir operasyon çağrısı yapılabileceği de belirtilmiş olmaktadır. Nitekim Root[] dizisinin çekilmesi için GetBestRootAsync metoduna yapılan çağrıda, await anahtar kelimesine yer verilmiştir. Olay metodu başında ve veriler DataGrid kontrolüne bağlandıktan sonra da lblStatus kontrolü içerisinde kısa bilgilendirmeler yapılmaktadır.

İşin güzel yanı ise şudur; Asenkron olarak çağırılan servis metodunun işleyişi sırasında, Form, kullanıcı tepkilerine cevap verebilir durumdadır. Yani ekran üzerinde formu başka bir yere sürükleyebilir, içeride yer alan TextBox kontrolünde bir şeyler yazabiliriz

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_165.png)

Oysaki eskiden, Dispatcher’ lardan ve hatta daha eskiden de Method Invoker’ lardan yararlanarak ekran arayüzünün cevap verebilir olmasını sağlamaya çalışırdık. Kafa karıştırıcı kodlar ile uğraşmak zorunda kalırdık. Bir kontrol için “hadi neyse…” derken, aynı anda yapılması gereken asenkron çağrı sayısının arttığı durumlarda kod kalabalığı ve karmaşıklığını daha da fazlalaştırıdık. Aslında uygulanan yeni model ile basitleşen bu durumu kendi gözlerinizle görmeniz daha iyi olacaktır. Ben sadece bir ekran görüntüsünü koyabilebileceğim. Siz mutlaka örnek kodu indirim test etmeye çalışın.

[![tbawcf_5](/assets/images/2013/tbawcf_5_thumb.png)](/assets/images/2013/tbawcf_5.png)

Görüldüğü üzere async ve await anahtar kelimelerinden de yararlandığımız bu senaryoda, kod daha az karmaşık olmakla beraber, istemci arayüzünün de asenkron işleyiş sırasında cevap verebilir olması sağlanmıştır. WCF operasyonlarının Windows Phone 8 ([Şu adresteki](http://stackoverflow.com/questions/13173614/async-await-in-windows-phone-web-access-apis) tartışmaya da bir kulak verin) gibi cevap verebilir arayüz ihtiyaçları yüksek olan uygulama çeşitlerinde de kullanıldığı düşünüldüğünde, kazanılan kabiliyetin önemli olduğu aşikardır. Böylece geldik kısa bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_TaskBasedAsyncOperations.zip (167,69 kb)](/assets/files/2013/HowTo_TaskBasedAsyncOperations.zip)