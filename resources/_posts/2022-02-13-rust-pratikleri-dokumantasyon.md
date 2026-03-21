---
layout: post
title: "Rust Pratikleri - Dokümantasyon"
date: 2022-02-13 09:00:00 +0300
categories:
  - rust
tags:
  - rust
  - rust-lang
  - documentation
  - idiomatic-code
  - cargo
  - clippy
---
Bir programlama dilini iyi yapan ve onu öne çıkaran bazı önemli unsurlar vardır. İdeal bir söz dizimi oluşturulması için önerilerde bulunmak, kullanılan fonksiyon veya türlerle ilgili yardım dokümantasyonları sunmak, merkezi ve başarılı bir paket yönetim sistemine sahip olmak bunlar arasında sayılabilir. Rust dilindeki pek çok kural sayesinde bellek sahasının güvende kaldığı (memory safe), dangle pointer, data race, memory leak gibi sorunların oluşmadığı, performansı yüksek ve üstelik bütün bunlar için garbage collector benzeri mekanizmalara ihtiyaç duymayacak şekilde geliştirme yapmamız mümkün. Yine de idiomatic olarak ifade edilen ve dilin en ideal şekilde kullanılmasını tarifleyen ihtiyaç için yardım almamız gerekiyor. Bu anlamda cargo clippy en büyük destekçimiz. Ancak kaliteli kodlamanın olmazsa olmaz önemli özelliklerinden birisi de elbette verimli içerik sunan dokümantasyon. Özellikle yazdığımız kütüphaneleri herkesin kullanımına açmak istediğimiz senaryolarda bu konuya azami özeni göstermek lazım.

![horten.jpg](/assets/images/2022/horten.jpg)

Rust'ın kendi built-in içeriğinin sağladığı dokümantasyon son derece etkilidir. Neredeyse kitaplara taş çıkartacak kadar iyi bilgi verir ve aynı zamanda ilgili enstrümanın kullanımına dair örnekler sunar. [crates.io](https://crates.io/), Rust kodlamacıların kullandığı en önemli kütüphane deposudur. Buraya çıkan ve silinemeyen kütüphanelerimiz için iyi bir dokümantasyon sunmak programcı olarak vazifemizdir.

Peki Rust tarafında dokümantasyon nasıl sağlanır? Aslında bir.Net geliştiricinin oldukça aşina olduğu şekilde XML Comment benzeri yorum satırları ile kodun dokümantasyonu çıkarılabilmekte. İşin güzel yanı Rust'ın bu dokümanlarda Markdown formatını kullanıyor olması. Yani dokümanda link verebilir, resim gösterebilir, bullet list, heading vs kullanabiliriz. Bu sayede farklı ortamlara kolayca entegre olabilen ve hatta HTML olarak da servis edilebilen bir içerikle karşılaşıyoruz. Gelin bu laf salatalığını bırakalım ve örnek bir kod parçasını dokümante edelim.

```bash
cargo new doc_sample
cd doc_sample
touch src/lib.rs
```

Birkaç dil enstrümanını kullandığımız hafifsiklet bir modülümüz var. Modül içeriğini aşağıdaki gibi geliştirebiliriz. Yorum satırlarına dikkat!

