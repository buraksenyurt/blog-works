---
layout: post
title: "Asp.Net Core Web API için Custom MiddleWare Yazmak"
date: 2017-12-18 06:01:00 +0300
categories:
  - dotnet-core
  - asp-dotnet-core
tags:
  - .net-core
  - asp.net-core
  - asp.net-core-webapi
  - webapi
  - middleware
  - custom-middleware
  - kestrel
  - pipeline
---
Uzun zamandır televizyon dizisi izlemiyorum. Aslında bir dönem düzenli olarak takip ettiğim diziler vardı. Bir tanesi de usta oyuncular Benedict Cumberbatch (Sherlock Holmes) ve Martin Freeman (Dr. John Watson) ın oynadığı Sherlock Holmes idi. Bu Cumartesi gecesi bir şekilde dizinin bir bölümüne rastladım. Keyifli bir bölüm tekrarı yaptım. Oyunculuklara yine hayran kaldım. Sherlock'un keskin zekasına, Watson'un her zamanki sorgulayıcı düşünce tarzının eklendiği bir bölümdü.

![custom_mw4.gif](/assets/images/2017/custom_mw4.gif)

Ardından kahvemi alıp West-World'e doğru yola çıktım. Bu kez Sergei Rachmaninof, Henry Auguste ve Dan Hawkins'in aralarında yer aldığı piyano tınıları eşliğindeydim. Saatler gece yarısını geçeli bir kaç dakika olmuştu. Çantamı açtım ve hafta içi.Net Core tarafındaki ara katmanın (Middleware) ne işe yaradığını anlamaya çalışırken karaladığım notlarımı buldum. Bilgilerimi toparlamanın ve güzel bir örnek yapıp tekrar unutmamak üzere bloğuma bir şeyler yazmanın tam vaktiydi.

![custom_mw5.gif](/assets/images/2017/custom_mw5.gif)

.Net Core açık kaynak olarak geliştirilmiş Kestrel web sunucusunu kullanmakta. Kestrel, web çalışma zamanının ayağa kaldırılmasından, istemciden gelen taleplerin (Request) hat üzerindeki ara katman modüllerine (pipeline middleware) geçirilmesinden ve tabii ki üretilen cevabın tekrar istemciye döndürülmesinden sorumlu (En temel fonksiyonellikleri bunlar) IIS gibi geniş kabiliyet ve yönetsel özelliklere sahip görünmese de bence çarpraz platformlarda microservice mimarisi için ideal bir çatı sunucusu. Benim ilgimi çekense Pipeline Middleware tarafı. Burada, seçilen web projesine göre varsayılan olarak gelen ara katman modülleri zaten mevcut. Peki kendi middleware tipimizi de sisteme dahil edebilir miyiz? (Saçma bir soru. Tabii ki de edebilirsin Burak; ama nasıl?) O zaman gelin bu işi nasıl yapabileceğimize bir bakalım.

Başlangıçta aşağıdaki terminal komutunu kullanarak standart bir Web API projesi oluşturarak işe başlayabiliriz. Örneğimizdeki amacımız orta katmana yeni bir arabirim ekleyebilmek. Bu nedenle ValuesController yapısını olduğu gibi bırakabiliriz.

```bash
dotnet new webapi -o CustomMW
```

Asp.Net Core tarafında oluşturulan projeler bildiğiniz gibi standart bir takım kodlarla gelmekte. Startup.cs içeriği bu açından önemlidir. Nitekim web sunucusu ayağa kalkarken gerçekleştirilecek bir çok ön hazırlık ve çalışma zamanı hareketliliklerine ait işleyişler burada ele alınır. Configure metodunda yer alan app değişkeni, IApplicationBuilder arayüzünün oldukça şık bir uyarlamasını kullanır ve bir çok yeni ara katman özelliğinin çalışma zamanına dahil edilebilmesine olanak sağlar. Neler olduklarını görmek için Use ön eki ile başlayan metodlara bakmamız yeterli.

![custom_mw2.gif](/assets/images/2017/custom_mw2.gif)

