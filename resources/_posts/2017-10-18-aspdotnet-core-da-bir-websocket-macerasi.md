---
layout: post
title: "Asp.Net Core'da Bir WebSocket Macerası"
date: 2017-10-18 14:29:00 +0300
categories:
  - asp-dotnet-core
tags:
  - asp.net-core
  - websockets
  - web-socket
  - http
  - tcp
  - duplex-communication
  - signalr
  - ws
  - async
  - await
---
Meşhur ve özlenen telefon markası Nokia'nın o başarılı sloganını hatırlıyor musunuz?, "Nokia, connecting people":) İşte bugünkü konumuz da o jeneriği aratmayacak türden. "WebSockets, connecting applications."(Burayı o adamın ses tonu ile zihninizde canlandırın derim) Evet berbat bir giriş oldu ama olsun. Gelelim konumuza.

![websockets_5.gif](/assets/images/2017/websockets_5.gif)

Web Sockets aynı anda çift yönlü haberleşmeye olanak sağlayan bir protokol olarak karşımıza çıkmakta. HTTP'nin klasik Request/Response modelinden farklı çalışan bu model ile masaüstü uygulamalarının eş zamanlı olarak karşılıklı haberleşebilme yetenekleri web ortamına da taşınmış oluyor. Böylece akıllı telefonlardan, tabletlerden, masaüstü uygulamalarından, tarayıcılardan bir sunucu ile Web Socket haberleşmesi gerçeklenebiliyor. Bu iletişim de istemci ve sunucu birbirlerini beklemeden eş zamanlı olarak karşılılklı paket alışverişinde bulunmakta. Chat arabirimleri, Bot programları, borsa ürünleri, gerçek zamanlı oyunlar ve benzeri örneklerde Web Socket modeli kullanılabiliyor. Aslında TCP haberleşmesini web için çift yönlü iletişime dönüştüren bir model olduğunu da belirtebiliriz. En temelinde ise handshaking adı verilen bir kavrama dayanıyor. Sunucu, kendisine bağlı olan istemcileri ile senkronize kalıyor. Web Socket'lerin en önemli yanları ise gerçek anlamda eşzamanlılık ve optimize edilmiş bir mesaj trafiği sunması.