```csharp
//! Maket uçak yapımı sevenler için koleksiyonlarını yönetecekleri basit kütüphane.
//!
//! # Bazı Yardımcı Bilgiler
//!
//! Model uçak yapımı çok sevilen bir hobidir. Meşakkatli bir iştir ama sonuçları oldukça harikadır.
//! Yeni başlayanlar genelde 1:72 ölçekle çalışır. Az parçadan oluşan maketlerin bazıları için boya,
//! fırça, yapıştırıcı gibi unsurlar paketle birlikte gönderilir. İlk önce parçaların uygun şekilde
//! boyanması gerekir. Sonrasında plana uygun olarak yapıştırma işlemleri icra edilir. En son olarak
//! da logoların yapıştırılması işlemi uygulanır.
//!
//! # İçerik
//!
//! Kütüphanede yer alan temel enstrümanlar.
///
/// Bir maket modelinin temel bilgilerini taşır.
pub struct Model {
    /// Modelin başlığı. Örneğin Messerschmitt 109
    pub title: String,
    /// Model yapımının zorluk derecesi [Level]
    pub level: Level,
    /// Modelin parça sayısı
    pub part_count: u8,
    /// Güncel liste fiyatı
    pub list_price: f32,
}

#[derive(Debug)]
pub enum Level {
    /// Nispeten yapımı kolay olan seviye
    Easy,
    /// Artık güzel bir şeyler görmek isteyenlerin seviyesi
    Hard,
    /// Sınırları zorlayanların seviyesi
    Pro,
}

use std::fmt::{Display, Formatter};

impl Display for Model {
    /// Modelin bilgilerini String formatta geri döndürür.
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{}. Zorlu {:?}.{} parça. Liste fiyatı {}",
            self.title, self.level, self.part_count, self.list_price
        )
    }
}

/// Uygulanabilecek en yüsek indirim oranı
pub const MAX_DISCOUNT_LEVEL: f32 = 10.99;

impl Model {
    /// Yeni bir model nesnesi oluşturmak için kullanılır.
    pub fn new(title: String, level: Level, part_count: u8, list_price: f32) -> Self {
        Model {
            title,
            level,
            part_count,
            list_price,
        }
    }

    /// Modelin fiyatına belirtilen miktarda indirim uygular
    pub fn apply_discount(&mut self, amount: f32) {
        if amount <= MAX_DISCOUNT_LEVEL {
            self.list_price -= amount
        } else {
            self.list_price -= MAX_DISCOUNT_LEVEL
        }
    }

    // cargo clippy sonrası aşağıdaki kullanım yerine Display trait'inin uyarlanması önerildi
    /*pub fn to_string(&self) -> String {
        format!(
            "{}. Zorluk {:?}.{} parça. Liste fiyatı {}",
            self.title, self.level, self.part_count, self.list_price
        )
    }*/
}
```

Kodda benim gibi maket uçak yapmayı sevenler için bir veri yapısı ve bağlı birkaç fonksiyon yer alıyor. Maketin modeli, zorluk derecesi, parça sayısı ve fiyatı gibi az sayıda özellik barındıran Model isimli struct var. Yeni bir nesneyi kolayca oluşturmak için new fonksiyonunu uyarlıyor ve hatta fiyat indirimi için de bir metot sunuyoruz. İlk etapta makete ait bilgileri String türde döndüren to_string isimli bir metot kullandık. Ancak yazının başında kısaca bahsettiğimiz cargo clippy komutunu kullandığımızda idiomatic bir öneride bulunduğunu göreceğiz.

```bash
cargo clippy
```

![doc_sample_3.png](/assets/images/2022/doc_sample_3.png)

Dikkat edileceği üzere to_string yerine Display trait'ini implemente etmemiz öneriliyor. Bizde kod içeriğini buna göre düzenledik. Dokümantasyonumuz oldukça sadece. Aslında eklenebilecek bazı kısımlar var. Örneğin bir Model nesnesi nasıl örneklenir ve kod içinde kullanılır yine dokümante edebiliriz. Bu pratikte ihtiyaç duymadım ancak markdown'lar da olduğu gibi bir yol izleyebilirsiniz.

