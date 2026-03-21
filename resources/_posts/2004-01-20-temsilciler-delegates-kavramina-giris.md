---
layout: post
title: "Temsilciler (Delegates) Kavramına Giriş"
date: 2004-01-20 10:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - delegate
  - delegates
  - attribute
  - oop
  - multicast-delegates
---
Bugünkü makalemizde, C# programlama dilinde ileri seviye kavramlardan biri olan Temsilcileri (delegates) incelemeye başlayacağız. Temsilciler ileri seviye bir kavram olmasına rağmen, her seviyden C# programcısının bilmesi gereken unsurlardandır. Uygulamalarımızı temsilciler olmadan da geliştirebiliriz. Ancak bu durumda, yapamıyacaklarımız, yapabileceklerimizin önüne geçecektir. Diğer yandan temsilcilerin kullanımını gördükçe bize getireceği avantajları daha iyi anlayacağımız kanısındayım. Bu makalemizde temsilcileri en basit haliyle anlamaya çalışıcağız.

Temsilci (delegate), program içerisinde bir veya daha fazla metodu gösteren (işaret eden), referans türünden bir nesnedir. Programlarımızda temsilciler kullanmak istediğimizde, öncelikle bu temsilcinin tanımını yaparız. Temsilci tanımları, arayüzlerdeki metod tanımlamaları ile neredeyse aynıdır. Tek fark delegate anahtar sözcüğünün yer almasıdır. Bununla birlikte, bir temsilci tanımlandığında, aslında işaret edebileceği metod (ların) imzalarınıda belirlemiş olur. Dolayısıyla, bir temsilciyi sadece tanımladığı metod imzasına uygun metodlar için kullanabiliceğimizi söyleyebiliriz. Temsilci tanımları tasarım zamanında yapılır. Bir temsilciyi, bir metodu işaret etmesi için kullanmak istediğimizde ise, çalışma zamanında onu new yapılandırıcısı ile oluşturur ve işaret etmesini istediğimiz metodu ona parametre olarak veririz. Bir temsilci tanımı genel haliyle, aşağıdaki şekildeki gibidir.

![mk43_1.gif](/assets/images/2004/mk43_1.gif)

Şekil 1. Temsilci tanımlaması.

Şekildende görüldüğü gibi, temsilciler aslında bir metod tanımlarlar fakat bunu uygulamazlar. İşte bu özellikleri ile arayüzlerdeki metod tanılamalarına benzerler. Uygulamalarımızda, temsilci nesneleri ile göstermek yani işaret etmek istediğimiz metodlar bu imzaya sahip olmalıdır. Bildiğiniz gibi metod imzaları, metodun geri dönüş tipi ve aldığı parametreler ile belirlenmektedir.

Bir temsilcinin tanımlanması, onu kullanmak için yeterli değildir elbette. Herşeyden önce bir amacımız olmalıdır. Bir temsilciyi çalışma zamanında oluşturabiliriz ve kullanabiliriz. Bir temsilci sadece bir tek metodu işaret edebileceği gibi, birden fazla metod için tanımlanmış ve oluşturulmuş temsilcileride kullanabiliriz. Diğer yandan, tek bir temsilcide birden fazla temsilciyi toplayarak bu temsilcilerin işaret ettiği, tüm metodları tek bir seferde çalıştırma lüksünede sahibizdir. Ancak temsilciler gerçek anlamda iki amaçla kullanılırlar. Bunlardan birincisi olaylardır (events). Diğer yandan, bugünkü makalemizde işleyeceğimiz gibi, bir metodun çalışma zamanında, hangi metodların çalıştırılacağına karar vermesi gerektiği durumlarda kullanırız. Elbette bahsetmiş olduğumuz bu amacı, herhangibir temsilye ihtiyaç duymadan da gerçekleştirebiliriz. Ancak temsilcileri kullanmadığımızda, bize sağladığı üstün programlama tekniği, kullanım kolaylığı ve artan verimliliğide göz ardı etmiş oluruz.

Şimdi dilerseniz bahsetmiş olduğumuz bu amaçla ilgili bir örnek verelim ve konuyu daha iyi kavramaya çalışalım. Örneğin, personelimizin yapmış olduğu satış tutarlarına göre, prim hesabı yapan ve ilgili yerlere bu değişiklikleri yazan bir projemiz olsun. Burada primlerin hesaplanması için değişik katsayılar, yapılan satışın tutarına göre belirlenmiş olabilir. Örneğin bu oranlar düşük, orta ve yüksek olarak tanımlanmış olsun. Personel hangi gruba giriyorsa, metodumuz ona uygun metodu çağırsın. İşte bu durumda karar verici metodumuz, çalıştırabileceği metodları temsil eden temsilci nesnelerini parametre olarak alır. Yani, çalışma zamanında ilgili metodlar için temsilci nesneleri oluşturulur ve karar verici metoda, hangi metod çalıştırılacak ise onun temsilcisi gönderilir. Böylece uygulamamız çalıştığında, tek yapmamız gereken hangi metodun çalıştırılması isteniyorsa, bu metoda ilişkin temsilcinin, karar verici metoda gönderilmesi olucaktır.

