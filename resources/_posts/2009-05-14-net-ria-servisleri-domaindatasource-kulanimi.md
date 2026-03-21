---
layout: post
title: ".Net RIA Servisleri - DomainDataSource Kulanımı"
date: 2009-05-14 12:30:00 +0300
categories:
  - dotnet-ria-services
tags:
  - .net-ria-services
  - silverlight
---
Her ne kadar şu günlerde güzel ülkemizin Ege kıyılarında kısa bir dinlenme molası vermiş olsamda, internetin sahil kıyılarındaki cafe'lere kadar girmiş olması, herşeyi değiştiriyor.

![blg17_4.jpg](/assets/images/2009/blg17_4.jpg)

![Cool](/assets/images/2009/smiley-cool.gif)

Artık bir yaşam tarzı haline gelen Yazılımdan, onun gizemli dünyasından uzak durmak bu nedenle, şu sıralar aşağıdaki şekilde görülen yerde tatilde bile olsam çok zor.

Bu kısa yazımda sizlere yine.Net RIA Servisleri ile ilişkili bilgilerimi aktarmaya gayret edeceğim. Bu seferki konumuz DomainDataSource isimli Silverlight kontrolü. Kontrolün adında yer alan DataSource son eki aslında olayı biraz olsun açıklamakta..Net RIA Servislerinin kullanıldığı senaryolarda, sunucu tarafında mutlaka bir veri kaynağı yer almaktadır. Ağırlık olarak Ado.Net Entity Framework veya LINQ to SQL tabanlı sağlayıcılar ile eriştiğimiz bu veri kaynaklarını, istemci tarafında değiştirmek gibi işlemlerle uğraştığımızda bir gerçektir. Kısacası, istemci tarafına çekilen verinin sadece gösterilmesi dışında, düzenlenmesi, yenilerinin eklenmesi veya var olanların silinmesi gibi operasyonlar söz konusudur. Bunlara ek olarak, Silverlight tabanlı istemci tarafını düşündüğümüzde, verinin kullanıcı ile etkileşimde olan kontrollerde gösterilmeside bu işin önemli kısımlarından birisidir.

Tam bu noktada aklıma Asp.Net 2.0 ile birlikte gelen veri-bağlı kontrolleri (Data-Bound Controls) geliyor. SqlDataSource, ObjectDataSource, SiteMapDataSource vb...Bu kontrollerin en büyük amacı, sayfa üzerindeki sunucu kontrollerini, veri kaynağını bağlamaktır. SqlDataSource gibi kontroller sayesinde bu bağlama işlemleri ile birlikte, Insert, Update ve Delete operasyonlarına hizmet edecek kod parçalarının kolay bir şekilde geliştirilmesi ve ele alınmasıda mümkün olmaktadır. Şu anda bulunduğumuz noktayı düşündüğümüzde,.Net RIA Servislerini kullanan Silverlight istemcileri içinde benzer bir kolaylığın sağlanması önemlidir. Öyleyse bu kontrol nasıl kullanılır, tam olarak ne işe yarar hemen bir bakalım.

Örnek Silverlight Projemizde bu kez Northwind veri kaynağına Ado.Net Entity Framework öğesi yardımıyla bağlanıyor olacağız. Yine bir önceki blog yazımızda olduğu gibi Categories tablosunu ve ek olarak Products tablosunu kullanabiliriz. İstemci tarafında, Insert, Update ve Delete gibi işlemleride ele alma ihtimalimiz olduğundan (en azından sonraki blog yazılarımda), DomainService tipinin eklenmesi sırasında, her iki Entity tipi içinde Enable Editing özelliğinin işaretli olduğuna dikkat etmemiz gerekmektedir. Gelelim kullanacağımız DomainDataSource bileşenine. Bu bileşen varsayılan olarak Silverlight kontrol sekmesinde görünmemektedir. Dolayısıyla söz konusu kontrolün,.Net RIA Service sisteme yüklendikten sonra Visual Studio 2008 ortamında kullanılabilmesi için Toolbox'a Silverlight Components kısmından eklenmesi gerekir.

![blg17_1.gif](/assets/images/2009/blg17_1.gif)

