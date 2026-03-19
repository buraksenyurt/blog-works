---
layout: post
title: "Custom Serialization"
date: 2006-05-14 12:00:00 +0300
description: "Nesnelerin çalışma zamanındaki durumlarını alıp herhangi bir kaynağa doğru yazmak ve başka bir zaman dilimi içerisinde bu kaynaktan aynı nesne durumunu elde etmek amacıyla serileştirme tekniklerinden sıkça faydalanılmaktadır. Biz bu makalemizde özel serileştirmeyi (Custom Serialization), Framework 1.1' den itibaren incelemeye başlayacak ve 2.0' da getirilen yeniliklere değineceğiz. Bazen serileştirme (Serialization) veya ters-serileştirme (Deserialization) işlemleri sırasında, veriyi değiştirmek isteyebiliriz..."
categories:
  - csharp
tags:
  - csharp
  - soap
  - serialization
  - generics
---
# Custom Serialization
Değerli Okurlarım Merhabalar,

Nesnelerin çalışma zamanındaki durumlarını alıp herhangi bir kaynağa doğru yazmak ve başka bir zaman dilimi içerisinde bu kaynaktan aynı nesne durumunu elde etmek amacıyla serileştirme tekniklerinden sıkça faydalanılmaktadır. Biz bu makalemizde özel serileştirmeyi (Custom Serialization), Framework 1.1' den itibaren incelemeye başlayacak ve 2.0' da getirilen yeniliklere değineceğiz. Bazen serileştirme (Serialization) veya ters-serileştirme (Deserialization) işlemleri sırasında, veriyi değiştirmek isteyebiliriz. Örneğin serileştirilecek olan veriyi ilgili stream üzerine şifrelemek isteyebiliriz. Söz konusu stream bir ağ ortamında, fiziki bir dosya veya bellek bölgesi olabilir. Özellikle ağ üzerinden harekete eden verilerin serileştirme işlemine tabi tutulduğu Remoting gibi mimarilerde şifreleme işlemi zaman zaman önem arz edebilir. Elbette şifreleme gibi amaçlar dışında da başka nedenlerden dolayı özel serileştirme işlemlerini gerçekleştirmek isteyebiliriz.

Hangi nedenle olursa olsun özel serileştirme, kod tarafında bir takım özel işlemleri gerektirir. Özel serileştirmeyi kavrayabilmek ve uygulayabilmek için var olan serileştirme süreçlerini basitten detaya kadar incelemek gerekir. Dilerseniz ilk olarak bir nesnenin serileştirme sürecine kısaca bir göz atalım.

![mk161_1.gif](/assets/images/2006/mk161_1.gif)

Çoğunlukla, serileştirmek veya ters-serileştirmek istediğimiz verinin içeriği Formatter'lar tarafından kullanılırlar. Serileştirme işlemi sırasında bu bilgiden yararlanılarak veri bir stream'e doğru yazılır. Ters-serileştirme işleminde ise stream'den okunan veri yine formatter nesnesi yardımıyla orjinal nesne üzerine yüklenir. Serileştirme ve ters-serileştirme işlemleri için Framework 2.0' da getirilen en büyük yeniliklerden birisi, olayların ele alınabileceği metodların yazılabiliyor oluşudur. Framework 2.0 perspektifinden bakıldığında bir nesnenin serileştirilmesi veya ters-serileştirilmesi sırasında meydana gelen olaylar daha kolay ele alınabilmektedir.

![mk161_2.gif](/assets/images/2006/mk161_2.gif)

Şekilden de görebileceğiniz gibi serileştirme süreci gerçekleşirken meydana gelen iki olay vardır. Bunlardan ilki serileştirme işleminin gerçekleştiği sırada çalışan OnSerializing olayıdır. Serileştirme işlemi gerçekleştirildikten hemen sonra devreye giren olay ise OnSerialized'dir. Ters serileştirme (Deserialization) işleminde de benzer bir durum söz konusudur. Ters serileştirmenin gerçekleştiği sırada OnDeserializing olayı ele alınabilir. Bu işlem bittikten hemen sonrasında ise, eğer sınıf IDeserializationCallback arayüzünü uygulamış ise OnDeserialization olayı öncelikli olarak devreye girecektir. Son olarakta OnDeserialized olayı gerçekleşecektir. İşte Framework 2.0 bahsetmiş olduğumuz serileştirme olaylarını daha iyi ele alabileceğimiz yeni nitelikler sunmaktadır.

