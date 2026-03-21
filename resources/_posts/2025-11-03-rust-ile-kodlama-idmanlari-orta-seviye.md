---
layout: post
title: "Rust ile Kodlama İdmanları - Orta Seviye"
date: 2025-11-03 07:28:00 +0300
categories:
  - rust
tags:
  - rust
  - rust-lang
---
Şunu fark ettim ki, hangi programlama dili olursa olsun bilgilerimizi taze tutmanın yollarından birisi öğrenilenleri düzenli olarak not haline getirmek ve yazarak kayıt altına almak. Elbette tek yol bu değil. Mutlaka her gün bir parça da olsa kod yazmak, belki bir proje üzerinden ilerlemek (hiçbir yerde kullanılmayacak olsa bile), onunla ilgili bir makale okumak veya bir video izlemek lazım. Ünlü düşünür Johann Wolfgang von Goethe ne demiş "İnsan her gün bir parça müzik dinlemeli, iyi bir şiir okumalı, güzel bir tablo görmeli ve mümkünse birkaç mantıklı cümle söylemelidir." Yapay zekanın bilgisayar ve internet'ten sonra yeni bir devrim olma çabasıyla koştuğu şu dönemde iyi bir programcı olmak için daha çok okuyalım, daha çok pratik yapalım, daha çok araştıralım, daha çok dinleyelim derim. Elbette yapay zeka araçlarına sırtımızı da dönmeyelim. Nimetlerinden, verimliliğimizi artıracak ölçüde yararlanalım.

![ferris_mini_11.png](/assets/images/2025/ferris_mini_11.png)

Bir önceki yazımızda Rust programlama dili için başlangıç seviyesinde değerlendirebileceğimiz örnek kodlara değinmiştik. Bu yazımızda ise orta seviye konulara ait örneklere yer vermeye gayret edeceğiz. Composition'dan kapsamlı test senaryolarına, lazy iterator kullanımından generic türlerde kısıtlamaları kullanmaya kadar farklı konu başlıklarında kısa ve hatırlatıcı bilgileri örnek kodlar ile ele alacağız. Dilerseniz vakit kaybetmeden ilk konumuzla başlayalım.

## Composition Over Inheritance ile Daha Modüler Tasarım

Rust nesne yönelimli programlama (Object Oriented Programming) paradigmalarını tam olarak destekler mi desteklemez mi veya buna ihtiyacı var mıdır bilinmez ancak birçok tasarım kalıbı uygulanabilir. Hatta temel prensiplerden birisi olan Composition over Inheritance oldukça ön plana çıkar. Bevy gibi ECS (Entity Component System) tabanlı oyun motorları bu prensibi temel alarak geliştirilmiştir. Bir nesnenin ihtiyaç duyduğu özellik ve davranışların başka nesnelerden kalıtım yoluyla alınmasından ziyade bileşenler (components) aracılığıyla alınması tercih edilir. Bu yaklaşım kodun daha esnek, yeniden kullanılabilir ve test edilebilir olmasını sağlar.

Bir yazılım sistemindeki kullanıcıları temsil edecek bir yapı geliştirmeye çalıştığımızı düşünelim. Kullanıcı ile ilgili tüm bilgileri tek bir God Object içinde toplamak yerine, kullanıcıya ait farklı özellikleri ve davranışları ayrı bileşenler olarak tasarlayıp, kullanıcı yapısını ona bu bileşenleri ekleyerek oluşturmak daha esnek bir tasarım sağlar. Bu amaçla aşağıdaki örnek kod parçasını değerlendirebiliriz.

