---
layout: post
title: "Pug, Pug, Pug"
date: 2018-07-08 21:30:00 +0300
categories:
  - nodejs
tags:
  - nodejs
  - bash
  - json
  - javascript
  - dotnet
  - rest
  - http
  - python
  - ruby
  - github
---
Kısa bir süre önce çalışmakta olduğum şirkette epey eğlenceli bir mevzunun içerisinde kaldım. Daha önceden de bahsettiğim gibi kurumumuzun bize sunduğu güzel bir hizmet var...[Pluralsight](https://www.pluralsight.com/). Ne var ki benim gibi dikkatsiz kullanıcılar için enteresan şeyler olabiliyor. İzin verin hikayeyi size anlatayım; İlk hedefim oldukça hoşuma giden Node.js tarafında ilerlemekti.

![node_with_pug_3.jpg](/assets/images/2018/node_with_pug_3.jpg)

Bu amaçla kendime çizdiğim kariyer yolunda (aslında Pluralsight Path demek daha doğru olur) bir takım Node.js eğitimlerini izlemeye başladım. Hatam bu eğitimlerin tarihlerine pek dikkat etmiyor oluşumdu. Öyle ki eğitimde sözü geçen bir npm modülü pekala eskimiş olabilirdi.

O hafta izlediğim eğitim serisi Node.js ile web programlama üzerineydi. Express ile işin içerisine girdikten sonra bir takım yardımcı çatılardan daha bahsedilmeye başlandı. Derken HTML'in sıkıcı olan o açısal ayraçlarından bizi kurtaran Jade isimli bir paketle karşılaştım. İnanılmaz hoşuma gitti. Nerden bilebilirdim aslında onun adının sonradan Pug olarak değiştirildiğinden ve hatta çalıştığım kurumun yeni nesil.Net projelerinde bu ürünü kullandığından:)

> Bu arada ben Jade kod adını daha çok beğendiğimi ifade etmek isterim. Nitekim "Yeşim Taşı" olarak anlam kazanır. Oysa ki pug...Peah...Buldoğa benzeyen ufak bir köpek olduğu ifade ediliyor. Aslında Jade'in Pug olma hiyakesini [şuradan](https://github.com/pugjs/pug/issues/2184) okuyabilirsiniz.

O Cuma günü her zaman ki gibi sabah izlediğim derslerin bir uygulamasını gün içerisinde sıkıldığım anlarda yapmıştım (aman patron duymasın) Sonrasında sevgili [Hilmi ağabeye](https://www.linkedin.com/in/hilmidonmez/) durumu anlatmak istedim. Sık sık öğrendiklerimizi birbirimizle paylaşıyırız zaten. Heyecanlı bir şekilde anlattım da anlattım...Hatta yaptıklarımı gösterdim bile. Sonra çok güzel bir cümle sarfetti "yahu bunun bizim kullandığımız Pug'dan farkı ne? Neredeyse aynı..." Hemen google'ladık tabii...Ne gördük dersiniz? Pug meğerse Jade'in yeni adıymış. Önce birbirimize şaşkın şaşkın baktık ve bir süre sonra kahkayı patlattık:D

Tabii ki yaptığım kodlar çöpe gitmedi. Jade yerine hemen [Pug paketini](https://pugjs.org/api/getting-started.html) uygulamaya dahil edip aynı sonuçları elde etmeyi başardım. Peki neydi o becerebildiğim ve beni heyecanlandıran şey? Malum uzun yıllar back-end tarafında geliştirme yapmış birisi olarak front-end benim için epey geniş ve bir o kadar da korkutucu bir alan. Ancak görevim gereği o noktalara da dokunmam gerekiyor. Dilerseniz elde ettiğim sonucu göstererek başlayalım. Aşağıdaki ekran görüntüsüne bakalım.

![node_with_pug_1.gif](/assets/images/2018/node_with_pug_1.gif)

Tarayıcı penceresinde açılmış basit bir HTML içeriği görüyorsunuz aslında değil mi? Hatta berbat ötesi bir tasarımı da var buna hiç şüphe yok:) Peki ya aslında bu sayfanın HTML tarafının aşağıdaki gibi yazıldığını söylesem.

```text
html
    head
        meta(charset='utf-8')
        link(href='https://fonts.googleapis.com/css?family=Chewy',rel='stylesheet')
        title I love this game
    body(style='width:410px;font-family:Chewy,cursive')
        h2(id='myId',style='background-color:darkblue;color:white') İ s t a t i s t i k l e r
        p
            h3(style='background-color:red;color:white') En iyi takımlar
            p.
                NBA tarihinde tüm zamanların en iyi takımları   
            ul
                each team in teams
                    b
                        li=team
        p
            h3(style='background-color:green;color:white') En iyi oyuncular
            p.
                NBA tarihinin bugüne kadar gelmiş geçmiş en iyi oyuncuları
            mixin player-card(player)
                div(style='background-color:lightgray').player-card
                    a(href=player.url)
                        div.player-name=player.name
                    p
                        img.player-photo(src=player.photo)
                    p
                        i.heigth=player.height                        
                    p
                        i.bio=player.bio     
                    p                                        
            for player in players
                +player-card(player)
```

Şimdi işler biraz değişti değil mi? En azından bir farkındalık oluştu. Normalde yukarıdaki içeriğin görüntülenmesi için klasik ve bilinen yöntemlerle aşağıdakine benzer bir HTML dosyası hazırlanır.

```text
<html>

<head>
    <meta charset="utf-8" />
    <link href="https://fonts.googleapis.com/css?family=Chewy" rel="stylesheet" />
    <title>I love this game</title>
</head>

<body style="width:410px;font-family:Chewy,cursive">
    <h2 id="myId" style="background-color:darkblue;color:white">İ s t a t i s t i k l e r</h2>
    <p>
        <h3 style="background-color:red;color:white">En iyi takımlar</h3>
        <p>NBA tarihinde tüm zamanların en iyi takımları </p>
        <ul>
            <b>
                <li>bulls</li>
            </b>
            <b>
                <li>celtics</li>
            </b>
            <b>
                <li>lakers</li>
            </b>
        </ul>
    </p>
    <p>
        <h3 style="background-color:green;color:white">En iyi oyuncular</h3>
        <p>NBA tarihinin bugüne kadar gelmiş geçmiş en iyi oyuncuları</p>
        <div class="player-card" style="background-color:lightgray">
            <a href="http://www.espn.com/nba/player/_/id/3975">
                <div class="player-name">sitivin köri</div>
            </a>
            <p>
                <img class="player-photo" src="curry.gif" />
            </p>
            <p>
                <i class="heigth">190cm</i>
            </p>
            <p>
                <i class="bio">benzersiz top tekniği, yüksek şut yüzdesi, sınır tanımaz üçlükleri...</i>
            </p>
            <p> </p>
        </div>
        <div class="player-card" style="background-color:lightgray">
            <a href="http://www.espn.com/nba/player/_/id/1966">
                <div class="player-name">löbron ceyms</div>
            </a>
            <p>
                <img class="player-photo" src="lebron.gif" />
            </p>
            <p>
                <i class="heigth">206cm</i>
            </p>
            <p>
                <i class="bio">çok güçlü, ani hızlanma, meydan okuma, istatistikleri alt üst etme...</i>
            </p>
            <p> </p>
        </div>
        <div class="player-card" style="background-color:lightgray">
            <a href="http://www.espn.com/nba/player/_/id/2384">
                <div class="player-name">divayt hauvırd</div>
            </a>
            <p>
                <img class="player-photo" src="howard.gif" />
            </p>
            <p>
                <i class="heigth">206cm</i>
            </p>
            <p>
                <i class="bio">o boyla o fudamental hareketleri yapabiliyor olmak, oyun zekası...</i>
            </p>
            <p> </p>
        </div>
    </p>
</body>

</html>
```

İşte Pug'ın ortaya koyduğu fark bu. "İki resim arasındaki 9 farkı bulun" bulmacasını hatırlar mısınız bilmem ama bu kötü espriyi yapmanın tam yeri ve zamanı sanırım:) Gelin kolları sıvayalım ve bu işi nasıl yaptığımızı kısaca özetleyelim. Uygulamanın bir kısmını şirket kaynaklarını kullaranak Windows tabanlı bir sistem üzerinde yapmış olsam da asıl tecrübe etmek istediğim yer elbette ki West-World topraklarıydı. Ubuntu'nun başına geçtim ve sonradan aşağıdaki ekran görüntüsünde yer alan yapıya dönüşecek klasör ağacını oluşturmaya başladım.

![node_with_pug_2.gif](/assets/images/2018/node_with_pug_2.gif)

public/images klasörü içerisinde basketbol oyuncularına ait fotoğraflar yer alıyor (itinayla taranmışlardır) src/views altında pug uzantılı dosyamız bulunmakta. Bu yazının ilk kısmında paylaşmış olduğumuz içeriğe sahip. Aslında dikkatli bir şekilde okuduğunuzda girinti sistemine dayanan ve bu sayede HTML'in < ve > gibi açısal ayraçlarını geride bırakan bir yazım stili söz konusu olduğunu rahatlıkla anlayabilirsiniz. Klasörde yer alan server.js ve package.json içeriklerini biraz sonra dolduracağız. Nitekim yapmamız gereken bazı ön hazırlıklar var. İlk olarak terminal'den aşağıdaki komutları kullanarak gerekli yüklemeleri yapalım.

```bash
npm init
npm install express --save
npm install pug --save
```

npm init kullanımı ile package.json dosyasının oluşmasını sağlıyoruz. Bu komutu çalıştırdığımızda bazı sorularla da karşılaşırız. Uygulamanın adı, versiyonu, yazarı, test komutu vs...Diğer iki komut express ve pug paketlerinin yüklenmesi için kullanılıyorlar. express paketini web hizmetleri için kullanacağız. Static içeriklerin görüntülenmesi veya belirlenen adrese gelen taleplerin yönetilmesi dışında servis geliştirilmesinde de kullanılabilir. Pug ise az önce sıklıkla değindiğimiz yeni stil HTML şablonunun çalışma zamanında anlaşılabilmesi için gerekli. Bu işlemler sonrasında en azından West-World için ilgili package.json içeriğinin aşağıdaki gibi oluştuğunu ifade edebilirim.

```json
{
  "name": "en.bi.ey",
  "version": "1.0.0",
  "description": "NBA istatistikleri",
  "main": "index.js",
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1",
    "start": "node server.js"
  },
  "keywords": [
    "nba",
    "spor",
    "istatistik",
    "nodejs"
  ],
  "author": "burak selim senyurt",
  "license": "ISC",
  "dependencies": {
    "express": "^4.16.3",
    "pug": "^2.0.3"
  }
}
```

Package.json içerisinde müdahale ettiğimiz bir kısım var. scripts elementine dikkat ederseniz start özelliği için node server.js şeklinde bir tanımlama yaptık. Buna göre uygulamamızı "node server.js" yerine "npm start" terminal komutu ile de çalıştırabiliriz ki genel jargon bu yöndedir.

Gelelim server.js dosyasının içeriğine. Burada da öğrendiğim bir çok şey olduğunu ifade edebilirim. Python, Ruby, Go gibi pek çok programlama ortamındakine benzer bir işleyiş söz konusu tabii ama Node.js açısından düşündüğümde benim için yeni yeni şeyler...

```javascript
var express = require('express');
var app = express();
var port = 65003;

app.use(express.static('public/images'))
app.set('views', './src/views');
app.set('view engine', 'pug');

app.get('/', function (req, res) {
    res.render('index'
        , {
            teams: ['bulls', 'celtics', 'lakers'],
            players: [
                {
                    name: 'sitivin köri',
                    bio: 'benzersiz top tekniği, yüksek şut yüzdesi, sınır tanımaz üçlükleri...',
                    height: "190cm",
                    url: 'http://www.espn.com/nba/player/_/id/3975',
                    photo: 'curry.gif'
                },
                {
                    name: 'löbron ceyms',
                    bio: 'çok güçlü, ani hızlanma, meydan okuma, istatistikleri alt üst etme...',
                    height: "206cm",
                    url: 'http://www.espn.com/nba/player/_/id/1966',
                    photo: 'lebron.gif'
                },
                {
                    name: 'divayt hauvırd',
                    bio: 'o boyla o fudamental hareketleri ​_yapabiliyor olmak, oyun zekası...',
                    height: "206cm",
                    url: 'http://www.espn.com/nba/player/_/id/2384',
                    photo: 'howard.gif'
                }
            ]
        }
    );
});

app.listen(port, function (err) {
    console.log('running server on port ' + port);
});
```

Neler olduğuna kısaca değinelim. Web hizmeti için express modülüne ihtiyacımız var. Bu nedenle bir require bildirimi ile başlıyoruz. epxress modülünü rahat kullanabilmek için app isimli bir değişken tanımlanıyor ve bir express nesnesi örnekleniyor. app.use ve app.set isimli fonksiyon çağırımları önemli. use metodunda static dosyalara hizmet verilmesi için gerekli bildirimleri yapmaktayız. NBA oyuncularının fotoğrafları public/images klasörü altında yer alıyor. Bunları img elementinin src niteliğinde bildirdiğimizde sunucu tarafının ilgili içerikleri bulabilmesi lazım. O nedenle yaptığımız bir bildirim olduğunu ifade edebiliriz (Detaylı bilgi için [şu adrese](https://expressjs.com/en/starter/static-files.html) bakabilirsiniz) İki tane set metot çağrımı söz konusu. İlkinde önyüz içeriklerine nereden bakılacağını, ikincisinde Pug'ın görüntüleme motoru olarak kullanılacağını belirtmekteyiz. Bundan sonra tek yaptığımız root adrese gelecek olan talebi yönetmek.

app.get ('/' metod çağırımındaki callback fonksiyonuna dikkat edelim. Burada index sayfasının ele alınacağı belirtilmekte. Öncesinde View Engine olarak da pug belirtildiğinden index.pug HTML formatına çevrilerek istemciye gönderilecektir. Aslında response değişkeni üzerinden çağırılan render fonksiyonunda yaptığımız tek şey JSON tipinden bir içerik oluşturmak. Lakin buradaki teams ve players isimli özellikler index.pug içerisinde değer kazanmaktalar. Son satırda yer alan listen metodu tahmin edeceğiniz üzere belirtilen port'tan ilgili web hizmetini yayına almak için kullanılıyor.

Biraz da index.pug dosyası içerisine bakalım. Aslında anlaşılması oldukça kolay bir mekanizma söz konusu. Kapatma tag'leri yok, < ve > işaretleri yok. Bunların yerine hiyerarşiyi tanımlayabilmek için girintili bir söz dizimi kullanılıyor. Belki bizler için yeni olabilecek mixin ve for each kullanımları söz konusu. Sizden ricam server.js dosyasında yer alan teams ve players isimli dizilerin pug dosyası içerisinde nasıl ele alındığı incelemeniz. Buna ek olarak bir CSS niteliğini ilgili elemente nasıl bağlıyoruz bakmanızı öneririm. html, head, body, b, p, div gibi elementlerin nasıl kullanıldığını kavradığınızda sizin için de yazmak oldukça kolaylaşacaktır.

Pek tabii pug'ın etkin kullanımı için [bu adresteki](https://pugjs.org/api/getting-started.html) içeriklere göz atmamızda yarar var. mixin olarak oluşturduğumuz div içerisinde oyuncuların bir takım bilgilerini gösteriyoruz. Basit bir şablon oluşturduk aslında. Bu şablonun verisini ise son satırdaki for döngüsü ile doldurmaktayız. Benzer bir döngü takımların bilgilerini birer liste elementi olarak yazdırırken de kullanılmakta (each team in teams kısmı) Yani pug dosyasında HTML haricinde javascript gibi kod parçalarını da kullanabiliriz. Tek değişen yazım formatı dikkat ettiyseniz.

Benim için epey yeni ve farklı bir deneyimdi. Front-end tarafına uzak olmama rağmen pug'ın sunduğu imkanlardan olumlu şekilde etkilendiğimi ifade edebilirim. HTML'e göre okunurluğu çok daha kolay olan bir altyapı sağladığı aşikar. Aslında siz bu örneği çok daha iyi bir noktaya taşıyabilirsiniz. Örneğin oyuncu verilerini gerçek zamanlı bir REST servisinden veya veritabanından çekebilir, önyüz tarafı için Bootstrap gibi daha şık çatılardan yararlanabilirsiniz. Son olarak örneği çalıştırmak için terminalden npm start komutunu vermemiz ve herhangi bir tarayıcıdan localhost:65003 adresine gitmemizin yeterli olacağını belirteyim. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
