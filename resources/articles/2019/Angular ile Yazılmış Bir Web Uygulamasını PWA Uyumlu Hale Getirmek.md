---
title: "Angular ile Yazılmış Bir Web Uygulamasını PWA Uyumlu Hale Getirmek"
pubDate: 2019-06-28 07:00:00
categories:
  - Angular
tags:
  - Angular
  - Bash
  - JavaScript
  - JSON
  - HTTP
  - Caching
  - Dependency Injection
  - GitHub
  - Dependency Management
---

# Angular ile Yazılmış Bir Web Uygulamasını PWA Uyumlu Hale Getirmek
![LesEclaireurs.png](images/LesEclaireurs.png)

Geminin neredeyse tüm seyrüsefer sistemi ve radarı arka arkaya gelen alarm sinyalleri sonrası bozulmuştu. Güney pasifiği terk etmek üzere olan koca tekne en son Arjantin kıyılarına yakın seyrediyordu. Gecenin zifiri karanlığında ilerlerken kaptanın en büyük yol bulma ümitlerinden olan kuzey yıldızı bulutlarla kaplı gökyüzünden saatlerdir görülmüyordu.

Altı kişilik güverte mürettabı normal şartlarda geminin seyri için fazlasıyla yeterliydi. Yaklaşık yirmibin groston ağırlığındaki gemi son teknloji cihazlarla donatıldığı için az sayıda personel ile kıtalar arası seyahat edebiliyordu.

Ama şimdi kaptan ve yardımcısı dışındaki mürettebat geminin çeşitli noktalarına yayılmış, dürbünleriyle bir şeyler arıyordu. Korkutucu olan, yönü belirleyemezlerse kendilerini Antartika güzergahında bulabilecek olmalarıydı. Koca okyanusta bu derecede bir rota sapması işlerin daha da korkunç bir hal almasına neden olabilirdi.

Derken kıç tarafa giden denizcinin sesi duyuldu telsizden. Güverte umuttan daha da fazla bir hisle dolmuştu anında. Gemi, güçlü bir manevra ile geri dönüp ufukta belirmiş ve aralıklarla yanıp sönen ışığa doğru yöneldi. Kaptan mürettabatı tekrar güverteye davet ederken yardımcısına şöyle seslendi "Tanrıya şükür Eric. Sonunda Les Eclaireurs yüzünü gösterdi"

