---
layout: post
title: "Struct Kavramı ve Class ile Struct Arasındaki Farklar"
date: 2003-12-04 11:00:00 +0300
categories:
  - csharp
tags:
  - struct
  - class
  - csharp
---
Bugünkü makalemizde struct kavramını incelemeye çalışacağız. Hatırlayacağınız gibi, kendi tanımladığımız veri türlerinden birisi olan Numaralandırıcıları (Enumerators) görmüştük. Benzer şekilde diğer bir veri tipide struct (yapı) lardır.Yapılar, sınıflar ile büyük benzerleklik gösterirler. Sınıf gibi tanımlanırlar. Hatta sınıflar gibi, özellikler,metodlar,veriler, yapıcılar vb... içerebilirler. Buna karşın sınıflar ile yapılar arasında çok önemli farklılıklar vardır.

Herşeyden önce en önemli fark, yapıların değer türü olması ve sınıfların referans türü olmasıdır. Sınıflar referans türünden oldukları için, bellekte tutuluş biçimleri değer türlerine göre daha farklıdır. Referans tiplerinin sahip olduğu veriler belleğin öbek (heap) adı verilen tarafında tutulurken, referansın adı stack (yığın) da tutulur ve öbekteki verilerin bulunduğu adresi işaret eder. Ancak değer türleri belleğin stack denilen kısmında tutulurlar. Aşağıdaki şekil ile konuyu daha net canlandırabiliriz.

![mk12_1.gif](/assets/images/2003/mk12_1.gif)

Şekil 1. Referans Tipleri

Aşağıdaki şekilde ise değer tiplerinin bellekte nasıl tutulduğunu görüyorsunuz.

![mk12_2.gif](/assets/images/2003/mk12_2.gif)

Şekil 2. Değer Tipleri

İşte sınıflar ile yapılar arasındaki en büyük fark budur. Peki bu farkın bize sağladığı getiriler nelerdir? Ne zaman yapı ne zaman sınıf kullanmalıyız? Özellikle metodlara veriler aktarırken bu verileri sınıf içerisinde tanımladığımızda, tüm veriler metoda aktarılacağını sadece bu verilerin öbekteki başlangıç adresi aktarılır ve ilgili parametrenin de bu adresteki verilere işaret etmesi sağlanmış olur. Böylece büyük boyutlu verileri stack’ta kopyalayarak gereksiz miktarda bellek harcanmasının önüne geçilmiş olunur. Ancak küçük boyutlarda veriler ile çalışırken bu verileri sınıflar içerisinde kullandığımızda bu kezde gereksiz yere bellek kullanıldığı öbek şişer ve performans düşer. Bu konudaki uzman görüş 16 byte’tan küçük veriler için yapıların kullanılması, 16 byte’tan büyük veriler için ise sınıfların kullanılmasıdır.

Diğer taraftan yapılar ile sınıflar arasında başka farklılıklarda vardır. Örneğin bir yapı için varsayılan yapıcı metod (default constructor) yazamayız. Derleyici hatası alırız. Ancak bu değişik sayıda parametreler alan yapıcılar yazmamızı engellemez. Oysaki sınıflarda istersek sınıfın varsayılan yapıcı metodunu kendimiz yazabilmekteyiz.

Bir yapı içersinde yer alan constructor metod (lar) içinde tanımlamış olduğumuz alanlara başlangıç değerlerini atamak zorundayız. Oysaki bir sınıftaki constructor (lar) içinde kullanılan alanlara başlangıç değerlerini atamaz isek, derleyici bizim yerimize sayısal değerlere 0, boolean değerlere false vb... gibi başlangıç değerlerini kendisi otomatik olarak yapar. Ancak derleyici aynı işi yapılarda yapmaz. Bu nedenle bir yapı içinde kullandığımız constructor (lar) daki tanımlamış olduğumuz alanlara mutlaka ilk değerlerini vermemiz gerekir. Ancak yinede dikkat edilmesi gereken bir nokta vardır. Eğer yapı örneğini varsayılan yapılandırıcı ile oluşturursak bu durumda derleyici yapı içinde kullanılan alanlara ilk değerleri atanmamış ise kendisi ilk değerleri atar. Unutmayın, parametreli constructorlarda her bir alan için başlangıç değerlerini bizim vermemiz gerekmektedir. Örneğin, aşağıdaki Console uygulamasını inceleyelim.

```csharp
using System;

namespace StructSample1
{
    struct Zaman
    {
        private int saat, dakika, saniye;
        private string kosucuAdi;
        public string Kosucu
        {
            get
            {
                return kosucuAdi;
            }
            set
            {
                kosucuAdi =value;
            }
        }
        public int Saat
        {
            get
            {
                return saat;
            }
            set
            {
                saat =value;
            }
        }

        public int Dakika
        {
            get
            {
                return dakika;
            }
            set
            {
                dakika =value;
            }
        }

        public int Saniye
        {
            get
            {
                return saniye;
            }
            set
            {
                saniye =value;
            }
        }
    }

    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
            Zaman z;
            Console.WriteLine("Koşucu:" + z.Kosucu);
            Console.WriteLine("Saat:" + z.Saat.ToString());
            Console.WriteLine("Dakika:" + z.Dakika.ToString());
            Console.WriteLine("Saniye:" + z.Saniye.ToString());
        }
    }
}
```

