---
title: "Tek Fotoluk İpucu 70.5–Asp.Net Multiple File Upload"
pubDate: 2012-11-13 08:57:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - .NET
  - ASP.NET
---

# Tek Fotoluk İpucu 70.5–Asp.Net Multiple File Upload
Merhaba Arkadaşlar,

Asp.Net 4.5 ile FileUpload kontrolüne gelen iki önemli özellik (Property) mevcuttur. Bunlardan birisi AllowMultiple, diğeri ise PostedFiles’ dır. Bu iki özelliği kullanarak birden fazla dosyanın, istemciden sunucu tarafına yüklenme işlemlerini (Multiple Upload Files) kolayca ele alabilirsiniz. Nasıl mı? Buyrun

![Winking smile](images/wlEmoticon-winkingsmile_151.png)

[![tfi_70dot5](images/tfi_70dot5_thumb.png)](images/tfi_70dot5.png)

İşin özünde AllowMultiple özelliğine atanan true değeri (ki varsayılan olarak false dur) yatmaktadır. Bu sayede, istemci tarafında açılan pencerede birden fazla dosyasnın seçilmesine izin verilmektedir. Dolayısıyla PostedFiles özelliği HttpPostedFile tipinden birden fazla örnek ile dolar ve sunucu tarafında istenilen şekilde değerlendirilebilir.

Bir başka ipucunda görüşmek dileğiyle

![Winking smile](images/wlEmoticon-winkingsmile_151.png)
