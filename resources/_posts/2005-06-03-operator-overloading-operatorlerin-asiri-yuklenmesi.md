---
layout: post
title: "Operator Overloading (Operatörlerin Aşırı Yüklenmesi)"
date: 2005-06-03 12:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - operator-overloading
  - method-overloading
---
Hepimiz uygulamalarımızda sıklıkla operatörleri kullanmaktayız. Matematiksel işlemlerde, koşullu ifadelerde,tip dönüştürme işlemlerinde vb...Ancak onların kendi yazdığımız sınıflar için özel anlamlar ifade edecek şekilde yüklenmesi ile pek az uğraşmaktayız. Basit bir toplama operatörünün bile, yeri geldiğinde kendi sınıflarımıza ait nesne örnekleri üzerinde daha farklı davranışlar gösterecek şekilde yeniden yapılandırılması son derece önemlidir. Bu aynı zamanda dilin sağladığı esnekliği ve genişletilebilirliğini de gözler önüne sergilemektedir. İşte bu makalemizde, basit olarak operatörlerin aşırı yüklenmelerinin nasıl gerçekleştirilebileceğini örnek bir uygulama üzerinden incelemeye çalışacağız.

İlk olarak senaryomuzdan kısaca bahsedelim. Uygulamamızda System.Drawing isim alanını kullanarak dörtgen ve eliptik şekilleri çizmemizi sağlayacak iki adet sınıfımız olacak. Bu sınıfların çizim metodlarına baktığımızda ortak parametreler içerdiklerini görürüz. Bu nedenle bu ortak parametreleri bir arada toplayacağımız bir üst sınıfıda işin içine katarak ilgili şekil sınıflarını buradan türeteceğiz. Amacımız kalıtım kavramı üzerinde durmak değil. Bunu sadece kod okunabilirliğini ve nesnelerinin kullanılabilirliğini kolaylaştırmak amacıyla gerçekleştiriyoruz. Peki bu sınıfların yer aldığı bir uygulamada hangi operatörleri ne amaçla aşırı yükleyebiliriz?

İlk başta akla gelen eliptik bir şeklin içerdiği koordinat, boyut, renk gibi değerleri ile birlikte bir dörtgene çevrilmesi olabilir. Burada dörtgen tipinden bir nesne örneğinin, bilinçli (explicit) veya bilinçsiz (implicit) olarak eliptik bir nesne örneğine dönüştürülmesi söz konusudur. Bunun için cast operatörünü aşırı yükleyebiliriz. Diğer taraftan, var olan dörtgen veya eliptik nesnelerinin kendi aralarında toplama operatörleri ile toplanması sonucu çeşitli kriterlere uyum sağlayacak yeni bir dörtgen veya elips nesnesini elde etmeyi düşünebiliriz. Örneğin, iki kareyi toplayıp, yeni boyutları bu iki karenin toplamı kadar olan başka bir kare nesnesini çizdirebiliriz. Bu işlevsellikte ancak ve ancak toplama operatörünün burada söz konusu olan sınıflar için aşırı yüklenmesi ile mümkün olabilir. Şimdi gelin uygulamamızı geliştirmeye başlayalım. Operatörlerin aşırı yüklenmesini aşağıdaki görünüme sahip bir windows uygulamasında inceleyeceğiz.

![mk123_1.gif](/assets/images/2005/mk123_1.gif)

Uygulamayı mümkün olduğu kadar basit tasarlamaya çalıştım. Amacımız operatörlerin aşırı yüklenmesini incelemek. Bu nedenle Macromedia Fireworks gibi bir grafik tasarım programını icat etmeye çalışmıyoruz. Programımız temel olarak belirli renkte çizgilere sahip olan dörtgensel ve eliptik şekilleri çiziyor. Bir şekli çizmek için genişlik ve yüksekliğini ilgili textBox kontrollerine atadıktan sonra mouse ile ekranın herhangibir yerine tıklamanız yeterli olacaktır. Bununla birlikte işe biraz renk katmak amacı ile çizgi renklerini seçebiliyorsunuz. Menüde bizim asıl ilgilendiğimiz iki seçenek var. Bunlardan birisi bir Elips nesnesini, Dörtgen tipinden bir nesneye dönüştürerek ekrana çiziyor. Diğer menü seçeneği ilede iki Dörtgen nesnesini toplayıp sonucunu ekrana çizdiriyoruz. Makalenin ilerleyen safhalarında iki şeklin boyutsal bazda birbirlerine eşit olup olmadığını bildirecek şekilde koşul operatörlerini de aşırı yükleyeceğiz. Nesneleri tutmak amacıyla iki ArrayList koleksiyonu kullanmayı tercih ettim. Elbetteki siz bu programı dahada geliştirmeli ve nesnelerin daha esnek olarak tutulabileceği bir yapıyı kurgulamalısınız.

