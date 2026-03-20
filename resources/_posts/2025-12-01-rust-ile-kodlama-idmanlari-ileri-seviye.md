---
layout: post
title: "Rust ile Kodlama İdmanları - İleri Seviye"
date: 2025-12-01 19:51:00 +0300
categories:
  - rust
tags:
  - rust
  - authentication
  - async-await
  - threading
  - concurrency
  - performance
  - pointers
  - github
  - thread-safety
  - mutex
  - atomic-operations
  - refcell
  - rc
  - arc
  - dependency-management
---
Rust programlama dili ile ilgili maceralarımıza devam ediyoruz. Gerçi geçtiğimiz günlerde sizin de bildiğiniz üzere unwrap metodunun hatalı bir kullanımı sebebiyle internet sitelerinin %20sini koruyan [Cloudfare](https://www.cloudflare.com/) çöktü ve birçok hizmet uzun süre kullanılamaz hale geldi. Konuyla ilgili detaylı bilgilere[şu adreste](https://blog.cloudflare.com/18-november-2025-outage/)ki yazıdan ulaşabilirsiniz. Ferris şaşkın, ferris üzgün!

![15_mini.png](/assets/images/2025/15_mini.png)

Şunu kabul etmek lazım ki C, C++ gibi diller gerçekten dikkat isteyen, kaotik sorunlara yol açabilen, programıcısının yüksek yetkinlikte olmasını gerektiren programlama dilleri. En azından ben bu şekilde düşünüyorum. Rust'ın kendi bellek yönetim felsefesi bu sebeple en çok öne çıkan ve rağbet gören özellikleri arasında. Lakin dil ne kadar becerikli de olsa biz programcılar zaman zaman kötü hatalara sebebiyet veriyoruz.

Unwrap fonksiyonu genellikle yazılımı geliştirme aşamasında, testleri kolaylaştırmak amacıyla tercih edilen bir metot. Result değerini match ifadeleri ile sürekli kontrol ederek vakit kaybetmek yerine, zaten bir değer döndüreceğinden emin olduğumuz çağrılarda doğrudan değeri alıp kod işletmeye devam ediyoruz. Ama işte bu kod parçası üretime çıktığında olaslığının %0.00001 olduğu ihtimalin gerçekleşmesi mümkün. Bu yakın zaman trajedisini şimdilik kenara park edelim ve ileri seviye konularımıza başlayalım.

## Unsafe Kodları Soyutlamalar ile Sarmak

Derleyicinin bellek güvenliğini garantiye alamadığı durumlar için unsafe kod blokları kullanılır. Ancak unsafe kodların doğrudan kullanımı, bellek güvenliği sorunlarına da yol açabilir. Bu nedenle unsafe kodları güvenli soyutlamalar (safe abstractions) ile sarmak ideal yaklaşımlardan birisidir.

Örneğin bir sayı dizisini referans olarak kullanırken ödünç alma kurallarını atlayarak herhangi bir noktasından ikiye bölmek istediğimizi düşünelim. 101 elemanlı bir sayı dizisini 16ncı indisinden itibaren iki ayrı parça halinde değiştirilebilir referans olarak ele almak istiyoruz. Normalde rust aynı anda aynı veriye iki farklı değiştirilebilir referans vermeye izin vermez. Lakin unsafe çağrılabileceğini bildiğimiz bir fonksiyona göz yumup bu kuralı atlayarak geliştirme yapabiliriz. İşte burada unsafe kodu güvenli bir soyutlama ile sarmak önemlidir. Aşağıdaki kod parçasında bu durum basitçe ele alınmakta.

```rust
use std::slice;

fn main() {
    let mut numbers = vec![1, 4, 6, 1, 6, 2, 4, 6, 7, 9, 123, 7, 1, 7];

    // numbers dizisi 3. indexten ikiye bölünüyor
    let (left_slice, right_slice) = split_array_from(&mut numbers, 3);

    println!("Left slice values: {:?}", left_slice);
    println!("Right slice values: {:?}", right_slice);

    // left_slice dilimindeki ilk elemanı değiştiriyoruz
    // bu değişiklik orijinal numbers dizisini de etkileyecektir
    left_slice[0] = 345;
    println!("After changed the left slice: {:?}", numbers);
}

/// Bu fonksiyon, verilen `values` dilimini `index` konumunda ikiye böler
/// ve iki ayrı dilim olarak döner.
///
/// # Güvenlik Notu
///
/// Bu fonksiyon unsafe kod kullanır, bu nedenle dikkatli olunmalıdır.
///
/// # Parametreler
///
/// - `values`: Bölünecek olan tamsayı dilimi.
/// - `index`: Bölme işleminin gerçekleşeceği konum.
///
/// # Dönüş Değeri
/// İki ayrı tamsayı dilimi olarak döner.
fn split_array_from(values: &mut [i32], index: usize) -> (&mut [i32], &mut [i32]) {
    let len = values.len();
    // ptr değişkeni, values diliminin başlangıç adresini tutan bir işaretçidir(pointer).
    let ptr = values.as_mut_ptr();

    /*
        from_raw_parts_mut fonksiyonu unsafe türdendir ve bu nedenle
        unsafe kod bloğu içerisinde çalıştırılması gerekir.
    */
    unsafe {
        // ptr ile tutulan adresten başlayarak index uzunluğunda bir dilim oluşturur.
        let left = slice::from_raw_parts_mut(ptr, index);
        // index noktasından başlayarak len - index uzunluğunda bir dilim oluşturur.
        let right = slice::from_raw_parts_mut(ptr.add(index), len - index);
        (left, right)
    }
}
```

Burada özellikle fromrawpartsmut metodunun resmi dokümantasyonunda yer alan Safety kısmını okumak lazım. Performans açısından bize avantaj sağlar ama beraberinde bazı riskler de getirir sonuçta. Diğer yandan bu metot unsafe çağırılabilen bir fonksiyondur ve unsafe kod bloğuna almadığımız takdirde call to unsafe function is unsafe and requires an unsafe function or block şeklinde bir derleme hatası ile karşılaşırız.

Kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![rust_exc_19.png](/assets/images/2025/rust_exc_19.png)

## Eşzamanlı (Concurrency) Veri Paylaşılan Durumlarda Kilitlenme ve Yarış Durumlarından (Data Races) Kaçınmak

Farklı iş parçacıklarının aynı veriye eşzamanlı olarak erişmesi gereken senaryolar vardır. Özellikle erişilen veri üzerinde değişiklik yapılacaksa deadlock'lar oluşması muhtemeldir. Hatta bu durum çoğunlukla Data Races olarak da bilinir. Rust tarafında son yazan kazanır gibi durumlarının üstesinden gelmek için bir Smart Pointer türevi olan Arc (Atomic Reference Counting) ve Mutex (Mutual Exclusion) enstrümanları kullanılır.

Bir web sunucusuna gelen sayısız isteğin birden fazla iş parçacığı tarafından işlendiğini düşünelim. Her bir thread gelen request ile ilgili bir şeyler yapıyor olsun. Bu vakada toplam istek sayısını global bir sayaç ile tuttuğumuzu varsayalım. Her thread aynı veri üzerinde değişiklik yapmaya çalışacaktır. Bu durumu Arc ve Mutex enstrümanlarını kullanarak aşağıdaki kod parçasında olduğu gibi ele alabiliriz.

```rust
use std::sync::{Arc, Mutex};
use std::thread;
use std::time::Duration;

fn main() {
    // Global paylaşımlı değişken
    // Arc ile çoklu sahiplik
    // Mutext kilitleme ile değiştirebilir erişim imkanı
    let counter = Arc::new(Mutex::new(0));
    let mut threads = vec![];
    let thread_count = 4;

    for i in 0..thread_count {
        let counter_clone = Arc::clone(&counter); // Referansları say

        let thread = thread::spawn(move || {
            println!("Thread {} starting", i);

            // Mutext ile kilitlenir ve MutexGuard alınır.
            // Diğer erişmeye çalışanlara müsaade edilmez
            let mut value = counter_clone.lock().unwrap();
            *value += 1;

            thread::sleep(Duration::from_millis(100));
        });
        threads.push(thread);
    }

    // Tüm iş parçacıklarının bitmesini bekleyelim
    for t in threads {
        t.join().unwrap();
    }

    println!(
        "Current total request count is {}",
        *counter.lock().unwrap()
    );
}
```

Bu kodun çalışma zamanı çıktısı aşağıdakine benzer olacaktır. Dikkat edileceği üzere ay sayıda thread açılmış olmasına rağmen her çalıştırmada genelde farklı sıralamalarda işletimler söz konusudur. Bu son derece doğaldır zira thread'ler herhangi bir sırada başlar. Ancak hangi sırada başlarlarsa başlasınlar sonuçta toplam değeri hep aynı çıkar.

![rust_exc_20.png](/assets/images/2025/rust_exc_20.png)

## Spawn Blocking Tasks ile Asenkron Kodlarda Performans Artışı Sağlamak

Merkezi işlem biriminin (CPU) yoğun kullanıldığı uzun süreli işler veya bloklamaya neden olan I/O operasyonlarında asenkron yürütücüler sorunlar yaşar. Örneğin diğer asenkron görevlerin ilerlemesi bu bloklamalar sırasında durur. Eğer işler farklı bir thread pool'a alınabilecek kıvamdaysa örneğin tokio küfesinin spawnblocking yapısı kullanılarak ilerlenebilir. Örneğin bir web sunucusu gelen isteğe ait bir asenkron iş akışı yürütülürken, şifre çözme gibi CPU'yu yoracak bir görevin gereksiz beklemeler olmadan çalıştırılması için bu özellik kullanılabilir. Tabii öncesinde rust projesinde gerekli Tokio küfesinin (crate) yüklü olması gerektiğini de hatırlatalım.

```text
[dependencies]
tokio = { version = "1.48.0", features = ["full"] }
```

Şimdi örnek uygulama kodlarına bakalım.

```rust
use tokio::time::{self, Duration};
use std::thread;

#[tokio::main]
async fn main() {
    call().await;
}

async fn call(){
    let start_time = time::Instant::now();
    println!("Service started...");

    // // Bad Practice: CPU yoğun işlemi doğrudan asenkron bağlam içinde ele aldığımızda
    // // asıl executor'ı da engeller
    // let pwd = decrypt("some hash value");

    // Good Practice: CPU yoğun işlemi spawn_blocking ile ayrı bir thread pool'a devrediyoruz
    let pwd_handle = tokio::task::spawn_blocking(|| {
        decrypt("some hash value")
    });

    // Diğer asenkron işlemleri simüle etmek için geçici bir bekleme yapıyoruz
    let io_opt = time::sleep(Duration::from_millis(500));

    // Burada tokio join ile iki asenkron işlemi paralel olarak işletiliyor
    tokio::join!(
        async{
            // Sembolik bir I/O operasyonu icra ettiğimizi düşünelim.
            println!("I/O operations completed");
            io_opt.await;
            println!("I/O wait is over");
        },
        async {
            // // Bad Practice :
            // println!("Decryption result '{}'",pwd);

            // Good Practice :
            let pwd = pwd_handle.await.expect("Blocking task failed.");
            println!("Decryption result '{}'",pwd);
        }
    );

    /*
        Toplam süreyi raporluyoruz.
        Gözlemlere göre spawn_blocking kullanımı ile asenkron işlemler engellenmeden paralel yürütülüyor.
        Buna göre toplam çalışma süresi yaklaşık olarak 1 saniye civarında oluyor.
        Ancak decrypt fonksiyonunu doğrudan asenkron bağlam içinde çağırıldığında bu süre 1.5 saniye civarına çıkıyor.
        Çünkü, decrypt fonksiyonu asenkron executor'ı bloke ediyor.

        Bad Practice toplam süre: ~1500 ms ve çalışma zamanı çıktısı:

        Service started...
        Starting decryption for 'some hash value'
        I/O operations completed
        Decryption result 'value decrypted'
        I/O wait is over
        Total process duration is 1506

        Good Practice toplam süre: ~1000 ms ve çalışma zamanı çıktısı:

        Service started...
        I/O operations completed
        Starting decryption for 'some hash value'
        I/O wait is over
        Decryption result 'value decrypted'
        Total process duration is 1002
    */

    println!("Total process duration is {}",start_time.elapsed().as_millis());
}

fn decrypt(value:&str) -> String {
    println!("Starting decryption for '{}'",value);
    thread::sleep(Duration::from_millis(1000));
    "value decrypted".to_string()
}
```

Uygulama kodunda hem Bad Practice hem de Good Practice şeklinde yorum satırlarımız var. Bunları ayrı ayrı açarak test etmekte yarar var. İdeal ve önerdiğimiz pratikte tokio küfesinden spawnblocking fonksiyonunu kullanarak decrypt işlemini farklı bir thread havuzuna devretmekteyiz. Buna göre sembolik olarak duraksatma yaptığımız ioopt işlemi ile ayrı bir havuz işletiliyor diyebiliriz. İlk versiyonda 1500 mili saniye süren işlem ideal sürümde 1000 mili saniye seviyelerine iniyor. Dolayısıyla bir thread'in ilgisi olmayan diğer thread'leri bekletmesinin önüne geçmiş oluyoruz.

![rust_exc_30_new.png](/assets/images/2025/rust_exc_30_new.png)

## Typestate Pattern ile Daha Güvenli Program Arayüzleri (API) Tasarlamak

Typestate Pattern'de bir nesnenin durumu tür sistemi ile ifade edilir. Böylece nesnenin belirli bir durumda hangi işlemleri yapabileceği tür sistemi tarafından garanti altına alınır. Bu desen, özellikle karmaşık State makineleri veya belirli adımların sırasıyla takip edilmesi gerektiği süreçler için faydalı bir kullanımdır.

Örneğin bir ağ nesnesninin alabileceği durumları düşünelim: Bağlantı kurulmamış, bağlantı kurulmuş, veri gönderilmiş veya veri alınmış gibi farklı pozisyonlarda olabilir. Bu durumlardan hangisinde ne tür işlemlerin yapılabileceğini de tür sistemi üzerinden ifade edebiliriz. Böylece yanlış sırayla yapılan işlemler derleme zamanında kolayca yakalanabilir. Aşağıdaki kod parçasına bu gözle bakabiliriz.

```rust
fn main() {
    let connection = Connection::new();
    let initialized_connection = connection.initialize("server=localhost;port=8080");
    match initialized_connection.connect() {
        Ok(_connected_connection) => {
            println!("Connection established successfully!");
        }
        Err(e) => {
            println!("Failed to connect: {}", e);
        }
    }
}

/*
    Durumları temsil eden tipler. Genellikle veri içermezler.
    Bunlar marker types olarak da bilinir.

    Aşağıdaki örnekte üç durum tanımlanmıştır:
    - Disconnected: Bağlantı kurulmamış durum
    - Initialized: Bağlantı başlatılmış ama henüz bağlanmamış durum
    - Connected: Bağlantı kurulmuş durum

    Initialized durumuna geçilebilmesi için önce Disconnected durumunda olunması gerekir.
    Connected durumuna geçilebilmesi için ise Initialized durumunda olunması gerekir.
*/
struct Disconnected;
struct Initialized;
struct Connected {
    _address: String,
}

// Connection yapısı, State tür parametresi ile durumunu belirtir.
struct Connection<State> {
    config: String,
    // State türü, Connection yapısının bir parçası değildir ancak tür sistemi tarafından da izlenmesi gereken bir bilgidir.
    // Bu nedenle PhantomData kullanılmakta. PhantomData, built-in bir marker type'dır. Rust ile gelen standart tür sistemi dışındaki
    // tür bilgilerini taşımak için kullanılır.
    state: std::marker::PhantomData<State>,
}

impl Connection<Disconnected> {
    fn new() -> Self {
        println!("Creating new connection");
        Connection {
            config: String::new(),
            state: std::marker::PhantomData,
        }
    }

    fn initialize(mut self, config: &str) -> Connection<Initialized> {
        println!("Initializing connection with config: {}", config);
        self.config = config.to_string();

        Connection {
            config: self.config,
            state: std::marker::PhantomData,
        }
    }
}

impl Connection<Initialized> {
    fn connect(self) -> Result<Connection<Connected>, String> {
        println!("Connecting with config: {}", self.config);
        // Konfigürasyon geçerli ise ve bağlantı başarılı ise Connected durumuna geçiş yaparız.
        // Aksi halde hata döneriz. Burada basit bir örnek olması için her zaman başarılı sonuç dönüyoruz.
        Ok(Connection {
            config: self.config,
            state: std::marker::PhantomData,
        })
    }
}
```

Aslında bu kod parçasındaki en kritik konulardan birisi PhantomData türünün kullanımıdır. İleride tekrar uğrayacağımız bu tür ile derleme zamanında bilinen ama çalışma zamanına taşınmayacak türlerin kullanımı mümkün hale gelir. Bu, tipik olarak sıfır maliyetli bir soyutlama yaklaşımdır (Zero Cost Abstraction) ve derleme zamanında tür kontrolü ile bazı garantileri çalışma maliyeti olmaksızın tesis etmemizi sağlar.

Örnek kod parçasının çalışma zamanı çıktısı aşağıdaki gibidir.

![rust_exc_22.png](/assets/images/2025/rust_exc_22.png)

## Uygulama Düzeyinde Hata Yayılımı (Error Propagation) için anyhow Kullanmak

Uygulamalar büyüdükçe hata yönetimi de karmaşıklaşır. Farklı modüllerin peşi sıra çağrılan farklı fonksiyonlarından gelen hata türlerini tek bir dinamik hata türünde toplamak ve yönetmek için anyhow kütüphanesi kullanılabilir. Bu kütüphane, farklı hata türlerini tek bir Error türüne sararark hata yayılımını (Error Propagation) kolaylaştırır. anyhow kütüphanesi ayrıca hata bağlamı (context) ekleme yeteneği de sağlar. Bu sayede hataların nerede ve neden oluştuğu daha iyi loglanabilir. Hatalar genel bir türe evrilirken detaydaki hatalar da downcast edilerek yakalanabilir. Tabii bu küfeyi kullanabilmek için projeye yüklenmiş olması gerekir. Yani toml dosyamızda aşağıdaki gibi bir bağımlılık tanımı olmalıdır.

```text
[dependencies]
anyhow = "1.0.100"
```

Tabii sürüm değişirse kullanım şeklinde de değişiklik olabilir. Lütfen güncel sürümdeki fonksiyonları kontrol ederek deneyiniz.

Şimdi gelin bu küfeyi basit bir kod parçasında ele alalım.

```rust
use anyhow::{Context, Result};
use std::io;
use std::num::ParseIntError;

fn main() {
    match run() {
        Ok(_) => println!("All operations completed successfully."),
        Err(e) => {
            // Burada oluşan tüm hataları ve context bilgilerini yazdırabiliriz
            let mut source = e.source();
            let mut level = 1;
            while let Some(err) = source {
                println!("  {}. {}", level, err);
                source = err.source();
                level += 1;
            }

            // İstersek bir anyhow::Error içindeki spesifik hata türlerine de erişebiliriz
            // Bunu, downcast_ref fonksiyonu ile sağlayabiliriz.
            if let Some(io_err) = e.downcast_ref::<io::Error>() {
                println!("IO Error details: {:?}", io_err.kind());
            }

            // Örneğin detaya gelen hata ParseIntError ise,
            if let Some(parse_err) = e.downcast_ref::<ParseIntError>() {
                println!("Parse Error details: {}", parse_err);
            }
        }
    }
}

// Bu fonksiyonda farklı senaryoları test ediyoruz
// Her adımda context ekleyerek hataların nerede oluştuğunu daha iyi anlamak mümkün.
// Kod tabanı geniş uygulamalarda bu yaklaşım hata ayıklamayı kolaylaştırır.
fn run() -> Result<()> {
    // Senaryoları tek tek açarak deneyebiliriz.
    add_product(1001, "ElCi Laptop", 999.99)
        .with_context(|| "Failed in scenario 1 - product not found")?;

    add_product(1003, "AyFone Smartphone", -399.99)
        .with_context(|| "Failed in scenario 3 - negative price test")?;

    add_product(9999, "Mouse Optical", 100.45)
        .with_context(|| "Failed in scenario 4 - database error test")?;

    Ok(())
}

// business modülünde ürün ekleme fonksiyonu
// anyhow ile context ekleme örneği
fn add_product(id: u32, name: &str, price: f64) -> Result<()> {
    validate_product(id, name, price)
        .with_context(|| format!("Product validation failed for ID: {}", id))?;
    write(&Product::new(id, name, price))
        .with_context(|| format!("Database operation failed for product: {}", name))?;

    Ok(())
}

// business modülünde çağrılan bir ürün doğrulama fonksiyonu
fn validate_product(id: u32, name: &str, price: f64) -> Result<()> {
    if id == 0 {
        return Err(anyhow::anyhow!("Product ID cannot be zero"));
    }

    if name.is_empty() {
        return Err(anyhow::anyhow!("Product name cannot be empty"));
    }

    if name.len() > 50 {
        return Err(anyhow::anyhow!(
            "Product name too long: {} characters (max: 50)",
            name.len()
        ));
    }

    if price < 0.0 {
        return Err(anyhow::anyhow!(
            "Product price cannot be negative: ${:.2}",
            price
        ));
    }

    if price > 10000.0 {
        return Err(anyhow::anyhow!(
            "Product price too high: ${:.2} (max: $10000.00)",
            price
        ));
    }

    Ok(())
}

// db modülünde bir ürün yazma fonksiyonu
// En alt katman - io::Error döndürüyor, anyhow yukarıdaki katmanlarda kullanılıyor
fn write(product: &Product) -> io::Result<()> {
    // Sadece database bağlantı hatasını simüle etmek için
    if product.id == 9999 {
        return Err(io::Error::new(
            io::ErrorKind::ConnectionRefused,
            "Database connection failed",
        ));
    }

    Ok(())
}

#[derive(Debug)]
#[allow(dead_code)]
struct Product {
    id: u32,
    name: String,
    price: f64,
}

impl Product {
    fn new(id: u32, name: &str, price: f64) -> Self {
        Product {
            id,
            name: name.to_string(),
            price,
        }
    }
}
```

Bu kodun çalışma zamanı çıktısı ise aşağıdaki gibidir.

![H1Tz6WZq55bbAAAAAElFTkSuQmCC](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAqIAAACiCAYAAACanq/aAAAgAElEQVR4Xu19CZxUxbV+DTAMDMMiMKKCsigiLiwGFSU+UTCIgCaKGhNwifISt0hi4ktUzDNoooka/poY88REFGKIaKJIDCqKUVSEyKIREIUBUXYEZRsYmH9/NZ62pqburXO7b/d095xK/NHTXbfq1Hdq+e45p6qK+n7lxOqyFi1USdNitWHDOiVJEBAEBAFBQBAQBAQBQUAQyAYCRSeedEp1eXl7tfaTNaqqqiobdUodgoAgIAgIAoKAICAICAKCgCr6r9POqK6u3qe2bf1U4BAEBAFBQBAQBAQBQUAQEASyhkDRoMFDqrd/vk3t2rUza5VKRYKAICAICAKCgCAgCAgCgkDRWUOHVa9ft1bt2ydueekOgoAgIAgIAoKAICAICALZQ6Bo2PBzqj9aXZG9GqUmQUAQEAQEAUFAEBAEBAFBIIFA0bBhI6o/+mhVJDB27WuqtlW1ULv2lajd+5qoquomkZ6XzIKAIBAfAk2KqlSzJlWqeeM9qk2Tz1WzRnvjK1xKEgQEAUFAEBAEMohA0dkJIrqGSUSrq5VaV9laba5qr6qLGoHHJv6PfyUJAoJAvSJQvT9RfbUqqq5S5cWbVYeSz+pVHKlcEBAEBAFBQBDgIMAmovv3V6sVuw5Wu1QrIZ8cZCWPIFBfCCROwWihtqmupetVUVHiZVGSICAICAKCgCCQowiwiejaXa3UpupDcrQZIpYgIAjURqBadWj0sTqw2XYBRhAQBAQBQUAQyFkEWER0x57GauXew79wx+dsW0QwQUAQMBAoqt6rDm+6UjUvhts+PCUOE86q9bR169Zq27ZtPrFi+z3b7YtNcClIEBAEBIECR8BLRDGBr69spzbuP7DAoZDmCQKFh8CBjdeqA5tuzSrJ5KCYbSLKkUnyCAKCgCAgCGQfAS8R3b9/v1q9p5P6fH8iNlSSICAI5BUCrRt9qjo1XasaNcqtTYVCRPOqG4mwgoAgIAhkDIGicxO75leG7Jrft2+fer+yh6pSckRTxrQgBQsCGUKgqdqtjij5UDVu3LhODfXprq5PIlqf7c6QmqVYQUAQEATyFgEvEd27d69auve4vG2gCC4INHQEejZ9VzVpklsvkvVJRBt6f5D2CwKCgCCQSwgIEc0lbYgsgkAGEDiq+B1VXFycgZJTL1KIaOrYyZOCgCAgCBQSArET0Q4dj1Kdup/0BUY48B7nGBbVbJZI/If/mX/ju6qqvertlyYWEq7SFkEgZxDIBBEdMmSImjlzZsptFCKaMnTyoCAgCAgCBYVA7ES0/5Cr1CFHnqZO7Ja452X/PvXm8j2qel+V2r9/r/67et/exOeqxOfEd4nv8W+TkjL1z8k3ZgzYi4adoqbOeD1j5UvBgkAqCHz/kqFq/eZtzr7ZuWO5uuGK4er7P/9TKkXXeiYTRHT69OlqxIgRKcsmRDRl6ORBQUAQEAQKCoFYiWiT4hJ15jfvUM1bd1S3nV+kjulUpO57bpd6cdEuTThN8ml+3r1nt5oz/Z6MAPvo3deqUeeeqi4e+/8ik9H9y6eqZ2bNV1//3q8zIls+Fvr3B3+seh7eUfU4c2wk8Ze9MEEt+fBjjWWqZUSqMA8y4wXpt//7HXXt//6xVt8ECX31L7epv78wT4hoHuhRRBQEBAFBQBBIHQHv8U1RNiu169BVDTj3ZtW4STPVuf0+Nf7CErW/ao+6acpWtXLDvhpL6BdWUPPz+nUV6j9vTGW1YuH0X6u2bVqow069Opl/++JH1ZZt25PfYSFfOfu36reP/VPd8/CzavwPLlKX/Oi3rPLNTNkmorCQTRh3WVKEzVs/V3P+vaxeiPBtYy9UV3/7a6pdm5Zq565K9eLr76RFIoWIurufTUbjJqGoNZ8tou3bt1ebNm1yghf2W+TBLg8IAoKAICAI1AsCsRLRw48dqHqddmXS5d6lfbW64+KWiYZVq59M2qA+WJuwjCZd8zVu+qLGxeq9f/9DrV35NguA+269XF07+ix12sU/U6/OX6qwkD/8y++p0uYlye9AosZdc77qOvBaterjjaxyXZnqi4hOfvpVNX/xh+qbIwao/n26q7HjH1H3Pfpcyu2I+uCp/Y5Srzx+m5r1+rtqesIifESXg3QRcBOnas0UIhqsBSKj4+9/Uv1ozIjYLKFUYz4T0ccee0zHok6ePLkWgKNGjVKIUx09enTU7i35BQFBQBAQBHIIgViJ6MlnXa0O7HpiLavnsZ0aqV+Obqd27N6vzrl1gWpSXFrLMtq0RTv10pO3q72VO1iwEEmCtZOIUZeOB6ojOndQf5z2sv7uhUnjVI9uB2sLKVkZG3W/SJdP5HLAV3poax/I1pmXjte/geRenCB/+H7x0tWq11GHJV3zsFQ9PuF61avHYZr0rlm3Wf34zsnapQqL7FPPv6WtrkSCx//uSfWzCX/VspS3baXO/d6v9PMglkggm7aVlmQ1iSfJ+/j0Oeqn3/uGlgkJ8qFMEG1O/X1G/FjLRlZOWFvJJUzkEuV27NBW3XT349oy6yLAlHf1J5vVoFOOVWY5eB6/Dz7lOI3R8oq16muX3aFlFCIa3r1J95kIBUmXiILwXXvttbUacOCBB6oNGzbU+m7OnDnqzjvvZI1jboworJ4TJ05U06ZNS5JRkNCRI0eqK6+8MtBayhJCMgkCgoAgIAjUOwKxEdGa+NDbVXFp2yQR7VJepMaOaKW6dShW8977WF1/3wLV4oDONfGiegNTldqnitVrCXd7lLT61QcSRHCLOuWCWxQ+z577XoIgdlY7d1cmv3v7Pyu1K9lFRInEwWWP+FEQrgXvrdRWQCICZHmlv19/4nbVPWEZPO+qu7UlFn+DlB4z9AZNMEublSiQPRDPk/t2V28sWK4JLuRDrF+bVi3UwJOOVqd+82fqsIPbqUMShM/eQEWykkX0OxecoYkn4lsRS/jxuk81+URCDOGu3Xt0rCZk8dX/xD/e0O0jconY2aGn9VHlJ1ypyeM5g/rptoPwQi7I3emgdppwA99xv5mqCSXlRTlPJ/JDji1bd+i2E5EnjBBGQToRIhrcw8kdjz6LFyQ7ZjTK2HDlTZeIusrM5mYlk4xCFiGh6fYIeV4QEAQEgdxBIDYiWn7wEeqkoT9IuNqbaJI5ol+JGnMm3PJK3fXIv9TfXt+iyg7oov/+kojuTZCYT9W7c6ZEQgRkr8/RnVW/r/9Ux4LCTX/B2Ser74w8XQ39zi804aLNSS4ialsc8Xe/XodrUkqWUwhkuuZhdaQ4SfwGdyoIKJ6F6xp1l/W6RG2cN1HHdR5/TFdNOkm+vkd31VZGkODFy1YliZ3ZcDtGFCTwT0/OVu8tX5Osi1z0tAkL8oIA+uof883Bun12wvMglyBAIKVmggX1zAG9NOGmGFzbNW/+DeJJFlsqB1ZRkGUhou4ubseEBm1gijRArMz5TkTRHCKj+CyW0HR6gzwrCAgCgkBuIRAbEe3e+0x11IkXqNLifer6YS1U/yNL1NKKjerWB2erDgd3VScem7CEVu8HC9X/vrpog97AtGzxbPXJinmRUCESCAseESgzrhEWSZBCJC4RPSPhZoZVMIiI2vGiJhGFZRCEE+54uL6JICN84OtnnpDcRAUZb7hyhJZ5y9btdXaeu1zzZhtMAm0SUdqcFVY/WTLN9hHovrhPCjcAuUfogrlr3nwWZBPJtaNeiGjdLh60MSluMloIRBTolZaWahB37twZab6QzIKAICAICAK5i0BsRPTks65V7ToerS4e0ExdfGoLNfkfC9XDzy5XxW16qtGDOiT+K6+Fwq8enaNe+qCDevmpX7DjQ80CYKHcVblHLXxvVTLGE+7ktq3LEhbH1dpFH4WI4jxHWDgpdpPrmifCC6LVtk1ZUh7IgkQhAiChsIrComm6xc02BRFR5IGl1XbNk0scv/vqN8k7QhYgz+Cv9tJxrDYRhRzYKPXCnMVq85bPFUIEEIOLtoZZRIkcE2E2z8kUIlp3EkAf033UcVYo9HXlhYOSfTudKaRQiGg6GMizgoAgIAgIArmJQIKIDq9e89HqQOm4xzed9e27VOOmzVV5y2q1Z/NiVbG5kWrR+tDk+aHd23+m9lXtTlhEq9VnO3arDz/ZpZq1OTxyfCgJirhIbPyhTUH4Hi57bKChjUxRiCgRxPO+dqLeaINNTCiLYkRpsxJtNoLL+dYEiaM4T5uEkQWSQgTsjUIPTHlek0AuEQUx+XnCVd69y8H6kTcXLtfhB3QqgK9+PGNuxsImo+deWag3TLmI6DWjhyTrMjdmhRFR0gEs0rSh6+6HpmvyLUS0/iaATBDRn/zkJ+yNSa6Wczcr1R9qUrMgIAgIAoJANhCIjYj2+eq3VHGzspqrPFUj3OKZSLjQ84vPuN6zKPHZSGsrFqlVS2Zno51ShyDQYBHIBBFNF0whoukiKM8LAoKAIFAYCMRGRAsDDmmFIFB4CAgRLTydSosEAUFAECgUBISIFoompR2CQAACQkSlawgCgoAgIAjkKgJCRHNVMyKXIBATAkFEtDoRr10TSpP9VJ+u+fpsd/aRlhoFAUFAEMhtBISI5rZ+RDpBIG0ExCKaNoRSgCAgCAgCgkCGEBAimiFgpVhBIFcQECKaK5oQOQQBQUAQEARsBISISp8QBAocAQ4Rzba7Otuu+Wy3r8C7lDRPEBAEBIHYEAglopi8d1c1Uh/s7RFbhVKQICAIZBeBHk2XqKZN6icWNLstldoEAUFAEBAE8g2BoovOHl69ZI37QPv9+/erT6taq0+qOuZbu0ReQUAQ+AKBQ4s/Uq0af64aNap9jq8AJAgIAoKAICAI1DcCTosoLKH4b8++RmplVXdVVd24vuWU+gUBQSBFBIqLqlS34uWqSaNqIaMpYiiPCQKCgCAgCGQGAU1EV674sFbpVdVN1M7qFmrD/o5CQjODu5QqCGQVAZDRAxt/opqrHapJ4jNSVVXNvw0xNWnSpCE2W9osCAgCgkDOIaCJ6HPLSnNOMBFIEBAEBAFBQBAQBAQBQaCwESgamogR/ef7QkQLW83SOkFAEBAEBAFBQBAQBHIPgaILE0T0CSGiuacZkUgQEAQEAUFAEBAEBIECR0AsogWuYGmeICAICAKCgCAgCAgCuYqAENFc1YzIJQgIAoKAICAICAKCQIEjIES0wBUszRMEBAFBQBAQBAQBQSBXERAimquaEbkEAUFAEBAEBAFBQBAocASEiBa4gqV5goAgIAgIAoKAICAI5CoCQkRzVTMilyAgCAgCgoAgIAgIAgWOgBDRAlewNE8QEAQEAUFAEBAEBIFcRaBgiegxRxyszhvcR81+63316tu1rzDNVWVw5JJ2cVCSPJlGoFD7YaZxk/IFAUFAEBAEaiNQsET0wLYt1V/vuUItq1ivvnvb4wWjd267Hv3FJapnt4PUCd/8VU603ScPt1050ZgvhBhx2nHq1quGJkVasmKduuSmR3NJxIzJko/6yhgYEQoe972h6pyBxznHJfpT9y7l6t5JL0UoMX+z+uaE/G2ZSF5oCFw66BB1YOum6tdPVbCa1qdrS9WuVbGatWgLK39Dz8QmojRpmIC9veQjNXn6W7Usjpho+/fqorBQISHPs7PfVdNfeScS1rC4jDl/gOpzVCfVonnTZFl2fWGF0qR/2S2Pqf98sNaZdd5fblQ2gSCC8fPfPxdZ7kiNTDEzp125Nslz5OG0C/oy06q1W9QLbyxVf/jraymimfpj6OMnHddFF3Dp109SO3ftKSgiinHQp2cn9dSLC53jh6Ov1NFN7UmfzKmVGt9TYUR09p/G6rkuV+ed+FCoKYkzJ8RZZ673jTjbKmXFi0BUIjr23M6quHGRemHBZrVw5efxCpPl0sDFhgzoqU7p0011PrhtHb5E4vzw0jPUjsQaSGuxi1sFiR6JiLZr00K9ubhCl9WudQtNEpGuuWOqXqi+e+FX1ZXnnaJADhYt+7hWngtveFht2MJTCBr+u5sv0pMyiOya9Vt1WQP6dlMvvrlM/XTC0yxVoJxHbh+t5ixYocbeNc35TD4SUU67sj3J+xTCkYfTLugL/Qj9sLRZU9XryMSbaoIQhunYJ1scv6N9SIVkESXSFESMOPqKA9soZfhkjlJWJvKGEVFM5D26dFDj7n+WPVdmQsZslcmZE+KUJY6+gT4fZNSIU1YpK7cQiEpEB/Vuqy2o/3x7s/p0+97cakxEaWjcgIt16tBGbd66w7nOYW02+VnGiKi90JLlkCr/w88uVsf3PFQNu/r3yYkUJKG8bVmkwUsT1MSnXq9l6UplEpjwPyM1YQ4iwvlIRKEHX7uyPcn7+jZXHl+7XPqifleflqSGSEQ5/dDXL+L+PQ6yEbdMZnlhRDST9eZi2dw5IS7Z4+gbkLk0YSCBF+ZvLy5qEC8MceGfz+VEJaL53FZbdvCujVu2674etM4RFzQ5WyQies7Zw6unv1/qxS1IALMykAhYLX/4qydT3iAU1coCAG64bJB6Y9FKp6WUyntm9jtq/IPP1Wknl4hiEuvdo6M2TVOCYkC6KX172AnqG4N7J/PgDeK+KbOTJJzcynjuzonPq+tHn67z0t+0qYpTl69dJpm/eGg/bV2GpXrS3+cmww1cOrW/48hMsgAHmOYRl2uHY7gWHViAIJtJIH3tcunL1WfCdHHq8Yere288v4513fW9T6ekex8RRTnfGtZPW2+B0cKla9QvHpqZXMjMBc7UFxa7KTPmKRrosPySJ+Lx5+Zr/JDIK4HPPplddSE85aFpc/S4pQU7aFIw4445+kI5YX2e0w9RRti4iCKzd7JLZADewwceq1+sSX5Y4c05xKdT6PqmMUP0nIiEF/adu/fUihG144yRz47r9ukLz9h1oZ9s3rYjMB7VhQGNR8zfbVqW6vhnmjdnPHBVLUuIr+2ceYM7J/jq8vWfKH3DN/cCo0En9ag1jp98YUFK690V53VRl557mDrx2AO0OtZvrlSz521Ul978b/33Sce1Va88cqr+vH1nlXpvxedq0tOr1cNGnOLuf5+bfPa6Xy5Sd/7gGNWtYwtdFv5+5uW16tCDmqv/+1lfdfqJ5Trvy29tTIzHSnXRWZ1Us6986V388eVHqmsv7qY6tCvR9c1791P137ctUB+t28UZMjoPV+/Ia3qPbB1y+jxbqBgyEhF9c9k29ZUjWmm3+9YdCYze35Z0vSMu9My+7WrVZseUopymTRqppWt2JMvZsG2PmpsoF99FSb55ftpvrlTt25TVWhvIaGNyNM5cR3IFrXPkDTfX8nojojSpYqGdNXdZYGxZGNhUBhZZTtA+AQDSF7QpCeAhrMAkjSQDh4jSBE0hB/QsFleKh0CnGJsglqbb+OTeXROkozJZL9o2qH8PvTBhoUDaVblXDU58h7JH/mCi4tRldoqgdtEkj3KXr9qoH4E8IKQUM+ubwPEMR2baQY38CKHo3rlcT9Rmp3QtOi4LOsqIqi88Yy6UHF1gkDYvKa7VJ3459lytCxqknHJ8AxS/2+XAvQGCY8YmEz7oP4vf/ySpL3wAyezWsb0mBnhm5pwluq9R2Ao2vxBh4MhMdaGs5as3JsNeqA9Cn6iP+irG4vKKmj6EZMd7h+mL0384/dA3LqLKzJmDMI/hBRfkkV5C6Y0/ik4J5+6Hlet5CGODyKYZZ0x4u4goNh4G6YvGjJnHVVdYm2mso49h3B7SobV+yaEXfMyTFP7CaTtX7/aGSntO4NTl6z/cvuHrYyZ+1D7aw4CxQy+NPpzxO0jo727urQnf7HmbEutElTrhuAM0ibxn0gfq5vv+o4no1d/sqos7sG2JOvrwVpokXnPHoiQZRTkjzzxEk0wQTKQdu/ap4acdpFZ8vEMdfc6Lav7U09WxCfL07gefJYwin6ljEp/LDyjRZRERBQkdf23PJBnu2qmFJsh4pt9FL3OapPNw9Y68PiLq6/NsoWLISEQU5HP91kpdYreDSjUhnfrqOrV64251QFmx6lzeTP92xCGlqmuH5nU2N1E5IJ/4r0VJY50P5T40cw1bUs48TyGOqxN9E1hT/za5lcnZguY6U6ggIkprpzl3ZY2IEhhmXACsSmNGDtA7tpHw228efYntwkhloxDqXFaxIbAOl9mYwOUQUddbhN1jaDG+/JbJSTkIH5OQ0du5aaE1SRqnLqo7rF1UprlRi/JT3b4JnOrxyWxjgcUVxNDsFzYRJeuj6wUiqr5Qv9kWji5c1ljIjBcDvBBQmSAOPp3a9bv6BsaDqQtbP/S32VfoJYuIoGmhQr+FHhcuWVPLcsVpu6tvkDfDnEi4rswwfQELX//h9EPuuAiT2WV9NHVF4yKoLjxPJNyFofkdXFnoT/ZGSHxnElGz/iC3vU9fNN7C6uK0HUfdwVMw4bGXVbdD2yuQWSIM1N9gEfa1nWIoOXo3iahrTuDUxek/Zj8MCuHh9jFTZ1jsb7tmWOgmDhe7+Nek/9JE78IfvaWtlpRALE2Lp/ksLJvLZ3xNPfvKOjXyh3OTP0264yvaujn1n2uS1lQin92HPa+fsQnlqufPqkVEKf9pl72q5r5Ts9Pb9R2HKXH0Tv2KynNZRO050zVHhcnjslCa+d9dvV2tTViObSumnee5+ZsUEUginchD5aMc5DHT0H7t1bGHlQUSUbOckQM6OElrWNs48zyeJw5CnjT7FCHOXGfKEURE8T1CVmjtxDMZI6L2ZiVY9mA1MN2CJDQmvguG9NWElCwtnE5skyXOM5w8WACQbKsoh4hCmSDX5N6GhfHd5Z9olykleze3KZOLiAZNhpy6zLKD2uWyQFLnSJWIBsmMhXDUiBNUnx41m9eQ6G2W3npJHtRNG8/Qd8bdP93p1oqiL9QHCyftWufoghZvsvLQImi+LXLKofYGDdCgAWkTJp++bMIZREQ5MrvqcpEgLhFFG4P0hd985XCIBHdchNVlWu5dcwYwBtHE7nWyIgTNLa55w6wbz5kvDlSOy3JAv/mIqOslAd8FzZlmXdy2U7+C1f5f8z/Q4SQPTn1Nt4XGv6/tRNY5esc8ETYncOri9B9OP+T2McwdCME68+SjNAGlUJsoLvpNrw5TFZ/sDLU2gnjeeMWR6uReX4aDkWXTtFISETUtpdSnyPJqklQ9X957kraakkUULn6brIaVGzQuODhz9MWdo8LkOCxhnTymc1lgFpDQLYmNRL482PUeFCP648SLQypE1HTZB5HWsLZx5nl6ngg8vG2mUQW/c+Y6U46gdc70mFD+jBFRsnKiIooF9B2nZLs7w8DFbxRzFuZq95Xh+t0VwxBEElxWWZp8enY9KOl6Ngk2KfSJmQvqVD/3nYqkldQ3OeNhX11mBUHtcg1kIlthRNQkdPYCGUREKRaFTPt4Du5il+vZlD3s3M0o+iJSSX2Gqwu8DWKX8sDLJyjqp6bVklsO2pQrRJQjM3eSd1mNg8ZekL5SXZhc/ZAzLqLIHNQWzuTsI0goO5eIKHfOJCIKa+hdf3xBn15yzyOzdFsoZMXX9qhENGxO4NTlGnuu/sPpG74+RvMEZMbCjrk0lU1LHCL63jOD1YEJFzq57lEnLJ9RCGO+EFFbX9w5ituv083nIqJHJcIXRiRCIuqDiHLmeWoz9dlMEVHibBQ6RPWCA9AY8R2vWJTuZiVTwRjE9hFNBEKUHc3UCe1d80Gdyeeap+dcCwyZuE1LKZHFsE1XNsEmUhNk4eOSOlcbfWQ+qF1B8VfULtucHuTi47g74c4jC7GrHHtiIZeA+Zzddle7XAsTvfFRf+HqgsgTZIDlB8eEmXHG3HKIiAZdIMB1L+J5c5yQW8Xlgg+yiHJk5k7yPpc7R19cImq6dYL6oV2fa1xEldk13jguWp9OUS6OjrNfqNNxzQdZRINe3sPqCppL0a4t23bqOHbUh78/3rBNx05T/b622675oLmfMydw6uLOY6n0DbuPET6zEiFnUc/GNjEPcs1THiKQ4367RP36T+/rr8k1H4WI0oantxIbj/7r0n8lRciGaz5M777xzp2j0iWY3OddRPTi/zpIdWrfTE1PxObaG418rvl0LaKceR5tM8MnMYbt+Ygz15kYuV76qI4gLDkXvcRKRCnGDq5rBPibLlhYnbgJxHL8dSOSrnCcSYoEdxFiHMxNTJzNSjYJNK1e9JYcthEARAxnVqJNSOYZqtQukhm/Y0c0dqxSQlxVUMC8aS2lRdtXl40jEUVXDCJIDBJdMmDGbRKBo00vFBOGEAxsNsOmGNemFVNm820I1uBWZc202woJm4FQDvRlTyx0Mw/yBR2t5WoXCBhtCDPbZQ4wny5M/EB2N23drl1s9otPlHJowTL7EbnquBsuKIzFtbmMNiuRNTuIiHJk5k7ypo5Ma3fQtbm2vrh93tcP0X84YxB6jSqza04y5x/afU7jns4jjqJTc3zZm5VQF3aoI9FmJSzgSCs+3qRP3ODoi/KE1eVqq/0dkTp8j3gv6KbTQW1qbezztZ2rd86c4KsLcnL6D6dvcPsYB0dfnnNOP1j98efHq7LSJsld7DUylqihV72e3DEP0vn7qStV28SZlJd/4zBdbItmTdQzr6xVk5/5SB3bvVVys9JDid30C5dsU8+/vr7WTneK9QQZXZnYmZ2pzUpcvXP0xenzPozj/J2IKKyfSF0PbJ7QQ2O1LLEh7Jm5NZvEYCFtVtxIf6bNSjjQHgkhANjQ5CK0qbjmOfM8Z7MSZ64Dx6ILisAjkOg8+aDLTpAnY655FB52YDcWY+yYpiOOuO57V4cBiLhZiXZgI4/rFh3f8U1m2bRIgSiaB9xjAiKi5jpahwYOlRXULih11IgTtbvXvA0KVragI0Tst0ZuXb522eXQbmszrhV4jL9uuN7BjTbBBUdxvSgfpAfmdTvZMqOjIh9ZxPEcyqSjb8iyYlsM6SUg6JICl77s2Bj0idcXrqhzwkKYLsz2EE5ov+tliVuOiSXKB1lGbB1ZTThH0AAfkGETyyNggsUAACAASURBVD/PmF/r+CYfEUXdPpmjTPIYX7g1yjy2LMhTYOuL2+d9/RD9J8q4iCKza+4JwtB1XFvYkVx2n3Ad30S6cMlhhtDYY8eOJ+XUFdRW83sql17s6G/bqhHWn7l6d/VD15zgGzuc/kNtDOsbUfoYB0tfHpDRH13WXR3draUmpEg4dqnz1/6pP9/x/WPUqOGH6k1F+H7ysx+pU7/SLnncE+I+4aq3kx0rCkvqlLtOSD6HzU7YpR/38U1cvXP0FWWO8uEcx++0oYjKwo73pR/tUHMTxzdRIpLpqo/c93ERUc48zz2+ybVemHNd2BwV5unOCBGNQ5m5UAZNNmHXfuaCnFFlkHZFRSy38rsm3tySkCdNofZDXutzKxe53ezjoHJLSpGmPhCg0ADzHNH6kEPqFASAANs1Xyhw0e7RIPdivrZT2pWvmquRu1CIaKH2w3zrXWSdRtiJeaRKvrVD5I0fAVhIFzxxhtrwaaU+Z1SSIFDfCDQ4IlrfgEv9goALgUIhoqLd+kHAjOOiGHaEB4VtBqwfSaXWbCMAF3+nDjUHrSMG9YTE+aUIBTA3QmVbJqlPEDARECIq/UEQyAEEhIjmgBLyWAQzPg/xXTgBwr5mN4+bJ6KngQCdB4oiEGu6au3OOleFplG8PCoIpI1A0dCzh1X/8/0WaRckBQgCgoAgIAgIAoKAICAICAJREBAiGgUtySsICAKCgCAgCAgCgoAgEBsCQkRjgzJ/C3Id6l+frck1eeoTi0zWTYcTUx04uPynE57OZJVStiBQ8AggXndAn2612jknccSc73aZggcmoIHAq8sh7fSvUeYfPIebDlev26ImT59X5zKdhopnPra7wRDR5gM/Uo06blc7pvRMS0/plHPAz+eoJl22qY2XnJ2WDK6Hy65YrJqftialsl1XmoYJGHQndlyNiipPXPWmW459RqHrmlrk6d6lvNa5p9Reqp9zE0W6suJ5uhxg89aayxcKiYi6cE4Fs7jKiVJ3psdXFFmQNxV56ivmOS59ccZyEI4mEcUNQjiDl86EjYp9IefHyQ4PjLsoeUZx0HnSYThfPLSfPrMb50A/NG1O8na/VHGLq/+kWn9DfS5viSgRL5fiqipaq09vHVDrp/b/N1MVNduntv/xOLVr9qEp6zudcnKViAIM19VdQSClsjBFBTyKPFHLzkR+HAp8743n6wmRbiHCphHbCoKbnDBxmgcBY0I+6bguWiwcHr8zUUbYxRFxyU+3M+Hmr3xIWCT69Oykwm7zoHa4cE6ljXGVE6XubIyvTMtTX0Q0Dn1xxzIHQ3rJTJWIRunzHHmykYcrM/WRVLGhttClNvj7mjum6tvIUk1x9B9f3bigAbcP4gWF1osolmCUj2PyhgzoqU5JWN5RTpjxAusL6oOV/rBEXtx2mGtzft4T0V2v1L1ZYv/WZmrnk0fW6g8tvr1EFSeskZ8/1Evt21BzpV4qKZ1ycpmIRsEi1xbKKLJnKi8d5B506xDVi5tjcPPWuPufdbqSsknA842IUr8Lu82DizO3H/j0xS0nSr50xxcWZroeNEq9QXlTkae+iGgc+uKOZQ626RLRKH2eI0828nBkJlxwha55y2Gq8sVVXhz9J6wNJCddU41rtXFrWlRrsHkDGq4+h1fLZbzAS9VPrvxa8tZDXB+9ZOW6nAsTyXsimgk3d6qDwfecEFEfQvn7O93Rne7h4UJEg/sAZ4HL3x70peSpED+z3XG/YKQiT30R0Tj0H9dYhixCRN9xqgTXgQ/u30PFecMhXWnpuqo5jn4RRxl025nZbhorw67+PTvOFRbRjVu26/xBawZdagG5cX03XTcdRzviLqOgiSjiOcu+U3sg2MQV5LCo2V61e+4hqnTISu2+h2t/5zOHq8r5B2m8OeUgH8IFmvbYohoftDOpp/3bStTm6wbpv4mI7nj6iMC6kK/07BWq2cDVyXL2LkuY8KcdqfAvUuMDd6qy0f9RTXtv1H/vfvNgVV3ZOOUY0bBOhc5805ghakDfmuB7133Z+N50N+BvxEfeN2W2dpOQq8t+6wv6Pt1OTgvJC28sVRRDhDvp//biomQMEd1Zj4F858Tn1fWjT9cuDvr71bc/1GL47rkmWcMIpB0DimeCrl30EdEwnKPiFkZYXHLY37lwhosIsVqEHwdDn76C7rCm9hKWPpxNq0mfo2o8KY8/N1/3ESRy6/nKQd6w+5dN3fr0xR1fUXSbDhHlyuNrF+Ez8anXa43BSX+fm1wQOX3MlgcWtM3bdqhzBh6XHENcfSFe05wTXH2VdIt/g8JjuHMCykiViHL7PAjJI7eP1t0Dbt5lFevrnB/Lmes4OPvmea7MhHG7Ni0UyFdQQnm9e3RMxpAin3kHuv0c1e/zStnPxd1/wsYqXP+rE2uR2bcQU3zleafUCteKMt6D1gzui3tUnKPIxs1b0EQUhK3p0Zs1Fk2/sk4TNxcRxQYikM+9q1qqxm0qdb5960rVlhsH6mc55cBlDyKL5/Z8QRjxrBkmQEQ0rC6Q0BbfXKpAYCsXlquikn2qpM+GBNFsUofQUjnFnT9XjQ7YrRq1rkxps1JYZ6EFBZP28tUbFVwJmEAwcdGCi4l5bILIkbuhtFlTdXLvromJsTI50eBttXlJca2Jh96Ko04cvs5NMkOexe9/orNDHiSTbAxKvJGDYGNxQ9pVuVe/pYO0wrJptwsukON7HpqMx8Ei0K1je/0sYjuRsNBSmvtOhcbEjAGlOlMhohycfdiYv8dBROFWor6BG32AJ+GHunwYIo9PX8gDnAk7kMflFTUvYUj0pu/DmRYcyDtzzhLdZ/HChMPfQWwoVs1XDuo0bzLC39Q3wtruGhec8RVFp8ibDhHlyMPph1QO8IA7EAljEPHRZA3iEFGOPBx92eXYfZU7ljn92dRXqkSU5PH1ebpSF3WiTd07l+v5xgxfgQy+uY6Ds0/vXJkhq4uQmbjBRY4XRPSfRcs+Tv7kirunH1MldHH0H+4YxdjEevOLh2bqOWThkjX60VuvGpryhrYgIorvsVb/ecZ89a1h/ZLuecxztHchFZy5bY2SL++JqB0jWvlap6Tl0AQiaFc5kcOtd/RPPtf6hnlO0orygsppc/ObqjhhDf3svuOTllRbEa666DkiyMgDUrl1/MnJWFYip9hotee9dqrt3bM1cTY3ZLW7f1bsRBQDdMYDV9UJhMZ3JhGlDn/5LZOTrgWatGhCpA5vTpAoB+QvXXe2jTNNqmZdNEmBxNw76SX9CL0xmsHypkuRPrvcKPjuvMF9NIEJSq5YRp+bM8wiysE5yuCPi4ia+FB8HRFtH4awmEfVFydG1IWzTQqo/VgMghYCn76ANyz7468boaE3N0v49IUXFc748unUZdExn+FuBolzvLv0buPvI6Jcecy2BunLJY/ZV33WPOpznP5sypMqEaUyuFYtyk+Y2d6nsLkOVklOP/T1Z3oh5MiMsRe2wYZc2FGMFIQ1Z34IGlOp9h/fGKXfac7B31g7gMETMxdkhIiiLhB5JJB5ehE2XwZTwZnb1ij58p6I2o0N2hXvI6KmpTTsKKSg30AWS7++XLv2YRXdmyCK++Di/8eX58m5YkTt8sof/Ueg/tA2JIQbgIBvf7hXMm/LqxeoZv3XxmoRDZpEyZJJZINcPy7BaVKgCZKC08ktbxLDKB03LK9JJs18NvHyTZiuyTLomTACyVkoKU9YORyco2AYFxE1rbv2RM7BMC59+XDOBBFFv/7T7aMSlr6SxAa06bVCEnz6grwuAmyPL59OTauYucDBg4EEos2JD4tzvHN06iOiXHl8esfvLnnCSAeecbnmOf3ZlCfTRBT9b9SIE1SfHl9u2iUvhSl/2FzHxdnXn6MQURDfoA02wA+GjDEjB2gLOlnV313+SejxTC5jh2/s2L/7iGjYXMepCxjiJWHys2+p7397oPbIfLJ+W8aIKCzILgMRvZymgjOnnVHz5D0R5W5WyjQRBfBw4Zecukbvzm/S5TNtoTRd/BwiiuOh9q0rU7tfOqyOLmENRahBrhFRcrPgzc5O5J7G93j7wo5xBJNnIljdJHOYjG33d74TUS7O3EkgKhFFeIV5tBRncecs3BzSgjZFWWiyYRGlcxDbtymrQ0Ihr09fOLIrDiJq6ztV1zyXkPjaRRso7DFIL5+0CLqIqNnHuPIUMhH19XnaoENHxpkvInETUY7eueMUusdRQmEbi+jYIRxaTyEHZuhLEImMYkXNNhENixGd8NjLKZ2DGmS8cM29ZBAyvSRRceauL1HyFZ01dFj1zOX5d9d81APcs0FEbeDJSknueg4Rhau+SefEMVP/19vp4of7H3mwcQnhBJQy4ZqnQHj7YHbbNU8E07YI2XiQexwDDjEriM377m2PR+mvrLwuVy+FCpgD0GcRjeKGi9Mi6iLRJpH34cwCKZHJR0SxuYPCJlxuUg4R5WDI1RcRE2yA8d1Sk2kiah7GHbSA+MYFd3xx9Un5UiWiXHl87YIcrr5huwGRJ6yPceXJJhHl9GdTnnQtomF9nn4z+19QOEPYXMfFmaN3tJ0zTlPZH+B7Jo5d85m2iEbdNY+Xt2UVG0J30wetPVinkUyLqCtEzZ5ffDhHnY84+QuaiJb0W6cale3VONBmJXJvV61tockchxxyygHRxcYi7F5HwqYnkEakTf89RP/Lravlfy/S+SHfvq0lST2SK57Kwe9ViTCAbGxWok0drs1KZozcwqVr9K5WSvbBuXgj3LR1u94JySEUnE5s56HFImijBPK7Nr+Y1lvkibIxIYyIAp82LWvOrqVNAwhZQLLPe6RJwNwA9OQLC7TLNwrOHNzCCAvFz5l6R5kIfseByIiz5RBRDoY+fdEB1eZxJKYFaPZb7yfxCcMZOjctkEExohx90YJib6YARtTnOfqitoeNL44uzTwoM9UrJTnyRGkXXvyQ+vfqouPKzdjFKH3MN//4xhenrxKGYWOZ059NXaRLRMP6/JbPduod8xRn2KqsmT64HAkbQzFOsSmPM9fFpXfU7RunyEPWcdftc/gdhBAxjTt379HtwUYsOunCZUVN5xxRzniP0n/CxmuUc0SJNLowMjdLYmwhvbm4Qv9LF34QqbY3ESPPhTc8rMltVJyjzkXc/AVNRImwucCgGEsOOeSUQxucqK7q3Y1V1arWatfMLknLJqcuPA/i23xIhbaMIuYUybSAIgSg5ZjFSaKb6eObxl83XO8WRwo6vgmDedSIE7XrHXE9SK4BRIsPjhrJ1HlvNGmA6CIgHBMjBh12D06ZMS+5ScnuF64gd+5RLWGLF8nj6of2RhLIauINuR+c+loyxo+LM2cCCCOiphzQFc6hu2BIX334MhLCHriTsw9Dn77MtmAixwkFeJGhRK44H872pqQgIuorB0QzLI8ZEuLTl63voPHF0Wccebjy+NpF45xkopMKMP4ocfoYR54o+uLE+Pm8G77+bPfXdHZEo6ywPg9CYs5xmE8wV9N8jb9dGyrtuY6DM2Tx6Z3aHiYz5aGXOdpFjrmOkt1/6GiqydPfqhWHjfzAwD6CLcpYiLv/+Orm3qwEDG+4bJC+qc++eSlMZlO3IJr0EkhrMh2riL+j4OxrVzq/5y0RTafR8mxhI+AiSIXd4tRaByKGyb++75oXfaWmv4b6FBGYoCPQ6huXQrlrPtM4g/ze/aNvJF9uw3bRu3RqnnQAohpXyFJ995+GWL8Q0Yao9USbQULMXe+F9BkTmmm5g4oLub3p6A5YIZnXzGUbK9GX9E9uH6YYSNq0ku2+ypETnhjc602712l85Spxdi2B5F5HGFXcx+vZ9cHy16dnJ/1S7Iv9Np81n8NlJaZFtYEu63nbbCGieas6ETwIAbGw5VffEH3ll76yKa0ZC0dxggj9SXWHcTZlz6e6BOd80lbhySpEtPB02uBbJMQmv7qA6Cu/9JVNaU33KyxeOGnj2dnvss5Fzaac+V6X4JzvGsxv+YWI5rf+RHpBQBAQBAQBQUAQEATyFgEhonmrOhFcEBAEBAFBQBAQBASB/EZAiGh+6y8W6eEaxfmQuHO4ISS6EpHOnyyUNhdqu3JZPw1t7OSyLkQ2QUAQyE8EhIjWk96aD/xINeq4Xe2Y0rOeJPiyWjpk13WOZr0LlwEBaEfosor1kW92Crp5I0zMbMVAptOuDMDcIIpsaGOnQShVGikICAJZRaCgiWj5o/+oAyYOhjcPmc8q2kZluFMeh9Xjpqdds2sOi6/PFHaIMx2TQTc21Kec3Lp9MhOhvOyWxxTd3MMpO5eJKORPtV2ctqeSB1baIQN6qlMSx9ngEPqgswJxpzbOAqTjW1z3JKdSfzae8R2Ang0ZpA5BQBAQBPIVgYInovu3lajKheVaP7iCs6TPBv152z0n6NuK6iu1+PYSVdwlcaf8Q73Uvg011z/mavLdyZ6LcvtkpvuVcavH2LumsZuQ60Q01XaxAYiYkfDCLVudOrTRh+dfctOjdUoB8TSvf8wnIhoREskuCAgCgoAgYCBQ8ES0qqK1+vTWAckml57/vmpx7gdq58yuOeEWz4fe6CN1udgGjsy43gz3F9O9u5x25DoRRRtSaRen7ankATHeuGW7Pmw6yHJI7m0cBJ6PFtFUcJFnBAFBQBAQBGoQaHBEFLGZZd95R5l3zQMIk6ziTnjzO/xd1Gyv2j33EFU6ZKV2qYPg7nzm8Fr3yPvyUN1m59t4ydm1+iKnLtw1Xzb6P6pp74362T2LytW+rSWq+WlrlF1eOh3dPFvOVQ7dFEJWOOShO4Hts/5g4UICIblz4vPq+tGna1ct/f3q2x/qO+FvGjNEDejbTeeFtXLzth36rmTzVhLzrl7kg7WN7s/lyoznSG77vndqqy1P0D3gYfKgLPMuddyJjAO5cTPMpL/PTZ6H6CJp9ndcfHztgkw+maf95krVvk2ZuuaOqcnQBbryj+52RzkgkcMHHpu82xr6fHNxhcJ97HYKIqI4TPvK805RZoxyqhbRuNrlKyedcSXPCgKCgCAgCHyJQIMjoqVnr1Atvrk0aRG1SSegcRHRJgk3Osjn3lUtVeM2lZoE7ltXqrbcOFCjiWd8eUAgmx69Wedv+pV1ugwXEfWVY9dV3Plz1eiA3apR68pYiSgITbeO7dWg/j00OXz8uflqeUUN+UWa/so7+l/arY3PuP2ke+dyTSpNYgHCQuWAYCLtqtyrBifKpuv6iLAhjnD56o2q+2Hlejc/yiIiCoIwNkFiifCUNmuqTu6dsG7vqtS7/rkyUxvCdj3HIQ/qoXLQzuWravCDzCCkFKPKIaIceTjt8mFIOv3dzRep1QmZ4UpHDCdINPrAvZNe0tWQJRMvH28sWql27t6jevfoqF8wTOumKRM+2675X449V/cD82UjFSIaV7s45SQHgXwQBAQBQUAQSAuBgieiQTGin95yqo7NjEJEt97RPxlX2vqGebWIJJHDsDympsquWOy0YPrKAZlte/dsTYpNK267+2fFTkRJXo6b22wb3QdtxvzhdyrHtEASuQKJnPHAVXU2s+A7k4gScbz8lsnJu4WJOJjElyuzyy0MWakN9uaaVOShNpobo6hewsJHRLnykB6C2kXEGATfhyHhihcHhDDYpwy4LKREUOklxewXQRZRfF+aIOXmndapEFFu3/C1i1tOWjOvPCwICAKCgCCgESh4ImrrGcR0+6RjarnUkcfnmoeV0rRe2kSSCGRYnihENKgcO7SAymx59QLVrP/aWC2iXCIKkjRqxAmqT49OySb27HZQHVIZRg5tYkYF2dYycvG7xm8qRBTlgFwimeeoxilP0PFNaAuXiHLlMXFxtQu/czFEXsSbwhoOC7RJXPHb7D+NTVpMOfNpEBGFPPamsVSIaFztilIOp92SRxAQBAQBQSAYgYInoqblkKyYptXSZRFt+6vZqnp3cZKcckgmJ0++ElFyywadM0rxhOSeRTsR12lbE+MgokR+npi5oE6vnvtORdJK6pPZfNgVo8glfhx5XET01OMPV/feeH4oEQWuOxNub7iyufL42mUSSB+GyEsvApkiohTPSuEYJD/6D+oEUacNTL6JnKMLKoPTLg4+Ppnkd0FAEBAEBIFwBBoUESVrIjb34PgmJNocRLGeLtc3h2Ry8sRBRIt7bFFtbn5ThwiAUFPKpGs+zM1Lv0147GU1ZcY8LU6QGzmMiBIhwcaj7972eLJdtisc7uAeXTqocfdPV9jgFJTCZHY9Y1v34pTHRURtt7btnrYx5Mpjt81lteRiSC5shFgghtPWTZBrPkgnLoso1RH0jOvcUZD4ZRUbki8d9Gyc7eL0MVlcBAFBQBAQBNJHoEERUSKecLOTVZSspCB2VYnNR9j4g4TNP7vfPEQf8cQhmZw8Jf3WqUZle3X5tFkJB9ojVa1tocklpxzKY8qcic1K1L3oxh78bVo9cUXmls92qkduH62tn7AgtSprpr4xuLd+tHlJsZo1d5maOWeJc9OTacFEfiJsIDxr1m91blYCCRl/3Qhd/sKla/SuekrmTu0wmV0E1nUQfFzyUDmw7iH179VFk3UzhpZc4GbbkRexnMAQG4Q48thTgqtdHAxBfH2blagcbLqiEw60zIkNa3Q2K6zNaCu1G/9iVz1S2AUJQa55sl7bpBjlxd0uXx+zsZa/BQFBQBAQBKIj0OCIqG0VhQW05ZjFCpbG6t2N1Y4/H62anbFa74BHQqxmFHLIiSN1qck8TsoXj2rKjLJ2v3mwqq5sHPvxTaacsDBe+vWT9I5oSnSMD8gBXKkgHOROPb7nockjfUDA8LudbFc/nh9/3fDkc0HHJYFwjBpxoraMggQhuYhJmMy2LERcQW6JRMUlD5FMqhOkHeScLMj43qwLu9DveWSWumBIX4VYWyTsKOfKY7bN1S4ibWEYco9vcukCfYDibYk8u/p82JWyQUQUOr3hskH6heinE56uU6yvb6TTLlcfiz7lyhOCgCAgCAgCJgIFTUQbkqrhrgeZjvMc0VzAj9y/5tE+mZKLCGPUaz8zJQ+nXA4++dguTtsljyAgCAgCgkD+IyBENP91qGAhPeD2V9X+xKH2FOtaAM3SFsC/3nOF2rR1e62jfTLVNjoPFSEHYfGnmao/arlcfPKtXVFxkPyCgCAgCAgC+YuAENE81B2uKW3UZreWHIfrwxKK2552/OUotfMfNbcS5WMy4wkRZ4izK+F6NzdC5WO74pJZ8IkLSSlHEBAEBAFBIFcQECKaK5qIIAedYYpHcC7qvnUtVOWcjmrX7EMjlJJ7Wc3rORFniA1L9lWhuSd19iQSfLKHtdQkCAgCgoAgkB0EEkT07OqZy8uyU5vUIggIAoKAICAICAKCgCAgCHyBgBBR6QqCgCAgCAgCgoAgIAgIAvWCgBDReoFdKhUEBAFBQBAQBAQBQUAQECKawT5AMX3ZOHoog80ILZpuMKJMrptw7ALwTPcu5fqQdl/KBoZR5PHJG8fv9vmnqZ5f6WpXKvqK0qZ09RVX26PILHkFAUFAEBAE6g+BrBNRHCUzZEBPdUqfbvpwdA5xSRUe12HaWNQnT38rK8fzpLsoh7UbC3afnp1Cb6dJFbcoz+EIoZOO66IfwYH3dDd6WBm4dhK74cMONKfnU8UwCj5R5ImCTSp56Q56HGpPt1hh4xb3vnWzTle7UtFXlHakqi/UEWfbo8hs5w07hJ9eKnHgvplWrd2iXnhjaUp68snKnTNxXSpuNcO8Sv3HPvTflwf946YxQ1T3zuXJG7EwZ467/9k6V6r65JbfBQFBQBDgIJB1IkoLFSa3Th3aqM1bd6hLbnqUI2vkPFhQcEUiXSlIRwKhoGvumKr+88HayGVGeSCdRdlXT9i97b5nM/W76y5xV10/vPSML+6L9y9uqWIYBZ8o8mQKOyqXDp+nW6vSqc/XLq6+osiQqr5QR5xtjyKzndc8Jgs3guFlefnqjTobXSMLIooXBMwtpc2aql5HHqKJG646pZu50pHBfJYzZ5Klm2Tqfli5vpXLvEaWkwc3T4HI0lWzmKNxS1qqVvm4MJByBAFBoHARyDoRxdv9xi3b9SSeiYXQVJWrfJqMzQk6U+pNZ1H2yRSFaPnKiuv3TOgzVQxzER8OzsCwNGEtHvmDiZzsaeXJJX2hIdlsOxc4EE5cUUsElJ5zXUFKt1xxLP3c+pGPM2dS3eatYGTZxXWr2qr+s4s1qQzLg7ratiqt5TEiclrIIUZR9CF5BQFBIF4Esk5EfUQxzuYFLbTmIkIuNkzUd058Xl0/+nRtEaC/6YYduLS+NayftnrA7YU7yX/x0Myku4pcWgP61hwo77on3SVPEFkePvDY5J3rZOXAYmieJenCirtYkBvUJuT291iYHrl9tK4K7V5WsT7wbM8wYmPHJqI8W1YOhj55uPhw5IGMPr0TeYJb9uKh/XTIASxoD02bk1L4RxiGvrZDXm67iPjh3yCPhOnGRT5Yxe6bMjvpSeDoK8p49hFjny64YzmKTFGIKOnHtopCJzdcNkiHWtiu8iiyhOkMYRirE+EBpi5h2b3yvFOSITCcPLY80PGfbh+llq/aGLulN2rbJb8gIAgUJgINnohikRjUv4cCgcQCgrSrcq8anPgOcV+wTGEBHJsgqEQIyV1lxreS9YHceHCNISwAEzkRLg4RJSJhxgj27tFRk+OJT72uXnv7Q9WtY/ukzI8/N18tr6hxGyJNf+Uddk+FpaN5SbGCxYTSL8eeq9tOrmG6HhK/I7SBYsdcVp8wImHGJhLeNhHlYOiTB79z8OHIk4regRH6EvUdjjJIZuRFnC3SpL/PTT46950K3fd8bccDnHZRwWH6stsO9/PJvbsmXkYqk/2Foy9f+7lt5+iCM5Z98ti/RyGieHbGA1fVCTciQhiHezvs5RrzF16OUd/CJWt0U269amjSoou2+PJQ+6GXrx5/uDrz5MRtbYkX0B/d/TeJEY3aSaoxvwAAHNZJREFUeSS/ICAIsBBocESUFjTTEkhWNNMFR4ssyBJ9drm08B1CDbAA2Ruv8F1UIkruMztGEIusSTLjcD0jhhBWPJNUQmYQ8SDXMNqDPK7QBp9Fi3qky91O5fowtHt1kDxR8Aly//v0jhhjVx6KdeRap31W3CBXb5gugJMvrCFMXxRfffktk5MEhMYO5AE55vR53yzEbTtHF2abg8ayT550iWgQpvA0LKvYkDaZCyOiaDMSxbU+MXNBHSLqy4PnTcs7vXxjsxw+SxIEBAFBIG4ECp6I2puVYK2CtdHcrOQjLa54MPMZ2/JASiLrYhSLqMt95lK6T2ZORyEiQ65EcsvDykpHKyHPqBEnqD49OiWLxCYI12kH6RBRsgTb8Xg2hlx5ouATRNh8eseLgfnCEka0OfpAHp9VmauLdImovSvclB9ElNvnue32tZ2jC7PNccVpRrWIwsvAOTkiCi5m3jAiipfDyc++pb7/7YE6jOKT9dvqEFFfHrMujDXswsfLqu32T1V+eU4QEAQEARuBgieiIE2UKMbRPr7JR1p8iyB3UXYtIvbCxSWiLmtmKt0bFtgeXTqogZdPUET6TMsv5Gvfpix5lBDqIIuLHVuYDSLKlScKPvlCRLlt5xLiMH1RP4RVzU6whuLILtPtS3nsF4cofTJMHt8YJG+BbyxHkQd5oxBRerGLwwUfJGcQRmHxnxMee1lNmTFPcfK46nXNC1FxlPyCgCAgCAQhkPNENB2XVlRiFGRF8bkFAS429NgLkMs1b+6Idrmjg1zztgLJgoi40VTOmKTyKH4NixU2Y61Zv1V997bH9c9UBy1k+C7IhY7fXNZBV8dzET9yB4ZhGEWeKPjE4Zo33fA+l3jYdBTUZ6O0PQoRxYuaK4SAXlDG3T/duemKo6+o064vVACyBoXH0FFs9UlEKSTDNSbTmcdMHIMw4uyI5+Rx6cz1XFTdSn5BQBAQBHKGiJpn9PXv1UXLRed8PvXiwlpne6Yb5O8jokEbW2hzCIHG2ShBJIzO33NtVqKFysyDOhA+MGvuMu0Ox4I1/roRevc1XOabt+3QYmATjHk+IQjhX++5Qv9GB5/j8+y33o+8WxuWkk1btyc3RBGxJbIBNzwsY63KmmlXHRI2OZHMhBNZTmjDFmR+8oUFWh60q03LUp2VNiuRi3fFx5u03n0YRpHHhw9Hnih6zzQR5bad0y6uvtAPkXBCBPVD/E1HGfn0FXXajbJ5yt4wyB3LHJminiOKMjGXoc+5rKHpzmOcOZNzRignD+YoJNI3bZTM5MUjHJ1IHkFAEChcBLJuEaXFywWpbZFM99gTHxEN2ijhsoz6jo7BIjT+uuHJI5dcxzeZeRAmcM8js9QFQ/rqg6eRiMyATIwacaJ2mYOQImGjgLm7Hd/Z1yHiu1QOQieCDJngojcTFkG44iE7ZEAMJ84ixH+mzPhsY4D8D059TW+yCtM7xYVyMIwiTxg+HHnQJp/esxkjymk7t10+feF3Vz80iRZHX1GmTt94DdNFlLHskykMw7CblV5fuMJ5bW0c85gZYmTKb85VvluTqD+H3b6El0mcjmDOOzAUyGYlX6+R3wUBQSBVBLJORFMVVJ4TBAQBQUAQEAQEAUFAECgsBISIFpY+pTWCgCAgCAgCgoAgIAjkDQJCRPNGVSKoICAICAKCgCAgCAgChYWAENHC0qe0RhAQBAQBQUAQEAQEgbxBQIho3qhKBBUEBAFBQBAQBAQBQaCwEBAiWlj6lNYIAoKAICAICAKCgCCQNwgIEc0bVYmggoAg0NARwNFSONOXbpJq6HhI+wUBQSD/ERAimkEdpnPDTgbFymjRODOxe5dy53mKGa1YCtcIcG+3ErhqzuHNt74qRFR6riAgCBQaAlknojgE+6YxQ1SfozrpQ5NxiDpuBvrNoy/pA9PjTK6DqXEgt33XfJx1mmVlkohiEe3Ts5Oyb6PKVFu45eKWJug16LpUbjkNMV8cOm3oRDQKhvnUVzGX0O1NGBuYx56d/W4kyyjNvd07l+vLJ6iccfc/W2vuddVlz5mcPJwxzCmHk4dTl+QRBASB3EQgq0SUrl00r6/EVZi4NQQ3Ef10wtOxooRFGddn0hWiuHISBBjpmjum1rpONNaKvygsk0Q07ju142r/Dy89Q98IZS9ucZVfyOXEodOGTkSjYJgvfZWuCF21dou+WnfN+q16jCFdeMPD7Bf4ab+5Ul/jS1cM0zWp5m1ZhJ95Te+Avt20wYDq4uThjFNOOZw8nLokjyAgCOQuAlklooABk+p7iXvFcf84pRkPXKU/2ldYpgub68pAum85E8TXlrchEtF0ddaQn49CooJwEiI6VF9JW0gW+T/87GJ9pS7mx7t/9A0dIzr3nQpV3rYs0sv0MUccrNq2Kq019xI5patLYSXetHW7GvmDickuBsJ+8dB+asJjL6spM+YpTh7OOOaUw8nDqUvyCAKCQO4iUDTkrLOrn/+grF4lzNTiGXR39by/3Kjwxn/JTY8qfEZCWMCdE59X148+XVsN6G8izL47x8ntBesBkuuueZc8QWR5+MBjk/e5QxZYdcc/+JwKulObFEgLCkehqLs04UZ/4Y2leqGBpRpWl7+9uEgvOEgcfIjcm3W65EC+oHbRs+Z92fgO1pr7psyOtODiOV9dWJQfuX20rhbWnmUV6+u4Ol34oN88NG1OcjHn5EEdqdyTnqpO4WGY+NTrtXQ66e9za7lx48A5StvpfnOXTu2xM2fBCrV52w5NKKkf+fTFHRe+vnrq8Yere288v46HxvU9F8N075qf8D8jFeaVH/7qSTVm5IDYNisB9z/dPkotX7VRjb1rWnK8PzP7HT3XUCLM6HvMCb48nPmHUw4nD6cuySMICAK5i0BOEFFYRDdv3aGJYZyJQ0QxyQ7q30NP9FgAkXZV7lWDE9+BlMEygAVnbIKgEiEklxaRWTxDZJpcWgg5QFgAJntaTDlElCZ9ip3duXuP6t2joybHIBevJSzJ3Tq2T8r8+HPz1fKKjUnYouymJZnRrsXvf6LLOLl3V/0vhS5w8EEbTzqui36OsLSJqK9df/jra3VwLm3WVMuzY1dlJGs5py4Qm/MG99EyI2SD4uZMS5qtU+RDP6F+4dK7K4+v/0CWuHUKGUEwSKd4ybjslsc0obflSRVnDj6cujhjx6cvLoacvgorIVzgpofml2PP1XMCyCBeTjntokFJrnXTBR5lnjP7M57DixC9KEYph/ICq68mCPeZJx+ldiZewn5099+S7n3zJZ3yE8k3iag59yGfnYcjVzbr4sgjeQQBQaB+EKh3IkqTNEgWyEicyUX8aAExXfOuSdS00tJnWshNAoLvNm7ZrkCm7ckZ30UlouSGowWP8MBiZJLMON24JvkifYDg3jvpJV29Dx9TZ0HhCJx2UUzv5bdMTi6MpK8orlZOXXY/g56gL7NfuPRO1inz5QIWSLNvcPK4yo5Tp6Y8tkUrLpy5+OCFLEincDFzxg5HX2Zf5fSXoL5KrmizDMiIF1RyWUfFEBbVZRUb2PGcdnvxPKyh6GtI6Ke0wdNl5TWfN62XpmWZXqwx79JGUdfLRdvWpbpemhM4eTjzOKccTh5OXZJHEBAEcheBeiWimFzHXzeiTkxSXHC5NitR4L25WclHAMLe3LFYId161dA67iqyokSxiCImanXCmuWzDvtk5mAYFBJhu8Oi1BW0uHPaRWEALtk5xIKe49QF4jlqxAmqT4+azWtIWGxdVm7Tumu3z4WhncfXf+gFIwrOQfrl6DQunLltD5I1ytjh6CsuIkovJfCQwGVNbnnz5SwuDDnj1MxDcxpkJMu8aS12lbdwyZo6u+vxPMIlEJJjzjf4fvx1w5NhQSC8Gz/9XOejMcjJw2kXpxxOHk5dkkcQEARyF4F6I6ImCb16/NSULQVh0NJCSXkoFtB1FEnYBgcfkUiHiMINCPcYEU8OiUJ9LqtN1G7GIS1xLe6cdlEebMawEyxn3OO9OHUB9/ZtyvTRYQh/QEIfqE8imimdEpEiy1hcOHOIqK8uhHRwXuI4+oo6LsI2E8Kqjp3pAy+foOiF0rQy+9rF7aupjFmMj1MSISJmqEDUcii/q212WbAGt2heorEISpw8HBk55XDycOqSPIKAIJAbCGSdiOINF+5fWvTN+CQXJOm4tIJiRO16fJYon2se5WHjix0D5nLNY3MQuffI8mKSnyC3si0zuePSCWmgdpnWRnKFm+48Hz6mbFFd8+aztPiPu396rZ29UYeKD0PCjnYBo3yXLjhEK0qeoNAOxG0ixalT04pr4xEXzpy2++oiV3HY2OHqKyqGYUSUQlTQR741rJ8+Mum7tz2e7Iq+dtl9Np15DH3TdJ2bRDSKp8A1jqhvmH2T8pnWSNMabJbDycNpO6ccTp6oc4XkFwQEgfpHIOtElCY+WCdnzV1WCwHbhZRukL+PiAZtcLCtb77NJmgELcp0Rp9rsxLFDpp58Cxi6IAFYjLJUmyetarzJDbK0M5WIk5/vecKjZ9p1Zv91vtsEkcyB21sQdmuTTQ2PpC5TctSLQttViK364qPN+kNMpx2UR6Us3DpGr1zmpK5i9c3bHx1EfnBCwAW9VZlzbSbEgmbVEgXHKLFycPpP3HrFC8SSHQIuhn7GhfOnLZz6vKNHa6+OBhy+ir1Lzo6iDYKmjHsnHZROenOYxSfis1nvY48RG/sRBgJ5tAwK6U9TjD/ING4ok2Q5oswufnpfGfkt4+64+ThtJ1TDiePbz6Q3wUBQSC3Ecg6EbXd5SY8riNBbrhskCZaqRx27yOiQUe+uKwMnOOb7NgquHzNI2jMN3osIvc8MktdMKRvcgMCWbGwyI0acaJ2DYKQIsEiYp+zCkvRpV8/Se+op2RvcgrrfqQLWFUhJ1le/jxjvt6Vy8WHq1NOu1x5Utlt7KuLrPLUZvQ9nNWI/5CgCw7R4uRBeb7+Q3pKV6f0skPlgWTMnLOkzi7rOHDmtt1XlysO0B47HH1xMOT2VZRFWAYRPl+7THnSmcfgPsepDjTOg0KMfEsNysEpFOacgmPhzM1KRPrp5XTys2/VOTqNk4fTdk45nDy+dsvvgoAgkNsIZJ2I5jYcDUu6oBjRhoWCtDYXESDPSZRzcXOxHXHLhDELK36UY9rilkHKEwQEAUEgTgSEiMaJZp6VJUQ0zxTWQMSlq4DtG34aSPNDmylEVHqBICAIFBoCQkQLTaMR2iNENAJYkjVjCMDtDvKJhFjoPkd10u5jczNZxiqXggUBQUAQEATqFQEhovUKf/1WLkS0fvGX2msQMGOREQuNHerPzn5X3M/SQQQBQUAQaAAICBFtAEqWJgoCgoAgIAjkBgL7njtfNR76ZG4II1IIAjmAgBDRHFCCiCAICAKCgCDQMBAQItow9Cyt5CNQsESUzp+Lcq4mH7b6yyntqj/speYvESjUfig6FgQyjYAQ0UwjLOXnGwIFS0Rp5+2yivW1bkTJNwXZ8nLblWvxnz55uO3KJf3RjT8kk3kweC7JmQlZ8lFfmcAhaplhtzmhP3XvUq4vtmgIyTcnFCoGQkQLVbPSrlQRYBNR10HQOGjcdW873eYCoZAnlY0HsLiMOX9AcgctlWXXF9ZwmvRd19fRc6575IlgpHt9XqpK8T3HaVeuTfIceTjtgr7MhIO3X3hjqT6UO9sJZAx3pSPhYoGdiUsKLrnp0WyLkbH6MA769OyknnpxYZ1DzVEpR18ZEy6gYJ/M2ZbHri+MiOImJ5wWkKvzTtzYceaEOOvMlb4hRDROrUpZhYBAJCKKqyhxEwcSHbOCz9fcMVUvVHSVHcjBomUf18pz4Q0PJ+9L9gEHEvq7my/SkzJdh4n6BvTtVue6ubCy6FaOOQtW1Loe03wmH4kop13ZnuR9OuXIw2kX9IWd1eiHpc2a6isPQQjDdOyTLY7ffbd4xVFHtssg0hREjDj6yjWZsy1PFCL6w0vP0Lepjbv/WfZcWd/tSad+zpyQTvlB2KdD9NHnsdalk4SIpoOePFuICEQiogDAtPiQ5ZDuIqbbUHAVJcgCEkhCeduySIPXvHrStHSlMgngmj6cSxhEhPORiAJXX7uyPcn7BgdXHl+7XPqifpfOAuOT3/d7QySinH7owy3u333kOe76opYXZhGNWla+5+fOCXG1M46+AZlLEwYSeGH+9uKilF4YhIjGpVEpp1AQSIuIAgSTGNDdzFHuO7eBjGplARkOu8eZyrPvsad6uUQUk1jvHh1r3etu3/+O+8S/Mbh3Mg+sufdNmZ0k4eRWxnN3TnxeXT/6dJ2X/n717Q+1WJy6fO0yyfzFQ/tp6zIs1ZP+Pjd5PqOLPNnfcWQmWSA73YNth2O4Fh1YgCCbSSB97XLpy9VnwnSBO8LvvfH8OtZ11/c+nVI/8hFR313z5gJn6guL3ZQZ8xS99MHyixcrpMefm6/xQyKvBD77ZHbVhfjWh6bNUeiD5rmeronOvHaToy+UEdbnOf3QNy6iyMyZvIH38IHHquN7HqqzkxV+/IPPJR/36RQv4TeNGaI9OUh4Yd+5e486Z+BxijC044yRz77W1KcvPGPXhX6yeduOWnX52k3jEfN3m5al6tarhiqaN2c8cJXavHVH0hDhaztn3uDOCb66fP0nSt/wzb3AaNBJPTTemOsWLl2jnnxhgR433CRElIuU5GsoCMRKRGlSxQCdNXdZYGxZGLhUBhZZTtA+hQOA9H33tsedRWOiQlgBLLV24hBRmqAp5IDKwOJEFltMlmMTxNJ0G5/cu2tisqpM1ou2DerfQy9MWCiQdlXuVYMT36HskT+YqDh1Uf1h7aJJHuUuX7VRPwJ5QEgpZtY3geMZjsy0gxr5EULRvXO5nqhNguladFwWdJQRVV94xlwoObqY9psrVfOS4lp94pdjz9W6oBcpTjmmLvDZFSNql9OpQxtNcMzNTYQP+s/i9z9J6gsfQDK7dWyviQGemTlnie5rFLYCYkOEgSMz1YWylq/eqHWGPkl9EPpEfdRXMRaXV9T0IST7nvMwfXH6D6cf+sZFVJmdE8UXX5rz2BuLVmrySC+hE596XY/5KDolnLsfVq7nIYwNIptmnDHh7SKiPbsdpHXv0heNGTOPq66wNtNYRx/DuD2kQ2v9koP2/3TC09rgQOEvnLZz9Q6ZzfbacwKnLl//4fYNXx8z8aP20S1gGDv00ujDWYioDyH5vaEhkBYRpUmCXPMAD1alMSMHKEwwSPjtN4++xHZhpLJRCHUuq9gQWAeVSYuIqWQOEaXJMczSS4vx5bdMTspB+JiEjN7OTQutSdI4dZH8Ye2iMs2NWpSf6vZN4FSPT2Z70GBxBTE0+4VNRMn66HqBiKovWoiJCHJ04bLGQma8GOCFgMoEcfDp1K7fxsOlC/s7+tvsK/SSRUTQtFCh30KPC5esqWW54rTdJQ95M0xSwHVlhukLWPj6D6cfcsdFmMwu66OpKxoXQXXheSLhPp1u3LJdjwH7JAV8ZxJRs/4gt71PXzTewuritB1H3cFTgKtVux3aXoHM0pii/gaLsK/tFEPJ0btJRF1zAqcuTv8x+2FQCA+3j5k6A8m97Zph2rPFPTVDiGhDo1nSXh8CRZcMGVr92Ic19zyHJVrgzM1KsKLA+mm6BU2SdMGQvpqQkqXFVwd+t8kS5xlOHiwASLZVlENEQShBrsm9DQvju8s/0S5TSvZublMmFxENmgw5dZllB7XLZYHEc/aCQgsNlema1H2EBAvhqBEnqD49alzGSGSdIQshyYPFnixw6Dvj7p/udGtF0Rfqg4WTdq1zdEGLN1l5aBE0LfGccsJwM/uGvUjZmPr0ZRPOICLKkdlVl4sE+fTO6YfI4yuHQyS44yKsLtNy75ozgDGIJnavr05YuMJOQHDNG2bdKN98caD6yOpuWz1NnIIsoq6XBHwXNGeadXHbTv0KVvt/zf9AfWtYP/Xg1Nd0W2jO8rWdyDpH75gnwuYETl2c/sPph9w+hrkDIVhnnnyUJqBRXfRCRDkrtuRpSAhEIqJk5QRAFAvoO07Jdnf6wKWYszBXu68M1+9kXbIJIIeIojyafHp2PSjpejYJNi1eT8xcUKf6ue9UJK2kvsmZU5dZQVC7XGSDyFaYRdQkdFSPT2Y8075NmXbjwY2JBHexy/Vsyh5mQYiiLyKV1Ge4uoAFBLuUB14+QVE/NS3I3HLQJtdiWB9ElCMzl4i6rMZBYy9IXxwC4MLO1Q99YxB1RZE5qC2FSES5cyYRUVhD7/rjC/r0knsemaWJKHmEOOSQq3dzTcEz9pzAqYvbfzh9w9fHaJ6ArAijwVwaddOSEFFub5R8DQWBSEQUoIRZCTCIabc8AUgDN8qOZlooXa50l2J8rnl6xrXAkKXXtJQS8QpzxdsEm0hNkIWPS+pc7fOR+aB2BcVfUbvQduwAJVd0kIuP4+6EO48sxK5ybPJDbjDzObvtrna5FiZyK1N/4eqCyBNkgOVnzfqtteKMueUQEbXxpvZw3Yt43hwnFNrhcsEHWUQ5MnOJqM/lztEXl5Bw+qFdn2tcRJXZNd44LlqfTlHuI7eP1nG8Zux6Oq75IIto0Mt7WF1BCxzatWXbTh0zjPrw98cbtunYaarf13bbNR8093PmBE5d3Hkslb5h9zHCZ1Yi5MyOleaSBiGiXKQkX0NBoOiSryVc8yt4rnkfEaUYO7iuYRkzXbCwOnETiOX460YkXeE4kxQJ7iLclGRuYuJsVrJJoGn1orfksI0AIGI4s5KsfeYZqtQukhl1YScldqxSQlxVUMC8aS2lRdtXl42j62Bx0xWO/HTJgBm3SQSONr1QTBjiIrHZDJtiXJtWTJlpEQR+sAa3Kmum3VZI2AyEcqAve9Ghm3mQL+hoLVe7zHNEzXaZC75PFyZ+ILubtm7XLjb7xSdKObRgmf2IdtNyN1xQGItrcxltViJrdhAR5cjMJaKmjkxrd9C1uba+uH3e1w/RfzhjEHqNKrNrTjLnH9p9jnwY92PvmqYfiaJTc3zZm5VQF3aoI9FmJRA3pBUfb9InbnD0RXnC6nK11f6OSB2+xwsqdNPpoDa1Nvb52s7VO2dO8NUFOTn9h9M3uH2Mg2NYHiGi6SIozxcaArESUSzG2DGNRR2J6753gUo3K9EObORx3aLjO77JLJsWKRBFWlDwOyYgImoU7/OLh2Ymrbs00VFZQe3CojJqxIna3Yt4UiQiSEFHiNjWAm5dvnbZ5dBuazOuFXiMv2643sGNNsEFR3G9KB+kBy52O9ky42UA+cgijudQJh19Q5YV22JILwEmOfa1y46BRJ94feGKOicshOnCrINwQvtdL0vcckwsUT48A4itI6sJ5wga4AMybGL55xnzax3f5COiqNsnM4fYEEYYX7g1isY0vg/yFNjji9vnff0Q/SfKuIgic9CE7sLQdVwbLOnmUT7mvGH3CdfxTaQLlxxmCI09duyYXk5dnMWLyrXnLdtlHtafuXp39UPXnOAbO5z+w+nPUfoYB8ugPEJE00FPni1EBNhEtFAaT5NN2LWf+dhWaVc+au1LmV2Lcj62qFD7YT7qgkIMXBuj8rE9hSKzENFC0aS0Iy4EEsc3Da1+/gO/az6uCuu7HNo9GuRerG/5Uq1f2pUqcrnxXKEQ0ULth7nRS/hSkHUaYScUA85/WnJmEgEhoplEV8rORwQaHBHNRyWJzIWPQKEQ0cLXVG62EOExIJ9IFMOO8KCwzYC52ZLCl0qIaOHrWFoYDQEhotHwktyCQEYQECKaEVgbTKFmXCZiWXEChH3NboMBI8cbKkQ0xxUk4mUdASGiWYdcKhQEBAFBQBAQBAQBQUAQAAJCRKUfCAKCgCAgCAgCgoAgIAjUCwL/H1Tz6WZq55bbAAAAAElFTkSuQmCC)

## FFI (Foreign Function Interface) Kullanımlarında Unsafe ile Güvenli Soyutlamalar Oluşturmak

Rust kodlarında diğer diller ile etkileşim kurmak için FFI (Foreign Function Interface) desteği vardır. Ancak FFI kullandığımızda bellek güvenliği garantileri devre dışı kalır. Bir başka deyişle unsafe kod blokları kullanmak durumunda kalırız. İdeal yaklaşımda harici FFI çağrılarının güvenli soyutlamalar ile sarılması benimsenir. Böylece dışarıya açık API'ler güvenli kalır ve bellek güvenliği sorunları minimize edilir (Safe Abstractions). Örneğin C dilindeki bazı fonksiyonları kullanarak rastgele sayılar üretmek istediğimiz düşünelim. Bu senaryoda genelde libc'den srand ve rand gibi C fonksiyonlarını kullanırız. Aşağıdaki örnek kod parçasında bu fonksiyonların güvenli bir soyutlama ile nasıl sarılabileceği ele alınmaktadır.

```rust
use std::os::raw::{c_int, c_uint};
use std::sync::Mutex;

/*
    C ile yazılmış rand/srand fonksiyonlarını Rust tarafında kullanmak için FFI(Foreign Function Interface) tanımı.
    libc kütüphanesinden srand fonksiyonu ile rastgele sayı üreteci başlatılır ve rand fonksiyonu ile rastgele sayı alınır.
*/
unsafe extern "C" {
    fn rand() -> c_int;
    fn srand(seed: c_uint);
}

// Thread-safety için global mutex oluşturmamızda fayda var
static RAND_MUTEX: Mutex<()> = Mutex::new(());
static mut INITIALIZED: bool = false;

fn main() {
    for i in 0..5 {
        let random_number = generate_random_number();
        println!("#{}: {}", i + 1, random_number);
    }
}
/*
    Güvenli Soyutlamayı yaptığımız metot.
    Öncelikle global mutext ile thread-safety sağlanıyor.
    Daha sonra unsafe blok içinde C'nin srand fonksiyonu ile rastgele sayı üreteci initialize ediliyor.
    Ardından rand fonksiyonu çağrılıyor ve dönen değer güvenli bir şekilde u32'ye çevriliyor.
    Negatif değerler 0'a mapleniyor, böylece u32 overflow riski minimize ediliyor.
    Tüm unsafe kod bu fonksiyon içinde kapsülleniyor, dışarıya güvenli bir API sunuluyor.
*/
fn generate_random_number() -> u32 {
    // Thread safety için lock alıyoruz
    let _guard = RAND_MUTEX.lock().unwrap();

    /*
        unsafe blok için iki C fonksiyonu çağrılıyor.
        ilki srand ile rastgele sayı üretecini initialize ediyor.
        Burada amaç tutarlı rastgele sayılar üretmek.
        İkinci fonksiyon rand ile de gerçekten bir rastgele sayı oluşturuluyor.
    */
    unsafe {
        if !INITIALIZED {
            let seed = std::time::SystemTime::now()
                .duration_since(std::time::UNIX_EPOCH)
                .unwrap()
                .as_secs() as c_uint;
            srand(seed);
            INITIALIZED = true;
        }
    }

    let result = unsafe { rand() };

    // max çağrısı ile negatif değerleri 0'a map ederek bir overflow oluşma riskini minimize ediyoruz
    result.max(0) as u32
}
```

Bu kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![rust_exc_23.png](/assets/images/2025/rust_exc_23.png)

## Eşzamanlılık (Concurrency) Garantisi için Send ve Sync Trait'lerini Kullanmak

Eş zamanlı (Concurrent) çalışan işler arasında tiplerin güvenli bir şekilde paylaşımı/kullanımı önemli bir konudur. Send ve Sync trait'leri eş zamanlılık için gerekli kısıtlamaları tiplere ekleme becerisine sahiptir. Bir tür Send trait'ini implemente ediyorsa, bu türün sahipliği bir iş parçacığından diğerine güvenli bir şekilde taşınabilir. Sync trait'ini implemente eden türler ise, birden fazla iş parçacığı tarafından güvenli bir şekilde erişilebilir hale gelir.

Rust'ın standart kütüphanesinde yer alan pek çok tür varsayılan olarak send ve sync trait'lerini uygularken bazıları uygulamaz. Örneğin smart pointer'lardan birisi olan Rc yapısı Send ve Sync trait'lerini implemente etmez. Dolayısıyla referans sayımı iş parçacıkları arasında güvenli bir şekilde paylaşılamaz. Zaten yapının kullanım amacında bu yoktur. Diğer yandan örneğin Cell ve RefCell türleri de Sync trait'ini implemente etmezler çünkü sadece dahili olarak değiştirilebilirlik (mutability) sağlarlar. Diğer yandan Mutex ve RwLock türleri Sync trait'ini uygularlar, zira değiştirilebilirliği güvenli bir şekilde yönetirler. Özellikle thread-safe olmayan varlıkları kullandığımız veri yapıları olduğunda bu trait'leri kullanmak önemlidir. Aşağıdaki tabloda Send ve Sync trait'lerinin uygulandığı türlerle ilgili bilgiler verilmiştir.

![rust_exc_24.png](/assets/images/2025/rust_exc_24.png)

Buna göre örnek olarak aşağıdaki kod parçasını ele alabiliriz. Senaryo ve işleyiş yorum satırlarında yer almaktadır.

```rust
/*
    Bir oyundaki oyuncu ve takım sayılarına ait istatistikleri tutan Stats isimli bir veri yapımız var.
    Bu veri yapısındaki player_count alanı Raw Pointer türündendir ve bu türler
    Rust'ta Send ve Sync trait'lerini otomatik olarak implemente etmezler.

    Dolayısıyla bu veri yapısını farklı thread'ler arasında paylaşmaya çalıştığımızda,
    "*mut 32 cannot be sent between threads safely" hatasını alırız. Bu hatayı çözmek için
    Stats yapısına manuel olarak Send trait'ini implemente etmemiz gerekir.

    Şu anki senaryoda sync trait'ine de ihtiyacımız yok gibi görünebilir zira derleme zamanında
    hata alınmaz. Yine de raw pointer'lar thread-safe olmadığından dolayı Sync trait'ini açıkça
    ekleyerek kodun güvenliğini artırabiliriz.
*/
use std::sync::{Arc, Mutex};
use std::thread;

fn main() {
    let game_stats = Arc::new(Mutex::new(Stats {
        player_count: Box::into_raw(Box::new(0)),
    }));

    let mut handlers = vec![];

    for _ in 0..10 {
        let stats_clone = Arc::clone(&game_stats);
        let handle = std::thread::spawn(move || {
            let mut stats = stats_clone.lock().unwrap();
            unsafe {
                *stats.player_count += 1;
            }
            thread::sleep(std::time::Duration::from_millis(10));
        });
        handlers.push(handle);
    }

    for handle in handlers {
        handle.join().unwrap();
    }

    println!("Player Count: {}", unsafe {
        *game_stats.lock().unwrap().player_count
    });
}

#[allow(dead_code)]
#[derive(Debug)]
struct Stats {
    player_count: *mut u32,
}

unsafe impl Send for Stats {}
unsafe impl Sync for Stats {}
```

Bu kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![rust_exc_25.png](/assets/images/2025/rust_exc_25.png)

## Eşzamanlı Garantilerde Mutex (Mutual Exclusion) Yerine Atomic Türleri Kullanmak

Rust'ta eşzamanlı (concurrent) programlama işlemlerinde paylaşılan veriyi yönetmek için genellikle Mutex enstrümanından yararlanılır. Ancak özellikle yüksek performans gerektiren senaryolarda Mutex yerine Atomic türleri tercih edilebilir. Atomic türleri kullanarak kilitlenme (locking) maliyetinden kaçılabilir ki bu da performansı artırır.

Örneğin mikroservis cennetine dönüşmüş bir eko sistemde servislere gelen istek sayılarını tuttuğumuzu ve belli bir eşik değerinin aşılması halinde yükün arttığını belirten alarm mekanizmalarını tetiklemek istediğimizi düşünelim. Gelen her istek için paylaşılan bir sayaç değeri üzerinden artış yapmamız gerekir. Mutex kullanabiliriz ama her artış işlemi için kilitleme ve açma maliyeti oluşur. Bunun yerine örneğin AtomicI32 türünü kullanarak Lock-free (kilitsiz) bir sayaç oluşturabiliriz. Aşağıdaki ilk kod parçasında klasik Mutex kullanımı ile bu senaryo ele alınmaktadır.

> Mutex senaryosunda bir thread öncelikle kilit ister, işletim sistemi buna göre thread takvimini planlar, eğer bir thread müşterek veri üzerinde çalışmak için onu kitlediyse diğer thread'ler uykuya geçer ve işletim sistemi planlamayı buna göre düzenler, müşterek veriyi çalışan thread işini tamamladığında kilit serbest kalır ve işletim sistemi planına göre uykudaki bir başka thread işine devam eder. Oysa ki Atomic tür kullanıldığında doğrudan CPU üzerindeki komut çalıştırılır. Elbette Mutex daha çok karmaşık veri türlerinin söz konusu olduğu senaryolarda çne çıkan bir kullanım şeklidir.

Senaryonun çok karmaşık olmaması için servis çağrılarının belli bir eşiği aşması halinde sadece bir uyarı mesajı bastırılıyor. Normal şartlarda servis çağrısını başka bir servise yönlendirme gibi işlemler de yapılabilir.

```rust
use std::sync::Mutex;
use std::thread;
use std::time::Duration;

static REQUEST_COUNTER: Mutex<i32> = Mutex::new(0);
const THRESHOLD_LEVEL: i32 = 100;

fn main() {
    let mut handlers = vec![];

    // Farklı servis isteklerini simüle eden thread'ler oluşturuyoruz
    // İlk thread ProductService isteklerini simüle ediyor
    let handle = thread::spawn(move || {
        for _ in 1..100 {
            handler(
                ServiceId {
                    name: "ProductService",
                    id: 1,
                },
                "api/product/10".to_string(),
            );
        }
    });
    handlers.push(handle);

    // İkinci thread CatalogService isteklerini simüle ediyor
    let handle = thread::spawn(move || {
        for _ in 1..100 {
            handler(
                ServiceId {
                    name: "CatalogService",
                    id: 2,
                },
                "api/catalog/computers/top/10".to_string(),
            );
        }
    });
    handlers.push(handle);

    // Tüm thread'lerin bitmesini bekliyoruz
    for handle in handlers {
        handle.join().unwrap();
    }
}

/*
    Sunucuya gelen servis isteklerini ele alan handler fonksiyon olarak düşünebiliriz.
    Parametrelerin senaryomuz gereği çok bir önemi yok.

*/
fn handler(service: ServiceId, body: String) {
    loop {
        /*
            Sonsuz döngüde iken sayacı hemen 1 artırıyoruz.
            Ardından sembolik olarak gelen isteği işliyoruz.
            Son olarak sayaç eşiği aşıldıysa alarm fonksiyonunu çağırıyoruz.
        */

        // REQUEST_COUNTER değişkenini kullanabilmek için öncelikle kilidini açıyoruz
        let mut counter = REQUEST_COUNTER.lock().unwrap();
        // * operatörü ile Mutex içindeki gerçek değere erişiyoruz
        *counter += 1;

        _ = read_request(&body);

        // Sayaç eşiğini aşıp aşmadığını kontrol ediyoruz
        if *counter > THRESHOLD_LEVEL {
            alert(service);
        }
    }
}

// Simülasyon amaçlı çalışan ve gelen isteği güya işleyen bir fonksiyon
fn read_request(body: &str) -> Result<(), ()> {
    // Sanki gerçekten bir iş yapılıyormuş gibi talep okuma işini belirli bir süre uyutuyoruz
    println!("Processing request body: {}", body);
    thread::sleep(Duration::from_millis(100));
    Ok(())
}

/*
    Uyarı mesajı veren fonksiyon.
    Örneğin basit olması açısından sadece mesaj veriyoruz.
    Aslında buradan dönecek değere göre ana süreç servis çağrılarını başka bir servise yönlendirebilir.
*/
fn alert(service: ServiceId) {
    println!("Alert for {:?}", service);
}

// Sadece servis ile ilgili bilgi taşımak için kullandığımız bir veri yapısı
// Sembolik olarak servisin adını ve sayısal değerini taşıyor
#[derive(Debug, Copy, Clone)]
struct ServiceId<'a> {
    id: u32,
    name: &'a str,
}
```

Bu kodun çalışma zamanı çıktısı aşağıdaki gibi olur.

![rust_exc_26.png](/assets/images/2025/rust_exc_26.png)

Şimdi aynı senaryoyu AtomicI32 türünü kullanarak yazalım.

```rust
use std::sync::atomic::{AtomicI32, Ordering};
use std::thread;
use std::time::Duration;

static REQUEST_COUNTER: AtomicI32 = AtomicI32::new(0);
const THRESHOLD_LEVEL: i32 = 100;

fn main() {
    let mut handlers = vec![];

    let handle = thread::spawn(move || {
        for _ in 1..100 {
            handler(
                ServiceId {
                    name: "ProductService",
                    id: 1,
                },
                "api/product/10".to_string(),
            );
        }
    });
    handlers.push(handle);

    let handle = thread::spawn(move || {
        for _ in 1..100 {
            handler(
                ServiceId {
                    name: "CatalogService",
                    id: 2,
                },
                "api/catalog/computers/top/10".to_string(),
            );
        }
    });
    handlers.push(handle);

    for handle in handlers {
        handle.join().unwrap();
    }
}

fn handler(service: ServiceId, body: String) {
    loop {
        /*
            Sonsuz döngüde iken sayacı hemen 1 artırıyoruz.
            Ardından sembolik olarak gelen isteği işliyoruz.
            Son olarak sayaç eşiği aşıldıysa alarm fonksiyonunu çağırıyoruz.

            Bunu iki farklı şekilde yapmaktayız.
            Normalde Mutex kullanarak yaptığımız işlemin çalışma zamanında kilit açma ve kapama
            maliyeti olduğunu düşünürsek bunu Atomic değişkenler ile yapmanın daha performanslı
            olacağını iddia edebiliriz.
        */

        REQUEST_COUNTER
            .fetch_update(Ordering::Relaxed, Ordering::Relaxed, |count| {
                Some(count + 1)
            })
            .ok();

        _ = read_request(&body);

        if REQUEST_COUNTER.load(Ordering::Relaxed) > THRESHOLD_LEVEL {
            alert(service);
        }
    }
}

fn read_request(body: &str) -> Result<(), ()> {
    println!("Processing request body: {}", body);
    thread::sleep(Duration::from_millis(100));
    Ok(())
}

fn alert(service: ServiceId) {
    println!("Alert for {:?}", service);
}

#[derive(Debug, Copy, Clone)]
struct ServiceId<'a> {
    id: u32,
    name: &'a str,
}
```

Bu kodun çalışma zamanı çıktısı da aşağıdaki gibi olacaktır.

![rust_exc_27.png](/assets/images/2025/rust_exc_27.png)

Bu yeni kullanımda görüldüğü üzere herhangi bir kilit açma veya kapatma işlemi kullanılmıyor. Bunun yerine fetchupdate metodu ile atomik bir şekilde sayaç değerini artırıyoruz. Ayrıca sayaç değerini okumak için load metodunu kullanıyoruz. Atomik operasyonlarda işin sırrı Rust'ın doğrudan CPU komutlarını (Instructions) kullanmasıdır. Bu komutlar CPU seviyesinde kesintilerin sorunsuz şekilde garanti edilmesini sağlar. Bir nevi CPU komutlarının garantisini kullandığımızı ifade edebiliriz zira donanım seviyesinde gerçekleştirilen işlemler söz konusudur. Örneğin x86 tabanlı işlemci setlerinde LOCK XADD komutu kullanılarak kesintisiz sayaç artımı yapılabilir. Burada tüm çekirdekler değişikliği anında görür, hiçbir işletim sistemi mekanizması devreye girmez (thread scheduling gibi)

Lakin burada özellikle dikkat edilmesi gereken nokta Ordering parametresine verilen değerle ilgilidir. Bu parametreler, atomik işlemlerin bellek sıralaması üzerindeki etkilerini belirler. Yukarıdaki örnekte Relaxed sıralama kullanılmıştır ki bu en gevşek sıralamadır ve en yüksek performansı sağlar. Ancak bazı senaryolarda daha güçlü sıralama garantilerine ihtiyaç duyulabilir. Bu durumda Acquire, Release veya SeqCst gibi farklı sıralama türleri tercih edilmelidir. Karar vermek zor olabilir bu yüzden aşağıdaki tabloyu incelemek faydalı var.

![rust_exc_28.png](/assets/images/2025/rust_exc_28.png)

Diğer sıralama türleri Release ve Acquire için [resmi dokümantasyona](https://doc.rust-lang.org/std/sync/atomic/enum.Ordering.html) bakılabilir.

Bunlardan hangisinin kullanılacağına karar vermek içinse aşağıdaki tablodan yararlanılabilir.

![rust_exc_29.png](/assets/images/2025/rust_exc_29.png)

Bu iki yaklaşım arasındaki süre farkı hakkında fikir vermek için aşağıdaki örnek kod parçasını da ele alabiliriz. Senaryoda 10 farklı thread tarafından ortak bir sayaç değerinin artırılması söz konusu. Mutex ve AtomicI32 kullanımı arasındaki süre farkını ölçüyoruz.

```rust
use std::sync::atomic::{AtomicU32, Ordering};
use std::sync::Mutex;
use std::thread;
use std::time::Instant;

static COUNTER: Mutex<u32> = Mutex::new(0);
static ATOMIC_COUNTER: AtomicU32 = AtomicU32::new(0);
const NUMBER_OF_ITERATIONS: u32 = 10_000_000;

fn main() {
    println!("Calculating with Mutex:");
    calculate_with_mutex();

    println!("\nCalculating with Atomic:");
    calculate_with_atomic();
}

fn calculate_with_mutex() {
    let mut threads = vec![];
    let time_start = Instant::now();

    for _ in 0..10 {
        let t = thread::spawn(|| {
            for _ in 0..NUMBER_OF_ITERATIONS {
                let mut num = COUNTER.lock().unwrap();
                *num += 1;
            }
        });
        threads.push(t);
    }

    for t in threads {
        t.join().unwrap();
    }

    let duration = time_start.elapsed();
    println!("Final counter value: {}", *COUNTER.lock().unwrap());
    println!("Time elapsed: {:?}", duration);
}

fn calculate_with_atomic() {
    let mut threads = vec![];
    let time_start = Instant::now();

    for _ in 0..10 {
        let t = thread::spawn(|| {
            for _ in 0..NUMBER_OF_ITERATIONS {
                ATOMIC_COUNTER.fetch_add(1, Ordering::SeqCst);
            }
        });
        threads.push(t);
    }

    for t in threads {
        t.join().unwrap();
    }

    let duration = time_start.elapsed();
    println!(
        "Final atomic counter value: {}",
        ATOMIC_COUNTER.load(Ordering::SeqCst)
    );
    println!("Time elapsed: {:?}", duration);
}
```

İşte sonuçlar...

![mutex_vs_atomic.png](/assets/images/2025/mutex_vs_atomic.png)

Böylece geldik bir makalemizin daha sonuna.

[Bu bölümde yer alan kod parçalarına github reposu üzerinden de erişebilirsiniz](https://github.com/buraksenyurt/friday-night-programmer/tree/main/src/rust-exercises). Ayrıca [Ferris logosunu sevdiyseniz Maria Letta'nın reposunda daha fazlasını da bulabilirsiniz](https://github.com/MariaLetta/free-ferris-pack);)

Kaynaklar:

- [Rust for the Polyglot Programmer](https://www.chiark.greenend.org.uk/~ianmdlvl/rust-polyglot/intro.html)
- [Nine Rust Pitfalls,Daniel Hayes](https://leapcell.io/blog/nine-rust-pitfalls)
- [The Rust Programming Language, 2nd Edition](https://nostarch.com/rust-programming-language-2nd-edition)
- [Welcome to Comprehensive Rust](https://google.github.io/comprehensive-rust/comprehensive-rust.pdf)
- [Design Patterns in Rust](https://refactoring.guru/design-patterns/rust)
- [Rust by Example](https://doc.rust-lang.org/stable/rust-by-example/)
- [Too Many Lists](https://rust-unofficial.github.io/too-many-lists/index.html)
- [Rust Cookbook](https://rust-lang-nursery.github.io/rust-cookbook/)

Bir başka yazıda görüşünceye dek hepinize mutlu günler dilerim.
