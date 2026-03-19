---
title: "Socket-IO Yardımıyla RealTime Çalışan Bir Angular Uygulaması Geliştirmek"
pubDate: 2019-08-08 15:17:00
categories:
  - Angular
tags:
  - Angular
  - Bash
  - JavaScript
  - JSON
  - HTTP
  - TypeScript
  - Node.js
  - Async/Await
  - GitHub
---

# Socket-IO Yardımıyla RealTime Çalışan Bir Angular Uygulaması Geliştirmek
![image.axd](images/image.axd)

Dünyanın aslen hukukçu olmasına rağmen en ünlü matematikçilerinden olan Fermat'nın (1601-1665) asal sayıları bulduğunu iddia ettiği denklemini bir diğer matematikçi Euler (1707-1773), n=5 değeri için bozmuştur. Lakin matematikçilerin ve diğer pek çok kişinin asalları bulma tutkusu bitmemiştir. Bilim, felsefe ve müzikle haşırneşir olmayı seven Fransız rahibi Marin Mersenne (1558-1648) 2n-1 şeklindeki formülü ile ünlenmiştir. Formüldeki n değerinin asal sayı olarak kabul edildiği hallerde bulunan sayıların da asal olduğunun belirtildiği bir teorem söz konusudur (Bu formül ile bulunan bir sayının asal olup olmadığı Lucas-Lehmer testi ile kontrol edilebilir)

Nitekim mesele 1000-2000 arası asalları bulmakla ilgili değildir. En büyük asal değeri bulabilmektir. Çünkü n değeri büyüdükçe en büyük asalı bulmak da zorlaşır (Nadir olan her zaman daha kıymetlidir) Mersenne sayıları olarak adlandırılan bu asalların en kocamanı 2018 yılında elektrik mühendisi Jonathan Pace tarafından keşfedilmiştir. n = 82.589.933 değeri için bulunan 50nci Mersenne asalı tam 24.862.048 rakamdan oluşmaktadır (Ocak 2019 itibariyle) Dilerseniz 51nci Mersenne asalını bulmak için siz de katkıda bulunabilir hatta bulursanız küçük bir ödül bile alabilirsiniz. [Şu adrese girip](https://www.mersenne.org/) GIMPS (Great Internet Mersenne Prime Search) sistemine gönüllü olarak katılmanız yeterli.

Lakin hangi formül olursa olsun çıkan sonucun asal sayı olacağının garantisi veya ispatı henüz yoktur (May Be Prime!) Hatta dünyadaki tüm asal sayılarının dizisini bize getirebilecek bir denklem de henüz mevcut değildir. Peki bugünkü konumuzun Mersenne asalları ile bir ilgisi var mı dersiniz? Bu cumartesi gecesi derlemesinin 29ncu çalışmaya ait olması haricinde pek yok;)

Bilindiği üzere istemci-sunucu geliştirme modelinde gerçek zamanlı ve çift yönlü iletişim için WebSocket yaygın olarak kullanılan protokollerden birisi. Klasik HTTP request/response modelinden farklı olarak WebSocket protokolünde sunucu, istemcinin talep göndermesine gerek kalmadan mesaj gönderebiliyor. Chat uygulamaları, çok kullanıcılı gerçek zamanlı oyunlar, finansal bildirim yapan ticari programlar, online doküman yönetim sistemleri ve benzerleri WebSocket protokolünün kullanıldığı ideal ortamlar. Benim [29 numaralı Saturday Night Works çalışması](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2029%20-%20Real%20Time%20App%20with%20Angular)ndaki amacım Socket.IO kütüphanesinden yararlanan bir Node sunucusu ile Angular'da yazılmış bir web uygulamasını WebSocket protokolü tabanında deneyimlemekti. Hazırsanız notlarımızı toparlamaya başlayalım.

Öncelikle örneğimizde neler yapacağımızdan bahsdelim. Kullanıcıların aynı doküman üzerinde ortaklaşa çalışabileceği bir örnek geliştirmeye çalışacağız. İstemciler yeni bir doküman başlatabilecek. Dokümanların tamamı tüm kullanıcılar tarafından görülebilecek ve yazılanlar her istemcinin penceresine yansıyacak. Bir nevi ortak dashboard üzerindeki post-it'lerin herkes tarafından düzenlenebildiği bir ortam gibi düşünebiliriz. Ben örneği her zaman olduğu gibi WestWorld (Ubuntu 18.04, 64bit) üzerinden denemeye çalışıyorum. Bu arada makinenizde node, npm, angular CLI'ın yüklü olduğunu varsayıyorum.

