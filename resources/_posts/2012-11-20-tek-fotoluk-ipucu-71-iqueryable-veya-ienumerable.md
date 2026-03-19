---
layout: post
title: "Tek Fotoluk İpucu–71–IQueryable veya IEnumerable"
date: 2012-11-20 03:01:00 +0300
description: "Bu sefer ki ip ucumuz biraz daha kışkırtıcı aslında. Aşağıdaki fotoğrafı bir inceleyin öncelikle ve nasıl bir fark olabileceğini düşünmeye çalışın. Yani kafanızda kod parçasını debug etmeye gayret edin. (Visual Studio ve benzeri herhangibir geliştirme aracı kullanmamanız şiddetle tavsiye edilir)"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - sql-server
  - visual-studio
---
# Tek Fotoluk İpucu–71–IQueryable veya IEnumerable
Merhaba Arkadaşlar,

Bu sefer ki ip ucumuz biraz daha kışkırtıcı aslında. Aşağıdaki fotoğrafı bir inceleyin öncelikle ve nasıl bir fark olabileceğini düşünmeye çalışın. Yani kafanızda kod parçasını debug etmeye gayret edin.

(Visual Studio ve benzeri herhangibir geliştirme aracı kullanmamanız şiddetle tavsiye edilir

![Yell](/assets/images/2012/smiley-yell.gif)

)

[![tfi_71](/assets/images/2012/tfi_71_thumb.png)](/assets/images/2012/tfi_71.png)

Tabi fotoğrafa bakınca durumu görmek zor olabilir. Ama fotoğrafın arkasında yatan gerçeklere bakarsak (örneğin SQL Server Profiler yardımıyla) customerList1 üzerinden uygulanan Take (10) çağrısı için aşağıdaki SQL sorgusunun çalıştırıldığını görürüz.

```text
SELECT TOP (10) 
[Extent1].[CustomerId] AS [CustomerId], 
[Extent1].[FirstName] AS [FirstName], 
[Extent1].[LastName] AS [LastName], 
[Extent1].[Company] AS [Company], 
[Extent1].[Address] AS [Address], 
[Extent1].[City] AS [City], 
[Extent1].[State] AS [State], 
[Extent1].[Country] AS [Country], 
[Extent1].[PostalCode] AS [PostalCode], 
[Extent1].[Phone] AS [Phone], 
[Extent1].[Fax] AS [Fax], 
[Extent1].[Email] AS [Email], 
[Extent1].[SupportRepId] AS [SupportRepId] 
FROM [dbo].[Customer] AS [Extent1] 
ORDER BY [Extent1].[LastName] ASC, [Extent1].[FirstName] ASC
```

customerList2 üzerinden yapılan Take (10) içinse benzer bir sorgu üretilir.

```text
SELECT 
[Extent1].[CustomerId] AS [CustomerId], 
[Extent1].[FirstName] AS [FirstName], 
[Extent1].[LastName] AS [LastName], 
[Extent1].[Company] AS [Company], 
[Extent1].[Address] AS [Address], 
[Extent1].[City] AS [City], 
[Extent1].[State] AS [State], 
[Extent1].[Country] AS [Country], 
[Extent1].[PostalCode] AS [PostalCode], 
[Extent1].[Phone] AS [Phone], 
[Extent1].[Fax] AS [Fax], 
[Extent1].[Email] AS [Email], 
[Extent1].[SupportRepId] AS [SupportRepId] 
FROM [dbo].[Customer] AS [Extent1] 
ORDER BY [Extent1].[LastName] ASC, [Extent1].[FirstName] ASC
```

![Surprised smile](/assets/images/2012/wlEmoticon-surprisedsmile_2.png)

Uppsss!!! Benzer bir sorgu derken!

IQueryable üzerinden yapılan Take (10) çağrısı dikkat edileceği üzere TOP 10 ifadesini kullanmıştır. Peki ya IEnumerable üzerinden yapılan Take (10) çağrısı ne yapmıştır

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_118.png)

Aslında tüm liste çekilmiş sonrasında Take metodu, belleğe aldığı koleksiyon seti üzerinden ilk 10luk parçayı almıştır.

Sanırım artık IQueryable mı olsun, IEnumerable mı olsun çıktı sonucu ya da var anahtar kelimesini kullanırsak hangisini göz önüne alır, hangisi daha avantajlıdır diye bir kuşku oluşturmuş bulunmaktayım içinizde

![Smile](/assets/images/2012/wlEmoticon-smile_50.png)

E hadi hayırlısı diyelim.

Başka bir ipucunda görüşmek dileğiyle.