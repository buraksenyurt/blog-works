---
layout: post
title: "Stream ve Pipe Mevzusu"
date: 2018-07-12 21:30:00 +0300
categories:
  - nodejs
tags:
  - nodejs
  - javascript
  - json
  - http
  - performance
---
West-World bu hafta neredeyse savaş alanı gibiydi. Node.js tarafında öğrenmeye çalştığım yeni konu sebebiyle makineyi bir çok kez restart etmek zorunda kaldım. Üstelik düğmeden:|

![piping_g_1.gif](/assets/images/2018/piping_g_1.gif)

Sebep çok büyük boyutlu bir dosya içeriğini basit bir web sunucusu üzerinden sunmaya çalışmaktı. Aslında kimse bu tip bir şey yapmaz. Hadi yapsa da koca dosyayı tek seferde istemciye göndermez. Kaldı ki istemci de bu web hizmetine herhangi bir tarayıcıdan talep göndermez.

Neyse ki sonunda doğru yolu buldum ve bu tip bir araştırma senaryosunda terminalden curl komutunu kullanarak ilerlemenin daha mantıklı olduğunu öğrendim. Tabii tüm bunlar için geçerli bir sebebim vardı. Akımların sıklıkla bahsedilen pipe fonksiyonunu denemek ve bunun performansa olan olumlu etkilerini görebilmek.

Node.js tarafında anlaşılması en zor konulardan birisinin akımlar (stream olarak telafüz edelim) olduğu söyleniyor. Özellikle event ve multi-process gibi kavramlarla yakın temas içerisinde. Okuduğum kaynaklar ve izlediğim Pluralsight eğitimlerine göre performans konusunda dikkat edilmesi gereken ve önemli özellikler barındıran bir mevzu. Özellikle büyük veri ile çalışan bir web sunucusu söz konusu ise stream nesnelerinin pipe mekanizması ile birlikte kullanılması tercih edilmeli. Gelin ne demek istediğimi benden daha iyi özetleyecek basit bir örnek ile konuya giriş yapalım.

Örnek Dosyanın Oluşturulması

Yapmak istediğim aynen kaynaklarda tariflendiği üzere dosya hizmeti veren bir web sunucusu oluşturmaktı. Performans farklılıklarını canlı görebilmek için en az bir dosyaya ve bunun farklı boyutlardaki hallerini ele alan senaryolara ihtiyacım vardı. Büyük boyutlu bir dosya bulmakla neden uğraşayım ki? Pekala içi anlamsız verilerle dolu bir dosyayı kendim oluşturabildim. İşe aşağıdaki kodlarla başladım.

```javascript
var fs = require('fs');

console.log("Big file is creating...");
var bigEF = fs.createWriteStream('bigEF.data');
for (var i = 0; i < 3e6; i++) {
    bigEF.write('{"fname": "Devon","lname": "Karma"},{"fname": "Lorenz","lname": "Douglas"},{"fname": "Ora","lname": "Wade"},{"fname": "Kelly","lname": "Ragusa"},{"fname": "Teresa","lname": "Gergely"},{"fname": "Wendy","lname": "Kerkemeyer"},{"fname": "Georgia","lname": "Malo"},{"fname": "Tonja","lname": "Lichtenwalner"},{"fname": "Dorota","lname": "Breiter"},{"fname": "Priscilla","lname": "Bartovics"}');
}
bigEF.end();
```

fs (Temel IO işlemleri için kullanıyoruz diyebilirim) modülünün kullanıldığı ve içerisinde JSON cemiyetinden rastgele insanların olduğu büyük boyutlu bir dosya üretiliyor. createWriteStream ile bigEF.data isimli örnek dosya için yazılabilir bir Stream nesnesi örnekliyoruz. Buraya uygulayacağımız write çağrısı tahmin edeceğiniz üzere dosya içerisine ilgili metinsel içeriklerin yazılmasını sağlamakta. Tüm işlerin onaycısı sondaki end çağrısı. Kodu çalıştırdığımda West-World üzerinde 1.2 Gb'lık alan işgal edecek tamamen atmasyon verilerden oluşan bir dosya üretilmiş oldu. İlk senaryo için yeterli büyüklükte.

