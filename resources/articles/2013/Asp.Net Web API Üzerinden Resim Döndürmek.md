---
title: "Asp.Net Web API Üzerinden Resim Döndürmek"
pubDate: 2013-07-02 13:08:00
categories:
  - Asp.Net Web API
tags:
  - Asp.Net Web API
  - C#
  - .NET
  - ASP.NET
  - ADO.NET
  - Entity Framework
  - LINQ
  - Web API
  - HTTP
  - Generics
  - Visual Studio
---

# Asp.Net Web API Üzerinden Resim Döndürmek
[![Road-Runner-Wile-E-Coyote-looney-tunes-5226561-1024-768](images/Road-Runner-Wile-E-Coyote-looney-tunes-5226561-1024-768_thumb.jpg)](images/Road-Runner-Wile-E-Coyote-looney-tunes-5226561-1024-768.jpg) Merhaba Arkadaşlar,

Eminim çocukken çizgi filmlerle aranız vardı. Hatta çoğumuz yaşı kaç olursa olsun çizgi filmlere arada sırada da olsa zaman ayırmakta. (Ben Batman gördüm mü pür dikkat izlerim örneğin) Keza pek çok büyüğümüz de, eskiden izlediği çizgi filmler ile karşılaştığında taaaa çocukluk yıllarına kadar gidip aynı o zamanki gibi içten gülebiliyorlar da (Rahmetli babamdan bilirim)

