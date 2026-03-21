---
layout: post
title: "WPF - Veriye Bağlanmak (Data Binding)"
date: 2007-09-03 12:00:00 +0300
categories:
  - wpf
tags:
  - windows-presentation-foundation
  - data-binding
---
Bu gün geliştirdiğimiz programların çoğu veri (Data) ile ilişkili kaynakları kullanmaktadır. Özellikle büyük ölçekli pek çok proje tipi içerisinde mutlaka verilerin kullanılması söz konusudur. Veriler kimi zaman müşteri bilgilerini, kimiz zaman ürün bilgilerini, kimi zamanda uygulamaya ait konfigurasyon bilgilerini vb... tutar. Verilerin çoğu zaman veritabanı sistemlerinde, fiziki dosyalarda veya program içerisindeki özel tiplerde saklandıklarını görürüz. Çok doğal olarak bu veri depoları içerisinde tutulan bilgilerin son kullanıcılara gösterilmeside söz konusudur.

Bu noktada, geliştirilen uygulamaya bakılmaksızın pek çok veri bağlama tekniği olduğunu söyleyebiliriz. Ama bu yazımızda özellike Windows Presentation Foundation (WPF) uygulamalarında veri bağlama işlemlerinin nasıl yapılabileceğini basit örneklerden hareket ederek incelemeye çalışıyor olacağız. Windows tabanlı programlamada özellikle Visual Studio 2005 ile birlikte veri bağlama işlemlerinin dahada genişletildiğine şahit olduk. Data Source menüsü bunun en güzel örneklerinden birisidir. Hatta XML ve nesne (object) kaynaklarına daha kolay bağlanılmasını sağlayan XmlDataSource ve ObjectDataSource gibi kontrolleri gördük. Aslında veri bağlama (Data Binding) denildiği zaman akla gelmesi gereken; "bir tipin herhangibir üyesinin başka bir tipin sahip olduğu veriye otomatik olarak erişmesidir" diyebiliriz. Form üzerindeki bir metin kutusunun (TextBox) text özelliğinin, veritabanındaki bir alana bağlanması buna verilebilecek basit bir örnektir.

Ne varki WPF mimarisinde durum biraz farklı bir hal almıştır. Özellikle.Net Framework 3.0 ve LINQ (Language Integrated Query) gibi yenilikler, verilerin işleniş ve ele alınış şekillerinide değiştirmektedir. WPF'e kısaca bakıldığında ilk fark edilen noktalardan birisi.Net Framework 2.0 formlarındaki kadar çok kontrolün Toolbox sekmesine gelmediğidir. Dahası, Visual Studio 2005 uygulama geliştirme ortamından hatırladığımız pek çok veri kontrolü burada yer almamaktadır. Üstelik bizleri bekleyen sayısız yeni kontrol bulunmaktadır. Peki bir WPF uygulamasında, pencere (Window) üzerindeki kontrollerin çeşitli özelliklerinin verilere olan bağlantılarını nasıl gerçekleştirebiliriz? İşte bu yazımızda araştıracağımız ve örnekleyeceğimiz konular bunlar olacaktır.

WPF uygulamlarında veri kaynaklarına bağlanabilmek amacıyla kullanılan iki temel sağlayıcı bulunmaktadır. Bunlardan birisi XML kaynaklarına bağlanma işlemini gerçekkleştirmemizi sağlayan XmlDataProvider bileşenidir. Diğeri ise, herhangibir.Net nesnesine (.Net Object) bağlanmamızı kolaylaştıran ObjectDataProvider bileşenidir. (Bunların dışında özellikle bağlantısız katman nesneleri ilede kullanılan DataContext özelliğide veri bağlama işlemlerinde kullanılmaktadır.) Bu bileşenler sayesinde XAML içerisinden ilgili kaynaklara bağlanma, tek yönlü ve çift yönlü olarak veri transfer etme işlemlerini gerçekleştirebiliriz. Bu tiplerin kullanımını gösteren basit örneklerimize geçmeden önce veri bağlama işlemine basit ve genel bir bakış atmakta yarar var. Bu amaçla Visual Studio 2008 Beta 2 sürümünde yeni bir WPF uygulaması açıyor ve Window1 penceresinin XAML içeriğini aşağıdaki gibi kodluyoruz.

![mk221_1.gif](/assets/images/2007/mk221_1.gif)

```xml
<Window x:Class="DataBindIslemleri.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Bir kontrol niteliğini başka bir kontrodekine bağlamak" Height="169" Width="275" WindowStartupLocation="CenterScreen" Name="wndBasitBaglama">
    <Grid>
        <Button Background="#FFFFCC66" ClickMode="Release" Height="23" HorizontalAlignment="Left" Margin="10,16,0,0" Name="btnGiris" VerticalAlignment="Top" Width="75" Click="btnGiris_Click">Giriş</Button>
        <TextBox Background="Black" Foreground="{Binding ElementName=btnGiris, Path=Background}" Margin="10,47,44,54" Name="txtSifre" />
    </Grid>
</Window>
```

