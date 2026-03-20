---
layout: post
title: "Rust ile Kodlama İdmanları - Başlangıç Seviyesi"
date: 2025-10-25 20:04:00 +0300
categories:
  - rust
tags:
  - rust
  - json
  - http
  - authentication
  - java
  - performance
  - generics
  - visual-studio
  - github
  - mutex
  - ownership
  - borrow-checker
---
Rust, daha çok öğrenme eğrisinin zorluğu ile tanınan bir sistem programlama dilidir desek sanırım yanlış olmaz. Ownership, borrow-checker, lifetimes, macro'lar, mutex vs derken managed ortamlarda (.Net, Java, Go gibi) geliştirme yapan programcıları epeyce zorlayan konu başlıklarına sahiptir. Şahsen, aynı öğrenme eğrisi zorluğunu yaşamış birisi olarak kodladıkça daha fazla tutulacağınız bir dil olduğunu da belirtmek isterim.

![rust_mini_00.png](/assets/images/2025/rust_mini_00.png)

Buna rağmen son dönemlerde özellikle github copilot gibi asistanlar veya kodlama üzerine uzmanlaşmış yapay zeka ajanları yaygın olarak kullanılmakta ve kod satırlarını otomatik olarak neredeyse tam da düşündüğümüz gibi tamamlamakta.

Çok basit bir örnek vererek devam edelim; Söz gelimi bir sayının faktöryelini hesaplayan metodu recursive modada yazacaksınız ama nasıl olduğunu pek de hatırlamıyorsunuz. Ya da öğrenmekte olduğunuz dilde bu size biraz tuhaf geliyor. Editöre, Todo başlıklı yorum satırını bırakın asistan sizin için tamamlasın. Biraz daha deneyimliyseniz ve memoization kullanmanın daha doğru olacağını düşündünüz. Onu da not olarak yazın asistan çat diye tamamlasın.

Bu yaklaşımın avantajları olduğu kadar dezavantajları olduğu da pekala aşikar. Öncelikle kod yazma pratikliğimizi olumsuz yönde etkileyebilir. Bir programlama diline iyi seviyede hakim birisi için kodun otomatik tamamlanması verimliliği artıran bir özellik olsa dahi zamanla "düşünerek kod yazma" yetkinliğini köreltebilir. Çünkü beyin bir süre sonra recursive faktöryel hesaplama kodunun Rust veya C# ile nasıl yazıldığını unutmaya başlayacaktır. Ancak bu benim kişisel fikrim zira bilimsel bir dayanağım yok. İnanıyorum ki yapay zeka araçları etrafımızı sarmış ve kodlamacının hayatını kolaylaştırmak adına önemli mesafe kat etmiş olsa da bu tip araçların yazdığı kodları denetleyebilmek, ideal olup olmadığına karar verebilmek kısacası Code Review'unu yapmak için de iyi seviyede bilgiye sahip olmamız gerektiğini düşünüyorum.

Bu nedenle rust dili ile ilgili kodlama bilgimizi pekiştirebileceğimiz ya da bir okusak da önemli noktaları hatırlasak diyebileceğimiz bir idman programı hazırlayabiliriz düşüncesindeyim. Bu amaçla bir süredir takip ettiğim bazı kaynaklardaki örnekleri çeşitli seviyelerde ayrıştırarak ilerlemeye karar verdim. İlk etapta başlangıç seviyesinden birkaç maddeyi ele alalım. Yazının devam eden kısmında konu başlıkları altına serpiştirilmiş örnek kod parçaları bulacaksınız. Eğer Visual Studio Code veya IntelliJ RustRover ile geliştirme yapacaksanız kod asistanlarını kapatarak ilerlemenizi öneririm. Bu ve devam yazısında kullandığım orijinal referanslar için kaynaklar bölümüne bakabilirsiniz.

## Unwrap/Expect Tuzaklarından Kaçınmak

Rust'ın güçlü yönlerinden birisi generic Option ve Result tipleri ile hata yönetimidir. Option tipi ile Some ve None kurguları oluşturarak mutlak değer dönüşü sağlayabiliriz. Rust dilinde null diye bir kavram olmadığını hatırlayalım. Result türü ise çok daha güçlüdür ve olası panik noktalarını kontrol altına almamızda bize yardımcı olur. Ancak özellikle deneysel kodlamalarda ya da birşeyler öğrenirken unwrap ve expect kullanarak ilerleriz zira match veya if let kullanarak kodu daha da uzatmak istemeyiz. Sonuçarın döneceği değerlerden eminizdir. Ancak bu yaklaşım üretim kodunda ciddi problemlere yol açabilir. O nedenle prensibe baştan sahip olmak ve o alışkanlığı kazanmak önemlidir.

