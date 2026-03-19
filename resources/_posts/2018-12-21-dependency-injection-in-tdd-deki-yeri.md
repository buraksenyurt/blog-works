---
layout: post
title: "Dependency Injection'ın TDD'deki Yeri"
date: 2018-12-21 10:30:00 +0300
description: "Test odaklı yazılım geliştirirken özellikle entegrasyon testlerinde(Integration Tests) yaşadığımız önemli sorunlardan birisi de test edilen nesnelerin diğer nesnelerle olan bağımlılıklarıdır. Söz gelimi test edilmek istenen birimin bağımlılıkları arasında servis çağrıları, veritabanı operasyonları veya uzak sunucu dosya hareketleri gibi işlemler söz konusuysa otomatik olarak çalıştırılan birim testlerinin CI sürecini sekteye uğratmaması için bir şeyler yapılması gerekebilir. Biliyorum çok karışık bir paragraf ile işe başladım. O yüzden problemi ortaya koymak adına aşağıdaki kod parçalarını göz önüne alalım."
categories:
  - csharp
tags:
  - csharp
  - bash
  - dotnet
  - web-service
  - authentication
  - generics
  - dependency-injection
  - visual-studio
  - github
  - dependency-management
---
# Dependency Injection'ın TDD'deki Yeri
![tdd_di_5.jpg](/assets/images/2018/tdd_di_5.jpg)

Merhaba Arkadaşlar,

Ne zamandır oturup da Lego yapmadığımı fark ettim. Her ne kadar fiyatları epeyce artmış olsa da geçenlerde dayanamayıp bir tane aldım. Bitirir bitirmez beni tatile götüreceğini düşündüğüm güzel bir karavan. Bloklardaki canlı renklerin tatlılığı, masmavi surf tahtası, uydu alıcısı, konforlu koltukları, panaromik tavanı, spor lastikleri ile bir saate kalmadan hazırdı bile.

Onunla biraz oynayıp, wrom wrom yaptım. Lego City'nin caddelerinden yavaş ve sakince akarak Malaga kıyılarındaki plajlara indim. Güneş batana kadar yüzdüm, surf yaptım. Bazen sahilden iyice uzaklaştım. İnsanların nokta gibi gözüktüğü mesafelere geldim. Şehre akşamın sakinliği çöküp karanlık bastırdıktan sonra plaja tekrar gelen gençlerin gitar melodilerinden çıkan tınıları dinlemeye başladım. Yaktıkları ateş etrafında toplanmış enerji dolu gençler. Karavanın hemen yanıbaşına kurduğum şezlongumda uzanmış onları izliyordum. Tebessümle. Buzlu kahvemden bir yudum aldım ve açık duran bilgisayarımda yanıp sönen cursor'u izlemeye başladım. Yazılmayı bekleyen kuyrukta kalmış bir şeyler vardı...

TDD süreçlerindeki birim ve entegrasyon testlerinde (Integration Tests) yaşadığımız önemli sorunlardan birisi, test edilmek istenen fonksiyonelliklerde kullanılan nesnelerin diğer nesnelerle olan bağımlılıkları sebebiyle yaşanmaktadır. Söz gelimi test edilmek istenen birimin bağımlılıkları arasında servis çağrıları, veritabanı operasyonları veya uzak sunucu dosya hareketleri gibi işlemler söz konusuysa, otomatik olarak çalıştırılan birim testlerinin CI (Continuous Integration) sürecini sekteye uğratmaması için bir şeyler yapılması gerekebilir. Biliyorum çok karışık bir paragraf ile işe başladım. O yüzden problemi ortaya koymak adına aşağıdaki kod parçalarını göz önüne alarak ilerlemeye çalışalım.

```csharp
using System;

namespace CodeKata.Services{
    public class CalculationService
    {
        private UserSerice _service;
        public CalculationService()
        {
            _service=new UserSerice();
        }
        public string GetInvoices()
        {
            if(_service.CheckRequest(GetCurrentContext()))
            {
                return "Invoice list";
            }
            else{
                return String.Empty;
            }
        }

        private string GetCurrentContext()
        {
            return "{\"operation\":\"Sum\"}";
        }
    }
}
```