> WebSockets protokolü Internet Engineering Task Force tarafından önerilmektedir. Standarda ait detaylı bilgiler için [IETF'un ilgili sayfasına](https://tools.ietf.org/html/rfc6455) bakabilirsiniz.

Epey zamandır varlığından haberdar olduğumuz bu modelin Asp.Net tarafında daha da geliştirilmiş bir versiyonu var; SignalR. Ancak Asp.Net Core tarafına baktığımızda SignalR desteğinin henüz tam olarak gelmediğini görüyoruz (En azından araştırmayı yaptığım tarih itibariyle durum böyleydi. Gerçi yazıyı yayınladığım tarih itibariyle desteği gelmiş olsa da sorun değil. Ben Web Sockets kullanımını merak ettiğim için yazıda ısrar ettim) Şuradaki [github adresinden](https://github.com/aspnet/SignalR) Asp.Net Core tarafındaki SignalR gelişmeleri takip edilebilirsiniz. Diğer yandan 17 Eylül tarihindeki haber göre [Alpha sürümünü de test edebilirsiniz](https://blogs.msdn.microsoft.com/webdev/2017/09/14/announcing-signalr-for-asp-net-core-2-0/) ki ben de bir ara deneyeceğim.

> Bu arada WebSockets ile geliştirme yapmak için temel HTML ve Javascript bilgisi yeterlidir. Hatta Tutorials Point'te güzel bir eğitim seti de bulunuyor. İsterseniz platform bağımsız olarak [bu adresten](https://www.tutorialspoint.com/websockets/index.htm) de çalışabilirsiniz.

Neyse ki Asp.Net Core tarafında WebSockets desteği mevcut. Tabii SignalR bu işin nirvanası gibi duruyor ama ben Web Socket kullanımını merak etmekteyim. O zaman kolları sıvayalım ve yeni bir Asp.Net Core projesi oluşturarak işe koyulalım. Bakalım olayın özünü anlayabilecek miyim?

Projenin Oluşturulması

Örneği her zamanki gibi şirket bilgisayarında geliştirmekteyim. Dolayısıyla Windows 7 tabanlı bir işletim sisteminde.Net Core 2.0 ortamı olduğunu ifade edebilirim. Önceki yazıda kurdurttuğum ortam. Terminalden aşağıdaki komutu vererek HelloWebSockets isimli projeyi oluşturarak başlayalım.

```bash
dotnet new web -o HelloWebSockets
```

![websockets_1.gif](/assets/images/2017/websockets_1.gif)

Sıradaki operasyon Microsoft.AspNetCore.WebSockets paketinin aşağıdaki terminal komutu ile projeye eklenmesi. Bu sayede System.Net.WebSockets isim alanı altında yer alan tipleri kullanabileceğiz.

```bash
dotnet add HelloWebSockets.csproj package Microsoft.AspNetCore.WebSockets
```

![websockets_2.gif](/assets/images/2017/websockets_2.gif)

Kodlar

Artık gerekli kodları yazarak ilerleyebiliriz. İlk olarak Startup sınıfındaki Configure metoduyla uğraşacağız. Burada çalışma zamanına WebSockets kullanacağımızı bildireceğiz ve istemci ile sunucu arasındaki iletişimi açıp hareket eden mesajların kontrolünü sağlayacağız. Amacımız istemciden gelen mesajı yakalamak ve ona bir şeyler söylemek. Örneğin bugünkü şanslı numarasının ne olduğunu...

Startup.cs

```csharp
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;

namespace HelloWebSockets
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.Use(async (ctx, nextMsg) =>
            {
                Console.WriteLine("Web Socket is listening");
                if (ctx.Request.Path == "/nokya")
                {
                    if (ctx.WebSockets.IsWebSocketRequest)
                    {
                        var wSocket = await ctx.WebSockets.AcceptWebSocketAsync();
                        await Talk(ctx, wSocket);
                    }
                    else
                    {
                        ctx.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await nextMsg();
                }
            });

            app.UseFileServer();
        }

        private async Task Talk(HttpContext hContext, WebSocket wSocket)
        {
            var bag = new byte[1024];
            var result = await wSocket.ReceiveAsync(new ArraySegment<byte>(bag), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                var incomingMessage = System.Text.Encoding.UTF8.GetString(bag, 0, result.Count);
                Console.WriteLine("\nClient says that '{0}'\n", incomingMessage);
                var rnd = new Random();
                var number = rnd.Next(1, 100);
                string message = string.Format("Your lucky Number is '{0}'. Don't remember that :)", number.ToString());
                byte[] outgoingMessage = System.Text.Encoding.UTF8.GetBytes(message);
                await wSocket.SendAsync(new ArraySegment<byte>(outgoingMessage, 0, outgoingMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await wSocket.ReceiveAsync(new ArraySegment<byte>(bag), CancellationToken.None);
            }
            await wSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
```

Kodlar biraz korkutucu görünebilir. Bir sürü async, await görmekteyiz. Asenkron bir şeyler oluyor. Kısaca üzerinden geçelim. İlk odaklanacağımız nokta Configure metodu. Web Sockets kullanacağımızı UseWebSockets fonksiyonu ile ortama bildiriyoruz. Takip eden satırda ise istemcileri dinlemeye başlıyoruz. Burası asenkron çalışan bir kod bloğu. İçinde /nokya adresine gelen bir talep olup olmadığına bakılıyor. Eğer bu adrese gelen bir talep varsa ve gelen bu talep bir Web Sockets isteğiyse (ki bunun ws:// protokolü ile başlayan bir talep olması gerekiyor) bir socket nesnesi yakalıyor ve Talk isimli fonksiyonu çağırıyoruz (Yine asenkron olarak tabii) Eğer gelen istek /nokya adresine yapılmamışsa bir sonraki mesajı bekleyecek şekilde dinleme işlemine devam ediliyor. Özetle o anki context'i ve mesajı yakalayıp bunun bir Web Sockets iletişimi olup olmadığının anlaşılması ve buna göre Talk operasyonunun çağırılması söz konusu.

Talk operasyonu içerisinde istemciden gelen mesajın terminale basılması ve rastgele üretilen bir sayı değerinin geri gönderilmesi için gerekli kodlar yer alıyor. İstemcinin gönderdiği mesajın yakalanması için ReceiveAsync isimli metoddan yararlanılmakta. Metodun ilk parametresi gelen içeriği bir Byte dizisine alıyor. Eğer aradaki iletişim açıksa (CloseStatus.HasValue ile bu kontrol edilmekte) bu byte içeriğini string'e dönüştürmek yeterli. Bağlı olan istemciye soket üzerinden mesaj göndermek içinse SendAsync metodundan yararlanılmakta. Trafik byte array'ler ile çalışıyor bu nedenle string operasyonları için UTF8 tabanlı Encoding mekanizmalarından yararlanılmakta.

Program.cs

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelloWebSockets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5556/") 
                .UseStartup<Startup>()
                .Build();
    }
}
```

Program.cs içeriğinde çok fazla müdahalemiz yok. Sadece kullanılacak port bilgisini ayarlıyoruz. Buna göre http://localhost:5556 adresinden yayın yapacağız ve istemciler ws://localhost:5556/nokya adresini kullanarak Web Sockets temelli iletişimi gerçekleştirebilecekler. Örnekte istemci olarak index.html sayfasını ele alacağız. wwwroot klasöründe konuşlandıracağımız sayfanın içeriğini aşağıdaki gibi yazabiliriz.

index.html

```text
<html>
<head>
    <title>Asp.Net Core ile Web Sockets Kullanımı</title>
</head>
<body>
<button id="btnConnect" type="submit">Connect</button><br/>
Message : <input id="lblMessage" style="width:300;" /><br/>
<button id="btnSendMessage" type="submit">Send Message</button><br/>
<button id="btnDisconnect" type="submit">Disconnect</button><br/>
<script>
    var btnConnect = document.getElementById("btnConnect");
    var btnSendMessage=document.getElementById("btnSendMessage");
	var lblMessage=document.getElementById("lblMessage");
	var btnDisconnect=document.getElementById("btnDisconnect");
	var socket;
	
	btnConnect.onclick = function() {
            socket = new WebSocket("ws://localhost:5556/nokya");
            socket.onopen = function (e) {				
                console.log("Connected",e);
            };
            socket.onclose = function (e) {
                console.log("Disconnected",e);
            };
            socket.onerror = function(e){
				console.error(e.data);
			};
            socket.onmessage = function (e) {
                console.log(e.data);
            };
		}
	
	btnSendMessage.onclick = function () {
            if (!socket || socket.readyState != WebSocket.OPEN) {
                console.error("Houston we have a problem! Socket not connected.");
            }
            var data = lblMessage.value;
            socket.send(data);
            console.log(data);
        }
		
	btnDisconnect.onclick = function () {
			if (!socket || socket.readyState != WebSocket.OPEN) {
				console.error("Houston we have a problem! Socket not connected.");
			}
			socket.close(1000, "Closing from Apollo 13");			
        }
	
</script>
</body>
```

Temel olarak bağlantıyı açmayı, text kutusuna yazılan içeriği karşı tarafa göndermeyi, aradaki iletişimi debug penceresinden izlemeyi ve son olarak da iletişimi başarılı şekilde sonlandırmayı hedefliyoruz. İçerikteki en değerli kodlarımız btnConnect butonuna basıldığında çalışıyor. Dikkat edileceği üzere bir WebSocket nesnesi üretilmekte. Adres olarak da ws://localhost:5556/nokya adresi kullanılıyor. Adresin ws ile başladığına dikkat edelim. Oluşturulan WebSockets nesnesi için bazı olaylar tanımlanıyor. Bağlantı açıldığında, kapatıldığında, sunucudan mesaj alındığında veya bir hata oluştuğunda. Sunucuya mesaj göndermek için WebSocket nesnesinin send fonksiyonundan yararlanılmakta.

Sonuçlar

Öncelikli olarak

```bash
dotnet run
```

komutuyla web sunucusunun çalıştırılması gerekiyor. Sonrasında herhangibir tarayıcıdan (Örnekte Google Chrome kullanılmıştır) http://localhost:5556/ adresine gidilmesi yeterli. Önce bağlanıp ardından mesaj gönderebiliriz. Sonrasında da iletişimi kapatmamız yeterli. İşte benim elde ettiğim sonuçlar.

Tarayıcı tarafı;

![websockets_3.gif](/assets/images/2017/websockets_3.gif)

Dikkat edileceği üzere aradaki haberleşme console ekranına düşmektedir (Chrome'da debug penceresini açmak için F12 tuşunu kullanabilirsiniz) Ayrıca "Don't remember" nedir yahu...

Sunucu tarafı

![websockets_4.gif](/assets/images/2017/websockets_4.gif)

Sunucu tarafında da istemciden gelen mesajların başarılı bir şekilde yakalandığı görülmektedir.

Dilerseniz n sayıda istemciyi bağlayabilir ve herbirinin ayrı bir konuşma içerisinde değerlendirildiğini de test edebilirsiniz. Aşağıdaki ekran görüntüsünde olduğu gibi.

![websockets_6.gif](/assets/images/2017/websockets_6.gif)

Asp.Net Core ile Web Sockets haberleşmesi yapmak oldukça kolay. Bu örneği tabii diğer platformalarda da denemek lazım. İstemciyi çeşitlendirebiliriz. Bir mobil uygulama, desktop uygulaması veya farklı bir servis dahi olabilir. Hatta örnek daha da zenginleştirilebilir ve biraz oyunlaştırılabilir. Bir sayı tahmin oyununu bu şekilde yapmaya çalıştığınızı düşünün. Hatta istemcinin ekranına soru seti gönderip, cevaplarına göre hikayeleştirdiğiniz bir senaryoyu işletebilirsiniz. Şu sıralar tabletten oynadığım My Cafe isimli oyunda restorana gelen müşterilerim ile aramda böyle hikayeleştirilmiş soru-cevap olayları söz konusu. Acaba Web Sockets mi kullanılmış merak etmekteyim. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
