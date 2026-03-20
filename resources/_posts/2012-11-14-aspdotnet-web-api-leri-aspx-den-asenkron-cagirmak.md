---
layout: post
title: "Asp.Net Web API’ leri ASPX’ den Asenkron Çağırmak"
date: 2012-11-14 00:00:00 +0300
categories:
  - dotnet-framework-4-5
tags:
  - dotnet-framework-4-5
  - csharp
  - xml
  - dotnet
  - aspnet
  - aspnet-mvc
  - entity-framework
  - linq
  - rest
  - json
  - web-api
  - http
  - async-await
  - task-parallel-library
  - threading
  - concurrency
  - generics
---
Bateri çalanları her zaman için büyük bir hayranlıkla izlerim/izlemişimdir. Özellikle 4 uzuvlarını da (iki kol iki ayak) kullanırlar ama daha da önemlisi tüm bu unsurları eş zamanlı olarak çalıştırabilirler. Sadece iki kolu çalıştırmak nispeten bir dereceye kadar kolay olabilir biz normal insan oğulları için ama bir de ayakları devreye sokmak. Hele de tüm bu hareketli parçalardan anlamlı bir melodi çıkartmak gerçekten çok ama çok zor bir iştir.

[![baterist-290x290](/assets/images/2012/baterist-290x290_thumb.jpg)](/assets/images/2012/baterist-290x290.jpg)

Tabi eğitimler ile belirli seviyede bateri çalabilmek pek çok insan için mümkün olabilir ama tabi ritim tutturmak, ataklarda bulunmak veya çok uzun süre boyunca aynı tempoyu hiç bozulmadan devam ettirebilmek inanılmaz bir konsantrasyon veya yetenek gerektirmektedir. Peki bu eş zamanlı çalışabilme bir yazılımcı için ne ifade edebilir? Tabi ki de asenkron çalışan kod parçalarını

![Open-mouthed smile](/assets/images/2012/wlEmoticon-openmouthedsmile_31.png)

Bilindiği gibi Asenkron (Asynchronous) çalışma, günümüz yazılım geliştirme araçlarının olmazsa olmaz parçalarından birisidir. Nitekim kullanıcı deneyimi (User Experience) yüksek olan uygulamalarda, istemcilerin uzun sürebilecek işlemleri beklemeden başkalarına devam edebilmesi tercih edilen ve işlem sürelerini kısaltan bir kabiliyettir.

Ne varki bu çalışma mantığı özellikle Asp.Net gibi sunucu tabanlı çalışan web uygulama modelleri düşünüldüğünde biraz daha farklı ele alınmalıdır/anlaşılmalıdır. Bunun en büyük sebeplerinden birisi de Web'in sunucu taraflı çalışması sırasında devreye girmekte olan sayfa yaşam döngüsüdür (Page Life Cycle)

Asp.Net tabanlı uygulamalar çok doğal olarak istemciden gelen taleplere cevap üretirken sunucu üzerinde yürüyen bir süreci devreye sokar. Bu süreç içerisinde talepte bulunulan sayfaların da belirli yaşam döngüleri ve dolayısıyla olay metodları devreye girer. Dolayısıyla şu senaryo her zaman için benim gibi yazılım geliştiricilerin kafasını karıştırır;

> Bir web sayfası metodu içinden asenkron olarak bir servis çağrısı gerçekleştirirsem sunucu tarafındaki yaşam döngüsünde nasıl bir hareketlilik olur?

[Dikkat edileceği üzere burada konu, istemcinin AJAX tabanlı bir servis çağrısı yapmasından farklıdır. İrdelenmek istenen sunucu tarafındaki bir servis çağrısına ait asenkron çalışma mantığıdır]

Aslında senaryomuza göre istemci herhangibir şekilde web sayfasına talep de bulunduğunda, sayfanın yaşam döngüsü içerisinde kalan bazı servis çağrılarının asenkron olarak yürütülebilmesi ele alınmaktadır. İstemciler standart HTTP Get çağrısı dışında kendi tarayıcılarından bir düğmeye bastıklarında da sunucu tarafında bazı olayları tetiklerler. Ancak sıkıntı şudur. Sayfanın yaşam döngüsü gereği kontrollere ait Click/Change olay metodlarının da devreye girdiği bir an vardır. En azından o ana gelene kadar bile, bazı servislere olan çağrıların asenkron olarak başlatılıp sonuçlarının alınması düşünülebilir.

Sanırım kendi kafamı karıştırdığım kadar sizin kafanızı da epeyce karıştırmış olabilir!

![Smile](/assets/images/2012/wlEmoticon-smile_46.png)