Bu ilk örneğimizde penceremiz üzerinde bir Button ve birde TextBox kontrolümüz bulunmaktadır. Dikkat edilmesi gereken nokta TextBox kontrolünün ön plan renginin, Button kontrolünün arka plan rengine bağlanmış olmasıdır. Bunun için TextBox elementi içerisinde Forground niteliğine bir değer ataması gerçekleştirilmiştir. Binding ile başlayan bu ifade içerisinde ElementName isimli özellik verinin kaynağı olan nesneyi temsil etmektedir. Bu örnekte söz konusu nesne btnGiris isimli Button kontrolüdür. Foreground özelliğine bağlanmasını istediğimiz veri içeriği ise Path tanımlaması ile belirtilmektedir. Buna göre btnGiris isimli veri kaynağındaki Background isimli özelliğin değerinin atanması söz konusudur. Uygulamayı çalıştırıp test ettiğimizde örnek olarak aşağıdakine benzer bir görüntü elde ederiz.

![mk221_2.gif](/assets/images/2007/mk221_2.gif)

Burada bahsedilen teknik en basit haliyle veri bağlamayı göstermektedir. Şimdi işi biraz daha ilerletip örnek bir XML dökümanı içerisinden, WPF kontrollerine veri bağlama işlemlerini nasıl gerçekleştirebileceğimize bakalım. Aşağıdaki gibi bir XML içeriğimiz olduğunu ve projemizde Urunler.xml adıyla kaydedildiğini göz önüne alabiliriz.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Depo>
    <Urun id="1000">
        <Ad>Ekran Kartı(VGA)</Ad>
        <BirimFiyat>35</BirimFiyat>
        <StokMiktari>100</StokMiktari>
        <Durum>OK.bmp</Durum>
        <Kategori>Yedek Parça</Kategori>
    </Urun>
    <Urun id="1001">
        <Ad>Intel Core Duo İşlemci (CPU)</Ad>
        <BirimFiyat>90</BirimFiyat>
        <StokMiktari>125</StokMiktari>
        <Durum>OK.bmp</Durum>
        <Kategori>Yedek Parça</Kategori>
    </Urun>
    <Urun id="1002">
        <Ad>17Inch LCD Monitor</Ad>
        <BirimFiyat>150</BirimFiyat>
        <StokMiktari>35</StokMiktari>
        <Durum>Warning.bmp</Durum>
        <Kategori>Ekran</Kategori>
    </Urun>
    <Urun id="1003">
        <Ad>250 GB Usb Harddisk</Ad>
        <BirimFiyat>150</BirimFiyat>
        <StokMiktari>90</StokMiktari>
        <Durum>Serious.bmp</Durum>
        <Kategori>Depolama Aygıtı</Kategori>
    </Urun>
    <Urun id="1004">
        <Ad>1 GB Usb Flash Bellek</Ad>
        <BirimFiyat>28</BirimFiyat>
        <StokMiktari>300</StokMiktari>
        <Durum>Warning.bmp</Durum>
        <Kategori>Depolama Aygıtı</Kategori>
    </Urun>
</Depo>
```

XML dökümanımız içerisinde çeşitli tipte bilgisayar ürünlerine ait bilgiler yer almaktadır. Temel olarak ürünün adı, birim fiyatı, stok miktarı, kategorisi ve durumuna ait bilgisi yer almaktadır. Söz gelimi bu XML veri kümesi içerisinde yer alan her bir ürünün adlarının bir ComboBox kontrolünde gösterilmesini istediğimizi düşünelim. Bu amaçla yine Visual Studio 2008 Beta 2 üzerinde tasarladığımız WPF projemize yeni bir pencere (Window) ekliyor ve XAML (eXtensible Application Markup Language) içeriğini aşağıdaki gibi düzenliyoruz.

![mk221_3.gif](/assets/images/2007/mk221_3.gif)

```xml
<Window x:Class="DataBindIslemleri.Window2" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="XmlDataProvider ile Xml verilerine basit bağlanmak" Height="150" Width="290" WindowStartupLocation="CenterScreen"> 
    <Grid>
        <Grid.Resources>
            <XmlDataProvider x:Key="UrunlerProvider" Source="Urunler.xml"/>
        </Grid.Resources>
        <ComboBox Height="28" Margin="23,42,60,0" Name="cmbUrunler" VerticalAlignment="Top" ItemsSource="{Binding Source={StaticResource UrunlerProvider},XPath=/Depo/Urun/Ad}" FontSize="13" FontWeight="Bold" />
        <Label Height="23" HorizontalAlignment="Left" Margin="18,12,0,0" Name="label1" VerticalAlignment="Top" Width="120" FontSize="12" FontWeight="Bold">Ürünler</Label> 
    </Grid>
