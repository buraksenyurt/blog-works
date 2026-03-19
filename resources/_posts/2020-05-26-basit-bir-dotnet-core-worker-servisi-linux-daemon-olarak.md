---
layout: post
title: "Basit Bir .Net Core Worker Servisi(Linux Daemon Olarak)"
date: 2020-05-26 20:05:00 +0300
description: ".Net Core Worker Service'ler ile planlanmış görevlerin arka planda icra edilmesi mümkün. Mesela belirli aralıklarla sistemden veri toplayıp kullanan bir Windows Service bu şekilde geliştirilebilir. Lakin o zaman .Net Core kullanmanın bir esprisi kalmıyor öyle değil mi? :) Bunun yerine bir Linux servisi geliştirmeyi ne dersiniz? En azından nasıl geliştirilebileceğini öğrenmeye. İşte benim amacım Heimdall(Ubuntu-20.04) üzerinde Linux Daemon olarak çalışacak bir servis yazmak. Örneğin günlük hava durumu bilgilerini 24 saatte bir toplayıp Redis üzerinde saklayan bir servis pekala güzel ve eğlenceli olabilir(Belki de olmaz, neyse) Daha önceki örneklerde kullandığımız Redis Docker Container'ı burada da kullanabiliriz. Öyleyse ne duruyoruz. Haydi kodlamaya."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - linq
  - redis
  - json
  - docker
  - async-await
  - caching
  - generics
  - github
---
# Basit Bir .Net Core Worker Servisi(Linux Daemon Olarak)
Konfor alanı dışında çalışmak geliştiricileri zaman zaman zorlasa da pek çok yeniliğin de kapısını açıyor. Örneğin yıllar önce pek çok kurumsal projede Windows Service'ler geliştirmiş olan ben bunun Linux platformunda yapılıp yapılamayacağını asla bilemezdim; şayet evdeki makineme Ubuntu kurup üstünde.Net Core ile bir şeyler yapmaya çalışana kadar. İşte günün konusu planlı işler için bir alternatif olan Worker Service'ler.

.Net Core Worker Service'ler ile planlanmış görevlerin arka planda icra edilmesinin mümkün olduğunu biliyoruz. Mesela belirli aralıklarla sistemden veri toplayıp kullanan bir Windows Service bu şekilde geliştirilebilir. Lakin o zaman.Net Core kullanmanın bir esprisi kalmıyor öyle değil mi?:) Bunun yerine bir Linux servisi geliştirmeyi ne dersiniz? En azından nasıl geliştirilebileceğini öğrenmeye...İşte benim amacım da tam olarak bu. Heimdall (Ubuntu-20.04) üzerinde Linux Daemon olarak çalışacak bir servis yazmak. Örneğin günlük hava durumu bilgilerini 24 saatte bir toplayıp Redis üzerinde saklayan bir servis pekala güzel ve eğlenceli olabilir (Belki de olmaz, neyse) Daha önceki örneklerde kullandığımız Redis Docker Container'ı burada da kullanabiliriz. Öyleyse ne duruyoruz. Haydi kodlamaya.

## Hazırlıklar ve Kodlama

Önce Redis Docker Container'ını bir ayağa kaldıralım. Kaldırdıktan sonra onunla ping pong oynamak ve hatta bir anahtar değeri oluşturup okumaya çalışmak yararlı olabilir. Aynen aşağıdaki terminal komutlarında olduğu gibi.

```bash
sudo docker run -d --name liverpool -p 6379:6379 redis
sudo docker exec -it liverpool redis-cli
ping
SET name "Merhaba Redis"
GET name
DEL name
GET name
```

Projemizi iskeletini de aşağıdaki gibi inşa edebiliriz.

```bash
dotnet new worker -o WeatherCollector
cd WeatherCollector
dotnet add package Microsoft.Extensions.Hosting.Systemd
dotnet add package Microsoft.Extensions.Caching.Redis
```