DataSource bileşenleri genellikle veri-bağlı kontroller ile kullanılırlar. DomainDataSource kontrolünü bu anlamda, DataGrid bileşeni ile etkileştirebiliriz. XAML içeriğimizin ilk halini aslında aşağıdaki gibi tasarladım.

```xml
<UserControl xmlns:riaControls="clr-namespace:System.Windows.Controls;assembly=System.Windows.Ria.Controls"  
    xmlns:data="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data" 
    x:Class="DomainDS.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
    xmlns:ds="clr-namespace:DomainDS.Web"
    Width="500" Height="300">    
    <Grid x:Name="LayoutRoot" Background="White">
        <riaControls:DomainDataSource x:Name="dsProducts" AutoLoad="True" LoadSize="10" LoadMethodName="LoadProducts">
            <riaControls:DomainDataSource.DomainContext>
                <ds:NorthwindContext/>
            </riaControls:DomainDataSource.DomainContext>
        </riaControls:DomainDataSource>
    <data:DataGrid x:Name="dgProducts" Height="Auto" Background="BlanchedAlmond" ItemsSource="{Binding Data, ElementName=dsProducts}"/>
    </Grid>
</UserControl>
```

Şimdi bu XAML içeriğinde, üzerinde durulması gereken önemli noktalar olduğu aşikardır. Öncelikli olarak DataGrid veya DomainDataSouce kontrollerini kullanabilmemiz için gerekli assembly veya isim alanları (Namespaces), ilgili bileşenleri XAML içerisine sürüklediğimizde, eğer gerekiyorsa projeye otomatik olarak dahil edileceklerdir. riaControls ön eki ile eklenen DomainDataSource elementi içerisinde kullanılan bazı nitelik verileri dikkate değerdir.

LoadSize niteliğine atanan değer, Silverlight uygulaması ilk yüklendiğinde çekilecek olan satır sayısını belirtmektedir. Gerçektende, uygulama bu haliyle çalıştırıldığında, SQL Server Profiler aracından yakalanan sorgu cümlesi aşağıdaki gibidir.

```text
SELECT 
[Limit1].[C1] AS [C1], 
[Limit1].[ProductID] AS [ProductID], 
[Limit1].[ProductName] AS [ProductName], 
[Limit1].[SupplierID] AS [SupplierID], 
[Limit1].[QuantityPerUnit] AS [QuantityPerUnit], 
[Limit1].[UnitPrice] AS [UnitPrice], 
[Limit1].[UnitsInStock] AS [UnitsInStock], 
[Limit1].[UnitsOnOrder] AS [UnitsOnOrder], 
[Limit1].[ReorderLevel] AS [ReorderLevel], 
[Limit1].[Discontinued] AS [Discontinued], 
[Limit1].[CategoryID] AS [CategoryID]
FROM ( SELECT TOP (10) 
 [Extent1].[ProductID] AS [ProductID], 
 [Extent1].[ProductName] AS [ProductName], 
 [Extent1].[SupplierID] AS [SupplierID], 
 [Extent1].[CategoryID] AS [CategoryID], 
 [Extent1].[QuantityPerUnit] AS [QuantityPerUnit], 
 [Extent1].[UnitPrice] AS [UnitPrice], 
 [Extent1].[UnitsInStock] AS [UnitsInStock], 
 [Extent1].[UnitsOnOrder] AS [UnitsOnOrder], 
 [Extent1].[ReorderLevel] AS [ReorderLevel], 
 [Extent1].[Discontinued] AS [Discontinued], 
 1 AS [C1]
 FROM [dbo].[Products] AS [Extent1]
)  AS [Limit1]
```

Sanıyorumki SELECT TOP (10) ifadesi sizlerin dikkatinizden kaçmamıştır. Ancak LoadSize niteliği kaldırılırsa bu durumda SQL tarafında aşağıdaki sorgunun çalıştırıldığı görülecektir.

```text
SELECT 
1 AS [C1], 
[Extent1].[ProductID] AS [ProductID], 
[Extent1].[ProductName] AS [ProductName], 
[Extent1].[SupplierID] AS [SupplierID], 
[Extent1].[QuantityPerUnit] AS [QuantityPerUnit], 
[Extent1].[UnitPrice] AS [UnitPrice], 
[Extent1].[UnitsInStock] AS [UnitsInStock], 
[Extent1].[UnitsOnOrder] AS [UnitsOnOrder], 
[Extent1].[ReorderLevel] AS [ReorderLevel], 
[Extent1].[Discontinued] AS [Discontinued], 
[Extent1].[CategoryID] AS [CategoryID]
FROM [dbo].[Products] AS [Extent1]
```

