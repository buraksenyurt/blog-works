---
layout: post
title: "Angular ile Basit Bir Tahmin Oyunu Yazmak"
date: 2019-05-01 17:00:00 +0300
categories:
  - angular
tags:
  - angular
  - bash
  - javascript
  - json
  - typescript
  - github
---
Commodore 64 sahibi olduğum günlerde beni çok etkileyen bir Futbol oyunu vardı. Üstelik yerli malıydı. Görsel bir arabirimi yoktu. Komut satırından size sorulan sorulara verdiğiniz cevaplara göre Türkiye birinci futbol liginde maçlar yapıyordunuz. Açılışta takımınızı ve rakibinizi seçtikten sonra yazı tura sorusu ile başlıyordu her şey. Kazandıysanız da "top mu, kale mi" sorusuyla devam ediyordu. Maçın süresi ilerledikçe komut satırından sorular gelmeye devam ediyordu. "Rakip ceza sahasının gerisinde şut çekti. Kaleciniz ne yapacak?" Ve seçenekler geliyordu. "Plonjon, out'a çelme vs" Yapılan seçime göre gol yiyebilir, topu çelebilir veya tutabilirdiniz. İsmini bir türlü hatırlayamadığım ama komut satırından olsa bile beni saatlerce monitör başına kitleyen bir oyundu. Zaten o devrin Commodore 64 oyunlarındaki yaratıcılık, programlama kabiliyetleri bir başkaydı. Bu düşünceler ışığında günlerden bir gün Angular tarafı ile ilgili [saturday-night-works çalışmalarımı](https://github.com/buraksenyurt/saturday-night-works) yapmaktayken bende basit ama bana keyif verecek bir oyun yazayım istedim.

![com64k.jpg](/assets/images/2019/com64k.jpg)

Esasında Angular tarafında çok deneyimli değildim. Eksiğim çoktu. Onu daha iyi tanımak için bol bol örnek yapmam gerekiyordu. Bilgilerimi pekiştirmek için farklı öğretileri uygulamaya devam ediyordum. Bu kez temelleri basit şekilde anlamak adına bir şehir tahmin oyunu yazmaya karar verdim. Uygulama havanın rastsal durumuna göre kullanıcısına bir soru soracak ve hangi şehirde olduğunun bulmasını isteyecek. Kabaca şu aşağıdaki cümleye benzer bir düşünce ile yola çıktığımı söyleyebilirim.

"Merhaba Burak. Bugün hava oldukça 'güneşli've ben kendimi bir yere ışınladım. Neresi olduğunu tahmin edebilir misin?"

'güneşli'yazan kısım rastgele gelecek bir kelime. Yağmurlu olabilir, sisli olabilir vb...Buna göre uygun şehirlerden rastgele birisine gidecek bilgisayar. Biz de bunu tahmin etmeye çalışacağız. Tabii tahmini kolaylaştırmak için minik bir ipucu vereceğiz. Baş harfini söyleyeceğiz (ki siz bunu daha da zenginleştirebilirsiniz. Tahmin sayısını tutup belli bir oranda hak tanıyabilir, tahmin edemedikçce daha fazla harf çıkarttırabilirsiniz)

Öyleyse vakit kaybetmeden işe koyulalım değil mi? Ben örneği artık sonbaharını yaşamakta olan WestWorld (Ubuntu 18.04, 64bit) üzerinde geliştirdim.

## Ön Gereksinimler ve Kurulumlar

Sisteminizde angular CLI yüklü olursa iyi olur. Komut satırından angular projesi başlatmak için işimizi oldukça kolaylaştıracaktır. Sonrasında boilerplate etkisi ile uygulamayı oluşturabiliriz. Arayüzün şık görünmesini sağlamak için (ben ne kadar şıklaştırabilirsem artık:D) bootstrap'i tercih edebiliriz. Aşağıdaki terminal komutları gerekli yükleme işlemlerini yapacaktır. İlk komutla angular CLI aracını yüklerken, ikinci komutla yeni bir angular projesi oluşturuyoruz. Son terminal komutuyla da bootstrap'i projemize dahil ediyoruz. Hepsi Node Package Manager yardımıyla gerçekleştirilmekte.

```bash
sudo npm install -g @angular/cli
ng new where-am-i --inlineTemplate
cd where-am-i
npm install bootstrap --save
```

## Yapılan Değişiklikler

Uygulama kodlarında değişiklik yaptığım çok az yer var. Malum boilerplate etkisi ile zaten hazır bir proje şablonu üretilmiş durumda. Biz temel olarak bir bileşen oluşturup bunu ana sayfada kullanıyoruz.

Bootstrap'i kullanabilmek için proje klasöründeki angular.json dosyasındaki styles elementine ilave bir bildirim yaptık. Buna ek olarak src/app klasöründeki app.component.html dosyasını aşağıdaki gibi değiştirdik (Size yardımcı olacak bilgiler kodların yorum satırlarında yer alıyor. Direkt copy-paste yapmadan önce okuyun)

{% raw %}
```text
<!--
  bootstrap css stilleri ile donattığımız basit bir arayüzümüz var.

  app.component sınıfındaki property'lere erişmek için {{propertyName}} notasyonu kullanılıyor.
  Yine bileşen üzerinde bir metod çağrısı yapmak ve bunu bir kontrol olayı ile ilişkilendirmek için 
  (eventName)="method name" şeklinde bir notasyon kullanılıyor.
  
  Angular direktiflerinde *ngIf komutunu kullanarak tahmine göre bir HTML elementinin gösterilmesi sağlanıyor.
-->
<div class="container">
  <h2>Bil bakalım hangi şehre gittim? :)</h2>

  <div class="card bg-light mb-3">
    <div class="card-body">
      <p class="card-text">Bugün hava <b>{{currentWeather}}</b> ve ben ... şehrine gittim.</p>
    </div>
  </div>
  <div>
    <p>
      <button class="btn btn-primary btn-sm" (click)="fullThrottle()">Hey Scotty. Beni yeniden
        ışınla</button>
    </p>
  </div>
  <div>
    <label>Tahminin nedir?</label>
    <input (input)="playersGuess=$event.target.value" type="text" />
    <button class="btn btn-primary btn-sm" (click)="checkMyGuess()">Dene</button>
  </div>
  <div>
    <p *ngIf="guessIsCorrect" class="alert alert-success">Bravo! Yakaladın beni</p>
    <p *ngIf="!guessIsCorrect" class="alert alert-warning">Tüh. Tekrar dener misin?</p>
    <p class="text-info">İşte sana bir ipucu. {{hint}}</p>
  </div>
</div>
```

Son olarak src/app klasöründeki app.component.ts typescript dosyasındaki bileşen sınıfının değiştirildiğini ifade edebilirim.

```javascript
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'Şimdi Hangi Şehirdeyim?';
  currentWeather: string; // Güncel hava durumu bilgisini tutan property
  computersLocation: string; //Bilgisayarın yerini tutacak property
  playersGuess: string; // Oyuncunun tahminini tutacak property
  guessIsCorrect: boolean; // Tahminin doğru olup olmadığını tuttuğumuz property
  hint:string; // Tahmini kolaylaştırmak için verdiğimiz ipucunu tutan property

  // Örnek veri dizileri. 
  // TODO: Daha uygun bir key-value dizisi bulunabilir mi?

  airConditions = ['güneşli', 'yağmurlu', 'karlı', 'sisli'];
  cities = [
    ['Barcelona', 'Madrid', 'Lima', 'Rio', 'Miami', 'Sydney', 'Antalya'],
    ['Prag', 'Paris', 'Tokyo', 'Dublin', 'Londra', 'Pekin'],
    ['Moskova', 'Montreal', 'Boston', 'Ağrı'],
    ['London', 'Glasgow', 'Mexico City', 'Frankfurt', 'İstanbul']
  ];

  /*
  Uygulama button bağımsız ilk başlatıldığında da hava tahmini yapılsın ve şehir tutulsun.
  */
  constructor() {
    this.hint = "";
    this.computersLocation="";
    this.currentWeather="";
    this.fullThrottle();
  }
  /*
  Bilgisayar için rastgele hava durumu üreten fonksiyon
  Random fonksiyonundan yararlanıp uygun aralıklarda rastgele sayı üretir
  ve buna göre rastgele bir şehir tutar.
  */
  fullThrottle() {
    // hava durumlarını tutan dizinin boyutuna göre rastgele sayı ürettik
    var rnd1 = Math.floor((Math.random() * this.airConditions.length));
    // rastgele bir hava durumu bilgisi aldık
    this.currentWeather = this.airConditions[rnd1];

    // şehirlerin tutulduğu dizide, hava durumu bilgisine uyan (örnekte indeks sırası) dizinin uzunluğunu aldık
    var arrayLength = this.cities[rnd1].length;
    // uzunluğuna göre rastgele bir sayı ürettik
    var rnd2 = Math.floor((Math.random() * arrayLength));
    // üretilen rastgele sayıya göre diziden bir şehir adı aldık
    this.computersLocation = this.cities[rnd1][rnd2];

    this.hint="Baş harfi "+this.computersLocation[0];

    console.log(this.computersLocation); // Şşşşttt. Kimseye söylemeyin. F12'ye basınca ışınlanan şehri görebilirsiniz.
  }

  /*
  Oyuncunun tahminini kontrol eden fonksiyon
  */
  checkMyGuess() {

    if (this.playersGuess == this.computersLocation)
      this.guessIsCorrect = true;
    else
      this.guessIsCorrect = false;
  }
}
```
{% endraw %}

## Çalışma Zamanı

Uygulamayı çalıştırmak için terminalden aşağıdaki komutu vermek yeterlidir.

```bash
ng serve
```

Çalışma zamanına ait örnek ekran görüntülerimiz ise aşağıdakine benzer olacaktır. Mesela bir tahmin yaptık ve sonucu bulamadıysak şuna benzer bir sonuçla karşılaşırız.

![04_30_credit_1.png](/assets/images/2019/04_30_credit_1.png)

Ama sonucu bilirsek de şöyle bir ekranla karşılaşırız.

![04_30_credit_2.png](/assets/images/2019/04_30_credit_2.png)

## Ben Neler Öğrendim

Pek tabii bu antrenmanla da bir çok şey öğrendim. Aklımda kaldığı kadarıyla onları şöyle özetleyebilirim.

- Component bileşeni ile HTML arayüzünü, sınıf özellikleri üzerinden nasıl konuşturabileceğimi
- Bootstrap temel elementlerini Angular bileşenlerinde nasıl kullanabileceğimi
- ng serve komutu ile uygulamayı çalıştırdıktan sonra, bileşen ve arayüzde yapılan değişikliklerin, save sonrası uygulamayı tekrardan çalıştırmaya gerek kalmadan çalışma zamanına yansıtıldığını
- Component arayüzünden, Typescript tarafındaki metodların bir olaya bağlı olarak nasıl tetiklenebileceklerini

Böylece geldik bir maceramızın daha sonuna. [Saturday-Night-Works'ün 30 numaralı projesi](https://github.com/buraksenyurt/saturday-night-works)ne ait blog notlarımı da tamamlamış oldum. Ben bu maceralar sırasında güzel şeyler araştırıyor ve öğreniyorum. Size de böyle bir macerayı tavsiye ederim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
