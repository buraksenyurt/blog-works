---
layout: post
title: "C# 4.0 - Metod Overloading ve Dynamic Tipler"
date: 2010-04-13 04:40:00 +0300
categories:
  - csharp-4-0
tags:
  - csharp
  - dynamic
  - object
  - method-overloading
---
Eminim hepimiz çocukluğumuzda en az bir kere olmak üzere yediklerimizi, elimize yüzümüze bulaştırmış ve kirlenmişizdir. Her ne kadar bazı şirketler reklam kapmanyalarında kirlenmek güzeldir diyerek annelerin yüreğine su serpseler de, bu aslında pek gerçeği yansıtmamaktadır. Nitekim anneler, çocuklarının ellerini yüzlerini yediklerine bulayıp etraftaki eşyalara dokunmalarını pek hoş karşılamayabilirler.

![blg150_Giris.jpg](/assets/images/2010/blg150_Giris.jpg)

Ben şahsen bunu çocukken bir kaç kez tecrübe etmiş bir insanımdır. Yine de, yaşım hayatın yarısına merdiven dayamış olsa da, bazı zamanlarda o çok sevdiğim kayısı marmelatlı ve üstü pudralı olan Berliner tatlısını elime yüzüme (hatta burnuma) bulaştırarak yemeyi severim (Tabi evde ve en fazla eşimin yanında) Şimdi diyeceksiniz ki Burak Hoca gene başladı bir yiyecek ile...

![Embarassed](/assets/images/2010/smiley-embarassed.gif)

Yok. Aslında odaklanacağımız nokta herşeyi ele yüze bulaştırmak. Şimdi öyle bir konuya dalacağız ki herşeyi karıştırıp allak bullak edip yüzümüze gözümüze bulaştıracağız. Öyleyse gelin hiç vakit kaybetmeden üstümüzü biraz kirletelim

![Wink](/assets/images/2010/smiley-wink.gif)

Bu günkü yazımızda Dynamic tiplerin, metodların aşırı yüklenmesi (overload) durumunda nasıl bir duruma neden olduklarını incelemeye çalışıyor olacağız. Bildiğiniz üzere bir metodun aynı isme ait birden fazla versiyonu yazılabilmektedir. Bu durum kısaca metodların aşırı yüklenmesi (Overloading) olarak adlandırılmaktadır. Metodların aşırı yüklenmesindeki en büyük gayelerden birisi de, aynı amaca hizmet eden ama bunu farklı sayıda veya tipte parametre ile yerine getirebilen fonksiyonların farklı isimler ile yazılmasını engellemek ve böylece anlamsal bütünlüğü korumaktır..Net Framework, ilk versiyonundan itibaren bu özelliği içermektedir. Çok eskiden eğitmenlik yaptığım dönemlerde, metodların aşırı yüklenmesi konusu ile ilişkili olarak verdiğim ilk örnek her zaman için Console sınıfının static WriteLine metodu olmuştur.

![blg150_Cw.gif](/assets/images/2010/blg150_Cw.gif)

Şekilden de görüleceği üzere, WriteLine metodunun farklı tipte veya sayıda parametre ile çalışabilen 19 farklı versiyonu bulunmaktadır. Burada derleme zamanı açısından önem arz eden konulardan birisi de, metodların hangisinin çağırıldığının ayırt edilmesidir. İşte bu noktada metodun imzası (Signature) adı verilen kavram devreye girmektedir. Metod imzası, parametre sayısı veya tiplerini kapsamaktadır. Buna göre aynı tipten ama farklı sayıda parametre veya farklı tipten ama aynı sayıda parametre, çoğunlukla geçerlidir. Upsss...Çoğunlukla mı?

![Surprised](/assets/images/2010/smiley-surprised.gif)

Neden böyle söylediğimi ispat etmek için C# 4 ile birlikte gelen dynamic tiplerin, metodların aşırı yüklenmesi sırasındaki kullanımlarına göz atmamız yeterlidir. Öncelikli olarak aşağıdaki kod parçasını göz önüne alalım.