</Window>
```

Daha öncedende bahsettiğimiz gibi WPF uygulamalarında veri bağlama işlemleri için XmlDataProvider ve ObjectDataProvider tipleri kullanılmaktadır. Bu örnekte yer alan Grid alanı içerisindeki kontrollerin bir XML veri kaynağını kullanacağını belirtmek amacıyla Grid.Resources elementi içerisinde XmlDataProvider tanımlaması yapılmıştır. XmlDataProvider elementi bu örnekte iki önemli nitelik (attribute) kullanmaktadır. Bunlardan birisi x isim alanı altında bulunan Key niteliğidir. Key aslında söz konusu veri kaynağının diğer kontrollerde ele alınmasını sağlamak amacıyla bir isim tanımlaması yapılmasını sağlar.

Öyleki UrunlerProvider adı, ComboBox kontrolünde ele alınan Binding ifadesinde kullanılmaktadır. XmlDataProvider elementinin Source niteliğinde ise veri kaynağının yeri işaret edilmektedir. Burada Urunler.xml isimli dosya gösterilmektedir. Source özelliğine internet üzerindeki bir URL adresi atanabileceği gibi, başka bir fiziki lokasyondaki XML dosya yeride verilebilir. ComboBox kontrolünde öğelerin (Items) içeriklerinin aslında XML dosyasındaki Ad elementlerinden geleceğini belirtmek amacıyla aşağıdaki ifade kullanılmıştır.

```csharp
ItemsSource="{Binding Source={StaticResource UrunlerProvider},XPath=/Depo/Urun/Ad}"
```

Burada en çok göze çarpan nokta XPath niteliğine atanan değerdir. Tahmin edileceği üzere burada bir XPath ifadesi kullanılmış ve Ad elementine kadar geçişler yapılmıştır. Bu anlamda özellikle XAML tarafında, XML veri kaynaklarının söz konusu olduğu durumlarda XPath ifadelerinin büyük önem taşıdığını ifade edebiliriz. Örneğimizi çalıştırdığımızda aşağıdaki ekran görüntüsü elde edilecektir. Dikkat edilecek olursa Urunler.xml içerisindeki tüm ürünlerin Ad elementlerinin değerleri gelmiştir.

![mk221_4.gif](/assets/images/2007/mk221_4.gif)

XmlDataProvider tipinin her zaman için harici bir XML veri kümesini işaret etmesine gerek yoktur. XAML içeriğinde yer alan gömülü (Embedded) bir XML veri kümeside bu anlamda kullanılabilir. Üçüncü örneğimizde bu durumu analiz ederek yazımıza devam edelim. Yeni bir pencereyi (Window) aşağıdaki gibi tasarladığımızı düşünelim. Kahramanımız yine bir ComboBox kontrolü olacak.

![mk221_5.gif](/assets/images/2007/mk221_5.gif)

```xml
<Window x:Class="DataBindIslemleri.Window3" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Inline Xml kaynağını kontrollere bağlamak" Height="173" Width="280">
    <Grid>
        <Grid.Resources>
            <XmlDataProvider x:Key="SehirVerisi">
                <x:XData>
                    <Sehirler xmlns="">
                        <Sehir kod="+90216" ad="Istanbul Anadolu"/>
                        <Sehir kod="202" ad="Kahire"/>
                        <Sehir kod="813" ad="Tokyo"/>
                        <Sehir kod="+44171" ad="Londra"/>
                        <Sehir kod="+1718" ad="New York City"/>
                        <Sehir kod="4989" ad="Münih"/>
                    </Sehirler>
                </x:XData>
            </XmlDataProvider>
        </Grid.Resources>
        <ComboBox ItemsSource="{Binding Source={StaticResource SehirVerisi}, XPath=/Sehirler/Sehir/@ad}" Margin="15,57,16,51" FontSize="14" FontWeight="Bold" /> 
        <Label Height="23" HorizontalAlignment="Left" Margin="15,22,0,0" Name="label1" VerticalAlignment="Top" Width="120">Şehir Telefon Kodları</Label>
    </Grid>
</Window>
```

Bu sefer XML veri kümesi XAML dökümanı içerisinde yer alan Grid'in kaynağı olarak tanımlanmıştır. Bunun için XmlDataProvider elementi içerisinde x:XData isimli bir alt eleman (Child Element) tanımlanmaktadır. Bu elementin içerisinde ise XML veri kümesi bulunmaktadır. Buradaki XML içeriği istenilen şekilde tasarlanabilir. Önemli olan noktalardan birisi bir önceki örnekte olduğu gibi yine x:Key niteliğinin tanımlanmış olmasıdır. ComboBox kontrolünde Sehir elementi içerisindeki ad niteliklerinin (attributes) değerleri gösterilmektedir. ItemsSource niteliğine atanan ifade içerisinde bağlanılacak veri kaynağı SehirVerisi olarak belirtildikten sonra ad niteliklerinin değerlerinin elde edilmesi için XPath ifadesinde @ işaret kullanılmıştır.(Hatırlayalım; XPath ifadelerinde nitelikleri ele alırken @ işareti kullanılır) Uygulama bu haliyle çalıştırıldığında aşağıdaki ekran görüntüsü ile karşılaşılacaktır.

![mk221_6.gif](/assets/images/2007/mk221_6.gif)

Görüldüğü gibi ad niteliklerinin değerleri ComboBox kontrolü içerisine alınmıştır. Geliştirdiğimiz son örneğin bir öncekinden tek farkı, harici bir XML veri kümesi kullanmaktansa, gömülü (Embeded) bir XML kaynağının kullanılmasıdır. Pek çok kaynakta özellikle MSDN'de bu tip bir XML içeriği için XML veri adası (XML Data Island) tanımlaması yapılmaktadır. XML veri adaları x:XData elementleri arasında tutulmak zorundadır.

Sıradaki örneğimizde yine bir XML veri kaynağını ele alacağız. Bu sefer diğer örneklerden farklı olarak ComboBox'ın ItemTemplate elementini ele alacağız. Bu sayede ComboBox içerisinde birden fazla veri bağlı kontrolü yan yana göstermemiz mümkün olacaktır. Bu amaçla yeni penceremizi aşağıdaki gibi tasarlamamız yeterlidir.

![mk221_7.gif](/assets/images/2007/mk221_7.gif)

```xml
<Window x:Class="DataBindIslemleri.Window4" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="DataTemplete Yardımıyla Xml verisine bağlanma" Height="154" Width="384">
    <Grid>
        <Grid.Resources>
            <XmlDataProvider x:Key="UrunVerileri" Source="Urunler.xml"/>
        </Grid.Resources>
        <ComboBox Height="40" Margin="21,37,15,0" Name="cmbUrunler" VerticalAlignment="Top" ItemsSource="{Binding Source={StaticResource UrunVerileri},XPath=Depo/Urun}" FontSize="14" FontWeight="Bold"> 
            <ComboBox.ItemTemplate> 
                <DataTemplate>
                    <TextBlock>
                        <Label>
                            <Label.Content>
                                <Binding XPath="Ad"/>
                            </Label.Content>
                        </Label>
                        <Button>
                            <Button.Content>
                                <Binding XPath="StokMiktari"/>
                            </Button.Content>
                        </Button>
                        <Image>
                            <Image.Source>
                                <Binding XPath="Durum"/>
                            </Image.Source>
                        </Image>
                        <Button Name="btnSiparisVer" Content="Sipariş Ver" FontSize="10" FontWeight="Bold" Background="Black" Foreground="Gold"/>
                    </TextBlock>
                </DataTemplate>
            </ComboBox.ItemTemplate> 
        </ComboBox> 
    </Grid>
