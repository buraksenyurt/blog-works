---
layout: post
title: ".Net Tarafında Xml ile Oynamak-2"
date: 2007-03-08 12:00:00 +0300
categories:
  - csharp
tags:
  - xml
  - document-object-model
  - domain
  - x-path
---
Yıllar önce odamdaki bilgisayarımda arkadaşlarımın telefon ve doğum günü bilgilerini C tabanlı bir programda kütük dosyasına kaydetmeye çalışmıştım. O günlerde sadece bulunduğum oda içerisindeki alanla sınırlıyken, bir süre sonra internet ortamına taşınıvermiştik. Dolayısıyla artık kütük dosyasını başka ortamlara aktarabilme imkanı doğmuştu. Elbetteki bu taşıma işinin bir standart dahilinde olması önemli idi. Sonuçta günümüzde bu tip veri taşıma standartları için Xml kullanır hale geldik. Xml elbette beraberinde pek çok teknolojiyide getirdi.

XPath, XQuery, Xslt bunlardan sadece bazılarıdır. Dolayısıyla bu popüler veri standartı pek çok programlama dili tarafından desteklenir hale gelmiştir. Microsoft.Net Framework platformunda buna destek verecek tipler yer almaktadır. Bunlardan biriside bir önceki makalemizde değindiğimiz XmlDocument tipidir. Serinin bu ikinci bölümünde bu tipin diğer özelliklerinide öğrenmeye devam ediyor olacacağız. Hazır kütük dosyası yazma ve okuma işleminden söze başlamışken aynı işi Xml dosyası üzerinde nasıl yapabileceğimizi inceleyecebiliriz. Bu makalemizde telefonlardan ziyade kendi kütüphanemizdeki kitaplarımızı taşıyacak olan bir Xml dökümanı üzerinde bazı işlemler yapmaya çalışacağız. Elbette gerekli aksiyonların üzerinde gerçekleştirileceği bir Xml dosyasının olması gerekir. Bu dosyanın temel desenini aşağıdaki gibi tasarlayabiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Kitaplik>
    <Kitap Id="">
        <Ad></Ad>
        <Fiyat></Fiyat>
        <Basim></Basim>
        <Yazarlar>
            <Yazar></Yazar>
            <Yazar></Yazar>
        </Yazarlar>
    </Kitap>
