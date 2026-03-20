---
layout: post
title: "Rust Pratikleri - Value Moved Here"
date: 2022-05-22 09:00:00 +0300
categories:
  - rust
tags:
  - rust
  - memory-management
  - pointers
  - github
  - ownership
  - borrow-checker
---
Sıklıkla vurgulandığı üzere Rust programlama dili bellek yönetimi ve güvenliği konusunda son derece hassas kurallar içeriyor. Değişken sahipliği kuralları ve ödünç alma kontrolü (Ownership, Borrow Checker) olası bir çok bellek probleminin henüz derleme aşamasındayken önüne geçilmesini sağlıyor ancak dilin öğrenme eğrisini de oldukça dikleştiriyor (En azından ilk zamanlarda) Esasında C,C++ türevi sayabileceğimiz Rust'ın bir Garbage Collector mekanizması kullanmaması belleğin çalışma dinamiklerini daha iyi bilmemizi gerektiriyor. Ancak sanılanın aksin Rust'ın bir Garbage Collector mekanizması içermemesi bellek yönetimi yapmadığı anlamına gelmemeli. Nitekim Rust bellek yönetimi için Ownership, Resource Acquisition is Initialization (RAII), Borrow Checker, Lifetimes ve Smart Pointers gibi birçok enstrüman kullanmakta.

![moved_cover.png](/assets/images/2022/moved_cover.png)

Yine de Rust ile ilk kez tanışanların Stack ve Heap özelinde değişkenlerin nasıl tutulduğunu, kaynak tahsislerinin nasıl yapıldığını, hangi noktada verilerin bellekten atıldığını bilmeleri işleri kolaylaştırmak adına oldukça önemli. Bu sayede pek çok derleme zamanı hatasını anlamlandırmak mümkün hale geliyor (Özellikle Garbage Collector destekli bir dilden geliyorsanız) Bu yazıdaki amacımız Rust öğrenenlerin sıklıkla yakalandığı "Value borrowed here after move" hatasını bellek çalışma prensipleri kapsamında özetleyerek anlamaya çalışmak.

Başlamadan önce Rust'ın bellek yönetim sistemi hakkında birkaç kısa bilgi de verelim. Bir Rust programı çalıştığında işletim sistemince ona ayrılan bir sanal bellek (Virtual Memory) bölgesini kullanmaya başlıyor. Pek tabii Stack ve Heap bellek bölgeleri kullanılıyor. Derleme zamanında boyutu bilinen statik veriler ile fonksiyonlara ait çerçeveler (Function Frames), dile ait birincil tipler (primitive type olarak ifade edilen tamsayı, virgüllü sayı gibiler), veri yapıları (struct) ve işaretçiler (pointers) boyutu heap ile kıyaslandığında çok daha küçük olan (varsayılan hali ile 8Mb büyüklüğünde bir alandır) Stack bellek bölgesinde tutulmaktalar.

Dinamik özellikli olarak ifade edilen, yani boyutu derleme zamanında tam olarak kestirilemeyen (sanırım en güzel örnekleri String ve Vector veri yapılarıdır) veya çalışma zamanında değişmesi muhtemel verilerse çok daha geniş bir kullanım alanına sahip olan Heap üzerinde tutuluyorlar. Heap kısmının yönetimi için Rust sahiplik (ownership) kuramı kurallarını işletiyor. Bu arada sabit uzunlukta bir verinin Heap'e alınarak Stack üstünden referans edilmesi de mümkün. Bu noktada smart pointer olarak bilinen Box enstrümanının devreye girdiğini söyleyebiliriz.

İşletilebilir Rust programlarında mutlaka main fonksiyonu yer alır. Program çalıştığında main fonksiyonu için stack üstünde bir frame bloğu açılır. Esasında her fonksiyon çağrımında stack'de bir frame açılmaktadır. Çağırılan fonksiyonun içerdiği değişkenler, aldığı parametreler ve dönüş türü dahil kullandığı tüm argümanlar onun için açılan frame içerisinde tutulurlar. Stack'in LIFO (Last In First Out) ilkesine göre çalıştığını düşünürsek çağırılan bir fonksiyon Stack'te üst sıraya konulur (push fonksiyon çağrımı gibi düşünelim) Çok doğal olarak ilgili fonksiyon işleyişini tamamladığında yani ona ait scope sonlandığında Stack'ten alınır (pop olarak düşünelim) ve kullandığı argümanlar da bellekten atılır.

