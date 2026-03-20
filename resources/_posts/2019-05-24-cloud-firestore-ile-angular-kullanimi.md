---
layout: post
title: "Cloud Firestore ile Angular Kullanımı"
date: 2019-05-24 09:00:00 +0300
categories:
  - angular
tags:
  - angular
  - bash
  - xml
  - javascript
  - nosql
  - json
  - typescript
  - concurrency
  - github
---
Earvin (Magic) Johnson. Michael Jordan'la geçen gençlik yıllarımın henüz başlarında rastladığım NBA'in ve Los Angles Lakers'ın 2.06lık unutulmaz oyun kurucusu. O dönemlerde yaptığı inanılmaz assistler ve oyun zekası hala aranır nitelikte. Aslında sadece oyun kurucu değil zaman zaman şutör gard ve uzun forvet pozisyonlarında da oynamıştır.

![magicjohson.png](/assets/images/2019/magicjohson.png)

Lakers tarafından 1979 yılında birinci sırada draft edilen Johnson toplamda 5 NBA şampiyonluğu yaşamış efsanelerden birisi. [NBA istatistiklerine göre](https://stats.nba.com/player/77142/career/) oynadığı 906 maçta 19.5 sayı ve 11.2 assist ortalamaları ile double double yapmıştır. Toplamda 10141 asist ile tüm zamanların en çok asist yapan 5nci oyuncusu durumunda. 32 numaralı formasıyla 12 sezon Lakers'da görev alan oyun kurucunun hayatını sevgili Murat Murathanoğlu'nun eşsiz anlatımıyla dinlemek isterseniz [şöyle buyrun](https://youtu.be/xyxIiLpNDu8). Onun [Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) çalıştayımla olan tek ilgisi ise [forma numarası](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2032%20-%20Angular%20with%20Firebase). Hoş bir giriş olsun istedim de:[]

Gelelim derleyip toparladığım blog notlarıma.

Angular tarafına yavaş yavaş alışmaya başlamıştım. Yine de fazladan idman yapmaktan ve tekrar etmekten zarar gelmez diye düşünüp farklı örnekleri uygulamaya çalışıyordum. Bu sefer temel CRUD (Create Read Update Delete) operasyonlarını Cloud Firestore üzerinden icra ederken Angular'da koşmaya çalışmışım. Amaçlarımdan birisi servis tarafında Form kontrolü kullanabilmek. Örnekte ikinci el eşya satışı yapmak üzere kurgulanan basit bir web arayüzü söz konusu. Programı her zaman olduğu gibi WestWorld (Ubuntu 18.04, 64bit) üzerinde yazdım.

> Google bilindiği üzere 2014 yılında bir bulut servis sağlayıcı olan Firebase'i satın almıştı. Sonrasında bu servisin Web ve Mobil tarafı için kullanılabilen Firestore isimli NoSQL tabanlı veri tabanını kullanıma açtı. Firestore, Realtime Database alternatifi olarak kullanıma sunuldu. Realtime Database'e göre bazı farklılıkları var. Örneğin sadece mobil değil web tarafı için de offline kullanım imkanı sağlıyor. Ölçekleme bulut sisteminde otomatik olarak yapılıyor. Realtime Database'e göre karmaşık sorgu performansının daha iyi olduğu belirtiliyor. Ücretlendirme politikası uygulamanın büyüklüğüne göre Realtime Database'e göre daha ekonomik olabiliyor. Dolayısıyla mobil tarafta ilerleyen Startup projelerinin MVP modelleri için ideal bir çözüm gibi duruyor.

## İlk Hazırlıklar

Tabii konumuz esas itibariyle Angular deneyimini arttırmak. Cloud Firestore bu noktada bir veri sağlayıcısı rolünü üstlenecek. İşe Angular projesini oluşturarak başlayabiliriz. Bir Angular projesini kolayca oluşturmanın en etkili yolu bildiğiniz üzere CLI (Command-Line Interface) aracından yararlanmak. Dolayısıyla sistemimizde Angular CLI yüklü olmalı. Eğer yüklü değilse aşağıdaki ilk terminal komutunu bu amaçla kullanabiliriz.

```bash
sudo npm install -g @angular/cli
ng new quick-auction
npm i --save bootstrap firebase @angular/fire
cd speed-sell
ng g c products
ng g c product-list
ng g s shared/products
```

Takip eden komutlara gelirsek...

ng new ile quick-action isimli yeni bir Angular projesi oluşturmaktayız (Sorulan sorularda Routing seçeneğine No dedim ve Style olarak CSS'i seçili bıraktım. Ancak bunun yerine bootstrap kullanacağız) npm i ile başlayan komutlarda stil için bootstrap, Google'ın Cloud Firestore tarafı ile konuşabilmek içinde firebase ve anglular'ın firebase ile konuşabilmesi içinse @angular/fire paketlerini ekliyoruz. ng g ile başlayan komutlarda iki bileşen (component) ve her iki bileşen için ortaklaşa kullanılacak bir servis nesnesi oluşturuyoruz. Bu servis temel olarak firestore veri tabanı ile olan iletişim görevlerini üstlenecek.

## Firebase Tarafı (Cloud Firestore)

Google Cloud tarafında yapacağımız bazı hazırlıklar var. Firebase tarafında yeni bir proje açıp içerisinde test amaçlı bir Firestore veri tabanı oluşturacağız. Öncelikle b[u adresten](https://console.firebase.google.com/) Firebase Console'a gidelim ve örnek bir proje üretelim. Ben aşağıdaki ekran görüntüsüneki gibi quict-auctions-project isimli bir uygulama oluşturdum (Esasen quick demek istemiştim ama dikkatsizlik olsa gerek quict demişim, olsun. Özgün bir isim olmuş:P)

![04_32_credit_1.png](/assets/images/2019/04_32_credit_1.png)

Sonrasında Database menüsünden veya kocaman turuncu kutucuk içerisindeki Cloud Firestorm bölümünden hareket ederek yeni bir veri tabanı oluşturalım. Aşağıdaki ekran görüntüsünde olduğu gibi veri tabanını Test modunda açabiliriz.

![04_32_credit_2.png](/assets/images/2019/04_32_credit_2.png)

Şimdi Angular uyguaması ile Firebase servis tarafını tanıştırmalıyız. Project Overview kısmından hareket ederek

![04_32_credit_3.png](/assets/images/2019/04_32_credit_3.png)

kırmızı kutucuktaki düğmeye basalım. Gerekli ortam değişkenleri otomatik olarak üretilecektir. Karşımıza gelen ekrandaki config içeriğini uygulamanın environment.ts dosyası içerisine almamız yeterli.

![04_32_credit_4.png](/assets/images/2019/04_32_credit_4.png)

## Kod Tarafı

Gelelim kod tarafında yaptığımız değişikliklere. Arayüz tarafını daha şık hale getirmek için bootstrap kullanıyoruz. Bu nedenle angular.json dosyasındaki style elementini değiştirdik.

```xml
"styles": [
	"node_modules/bootstrap/dist/css/bootstrap.min.css",
        "src/styles.css"
        ],
```

Uygulama, satılacak ürünlerin yönetimi ilgili iki bileşen kullanıyor. Hatırlayacağınız üzere bunları terminalden üretmiştik (products ve product-list) Birisi tipik listeleme diğeri ise ekleme işlemi için kullanılacak. Bu bileşenlere ait HTML ve Typescript kodlarını aşağıdaki gibi geliştirebiliriz. Kodların anlaşılması adına mümkün mertebe yorum satırları kullandım.

products.component.ts

```javascript
import { Component, OnInit } from '@angular/core';
import { ProductsService } from '../shared/products.service'; // ProductsService modülünü bildiriyoruz

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {

  constructor(private productsService: ProductsService) { } // Constructor injection ile ProductsService tipini içeriye alıyoruz
  auctions = []; // açık artırma verilerini tutacağımız array
  ngOnInit() {
  }

  // Bileşendeki button'a basıldığında (click) niteliğine atanan olay bildirimi nedeniyle bu metod çalışacaktır
  onSubmit() {
    let formData = this.productsService.productForm.value; // aslında servis tarafındaki form kontrolü bileşenle ilişkilendirildiğinden girilen değerler oraya da yansır
    console.log(formData); // F12 ile tarayıcı Console penceresinden bu çıktıya bakabiliriz
    this.productsService.addProduct(formData);
  }
}
```

products.component.html

```text
<form [formGroup]="this.productsService.productForm">
  <div class="form-group">
    <label for="lblTitle">Tanıtım başlığı</label>
    <input type="text" formControlName="title" class="form-control" id="txtTitle" placeholder="Tanıtım başlığını giriniz">
  </div>
  <div class="form-group">
    <label for="lblSummary">Açıklaması</label>
    <input type="text" formControlName="summary" class="form-control" id="txtSummary" placeholder="Ne satıyorsunuz az biraz bilgi...">
  </div>
  <div class="form-group">
    <label for="lblPrice">Fiyat</label>
    <input type="number" formControlName="price" class="form-control" id="txtPrice" placeholder="10">
  </div>
  <div class="form-group form-check">
    <input type="checkbox" formControlName="bargain" class="form-check-input" id="chkBargain">
    <label class="form-check-label" for="chkBargain">Pazarlık olur mu?</label>
  </div>
  <button class="btn btn-primary" (click)="onSubmit()">Yolla</button>
</form>
<!--
  form elementindeki [formGroup] niteliğine dikkat edelim. Buraya atanan değer,
  bileşene enjekte edilen ProductsService nesnesine ait form özelliğidir.
  Servis tipinin productForm değişkenindeki alanlar bu bileşen üzerindeki kontrollere
  formControlName niteliği yardımıyla bağlanırlar.
-->
```

product-list.component.ts

```javascript
import { Component, OnInit } from '@angular/core';
import { ProductsService } from '../shared/products.service'; // ProductService modülünü bildiriyoruz

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  constructor(private productService: ProductsService) { } // servisi constructor üzerinden enjekte ettik
  allProducts; // Firestore koleksiyonundaki tüm dokümanları temsil edecen değişkenimiz
  ngOnInit() {
    /*
    Bileşen initialize aşamasındayken servisten tüm ürünleri çekiyoruz.
    Subscribe metoduyla da servisin getProducts metodundan dönen sonuç kümesini,
    allProducts isimli değişkene bağlıyoruz ki bunu bileşenin ön yüzü kullanıyor
    */
    this.productService
      .getProducts()
      .subscribe(res => this.allProducts = res);
  }

  /*
    Bir ürünü silmek için kullandığımız metod. 
    Servis tarafındaki deleteProduct çağrılıyor.
    Parametre olarak o anki product içeriği gönderilmekte
  */
  delete = p => this.productService.deleteProduct(p).then(r => {
    //alert('silindi');
  });

  /*
    Ürünün sadece bargain özelliğini update eden bir metod 
    olarak düşünelim. Senaryoda pazarlık payı olup olmadığını belirten
    checkbox'ın durumunu güncelletiyoruz
  */

  // Güncelleme örneği (fiyatı 10 birim arttırıyoruz)
  increasePrice = p => this.productService.updateProduct(p, 10);

  // Güncelleme örneği (fiyatı 10 birim düşürüyoruz)
  decreasePrice = p => this.productService.updateProduct(p, -10);
}
```

product-list.component.html

{% raw %}
```text
<table class="table">
  <thead class="thead-dark">
    <tr>
      <th>Açıklama</th>
      <th>Başlık</th>
      <th>Fiyat</th>
      <th>Pazarlık?</th>
      <th></th>
    </tr>
  </thead>
  <tbody>
    <tr *ngFor="let product of allProducts">
      <td>{{product.payload.doc.data().summary}}</td>
      <td>{{product.payload.doc.data().title}}</td>
      <td>{{product.payload.doc.data().price}}</td>
      <td>
        {{product.payload.doc.data().bargain?'Var':'Yok'}}
      </td>
      <td>
        <div class="btn-group-sm">
          <button class="primary btn-default btn-block" (click)="increasePrice(product)">+</button>
          <button class="primary btn-default btn-block" (click)="decreasePrice(product)">-</button>
          <button class="primary btn-danger btn-block" (click)="delete(product)">Sil</button>
        </div>
      </td>
    </tr>
  </tbody>
</table>

<!--
  Klasik bir Grid tasarımı söz konusu.
  *ngFor ile bileşenin init metodunda doldurulan allProducts dizisini dönüyoruz.
  Firestore'dan gelen her bir dokümanın elemanlarına ulaşmak için,
  payload.doc.data().[özellik adı] notasyonunu kullandık.

  Sil başlıklı button'a basıldığında bileşendeki delete metodunu çağrılmış oluyor.

  Checkbox kontrolünün click olayında bileşendeki güncelleme metodunu ve dolayısıyla
  servis tarafındaki versiyonunu çağırmış oluyoruz.
-->
```
{% endraw %}

CRUD operasyonları her iki bileşen içinde ortaklaşa kullanılabilecek fonksiyonellikler. Bu nedenle Shared klasörü altında konuşlandırdığımız products.service.ts isimli bir tip mevcut.

```javascript
import { Injectable } from '@angular/core';
import { FormControl, FormGroup } from "@angular/forms"; // FormGroup ve FormControl tiplerini kullanabilmek için eklemeliyiz
import { AngularFirestore } from "@angular/fire/firestore"; // Firestore tarafı ile konuşmamızı sağlayacak modül. Servisini constructor'da enjekte ediyoruz

@Injectable({
  providedIn: 'root'
})
export class ProductsService {

  constructor(private firestore: AngularFirestore) { }

  /* 
  Yeni bir FormGroup nesnesi örnekliyoruz.
  title, summary, price ve online isimli FormControl nesneleri içeriyor.
  bu özelliklere atanan değerleri Firebase tarafına yazacağız.
  element adları arayüz tarafında da birebir kullanılacaklar
  */
  productForm = new FormGroup({
    title: new FormControl(''),
    summary: new FormControl(''),
    price: new FormControl(100),
    bargain: new FormControl(false),
  })

  /*
  Firestore veritabanına yeni bir Product verisi eklemek için kullanılan servis metodu.
  collection ile Firestore tarafındaki koleksiyonu işaret ediyoruz.
  Gelen json içeriği products isimli koleksiyona yazılıyor.
  */
  addProduct(p) {
    return new Promise<any>((resolve, reject) => {
      this.firestore.collection("products").add(p).
        then(res => { }, err => reject(err));
    });
  }

  getProducts() {
    /*
     Firestore veri tabanındaki products koleksiyonu içerisinde yer alan tüm dokümanları alıyoruz.
     snapshotChanges çağrısı değişikliklerin kontrol altında olmasını sağlar. 
     Bizim değişiklikleri yakalayıp güncellemeler yapmamıza gerek kalmaz.
    */

    return this.firestore.collection("products").snapshotChanges();
  }

  // silme işlemini üstlenen servis metodumuz
  deleteProduct(p) {
    return this.firestore
      .collection("products")
      .doc(p.payload.doc.id) // firestrore tarafındaki id bilgisini kullanacak.
      .delete();
  }

  // Güncelleme operasyonu. rate değişkenine gelen değere göre price değerini değiştiriyoruz
  updateProduct(p, rate) {
    // Önce üzerinde çalışılan veriyi alalım.
    var prd=p.payload.doc.data();
    if(prd.price==10 && rate<0) // fiyatı sıfırın altına indirmek istemeyiz çünkü
      return;    
    // Üst limit kontrolü de konulabilir belki

    // fiyat arttırımı veya azaltımı uygunsa yeni değeri alıyoruz ve firestore üzerinden güncelleme yapıyoruz
    var newPrice=prd.price+rate;
    return this.firestore
      .collection("products")
      .doc(p.payload.doc.id)
      .set({ price: newPrice }, { merge: true });
    // merge özelliğine atanan true değeri, tüm entity değerlerinin güncellenmesi yerine sadece metoda ilk parametre ile gelenlerin ele alınmasını söyler.
  }
}
```

Eklediğimiz bileşenleri kullandığımız yer app nesnesi. Angular tarafındaki ana bileşenimiz olarak düşünebiliriz. Dolayısıyla modül bildirimleri ve bileşenlerin HTML yerleşimleri için app.module.ts ve app.component.html dosyalarını da kodlamamız gerekiyor.

app.module.ts

```javascript
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { environment } from "src/environments/environment"; //environment.ts içerisindeki firebaseConfig sekmesinin anlaşılabilmesi için gerekli modül
import { AngularFireModule } from "@angular/fire";
import { AngularFirestoreModule } from "@angular/fire/firestore";

import { AppComponent } from './app.component';
import { ProductsComponent } from './products/products.component';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductsService } from './shared/products.service'; // Tüm bileşenlerde kullanabilmek için ProductsService modülünü bildirip alttaki providers özelliğine de ekledik
import {ReactiveFormsModule} from '@angular/forms'; // Service tarafında FormControl ve FormGroup modüllerini kullanabilmek için bildirdik ve aşağıdaki import kısmında ekledik

@NgModule({
  declarations: [
    AppComponent,
    ProductsComponent,
    ProductListComponent
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    AngularFireModule.initializeApp(environment.firebaseConfig), // AngularFireModule' ü environment.ts içerisindeki firebaseConfig ayarları ile başlatmış olduk
    AngularFirestoreModule
  ],
  providers: [ProductsService], 
  bootstrap: [AppComponent]
})
export class AppModule { }
```

ve app.component.html

```text
<!-- 
  Uygulama hizmete alındığında render edilecek olan ana bileşenimiz.
  ng g c komutları ile oluşturduğumuz products ve product-list bileşenlerini bootstrap grid sistemini kullanarak ekrana yerleştiriyoruz.
  class niteliklerinde kullandığımzı değerler ile ortamı biraz renklendirmeye çalıştım

-->
<div class="container px-lg-5">
  <div class="row">
    <h1>Hızlı Satış?</h1>
  </div>
  <div class="row border border-primary rounded">
    <div class="col py-3 px-lg-3 col-md-12">
      <app-products></app-products>
    </div>
  </div>
  <div class="row border border-dark rounded">
    <div class="col py-3 px-lg-3 col-md-12">
      <app-product-list></app-product-list>
    </div>
  </div>
</div>
```

Son olarak Firestore tarafı için gerekli apiKey, databaseUrl, senderId, projectId gibi bilgilerin environment.ts dosyasına eklenmesi lazım ki çalışma zamanında kullanılabilsinler. Bu bilgileri Google Cloud tarafı otomatik olarak üretmişti hatırlayacağınız üzere.

```javascript
export const environment = {
  production: false,
  // Firebase'in orada oluşturduğumuz projemiz için bize verdiği ayarlar
  firebaseConfig: {
    apiKey: ".....", //Burası sizin projenizin Api Key değeri olmalı
    authDomain: "quict-auctions-project.firebaseapp.com",
    databaseURL: "https://quict-auctions-project.firebaseio.com",
    projectId: "quict-auctions-project",
    storageBucket: "quict-auctions-project.appspot.com",
    messagingSenderId: "....." // Bu da sizin projeniz için verilen senderId değeri olmalı
  },
};
```

Hepsi bu kadar:) Artık uygulamayı çalıştırıp sonuçlarına bakabiliriz.

## Çalışma Zamanı

Uygulamayı çalıştırmak için terminalden

```bash
ng serve
```

komutunu vermemiz yeterli. 4200 numaralı port üzerinden web arayüzüne erişebiliriz. WestWorld testlerinde benim aldığım örnek bir ekran görüntüsünü aşağıda bulabilirsiniz (Hani ispatı olsun da sonra çalışmıyor bu filan demeyelim)

![04_32_credit_5.png](/assets/images/2019/04_32_credit_5.png)

Örneğin ilgi çekici yanlarından birisi, önyüz ve Firestore taraflarının eş zamanlı olarak güncel kalabilmeleridir. Firestore web konsolunda eriştiğimiz dokümanlarda yapacağımız değişiklikler anında önyüz tarafına push edilir, önyüzde yaptığımız değişiklikler de benzer şekilde Firestore tarafına yansır. Bunu denemenizi öneriririm. + ve - düğmeleri ile güncel fiyat bilgisini arttırma veya azaltma işlemlerini yapabiliriz. Sil düğmesi tahmin edileceği üzere satışa çıkarttığımız ürünü repository'den kaldırmak içindir. Güncelleme oparasyonunu sadece fiyat ayarlamaları için yaptık lakin ürün bilgilerinin düzenlenmesi ihtiyacı da var. Bunu uygulamaya nasıl ekleyebiliriz bir düşünün;)

## Ben Neler Öğrendim

Bu örnek çalışma ile Angular bilgilerimi biraz daha pekiştirmiş ama daha da önemlisi veri kaynağı olarak Google Cloud Platform'un bir ürününü kullanmış oldum. Genel hatlarıyla öğrendiklerimi şöyle özetleyebilirim.

- Bir component üzerindeki element değerlerinin formControlName niteliği yardımıyla servis tarafındaki FormControl nesnelerine bağlanabileceğini
- Firebase üzerinde Cloud Firestore veri tabanının nasıl oluşturulabileceğini
- Uygulamanın Firebase tarafı ile haberleşebilmesi için gerekli konfigurasyon ayarlarının nereye konulması gerektiğini ve nasıl çağırılabildiğini
- Cloud Firestore ve önyüzün birbirlerinin değişikliklerini anında görebildiklerini
- Bileşenlerdeki kontrollere olay metodlarının nasıl bağlanabileceğini
- Firestore paketinin temel CRUD (Create Read Update Delete) komutlarını

Böylece geldik bir maceranın daha sonuna. [32 numaralı Saturday-Night-Works çalışmasının kodlarına buradan ulaşabilirsiniz](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2032%20-%20Angular%20with%20Firebase). Yeni bir gözden geçirme yazısında buluşuncaya dek hepinize mutlu günler dilerim.
