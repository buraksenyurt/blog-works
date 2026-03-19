---
layout: post
title: "DataTable.Compute Metodu"
date: 2004-03-29 12:00:00 +0300
description: "Çoğu zaman, uygulamalarımızda T-Sql' in Aggregate fonksiyonlarını kullanarak, belirli sütunlara ait veriler üzerinden, toplam değer, en büyük değer, en küçük değer, ortalama değer vb. gibi sonuçlara ulaşmaya çalışırız. Bu amaçla T-Sql' in Avg, Sum, Count gibi Aggregate fonksiyonlarından yararlanırız. İşte bu makalemizde, bu fonksiyonları, DataTable sınıfının Compute metodu yardımıyla nasıl kullanabileceğimizi incelemeye çalışacağız."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - t-sql
  - datatable
---
# DataTable.Compute Metodu
Değerli Okurlarım, Merhabalar.

Çoğu zaman, uygulamalarımızda T-Sql'in Aggregate fonksiyonlarını kullanarak, belirli sütunlara ait veriler üzerinden, toplam değer, en büyük değer, en küçük değer, ortalama değer vb. gibi sonuçlara ulaşmaya çalışırız. Bu amaçla T-Sql'in Avg, Sum, Count gibi Aggregate fonksiyonlarından yararlanırız. İşte bu makalemizde, bu fonksiyonları, DataTable sınıfının Compute metodu yardımıyla nasıl kullanabileceğimizi incelemeye çalışacağız.

Öncelikle, T-Sql'de yer alan Aggregate fonksiyonlarından kısaca bahsetmekta yarar olduğunu düşünüyorum. Bu fonksiyonların en önemlileri ve kullanışlıları aşağıdaki tabloda yer almaktadır.

Fonksiyon
Prototipi
Açıklama
Dönüş Tipi
Örnek

AVG (Ortalama)
AVG ([ ALL | DISTINCT ] ifade)
Sql sorgusunda belirtilen kritere uyan alanların ortalamasını alır.
int, decimal, money, float
SELECT AVG (Prim)
FROM Primler
WHERE PerID = 1002124

SUM (Toplam)
SUM ([ ALL | DISTINCT ] ifade)
Sql sorgusunda belirtilen kritere uyan alanların toplam değerini alır.
int, decimal, money, float
SELECT SUM (Prim)
FROM Primler

COUNT (Toplam Sayı)
COUNT ({ [ ALL | DISTINCT ] ifade ] | })
Satır sayısını verir.
int
SELECT COUNT (*)
FROM Primler

MAX (En büyük değer)
MAX ([ ALL | DISTINCT ] ifade)
Belirtilen alana ait sütundaki en büyük değeri verir.
ifade olarak belirtilen tip ile aynıdır.
SELECT MAX (Prim)
FROM Primler

MIN (En küçük değer)
MIN ([ ALL | DISTINCT ] ifade)
Belirtilen alana ait sütunlardaki en küçük değeri verir.
ifade olarak belirtilen tip ile aynıdır.
SELECT MIN (Prim) FROM Primler

COUNT_BIG (Toplam Sayı)
COUNT Fonksiyonu gibi satır sayısını verir. Tek fark dönüş değeridir.
bigint

SELECT COUNT_BIG (*)
FROM Primler

STDEV (Standart Sapma)
STDEV (expression)
Belirtilen kritere uyan alanlar için Standart Sapma değerini hesaplar.
float
SELECT STDEV (Alan)
FROM Tablo

Tablo 1. Aggregate Fonksiyonları