![dikkat.gif](/assets/images/2006/dikkat.gif)
OnSerializing, OnSerialized, OnDeserializing, OnDeserialized olayları sadece Binary serileştirmede gerçekleşir. Soap formatında yapılan serileştirmede sadece Serialization olayları meydana gelir.

Peki bahsetmiş olduğumuz bu olayları nasıl ele alacağız? Bunun için Framework 2.0 ile birlikte gelen yeni attribute (nitelik) larımız mevcuttur. Bu attribute'ların listesini ve kısa açıklamalarını aşağıdaki tabloda bulabilirsiniz.

Attribute
Kısa Açıklama

OnSerializing
Serileştirme işlemi sırasındaki olay metodlarını niteler.

OnSerialized
Serileştirme işlemi tamamlandıktan sonraki olay metodlarını niteler.

OnDeserializing
Ters-Serileştirme işlemi yapıldığı sıradaki olay metodlarını niteler.

OnDeserialized
Ters-Serileştirme işlemi tamamlandıktan sonraki olay metodlarını niteler.

Bu attribute'lar yardımıyla bir metodu işaretlediğimizde, serileştirme (ters-serileştirme) sırasındaki süreci ele alabiliriz. Framework 2.0 ile özel serileştirmeye getirilen yenilikleri ele almadan önce Framework 1.1' de bu işi nasıl gerçekleştirebileceğimizi incelemekte fayda var. Eğer bir sınıfa özel serileştirme uygulamak istiyorsak öncelikle bu sınıfa ISerializable arayüzünü uygulamamız gerekmektedir. Tabiki bu sınıfın Serializable niteliği ile işaretlenmiş bir sınıf olması gerektiğide unutulmamalıdır. ISerializable arayüzünün sunduğu GetObjectData metodu yardımıyla, veri içeriğini özel serileştirme işlemine tabi tutabiliriz. Diğer taraftan ters-serileştirme işlemi için söz konusu sınıfın mutlaka özel bir yapıcı metodu (constructor) kullanılması şarttır. Bu yapıcı metodun parametrik yapısı, ISerializable arayüzünden uygulanan GetObject metodunun parametrik yapısı ile aynı olmak zorundadır.

Konuyu daha iyi anlayabilmek için basit bir örnek geliştirerek makalemize devam edelim. Örneğimizde, Personel tipinden basit bir nesne örneğinin binary formatta serileştirmesi işlemi ele alınmaktadır. Lakin bu işlemde serileştirme yaparken nesnenin grafiğini oluşturan içerik Rijndael algoritmasına göre şifrelenmektedir. Ters-serileştirme işlemi sırasında ise, şifrelenmiş olan içerik tekrar Personel tipi nesne örneğine atanmaktadır. Uygulamamızda şifreleme işlemlerini ele alan SifrelemeYoneticisi isimli sınıfımız temel olarak bizim için gerekli encryption ve decryption metodlarını sağlamaktadır. Personel isimli sınıfımız ise bir personel için gerekli temel özellikleri taşıyıp özel serileştirme yeteneğine sahip olacak şekilde tasarlanmıştır. Dikkat edeceğimiz en önemli nokta, Personel isimli sınıfımızın uyguladığı GetObjectData metodu ve özel yapıcısıdır.

![mk161_5.gif](/assets/images/2006/mk161_5.gif)

