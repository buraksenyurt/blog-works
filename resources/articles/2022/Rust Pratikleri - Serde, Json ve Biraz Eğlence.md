---
title: "Rust Pratikleri - Serde, Json ve Biraz Eğlence"
pubDate: 2022-03-13 09:00:00
categories:
  - Rust
tags:
  - Rust
  - Java
  - Bash
  - XML
  - REST
  - JSON
  - Serialization
  - GitHub
---

# Rust Pratikleri - Serde, Json ve Biraz Eğlence
![image.axd](images/image.axd)

Sanıyorum JSON veriler ile çalışmayan programlama dili veya ortam yoktur. Sonuç itibariyle bir takım verileri düzenli, standart ve insan gözüyle okunabilir bir formatta tutmanın en iyi yollarından birisi şüphesiz ki JSON. Öncesinden gelen XML formatına göre daha az yer tutması da cazibesini artırmaktadır. Tabii günümüzde BSON gibi sıkıştırılabilir ve çok daha hızlı yol alabilen seçenekler de mevcut ama rust dilini öğrenirken bunun pratiğini yapmadan olmaz. Bu noktada işimizi epey kolaylaştıran bir kütüphane olduğunu ifade edebilirim. [Serde](https://docs.serde.rs/serde/index.html) isimli çatı (ki framework olduğu vurgulanıyor JSON ile çalışma konusunda epey popüler. Hiç vakit kaybetmeden örnek bir uygulama üstünden ilerleyelim.

Senaryomuzda basit olması nedeniyle sıklıkla tercih ettiğim bir terminal uygulaması kullanacağız. Programı komut satırından argüman vererek çalıştırabileceğiz. Dolayısıyla yürütülebilir bir rust programına komut satırından argüman nasıl yollanır öğreneceğiz. JSON dosya içeriğinde türlü türlü ipuçları olacak. Uygulamamızdan rustgele veya belli bir konu başlığında ipucu isteyebileceğiz. İpuçlarını tutan JSON dosyasında ise örnek olarak aşağıdaki gibi bilgiler saklayacağız.

```java
[
  {
  "id": 1,
  "category": "Rust",
  "description": "Veri türleri varsayılan olarak immutable'dır."
  },
  {
    "id": 2,
    "category": "Rust",
    "description": "Kullanıcı tanımlı veri türü oluşturmanın bir yolu struct kullanmaktır."
  },
...
```

Dosyanın tamamı ve örneğe ait kodlar için [github reposu](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/gettip)na uğrayabilirsiniz. İlk olarak aşağıdaki terminal komutu ile projemizi oluşturalım.

```bash
cargo new gettip
```

serde paketini kullanacağız ancak rastgele bir ipucu getirilmesi de elbette güzel olur. Bu amaçla rand kütüphanesini kullanabiliriz. Dolayısıyla toml dosyasındaki dependencies kısmında ilgili kütüphane bildirimlerini eklemek gerekiyor. Aynen aşağıda olduğu gibi.

```text
[package]
name = "gettip"
version = "0.1.0"
edition = "2021"

[dependencies]
serde = { version = "1.0.133", features = ["derive"] }
serde_json="1.0.74"
rand="0.8.4"
```

serde paketi için birde derive özelliğini kullanmak istediğimizi belirttik. Kod tarafında JSON dosyasındaki bir ipucunu işaret edebilecek bir struct tanımlayacağız. Bu veri modelinin JSON serileştirme davranışlarını otomatik olarak kazanması için deserialize trait'ini derive niteliği üstünden kullandıracağız. Nitekim dosya içeriğini bizim tanımladığımız bir veri yapısına ters serileştirme ile almamız gerekiyor. features içerisinde tanımlanan derive bildirimi bu fonksiyonelliği uygulamamıza kazandıracak.

Gelelim kod tarafına. main.rs dosya içeriğini aşağıdaki gibi yazarak devam edelim.

```rust
use rand::{thread_rng, Rng};
use serde::{Deserialize};
use std::env;
use std::fmt::{Display, Formatter};
use std::fs::File;
use std::io::BufReader;

fn main() {
    let args: Vec<String> = env::args().collect();
    let tips = load_tips();

    match args.len() {
        2 => {
            let command = &args[1];
            if command == "r" {
                println!("{}", get_random_tip(&tips));
            } else {
                println!("r girerek deneyin.");
            }
        }
        3 => {
            let category = &args[2];
            let sub_tips: Vec<Tip> = tips
                .into_iter()
                .filter(|t| t.category == *category)
                .collect();
            if !sub_tips.is_empty() {
                let tip = get_random_tip(&sub_tips);
                println!("{}", tip);
            } else {
                println!("{} için hiçbir ipucu yok.", category);
            }
        }
        _ => {
            println!("Rustgele bir ipucu için `r` ile\nBelli bir kategoride rustgele ipucu için `r rust` ile \ndeneyin lütfen;)");
        }
    };
}

fn load_tips() -> Vec<Tip> {
    let f = File::open("tips.json").expect("Dosya açılırken hata");
    let reader = BufReader::new(f);
    let tips: Vec<Tip> = serde_json::from_reader(reader).expect("json okumada hata");
    tips
}

fn get_random_tip(tips: &[Tip]) -> String {
    let mut rng = thread_rng();
    let number = rng.gen_range(0..tips.len());
    tips[number].to_string()
}

#[derive(Deserialize)]
pub struct Tip {
    pub id: i32,
    pub category: String,
    pub description: String,
}

impl Display for Tip {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{} -> {}", self.category, self.description)
    }
}
```

Bir ipucu Tip isimli struct ile temsil edilmekte. Bu veri modeli json içerisindeki nitelikleri karşılayan alanlar içermekte. id, category ve description. Bir ipucunu yazılabilir formatta servis etmek içinse Display trait'ini uyguluyoruz. Böylece to_string fonksiyonuna karşılık vermekteyiz. load_tips isimli fonksiyon adından da anlaşılacağı üzere tips.json dosya içeriğini okuyup, Tip nesnelerinden oluşan bir vector olarak geriye döndürmekte. get_random_tip fonksiyonu ise Tip türünden diziyi referans alıp rastgele üretilen bir değeri kullanarak metinsel formatta bilgi döndürmekte.

Program başında env modülünden hareketle terminalden girilen argümanları yakalamaktayız. İki veya üç argümanla çalışacağımız için bu durumu kontrol altına alan bir match ifadesi söz konusu. Daha az veya daha çok argüman gelmesi halinde bir bilgi mesajı vererek kullanıcıyı uyarıyoruz. İki parametre gelmesi halinde ilgili anahtarın r olup olmadığına göre kod akışı değişiyor. Eğer üç argüman girilmişse son argümanın kategori olduğunu düşünerek hareket ediyoruz. Burada da Higher Order Function'lardan yararlanarak basit bir filtreleme yaptığımızı görebilirsiniz.

Örnekte hata yönetimi konusunda çok radikal işler yapmadığımızı ifade edebilirim. Result döndüren birkaç operasyonda expect fonksiyonunu kullanarak olası panik durumunda ek bilgi verip uygulamanın sonlanmasına müsaade ettik.

Artık uygulamayı deneyebilir ve sonuçlara bakabiliriz.

```bash
cargo run r
cargo run r C#
cargo run r Rust
cargo run r Arch
cargo run r none
```

Kendi sistemimdeki çalışma zamanına ait çıktıyı aşağıda görebilirsiniz.

![image.axd](images/image.axd)

Tabii örneğimizi yürütülebilir bir binary olarak hazırlamakta yarar var. Bunun için build işlemini aşağıdaki gibi icra edip yine gerekli denemelerimizi yapabiliriz.

```bash
cargo build --release
cd target/release
./gettip r rust
./gettip r
```

Ancak şöyle bir hatırlatma yapalım. Program, tips.json dosyası ile çalıştığından onu da binary'nin olduğu klasörle birlikte dağıtmalıyız. En azından siz daha iyi bir çözüm bulana kadar böyle. Şimdi cargo paketine gereksinim duymadan binary'yi yürütebiliriz. İşte birkaç örnek.

![image.axd](images/image.axd)

Rust programlama dilinde başlangıç seviyesini tamamlamış herkesin yapabileceği türden bir örnek. Bizim için işleri kolaylaştıran serde ve rand kütüphanelerini kullandık. Biraz pattern matching, biraz dosya okuma, komut satırından argüman alma, fonksiyon tanımlama, vector, struct ve trait uyarlaması gibi konuları değerlendirmiş olduk. Elbette örnek daha da geliştirilebilir ve eksik yönleri de yok değil. Örneğin JSON dosya içeriği çok büyük olursa uygulama performansı bundan nasıl etkilenir? Ya da ipuçlarını bir JSON dosyasından değil de herhangi bir servisten alsak güzel olmaz mı? Kendinizi güçlü gördüğünüz bir programlama dili ile pekala REST tabanlı bir servis yazıp bu terminal uygulamasından çağırmayı deneyebilirsiniz. JSON dosyasını binary ile birlikte taşımanın daha kolay bir yolu var mıdır? Örnekte sadece dosyadan json veri okuyup ters serileştirme ile bir vektor dizisine nasıl alınacağına baktık. Peki komut satırından bu dosyaya yeni bir ipucu eklemek istersek nasıl bir yol izleriz? İşte bana ve size pek güzel sorular:)

Böylece geldik bir rust pratiğimizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
