---
title: "Rust Dilinde Warp, Tokio Küfelerini Kullanarak Asenkron Web Api Geliştirmek"
pubDate: 2020-11-13 08:57:00
categories:
  - Rust
tags:
  - Rust
  - Bash
  - REST
  - JSON
  - Web API
  - HTTP
  - Async/Await
  - Threading
  - Concurrency
  - Serialization
  - Pointers
  - Visual Studio
  - GitHub
  - Thread Safety
  - Mutex
  - Atomic Operations
  - Ownership
  - Arc
---

# Rust Dilinde Warp, Tokio Küfelerini Kullanarak Asenkron Web Api Geliştirmek
![image.axd](images/image.axd)

Geçtiğimiz birkaç ay içerisinde Rust dilini öğrenmeye çalıştım. Zevkli olduğu kadar bir o kadar da zorlayıcı bir süreçti. Öğrendin mi derseniz, "Hayır!" derim:D İşlediğim konuları tekrar etmem gerekiyor. En çok sahiplenme (ownership) ve borçlanma (borrowing) konularında beynimi yaktım diyebilirim (Ah birde trait ve smart pointer konuları var!) Yinede Rust ile vakit geçirdikçe ortaya karışık bir şeyler çıkmaya da başladı. Dilin, Message Passing ve Mutex konularını öğrenmeye çalışırken karşıma Warp ve Tokio küfeleri (Crates) çıktı. Derken olay asenkron çalışan bir Web API geliştirmeye kadar gitti. Her ne kadar Warp denince aklımıza Star Trek gelse de mevzu bambaşka.

Warp, Rust için geliştirilmiş bir Web Server Framework (Rust dünyasında Tide, Rocket, actix-web gibi ürünler de mevcut) Eğer geliştireceğimiz enstrüman bir Web API ise öne çıkan alternatifler arasında yer alıyor. Tokio ise Rust dilinde asenkron çalışmayı kolaylaştıran fonksiyonellikler sunan bir küfe (Crate). Şu sıralarda okuduğum yazılardan öğrendiğim kadarıyla ciddi bir rakibi de var; async-std isimli küfe. Esasında uygulamaya söz konusu asenkron kabiliyetleri sıfırdan kazandırmakta mümkün ancak üretim bandına gidecek projelerde çok da tercih edilen bir yol değil. Nitekim endüstüriyel anlamda kendini kanıtlamış bir çatı pekala işimizi kolaylaştırır. Öyleyse gelin hiç vakit kaybetmeden çalışmamıza başlayalım. Bakalım küfeden neler çıkacak?

Yine baştan söyleyeyim, ben örneği Heimdall (Ubuntu 20.04) üzerinde ve Visual Studio Code arabirimini kullanarak geliştiriyorum. Eğer sizin de ortamınız hazırsa aşağıdaki terminal komutları ile projenin inşasına ve kodlamaya geçebiliriz.

```bash
# İlk önce web api projesini oluşturalım
cargo new musician-api

# Gerekli Paketlerin Yüklenmesi
# Tokio, Warp ve JSON serileştirme için gerekli Serde paketleri 
# Cargo.toml içerisindeki Dependencies sekmesinde yer alıyorlar
# Dolayısıyla sonrasında build işlemi yapmak lazım
cd musician-api
cargo build

# Entity olarak bir struct kullanacağız
# Models isimli küfede Product ve 
# başkalarını konuşlandırabiliriz
touch models.rs

# Veritabanı tarafı
# Aslında in-memory çalışacan bir veri modelimiz var
# Bir json kaynağındaki veriyi okuyor
touch rust_lite.rs

# Product tipi ile ilgili CRUD operasyonlarını
# product-repository isimli dosyada toplayabiliriz
touch product_repository.rs

# Web API taleplerini yöneteceğimiz bir mekanizma da gerekiyor
# Bunu router.rs içinde toplayabiliriz
touch router.rs
```

Uygulamamızdaki paketler için cargo.toml dosyasına aşağıdaki gibi dependencies bildirimlerinin eklenmesi ve build edilmesi gerekmektedir.