```csharp
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace OzelSerilestirme
{
    // Personel sınıfımın ikili serileştirmeye destek verebilmesi ve özel serileştirmeyi kullanabilmesi için Serializable niteliğine mutlaka sahip olması gerekir.
    [Serializable]
    class Personel:ISerializable
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _ad;
    
        public string Ad
        {
            get { return _ad; }
            set { _ad = value; }
        }
        private DateTime _dogum;

        public DateTime Dogum
        {
            get { return _dogum; }
            set { _dogum = value; }
        }
        private double _maas;
        
        public double Maas
        {
            get { return _maas; }
            set { _maas = value; }
        }

        public override string ToString()
        {
            return Id.ToString() + " " + Ad + " " + Dogum.ToShortDateString() + " " + Maas.ToString();
        }

        public Personel(int id, string ad, DateTime dogum, double maas)
        {
            Id = id;
            Ad = ad;
            Dogum = dogum;
            Maas = maas;
        }

        #region Deserialization için kullanılan özel constructor metodumuz
    
        public Personel(SerializationInfo info, StreamingContext context)
        {
            // Şifrelenmiş veriler info nesnesi üzerinden alınarak, çözücü nesnemize gönderilir. Elde edilen sonuçlar Personel nesnesinin çalışma zamanındaki örneğinin ilgili alanlarına setlenir.
            Id = Convert.ToInt32(SifrelemeYoneticisi.SifreCoz((byte[])info.GetValue("Identity", typeof(object))));
            Ad = SifrelemeYoneticisi.SifreCoz((byte[])info.GetValue("Name", typeof(object)));
            Dogum = Convert.ToDateTime(SifrelemeYoneticisi.SifreCoz((byte[])info.GetValue("BirthDate", typeof(object))));
            Maas = Convert.ToDouble(SifrelemeYoneticisi.SifreCoz((byte[])info.GetValue("Salary", typeof(object))));
        }

        #endregion

        #region ISerializable Members
    
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Öncelikle, Personel sınıfının çalışma zamanındaki nesne örneğinin sahip olduğu özellik değerleri şifreleme işleminden geçirilir. Elde edilen byte dizileri ise info nesnesinin AddValue metodu yardımıyla, binary serileştirilmenin yapıldığı stream' e doğru yazılır.
            byte[] sifrelenmisId = SifrelemeYoneticisi.Sifrele(Id.ToString());
            byte[] sifrelenmisAd = SifrelemeYoneticisi.Sifrele(Ad);
            byte[] sifrelenmisDogum = SifrelemeYoneticisi.Sifrele(Dogum.ToString());
            byte[] sifrelenmisMaas = SifrelemeYoneticisi.Sifrele(Maas.ToString());
    
            info.AddValue("Identity", sifrelenmisId);
            info.AddValue("Name", sifrelenmisAd);
            info.AddValue("BirthDate", sifrelenmisDogum);
            info.AddValue("Salary", sifrelenmisMaas);
        }

        #endregion
    }

    // Şifreleme ve çözme işlemleri için kullandığımız sınıfımız, Rijndael algoritmasını temel alır.
    class SifrelemeYoneticisi
    {
        private static byte[] Key = { 0x01, 0x06, 0x03, 0x07, 0x05, 0x06, 0x07, 0x11, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        private static byte[] IV = { 0x01, 0x06, 0x03, 0x07, 0x05, 0x06, 0x07, 0x11, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15,     0x16 };

        public static byte[] Sifrele(string sifrelenecekVeri)
        {
            MemoryStream ms = new MemoryStream();
            RijndaelManaged rm=new RijndaelManaged();
            CryptoStream cs;
            byte[] veri = Encoding.UTF8.GetBytes(sifrelenecekVeri);
            cs = new CryptoStream(ms, rm.CreateEncryptor(Key, IV), CryptoStreamMode.Write);
            cs.Write(veri, 0, veri.Length);
            cs.Close();
            ms.Close();
            return ms.ToArray(); 
        }

        public static string SifreCoz(byte[] sifrelenmisVeri)
        {
            MemoryStream ms = new MemoryStream();
            RijndaelManaged rm = new RijndaelManaged();
            CryptoStream cs = new CryptoStream(ms, rm.CreateDecryptor(Key, IV), CryptoStreamMode.Write);
            cs.Write(sifrelenmisVeri, 0, sifrelenmisVeri.Length);
            cs.Close();
            ms.Close(); 
            return Encoding.UTF8.GetString(ms.ToArray()); 
        }
    }
}
```

Uygulama kodumuz ise aşağıdaki gibidir.