![piping_0.gif](/assets/images/2018/piping_0.gif)

Problem Çıkartalım

İlk hedef bu dosyayı bir web sunucusu üzerinden sunmak. Yani istemcilerin göndereceği bir HTTP talebine göre kendilerine bu dosya içeriğini göndereceğimiz anlamısız bir senaryomuz var. Aslında kodun yapacağı iş oldukça basit. Sunucuya gelen talep sonrası ilgili dosyayı okumak ve istemciye paslamak. Aşağıdaki kod parçası bu işi görebilir.

```javascript
var fs = require('fs');
var http = require('http');

var server = http.createServer().on('request', function (req, res) {
    fs.readFile('bigEF.data', function (err, data) {
        if (err)
            throw err;
        res.end(data);
    });
});
server.listen(65002);
console.log('Server is online');
```

Daha önceden aşina olduğumuz üzere http modülüne ihtiyacımız var. Sunucu nesnesi örneklendikten sonra request olayını dinleyecek şekilde bir metod zinciri bulunuyor. Callback fonksiyonu istemciden gelen talep sonrası çalışacaktır. İçerisinde öncelikle dosyanın okunmasını sağlıyoruz. readFile'ın Callback fonkisyonunda ise okunan dosya içeriğini response değişkenine yazıyoruz. Daha doğrusu response değişkeninin açtığı stream'e aktarıyoruz. Sunucu West-World'ün 65002 nolu Kuzey Batı kapısından hizmet verecek şekilde tasarlanmış durumda.

Uygulamayı çalıştırıp curl ile (özellikle curl ile denedim çünkü tarayıcı ile gerçekleştirdiğim acı deneyimler sonrası makineyi bir kaç kez düğmesinden kapatıp açmak zorunda kaldım) 65002 nolu porta talep gönderdiğimde aşağıdaki sonuçlarla karşılaştım.

![piping_1.gif](/assets/images/2018/piping_1.gif)

Aynen kaynaklarda bahsedildiği gibi olmuştu. Sunucu açık ve istemci curl komutu ile adrese bir talep gönderiyor. Kısa bir süre için sorun yok. Derken bir anda bellek tüketimi artıp çıktığı noktada seyretmeye devam ediyor. Hatta West-World'ün klima sistemi de aynı anda coşmuştu diyebilirim. Uygulama çalışmasını tamamlandığında ise her şey normale döndü. Bellek tüketimi kısa süre içinde baştaki seviyelere indi.

![piping_2.gif](/assets/images/2018/piping_2.gif)

Burada gözle görülür bir performans sıkıntısı olduğu ortada. Sadece tek bir dosya için gönderilmiş bir talep var ancak n sayıda talebin gelmesi ve küçük boyutlarda olsalar bile onlarca, yüzlerce dosyanın sunulacağı bir sistem için çok daha büyük sorunlar oluşması pekala mümkün.

Pipe Kullanımı

İşte bu noktada pipe fonksiyonu ile karşılaşılıyor. Çok temel olarak kaynak ve hedef arasında tek veya çift yönlü bir boru hattının oluşturulmasına yarayan ama enteresan avantajlar sunan bir fonksiyondan bahsediyoruz aslında. Bu fonksiyon, açılan kanal üzerinden verilerin akışını sağlıyor ancak bi farkla; kendisi stream nesne örneklerine uygulanabiliyor ve verinin tamamının belleğe alınması yerine belli boyutlarda parçalanarak kullanılmasına olanak sağlıyor. Örneğin 1.2 Gb'lık devasa bir dosyanın tamamen belleğe alınıp işlenmesi yerine küçük parçalar haline (chunk) bölünerek ele alınmasını sağlıyor (Bunu nasıl yapabileceğini bir düşünün derim. Yani siz böyle bir fonksiyon yazmaya çalışsanız nasıl yazardınız?) pipe için şöyle bir ifade doğru olacaktır.

[kaynak].pipe ([hedef])

West-World'de ikinci deneyi gerçekleştirmenin zamanı gelmişti. Öğrendiklerimi uygulayarak aşağıdaki kod parçasını hazırladım.

