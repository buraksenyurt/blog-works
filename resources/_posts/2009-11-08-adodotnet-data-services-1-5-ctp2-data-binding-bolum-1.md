---
layout: post
title: "Ado.Net Data Services 1.5 CTP2 - Data Binding Bölüm 1"
date: 2009-11-08 18:30:00 +0300
description: "Ado.Net Data Services v1.5 ile birlikte istemci tarafına getirilen DataServiceCollection isimli koleksiyonun veri bağlama işlemlerinde kullanılabilmekte olup, CTP2 versiyonunda dahada iyileştirilmiş olarak karşımıza çıkmaktadır. Buna göre istemci tarafı için üretilen kütüphanede(Client Library) kolaylaştırıcı değişiklikler yapıldığı söylenebilir. DataServiceCollection<T> koleksiyonu ObservabelCollection<T> tipinden türemekte olup, INotifyPropertyChanged ve INotifyCollectionChanged arayüzlerini(Interface) uygulamaktadır. Aşağıdaki Object Browser çıktısında bu tipin içeriği açık bir şekilde görülmektedir."
categories:
  - ado-net-data-services
tags:
  - ado-net-data-services
  - csharp
  - xml
  - dotnet
  - aspnet
  - ado-net
  - entity-framework
  - sql-server
  - wpf
  - silverlight
  - xaml
  - http
  - visual-studio
---
# Ado.Net Data Services 1.5 CTP2 - Data Binding Bölüm 1
Merhaba Arkadaşlar,

Ado.Net Data Services ile geliştirilen servislerin tüketilmesi sırasında önem arz eden konulardan biriside, istemci tarafındaki veri bağlama (DataBinding) işlemleridir. Öyleki, servisin tüketicisi olan istemcilerin

- Veriye bağlanılması,
- Bağlanılan verilerin ilgili kontrollerde gösterilmesi,
- Kontroller üzerinden yapılan değişikliklerin aslında bağlanılan Entity içeriklerinde de gerçekleştirilmesi,
- İstemci tarafındaki veri içeriğindeki değişimlerin servis tarafına da gönderilmesi (SaveChanges çağrısı sonrası)

gibi fonksiyonellikleri desteklemesi gerekir. Ancak istmeci tarafında WPF (Windows Presentation Foundation) veya Silverlight gibi zengin içerik sağlayan uygulamalar söz konusu olduğunda, bu modellerin getirdiği veri bağlama kolaylıklarından da yararlanılmalıdır.

Ado.Net Data Services v1.5 ile birlikte istemci tarafına getirilen DataServiceCollection isimli koleksiyonun veri bağlama işlemlerinde kullanılabilmekte olup, CTP2 versiyonunda dahada iyileştirilmiş olarak karşımıza çıkmaktadır. Buna göre istemci tarafı için üretilen kütüphanede (Client Library) kolaylaştırıcı değişiklikler yapıldığı söylenebilir. DataServiceCollection koleksiyonu ObservableCollection tipinden türemekte olup, INotifyPropertyChanged ve INotifyCollectionChanged arayüzlerini (Interface) uygulamaktadır. Aşağıdaki Object Browser çıktısında bu tipin içeriği açık bir şekilde görülmektedir.

![blg82_DataServiceCollection.gif](/assets/images/2009/blg82_DataServiceCollection.gif)

Bu nedenle WPF ve Silverlight tarafında ele alınanveri bağlama (DataBinding) ihtiyaçlarını hem one-way hemde two-way olarak karşılayabilecek bir koleksiyon olarak düşünülmelidir. Buna göre WPF ve Silverlight tarafındaki veri bağlı kontrollerin, DataServiceCollection sınıfından örneklenen koleksiyonları kullanabilmeleri mümkündür. Tahmin edileceği üzere iki yönlü bağlama sayesinde istemci Context'i ile Entity'ler arasındaki iletişimde, değişikliklerin karşılıklı olarak yansıtılabilmesi otomatikleştirilmektedir. Bir DataServiceCollection basit olarak, Ado.Net Data Service tarafına yapılacak bir çağrı ile kolayca doldurulabilir. Özellikle CTP2' de istemci tarafında DataServiceCollection örneklerinin daha güçlü bir şekilde ele alınması için gerekli iyileştirmelerin yapıldığı görülmektedir.

