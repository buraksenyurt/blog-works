---
layout: post
title: ".Net Core Konfigurasyon Yönetimi Üzerine"
date: 2018-03-01 07:33:00 +0300
categories:
  - dotnet-core
tags:
  - json
  - configuration-management
  - .net-core
  - dependency-injection
  - IConfigurationRoot
  - configuration-builder
---
West-World üzerinde bir şeyler araştırmak için vaktim olan bir haftayı geride bırakmak üzereyim. Internet üzerinde derya deniz içerik olsa da bazen ne araştıracağımı şaşırmıyor değilim. İşte bu anlarda [MSDN dokümanları](https://docs.microsoft.com/en-us/) imdadıma yetişiyor. İlk zamanlarından beri oldukça verimli olduğunu düşündüğüm içerik son yıllarda çok daha profesyonelleşti (Tabii MSDN dokümanlarını CD veya DVD olarak edindiğimiz zamanları da hatırlıyorum) İşin aslı sadece MSDN değil, yazılım ürünü sahibi pek çok öncünün teknik destek dokümanları inanılmaz derecede doyurucu ve birbirleriyle yarışır durumdalar. Son zamanlarda uğradıklarım arasında [Google Cloud Platform](https://cloud.google.com/docs/) ve [Amazon Web Services](https://aws.amazon.com/documentation/) var. Bu rehberler ilk kaynak niteliğinde olduğu için bir şeyleri öğrenebilmemiz adına doğru adresler. Hatta yazılım geliştirici olarak ortalama bir seviyenin üstüne çıktıktan sonra bunlar gibi dokümantasyonlara uğramak, rastgele bir ürün seçip teknik dokümantasyonunu okumak, Get Started örneklerini yapıp bir şeylerin farkına varabilmek gerekiyor. Sözü fazla uzatmadan gerekli mesajları da verdiğimi düşünerek yazımıza başlayalım diyorum.

![customconfig_0n.gif](/assets/images/2018/customconfig_0n.gif)

Çalışma zamanına bilgi taşımanın ve bazı ayarlamalar için gerekli değerleri okumanın en popüler yollarından birisi de bildiğiniz üzere konfigurasyon dosyalarından yararlanmak. Zaman içerisinde app.config, web.config gibi XML tabanlı konfigurasyon dosyalarına aşina olan bizler,.Net Core ile birlikte JSON formatlı içeriklerle çalışmaya başladık..Net Core tarafında bu JSON içeriklerini yönetmek oldukça kolay. Farklı yöntemlerimiz var. Dahası Dependency Injection yeteneklerinden yararlanılabildiği için özel sekmelerin (section) sınıflara bağlanması da münkün (Hatta Interface Segregation Principle ve Seperation of Concerns ilkelerini kullanan Options deseni var ki ilk fırstatta inceleyip öğrenmek istiyorum. Farkında olmadan kullanıyoruz ama altındaki çalışma dinamiklerini öğrenmek çok yerinde olacaktır) Gelin bir kaç basit örnek ile konfigurasyon yönetimini nasıl yapabileceğimizi incelemeye çalışalım. Ağırlıklı olarak varsayılan konfigurasyon dosyaları haricinde kendi özel içeriklerimizle çalışacağız. Kodlarımızı Console tabanlı bir uygulamada deneyimleyeceğiz ama aynı teknikleri Web, Web API gibi diğer proje türlerinde de kullanabilirsiniz.

```bash
dotnet new console -o CustomConfig
```

Kendi JSON İçeriğimiz İle Çalışmak

İlk örnek için aşağıdaki içeriğe sahip olan aws.json isimli bir dosyadan yararlanacağız.

```json
{
  "default_region": "east-2",
  "provider": "amazon",
  "region": {
    "name": "east-2",
    "address": "amazonda.bir.yer.east-2"
  },
  "services": [
    {
      "address": "products/get",
      "response_type": "json",
      "isPublic": "true"
    },
    {
      "address": "products/get/{categoryName}",
      "response_type": "json",
      "isPublic": "true"
    }
  ]
}
```

Tamamen hayal ürünü olan içerikte iç içe geçen alanlar da yer alıyor. default_region, provider, region ve services aynı seviyede olmakla birlikte, services içerisinde n sayıda eleman bulunabiliyor. Amacımız bu içeriği çalışma zamanında okuyabilmek. Özellikle JSON tabanlı çalışacağımız için bize gerekli fonksiyonellikleri sağlayacak iki pakete de ihtiyacımız bulunuyor.

```bash
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.Configuration.CommandLine
```

Bu paketleri ekledikten sonra ilk örnek kodlarımızı aşağıdaki gibi yazabiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace CustomConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigSupervisor rubio = new ConfigSupervisor();
            rubio.ExecuteJsonSample();
        }
    }

    public class ConfigSupervisor
    {
        public IConfigurationRoot ConfigurationManager { get; set; }

        public void ExecuteJsonSample()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("aws.json");

            ConfigurationManager = builder.Build();

            Console.WriteLine($"default_region: \t{ConfigurationManager["default_region"]}");
            Console.WriteLine($"provider: \t{ConfigurationManager["provider"]}");
            Console.WriteLine($"region name: \t{ConfigurationManager["region:name"]}");
            Console.WriteLine($"region address: \t{ConfigurationManager["region:address"]}");
            Console.WriteLine($"service[1] address : \t{ConfigurationManager["services:1:address"]}");
            Console.WriteLine($"service[1] type: \t{ConfigurationManager["services:1:response_type"]}");
            Console.WriteLine($"service[1] isPublic: \t{ConfigurationManager["services:1:isPublic"]}");

            var services = ConfigurationManager.GetSection("services").AsEnumerable();
            foreach (var service in services)
            {
                Console.WriteLine($"{service.Key}-{service.Value}");
            }
        }
    }
}
```

ConfigSupervisor sınıfı örneklere ait fonksiyonellikleri içeriyor. ExecuteJsonSample metodumuzun başında aws.json dosyasını ele alması için bir ConfigurationBuilder örneği oluşturuyoruz. Build çağrısı sonucu IConfigurationRoot arayüzü üzerinden taşınabilecek bir nesne örneği elde ediyoruz. Sonuç olarak indeksleyici operatörünü kullanarak konfigurasyon öğelerine erişim sağlıyoruz. root altında yer alan default_region ve provider alanlarının değerlerine erişmek oldukça kolay. region içerisindeki name niteliğine erişmek içinse: operatörünü kullanıyoruz (region:name şeklinde)

Bu notasyona göre services olarak isimlendirilmiş array içerisindeki bir elemana erişirken index değerini kullanarak ilerleyebiliyoruz. Söz gelimi services:1:isPublic ile 1 indisli elemanın isPublic niteliğinin değerine ulaşmış oluyoruz. Elbette services isimli dizinin elemanlarını bir döngü yardımıyla okuyabiliriz de. GetSection fonksiyonu ile konfigurasyon yöneticisinin okuduğu dosyadan ilgili sekmeyi almamız yeterli. AsEnumerable metodu ile üzerinde ileri yönlü hareket edilebilir hale getirdikten sonra Key ve Value değerlerine erişmemiz oldukça basit. Uygulamanın çalışma zamanı çıktısı aşağıdaki gibi olacaktır (services sekmesini daha iyi okumanın bir yolunu bul Burak! O ne öyle -, 1-,0-:D)

![customconfig_1.gif](/assets/images/2018/customconfig_1.gif)

JSON İçeriğini Sınıflar ile İlişkilendirmek

Peki JSON dosyasının içeriğinde yer alan sekmeleri birer tiple ilişkilendirmek istersek? Ki zaten ezelden beridir konfigurasyon içerikleri.Net dünyasında sınıflarla ilişkilendirilip yönetimli kod tarafında kullanılabiliyorlar. Bunu.Net Core ortamında JSON içeriklerimiz için gerçekleştirmemiz de mümkün. İlk olarak aşağıdaki json içeriğini barındıracak gamesettings.json dosyasını projemize ekleyelim.

```json
{
    "Game": {
        "Requirement": {
            "OS": "Ubuntu",
            "RAM": "8",
            "Region": "west-world",
            "Online": "true"
        },
        "Contacts": [
            {
                "Name": "tech support",
                "Email": "techsupport@west-world.bla.bla"
            },
            {
                "Name": "game master",
                "Email": "gamemaster@west-world.bla.bla"
            },
            {
                "Name": "help desk",
                "Email": "helpdesk@west-world.bla.bla"
            }
        ]
    }
}
```

Bu içeriğin kod tarafındaki karşılığı olacak sınıflarımızı ise aşağıdaki gibi yazalım.

Tüm JSON içeriğini işaret edecek GameSetting sınıfı

```csharp
using System.Collections.Generic;