Bu kez tüm Products tablosunun içeriği seçilmektedir. LoadSize özelliğini kullanarak, Silverlight uygulamasının ilk açılışı sırasındaki veri kümesinin yoğunluğunu kontrol altına alabilir ve performansı doğudan etkileyebiliriz. Burada dikkat çeken bir diğer önemli nitelik ise LoadMethodName niteliğine atanan değerdir. Bu değer, istemci tarafında kullanılan DomainContext tipinin içerisinde yer alan yükleme metodunun kendisidir. Örneğimizde bu metod LoadCategories isimli fonksiyondur. Ancak fonksiyon adı text tabanlı olarak yazılmaktadır. Peki, DomainDataSource bileşeni, hangi DataContext nesne örneği içerisindeki LoadCategories metodunu kullanacağını nasıl bilecektir?

![Undecided](/assets/images/2009/smiley-undecided.gif)

Bu sorunun cevabı, DomainDataSource.DomainContext elementi içerisinde verilmektedir. Bu kısımda, ds ön ekli namespace üzerinden NorthwindContext isimli DomainContext nesne referansının tanımlaması yapılmaktadır. Böylece, DomainDataSource bileşeninin kullanacağı DomainContext nesne örneği belirlenmiş olur.

DataGrid bileşeninin söz konusu DomainDataSource kontrolüne bağlanması içinse, ItemsSource niteliğine ilgili değerin atanması yeterlidir. (İtiraf etmeliyim ki, ItemsSource niteliğine atanan değerin yazım stilini, ne kadar dekleratif olsada halen ezbere yazamamaktayım

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

) Uygulamayı bu haliyle çalıştırdığımızda aşağıdaki ekran görüntüsü ile karşılaşmamız son derece muhtemeldir.

![blg17_2.gif](/assets/images/2009/blg17_2.gif)

Burada önemli olan noktalardan birisi, tanımlamalarım tamamen dekleratif olarak yapılmış olması ve geliştiricinin herhangibir kodlama yapmamış olmasıdır. Daha önceki blog yazılarımda yer alan örnekler göz önüne aldığımızda, CRUD operasyonları için istemci tarafında bazı kodlamalar yaptığımız ortadadır. Şimdi XAML içeriğimizi biraz daha zengineştirmeye çalışalım. Örneğin, sıralama kriteri ekleyebiliriz. Bunun için SortDescriptor elementinin kullanılması gerekmektedir. Bu element, System.Windows.Ria.Controls.dll assembly'ı içerisinde yer aldığında, isim alanının XAML içeriğinde bildirilmesi gerekir. Bu şekilde başlayan düzenlemelerin son hali aşağıdaki gibidir.

```csharp
<UserControl xmlns:riaControls="clr-namespace:System.Windows.Controls;assembly=System.Windows.Ria.Controls"  xmlns:data="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data" 
    x:Class="DomainDS.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
    xmlns:ds="clr-namespace:DomainDS.Web"
    xmlns:riaData="clr-namespace:System.Windows.Data;assembly=System.Windows.Ria.Controls"
    Width="500" Height="300">    
    <Grid x:Name="LayoutRoot" Background="White">
        <riaControls:DomainDataSource x:Name="dsProducts" AutoLoad="True" LoadSize="40" LoadMethodName="LoadProducts">
            <riaControls:DomainDataSource.DomainContext>
                <ds:NorthwindContext/>
            </riaControls:DomainDataSource.DomainContext>
            <riaControls:DomainDataSource.SortDescriptors>
                <riaData:SortDescriptor Direction="Descending" PropertyPath="UnitPrice"/> 
            </riaControls:DomainDataSource.SortDescriptors>
        </riaControls:DomainDataSource>
    <data:DataGrid x:Name="dgProducts" Height="Auto" Background="BlanchedAlmond" ItemsSource="{Binding Data, ElementName=dsProducts}"/>
    </Grid>
</UserControl>
```

