---
layout: post
title: "Xml Web Servislerine Giriş - 2"
date: 2004-09-30 12:00:00 +0300
categories:
  - xml-web-services
tags:
  - xml-web-services
  - csharp
  - dotnet
  - aspnet
  - xml
  - web-service
  - http
  - visual-studio
  - asmx
---
Bu makalemizde, bir Xml Web Servisinin Visual Studio ile nasıl oluşturulabileceğini ve bir web sayfası üzerinden nasıl çağırılıp kullanılabileceğini incelemeye çalışacağız. Visual Studio.Net ortamında bir web servisi geliştirmek için, ilk olarak New Project bölümünden, ASP.NET Web Service şablonu seçilir. Visual Studio.Net, yerel makinede bu web servisi için gerekli fiziki ve sanal klasörleri, otomatik olarak oluşturacaktır. Notepad editoründe yazdığımız örneğin aynısını, Visual Studio.Net ortamında gerçekleştireceğimizden, proje ismi olarak GeoWebServis'i kullanalım. Bu aynı zamanda web servisimizin varsayılan isim alanı (default namespace) olacaktır.

![mk99_1.gif](/assets/images/2004/mk99_1.gif)

Şekil 1. Yeni bir Web Servis projesinin eklenmesi.

Bu işlemin ardından Visual Studio.Net, web servisimiz için gerekli dosyaları oluşturur. Varsayılan olarak servisimiz, Service1.asmx adını alacaktır. Bununla birlikte, bu servise ait Code-Behind dosyasının kodlarıda Visual Studio.Net tarafından otomatik olarak hazırlanır. Visual Studio.Net, servisin kullanımına örnek teşkil edicek bir metodu da yorum satırları halinde sunmaktadır. (HelloWorld () metodu)

```csharp
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
namespace GeoWebServis
{
     public class Service1 : System.Web.Services.WebService
     {
          public Service1()
          {
             InitializeComponent();
          }
          #region Component Designer generated code
          private IContainer components = null;
          private void InitializeComponent()
          {           }
          protected override void Dispose( bool disposing )
          {
              if(disposing && components != null)
              {
                   components.Dispose();
              }
              base.Dispose(disposing);          
          }
          #endregion //        [WebMethod]
//        public string HelloWorld()
//        {
//            return "Hello World";
//        } 
       }
    }
}
```

İlk olarak, servisimizin adını değiştireceğiz. Bunun için öncelikle, Solution Explorer kısmında, Service1.asmx dosyasının ismini, GeoMat.asmx olarak değiştirelim.

![mk99_2.gif](/assets/images/2004/mk99_2.gif)

Şekil 2. Service isminin değiştirilmesi.

Şimdi ise, daha önce Notepad uygulamamızda yazdığımız kodları buradaki GeoMat.asmx.cs Code-Behind dosyamız içinde aynen yazıyoruz.

```csharp
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
namespace GeoWebServis
{
     [WebService(Namespace="http://ilk/servis/",Description="Geometrik Hesaplamalar Üzerine Metodlar İçerir. Ucgen, Dortgen gibi şekillere yönelik alan ve çevre hesaplamaları.",Name="Geometrik Hesaplamalar")]
     public class TemelIsler : System.Web.Services.WebService
     {
          private const double pi=3.14;
          [WebMethod(Description="Daire Alan Hesabı Yapar")]
          public double DaireAlan( double r)
          {
              return (r*r)*pi;
          }
          [WebMethod(Description="Daire Çevre Hesabı Yapar.")]
          public double DaireCevre( double r)
          {
              return 2*pi*r;
          }
     }
}
```

Projemizi derlediğimizde, Visual Studio.Net, web servisimizin Code-Behind dosyasını kullanarak, servisimiz için gerekli dll dosyasını otomatik olarak bin klasörü içerisinde oluşturacaktır. Hazır olan web servisini test etmek için, projeyi çalıştırmamız (Run) yeterlidir.

