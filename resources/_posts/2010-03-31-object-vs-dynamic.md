---
layout: post
title: "Object vs Dynamic"
date: 2010-03-31 15:10:00 +0300
categories:
  - csharp
  - csharp-3-0
  - csharp-4-0
tags:
  - dynamic-language-runtime
  - csharp
---
Ayrıntılar detaylarda saklıdır. Bu cümleyi çok severim. Sevdiğim özlü sözler arasındadır. Gerçel bir nesnenin ne kadar kaliteli olduğunu anlamak için detaylarına bakmak gerekir. İşçiliğine, kullanılan malzemeye, malzemelerin uyumuna vs...Hatta benzer diğerleri ile olan kalite farkını anlamak için bile. Çok doğal olarak yazılım dünyasında da bir takım konuların anlaşılabilmesi, kavranabilmesi, benzerleri ile olan farklarının irdelenebilmesi için mutlaka detaylara bakmak, ama sıkılmadan bakmak gerekir. Aynen bu günkü yazımızda yapacağımız gibi.

![blg139_Giris.jpg](/assets/images/2010/blg139_Giris.jpg)

Bu yazımızda Dynamic Language Runtime kullanımında büyük öneme sahip olan dynamic ile.Net Framework'ün ilk çıktığı zamandan beri var olan Object tipi arasındaki farklılıkları görmeye çalışacağız. Bunun için kod tarafında biraz daha detaya girmemiz gerekecek. Çok derin değil belki de ama aradaki farklılıkları çıkartabilmek adına önemli detaylar. Başlamadan önce örneklerimizi Visual Studio 2010 Ultimate RC sürümünde geliştirdiğimizi ve ilerleyen sürümlerde farklılıklar olabileceğini hatırlatmak isterim.

Öncelikli olarak işe aşağıdaki basit kod parçası ve Object tipini ele alarak başlayalım.

Case 1;

```csharp
object pi = Math.PI;
Console.WriteLine(pi.GetType().ToString());
```

İlk satırda Math.PI sabit değerinin object tipine atandığını görüyoruz. Aslında bilinçsiz olarak bir tür dönüşümü söz konusu (Implicitly Type Casting). Burada herhangibir sıkıntı yok. pi.GetType satırının çalışma zamanı çıktısı ise System.Double olmalıdır. Nitekim eşitliğin sağ tarafından gelen Math.PI, double tipinden olduğu için pi isimli object tipi üzerinden ele alınsa da kendi tipini taşımış olmaktadır.

![blg139_Case1.gif](/assets/images/2010/blg139_Case1.gif)

Şimdi kodumuzu aşağıdaki gibi değiştirdiğimizi düşünelim.

Case 2;

```csharp
object pi = Math.PI;
object square = pi * 10 * 10;
```

Bu kez object tipinden olan pi ile iki sayısal değeri (ki bunlarda int tipindendir) matematiksel bir işleme tabi tutup sonucu yine bir object tipine atamaya çalışıyoruz. Ancak kodu derlediğimizde Compiler'ın bir derleme zamanı hata mesajı ürettiğini görürüz.

![blg139_Case2.gif](/assets/images/2010/blg139_Case2.gif)

Hımmm...Aslında bu son derece doğal bir sonuç. Nitekim derleme zamanında pi değişkeninin tipi object ve biz object tipi ile int tiplerini işleme sokmaya çalışıyoruz. Burada çözüm, dönüştürme işlemini bilinçli olarak yapmaktan ibaret (Explicitly Type Casting). Yani kodu aşağıdaki gibi düzenlemekten;

Case 3;

```csharp
object pi = Math.PI;
object square = (double)pi * 10 * 10;
```

Dikkat edileceği üzere pi değişkeni bilinçli olarak double tipine dönüştürülmüş durumdadır. Aslında az önceki senaryomuzda pi'nin taşıdığı tipin double olduğunu görmüştük. Yine de başka tipler ile işleme tabi tuttuğumuzda derleme zamanı hatası ile karşılaşmaktan kurtulamadık. Çünkü pi değişkeni double tipten değer taşıyan bir object idi aslında.

Şimdi durumu biraz daha entersan bir hale getireceğiz. Aşağıdaki kod parçasını göz önüne alın ve kodu kafanızda derleyip hata olup olmadığını söyleyin. Sonrasında ise çalışma zamanında bir hata oluşup oluşmayacağını düşünün ve bir karar daha verin. En sonunda ise bunu, konu ile ilgili yakın arkadaşlarınızla tartışın

![Wink](/assets/images/2010/smiley-wink.gif)

```csharp
object pi = Math.PI;
object square = (int)pi * 10 * 10;
```

Derleme zamanında herhangibir hata mesajı alınmayacaktır. Ancak çalışma zamanına (Runtime) geçtiğimizde aşağıdaki ekran görüntüsünde yer alan istisna (Exception) ile karşılaşırız.

![blg139_Case3.gif](/assets/images/2010/blg139_Case3.gif)

Upsss!!!

Sorun şudur; pi değişkeni object tipinden tanımlanmıştır ve eşitliğin sağ tarafına göre double tipinden bir değer taşımaktadır. Ancak bu tip cast operatörüne göre int tipine dönüştürülmeye çalışılmaktadır. Oysaki çalışma zamanının burada beklediği tam olarak double tipine dönüştürme işlemidir.

Dolayısıyla object tipini kullandığımız bu senaryoda matematiksel işlemlerin yapılabilmesi için, mutlaka doğru tipe dönüşüm gerçekleştirilmelidir. Eğer bir dönüştürme işlemi yapmassak, derleme zamanında hata mesajı alırız. Ancak tip dönüşümü yaparkende koltuğumuzda rahat edemeyiz, çünkü yanlış tipe dönüştürme işlemleri çalışma zamanı istisnaları ile cezalandırılır. Buna göre gerçekten beklenen tipe dönüşüm işlemi sağlanmalıdır.