CalculationService isimli sınıfın GetInvoices metoduna odaklanalım. UserService sınıfına ait nesne örneğinin CheckRequest fonksiyonu kullanılarak bir işlem gerçekleştirilmekte. Açıkça CalculationService sınıfı için bir bağımlılık bulunduğunu görebiliyoruz. Buna ilaveten UserService'in bir web servisi olduğunu düşünelim.

Sorun şu; "GetInvoices için yazılan testler çalışırken ya UserService çalışır durumda değilse?"

Bu Continuous Integration sırasında test aşamasının geçilmesine de engel teşkil edebilecek bir durum olabilir. İşte burada söz konusu servisin CheckRequest fonksiyonunun aslında istediğimiz tipte veriyi döndürecek şekilde kullanılması sorunu çözümleyebilir. Yani bağımlılığı test tarafında istediğimiz gibi enjekte edersek asıl testin çalışıp çalışmadığına odaklanabiliriz. Bunu yapmak için Dependency Injection'a uygun bir tasarıma geçmemiz gerekiyor.

Bildiğiniz üzere Dependency Injection mekanizması ile bu tip nesne bağımlılıklarının soyutlaştırılması mümkün. Temelde bunu üç farklı şekilde yapabiliriz. Yapıcı metod (Constructor) üzerinden, özelliğe (Property) kullanarak ve arayüz (Interface) tipinden yararlanarak. Yukarıdaki örneği düşünerek bu üç tekniğini nasıl kullanabileceğimizi incelemeye çalışalım.

Yapıcı Metod Kullanımı

Burada bağımlılığın nesne içerisine yapıcı metod üzerinden aktarımı söz konusudur. Öncelikle aşağıdaki gibi bir arayüze ihtiyacımız var.

```csharp
public interface IUserService
{
    bool CheckRequest(string request);
}
```

Buna göre CalculationService sınıfını da aşağıdaki gibi değiştirilmesi gerekiyor.

```csharp
using System;

namespace CodeKata.Services{
    public class CalculationService
    {
        private IUserService _service;
        public CalculationService(IUserService service)
        {
            _service=service;
        }
        public string GetInvoices()
        {
            if(_service.CheckRequest(GetCurrentContext()))
            {
                return "Invoice list";
            }
            else{
                return String.Empty;
            }
        }

        private string GetCurrentContext()
        {
            return "{\"operation\":\"Sum\"}";
        }
    }
}
```

Dikkat edileceği üzere yapıcı metoda parametre olarak IUserService arayüzünü veriyoruz. Bu arayüz, UserService sınıfının uygulaması gereken CheckRequest metodunu tanımlamakta. Dolayısıyla IUserService arayüzünü uygulayan herhangi bir sınıfı kullanabilir ve asıl servise gitmeye gerek kalmadan istediğimiz cevabı döndürerek testin ilerlemesini sağlayabiliriz. Bunun için test tarafında aşağıdaki gibi bir sınıfa ihtiyacımız olacak.

```csharp
public class FakeUserService
    : IUserService
{
    public bool CheckRequest(string request)
    {
        return true;
    }
}
```

CheckRequest operasyonunu her türlü true döndürecek hale getirdik. Dolayısıyla şöyle bir test metodu yazmamız artık mümkün.

```csharp
using System;
using CodeKata.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeKata.Tests
{
    [TestClass]
    public class CalculationServiceTests
    {
        [TestMethod]
        public void Should_Return_Current_Invoice_List_Is_Ok()
        {
            var calcService = new CalculationService(new FakeUserService());
            Assert.AreEqual("Invoice list", calcService.GetInvoices());
        }
    }
}
```

Should_Return_Current_Invoice_List_Is_Ok test metodunun içerisinde CalculationService nesnesini örneklerken parametre olarak FakeUserService sınıfını verdiğimize dikkat edelim. FakeUserService, test projesi içerisinde yer alıyor ve asıl servisin bu test için gerekli olan davranışını taklit ediyor. İşte çalışma zamanı sonucu (Örnekleri Visual Studio Code üzerinde ve MSTest türevli bir test projesi ile geliştirmekteyim)

