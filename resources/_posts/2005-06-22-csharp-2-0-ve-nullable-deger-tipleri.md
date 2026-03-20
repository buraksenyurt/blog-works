---
layout: post
title: "C# 2.0 ve Nullable Değer Tipleri"
date: 2005-06-22 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - generics
---
C# programlama dilinde bildiğiniz gibi veri türlerini Referans Türleri (Reference Types) ve Değer Türleri (Value Types) olmak üzere iki kısma ayırıyoruz. Bu iki tür arasında bellek üzerinde fiziki tutuluş şekillerinden tutunda birbirleri arasındaki atamalara kadar pek çok farklılık vardır. Bu farklılıklardan birisi de, referans türlerinin null değerleri alabilmelerine karşın, değer türlerinin aynı özelliğe sahip olmayışlarıdır.

Bu durum özellikle veritabanı tablolarında null değer alabilen alanların, dil içerisindeki tip karşılığı değer türüne denk düştüğünde bazı zorluklar çıkartabilmektedir. İşte bu makalemizde, C# 2.0 diliyle gelen yeni özelliklerden birisi olan Nullable Value Types (Null değer alabilen değer türleri) ni incelemeye çalışacağız. Başlangıç olarak C# 1.1 versiyonundaki durumu analiz ederek işe başlayalım. Aşağıdaki kod parçasında bir referans türüne ve bir de değer türüne null değerler atanmaya çalışılmaktadır.

```csharp
using System;

namespace ConsoleApplication2
{
    class Class1
    {
        static void Main(string[] args)
        {
            string refTuru=null;
            int degerTuru=null;
        }
    }
}
```

Bu uygulamayı derlemeye çalıştığımızda Cannot convert null to 'int'because it is a value type hatasını alırız. Aynı durumu kendi tanımladığımız referans ve değer türleri içinde gerçekleştirebiliriz. Aşağıdaki kod parçasında bu durum örneklenmiştir.

```csharp
using System;

namespace ConsoleApplication2
{
    class Kitap
    {
    }
    struct Dvd
     {
     }
    class Class1
    {
        static void Main(string[] args)
        {
            Kitap kitap=null;
            Dvd dvd=null;
        }
    }
}
```

Bu kod parçasındada aynı hatayı alırız. Çünkü struct'lar değer türüdür ve bu sebeple null değerler alamazlar. Oysaki aynı istisnai durum class gibi kendi tanımlamış olduğumuz referans türleri için geçerli değildir. Peki değer türlerinin null değer içerme ihtiyacı ne zaman doğabilir? Bir veritabanı uygulamasını göz önüne alalım. Bu tabloda int, double gibi değer türlerine karşılık gelecek alanların var olduğunu düşünelim. Veri girişi sırasında bu int ve double değişkenleri null olarak tabloya aktarmak isteyebiliriz.

Ya da tablodan veri çekerken, değer türü karşılığı alanların null değer içerip içermediğini anlamak isteyebiliriz. İşte bu gibi durumlarda değer türlerinin null veriler içerebilecek yapıda olması, kodumuzun ölçeklenebilirliğini arttıracak bir yetkinlik olarak düşünülebilir. Veritabanları için geçerli olan bu senaryoyu göz önüne almadan önce C# 2.0 için değer türlerinin nasıl null veriler taşıyabileceğini incelemeye çalışalım. Değer türlerinin C# 2.0 için iki versiyonu vardır. Nullable değer türleri ve Non-Nullable değer türleri. Bir değer türünün null değerler içerecek tipte olacağını belirtmek için? tip belirleyicisi kullanılır.

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace TestOfNullableValues
{
    class Program
    {
        static void Main(string[] args)
        {
            int? maas;
            double? pi;
            maas = null;
            pi = null;
        }
    }
}
```

Yukarıdaki kod parçası sorunsuz olarak derlenecek ve çalışacaktır.? ile tanımlanan değer türleri null veriler taşıyabilen değer tipindendir. Aynı durum kendi tanımladığımız bir struct içinde geçerlidir. Aşağıdaki kod parçasında Personel isimli struct'a null değer ataması yapılmıştır.

```csharp
struct Personel
{
}

