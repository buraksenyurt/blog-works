---
layout: post
title: "C# 6.0–Yeni Esintiler"
date: 2014-04-03 13:04:00 +0300
description: "İngilizce kelime anlamı Build olan inşa etmek(yapmak, kurmak) üzerine bu aralar uzak uzak diyarlarda da yapılmakta olan konuşmalar da var. Evet tahmin ettiğiniz gibi Microsoft’ un Build etkinlikleri dolayısıyla yazılım dünyasında hareketli günler yaşanmakta. Yeni ürünler, var olan ürünlere eklenen yeni özellikler, gelecek ile ilişkili planlamalar ve diğerleri. Konuşulabilecek ve üzerinde durulabilecek pek çok konu var. Benim dikkatimi çeken nokta ise bir süredir varlığından haberdar olduğumuz ve şu anda Roslyn’ in End User Preview sürümü ile Visual Studio 2013 üzerinden anında inceleyebileceğimiz C# 6.0 dili ile ilişkili yeni kabiliyetler. Bu yazımızda söz konusu yeteneklerden bir kaçına kısaca değinmeye çalışacağım. Amacımız öncelikli olarak söz konusu bu yeteneklerin ne olduklarını kavrayabilmek."
categories:
  - csharp
  - csharp-6-0
tags:
  - csharp
  - csharp-6-0
  - entity-framework
  - json
  - async-await
  - generics
  - visual-studio
  - datatable
---
# C# 6.0–Yeni Esintiler
[![LegoBuild](/assets/images/2014/LegoBuild_thumb.jpg)](/assets/images/2014/LegoBuild.jpg) Merhaba Arkadaşlar,

Çok şanslı bir çocukluk geçirdim. 80li yıllarda daha minik bir çocukken Lego’ lar oyuncak dolabımdan eksik olmazdı. O zamanlar benim için çok kıymetliydiler. Tabi büyüdükçe başka başka şeyler önem kazanmaya başladı. Lego’ nun pabucu belki de dama atıldı. Ta ki bir oğlum olana kadar.

Şimdiler de 4lü yaşlarını yaşayan S (h) arp Efe’ nin en sevdiği oyuncakların başında geliyor Lego. Ülkemizdeki fiyatları her ne hikmetse yüksek olan Lego’ lardan çok fazla alamıyoruz belki ama işin güzel bir tarafı var. 80li yıllarda oynadığım ve Annem tarafından saklanan Lego parçaları günümüzdekiler ile de uyumlu. Yani var olanları yeniler ile bir arada kullanıp hayal gücümüze göre farklı farklı yapılar inşa edebiliyoruz.

İngilizce kelime anlamı Build olan inşa etmek (yapmak, kurmak) üzerine bu aralar uzak uzak diyarlarda da yapılmakta olan konuşmalar da var. Evet tahmin ettiğiniz gibi Microsoft’ un Build etkinlikleri dolayısıyla yazılım dünyasında hareketli günler yaşanmakta. Yeni ürünler, var olan ürünlere eklenen yeni özellikler, gelecek ile ilişkili planlamalar ve diğerleri. Konuşulabilecek ve üzerinde durulabilecek pek çok konu var. Benim dikkatimi çeken nokta ise bir süredir varlığından haberdar olduğumuz ve şu anda Roslyn’ in End User Preview sürümü ile Visual Studio 2013 üzerinden anında inceleyebileceğimiz C# 6.0 dili ile ilişkili yeni kabiliyetler. Bu yazımızda söz konusu yeteneklerden bir kaçına kısaca değinmeye çalışacağım. Amacımız öncelikli olarak söz konusu bu yeteneklerin ne olduklarını kavrayabilmek.

> Dilin şu anki Preview sürümüne eklenen özellikleri zaman içerisinde değişime uğrayabilir. Ayrıca son sürüme geldikten sonra bu kabiliyetlerin bana kalırsa kendisini endüstüriyel anlamda ispat etmesi de önemlidir.

Ön Hazırlıklar

