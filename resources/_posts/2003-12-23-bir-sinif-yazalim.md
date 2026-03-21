---
layout: post
title: "Bir Sınıf Yazalım"
date: 2003-12-23 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - oop
  - class
  - .net
---
Bugünkü makalemizde ADO.NET kavramı içerisinde sınıfları nasıl kullanabileceğimizi incelemeye çalışacak ve sınıf kavramına kısa bir giriş yapıcağız. Nitekim C# dili tam anlamıyla nesne yönelimli bir dildir. Bu dil içerisinde sınıf kavramının önemli bir yeri vardır. Bu kavramı iyi anlamak, her türlü teknikte, sınıfların avantajlarından yararlanmanızı ve kendinize özgü nesnelere sahip olabilmenizi sağlar. Zaten.net teknolojisinde yer alan her nesne, mutlaka sınıflardan türetilmektedir.

Çevremize baktığımız zaman, çok çeşitli canlılar görürüz. Örneğin çiçekler. Dünya üzerinde kaç tür (cins) çiçek olduğunu bileniniz var mı? Ama biz bir çiçek gördüğümüzde ona çoğunlukla “Çiçek” diye hitap ederiz özellikle adını bilmiyorsak. Sonra ise bu çiçeğin renginden, yapraklarının şeklinden, ait olduğu türden, adından bahsederiz. Çiçek tüm bu çiçekler için temel bir sınıf olarak kabul edilebilir. Dünya üzerindeki tüm çiçekler için ortak nitelikleri vardır.

Her çiçeğin bir renginin (renklerinin) olması gibi. İşte nesne yönelimli programlama kavramında bahsedilen ve her şeyin temelini oluşturan sınıf kavramı bu benzetme ile tamamen aynıdır. Çiçek bir sınıf olarak algılanırken, sokakta gördüğümüz her çiçek bu sınıfın ortak özelliklerine sahip birer nesne olarak nitelendirilebilir. Ancak tabiki çiçekler arasında da türler mevcuttur. Bu türler ise, Çiçek temel sınıfından türeyen kendi belirli özellikleri dışında Çiçek sınıfının özelliklerinide kalıtsal olarak alan başka sınıflardır. Bu yaklaşım inheritance (kalıtım) kavramı olarak ele alınır ve nesne yönelimli programlamanın temel üç öğesinden biridir. Kalıtım konusuna ve diğerlerine ilerliyen makalelerimizde değinmeye çalışacağız.

Bugün yapacağımız bir sınıfın temel yapı taşlarına kısaca değinmek ve kendimize ait işimize yarayabiliecek bir sınıf tasarlamak. Çiçek sınıfından gerçek C# ortamına geçtiğimizde, her şeyin bir nesne olduğunu görürüz. Ancak her nesne temel olarak Object sınıfından türemektedir. Yani herşeyin üstünde bir sınıf kavramı vardır. Sınıflar, bir takım üyelere sahiptir. Bu üyeler, bu sınıftan örneklendirilen nesneler için farklı değerlere sahip olurlar. Yani bir sınıf varken, bu sınıftan örneklendirilmiş n sayıda nesne oluşturabiliriz. Kaldıki bu nesnelerin her biri, tanımlandığı sınıf için ayrı ayrı özelliklere sahip olabilirler.

![mk25_1.gif](/assets/images/2003/mk25_1.gif)

Şekil 1. Sınıf (Class) ve Nesne (Object) Kavramı

Bir sınıf kendisinden oluşturulacak nesneler için bir takım üyeler içermelidir. Bu üyeler, alanlar (fields), metodlar (methods), yapıcılar (constructor), özellikler (properties), olaylar (events), delegeler (delegates) vb… dır. Alanlar verileri sınıf içerisinde tutmak amacıyla kullanılırlar. Bir takım işlevleri veya fonksiyonellikleri gerçekleştirmek için metodları kullanırız. Çoğunlukla sınıf içinde yer alan alanların veya özelliklerin ilk değerlerin atanması gibi hazırlık işlemlerinde ise yapıcıları kullanırız. Özellikler kapsülleme dediğimiz Encapsulating kavramının bir parçasıdır. Çoğunlukla, sınıf içersinden tanımladığımız alanlara, dışarıdan doğrudan erişilmesini istemeyiz. Bunun yerine bu alanlara erişen özellikleri kullanırız. İşte bu sınıf içindeki verileri dış dünyadan soyutlamaktır yani kapsüllemektir. Bir sınıfın genel hatları ile içereceği üyeleri aşağıdaki şekilde de görebilirsiniz.

![mk25_2.gif](/assets/images/2003/mk25_2.gif)

Şekil 2. Bir sınıfın üyeleri.

