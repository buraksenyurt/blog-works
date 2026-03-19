---
title: "Eğlenceli SignalR"
pubDate: 2018-10-31 21:30:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - .NET
  - LINQ
  - JSON
  - HTTP
  - JavaScript
  - Async/Await
  - Generics
  - GitHub
---

# Eğlenceli SignalR
![funnys_0.jpg](images/funnys_0.jpg)

Merhaba Arkadaşlar,

Önce üç yumurtayı derin kabın içerisine kırıp el mikseri ile güzelce karıştırdım. Bir su bardağının 4te 3ü kadar şeker ilave edip karıştırmaya devam ettim. Şeker iyice eriyene kadar. Sonra bir su bardağı kadar zeytinyağı, iki su bardağı kadar esmer un katıp (unu önce el eleğine aldım oradan kaba boşalttım. Unu havalandırarak eklemek daha iyi sonuç veriyor çünkü) kulak memesi kıvamına gelinceye kadar çırpmaya devam ettim. Kıvamı ayarlamak için yer yer göz kararı bir miktar daha un eklemem de gerekti. Acemilik tabii...

Bir süre sonra doktor ötker'in kabartma ve vanilya tozlarından birer paketi kaba boşalttım. Kıvam istediğim hale geldikten sonra rendeledeğim limon kabuklarını ve suyunu döküp çok az daha karıştırdım. Karışımı döktüğüm kabı yüksek ısıya dayanıklı eldivenlerimle tuttum ve 180 derece sıcaklığa ayarladığım fırının üst katına yerleştirdim. Yarım saat kadar bekledim ve soununda üstü kahverengileşmiş, hafiftçe de kabarmış limonlu kekim hazırdı. Onu büyük bir keyifle yapmıştım. Tarif önümde hazırdı ve bu nedenle yapması oldukça kolaydı. Ancak mutluluğumun asıl sebebi keki hazırlamaya başlamadan önce bitirdiğim örnek uygulamaydı.

Bu yazımızda, Chart.js kütüphanesini kullanarak tarayıcı üzerindeki bir grafiğin SignalR üzerinden nasıl beslenebileceğini incelemeye çalışacağız. Bunu yaparken Chart.js isimli Javascript kütüphanesinin nefis özelliklerini kullanacağız ama daha da önemlisi verilerin belirli periyotlarla istemci tarafına akmasını ve grafik üzerinden anlık (SignalR kullanmamızın bir sebebidir) olarak izlenebilmesini sağlayacağız. Tarifimiz pratik ve uygulaması kolay.

Öncelikle senaryomuzdan bahsedelim. Kurum için kullanılan servislerin anlık olarak karşıladıkları talep bilgilerini gerçek zamanlı olarak raporlamak istiyoruz. Raporlama aracımız basit bir HTML sayfası üzerindeki çizgi grafiği (Line chart) Bu grafiğin anlık olarak (saniyede bir örneğin) değişmesini istiyoruz. Önemli noktalardan birisi grafiğin ihtiyaç duyacağı veriyi dinlemede olan istemcilere sunmak. Burada SignalR'dan yararlanabiliriz.

Buna göre örneğimiz üç uygulamadan oluşacak. Birisi Hub rolünü üstlenecek ve veri akışına aracılık edecek. Bu tahmin edeceğiniz üzere bir Web uygulaması olacak ve grafik fonksiyonelliğini içeren HTML içeriğini de barındıracak (ama istemci aynı uygulama üzerinde olmak zorunda değil tabii) Bir diğer uygulama ise veri beslemesi için kullanılacak. Yani Hub'ı dinleyen istemcilere rastgele veriler sunacak. Onu Console olarak tasarlayabiliriz. Her iki uygulamanın ihtiyaç duyduğu verileri tanımlayan ortak bir sınıf kütüphanesi de üçüncü projemiz olarak karşımıza çıkacak. Dilerseniz hiç vakit kaybetmeden başlayalım.

Önce veri modelini içeren projemizi oluşturalım.

```bash
dotnet new classlib -o ServiceSensor.Common
dotnet add package Newtonsoft.json
```

