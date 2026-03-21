---
layout: post
title: "Tip Güvenli (Type Safety ) Koleksiyonlar Oluşturmak - 2"
date: 2005-07-31 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - strongly-typed-collections
  - collections
---
Bir önceki makalemizde tip güvenli koleksiyon nesnelerimizi CollectionBase sınıfı yardımıyla nasıl oluşturabileceğimizi incelemiştik. CollectionBase bize ArrayList benzeri koleksiyon sınıflarını yazma fırsatı vermektedir. Diğer yandan Hashtable koleksiyonunda olduğu gibi key (anahtar) - value (değer) çiftlerinden oluşacak tip güvenli bir koleksiyon sınıfı yazmak isteyebiliriz. Bu durumda, DictionaryBase sınıfından yaralanabiliriz. DictionaryBase sınıfıda CollectionBase sınıfı gibi abstract yapıdadır. Yani kendisini örnekleyemeyiz. Temel olarak DictionaryBase key-value çiftlerine sahip bir koleksiyonun kullanması gereken üyeleri sunan arayüzlerden türemiştir. Yani IDictionary, IEnumerable ve ICollection arayüzlerini uyarlamaktadır. Dikkat ederseniz CollectionBase sınıfınında türediği IEnumerable ve ICollection arayüzleri DictionaryBase içinde söz konusudur.

![mk131_1.gif](/assets/images/2005/mk131_1.gif)

CollectionBase sınıfı gerekli fonksiyonelliği sağlamak için nasıl ki bir ArrayList koleksiyonunu çevreliyorsa (encapsulate), DictionaryBase sınıfıda bir Hashtable koleksiyonunu çevreler. DictionaryBase sınıfının protected özellikleri (Dictonary, InnerHashtable) yardımıyla oluşturduğumuz koleksiyon içindeki key-value çiftlerine erişilebilir ve eleman ekleme, silme, arama vb. gibi pek çok var olan aksiyonu gerçekleştirebiliriz. Örneğimizi incelediğimizde DictionaryBase yardımıyla tip güvenli bir koleksiyon nesnesi oluşturmanın, CollectionBase kullanıldığında gerçekleştirilen uyarlama ile neredeyse aynı olduğunu görecekseniz. Tek fark, içeride erişilen nesnenin bir Dictionary nesnesi olması ve key-value çiftlerinin söz konusu olmasıdır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
DictionaryBase sınıfı key-value (anahtar - değer) çiftlerinin kullanıldığı Hashtable tipi koleksiyon sınıflarını yazmamızı ve kendi tip güvenliğimizi oluşturabilmemizi sağlar.

Şimdi gelin bir örnek üzerinden bu konuyu incelemeye çalışalım. Bu sefer anahtar-değer çiftleri yapısına uygun bir koleksiyon nesnesi olarak ISBN numarasına sahip Kitapları göz önüne alacağız. ISBN numaraları bizim için key (anahtar) olacak. Bunun karşılığında ise bir Kitap nesnesini value (değer) olarak tutacağız. Bu elbette tipik olarak bir Hashtable koleksiyonu ile de yapılabilir. Ancak bizim amacımız anahtarların (keys) mutlaka integer ve değerlerin (values) mutlaka Kitap tipinden nesne örnekleri olmasıdır. Normal bir Hashtable key-value çiftlerini object tipinden tuttuğu için, herhangibir tipi bu çiftlere atayabiliriz. İşte burada tip güvenliği bizim esas olan amacımız olmaktadır. Bu nedenlede DictionaryBase sınıfı yardımıyla kendi koleksiyon sınıfımızı yazacağız. İlk olarak Kitap nesnemize ait sınıfımızı oluşturalım. Sınıfımızın basit yapısı aşağıdaki gibidir.

![mk131_2.gif](/assets/images/2005/mk131_2.gif)

Sınıfımız kodları;

```csharp
using System;

namespace StrongCollections
{
    public class Kitap
    {
        private int m_ISBN;
        private string m_Baslik;
        private int m_Fiyat;

        public int ISBN
        {
            get{return m_ISBN;}
            set{m_ISBN=value;}
        }
        public string Baslik
        {
            get{return m_Baslik;}
            set{m_Baslik=value;}
        }
        public int Fiyat
        {
            get{return m_Fiyat;}
            set{m_Fiyat=value;}
        }
        public Kitap(int isbn,string baslik,int fiyat)
        {
            m_ISBN=isbn;
            m_Baslik=baslik;
            m_Fiyat=fiyat;
        }
        public override string ToString()
        {
            return m_ISBN.ToString()+" "+m_Baslik+" "+m_Fiyat.ToString();
        }
        public Kitap()
        {
        }
    }
}
```

Şimdi kendi tip güvenli koleksiyon sınıfımızı yazalım. Sınıfımızı DictionaryBase'den türettikten sonra içeriden Dictionary özelliğine erişerek eleman ekleme, çıkarma, arama, bulma gibi işlemleri yapacağımız metodlar ile bir indeksleyici ve anahtarlar (keys) üzerinde öteleme yapabileceğimiz bir numarator metod ekleyeceğiz. Basit olarak sınıfımızın içeriği aşağıdaki gibi olacak.

![mk131_3.gif](/assets/images/2005/mk131_3.gif)

Sınıf kodlarımız;