SortDescription elementi içerisinde yer alan Direction niteliğine atanan değer ile sıralamanın yönü belirtilmektedir. Diğer taraftan PropertyPath niteliğine atanan değer ilede hangi alana göre sıralama yapılacağına karar verilir. Bu ayarlamalara göre Products tablosundan ilk yüklemede 40 adet ürün bilgisi, UnitPrice değerlerine göre ters sırada çekilecektir. Nitekim SQL tarafında çalıştırılan sorguya bakıldığında aşağıdaki cümlenin çalıştırıldığı kolayca tespit edilebilir.

```text
SELECT TOP (40) 
[Project1].[C1] AS [C1], 
[Project1].[ProductID] AS [ProductID], 
[Project1].[ProductName] AS [ProductName], 
[Project1].[SupplierID] AS [SupplierID], 
[Project1].[QuantityPerUnit] AS [QuantityPerUnit], 
[Project1].[UnitPrice] AS [UnitPrice], 
[Project1].[UnitsInStock] AS [UnitsInStock], 
[Project1].[UnitsOnOrder] AS [UnitsOnOrder], 
[Project1].[ReorderLevel] AS [ReorderLevel], 
[Project1].[Discontinued] AS [Discontinued], 
[Project1].[CategoryID] AS [CategoryID]
FROM ( SELECT [Project1].[ProductID] AS [ProductID], [Project1].[ProductName] AS [ProductName], [Project1].[SupplierID] AS [SupplierID], [Project1].[CategoryID] AS [CategoryID], [Project1].[QuantityPerUnit] AS [QuantityPerUnit], [Project1].[UnitPrice] AS [UnitPrice], [Project1].[UnitsInStock] AS [UnitsInStock], [Project1].[UnitsOnOrder] AS [UnitsOnOrder], [Project1].[ReorderLevel] AS [ReorderLevel], [Project1].[Discontinued] AS [Discontinued], [Project1].[C1] AS [C1], row_number() OVER (ORDER BY [Project1].[UnitPrice] DESC) AS [row_number]
 FROM ( SELECT 
  [Extent1].[ProductID] AS [ProductID], 
  [Extent1].[ProductName] AS [ProductName], 
  [Extent1].[SupplierID] AS [SupplierID], 
  [Extent1].[CategoryID] AS [CategoryID], 
  [Extent1].[QuantityPerUnit] AS [QuantityPerUnit], 
  [Extent1].[UnitPrice] AS [UnitPrice], 
  [Extent1].[UnitsInStock] AS [UnitsInStock], 
  [Extent1].[UnitsOnOrder] AS [UnitsOnOrder], 
  [Extent1].[ReorderLevel] AS [ReorderLevel], 
  [Extent1].[Discontinued] AS [Discontinued], 
  1 AS [C1]
  FROM [dbo].[Products] AS [Extent1]
 )  AS [Project1]
)  AS [Project1]
WHERE [Project1].[row_number] > 40
ORDER BY [Project1].[UnitPrice] DESC
```

Oldukça kolay gördüğünüz gibi..Net RIA Servislerini sisteme yüklediğimizde gelen dökümantasyon içerisinde bu tip bir örnek yapılmaktadır. İlerleyen kısımlarında, verinin çekilmesi işlemi sırasında kullanılabilecek sayfalama (Paging) ve filtreleme (Filtering) seçenekleride örneğe dahil edilmektedir. Size tavsiyem söz konusu dökümantasyonda yer alan örneği incelemeniz olacaktır.

Ben yazımı sonlandırmadan önce sayfalama kriterinide XAML içeriğine dahil etmeye çalışacağım. Bu amaçla, DataPager isimli Silverlight bileşenini XAML içerisine sürüklememiz yeterli olacaktır. MainPage.xaml içeriğinin son hali aşağıdaki gibidir.