Oldukça karşışık görünüyor. Ancak örnekleri yazdıkça daha iyi kavrayacağınıza inanıyorum. Şimdiki örneğimizde, temsilcilerin tasarım zamanında nasıl tanımlandığını, çalışma zamanında nasıl oluşturulduklarını ve karar verici bir metod için temsilcilerin nasıl kullanılacağını incelemeye çalışacağız.

```csharp
using System;

namespace Delegates1
{
     public class Calistir
     {
          public static int a;
          public delegate void temcilci(int deger); /* Temsilci tanımlamamızı yapıyoruz. Aynı zamanda temsilcimiz , değer döndürmeyen ve integer tipte tek bir parametre alan bir metod tanımlıyor. Temsilcimizin adı ise temsilci.*/

          /* Şimdi bu temsilciyi kullacanak bir metod yazıyoruz. İşte karar verici metodumuz budur. Dikkat ederseniz metodumuz parametre olarak, temsilci nesnemiz tipinden bir temsilci(Delegate) alıyor. Daha sonra metod bloğu içinde, parametre olarak geçirilen bu temsilcinin işaret ettiği metod çağırılıyor ve bu metoda parametre olarak integer tipte bir değer geçiriliyor. Kısaca, metod içinden, temsilcinin işaret ettiği metod çağırılıyor. Burada, temsilci tanımına uygun olan metodun çağırılması garanti altına alınmıştır. Yani, programın çalışması sırasında, new yapılandırıcısı kulllanarak oluşturacağımız bir temsilci(delegate), kendi metod tanımı ile uyuşmayan bir metod için yaratılmaya çalışıldığında bir derleyici hatası alacağızdır. Dolayısıyla bu, temsilcilerin yüksek güvenlikli işaretçiler olmasını sağlar. Bu , temsilcileri, C++ dilindeki benzeri olan işaretçilerden ayıran en önemli özelliktir. */
 
          public void Metod1(Calistir.temcilci t)
          {
                t(a);
          }
     }

     class Class1
     {
           /* IkıKat ve UcKat isimli metodlarımız, temsilcimizin programın çalışması sırasında işaret etmesini istediğimiz metodlar. Bu nedenle imzaları, temsilci tanımımızdaki metod imzası ile aynıdır. */
          
          public static void IkiKat(int sayi)
          {
               sayi=sayi*2;
              Console.WriteLine("IkiKat isimli metodun temsilcisi tarafindan çagirildi."+sayi.ToString());
          }
          public static void UcKat(int sayi)   
          {
               sayi=sayi*3;
               Console.WriteLine("UcKat isimli metodun temsilcisi tarafindan çagirildi."+sayi.ToString());
          }
          static void Main(string[] args)
         {
                /* Temsilci nesnelerimiz ilgili metodlar için oluşturuluyor. Burada, new yapılandırıcısı ile oluşturulan temsilci nesneleri parametre olarak, işaret edecekleri metodun ismini alıyorlar. Bu noktadan itibaren t1 isimli delegate nesnemiz IkiKat isimli metodu, t2 isimli delegate nesnemizde UcKat isimli metodu işaret ediceklerdir. */
               Calistir.temcilci t1=new Delegates1.Calistir.temcilci(IkiKat);
               Calistir.temcilci t2=new Delegates1.Calistir.temcilci(UcKat);
 
               Console.WriteLine("1 ile 20 arası değer girin");
                  Calistir.a=System.Convert.ToInt32(Console.ReadLine());
               Calistir c=new Calistir();
              /* Kullanıcının Console penceresinden girdiği değer göre, Calistir sınıfının a isimli integer tipteki değerini 10 ile karşılaştırılıyor. 10 dan büyükse, karar verici metodumuza t1 temsilcisi gönderiliyor. Bu durumda Metod1 isimli karar verici metodumuz, kendi kod bloğu içinde t1 delegate nesnesinin temsil ettiği IkıKat metodunu, Calistir.a değişkeni ile çağırıyor. Aynı işlem tarzı t2 delegate nesnesi içinde geçerli.*/

                   
               if(Calistir.a>=10)
               {
                    c.Metod1(t1);
               }
               else
               {
                    c.Metod1(t2);
               }
          }
     }
}
```