Bir web servisi uygulaması elbette tek başına bir anlam ifade etmez. Web servisleri, ancak istemciler tarafından kullanıldıkları takdirde anlam kazanacaktır. Bir web servisinin, herhangibir istemci uygulamada kullanılabilmesi yüzeysel olarak bakıldığında çok karmaşık değildir. İstemcinin herşeyden önce, web servisinin yer aldığı adrese (URL) bir referansta bulunması gerekir. Bu referansın geçerli olması halinde, bir sonraki adımda istemci uygulamanın servisin sahip olduğu yetenekleri bilmesi gerekmektedir. Bu mimarinin işleyişini ilerleyen makalelerimizde daha derinlemesine incelemeden önce basit olarak geliştirdiğimiz web servisini bir istemci uygulamada nasıl kullanacağımızı görelim.

İstemci uygulamamızı şimdilik Visual Studio.Net ortamında geliştireceğiz. Mimarinin temellerini daha iyi kavradıktan sonra bu işin komut satırı araçları ile nasıl gerçekleştirildiğini daha iyi anlayacağız. Şimdi, Visual Studio.Net ortamında bir uygulama oluşturalım. Bu uygulama bir Windows uygulaması, bir Asp.net uygulaması olabileceği gibi bir Mobil uygulamada olabilir. Şimdi, New Project bölümünden, Asp.Net Web Application tipini seçelim ve uygulamamızı Istemci ismi ile oluşturalım.

![mk99_3.gif](/assets/images/2004/mk99_3.gif)

Şekil 3. Yeni bir web uygulamasının açılması.

Sırada bir web servisini uygulamamıza ekleyeceğimiz en önemli kısım var. Bunun için Solution Explorer’ da projemize sağ tıklıyor ve açılan menüden Add Web Reference öğesini tıklıyoruz.

![mk99_4.gif](/assets/images/2004/mk99_4.gif)

Şekil 4. Web servisi için gerekli referansın eklenmesi.

Bu işlem sonucunda ekrana, web servisini ekleyebilmemiz için bir kaç yöntem sunan Add Web Reference dialog penceresi gelecektir.

![mk99_5.gif](/assets/images/2004/mk99_5.gif)

Şekil 5. Web servisinin aranması.

Servisimizi, yerel makinede geliştirdiğimiz için, Web services on the local machine bağlantısına tıklamamız halinde, localhost’ ta yer alan kullanılabilir tüm web servislerinin listesini elde ederiz. Bu linke tıkladığımızda, geliştirmiş olduğumuz Web Servisini ve bu servise ulaşabileceğimiz adresi elde ederiz.

![mk99_6.gif](/assets/images/2004/mk99_6.gif)

Şekil 6. Yerel sunucuda bulunan web servisi.

GeoMat isimli bağlantıya tıkladığımızda, web servisine ait bilgilerin yer aldığı ve web servisini test edebilmemizi sağlıyan pencere ile karşılaşırız. Hatırlatmak gerekirse, burada yer alan servise ve metodlara ait temel bilgiler WebService ve WebMethod nitelikleri sayesinde gerçekleştirilmiştir.

![mk99_7.gif](/assets/images/2004/mk99_7.gif)

Şekil 7. Servisimizin sağladıklarına bakılması.

Artık web servisimizi bulduğumuza ve ne tür işlemler gerçekleştirebildiğini öğrendiğimize, dolayısıyla servisi keşfettiğimize (Discovery) göre, bu servisi uygulamamıza Add Reference butonuna tıklayarak ekleyebiliriz. Bu işlemin ardından, Visual Studio.Net uygulamaya bazı yeni dosyalar ekleyecektir.

![mk99_8.gif](/assets/images/2004/mk99_8.gif)

Şekil 8. Servisimiz ile birlikte oluşturulan ek dosyalar.

Tüm bu dosyalar Web References sekmesinin altında yer almaktadır. Bu dosyaların ne işe yaradığını ve ne amaçla oluşturululduğunu incelemeden önce, projemize eklediğimiz web servisini uygulama içerisinde nasıl kullanacağımızı görelim. Bu amaçla basit Web Form’ umuzu aşağıdaki gibi tasarlayalım.