```text
[package]
name = "musician-api"
version = "0.1.0"
authors = ["buraksenyurt <burakselimsenyurt@gmail.com>"]
edition = "2018"

# See more keys and their definitions at https://doc.rust-lang.org/cargo/reference/manifest.html

[dependencies]
tokio = { version = "0.2", features = ["macros"] }
warp = "0.2"
serde = { version = "1.0", features = ["derive"] }
serde_json = "1.0"
```

Bağımlılıklar tamamlandıktan sonra kobay veri yapısını (struct) içeren models.rs içeriğini aşağıdaki gibi oluşturarak devam edebiliriz.

models.rs;

```cpp
/*
    Product modeli
    serde kütüphanesini kullanarak serileştirme, ters serileştirme
    işlemlerini otomatize ediyoruz.
    Diğer yandan veritabanında saklamak isteyeceğimiz bir veri olacağından
    kopyalama işlemleri sırasında oluşabilecek borrowing sorunlarının da
    önüne geçiyoruz(Clone)
*/

use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, Deserialize, Serialize)]
pub struct Product {
    pub id: String,
    pub title: String,
    pub price: String,
}
```

Örneğimizde kolaya kaçarak veriyi tutmak için in-memory çalışan bir vector yapısı kullanıyoruz. Buradaki asenkron erişimi de thread safe yapıda kurgulamaktayız.

rustlite.rs;

```rust
/*
    products.json içerisindeki veriyi belleğe çekip bir vector'e parse eden ve
    asenkron olarak farklı thread'lerin güvenli bir şekilde kullanabilmesine
    imkan sağlayan fonkisyonelliği tutan dosyamız.
*/
use crate::models::Product;
use serde_json::from_reader;
use std::fs::File;
use std::sync::Arc;
use tokio::sync::Mutex;

// Mutex<T> smart pointer nesnemiz Product türünden Vector taşıyacacak
// Arc = Atomic Referance Counting
pub type ProductDb = Arc<Mutex<Vec<Product>>>;

/*
    Json dosyasından veriyi yüklemek için kullanılan fonksiyonumuz
    Önce open fonksiyonu ile dosyayı açıyoruz.
    Eğer dosya içeriği başarılı şekilde okunduysa (Ok(json) durumu),
    veriyi JSON'dan ters serileştirip Product türünden vector nesnesine
    aktarıyoruz ve bunu kullanan Mutex'imizi örnekleyip geriye döndürüyoruz.
    Veriyi asenkron olarak gelecek Web API isteklerinin eş zamanlı kullanabilmesi
    için Mutex<T> türünden yararlandık. Thread Safety olmasını da Arc tipinden faydalanarak sağladık.
*/
pub fn load() -> ProductDb {
    println!("Veritabanı yükleme adımı");

    let file = File::open("./products.json");
    match file {
        Ok(json) => {
            let data = from_reader(json).unwrap();
            Arc::new(Mutex::new(data))
        }
        Err(e) => {
            println!("{}", e); // Olası hata durumuna karşı
            Arc::new(Mutex::new(Vec::new()))
        }
    }
}
```

