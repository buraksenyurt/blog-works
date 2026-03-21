---
layout: post
title: "Rust Pratikleri - Channels"
date: 2022-02-20 09:00:00 +0300
categories:
  - rust
tags:
  - rust
  - rust-lang
  - channel
  - crossbeam
  - thread
  - multi-thread
  - unbounded
---
Thread'ler aralarında haberleşmek için kanallardan (channels) yararlanır. Rust dilinde bu amaçla built-in modüllerinden olan mpsc (multi-producer single-consumer) paketi kullanılır. Bu paket aslında FIFO (First-In First-Out) ilkesine göre çalışan tipik bir kuyruk yapısıdır. Kanallar yardımıyla örneğin iki thread arasında bir yol açıp tek yönlü olarak mesaj göndermek mümkündür. Böylece bir thread'den diğerine çeşitli verileri aktarabiliriz. Hatta asenkron ve olay güdümlü (event-driven) haberleşmeler dahi tesis edebiliriz. Bir veri türünün kanalda akması için Send trait'ini uyarlamış olması gerekir. Primitive tiplerin hepsi bu davranışa sahiptir.

![peasant.jpg](/assets/images/2022/peasant.jpg)

Kanallarla ilgili olarak dikkate değer bir diğer konu da türleridir. Bounded ve Unbounded olmak üzere iki seçenek vardır. Bounded kanallarda kapasite bellidir. Bir başka deyişle bir thread'den diğerine veri taşımak için kullanılan kanalın kapasitesi sınırlandırılır. Eğer kanal kapasitesi dolarsa, yayın yapan (mesaj gönderen) thread doğal olarak bloklanır. Unbounded kanallarda ise bir kapasite sınırı yoktur. Bellek yetersiz olana ya da sistem bir şekilde çökene kadar kanal kullanılabilir.

Kanalların bu iki türünün farklı sayıda alıcı ve yayıncıları olabilir. Örneğin tek bir yayıncının birden çok alıcısı olabilir ve kanala atılan veri bunlardan herhangi birisi tarafından işlenir. Bu senaryoda kanaldaki veriyi hangi thread'in alacağı ise belirsizdir. Çok doğal olarak birden çok yayıncı thread'de olabilir. Bu sefer hangi mesajın kanala ilk sırada gireceği konusu ortaya çıkar ki kural basittir; ilk bırakılan ilk gider. Esasında kanalların her iki yönünde de birden çok taraf olabilir. Buna göre aynı t anında çalışan birden çok yayıncı ve alıcı thread mümkündür.

Tabii bahsettiğimiz bu senaryolarda kanallar hep tek yönlüdür. Yani yayıncı taraftan alıcı tarafa doğru kurulan bir iletişim hattı söz konusudur. Aksi mümkün müdür? Elbette... Pekala ayrı kanallar açarak thread'ler arasındaki iletişimi çift yönlü kanallar üzerinden de sağlayabiliriz. Ancak bu kullanımda deadlock oluşturma ihtimalimiz vardır. Örneğin bounded kanallar kurduğumuz ve verinin döngüsel bir akış içerisinde yer aldığı bir senaryoda tarafların birbirini beklemesi söz konusu olabilir ki bu da deadlock durumunun oluşmasına davetiye çıkarır.

Bu teorik bilgiler bazen can sıkıcı olabiliyor. İyisi mi Rust Pratikleri'nin bu bölümüne kolay bir örnekle devam edelim. Amacımız Rust'ın kanal kullanımlarında öne çıkan kütüphanelerinden crossbeam sandığını kullanarak basit bir senaryoyu işletmek. Örnekte bir simülasyon oyunundaki işçilere çeşitli görevler atayacağız ve thread'ler arasındaki iletişim için bir kanal kurgusunu nasıl tesis edebileceğimize bakacağız. Fakat başlamadan önce Multi-Producer Single-Consumer paketi ile kanal kullanımını kısaca hatırlayalım. Bu amaçla channels isimli bir program oluşturalım.

```bash
cargo new channels
```

channels isimli projemizin kodlarını aşağıdaki gibi yazarak devam edebiliriz.