![mk99_9.gif](/assets/images/2004/mk99_9.gif)

Şekil 9. Web sayfamız.

Şimdi, Hesapla başlıklı butona basıldığında meydana gelecek işlemleri kodlayacağız. Burada, web servisimizindeki Alan ve Cevre isimli metodları çağıracak ve bu metodlara parametre olarak TextBox metin kutusu kontrolüne girilen değeri göndereceğiz. Daha sonra Web Servisimiz bu metin kutusundaki değeri ilgili metodlarda çalıştıracak ve işlemlerin sonucu olarak metodlardan dönen değerleri, web sayfamıza gönderecek. Bunu gerçekleştirebilmek için tek yapmamız gereken aşağıdaki kodları yazmak.

```csharp
private void btnHesapla_Click(object sender, System.EventArgs e)
{
     double r=Convert.ToDouble(txtYaricap.Text);
     localhost.GeometrikHesaplamalar gh=new Istemci.localhost.GeometrikHesaplamalar();
     lblAlan.Text=gh.DaireAlan(r).ToString();
     lblCevre.Text=gh.DaireCevre(r).ToString();
}
```

Burada belkide en önemli kısım, Web Servisimizde yer alan GemotrikHesaplamalar isimli sınıfa ait bir nesne örneğinin aşağıdaki kod satırı ile oluşturulmasıdır.

```csharp
localhost.GeometrikHesaplamalar gh=new Istemci.localhost.GeometrikHesaplamalar();
```

Web servisimizdeki metodlara işte bu nesne vasıtasıyla erişmekteyiz.

```csharp
lblAlan.Text=gh.DaireAlan(r).ToString();
lblCevre.Text=gh.DaireCevre(r).ToString();
```

Burada kullanılan web servisinin, global seviyedeki bir ağda yer aldığını düşünürsek, servis içindeki sınıfa ait nesnenin istemci makinede nasıl örneklenebildiğini anlamak önemlidir. Nitekim sınıfa ait nesne örneğininin oluşturulabilmesi, istemci makinede de, bu sınıfın olmasını gerektirir. Bu Remoting teknolojisinden gelen bir özelliktir. Bir Remoting uygulamasında, uzak hizmetleri sağlayan nesne modelleri (dll dosyaları), bu hizmetleri kullanmak isteyen makinelere kopyalanır ve uygulamalara referans olarak belirtilir. Oysaki web servislerinde durum farklıdır. Web servisimize ait sınıfı, istemci uygulamaya kopyalamadık. Peki nasıl oldu da bu servis sınıfı, istemci uygulamada kullanılabildi. İşte WSDL (Web Services Description Language) bu noktada devreye girerek bize yardımcı olmaktadır. Mimarinin daha derinlerine girerek bu konuyu net bir şekilde kavramadan önce uygulamamızın nasıl çalıştığına bakalım. Bunun için projeyi Run edelim ve metin kutusuna bir yarıçap değeri girelim. Sonuç aşağıdaki gibi olacaktır.

![mk99_10.gif](/assets/images/2004/mk99_10.gif)

Şekil 10. Web servisinin çalıştırılması sonucu.

Artık web servisimizi kullanan bir istemci uygulamamız var. Peki gerçekte, kamera arkasında olanlar neler. Nasıl oluyorda, web servisini kullanmak istediğimizde, (bu servis dünyanın başka ucundaki bir makinede olsa bile) bu servise ait bir nesne örneğini istemci uygulamada oluşturabiliyoruz? Hatta bu nesne nasıl oluyorda, web servisindeki metodları biliyor ve onları parametreleri ile birlikte çağırabiliyor? İşte tüm bu soruların cevabı için öncelikle mimariye daha derinlemesine bakmamız gerekiyor. Web servislerinin mimarisini ilerleyen makalelerimizde incelemeye çalışacağız. Tekrar görüşünceye dek hepinize mutlu günler.