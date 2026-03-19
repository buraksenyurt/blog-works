---
layout: post
title: "Arayüzler'de is ve as Anahtar Sözcüklerinin Kullanımı"
date: 2004-01-12 12:00:00 +0300
description: "Bugünkü makalemizde, arayüzlerde is ve as anahtar kelimelerinin kullanımını inceleyeceğiz. Bir sınıfa arayüz(ler) uyguladığımızda, bu arayüzlerde tanımlanmış metodları çağırmak için çoğunlukla tercih edilen bir teknik vardır. O da, bu sınıfa ait nesne örneğini, çalıştırılacak metodun tanımlandığı arayüz tipine dönüştürmek ve bu şekilde çağırmaktadır. Bu teknik, her şeyden önce, program kodlarının okunabilirliğini ve anlaşılabilirliğini arttırmaktadır. Öyleki, bir isim uzayında yer alan çok sayıda arayüzün ve sınıfın yer aldığı uygulamalarda be tekniği uygulayarak, hangi arayüze ait metodun çalıştırıldığı daha kolay bir şekilde gözlemlenebilmektedir. Diğer yandan bu teknik, aynı metod tanımlamalarına sahip arayüzler için de kullanılır ki bunu bir önceki makalemizde işlemiştik."
categories:
  - csharp
tags:
  - csharp
---
# Arayüzler'de is ve as Anahtar Sözcüklerinin Kullanımı
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, arayüzlerde is ve as anahtar kelimelerinin kullanımını inceleyeceğiz. Bir sınıfa arayüz (ler) uyguladığımızda, bu arayüzlerde tanımlanmış metodları çağırmak için çoğunlukla tercih edilen bir teknik vardır. O da, bu sınıfa ait nesne örneğini, çalıştırılacak metodun tanımlandığı arayüz tipine dönüştürmek ve bu şekilde çağırmaktadır. Bu teknik, her şeyden önce, program kodlarının okunabilirliğini ve anlaşılabilirliğini arttırmaktadır. Öyleki, bir isim uzayında yer alan çok sayıda arayüzün ve sınıfın yer aldığı uygulamalarda be tekniği uygulayarak, hangi arayüze ait metodun çalıştırıldığı daha kolay bir şekilde gözlemlenebilmektedir. Diğer yandan bu teknik, aynı metod tanımlamalarına sahip arayüzler için de kullanılır ki bunu bir önceki makalemizde işlemiştik.

Bu teknik ile ilgili olarak, dikkat edilmesi gereken bir noktada vardır. Bir sınıfa ait nesne örneğini, bu sınıfa uygulamadığımız bir arayüze ait herhangibir metodu çalıştırmak için, ilgili arayüz tipine dönüştürdüğümüzde InvalidCastException istisnasını alırız. Bu noktayı daha iyi vurgulayabilmek için aşağıdaki örneğimizi göz önüne alalım. Bu örnekte iki arayüz yer almakta olup, tanımladığımız sınıf, bu arayüzlerden sadece bir tanesini uygulamıştır. Ana sınıfımızda, bu sınıfa ait nesne örneği, uygulanmamış arayüz tipine dönüştürülmüş ve bu arayüzdeki bir metod çağırılmak istenmiştir.

```csharp
using System;

namespace Interface3
{
     public interface IKullanilmayan
     {
          void Yaz();
          void Bul();
     }
     public interface IKullanilan
     {
          void ArayuzAdi();

     }
  
     public class ASinifi:IKullanilan
     {
          public void ArayuzAdi()
          {
               Console.WriteLine("Arayüz adl:IKullanilan");
          }
     }
     class Class1
     {
          static void Main(string[] args)
          {
               ASinifi a=new ASinifi();
               IKullanilan Kul=(IKullanilan)a;
               Kul.ArayuzAdi();
               IKullanilmayan anKul=(IKullanilmayan)a;
               anKul.Yaz();
          }
     }
}
```

Bu örneği derlediğinizde herhangibi derleyici hatası ile karşılaşmassınız. Ancak çalışma zamanında "System.InvalidCastException: Specified Cast Is Invalid" çalışma zamanı hatasını alırız. İşte bu sorunu is veya as anahtar sözcüklerinin kullanıldığı iki farklı teknikten birisi ile çözebiliriz. Is ve as bu sorunun çözümünde aynı amaca hizmet etmekle beraber aralarında önemli iki fark vardır.

