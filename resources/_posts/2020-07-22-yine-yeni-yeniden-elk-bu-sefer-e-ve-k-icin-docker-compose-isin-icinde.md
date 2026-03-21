---
layout: post
title: "Yine Yeni Yeniden ELK(Bu sefer E ve K için docker-compose işin içinde)"
date: 2020-07-22 20:23:00 +0300
categories:
  - dotnet-core
tags:
  - docker
  - .net
  - .net-core
  - elk
  - elasticsearch
  - kibana
  - logging
---
Her ne kadar artık.Net 5.0 hayatımızın içinde olsa da bu yıl içinde bir yerlerde.Net Core 3.1 ile ELK kurgusunu yeniden değerlendirme ihtiyacı hissetmişim. Elasticsearch, Logstash ve Kibana kurgusu aslında günümüz uygulamalarında son derece popüler. Genellikle uygulama loglarının devasa şekilde biriktiği durumların çözümünde ideal bir kurgu olarak karşımıza çıkıyor. Bu kurguda uygulama loglarını standart bir formata uygun olacak şekilde Elasticsearch'e atar, Kibana arayüzünü kullanarak izleme yapar ve çeşitli durumların kontrolünü gerçekleştiririz. Ağırlıklı olarak üretim ortamında oluşacak hataların, performans kayıplarının ve dar boğazların yakalanması noktasında işimize yarayan bir düzenek olarak düşünebiliriz.

![elk_2.png](/assets/images/2020/elk_2.png)

Şunu da belirtmekte yarar var; günümüz uygulamalarında hataları debug ederek bulmak yerine detaylı ve iyi bir strateji ile oluşturulmuş logları takip ederek tedbir almak son derece kıymetlidir. Benim bu çalışmadaki amacım.Net Core 3.1 üstünden ELK düzeneğini kurgulayıp loglama ve izleme işlerini tekrarlamaktı. Ancak bu sefer ElasticSearch ve Kibana ortamları için docker compose aracını kullanmayı tercih etmiştim. Yani çalışmanın ana noktası ilgili düzeneği Docker Compose ile kurgulamak ve çalışma sonuçlarını görmekten ibaret. Öyleyse kurgumuza başlayalım

## Düzeneğin Hazırlanması

Aşağıdaki terminal komutlarında görüldüğü gibi çalışmanın iskeleti ve Asp.Net Web API servisimizin ihtiyaç duyacağı Nuget paketlerini yükleyerek işe başlanabilir. Evet senaryoda kobay bir servisimi var ve bu servisin örnek bir fonksiyonu içerisinde log fırlatıp izlemeyi planlıyoruz.

```bash
mkdir docker
touch docker/docker-compose.yml
dotnet new webapi --no-https -o Readers
cd Readers
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Sinks.Debug
dotnet add package Serilog.Sinks.Elasticsearch
dotnet add package Serilog.Exceptions
```

Bu arada Heimdall (Ubuntu-20.04) üstünde docker-compose yoktu. Bu nedenle sudo apt install docker-compose ile yüklemem gerekti. Sizin sisteminizde de buna benzer bir kurulum gerekebilir. Senaryo gereği elbette kobay bir web api servisine ihtiyacımız var. Readers isimli bu servisin temel kodlarını aşağıda bulabilirsiniz.

Kitap ve yazar bilgilerini tutan Book sınıfı (Siz gerçek hayatta yazarları bu şekilde tutmayın tabii)

```csharp
using System;

namespace Readers
{
    public class Book
    {
        public string Name { get; set; }
        public string Authors { get; set; }
    }
}
```

Controller sınıfımız.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Readers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;

        public BookController(ILogger<BookController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Book Get()
        {
            _logger.LogInformation("{date} zamanı itibariyle bir kitap önerildi...", DateTime.UtcNow);

            _logger.LogError(new System.IO.FileNotFoundException(), "Yeni bir kitap eklemeye çalışırken hata oluştu");

            return new Book { Name = "Yabancı", Authors = "Alber Camus" };
        }
    }
}
```

Get fonksiyonunda sadece loglama operasyonları için bir bilgi ve hata bildirimi yapmaktayız. Tabii burada soru Loglama'yı kimin, hangi formatta, nereye ve nasıl yapacağı?........................................................................................................................................................................................................................................Bu "fill in the blanks" kısmında bir süre düşündüğünüzü varsayıyorum:P Tahmin edeceğiniz üzere Program.cs, Serilog tanımlarını yaptığımız yerdir.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Elasticsearch;

namespace Readers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ElasticSearch log için gerekli konfigurasyon ayarları
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", "Readers") // Hangi uygulama log atıyor
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(
                        new Uri("http://localhost:9200/"))
                    {
                        AutoRegisterTemplate = true,
                        TemplateName = "serilog-events-template",
                        IndexFormat = "readers-api-log-{0:yyyy.MM.dd}"
                    })
                //.MinimumLevel.Verbose()
                .CreateLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog();
    }
}
```