```rust
fn main() {
    let personal_info = PersonalInfo::new("John".to_string(), "Doe".to_string(), 25);
    let contact_info = ContactInfo::new("john.doe@nowhere.com".to_string());
    let activity_status = ActivityStatus::new(true, 120120044543);
    let gaming_info = GamingInfo::new(7);

    let user = User::new(personal_info, contact_info, activity_status, gaming_info);

    println!("User: {}", user.get_full_name());
    println!("Email: {}", user.get_email());
    println!("Active: {}", user.is_active());
    println!("Level: {}", user.get_level());

    let mut mutable_user = user;
    mutable_user.set_active(true);
    mutable_user.level_up();

    println!("New level: {}", mutable_user.get_level());
}

// Bad Practice: God Object - Tüm bilgileri tek bir struct'ta toplamak
#[allow(dead_code)]
struct BadUser {
    first_name: String,
    last_name: String,
    age: u8,
    email: String,
    is_active: bool,
    last_activity_timestamp: u64,
    level: u8,
}

// Good Practice: Composition over Inheritance - Farklı sorumlulukları ayrı bileşenlerde tutmak
#[derive(Debug, Clone)]
struct PersonalInfo {
    first_name: String,
    last_name: String,
    age: u8,
}

impl PersonalInfo {
    fn new(first_name: String, last_name: String, age: u8) -> Self {
        Self {
            first_name,
            last_name,
            age,
        }
    }

    fn get_full_name(&self) -> String {
        format!("{} {}", self.first_name, self.last_name)
    }

    fn get_age(&self) -> u8 {
        self.age
    }
}

#[derive(Debug, Clone)]
struct ContactInfo {
    email: String,
}

impl ContactInfo {
    fn new(email: String) -> Self {
        Self { email }
    }

    fn get_email(&self) -> &str {
        &self.email
    }

    fn update_email(&mut self, new_email: String) {
        self.email = new_email;
    }
}

#[derive(Debug, Clone)]
struct ActivityStatus {
    is_active: bool,
    last_activity_timestamp: u64,
}

impl ActivityStatus {
    fn new(is_active: bool, last_activity_timestamp: u64) -> Self {
        Self {
            is_active,
            last_activity_timestamp,
        }
    }

    fn is_active(&self) -> bool {
        self.is_active
    }

    fn set_active(&mut self, active: bool) {
        self.is_active = active;
        if active {
            self.last_activity_timestamp = std::time::SystemTime::now()
                .duration_since(std::time::UNIX_EPOCH)
                .unwrap()
                .as_secs();
        }
    }

    fn get_last_activity(&self) -> u64 {
        self.last_activity_timestamp
    }
}

#[derive(Debug, Clone)]
struct GamingInfo {
    level: u8,
}

impl GamingInfo {
    fn new(level: u8) -> Self {
        Self { level }
    }

    fn get_level(&self) -> u8 {
        self.level
    }

    fn level_up(&mut self) {
        if self.level < u8::MAX {
            self.level += 1;
        }
    }

    fn set_level(&mut self, level: u8) {
        self.level = level;
    }
}

#[derive(Debug, Clone)]
struct User {
    personal_info: PersonalInfo,
    contact_info: ContactInfo,
    activity_status: ActivityStatus,
    gaming_info: GamingInfo,
}

#[allow(dead_code)]
impl User {
    fn new(
        personal_info: PersonalInfo,
        contact_info: ContactInfo,
        activity_status: ActivityStatus,
        gaming_info: GamingInfo,
    ) -> Self {
        Self {
            personal_info,
            contact_info,
            activity_status,
            gaming_info,
        }
    }

    fn get_full_name(&self) -> String {
        self.personal_info.get_full_name()
    }

    fn get_age(&self) -> u8 {
        self.personal_info.get_age()
    }

    fn get_email(&self) -> &str {
        self.contact_info.get_email()
    }

    fn update_email(&mut self, new_email: String) {
        self.contact_info.update_email(new_email);
    }

    fn is_active(&self) -> bool {
        self.activity_status.is_active()
    }

    fn set_active(&mut self, active: bool) {
        self.activity_status.set_active(active);
    }

    fn get_last_activity(&self) -> u64 {
        self.activity_status.get_last_activity()
    }

    fn get_level(&self) -> u8 {
        self.gaming_info.get_level()
    }

    fn level_up(&mut self) {
        self.gaming_info.level_up();
    }

    fn set_level(&mut self, level: u8) {
        self.gaming_info.set_level(level);
    }

    fn get_user_summary(&self) -> String {
        format!(
            "User: {} ({}), Email: {}, Active: {}, Level: {}",
            self.get_full_name(),
            self.get_age(),
            self.get_email(),
            self.is_active(),
            self.get_level()
        )
    }
}
```

Bu örnekte doğrudan bir kalıtım kullanımı söz konusu değildir ancak User veri yapısının tasarımına dikkat edilmelidir. Personel, iletişim, aktivite ve oyun bilgileri ayrı birer veri yapısı olarak tasarlanmış ve User isimli veri yapısında birer alan olarak kullanılmışlardır. Bad Practice olarak tasarlanan BadUser veri yapısına göre yeniden kullanılabilir bileşenler söz konusudur. Bir başka deyişle farklı veri yapılarında da kullanılabilirler. Örneğe ait çalışma zamanı çıktısı aşağıdaki gibidir.

![rust_exc_10.png](/assets/images/2025/rust_exc_10.png)

## Daha Kapsamlı Test Senaryoları Yazmak

Kodun kalitesini ve doğruluğunu artırmak için kapsamlı test senaryoları yazmak önemlidir. Burada normal durumlar dışında uç vakalar (edge cases) ve hata senaryolarını da kapsayan testler yazılması önemlidir. Söz gelimi bir sosyal sigorta güvenlik numarasının doğruluğunu kontrol eden bir fonksiyon geliştirdiğimizi düşünelim. Bu fonksiyon için sadece geçerli numaraları değil, aynı zamanda format ihlallerini, eksik karakterleri ve diğer olası hata durumlarını da test etmeliyiz. Aşağıdaki örnek kod parçasında bu senaryo ele alınmaktadır. Kodun test edilmiş olması ve olası tüm senaryolarının ele alınması aynı zamanda Code Coverage değerini artıran ve dolayısıyla kodun kalitesini pozitif anlamda etkileyen önemli bir unsurdur.

