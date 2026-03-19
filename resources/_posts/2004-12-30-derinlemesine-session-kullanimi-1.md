---
layout: post
title: "Derinlemesine Session Kullanımı - 1"
date: 2004-12-30 18:00:00 +0300
description: "Bu makalemizde, Asp.Net sunucularında durum yönetimi (state management) amacıyla kullanılan Session nesnesini detaylı bir şekilde incelemeye başlıyacağız. Bildiğiniz gibi, web anatomisinde durum yönetimi başlı başına bir terminolojidir. Web sitelerinin yer aldığı web sunucularının istemci makinelerde durum yönetme yeteneğine sahip olamamaları sonucu, belli bir kullanıcıya ait bilgilerin sayfalar arasında korunması veya taşınabilmesi için değişik teknikler geliştirilmiştir. Session kullanımı bu tekniklerden birisidir. Öncelikle Session kavramının ne olduğunu ve ne için kullanıldığını kavramaya çalışalım."
categories:
  - aspnet
tags:
  - aspnet
  - csharp
  - dotnet
  - http
  - performance
  - dataset
---
# Derinlemesine Session Kullanımı - 1
Değerli Okurlarım, Merhabalar.

Bu makalemizde, Asp.Net sunucularında durum yönetimi (state management) amacıyla kullanılan Session nesnesini detaylı bir şekilde incelemeye başlıyacağız. Bildiğiniz gibi, web anatomisinde durum yönetimi başlı başına bir terminolojidir. Web sitelerinin yer aldığı web sunucularının istemci makinelerde durum yönetme yeteneğine sahip olamamaları sonucu, belli bir kullanıcıya ait bilgilerin sayfalar arasında korunması veya taşınabilmesi için değişik teknikler geliştirilmiştir. Session kullanımı bu tekniklerden birisidir. Öncelikle Session kavramının ne olduğunu ve ne için kullanıldığını kavramaya çalışalım.

