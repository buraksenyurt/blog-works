---
title: "Rust Pratikleri - Lifetimes Mevzusu"
pubDate: 2022-03-06 09:00:00
categories:
  - Rust
tags:
  - Rust
  - Bash
  - C#
  - Memory Management
  - Performance
  - Pointers
  - GitHub
  - Ownership
---

# Rust Pratikleri - Lifetimes Mevzusu
![vivalasvegas.jpg](images/vivalasvegas.jpg)

Rust'ın özellikle Garbage Collector kullanan dillerden çok farklı olduğunu bellek yönetimi için getirdiği kurallardan dolayı biliyoruz. Ownership, borrowing gibi hususlar sayesinde güvenli bir bellek ortamını garanti etmek üzerine ihtisas yapmış bir dil olduğunu söylesek yeridir. Bunlar pek çok dilde otomatik yönetildiği için Rust'ı öğrenmek biraz zaman alabiliyor ve bu yolda karşımıza çıkacak zor konulardan birisi de Lifetimes mevzusu. Kısaca nesnelere yaşam süresini bilinçli olarak vermek diye ifade edebileceğimiz bu konuyu esasında sevgili [Bora Kaşmer](https://www.borakasmer.com/) ile başladığımız [45 Byte sohbetleri](https://youtube.com/playlist?list=PLY-17mI_rla7WSQoRx_8a_1k79x3LDX4W)nde dile getirmek istiyorum. Lakin çok basit bir örnek ile konuyu olabildiğince sade bir şekilde anlatmam gerekiyor ve String ile &str arasındaki fark bunun için ideal olabilir.

String gibi veri türleri, bulundukları tüm dillerde Heap ovasını dilediğince kullandığı için hem pratik hem de tehlikeli olabiliyor. Özellikle performans odaklı diller Heap konusunda çok hassas ve gereksiz kaynak tüketimlerini sevmiyorlar. Olayıların daha çok stack üstünde kalmasını tercih ediyorlar ve gerçekten gereken hallerde Heap'e çıkılmasını bekliyorlar. Bunu basit bir örnekle pekiştirelim. Diyelim ki TCP protokolü üstünden belli bir sokete gelen paketleri işliyoruz. Bu paketler uygulama tarafına buffer nesnesi olarak alınırlar. Bir buffer belleğe alındığında, onun içinden işe yarar bilgileri alıp örneğin bir struct'ın String türünden değerlerinde saklamak mümkündür. Hatta ilk aklımıza gelen yol budur. Böylece değiştirilebilir (mutable) bir veri modelini de tesis etmiş oluruz. Lakin söz konusu buffer içindeki verileri çektikten sonra değiştirmek gibi bir niyetimiz yoksa (bir başka deyişle onları sadece okunabilir olarak kullanacaksak) heap üstünde String veri türleri için ekstra alanlar açmak yerine buffer içindeki ilgili dilimleri işaret eden &str türlerini kullanabiliriz. Dolayısıyla ağ paketi olarak gelen veriyi alıp doğrudan kullanmak hem bellek üzerindeki operasyonu azaltır hem de performansı artırır.

Peki pratiğimize konu olan lifetimes mevzusu bunun neresinde? Sorun şu ki yukarıda anlattığımız senaryo için ikinci kullanımı tercih edersek (yani String yerine &str kullanırsak) buffer olarak tutulan veri kümesinin bellekten atılması (deallocate) sonrasında Struct nesnemizin &str tipindeki alanları tarafından referans edilen bölgeler geçersiz hale gelecektir. Bu Rust'ın istemediği bir durumdur nitekim Danling Pointer probleminin oluşmasına sebeptir.

Olay biraz karıştı farkındayım. Dilerseniz hiç vakit kaybetmeden proje iskeletini oluşturarak konumuza devam edelim.

```bash
cargo new viva_las_vegas
```

Program kodlarımızı ilk etapta aşağıdaki gibi yazabiliriz.

```bash
#[allow(dead_code)]

#[derive(Debug)]
struct Player {
    id: u32,
    nick: String,
    country: String,
    level: u16,
}

impl Player {
    fn new(id: u32, nick: String, country: String, level: u16) -> Self {
        Self {
            id,
            nick,
            country,
            level,
        }
    }
}

fn main() {
    let gonzi = Player::new(1, "Gonsalez".to_string(), "Brasil".to_string(), 88);
    println!("{:#?}", gonzi);
}
```

Senaryomuz bir oyuncuyu temsil eden veri modelinin tasarlanması ile başlıyor. Player isimli struct işaretsiz 32 bit tamsayı ve String veri türlerinden oluşan alanlara sahip. Kolayca oluşturmak için new isimli bir yapıcı metodumuz da var (constructor). Kod bu haliyle aşağıdaki ekran görüntüsündekine benzer şekilde sorunsuz çalışmakta.

![viva_las_vegas_1.png](images/viva_las_vegas_1.png)

Şimdi senaryomuza oyuncunun nickname bilgisini değiştirecek yeni bir fonksiyon daha dahil edelim. Çok gerekli değil ama tuzağı hazırlamamız için şart.

```csharp
fn change_nickname(p: &mut Player, new_nickname: String) -> &Player {
    p.nick = new_nickname;
    p
}
```

Player yapısının nick değerini parametre olarak gelen new_nickname ile değiştirmek istiyoruz. Tabii nick değerini doğrudan da değiştirebiliriz lakin bu fonksiyon Player nesnesi için ayrı bir scope açacağından ve bu stack bellek bölgesinde yeni bir alan anlamına geldiğinden parametre olarak gelen nesne davranışlarını öğrenmek açısından gerekli. Fonksiyon dikkat edileceği üzere mutable bir Player referansı alıyor, kapsamında nick bilgisini değiştiriyor ve ilgili Player referansını tekrar geri döndürüyor. Tam anlamıyla Player değişkenini referans olarak ödünç alan ve geri veren bir fonksiyon olduğunu söyleyebiliriz. Yeni fonksiyonu kullanacak şekilde main içeriğini değiştirerek devam edelim.

```rust
fn main() {
    let mut gonzi = Player::new(1, "Gonsalez".to_string(), "Brasil".to_string(), 88);
    println!("{:#?}", gonzi);
    let ceremiya = change_nickname(&mut gonzi, "Ceremiya".to_string());
    println!("{:#?}", ceremiya);
}
```

Programı bu haliyle çalıştırdığımızda gonzi değişkeni ile temsil edilen Player nesnesine ait nick alanının başarılı şekilde değiştirildiğini görebiliriz.

![viva_las_vegas_2.png](images/viva_las_vegas_2.png)

Her şey yolunda ve aslında henüz kafamızdan duman çıkaracak bir şey olmadı. Aslında Player değişkenindeki String içerikler literal türden de tanımlanabilirler. String veri türü heap alanını kullanan ve genişleyebilen bir yapıdır. Stack üzerinde Heap'teki metinsel alanı işaret eden, pointer ve referans adresi gibi bilgileri tutar. Ayrıca işaret edilen alanın byte cinsinden uzunluğunu ve ayrılan kapasiteyi de saklar. Ancak en nihayetinde onun için bir allocation söz konusudur. str literal ise String veri türünün olduğu bellek bölgesinin bir parçasını (metnin bir kısmını ki slice olarak ifade edilir) referans eder ve sabit uzunluktadır. Yani bir String üstünden literal çekildikten sonra içeriği değiştirilemez. Metinsel bilginin değişmeyeceği durumlarda literal kullanmak oldukça mantıklıdır (Aynen yazının girişindeki senaryoda bahsettiğimiz gibi). O halde gelin Player veri yapısında yer alan nick ve country alanlarını literal string türüne çevirip devam edelim. Tabii bunu yaptığımız zaman new fonksiyonundaki parametreleri de String türünden &str türüne dönüştürmemiz gerekiyor.

```bash
#[derive(Debug)]
struct Player {
    id: u32,
    nick: &str,
    country: &str,
    level: u16,
}

impl Player {
    fn new(id: u32, nick: &str, country: &str, level: u16) -> Self {
        Self {
            id,
            nick,
            country,
            level,
        }
    }
}
```

Ardından programı clippy ile bir kontrol edelim.

```bash
cargo clippy
```

![viva_las_vegas_3.png](images/viva_las_vegas_3.png)

Upss!!! Bir sürü hata aldık:(nick ve country alanlarını literal string türünden değiştirdik. Ancak bu referanslar Player'ın kullanıldığı scope'lar düşünüldüğünde deallocate işlemi sonrası, var olmayan bellek alanlarını referans eder hale gelebilirler. Bu durumun oluşmaması için Rust söz konusu alanların ne kadar süre yaşayacağını bilmek istiyor. Böylece Dangling Pointer oluşmasının önüne geçmiş oluyoruz. Bunun üstüne ilk olarak derleyicinin de önerdiği üzere Player için gerekli lifetime parametrelerini ekleyerek evam edelim.

```bash
#[derive(Debug)]
struct Player<'a> {
    id: u32,
    nick: &'a str,
    country: &'a str,
    level: u16,
}
```

Lifetime parametrelerinde genel olarak a,b gibi harfler kullanılmakta ancak ideal olanı nesnelerin gireceği scope'ları düşünerek isimlendirmek. Örneğin buradaki değişkenler bir stream okuma sürecinde yer alsalardı buffer veya buf gibi bir isimlendirme daha doğru olabilirdi. Yazımına alışmak biraz zaman alsa da Rust derleyicisine yaşam ömrü ile ilgili bir ipucu vermiş oluyoruz. Bu değişiklikerden sonra programımıza clippy ile tekrar bakalım.

![viva_las_vegas_4.png](images/viva_las_vegas_4.png)

Hobaaa!!! İşler daha da kötüye gitti sanki:D Hata mesajlarını ve uyarıları okursak işimizin daha kolay olduğunu görebiliriz. Player nesnesindeki literal string alanları için lifetime belirttiğimiz anda, Player'ın kullanıldığı ne kadar scope varsa onlar için de kullanım sürelerini belirtmemiz gerekiyor. Bu örneğe göre new ve change_nickname fonksiyonları Player türü ile çalışıyor. Dolayısıyla bu fonksiyonların stack bellek bölgesinde kaldığı süre doğal olarak new ve change_nickname metotlarına ait parametreler için de geçerli olmalı. Öyle ki dışarıdaki bir Player nesnesi parametrelerle aynı süre boyunca hayatta kalsın. Bu bilgiler ışığında kodları aşağıdaki gibi değiştirerek devam edelim.

```bash
#[derive(Debug)]
struct Player<'a> {
    id: u32,
    nick: &'a str,
    country: &'a str,
    level: u16,
}

impl<'a> Player<'a> {
    fn new(id: u32, nick: &'a str, country: &'a str, level: u16) -> Self {
        Self {
            id,
            nick,
            country,
            level,
        }
    }
}

fn change_nickname<'a>(p: &'a mut Player<'a>, new_nickname: &'a str) -> &'a Player<'a> {
    p.nick = new_nickname;
    p
}
```

Dikkat edileceği üzere new ve change_nickname fonksiyonlarında lifetime bildirimleri kullanmakta. Buna göre new ile bir Player nesnesi örneklendiğinde derleyici onun için bir yaşam süresi biçiyor. change_nickname'e parametre olarak gelen Player değişkeni de bir lifetime bilgisi bekliyor. Hatta aynı lifetime bilgisi ikinci parametre olan new_nickname için de geçerli. Rust'ta kodlama yaparken çoğu zaman bu tip lifetime bildirimleri ile karşılaşmıyoruz. Nitekim gerek olmadığı hallerde biz açık bir şekilde belirtmesek bile Rust derleyicisi ilgili lifetime bildirimlerini eklemekte (Implicit Lifetimes) Şimdi kodumuzu tekrar clippy ile gözden geçirip çalışma sonuçlarına bir bakalım.

![viva_las_vegas_5.png](images/viva_las_vegas_5.png)

İstediğimiz sonuçları elde ettiğimizi ifade edebiliriz. Tekrar hatıralayım. String heap'de duran, UTF8 formatındaki bir veri tipidir ve bulunduğu yere erişebiliriz. Onun için heap'te bir yer ayrılır (allocation) &str ise bir parça dilimdir (slice type) Yani zaten var olan bir String'in bir parçasını işaret eder, çalışma zamanında herhangi bir allocation gerektirmez ve sabit uzunlukta olan &str yeniden boyutlandırılamaz. Performans açısından yeni String sahaları oluşturmak yerine dilimlerle çalışmak pek tabii daha iyidir lakin bunu yaptığımızda lifetime gibi konular da karşımıza çıkabilir. Özellikle stream şeklinde akan ağ paketlerinde içerikten üretilen metinsel alanlarda değişiklikler yapılması düşünülmüyorsa String kullanım maliyeti çok yüksek olacaktır. Böylece geldik bir Rust Pratiğinin daha sonuna. Her zaman olduğu gibi [örnek kodlara github reposu üzerinden erişebilirsiniz](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/viva_las_vegas). Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