```rust
pub fn validate_social_security_number(ssn: &str) -> bool {
    // Basit bir doğrulama: SSN 9 haneli olmalı ve sadece rakamlardan oluşmalı
    let is_nine_digits = ssn.len() == 9;
    let all_digits = ssn.chars().all(|c| c.is_digit(10));
    is_nine_digits && all_digits
}

#[cfg(test)]
mod tests {
    use super::*;

    // Normal durum testi
    #[test]
    fn test_valid_ssn() {
        assert!(validate_social_security_number("123456789"));
    }

    // Edge case testleri
    #[test]
    fn test_empty_or_whitespace_ssn() {
        assert!(!validate_social_security_number("")); // Boş string
        assert!(!validate_social_security_number("   ")); // Sadece boşluk
    }
    
    #[test]
    fn test_too_long_or_short_ssn() {
        assert!(!validate_social_security_number("123456789012345")); // Çok uzun
        assert!(!validate_social_security_number("12345")); // Çok kısa
    }

    // Hata Senaryosu/Negatif testleri
    #[test]
    fn test_invalid_format_ssn() {
        assert!(!validate_social_security_number("123-45-6789")); // Yanlış format
        assert!(!validate_social_security_number("12345678A")); // Harf içeriyor
        assert!(!validate_social_security_number("12 3456789")); // Boşluk içeriyor
    }

    #[test]
    fn test_right_length_but_wrong_characters_ssn() {
        assert!(!validate_social_security_number("12345A789")); // 8 haneli
    }
}
```

Bu kodun test çıktıları da aşağıdaki gibi olacaktır.

![rust_exc_11.png](/assets/images/2025/rust_exc_11.png)

## Lazy Iterator Kullanımı ile Bellek Verimliliğini Artırmak

Rust, fonksiyonel dil özellikleri barındırır ve güçlü iterator fonksiyonlarına sahiptir (Hatta zero-cost abstraction söz konusudur ve dolayısıyla iteratif fonksiyonların maliyetleri oldukça düşüktür) map, filter ve collect gibi birbirlerine bağlaran fonksiyonel bir akış oluşturan metotlar esasında next işlevi çağırılana kadar yürütülmezler. Bunu Lazy Evaluation olarak ifade edebiliriz. Bu durumda gereksiz hesaplamaların önüne geçilerek bellek verimliliği artırılabilir. Elbette bunun tam tersi olarak birde Eager Evaluation durumu vardır. Eager Evaluation senaryosunda tüm veri üzerinde işlemler hemen gerçekleştirilir ve sonuçlar anında elde edilir. Ancak bu durum büyük veri setlerinde performans ve bellek kullanımı açısından dezavantajlı olabilir. Dolayısıyla duruma göre Lazy veya Eager load stratejileri tercih edilebilir.

Çok büyük bir log dosyasından ham metin girdilerinin okunup analiz edildiği durumlarda Lazy Evaluation ile bellek kullanımını daha optimize edebiliriz. Aşağıda bu senaryoya ilişkin basit bir kod parçası yer almaktadır.

```rust
fn main() {
    let log_data = vec![
        String::from("INFO: Application started"),
        String::from("ERROR: Failed to load configuration"),
        String::from("INFO: User logged in"),
        String::from("ERROR: Database connection lost"),
    ];

    println!("--- Lazy Evaluation Results ---");
    let error_logs = get_error_logs_lazy(&log_data);
    error_logs.iter().for_each(|log| println!("{}", log));

    println!("--- Eager Evaluation Results ---");
    let error_logs = get_error_logs_eager(&log_data);
    error_logs.iter().for_each(|log| println!("{}", log));
}

/// Basit bir log analiz fonksiyonu (Lazy Evaluation ile)
/// Log verisi alır ve "ERROR" içeren satırları döner
///
/// # Arguments
///
/// * `log_data` - Log verisi içeren String vektörü
///
/// # Returns
///
/// * `impl Iterator<Item=String>` - "ERROR" içeren log satırlarını üreten iterator
fn get_error_logs_lazy(log_data: &[String]) -> Vec<String> {
    /*
        Bu yaklaşımda Lazy Evaluation kullanılmaktadır.
        Log verisi üzerinde bir iterator oluşturulur ve
        "ERROR" içeren satırlar filtrelenir.
        Bu sayede gereksiz yere tüm veriyi işlemekten kaçınılır.
    */
    log_data
        .into_iter()
        .filter(|line| line.contains("ERROR"))
        .map(|line| {
            let columns = line.split(": ").collect::<Vec<&str>>();
            format!(
                "Critical Error Found: {}",
                columns.last().unwrap_or(&"Unknown Error")
            )
        })
        .collect()
}

/// Basit bir log analiz fonksiyonu (Eager Evaluation ile)
/// Log verisi alır ve "ERROR" içeren satırları döner
///
/// # Arguments
///
/// * `log_data` - Log verisi içeren String vektörü
///
/// # Returns
///
/// * `Vec<String>` - "ERROR" içeren log satırlarını içeren vektör
fn get_error_logs_eager(log_data: &[String]) -> Vec<String> {
    /*
        Bu yaklaşımda Eager Evaluation kullanılmaktadır.
        Tüm log verisi işlenir ve "ERROR" içeren satırlar
        hemen döndürülür.
    */
    let mut error_logs = Vec::new();
    for line in log_data {
        if line.contains("ERROR") {
            let columns: Vec<&str> = line.split(": ").collect();
            let formatted_log = format!(
                "Critical Error Found: {}",
                columns.last().unwrap_or(&"Unknown Error")
            );
            error_logs.push(formatted_log);
        }
    }
    error_logs
}
```

