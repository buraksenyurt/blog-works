---
layout: post
title: "C# 2.0 ile Partial Types (Kısmi Tipler)"
date: 2005-06-27 12:00:00 +0300
description: "Visual Studio.Net ile windows veya web uygulamaları geliştirirken, kod yazılması sırasında karşılaştığımız güçlüklerden birisi, tasarım kodları ile kendi yazmış olduklarımızın iç içe geçmeleridir. Bu zamanla kodun okunabilirliğini zorlaştıran bir etmendir. Bunun windows uygulamalarını veya asp.net uygulamalarını geliştirirken sıkça yaşamaktayız. Bununla birlikte, özellike soruce safe gibi ortamlarda farklı geliştiricilerin aynı sınıf kodları üzerinde eş zamanlı olarak çalışması pek mümkün değildir."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - aspnet
  - concurrency
  - generics
  - visual-studio
---
# C# 2.0 ile Partial Types (Kısmi Tipler)
Değerli Okurlarım Merhabalar,

Visual Studio.Net ile windows veya web uygulamaları geliştirirken, kod yazılması sırasında karşılaştığımız güçlüklerden birisi, tasarım kodları ile kendi yazmış olduklarımızın iç içe geçmeleridir. Bu zamanla kodun okunabilirliğini zorlaştıran bir etmendir. Bunun windows uygulamalarını veya asp.net uygulamalarını geliştirirken sıkça yaşamaktayız. Bununla birlikte, özellike soruce safe gibi ortamlarda farklı geliştiricilerin aynı sınıf kodları üzerinde eş zamanlı olarak çalışması pek mümkün değildir.

Visual Studio.2005 ile birlikte, sınıf (class), arayüz (interface) ve yapı (struct) gibi tipleri mantıksal olarak ayrıştırabileceğimiz ve farklı fiziki dosyalarda (veya aynı fiziki dosya üzerinde) tutabileceğimiz yeni bir yapı getirilmiştir. Bu yapının kilit noktası tiplerin partial anahtar sözcüğü ile imzalanmasıdır. Partial olarak tanımladığımız tipleri farklı fiziki dosyalarda (veya aynı fiziki dosya içerisinde) tutabiliriz. Burada önemli olan, çalışma zamanında yazmış olduğumuz tipin mutlaka tek bir bütün olarak ele alınıyor olmasıdır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Partial tipler, bir tipin bütününü oluşturan soyutsal parçalardır.

İşte bu makalemizde kısaca partial tiplerin nasıl kullanıldığını incelemeye çalışacağız. Şimdi Visual Studio.2005 ile geliştirdiğimiz aşağıdaki örneği göz önüne alalım. Yeni açtığımız bir Console uygulamasında projemize şekilden de göreceğiniz gibi VeriYonetim.Ozellik ve VeriYonetim.Metod adlı iki kaynak kod dosyası ekledik.

![mk126_1.gif](/assets/images/2005/mk126_1.gif)

Öncelikle buradaki amacımızdan bahsedelim. Veritabanı ile ilgili yönetim işlerini üstlenecek bir sınıf geliştirmek istiyoruz. Ancak bu sınıfın özelliklerini ve metodlarını ayrı fiziki kaynak kod dosyalarında tutacağız. Bu mantıksal ayrımı yapmamızın çeşitli nedenleri olabilir. Geliştiricilerin aynı sınıfın çeşitli mantıksal parçaları üzerinde bağımsız ama eş zamanlı olarak çalışmalarını isteyebiliriz. Çoğunlukla en temel nedemiz sınıf bütününü mantıksal olarak ayrıştırarak kodlamayı kolaylaştırmaktır. İşte bu amaçla VeriYonetim isimli sınıfımızın bu ayrı parçalarını tutacak iki fiziki sınıfı dosyasını projemize ekledik. Böylece sınıfımızı aslında iki mantıksal parçaya bölmüş olduk. İlk parçada sadece gerekli özellik tanımalamalarını ikinci parçada ise işlevsel metodları barındıracağız. Böylece kod geliştirme safhasında bu mantıksal parçaların iki farklı dosyada tutulmasını sağlamış oluyoruz.

![mk126_2.gif](/assets/images/2005/mk126_2.gif)

Tasarım zamanında sınıfımızı iki farklı parçaya ayırmış olsakta, kod geliştirme sırasında veya çalışma zamanında tek bir tip olarak ele alınacaktır. Yani sınıfın bütünlüğü aslında hiç bir şekilde bozulmamaktadır. Bu önemli bir noktadır. Şimdi ilk örneğimizin kodlarına kısaca bakalım.