Yukarıdaki kod derlenmeyecektir. Nitekim derleyici “Use of unassigned local variable 'z'” hatası ile z yapısı için ilk değerlerin atanmadığını bize söyleyecektir. Ancak z isimli Zaman yapı türünü new anahtarı ile tanımlarsak durum değişir.

```csharp
Zaman z; 

Satırı yerine 

Zaman z=new Zaman();
```

yazalım.Bu durumda kod derlenir. Uygulama çalıştığında aşağıdaki ekran görüntüsü ile karşılaşırız. Görüldüğü gibi z isimli yapı örneğini new yapılandırıcısı ile tanımladığımızda, derleyici bu yapı içindeki özelliklere ilk değerleri kendi atamıştır. Kosucu isimli özellik için null, diğer integer özellikler için ise 0.

![mk12_3.jpg](/assets/images/2003/mk12_3.jpg)

Şekil 3.New yapılandırıcısı ile ilk değer ataması.

Yine önemli bir farkta yapılarda türetme yapamıyacağımızdır. Bilindiği gibi bir sınıf oluşturduğumuzda bunu başka bir temel sınıftan kalıtım yolu ile türetebilmekteyiz ki inheritance olarak geçen bu kavramı ilerliyen makalelerimizde işleyeceğiz. Ancak bir yapıyı başka bir yapıyı temel alarak türetemeyiz. Şimdi yukarıda verdiğimiz örnekteki yapıdan başka bir yapı türetmeye çalışalım.

```csharp
struct

yeni:Zaman
{ 

} 
```

satırlarını kodumuza ekleyelim.Bu durumda uygulamayı derlemeye çalıştığımızda aşağıdaki hata mesajını alırız.

'Zaman': type in interface list is not an interface

Bu belirgin farklılıklarıda inceledikten sonra dilerseniz örneklerimiz ile konuyu pekiştirmeye çalışalım.

```csharp
using System; 

namespace StructSample1
{
     struct Zaman
     {
        private int saat,dakika,saniye;
        private string kosucuAdi;
        /* Yapı için parametreli bir constructor metod tanımladık. Yapı içinde yer alan kosucuAdi,saat,dakika,saniye alanlarına ilk değerlerin atandığına dikkat edelim. Bunları atamassak derleyici hatası alırız. */
        public Zaman(string k,int s,int d,int sn)
        {
               kosucuAdi=k;
               saat=s;
               dakika=d;
               saniye=sn;
          } 

     /* Bir dizi özellik tanımlayarak private olarak tanımladığımız asıl alanların kullanımını kolaylaştırıyoruz. */
    public string Kosucu
    {
        get
        {
            return kosucuAdi;
        }
        set
        {
            kosucuAdi=value;
        }
    } 
         public int Saat
         {             
             get
             {               
                 return saat;
             }
             set
             {
                 saat=value;
             }
         }  
         public int Dakika
         {
             get
             {
                 return dakika;
             }         
             set
             {
                 dakika=value;
             }
         }       
         public int Saniye
         {
             get
             {
                 return saniye;
             }            
             set
             {
                 saniye=value;
             }
          }
     } 
    class Class1
    {         
        static void Main(string[] args)
        {
            /* Zaman yapısı içinde kendi yazdığımız parametreli constuructorlar ile Zaman yapısı örnekleri oluşturuyoruz. Yaptığımız bu tanımlamarın ardından belleğin stack bölgesinde derhal 4 adet değişken oluşturulur ve değerleri atanır. Yani kosucuAdi,saat,dakika,saniye isimli private olarak tanımladığımız alanlar bellekte stack bölgesinde oluşturulur ve atadığımız değerleri alırlar. Bu oluşan veri dizisinin adıda Zaman yapısı tipinde olan Baslangic ve Bitis değişkenleridir. */ 
 
```

![mk12_4.gif](/assets/images/2003/mk12_4.gif)

```csharp
               Zaman Baslangic= new Zaman("Burak",1,15,23);
               Zaman Bitis=new Zaman("Burak",2,20,25); 
              

/* Zaman yapısı içinde tanımladığımız özelliklere erişip işlem yapıyoruz. Burada elbette zamanları birbirinden bu şekilde çıkarmak matematiksel olarak bir cinayet. Ancak amacımız yapıların kullanımını anlamak. Bu satırlarda yapı içindeki özelliklerimizin değerlerine erişiyor ve bunların değerleri ile sembolik işlemler yapıyoruz */             

int saatFark=Bitis.Saat-Baslangic.Saat;
              
int dakikaFark=Bitis.Dakika-Baslangic.Dakika;
              
int saniyeFark=Bitis.Saniye-Baslangic.Saniye; 
               Console.WriteLine("Fark {0} saat, {1} dakika, {2} saniye",saatFark,dakikaFark,saniyeFark);      
          }
     }
} 
```

Bir sonraki makalemizde görüşmek dileğiyle. Hepinize mutlu günler dilerim.