Veri odaklı uygulamamızdaki CRUD operasyonlarını (sadece Create ve Read'i ele aldık) aşağıdaki gibi yazabiliriz.

productrepository.rs;

```rust
use crate::models::Product;
use crate::rust_lite::ProductDb;
use std::convert::Infallible;

use warp::{self, http::StatusCode};

/*
    Ürün listesini Thread-safe döndüren fonksiyon
    rust_lite paketindeki Product_Db'yi kullanıyor ki O da products.json dosyası ile beslenmekte
*/
pub async fn get_all(db: ProductDb) -> Result<impl warp::Reply, Infallible> {
    println!("get_products fonksiyonu çağrıldı");

    let products = db.lock().await; // Arc klonlandı ve thread-safety sağlandı
    let products: Vec<Product> = products.clone(); // Mutex içindeki veriyi de klonladık
                                                   // json formatlı olarak geriye döndürdük
    Ok(warp::reply::json(&products))
}

/*
    Id bilgisine göre ürün bilgisi yine Thread-Safe döndüren fonksiyon.
    Bu fonksiyonu servise gelen talepleri karşılayan router kullanıyor.
*/
pub async fn get_by_id(id: String, db: ProductDb) -> Result<Box<dyn warp::Reply>, Infallible> {
    println!("get_by_id fonksiyonu çağrıldı");

    // Önce db nesnesini tutan Mutex thread-safe klonlanır
    let products = db.lock().await;
    // Amelece olacak ama tüm vector nesnelerini bir iterasyon ile dolaşıyoruz
    for p in products.iter() {
        // parametre olarak gelen id'yi bulursak
        if p.id == id {
            // Bulunan vector satırının json formatına dönüştürülmüş halinin
            // Heap'e çekilmiş bir versiyonunu döndürüyoruz
            return Ok(Box::new(warp::reply::json(&p)));
        }
    }

    // Eğer kayıt bulunamazsa HTTO 404 Not Found durumunu döndüreceğiz
    Ok(Box::new(StatusCode::NOT_FOUND))
}

/*
    Yeni bir ürünün eklenmesi işini üstlenen fonksiyonumuz.
    Router tarafında yeni bir ürün oluşturmak için gelecek POST talebi bu fonksiyona inecek
*/
pub async fn create(payload: Product, db: ProductDb) -> Result<impl warp::Reply, Infallible> {
    println!(
        "Create operasyonuna gelen içerik\n{} {} {}",
        payload.id, payload.title, payload.price
    );
    let mut products = db.lock().await;
    products.push(payload); // vector'e gelen ürünü ekliyoruz
    Ok(StatusCode::CREATED) // HTTP 201 döndürüyoruz
}
```

REST taleplerini toplayıp yönetecek router içeriğini de şöyle kodlayabiliriz.

router.rs;

```rust
/*
    Burası router işlemlerini yönettiğimiz yer
    Mesela doğrudan /products adresine gelecek HTTP Get taleplerine karşılık get_all'un çalışmasını sağlıyoruz.
    Yönlendirme adresleri için warp'un path fonksiyonundan yararlanıyoruz.
    HTTP nin hangi metodunu karşılayacağımız warp::get, warp::post, warp::put gibi çağrılarla belirleniyor.
*/
use warp::{self, Filter};

// use crate::models::Product;
use crate::product_repository;
use crate::rust_lite::ProductDb;

pub fn setup(
    db: ProductDb,
) -> impl Filter<Extract = impl warp::Reply, Error = warp::Rejection> + Clone {
    /*
        Sırasıyla HTTP taleplerini ele alacak fonksiyonlar bildiriliyor.
        (get_all_products, get_product_by_id, add_product vb)
        Bu fonksiyonlar eş zamanlı gelecek istemci taleplerini işlerken db nesnesinin
        (ProductDb) thread-safe klonlanmış bir versiyonlarını parametre olarak alıyorlar.
    */
    get_product_by_id(db.clone())
        .or(add_product(db.clone()))
        .or(get_all_products(db))
    //get_all(db.clone()).or(get_by_id(db)) // Bomba soru için eklendi
}

// Burası /products için HTTP Get talebi geldiğinde çalışacak olan fonksiyon
fn get_all_products(
    db: ProductDb,
) -> impl Filter<Extract = impl warp::Reply, Error = warp::Rejection> + Clone {
    warp::path("products")
        .and(warp::get())
        .and(warp::any().map(move || db.clone())) //Veritabanı referansını (ki bu örnekte Product_Db nesnesi) router tarafına referans olarak paslayabilmek için kullanılan yardımcı fonksiyon.
        .and_then(product_repository::get_all)
}

/*
    URL'den gelen id değerine göre ürün bilgisi getirecek fonksiyonumuz.
    products/{id} şeklinde bir map söz konusudur.
*/
fn get_product_by_id(
    db: ProductDb,
) -> impl Filter<Extract = impl warp::Reply, Error = warp::Rejection> + Clone {
    // path! makrosu URL tarafında parametre kullanımını kolaylaştırır
    warp::path!("products" / String)
        .and(warp::get())
        .and(warp::any().map(move || db.clone())) //with_db fonksiyonelliğini bu şekilde closure olarak da kullanabiliriz
        .and_then(product_repository::get_by_id)
}

/*
    HTTP Post talebine göre JSON İçeriğini alıp yeni bir ürün olarak ekleyecek fonksiyon.
*/
fn add_product(
    db: ProductDb,
) -> impl Filter<Extract = impl warp::Reply, Error = warp::Rejection> + Clone {
    warp::path("products")
        .and(warp::post()) // HTTP Post beklediğimizi belirttik.
        .and(warp::body::json()) // Body'de gelen JSON içeriğini,
        .and(warp::any().map(move || db.clone())) // db nesnesini de klonlayarak
        .and_then(product_repository::create) // create fonksiyonuna paslıyoruz
}
```

Pek tabii uygulamamızın giriş noktası olan main fonksiyonunu da unutmamalıyız.

```rust
use warp;
mod models;
mod product_repository;
mod router;
mod rust_lite;

/*
    Eş zamanlı olarak talep karşılayacak olan main fonksiyonumuz.
    async ile işaretlenmesinin sebebi de bu.
    veritabanını (Aslında json dosya içeriğini belleğe alıp kullandık)
    ve HTTP talep yönlendiricisini örnekledikten sonra,
    warp::serve fonksiyonu ile web sunucusunu localhost:5555 portundan etkinleştirdik.
    router.rs içindeki talimatlara göre talepleri yollayabiliriz.
*/
#[tokio::main]
async fn main() {
    let db = rust_lite::load();
    let product_router = router::setup(db);

    warp::serve(product_router)
        .run(([127, 0, 0, 1], 5555))
        .await;
}
```

Hepsi bu kadar:) Uygulamamızı,

