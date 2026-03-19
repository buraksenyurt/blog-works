---
title: "Kimdir Bu Travis?"
pubDate: 2019-04-29 07:24:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - .NET
  - Docker
  - GitHub
---

# Kimdir Bu Travis?
![Travis.jpg](images/Travis.jpg)

Geçen gün çalışma odamın artık ardiye haline gelmiş bir dolabını temizlemek üzere kolları sıvadım. Sayısız network kablosu, yazılabilir DVDler, müsvette notlar, bir kaç müzik CDsi, yüksek lisanstan kalma ders kitapları, kulaklıkları kayıp walkman, bataryası şişmiş Playstation Portable ve daha bir çok ıvır zıvır eşyayla doluydu. Hangileri gerekli hangileri gereksiz diye ayıklarken dönem dönem sebebsiz yere aldığım MP3 çalarlara denk geldim. Kocaman bir discman bile vardı. Ancak gözüm arkalarda köşeye sıkışmış 1 TBlık Harddisk'e takıldı. Zamanında E-book'lar, filmler ve müzikler için kullandığım bir disk olduğunu hayal mayal hatırlıyordum.

Eşyaları ayıklamayı bırakıp diskin içinde ne var ne yok bakmak istedim. Daha yeni aldığım Mac Mini (ona ahch-to adını verdim) ile açmaya çalıştım. Sierra onu pek sevmedi diyebilirim. Bunun üzerine Westworld'e geçmeye karar verdim. Ubuntu'nun açamayacağı disk yoktu. Diskin içinde beklediğimden de geniş bir arşiv vardı..Net 2.0/3.5 ile yazılmış projeler, makaleler için toplanmış belgeler, cv'min seksensekiz çeşidi, bloğun dönemsel yedekleri, fotoğrafçılık ile uğraştığım zamanlardan kalma klasörler ve diğer şeyler

Günün bir bölümünü o arşivleri tarayarak geçirdim. Sonra ID Taglerine kadar düzenlediğim MP3leri karışık sırada çalayım dedim. MP3 çalmayalı yıllar olmuştur. Hayatımız bulutlar üzerinde seyretmeye başladığından beri onu da Spotify gibi ortamlardan dinler olduk. Pek azımızın MP3 satın aldığını veya indirdiğini düşünüyorum. Hele ki mobil cihazlarda 1 kb yer bile tutmayan milyonlarca parçayı dinleme fırsatı varken. Şarkılar çalarken bende yeni araştırma konum olan Travis'le ilgili saturday-night-works çalışmalarından birisini yapıyordum. Hoş bir tesadüf olmalı ki çalışırken çalan parçalardan birisi de Travis'in 2001 yılında çıkarttığı The Invisible Band albümündeki Sing isimli şarkılarıydı. Konuyla ne alakası vardı bilemiyorum. Sadece isim benzerliği:) Aradan bir süre geçtikten sonra Travis-CI çalışmasını bloğuma not olarak düşmeye karar verdim.

Continuous Integration kaliteli ve sorunsuz kod çıkartmanın önemli safhalarından birisi. DevOps kültürü için değerli olan, Continuous Deployment/Delivery ile bir anılan CI'ın uygulanmasında en temel noktalar kodun sürekli test edilebilir olması ve ne kadarının kontrol altına alındığının bilinmesi. Başarılı bir Build için bu kriterlerin metrik olarak gerekli değerlerin üzerine çıkması şart. Ancak bu metriklere uyan bir Build, dağıtıma gönderilebilir bir aday sürüm haline gelebilir.

CI/CD hattını tesis ederken kullanılabilecek bir çok yardımcı ürün bulunuyor. Güncel olarak çalışmakta olduğum şirkette Microsoft'un VSTS'i kullanılmakta. Bunun muadili olabilecek Jenkins'de diğer bir alternatif olarak karşımıza çıkıyor. Benim öğrenmek istediğim ise Travis. Travis, Jenkins gibi kurulum ve bilgi maliyeti fazla olmayan, github ile kolayca entegre edilebilen, geliştirici dostu, ücretsiz bir CI ürünü olarak karşımıza çıkıyor. Amacım onu çok basit bir uygulama ile deneyimlemek.

## İhtiyaçlar (Yapılacaklar)

İlk başta ihtiyaçları ve yapılacakları belirlemek lazım.