Tahmin edeceğiniz üzere main fonksiyonunun sonlanması ana sürecin de (main process) sonlanması demektir ve bu noktada programın Heap'te kalan verileri de yok edilir. Stack üstündeki bu çalışma mekaniği zaten otomatik olarak işletim sistemi tarafından yönetilir. Ancak bu Heap bölgesi için söz konusu değildir. Heap çok daha büyük bir alandır ve içerisinde yer alan dinamik veriler bu sahanın kolayca dolmasına sebebiyet verir. Özellikle kopyalanan veriler düşünülürse. Bu orantısız büyüme uygulamada yavaşlamalara da neden olur ki Rust'ın bellek yönetimi için kullandığı sahiplenme (Ownership) kuralları da burada işe yaramaktadır.

Biliyorum kafalar epeyce karıştı. Dilerseniz odağımızı daha fazla dağıtmadan "Value Moved Here" durumuna bakalım. İşe aşağıdaki kod parçasını ile başlayabiliriz.

```rust
fn main() {
    let player_name=String::from("Obi Wan");
    let name=player_name;
}
```

main fonksiyonu içerisinde player_name isimli String veri yapısından bir değişkenin kullanımı söz konusu. Dikkat edeceğimiz nokta name değişkenine yapılan atama. Aslında burada sahipliğin (ownership) taşındığını (moved) ifade edebiliriz. Gelin program çalışırken belleğin nasıl yönetildiğine kaba taslak bir bakalım.

![moved_1.png](/assets/images/2022/moved_1.png)

İlk olarak main fonksiyonu için Stack'te bir alan açılır. 1nci durumda player_name değişkeni için stack bellek bölgesinde bir tanımlama yapıldığını söyleyebiliriz. String bilindiği üzere verisini gösteren bir işaretçi türüdür. Veri yapısında heap'teki bellek alanını işaret eden bir referans, ayrılan kapasite ve uzunluk bilgileri yer alır. Şimdi bir alt satıra geçelim (Resmen hayali debug yapıyoruz. Keşke belleğin çalışırken ki röntgenini çekecek bir yol bilsem. Bilsen yorumlara yazsın.)

![moved_2.png](/assets/images/2022/moved_2.png)

2nci durumda player_name'in stack üstündeki bilgileri name isimli yeni değişkene taşınır. name isimli değişken de heap üstündeki aynı veri bölgesini işaret etmektedir. Bu işlem taşıma (move) operasyonu olarak ifade edilebilir. Atama sonrasında (veya birlikte) aşağıdaki görselde yer alan 3ncü durum oluşur.

![moved_3.png](/assets/images/2022/moved_3.png)

Rust'ın sahiplik kurallarına göre bir verinin t anında tek bir sahibi olabilir. Dolayısıyla taşıma işlemi sonrasında player_name artık kullanılabilir değildir, yok edilir. Onun verisini artık name isimli değişken işaret etmektedir. İşte bu yüzden aşağıdaki kod parçası hata verecektir.

```rust
fn main() {
    let player_name=String::from("Obi Wan");
    let name=player_name;
    println!("User name is '{}'",player_name)
}
```

Nitekim player_name sahipliğini name isimli değişkene taşıdıktan sonra yine ona erişilmeye çalışılmıştır. Sonuçta aynı veriye işaret eden iki stack değişkeni olması hem bellek bölgesini hovardaca kullanmak hem de herhangi bir deallocate işlemi vuku bulduktan sonra double free gibi sorunların ortaya çıkmasına neden olacaktır. Ayrıca bu tip güvenliğini bozan bir durumdur.

![moved_4.png](/assets/images/2022/moved_4.png)

Tekrar çalışan eski kod parçasına dönelim ve her şey yolunda giderse işleyiş nasıl devam eder kısaca bakalım.

![moved_5.png](/assets/images/2022/moved_5.png)

Scope sonuna gelindiğinde, bir başka deyişle artık main fonksiyonundan dönüldüğünde name değişkeni silinecektir. Ardından,

![moved_5_2.png](/assets/images/2022/moved_5_2.png)

String veri türü için bellekten atılma işlemi icra edilir. Bu drop trait'inin uygulandığı türler için geçerli bir operasyondur. Yani heap üstünde kalan verinin yok edilmesi söz konusudur. Son olarak işletim sistemi stack çerçevesinde kalan main fonksiyonunu da kaldırır. Onu da aşağıdaki temiz sayfa ile görselleştirebiliriz.