Uygulamamızı çalıştıralım ve bir değer girelim.

![mk43_2.gif](/assets/images/2004/mk43_2.gif)

Şekil 2. Programın çalışmasının sonucu.

Bu basit örnek ile umarım temsilciler hakkında biraz olsun bilgi sahibi olmuşsunuzdur. Şimdi temsilciler ile ilgili kavramlarımıza devam edelim. Yukarıdaki örneğimiz ışığında temsilcileri programlarımızda temel olarak nasıl kullandığımızı aşağıdaki şekil ile daha kolay anlayabileceğimizi sanıyorum.

![mk43_3.gif](/assets/images/2004/mk43_3.gif)

Şekil 3. Temsilcilerin Karar Verici metodlar ile kullanımı.

Yukarıdaki örneğimizde, her bir metod için tek bir temsilci tanımladık ve temsilcileri teker çağırdık. Bu Single-Cast olarak adlandırılmaktadır. Ancak programlarımız da bazen, tek bir temsilciye birden fazla temsilci ekleyerek, birden fazla metodu tek bir temsilci ile çalıştırmak isteyebiliriz. Bu durumda Multi-Cast temsilciler tanımlarız. Şimdi multi-cast temsilciler ile ilgili bir örnek yapalım. Bu örneğimizde t1 isimli temsilcimiz, multi-cast temsilcimiz olucak.

```csharp
using System;

namespace Delegates2
{
     public class temsilciler
     {
          public delegate void dgTemsilci(); /* Temsilcimiz tanımlanıyor. Geri dönüş değeri olmayan ve parametre almayan metodları temsil edebilir. */

          /* Metod1, Metod2 ve Metod3 temsilcilerimizin işaret etmesini istediğimiz metodlar olucaktır.*/
 

          public static void Metod1()
          {
               Console.WriteLine("Metod 1 çalıştırıldı.");
          }
          public static void Metod2()
          {
               Console.WriteLine("PI değeri 3.14 alınsın");
          }
          public static void Metod3()
          {
               Console.WriteLine("Mail gönderildi...");
          }
          /* Temsilcilerimizi çalıştıran metodumuz. Parametre olarak gönderilen temsilciyi, dolayısıyla bu temsilcinin işaret ettiği metodu alıyor. */

          public static void TemsilciCalistir(temsilciler.dgTemsilci dt)
          {
               dt();
/* Temsilcinin işaret ettiği metod çalıştırılıyor.*/
          }
     }
     class Class1
     {
          static void Main(string[] args)
          {
               /* Üç metodumuz içinde temsilci nesnelerimiz oluşturuluyor .*/
               temsilciler.dgTemsilci t1=new Delegates2.temsilciler.dgTemsilci(temsilciler.Metod1);

               temsilciler.dgTemsilci t2=new Delegates2.temsilciler.dgTemsilci(temsilciler.Metod2);

               temsilciler.dgTemsilci t3=new Delegates2.temsilciler.dgTemsilci(temsilciler.Metod3);

               Console.WriteLine("sadece t1");
               temsilciler.TemsilciCalistir(t1);
               Console.WriteLine("---");

               /* Burada t1 temsilcimize, t2 temsilcisi ekleniyor. Bu durumda, t1 temsilcimiz hem kendi metodunu hemde, t2 temsilcisinin işaret ettiği metodu işaret etmeye başlıyor. Bu halde iken TemsilciCalistir metodumuza t1 temsilcisini göndermemiz her iki temsilcinin işaret ettiği metodların çalıştırılmasına neden oluyor.*/

               t1+=t2;
               Console.WriteLine("t1 ve t2");
               temsilciler.TemsilciCalistir(t1);
               Console.WriteLine("---");
               t1+=t3;
/* Şimdi t1 temsilcimiz hem t1, hem t2, hem de t3 temsilcilerinin işaret ettiği metodları işaret etmiş olucak.*/
               Console.WriteLine("t1,t2 ve t3");
               temsilciler.TemsilciCalistir(t1);
               Console.WriteLine("---");
               t1-=t2;

/* Burada ise t2 metodunu t1 temsilcimizden çıkartıyoruz. Böylece, t1 temsilcimiz sadece t1 ve t3 temsilcilerini içeriyor. */
               Console.WriteLine("t1 ve t3");
               temsilciler.TemsilciCalistir(t1);
               Console.WriteLine("---");
          }
     }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk43_4.gif](/assets/images/2004/mk43_4.gif)

Şekil 4. Multi-Cast temsilciler.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde temsilcilerin kullanılıdığı olaylar (events) kavramına gireceğiz. Hepinize mutlu günler dilerim.