Gelelim uygulamamızdaki kritik sınıflara. Bu sınıflar, Dortgen, Elips ve TemelSekil sınıflarıdır.

![mk123_5.gif](/assets/images/2005/mk123_5.gif)

Dortgen ve Elips sınıfları TemelSekil sınıfından türetilmiştir. Sebebi, Dortgen ve Elips sınıflarının çizimi için kullanılan metodların aynı tipte ve sayıda parametre alıyor olmalarıdır. Dolayısıyla çizim için gerekli materyalleri bir üst sınıfta tutmak ve bunlara tek bir yerden erişebilmek amacıya bu tarz bir yapı tercih edilmiştir. Sınıflarımıza ilişkin başlangıç kodları aşağıdaki gibidir.

Dörtgen.cs

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UsingGDIWithOperatorOverloading
{
    public class Dortgen:TemelSekil
    {
        public Dortgen(Panel aktifForm,Color color,int penSize,int x,int y,int width,int height)
        {
            base.m_X=x;
            base.m_Y=y;
            base.m_Width=width;
            base.m_Height=height;
            base.m_Color=color;
            base.m_PenSize=penSize;
            base.m_AktifForm=aktifForm; 
        }
    
        public Dortgen()
        { 
        }

        public void Ciz()
        {
            Graphics cizici=m_AktifForm.CreateGraphics();
            Pen kalem=new Pen(m_Color,m_PenSize);
            cizici.DrawRectangle(kalem,m_X,m_Y,m_Width,m_Height);
        }        
    }
}
```

Dortgen sınıfında şu an için sadece Ciz isimli bir metodumuz var. Constructor metodumuz aldığı parametreleri direkt olarak TemelSekil sınıfına göndermekte. Ciz metodu, Dortgen sınıfına ait nesne örneğini parametre olarak gelen alan üzerinde çizen işlevlere sahiptir. Dikkat ederseniz, dörtgenin çizileceği yer, çizgi kalınlığı, X ve Y koordinatları, çizgi rengi, şeklin genişliği ve yüksekliği gibi bilgiler parametrik olarak kullanılmaktadır. Elips sınıfıda Dortgen sınıfına çok benzer bir yapıdadır.

Elips.cs

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UsingGDIWithOperatorOverloading
{
    public class Elips:TemelSekil
    { 
        public Elips(Panel aktifForm,Color color,int penSize,int x,int y,int width,int height)
        {
            base.m_X=x;
            base.m_Y=y;
            base.m_Width=width;
            base.m_Height=height;
            base.m_Color=color;
            base.m_PenSize=penSize;
            base.m_AktifForm=aktifForm; 
        }

        public Elips()
        {
    
        }

        public void Ciz()
        {
            Graphics cizici=m_AktifForm.CreateGraphics();
            Pen kalem=new Pen(m_Color,m_PenSize);
            cizici.DrawEllipse(kalem,m_X,m_Y,m_Width,m_Height);
        }
    
        public static Elips operator+(Elips k1,Elips k2)
        {
            int R=(k1.m_Color.R+k2.m_Color.R)%255;
            int G=(k1.m_Color.B+k2.m_Color.B)%255;
            int B=(k1.m_Color.G+k2.m_Color.G)%255;
            Color c=Color.FromArgb(R,G,B);
            Elips elips=new Elips(k1.m_AktifForm,c,k1.m_PenSize+k2.m_PenSize, k1.m_X+k2.m_X,k1.m_Y+k2.m_Y,k1.m_Width+k2.m_Width,k1.m_Height+k2.m_Height);
            return elips;
        }
    }
}
```

