---
title: "Visual Studio Code İçinde Unit Test Yazmak"
pubDate: 2018-11-26 21:15:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - .NET
  - LINQ
  - XML
  - Testing
  - Visual Studio
---

# Visual Studio Code İçinde Unit Test Yazmak
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Geçtiğimiz günlerde şirketimizin düzenlediği kişisel gelişim eğitimlerinden birisindeydim. Transaksiyonel Analiz'in konularından olan Ego üzerine kişiliğimizin parçası olan ve hayatımızı etkileyen iç karakterlerimizden bahsediliyordu. Yaklaşık üç saatlik eğitimde hoşça dakikalar geçirdik ve epey değişik bilgiler öğrendik. Özellikle uzman psikoloğun yer yer kullandığı görseller ve nokta atışı yapan karikatürler eğitimi çok keyifli hale getirmeye yetmişti. Üstelik uygulamalı olarak yaptığımız testler ile iç benliğimizdeki karakterlerin hangi noktalarda olduğunu da gördük. Eğitim sonrası masama döndüm ve bir kaç gün önce başladığım ama iş yoğunluğu sebebiyle yarım kalan yazımın başına geçtim. Derken eğitimde kullanılan Yiğit Özgür imzalı nefis karikatür geldi aklıma. Okur için tebessüm ettirici bir başlangıç olur diye düşündüm. Gelelim konumuza.

Şirketimizdeki Framework geliştirme ekibi, yazılan yeni nesil ürünlerde Visual Studio yerine Visual Studio Code ile çalışmamızı önermekte. Bir mecburiyet olmamakla beraber kendi Sprint Review toplantılarında gösterdikleri her şey Visual Studio Code üzerinden anlatılıyor. Çok eski ürünler için olmasa da (15 yıllık monolitik haline gelmiş devasa bir ERP uygulaması mesela) yeni nesil geliştirmeler için özellikle tercih ettiklerini ifade edebilirim. Bu da Visual Studio Code ile daha fazla haşır neşir olmamız ve el yatkınlığımızı arttırmamız gerektiği anlamına geliyor.

Ekibin bize verdiği yönergeleri takip ederek uygulamalara ait.Net çözümlerini kendi makinemde ayağa kaldırırken hem öğrendiğim hem de merak ettiğim şeyler oluyor. Pek çok şeyi Code'un terminalinden hallediyoruz. Özellikle git ile yakın temastayız. Pek tabii bir çok kullanışlı uzantıdan (extensions) da yararlanıyoruz. Benim en son baktığım konu ise Unit Test'lerin yazılması üzerineydi. Visual Studio Code'da bir.Net çözümü için klasör yapısı nasıl kurgulanır, test projesi nasıl oluşturulur, hangi test kütüphanesi kullanılabilir vb...Sonunda Microsoft dokümanlarını kurcalayarak bu işin en basit (ve birazda ilkel) haliyle nasıl yapılabildiğini öğrendim. Bir kaç deneme sonra evdeki West-World'un başına geçtim ve öğrendiklerimi uygulamaya koyuldum. Haydi başlayalım.

Solution Ağacının Oluşturulması

Amacımız bir kütüphane özelinde test projesi ve bağıntılarının nasıl kurgulanabileceğini görmek. Normalde Visual Studio gibi çok gelişmiş arabirimlerde bu oldukça kolay. Bir metoda sağ tıklayarak hızlı bir şekilde Unit Test projesini ve içeriğini oluşturabilirsiniz. Peki ya Code tarafında! Öncelikle bir dizi komut ile çözümün klasör ağacını ve içerisinde yer alacak projelerini oluşturacağız. Visual Studio Code terminalini kullanarak aşağıdaki komutları sırasıyla çalıştıralım.

```bash
mkdir UnitTestSample
cd UnitTestSample
dotnet new sln
mkdir TextService
cd TextService
dotnet new classlib
cd ..
dotnet sln add TextService/TextService.csproj
mkdir TextService.Tests
cd TextService.Tests
dotnet new mstest
dotnet add reference ../TextService/TextService.csproj
cd ..
dotnet sln add TextService.Tests/TextService.Tests.csproj
cd TextService.Tests
```