Bir kullanıcı, bir web sunucusundan herhangibir sayfa talep ettiğinde, web sunucusu bu kullanıcı için kendi sistemi üzerinde veya yardımcı bir sunucuda (web farm'lar göz önüne alındığında) bir oturum başlatır. İşte Session nesneleri yardımıyla kullanıcıya ait olan bu oturum boyunca sayfalar arasında bilgi taşıma işlemini gerçekleştirebiliriz. Session kullanımına verilebilecek en güzel ve klasik örnek, kullanıcılarının alış veriş sepetleri ile birlikte site içindeki sayfalar boyunca gezinebilmeleridir. Burada temel fikir kullanıcının sahip olduğu sepete ait bilgileri site içerisinde yer alan sayfalar boyunca taşıyabilmektir. Biz bu amaçla Session nesnelerini kullanabilir ve kullanıcının oturumu sonlanıncaya kadar sayfalar arasında veri taşıyabiliriz. Elbette başlayan bir oturum mutlaka bir şekilde sonlandırılır. Bir oturum temel olarak aşağıdaki nedenlerden dolayı sonlanabilir?

Oturum Sonlanmasına Neden Olacak Etmenler

Oturumun zaman aşımına uğraması. (Timeout)
Varsayılan olarak sunucu üzerinde açılan her bir oturum 20 dakikalık zaman aşımı süresine sahiptir. Sitemizi ziyaret eden kullanıcıların Session verileri ile ne kadar süre durabilecekleri göz önüne alınarak bu süre daha da düşürülebilir veya arttırılabilir. Sürede yapılan ayarlamalar kesinlikle sunucuya ait bellek tüketimini etkileyecektir.

Kullanıcının tarayıcı penceresini kapatması veya sayfaya yeni bir tarayıcı penceresi açarak ulaşması.
Bir web sunucusu kullanıcının tarayıcısını kapatıp kapatmadığını anlayamaz. Onun yerine barındırdığı sayfalara yapılan çağrıları tespit eder. Dolayısıyla bir kullanıcı açık bir oturuma sahip iken tarayıcısını kapattığında veya yeni bir tarayıcı penceresi açtığında aynı sayfaya yeni bir oturum içinde bağlanır. Bunun sebebi sunucunun ürettiği ASP.NET_SessionId'lerin her bir istekte yeniden oluşturulmasıdır.

Oturum kapatılmaya zorlandığında.
Session nesnesine Abondon metodu uygulanarak oturumlar iptal edilebilir.

Sunucu yeniden başlatıldığında.
Bir sunucunun yeniden başlatılması eğer SessionId değerleri işlem içinde (yani asp.net work processor'un açtığı iş parçalarında) tutuluyorsa otomatik olarak kaybedilecektir. Ancak SessionId lerin bir Sql sunucusunda database'de veya StateServer isimli windows servisinde tutulması sağlanarak bu kayıpların önüne geçilebilir.

Gelelim Session nesnelerinin sunucu tarafından doğru istemciler ile nasıl eşleştirilebildiğine. Web sunucusundan bir sayfa talep edildiğinde (1) Asp.Net Work Processor bu istemci için otomatik olarak benzersiz bir tanımlama (Unique Identity) değeri üretir (2). Bu 120 bitlik bir sayıdır ve özel bir algoritma yardımıyla sunucu üzerinde oluşturulur. Daha sonra bu talep karşılığında sunucu, oluşturduğu ASP.NET_SessionId değerini istemci bilgisayara gönderir (3). Bu ASP.NET_SessionId değerinin istemci üzerinde tutuluşu varsayılan olarak bir çerez (Cookie) vasıtasıyla sağlanır (4). Aynı değer Sunucu üzerinde de yer almaktadır (4). Bu noktadan itibaren istemci, kendisine ait oturumu sunucu üzerinde bir şekilde sonlanıncaya kadar, site içerisinde bu ASP.NET_SessionId değeri ile tanınır. Dolayısıyla artık sunucu, oturum sahibinin kim olduğunu bilmektedir.

![dikkat.gif](/assets/images/2004/dikkat.gif)
Tanıma; Sunucular oturum sahiplerini tanıyabilmek için, özel bir algoritma ile oluşturdukları 120 bitlik benzersiz bir tanımlayıcı değer kullanırlar. ASP.NETSessionId.

Ancak burada istisnai bir durum vardır. Her istemci cookie'leri desteklemez. Bu durumda yapılacak ufak bir ayarlama ile ASP.NET_SessionId değerinin url'ye eklenmesi sağlanır. Bunu ilerleyen safhalarda inceleyeceğiz. Aşağıdaki şekil basit olarak bir SessionId değerinin nasıl oluşturulduğunu betimlemektedir. Bu varsayılan, yani Cookie desteği veren tarayıcılar için geçerli olan senaryodur.

![mk111_1.gif](/assets/images/2004/mk111_1.gif)

Session nesnesine atanan veriler web sunucusu üzerindeki bellekte tutulmaktadır. Bu bilgiler asla istemci bilgisayara gönderilmezler. Bu da güvenlik açısından önemli bir avantajdır.

![dikkat.gif](/assets/images/2004/dikkat.gif)
Güvenlik; Session nesnelerine ait veriler Sunucu üzerindeki bellek alanlarında tutulur ve asla istemcilere gönderilmez.

Burada dikkat etmemiz gereken önemli bir husus vardır. Session bilgilerinin sunucu belleğinde (varsayılan olarak budur) tutulması. Bir Session nesnesinin en büyük özelliklerinden birisi serileştirilebilir (Serializable) nesneleri taşıyabilme özelliğine sahip olmasıdır. Yani, bir Session nesnesi yardımıyla kullanıcının oturumu boyunca bir DataSet nesnesini sayfalar arasında dolaştırabiliriz. Peki burada dikkat etmemiz gereken nokta nedir? Sorun DataSet gibi bir nesnenin tek başına çalıştığı bir bilgisayar için bile fazla bellek alanı harcamasıdır. Yani, sunucuya bağlanan her bir kullanıcı için oturum süresi boyunca DataSet nesne örneklerinin taşınması sunucu kaynaklarını ciddi ölçüde azaltır. Bellekte meydana gelen bu azalma elbetteki web sunucusunun performansını olumsuz yönde etkileyecektir.

![dikkat.gif](/assets/images/2004/dikkat.gif)
Performans; Bir web uygulamasında Session nesnelerinin DataSet nesne örnekleri taşıdığını varsayarsak, N sayıda kullanıcının bağlanması, sunucu belleğinde N sayıda DataSet nesne örneğinin oluşturulması anlamına gelmektedir.

Peki çözüm olarak ne üretebiliriz? İlk olarak önbellekleme tekniklerini kullanabiliriz. Ya da Serileştirilebilir nesneleri örneklendiren sınıfları kendimiz yazarız. Tabi böyle bir durumda yazacağımız sınıflarında serileştirilebilir olması bir başka deyişle serileştirmeye izin veren nitelikleri (Attribute) kullanması gerekecektir. Bu konuyu ilerleyen makalelerimizde incelemeye çalışacağız. Elbetteki bir Session sadece serileştirilebilir nesneleri değil her tipte.NET nesne örneğini saklayabilir.

Bu kısa açıklamalardan sonra dilerseniz, Session nesnelerinin nasıl oluşturulduğuna ve kullanıldığına kod kısmından basit bir örnel ile bakmaya çalışalım. Aşağıdaki uygulamada, default.aspx, sayfa2.aspx ve sayfa3.aspx isimli 3 adet web sayfamız var. Biz default.aspx içerisinde kullanıcıdan isim ve soyisim bilgisini alıyor ve bunu Session nesnemizde tutuyoruz. Daha sonra sayfa2.aspx ve sayfa3.aspx içerisinden bu session değerlerini okuyup ekrana yazdırıyoruz. Default.aspx ilk açıldığında, üretilen ASP.NET_SessionId değerini de Session nesnesinin SessionId özelliği ile elde ediyoruz. Örnek uygulamayı [bu linkten](/assets/files/2004/SessionSample1.zip) indirebilirsiniz.

```csharp
//default.aspx
private void btnEkle_Click(object sender, System.EventArgs e)
{
    Session["Kimlik"]=txtBilgi.Text; //Session' a değer atıyoruz.
}

//sayfa2.aspx
private void Page_Load(object sender, System.EventArgs e)
{
    lblAdSoyad.Text=Session["Kimlik"].ToString(); //Session değerini okuyoruz.
}

//sayfa3.aspx
private void Page_Load(object sender, System.EventArgs e)
{
    lblAdSoyad.Text=Session["Kimlik"].ToString(); //Session değerini okuyoruz.
}
```

![mk111_2.gif](/assets/images/2004/mk111_2.gif)

Yukarıdaki örnek Session kullanımının en basit halidir.

Oturum yönetiminde işimize yarayacak iki olay vardır. Session_Start ve Session_End olayları. Bir istemci bir web sunucusunda bir oturum başlattığında uygulamaya ait global.asax dosyasında yer alan Session_Start olayı çalışır. Oturum herhangibir neden ile sonlandığında ise yine global.asax dosyasındaki Session_End olayı çalışır. Bu olaylar yardımıyla, uygulamanızda oturum halinde bulunan kullanıcı, sayısını bir başka deyişle sayfadaki ziyaretçi sayısını öğrenebilirsiniz. Örneğin az önceki uygulamamızı ele alalım ve Global.asax dosyasında aşağıdaki değişiklikleri yapalım. Uygulamamızdaki default.aspx sayfasında da aktif ziyaretçi sayısını göstermek için bir label kullanacağız.

```csharp
protected void Session_Start(Object sender, EventArgs e)
{
    /*Uygulama seviyesinde bir değişken kullanmak istediğimizden Application nesnesini kullandık. Eğer ilk kez oturum açılıyor ise varsayılan değeri 1 olarak ayarlıyoruz. İlk kez açılmıyorsa var olan değeri 1 arttırıyoruz.*/
    if((int)Application["ToplamZiyaretci"]==0)
        Application["ToplamZiyaretci"]=1;
    else
    {
        int deger=(int)Application["ToplamZiyaretci"];
        deger+=1;
        Application["ToplamZiyaretci"]=deger;
    }
}

protected void Session_End(Object sender, EventArgs e)
{
    /* Oturum kapatıldığında devereye giren bu olayda, uygulama seviyesindeki Application nesnesinde saklanan değerini 1 azaltıyoruz. Böylece online ziyaretçi sayısını 1 azaltmış oluyoruz.*/
    int deger=(int)Application["ToplamZiyaretci"];
    deger-=1;
    Application["ToplamZiyaretci"]=deger;
}
```

Kodda dikkat ederseniz Application nesnesinin değerini deger isimli değişkene alırken (int) tür dönüştürme operatörünü kullandık. Bunun sebebi, Application nesnesininde Session nesnesi gibi verileri object tipinde saklıyor olmasıdır. Dolayısıyla Session veya Application nesnelerinin değerlerini okurken uygun tür dönüşümlerini açıkça yapmamız gerekiyor.

Artık tek yapmamız gereken Application nesnesinin değerini okumak ve label kontrolümüze yazdırmak.

```csharp
private void Page_Load(object sender, System.EventArgs e)
{
    Session["Kimlik"]=txtBilgi.Text;
    lblZiyaretciSayisi.Text=Application["ToplamZiyaretci"].ToString();
}
```

Bundan sonra uygulamamıza her yeni oturum açışımızda, Session_Start devreye girecek ve kullanıcı sayısını otomatik olarak 1 arttıracaktır. Session'lar sonlandığında ise 1 eksileceklerdir. Bunu daha iyi kavrayabilmek için oturumların timeout süresini 1 dakikaya indirelim. Session nesnesinin TimeOut özelliğine 1 değerini atayarak bunu sağlayabiliriz.

```csharp
Session.Timeout=1;
```

Şimdi peş peşe tarayıcı pencereleri açıp uygulamamızın olduğu default.aspx sayfasını çağıralım.

![mk111_3.gif](/assets/images/2004/mk111_3.gif)

Burada da görüldüğü gibi, Session_Start olayı başarılı bir şekilde çalışarak Application nesnesinin ToplamZiyaretci elemanının değerini sürekli olarak 1 arttırmıştır. Lakin biz Session Timeout süresini 1 dakika yaptığımız için 1 dakika sonra Session nesnesinin değerinin null olması gerekmektedir. Bu süreyi bekledikten sonra yeni bir tarayıcı penceresi açıp sayfamıza girmek istersek aşağıdaki ekran görüntüsü ile karşılaşırız. Görüldüğü gibi ToplamZiyaretci değeri başlangıç değerine atanmıştır.

![mk111_4.gif](/assets/images/2004/mk111_4.gif)

Burada diğer bir durum daha vardır. Diyelim ki Timeout süresini 10 dakika yaptık. Uygulama çalıştırıldı ve bir kaç oturum açıldı. Sonra ise, 10 dakika dolmadan önce Timeout süresini 1 dakika yaptık. Ardından yeni oturumlar açtık. Son oturum açışından 1 dakika geçtikten sonra, önceden yaşam süreleri 10 dakika iken çalışan oturumlar durumlarını korumaya devam ederler.

Bazen istemci tarayıcılar Cookie desteğine sahip değildir veya bu özellikleri bilerek kapatılmıştır. Peki böyle bir durumda istemci bilgisayar, sunucuda kendisi için açılan oturum ile nasıl eşleştirilebilir? Bu sorunun çözümü ASP.NET_SessionId değerinin site içinde hareket edilen sayfaların linklerine eklenmesidir. Asp.Net bunu bizim için otomatik olarak yapmaktadır. Tek yapmamız gereken web.config dosyasında sessionState elemanında, Cookieless özelliğine true değerini atamaktır.

```text
<sessionState 
    mode="InProc" stateConnectionString="tcpip=127.0.0.1:42424" 
    sqlConnectionString="data source=127.0.0.1;Trusted_Connection=yes" 
    cookieless="true" 
    timeout="20" 
/>
```

Eğer uygulamamızı bu haliyle denersek sayfalar arasında gezinirken url bilgilerinin aşağıdaki gibi değiştiğini görürüz. Görüldüğü gibi ASP.NET_SessionId değeri otomatik olarak relative url bilgisinin önüne eklenmiştir.

default.aspx için;

![mk111_5.gif](/assets/images/2004/mk111_5.gif)

Eğer bu sayfadan diğer sayfalara geçiş yapar isek, oluşturulan ASP.NET_SessionId değerinin bu sayfaların url bilgisine de eklendiğini dolayısıyla Session'ın korunduğunu görürüz.

![mk111_6.gif](/assets/images/2004/mk111_6.gif)

Ancak burada dikkat etmemiz gereken önemli bir nokta vardır. Linkler arasında ASP.NET_SessionId değerinin url bilgisi ile aktarılabilmesini istiyorsak relative linkleri kullanmak zorundayız. Yani Sayfa2 başlıklı LinkLabel nesnesinin NavigateUrl özelliğinin değerine, sayfa2.aspx (default.aspx ile aynı lokasyonda olduğu için) değerini vermemiz gerekir. Eğer buraya http://localhost/SessionSample1/Sayfa2.aspx gibi bir tam yol linki verirsek; bu linke tıkladığımızda default.aspx'teki oturum yok sayılacak ve yeni bir oturum açılmaya çalışılacaktır.

![mk111_7.gif](/assets/images/2004/mk111_7.gif)

Görüldüğü gibi default.aspx sayfasını açtığımızda oluşan ASP.NET_SessionId değeri ile, buradan Sayfa2.aspx'e gittiğimizde URL bilgisine eklenen ASP.NET_SessionId değerinden farklıdır. Dahası burada, Sayfa2.aspx için yeni bir oturum söz konusu olduğundan, default.aspx içinde oluşturulan Session["Kimlik"] bilgisine buradan erişilemiyecektir. Bunun sonucuda da var olmayan bir nesneye erişmek istediğimizden hata almaktayız.

Bu makalemizde Session nesnesine kısaca değinmeye çalıştık. İzleyen makalemizde, Session nesnesi ile ilgili daha farklı konulara göz atmaya çalışacağız. Bu konular arasında ASP.NET_SessionId değerlerinin bir sql sunucusunda veya StateServer servisinde tutulması da yer almaktadır. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.