Dilersek ara katmanda devreye girebilir ve pipeline üzerinden hareket eden mesajları yakalayabiliriz (Aslında pek çok kaynakta ilk yapılan örnek bu) Bunun en basit yolu belli bir tipe bağlı olmadan kullanılabilen Use fonksiyonunu yazmaktan geçmekte. Aşağıdaki örnek kod parçasını ele alaım.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.Use(async (HttpContext ctx,Func<Task> next)=>{
        var request=ctx.Request;
        Console.WriteLine("REQUEST {0}",DateTime.Now.ToLongTimeString());
        Console.WriteLine("{0}-{1}{2}",request.Method,request.Host,request.Path);

        await next.Invoke();

        var response=ctx.Response;
        Console.WriteLine("RESPONSE:{0}",DateTime.Now.ToLongTimeString());
        Console.WriteLine("{0}\t({1}){2}",response.ContentType,response.StatusCode, (HttpStatusCode)response.StatusCode);
    });

    app.UseMvc();

}
```

Use metodu HttpContext ve Func tipinden parametreler alan metodları işaret edebilecek bir temsilci (delegate) ile çalışır. HttpContext ile web çalışma zamanına gelen ve giden içerikleri kontrol edebiliriz. Örnekte gelen mesaj ile ilgili HTTP metodu, adres bilgisi gibi değerler çekilip Console penceresine yazdırılmaktadır. Sonrasında işleyişin sıradaki ara katman hattına devredilmesi sağlanır (Invoke çağırımı) Ardından istemciye gönderilecek cevaba ait bir takım bilgiler yazdırılır. Örneğin dönen içeriğin tipi ve durum kodu bilgisi gibi. Çalışma zamanında yapılan denemelerin sonuçlarından örnek bir ekran görüntüsü aşağıdaki gibidir.

![custom_mw1.gif](/assets/images/2017/custom_mw1.gif)

Dikkat edileceği üzere yapılan her talebe ilişkin bir takım bilgiler terminal penceresine yazılmıştır. IApplicationBuilder arayüzü üzerinden Use operasyonunu kullanılarak ilerlenilmesi basit ve pratik bir yol sunsa da, UseBlaBlaBla kullanımı kadar doğru değildir. Ara katmana dahil etmek istediğimiz operasyonları bir sınıf ile ilişkilendirmek sorumluluğun doğru yöne alınması ve tekrarlı kodların önlenmesi açısından iyi bir tercih olacaktır (Hatta siz ayrı bir Class Library içerisine alarak ilerleseniz daha iyi olur) Öyleyse yazının en heyecan verici kısmına başlayalım.

Senaryomuz oldukça anlamsız ama olsun. En nihayetinde varmak istediğimiz nokta app.UseWatson gibi bir ara katman modülünü boru hattına entegre edebilmek. Ben örnek olarak HTTP Post ile gelen mesajların içerik boyutlarını kontrol etmeyi ve belli bir sınırın üstünde olmaları halinde terminal penceresine uyarı çıkartmayı planlıyorum. Örneğin yeni bir value eklenmek istendiğinde istemcinin JSON formatında bir içerik göndermesi gerekiyor. Bu içeriğin boyutunun kontrolünü şüpheci Watson'a verebiliriz. Pek tabii bir gerçek hayat senaryosunda talebin doğrudan NotAllowed gibi bir durum koduna çekilerek reddedilmesi de sağlanabilir.

Senaryoda kritik olan noktalardan birisi mevzubahis içerik limitinin bir yerlerden alınması gerekliliği. Kuvvetle muhtemel bunu app.UseWatson gibi bir fonksiyonla birlikte kullanmalıyız. Önce bu seçeneği içerecek basit bir sınıfı projeye ekleyelim. İşte WatsonOptions.

```csharp
using System.Net;

public class WatsonOptions
{
    public long MaxSizeForPostContent{get;set;}
}
```

Bu sınıf üzerinden içerik limit kontrolünü yapabilmek için maksimum boyut değerini taşıyacağız. Peki nereye? Ara katman modülümüz olan Watson sınıfına.

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class Watson
{
    private readonly RequestDelegate _nextMiddleWare;
    private readonly WatsonOptions _options;

    public Watson(RequestDelegate next, IOptions<WatsonOptions> options)
    {
        _nextMiddleWare = next;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var request=context.Request;
        if(request.Method=="POST")
        {
            var contentLength=request.ContentLength;
            Console.WriteLine("[{0}]:{1}-{2}",DateTime.Now.ToLongTimeString(),request.Method,request.Path);
            
            if(contentLength>_options.MaxSizeForPostContent)
            {
                Console.ForegroundColor=ConsoleColor.Red;
                Console.WriteLine("POST size limit violation : {0} bytes\nLimit->{1}",contentLength,_options.MaxSizeForPostContent);
                Console.ForegroundColor=ConsoleColor.White;
                //TODO Something
            }
            else
            {
                Console.ForegroundColor=ConsoleColor.Green;
                Console.WriteLine("Length is OK ({0})",contentLength);
                Console.ForegroundColor=ConsoleColor.White;
            }
        }

        await _nextMiddleWare(context);                  
    }  
}
```

