---
title: "Web API'leri Swagger ile Zenginleştirelim"
pubDate: 2018-09-20 21:22:00
categories:
  - Asp.Net Core
tags:
  - Asp.Net Core
  - Bash
  - XML
  - C#
  - .NET
  - LINQ
  - JSON
  - Web API
  - Web Service
  - HTTP
  - Generics
  - Visual Studio
---

# Web API'leri Swagger ile Zenginleştirelim
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Dokümantasyon sektörümüzün olmazsa olmazları arasında yer alan bir konu. Ancak güncelliğini korumak ve okunabilirliğini sağlamak da bir o kadar zor olabiliyor. Ayrıca dokümantasyon hazırlamak da çoğumuza bir işkence gibi geliyor. Yine de belirli alanlarda tüketicilerin iyiliği için bu dokümantasyonları hazırlamak boynumuzun borcu diye düşünüyorum. Devasa kütüphanalerden oluşan uygulamalarda gerçekten dokümantasyon başlı başına bir işken daha küçük alanlarda etkili kullanabileceğimiz yerleri de var.

Ağırlıklı olarak web servislerini göz önüne alalım (Genel standartlara uyan her tür servis olabilir) Bir servis ne iş yapar, tanımı nedir, içerisinde hangi operasyonları barındırır, bu operasyonların kullanım şekli nasıl olmalıdır, giriş çıkış parametreleri ne türdedir ve akla gelebilecek bir kaç soruyu düşünelim. Bu sorular ilgili web servisini kullanacak taraf için kendi geliştirmeleri sırasında önem arz eder ve mutlaka hayatımızın bir noktasında entegre etmemiz gereken bir servis olmuştur.

Eğer bu bir kurum servisi ise genelde elimize tutuşturulan uzun bir Word dokümanı da olur. Servisin genel yapısı, işleyiş şekli ve diğer bilgilerine buradan bakarız. İşin içinden çıkılır çıkılmasına ama bugüne gelindiğinde artık servislerin kendilerini anlattıkları arabirimleri de üzerinde taşıdıklarını görüyoruz. Hazır ortalık mikro seviyede sayısız Web API hizmetinden geçilmiyorken hızlı ama etkili dokümanlar oluşturmak bu açıdan önemli.

> XML tabanlı dokümantasyon konusu aslında.Net'in ilk yıllarından beri hayatımızda yer alıyor. Temiz kod (Clean Code) çerçevesinde yapılan geliştirmeler bu tip yorum satırlarına gerek bırakmıyor gibi düşünülebilir, lakin ilgili XML girdileri otomatik dokümantasyon hazırlayan uygulamalar içindir (Basit bir Help dosyası olabileceği gibi, Swagger arabirimi de olabilir) İşin içerisinde yalın ve hafif Web API'ler söz konusu ise XML yorumlarından kaçınmamakta yarar var.