Öncelikle sistemimizde UnitTestSample isimli bir klasör oluşturduk ve içerisine girerek yeni bir Solution ürettirdik. Hemen ardından TextService isimli bir alt klasör daha oluşturduk. Bu klasörün içerisine girip bu sefer de bir sınıf kütüphanesi (Class Library) ürettirdik. Testlerini yazacağımız ve asıl işi yapan fonksiyonelliklerimizi burada biriktirebiliriz. Sonrasında solution dosyasına henüz üretilen TextService.csproj dosyasını ve dolayısıyla ilgili projeyi dahil ettik. Dolayısıyla ilgili sınıf kütüphanesinin projesini çözümün bir parçası haline getirdik. Birim testleri barındıracak olan TextService.Tests projesi için de benzer bir klasör yapısı söz konusu. Tek fark test projesini üretmek için "dotnet new mstest" komutlarından yararlanmış olmamız. Buna göre MStest kütüphanesini kullanan standart bir test projesi üretiliyor. Pek tabii test projesi TextService kütüphanesini kullanacağından proje referansını da eklememiz gerekiyor. Son adım olarak da TextService.Tests.csproj dosyasını Solution'a dahil ediyoruz. İşlemler bittiğinde kabaca aşağıdaki ağaç yapısını elde etmiş olmalıyız. TextService ve TextService.Tests aynı seviyede klasörlerdir.

![image.axd](images/image.axd)

Hemen şu notu da belirtelim; MSTest kullanmak zorunda değiliz. Pek çok bağımsız test kütüphanesi var. Örneğin Microsoft dokümanlarına baktığımızda NUnit ve xUnit örneklerinin olduğunu da görebiliriz.

Esas Sınıf

Şimdi TextService projesinde basit bir sınıf kullanalım. Stringer isimli örnek sınıf bir metin ile ilgili değişik işlemlere ev sahipliği yapacak (güya) Bir fonksiyonu da metin içerisinde belli bir karakterden kaç tane olduğunun hesaplanması. Sınıfın ilk halini aşağıdaki gibi tasarlayabiliriz. Şimdilik metoda ait bir implemantasyonumuz bulunmuyor. Yani kasıtlı olarak NotImplementedException fırlatan bir metodumuz var.

```csharp
using System;

namespace TextService
{
    public class Stringer
    {
        public int FindCharCount(string content,char character){
            throw new NotImplementedException("Makine soğuk henüz :P");
        }
    }
}
```

Test Sınıfı

Stringer sınıfının ilgili Test sınıfını da malumunuz TextService.Tests projesi altında yazmalıyız. StringerTest sınıfının ilk hali de aşağıdaki gibi olsun.

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextService;

namespace TextService.Tests
{
    [TestClass]
    public class StringerTest
    {
        private readonly Stringer _poo;
        public StringerTest()
        {
            _poo=new Stringer();
        }

        [TestMethod]
        public void Text_Should_Contain_1_S()
        {
            var result=_poo.FindCharCount("",'s');
            Assert.AreNotEqual(0,result);
        }
    }
}
```

TestClass ve TestMethod nitelikleri ile bezenmiş standart bir Unit Test sınıfı. Text_Should_Contain_1_S isimli örnek test metodu, FindCharCount fonksiyonu için bir kabul kriterine sahip. Buna göre s karakterinden en az bir tane olmalı. Testi çalıştırmak için test projesinin klasöründe olmamız ve aşağıdaki komutu vermemiz yeterli.

```bash
dotnet test
```

![image.axd](images/image.axd)

Revize

İlk test sonucu tahmin edileceği üzere bir Exception ile sonlanmış durumda. O halde gelin FindCharCount metodunu bir kaç vakayı karşılayabilecek şekilde tekrardan yazalım.

```csharp
using System;
using System.Linq;

namespace TextService
{
    public class Stringer
    {
        public int FindCharCount(string content,char character){
            if(String.IsNullOrEmpty(content))
                throw new ArgumentNullException("Bir içerik girilmeli");
            else{
                if(content.Length>10)
                    throw new ArgumentOutOfRangeException("Çok uzun, kısalt biraz");
                else{
                    var count=0;
                    foreach(var c in content)
                    {
                        if(c==character)
                            count++;
                    }
                    return count;
                }
            }
        }
    }
}
```

Bu kez bir kaç durum oluşabilir gibi. Metnin boş gelmesi veya örneğin 10 karakterden fazla olması hallerinde fonksiyonumuz uygun bir Exception ile çağıran tarafı cezalandırmakta. Ancak bu koşullar gerçekleşmediyse bir karakter sayımı yapılmakta ve parametre olan gelen harften kaç tane olduğu bulunmakta. Şimdi yazabileceğimiz bir kaç test metodu daha var. Test sınıfını aşağıdaki gibi değiştirerek devam edelim.

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TextService.Tests
{
    [TestClass]
    public class StringerTest
    {
        private readonly Stringer _poo;
        public StringerTest()
        {
            _poo=new Stringer();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Text_Is_Null_Or_Empty_Should_Throw_Exception()
        {
            var result=_poo.FindCharCount("",'s');
            Assert.AreNotEqual(0,result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Text_Length_Is_Too_Long_Should_Throw_Exception()
        {
            var result=_poo.FindCharCount("güzel güneşli bir günden kalan.",'s');
            Assert.AreNotEqual(0,result);
        }

        [TestMethod]
        public void Text_Should_Contain_Any_S_Character()
        {
            var result=_poo.FindCharCount("pilav",'s');
            Assert.AreEqual(true,result>0);
        }
    }
}
```

