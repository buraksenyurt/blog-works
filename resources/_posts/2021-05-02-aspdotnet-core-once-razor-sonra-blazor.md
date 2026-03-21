---
layout: post
title: "Asp.Net Core - Önce Razor Sonra Blazor"
date: 2021-05-02 21:30:00 +0300
categories:
  - asp-dotnet-core
tags:
  - asp.net
  - asp.net-core
  - razor-page
  - csharp
  - cshtml
  - tag-helper
---
Kendime geldiğimde hiçbir şey göremediğimi fark ettim. Üstüme çöken zifiri karanlığa rağmen halen daha hayatta olduğuma dair tek şey yağmur damlalarının birkaç metre üstümde olduğunu sandığım metal tavana vurarak çıkardıkları seslerdi. Ensemden neredeyse ayak parmaklarıma kadar yayılan ağrı hiçbir şeyi umursamaz bir tavırda yattığım yerden doğrulmamı güçleştiriyordu. Son hatırladığım CloudTown'dan birkaç sibernetik coder ile karşılaştığım belli belirsiz yansımalardan ibaretti. Kısa süre sonra yakınlarımda koşuşturan bazı ayak sesleri işittim. Yer yer duraksıyor yer yer su birikintilerine girip çıkıyorlardı. Fısıltılar daha duyulur sesler haline gelmeye başladı. Tavandaki kapağı açmak üzere içlerinden birinin elindeki anahtarları hazırladığını işittim. Sonrası gözlerim için çok korkunç bir deneyimdi. Bu zifiri karanlıkta ne kadar kaldığımı bilmiyorum ama gözlerim dışardan gelen o parlak ışığa karşı adeta haykırıyordu. Üstüme boca edilen bir kova soğuk suyun ardından gelen kaba ses ise çok tanıdıkdı. Ve şöyle seslendim; "Reyzor! Sen haaa":P

![jack-finnigan-00yDgACVeMA-unsplash.jpg](/assets/images/2021/jack-finnigan-00yDgACVeMA-unsplash.jpg)

