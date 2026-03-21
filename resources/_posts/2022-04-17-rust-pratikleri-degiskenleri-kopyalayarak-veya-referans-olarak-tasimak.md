---
layout: post
title: "Rust Pratikleri - Değişkenleri Kopyalayarak veya Referans Olarak Taşımak"
date: 2022-04-17 09:00:00 +0300
categories:
  - rust
tags:
  - rust
  - rust-lang
  - memory-management
  - stack
  - heap
  - gdb
---
Rust bellek yönetimi konusunda epey hassas olduğundan, fonksiyonlara aktarılan değerlerin nasıl taşınacağı da önemli bir konudur. Bir.net geliştiricisi olarak değişkenlerin fonksiyonların değer türü veya referans türü olarak aktarıldığını biliyorum. Söz gelimi Rust tarafında olmayan class türevli nesneler fonksiyonlara otomatik olarak referans adresleri ile aktarılıyorlar. Üstelik bunu açıkça belirtmemize gerek olmadığını söyleyebilirim. Peki Rust tarafında durum nasıl? Sonuçta ortada bir Garbage Collector yok. Değişkenler varsayılan olarak değiştirilemez (immutable). Class diye bir kavram yok ve kodlarda kullandığımız değişkenler katı bir biçimde ownership, borrowing gibi kural denetimlerine tabiler.

![ownership.jpg](/assets/images/2022/ownership.jpg)

İşte bu pratikte içinde başka bir enum sabiti kullanan bir başka enum sabitinin uzak diyarlardaki bir fonksiyon tarafından parametre olarak kullanılması sırasında oluşabilecek Value Moved Here sorununa bakmaya çalışacağız ki Rust dilini yeni yeni öğrenmeye başlayanların en sık karşılaştığı derleme zamanı hata mesajlarından birisi olduğunu ifade edebilirim. Cümle biraz karışık oldu:D Haydi gelin basit bir örnek üstünden konuyu detayları ile incelemeye çalışalım. Senaryomuz gereği ilk olarak sorunsuz bir program kodu yazacağız. Ardından problemi oluşturacak ve son olarak iki farklı kullanım şeklini ele alacağız.

```bash
# projeyi oluşturma aşaması
cargo new by_val_by_ref
```

Program kodumuzun ilk versiyonunu aşağıdaki gibi tasarlayabiliriz.

```rust
fn main() {
    let binding = Binding::Https(SoapVersion::V1_2);
    prepare_env(binding);
}

#[derive(Debug)]
enum SoapVersion {
    V1_1,
    V1_2,
}
enum Binding {
    Http(SoapVersion),
    Https(SoapVersion),
    Rest,
    Grpc,
}

fn prepare_env(b: Binding) {
    match b {
        Binding::Http(v) | Binding::Https(v) => {
            println!("HTTP {:?} versiyonu için ortam hazırlanıyor.", v)
        }
        Binding::Rest => println!("Servis REST protokolüne göre hazırlanıyor"),
        Binding::Grpc => println!("Servis GRPC protoklüne göre hazırlanıyor"),
    }
}
```

Örnek olarak bir servisin bağlantı noktasını temsil eden Binding isimli bir enum sabitimiz var. Bu enum sabiti içinde yer alan Http ve Https değerleri de aslında SoapVersion isimli bir başka enum sabiti ile ifade ediliyorlar. Böylece bir servisin bağlantı noktasında kullanılan protokolü farklı şekillerde ifade etme şansına sahip oluyoruz. Rust'taki enum türü bu tip kullanımlar düşünüldüğünde bence epey güçlü bir enstrüman. prepare_env isimli fonksiyonumuz parametre olarak gelen Binding sabitinin değerlerine bakmak için bir match ifadesi kullanıyor. Sadece ekrana bilgi yazdırdığımızı söyleyebiliriz. Programımız bu haliyle çalıştırdığımızda herhangi bir sıkıntı olmayacaktır. Oldukça sade ve anlaşılır zaten.

![by_val_by_ref_1.png](/assets/images/2022/by_val_by_ref_1.png)

Şimdi binding değişkeni için prepare_env fonksiyonunu bir kez daha çağıralım. Şeytanlık yapacağız ya...

```rust
fn main() {
    let binding = Binding::Https(SoapVersion::V1_2);
    prepare_env(binding);
    prepare_env(binding);
}
```

Bu durumda aşağıdaki ekran görüntüsünde yer alan çalışma zamanı çıktısı ile karşılaşırız.

![by_val_by_ref_2.png](/assets/images/2022/by_val_by_ref_2.png)

İşte bahsettiğimiz "value moved here" hatası. Rust'a göre kural şöyle; Bir değeri (Value) bu örnekteki gibi bir scope'a taşıdıktan sonra sahipliğini (Ownership) kaybederiz.

Yukarıdaki gibi bir kullanım çok anlamlı değil gerçi ama problemin çözümü adına değişken sahipliğinin binding_env scope'undan döndükten sonra nasıl korunacağını öğrenmeliyiz. Sahipliğin korunması için basit iki yöntem bulunuyor. Binding ve SoapVersion enum sabitlerinin bit seviyesinde kopylanmasına izin vermek ki bu değerlerin kopyalanarak taşınması anlamına gelmekte ya da nesne referansını taşımak. İlki için built-in olarak gelen Clone ve Copy trait'lerini derivable modada uyarlamak yeterli. Aynen aşağıdaki kod parçasında görüldüğü gibi.

