---
title: "Tek Fotoluk İpucu 67.75–Asp.Net 4.5 ControlAttribute"
pubDate: 2012-10-02 02:00:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - .NET
  - ASP.NET
---

# Tek Fotoluk İpucu 67.75–Asp.Net 4.5 ControlAttribute
Merhaba Arkadaşlar,

Asp.Net 4.5 ile gelen önemli tiplerden birisi de, System.Web.ModelBinding isim alanı (System.Web.dll assembly’ ı içerisindedir) altında yer alan ControlAttribute niteliğidir (Attribute). Metod parametrelerine uygulanabilen bu nitelik ile, veri bağlı kontrollerin (GridView gibi) filtre bazlı çalıştığı senaryolarda, filtreleme kriterinin/kriterlerinin nereden alınacağı, kod seviyesinde kolayca belirtilebilir. Aşağıdaki fotoğrafta görülen örnekte, albümlerin sorgulanmasında kullanılan ArtistId değerinin bir DropDownList öğesinden çekileceği, GetAlbums metodu içerisindeki Control niteliği yardımıyla ifade edilmiştir

![Winking smile](images/wlEmoticon-winkingsmile_132.png)

[![tfi_67_75](images/tfi_67_75_thumb.png)](images/tfi_67_75.png)

Bir başka ipucunda görüşmek dileğiyle

![Winking smile](images/wlEmoticon-winkingsmile_132.png)
