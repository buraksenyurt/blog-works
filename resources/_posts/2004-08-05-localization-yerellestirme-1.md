---
layout: post
title: "Localization (Yerelleştirme) - 1"
date: 2004-08-05 09:00:00 +0300
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - threading
  - visual-studio
---
Dünya çapında yada başka bir deyişle global çapta uygulamalar geliştirilirken karşılaşılabilecek zorluklardan birisi, uygulamanın farklı kültür ve dil seçeneklerine göre çalışabilecek şekilde tasarlanmasıdır. Eski programlama dilleri göz önüne alındığında, özellikle farklı dil desteği sağlayacak uygulamaların geliştirilmesi tam anlamıyla bir kabus olmuştur. Söz gelimi 2 dile destek verecek bir uygulama geliştirilmek istendiğinde, her iki dil içinde farklı uygulama kodları yazılması gerekirdi. Böyle bir durumda, uygulamanın piyasaya sürülmesinden sonra yapılacak güncelleme paketleri içinde aynı durum söz konusuydu. Dahası, 3ncü bir dilin desteğinin sağlanması için, aynı projenin bu kezde bu dil için geliştirilmesi gerekmekteydi. Dil çeşitliliğinin yanı sıra, aynı dili konuşan fakat farklı takvimler, farklı parasal formatlar, farklı sayısal formatlar hatta farklı sıralamalar kullanan kültürler işin içine sokulduğunda durum tam anlamıyla bir paradoks haline gelmektedir.

Peki Microsoft.Net ile bu soruna nasıl bir çözüm getirmektedir? İşte bu günkü makalemizin konusu budur. Microsoft.Net platformu içinde geliştirdiğimiz çözümlerin (solutions), çeşitli dillere veya kültürlere destek verebilmesini sağlamak amacıyla, öncekilerine nazaran çok daha mantıklı ve verimli bir teknik uygulanır. Bu teknikte, uygulama kodu bir kez yazılır. Bu uygulamanın dil desteği ve kültür desteği vermesi için ise, framework içinde yer alan System.Globalization isim alanındaki sınıflardan ve resource files dediğimiz kaynak dosyalarından yararlanılır. Bir başka deyişle, bir uygulamanın yerelleştirilmesi yani dil desteği ve kültürel desteğin sağlaması için, uygulama kodları ile destek birimleri birbirinden ayrı tutulmuştur. Böylece, bir uygulamayı n sayıda kültür için yerelleştirmek mümkün olmaktadır.

Bu teorik bilgiler ilk okunduğunda akla karmaşık gelebilir. Olayı daha iyi anlayabilmek için, yeryüzünde yer alan toplumların birbirlerinden farklılaşmasına neden olan kültürel özellikleri keşfetmek gerekmektedir. Dünyamız konuşulan ana dillere göre çeşitli bölgelere ayrılmıştır. Bu diller, bir toplumun doğal kültürünü (Neutral Culture) temsil eder. Yada başka bir deyişle doğal kültürler dillere göre kategorize edilir. Örneğin İngilizce, Almanca, Fransızca, İtalyanca dilleri doğal kültürleri (Neutral Cultures) yansıtmaktadır. Bununla birlikte, çeşitli dillere destek verecek çevirisel uygulamalarda, doğal kültürlerin bilinmesi yeterli iken, parasal işlemlerin, sayısal işlemlerin, tarihsel işlemlerin, sıralamaların vb... yer aldığı daha belirleyici yerelleştirme işlemlerinde, dile bağlı doğal kültür bilgileri yeterli değildir. Örneğin, İngilizce konuşan pek çok alt kültür vardır. İngiltere, Amerika, Kanada, Avusturalya vb... Bu ülkelerin her biri İngilizce konuşmakla birlikte, farklı kültür özelliklerine sahip olabilirler. Örneğin kullandıkları para birimi farklılığı, zip kodların farklılığı, sayısal ifadelerdeki nokta ve virgül kullanımlarının farklılıkları vb... Dolayısıyla bir uygulamayı, belirli bir kültür için özelleştirecek isek, Specific Culture denilen alt kültür bilgilerinden yararlanmamız gerekmektedir.

Tabiki kültür bilgilerinin belirli bir standart çerçevesinde ifade edilmesi ve küresel çapta kabul görmesi oldukça önemlidir. Microsof bunun için, IETF tarafından belirlenmiş RFC 1766 standardını kullanır. Bu standarda göre, her doğal kültür ve bu doğal kültüre bağlı alt kültürler ikişer karakterden oluşan kısaltma kodlar ile ifade edilirler. Tabiki bir doğal kültür birden fazla alt kültüre ve bu alt kültürlerde başka alt kültürlere sahip olabilirler. Konuyu aşağıdaki örnek şekil daha iyi ifade edecektir.

![mk81_1.gif](/assets/images/2004/mk81_1.gif)

Şekil 1. Fransızca nın alt kültürleri.