```javascript
var fs = require('fs');
var http = require('http');

var server = http.createServer().on('request', function (req, res) {
    var source = fs.createReadStream('bigEF.data');
    source.pipe(res);
});
server.listen(65002);
console.log('Server is online');
```

Bir önceki örnekten farklı olarak doğrudan readFile fonksiyonunu kullanmak yerine createReadStream için çağrı yapılmakta. Bunun sonucu olarak veri okunabilir bir akım örneklenecek (ReadableStream). source olarak isimlendirilen nesne örneği üzerinden pipe metodunun nasıl kullanıldığına dikkat edelim. Parametre olarak response değişkenini alıyor. response, bu senaryo gereği üzerine veri yazılabilir bir akım (WritableStream) Buna göre dosyadan okudukça, çıktı olarak ağ üzerindeki kanala veri yazılıyor diye düşünebiliriz.

Testi tekrar yaptığımda West-World'te ortam gayet sakin görünmekteydi. Önce sunucu uygulamasını çalıştırdım, ardından curl ile talebi gönderdim.

![piping_3.gif](/assets/images/2018/piping_3.gif)

Veriler yine okunuyordu ancak sunucunun bellek tüketiminde gözle görülür önemli bir artış olmamıştı. Hatta neredeyse hiç olmamıştı.

Pipe Yerine Event Kullanımı

Bir önceki örnekte yer alan pipe fonksiyonu yerine olay bazlı kurgulama ile de aynı senaryo çalıştırılabilir. Hatta bu durumda olay fonksiyonlarındaki parametrelerin gücünden de yararlanılabilir. Kodları aşağıdaki gibi düzenleyerek testlere devam ettim. Ancak öncesinde üzerinde çalışacağım dosya boyutunu epeyce küçülttüm. Nitekim buffer kullanacağım için bu veri kümelerini (chunk) izleyebilmek istiyordum (Burada böyle yazıyorum çünkü önce bir kaç deneme yaptım. Baktım terminalden parçaları göremiyorum, üzerinde çalışacağım dosya boyutunu küçülttüm)

```javascript
var fs = require('fs');
var http = require('http');

// pipe yerine stream eventlerini kullanmak
// daha küçük boyutlu bir dosya seçelim ki takibimiz kolay olsun

var server = http.createServer().on('request', function (req, res) {
    var source = fs.createReadStream('bigEF.data');
    // aşağıdaki data ve end olayları da bir nevi pipe'ın karşılığıdır.
    source.on('data', function (chunk) {
        res.write(chunk);
        var date = new Date().toISOString();
        console.log('\n' + date + '\n');
        console.log('\t' + chunk);
    });
    source.on('end', function () {
        res.end();
        console.log('end');
    });
});

server.listen(65002);
console.log('Server is online');
```

Dikkat edileceği üzere source nesnesi yine okunabilir stream örneği olarak başrolde. Bu sefer ilgili nesne için data ve en isimli olayları bildiriyoruz. Her iki olay için de callback fonksiyonları içinde gerekli işlerin yapıldığını belirtebiliriz. data olayı gerçekleştiğinde istemci talebine cevaben verinin küçük bir parçasını gönderiyoruz. Dosyaya ait bütün parçaların gönderimi tamamlandığında end olayı tetiklenmekte. Burada da istemciye göndereceğimiz verilerin tamamlandığını belirtiyoruz.

Çalışma zamanı çıktılarına baktığımda aşağıdaki gibi tampon bölgeye alınan veri kümelerinin değerlendirildiğini gördüm. Hatta üstteki bilginin kesildiği yerden alttakinin devam ettiğini de fark etmiş olmalısınız.

![piping_4.gif](/assets/images/2018/piping_4.gif)

Dikkat edileceği üzere bellek kullanımında yine önemli bir sıkıntı görülmüyor.

Limitleri Zorlayalım

Peki ya dosya boyutu baya baya büyük olsaydı. Kaynaklarda bahsedilen 2Gb sınırını merak ediyordum aslında. Özellikle bu değer için readFile'ın cevap vermediği ifade ediliyordu. Bu nedenle saçma veriler içeren dosya boyutunu 2Gb'ın üstüne çıkarttım.

