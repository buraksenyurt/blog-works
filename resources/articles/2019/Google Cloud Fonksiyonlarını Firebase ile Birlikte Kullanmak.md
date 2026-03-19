---
title: "Google Cloud Fonksiyonlarını Firebase ile Birlikte Kullanmak"
pubDate: 2019-06-21 07:00:00
categories:
  - GCP
tags:
  - GCP
  - Bash
  - JavaScript
  - .NET
  - NoSQL
  - REST
  - JSON
  - HTTP
  - TypeScript
  - Node.js
  - Transactions
  - GitHub
  - Dependency Management
---

# Google Cloud Fonksiyonlarını Firebase ile Birlikte Kullanmak
![image.axd](images/image.axd)

Google'ın Doodle hizmetini takip ediyor musunuz bilemiyorum ancak ben zaman zaman orada hazırlanmış ikonik görsellerden harika hikayelere gidiyorum. Bu seferki yazının derlemesi sırasında da yolum bir şekilde onunla kesişti ve girişte kimden bahsedebilirim derken havacılılk tarihinin en önemli isimlerinden olan Türkiye'nin ilk kadın pilotu Sabiha Gökçen'i (22 Mart 1913 - 22 Mart 2001) anmaya karar verdim.

Amerikan Hava Kurmay Koleji'nin 1996 yılında Maxwell Hava Üssünde yapılan töreninde Dünya tarihine adını yazdıran 20 havacıdan birisi olarak ödül alan Sabiha Gökçen'in tarihde iz bırakan başarıları saymakla bitmez elbette. Lakin diğer pek çok başarısının yanında bu en çok dikkatimi çekenlerden birisiydi. İçindeki uçma arzusu ve sevgisi öyle büyük olmalı ki Fransız pilot Daniel Acton ile son uçuşunu yaptığında 83 yaşındaydı. Türk Hava Kurumu Türkkuşu'nda Başöğretmen olarak görev aldı ve 1955 yılına kadar bir çok değerli pilotun yetişmesine ön ayak oldu.

Arada bir sizde doodlelayın derim. Bazen çok değerli bilgilere ulaşabiliyoruz. Gelelim Google ile ne işimiz olduğuna (Hoş onsuz hareket ettiğimiz bir günümüz de yok) Bu kez [Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) birinci fazdan [27 numaralı örneğin derlemesi](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2027%20-%20Google%20Cloud%20Functions%20with%20Firebase) ile karşınızdayım. Konumuz Google Cloud Platform üzerinden Firebase tabanlı bir bulut fonksiyon sunmak.

Bulut çözümlerin sunduğu imkanlardan birisi de sunucu oluşturma, barındırma, yönetme gibi etkenleri düşünmemize gerek kalmayacak şekilde uygulama geliştirme ortamları sağlamaları. Bazen bulut platform üzerinde tutulan bir veri tabanı ile konuşan servis kodlarını yine o platformun sunucularında barındırmak suretiyle hizmet sunarız. Söz gelimi Google'ın Firebase veri tabanı ve onu kullanan servis tabanlı fonksiyonları Google Cloud Platform üzerinde konuşlandırabiliriz. Bu örnekteki amacımsa Firebase ile ilişkili bir uygulama servisini Google Cloud Platform üzerinde fonksiyonlaştırabilmekmiş. Her zaman olduğu gibi örneği WestWorld (Ubuntu 18.04, 64bit) üzerinde geliştirmişim. Öyleyse gelin notlarımızı derlemeye başlayalım.

Örnekte Firebase'in Realtime Database seçeneği kullanılmakta. Veriyi JSON tipinde tutan bir NoSQL sistemi olarak düşünülebilir. Veri, bağlı olan tüm istemciler için gerçek zamanlı (realtime) olarak eşlenir. Dahası, istemci uygulama kapansa bile veriyi hatırlar. Cloud-Hosted bir veri tabanıdır. Bir başka deyişle veri tabanı sunucusu google üzerinde durmaktadır. Özellikle Cross-Platform tipinden uygulamalar söz konusuysa (iOS, Android, Javascript veya Typescript fark etmez) tüm bağlı istemcilerle aynı verinin senkronize olarak paylaşılmasını sağlamak gibi önemli bir özelliği vardır. Diğer yandan söz konusu Realtime Database ürünü dışında Cloud Firestore isimli daha önceden üzerinden durup düşündüğümüz bir veri tabanı modeli daha vardır. Firebase'in orjinal veri tabanı olan Realtime modelinin daha geliştirilmiş bir versiyonu olarak düşünebiliriz. Her iki ürün arasındaki farklılıkları kabaca aşağıdaki gibi özetleyebiliriz.

