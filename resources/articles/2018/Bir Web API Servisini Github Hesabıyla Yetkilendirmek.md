---
title: "Bir Web API Servisini Github Hesabıyla Yetkilendirmek"
pubDate: 2018-04-03 22:49:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - .NET
  - LINQ
  - REST
  - JSON
  - Web API
  - HTTP
  - OAuth
  - Authentication
  - Authorization
  - Async/Await
  - Generics
  - Visual Studio
  - GitHub
---

# Bir Web API Servisini Github Hesabıyla Yetkilendirmek
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Teknik konularda yazı yazmaya çalışmanın en zor yanlarından birisi de anlatımı basitleştirmek. Şüphesiz ki uğraştığımız konular bazen anlamakta güçlük çektiğimiz karmaşıklıkta olabiliyor. Böyle konuların bırakın anlatılması öğrenilmesi de güçleşebiliyor. Şahsen kendi adıma bazı konuları anlamak için epey çaba sarf ettiğimi söylesem yeridir. Üstelik bu konuları zaman içerisinde gerçek hayat senaryolarında kullanmaz veya üzerinde saatler geçirmessek unutuyoruz da. Oysaki bir konuyu basitçe anlatabiliyorsak hem iyi anlamışız demektir hem de verimli bir çalışma safhası geçirmişizdir. Ne demiş Austion Freeman;

> "Simplicity is the soul of efficiency" - Austin Freeman

Bugün de benzer bir durumla karşı karşıyayım. Ne gecenin bu sessiz vaktinde o güzel kokusu ile odamı dolduran filtre kahvem ne de arka planda çalan piyano resitali aslında işimi kolaylaştırmıyor. Yine de West-World, odaklanmam için yeterli şartları sağlamış durumda. Bu yazımızda en azından benim için hep karmaşık olan OAuth tabanlı bir yetkilendirme sürecinin nasıl yapılabileceğini incelemeye çalışacağız. İlk önce ne yapacağımızı özetlemeye çalışalım.

Senaryomuzda basit bir Web API Servisi bulunuyor..Net Core ile geliştirilen servisin bir Controller'ı için yetkilendirme (Authorization) sürecini uygulatmak istiyoruz. Burada OAuth 2 standardını ele almak, kullanıcı yetkilendirme yöneticisi ve bilet (Token) tedarikçisi olarak Github'dan yararlanmak istiyoruz. Tabii bu senaryonun gerçekleşmesi için bizim Github'a bir proje kaydettirmemiz (Pek çok platform için söz konusu olan Application Registration işlemi diyelim) ve özellike Redirect URI bilgisini Consumer rolündeki uygulamamıza bildirmemiz gerekiyor (Az sonra yapacağız)

Yaşam Döngüsü

OAuth 2 temelli sistemin çalışma prensibi basit (Aslında karmaşık ama iyice didikleyince gayet anlaşılır, mantıklı ve basit) Ortada üç aktör yer var. Senaryomuzu göz önüne alırsak bu aktörlerimiz Web API (Consumer), Github (Idendity and Token Service Provider) ve Web API servisini tüketmek isteyen kullanıcı (User) şeklinde ifade edilebilirler. Bu üç aktörün yaşam döngüsü içerisindeki iletişimi ise sırasyıla şöyle özetlenebilir.

Kullanıcı yerel makinesindeki servise bir talepte bulunur (HTTP Get gibi) O anda elinde geçerli bir bilet olmadığını düşünelim.
Bunun üzerine Web API uygulaması Github'dan bir kullanıcı doğrulaması ister.
Github kullanıcıyı doğrulamak için Login sayfasına yönlendirme yapar.
Kullanıcı doğrulanırsa, Github'ın bir sorusu ile karşılaşılır. Github'daki "bla bla" uygulamasının bilgilerinize erişmesine izin veriyor musunuz? gibi.
Eğer kullanıcı bunu kabul ederse, Github tarafından Redirect URI ile belirtilen adrese yönlendirilir. Bu yönlendirmede geçici bir erişim kodu bulunur.
Web API servisi aldığı kod ile Github'ın bilet sağlayan adresine (Token Endpoint) gider.
Github bu talep üzerine daha kalıcı olan onaylanmış bir bilet hazırlayıp bunu Web API servisine verir.
Web API servisi bu bilgiyi saklar (genelde bir son kullanma tarihi olur ama Github OAuth biletlerinde durum farklı) ve sonraki taleplerde bu bilet kullanılır.

