---
layout: post
title: "Asp.Net 4.0 - Özelleştirilmiş Cache Sağlayıcısı(Custom Cache Provider) [Beta 2]"
date: 2009-11-17 06:10:00 +0300
categories:
  - aspnet-4-0-beta-2
tags:
  - aspnet-4-0-beta-2
  - csharp
  - xml
  - dotnet
  - aspnet
  - linq
  - http
  - threading
  - concurrency
  - caching
  - serialization
  - generics
  - dependency-management
---
Çok çok uzun zamandır Asp.Net üzerine eğilmiyordum. Hem tasarım yönünden kabiliyetsiz olmam (iki rengi bir araya getir deseler kesin uyumsuz renkler çıkartırım) hemde servis yönelimli mimari dünyasına dalmış olmamın bunda büyük bir rol oynadığını itiraf etmek isterim. Yine de Asp.Net 4.0 ile birlikte gelen yenilikleri okuyunca biraz olsun araştırmak ve edindiğim tecrübeleri sizlere aktarmak istedim.

![blg102_Giris.jpg](/assets/images/2009/blg102_Giris.jpg)

Dikkatimi çeken ilk özellik ön bellekleme (Caching) sisteminin genişletilebilmesi ile alakalıydı. Bilindiği üzere web uygulamalarında performansı arttırmanın en etikili yollarından biriside sunucu tarafındaki yükü azaltarak mümkün olabilmektedir. Bu manada istemci tarafına, sunucu üzerinde ön belleğe atılmış hazır veri çıktılarını göndermek etkili bir yaklaşımdır. Asp.Net tarafında ön bellekleme için farklı teknikler kullanılabilir. Son süre bildirimli (Expire Date), uzatmalı (Sliding), bağımlı (Dependency, SqlCacheDependency, FileDependency gibi) vb...Üstelik ön belleklenecek veri içeriği sayfa bazında, Web User Control bazında vb olabilir. Yine de eksik olan bir şeyler vardır. Herşeyden önemlisi ön bellekleme gerçekten bellek üzerinde yapılmaktadır.

![Smile](/assets/images/2009/smiley-smile.gif)

Basit bir blog sitesi için ön belleklenecek veri içeriği çok büyük problem teşkil etmeyebilir.

Ne varki çok sayıda kullanıcıya hizmet veren portallerde ön belleklenen nesnelerin sayısının, içeriğinin artması, beraberinde belleğinde ölçeklendirilmesi ihtiyacını doğurmaktadır. Buna göre daha çok bellek almak gibi bir maliyet altına girmek gerekebilir. Bu duruma karşın Asp.Net 4.0 ile birlikte ön bellekleme işlemini daha kolay bir şekilde özelleştirebilme şansına sahibiz. Bir başka deyişle kendi Cache Provider tiplerimizi geliştirerek ön bellekleme yerini ve modelini değiştirebilir kendi algoritmalarımızı işin içerisine katabiliriz. Tabi Beta 2 sürümüne göre bu konuyu incelediğimden ve yazıyı hazırladığım tarih itibariyle internet üzerinde çok fazla kaynak bulamadığımdan halen daha pek çok noktada soru işareti vuku bulmuş durumdadır. Bu nedenle zaman içerisinde bu konuda çok daha detaylı bilgiye ulaşabileceğimizi düşünmekteyim.

Dilerseniz hiç vakit kaybetmeden örnek bir senaryo üzerinden hareket edelim. Amacımız web uygulamamızda kullanılan bir Web User Control'ün içeriğini zamana bağlı olarak ön belleklerken, dosya tabanlı bir sistemden yararlanmak. Buna göre ön belleğe atılacak olan nesne içeriğinin fiziki olarak bir dosya içerisine serileştirmeyi (Serialization) planlıyoruz. Çok doğal olarak web uygulaması içerisinde n sayıda sayfa ve n sayıda Web User Control olabilir. Buda fiziki olarak her Web User Control için birden fazla serileştirilmiş veri içeriği tutan dosya anlamına gelmektedir. Diğer yandan sistemimizde Expire eden dosyaların silinmesi işlemlerini de göz önüne almalıyız. Yani bu dosyaların içerikleri gelen yeni bir talep sonrasında yeniden üretilmeli, talep gelmediği ve Duration dolduğunda ise silinmelidir şeklide bir yol tercih ediyoruz.

