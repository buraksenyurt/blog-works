---
layout: post
title: "Tek Fotoluk İpucu 86–Zahmetsizce Encryption (ProtectedMemory)"
date: 2013-03-25 20:50:00 +0300
categories:
  - tek-fotoluk-ipucu
tags:
  - tek-fotoluk-ipucu
  - cryptography
  - data-protection
  - dpapi
  - windows-protection
  - current-user
  - local-machine
  - security
  - encryption
  - decryption
  - şifreleme
  - çözümleme
  - memory-protection
---
[Bir önceki tek fotoluk ipucunda ProtectedData sınıfından yararlanmış](http://www.buraksenyurt.com/post/Tek-Fotoluk-Ipucu-85-Zahmetsizce-Encryption.aspx) ve basitçe bir byte dizisinin nasıl şifrelenebileceğini/çözümlenebileceğini görmüştük. Hatırlarsanız veriyi Current User ve Local Machine seviylerinde ele alabiliyorduk. DPAPI’ nin kullanıldığı veri odaklı bu tekniğin yanında, bellek üzerinde yer alan bir içeriğin Process bazında şifrelenmesi/çözümlenmesi de mümkündür. Aynı Process (SameProcess), farklı Process (CrossProcess) veya aynı giriş (SameLogon) bilgisi için…Üstelik son derece de kolay bir şekilde. Nasıl mı?

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_160.png)

[![tfi_86](/assets/images/2013/tfi_86_thumb.png)](/assets/images/2013/tfi_86.png)

Bir başka ipucunda görüşmek dileğiyle

![Smile](/assets/images/2013/wlEmoticon-smile_71.png)
