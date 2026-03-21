---
layout: post
title: "Asp.Net 4.5–Asenkron HTTP Module Geliştirmek"
date: 2013-10-13 11:00:00 +0300
categories:
  - aspnet-4-5
tags:
  - asp.net
  - http-handler
  - async
  - await
  - http-module
  - asynchttpmodule
  - module
  - asp.net-pipeline
  - asp.net-runtime
---
Bir çoğunuz gibi ben de düzenli olarak bazı dergilerin abonesiyim ve her ay onları alıp biraz karıştırdıktan sonra arşive (yani çalışma odasındaki kütüphaneye) kaldırmaktayım.

[![Pioneer Stereo Ad 1974](/assets/images/2013/Pioneer%20Stereo%20Ad%201974_thumb.jpg)](/assets/images/2013/Pioneer%20Stereo%20Ad%201974.jpg)


Tabi gün oluyor pek çoğuna dönüp bakmıyorum bile. Hatta kimisinin rengi sararıp soluyor bir köşede mazlum mazlum kalıyor. Ama eminim ki geride kalanlar için bazıları güncel içeriklere sahip iken, bazıları da tam anlamıyla bir Retro havası veriyor. Ve hatta çoğu, yıllar geçtikçe daha fazla değer kazanıyor.

Söz gelimi yandaki basılı medya görselinde 1974 model bir Pioneer ses sisteminin reklamı yer almakta. Bu pek çoğumuzun daha henüz hayatta bile olmadığı, olsa da pek hatırlamadığı bir yıl belki de? Sanıyorum blog tutan bizlerin yazıları da, zaman içerisinde bu şekilde eskiyor. Hele de yazılım teknolojileri gibi çok çabuk gelişen konular ele alınıyorsa.

Ben bazen geçmişte yazmış olduğum yazılara bakıyorum ve ne kadar da çabuk eskidiklerini görüyorum. Ancak bir yandan da, “acaba yeni sürümde bu konuda neler yapılmış?” sorusunun cevabını da kurcalamaya çalışıyorum.

