---
title: "Rust Pratikleri - Multithreading"
pubDate: 2022-02-06 09:00:00
categories:
  - Rust
tags:
  - Rust
  - Bash
  - Threading
  - Concurrency
  - GitHub
  - Ownership
---

# Rust Pratikleri - Multithreading
![image.axd](images/image.axd)

Uygulamalar işletim sistemlerince Process olarak ayağa kaldırılırlar. Bir process içerisindeki işleri birbirlerinden bağımsız olarak yapan thread'ler de söz konusu olabilir. Çoğu zaman çalıştırılabilir programın main fonksiyonu ile akan akış tek bir thread ile işleyişini sürdürür ama ihtiyaç dahillinde yeni thread'ler açmak gerekir. Rust için process içerisinde bir thread açmak oldukça kolaydır ve bellek tüketimi açısından maliyeti düşüktür. Ownership ve borrowing kuralları sayesinde bellek sahası güvende kalır ve özellikle data-race sorunları oluşmaz.

Nitekim bir veri parçasının sadece tek bir sahibi olabilir ve bu kural thread'ler için de geçerlidir. Üstelik aynı Process içerisindeki thread'ler birbirleriyle kolayca haberleşebilirler (channels konusunda bakarız) Şu da bir gerçek ki çok uzun zamandır birden fazla çekirdeğe sahip işlemcilerin olduğu sistemlerde çalışıyoruz. Bu işlemcilerdeki her bir çekirdek (core) belli bir anda tek bir thread işletebilir. Dolayısıyla programlarımızdaki thread'leri bu işlemci çekirdeklerine verip bir takım işlerin eş zamanlı çalıştırılmasını da sağlayabiliriz ki bu Parallel Processing olarak da bilinir. Ancak oraya gelmeden önce Rust dilinde thread'leri nasıl kullanırız pratik anlamda bilmemiz gerekiyor. İzleyen örnek Rust dilinde bir thread nasıl oluşturulur ve kullanılır sorusuna en basit haliyle cevap vermeye çalışır.

Senaryoda aynı öğrenci evinde kalan üç kafadar vardır. O güzel güneşli cumartesi gününün akşamında basketbol milli takımımızın maçını izlemek için misafirleri gelecektir. Zaman azdır. Karadenizli Dursun'un pazartesi günü gireceği Lineer Cebir sınavı vardır ama Danimarkalı Yensen ile Yeni Zellandalı Gibsın şimdilik boştadır ve oturma odasında tavla oynamaktadırlar. Dursun ders çalışırken Gibsın kendisine verilen listedekileri almak üzere alışverişe çıkabilir ve Yensen'de evi köşe bucak toparlayıp temizleyebilir. Esasında Yensen, Gibsın ve Dursun belli bir müddet birbirlerinden bağımsız şekilde hareket edip aksiyon alabilirler. Dursun dersini çalışmaya devam ederken, Gibsın alışverişi yapabilir ve Yensen'de evi süpürebilir. İşte size 3 tane thread. Şimdi sıra bu işleyişi programlamakta. İşe aşağıdaki terminal komutları ile başlayabiliriz.

Örneğe ait kodlara [rust-farm github reposundan](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/fellowship) ulaşabilirsiniz.

```bash
cargo new fellowship
cd fellowship
touch src/jhensen.rs
touch src/gibson.rs
touch src/dursun.rs
touch src/common.rs
```

fellowship isimli çalıştırılabilir uygulamada Dursun, Yensen ve Gibsın için ayrı birer modül dosyası yer almakta. Çıktıları izlemek için [bir önceki pratik](Rust Pratikleri - Loglama.md)te olduğu üzere loglama modülünü kullanabiliriz. Thread'lerin uzun süren işleri simüle etmesi için yardımcı bir fonksiyonumuz da var, common.rs. Çok yaratıcı bir isim değil ama şimdilik idare eder.

common.rs

```rust
use std::thread;
use std::time::Duration;

/// Örnekte thread'leri belli süre durdurup uzun çalışmaları simüle etmek içindir.
pub fn sleep_while(seconds: f32) {
    thread::sleep(Duration::from_secs_f32(seconds));
}
```

Tek yaptığı parametre olarak gelen süre kadar içerisinde çalıştırıldığı thread'i durdurmak. Şimdi Dursun ile devam edelim. Aşağıdaki basit içeriği oluşturmamız yeterli.

dursun.rs

```rust
use log::info;
use crate::common::sleep_while;

pub fn do_homework(work: &str) {
    info!("{} ödevine çalışmaya başladım", work);
    sleep_while(4.0);
    info!("Ödevler bitti");
}
```

Dursun'un ev ödevini yapma süresini de hesaba katarak çalışan basit bir fonksiyon söz konusu. Benzer şekilde Yensen ve Gibsın dosyalarını da oluşturalım.