Örneğin Fransıca dili bir doğal kültür olarak ele alındığında, dünya üzerinde Fransızca'yı konuşan bölgelerde bu kültürün alt kültürlerini oluşturur. fr-FR Fransa'yı, fr-BE Belçikayı fr-LU Lüksemburg'u temsil eden belirleyici kültür kodlarıdır. Peki uygulamalarımızda bu kültür kodlarını nasıl kullanabiliriz? Bunun için System.Globalization isim alanındaki CultureInfo ve RegionInfo sınıflarını kullanabiliriz. CultureInfo sınıfı yardımıyla belilri bir kültüre ait bilgilere sahip oluruz. RegionInfo sınıfı ilede, belirli bir bölgeye ait bilgilere ulaşırız. Örneğin para birimi ve metrik sistemin kullanılıp kullanılmadığı gibi bilgiler RegionInfo sınıfını ilgilendirirken, takvim bilgisi, doğal ad gibi bilgilerde CultureInfo sınıfını ilgilendirir. İşin özel yanı, bir uygulamanın kullanıcı arayüzündeki yerel kriterleri değiştirmek istediğimizde, uygulamanın çalıştığı thread nesnesine ait metodları kullanamamızın yeterli olacağıdır. Tabiki burada bahsi geçen proses, belirtilen bir kültür bilgisine yani CultureInfo nesnesine göre ayarlandığı takdirde, programın yerelleştirilmesi gerçekleştirilmiş olacaktır.

Şimdi bu bahsettiklerimizi daha iyi anlayabileceğimiz basit bir uygulama geliştirmeye çalışlım. Bu uygulamamızda, kabul gören standartlar içindeki tüm kültürlere ait kodları listeleyeceğimiz bir treeView kontrolü kullacağız. Her hangibir kültür seçildiğinde bu kültüre ait bilgiler ekrandaki kontrollere dolacak. Ayrıca, tarih, saat ve parasal bilgi veren textBox'larımızda örnek değerler tutacak ve bu bilgilerin seçilen kültürün belirttiği özelliklere göre gösterilmesi sağlanacak. Öncelikle Vs.Net içinde aşağıdaki forma görüntüsüne sahip olan bir windows uygulaması oluşturalım.

![mk81_2.gif](/assets/images/2004/mk81_2.gif)

Şekil 2. Form tasarımımız.

Gelelim uygulama kodlarımıza. Her şeyden önce System.Globalization isim alanını uygulamamıza using anahtar kelimesi ile eklemeliyiz. Nitekim kullacanağımız CultureInfo ve RegionInfo sınıfları bu isim alanı içinde yer almaktadır. Kulturler başlıklı butona bastığımızda, Microsoft'un kullandığı standart içindeki tüm kültür kodlarının listesini almak ve bunları treeView kontrolünde ağaç yapısı şeklinde hiyerarşik bir düzende göstermek istiyoruz. İşte Kulturler başlıklı butona ait kodlarımız.

```csharp
private void btnKulturler_Click(object sender, System.EventArgs e)
{
    CultureInfo[] kulturDizi=CultureInfo.GetCultures(CultureTypes.AllCultures);

    TreeNode[] nodes=new TreeNode[kulturDizi.Length];

    int i=0;
    TreeNode parent=null;

    foreach(CultureInfo kulturBilgi in kulturDizi)
    {
        nodes[i]=new TreeNode();
        nodes[i].Text=kulturBilgi.DisplayName;
        nodes[i].Tag=kulturBilgi;

        if(kulturBilgi.IsNeutralCulture)
        {
            parent=nodes[i];
            treeView1.Nodes.Add(nodes[i]);
        }
        else if(kulturBilgi.ThreeLetterISOLanguageName==CultureInfo.InvariantCulture. ThreeLetterISOLanguageName)
        {
            treeView1.Nodes.Add(nodes[i]);
        }
        else
        {
            parent.Nodes.Add(nodes[i]);
        }
    }
}
```

Öncelike, var olan kültürleri elde etmek için CultureInfo sınıfına ait GetCultures metodunu kullandık. Bu metoda parametre olarak, CultureTypes.AllCultures değerini verdik. Metodumuz bu parametre değeri dışında aşağıdaki değerleride alabilmektedir.

CultureTypes Numaralandırıcı Değeri
Açıklama

AllCultures
Tüm kültürleri listeler.

InstalledWin32Cultures
Windows sistemlerinde kullanılan kültürleri listeler.

NeutralCultures
Sadece dille ilişkilendirilmiş doğal kültürleri listeler.

SpecificCultures
Belirleyici kültürleri listeler.

Bundan sonraki kod satılarında treeView kontrolümüzü doldurmak için gerekli işlemler yapılmıştır. Dikkat etmemiz gereken noktalar, foreach döngüsü içindeki if koşullarıdır. Buradaki ilk koşulda CultureInfo nesnesinin doğal bir kültür olup olmadığına bakılır. Diğer koşulda ise kültürün invariant culture olup olmadığı koşulu değerlendirilir. Invariant kultürler, gerçek kültürlerden bağımsız olan yapılardır. Şimdi uygulamamızın diğer kodlarını oluşturalım.