Peki Asp.Net 4.0 tarafında bu tip özelleştirilmiş bir Cache sağlayıcısı için ne getirilmiştir?

Aslında tahmin etmek oldukça kolaydır. Var olan bir sisteme yeni bir eklenti ilave etmek istiyorsak tercih edilecek yollardan birisi, sistemin bize söylediği kurallara uymaktır.

![Wink](/assets/images/2009/smiley-wink.gif)

Şimdilik bu kuralı söyleyen OutputCacheProvider isimli abstract bir sınıftır. Söz konusu sınıf Add, Get, Set ve Remove isimli abstract metodlar içermektedir. Buna göre geliştireceğimiz Cache Provider sınıfının bu fonksiyonları mutlaka ve mutlaka ezmesi gerekmektedir. Bir başka deyişle Cache sistemi için gerekli temel CRUD operasyonlarının tarafımızdan uygulanması gerekmektedir. Peki yeterli midir? Elbette değildir.

Bir şekilde web uygulaması tarafına, kendi özel Cache sağlayıcımızı kullanabileceğimizi ifade etmemiz gerekecektir ki bunun içinde tahmin edeceğiniz üzere web.config dosyasından yararlanılacaktır. Geliştireceğimiz örnekte özel Cache sağlayıcısını çok kısıtlı olarak kullanabileceğiz. İlk hedefimiz senaryomuzda belirttiğimiz üzere Web User Control içeriklerini ön belleklemektir. Buna göre standart olarak kullandığımız OutputCache direktifinde bir şekilde özel Cache sağlayıcımızı da işaret edebilmeliyiz ki buda oldukça kolaydır. Öyleyse hiç vakit kaybetmeden örneğimizi geliştirmeye başlayalım. İlk olarak DiskCacheProvider isimli OutputCacheProvider tipinden türeyen aşağıdaki sınıfı geliştirmeliyiz.

DiskCacheProvider sınıfına ait diagram;

![blg102_ClassDiagram.gif](/assets/images/2009/blg102_ClassDiagram.gif)

Custom Provider kodu;

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using System.Web.Caching;
using System.Collections.Concurrent;

namespace CustomCaching
{
    public class DiskCacheProvider
        : OutputCacheProvider
    {
        // Expire olan Cache nesnelerini tutan dosyaların silinmesi için bir Timer nesnesi kullanılır
        Timer watcher;
        // Cache nesnelerinin serileştirildiği dosyaların tutulduğu klasör
        string cachePath = "C:\\Cache\\";
        // Dosya adı ve Expire zamanlarını tutan koleksiyon nesnesi. Eş zamanlı çıkartma işlemine destek verebilmek için .Net 4.0 ile gelen Concurrent koleksiyonlardan birisi kullanılmaktadır.
        ConcurrentDictionary<string, DateTime> cacheExpireList;

        // Yapıcı metod içerisinde gerekli nesne başlatma işlemleri yapılır
        public DiskCacheProvider()
        {
            cacheExpireList = new ConcurrentDictionary<string, DateTime>();

            // Timer nesne örneği 3 saniyede bir Elapsed olayını tetikleyecektir
            watcher = new Timer(3000);
            // Elapsed olayı içerisinde Expire olan output cache dosyalarının bulunması sağlanır
            watcher.Elapsed += (o, e) =>
            {
                // Koleksiyonda duran Expire zamanı ile güncel zaman karşılaştırılarak bir sonuca gidilmeye çalışılır
                var discardedList = from cacheItem in cacheExpireList
                                    where cacheItem.Value < DateTime.Now
                                    select cacheItem;
                // Expire olmaya aday olan Cache nesnelerine ait dosyalar için Remove metodu çağırılır.
                // Eğer normal bir Dictionary<T,K> koleksiyonu kullanılırsa çalışma zamanında InvalidOperationException alınabilir. Nitekim discardedList ile gezilirken cacheExpireList' in değişmiş olma ihtimali bulunabilir. Bu nedenle ConcurrentDictionary<T,K> kullanılması tercih edilmiştir
                foreach (var discarded in discardedList)
                {
                    Remove(discarded.Key);
                    // Koleksiyondan çıkartılır
                    DateTime discardedDate;
                    cacheExpireList.TryRemove(discarded.Key, out discardedDate);
                }
            };
            // Timer nesne örneği başlatılır
            watcher.Start();
        }

        // utcExpiry parametresi, Cache için Expire süresini belirtir
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            FileStream fs = new FileStream(String.Format("{0}{1}.binary", cachePath, key), FileMode.Create, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, entry);
            fs.Close();
            cacheExpireList.TryAdd(key, utcExpiry.ToLocalTime());
            return entry;
        }