Bu tip fonksiyonları.net uygulamalarımızda kullanmak için, akla gelen ilk yol Command nesnelerinden yararlanmaktır. Örneğin, Sql sunucumuzda yer alan, Northwind veritabanındaki, Products tablosunu ele alalım. Bu tabloda, Aggregate fonksiyonlarını test edebilmemiz için kullanabileceğimiz alanlar mevcuttur.(UnitPrice, UnitsInStock vb.) Şimdi ilk düşündüğümüz şekilde, yani bir Command nesnesini kullanarak, belli bir gruba ait UnitPrice ve UnitsInStock alanlarının değerleri üzerinde, Aggregate fonksiyonları ile denemeler yapalım. Örneğin basit olması amacıyla bir Console uygulaması geliştirebiliriz.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace Compute
{
    class Class1
    {
        static void Main(string[] args)
        {
            /* Yerel sql sunucumuzdaki Northwind veritabanına bir bağlantı hattı oluşturuyoruz.*/
            SqlConnection con=new SqlConnection("Data Source=localhost;initial catalog=Northwind;Integrated Security=SSPI");
            /* Bu SqlCommand nesnesinin içerdiği sql cümleciği ile, SupplierID değeri 11 olan Products tablosu alanlarının UnitPrice değerlerinin toplamını ve kaç satır olduklarının sayısını elde ediyoruz. */
            SqlCommand cmdSum=new SqlCommand("SELECT SUM(UnitPrice),COUNT(SupplierID) FROM Products WHERE SupplierID=11",con);
            /* Bu SqlCommand nesnesinin içerdiği sql cümleciği ile, Products tablosundaki UnitPrice alanının değerlerinin ortalamasını elde ediyoruz. */
            SqlCommand cmdAvg=new SqlCommand("SELECT AVG(UnitPrice) FROM Products",con);

            /* Bağlantımızı açıyoruz. */
            con.Open();
            SqlDataReader dr; /* SqlDataReader nesnemizi tanımlıyoruz.*/ 
            dr=cmdSum.ExecuteReader(); 
/* Komutumuzu çalıştırıp sonuçları bir akım şeklinde SqlDataReader nesnemize aktarılacağını belirtiyoruz. */
            /* SqlDataReader akım içinde satır okuyabildiği sürece devam edicek döngümüzü başlatıyoruz ve sorgu sonucu elde edilen değerleri ekrana yazdırıyoruz. */
            while(dr.Read())
            {
                Console.WriteLine("Toplam Fiyat {0}, Satır Sayısı {1} ",dr[0],dr[1]);
            }
            dr.Close(); /* SqlDataReader nesnemizi kapatıyoruz. */

            dr=cmdAvg.ExecuteReader(); /* Bu kez SqlDataReader nesnemizi ikinci sorgu cümleciğimizi çalıştıracak SqlCommand nesnesi ile oluşturuyoruz. */
            /* SqlDataReader akım içinde satır okuyabildiği sürece devam edicek döngümüzü başlatıyoruz ve sorgu sonucu elde edilen değerleri ekrana yazdırıyoruz. */
            while(dr.Read())
            {
                Console.WriteLine("Ortalama Fiyat {0}",dr[0]);
            }

            dr.Close(); /* SqlDataReader nesnemizi kapatıyoruz. */
            con.Close(); /* SqlConnection nesnemizi kapatıyoruz. */
        }
    }
}
```

Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk61_1.gif](/assets/images/2004/mk61_1.gif)

Şekil 1. SqlCommand ile Aggregate Fonksiyonlarının Kullanımı.

Şimdi gelelim, bu gibi işlemlerin DataTable sınıfı ile nasıl gerçekleştirilebileceğine. Çoğu zaman uygulamalarımızda bağlantısız katman nesneleri ile çalışkmaktayız. Bunlardan biriside DataTable nesnesidir. DataTable nesneleri bildiğiniz gibi, veritabanındaki bir tabloya ait içeriğin bellekte tutulduğu bölgeyi işaret ederler. Yada uygulama içerisinde bizim oluşturacağımız bir tablonun bellek görüntüsünü temsil ederler. Her iki haldede, DataTable nesnesinin temsil ettiği bölgede veriler yer alabilir. Bu veriler üzerinde, Aggregate Fonksiyonlarını kullanmak istediğimizde, aşağıda prototipi belirtilen Compute metodunu kullanabiliriz.

```csharp
public object Compute(string ifade,string filtre);
```

Compute metodu, belirtilen bir alan için, belirtilen filtreleme mekanizmasının şartları dahilinde, Aggregate Fonksiyonlarının işletilmesinde kullanılır. İlk parametrede SUM, AVG gibi Aggregate fonksiyonlarının kullanıldığı ifade yer alır. İkinci parametre ise karşılaştırma koşulumuzdur. Bu koşul aslında Where koşulunun devamındaki ifadeyi içerir. Dikkat edicek olursanız, Compute metodunun geri dönüş değerinin tipi Object türündendir. Bunun sebebi, çalıştırılan fonksiyonlar sonucu elde edilecek sonuçların veri tipinin tam olarak kestirilememesidir. Aşağıda, Compute metodunun kullanımına ilişkin örnek ifadeler yer almaktadır.

```csharp
object objToplam;
objToplam= Tablo.Compute("Sum(Primler)", "PerID = 8");
object objToplam;
objToplam= Tablo.Compute("Sum(Primler)", "Baslangic > 1/1/2004 AND Bitis < 31/1/2004");
```

Şimdi yukarıdaki örneğimizde, Product isimli veritabanına ait verileri bellekte bir DataTable içinde sakladığımızı düşünelim. Şimdi Aggregate fonksiyonlarımızı bu örnek üzerinde kullanalım. Dilerseniz bu sefer, DataTable üzerindeki Compute metodunun sonuçlarını daha kolay izleyebileceğimiz bir Windows uygulaması geliştirelim. Form tasarımımız aşağıdakine benzer şekilde olabilir.

![mk61_3.gif](/assets/images/2004/mk61_3.gif)

Şekil 2. Form tasarımımız.

Şimdide uygulamamızın kodlarını yazalım.

```csharp
/* SqlConnection, SqlDataAdapter ve DataTable nesnelerimiz tanımlanıyor.*/
SqlConnection con;
SqlDataAdapter da;
DataTable dtProducts;