Son olarak TemelSekil.cs sınıfımız ise aşağıdaki gibidir.

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UsingGDIWithOperatorOverloading
{
    public class TemelSekil
    {
        protected int m_X;
        protected int m_Y;
        protected int m_Width;
        protected int m_Height;
        protected Color m_Color;
        protected int m_PenSize;
        protected Panel m_AktifForm;

        public int X
        {
            get
            {
                return m_X;
            }
        }
        public int Y
        {
            get
            {
                return m_Y;
            }
        }
        public int Width
        {
            get
            {
                return m_Width;
            }
        }
        public int Height
        {
            get
            {
                return m_Height;
            }
        }
        public Color Renk
        {
            get
            {
                return m_Color;
            }
        }
        public Panel AktifForm
        {
            get
            {
                return m_AktifForm;
            }
        }

        public int KalemUcu
        {
            get
            {
                return m_PenSize;
           }
        }
        public TemelSekil()
        {
        }
    }
}
```

Şimdi gelelim asıl sorunumuza; bir Dortgen nesne örneğini oluşturmak son derece basittir.

```csharp
Dortgen dortgen=new Dortgen(this.pnlKaraTahta,Renk,2,Baslangic_X,Baslangic_Y,Genislik,Yukseklik);
```

Hatta bu nesneyi ekrana çizdirmek artık çok daha kolaydır.

```csharp
dortgen.Ciz();
```

Gel gelelim aşağıdaki kod satırlarının işletilmesi sonrasında nasıl bir sonuç alacağımı meçhuldür?

```csharp
Dortgen dortgen1=new Dortgen(this.pnlKaraTahta,Color.Black,2,5,20,100,50);
Dortgen dortgen2=new Dortgen(this.pnlKaraTahta,Color.Yellow,2,10,40,75,80);
Dortgen d=dortgen1+dortgen2;
d.Ciz();
```

Bu haliyle uygulamamızı derlediğimizde aşağıdaki hata mesajını alırız;

![dikkat.gif](/assets/images/2005/dikkat.gif)
Operator '+' cannot be applied to operands of type 'UsingGDIWithOperatorOverloading.Dortgen'and 'UsingGDIWithOperatorOverloading.Dortgen'

Sebep gayet açıktır. Dortgen sınıfı toplam işleminin nasıl yapılacağını bilemez. Bunu geliştirici olarak bizim ona öğretmemiz gerekmektedir. O halde gelin toplama işlemini bu sınıfa nasıl öğreteceğimize bakalım. Herşeyden önce operatörlerin aşırı yüklenmesi ile ilgili olaraktan bir takım kurallar vardır. Aslında bu kuralları bizde tahmin edebiliriz.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Kural 1; operatörler, operatör metodları yardımıyla aşırı yüklenirler. Bu sebepten bir metod gövdeleri, parametreleri ve dönüş değerleri vardır.
Kural 2; operatör metodları static olmalıdır. Bunun sebebi operatör işlevselliği için nesne örneğine ihtiyaç duyulmamasıdır.
Kural 3; operatör metodları operator anahtar kelimesini içermelidir. Örneğin: operator + gibi.
Kural 4; elbette heryerden erişilebilmeleri gerektiğinden public olmalıdır.
Kural 5; operatörler doğaları gereği en az bir operand ile çalışır. Dolayısıyla aşırı yükleyeceğimiz operator metodların en az bir parametre alması şarttır.

Bu kuralları dikkate aldığımızda Dortgen sınıfı için toplama operatörünü aşağıdaki haliyle aşırı yükleyebiliriz.

```csharp
public static Dortgen operator+(Dortgen k1,Dortgen k2)
{
    int R=(k1.m_Color.R+k2.m_Color.R)%255;
    int G=(k1.m_Color.B+k2.m_Color.B)%255;
    int B=(k1.m_Color.G+k2.m_Color.G)%255;
    Color c=Color.FromArgb(R,G,B);
    Dortgen Dortgen=new Dortgen(k1.m_AktifForm,c,k1.m_PenSize+k2.m_PenSize,k1.m_X+k2.m_X,k1.m_Y+k2.m_Y, k1.m_Width+k2.m_Width,k1.m_Height+k2.m_Height);
    return Dortgen;
}
```

Dikkat ederseniz + operatörümüze ilişkin metodumuz, Dortgen tipinde iki nesne örneğini alıp bunlar üzerinde bir takım işlemler yaparak sonuç olarak ürettiği Dortgen tipinden nesneyi geriye döndürmektedir. Artık biraz önce hata veren kodlarımız şimdi çalışacaktır. Asıl uygulamamızı yürüttüğümüzde aşağıdakine benzer bir sonuç elde ederiz.

![mk123_2.gif](/assets/images/2005/mk123_2.gif)

Toplama operatörüne yaptığımız yüklemeyi diğer operatörlere de yapabiliriz. Ancak aşırı yüklenecek operatörler arasında özel öneme sahip olanlar ve hatta aşırı yükleme yapılamıyacak olanlar da vardır. Sözgelimi ekrandaki bir elips şeklini dörtgen tipine çevirmek istediğimizi varsayalım. Burada bilinçsiz olarak aşağıdaki gibi bir atama yapmak isteyebiliriz.

```csharp
Dortgen d=elipsOrnegi;
```

Diğer yandan bilinçli olarakta aşağıdaki tarzda bir dönüşüm de yapmak isteyebiliriz.

```csharp
Dortgen d=(Dortgen)elipsOrnegi;
```

Dikkat ederseniz ilk örnekte bilinçsiz, ikinci örnekte ise bilinçli tür dönüşümü söz konusudur. Burada mevzu bahis olan dönüştürme işlemlerini ilgili sınıfa öğretebilmek için yine operatör aşırı yüklemeden faydalanabiliriz. Örneğin aşağıdaki metod bilinçli olarak Dortgen tipine ait cast operatörünü aşırı yüklemektedir.

```csharp
public static explicit operator Dortgen(Elips elips)
{
    TemelSekil ts=elips;
    Dortgen Dortgen=new Dortgen(ts.AktifForm,ts.Renk,ts.KalemUcu,ts.X,ts.Y,ts.Width,ts.Height);
    return Dortgen;
}
```

Metodumuzda dikkat çeken en önemli nokta explicit anahtar sözcüğüdür. Bu Dortgen anahtar sözcüğünün cast operatörü olarak kullanıldığı durumlarda blok içerisindeki kod satırlarının çalıştırılacağını ifade etmektedir. Aynı şekilde bilinçsiz tür dönüşümüne izin verecek operatör yüklemelerini de yapabiliriz. Tek yapmamız gereken implicit anahtar sözcüğünü kullanmaktır.

```csharp
public static implicit operator Dortgen(Elips elips)
{
    TemelSekil ts=elips;
    Dortgen Dortgen=new Dortgen(ts.AktifForm,ts.Renk,ts.KalemUcu,ts.X,ts.Y,ts.Width,ts.Height);
    return Dortgen;
}
```

Elbette dönüştürme operatörlerinin aşırı yüklenmesi ile ilgili olaraktan dikkat etmemiz gereken önemli bir ayrıntı vardır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Hem implicit hem de explicit operatörlerini aynı anda aşırı yükleyemeyiz.

Ancak, sadece implicit operatörünün yüklemesi ile, çalışma zamanında hem explicit hem de implicit dönüşümlere izin vermiş oluruz. Yani aşağıdaki iki kod satırıda başarılı bir şekilde çalışacaktır.

```csharp
Dortgen dortgen=elipsNesnesi1;
Dortgen dortgen2=(Dortgen)elipsNesnesi2;
```

Görüldüğü gibi operatörlerin aşırı yüklenmesi son derece kolay. Bunları kullandığımız windows uygulamasına ait kod satırları ise aşağıdaki gibidir.

```csharp
private Color Renk;
private int Baslangic_X;
private int Baslangic_Y;
private int Genislik;
private int Yukseklik;
private ArrayList alDortgenler;
private ArrayList alElipsler;

