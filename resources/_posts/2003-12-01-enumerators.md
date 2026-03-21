---
layout: post
title: "Enumerators"
date: 2003-12-01 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - enums
  - type
  - class
  - delegate
  - interface
  - cts
---
Bugünkü makalemizde, kendi değer türlerimizi oluşturmanın yollarından birisi olan Enumerator’ları inceleyeceğiz. C# dilinde veri depolamak için kullanabileceğim temel veri türleri yanında kendi tanımlayabileceğimiz türlerde vardır. Bunlar Structs (Yapılar), Arrays (Diziler) ve Enumerators (Numaralandırıcılar) dır. Numaralandırıcılar, sınırlı sayıda değer içeren değişkenler yaratmamıza olanak sağlarlar. Burada bahsi geçen değişken değerleri bir grup oluştururlar ve sembolik bir adla temsil edilirler. Numaralandırıcıları kullanma nedenlerimizden birisi verilere anlamlar yüklekleyerek, program içerisinde kolay okunabilmelerini ve anlaşılabilmelerini sağlamaktır. Örneklerimizde bu konuyu çok daha iyi anlıyacaksınız. Bir Numaralandırıcı tanımlamak için aşağıdaki syntax kullanılır.

```csharp
Kapsam belirteçleri

enum numaralandırıcıAdi
{

     birinciUye,

     ikinciUye,

     ucuncuUye,

} 
```

Kapsam belirteçleri protected,public,private,internal yada new değerini alır ve numaralandırıcının yaşayacağı kapsamı belirtir. Dikkat edilecek olursa, elemanlara herhangibi değer ataması yapılmamıştır. Nitekim Numaralandırıcıların özelliğidir bu. İlk eleman 0 değerine sahip olmak üzere diğer elemanlar 1 ve 2 değerlerini sahip olucak şekilde belirlenirler. Dolayısıyla programın herhangibir yerinde bu numaralandırıcıya ait elemana ulaştığımızda, bu elemanın index değerine erişmiş oluruz. Gördüğünüz gibi numaralandırıcı kullanmak okunurluğu arttırmaktadır.Dilersek numaralandırıc elemanlarının 0 indexinden değil de her hangibir değerden başlamasını sağlayabilir ve hatta diğer elemanlarada farklı index değerleri atayabiliriz. Basit bir Numaralandırıcı örneği ile konuyu daha iyi anlamaya çalışalım.

```csharp
using System;

namespace enumSample1
{
    class Class1
    {
        /* Haftanın günlerini temsil edicek bir numaralandırıcı tipi oluşturuyoruz. Pazartesi 0 index değerine sahip iken Pazar 6 index değerine sahip olucaktır.*/
        enum Gunler
        {
            Pazartesi,
            Sali,
            Carsamba,
            Persembe,
            Cuma,
            Cumartesi,
            Pazar
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Pazartesi gününün değeri={0}", (int)Gunler.Pazartesi);
            Console.WriteLine("Çarşamba günün değeri={0}", (int)Gunler.Carsamba);
        }
    }
}
```

Burada Gunler. Yazdıktan sonar VS.NET ‘in intellisense özelliği sayesinde, numaralandırıcının sahip olduğu değerler kolayca ulaşabiliriz.

![mk11_1.jpg](/assets/images/2003/mk11_1.jpg)

Şekil 1. Intellisense sağolsun.

Programı çalıştıracak olursak aşağıdaki ekran görüntüsünü elde ederiz.

![mk11_2.jpg](/assets/images/2003/mk11_2.jpg)

Şekil 2. Ilk Ornek

Şimdi başka bir örnek geliştirelim. Bu kez numaralandırıcının değerleri farklı olsun.

```csharp
enum Artis
{
     Memur=15,
     Isci=10,
     Muhendis=8,
     Doktor=17,
     Asker=12
} 

static void Main(string[] args)
{
     Console.WriteLine("Memur maaşı zam artış oranı={0}",(int)Artis.Memur);
     Console.WriteLine("Muhendis maaşı zam artış oranı= {0}",(int)Artis.Muhendis); 
}
```

![mk11_3.jpg](/assets/images/2003/mk11_3.jpg)

Şekil 3. İkinci Örneğin Sonucu

Dikkat edicek olursak, numaralandırıcıları program içinde kullanırken, açık olarak (explicit) bir dönüşüm yapmaktayız. Şu ana kadar numaralandırıcı elemanlarınıa integer değerler atadık. Ama dilersek Long tipinden değerde atayabiliriz. Fakat bu durumda enum ‘ ın değer türünüde belirtmemiz gerekmektedir.Örneğin,

```csharp
enum Sinirlar:long
{
     EnBuyuk=458796452135L,
     EnKucuk=255L
} 

static void Main(string[] args)
{
    Console.WriteLine("En üst sınır={0}",(long)Sinirlar.EnBuyuk);
    Console.WriteLine("Muhendis maaşı zam artış oranı={0}",(long)Sinirlar.EnKucuk);
} 
```

Görüldüğü gibi Sinirlar isimli numaralandırıcı long tipinde belirtilmiştir. Bu sayede numaralandırıcı elemanlarına long veri tipinde değerler atanabilmiştir. Dikkat edilecek bir diğer noktada, bu elemanlara ait değerleri kullanırken, long tipine dönüştürme yapılmasıdır. Bir numaralandırıcı varsayılan olarak integer tiptedir. Bu nedenle integer değerleri olan bir numaralandırıcı tanımlanırken int olarak belirtilmesine gerek yoktur. Şimdi daha çok işe yarar bir örnek geliştirmeye çalışalım. Uygulamamız son derece basit bir forma sahp ve bir kaç satır koddan oluşuyor. Amacımız numaralandırıcı kullanmanın programcı açısından işleri daha da kolaylaştırıyor olması. Uygulamamız bir Windows Application. Form tasarımımız aşağıdaki gibi olucak.

![mk11_4.jpg](/assets/images/2003/mk11_4.jpg)

Şekil 4. Form Tasarımımız.

Form yüklenirken Şehir Kodlarının yer aldığı comboBox kontrolümüz otomatik olarak numaralandırıcının yardımıyla doldurulucak. İşte program kodları.

```csharp
public enum AlanKodu
{
     Anadolu=216,
     Avrupa=212,
     Ankara=312,
     Izmir=412
}

private void Form1_Load(object sender, System.EventArgs e)
{
     comboBox1.Items.Add(AlanKodu.Anadolu);
     comboBox1.Items.Add(AlanKodu.Ankara);
     comboBox1.Items.Add(AlanKodu.Avrupa);
     comboBox1.Items.Add(AlanKodu.Izmir);
}
```

İşte sonuç,

![mk11_5.jpg](/assets/images/2003/mk11_5.jpg)

Şekil 5. Sonuç.

Aslında bu comboBox kontrolünü başka şekillerde de alan kodları ile yükleyebiliriz. Bunu yapmanın sayısız yolu var. Burada asıl dikkat etmemiz gereken nokta numaralandırıcı sayesinde bu sayısal kodlarla kafamızı karıştırmak yerine daha bilinen isimler ile aynı sonuca ulaşmamızdır. Geldik bir makalemizin daha sonuna. İlerliyen makalelerimizde bu kez yine kendi değer tiplerimizi nasıl yaratabileceğimize struct kavramı ile devam edeceğiz. Hepinize mutlu günler dilerim.