</Window>
```

Her zamanki gibi Grid içerisindeki kontrollerin bağlanabileceği veri kaynağı sağlayıcısını XmlDataProvider yardımıyla Grid.Resources elementi içerisinde tanımlamaktayız. Bundan sonra ise ComboBox elementi altında bir ItemTemplate elementi açılmaktadır. Bu element içerisinde yer alan TextBlock elementi altına Label, Button, Image kontrolleri atılmıştır. Hepsinin ortak özelliği, hangi niteliklerine veri bağlayacaksak, bununla ilgili alt elementin açılması ve Binding elementi ile bağlama işleminin gerçekleştirilmesidir. Söz gelimi ürünün Ad elementinin değerini Label kontrolünün Content özelliğine atamak istiyorsak aşağıdaki gibi bir bildirim yapılması yeterlidir.

```xml
<Label>
    <Label.Content>
        <Binding XPath="Ad"/>
    </Label.Content>
</Label>
```

Burada Content elementinin değerinin, Ad isimli elementten alınacağı belirtilmiştir. Yanlız dikkat edilmesi gereken bir nokta vardır. Ad elementi aslında XML ağaç yapısına bakıldığında Depo/Urun üzerinden elde edilebilmektedir. Dolayısıyla burada XPath ifadesinde doğrudan Ad değerinin alınabilmesi için Urun elementlerine ulaşılmış olması gerekmektedir. Bunu sağlamak için ComboBox bileşeninin ItemsSource niteliğindeki XPath ifadesi Depo/Urun şeklinde ayarlanmıştır. Uygulamayı bu haliyle çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü ile karşılaşırız.

![mk221_8.gif](/assets/images/2007/mk221_8.gif)

Oldukça etkileyici değil mi? Bir ComboBox'ın her bir öğesi bir taşıyıcı (Container) gibi davranıp birden fazla farklı bileşeni içeriyor ve içerikleri bir XML veri kümesinden geliyor. Bence süper.

Şu ana kadar geliştirdiğimiz örneklerimizde XmlDataProvider tipinden yararlandık ve XML veri kümelerine bağlandık. XML dışındaki veri kaynakları göz önüne alındığında ObjectDataProvider bileşeninin kullanılması söz konusudur. Bu sınıf yardımıyla herhangibir.Net tipine bağlanmak mümkündür. MSDN bu konu ile ilişkili olarak çoğunlukla koleksiyonları örnekleyerek işe başlamaktadır. Bizde dilerseniz geleneği bozmayalım. Şimdiki örneğimizde Urun isimli bir sınıfa ait nesne örneklerini barındıran generic bir List koleksiyonunun veri kaynağı olarak kullanılmasını ele alıyor olacağız. İlk olarak aşağıdaki sınıf diagramında (class diagram) görülen Urun isimli tipi tasarlayarak başlayalım.

![mk221_9.gif](/assets/images/2007/mk221_9.gif)

```csharp
public class Urun
{
    public int Id;
    public string Ad;
    public double BirimFiyat;
    public int StokMiktari;
    public bool Durum;
    