```rust
use std::sync::mpsc;
use std::thread;
use std::thread::sleep;
use std::time::{Duration, SystemTime};

fn main() {
    /*
       Olabildiğince basit bir örnek.
       Main thread içinden iki thread başlatalım.
       Bu thread'ler bir transmitter kullanarak kanala çeşitli bilgiler bıraksınlar.
       Ana thread'de bir dinleyici olarak bu kanala gelen mesajları alsın.
    */

    let chrono = SystemTime::now();

    // channel fonksiyonu bir transmitter ve birde consumer kanalı oluşturur
    let (cmd_transmitter, cmd_receiver) = mpsc::channel();
    // İKinci bir transmitter nesnesini birincisinden klonlarız.
    // Böylece ikinci thread aynı kanala mesaj bırakabilir.
    let cmd_transmitter2 = cmd_transmitter.clone();

    // İki thread açacağız. Bu thread'ler sonlanmadan main bitsin istemeyiz.
    let mut handlers = vec![];

    // bir thread açıyoruz ve cmd_transmitter ile işlem sonunda kanala mesaj bırakıyoruz.
    handlers.push(thread::spawn(move || {
        println!("#{:?} Yolcu#23 sefere başlıyor.", thread::current().id());
        sleep(Duration::from_secs(3));
        cmd_transmitter.send("Yolcu#23 hedefte.").unwrap();
    }));

    // Burada ikinci bir thread söz konusu ve bu thread işini bitirdiğinde ilk transmitter
    // clone'u üstünden yine kanala bir mesaj bırakıyor.
    handlers.push(thread::spawn(move || {
        println!(
            "#{:?} Kaşif#24 warp hızlanma motoru aktif.",
            thread::current().id()
        );
        sleep(Duration::from_secs(5));
        cmd_transmitter2.send("Kaşif#24 öte evrene ulaştı").unwrap();
    }));

    // Başlatılan thread'ler bittikçe kanala bıraktıkları mesajları okuyoruz.
    for h in handlers {
        let _ = h.join();
        let msg = cmd_receiver.recv().unwrap();
        println!("İşlem bilgisi : {}", msg);
    }

    println!(
        "İşlemler {} saniyede tamamlandı",
        chrono.elapsed().unwrap().as_secs_f32()
    );
}
```

Main bilindiği üzere ana thread olarak çalışır. Örnekte iki farklı thread açılır. Açılan thread'ler içerisinde sembolik olarak uzun süren işler düşünülmüştür. İşler tamamlandığında her bir thread kendi transmitter nesnesini kullanarak aynı kanala birer mesaj bırakır. Ana uygulama thread'i receiver nesnesini kullanarak bu kanala akan mesajları yakalar. Esas itibariyle bir thread'den değer döndürebildiğimiz için aynı işi kanallara başvurmadan da yapabiliriz. Ancak transmitter'ları thread içerisinde çeşitli noktalarda kullanıp duruma göre farklı anlarda kanala mesaj bırakmak gibi bir durum da söz konusu olabilir. Bu tip mesaj akışlarının yer aldığı senaryolarda kanal kullanımı oldukça idealdir. Yukarıdaki örneğin çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![channels_1.png](/assets/images/2022/channels_1.png)

Şimdi gelelim crossbeam paketini kullandığımız örneğe. Bu sefer senaryoyu biraz daha eğlenceli hale getirmeye çalışacağız. İşe projemizi oluşturarak başlayalım.

```csharp
cargo new vorcraft
cd vorcraft
touch src/lib.rs
```

