---
layout: post
title: "Asp.Net 2.0 için Site Map Kullanımı"
date: 2004-09-03 06:00:00 +0300
categories:
  - aspnet
tags:
  - aspnet
  - csharp
  - dotnet
  - xml
  - visual-studio
---
Bu makalemizde, web sitelerinde özellikle sayfalar arasındaki hareketlerde, kullanıcıların nerede olduklarını bilmelerine yardımcı olan site haritaları üzerinde duracağız. Bununla birlikte, site haritalarının Asp.Net 2.0' daki kullanım yollarından birisini sağlayan SiteMapPath sunucu kontrolünü kısaca incelemeye çalışacağız.

Internet sitelerinin çoğu, pek çok web sayfasından oluşur. Bu sayfalar arasında her zaman için bir ilişki ve hiyerarşi söz konusudur. Dolayısıyla kullanıcılar, internet sitelerine ait sayfalarda gezinirken, onlara sitenin neresinde olduklarını göstermek ve bulundukları sayfaya nereden geldiklerini belirtmek, sitenin mantıksal ve anlamsam bütünlüğü açısından oldukça önemlidir.

Bu tarz bir kolaylığı kullanıcılara sağlayabilmek amacıyla çeşitli yollar kullanabiliriz. Kullanıcı tanımlı kontroller geliştirebilir, programatik olarak geliştirimiş teknikler uygulayabiliriz. Ancak Asp.net 2.0, bu tip navigasyon izleme işlemleri için, Xml tabanlı, esnek ve daha pratik bir yol sunmaktadır. Bu teknikteki anahtar noktalar, Site Map dosyası ile, SiteMapPath sunucu kontrolüdür. Olayı daha iyi anlayabilmek amacıyla aşağıdaki hiyerarşik düzene sahip bir internet sitemizin olduğunu varsayalım.

![mk87_1.gif](/assets/images/2004/mk87_1.gif)

Şekil 1. Site Haritamız.

Burada görüldüğü gibi ana sayfamız dallanarak çeşitli alt sayfalara doğru ilerlenmektedir. Şimdi, Film Dvdsi sayfamızda olduğumuzu düşünelim. Burada kullanıcıya, nerede olduğu göstermek, son kullanıcı açısından oldukça değerli bir bilgidir. Heleki web sitemiz yüzlerce sayfa içeriyoru ve sayfaların çoğu 3 yada daha fazla alt sayfaya mantıki olarak bağlanıyorsa çok daha önemlidir. Ayrıca, kullanıcının bu sayfaya geldiği sayfalarada gitmesini kolayca sağlayacak linklerin olmasıda tercih nedenidir. İşte bu tip işlemleri gerçekleştirmek için, şekilsel olarak ifade ettiğimiz bu haritayı, Xml ortamına taşımamız gerekmektedir. Bu iş için kullanılan Site Map tipindeki dosyalar, Asp.Net 2.0 için geliştirilmiş özel nitelikli dosyalar olup tamamıyla, site haritalama işlemlerine hizmet etmektedir. Şimdi öncelikli olarak yukarıdaki hiyerarşiye sahip sitemizi Visual Studio.Net 2005 ortamında oluşturalım. Her sayfamızı aşağıdaki isimler ile Solution'ımıza ekleyelim.

![mk87_2.gif](/assets/images/2004/mk87_2.gif)

Şekil 2. Azon internet sitemizin sayfaları.

Sıradaki işlemimiz web.sitemap dosyasını oluşturmak olacaktır. Bunun için Solution Explorer'da sağ tıklayıp Add New Item'i seçtikten sonra aşağıdaki şekilde görüldüğü gibi Site Map dosya tipini işaretlemeliyiz.

![mk87_3.gif](/assets/images/2004/mk87_3.gif)

Şekil 3. Site Map dosyamızı oluşturuyoruz.