Exception beklediğimiz iki test metodu var. Ayrıca bir de içinde s karakteri barındırmasını beklediğimiz kabul kriterimiz. Exception beklediğimiz durumlar için bilinçli olarak ExpectedException niteliğini (attribute) kullandık. Bu iki test başarılı çalışacaktır. Ancak "pilav" kelimesinde "s" karakteri olmadığından 1 test hatalı sonuçlanacaktır. Testi tekrar çalıştırdığımızda aşağıdaki sonuçları elde ederiz.

![image.axd](images/image.axd)

Aslında bakmamız gereken başka durumlar da var. Örneğin gerçekten içinde s harfi olan bir metin için gerekli kabul kriterimizi de yazabiliriz. Bu şekilde değişken girdi parametreleri için ayrı ayrı test metodları yazmaktansa DataTestMethod ve DataRow nitelikleri yardımıyla çok sayıda farklı girdiyi kabul kriterlerine dahil edebiliriz. Nasıl mı? Aynen aşağıdaki gibi.

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TextService.Tests
{
    [TestClass]
    public class StringerTest
    {
        private readonly Stringer _poo;
        public StringerTest()
        {
            _poo=new Stringer();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Text_Is_Null_Or_Empty_Should_Throw_Exception()
        {
            var result=_poo.FindCharCount("",'s');
            Assert.AreNotEqual(0,result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Text_Length_Is_Too_Long_Should_Throw_Exception()
        {
            var result=_poo.FindCharCount("güzel güneşli bir günden kalan.",'s');
            Assert.AreNotEqual(0,result);
        }

        [DataTestMethod]
        [DataRow("password")]
        [DataRow("none")]
        [DataRow("sarı")]
        public void Text_Should_Contain_Any_S_Character(string text)
        {
            var result=_poo.FindCharCount(text,'s');
            Assert.AreEqual(true,result>0);
        }
    }
}
```

Bu sefer dikkat edileceği üzere 3 değişik metin için aynı test metodunu çalıştırdık. Sadece DataRow niteliğinin aldığı değeri Test metodunda parametre olarak ele almamız yeterli. Şimdi testlerimizi tekrar çalıştırırsak aşağıdaki sonuçları elde ettiğimizi görebiliriz.

![image.axd](images/image.axd)

Dikkat edileceği üzere Visual Studio Code arabirimi dışına çıkmadan MSTest kütüphanesini kullanarak birim testler içeren çözümler üretmemiz oldukça basit ve kolay. Pek tabii görsel olarak test sonuçlarını görebilmek de güzel olurdu. Visual Studio bu anlamda çok fazla ve güzel imkan sunuyor. Ne var ki Visual Studio Code için de açık kaynak olarak geliştirilmiş bir çok Test Explorer var. Bu arada "dotnet test" komutunun da gizemli parametreleri yok değil. Örneğin aşağıdaki komut ile Test sonuçlarının bir XML dosyası içerisine loglanmasını sağlayabiliriz.

```bash
dotnet test --logger trx
```

(Ben örnek deneme sonrasında TestResults klasörü altında trx uzantılı bir log dosyası elde ettim)

![image.axd](images/image.axd)

Ya da

```bash
dotnet test --list-tests
```

komutuyla var olan klasörde kullanılabilecek ne kadar test varsa terminal penceresine basabiliriz. Diğer komutları öğrenmek için ne yapmanız gerektiğinizi biliyorsunuz.

```bash
dotnet test --help
```

Bu yazımızda Test Driven Development odaklı yazılım geliştiren ekipler için Visual Studio Code tarafında basitçe nasıl ilerleyebileceğimizi gördüğümüzü düşünüyorum. Hatta hali hazırda var olan projelerinizdeki test senaryoları için Code tarafında denemeler yaparak el alışkanlığı kazanabilirsiniz. Biliyorsunuz ki test edilmemiş kod kaliteden uzaklaşmamız ve teknik borç bırakmamız için gerek ve yeter sebeplerden birisidir.Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
