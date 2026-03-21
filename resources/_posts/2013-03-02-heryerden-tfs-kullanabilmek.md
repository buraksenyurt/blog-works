---
layout: post
title: "Heryerden TFS Kullanabilmek"
date: 2013-03-02 08:50:00 +0300
categories:
  - team-foundation-server
tags:
  - team-foundation-server
  - team-explorer-everywhere
  - msscci-provider
  - sql-navigator
  - oracle
  - java
  - eclipse
  - branch
  - checkin
  - checkout
---
Yandaki fotoğrafta bir duvar prizi içinden USB bağlantısı yapıldığını ve cep telefonunun şarj edildiğini görmektesiniz. Bir süredir hayatımızda olan ilginç buluşlardan birisi de USB Priz’ ler. Bu aslında USB’ nin pek çok farklı ortama entegre edilebilmesi anlamına da geliyor. Söz gelimi bir süredir pek çok araç modelin USB çıkışları neredeyse standart. Telefonlarımızı yol boyunca şarj edebiliyoruz. Hatta USB olmayan araçlarda, çakmaktan gelen enerjiyi USB bağlantısı ile aktaran ara dönüştürücüler bile var.

[![usb-prizi](/assets/images/2013/usb-prizi_thumb.png)](/assets/images/2013/usb-prizi.png)


Dolayısıyla çeşitli ve pek çoğu standart hale gelen cihazlar ile USB çıkışları verebilmek mümkün. Nerden geldik şimdi bu USB Priz konusuna. Hem bir Plug-In gibi görülebildiği hem de entegrasyon anlamında sağladığı yetenekleri göz önüna alalım.

Bazen kullandığımız yazılım ürünlerinin de bu tip kolay takılabilir ve entegre olabilir şekilde üretilmelerini bekleriz. Örneğin TFS’ in sadece Visual Studio, MS Office, Sharepoint vb ürünler ile değil başka başka ürünler ile de çalışmasını isteriz.

Microsoft’ un ALM (Application Lifecycle Management) tarafındaki en önemli aracı bilindiği üzere Team Foundation Server ürünüdür. Genellikle Microsoft’ un yazılım geliştirme ürünleri ile haşırneşir olan firmalar TFS’ i ve uygun bir süreç geliştirme metodolojisini seçerek yaşamlarına devam ederler. Bu tip firmalar için karşılaşılabilecek sorunlar daha çok ALM’ in layıkıyla uygulanamayışıdır ki bu aslında hepimizin en büyük sorunudur ve tool’ dan bağımsız bir konudur. Yine de TFS kullanımı ile ilişkili olarak çok daha büyük bir sıkıntı vardır. Entegrasyon

![Thinking smile](/assets/images/2013/wlEmoticon-thinkingsmile_5.png)

Özellikle Enterprise çözümler üretmeye çalışan, bünyesinde 100lerce proje barındırabilen, zaman zaman hantallaşan firmaların, duruma göre tercih ettikleri pek çok geliştirme aracı/ortamı söz konusudur. Bazı firmalarda, Oracle’ cıları, Java’ cıları, Linux üzerinde C kodları yazanları, yeni nesil uygulamaları.Net ile geliştirenleri sıklıkla görebilirsiniz.

Hatta daha başka 3ncü parti araçlar bile söz konusu olabilir. Bu yerlerde pek tabi irili ufaklı sayısız ekip de söz konusudur. Bu ekipler, kendi içlerinde olduğu gibi şirket bazında da bir uygulama yönetim sürecine dahil olmak durumunda kalırlar. Kimi zaman CMMI gibi sıkıcı süreçler, kimi zaman da Scrum gibi eğlenceli süreçler, sistemin bir parçasıdır. İşte böyle bir senaryoda firmanın topyekün bir karar alarak TFS’ e geçeceğini hayal ediniz (ve tabi gelen tepkileri, direnci de haya ediniz). Aslında hayal etmenize gerek yok. Yapanlar var

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_172.png)

Bu durumda entegrasyon son derece önemli bir hale gelmektedir.

İşte bu yazımızda TFS’ in bize yabancı gelebilecek bazı geliştirme ortamları ile olan entegrasyonunu incelemeye çalışıyor olacağız.