Dilerseniz Ado.Net Data Service hizmetlerinin kullanıldığı senaryolarda, veri bağlama işlemlerinin nasıl yapılacağını örnekler üzerinden incelemeye başlayalım. İlk olarak tek yönlü (One Way) sonrasında ise iki yönlü (Two Way) veri bağlama işlemlerini ele alıyor olacağız. İşe ilk olarak basit bir Asp.Net Web Application projesi oluşturarak başlayabiliriz. Projemizde Ado.Net Entity Framework tabanlı bir veri kaynağı kullanıyor olacağız. İstemci tarafında one-to-many ilişki içerisinde değerlendirilebilecek tipleri ele almak istediğimizden kobay olarak AdventureWorks veritabanındaki ProductSubcategory ve Product tablolarını kullanıyor olacağız.

![Wink](/assets/images/2009/smiley-wink.gif)

Ado.Net Entitiy Diagramımız aşağıda görüldüğü gibidir.

![blg82_Edm.gif](/assets/images/2009/blg82_Edm.gif)

Ado.Net Data Service öğesini projeye ekledikten sonra, ProductionService.svc'ye ait kod içeriğini aşağıdaki gibi düzenleyebiliriz.

```csharp
using System.Data.Services;

namespace AdventureServices
{
    public class ProductionService 
        : DataService<AdventureWorksEntities>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
        
            config.DataServiceBehavior.MaxProtocolVersion = System.Data.Services.Common.DataServiceProtocolVersion.V2;
     }
    }
}
```

Önemli olan noktalarda birisi versiyon olarak CTP2' nin kullanılacağının DataServiceProtocolVersion.V2 ile belirtilmesidir.

Artık istemci tarafını tasarlamaya başlayabiliriz. Ancak öncesinde istemci tarafının Bağlayıcı Arayüzlerini (Binding Interfaces) uygulayabilmesi için komut satırından (Visual Studio 2008 Command Prompt) kod üretici ile ilgili bazı işlemlerin yapılması gerekmektedir. Visual Studio tarafından ele alanın ilgili kod üretim (Code Generation) kütüphanesine bu uygulama işini bildirmek için aşağıdaki komutlarım çalıştırılması yeterli olacaktır.

![blg82_CommandPrompt.gif](/assets/images/2009/blg82_CommandPrompt.gif)

Bu kez gerçekten istemci tarafını yazmaya başlayabiliriz.

![Smile](/assets/images/2009/smiley-smile.gif)

Bu amaçla basit bir WPF uygulaması geliştireceğiz. (Size tavsiyem aynı örneği Silverlight üzerinde geliştirmeye çalışmanız olacaktır.) Uygulamamızı oluşturduktan sonra ilk yapacağımız işlem, Ado.Net Data Service'imize ait URL adresinden gerekli servis referansının, Add Service Reference yardımıyla projeye eklenmesi olacaktır.

![blg82_AddServiceReference.gif](/assets/images/2009/blg82_AddServiceReference.gif)

Görüldüğü üzere, servis üzerinden sunduğumuz Product ve ProductSubcategory Entity içerikleri buraya yansıtılmaktadır. Referansın eklenmesinden sonra istemci tarafında aşağıdaki sınıf diagramında görülen tiplerin oluşturulduğu fark edilebilir.

![blg82_ClassDiagram.gif](/assets/images/2009/blg82_ClassDiagram.gif)

