---
layout: post
title: "Sıfır Sabit Değeri ve Enum Sorunu"
date: 2013-09-19 10:48:00 +0300
description: "C# konulu yeni bir bilmece ile karşı karşıyayız. Bu sefer kolay kolay fark edemeyebileceğimiz, basit ama irdelenmesi gereken bir vakayı göz önüne alacağız."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - generics
---
# Sıfır Sabit Değeri ve Enum Sorunu
[![1186819_puzzle_time_1](/assets/images/2013/1186819_puzzle_time_1_thumb.jpg)](/assets/images/2013/1186819_puzzle_time_1.jpg) Merhaba Arkadaşlar,

C# konulu yeni bir bilmece ile karşı karşıyayız. Bu sefer kolay kolay fark edemeyebileceğimiz, basit ama irdelenmesi gereken bir vakayı göz önüne alacağız.

Bir uygulamayı geliştirirken, Developer olarak son derece dikkatli olmalı ve testlerimizi gerçekleştirirken de tüm senaryoları göz önüne almalıyız. Bu anlamda gerek dilin gerek.Net Framework alt yapısının tüm unsurlarına hakim olmak da son derece önemli.

Nitekim bazı noktalarda profesyonel bir geliştiricinin dahi kestiremeyeceği sorunsallar yaşanabiliyor. Yazımızın ilerleyen kısımlarında bu perspektiften bakarak sorun olarak da görülebilecek bir konuyu masaya yatıracağız.

## Senaryo

İlk olarak C# dili ile geliştireceğimiz aşağıdaki basit kod parçasını göz önüne alalım.

[![ze_0](/assets/images/2013/ze_0_thumb.png)](/assets/images/2013/ze_0.png)

```csharp
using System; 
using System.Collections.Generic;

namespace EnumAndZeroConstant 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            List<Information> infos = new List<Information>() 
                                          { 
                                              new Information("Hata mesajı"), 
                                              new Information(InformationType.Application), 
                                                              new Information(InformationType.Member), 
                                                              new Information(InformationType.System), 
                                                              new Information(0), 
                                                              new Information(1), 
                                                              new Information(2), 
                                                              new Information(3) 
                                          };

            foreach (Information info in infos) 
            { 
                Console.WriteLine(info.SummaryMessage); 
            }

        } 
    }

    enum InformationType 
    { 
        System, 
        Application, 
        Member 
    }

    class Information 
    { 
        public string SummaryMessage { get; private set; }

        public Information(object commonInformation) 
        { 
            SummaryMessage = commonInformation.ToString(); 
        }

        public Information(InformationType informationType) 
        { 
            switch (informationType) 
            { 
                case InformationType.Application: 
                    SummaryMessage = "Uygulama bilgisi"; 
                    break; 
                case InformationType.System: 
                    SummaryMessage = "Sistem bilgisi"; 
                    break; 
                case InformationType.Member: 
                    SummaryMessage = "Üyeden"; 
                    break; 
                default: 
                    SummaryMessage = "Bilinmeyen kaynak"; 
                    break; 
            } 
        } 
    } 
}
```

Uygulamamızda InformationType isimli bir Enum sabiti ve bu tipi kullanan Information isimli bir sınıf (Class) tanımlanmıştır. Information sınıfı içerisinde yer alan SummaryMessage isimli özelliğimiz (Property) dikkat edileceği üzere Read Only belirtilmiştir (set keyword’ ü başındaki private kullanımına dikkat)

Information sınıfı içerisinde kullanılan SummaryMessage özelliğinin değerini set etmek için, yapıcı metod (Constructor) parametrelerinden yararlanılmaktadır. İki adet aşırı yüklenmiş yapıcı metod (Overload Contstructor) mevcuttur. Bunlardan birisi InfortmationType enum sabiti tipinden bir değişken almakta ve bunun değerine göre SummaryMessage özelliğine string bir değer atamaktadır. Bu anlamda diğer aşırı yüklenmiş versiyon ise, object tipinden gelen referans değişkenin, ToString metodunun sonucunu kullanmaktadır.

Main metodu içerisinde test amaçlı olaraktan, Information tipinden generic bir List koleksiyonu değerlendirilmektedir. Dikkat edileceği üzere, koleksiyonun Information tipinden olan her bir nesne örneği üretilirken, parametre olarak farklı referanslar gönderilmektedir. Test amacıyla String, int, InfortmationType türünden değişken değerleri ele alınmaktadır.

“Peki bu kod parçasında nasıl bir tuzak olabilir? Ya da başka bir deyişle bir tuzak var mıdır? Gözümüzden kaçırdığımız bir nokta söz konusu ise nedir? “