Microsoft, TFS’ in çevre ürünler (Visual Studio harici diyebiliriz) ile olan entegrasyonunda için 3 önemli çözüm sunmaktadır. Temel prensip Team Explorer’ a ait arabirimlerin bir şekilde diğer araçlara takılabilmesidir (Plug-In, Add-In vb olacak şekilde) Yazının bundan sonraki kısımlarında ilgili araçları ve özellikle çeşitli arabirimler ile olan entegrasyonlarını mercek altına alıyor olacağız. Sloganımsı ada sahip olan ürün ile işe başlayalım.

Team Explorer Everywhere

Özellikle Eclipse gibi IDE’ lerin, Team Explorer arabirimine sahip olması ve TFS ile entegre çalışabilmesi için kullanılmaktadır. [Bu adresten](http://www.microsoft.com/en-us/download/details.aspx?id=30661) indirilebilen ürünün ayrıca diğer platformlar için komut satırından çalışabilen bir versiyonu da bulunmaktadır. Peki, örneğin Eclipse Juno ile bu entegrasyonu nasıl gerçekleştirebiliriz? Gelin adım adım ilerleyelim.

> Bazı geliştirme ortamları, Eclipse IDE’ sini kabuk olarak kullanır. Örneğin Business Process Management araçlarından birisi olan TIBCO Business Studio örnek olarak verilebilir. Bu tip araçlar da Eclipse IDE’ sini baz aldıklarından Team Explorer Everywhere ile TFS’ e ve doğal olarak ALM süreçlerine dahil olabilirler. En azından TIBCO tarafı için bunu test ettiğimi rahatlıkla ifade edebilirim
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_172.png)

Eclipse Juno

