---
layout: post
title: "Tip Güvenli (Type Safety ) Koleksiyonlar Oluşturmak - 1"
date: 2005-07-23 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - generics
---
Tip güvenliğini sağlamak her zaman için önemli unsurlardan birisidir. Koleksiyon tabanlı nesneleri kullanırken çoğu zaman istediğimiz tip güvenliğini sağlayamayabiliriz. Buradaki en büyük etken, koleksiyon tabanlı nesnelerin object tipinden referanslar taşıyor olmasıdır. Bazen kendi belirlediğimiz tip dışında, başka her hangi bir tip barındırmasına izin vermek istemediğimiz yapıda koleksiyon nesnelerine ihtiyacımız olur. Böyle bir koleksiyon nesnesinin en büyük avantajı az önce bahsettiğimiz tip güvenliğini sağlamasıdır.

C# 2.0 versiyonunda koleksiyon nesnelerine ilişkin olarak tip güvenliği generic yapıları ile kazandırılmıştır. Peki 1.1 versiyonunda bu işleri nasıl gerçekleştirebiliriz? İki alternatif yolumuz vardır. Bunlardan birisi var olan bir koleksiyon nesnesini türetmektir. Diğer yol ise CollectionBase veya DictionaryBase tipinden türetme yaparak bir koleksiyon nesnesi tanımlamaktır. Biz bu makalemizde CollectionBase sınıfı yardımıyla tip güvenli koleksiyon sınıflarını nasıl oluşturabileceğimizi inceleyeceğiz. Bu sınıflar aynı zamanda strongly-typed collections (kuvvetle türlendirimiş koleksiyonlar) olarakta adlandırılmaktadır.

İlk olarak CollectionBase sınıfını kısacada olsa tanımakta fayda var. CollectionBase sınıfı abstract bir sınıftır. Bu kendisine ait bir nesne örneğinin oluşturulamayacağı anlamına gelmektedir. Ancak CollectionBase sınıfı IList, ICollection ve IEnumerable arayüzlerini implemente etmektedir. Buda tipik olarak bir koleksiyon nesnesi için gerekli üyeleri sağladığını gösterir. Aşağıdaki şekilde görülen YeniKoleksiyon strongly-typed koleksiyon sınıfımızdır. YeniKoleksiyın, CollectionBase sınıfından türetilmiştir. CollectionBase sınıfımız ise bir koleksiyonun taşıması gereken özellikleri sunan üyelerin bildirimini içeren üç temel arayüzden türemektedir.

![mk130_1.gif](/assets/images/2005/mk130_1.gif)

CollectionBase sınıfı kendi içerisinde IList tipinden bir tip döndüren protected yapıda List isimli bir özelliğe de sahiptir; ki bu özellik sayesinde CollectionBase sınıfından türeteceğimiz sınıflar içerisinden IList tipindeki koleksiyona erişebilir ve doğal olarak ekleme, çıkarma gibi temel koleksiyon işlevselliklerini sağlayabiliriz. List özelliğinin yanı sıra InnerList isimli özellik doğrudan bir ArrayList nesne referansına erişilmesini sağlar. Her iki özellikten de faydalanabilirsiniz. Biz bu makalemizde List özelliği yardımıyla koleksiyon aktivitelerini sağlayacağız. Şimdi gelin kendi strongly-typed koleksiyon sınıfımızı yazalım. Örneğimizde bir Dvd'ye ait bilgileri barındıran nesneler dizisini tutacak şekilde bir koleksiyon tasarlayacağız. İlk olarak bu koleksiyon içerisinde tutmak istediğinmiz Dvd nesnelerini temsil edecek sınıfımızı geliştirelim. Dvd sınıfının prototipi aşağıdaki şekilde olduğu gibidir.

![mk130_8.gif](/assets/images/2005/mk130_8.gif)

Dvd sınıfımız

```csharp
using System;

namespace StrongCollections
{
    public class Dvd
    {
        private string m_Baslik;
        private int m_Fiyat;
        private DateTime m_YapimYili;

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
        public DateTime YapimYili
        {
            get{return m_YapimYili;}
            set{m_YapimYili=value;}
        }
        public Dvd(string baslik,int fiyat,DateTime yapimYili)
        {
            Baslik=baslik;
            Fiyat=fiyat;
            YapimYili=yapimYili;
        }
        public Dvd()
        {
        }
        public override string ToString()
        {
            return this.Baslik+" "+this.Fiyat+" "+this.YapimYili.ToShortDateString();
        }
    }
}
```

