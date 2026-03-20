---
layout: post
title: "Angular ile Basit Bir Görevler Listesi Uygulaması Yazmak"
date: 2019-05-08 06:00:00 +0300
categories:
  - angular
tags:
  - angular
  - bash
  - javascript
  - http
  - typescript
  - react
  - vue
  - blazor
  - visual-studio
  - github
---
Bazen ne kadar basit olursa olsun üşenmeden bir örneğin üstüne gitmek gerekiyor. Çünkü çok basit örneklerle çalışıyor olsak bile gözümüzden kaçan önemli detaylar olabilir. Günümüzde kullanmakta olduğumuz pek çok geliştirme çatısı, belli ürünlere yönelik hazır şablonları kolayca üretebileceğimiz komut setleri sunmakta.

![albertsword.png](/assets/images/2019/albertsword.png)

Boilerplate olarak da ifade edebileceğimiz bu enstrümanlar sayesinde bir anda işler halde karşımıza çıkan uygulamalarla karşılaşıyoruz. Ancak ürüne hakim olabilmek, rahatça sağını solunu bükebilmek için hazır gelen şablonları bile kurcalamak gerekiyor. Benim [Saturday-Night-Works birinci fazında](https://github.com/buraksenyurt/saturday-night-works) sıklıkla icra ettiğim bir eğitim süreci bu. Angular, Blazor, React ve benzeri konu başlıklarında hazır hello world şablonları ile sıklıkla karşılaştım. Onları eğip bükerek daha çok şey öğrenmeye çalıştım. Sonuçta tecrübe etmediğimiz sürece bilgi dağarcığımız genişleyemez, yanılıyor muyum? Öyleyse gelin 09 numaralı çalışmayı kayıt altına alalım.

[Angular](https://angular.io/) ürünü web, mobil ve masaüstü uygulamalar geliştirmek için kullanılan Javascript tabanlı açık kaynak bir web çatısı olarak karşımıza çıkıyor. Uzun zamandır hayatımızda olan ve endüstüriyel anlamda kendisini kanıtlamış bir ürün. Pek tabii sıklıkla Vue ve React ile karşılaştırıldığına da şahit oluyoruz. Ben Saturday-Night-Works çalışmaları kapsamında herbiriyle ilgili en temel seviyede örnekler geliştirmeye de çalıştım. Nitekim bırakın bunları birbirleriyle karşılaştırmayı, gözü kapalı Hello World uygulamaları nasıl yazılır bile bilmiyordum.

Hali hazırda çalıştığım şirketteki yeni nesil uygulamalarda ağırlıklı olarak Vue.js kullanılıyor olsa da yeni özellikler eklemek için var olan öğelere bakıyorduk. Dolayısıyla Angular tarafında sıcak kalmaya çalışmak adına basit bir örnekle başlamak yerinde bir karardı. Bende böyle yapmışım. Örnekte kendime bir görev listesi oluşturuyorum. Sadece yeni giriş ve silme fonksiyonu olsa da bir şeyler öğrendim diyebilirim ("ToDo List" en yaygın Hello World örnekleri arasında yer alıyor) Uygulamayı her zaman ki gibi Visual Studio Code ile WestWorld (Ubuntu 18.04, 64bit) üzerinde icra etmekteyim.

## Ön Gereklilikler

Tabii işin başında bize bir takım alet edevatlar gerekiyor. node ve npm sistemde olması gerekenler. WestWorld'de bu araçlar zaten var (Yani sizin sisteminizde yoksa edinmelisiniz) npm'i Angular için Command Line Interface (CLI) aracını yüklemek maksadıyla kullanıyoruz. Kurulum için gerekli terminal komutu şöyle,

```bash
sudo npm install -g @angular/cli
```

Angular CLI ile projeyi oluşturmak oldukça basit. Önyüz tarafının görselliğini arttırmak adına Bootstrap kullanabiliriz. Tabii öncelikle ilgili bootstrap paketlerini sisteme dahil etmemiz gerekiyor. Bunu bower yöneticisinden yararlanarak aşağıdaki terminal komutu ile yapabiliriz.

```bash
bower i bootstrap
```

> Angular projesini oluşturduktan sonra bootstrap'in CSS dosyalarını assets/css altına alıp orayı referans etmeyi tercih ettim (index.html sayfasına bakın) Lakin Bootstrap için CDN adreslerini de pekala kullanabiliriz.

## Angular Uygulamasının Oluşturulması

Angular uygulamasını hazır şablonundan üretmek oldukça kolay ve sıklıkla tercih edilen yollardan birisi. Tek yapmamız gereken terminalden ng new komunutunu çalıştırmak. new sonrası gelen parametre tahmin edileceği üzere uygulamamızın adı olacak.

```bash
ng new life-pbi-app
```

ng new sonrası oluşan proje içerisinde çok fazla dosya bulunacaktır. Şu haliyle de uygulamayı çalıştırıp sonuçlarını görebiliriz ama başta da belirttiğim üzere biraz eğip bükmek lazım. Benim yaptığım değişiklikler son derece basit. Sonuçta tek bir arayüzüm olacak ki bu index.html. Önyüzde gösterilecek bileşenimiz ise yine şablon ile hazır olarak gelen app.component. Ona ait HTML içeriğini örnek için aşağıdaki gibi değiştirebiliriz.

app.component.html

{% raw %}
```text
<div class="container">
  <form>
    <div class="form-group">
      <h1 class="text-center text-success">Çalışma Planım...</h1>
      <p>Burada 1 haftalık kişisel görev planlarıma yer vermekteyim. Mesela <i>"bu hafta 10 km yürüyüş yapacağım"</i></p>
      <div class="card input-group-prebend">
        <div class="card-body">
          <input type="text" #job class="form-control" placeholder="Salı günü 100 faul atışı çalışacağım..." name="job"
            ngModel>
          <!-- addJob metodundaki job nesnesi üst kontroldeki #job niteliğidir. 
              value özelliğine giderek girilen bilgiyi addJob metoduna göndermiş oluyoruz. -->
          <input type="button" class="btn btn-info" (click)="addJob(job.value)" value="Ekle" />
        </div>
      </div>
      <!-- ngFor ile jobs dizisinde dolaşıyoruz ve her bir eleman için 
        card stilinde birer div oluşturulmasını sağlıyoruz
      -->
      <div *ngFor="let job of jobs" class="card">
        <div class="card-body">
          <div class="row">
            <div class="col-sm-10">
              {{job}} <!-- dizideki görevin bilgisini yazdırıyoruz-->
            </div>
            <div class="col-sm-2">
              <!-- Silme işlemi için removeJob fonksiyonu çağrılıyor. 
                Parametre ise dizinin o anki elemanı-->
              <input type="button" class="btn btn-primary" (click)="removeJob(job)" value="Çıkart" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </form>
</div>
<!-- addJob, removeJob metodları ile jos dizisi app.component.ts dosyası içerisinde yer alıyor -->
```
{% endraw %}

component içerisinde basit bir form grubu var. İçinde iki adet bileşen gövdesi bulunuyor. Üst taraf yeni görev girmek için kullanılan kısım. Ekle başlıklı düğmeye basıldığındaysa Typescript tarafındaki addJob metodu çağırılıyor. Parametre olarak job isimli text kontrolünün içeriği gönderilmekte.

Alt tarafta yer alan gövde içindeyse bir for döngüsünden yararlanılarak tüm görev listenin basıldığı satırlar bulunuyor. Çıkart başlıklı düğmeye basıldığında devreye giren removeJob fonksiyonu parametre olarak döngünün o anki Job nesne örneğini almakta ki bunu silme işlemi için kullanıyoruz. Burada aslında güncelleme içinde bir şeyler yapmak gerekiyor. Ne var ki çalışma sırasında bunu atlamışım. Kuvvetle muhtemel üşendiğim içindir. Siz güncelleme için ayrı bir bileşene yönlendirmeyi deneyebilirsiniz (ki ben ilerleyen safhalarda Firebase ile ilgili bir kullanımı da denemişim. Magic Johnson numaralı örnek. Onu da bir ara bloğa kayıt altına almalıyım)

app.component.ts (typescript tabanlı bileşenimiz)

```javascript
import { Component } from '@angular/core';
import { isJsObject } from '@angular/core/src/change_detection/change_detection_util';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html', //Bu Typescript dosyasının hangi html ile ilişkili olduğu belirtiliyor
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  jobs = []; //görev listesinin tutulacağı dizi

  // yeni bir job eklemek için
  addJob(value) {
    if (value !== "") {
      this.jobs.push(value)
      // console.log(this.jobs)  // Tarayıcı console penceresine log düşürebiliriz
    } else {
      alert('Bir görev girmelisin... ;)')
    }
  }

  // bir görevi listeden çıkartmak için
  removeJob(job) {
    for (let i = 0; i <= this.jobs.length; i++) {
      if (job == this.jobs[i]) {
        this.jobs.splice(i, 1)
      }
    }
  }
}
```

Bileşenin Typescript tabanlı arka tarafı görev ekleme ve silme operasyonlarını içermekte. app-root ile ilişkilendirilmiş durumda (Bu, index.html sayfasındaki yerleşim için önemli bir bilgi) Örneğin basit olması amacıyla görev listesi uygulama çalıştığı sürece bellekte duran bir diziyi kullanıyor. Elbette bunu farklı bir veri kaynağına bağlayabiliriz. Mesela Azure Cosmos DB veya SQLite gibi veri kaynaklarının kullanılması tercih edilebilir. Son olarak ilgili bileşenin gösterildiği index.html içeriği de aşağıdaki gibi değiştirilebilir.

```text
<!doctype html>
<html>

<head>
  <meta charset="utf-8">
  <title>Kişisel PBI Listem</title>
  <base href="/">

  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="icon" type="image/x-icon" href="favicon.ico">
  <link rel="stylesheet" href="/assets/files/2019/css/bootstrap.min.css" />
</head>

<body>
  <app-root>Az sabır. Yükleniyor daaa!</app-root>
</body>

</html>
```

## Çalışma zamanı

Uygulamayı çalıştırmak için aşağıdaki terminal komutunu vermek yeterli.

```bash
ng server
```

Buna göre http://localhost:4200 adresine talep gönderirsek uygulamamıza ulaşabiliriz (URL bilgisi javascript dosyalarından birisinde de parametrik olarak bulunuyor. Geliştirme ortamı için değiştirmek isteyebilirsiniz diye söylüyorum;)) Uygulamanın çalışma zamanına ait örnek bir görüntüde şöyle.

![04_09_credit_1.png](/assets/images/2019/04_09_credit_1.png)

## Ben Neler Öğrendim?

[Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) birinci fazındaki ilk acemilik uygulamalarımdan birisi olan [09 numaralı örneğin](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2009%20-%20ToDo%20App%20with%20Angular) de bana kattığı bir takım şeyler oldu tabii. Bunları genişletirsek aşağıdaki gibi listeleyebilirim.

- Typescript ile HTML tarafındaki Angular yapılarının nasıl anlaştığını
- Bootstrap'i bir Angular projesinde nasıl kullanabileceğimi
- component üzerindeki button kontrollerinden Typescript olaylarının nasıl tetiklendiğini
- Temel ng terminal komutlarını

Böylece geldik bir maceramızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