![tdd_di_1.gif](/assets/images/2018/tdd_di_1.gif)

Property Setter Kullanımı

Şimdi enjekte mekanizmasını property üzerinden kuralım. Tek yapmamız gereken CalculationService sınıfını aşağıdaki gibi değiştirmek olacak.

```csharp
using System;

namespace CodeKata.Services{
    public class CalculationService
    {
        public IUserService UserService
        {
            get;
            set;
        }
        public string GetInvoices()
        {
            if(UserService.CheckRequest(GetCurrentContext()))
            {
                return "Invoice list";
            }
            else{
                return String.Empty;
            }
        }

        private string GetCurrentContext()
        {
            return "{\"operation\":\"Sum\"}";
        }
    }
}
```

Bu sefer yapıcı metod yerine getter ve setter bloklarına sahip IUserService tipinden bir özellik kullanarak bağımlılığın içeriye alınmasını sağlıyoruz. Doğal olarak ilgili test metodunun da aşağıdaki gibi değişmesi gerekiyor.

```csharp
[TestMethod]
public void Should_Return_Current_Invoice_List_Is_Ok()
{
    var fakeService=new FakeUserService();
    var calcService=new CalculationService()
    {
        UserService=fakeService
    };
    Assert.AreEqual("Invoice list", calcService.GetInvoices());
}
```

Test bu durumda da beklediğimiz gibi çalışacak ve asıl servisi hiç kullanmadan ilerleyecektir.

![tdd_di_2.gif](/assets/images/2018/tdd_di_2.gif)

Interface Kullanımı

Son olarak daha çok tercih edilen interface üzerinden nasıl bağımlılık enjekte edebileceğimize bir bakalım. Bu yöntem aslında property tabanlı bağımlılık tanımlamanın genişletilmiş bir versiyonu olarak düşünülebilir ve daha çok birden fazla bağımlılığın olduğu durumlarda ele alınır. Bizim örneğimizde bu tekniği aşağıdaki gibi icra etmemiz mümkün.

```csharp
public interface IUserServiceInjector
{
    IUserService UserService{get;set;}
}
```

İlk olarak IUserService arayüzünden referansları döndürecek bir başka sözleşme tanımlıyoruz. Ardından IUserServiceInjector isimli arayüz CalculationService sınıfına uyarlanıyor. Böylece CalculationService sınıfının ilgili servis davranışını IUserServiceInjector üzerinden belirtilen sözleşme çerçevesinde almasını sağlıyoruz.

```csharp
public class CalculationService
    : IUserServiceInjector
{
    public IUserService UserService
    {
        get;
        set;
    }
    public string GetInvoices()
    {
        if (UserService.CheckRequest(GetCurrentContext()))
        {
            return "Invoice list";
        }
        else
        {
            return String.Empty;
        }
    }
    private string GetCurrentContext()
    {
        return "{\"operation\":\"Sum\"}";
    }
}
```

Çalıştığımız nesnelerde birden fazla bağımlılığın söz konusu olması halinde ayrı ayrı Injector arayüzleri tasarlayıp uygulamamız mümkün. Son değişikliklere rağmen test metodunda özellik tabanlı örneğimizden farklı bir işleyiş söz konusu olmayacak.

![tdd_di_3.gif](/assets/images/2018/tdd_di_3.gif)

Test güdümlü geliştirme kapsamında Dependency Injection tekniğini bağımlılıkların olduğu her yerde ele almak mümkün. Her ne kadar kod okunmasını biraz zorlaştırsa da daha kolay test yazılmasını sağlamakta olduğu aşikar.