Efendim bendeniz yine yazıya giriş yapacak güzel bir şeyler bulamayınca böyle garip bir hikayeyi ortaya atıverdim. Gel gelelim konumuz baş karakterimiz Reyzor ile de alakalı.(Photo by [Jack Finnigan](https://unsplash.com/@jackofallstreets?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText) on [Unsplash](https://unsplash.com/s/photos/rain?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText))

Asp.Net Core 5 cephesine baktığımızda üç temel uygulama modelini desteklediğini görürüz. Web uygulamaları, servisler ve gerçek zamanlı iletişim araçları. Gerçek zamanlı (Real-Time) iletişim tarafında SignalR karşımıza çıkar. Servisleri göz önüne aldığımızda ise Web API ve gRPC başrolde yer alır. Web tarafını düşündüğümüzde oldukça zengin bir ürün çeşitliliği söz konusudur. MVC (Model View Controller), Razor Pages, Blazor, Angular ve React tabanlı SPA'lar (Single Page Applications). Kuvvetle muhtemel Asp.Net Core 5 tarafına yeni başladıysanız ve haberleri yeterince takip ediyorsanız en çok ilgi çeken geliştirme çatısının Blazor olduğunda hem fikir sayılırız. Fakat Blazor'a doğrudan geçmeden önce bazı temellerin de öğrenilmesi gerekir.

Söz gelimi dahili Dependency Injection mekanizmasının nasıl çalıştığını anlamak bunlardan sadece birisidir. Bana göre bir diğer önemli konu da Razor View Engine ya da sık telafuz edilen adıyla Razor'dur. Nitekim MVC, Razor Pages ve Blazor geliştirme çatıları alt yapı olarak Razor View Engine üzerine oturmaktadır. Dolayısıyla aynı bileşen yazım şablonlarını farklı web geliştirme çatıları için kullanabiliriz. Bu yetenek çatılar arası geçiş yapmamızı da kolaylaştırır. Öğrenilmesi oldukça kolay olan Razor, sadece C# ve HTML bilgisi gerektirir. Son yıllarda desteklediği yardımcı takılar (Tag helper) sayesinde arayüz tasarımcıları için dostane bir sözdizimi (syntax) sunar. Ayrıca test edilebilirliği basitleştirir. Razor en temel tanımıyla dinamik içerikle HTML çıktısı üretmeyi amaçlayan C# temelli bir işaretleme şablonudur (Markup Template) İşte bu yazıdaki amacımız örnek kodlar yardımıyla ona merhaba demek.

Dilerseniz vakit kaybetmeden örnek kodlara geçelim. Microsoft.Net 5 yüklü herhangi bir platformda aşağıdaki terminal komutları ile işe başlayabiliriz. Sonrasında Visual Studio Code veya Visual Studio Communit Edition veya kendimizi rahat hissettiğimiz bir IDE ile ilerleyebiliriz.

```bash
dotnet new sln -o HelloRazor
cd .\HelloRazor\
dotnet new mvc -o FirstContact
dotnet sln add .\FirstContact\
```

HelloRazor isimli bir Solution var. Ona FirstContact isimli bir de MVC (Model View Controller) projesi ekledik. Bu, en hafif MVC şablonu olarak düşünülebilir. Varsayılan haliyle baktığımızda dahi Razor tabanlı sayfalar içerdiğini görebiliriz (Index.cshtml, Privacy.cshmtl gibi) Cshtml uzantılı bu dosyalar kendi içlerinde hem HTML hem de sunucu tarafında çalışan C# kodlarını barındırır. Bu noktadan sonra karşımıza sıklıkla @ sembolünün çıkacağını ifade edebilirim. @ Sembolü ile C# ifadelerini veya kod bloklarını HTML içerisinde kullanabiliriz. Hemen index.cshtml dosyasına geçelim ve içeriğini aşağıdaki gibi güncelleyelim.

```text
@{
    ViewData["Title"] = "Space Traveler's Base";
}

<div>
    <h1 class="display-4">@ViewData["Title"]</h1>
    <p>
        Today is @DateTime.Now.DayOfWeek ! Well, where do you want to go?
    </p>
    <ul>
        @{
            var locations = new List<string> { "Sirius", "Altair", "Betelgeuse", "Algol", "Messier 31", "Eta Carinae" };
            var count = locations.Count;

            foreach (string location in locations)
            {
                <li>@location</li>
            }
        }
    </ul>
</div>
```

Index.cshtml içerisinde hem HTML hem de C# kodları olduğunu görebiliriz. @{ } ifadeleri razor kod blokları olarak adlandırılır. İçlerinde özgürce C# kodlaması yapabiliriz. Kodun iki yerinde bu kullanım söz konusu. İkinci kullanım biraz daha ilgi çekici. string türden bir List nesnesini kullanarak tarayıcıya çeşitli yıldızların isimlerini basıyoruz. @location kullanımı mutlaka dikkatinizi çekmiştir. Zaten bir Razor kod bloğundaysak neden döngüdeki location değişkeni başında @ işareti var? Çünkü kod bloğu içerisinde bir HTML elementi kullandık. li elementi kullanılan kısımda bir anda Razor kod bloğu dışına çıkmış ve HTML dünyasına girmiş oluruz. Doğrudan HTML içerisinde C# ifadelerini çalıştırmamız gereken bu gibi hallerde, ifadenin başına @ işareti koyarak hareket edebiliriz (Razor Implicit Expression olarak adlandırılan teknik). location için geçerli olan bu kullanımın bir benzeri de hangi günde olduğumuzu yazdıran kısımda yer almaktadır.

Uygulamayı çalıştırdığımızda aşağıdakine benzer bir sonuç almamız gerekir.

![hellomvc_12.png](/assets/images/2021/hellomvc_12.png)

Pek tabii locations listesinin bir model nesnesi baz alınarak farklı bir bileşenden View tarafına çekilmesi asıl amaç olmalıdır. MVC ve Blazor uygulamalarında sıklıkla bavşuracağımız bir yoldur. Bu durumu daha iyi anlamak için Model klasörü altına StarModel isimli bir sınıf oluşturarak devam edelim. Bu literatürde ViewModel olarak adlandırılan tiptir.

```csharp
using System.ComponentModel.DataAnnotations;

namespace FirstContact.Models
{
    public class StarModel
    {
        [Required]
        [Display(Name="Star No")]
        public int ID { get; set; }
        [Required]
        [MinLength(3,ErrorMessage = "The name of the star must be at least 3 characters.")]
        public string Name { get; set; }
        [Required]
        [Range(1,750)]
        [Display(Name="Distance from Earth(LY)")]
        public double Distance { get; set; }
        [Required]
        public double SurfaceTemperature { get; set; }
    }
}
```

Özelliklerin üzerinde kullanılan nitelikler (Attribute) şu an için gerekli değil ama yazının ilerleyen kısmındaki Tag Helper kullanımında işimize yarayacaklar. Bunu bir kenara bırakırsak basit bir ViewModel tanımladığımızı söyleyebiliriz. Bu modele ait yükleme, silme, güncelleme gibi işlemleri ise farklı bir sınıfın sorumluluğuna vermemiz doğru olacaktır. Örneğin Data isimli yeni bir klasör altına koyacağımız Star isimli bir sınıf bu iş için ideal görünüyor (Konumuzla doğrudan alakalı olmasa da alışkanlık edinmemiz için bir interface tipi ile birlikte bu bileşenleri tanımlamakta ve Dependency Injection ile birlikte kullanmakta yarar var)

IStar arayüzü;

```csharp
using FirstContact.Models;
using System.Collections.Generic;

namespace FirstContact.Data
{
    public interface IStar
    {
        List<StarModel> GetStars();
    }
}
```

ve Star sınıfı;

```csharp
using FirstContact.Models;
using System.Collections.Generic;

namespace FirstContact.Data
{
    public class Star
        :IStar
    {
        public List<StarModel> GetStars()
        {
            return new List<StarModel>
            {
                new StarModel{ID=1,Name="Sirius",Distance=8.60,SurfaceTemperature=9940},
                new StarModel{ID=2,Name="Altair",Distance=16.73,SurfaceTemperature=7700},
                new StarModel{ID=3,Name="Betelgeuse",Distance=642.5,SurfaceTemperature=126000},
                new StarModel{ID=4,Name="Algol",Distance=92.95,SurfaceTemperature=13000},
                new StarModel{ID=5,Name="Eta Carinae",Distance=7.5,SurfaceTemperature=35200},
            };
        }
    }
}
```

Sadece Index sayfasında kullandığımız yıldız listesinin bir benzerini döndüren kobay bir sınıf. Peki Razor View Engine tarafı bu sınıfı nasıl kullanacak? Şu anki MVC senaryomuza göre HomeController tarafındaki hazır Action metodu bunun için uygun görünüyor.

```csharp
using FirstContact.Data;
using Microsoft.AspNetCore.Mvc;

namespace FirstContact.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStar _star;

        public HomeController(IStar star)
        {
            _star = star;
        }

        public IActionResult Index()
        {
            var starList = _star.GetStars();
            return View(starList);
        }
    }
}
```

Index metodunda bir Star nesne örneği kullanarak yıldız listesini almaktayız. Sonrasında bu listeyi View metoduna parametre olarak geçerek Home isimli View'a göndermekteyiz. Sınıfın yapıcı metodunda görüleceği üzere Star bileşenini Dependency Injection Container'dan istiyoruz. Dolayısıyla Star bileşeninin DI servislerine eklenmesi gerekiyor. Daha önceki bir yazımızda bu konuya değinmiştik;) [Nasıl yaparım diyorsanız buradaki yazıya bakmanızı önerebilirim](/2021/04/25/aspdotnet-core-a-nasil-merhaba-deriz/) ama "işimi uzatma" derseniz de yapmanız gereken tek şey Startup sınıfındaki ConfigureServices metoduna aşağıdaki satırı eklemekten ibaret.

```csharp
services.AddTransient<IStar, Star>();
```

Bu ara hazırlıklardan sonra yeniden Razor tarafına dönelim ve Index sayfasının içeriğini aşağıdaki gibi değiştirelim.

```csharp
@{
    ViewData["Title"] = "Space Traveler's Base";
}
@model List<StarModel>

<div>
    <h1 class="display-4">@ViewData["Title"]</h1>
    <p>
        Today is @DateTime.Now.DayOfWeek ! Well, where do you want to go?
    </p>
    <ul>
        @{
            foreach (var star in Model)
            {
                <li>@star.Name (@star.Distance) Light Year from Earth</li>
            }
        }
    </ul>
</div>
```

Bir önceki Razor örneğinden farklı olarak burada dikkat etmemiz gereken en önemli nokta @model ve Model enstrümanları. @model direktifi ile sayfanın kullanacağı ViewModel nesnesini belirtiyoruz. Star sınıfındaki GetStars metodunun dönüş tipini düşününce bunun List olması son derece doğal. Başta yapılan bu işaretleme, sayfanın devamındaki Model değişkenlerinin List tipinden bir referans olacağını belirtmekte. Bu nedenle for döngüsü içerisinde @star ifadesinden sonra StarModel nesnesinin özelliklerine erişebiliyoruz. Model nesnesini kimin doldurduğu sorusunun cevabını ise HomeController sınıfının Index metodundaki View çağrısı vermekte.

> Yaptığımız örnek MVC tarafına daha uygun. @model direktifi bu örnekte bir veri nesnesini referans ediyorken Razor Pages yapısında code-behind dosyasında yer alan (Index.cshtml.cs gibi) bir sınıf işaret edilir. Bir başka deyişle @ ile kullanılan built-in direktiflerin seçilen web geliştirme çatısına göre farklılıklar göstermesi olasıdır.

Yaptığımız bu son değişikliklere göre çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![hellomvc_13.png](/assets/images/2021/hellomvc_13.png)

Fark ettiyseniz Razor söz dizimi oldukça kolay. Görselliği basit dokunuşlarla artırmak da mümkün. Yardımcı takılara (Tag Helper) değinmeden önce dilerseniz sayfamızı aşağıdaki şekilde yeniden düzenleyelim.

```text
@{
    ViewData["Title"] = "Space Traveler's Base";
}
@model List<StarModel>

<div>
    <h1 class="display-4">@ViewData["Title"]</h1>
    <p>
        Today is @DateTime.Now.DayOfWeek ! Well, where do you want to go?
    </p>
    <table class="table table-dark">
        <thead class="bg-primary">
            <tr>
                <th>
                    No
                </th>
                <th>
                    Name
                </th>
                <th>
                    Distance (LY)
                </th>
                <th>
                    Surface Temperature (Kelvin)
                </th>
            </tr>
        </thead>
        <tbody>
            @foreach (var star in Model)
            {
                <tr>
                    <td>
                        <label>@star.ID</label>
                    </td>
                    <td>
                        <label class="font-weight-bold">@star.Name</label>
                    </td>
                    <td>
                        <label class="font-italic">@star.Distance</label>
                    </td>
                    <td>
                        <label>@star.SurfaceTemperature</label>
                    </td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

Bu sefer HTML Table elementini işin içerisine kattık. Sonuç biraz daha umut verici. En azından benim için;)

![hellomvc_14.png](/assets/images/2021/hellomvc_14.png)

Şu ana kadar yaptıklarımızla Razor söz dizimini en temel haliyle tanıdık ve bir Razor sayfasını çalışacağı örnek bir Model ile nasıl bağlayacağımızı öğrendik. Bilmemiz gereken giriş seviye konulardan bir diğeri de Tag Helper kullanımıdır. Tag Helper ifadeleri giriş kontrollerinde (input), veri doğrulamasında (validation), yönlendirmelerde (routing) ve form ektileşimlerinde (actions) kullanılabilen basitleştirici ifadeler olarak düşünülebilir.

> Normalde HTML Helper'lar söz konusudur ve Visual Studio editörünün Scaffolding mekanizması otomatik View üretimlerinde ağırlıklı olarak @Html.DisplayNameFor, @Html.DisplayFor ve @Html.ActionLink gibi fonksiyon çağrımlarını kullandırır. Ancak son yıllarda yardımcı takı ifadeleri hem okunabilir olmaları hem de bir HTML elementine doğrudan ataşlanabilmeleri sebebiyle öne çıkmaktadır. HTML Helper'lar bazen okunması zor fonksiyon ifadelerinden oluşur.

Şimdi Home klasörü altına Add isimli bir View ekleyelim. Galaksi veritabanımıza yıldız eklemek için kullanılan basit bir form olduğunu düşünebiliriz. Bu form içerisinde de bazı yardımcı takılardan faydalanacağız.

```text
@{
    ViewData["Title"] = "Space Traveler's Base";
}
@section scripts{
    <partial name="_ValidationScriptsPartial" />
}
@model StarModel

<div>
    <form asp-controller="Home" asp-action="OnSave" method="post">
        <div class="form-group">
            <label asp-for="@Model.ID"></label><br />
            <input asp-for="@Model.ID" /><br />
            <span asp-validation-for="@Model.ID" class="text-danger"></span>
        </div>
        <div class="form-group">
            <label asp-for="@Model.Name"></label><br />
            <input asp-for="@Model.Name" /><br />
            <span asp-validation-for="@Model.Name" class="text-danger"></span>
        </div>
        <div class="form-group">
            <label asp-for="@Model.Distance"></label><br />
            <input asp-for="@Model.Distance" /><br />
            <span asp-validation-for="@Model.Distance" class="text-danger"></span>
        </div>
        <div class="form-group">
            <label asp-for="@Model.SurfaceTemperature"></label><br />
            <input asp-for="@Model.SurfaceTemperature" /><br />
            <span asp-validation-for="@Model.SurfaceTemperature" class="text-danger"></span>
        </div>
        <div class="form-group">
            <button class="btn-primary" type="submit">Save</button>
        </div>
    </form>
</div>
```

asp- şeklinde başlayan ifadeler tag helper bildirimleridir. Örneğin form elementinde asp-controller ve asp-action isimli iki yardımcı kullanılmıştır. Bildiğiniz üzere bir web formunu sunucu tarafına gönderirken form elementinden yararlanılır. Submit tipinden bir butona basıldığında hangi Controller'ın hangi Action fonksiyonunun devreye gireceğini bu yardımcı takılar ile belirleyebiliriz. Buna göre HomeController'ın OnSave isimli metodu çağıralacaktır.

Kullanılan bir diğer yardımcı takı ise asp-for'dur. Kullanıldığı HTML elemanına göre farklı davranışlar sergileyebilir. Bir label ile ilişkilendirildiğinde ViewModel nesnesinin varsa Display değerini, yoksa özellik adını kullanır. input elementi ile kullanıldığında ise ekrandan girilen içeriğin modelin hangi özelliğine bağlanacağını belirtir. Sayfada bazı doğrulama kontrolleri de söz konusudur. Girdi ihlallerine ait bilgiler span elementleri içerisinde yazılırken asp-validation-for isimli yardımcı takı kullanılmıştır. Buna göre takriben aşağıdaki ekran görüntüsündekine benzer bir sonuç elde ederiz.

![hellomvc_15.png](/assets/images/2021/hellomvc_15.png)

Konumuz giriş verilerinin kontrolü değil bu nedenle çok fazla detaya girmiyoruz. Lakin örnekte kullandığımız sayfanın HTML çıktısına bakarsak asp-validation-for bilgilerinin istemci bazlı doğrulama işlemlerinde kullanılan jQuery için data-val-* formatına evrildiğini de görebiliriz. Örneğin Name input kontrol için üç karakterden az olmaması gerektiğini ifade etmiştik ya da dünyaya olan uzaklığın sıfır ışık yılı olmayacağını. Aşağıdaki ekran görüntüsünden de fark edileceği üzere tek bir asp-for-validation bildirimi HTML çıktısında data-val, data-val-minlength, data-val-minlength-min, data-val-required şeklinde çözümlenmiştir. Bunların bir kısmının da ViewModel nesnesinde kullandığımız DataAnnotations niteliklerinden (Attribute kullanımlarına bakın) kaynaklandığını ifade edebiliriz. Dolayısıyla Razor tarafındaki bir tag helper'ın işi nasıl kolaylaştırdığını net bir şekilde görmüş oluyoruz (Blazor tarafında doğrulama için asp-validaton-for yerine ValiadationMessage elementi kullanılmaktadır. Söz dizimi şuna benzer;)

![hellomvc_16.png](/assets/images/2021/hellomvc_16.png)

Razor tarafında kullanılan pek çok yardımcı takı var. Bir tanesini daha örneğe eklemeye ne dersiniz? Söz gelimi yıldızların dahil olduğu birkaç galaksiyi bir Enum sabiti olarak tuttuğunuzu ve yeni yıldız eklerken de bunları bir select elementinde göstermek istediğinizi düşünün. Bunun için asp-items isimli yardımcı takıyı kullanabilirsiniz. Haydi bir deneyin;) Buna ek olarak geliştirdiğimiz yıldız ekleme sayfasını HTML Helper fonksiyonları ile tekrardan yazmaya çalışın ve Tag Helper'lar ile aradaki farklılıkları kıyaslayın.

Başta da belirttiğimiz gibi önce Razor sonra Blazor. Hatta önce Razor sonra MVC, sonra Razor Pages ve daha sonra Blazor:) Nitekim bu çatılar kendileri için ortak olsa da Razor View Engine'i farklı şekilde kullanmaktalar. Örneğin MVC tarafında Razor sayfalarına yapılan yönlendirmeler Controller tarafından ele alınırken, Razor Pages yapısında pages klasörü altındaki sayfalara fiziki bir yönlendirme söz konusudur. Söz gelimi pages/space/star.cshtml şeklinde bir sayfamız varsa, bu sayfaya çalışma zamanında /space/start şeklinde ulaşabiliriz. Hatta Razor Pages tarafında sayfalar genellikle.cshtml ve.cshtml.cs şeklinde ikiye ayrılır ki bu kurguyu benim gibi Asp.Net Web Forms çağından gelenler iyi bilir. Ama en nihayetinde hepsinin altında yatan temel olgu Razor View Engine kullanımıdır.

Bu kısa çalışmada MVC, Blazor ve Razor Pages gibi çatıların temel yapıtaşı olan Razor View Engine tarafını basitçe anlamaya çalıştık. Artık bunun üstüne daha zengin Web uygulama örnekleri geliştirebileceğinizi düşünüyorum. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize sağlıklı günler dilerim.