private void Form1_Load(object sender, System.EventArgs e)
{
    con=new SqlConnection("Data Source=localhost;initial catalog=Northwind;Integrated Security=SSPI"); /* SqlConnection nesnemiz oluşturuluyor ve Northwind veritabanı için bir bağlantı hattı teşkil ediliyor. */
    da=new SqlDataAdapter("Select * From Products",con); /* SqlDataAdapter nesnemiz Products tablosundaki tüm veriler üzerinde çalışacak şekilde, geçerli bağlantı nesnesi üzerinden oluşturuluyor. */
    dtProducts=new DataTable("Urunler"); /* Products tablosundaki verilerin bellekte tutulacağı bölgeyi temsil edicek DataTable nesnemiz oluşturuluyor. */
}

private void btnDoldur_Click(object sender, System.EventArgs e)
{
    da.Fill(dtProducts); /* DataTable nesnemizin bellekte temsil ettiği bölge, SqlDataAdapter nesnemiz ile dolduruluyor. */
    dgProducts.DataSource=dtProducts; /* DataGrid nesnemiz veri kaynağına bağlanıyor ve Products tablosundaki verileri göstermesi sağlanıyoru. */
}

private void btnOrtalama_Click(object sender, System.EventArgs e)
{
    double ortalama;
    ortalama=Convert.ToDouble(dtProducts.Compute("AVG("+cmbAlan.SelectedItem.ToString()+")","SupplierID=11")); /* Burada kullanıcının seçtiği alana göre SupplierID değeri 11 olanların ortalaması hesaplanıyor. Sonuç noktalı sayı çıkabileceğinden Convert sınıfının ToDouble metodu ile Double veri tipine aktarılıyor. */
    lblOrt.Text=ortalama.ToString();
}

private void btnToplam_Click(object sender, System.EventArgs e)
{
    double toplam;
    toplam=Convert.ToDouble(dtProducts.Compute("SUM("+cmbAlan.SelectedItem.ToString()+")","SupplierID=11")); /* Bu kezde SupplierID alanının değeri 11 olan alanların Toplam değeri hesaplanıyor. */
    lblToplam.Text=toplam.ToString();
}
```

Uygulamadaki en önemli nokta Compute metodunun kullanım şeklidir. Burada kullanıcının ekrandaki ComboBox kontrolünden seçtiği alana göre işlemler yapılır. Uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz. Bu uygulama daha çok geliştirilebilir. Örneğin koşul ifadesininde kullanıcı tarafından belirlenmesi sağlanabilir. Bu geliştirmeleri siz değerli okurlarımıza bırakıyorum.

![mk61_2.gif](/assets/images/2004/mk61_2.gif)

Şekil 3. Compute metodu ile Aggregate fonksiyonlarının çalıştırılması.

Bu kısa makalemizde DataTable sınıfına ait Compute metodu yardımıyla bağlantısız katman verileri üzerinde Aggregate Fonksiyonlarını kolayca nasıl kullanabileceğimizi incelemeye çalıştık. Umarım siz değerli okurlarım için yararlı bir makale olmuştur. Bir sonraki makalemizde görüşmek dileğiyle, hepinize mutlu günler dilerim.