Watson, metodları rastgele tanımlanmış bir sınıf değil. Yapıcı (Constructor) metodu bir yana Task tipinden nesne döndüren ve HttpContext örneğini parametre alan Invoke isimli bir fonksiyon içermekte. Sanki ilk yazdığımız app.Use () metodunun kullandığı temsilciye (RequestDelegate) oldukça benziyor değil mi? Hatta tıpkısının aynısı diyebiliriz. Bu metod içerisinde HttpContext nesnesini kullanarak o anki talebin HTTP Post olup olmadığını kontrol ediyor ve eğer öyleyse gelen içeriğin boyutuna bakıyoruz. Şimdilik sadece ekrana bilgi yazdıran bir operasyon söz konusu. Metodun sonunda yapılan çağrı ile ilk başta gelen içeriğin çalışma zamanındaki bir sonraki ara katman modülüne devredilmesi sağlanıyor.

Sonuçta pipeline üzerinde bir metod zinciri söz konusu diyebiliriz. Ayrıca ilk başta kullandığımız app.Use () fonksiyonelliğinin sorumluluğunu başka bir tipe aldığımızı ifade edebiliriz.

Yalnız ortada küçük bir sorun var. Configure metodunda kullanılan IApplicationBuilder değişkeninin ardından UseWatson gibi bir metodu nasıl çıkartacağız? Neyse ki fii tarihinde.Net'e Extension Methods diye bir kavram eklenmiş:) Yapacağımız şey bu. Söz konusu genişletme işini WatsonExtensions isimli sınıfa şu şekilde yıkabiliriz.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

public static class WatsonExtensions
{
    public static IApplicationBuilder UseWatson(this IApplicationBuilder app,WatsonOptions options)
    {
        return app.UseMiddleware<Watson>(Options.Create(options));
    }
}
```

İlk parametre ile gelen app değişkeni üzerinden UseMiddlerware metodunun generic versiyonunu çağırıyoruz. Generic tipimiz Watson sınıfı. Parametre olarak da maksimum boyut özelliğini içeren options değişkenini aktarıyoruz. En dikkat çekici noktalardan birisi de UseWatson metodunun genişlettiği IApplicationBuilder tipinden bir referans döndürmesi. Resmen Fleunt bir Interface akımı söz konusu gibi;) Yapılan bu değişikliklere göre Startup.cs içerisindeki Configure metodunda aşağıdaki gibi bir kullanım artık mümkün.

```csharp
app.UseWatson(new WatsonOptions{
	MaxSizeForPostContent=1024
});
```

Volaaa!!! Açıkçası bu yaklaşım çok hoşuma gitti diyebilirim. Bir interface veya abstract sınıf türetimi ile plug-in mantığında bir genişletme yerine Fluent yapıyı benimseyen bir interface tipini genişleterek pipeline üzerine yeni bir middleware ekleyebildik. Bunu sıfırdan kurgulamaya çalışıp kendimizi daha da geliştirebiliriz ama şimdilik ödülümüz olan çalışma zamanı sonuçlarına bir bakalım derim. Firefox HttpRequester ile normal ve izin verilen limit üstündeki boyutlarda POST işlemleri yaptığımızda aşağıdaki ekran görüntüsündekine benzer sonuçlar alırız.

![custom_mw3.gif](/assets/images/2017/custom_mw3.gif)

Tabii burada gelen geçen tüm mesajları boyutu ne olursa olsun işlettik. Belki de boyut kontrolünün başka bir yolu da vardır ama amacımız özel bir ara katman modülünü nasıl entegre edebileceğimizi görmekti. Hayal gücünüzü zorlayın ve nasıl ara katman ekleyebilirsiniz bir düşünün. Bu yapıyı sadece Web API tarafında değil Kestrel kullanılan tüm.Net Core projelerinde ele alabiliriz. Dilerseniz benzer örneği farklı bir senaryo ile bir MVC projesinde deneyerek bilgilerinizi pekiştirebilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