Gelelim dosyamızın içeriğine. Bu dosya Xml tabanlı bir dosya olup, siteMapNode takılarından oluşmaktadır. SiteMap takısı altında yer alan, siteMapNode takıları, site haritasındaki sayfaları çeşitli özellikleri ile birlikte belirtmektedir. Harita hiyerarşisine göre derinlerdeki sayfalara inilmek istendiğinde iç içe geçen siteMapNode takıları kullanılmalıdır. siteMapNode takıları temel olarak, url, description, title olmak üzere 3 önemli özellik içerir. Title ile sayfanın başlığı, description ile sayfaya ait açıklama ve url özelliği ilede sayfanın adresi belirtilmektedir. Şimdi, oluşturduğumuz web.sitemap dosyasına aşağıdaki Xml içeriğini yazalım.

```text
<?xml version="1.0" encoding="utf-8" ?>
<siteMap>

    <siteMapNode url="default.aspx" title="Azon Kitap Cd Dvd" description="En iyi Kitaplar, Cdler, Dvdler bu siteden alınır.">
        <siteMapNode url="kitap.aspx" title="Kitaplar" description="Bilgisayar, hukuk ve tıp kitapları">
            <siteMapNode url="IngilizceBilgisayar.aspx" title="Ingilizce Bilgisayar Kitapları" description="Ingilizce Bilgisayar Kitapları. Onlardan iyisi yok."/>
            <siteMapNode url="TurkceBilgisayar.aspx" title="Türkçe Bilgisayar Kitapları" description="Türkçe Bilgisayar Kitapları Zengin Arşivimiz ile Karşınızda."/>
            <siteMapNode url="Hukuk.aspx" title="Hukuk Kitapları" description="Hukuğu öğrenebileceğiniz en iyi kitaplar."/>
            <siteMapNode url="Tip.aspx" title="Tıp Kitapları" description="Tıp şakaya gelmez. En iyi tıp kitaplarını bizden alın."/>
    </siteMapNode>

        <siteMapNode url="cd.aspx" title="CD ler" description="Film ve Müzik Cd' leri.">
            <siteMapNode url="MuzikCd.aspx" title="Müzik Cd' leri." description="Harika gruplar, harika albümler."/>
            <siteMapNode url="FilmCd.aspx" title="Film Cd' leri." description="En güzel filmler."/>
        </siteMapNode>

        <siteMapNode url="dvd.aspx" title="DVD ler" description="Film ve Müzik DVD' leri.">
            <siteMapNode url="MuzikDvd.aspx" title="Müzik DVD' leri." description="Harika gruplar, harika albümler."/>
            <siteMapNode url="FilmDvd.aspx" title="Film DVD' leri." description="En güzel filmler çoklu dil destekleri ve mükemmel görünt kaliteleriye Dvdlerde."/>
        </siteMapNode> 

        <siteMapNode url="sitehakkinda.aspx" title="Site Hakkında" description="Kimiz ve size ne sağlıyoruz."/>

    </siteMapNode>

</siteMap>
```

Artık elimizde site haritamızın xml içeriği mevcuttur. Şimdi bu Xml içeriğini kullanacak olan sunucu kontrolümüze bakalım. SiteMapPath sunucu kontrolü, bu xml içeriğine bakarak site haritasını değerlendirir ve son kullanıcıya akıllı navigasyon izleme yeteneğini sağlar.

![mk87_4.gif](/assets/images/2004/mk87_4.gif)

Şekil 4. SiteMapPath sunucu kontrolü.

Şimdi, toolbox'taki navigation sekmesinde yer alan SiteMapPath sunucu kontrollerini örnek olarak TürkceBilgisayar.aspx sayfasına bırakalım.

![mk87_5.gif](/assets/images/2004/mk87_5.gif)

Şekil 5. Sunucu kontrolünün sayfaya eklenmesi sonrası.

Görüldüğü gibi, site haritamızdaki hiyarerşik düzene göre bu sayfaya hangi sayfalardan gelindiği açıkça görülmektedir. Eğer bu noktada, TurkceBilgisayar.aspx sayfasını çalıştıracak olursak, "Kitaplar" ve "Azon Kitap Cd Dvd" linkleri ile, site haritasındaki üst sayfalara çıkılabileceğimizide görürüz. Dilersek, SiteMapPath sunucu kontrolüne hazır görsel formatlardan birisinide uygulayarak makyajlayabiliriz. Bunun için kontrolün sağında çıkan ok işaretinden sonra yer alan Auto Format seçeneğine basmamız ve herhangibir formatı (örneğin Colorful) seçmemiz yeterlidir.