## Uygulamanın İnşası

Uygulama iki önemli parçadan oluşuyor. Soket mesajlaşmasını yönetecek olan sunucu (node.js tarafı) ve istemci (Angular tarafı) Sunucu tarafının inşası için aşağıdaki terminal komutları ile işe başlayabiliriz.

```bash
mkdir docserver
cd docserver
mkdir src
npm init
npm install --save-dev express socket.io @types/socket.io
cd src
touch app.js
```

Bize yardımcı olacak sunucu ve soket özellikleri için bir epxress ve socket.io paketlerini yüklüyoruz. app.js dosyasının içeriğini ise aşağıdaki gibi geliştirebiliriz.

```javascript
/*
    sunucu özelliklerini kolayca kazandırmak için express modülünü kullanıyoruz.
    WebSocket kullanımı içinse socket.io paketi dahil ediliyor
*/
const app = require('express')();
const http = require('http').Server(app);
const io = require('socket.io')(http);

const articles = {}; // Üzerinde çalışılacak yazıların tutulacağı repo. Canlı ortamlar için fiziki alan ele alınmalı.

/* 
on metodları birer event listener'dır. İlk parametre olayın adı,
ikinci parametrede olay tetiklendiğinde çalışacak callback fonksiyonudur.

connection Socket.IO için tahsis edilmiş bir olaydır. 
Burada soket haberleşmesi tesis edilir ve bağlı olan
istemciler için broadcasting başlatılır.
*/

io.on("connection", socket => {

    /*
    updateRoom metodu bağlı olan tüm istemcilerin aynı doküman üzerinde çalışmasını garanti etmek içindir.
    İstemci bağlantı gerçekleştirip dokümanla çalışmak üzere bir odaya bağlanır(room).
    Bağlı olan istemci bu odadayken başka bir dokümanla çalışmasına izin verilmez.
    N sayıda istemci aynı odadayken aynı doküman üzerinde güncelleme yapabilir.
    İstemci bir başka dokümanla çalışmak isterse bulunduğu odadan ayrılır ve yeni bir tanesine katılır.
    Tabii Socket.IO ile n sayıda oda(room) ile çalışmak mümkündür. Ancak bu senaryoda istenmemektedir.
    */
    let preId;
    const updateRoom = currentId => {
        socket.leave(preId);
        socket.join(currentId);
        preId = currentId;
    };

    /*
    istemci get isimli bir olay yayınladığında çalışır.
    istemci bir odaya gelen id ile dahil edilir.
    sonrasında sunucu dokümanı istemciye yollar. 
    Bunun için ready isimli bir olay yayınlar ki istemci de bu olayı dinlemektedir.
    */
    socket.on("get", id => {
        console.log("get event id: " + id);
        updateRoom(id);
        socket.emit("ready", articles[id]);
    });

    /*
    add yeni bir dokümanın eklenmesi için kullanılır.
    istemci tarafından yayınlanan olayda payload olarak
    dokümanın kendisi gelir. 

    io üzerinden yayınlanan warnEveryone isimli olay
    istemcilerin tümünü yeni bir dokümanın eklendiği bilgisini vermek üzere tasarlanmıştır.

    socket üzerinden yapılan olay bildirimi payload dokümanı ile birlikte sadece bağlı
    olan istemci için geçerlidir. 

    socket ile io nesnelerinin emit kullanımları arasındaki farka dikkat edelim.
    io.emit bağlı olan tüm istmecileri ilgilendirirken, socket.emit o anki olayın
    sahibi bağlı olan istemciyi ilgilendirir.
    */
    socket.on("add", payload => {
        articles[payload.id] = payload;
        updateRoom(payload.id);
        console.log("add event " + payload.id);
        io.emit("warnEveryone", Object.keys(articles));
        socket.emit("ready", payload);
        console.log(articles);
    });

    /*
    İstemcilerin üzerinde çalıştıkları dokümanda yaptıkları herhangibir tuş darbesi
    bu olayın tetiklenmesi ile ilgilidir.
    Payload içeriğine göre odadaki doküman güncellenir ve
    sadece bu doküman üzerinde çalışanların bilgilendirimesi sağlanır.
    */
    socket.on("update", payload => {
        //console.log("update event");
        articles[payload.id] = payload;
        socket.to(payload.id).emit("ready", payload);
    });

    // Tüm bağlı istemcileri template dizisindeki key değerleri için bilgilendir
    io.emit("warnEveryone", Object.keys(articles));
});

http.listen(5004);
console.log("Ortak makale yazma platformu :P 5004 nolu porttan dinlemede...");
```

