---
title: ".Net Core Tarafında SignalR Kullanımı"
pubDate: 2017-12-31 21:02:00
categories:
  - Asp.Net Core
tags:
  - Asp.Net Core
  - Bash
  - C#
  - .NET
  - ASP.NET
  - HTTP
  - Threading
  - Concurrency
  - Generics
  - Visual Studio
  - GitHub
---

# .Net Core Tarafında SignalR Kullanımı
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Bir süre önce araştırmaya başladığım ama araya giren diğer konular (WebSockets ve CORS-Cross Origin Resource Sharing) nedeniyle askıda kalan SignalR mevuzusuyla ilgili West-World'de haftasonu önemli ve heyecanlı gelişmeler oldu. Epey zorlandığımı itiraf etmek isterim. Bunun en büyük sebebi standart öğretilerde yer alan web tabanlı örnekler yerine her şeyi Console üzerinde uygulamaya çalışmamdı. HUB için bir sunucu, mesaj yayını için bir başka uygulama ve yayınlanan mesajları alan bir diğeri.

Alınan sayısız çalışma zamanı hatası ve uykusuz bırakan birkaç saatin ardından sonunda konuyu bir şekilde toparlamayı başardım. Bu hataları gidermeye çalışırken farkına vardığım bir çok şey de oldu. SignalR'ın çalışma yapısı haricinde sunucu ve istemci taraflarının birbirleri üzerinden fonkisyon tetiklemesi noktasında nasıl bir yol izlediklerini.Net Core cephesinden görmüş oldum. Tüm bu didinmenin ardından güzel bir uyku çektim ve ertesi gün gelecekteki kendime not bırakmak için geçtim bilgisayarımın başına. Öncelikle çalıştığım kaynaklardan yararlanarak aşağıdaki özet şekli oluşturdum.

![image.axd](images/image.axd)

Şekilden de görüldüğü üzere olayın ana noktasında bir HUB yer alıyor. Bunu host eden bir uygulama kendisine bağlı olan diğer uygulamaların aynı anda etkileşimde olmasına olanak tanıyor. Genellikle meydana gelen bir olay sonrası (stok hareketlerindeki değişim, oyun ağına bağlı oyunculardan birisinin yaptığı bir hamle, kanala atılan ortak bir mesaj-hey gidi MIRC vb) hub üzerine bırakılan mesaj, bağlı olan tüm istemcilere iletilmekte. İstemciler basit web tarayıcıları üzerindeki uygulamalar olabileceği gibi, mobil çözümler, terminal programları vb de olabilir. Kritik olan nokta HUB sunucusunun kendisine bağlı istemciler üzerinde fonksiyon çağırabilmesi. Tam tersi iletişim zaten hepimizin aşina olduğu bir durum. SignalR'ın bu oluşum içerisindeki veri alışverişini kolaylaştıracak nimetler sunduğunu ifade edebiliriz. Neredeyse her tipten istemci için yazılımış kütüphaneler mevcut.

Şeklin sağ tarafında SignalR Hub ile istemci arasındaki temel iletişim sırası da yer alıyor. İstemci öncelikle HUB'a bağlantı isteği gönderiyor. Bağlantı başarılı bir şekilde sağlanırsa arada kalıcı bir iletişim hattı tesis ediliyor. Sonrasında HUB ile kullanıcısı arasında bir etkileşim süregeliyor. Burada istemci HUB üzerine mesaj bırakabileceği gibi dinleme modunda da çalışabiliyor. Dinleme modu ağırlıklı olarak HUB'a mesaj bırakan bir başka istemcinin mesajını almak gibi de düşünülebilir.

SignalR, Asp.Net tarafında uzun zamandır beri var olan bir konu olmasına rağmen,.Net Core tarafında halen Alpha sürümü olarak yer alıyor (Yazıyı okuduğunuz tarih itibariyle kontrol etmenizi öneririm. Release versiyon çıkmış olabilir) Tabii son sürüm yayınlanmadan bir gerçek hayat projesinde kullanmak pek doğru olmayacaktır.

HUB Sunucusu

Sözü fazla uzatmadan örneğimize geçelim dilerseniz. Senaryomuz standartların biraz dışında. Konuyu kavrayabilmek açısında HUB örneğini sunan bir sunucumuz, buraya abone olup mesaj yayını yapacak ve dinleyecek olan istemcilerimiz birer Console uygulaması olacak. İşe ilk olarak sunucu tarafını yazarak başlayalım (Bu arada örneği Ubuntu üzerinde Visual Studio Code ile geliştirdiğimi belirtmek isterim)

```bash
dotnet new console -o FabrikamServer
```

komutu ile FabrikamServer isimli bir Console uygulaması oluşturalım. SignalR kullanımı için Microsoft.AspNetCore.All ve Microsoft.AspNetCore.SignalR.1.0.0-alpha2-final paketlerine ihtiyacımız bulunuyor. Aşağıdaki terminal komutları ile bunları projemize dahil edebiliriz.

