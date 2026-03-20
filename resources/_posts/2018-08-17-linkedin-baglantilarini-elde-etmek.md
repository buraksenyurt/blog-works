---
layout: post
title: "Linkedin Bağlantılarını Elde Etmek"
date: 2018-08-17 21:30:00 +0300
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - xml
  - dotnet
  - aspnet
  - asp-dotnet-core
  - rest
  - json
  - http
  - oauth
  - authentication
  - authorization
  - javascript
  - async-await
---
Sosyal ağlar ile pek aram yok. Vakit kaybı gibi gelmese de oraya harcayacağım zamanı farklı alanlarda kullanmayı daha çok tercih ediyorum. Özellikle yeni bir şeyler öğrenmek benim epey zamanımı alıyor. Kullandığım tek sosyal platform diyebileceğim yerse Linkedin. Bu profesyonel iş ağında zaman içerisinde hatırı sayılır derecede bağlantım oluştu. Ne var ki sayı artınca ana sayfadaki akışı izlemek inanılmaz derecede zorlaştı. Hatta iş ağını geliştirirken sayının onbinler üzerine çıkması bir süre sonra bazı merak edilen sorulara cevap bulmayı zorlaştırıyor. Örneğin bağlantılarımdan kaçı İnsan Kaynakları alanında çalışıyor? Ya da İstanbul'da yaşayanlar kimler? Veya ayda ortalama 4 veya daha fazla paylaşım yapanlar hangi bağlantılarım? Peki ya yaptıkları paylaşımlarda yapay zeka ile ilgili anahtar kelimeler kullananlar?

![linkedin_11.jpg](/assets/images/2018/linkedin_11.jpg)

Herhalde nereye varmak istediğimi az çok anladınız. Bir şekilde Linkedin bağlantılarımı sorgulayabilmek istiyorum. Ancak kendi geliştireceğim uygulama üzerinden. Hal böyle olunca tüm kapılar diğer sosyal ağlarda da olduğu üzere firmanın bizlere sunduğu geliştirici API'lerine açılıyor. Bu amaç doğrultusunda yine bir Cumartesi gecesi oturdum bilgisayarımın başına ve Linkedin REST Api diyerekten google'lamaya başladım. Eh malum illa ki kendi içeriğini kullandırttığı REST tabanlı servisleri vardır diye düşündüm. Evet vardı. Sonra dokümantasyonunu okumaya başladım. Ağırlıklı olarak Android, Apple gibi platformlar için sunduğu SDK'lar öne çıkıyordu. Ben ilk başta basit tarayıcı veya Postman gibi araçları kullanarak ilerlemek istedim. Çünkü REST servisilerinin çalışır hallerini görmek için minimum kod eforu harcamak istedim.

Temel olarak yapılması gereken adımlar belliydi. Basitçe örnek bir servisi denerken OAuth 2.0 standardındaki işleyişi de daha iyi anladığımı fark ettim. Bugünün sosyal ağları ya da API sağlaycıları kendi operasyonlarını kullandırtırken güvenliğin ne kadar önemli olduğunun bilincinde hareket etmekte. Çoğunlukla güncel ve güçlü bir doğrulama (Authentication) standardı olan OAuth 2.0 kullanılıyor. Konumuza tekrar dönecek olursak; Linkedin açısından düşündüğümüzde bir REST çağrısı yapabilmenin adımlarını şöyle özetlememiz mümkün;

