---
title: "Sıkça Duyduğum Deno'ya Hello Dedim"
pubDate: 2020-01-04 21:00:00
categories:
  - Node.js
tags:
  - Node.js
  - Bash
  - JavaScript
  - REST
  - JSON
  - HTTP
  - Rust
  - TypeScript
  - Async/Await
  - Performance
  - Visual Studio
  - GitHub
---

# Sıkça Duyduğum Deno'ya Hello Dedim
![denverdino.jpg](images/denverdino.jpg)

Denver, the last dinosaur
He's my friend and a whole lot more
Denver, the last dinosaur
Shows me a world I never saw before...

Ha ha haaa! Deno'nun logosunu gördüğüm zaman her nedense aklıma, orta lise çağlarımda izlediğim çizgi dizi Denver'ın bu sözleri gelmişti. Ve hatta melodisi! O sözler eşliğinde 2020'nin herhangi bir noktasında sonradan eskiyeceğinden emin olduğum bir çalışmaya girişeyim istedim. Deno ile basit bir REST servisi ile merhaba demek. Notlar Github'daki skynet reposunda birikti. Buraya da derlenmiş bir özetini yazmak düştü.

Bir süre önce adını sıklıkla duyduğum ve NodeJs'in yerini alır mı almaz mı tartışmalarını (ki öyle bir şey yok) okuduğum Deno'yu basit bir örnekle incelemek istedim. Deno, Javascript haricinde dahili olarak Typescript desteği de sunan (ki örnekte de onu kullandım), V8 üzerinde koşan ve Rust ile yazılmış bir çalışma zamanı olarak nitelendiriliyor. İşin içerisinde Rust olduğu için performans anlamında oldukça önemli beklentiler de beraberinde geliyor. Ben nasıl bir geliştirme tecrübesi yaşatacağını tatmak istemiştim. Klasik kurgu olarak REST tipinden bir servisin birkaç operasyonunu icra etsem yeterli olacaktı. Örnek verileri almak için [International Chuck Norris](http://www.icndb.com/) veritabanını kullandım:D Keza olaya biraz da olsa eğlence katmak her zaman iyidir. Hatta verileri SQLite veritabanında tutmak da fena olmaz. Onu kullanmak için bir kurulum yapmaya da pek gerek yok doğrusu. Sadece Deno Land'den çekeceğimizi söylesek yeterli. Öyleyse harakete geçme zamanı.

## Kurulum (Aslında Pek de Değil)

İlk olarak deno çalışma zamanını sistemime (Heimdall - Ubuntu 20.04) yüklemek gerekiyordu. [Resmi adresinde](https://deno.land/#installation) basit bir kurulum kılavuzu mevcut. Ben şirket bilgisayarına (powershell üzerinden) ve evdeki Linux sistemime ilgili kurulumu aşağıdaki komutlarla yaptım. Windows tarafında tek bir exe geldi ki zaten Deno'nun özelliği de buymuş. NodeJs gibi bir kurulum gerektirmiyor ve tek binary yeterli oluyor. İhtiyaç duyulan modüller [https://deno.land/std](https://deno.land/std) ve benzeri paket adreslerinden import ile uygulama ortamına indiriliyor. Doğruyu söylemek gerekirse zahmetsiz bir kurulum oldu diyebilirim:)

```bash
$env:DENO_INSTALL = "C:\Program Files\deno"
iwr https://deno.land/x/install/install.ps1 -useb | iex
curl -fsSL https://deno.land/x/install/install.sh | sh
```

Çalışmak istediğim uygulama iskeletini ise aşağıdaki şekilde kurguladım. Servis basit bir şekilde yeni Chuck Norris şakaları eklenmesine ve var olan şakaların listelenmesine imkan tanıyacak.

```bash
mkdir chuck_jokes
cd chuck_jokes
touch main.ts
mkdir controller route
touch controller/jokescontroller.ts route/jokesrouter.ts
```

## Kod İçerikleri

Şakaları kod tarafında ifade etmek için model klasörü içerisinde yer alan joke sınıfını ve hatta örnek şakaları tutmak için jokesdb dosyasını kullanmıştım. Sonrasında ise SQLite'ı kullanmaya karar verdim. Bu nedenle controller sınıfını SQLite kullanacak hale dönüştürdüm (Her nedense birkaç alan için ayrı bir model tipi kullanmayı terk etmişim. Sanırım çabuk bir Hello World olsun istemişim)

```javascript
import { open, save } from "https://deno.land/x/sqlite/mod.ts"; //SQLite'ı işin içerisine katalım

//SQLite veritabanını bir hazırlayalım
const db = await open("ICNDB.db"); // International Chuck Norris Database :)
// Şayet veritabanı dosyasında Jokes tablosu yoksa oluşturalım
await db.query("CREATE TABLE IF NOT EXISTS Jokes (id INTEGER PRIMARY KEY AUTOINCREMENT, content TEXT,popularity TEXT)")
await save(db);

// Yeni bir şaka eklemek için kullanılır
export const insert = async ({ request, response }: { request: any; response: any }) => {
    // HTTP Post Body'sinde bir şey yoksa 400 ile geri dönelim
    if (!request.hasBody) {
        response.status = 400;
        response.body = { msg: "Şaka mı yapıyorsun?" };
        return;
    }

    // body içeriğini content ve popularity'ye alalım. (bunların da olup olmadığını kontrol etmek lazım)
    const {
        value: { content, popularity }
    } = await request.body();

    //console.log(content + ' ' + popularity);

    // Insert sorgusunu çalıştıralım
    await db.query("INSERT INTO Jokes (content,popularity) VALUES (?,?)", [content, popularity]);
    // Değişikliği kaydedip kapatalım
    await save(db);

    // HTTP 200 OK dönelim
    response.status = 200;
    response.body = { message: 'Chuck Norris buna sevindi :D' }
}

// Select All fonksiyonu
export const getAll = async ({ request, response }: { request: any; response: any }) => {

    // Select sorgusunun sonucunu bir array'e alıyoruz
    const allJokes = [];
    for (const [id, content, popularity] of await db.query(
        "SELECT id,content,popularity FROM jokes ORDER BY id DESC")) {
        allJokes.push({ RuleNo: id, chuksMessage: content, Pop: popularity });
    }

    // HTTP 200 ile elde ettiğimiz içeriği döndürüyoruz
    response.status = 200;
    response.type = "application/json";
    response.body = JSON.stringify(allJokes);
}

//TODO: getById, Delete, Update gibi operasyonlar eklenebilir
```

Olmazsa olmaz bu CRUD (CreateReadUpdateDelete) operasyonlarının HTTP talepleri ile buluştuğu noktada devreye girecek bir sunucu da gerekiyor. Sunucu görevini ise jokesrouter sınıfı üstleniyor. Kodu oldukça basit. Kritik nokta gelen HTTP taleplerinin jokescontroller içerisindeki fonksiyonlarla eşleştirilmesi.

```javascript
import { Router } from "https://deno.land/x/oak/mod.ts"; //HTTP Server görevini üstlenecek typescript modülü
// deno.land/x adresinde 3rd Party modüller yer alır
import { getAll, insert } from '../controller/jokescontroller.ts';

const router = new Router();

// Root web adresine gelen ki(http://localhost:5555 oluyor) talepler için yönlendirme
router
    .get("/", getAll)
    .post("/", insert);
    //TODO: getById, Delete, Update gibi operasyonlar eklenebilir

export default router;
```

Elbette uygulamanın ana çalıştırıcısının da kodlanması gerekir. İşte çalıştırıcı görevini üstlenen main.ts içeriği.

```javascript
import { Application } from "https://deno.land/x/oak/mod.ts"; //Modül sistemi NodeJS ile farklı. URL kullanılıyor.
import router from "./route/jokesrouter.ts"; // Deno Varsayılan olarak Typescript kullanıyor ve destekliyor
import errorHandler from "./errorHandler.js";
import { open, save } from "https://deno.land/x/sqlite/mod.ts";

const PORT = 5555;
const app = new Application();

app.use(errorHandler);
// Middleware'e router eklendi.
app.use(router.routes());
// HTTP Get, Post, Put, Delete, Head, Options, Patch metodlarının kullanımına izin veriyoruz
app.use(router.allowedMethods());

console.log(`Chuck Norris ${PORT} nolu portta hazır :[] `);

// Portu açıp dinlemeye başlıyoruz
app.listen({ port: PORT });
```

Küçük bir dipnot; hata yönetimi için aşağıdaki errorHandler sınıfı kullanılmakta.

```javascript
export default async ({ response }, nextFn) => {
    try {
        await nextFn();
    } catch (err) {
        response.status = 500;
        response.body = { msg: err.message };
    }
};
```

Kod tarafındaki bu hazırlıkları tamamladıktan sonra birkaç deneme yaptım.

## Çalışma Zamanı

Uygulamayı çalıştırırken internetten indirilecek bazı modüller olması sebebiyle --allow-net ile erişime izin verilmesi gerekiyor. Ayrıca SQLite veritabanı için diske yazma ve diskten okuma iznini de bu uygulamaya vermek lazım. Sanırım NodeJs'in yaratıcısının dili bir takım zafiyetlerden dolayı epeyce yanmış. O nedenle bu insiyatif geliştiricinin sorumluluğunda.

```bash
deno run --allow-net --allow-write --allow-read main.ts
```

İşte Heimdall (Ubuntu 20.04) çalışma zamanına ait birkaç görüntü (Eğer aynı örneği kendi ortamınızda inşa edip benzer görüntüleri alamıyorsanız sürüm farklılıklarına takılmış veya kod tarafında bir hata yapmış olabilirsiniz)

Terminalden gerekli izinleri verip uygulamayı çalıştırdıktan sonra curl ile çektiğimiz örnek veriler...

![screenshot_4.png](images/screenshot_4.png)

Çalışma sırasında sık sık verilerin db'ye yazılıp yazılmadığını da Visual Studio Code üstündeki SQLite eklentisinden kontrol ettim. Başta söylemeyi unuttum ama Ubuntu ortamımda her zaman olduğu gibi geliştirme aracı olarak Visual Studio Code kullanmaktayım.

![screenshot_1.png](images/screenshot_1.png)

Tabii veri eklemek için en kolay yollardan birisi Postman. Aşağıda örnek bir POST çağrısı ile bir Chuck Norris şakası ekleyişimiz resmedilmekte:)

![screenshot_2.png](images/screenshot_2.png)

Http Get ile tüm listeyi de aşağıdaki gibi çekebiliyoruz.

![screenshot_3.png](images/screenshot_3.png)

Chuck Norris artık mutlu diyebilirim. Ancak yapılması gereken birçok şey var. Örneğin verinin devasallaşacağını düşünerek servisin sayfalamalı bir şekilde cevap döndürmesini sağlayabiliriz. Bunlara ek olarak güncelleme ve silme gibi operasyonları da işin içerisine katmak iyi bir pratik olabilir. Bunları Deno'nun güncel sürümleri üzerinden denemenizi öneririm. Kodun son haline [github](https://github.com/buraksenyurt/skynet/tree/master/No%2007%20-%20What%20is%20Deno) SkyNet reposu üzerinden ulaşabilirsiniz. Tekrarda görüşünceye dek hepinize mutlu günler dilerim.