private void menuElipsToDortgen_Click(object sender, System.EventArgs e)
{
    if(alElipsler.Count>0)
    {
        Elips el=(Elips)alElipsler[0];
        Dortgen dortgen=el;
        dortgen.Ciz();
    }
}

private void BaslangicAyarlari()
{
    lblSecilenRenk.Text=Renk.Name;
    lblRenk.BackColor=Renk; 
}

private void btnRenkSec_Click(object sender, System.EventArgs e)
{
    colors.ShowDialog();
    Renk=colors.Color;
    BaslangicAyarlari();
}

private void Form1_Load(object sender, System.EventArgs e)
{
    Renk=Color.Black;
    alDortgenler=new ArrayList();
    alElipsler=new ArrayList();
    BaslangicAyarlari();
}

private void pnlKaraTahta_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
{
    Baslangic_X=e.X;
    Baslangic_Y=e.Y;
}

private void Ciz()
{
    if(rdbDortgen.Checked==true)
    {
        Dortgen dortgen=new Dortgen(this.pnlKaraTahta,Renk,2,Baslangic_X,Baslangic_Y,Genislik,Yukseklik);
        dortgen.Ciz();
        alDortgenler.Add(dortgen);
    }
    if(rdbElips.Checked==true)
    {
        Elips elips=new Elips(this.pnlKaraTahta,Renk,2,Baslangic_X,Baslangic_Y,Genislik,Yukseklik);
        elips.Ciz();
        alElipsler.Add(elips);
    }
}