![moved_5_3.png](/assets/images/2022/moved_5_3.png)

Sanıyorum bu şekilde görselleştirerek anlattığımızda konuyu anlamak çok daha kolay. Öyleyse gelin ikinci bir senaryoya daha bakalım;) Bu sefer String değişkenin bir fonksiyona parametre olarak geçilmesi söz konusu.

```rust
fn main() {
    let player_name = String::from("Obi Wan");
    print_with_me(player_name);
}

fn print_with_me(input: String) {
    println!("User name is '{}'", input.to_uppercase());
    let something = input;
}
```

Önceki örnekten farklı olarak main içerisinde tanımlanan player_name değişkeni print_with_me isimli bir fonksiyona parametre yoluyla aktarılmakta. Ayrıca print_with_me fonksiyonu içerisinde parametre olarak gelen input değişkeninin, something isimli bir başka değişkene atanması da söz konusu. Burada birkaç move hareketi olduğunu ifade edebiliriz. Programın bellek üzerindeki akışını yine çizimlerle ifade etmeye çalışalım.

![moved_6.png](/assets/images/2022/moved_6.png)

main fonksiyonu için stack üzerinde bir çerçeve açıldıktan sonra içerisine de ilk satır itibariyle player_name isimli String değişken konur. player_name, heap'teki "Obi Wan" metin katarının olduğu bellek sahasını işaret edecek şekilde bir pointer, kapasite ve uzunluk bilgisini taşıyan veri yapısı olarak konumlanır. Şimdi kodu bir alt satıra geçirelim.

![moved_7.png](/assets/images/2022/moved_7.png)

Burada bir fonksiyon çağrımı söz konusu olduğundan stack üstünde print_with_me için yeni bir çerçeve açılır (push işlemini hatırlayalım) Ancak daha da önemlisi fonksiyona player_name değişkeninin input ismiyle alınmasıdır. Bu da bir taşıma işlemi (move) anlamına gelir. Dolayısıyla print_with_me için açılan stack çerçevesi içerisine input isimli alan eklenirken, taşıma sebebiyle main çerçevesinde yer alan player_name artık geçersizdir. Heap üzerindeki veri bölgesinin şimdiki sahibi print_with_me fonksiyonundaki input isimli değişkendir. Kod artık print_with_me fonksiyonu içerisinde akmaktadır. Burada println makro çağrısını göz ardı edersek devam eden kısımda input değişkeninin bu kez something isimli başka bir değişkene atanması işlemi yapılmaktadır. Yani aşağıdaki gibi bir durum oluşacaktır.

![moved_9.png](/assets/images/2022/moved_9.png)

Bu sefer print_with_me için ayrılan çerçeve içerisinde bir move operasyonu gerçekleşir. Heap üstündeki veri alanının yeni sahibi bu atmaya göre something isimli değişkendir. Atama sonrası input isimli değişken yine kullanılamaz hale gelir. Tam bu noktada durup ikinci örnek kodun aşağıdaki gibi değiştirildiğini düşünelim.

```rust
fn main() {
    let player_name = String::from("Obi Wan");
    print_with_me(player_name);
    println!("{}", player_name);
}

fn print_with_me(input: String) {
    println!("User name is '{}'", input.to_uppercase());
    let something = input;
}
```

main fonksiyonunda print_with_me çağrısından sonra player_name değişkenini kullanmak istiyoruz. Lakin player_name çoktan devre dışı kalmıştır. Dolayısıyla bir taşıma ihlali bir başka deyişle "value borrowed here after move" durumu söz konusudur. Bu durumu aşağıdaki çizelgeyle görselleştirelim.

![moved_8.png](/assets/images/2022/moved_8.png)

Burada dikkat edilmesi gereken bir husus daha var. Dikkat edileceği üzere player_name değişkeninin tanımlandığı yerle ilgili olarak Rust derleyicisinin önemli bir yorumu bulunuyor; "move occurs because 'playername'has type 'String', which does not implement the 'Copy'trait"... Burayı anlamamız oldukça önemli. Şu ana kadar anlattığımız taşıma halleri String veri türü üzerinden ele alındı. String, heap alanını kullandığı için sahiplik ve buna bağlı olarak taşıma ihlallerine yakalanıyoruz. Yani C# tarafından gelenler için değer türü (value type) olarak ifade edebileceğimiz int, float gibi sabit uzunlukta olduğu bilinen ve stack üstünde yaşayan değişkenler için aynı durum söz konusu değildir. Nitekim bu türler varsayılan olarak kopyalama yolu ile taşınabilirler. Bu sebepten copy trait'inin uygulandığı hallerde ya da sadece referansın (& ile) taşındığı durumlarda "Value moved here" ihlali oluşmamamlıdır (Bunun doğru olup olmadığını ispat etmek sizin göreviniz olsun)