ServiceSensor.Common kütüphanesi HealthInformation isimli bir sınıf içermekte ve JSON serileşmesinde alan adlarının nasıl olacağı ifade edilmekte. Bunu belirtmezsek istemci tarafında name ve level (küçük harf ile başlıyorlar) isimlerini kullanmamız gerekir. Zorunluluk değil ama standartlara uyum açısından JsonProperty kullanmak iyi olur.

```csharp
using System;
using Newtonsoft.Json;

namespace ServiceSensor.Common
{
    public class HealthInformation
    {
        [JsonProperty("serviceName")]
        public string Name { get; set; }
        [JsonProperty("healthLevel")]
        public int Level { get; set; }
        public override string ToString() => $"{Name} [{Level}]";
    }
}
```

HealthInformation nesne örnekleri servis adı ve o anki seviyesini (anlık durumunu işaret eden bir gösterge gibi düşünelim) tutmak için planlandılar. Şimdi sunucu tarafını geliştirelim.

```bash
dotnet new web -o ServiceSensor.House
dotnet add package Microsoft.AspNetCore.All
dotnet add reference ..\ServiceSensor.Common\
dotnet build
```

Öncelikle ServiceSensor.House isimli boş bir Web projesi oluşturuyoruz. Sonrasında SignalR yeteneklerinden faydalanmak için bir paket ekliyoruz (.Net Core'un son sürümünde buna gerek olmayabilir lütfen kontrol edin) Pek tabii HealthInformation sınıfını kullanabilmek için de ServiceSensor.Common kütüphanesinin referans edilmiş olması lazım. İlk olarak Hub sınıfını tasarlayalım.

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ServiceSensor.Common;

namespace ServiceSensor.House
{
    public class HealthSensorHub
        : Hub
    {
        public Task Broadcast(string sender, HealthInformation information)
        {
            return Clients.AllExcept(new[] { Context.ConnectionId })
                .SendAsync("Broadcast", sender, information);
        }
    }
}
```

HealthSensorHub sınıfının temel görevi bağlı olan tüm istemcilere parametre olarak gelen information içeriğini asenkron olarak göndermek. Hub türevli olduğu için bu yeteneğe haiz. Üst sınıftan Clients özelliğini kullanıyoruz. AllExcept nedeniyle gelen tüm istemci talepleri kabul edilmekte ve bağlananların her birine Broadcast takma adı üzerinden iki bilgi gönderilmekte. Sender ve Information. Gelelim program sınıfının kodlarına.

```csharp
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ServiceSensor.House
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://localhost:7001");
    }
}
```

Aslında root klasörün kullanılacağını (sonradan index.html ekleyeceğiz) ekledik ve web uygulamamızın http://localhost:7001 adresinden yayın yapacağını ifade ettik. Startup.cs içeriğini de aşağıdaki gibi tasarlamamız gerekiyor.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceSensor.House
{
    public class Startup
    {
       public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddCors(o =>
            {
                o.AddPolicy("All", p =>
                {
                    p.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseFileServer();
            app.UseCors("All");

            app.UseSignalR(routes =>
            {
                routes.MapHub<HealthSensorHub>("/healthSensor");
            });
        }
    }
}
```

İlk olarak SignalR servisini çalışma zamanına ekliyoruz. Cross-Origin Resource Sharing (CORS) ihlallerine takılmamamk için eklediğimiz birkaç kod parçası daha var. SignalR için gerekli route işaretlemesini yaparken HealthSensorHub sınıfının kullanılacağını belirtiyor ve healthSensor (istemci tarafı SignalR bağlantısını kurabilmek için bu path bilgisini kullanacak) isimli adres bilgisini veriyoruz.

Web uygulamamıza chart.js kütüphanesini ve HTML dosyasını eklemek için yeniden döneceğiz. Ancak öncesinde bağlı olan istemcilere veri sağlayacak uygulamamızı geliştirerek devam edelim. Bunu bir Console uygulaması olarak geliştirebiliriz. Tek yapacağı belirli periyotlarla servislere ait güncel sağlık bilgilerini göndermek olacak ki işimizi kolaylaştırmak için rastgele değerler ürettireceğiz.

```bash
dotnet new console -o ServiceSensor.Publisher
dotnet add package Microsoft.AspNetCore.SignalR.Client
dotnet add reference ..\ServiceSensor.Common\
dotnet build
```