Bu örnek elbette performans farkı ve çalışma zamanı bellek tüketim maliyetlerini göstermez ancak Lazy ve Eager Loading senaryoları için kodu nasıl kullanacağımızı açıklar. Çalışma zamanı çıktısını da buraya bırakalım. Zira siz denediğinizde de benzer sonuçlar almalısınız.

![rust_exc_12.png](/assets/images/2025/rust_exc_12.png)

## Generic Türlerde Kısıtlamaları (Constraint) Kullanmak

Generic türlerin kullanıldığı durumlarda türü belli davranışları uygulamaya zorlamak için trait'lerden yararlanılabilir. Böylece örneğin bir iterasyonun aynı davranış veya davranışlara sahip türler ile çalışması sağlanabilir. Dolayısıyla tip sistemini kullanarak işlevselliği bir nevi garanti altına almış oluruz ve bunu sıfır maliyetle yaparız.

Herhangi bir tür için minimum ve maksimum değerleri bulan bir fonksiyon geliştirdiğimizi düşünelim. Bunun için türün karşılaştırılabilir Ord ve kopyalanabilir Copy olması gerekir. Aksi takdirde fonksiyon doğru çalışmayacaktır. Bunu sağlamak için generic tür üzerinde trait kısıtlamaları kullanabiliriz. Aşağıdaki örnek kod parçasında bu durum ele alınmaktadır.

```rust
fn main() {
    let numbers = vec![3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5];
    match find_min_max(&numbers) {
        Some((min, max)) => {
            println!("Minimum: {}, Maximum: {}", min, max);
        }
        None => {
            println!("Empty slice provided.");
        }
    }

    let chars = vec!['y', 'c', 'm', 'e', 'q', 'l', 'x', 'k'];
    match find_min_max(&chars) {
        Some((min, max)) => {
            println!("Minimum: {}, Maximum: {}", min, max);
        }
        None => {
            println!("Empty slice provided.");
        }
    }

    let towers = vec![
        Tower { height: 150 },
        Tower { height: 200 },
        Tower { height: 175 },
    ];
    match find_min_max(&towers) {
        Some((min, max)) => {
            println!(
                "Minimum Tower Height: {}, Maximum Tower Height: {}",
                min.height, max.height
            );
        }
        None => {
            println!("Empty slice provided.");
        }
    }
}

/// Verilen bir slice içindeki minimum ve maksimum değerleri bulan fonksiyon.
/// Eğer slice boşsa None döner, aksi takdirde Some((min, max)) döner.
///
/// # Arguments
/// * `values` - Karşılaştırılacak değerlerin bulunduğu slice.
///
/// # Returns
/// * `Option<(T, T)>` - Minimum ve maksimum değerleri içeren bir tuple veya None.
///
/// # Constraints
/// * `T: Ord + Copy` - T türü karşılaştırılabilir ve kopyalanabilir olmalıdır.
fn find_min_max<T: Ord + Copy>(values: &[T]) -> Option<(T, T)> {
    if values.is_empty() {
        return None;
    }

    let mut min = values[0];
    let mut max = values[0];

    for &value in values.iter() {
        if value < min {
            min = value;
        }
        if value > max {
            max = value;
        }
    }

    Some((min, max))
}

#[derive(Copy, Clone, Eq, PartialEq)]
struct Tower {
    height: u32,
}

impl PartialOrd for Tower {
    fn partial_cmp(&self, other: &Self) -> Option<std::cmp::Ordering> {
        Some(self.cmp(other))
    }
}

impl Ord for Tower {
    fn cmp(&self, other: &Self) -> std::cmp::Ordering {
        self.height.cmp(&other.height)
    }
}
```

Örnekte yer alan findminmax fonksiyonu T türünden bir dizi referansı almakta ve yine aynı türden bir Tuple döndürmektedir. Fonksiyonda T türü için Ord ve Copy trait'lerini uygulama zorunluluğu getirilmiştir. Buna göre primitive tipler'den tutun da kendi yazdığımız türler için de aynı fonksiyonu kullanabiliriz. Tek şart kendi türümüzün de bu trait'leri uygulamış olmasıdır. Örnek kodda yer alan Tower veri yapısı açık bir şekilde PartialOrd ve Ord trait'lerini uygulamaktadır. Bu sayede karşılaştırılabilme davranışını kazanmıştır. Ayrıca derive niteliği üzerinden doğal yolla Copy trait'ini implemente etmektedir. Örneğin çalışma zamanı çıktısı aşağıda görüldüğü gibidir.