```bash
dotnet add package Microsoft.AspNetCore.All
dotnet add package Microsoft.AspNetCore.SignalR -v "1.0.0-alpha2-final"
```

Şu an için versiyon bilgisini belirtmemiz önemli. İşlerin yolunda gittiğinden emin olmak için belki bir restore ve build işlemi uygulamakta yarar olabilir. Uygulamada bir kaç sınıfımız var. Web çalışma ortamının ayağa kaldırılmasında kullanacağımız Startup.cs ve Hub görevini üstlenecek olan QutoeHub.cs. Evet tahmin edeceğiniz üzere oyunlardan beğendiğim bir kaç sözün yayınlanmasını sağlayacağız. Tabii online olan istemcilere.

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

public class QuoteHub 
    : Hub
{
    public string Send(string incomingQuote)
    {   
        Clients.All.InvokeAsync("GetQuote",incomingQuote);
        return $"[{Context.ConnectionId}]: {incomingQuote}";
    }
}
```

Hub sınıfından türetilmiş olan QuoteHub tipinde Send isimli bir metod yer alıyor. Bu metod o an bağlı olan istemci için üretilen GUID değeri ile birlikte parametre olarak gelen içeriği geriye döndürmekte. Ancak önemli bir görevi daha var. Bağlı olan ne kadar istemci varsa hepsindeki GetQuote operasyonunu tetiklemek. Buna göre, HUB'a bağlı olan bir istemci, Send mesajını kullanarak bir bilgi bıraktığında (ki senaryomuzda bu güzel bir oyun cümlesi), dinlemede olan ne kadar istemci varsa iletilecek. İşte basit bir broadcasting senaryosu. Startup sınıfı ile devam edelim.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
public class Startup
{
    public void ConfigureServices(
        IServiceCollection services)
    {
        services.AddSignalR();
    }

    public void Configure(
        IApplicationBuilder app, 
        IHostingEnvironment env)
    {
        app.UseSignalR(routes =>
        {
            routes.MapHub<QuoteHub>("QuoteHub");
        });
    }
}
```

Startup sınıfında tipik olarak route tanımlamasını yapıyor ve Middleware'e SignalR kabiliyetlerini ekliyoruz. Tahmin edileceği üzere istemciler belli bir HUB adresine doğru gelmeliler. Buradaki eşleştirme MapHub fonksiyonu ile belirtiliyor. Uygulamanın Program.cs içeriği ise şu şekilde geliştirilebilir.

```csharp
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace FabrikamServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()            
            .Build()
            .Run();
        }
    }
}
```

Tipik olarak Kestrel çalışma zamanının ayağa kaldırıldığını ifade edebiliriz.

Yayıncının Geliştirilmesi

Gelelim mesaj yayını yapacak olan uygulamaya. Aslında bu da bir nevi istemci gibi düşünülebilir. Tek farkı sadece Send operasyonunu çağıracak ve GetQuote ile ilgili hiçbir şey yapmayacak olması. FabrikamPostman isimli Console projesinde Microsoft.AspNetCore.SignalR.Client paketinin 1.0.0-alpha2-final sürümünün kullanılması gerekiyor (Bu Client versiyonu dikkat edin) Bu nedenle ilgili paketi hem bu hem de biraz sonra yazacağımız Console projelerine aşağıdaki komutu kullanarak eklemeliyiz.

```bash
dotnet add package Microsoft.AspNetCore.SignalR.Client -v "1.0.0-alpha2-final"
```

