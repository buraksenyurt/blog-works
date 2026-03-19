---
layout: post
title: "C# 4.0 - Seçilebilen, İsimlendirilebilen Parametreler(Named and Optional Parameters), ref' i Görmezden Gelmek(Ommit Ref) ve PIA için Yenilikler"
date: 2009-05-04 13:46:00 +0300
description: "Bir önceki blog yazımızda C# 4.0 ile birlikte gelen önemli yeniliklerden birisi olan dynamic kavramına değinmeye çalışmıştık. Elbetteki C# 4.0 ile birlikte gelen başka yeniliklerde var. Bu yeniliklerde, diğerleri gibi belirli ihtiyaçlardan ortaya çıkmıştır. Öncelikli olarak bu ihtiyaçları ortaya koymaya çalışıyor olacağız. Bu nedenle PDC 2008'de dağıtılan Visual Studio 2010(PreBeta) sürümü ile yazdığım aşağıdaki kod parçasını bir süreliğine göz önüne alalım."
categories:
  - csharp-4-0
tags:
  - csharp-4-0
  - csharp
  - dotnet
  - http
  - reflection
  - visual-studio
---
# C# 4.0 - Seçilebilen, İsimlendirilebilen Parametreler(Named and Optional Parameters), ref' i Görmezden Gelmek(Ommit Ref) ve PIA için Yenilikler
Merhaba Arkadaşlar,

