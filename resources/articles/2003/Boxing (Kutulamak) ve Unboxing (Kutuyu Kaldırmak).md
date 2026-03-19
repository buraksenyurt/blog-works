---
title: "Boxing (Kutulamak) ve Unboxing (Kutuyu Kaldırmak)"
pubDate: 2003-12-30 12:00:00
categories:
  - C#
tags:
  - C#
  - .NET
  - Delegates
---

# Boxing (Kutulamak) ve Unboxing (Kutuyu Kaldırmak)
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, Boxing ve Unboxing kavramlarını incelemeye çalışacağız. Boxing değer türü bir değişkeni, referans türü bir nesneye aktarmaktır. Unboxing işlemi ise bunun tam tersidir. Yani referans türü değişkenin işaret ettiği değeri tekrar, değer türü bir değişkene aktarmaktır. Bu tanımlarda karşımıza çıkan ve bilmemiz gereken en önemli noktalar, değer türü değişkenler ile referans türü nesnelerin bellekte tutuluş şekilleridir.

Net ortamında iki tür veri tipi vardır. Referans tipleri (reference type) ve değer tipleri (value type). İki veri türünün bellekte farklı şekillerde tutulmaları nedeni ile boxing ve unboxing işlemleri gündeme gelmiştir. Bu nedenle öncelikle bu iki farklı veri tipinin bellekte tutuluş şekillerini iyice anlamamız gerekmektedir.

Bu anlamda karşımıza iki önemli bellek bölgesi çıkar. Yığın (stack) ve öbek (heap). Değer tipi değişkenler, örneğin bir integer değişken vb.. belleğin stack adı verilen kısmında tutulurlar. DotNet’te yer alan değer türleri aşağıdaki tabloda yer almaktadır. Bu tiplerin stack bölegesinde nasıl tutulduğuna ilişkin açıklayıcı şekli de aşağıda görebilirsiniz.

Value type (Değer Tipler)

bool

long

byte

sbyte

char

short

decimal

Struct (Yapılar)

double

uint

Enum (Numaralandırıcılar)

ulong

float

ushort

int

Tablo 1. Değer Tipleri

![mk30_1.gif](images/mk30_1.gif)

Şekil 1. Değer Tiplerinin Bellekte Tutuluşu

Şekildende görüldüğü gibi, Değer Türleri bellekte, Stack dediğimiz bölgede tutulurlar. Şimdi buraya kadar anlaşılmayan bir şey yok. İlginç olan reference (başvuru) tiplerinin bellekte nasıl tutulduğudur. Adındanda anlaşıldığı gibi reference tipleri asıl veriye bir başvuru içeriler. Örneğin sınıflardan türettiğimiz nesneler bu tiplerdendir. Diğer başvuru tipleri ise aşağıdaki tabloda yer almakdadır.

Reference Type (Başvuru Tipleri)

Class (sınıflar)

Interface (arayüzler)

Delegate (delegeler)

Object

String

Tablo 2. Başvuru Tipleri

Şimdi gelin başvuru türlerinin bellekte nasıl tutulduklarına bakalım.

![mk30_2.gif](images/mk30_2.gif)

Şekil 2. Başvuru tiplerinin bellekte tutuluşu.

Görüldüğü gibi,başvuru tipleri hakikatende isimlerinin layığını vermekteler. Nitekim asıl veriler öbek’te tutulurken yığında bu verilere bir başvuru yer almaktadır. İki veri türü arasındaki bu farktan sonra bir farkta bu verilerle işimiz bittiğinde geri iade ediliş şekilleridir. Değer türleri ile işimiz bittiğinde bunların yığında kapladıkları alanlar otomatik olarak yığına geri verilir. Ancak referans türlerinde sadece yığındaki başvuru sisteme geri veririlir. Verilerin tutulduğu öbekteki alanlar, Garbage Collector’un denetimindedirler ve ne zaman sisteme iade edilicekleri tam olarak bilinmez. Bu ayrı bir konu olmakla beraber oldukça karmaşıktır. İlerleyen makalelerimizde bu konudan da bahsetmeye çalışacağım.

Değer türleri ile başvuru türleri arasındaki bu temel farktan sonra gelelim asıl konumuza.. NET’te her sınıf aslında en üst sınıf olan Object sınıfından türer. Yani her sınıf aslında System.Object sınıfından kalıtım yolu ile otomatik olarak türetilmiş olur. Sorun object gibi referans bir türe, değer tipi bir değerin aktarılmasında yaşanır.. NET’te herşey aslında birer nesne olarak düşünülebilir. Bir değer türünü bir nesneye atamaya çalıştığımızda, değer türünün içerdiği verinin bir kopyasının yığından alınıp, öbeğe taşınması ve nesnenin bu veri kopyasına başvurması gerekmektedir. İşte bu olay kutulama (boxing) olarak adlandırılır. Bu durumu minik bir örnek ile inceleyelim.