```csharp
namespace DynamicAndOverloading
{
    class Program
    {
        static void Topla(int x) { }
        static void Topla(int x, dynamic y) { }
        static void Topla(dynamic x, int y) { }

        static void Main(string[] args)
        {
            Topla(4);
            Topla(2,3);
        }
    }
}
```

Topla metodunun 3 farklı versiyonunun yazıldığı görülmektedir. Teorimize göre, tüm metodlar birbirlerinde farklıdır. Nitekim metod imzası kriterleri sağlanmaktadır. İlk Topla metodu tek parametre aldığı için iki parametre alan diğer versiyonları ile otomatik olarak ayrışmaktadır. Diğer yandan iki parametre alan versiyonlarda da, parametrelerin tipleri farklıdır. Farklıdır, nitekim sıraları aynı değildir. Dolayısıyla herhangibir sorun görünmemektedir. Oysaki daha Toplam metodunun iki parametreli versiyonunu yazarken, aşağıdaki ekran görüntüsünde yer alan hata mesajı ile karşılaşılır.

![blg150_CompileTimeError.gif](/assets/images/2010/blg150_CompileTimeError.gif)

Mesaja göre derleyici, Topla (int,dynamic) ile Topla (dynamic,int) çağrıları arasında kararsız kalmıştır. Açıkçası ortada tam bir belirsizlik söz konusudur. Peki bu durum size tanıdık geldi mi? Aslında dynamic tip yerine object tipini kullandığımızda da benzer bir sorunla karşılaşırız. Aynı örnek kod parçasında bu sefer dynamic yerine object tipini kullandığımızı düşünelim.

![blg150_ObjectError.gif](/assets/images/2010/blg150_ObjectError.gif)

Durum değişmemiştir. Derleyici yine hangi metodu çağıracağı konusunda kararsız kalmış ve hata mesajı üretilmesine sebebiyet vermiştir. Dolayısıyla program çalışmamaktadır.

Bu noktada metodların aşırı yüklenmesinde object tipi ile dynamic tiplerin benzer bir davranışa neden olduklarını düşünebiliriz. Ancak bunun ispatını da yaparsak ballı kaymaklı tap taze beyaz ekmek yemiş kadar oluruz.

![Laughing](/assets/images/2010/smiley-laughing.gif)

Şimdi durumu net bir şekilde ispatlamak adına kodu aşağıdaki gibi değiştirelim.

```csharp
namespace DynamicAndOverloading
{
    class Program
    {
        static void Topla(int x) { }
        static void Topla(int x, dynamic y) { }
        //static void Topla(dynamic x, int y) { }  // Yorum satırı yaptık

        static void Main(string[] args)
        {
            Topla(4);
            Topla(2,3);
        }
    }
}
```

