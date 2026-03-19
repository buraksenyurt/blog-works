---
layout: post
title: ".Net Tarafında Xml ile Oynamak-1"
date: 2007-02-28 12:00:00 +0300
description: "Bu makalemiz ile birlikte Xml mimarisini yönetimli kod(managed code) tarafından ele almaya çalışacak ve konuları örnek senaryolar üzerinden geliştireceğiz. Bildiğiniz gibi Xml(eXtensible Markup Language) çok yaygın olarak kullanılan, platformlar arası veri taşıma standartlarından birisidir."
categories:
  - csharp
tags:
  - csharp
  - xml
  - http
  - generics
---
# .Net Tarafında Xml ile Oynamak-1
Değerli Okurlarım Merhabalar,

Bu makalemiz ile birlikte Xml mimarisini yönetimli kod (managed code) tarafından ele almaya çalışacak ve konuları örnek senaryolar üzerinden geliştireceğiz. Bildiğiniz gibi Xml (eXtensible Markup Language) çok yaygın olarak kullanılan, platformlar arası veri taşıma standartlarından birisidir..Net Framework içerisinde Xml standartları ile doğrudan iş yapmamızı sağlayan yönetimli tipler bulunmaktadır. Bu tipler sayesinde sadece Xml değil, Xml ile ilişkili diğer standartlarıda etkili bir şekilde kullanabilmekteyiz. Xml Schema, Xslt ve XPath mimarileri bunlar arasında sayılabilir. Biz bu günkü makalemizde çalışma zamanında dinamik olarak bir Xml belgesini nasıl oluşturabileceğimizi, bu belgenin kaydedilmesini ve hatta yeniden okunmasını ele alacağımız bir senaryo üzerinde duracağız. Dilerseniz işe örnek senaryomuzdan bahsederek başlayalım.

Örneğimiz bir windows uygulaması olacak ve MDI (Multiple Document Interface) tarzında tasarlanacak. Ana formun altın yer alabilecek olan alt formlarımız (Child Forms) üzerinde mouse yardımıyla düz çizgiler çiziyor olacağız. Çizim işlemleri için GDI+ API'sinden yararlanıyor olacağız. Amacamız ise bu çizgilerin ilgili form için bir Xml dosyasında kendi belirleyeceğimiz bir desende saklanmasını sağlamak. Xml tarafında geçmeden önce Windows uygulamamızı tasarlamakta fayda var. Alt formlarımızda çizilen çizgilerin hatırlanması ve generic bir koleksiyon içerisinde saklanması programatik olarak işimizi oldukça kolaylaştıracaktır. Temel olarak bir çizginin, başlangıç ve bitiş koordinatlarına ait bilgilerin önemli olduğunu düşünecek olursak aşağıdaki gibi bir sınıf bizim için yeterli olacaktır.

![mk193_1.gif](/assets/images/2007/mk193_1.gif)

```csharp
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicXmlDocument
{
    public class Cizgim
    {
        private int _X1;
        private int _Y1;
        private int _X2;
        private int _Y2;

        public int X2
        {
            get { return _X2; }
            set { _X2 = value; }
        }
        public int Y2
        {
            get { return _Y2; }
            set { _Y2 = value; }
        }
        public int X1
        {
            get { return _X1; }
            set { _X1 = value; }
        }
        public int Y1
        {
            get { return _Y1; }
            set { _Y1 = value; }
        }
        public Cizgim(int x1, int y1,int x2,int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }
    }
}
```

Alt formlarımız kendi içlerinde, ekrana çizilen çizgilerin bilgilerini hatırlamak durumundadır. Nitekim bunu yapmadığımız takdirde formun mimimize edilmesi yada üstüne başka bir görüntünün gelmesi sonucu, üzerlerinde taşıdıkları çizgiler kaybolmaktadır. Bunun önüne geçmek için form üzerindeki çizgilerin hatırlanması şarttır. Bu sebepten alt formlarımızı şağıdaki gibi tasarlayabiliriz. Elbette hatırlanacak olan çizgilerin sürekli çizdirilmesi gerekir. Bu nedenle formlarımızın MouseUp ve Paint olayları içerisinde gerekli çizdirme operasyonları tetiklenmelidir.

