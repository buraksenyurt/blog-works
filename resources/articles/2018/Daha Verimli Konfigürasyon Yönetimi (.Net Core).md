---
title: "Daha Verimli Konfigürasyon Yönetimi (.Net Core)"
pubDate: 2018-12-10 06:36:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - JSON
  - C#
  - .NET
  - XML
  - Web API
  - HTTP
  - Generics
---

# Daha Verimli Konfigürasyon Yönetimi (.Net Core)
![easy_config_0.jpg](images/easy_config_0.jpg)

Merhaba Arkadaşlar,

Epey zamandır onbeş yaşından büyük bir proje ile ilgili geliştirmeler yapmaktayız. Uygulamayı ilk kez kullanmaya başladığımda en çok zorlandığım şeylerden birisi, küçük birimlerin testini yazmak olmuştu. Sıkılaşmış ve hatta kemikleşmiş bağımlılıklar nedeniyle basit bir fonksiyon testi için gerekli gereksiz bir çok kütüphanenin kullanılması gerekebiliyordu. Sahte nesneleri araya almak bir yere kadar çözüm olabilirdi. Lakin entegrasyon testlerini deneyimlerken karşılaştığım bir başka sorun daha vardı. Karmaşıklık değeri yüksek konfigurasyon dosyaları. Çok fazla konfigurasyon ayarı, özelleştirilmiş sektör, şifrelenmiş bağlantı cümleleri ve diğer parametrik ayarlarla ilgili bilgiler tutan web.config zaman içerisinde epeyce şişmanlamıştı. God Object değil ama God Configuration (Böyle bir terim yok tabii ben uydurdum) gibi bir anti-pattern oluşmuştu. İlk başlarda sadece gereken ayarları alarak test projesini ayağa kaldırmaya çalışmıştım. İşin sonunda ise tüm web.config'i kopyalamıştım.

Oysa ki konfigurasyon yönetimi de en az temiz kod yazmaya çalışmak kadar titizlikle üzerinde durulması gereken bir mevzu. Bazen bir konfigurasyon dosyasını parçalamak ve bu şekilde yönetmeye çalışmak çok daha anlamlı olabilir. Hele ki ortamların test, pre-prod ve prod olarak ayrıldığı ve Continuous Integration/Deployment/Delivery hattı üzerinde değer bulduğu bir dünyada oldukça önemli. Microsoft.Net dünyasında çok uzun zamandır konfigurasyon içeriklerini efektif bir şekilde yönetebiliyoruz. Pratik bir kaç bilgi ile bu yönetim gücünü daha da verimli hale getirebiliriz.

Bildiğiniz üzere.Net core tarafında konfigurasyon bilgileri varsayılan olarak JSON formatlı olarak tutulur (appSettings.json) Burada ön tanımlı parametreler dışında özel konfigurasyon içeriklerini kullanmak da mümkündür. Hatta istersek birden fazla ve farklı formatta konfigurasyon dosyasını kullanabiliriz. Her ikisini uygulamak da oldukça kolay aslında. Nasıl mı? Gelin birlikte bakalım. İlk olarak basit bir Web API oluşturarak işe başlayabiliriz. Amacımız kendi eklediğimiz bir sekme ve içindeki parametreleri çalışma zamanında kullanabilmek.

```bash
dotnet new webapi -o ConfigSample
```

Varsayılan olarak appSettings.json içeriği aşağıdaki gibi oluşur (Tabii kullanılan.net core sürümüne göre ufak tefek farklılıklar olabilir)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Logging ve AllowedHosts isimli iki ana sekme bulunuyor. Şimdi bu içeriği aşağıdaki gibi değiştirelim.

```json
{
  "Logging": {
    "IncludeScopes": true,
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "DefaultSettings": {
    "Owner": "B&B Organization",
    "Address": "One way road, Dublin, 10",
    "Contact": "contact@BandB.com"
  }
}
```

DefaultSettings isimli yeni bir parametre seti ekledik. İçerisinde Owner, Address ve Contact isimli üç anahtar:değer çifti var. Bu içeriği kod tarafında kullanmak için IConfiguration arayüzünden yararlanabiliriz. Örneğin şablon ile birlikte hazır olarak gelen ValuesController sınıfında DefaultSettings içeriğini kullanmak istediğimizi düşünelim. Kodları aşağıdaki gibi düzenleyerek ilerleyelim.