C# veya VB dillerinin gelecek nesil versiyonunda planlanan yenilikleri şimdiden test etmek için Roslyn End User Preview kullanılabilir. İlgili sürümü [Bu adreste indirilebilir](https://connect.microsoft.com/VisualStudio/Downloads/DownloadDetails.aspx?DownloadID=52793) ve kendimizi esintilerin akışına bırakabiliriz. Ayrıca güncel C# ve VB dil gelişimlerini, özellik bazında [şu adresten](https://roslyn.codeplex.com/wikipage?title=Language%20Feature%20Status&referringTitle=Documentation) takip edebiliriz. Bu adresteki tabloda halen planlanan, tamamlanmış olan, uzun süredir var olan veya dilin bu versiyonunda düşünülmeyen kabiliyetler tablo halinde bilgimize sunulmaktadır. (Sık sık uğrayıp güncellemeleri takip etmekte yarar olduğu kanısındayım)

End User Preview’ u içeriğini indirip gelen vsix dosyasını yükledikten sonra yeni özellikleri hemen denemeye başlayabiliriz. Ayrı bir proje şablonu oluşturmamıza gerek yoktur. Ben yeni özelliklerden gözüme kestirdiklerimin bir kaçını incelemek üzere, basit bir Console uygulaması üzerinden ilerlemeye çalışacağım.

Auto Property Initializers

Auto Property uzun süredir kullanmakta olduğumuz bir dil kabiliyeti. Özellikle Entity Framework tarafında POCO (Plain Old CLR Object) tiplerin tanımlanmasında yaygın bir şekilde ele alınmakta. Ben de açıkçası çok uzun zamandır property tanımlamalarında get ve set bloklarını kullanmıyor, sınıf içi alanlar açıkça değer atama veya onlardan veri okuma işlemlerini yazmıyorum. C# 6.0 sürümünde planlanan ve şu anda Roslyn End User Preview'da test edebildiğimiz yeni yetenek ise söz konusu Auto Property'lerin ilk değerlerinin verilmesi ile alakalı. Bu anlamda aşağıdaki kod parçasını göz önüne alabiliriz.

```csharp
using System;

namespace NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            ConnectionManager manager = new ConnectionManager(); 
            Console.WriteLine("Client ID:{0}\nConnection String:{1}",manager.ClientID,manager.ConnectionString); 
            manager.ClientID = -1; 
            Console.WriteLine("New Client ID:{0}",manager.ClientID); 
        } 
    }

    #region Auto Property Initializers 
    class ConnectionManager 
    { 
        public string ConnectionString { get; } = "data source=.;database=Core;integrated security=sspi"; 
        public int ClientID { get; set; }= 99; 
    } 
    #endregion 
}
```

Bu kod parçasında görüldüğü üzere ConnectionString ve ClientID isimli özellikler tanımlandıkları satırda ilk değerlerine sahip olmaktalar. İfade noktalı virgül ile tamamlandığında hem özellik tanımını hem de bu özelliklere ait arka plan alanları (Backing Field) için ilk değerleri vermiş oluyoruz. Bu arada ConnectionString'in sadece okunabilir (Readonly) bir özellik olarak tanımlandığına dikkat edelim. Çalışma zamanı sonuçlar aşağıdaki gibidir.

[![cs6_2](/assets/images/2014/cs6_2_thumb.png)](/assets/images/2014/cs6_2.png)

Tabi arka planda yer alan IL (Intermediate Language) görüntüsüne bakmakta da yarar var ki o da şu şekildedir.

[![cs6_1](/assets/images/2014/cs6_1_thumb.png)](/assets/images/2014/cs6_1.png)

Dikkat edilmesi gereken nokta özellik değerlerinin ConnectionManager sınıfının yapıcı metodu (Constructor) içerisinde atanmış olmasıdır.

Primary Constructors

Bildiğiniz üzere yapıcı metodların pek çok farklı versiyonu bulunmakta. Static Constructor, Copy Constructor, Default Constructor ve tabi Overload edilmiş versiyonları. Hatta yapıcı metodların base ve this anahtar kelimelerinin de işin içerisine katılmasıyla üst sınıf veya aynı sınıftaki öncelikli yapıcı metoda parametre taşıyan versiyonları da mevcut. C# 6.0’ ın planlanmış yeni kabiliyetlerinden birisi de Primary Constructor olarak karşımıza çıkmakta. Buna göre aşağıdaki gibi bir sınıf inşası mümkün.