```csharp
using System;
using System.Collections;

namespace StrongCollections
{
    public class KitapKoleksiyon:DictionaryBase
    {
        public void Ekle(int isbn,Kitap kitap)
        {
            Dictionary.Add(isbn,kitap);
        }
        public void Cikart(int isbn)
        {
            Dictionary.Remove(isbn);
        }
        public bool Varmi(int isbn)
        {
            return Dictionary.Contains(isbn);
        }
        public Kitap Bul(int isbn)
        {
            return (Kitap)Dictionary[isbn];
        }
        public Kitap this[int isbn]
        {
            get{return (Kitap)Dictionary[isbn];}
            set{Dictionary[isbn]=value;}
        }
        public IEnumerator NumaratorAl()
        {
             return Dictionary.Keys.GetEnumerator();
        }
        public KitapKoleksiyon()
        {
        }
    }
}
```

Ekle metodumuz iki parametre almaktadır. Tahmin edeceğiniz gibi ilk parametremiz Hashtable koleksiyonu için gerekli key (anahtar), ikinci parametremiz ise value (değer) dir. Bu metod sayesinde, belli bir isbn numarasının karşılığı olarak bir Kitap nesne örneğini koleksiyonumuza eklemiş oluruz. Cikart metodu, parametre olarak verilen isbn değerini Dictionary içerisinde bulur ve listeden çıkartır. Varmi metodumuz geriye bool tipinden bir değer döndürür. Amacı belirtilen isbn değerinin koleksiyonda yer alıp almadığını belirlemektir. Bunun için Dictionary özelliği üzerinden Contains metodunu çağırırız.

Bul metodumuz ise, parametre olarak girilen isbn'i Dictionary üzerinde arar ve sonucu geriye bir Kitap nesne örneği şeklinde döndürür. Bu dönüştürme gereklidir çünkü Dictionary geriye object tipinden bir nesne örneğini döndürecektir. Indeksleyicimizin indeks değerleri bu sefer key (anahtar) lardır. Dikkat ederseniz tüm metodlarımız int-Kitap tipinden anahtar-değer çiftlerini kullanmaktadır. Böylece aslında tip güvenliğinide sağlamış oluyoruz. Son olarak eklediğimiz NumaratorAl isimli metodumuz Dictionary üzerinde Key değerleri için bir IEnumerator nesne örneğini geriye döndürüyor. Bunu çalışma zamanında koleksiyonumuz içindeki anahtarlar üzerinden öteleme yaparak Kitap nesnelerini elde etmek için kullanabiliriz. Kısacası bir listemele işlemi için kullanabiliriz. Şimdi koleksiyonumuzu kullanacağımız bir örnek uygulama yazalım.

```csharp
using System;
using System.Collections;

namespace StrongCollections
{
    class Uygulama
    {
        static KitapKoleksiyon kitapCol;
    
        static void Listele()
        { 
            IEnumerator numarator=kitapCol.NumaratorAl();
            while(numarator.MoveNext())
            {
                Console.WriteLine(kitapCol[Convert.ToInt32(numarator.Current)].ToString());
            }
        }
    
        static void KoleksiyonOlustur()
        {
            kitapCol.Ekle(1000,new Kitap(1000,"Her Yönüyle C#",1));
            kitapCol.Ekle(1001,new Kitap(1001,"Thinking in C#",1));
            kitapCol.Ekle(1002,new Kitap(1002,"Truva",1));
            kitapCol.Ekle(1003,new Kitap(1003,"Java in a Nuthshell",1));
        }

        static void Main(string[] args)
        {
            // KitapKoleksiyon nesne örneğimizi oluşturuyoruz.
            kitapCol=new KitapKoleksiyon();
            // Koleksiyonumuza bir kaç örnek Kitap elemanını ekliyoruz.( key-value çifti olarak)
            KoleksiyonOlustur();
            // Koleksiyonumuzdaki elemanları listeliyoruz.
            Listele();
            Console.WriteLine("-------------");
            // Koleksiyonda belirtilen isbn değerine sahip Kitap nesnesi olup olmadığına bakıyoruz. (Sonuçlar bool tipinden)
            Console.WriteLine("ISBN: 1000 var mı? {0}",kitapCol.Varmi(1000));
            Console.WriteLine("ISBN: 9999 var mı? {0}",kitapCol.Varmi(9999));
            Console.WriteLine("-------------");
            // Koleksiyonumuzdan key değeri 1001 olan key-value çiftini çıkartıyoruz.
            kitapCol.Cikart(1001);
            Console.WriteLine("ISBN: 1001 çıktı"); 
            // Koleksiyonumuzdaki elemanların son halini listeliyoruz. (Artık 1001 numaraları eleman yok)
            Listele();
            Console.WriteLine("-------------");
            // Koleksiyonumuzda 1003 isbn değerine sahip key-value çiftini buluyoruz.
            Kitap bulunanKitap=kitapCol.Bul(1003);
            // Bulunan Kitap nesnesinin içeriğini override ettiğimiz ToString metodu ile ekrana yazdırıyoruz.
            Console.WriteLine(bulunanKitap.ToString());
        }
    }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk131_4.gif](/assets/images/2005/mk131_4.gif)

Görüldüğü gibi KitapKoleksiyon isimli koleksiyonumuza eleman eklemek istediğimizde bizden bir key-value çifti beklenmektedir. Öyleki key integer tipinde, value ise Kitap tipinde olmak zorundadır. Bunu tasarım zamanında koleksiyonumuza eleman eklerken kolayca görebiliriz.

![mk131_5.gif](/assets/images/2005/mk131_5.gif)

İşte bu bizim için tip güvenliğini sağlamaktadır. Çünkü KitapKoleksiyon tipinden nesne örneğimize int-Kitap tipi dışında bir anahtar-değer çifti ekleyemiz. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.