![piping_5.gif](/assets/images/2018/piping_5.gif)

West-World bu kez 2.3Gb'lık bir saha işgali ile karşı karşıyaydı. pipe mekanizmasını kullandığım kodu bir kenara bıraktım ve ilk olarak standart okuma yöntemini kullanmaya karar verdim. Sonuçları almam hiç uzun sürmedi.

![piping_6.gif](/assets/images/2018/piping_6.gif)

Görüldüğü üzere dosya boyutu olası Buffer boyutunun üzerindeydi. Bu sebepten işlemler zaten yapılamadı. Ancak pipe fonksiyonelliğinin kullanıldığı kod parçası söz konusu dosyayı sorunsuz bir şekilde işlemeyi başarmıştı.

![piping_7.gif](/assets/images/2018/piping_7.gif)

Diğer Kullanışlı Bilgiler

Bitirmeden önce bir kaç teknik bilgi daha vermeye çalışayım (Anladığım kadarıya tabii) Node.js ile birlikte gelen bir çok standart stream enstrümanı söz konusu. Bunları iki ana kategoriye ayırmak mümkün. Okuma amaçlı kullanılanlar (readable streams) ve yazma amaçlı (writable streams) kullanılanlar. Sonuç itibariyle bir kaynaktan veri okuma veya bir hedefe veri yazma işleri bu kapsamlara giriyor. Bazı node.js türleri ise her iki rolü birden üstlenebiliyor. TCP soketleri, sıkıştırma kütüphanesi (zlib) ve şifreleme nesneleri her iki rolü üstlenen aktörlerden.

Bunlara ek olarak tek yönlü olan stream nesneleri de bulunuyor. Örneğin bir istemcinin sunucuya gönderdiği HTTP talebi sunucu açısından readable stream olarak değerlendirilirken, istemci açısından writable stream şeklinde anlam kazanıyor. Tam tersi durumda söz konusu elbette. Okunabilir veya yazılabilir akımlar yalnız değiller. Bunlara ilaveten çift yönlü (duplex) ve dönüşebilir (transform) akım türleri de var.

Özellikle duplex formatta olanlar çok ilginç. Öyle ki bunları ele aldığımız senaryolarda arka arkaya birden fazla pipe çağrısının yer aldığı metod zincirleri oluşturulması mümkün. Nitekim pipe çağrısının sonucu yine okunabilir ve dolayısıyla başka bir kaynağa girdi olarak aktarılabilir bir stream olabiliyor. Mesela Veli okunabilir, Ayşe ve Hakan hem okunabilir hem yazılabilir, son olarak da Levent sadece yazılabilir ise şu şekilde bir ietişim hattı oluşturmak mümkün (müş).

```text
veli.pipe(ayşe).pipe(hakan).pipe(levent) 
```

İnanın bu noktada benim de kafam epey karışmış durumda. Şimdilik yapacağım şey biraz dinlenmek ve konunun diğer detaylarına bir şekilde inmeye çalışmak. Lütfen siz de araştırın ve Node.js dünyasındaki stream konusunu en ince detayına kadar öğrenmeye bakın. Söz gelimi dosya harici büyük veri kümeleri üzerinde işlem yapmak istediğiniz soket haberleşmesi odaklı senaryolar da bu vakaları ele almayı deneyebilirsiniz. Benden şimdilik bu kadar. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

## Kaynaklar

[Basic of Node.js Streams](https://www.sitepoint.com/basics-node-js-streams/)
[Node.js Offical Documentation](https://nodejs.org/api/stream.html)[The Definitive Guide of Object Streams in Node.js](https://community.risingstack.com/the-definitive-guide-to-object-streams-in-node-js/)
[Free Code Camp](https://medium.freecodecamp.org/node-js-streams-everything-you-need-to-know-c9141306be93)
[W3School](https://www.w3schools.com/nodejs/ref_stream.asp)
[Events and Streams in Node.js](https://codeburst.io/basics-of-events-streams-and-pipe-in-node-js-b84578c2f1be)
