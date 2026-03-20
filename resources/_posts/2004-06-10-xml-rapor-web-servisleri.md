---
layout: post
title: "XML Rapor Web Servisleri"
date: 2004-06-10 12:00:00 +0300
categories:
  - xml-web-services
tags:
  - xml-web-services
  - csharp
  - dotnet
  - aspnet
  - sql-server
  - xml
  - soap
  - web-service
  - http
  - visual-studio
  - asmx
---
Bu makalemizde, Crystal Report'ların birer web servisi olarak nasıl yayınlanacaklarını ve istemciler tarafından kullanılacaklarını kısaca incelemeye çalışacağız. Hepimizin bildiği gibi Web Servislerinin günümüz teknolojilerine getirdiği en büyük yenilik, merkezileştirilmiş metodların, herhangibir platformda yer alan sayısız istemci tarafından, hiç bir engele yada kısıtlamaya takılmadan kolayca çağırılabilmeleri ve sonuçların aynı yollar ile kolayca sorunsuz elde edilebilmeleridir. Öyleki, web servislerinin XML tabanlı olarak, SOAP protokolünün belirlediği kriterlerde HTTP gibi basit iletişim protokolleri üzerinden anlaşmayı desteklemesi, onların esnek, genişleyebilir, kolay erişilebilir ve popüler olmalarını sağlamıştır.

İşte web servislerinin sunmuş olduğu bu imkanlardan, geliştirmiş olduğumuz Crsytal Report'larında faydalanmalarını sağlayabiliriz. Çoğunlukla, müşterilerimizin yada firmamız ile birlikte çalışan tedarikçilerimizin, ortak bir noktadan, Crystal Report olarak hazırladığımız esnek ve güçlü raporlara, her platformdan sorunsuz ulaşılabilmeleri amacıyla web servisi tabanlı raporlar geliştirilir.

XML Rapor Web Servislerinin geliştirilmesi iki aşamalı bir işlemden ibarettir. İlk olarak, rapor dosyamızı baz alıcak bir web servisinin geliştirilemesi gerekir. Bu raporu kullanmak için ise, standart bir web servisinin kullanılmasında uygulanan prosedürler işletilir. Yani, web servisine ait referans, uygulamaya eklenir. Sonrasında ise, eğer uygulamayı Visual Studio.Net ile geliştirdiysek, web servisimiz için uygulamamızda bir proxy nesnesi, bu servisin istemci uygulamaya indirilen wsdl dökümanı vasıtasıyla otomatik olarak oluşturulur. Sonrasında, istemci uygulamamız ile web servisimiz arasındaki mesajlaşmalar SOAP protokolünün belirlediği tipte bu proxy nesnesi yardımıyla gerçekleştirilir. Dolayısıyla, geliştireceğimiz istemci uygulamada, bu proxy sınıfına ait nesne örneğini kullanmamız gerekecektir.

Lafı fazla uzatmadan XML Rapor Web Servislerinin geliştirilmesi ile işe başlıyalım. Bu amaçla ilk olarak Visual Studio.Net ortamında bir Asp.Net Web Uygulaması açalım. Bu noktada uygulamamıza, Solution Explorer'dan sağ tıklayarak açtığımız menüden Add Exsiting Item ile daha önceden geliştirmiş olduğumuz herhangibir rpt uzantılı rapor dosyasını ilave edelim. (Bu noktada asıl amacımız rapor dosyasının nasıl oluşturulduğu olmadığı için, bu konunun üstünde fazla durmayacağım.)

Ben uygulamamda örnek olarak, daha önceden geliştirdiğim çok basit bir rapor dosyasını kullandım. Bu rapor, Sql Server sunucusunda yer alan Northwind veritabanındaki Customers tablosuna ait bir kaç veriden oluşmakta. Bizim için asıl önemli olan kısım, bu raporun, bir web servisi olarak yayınlanmak istenmesidir. İşte bu amaçla raporumuza sağ tıklıyor ve çıkan menüden Publish As Web Service seçeneğini işaretliyoruz.

![mk72_1.gif](/assets/images/2004/mk72_1.gif)

Şekil 1: Raporun Web Servisi Olarak Yayınlanması.

Bu işlemin ardından raporumuz için, asmx uzantılı web servisi dosyamızın oluşturulduğunu görürüz.

![mk72_2.gif](/assets/images/2004/mk72_2.gif)

Şekil 2: Raporumuzun Web Servisi Haline Getirilmesi.

Dilersek bu sayfayı web browser'dan çalıştırarak raporumuz ile ilgili olarak neler yapabileceğimi görebiliriz. Artık elimizde, raporumuzu kullanmak isteyen istemcilere sunabileceğimiz bir web servisimiz var. Son adım olarak uygulamamızı derleyelim. Şimdi bu XML Rapor Web Servisini kullanmak isteyen istemcileri nasıl geliştireceğimize bakalım. Bunun için örnek olarak bir Asp.Net web uygulaması açalım. Bu uygulamaya web servisinden elde edeceğimiz raporu gösterecek olan bir Cyrstal Report Viewer nesnesini ekleyelim. Sayfamız aşağıdakine benzer bir yapıda olabilir.