> Başlangıç noktası olarak Asp.Net Server Based Web uygulamalarının yaşam döngülerine ve web dışında normal bir Console uygulaması üzerinde yapılabilen asenkron çalışma mantığına bakmanızı öneririm.

Peki biz bu yazımızda neyi değerlendiriyor olacağız? Bize ne kaldı

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_114.png)

Malum.Net Framework 4.5 ile birlikte, asenkron programlamayı biraz daha kolaylaştıran ve Task Parallel Library'nin tamamlayıcısı olarak da görebileceğimiz async ve await isimli iki yetenekli keyword ile tanıştık (Yetenekli diyorum nitekim Intermediate Language tarafında önemli eklemeler yapıyorlar) Dolayısıyla Asp.Net Web Forms uygulamalarında asenkron çalışma mantığını yeni baştan ele almamızı gerektirecek bazı kabiliyetler söz konusu.

İşte bu amaça bir Asp.Net Web Forms uygulamasından bir Asp.Net Web API servis operasyonunu asenkron olarak nasıl çağırabileceğimizi adım adım incelemeye çalışıyor olacağız. Bu sayede bir Web uygulamasında asenkron erişim tekniklerini nasıl ele alabileceğimizi de.Net Framework 4.5 stilinde görmüş olacağız

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_114.png)

İşe bir adet Empty Asp.Net Web Application, bir adet Asp.Net MVC 4.0 Application (ama Web API şablonunu kullanan) ve bir adet te Class Library oluşturarak başlayalım. Web API uygulamamız içerisinde Entity Framework tabanlı bir Web API servisini de kullanıyor olacağız. Temel olarak solution içeriğimizin aşağıdaki şekilde tesis edileceğini ifade edebiliriz.

[![asynwa_3](/assets/images/2012/asynwa_3_thumb.png)](/assets/images/2012/asynwa_3.png)

> Her iki uygulamada da aynı Entity tipleri kullanılacağından basit olarak Class Library’ nin her iki projeye de referans edilmesi yolunu tercih ettik.

MVC uygulamamızda örnek veritabanı olarak Chinook'u işaret eden bir Entity Model'imiz bulunmakta. Buna uygun olacak şekilde örneğin Employee Entity tipi ile çalışacak bir de Controller eklediğimizi düşünelim. Controller tipimizi Empty Controller şablonunda ekleyebiliriz. Nitekim veri çekme işlemine ait kod içeriğini kendimiz geliştiriyor olacağız. EmployeeController ismiyle üretilen sınıfımızın içeriğini ise aşağıdaki gibi geliştirebiliriz.

```csharp
using ChinookEntityLibrary; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading; 
using System.Web.Http;

namespace ChinookWebApi.Controllers 
{ 
    public class EmployeeController 
        : ApiController 
    { 
        private ChinookEntities db = new ChinookEntities();

       public IEnumerable<Employee> GetEmployees() 
        { 
            ChinookEntities db = new ChinookEntities(); 
            var allEmployees = from e in db.Employees 
                               orderby e.LastName 
                               select e; 
            Thread.Sleep(5000); // Bilinçli olarak duraksatma yapıyoruz 
            return allEmployees; 
        } 
    } 
}
```

> Tabi WebApi örneğinin çalışabilmesi için WebApiConfig sınıfı içerisinde yer alan Register metodunun da aşağıdaki şekilde güncellenmesi gerekmektedir.
> using System.Web.Http;
> namespace ChinookWebApi
> {
> public static class WebApiConfig
> {
> public static void Register (HttpConfiguration config)
> {
> config.Routes.MapHttpRoute (name: "EmployeeApi",
> routeTemplate: "api/{controller}/{id}",
> defaults: new { id = RouteParameter.Optional });
> }
> }
> }

EmployeeController tipi içerisinde yer alan Get metodu klasik olarak Chinook Entity Model'i sorgulamakta ve tüm Employee listesini LastName alanına göre sıralayarak geriye döndürmektedir. Metodun örneğimiz için önemli olan kısmı ise çalışmakta olan Thread'in 5 saniye boyunca duraksatıldığı kısımdır. Bu, verinin en az 5 saniye geç gelmesine neden olacak ve asenkron için gerekli senaryoya zemin hazırlayacaktır.

Bu arada örneği test etmemizde ve servis çağrısı sonucu geçerli bir veri içeriğini elde edebildiğimizi görmemiz de yarar var. Eğer sıkıntı yoksa aşağıdakine benzer bir ekran görüntüsü almamız gerekecektir.

[![asynwa_1](/assets/images/2012/asynwa_1_thumb.png)](/assets/images/2012/asynwa_1.png)