class Program
{
    static void Main(string[] args)
    {
        Personel? person = null;
    }
}
```

Elbette referans türlerinde bu tarz bir kullanım geçerli olmayacaktır. Örneğin aşağıdaki kod parçasında kendi tanımlamış olduğumuz bir sınıf nesnesine ve string türünden bir değişkene? belirleyicisi vasıtasıyla null değerler atanmaya çalışılmıştır.

![mk125_4.GIF](/assets/images/2005/mk125_4.GIF)

Oysaki referans türleri zaten null değerler alabilmektedir.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Referans türlerine? tip belirleyicisi uygulayarak null değer ataması yapılamaz.

? tip belirleyicisi aslında tanımlanan değer türünün null veri taşıyabilecek başka bir versiyonunu kullanılacağını belirtmektedir. Bu yeni değer türü versiyonları ise null verileri taşıyabilecek bir yapıda tasarlanmışlardır.? belirleyicisinin uygulandığı bir değer türünün null değerler içerip içermediğine HasValue özelliği ilede bakılabilmektedir. Bu özellik, ilgili değer türü null veri içerdiği sürece false döndürecektir. Örneğin,

```csharp
int? Yas;
Yas = null; // Yas değer türüne null veri atanıyor.
if (Yas.HasValue) // false dönecektir
{
    Console.WriteLine("Yas null değil...");
}
else
{
    Console.WriteLine("Yas null..."); // Bu satır işletilir.
}
```

C# 2.0 diline? tip belirleyicisinden yola çıkılaraktan yeni bir operatör eklenmiştir.?? operatörü. Bu operatör kısaca bir değer türünün içeriğinin null olup olmamasına göre koşullu olarak atama yapmaktadır. Eğer operatörde kullanılan null veri taşıyabilir değer türünün o anki değeri null ise, koşul olarak belirtilen değer ilgili değişkene atanır. Aksi takdirde, değer türünün o anki verisi, ilgili değişkene atanır. Aşağıdaki örnek kod parçasında bu operatörün kullanım şekli gösterilmeye çalışılmıştır.

```csharp
int? Yas; // null değer alabilecek bir değer türü tanımlanıyor.

Yas = null; // null değer ataması
int yasi = Yas ?? 0; // Eğer Yas null ise yasi alanına 0 atanır.
Console.WriteLine(yasi); // 0 yazar

Yas = 12;
yasi = Yas ?? 0; // Eğer Yas null değil ise yasi alanına Yas' ın o anki değeri atanır.
Console.WriteLine(yasi); // 12 yazar
```

İlk kullanımda, Yas değer türünün sahip olduğu veri null olarak belirlenmiştir.?? operatörü bu durumda yasi alanına 0 değerini atayacaktır. İkinci kullanımda ise Yas değer türünün verisi 12 olarak belirlenmiştir. Buna görede?? operatörü yasi değişkenine 12 değerini (yani Yas nullable değer türünün o anki verisini) atayacaktır. Burada dikkat ederseniz Yas null değer alabilen bir int tipidir. Bununla birlikte?? operatörü ile yapılan veri ataması null değerler içeremiyen normal bir int tipine doğru yapılmaktadır. Burada söz konusu olan atama işlemini biraz irdelemekte fayda vardır. Aşağıdaki kod parçasını ele alalım.

```csharp
int? pi = 3;
int m_Pi;
m_Pi = pi;
```

Bu örnek derlenmeyecektir. Bunun sebebi ise null değer alabilen bir değer türünü, normal bir değer türüne bilinçsiz olarak atamaya çalışmamızdır. Dolayısıyla,

![dikkat.gif](/assets/images/2005/dikkat.gif)
Nullable Değer türleri bilinçsiz olarak normal değer türlerine dönüştürülemez.

Ancak aşağıdaki kod parçasında görülen tür dönüşüm işlemi geçerlidir.

```csharp
int? pi = 3;
int m_Pi;
m_Pi = (int)pi;
```

Elbette burada dikkat edilmesi gereken bir durum daha vardır. Eğer o anki değeri null olan bir değer türünü normal bir değer türüne bilinçli olarak atamaya çalışırsak çalışma zamanında InvalidOperationException hatası alırız.

![mk125_5.GIF](/assets/images/2005/mk125_5.GIF)

Dolayısıyla bu tip atamalarda?? operatörünü tercih etmek çok daha akılcı bir yaklaşım olacaktır. Nullable değer türlerine, normal değer türlerinin atanmasında ise bilinçsiz tür dönüşümü de geçerlidir.

```csharp
double e = 2.7;
double? E;
E = (double?)e; // Bilinçli tür dönüşümü
E = e; // Bilinçsiz tür dönüşümü
```

Yukarıdaki kod parçasında E null değerler taşıyabilen bir değer türüdür. e ise normal değer türüdür.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Normal değer türleri, nullable değer türlerine hem bilinçli hem de bilinçsiz olarak dönüştürülebilir.

Makalemizin sonunda bir veritabanı uygulamasında null değerler alabilen değer türlerinin nasıl kullanılabildiğini incelemeye çalışacağız. İlk olarak C# 1.1 versiyonunda aşağıdaki yapıda bir uygulama geliştirelim. Bu örneğimizde, Sporculara ait bir takım temel bilgileri tutan bir tabloyu kullanacağız. Tablomuzda tanımlı olan Sporcu, Boy ve Yaş alanları null değerler içerebilecek şekilde yapılandırılmıştır.

![mk125_2.gif](/assets/images/2005/mk125_2.gif)

Örnek olarakta aşağıdaki iki satır verinin var olduğunu düşünelim. Dikkat ederseniz ikinci satırda değer türü olarak ele alacağımız alanlara değerler atanmıştır.

![mk125_3.gif](/assets/images/2005/mk125_3.gif)

Uygulama kodlarımız ise başlangıç olarak aşağıdaki gibidir. Basit olarak Sporcuları temsil edecek bir sınıf ve tablo üzerindeki temel veritabanı işlemlerini gerçekleştirecek bir katman sınıfı yer almaktadır. Katman sınıfımız şu an için sadece ve sadece Sporcu tablosundaki verileri okuyup, her bir satır için birer Sporcu nesnesi yaratan ve onun override edilmiş ToString metodu ile bilgilerini ekrana yazdıran bir metoda sahiptir.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace TestNullableValues
{
    class Sporcu
    {
        private int m_id;
        private string m_sporcu;
        private double m_boy;
        private int m_kilo;
        public Sporcu(int id,string sporcu,double boy,int kilo)
        {
            m_id=id;
            m_sporcu=sporcu;
            m_boy=boy;
            m_kilo=kilo;
        }
        public override string ToString()
        {
            return m_id.ToString()+" "+m_sporcu+" "+m_boy.ToString()+" "+m_kilo.ToString();
        }
    }

    class SporYonetim
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader dr;
    
        public SporYonetim()
        {
            con=new SqlConnection("data source=localhost;database=MyBase;user id=sa;password=");
        }

        public void SporcuListesi()
        {
            cmd=new SqlCommand("Select ID,Sporcu,Boy,Kilo From Sporcular",con);
            con.Open();
            dr=cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while(dr.Read())
            {
                int id=(int)dr["ID"];
                string sporcu=dr["Sporcu"].ToString();
                double boy=(double)dr["Boy"];
                    int kilo=(int)dr["Kilo"];
                Sporcu sprc=new Sporcu(id,sporcu,boy,kilo);
                Console.WriteLine(sprc.ToString());
            }
        }
    }

    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
            SporYonetim ynt=new SporYonetim();
            ynt.SporcuListesi();
        }
    }
}
```