## Düşünsel ve Gerçek Çalışma Sonuçları

Bu sorulara cevap bulabilmek için öncelikli olarak kodun nasıl bir sonuç üreteceğini düşünmeye çalışalım. Aslında beklentimiz, uygulamanın çıktısının aşağıdaki temsilde olduğu gibi üretilmesidir.

Hata mesajı
Uygulama bilgisi
Üyeden
Sistem bilgisi
0
1
2
3

Ancak, yapıcı metoda gönderilen 0 değerinden pis kokular gelmektedir. Ne demek istediğimi daha net bir şekilde ifade edebilmek için uygulamanın çalışma zamanı (Runtime) ekran çıktısına bir bakalım dilerseniz.

[![ze_1](/assets/images/2013/ze_1_thumb.png)](/assets/images/2013/ze_1.png)

Uppsss! Bir fark görebildiniz mi? Biz yapıcı metodumuza 0 değişkenini göndermemize rağmen, ekrana InformationSystem enum sabitindeki ilk değere karşılık gelen string mesajı döndürülmüştür. Gerçekten de uygulamayı debug ettiğimizde, 0 değerini yapıcı metoda gönderdikten sonra Enum sabitini parametre olarak kullanan versiyonuna gittiğimizi görürüz. Kodu biraz değiştirip Debug işlemimizi icra edelim ve bu durumu çalışma zamanında görelim.

[![ze_2](/assets/images/2013/ze_2_thumb.png)](/assets/images/2013/ze_2.png)

infoZero isimli Information tipinden olan değişken örneklenirken, yapıcı metoda 0 integer değeri gönderilmiştir. Ancak kod bu satırdan sonra object tipinden parametre alan versiyon yerine InformationType tipinden referans kullanan yapıcı metoda sıçramıştır. Üstelik 0 integer değer, bilinçsiz bir şekilde (Implicitly) InfortmationType.System sabit değerine dönüştürülmüştür.

Tabi bizim buradaki asıl beklentimiz 0 gibi bir sayısalın object gibi algılanması ve uygun olan yapıcı metoda gitmesi yönündedir. Ne varki bu gerçekleşmemiştir. Bu aslında 0 sayısal değerinin bilinçsiz olarak bir Enum sabiti formunda algılanıyor olmasıdır. Yani sıfır tabanlı sabit olarak algınanan yapıcı metod parametresi, bilinçsiz olarak Enum karşılığına dönüştürülmüştür.

## IL Tarafındaki Görüntü

Zaten uygulamamızın IL (Intermediate Language) tarafındaki çıktısına baktığımızda, 0 atamasının yapıldığı yerde InformationType enum sabitinin kullanıldığı versiyonun çağırılacağı da önceden belirtilmiştir.

```text
.method private hidebysig static void  Main(string[] args) cil managed 
{ 
  .entrypoint 
  // Code size       191 (0xbf) 
  .maxstack  3 
  .locals init ([0] class EnumAndZeroConstant.Information infoZero, 
           [1] class [mscorlib]System.Collections.Generic.List`1<class EnumAndZeroConstant.Information> infos, 
           [2] class EnumAndZeroConstant.Information info, 
           [3] class [mscorlib]System.Collections.Generic.List`1<class EnumAndZeroConstant.Information> '<>g__initLocal0', 
           [4] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class EnumAndZeroConstant.Information> CS$5$0000, 
           [5] bool CS$4$0001) 
  IL_0000:  nop 
  IL_0001:  ldc.i4.0 
  IL_0002:  newobj     instance void EnumAndZeroConstant.Information::.ctor(valuetype EnumAndZeroConstant.InformationType)
```

## Sorunun Çözümü

Bu gözden kolayca kaçabilecek bir noktadır. Hani test süreçlerinde Zero-Data diye bir senaryo vardır gerçi ama bu vaka zaten onunla bir değildir. Sadece bir isim benzerliği olduğunu ifade edebiliriz. Bizim burada sorunu nasıl çözebileceğimize bir bakmamız gerekiyor. Mademki ortada bilinçsiz bir tür dönüşümü var (Implicitly Type Conversion) bu durumda dönüşümü bilinçli hale getirmeyi deneyebiliriz (Explicitly Type Conversion). Aşağıdaki kod parçasındaki denemeleri yaptığımızı düşünelim.

