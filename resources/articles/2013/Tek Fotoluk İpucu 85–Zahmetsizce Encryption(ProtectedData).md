---
title: "Tek Fotoluk İpucu 85–Zahmetsizce Encryption(ProtectedData)"
pubDate: 2013-03-25 20:46:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - .NET
---

# Tek Fotoluk İpucu 85–Zahmetsizce Encryption(ProtectedData)
Merhaba Arkadaşlar,

Cryptography denilince.Net Framework tarafında epey bir çözüm bulunmakta. Bazıları oldukça karmaşıktır ve simetrik yada a-simetrik olmalarına bağlı olaraktan, ortak noktalarından birisi de, tekniğe göre kullanılan Vector-Key değerlerinin tutulması/bilinmesi gibi zorunluluklardır.

Aslında Windows tarafında, XP işletim sisteminden beri var olan (hatta Windows 8 de bunun Cloud destekli bir versiyonu da vardır-> DPAPI-NG) bir API mevcut. DPAPI. Nam-ı diğer Data Protection API.

Dilersek bu API’ yi kullanarak verilerimizi şifreleyebilir ve güvenliklerini sağlayabiliriz. Bunu yaparken CurrentUser veya LocalMachine seçeneklerini belirterek şifreyi kimin açabileceğini kolayca ifade edebiliriz.

Oldukça kolay bir biçimde; herhangibir algoritmaya bağımlı kalmadan, vector-key saklamadan, üstelikte karmaşıklığı basitçe arttırabileceğimiz (entropy değerlerini değiştirerek deneyin) bir formatta…Nasıl mı? Buyrun

![Winking smile](images/wlEmoticon-winkingsmile_159.png)

[![tfi_85](images/tfi_85_thumb.png)](images/tfi_85.png)

Burada ki ipucundan Data Protection kullanımı söz konusudur. Aslında aynı API’ yi kullanarak bir de Memory Protection yapabiliriz. Buna da bir sonraki ipucumuzda bakalım

![Winking smile](images/wlEmoticon-winkingsmile_159.png)