Bizim için önemli olan nokta Boy ve Yas alanlarının değererinin double ve int alanlara atıldığı yerdir. Uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki hata mesajını alırız.

![mk125_1.gif](/assets/images/2005/mk125_1.gif)

Sebep gayet net ve açıktır. Değer türleri null veri içeremeyeceği için cast işlemleri çalışma zamanında InvalidCastException tipinden bir istisna nesnesi fırlatılmasına neden olmuştur. Bu sorunu aşmak için kullanabileceğimiz tekniklerden bir tanesi aşağıdaki gibidir. dr ile okunan tablo alanları object tipinden geriye döndüğünden ve object tipide referans türü olduğundan null değerler içerebilir. Burada alanın o anki verisinin null olup olmamasına göre uygun atamalar gerçekleştirilmektedir.

```csharp
double boy;
int kilo;
if(dr["Boy"]==System.DBNull.Value)
    boy=0;
else
    boy=(double)dr["Boy"];
if(dr["Kilo"]==System.DBNull.Value)
    kilo=0;
else
    kilo=(int)dr["Kilo"];
Sporcu sprc=new Sporcu(id,sporcu,boy,kilo);
```

Aynı örneği aşağıdaki haliyle C# 2.0 versiyonunda daha farklı bir yaklaşım ile yazabiliriz. Bu kez, Sporcu sınıfımızın yapıcı metodunda yer alan boy ve kilo parametleri ile private field olarak tanımladığımız m_Boy ve m_Kilo alanlarını null değer içerebilecek şekilde tanımlayarak işe başlayacağız. Yine SqlDataReader ile okuduğumuz alanların null değer içerip içermediğini kontrol edeceğiz. Ancak bu kez null değer içerseler dahi onları taşayabilecek değer türlerimiz elimizde olacak. Böylece tablomuzda null değere sahip olan alanlarımızı ekrana yazdırabileceğiz.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace UsingNullableValues
{
    class Sporcu
    {
        //Diğer kod satırları

        // Sınıf içi bu iki alanı null değerler taşıyabilecel tipten tanımladık.
        private double? m_boy;
          private int? m_kilo;

        // Diğer kod satırları
    }

    class SporYonetim
    {
        // Diğer kod satırları

        public void SporcuListesi()
        {    
            // Diğer kod satırları
            while (dr.Read())
            {
                // Diğer kod satırları
                
                double? boy=null;
                    int? kilo=null;
                    if(dr["Boy"]!=System.DBNull.Value)
                         boy=(double)dr["Boy"];
                    if (dr["Kilo"] != System.DBNull.Value)
                         kilo = (int)dr["Kilo"];

                // nullable değişkenleri Sporcu sınıfının yapıcı metoduna parametre olarak gönderiyoruz.
                Sporcu sprc = new Sporcu(id, sporcu, boy, kilo);
                Console.WriteLine(sprc.ToString());
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            SporYonetim ynt = new SporYonetim();
            ynt.SporcuListesi();
        }
    }
}
```

![mk125_6.gif](/assets/images/2005/mk125_6.gif)

Uygulamamızı bu haliyele çalıştırdığımızda tablodaki tüm satırların ekrana yazdırıldığını görürüz. C# 1.1 de geliştirdiğimiz uygulama ile bu versiyon arasındaki en önemli fark, nullable değer türlerinin kullanılarak null verilerin uygulama içerisinde kullanılabilmesidir. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.