[Crossbeam](https://crates.io/crates/crossbeam) harici bir paket olduğundan projenin toml dosyasında gerekli düzenlemeleri yapmalıyız. Örneğimizde kod takibi açısından log mekanizmasından da yararlanıyoruz. Bu nedenle toml dosyasını aşağıdaki gibi düzenleyebiliriz.

```text
[package]
name = "vorcraft"
version = "0.1.0"
edition = "2021"

[dependencies]
crossbeam = "0.8.1"
log="0.4.14"
env_logger = "0.9.0"
```

Sırada lib dosyası var. Haydi rustgele:P

lib.rs

```csharp
use crossbeam::channel::{Receiver, Sender};
use log::{error, info, warn};
use std::thread;
use std::time::Duration;

// Yaptıracağımız işleri tutan bir enum türü. Receiver tarafından kullanılır.
#[derive(Debug)]
pub enum Job {
    WheatFarm,
    FishFarm,
    Shack(u8),       // Kaç kişilik bir kulübe olacağını u8 ile atabiliriz
    ArcherTower(u8), // Belki u8 ile okçu kulesinin seviyesini ifade ederiz
    Ditch(f32),      // hendeğin uzunluğunu u32 ile alabiliriz
}

// İşler tamamlandıktan sonra kanala bırakacağımız mesajlar için aşağıdaki enum kullanılabilir.
// Sender tarafından kullanılır.
#[derive(Debug)]
pub enum Harvest {
    WheatFarm,
    FishFarm,
    Shack,
    ArcherTower,
    Ditch,
}

// Fonksiyon Receiver ve Sender türünden iki parametre almakta.
// Buna göre kanaldan mesaj alma ve kanala mesaj bırakma işlevlerini üstlendiğini ifade edebiliriz.
pub fn pesant_worker(job_no: i32, jobs: Receiver<Job>, results: Sender<Harvest>) {
    warn!("{} numaralı iş", job_no);
    // Bir döngü ile gelen Job listesini dolaşıyoruz.
    for job in jobs {
        // her bir Job'u match ifadesi ile kontrol ediyor ve sembolik bir gecikme ile işletip
        // Sender için bir sonuç alıyoruz.
        let response = match job {
            Job::ArcherTower(l) => {
                info!("{} seviyesinde okçu kulesi inşaası", l);
                thread::sleep(Duration::from_secs_f32(2.0));
                Harvest::ArcherTower
            }
            Job::Ditch(l) => {
                info!("{} uzunluğunda hendek.", l);
                thread::sleep(Duration::from_secs_f32(1.5));
                Harvest::Ditch
            }
            Job::FishFarm => {
                info!("Kıyıya balık çifliği inşaası.");
                thread::sleep(Duration::from_secs_f32(3.5));
                Harvest::FishFarm
            }
            Job::WheatFarm => {
                info!("Buğday tarlası inşaası.");
                thread::sleep(Duration::from_secs_f32(0.5));
                Harvest::WheatFarm
            }
            Job::Shack(p) => {
                info!("{} kişilik kulübe inşaası.", p);
                thread::sleep(Duration::from_secs_f32(p as f32 * 0.30));
                Harvest::Shack
            }
        };
        info!("Yapılan iş {:?}", response);
        // İstenen işlem tamamlandıktan sonra sonucu Sender ile kanala bırakmaktayız.
        // send işlemi sırasında bir hata olma ihtimaline karşı da durumu kontrol ediyoruz.
        if results.send(response).is_err() {
            error!("Ups bir hata oluştu.");
            break;
        }
    }
}
```

Sanki Warcraft benzeri bir oyundayız da köylülerimize tarla, balık çifliği, kulübe gibi unsurları inşa etmek gibi görevler veriyoruz. İşleri ve yapılan çalışma sonuçlarını birer enum sabiti ile tutmaktayız. Hatta bunları birer olay (event) gibi de düşünebiliriz. pesant_worker isimli fonksiyon transmitter ve receiver nesnelerini parametre olarak alıp kanaldan bilgi okuma ve yazma opsiyonlarına sahip. Dolayısıyla mesaj dinleyip (yani gelen görevi alıp) buna bağlı işi icra ettikten sonra kanala bir bilgi yollayabilir (Hangi işin bittiğini söyleyen bir durum bilgisi). Kurgunun şekilleneceği yer ise elbette main fonksiyonu.

main.rs

```rust
use crossbeam::channel;
use std::thread;
use vorcraft::{pesant_worker, Job};

fn main() {
    env_logger::init();

    println!("Oyun başladı. Görevler dağıtılacak.");

    // İlk olarak unbounded kanallarımızı oluşturalım.
    // unbounded bir Tuple döner.
    // jt -> Jobs Transmitter, jr -> Jobs Receiver anlamında.
    // rt -> Results Transmitter, rr -> Results Receiver anlamında.
    let (jt, jr) = channel::unbounded();
    let (rt, rr) = channel::unbounded();

    let jr2 = jr.clone();
    let rt2 = rt.clone();
    let jr3 = jr.clone();
    let rt3 = rt.clone();

    // Şimdi üç thread oluşturacağız. Bunları JoinHandle serisinde toplayabiliriz.
    // Tohumlanan thread'ler pesant_worker fonksiyonunu çağırmakta ve buraya birer reciver ile
    // transmitter nesnesi göndermekte. Ancak her thread kendi transmitter ve receiver'ı ile çalışmalı.
    // Bu nedenle bir üst satırda clone'landıklarını görebiliriz.
    let handles = vec![
        thread::spawn(|| pesant_worker(1001, jr, rt)),
        thread::spawn(|| pesant_worker(1002, jr2, rt2)),
        thread::spawn(|| pesant_worker(1003, jr3, rt3)),
    ];

    // Birkaç kobay iş isteiğinden oluşan bir vector hazırlayalım
    let jobs = vec![
        Job::WheatFarm,
        Job::FishFarm,
        Job::Shack(8),
        Job::Ditch(23.0),
        Job::ArcherTower(100),
        Job::Shack(4),
        Job::FishFarm,
        Job::ArcherTower(50),
        Job::Shack(10),
    ];

    // Herbir iş isteğini ilgili kanala bırakacak bir döngü.
    for j in jobs {
        println!("İstenen iş {:?}", j);
        let _ = jt.send(j); // Kanala istenen işi bıraktık
    }
    // Artık kanala göndereceğimiz bir iş isteği kalmadığından transmitter'ı hemen kapatıyoruz.
    drop(jt);

    // Burada da thread'lerin yaptığı iş sonuçlarının aktığı kanalı dinleyerek sonuçları almaktayız.
    for r in rr {
        println!("Tamamlanan iş {:?}", r);
    }

    // İşlemler bitmeden main'in sonlanmasını engelliyoruz
    for h in handles {
        let _ = h.join();
    }
}
```

Dikkat edileceği üzere Job ve Result türleri için iki ayrı unbounded kanal oluşturulmakta. Dolayısıyla yapılacak işler ve sonuçlar için iki ayrı kanal açtığımızı düşünebiliriz. Senaryoda 3 işçimiz var ve her biri için birer thread oluşturulmakta. Thread'ler pesant_worker fonksiyonunu çağırırken kendileri için gerekli transmitter ve receiver nesnelerinin birer klonunu almaktalar (Clone kullanmadığımız durumda nasıl bir sorun oluşacağını araştırınız)

Artık örneğimizi çalıştırıp sonuçlara bakabiliriz. Pek tabii clippy ile kodun halini hatırını sorup gerekli düzenlemeleri yaparak ilerlediğimi ve olabildiğince idiomatic kod oluşturmaya çalıştığımı baştan söylemek isterim.

```bash
# Kendi örneğinizi çalıştırmadan önce sık sık clippy ile uyarıları gözden geçirin
cargo clippy
# Doğrudan çalıştırmak için aşağıdaki gibi,
cargo run

# logları görmek içinse aşağıdaki gibi.
RUST_LOG=info cargo run
```

İşte kendi sistemimde elde ettiğim çalışma zamanı sonuçları.

![channels_2.png](/assets/images/2022/channels_2.png)

Birden çok iş parçacığının yer aldığı ve bu işler arasında haberleşmenin önce çıktığı senaryolarda kanal kullanımı son derece yaygın. Built-in olarak gelen mpsc kütüphanesini kullanabileceğimiz gibi Rust konusunda ileri seviye olanların önerdiği crossbeam paketini de tercih edebiliriz. Ben aradaki farkları yorumlayacak mertebede yetkinliğe sahip olmasam da önerilere kulak verip crossbeam'i tercih ediyorum. Örnek üzerinde bol bol uğraşmanızı öneririm. Söz gelimi thread'lerin açılması için bir for döngüsü kullanabilir miyiz?

Böylece geldik bir rust pratiğimizin daha sonuna. Tekrardan görüşünce dek hepinize mutlu günler dilerim.

Örnek kodlara [Rust Pratikleri github reposu](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/vorcraft)ndan erişebilirsiniz.