![rust_exc_13.png](/assets/images/2025/rust_exc_13.png)

## Daha Güçlü Hata Yönetimi için Custom Error Türleri Oluşturmak veya thiserror Kullanmak

Uygulamalarda hata yönetimi kritik bir öneme sahiptir. I/O işlemleri, network operasyoları, veri tabanı erişimleri, dosya okuma yazma vb işlemler sırasında çeşitli hatalar meydana gelebilir. Rust'ın standart kütüphanesi hata yönetimi için Result türünü sağlar ancak daha karmaşık senaryolarda özel hata türleri oluşturmak gerekir. Burada genellikle kendi enum türlerimizi kullanırız ama idiomtik olarak tüm olası hataları modelleyen thiserror gibi neredeyse bir hata yönetim standardı olmuş kütüphaneleri de kullanabiliriz. Aşağıdaki örnek kod parçasında bir hata yönetimi senaryosu ele alınmaktadır.

```rust
use serde::Deserialize;
use std::{fs, io};
use thiserror::Error;

fn main() -> Result<(), ApiError> {
    let settings = load_settings("config.json");
    match settings {
        Ok(cfg) => {
            println!("Settings loaded: {:?}", cfg);
        }
        Err(e) => {
            eprintln!("Error loading settings: {}", e);
        }
    }

    let ping_result = send_ping("localhost:67000");
    match ping_result {
        Ok(_) => println!("Ping successful!"),
        Err(e) => eprintln!("Error sending ping: {}", e),
    }

    Ok(())
}

#[derive(Error, Debug)]
pub enum ApiError {
    // io:Error türündeki hataları otomatik olarak ApiError::Io varyantına dönüştürür.
    #[error("I/O Error: {0}")]
    Io(#[from] io::Error),

    // Ağ ile ilgili hataları temsil eder.
    #[error("Network Error: {0}")]
    Network(String),

    // JSON serileştirme/deserileştirme hatalarını temsil eder.
    #[error("JSON Error: {0}")]
    Json(#[from] serde_json::Error),
}

fn load_settings(path: &str) -> Result<Settings, ApiError> {
    let data = fs::read_to_string(path)?; // io::Error otomatik olarak ApiError::Io'ya dönüştürülür
    let settings: Settings = serde_json::from_str(&data)?; // serde_json::Error otomatik olarak ApiError::Json'a dönüştürülür
    Ok(settings)
}

fn send_ping(api_url: &str) -> Result<(), ApiError> {
    let response = std::net::TcpStream::connect(api_url);
    match response {
        Ok(_) => println!("Ping to {} successful!", api_url),
        Err(e) => return Err(ApiError::Network(e.to_string())),
    }
    Ok(())
}

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
struct Settings {
    api_url: String,
    timeout: u64,
}
```

Burada hata durumları için ApiError isimli enum türünden bir veri yapısı kullanılmaktadır. Dikkat edileceği üzere bu enum yapısı Error ve error nitelikleri ile donatılmıştır. thiserror crate'inden gelen bu nitelikler ile farklı hata türleri kendi standart hata türümüze kolayca çevrilebilir ve bu uygulama genelinde bir standartlık sağlar. Söz gelimi io::Error türünden bir hata oluştuğunda bu otomatik olarak ApiError:Io'ya evrilir. Result üzerinden Error dönme potansiyeli olan her yerde ApiError veri yapımız ile çalışabilir ve genelleştirdiğimiz hataları ele alabiliriz.

Tabii bu örneği çalıştırmak için gerekli crate'lerin projeye yüklenmiş olması gerektiğini hatırlatalım.

```text
[dependencies]
serde = { version = "1.0.228", features = ["derive"] }
serde_json = "1.0.145"
thiserror = "2.0.17"
```

Konu kapsamında thiserror crate'ine odaklanmak gerekir. serde ve serdejson küfeleri sadece JSON bazlı örnekler için eklenmiştir.

![rust_exc_14.png](/assets/images/2025/rust_exc_14.png)

## Tip Dönüşümlerinde From ve Into Trait'lerini Kullanmak

Rust dilinde tip dönüşümleri için genellikle From ve Into trait'leri kullanılır. Bu trait'ler, bir türün başka bir türe dönüştürülmesini sağlar. From trait'i, bir türden diğerine dönüşüm için bir yöntem tanımlar. Bu trait uygulandığında otomatik olarak Into trait'i de uygulanmış olur. Yani bir türden diğerine dönüşüm yapmak için ya From ya da Into uyarlamaları kullanılabilir. Söz konusu trait'ler Ownership (sahiplik) ve borçlanma (borrowing) kuralları ile uyumlu çalışımasını sağlar ve dolayısıyla dönüşümler güvenli bir şekilde gerçekleştirilir. Ayrıca Rust bu dönüşümleri optimize edebilir ve gereksiz kopyalamaları önleyebilir.