Şimdi yukarıdaki ihlali geri alıp programın kalan akışında belleğin nasıl bir hareket içerisinde olduğuna bakalım.

![moved_10.png](/assets/images/2022/moved_10.png)

Kod akışı şimdi print_with_me fonksiyonuna ait scope'un sonlandığı yerde. Stack'te kalan something değişkenini takiben doğal olarak Heap üstündeki veri alanı da temizlenir.

![moved_11.png](/assets/images/2022/moved_11.png)

Ardından print_with_me fonksiyonu için açılmış olan stack çerçevesi silinir ve son olarak da main için ayrılmış olan bölüm sonlanır.

![moved_12.png](/assets/images/2022/moved_12.png)

ve yine tertemiz bir bellek sayfası ile karşı karşıya kalırız:)

Ele aldığımız örnek anlamsız olsa da yukarıda oluşan taşıma ihlalini örneğin print_with_me fonksiyonundan dönüş sağlayarak aşabiliriz. Yani aşağıdaki kod parçasında olduğu gibi.

```rust
fn main() {
    let player_name = String::from("Obi Wan");
    let changed_name = print_with_me(player_name);
    println!("{}", changed_name);
}

fn print_with_me(input: String) -> String {
    let something = input.to_uppercase();
    println!("User name is '{}'", something);
    something
}
```

Hatta something değişkenini daha doğru bir yerde konumlandırdık diyebilirim. Bu kod parçası sorunsuz bir şekilde derlenip çalışacaktır.

![moved_13.png](/assets/images/2022/moved_13.png)

Şimdi gelin biraz önce bahsettiğimiz stack odaklı türler için benzer senaryonun nasıl sonuçlanacağına bir bakalım.

```rust
fn main() {
    let lucky_number = 23;
    do_something(lucky_number);
    println!("but original number is {}", lucky_number);
}

fn do_something(input:i32) {
    let something = input+1;
    println!("New number is '{}'", something);
}
```

Örnekte lucky_number isimli 32 bitlik bir tam sayının kullanımı söz konusu. Bu değişken do_something fonksiyonuna parametre olarak yollandıktan sonra o fonksiyon içerisinde değiştiriliyor. String kullanılan örnekten farklı olarak lucky_number main fonksiyonunda, do_something çağrısından sonra da kullanılabilir. Nitekim stack üzerine açılan fonksiyon çerçevesi sınırları içerisinde yaşamaktadır. Bir başka deyişle Heap üzerindeki bir bölgeyi referans etmemektedir. Sabit uzunlukta, kapladığı alan derleme zamanında belli olan bir tür kullanılmaktadır ve bu tür Copy trait'ini de uygulamaktadır. Dolayısıyla değişken değeri fonksiyona kopyalanarak taşınmaktadır. Buna göre bir sahiplik ihlali söz konusu değildir. Zaten herkes kendi verisinin sahibidir. İşte çalışma zamanı çıktısı.

![moved_14.png](/assets/images/2022/moved_14.png)

Bu yeni kodun bellekteki çalışma modelini çizmeye çalışarak konuyu daha da iyi bir şekilde pekiştirmeye çalışabilirsiniz;) Yazıyı sonlandırmadan önce ikinci senaryomuza geri dönmek istiyorum. Sizce String türdeki player_name isimli değişkeni print_with_me isimli fonksiyonda kullanıp something üstünden döndürmek yerine farklı bir şekilde yollayıp main fonksiyonunda oluşan taşıma hatasının önünce geçebilir miyiz?;) Bunu bir düşünün, araştırın ve çözümünüz olursa lütfen yorum kısmında paylaşın. Böylece geldik bir Rust pratiğimizin daha sonuna. Bu örnek kısa olduğu için github üzerinde bir örneğini oluşturmadım ancak Rust dili ile ilgili çalışmalarıma [rust-farm](https://github.com/buraksenyurt/rust-farm) üzerinden ulaşabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