Amaç: Uygulamaya giriş yapmış bir Linkedin kullanıcısının temel bilgilerini elde etmek (adı, soyadı, ünvanı,id'si gibi)

Öncelikle Linkedin üzerinde bir uygulama (Application) oluşturmamız gerekiyor. Bu uygulamayı tanımlandığında Linkedin bize bir Client Id ve Client Secret verecek. Ayrca bizim, OAuth 2.0 için bir Callback URL bilgisi de vermemiz lazım.
Uygulama hazır olduğunda işlemlere devam edebilmek için Linkedin'den bir Authorization Code almalıyız. Bu kod Linkedin'e giriş yapan ve söz konusu uygulamanın client id bilgisi ile gelen kişi için üretilecek. Talep sonrası akış, uygulamayı tanımlarken belirtilen Callback URL adresine doğru devam edecek ve bir Authorization Code dönecek (Querystring içerisinde code ismiyle)
Bu noktada Linkedin'in herhangibir API hizmetini kullanabilmek için gerekli Access Token bilgisini almaya çalışacağız. Az önce verilen Authorization Key bilgisi işte bu aşamada devreye giriyor. Az önce kendimizi Linkedin'e doğrulatıp bir anahtar almıştık. Şimdi bu anahtarı kullanarak Linkedin'den bir Bearer Token isteyeceğiz. Token'ı alırken sadece anahtar değil, Client ID ve Client Secret bilgilerini de Linkedin'in ilgili token servisine göndereceğiz.
Son adım için gerekli talep yapıldığında Linkedin bize belli bir periyot boyunca geçerli olacak Access Token bilgisini döndürecek. Biz de bu token bilgisini alıp Linkedin'in ilgili API servislerini kullanacağız.

![linkedin_13.gif](/assets/images/2018/linkedin_13.gif)

Kabaca adımlarımız bu şekilde. Şimdi gerçek zamanlı örneğimizi yapmaya çalışalım.

Callback Sayfasının Oluşturulması

Authorization Key bilgisinin döndürüleceği sırada talebin yeniden yönlendirileceği bir adres olduğundan bahsetmiştik. Burası üzerinde Linkedin için Javascript SDK'sı kullanılan basit bir HTML sayfası da olabilir, Apple SDK'sı kullanan bir mobil uygulamada. Ben boş bir Asp.Net Core uygulaması oluşturup belli bir porttan yayın yapmasını tercih ettim. Önce komut satırından boş bir proje şablonu açtım.

```bash
dotnet new web -o LinkedinCb
```

Startup.cs içerisindeki Configure metodunu da aşağıdaki hale getirdim.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.Run(async (context) =>
    {
        var authorization_code=string.Empty;
        if(context.Request.QueryString.HasValue)
        {
            authorization_code=context.Request.Query["code"].ToString();
        }
        context.Response.ContentType="text/html";
        await context.Response.WriteAsync($"Linkedin Callback Page<br/>{authorization_code}");
    });
}
```

Tek yaptığım QueryString ile gelebilecek bir code değeri varsa bunu ekrana bastırmak. Birde uygulamanın 8144 portundan çalışacağını belirtmek için launchSettings.json'da aşağıdaki değişikliği yaptım (http://localhost:8144 buradaki çalışma özelinde oluşturulmuş bir adres. Pek tabii Linkedin hizmetlerini kullanacak uygulama nerede host ediliyorsa o makine üzerindeki bir konum da olabilir)

```xml
"LinkedinCb": {
      "commandName": "Project",
      "launchBrowser": true,
      "applicationUrl": "http://localhost:8144",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
```

Linkedin Uygulamasının Oluşturulması

Linkedin'de [şu sayfaya giderek](https://www.linkedin.com/developer/apps) bir uygulama oluşturmamız gerekiyor. Ben aşağıdaki ekran görüntüsünde yer alan bilgileri kullandım ama bir şirket olsaydım ve örneğin pazarlama amaçlı bir çalışma yapsaydım pek tabii gerçek firma bilgileri ile hareket edebilirdim.

![linkedin_1.gif](/assets/images/2018/linkedin_1.gif)

Pek çok alan zorunlu. Web sitesi adresi, elektronik posta hesabı, telefon numarası vb... Gönder düğmesine bastıktan sonra ise aşağıdaki sayfaya ulaştım.

![linkedin_2.gif](/assets/images/2018/linkedin_2.gif)

Burada verilen Client Id ve Client Secret değerleri önemli. Hatta onları çok iyi korumanız gerekiyor. OAuth 2.0 tarafında Callback URL bilgisi, biraz önce yazdığımız.Net Core uygulamasının yayınlandığı ve adres yönlendirmenin yapılacağı adres. Uygulama için varsayılan olarak r_basicprofile yetkilendirmesi söz konusu ki bu login olan kullanıcının temel bilgilerini görmek için yeterli. Ancak farklı izin seviyeleri de söz konusu (Hatta okuduğum kadarıyla çok daha geniş yetkilere sahip olabiliyoruz ama bunun için Linkedin ile partner olarak anlaşmak gerekiyor olabilir. Emin değilim araştırmak lazım)

Authorization Kodunun Çekilmesi

Artık doğrulama kodunu alabilirdim. Bunun için tarayıcıdan aşağıdaki talebi göndermem yeterliydi.

```text
https://www.linkedin.com/oauth/v2/authorization?response_type=code
&client_id={burada_uygulamanın_client_id_bilgisi_var}&
redirect_uri=http%3A%2F%2Flocalhost%3A8144&
state=zRt1poWf45A53sdfKef90pp4567
```

Talebin içerisinde bir kaç parametre var. response_type ile aslında Linkedin'in authorization servisinden ne istediğimizi belirtiyoruz. client_id az önce oluşturduğumuz uygulamadan geliyor. Yine uygulama için belirttiğimiz Callback adresini redirect_uri parametresi ile bildiriyoruz. Son parametre olan state değeri ise Cross Site Request Forgery saldırılarına önlem olması için verilen tahmin edilmesi zor bir değer olmalıdır. Talebi göndermeden önce.Net uygulamasını çalıştırmayı da ihmal etmedim. Çok doğal olarak talep sonrası hemen Linkedin'in Login sayfasına yönlendirildim.

![linkedin_3.gif](/assets/images/2018/linkedin_3.gif)

Login işlemini gerçekleştirdikten sonra da aşağıdaki sayfaya yönlendirildim.

![linkedin_4.gif](/assets/images/2018/linkedin_4.gif)

Tahmin edileceği üzere uygulamaya kullanıcının bir izin vermesi gerekiyor. Bu izni verdikten sonra artık elimde bir Authorization kodu da vardı.

![linkedin_5.gif](/assets/images/2018/linkedin_5.gif)

Access Token Değerinin Alınması

Authorization Key değeri artık elimdeydi. Postman'i kullanarak aşağıdaki POST talebini oluşturdum.

```text
https://www.linkedin.com/oauth/v2/accessToken
POST
HTTP 1.1
Content-Type: application/x-www-form-urlencoded

Body Key-Value içerikleri;

grant_type=authorization_code
code=AQRf-GqTIUisUY6hP..............
redirect_uri=http%3A%2F%2Flocalhost%3A8144
client_id={linkedin_uygulamanızın_client_id_değeri}
client_secret={linkedin_uygulamanızın_client_secret_değeri}
```

Görüldüğü üzere Linkedin'in accesstoken adresine bir talep gidecek. Talep başarılı olmuş ve Linkedin servisi bana bir token bilgisi göndermişti.

![linkedin_6.gif](/assets/images/2018/linkedin_6.gif)

Linkedin Üye Bilgisinin Çekilmesi

Artık elimde REST servislerini yaşam süresi dolana kadar kullanabileceğim bir Bearer Token var. Postman'den aşağıdaki talebi kullanarak kendi bilgilerime ulaşmayı başardım.

```text
https://api.linkedin.com/v1/people/~?format=json
GET HTTP 1.1
Authorization: Bearer AQUmIOSKG0UoLxN-NnoNGqJYvgIQ6QU9.................
```

![linkedin_10.gif](/assets/images/2018/linkedin_10.gif)

Aslında buraya kadar ki kurgu, geliştirdiğimiz herhangi bir uygulamaya Linkedin ile Login olan kullanıcının temel bilgilerini okuyup ekrana basmak için de yeterli.

> Authorization Adımı Şart mı?
> Aslında Linkedin API servislerini kullanırken Authorization adımına ille de uğramak gerekmeyebilir. Client ID ve Client Secret değerlerini kullanarak accessToken hizmetine doğrudan ulaşmakta mümkün. Ancak bu akış için [Linkedin'in ile iletişime geçmek gerekiyor](https://developer.linkedin.com/docs/v2/oauth2-client-credentials-flow).

Tam Olarak İstediğimi Elde Edemedim

Aslında istediğim şeyi tam olarak elde edebilmiş değilim. Ben bağlantıda olduğum kişilerin bilgilerine de ulaşabilmek istiyordum. Bununla ilgili olarak Linkedin dokümantasyonundan yararlanarak aşağıdaki talebi denedim.

```text
https://api.linkedin.com/v2/connections?q=viewer&projection= (elements*(to~(id,localizedFirstName,localizedLastName)))
GET HTTP 1.1
Authorization: Bearer AQUmIOSKG0UoLxN-NnoNGqJYvgIQ6QU9nj7.............
```

Bağlantılarımın id, ad ve soyad bilgilerini görmek istemiştim.

![linkedin_8.gif](/assets/images/2018/linkedin_8.gif)

Volaaa..Linkedin beni acı bir şekilde geri çevirmişti. Neden kendi bağlantılarımı çekememiştim? Bir süre bilgisayarımın başında mutsuz mutsuz oturdum. Mutfağa gidip yaz sıcağına aldırmadan sıcak bir kahve yaptım. Soğumasını camın önünde beklerken saatin gece yarısını geçişisini izliyordum. Tekrar bilgisayarımın başına döndüğümde ilk gözüme çarpan şey servis versiyonunda v1 değil de v2 kullanmış olmamdı. Ama bu son derece olağandı çünkü Linkedin API tarafında versiyon değişikliğine gitmişti. Yine de v1 yaparak aynı talebi tekrar denedim. Bu sefer de bad request aldım.

```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<error>
    <status>400</status>
    <timestamp>1533423097840</timestamp>
    <request-id>VNSLB86CIJ</request-id>
    <error-code>0</error-code>
    <message>Unknown field {connections} in resource {Root}</message>
</error>
```

O zaman v1 de çalışan people talebini v2 ile yapayım dedim;) Tahmin ettiğim gibi v2 için yine yetkilendirme hatası (403 Forbidden) aldım.

![linkedin_12.gif](/assets/images/2018/linkedin_12.gif)

Sonrasında google amcaya giderek beni mutlu edecek bir şeyler söylemesini istedim ve [Stackoverflow'un şu adresteki girdisinde bir bilgi](https://stackoverflow.com/questions/46960458/any-queries-to-the-api-linkedin-com-v2-return-not-enough-permissions-to-access) buldum. Uzun bir Linkedin formunda onları amacımla ilgili ikna etmeye çalıştım. 30 iş günü içerisinde bana döneceklerini söylediler. Muhtemelen tekrardan token alacağım tabii ama önemli değil. Servis kurgusunun değişmeyeceği ortada. Ancak Linkedin'in bildirimine göre eğer beni anlarlarsa yeni bir client id ve client secret bilgisi paylaşacaklar. Bu sebeptendir ki makalem şu an itibariyle yarım kaldı. Baştaki "To Be Continued" un anlamı da bu işte:|

## To Be Continued (maybe)