DvdKoleksiyon sınıfımızı ise yapısı içerisinde bir koleksiyon için ihtiyaç duyulabilecek bir iki metod içerecek şekilde tasarlayacağız. Temel olarak bir Dvd nesnesini koleksiyona ekleme ve çıkarma gibi işlemleri üstlenen metodlara sahip olacak. Ancak önemli bir özellik olarak koleksiyon içerisindeki elemanlara indeksler üzerinden erişebilmemizi sağlayacak bir indeksleyicide yer alacak. Koleksiyon sınıfımızın yapısını aşağıdaki şekilden daha net olarak görebilirsiniz.

![mk130_9.gif](/assets/images/2005/mk130_9.gif)

DvdKoleksiyon sınıfı

```csharp
using System;
using System.Collections;

namespace StrongCollections
{
    public class DvdKoleksiyon:CollectionBase
    {
        public void Ekle(Dvd dvd)
        {
            this.List.Add(dvd);
        }
        public void Cikart(Dvd dvd)
        {
            this.List.Remove(dvd);
        }
        public Dvd this[int indeks]
        {
            get{return (Dvd)this.List[indeks];}
            set{this.List[indeks]=value;}
        }
        public void Cikart(int indeks)
        {
            this.List.RemoveAt(indeks);
        }
        public void Ekle(int indeks,Dvd dvd)
        {
            this.List.Insert(indeks,dvd);
        }
        public DvdKoleksiyon()
        {
        }
    }
}
```

Ekle isimli metodumuzun iki versiyonu vardır. Bunlardan birisi koleksiyonun sonuna bir Dvd nesnesini eklerken diğeri, parametre olarak belirtilen indekse ekleme işlemini gerçekleştirir. Cikart isimli metodumuzda iki versiyona sahiptir. Bunlardan birisi koleksiyondan parametre olarak aldığı Dvd nesnesini çıkartırken, ikincisi belirtilen indeks üzerindeki Dvd nesnesini koleksiyondan çıkartmaktadır. Indeksleyicimiz ise, DvdKoleksiyonu içerisindeki Dvd tipinden nesnelere indeks değerleri üzerinden erişmemizi sağlar.

Dikkat ederseniz tüm bu üyeler sadece ve sadece Dvd tipinden nesneler üzerinden iş yapmaktadır. Örneğin koleksiyona nesne ekleme ve koleksiyondan nesne çıkartmak için kullandığımız metodlar parametre olarak mutlaka bir Dvd nesne örneğini referans ederler. Benzer şekilde indeksleyicimizde sadece Dvd tipinden nesneler üzerinden çalışır. Şimdi yazdığımız koleksiyon sınıfımızı deneyeceğimiz bir uygulama geliştirelim.

```csharp
using System;
using System.Collections;

namespace StrongCollections
{
    class Uygulama
    {
        static DvdKoleksiyon dvdCol;

        static void Listele()
        { 
            foreach(Dvd dvd in dvdCol)
            {
                Console.WriteLine(dvd.ToString());
            }
            Console.WriteLine("------------------------");
        }

        static void KoleksiyonOlustur()
        {
            dvdCol.Ekle(new Dvd("Gladiator",10,new DateTime(2000,1,1)));
            dvdCol.Ekle(new Dvd("Star Wars 3",20,new DateTime(2005,1,4)));
            dvdCol.Ekle(new Dvd("Crow",15,new DateTime(1997,3,9)));
        } 

        static void Main(string[] args)
        {
            dvdCol=new DvdKoleksiyon();
            KoleksiyonOlustur();

            for(int i=0;i<dvdCol.Count;i++)
            {
                Console.WriteLine(dvdCol[i].ToString());
            }
            Console.WriteLine("--------------------");
            dvdCol.Cikart(1);

            Console.WriteLine("1 nolu eleman çıkartıldı...");
            Listele();

            // dvdCol.Ekle(123); // type - safety sağlanır.
        }
    }
}
```

Uygulamayı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk130_10.gif](/assets/images/2005/mk130_10.gif)

Aslında sanki normal bir koleksiyondan, örneğin bir ArrayList ile yaptığımız işlemlerden pek bir farkı yokmuş gibi görünüyor. Farkı anlayabilmek için yorum satırımızı koda katarak uygulamayı yeniden derleyelim.

![mk130_3.gif](/assets/images/2005/mk130_3.gif)

