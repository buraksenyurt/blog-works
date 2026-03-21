---
layout: post
title: "Tek Fotoluk İpucu 78 - Asp.Net 4.5 ile HtmlEncode"
date: 2013-02-03 17:07:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - html
  - asp.net
  - data-binding
  - expression
---
Bazı durumlarda Asp.Net sayfasının çıktısına basacağımız içeriğin HTML formatlı elementlerinin Text tabanlı görünümleri olmasını isteriz. Örneğin takısının, uygulandığı metni bold olarak göstermesini istemeyiz. Bunun yerine yazı şeklinde düz metin olarak gösterilmesini arzu ederiz (Hatta bazı blogların yorum kısımlarında, yorumda kullanılabilecek HTML Tag'leri ifade edilir. Ama metin olarak basılmışlardır) Bunun için Asp.Net 4.5 tarafında işimizi oldukça kolaylaştıracak bir özellik yer almakta. İki nokta üst üste işaretini kullanmamız HTML içeriğinin metinsel olarak kullanılmasında yeterli oluyor. Nerede mi? Özellikle Veri bağlama (Data Binding) noktalarında

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_133.png)

Örneğin,

[![tfi_78](/assets/images/2013/tfi_78_thumb.png)](/assets/images/2013/tfi_78.png)

Bir başka ip ucunda görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_133.png)
