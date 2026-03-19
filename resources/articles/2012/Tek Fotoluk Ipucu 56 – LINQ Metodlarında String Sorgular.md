---
title: "Tek Fotoluk Ipucu 56 – LINQ Metodlarında String Sorgular"
pubDate: 2012-07-08 19:45:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - Entity Framework
  - LINQ
---

# Tek Fotoluk Ipucu 56 – LINQ Metodlarında String Sorgular
Merhaba Arkadaşlar,

Bazı durumlarda Entity Framework tabanlı nesne koleksiyonlarını sorgularken, Extension Method’ lar içerisine gelecek olan sorgulama ifadelerinin string bazlı olarak gelmesi söz konusu olabilir. Örneğin servis metodlarının istemci tarafından parametre olarak bu tip sorgu ifadeleri aldığı sıklıkla görülmektedir. Peki ama nasıl? Bunun bir örneği var mıdır? Hani elimizin altında dursa ve bir fikir verse iyi olmaz mı?

![Winking smile](images/wlEmoticon-winkingsmile_88.png)

Buyrun öyleyse.

[![tfi_57](images/tfi_57_thumb.png)](images/tfi_57.png)

Bu koda göre arka planda hareket eden SQL sorgusu da şöyledir.

> SELECT
> 1 AS [C1],
> [Extent1].[Name] AS [Name],
> [Extent1].[Class] AS [Class],
> [Extent1].[ListPrice] AS [ListPrice]
> FROM [Production].[Product] AS [Extent1]
> WHERE ([Extent1].[Name] LIKE 'M%') AND ([Extent1].[ListPrice] >= 3000)

Bir diğer ipucunda görüşmek dileğiyle.