.
.
.
.
</Kitaplik>
```

Öncelikli olarak root elementimizin Kitaplik olacağını söyleyebiliriz. Kitaplik elementi içerisinde Kitap tipinden bir çok alt element yer alabilir. Her Kitap elementi için Id isimli bir nitelik (attribute) söz konusudur. Bununla birlikte bir kitaba ait yazarlarda, Yazalar isimli element altındaki Yazar boğumları (nodes) içerisinde tutulmaktadır. Dolayısıyla programımız içerisindeki mantığı bu hiyerarşiye göre düzenlemeliyiz. Xml dökümanı üzerindeki işlemlerimizi gerçekleştirmek için yine DOM (Document Object Model) kullanıyor olacağız. Buna göre XmlDocument nesnesi oldukça işimize yarayacaktır. Bu makalemizde XmlDocument nesnesi üzerinden ele alabileceğimiz bazı yardımcı metod ve özellikler ise aşağıdaki gibidir.

Yardımcı Metod
Açıklaması

GetElementsByTagName
Parametre olarak verilen elementin adına göre geriye bir XmlNodeList döndürür. Örneğimizde tüm Kitap boğumlarını elde etmek istediğimiz durumlarda bu metoddan faydalanılmaktadır.

SelectSingleNode
Tek bir node elde etmek istediğimizde kullanabileceğimiz metod. Bir XPath ifadesi alarak çalışmaktadır. Örneğimiz göz önüne alındığında belirli bir Id değerine sahip bir node elde edilmek istendiği durumlarda kullanılabilir. Geriye XmlNode tipinden bir nesne örneği döndürmektedir.

CreateNode
Herhangibir Xml ağacı üzerinde bir boğum (node) oluşturmak istediğimizde kullanabiliriz. Genellikle oluşturulacak olan boğumun çeşidinide belirtiriz. Böylece element, processing instruction, comment, attribute gibi Xml üyelerini ve benzerlerini oluşturabiliriz. Örneğimizde yeni bir Kitap eklerken pek çok boğumu oluşturmamız gerekecektir. CreateNode metodu ile bu işlevsellikler sağlanabilir.

AppendChild
Genellikle oluşturulan bir boğumun başka bir boğuma eklenmesi amacıyla kullanılır. Örneğimizde, yeni bir Kitap eklenmek istendiğinde sıkça kullanacağımız bir metoddur.

Load
Xml içeriğini belleğe almak istediğimizde ele alacağımız metoddur.

Save
Xml içeriğini fiziki ortama kaydetmek için kullanabileceğimiz metoddur.

Yardımcı Özellikler
Açıklaması

FirstChild
Xml ağacı içerisindeki, root boğum içerisindeki veya herhangibir boğum içerisindeki alt boğumlarından ilkini elde etmemizi sağlar. Bu nedenle geriye bir XmlNode referansı döner.

PreviousSibling
Ağaç yapısında o an üzerinde durulan boğum biliniyorsa, bir öncekinin elde edilmesini sağlar. Geriye XmlNode tipinden bir referans döndürür.

NextSibling
Ağaç yapısında o an üzerinde durulan boğum biliniyorsa eğer, bir sonraki boğumun elde edilmesini sağlar. Geriye XmlNode tipinden bir referans döndürür.

LastChild
Root element altındaki veya herhangibir boğum altındaki alt boğumlardan sonuncusuna gidilmesini sağlar. Geriye XmlNode tipinden bir referans döndürür.

Attributes
Bir boğumun içerisindeki niteliklere erişmemizi sağlar. Özellikle bir Kitap elementinin Id niteliğini (attribute) elde etmek istediğimiz durumlarda kullanabiliriz.

Dilerseniz öncelikli olarak uygulama ekranımızı tasarlayarak işe başlayalım. Örneğimizi bir Windows uygulaması olacak şekilde tasarlayacağız. Bu amaçla aşağıdaki ekran görütünsündekine benzer bir form oluşturarak işe başlayabiliriz.

![mk194_2.gif](/assets/images/2007/mk194_2.gif)

Bizim için gerekli olan bir diğer üye ise kitaplarımızı uygulama ortamı içerisinde temsil edebileceğimiz bir tiptir. Bir başka deyişle Xml dökümanı içerisindeki herhangibir Kitap boğumunun alt elementlerinin ve niteliklerinin değerlerini çalışma zamanında nesnel olarak taşıyabilecek bir sınıf tasarımına ihtiyacımız vardır. Söz gelimizi, kullanıcı uygulamayı açtığında eğer Kitaplık.xml isimli bir dosya var ise ve içeriğinde Kitap elementleri bulunuyorsa, bunların liste kutusuna birer Kitap nesne örneği olarak atanması sağlanabilir. Bunu yaptığımızda, liste kutusundan seçilen bir öğenin işaret ettiği Kitap referansını bulabilir ve bu nesne ile ilişkili verileri çekebiliriz ki bu özellikle bir önceki yada bir sonraki boğuma geçme işlemleri sırasında da önem arz edecektir. Diğer taraftan seçilen kitabın, Xml ağaç yapısı içerisindeki yerini bulmak için kullanılacak XPath sorgusunda Id niteliğinin değeri gerekmektedir ve bunu liste kutusundan seçilen öğe üzerinden elde edebiliriz. Öyleyse gelin Kitap isimli sınıfımızı aşağıdaki gibi tasarlayalım.

```csharp
using System;

namespace ReadAndWrite
{
    class Kitap
    {
        #region Alanlar(Fields)

        private int _id;
        private string _ad;
        private DateTime _basim;
        private float _fiyat;
        private string _yazarlar;

        #endregion

        #region Özellikler(Properties)

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Ad
        {
            get { return _ad; }
            set { _ad = value; }
        }
        public DateTime Basim
        {
            get { return _basim; }
            set { _basim = value; }
        }
        public float Fiyat
        {
            get { return _fiyat; }
            set { _fiyat = value; }
        }
        public string Yazarlar
        {
            get { return _yazarlar; }
            set { _yazarlar = value; }
        }

        #endregion