Kodu içerisindeki yorumlar ile mümkün mertebe açıklamaya çalıştım. Buraya kadar her şey yolunda gittiyse istemci uygulamanın inşası ile devam edebiliriz.

## İstemcinin (Angular tarafı) İnşası

Soket yöneticisi ile konuşacak olan istemciyi bir Angular uygulaması olarak geliştireceğiz. İşe aşağıdaki terminal komutları ile başlayabiliriz.

```bash
ng new authorApp --routing=false --style=css
cd authorApp
sudo npm install --save-dev ngx-socket-io
ng g class article
ng g component article-list
ng g component article
ng g service article
```

İlk komutla authorApp isimli bir Angular uygulaması oluşturulur. Socket.IO ile Angular tarafında konuşmamızı sağlayacak ngx-socket-io paketi proje klasörü içindeyken npm yardımıyla yüklenir. Yine aynı klasörde article isimli sınıf, article-list ve article isimli bileşenler ve soket sunucusuyla iletişimde kullanacağımız article isimli servis oluşturulur (g sonrasında gelen component ve service anahtar kelimeleri için c ve skısaltmaları da kullanılabilir)

![image.axd](images/image.axd)

Gelelim istemci tarafındaki kodlarımıza. Öncelikle app.module.ts dosyasında SocketIoModule ile ilgili bir kaç konfigurasyon ayarlaması yapalım. Böylece hangi sunucu ile web socket haberleşmesi yapılacağı tüm modüller için ayarlanmış olur.

```javascript
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { ArticleListComponent } from './article-list/article-list.component';
import { ArticleComponent } from './article/article.component';
import { FormsModule } from '@angular/forms';
/*
  Angular tarafından socket haberleşmesi için gerekli modül
  bildirimleri. Web Socket sunucusunun adresi de konfigurasyon bilgisi olarak tanımlanmakta.
*/
import { SocketIoModule, SocketIoConfig } from 'ngx-socket-io';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
const config: SocketIoConfig = { url: 'http://localhost:5004' };

@NgModule({
  declarations: [
    AppComponent,
    ArticleListComponent,
    ArticleComponent
  ],
  imports: [
    BrowserModule,
    // Üstte belirtilen url bilgisi ile birlikte socket modülünü hazır hale getirip içeri alıyoruz
    SocketIoModule.forRoot(config),
    BrowserAnimationsModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

Odalardaki makaleleri temsil eden article.ts sınıfını da şöyle yazabiliriz.

```javascript
/*
    Ortaklaşa çalışılacak dokümanı temsilen kullanılacak tip
*/
export class Article {
    id: string;
    content: string;
}
```

Pek tabii asıl iş yükü proxy sınıfı görevi üstlenen article.service.ts içerisinde. Socket sunucusu ile haberleşecek ve arayüz tarafında kullanacağımız servis kodlarını aşağıdaki gibi geliştirebiliriz.

```javascript
import { Injectable } from '@angular/core';
import { Socket } from 'ngx-socket-io';  // Socket sunucusuna event fırlatıp yakalayacağımız için
import { Article } from '../app/article'; //Article tipini kullanacağımız için

@Injectable({
  providedIn: 'root'
})
export class ArticleService {

  /*
   Socket sunucusundan yayınlanan ready ve warnEveryOne isimli olaylar için kullanacağımız özellikleri tanımlıyoruz.
   
   Sunucu tüm istemcilere makale listesini string array olarak gönderirken warnEveryOne olayını yayınlamakta.
   Doküman ekleme, güncelleme ve tek birisini çekme işlemlerine karşılık olarak da ready olayını yayınlıyordu.
   
   fromEvent dönüşleri Observable tiptedir. Yani değişiklikler otomatik olarak abonelerine yansıyacaktır. 
*/
  currentArticle = this.socket.fromEvent<Article>('ready');
  allOfThem = this.socket.fromEvent<string[]>('warnEveryone');

  constructor(private socket: Socket) { } //Constructor injection ile Socket modülünü yükledik