Is anahtar kelimesi aşağıdaki formasyonda kullanılır.

nesne is tip

Is anahtar kelimesi nesne ile tipi karşılaştırır. Yani belirtilen nesne ile, bir sınıfı veya arayüzü kıyaslarlar. Bu söz dizimi bir if karşılaştırmasında kullanılır ve eğer nesnenin üretildiği sınıf, belirtilen tip'teki arayüzden uygulanmışsa bu koşullu ifade true değerini döndürecektir. Aksi durumda false değerini döndürür. Şimdi bu tekniği yukarıdaki örneğimize uygulayalım. Yapmamız gereken değişiklik Main metodunda yer almaktadır.

```csharp
static void Main(string[] args)
{
     ASinifi a=new ASinifi();
     IKullanilan Kul=(IKullanilan)a;
     Kul.ArayuzAdi();
     if(a is IKullanilmayan){
          IKullanilmayan anKul=(IKullanilmayan)a;
          anKul.Yaz();
     }
     else
     {
          Console.WriteLine("ASinifi, IKullanilmayan arayüzünü uygulamamıştır.");
     }
}
```

![mk40_1.gif](/assets/images/2004/mk40_1.gif)

Şekil 1: is Anahtar Kelimesinin Kullanımı.

If koşullu ifadesinde, a isimli nesneyi oluşturduğumuz ASinifi sınıfına, IKullanilmayan arayüzünü uygulayıp uyguladığımızı kontrol etmekteyiz. Sonuç false değerini döndürecektir. Nitekim, ASinifi sınıfına, IKullanilmayan arayüzünü uygulamadık.

Is anahtar sözcüğü arayüzler dışında sınıflar içinde kullanabiliriz. Bununla birlikte is anahtar sözcüğünü kullanıldığında, program kodu Intermediate Language (IL) çevrildiğinde, yapılan denetlemenin iki kere tekrar edildiğini görürüz. Bu verimliliği düşürücü bir etkendir. İşte is yerine as anahtar sözcüğünü tercih etmemizin nedenlerinden biriside budur. Diğer taraftan is ve as teknikleri, döndürdükleri değerler bakımından da farklılık gösterir. Is anahtar kelimesi, bool tipinde ture veya false değerlerini döndürür. As anahtar kelimesi ise, bir nesneyi, bu nesne sınıfına uygulanmış bir arayüz tipine dönüştürür. Eğer nesne sınıfı, belirtilen arayüzü uygulamamışsa, dönüştürme işlemi yinede yapılır, fakat dönüştürülmenin aktarıldığı değişken null değerine sahip olur. As anahtar kelmesinin formu aşağıdaki gibidir.

sınıf nesne1=nesne2 as tip

Burada eğer nesneye belirtilen tipi temsil eden arayüz uygulanmamışsa, nesne null değerini alır. Aksi durumda nesne belirtilen tipe dönüştürülür. İşte is ile as arasındaki ikinci farkta budur. Konuyu daha iyi anlayabilmek için as anahtar kelimesini yukarıdaki örneğimize uygulayalım.

```csharp
static void Main(string[] args)
{
     ASinifi a=new ASinifi();
     IKullanilan Kul=(IKullanilan)a;
     Kul.ArayuzAdi();
     IKullanilmayan anKul=a as IKullanilmayan;
     if(anKul!=null)
     {
          anKul.Yaz();
     }
     else
     {
          Console.WriteLine("ASinifi IKullanilmayan tipine dönüştürülemedi");
     }
}
```

Burada a nesnemiz ASinifi sınıfının örneğidir. As ile bu örneği IKullanilmayan arayüzü tipinden anKul değişkenine aktarmaya çalışıyoruz. İşte bu noktada, ASinifi, IKullanilmayan arayüzünü uygulamadığı için, anKul değişkeni null değerini alıcaktır. Sonra if koşullu ifadesi ile, anKul 'un null olup olmadığı kontrol ediyoruz. Uygulamayı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk40_2.gif](/assets/images/2004/mk40_2.gif)

Şekil 2: as Anahtar Kelimesinin Kullanımı

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.