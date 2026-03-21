---
layout: post
title: "Checked, Unchecked Anahtar Kelimeleri ve OverFlow Hatası"
date: 2004-01-15 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - checked
  - unchecked
---
Bugünkü makalemizde, değişkenlerin içerdikleri verilerin birbirleri arasında atanması sırasında oluşabilecek durumları incelemeye çalışacağız. Bildiğiniz gibi, değişkenler bellekte tutulurken, tanımlandıkları veri tipine göre belirli bir bit boyutuna sahip olurlar. Ayrıca her değişkenimizin belli bir değer aralığı vardır. Programlarımızı yazarken, çoğu zaman değişkenleri birbirlerine atarız. Küçük boyutlu bir değişkeni, kendisinden daha büyük boyutlu bir değişkene atarken bir problem yoktur. Ancak, boyutu büyük olan bir değişkeni, daha küçük boyuta sahip bir değişkene atamak istediğimizde durum değişir. Elbette böyle bir durumda, derleyicimiz bizi uyaracaktır. Ancak bilinçli olarak yani tür dönüştürme anahtar kelimelerini kullandığımız durumlarda herhangibir derleyici hatasını almayız. Bu konuyu daha iyi anlayabilmek, değişkenleri tanımladığımız türlere ait boyut bilgilerinin iyi bilinmesini gerektirir. Bu amaçla aşağıdaki tabloda, C# programlama dilinde kullanılan değişken türlerini bulabilirsiniz.

Değişken Türü
Boyut (Bit)
Alt Aralık
Üst Aralık

Byte
8
0
255

SByte
8
-128
127

Short
16
-32768
32767

UShort
16
0
65535

Int
32
-2,147,483,648
2,147,483,647

UInt
32
0
4,294,967,295

Long
64
-9,223,372,036,854,775,808
9,223,372,036,854,775,807

ULong
64
0
18,446,744,073,709,551,615

Float
32
+/- 1.5 X 10^-45
+/- 3.4 X 10^38

Double
64
+/- 5 X 10^-324
+/- 1.7 X 10^308

Decimal
128
1 X 10^-28
7.9 X 10^1028

Char
16
-
-

Bool
-
-
-

Tablo 1. C# Değişken Türlerini Hatırlayalım.

Büyük alanlı değişkenlerin, küçük alanlı değişkenler içine alınması sırasında neler olabilieceğini gözlemlemek amacıyla aşağıdaki örneğimizi inceleyelim.

```csharp
using System;

namespace checkedUnchecked
{
     class Class1
     {
          static void Main(string[] args)
          {
               short degisken1=32760;
               byte degisken2;
               degisken2=(byte)degisken1;
               Console.WriteLine("Short tipinden değişkenimiz : {0}",degisken1);
               Console.WriteLine("Short değişkenimizi byte tipinden değişkene aldık : {0}",degisken2);
             }
     }
}
```

Uygulamamızda, Short tipinde degisken1 isminde bir değişkenimiz var. Değeri 32760. Short tipi değişken türleri -32768 ile 32767 arasındaki değerleri alabilen sayısal bir türdür. Degisken2 isimli, değişkenimiz ise Byte türünden olmakla birlikte değer aralığı 0 ile 255 arasındadır. Kodumuzda bilinçli bir şekilde, (byte) dönüştürücüsü yardımıyla, short türünden değişkenimizi, byte türünden değişkenimize atıyoruz. Bu kod hatasız olarak derlenecektir. Ancak, uygulamamızı çalıştırdığımızda karşımıza çıkacak olan sonuç beklemediğimiz bir sonuç olucaktır.

![mk42_1.gif](/assets/images/2004/mk42_1.gif)

Şekil 2. Sonuç şaşırtıcı mı?

Gördüğünüz gibi anlamsız bir sonuç elde ettik. Şimdi gelin bunun nedenini ele alalım. Öncelikle degisken1 isimli short türünden değişkenimizi ele alalım. Short tipi 16 bitlik bir veri alanını temsil eder. Degisken1 isimli veri tipimizin bellekte bitsel düzeyde tutuluş şekli aşağıdaki gibi olucaktır.

![mk42_2.gif](/assets/images/2004/mk42_2.gif)

Şekil 2. Short tipinden değişkenimizin bellekte tutuluşu.

Şimdi byte türünden tanımladığımız değişkenimizi ele alalım. Byte türü 8 bitlik bir veri türüdür ve 0 ile 256 arasında sayısal değerler alır. degisken1 isimli short türünden değişkenimizi, byte türünden değisken2 değişkenimiz içine almaya çalıştığımızda aşağıdaki sonuç ile karşılaşırız.

![mk42_3.gif](/assets/images/2004/mk42_3.gif)

Şekil 3. Atama sonrası.