Burada problem Ekle metoduna sayısal bir değerin parametre olarak girilmeye çalışılmasıdır. DvdKoleksiyon sınıfımızın Ekle metodlarını kısaca bir hatırlarsak;

```csharp
public void Ekle(Dvd dvd)
{
     this.List.Add(dvd);
}
public void Ekle(int indeks,Dvd dvd)
{
     this.List.Insert(indeks,dvd);
}
```

parametre olarak sadece Dvd tipinden nesneleri aldıklarını görebiliriz. Bu nedenle kod derleme zamanında hata vererek geliştiricinin çalışma zamanında bir hataya düşmesini engellemiştir. İşte bu tip güvenliğini sağlar. Benzer şekilde koleksiyondan okuduğumuz bir Dvd nesnesini farklı bir nesne tipine de atayamayız. Örneğin aşağıdaki kod parçasını göz önüne alalım.

```csharp
Kitap ktp=new Kitap();
ktp=(Kitap)dvdCol[0];

double dbl=(double)dvdCol[0];
```

Bu durumda derleme zamanında aşağıdaki hata mesajlarını alırız.

![mk130_4.gif](/assets/images/2005/mk130_4.gif)

Burada var olan bir tipe ve bizim yazdığımız tipe atamalar yapılmaya çalışılmıştır. Ancak koleksiyonumuz sadece Dvd tipinden nesne örneklerini geriye döndüren bir indeksleyiciye sahiptir. Bunu indeksleyiciyi kullanırken de görebilirsiniz.

![mk130_5.gif](/assets/images/2005/mk130_5.gif)

Oysaki, bir ArrayList göz önüne alındığında geriye dönen değer her zaman object tipinden olacaktır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
CollectionBase tipinden türettiğimiz nesnelerde, koleksiyonda işlenecek olan nesne tipi ne ise onu kullanmalıyız. Bu tip güvenliğini (type - safety) sağlayabilmemizi olanaklı kılar.

CollectionBase'den türettiğimiz sınıfların sağladığı tip güvenliği daha iyi anlayabilmek için Dvd nesnelerini taşıyacak bir ArrayList koleksiyonunun kullanıldığı aşağıdaki örneği göz önüne almakta fayda var.

```csharp
ArrayList alDvd=new ArrayList();

alDvd.Add(new Dvd("Gıladyatör",10,new DateTime(2000,1,1)));
alDvd.Add(new Dvd("Sıtar vars 3",20,new DateTime(2005,1,4)));
alDvd.Add(new Dvd("kırouv",15,new DateTime(1997,3,9)));
alDvd.Add(12); 

foreach(Dvd dvd in alDvd)
{
    Console.WriteLine(dvd.ToString());
}
```

Yazılan kod son derece masumane görünmektedir. Üstelik derleme zamanında hiç bir hata mesajı vermez. Yani çalışır bir koddur. Ancak uygulamayı bu haliyle çalıştırdığımızda aşağıdaki hata mesajını alırız.

![mk130_6.gif](/assets/images/2005/mk130_6.gif)

Sorun foreach döngüsünde açıkça görülmektedir. foreach döngüsü sadece Dvd tipinden elemanlar üzerinde bir öteleme gerçekleştirmek isterken koleksiyonun sonuna eklenen sayısal değer bu durumu bozmaktadır. Eğer foreach döngüsünü terk edip aşağıdaki gibi bir for döngüsünü tercih ederseniz durum biraz daha ilginç bir hal alacaktır.

```csharp
for(int i=0;i<alDvd.Count;i++)
{
    Console.WriteLine(alDvd[i].ToString());
}
```

Kodda herhangibir derleme zamanı hatası alınmaz. Çalışma zamanında da bir hata alınmaz. Uygulama başarılı bir şekilde çalışır. Ancak bu kez de programın mantıksal bütünlüğü bozulmuştur.

![mk130_7.gif](/assets/images/2005/mk130_7.gif)

İşte bu tip bir durumla karşılaştığımızda geliştirici olarak tip güvenliğini ve uygulamanın mantıksal bütünlüğünü korumak amacıyla kendi koleksiyon sınıflarımızı kullanmayı tercih ederiz. Yazımızın başında hatırlarsanız kendi yazacağımız koleksiyon sınıflarını CollectionBase yoluyla veya DictionaryBase yoluyla geliştirebildiğimizi söylemiştik. Bu makalemizde CollectionBase sınıfı ile bu işi nasıl yapacağımızı gördük. Bir sonraki makalemizde ise DictionaryBase sınıfını inceleyemeye çalışacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.