private void menuTopla_Click(object sender, System.EventArgs e)
{
    if(alDortgenler.Count>=2)
    {
        Dortgen d1=(Dortgen)alDortgenler[0];
        Dortgen d2=(Dortgen)alDortgenler[1];
        Dortgen d3=d1+d2;
        d3.Ciz();
    }
}
private void menuTemizle_Click(object sender, System.EventArgs e)
{
    this.Refresh();
    Baslangic_X=0;
    Baslangic_Y=0;
    alDortgenler.Clear();
    alElipsler.Clear();
}

private void pnlKaraTahta_DoubleClick(object sender, System.EventArgs e)
{
    Genislik=Convert.ToInt32(txtGenislik.Text);
    Yukseklik=Convert.ToInt32(txtYukseklik.Text);
    Ciz();
}

private void menuKapat_Click(object sender, System.EventArgs e)
{
    Close();
}
```

Dortgen sınıfı içerisinde aritmetik operatörlerin ve dönüştürme operatörlerinin nasıl yükleneceğini kısaca inceledik. Dilersek koşullu ifadelerde kullanılan operatörleride aşırı yükleyebiliriz. Örneğin == operatörünü yeniden yükleyerek dörtgen sınıfımız için özel olarak kullanabiliriz.

![dikkat.gif](/assets/images/2005/dikkat.gif)
== gibi karşılaştırma operatörlerinin aşırı yüklenmesinde tek şart, zıt operatörlerinde yüklenme zorunluluğunun olmasıdır. Örneğin, == için!=, < için > operatörünün aşırı yüklenmesi gibi...

Örneğimiz çok basit olduğundan genellikle koleksiyonlarda tuttuğumuz ilk iki nesne üzerinde işlem yapıyoruz. Yine bu tarz bir işlem yaptığımızı düşünelim ve iki Dortgen nesnesinin boyutlarının aynı olması halinde eşit olduklarını gösterelim. Bunun için Dortgen sınıfında == ve!= operatörlerini aynı anda aşırı yüklemeliyiz. Aşağıdaki kodlarda Dortgen sınıfına bu işlevselliği nasıl kazandırdığımızı görebilirsiniz.

Dortgen.cs sınıfına eklenen operator metodlarımız;

```csharp
public static bool operator==(Dortgen d1,Dortgen d2)
{
    if((d1.Width==d2.Width)&&(d1.Height==d2.Height))
        return true;
    else
        return false;
}

public static bool operator !=(Dortgen d1,Dortgen d2)
{
    return !(d1==d2);
}
```

Operatörlerin uygulama içerisinde kullanımı;

```csharp
private void menuEsitmi_Click(object sender, System.EventArgs e)
{
    Dortgen d1=(Dortgen)alDortgenler[0];
    Dortgen d2=(Dortgen)alDortgenler[1];
    if(d1==d2)
    {
        MessageBox.Show("Dortgenlerin boyutları eşit...");
    }
    if(d1!=d2)
    {
        MessageBox.Show("Dörtgenlerin boyutları eşit değil...");
    }
}
```

Eşitlik kontrolünün sonucu;

![mk123_3.gif](/assets/images/2005/mk123_3.gif)

Eşit değildir kontrolünün sonucu;

![mk123_4.gif](/assets/images/2005/mk123_4.gif)

Elbetteki aşırı yükleme yapamayacağımız operatörlerde vardır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
=,.,?:, ->, new, is, as, sizeof,&&, ||, () operatörlerini aşırı yüklememiz yasaklanmıştır.

Görüldüğü gibi nesnelerimiz için, C# dilinde var olan operatörleri aşırı yüklemek son derece kolaydır. Dikkat etmemiz gereken bir takım kurallar vardır ki bunlar zamanla öğrenilebilir. Operatörlerin özellikle aşırı yüklenmesine ihtiyaç duyulacağı durumları göz önüne aldığımızda, grafik ve matematik uygulamalarının üst sıralarda yer aldığını görürüz. Örneğin sevgili Sefer ALGAN, Her Yönüyle C# Kitabında operatörlerin aşırı yüklenmesi ile ilgili olaraktan Kompleks sayıları incelemiştir. Özetle operatörleri aşırı yüklemek özellikle kendi oluşturduğumuz nesnelerin esnekliği açısından önemlidir. Bu makalemizde işlediğimiz [örnekte](/assets/files/2005/OperOver.rar) bahsedilen aşırı yükleme işlemleri sadece Dortgen sınıfı için yapılmıştır. Size tavsiyem Elips sınıfı içinde benzer yüklemeleri yapmaya çalışmanızdır.