![mk193_2.gif](/assets/images/2007/mk193_2.gif)

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DynamicXmlDocument
{
    public partial class Sahne : Form
    {
        private int _X, _Y;
        private bool _MouseDownOk = false;
        private List<Cizgim> _Cizgiler;

        public List<Cizgim> Cizgiler
        {
            get
            {
                return _Cizgiler;
            }
        }

        public Sahne()
        {
            InitializeComponent();
            _Cizgiler = new List<Cizgim>();
        }

        private void Sahne_MouseDown(object sender, MouseEventArgs e)
        {
            _X = e.X;
            _Y = e.Y;
            _MouseDownOk = true;
        }

        private void Sahne_MouseUp(object sender, MouseEventArgs e)
        { 
            _Cizgiler.Add(new Cizgim(_X,_Y,e.X,e.Y));
            _MouseDownOk = false;
        }

        private void Sahne_MouseMove(object sender, MouseEventArgs e)
        {
            if (_MouseDownOk)
            {
                Graphics grp = this.CreateGraphics();
                grp.Clear(this.BackColor);
                Pen pn = new Pen(Brushes.Blue); 
                pn.Width = 3;
                grp.DrawLine(pn, _X, _Y, e.X, e.Y);
                CizgileriCiz();
            }
        }

        private void Sahne_Paint(object sender, PaintEventArgs e)
        {
            CizgileriCiz();
        }

        private void CizgileriCiz()
        {
            Graphics grp = this.CreateGraphics();
            Pen pn = new Pen(Brushes.Blue);
            pn.Width = 3;
            foreach (Cizgim czg in _Cizgiler)
                grp.DrawLine(pn, czg.X1, czg.Y1, czg.X2, czg.Y2);
        }
    }
}
```

Bu kısımlarda çok fazla durmayacağız. Nitekim makalemizin konusu dışında kalmaktalar. Ancak özetle amacımızın GDI+ kullandığımızda, sahneler üzerindeki çizgilerin kaybolmasını engellemek olduğunu söyleyebiliriz. Bu formlarımızı taşıyacak olan MDI formumuza ise bir menü koyup, yeni sahneler açılabilmesini, var olan sahnelerin kaydedilebilmesini, yada bir Xml içerisinden var olan bir sahnenin okunabilmesini sağlayacağız. Bu düzenlemeler ile birlikte uygulamamız temel olarak aşağıdaki ekran görüntüsünde olduğu gibi çalışacaktır.

![mk193_3.gif](/assets/images/2007/mk193_3.gif)

Şimdi ilk amacımız ile işe başlayalım. Alt formlar üzerindeki çizgilerin saklandığı generic List koleksiyonu içeriğini, veri formatı tarafımızca tasarlanacak olan bir Xml dokümanında tutmak ve fiziki olarak kaydetmek. Bu amaçla öncelikli olarak Xml verimiz için bir şablon düşünelim. Bu formatı herkes kendi istediği şekilde tasarlayabilir. Örneğin ben herhangibir Sahnenin üzerindeki çizgileri aşağıdaki formatta tutmayı planladım.

```xml
<?xml version="1.0" encoding="utf-8"?>
    <Sahne>
        <Cizgim ID="0">
            <Koordinatlar>
                <Baslangic X="40" Y="40" />
                <Bitis X="187" Y="143" />
            </Koordinatlar>
        </Cizgim>
        <Cizgim ID="1">
            <Koordinatlar>
                <Baslangic X="241" Y="61" />
                <Bitis X="273" Y="259" />
            </Koordinatlar>
        </Cizgim>
