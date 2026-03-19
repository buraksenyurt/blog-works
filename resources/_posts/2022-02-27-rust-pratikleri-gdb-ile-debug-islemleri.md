---
layout: post
title: "Rust Pratikleri - GDB ile Debug İşlemleri"
date: 2022-02-27 09:00:00 +0300
description: "Rust ile ilgili öğretilerde bellek yönetimi konusunu incelerken fonksiyon ve değişkenlere ait kapsamların bellekte nasıl açıldığını görmek için GNU Debugger'dan(kısaca GDB) yararlanıldığını gördüm. Kodu debug etmek deyince insanın aklına Visual Studio gibi gelişmiş IDE'lerin kolaylıkları geliyor ve bu nedenle GDB ilkel bir araç gibi görünebilir. Yine de rust kodunu terminalden adım adım işletmek ve bellek üzerindeki konumlandırmaları görmek(stack açılımlarını izleyip pointer'ları analiz etmek gibi) adına son derece faydalı bir araç. Rust dilinde ilerlemek isteyenlerin bilmesi ve kullanması gereken bir yardımcı. Öncelikle üzerine çalıştığım Ubuntu platformuna yüklemem lazım. Bu arada GDB ile ilgili detaylar için şu adrese bakılabilir."
categories:
  - rust
tags:
  - rust
  - bash
  - csharp
  - http
  - memory-management
  - pointers
  - debugging
  - visual-studio
  - race-condition
---
# Rust Pratikleri - GDB ile Debug İşlemleri
![gdb.png](/assets/images/2022/gdb.png)

Rust dilinin en güçlü olduğu yer etkili bellek yönetimi ve olası kaosların önüne herhangi bir garbage collector veya başka bir unsura ihtiyaç duymadan geçebilecek kural setleri barındırmasıdır. Özellikle Memory Leak, Double Free, Data Race gibi C, C++ dillerinde sıklıkla rastlanan durumların oluşmaması için basit kurallar barındırır. Bu kurallar ilk başlarda rust öğrenenleri epey zorlar fakat bir kez alışılınca her şey çok daha net ve berrak hale gelir. Bellek yönetimi denilince içeride neler oluyor bitiyor görmek de önemlidir. Fonksiyonlar birer kapsam olarak Stack'e yığılır, çeşitli veri türleri (String gibi) heap'e açılıp pointer alır, kapsamlar sonlandığında bir şeyler olur vs

