---
layout: post
title: "Overload Metodların Gücü"
date: 2003-11-13 13:00:00 +0300
description: "Bu makalemde sizlere overload kavramından bahsetmek istiyorum. Konunun daha iyi anlaşılabilmesi açısından, ilerliyen kısımlarda basit bir örnek üzerinde de çalışacağız. Öncelikle Overload ne demek bundan bahsedelim. Overload kelime anlamı olarak Aşırı Yükleme anlamına gelmektedir. C# programlama dilinde overload dendiğinde, aynı isme sahip birden fazla metod akla gelir. Bu metodlar aynı isimde olmalarına rağmen, farklı imzalara sahiptirler. Bu metodların imzalarını belirleyen unsurlar, parametre sayıları ve parametre tipleridir. Overload edilmiş metodları kullandığımız sınıflarda, bu sınıflara ait nesne örnekleri için aynı isme sahip fakat farklı görevleri yerine getirebilen ( veya aynı görevi farklı sayı veya tipte parametre ile yerine getirebilen ) fonksiyonellikler kazanmış oluruz."
categories:
  - csharp
tags:
  - csharp
  - dotnet
---
# Overload Metodların Gücü
Değerli Okurlarım, Merhabalar.

Bu makalemde sizlere overload kavramından bahsetmek istiyorum. Konunun daha iyi anlaşılabilmesi açısından, ilerliyen kısımlarda basit bir örnek üzerinde de çalışacağız.

Öncelikle Overload ne demek bundan bahsedelim. Overload kelime anlamı olarak Aşırı Yükleme anlamına gelmektedir. C# programlama dilinde overload dendiğinde, aynı isme sahip birden fazla metod akla gelir. Bu metodlar aynı isimde olmalarına rağmen, farklı imzalara sahiptirler. Bu metodların imzalarını belirleyen unsurlar, parametre sayıları ve parametre tipleridir. Overload edilmiş metodları kullandığımız sınıflarda, bu sınıflara ait nesne örnekleri için aynı isme sahip fakat farklı görevleri yerine getirebilen (veya aynı görevi farklı sayı veya tipte parametre ile yerine getirebilen) fonksiyonellikler kazanmış oluruz.

Örneğin;

![mk4_1.gif](/assets/images/2003/mk4_1.gif)

Şekil 1: Overload metodlar.

Şekil 1 de MetodA isminde 3 adet metod tanımı görüyoruz. Bu metodlar aynı isime sahip olmasına rağmen imzaları nedeni ile birbirlerinden tamamıyla farklı metodlar olarak algılanırlar. Bize sağladığı avantaj ise, bu metodları barındıran bir sınıf nesnesi yarattığımızda aynı isme sahip metodları farklı parametreler ile çağırabilmemizdir. Bu bir anlamda her metoda farklı isim vermek gibi bir karışıklığında bir nebze önüne geçer. Peki imza dediğimiz olay nedir? Bir metodun imzası şu unsurlardan oluşur.

Metod İmzası Kabul Edilen Unsurlar
Metod İmzası Kabul Edilmeyen Unsurlar

Parametre Sayısı
Parametrenin Tipleri

Metodun Geri Dönüş Tipi

Tablo 1. Kullanım Kuralları

Yukarıdaki unsurlara dikkat ettiğimiz sürece dilediğimiz sayıda aşırı yüklenmiş (overload edilmiş) metod yazabiliriz. Şimdi dilerseniz küçük bir Console uygulaması ile, overload metod oluşumuna engel teşkil eden duruma bir göz atalım.Öncelikle metodun geri dönüş tipinin metodun imzası olarak kabul edilemiyeceğininden bahsediyoruz. Aşğıdaki örneğimizi inceleyelim.

```csharp
using

System; 
namespace

Overloading1
{
    class Class1
     {
          public int Islem(int a)
          {
               return a*a;
          }

          public string Islem(int a)
          {
               string b=System.Convert.ToString(a);
               return "Yaşım:"+b;
          } 

          [STAThread]
          static void Main(string[] args)
          {
          }
     }
}
```

Overloading1.Class1' already defines a member called 'Islem'with the same parameter types

Örneğin yukarıdaki uygulamada, Islem isimli iki metod tanımlanmıştır. Aynı parametre tipi ve sayısına sahip olan bu metodların geri dönüş değerlerinin farklı olması nedeni ile derleyici tarafından farklı metodlar olarak algılanmış olması gerektiği düşünülebilir. Ancak böyle olmamaktadır. Uygulamayı derlemeye çalıştığımızda aşağıdaki hata mesajı ile karşılaşırız.

Yapıcı metodlarıda overload edebiliriz. Bu da oldukça önemli bir noktadır. Bunu zaten.NET ile program geliştirirken sıkça kullanırız. Örneğin SqlConnection sınıfından bir nesne örneği yaratmak istediğimizde, bunu yapabileceğimiz 2 overload edilmiş yapıcı metod olduğunu görürüz. Bunlardan birisi aşıda görünmektedir.