Dikkat edileceği üzere Product ve ProductSubcategory tiplerine INotifyPropertyChanged arayüzü uygulanmıştır. Buna göre söz konusu tiplere ait özelliklerde olacak değişimler, örneklerin bağlandığı ortamlara otomatik olarak bildirilecektir. Elbette tam tersi durumda geçerlidir. Yine servis referansının eklenmesi sonrası, istemci tarafına Microsoft.Data.Services.Client assembly'ınında bildirildiği görülebilir ki bu assembly DataServiceCollection gibi önemli tipleri içermektedir.

![blg82_ClientAsmbly.gif](/assets/images/2009/blg82_ClientAsmbly.gif)

WPF uygulamamızın Window1 içeriğini görsel olarak aşağıdaki gibi tasarladığımızı düşünelim.

![blg82_Window1Design.gif](/assets/images/2009/blg82_Window1Design.gif)

Görsel içeriğimizde yer alan ComboBox bileşenini ProductSubcategory içeriği ile dolduracağız. Diğer yandan alt tarafta görülen GridView kontrolünde, ComboBox bileşeninden seçilen alt kategoriye bağlı ürünlerin (dolayısıyla Product nesne örneklerinin) bazı özellikleri gösterilecektir. Çok doğal olarak ComboBox ve GridView bileşenlerinin, Data Service tarafından gelen içeriğe bağlanmaları gerekmektedir. Üstelik ProductSubcategory ve Product entity'leri arasında bire çok ilişki söz konusu olduğundan, ComboBox kontrolünde bir öğeden diğerine geçildiğinde, buna bağlı ürünlerinde GridView kontrolünde gösterilmesi sağlanmalıdır. Bu durumlar göz önüne alındığında söz konusu XAML içeriğini aşağıdaki gibi tasarlamamız yeterli olacaktır.

```xml
<Window x:Class="ClientApp.Window1"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    Title="Production Space" Height="300" Width="401">
    <Grid x:Name="grdProduction">
        <ComboBox ItemsSource="{Binding}" Height="23" Margin="0,33,0,0" Name="cmbSubCategories" VerticalAlignment="Top" IsSynchronizedWithCurrentItem="True">
            <ComboBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Path=Name}"/>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>
        <Label Height="28" HorizontalAlignment="Left" Margin="0,-1,0,0" Name="label1" VerticalAlignment="Top" Width="136">Product Sub Categories</Label>
        <Label Height="28" HorizontalAlignment="Left" Margin="0,71,0,0" Name="label2" VerticalAlignment="Top" Width="136">Products</Label>
        <ListView ItemsSource="{Binding Product}" Margin="0,96,0,0">
            <ListView.View>
                <GridView>
                    <GridViewColumn Header="Id" DisplayMemberBinding="{Binding Path=ProductID}"/>
                    <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Path=Name}"/>
                    <GridViewColumn Header="List Price" DisplayMemberBinding="{Binding Path=ListPrice}"/>
                    <GridViewColumn Header="Class" DisplayMemberBinding="{Binding Path=Class}"/>
                    <GridViewColumn Header="Number" DisplayMemberBinding="{Binding Path=ProductNumber}"/>
                </GridView>
            </ListView.View>
        </ListView>
    </Grid>
</Window>
```

ComboBox kontrolünün ItemsSource özelliğinin Binding olarak bırakıldığına dikkat edelim. Buna göre grdProduction isimli Grid kontrolünün DataContext kaynağı ne ise, ComboBox kontrolü bu kaynağa otomatik olarak bağlanacak ve her bir öğesinde, Name alanının değerini ({Binding Path=Name} den dolayı) gösterecektir. Dolayısıyla kod tarafında Grid kontrolünün DataContext özelliğine atanan veri kümesi önem arz etmektedir. Diğer yandan ComboBox kontrolünün IsSynchronizedWithCurrentItem niteliğine true değeri atandığına da dikkat edilmelidir. Bu sayede ComboBox içerisinde olan değişiklikler, diğer veri bağlı kontrollerede iletilecektir.