```text
/// # Examples
/// ```
/// // Burada kod kullanım örneği yer alıyor
/// 
/// ```
///
```

Dikkat etmemiz gereken tek şey yardım dokümantasyonuna ekleyeceğimiz kod parçalarının da çalışır olması. Rust test aracı buradaki kodları denetler ve çalıştırılabilir olmasını bekler;) Mesela apply_discount fonksiyonumuz için aşağıdaki gibi bir içerik hazırladığımızı düşünelim.

```csharp
/// Modelin fiyatına belirtilen miktarda indirim uygular
///
/// # Examples
///
/// ```
/// let m109 = Model::new(String::from("Meserrschmitt 109"), Level::Easy, 42, 270);
/// m109.apply_discount(32.0);
/// assert_eq!(m109.list_price,228.0);
/// ```
///
pub fn apply_discount(&mut self, amount: f32) {
   if amount <= MAX_DISCOUNT_LEVEL {
      self.list_price -= amount
   } else {
      self.list_price -= MAX_DISCOUNT_LEVEL
   }
}
```

Dikkat edileceği üzere examples kısmında bir Model nesnesi üretiyor ve apply_discount fonksiyonunu çağırıyoruz. İlk bakışta bir problem yok gibi görülebilir. Birde aşağıdaki komutla deneyelim.

```text
cargo test --doc
```

![doc_sample_6.png](/assets/images/2022/doc_sample_6.png)

Upsss!!! Görüldüğü üzere Model ve Level türleri bulunamıyor. Bunları ekleyerek ilerlesek bile dokümanı tekrardan test ettiğimizde bu kez immutable değişken kullanımı ve hatta 270 değerini float kullanmamak sebebiyle farklı hatalara da rastlarız. Aşağıdaki ekran görüntüsünde olduğu gibi.

![doc_sample_9.png](/assets/images/2022/doc_sample_9.png)

Sözün özü dokümantasyonda gerçekten çalıştırılabilir bir kod parçasının kondması bekleniyor. Dolayısıyla içeriği aşağıdaki şekilde değiştirmeliyiz.

```rust
/// Modelin fiyatına belirtilen miktarda indirim uygular
///
/// # Examples
///
/// ```
/// use doc_sample::{Model,Level};
/// let mut m109 = Model::new(String::from("Meserrschmitt 109"), Level::Easy, 42, 270.0);
/// m109.apply_discount(32.0);
/// assert_eq!(m109.list_price,259.01);
/// ```
///
```

Artık doküman içerisindeki kod parçası da çalıştırılabilir durumda, üstelik testi de başarılı.

![doc_sample_7.png](/assets/images/2022/doc_sample_7.png)

Şimdi eklediğimiz yorum satırlarına istinaden nasıl bir doküman çıktısı alabiliriz bir bakalım.

```bash
# Normalde doküman üretimi için aşağıdaki komut kullanılır
cargo doc

# Ancak bağımlı kütüphanelerin dokümantasyonunu işin içerisine dahil etmek istemezsek şöyle kullanabiliriz.
cargo doc --no-deps

# Hatta geliştirme sırasında şu kullanımı daha şık olur. 
cargo doc --no-deps --open

# doküman içine eklenmiş gerçek kod parçaları varsa test edebiliriz
cargo test --doc
```

Bu arada lib içerisindeki kodların ilk kısmında //! ile başlayan yorum satırları olduğunu görebilirsiniz. Bunlar inner doc olarak ifade edilirler ve HTML dokümantasyonunda aşağıdaki şekilde gösterilirler.

![doc_sample_2.png](/assets/images/2022/doc_sample_2.png)

/// olarak kullandıklarımızda ise aşağıdaki sonuçları elde ederiz.

![doc_sample_1.png](/assets/images/2022/doc_sample_1.png)

İşin güzel yanı kütüphane içeriğini kullandığımız yerde de IDE'lerin bize yardımcı olmasıdır. main.rs içeriğini aşağıdaki gibi tasarladığımızı düşünelim.

```rust
use doc_sample::{Level, Model};

fn main() {
    let mut m109 = Model::new(String::from("Meserrschmitt 109"), Level::Easy, 42, 270.50);
    println!("{}", m109.to_string());
    m109.apply_discount(32.0);
    println!("{}", m109.to_string());
}
```

Kullandığım IntelliJ IDEA'da apply_discount üstüne gelince aşağıdaki gibi çıktı elde ettim.

![doc_sample_8.png](/assets/images/2022/doc_sample_8.png)

Görüldüğü üzere Rust kod dokümantasyonu konusunda pek çok dil veya çatıda olduğu gibi bir standart sunmaktadır. Avantajlı noktalardan birisi bu dokümantasyonun markdown formatını kullanmasıdır. Diğer yandan yorumlara serpiştirilen örnek kod parçaları varsa bunların çalışır hatta testten geçmiş olmasını da garanti edebilir. Elbette dokümantasyonun içeriği, en önemli kısımdır. Enstrümanları kafaları fazla karaştırmadan basit ve kaliteli bir şekilde anlatmak mühimdir. Pek tabii dokümantasyon oluşturmada nasıl bir yol izleneceğine dair en güzel kaynak Rust'ın var olan yardım dokümanlarıdır. Böylece geldik Rust Pratikleri serisinden bir bölümün daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

Örneğe ait kodlara [rust-farm github reposundan](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/doc_sample) erişebilirsiniz.