```xml
<UserControl xmlns:dataControls="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data.DataForm"  xmlns:riaControls="clr-namespace:System.Windows.Controls;assembly=System.Windows.Ria.Controls"  xmlns:data="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data" 
    x:Class="DomainDS.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
    xmlns:ds="clr-namespace:DomainDS.Web"
    xmlns:riaData="clr-namespace:System.Windows.Data;assembly=System.Windows.Ria.Controls"
    Width="500" Height="350">    
    <StackPanel x:Name="LayoutRoot" Background="White" Orientation="Vertical">
        <riaControls:DomainDataSource x:Name="dsProducts" AutoLoad="True" LoadSize="40" LoadMethodName="LoadProducts">
            <riaControls:DomainDataSource.DomainContext>
                <ds:NorthwindContext/>
            </riaControls:DomainDataSource.DomainContext>
            <riaControls:DomainDataSource.SortDescriptors>
                <riaData:SortDescriptor Direction="Descending" PropertyPath="UnitPrice"/> 
            </riaControls:DomainDataSource.SortDescriptors>
        </riaControls:DomainDataSource>
    <data:DataGrid x:Name="dgProducts" Height="330" Background="BlanchedAlmond" ItemsSource="{Binding Data, ElementName=dsProducts}"/>
    <dataControls:DataPager PageSize="20" Height="20" Source="{Binding Data,ElementName=dsProducts}"></dataControls:DataPager>
    </StackPanel>
</UserControl>
```

DataPager kontrolü doğal olarak kimi (yani hangi veri kaynağını) sayfalayacağını bilmek zorundadır. Bu nedenle Source niteliğine dsProducts isimli DomainDataSource bileşeni atanmıştır. Diğer taraftan PageSize niteliğine atanan değer ile her sayfada 20 adet satırın gösterileceği belirtilmektedir. Uygulamayı bu haliyle çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü ile karşılaşmamız muhtemeldir.

![blg17_3.gif](/assets/images/2009/blg17_3.gif)

Burada dikkat çeken noktalardan biriside LoadSize ile ilk etapta 40 satırın yüklenmesine rağmen, sayfalama içerisinde en çok 80 (4X20) kaydın gösterilebilecek olmasıdır. Bu aslında kayda değer ve incelenmesi gereken bir durumdur. Nitekim SQL tarafında çalıştırılan sorgu cümelelerine dikkatlice bakmak gerekmektedir. İşte eğlence başlıyor.

![Tongue out](/assets/images/2009/smiley-tongue-out.gif)

Sayfa ilk yüklendiğinde TOP 40 ile 40 satırlık bir veri bloğunun yüklenmesi sağlanır. PageSize değeri 20 olarak berlilendiğinden 1nci sayfadan 2nci sayfaya geçtiğimizde, SQL tarafında herhangibir sorgu çalıştırılmadığı gözlemlenir.(İyi bir gelişme

![Wink](/assets/images/2009/smiley-wink.gif)

) Ancak 3ncü sayfaya geçmek istediğimizde, 40 satırlık yükleme boyutunu geçtiğimiz için sunucu tarafında yeni bir SQL sorgusu çalıştırılacak ve rownumber değeri 40' ın üzerinde olanlar talep edilecektir. Aşağıdaki SQL cümlesinde görüldüğü gibi...

```text
SELECT TOP (40) 
[Project1].[C1] AS [C1], 
[Project1].[ProductID] AS [ProductID], 
[Project1].[ProductName] AS [ProductName], 
[Project1].[SupplierID] AS [SupplierID], 
[Project1].[QuantityPerUnit] AS [QuantityPerUnit], 
[Project1].[UnitPrice] AS [UnitPrice], 
[Project1].[UnitsInStock] AS [UnitsInStock], 
[Project1].[UnitsOnOrder] AS [UnitsOnOrder], 
[Project1].[ReorderLevel] AS [ReorderLevel], 
[Project1].[Discontinued] AS [Discontinued], 
[Project1].[CategoryID] AS [CategoryID]
FROM ( SELECT [Project1].[ProductID] AS [ProductID], [Project1].[ProductName] AS [ProductName], [Project1].[SupplierID] AS [SupplierID], [Project1].[CategoryID] AS [CategoryID], [Project1].[QuantityPerUnit] AS [QuantityPerUnit], [Project1].[UnitPrice] AS [UnitPrice], [Project1].[UnitsInStock] AS [UnitsInStock], [Project1].[UnitsOnOrder] AS [UnitsOnOrder], [Project1].[ReorderLevel] AS [ReorderLevel], [Project1].[Discontinued] AS [Discontinued], [Project1].[C1] AS [C1], row_number() OVER (ORDER BY [Project1].[UnitPrice] DESC) AS [row_number]
 FROM ( SELECT 
  [Extent1].[ProductID] AS [ProductID], 
  [Extent1].[ProductName] AS [ProductName], 
  [Extent1].[SupplierID] AS [SupplierID], 
  [Extent1].[CategoryID] AS [CategoryID], 
  [Extent1].[QuantityPerUnit] AS [QuantityPerUnit], 
  [Extent1].[UnitPrice] AS [UnitPrice], 
  [Extent1].[UnitsInStock] AS [UnitsInStock], 
  [Extent1].[UnitsOnOrder] AS [UnitsOnOrder], 
  [Extent1].[ReorderLevel] AS [ReorderLevel], 
  [Extent1].[Discontinued] AS [Discontinued], 
  1 AS [C1]
  FROM [dbo].[Products] AS [Extent1]
 )  AS [Project1]
)  AS [Project1]
WHERE [Project1].[row_number] > 40
ORDER BY [Project1].[UnitPrice] DESC
```