public class GameSetting
{
    public Requirement Requirement { get; set; }
    public IEnumerable<Contact> Contacts{get;set;}
}
```

GameSettings sekmesinde yer alan Contacts bir dizi olduğu için, IEnumerable tipinden Contacts isimli bir özellik söz konusu.

Requirement kısmını işaret eden Requirement sınıfı;

```csharp
using System.Collections.Generic;

public class Requirement
{
    public string OS { get; set; }
    public int RAM { get; set; }
    public string Region { get; set; }
    public bool Online { get; set; }
}
```

ve son olarak Contacts sekmesi altındaki bağlantıların her birisini işaret edecek Contact sınıfı.

```csharp
public class Contact
{
    public string Name { get; set; }
    public string Email { get; set; }
}
```

Sınıfımıza ekleyeceğimiz fonksiyonumuz ise şu şekilde yazılabilir.

```csharp
public void ExecuteObjectGraphSample()
{
    var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("gamesettings.json");

    ConfigurationManager = builder.Build();

    var gameConfig = new GameSetting();
    ConfigurationManager.GetSection("Game").Bind(gameConfig);
    var requirement=gameConfig.Requirement;
    Console.WriteLine($"OS {requirement.OS} ({requirement.RAM} Ram)");            
    foreach(var contact in gameConfig.Contacts)
    {
        Console.WriteLine($"{contact.Name}({contact.Email})");
    }
}
```

Öncelikle gamesettings.json dosyasını ele alacak ConfigurationBuilder örneği oluşturulup Build operasyonu ile konfigurasyon yöneticisi üretiliyor. Sonraki kısım ise epey keyifli. GetSection ile yakalanacak olan Game içeriğini Bind metodundan yararlanarak GameSetting nesne örneği olan gameConfig'e bağlıyoruz. JSON konfigurasyonundaki isimlendirmelere göre Bind metodu doğru eşleştirmeleri bizim için otomatik olarak yapacak. Sonrasında örnek olması açısından OS ve RAM bilgileri ile firma kontaklarına ait Name ve Email değerlerini ekrana yazdırıyoruz. Dikkat edilmesi gereken nokta bir önceki örnekten farklı olarak tüm bu değerlerin Bind işlemi sonrası JSON İçeriğine bağlanan gameConfig nesnesi üzerinden yakalanabilmesi. Çalışma zamanı sonuçları aşağıdaki gibi olacaktır.

![customconfig_2.gif](/assets/images/2018/customconfig_2.gif)

Bellekte Konuşlandırılmış Konfigurasyon İçeriği ile Çalışmak

MSDN dokümanlarından öğrendiğim ilginç örneklerden birisi de konfigurasyon bilgilerinin in-memory olarak tutulup yönetilebilmesi. Yeni fonksiyonumuz ExecuteInMemorySample'ı aşağıdaki gibi yazalım.

```csharp
public void ExecuteInMemorySample()
{
    var builder = new ConfigurationBuilder();

    var parameters = new Dictionary<string, string>{
        {"Region:Name","east-us-2"},
        {"Region:BaseAddress","amazon.da.bir.yer/west-world/api"},
        {"Artifact:Service:Name","products"},
        {"Artifact:Service:MaxConcurrentCall","3500"},
        {"Artifact:Service:Type","json"},
        {"Artifact:Service:IsPublic","true"}
    };

    builder.AddInMemoryCollection(parameters);
    ConfigurationManager = builder.Build();
    Console.WriteLine($"{ConfigurationManager["Artifact:Service:Name"]}");
    Console.WriteLine($"{ConfigurationManager["Artifact:Service:Type"]}");
    Console.WriteLine($"{ConfigurationManager["Artifact:Service:MaxConcurrentCall"]}");
    Console.WriteLine($"{ConfigurationManager["Artifact:Service:IsPublic"]}");

    var service = new Service();
    ConfigurationManager.GetSection("Artifact:Service").Bind(service);
    Console.WriteLine($"{service.Name},{service.MaxConcurrentCall},{service.Type},{service.IsPublic}");
}
```

Kodun kilit noktası builder örneği üzerinden çağırılan AddInMemoryCollection metodu. Bu metoda parametre olarak parameters isimli Dictionary tipinden bir koleksiyon verilmekte. Dictionary, key:value şeklindeki konfigurasyon mantığına uygun olduğu için biçilmiş kaftandır. Tabii alt elemanlar için yine: ayracına başvurulur. Örnek koleksiyonda Region ve Artifact aynı seviyede yer alan elemanlardır. Artifact altında Service ve onun altında da Name, MaxConcurrentCall, Type ve IsPublic isimli nitelikler yer almaktadır.

Build çağrısı sonrası bu bilgilere ConfigurationManager isimli IConfigurationRoot arayüzü üzerinden erişilebilir. Dahası bellekte konuşlandırılan bu konfigurasyon içeriği herhangibir seviyesi için bir nesneye de bağlanabilir. Service isimli aşağıdaki sınıfı göz önüne aldığımızda,

```csharp
public class Service
{
    public string Name { get; set; }
    public int MaxConcurrentCall { get; set; }
    public string Type { get; set; }
    public bool IsPublic { get; set; }
}
```

GetSection ("Artifact:Service").Bind (service) çağrımı ile Artifact altındaki Service içeriğinin ilgili nesne örneğine bağlanması sağlanmış olur. Bu noktadan sonra MaxConcurrentCall, Name gibi özelliklere yönetimli kod üzerinden erişilebilinir. Fonksiyonun çalışma zamanı çıktısı aşağıdaki gibidir.

![customconfig_3.gif](/assets/images/2018/customconfig_3.gif)

Konfigurasyon Parametrelerini Komut Satırından Göndermek

Bir önceki örnekte kullandığımız In-memory çözümünde, parametre değerlerinin komut satırından gönderilmesi de mümkündür. Bu güzel ve ilginç bir kullanım şekli olsa de pek çok durumda işimize yarayabilir. Peki nasıl yapabiliriz? Aşağıdaki metodu ConfigSupervisor sınıfımıza ekleyerek devam edelim (Başta eklediğimiz Microsoft.Extensions.Configuration.CommandLine paketi bu örnek için gerekli)

```csharp
public void ExecuteCommandLineSample(string[] args=null)
{
    var builder=new ConfigurationBuilder();
    var connection=new Dictionary<string,string>{
        {"Connection:Value","data source=aws;provider:amazon;"},
        {"Connection:Name","aws"}
    };
    builder
    .AddInMemoryCollection(connection)
    .AddCommandLine(args);

    ConfigurationManager=builder.Build();
    Console.WriteLine($"Connection : {ConfigurationManager["Connection:Value"]}");
    Console.WriteLine($"Connection : {ConfigurationManager["Connection:Timeout"]}");
}
```

Yine bellekte tutulan bir konfigurasyon içeriği söz konusu. Bunun için generic Dictionary koleksiyonunu kullandık. builder üzerinden çağırdığımız AddCommandLine fonksiyonuna parametre olarak gelen args dizisinin içeriği tahmin edeceğiniz üzere komut satırından gelecek. Kodun ilerleyen satırlarında Connection:Value ve Connection:Name değerlerini ekrana bastrırıyoruz. Main kodunun içeriğini de aşağıdaki hale getirelim. Tek yaptığımız Main fonksiyonuna gelen args değişkenini ExecuteCommandLineSample çağrısına parametre olarak geçmek.

```csharp
static void Main(string[] args)
{
    ConfigSupervisor rubio = new ConfigSupervisor();
    rubio.ExecuteCommandLineSample(args);
}
```

Eğer programımızı aşağıdaki gibi çalıştırırsak konfigurasyon içeriğinin bizim istediğimiz gibi değiştiğini görürüz.

```bash
dotnet run Connection:Value="Azure;timeout=1000;region=EU-1" Connection:Name="azure"
```

Tabii bu parametreyi vermeden uygulamayı çalıştırırsak varsayılan Connection:Value ve Connection:Name değerlerine ulaşırız. Bu arada tüm parametreleri detaylı olarak girmek zorunda değiliz. İsimle ulaştığımız için sadece değiştirmek istediklerimizi girebilir veya farklı sıralarda atamalar yapabiliriz. Aşağıdaki çalışma zamanı sonuçlarına bu anlamda bakabilirsiniz.

![customconfig_8.gif](/assets/images/2018/customconfig_8.gif)

Konfigurasyon yönetimi ile ilgili daha pek çok şey var (Ben MSDN'in [şu adresteki](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?tabs=basicconfiguration) oldukça doyurucu dokümanını izleyerek öğrenmeye çalışıyorum) Örneğin özel bir Entity Framework provider'ının oluşturulması, komut satırı argümanlarında switch mapping tekniğinin kullanılması gibi konulara bu adresten bakılabilir. Şimdilik benden bu kadar. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örnek kodlara Git üzerinden de erişebilirsiniz.](https://github.com/buraksenyurt/dotnetcore/tree/master/CustomConfig)