- Realtime modelinde veri JSON ağaç yapısı şeklinde saklanırken Firestore'da koleksiyon biçiminde organize edilmiş dokümanlar söz konusudur (Firestore, Mongo'yu hatırlattı burada bana)
- Firestore özellikle karmaşık ve hiyerarşik veri kümelerini ölçeklerken Realtime modele göre daha başarılıdır.
- Realtime veri tabanı iOS ve Android gibi mobil platformlar için çevrim dışı (offline) çalışma desteği sunar. Firestore buna ek olarak Web tabanlı istemciler için de offline çalışma desteği sağlar.
- Sıralama ve filtreleme imkanları Cloud Firestore'da Realtime modeline göre çok daha geniştir.
- Firestore'da bir transaction tamamlanıncaya kadar otomatik olarak tekrar ve tekrar denenir.
- Realtime veri tabanı modelinde ölçekleme için Sharding uygulanması gerekirken Firestore'da bu iş otomatik olarak yapılır.

Ben uygulaması çok daha basit olduğundan Realtime Database modelini tercih ettim.

## İlk Hazırlıklar

Her şeyden önce Google Cloud Platform üzerinde bir hesabımızın olması lazım. Hesabımız ile login olduktan sonra [Firebase Console adresine](https://console.firebase.google.com/) gidip bir proje oluşturacağız. Söz gelimi project-new-hope gibi bir isimle...

![image.axd](images/image.axd)

Projeyi komut satırından yönetebilmek önemli. Nitekim yazdığımız kodları kolayca deploy edebilmeliyiz. Bu nedenle Firebase CLI (Command Line Interface) aracına ihtiyacımız var. Kendisini npm ile aşağıdaki gibi yükleyebiliriz (Dolayısıyla sistemimizde node ve npm yüklü olmalıdır)

```bash
npm install -g firebase-tools
```

Yükleme işlemi başarılı olduktan sonra proje ile aynı isimde bir klasör oluşturup, içerisinde sırasıyla login ve functions komutlarını kullanarak ilerleyebiliriz. Bu komutlarla Firebase ortamına giriş yapma ve projenin başlangıç iskeletinin oluşturulması işlemleri yapılmaktadır.

```bash
mkdir project-new-hope
cd project-new-hope
firebase login
firebase init functions
```

Login işlemi sonrası arabirim bizi tarayıcıya yönlendirecek ve platform için giriş yapmamız istenecektir. Başarılı login sonrası tekrardan console ekranına dönüş yapmış oluruz.

![image.axd](images/image.axd)

init functions çağrısı ile yeni bir google cloud function oluşturma işlemine başlanır. Dört soru sorulacaktır (En azından çalışmanın yapıldığı tarih itibariyle böyleydi) Projeyi zaten Firebase Console'unda oluşturmuştuk. Klasör adını aynı verdiğimiz için varsayılan olarak onu kullanacağını belirtebiliriz. Dil olarak Typescript ve Javascript desteği sorulmakta ki ben ikincisi tercih ettim. Üçüncü adımda [ESLint](https://eslint.org/) kullanıp kullanmayacağımız soruluyor. Şimdilik 'No'seçeneğini işaretleyerek ilerlenebilir ancak gerçek hayat senaryolarında etkinleştirmek iyi bir fikirdir. (İleriye yönelik problem yaratabilecek olası kod hatalarının önceden tespitinin kritikliği sebebiyle) Projenin bağımlılık duyduğu npm paketleri varsa bunların install edilmesini de istediğimizden son soruda 'Yes'seçminini yapmalıyız.

![image.axd](images/image.axd)

Komut çalışmasını tamamladıktan sonra aşağıdaki klasör yapısının oluştuğunu görebiliriz.

![image.axd](images/image.axd)

Bundan sonra index.js dosyası ile oynayıp örnek bir dağıtım (deployment) işlemi gerçekleştirebiliriz de. Index sayfasında yorum satırı içerisine alınmış bir kod parçası bulunmaktadır. Bu kısmı açarak hemen Hello World sürecini işletmemiz mümkün. Ama bunun için, yapılan değişiklikleri platforma almamız lazım. Aşağıdaki terminal komutu ile bunu sağlayabiliriz. Örnekteki amacımıza göre sadece fonksiyonların taşınması söz konusudur.

```bash
firebase deploy --only functions
```

## Sorun Yaşayabiliriz

Yukarıdaki terminal komutunu denediğimde aktif bir proje olmadığına dair bir hata mesajı aldım ve deployment işlemi başarısız oldu. Bunun üzerine önce aktif proje listesine baktım (firebase list) ve sonrasında use --add ile tekrardan proje seçimi yaptım. Bir alias tanımladıktan sonra (ki her nedense proje adının aynısını vermişim:S) tekrardan deploy işlemini denedim. Bu seferde sadece fonksiyon olarak dağıtım yapmak istediğimi belirtmediğim için başka bir hata aldım. Nihayetinde çalıştırdığım terminal komutu işe yaradı ve proje GCP'a deploy edildi.

```bash
firebase list
firebase use --add
firebase deploy --only functions
```

![image.axd](images/image.axd)

Firebase Dashboard'una gittiğimizde helloworld isimli API fonksiyonunun (ki index.js dosyasından export edilen metodumuzdur) eklenmiş olduğunu görebiliriz.

![image.axd](images/image.axd)

Çalışmanın bu ilk yalın versiyonunda Google'ın index.js içerisine koyduğu yorum satırları kaldırılarak bir deneme yapılmıştır. Bu taşıma işlemi sonrası Firebase tarafında üretilen fonksiyona ait API adresini aşağıdaki gibi curl ile çağırdığımızda 'Hello from Firebase!' yazısını görebiliriz.

```bash
curl -get https://us-central1-project-new-hope.cloudfunctions.net/helloWorld
```

![image.axd](images/image.axd)

## Kod Tarafı

Asıl işi yapan örneğimiz ise basit bir REST hizmeti. POST ve GET mesajlarını destekleyen metotlar içeriyor ve temel olarak veri ekleme ve listeleme fonksiyonelliklerini sağlıyor. Arka planda Firebase veri tabanının Realtime çalışan modelini kullanıyor. Arka plandan kastımız GCP üzerindeki Firebase veri tabanı. Yani kendi makinemizde geliştirdiğimiz bir API servisini, firebase veri tabanını kullanacak şekilde GCP üzerinde konuşlandırmış oluyoruz. İkinci örnek için gerekli bir kaç npm paketi var. REST (Representational State Transfer) modelini node tarafında kolayca kullanabilmek için express ve CORS (Cross-Origin Resource Sharing) etkisini rahatça yönetebilmek için cors:D Aşağıdaki terminal komutları ile onları projemize ekleyebiliriz.

```bash
npm install --save express cors
```

> İlgili paketleri functions klasöründeyken yüklememiz gerekiyor. Nitekim deploy sırasında bu JSON dosyasındaki paket bilgileri, GCP tarafında da yüklenmeye çalışacak. Dolayısıyla GCP'nin, kendi ortamında kullanacağı paketlerin neler olduğunu bilmesi lazım.

index.js dosyasına ait kod içeriğini aşağıdaki gibi geliştirebiliriz.

```javascript
const functions = require('firebase-functions');
const express = require('express');
const cors = require('cors');
const admin = require('firebase-admin');
admin.initializeApp();

const app = express();
app.use(cors()); //CORS özelliğini express nesnesi içine enjekte ettik

// HTTP Get çağrısı gelmesi halinde çalışacak metodumuz
app.get("/", (req, res) => {

    return admin.database().ref('/somedata').on("value", snapshot => {
        // HTTP 200 Ok cevabı ile birlikte somedata içeriğini döndürüyoruz
        return res.status(200).send(snapshot.val());
    }, err => {
        // Bir hata varsa HTTP Internal Server Error mesajı ile birlikte içeriğini döndürüyoruz
        return res.status(500).send('There is something go wrong ' + err);
    });
});

// HTTP Post çağrısını veritabanına veri eklemek için kullanacağız
app.post("/", (req, res) => {
    const payload = req.body; // gelen içeriği bir alalım
    // push metodu ile veriyi yazıyoruz.
    // işlem başarılı olursa then bloğu devreye girecektir
    // bir hata oluşması halinde catch bloğu çalışır
    return admin.database().ref('/somedata').push(payload)
        .then(() => {
            // HTTP 200 Ok - yani işlem başarılı oldu diyoruz
            return res.status(200).send('Eklendi');
        }).catch(err => {
            // İşlem başarısız oldu
            // HTTP 500 Internal server error ile hata mesajını yollayabiliriz
            return res.status(500).send('There is something go wrong ' + err);
        });
});

// Servisten dışarıya açtığımız fonksiyonlar
// somedata fonksiyonumuz için app isimli express nesnemiz ve doğal olarak Get, Post metodları ele alınacak
exports.somedata = functions.https.onRequest(app);

// Servis hayatta mı metodumuz :P
// Ping'e Pong dönüyorsa yaşıyordur deriz en kısa yoldan.
exports.ping = functions.https.onRequest((request, response) => {
    response.send("Pong!");
});
```

Kod nihai halini aldıktan sonra tekrardan dağıtım işlemi yapılmalıdır.

```bash
firebase deploy --only functions
```

![image.axd](images/image.axd)

Dağıtım işlemi sonrasında somedata ve ping referans adresli endpoint bilgilerini dashboard üzerinde görebilmemiz gerekiyor.

![image.axd](images/image.axd)

Şimdi somedata fonksiyonunun Post metodunu kullanarak bir kaç örnek veri girişi yapalım. Postman gibi bir araçtan yararlanarak bu işlemleri kolayca gerçekleştirebiliriz.

> Hızlıca bir test yapmak için ping fonksiyonunu da çağırabilirsiniz. https://us-central1-project-new-hope.cloudfunctions.net/ping adresine talep göndermeniz yeterlidir.

```text
Adres : https://us-central1-project-new-hope.cloudfunctions.net/somedata/
Metod : HTTP Post
Body : JSON
Örnek Veri : {
"Id":1000,
"Quote":"Let’s go invent tomorrow rather than worrying about what happened yesterday.",
"Owner":"Steve Jobs"
}
```

![image.axd](images/image.axd)

Bir kaç deneme girişi yaparak veriyi çoğaltabiliriz. JSON formatlı olmak suretiyle istediğimiz şema yapısında veriler yollamamız mümkün. Firebase sayfasındaki Database kısmına baktığımıza aşağıdakine benzer sonuçları görürürüz.

![image.axd](images/image.axd)

Pek tabii HTTP Get çağrıları sonuncunda da aktardığımız tüm verileri çekebiliriz. Bunun için aşağıdaki adrese talepte bulunmak yeterlidir.

```text
Adres : https://us-central1-project-new-hope.cloudfunctions.net/somedata/
Metod : HTTP Get
```

![image.axd](images/image.axd)

## Başka Neler Yapılabilir?

Ben bir an önce deneyimlemenin heyecanından olsa gerek bu tip Hello World örneklerinde Get, Post harici fonksiyonları uygulamayı çoğunlukla atlıyorum. Dolayısıyla siz tembellik etmeyerek Put, Delete ve filtre bazlı Get metodlarını da örneğe katabilirsiniz. Hatta bu örneğin aksine Realtime Database yerine Cloud Firestore modelini kullanmayı denemenizi de şiddetle öneririm. Ayrıca şema olarak daha düzgün bir veri modeli üzerinden ilerlenebilir.

> Malum bulut hizmetleri belli bir noktadan sonra kullanımlarımıza göre ücret alıyorlar. Bu nedenle yukarıdaki servislere an itibariyle ulaşamayabilirsiniz. Nitekim Azure, Google Cloud Platform veya Amazon Web Services gibi ortamlarda hazırladığım kaynakları işim bittikten bir süre sonra mutlaka kaldırıyorum. Daha önceden yaşadığım bazı acı tecrübeler nedeniyle...

## Ben Neler Öğrendim?

Bu çalışma kapsamında daha çok GCP üzerinde bulut fonksiyon barındırma ve veri tabanı ile ilişkilendirme konularını inceleme fırsatı bulmuş oldum. Pek tabii bu çalışmanın da bana kattığı bir şeyler oldu. Bunları aşağıdaki maddeler halinde özetleyebilirim.

- Firebase üzerinde bir projenin nasıl oluşturulacağını
- firebase-tools ile proje başlatma, fonksiyon dağıtma gibi temel işlemlerin terminalden nasıl yapılacağını
- Kendi geliştirme ortamımızda yazılan node.js tabanlı bir API hizmetini Function olarak Firebase'e nasıl deploy edeceğimi
- Realtime veri tabanı modelinin kabaca ne olduğunu

Böylece geldik bir [cumartesi gecesi macerası](https://github.com/buraksenyurt/saturday-night-works)nın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