</Sahne>
```

Xml verisi, bir Sahne içerisindeki her bir düz çizgi için Cizgim isimli bir element tutmaktadır. Bir çizginin başlangıç ve bitiş noktalarına ati koordinatlarını ise, Baslangic ve Bitis isimli elementler (elements) içerisinde X ve Y nitelikleri ile tutuluyor. Bir sahne içerisinde birden fazla çizgi olacağından bunları daha kolay bir şekilde ayırd edebilmek için Cizgim elementlerine birde ID isimli nitelikler (attributes) ekledik. Bu tip bir Xml içeriğini çalışma zamanında dinamik olarak oluşturabilmek için öncelikle Xml içerisindeki ağaç yapısını kavramak ve buna göre uygun yönetimli tipleri kullanmak gerekir. Aşağıdaki şekilde, Xml verimiz içerisinde yer alan üyelere dair fikirler verilmektedir.

![mk193_4.gif](/assets/images/2007/mk193_4.gif)

Buna göre şunları söyleyebiliriz;

- Dinamik olarak oluşturulacak Xml verisi processing instructions komutu ile başlamalıdır. Bu zaten geçerli bir Xml dokümanı için bir kuraldır.
- Xml dökümanımızın root elementinin adı Sahne'dir. Xml belgeleri sadece bir tane root element içerebilir. Ama mutlaka bir root element içermek zorundadır.
- Sahne elementi içerisinde birden fazla Cizgim elementi olabilir.
- Cizgim elementleri, formun üzerindeki çizgilerin koordinat bilgilerini taşımak üzere Koordinatlar isimli bir alt element (Child Element) içermektedir.
- Koordinatlar elementi içerisinde çizginin başlangıç ve bitiş noktalarının tutulduğu elementler aynı seviyedir.
- Başlangıç ve bitiş noktalarına ait X ve Y değerleri sırasıyla Baslangic ve Bitis elementleri içerisinde nitelikler (attributes) yardımıyla tutulmaktadır.
- Her bir çizgi için otomatik artan bir ID değeri programatik olarak ayarlanmakla birlikte, Cizgim elementleri içerisinde ID isimli niteliklerde saklanmaktadır.

Artık Xml tarafı için gerekli kodlamalarımızı yazabiliriz. Öncelikli olarak Xml işlemlerini tek bir çatı altında toplamak adına bir sınıftan yararlanmakta fayda vardır. Sınıfımız içerisinde herhangibir alt formun üzerindeki çizgileri kaydedecek olan static bir metodu aşağıdaki gibi düşünebiliriz.

![mk193_5.gif](/assets/images/2007/mk193_5.gif)

```csharp
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace DynamicXmlDocument
{
    class XmlDonusturucu
    {
        public static void ProjeKaydet(string dosya, List<Cizgim> cizgiler)
        {
            XmlDocument doc = new XmlDocument();
            // Önce Xml dökümanımızın başındaki processing instruction komutu oluşturulur.
            XmlProcessingInstruction instructor=doc.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
            // Oluşturulan processing instruction XmlDocument nesne örneğine eklenir.
            doc.AppendChild((XmlNode)instructor);
            // root element oluşturulur
            XmlNode rootNode = doc.CreateNode(XmlNodeType.Element, "Sahne", "");
            // root element XmlDocument nesne örneğine eklenir.
            doc.AppendChild(rootNode);
            // cizgiler koleksiyonundaki eleman sayısı kadar Cizgim elementi oluşturulur, alt elementleri doldurulur ve bunla Sahne node' una (bir başka deyişe rootNode' a eklenir.
            int cizgiNumarasi=0;
            foreach (Cizgim czg in cizgiler)
            {
                XmlNode cizgimNode = doc.CreateNode(XmlNodeType.Element, "Cizgim", "");

                XmlNode idAttribute = doc.CreateNode(XmlNodeType.Attribute, "ID", "");
                idAttribute.Value = cizgiNumarasi.ToString();
                cizgimNode.Attributes.Append((XmlAttribute)idAttribute);
    
                XmlNode koordinatlarNode = doc.CreateNode(XmlNodeType.Element, "Koordinatlar", "");
                XmlNode baslangicNode = doc.CreateNode(XmlNodeType.Element, "Baslangic", "");
    
                XmlAttribute xAttribute = doc.CreateAttribute("X");
                xAttribute.Value = czg.X1.ToString();
                baslangicNode.Attributes.Append(xAttribute);
    
                XmlAttribute yAttribute = doc.CreateAttribute("Y");
                yAttribute.Value = czg.Y1.ToString();
                baslangicNode.Attributes.Append(yAttribute);
    
                koordinatlarNode.AppendChild(baslangicNode);
    
                XmlNode bitisNode = doc.CreateNode(XmlNodeType.Element, "Bitis", "");
    
                xAttribute = doc.CreateAttribute("X");
                xAttribute.Value = czg.X2.ToString();
                bitisNode.Attributes.Append(xAttribute);

                yAttribute = doc.CreateAttribute("Y");
                yAttribute.Value = czg.Y2.ToString();
                bitisNode.Attributes.Append(yAttribute);
    
                koordinatlarNode.AppendChild(bitisNode);
        
                cizgimNode.AppendChild(koordinatlarNode);
    
                rootNode.AppendChild(cizgimNode);
    
                cizgiNumarasi++;
            }
            doc.Save(dosya);
        }
    }
}
```

ProjeKaydet isimli metodumuz ilk parametre olarak form üzerindeki çizgileri saklayan koleksiyon tipini almaktadır.(List). İkinci parametre olaraksa Xml bilgisinin kaydedileceği dosya adı verilmektedir. Bir windows uygulaması tasarladığımız için dosya adını saveFileDialog kontrolü yardımıyla gönderebiliriz. Peki metodumuzun içerisinde neler yapıyoruz? İlk olarak XmlDocument sınıfının AppendChild metodunun XmlNode tipinden parametreler aldığını belirtelim. Bunun dışında XmlDocument nesnesi belleğa açılan veri alanına Element, Attribute, Comment gibi Xml üyelerini eklemek için gerekli Create metodlarına sahiptir. Dolayısıyla bir Processing Instruction komutuna ihtiyacımız var ise bu durumda aşağıdaki kod satırından faydalanabiliriz.

```csharp
XmlProcessingInstruction instructor=doc.CreateProcessingInstruction("xml",  "version=\"1.0\" encoding=\"utf-8\"");
```

Bu kod satırı ile bir Processing Instruction oluşturulur. Sonrasında ise bunu XmlDocument nesne örneğine ilave etmemiz gerekir. Böylece bellekteki Xml alanı içerisine ilgili üyeyi dahil etmiş oluruz. Bu amaçlada aşağıdaki kod satırı kullanılabilir.

```csharp
doc.AppendChild((XmlNode)instructor);
```

Dikkat ederseniz AppendChild metodu içerisinde bir dönüştürme işlemi yapılmaktadır. Bunun sebebi metodun parametre olarak XmlNode tipinden bir değişken beklemesidir. Aslında XmlElement, XmlAttribute, XmlProcessingInstruction gibi tipleri düşündüğümüzde bunların dolaylıda olsa XmlNode tipinden türediklerini söyleyebiliriz. Buradaki mantık kodun kalan kısmınada uyarlanmıştır. Özellikle elementleri oluştururken XmlDocument sınıfının CreateNode metodu göz önüne alınmıştır.

```csharp
XmlNode cizgimNode = doc.CreateNode(XmlNodeType.Element, "Cizgim", "");

