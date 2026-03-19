---
layout: post
title: "Tek Fotoluk İpucu 77–Asp.Net 4.5 QueryStringAttribute"
date: 2013-01-22 23:55:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - dotnet
  - aspnet
---
# Tek Fotoluk İpucu 77–Asp.Net 4.5 QueryStringAttribute
Merhaba Arkadaşlar,

Asp.Net 4.5 tarafında gelen yeniliklerden birisi de System.Web.ModelBinding isim alanı altında yer alan ve metod parametrelerine uygulanan QueryString niteliğidir (Attribute). Bu nitelik ile bir metodun parametre değerinin, URL Querystring üzerinden okunabileceği ifade edilmektedir.

Özellikle veri bağlı kontrollerin (GridView gibi), IQueryable benzeri referanslar döndüren metodlar ile ilişkilendirildikleri senaryolarda, query string yardımıyla filtreleme yapılmasında kullanılır. Aynen aşağıdaki fotoğrafta görüldüğü gibi

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_131.png)

[![tfi_77](/assets/images/2013/tfi_77_thumb.png)](/assets/images/2013/tfi_77.png)

Bir başka ipucunda görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_131.png)