Karışık değil mi? Hofff...Siz birde bana sorun. Eğer basit bir şekilde anlatamadıysam bu konuyu anlamamışım demektir. Diğer yandan adım adım örneği işlettiğimizde konuyu biraz daha pekiştirebileceğimizi düşünüyorum. Haydi başlayalım.

OAuth Uygulaması için Kayıt İşlemi

İlk olarak [şu adrese](https://github.com/settings/developers) giderek OAuth uygulamamızı Github'a kayıt etmemiz gerekiyor. "Register a new application" başlıklı düğmeye basarak işleme başlayabiliriz. Burada uygulamaya ait bazı bilgileri doldurmamız lazım.

![image.axd](images/image.axd)

vb bilgiler olabilir. Authorization Callback URL bilgisi dikkatiniz çekmiş olmalı. Senaryoya göre Service Provider rolü üstlenen Github, Consumer rolündeki yerel Web API servisi üzerinden gelen kullanıcıyı yetkilendirirse bu URL adresine doğru bir yönlendirme gerçekleştirilecek ki, bu yönlendirme sırasında Consumer'a birde geçici erişim kodu verilecek. Sonrasında Consumer (yani Web API hizmetimiz) bu geçici kod ile Github'ın Token Endpoint'ine gelerek daha kalıcı olan erişim biletini (Access Token) alacak.

"Register Application" başlıklı düğmeye basıldıktan sonra uygulamanın oluşturulduğu ve Web API servisimizde kullanılmak üzere Credential bilgisinin üretildiği görülebilir.

![image.axd](images/image.axd)

Buradaki Client ID ve Client Secret değerleri Web API servisimizin Github uygulamasını kullanabilmesi için gereklidir.

Web API Servisinin Geliştirilmesi

Sırada Web API servisinin oluşturulması adımı var. Bunun için aşağıdaki terminal komutunu kullanabiliriz.

```bash
dotnet new webapi -o MyQuoteService
```

İlk olarak Kestrel sunucusunu 5005 numaralı porta ayarlayalım (Bildiğiniz üzere varsayılan port 5000) Bunu Program.cs içerisinde yapabiliriz.

```csharp
public static IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseUrls("http://localhost:5005")
        .Build();
```

Uygulamanın en önemli değişiklikleri Startup sınıfında gerçekleştirilecek. Bu dosyanın içeriğini aşağıdaki hale getirelim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace MyQuoteService
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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie()
            .AddOAuth("GitHub", options =>
            {
                options.ClientId = "sizinki";
                options.ClientSecret = "sizinki";
                options.CallbackPath = new PathString("/signin-github");

                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.ClaimActions.MapJsonKey("urn:github:blog", "blog");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async ctx =>
                    {
                        Console.WriteLine("OnCreatingTicket Event");

                        var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);

                        var response = await ctx.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ctx.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var userInfo = JObject.Parse(await response.Content.ReadAsStringAsync());
                        ctx.RunClaimActions(userInfo);
                        Console.WriteLine($"User Info:\n{userInfo.ToString()}");
                    }
                };
            });
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
```

Sonuç itibariyle Middleware tarafına bir doğrulama katmanının eklenmesi söz konusu. Bunun hazırlığı için de Authentication, Cookie ve OAuth gibi servislerin devreye alınması gerekiyor. AddAuthentication metodu ile doğrulama servisini devreye alıyoruz. Buradaki ayarlar seçilen doğrulama servisine göre farklılık gösterebilir. Örnekte Cookie'lerden yaralanacağımızı belirtiyoruz. Buna göre çalışma zamanı kullanıcı doğrulamasını kontrol etmek için Cookie Authentication Handler'dan yararlanacak. Kullanıcı doğrulandığında da ise kullanıcı bilgisi Cookie içerisinde saklanacak (DefaultSignInScheme atamasına göre) Cookie Authentication Handler'ın devreye alınması işini AddCookie fonksiyon çağrısı ile bildiriyoruz. OAuth Handler'ını kayıt ederken Github tarafında oluşturduğumuz uygulama için üretilen Client ID, Client Secret değerleri ile bizim belirlediğimiz Callback adresini atıyoruz. Bu değerleri konfigurasyon dosyasından ya da daha güvenli bir ortamdan (Cyberark gibi) alabilirsiniz. Sonuçta hassas bilgiler.

Yetkilendirme, bilet alma ve kullanıcı bilgilerini çekme gibi operasyonlar, Github tarafında belirli adreslerden sunulan servisler tarafından karşılanmakta. Bu nedenle options parametresi üzerinden ilgili Endpoint adresleri belirtiliyor. Bu adreslerden sunulan servisler birer REST servis. Yani Postman, SOAPUI gibi araçları kullanarak da deneyebiliriz. ClaimsAction üzerinden çağırılan MapJsonKey metodu iki parametre ile çalışıyor. İlk parametre ile kullanıcı için Github tarafından gelen içerikteki Claim tipi, ikinci parametre ile de key bilgisi belirleniyor. Buradaki atamalara Controller tarafındaki User nesnesi üzerinden erişebileceğiz.

Kodun ilerleyen kısmında bir olay metodu da yer alıyor. OnCreatingTicket kullanıcı doğrulamasını takip eden süreçte bilet üretildikten sonra devreye giren bir olay olarak düşünülebilir. Bu olay metodu içerisinde Github'ın UserInformationEndpoint ile bildirilen adresine HTTP Get talebinde bulunuyoruz. Dikkat ederseniz bir Authentication Header bilgisi de veriyoruz ki bu bize Github tarafından verilen bilet (Bearer Token) SendAsync ile ilgili talep gerçekleştirildikten sonra kullanıcı bilgilerini elde etmiş oluyoruz. Bunları sadece örnekte görmek amacıyla ekrana bastırdık. Artık servisler devrede. Bu durumu Middleware tarafında etkinleştirmek içinse Configure fonksiyonunda UseAuthentication çağrısını yapmamız gerekli. Gelelim yetkilendirme sürecine dahil edeceğimiz Controller tipine. Örneğimizde WebApi şablonu ile gelen ValuesController yerine aşağıdaki içeriğe sahip QuotesController sınıfını kullanacağız.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyQuoteService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class QuotesController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var id = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
            var email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            var blog = User.FindFirst(c => c.Type == "urn:github:blog")?.Value;
            Console.WriteLine($"{DateTime.Now}\nCurrent user:{userName}({id})\n{email}\n{blog}");
            return new ObjectResult(_quotes);
        }

        List<Quote> _quotes = new List<Quote>{
                new Quote{Id=122548,Owner="Michael Jordan",Text="I have missed more than 9000 shots in my career. I have lost almost 300 games. 26 times, I have been trusted to take the game winning shot and missed. I have failed over and over and over again in my life. And that is why I succeed."},
                new Quote{Id=325440,Owner="Vince Lombardi",Text="We didn't lose the game; we just ran out of time"},
                new Quote{Id=150094,Owner="Randy Pausch",Text="We cannot change the cards we are dealt, just how we play the game"},
                new Quote{Id=167008,Owner="Johan Cruyff",Text="Football is a game of mistakes. Whoever makes the fewest mistakes wins."},
                new Quote{Id=650922,Owner="Gary Lineker",Text="Football is a simple game. Twenty-two men chase a ball for 90 minutes and at the end, the Germans always win."},
                new Quote{Id=682356,Owner="Paul Pierce",Text="The game isn't over till the clock says zero."},
                new Quote{Id=156480,Owner="Jose Mourinho",Text="Football is a game about feelings and intelligence."},
                new Quote{Id=777592,Owner="LeBron James",Text="You know, when I have a bad game, it continues to humble me and know that, you know, you still have work to do and you still have a lot of people to impress."},
                new Quote{Id=283941,Owner="Roman Abramovich",Text="I'm getting excited before every single game. The trophy at the end is less important than the process itself."},
                new Quote{Id=185674,Owner="Shaquille O'Neal",Text="I'm tired of hearing about money, money, money, money, money. I just want to play the game, drink Pepsi, wear Reebok."}
            };
    }

    class Quote
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Text { get; set; }
    }
}
```