        public Kitap(int id,string ad, DateTime basim, float fiyat, string yazarlar)
        {
            Id = id;
            Ad = ad;
            Basim = basim;
            Fiyat = fiyat;
            Yazarlar = yazarlar;
        }
        public override string ToString()
        {
            return Ad;
        }
    }
}
```

Bir kitabın birden fazla yazarı olabilmektedir. Bu amaçla bir yazara ait bilgileri tutacak başka bir sınıf tasarımıda düşünülebilirdi. Bu durumda Kitap sınıfımız kendi içerisinde yazarları taşıyabilecek bir koleksiyon veya diziye sahip olmalıdır. Biz örneğimizde biraz daha basite kaçtık ve birden fazla yazar bilgisini aralarına | işareti koyarak taşıyacak string tipinden tek bir özellik kullandık.

Şimdi gelelim daha önemli olan kısımlara. Özellikle Xml üzerinde yapacağımız genel işlevsellikleri taşıyacak ayrı bir tip tasarlayacağız. XmlYoneticisi isimli tipimiz genel olarak, Xml ağacı üzerinde navigasyon, Xml içeriğini liste kutusuna aktarma, Xml boğumu güncelleme, silme ve ekleme gibi işlevsellikleri bünyesinde barındırmaktadır. Elbette bunların işlenmesi sırasında ele alınacak bazı yardımcı fonksiyonellikler de yer almaktadır. XmlYoneticisi isimli sınıfımızın şeması ve kodları aşağıdaki gibidir.

![mk194_4.gif](/assets/images/2007/mk194_4.gif)

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Xml.XPath;

namespace ReadAndWrite
{
    class XmlYoneticisi
    {
        XmlDocument _doc;

        #region Yapıcı Metod(lar)

        public XmlYoneticisi()
        {
            _doc = new XmlDocument();

            if (!File.Exists("Kitaplik.xml"))
            {
                XmlProcessingInstruction instructor = _doc.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
                // Oluşturulan processing instruction XmlDocument nesne örneğine eklenir.
                _doc.AppendChild((XmlNode)instructor);
                // root element oluşturulur
                XmlNode rootNode = _doc.CreateNode(XmlNodeType.Element, "Kitaplik", "");
                // root element XmlDocument nesne örneğine eklenir.
                _doc.AppendChild(rootNode);
                _doc.Save("Kitaplik.xml"); 
            }
            _doc.Load("Kitaplik.xml");
        } 
        
        #endregion

        #region Genel Metodlar
        /// <summary>
        /// Herhangibir Kitap boğumu içerisindeki bilgilerini alıp bir Kitap nesne örneğinde birleştirilmesini sağlar
        /// </summary>
        /// <param name="bogum">Bilgileri alınacak olan Kitap boğumu (node)</param>
        /// <returns>Bogumdaki bilgilerden üretilen Kitap nesne örneği</returns>
        private Kitap BilgileriAl(XmlNode bogum)
        {
            int id;
            string ad = "", yazarlar = "";
            DateTime basim = DateTime.Now;
            float fiyat = 0;
            // Ad elementinin içeriği alınır
            ad = bogum["Ad"].InnerText;
            // Fiyat elementinin içeriği alınır
            fiyat = float.Parse(bogum["Fiyat"].InnerText);
            // Basim elementinin içeriği alınır
            basim = DateTime.Parse(bogum["Basim"].InnerText);
            // Id niteliğinin değeri Attributes özelliğine indeksleyici operatörü uygulanarak alınır
            id = Int32.Parse(bogum.Attributes["Id"].Value);
            yazarlar = "";
            // Yazarlar elementi içerisindeki tüm Yazar alt elementlerinin değerleri alınır.
            foreach (XmlNode yazar in bogum["Yazarlar"].ChildNodes)
                yazarlar += yazar.InnerText + "|";
            // Elde edilen değişken bilgilerinden Kitap nesne örneği üretilir.
            Kitap ktp = new Kitap(id, ad, basim, fiyat, yazarlar);
            // Üretilen nesne örneği geri döndürülür.
            return ktp;
        }
    
        /// <summary>
        /// XmlDocument nesne örneğinin işaret ettiği xml dokümanındaki Kitap boğumlarını çeker ve her biri için bir Kitap nesne örneği oluşturup, parametre olarak gelen         ListBox kontrolüne yükler.
        /// </summary>
        /// <param name="lst">Windows formundaki liste kontrolü</param>
        public void KitaplariCek(ListBox lst)
        {
            // Önce Liste kutusu içeriği temizlenir
            lst.Items.Clear();
            /* GetElementsByTagName metodu parametre olarak aldığı takı adlarını ilgili xml ağacından çeker ve bir XmlNodeList olarak geri döndürür. */
            XmlNodeList kitaplar = _doc.GetElementsByTagName("Kitap");
            // Elde edilen node listesindeki her bir XmlNode nesne örneği dolaşılır.
            foreach (XmlNode kitapNode in kitaplar)
            {
                lst.Items.Add(BilgileriAl(kitapNode));
            }
        } 
        #endregion

        #region Navigasyon Metodları
        /// <summary>
        /// Önceki ve Sonraki boğumların hareketi sırasında o an xml ağacında bulunulan elementin elde edilmesini sağlar.
        /// </summary>
        /// <param name="id">Konumu tespit edilecek olan Kitap elementinin Id değeri</param>
        /// <returns></returns>
        private XmlNode SecilenNode(int id)
        {
            /* Id değerine ait XmlNode' unu bulabilmek için XPath sorgusundan faydalanılır. Buna göre Kitaplik root elementi içerisindeki Kitap elementlerinden Id niteliğinin     değeri parametre olarak gelen değişken değerine eşit olan Kitap elementi bulunur. */
            XmlNode secilenNode = _doc.SelectSingleNode("/Kitaplik/Kitap[@Id=" + id + "]");
            return secilenNode;
        }

        /// <summary>
        /// Xml ağacında, root element altındaki ilk alt elemente gidilmesini sağlar
        /// </summary>
        /// <returns>Gidilen ilk alt elementteki değerler göre üretilen Kitap nesne örneğidir</returns>
        public Kitap Ilk()
        {
            // FirstChild özelliği ile DocumentElement özelliği üzerinden uygulandığı takdirde Kitap node' larından ilkine gidilir.
            XmlNode bogum = _doc.DocumentElement.FirstChild;
            return BilgileriAl(bogum);
        }

        /// <summary>
        /// Xml ağacında bulunulan boğumdan bir öncekine geçilmesini sağlar
        /// </summary>
        /// <param name="id">O an üzerinde bulunulan boğumun tespiti için gerekli olan id değeri</param>
        /// <returns>Bir önceki boğuma ait bilgilerden üretilen Kitap nesne örneği</returns>
        public Kitap Onceki(int id)
        {
            // Eğer ağaç üzerinde hangi boğumda olduğumuzu biliyorsak PreviousSibling özelliği sayesinde bir önceki boğuma geçebiliriz
            XmlNode bogum = SecilenNode(id).PreviousSibling;
            return BilgileriAl(bogum);
        }

        /// <summary>
        /// Xml ağacında bulunulan boğumdan bir sonrakine geçilmesini sağlar
        /// </summary>
        /// <param name="id">O an üzerinde bulunulan boğumun tespiti için gerekli olan id değeri</param>
        /// <returns>Bir sonraki boğuma ait bilgilerden üretilen Kitap nesne örneği</returns>
        public Kitap Sonraki(int id)
        {
            // Eğer ağaç üzerinde hangi boğumda olduğumuzu biliyorsak NextSibling özelliği sayesinde bir sonraki boğuma geçebiliriz
            XmlNode bogum = SecilenNode(id).NextSibling;
            return BilgileriAl(bogum);
        }

        /// <summary>
        /// Xml ağacında, root element altındaki son alt elemente gidilmesini sağlar
        /// </summary>
        /// <returns>Gidilen son alt elementteki değerler göre üretilen Kitap nesne örneğidir</returns>
        public Kitap Son()
        {
            // DocumentElement özelliği üzerinden uygulanan LastChild özelliği sayesinde root element içerisindeki son elemente gidilir.
            XmlNode bogum = _doc.DocumentElement.LastChild;
            return BilgileriAl(bogum);
        }

        #endregion

        #region Temel Veri Değiştirme Metodları
        /// <summary>
        /// Var olan bir boğumun içeriğin günceller
        /// </summary>
        /// <param name="id">Id değeri</param>
        /// <param name="ad">Kitabın yeni adı</param>
        /// <param name="lstYazarlari">Son haliyle kitabın yazarlarını taşıyan liste kutusu</param>
        /// <param name="fiyat">Kitabın yeni fiyatı</param>
        /// <param name="basim">Kitabın yeni basım tarihi</param>
        /// <param name="ktp">O an üzerinde durulan Kitap nesne örneğinin lstKitaplar liste kutusundaki referansı</param>
        public void Guncelle(int id, string ad, ListBox lstYazarlari, float fiyat, DateTime basim, Kitap ktp)
        {
            // Önce güncelleme yapılacak olan boğum tespit edilir.
            XmlNode bogum = SecilenNode(id);
            // Boğumun elemanlarına yeni değerleri atanır
            bogum["Ad"].InnerText = ad;
    
            // Yazalar boğumu bir liste kutusundan geldiği için önce xml ağacındaki yazarlar boğumu kaldırılır.
            bogum.RemoveChild(bogum["Yazarlar"]);
            // Sonra son hali ile yazarlar boğumu yeniden oluşturulur. 
            XmlNode yazarlar = _doc.CreateNode(XmlNodeType.Element, "Yazarlar", "");
            // Oluşturulan yazarlar boğumu tekrardan ağaç yapısı içerisinde o anki Kitap boğumuna eklenir.
            bogum.AppendChild(yazarlar);
        
            // Yazarları taşıyan liste kutusunun son içeriğne göre Yazarlar elementi altındaki Yazar alt elementleri tekrardan oluşturulur.
            string yazarlari = "";
            for (int i = 0; i < lstYazarlari.Items.Count; i++)
            {
                XmlNode yazar = _doc.CreateNode(XmlNodeType.Element, "Yazar", "");
                yazar.InnerText = lstYazarlari.Items[i].ToString();
                bogum["Yazarlar"].AppendChild(yazar);
                yazarlari += lstYazarlari.Items[i] + "|";
            }
            bogum["Fiyat"].InnerText = fiyat.ToString();
            // Tarih bilgisini standart olması açısından Universal formatında değiştiriyoruz
            bogum["Basim"].InnerText = basim.ToUniversalTime().ToString();
            // ağaç yapısındaki değişiklikleri son hali ile Xml dosyasına kaydediyoruz.
            _doc.Save("Kitaplik.xml");
        
            // Yapılan değişiklikleri, ilgili Kitap nesne örneği üzerindede gerçekleştiriyoruz.
            ktp.Ad = ad;
            ktp.Basim = basim;
            ktp.Fiyat = fiyat;
            ktp.Yazarlar = yazarlari;
        }
    
        /// <summary>
        /// Bir Kitap boğumunun ağaç yapısından silinmesini sağlar
        /// </summary>
        /// <param name="id">Silinecek Kitap' ın Id değeri</param>
        public void Sil(int id)
        {
            XmlNode bogum = SecilenNode(id);
            if (bogum != null)
            {
                // Secilen boğumun ağaç yapısından çıkartılmasını sağlamak için RemoveChild metodu kullanılır.
                _doc.DocumentElement.RemoveChild(bogum);
                // Son değişiklikler için Save metodu çağırılır
                _doc.Save("Kitaplik.xml");
            }
        }

        /// <summary>
        /// Yeni bir Kitap boğumunun xml içeriğine dahil edilmesini sağlar
        /// </summary>
        /// <param name="adi">Eklenecek kitabın adı</param>
        /// <param name="fiyati">Eklenecek kitabın fiyatı</param>
        /// <param name="lstYazarlari">Eklenecek kitabın yazalarını taşıyan liste kutusu</param>
        /// <param name="basimTarihi">Eklenecek kitabın basım tarihi</param>
        /// <returns>Yeni bilgilerden elde edilen Kitap nesne örneği</returns>
        public Kitap Ekle(int yeniId,string adi, float fiyati, ListBox lstYazarlari, DateTime basimTarihi)
        {
            // Ağaç yapısı içerisinde yeni bir Kitap boğumu oluşturulur
            XmlElement kitap = _doc.CreateElement("Kitap");
    
            // Id niteliği oluşturulur
            XmlAttribute id = _doc.CreateAttribute("Id");
            // Id niteliğinin değeri verilir

            id.Value = yeniId.ToString();
            // Oluşturulan Id niteliği Kitap boğumuna eklenir
            kitap.Attributes.Append(id);
 
            // Ad boğumu oluşturulur.
            XmlNode ad = _doc.CreateNode(XmlNodeType.Element, "Ad", "");
            // Ad boğumuna değeri verilir
            ad.InnerText = adi;
            // Ad boğumu Kitap boğumu altına eklenir
            kitap.AppendChild(ad);
        
            // Fiyat boğumu oluşturulur
            XmlNode fiyat = _doc.CreateNode(XmlNodeType.Element, "Fiyat", "");
            // Fiyat boğumunun değeri verilir
            fiyat.InnerText = fiyati.ToString();
            // Fiyat boğumun Kitap boğumu altına eklenir
            kitap.AppendChild(fiyat);
    
            // Basim boğumu oluşturlur.
            XmlNode basim = _doc.CreateNode(XmlNodeType.Element, "Basim", "");
            // Basim boğumun değeri verilir
            basim.InnerText = basimTarihi.ToUniversalTime().ToString();
            // Basim boğumu Kitap boğumu altına eklenir
            kitap.AppendChild(basim);
    
            // Yazarlar boğumu oluşturulur
            XmlNode yazarlar = _doc.CreateNode(XmlNodeType.Element, "Yazarlar", "");
        
            string yazarlarStr = "";
            for (int i = 0; i < lstYazarlari.Items.Count; i++)
            {
                // Gelen liste kutusu kontrolündeki her bir yazar için Yazar boğumu oluşturulur ve Yazarlar boğumu altına eklenir
                XmlNode yazar = _doc.CreateNode(XmlNodeType.Element, "Yazar", "");
                yazar.InnerText = lstYazarlari.Items[i].ToString();
                yazarlar.AppendChild(yazar);
                yazarlarStr += lstYazarlari.Items[i].ToString() + "|";
            }
            kitap.AppendChild(yazarlar);
            _doc.DocumentElement.AppendChild((XmlNode)kitap);
            // Xml ağacı Kitaplık.xml dosyasına kaydedilir
            _doc.Save("Kitaplik.xml");
            // Gelen bilgilerden elde edilen Kitap nesne örneği geri döndürülür
            return new Kitap(yeniId, adi, basimTarihi, fiyati, yazarlarStr);
        } 
        #endregion
    }
}
```