gibson.rs

```rust
use crate::common::sleep_while;
use log::info;

pub fn clear_home(equipment: &str) -> bool {
    info!("Salonu temizlemeye başladım. Malzeme {}", equipment);
    sleep_while(2.0);
    info!("Şu anda balkonu temizliyorum.");
    sleep_while(3.0);
    info!("Banyo da temizlendi");
    sleep_while(2.0);
    info!("Mutfakta bitmiştir");
    true
}
```

jhensen.rs

```rust
use crate::common::sleep_while;
use log::info;

pub fn do_shopping(list: Vec<&str>) -> bool {
    info!("Alışveriş listesini aldım. Göreve başlıyorum.\n{:#?}", list);
    // sembolik olarak bu thread'i 5 saniye duraksatıyoruz
    sleep_while(5.0);
    info!("Alışveriş tamamlandı ve eve geldim :)");
    true
}
```

Her üç fonksiyonda da çok özel bir şey yok. Sadece belli operasyonları belli sürelerde icra eden işlevler olduğunu varsaymaktayız. Pratiğin can alıcı kısmı tahmin edeceğiniz üzere main fonksiyonunda yapılanlar.

```rust
use crate::dursun::do_homework;
use crate::gibson::clear_home;
use crate::jhensen::do_shopping;
use log::{error, warn};
use std::thread;

mod common;
mod dursun;
mod gibson;
mod jhensen;

fn main() {
    env_logger::init();
    println!("Akşama misafir varrrr!!!");

    let market = vec![
        "Kuruyemiş",
        "Portakal Suyu",
        "8 Adet Muz",
        "2 Kilo Kızartmalık Patates",
    ];
    let mut handles = Vec::new();

    // İki tane thread başlatılıyoruz ve bunları handles'e ekliyoruz.
    // Nitekim ana thread'in bu iki thread'teki işler bitene kadar durmasını da sağlamalıyız.
    let jhensen_handle = thread::spawn(|| do_shopping(market));
    handles.push(jhensen_handle);
    let gibson_handle = thread::spawn(|| clear_home("Roventa Max"));
    handles.push(gibson_handle);

    // dursun'un işi ise main thread içinde çalışan normal bir fonksiyon
    do_homework("Lineer Cebir");

    // Yukarıda eş zamanlı başlatılan threar'lerin bitmesini beklettiğimiz yer
    for handle in handles {
        if handle.join().unwrap_or(false) {
            warn!("Bir iş bitti!");
        } else {
            error!("Upss. Bu işte bir yanlış var sanki");
        }
    }
    println!("Her şey yolunda. Misafirlerimizi bekliyoruz :)");
}
```

Main zaten process içerisine açılan ana thread içinde yaşar. Ek olarak Yensen ve Gibsın'ın işleri için ayrı thread'ler açıyor ve bu thread'lerin işleyişleri bitmeden de main'in sonlanmasını engelliyoruz. Dursun'un ilgilendiği fonksiyon başka bir modül olarak dursa da main thread'e dahildir. Rust dilinde bir thread başlatmak için spawn fonksiyonundan yararlanılmakta. Bu metot ile başlatılan thread'ler sadece do_shoping ve clear_home fonksiyonlarını çağırıp bir takım parametreler aktarıyorlar. Başlatılan thread'leri ele alan nesneleri bir vector serisinde topluyoruz. Nitekim n sayıda thread olduğunda uygulama akışının belli bir noktasında onların sonuçlarını almadan ilerlemek istemeyebiliriz. Fonksiyon sonundaki for döngüsü bu vector nesnelerini dolaşıyor ve biten olduğu takdirde sonuçları paylaşıp sonraki iterasyondan yola devam ediyor. Dolayısıyla thread'lerdeki işler bitmeden main işlevi, yani program sonlanmıyor.

Senaryomuz en basit haliyle Rust ile thread oluşturma, join ile başka thread'lere dahil etme ve bekleme işlerini icra etmekte. Tabii biz çıktıları log üstünden rengarenk biçimde takip etmek istedik. Bu nedenle örneği aşağıdaki gibi RUST_LOG komutu ile çalıştırmalıyız.

```bash
# Alışkanlık olsun, idiomatic öneriler için clippy'yi kullanalım.
cargo clippy

# log paketini kullandığımız için örneği aşağıdaki gibi çalıştıralım.
RUST_LOG=info cargo run
```

Gelelim çalışma zamanı çıktılarına.

![image.axd](images/image.axd)

Örnekte move, channels gibi kullanmadığımız önemli kavramlar da var elbette ancak Rust öğrenmeye çalışanlar için eğlenceli bir pratik olduğunu düşünüyorum. Örneği genişletmek elbette sizin elinizde. Bir başka rust pratiğinde görüşünceye dek hepinize mutlu günler dilerim.