Sınıflar ile ilgili bu kısa bilgilerden sonra dilerseniz sınıf kavramını daha iyi anlamamızı sağlıyacak basit bir örnek geliştirelim. Sınıflar ve üyeleri ile ilgili diğer kavramları kodlar içerisinde yer alan yorum satırlarında açıklamaya devam edeceğiz. Bu örnek çalışmamızda, Sql Suncusuna bağlanırken, bağlantı işlemlerini kolaylaştıracak birtakım üyeler sağlıyan bir sınıf geliştirmeye çalışacağız. Kodları yazdıkça bunu çok daha iyi anlayacaksınız. İşte bu uygulama için geliştirdiğimiz, veri isimli sınıfımızın kodları.

```csharp
using System;
using System.Data.SqlClient;

namespace Veriler /* Sınıfımız Veriler isimli isim uzayında yer alıyor. Çoğu zaman aynı isme sahip sınıflara sahip olabiliriz. İşte bu gibi durumlarda isim uzayları bu sınıfların birbirinden farklı olduğunu anlamamıza yardımcı olurlar.*/
{
    public class Veri /* Sınıfımızın adı Veri */
    {
        /* İzleyen satırlarda alan tanımlamalarının yapıldığını görmekteyiz. Bu alanlar private olarak tanımlanmıştır. Yani sadece bu sınıf içerisinden erişilebilir ve değerleri değiştirilebilir. Bu alanları tanımladığımız özelliklerin değerlerini tutmak amacıyla tanımlıyoruz. Amacımız bu değerlere sınıf dışından doğrudan erişilmesini engellemek.*/
        
        private string SunucuAdi;
        private string VeritabaniAdi;
        private string Kullanici;
        private string Parola;
        private SqlConnection Kon; /* Burada SqlConnection tipinden bir değişken tanımladık. */
        private bool BaglantiDurumu; /* Sql sunucumuza olan bağlantının açık olup olmadığına bakıcağız.*/
        private string HataDurumu; /* Sql sunucusuna bağlanırken hata olup olmadığına bakacağız.*/
        /* Aşağıda sunucu adında bir özellik tanımladık. Herbir özellik, get veya set bloklarından en az birini içermek zorundadır. */

        public string sunucu /* public tipteki üyelere sınıf içinden, sınıf dışından veya türetilmiş sınıflardan yani kısaca heryerden erişilebilmektedir.*/
        {
            get
            {
                return SunucuAdi; /* Get ile, sunucu isimli özelliğe bu sınıfın bir örneğinden erişildiğinde okunacak değerin alınabilmesi sağlanır . Bu değer bizim private olarak tanımladığımız SunucuAdi değişkeninin değeridir. */
            }
            set
            {
                SunucuAdi = value; /* Set bloğunda ise, bu özelliğe, bu sınıfın bir örneğinden değer atamak istediğimizde yani özelliğin gösterdiği private SunucuAdi alanının değerini değiştirmek için kullanırız. Özelliğe sınıf örneğinden atanan değer, value olarak taşınmakta ve SunucuAdi alanına aktarılmaktadır.*/
            }
        }

        public string veritabani
        {
            get
            {
                return VeritabaniAdi;
            }
            set
            {
                VeritabaniAdi =value;
            }
        }

        public string kullanici /* Bu özellik sadece set bloğuna sahip olduğu için sadece değer atanabilir ama içeriği görüntülenemez. Yani kullanici özelliğini bir sınıf örneğinde, Kullanici private alanının değerini öğrenmek için kullanamayız.*/
        {
            set
            {
                Kullanici =value;
            }
        }

        public string parola
        {
            set
            {
                Parola =value;
            }
        }

        public SqlConnection con /* Buradaki özellik SqlConnection nesne türündendir ve sadece okunabilir bir özelliktir. Nitekim sadece get bloğuna sahiptir. */
        {
            get
            {
                return Kon;
            }
        }

        public bool baglantiDurumu
        {
            get
            {
                return BaglantiDurumu;
            }
            set /* Burada set bloğunda başka kodlar da ekledik. Kullanıcımız bu sınıf örneği ile bir Sql bağlantısı yarattıktan sonra eğer bu bağlantıyı açmak isterse baglantiDurumu özelliğine true değerini göndermesi yeterli olucaktır. Eğer false değeri gönderirse bağlantı kapatılır. Bu işlemleri gerçekleştirmek için ise BaglantiAc ve BaglantiKapat isimli sadece bu sınıfa özel olan private metodlarımızı kullanıyoruz.*/
            {
                BaglantiDurumu =value;
                if (value == true)
                {
                    BaglantiAc();
                }
                else
                {
                    BaglantiKapat();
                }
            }
        }

        public string hataDurumu
        {
            get
            {
                return HataDurumu;
            }
        }

        public Veri() /* Her sınıf mutlaka hiç bir parametresi olmayan ve yandaki satırda görüldüğü gibi, sınıf adı ile aynı isme sahip bir metod içerir. Bu metod sınıfın yapıcı metodudur. Yani Constructor metodudur. Bir yapıcı metod içersinde çoğunlukla, sınıf içinde kullanılan alanlara başlangıç değerleri atanır veya ilk atamalar yapılır. Eğer siz bir yapıcı metod tanımlamaz iseniz, derleyici aynen bu metod gibi boş bir yapıcı oluşturacak ve sayısal alanlara 0, mantıksal alanlara false ve string alanlara null başlangıç değerlerini atayacaktır.*/
        {
        }

        /* Burada biz bu sınıfın yapıcı metodunu aşırı yüklüyoruz. Bu sınıftan bir nesneyi izleyen yapılandırıcı ile oluşturabiliriz. Bu durumda yapıcı metod içerdiği dört parametreyi alıcaktır. Metodun amacı ise belirtilen değerlere göre bir Sql bağlantısı yaratmaktır.*/
        public Veri(string sunucuAdi, string veritabaniAdi, string kullaniciAdi, string sifre)
        {
            SunucuAdi = sunucuAdi;
            VeritabaniAdi = veritabaniAdi;
            Kullanici = kullaniciAdi;
            Parola = sifre;
            Baglan();
        }

        /* Burada bir metod tanımladık. Bu metod ile bir Sql bağlantısı oluşturuyoruz. Eğer bir metod geriye herhangibir değer göndermiyecek ise yani vb.net teki fonksiyonlar gibi çalışmayacak ise void olarak tanımlanır. Ayrıca metodumuzun sadece bu sınıf içerisinde kullanılmasını istediğimiz için private olarak tanımladık. Bu sayede bu sınıf dışından örneğin formumuzdan ulaşamamalarını sağlamış oluyoruz.*/

        private void Baglan()
        {
            SqlConnection con =new SqlConnection("data source=" + SunucuAdi + ";initial catalog=" + VeritabaniAdi + ";user id=" + Kullanici + ";password=" + Parola);
            Kon = con;
        }

        /* Bu metod ile Sql sunucumuza olan bağlantıyı açıyoruz ve BaglantiDurumu alanına true değerini aktarıyoruz.*/
        private void BaglantiAc()  /* Bu metod private tanımlanmıştır. Çünkü sadece bu sınıf içerisinden çağırılabilsin istiyoruz. */
        {
            Kon.Open();
            try
            {
                BaglantiDurumu =true;
                HataDurumu = "Baglanti sağlandi";
            }
            catch (Exception h)
            {
                HataDurumu = "Baglanti Sağlanamdı. " + h.Message.ToString();
            }
        }
        /* Bu metod ilede Sql bağlantımızı kapatıyor ve BaglantiDurumu isimli alanımıza false değerini akatarıyoruz.*/
        private void BaglantiKapat()
        {
            Kon.Close();
            BaglantiDurumu =false;
        }
    }
}
```