XmlNode idAttribute = doc.CreateNode(XmlNodeType.Attribute, "ID", "");
```

Örneğin yukarıdaki kod parçasında CreateNode metodlarında ilk parametre olarak üretilmek istenilen node tipi belirtilmiştir. Sonrasında ikinci parametre olarak node'un adı ve eğer varsa üçünü parametre ilede Xml isim alanı (namespace) belirtilmektedir.

> CreateNode metodunun aşırı yüklenmiş üç versiyonu vardır. Özellikle bir Xml isim alanı söz konusu olduğunda, şekilde versiyondan faydalanılabilinir.
> ![mk193_6.gif](/assets/images/2007/mk193_6.gif)

Artık sınıfımızı bu haliyle kullanıp alt formlarımızın çizgilerini Xml formatında saklamak için gerekli kodları yazabiliriz. Bu amaçla MDI formumuz içerisindeki Proje Kaydet başlıklı menü öğesi için aşağıdaki kodları yazmamız yeterli olacaktır.

```csharp
private void projeYeniToolStripMenuItem_Click(object sender, EventArgs e)
{
    // Yeni Sahne nesnesi oluşturmak.
    Sahne shn = new Sahne();
    shn.MdiParent = this;
    shn.Show();
}

private void projeKaydetToolStripMenuItem_Click(object sender, EventArgs e)
{
    // Eğer ekranda sahneler var ise bunlardan aktif olanın çizgilerini Xml dosyasına kaydet.
    if (this.MdiChildren.Length > 0)
    {
        Sahne shn = (Sahne)ActiveMdiChild;
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            XmlDonusturucu.ProjeKaydet(saveFileDialog1.FileName, shn.Cizgiler);
    }
}
```

Dikkat ederseniz yazmış olduğumuz sınıf sayesinde, sayfa programcısı için karmaşık olacak olan Xml ayrıştırma kodlarını kapsüllemiş olduk. (Encapsulate)

Şimdide Xml'e yazmış olduğumuz dosyayı okuyup, tekrardan ilgili şekillerini nasıl çizdireceğimizi görelim. Bu amaçla XmlDonusturucu sınıfımıza yeni bir static metod ekleyebiliriz. Metodumuzun yapması gereken, açılmak istenen Xml dosyasını parametre olarak almak ve geriye Cizgim tipinden elemanlar taşıyan generic bir List koleksiyonu döndürmektir. Ancak Sahne isimli Windows forumumuzun içerisinde yer alan Cizgiler isimli özelliğimiz yanlız okunabilir (read-only) olarak tasarlanmıştır. Dolayısıyla ilk önce, bu özelliğe bir set bloğu ekleyerek veri atanabilir hale getirmekle işe başlayabiliriz.

```csharp
public List<Cizgim> Cizgiler
{
    get
    {
        return _Cizgiler;
    }
    set
    {
        _Cizgiler = value;
    }
}
```

Ardından XmlDonusturucu sınıfımıza aşağıdaki ProjeAc isimli metodumuzu dahil edelim.

![mk193_7.gif](/assets/images/2007/mk193_7.gif)

```csharp
public static List<Cizgim> ProjeAc(string dosya)
{
    List<Cizgim> cizgiler = new List<Cizgim>();
    XmlDocument doc = new XmlDocument();
    doc.Load(dosya);
    XmlNodeList koordinatlar = doc.SelectNodes("/Sahne/Cizgim/Koordinatlar");
    foreach (XmlNode koordinat in koordinatlar)
    {
        int x1 = Int32.Parse(koordinat["Baslangic"].Attributes["X"].Value);
        int y1 = Int32.Parse(koordinat["Baslangic"].Attributes["Y"].Value);
        int x2 = Int32.Parse(koordinat["Bitis"].Attributes["X"].Value);
        int y2 = Int32.Parse(koordinat["Bitis"].Attributes["Y"].Value);
        Cizgim czg = new Cizgim(x1, y1, x2, y2);
        cizgiler.Add(czg);
    }
    return cizgiler;
}
```

Metodumuzun belkide en can alıcı noktası XmlDocument nesne örneği üzerinden SelectNodes metodunun çağırılmasıdır. SelectNodes metodu parametre olarak aldığı XPath sorgusuna uygun olacak şekilde bir node listesi döndürmektedir. Dolayısıyla bu proje tarafından oluşturulmuş bir Xml dosyasını açtığımızda, Sahne root elementi içerisindeki Cizgim elementi içerisindeki tüm Koordinatlar elementleri elde edilebilecektir.(Tabi varsalar)

> XPath ile ilişkili olaraktan daha detaylı bilgi için daha önceki bir [makaleden](http://www.bsenyurt.com/MakaleGoster.aspx?ID=147) faydalanabilirsiniz.

Bundan sonra, elde edilen node listesi üzerinde bir iterasyon gerçekleştirilmektedir. Bu iterasyon içerisinde, her bir XmlNode nesne örneği ele alınır. Dikkat ederseniz Baslangic ve Bitis alt elementlerindeki X ve Y niteliklerinin değerlerine erişmek için aşağıdaki notasyon kullanılmıştır.

```csharp
koordinat[Alt Elementin Adı].Attributes[Niteliğin Adı].Value
```

Böylece bir cizgi için tutulan başlangıç ve bitiş noktası değerlerini kolayca elde edebiliriz. Metodumuz elde edilen bu değerlere göre Cizgim tipinden nesne örnekleri üretmektedir ve bunları cizgiler isimli koleksiyonda toplayarak geri döndürmektedir. Artık görsel tarafta tek yapmamız gereken, MDI pencerimizin menüsündeki proje aç başlıklı seçenek için aşağıdaki kodları yazmaktır. (Buradada Xml dosyasını açabilmek için openFileDialog kontrolünden yararlanılmıştır. Sadece Xml uzantılı dosyaları açmak veya yazmak için saveFileDialog ve openFileDialog kontrollerinin Filter özelliklerinden faydalanılmaktadır.)

```csharp
private void projeAToolStripMenuItem_Click(object sender, EventArgs e)
{ 
    if (openFileDialog1.ShowDialog() == DialogResult.OK)
    {
        Sahne shn = new Sahne();
        shn.Cizgiler = XmlDonusturucu.ProjeAc(openFileDialog1.FileName);
        shn.MdiParent = this;
        shn.Show();
    }
}
```

Eğer uygulamamızı çalıştıracak olursak aşağıdaki flash animasyonundakine benzer bir sonuç ile karşılaşırız. (Flash dosyasının boyutu 192 kb olduğundan yüklenmesi zaman alabilir.)

Elbette bu proje içinde söz konusu olan bir sürü bug vardır. Örneğin, kullanıcılar herhangibir veri içeriğini taşıyan Xml dosyalarınıda açabilmektedir. Böyle bir durumda Sahne isimli windows formu oluşturulmakta ama içerisine hiç bir çizgi doğal olarak gelmemektedir. Pekala okuma sırasında SelectNodes metodunun dönüş değerine göre bir takım kontrol mekanizmaları geliştirilebilir. Nitekim elde edilen XmlNodeList tipinin eleman sayısı 0 ise bu durumda kalan işlemleri yapmaya gerek yoktur. Ancak daha güvenli bir yol tercih edilebilir. Bir başka deyişle kullanıcıyı daha detaylı bir şekilde bilgilendirmek amacıyla açılacak olan Xml dökümanının bizim standart veri şablonumuza uygun olup olmadığı bir Xml Schema dosyası yardımıyla kontrol edilebilir.

> Xml Schema'larını yönetimli kod tarafında nasıl ele alabileceğimize dair daha önceki bir [makalemizden](http://www.bsenyurt.com/MakaleGoster.aspx?ID=172) yararlanabilirsiniz.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde XmlDocument tipini farklı bir şekilde kullanmaya çalıştık. Umarım sizler için yararlı bir deneyim olmuştur. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.