Uygulama seviyesindeki hataları temsil eden bir enum türümüz olduğunu düşünelim. Bazı iç fonksiyonlardan da Result türünde bu enum türü ile hata dönüyor olsun. Diğer hata türlerinden bu enum türüne dönüşüm yapabilmek için From trait'ini kullanabiliriz. Örneğin bir I/O hatası yakalandıysa ve bunu katmanlara çıkarken kendi hata türümüze dönüştürmek istiyorsak From trait'ini uygulayabiliriz. Bu senaryoda? operatörü de otomatikman çalışacak ve türler arasında dönüşüm sağlanacaktır. Bir nevi önceki örnekte kullandığımız thiserror crate ile ele aldığımız senaryoyu işletebileceğimizi ifade edebiliriz. Aşağıdaki örnek kod parçasında bu senaryo ele alınmaktadır.

```rust
use std::fs::File;
use std::io;
use std::num;

fn main() {
    let result = search("WARNING");
    match result {
        Ok(content) => println!("Search successful: {}", content),
        Err(e) => match e {
            AppError::Io(err) => eprintln!("I/O Error: {}", err),
            AppError::Parse(err) => eprintln!("Parse Error: {}", err),
            AppError::Auth(msg) => eprintln!("Authentication Error: {}", msg),
            AppError::NotFound(msg) => eprintln!("Not Found Error: {}", msg),
        },
    }

    let num_str = "32a14";
    if let Err(e) = parse_number(num_str) {
        match e {
            AppError::Parse(err) => eprintln!("Failed to parse number: {}", err),
            _ => eprintln!("An unexpected error occurred"),
        }
    }

    // into kullanımı ile de türler arası dönüşüm yapılabilir
    // Burada io::Error türündeki hata AppError türüne dönüştürülmektedir
    let io_error = io::Error::new(io::ErrorKind::Other, "an I/O error occurred");
    let app_error: AppError = io_error.into();
    match app_error {
        AppError::Io(err) => eprintln!("Converted I/O Error: {}", err),
        _ => eprintln!("An unexpected error occurred"),
    }
}

fn parse_number(s: &str) -> Result<i32, AppError> {
    /*
    parse fonksiyonu str türündeki bir veriyi i32 türüne dönüştürmeye çalışır.
    Eğer dönüşüm başarılı olursa, Ok(num) döner aksi durumda ParseIntError türünde bir hata oluşur.
    From trait implemente edildiği için bu hata AppError türüne otomatikman dönüştürülür.
    */
    let num: i32 = s.parse()?;
    Ok(num)
}

fn search(query: &str) -> Result<String, AppError> {
    /*
    Bu fonksiyon belirtilen dosyayı açar ve içeriğinde query parametresi ile gelen veriyi arar.
    Eğer dosya açılamazsa io::Error türünde bir hata oluşur ve bu hata AppError türüne dönüştürülür.
    Zira, From trait implemente edilmiştir.
    */
    let f = File::open("games.dat")?;
    println!("File opened successfully: {:?}", f);
    println!("Searching for query: {}", query);
    Ok(String::from("Content found"))
}

#[allow(dead_code)]
#[derive(Debug)]
enum AppError {
    Io(io::Error),
    Parse(num::ParseIntError),
    Auth(String),
    NotFound(String),
}

impl From<io::Error> for AppError {
    fn from(error: io::Error) -> Self {
        AppError::Io(error)
    }
}

impl From<num::ParseIntError> for AppError {
    fn from(error: num::ParseIntError) -> Self {
        AppError::Parse(error)
    }
}
```

Dikkat edileceği üzere io kütüphanesinden Error ve num kütüphanesinden ParseIntError türlerinin AppError türüne dönüşmesi için birer uyarlama söz konusu. Buna göre search ve parsenumber fonksiyonlarında meycana gelebilecek ve? operatörü ile otomatik olarak yakalanacak bu hatalar otomatik olarak AppError türüne dönüştürülebilir. Kodun çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![rust_exc_15.png](/assets/images/2025/rust_exc_15.png)

## Generic Trait'lerde Associated Types Kullanımı

Trait'lerde generic tür parametreleri kullanmak yerine ilişkili tipler (associated types) kullanarak soyutlamalar yapabiliriz. Bunu yaparak trait'in uygulamadaki kullanımında sadece tek bir somut tür ile çalışmasını garanti ederiz. Örneğin bir veri deposu (data store) soyutlaması tasarladığımızı düşünelim. Bu soyutlama farklı veri türleri ile çalışabilir ancak her bir veri deposu uyarlaması sadece tek bir veri türü ile çalışmalıdır. Bu durumda generic tür parametreleri yerine ilişkili tipler kullanmak daha uygun olacaktır. Aşağıdaki kod parçasında her iki yaklaşımın kullanımına ait örnek kodlar yer almaktadır.

