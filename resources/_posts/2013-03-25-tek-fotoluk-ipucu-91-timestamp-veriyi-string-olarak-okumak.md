---
layout: post
title: "Tek Fotoluk İpucu 91–Timestamp Veriyi String Olarak Okumak"
date: 2013-03-25 21:15:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - sql-server
---
Diyelim ki SQL Server üzerinde duran tablolarda timestamp veri tipinden alanlar bulunmakta ve siz bu alanları belki bir Backoffice uygulamasında belki bir admin panelde, kullanıcalara göstermek istiyorsunuz. Normal şartlarda bilindiği üzere bu alan bir byte[] array olarak elde edilmektedir. Dolayısıyla timestamp içeriği taşıyan bu byte[] array’ in anlamlı bir string tipine dönüştürülmesi okunurluğu açısından şarttır. Ne yaparsınız? Belki basit bir extension method’ u bu amaçla projeye dahil edebilirsiniz. Aynen aşağıda görüldüğü gibi.

[![tfi_91](/assets/images/2013/tfi_91_thumb.png)](/assets/images/2013/tfi_91.png)

Bir başka ipucunda görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_173.png)