XmlYoneticisi isimli sınıfımızın kodları her ne kadar çok olsada aslında temel amaçlar bellidir. Genel olarak sınıfımız belleğe alınan Xml ağaç yapısı üzerinde navigasyon işlemlerini gerçekleştirebilmektedir, ağaca yeni bir kitap eklemek, seçilen kitabı silmek veya güncellemek gibi işlemleride yerine getirebilmektedir. Çok doğal olarak sınıfımızın Windows tarafında kullanılması gerekmektedir. Lakin windows formu içerisindede bazı kodlamalar yapmamız gerekir.

> Windows tarafında yer alan kalabalık kodları optimize etmek gerekmektedir. Bu amaçla buradaki işlemleri üstlenecek ayrı bir sınıf tasarımı ele alınabilir. Bunu yapmaya çalışmak uygulama geliştiriciyi bir adım daha öteye taşıyacak ve kod optimizasyonu, performans, bakım kolaylığı gibi konularda uygulamaya büyük avantajlar getirecektir.

Öyleyse gelin ilk aşamada Windows uygulamamızda bize gereken kodları aşağıdaki gibi yazmaya çalışalım.

![mk194_5.gif](/assets/images/2007/mk194_5.gif)

```csharp
public partial class Form1 : Form
{
    // Yönetsel işlemler için gerekli olan XmlYoneticisi tipimizi tanımlıyoruz
    XmlYoneticisi xmlYnt;

    public Form1()
    {
        InitializeComponent();

        // Xml üzerindeki işlemler için (ekleme,güncelleme,silme,navigasyon vs...) tasarlanan XmlYoneticisi nesne örneği oluşturulur.
        xmlYnt = new XmlYoneticisi();
        // Eğer Kitaplik.xml dosyası var ise Kitap bilgileri çekilir
        xmlYnt.KitaplariCek(lstKitaplar); 
    }

    /* Id değerini otomatik arttırabilmek için yardımcı bir metoda başvurabiliriz. Bu metod liste kutusundaki Kitap öğelerinde(eğer varsalar) yer alan Id değerlerinin en büyüğünü bulur. Bu elbette tek başına çalışan bir uygulama olacağından (yani client server mimaride yer almayacağından) eş zamanlı çakışma gibi durumlar ele alınmıyor.*/
    private int EnBuyuk()
    {
        int deger1=0;
        for (int i = 0;i< lstKitaplar.Items.Count; i++)
        {
            int deger2=((Kitap)lstKitaplar.Items[i]).Id;
            if (deger2 > deger1)
                deger1 = deger2;
        }
        return deger1;
    }
    private void Form1_Load(object sender, EventArgs e)
    {
        // Eğer liste kutusunda eleman var ise ilk elemana konumlan
        if (lstKitaplar.Items.Count > 0)
            lstKitaplar.SelectedIndex = 0;
    }

    #region Navigasyon işlemleri
    private void lstKitaplar_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Liste kutusunda bir öğeye tıklandığından buna ait bilgileri diğer kontrollere aktar
        Goster((Kitap)lstKitaplar.SelectedItem);
    }

    private bool Goster(Kitap ktp)
    {
        // Eğer bir Kitap nesne örneği var ise
        if (ktp != null)
        {
            // Diğer kontrolleri doldur
            lblId.Text = ktp.Id.ToString();
            txtAd.Text = ktp.Ad;
            txtBasim.Text = ktp.Basim.ToShortDateString();
            txtFiyat.Text = ktp.Fiyat.ToString();
            lstYazarlar.Items.Clear();
            // Yazar bilgilerinin Yazarlar isimli özellikte bir string katarı şeklinde tuttuğumuz için | işaretine göre ayrıştırıp ele alıyoruz.
            string[] yazarlari = ktp.Yazarlar.Split('|');
            for (int i = 0; i < yazarlari.Length - 1; i++)
                lstYazarlar.Items.Add(yazarlari[i]);
            return true;
        }
        else
            return false;
    }

    /* Not: Burada, liste kutusu içerisindeki nesnelerden de doğrudan faydalanılabilinir. Ancak Xml içerisindeki navigasyon işlemlerini öğrenebilmek için bu yol tercih edilmemiştir. */
    private void btnIlk_Click(object sender, EventArgs e)
    { 
        // İlk boğuma geç.
        if ((lstKitaplar.Items.Count > 0) && (lstKitaplar.SelectedIndex != 0))
            if (Goster(xmlYnt.Ilk()))
                lstKitaplar.SelectedIndex = 0;
    }

    private void btnOnceki_Click(object sender, EventArgs e)
    {
        /* Önceki boğuma geçme işlemi için Onceki metodunu çağırıyoruz. Burada doğal olarak eğer ilk elemanda isek ya da var olan eleman yoksa hareket edilmesinide engellemeliyiz. If kontrolünün yapılmasının sebebi budur. Bu ve benzeri kontrolleri diğer navigasyon işlemlerindede yapıyoruz. */
        if ((lstKitaplar.Items.Count > 0) && (lstKitaplar.SelectedIndex != 0))
            if (Goster(xmlYnt.Onceki(((Kitap)lstKitaplar.SelectedItem).Id)))
                lstKitaplar.SelectedIndex--;
    }

    private void btnSonraki_Click(object sender, EventArgs e)
    {
        // Sonraki boğuma geç
        if ((lstKitaplar.Items.Count > 0) && (lstKitaplar.SelectedIndex < lstKitaplar.Items.Count - 1))
            if (Goster(xmlYnt.Sonraki(((Kitap)lstKitaplar.SelectedItem).Id)))
                lstKitaplar.SelectedIndex++;
    }

    private void btnSon_Click(object sender, EventArgs e)
    {
        //Son boğuma geç
        if ((lstKitaplar.Items.Count > 0) && (lstKitaplar.SelectedIndex < lstKitaplar.Items.Count - 1))
            if (Goster(xmlYnt.Son()))
                lstKitaplar.SelectedIndex = lstKitaplar.Items.Count - 1;
    } 
    #endregion

    #region Temel veri işlemleri

    private void btnYazarEkle_Click(object sender, EventArgs e)
    { 
        // Eğer yazar liste kutusuna daha önce eklenmemişse 
        if (!lstYazarlar.Items.Contains(txtYazar.Text))
            if (!String.IsNullOrEmpty(txtYazar.Text)) // ve txtYazar kutucuğu boş değil ise ekle
                lstYazarlar.Items.Add(txtYazar.Text);
    }

    private void Temizle()
    {
        // Yeni bilgi girişi için kontrollerin içeriğini temizle
        txtAd.Text = string.Empty;
        lblId.Text = string.Empty;
        txtFiyat.Text = string.Empty;
        txtBasim.Text = string.Empty;
        lstYazarlar.Items.Clear();
        txtYazar.Text=string.Empty;
    }

    private void btnYeni_Click(object sender, EventArgs e)
    {
        // Yeni bir Kitap girilmek istendiğinde öncelikle veri girişi yapılacak kontrollerin içeriğini temizleriz.
        Temizle();
    }

    // Kullanıcının yeni bir kitap eklerken kontrollere eksik veri girişi yapıp yapmadığını denetliyor
    private bool GirisKontrol()
    {
        DateTime tarih;
        float fiyat;

        // Kullanıcının eksik kontrol girip girmediğini ele alabilmek için aşağıdaki bool değişken atamaları kullanılmıştır. TryParse metodunun nasıl kullanıldığına dikkat edelim.
        bool tarihGecerli = DateTime.TryParse(txtBasim.Text,out tarih);
        bool fiyatGecerli = float.TryParse(txtFiyat.Text,out fiyat);
        bool adGecerli = !String.IsNullOrEmpty(txtAd.Text);
        bool yazarlarGecerli = lstYazarlar.Items.Count == 0 ? false : true;
        // Eğer eksik veri girişi yok ise ekleme işlemini yap, tersine eksik veri girişi var ise uyarı mesajı ver.
        if ((tarihGecerli == false) || (fiyatGecerli == false) || (adGecerli == false) || (yazarlarGecerli == false))
            return false;
        else
        {
            //TODO: Girilen bilginin daha önceden eklenip eklenmediğinin kontrolü konulabilir.
            return true;
        }
    }

    private void btnKaydet_Click(object sender, EventArgs e)
    {
        if(!GirisKontrol())
            MessageBox.Show("Eksik giriş var");
        else
        {
            // Yeni kitabı Ekle metodu ile ekliyoruz. Ekle metodu geriye Kitap tipinden bire referans döndürdüğü için bunuda alıp liste kontrolüne otomatik olarak ekleyebiliriz.
            Kitap eklenen = xmlYnt.Ekle( EnBuyuk()+1,txtAd.Text, float.Parse(txtFiyat.Text), lstYazarlar, DateTime.Parse(txtBasim.Text));
            lstKitaplar.Items.Add(eklenen);
            // Eklenen yeni Kitaba ait bilgileri ilgili kontrollerde göster
            Goster(eklenen);
        }
    }

    private void btnGuncelle_Click(object sender, EventArgs e)
    {
        // Seçili olan xml boğumunu güncelle
        /* Güncelleme işlemi için Guncelle metodunu kullanmaktayız. Bu metodunda Ekle metodunda olduğu gibi geriye bir Kitap referansı döndürmesi belki göz önüne alınabilir. Böylece sunu tarafında güncellenen bilgiler ile ilişkili referansıda son hali ile değiştirmek mümkün olabilir. */
        if (lstKitaplar.Items.Count > 0)
            xmlYnt.Guncelle(int.Parse(lblId.Text), txtAd.Text, lstYazarlar, float.Parse(txtFiyat.Text), DateTime.Parse(txtBasim.Text), (Kitap)lstKitaplar.SelectedItem);
        else
            MessageBox.Show("Güncellenecek veri yok");
    }

    // Seçili olan xml boğumunu sil
    private void btnSil_Click(object sender, EventArgs e)
    {
        // Eğer silinebilecek bir eleman var ise
        if (lstKitaplar.Items.Count > 0)
        {
            // Seçili olan elemanı liste kutusundan elde edip, bunun üzerinden Id özelliğinin değerini alıyoruz.
            int silinecekKitapId = ((Kitap)lstKitaplar.SelectedItem).Id;
            // Xml ağacından çıkart.Bunun için Sil metodunu çağırıyoruz.
            xmlYnt.Sil(silinecekKitapId);
            // Xml ağacından ve dolayısıylada fiziki Xml dosyasından çıkarttığımız öğeyi Liste kutusundan da kaldırıyoruz
            lstKitaplar.Items.Remove(lstKitaplar.SelectedItem);
            Temizle();
            if (lstKitaplar.Items.Count > 0)
                lstKitaplar.SelectedIndex = 0;
        }
    } 
    #endregion

    private void lstYazarlar_KeyUp(object sender, KeyEventArgs e)
    {
        // yazarlardan biri seçili iken Delete tuşuna basılırsa onu lstYazarlar isimli liste kutusundan çıkartıyoruz
        if ((e.KeyCode == Keys.Delete)&&(lstYazarlar.Items.Count>0))
            lstYazarlar.Items.Remove(lstYazarlar.SelectedItem);
    }
}
```

