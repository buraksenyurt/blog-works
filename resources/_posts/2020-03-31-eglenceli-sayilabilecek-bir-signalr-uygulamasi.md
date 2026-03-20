---
layout: post
title: "Eğlenceli Sayılabilecek bir SignalR Uygulaması"
date: 2020-03-31 21:44:00 +0300
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - javascript
  - dotnet
  - linq
  - async-await
  - concurrency
  - generics
  - dependency-injection
  - github
  - dependency-management
---
Turşunun iyisi limonla mı olur yoksa sirkeyle mi? Neşeli Günler'i izleyenleriniz rahmetli Münir Özkul ve Adile Naşit arasındaki atışmayı gayet iyi bilir:) Arada bir eski de olsa her yönüyle bizlere tarifsiz dersler veren yapımlarımızı izlemek gerekiyor. Tabii yine de turşunun iyisi limonla mı olur yoksa sirkeyle mi olur pek bilemiyorum. İyisi mi bunu bir SignalR servisine bırakalım. Ne dersiniz?

![neseligunler.jpg](/assets/images/2020/neseligunler.jpg)

Birkaç kez.Net Core'un farklı versiyonları ile SignalR uygulaması yazmaya çalışmıştım. Tazelenme sürecindeki skynet'te ona yer vermezsem olmazdı. Bu sefer biraz daha eğlenceli olsun istedim. Amacım siteye bağlanan kullanıcıların oy vermek suretiyle iyi turşunun limonla mı yoksa sirkeyle mi yapılacağına karar vermelerini sağlamak:D Yarışma gibi olan oylamada birden fazla kullanıcı bağlanınca sonuçları anlık olarak progress bar'lar üzerinde de görünsün istiyorum. En azından klasik chat uygulamasından farklı olacak bir antrenmana başlıyoruz diyebilirim.

## Ön Hazırlıklar

.Net Core tarafında normal bir web uygulamasını açmak oldukça kolay. Ancak istemcinin SignalR Hub ile iletişim kurması için gerekli Javascript kütüphaneleri haricen yüklenmeli. Söz konusu kütüphaneyi yüklemenin yollarından birisi LibMan (Library Manager) aracını kullanmak. Bu nedenle onu sisteme install ediyoruz. Aşağıda gerekli terminal komutlarını bulabilirsiniz. Libman gerekli Javascript kütüphanelerini (signalr.js ve signalr.min.js) wwwroot/js altına otomatik olarak açıyor.

```bash
dotnet new webapp -o Tursucu
cd Tursucu
mkdir HubStation
touch ./HubStation/VoteHub.cs
touch ./wwwroot/js/votemngr.js

# libman aracı kuruluyor
dotnet tool install -g Microsoft.Web.LibraryManager.Cli

# libman ile gerekli signalr js dosyaları kuruluyor
libman install @microsoft/signalr@latest -p unpkg -d wwwroot/js/signalr --files dist/browser/signalr.js --files dist/browser/signalr.min.js
```

## Kod Tarafı

Gelelim kodlarımıza. Önce VoteHub.cs ile işe başlayalım.

```csharp
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Tursucu.HubStation
{
    public class VoteHub
        : Hub // SignalR başkanı olmanın doğası Hub sınıfından türemektir.
        // Hub sınıfı mesajlaşma alt yapısını ve mesaj dağıtımını kolaylaştırır
    {
        // İstemci ile sunucunun eş zamanlı konuşmasının doğası gereği
        // Asenkron bir metodumuz var.
        // Metot adı istemci tarafındaki Javascript için önemli (invoke kısmına bak)
        public async Task PushVoteMessage(string user,string userChoice)
        {
            // user : Kimden mesaj geliyor
            // userChoice : kullanıcı hangi seçeneği seçiyor. Sirke mi limon mu?
            // GetVoteMessage ismi önemli. Javascript tarafındaki on event'inde yakalancak
            // All ile bağlı olan tüm kullanıcıları gösterdik
            // ve SendAsync ile hepsine GetVote isimli bir mesaj yayınladık
            // Şayet karşı tarafta bağlanıp da bu olayı dinleyen varsa yaşadı
            await Clients.All.SendAsync("GetVoteMessage",user,userChoice);   
        }
    }
}
```

SignalR'ın middleware tarafına da eklenmesi gerekiyor. Bunu Startup.cs içerisinde yapabiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tursucu.HubStation;

namespace Tursucu
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSignalR(); // SignalR, Dependency Injection mekanizmasına bildirilir
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                // endPoint'lere hub için gerekli route tanımı eklenir
                // /voting ile gelindiğinde bu işi VoteHub ele alacak
                endpoints.MapHub<VoteHub>("/voting");
            });
        }
    }
}
```

Şimdi de votemngr.js isimli Javascript dosyasımızı geliştirelim.

```javascript
"use strict";

// Çalışma zamanında sayfadan gelen voting route kullanılarak bağlantı nesnesi oluşuyor
var connection = new signalR.HubConnectionBuilder().withUrl("/voting").build();

// bunları limoncu ve sirkecileri hesaplamak için kullanıyorum ancak önemli bir probleme neden oluyor?
var lemonPoints = 0;
var vinegardPoints = 0;