VeriYonetim.Ozellikler.cs;

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace PartialClasses
{
    partial class VeriYonetim
    {
        private SqlConnection m_GuncelBaglanti;
        private SqlCommand m_SqlKomutu;

        public SqlConnection GuncelBaglanti
        {
            get { return m_GuncelBaglanti; }
            set { m_GuncelBaglanti = value; }
        }
        public SqlCommand SqlKomutu
        {
            get { return m_SqlKomutu; }
            set { m_SqlKomutu = value; }
        }
    }
}
```

VeriYonetim.Metodlar.cs;

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace PartialClasses
{
    partial class VeriYonetim
    {
        public VeriYonetim(string baglanti)
        {
            m_GuncelBaglanti = new SqlConnection(baglanti);
        }
        public void KomutHazirla(string sorguCumlesi)
        {
            m_SqlKomutu = new SqlCommand(sorguCumlesi, GuncelBaglanti);
        }
    }
}
```

Uygulama;

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace PartialClasses
{
    class Program
    {
        static void Main(string[] args)
        {
            VeriYonetim yonetici = new VeriYonetim("data source=localhost;database=AdventureWorks;integrated security=SSPI");
            yonetici.KomutHazirla("SELECT * FROM Customers");
        }
    }
}
```

VeriYonetim isimli sınıfımızı her iki kaynak kod dosyası içerisinde partial anahtar kelimesi ile tanımlıyoruz. Böylece önceden bahsetmiş olduğumuz mantıksal ayrıştırmayı gerçekleştirmiş olduk. Eğer MS ILDasm (Microsoft Intermediate Language DisAssembler) programı yardımıyla derlediğimiz uygulamamıza göz atarsak ayrı parçalar halinde geliştirdiğimiz VeriYonetim sınıfının tek bir tip olarak yazıldığını görürüz.

![mk126_3.gif](/assets/images/2005/mk126_3.gif)

Bizim partial sınıflar içerisine böldüğümüz özellik, alan ve metodlar burada tek bir çatı altında toplanmıştır. Partial sınıfları kullanırken elbetteki dikkat etmemiz gereken noktalar vardır. Örneğin partial sınıflar içerisinde aynı elemanları tanımlayamayız. Yukarıdaki örneğimizde özellikleri tuttuğumuz sınıf parçamıza metodları tuttuğumuz sınıf parçasında olan constructor (yapıcı metod) dan bir tane daha ekleyelim. Özellikle aynı metod imzalarına sahip olmalarına dikkat edelim. Bu durumda uygulamayı derlemeye çalıştığımızda aşağıdaki hata mesajını alırız.

![mk126_5.gif](/assets/images/2005/mk126_5.gif)

Bu hata bize, partial class'ların asında tek bir sınıfı oluşturan parçalar olduğunu ispat etmektedir. Bu teoriden yola çıkaraktan metodların aşırı yüklenmiş (overload) hallerini partial class'lar içerisinde kullanabileceğimizi söyleyebiliriz. Bu sefer özellikleri tuttuğumuz partial sınıfımız içerisine VeriYonetim tipimiz için varsayılan yapıcı metodu (default constructor) ekleyelim.

```csharp
partial class VeriYonetim
{
    public VeriYonetim()
    {
        m_GuncelBaglanti = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI");
    }
    // Diğer kodlar
}
```

Bu durumda uygulamamız sorunsuz şekilde derlenecek ve çalışacaktır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Partial sınıflar (classes) tanımlayabildiğimiz gibi partial arayüzler (interfaces) veya yapılarda (structs) tanımlayabiliriz.

Örneğin, parçaları aşağıdaki şekilden de görüldüğü gibi iki farklı fiziki kaynak dosyada tutulacak IVeriYonetim isimli bir arayüz oluşturmak istediğimizi düşünelim. IVeriYonetim.Metodlar.cs kaynak kod dosyası içerisinde arayüzü uygulayacak olan tiplerin içermesi gereken metodları bildireceğimizi farzedelim. IVeriYonetim.Ozellikler.cs kaynak kod dosyasında ise, bu arayüzü uygulayacak tiplerin içermesi gereken özellik bildirimlerini yapacağımızı varsayalım.

![mk126_6.gif](/assets/images/2005/mk126_6.gif)

Tek yapmamız gereken arayüzlerimizi mantıksal olarak ayrıştırmak ve partial anahtar kelimesini kullanarak kodlamak olacaktır.

IVeriYonetim.Ozellikler.cs dosyası;

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace PartialClasses
{
    partial interface IVeriYonetim
    {
        int MaasArtisOrani
        {
        get;
        set;
        }
    }
}
```