![mk72_3.gif](/assets/images/2004/mk72_3.gif)

Şekil 3: İstemci Asp.Net sayfasının tasarım görünümü.

Tanımlanmış olan bir web servisini, bir istemciye eklemek için, Add Web Reference seçeneğini kullanırız. Bu durum, elbetteki XML Rapor Web Servisleri içinde geçerlidir.

![mk72_4.gif](/assets/images/2004/mk72_4.gif)

Şekil 4. XML Rapor Web Servisinin İstemci Uygulamaya Refernas Edilmesi.

Buradab karşımıza aşağıdaki pencere gelicektir. Biz yerel makinede çalıştığımız ve web servisimizide localhost'umuzda geliştirdiğimiz için, kullanılabilir web servislerini öğrenmek amacıyla getirilen listeden, Web services on the local machine linkini seçebiliriz.

![mk72_5.gif](/assets/images/2004/mk72_5.gif)

Şekil 5. Kullanılabilir web servislerinin öğrenilmesi.

Bu linki seçtiğimiz takdirde sistemde kullanabileceğimiz web servisleri ekrana gelicektir. Bu servislerden bir taneside, geliştirmiş olduğumuz XML Rapor Web Servisidir. Bu raporu seçerek, web servisine ait referansı uygulamamıza ekleyelim.

![mk72_6.gif](/assets/images/2004/mk72_6.gif)

Şekil 6. XML Rapor Web Servisimizin Seçilmesi

XML Rapor Web Servisimizin seçilip, istemci uygulamamıza eklenmesi ile birlikte, vs.net bu web servisine ait wsdl dosyasını talep eder. Wsdl dökümanı web servisinin public arayüzünün bir görüntüsünü xml formatında istemci uygulamada kullanabilmemizi sağlar. Bu işlemin ardından, istemci uygulamada XML Rapor Web Servisimize ait Wsdl dökümanıda otomatik olarak oluşturulur. Bu döküman yardımıyla da, web servisimiz ile aramızdaki ilişkiyi nesnesel bazda gerçekleştirebilmemizi sağlayan proxy sınıfımız oluşturulacaktır. Bu durumda Solution Explorer'da projemizin son halinin aşağıdaki gibi olduğunu görürüz.

![mk72_7.gif](/assets/images/2004/mk72_7.gif)

Şekil 7. Proxy Sınıfımız Oluşturulur.

Artık tek yapmamız gereken, XML Rapor Web Servisimizden elde edeceğimiz raporun içeriğini, Crystal Report Viewer nesnemizde göstermek. Bunun için aşağıdaki basit kod satırlarını uygulamamıza eklememiz yeterli olucaktır.

```csharp
private void btnRaporla_Click(object sender, System.EventArgs e)
{
    /* Öncelikle proxy sınıfımıza ait bir nesne örneğini oluşturuyoruz. */
    localhost.MusteriRaporlariService rapor=new RaporIstemci.localhost.MusteriRaporlariService(); 
    this.CrystalReportViewer1.ReportSource=rapor; // Web sayfamızdaki CrystalReportViewer nesnemize rapor kaynağı olarak, proxy nesnemizi atıyoruz. */
    this.CrystalReportViewer1.DataBind(); /* Nesnemizi gelen rapor verilerine bağlıyoruz.*/
}
```

Burada olanları kısaca özetlemek gerekirse; öncelikle XML Rapor Web Servisimizin istemci uygulama için oluşturulan proxy sınıfından bir nesne örnekledik. Böylece, istemcimizdeki Crystal Rapor nesnemiz, bu proxy sınıfından elde edilen verileri gösterecek. Elbetteki proxy nesnemizde bu verileri, XML Rapor Web Servisimizden almakta. Web sayfamızı çalıştırıp, buton kontrolümüze tıkladığımızda aşağıdakine benzer bir sayfa ile karşılaşırız.

![mk72_8.gif](/assets/images/2004/mk72_8.gif)

Şekil 8. Raporun Elde Edilmesi.

Bu makalemizde en basit haliyle, XML Rapor Web Servislerinin nasıl oluşturulduğunu ve kullanıldığını incelemeye çalıştık. Görüldüğü gibi kilit nokta, raporun web servisi olarak yayınlanmasıdır. (Publish As Web Service) Diğer taraftan geri kalan bütün işlemler web servislerinin istemcilerde kullanılmasındaki teknikler ile aynıdır. İlerleyen makalelerimizde görüşmek dileğiyle hepinize mutlu günler dilerim.