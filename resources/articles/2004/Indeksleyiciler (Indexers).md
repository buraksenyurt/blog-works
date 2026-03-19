---
title: "Indeksleyiciler (Indexers)"
pubDate: 2004-01-27 12:00:00
categories:
  - C#
tags:
  - C#
---

# Indeksleyiciler (Indexers)
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde kısaca indeksleyicilerin C# programlama dilindeki kullanımını incelemeye çalışacağız. Bir indeksleyici, bir sınıfı dizi şeklinde kullanabilmek ve bu sınıftan türetilen nesneleri dizinleyebilmek amacıyla kullanılır. Başka bir deyişle bir indeksleyici, nesnelere dizi gibi davranılabilmesini sağlar.

Indeksleyiciler tanımlanışları itibariyle, özelliklere (properties) çok benzerler. Ancak aralarında temel farklılıklarda vardır. Herşeyden önce bu benzerlik, indeksleyicilerin tanımlanmasında göze çarpar. Bir indeksleyiciyi teorik olarak aşağıdaki söz dizimi ile tanımlanır.

```csharp
public int this[int indis]
{
     get
     {
         // Kodlar
     }
     set
     {
         // Kodlar
     }
}
```

Görüldüğü gibi bir indeksleyici tanımlanması, özellik tanımlanması ile neredeyse aynıdır. Ancak bir indeksleyici tanımlarken uymamız gereken bir takım kurallarda vardır. Bu kurallar aşağıdaki tabloda belirtilmiştir.

Indeksleyici Kuralları

Bir indeksleyici mutlaka bir geri dönüş tipine sahip olmalıdır. Yani bir indeksleyiciyi void olarak tanımlayamayız.

Bir indeksleyiciyi static olarakta tanımlayamayız.

Bir indeksleyici en az bir parametre almalıdır. Bununla birlikte, bir indeksleyici birden fazla ve çeşitte parametrede alabilmektedir.

Indeksleyicileri aşırı yükleyebiliriz (Overload). Ancak bir indeksleyiciyi aşırı yüklediğimizde, bu indeksleyicileri birbirlerinden ayırırken ele aldığımız imzalar sadece parametreler ile belirlenir. Indeksleyicinin geri dönüş değeri bu imzada ele alınmaz.

Indeksleyici parametrelerine, normal değişkenlermiş gibi davranamayız. Bu nedenle bu parametreleri ref ve out anahtar sözcükleri ile yönlendiremeyiz.

Bir indeksleyici her zaman this anahtar sözcüğü ile tanımlamalıyız. Nitekim this anahtar sözcüğü, indeksleyicinin kullanıldığı sınıf nesnelerini temsil etmektedir. Böylece sınıfın kendisi bir dizi olarak kullanılabilir.

Tablo 1. Indeksleyici tanımlama kuralları.

Indeksleyicileri siz değerli okurlarıma anlatmanın en iyi yolunun basit bir örnek geliştirmek olduğunu düşünüyorum. Dilerseniz vakit kaybetmeden örneğimize geçelim.

Öncelikle bir sınıf tanımlayacağız. Bu sınıfımız, Sql veri tabanında oluşturduğumu Personel isimli tablonun satırlarını temsil edebilecek bir yapıda olucak. Tablomuz örnek olarak Personelimize ilişkin ID,Ad,Soyad bilgilerini tutan basit bir veri tablosu. Her bir alan, bahsetmiş olduğumuz sınıf içinde birer özellik olarak tanımlanacak. Diğer yandan başka bir sınıfımızda daha var olucak. Bu sınıfımız ise, bir indeksleyiciye sahip olucak. Bu indeksleyiciyi kullanarak, veri satırlarını temsil eden sınıf örneklerini, bu sınıfı içerisinde tanımlayacağımız object türünden bir dizide tutacağız. Sonuç olarak tablo satırlarına, sınıf dizi elemanlarıymış gibi erişebileceğiz. Burada indeksleyiciler sayesinde sınıfımıza sanki bir diziymiş gibi davrancak ve içerdiği veri satırlarına indeks değerleri ile erişebileceğiz. Örneğimizi geliştirdiğimizde konuyu daha iyi kavrayacağınıza inanıyorum. Şimdi dilerseniz bir console uygulaması açalım ve aşağıdaki kodları yazalım.

