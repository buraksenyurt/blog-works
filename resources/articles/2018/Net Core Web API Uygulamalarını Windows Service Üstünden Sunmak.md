---
title: ".Net Core Web API Uygulamalarını Windows Service Üstünden Sunmak"
pubDate: 2018-10-17 07:53:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - XML
  - C#
  - .NET
  - ASP.NET
  - ASP.NET Core
  - WCF
  - Web API
  - HTTP
  - IIS
  - Visual Studio
---

# .Net Core Web API Uygulamalarını Windows Service Üstünden Sunmak
![scrummy.jpg](images/scrummy.jpg)

Merhaba Arkadaşlar,

Scrum'lı çevik günlerimiz tüm hızıyla devam ediyor. Artık takımın kapasitesi standart çizgiye oturmaya başladı. Daha verimli çalışıyor ve daha iyi değerler üretebiliyoruz. Bunun bana olan en büyük artılarından birisi de yeni bir şeyler araştıracak vakit bulabilmek. Nitekim takım üyelerinin değişime uygun olacak şekilde kendisini sürekli yeniliyor olması lazım. Bu açıdan motive edildiğimizi ifade edebilirim. Gündem maddemiz ise uzun zamandır pek yanına uğramadığımız ama mutlaka sistemlerimizin bir yerlerinde koşan Windows Service'ler.

Micorosft, çıkarttığı 2.1 sürümü ile birlikte Asp.Net Core'a bazı yeni özellikler ekledi. Ben de geçtiğimiz zaman zarfında bunlardan bir kısmını inceleme fırsatım buldum. İlgimi çeken yeniliklerden birisi de artık Asp.Net Core uygulamalarını Windows Service üzerinden sunabiliyor olmamız. Bu bana eski WCF'li günlerimi anımsattı. Orada da servisleri self-host mekanizması dışında (Console gibi çalıştırmak), IIS'de veya Windows Service olarak yayınlama şansına sahiptik (hala sahibiz) Yeni gelen kabiliyet tahmin edeceğiniz üzere Windows tabanlı sistemler için geçerli.