> Nostalji: [2006 dan](https://www.buraksenyurt.com/post/HTTPHandler-ve-HttpModule-Kavramlarc4b1-bsenyurt-com-dan) bir makale. O zamanlar Asp.Net Pipeline’ ın önemli parçaları olan HttpModule ve HttpHandler kavramlarını incelemeye çalışmıştım.

İşte bu günkü konumuzda HttpModule tipleri içerisindeki işlemleri asenkron olarak nasıl yaptırabileceğimizi incelemeye çalışıyor olacağız. Bildiğiniz üzere.Net Framework 4.0 ile hayatımıza giren Task ve doğal olarak Task Parallel Library kavramı, 4.5 sürümünde gelen async ve await anahtar kelimeleri ile birlikte alt yapının pek çok noktasında daha sık görülmeye başladı. Bu açıdan bakıldığında Asp.Net 4.5 tarafında da ilgili anahtar kelime ve Task tiplerini kullanarak bazı senkronize edilmiş işlemlerin asenkron hale getirilmesi sağlanabilmekte.

> Asp.Net ve asenkron çalışma diyince bir duraksayıp düşünmek gerekir.
> Asp.Net aslında client-server modelini baz alarak çalışır ve stateless bir ortam sunar. Bu yüzden istemciden gelen her talep (Request) sonrası, uygulama ve talep gören sayfanın Lifecycle’ ının tekrar işletilmesi gerekir. Web tarafında asenkronluk diyince bunu belki de iki taraflı olarak düşünmek gerekebilir. Sunucu tarafında çalışan asenkron işlemler ve istemcinin kendi açısından asenkron olarak başlatıp, Response’ u beklemeye gerek kalmadan başka talepleri de gönderebildiği işlemler (Genelde AJAX tarzı çağrılar ile hallettiğimiz çalışma şekli)
> Module ve Handler gibi tipler sunucu tarafında çalışan yapılar olduklarından, yazıda bahsedeceğimiz asenkron çalışma aslında, sunucu üzerinde söz konusu olan ve LifeCycle yaşamı boyunca ele alınan yapılar olarak düşünülmelidirler.

Bu işten nasibini alan kısımlardan ikisi de Asp.Net uygulamalarının yaşam döngüsünde önemli yere sahip olan Module ve Handler’ lardır. Aslında bir Web sayfasını talep ettiğimizde, sunucu tarafında gerçekleşen bir dizi işlem söz konusudur. Ayrıca yaşam döngüsüne ait süreçte devreye giren bir kanal yapısı mevcuttur. Bu kanal yapısına göre istemciden gelen Request’ in önce Http bazlı Module’ lerden geçmesi ve ardından da Handler’ lar tarafından değerlendirilmesi söz konusudur. Klasik çalışma modeline baktığımızda aşağıdaki grafikte yer alan ve özetle ifade edilmeye çalışılan işleyişin gerçekleştiğini söyleyebiliriz.

Klasik Senkron Çalışma Modeli

[![ahm_1](/assets/images/2013/ahm_1_thumb.png)](/assets/images/2013/ahm_1.png)

Görüldüğü gibi gelen talep, Built-In bazı Http Module’ lerinden geçmekte ve Handler seviyesinde de değerlendirildikten sonra geriye döndürülmektedir. (Bu çizelgede talep edilen içeriğe ait sayfa veya user control’ un iç çalışma şekli göz ardı edilmiştir) Dikkat edilmesi gereken husus, Module ve Handler’ lardan oluşan bu kanal yapısının Thread havuzundan çekilen tek bir Thread içerisinde uçtan uca çalışıyor olmasıdır. Dolayısıyla bir module’ den diğerine olan geçiş sırasında, işlemekte olan module’ ün işini tamamlamış olması gerekmektedir. Aynı durum doğal olarak Handler’ lar için de geçerlidir.

Asenkron Çalışma Modeli

Asenkron çalışma modeline göre ise, bir Module veya Handler’ ın kendi çalışmasını farklı bir Thread’ e yıkması ve daha sonra ana Thread’ e sonuç dönerek işleyişi uzun süre duraksatmaması hedeflenmektedir. Bu, özellikle geliştirici tarafından yazılan Module veya Handler tipleri için kullanılabilecek bir stratejidir. Aşağıdaki grafikte bu yaklaşım modeli özetlenmeye çalışılmıştır.

[![ahm_2](/assets/images/2013/ahm_2_thumb.png)](/assets/images/2013/ahm_2.png)

Dikkat edileceği üzere X Module tipi kendi işleyişini farklı bir Thread altında yapmaktadır. Dolayısıyla işlenmekte olan talebin akışı sırasında X modülüne gelindikten sonra, sıradaki Module veya takip eden Handler’ lara geçilmesi için bir duraksama söz konusu değildir. İlgili Module çalışmasını tamamladığında, kanal içerisindeki işleyişe otomatikman dahil olacaktır.

Asp.Net 4.5 için Örnek Uygulama

Peki bu modeli Asp.Net 4.5 içerisinde nasıl gerçekleştirebiliriz? Gelin basit ve klasik bir örnek üzerinden ilerleyelim. Bu amaçla Visual Studio 2012 ortamında Asp.Net Empty Web Application tipinden bir proje açalım. Bir Web uygulamasında veya bir Application Pool içerisinde kendi Http Module’ lerimizi kullanabilmenin yolu IHttpModule arayüzünden türeyen sınıflar geliştirmekten geçmektedir.

IHttpModule kendi içerisinde Asenkron kullanım için doğal bir metod içermemektedir. Bunun yerine ezilmesi gereken Init ve Dispose metodlarını sunmaktadır. Init fonksiyonu tahmin edileceği üzere Module devreye girdiğinde yapılacak işlemleri içermekte ve icra ettirmektedir. Biz burada asenkron işleyişlere yer verebiliriz. Nasıl mı? İşte uygulama içerisindeki sınıf çizelgesi ve kod yapısı.

[![ahm_3](/assets/images/2013/ahm_3_thumb.png)](/assets/images/2013/ahm_3.png)

RequestLogInfo POCO tipinin içeriği;

```csharp
using System;

namespace HowTo_AsyncModules 
{ 
    public class RequestLogInfo 
    { 
        public string IpAddress { get; set; } 
        public DateTime Time { get; set; } 
        public bool IsAuthenticated { get; set; } 
        public string User { get; set; } 
        public string RequestType { get; set; } 
        public Uri Url { get; set; }

        public override string ToString() 
        { 
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}" 
                ,Time.ToLongTimeString() 
                ,IpAddress 
                ,IsAuthenticated.ToString() 
                ,User 
                ,RequestType 
                ,Url.ToString() 
                ); 
        } 
    } 
}
```

RequestLogInfo örnek senaryoda kullanacağımız ve içerisinde istemciden gelen talep (Request) ile ilişkili bir kaç basit bilgiyi içeren POCO (Plain Old CLR Object) sınıfımızdır. Gelelim AsyncHttpLogModule sınıfının içeriğine.

```csharp
using System; 
using System.Configuration; 
using System.IO; 
using System.Threading.Tasks; 
using System.Web;

namespace HowTo_AsyncModules 
{ 
    public class AsyncHttpLogModule 
        :IHttpModule 
    { 
        public void Dispose() 
        { 
         // Do Something   
        }

        public void Init(HttpApplication context) 
        { 
            EventHandlerTaskAsyncHelper eventHandler =new EventHandlerTaskAsyncHelper(WriteLogToTextFile); 
            context.AddOnPostAuthorizeRequestAsync( 
                eventHandler.BeginEventHandler 
                , eventHandler.EndEventHandler 
                ); 
        }

        private async Task WriteLogToTextFile(object sender, EventArgs e) 
        { 
            var context = HttpContext.Current; 
            string filePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["LogFileAddress"]);

            RequestLogInfo requestLog = new RequestLogInfo() 
            { 
                Time = DateTime.Now, 
                IpAddress = context.Request["REMOTE_ADDR"], 
                User = context.User.Identity.Name, 
                IsAuthenticated = context.User.Identity.IsAuthenticated, 
                RequestType = context.Request.RequestType, 
                Url = context.Request.Url 
            }; 
            using (StreamWriter streamWriter = File.AppendText(filePath)) 
            { 
                await streamWriter.WriteLineAsync(requestLog.ToString()); 
            } 
        } 
   } 
}
```

Örnek kod parçasında dikkat edileceği üzere Init metodu içerisinde gerçekleştirilen olay bazlı bir asenkron çalıştırma kayıt bildirimi söz konusudur. Init metodu içerisinde kullanılan EventHandlerTaskAsyncHelper sınıfı, TaskEventHandler temsilci (delegate) tipinden bir parametre almaktadır.

Aslında bu temsilci geriye Task örneği döndüren ve object ile EventArgs tipinden parametre alan metodları işaret edebilir ki örneğimizde bu fonksiyon WriteLogToTextFile’ dır. Bu metod async anahtar kelimesi ile işaretlendiğinden kendi içerisinde awaitable operasyonları da içerebilmektedir. Örnekte bunu sembolize etmek için StreamWriter tipinin asenkron çalışabilen WriteLineAsync metodu kullanılmıştır.

> Çok doğal olarak bir Module her zaman asenkron çalışacak şekilde tasarlanmamalıdır. Nitekim içerisindeki işlemlerin sonucuna göre sonraki Module’ lere bilgi taşınması veya çalışmanın kesilerek anında bir Exception döndürülmesi vb durumlar söz konusu olabilir.

İlgili asenkron işleyişlerde text tabanlı olarak fiziki bir dosyaya log atma işlemi sembolize edilmiştir. Pek tabi bunun yerine log içeriğinin veritabanına aktarılması, bir servise çağrıda bulunulması, harici 3ncü parti bir arayüz ile konuşulması (Örneğin bir Bussines Process Management sistemine mesaj gönderilmesi) ve benzeri zaman alabilecek ve dolayısıyla asenkron hale getirilebilecek işlemler ele alınabilir. Uygulamayı teste tabi tutmadan önce Web.config dosyası içerisinde, tasarlanmış olan yeni HttpModule tipine ait bildirimin de yapılması gerekmektedir ki çalışma zamanı var olan Module’ lere ek olarak AsyncHttpLogModule örneğini de devreye alabilsin. Bunun için system.webServer elementi içerisinde aşağıdaki module tanımlaması yapılmalıdır.

Config Ayarlarmaları

```xml
<?xml version="1.0"?> 
<configuration> 
    <system.web> 
      <compilation debug="true" targetFramework="4.5" /> 
      <httpRuntime targetFramework="4.5" /> 
    </system.web> 
    <system.webServer> 
      <modules> 
        <add name="AsyncLogModule" type="HowTo_AsyncModules.AsyncHttpLogModule,HowTo_AsyncModules"/> 
      </modules> 
  </system.webServer> 
  <appSettings> 
    <add key="LogFileAddress" value="~/App_Data/Logs.txt"/> 
  </appSettings>
</configuration>
```

modules elementi içerisinde dikkat edilmesi gereken nokta type niteliğine atanan değerdir. Burada [Namespace Adı].[Module Tip Ad],[Assembly Adı] notasyonundan yararlanılmıştır. Bu, özellikle ilgili Module tipinin harici bir Class Library içerisinde olduğu durumlarda ön plana çıkan önemli kurallardan birisidir.

Test

Test için dilerseniz çok basit bir de aspx sayfası hazırlayalım. Böylece basit anlamda Get, Post gibi aksiyonları simüle ettirmiş oluruz.

Örnek aspx içeriği;

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HowTo_AsyncModules.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
    Mesajınız : <asp:TextBox ID="txtMessage" runat="server" /><br /> 
        <asp:Button ID="btnSendMessage" runat="server" Text="Send My Message" OnClick="btnSendMessage_Click"/> 
    </div> 
    </form> 
</body> 
</html>
```

WebForm içerisinde bir TextBox ve Button kontrolü bulunmaktadır. Kullanıcı sayfayı ilk kez talep ettiğinde HTTP Get ve Button’ a her bastığında da HTTP Post tipinden talepler üretilmesine neden olmaktadır. Çalışma zamanında yapılan çeşitli aksiyonlar sonrası oluşan örnek bir log içeriği ise aşağıda görüldüğü gibidir.

[![ahm_4](/assets/images/2013/ahm_4_thumb.png)](/assets/images/2013/ahm_4.png)

Dikkat edileceği üzere geliştirici tanımlı bir HttpModule tipinin işleyişinin asenkron hale getirilmesi mümkündür. Aynı prensiplerden yola çıkarak bir HttpHandler tipinin de asenkron çalışacak hale getirilmesi söz konusu olabilir elbette. Lakin Asp.Net 4.5, HttpHandler tiplerinin asenkron yazılabilmesi için daha güçlü bir yol sunmaktadır. Bu yolun başında HttpTaskAsyncHandler isimli soyut sınıf (Abstract Class) yer almaktadır. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_AsyncModules.zip (25,83 kb)](/assets/files/2013/HowTo_AsyncModules.zip)