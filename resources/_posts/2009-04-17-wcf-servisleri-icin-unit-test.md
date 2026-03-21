---
layout: post
title: "WCF Servisleri için Unit Test"
date: 2009-04-17 07:28:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
  - unit-test
---
Yazdığımız programların belirli kriterlere göre test edilmesi proje süreçleri içerisinde önem arz eden konulardan birisidir. Bilindiği üzere pek çok test çeşidi vardır. Bunların bir kısmı standart haline gelmiş tekniklerden oluşmaktadır. Örneğin web uygulamalarının beliri bir düzenle çalıştırılaran Request'ler ile test edilmesi. Stres testlerine tabi tutularaktan çok sayıda request sonrası web uygulamasının çalışmasının analiz edilmesi veya en basit anlamda bir programın çalışmasının ana parçalarından olan metodlarının, beklenen sonuçları verip vermediğinin araştırılması vb...

Nesne yönelimli programlama (Object Oriented Programming) dilleri ile geliştirilen uygulamalarda, özellikle metod bazında yapılan birim testlerini (Unit Test) WCF servislerine de uygulayabiliriz. Bu kısa yazımızda söz konusu işlemin Visual Studio ile birlikte gelen Test Tool'u yardımıyla nasıl gerçekleştirebileceğimizi incelemeye çalışacağız. Konu ile ilişkili olarak internette eğer araştırma yaparsanız, test amacıyla NUnit gibi araçlardan da faydalanabileceğimizi görebilirsiniz. Nitekim bazı durumlarda test yapan tarafta Visual Studio gibi bir araç olmayabilir ve [NUnit](http://www.nunit.org/index.php)aracı ile söz konusu analizler kolayca yapılabilir.

Şimdi elimizde kobay olarak kullanacağımız basit bir WCF servisi olduğunu düşünelim. Bu konu ile uğraşırken yaptığım araştırmalarda pek çok sitede, dört işlemin ele alındığına şahid oldum. Geleneği bozmadan devam edelim:) Servisimizi Visual Studio 2008 Professional ortamında geliştirebiliriz. Sözleşme (Contract) ve uygulayıcı tip içeriklerimiz aşağıdaki kodlarda görüldüğü gibidir.

IAlgebraService arayüzü (Interface)

```csharp
using System.ServiceModel;

[ServiceContract]
public interface IAlgebraService
{
    [OperationContract]
    double Toplama(double x, double y);

    [OperationContract]
    double Cikarma(double x, double y);

    [OperationContract]
    double Carpma(double x, double y);

    [OperationContract]
    double Bolme(double x, double y);
}
```

Uygulayıcı sınıf (AlgebraService.cs);

```csharp
public class AlgebraService 
    : IAlgebraService
{
    #region IAlgebraService Members

    public double Toplama(double x, double y)
    {
        return x + y;
    }

    public double Cikarma(double x, double y)
    {
        return x - y;
    }

    public double Carpma(double x, double y)
    {
        return x * y;
    }

    public double Bolme(double x, double y)
    {
        return x / y;
    }

    #endregion
}
```

Operasyonlarımızı AlgebraService.svc isimli servis üzerinden host ettiğimizi düşünecek olursak aşağıdaki web.config ayarlarını test amacıyla kullanabiliriz.

![blog2_1.gif](/assets/images/2009/blog2_1.gif)

Görüldüğü gibi çok basit olarak BasicHttpBinding bağlayıcı tipini kullanan dolayısıyla HTTP bazlı hizmet veren bir servisimiz var. Böyle bir servis geliştirildiğinde developer'ların test etmek amacıyla ilk yapacağı iş servisin tarayıcı üzerinden erişilebilirliğidir. Ki bu sayede geliştirici kendini biraz iyi hisseder:) Ancak bizim amacımız servis metodlarının birim olarak test edilmesidir. Bu amaçla Solution içerisinde yeni bir Test Project açmak ilk adımımız olacaktır. (Test Project şablon olarak Unit Test için gerekli nitelikleri içeren assembly'ın referansını otomatik olarak içerir. Dolayısıyla işimiz kolaylaştıran bir şablondur.)

![blog2_2.gif](/assets/images/2009/blog2_2.gif)

Test projesi içerisinde önemli olan noktalardan birisi, test edilmek istenen metodları kapsülleyen tipin bir servis sınıfı olmasıdır. Bir başka deyişle, test projesi söz konusu metodları deneyecekse eğer, AlgebraService isimli servise ulaşabiliyor ve metodlarını çağırabiliyor olmalıdır. Bu açıdan bakıldığında yapılan testin gerçekten birim testi olduğu açıktır.