Görüldüğü gibi 16 bitlik short tipi değişkenin ilk 8 biti havaya uçmuştur. Çünkü, byte veri tipi 8 bitlik bir veri tipidir. Dolayısıyla 16 bitlik bir alanı, 8 bitlik alana sığdırmaya çalıştığımızda veri kaybı meydana gelmiş ve istemediğimiz bir sonuç ortaya çıkmıştır. Elbette hiçbirimiz, yazdığımız programların çalışması sırasında böylesi mantık hatalarının olmasını istemeyiz. Bu durumun en güzel çözümlerinden birisi, checked anahtar kelimesini kullanmaktır. Checked anahtar kelimesi, uygulandığı bloktaki tüm tür dönüşümlerini kontrol eder ve yukarıdaki gibi bir durum oluştuğunda, OverFlow istisnasının fırlatılmasını sağlar. Yukarıdaki örneğimizi şimdide checked bloğu ile çalıştıralım.

```csharp
static void Main(string[] args)
{
     short degisken1=32760;
     byte degisken2;
     checked
     {
          degisken2=(byte)degisken1;
     }
     Console.WriteLine("Short tipinden değişkenimiz : {0}",degisken1);
     Console.WriteLine("Short değişkenimizi byte tipinden değişkene aldık : {0}",degisken2);
}
```

Bu durumda uygulamamızı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk42_4.gif](/assets/images/2004/mk42_4.gif)

Şekil 4. OverFlow istisnasının fırlatılması.

Derleyicimiz, checked anahtar kelimesinin kullanıldığı bloktaki tüm tür dönüşümlerini izlemeye alır. Eğer büyük alanlı bir değişken türü, kendisinden daha küçük alanlı bir değişken türüne atanmaya çalışırsa derleyici, OverFlow istisnasını fırlatır. Bu bize, checked bloklarının, try...catch...finally blokları ile kullanarak, kodumuzu dahada güvenli bir hale getirmemize imkan sağlar. Ne dediğimizi daha iyi anlayabilmek için, yukarıda yazdığımız kodu aşağıdaki gibi değiştirelim.

```csharp
static void Main(string[] args)
{
     short degisken1=32760;
     byte degisken2=0;
     try
     {
          checked
          {
               degisken2=(byte)degisken1;
               Console.WriteLine("Short tipinden değişkenimiz : {0}",degisken1);

               Console.WriteLine("Short değişkenimizi byte tipinden değişkene aldık : {0}",degisken2);
          }
     }
     catch(System.OverflowException hata)
     {
          Console.WriteLine(hata.Message.ToString());
          Console.WriteLine("Değişken 2 :{0}",degisken2.ToString());
     }
}
```

Burada, checked bloğunu, try...catch...finally bloğu içine alarak, programın kesilemesinin de önüne geçmiş olduk. Bununla birlikte, checked anahtar kelimesinin bir diğer özelliğide, kontrol altına aldığı blok içresinde oluşabilecek taşma hatalarının sonucunda, taşma hatasına maruz kalan değişkenlerin orjinal değerlerini korumasıdır. Örneğin yukarıdaki örnekte, byte türündeki değişkenimiz 248 değeri yerine ilk atama yaptığımız 0 değerini korumuştur.

Diğer yandan bazen, meydana gelebilecek bu tarz taşma hatalırını görmezden gelerek, bazı tür atamalarının mutlaka yapılmasını isteyebiliriz. Bu durumda unchecked bloklarını kullanırız. Bunu daha iyi anlayabilmek için, aşağıdaki örneğimize bir göz atalım.

```csharp
static void Main(string[] args)
{
     short degisken1=32760;
     byte degisken2=0;
     byte degisken3=0;
     try
     {
          checked
          {
               unchecked
               {
                    degisken3=(byte)degisken1;
                    Console.WriteLine("Kontrol edilmeyen degisken3 değeri: {0}",degisken3);
               }
                degisken2=(byte)degisken1;
               Console.WriteLine("Short tipinden değişkenimiz : {0}",degisken1);
               Console.WriteLine("Short değişkenimizi byte tipinden değişkene aldık : {0}",degisken2);
           }
     }
     catch(System.OverflowException hata)   
     {
          Console.WriteLine(hata.Message.ToString());
          Console.WriteLine("Değişken 2 :{0}",degisken2.ToString());
     }
}
```

Uygulamamızı çalıştırdığımızda, degisken3 isimli byte türünden değişkenimiz için, bilinçli olarak gerçekleştirdiğimiz dönüşümün gerçekleştiğini görebiliriz. Bunu sağlayan unchecked anahtar kelimesidir. Dolayısıyla oluşacak OverFlow hatasının görmezden gelindiğini görürüz.

![mk42_6.gif](/assets/images/2004/mk42_6.gif)

Şekil 5. Unchecked anahtar kelimesinin uygulanmasının sonucu.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.