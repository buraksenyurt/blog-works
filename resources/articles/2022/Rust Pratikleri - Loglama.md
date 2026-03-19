---
title: "Rust Pratikleri - Loglama"
pubDate: 2022-01-30 13:45:00
categories:
  - Rust
tags:
  - Rust
  - Bash
  - XML
  - Redis
  - RabbitMQ
  - Docker
  - GitHub
---

# Rust Pratikleri - Loglama
![voyager.jpg](images/voyager.jpg)Bugün bir redis, rabbitmq, kafka sunucusu başlattığımızda ya da docker container içerisine komut satırı açtığımızda terminal ekranına akan sayısız log içeriği olduğunu görüyoruz. Bu loglar hataları görmek, kodun akışını izlemek ve uyarıları çabucak fark etmek açısından sistem programcıları için son derece kıymetli bilgilerden oluşuyor. Çok doğal olarak Rust ile yazılan uygulamalar içinden de log yayınlamak isteyebiliriz ki Rust’ın asıl odağının sistem programlama olduğunu düşünecek olursak bu gereklidir. Rust Pratiklerinin bu ilk bölümünde [log](https://crates.io/crates/log) ve [env_logger](https://docs.rs/env_logger/0.9.0/env_logger/) küfelerini kullanarak basit anlamda loglamanın nasıl yapıldığını öğreneceğiz.

Normalde bir kütüphane geliştiriyorsak sadece log paketini kullanmak yeterlidir. Ancak çalıştırılabilir bir uygulamadan log basmak istersek onu implemente eden crate'leri kullanmamız gerekir. Façade görevi üstlenen bu tetikleyiciler farklı ortamlara log basma kabiliyetlerine de sahiptir. İzleyen örnekte minimal olanlardan env_logger kullanılıyor. Ancak daha karmaşık sistemler için log4rs (log4net gibi düşünün), web assembly'lar için console_log, android, windows, unix gibi ortamlar için de android_log, win_dbg_logger, syslog vb kütüphaneler de var.

> Örneğe ait kodlara [rust-farm github reposundan](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/logging) ulaşabilirsiniz.

Senaryoda kobay bir Voyager veri yapısı ve üzerinde uygulanan birkaç fonksiyon yer alıyor. Fonksiyonların bazı noktalarına log mekanizması serpiştirilmiş durumda. Amaç terminal ekranına çeşitli türde log basılmasını sağlamak. Örneğimizi aşağıdaki terminal komutları ile oluşturarak devam edelim.

```bash
cargo new logging
touch src/lib.rs
```

İlk olarak toml dosyasına gerekli crate bildirimlerini ekleyelim.

```xml
[package]
name = "logging"
version = "0.1.0"
edition = "2021"

[dependencies]
# library'ler için log paketi yeterlidir,
log="0.4.14"
# ancak executable programlarda log'un dışarıya çıkartılması gerekir.
# log paketinin farklı implementasyonları bu amaçla kullanılır.
# örneğin env_logger.
env_logger = "0.9.0"
```

lib.rs dosyasının içerisinde Voyager isimli kobay bir struct ve onunla ilişkilendirilmiş çeşitli fonksiyonlar yer alıyor.

```rust
// log paketinden kullanacağımız macro'lar için gerekli bildirimler
use log::{debug, error, info, trace, warn};

#[derive(Debug)]
pub struct Voyager {
    pub life: u8,
    pub nickname: String,
    pub universe: String,
    pub is_active: bool,
}

impl Voyager {
    pub fn new(nickname: String) -> Self {
        // debug türünden bir log bırakıyoruz
        debug!(target:"app_events","Oyuna {} isimli bir gezgin katılıyor.",nickname);
        Voyager {
            nickname,
            ..Default::default()
        }
    }

    pub fn connect(&mut self, universe: String) {
        if !self.is_active && self.life > 0 {
            // info türünden bir log bırakıyoruz
            info!(target:"app_events","{}, {} evrenine bağlanıyor",self.nickname,universe);
            self.is_active = true;
            self.universe = universe;
        }
    }

    pub fn hited(&mut self) {
        self.life -= 1;
        // warn türünden bir log bırakıyoruz
        warn!(target:"app_events","{} vuruldu ve {} canı kaldı.",self.nickname,self.life);

        if self.life == 0 {
            // error türünden bir log bırakıyoruz
            error!(target:"app_events","{} ne yazık ki tüm canlarını kaybetti. Bağlantısı kesiliyor",self.nickname);
            self.is_active = false;
        }
    }
}

impl Default for Voyager {
    fn default() -> Self {
        let voyager = Voyager {
            life: 3,
            is_active: false,
            universe: String::from("nowhere"),
            nickname: String::from("unknown"),
        };
        // trace türünden bir log bırakıyoruz
        trace!(target:"app_events","Gezgin için varsayılan değerler yükleniyor.{:?}",voyager);
        voyager
    }
}
```

Voyager'ı çeşitli evrenlere bağlanan bir gezgin olarak düşünelim. Kolayca örneklemek için new fonksiyonuna sahip. Bir evrene bağlanmak için connect ve vurulduğunda can sayısını azaltmak için hited isimli iki fonksiyonu daha var. Ayrıca to_string kullanımı için önerilen Default trait'ini de uyarlamakta. Voyager ve bağlı fonksiyonlarında yer yer loglama yapıldığını görebiliriz. Logun seviyesine göre uygun bir macro fonksiyonu kullanılmaktadır. Uyarı mesajları için warn!, bilgilendirici notlar için info!, debug çıktıları için debug!, hata durumları için error! ve izleme operasyonları için trace! makrolarından yararlanıyoruz. Gelelim main dosyamıza. Onu da aşağıdaki şekilde kodlayarak devam edebiliriz.

```rust
use logging::Voyager;

fn main() {
    // önce loglayıcıyı oluşturalım
    env_logger::init();

    let mut gemini = Voyager::new(String::from("Gemini"));
    println!("{}\n", gemini.nickname);
    gemini.connect(String::from("Andromeda"));

    for _ in 0..3 {
        println!("{:?}", gemini);
        gemini.hited();
    }

    gemini.life = 1;
    gemini.connect(String::from("Orion"));
}
```

Önemli olan nokta env_logger için init fonksiyonunun çağırılmasıdır. Bu sayede terminal ortamına log atmak için gerekli ortam hazırlanmış olur. Kodun ilerleyen kısımlarında Voyager türünden bir değişken oluşturup üzerinde bazı işlemler uyguluyoruz. Önce Andromeda galaksisine seyahat eden gemini yolda birkaç kez vuruluyor. Derken program insafa gelip ona bir hak daha veriyor ve o da bunu Orion galaksisine giderek değerlendiriyor. Bu aşamadan sonra ilk olarak clippy ile ne kadar ideomatic kod yazdığımıza bakmakta yarar var. Eğer uyarılar varsa buna göre kodu düzeltmemiz iyi olacaktır. Ardından run komutu ile örneği çalıştırabiliriz. Ancak ekrana istediğimiz log bilgileri akmayacaktır. Log okumak için RUST_LOG komutundan yararlanılır. Terminalden yapacaklarımızı aşağıdaki şekilde özetleyebiliriz.

```bash
# önerileri alıp kodu toparlamak için
cargo clippy
# library'nin başarılı şekilde build olup olmadığını görmek için
cargo build --lib

# varsayılan çalıştırmada sadece ERROR Logları görünür
cargo run

# log çıktılarını okumak için farklı yollar kullanabiliriz.
# warn ve error mesajlarını gösterir
RUST_LOG=warn cargo run

# trace ile birlikte debug,warn,info,error mesajlarını alırız
RUST_LOG=trace cargo run
```

Sonuç itibariyle aşağıdaki renkli ve iç ısıtan çıktıyı alabilmemiz gerekiyor.

![logging_1.png](images/logging_1.png)

Görüldüğü üzere log ve env_logger küfelerini kullanarak terminale log bırakmak oldukça pratik. Bir başka rust pratiğinde görüşmek dileğiyle, sağlıklı günler.
