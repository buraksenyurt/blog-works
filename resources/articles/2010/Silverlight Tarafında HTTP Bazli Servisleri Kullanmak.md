---
title: "Silverlight Tarafında HTTP Bazli Servisleri Kullanmak"
pubDate: 2010-07-12 00:55:00
categories:
  - Silverlight 4.0
  - WCF Eco System
  - WCF WebHttp Services
tags:
  - Silverlight 4.0
  - WCF Eco System
  - WCF WebHttp Services
  - C#
  - XML
  - .NET
  - LINQ
  - WCF
  - Silverlight
  - XAML
  - REST
  - JSON
  - HTTP
  - IIS
  - JavaScript
  - Generics
  - Visual Studio
  - Rc
---

# Silverlight Tarafında HTTP Bazli Servisleri Kullanmak
![blg175_Giris.jpg](images/blg175_Giris.jpg)

Merhaba Arkadaşlar,

Eğitmenlik yaptığım yıllarda Microsoft'un ders kitaplarında yer alan LAB çalışmalarını mümkün mertebe yapmaya ve yaptırmaya çalışırdım. Hatta çoğu zaman eğitimlere hazırlanırken sık sık bu lab çalışmalarını kendim yapar ve hatta ek ilaveler ile daha da eğlenceli hale getirmeye çalışırdım. Tabi bazen elimizde lab yapacağımız kitaplarımız olmazdı ki o ayrı bir hikaye.

Lab çalışmaları öğrencinin adım adım yapması gerekenleri söylerek, konunun en yalın haliyle anlaşılmasını sağlamakta önemli rol oynamaktadır. Lab çalışmalarındakine benzer konu anlatımları benimde özümsediğim ve faydalı bulduğum öğrenme tekniklerinden birisidir. İşte bu yazımızda da bu kültüre uymaya çalışarak ilerlemeye çalışıyor olacağız. Hedefimiz Silverlight uygulamalarından, HTTP tabanlı taleplere göre operasyonel hizmetlerde bulunan servisleri nasıl kullanabileceğimizi, en yalın haliyle görmek. Haydi o zaman lab için gerekli materyalleri değerlendirerek yola koyulalım.

Adım 0: Mevzumuz

