---
title: "HTTP/2 Server Push Nasıl Bir Şeydir?"
pubDate: 2019-04-08 21:37:00
categories:
  - Node.js
tags:
  - Node.js
  - Bash
  - JavaScript
  - .NET
  - JSON
  - HTTP
  - Performance
  - GitHub
---

# HTTP/2 Server Push Nasıl Bir Şeydir?
![http2_writer.jpg](images/http2_writer.jpg)

Bir türlü giriş hikayesini bulamıyordum. Takip ettiğim referansta geçenleri West World üzerinde kurgulayıp sonuçları görmüş ve anladığım haliyle yazıya dökmüştüm. Ama o klasik girizgah kısmına koymam gereken hikayeyi bulamıyordum. Ne hikmetse ilgi çekici olması için her fırsatta üzerinde titizlikle durduğum bu kısmın ilham perisi tatile çıkmış aklıma tek bir düşünce dahi gelmemişti. Sonuçta istediğim girizgahı yapamadım... Yine de başlayalım.

HTTP/2 protokolü ile gelen önemli özelliklerden birisi de, tek bir TCP/IP bağlantısında sunucudan istemciye birden fazla kaynağın (Resource) gönderilebilmesidir. HTTP/2 esas itibariyle 2015 yılından beri hayatımızda. Sevgili Recep Duman arkadaşımın konu ile ilgili [şurada bir yazı](http://devnot.com/2015/http2-neleri-degistirecek/)sı da bulunuyor.

Tabii olaya HTTP 1.1 ile HTTP/2 arasındaki farklılıkları göz önüne alarak bakmak gerekiyor. HTTP/2, 1.1 versiyonu gibi metin tabanlı değil, binary çalışan bir protokol. Bu nedenle ağda daha hızlı. HTTP/2 tek bir bağlantı üzerinden aynı anda n sayıda talebi karşılayabilecek ve bu tek bağlantı içerisinde istemci talep etmese bile onun için gerekli kaynakları kendisine gönderebilecek şekilde tasarlanmış. Dolayısıyla istemci index.html sayfasını talep ettiği zaman açılan bağlantı içerisinde, istemcinin index.html için gerek duyacağı ne kadar kaynak varsa sunucudan gönderilebilir (push işlemi olarak ifade edebiliriz) Header'lar konusunda da HTTP/2 oldukça yenilikçi. [HPACK](https://http2.github.io/http2-spec/compression.html) header sıkıştırma tekniğini kullanıyor ve bu sayede header boyutlarının azaltılmasın olanak tanıyor. Görüldüğü üzere HTTP/2'nin 1.1 sürümüne göre önemli performans artıları var (2015ten beri hayatımızda olan HTTP/2 ye ait endüstüriyel tanımlara [IETF'nin şuradaki sayfasından](https://tools.ietf.org/html/rfc7540) ulaşabilirsiniz)

Günümüzde pek çok web sitesi HTTP/2 protokolüne destek veriyor ve bir kaynağa bağlı içerikleri istemcinin talep etmesini beklemeden proaktif hareketle karşı tarafa gönderiyor. Örneğin Medium'un ana sayfasına gidelim. Google Chrome'da F12 ile açılan developer sekmesine bakılırsa, Network trafiğini izleyen kısımda h2 lakaplı izlere rastlanır. Bu izler HTTP/2 protokolüne aittir ve ilgili kaynakların istemci talep etmeden Initiator (bu örnek kapsamında index sayfası olarak görünüyor) için gönderildiğini ifade etmektedir.

![http2_spush_3.gif](images/http2_spush_3.gif)

Örneğin m2.css, main-base.bundle.js dosyaları HTTP/2, p.js HTTP 1.1 ve son olarak analytics.js SPDY ile gelmekte ([SPeeDY diye okunuyor ve Google](https://www.chromium.org/spdy/spdy-whitepaper)'ın web'i daha hızlı hale getirmek için üzerinde çalıştığı deneysel bir protokol olduğu belirtiliyor. Henüz detaylarını öğrenemedim) Tabii Medium gibi içerik açısından zengin bir sayfanın ağ trafiğini takip ederken tekil bağlantıya karşılık n kaynağın tek seferde gönderildiğini görmek zor.

Peki biz kendi sunucularımızda HTTP/2 protokolünü ve Server Push özelliğini nasıl kullanabiliriz? Bunu node.js kullanarak gerçekleştirmek mümkün kaynaklardaki örnekler oldukça açıklayıcı. Bir benzerini yapmaya çalışalım. Örneğimizde ilk olarak HTTP 1.1 tabanlı standart bir sunucu kodunu ele alacağız. İkinci aşamada ise HTTP/2 tabanlı çalışan versiyona bakacağız. Ben referanstakine benzer olarak aşağıdaki gibi bir yapı hazırladım.

sample
--- images
------ sample_1.jpg
------ sample_2.jpg
------ ve diğer bir kaç imaj
--- scripts
------ jquery.js
--- style
------ style.css
appv1.js
appv2.js
package.json
simpleCert.pem
simpleKey.pem

images, scripts ve style klasörü içerisinde yer alan içerikleri (javascript betikleri, medya ve css gibi materyaller) index.html'e gelen isteğe ait oturumda henüz istemci talep etmeden karşı tarafa gönderildiklerini görmeyi umut ediyorum. Her iki örnek içinde test amaçlı sertifikalara ihtiyaç var. Self-Signed sertifikaları West-World (Ubuntu olduğunu ezberldiniz artık) ortamında openssl ile aşağıdaki şekilde üretebildim. 2048 bit RSA baz alarak hareket ediyoruz.

```bash
openssl req -x509 -newkey rsa:2048 -nodes -sha256 -subj '/CN=localhost' -keyout simpleKey.pem -out simpleCert.pem
```

![http2_spush_1.gif](images/http2_spush_1.gif)

index.html içeriği çok önemli değil ancak beraberinde gitmesini beklediğimiz kaynakları taşıması gerekiyor.

```text
<!DOCTYPE html>
<html>

<head>
    <title>HTTP2 Server Push</title>
    <link rel="stylesheet" href="style/style.css">
    <script src="scripts/jquery.js"></script>
</head>

<body>
    <h1>Simple HTTP/2 Server Push Sample</h1>
    <p>"Lorem ipsum dolor sit amet, consectetur adipiscing elit
        , sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "</p>
    <table>
        <tr>
            <td><img src="/images/sample_5.jpg" alt="sample_5" /></td>
            <td><img src="/images/sample_3.jpg" alt="sample_3" /></td>
        </tr>
        <tr>
            <td><img src="/images/sample_2.jpg" alt="sample_2" /></td>
            <td><img src="/images/sample_4.jpg" alt="sample_4" /></td>
        </tr>
        <tr>
            <td colspan="2"><img src="/images/sample_1.jpg" alt="sample_1" /></td>
        </tr>
    </table>
</body>

</html>
```

HTML içeriğinde referans olunan bazı kaynaklar var. jQuery.js, style.css ve örnek jpg uzantılı resim dosyaları bu sayfa ile alakalı. Normal şartlarda index.html sayfasını talep ettiğimizde tarayıcı ile sunucu arasındaki ağ trafiğinde nasıl hareketlilikler olur görmek için appv1.js dosyasında aşağıdaki kodlar kullanılıyor.

```javascript
const fs = require("fs");
const mime = require("mime");
const https = require("https");

const securityOptions = {
    key: fs.readFileSync("simpleKey.pem"),
    cert: fs.readFileSync("simpleCert.pem")
};

const handler = (req, res) => {
    console.log("[Request]", req.url);
    if (req.url === "/favicon.ico") {
        res.writeHead(200);
        res.end();
        return;
    }
    const fileName = req.url === "/" ? "index.html" : __dirname + req.url;
    fs.readFile(fileName, (err, data) => {
        if (err) {
            res.writeHead(503);
            res.end("File read error", fileName);
            return;
        }
        res.writeHead(200, { "Content-Type": mime.getType(fileName) });
        res.end(data);
    });
};
https.createServer(securityOptions, handler)
    .listen(5047, () => console.log("Server listening at 5047"));
```

https modülünü kullanarak 5047 portundan dinleme yapacak bir nesne oluşturuluyor. createServer metoduna verilen parametrelerde sertifika ve talepleri ele alacak değişken bildirimlerine yer veriliyor. Handler içerisinde favicon için boş bir response dönüyor ve gelen talebe göre statik dosyanın yollanması sağlanıyor. Örneğe göre dosya adı belirtilmediği durumda otomatik olarak index.html dosyasının işlenmesi sağlanıyor. Eğer olmayan bir kaynak talep edilirse, kibarca 503 hatası basılıyor. Eğer appv1.js dosyası komut satırından

```bash
node appv1.js
```

ile çalıştırılıp, https://localhost:5047/ adresine gidilirse aşağıdaki ekran görüntüsünde yer alan hareketlilikleri görmemiz muhtemel.

![http2_spush_4.gif](images/http2_spush_4.gif)

Dikkat edileceği üzere jpg, jquery.js ve style.css kaynakları HTTP 1.1 protokolü nezninde değerlendirilmiştir. Ancak daha da önemlisi orta kısımda yer alan renkli çizgilerdir. Burada her kaynak için istemciden sunucuya gelindiği görülebilir (Sondaki kaynakça listesinde çok daha alofortanfaneli örnekler var bakın derim) Bu durumun HTTP/2 örneğinde değişmesini bekliyoruz. Hiç vakit kaybetmeden appv2.js içeriğine geçelim.

```javascript
const http2 = require("http2");
const fs = require("fs");
const mime = require("mime");

const securityOptions = {
  key: fs.readFileSync("simpleKey.pem"),
  cert: fs.readFileSync("simpleCert.pem")
};

const sendResource = (stream, fileName) => {
  const fd = fs.openSync(fileName, "r");
  const stat = fs.fstatSync(fd);
  const headers = {
    "content-length": stat.size,
    "last-modified": stat.mtime.toUTCString(),
    "content-type": mime.getType(fileName)
  };
  stream.respondWithFD(fd, headers);  
  stream.end();
};

const pushResource = (stream, path, fileName) => {
  stream.pushStream({ ":path": path }, (err, pushStream) => {
    if (err) {
      throw err;
    }
    console.log("[Pushing]", fileName);
    sendResource(pushStream, fileName);
  });
};

const handler = (req, res) => {
  console.log("[Request]", req.url);

  if (req.url === "/") {
    pushResource(res.stream, "style/style.css", "style.css");
    pushResource(res.stream, "scripts/jquery.js", "jquery.js");

    const images = fs.readdirSync(__dirname + "/images");
    for (let i = 0; i < images.length; i++) {
      const fileName = __dirname + "/images/" + images[i];
      const path = "images/" + images[i];
      pushResource(res.stream, path, fileName);
    }

    sendResource(res.stream, "index.html");
  } else {
    if (req.url === "/favicon.ico") {
      res.stream.respond({ ":status": 200 });
      res.stream.end();
      return;
    }
    const fileName = __dirname + req.url;
    sendResource(res.stream, fileName);
  }
};

http2.createSecureServer(securityOptions, handler)
  .listen(5048, () => {
    console.log("Server is listening on 5048");
  });
```

Kodlar bir önceki kod parçasına göre biraz daha karışık ancak temel ilkeleri oldukça basit. Bu kez http2 modülünü kullanarak 5048 portu üzerinden hizmet veren bir sunucu söz konusu. Sertifikalar securityOptions değişkeni ile verilirken talepler yine handler fonksiyonu üzerinden ele alınıyor. Burada css, js ve jpg kaynakları için çağırılan sendResource metoduna odaklanmamız lazım. Duruma göre tek bir dosya veya klasör içindeki tüm jpg dosyaları için çağırılan sendResource'a gelinmekte. sendResource istemci ve sunucu arasındaki veri akışında rol oynanan stream nesnesini kullanıyor. İlk etapta talep edilen dosya ile ilgili bilgileri alıyor. Boyut, son değişiklik zamanı ve içerik tipi. Sonrasında açılan stream kapatılıyor.

Şimdi,

```bash
node appv2.js
```

ile sunucu çalıştırılıp https://localhost:5048/index.html adresine gidilirse bu kez bir öncekinden farklı olarak tek bir ağ çizgisinin oluştuğu görülebilir.

![http2_spush_5.gif](images/http2_spush_5.gif)

Dikkat edileceği üzere tek bir çizgi var. Bir başka deyişle sunucuya yapılan index.html talebi sonrası açılan tekil bağlantı (connection) için, stream süresince sunucudan gönderilen diğer kaynaklar da söz konusu. Kabaca aşağıdaki gibi bir durum var diyebiliriz.

![http2_spushg.gif](images/http2_spushg.gif)

Pek tabii bu çalışma mantığını daha otomatize etmenin bir yolu var mıdır henüz bilmiyorum. Nitekim kaynakları fazla olan sayfalar için sunucu tarafındaki kod karmaşıklığını arttırmamak bence önemli. Diğer yandan bu senaryoyu.Net Core için nasıl hazırlayabiliriz bir bakmakta yarar var. Araştırmak istediğim konulardan birisi de bu.

Internet üzerinde hareket eden içeriklerin kalitesi, boyutu ve çeşitliliği arttıkça daha hızlı protokollere ihtiyacamız olacak gibi görünüyor. Etkileyici görünen bir web sayfasının ağ tarafındaki hareketlilik çoğu zaman inanılmaz boyutlarda. HTTP/2 şu anda iyi bir çözüm olarak görünse de SPDY ile birlikte neler olacağını da göreceğiz. Nitekim ihtiyaç olunmuş ki Google bunun üzerinde çalışmalara başlamış. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örnek kodunu [github'dan](https://github.com/buraksenyurt/nodejs-tutorials/tree/master/Day11) indirebilirsiniz
[Kaynak 1](https://medium.com/@noobj/exploring-http2-part-2-with-node-http2-core-and-hapijs-74e3df14249)
[Kaynak 2](https://medium.com/@noobj/exploring-http2-part-1-overview-dc3e9b53968f)
[Kaynak 3](https://medium.com/the-node-js-collection/node-js-can-http-2-push-b491894e1bb1)
[Kaynak 4](https://medium.com/@sibu.it13/an-example-of-server-push-with-http-2-in-node-js-22757256f0b3) (Asıl izlediğim kaynak)
