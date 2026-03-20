---
layout: post
title: "Tek Fotoluk İpucu-28(Bir Klasörün Yaklaşık Toplam Boyutunu Bulmak)"
date: 2011-08-24 00:09:00 +0300
categories:
  - csharp
  - tek-fotoluk-ipucu
tags:
  - csharp
  - tek-fotoluk-ipucu
---
Bir klasörün tüm içeriğinin toplam boyutunu öğrenmek isteyebiliriz. Bunun için DirectoryInfo tipine bir ExtensionMethod eklersek de güzel olur. Hatta bu metodun alt klasörleri de gezebilmesi için Recursive olarak yazılması da gerekir. Nasıl mı?

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_46.png)

Not: Yanlız erişim yetkisi olmayan klasörler söz konusu olduğunda boyut bilgisi eksik çıkacaktır. Bunun çözümünü de size bırakıyorum. Biraz araştırın bakalım ![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_46.png)

[![PhotoTrick28](/assets/images/2011/PhotoTrick28_thumb.png)](/assets/images/2011/PhotoTrick28.png)

[DirectoryExtensions.rar (23,28 kb)](/assets/files/2011/DirectoryExtensions.rar)