IVeriYonetim.Metodlar.cs dosyası;

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace PartialClasses
{
    partial interface IVeriYonetim
    {
        void Bilgilendir();
        int PersonelSayisi(string sqlCumlesi);
    }
}
```

Her iki interface tanımlamasıda aslında tek bir interface'in kendi kurduğumuz mantıksal mimari çerçevesinde ayrılmış soyut parçalardır. Keza, IVeriYonetim isimli arayüzümüzü her hangi bir sınıfa uyguladığımızda aşağıdaki şekilden de görüldüğü gibi arayüzümüzün parçaları değil bütünlüğü ele alınacaktır.

![mk126_7.gif](/assets/images/2005/mk126_7.gif)

Sınıflarda olduğu gibi interface'ler içinde IL kodunun verdiği görünüm benzer olacaktır. Arayüzü ilgili sınıfımıza uyguladığımızda partial bölümlerde yer alan tüm elemanların implementasyona dahil edildiğini görürüz.

```csharp
class VeriYoneticisi:IVeriYonetim
{

    #region IVeriYonetim Members

    // IVeriYonetim.Ozellikler.cs kaynak kod dosyası içerisindeki partial kısımdan gelen üyeler.
    public void Bilgilendir()
    {
        //
    }

    public int PersonelSayisi(string sqlCumlesi)
    {
        //
    }

    // IVeriYonetim.Metodlar.cs kaynak kod dosyası içindeki partial kısımdan gelen üyeler.
    public int MaasArtisOrani 
    {
        get
        {
            //
        }
        set
        {
            //
        }
    }
    #endregion
}
```

Tek bir çatı altında toplanmış kod içerisinde mantıksal olarak ayrı parçalara bölünmüş bir tek arayüz.

![mk126_8.gif](/assets/images/2005/mk126_8.gif)

Partial kısımların, sturct'larda kullanılış biçimleride sınıflar ve arayüzlerde olduğu gibidir.

Özellikle partial sınıflar için bir takım kurallar vardır. Bu kurallara göre partial sınıflardan her hangi biri abstract veya sealed olarak belirtilirse, söz konusu tipin tamamı bu şekilde değerlendirilir. Örneğin aşağıdaki uygulamada partial sınıflarımızdan birisi sealed olarak tanımlanmıştır. Bunun doğal sonucu olarak söz konusu olan tip sealed olarak ele alınır. Dolayısıyla bu tipten her hangi bir şekilde türetme işlemi gerçekleştirilemez. Örneğin aşağıdaki kod derleme zamanında hataya yol açacaktır.

```csharp
sealed partial class Islemler
{
}
partial class Islemler
{
}
class AltIslemler : Islemler
{
}
```

Peki Partial sınıfları birden çok defa sealed tanımlarsak ne olur? Örneğin yukarıdaki kodumuzda Islemler sınıfının her iki parçasınada sealed anahtar sözcüğünü uyguladığımızı düşünelim.

```csharp
sealed partial class Islemler
{
}
sealed partial class Islemler
{
}
```

Burada ki gibi bir kullanım tamamen geçerlidir. Derleme zamanında her hangi bir hata alınmaz. Parital tiplerin kullanımı ile ilgili bir diğer kısıtlamaya göre partial olarak imzalanmış bir sınıfı partial olamayan bir sınıf olarak tekrardan yazamayız. Yani aşağıdaki ekran görüntüsünde olduğu gibi Islemler isimli sınıfı hem partial olarak hem de normal bir sınıf olarak tanımlayamayız. Böyle bir kullanım sonrasında derleme zamanı hatasını alırız.

![mk126_9.gif](/assets/images/2005/mk126_9.gif)

Görüldüğü gibi sınıflarımızı, arayüzlerimizi veya yapılarımızı partial olarak tanımlamak son derece kolaydır. Tek yapmanız gereken partial anahtar sözcüğünü kullanmaktır. Asıl zor olan, bu tipleri bölme ihtiyacını tespit edebilmek ve ne şekilde parçalara ayırabileceğimize karar vermektir. Yani bir sınıfı gelişi güzel parçalara bölmektense bunun için geçerli bir sebep aramak son derece önemlidir. Ayrıca mantıksal bölümlemeyi çok iyi analiz etmemiz gerekir. Bu analizin en iyi çözümü sunabilmesi ise sizlerin proje tecrübenize, planlama ve öngörü yeteneklerinize bağlıdır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.