Örneğin bir sistemin açılırken kritik bir yapılandırma dosyasını okumaya çalıştığını düşünelim. Birçok sistem çalışmak için ihtiyaç duyduğu parametreleri konfigurasyon dosyalarından, environment değişkenlerinden ya da uç senaryo vault gibi servislerden karşılıyor. Ancak burada basit anlamda sadece bir dosyadan okunduğunu varsayalım. Dosyanın bulunamaması veya okuma sırasında bir hata alınması halinde programın paniklemesi yerine kullanıcıya anlamlı bir hata mesajı döndürmek veya izlenebilir ve dolayısıyla tedbir alınabilir bir makine logu bırakmak daha sağlıklı olacaktır. Aşağıdaki örnek kod parçasında bu durum ele alınmakta ve hem kötü kodlama pratiği hem de ideal yöntem sunulmaktadır.

```rust
use std::fs;

// Kötü pratik: unwrap ve expect kullanımı
#[allow(dead_code)]
fn read_file(path: &str) -> String {
    fs::read_to_string(path).unwrap()
}

// İyi pratik: Hata yönetimi ile dosya okuma
fn read_file_safely(path: &str) -> Result<String, std::io::Error> {
    fs::read_to_string(path)
}

fn main() {
    // let content = read_file("appSettings.json");
    // println!("{}", content);

    match read_file_safely("appSettings.json") {
        Ok(content) => println!("{}", content),
        Err(e) => {
            if e.kind() == std::io::ErrorKind::NotFound {
                println!("Dosya bulunamadı: {}", e);
            } else {
                println!("Dosya okunurken bir hata oluştu: {}", e);
            }
        }
    }

    println!("Paniksiz günler dilerim!");
}
```

## ![rust_exc_00.png](/assets/images/2025/rust_exc_00.png)

## Gereksiz clone Çağrılarından Kaçınmak

Rust sahiplik (ownership) modeline göre Vector, String gibi heap bellek bölgesinde ele alınan veri yapıları kapsamlar (scopes) arasında taşınırken varsayılan olarak sahipliğin aktarımı söz konusudur. Eğer veri yapısı taşındığı fonksiyonda bir değişikliğe, başka bir deyişle mutasyona uğramayacaksa tüm veri yapısını klonlayarak göndermek yerine referans ile göndermek daha performanslı ve bellek dostu bir yaklaşımdır. Söz gelimi büyük bir sayı listesinin vektör veri yapısında ele alındığını ve matematiksel bir analiz fonksiyonu işleten bir metot tarafından kullanıldığını varsayalım. Analizi yapan fonksiyon veriyi değiştirmeyeceği için tüm vektörün klonlanması yerine referans ile gönderilmesi daha optimize bir çözüm olacaktır. Zira bu koca vektörün klonlanması bellek üzerinde maliyetli bir operasyondur. Aşağıdaki örnek kod parçasında bu durum ele alınıyor.

Kötü kotlama pratiğini ifade eden calculate_bad metodunda doğrudan vector kullanımı söz konusu. İlk kullanımda value moved here hatası alındığından clone çağrımına gidilmiştir. Oysa ki parametre olarak gelen vektor üzerinde hiçbir değişiklik yapılmayacaktır. Referans yolu ile kapsamlar arası transferi mümkündür.

```rust
// Kötü pratik: ownership alan fonksiyon kullanımı
#[allow(dead_code)]
fn calculate_bad(data: Vec<i32>) -> i32 {
    let sum: i32 = data.iter().sum();
    sum / (data.len() as i32)
}

// Tercih edilen pratik: referans ile veri geçme
fn calculate(data: &[i32]) -> i32 {
    let sum: i32 = data.iter().sum();
    sum / (data.len() as i32)
}

fn main() {
    /*
     Aşağıdaki kullanım value moved here hatası verir çünkü calculate fonksiyonu ownership'i alır ve data'yı kullanır.

     Sık yapılan çözümlerden birisi vektörü klonlamaktır ancak bu performans açısından maliyetlidir.
     Eğer veri değişmeyecekse, ownership almak yerine referans ile geçmek daha iyidir.

     error[E0382]: borrow of moved value: `numbers`
    --> exc01\src\main.rs:11:22
    |
    7  |     let numbers = vec![10, 20, 30, 40, 50];
    |         ------- move occurs because `numbers` has type `Vec<i32>`, which does not implement the `Copy` trait
    8  |     let result = calculate(numbers);
    |                            ------- value moved here
    ...
    11 |     println!("{:?}", numbers);
    |                      ^^^^^^^ value borrowed here after move
    |
    note: consider changing this parameter type in function `calculate` to borrow instead if owning the value isn't necessary
    --> exc01\src\main.rs:1:20
    |
    1  | fn calculate(data: Vec<i32>) -> i32 {
    |    ---------       ^^^^^^^^ this parameter takes ownership of the value
    |    |
    |    in this function
    = note: this error originates in the macro `$crate::format_args_nl` which comes from the expansion of the macro `println` (in Nightly builds, run with -Z macro-backtrace for more info)
    help: consider cloning the value if the performance cost is acceptable
    |
    8  |     let result = calculate(numbers.clone());
    |                                   ++++++++

    */
    let numbers = vec![10, 20, 30, 40, 50];

    // Bad practice: ownership alan fonksiyon kullanımı
    // // let result = calculate_bad(numbers);
    // let result = calculate_bad(numbers.clone()); // Performans maliyeti var
    // println!("Sonuç: {}", result);

    // println!("{:?}", numbers);

    // Good practice: referans ile veri geçme
    let result = calculate(&numbers);
    println!("Sonuç: {}", result);
    println!("{:?}", numbers);
}
```

