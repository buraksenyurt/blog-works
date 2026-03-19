---
layout: post
title: "SSIS - Programatik Olarak Variable Değeri Set Etmek"
date: 2011-11-11 14:59:00 +0300
description: "Ne varki bu ham veri içeriklerinin operasyonel düzeyde ele alınabilmeleri için ilişkisel hale getirilmeleri, bir başka deyişle SQL Server gibi ilişkisel veritabanı ortamlarına aktarılmarı gerekmektedir. Pek tabi tersi bir durumda çoğu zaman söz konusu olmaktadır. Bu gibi ihtiyaçlar dahilinde SSIS(Sql Server Integration Services) paketleri oldukça kullanışlıdır. Özellikle bir tasarım aracının söz konusu olması, zengin kontrol seti ve akış bazlı çalışma modeli önemli avantajlar olarak karşımıza çıkmaktadır."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - sql-server
  - xml
  - visual-studio
---
# SSIS - Programatik Olarak Variable Değeri Set Etmek
[![SQL-Server-2008-Grid-v-r_2-300x187](/assets/images/2011/SQL-Server-2008-Grid-v-r_2-300x187_thumb.png)](/assets/images/2011/SQL-Server-2008-Grid-v-r_2-300x187.png) Merhaba Arkadaşlar,

Beni tanıyanlar SQL ailesini pek sevmediğimi ve biraz uzak durmaya çalışmak istediğimi bilirler. Ne varki bazen iş hayatının gerçekleri ile karşı karşıya kalırız ve mecburen SQL ailesinin bazı fertleri ile yakın ilişkiler içerisine gireriz

![Confused smile](/assets/images/2011/wlEmoticon-confusedsmile_13.png)

Örneğin ben çalışmakta olduğum bankanın önemli bazı operasyonlarında SSIS (Sql Server Integration Services) paketleri ile çalışmak durumundayım. Özellikle bankaların metin tabanlı dosya formatlarını sıklıkla tercih ettiklerini biliyoruz.

Ne varki bu ham veri içeriklerinin operasyonel düzeyde ele alınabilmeleri için ilişkisel hale getirilmeleri, bir başka deyişle SQL Server gibi ilişkisel veritabanı ortamlarına aktarılmarı gerekmektedir. Pek tabi tersi bir durumda çoğu zaman söz konusu olmaktadır. Bu gibi ihtiyaçlar dahilinde SSIS (Sql Server Integration Services) paketleri oldukça kullanışlıdır. Özellikle bir tasarım aracının söz konusu olması, zengin kontrol seti ve akış bazlı çalışma modeli önemli avantajlar olarak karşımıza çıkmaktadır. Tabi böyle bir senaryo ve işin içerisinde benim gibi bir.Net geliştiricisi olunca, ister istemez bir SSIS paketini programatik olarak değerlendirmek söz konusu olabilmektedir

![Smile](/assets/images/2011/wlEmoticon-smile_22.png)

Ben de bu düşünceden yola çıkarak programatik anlamda bir SSIS paketinin değişkenlerine (Variables) dış ortamdan nasıl erişebileceğimizi ve değiştirebileceğimizi incelemeye çalıştım.

Normal şartlarda bir SSIS paketinin çalıştırılması için kullanılabilecek pek çok yol bulunmaktadır. Doğrudan designer aracı ile, komut satırından Dtcexec.exe programı ile, bir SQL Server Job şeklinde vb...Ancak hangi çalıştırma modeli seçilirse seçilsin bazen söz konusu paketin dış ortamdan parametrik değerler alması ve bunları içerisinde kullanması gerekmektedir. Bunun için de bir kaç yol mevcuttur aslında.

Söz gelimi paket seviyesindeki değişkenleri, pakete ait XML bazlı bir konfigurasyon dosyası içerisinde tutabiliriz. Bu sayede basit bir metin editörü yardımıyla söz konusu XML dosyası içeriğini değiştirebilir ve paketin yeni paremetre değerleri ile yürütülmesini sağlayabiliriz. Hatta bu XML dosyasını yönetimli kod (Managed Code) tarafında geliştireceğimiz bir kod parçasıda XML API'sini kullanaraktan da ele alabiliriz. Ancak ben bu yazımızda SSIS paketinin yönetimli kod tarafında nesnel olarak ele alınması ve bu tekniğe göre ilgili Variable değerlerinin değiştirilmesi üzerinde durmaya çalışacağım. Bu amaçla basit bir senaryo üzerinden ilerliyor olacağız.

Senaryomuza göre, çok basit ve sembolik bir SSIS paketinin kendi içerisinde kullandığı variable’ lara başka bir Console uygulaması üzerinden müdahale etmeyi ve paketin yeni ortam değerlerine göre yürütülmesini sağlamayı bekliyor olacağız. İlk olarak senaryomuz gereği aşağıdaki ekran görüntüsünde yer alan SSIS paketini geliştirdiğimizi düşünelim.

[![artcl_2_2](/assets/images/2011/artcl_2_2_thumb.gif)](/assets/images/2011/artcl_2_2.gif)

Paket içerisinde kullanılan değişkenlerimiz (Variables) ise şunlardır.

[![artcl_2_1](/assets/images/2011/artcl_2_1_thumb.gif)](/assets/images/2011/artcl_2_1.gif)

DatabaseName ve TableName isimli değişkenler string tipindendir ve paket seviyesinde tanımlanmışlardır. Paket içerisinde yer alan Script Task tipinden olan Task bileşeni ise kendi içerisinde söz konusu değişkenleri ReadOnly seviyede kullanmaktadır.