Şimdi elimizde HTTP tabanlı çalışan bir REST servisi bulunmakta. Bu servisi konuşlandırdığımız adresi kullanarak, diğer Web uygulamasındaki Web Form üzerinden bir Request gönderiyor olacağız. Ancak bu Request'in asenkron şekilde gerçekleştirilmesi de önemli. Bunu sağlamak için öncelikli olarak istemci Web Form'u içerisine aşağıdaki metodları yazdığımızı göz önüne alalım.

```csharp
using ChinookEntityLibrary; 
using Newtonsoft.Json; 
using System; 
using System.Collections.Generic; 
using System.Configuration; 
using System.Diagnostics; 
using System.Net.Http; 
using System.Threading.Tasks; 
using System.Web.UI;

namespace ClientApp 
{ 
    public partial class AsyncTestPage : System.Web.UI.Page 
    { 
        protected void Page_Load(object sender, EventArgs e) 
        { 
            RegisterAsyncTask(new PageAsyncTask(GetEmployeeDataFromServiceAsync)); 
        }

        public async Task<List<Employee>> InvokeEmployeeService() 
        { 
            using (HttpClient client = new HttpClient()) 
            { 
                HttpResponseMessage serviceCallResponse = await client.GetAsync(ConfigurationManager.AppSettings["EmployeeServiceAddress"]); 
                string jsonContent = (await serviceCallResponse.Content.ReadAsStringAsync()); 
                List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(jsonContent); 
                return employees; 
            } 
        }

        private async Task GetEmployeeDataFromServiceAsync() 
        { 
            Stopwatch stopWatch = new Stopwatch(); 
            stopWatch.Start();

            var taskInvokeEmployee = InvokeEmployeeService();

            await taskInvokeEmployee;

            List<Employee> data1 = taskInvokeEmployee.Result;

            stopWatch.Stop(); 
            lblResult.Text = string.Format("<h2>{0} adet Employee {1} saniye de çekilmiştir.</h2>" 
                ,data1.Count, stopWatch.Elapsed.TotalSeconds); 
        } 
    } 
}
```

> Client uyulamasında HttpClient tipinin kullanımı için System.Net assembly'ının referans edilmesi gerekir. Ayrıca JSON içeriğinin daha kolay bir şekilde ele alınabilmesi için Newtonsoft'un Json.Net kütüphanesinden yararlanılmaktadır. Bu kütüphane NuGet Package Manager ile uygulamaya kolayca yüklenebilir.

InvokeEmployeeService metodu dikkat edileceği üzere async anahtar kelimesi (keyword) ile işaretlenmiştir. Bunun en önemli nedenlerinden birisi de, içerisinde awatiable iki fonksiyon içermesidir. Bu fonksiyonlardan birisi servis operasyonunu asenkron olarak çağıran GetAsync'dir. Diğeri ise servis operasyonuna ait response'daki içeriği Json formatında çeken ReadAsStringAsync'dir. Her iki asenkron metodun çağrısı sırasında await keyword'ünün kullanıldığına dikkat edilmelidir.

InvokeEmployeeService metodu aslında Employee tipinden olan generic List koleksiyonunu sarmallayan bir Task nesne örneğini döndürmektedir. await ile işaretlenmiş olan metodlar aslında bir sonraki satıra geçilmesi noktasında beklenilmesi gerektiğini belirtmektedir. Dolayısıyla GetAsync metodu ile Response alınmadan sonraki ifadeye geçilmemesi gerektiği await ile bildirilmektedir.

Gelelim GetEmployeeDataFromServiceAsync metoduna. Bu metod içerisinde aslında Stopwatch nesne örneği ile yapılan süre ölçümü haricinde, InvokeEmployeeService metoduna yapılan bir çağrı da söz konusudur. Bu çağrı yapılırken dikkat edileceği üzere, taskInvokeEmployee isimli Task tipinden nesne örneği için await keyword'ünün kullanıldığı görülmektedir. Dolayısıyla taskInvokeEmployee nesne örneğinin sarmalladığı (Wrap) generic Employee listesi dönmeden, sonraki ifadeye geçilmemesi gerektiği ifade edilmektedir.

async keyword'ü ile işaretlediğimiz GetEmployeeDataFromServiceAsync metodunun Page Framework'e register edilmesi için PageLoad olay metodu içerisinde Page özelliği ile erişilebilen RegisterAsyncTask metodunun kullanıldığını görüyoruz. Bu metoda yapılan çağrı ile, parametre olarak gelen async şeklinde işaretlenmiş asenkron çağırılabilen metodun sayfa ile ilişkilendirilmesi sağlanmış olunmaktadır.

Ancak işlemlerimiz bunlarla sınırlı değil

![Confused smile](/assets/images/2012/wlEmoticon-confusedsmile_23.png)