Rust ile ilgili öğretilerde bellek yönetimi konusunu incelerken fonksiyon ve değişkenlere ait kapsamların stack ve heap bölgelerine nasıl açıldığını görmek için GNU Debugger'dan (kısaca GDB) yararlanılabilir. Kodu debug etmek deyince insanın aklına Visual Studio gibi gelişmiş IDE'lerin kolaylıkları geliyor ve bu nedenle ilk kez karşılaşanlar için GDB ilkel bir araç gibi görünebilir elbette. Ancak rust ile yazılmış programları terminalden adım adım işletmek ve bellek üzerindeki konumlandırmaları görmek (stack açılımlarını izleyip pointer'ları analiz etmek gibi) adına son derece faydalı bir araçtır. Rust dilinde ilerlemek isteyenlerin bilmesi ve kullanması gereken bir yardımcı olduğunu düşünüyorum. Tabii her şeyden önce onu üzerinde çalıştığım Ubuntu platformuna yüklemem gerekiyor. Bu arada GDB ile ilgili detaylar için [şu adrese](http://www.gdbtutorial.com/) bakılabilir.

```bash
sudo apt-get update
sudo apt-get install gdb

gdb --version
```

Eğer her şey yolunda giderse aşağıdaki gibi versiyon numarasını görebilmeliyiz.

![debugging_1.png](/assets/images/2022/debugging_1.png)

Gelelim örnek kodlara. Debugger kullanımını basit seviyede deneyimlemek için bir rust projesi oluşturup ilerleyelim.

```bash
cargo new debugging
```

main.rs içeriğini aşağıdaki kod parçasında olduğu gibi geliştirebiliriz.

```rust
fn main() {
    let mut calderon = Player {
        id: 1,
        name: String::from("Hoze Kalderon"),
        level: 78,
    };
    increase_level(&mut calderon);
    dbg!(calderon.level);
    decrease_level(&mut calderon);
    dbg!(calderon.level);
}

fn increase_level(p: &mut Player) {
    p.level += 10;
}
fn decrease_level(p: &mut Player) {
    let rate = 10;
    p.level -= rate;
}

#[allow(dead_code)]
struct Player {
    id: u16,
    name: String,
    level: u16,
}
```

Player isimli bir veri yapısı ve onun level alan değerini artırıp azaltan iki fonksiyon kullanmaktayız. Fonksiyonlara Player nesne örneğini referans olarak geçiyoruz (bir başka deyişle fonksiyon kapsamlarına onu ödünç veriyoruz - borrowing) ve dbg! makrosunu kullanarak debug ekranına bilgi yazdırıyoruz. Kodun uygunluğunu clippy ile iyileştirdikten sonra çalıştığından emin olmalı ve daha da önemlisi debug işlemleri için build etmeliyiz. İşte kullanacağımız terminal komutları.

```bash
cargo new debugging
cd debugging
cargo clippy
cargo run

# kodun çalıştığından emin olduktan sonra build etmeliyiz
cargo build
```

Artık kodu adım adım debug etmeye başlayabiliriz. GDB aracının belli başlı komutları var. İzleyen terminal komutlarına ait yorum satırlarında kullanımlarına ait kısa bilgiler bulabilirsiniz.

```bash
# Programa ait binary'yi debug modda açalım
gdb debugging
# Çalıştığını görelim
run
# ve ilk satırından itibaren kod içeriğine bir bakalım
list

# ardından örneğin increase_level ve decrease_level fonksiyonlarına birer breakpoint koyalım
b increase_level
b decrease_level

# kodu çalıştıralım
r

# Artık breakpoint noktalarında bir takım bilgilere bakabiliriz.
# Örneğin o andaki local değişkenlere ve argümanlara bakalım
info locals
info args

# Kodu bir adım ilerletelim
n

# Aynı bilgilere tekrar bakalım ve hatta stack bellek bölgesine bir göz atalım.
bt
info locals
info args

# Hatta pointer olarak gelen değişkenlerin içeriklerini şöyle görebiliriz
print *p

# Bir sonraki breakpoint noktasına geçmek için c komutunu kullanırız
c

# stack üzerindeki scope'ları görmek için yine bt'den yararlanabiliriz
bt

# debugger'dan çıkmak içinse aşağıdaki komutu kullanırız.
q

# Bu arada minik bir ipucu bırakalım. Ekran çok kalabalıklaştığında
# muhtemelen silmek isteyeceksiniz. Ctrl + L işinizi görecektir.
```

Tabii bu komutları denerken ekran görüntüsü aşağıya doğru uzayıp gidebilir:) Neyse ki sağdaki dikey monitör bana epeyce yardımcı oldu. Yine de sonuçları iki parça halinde paylaşacağım. İlk kısımda gdb aracını başlatıp kodun içeriğini gösteriyoruz. Bu arada binary dosyanın olduğu klasöre gittiğimize dikkat edelim.

![debugging_2.png](/assets/images/2022/debugging_2.png)

Devam eden kısımda ise kalan komutların verdiği sonuçlarını görmekteyiz.

![debugging_3.png](/assets/images/2022/debugging_3.png)