![mk4_3.gif](/assets/images/2003/mk4_3.gif)

Şekil 2. Örnek bir Overload Constructor (Aşırı Yüklenmiş Yapıcı) metod.

Dolayısıyla bizde yazdığımız sınıflara ait constructorları overload edebiliriz. Şimdi dilerseniz overload ile ilgili olaraktan kısa bir uygulama geliştirelim. Bu uygulamada yazdığımız bir sınıfa ait constructor metodları overload ederek değişik tipte fonksiyonellikler edinmeye çalışacağız.

Bu uygulamada KolayVeri isminde bir sınıfımız olucak. Bu sınıfın üç adet yapıcısı olucak. Yani iki adet overload constructor yazıcaz. İki tane diyorum çünkü C# zaten default constructoru biz yazmasak bile uygulamaya ekliyor. Bu default constructorlar parametre almayan constructorlardır. Overload ettiğimiz constructor metodlardan birisi ile, seçtiğimiz bir veritabanına bağlanıyoruz. Diğer overload metod ise, parametre olarak veritabanı adından başka, veritabanına bağlanmak için kullanıcı adı ve parola parametrelerinide alıyor. Nitekim çoğu zaman veritabanlarımızda yer alan bazı tablolara erişim yetkisi sınırlamaları ile karşılaşabiliriz. Bu durumda bu tablolara bağlantı açabilmek için yetkili kullanıcı adı ve parolayı kullanmamız gerekir. Böyle bir olayı canlandırmaya çalıştım. Elbetteki asıl amacımız overload constructor metodların nasıl yazıldığını, nasıl kullanıldığını göstermek. Örnek gelişmeye çok, hemde çok açık. Şimdi uygulamamızın bu ilk kısmına bir gözatalım. Aşğıdakine benzer bir form tasarım yapalım.

![mk4_4.gif](/assets/images/2003/mk4_4.gif)

Şimdi sıra geldi kodlarımızı yazmaya. Öncelikle uygulamamıza KolayVeri adında bir class ekliyoruz. Bu class’ın kodları aşağıdaki gibidir. Aslında uygulamaya bu aşamada baktığımızda SqlConnection nesnemizin bir bağlantı oluşturmasını özelleştirmiş gibi oluyoruz. Gerçektende aynı işlemleri zaten SqlConnection nesnesini overload constructor’lari ile yapabiliyoruz. Ancak temel amacımız aşırı yüklemeyi anlamak olduğu için programın çalışma amacının çok önemli olmadığı düşüncesindeyim. Umuyorum ki sizlere aşırı yükleme hakkında bilgi verebiliyor ve vizyonunuzu geliştirebiliyorumdur.

```csharp
using System;

using System.Data.SqlClient;

namespace Overloading
{
    public class KolayVeri
    {
        private string baglantiDurumu; /* Connection'ın durumunu tutacak ve sadece bu class içinde geçerli olan bir string değişken tanımladık. private anahtar kelimesi değişkenin sadece bu class içerisinde yaşayabilceğini belirtir. Yazmayabilirizde, nitekim C# default olarak değişkenleri private kabul eder.*/
        public string BaglantiDurumu /* Yukarıda belirttiğimiz baglantiDurumu isimli değişkenin sahip olduğu değeri, bu class'a ait nesne örneklerini kullandığımız yerde görebilmek için sadece okunabilir olan (readonly), bu sebeplede sadece Get bloğuna sahip olan bir özellik tanımlıyoruz.*/
        {
            get
            {
                return baglantiDurumu; /* Bu özelliğe eriştiğimizde baglantiDurumu değişkeninin o anki değeri geri döndürülecek. Yani özelliğin çağırıldığı yere döndürülücek.*/
            }
        }
        public KolayVeri() /* İşte C# derleyicisinin otomatik olarak eklediği parametresiz yapıcı metod. Biz bu yapıcıya tek satırlık bir kod ekliyoruz. Eğer nesne örneği parametresiz bir Constructor ile yapılırsa bu durumda baglantinin kapalı olduğunu belirtmek için baglantiDurumu değişkenine bir değer atıyoruz. Bu durumda uygulamamızda bu nesne örneğinin BaglantiDurumu özelliğine eriştiğimizde BAGLANAMADIK değerini elde edeceğiz.*/
        {
            baglantiDurumu = "BAGLANAMADIK";
        }

        public KolayVeri(string veritabaniAdi) /* Bizim yazdığımı aşırı yüklenmiş ilk yapıcı metoda gelince. Burada yapıcımız, parametre olarak bir string alıyor. Bu string veritabanının adını barındırıcak ve SqlConnection nesnemiz için gerekli bağlantı stringine bu veritabanının adını geçiricek.*/
        {
            string connectionString = "initial catalog=" + veritabaniAdi + ";data source=localhost;integrated security=sspi";
            SqlConnection con = new SqlConnection(connectionString); /* SqlConnection bağlantımız yaratılıyor.*/

            try /* Bağlantı işlemini bir try bloğunda yapıyoruz ki, herhangibir nedenle Sql sunucusuna bağlantı sağlanamassa (örneğin hatalı veritabanı adı nedeni ile) catch bloğunda baglantiDurumu değişkenine BAGLANAMADIK değerini atıyoruz. Bu durumda program içinde KolayVeri sınıfından örnek nesnenin BaglantiDurumu özelliğinin değerine baktığımızda BAGLANAMADIK değerini alıyoruz böylece bağlantının sağlanamadığına kanaat getiriyoruz. Kanaat dedikte aklıma Üsküdarda ki Kanaat lokantası geldi :) Yemekleri çok güzeldir. Sanırım karnımız acıktı değerli okuyucularım.Neyse kaldığımız yerden devam edelim.*/
            {
                con.Open();
                // Bağlantımız açılıyor.
                baglantiDurumu = "BAGLANDIK";
                /* BaglantiDurumu özelliğimiz (Property), baglantiDurumu değişkeni sayesinde BAGLANDIK değerini alıyor.*/
            }
            catch (Exception hata) /* Eğer bir hata olursa baglantiDurumu değişkenine BAGLANAMADIK değerini atıyoruz.*/
            {
                baglantiDurumu = "BAGLANAMADIK";
            }
        }

        public KolayVeri(string veritabaniAdi, string kullaniciAdi, string parola) /* Sıra geldi ikinci overload constructor metoda. Bu metod ekstradan iki parametre daha alıyor. Bir tanesi user id ye tekabül edicek olan kullaniciAdi, diğeri ise bu kullanıcı için password'e tekabül edicek olan parola. Bunlari SqlConnection'ın connection stringine alarak , veritabanına belirtilen kullanıcı ile giriş yapmış oluyoruz. Kodların işleyişi bir önceki metodumuz ile aynı.*/
        {
            string connectionString = "initial catalog=" + veritabaniAdi + ";data source=localhost;user id=" + kullaniciAdi + ";password=" + parola;
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                baglantiDurumu = "BAGLANDIK";
            }
            catch (Exception hata)
            {
                baglantiDurumu = "BAGLANAMADIK";
            }
        }
    }
}
```

