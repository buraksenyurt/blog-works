---
layout: post
title: "Silverlight - JSON ile Çalışmak"
date: 2010-08-13 01:15:00 +0300
categories:
  - silverlight-4-0
  - wcf-eco-system
  - wcf-webhttp-services
tags:
  - silverlight-4-0
  - wcf-eco-system
  - wcf-webhttp-services
  - csharp
  - xml
  - dotnet
  - linq
  - sql-server
  - wcf
  - workflow-foundation
  - silverlight
  - xaml
  - rest
  - json
  - web-service
  - http
  - iis
  - javascript
  - transactions
  - generics
  - visual-studio
---
Uzun süredir şöyle deliksiz uyuyamıyordum. Malum evde bir afacan var. Pek uyumayı sevmeyen, sürekli hareket halinde olmak isteyen S (h) arp Efe izin verdiğinde, eşim ve ben dinlenmek için çeşitli işlere dalıyoruz. Ben uzun süredir Bulmacalara takılmış durumdayım. Bir de şu eski dil karşılıklarını isteyen sorular olmasa. Geçtiğimiz günlerde yine böyle bir boşluk yakalamışken, kendimi bulmacalar arasında yüzerken buluverdim. Ancak bir süre sonra "...eski dildeki karşılığı..." sorularından sıkıldım ve televizyonda neler olduğuna bir akayım dedim.

![blg177_Giris.jpg](/assets/images/2010/blg177_Giris.jpg)

Televizyonda yandaki resimde görülen adam vardı ve ismi Jason'dı. Açıkçası Jason Statham'ın fanatiği bir sinemasever olarak bu isim benzerliğinin, böyle korkutucu bir karakter üzerinde olması beni üzmüştü. Nitekim Jason ismini düşününce aklıma gıcır gıcır parlayan Audi marka arabalar gelmekteydi. Her neyse...Filme fazla takılmadım ama Jason, Jason derken, bu isim JSON diye dudaklarımdan süzülmeye başladı. Pek tabi bunun doğal sonucu olarak bilgisayarımın başına oturdum ve JSON ile ilişkili bir şeyler yazmaya karar verdim. İşte başlıyoruz

![Wink](/assets/images/2010/smiley-wink.gif)