Systemd modülü Linux Daemon'ının uygulamayı kullanılabilmesi için eklenmiştir. Servisimiz worker.cs içerisinde hayat buluyor. Bu nedenle içeriğini anlamak önemli. Lütfen yorum satırlarını okuyunuz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace WeatherCollector
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDistributedCache _distributedCache; //Redis için gerekli
        private Task _executingTask;
        private CancellationTokenSource _cts;

        // Loglama ve Redis için gerekli nesneleri constructor'dan içeriye enjekte ediyoruz
        public Worker(ILogger<Worker> logger, IDistributedCache distributedCache) 
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        // Servis başladığında devreye giren metot. Override etmek zorunda değiliz
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning($"Weather Collector service started at {DateTimeOffset.Now}");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        // Arka plan görevinin başladığı metot
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = new DistributedCacheEntryOptions()
                       .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                       .SetAbsoluteExpiration(DateTime.Now.AddHours(1));

            while (!stoppingToken.IsCancellationRequested) // Eğer bir iptal talebi gelmediyse
            {
                _logger.LogInformation($"Looking for weather informations at: {DateTimeOffset.Now}");

                // Normalde hava durumu verisi harici bir servisten geliyor olmalı.
                // Burada tamamen sembolik bir JSON içeriği söz konusu
                var temprature = "[{\"city\":\"İstanbul\",\"value\":\"39\"},{\"city\":\"Ankara\",\"value\":\"34\"}]";
                var redisValue = Encoding.UTF8.GetBytes(temprature);

                // veriyi Redis Cache'e alıyoruz. Farklı bir veritabanı da kullanılabilir
                await _distributedCache.SetAsync($"State_{DateTime.Now.Day}_{DateTime.Now.ToString("hh_mm_ss")}", redisValue, options);

                // Arka plan görevi bu eğitim örneği özelinde 3 dakikada bir işleyecek
                await Task.Delay(3 * 60 * 1000, stoppingToken);
            }
        }

        // Servis durdurulduğunda override edilmişse devreye giren metot
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return Task.CompletedTask;
            }

            _logger.LogWarning($"Weather Collector stopping at: {DateTimeOffset.Now}");
            _cts.Cancel();
            Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken)).ConfigureAwait(true);
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogWarning($"Weather Collector stopped at: {DateTimeOffset.Now}");

            return Task.CompletedTask;
        }
    }
}
```

Program.cs dosyasının içeriği de şu şekilde yazılabilir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace WeatherCollector  
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    // Redis servisini middleware'e ekledik. Artık Worker sınıfının yapıcı metodundan içine enjekte edilebilir
                    services.AddDistributedRedisCache(action =>
                    {
                        action.Configuration = "localhost:6379";
                    });
                });
    }
}
```

## Daemon Kurulumu için Hazırlıklar

Uygulama kodları tamamlandıktan ve dotnet run sonrası düzgün bir şekilde çalıştığından emin olduktan sonra publish ederek devam edebiliriz. Nitekim servis olarak release edilmiş sürümün kullanılması gerekiyor.

```bash
dotnet publish -o artifact
```

Ardından.service uzantılı bir Unit dosyası hazırlamalıyız. Burada servise ait bazı bilgiler yer alıyor. Açıklaması, türü, başlangıç noktası, kurulum şekli gibi.

```text
[Unit]
Description=Hava Durumu Servisi

[Service]
Type=notify
ExecStart=/home/buraks/Documents/Services/artifact/WeatherCollector

[Install]
WantedBy=multi-user.target
```

Bu dosya /etc/systemd/system altına alınmalı. Normalde ilgili dosya systemd/system klasörüne atılır atılmaz etkinleşiyor ama etkinleşmezse dameon-reload çağrımını deneyebiliriz. Durum kontrolü içinse status komutundan yararlanmak bu aşamada önemli. Nitekim servis çalışıyor mu, hata mı aldı gibi durumları gözlemlemeliyiz. İşte gerekli ilk terminal komutlarımız.

```bash
sudo cp WeatherCollector.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl status WeatherCollector
```

Servisi yeniden başlatmak, durdurmak veya sistemden kaldırmak için systemctl aracının belli başlı komutlarını bilmekte de yarar var.

```bash
sudo systemctl daemon-reload
sudo systemctl status WeatherCollector
sudo systemctl restart WeatherCollector
sudo systemctl stop WeatherCollector
sudo systemctl disable WeatherCollector
sudo rm WeatherCollector.service
```

İlk komut servis dosyasında değişiklik olduysa demaon'ı yeniden yüklemek için kullanılıyor. İkinci komut servisin güncel durumunu görmek, üçüncüsü yeniden başlatmak, dördüncüsü durdurmak ve beşincisi pasif hale çekmek için. Son komut ile de servis dosyasını kaldırıyoruz. Bu bir öğreti çalışması olduğu için servisi içerde unutmamak önemli. Yoksa üç dakikada bir...:D

İşte uygulamanın en azından Heimdall üzerindeki çalışma zamanına ait iki ekran görüntüsü.

![skynet_11_Screenshot_1.png](/assets/images/2020/skynet_11_Screenshot_1.png)

![skynet_11_Screenshot_2.png](/assets/images/2020/skynet_11_Screenshot_2.png)

Tabii örneği ben vakti zamanında.Net Core 3.1 ile geliştirmiştim. Bunu.Net 5.0 ile de kurgulamak gerekli. Hatta servisin gerçekten de gerçek bir dış servisten bilgi alıp Redis'e atmasını da sağlarsanız pek bir güzel olur. Ya da biraz daha uç örnekler göz önüne alınabilir. Söz gelimi sistemin durumu hakkında bir takım bilgileri çeşitli periyotlarda toplayıp uzak sunucudaki bir Elasticsearch servisine loglayan bir Dameon kurgulanabilir;)

Örneğe ait kodlara [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2011%20-%20.Net%20Core%20Worker%20Services) üzerinden erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.