Örneklerde kullandığımız FakeUserService sınıfı terminolojide "Test Double" tipi olarak geçmekte (Hatta Stub türünden bir nesne olduğunu ifade edebiliriz) Test Double, basitçe bir üretim nesnesinin test amaçlı olarak değiştirilerek kullanılması olarak ifade ediliyor. Hatta kullanım şekline göre Dummy, Fake, Stubs, Spies ve Mocks gibi farklı türleri de bulunuyor. En çok kullanılan versiyonlar Mocks ve Stubs nesneleri. "Test Double" türlerini uygulamalarımızda daha kolay kullanabilmek de mümkün. Bu işe özel kütüphaneler bulunmakta.

Mock Nesne Kullanımı

Şimdi [Moq Nuget paketini kullanarak](https://github.com/moq/moq4) Stub yerine mock nesne kullanımını nasıl yapabileceğimize bir bakalım. Bu sayede mock kullanımını daha sade bir şekilde öğrenebiliriz. İlk olarak test projesine Moq4 paketini eklememiz gerekiyor. Bunun için Visual Studio Code terminalinden aşağıdaki komutu vermemiz yeterli.

```bash
dotnet add package Moq
```

Sonrasında test metodunda Moq kütüphanesini kullanmaya başlayabiliriz. Örneğimizde yapıcı metod odaklı enjekte mekanizmasını kullanabiliriz. Hatırlayacağınız gibi bağımlılığın yapıcı metod üzerinden aktarımı söz konusuydu.

```csharp
using System;
using CodeKata.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CodeKata.Tests
{
    [TestClass]
    public class CalculationServiceTests
    {
        [TestMethod]
        public void Should_Return_Current_Invoice_List_Is_Ok()
        {
            var mockUserService=new Mock<IUserService>();
            mockUserService.Setup(o=>o.CheckRequest("{\"operation\":\"Sum\"}")).Returns(true);
            var calcService=new CalculationService(mockUserService.Object);
            var result=calcService.GetInvoices();
            Assert.AreEqual("Invoice list",result);
            mockUserService.Verify(o => o.CheckRequest("{\"operation\":\"Sum\"}"), Times.Once());
        }
    }
}
```

Mock nesne örneğini oluştururken generic bir parametre kullanıyoruz. Burada mock'lamak istediğimiz tip IUserService arayüzü. Sonrasında Setup fonksiyonuna bir çağrı yapılıyor. Setup metodunda CheckRequest metodunu çağırıp ve sonucunda da geriye true döndürülmesini istediğimiz belirtiyoruz. Burada ilgili mock tipinin herhangi bir metodunun ele alabiliriz. Yapılan şey o fonksiyon çağırılmış da dönüşünde Returns içerisinde yazan değer döndürülmüş hissiyatını vermek. CalculationService sınıfına ait nesneyi örneklerken de yapıcı metoda parametre olarak mock nesnesini bir başka deyişle çalışma zamanında IUserService için örneklenen referansını atıyoruz. Sonrasında tek yaptığımız GetInvoices fonksiyonunu çağırmak. Kabul kriterinin kontrolünden sonra birde doğrulama işlemimiz var. Burada ilgili operasyonun sadece bir kere çağırılıp çağırılmadığını kontrol etmekteyiz. Test beklendiği gibi çalışacaktır.

![tdd_di_4.gif](/assets/images/2018/tdd_di_4.gif)

Pek tabii TDD tarafında Dependency Injection kullanımı bu örnek kodlarda olduğu kadar kolay olmayabilir. Özellikle legacy olarak anılan eski projleri sonradan Continuous Integration hattına soktuğumuzda test yazmak gerçekten başa bela olabilir. Çok fazla sayıda özellik barındıran Entity sınıfları ile yürüyen ve iş akışı karmaşık fonksiyonar için entegrasyon testleri yazmak istediğinizi düşünün. Mock nesne kullanımı biraz can acıtıcı olabilir ama uzun vadede rahat edileceği kesindir.

Böylece geldik bir makalemizin daha sonuna. Bu yazımızda Dependency Injection kavramını entegrasyon testlerinde değerlendirerek daha iyi anlamaya çalıştık. Size tavsiyem var olan entegrasyon testlerinizde belli başlı bağımlılıkları mock nesneler kullanarak ortadan kaldırmaya çalışmanız olacaktır. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.