Bu durumda program kodu başarılı bir şekilde derlenecektir. Şimdi belki de uzun zamandır yanına uğramadığımız [Red Gates.Net Reflector](http://www.red-gate.com/products/reflector/) aracını açalım ve program kodumuzun içeriğine bir bakalım. Aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaşırız.

![blg150_Reflector1.gif](/assets/images/2010/blg150_Reflector1.gif)

Dikkatinizi çeken bir şey oldu mu?

![Wink](/assets/images/2010/smiley-wink.gif)

Topla metodunun iki parametre alan versiyonunun IL (Intermediate Language) tarafına olan aktarımına göre dynamic tip olarak tanımladığımız y değişkeni, aslında Object tipi olarak değerlendirilmektedir. Bir dakika... Peki çalışma zamanı bunun aslında dynamic bir tip olduğunu nasıl anlayacaktır? Bunu Topla (Int32,Object): Void metodunun C# kodu çıktısına bakarak görebiliriz. İşte.Net Reflector çıktısı.

![blg150_Reflector2.gif](/assets/images/2010/blg150_Reflector2.gif)

Görüldüğü üzere object y değişkeni [Dynamic] niteliği ile imzalanmıştır. Dolayısıyla çalışma zamanı tarafından aslında dynamic tip olarak yorumlanacaktır.

Buraya kadar her şey netleşmiş gibi düşünülebilir ve hatta yazımızın artık bitmemesi için bir neden olmadığı da düşünülebilir. Ancak yazımızı sonlandırmadan önce, object tipinin metodların aşırı yüklenmesinde yol açtığı sorunun, diğer tipler içinde geçerli olabileceğini gösterek ilerlememizde yarar vardır. Söz gelimi aşağıdaki şekilde yer alan kod parçasında da benzer durumun oluştuğu görülebilir.

![blg150_DoubleError.gif](/assets/images/2010/blg150_DoubleError.gif)

Dikkat edilecek olursa derleyici, double tipinin kullanıldığı Topla metodlarından hangisinin kullanılacağı konusunda kararsız kalmaktadır. Yoksa metod imzası kavramı çatlamakta mı? Aslında bir çözüm söz konusudur. O da, doğru değerler ile ilgili metodların çağırılmasıdır. Yani gerçekten double tip ile çağrı yapılmasıdır. Bu durumda aşağıdaki kod parçası derleme zamanı hatasına yol açmayacaktır.

```csharp
namespace DynamicAndOverloading
{
    class Program
    {
        static void Topla(int x) { }
        static void Topla(int x, double y) { }
        static void Topla(double x, int y) { }

        static void Main(string[] args)
        {
            Topla(4);
            Topla(Math.PI,3);
        }
    }
}
```

Derleme hatası olmaması son derece doğaldır. Nitekim Math.PI değişkeninin kullanılması, Topla (double x,int y) metodunun tespit edilmesini sağlamaktadır. Şimdi olay biraz daha ilginç bir hal almaya başlayacaktır. Bunun için program kodunu aşağıdaki şekilde güncelleyip devam ettiğimizi düşünelim.

```csharp
namespace DynamicAndOverloading
{
    class Program
    {
        static void Topla(int x) { }
        static void Topla(int x, dynamic y) { }
        static void Topla(dynamic x, int y) { }

        static void Main(string[] args)
        {
            Topla(4);
            Topla(Math.PI,3);
        }
    }
}
```

Uygulamayı derlediğimizde her hangibir hata mesajı ile karşılaşmadığımızı görürüz.

![blg150_Succeeded.gif](/assets/images/2010/blg150_Succeeded.gif)

Oysa ki az önce dynamic tipin kullanıldığı senaryoda kararsızlık yaşandığına gözümüzle şahit olmuştuk. Peki ya şimdi ne oldu da sorun çözüldü? Aslında sorun değerlerin farklılaştırılması ile çözüm bulmuştur. Dikkat edilecek olursa Topla (2,3) çağrısı derleyicinin kafasının karışmasına neden olurken, Topla (Math.PI,3) çağrısında bu sorun oluşmamıştır. Tahmin edeceğiniz üzere object tipi için yaşanan sorunda farklı tipteki değerlerin kullanılması halinde çözülecektir. Ve çok doğal double tipini kullandığımız vaka için de çözüm olacaktır.

![Wink](/assets/images/2010/smiley-wink.gif)

Tabi bu noktada şunu da belirtmekte yarar vardır. Söz konusu metodların ayrı bir kütüphane içerisinde tanımlanması halinde, bu kütüphaneyi kullanan tiplerin yazıda ele aldığımız hatalara düşme olasılıkları bulunabilir ki bu da istenen bir durum değildir. Dolayısıyla C#' ın temel kavramlarından birisi olarak ele alınana metodların aşırı yüklenmesi (Overload) aslında derinlerine inildiğinde dikkatli olunmayı gerektirecek vakaları içermektedir. Aynen yazımızda ele aldığımız üzere. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kodlar Visual Studio 2010 Ultimate RTM sürümü üzerinde geliştirilmiş ve test edilmiştir]