```csharp
using System;

namespace NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Segment rZone = new Segment("RED", 1900); 
            Console.WriteLine("{0} {1}",rZone.Code,rZone.Length); 
        } 
    }

    #region Primary Constructors

    class Segment(string code,int length) 
    { 
        public string Code { get; }=code; 
        public int Length { get; }=length; 
    } 
    #endregion 
}
```

Segment isimli sınıf inşa edilirken parantezler içerisinde iki değişken tanımlaması yapıldığı görülmektedir. Bu, aslında code ve length isimli parametreleri kullanan bir yapıcı metod (Constructor) tanımlamasıdır. Örnekte yapıcı metodun daha anlamlı hale gelmesi adına, ilgili metod parametrelerinin Read Only olarak ifade edilmiş Auto Property'ler de ilk değerler olarak kullanılması sağlanmıştır. Dolayısıyla Segment tipinden bir nesne örneği oluşturulurken, yapıcı metod içerisinde verilen değerler aynı zaman da Code ve Length özelliklerinin set edilmesinde kullanılmaktadır. Çalışma zamanı görüntüsü aşağıdaki gibidir.

[![cs6_3](/assets/images/2014/cs6_3_thumb.png)](/assets/images/2014/cs6_3.png)

Ama bizi asıl ilgilendiren kodun IL tarafına nasıl yansıdığıdır. İşte o görüntüler.

[![cs6_4](/assets/images/2014/cs6_4_thumb.png)](/assets/images/2014/cs6_4.png)

Dikkat edileceği üzere yapıcı metod için code ve length isimli argumanlar tanımlanmış ve bu argumanlara ait değerler Segment ve Code özelliklerine ait alanlara ldarg.1 ve ldarg.2 üzerinden atanmıştır.

Şu da bir gerçek ki normal şartlarda bu tip bir sınıfı aşağıdaki gibi yazardık.

```csharp
class SegmentV2 
{ 
    private string _code; 
    private int _length; 
    public int Length { get { return _length; } } 
    public string Code { get { return _code; } }

    public SegmentV2(string code,int length) 
    { 
        _code = code; 
        _length = length; 
    } 
}
```

Ama bu, Primary Constructor özelliğinin "kodu kısaltarak daha kolay ve kullanışlı hale getirmiştir" şeklinde yorumlanmasını gerektirmez.

Primary Constructor kullanıldığında dikkat edilmesi gereken bir durumda diğer yapıcıların nasıl yazılabileceğidir. Örneğin aşağıdaki gibi bir varsayılan yapıcı metod (Default Constructor) tanımı derleme zamanı hatasına neden olacaktır.

```csharp
class Segment(string code,int length) 
{ 
    public string Code { get; }=code; 
    public int Length { get; }=length;

    public Segment() 
    { 
    } 
}
```

[![cs6_5](/assets/images/2014/cs6_5_thumb.png)](/assets/images/2014/cs6_5.png)

Bu son derece doğaldır nitekim Primary Constructor sınıfı başlatmak için gerekli olan minimum argüman desenine sahip yapıcıyı tanımlamaktadır. Dolayısıyla aşağıdaki gibi bir kullanıma gidilmesi gerekecektir.

[![cs6_6](/assets/images/2014/cs6_6_thumb.png)](/assets/images/2014/cs6_6.png)

```csharp
using System;

namespace NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Segment gZone = new Segment(); 
            Console.WriteLine("{0} {1}", gZone.Code, gZone.Length); 
        } 
    }

    #region Primary Constructors

   class Segment(string code,int length) 
    { 
        public string Code { get; }=code; 
        public int Length { get; }=length;

        public Segment() 
            :this("GZONE",980) 
        { 
        } 
    }

    #endregion 
}
```