İlk olarak Eclipse Juno IDE’ si üzerinden Help menüsüne girip buradan Install New Software seçeneğini işaretlemeliyiz. Gelen arabirim de Work With kutucuğuna [http://dl.microsoft.com/eclipse/tfs/](http://dl.microsoft.com/eclipse/tfs/) adresine gitmemiz yeterlidir.

[![tfsint_4](/assets/images/2013/tfsint_4_thumb.png)](/assets/images/2013/tfsint_4.png)

Microsoft sunucularına gidilecek ve Team Explorer’ ın kullanmakta olduğumuz Eclipse IDE’ sine uygun olan versiyonu çekilecektir. Team Foundation Server Plug-in for Eclipse seçimi yapıldıktan sonra Next ile ilerlenip kurulum işlemi tamamlanır.

Sonuç olarak Eclipse IDE’ sinde Visual Studio’ dan aşina olduğumuz Team Explorer penceresi oluşacaktır. Buradan çok doğal olarak bir TFS sunucuna bağlanılabilir ve bir Team Project’ e dahil olunabilir.

[![tfsint_5](/assets/images/2013/tfsint_5_thumb.png)](/assets/images/2013/tfsint_5.png)

Eclipse kurulumunda yaşanabilecek sıkıntılardan biriside internet bağlantısı olmayan bir ortamda bu işin nasıl gerçekleştirilebileceğidir. Bu durumda bir şekilde yine download sayfasından TFSEclipsePlugin-UpdateSiteArchive-11.0.0.1212.zip dosyasının indirilmesi ve bu kez Install->Add kısmında aşağıdaki gibi ilgili dosyanın seçilerek eklenmesi yeterli olacaktır.

[![tfsint_6](/assets/images/2013/tfsint_6_thumb.png)](/assets/images/2013/tfsint_6.png)

> Eclipse IDE’ leri arasında Install adımları için farklılıklar olabilir. Test ettiğim IDE’ lerden birisinde Install Software penceresinde iki farklı tab bulunmaktaydı. Önce Available Software kısmından ilgili Team Explorer Everywhere ürününü bulmak (local file veya url ile) sonrasında Installed Software tabına geçerek ilgili versiyonu seçtikten sonra Install işlemini uygulamak mecburiyetinde kalmıştım. Son test ettiğim güncel Eclipse Juno ürününde ise bu işlem tek adıma indirilmişti.

Peki ya Team Explorer IDE’ sine sahip olamayacak farklı geliştirme ortamları söz konusu olursa ne yapacağız?

MSSCCI Provider

Özellike Team Explorer desteği bulunmayan (ya da entegre edilemeyen) IDE ve ortamlar için geliştirilmiş olan bu sağlayıcı, esasında bir Source Code Control API’ si olarak düşünülebilir. [Bu adresten](http://visualstudiogallery.msdn.microsoft.com/bce06506-be38-47a1-9f29-d3937d3d88d6) indirebileceğiniz Provider’ ın benim incelediğim sürümünün güncel olarak desteklediği IDE’ ler aşağıdaki gibidiydi.

- Visual Studio.NET 2003
- Visual C++ 6 SP6
- Visual Visual Basic 6 SP6
- Visual FoxPro 9 SP2
- Microsoft Access 2007
- SQL Server Management Studio
- Enterprise Architect 7.5
- PowerBuilder 11.5
- Microsoft eMbedded VC++ 4.0

Hımmm güzel bir listeye benziyor. Sanırım ilk dikkati çekenlerden birisi de PowerBuilder 11.5

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_172.png)

Peki ya elinizde çok daha eski bir sürüm var ise. Ya bu sürüm üzerinde geliştirilmiş onlarca uygulama bulunmaktaysa ve daha uzun bir süre hayatta olacaklar ise. Ya.Net tabanlı geliştirme yapanların bu ortamda geliştirilen arabirimleri de kullanması söz konusu ise. Ya her iki ortamda ALM üzerinden takip edilmek zorunda ise. Örneğin elinizde PowerBuilder’ ın 9.0.3 sürümü olduğunu düşünün.

> MSSCCI Provider’ ın yüklenmesi sırasında makinede en azından Visual Studio Team Explorer’ ın yüklü olması gerekmektedir. Ayrıca ürünün versiyonu da önemlidir. Örneğin, TFS 2012 ile entegre olunacak ise MSSCCI Provider’ ın 2012 sürümü kullanılmalıdır.
> Bir diğer ön gereklilik de şudur. Diyelim ki developer makinesinde Visual Studio 2010 da bulunmakta ve yine aynı makine de yer alan SQL Navigator ile TFS 2012’ ye bağlanılmak isteniyor. MSSCCI Provider’ ın 2012 sürümü yüklenirken uyarı mesajı alınacak ve Team Explorer’ ın yüklenmesi istenecektir. Dolayısıyla 2012 tabanlı bir TFS ortamında, MSSCCI Provider 2012 ve Team Explorer 2012 olmalıdır.

PowerBuilder

İlk olarak MSSCCI Provider’ ın PowerBuilder’ ın bulunduğu ortama yüklenmesi gerekmektedir.

> Bazen kalabalık ekiplerin yer aldığı firmalarda, geliştiricilerin bir program install etmeye yetkisi olmaz. Böyle bir durumda gerekli tüm makinelere sistem ekibi tarafından Update geçilir. Ya da geliştirici makinelerinde uzaktan bağlanılarak sessiz modda bir kurulum (Slient Install) işlemi yapılır. Bu noktada msi tipinden bir setup dosyasının sessiz modda nasıl çalıştırılacağı da önemlidir. İşte MSSCCI Provider için örnek bir kullanım şekli.
> msiexec /i"Visual Studio Team Foundation Server 2012 MSSCCI Provider (32-bit).msi" /quiet /norestart
> Buna göre MSSCCI provider bir arayüz göstermeden arka planda otomatik olarak varsayılan ayarları ile kurulacaktır. /quiet parametresi bunun için kullanılmaktadır. Tahmin edileceği üzere /norestart parametresi sayesinde de, ilgili kurulum işlemi sonrası makinede bir restart yaptırılmaması sağlanmaktadır. Elbetteki başrol oyuncusu msiexec aracıdır.

PowerBuilder gibi ortamların TFS ile ortak çalışması Eclipse’ de olduğu gibi değildir. Aslında MSSCCI sağlaycısını kullanan ürünlerin TFS ile çalışmaları oldukça farklı olabilmektedir. Örneğin Powerbuilder’ da Workspace’ ler ile çalışılmaktadır (Powerbuilder uzmanı değilim onu belirteyim) Bu sebepten bir Workspace’ in TFS üzerindeki bir Team Project’ e hatta bir Branch’ a dahil edilmesi için Properties kısmına gidilmesi ve Source Control tabında gerekli ayarların yapılması gerekir.

[![tfsint_7](/assets/images/2013/tfsint_7_thumb.png)](/assets/images/2013/tfsint_7.png)

Eğer provider yüklendiyse Source Control System listesinde de çıkacaktır. Çok doğal olarak PowerBuilder geliştiricisi, TFS üzerindeki projeye giderken bir kullanıcı adı ile hareket etmelidir. Bu sepebten projede uygun rolde olması şarttır (Contributor olur, Project Administrator olur vs) Dolayısıyla User ID kısmında domainaı/kullanıcıadı gibi bir bildirim yapılmalıdır. Project kısmı ise en sevdiğim taraflardan birisidir. Burada 3 nokta butonuna basıldıktan sonra TFS sunucunun adresinin sorulduğu bir pencere ile karşılaşılacaktır.

[![tfsint_8](/assets/images/2013/tfsint_8_thumb.png)](/assets/images/2013/tfsint_8.png)

Eğer sistemde yüklü bir Visual Studio ürünü varsa ve daha önceden çeşitli TFS sunucularına bağlanıldıysa, bu durumda ilgili Server listesinin bu pencerelerde de çıktığı görülecektir. İstenirse Add düğmesi ile yeni bir TFS sunucu adresinin bildirimi de pekala yapılabilir.

Sunucu seçimi yapıldıktan sonra ise Choose Folder in Team Foundation Server isimli bir diğer pencere açılacaktır. Burada ise Team Project’ in ve ilgili klasörün (hatta çoğunlukla Branch’ in) seçilmesi yeterli olacaktır. Tüm bu işlemler sonrasında Workspace’ in belirtilen TFS Branch’ ine dahil edilmesi işlemi de tamamlanmış olmaktadır. Eğer herşey yolunda gittiyse ve doğru ayarlamaları yaptıysanız bir Workspace’ in öğelerinde aşağıdaki şekilde görülen özelliklere ulaşılabildiğini fark edebilirsiniz.

[![tfsint_9](/assets/images/2013/tfsint_9_thumb.png)](/assets/images/2013/tfsint_9.png)

Şimdi bakış açımızı yine eskilerden birisine çevirelim.

SQL Navigator for Oracle 6.5.

Öncelikli olarak Team Coding menüsünden Connection Settings kısmına girilir. Eğer MSSCCI yüklüyse burada SCC: Microsoft Team Foundation Server MSSCCO Provider ismi ile listelenecektir.

[![tfsint_10](/assets/images/2013/tfsint_10_thumb.png)](/assets/images/2013/tfsint_10.png)

Bu seçim yapıldıktan sonra ise yine Team Coding menüsünden Code Control Groups’ a geçilerek yeni bir kod kontrol grubunun eklenmesi adımına geçilir.

[![tfsint_11](/assets/images/2013/tfsint_11_thumb.png)](/assets/images/2013/tfsint_11.png)

Grup adının girilmesi haricinde en önemli kısım tahmin edileceği üzere Project Name seçimidir. Burada yine PowerBuilder implementasyonunda olduğu gibi bir TFS sunucusunun seçimi ile başlayan adımlar yer almaktadır. Sunucu seçiminin ardından yine Team Project’ in ve Branch’ in işaretlenmesi gerekmektedir. Sonrasında ise bir Working Directory belirtilmelidir.

Buraya kadarki işlemler tamamlansa da yeterli değildir

![Confused smile](/assets/images/2013/wlEmoticon-confusedsmile_28.png)

Bir de Mask belirtilmesi ve Source Code Control operasyonlarının ilgili Oracle şemasındaki hangi nesneler için yapılacağının belirtilmesi gerekir. Bunun için oluşturulan grup çift tıklanır ve açılan arabirimden Add DB Object Mask seçimi yapılır.

[![tfsint_12](/assets/images/2013/tfsint_12_thumb.png)](/assets/images/2013/tfsint_12.png)

Örnekte ilgili şemada yer alan herhangibir Procedure’ ün ele alınacağı ifade edilmektedir. Bu işlemi de tamamladıktan sonra grub adı seçilerek istenirse Export to VCS ile Mask’ a uygun olarak çıkan listeden nesne seçimleri yapılabilir. Bu kalabalık bir Procedure topluluğunda sadece işaretli olanların Source Code Control ile ele alınması açısından işe yarar bir özellik olabilir (Denemedim deneyiniz) Artık Navigator IDE’ sinde çalışırken Procedure’ lerin Check-In/Check-Out edilmesi mümkündür. Hatta Get Latest Version özellikleri dahi çalışacaktır. Zaten projeyi ilişkilendirirken Branch seçimini de yapmıştık ve doğal olarak yapılanların hangi branch üzerinde olacağı da bellidir.

[![tfsint_13](/assets/images/2013/tfsint_13_thumb.png)](/assets/images/2013/tfsint_13.png)

Kurulum aşamalarında değinmedik ancak her hangibir IDE’ deki bir öğenin Check-In’ lenmesi sırasında çok tanıdık bir arabirim ile karşılaşılmaktadır.

[![tfsint_14](/assets/images/2013/tfsint_14_thumb.png)](/assets/images/2013/tfsint_14.png)

Edindiğim bu araştırma tecrübeleri sonucunda aşağıdaki ürünleri TFS 2012’ ye entegre edebildiğimi ve ALM içerisinde çalışabildiğimi gördüm. Üstelik çoğu oldukça eski sürümler olmasına rağmen.

- SQL Navigator for Oracle 6.5
- Powerbuilder 9.0.3
- TIBCO Business Studio 3.4
- Eclipse Juno 4.2.1

Tabi kafa da bazı sorular mutlaka oluşacaktır. Özellikle Branch’ lerin Merge edilmeleri noktasında. Şunu açıkça ifade edelim ki aslında projenin ana yöneticisi Visual Studio arabirimine ait olan Team Explorer bölümü Source Code Explorer’ dır Dolayısıyla Merging gibi bazı yönetimsel operasyonlar zaten bu IDE üzerinden yapılır/yapılmalıdır.

[![tfsint_15](/assets/images/2013/tfsint_15_thumb.png)](/assets/images/2013/tfsint_15.png)

Anlattıklarımız TFS’ in, Microsoft dışı veya eski Microsoft ürünlerinin tamamı ile entegre olabileceğini ifade etmese de ucundan da olsa bir tüme varım ispatını gerçekleştirdiğimizi ifade edebiliriz.

Yazının bu kısmına geldiyseniz eğer kafanızda bir soru da oluşmuş olabilir. 3ncü çözüm nedir?

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_172.png)

