---
layout: post
title: "Asp.Net 2.0 ve TreeView Kontrolü"
date: 2004-09-06 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - asp.net
  - treeview
---
Bu makalemizde, Asp.Net 2.0 ile birlikte gelen yeni kontrollerden birisi olan TreeView kontrolü ile, özellikle Xml tabanlı veri kaynaklarına ait bilgilerin, internet ortamında hiyerarşik bir yapıda nasıl gösterilebileceğini incelemeye çalışacağız. Bir önceki makalemizden hatırlayacağınız gibi, site içinde son kullanıcıya akıllı navigasyon hizmeti sunabilmek için sitemap dosyalarından faydalanabileceğimizi ve bu dosyalara ait Xml içeriğinin, SiteMapPath kontrolü ile sayfalarımızda gösterilebileceğinden bahsetmiş ve basit bir örnek geliştirmiştik. Benzer işlevselliğe TreeView kontrolü yardımıyla daha kuvvetli bir biçimde sahip olabiliriz.

TreeView kontrolü, Xml tabanlı veri kaynaklarını kullanır. Bu anlamda, hiyerarşik verilerin özellikle ağaç yapısı şeklinde ifade edilmesinde kullanılmaktadır. Şimdi bir önceki makalemizde geliştirdiğimiz örneğe TreeView kontrolünü ekleyerek, sitemap işleminin nasıl yapılacağını incelemeye çalışalım. Öncelikle, formumuza bir TreeView kontrolü ve birde SiteMapDataSource kontrolü ekleyeceğiz. TreeView kontrolünün önemli bir özelliği mutlaka bir veri kaynağına bağlanması gerekliliğidir.

Oysaki SiteMapPath kontrolü doğal olarak, site için oluşturulan sitemap dosyasına doğrudan bağlanabilmektedir. Fakat TreeView kontrolü Xml tabanlı verilere bağlanbilirliğinin gücünü yansıtabilmek için, DataSource kontrollerinden birisini kullanmak zorundadır. Bizim burada kullanacağımız SiteMapDataSource basit bir şekilde, sitenin sitemap dosyasına bağlanır ve Xml tabanlı içeriğin node seviyesinde, TreeView kontrolü ile ilişkilendirilmesini sağlar. İlk olarak bu kontrolleri geçen örneğimizdeki default.aspx sayfasına aşağıdaki tarzda yerleştirelim.

![mk88_1.gif](/assets/images/2004/mk88_1.gif)

Şekil 1. Form Tasarımımız.

Formumuzu yukarıdaki gibi tasarladıktan sonra tek yapmamız gereken, TreeView kontrolüne veri kaynağını bildirmek olacaktır. Burada veri kaynağımız SiteMapDataSource kontrolü olduğu için, TreeView kontrolünün DataSourceID özelliğine, bu kontrolün ID değerini atamamız yeterlidir. Bu atamayıda yaptığımız takdirde aspx kodlarımız aşağıdaki gibi olacaktır.

```text
<asp:TreeView ID="TreeView1" Runat="server" DataSourceID="SiteMapDataSource1"></asp:TreeView>
<asp:SiteMapDataSource ID="SiteMapDataSource1" Runat="server" />
```

Ayrıca, tasarım ekranımızda aşağıdaki görünümü alacaktır.

![mk88_2.gif](/assets/images/2004/mk88_2.gif)

Şekil 2. TreeView, SiteMapDataSource kontrolüne bağlandıktan sonra.

Görüldüğü gibi, sitemap dosyasının Xml içeriği, SiteMapDataSource kontrolü yardımıyla, TreeView kontrolüne bağlanmıştır. Böylece hiyerarşik yapının ağaç görünümü elde edilmiştir. Bu noktada, ağaç görünümünün temel yapıtaşı olan node kavramından bahsetmekte yarar olacağını düşünüyorum. TreeView kontrolünün ifade ettiği ağaç yapısında, "Azon Kitap Cd Dvd" elemanı, root node olarak adlandırılır. Root Node, başka alt node'lar içerebilen ve hiyerarşide en üst sırada yer alan node olarak değerlendirilir. Bununla birlikte bir TreeView kontrolü birden fazla Root Node içerebilmektedir. Root Node altında yer alabilecek 3 tip node daha vardır. Bunlar Parent, Child ve Leaf Node'larıdır. Bir Parent Node, Child Node'lar içerir ve root node altında yer alır. Diğer taraftan, herhangibir Child Node içermeyen node'lar Leaf Node olarak adlandırılır. Node'ların mantığını okuyarak anlamak her nekadar karışık görünsede, aşağıdaki şekil bu konuyu daha net bir şekilde açıklamaktadır.

![mk88_3.gif](/assets/images/2004/mk88_3.gif)

Şekil 3. Node Kavramı.