  /*
  Boş bir doküman üretmek için kullanılıyor.
  emit metodu add olayını tetiklemekte. 
  Sunucuya ikinci parametrede belirtilen içerik gönderiliyor.

  emit metodlarındaki ilk parametrelerdeki olaylar sunucunun dinlediği olaylardır.
  */
  add() {
    let randomArticleName = Math.floor(Math.random() * 1000 + 1).toString();
    this.socket.emit('add', {id: randomArticleName,content:'' });
    // console.log(this.allOfThem.forEach(a=>console.log(a)));
  }

  /*
  makale içeriğinin güncellenmesi halinde sunucu tarafına update olayı basılır
  */
  update(article:Article){
    this.socket.emit('update',article);
  }

  /*
  id değerine göre bir makaleyi almak için get olayını fırlatıyor.
  */
  get(id:string){
    this.socket.emit('get',id);
  }
}
```

Ön yüz tarafında daha çok bileşenleri kodlayacağız. article ve article-list bileşenleri ana bileşen olan app içerisinde kullanılmaktalar. Tüm bu bileşenlere ait html ve typescript içeriklerini aşağıdaki gibi düzenleyebiliriz.

article-component.html

```text
<textarea [(ngModel)]='article.content' (keyup)='updateArticle()' placeholder='Haydi bir şeyler yazalım...'></textarea>
<!--textarea'yo ngModel niteliği ile arka plandaki article sınıfının content özelliğine bağlıyoruz
    keyup olayı parmağımızı her tuştan çektiğimizde çalışacak ve bileşenin typescript tarafındaki updateArticle
    metodunu çağıracak.
-->
```

article-component.ts

```javascript
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ArticleService } from 'src/app/article.service';
import { Subscription } from 'rxjs';
import { Article } from 'src/app/article';
import { startWith } from 'rxjs/operators';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.css']
})

export class ArticleComponent implements OnInit, OnDestroy {
  article: Article;
  private _subscription: Subscription;

  /*
  ArticleService, Constructor Injection ile içeriye alınır.
  */
  constructor(private ArticleService: ArticleService) { }

  /*
  Bileşen initialize edilirken güncel makale için bir abonelik başlatılır.
  Böylece gerek bu aboneliğin sahibinin değişiklikleri
  gerek diğerlerinin değişiklikleri aynı makalede çalışan herkese yansır.
  */
  ngOnInit() {
    this._subscription = this.ArticleService.currentArticle.pipe(
      startWith({ id: '', content: 'Var olan bir makaleyi seç ya da yeni bir tane oluştur' })
    ).subscribe(a => this.article = a);
  }

  // Bileşen ölürken üzerinde çalışan makalenin aboneliğinden çıkılır
  ngOnDestroy() {
    this._subscription.unsubscribe();
  }

  /*
   Arayüzdeki keyup olayı ile bağlanmıştır
  Yani tuştan parmak kaldırdıkça servise bir güncelleme olayı fırlatılır 
  ki bu tüm abonelerce alınır.
  */
  updateArticle() {
    this.ArticleService.update(this.article);
  }
}
```

article-list.component.html

```text
<div>
  <button (click)='newArticle()'>Yeni makale başlat</button>
</div>
<div style="height: 100%;">
  <span *ngFor='let a of articles | async' (click)='getArticle(a)'>
      <b>{{a}}</b>  
      <br/>
  </span>
</div>
<!--
  *ngFor ile Typescript tarafındaki articles isimli diziyi dönüyoruz.
  Her bir elemanı için {{a}} ile dizi elemanını basıyoruz ki bu içerde
  rastgele üretilen dosya numarası oluyor.

  click olayı tetiklendiğinde dosya numarasının içeriğini çeken getArticle metodu
  çağırılıyor ki o da article-list.component.ts içerisinde yer alıyor.

  button kontrolüne basıldığındaysa yine article-list.component.ts içerisindeki
  newArticle metodu çağırılıyor.
-->
```

artcile-list.component.ts

```javascript
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ArticleService } from 'src/app/article.service';

@Component({
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css']
})