Evrensel anlamda bir standartlaşma da var. [OpenAPI](https://swagger.io/specification/) bildirimlerine uyan Swagger arayüzleri Web API hizmetleri için genel kabul görmüş durumda. Bu yazımızdaki amacımız ise bir Web API servisine Swagger modelinde bir yardım dokümanı eklemek. İşimiz çok kolay ve sonuçları oldukça tatmin edici. İlk olarak bir uygulama oluşturalım ve Swagger kullanımını kolaylaştıracak Swashbuckle paketini ekleyelim.

```bash
dotnet new webapi -o QuoteWallAPI
dotnet add package Swashbuckle.AspNetCore
```

XML Documentation File'ın etkinleştirilmesi için proje dosyasına da küçük bir ek yapmamız gerekiyor. PropertyGroup altına GenerateDocumentationFile elementini ekleyip true değerini vermeliyiz. Normalde Visual Studio arayüzünde kolayca yapılabilir ama Visual Studio Code kullanıyorsanız dosya üstünde eklemeniz gerekebilir.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

  <ItemGroup>
    <Folder Include="wwwroot\" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="3.0.0" />
  </ItemGroup>

</Project>
```

Ben standard olarak gelen ValuesController sınıfını QuotesController olarak isimlendirip içeriğini aşağıdaki gibi değiştrdim. Bol miktarda XML Comment ve bir kaç tane de nitelik (Attribute) kullanımı var.

```csharp
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QuoteWallAPI.Models;

namespace QuoteWallAPI.Controllers
{
    ///<summary>
    ///CRUD Controller for Daily Quote
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController 
        : ControllerBase
    {
        ///<summary>
        ///List all public quotes sorted by most recently liked.
        ///</summary>
        ///<remarks>with paging, this returns up to 100 quotes.</remarks>
        ///<return>Quotes list</return>
        ///<response code="200"></response>    
        [Produces("application/json")]  
        [HttpGet]
        public ActionResult<IEnumerable<Quote>> Get()
        {
            return new List<Quote>();
        }

        ///<summary>
        ///Return a specific Quote from ID
        ///</summary>
        ///<param name="id">ID of Quote</param>
        ///<return>Quotes list</return>
        ///<response code="200">If found</response>    
        ///<response code="404">If not found</response>
        [Produces("application/json")]
        [HttpGet("{id}")]
        public ActionResult<Quote> Get(int id)
        {
            return new Quote();
        }

        ///<summary>
        ///Add a new Quote to QDB
        ///</summary>
        ///<param name="value">JSON content of Quote</param>
        ///<remarks>
        ///Sample body content
        ///{"id":1,"Text":"Some words..","Author":"you"}
        ///</remarks>
        ///<response code="201">Added</response>    
        [Consumes("application/json")]
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        ///<summary>
        ///Update any Quote belongs to a specific Author and ID
        ///</summary>        
        ///<param name="id">Identity value of Quote</param>
        ///<param name="author">Author of Quote</param>
        ///<param name="value">JSON content of updates</param>
        ///<remarks>
        ///Sample body content
        ///{"Text":"Any update"}
        ///</remarks>
        ///<response code="201">Updated</response>    
        ///<response code="404">If not found from ID</response> 
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public void Put(int id,string author, [FromBody] string value)
        {
        }

        ///<summary>
        ///Delete any Quote from QDB
        ///</summary>
        ///<param name="id">ID of Quote</param>
        ///<response code="204">Deleted</response>    
        ///<response code="404">If not found from ID</response> 
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
```

Özet bildirimleri için summary, örnek kod parçası yerleştirmek veya daha fazla bilgi vermek için remarks, dönüş tipini bildirmek için return, HTTP durum kodu için response, fonksiyon parametreleri için param elementlerinden yararlanmaktayız. Ayrıca fonksiyonun Request veya Response içerik tipleri için Consumes ve Produces niteliklerini de ele alıyoruz.

QuotesController'ın sunduğu 5 standart fonksiyonellik bir şey yapmıyorlar ancak siz kendi örneğinizi geliştirirken özellikle "Try it out" butonuna basarak yapacağınız test sonuçlarını görmek için içeriklerini doldurabilirsiniz. Örnekte kullanılan Quote isimli bir Model sınıfımız da var. Onu da dokümantasyon için XML Comment ve bir takım niteliklerle zenginleştirmeliyiz.

```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuoteWallAPI.Models
{
    ///<summary>
    ///Some quote from you
    ///</summary>
    public class Quote
    {
        ///<summary>Id of the Quote</summary>
        [Required]
        public int Id { get; set; }
        ///<summary>Text of the Quote</summary>
        [Required]
        public string Text { get; set; }
        ///<summary>Author of this Quote</summary>
        [DefaultValue("Anonymous")]
        public string Author { get; set; }
    }
}
```

Id ve Text elementleri Required olarak işaretlendiler. Bu yüzden yardım dokümantasyonunda * sembolü taşıyacaklar. Author değişkeni için varsayılan bir değer belirttik ki bu da Swagger tarafından ele alınacak. Bu nitelikler dışında çok kısa özet bilgilere yer veriyoruz.

Buraya kadar yaptıklarımızla hem Controller hem Model sınıfları hakkında bir kaç bilgi sağlamış olduk. Ancak Swagger arayüzünün bu içeriği kullanması ve otomatik olarak oluşturulması için ilgili Middleware parçasının eklenmesi lazım. Bunun için Startup sınıfında bazı eklemeler yapacağız.

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace QuoteWallAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(g=>{
                g.SwaggerDoc("v2",new Info
                {
                    Title="DailyQuote CRUD API",
                    Version="2.0",
                    Description="Get your friends daily quotes, add something beaty words and more...",
                    Contact=new Contact { Name = "burak", Email = "selim@buraksenyurt.com", Url = "http://www.buraksenyurt.com"}                    
                });

                g.IncludeXmlComments(Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(s=>{
                s.SwaggerEndpoint("/swagger/v2/swagger.json","Daily Quote");
            });
        }
    }
}
```

Öncellikle AddSwaggerGen metodu ile ilgili hizmeti çalışma zamanına eklemekteyiz. Burada kullanılan SwaggerDoc içerisinde ana sayfa için gerekli başlık bilgilerini dolduruyoruz. Servisin adı, versiyon numarası, kısaca ne yaptığının açıklanması ve kontak kurulabilecek kişi bilgilerine burada yer vermekteyiz. Önemli olan kısımlardan birisi de XML dokümantasyon dosyasının bildirimi. Aksi belirtilmedikçe bu dosya üretilen dll dosyası ile aynı yerde olacaktır.

Servisi bu şekilde bildirdikten sonra kullanılması için de UseSwagger ve UseSwaggerUI fonksiyonlarından yararlanmaktayız. Bir endpoint veriyoruz ki kullanıcılar veya servis kaşifi olan botlar kolaylıkla yardım dokümanına erişebilsinler.

Bu çalışmalar sonrasında XML comment'lerinin bir dosya olarak Bin klasörü altında oluştuğunu da görebiliriz. İstersek buradan da içeriğe müdahale etmemiz mümkün.

![image.axd](images/image.axd)

Uygulamayı

```bash
dotnet run
```

komutuyla çalıştırdıktan sonra artık Swagger arayüzüne ulaşabiliriz. Tek yapmamız gereken herhangibir tarayıcıyı kullanarak http://localhost:5554/swagger/ adresine gitmek (5000 nolu Port Apache hegamonyasında olduğundan UseUrls ile değiştirdim) Karşılama sayfası aşağıdaki gibi açılır. Gayet şık ve göz alıcı gördüğünüz üzere:)

![image.axd](images/image.axd)

Üst kısımdaki özet bilgileri SwaggerDoc metodu içerisinde belirlemiştik. Sunmuş olduğumuz API operasyonlarına göre bir kaç bölüm açıldığını görebiliriz. Get, Post, Put ve Delete için. Ayrıca Controller tarafında kullanılan Quote sınıfı da Models bölümünde yer alır. Dolayısıyla XML Comment'ler ve Swagger nitelikleri arayüz tarafına da yansımaktadır. Diğer kısımların nasıl göründüğüne de kısaca bakalım dilerseniz.

Sayfalama yapıldığı takdirde 100 quote getiren Get talebine ait parça aşağıdaki gibidir.

![image.axd](images/image.axd)

Response Content Type'ın JSON olduğunu, HTTP 200 kodu döndüreceğini ve dönüş çıktısının da Example Value kısmındaki gibi olacağını görebiliyoruz. "Try It out" butonuna basarak hemen test de edebiliriz.

Yeni bir quote eklemek istediğimizde kullanacağımız POST işlemine ait aşağıdaki kısım oluşur.

![image.axd](images/image.axd)

Örnek body içeriği, olası HTTP Dönüş kodları, parametrenin JSON tipinden olacağı, fonksiyonun kısaca ne yaptığı gibi bilgileri görebiliyoruz. Diğer metodlar için de benzer yardım sayfaları ve anında test edebilmemizi sağlayacak "Try It Out" düğmeleri olacakatır.

Belli bir ID için Quote döndüren Get operasyonu (id alanının Reuired olduğuna dikkat edelim)

![image.axd](images/image.axd)

Update işlemi için kullanılan Put metodu (Bu fonskiyon için de id alanı zorunludur)

![image.axd](images/image.axd)

ve son olarak silme işlemleri için kullanılan Delete operasyonu (Kırmızı renk dikkat edilmesi gereken bir işlem olduğuna işaret ediyor olmalı)

![image.axd](images/image.axd)

Görüldüğü üzere APIyi kullanacak olan geliştirici ve hatta API hizmetini tarayan robot için gerekli tüm bilgiler burada yer alıyor. Operasyon adları, açıklamaları, dönüş durum kodları, ne tür içeriklerle çalıştıkları, örnek mesaj gövdeleri ve tabii test edilmelerini sağlayan "Try It Out" düğmeleri. Bu standartlaştırılmış yardım sayfalarını basit bir kaç hareket ile uygulamak da oldukça kolay. Bize düşen bu standart yardım dokümanlarını hazırlamak için gerekli XML Comment ve nitelikleri doğru bir şekilde uygulamak. Bundan sonra yazacağımız her Web API için biraz vakit ayırıp bu dokümanları hazırlamakta yarar var. Gelecek sadece insanların değil, belli standartları takip eden robotların kullanacağı servislerle dolu olacak. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