Tabi burada dikkat edecek olursanız, örneğin "CD ler" node'u Parent Node olmakla birlikte, Root Node'un Child Node'u olarakta değerlendirilir. Benzer şekilde, "Film CD'leri" node'u "CD ler" node'unun Child Node'u olmakla birlikte, başka alt node'lar içermediği için aynı zamanda bir Leaf Node'dur. Node'lar ile ilgili bu kısa açıklamalardan sonra, uygulamamızı geliştirmeye devam edelim. TreeView kontrolü, kullanıcıya görsel açıdan çeşitli imkanlarıda beraberinde getirmektedir. Örneğin MSDN'de kullanılan ağaç yapısının formatını TreeView kontrolünüze uygulayabilirsiniz. Ya da, aşağıdaki ekran görüntüsünü veren, News formatınıda kullanabilirsiniz. Bunun için tek yapmanız gereken, TreeView kontrolünün Auto Format özelliklerini, önceden tanımlanmış formatlardan birini seçerek tamamlamaktır.

![mk88_4.gif](/assets/images/2004/mk88_4.gif)

Şekil 4. Önceden tanımlanmış News formatının TreeView kontrolüne uygulanmasının sonucu.

Dilerseniz sayfamızı çalıştıralım ve TreeView kontrolünün bize nasıl bir imkan sağladığına bakalım. Sayfamızı çalıştırdığımızda tarayıcı penceresinde TreeView kontrolünün, sitemap dosyasının içeriğini, ağaç yapısı şeklinde aşağıdaki şekilde olduğu gibi gösterdiğini görürüz.

![mk88_5.gif](/assets/images/2004/mk88_5.gif)

Şekil 5. Sayfamız çalıştığında.

İşin en güzel yanı, node'lardan birisine tıkladığımızda, sitemap dosyasında o node için belirtilen link'e başka bir deyişle sayfaya gidebilmemizdir. Örneğin, Muzik Cd'leri sayfasına tıkladığımızda, MuzikCd.aspx sayfasına gideriz. Dolayısıyla TreeView kontrolünü, onu sitemap dosyasına bağlayan SiteMapDataSource kontrolü ile birlikte bir kullanıcı tanımlı kontrol olarak geliştirmemiz halinde, sitemizin tüm sayfalarında son derece etkin ve şık bir navigasyon sistemine sahip olabiliriz.

