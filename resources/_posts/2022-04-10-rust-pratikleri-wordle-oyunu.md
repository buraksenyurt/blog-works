---
layout: post
title: "Rust Pratikleri - Wordle Oyunu"
date: 2022-04-10 09:00:00 +0300
categories:
  - rust
tags:
  - rust
  - bash
  - github
---
Doğruyu söylemek gerekirse mobil oyunlarla çok fazla aram yok. Ancak platform ne olursa olsun oyun yazmaya çalışmak programlama dili öğrenenler için çok iyi bir egzersiz. Bu nedenle bazen var olan oyunların nasıl yazıldığını anlatan öğretileri uygulamaya çalışıyorum. Geçtiğimiz günlerde [The Pragmatic Programmers](https://medium.com/pragmatic-programmers) mecrasından Herbert Wolverson'un Wordle isimli popüler bir oyunun Rust ile nasıl yazılabileceğini anlattığı [şu yazısına](https://medium.com/pragmatic-programmers/rustle-5c15d1c153a1) denk geldim. Tamda başta belirttiğim tipte bir egzersiz karşıma çıkmıştı. E boş durur muyum? Adım adım tatbik etmeye karar verdim. Nitekim bu yolculuk Rust ile ilgili birçok şey öğretecekti bana. Her şeyden önce pratik yapacaktım. İşte bu yazıda izlediğim adımları ve kendi yorumlarımı bulabilirsiniz.

Öncelikle Wordle oyunu nasıl bir şey anlamak lazım. Google Play veya App Store'dan indirebileceğimiz oyunun Huawei App Gallery'de bir karşılığını bulamadım ama benzerleri vardı. Önce oynamalıydım ki nasıl bir şey olduğunu anlayayım. Düşündüren bir kelime oyunu olduğunu ifade edebilirim. Hatta isterseniz [Newyork Times'ın şu adresinden](https://www.nytimes.com/games/wordle/index.html) hemen çevrimiçi bir sürümünü de deneyebilirsiniz. Kendi denememden bir örneği de aşağıdaki ekran görüntüsü ile paylaşmak isterim.

![wordle_1.png](/assets/images/2022/wordle_1.png)

Program 5 harfli bir kelime tutuyor ve oyun sahası 5 sütün, 6 satırdan oluşuyor. İlk satıra (en üst satır) 5 harfli ve anlamlı bir kelime yazarak başlıyorsunuz. Program yazdığınız kelimeye bakıp harflerin doğru yerleşimde olup olmadığını kontrol ediyor. Eğer harf tam da yerindeyse arka planı yeşil renge boyuyor. Harf doğru ama yanlış yerdeyse bu sefer arka planı sarı renge boyuyor. Eğer harf programın tuttuğu kelimede yoksa gri rengi kullanıyor. Kullanılan harfleri de aşağıdaki klavyede farklı renklere boyayarak işaretliyor. Bu ipuçlarından yararlanan oyuncu doğru kelimeyi bulmaya çalışıyor ve hakkı olduğu sürece sonraki satıra geçip yeni bir tahminde bulunuyor. Ben ilk satırda S ve E harflerini tutturdum ama yerleri yanlıştı. İkinci denemede ise büyük bir şans eseri birinci harfi tam da olması gerektiği yerde buldum. Ancak E ve T harfleri halen yanlış yerdeydi. Sonuç olarak 6ncı seferde SWEER kelimesini bulmayı başardım. Şimdi Herbert'e kulak verelim. Bakalım neler neler yapacağız?:)

```bash
# Önce projeyi oluşturuyoruz elbette
cargo new wordle
```

Programda iki yardımcı küfe/sandık (crate) kullanılıyor. Pek çok rust öğretisinde rastgele sayı üretmek için rand paketi kullanılmakta. Bu örnekte ise bracket-random isimli bir paket var. Esasında bracket-random, Herbert'in [Hands-On Rust,Effective Learning through 2D Game Development and Play](https://www.amazon.com/Hands-Rust-Effective-Learning-Development-dp-1680508164/dp/1680508164/ref=mt_other?_encoding=UTF8&me=&qid=) isimli kitabında kullandığı bracket ekosisteminin bir parçası. Diğer yandan terminal penceresini renklendirmek için (ki bayılırım buna) colored isimli bir modülden yararlanılmakta. Bu paket bildirimlerini tahmin edileceği üzere toml dosyasına eklememiz gerekiyor.

```text
[dependencies]
bracket-random = "0.8.2"
colored="2.0.0"
```

Çok doğal olarak program bir veri havuzundan rastgele bir kelime tutmalı. Tabii ki oyuncuya göstermeden:P Herbert bu kelime havuzunu oluşturmak için kaynak olarak [şu adresteki text dosya içeriğini önermiş](https://www.wordgamedictionary.com/twl06/download/twl06.txt). Ben işi uzatmamak için aynı veri kaynağını kullanarak devam ettim lakin farklı bir alandaki sözlüğü de kullanabiliriz. Örneğin 5 harfli bilişim terimleri gibi. Henüz anlayamadım ama veri dosyasında 5 harfli dışında birçok kelime de yer alıyor. Sanırım sadece beş harflileri ayıklamak zor olduğu için komple alıp kullanmış. Elbette kod tarafında 5 harfli olmayanları eleyeceğiz. Dosyayı words.data ismiyle src klasörü altına ekleyerek devam edebiliriz. Main fonksiyonunun fazla karışmaması için çekirdek fonksiyonları lib.rs isimli yeni bir modül üstünde tutabiliriz. Uzun bir içerik ancak sabırlı olun.

```rust
/*
   Kelime listesi sadece okuma amaçlı kullanılacak.
   include_str! makrosu parametre olarak gelen dosyayı derleme zamanında alıp kaynak kodun içerisine gömer.
   Dolayısıyla data dosyasını release aldıktan sonra programı götürdüğümüz yere taşımaya gerek yoktur.
*/
use bracket_random::prelude::RandomNumberGenerator;
use colored::*;
use std::collections::HashSet;

const WORDS: &str = include_str!("words.data");
const WORD_LENGTH: usize = 5; // Kelime maksimum 5 harfli olabilir
const TRY_COUNT: usize = 6; // Oyuncuya 6 deneme hakkı veriyoruz

/// Kelime üstünde bazı iyileştirmeler yapan fonksiyondur.
fn sanitize_word(word: &str) -> String {
    /*
    Bir kaç Higher Order Function kullanarak gelen kelime üstünde işlmeler yapılmakta.
    önce gereksiz boşluklar trim ile atılıyor.
    Kelime büyük harfe çevriliyor ve chars fonksiyonu ile tüm karakter listesi alınıyor.
    Rust'ta tüm string'ler UTF-8 formatında. Dolayısıyla aralara harf olmayan karakterler(emoji gibi) gelebilir.
    Bunu önlemek için kelimede yer alan ascii karakterler bulunuyor. filter fonksiyonu bunun için kullanılmakta.
    Son olarak bulunan karakterler collect ile toplanıp fonksiyondan String değişken olarak döndürülüyor.
     */
    word.trim()
        .to_uppercase()
        .chars()
        .filter(|c| c.is_ascii_uppercase())
        .collect()
}

/// Kelimeleri String türden vektöre alan fonksiyon
fn word_list() -> Vec<String> {
    /*
    Şimdi elimizde kelimeleri tutan dosya var. Bunu WORDS isimli constant'ta tutuyoruz.
    Bu dosyadaki herbir satırı okuyup, sanitize işleminden geçirdikten sonra,
    uzunluğu 5 karakter olanları String türden bir vector'de topluyoruz.
     */
    WORDS
        .split('\n')
        .map(sanitize_word)
        .filter(|line| line.len() == WORD_LENGTH)
        .collect()
}

/// Yönetici sınıf. Kelimeler, seçilen kelimeyi, tahmin edilen harfleri ve tahmin edilen kelimeleri yönetir
pub struct Manager {
    available_words: Vec<String>,
    chosen_word: String,
    guessed_letters: HashSet<char>,
    guesses: Vec<String>,
}
impl Default for Manager {
    fn default() -> Self {
        Self::new()
    }
}

impl Manager {
    /*
       Yapıcı metot kelimelerin olduğu sözlükten rastgele bir kelimeyi de seçerek bir Manager örneği döner.
    */
    pub fn new() -> Self {
        // Rastgele sayı üretici
        let mut rnd_gnr = RandomNumberGenerator::new();
        let dictionary = word_list();
        // random_slice_entry fonksiyonu parametre olarak gelen dilimden rastgele bir tane çeker.
        // değerin bir klonunu word değişkenine alırız.
        let chosen_word = rnd_gnr.random_slice_entry(&dictionary).unwrap().clone();
        Self {
            available_words: dictionary,
            chosen_word,
            guessed_letters: HashSet::new(),
            guesses: Vec::new(),
        }
    }

    /*
       Oyun sahamız terminal ekranı. Manager'ın tuttuğu kelime ve
       oyuncunun tahminlerine göre 5X6 lık matrisi çizen bir fonksiona ihtiyacımız var.
       self üzerinden guessed_letters vector'üne oyuncunun tahmin ettiği
       ama programın tuttuğu kelimede olmayan harfler eklenecek.
       Bu nedenle self, mutable referans olarak alındı.
    */
    /// Oyun sahasını tahmin edilen kelimeler ve sonuçları ile çizer
    pub fn draw_board(&mut self) {
        // önce yapılan tahminleri gezen bir döngü açıyoruz.
        // for_each fonksiyonunda bir tuple kullandığımıza dikkat edelim.
        // Bu tuple'da satır numarası ve guesses vector'ündeki kelime yer alır.
        // Satır numarasını şu an için kullanmayacağız. O yüzden _ ile açıkça kullanmayacağımızı belirttik.
        self.guesses.iter().enumerate().for_each(|(_, guess)| {
            // Şimdi bulunduğumuz satırdaki kelimenin harflerini dolaşacağız
            // Yine for_each döngüsü kullanılıyor. Her iterasyonda kelimedeki karakteri ve indisini bir tuple ile ele alıyoruz.
            guess.chars().enumerate().for_each(|(i, c)| {
                // Şimdi karakterleri programın tuttuğu kelimedekiler ile karşılaştıracağız.

                // Eğer chosen_word'deki i. sıradaki karakter guess'teki c karakterine eşitse
                // harf doğrudur ve kelimede doğru yerdedir
                let row = if self.chosen_word.chars().nth(i).unwrap() == c {
                    format!("{}", c).bright_green()
                } else if self.chosen_word.chars().any(|wc| wc == c) {
                    // Harf doğrudur ama yeri yanlıştır. Bunu da any fonksiyonu üstünden kontrol edebiliriz.
                    format!("{}", c).bright_yellow()
                } else {
                    // Harf programın tuttuğu kelimede yoksa bu durumda tahmin edilen harfler
                    // listesine eklenir ve kullanıcının karakteri kırmızıya boyanır.
                    self.guessed_letters.insert(c);
                    format!("{}", c).red()
                };
                print!("{} ", row);
            });
            println!(); // Bir alt satıra geç
        })
    }

    /*
       Wordle oyununda kullanılan harflerde gösterilmekte.
       Bunun için de yardımcı bir fonksiyon kullanılabilir
    */
    /// Oyuncunun kullandığı ama programın tuttuğu kelimede olmayan harflerin listesini ekrana basar.
    pub fn show_invalid_letters(&self) {
        if !self.guessed_letters.is_empty() {
            self.guessed_letters.iter().for_each(|c| print!("{}", c));
            println!(
                "{}",
                "\nBu harfleri kullandın ancak aklımdaki kelimede yoklar!\n"
                    .to_string()
                    .cyan()
            );
            println!()
        }
    }

    /*
       Bu fonksiyon kullanıcıdan tahminini alıp kontrol etmekte.
       Geçerli bir uzunlukta mı, oyunun kullandığı sözlük içerisinde yer alıyor mu gibi.
    */
    /// Oyuncudan tahminini alır ve belli kurallara göre kontrol eder
    pub fn take_guess(&mut self) -> String {
        println!(
            "{}",
            format!(
                "Hey! Oyuncu.\nHadi bana {} karakterden oluşan bir kelime yaz ve ENTER'a bas",
                WORD_LENGTH
            )
            .purple()
        );
        // Önce tahminde olupta programın tuttuğu kelimede olmayan harfleri gösterelim
        self.show_invalid_letters();
        // Kullanıcının tahmini ve kelimenin geçerli olup olmadığını tutan iki mutable değişkenimiz var.
        let mut user_guess = String::new();
        let mut is_guess_valid = false;
        // Döngümüz kullanıcının girdiği kelime tüm kuralları sağlayana kadar devam edecek
        while !is_guess_valid {
            user_guess = String::new();
            // Oyuncunun girdisi terminal ekranından olacak.
            // İçeriği read_line fonksiyonu ile user_guess değişkenine yazabiliriz.
            // Çok büyük bir problem olmayacağını düşünerekten unwrap ile girilen bilgiyi alıyoruz.
            std::io::stdin().read_line(&mut user_guess).unwrap();
            // Kelimedeki gereksiz boşlukları çıkartıp harfleri büyük harfe çeviren bir fonksiyonumuz var.
            // Kelimeyi o işlemden geçiriyoruz.
            user_guess = sanitize_word(&user_guess);
            // Kontrollerimiz basit. Kelime belirlediğimiz uzunlukta olmalı ve
            // programın kullandığı sözlükte yer almalı.
            // Eğer böyle değilse is_guess_valid değişkeni false olarak kalacak ve döngü devam edecek
            if user_guess.len() != WORD_LENGTH {
                println!(
                    "{}",
                    format!("{} uzunluğunda bir kelime girmelisin.", WORD_LENGTH).red()
                )
            } else if !self.available_words.iter().any(|word| word == &user_guess) {
                println!("{}", "Girdiğin kelime benim dükkanda bile yok :S".red())
            } else {
                self.guesses.push(user_guess.clone());
                is_guess_valid = true;
            }
        }
        // Kod buraya geldiyse geçerli bir tahmin elimizdedir. Bunu fonksiyondan geri dönüyoruz.
        user_guess
    }

    /*
       İhtiyacımız olan bir diğer fonksiyonda oyuncunun kazanıp kazanmadığının bulunması.
       Sonuçta bu da bir operasyon gerektirdiğinden main içerisinde tutmak yerine ayrı bir
       fonksiyona almak daha mantıklı.
    */
    /// Oyuncunun oyunu kazanıp kazanmadığını söyler
    pub fn is_it_over(&self, user_guess: &str) -> bool {
        // Oyuncunun kelimesi programın tuttuğu kelime ise tamam.
        let try_count = self.guesses.len();
        if user_guess == self.chosen_word {
            println!(
                "{}",
                format!("Kelimeyi {} denemede buldun. Tebrikler.", try_count).blue()
            );
            true
        } else if try_count >= TRY_COUNT {
            // Kelime doğru değilse deneme sayısını kontrol ediyoruz ve haklarımız tükendiyse
            // bunu üzülerek de olsa bildiriyoruz :P
            println!(
                "{}",
                format!(
                    "Malesef tüm hakların doldu. Doğru kelime -> {}",
                    self.chosen_word
                )
                .bright_green()
            );
            true
        } else {
            false
        }
    }
}

#[cfg(test)]
mod test {
    use crate::{sanitize_word, word_list, Manager};

    #[test]
    fn should_manager_crated_successfully() {
        let poe = Manager::new();
        assert_eq!(poe.chosen_word.chars().count(), 5);
        assert!(poe.available_words.len() > 0);
        assert!(poe.guesses.len() == 0);
    }

    #[test]
    fn should_sanitize_word_fn_works() {
        let word = "gol Dy   ";
        let result = sanitize_word(word);
        assert_eq!(result, "GOLDY");
    }

    #[test]
    fn should_world_list_fn_works() {
        let words = word_list();
        assert!(words.len() > 1);
        let count = words.iter().filter(|w| w.chars().count() != 5).count();
        assert_eq!(count, 0);
    }
}
```

Lib modülündeki enstrümanları mümkün mertebe yorum satırlarında anlatmaya çalıştım. Artık main fonksiyonumuzu geliştirebiliriz.

```rust
use colored::Colorize;
use wordle::Manager;

fn main() {
    let wellcome = "World oyununun klonuna hoş geldiniz.\nHaydi başlayalım.".yellow();
    println!("{}", wellcome);

    let mut poe = Manager::new();
    loop {
        poe.draw_board();
        let user_guess = poe.take_guess();
        if poe.is_it_over(&user_guess) {
            break;
        }
    }
}
```

Nispeten main fonksiyonu çok daha basit. Terminalden oynana bir oyunda olsa akışın belli koşullar altında sürekli yinelenen bir döngüye ihtiyacı olduğu aşikar. Döngü adımlarında oyun sahası ekrana çizilir, oyuncunun tahmini alınır ve oyunun sonlanıp sonlanmadığı kontrolü yapılır. Oldukça basit bir akış olduğunu söyleyebiliriz. Bu kadar zahmetten sonra artık oyunumuzu/oyununuzu deneyelim öyle değil mi?

Bende bir alışkanlık oldu o da clippy ile kodun kusurlarına bakmak. Bu nedenle uygulama kodlarını yazdıkça clippy ile nerelerde ideomatikliğin dışına çıktım kontrol etmekteyim. Ayrıca fonksiyonları inşa ettikçe testlerini koşturmakta da yarar var. Tüm bu aşamalardan sonra belki de run ile programı çalıştırıp oyunun keyfini çıkarmak lazım.

```bash
cargo clippy
cargo test
cargo run
```

Kendi ortamımda elde ettiğim sonuçları aşağıdaki ekran görüntüsü ile paylaşmak isterim. Her ne kadar buradaki kelimeyi bilemesemde sonraki denemelerde tutturduklarım olduğunu da ifade edebilirim. İç eğitimlerde buzları kırmak adına gayet güzel bir atıştırmalık oldu:)

![wordle_2.png](/assets/images/2022/wordle_2.png)

## Öğrendiğim Yeni Şeyler

Herbert'in adımlarını takip ederken hem var olan Rust bilgimi tekrar ettim hem de yeni şeyler öğrendim. İşte onlardan bazıları.

- rand yerine kullanılabilecek alternatif bir rastgele sayı üretme kütüphanesi bracket-random ile çalışmayı öğrendim.
- Terminal ekranını [colored](https://crates.io/crates/colored) küfesini (Crate) kullanarak renklendirebiliriz.
- include_str makro fonksiyonu ile bir dosya içeriğini src klasöründen okuyup derlenen kodun içerisine gömmek mümkün. Dosyayı build çıktılarındaki binary'ler ile birlikte taşımak mümkün hale geliyor.
- RandomNumberGenerator nesnesinin random_slice_entry fonksiyonu sayesinde parametre olarak verilen bir vector içinden rastgele bir eleman kolayca çekilebilir.
- for_each fonksiyonu ile metnin karakterlerini ya da vector elemanlarını dolaşırken tuple türünden yararlanarak indis ve değer çiftlerine ulaşabiliriz.

Böylece geldik bir rust pratiğimizin daha sonuna. Oyunu daha da geliştirmek elinizde. Örneğin daha önceden de belirttiğim üzere mesleki terimlerden oluşan sözlükleri kullanabiliriz. Öğrencilere ders çalışmalarında onlara yardımcı olacak veri depolarını da kullanabiliriz. Beş kelime sınırını kaldırıp farklı sayıda harf seçeneklerini belki bir giriş menüsü ile sunacağımız ayarlar kısmında oyuncuya seçtirebiliriz vs Tamamen sizin hayal gücünüze kalmış. Herbert'e sonsuz teşekkürler:) Yine harika bir iş çıkarmış. Kitabını da şiddetle tavsiye ederim. Yazıdaki örneğe ait kodlara her zaman olduğu gibi [github hesabımdan erişebilirsiniz](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/wordle). Tekrardan görüşünceye dek hepinize mutlu günler dilerim.