    public override string ToString()
    {
        return Id.ToString() + " " + Ad+" "+BirimFiyat.ToString();
    }
}
```

Urun sınıfı yine bir ürünü tanımlayabilecek bazı public alanlara sahiptir. ToString metodunu ezmemizin (override) sebebi ise, ComboBox kontrolüne bağlandıklarında ne gösterileceğini belirtmektir. Eğer bunu belirtmessek, ComboBox kontrolünde IsimAlaniAdi.TipAdi (Namespace.TypeName) notasyonuna uygun olacak şekilde bir görüntü elde edilir. Gelelim ürünlere ait nesne örneklerini taşıyacak koleksiyon sınıfını tasarlamaya. UrunListesi isimli sınıfımız aşağıdaki gibidir.

![mk221_10.gif](/assets/images/2007/mk221_10.gif)

```csharp
public class UrunListesi:List<Urun>
{
    public UrunListesi()
    {
        Add(new Urun() { Id = 1, Ad = "Grafik Kartı", BirimFiyat = 35, StokMiktari = 100,Durum=true });
        Add(new Urun() { Id = 2, Ad = "Monitor", BirimFiyat = 150, StokMiktari = 50, Durum = false });
        Add(new Urun() { Id = 3, Ad = "CPU X86", BirimFiyat = 145, StokMiktari = 150, Durum = true });
        Add(new Urun() { Id = 4, Ad = "USB Bellek", BirimFiyat = 15, StokMiktari = 250, Durum = true });
        Add(new Urun() { Id = 5, Ad = "HDD 250 Gb", BirimFiyat = 250, StokMiktari = 14, Durum = false });
    }
}
```

UrunListesi sınıfı veri bağlanmasında kullanılacak kaynak bir tip olarak göz önüne alındığından List koleksiyonundan türetilmiştir. Daha önceki yazılarımızda da değinildiği gibi C# 3.0 ile gelen yeniliklerden birisi olan nesne başlatıcıları (Object Initializers) kullanılarak Urun nesneleri örneklenmiş bir List koleksiyonuna eklenmiştir. Şimdi UrunListesi sınıfını veri kaynağı olarak kullanacak bir pencereyi (Window) aşağıdaki gibi tasarlayabiliriz. Kahramanımız her zamanki gibi bir ComboBox bileşenidir.

![mk221_11.gif](/assets/images/2007/mk221_11.gif)

```xml
<Window x:Class="DataBindIslemleri.Window5" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Basit ObjectDataProvider Kullanımı" Height="158" Width="290" xmlns:dahili="clr-namespace:DataBindIslemleri">
    <Grid>
        <Grid.Resources>
            <ObjectDataProvider x:Key="UrunVerileri" ObjectType="{x:Type dahili:UrunListesi}" />
        </Grid.Resources>
        <ComboBox FontSize="12" FontWeight="SemiBold" Height="29" Margin="17,44,45,0" Name="comboBox1" VerticalAlignment="Top" ItemsSource="{Binding Source={StaticResource UrunVerileri}}" /> 
    </Grid>
</Window>
```

ObjectDataProvider tipide x:Key isimli bir niteliği kullanarak, veriye bağlanacak kontrollerin kullanabilmesi için ortak bir isim tanımlaması yapmaktadır. XmlDataProvider tipinde Source isimli nitelik ile veri kümesi belirtilirken ObjectDataProvider tipinde bu iş için ObjectType niteliği (attribute) kullanılmaktadır. ObjectType niteliğinde ise UrunListesi isimli bir tipin veri kaynağı olarak kullanılacağı ve bunun, dahili kelimesi ile ifade edilen isim alanında olduğu belirtilmektedir. Peki dahili XML isim alanı nereden tanımlanmıştır? Bunun için Window elementinde bir XML isim alanı (XML Namespace) tanımlaması aşağıdaki gibi yapılmıştır. Burada, clr-namespace ifadesini izeleyen ismin bir CLR isim alanı (Namespace) adı olduğu ve XAML dökümanı içerisinde dahili kısa adı ile ifade edileceği belirtilmektedir.

```xml
xmlns:dahili="clr-namespace:DataBindIslemleri"
```

Dikkat edilecek olursa, tipin içerisinde yer aldığı isim alanı (Namespace) işaret edilmektedir. Buradan şu sonucada varabiliriz. Veri bağlama amacıyla kullanılan tip farklı bir assembly içerisinde, dolayısıyla farklı bir isim alanında bulunuyorsada ilgili XAML içeriğinde kullanılabilir. Farklı bir assembly söz konusu olduğunda aşağıdakine benzer bir tanımlama yeterli olacaktır.

```xml
xmlns:dahili="clr-namespace:DataBindIslemleri,assembly=UrunLibrary"
```

Uygulamayı çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsünü elde ederiz.

![mk221_12.gif](/assets/images/2007/mk221_12.gif)

Dikkat edilecek olursa ToString metodu içeriği ComboBox kontrollerinde birer öğe olarak görülmektedir.

Veri bağlama işlemlerinde tip olarak DataTable veya DataSet gibi bağlantısız katman nesne örneklerinin kullanılması çok daha yaygındır. Bu tip bir senaryoda DataContext sınıfı ele alınmaktadır. Sıradaki örneğimizde bir DataTable içerisindeki veri kümesinin, WPF kontrollerine nasıl bağlanabileceğini incelemeye çalışacağız. Örnek bir senaryo olarak SQL Server 2005 ile birlikte gelen veritabanlarından birisi olan AdventureWorks ve Product, ProductPhoto, ProductProductPhoto tablolarını göz önüne alabiliriz. Bu tablolar arasındaki ilişki aşağıdaki şekilde olduğu gibidir.

![mk221_13.gif](/assets/images/2007/mk221_13.gif)

Amacımız resimli ürün bilgilerini bir ListBox kontrolünde gösterebilmek. Bu amaçla ilk olarak Window6 isimli penceremizin kodlarını aşağıdaki gibi geliştirelim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace DataBindIslemleri
{
    public partial class Window6 : Window
    {
        string sqlConn = "data source=.;database=AdventureWorks;integrated security=SSPI";
        string sorgu = @"SELECT PRD.ProductID, PRD.Name AS ProductName, PRD.SafetyStockLevel , PRD.StandardCost,PRD.ListPrice, PH.ThumbNailPhoto FROM Production.Product PRD INNER JOIN Production.ProductProductPhoto PPH ON PRD.ProductID = PPH.ProductID INNER JOIN Production.ProductPhoto PH ON PPH.ProductPhotoID = PH.ProductPhotoID"; 

        private void VeriyiCek()
        {
            DataTable dtUrunler = new DataTable();
            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                SqlDataAdapter da = new SqlDataAdapter(sorgu, conn);
                da.Fill(dtUrunler);
            }
            DataContext = dtUrunler;
        }

        public Window6()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstUrunler.Items.Clear();
            VeriyiCek(); 
        }
    }
}
```