Program.cs içeriğini aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace FabrikamPostman
{
    class Program
    {
        static void Main(string[] args)
        {   
            HubConnection conn = new HubConnectionBuilder()
                 .WithUrl("http://localhost:5000/QuoteHub")
                 .WithConsoleLogger()
                 .Build();

            conn.StartAsync().ContinueWith(t=>{
                if(t.IsFaulted)
                    Console.WriteLine(t.Exception.GetBaseException());
                else
                    Console.WriteLine("Connected to Hub");

            }).Wait();

            conn.On<string>("GetQuote", param => {                
            });
            
            for(int i=0;i<10;i++)
            {   
                Random random=new Random();
                int index=random.Next(0,QuoteFabric.GetQuotes().Count);
                conn.InvokeAsync<string>("Send",QuoteFabric.GetQuotes()[index].ToString())
                .ContinueWith(t=>{
                    if(t.IsFaulted)
                        Console.WriteLine(t.Exception.GetBaseException());
                    else
                        Console.WriteLine(t.Result);
                });
                Thread.Sleep(10000);
            }

            conn.DisposeAsync().ContinueWith(t=>{
                if(t.IsFaulted)
                    Console.WriteLine(t.Exception.GetBaseException());
                else
                    Console.WriteLine("Disconnected");
            });
        }
    }
}
```

İlk olarak bir HubConnection nesnesi örneklenmekte. WithUrl fonkisyonuna parametre olarak geçilen adrese dikkat edelim. Az önce yazdığımız sunucunun yayın yaptığı adresteki yönlendirme bilgisine göre belirlenmiş durumda. Console penceresine log bırakacağımızı da belirtmekteyiz. İşlemleri izlemek keyifli olacak.

StartAsync operasyonu ile bağlantının açılmasını sağlıyoruz. On metodu GetQuote için yapılan çağrıları dinlemek için ele alınmakta. Ancak az öncede belirttiğimiz gibi bu operasyonu mesaj yayını yapan programımız için kullanılmıyor. "O zaman yazmasaydın?" dediğinizi duyar gibiyim. Hemen QuoteHub üzerindeki Send fonksiyonuna dönelim o zaman. Orada tüm bağlı istemciler GetQuote çağrısı söz konusu. Burası da bir istemci olduğu için ilgili operasyonun bulunması gerekiyor. Aksi durumda çalışma zamanı hatası alırız.

10 elemanlı döngü içerisinde yapılan InvokeAsync çağırısı önemli. İlk parametre dikkat edileceği üzere QuoteHub sınıfındaki Send metodunun adı. Sonrasında ise göndereceğimiz parametre söz konusu. Burada QuoteFabric (github üzerinden yayınladığım proje kodlarını [bu adresten](https://github.com/buraksenyurt/dotnetcore/tree/master/signalr) çekebilirsiniz) sınıfı içerisinde yer alan bir kaç özlü sözden rastgele seçilen birisinin gönderildiğini ifade edebiliriz. Send metodu geriye de bir string veri döndürdüğünden ContinueWith içerisinden bu sonucu yakalamamız da mümkün (t.Result)

Bu 10 cümlelik görev tamamlandıktan sonra DisposeAsync çağrısı yapılarak bağlantının kesilmesi sağlanıyor.

Tipik İstemci

Son olarak istemci tarafını yazabiliriz. İstemci Send metodu ile bir çağrı yapmayacak. Sadece GetQuote mesajlarını dinleyecek. Console olarak oluşturacağımız projeye Microsoft.AspNetCore.SignalR.Client paketini ekledikten sonra Program.cs içeriğini aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace FabrikamPostman
{
    class Program
    {
        static void Main(string[] args)
        { 
            HubConnection conn = new HubConnectionBuilder()
                 .WithUrl("http://localhost:5000/QuoteHub")
                 .WithConsoleLogger()
                 .Build();

            conn.StartAsync().ContinueWith(t=>{
                if(t.IsFaulted)
                    Console.WriteLine(t.Exception.GetBaseException());
                else
                    Console.WriteLine("Connected to Hub");

            }).Wait();              

            conn.On<string>("GetQuote", param => {
                Console.WriteLine(param);
            });

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            conn.DisposeAsync().ContinueWith(t=>{
                if(t.IsFaulted)
                    Console.WriteLine(t.Exception.GetBaseException());
                else
                    Console.WriteLine("Disconnected");
            });
        }
    }
}
```

Bir önceki uygulamadan pek bir farkı yok neredeyse. Yine bir HubConnection nesnesi örnekleniyor, StartAsync ile bağlantı açılıyor ve On olay metodu ile GetQuote için yapılan mesaj yayınları dinleniyor. Bir tuşa basıldıktan sonra da aradaki iletişim kopartılıyor. Tabii GetQuote operasyonuna gelecek olan mesajlar QuoteHub'daki Send metodu içerisinden yayınlanmakta (Clients.All.InvokeAsync ("GetQuote",incomingQuote))

Artık bir kaç test yapabiliriz. İlk olarak HUB sunucusunu sonra yayın yapan istemciyi ve son olarak da dinleyici rolündeki programı çalıştıralım. Dinleyici rolündeki programdan bir kaç tane çalıştıraraktan da sonuçları irdeleyebiliriz.

![image.axd](images/image.axd)

Ekran görüntüsünden de görüldüğü üzere ben iki istemci (FabrikamSomeClient) çalıştırarak sonuçları değerlendirdim. FabrikamPostman üzerinden yayınlanan sözler, bağlı olan tüm istemcilere ulaştırıldı. Ayrıca FabrikamServer üzerindeki log izlerine bakıldığında bağlanan herbir uygulama için benzersiz Guid üretildiği de gözlemlendi. SignalR'ın WebSocket modelini baz alan eş zamanlı haberleşebilme yeteneklerini kolaylaştıran yanlarını az çok bu örnekle anlamış bulundum. Umarım sizler için de anlaşılır olmuştur. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