[![artcl_2_3](/assets/images/2011/artcl_2_3_thumb.gif)](/assets/images/2011/artcl_2_3.gif)

Script kodu ise aşağıdaki gibidir.

```csharp
using System; 
using System.IO;

namespace ST_7bc89561acee425798facb4212b1828a.csproj 
{ 
    [System.AddIn.AddIn("ScriptMain", Version = "1.0", Publisher = "", Description = "")] 
    public partial class ScriptMain : Microsoft.SqlServer.Dts.Tasks.ScriptTask.VSTARTScriptObjectModelBase 
    { 
        #region VSTA generated code

        enum ScriptResults 
        { 
            Success = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success, 
            Failure = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure 
        };

        #endregion

        public void Main() 
        { 
            string dbName = Dts.Variables["DatabaseName"].Value.ToString(); 
            string tbName = Dts.Variables["TableName"].Value.ToString(); 
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Results.txt"), String.Format("{0}.{1} için işlemler yapılacak.", dbName, tbName)); 
            Dts.TaskResult = (int)ScriptResults.Success; 
        } 
    } 
}
```

Bu tabi kobay bir SSIS paketi olduğundan ne yaptığının çok fazla önemi yoktur. Ancak senaryomuz gereği Script olarak çalıştırılan kod bloğu içerisindeki kullanım şekli önemlidir. Main metodunda paket seviyesindeki DatabaseName ve TableName değişkenleri kullanılmakta olup değerleri bir Text dosyaya yazdırılmaktadır. Aslına bakarsanız senaryomuzu test etmek için yeterli bir kod parçasıdır

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_73.png)

Bildiğiniz üzere amacımız başka bir.Net uygulamasını kullanarak söz konusu değişkenlerin dış ortamdan set edilmesini sağlamaktı. Şimdi dilerseniz basit bir Console uygulaması oluşturarak akışımıza devam edelim.

İlk olarak SSIS paketleri üzerinde yönetimli kod tarafını kullanabilmek için ilgili Assembly'ın projeye referans edilmesi gerekmektedir. Bu yüzden C:\Program Files\Microsoft SQL Server\100\SDK\Assemblies klasörü içerisinde yer alan Microsoft.SQLServer.ManagedDTS.dll'ini projeye referans etmemiz yeterlidir. (SSIS aslında eskiden DTS olarak anılan bir alt yapıdır. Bu yüzden assembly adlarına şaşırmayın)

[![artcl_2_4](/assets/images/2011/artcl_2_4_thumb.gif)](/assets/images/2011/artcl_2_4.gif)

Bundan sonrası ise oldukça basit. Hatta zevkli bir oyun gibi diyebilirim

![Smile](/assets/images/2011/wlEmoticon-smile_22.png)

İşte örnek kod parçamız.

```csharp
using System; 
using Microsoft.SqlServer.Dts.Runtime;

namespace SSISExecuter 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string packagePath = @"C:\Users\buraksenyurt\Documents\Visual Studio 2008\Projects\SIISProjects\SampleSSIS\SampleSSIS\bin\Package.dtsx";

            // Öncelikli olarak paketi yüklemek için kullanılacak uygulama nesnesi oluşturulur 
            Application app = new Application(); 
            // SSIS paketi yüklenir 
            Package package = app.LoadPackage(packagePath, null); 
            // Paket içerisinde tanımlanmış olan değişkenlere erişilir ve Value özelliklerinden yararlanılarak ilgili değerleri set edilir 
            package.Variables["DatabaseName"].Value = "AdventureWorks"; 
            package.Variables["TableName"].Value = "Production.Product"; 
            // Paket çalıştırılı ve sonucu alınarak değerlendirilir 
            DTSExecResult result = package.Execute(); 
            switch (result) 
            { 
                case DTSExecResult.Canceled: 
                    Console.WriteLine("Canceled"); 
                    break; 
                case DTSExecResult.Completion: 
                    Console.WriteLine("Completion"); 
                    break; 
                case DTSExecResult.Failure: 
                    Console.WriteLine("Failure"); 
                    break; 
                case DTSExecResult.Success: 
                    Console.WriteLine("Success"); 
                    break; 
                default: 
                    break; 
            } 
        } 
    } 
}
```

Görüldüğü üzere ilk olarak paketin ilgili adresten yüklenmesi sağlanmış ve arından Variables özelliği üzerinden DatabaseName ve TableName parametrelerine yeni değerleri aktarılmıştır. Son olarakta ilgili paket çalıştırılmıştır. Paketin çalıştırıcısı bu Console uygulaması olduğu için text dosyasının çıktısı da exe'nin bulunduğu dosya adresi olacaktır. İçeriği ise tam istediğimiz gibidir

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_73.png)

[![artcl_2_5](/assets/images/2011/artcl_2_5_thumb.gif)](/assets/images/2011/artcl_2_5.gif)

Sonuç olarak bir SSIS paketinin iç değişkenlerine dış ortamdan değer atamanın farklı bir yolunu görmüş olduk. Managed tarafta SSIS paketlerini daha da etkin yönetebilmemiz de mümkündür. Hatta bu kütüphaneyi kullanarak özellikle görsel SSIS yürütücüleri geliştirebilirsiniz. Bir düşünün

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_73.png)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[SampleSSIS.rar (25,47 kb)](/assets/files/2011/SampleSSIS.rar)

[SSISExecuter.rar (25,64 kb)](/assets/files/2011/SSISExecuter.rar)