```csharp
using System; 

namespace boxunbox
{
      class Class1
      {
		  static void Main(string[] args)
		  {
				double db=509809232323;
				object obj; 
				obj=db; 
				Console.WriteLine(db.ToString());
				Console.WriteLine(obj.ToString());
				db+=1;
				Console.WriteLine(db.ToString());
				Console.WriteLine(obj.ToString());
		  }
      }
} 
```

Kodumuzun çalışmasını inceleyelim. Db isimli double değişkenimiz bir değer tipidir. Örnekte bu değer tipini object tipinden bir nesneye aktarıyoruz. Bu halde bu değerler içerisindeki verileri ekrana yazdırıyoruz. Sonra db değerimizi 1 arttırıyor ve tekrar bu değerlerin içeriğini ekrana yazdırıyoruz. İşte sonuç;

![mk30_3.gif](images/mk30_3.gif)

Şekil 3 Boxing İşlemi

Görüldüğü gibi db değişkenine yapılan arttırım object türünden obj nesnemize yansımamıştır. Çünkü boxing işlemi sonucu, obj nesnesi, db değerinin öbekteki kopyasına başvurmaktadır. Oysaki artım db değişkeninin yığında yer alan orjinal değeri üzerinde gerçekleşmektedir. Bu işlemi açıklayan şekil aşağıda yer almaktadır.

![mk30_4.gif](images/mk30_4.gif)

Şekil 4. Boxing İşlemi

Boxing işlemi otomatik olarak yapılan bir işlemdir. Ancak UnBoxing işleminde durum biraz daha değişir. Bu kez, başvuru nesnemizin işaret ettiği veriyi öbekten alıp yığındaki bir değer tipi alanı olarak kopyalanması söz konusudur. İşte burada tip uyuşmazlığı denen bir kavramla karşılaşırız. Öbekten, yığına kopylanacak olan verinin, yığında kendisi için ayrılan yerin aynı tipte olması veya öbekteki tipi içerebilecek tipte olması gerekmektedir. Örneğin yukarıdaki örneğimize unboxing işlemini uygulayalım. Bu kez integer tipte bir değer türüne atama gerçekleştirelim.

```csharp
using System; 

namespace boxunbox
{
      class Class1
      {
            static void Main(string[] args)
            {
                  double db=509809232323;
                  object obj; 
                  obj=db; 
                  Console.WriteLine(db.ToString());
                  Console.WriteLine(obj.ToString());
                  db+=1;
                  Console.WriteLine(db.ToString());
                  Console.WriteLine(obj.ToString()); 
                  int intDb;
                  intDb=(int)obj;
                  Console.WriteLine(intDb.ToString());
            }
      }
}
```

Bu kodu çalıştırdığımızda InvalidCastException istisnasının fırlatılacağını görüceksiniz. Çünü referenas tipimizin öbekte başvurduğu veri tipi integer bir değer için fazla büyüktür. Bu noktada (int) ile açıkça dönüşümü bildirmiş olsak dahi bu hatayı alırız.

![mk30_5.gif](images/mk30_5.gif)

Şekil 5. InvalidCastException İstisnası

Ancak küçük tipi, büyük tipe dönüştürmek gibi bir serbestliğimiz vardır. Örneğin,

```csharp
using System; 
namespace boxunbox
{
	class Class1
	{
		static void Main(string[] args)
		{
			double db=509809232323;
			object obj;
			obj=db;
			Console.WriteLine(db.ToString());
			Console.WriteLine(obj.ToString());
			db+=1;
			Console.WriteLine(db.ToString());
			Console.WriteLine(obj.ToString());
			/*int intDb;
			intDb=(int)obj;
			Console.WriteLine(intDb.ToString());*/
			double dobDb;                                   dobDb=(double)obj;
			Console.WriteLine(dobDb.ToString());   
		}
	}
}
```

Bu durumda kodumuz sorunsuz çalışacaktır. Çünkü yığında yer alan veri tipi daha büyük boyutlu bir değer türünün içine koyulabilir. İşte buradaki aktarım işlemi unboxing olarak isimlendirilmiştir. Yani boxing işlemi ile kutulanmış bir veri kümesi, öbekten alınıp tekrar yığındaki bir Alana konulmuş dolayısıyla kutudan çıkartılmıştır. Olayın grafiksel açıklaması aşağıdaki gibidir.

![mk30_6.gif](images/mk30_6.gif)

Şekil 6. Unboxing İşlemi

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
