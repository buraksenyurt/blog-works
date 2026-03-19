---
title: "TFS 32Bit Uygulama Hatası (Bir Garip Geliştiricinin Haykırışı)"
pubDate: 2013-01-01 15:52:00
categories:
  - Team Foundation Server
tags:
  - Team Foundation Server
  - Oracle
  - WCF
  - HTTP
  - IIS
---

# TFS 32Bit Uygulama Hatası (Bir Garip Geliştiricinin Haykırışı)
[![big-mistake](images/big-mistake_thumb.jpg)](images/big-mistake.jpg) Merhaba Arkadaşlar,

Genelde bu kadar kısa yazılar pek yazmıyorum. En fazla Tek Fotolok İpucu serisi altında paylaşım yapmaktayım. Ancak karşılaştığım ilginç bir durumu da sizinle paylaşmak istedim. Tabi olayın başrol oyuncusu olarak en büyük kabahat bende

![Smile](images/wlEmoticon-smile_86.png)

Öyleyse haydi buyrun bakalım hiyayemize…

Biliyorsunuz TFS kurduğunuzda IIS alına bir Team Foundation Server isimli bir Web Site oluşturulmakta (Web Access arayüzü buradaki tfs klasörü altında duruyor ve hatta TFS servisleri de yine buradaki TeamProjectServices uygulaması içerisinde yer almakta)

Web Site’ ın en belirgin özelliği ise Microsoft Team Foundation Server Application Pool isimli bir havuzu kullanıyor olması. Bu havuzun özelliklerine genellikle pek dokunmuyoruz ama ben bir test sırasında dokundum ve bakın neler oldu. Lafı fazla uzatmadan hemen senaryoya geçeyim dilerseniz

![Winking smile](images/wlEmoticon-winkingsmile_183.png)

O gün elimde geliştirmelerini yeni bitirdiğim ve yerel makinede test ettiğim bir WCF Servis uygulaması vardı ve ağda ilk bulabildiğim sunucu üzerinde de test etmek istiyordum. Erişim hakkım olan ve üzerinde TFS yüklü makinemi gözüme kestirdim. Bu arada söz konusu WCF servis uygulaması içerisinde, sadece 32bit uyumlu olan bir Assembly da kullanmaktaydım (Oracle.DataAccess.dll).

Hemen bir deploy paketi oluşturdum ve servis uygulamasını IIS üzerinde bir Web Site’ a atmak istedim. Ancak Default Web Site, Sharepoint’ in 80 numaralı portu hakimiyeti altına alması nedeniyle kaput durumdaydı ve hiç hayat belirtisi vermiyordu. En uygun yer Team Foundation Server isimli Web Site idi. Ben de bunun üzerine paketi alıp Team Foundation Server sitesi altında bir Application haline dönüştürdüm.

Ancak servise erişmek istediğimde Oracle.DataAcesss.dll ile ilişkili bir hata aldım.

[![tfserror_3](images/tfserror_3_thumb.png)](images/tfserror_3.png)

Hatanın sebebi belirgindi. Makine üzerindeki işletim sistemi 64bit olarak yüklenmişti ve IIS’ de bu şekilde yürütülmekteydi (Zaten TFS 2012’ de 64bit işletim sistemi üzerine kurulmaktadır. [Detaylar için bu adresteki yazıya bakabilirsiniz](http://msdn.microsoft.com/en-us/library/vstudio/dd578592.aspx)) Dolayısıyla Microsoft Team Foundation Server Application Pool’ un 32bit yazılmış assembly’ ları yükleyebilmesi bu senaryo için gerekiyordu. Ben de ilgili Pool’ un Advanced Settings kısmına giderek Enable 32-Bit Applications değerini true olarak değiştirdim. Bu masumane davranışın nasıl kötü bir sonucu olabilirdi ki

![Smile](images/wlEmoticon-smile_86.png)

[![tfserror_2](images/tfserror_2_thumb.png)](images/tfserror_2.png)

Hay değiştirmez olaydım

![Open-mouthed smile](images/wlEmoticon-openmouthedsmile_42.png)

Artık servis çalışıyordu bunu görebiliyordum ama…Kendi kendimi bir sonraki adıma geçmeyi planladığım sırada bir telefon sesi duydum. Takım arkadaşlarımdan birisi TFS bağlantısı sırasında bir hata aldığını söylüyordu. Hemen TFS adresine girdim ve ben de hatayı gördüm

![Disappointed smile](images/wlEmoticon-disappointedsmile_3.png)

[![tfserror](images/tfserror_thumb.png)](images/tfserror.png)

Bir anda ortalık karıştı tabi. Telefonlar ardı ardına geliyor, ter damlaları heryerden boşalıyordu. TFS ile çalışan çok fazla ekip vardı. Ürün lisanslıydı. Bunu biliyorduk. Emin olmak için IT departmanımız ile görüştük. Doğruladılar. Lisans numaralarını kontrol ettik vs…

Sonunda oluşan hatanın sebebinin Enable 32-Bit Applications değerinin true olması olduğunu anladık. Nitekim 64bit işletim sistemi üzerinde kurulmuş olan TFS, her nedense bu değişikliği lisans ihlali gibi algılamıştı (Öyle tahmin ediyorum)

O yüzden siz siz olun, mutlaka servislerinizi test etmek için ayrı bir IIS sunucusunun tahsis edilmesini isteyin

![Smile](images/wlEmoticon-smile_86.png)

Başıma gelen başka bir garip olayda görüşmek dileğiyle hepinize mutlu günler dilerim.