IL koduna baktığımızda varsayılan yapıcı metodun beklendiği gibi Primary Constructor'un işaret ettiği fonksiyonu çağırdığı görülmektedir.

[![cs6_7](/assets/images/2014/cs6_7_thumb.png)](/assets/images/2014/cs6_7.png)

Static Metodlar için using Bildirimi

Enteresan ve aslında hoşuma giden özelliklerden birisi de, static metodların tanımlandıkları tip adına ihtiyaç duymadan çağırılabilmeleri. Aynen aşağıda kod parçasında görüldüğü gibi.

```csharp
using System; 
using System.IO.Path;

namespace NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Static Metodların Kullanım

            var logFile=Combine(Environment.CurrentDirectory, "Logs.txt"); 
            Console.WriteLine(logFile);

            #endregion 
        } 
    } 
}
```

[![cs6_8](/assets/images/2014/cs6_8_thumb.png)](/assets/images/2014/cs6_8.png)

Dikkat edileceği üzere using System.IO.Path şeklinde bir bildirim yapılmıştır. Bu bildirim nedeniyle Path sınıfının static metodlarından Combine, tanımlandığı sınıf adı belirtilmeksizin kullanılabilmiştir. (Normal şartlarda Path.Combine şeklinde bir kullanım olduğunu hatırlayalım)

Bu kullanım şekli genişletme metodları (Extension Method) için de geçerlidir. Aşağıdaki kod parçasında olduğu gibi.

```csharp
using System; 
using System.Data; 
using NewFeatures.DataExtensions;

namespace NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Static Metodların Kullanım

            LoadFromExcel(new DataTable(), "");

            #endregion 
        } 
    }

    static class DataExtensions 
    { 
        public static DataTable LoadFromExcel(this DataTable Table,string Source) 
        { 
            throw new NotImplementedException(); 
        } 
    } 
}
```

DataExtensions sınıfı içerisinde yer alan LoadFromExcel genişletme metodunu, tip adı olmadan kullanabilmek için

using NewFeatures.DataExtensions;

şeklinde ki tanımlama yeterli olmuştur.

Decleration Expressions

Bu kabiliyeti anlamak için aşağıdaki kod parçasını göz önüne alarak işe başlamamız gerekiyor.

```csharp
using System;

namespace NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Decleration Expressions

            string cDate = "2014-04-04 10:30:00"; 
            DateTime result; 
            if(DateTime.TryParse(cDate, out result)) 
            { 
                // do something 
                Console.WriteLine(result.ToString()); 
            } 
            
            #endregion 
        } 
    } 
}
```

Tipik olarak string bir içeriğin DateTime tipine dönüşümünün TryParse metodu ile kontrollü hale getirildiği bir senaryo söz konusudur. Decleration Expressions sayesinde aynı kod parçası aşağıdaki gibi yazılabilir.

```csharp
using System;

namespace NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Decleration Expressions

            string cDate = "2014-04-04 10:30:00"; 
            if(DateTime.TryParse(cDate,out DateTime result2)) 
            { 
                Console.WriteLine(result2.ToString()); 
            } 
            
            #endregion 
        } 
    } 
}
```

Dikkat edilmesi gereken nokta tahmin edileceği üzere TryParse metodunun ikinci parametresinde bir değişken tanımının yapılmasıdır. result2 parametre olarak kullanılacağı yerde aynı zamanda değişken olarak tanımlanmıştır. IL içeriğine baktığımızda beklediğimiz gibi result2’ nin dışarıda bir local değişken olarak yerleştirildiği gözlemlenecektir.

[![cs6_9](/assets/images/2014/cs6_9_thumb.png)](/assets/images/2014/cs6_9.png)

Bu yetenek özellikle out gibi parametrelerin kullanıldığı yerde ön plana çıkmaktadır.

Dictionary Initializers

Dictionary tipinden bir koleksiyonu nasıl oluşturursunuz? Kuvvetle muhtemel aşağıdaki kod parçasında görüldüğü gibi.

```csharp
Dictionary<int, string> values = new Dictionary<int, string>   
{ 
    { 1,"Red" }, 
    {9,"Green" }, 
    {4,"Blue" } 
};
```

