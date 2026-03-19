---
title: "Tek Fotoluk İpucu 97–Google Shortener URL Hizmetini C# ile Kullanmak"
pubDate: 2013-03-25 21:30:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - JSON
  - HTTP
---

# Tek Fotoluk İpucu 97–Google Shortener URL Hizmetini C# ile Kullanmak
Merhaba Arkadaşlar,

Malumunuz bazen Web adreslerine ait URL satırları epeyce uzun olabiliyorlar ve bunları saklamak gibi amaçlarla kullanmak istediğimizde, genellikle kısaltma yoluna gitmeyi tercih ediyoruz (Sanırım kimse 20 haneye sığdırılabilecek 200 karakterlik bir URL bilgisi ile uğraşmak istemez) Bir URL adresini kısaltmak için kullanılabilecek pek çok global hizmet bulunmakta. Bunlardan birisi de Google’ ın Shortener servisi (ki [bu adresten de görebileceğiniz](http://goo.gl/) gibi kendisi de epeyce kısa ![Smile](images/wlEmoticon-smile_92.png)). Peki bir tarayıcı ile bu söz konusu servise kolayca gönderebildiğimiz bir talebi kod tarafında C# ile gerçekleştirmek isteseydiniz nasıl bir yol izlersiniz? Aşağıdaki gibi olabilir mi?

[![tfi97](images/tfi97_thumb.png)](images/tfi97.png)

Örnekten de görüleceği üzere [https://www.googleapis.com/urlshortener/v1/url](https://www.googleapis.com/urlshortener/v1/url) adresine JSON formatında bir talep gönderilmekte olup, gelen cevap içerisinden id niteliğinin değeri yakalanmaktadır. Üstelik bu işlem sırasında NewtonSoft’ un ilgili NuGet paketinden yararlanılmış olup söz konusu fonksiyonellik, Uri sınıfı için bir Extension Method olarak tanımlanmıştır. (NewtonSoft ile ilişkili olarak [şuradaki](http://www.buraksenyurt.com/post/Tek-Fotoluk-Ipucu-69-Newtonsoft-JSONNet-ve-dynamic-Keyword) ve [buradaki](http://www.buraksenyurt.com/post/Tek-Fotoluk-Ipucu-70-Newtonsoft-Jsonnet-and-dynamic-and-parsing.aspx) ipuçlarına bakabilirsiniz) Bir başka ip ucunda görüşmek dileğiyle

![Winking smile](images/wlEmoticon-winkingsmile_191.png)

[Örneği denerken Google’ ın Shortener servisinin web adresini kontrol etmenizi öneririm. Yazının yazıldığı tarihe rağmen, yayınlandığı tarihte değişmiş, hatta kaldırılmış dahi olabilir]
