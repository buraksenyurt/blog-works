---
layout: post
title: "Core Web API  için Planlanmış Görevler"
date: 2018-02-09 04:27:00 +0300
description: "Bu makalemizde Hosted Service olarak adlandırılan ve arka plan işleri(background jobs) ya da planlanmış görevler ile ilişkilendirilmiş servislerin Web API gibi WebHost türevli uygulamalarda nasıl ve ne amaçla kullanılabileceklerini inceleme çalışıyoruz."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - rabbitmq
  - rest
  - json
  - web-api
  - http
  - async-await
  - threading
  - generics
  - dependency-injection
  - microservices
  - dependency-management
---
# Core Web API  için Planlanmış Görevler
![hostedsrv_1.gif](/assets/images/2018/hostedsrv_1.gif)

Merhaba Arkadaşlar,

[Chuck Norris](https://chucknorris.com/). Sanıyorum hayatımın bir bölümü onun televizyonda arka arkaya yayınlanan filmleri ile geçmiştir. Asıl adı Carlo Ray Norris'tir ve Chuck ismi 1958de O, hava kuvvetlerindeyken takma ad olarak ortaya çıkmıştır. Dövüş sanatları ustası olan Chuck'ın harika bir web sitesi var. Hayran kitlesi oldukça geniş. 1940 doğumlu olan film yıldızını Google aramalarında daha çok "Chuck Norris Facts" ile biliyoruz.

Hatta onun hayatına dair şakalar, olaylar, sözler o kadar popüler hale gelmiş ki, [International Chuck Norris Database](http://www.icndb.com/) isimli gönüllülük esasına göre geliştirilmiş bir hizmet bile var. Üstelik [http://api.icndb.com/jokes/random](http://api.icndb.com/jokes/random) adresine gittiğinizde rastgele bir fıkrasını veya şakasını çekebileceğiniz JSON formatlı bir REST servisi de bulunuyor. İşin aslı.Net Core tarafında Hosted Service kavramını araştırırken örnek olarak kullanılan ve günün özlü sözünü sunan örnek bir REST servisten Chuck Norris REST API hizmetine kadar geldiğimi belirtmek isterim. Üstelik onu basit bir örnekte kullanmayı da başardım. West-World'ün yeni konusu Hosted Service.

Ağırlıklı olarak masaüstü uygulamalarından aşina olduğumuz arka plan işleri (Background Worker Process diyelim) pek çok alanda karşımıza çıkıyor. Bu arka plan işlerini o an çalışmakta olan uygulamanın ana Thread'inden bağımsız işleyen iş birimleri olarak düşünebiliriz..Net dünyasında uzun zamandır paralel zamanlı çalışmalar için kullanılan Task odaklı bir çatı da mevcut. Bu tanımlamalar bir araya getirildiğinde belirli periyotlarda çalışan, çeşitli iş kurallarını işleten görev odaklı fonksiyonlar ortaya çıkıyor..Net Core dünyasına baktığımızda ise, özellikle WebHost veya Host tarafı için kullanılan Hosted Service isimli bir kavram mevcut ve bahsettiğimiz planlı işler (Scheduled Jobs) ile yakından ilişkili.

> Özellikle MicroService odaklı çözümlerde, servislerin yaşamı boyunca belli bir plana/takvime göre çalışan arka plan işleri için Hosted Service enstrümanının kullanılabileceği ifade ediliyor.

Kimi senaryolarda bir Web uygulamasının veya Web API hizmetinin çalıştığı süre boyunca arka planda işletilmesini istediğimiz planlanmış görevlere ihtiyacımız olabilir. Örneğin içeride biriken log'ların belirli periyotlarda Apache Kafka gibi bir kuyruk sistemine aktarılması, servis üzerinde işletilecek istemci talepleri sonrası veritabanında oluşan değişikliklerin aralıklarla dinlenip çeşitli görevlerin işletilmesi, ön belleğin zaman planlamasına göre temizlenmesi ve başka bir çok senaryo burada göz önüne alınabilir. Kısacası WebHost (bu makale özelinde) ayağa kalktıktan sonra yaşamı sonlanıncaya kadar geçen sürede belirli bir takvimlendirmeye göre çalıştırılmasını istediğimiz arka plan görevleri söz konusu ise Hosted Service'leri kullanabiliriz.

.Net Core 2.0 ile birlikte Hosted Service'lerin kolay bir şekilde uygulanabilmesini sağlamak amacıyla [IHostedService](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostedservice?view=aspnetcore-2.0) (Microsoft.Extensions.Hosting isim alanında yer alıyor) isimli bir arayüz gelmiş. Hatta.Net Core 2.1 ile birlikte bu arayüzden türetilmiş ve implementasyonu da içeren BackgroundService isimli abstract bir sınıfta söz konusu. Bu sınıfın temel amacı CancellationToken mekanizmasının yönetiminin kolaylaştırılması (Kontrol edin. Adı değişmiş olabilir. West-World üzerinde halen.Net Core 2.0 var olduğundan okuduğum bloglardaki gibi açık kaynak olarak sunulan bir IHostedService implementasyonunu kullanmacağım)

IHostedService arayüzü iki operasyon tanımlamakta. StartAsync ve StopAsync isimli fonksiyonlar CancellationToken türünden parametre alıyorlar. StartAsync operasyonu ana uygulama (WebHost veya Host türevli olabilir) başlatıldığında devreye girerken tam tersine StopAsync uygulama kapanırken işletilmekte. IHostedService uyarlaması tipler Middleware tarafında Dependency Injection mekanizması kullanılarak çalışma zamanına monte edilebiliyorlar. Singleton tipinde n sayıda Hosted Service'in çalışma zamanına eklenmesi mümkün. Bir başka deyişle uygulamanın yaşam döngüsüne istediğimiz kadar planlanmış işi birer servis olarak ekleyebiliriz. Dilerseniz hiç vakit kaybetmeden örnek bir uygulama ile konuyu anlamaya çalışalım. Boş bir Web API projesi şu an için işimizi görecek.

```bash
dotnet new webapi -o HowToHostedService
```

Sonrasında projemize HostedService isimli aşağıdaki sınıfı ekleyelim.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public abstract class HostedService : IHostedService, IDisposable
{
    private Task currentTask;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    protected abstract Task ExecuteAsync(CancellationToken cToken);

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        currentTask = ExecuteAsync(cancellationTokenSource.Token);

        if (currentTask.IsCompleted)
            return currentTask;

        return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        if (currentTask == null)
            return;

        try
        {
            cancellationTokenSource.Cancel();
        }
        finally
        {
            await Task.WhenAny(currentTask, Task.Delay(Timeout.Infinite,cancellationToken));
        }
    }
    public virtual void Dispose()
    {
        cancellationTokenSource.Cancel();
    }
}
```

Dikkat edileceği üzere HostedService sınıf IHostedService ve IDisposable arayüzlerini (Interface) uygulamakta. Temel görevi Hosted Service'ler ile ilişkilendirilecek olan Task'ların iptal edilme mekanizmalarının kolayca yönetilebilmesi. Dispose edilebilir bir nesne olarak tanımlandığına da dikkat edelim. Ayrıca, HostedService abstract bir sınıf. Dolayısıyla kendisini örnekleyemeyiz. Bununla birlikte kendisini uygulayan sınıfların mutlaka ezmesi gereken ExecuteAsync isimli bir metod tanımı da sunuyor. Bu metodu alt tiplerde ezerken planlanmış görevin yapacağı çalışma için kodlamamız yeterli. Parametre olarak gelen CancellationTokenSource örneği ise StartAsync üzerinden devrediliyor. StartAsync metodu alt tipin uyguladığı ExecuteAsync operasyonunu çağırıp tamamlanıp tamamlanmadığına bakıyor. StopAsync operasynonu ise iptal işleminin yönetimini gerçekleştirmekte. Yukarıda da bahsettiğim gibi bu sınıf.Net Core 2.1 ile birlikte hazır olarak gelmesi beklenen bir tip (İçinde kocaman gülümseme olan [şu makaleye](http://www.iaspnetcore.com/Blog/BlogPost/5a6d18725430ff15206d1b52/asp-net-core-background-tasks-implementing-background-taskswith-ihostedservice) göz gezdirebilirsiniz)

Artık planlanmış görevleri içerecek örnek sınıfların yazılmasına başlanabilir. Ben sadece iki sınıfı sisteme dahil edeceğim. Her ikisi de belirli aralıklarla Console ekranına bir şeyler yazacaklar. Amacımız sadece Hosted Service mekanizmasının Web API tarafındaki tesisatının nasıl kurulması gerektiğini öğrenmek olduğundan bu basit yaklaşım yeterli olacaktır (Nitekim söz konusu arka plan servisleri gerçek hayat örneklerinde gerçek iş modellerini baz alarak kurgulanmalılar) RequestCollectorService ve ChuckFactService isimli arka plan servis sınıflarımızı aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class ChuckFactService
: HostedService
{
    HttpClient restClient;
    string icndbUrl="http://api.icndb.com/jokes/random";
    public ChuckFactService()
    {
        restClient=new HttpClient();
    }
    protected override async Task ExecuteAsync(CancellationToken cToken)
    {
        while (!cToken.IsCancellationRequested)
        {   
            var response = await restClient.GetAsync(icndbUrl, cToken);
            if (response.IsSuccessStatusCode)
            {
                var fact = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"{DateTime.Now.ToString()}\n{fact}");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), cToken);
        }
    }
}
```

Ezilen ExecuteAsync metodunda icndb adresine bir talepte bulunup, talep sonucu HTTP 200 ise elde edilen sonucu ekrana bastırıyoruz. REST talebini göndermek için HttpClient tipinden yararlanmaktayız. Bu sınıfın awaitable GetAsync fonkisyonunu kullanıyoruz. Fonkisyonda dikkat edileceği üzere ilgili Task için iptal talebi olup olmadığının sürekli olarak kontrol edildiği bir while döngüsü bulunuyor. Ayrıca söz konusu görevin 10 saniyede bir işletilmesini Task tipinin Delay metodu ile sağlamaktayız. Bir başka deyişle tekrarlı görevlerin zamanlamalarını bu teknikle ayarlayabiliriz.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

public class RequestCollectorService
: HostedService
{
    protected override async Task ExecuteAsync(CancellationToken cToken)
    {
        while (!cToken.IsCancellationRequested)
        {
            Console.WriteLine($"{DateTime.Now.ToString()} Çalışma zamanı taleplerini topluyorum.");
            await Task.Delay(TimeSpan.FromSeconds(30), cToken);
        }
    }
}
```

RequestCollectorService sınıfında ise sadece ekrana bir mesaj bastırıyoruz. Çalışmasını 30 saniyede bir gerçekleştiren bir görevlendirme söz konusu. Tanımladığımız bu görev servislerini Host uygulamaya enjekte etmek için, Startup sınıfındaki ConfigureServices metodunu aşağıdaki gibi güncellememiz yeterli.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    services.AddSingleton<IHostedService,ChuckFactService>();
    services.AddSingleton<IHostedService,RequestCollectorService>();
}
```

Bu değişiklikle, AddSingleton metodunun generic versiyonunu kullanarak IHostedService uyarlamasını gerçekleştiren ChuckFactService ve RequestCollectorService sınıflarının arka plan hizmetlerine eklenmesini sağladık. Artık Web uygulaması çalışmaya başladığında bu sınıflar otomatik olarak devreye alınacak ve üzerlerindeki görevler belirlenen sürelerinde işletilecekler. Uygulamamızı çalıştırdıktan sonrasına ait örnek bir ekran görüntüsü aşağıdaki gibidir.

![hostedsrv_2.gif](/assets/images/2018/hostedsrv_2.gif)

10 saniyede bir Chuck Norris'e ait servise bir çağrı ve 30 saniyede bir ortam verilerini toplama işlemi gerçekleşmektedir. Bu sırada Web API servisinin normal hizmetini sürdürdüğünü de ifade edelim. Yani gelen talepleri karşılar haldedir. Görüldüğü üzere arkaplan görevlerinin Web tabanlı uygulamalarda konuşlandırılması oldukça kolay..Net Core tarafının Dependency Injection mekanizması da bu işi basitleştirmekte. Microservice odaklı çözümlerde bu teknikten yararlanılarak arka plan görevlerinin tesis edilmesi kolaylıkla sağlanabilir. Hosted Service tipleri Task'ların yürütüldüğü noktalarda asenkron çalışan dış sistemlerle entegre olabilirler (RabbitMQ, Kafka, MSMQ. Azure Service Bus, WSO2 vb) Benim için yine keşfedilmesi, çalışılması, uygulanması ve öğrenilmesi keyifli bir konuydu. Bir başka makalede görüşünceye dek hepinize mutlu günler dilerim.