```rust
/*
    DataStore trait'i basit bir veri deposu soyutlaması sağlar.
    Associated type ile veri tipinin belirtilmesi zorunlu kılınır.

    InMemoryStore yapısı, string türünden verileri bellekte saklayan bir veri deposu.
    DataStore trait'ini implemente ediyor ve ilişkili tip olarak String kullanacağını somut bir şekilde belirtiyor.
*/
#[allow(dead_code)]
trait DataStore {
    type Item; // Associated type tanımı

    fn save(&mut self, item: Self::Item);
    fn read(&self, id: u32) -> Option<Self::Item>;
}
struct InMemoryStore {
    items: Vec<String>,
}
impl DataStore for InMemoryStore {
    type Item = String; // Somut tür belirtimi. Artık Item türü String dolayısıyla InMemoryStore sadece String türünden verilerle çalışır.

    fn save(&mut self, item: Self::Item) {
        self.items.push(item);
    }

    fn read(&self, id: u32) -> Option<Self::Item> {
        self.items.get(id as usize).cloned()
    }
}

/*
    Generic tür parametreleri ile aynı soyutlamayı yapıyoruz.
    Burada ilişkili tip yerine generic tür parametresi T kullanılıyor.
    Dikkat edileceği üzere type şeklinde bir tanımlama yok. Dolayısıyla bu trait'i implemente etmek isteyen bir yapı,
    hangi türü kullanacağını her seferinde belirtmek zorunda. Elbette bazen bu esnekliğe ihtiyaç duyuyoruz.
*/
#[allow(dead_code)]
trait GenericDataStore<T> {
    fn save(&mut self, item: T);
    fn read(&self, id: u32) -> Option<T>;
}
struct GenericInMemoryStore<T> {
    items: Vec<T>,
}

impl<T: Clone> GenericDataStore<T> for GenericInMemoryStore<T> {
    fn save(&mut self, item: T) {
        self.items.push(item);
    }

    fn read(&self, id: u32) -> Option<T> {
        self.items.get(id as usize).cloned()
    }
}

fn main() {
    let mut store = InMemoryStore { items: vec![] };
    store.save("connection string".to_string());
    store.save("minio address".to_string());
    for item in store.items.iter() {
        println!("Loaded from InMemoryStore: {}", item);
    }

    let mut generic_store = GenericInMemoryStore { items: vec![] };
    generic_store.save(42);
    generic_store.save(100);
    if let Some(item) = generic_store.read(1) {
        println!("Loaded from GenericInMemoryStore: {}", item);
    }
}
```

Örneğin çalışma zamanı çıktısı aşağıdaki gibidir.

![rust_exc_16.png](/assets/images/2025/rust_exc_16.png)

## Iterator Adaptörleri ve collect Kullanımı

Rust'ın fonksiyonel programlama özelliklerinden biri olan iterator adaptörleri akan veri üzerinde işlem yapmamızı kolaylaştırır. Bunu yaparken döngüsel yapılar kurmamıza gerek kalmaz. Tüm operasyonu zincir metotlar üzerinden halledebiliriz. Collect, fold, reduce, find, any, all gibi pek çok adaptör metodu mevcuttur. Bu adaptörler sayesinde veriyi filtreleyebilir, dönüştürebilir, toplayabilir veya belirli koşullara göre sorgulayabiliriz.

Bu adaptörlerden birisi olan collect en çok kullanılanlar arasındadır. Standart bir iterator zinciri üzerinden elde edilen veriyi farklı koleksiyon türlerine dönüştürmek için kullanılır. Örneğin bir vektör içindeki sayıları filtreleyip bir HashSet veya başka bir veri yapısına dönüştürmek istediğimizde collect metodunu kullanabiliriz. Collect çağrısı sonucu bir değişkene atanabilir ve burada dönüş türü derleyici tarafından otomatik olarak tahmin edilebilir (type inference) ama bazen dönüş türünün açıkça belirtilmesi gerekir. Aşağıdaki kod parçasında collect kullanımına dair birkaç örnek bulunmaktadır.

```rust
use rand::Rng;

fn main() {
    // 10 adet rastgele sayı üretimi (map ile birlikte kullanım)
    let mut rng = rand::rng();
    let numbers: Vec<i32> = (0..10).map(|_| rng.random_range(1..101)).collect();
    println!("Random 10 numbers: {:?}", numbers);

    // Rastgele üretilmiş olan sayılardan çift olanların filtrelenmesi (filter ile birlikte kullanım)
    let even_numbers: Vec<i32> = numbers.into_iter().filter(|&x| x % 2 == 0).collect();
    println!("Even numbers: {:?}", even_numbers);

    // Bir sayı dizisindeki asal sayıların listesi ve toplam sayısı (filter ile birlikte kullanım)
    let numbers: Vec<i32> = (0..20).map(|_| rng.random_range(1..101)).collect();
    let primes: Vec<i32> = numbers.into_iter().filter(|&x| is_prime(x)).collect();
    println!("Prime numbers: {:?}", primes);
    println!("Count of prime numbers: {}", primes.len());

    // 8 adet güçleri 2 ile 5 arasında değişen AIPlayer nesneleri oluşturulması
    let ai_players: Vec<AIPlayer> = (0..8)
        .map(|i| AIPlayer {
            name: format!("AI_Player_{}", i + 1),
            power: rng.random_range(2..6),
        })
        .collect();

    // Bu oyunculardan gücü 4'ten büyük olanların rastgele bir lokasyona atanması
    let strong_ai_locations: Vec<(AIPlayer, Location)> = ai_players
        .into_iter()
        .filter(|player| player.power > 4)
        .map(|player| {
            let location = Location {
                x: rng.random_range(0.0..100.0),
                y: rng.random_range(0.0..100.0),
            };
            (player, location)
        })
        .collect();

    println!("Strong AI Players and their Locations:");
    strong_ai_locations.iter().for_each(|(player, location)| {
        println!(
            "{} (Power: {}) is at Location ({:.2}, {:.2})",
            player.name, player.power, location.x, location.y
        );
    });
}

fn is_prime(num: i32) -> bool {
    if num <= 1 {
        return false;
    }

    for i in 2..=((num as f64).sqrt() as i32) {
        if num % i == 0 {
            return false;
        }
    }

    true
}

#[allow(dead_code)]
#[derive(Debug)]
struct AIPlayer {
    name: String,
    power: u16,
}

#[derive(Debug)]
struct Location {
    x: f64,
    y: f64,
}
```

Kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![rust_exc_17.png](/assets/images/2025/rust_exc_17.png)

## Module Gizleme ve Erişim Kontrolü

Modüller alan adı (namespace) ve gizlilik sınırı (privacy boundary) işlevi sağlar. Benzer amaca yönelik enstrümanları bir arada tutmak için kullanılırlar ve ayrıca erişim kontrolü de sağlarlar. Rust'ta modül içi öğelerin standart erişim seviyesi private şeklindedir. Yani bir modül içindeki öğelere sadece o modülün içinden erişilebilir. Ancak pub anahtar kelimesi kullanılarak bu öğelerin erişim seviyesi artırılabilir ve dışarıdan erişime açılabilir. Ayrıca modüller hiyerarşik bir yapıya sahip olabilir ve alt modüller oluşturulabilir. Bu durumda erişim kontrolü daha da detaylandırılabilir. Örneğin bir modül içindeki bazı fonksiyonlar veya yapılar sadece o modül içinden erişilebilirken, bazıları dışarıdan erişime açık olacak şekilde tasarlanabilir. Bazı durumlarda bir veri yapısının (struct) yalnızca kendi implementasyon bloğu içindeki metotlar tarafından değiştirilmesi istenebilir. Böyle bir senaryoda veri yapısını bir modül içine alıp kapsülleme (encapsulation) sağlanabilir. Aşağıdaki örnek kod parçasında bu durum ele alınmaktadır.

```rust
fn main() {
    let mut settings = settings::AppSettings::new(settings::LogLevel::Info);
    println!("Initial Settings: {:?}", settings);

    settings.set_connections(200);
    settings.set_port(9090);
    println!("Settings after update: {:?}", settings);
}

/*
    settings modülünde yer alan LogLevel pub erişim belirleyicisi ile tanımlanmış bir enum'dur.
    Dolayısıyla settings modülü dışından da erişilebilir.
    
    modüle veri yapılarından olan AppSettings struct'ı da dışarıdan erişilebilir çünkü o da
    pub erişim belirleyicisi ile tanımlanmıştır. Ancak, AppSettings struct'ının bazı alanları
    (max_connections ve port) pub olarak tanımlanmadıklarından dışarıdan doğrudan erişilemezler.
    Bu alanlara erişim ve değiştirme işlemleri için public metotlar (getters ve setters) ile sağlanır.

    AppSettings veri yapısı new metodu ile oluşturulurken LogLevel değerini dışarıdan alabilir ancak,
    max_connections ve port alanları varsayılan değerlerle (DEFAULT_MAX_CONNECTIONS ve DEFAULT_PORT)
    başlatılır. Bu sayede, dışarıdan erişilemeyen alanların kontrolü modül içinde tutulmuş olur.
    Yani, bir encapsulation (kapsülleme) sağlanmış olur.
*/
#[allow(dead_code)]
mod settings {

    #[derive(Debug)]
    pub enum LogLevel {
        Error,
        Warn,
        Info,
        Debug,
        Trace,
    }

    #[derive(Debug)]
    pub struct AppSettings {
        pub log_level: LogLevel,
        max_connections: u32,
        port: u16,
    }

    impl AppSettings {
        const DEFAULT_MAX_CONNECTIONS: u32 = 100;
        const DEFAULT_PORT: u16 = 8080;

        pub fn new(log_level: LogLevel) -> Self {
            AppSettings {
                log_level,
                max_connections: Self::DEFAULT_MAX_CONNECTIONS,
                port: Self::DEFAULT_PORT,
            }
        }

        pub fn get_connections(&self) -> u32 {
            self.max_connections
        }

        pub fn set_connections(&mut self, connections: u32) {
            self.max_connections = connections;
        }

        pub fn get_port(&self) -> u16 {
            self.port
        }

        pub fn set_port(&mut self, port: u16) {
            self.port = port;
        }
    }
}
```

![rust_exc_18.png](/assets/images/2025/rust_exc_18.png)

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