Bu kısmı yorumlamak oldukça önemli. Kodumuzdaki fonksiyonlar Player verisini referans olarak ödünç alıp kullanmaktalar. Bu nedenle girdiğimiz fonksiyonlarda birer pointer görmekteyiz. Pointer adresi ve hatta kullandığı String değişkeninki değişmiyor elbette. Dikkat çekici bir diğer nokta da fonksiyonlara parametre olarak gelen Player nesnesinin işaret ettiği veri yapısı. Dikkat edileceği üzere String olarak tasarladığımız name değişkeni String veri yapısının tasarımı gereği heap bölgesindeki içeriği işaret etmekte. Diğer yandan String türünün kendisi esasında bir Smart Pointer'dır. Yani scope dışına çıkıldığı anda otomatik olarak heap içeriği deallocate edilir. GDB aracını kullanarak özellikle Smart Pointer gibi enstrümanların işleyişini anlamak çok daha kolaydır. Bunun için örneğimize aşağıdaki fonksiyonu eklediğimizi düşünelim.

```csharp
fn change_level(p: &mut Player) {
    let level = Box::new(90);
    p.level = *level;
}
```

Fonksiyon kendisine referansı verilen Player nesnesinin yine level isimli değerini değiştirmekte. Ancak yeni level bilgisinin kasıtlı olarak bir Smart Pointer tarafından tutulduğuna dikkat edelim. Box türünden bu değişken heap alanında duracak şekilde ilkel bir tamsayı verisi taşımakta. İlgili fonksiyonu main içerisinde aşağıdaki gibi kullanabiliriz.

```rust
fn main() {
    let mut calderon = Player {
        id: 1,
        name: String::from("Hoze Kalderon"),
        level: 78,
    };
    increase_level(&mut calderon);
    dbg!(calderon.level);
    decrease_level(&mut calderon);
    dbg!(calderon.level);
    change_level(&mut calderon);
    dbg!(calderon.level);
}
```

Sadece konuyu değerlendirmek için level isimli bir smart pointer kullanıyoruz. Smart Pointer'lar scope sonlandığında otomatik olarak heap'ten atılırlar ki bu özellikle silmeyi unuttuğumuz pointer'ların oluşturacağı Memory Leak durumunun oluşmamasını garanti eder. Gerçekten böyle olup olmadığını anlamanın (yani fonksiyon sonlanıp scope dışına çıkıldığında pointer'ın işaret ettiği bellek bölgesinde bir değer kalmadığını görmenin) bir yolu kodu debug ederken fonksiyon çağrısı tamamlandıktan sonraki fotoğrafa bakmaktır. Öyleyse tekrardan terminale dönüp debug işlemlerine başlayalım.

```bash
gdb debugging
# breakpoint'i ekleyelim
b change_level
# programı çalıştıralım(run)
r
# Birkaç satır ilerleyelim
n
n
n
# change_level fonksiyonu içinde tanımlanan local değişkenlere bir bakalım
info locals
#pointer değerini okuyalım (Tabii siz denerken adres farklı olacaktır)
x /d 0x5555555a5af0

# Kodu ilerletip scope'u sonlandıralım. Yani fonksiyon işleyişini tamamlayalım.
n
n
# Şimdi tekrar aşağıdaki komutu çalıştıralım
x /d 0x5555555a5af0

# sonuç 0 olmalı. Bu Smart Pointer'ın söylediği üzere ilgili bellek bölgesinin kaldırıldığı anlamına gelir.
```

Çalışma zamanı sonuçları aşağıdaki gibidir.

![debugging_4.png](/assets/images/2022/debugging_4.png)

Dikkat edileceği üzere fonksiyon dışına çıkıldığında ilgili adres değeri 0 olarak elde edilmiştir. Smart Pointer'ın çalıştığının bir nevi ispatı olarak düşünebiliriz. Tabii büyük projelerde ve kalabalık kod parçalarında GDB ile debug işlemleri çok kolay olmayabilir. Hatta sağlıklı loglar daha çok işe yarayabilir. Yine de iç dinamikleri öğrenme aşamasındayken bu debugger'ı kullanmak bence oldukça önemli. Böylece geldik Rust Pratiklerinde bir bölümün daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.