Bilindiği üzere bazı servisler HTTP protokolü üzerinden GET, POST, PUT veya DELETE metod çağrıları ile kullanılabilmektedir. Bu anlamda WCF Eco System içerisinde yer alan [WebHTTP servisleri](https://www.buraksenyurt.com/archive.aspx#WCF-WebHttp-Services), söz konusu tipteki hizmetleri sunmak üzere WCF alt yapısı üzerine oturmuş bir model sunmaktadır. Çok doğal olarak Silverlight tabanlı istemciler de bu servislerin tüketicileri olabilirler. Bu tip servislerin kullanıldığı senaryolarda istemci tarafında herhangibir Proxy tipi söz konusu olmadığı için, HTTP GET,POST,PUT veya DELETE metodlarının manuel olarak hazırlanması ve gönderilmesi gerekmektedir. Silverlight tarafında bu işlemler için WebClient veya HttpWebRequest tiplerinden yararlanılabilmektedir. Biz bu yazımızda WebClient tipinden yararlanarak, IIS (Internet Information Services) üzerinde konuşlandırılmış basit bir WebHttp Service örneğinin nasıl kullanılabileceğini incelemeye çalışıyor olacağız.

Adım 1: WCF Rest Application Uygulaması ve Entity Data Model'in Oluşturulması

İşe ilk olarak WCF Rest Service Application şablonunda bir proje oluşturarak başlayabiliriz. Bildiğiniz üzere bu proje şablonu (Project Template) hali hazırda yüklü değilse Online Template'ler arasından install etmeniz gerekmektedir. Söz konusu örnekte Chinook veritabanında yer alan ve çok basit olarak ilerlemek istediğimizden sadece Album tablosunu içeren bir Entity Data Model kullanabiliriz. Aşağıdaki şekilde örneğimizde kullanmakta olduğumuz Entity Data Model yer almaktadır.

![blg175_Edm.gif](images/blg175_Edm.gif)

Adım 2: WebHttp Service Örneğinin Geliştirilmesi

Entities isimli WCF WebHttp Service sınıfımızın içeriğini ise aşağıdaki gibi düzenlediğimizi düşünebiliriz.

```csharp
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace ChinookDataPortal
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class Entities
    {
        [WebGet(UriTemplate = "Albums/All")]
        public List<Album> GetAlbums()
        {
            List<Album> albums = null;

            ChinookEntities entities = new ChinookEntities();
            albums=(from albm in entities.Albums
                    orderby albm.Title
                    select albm).ToList();

            return albums;
        }

        [WebGet(UriTemplate="Albums/{firstLetter}")]
        public List<Album> GetAlbumsByFirstLetter(string firstLetter)
        {
            List<Album> albums = null;

            ChinookEntities entities = new ChinookEntities();
            albums = (from albm in entities.Albums
                      where albm.Title.ToLower().StartsWith(firstLetter.ToLower())
                      orderby albm.Title
                      select albm).ToList();

            return albums;
        }
    }
}
```

Servis tipimiz iki operasyon içermekte olup her ikiside HTTP Get çağrılarına cevap verecek şekilde düzenlenmişlerdir. GetAlbums metoduna yapılan çağrılarda servis URL adresine Albums/All takısı eklenmelidir. Diğer yandan ilk harflerine göre albümleri listeleyen GetAlbumsByFirstLetter metodu, URL adresine Albums/{firstLetter} bilgisinin eklenmesini beklemektedir. Her iki metod ChinookEntities tipini kullanmakta ve basit LINQ sorguları ile sonuç üretmektedir. Servisimizi bu şekilde geliştirdikten sonra IIS altına Publish ederek devam edebiliriz.

Adım 3: IIS Publish

Publish işlemleri için aşağıdaki şekilde görülen Profile ayarlarını kullanabilirsiniz.

![blg175_PublishProfile.gif](images/blg175_PublishProfile.gif)

Bu ayarlara göre servisimizin IIS üzerinde yer alan Default Web Site isimli Application Pool altına dağıtılacağı belirtilmiş olunur.

Not: IIS üzerinden Convert To Application işlemini yapmanız gerekebilir.

Sonuç olarak IIS içerisinde aşağıdaki gibi servisin üretilmiş olması gerekmektedir. Bu arada örneği geliştirdiğimiz makinede Windows 7 Enterprise işletim sisteminin ve IIS 7.5.7600.16385 sürümünün olduğunu belirtelim.

![blg175_IIS.gif](images/blg175_IIS.gif)

Bu noktadan sonra Silverlight uygulamasının geliştirilmesi aşamına geçilecektir. Ancak öncelikle gerekli testleri yapılmasında yarar vardır.

[ChinookDataPortal.rar (46,54 kb)](assets/ChinookDataPortal.rar) [Örnek Visual Studio 2010 Ultimate RC ortamında geliştirilmiş ve test edilmiştir]

Adım 4: WebHttp Service Test

Silverlight tarafındaki uygulamamızı geliştirmeden önce servisimizi IIS üzerinden test etmemizde ve çalıştığından emin olmamızda yarar olacağı kanısındayım. İlk olarak yardım sayfasına ulaşıp ulaşamadığımızı öğrenelim. Bilindiği üzere WebHttp Service örnekleri aksi belirtilmedikçe hazır bir yardım sayfası sunmaktadır. Bu amaçla tarayıcı uygulamadan http://localhost/ChinookDataPortal/Entities/help şeklinde bir talepte bulunduğumuzda, aşağıdaki ekran çıktısı ile karşılaşmış olmalıyız.

![blg175_HelpPage.gif](images/blg175_HelpPage.gif)

Yardım sayfasının çalışıyor olması dışında servis tarafında yer alan operasyonel metodların da test edilmesinde yarar vardır. Örneğin tüm albümleri elde etmek için http://localhost/ChinookDataPortal/Entities/Albums/All şeklinde talepte bulunduğumuzda, aşağıdaki ekran görüntüsünde yer alan sonuçları elde etmiş olmamız gerekmektedir. Tabi veri içeriklerinde değişiklikler söz konusu olabilir. Ancak XML çıktısının şematik yapısının benzer olması gerekmektedir.

![blg175_AllAlbums.gif](images/blg175_AllAlbums.gif)

Son olarak örneğin Cake adı ile başlayan albümleri çekmek istediğimizi ve bu amaçla URL satırından http://localhost/ChinookDataPortal/Entities/Albums/Cake şeklinde bir talep gönderdiğimizi düşünelim. Bu durumda ekran çıktısının aşağıdakine benzer olması gerekmektedir.

![blg175_ByFirstLetter.gif](images/blg175_ByFirstLetter.gif)

Eğer bu sonuçları elde edebiliyorsak servisimizin çalıştığını ve Sliverlight tarafı için kullanılabilir olduğunu söyleyebiliriz. Lakin dikkat etmemiz gereken bir nokta daha vardır.

Adım 5: Client Access Policy Ayarları

Silverlight uygulamamızın farklı bir Domain içerisinde host edilmesine karşılık, IIS üzerinde gerekli Client Access Policy ayarlarının bulunması gerekmektedir. Bu nedenle IIS root klasörü altında yer alması gereken ClientAccessPolicy.xml dosyasının içeriğini aşağıdaki gibi düzenleyebiliriz.

```xml
<?xml version="1.0" encoding ="utf-8"?>
<access-policy>
  <cross-domain-access>
    <policy>
      <allow-from>
        <domain uri="*" />
      </allow-from>
      <grant-to>
        <resource path="/WorldWeatherService" include-subpaths="true"/>
        <resource path="/ChinookDataPortal" include-subpaths="true"/>
      </grant-to>
    </policy>
  </cross-domain-access>
</access-policy>
```

Burada görüleceği üzere ChinookDataPortal ve alt yollarına erişim izni verilmiştir. Artık Silverlight tarafını geliştirmeye başlayabiliriz.

Adım 6: Silverlight Application Projesinin Oluşturulması

Bu amaçla Visual Studio 2010 ortamında ConsumingHTTPBasedServices isimi ve Silverlight 4.0 tabanlı bir Application oluşturduğumuzu düşünelim. Söz konusu uygulamada RIA Service kullanılmayacağı için bu seçeneği pasif olarak bırakabiliriz. Bu işlem sonucu oluşturulan MainPage sayfasına ait XAML içeriğini ise aşağıdaki gibi geliştirebiliriz.

Adım 7: MainPage.Xaml içeriği ve Kodun Yazılması

```xml
<UserControl x:Class="ConsumingHTTPBasedServices.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="300" d:DesignWidth="517">

    <Grid x:Name="LayoutRoot" Background="White">
        <ListBox Height="203" HorizontalAlignment="Left" Margin="8,70,0,0" Name="AlbumListBox" VerticalAlignment="Top" Width="497" />
        <StackPanel Height="54" HorizontalAlignment="Left" Margin="9,10,0,0" Name="ButtonsStackPanel" VerticalAlignment="Top" Width="496" Orientation="Horizontal" />
    </Grid>
</UserControl>
```

MainPage içerisinde yer alan ListBox kontrolü içeriği A'dan Z'ye harfler ile doldurulacaktır. Herhangibir harfe basıldığında, WebHttp Service'imiz için bir HTTP Get talebi oluşturulacak ve sonuçların ListBox içerisinde gösterilmesi sağlanacaktır. Bu amaçla kod içeriğini aşağıdaki gibi geliştirmemiz yeterlidir.

```csharp
using System;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace ConsumingHTTPBasedServices
{
    public partial class MainPage 
        : UserControl
    {
        // WebHttp Servisine basit HTTP metodları ile talepte bulunabilmemizi sağlayan WebClient nesnesi tanımlanır
        WebClient client;

        public MainPage()
        {
            InitializeComponent();

            // WebClient nesnesi örneklenir
            client=new WebClient();
            // Belirtilen URL adresine yapılan talep sonucu gerçekleşecek okuma işlemi tamamlandığında(bir başka deyişle veri istemci tarafında indirildiğinde) devreye girecek olan olay metodu tanımlanır.
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);

            // A...Z Button üretimleri gerçekleştirilir
            for (int i = 65; i < 91; i++)
            {
                Button btn = new Button();
                btn.Width = 18;
                btn.Height = 18;
                btn.FontSize = 10;
                btn.Content = ((char)i).ToString();
                ButtonsStackPanel.Children.Add(btn);
                // Herhangibir Button tıklandığında
                btn.Click += (o, e) =>
                {                   
                    // Önce WebHttp Service' ne doğur yapılacak HTTP Get talebi için gerekli URI oluşturulur
                    Uri address = new Uri(String.Format("http://localhost/ChinookDataPortal/Entities/Albums/{0}", ((Button)o).Content));
                    // Belirtilen URI talebi asenkron olarak çalışan OpenReadAsycn metodu ile gönderilir
                    client.OpenReadAsync(address);                    
                };
            }
        }

        // URI ile belirtilen adres talebi gerçekleştirilip ilgili veri içeriği istemci tarafına indirildikten sonra devreye giren olay metodudur
        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // İçerik bir Stream olarak gelmektedir ve tasarlanan ChinookDataPortal WebHttp Servisi varsayılan olarak XML içerik göndermektedir.
            // Bu sebepten Stream XmlReader ile okunur
            XmlReader xReader = XmlReader.Create(e.Result);
            // XLINQ sorgusunun yapılabilmesi için XElement.Load metodu parametre olarak Stream' i kullanan XmlReader nesne örneğini alır
            XElement xElement = XElement.Load(xReader);
            // XLINQ sorgusu ile Title elementleri çekilir. XName.Get metodunun ikinci parametre XML Namespace' inin adıdır.
            var titles = from x in xElement.Elements().Elements(XName.Get("Title", "http://schemas.datacontract.org/2004/07/ChinookDataPortal"))
                         select x.Value;
            // Çekilen veri içeriği ListBox kontrolünün ItemsSource özelliğine bağlanır
            AlbumListBox.ItemsSource = titles;
        }
    }
}
```

> XElement tiplerini kullanabilmek ve XLINQ sorgularını yazabilmek için, Silverlight uygulamasına (ConsumingHTTPBasedServices.Web uygulamasına değil) System.Xml.Linq.dll Assembly'ının referans edilmesi gerekmektedir.

Adım 8: Silverlight Uygulamasının Test Edilmesi

Dilerseniz uygulamanın çalışma zamanı sonuçlarına hemen bakalım. Böylece çalışma zamanı testlerini yapmış oluruz. Örneğin A başlıklı Button kontrolüne bastığımızda, aşağıdaki ekran görüntüsündekine benzer sonuçları almış olmalıyız. Yani Title alanındakilerden A harfi ile başlayanların listesinin elde edilebiliyor olması gerekmektedir.

![blg175_Runtime1.gif](images/blg175_Runtime1.gif)

Görüldüğü üzere ListBox içeriği baş harfi A olan albüm adları ile doldurulmuştur. Hemen bu işlemin arkasından örneğin C başlıklı Button kontrolüne basarsak aşağıdaki sonuçlar ile karşılaştığımız görürüz.

![blg175_Runtime2.gif](images/blg175_Runtime2.gif)

Süper değil mi?

![Wink](images/smiley-wink.gif)

Özet

Tabi bu örnekte dikkat edilmesi gereken noktalardan birisi de, istemci tarafında herhangibir Proxy tipinin olmayışıdır. Bunun yerine HTTP Get metodu ile talepte bulunulmuş ve elde edilen Stream üzerindeki XML içeriği değerlendirilmiştir. Diğer yandan çok doğal olarak Servis tarafında kullanılan Entity Data Model içerisindeki tiplerin istemci tarafındaki karşılıkları bulunmamaktadır. Eğer bu tiplerin istemci tarafında ele alınması arzu edilirse açık bir şekilde oluşturulmaları gerekecektir. Tabi böyle bir senaryoda gelen XML veya JSON tipindeki içeriğinde ilgili tiplere dönüştürülmesi gibi bir işlem söz konusu olacaktır.

Ödev

![Smile](images/smiley-smile.gif)

Servisin XML yerine JSON (JavaScript Object Notation) formatında bir çıktı vermesi halinde, Silverlight tarafında gerekli olan kod düzenlemelerini yapınız.
Servis üzerinden HTTP Put metod ile güncelleme işlemi yapabilmenizi sağlayacak bir geliştirmeyi aynı örnek üzerinden yapmaya çalışınız.

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ConsumingHTTPBasedServices_RTM.rar (273,75 kb)](assets/ConsumingHTTPBasedServices_RTM.rar)[Örnek Visual Studio 2010 Ultimate RC Sürümü üzerinde geliştirişmiş ve RTM sürümü üzerinde test edilmiştir]
