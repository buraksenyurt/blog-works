---
layout: post
title: "TFİ 109 - IIS Üzerindeki Uygulamaları Kod Yoluyla Öğrenmek"
date: 2014-08-27 13:52:00 +0300
description: "Merhaba Arkadaşlar,"
categories:
  - aspnet
  - aspnet-2-0
  - aspnet-4-0-beta-2
  - aspnet-4-5
  - aspnet-web-api
tags:
  - aspnet
  - aspnet-2-0
  - aspnet-4-0-beta-2
  - aspnet-4-5
  - aspnet-web-api
  - windows-forms
  - iis
---
# TFİ 109 - IIS Üzerindeki Uygulamaları Kod Yoluyla Öğrenmek
Merhaba Arkadaşlar,

Diyelim ki sunucudaki IIS üzerinde konuşlandırdığınız Web uygulamalarının bir listesini almak istiyorsunuz. Bunun elbette pek çok yolu olduğunu biliyorsunuz. Bir Powershell script'i belki de işinizi görür. Ancak belki de siz bunu kendi geliştireceğiniz windows forms uygulamasında bu listeyi kullanmak istiyorsunuz. Ne yaparsınız? Kod yardımıyla IIS üzerindeki Application'ları, Site'ları öğrenebilir misiniz?

Aslında hep elinizin altında olan (Windows\System32\inetsrv\Microsoft.Web.Administration.dll) ve hatta isterseniz NuGet Package Manager ile de indirebileceğiniz Microsoft.Web.Administration kütüphanesini kullanarak bu işi gerçekleştirmeniz oldukça kolay. Nasıl mı? İşte böyle.

![tfi109.png](/assets/images/2014/tfi109.png)

Başka neler mi yapabilirsiniz? Örneğin bir Application Pool'u Recylce edebilirsiniz. Ya da bir Web Site'ı Stop-Start. Hatta yeni bir Web Site bile açabilirsiniz. Araştırmaya değer değil mi?

Başka bir ipucunda görüşmek dileğiyle.