```csharp
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConfigSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController
    : ControllerBase
    {
        private readonly IConfiguration _config = null;
        private readonly ILogger<ValuesController> _logger = null;
        public ValuesController(IConfiguration config, ILogger<ValuesController> logger)
        {
            _config = config;
            _logger = logger;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var contact = _config.GetValue<string>("DefaultSettings:Contact");
            var owner = _config.GetValue<string>("DefaultSettings:Owner");
            _logger.LogInformation($"Contact Email : {contact}\n");
            _logger.LogInformation($"Owner : {owner}\n");
            return new string[] { "value1", "value2" };
        }
    }
}
```

ValuesController sınıfının yapıcı metoduna (Constructor) müdahale ettik. IConfiguration ve ILogger arayüzleri parametre olarak geliyor. Aslında çalışma zamanına enjekte edilen nesnelerin içeriye alındığını ifade edebiliriz. Sonrasında _config ve _logger değişkenlerini diğer metodlarda kullanmamız mümkün. _logger nesnesini log bilgisi vermek için kullanıyoruz. DefaultSettings içerisindeki değerleri okumak için {SectionName}:{KeyName} notasyonundan yararlandağımıza dikkat edelim. Değerleri GetValue metodu ile almaktayız. Eğer uygulamayı

```bash
dotnet run
```

terminal komutu ile çalıştırıp http://localhost:5000/api/values adresine HTTP Get talebinde bulunursak appSettings içerisindeki ilgili değerlere erişebildiğimizi görürüz.

![easy_config_1.gif](images/easy_config_1.gif)

Tabii geliştireceğimiz uygulamaların çeşitli ve çok sayıda konfigurasyon bağımlılığı olabilir ve bu bağımlılıkları mantıksal düzende ayrı dosyalar halinde tutmak isteyebiliriz. Bu durumu deneyimlemek için DefaultSettings içeriğini örneğin copyrightSettings.json isimli ayrı bir dosyaya aldığımızı düşünelim (appSettings.json ile aynı yerde olmalarında yarar var)

![easy_config_2.gif](images/easy_config_2.gif)

copyrightSettings.json;

Artık appSettings.json dışında yeni bir konfigurasyon dosyamız daha olduğunu uygulama çalışma zamanına bildirmemiz gerekiyor. Bunu gerçekleştirmenin yollarından birisi Startup.cs içeriğini değiştirmekle mümkün. Kodu aşağıdaki gibi düzenleyelerek örneğimize devam edelim.

Startup sınıfının yapıcı metodunda ConfigurationBuilder nesnesini kullanıyoruz. Root Path bilgisine göre AddJsonFile metodundan yararlanarak kullanmak istediğimiz konfigurasyon dosyalarını orta katmana bildiriyoruz. Son olarak ConfigurationBuilder'dan üretilen IConfiguration referansını AddSingleton ile çalışma zamanı servislerine ekliyoruz. Dikkat ederseniz yorumlanmış bir kod satırı da var. Tahmin edeceğiniz üzere JSON formatlı dosyalara bağımlı değiliz. İstersek eski dostumuz XML tabanlı konfigurasyon dosyalarını da işin içerisine katabiliriz. Bunu denemenizi öneririm. Uygulamayı tekrar çalıştırıp doğru çalıştığından emin olmakta yarar var.

![easy_config_3.gif](images/easy_config_3.gif)

Konfigurasyon yönetimi her zaman için önemli konulardan birisi. Veritabanı bağlantı bilgileri, loglama kriterleri, çeşitli ortam parametrelerinin varsayılan değerleri, şifrelenmiş token bilgileri çoğunlukla bu dosyalar içerisine konuluyor. Bu nedenle kod tarafında nasıl yönetebileceğimizi bilmekte yarar var. Bu kısa yazımızda konfigurasyon içerisine alacağımız özel sekmeleri nasıl kullanabileceğimizi ve içerikleri ayrı dosyalar halinde nasıl ele alabileceğimizi incelemeye çalıştık. Umarım faydalı olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