Les Eclaireurs...1920 yılında inşa edilen bu deniz feneri, Arjantinin en güney şehri olarak bilinen Ushuaia'nın yaklaşık 9.3 km doğusundadır. "Dünyanın Ucundaki Fener" olarak da bilinir. Turistlerin popüler uğrak noktası olan ve küçük bir ada üzerinde duran fener tarih boyunca bir çok denizci için yol gösterici olmuştur. Deniz feneri kelimesinin İngilizce karşılığı Lighthouse'dur ve bu kelime [bir cumartesi gecesi çalışması](https://github.com/buraksenyurt/saturday-night-works)nda bana yol göstericilik yapmıştır. Öyleyse gelin derlememize başlayalım (Biraz eski bir araştırma olsa da [dünyanın 10 ünlü feneri için şu yazıya](https://10mosttoday.com/10-most-famous-lighthouses-in-the-world/) bakabilirsiniz. Ben özellikle İskoçya'daki Bell Rock deniz fenerinden çok etkilendim)

PWA (Progressive Web App) tipindeki uygulamalar özellikle mobil cihazlarda kullanılırken sanki AppStore veya PlayStore'dan indirilmiş native uygulamalarmış gibi görünürler. Ancak native uygulamalar gibi dükkandan indirilmezler ve bir web sunucusundan talep edilirler. Https desteği sunduklarından hat güvenlidir. Bağlı olan istemcilere push notification ile bildirimde bulunabilirler. Cihaz bağımsız olarak her tür form-factor'ü desteklerler. Bu uygulama modelinde Service Worker'lar iş başındadır ve sürekli taze kalınmasını sağlarlar. Düşük internet bağlantılarında veya internet olmayan ortamlarda çevrim dışı da çalışabilirler. URL üzerinden erişilen uygulamalar olduklarından kurulum ihtiyaçları yoktur.

Benim bu cumartesi gecesi çalışmasındaki amacım ise gayet basitti. Biraz yabancısı olduğum Angular ile basit bir web uygulaması yazmak ve bunu PWA uyumlu hale getirmek.

Peki bir web sayfasından gelen içeriğin PWA uyumluluğunu nasıl test edebiliriz? Bunun için Google'ın geliştirdiği ve Chrome üzerinde bulunan Lighthouse isimli uygulamadan yararlanabiliriz (Ta taaaa...Hikayeyi bağladım işte) F12 ile açılan Developer Tools'tan kolayca erişilebilen Lighthouse ile o anki sayfa için uyumluluk testleri yapabiliriz. Örneğin kendi blogum için bunu yaptığımda mobile cihazlardaki PWA uyumluluğunun %50 olarak çıktığını gördüm:/ Yarı yarıya uyumsuz. Bu nedir arkadaş ya?

![06_28_credit_1.png](images/06_28_credit_1.png)

Bakalım boş bir uygulama için bu durumu değiştirebilecek miyiz?

## Ön Hazırlıklar

Angular ile ilgili işlemler için command-line interface (CLI) aracından yararlanabiliriz. Yoksa aşağıdaki ilk komutla kurmak lazım tabii. Angular CLI komut satırı bir çok konuda yardımcı olacaktır. Projenin oluşturulması, angular için yazılmış paketlerin kolayca eklenmesi vb...İkinci komutla projemizi hazır şablonla oluşturuyoruz. UI tarafında Material Design kullanmayı öğrenmeye çalışacağım. Bu nedenle proje klasörüne girdikten sonra ng add komutu ile material'ın angular sürümünü de projeye ilave etmemiz lazım (Prebuilt tema seçimini Indigo/Pink olarak bıraktım)

```bash
sudo npm install -g @angular/cli
ng new quotesify
cd quotesify
ng add @angular/material
```

## Kod Tarafı

Kodları mümkün mertebe açıklamalarla desteklemeye çalıştım ancak genel hatları ile önyüz bileşenini değiştirdiğimiz, farklı bir adresle haberleşecek bir servis yazdığımızı ifade edebiliriz. İşe ilk olarak app.module.ts dosyasından başlayalım. app.module.ts dosyasında HTTP çağrılarını yapmamızı sağlayan HttpClientModule modülünü tanımlıyoruz. Böylece HttpClient, ana modüle bağlı tüm bileşen ve servislere enjekte edilebilir (Evet burada da Dependency Injection var. O her yerde:P) Ayrıca UI tarafı kontrolleri için ilgili Material modüllerini de eklememiz lazım. Örnekte Toolbar, Card ve Button kontrollerine ait modülleri ele almaktayız. Kodda geçen diğer modüller zaten şablon ile birlikte gelmiş olanlar.

```javascript
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

/*
UI tasarımında kullanacağımız Material bileşenlerine ait modül bildirimleri
*/
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

/* 
 HttpClientModule'ü burada import ettik.
 Böylece HTTP çağrıları yapabilmemizi sağlayan
 HttpClient nesnesini ana modüle bağlı olan 
 tüm componenetlere enjekte edebiliriz.

 HttpClient'ı arayüze veri döndüren dummy bir API
 servisine Get çağrısı yapmak için kullanacağız.
 */
import { HttpClientModule } from '@angular/common/http';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    NoopAnimationsModule,
    HttpClientModule, //Buraya da eklemeyi unutmamak lazım
    // Aşağıdakilerde Material modülleri için yapılan ilaveler
    MatToolbarModule,
    MatCardModule,
    MatButtonModule,
    // PWA güncellemesi sonrası eklenen Worker Service kaydının yapılması
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production })],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

Bu adımdan sonra ng g service dummy terminal komutunu kullanarak DummyService isimli bir servis sınıfı ekliyoruz. [Şuradaki dummy servis](https://jsonplaceholder.typicode.com/posts) adresinden veri çekip sunmakla görevli bir modül esas itibariyle. Sunmak derken uygulama arayüzündeki bileşenleri besleyecek diyebiliriz.

```javascript
/*
Servisin görevi https://jsonplaceholder.typicode.com/posts adresinden
dummy veri çekmek ve bunu bir Observable nesne olarak sunmak.
*/
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'; // HttpClient nesnesini içeriye constructor üzerinden enjekte edeceğiz
import { Observable } from 'rxjs'; //RxJS kütüphanesinden Observable tipini kullanıyoruz

/*
JSON servisinden dönen öğeleri ifade eden arayüz tanımı.
Post tipini temsilen bazı alanlar içeriyor.
*/
export interface Post {
  userId: number;
  id: number;
  title: string;
  body: string;
}

@Injectable({
  providedIn: 'root'
})

/*
DummyService'i üretmek için komut satırından 
ng g service dummy
komutunu kullandık
*/
export class DummyService {

  // Constructor bazlı dependency injection
  constructor(private httpClient: HttpClient) { }

  /* 
    get metodu Observable tipte bir koleksiyon döndürür 
  */
  get(): Observable<Post[]> {
    var url = "https://jsonplaceholder.typicode.com/posts";
    /*
      url ile belirtilen adrese get talebi gönderiyor
      ve içeriğini Post dizisi olarak alıp
      Observable nesnesiyle geriye dönüyoruz
    */
    return <Observable<Post[]>>this.httpClient.get(url);
  }
}
```

Oluşturulan servisi app.component.ts dosyasında kullanabilmek içinse bir takım eklemeler yapmalıyız.

```javascript
import { Component, OnInit } from '@angular/core';
import {DummyService} from './dummy.service'; // yeni eklediğimiz servisi kullanacağımızı belirtiyoruz
import {Post} from './dummy.service'; //ki Post arayüz tipinide oradan export etmiştik

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

// AppComponent, OnInit metodunu uygulamalı
export class AppComponent implements OnInit {
  title = 'Dummy Posts';
  posts: Array<Post>; // çekilen Post verilerini saklamak için kullanacağımız dizi

  constructor(private dummyService:DummyService){

  }

  /*
  OnInit, Angular bileşeninin yaşam döngüsünde çalışan metodlardan birisi.
  Component oluşturulurken devreye girip ilgili servisten veriyi çeken bir 
  işlevi yürütecek şekilde programlandı.

  OnInit AppComponent bileşeni oluşurken bir seferliğine çağrılır.
  */
  ngOnInit(){
    /*
    Constructor'dan enjekte edilen DummyService örneğini kullanarak
    get metoduna başvuruyor ve Post dizisini çekiyoruz.

    DummyService servisindeki get metodu Observable bir nesne döndürüyor.
    Burada ona abone(subscribe) oluyoruz. Asenkron çalışma durumu söz konusu
    olduğunda servis ilgili veriyi çektiğinde kendisine abone olanları da bilgilendirecektir.
    Yani çekilen Post dizisindeki değişim(bu senaryoda servisten alınması) component'e
    bildirilmiş olacaktır. 
    
    */
    this.dummyService.get().subscribe((data:Array<Post>)=>{
      this.posts=data;
    },(err)=>{
      console.log(err);
    });
  }
}
```

Önyüz bileşeni olarak src/app/app.component.html içeriği de tamamen değiştirildi. Material bileşenlerine yer verildi.

```text
<mat-toolbar>
  <mat-toolbar-row>
    <span>Some dummy posts from universe</span>
  </mat-toolbar-row>
</mat-toolbar>
<main>
  <mat-card *ngFor="let post of posts">
    <mat-card-header>
      <mat-card-title>{{post.title}}</mat-card-title>
    </mat-card-header>
    <mat-card-content>
      {{post.body}}
    </mat-card-content>
  </mat-card>
</main>
```

Toolbar tipinde bir Navigation kontrolü, Post bilgilerini göstermek içinse Card kontrolünden yararlanıyoruz. UI, bağlı olduğu AppComponent içerisindeki posts dizisini kullanıyor. Tüm dizi elemanlarında gezmek içinse *ngFor komutundan yararlanılmakta. Bir özellik değerini arayüzde göstermek istediğimizde {{post.title}} benzeri notasyonlar kullandığımız da gözden kaçmamalı.

## PWA Uyumluluğu için Hazırlıklar

Amacımız uygulamanın PWA uygunluğunu kontrol etmek olduğu için öncelikle onu canlı ortam için hazırlamalıyız (Yani Production Build işlemini yapmamız gerekiyor) Nitekim PWA özelliklerinin bir çoğu geliştirme ortamına dahil edilmemekte. Build işlemi için ng CLI aracını aşağıdaki gibi kullanabiliriz.

```bash
ng build --prod
```

![06_28_credit_2.png](images/06_28_credit_2.png)

Uygulama dist klasörüne build edilmiş olur. Hizmete sunmak için http-server gibi bir araçtan yararlanılabilir. Eğer sistemde yüklü değilse npm ile kurmamız gerekir. İlk komutla bunu yapıyoruz. İkinci terminal komutuysa uygulamayı localhost üzerinden ayağa kaldırmakta.

```bash
sudo npm install -g http-server
cd dist
cd quotesify
http-server -o
```

Bunun sonucu olarak 127.0.0.1:8080 veya 8081 portundan yayın yapılır ve uygulama açılır.

![06_28_credit_3.png](images/06_28_credit_3.png)

Uygulama çalıştıktan sonra F12 ile Audits kısmına gidip 'Run Audit'ile PWA testi başlatılırsa, Lighthouse bize aşağıdakine benzer sonuçlar verecektir (Tabii sizin denediğiniz vakitlerde bu kurallar değişmiş olabilir. O nedenle bilgileri güncellemekte yarar var)

![06_28_credit_4.png](images/06_28_credit_4.png)

PWA uyumluluğu oldukça düşük ki bu zaten şu aşamada beklediğimiz bir şey. PWA uyumlu hale getirmek için neler yapılabilir bakalım.

## İhlal Edilen PWA Kriterleri

Önce hangi kuralların ihlal edildiğinde ve bunların ne anlama geldiğine bir bakalım.

- Uygulamanın HTTPS desteği olmazsa olmazlardandır. Development tarafında sıkıntı olmasa da uygulamayı üretim ortamlarına aldığımızda sertifika tabanlı iletişim sağlanmalıdır.
- Service Worker olmaması sebebiyle offline çalışma ve cache kabiliyetlerinin yanı sıra push notification kabiliyletleri de ortada yoktur. Service Worker, ağ proxy'si gibi bir görev üstlenir ve uygulamanın çektiği öğeler (asset) ile veriyi taleplerden (requests) yakalayıp önbelleğe alma operasyonlarında işe yarar.
- Manifesto dosyasının bulunmayışı ki bu dosyada uygulama adı, kısa açıklaması, icon'lar ve diğer gerekli bilgiler yer alır. Ayrıca manifesto dosyası sayesinde add-to-home-screen ve splash screen özellikleri de etkinleşir.
- Progressive Enhancment desteğinin olmaması da bir PWA ihlalidir. Uygulamanın çağırıldığı tarayıcıya göre ileri seviye özelliklerin kullanılabileceğinin ifade edilmesi beklenmektedir.

## PWA Uyumluluğu için Yapılanlar

Angular tarafında uygulamayı PWA uyumlu hale getirmek için aşağıdaki terminal komutunu çalıştırmak yeterlidir. (Proje klasöründe çalıştırdığımıza dikkat edelim)

```bash
ng add @angular/pwa
```

Komut çalıştırıldığında eksik olan manifesto ve service worker dosyaları eklenir. Ayrıca assets altındaki icon'ların form factor desteği açısından farklı boyutları oluşur.

![06_28_credit_5.png](images/06_28_credit_5.png)

Yeni bir dağıtım paketi çıktığımızda PWA için eklenen Service Worker ve manifesto dosyalarını da görebiliriz.

![06_28_credit_6.png](images/06_28_credit_6.png)

Tekrardan Lighthouse raporunu çektiğimizde aşağıdaki gibi %92lik bir karşılama oranı oluştuğunu görebiliriz. Fena değil ama eksik. Çünkü HTTPS desteğini göremedi.

![06_28_credit_7.png](images/06_28_credit_7.png)

Peki ya kalan HTTPS ihlalini development ortamında nasıl aşabiliriz? Aşabilir miyiz? Eğer buraya kadar gelebildiyseniz bir adım daha ilerleyebilirsiniz sevgili okur;)

## Ben Neler Öğrendim?

[Yirmisekiz numaralı bu cumartesi gecesi çalışması](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2028%20-%20PWA%20with%20Angular)nın da bana kattığı değerli bilgiler oldu elbette. Bunları kabaca aşağıdaki gibi sıralayabilirim.

- Angular CLI'ın (command-line interface) temel komutlarını
- Component'lere servislerin nasıl enjekte edilebileceğini
- Çok basit anlamda Material bileşenlerini arayüzde nasıl kullanabileceğimi
- PWA tipindeki uygulamaların genel karakteristiklerini ve avantajlarını
- PWA ihlallerinin kısaca ne anlama geldiklerini ve tespitinde Lighthouse'un nasıl kullanılabileceğini

Böylece geldik bir maceramızın daha sonuna. Bu yazıda basit bir Angular uygulaması geliştirip bunu Progressive Web App modelinde yayınlanabilecek kıvama getirmeye çalıştık. Umarım sizler için de faydalı bir çalışma olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