Gelelim yeni gözdemiz olan Dynamic tipe. Yukarıdaki senaryoları bire bir, ama bu kez dynamic tipini kullanarak değerlendireceğiz.

```csharp
dynamic pi = Math.PI;
Console.WriteLine(pi.GetType().ToString());
```

Bu kez eşitliğin sol tarafında dynamic tipi vardır. object tipinin kullanıldığı örnektekine (Case 1) benzer olaraktan pi değişkeni yine double tipinden bir değer taşımaktadır. Çalışma zamanının üreteceği sonuçta aynı olacaktır. Ancak aşağıdaki kod parçasını göz önüne aldığımızda,

```csharp
dynamic pi = Math.PI;
dynamic square = pi * 10 * 10;
```

object tipinin kullandığımız örneğe (Case 2) baktığımızda derleme zamanında bir hata aldığımızı hatırlıyoruzdur sanırım. Oysaki dynamic tipi derleme zamanı ile ilgilenmemektedir. Çalışma zamanında ise pi'yi gereken tipte ele almaktadır. Zaten object tipini kullandığımız üçüncü senaryomuzu ele almamıza da gerek kalmamıştır.

![Wink](/assets/images/2010/smiley-wink.gif)

Buna göre şöyle bir sonuca varabiliriz. Dynamic tipin kullanıldığı hallerde derleyicinin (Compiler) derleme zamanında tip tahmini yapmasına gerek yoktur. Bu çözümleme işi çalışma zamanında yapılmaktadır.

Tabi bu sonuçlara göre "her yerde dynamic tip kullanalım mı?" sorusu da gündeme gelir. Ancak "metodlara parametre aktarımlarında dynamic kullanımının kodun kırılmasına neden olması söz konusu olabilir mi?" sorusu daha da önemlidir. Şimdi bu durumu ele almaya çalışalım. İşte kodlarımız;

```csharp
dynamic R = "on iki";
	Calculate(R);
	#endregion
	#endregion
}
static double Calculate(double r)
{
	return Math.PI * r * r;
}
```

Bu kod parçasında kodu kırmaya yönelik olarak bir hamle yapıldığı düşünülebilir. Calculate metodu double tipinden bir değer beklemektedir. Biz ise dynamic olarak tanımladığımız R değişkenine string bir değer atayarak parametre gönderme işlemini gerçekleştirmekteyiz. Dynamic kullanımına göre derleme zamanında bir hata mesajı alınmaması normaldir. Benzer şekilde çalışma zamanında gelen string tipinin, double tipe otomatik olarak dönüştürüleceğini de düşünebiliriz. Eğer böyle olsaydı, kodun dynamic tipi yardımıyla kolayca kırılabileceği sonuçlarına varabilirdik. Şükür ki çalışma zamanında aşağıdaki hata mesajı ile cezalandırılırız.

![Laughing](/assets/images/2010/smiley-laughing.gif)

![blg139_Case6.gif](/assets/images/2010/blg139_Case6.gif)

Ancak,

```csharp
dynamic R=12;
```

atamasını yaparsak herhangibir sorun ile karşılaşmayız. Çünkü Calculate metodunun beklediği (veya taşıyabildiği) tipte bir değişkenin gönderilmesi sağlanmaktadır.

Sonuç olarak bir metoda dynamic tipte veri atayabiliyor olsakta bu, metoda her tipten değeri aktarabileceğimiz anlamına gelmemektedir. Gerçekten metodun beklediği veya kabul edebileceği türden bir değişkenin atanması şarttır.

Son senaryomuzu object tipi ile düşündüğümüzdeyse yine başta açıklanan 3 vakanın gerçekleşeceği görülecektir. Gelin bu durumları bir kere daha inceleyelim. Kodu ilk etapta aşağıdaki gibi düzenleyelim.

```csharp
object R = 12.1;
Calculate(R);
```

Bu durumda daha derleme zamanında hata mesajı alırız. Aşağıdaki şekilde olduğu gibi.

![blg139_Case7.gif](/assets/images/2010/blg139_Case7.gif)

Dolayısıyla tip dönüşümü yapmamız şarttır. Öyleyse yapalım.

![Laughing](/assets/images/2010/smiley-laughing.gif)

```csharp
object R = 12.1;
Calculate((int)R);
```

Derleme zamanında hata yok. Süper...Ama oda ne? Çalışma zamanında yine hata aldık.

![Undecided](/assets/images/2010/smiley-undecided.gif)

![blg139_Case8.gif](/assets/images/2010/blg139_Case8.gif)

Tahmin edileceği üzere object tipi kullanıldığından çalışma zamanında tam olarak double tipinden bir değer taşınması beklenmektedir. 12.1 double olarak ifade edilmesine rağmen int tipine yapılan dönüşüm geçersizdir (Dynamic tipin kullanımının tam aksine). Dolayısıyla kodun aşağıdaki gibi düzenlenmesi gerekir.

```csharp
object R = 12.1;
Calculate((double)R);
```

Bu durumda çalışma zamanında her hangibir hata mesajı alınmadan ilerlenebilecektir.

Görüldüğü üzere derinlerde, dynamic ve object kullanımları arasında belirgin farklılıklar bulunmaktadır. Bunların sebepleri aşikardır. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek, hepinize mutlu günler dilerim.

[DynamicVsObject_RC.rar (20,73 kb)](/assets/files/2010/DynamicVsObject_RC.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir]
