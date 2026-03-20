---
layout: post
title: "Tek Fotoluk İpucu 67.75–Asp.Net 4.5 ControlAttribute"
date: 2012-10-02 02:00:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - dotnet
  - aspnet
---
Asp.Net 4.5 ile gelen önemli tiplerden birisi de, System.Web.ModelBinding isim alanı (System.Web.dll assembly’ ı içerisindedir) altında yer alan ControlAttribute niteliğidir (Attribute). Metod parametrelerine uygulanabilen bu nitelik ile, veri bağlı kontrollerin (GridView gibi) filtre bazlı çalıştığı senaryolarda, filtreleme kriterinin/kriterlerinin nereden alınacağı, kod seviyesinde kolayca belirtilebilir. Aşağıdaki fotoğrafta görülen örnekte, albümlerin sorgulanmasında kullanılan ArtistId değerinin bir DropDownList öğesinden çekileceği, GetAlbums metodu içerisindeki Control niteliği yardımıyla ifade edilmiştir

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_132.png)

[![tfi_67_75](/assets/images/2012/tfi_67_75_thumb.png)](/assets/images/2012/tfi_67_75.png)

Bir başka ipucunda görüşmek dileğiyle

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_132.png)