Kobay servisimiz hazır. Şimdi docker compose dosyasını hazırlayalım (Bu arada Visual Studio Code için kullanılan Docker Extension, yaml dosyasının hazırlanmasında bana epey yardımcı oldu diyebilirim)

```yml
version: '3.7'

services:

    elasticsearch:
        container_name: elasticsearch
        image: docker.elastic.co/elasticsearch/elasticsearch:7.7.0
        ports:
            - 9200:9200
        volumes: 
            - elasticsearch-data:/usr/share/elasticsearch/data
        environment: 
            - xpack.monitoring.enabled=true
            - xpack.watcher.enabled=false
            - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
            - discovery.type=single-node
        networks:
            - elastics

    kibana:
        container_name: kibana
        image: docker.elastic.co/kibana/kibana:7.7.0
        ports:
            - 5601:5601
        depends_on: 
            - elasticsearch
        environment: 
            - ELASTICSEARCH_URL=http://localhost:9200
        networks:
            - elastic

networks:
    elastic:
        driver: bridge

volumes: 
    elasticsearch-data:
```

Bu kompozisyonda Elasticsearch ve Kibana servisleri için bazı tanımlamalar yer alıyor. Her iki container elastic isimli aynı ağ üzerinden birbirleriyle haberleşmekte. Servis tanımlamalarında (elasticsearch:, kibana: kısımları) hangi imajların kullanılacağı, container adlarının ne olacağı, port bilgileri ile yayınlanma adresleri gibi ürüne özel ayarlara yer verilmekte (Docker Compose ile ilgili detaylı bilgi için [şuraya](https://docs.docker.com/compose/gettingstarted/) bakabilirsiniz) Tüm hazırlıklar tamam diyebilirim.

## Çalışma Zamanı

Öncelikle Elasticsearch ve Kibana ortamlarının ayağa kaldırılması gerekiyor. Aşağıdaki terminal komutu ile bunu sağlayabiliriz. Görüldüğü üzere Elastichsearch için ayrıca veya Kibana için ayrıca docker komutlarını kullanmaya gerek yok. Tek bir kompozisyon altında ihtiyacımız olan docker birimlerinin işletilmesini sağlıyoruz.

```bash
cd docker
sudo docker-compose up -d
```

Bunun arından belki küçük bir kontrol yapmakta yarar olabilir. Nitekim http://localhost:9200 adresinden ElasticSearch, http://localhost:5601 adresinden de Kibana servislerine sorunsuz erişebiliyor olmalıyız.

![skynet_08_Screenshot_3.png](/assets/images/2020/skynet_08_Screenshot_3.png)

Sonrasında kobay web api servisimizi ayağa kaldırılabilir ve bazı denemeler yapabiliriz.

```bash
cd Readers
dotnet run
```

Örneğin http://localhost:5000/book adresinden bir HTTP Get talebi yollayabiliriz. Hatırlarsanız Get fonksiyonu içinden Information ve Error türlerinde örnek log mesajları fırlatmıştık (Tabii ki pratikte Error tipinden mesajları exception oluştuğu durumlarda göndermek lazım) Bu arada Kibana'ya erişmek log bilgilerini takip etmek için kafi ama yeterli değil. http://localhost:5601 adresine uğradıktan sonra Readers isimli web api servisi için aşağıdaki görsellerde olduğu gibi bir index eklemek gerekir.

![skynet_08_Screenshot_1.png](/assets/images/2020/skynet_08_Screenshot_1.png)

![skynet_08_Screenshot_2.png](/assets/images/2020/skynet_08_Screenshot_2.png)

Sonrasında örneğin Kibana ortamında ([KQL - Kibana Query Language](https://www.elastic.co/docs/explore-analyze/query-filter/languages/kql)) ile Error seviyesinde olan veya mesaj içeriğinde "zamanı" kelimesi geçen logları aratabiliriz.

```text
level : "Error" or message : "zamanı"
```

![skynet_08_Screenshot_4.png](/assets/images/2020/skynet_08_Screenshot_4.png)

Görüldüğü üzere ELK için docker-compose'dan yararlanarak ideal loglama senaryosunu kurgulamak oldukça basit. Gerçek hayat senaryosuna baktığımızda docker-compose pekala ayrık ve dağıtık suncularda hizmet edecek şekilde konuşlandırılabilir. Mikroservisler veya başka türden uygulamalar docker-compose ile ayağa kalkan Elastichsearch servisine erişebildiği sürece Kibana ile logları izlemek ve belki de alarm sistemleri kurarak sistemi kontrol altında tutmak pekala mümkündür.

Konuya ait örnek kodların tamamını [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2008%20-%20ELK%20Again)nda bulabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