Aynı koleksiyonu yeni gelen Initilaization yapısı ile aşağıdaki şekilde de oluşturabilmekteyiz.

```csharp
Dictionary<int, string> valuesV2 = new Dictionary<int, string> 
{ 
    [1]="Red", 
    [9]="Green", 
    [4]="Blue" 
};
```

Doğrudan index bilgisini kullanarak Microsoft’ un kaynaklarına göre daha şık bir atama söz konusu. Ancak işin asıl dikkat çekici tarafı aşağıdaki gibi yapılabilen koleksiyon tanımı.

```csharp
using System; 
using System.Collections.Generic;

namespace NewFeatures 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Dictionary Initilaziers

            var values = new Dictionary<string, string> 
            { 
                $code="G-ZONE", 
                $length="1900", 
                $creationTime=DateTime.Now.ToLongTimeString(), 
                $identity=Guid.NewGuid().ToString(), 
                $color="Green" 
            };

            Console.WriteLine( 
                "{0}\n{1}\n{2}\n{3}" 
                , values.$code 
                , values.$creationTime 
                , values.$identity 
                , values.$color 
                );           

            #endregion 
        } 
    } 
}
```

Sanırım bu yazım şekli biraz daha dikkatinizi çekmiştir. values isimli generic Dictionary koleksiyonunun içeriği belirlenirken $keyName şeklinde bir kullanım söz konusudur. Bu kullanımın faydası, values değişkeninin key değerlerine erişirken kendisini göstermektedir. Her ne kadar şu anda intellisense bir yardımda bulunmasa da JSON vari bu yazım şekli oldukça kullanışlıdır. İşte çalışma zamanı sonuçları.

[![cs6_10](/assets/images/2014/cs6_10_thumb.png)](/assets/images/2014/cs6_10.png)

Başka Neler Var?

Henüz inceleme fırsatı bulamadığım Exception Filters ve try…catch…finally bloklarında await kullanımı dışında plan dahilinde olan pek çok yeni kabiliyet gelecek dönemlerde bizleri beklemekte. Örneğin IEnumerable tipinin params ile metodlarda kullanılabilmesi, yapıcı metod seviyesinde tahmin edilebilirlik (Constructore Inference), Event Initializers, private protected ve diğerleri. Plan dahilinde olan yetenekleri şu anki Preview sürümünde kullanamadım. Anlayacağınız takipte olacağız

![Winking smile](/assets/images/2014/wlEmoticon-winkingsmile_222.png)

Sonuç

Doğruyu söylemek gerekirse yeni gelen dil özelliklerinin daha yalın ve sade kod üretimi noktasında katkılar sağladığı ilk göze çarpan noktalar arasında. Bu, Clean Code olarak nitelendirdiğimiz okunabilir, yönetilebilir, bakımı daha kolay yapılabilir, hata yapma olasılığını azaltan kod parçalarının oluşmasında önemli bir dayanak noktası. Ancak söz konusu özelliklerin gerçekten hayat kurtardığı veya “farklı olarak şu işe” yaradığını söylemek için sanırım biraz erken. Ben de halen çoğunu özümsemeye ve farklı faydalarının neler olabileceğini kavramaya çalışmaktayım.

Önemli olan noktalardan bir diğeri aslında CLR’ ın ve IL tarafının oldukça iyi tasarlanmış olduğu gerçeği. (Sanki 80li yıllardaki Lego parçalarının üstüne günümüz modellerini sorunsuz ve kolayca entegre edebilmek gibi) Dikkat edileceği üzere incelediğimiz yeni yeteneklerin çoğu syntax açısından kolaylıklar gösterse de IL tarafında beklediğimiz şekillere dönüşmekte. Dolayısıyla syntax tarafında planlanan bir kabiliyetin aslında IL tarafında karşılığı olmasının, bu yeni yeteneğin ortaya konulmasının sırrı olduğunu ifade edebiliriz.

İlerleyen dönemlerde söz konusu özelliklerin endüstüriyel anlamda faydalarını göreceğimizi umuyorum. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.