value moved hatası;

![rust_exc_01_1.png](/assets/images/2025/rust_exc_01_1.png)

clone yerine referans kullanımı;

![rust_exc_01.png](/assets/images/2025/rust_exc_01.png)

## Mutasyon Kapsamını Sınırlamak

Rust programlama dilinde değişkenler varsayılan olarak immutable (değiştirilemez) olarak tanımlanır. Bir değişkenin atıfta bulunduğu veri değerini değiştirmek istediğimizde mut anahtar kelimesi ile değişkeni mutable (değiştirilebilir) olarak tanımlamamız gerekir. Mutasyonu mümkün olan en dar kapsamda kullanmak kod okunurluğu ve güvenliğini artıran bir pratik olarak değerlendirilmektedir.

Aşağıdaki kod parçasında bileşik faiz hesaplaması yapan bir muhasebe fonksiyonu bulunmaktadır. Bu fonksiyondaki döngü içinde güncellenen belli değişkenler vardır (currentamount ve totalinterest) Bu değişken değerleri sadece döngü içinde güncellenir ve hesaplama için ihtiyaç duyulan ara değerler (bu örnekte sadece yearlyinterest) değiştirilemez (immutable) kullanılabilir. Rust'ın değişkenleri varsayılan olarak immutable kabul etmesinin bir sebebini de, mutasyonları mümkün mertebe dar kapsamda ele almak istemesi olarak düşünebiliriz.

```rust
fn calculate_compound_interest(principal: f64, annual_rate: f64, years: u32) -> f64 {
    let mut current_amount = principal;
    let mut total_interest = 0.0;

    for year in 1..=years {
        let yearly_interest = current_amount * annual_rate / 100.0;
        current_amount += yearly_interest;
        total_interest += yearly_interest;
        
        println!("Year {}: Interest earned: {:.2}, Total amount: {:.2}", 
                 year, yearly_interest, current_amount);
    }

    total_interest
}

fn main() {
    let principal = 1000.0;
    let annual_rate = 4.5;
    let years = 3;

    let total_interest = calculate_compound_interest(principal, annual_rate, years);
    let final_amount = principal + total_interest;
    
    println!("\nSummary:");
    println!("Principal amount: {:.2}", principal);
    println!("Annual interest rate: {:.1}%", annual_rate);
    println!("Time period: {} years", years);
    println!("Total compound interest earned: {:.2}", total_interest);
    println!("Final amount: {:.2}", final_amount);
}
```

![rust_exc_02.png](/assets/images/2025/rust_exc_02.png)

## Dangling Referanslardan Kaçınmak

Rust'ın güçlü sahiplik (ownership) ve borçlanma (borrowing) modeli, dangling (Sarkmış) referansların oluşmasını derleme zamanında engeller. Dangling referanslar, bir değişken kapsam dışına çıktıktan sonra dahi ona erişmeye çalıştığımızda ortaya çıkar ve bu durum bellek güvenliği sorunlarına yol açar. Zira bellekten düşürdüğümüz bir veri bütünün bir parçası halen daha referans edilebilir ve kötü niyetli kişilerin erişimine açık pozisyonda kalabilir. Rust, bu tür hataların oluşmasını önlemek için katı kurallar uygular. Borrow Checker prensiplerine göre bir referansın atıfta bulunduğu değerden daha uzun süre yaşaması da mümkün değildir. Aslında Dangling (Sarkmış) referanslar genelde bir fonksiyonun local bir değere referans döndürmeye çalışması sırasında ortaya çıkan kritik bir bellek güvenliği hatasıdır.

N sayıda cümleyi literal string olarak tutan bir dizideki en uzun cümleyi bulmaya çalışan bir fonksiyon yazdığımızı düşünelim. En uzun cümleyi referans olarak döndürmeye çalışırsak, fonksiyonun kapsamı sona erdiğinde taşınan dizinin bellekten silinmesiyle birlikte döndürdüğümüz referansın geçersiz hale gelmesi söz konusu olur ve sorunu çözmek için karmaşık lifetime annotasyonları kullanmamız gerekir. Bunun yerine en uzun cümleyi sahiplenen bir String değişkeni fonksiyondan geriye döndürmek daha doğru bir yaklaşımdır.