Uygulamız çalışma zamanında aşağıdaki örnek ekran görüntüsünde olduğu gibi kullanılabilir. (Video formatı flash olup boyutu 239 kb olduğundan yüklenmesi zaman alabilir.)

Test amacıyla 3 yeni kitap girilmiştir. Kitaplardan birisi üzerinde değişiklik yapılmıştır. Var olan bir kitap silinmiştir ve navigasyon işlemleri gerçekleştirilmiştir. Ekran görüntüsündeki işlemleri yaptığımızda Xml dosyamızın son hali aşağıdaki gibi olacaktır.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Kitaplik>
    <Kitap Id="1">
        <Ad>.Net in ABC' si</Ad>
        <Fiyat>10</Fiyat>
        <Basim>28.02.2007 22:00:00</Basim>
        <Yazarlar>
            <Yazar>Emrah Uslu</Yazar>
            <Yazar>Osman Çokakoğlu</Yazar>
            <Yazar>Burcu Günel</Yazar>
        </Yazarlar>
    </Kitap>
    <Kitap Id="2">
        <Ad>Her Yönüyle C#</Ad>
        <Fiyat>29</Fiyat>
        <Basim>31.12.2002 22:00:00</Basim>
        <Yazarlar>
            <Yazar>Sefer Algan</Yazar>
        </Yazarlar>
    </Kitap>
