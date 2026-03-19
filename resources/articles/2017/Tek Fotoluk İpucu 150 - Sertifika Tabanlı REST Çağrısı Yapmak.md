---
title: "Tek Fotoluk İpucu 150 - Sertifika Tabanlı REST Çağrısı Yapmak"
pubDate: 2017-01-12 21:15:00
categories:
  - Tek Fotoluk Ipucu
tags:
  - Tek Fotoluk Ipucu
  - REST
  - HTTP
  - Delegates
---

# Tek Fotoluk İpucu 150 - Sertifika Tabanlı REST Çağrısı Yapmak
Merhaba Arkadaşlar,

Geçtiğimiz günlerde üzerinde çalıştığımız projede şöyle bir ihtiyaç oldu: Uygulamanın şirket ağı dışındaki bir kuruma ait REST (Representational State Transfer) tabanlı servis noktalarını kullanması gerekiyordu. Bu noktalara göndereceğimiz HTTP Get,Post taleplerine göre bir takım sonuçlar alacak ve kurum içi süreçleri işletecekti. Söz konusu servis ve sunduğu EndPoint'ler ile olan iletişim ise X509 standardındaki bir sertifika üzerinden gerçekleştirilmeliydi. Test ortamında yaptığımız çalışmada, sunucu sertifikasının doğrulanması sonrası devreye girecek Callback operasyonunda hata mesajı aldık. Kurumla yaptığımız mutabakat sonrasında ise bu adımı atlayabileceğimizi öğrendik. Çözüm olarak küçük bir hile yaptık. Nasıl mı? Aynen aşağıdaki fotoğrafta görüldüğü gibi.

![tfi150.gif](images/tfi150.gif)

InternalWebClient, WebClient tipinden türetilmiş bir sınıf. Bu nedenle REST servisleri ile iletişim için gerekli temel fonksiyonlara sahip. Önemli olan ise GetWebRequest metodunun ezilmiş (override) olması. Bu fonksiyon bir web kaynağına doğru yapılan çağrılarda devreye giriyor. Metodun içinde ilk olarak HttpWebRequest örneği yakalanıyor. Ardından sertifikayı yüklüyor ve ServerCertificateValidationCallback temsilcisini (delegate) hile yolu ile devre dışı bırakıyoruz (senaryomuzda geriye hep true döndürmek üzere kurgulandı) Böylece InternalWebClient sınıfının tanımlanan ByPassCertValidateCallback özelliğine atanan değere göre sertifika ile ilgili Callback sürecinin atlanması veya atlanmaması sağlanıyor.

> Senaryomuzda WebClient türevli bir sınıf kullanmamızın sebebi, DownloadString DownloadData ve DownloadFile gibi REST servisine yapacağımız çağrı sonrası gelecek içerikleri kolayca almamızı sağlayan metodlar sunmasıydı. Ama sertifika senaryosunda yaşadığımız sorunu aşmak için minik bir takla atıp WebClient sınıfından gelen ve talebin (Request) hazırlandığı sırada devreye giren bir metodun davranışını değiştirmemiz gerekti.

Böylece geldik bir ipucumuzun daha sonuna. Başka bir ipucunda görüşünceye dek hepinize mutlu günler dilerim.