Söz konusu yeniliği inceleyeceğimiz bu yazımızda bir Web API hizmetini host etmeyi öğreneceğiz. Dilerseniz vakit kaybetmeden işlemlerimize başlayalım. Öncesinde sistemimizde [.Net Core 2.1](https://www.microsoft.com/net/download/windows)'ın yüklü olduğundan emin olmakta yarar var. Örneği Windows 10 işletim sisteminde, Visual Studio Code kullanarak yapacağım.

Web API Uygulamasının Oluşturulması

İlk adım olarak dummy web api uygulamasını oluşturalım. Artık adımız gibi bildiğimiz komutla house isimli bir proje oluşturdum.

```bash
dotnet new webapi -o house
```

Windows Service mekanizmasını Middleware tarafında kullanabilmek ve çalışma zamanında gerekli yönlendirmeleri yaptırmak için Microsoft.AspNetCore.Hosting.WindowsServices paketinin projeye eklenmesi gerekiyor. Bu yüzden terminalden şu şekilde ilerliyoruz.

```bash
dotnet add package Microsoft.AspNetCore.Hosting.WindowsServices
dotnet restore
```

Gelelim kod tarafına. Proje dosyasında yapmamız gereken değişiklikle başlayalım. house.csproj dosyasını açıp PropertyGroup altına eğer yoksa RuntimeIdentifier bilgisini eklememiz gerekiyor.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>
    <RuntimeIdentifier>win7-x64</RuntimeIdentifier>
  </PropertyGroup>

  <ItemGroup>
    <Folder Include="wwwroot\" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.AspNetCore.Hosting.WindowsServices" Version="2.1.1" />
  </ItemGroup>

</Project>
```

Standart şablon olarak gelen Controller'lar da bir değişiklik yapmayacağız. Amacımız servis geliştirmekten ziyade host ettirmek. Ancak program sınıfının main metodunda servisin bir Windows Service olarak ayağa kalkacağını belirtmemiz gerekiyor. Bu yüzden Program.cs içeriğini aşağıdaki gibi değiştireceğiz.

```csharp
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;

namespace house
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var rootFolder = Path.GetDirectoryName(exePath);
            var host = WebHost.CreateDefaultBuilder(args)
                                .UseContentRoot(rootFolder)
                                .UseStartup<Startup>()
                                .UseUrls("http://localhost:3403")
                                .Build();

            host.RunAsService();
        }
    }
}
```

O anki güncel Process'ten yararlanarak exe'nin adını ve yerini buluyoruz. Bu bilgi UseContentRoot tarafından kullanılacak. Windows Service hosting sürecini devreye sokmak içinde RunAsService metodundan yararlanılıyor.

Publish İşlemi

Sırada publish işlemleri var. Uygulamanın release edilebilir bir sürümünü oluşturabilmek için aşağıdaki komutla devam edebiliriz.

```bash
dotnet publish --configuration Release
```

Şirket bilgilsayarımdaki yapıya göre ilgili artifact'ler C:\Projects\tips_tricks\house\bin\Release\netcoreapp2.1\win7-x64\publish klasörüne çıkartıldı. Burada en dikkat çekici nokta house.exe isimli bir dosyanın oluşması. Bildiğiniz üzere web uygulamaları dll olarak dağıtılmakta ancak Windows Service'ler için çalıştırılabilir program dosyası (exe) olması gerekiyor. Bu dosyada üretildiğine göre artık komut satırından Windows Servisi ile ilgili işlemlerimize başlayabiliriz.

SC ile Servisin Hazırlanması

sc aracından ve create komutundan yararlanarak bir servis oluşturabiliriz. Oluşan servisin o anki durumunu öğrenmek için query, Servisi başlatmak içinse start parametresinden yararlanıyoruz. Aynen aşağıda görüldüğü gibi.

```bash
sc create HouseAPIService binPath= "C:\Projects\tips_tricks\house\bin\Release\netcoreapp2.1\win7-x64\publish\house.exe"
sc query HouseAPIService
sc start HouseAPIService
sc query HouseAPIService
```

create komutunda servisin adını ve exe dosyasının olduğu konumu belirtiyoruz. Çok uzun bir adres olduğu için siz denemelerinizi yaparken.Net Core uygulamanızı farklı bir lokasyona çıkartabilirsiniz. --output parametresi bu noktada yardımcı olacaktır. Ben hat ettim siz etmeyin.

Yukarıdaki komutları çalıştırdıktan sonra aşağıdaki ekran görüntüsündeki sonuçları aldım.

![corewins_1.gif](images/corewins_1.gif)

Görüldüğü üzere HouseAPIService başarılı bir şekilde çalışır konuma geldi. State geçişlerinden bunu görebiliriz. Buna göre artık http://localhost:3403/api/values adresine talepte bulunabiliriz. İşte postman'den elde ettiğim sonuç.

![corewins_2.gif](images/corewins_2.gif)

Artık Windows Service üzerinden yayında olan bir Web API hizmetimiz var. Windows servisini durdurursak tahmin edeceğiniz üzere Web API'yi kullanamayız.

![corewins_3.gif](images/corewins_3.gif)

Tabii bu işsiz servisi sistemde tutmak doğru değil. Sonuçta unutulabilir ve boat anchor anti-pattern hali oluşabilir. Bu servisi silmek için sc'nin delete komutundan yararlanabiliriz.

```text
sc delete HouseAPIService
```

Artık bu servis hayatta değil. Hepsi bu:)

Böylece bir Web API servisini Windows Service üzerinde nasıl yayınlayabileceğimizi öğrenmiş olduk. Artık Asp.Net Core uygulamaları için elimizde alternatif bir host seçeneği daha var. Asp.Net Core 2.1 ile birlikte gelen diğer özellikleri de zaman içerisinde incelemeye çalışacağım. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