Sadece özlü sözler listesini HTTP Get talebi karşılığında geriye döndüren bir operasyonumuz var. Get operasyonu içerisinde Github kullanıcısının doğrulanması sonrası çekilen ClaimSet içerisindeki bazı değerlere ulaşılmakta. Bu örnekte login olan Github kullanıcısına ait username,id,email ve blog bilgilerini Console ekranına bastırmaktayız. Bu bilgiler loglama amacıyla kullanılabilir. QuotesController sınıfının bir diğer önemli özelliği de Authorize niteliği ile işaretlenmiş olması. Buna göre tüm operasyonları için yetkilendirme sürecine dahil olacağız.

Testler

Geliştirme safhasını sonlandırdığımıza göre test sürüşüne çıkabiliriz. Uygulamayı

```bash
dotnet run
```

terminal komutu ile çalıştırdıktan ve tarayıcı üzerinden http://localhost:5005/api/quotes adresine gittikten sonra aşağıdaki ekran görüntüsü ile karşılaşırız.

![image.axd](images/image.axd)

Dikkat edileceği üzere Github login sayfasına yönlendirildik. Eğer Network hareketliliklerini izlersek aşağıdaki geçişlerin olduğunu fark edebiliriz.

localhost:5005/api/quotes HTTP Get talebi HTTP 302 koduna çevrilerek Location header bilgisindeki github adresine yönlendirilir.
Yönlendirildiğimiz https://github.com/login/oauth/authorize? client_id={client id bilgisi}&scope=&response_type=code&redirect_uri= http://localhost:5005/signin-github&state= {uzuuunnn bir state bilgisi var} adresinde Authorize kontrolünden geçeriz ki önceden login olmamışsak yeni bir adrese yönleniriz.
HTTP Get ile https://github.com/login?client_id= {client id bilgisi}&return_to=/login/oauth/authorize?client_id= {client id bilgisi}&redirect_uri= http%3A%2F%2Flocalhost%3A5005%2Fsignin-github&response_type=code&scope=&state= {uzuuuuun state bilgisi} geldiğimiz bu adreste ise Login olmamız yeterli olacaktır.
Sonrasında Github kullanıcısının söz konusu uygulama için yetki vermesini bekleyen bir onayı penceresi ile karşılaşabiliriz.