SignalR ile konuşacağımız için Microsoft.AspNetCore.SignalR.Client paketini eklememiz gerekiyor. Artık program sınıfının kodlarını yazabiliriz.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using ServiceSensor.Common;

namespace ServiceSensor.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var cToken = new CancellationTokenSource();
            Task.Run(() => SendInformationAsync(cToken.Token)
                            .GetAwaiter()
                            .GetResult(), cToken.Token);

            Console.WriteLine("Sonlandırmak için bir tuşa basın...");
            Console.ReadLine();
            cToken.Cancel();
        }

        static async Task SendInformationAsync(CancellationToken cToken)
        {
            var connBuilder = new HubConnectionBuilder()
                .WithUrl("http://localhost:7001/healthSensor")
                .Build();

            await connBuilder.StartAsync();
            Random rnd = new Random();
            int randomHealthPoint = 0;

            while (!cToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cToken);
                randomHealthPoint = rnd.Next(1, 25);
                var information = new HealthInformation() 
                    {
                         Name = $"service_{randomHealthPoint}", 
                         Level = randomHealthPoint
                     };
                Console.WriteLine(information.ToString());
                await connBuilder.InvokeAsync("Broadcast", "HealthSensor", information, cToken);
            }

            await connBuilder.DisposeAsync();
        }
    }
}
```

Programın temel görevi kullanıcı kesene kadar asenkron bir işin yürütülmesini sağlamak. SendInformationAsync içerisinde ise önemli işler icra ediliyor. Öncelikle http://localhost:7001/healthSensor adresini kullanan bir HubConnecton nesnesi örnekleniyor. StartAsync metodu ile iletişim başlatılıyor. Kurulan sonsuz döngü içerisinde 1 saniyelik aralıklarla rastgele HealthInformation nesneleri örneklenmekte. Bu nesneler InvokeAsync metodundan yararlanılarak Hub'a bağlı olan istemcilere dağıtılıyor. Kullanıcı SendInformationAsync fonksiyonunu kesene kadar bu gönderim işlemi devam edecek.

Artık ServiceSensor.House isimli Web uygulamamıza yeniden dönebiliriz. Öncelikle grafik için chart.js ve SignalR tarafı ile konuşmak için de aspnet/signalr paketlerini yüklememiz lazım. Bu yüklemeler içn npm aracından yararlanabiliriz.

```bash
npm install chart.js --save
npm install @aspnet/signalr --save
```

Paketler varsayılan olarak tüm içerikleri ile birlikte inerler. Ancak ihtiyacımız olan dosyalar sadece chart.js ve signalr.js. Bu dosyaları alıp wwwroot altında açacağımız scripts klasörü içerisine kopyalayabiliriz. Gelelim eğlenceyi oluşturacak kısıma. wwwroot altındaki index.html dosyasını aşağıdaki gibi kodlayarak devam edelim.

```text
<html>

<head>
    <meta charset="utf-8" />
    <title>Service Health Sensor Sample</title>
    <script src="scripts/chart.js"></script>
    <script src="scripts/signalr.js"></script>
    <script type="text/javascript">

        document.addEventListener('DOMContentLoaded', function () {
            var samples = 100;
            var speed = 300;
            var values = [];
            var labels = [];
            var charts = [];
            var value = 0;
            values.length = samples;
            labels.length = samples;
            values.fill(0);
            labels.fill(0);

            var chart = new Chart(document.getElementById("serviceChart"),
                {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [
                            {
                                data: values,
                                backgroundColor: 'rgb(102, 178, 255)',
                                borderColor: 'rgb(0, 102, 0)',
                                borderSize: 3
                            }
                        ]
                    },
                    options: {
                        responsive: false,
                        animation: {
                            duration: speed * 1,
                            easing: 'easeInQuad'
                        },
                        legend: false,
                        scales: {
                            xAxes: [
                                {
                                    display: false,
                                }
                            ],
                            yAxes: [
                                {
                                    ticks: {
                                        max: 30,
                                        min: 0
                                    }
                                }
                            ]
                        }
                    }
                });

            var hubConn = new signalR.HubConnectionBuilder()
                .withUrl("healthSensor")
                .build();
            hubConn.on('Broadcast',
                function (sender, info) {
                    values.push(info.healthLevel);
                    values.shift();
                    labels.push(info.serviceName);
                    labels.shift();
                    chart.update();
                });
            hubConn.start();
        });
    </script>