        // Cache' den nesnesi elde etmek için kullanılan metoddur
        // OutputCacheProvider abstract sınıfından gelen ve ezilmesi mecburi olan bir fonksiyondur.
        public override object Get(string key)
        {
            string path = String.Format("{0}{1}.binary", cachePath, key);
            // Örnekteki sistem Cache nesne içeriklerini dosya tabanlı olarak tutmaktadır. Bu noktada dosyanın sistemde var olup olmadığına bakılarak Cache' lenen bir içerik olup olmadığı sonucuna varılabilir
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                // Ters serileştirme işlemi ile dosya içeriğinden Cache' lenen nesnenin canlandırılması sağlanır
                object result = formatter.Deserialize(fs);
                fs.Close();
                return result;
            }
            else
                return null;
        }

        // Cache nesnesinin kaldırılması için kullanılan metoddur. OutputCacheProvider abstract sınıfından gelen ve ezilmesi mecburi olan bir fonksiyondur.
        // OutputCacheProvider abstract sınıfından gelen ve ezilmesi mecburi olan bir fonksiyondur.
        public override void Remove(string key)
        {
            string path = String.Format("{0}{1}.binary", cachePath, key);
            if (File.Exists(path)) // Eğer dosya var ise Cache' lenen bir nesne var olduğu sonucuna ulaşabiliriz
            {
                // Dosya silinir
                File.Delete(path);
            }
        }

        // Set metodunda Cache in saklanması işlemleri gerçekleştirilir. Genellikle overwrite mantığına göre çalışır. Yani Cache' lenen nesne varsada üzerine yazılması yoluna gidilir
        // OutputCacheProvider abstract sınıfından gelen ve ezilmesi mecburi olan bir fonksiyondur.
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            // Cache' lenen içerik varsa overwrite işlemi yapılır.
            string path = String.Format("{0}{1}.binary", cachePath, key);

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            // Cache nesnesi serileştirilerek dosyaya yazdırılır
            formatter.Serialize(fs, entry);
            fs.Close();
            // Cache' lenen nesne serileştirildiği dosyada tutulduğundan expire sürelerini takip edebilmek için ilgili koleksiyonda bilgilendirme yapılır.
            cacheExpireList.TryAdd(key, utcExpiry.ToLocalTime());
        }
    }
}
```

Şimdi çok kısaca neler yaptığımıza bir bakalım. Cache sağlayıcımız Cache'lenecek olan nesneleri varsayılan olarak C:\Cache isimli bir klasör altında dosyalamaktadır (Aslında bu bilgide ilgili provider'a örneğin yapıcı metod yardımıyla web.config dosyası üzerinden geçirilebilir). Söz konusu nesne içerikleri ilgili dosyalara binary formatta serileştirilmektedir. Bu amaçla BinaryFormatter tipinden yararlanılmaktadır. Özellikle Cache'e nesne atma işlemi sırasında devreye giren Set metodu içerisinde, dosya adı için key parametresininin kullanıldığına dikkat edilmelidir. Hatta asıl dikkat edilmesi gereken nokta her cache için değişik bir key değerinin üretildiğidir (Ancak ispatını yapamadım henüz bunu belirteyim).

Diğer yandan Expire süreleride ilgili metodlara (örneğin Set) parametre olarak gelmektedir. Bu süre bilgilerinden yararlanılarak ön bellekten düşürme işlemleri yapılmalıdır. Örneği geliştirirken beklentilerimi boşa çıkaran noktalardan biriside aslında Expire süreleri ile ilişkilidir. Şöyleki; örneği geliştirirken Remove metodunun belirtilen Duration süresi sonlandığında otomatik olarak devreye gireceğini tahmin etmiştim. Ancak bu şekilde olmadı

![Undecided](/assets/images/2009/smiley-undecided.gif)

Bu nedenle Cache'lenen dosyaların zamanı geldiğinde silinmesi için bir mekanizmanın geliştirilmesi gerekiyordu. Bu amaçla Timer nesnesinden yararlanarak Elapsed olayı içerisinde gerekli silme işlemlerini gerçekleştirmeyi tercih ettim. Çok doğal olarak hangi dosyaların silinmesi gerektiğini anlayabilmek içinde basit bir koleksiyon tabanlı yapıyı tercih ettim. Tabiki Thread Safe bir yapı söz konusu değil. Hatta eş zamanlı gerçekleşebiliecek çıkarma işlemlerine karşın kolaya kaçıp ConcurrentCollection kullandığımı ifade edebilirim. Aslına bakarsanız şu an için tek derdim gerçekten özelleştirilmiş bu Cache sağlayıcısının çalışıp çalışmadığını görebilmek.

![Cool](/assets/images/2009/smiley-cool.gif)

Şimdi bu Cache Provider tipini web uygulamamızda nasıl kullanacağımızı belirtmemiz gerekiyor. Bu amaçla Web.config dosyası içerisinde aşağıdaki eklemeleri yapmamız yeterli olacaktır.

web.config içeriği;

```xml
<?xml version="1.0"?>
<configuration>

    <system.web>
        <compilation debug="true" targetFramework="4.0" />
      <caching>
        <outputCache defaultProvider="AspNetInternalProvider">
          <providers>
            <add name="DiskBasedCacheProvider" type="CustomCaching.DiskCacheProvider,CustomCaching"/>
          </providers>
        </outputCache>
      </caching>
    </system.web>
    <system.webServer>
      <modules runAllManagedModulesForAllRequests="true"/>
    </system.webServer>