- Öncelikle test edilebilir örnek bir uygulamaya ihtiyacımız var. Travis'in desteklediği dil ve platform yelpazesi oldukça geniş. (Ben.Net Core tabanlı bir kütüphaneyi ele almayı tercih ettim)
- Uygulamayı github üzerindeki bir proje ile ilişkilendirmek gerekiyor. Nitekim Travis, code base olarak GitHub tarafını kullanmakta.
- Travis'in Github entegrasyonu sayesinde code base üzerinde yapılan her Push sonrası otomatik olarak CI süreci başlayacak. Bu süreçte uygulamanın ihtiyaç duyduğu ortam paketleri yüklenip, build işlemi gerçekleştirilirken, aynı zamanda testler de koşulacak (CI süreci tamamen bulutta işleyecek)
- Uygulama için belki de en kritik ihtiyaç.travis.yml dosyası ve içeriği. Docker çalışma dinamiklerine benzer şekilde Travis ortamı için gerekli bilgileri içeren bir dosya olarak düşünebiliriz.

## Travis Tarafının Hazırlanması

Öncelikle [Travis'in ilgili sayfasına gidip](https://travis-ci.com) Github hesabımız ile kayıt olmamız gerekiyor. Sonrasında Acivate düğmesine basarak ilerliyoruz.

![04_39_credit_1.png](images/04_39_credit_1.png)

İzleyen adımda CI sürecine dahil etmek istediğimiz Github projesini seçiyoruz. Ben örnek için hello-travis isimli bir repo oluşturdum (Bu arada Travis'in yer yer çıkan logo'ları gerçekten çok tatlı)

![04_39_credit_2.png](images/04_39_credit_2.png)

Artık Travis ile Github projemiz birbirlerine bağlanmış durumdalar. Bunu Travis tarafındaki Repositories sekmesinden görebiliriz.

![04_39_credit_3.png](images/04_39_credit_3.png)

## Projenin Geliştirilmesi

Örnek olarak.Net Core tabanlı bir sınıf kütüphanesi geliştirmeye karar verdim. İlk olarak Github projesini Westworld'e (Ubuntu 18.04, 64bit) klonladım.

```bash
git clone https://github.com/buraksenyurt/hello-travis.git
```

Ardından aşağıdaki adımları izleyerek bir.Net Core klasör ağacı oluşturdum.

```bash
dotnet new sln
mkdir MathService
cd MathService
dotnet new classlib
mv Class1.cs Common.cs
cd ..
dotnet sln add ./MathService/MathService.csproj
mkdir MathService.Tests
cd MathService.Tests
dotnet new xunit
dotnet add reference ../MathService/MathService.csproj
mv UnitTest1.cs CommonTest.cs
cd ..
dotnet sln add ./MathService.Tests/MathService.Tests.csproj
touch .travis.yml
```

Öncelikle klonlanan klasörde bir Solution oluşturuyoruz. İsim vermediğimiz için hello-travis isimli bir solution dosyası üretilecektir. Ardından MathService isimli bir sınıf kütüphanesi üretiyor ve Class1.cs dosyasının adını Common.cs olarak değiştiriyoruz. Projeyi, solution içeriğine de ekledikten sonra bu kez MathService.Tests isimli xUnit tipinden bir test projesi oluşturuyoruz. Bu projeye MathService kütüphanesini referans edip son olarak test projesini solution'a bildiriyoruz. En son adımda dikkat edeceğiniz üzere.travis.yml isimli yaml dosyasını oluşturmaktayız.

![04_39_credit_4.png](images/04_39_credit_4.png)

Kodları aşağıdaki gibi geliştirebiliriz.

Common.cs

```csharp
using System;

namespace MathService
{
    public class Common
    {
        public bool IsNegative(int number)
        {
            return false;
        }

        public bool IsEven(int number)
        {
            return number % 2 == 0;
        }
    }
}
```

CommonTest.cs içeriği

```csharp
using System;
using Xunit;
using MathService;

namespace MathService.Tests
{
    public class CommonTest
    {
        private Common _common;

        public CommonTest()
        {
            _common = new Common();
        }

        [Fact]
        public void Negative_Four_Is_Negative()
        {
            var result=_common.IsNegative(-4);

            Assert.True(result,"-4 is negative number");
        }

        [Fact]
        public void Four_Is_Even()
        {
            var result=_common.IsEven(4);

            Assert.True(result,"4 is an even number");
        }
    }
}
```

## .travis.yml

Pek tabii Travis entegrasyonu için en kritik nokta bu dosya ve içeriği.

```bash
language: csharp
solution: hello-travis.sln
mono: none
dotnet: 2.1.502

script:
- dotnet build
- dotnet test MathService.Tests/MathService.Tests.csproj
```

Dosya içerisinde Travis'in çalışma zamanı ortamı için bir takım bilgiler yer alıyor. Bu bilgilere göre.Net Core 2.1.502 versiyonlu runtime üzerinde C# dilinin kullanıldığı bir uygulama söz konusu. Buna uygun bir makineyi Travis kendisi hazırlayacak (Travis'in log detaylarını incelemekte yarar var) script bloğunda yer alan ifadeler ise her push sonrası Travis tarafından icra edilecek olan işleri içeriyor. Önce build işlemi, sonrasında da test'in çalıştırılması. Örnekte kullanılan.Net çözümünün [orjinal github adresi](https://github.com/buraksenyurt/hello-travis) burasıdır.