LINUX/UNIX/MAC OS X Tarafı

Aslında 3ncü çözüm Windows dışı işletim sistemlerini daha fazla ilgilendirmektedir. Örneğin SOLARIS SPARC sistemi veya RED HAT yüklü bir LINUX sistemi söz konusu olabilir. Pek tabi bu platformlar üzerinde yapılan geliştirmelerde komut satırı yaygın olarak kullanılmakta ve hatta ağırlıklı olarak C kodları geliştirilmektedir. Hal böyle olunca TFS ile olan entegrasyon kocaman bir soru işerati olarak görünmektedir. İşte 3ncü çözüm bu konu ile ilintilidir. Git-tf

![Surprised smile](/assets/images/2013/wlEmoticon-surprisedsmile_4.png)

Sanırım adı siz de bir çağırışım yapmıştır. Git ile Team Foundation Server’ ın arasında bir köprü görevi gören bu araç, Codeplex üzerinden sunulan bir projedir. [Bu adresten](http://gittf.codeplex.com/) ulaşabilen proje, 27 Ağustos 2012’ de ilk stable sürümünü de çıkartmıştır. Henüz testlerini yapamadım ama ana sayfada da bahsedildiği üzere Linux ve Mac OS X platformlarında kurulabilen (nitekim Java Runtime üzerinde çalışmakta olan) bir ürün. Üstelik komut satırından çalıştırılabildiği için Linux/Unix tarafında çalışan geliştiriciler için de oldukça kullanışlı. Bu konuda ilk testleri yaptığımda sanıyorum ki burayı güncelliyor olacağım. (Şimdilik kurulum ve diğer detaylarla ilişkili olarak Microsoft’ un yayınladığı [şu dökümana bir göz atabilirsini](https://www.google.com.tr/url?sa=t&rct=j&q=&esrc=s&source=web&cd=3&ved=0CEAQFjAC&url=http%3A%2F%2Fdownload.microsoft.com%2Fdownload%2FA%2FE%2F2%2FAE23B059-5727-445B-91CC-15B7A078A7F4%2FGit-TF_GettingStarted.html&ei=spbuULFbzNCyBueugNgJ&usg=AFQjCNEHn0PqmiDDtfdO_b45A-WPovulEA&sig2=6zA7olBg18UM62tY4W4B9Q&bvm=bv.1357700187,d.Yms)z)

Red Hat Linux’ testi gelecek[Henüz yazılmadı]

Bu makalemizde Team Foundation Server’ ın çevre IDE’ ler ile olan entegrasyonunu incelemeye çalıştık ve bu konuda başarılı olduğunu gördük. Ancak pek tabiki proje geliştirmek ve özellikle ALM akışlarının doğru ve sorunsuz bir şekilde yürütülmesini sağlamak, sadece kullanılan araçların entegre edilmesi ile olacak bir iş değildir. Dolayısıyla olayın farklı bir boyutu olduğunu da gözden kaçırmamak gerekir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.