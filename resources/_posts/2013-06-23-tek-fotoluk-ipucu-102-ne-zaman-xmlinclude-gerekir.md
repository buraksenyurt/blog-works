---
layout: post
title: "Tek Fotoluk İpucu 102–Ne Zaman XmlInclude Gerekir?"
date: 2013-06-23 11:00:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - xml
---
# Tek Fotoluk İpucu 102–Ne Zaman XmlInclude Gerekir?
Merhaba Arkadaşlar,

Diyelim ki elinizde Role isimli bir sınıf var. Hatta bu sınıftan türemiş Manager ve Worker isimli iki ayrı sınıf daha var. Hatta Role tipinden Employees isimli bir listeyi özellik (Property) olarak içeren Company isimli başka bir sınıf daha var…Derken Company sınıfına ait bir nesne örneğini çalışma zamanında XML serileştirmek istediniz. Klasik olarak XmlSerializer tipini işin içerisine kattınız. Peki ya sonra? Aldınız InvalidOperationException hatasını oturdunuz aşağıya

![Who me?](/assets/images/2013/wlEmoticon-whome_11.png)

Nasıl çözersiniz? Aşağıdaki gibi olabilir mi?

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_208.png)

[![tfi_102](/assets/images/2013/tfi_102_thumb.png)](/assets/images/2013/tfi_102.png)

Aslında yapılan işlem gayet basittir. Role tipinin Manager ve Worker isimli sınıflara ait nesne örneklerini içerebileceği XmlInclude niteliği (Attribute) yardımıyla ifade edilmiştir. Hepsi bu. Bir başka ipucunda görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_208.png)