</head>

<body>
    <canvas id="serviceChart" style="width: 600px; height: 440px"></canvas>
</body>

</html>
```

Açkçası chart kullanımını bulduğum örnekler üzerinden yapmaya çalıştım (chart.js çok kapsamlı ve geniş bir kütüphane. Değişik örneklerine ulaşmak için [şu adrese](http://www.chartjs.org/samples/latest/) uğranabilir) Ancak grafiğin veri için kullandığı values ve labels değişkenlerini nasıl beslediğimizi anlatabilirim. hubConn değişkenine dikkat edelim. Önce ServiceSensor.Publisher uygulamasındakine benzer bir şekilde nesne örneklemesi yapılmakta. withUrl parametresinin değeri önemli. on fonksiyonu start sonrası bağlantı bsaşarılı bir şekilde sağlanırsa devreye girecek. Yani istemci dinleme moduna geçecek. Broadcast anahtar kelimemiz (Önceki kodlarda nerede kullanıldığını hatırlayın) İsimsiz fonksiyona gelen parametreler tahmin edeceğiniz üzere Publisher tarafından JSON'a dönüştürülerek gönderilen HealthInformation değerlerini içeriyor. JsonProperty'lerde belirttiğimiz isimlerle anlık verilere ulaşmamız mümkün. push ile veriyi grafiğe basıyor, shift ile o anki barı yana kaydırıyor ve update ile kaynak veri setinin güncellenmesini sağlıyoruz. Hepsi bu kadar. Artık testimize başlayabiliriz. İlk olarak ServiceSensor.House uygulamasını, ardından da veri basan ServiceSensor.Publisher uygulamasını

```bash
dotnet run
```

komutları ile başlatmalıyız. Publisher belirli aralıklarla rastgele bilgi basacak. Sunucu uygulamanın çalışmasına ait örnek ekran görüntüsü aşağıdaki gibi.

![funnyS_1.gif](images/funnyS_1.gif)

Publisher uygulamasına ait bir ekran görüntüsü de şu. Görüldüğü üzere Console'a düşen bilgilerden rastgele üretilen verileri izleyebiliriz.

![funnys_2.gif](images/funnys_2.gif)

Artık tarayıcıdan http://localhost:7001/index.html adresine gidebiliriz. Grafik sağdan sola doğru gelen değerlere göre akmaya başlayacaktır. Bunu canlı canlı izlemek çok zevkliydi benim için. Hatta video kaydını alıp paylaşmak da istedim ama onu görmek için örneği tamamlamaya gayret etmenizin daha faydalı olabileceğini düşündüm. Sizde görmek istiyorsanız örneği tamamlamalısınız:D Şimdilik iki ekran görüntüsü paylaşarak az biraz heyecan yaratayım.

![funnyS_3.gif](images/funnyS_3.gif)

ve

![funnyS_4.gif](images/funnyS_4.gif)

Örnek daha da zenginleştirilebilir. Grafiği değiştrerek işe başlayabilirsiniz. Servislere ait anlık verilerin gerçekten de takip edilmek istenen servislerden beslenmesine çalışabilirsiniz. Tüm istemcilerin değil de sadece yetki bazlı olanların bu grafiğe ulaşmasını sağlayacak güvenlik tedbirlerini deneyebilirsiniz. Servis durumları yerine IoT cihazınıza bağladığınız bir takım sensör verilerinin bu grafiğe yansımasını da sağlayabilirsiniz;) Neler yapabileceğiniz tamamen sizin hayal gücünüze bağlı.

Bu yazımızda SignalR'ı kullanarak gerçek zamanlı basılan verilerin, dinlemede olan istemcilerde grafiksel gösteriminin nasıl yapılabileceğini incelemeye çalıştık. Ve böylece adım adım yaparken bir kek kadar lezzetli olmasa da çok keyif alacağınızı düşündüğüm bir makalemizin daha sonuna geldik. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örneğe [github üzerinden](https://github.com/buraksenyurt/dotnetcore/tree/master/FunnySignalR) de ulaşabilirsiniz.