Yani GridView kontrolünün alt kategoriye bağlı ürünler ile doldurulması işleminin gerçekleştirilebilmesi için söz konusu niteliğin true değerine sahip olması gerekmektedir. ListView bileşeninin ItemsSource özelliğine {Binding Product} değeri atanmıştır. Buna göre, ListView içerisindeki veri bağlı kontrollerin Product kaynağına bağlanabileceği belirtilmiş olur ki bu sayede GridView kontrolünün GridViewColumn elementlerinin DisplayMemberBinding niteliklerine Product tipine ait özelliklerin adları atanmıştır. Peki tüm bu veri bağlı kontrollerin baz alacağı veri kaynağı nerede atanacaktır?

Bu amaçla kod tarafında aşağıdaki işlemleri yapmamız yeterli olacaktır.

Window1 Code içeriği;

```csharp
using System;
using System.Data.Services.Client;
using System.Windows;
using ClientApp.ProductionSpace;

namespace ClientApp
{
    public partial class Window1 
        : Window
    {
        AdventureWorksEntities adw = new AdventureWorksEntities(new Uri("http://localhost:1757/ProductionService.svc/"));

        public Window1()
        {
            InitializeComponent();

            grdProduction.DataContext = DataServiceCollection
                .CreateTracked<ProductSubcategory>(adw, adw.ProductSubcategory.Expand("Product"));
        }
    }
}
```

DataServiceContext türevli nesne örneği oluşturulduktan sonra Window1 yapıcı metodu (Constructor) içerisinde DataServiceCollection üzerinden CreateTracked isimli bir çağrı yapıldığı görülmektedir. Bu çağrıda ProductSubcategory tipinden bir nesne kümesinin listesi alınmaktadır. Ayrıca metodun ikinci parametresine dikkat edilecek olursa, her bir ProductSubcategory için Product genişletmesinin yapıldığı, yani alt kategoriye bağlı olan ürünlerinde talep edildiği görülmektedir. Uygulamanın çalıştırılması sonrasında CreateTracked metodunun çağırıldığı yerde Breakpoint ile ilerlenir ve SQL Server Profiler aracından arka planda çalıştırılan sorgu incelenirse aşağıdaki ifadenin yürütüldüğü görülebilir.