```bash
cargo run
```

terminal komutu ile çalıştırdıktan sonra Postman veya curl gibi araçları kullanarak çeşitli talepler gönderebiliriz. İlk olarak tüm ürünlerin listesini çekmeyi deneyelim.

```text
Adres : http://localhost:5555/products
Metot : HTTP Get
```

![image.axd](images/image.axd)

ve şimdi de belli bir id değerine göre ürün çekmeyi deneyelim.

```bash
Adres : http://localhost:5555/products/1
Metot : Http Get
```

![image.axd](images/image.axd)

İlk çağrıda bir ürün bilgisi beklerken ikinci denemede HTTP 404 almamız gerekiyor.

```bash
Adres : http://localhost:5555/products/123456
Metot : Http Get
```

![image.axd](images/image.axd)

Yeni bir ürün eklemek için HTTP Post tipinden bir çağrı yapmamız gerekir.

```text
Adres : http://localhost:5555/products
Metot : Post
Type : JSON
Body :
{
	"id": "11",
	"title": "Cheese - Le Cru Du Clocher",
	"price": "€9,01"
}
```

![image.axd](images/image.axd)

Pek tabii eklenen içeriği bir Get talebi ile kontrol etmekte yarar var.

![image.axd](images/image.axd)

Uygulama çalışıyor...Güzel...Ama Warp ile Tokio'nun gerçek hayat senaryolarındaki performansını ölçümleyebilmiş değiliz. Şimdilik kaynakların verdiği bilgilere göre asenkron operasyonlarda thread-safe ve yüksek işlem gücü sunduğunu ifade edebiliriz. Ancak siz bana kulak asmayın ve bunu ispat etmeye çalışın. Nitekim yine tokio ile birlikte çalışan actix-web üretim ortamlarında ilk sıraya yerleştiriliyor.

Bu arada ben örnekleri denerken bazı sürprizlerle de karşılaşmadım değil. Örneğin router->setup fonksiyonunda yönlendirme yaptığımız yerde getbyid (db.clone ()).or (getall (db)) yerine getall (db.clone ()).or (getbyid (db)) kullanınca bir terslikle karşılaştım ama söylemem:D Bunu siz bulmaya çalışın. Diğer yandan bu eksik çalışmayla ilgili kendimize birçok ödev çıkartabiliriz. Mesela Router'daki tekrarlanan kod parçalarını gözden geçirebilir, aynı üründen birden fazla eklenmesini önleyebilir, ürün silme ve güncelleme operasyonlarını dahil edebiliri ve hatta ürün eklerken istemcinin göndereceği mesaj boyutunu kontrol altına alabiliriz (1 Megabyte'lık bir JSON içeriğini eklemeye çalışmak istemeyiz öyle değil mi?:D)

Böylece geldik bir SkyNet derlememizin daha sonuna. Örnek uygulama kodlarına [github reposu üzerinden](https://github.com/buraksenyurt/skynet/tree/master/No%2036%20-%20Rust%20at%20Warp%20Speed) erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