```csharp
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using OzelSerilestirme;

public class main
{
    public static void Main(string[] args)
    {
        #region Binary serileştirme

        BinaryFormatter bfYaz = new BinaryFormatter();
        FileStream fsYaz = new FileStream("Personel.txt", FileMode.Create);
        Personel pers = new Personel(1000, "Burak Selim Şenyurt", new DateTime(1976, 12, 4), 2000);
        bfYaz.Serialize(fsYaz, pers);
        fsYaz.Close();

        #endregion

        #region Binary ters-serileştirme

        BinaryFormatter bfOku = new BinaryFormatter();
        FileStream fsOku = new FileStream("Personel.txt", FileMode.Open);
        Personel persOkunan = (Personel)bfOku.Deserialize(fsOku);
        Console.WriteLine(persOkunan.ToString());
        fsOku.Close();

        #endregion
    }
}
```

Uygulamamızda ilk olarak bir Personel tipi nesne örneğini fiziki bir dosyaya ikili formatta serileştirmekteyiz. Sonrasında ise fiziki dosyadan okuduğumuz veriyi yeni bir Personel tipi nesne örneğine taşımaktayız. Buradaki işlemler aslında normal bir serileştirme işleminden farksız. Ancak Personel sınıfımız özel serileştirme işlemini uyguladığı için arka planda verilerin şifrelenmesi ve çözümlenmesi söz konusu.

Uygulama çalıştıktan sonra serileştirilmiş verimizi tutan dosyamızın içeriği aşağıdaki gibi olacaktır. Gördüğünüz gibi Id, Ad, Dogum ve Maas gibi bilgilerimiz şifrelenmiştir. Gerçi, normal bir serileştirme işlemi sırasında da binary formatını kullandığımız için veri içeriği çok kolay okunamaz haldedir. Ancak özellikle string bazlı veriler okunabilir formatta olacaktır. Yinede biz, kullandığımız şifreleme anahtarına sahip olmayanların, nesneyi o anki içeriği ile elde edememesini garanti altına almış oluyoruz. Aşağıdaki ilk ekran görüntüsünde Personel tipine ait nesne örneğimizin şifrelenmiş halini görmektesiniz. İkinci görüntüde ise normal ikili serileştirmenin etkisini görmekteyiz.

![mk161_3.gif](/assets/images/2006/mk161_3.gif)

Eğer şifreleme yapmassak;

![mk161_6.gif](/assets/images/2006/mk161_6.gif)

Uygulamamızı çalıştırdığımızda şifrelediğimiz personel tipi nesne örneğinin başarılı bir şekilde çözülerek ters-serileştirilebildiğini de görmekteyiz. Elbette burada örnek olması açısından Rijndael algoritmasını kullandık. Bunun yerine diğer şifreleme algoritmalarınıda kullanabiliriz. Sonuçta önemli olan serileştirme sürecinde nerede neyi kullanacağımızı bilmektir.

![mk161_4.gif](/assets/images/2006/mk161_4.gif)

Framework 2.0' da öncedende bahsettiğimiz gibi, serileştirme ve ters-serileştirme sürecindeki olayları ele alabilecek metodları, çeşitli nitelikler yardımıyla yazabilmekteyiz. Bununla ilgili olaraktan Personel sınıfımızın yeni versiyonunu aşağıdaki gibi yazacağız. Dikkat ederseniz bu sefer özel bir constructor yazmadık veya GetObjectData metodunu kullanmadık. Diğer taraftan sınıfımıza ISerializable arayüzünüde uygulamadık. Framework 2.0 getirdiği bu yeniliklerle birlikte, ISerializable arayüzüne destek vermeye devam etmektedir. Özellikle attribute'ların desteklediği metodların SerializationInfo tiplerini doğrudan almaması nedeniyle, yukarıda yazdığımız şifreleme örneğini Framework 2.0' daki nitelikleri kullanacağımız metodlar yardımıyla yazmak gerçekten zordur.

![mk161_7.gif](/assets/images/2006/mk161_7.gif)

