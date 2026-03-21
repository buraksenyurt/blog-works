---
layout: post
title: "Tek Fotoluk İpucu–70–Yine Newtonsoft Json.net ve dynamic"
date: 2012-11-12 02:30:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - dynamic
  - csharp
  - json
  - serialization
  - newtonsoft
  - jobject
  - jarray
---
Diyelim ki elimizde aşağıdaki gibi bir JSON içeriği var.

```json
{ 
  "categoryName": "Objektif", 
  "description": "Her çeşit DSLR lensi", 
  "productCount": 4, 
  "id": "bb913579-ac93-4398-a77d-dd07db825df8", 
  "products": [ 
    { 
      "name": "Canon 50mm f/1.8 AF/IS", 
      "listPrice": 250.0 
    }, 
    { 
      "name": "Canon 17-50mm f/2.8 AF/IS", 
      "listPrice": 1600.5 
    }, 
    { 
      "name": "Tamron 24-70mm f/3.5 AF", 
      "listPrice": 350.0 
    }, 
    { 
      "name": "Canon 100mm f/2.8 IS AF", 
      "listPrice": 1456.85 
    } 
  ] 
}
```

ve hatta NuGet ile eklediğimiz Newtonsoft'un JSON.net kütüphanesi.

Ha bir de dynamic keyword'ümüz var.

O halde bu doküman içeriğini okumamız ne kadar zor olabilir ki?

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_113.png)

[![tfi_70](/assets/images/2012/tfi_70_thumb.png)](/assets/images/2012/tfi_70.png)

Başka bir ipucunda görüşmek dileğiyle

![Smile](/assets/images/2012/wlEmoticon-smile_45.png)