Şimdi ise sınıfımızı kullandığımız örnek uygulama formunu tasarlayalım. Bu uygulamamız aşağıdaki form ve kontrollerinden oluşuyor.

![mk25_3.gif](/assets/images/2003/mk25_3.gif)

Şekil 3. Form Tasarımımız.

Formumuza ait kodlar ise şöyle.

```csharp
Veriler.Veri v;
private void btnBaglan_Click(object sender, System.EventArgs e)
{
     /* Bir sınıf örneği yaratmak için new anahtar kelimesini kullanırız. New anahtar kelimesi bize kullanabileceğimiz tüm yapıcı metodları gösterecektir. (IntelliSense özelliği). */
     v=new Veri(txtSunucu.Text,txtVeritabani.Text,txtKullanici.Text,txtParola.Text);
```

![mk25_4.gif](/assets/images/2003/mk25_4.gif)

```csharp
}

private void btnAc_Click(object sender, System.EventArgs e)
{

     v.baglantiDurumu= true;
     stbDurum.Text="Sunucu Bağlantısı Açık? "+v.baglantiDurumu.ToString();

}

private void btnKapat_Click(object sender, System.EventArgs e)
{
     v.baglantiDurumu=false;
     stbDurum.Text="Sunucu Bağlantısı Açık? "+v.baglantiDurumu.ToString();
} 
```

Şimdi uygulamamızı bir çalıştıralım.

![mk25_5.gif](/assets/images/2003/mk25_5.gif)

Şekil 5. Bağlantıyı açmamız halinde.

![mk25_6.gif](/assets/images/2003/mk25_6.gif)

Şekil 6. Bağlantıyı kapatmamız halinde.

Değerli okurlarım, ben bu sınıfın geliştirilmesini size bırakıyorum. Umarım sınıf kavramı ile ilgili bilgilerimizi hatırlamış ve yeni ufuklara yelken açmaya hazır hale gelmişsinizdir. Bir sonraki makalemizde sınıflar arasında kalıtım kavramına bakıcak ve böylece nesneye dayalı programlama terminolojisinin en önemli kavramlarından birini incelemeye çalışsacağız. Hepinize mutlu günler dilerim.