```csharp
[Serializable]
class Personel2
{
    private int _id;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }
    private string _ad;

    public string Ad
    {
        get { return _ad; }
        set { _ad = value; }
    }
    private DateTime _dogum;

    public DateTime Dogum
    {
        get { return _dogum; }
        set { _dogum = value; }
    }
    private double _maas;

    public double Maas
    {
        get { return _maas; }
        set { _maas = value; }
    }

    public override string ToString()
    {
        return Id.ToString() + " " + Ad + " " + Dogum.ToShortDateString() + " " + Maas.ToString();
    }

    public Personel2(int id, string ad, DateTime dogum, double maas)
    {
        Id = id;
        Ad = ad;
        Dogum = dogum;
        Maas = maas;
    }

    [OnSerializing()]
    protected void Serilestiriliyor(StreamingContext ctx)
    {
        Console.WriteLine("Serileştiriliyor...");
    }

    [OnSerialized()]
    protected void Serilestirildi(StreamingContext ctx)
    {
        Console.WriteLine("Serileştirildi...");
    }

    [OnDeserializing()]
    protected void TersSerilesitiriliyor(StreamingContext ctx)
    {
        Console.WriteLine("Ters serileştiriliyor...");
    }

    [OnDeserialized()]
    protected void TersSerilestirildi(StreamingContext ctx)
    {
        Console.WriteLine("Ters Serileştirildi...");
    }
}
```

Personel sınıfının ikinci versiyonuna dikkat ederseniz yeni attribute'larımızın kullanıldığı metodların hepsinin aynı parametrik yapıda olduğunu görebilirsiniz. Metodlarımızın hepsi değer dönürmeyen (void) ve sadece StreamingContext tipinden tek parametre alan modeldedir. Bu metodlar içerisinde şu an için sadece serileştirme sürecini takip etmekteyiz. Uygulamamızı Personel2 tipine göre güncellediğimizde aşağıdakine benzer bir ekran görüntüsü alırız. Dikkat ederseniz, tüm süreci yakalayabilmekteyiz. Buda bize seirleştirme süreci sırasında, dış kaynaklar ile ilgili işlemler yapabilmek için uygun bir zemin hazırlamaktadır.

![mk161_8.gif](/assets/images/2006/mk161_8.gif)

Framework 2.0 ile gelen belkide en önemli yenilik generic mimaridir. Özellikle generic modelin hemen her tipe uygulanabiliyor olması dikkatleri serileştirilebilir tipler üzerine de çekmektedir. Dolayısıyla özel serileştirme işlemini yaparken generic alanları ele alabileceğimizi söyleyebiliriz. Yanlız generic mimariyi kullandığımız sınıflarda eğer özel serileştirme yapıyorsak dikkat etmemiz gereken bir takım noktalar vardır. Örneğin aşağıdaki sınıfı ele alalım.

![mk161_10.gif](/assets/images/2006/mk161_10.gif)

```csharp
[Serializable()]
public class Personel3<G>:ISerializable
{
    private G _eleman;
    private string _ad;

    public string Ad
    {
        get { return _ad; }
        set { _ad = value; }
    }

    public G Eleman
    {
        get { return _eleman; }
        set { _eleman = value; }
    }

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Eleman", _eleman);
        info.AddValue("Ad", _ad);
    }

    #endregion

    public Personel3(SerializationInfo info, StreamingContext context)
    {   
        Ad = info.GetString("Ad");
        Eleman = (G)info.GetValue("Eleman",typeof(G));
    }

    public Personel3(G eleman,string ad)
    {
        _eleman = eleman;
        _ad = ad;
    }

    public override string ToString()
    {
        return _eleman.ToString()+" "+_ad.ToString();
    }
}
```

Görüldüğü gibi özel serileştirmeye ihtiyacımız olduğunda ele alabileceğimiz teknikler ortadadır. Şifreleme işlemleri gibi, verinin doğrudan değiştirilmesi gerektiği durumlarda ISerializable arayüzüne ait üyelerin kullanılması çok daha doğru olacaktır. Nitekim böyle bir ihtiyaçta doğrudan süreç içerisinde yer alan nesne değerlerini yakalayabilmek adına ele alınabilecek en uygun yol budur. Bununla birlikte serileştirme süreci içerisinde farklı kaynaklara yönelik işlemler yapılması düşünülüyorsa Framework 2.0 ile birlikte gelen attribute'lardan yararlanılabilir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kod için tıklayınız.](/assets/files/2006/UsingRijnadel.rar)