```csharp
using System;
using System.Collections;
using

System.Data.SqlClient;

namespace Indexers1
{
      /* Tablomuzda yer alan satırları temsil eden sınıfımızı ve bu tablodaki her bir alanı temsil edicek özelliklerimizi tanımlıyoruz. */
 
     public class Personel
     {
          private int perid;
          private string perad;
          private string persoyad;

          public int PerID
          {
               get
               {
                    return perid;
               }
               set
               {
                    perid=value;
               }
          }
          public string PerAd
          {
               get
               {
                    return perad;
               }
               set
               {
                     perad=value;
                }
           }
           public string PerSoyad
          {
               get
               {
                    return persoyad;
               }
               set
               {
                     persoyad=value;
                }
           }
     }

     /* PersonelListesi sınıfımız, Personel tipinden nesneleri tutucak Object türünden bir tanımlar. Bu dizimizde, Personel sınıfı türünden nesneleri tutacağız. Bu nedenle Object türünden tanımladık. Ayrıca sınıfımız bir indeksleyiciye sahip. Bu indeksleyici, object türünden dizimizdeki elemanlara erişirken, bu sınıftan türetilen nesneyi bir diziymiş gibi kullanabilmemize imkan sağlayacak. Yani uygulamamızda, bu sınıftan bir nesne türetip nesneadi[indis] gibi bir satır yazdığımızda buradaki indis değeri, indeksleyicinin tanımlandığı bloğa aktarılıcak ve get veya set blokları için kullanılacak. Bu bloklar aldıkları bu indis parametresinin değerini Object türünden dizimizde kullanarak, karşılık gelen dizi elemanı üzerinde işlem yapılmasına imkan sağlamaktadırlar.*/ 

     public class PersonelListesi
     {
          private Object[] liste=new Object[10];

          /* Indeksleyicimizi tanımlıyoruz.*/

          public Personel this[int indis]
          {
               get
               {
                    /* liste isimli Object türünden dizimizin indis indeksli değerini döndürür. Bunu döndürürken Personel sınıfı tipinden döndürür. Böylece iligili elemandan Personel sınıfındaki özelliklere, dolayısıyla tablo alanlarındaki değere ulaşmış oluruz. */
                    return (Personel)liste[indis];
                }
               set
               {
                    /* liste isimli Object türünden dizimizdeki indis indeksli elemana value değerini aktarır. */
                    liste[indis]=(Personel)value;
                }
           }
     }
     class Class1
     {
          static void Main(string[] args)
          {
                SqlConnection con=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=sspi");
 
               SqlCommand cmd=new SqlCommand("Select * From Personel",con);
                SqlDataReader dr;
                con.Open();
                dr=cmd.ExecuteReader();
                PersonelListesi pliste=new PersonelListesi(); /* Indeksleyicimizi kullandığımız sınıftan bir nesne türetiyoruz. */
 
               int PersonelSayisi=0;
               int i=0;
               try
               {
                    /* SqlDataReader nesnesi ile, satırlarımızı okurken bu satıra ait alanların değerlerini tutacak Personel tipinden bir sınıf nesnesi oluşturulur ve ilgili alan değerleri bu nesnenin ilgili özelliklerine atanır. */
                    while(dr.Read())
                    {
                          Personel p=new Personel();
                          p.PerID=(int)dr[0];
                          p.PerAd=dr[1].ToString();
                          p.PerSoyad=dr[2].ToString();
                          /* Şimdi PersonelListesi sınıfı türünden nesnemize güncel satırı temsil eden Personel nesnesini atıyoruz. Bunu yaparken bu sınıf örneğine sanki bir diziymiş gibi davrandığımıza dikkat edelim. İşte bunu sağlayan indeksleyicimizdir. Burada PersonelListesi içindeki, object türünden liste isimli dizideki i indeksli elemana p nesnesi aktarılıyor. */
                         pliste[i]=p;
                         i+=1;
                         PersonelSayisi+=1;
                    }
                    /* Bu döngüde, pliste isimli PersonelListesi türünden nesneye, i indeksini kullanarak  içerdiği object türünden liste isimli dizi elemanlarına, tanımladığımız indeksleyici sayesinde bir dizi elemanına erişir gibi erişiyoruz. */

                    for(int j=0;j<PersonelSayisi;++j)
                    {
                         /* pliste'nin türetildiği PersonelListesi sınıfı indeksleyicisinin kullandığı dizi elemanlarnı Personel türüne dönüştürerek elde ettiğimiz için, bu nesnenin özelliklerinede yani veri satırı alanlarınada kolayca erişebiliyoruz. */

                         Console.WriteLine(pliste[j].PerAd.ToString()+" "+pliste[j].PerSoyad.ToString()+" "+pliste[j].PerID.ToString());
                    }
               }
               catch(Exception hata)
               {
                    Console.WriteLine(hata.Message.ToString());
               }
               finally
               {
                    dr.Close();
                    con.Close();
               }
          }
     }
}
```

Kodlar size karmaşık gelebilir o nedenle aşağıdaki şekil indeksleyicilerin kullanımını daha iyi anlayabilmemizi sağlayacaktır.

![mk47_2.gif](images/mk47_2.gif)

Şekil 1. Indeksleyicilerin Kullanımı

Uygulamamızda pliste[i]=p; satırı ile, Personel sınıfı türünden nesnemiz, pliste isimli PersonelListesi sınıfının i indeksli elemanı olarak belirleniyor. Bu satır derleyici tarafından işlendiğinde, PersonelListesi sınıfındaki indeksleyicimizin set bloğu devreye girer. Set bloğu pliste[i] deki i değerini indis parametresi olarak alır ve Object türünden liste isimli dizide indis indeksine sahip elemana nesnemizi aktarır.

Diğer yandan, pliste[j] ile PersonelListesi sınıfına erişildiğinde, indeksleyicinin get bloğu devreye girer. Get bloğunda, indeksleyicinin parametre olarak aldığı indis değeri j değeridir. Bu durumda, liste[indis] ile, j indeksli liste dizisi elemanı çağırılır. Sonra bu eleman Personel tipine dönüştürülür. Bu sayedede pliste[j].PerAd.ToString () gibi bir ifade ile, bu nesnenin temsil ettiği özellik değerinede ulaşılabilir.

Uygulamamızı çalıştıralım ve deneyelim. Aşağıdaki ekran görüntüsünü elde ederiz.

![mk47_1.gif](images/mk47_1.gif)

Şekil 2. Uygulamanın Çalışmasının Sonucu.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