Son olarak sayfanın Page direktifi içerisindeki Async niteliğine true değerinin atanması gerekmektedir. Böylece web sayfanın asenkron olarak çalıştırılacağı belirtilmektedir.

```xml
<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="AsyncTestPage.aspx.cs" Inherits="ClientApp.AsyncTestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
        <asp:Label ID="lblResult" runat="server" /> 
    
    </div> 
    </form> 
</body> 
</html>
```

Artık Web sayfamızı çalıştırabiliriz. İlk çağrı sırasında servisin ayağa kalkması veya sayfanın ilk kez yürütülmesinden kaynaklanan bir gecikme sorunu yaşanabilir ve beklemediğimiz kadar uzun bir süre ile karşılaşabiliriz. Ancak sonraki çağrılarda aşağıdakine benzer bir çıktı alırız. Yaklaşık olarak 5 saniye civarında bir çalışma zamanı söz konusudur ki bu son derece normaldir.

[![asynwa_2](/assets/images/2012/asynwa_2_thumb.png)](/assets/images/2012/asynwa_2.png)

Yine de bu sayfanın tam anlamıyla asenkron çalıştığına dair bir kanıt değildir (Unutmayın ki sayfanın sunucu tarafındaki yaşam döngüsü içerisinde bir asenkron çalışma senaryosu göz önüne alınmaktadır) Bu kanıt için kodu biraz daha ilginçleştirelim ve sayfa içeriğini aşağıdaki hale getirelim.

```csharp
using ChinookEntityLibrary; 
using Newtonsoft.Json; 
using System; 
using System.Collections.Generic; 
using System.Configuration; 
using System.Diagnostics; 
using System.Net.Http; 
using System.Threading; 
using System.Threading.Tasks; 
using System.Web.UI;

namespace ClientApp 
{ 
    public partial class AsyncTestPage : System.Web.UI.Page 
    {        
        protected void Page_Load(object sender, EventArgs e) 
        { 
            RegisterAsyncTask(new PageAsyncTask(GetEmployeeDataFromServiceAsync)); 
            DoSomething(); 
        }

        public async Task<List<Employee>> InvokeEmployeeService() 
        { 
                . 
                . 
                . 
        }

        private async Task GetEmployeeDataFromServiceAsync() 
        { 
                . 
                . 
                . 
        }

        private void DoSomething() 
        { 
            Thread.Sleep(5000); 
        } 
    } 
}
```

Dikkat edileceği üzere RegisterAsyncTask metoduna yapılan çağrının hemen ardından içerisinde sayfaya ait Thread'i 5 saniye kadar geciktiren bir fonksiyon çağrısı daha yapılmıştır. Buna göre çalışma zamanında aşağıdakine benzer bir sonucun alındığı görülebilir.

[![asynwa_4](/assets/images/2012/asynwa_4_thumb.png)](/assets/images/2012/asynwa_4.png)

Normal şartlarda sayfanın yaşam döngüsünü düşündüğümüzde, senkron yapılan bir işleyiş de servis tarafındaki 5 saniyelik gecikme ve içerideki DoSomething üzerinden gelen 5 saniyelik gecikme sonrası en az 10 saniyelik bir işlem süresi olması gerekmektedir. Aslında böyledir de

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_114.png)

Eğer sayfanın Trace modunu açarsak aşağıdaki Trace Information sonuçları ile karşılaşırız.

[![asynwa_5](/assets/images/2012/asynwa_5_thumb.png)](/assets/images/2012/asynwa_5.png)

Görüldüğü üzere sayfanın render edilme süresi yine 11 saniyeler civarındadır. Yaklaşık 5 saniyelik servis çağrı süresi + 5 saniyelik DoSomething süresi. Peki biz neyi başarmış olduk?

![Open-mouthed smile](/assets/images/2012/wlEmoticon-openmouthedsmile_31.png)

Başarılan, servis çağrısının sayfanın yaşam döngüsü içerisinde asenkron olarak gerçekleştirilebilmesidir

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_114.png)

Böylece geldik bir yazımızın daha sonuna. Bu makalemizde bir ASP.Net Web API servisinin, bir Web uygulaması içerisinden asenkron olarak nasıl çağırılabileceğini,.Net Framework 4.5 ile birlikte gelen async ve await keyword'lerini de işin içerisine katarak değerlendirmeye çalıştık. Benim için de oldukça yeni ve halen daha öğrenmeye çalıştığımı bir konu. Özellikle MVC (Model View Controller) tabanlı Asp.Net uygulamalarında bu asenkron çağırımları nasıl değerlendirebileceğimizi de incelemeye çalışıyorum. Elde ettiğim bulguları ve öğrendiklerimi her zaman ki gibi bloğumda sizlerle paylaşıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.