Şimdi sıra geldi, formumuz üzerindeki kodları yazmaya.

```csharp
string veritabaniAdi;

private void lstDatabase_SelectedIndexChanged(object sender, System.EventArgs e)
{
	veritabaniAdi=lstDatabase.SelectedItem.ToString();
	/* Burada kv adında bir KolayVeri sınıfından nesne örneği (object instance) yaratılıyor. Dikkat edicek olursanız burada yazdığımı ikinci overload constructor'u kullandık.*/

	KolayVeri kv= new KolayVeri(veritabaniAdi); /* Burada KolayVeri( dediğimizde .NET bize kullanabileceğimiz aşırı yüklenmiş constructorları aşağıdaki şekilde olduğu gibi hatırlatacaktır. IntelliSence’in gözünü seveyim.*/
```

![mk4_5.gif](/assets/images/2003/mk4_5.gif)

```csharp
stbDurumBilgisi.Text=lstDatabase.SelectedItem.ToString()+" "+kv.BaglantiDurumu;

private void btnOzelBaglan_Click(object sender, System.EventArgs e)
{
	string kullanici,sifre;
	kullanici=txtKullaniciAdi.Text;

	sifre=txtParola.Text;
	veritabaniAdi=lstDatabase.SelectedItem.ToString();

	KolayVeri kvOzel= new KolayVeri(veritabaniAdi,kullanici,sifre); /* Burada ise diğer aşırı yüklenmiş yapıcımızı kullanarak bir KolayVeri nesne örneği oluşturuyoruz.*/
```

![mk4_6.gif](/assets/images/2003/mk4_6.gif)

```csharp
	stbDurumBilgisi.Text=lstDatabase.SelectedItem.ToString()+" "+kvOzel.BaglantiDurumu+" User:"+kullanici;
	}
}
```

Evet şimdide programın nasıl çalıştığına bir bakalım. Listbox nesnesi üzerinde bir veritabanı adına bastığımızda bu veritabanına bir bağlantı açılır.

![mk4_7.gif](/assets/images/2003/mk4_7.gif)

Şekil 6. Listboxta tıklanan veritabanına bağlandıktan sonra.

Ve birde kullanıcı adı ile parola verilerek nasıl bağlanacağımızı görelim.

![mk4_8.gif](/assets/images/2003/mk4_8.gif)

Şekil 7. Kullanıcı adı ve parola ile baplantı

Peki ya yanlış kullanıcı adı veya parola girersek.

![mk4_9.gif](/assets/images/2003/mk4_9.gif)

Şekil 8. Yanlık kullanıcı adı veya parolası sonrası.

Evet değerli MsAkademik okuyucuları bu seferlikte bu kadar. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler, yarınlar dilerim.