```text
SELECT 
[Project1].[ProductSubcategoryID] AS [ProductSubcategoryID], [Project1].[ProductCategoryID] AS [ProductCategoryID], [Project1].[Name] AS [Name], [Project1].[rowguid] AS [rowguid], [Project1].[ModifiedDate] AS [ModifiedDate], [Project1].[C1] AS [C1], [Project1].[C2] AS [C2], [Project1].[ProductID] AS [ProductID], [Project1].[Name1] AS [Name1], [Project1].[ProductNumber] AS [ProductNumber], [Project1].[MakeFlag] AS [MakeFlag], [Project1].[FinishedGoodsFlag] AS [FinishedGoodsFlag], [Project1].[Color] AS [Color], [Project1].[SafetyStockLevel] AS [SafetyStockLevel], [Project1].[ReorderPoint] AS [ReorderPoint], [Project1].[StandardCost] AS [StandardCost], [Project1].[ListPrice] AS [ListPrice], [Project1].[Size] AS [Size], [Project1].[SizeUnitMeasureCode] AS [SizeUnitMeasureCode], [Project1].[WeightUnitMeasureCode] AS [WeightUnitMeasureCode], [Project1].[Weight] AS [Weight], [Project1].[DaysToManufacture] AS [DaysToManufacture], [Project1].[ProductLine] AS [ProductLine], [Project1].[Class] AS [Class], [Project1].[Style] AS [Style], [Project1].[ProductModelID] AS [ProductModelID], [Project1].[SellStartDate] AS [SellStartDate], [Project1].[SellEndDate] AS [SellEndDate], [Project1].[DiscontinuedDate] AS [DiscontinuedDate], [Project1].[rowguid1] AS [rowguid1], [Project1].[ModifiedDate1] AS [ModifiedDate1]
FROM ( SELECT 
 [Extent1].[ProductSubcategoryID] AS [ProductSubcategoryID],  [Extent1].[ProductCategoryID] AS [ProductCategoryID],  [Extent1].[Name] AS [Name],  [Extent1].[rowguid] AS [rowguid],  [Extent1].[ModifiedDate] AS [ModifiedDate],  1 AS [C1],  [Extent2].[ProductID] AS [ProductID],  [Extent2].[Name] AS [Name1],  [Extent2].[ProductNumber] AS [ProductNumber],  [Extent2].[MakeFlag] AS [MakeFlag],  [Extent2].[FinishedGoodsFlag] AS [FinishedGoodsFlag],  [Extent2].[Color] AS [Color],  [Extent2].[SafetyStockLevel] AS [SafetyStockLevel],  [Extent2].[ReorderPoint] AS [ReorderPoint],  [Extent2].[StandardCost] AS [StandardCost],  [Extent2].[ListPrice] AS [ListPrice],  [Extent2].[Size] AS [Size],  [Extent2].[SizeUnitMeasureCode] AS [SizeUnitMeasureCode],  [Extent2].[WeightUnitMeasureCode] AS [WeightUnitMeasureCode],  [Extent2].[Weight] AS [Weight],  [Extent2].[DaysToManufacture] AS [DaysToManufacture],  [Extent2].[ProductLine] AS [ProductLine],  [Extent2].[Class] AS [Class],  [Extent2].[Style] AS [Style],  [Extent2].[ProductModelID] AS [ProductModelID],  [Extent2].[SellStartDate] AS [SellStartDate],  [Extent2].[SellEndDate] AS [SellEndDate],  [Extent2].[DiscontinuedDate] AS [DiscontinuedDate],  [Extent2].[rowguid] AS [rowguid1],  [Extent2].[ModifiedDate] AS [ModifiedDate1], 
 CASE WHEN ([Extent2].[ProductID] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS [C2]
 FROM  [Production].[ProductSubcategory] AS [Extent1]
 LEFT OUTER JOIN [Production].[Product] AS [Extent2] ON [Extent1].[ProductSubcategoryID] = [Extent2].[ProductSubcategoryID]
)  AS [Project1]
ORDER BY [Project1].[ProductSubcategoryID] ASC, [Project1].[C2] ASC
```

Çok fazla alan var değil mi?

![Undecided](/assets/images/2009/smiley-undecided.gif)

Her neyse...Gelelim çalışma zamanındaki duruma. Örneğin Handlebase alt kategorisini seçtiğimizde aşağıdaki şekilde görülen durum oluşmaktadır.

![blg82_Run1.gif](/assets/images/2009/blg82_Run1.gif)

Başka bir alt kategori seçtiğimizde ise (örneğin Bottom Brackets) buna bağlı ürünlerin GridView kontrolüne doldurulduğu görülecektir.

![blg82_Run2.gif](/assets/images/2009/blg82_Run2.gif)

Tabiki burada sadece entity içeriklerinin doldurulması ve veri kontrollerine tek yönlü (One-Way) bağlanması söz konusudur. Ancak tahmin edileceği üzere birde kontroller üzerinden verilerde yapılan değişiklikler sonrası bunların Entity içeriklerine yansıtılması ve sonrasında SaveChanges metodu ile tüm değişikliklerin servis tarafına gönderilmesi söz konusu olabilir ki buda iki yönlü (Two Way) bağlamanın tesis edilmesi ile kolayca gerçekleştirilebilir. Nitekim two-way binding metoduna göre, kolekisyonda olacak değişimler, SaveChanges metoduna yapılan çağrı sonucu servis tarafına ve dolayısıyla sunucu üzerindeki veri kaynağına da iletilecekteir. Bu konuyu bir sonraki yazımızda ele alıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[DataBinding.rar (116,66 kb)](/assets/files/2009/DataBinding.rar)