---
layout: post
title: "Tek Fotoluk İpucu 87–Enum Sabitleri ile Attribute Kullanımı"
date: 2013-03-25 20:55:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
---
# Tek Fotoluk İpucu 87–Enum Sabitleri ile Attribute Kullanımı
Merhaba Arkadaşlar,

Bazen uygulamalarımızda kullandığımız Enum sabitlerinin içerikleri anlaşılabilir kelimelerden oluşmayabilir. Örneğin AR101,BR103 isimli Enum değerleri çalışma zamanında nasıl yorumlanabilirler. Mantıklı isimler verebilecek olsak da bazen bu değerler sistemin kendisi için anlamlı olabilirler. Ancak dilerseniz enum sabiti değerlerini, kendi geliştireceğiniz nitelikler (Attribute) ile donatabilir ve çalışma zamanına ek bilgiler sunabilirsiniz. Hem de basit bir extension metod yardımıyla bu fonksiyonelliğin kolayca ulaşılabilir olmasını sağlayabilirsiniz. Nasıl mı? İşte size bir örnek

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_166.png)

[![tfii_87](/assets/images/2013/tfii_87_thumb.png)](/assets/images/2013/tfii_87.png)

Bir başka ipucunda görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_166.png)