---
title: "WCF RIA Services - Custom Authorization [Beta 2]"
pubDate: 2010-01-26 01:52:00
categories:
  - WCF Eco System
  - WCF RIA Services
tags:
  - WCF Eco System
  - WCF RIA Services
  - C#
  - .NET
  - ASP.NET
  - LINQ
  - WCF
  - XML
  - Authorization
---

# WCF RIA Services - Custom Authorization [Beta 2]
![blg116_Giris.jpg](images/blg116_Giris.jpg)

Merhababa Arkadaşlar,

Geçtiğimiz günlerde uzun süredir yemediğim şu meşhur Dunkin & Donuts'tan bir iki kurabiye almak istedim.

![Wink](images/smiley-wink.gif)

Ansızın gelen bu dayanılmaz istek üzerine oturduğumuz semte en yakın dükkanına gidip hem kendim hemde eşim için bir kaç tane aldım. Sonrası malum...Yanında güzel bir kahve ve harika bir tat...Tattıları afiyetle mideye indirdikten sonra evde sessiz ve sakin bir ortamın olduğunu farkettim. Bizim azman ufaklık uyumuş ve gıkı bile çıkmıyorken, yorduğu eşim divanda mışıl mışıl sızmıştı.

Herkesin böyle huzurlu bir ortamı hakkettiğini düşünürken, yağmur damlalarının cama vuruşunu izliyordum. Derken ansızın bir ilham geldi ve bloğuma bir şeyler yazmamın iyi olacağı kanısına vardım. Nede olsa gerekli glikoz yüklemesi fazlasıyla yapılmıştı. Günlüğüme yazılacaklar listeme baktığımda, sıradaki konunun WCF RIA Service'lerinde özel yetkilendirme niteliklerinin (Custom Authorization Attributes) nasıl yazılacağının anlatılması olduğunu farkettim. Neyseki hafta içi bu konu ile ilişkili olaraktan internetteki az sayıda kaynaktan bilgi edinmiştim. Tabiki en güncel ve geçerli kaynak her zamanki gibi MSDN'di.

Hatırlayacağınız gibi bir önceki yazımızda niteliklerden yararlanarak yetkilendirme işlemlerinin nasıl yapılabileceğini basit bir örnek üzerinden incelemeye çalışmıştık. Buna göre önemli olan nokta, nitelik (Attribute) yardımıyla çalışma zamanının nasıl davranacağının belirlenebilmesidir. Ancak bazen, yetkilendirme (Authorization) kontrolü için özel durumların ele alınması da gerekebilir. Nitekim sadece istemcinin içinde bulunduğu role göre karar vermenin dışında yapılması gereken yetki kontrolleri söz konusu olabilir. Böyle bir durumda çalışma zamanının anlayacağı kendi niteliklerimizi yazmamız gerekmektedir.

Çok doğal olarak bu geliştirme işleminde yazılacak olan nitelik tipinin, çalışma zamanının anlayacağı ve kullanacağı bir veya daha fazla operasyonu uygulaması şarttır. Burada tipik olarak, çalışma zamanının değerlendireceği fonksiyonellikleri barındıran bir ata nitelik sınıfından yapılacak olan türetme (Inherit) işleminden bahsettiğimizi ifade edebiliriz. Dilerseniz konuyu daha iyi anlamak için bu yazımızda geliştireceğimiz örnek senaryomuzdan kısaca bahsedelim.

Senaryomuza göre Domain Service içerisinde tanımlanmış olan herhangibir operasyonun gerçekleştirilmesi sırasında, talepte bulunan kullanıcının içerisinde bulunduğu role değil, adının özel olarak tutulan yasaklı bir listede bulunup bulunmadığına bakılması durumu ele alınmaktadır. Bu yasaklı listenin ASP.NET Membership tarafından hazır olarak tutulan veritabanına extend edilmiş bir tablo içerisinde tutulması mümkündür. Bunun dışında dosya tabanlı olarak XML veya basit Text formatında dahi tutulabilir. Hatta olayı biraz abartıp Windows Registry ayarlarında dahi tutulması ve benzer saklama alanları düşünülebilir.

Hatta bu listenin bir uzak sunucuda duruyor ve ancak servis bazlı bir operasyon yardımıyla kontrollerin yapılabiliyor olması da söz konusu olabillir. Tabiki dosya tabanlı olan saklama şekli bu seçenekler arasında en az güvenli olanıdır. Nitekim dosyanın güvenliğini sağlamak, veritabanı içerisindeki bir tabloya göre çok daha zor olabilir. Ancak amacımız yetkilendirme işlemi için özel nitelik yazılması ve kullanılması olduğundan bebek adımlarıyla ilerleyeceğiz. Bu nedenle geliştireceğimiz örnekte basit olarak yasaklı listenin bir Text dosyada düzenli olarak tutulduğunu varsayıyor olacağız.

