---
layout: post
title: "Tek Fotoluk İpucu 94–WMI ile Disk Bilgilerini Okumak"
date: 2013-03-25 21:22:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - http
---
WMI (Windows Management Instrumentation) maceralarımıza devam etmeye ne dersiniz? Eğer biraz daha kasarsak, geniş bir WMI kütüphanesi bile oluşturabiliriz. Bu fotoğrafımıza konu olan güncel senaryomuz ise şöyle; İşletim sistemi tarafından Map edilmiş Disk bilgilerine nasıl ulaşabiliriz? Sadece Hard Disk’ ler değil. Bağlı olduğumuz Network Driver’ ları da öğrenmek istediğimizi varsalayım

![Who me?](/assets/images/2013/wlEmoticon-whome_8.png)

Dilerseniz önce WMI tarafında önceki ipuçlarımızdan da yararlanarak senaryoyu gerçekleştirmeye çalışın. Sonrasında fotoğrafımıza bakarsınız.

[![tfi_94](/assets/images/2013/tfi_94_thumb.png)](/assets/images/2013/tfi_94.png)

Tabi Win32LogicalDisk tipinin kullanılabilecek farklı özellikleri de mevcut. Bu özelliklere de [şu adresten](http://msdn.microsoft.com/en-us/library/windows/desktop/aa394173(v=vs.85).aspx) bakabilirsiniz. Bir başka ipucundan görüşmek dileğiyle

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_182.png)
