---
layout: post
title: "Tek Fotoluk İpucu 83–XML, XAML, XmlDataProvider ve Master Child Binding"
date: 2013-03-25 20:35:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - xml
  - xaml
  - windows-presentation-foundation
  - xmldataprovider
  - binding
  - x-path
  - master-detail
  - listbox
  - itemssource
  - datatemplate
  - itemtemplate
  - issychronizedwithcurrentitem
---
Diyelim ki elinizde aşağıdaki gibi Master-Child veri ilişkisi içeren (1 gruba bağlı birden fazla albüm) bir XML dosyası var.

```xml
<?xml version="1.0" encoding="utf-8"?> 
<Depo> 
  <Group GroupId="1" Name="ACDC"> 
    <Album AlbumId="1" Name="Back in black"/> 
    <Album AlbumId="2" Name="Black ice"/> 
    <Album AlbumId="3" Name="The Razor's Edge"/> 
    <Album AlbumId="4" Name="Black ice"/> 
  </Group> 
  <Group GroupId="2" Name="Aerosmith"> 
    <Album AlbumId="5" Name="O Yeah! Ultimate Aerosmith"/> 
  </Group> 
  <Group GroupId="3" Name="The Darkness"> 
    <Album AlbumId="6" Name="One way ticket to hell"/> 
    <Album AlbumId="7" Name="Permission to land"/> 
  </Group> 
</Depo>
```

ve sizde örneğin WPF-XAML tarafında buradaki Master-Detail ilişkiyi kullanmak ve hatta iki veri bağlı kontrol üzerinden sembolize etmek istiyorsunuz. Ne yaparsınız? Belki de aşağıdaki fotoğrafta görülen tekniği kullanırsınız

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_155.png)

[![tfi_83](/assets/images/2013/tfi_83_thumb.png)](/assets/images/2013/tfi_83.png)

Bir başka ipucunda görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_155.png)