Ne yazıkki 20 satır veri çekilmesi gerekmesine rağmen LoadSize özelliği nedeniyle Top 40 kullanımı söz konusudur. (

![Undecided](/assets/images/2009/smiley-undecided.gif)

Bu açıkçası benim pek beklediğim bir durum değildi.) Peki 4ncü sayfaya geçmek istersek ne olacaktır? Bu durumda rownumber değeri 60' ın (3X20 veya 3ncü sayfa X PageSize) üzerinde olan veriler çekilmeye çalışılacaktır.

```csharp
SELECT TOP (40) 
[Project1].[C1] AS [C1], 
[Project1].[ProductID] AS [ProductID], 
[Project1].[ProductName] AS [ProductName], 
[Project1].[SupplierID] AS [SupplierID], 
[Project1].[QuantityPerUnit] AS [QuantityPerUnit], 
[Project1].[UnitPrice] AS [UnitPrice], 
[Project1].[UnitsInStock] AS [UnitsInStock], 
[Project1].[UnitsOnOrder] AS [UnitsOnOrder], 
[Project1].[ReorderLevel] AS [ReorderLevel], 
[Project1].[Discontinued] AS [Discontinued], 
[Project1].[CategoryID] AS [CategoryID]
FROM ( SELECT [Project1].[ProductID] AS [ProductID], [Project1].[ProductName] AS [ProductName], [Project1].[SupplierID] AS [SupplierID], [Project1].[CategoryID] AS [CategoryID], [Project1].[QuantityPerUnit] AS [QuantityPerUnit], [Project1].[UnitPrice] AS [UnitPrice], [Project1].[UnitsInStock] AS [UnitsInStock], [Project1].[UnitsOnOrder] AS [UnitsOnOrder], [Project1].[ReorderLevel] AS [ReorderLevel], [Project1].[Discontinued] AS [Discontinued], [Project1].[C1] AS [C1], row_number() OVER (ORDER BY [Project1].[UnitPrice] DESC) AS [row_number]
 FROM ( SELECT 
  [Extent1].[ProductID] AS [ProductID], 
  [Extent1].[ProductName] AS [ProductName], 
  [Extent1].[SupplierID] AS [SupplierID], 
  [Extent1].[CategoryID] AS [CategoryID], 
  [Extent1].[QuantityPerUnit] AS [QuantityPerUnit], 
  [Extent1].[UnitPrice] AS [UnitPrice], 
  [Extent1].[UnitsInStock] AS [UnitsInStock], 
  [Extent1].[UnitsOnOrder] AS [UnitsOnOrder], 
  [Extent1].[ReorderLevel] AS [ReorderLevel], 
  [Extent1].[Discontinued] AS [Discontinued], 
  1 AS [C1]
  FROM [dbo].[Products] AS [Extent1]
 )  AS [Project1]
)  AS [Project1]
WHERE [Project1].[row_number] > 60
ORDER BY [Project1].[UnitPrice] DESC
```

Yinede TOP 40 oluşumu söz konusudur. Ancak istediğimiz sonuç alınmıştır. Sayfalama işlemide başarılı bir şekilde gerçekleştirilmiştir. Böylece geldik bir blog yazımızın daha sonuna. Şimdi müsadenizle biraz dinlenmeye çekileceğim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