Aşağıdaki kod parçasında bu senaryo ele alınmaktadır. Esasında ne kadar uğraşırsak uğraşalım zaten dangling referans oluşmayacaktır. Zira ısrarla &str döndürmek istediğimizde Rust bunu derleme zamanında anlayıp dönüş referansına ait yaşam ömrünü kontrol etmemizi isteyen bir hata mesajı yayınlayacaktır. Dolayısıyla &str kullanmaya lifetime annotation kullanımı ile devam da edebiliriz. Lakin maliyet çok yüksek değilse doğrudan bir String döndürmek kod okunurluğunu artırmak, çok sayıda katmandan oluşan daha büyük bir kod tabanında lifetime karmaşıklığı ile uğraşmamak adına daha iyidir.

```rust
// // Kötü pratik: Dangling referans sorunu oluşması ve lifetime kullanma gerekliliği
// fn find_longest_sentence_badly(lines: &[&str]) -> &str {
//     let mut longest: &str = "";
//     for &line in lines {
//         if line.len() > longest.len() {
//             longest = line;
//         }
//     }
//     longest
// }

// Doğru pratik: String döndürme
fn find_longest_sentence_safely(lines: &[&str]) -> String {
    let mut longest = String::new();
    for line in lines {
        if line.len() > longest.len() {
            longest = line.to_string();
        }
    }
    longest
}

fn main() {
    let lines = vec![
        "Rust is a systems programming language.",
        "It is designed for performance and safety.",
        "Ownership and borrowing are key concepts in Rust.",
    ];

    /*
    Bu fonksiyon dangling referans hatasına neden olur ve ayrıca derleme zamanında 'expected named lifetime parameter' hatası verir.
    Sorunu çözmek için fonksiyon imzasına yaşam süresi parametreleri eklemek gerekir.
    Bunun yerine en uzun cümleyi String olarak döndürmek daha güvenlidir.

    error[E0106]: missing lifetime specifier
    --> exc03\src\main.rs:2:51
    |
    2 | fn find_longest_sentence_badly(lines: &[&str]) -> &str {
    |                                       -------     ^ expected named lifetime parameter
    |
    = help: this function's return type contains a borrowed value, but the signature does not say which one of `lines`'s 2 lifetimes it is borrowed from
    help: consider introducing a named lifetime parameter
    |
    2 | fn find_longest_sentence_badly<'a>(lines: &'a [&'a str]) -> &'a str {
    |                               ++++         ++   ++           ++

    For more information about this error, try `rustc --explain E0106`.

    */

    // let longest_sentence = find_longest_sentence_badly(&lines);
    // println!("En uzun cümle (kötü pratik): {}", longest_sentence);

    let longest_sentence = find_longest_sentence_safely(&lines);
    println!("En uzun cümle (iyi pratik): {}", longest_sentence);
}
```

lifetime hatası;

![rust_exc_03.png](/assets/images/2025/rust_exc_03.png)

String döndürdüğümüz senaryo;

![rust_exc_03_1.png](/assets/images/2025/rust_exc_03_1.png)

## Public API'lerde Kapsamlı Dokümantasyon Kullanmak

Rust'ın güçlü yanlarından birisi de markdown formatını baz alan zengin yardım dokümantasyonu desteğidir. Özellikle public API olarak ifade edebileceğimiz genel açık her tür kütüphane geliştirirken kapsamlı dokümantasyon kullanmak, diğer geliştiricilerin fonksiyonların nasıl kullanılacağını ve ne işe yaradığını anlamalarına yardımcı olur. Özellikle pub erişim belirleyicisi ile işaretlenmiş tüm enstrümanlarda zengin dokümantasyon yorumları kullanmak gerekir. Dokümantasyon kendi deneysel projelerimizde de önemli bir pratiktir. Zira kodun ne yaptığının belli standartlar üzerine oturtulmuş titiz bir anlatımıdır.