Buna göre System.Web.DomainServices.AuthorizationAttribute niteliğinden türettiğimiz tipin içerisinde yer alan Authorize metodu içerisinde gelen kullanıcı adının, yasaklı listede olup olmadığını kontrol etmemiz yeterli olacaktır. İşte sunucu tarafında yer alan CheckBannedListAttribute sınıfı içeriğimiz.

![blg116_ClassDiagram.gif](images/blg116_ClassDiagram.gif)

```csharp
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DomainServices;

namespace ChinookCustomAuthorization.Web
{
    public class CheckBannedListAttribute
        :AuthorizationAttribute
    {
        public override bool Authorize(System.Security.Principal.IPrincipal principal)
        {
            string filePath = HttpContext.Current.Server.MapPath("~\\BannedList.txt");
            string[] bannedList=File.ReadAllLines(filePath);
            return !bannedList.Contains(principal.Identity.Name);
        }
    }
}
```

CheckBannedListAttribute sınıfı AuthorizationAttribute tipinden türemektedir. Buna göre sınıf diagramı görüntüsünden de fark edileceği üzere, abstract olan Authorize metodunu ezmek zorundadır. Authorize metodu, niteliğin uygulanacağı metodlar için gerekli yetki kontrolü operasyonunu üstlenmektedir. Dikkat edileceği üzere Authorize metodu parametre olarak tanıdık bir arayüzü almaktadır; IPrincipal. Bu arayüze gelen çalışma zamanı referansından yararlanarak sisteme giriş yapan kullanıcının adını, hangi rolde yer aldığını, doğrulanıp doğrulanmadığını öğrenebiliriz.

Biz örnek senaryomuza göre Login olan kullanıcı adının yasaklı listenin tutulduğu text tabanlı dosya içerisinde olup olmadığını incelemeyi hedefliyoruz. Bu sebepten Authorize metodu içerisinde BannedList isimli ve sunucu proje içerisinde tutulan text tabanlı dosyanın tüm satırlarının yüklenmesi ve elde edilen dizi içerisinde olup olmadığına göre bool tipte bir sonucun döndürülmesi söz konusu. Yazmış olduğumuz özel nitelik tipini sunucu tarafında yer alan ChinookDomainService sınıfı içerisinde uygulamamız ise son derece kolay.

> Örneğimizin kalan kısmı daha önceki yazımızda geliştirdiğimiz ile benzer. Bu nedenle detaya girerek konudan uzaklaşmamayı tercih etmekteyim.

```csharp
namespace ChinookCustomAuthorization.Web
{
    using System.Linq;
    using System.Web.DomainServices;
    using System.Web.DomainServices.Providers;
    using System.Web.Ria;

    [RequiresAuthentication]
    [EnableClientAccess()]
    public class ChinookDomainService 
        : LinqToEntitiesDomainService<ChinookEntities>
    {
        [CheckBannedList]
        public IQueryable<Album> GetAlbums()
        {
            return this.ObjectContext.Albums;
        }
    }
}
```

Görüldüğü gibi CheckBannedList niteliği, yasaklı liste yetki kontrolü yapılmak istenen operasyonun üzerinde uygulanmaktadır. Buna göre çalışma zamanında Login olan bir kullanıcının söz konusu GetAlbums operasyonunu talep etmesi halinde devreye girecektir. Söz gelimi text dosyamız içerisinde aşağıdaki isimlerin yer aldığını varsayalım.

![blg116_Banned.gif](images/blg116_Banned.gif)

Buna göre örneğin buraks isimli kullanıcı ile Login olup albüm listesinin yüklendiği operasyonu talep ettiğimizde, tarayıcı uygulama üzerinde aşağıdaki script hatasını aldığımızı görürüz.

![blg116_Error.gif](images/blg116_Error.gif)

Görüldüğü üzere Access Denied kelimeleri hata mesajı içerisinde yer almaktadır. Çok doğal olarak yasaklı liste içerisinde yer almayan bir kullanıcı ile sistem girdiğimizde (örneğin senaryomuza göre bill olabilir) albüm listesinin başarılı bir şekilde elde edildiği görülecektir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![blg116_Complete.gif](images/blg116_Complete.gif)

Sonuç olarak eklediğimiz özel authorization niteliği yardımıyla, Domain Service üzerinden çağırılacak bir operasyon için, sisteme giriş yapan kullanıcının rolüne bakmatan farklı bir yetkilendirme kontrolü gerçekleştirebildiğimizi görmüş olduk. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[CustomAuthorization.rar (1,29 mb)](assets/CustomAuthorization.rar)