## Çalışma Zamanı

İlk olarak hatalı çalışan testi bulunduran bir geliştirme yapmayı tercih ettim. Local'de test sonuçları aşağıdaki şekilde görüldüğü gibiydi.

```bash
dotnet test
```

![04_39_credit_5.png](images/04_39_credit_5.png)

Hal böyleyken kodları commit edip github sunucusuna push ile gönderdim.

```bash
git add .
git commit -m "fonksiyonal eklendir ve test kodları yazıldı"
git status
git push
```

Travis'e gittiğimde otomatik bir Build işleminin başladığını fark ettim.

![04_39_credit_6.png](images/04_39_credit_6.png)

Bir süre sonra Fail eden test nedeniyle Build işlemi de hatalı olarak sonlandı (Bu zaten istediğimiz ve beklediğimiz durum)

![04_39_credit_7.png](images/04_39_credit_7.png)

Log raporu sonuçları da aşağıdaki gibi oluştu.

![04_39_credit_8.png](images/04_39_credit_8.png)

Sonrasında hata alan test kodunu düzelterek ilerledim.

```csharp
using System;

namespace MathService
{
    public class Common
    {
        public bool IsNegative(int number)
        {
            return number<0;
        }

        public bool IsEven(int number)
        {
            return number % 2 == 0;
        }
    }
}
```

Westworld üzerinde dotnet test terminal komutu ile testlerin tamamının (sadece iki test var:P) başarılı olup olmadığını kontrol ettim. Ardından kodu commit edip tekrardan github'a push'ladım. Travis kısa süre içinde otomatik olarak yeni bir build işlemi başlattı. Bu sefer beklediğim gibi testler başarılı olduğundan build sonucu Passed olarak işaretlendi. İşte çalışma zamanına ait ekran görüntüleri.

![04_39_credit_9.png](images/04_39_credit_9.png)

![04_39_credit_10.png](images/04_39_credit_10.png)

![04_39_credit_11.png](images/04_39_credit_11.png)

Dikkat edileceği üzere tüm build işlemlerinin tarihçesini de görebiliyoruz. Bu tip loglar bizim için oldukça önemli.

![04_39_credit_12.png](images/04_39_credit_12.png)

## Ben Neler Öğrendim

Pek tabii bu çalışma sırasında da öğrendiğim bir çok şey oldu. Kabaca öğrendiklerimi şöyle sıralayabilirim.

- Travis'in CI sürecindeki yerini
- Travis ile bir Github reposunun nasıl bağlanabileceğini
- .travis.yml dosyasının içeriğinin nasıl olması gerektiğini ve içeriğindeki ifadelerin ne anlama geldiğini
- .Net Core tarafında xUnit test ortamının nasıl oluşturulabileceğini
- git push sonrası işletilen Build sürecinin izlenmesini

Böylece geldik bir maceranın daha sonuna. Sanırım Startup tadında bir projeye başlayacak olsak, takımın geliştirme sürecinde CI aracı olarak Travis'i alternatif olarak düşünebiliriz. Kullanımının kolay olması, github ile entegre çalışabilmesi ve bu nedenle push işlemleri sonrası build işlemlerinin otomatik olarak başlaması cezbedici özelliklerden. Tekrardan görüşünceye dek hepinize mutu günler dilerim.