</configuration>
```

Dikkat edileceği üzere providers elementi içerisinde DiskBasedCacheProvider isimli bir bildirim yer almakta ve geliştirdiğimiz DiskCacheProvider tipini işaret etmektedir. Bununla birlikte outputCache elementinin defaultProvider niteliğinde yer alan AspNetInternalProvider değeri, standart Asp.Net ön bellekleme sisteminin kullanılacağını ifade etmektedir. Örneğimize aşağıdaki basit Web User Control'ü ekleyerek devam edebiliriz.

Web User Control içeriği;

```text
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CurrentDateTime.ascx.cs" Inherits="CustomCaching.CurrentDateTime" %>

<%@ OutputCache Duration="20" VaryByParam="none" ProviderName="DiskBasedCacheProvider" %>
<style type="text/css">
    .style1
    {
        color: #FFCC00;
    }
</style>
<div style="background-color:Gray">
    <strong><span class="style1">Güncel Zaman :
</span>
<asp:Label ID="Label1" runat="server" Text="Label" CssClass="style1"></asp:Label>
    </strong>
</div>
```

OutputCache direktifi içerisindeki ProviderName niteliği mutlaka dikkatinizi çekmiştir. Burada DiskBasedCacheProvider isimli bir değer kullanılmaktadır. Buda bilindiği üzere web.config dosyasında yer alan özel Cache sağlayıcı tipini işaret etmektedir. Duration değeri 20 olduğu için söz konusu Web User Control içeriğinin 20 saniye süreyle tutulması söz konusudur.

Web User Control kodu;

```csharp
using System;

