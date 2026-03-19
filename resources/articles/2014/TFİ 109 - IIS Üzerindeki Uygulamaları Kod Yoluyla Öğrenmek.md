---
title: "TFİ 109 - IIS Üzerindeki Uygulamaları Kod Yoluyla Öğrenmek"
pubDate: 2014-08-27 13:52:00
categories:
  - Asp.Net
  - Asp.Net 2.0
  - Asp.Net 4.0 Beta 2
  - Asp.Net 4.5
  - Asp.Net Web API
tags:
  - Asp.Net
  - Asp.Net 2.0
  - Asp.Net 4.0 Beta 2
  - Asp.Net 4.5
  - Asp.Net Web API
  - Windows Forms
  - IIS
---

# TFİ 109 - IIS Üzerindeki Uygulamaları Kod Yoluyla Öğrenmek
Merhaba Arkadaşlar,

Diyelim ki sunucudaki IIS üzerinde konuşlandırdığınız Web uygulamalarının bir listesini almak istiyorsunuz. Bunun elbette pek çok yolu olduğunu biliyorsunuz. Bir Powershell script'i belki de işinizi görür. Ancak belki de siz bunu kendi geliştireceğiniz windows forms uygulamasında bu listeyi kullanmak istiyorsunuz. Ne yaparsınız? Kod yardımıyla IIS üzerindeki Application'ları, Site'ları öğrenebilir misiniz?

Aslında hep elinizin altında olan (Windows\System32\inetsrv\Microsoft.Web.Administration.dll) ve hatta isterseniz NuGet Package Manager ile de indirebileceğiniz Microsoft.Web.Administration kütüphanesini kullanarak bu işi gerçekleştirmeniz oldukça kolay. Nasıl mı? İşte böyle.

![tfi109.png](images/tfi109.png)

Başka neler mi yapabilirsiniz? Örneğin bir Application Pool'u Recylce edebilirsiniz. Ya da bir Web Site'ı Stop-Start. Hatta yeni bir Web Site bile açabilirsiniz. Araştırmaya değer değil mi?

Başka bir ipucunda görüşmek dileğiyle.