Aslına bakarsanız bazen teknoloji de bizi aynen bu mantıkta epeyce güldürebiliyor. Örneğin Microsoft’ un ürünlerini düşünelim. (Gerçi çok fazlalar ama gene de düşünmeye çalışalım) Sürekli yenilikler çıkartıyorlar, sürekli verisyon atlatıyorlar ve işin en acı tarafı da koşan [Road Runner](https://eksisozluk.com/road-runner--32627)’ a benziyorlar. Biz mi? Biz ise Road Runner’ ı her fırsatta yakalamaya çalışıp yakaladığını zanneden ama son anda hep elinden kaçıran [Coyote](https://eksisozluk.com/coyote--51208)’ ye

![Smile](images/wlEmoticon-smile_100.png)

Bence bu senaryoda developer’ lar biraz daha şanslı. Ya benim gibi düzenli blog tutmaya çalışanlar napsınlar

![Disappointed smile](images/wlEmoticon-disappointedsmile_7.png)

Sözü fazla uzatmadan ve moralimizi daha da bozmadan konumuza geçelim.

Bu yazımızda Asp.Net Web API üzerinden, SQL tablolarında binary formatta tutulabilen resim içeriklerini nasıl çekebileceğimizi basit bir örnek ile incelemeye çalışıyor olacağız. Örneğimizin özel yanlarından birisi de kısa süre önce yayınlanan [Visual Studio 2013 Preview](http://www.microsoft.com/visualstudio/tur/2013-downloads) ile geliştirilecek olması. Önce senaryomuza bir bakalım.

Senaryo

Uzun zamandır uğramadığımız hatta pek çok genç arkadaşımızın belki de adını bile duymadığı bir Microsoft veritabanını ele alıyor olacağız. Pubs, SQL 2000 sürümünde sıklıkla Northwind ile birlikte andığımız kobay veritabanlarından birisidir

![Smile](images/wlEmoticon-smile_100.png)

Bu veritabanında yayıncılara ait bazı bilgiler yer almaktadır. Örneğin pubinfo isimli tablo içerisinde pubid, logo ve pr_info isimli 3 adet alan yer almaktadır. Bu alanlardan logo tahmin edileceği üzere Binary veri tipindedir ve yayıncının firma logosunu tutmaktadır.

Hedefimiz bu binary içerikleri (yani logoları) bir Web API fonksiyonu üzerinden geriye döndürebilmek ve hatta en azından tarayıcı pencresinde resim formatında gösterebilmek olacaktır. O halde projeyi açarak ilk adımımız atalım.

[![wapigi_1](images/wapigi_1_thumb.png)](images/wapigi_1.png)

Projenin Oluşturulması

İlk olarak yeni bir Web uygulaması oluşturarak işe başlayabiliriz. Pek tabi Visual Studio 2013 preview içerisinde görünen önemli özelliklerden birisi de One Asp.Net yeteneğidir. Buna göre tek bir Web uygulaması şablonu üzerinden hareket edilerek istenen kabiliyetlere göre seçimler yapılması sağlanmaktadır.

[![wapigi_2](images/wapigi_2_thumb.png)](images/wapigi_2.png)

> Doğruyu söylemek gerekirse Asp.Net tarafındaki proje şablonlarının artması kafa karışıklıkları yanında bir arada kullanmak istediğimiz kabiliyetler olduğunda da sıkıntı yaratmaktaydı. Umarız bu özellik baki olur ve daha da iyileştirilir.

Asp.Net Web Application seçimi sonrasında karşımıza gelen pencereden Empty template tipini seçip Web API özelliğini etkineleştirebiliriz. Ya da Web API özelliğini işaretleyip ilerleyebiliriz. Ben mümkün mertebe sade bir ortam arzu ettiğimden Empty template seçip Web API kutusunu işaretledim.

[![wapigi_3](images/wapigi_3_thumb.png)](images/wapigi_3.png)

Bu işlemler sonucunda solution ve proje içeriği aşağıdaki gibi oluşacaktır.

[![wapigi_4](images/wapigi_4_thumb.png)](images/wapigi_4.png)

Modelin Eklenmesi

İzleyen adımda modelimizi ilave etmemiz gerekiyor. Tahmin edeceğiniz gibi Entity Framework den yararlanıyor olacağız. Projeye yeni bir öğe olarak Ado.Net Entity Data Model nesnesi ekledikten sonra klasik adımlarımızla ilerliyoruz (Model klasörü altına ekleyebilirsiniz) Lakin Visual Studio 2013 Preview’ a has bir özellik olarak Entity Framework versiyonunu seçebileceğimiz bir ekranla karşılaşacağız (Sanırım Entity Framework tarafı kadar hızlı versiyon atlatan ürün sayısı nadirdir) Ben 6.0 sürümünü seçtim ve bunun sonucu olarak Beta 1’ in kütüphane olarak ilave edildiğini fark ettim.

[![wapigi_5](images/wapigi_5_thumb.png)](images/wapigi_5.png)

İzleyen kısımda sadece pubinfo tablosunun eşleniği olan entity üretimini yaptırmamız yeterlidir. (Diğer tablolaraı dilersenize ekleyebilirsiniz ancak şu anki senaryomuz için çok da gerekli değiller)

[![wapigi_6](images/wapigi_6_thumb.png)](images/wapigi_6.png)

Controller Tipinin Yazılması

Web API’ nin temel yapı taşı olan Controller tipini ekleyerek örneğimize devam edelim.

[![wapigi_7](images/wapigi_7_thumb.png)](images/wapigi_7.png)

Web API controller sınıfı için iki farklı versiyon bulunmaktadır. (Ben v1’i seçerek ilerledim ama bunu yaparken iki versiyon arasındaki farkı tam olarak bilmediğimi itiraf etmek isterim ![Embarrassed smile](images/wlEmoticon-embarrassedsmile_7.png))

LogosController sınıfı içeriği

```csharp
using System.Collections.Generic; 
using System.IO; 
using System.Linq; 
using System.Net; 
using System.Net.Http; 
using System.Net.Http.Headers; 
using System.Web.Http; 
using WebApplication4.Models;

namespace WebApplication4.Controllers 
{ 
    public class LogosController 
        : ApiController 
    { 
        public List<string> Get() 
        { 
            List<string> pubIds = null; 
            using (PubsEntities _context = new PubsEntities()) 
            { 
                pubIds = (from p in _context.pub_info 
                          select p.pub_id).ToList(); 
            } 
            return pubIds; 
        } 
        public HttpResponseMessage Get(string id) 
        { 
            HttpResponseMessage response = null;

            using (PubsEntities _context = new PubsEntities()) 
            { 
                var pubPicture = (from p in _context.pub_info 
                                       where p.pub_id == id 
                                       select p.logo).FirstOrDefault(); 
                if (pubPicture==null) 
                { 
                    response = new HttpResponseMessage(HttpStatusCode.NotFound); 
                } 
                else 
                { 
                    MemoryStream ms = new MemoryStream(pubPicture); 
                    response = new HttpResponseMessage(HttpStatusCode.OK); 
                    response.Content = new StreamContent(ms); 
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png"); 
                } 
            } 
            return response; 
        } 
    } 
}
```

LogosController sınıfı içerisinde iki adet Get metodu bulunmaktadır. İstemci tarafından gelecek HTTP Get taleplerine cevap verecek olan bu fonksiyonlardan birisi pubinfo tablosundaki pubid alanlarını liste olarak döndürmektedir. Diğer yandan senaryomuzun can alıcı Get metodu ise, HttpResponseMessage tipinden bir nesne örneğini döndürmektedir. Bu metod parametre olarak string tipinden olan bir pubid değerini alır. İlgili alana eş satırın logo içeriğini bulur (eğer varsa). Bu içeriğin byte[] tipinden olan karşılığı bir MemoryStream referansından yararlanılarak HttpResponseMessage örneğinin Content özelliğine set edilir.

Bundan sonra yapılması gereken, istemciye dönecek cevap içeriğinin bir image olduğunu belirtmektir. Headers.ContentType özelliğine bir MediaTypeHeaderValue örneğinin atanmasının ve parametre olarak image/png verilmesinin sebebi de budur. Çok doğal olarak ilgili id değeri yanlış girilebilir ve LINQ sorgusu bu durumda null değer üretebilir. Null değer kontrolü yapılarak böyle bir vaka oluşması halinde HTTP 404 Not Found istisnasının döndürülmesi de sağlanmaktadır (Web’ in doğasına ve isteğine uygun şekilde ![Winking smile](images/wlEmoticon-winkingsmile_210.png))

Testler

Uygulama kodunun tamamlanmasını müteakip test çalışmalarına başlanabilir. Her hangi bir tarayıcı uygulama ile bu işlemi yapabiliriz (Ben tercihimi Google Chrome’ dan yana kullandım ![Smile](images/wlEmoticon-smile_100.png)) Örneğin api/logos şeklinde bir talepte bulunulduğunda aşağıdaki ekran görüntüsüne benzer olacak şekilde pubid bilgilerinin elde edildiği görülür.

![wapigi_8](images/wapigi_8_thumb.png)

Eğer belirli bir pubid değeri için talepte bulunulursa asıl istediğimiz sonuçlara ulaşırız. Yani yayıncının logosuna

![Winking smile](images/wlEmoticon-winkingsmile_210.png)

api/logos/0736 için aşağıdaki sonuç elde edilirken

![wapigi_9](images/wapigi_9_thumb.png)

api/logos/1756 için

![wapigi_10](images/wapigi_10_thumb.png)

sonucu elde edilir. Çok doğal olarak olmayan bir pub_id için istemci tarafında HTTP 404 hatası dönecektir.

Daha Neler Yapılabilir ve Size Kalan

Senaryomuz sadece yayın evinin logosunu ve yayın evi numaralarını döndürecek fonksiyonelliklere sahip bir Asp.Net Web API hizmetini içermektedir. Ancak siz bu senaryoyu daha da geliştirebilirsiniz.

- Örneğin jQuery kullanarak yayıncıların listesinin logoları ile birlikte bir View’ da görünmesini deneyebilirsiniz.
- Kuvvetle muhtemel yukarıdaki maddeyi bir MVC projesinde denersiniz. Ama aynısını Web Forms tabanlı bir uygulama için de yapmaya çalışabilirsiniz.
- Resimlerin gösterilmesi haricinde istemcilerin yine Asp.Net Web API’ den yararlanarak upload etme işlemlerini yapabilmelerini de sağlayabilirsiniz ![Winking smile](images/wlEmoticon-winkingsmile_210.png) Bunu bir araştırmanızı öneririm. POST şeklinde bir talebi ele almanız gerektiğini ip ucu olarak verebilirim.
- Bir önceki maddede var olan kısmı birden fazla dosyayı bir seferde yükleme senaryosu için ele alabilirsiniz (Multiple Upload)
- Büyük boyutlu resimleri parça parça atmayı veya okumayı deneyebilirsiniz.
- ve benim aklıma gelmeyen ama sizin ele alacağınız başka bir senaryo da söz konusu olabilir.

Görüldüğü üzere bir Asp.Net Web API servisini resim içeriklerinin elde edilmesini konu alan bir senaryo da kullanabildik. Örneğimizi yeni göz bebeğimiz Visual Studio 2013 Preview üzerinde geliştirmeye çalıştık ve böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