namespace CustomCaching
{
    public partial class CurrentDateTime : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = DateTime.Now.ToLongTimeString();
        }
    }
}
```

Ve hemen arkasından basit bir Web sayfası...

Default.aspx içeriği;

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CustomCaching.Default" %>

<%@ Register src="CurrentDateTime.ascx" tagname="CurrentDateTime" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Default Sayfası için Güncel Zaman :
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    
    </div>
    <uc1:CurrentDateTime ID="CurrentDateTime1" runat="server" />
    <p>
        İkinci Web User Control</p>
    <uc1:CurrentDateTime ID="CurrentDateTime2" runat="server" />
    </form>
</body>
</html>
```

Default.aspx kodu

```csharp
using System;

namespace CustomCaching
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = DateTime.Now.ToLongTimeString();
        }
    }
}
```

Tahmin edileceği üzere o anki zaman bilgisini hem aspx sayfası hemde Web User Control içerisinden göstererek bir karşılaştırma yapmaya çalışacağız. Öyleki; eğer Cache sağlayıcımız devreye girerse, Web User Control içeriği 20 saniyeliğine fiziki olarak tutulan bir dosyadan karşılanacak ve bu sebeple sayfanın zaman bilgisi değişirken kendisinin ki sabit olarak kalacak. Tabiki belirtilen Duration süresi kadar. Artık testlere başlayabiliriz. Size önerim indirdiğiniz örneği mutlaka Debug ederek incelemenizdir. Özellikle Set ve Get metodları ile Elpased olayında durmanızı tavsiye ederim. Ben ilk olarak aynı web sayfasına iki farklı talep gönderip aşağıdaki çıktıları elde ettim.

![blg102_Runtime1.gif](/assets/images/2009/blg102_Runtime1.gif)

Görüldüğü üzere iki farklı talep gönderilmiş ve özellikle ikince talepte Web User Control içerisindeki zamanın değişmediği görülmüştür. Çünkü bu içerik ilk talep ile birlikte 20 saniyeliğini fiziki olarak bir dosyaya serileştirilmiş ve bu zaman dilim içerisinde sürekli olarak ilgili dosyadan ters serileşerek (DeSerialization) getirilmiş bir HTML çıktısıdır. 20 saniyelik süre sona erdikten sonra sayfalarda herhangibir yeni talep oluşturmassak, ilgili fiziki dosyalarında Cache klasöründen silindiği gözlemlenir. Geliştirdiğimiz örnekte sadece Set, Get ve Remove metodları işlevsel durumdadır. Yani söz konusu vakaya göre Add metodu herhangibir sebeple çalışmamıştır.

Hemen bir noktayı aydınlığa kavuşturalım. Geliştirdiğimiz örnekte Web User Control'ün üretimi özel Cache sağlayıcısı içerisindeki serileştirme ve ters serileştirme gibi işlemlerin çıkarttığı maliyetten daha az olabilir. Yani aslında bu örneğe göre bir Cache sistemi kullanılmasına gerek duyulmaz. Bizim amacımız sadece özel bir Cache sağlayıcısının nasıl yazılabileceğini Asp.Net 4.0 Beta 2 cephesinden incelemektir. Tabiki genişletilebilir Cache sisteminin daha esnek imkanlar sunacağıda belirtilmektedir. Aslında bu konu ile ilişkili özet bir bilgiyi [asp.net](http://www.asp.net/LEARN/whitepapers/aspnet4/default.aspx)sitesinden indireceğiniz dökümanda bulabilirsiniz. Böylece geldik bir yazımızın daha sonuna. Tabiki buradaki eksikleri ve gereksinimleri en iyi değerlendirecek kişi sevgili arkadaşım [Uğur Umutluoğlu'dur (Asp.Net MVP)](http://www.umutluoglu.com/). Tekrardan görüşünceye dek hepinze mutlu günler dilerim.

[CustomCaching.rar (26,21 kb)](/assets/files/2009/CustomCaching.rar)