// Sunucu tarafından yayınlanan GetVote mesajını yakalıyor
connection.on("GetVoteMessage", function (user, choice) {

    // Güncel saat, kullanıcı ve yaptığı seçim bilgilerin
    // index sayfasının altındaki voices isimli listeye ekliyorum
    var today = new Date();
    var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();

    var voteMessage = "[" + time + "] " + user + " '" + choice + "' dedi.";
    var summary = document.getElementById("voices");
    var lineItem = document.createElement("li");
    lineItem.textContent = voteMessage;
    summary.insertBefore(lineItem, summary.childNodes[0]);

    // seçime göre puanları bir birim arttırıyorum
    if (choice == "limon")
        lemonPoints++;

    if (choice == "sirke")
        vinegardPoints++;

    // progressBar elementlerini yakalayıp güncel değerlere göre şekillendiriyorum
    var prgLemon = document.getElementById("prgLemon");
    prgLemon.innerHTML = "LİMON = " + lemonPoints;
    prgLemon.style.width = lemonPoints + "%";

    var prgVinegard = document.getElementById("prgVinegard");
    prgVinegard.innerHTML = "SİRKE = " + vinegardPoints;
    prgVinegard.style.width = vinegardPoints + "%";
});

// Sunucu ile aradaki bağlantı sağlandığında çalışıyor
connection.start().catch(function (err) {
    return console.error(err.toString());
});

// sayfadaki sendVote isimli button kontrolünün click olayını dinliyor
document.getElementById("sendVote").addEventListener("click", function (event) {

    // Kullanıcının girdiği bilgiyi alıyorum
    var user = document.getElementById("participantName").value;
    if (!user) {
        user = "[isimsiz]";
    }

    // RadioButton kontrollerinden hangisini seçtiğini buluyorum
    // neden name elementine voteOption diye ortak bir isim verdik
    // şimdi daha net oldu
    var choice = document.querySelector('input[name = voteOption]:checked').value;

    // Açık kanalı kullanarak PushVote isimli bir mesaj yayınlıyoruz
    // Parametre olarak kullanıcıyı ve yaptığı seçimi gönderiyoruz
    connection.invoke("PushVoteMessage", user, choice).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});
```

Pek tabi arayüze de biraz dokunmak gerekiyor. Berbat bir tasarımcıyımdır ama olsun. index.cshtml'i aşağıdaki gibi tasarlayabiliriz. Siz daha güzelini yapın:)

```text
@page
@model IndexModel
@{
    ViewData["Title"] = "Sence Hangisi?";
}

<div class="container">
    <div class="row">
        <div class="col-8">
            <h1 class="display-4">Sence Hangisi?</h1>
            <hr />
            Önce ismini söyle? <input type="text" id="participantName"/>
            <!-- Katılcımıyı örneğin gmail login ile sistem nasıl alırız? -->
            <hr />
            Sence turşunun iyisi hangisiyle yapılır? :)
            <br/>
            <label id="lblResult"></label>
            <input type="radio" name="voteOption" value="limon" />
            <label>Limonla</label>
            <br/>
            <div class="progress" style="height: 30px;">
                <div id="prgLemon" class="progress-bar bg-warning" role="progressbar" style="width: 0%;" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100">0</div>
            </div>
            <input type="radio" name="voteOption" value="sirke" />
            <label>Sirkeyle</label>
            <br/>
            <div class="progress" style="height: 30px;">
                <div id="prgVinegard" class="progress-bar bg-success" role="progressbar" style="width: 0%;" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100">0</div>
            </div>
            <hr/>
            <input type="button" id="sendVote" value="Oyla" />
            <ul class="list-group overflow-auto" id="voices"></ul>
        </div>
</div>
<script src="~/js/signalr/dist/browser/signalr.js"></script>
<script src="~/js/votemngr.js"></script>
```

ve index.cshtml.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Tursucu.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
```

## Çalışma Zamanı

Uygulamamızı aşağıdaki komutu kullanarak çalıştırabiliriz. Şahsen watch zorunlu değil ama ben örneği tasarlarken çok defa kodda değişiklik yaptım. Tekrar tekrar başlatmaktansa değişikliklerin otomatik olarak algılanıp çalışma zamanına yansımasını istedim. Bu nedenle watch komutunu kullandım diyebilirim. İşte çalışma zamanına ait örnek bir ekran görüntüsü. Üç farklı tarayıcı ile oylama yapıyoruz.

```bash
dotnet watch run
```

![skynet_15_Screenshot_1.png](/assets/images/2020/skynet_15_Screenshot_1.png)

## Uygulamada Koca Bir Bug!

Örneği geliştirirken içinden pek de çıkamadığım bir sorunla da karşılaştım. Birden fazla kullanıcı kendi tarayıcısını kullanarak sayfayı açtı ve oylamaya başladı. Herhangi birisi sayfayı tazelerse ondaki değerler sıfırlanıyor ama diğerleri kaldığı yerden devam ediyor. Yani herkes aynı oylama sonucunu göremiyor. Çünkü votemngr.js başındaki toplam değerleri tutan değişkenler sayfa tazelenip istemciye tekrar gönderildiklerinde sıfır değerine düşüyor. Peki bu problemi nasıl çözebiliriz? Lütfen yazının altındaki yorum kısmını kullanarak bana akıl verin.

Örnek uygulamanın kodlarına [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2015%20-%20Funny%20SignalR)ndan ulaşabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