```csharp
private void Temizle()
{
    for(int i=0;i<this.Controls.Count;i++)
    {
        if(this.Controls[i] is TextBox)
        {
            this.Controls[i].Text="";
        }
    }
}
private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
{

    Temizle();
    CultureInfo guncelKultur=(CultureInfo)treeView1.SelectedNode.Tag;
    txtName.Text=guncelKultur.Name;
    txtNativeName.Text=guncelKultur.NativeName;
    txtEnglishName.Text=guncelKultur.EnglishName;

    if(!guncelKultur.IsNeutralCulture)
    {
        RegionInfo bolgeBilgisi=new RegionInfo(guncelKultur.LCID);
        txtCurrency.Text=bolgeBilgisi.CurrencySymbol;
        txtRegion.Text=bolgeBilgisi.DisplayName;
        txtMetric.Text=bolgeBilgisi.IsMetric.ToString();

        Thread.CurrentThread.CurrentCulture=guncelKultur;
        double sayi=4587512.451;
        txtNumber.Text=sayi.ToString("N");

        txtDate.Text=DateTime.Today.ToString("D");

        txtTime.Text=DateTime.Now.ToString("T");
    } 
}
```

TreeView kontrolünde her hangibir öğe seçildiğinde bu öğeye ait bilgiler, textBox kontrollerine doldurulur. Eğer seçilen öğe doğal bir kültür değilse, yani belirleyici kültür ise, bu kültürün bulunduğu ülke veya bölgeye ait bilgilere ulaşmak için RegionInfo sınıfına ait bir nesne kullanılır. Bu nesnenin berlileyici bir kültüre ait olarak oluşturulması sırasında, belirleyici kültürü temsil eden bir değer kullanılır. Bu değer, CultureInfo sınıfının LCID özelliği ile elde edilen benzersiz bir belirleyicidir. RegionInfo nesnesi yardımıyla, belirleyici kültürün bulunduğu bölgeye has para birimi, bu bölgede metrik sistemin kullanılıp kullanılmadığına dair belirleyici bilgiler elde edilir. Gelelim işin en önemli kısmına. Yani uygulamanın seçilen kültüre göre yerelleştirildiği (Localization) kısma.

```csharp
Thread.CurrentThread.CurrentCulture=guncelKultur;
```

Bu kod satırı ile, çalışan proses için kültür değeri, seçilen kültüre göre ayarlanmıştır. Dolayısıyla bu kod satırını izleyen satırlardaki sayısal değer, tarih ve zaman formatları, seçilen kültüre göre ekrana gelecektir. İşte uygulamamız basit bir şekilde yerelleştirme işlemini gerçekleştirmiştir. Şimdi uygulamamızı çalıştıralım ve sonuçları gözlemleyelim. Programı ilk çalıştırdığımızda ve Kulturler başlıklı butona tıkladığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk81_3.gif](/assets/images/2004/mk81_3.gif)

Şekil 3. Tüm Kültürler.

Örneğin German doğal kültürünü seçelim. Bu durumda Almanca dilini konuşan alt kültürler görünecek ve Almanca doğal kültürüne ait kültür kodu, doğal adı ve ingilizce ad bilgileri elde edilecektir.

![mk81_4.gif](/assets/images/2004/mk81_4.gif)

Şekil 4. Almacan (German) Doğal Kültürü.

Şimdi belirleyici kültürlerden birini seçelim. Örneğin Avusturya alt kültürünü. Bu durumda bu kültüre ait yerel bilgiler ekrana gelecek ve uygulama bu noktadan sonra, Avusturya'ya ait yerel bilgilere göre sayıları formatlayacak, tarihleri gösterecek vb... işlemleri gerçekleştirecektir.

![mk81_5.gif](/assets/images/2004/mk81_5.gif)

Şekil 5. Avusturya Kültürüne ait özellikler.

Şimdi daha uç bir örnek gösterelim. Örneğin uygulamamızın Suudi Arabistan kültürüne göre yerelleştirilmek istendiğini farzedelim. Bu durumda ilk akla gelen arap takviminin farklılığıdır.

![mk81_6.gif](/assets/images/2004/mk81_6.gif)

Şekil 6. Arabistanda durum dahada karışık.

Böylece bir uygulamanın, belirli bir kültüre göre yerelleştirilmesini görmüş olduk. Diğer taraftan, uygulamamızın farklı dillere göre destek vermesinide isteyebiliriz. İşte bu noktada devreye Resource Files (Kaynak Dosyalar) girmektedir. Bu konuyu ise bir sonraki makalemizde incelemeye çalışacağız. Hepinize mutlu günler dilerim.