```rust
/// Verilen bir fonksiyonun türevini yaklaşık olarak hesaplar.
///
/// # Argümanlar
/// * `f` - Türevini almak istediğimiz fonksiyon.
/// * `x` - Türevini hesaplamak istediğimiz nokta.
/// * `h` - Küçük bir değer, türev hesaplamasında kullanılır (varsayılan: 1e-7).
/// # Dönüş Değeri
/// * `f` fonksiyonunun `x` noktasındaki yaklaşık türevi.
pub fn derivative<F>(f: F, x: f64, h: f64) -> f64
where
    F: Fn(f64) -> f64,
{
    (f(x + h) - f(x - h)) / (2.0 * h)
}

/// Verilen bir fonksiyonun belirli bir aralıktaki integralini yaklaşık olarak hesaplar.
///
/// # Argümanlar
/// * `f` - İntegralini almak istediğimiz fonksiyon.
/// * `a` - İntegral başlangıç noktası.
/// * `b` - İntegral bitiş noktası.
/// * `n` - İntegral hesaplamasında kullanılacak dikdörtgen sayısı (varsayılan: 1000).
/// # Dönüş Değeri
/// * `f` fonksiyonunun `[a, b]` aralığındaki yaklaşık integrali.
pub fn integral<F>(f: F, a: f64, b: f64, n: usize) -> f64
where
    F: Fn(f64) -> f64,
{
    let width = (b - a) / (n as f64);
    let mut total_area = 0.0;

    for i in 0..n {
        let x = a + (i as f64 + 0.5) * width;
        total_area += f(x) * width;
    }

    total_area
}

#[cfg(test)]
pub mod tests {
    use super::*;

    #[test]
    fn test_derivative() {
        let f = |x: f64| x.powi(2);
        let deriv_at_3 = derivative(f, 3.0, 1e-7);
        assert!((deriv_at_3 - 6.0).abs() < 1e-5);
    }

    #[test]
    fn test_integral() {
        let f = |x: f64| x;
        let integral_result = integral(f, 0.0, 1.0, 1000);
        assert!((integral_result - 0.5).abs() < 1e-5);
    }
}
```

ve modül içinde aşağıdaki gibi ilerlenebilir.

```rust
//! # Calculus Modülü
//!
//! Bu modül, temel matematiksel işlemleri gerçekleştiren fonksiyonlar içerir.
//! Örnek olarak, türev ve integral hesaplamaları için fonksiyonlar sağlar.
//!
//! # Örnekler
//! ```rust
//! mod calculus;
//!
//! use calculus::{derivative, integral};
//! fn main() {
//!   let f = |x: f64| x.powi(2);
//!   let deriv_at_3 = derivative(f, 3.0, 1e-7);
//!   println!("f'(3) yaklaşık olarak: {}", deriv_at_3); // Yaklaşık 6.0
//!   let integral_result = integral(f, 0.0, 1.0, 1000);
//!   println!("∫f(x)dx from 0 to 1 yaklaşık olarak: {}", integral_result); // Yaklaşık 0.3333
//! }
//! ```

pub mod calculus;
```

Komut satırından cargo doc komutu sonrası oluşan dokümantasyon içeriğini kontrol ettiğimizde daha profesyonel bir içerik oluştuğunu görebiliriz.

Modül tarafı;

![rust_exc_04_1.png](/assets/images/2025/rust_exc_04_1.png)

Örnek fonksiyon tarafı;

![rust_exc_04_2.png](/assets/images/2025/rust_exc_04_2.png)

veya örneğin RustRover IDE'sinde kod yazarken ve bu modüle ati bir fonksiyonu kullanırken;

![rust_exc_04_3.png](/assets/images/2025/rust_exc_04_3.png)

## Sahipliği Gözardı Etmek (Ignoring Ownership)

Rust'ın sahiplik (ownership) sisteminin bir dizi kuralı vardır. Bunlardan birisi de bir değerin yalnızca bir sahibinin olabileceğidir. Sahipliği alınan bir değer kapsam dışına çıktığında bellekten silinir (drop). Başka bir değişkene atama yaptığımızda ise verinin sahipliği aktarılır ve bu durumda da orjinal değişken kullanılmaz hale gelir. Ancak bazı durumlarda sahipliği göz ardı etmek mümkündür. Bunu daha çok farklı kapsamlara (scope) veri taşıyan değişkenler kullandığımızda ele alırız.

Söz gelimi bir web sunucusuna gelen istekleri işlerken HTTP Body içeriğini temsil eden bir String nesnesini, bir doğrulama fonksiyonuna geçirdikten sonra orjinal değişkeni de kullanmaya devam etmek istediğimizi düşünelim. Bu durumda sahipliği göz ardı ederek veriyi referans yoluyla geçmek en doğru ve maliyetsiz yaklaşım olacaktır. Aşağıdaki örnekte kod parçasında bu durum hem sahipliği devralan hem de sahipliği göz ardı eden iki fonksiyonla ele alınmaktadır.

```rust
// Sahipliği devralan fonksiyon
fn validate_with_ownership(input: String) -> bool {
    // Basit bir doğrulama: Şimdilik gelen veri içeriği boş değilse geçerli kabul ediyoruz
    !input.trim().is_empty()
    // input değişkeni fonksiyonun sonunda scope dışına çıktığında bellekten otomatik olarak temizlenecektir
}

// Sahipliği göz ardı eden fonksiyon
fn validate_without_ownership(input: &str) -> bool {
    // Basit bir doğrulama: Şimdilik gelen veri içeriği boş değilse geçerli kabul ediyoruz
    !input.trim().is_empty()
}