Bu kod parçasında üzerinde durulacak olan tek nokta Window sınıfının FrameworkElement sınıfından kalıtımsal olarak devraldığı DataContext özelliğine DataTable nesne örneğinin atanmış olmasıdır. Böylece bir anlamda XAML içerisindeki bileşenlerin bağlanabileceği veri içeriği set edilmiş olur. Buna göre Window6.xaml dosyasının içeriğini aşağıdaki gibi tasarlayabiliriz.

```xml
<Window x:Class="DataBindIslemleri.Window6" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Window6" Height="273" Width="567" Loaded="Window_Loaded">
    <Grid>
        <Grid.Resources>
            <DataTemplate x:Key="Urunler"> 
                <StackPanel Orientation="Horizontal">
                    <Label Content="{Binding Path=ProductID}"/>
                    <Label Content="{Binding Path=ProductName}"/>
                    <Label Content="{Binding Path=ListPrice}"/>
                    <Label Content="{Binding Path=SafetyStockLevel}"/>
                    <Image Name="imgPhoto" Source="{Binding Path=ThumbNailPhoto}"/>
                </StackPanel>
            </DataTemplate>
        </Grid.Resources>
        <ListBox Margin="13,28,17,31" Name="lstUrunler" ItemsSource="{Binding}" ItemTemplate="{StaticResource Urunler}"/>
    </Grid>
</Window>
```

Şimdi burada neler yaptığımıza kısaca bakalım. Öncelikli olarak Grid.Resources elementi içerisinde bir DataTemplete oluşturulmaktadır. Bu veri şablonu, kullanıldığı yerde nasıl bir içerik sunulacağını belirlemektedir. Söz gelimi, Label kontrollerimizin Content özelliklerine yapılan atamalarda DataContext'in işaret ettiği veri kümesindeki alanlar belirlenmektedir. Diğer taraftan Image kontrolünün Source özelliğine yapılan atama ile ThumbNailPhoto alanındaki binary içeriğin bağlanması sağlanmıştır. Peki bu veri şablonunu kim kullanacaktır? Bunun için örnek olarak bir ListBox kontrolü ele alınmaktadır. Bu kontrolde ItemsSource özelliğine sadece Binding atanması, veri kaynağı olarak DataContext içeriğinin ele alınacağını göstermektedir. Diğer taraftan veri şablonunun set edildiği yer ItemTemplate özelliğine yapılan atamadır. Burada atamada Urunler isimli veri şablonunun (Data Template) kullanılacağı belirtilmektedir. Bu durumda uygulama çalıştırıldığında aşağıdakine benzer bir ekran görüntüsü ile karşılaşılır.

![mk221_15.gif](/assets/images/2007/mk221_15.gif)

Dikkat edilecek olursa ListBox içeriği, DataTable'a yüklenen veriler ile dolmuştur. Her halde buradaki en güzel nokta, ürün resimlerininde ListBox içerisinde gösterilebiliyor olmasıdır. Şimdi bu örneği biraz daha farklılaştıralım. Örneğin ListBox kontrolünde ürün adları gözüküyor olsun. Bunlardan herhangibiri seçildiğindeyse, ürün ile ilgili diğer bilgiler TextBox ve Image kontrollerinde görünüyor olsun. Bunun için yeni bir pencere (Window) ekleyip aşağıdaki gibi tasarlamamız yeterli olacaktır.

![mk221_17.gif](/assets/images/2007/mk221_17.gif)

