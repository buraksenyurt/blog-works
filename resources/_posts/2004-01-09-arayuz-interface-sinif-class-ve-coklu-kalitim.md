---
layout: post
title: "Arayüz(Interface), Sınıf(Class) ve Çoklu Kalıtım"
date: 2004-01-09 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - oop
  - interface
---
Bugünkü makalemizde, arayüzleri incelemeye devam ediceğiz. Bir önceki makalemizde, arayüzleri kullanmanın en büyük nedenlerinden birisinin sınıflara çoklu kalıtım desteği vermesi olduğunu söylemiştik. Önce basit bir uygulama ile bunu gösterelim.

```csharp
using System;

namespace Interfaces2
{
     public interface IMusteri
     {
          void MusteriDetay();
          int ID{get;}
          string Isim{get;set;}
          string Soyisim{get;set;}
          string Meslek{get;set;}
     }

     public interface ISiparis
     {
         int SiparisID{get;}
          string Urun{get;set;}
          double BirimFiyat{get;set;}
          int Miktar{get;set;}
          void SiparisDetay();
     }
     public class Sepet:IMusteri,ISiparis /* Sepet isimli sınıfımız hem IMusteri arayüzünü hemde ISiparis arayüzünü uygulayacaktır. */
     {
          private int id,sipId,mkt;
          private string ad,soy,mes,ur;
          private double bf;
          public int ID
          {
               get{return id;}
          }
          public string Isim
          {
               get{return ad;}
               set{ad=value;}
          }
          public string Soyisim
          {
               get{return soy;}
               set{soy=value;}
          }
          public string Meslek
          {
               get{return mes;}
               set{mes=value;}
          }
          public void MusteriDetay()
          {
               Console.WriteLine(ad+" "+soy+" "+mes);
          }
          public int SiparisID
          {
               get{return sipId;}
          }
          public string Urun
          {
               get{return ur;}
               set{ur=value;}
          }
          public double BirimFiyat
          {
               get{return bf;}
               set{bf=value;}
          }
          public int Miktar
          {
               get{return mkt;}
               set{mkt=value;}
          }
          public void SiparisDetay()
          {
               Console.WriteLine("----Siparisler----");
               Console.WriteLine("Urun:"+ur+" Birim Fiyat"+bf.ToString()+" Miktar:"+mkt.ToString());
          }
     }
     class Class1
     {
          static void Main(string[] args)
          {
               Sepet spt1=new Sepet();

               spt1.Isim="Burak";
               spt1.Soyisim="Software";
               spt1.Meslek="Mat.Müh";
               spt1.Urun="Modem 56K";
               spt1.BirimFiyat=50000000;
               spt1.Miktar=2;
               spt1.MusteriDetay();
               spt1.SiparisDetay();
          }
     }
}
```

![mk39_1.gif](/assets/images/2004/mk39_1.gif)

Şekil 1. Programın Çalışmasının Sonucu.

Yukarıdaki kodlarda aslında değişik olarak yaptığımız bir şey yok. Sadece oluşturduğumuz arayüzleri bir sınıfa uyguladık ve çok kalıtımlılığı gerçekleştirmiş olduk. Ancak bu noktada dikkat etmemiz gereken bir unsur vardır. Eğer arayüzler aynı isimli metodlara sahip olurlarsa ne olur? Bu durumda arayüzlerin uygulandığı sınıfta, ilgili metodu bir kez yazmamız yeterli olucaktır. Söz gelimi, yukarıdaki örneğimizde, Baslat isimli ortak bir metodun arayüzlerin ikisi içinde tanımlanmış olduğunu varsayalım.

```csharp
public interface IMusteri
{
     void MusteriDetay();
     int ID{get;}
     string Isim{get;set;}
     string Soyisim{get;set;}
     string Meslek{get;set;}
     void Baslat();
}

public interface ISiparis
{
     int SiparisID{get;}
     string Urun{get;set;}
     double BirimFiyat{get;set;}
     int Miktar{get;set;}
     void SiparisDetay();
     void Baslat();
}
```

Şimdi bu iki arayüzde aynı metod tanımına sahip. Sınıfımızda bu metodları iki kez yazmak anlamsız olucaktır. O nedenle sınıfımza aşağıdaki gibi tek bir Baslat metodu ekleriz. Sınıf nesnemizi oluşturduğumuzda, Baslat isimli metodu aşağıdaki gibi çalıştırabiliriz.

spt1.Baslat ();

Fakat bazı durumlarda, arayüzlerdeki metodlar aynı isimlide olsalar, arayüzlerin uygulandığı sınıf içerisinde söz konusu metod, arayüzlerin her biri için ayrı ayrıda yazılmak istenebilir. Böyle bir durumda ise sınıf içerisindeki metod yazımlarında arayüz isimlerini de belirtiriz.Örneğin;

```csharp
void IMusteri.Baslat()
{
     Console.WriteLine("Müşteriler hazırlandı...");
}
void ISiparis.Baslat()
{
     Console.WriteLine("Siparişler hazırlandı...");
}
```

Metodların isimleri başında hangi arayüz için yazıldıklarına dikkat edelim. Diğer önemli bir nokta public belirtecinin kullanılmayışıdır. Public belirtecini kullanmamız durumunda, "The modifier 'public'is not valid for this item" derleyici hatasını alırız. Çünkü, metodumuzu public olarak tanımlamaya gerek yoktur. Nitekim, bu metodların kullanıldığı sınıflara ait nesnelerden, bu metodları çağırmak istediğimizde doğrudan çağıramadığımız görürüz. Çünkü derleyici hangi arayüzde tanımlanmış metodun çağırılması gerektiğini bilemez. Bu metodları kullanabilmek için, nesne örneğini ilgili arayüz tiplerine dönüştürmemiz gerekmektedir. Bu dönüştürmenin yapılması ilgili sınıf nesnesinin, arayüz tipinden değişkenlere açık bir şekilde dönüştürülmesi ile oluşur. İşte bu yüzdende bu tip metodlar, tanımlandıkları sınıf içinde public yapılamazlar. Bu açıkça dönüştürme işlemide aşağıdaki örnek satırlarda görüldüğü gibi olur.

```csharp
IMusteri m=(IMusteri)spt1;
ISiparis s=(ISiparis)spt1;
```

İşte şimdi istediğimiz metodu, bu değişken isimleri ile birlikte aşağıdaki örnek satırlarda olduğu gibi çağırabiliriz.

```csharp
m.Baslat();
s.Baslat();
```

![mk39_2.gif](/assets/images/2004/mk39_2.gif)

Şekil 2. Programın Çalışmasının Sonucu.

Geldik bir makalemizin daha sonuna ilerleyen makalelerimizde arayüzleri incelemeye devam edeceğiz. Hepinize mutlu günler dilerim.