fn main() {
    let user_input = String::from("<body><title>Request Form</title></body>");

    // Fonksiyona sahipliği devretmiyoruz, sadece referansını geçiriyoruz
    let is_valid = validate_without_ownership(&user_input);

    if is_valid {
        println!("Request is valid: {}", user_input);
    } else {
        println!("Invalid request.");
    }

    // user_input bu scope içerisinde hala kullanılabilir durumda çünkü sahipliği ilgili fonksiyonuna geçmedik
    println!("Original input is still available: {}", user_input);

    /*
        Aşağıdaki kullanımda owned_input değişkeninin sahipliği validate_with_ownership fonksiyonuna
        devredildiği için, fonksiyon çağrısından sonra owned_input değişkeni geçersiz hale gelir.
        Bu nedenle, fonksiyon çağrısından sonra owned_input değişkenine erişmeye çalışmak
        derleme hatasına neden olur.

        error[E0382]: borrow of moved value: `owned_input`
        --> exc15\src\main.rs:35:23
        |
        27 |     let owned_input = String::from("<body><title>Owned Request Form</title></body>");
        |         ----------- move occurs because `owned_input` has type `String`, which does not implement the `Copy` trait
        28 |     // Fonksiyona sahipliği devrediyoruz
        29 |     let is_owned_valid = validate_with_ownership(owned_input);
        |                                                  ----------- value moved here
        ...
        35 |     let body_length = owned_input.len(); // Hata: owned_input artık geçerli değil
        |                       ^^^^^^^^^^^ value borrowed here after move
        |
        note: consider changing this parameter type in function `validate_with_ownership` to borrow instead if owning the value isn't necessary
        --> exc15\src\main.rs:1:35
        |
        1  | fn validate_with_ownership(input: String) -> bool {
        |    -----------------------        ^^^^^^ this parameter takes ownership of the value
        |    |
        |    in this function
        help: consider cloning the value if the performance cost is acceptable
        |
        29 |     let is_owned_valid = validate_with_ownership(owned_input.clone());
        |                                                             ++++++++

        For more information about this error, try `rustc --explain E0382`.
        warning: `exc15` (bin "exc15") generated 1 warning

        Burada fonksiyona referans yolu ile sahipliği devrederek ilerlemek daha güvenlidir.
        Ya da maliyetine katlanarak klonlama (clone) yapabiliriz.
        Hatta çağırılan fonksiyondan geriye yeni bir String dönerek sahipliği koruyabiliriz. 
        Ancak bu senaryoda ideal olan referans ile geçiş yapmaktır.
    */
    // let owned_input = String::from("<body><title>Owned Request Form</title></body>");
    // // Fonksiyona sahipliği devrediyoruz
    // let is_owned_valid = validate_with_ownership(owned_input);
    // if is_owned_valid {
    //     println!("Owned request is valid.");
    // } else {
    //     println!("Invalid owned request.");
    // }
    // let body_length = owned_input.len(); // Hata: owned_input artık geçerli değil
}
```

Sahipliği fonksiyona devrettikten sonra halen değişkeni kullanmaya devam etmek istediğimizde;

![rust_exc_05.png](/assets/images/2025/rust_exc_05.png)

Referans yoluyla sahipliği aktardığımızda;

![rust_exc_06.png](/assets/images/2025/rust_exc_06.png)

## Makroları Hatalı Kullanmaktan Kaçınmak

Makrolar metadata programlamada oldukça işimize yarayan rust'ın güçlü enstrümanlarından birisidir. Makroları kullanarak kod üreten kodlar yazabilir, derleme sırasında kodu değiştirebiliriz. Genellikle tekrarlı işler için bu makro kullanımı çok yaygındır. Hatta Rust'ı öğrenmeye başladığımız andan itibaren ilk makromuzu da kullanırız (println!) Bilindiği üzere! işareti ile biten metotlar birer makrodur.

Ancak makroların yanlış kullanımı kodun okunurluğunu ve bakımını zorlaştırabilir. Mesela çok basit görevler için makro kullanmak yerine fonksiyonlardan yararlanmak daha doğrudur. Bu sayede kodun anlaşılması ve hataların ayıklanması daha kolay olur. Örneğin basit loglama operasyonlarında makro kullanmak yerine fonksiyon kullanımı tercih edilebilir. Aşağıdaki kod parçasında kötü ve ideal kullanım örnekleri basitçe ele alınmaktadır.

```rust
/*
    Log bırakmak için makro kullanmak yerine fonksiyon kullanmak kodun okunurluğunu daha da basitleştirir.
    Bir makroda genellikle expression ve çeşitli regex patternler kullanılır. Bu da kodun anlaşılmasını zorlaştırabilir.
    Özellikle basit işlemler için makro kullanmak yerine fonksiyon kullanmak çok daha kolaydır.
*/
macro_rules! log {
    ($msg:expr, $level:expr) => {
        println!("[{}]: {}", $level, $msg);
    };
}