Bildiğiniz üzere HTTP bazlı WCF servislerinden ([WCF WebHttp Services - JSON Formatlı Response Üretmek](https://www.buraksenyurt.com/admin/app/editor/post/WCF-WebHttp-Services-JSON-Formatli-Response-Uretmek)) JSON (JavaScript Object Notation) formatında çıktılar yayınlanabilmektedir. Bazı durumlarda istemci tarafı, JSON veri içeriği ile çalışmayı tercih edilebilir. Özellikle XML ile karşılaştırıldığında, JSON formatının daha az yer tutan bir yapıya sahip olması, bu seçimin yapılmasında önemli bir etkendir. Biz bu yazımızda bir WCF WebHttp Service tarafından yayınlanan JSON formatlı veri çıktısının, örnek bir Silverlight istemcisi tarafından nasıl ele alınabileceğini incelemeye çalışıyor olacağız.

Silverlight tarafında JSON içeriği ile çalışabilmek adına geliştirilmiş JsonArray, JsonObject, JsonPrimitive gibi tipler bulunmaktadır. Bu tipler sayesinde JSON veri kümesinde yer alan string, number, Boolean gibi veri türleri kod içerisinde ele alınabilir. Ayrıca tek JSON nesnesi veya bir JSON nesne listesinin ele alınması da sağlanabilir. Bu geliştiriciler için önemlidir. Nitekim Web ortamında gelen JSON içeriğinin Parse edilme işlemleri ile uğraşılmasına gerek kalmamaktadır.

Dilerseniz hiç vakit kaybetmeden örnek bir Silverlight uygulaması üzerinden ilerlemeye çalışalım. İşe ilk olarak IIS üzerinde host edeceğimiz WCF Rest Service Application projesini ve aşağıdaki kod içeriğine sahip LogService servis örneğini geliştirerek başlayabiliriz.

```csharp
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace TraceLogServiceApplication
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
    public class LogService
    {
        [WebGet(UriTemplate = "Logs/All",ResponseFormat=WebMessageFormat.Json)]
        public List<Log> GetAllLogs()
        {
            return new List<Log>()
            {
                new Log{ Source="Sql Server", Content="Sql servisi başlatıldı", IsCritical=false, Level=5},
                new Log{ Source="Sql Server", Content="Sql Agent servisinde hata.", IsCritical=true, Level=1},
                new Log{ Source="DTC", Content="Dağıtık Transaction nesnesi üretildi.", IsCritical=false, Level=3},
                new Log{ Source="WF Runtime", Content="Süreç persist edildi", IsCritical=true, Level=2}
            };
        }
    }

    public class Log
    {
        public string Source { get; set; }
        public string Content { get; set; }
        public int Level { get; set; }
        public bool IsCritical { get; set; }
    }
}
```

LogService içerisinde yer alan GetAllLogs isimli servis operasyonu Log tipinden bir kaç eleman içeren basit bir List koleksiyonunu geriye döndürmektedir. Çalışma zamanında oluşturulacak olan bu içerik, istemci tarafına JSON formatında gönderilecektir. Bunun için dikkat edileceği üzere ResponseFormat özelliğinin değeri WebMessageFormat.Json sabiti olarak belirlenmiştir. Servisimizi bu haliyle test etmek istediğimizde adres satırından http://localhost:12043/LogService/Logs/All gibi bir çağrı yapmamız yeterli olacaktır. Bunun sonucunda aşağıdaki JSON içeriği üretilecektir.

```xml
[{"Content":"Sql servisi başlatıldı","IsCritical":false,"Level":5,"Source":"Sql Server"},{"Content":"Sql Agent servisinde hata.","IsCritical":true,"Level":1,"Source":"Sql Server"},{"Content":"Dağıtık Transaction nesnesi üretildi.","IsCritical":false,"Level":3,"Source":"DTC"},{"Content":"Süreç persist edildi","IsCritical":true,"Level":2,"Source":"WF Runtime"}]
```

Bu işlemin ardından servisi IIS alınta Publish etmemiz yeterlidir. Publish ayarlarını aşağıdaki resimde görüldüğü gibi belirleyebiliriz.

![blg177_PublishProfile.gif](/assets/images/2010/blg177_PublishProfile.gif)

Eğer Publish işlemi başarılı olduysa (IIS üzerinden ilgili uygulamanın Web Application olarak set edilmesine-Convert to Application seçeneği dikkat ederekten) herhangibir tarayıcı uygulamadan, http://localhost/TraceLogServiceApplication/LogService/Logs/All şeklinde bir çağrıda bulunabiliyor olmamız gerekmektedir ki bu çağrının sonucu olarakta, yukarıdaki JSON içeriğine tekrardan ulaşabiliyor olmalıyız.

[TraceLogServiceApplication.rar (30,47 kb)](/assets/files/2010/TraceLogServiceApplication.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde test edilmiştir]

Tabi yapmamız gereken bir işlem daha bulunmaktadır. Hatırlayacağınız üzere Silverlight istemcileri için Cross-Domain Policy sorunsalı mevcuttur. Bu nedenle IIS üzerinde daha önceki yazılarda değindiğimiz ClientAccessPolicy.xml dosyasının içeriğini aşağıdaki gibi düzenlememiz ve TraceLogServiceApplication için gerekli garanti haklarını (grant-to) belirlememiz gerekmektedir.

![blg177_CAPFile.gif](/assets/images/2010/blg177_CAPFile.gif)

Artık Silverlight 4.0 tabanlı istemci uygulamamızı geliştirmeye başlayabiliriz. Bu amaçla, JsonConsumer isimli Silverlight uygulamamız içerisindeki MaingPage.xaml ve kod içerikleri aşağıdaki gibi geliştirilebilir.

MainPage.xaml içeriği;

```xml
<UserControl x:Class="JsonConsumer.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    d:DesignHeight="300" d:DesignWidth="400" xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk">

    <Grid x:Name="LayoutRoot" Background="White">
        <Button Content="Get All Logs" Height="23" HorizontalAlignment="Left" Margin="24,20,0,0" Name="GetLogsButton" VerticalAlignment="Top" Width="75" Click="GetLogsButton_Click" />
        <sdk:DataGrid AutoGenerateColumns="True" ItemsSource="{Binding}" Height="204" HorizontalAlignment="Left" Margin="24,56,0,0" Name="LogsDataGrid" VerticalAlignment="Top" Width="347"/>
    </Grid>
</UserControl>
```

MainPage.xaml.cs içeriği;

```csharp
using System;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

// JsonArray, JsonObject gibi tiplerin kullanılabilmesi için Silverlight projesine System.Json.dll assembly' ının referans edilmesi gerekmektedir.

namespace JsonConsumer
{
    public partial class MainPage 
        : UserControl
    {
        WebClient client = null;

        public MainPage()
        {
            InitializeComponent();

            client= new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
        }

        private void GetLogsButton_Click(object sender, RoutedEventArgs e)
        {
            client.OpenReadAsync(new Uri("http://localhost/TraceLogServiceApplication/LogService/Logs/All"));  
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            Stream responseStream = e.Result;
            // JsonArray sınıfının static Load metodu, Http Web servisine yapılan talep sonrası dönen Stream örneğini alır.
            // Load metodu JsonValue tipinden bir referans döndürmektedir ve dizi olarak ele alabilmek için JsonArray tipine bilinçli bir dönüşüm yapılmıştır.
            JsonArray logs=(JsonArray)JsonArray.Load(responseStream);
            // Elde edilen JSON verisinden IsCritical değeri true olanlar çekilir ve LogInfo isimli tip içerisinde toplanır.
            var criticialLogs = from log in logs
                                where log["IsCritical"]
                                select new LogInfo
                                {
                                     Content=log["Content"].ToString(),
                                     Source=log["Source"].ToString(),
                                     Level=log["Level"]
                                };

            // Elde edilen veri kümesi DataGrid kontrolüne veri kaynağı olarak gösterilir
            LogsDataGrid.DataContext = criticialLogs;
        }
    }
    // Servis tarafındaki Log tipinin istemci tarafındaki karşılığı
    public class LogInfo
    {
        public string Source { get; set; }
        public string Content { get; set; }
        public int Level { get; set; }
        public bool IsCritical { get; set; }
    }
}
```

Hatırlayacağınız üzere WCF WebHttp Service örneklerine yapılacak olan istemci çağrıları için WebClient tipinden yararlanılmaktadır. Bu amaçla Button kontrolüne basıldığında, asenkron olarak söz konusu servise bir talepte bulunulmaktadır (OpenReadAsync). Talep sonuçlandığında ise geri bildirim olay metodu devreye girmektedir (OpenReadCompleted). İşte bu olay metodu içerisinde JSON veri içeriğinin ele alınması için gerekli işlemler gerçekleştirilmektedir.

Bu metoda ait kod parçasındaki en büyük yardımıcı JsonArray tipi ve Load fonksiyonudur. Bu fonksiyon, parametre olarak LogService isimli WCF WebHttp Servisine gönderilen talep sonucu, istemci tarafına indirilen Stream referansını kullanmaktadır. Sonuç daha sonradan basit bir LINQ sorgusu ile değerlendirilmiş ve örnek olarak kritik seviyedeki log bilgilerinin değerlendirilmesi amaçlanmıştır. Uygulamanın çalışma zamanı görüntüsü aşağıdaki gibi olacaktır.

![blg177_Runtime.gif](/assets/images/2010/blg177_Runtime.gif)

Görüldüğü gibi JSON formatındaki içerik Silverlight tarafında başarılı bir şekilde ele alınmış ve veri bağlı bir kontrol (DataGrid) ile ilişkilendirilebilmiştir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[JsonConsumer.rar (1,92 mb)](/assets/files/2010/JsonConsumer.rar) [Örnek Visual Studio 2010 Ultimate sürümü üzerinde test edilmiştir]