![mk87_6.gif](/assets/images/2004/mk87_6.gif)

Şekil 6. Auto Format seçeneği.

![mk87_7.gif](/assets/images/2004/mk87_7.gif)

Şekil 7. Colorful seçeneği.

Bu makyajlama işlemi sonrası, SiteMapPath kontrolünün aspx sayfasındaki içeriği aşağıdaki gibi olacaktır.

```csharp
<asp:SiteMapPath ID="SiteMapPath1" Runat="server" Font-Size="0.8em" Font-Names="Verdana"
PathSeparator=" : ">
    <PathSeparatorStyle Font-Bold="True" ForeColor="#990000"></PathSeparatorStyle>
    <CurrentNodeStyle ForeColor="#333333"></CurrentNodeStyle>
    <NodeStyle Font-Bold="True" ForeColor="#990000"></NodeStyle>
    <RootNodeStyle Font-Bold="True" ForeColor="#FF8000"></RootNodeStyle>
</asp:SiteMapPath>
```

Burada önemli olan özelliklerden birisi, SiteMapPath sunucu kontrolüne ait, PathSeperator'dür. Bu özellik ile, sayfalar arasındaki ayraç işareti belirtilmektedir. Burada üst üste iki nokta kullanılmıştır. PathSeparatorStyle takısı ise, ayraç işaretinin font ve renk özelliklerini belirlemekte kullanılır. CurrentNodeStyle, kullanıcının o an bulunduğu sayfayı belirten node'un font ve renk özelliklerini belirlerken, NodeStyle üst nodelara ait ve RootNodeStyle ana sayfaya ait font ve renk özelliklerini belirler.

Şimdi, bu sayfaya uyguladığımız SiteMapPath sunucu kontrollünü kopyalayalım ve diğer sayfalarımıza yapıştıralım. Örnek olarak bu kezde, MuzikDvd.aspx sayfamızı çalıştıralım. Bu sayfayı çalıştırdığımızda ekran görüntüsü aşağıdaki gibi olacaktır.

![mk87_8.gif](/assets/images/2004/mk87_8.gif)

Şekil 8. MuzikDvd sayfamız.

Görüldüğü gibi, Xml içeriğinde belirttiğimiz description değeri burada ilgili sayfa için bir ipucu kutucuğu olarak ekrana çıkmıştır. Diğer taraftan, altı çizili linklere tıklayarak üst sayfalara hareket edebiliriz. Örneğin, Dvd ler linkine tıkladığımızda, Dvd.aspx sayfasına gideriz.

![mk87_9.gif](/assets/images/2004/mk87_9.gif)

Şekil 9. Dvd.aspx sayfasına geçtik.

SiteMapPath sunucu kontrolü için öenmli olan özelliklerden biriside, PathDirection'dır. Bu özellik, kontrol üzerindeki hiyerarşinin root'tan current'a yada tam tersi istikamette olup olmayacağını belirtlir. Varsayılan olarak bir SiteMapPath kontrolünün yönü, root'tan current'a doğrudur. Eğer yönü ters çevirmek istersek, sunucu kontrolüne ait kodu aşağıdaki gibi değiştirmemiz gerekir.

```csharp
<asp:SiteMapPath ID="SiteMapPath1" Runat="server" Font-Size="0.8em" Font-Names="Verdana"
PathSeparator=" : " PathDirection="CurrentToRoot">
```

Bu durumda, sunucu kontrolümüz aşağıdaki gibi görünecektir.

![mk87_10.gif](/assets/images/2004/mk87_10.gif)

Şekil 10. PathDirection özelliği CurrentToRoot yapıldığında.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde, TreeView kontrolü yardımıyla navigasyon işlemlerinin nasıl yapıldığını incelemeye çalışacağız. Şimdilik hoşçakalın.