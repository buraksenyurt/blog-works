---
layout: post
title: "Bu Sefer Bir React Uygulamasını Heroku Üzerine Alalım"
date: 2019-10-04 13:00:00 +0300
description: "Sekiz numaralı örnekteki amacım node.js ile çalıştırılan basit bir React uygulamasını Heroku üzerine taşımaktı. React ile node haberleşmesinde express paketini kullanmıştım. Bu paket deneyimlediğim kadarıyla HTTP yönlendiricisi olarak kullanılmaktaydı. React tarafına gelen HTTP taleplerini karşılarken kullanılabilmekte. Diğer yandan React tarafında çok fazla tecrübem olmadığından benim için hala kapalı kutu olma özelliğini taşıyor. Bir nevi ona da merhaba demek istediğim bir çalışma olduğunu ifade edebilirim."
categories:
  - react
tags:
  - react
  - bash
  - javascript
  - postgresql
  - json
  - http
  - python
  - java
  - ruby
  - nodejs
  - concurrency
  - github
---
# Bu Sefer Bir React Uygulamasını Heroku Üzerine Alalım
![hamlett.png](/assets/images/2019/hamlett.png)

Sir [Ken Robinson](http://sirkenrobinson.com/), çocukların hayal güçlerini sınırlayan eğitim sistemini eleştirdiği [TED'in en çok izlenen sunumu](https://www.ted.com/talks/ken_robinson_says_schools_kill_creativity?language=en)nda William Shakespeare ile ilgili güzel bir anektod paylaşır. Konuşmasının ilgili bölümünde profesör onun bir zamanlar yedi yaşında bir çocuk olduğunu dile getirir. Kısa bir an için duraksar ve ne diyeceğini merak eden seyirciye "...Shakespeare'i hiç çocuk olarak düşünmemiştiniz, değil mi?" der:)

Hepimiz onu ünlü İngiliz şair ve yazar olarak bilir Romeo Juliet, Macbeth, Othello ve diğer trajedileri ile hatırlarız. Hatta "olmak yada olmamak, işte bütün mesele bu" sözleri hafızalarımıza kazınmıştır. Ancak çoğumuz onun da bir zamanlar çocuk olduğunu ve bir öğretmenin edebiyat dersine girdiğini düşünmeyiz (Bunu Ken Robinson gayet güzel bir şekilde düşündürtüyor) Onun da hepimiz gibi çocukken kurduğu hayaller olduğunu bu cümleleri duyana kadar da fark etmeyiz. Çok şükür ki [sekiz numaralı çalışma](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2008%20-%20Express%20with%20React%20to%20Heroku)mın içerisinde geçen bir kelime benim onu, onun yardımıyla Sir Ken Robinson'u ve sonrasında da bu güzel anekdotu hatırlamamı sağladı. Nihayetinde Shakespeare'in ölümsüz eserlerinden olan Hamlet'in Heroku tarafından bana önerilmesi işte bu kısa girizgahın hayat bulmasına vesile oldu.

Sekiz numaralı örnekteki amacım node.js ile çalıştırılan basit bir React uygulamasını Heroku üzerine taşımaktı. React ile node haberleşmesinde express paketini kullanmıştım. Bu paket deneyimlediğim kadarıyla HTTP yönlendiricisi olarak kullanılmaktaydı. React tarafına gelen HTTP taleplerini karşılarken kullanılabilmekte. Diğer yandan React tarafında çok fazla tecrübem olmadığından benim için hala kapalı kutu olma özelliğini taşıyor. Bir nevi ona da merhaba demek istediğim bir çalışma olduğunu ifade edebilirim.

[Heroku](https://www.heroku.com/) 2007 yılında işe başladığında sadece Ruby on Rails bazlı web uygulamalarına destek veren bir bulut bilişim sistemiydi ancak Platform as a Service (PaaS) olarak olgunlaştıktan sonra Java, Node.js, Scala, Python, Go, Closure ve benzeri bir çok dil ile geliştirilen uygulmalar için de hizmet vermeye başladı. Aslında heroku üzerindeki ilk denememi 2018 yılında yapmış ve [şöyle bir yazı](https://buraksenyurt.com/post/express-api-hizmetini-heroku-uzerine-tasimak) yazmıştım. Teknolojinin gelişimi düşünüldüğünde aradan yadsınamayacak kadar çok zaman geçmiş diyebilirim. O yüzden bu tip platformlara ara ara dönüş yaparak farklı enstrümanlarla kullanmayı denemek güncel kalmamız açısından önemli. Öyleyse vakit kaybetmeden [sekiz numaralı Cumartesi gecesi çalışması](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2008%20-%20Express%20with%20React%20to%20Heroku)nı derlemeye başlayalım.

## Gerekli Hazırlıklar

Tabii öncelikle Heroku üzerinde bir hesap açmak gerekiyor. Ben gerekli hesabı açtıktan sonra WestWorld (Ubuntu 18.04, 64bit) üzerinde Heroku CLI (command-line interface) kurulumunu da yaptım. Böylece heroku ile ilgili işlemlerimizi terminal komutlarını kullanarak kolayca gerçekleştirebiliriz.

```bash
sudo snap install --classic heroku
```

Kurulum sonrası login olmamız gerekecektir.

```bash
heroku login -i
```

Yukarıdaki terminal komutunu çalıştırdıktan sonra credential bilgileri sorulur (-i parametresini heroku login bilgilerinin kalıcı olması için kullanabiliriz) Heroku tarafı ile iletişimi kurduğumuza göre uygulamanın çatısını oluşturmaya başlayabiliriz. Öncelikle app isimli bir klasör açıp aşağıdaki terminal komutu ile node tarafını başlatalım. Biraz sonra kuracağımız React uygulamamız ile konuşacağı basit node sunucusu bu klasörde yer alacak.

```bash
npm init
```

Bazı yardımcı paketlerimiz var. Bunları şu terminal komutu ile yükleyebiliriz.

```bash
npm i --save-dev express nodemon concurrently
```

express, servis tarafını daha kolay kullanabilmemiz için gerekli özellikleri sunan bir paket. nodemon ile de node.js tarafında yapılan değişikliklerin otomatik olarak algılanması sağlanıyor. Yani uygulamayı tekrar başlatmaya gerek kalmadan kod tarafındaki değişikliklerin çalışma zamanına yansıtılması sağlanabilir. concurrently paketi hem express hem react uygulamalarının aynı anda başlatılması için kullanılmakta. Paket yüklemeleri tamamlandıktan sonra app kök klasörü altında server.js isimli bir dosya oluşturup kodlamasını sonradan tamamlamak üzere çalışmamıza devam edebiliriz.

## React Uygulamasının Oluşturulması

React uygulmasını standart bir hello-world şablonu şeklinde açacağız. Bunun için aşağıdaki terminal komutunu kullanmamız yeterli.

```bash
npm i -g create-react-app
create-react-app fromwestworld
```

> Bu arada create-react-app komutu ile uygulamayı oluştururken şunu fark ettim ki proje adında sadece küçük harf kullanılabiliyor. İlerde değişir mi, siz bunu okurken değişmiş midir, neden böyledir tam bilemiyorum ama bir unix isimlendirme standardından kaynaklıdır diye düşünüyorum (Fikri olan?)

Bir süre geçtikten sonra fromwestworld klasörü içerisinde React için gerekli ne varsa oluşturulduğunu görürüz. Oluşturulan bu şablonu çok fazla bozmadan kullanabiliriz.

Şimdi geliştirme ortamı için fromwestworld klasöründeki package.json içerisine bir proxy tanımı ekleyerek devam edelim. Biraz sonra kodlayacağımız node sunucumuz 5005 numaralı porttan yayın yapacak (geliştirme ortamı için farklı bir portta tercih edilebilir) Bu bildirim ile React uygulmasının geliştirme ortamında konuşacağı servis adresi ifade edilir. Bir başka deyişle React'a gelen HTTP taleplerinin yönlendirileceği adresi belirtiyoruz.

```javascript
"proxy": "http://localhost:5005",
```

Şablonla gelen app.js içeriğini aşağıdaki gibi değiştirebiliriz. İçeriğin şu anda çok fazla önemi yok. Hedefimiz olan Heroku deployment için mümkün mertebe basit kodlar kullanmakta yarar var.

```javascript
import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';

class App extends Component {
  render() {
    return (
      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <p>
            Edit <code>src/App.js</code> and save to reload.
          </p>
          <a
            className="App-link"
            href="https://www.buraksenyurt.com"
            target="_blank"
            rel="noopener noreferrer"
          >
            Bu benim blog sayfamdır :)
          </a>
        </header>
      </div>
    );
  }
}

export default App;
```

React uygulamasının iletişimde olacağı sunucuya ait server.js dosyasını da aşağıdaki gibi yazabiliriz.

```javascript
var express = require('express');
var app = express();
var path = require('path');
var port = process.env.PORT || 5005; //heroku'nun portu veya local geliştirme için belirlediğimiz 5005 nolu port

// statik klasör bildirimini yapıyoruz
app.use(express.static(path.join(__dirname, 'fromwestworld/build')));

//canlı ortamdaysak yani uygulamamız Heroku'ya alınmışsa
if (process.env.NODE_ENV === 'production') {
    // build klasöründen index.html dosyasını alıp get talebinin karşılığı olarak istemciye sunuyoruz
    app.use(express.static(path.join(__dirname, 'fromwestworld/build')));
    app.get('*', (req, res) => {
        res.sendfile(path.join(__dirname = 'fromwestworld/build/index.html'));
    })
}

// Eğer canlı ortamda(heroku'da) değilsek ve amacımız sadece localhost'ta test ise
// index.html'i public klasöründen sunuyoruz
app.get('*', (req, res) => {
    res.sendFile(path.join(__dirname + '/fromwestworld/public/index.html'));
})

// express sunucusunu çalıştırıyoruz
app.listen(port, (req, res) => {
    console.log(`sunucumuz ${port} nolu porttan yayındadır`);
})
```

Temel olarak bulunulan ortama (development veya production) göre public klasöründe yer alan (ve benim üzerinde hiçbir değişiklik yapmadığım) index.html sayfasının sunulması söz konusu. Kod tarafındaki bu değişiklilere ilaveten root klasör olarak düşüneceğimiz app altındaki package.json dosyasındaki scripts kısmını da kurcalamalıyız. Güncel hali aşağıdaki gibi.

```javascript
"scripts": {
"client-install": "npm install --prefix fromwestworld",
"start": "node index.js",
"server": "nodemon index.js",
"client": "npm start --prefix fromwestworld",
"dev": "concurrently \"npm run server\" \"npm run fromwestworld\""
}
```

Bunlardan start haricindekiler npm run arkasına eklenen komutlardır. Örneğin npm start ile node index.js komutu ve dolayısıyla uygulama çalışır. Diğer yandan npm run server ile nodemon devreye alınır ve kodda yapılan değişiklik anında çalışma zamanına yansır. npm run client, sunucuyu başlatmadan react uygulamasını çalıştırma görevini üstlenir. npm run client-install sayesinde ise React uygulaması için gerekli tüm bağımlılıklar ilgili ortama (örnekte Heroku olacaktır) yüklenir. npm run dev ile development ortamı ayağa kalkar ve hem node sunucusu hem de react uygulaması aynı anda başlatılır.

Uygulamayı komple çalıştırmak için app klasöründeyken

```bash
npm run dev
```

terminal komutunu işletebiliriz. concurrently paketi bu noktada bize avantaj sağlamaktadır. Eş zamanlı olarak "npm run server" ve "npm run client" komutlarının işletilmesinde rol alır.

Bu işlem sonrasında önce node sunucusu yüklenir. Sunucu çalışmaya başladıktan sonra React uygulaması tetiklenir ve localhost:3000 nolu porttan ilgili içeriğe ulaşılır. Hatırlayacağınız üzere node sunucusu 5005 numaralı porttan hizmet veriyordu. React uygulamasında yapılan proxy bildirimi, 3000 nolu adrese gelen HTTP taleplerinin arkadaki node sunucusuna yönlendirilmesinde rol alır. Dolayısıyla React uygulaması çalışmaya başladığında oluşan HTTP Get talebi server.js dosyasındaki get metodlarına düşer. Buradaki bildirimlere göre fromwestworld içerisindeki index.html dosyasının sunulması söz konusudur. index.html içeriğinde dikkat edileceği üzere root id'li bir div elementi vardır. Yine dikkat edilecek olursa index.js tarafı da aşağıdaki gibidir (Hiç değiştirmedim)

```javascript
import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import * as serviceWorker from './serviceWorker';

ReactDOM.render(<App />, document.getElementById('root'));

serviceWorker.unregister();
```

ReactDOM.render metoduna dikkat edelim. root isimli DOM elementini yakalayıp buraya app isimli bileşenin basılması sağlanmaktadır. Özetlemek gerekirse React uygulaması kendi içeriklerini sunarken arka plandaki node sunucusu ile anlaşmaktadır. Buna göre server.js tarafında sunucu bazlı materyalleri (veri tabanı, asenkron operasyonlar vb) kullanarak bunların react bileşenlerince ele alınması sağlanabilir.

Çalışmayı gerçekleştiğimde West-World tarafında uygulamanın açılması biraz zaman almıştı. Sizin de benim gibi sebat edip panik yapmadan beklemeniz gerekebilir. İşte çalışma zamanı görüntüleri.

![06_08_credit_1.png](/assets/images/2019/06_08_credit_1.png)

Her şey yolunda giderse localhost:3000 adresinden aşağıdaki içeriğe ulaşabiliriz.

![06_08_credit_2.png](/assets/images/2019/06_08_credit_2.png)

## Uygulamanın Heroku Platformuna Alınması

Öncelikle Heroku üzerinde bir uygulama oluşturmamız gerekiyor. Bunu aşağıdaki terminal komutuyla yapabiliriz.

```bash
heroku create
```

Bana proje adı olarak Heroku'nun otomatik olarak ürettiği frozen-hamlet-75426 denk geldi. Ayrıca uygulama kodlarını atabilmek için github ve ulaşacağım web adresi bilgisi de iletildi.

![06_08_credit_3.png](/assets/images/2019/06_08_credit_3.png)

Uygulamanın web adresi https://frozen-hamlet-75426.herokuapp.com/ şeklinde. github adresi ise https://git.heroku.com/frozen-hamlet-75426.git. Hatta sonuçları Heroku Dashboard üzerinden de görebiliriz (Tabii siz örneği denerken güncel hali Heroku üzerinde olmayabilir. Kendiniz için bir tane yapsanız daha iyi olur)

![06_08_credit_4.png](/assets/images/2019/06_08_credit_4.png)

Uygulama klasöründeki json dosyasında yer alan heroku-postbuild script'i bu aşamada önem kazanıyor. Kodlar git ortamına taşındıktan sonra bir build işlemi gerekiyor. Heroku bu script kısmını ele alıyor.

```text
"heroku-postbuild":"NPM_CONFIG_PRODUCTION=false npm install --prefix fromwestworld && npm run build --prefix fromwestworld"
```

Bu düzenlemenin ardından yazılan kodların geliştirme ortamından github üzerine atılması gerekiyor. Yani Heroku'nun kod deposu olarak github'ı kullandığını ifade edebiliriz. Aşağıdaki komutlarla devam edelim öyleyse.

```bash
heroku git:remote -a frozen-hamlet-75426
git add .
git commit -am 'Heroku React Express örneği eklendi'
git push heroku master
```

Yapılanları aşağıdaki gibi özetleyebiliriz.

Heroku için git remote adresini belirle
Tüm değişiklikleri stage'e al
Değişiklikleri onayla (commit)
ve kodun son halini master branch'e push'la

Kodun github'a alınması aynı zamanda heroku'nun da ilgili uygulamayı gerekli build betiklerini çalıştırarak devreye alması anlamına gelmekte. Dolayısıyla bir süre sonra https://frozen-hamlet-75426.herokuapp.com/ adresine gitmek sonuçları görmemiz açısından yeterli olacaktır.

![06_08_credit_5.png](/assets/images/2019/06_08_credit_5.png)

## Bazı Hatalarım da Olmadı Değil

Çalışma sırasında bu basit hello-world uygulamasını tek seferde Heroku'ya taşıyamadığımı ifade etmek isterim. Eğer taşıma sırasında sorunlarla karşılaşırsanız bunları görmek için terminalden

```bash
heroku logs --tail
```

komutunu kullanabilirsiniz. Yaşadığım sorunları ve çözümlerini aşağıdaki maddelerde bulabilirsiniz.

- İlk hatam server.js dosyasında process.env.PORT yerine process.env.port kullanmış olmamdı. Heroku ortamı bu port'u anlamadığı için 5005 nolu porttan yayın yapmaya çalıştı ki bu mümkün değildi.
- İkinci hatam package.json içerisinde ortam için gerekli node engine versiyonunu söylememiş olmamdı. Nitekim Heroku tarafı node'un hangi sürümünü kullanacağını bilmek ister.
- Diğer problemse bağımlı olunan npm paketleri için package.json dosyasında dependencies yerine devDependencies sektörünü bırakmış olmamdı. Üretim ortamı için dependencies kısmına bakılıyor.
- Ayrıca.gitignore dosyasını koymayıp node_modules ve package-log.json öğelerini hariç tutmadığım için bu klasörleri de komple push'lamış oldum (Sonraki versiyonda düzelttim tabii)

Bu hususlara dikkat ettiğimiz takdirde ürünü başarılı bir şekilde yayına almış oluruz. Tabii uygulamanın şu an için yaptığı hiçbir şey yok. Oysa ki PostgreSQL kullanaraktan veri odaklı basit bir Hello World uygulaması pekala geliştirilebilir. Daha önceden sıkça dile getirdiğim üzere bu kutsal görevi siz değerli okurlarıma bırakıyorum:)

## Ben Neler Öğrendim?

Uzun süre sonra derlemek üzere ele aldığım bu çalışmada Heroku üzerine bir React uygulamasının nasıl alındığını hatırlayıp bilgilerimi tazeleme fırsatı buldum. Kabaca yaptıklarımın üstünden geçtikten sonra öğrendiklerimi aşağıdaki maddeler halinde ifade edebileceğimi düşünüyorum.

- Heroku'da hesap açma ve uygulama oluşturma adımlarını
- Heroku CLI üzerinden dağıtım işlemlerinin nasıl yapıldığını
- Bir node sunucusu üzerinden bir React uygulamasının ayağa kaldırılmasını
- En temel düzeyde app react bileşeninin nerede nasıl kullanıldığını
- Deployment sırasında veya çalışma zamanındaki hatalara ait loglara nasıl bakıldığını

Böylece geldik bir [cumartesi gecesi derlemesi](https://github.com/buraksenyurt/saturday-night-works)nin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.