```bash
#[allow(dead_code)]
#[derive(Debug, Clone, Copy)]
enum SoapVersion {
    V1_1,
    V1_2,
}

#[allow(dead_code)]
#[derive(Clone, Copy)]
enum Binding {
    Http(SoapVersion),
    Https(SoapVersion),
    Rest,
    Grpc,
}
```

Bu arada Copy ve Clone trait'lerini birlikte uygulamak gerekiyor. Sadece Copy trait'ini uygulamak en azından bu örnekteki enum sabitlerimiz için yeterli değil. Eğer böyle yaparsak derleyici yeni durumdan yine hoşnut olmayaca ve bizi şık bir şekilde uyaracaktır.

![by_val_by_ref_3.png](/assets/images/2022/by_val_by_ref_3.png)

Clone ve Copy trait'lerini uyguladığımız için binding değişkeni ilgili fonksiyona kopylanarak geçirilmiş oldu. Bu durumda çalışma zamanında bir problemle karşılaşmayız.

![by_val_by_ref_4.png](/assets/images/2022/by_val_by_ref_4.png)

Değerin kopyalanarak taşınması durumunu daha iyi anlamak için gdb debugger aracını kullanarak çalışma zamanındaki stack oluşumlarına bakmak iyi bir fikir olabilir. Aşağıda gerekli terminal komutlarını görebilirsiniz.

```bash
cargo build
cd target/debug
gdb by_val_by_ref
run
list
b prepare_env
r
info args
c
info args
c
```

İlk olarak programımızı derlememiz gerekiyor. Ardında oluşan binary dosyasının olduğu klasöre geçiyoruz. gdb ile debugger arabirimini açtıktan sonra uygulamayı run komutu ile bir kere çalıştırmakta yarar var. Ardından b komutu ile prepare_env fonksiyonuna bir breakpoint bırakıyoruz. r ile programı işlettiğimizde iki kez bu fonksiyona düşeceğiz. Her seferinde args ile fonksiyona gelen parametrelere bakarsak değişkenin birebir kopyasının aktarıldığını görebiliriz. Yukarıdaki işlemlerin çıktısı kendi ubuntu sistemimde aşağıdaki gibi oldu.

![by_val_by_ref_5.png](/assets/images/2022/by_val_by_ref_5.png)

Şimdi ikinci kullanım seçeneğine bakalım. Değeri referans olarak taşımak. Bir başka deyişle pointer adresini aktarmak. İşin püf noktası & sembolü. Bu amaçla uygulama kodumuzu aşağıdaki gibi düzenlememiz yeterli.

```rust
fn main() {
    let binding = Binding::Https(SoapVersion::V1_2);
    prepare_env(&binding);
    prepare_env(&binding);
}

fn prepare_env(b: &Binding) {
    match b {
        Binding::Http(v) | Binding::Https(v) => {
            println!("HTTP {:?} versiyonu için ortam hazırlanıyor.", v)
        }
        Binding::Rest => println!("Servis REST protokolüne göre hazırlanıyor"),
        Binding::Grpc => println!("Servis GRPC protoklüne göre hazırlanıyor"),
    }
}
```

Fonksiyonumuzun parametresini referans alacak şekilde değiştirdik ve elbette binding değişkenlerini gönderdiğimiz yerde de referans belirterek yolladık. Ayrıca match ifadesi içerisine dikkat edecek olursanız SoapVersion enum tipinin de referans olarak alındığını görebiliriz. Tabii bunu gösteren IDE'nin kendisi. IntelliJ IDE veya Visual Studio Code ile baktığımızda gri tonlamalı alanlardan bunu anlayabiliriz.

![by_val_by_ref_6.png](/assets/images/2022/by_val_by_ref_6.png)

Koplayama yöntemiyle taşımada yaptığımız gibi bu kullanımı da GDB Debugger ile analiz edelim derim. Sonuçta kod parçamız oldukça az. Kolayca debug edebiliriz. İşte program kodunun son halini derleyerek başlıyoruz.

```bash
cargo build
cd target/debug
gdb by_val_by_ref
run
list
b prepare_env
r
info args
print *b
c
info args
print *b
c
```

Özellikle prepare_env fonksiyonundaki breakpoint noktalarında argümanın bir pointer olduğuna ve her iki çağrıda da aynı referans adresinin kullanıldığında dikkat edelim. Pointer içeriklerini görmek için print b komutunu kullanıyoruz.

![by_val_by_ref_7.png](/assets/images/2022/by_val_by_ref_7.png)

Sanıyorum ki aktif olarak kullanmakta olduğunuz programlama dilinde değişken verilerini fonksiyonlara atarken bir kere daha düşüneceksiniz:) Çoğu yüksek seviyeli dilde bu tip ayrımlara hiç takılmadan geliştirme yapmaktayız. Bu geliştirme hızımızı elbette yükseltiyor ancak güvenli bellek sahaları ve performans öne çıktığında sistemin daha da yavaşlamasına sebebiyet veriyor. Tabii bu noktada aklımıza bir soru takılıyor olabilir. Rust varsayılan olarak neden bir değişkenin tek bir sahibi olmasını istiyor? İşte hem size hem de bana güzel bir soru:D

Böylece geldik bir Rust pratiğimizin daha sonuna. Her zaman olduğu gibi [örneğe ait kodlara github reposundan erişebilirsiniz](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/by_val_by_ref). Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