/*
Bileşen, OnInit ve OnDestroy fonksiyonlarını implemente ediyor.
Yani bileşen oluşturulurken ve iade edilirken yaptığımız bağzı işlemler var.

Init'te güncel makale listesi için bir stream açılmakta ve o an üzerinde çalışılan 
makale için bir abonelik başlatılmakta. Destroy metodunda ise üzerinde çalışılan makalenin aboneliğinden çıkılmakta.

articles değişkeni Observable tipinden bir string dizisi ve servisin allOfThem 
özelliği ile ilişkilendirilip bir stream oluşması sağlanıyor.

Bileşen üzerinden socket sunucusuna fırlatılan olayların karşılığından fırlatılan olaylar,
Observable değişkenin güncel kalmasını sağlayacaktır.
*/
export class ArticleListComponent implements OnInit, OnDestroy {

  articles: Observable<string[]>;
  currentArticle: string;
  private _subscription: Subscription;

  constructor(private articleService: ArticleService) { }

  ngOnInit() {
    this.articles = this.articleService.allOfThem;
    this._subscription = this.articleService.currentArticle.subscribe(a => this.currentArticle = a.id);
  }

  ngOnDestroy() {
    this._subscription.unsubscribe();
  }

  // id değerine göre makale çekilmesi için gerekli sunucu olayını tetikler
  getArticle(id: string) {
    this.articleService.get(id);
  }

  // Yeni bir makale oluşturulması için gerekli olayı tetikler
  newArticle() {
    this.articleService.add();
  }
}
```

app.component.html (HTML tablosunun üst tarafında article-list, alt satırında ise article bileşenlerini gösterecektir)

```text
<table>
  <tr>
    <td>
      <app-article-list></app-article-list>
    </td>
    <td style="width: 200px;">
      <app-article></app-article>
    </td>
  </tr>
</table>
```

app.component.ts

```javascript
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'authorApp';
}
```

Sunucu ve istemci tarafı uygulamalarımız artık hazır. Çalışma zamana geçip testlerimize başlayabiliriz.

## Çalışma Zamanı

İstemcilerin dokümanlar üzerinde çalışmasını sağlamak için öncelikle node sunucusunu ayağa kaldırmamız gerekiyor. Bunu için

```bash
npm run start
```

komutunu kullanabiliriz. İstemci tarafını çalıştırmak içinse,

```bash
ng serve
```

terminal komutundan yararlanılabilir.

Servis localhost:4200 nolu port'tan ayağa kalkar. Bu zorunlu değildir ve isterseniz geliştirme ortamı için angular.json dosyasındaki serve kısmına yeni bir options elementi olarak port bilgisi ekleyebilirsiniz veya ng komutu ile --port anahtarını kullanabilirsiniz.

```bash
ng serve --port 4003
```

gibi.

Örneği daha iyi anlamak için iki veya daha fazla istemci çalıştırmakta yarar var. Bir istemcide yeni bir sayfa açıp üzerinde yazarken diğer istemcide de aynı dosya numarası görünür ve değişiklikler karşılıklı olarak taraflara yansır. Yani Cenifır'ın 399 nolu dokümanda yaptığı değişikliği aynı dokümana bakan Brendon görebilir ve üstüne kendi değişikliklerini yazıp bunları Jenifer'ın görmesini sağlayabilir. Chat uygulaması gibisinden ama değil gibi...

![image.axd](images/image.axd)

Tasarım gerçekten çok kötü ancak amaç Socket.IO'nun Angular tarafında nasıl kullanılabileceğini anlamak olduğu için bir kaç fikir vermiş olmalı. En azından bana verdi ve aşağıda yazdığım maddelerdeki bilgileri öğrendiğimi ifade edebilirim.

## Ben Neler Öğrendim?

- WebSocket protokolünün Node.js tarafında Socket.IO paketi yardımıyla nasıl kullanılabileceğini
- emit ile bağlı istemciye ya da tüm istemcilere canlı yayının (broadcasting) nasıl yapılabileceğini
- on, event olay dinleyicilerinin ne işe yaradığını
- ng komutları ile proje oluşturulmasını, class, component ve service öğelerinin eklenmesini
- Angular component'lerinin bir üst component içerisinde nasıl kullanılabileceğini
- Bileşenlerin HTML tabanlı ön yüzünden, Typescript tarafındaki enstrümanlara (metod, property vb) nasıl ulaşılabileceğini

Böylece geldik bir cumartesi gecesi çalışmasına ait derlemenin daha sonuna. Bu derlememizde Angular ile yazılmış istemcilerin Web Socket üzerinden bir birleriyle nasıl haberleşebileceğini incelemeye çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