İlerlemek için servisin, test projesine referans edilmesi yeterlidir.

![blog2_3.gif](/assets/images/2009/blog2_3.gif)

Şekildende görüleceği üzere amacıma ancak ikinci test projesinde ulaşabilmiş durumdayım:) Dikkat edilmesi gereken bir hususda proje referanslarında Microsoft.VisualStudio.QualityTools.UnitTestFramework assembly'ının var olmasıdır. Bu assembly içerisinde yer alan nitelikleri (attributes) birim testini yapacak sınıfı yazarken kullanıyor olacağız. Tahmin edileceği üzere bir Test Project oluşturulduğunda Wizard yardımıyla hızlı ve kolay bir şekilde ilerlenebilir. Ben örneğimizde Wizard kullanmadan ilerlemeyi denedim ve aşağıdaki test sınıfını oluşturdum.

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject2.Calculus;

namespace TestProject2
{
    [TestClass]
    public class CalculusUnitTest
    {
        private AlgebraServiceClient client = null;

        [TestInitialize]
        public void Baslat()
        {
            client = new AlgebraServiceClient();
        }

        [TestCleanup]
        public void Bitir()
        {
            if(client.State== System.ServiceModel.CommunicationState.Opened)
                client.Close();        
        }

        [TestMethod]
        [Description("Toplama testi")]        
        public void ToplamaTest()
        {
            Assert.AreEqual(3, client.Toplama(2, 1));
        }

        [TestMethod]
        public void CikartmaTest()
        {
            Assert.AreEqual(2, client.Cikarma(3, 2));
        }

        [TestMethod]
        public void BolmeTest()
        {
            Assert.AreEqual(5, client.Bolme(25, 5));
        }

        [TestMethod]
        public void CarpmaTest()
        {
            Assert.AreEqual(8, client.Carpma(2, 4));
        }
    }
}
```

İşte yazımızın en önemli kısmı bu sınıftır. Dikkat edileceği üzere sınıfımız TestClass isimli bir nitelikle imzalanmıştır. Diğer taraftan TestInitialize, TestCleanup, TestMethod isimli nitelikler yardımıyla imzalanmış olan metodlarda vardır. Nitelik kullanılması, çalışma zamanında yer alan bir ortamın, bir takım hazırlıklar yapacağı anlamına gelmektedir. Özetle bu nitelikler Visual Studio ortamında yer alan Test araçları tarafından çalışma zamanında değerlendirilmektedir. TestInitialize niteliği ile imzalanan metodlarda, test başlamadan önce yapılması gereken ön hazırlıklar yer almaktadır. Söz gelimi birim testlerine tabi olacak metodları içeren servis nesnesinin örneklenmesi gibi. TestCleanup niteliği ilede, testlerin bitmesi ile işletilmesi gereken kodları içeren metod imzalanmaktadır. Örneğin birim testleri için kullanılan servis örneğinin kapatılması burada yapılacak işlemlerden birisi olarak düşünülebilir.

TestMethod niteliği ile imzalanmış olan metod içerikleri ise, VS ortamındaki test aracı tarafından ele alınacak ve testleri yapılacak içeriklere sahiptir. Metod içeriklerine dikkat edilirse Assert tipine ait AreEqual fonksiyonlarının kullanıldığı görülmektedir. İlk parametre ile test sonucu beklenilen değer yazılırken, ikinci parametre ilede test edilecek fonksiyon çağrısı gerçekleştirilir. Cikarma metodunu test ettiğimiz yerde 2 sonucu beklenirken biz Cikarma metoduna (ki Cikartma olarak yazsam dilimize daha uygun olurmuş, hata yapmışım

![Frown](/assets/images/2009/smiley-frown.gif)

) 3 ile 2 parametrelerini göndermekteyiz. Yani aslında sonucun 1 olması gerekiyor. Bunu bilinçli olarak eklediğimi ve hata sonucu test aracının nasıl davrandığını görmek istediğimi belirteyim. Artık projemiz hazır. Build ettikten sonra çalıştırırsak, Visual Studio ortamında aşağıdaki görüntü ile karşılaşma ihtimalimiz yüksektir.

![blog2_6.gif](/assets/images/2009/blog2_6.gif)

Görüldüğü gibi CikartmaTest isimli metod haricindeki tüm testlerde beklenen sonuçlara ulaşılmıştır. Yapılan testlere ait sonuçlar aynı zamanda trx uzantılı dosyalarda klasör bazlı olaraktan saklanmaktadır. Aşağıdaki ekran görüntüsünde olduğu gibi.

![blog2_10.gif](/assets/images/2009/blog2_10.gif)

Not: Eğer servisi host eden uygulama (IIS, Windows Servisi veya diğer host seçeneklerinde yer alan uygulama çeşitleri olabilir) çalışmıyorsa, çok doğal olarak test gerçekleşmeyecek ve aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![blog2_4.gif](/assets/images/2009/blog2_4.gif)

Burada.Net Remoting'den bu yana bildiğimiz meşhur hata mesajını alırız. "No connection could be made because the target machine actively refused it"

![Cool](/assets/images/2009/smiley-cool.gif)

Hatta hata mesajlarından birisine çift tıklarsak aşağıdaki detaylı bilgi penceresinede ulaşma şansımız bulunmaktadır.

![blog2_5.gif](/assets/images/2009/blog2_5.gif)

Peki Visual Studio yerine NUnit aracını kullanarak birim testi gerçekleştirmek istersek...

İlk olarak NUnit programının sistemimizde kurulu olduğunu göz önüne alıyoruz. Sonrasında ise aynen bir önceki projemizde olduğu gibi servis referansını eklememiz gerekiyor. Bunlara ek olarak, NUnit test programının çalışma ortamına bilgi vermek amacıyla kullanacağımız nitelikleri için, projeye nunit framwork'ununde (blog girişini yaptığım tarihlerde varsayılan olarak C:\Program Files\NUnit 2.5\bin\net-2.0\framework\nunit.framework.dll klasöründe yer almaktadır) referans edilmesi gerekir.

![blog2_7.gif](/assets/images/2009/blog2_7.gif)

ServiceTestClass isimli sınıfın içeriği ise aşağıdaki gibidir.

```csharp
using TestLib.Calculus;
using NUnit.Framework;