```csharp
Information infoZero1 = new Information((int)0); 
var zeroVar = 0; 
Information infoZero2=new Information(zeroVar); 
object zeroObj = 0; 
Information infoZero3 = new Information(zeroObj); 
int zeroInt = Convert.ToInt32("0"); 
Information infoZero4 = new Information(zeroInt); 
double zeroDbl = 0.0; 
Information infoZero5 = new Information(zeroDbl); 
Information infoZero6 = new Information(0.0); 
var infoZero7 = new Information(0);

List<Information> infos = new List<Information>() 
                              { 
                                                  infoZero1, 
                                                  infoZero2, 
                                                  infoZero3, 
                                                  infoZero4, 
                                                  infoZero5, 
                                                  infoZero6, 
                                                  infoZero7 
                              };
```

Burada farklı şekillerde 0 değerinin gönderilmesi söz konusu olmuştur. İlk olarak cast operatörü göz önüne alınarak 0 değerinin int tipinden ele alınması denenmiştir. İkinci olarak 0 değişkenini taşıyan bir var tanımlaması yapılmaktadır. Devam eden kısımda sırasıyla Object tipine atanarak yapıcı metoda gönderilme, Convert tipinin static Int32 metodundan yararlanma, double bir değişken atama, yapıcı metoda parametre olarak 0 gönderirken var kullanarak ilerleme seçenekleri de denenmiştir. Bu durumda uygulamanın ekran çıktısı aşağıdaki gibi olacaktır.

[![ze_3](/assets/images/2013/ze_3_thumb.png)](/assets/images/2013/ze_3.png)

Dikkat edileceği üzere yapıcı metoda 0 değeri atama işlemini, dışarıda tanımlanan bir değişken üzerinden gerçekleştirdiğimizde object tipini kullanan yapıcı metoda gidilmiştir. Tabi bu durumda Information tipini kullanan geliştiricinin bu durumu göz önüne alması ve öncesinde gerekli dönüşüm işlemlerini yapması gibi bir sorumluluk ortaya çıkmaktadır. Oysaki geliştirici veya Object User çok kolayca durumu gözden kaçırabilir. Daha kalıcı bir çözüm ile ilerlemek gerekmektedir. Bunun için Information tipinin yapıcı metodlarının sayısını arttırıp tipe göre bir aksiyon alınması sağlanabilir. Aşağıdaki kod parçasın görüldüğü gibi.

```csharp
class Information 
{ 
    public string SummaryMessage { get; private set; }

    public Information(int info) 
    { 
        SummaryMessage = info.ToString(); 
    } 
    public Information(double info) 
    { 
        SummaryMessage = info.ToString(); 
    } 
    public Information(object commonInformation) 
    { 
        SummaryMessage = commonInformation.ToString(); 
    }

    public Information(InformationType informationType) 
    { 
        switch (informationType) 
        { 
            case InformationType.Application: 
                SummaryMessage = "Uygulama bilgisi"; 
                break; 
            case InformationType.System: 
                SummaryMessage = "Sistem bilgisi"; 
                break; 
            case InformationType.Member: 
                SummaryMessage = "Üyeden"; 
                break; 
            default: 
                SummaryMessage = "Bilinmeyen kaynak"; 
                break; 
        } 
    } 
}
```

Dikkat edileceği üzere int ve double tiplerini parametre olarak alan iki ek yapıcı metod daha ilave edilmiştir. Bu durumda 0 ve 0.0 değerleri için uygun olan yapıcı metodlar devreye girecek, bir başka deyişle InformationType enum sabitin kullanan yapıcı metod göz ardı edilecektir. Tabi yine de çok şık bir çözüm olmadığını ifade etmemiz gerekiyor. Nitekim 0 değerini taşıyabilecek pek çok sayısal tip mevcuttur. Byte, short, float vb…Dolayısıyla bunların her biri için bir aşırı yüklenmiş yapıcı metod yazmak çok da yerinde olmayabilir. Yine de Information tipini kullanacak olan geliştirici için hata riski bu şekilde azaltılabilir. Uygulamayı bu haliyle çalıştırdığımızda aşağıdaki ekran çıktısını elde ederiz.

[![ze_4](/assets/images/2013/ze_4_thumb.png)](/assets/images/2013/ze_4.png)

İstediğimiz sonuca ulaştık. Görüldüğü üzere Enum sabitlerini yapıcı metodlara parametre olarak geçirdiğimizde, göndereceğimiz 0 sabit değeri otomatik olarak bir enum sabiti tipine dönüştürülmektedir. Dolayısıyla söz konusu sınıf içerisinde object tipini kullanan başka bir yapıcı metod varsa, tedbir almadığımız durumda fark etmeyeceğimiz bir çalışma zamanı sonucu üretilebilir. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[EnumAndZeroConstant.zip (1,97 mb)](/assets/files/2013/EnumAndZeroConstant.zip)