```xml
<Window x:Class="DataBindIslemleri.Window7" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Window7" Height="304" Width="632">
    <Grid>
        <Grid.Resources>
            <DataTemplate x:Key="Urunler">
                <StackPanel Orientation="Horizontal">
                    <Label Content="{Binding Path=ProductName}"/>
                </StackPanel>
            </DataTemplate>
        </Grid.Resources>
        <ListBox Margin="13,19,0,18" Name="lstUrunler" ItemsSource="{Binding}" ItemTemplate="{StaticResource Urunler}" HorizontalAlignment="Left" Width="139" IsSynchronizedWithCurrentItem="True" />
        <Label Height="23" HorizontalAlignment="Left" Margin="176,24,0,0" Name="label1" VerticalAlignment="Top" Width="61">Ürün Adı</Label>
        <TextBox Text="{Binding Path=ProductName}" Height="21" Margin="270,24,191,0" Name="txtProductName" VerticalAlignment="Top"></TextBox>
        <Label Height="23" HorizontalAlignment="Left" Margin="176,50,0,0" Name="label2" VerticalAlignment="Top" Width="120">Birim Fiyatı</Label>
        <TextBox Text="{Binding Path=ListPrice}" Height="21" Margin="270,50,193,0" Name="txtListPrice" VerticalAlignment="Top" />
        <Label Height="23" HorizontalAlignment="Left" Margin="176,77,0,0" Name="label3" VerticalAlignment="Top" Width="120">Stok Seviyesi</Label>
        <TextBox Text="{Binding Path=SafetyStockLevel}" Margin="270,77,193,0" Name="txtSafetyStockLevel" Height="20" VerticalAlignment="Top" />
        <Image Source="{Binding Path=ThumbNailPhoto}" Margin="182,113,195,20" Name="imgPhoto" />
        <Label Content="{Binding Path=ProductID}" Height="49" HorizontalAlignment="Right" Margin="0,24,57,0" Name="lblProductID" VerticalAlignment="Top" Width="103" Foreground="Red" FontSize="16" FontWeight="Bold"/>
    </Grid>
</Window>
```

Bir önceki örnek ile karşılaştırıldığında önemli olan tek fark ListBox kontrolünün IsSynchronizedWithCurrentItem özelliğinin değerinin true olarak set edilmiş olmasıdır. Eğer bu özelliğe true değerini atamassak, ListBox üzerinde dolaştığımızda, bir başka deyişle başka bir öğeye geçtiğimizde diğer kontrollerin içerikleri DataContext nesnesinden dolmayacaktır. Bu durumda ListBox kontrolünde diğer öğelere tıklasakta hep ilk satırın bilgileri görünecektir. Uygulamamızı bu haliyle çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk221_16.gif](/assets/images/2007/mk221_16.gif)

Görüldüğü gibi örnek kaydın üzerine ListBox ile gidildiğinde, kontrollerin içerikleride o an DataContext'te üzerinde bulunulan satıra ait değerler olarak değişmiştir.

Bazı durumlarda birbirleriyle ilişkili olan tabloların kullanılmasıda söz konusudur. Özellikle bağlantısız katman (Disconnected Layer) nesneleri göz önüne alındığında bu tip vakkaları karşılamak için DataRelation nesnelerinden yararlanmaktayız. Peki DataRelation örnekleri ile aralarındaki ilişkiler (Relations) ifade edilen tabloları WPF uygulamalarındaki kontrollerimize nasıl bağlayabiliriz? Sıradaki örneğimizde bu durumu ele almaya çalışacağız. Söz gelimi, AdventureWorks veritabanında yer alan ProductSubCategory ve Product tablolarını baz alalım. Bu tablolar aşağıdaki diagramdanda görüleceği gibi birbirlerine ProductSubCategoryID alanları üzerinden bağlıdırlar.

![mk221_19.gif](/assets/images/2007/mk221_19.gif)