</Kitaplik>
```

Programın elbetteki bazı bugları vardır, olmalıdır. Örneğin, kullanıcı arka arkaya Kaydet tuşlarına basabilir ki bu durumda aynı kitap defalarca eklenir. Bir şekilde bunun önüne geçmek gerekmektedir. Programı test ettikçe başka hatalarda çıkacaktır. Bu hataları tespit edip, tedbirlerini almaya çalışmak bizi biraz daha ileriye götürecektir. Sonuç itibariyle testler, uygulama geliştirme sürecinin önemli bir parçasıdır. İyi yapılan testler sonucu tespit edilen sorunların en optimal şekilde çözülmeside bu sürece dahildir. Dolayısıyla uygulamayı geliştirmenin siz değerli okurlar için önemli bir artı olacağı kanısındayım. Böylece geldik bir makalemizin daha sonuna. Bu makalemizde XmlDocument tipini ele alırken

- Fiziki bir Xml verisinin belleğe nasıl alınabileceğini,
- Bellek üzerindeki bir ağaçta nasıl hareket edebileceğimizi,
- Xml bilgisini taşıyan ağaça yeni boğumları nasıl ekleyebileceğimizi,
- Ağaçtaki herhangibir Xml bilgisini nasıl değiştirebileceğimizi,
- Ağaçtaki herhangibir Xml boğumunu nasıl silebileceğimizi,
- Ağaç üzerinde gerçekleştirilen değişikliklerin fiziki bir Xml dosyasına nasıl kaydedilebileceğini,

örnek bir uygulama üzerinden ele almaya çalıştık. Umarım sizler için yararlı bir deneyim olmuştur. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.