/// Basit bir log fonksiyonu. 
/// Mesajı, log seviyesini alır ve formatlı bir şekilde ekrana basar.
///
/// # Arguments
/// * `message` - Log mesajı.
/// * `level` - Log seviyesi (örneğin: "INFO", "WARN", "ERROR").
fn log(message: &str, level: &str) {
    println!("[{}]: {}", level, message);
}

fn main() {
    log!("This is a warning message.", "WARN");

    log("This is an info message.", "INFO");
    log("This is an error message.", "ERROR");
}
```

![Ekim_rust_exc_07.png](/assets/images/2025/Ekim_rust_exc_07.png)

![Ekim_rust_exc_07.png](/assets/images/2025/Ekim_rust_exc_07.png)

## String Yerine &str ile Çalışmak

Programlar belleğin stack ve heap bölgelerini kullanarak çalışırlar. Heap bellek bölgesi çok daha büyüktür ve rastgele okuma/yazma işlemleri sıklıkla gerçekleşir. Maliyet açısından bakıldığında en külfetli operasyonlar heap bölgesinde icra edilir (Yer tahsis işlemleri, veri taşıma operasyonları, serbest bırakmalar vb.) Özellikle veri okuma operasyonlarında heap allocation maliyetini minimize etmek için referanslarla çalışmak tercih edilen bir yaklaşımdır. Bir başka deyişle bu operasyonlarda ödünç alınabilen &str referansları kullanmak performans açısından daha iyidir.

&str, literal string verilerini temsil eden bir referanstır ve heap üzerinde yeni bir String nesnesi oluşturmaya gerek kalmadan veri okuma işlemlerini mümkün kılar. Tabii burada veri üzerinde değişiklik yapmayacağımızı kabul ettiğimizi varsayıyorum. Yani sahipliğin devredilmesi veya verinin değiştirilmesi gereken durumlarda yine String türü ile çalışmak gerekir.

Bir web suncusuna gelen isteklerin yönlendirilmesi ile ilgili bir kod parçası geliştirdiğimizi düşünelim. HTTP isteklerine ait path bilgilerini ele alırken, verinin kopyası üzerinden ilerlemek yerine referans kullanarak ilerlemek daha az bellek tüketimi sağlayacaktır zira gereksiz yer tahsisi operasyonuna (heap allocation) gerek kalmaz.

Aşağıdaki örnek kod parçasında bu senaryo basit bir şekilde ele alınmaktadır.

```rust
fn main() {
    let api_paths = vec![
        String::from("/api/v1/users"),
        String::from("/api/v1/orders"),
        String::from("/api/v1/products"),
    ];

    for path in api_paths {
        // // Bad Practice
        // route_request_owned(path.clone());

        // Good Practice
        route_request(&path);
    }
}

// Bad Practice: Kopya üzerinden işlem yapmak
#[allow(dead_code)]
fn route_request_owned(path: String) {
    match path.as_str() {
        "/api/v1/users" => println!("Routing to Users API"),
        "/api/v1/orders" => println!("Routing to Orders API"),
        "/api/v1/products" => println!("Routing to Products API"),
        _ => println!("404 Not Found"),
    }
}

// Good Practice: Referans üzerinden işlem yapmak
fn route_request(path: &str) {
    match path {
        "/api/v1/users" => println!("Routing to Users API"),
        "/api/v1/orders" => println!("Routing to Orders API"),
        "/api/v1/products" => println!("Routing to Products API"),
        _ => println!("404 Not Found"),
    }
}
```

Dikkat edileceği üzere apipaths dizisindeki her bir yol bilgisi için routerequest fonksiyonu çağrılırken bir referans türü olarak &str kullanılmıştır. Yine de ısrarla kopya üzerinden işlem yapmak istersek clone metodu ile kopyalama yapılarak ilerlenebilir ancak bu durumda da performans maliyeti ortaya çıkar. Çünkü her bir kopyalama işlemi için heap üzerinde yeni bir alan tahsis edilir ve bu da gereksiz bellek tüketimi demektir. Referans kullanımı ise bu maliyeti ortadan kaldırır.

![rust_exc_08.png](/assets/images/2025/rust_exc_08.png)

## if let ile Daha Temiz Eşleşmeler

Bir match ifadesinin tek bir varyantının ele alındığı durumlarda daha kısa ve temiz bir sözdizimi olarak if let kullanımı tercih edilebilir zira kod okunurluğu artar. if let ifadelerini de Option, Result veya enum türleri ile kullanmak mümkündür. Söz gelimi doğrulanmış (Authenticated) bir kullanıcının sisteme girdikten sonra profil bilgilerini almak istediğimizi düşünelim. Kullanıcının profil bilgileri doğrulanmışsa bu bilgileri ekrana basmak aksi durumda bir hata mesajı göstermek istiyoruz. Bu durumda if let kullanımı match ifadesine göre daha kısa ve anlaşılır olacaktır. if let daha çok tek bir durumu ele almak istediğimiz senaryolarda gerçekten idealdir. Aşağıdaki örnek kod parçasında match ve if let kullanımları karşılaştırılmaktadır.

```rust
/// Doğrulanmış ve doğrulanmamış kullanıcıları temsil eden bir enum tanımı
enum AuthenticatedUser {
    /// Doğrulanmış kullanıcı bilgilerini tutar
    Verified { username: String, email: String },
    /// Doğrulanmamış kullanıcı bilgisini temsil eder
    Unverified,
}