Bu ilişkiyi bağlantısız katmanda temsil edebilmek için DataSet, DataTable ve DataRelation nesnelerine ihtiyaç vardır. Window8 örneğinde, alt kategorilerin gösterildiği bir ComboBox kontrolü ve bu kategoriye bağlı ürünlerin gösterildiği bir ListBox kontrolü bulunmaktadır. Senaryomuza göre ComboBox kontrolünde değişiklik yapılması halinde, seçilen yeni alt kategoriye bağlı ürünlerinde ListBox kontrolünde gösterilmesi istenmektedir. Bu amaçla ilk olarak verinin çekilmesi ve pencerenin DataContext özelliğine gerekli DataTable nesne örneğinin atanması gerekmektedir. Bu sebepten Window8.xaml.cs dosyamızın içeriğini aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace DataBindIslemleri
{
    public partial class Window8 : Window
    {
        private string conStr = "data source=.;database=AdventureWorks;integrated security=SSPI";
        private string kategoriSorgusu = "Select ProductSubCategoryID,Name From Production.ProductSubCategory";
        private string urunSorgusu = "Select ProductID,ProductSubCategoryID,Name,ListPrice From Production.Product";

        private void VerileriCek()
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                SqlDataAdapter daKategori = new SqlDataAdapter(kategoriSorgusu, conn);
                DataTable dtKategori = new DataTable();
                daKategori.Fill(dtKategori);

                SqlDataAdapter daUrun = new SqlDataAdapter(urunSorgusu, conn);
                DataTable dtUrun = new DataTable();
                daUrun.Fill(dtUrun);

                DataSet ds = new DataSet();
                ds.Tables.Add(dtUrun);
                ds.Tables.Add(dtKategori);
        
                DataRelation iliski = new DataRelation("SubCatToProduct", dtKategori.Columns["ProductSubCategoryID"], dtUrun.Columns["ProductSubCategoryID"]);
                ds.Relations.Add(iliski);
        
                DataContext = dtKategori;
            }
        }
        public Window8()
        {
            InitializeComponent();
            VerileriCek();
        }
    }
}
```

Burada dikkat edilmesi gereken nokta, DataRelation nesne örneğinin mutlaka belirtilmesi ve DataSet nesne örneğinin Relations koleksiyonuna eklenmesi gerektiğidir. Pencerede kullanılacak olan ComboBox bileşeninin asıl bağlanacağı veri dtKategori tablosu olduğundan DataContext özelliğine bu tablonun değeri aktarılmıştır. Bundan sonra Window8 penceresinin XAML içeriği aşağıdaki gibi hazırlanabilir.

```xml
<Window x:Class="DataBindIslemleri.Window8" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Window8" Height="300" Width="440">
    <Grid>
        <Grid.Resources>
            <DataTemplate x:Key="KategoriVerisi">
                <StackPanel>
                    <TextBlock Text="{Binding Path=Name}"/>
                </StackPanel>
            </DataTemplate>
            <DataTemplate x:Key="UrunVerisi">
                <StackPanel Orientation="Horizontal">
                    <Label Content="{Binding Path=ProductID}"/>
                    <Label Content="{Binding Path=Name}"/>
                    <Label Content="{Binding Path=ListPrice}"/>
                </StackPanel>
            </DataTemplate>
        </Grid.Resources>
        <ComboBox Height="25" HorizontalAlignment="Left" Margin="12,35,0,0" Name="cmbAltKategori" VerticalAlignment="Top" Width="120" ItemsSource="{Binding}" ItemTemplate="{StaticResource KategoriVerisi}" IsSynchronizedWithCurrentItem="True"/>
        <Label Height="23" HorizontalAlignment="Left" Margin="10,12,0,0" Name="label1" VerticalAlignment="Top" Width="120">Alt Kategori</Label>
        <ListBox Margin="166,35,10,19" Name="lstUrunler" ItemsSource="{Binding SubCatToProduct}" ItemTemplate="{StaticResource UrunVerisi}" />
        <Label Height="23" Margin="166,12,132,0" Name="label2" VerticalAlignment="Top">Urunler</Label>
    </Grid>
</Window>
```

Grid içerisinde iki adet veri şablonu (Data Templete) kullanılmaktadır. Bunlardan birisi ComboBox, diğeri ise ListBox içindir. ComboBox bileşeni kendi içerisinde alt kategori adlarını göstermektedir. Veri kaynağını ItemsSource özelliği ile {Binding} olarak belirttiğimizden, DataContext içerisinden DataTable kullanılacaktır. Buna göre ItemTemplate'in ulaşacağı KategoriVerisi isimli DataTemplete elementinin içeriğine göre alt kategori adları görünecektir. Diğer tarafan, ComboBox üzerinde gezildikçe DataContext'in işaret ettiği veri kümesi üzerindede hareket edilebilmesini sağlamak istediğimizden IsSynchronizedWithCurrentItem niteliğine true değeri verilmesi şarttır.

ListBox kontrolünün ItemsSource özelliğine dikkat edilecek olursa Binding ifadesinden sonra, DataSet'e eklenen DataRelation nesne örneğinin adı yazılmıştır. İşte bu bizim örneğimizin kilit noktasıdır. Bir başka deyişle ListBox kontrolü, içeriğini oluştururken Binding ifadesinde belirtilen ilişki (Relation) üzerinden hareket edecek ve buna UrunVerisi isimli DataTemplete'e göre verilerini yükleyecektir. Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünde olduğu gibi alt kategori ve buna bağlı ürünlerin başarılı bir şekilde ilişkilendirildiği ve kontrollere bu değişimin yansıtıldığı görülür. Örnekte Shorts alt kategorisi seçilmiş ve buna bağlı olan ürünlerin bilgiside (DataTemplete ile çektiklerimiz) ListBox içerisinde gösterilmiştir.

![mk221_18.gif](/assets/images/2007/mk221_18.gif)

Buraya kadar geliştirdiğimiz örneklerimizde, WPF uygulamalarında yer alan kontrollerin çeşitli veri kaynaklarına (XML, Database, Object gibi) farklı şekillerde nasıl bağlanabileceklerini incelemeye çalıştık. Özellikle XML bazlı veri kaynakları için XmlDataProvider tipini, nesne (Object) bazlı kaynaklar için ObjectDataProvider tipini kullanmayı, bunlara ek olarak özellikle veritabanı bazlı gerçek bağlantılarda da DataContext tipinden yararlanmayı incelemeye çalıştık. Önceki Windows mantığına göre XAML getirdiği bir takım yenilikler ile, veri bağlama işleminin epey bir değiştiği sonucunuda çıkartmamız mümkün. Konuyla ilişkili detaylı bilgilere ve çok daha güzel örneklere MSDN'den ulaşabilirsiniz. Ayrıca bir önceki makalemizde tanıttığımız kitaplarında çok faydası olacağını söyleyebilirim. Böylece geldik bir makalemizin daha sonuna. İlerleyen makalelerimizde WPF ile uygulamalar geliştirmeye devam ediyor olacağız. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/DataBindIslemleri.zip)