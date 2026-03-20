---
layout: post
title: "Nginx Üzerinde Web API Servisi Çalıştırmak"
date: 2018-01-05 08:54:00 +0300
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - aspnet
  - asp-dotnet-core
  - wcf
  - rest
  - web-api
  - http
  - iis
  - authentication
  - authorization
  - threading
  - concurrency
  - performance
  - microservices
---
Animasyon izlemek keyif aldığım hobilerden birisi. İzlediğimde bende derin izler ve duygular bırakmış uzun metrajlı bir çok anım var. Bazen filmde geçen bir söz bazen karakterin gülme krizine sokan bir davranışı ya da başına gelen dramatik bir olay. Geçenlerde bilmem kaçıncı kez izlediğim [Sing](http://www.imdb.com/title/tt3470600/) filmi de benim uzun metraj anı defterim arasında yer alanlardan. Maddi sorunlar nedeniyle banka ile başı dertten kurtulamayan Buster Moon, filmin bir yerinde tam bütün ümitlerin tükendiği noktada şöyle bir cümle sarf ediyor;

![nginxcore_1.gif](/assets/images/2018/nginxcore_1.gif)

"When you've reached rock bottom, there's only one way to go, and that's up!"

Türkçe metrajlı çevirisinde, "dibe indiğin zaman gidilecek tek bir yön kalır. O da, YUKARISIIII!" gibi bir şeydi yanlış hatırlamıyorsam. Benim içinde 2017 sonu ve 2018 başlangıcı bu sözde ifade edilene benzer oldu diyebilirim. Şimdi merkez stüdyolarımıza bağlanıyoruz.

Bir zamanlar üzerinde çalıştığım bir vakada, WCF ile yazılmış bir REST servisinin Authorization mekanizmasını özelleştirip, IIS üzerinden SSL ile host etmek için uğraşmaktaydım. Servisi yazmak, IIS'e atmak, IIS tarafında deneysel sertifika üretip siteyi SSL ile çağrılabilir hale getirmek, ServiceAuthorizationManager türevli sınıfı devreye alarak yetkilendirme sürecini ele almak kolaydı. Birkaç küçük ince ayarlama derken kendimi test sahasında buldum. Ne var ki bir şekilde servisin ayağa kalkması sırasında hatalar alıyordum. Sonunda olan oldu ve uğraştığım senaryodan sıkılıp bunaldığım bir noktaya geldim.

Kendi kendime "ben bunu West-World ya da Gondor üzerinde deneyeyim" diyordum. Her ikisi de Ubuntu 16.04 versiyonlarına sahip bilgisayarlardı. Kolları sıvadım ve bir Web API servisi oluşturmak üzere terminalin başına geçtim. Ne var ki ortada önemli bir sorun vardı. Linux ve IIS. Ovv yooo...Tabiki böyle bir birliktelik olamazdı. Eldeki alternatiflerse belliydi. Apache Server, Lighttpd ve Nginx. Gözüme kestirdiğimse Nginx oldu. Konu bir anda bambaşka bir yere geldi tabii. Şimdiki amacım, West-World'de kuracağım NGinX ortamı üzerinde örnek bir Asp.Net Core Web API servisini host ettirmekti.

Nginx

[Nginx](https://nginx.org/en/) (Engine X olarak telafuz ediliyor) Kazakistan Almatı doğumlu bilgisayar programcısı Igor Sysoev (soyisminde sys var) tarafından 2002 yılında geliştirilmeye başlanmış ve 2004 yılında ürünleşmiş açık kaynak bir web sunucusudur. İlk olarak mail.ru için mail sunucusu olarak geliştirilmiş ama sonrasında çok daha geniş yetenekler kazanarak web siteleri için Apache'den çok daha hızlı çalışabilen bir ürün haline gelmiştir. Henüz doğrulayamadığım ama genel kabul görmüş bazı performans testlerine göre muadili olan Apache ve Lighttpd gibi ürünlere göre çok yüksek cevap süreleri ve minimum bellek tüketimi sağlamaktadır. Bu açılardan oldukça popüler olduğunu ifade edebiliriz. Yük dengeleme (Load Balancing), Sanal sunucu (Virtual Host), Otomatik indeksleme ve ters vekil sunucu (Reverse Proxy) gibi temel özellikleri vardır.

> Nginx'in geliştirilme hikayesinin detaylarını okumaya devam ediyorum. Aslında C10K sorunu olarak adlandırılan bir vakaya istinaden geliştirilmiş. C10K, bir web sunucusunun eş zamanlı onbin talep üstünü kaldıramaması olarak ifade ediliyor ([Şu adresten](http://www.kegel.com/c10k.html) Kegel'in konu ile ilgili dokümanına ulaşabilirsiniz) Diğer yandan Nginx, ağırlıklı olarak Apache ile karşılaştırılmakta. Dikkatimi çeken en önemli fark ise Apache'nin Multi-Thread çalışırken Nginx'in talepleri karşılama noktasında Single-Thread çalışma prensibini kullanması.

Kurulum

Bu düşünceler ile birlikte her zamanki gibi çalışma odamın yolunu tuttum. Amacım basit bir Web API hizmetini (standart şablonda üretilecek olan) nginx üzerinde host etmek. Öncesinde West-World'e NGinx'i kurmam gerekiyor. Terminal'den aşağıdaki komutları kullanarak kurulum adımlarını gerçekleştirdim.

```bash
sudo apt-get install nginx
sudo service nginx start
sudo service nginx status
```

![nginxcore_2.gif](/assets/images/2018/nginxcore_2.gif)

install komutu ile kurulum yapıldıktan sonra start ile nginx servisini başlatıyoruz. status ile de güncel durumunu görüyoruz. Yeşil renkteki active (running) yazısını görmek güzel.

Yönlendirme Ayarları

Nginx kurulduktan sonra Reverse Proxy Server olarak çalışması için bir ayar yapmam gerekiyor. Bu sayede Nginx sunucusuna gelen talepler Web API'nin ayağa kalktığı Kestrel sunucusunun ilgili adresine yönlendirilecekler. Pek tabii tam tersi istikamette söz konusu. Aslında istemciler hiçbir şeyin farkında olmayacaklar. Web API'ye tarayıcıdan yapılan HTTP çağrıları aslında Nginx sunucusu tarafından karşılanıp arka taraftaki asıl Kestrel çalışma zamanına akıtılacak. Bunun üzerine /etc/nginx/sites-available/default dosyasını gedit ile açıp içeriğinde aşağıdaki düzenlemeyi yaptım.

```text
server 
{
   listen 81; 
   location / 
   {      
      proxy_pass http://localhost:5000;
      proxy_http_version 1.1;
      proxy_set_header Upgrade $http_upgrade;
      proxy_set_header Connection keep-alive;
      proxy_set_header Host $host;
      proxy_cache_bypass $http_upgrade;
   }
}
```

Normalde localhost:80 şeklinde 80 portunun dinlenmesi söz konusu ama makinede 80 portunu kullanan Apache sunucusunu bozmak istemedim doğruyu söylemek gerekirse. Bu yüzden deneme olması açısından 81 nolu portu ele alıyorum. proxy_pass bildirimine göre http://localhost:81 adresine gelecek olan istekler http://localhost:5000 'e yönlendirilecek ki bu da Web API uygulamasının varsayılan olarak çalıştığı adres (biliyorsunuz bu adresleri senaryoya göre değiştirebiliriz)

Yapılan değişiklikleri devreye alabilmek için terminalden bir kaç işlem daha yapmak gerekiyor.

```bash
sudo nginx -t
sudo nginx -s reload
```

![nginxcore_4.gif](/assets/images/2018/nginxcore_4.gif)

İlk komut ile konfigurasyon için yapılan değişiklikler test edilip bir sorun olup olmadığına bakılıyor. reload komutu da yeni değişikliklerin çalışma zamanına yeniden yüklenmesi için kullanılmakta.

Service Definition Dosyasının Oluşturulması

Şimdi biraz düşünelim. Nginx platform bağımsız bir web sunucusu. Yazacağım Asp.Net Web API uygulaması ise normal şartlarda kendi web sunucusunu (Kestrel) kullanıyor. Her ikisi de farklı Process'ler anlamına gelebilir. Dolayısıyla Nginx'in yazılacak WebAPI servisi özelinde ilgili Kestrel Process'ini ayağa kaldıracağını bilmesi gerekiyor. IIS'in Worker Process çalışma modeline benzer bir yaklaşım olarak düşünelim. Bunun için service uzantılı bir dosya yazmak lazım. Senaryomda bunu kestrel-baseapi.service şeklinde isimlendirdim (Dosyanın oluşturulduğu yer önemli. etc altındaki systemd altındaki system atlında olsun lütfen)

```bash
sudo gedit /etc/systemd/system/kestrel-baseapi.service
```

Dosya içeriği ise şu şekilde

```text
[Unit]
Description=BaseWebAPI(Standard Asp.Net Web API 2.0 project) on NGinx

[Service]
WorkingDirectory=/var/www/core/BaseWebAPI
ExecStart=/usr/bin/dotnet/ /var/www/core/BaseWebAPI/BaseWebAPI.dll
Restart=always
RestartSec=30
SyslogIdentifier=dotnet-BaseWebAPI
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

Dosya içerisinde neler var şöyle bir bakmakta yarar var. Unit, Service ve Install isimli üç parçadan oluşan bir içerik söz konusu. Eminim çok daha fazla detay yazılabiliyordur ama Nginx kaynaklarından öğrendiğim kadarıyla bu içerik yeterli. En önemli bölüm service kısmı. WorkingDirectory ile WebAPI projesinin olduğu yer işaret ediliyor (ki henüz oluşturmadım) ExecStart komutu iki parametre alıyor. İlki dotnet programının yerini işaret etmekte. İkinci parametre ise Web API uygulamasının publish edilen dll dosyasını. Bir başka deyişle Nginx'in talep sonrası dotnet run BaseWebAPI işlemini uygulaması sağlanıyor. Restart ve Restartsec özelliklerine atanan değerler, ilgili web uygulamasının başı belaya girerse 30 saniye sonra tekrardan restart edilmesini belirtmekte. SyslogIdentifier ile de uygulamanın Nginx tarafında izlenirken kullanılacak olan takma ad olarak düşünülmeli. Diğer Environment değerleri ne anlama geliyor henüz bilmiyorum ama öğrenmek için epey zamanım var.

Web API Hizmetinin Yazılması

Standart web api şablonundan bir proje oluşturup bunu uygun klasöre publish etmem gerekiyor. Komut satırından her zaman yaptığım gibi projeyi oluşturuyorum.

```bash
dotnet new webapi -o BaseWebAPI
```

Bu adımdan sonra startup.cs dosyasında bir değişiklik yapılması öneriliyor. Bunun sebebi Reverse Proxy'ye gelen taleplerin NGinx'in istediği Header türlerine dönüştürülmesi gerekliliği.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseWebAPI
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
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
```

Söz konusu dönüşümler için Middleware katmanına müdahale edilmesi lazım. UseForwardedHeaders fonksiyonu bu noktada devreye girmekte. Bu işlemin ardından Publish ile dll üretimini yapmam yeterli. Publish adresi olarak service dosyasında belirttiğim klasörü kullanmayı tercih ediyorum.

```bash
sudo dotnet publish -o /var/www/core/BaseWebAPI
```

Toparlayabilirim

kestrel-baseapi.service ve Web API uygulaması hazır olduğuna göre systemctl (linux tabanlı sistemler için kullanılan servis yöneticisidir. Aslında systemd'nin organizasyonundaki çeşitli servislerin yönetilmesinde kullanılır. Buradaki senaryoda Nginx hizmetleri mevzubahistir) ile service dosyasını çalıştırabilir ve son durumu izleyebilirim. Terminalin başına geçiyorum ve aşağıdaki komutları sırasıyla çalıştırıyorum.

```bash
sudo systemctl enable kestrel-baseapi.service
sudo systemctl start kestrel-baseapi.service
sudo systemctl status kestrel-baseapi.service
```

![nginxcore_6.gif](/assets/images/2018/nginxcore_6.gif)

İlk komut ile yazılan service dosyası devreye alınmakta (disable ile devre dışı kalacağını da belirtelim) İkinci komut ile servis başlatılmakta ve son komutla da anlık durum hakkında bilgi alınabilmekte. Aslında hepsi bu kadar. Tarayıcımdan http://localhost:81/api/values şeklinde talepte bulunduğumda aşağıdaki gibi Web API servisinin çalıştırıldığını görüyorum. İşte bir mutluluk anı daha.

![nginxcore_7.gif](/assets/images/2018/nginxcore_7.gif)

Hatta kestrel-baseapi.service hizmetini pasif hale getirip aynı talebi yaptığımda Nginx'in bana o güzelim 502 hata sayfasını gösterdiğini de görüyorum. Bu gerçekten de istenildiği gibi proxy'nin çalıştığının ispatı aslında.

```bash
systemctl stop kestrel-baseapi.service
```

sonrası durum

![nginxcore_8.gif](/assets/images/2018/nginxcore_8.gif)

Benim için araştırması, öğrenmesi ve yazılması keyifli bir makalenin daha sonuna gelmiş bulunuyoruz. Bu yazıda bir Asp.Net Core Web API uygulamasının Ubuntu'da yüklü Nginx web sunucusu üzerinden nasıl host edilebileceğini incelemeye çalıştım. Microsoft ve.Net ürünlerine aşina olanlar için IIS yerine NGinx'i koyduk diyebiliriz de. Tabii işin çok daha fazla detayı olduğunu biliyoruz. Örneğin Load Balancing mevzusu, SSL tabanlı iletişimin kurulması, Authentication mekanizmaları vb konular var. Bunları da incelemeye çalışacağım. Ancak konuya küçük bir grizgah yaptığımızı da ifade edebilirim sanıyorum ki. Bu arada özellikle Microservice mimarisinin yüksek düzey mimari görünüme sahip basit bir örnek üzerinden incelemek isterseniz Nginx'in [şu adresteki yazısını](https://www.nginx.com/blog/introduction-to-microservices/) şiddetle tavsiye ederim. Renkli kalemlerinizi de hazır edin. Yazıyı okurken mimari çizimleri siz de çizmeye çalışın ve bilginizi pekiştirin derim.

![nginxcore_9.gif](/assets/images/2018/nginxcore_9.gif)

Örnekte Uber benzeri bir taksi çağırma ürününün geliştirilmesi konu alınmış. Önce mimari bildiğimiz Monolithic yaklaşım üzerine kurgulanıyor. Her şey güllük gülistanlık giderken iş birimlerinden gelen yeni talepler, devreye alınan parçlar ve kalabalıklaşan kodlar sebebiyle sistem büyüdükçe büyümeye başlıyor. Performans düşüyor, yönetim ve ölçeklenebilirlik zorlaşıyor, bakım maliyetleri artıyor. Sonrasında aynı senaryonun micoservice mimari yaklaşımı ile nasıl ele alınabileceği masaya yatırılıyor. Her iki yaklaşımın artı ve eksilerini görebilmek açısından oldukça güzel ve doyurucu bir yazı olduğunu ifade etmek isterim.

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