namespace TestLib
{
    [TestFixture]
    public class ServiceTestClass
    {
        private AlgebraServiceClient client=null;

        [TestFixtureSetUp]
        public void Ornekle()
        {
            client = new AlgebraServiceClient();
            client.Open();
        }

        [TestFixtureTearDown]
        public void Kapat()
        {
            if(client.State== System.ServiceModel.CommunicationState.Opened)
                client.Close();
        }

        [Test]
        public void ToplamaTest()
        {
            Assert.AreEqual(5, client.Toplama(2, 3));
        }

        [Test]
        public void CikartmaTest()
        {
            Assert.AreEqual(7, client.Cikarma(10, 3));
        }

        [Test]
        public void BolmeTest()
        {
            Assert.AreEqual(3, client.Bolme(9, 3));
        }

        [Test]
        public void CarpmaTest()
        {
            Assert.AreEqual(6, client.Carpma(2, 3));
        }
    }
}
```

İlk projemizdekine benzer olaraktan buradada çalışma zamanını ilgilendiren bir takım nitelikler bulunmaktadır. Teste tabi olacak metodlarımız için Test niteliği, testin başlamasından önce yapılması istenen ön hazırlıklar için TestFixtureSetUp, test sonrasında yapılması istenen işlemler içinse TestFixtureTearDown nitelikleri ele alınır. Yine beklenen değerleri tespit etmek amacıyla Assert tipinin AreEqual metodundan yararlanılmaktadır. Projenin çalıştırılması halinde genellikle NUnit aracının otomatik olarak başlatıldığına şahit oldum. Bu nedenle projenin özelliklerinde aşağıdaki ayarlamayı yapmamız yeterli olacaktır.

![blog2_8.gif](/assets/images/2009/blog2_8.gif)

Görüldüğü gibi bu dll çalıştırıldığında NUnit tool'unuda başlatıyoruz. Command line arguments kısmında ise /run [dll adı] parametrelerini vererekten, NUnit aracının [dll adı] isimli assembly'daki testleri başlatmasını belirtiyoruz. Artık tek yapmamız gereken servise ait host uygulamanın çalıştığından emin olmak. Sonrasında TestLib.dll isimli assembly çalışıtırılırsa aşağıdaki ekran görüntüsünde olduğu gibi NUnit aracının çalıştığını ve testlerin yapıldığını görebiliriz.

![blog2_9.gif](/assets/images/2009/blog2_9.gif)

İşte bu kadar. Hemen son bir noktayı daha aklıma gelmişken belirteyim. Visual Studio Test Tool'unu kullandığımız senaryoda istenirse debug işlemleride yapılabilir.

Umuyorumki yararlı bir yazı olmuştur. Tekrardan görüşmek dileğiyle...

[WCFUnitTesting.rar (725,67 kb)](/assets/files/2009/WCFUnitTesting.rar)