Bir önceki [blog](https://www.buraksenyurt.com/post/C-40-Dynamic-Olmak)yazımızda C# 4.0 ile birlikte gelen önemli yeniliklerden birisi olan dynamic kavramına değinmeye çalışmıştık. Elbetteki C# 4.0 ile birlikte gelen başka yeniliklerde var. Bu yeniliklerde, diğerleri gibi belirli ihtiyaçlardan ortaya çıkmıştır. Öncelikli olarak bu ihtiyaçları ortaya koymaya çalışıyor olacağız. Bu nedenle PDC 2008'de dağıtılan Visual Studio 2010 (PreBeta) sürümü ile yazdığım aşağıdaki kod parçasını bir süreliğine göz önüne alalım.

```csharp
using System;
using System.Reflection;
using Word=Microsoft.Office.Interop.Word;

namespace NewFeatures2
{
    class Program
    {
        static void Main(string[] args)
        {
            Word.Application wrdApp = new Microsoft.Office.Interop.Word.Application();            
            wrdApp.Visible = true;
            object fileNamePath = @"C:\Yeni Ozellikler.docx";
            object missingValue = Missing.Value;

            wrdApp
                .Documents
                .Open(ref fileNamePath, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue);
            Console.WriteLine("Kapatmak için bir tuşa basınız");
            Console.ReadLine();
        }
    }
}
```

İlk olarak şunu belirtmek isterim; bu basit Console uygulamasında Microsoft.Office.Interop.Word assembly'ına ait bir referans yer almaktadır.

![blg12_6.gif](/assets/images/2009/blg12_6.gif)

Program, sistemde yüklü olan Yeni Ozellikler.docx isimli bir Word dosyasını açmak için gerekli kodları içermektedir. Bu Word dosyasının açılması içinse, Microsoft.Office.Interop.Word assembly'ından yaralanılmaktadır. Bu assembly aslında Word ile konuşabilmemizi sağlayan COM API'sini sarmalayan (Wrap) bir yönetimli (Managed) kütüpanedir. Kod içerisinden, Word dökümanına erişebilmek için Application tipinden bir nesne oluşturulmaktadır. Uygulamanın görünürlüğü Visible özelliği ile set edildikten sonra ise, Open metodundan yararlanarak ilgili Word belgesinin açılması sağlanmaktadır. Ancak bu kod parçasında geliştirici açısından bazı zorluklar olduğu rahatlıkla gözlemlenebilir.

1- Open metoduna ait 16 adet parametrenin tamamının girilmesi zorunludur. Aşağıdaki şekilde durumun sıkıcılığı gözler önündedir.

[![blg12_1Mini.gif](/assets/images/2009/blg12_1Mini.gif)](/assets/images/2009/blg12_1Big.gif)

NOT: Burada Juval Lowy'nin [IDesign](http://www.idesign.net/idesign/DesktopDefault.aspx)şirketinde kullandığı ve pek çok şirket tarafından benimsenen [C# kodlama standartları](http://www.idesign.net/idesign/download/IDesign%20CSharp%20Coding%20Standard.zip)aklıma geliyor. Buradaki belirttiği bir maddeyi hatırlıyorum. "Metodların argüman sayılarının 5 i geçmesinden kaçının. Eğer öyleyse struct tipini kullanın". ![Wink](/assets/images/2009/smiley-wink.gif) Burada bir COM API'sinin Wrap edilmiş kütüphanesi içerisindeki bir fonksiyonun parametre yapısının değiştirilemeyeceği fikri belleğimizi tamamıyla kaplayabilir. Tabi C# 3.0 sonrasında bir fırsat olabileceği de akıllara gelebilir. "Bir Extension metod yardımıyla Open metodu yerine bir alternatif geliştirebilir miyim? En azından parametre sayısını düşürmemizi kolaylaştıracak..." Bunu denemenizi öneririm.

2- Parametreler COM nesnesine iletildiğinden, dışarıdan yapılacak olan atamalarda object tipinin kullanılması gerekmektedir.

3- ref anahtar kelimesinin kullanılması zorunludur.

Gerçekte, Open metodu içerisinde işimize yarayan ve bizim için anlam ifade eden tek bir parametre yer almaktadır. O da açılmak istenen dosyanın adıdır. Diğer parametrelerinin hiçbirini kullanmadığımız halde yazmak zorunda olduğumuzu görüyoruz. Keşke sadece gerekli olanları yazsabilseydik; o zaman bu iş daha kolay olmazmıydı?

![Frown](/assets/images/2009/smiley-frown.gif)

Nitekim buradaki Open metodu haricinde, çok daha fazla sayıda argüman ile çalışabilen COM fonksiyonellikleri söz konusu olabilir. Böyle bir durumda tam olarak tüm parametreleri yazma zorunluluğu bir kenara dursun, bunların bütünün ne işe yaradığınında bilinmesi gerekir.

Sanırım bu cümlelerden zaten nereye varmak istediğimi anlatabilmişimdir..Net in gelecek nesillerinin en büyük hedeflerinden birisi dinamik dillere ait nesneler ile konuşabilmek ve bunu mümkün olduğunca kolaylaştırmaktır. Bu noktada COM API'leri gibi nesnelerinde kullanımı söz konusudur. Aynen yukarıda geliştirdiğimiz örnekte olduğu gibi. Bu nedenle C# 4.0 içerisinde seçimlik parametre kullanımına izin veren geliştirmeler yapılmıştır (Optional Parameters) Buna göre yukarıdaki kod parçasını C# 4.0 stilinde aşağıdaki gibi geliştirebiliriz.

Optional Parameters ile

```csharp
using System;
using System.Reflection;
using Word=Microsoft.Office.Interop.Word;

namespace NewFeatures2
{
    class Program
    {
        static void Main(string[] args)
        {
            Word.Application wrdApp = new Microsoft.Office.Interop.Word.Application();            
            wrdApp.Visible = true;            
            wrdApp.Documents.Open(@"C:\Yeni Ozellikler.docx");
            Console.WriteLine("Kapatmak için bir tuşa basınız");
            Console.ReadLine();
        }
    }
}
```

Bu kod parçası çalıştığında da aynı sonucu alırız. Yine Word belgesi açılacak ve içeriği görüntülenecektir. Hem kodun okunurluğu kolaylaşmıştır, hem de kısalmıştır. Diğer taraftan parametre değerini aktarırken ref kullanılmadığına dikkat etmemiz gerekiyor.(Ommit ref özelliği) Üstelik object tipinden değişken ataması yerine doğrudan dosya adresininin içeriğini gönderebildiğimizede dikkat edelim.

Tabi ihtiyaçlar bitmek bilmiyor. Burada görüldüğü gibi gereksiz olan parametrelerin hiç biri bildirilmemiştir. Ayrıca ref anahtar kelimeside herhangibir şekilde kullanılmamıştır. Ancak arada başka bir parametrenin daha kullanılması gerekirse...

![Undecided](/assets/images/2009/smiley-undecided.gif)

Söz gelimi 3ncü parametre dosyanın yanlız okunabilir (ReadOnly) modda açılıp açılmayacağını belirtir. Optional Parameter tekniğini kullanırsak ikinci parametreyi atlamamız mümkün olmayacaktır. Acaba böyle bir vakada kodu yine istemediğimiz şekliyle aşağıdaki gibi geliştirmemiz mi gerekir?

```csharp
using System;
using System.Reflection;
using Word=Microsoft.Office.Interop.Word;

namespace NewFeatures2
{
    class Program
    {
        static void Main(string[] args)
        {
            Word.Application wrdApp = new Microsoft.Office.Interop.Word.Application();            
            wrdApp.Visible = true;            
            object fileNamePath = @"C:\Yeni Ozellikler.docx";
            object missingValue = Missing.Value;
            object onlyRead = true;

            wrdApp
                .Documents
                .Open(ref fileNamePath, ref missingValue, ref onlyRead, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue);           

            Console.WriteLine("Kapatmak için bir tuşa basınız");
            Console.ReadLine();
        }
    }
}
```

Oysaki C# 4.0 bu gibi durumlar için isimlendirilmiş parametre (Named Parameters) kullanımını olanaklı kılmaktadır. Aşağıdaki şekilde görüldüğü gibi, intellisense'de bize yardımcı olmaktadır.

![blg12_2.gif](/assets/images/2009/blg12_2.gif)

Dolayısıyla yukarıdaki kod parçasını aşağıdaki gibi geliştirebiliriz.

Named Parameters kullanımı ile

```csharp
using System;
using System.Reflection;
using Word=Microsoft.Office.Interop.Word;

namespace NewFeatures2
{
    class Program
    {
        static void Main(string[] args)
        {
            Word.Application wrdApp = new Microsoft.Office.Interop.Word.Application();            
            wrdApp.Visible = true;
            
            wrdApp.Documents.Open(@"c:\Yeni Ozellikler.docx", ReadOnly: true);

            Console.WriteLine("Kapatmak için bir tuşa basınız");
            Console.ReadLine();
        }
    }
}
```

Son olarak Platform Interop Assembly (PIA) ile ilgili gelen yeniliklerden birisine değinmek istiyorum. Normal şartlarda Visual Studio 2010 öncesinde bir COM API'sini uygulamaya referans ettiğimizde, sarmalanan kütüphanenin özelliklerinde aşağıdaki şekilde görülen Embed Interop Types isimli bir kriter olmadığı bilinmektedir.

![blg12_3.gif](/assets/images/2009/blg12_3.gif)

Oysaki Visual Studio 2010 ile birlikte bu özellikte gelmektedir. Bu tabiki sadece C# 4.0 diline bağlanacak bir yetenek olarak düşünülmemelidir.

Peki ne işe yarar? Eğer yukarıda geliştirdiğimiz C# 4.0 örneğinin ildasm (Intermediate Language DisAsseMbler) çıktısına bakacak olursak aşağıdaki durum ile karşılaşırız.

![blg12_4.gif](/assets/images/2009/blg12_4.gif)

Göze çarpan özel bir nokta yer almamaktadır. Ancak Microsoft.Office.Interop.Word assembly'ının özelliklerinde yer alan Embed Interop Types seçeneğini true olarak değiştirir ve söz konusu uygulamanın IL çıktısına tekrardan bakarsak aşağıdak sonuçlarla karşılaşırız.

![blg12_5.gif](/assets/images/2009/blg12_5.gif)

Görüldüğü gibi API içerisinde yer alan tipler,.Net programı içerisine birer tip olarak gömülmüştür. Aslında bu yenilik, PIA'ların, geliştirilen asıl uygulama içerisine tip bazında gömülerekten taşınabilmelerini kolaylaştırıcı bir özellik olarak görülebilir. Bu konudaki araştırmalarıma devam ediyorum. Yeni bilgiler kazandıkça sizlerle paylaşmaya devam ediyor olacağım.

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda sizlere C# 4.0 ile birlikte gelen bir kaç yeniliği aktarmaya çalıştım. Sonuç olarak bu yeniliklerin özellikle dynamic tiplerin kullanımı kolaylaştırmak üzere getirildiğini söyleyebiliriz.

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.