/// Kullanıcı bilgilerini temsil eden bir yapı
struct User {
    /// Kullanıcı adı
    username: String,
    /// Kullanıcı e-posta adresi
    email: String,
}

/// Kullanıcıyı doğrulayan bir fonksiyon
/// Eğer kullanıcı adı veya e-posta boş ise None döner.
/// E-posta "@" karakterini içeriyorsa Verified, içermiyorsa Unverified döner.
///
/// # Arguments
/// * `user` - Doğrulanacak kullanıcı bilgilerini içeren referans
/// # Returns
/// * `Option<AuthenticatedUser>` - Doğrulama sonucunu içeren enum
fn authenticate(user: &User) -> Option<AuthenticatedUser> {
    /*
    Çok basit birkaç doğrulama işlemi gerçekleştiriyoruz.
    Bir gerçek hayat senaryosunda elbetteki daha karmaşık doğrulama işlemleri yapılması gerekir.
    Örneğin, e-posta adresinin geçerliliğini kontrol etmek için regex kullanılabilir veya
    kullanıcı adı belirli kurallara göre doğrulanabilir.

    Bu da birden fazla enum varyantının ele alınması anlamına gelir.
    Eğer kodda tek varyantla ilgileniyorsak, match ifadesi kullanmak yerine if let kullanımı daha temiz ve okunabilir olur.
    */
    if user.username.is_empty() || user.email.is_empty() {
        return None;
    }

    if user.email.contains("@") {
        Some(AuthenticatedUser::Verified {
            username: user.username.clone(),
            email: user.email.clone(),
        })
    } else {
        Some(AuthenticatedUser::Unverified)
    }
}

fn main() {
    let user = User {
        username: "john_doe".to_string(),
        email: "john_doe@example.com".to_string(),
    };

    let auth_user = authenticate(&user);

    // Bad Practice: match ifadesi kullanımında tüm durumları ele almak zorundayız
    match auth_user {
        Some(AuthenticatedUser::Verified { username, email }) => {
            println!("Username: {}, Email: {}", username, email);
        }
        Some(AuthenticatedUser::Unverified) => {
            println!("User is unverified.");
        }
        _ => {
            println!("Authentication failed.");
        }
    }

    let user = User {
        username: "jessica".to_string(),
        email: "jessica@example.com".to_string(),
    };
    let auth_user = authenticate(&user);

    // Good Practice: if let kullanımı
    /*
    Sadece Verified durumunu ele almak istediğimiz bir senaryoda match ifadesi kullandığımız için tüm
    durumları kontrol etmek zorunda kalıyoruz. Bu da kodun gereksiz yere karmaşıklaşmasına neden oluyor.
    if let kullanımı ile sadece ilgilendiğimiz durumu ele alabiliriz ve kod daha temiz ve okunabilir olur.
    */
    if let Some(AuthenticatedUser::Verified { username, email }) = auth_user {
        println!("Username: {}, Email: {}", username, email);
    } else {
        println!("User is unverified.");
    }

    /*
    Aşağıdaki kullanımda sadece None durumunu ele alıyoruz.
    Diğer durumlarla ilgilenmiyoruz. Bu durumda match ifadesi yerine if let kullanımı daha temiz ve okunabilir olur.

    Lakin buna cargo clippy redundant pattern matching, consider using `is_none()` uyarısı verir.
    is_none() kullanımı daha da temiz ve okunabilirdir.
    */
    let user = User {
        username: "".to_string(),
        email: "".to_string(),
    };
    let auth_user = authenticate(&user);
    // if let None = auth_user {
    //     println!("Authentication failed.");
    // }

    if auth_user.is_none() {
        println!("Authentication failed.");
    }
}
```

![rust_exc_09.png](/assets/images/2025/rust_exc_09.png)

Şimdilik bu kadar...

Bu pratik kod örneklerini deneyerek temel rust bilgilerimizden bazılarını yeniden hatırlayabiliriz. İlerleyen yazılarda farklı seviyelerden örneklere de yer vermeye çalışacağım. [Bu bölümde yer alan kod parçalarına github reposu üzerinden de erişebilirsiniz](https://github.com/buraksenyurt/friday-night-programmer/tree/main/src/rust-exercises). Ayrıca [Ferris logosunu sevdiyseniz Maria Letta'nın reposunda daha fazlasını da bulabilirsiniz](https://github.com/MariaLetta/free-ferris-pack);)

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