![image.axd](images/image.axd)

Bu bir kereliğine sorulacaktır ancak Github üzerindeki uygulama ayarlarından Revoke All Users Tokens işlemini yaparsak tekrardan karşılaşabiliriz. Artık DailyQuoteService isimli uygulama için buraksenyurt kullanıcısı yetkilendirilmiş durumda. Dolayısıyla bir önceki taleple gelen Location header bilgisindeki URL adresine yönlendiriliriz ki bu da görmek istediğimiz özlü sözler operasyonudur.

![image.axd](images/image.axd)

Tabii Visual Studio Code arabirimine bakarsak Login olan kullanıcıya ait Github tarafından sunulan tüm ClaimSet değerlerinin JSON formatında geldiğini de görebiliriz. Ayrıca Get metodu içerisinden de oturum açan kullanıcının çeşitli bilgilerine erişebiliriz.

![image.axd](images/image.axd)

Servisimiz için Github tarafından sağlanan Token bilgisinin bir son kullanma tarihi bildiğim kadarı ile yok. Kullanıcının Token bilgisi sistemden düşmediği sürece servis yetkilendirme kontrolü yapma ihtiyacı duymadan çalışıyor olacak. Github'un ilgili servis adreslerine HTTP DELETE metoduyla ID bilgisiyle talepte bulunup düşürme işleminin bilinçli olarak uygulanabilineceği de ifade ediliyor. Bunu neden söylüyorum dersiniz? Uygulamayı denerken özlü sözler servisinin Authorization adımlarına takılmadan sürekli olarak çalıştığını gördüm. Bir yerlerde düşse de tekrar Login olmamı istese diye beklerken aslında kullanım amacının ne olduğunu hatırladım. Amaç bir uygulamanın Github üzerinden doğrulanmış kullanıcılar için OAuth protokolü üzerinden Bearer Token ile çalışmasıydı. Servisin çalıştığı sistem Github tarafından bir kere doğrulanıp ehliyet bilgisini aldıktan sonra hizmet verebilir konumda kalması yeterliydi. Bu arada pratik bir yol olarak tarayıcı çerezlerini temizlemeniz halinde tekrardan Login işlemine tabii tutulacağınızı söylemek isterim;)

Böylece geldik bir makalemizin daha sonuna. Bu yazımızda bir Web API servisinin yetkilendirme sürecinde Github'ın OAuth hizmetinden nasıl yararlanabileceğimizi incelemeye çalıştık. Umarım faydalı olur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örneğe [github adresi üzerinden](https://github.com/buraksenyurt/dotnetcore/tree/master/MyQuoteService) erişebilirsiniz.