TreeView kontrolünün bu kullanımı dışında daha pek çok tarz uygulanışı vardır. Öncedende söylediğimiz gibi, bir TreeView kontrolü mutlaka suretle bir veri kümesine bağlanır. Az önceki örneğimizde, bu veri kaynağı, sitemap dosyamızdı ve buraya bağlanmak için SiteMapDataSource kontrolünü kullanmıştık. Dilersek bir TreeView kontrolünü,bir Xml dosyasınada bağlayabiliriz. Örneğin aşağıdaki Xml dosyasını göz önüne alalım. (Bu dosyayı, Veriler.xml adıyla Solution'ımıza ekledik.)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Manav>
    <Meyve Kategori="Elma">
        <Miktar Deger="1 Kilo"></Miktar>
        <Miktar Deger="2 Kilo"></Miktar>
        <Miktar Deger="3 Kilo"></Miktar>
    </Meyve>
    <Meyve Kategori="Kiraz">
        <Miktar Deger="1 Kilo"></Miktar>
        <Miktar Deger="2 Kilo"></Miktar>
    </Meyve>
    <Meyve Kategori="Karpuz">
        <Miktar Deger="5 Kilo"></Miktar>
        <Miktar Deger="5 - 10 Kilo"></Miktar>
        <Miktar Deger="2 - 5 Kilo"></Miktar>
    </Meyve>
    <Meyve Kategori="Limon">
        <Miktar Deger="3 Tane"></Miktar>
        <Miktar Deger="10 Tane"></Miktar>
        <Miktar Deger="10-> Tane"></Miktar>
    </Meyve>
</Manav>
```

Bu Xml dosyasını,TreeView kontrolümüze bağlayabilmemiz için, bir XmlDataSource kontrolünü kullanmamız gerekiyor. XmlDataSource kontrolümüzün, bu Xml dosyasına işaret edebilmesi için tek yapmamız gereken, DataFile özelliğine ilgili Xml dosyasının adresini belirtmek olacaktır. Bunun için aspx kodlarımızı aşağıdaki gibi geliştirmemiz yeterli olacaktır.

```text
<asp:XmlDataSource ID="XmlDataSource1" Runat="server" DataFile="Veriler.xml">
</asp:XmlDataSource>
```

Şimdi, bu XmlDataSource nesnesini kullarak, veriler.xml dosyasına bağlanacak ve içeriğini gösterecek bir TreeView kontrolünü sayfamıza ekleyelim. Bu kez, SiteMapDataSource kontrolünde olduğu gibi, TreeView kontrolünün DataSourceID özelliğine, XmlDataSource kontrolünün ID değerini atamamız tek başına yeterli olmayacaktır. Nitekim, Xml dosyasındaki node'ları, TreeView kontrolüne bind etmemiz gerekmektedir. Bunun için, TreeView kontrolünün takısı kullanılır. Bu takı altında, her bir node için gerekli olan TreeNodeBinding aspx nesneleri tanımlanmalıdır. Bu bilgileri göz önüne alırsak, TreeView kontrolümüzü ve XmlDataSource kontrolümüzü aşağıdaki aspx kodları ile oluşturmamız gerekmektedir.

```text
<asp:TreeView ID="trvManav" Runat="server" DataSourceID="XmlDataSource1">
    <DataBindings>
        <asp:TreeNodeBinding DataMember="Manav" Text="Manav" Value="Manav"></asp:TreeNodeBinding>
        <asp:TreeNodeBinding DataMember="Meyve" TextField="Kategori"></asp:TreeNodeBinding>
        <asp:TreeNodeBinding DataMember="Miktar" TextField="Deger"></asp:TreeNodeBinding>
    </DataBindings>
</asp:TreeView>

<asp:XmlDataSource ID="XmlDataSource1" Runat="server" DataFile="Veriler.xml">
</asp:XmlDataSource>
```

Burada bizim için önemli olan özellikler, TreeNodeBinding kontrolüne ait olan, DataMember ve TextField özellikleridir. Şimdi, TreeView kontrolümüze yine otomatik formatlardan birisini (örneğin MSDN) uygulayalım ve sayfamızı tarayıcıda açalım.

![mk88_6.gif](/assets/images/2004/mk88_6.gif)

Şekil 6. TreeView kontrolünü Xml dosyasına, XmlDataSource kontrolü ile bağladığımızda.

Elbette, bu TreeView yapısını daha etkin bir şekilde kullanıma sunabiliriz. Örneğin, kullanıcı burada yer alan Leaf Node'lardan seçim yapabilir. Bunun için, TreeView kontrolünün, Leaf Node'larına CheckBox koymamızı sağlayacak bir özelliği vardır.

![mk88_7.gif](/assets/images/2004/mk88_7.gif)

Şekil 7. ShowCheckBoxes özelliği.

ShowCheckBoxes özelliği ile, TreeView kontrolünün hangi node'larında CheckBox gösterileceğini belirtebiliriz. Biz bu örneğimizde leaf seçeneğini işaretleyelim. Böylece, kullanıcının hangi meyveden kaç kilo (adet) almak isteyeceğini işaretleyebileceği kutucuklarımız olacaktır. Kullanıcının, işaretlediği kutucukları elde edebilmek ve bir label kontrolünde gösterebilmek amacıylada, örneğimizi aşağıdaki şekilde olduğu gibi geliştirelim.

![mk88_8.gif](/assets/images/2004/mk88_8.gif)

Şekil 8. Formun son hali.

Kullanıcı, sayfada "Gözden Geçir" başlıklı button kontrolüne tıkladığında, TreeView kontrolüne işaretlemiş olduğu miktarlar ve bu miktarların yer aldığı Parent Node'lar lblSecilenler isimli Label kontrolüne yazdırılacak. Bu işlemi gerçekleştirmek için, Button'umuzun Click olayına aşağıdaki kodları yazmamız yeterli olacaktır.

```csharp
void btnGozdenGecir_Click(object sender, EventArgs e)
{
    string secilenler="";
    if (trvManav.CheckedNodes.Count > 0)
    {
        foreach (TreeNode tn in trvManav.CheckedNodes)
        {
            secilenler += tn.Parent.Text+" "+tn.Text+"|";
        }

        lblSecilenler.Text = secilenler;
    }
    else
    {
        lblSecilenler.Text = " Manavdan bir şey almadınız...";
    }
}
```

TreeView kontrolünün CheckedNodes özelliği TreeNodeCollections koleksiyonu tipinden değer döndürmektedir ve işaretlenmiş olan node'lar kümesini temsil etmektedir. Dolayısıyla if kontrolünde, Count özelliği ile, kullanıcının herhangibir node'u işaretleyip işaretlemediği, başka bir deyişle CheckBox kutucuklarını seçip seçmediğine bakılmaktadır. Eğer seçilmiş node'lar var ise, bu node'lar arasında gezinmek için, foreach döngüsü kullanılarak CheckNodes koleksiyonundaki her bir TreeNode elemanı ele alınır. TreeNode elemanları bu döngüde, Leaf Node'larımızdan işaretlenmiş olanlarına işaret etmektedir. Dolayısıyla, o anki TreeNode nesnesinin Parent özelliği, Parent Node'u işaret eder. Uygulamamızı çalıştırıp aşağıdaki gibi bir kaç seçim yaptığımız takdirde, Label kontrolümüzün seçili olan Leaf Node'lar ve bu node'ların Parent Node'ları ile doldurulduğunu görürüz.

![mk88_9.gif](/assets/images/2004/mk88_9.gif)

Şekil 9. Sayfamız çalıştığında.

Böylece geldik bir makalemizin daha sonuna. İlerleyen makalelerimizde görüşmek dileğiyle hepinize mutlu günler dilerim.