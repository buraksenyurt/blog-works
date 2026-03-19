---
layout: post
title: "Rust Pratikleri - HTTP Sunucusu Yazmak/Yazmaya Çalışmak"
date: 2022-03-20 09:00:00 +0300
description: "Bir HTTP sunucusu yazmaya ne gerek var diyebilirsiniz. Öyle düşünmeyin. Bir programlama dilini öğrenmenin en iyi yolu, var olan yapıları o dille yazmaya çalışmaktır. Hangi dil ya da platform olursa olsun ortada dolaşan yüzlerce HTTP server zaten var. Ancak nasıl çalıştıklarını anlamak için de yine yeniden yazmakta yarar var. Bugünkü pratiğimizde Rust dilini kullanarak bir HTTP server nasıl yazılabilir incelemeye çalışacağız. Esasında sadece minik bir başlanıç yapacağız. Nitekim bir HTTP sunucusunun görevleri çok geniş olabilir. Sonuçta IIS Server yazmayacağız ancak genel felsefeyi öğreneceğiz."
categories:
  - rust
tags:
  - rust
  - bash
  - csharp
  - xml
  - json
  - http
  - github
  - ownership
---
# Rust Pratikleri - HTTP Sunucusu Yazmak/Yazmaya Çalışmak
![crayz_server.png](/assets/images/2022/crayz_server.png)

Bir HTTP sunucusu yazmaya ne gerek var diyebilirsiniz. Öyle düşünmeyin. Bir programlama dilini öğrenmenin en iyi yolu, var olan yapıları o dille yazmaya çalışmaktır. Hangi dil ya da platform olursa olsun ortada dolaşan yüzlerce HTTP server zaten var. Ancak nasıl çalıştıklarını anlamak için de yine yeniden yazmakta yarar var. Bugünkü pratiğimizde bir HTTP server Rust programlama dili ile nasıl yazılabilir incelemeye çalışacağız. Esasında minik bir başlanıç yapacağız. Nitekim bir HTTP sunucusunun görevleri çok geniş olabilir.

Tipik bir HTTP sunucusuna talep yollandığında bu belli bir protokol ve standarda göre gerçekleşir. HTTP protokolünün gereği olarak geçerli bir metot ile ilintili paketler söz konusudur. Sunucu bir IP adresi ve port üstünden gelen mesajları dinler ve gerekli çıktıları hazırlar. Sunucu tarafından üretilen içerik kimi zaman bir web sayfası kimi zaman da bir JSON çıktısıdır. İşin aslı gelen veriyi yorumladıktan sonra nasıl bir şeyler döndüreceğimiz bize bağlıdır. Lakin endüstriyel dinamikler göz önüne alındığında uç tarafların yorumlayacağı standartlar bellidir. HTML, JSON, XML, PDF vs gibi çeşitli türden tipler (Content Types) söz konusu olabilir. Bu pratikteki temel amacımız TCP paketi olarak gelen binary içeriği HTTP mesajı olarak yorumlamaya çalışmak olacak. Ele almaya çalışacağımız HTTP içeriklerine bir bakalım.

```text
Örnek bir GET paketi

GET /query?word=red HTTP/1.1
Host: localhost:5555
User-Agent: curl/7.68.0
Accept: *//*

Örnek bir POST paketi

POST /movies/ HTTP/1.1
Host: localhost:5555
User-Agent: curl/7.68.0
Accept: *//*
Content-Type: application/json
Content-Length: 36

{"message":"only one ping Vaseley."}
```

Tabii PUT, DELETE, PATCH, HEAD ve daha bir çok HTTP metodu mevcut bildiğiniz üzere. Ancak en azından gelen GET ve POST paketlerini yakalasak girizgah için yeterli. HTTP Get paketlerinde işimiz nispeten kolay. İlk satırı okuyup ayrıştırdığımızda GET ifadesini, takip eden kısımda path ve varsa querystring bölümlerini, son olarak da protokol versiyonunu öğrenmek mümkün. POST işlemi söz konusu olduğunda da mesajın gövde kısmına inip JSON içeriğini yakalamak kafi.

Örnek uygulamamızda gelen paketi bir veri yapısında (struct) tutacağız. İçerik stream üstünden binary olarak gelecektir. Rust açısından düşünecek olursak u8 tipinden oluşan bir veri dilimi olacağını söyleyebiliriz. Bu içeriği built-in trait tiplerini de kullanarak uygun veri modeline dönüştürebiliriz. Trait'leri kullanmak adına iyi bir pratik olur. Şimdi hiç vakit kaybetmeden örneğimizi geliştirmeye başlayalım.

```bash
cargo new crayz_server
cd crayz_server

# Genel tipleri ayrı bir modül içinde toplayalım
touch src/lib.rs
```

O kadar çılgın bir sunucu yazacağız ki HTTP Get ve Post paketlerini yorumlayacak:P (Bu arada örnek kodların tamamına [github reposundan](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/crayz_server) erişebilirsiniz) Kütüphanemizdeki enstrümanları farklı modüller içerisinde barındırabiliriz. Sunucu ile ilgili olanları server, http katmanı ile ilgili olanları da http modülünde tutabiliriz.

> Uygulamamızda hata tiplerimiz ve loglama için bazı yardımcı Rust kütüphaneleri kullanmaktayız. log, env_logger ve thiserror. İlgili paketleri toml dosyasındaki dependencies kısmına eklemeyi unutmayın.
> [dependencies]
> log="0.4.14"
> env_logger = "0.9.0"
> thiserror="1.0.30"

İlk olarak ihtiyacımız olan temel veri yapılarını yazarak kodlamaya başlayalım. Bir talebi Request isimli veri yapısı olarak tasarlayabiliriz. Sonuçta binary olarak gelen içeriğin kod tarafında anlamlı bir nesne olarak dolaşması idealdir. Gelen talebin dönüştürülmesi sırasında bir takım problemler de söz konusu olabilir. Örneğin encoding geçersiz olabilir, HTTP metodu yanlıştır ya da protokol geçersizdir vs. Bu tip sıkıntılar bir tarayıc üstünden veya curl gibi bir komut satırı aracından geliniyorsa nispeten zor ortaya çıkar ancak tedbir almakta yarar vardır. Üstelik basit bir Error tipini de nasıl kullanacağımızı görmüş oluruz. Söz konusu hata tipi için aşağıdaki gibi bir Enum türü kullanabiliriz.

```bash
#[derive(Debug, Error)]
pub enum RequestError {
    #[error("Paket geçersiz")]
    Invalid,
    #[error("Geçersiz HTTP komutu")]
    Command,
    #[error("Protokol geçersiz")]
    Protocol,
    #[error("Sorunlu encoding")]
    Encoding,
    #[error("HTTP ile uyumlu değil")]
    NotCompatible,
}
```

HTTP metotlarını Command isimli bir enum ile temsil edebiliriz. Hatta bir String'i bu veri yapısına dönüştürmek için FromStr trait'ini söz konusu veri yapısı için yeniden yazabiliriz. Aynen aşağıda görüldüğü gibi.

```bash
#[derive(Debug, PartialEq)]
pub enum Command {
    Get,
    Post,
    Put,
    Delete,
}

/// string'ten Command elde etme davranışını uygular
impl FromStr for Command {
    type Err = RequestError;

    /*
    from_str trait'ini Command veri yapısı için yeniden programladık.
    Böylece bir string'i ele alıp uygun Command nesnesini elde edebiliriz.
    Geçerli bir command nesnesi değilse de RequestError döndürmekteyiz.
    Bu fonksiyonu Request veri yapısında ele aldığımız TryFrom trait içerisinde kullanacağız.
     */
    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "GET" => Ok(Self::Get),
            "POST" => Ok(Self::Post),
            "DELETE" => Ok(Self::Delete),
            "PUT" => Ok(Self::Put),
            _ => {
                error!("Geçersiz bir metot geldi.");
                Err(RequestError::Command)
            }
        }
    }
}
```

Enum türü olarak tasarlanan Command dört temel HTTP metodunu temsil etmekte. Debug dışında PartialEq tarit'ini de devralmakta. Kodun ilerleyen kısımlarında bir Command değişkeninin Post olup olmadığına bakacağız. Bu karşılaştırma için enum sabitinin PartialEq davranışını taşıması gerekiyor. Asıl öğretici kısım from_str fonksiyonu. Parametre olarak gelen string literal değerine göre uygun bir Command nesnesi veya RequestError ile hata bilgisi dönüyor. Bu trait'i yeniden programladığımız için artık aşağıdaki gibi bir ifade yazmamız pekala mümkün.

```csharp
let c = Command::from_str("GET")?;
```

Taleplere ait method ve path bilgisini de Reuqest isimli veri yapısında tutabiliriz. Bu struct'ı ve ilgili fonksiyonlarını aşağıdaki gibi yazarak devam edelim.

```csharp
/// HTTP Request içeriğini tutar.
pub struct Request<'a> {
    pub method: Command,
    pub path: &'a str,
    pub body: &'a str,
}

impl<'a> Request<'a> {
    /// Yeni bir HTTP Request oluşturmak için kullanılır.
    pub fn new(method: Command, path: &'a str, body: &'a str) -> Self {
        Request { method, path, body }
    }
}

impl<'a> Display for Request<'a> {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{:?}, {}, {}", self.method, self.path, self.body)
    }
}
```

Elimizde Request, RequestError ve Command nesneleri var. Şunu da biliyoruz ki TCP dinleyicisi kendisine gelen byte array'leri yakalıyor. Byte array'den kastımız esasında 8 bitlik işaretsiz sayılardan oluşan bir dizi. Peki u8 türünden bir veri dilimini Request türüne nasıl dönüştürebiliriz? Bu her zaman doğru bir dönüşüm olmayacaktır ama denettirebiliriz. Bunun için TryFrom trait'i biçilmiş kaftan. Request veri yapımız için onu aşağıdaki gibi uyguladığımızı düşünelim.

```csharp
impl<'a> TryFrom<&'a [u8]> for Request<'a> {
    type Error = RequestError;

    fn try_from(value: &'a [u8]) -> Result<Request<'a>, Self::Error> {
        /*
            Pattern matching kullanımı yerine ? operatörü ile işi kısaltabiliriz.

            from_utf8 fonksiyonu eğer gelen parametreyi çözümleyemezse ParseError verir.
            or fonksiyonunda, ParseError olması halinde kendi Encoding error nesnemizi
            döndüreceğimizi belirtiyoruz.

            ? operatörü encoding sorunu yoksa, çözümlenmiş içeriğin package nesnesine
            alınmasını sağlar.
        */

        let package = str::from_utf8(value).or(Err(RequestError::Encoding))?;

        /*
           Gelen HTTP paketi satır satır akacaktır. Örneğin aşağıdaki gibi,

           POST /movies/ HTTP/1.1
           Host: localhost:5555
           User-Agent: curl/7.68.0
           Accept: *//*
           Content-Type: application/json
           Content-Length: 36

           {"message":"only one ping Vaseley."}

           ya da

           GET /query?word=red HTTP/1.1
           Host: localhost:5555
           User-Agent: curl/7.68.0
           Accept: *//*

           Satır bazında gelen isteği ayrıştırıp örneğin ilk satırdan HTTP metodu,
           path, query string son kısımdan JSON content vs almamız mümkün.
        */

        // Gelen içeriği satır bazında bir vector içinde topluyoruz.
        let parts: Vec<&str> = package.split('\n').collect();
        for p in &parts {
            info!("Part -> {}", p);
        }

        let first_row = parts[0];
        let cmd: Vec<&str> = first_row.split(' ').collect();

        /*
            Eğer sadece HTTP paketlerini ele alacaksak ilk satırda en azından

            GET /authors/ HTTP/1.1

            benzeri bir içerik olmalı.
            Dolayısıyla ilk satırın split edilmiş hali 3 eleman olmalı.
        */
        if cmd.len() != 3 {
            return Err(RequestError::Invalid);
        }

        let protocol = cmd[2];
        if !protocol.contains("HTTP") {
            return Err(RequestError::Protocol);
        }

        // from_str trait'ini yukarıda Command veri yapısına uyarlamıştık
        let c = Command::from_str(cmd[0])?;
        // Http metodunun Post olması halinde JSON içeriğini almayı da deneyebiliriz.
        let body = match c {
            Command::Post => parts[parts.len() - 1].trim(),
            _ => "",
        };
        Ok(Self::new(c, cmd[1], body))
    }
}
```

Buraya kadar yaptıklarımızla aslında byte byte gelen bir paketi anlamlı bir HTTP talebine kolaylıkla dönüştürebiliriz. En azından HTTP Get ve POST paketleri için çalışır bir sistem söz konusu olduğunu ifade edebilirim. Neden bu tip bir dönüşüme ihtiyaç duyduğumuzu sorgulayabilirsiniz. Sonuçta crayz_server bir HTTP sunucusu. Dolayısıyla kendisine gelen HTTP isteklerini dinlemekte. Bu istekler sisteme birer byte içeriği olarak girmekte. Sistem içerisindeki diğer enstrümanların onu kullanabilmesi için Rust'ın anladığı şekle dönüştürülmesi lazım. Hatta bir framework geliştiriyorsak da bu böyle olmalı. Onu kullanan yazılımcı için anlamlı veri yapıları ve fonksiyonlar sunabilmeliyiz. Yazılımcı anlayamayacağı byte serileri ile uğraşmak yerine Command::Get, Request::GetContent gibi enstrümanları kullanabilmeli.

Bu felsefe aktarımından sonra örneğimize geri dönelim. Artık paketleri dinleyen sunucu tarafını inşa edebiliriz. Bunun için gerekli yapıları server isimli modülde konuşlandırabiliriz.

```rust
/// HTTP Sunucu motoru ile ilgili çekirdek fonksiyonellikleri barındırır.
pub mod server {
    use crate::http::request::Request;
    use log::{error, info};
    use std::fmt::{Display, Formatter};
    use std::io::Read;
    use std::net::TcpListener;

    /// Sunucu bilgilerini taşıyan veri yapısı.
    pub struct Server<'a> {
        root: &'a str,
        port: u16,
        alias: &'a str,
    }

    impl<'a> Display for Server<'a> {
        fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
            write!(f, "({})->{}:{}", self.alias, self.root, self.port)
        }
    }

    impl<'a> Server<'a> {
        /// Yeni bir sunucu nesnesi örnekler.
        pub fn new(root: &'a str, port: u16, alias: &'a str) -> Self {
            Server { root, port, alias }
        }

        fn address(&self) -> String {
            format!("{}:{}", &self.root, &self.port)
        }

        /// Sunucuyu dinleme modunda başlatır
        pub fn run(self) {
            /*
               Tipik bir HTTP sunucusu başlatıldığında sonsuz bir döngüde talep dinler.
               Dolayısıyla run fonksiyonu sonlandığında self ile ifade edilen ve
               sahiliği(ownership)'i alınan Server nesnesinin deallocate edilmesinde yarar vardır.
               Bu sebepten &self yerine self kullandık.
            */
            info!("{} başlatılıyor...", self.address());
            /*
                Sunucuyu hazırlamak için TcpListener'ın bind fonksiyonu kullanılır.
            */
            let listener = TcpListener::bind(&self.address());
            match listener {
                Ok(l) => {
                    info!("{} başlatıldı...", self.to_string());
                    // Gelen talepleri sonsuz bir döngüde dinleyebiliriz.
                    loop {
                        // Gelen yeni bağlantıları match ifadesi ile kontrol altına alıyoruz.
                        match l.accept() {
                            Ok((mut stream, addrees)) => {
                                info!("İstemci -> {}", addrees.to_string());

                                /*
                                    İstemciden gelen talebi belli bir boyuttaki dizi içerisine almalıyız
                                    read trait'inin TcpStream için implemente edilmiş versiyonu,
                                    gelen içeriği mutable bir dizi içerisine yazmak üzere tasarlanmış.
                                    Başlangıç için 1024 elemanlı bir array göz önüne alabiliriz.
                                */
                                let mut buffer = [0_u8; 1024];
                                match stream.read(&mut buffer) {
                                    Ok(l) => {
                                        let msg = String::from_utf8(buffer[0..l].to_vec());
                                        info!("Gelen bilgi -> {:?}", msg.unwrap());
                                        // Request tipini try_from ile donatmıştık. Dolayısıyla gelen mesajı Request türüne çevirmeyi deneyebiliriz.
                                        let converted_msg = Request::try_from(&buffer[..]);
                                        match converted_msg {
                                            Ok(r) => {
                                                info!("Request dönüşümü başarılı.{}", r.to_string())
                                            }
                                            Err(e) => {
                                                error!("{:?}", e)
                                            }
                                        }
                                    }
                                    Err(e) => {
                                        error!("Stream okumada hata -> {}", e);
                                    }
                                }
                            }
                            Err(e) => {
                                error!("Bağlantı sağlanırken bir hata oluştu. Hata detayı -> {}", e)
                            }
                        }
                    }
                }
                Err(e) => {
                    error!("Sunucu başlatılamadı. Hata detayı -> {}", e);
                }
            }
        }
    }
}
```

Kodun en önemli kısmı run fonksiyonunun içeriği. Burada bir TcpListener oluşturulmakta ve sonsuz bir döngü yardımıyla gelen talepleri dinlemesi sağlanmakta. Talepler bir stream üzerine akmakta. Bu stream içeriğini basit olması açısından 1024 byte ile sınırladık. Yani 1 kilobyte büyüklüğündeki içeriği ele aldığımızı ifade edebiliriz. Yakalanan paketlere ne yaptığımıza dikkat edin... Request veri modeline öğrettiğimiz try_parse fonksiyonu ile Request değişkenine dönüştürmeye çalışıyoruz. Sonrasında yaptığımız tek şey başarılı bir dönüşüm gerçekleşmişse bunu log olarak basmak. Esasında istemciye HTTP metodunun tipine göre uygun dönüş kodu ile bir bilgi vermek gerekiyor öyle değil mi? İşte size bir ödev daha. Ben [github reposunda](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/crayz_server) bu şekilde geliştirmeye devam edeceğim. Ne yazık ki bu yazıya yetişmedi.

Çok doğal olarak program çalışmaya başladığında sunucunun ayağa kalkması ve bir şeyler dinlemeye başlaması gerekiyor. Bu amaçla main fonksiyonunu aşağıdaki gibi yazarak devam edebiliriz.

```rust
use crayz_server::server::Server;

fn main() {
    // loglamayı açtık
    env_logger::init();

    // Server veri yapımızı kullanarak bir örnek oluşturduk
    let alpha = Server::new("0.0.0.0".to_string(), 5555_u16, "localhost".to_string());
    // run fonksiyonunu çağırıp sunucuyu başlatıyoruz. ya da başlatamıyoruz :)
    alpha.run();
}
```

Soyutlamanın güzelliğini main fonksiyonunda yakalıyoruz. Her şey run içerisinde olup bitiyor:) Öyleyse bir test sürüşüne çıkalım mı?

```bash
# TcpListener ile dinelemeye başladıktan sonra test etmek istersek
# Önce uygulamayı aşağıdaki gibi başlatıp,
RUST_LOG=info cargo run
# ardından ikinci bir terminal açıp aşağıdaki komutlar ile mesaj göndermeyi deneyebiliriz.
echo "ping" | netcat localhost 5555
echo "ping pong" | netcat localhost 5555
curl localhost:5555
curl http://localhost:5555/query?word=red
curl -X POST http://localhost:5555/movies/ -H 'Content-Type: application/json' -d '{"message":"only one ping Vaseley."}'

# Hatta tarayıcı açıp http://localhost:5555/ping ile talep yapmayı da deneyebiliriz.
```

İçeride bir takım tedbirler almıştık. HTTP Get, Post, Put, Delete harici olanlar ele alınamayacaktı. Dolayısıyla netcat ile localhost:5555 adresine basit bir ping gönderdiğimizde ilgili paketin işlenmemesi ve hatta bir RequestError yayınlanması gerekir. Hatta mızmızlık yapıp sunucuya ping pong oynamak istediğinizi söylesek bile durum değişmez.

![crayz_server_3.png](/assets/images/2022/crayz_server_3.png)

Sorun netcat aracındadır diye düşünüp curl ile denesek nasıl olur?

![crayz_server_5.png](/assets/images/2022/crayz_server_5.png)

Hımmm... Bir yerlere vardık gibi. Üzerinde çalışmakta olduğum ubuntu sistemi için curl komutu HTTP komutlarını göndermek için biçilmiş kaftandır. Görüldüğü üzere hem root hem de query gibi path'lere yapılan talepleri yakalayabiliyoruz. Bir ödev olarak query string parametresini ve değerini yakalamayı deneyebilirsiniz. Hatta bu noktada programın bir bug'ı da olabilir. Mesela querystring parametre değerlerinde boşluk karakteri varsa...Sonuçta sunucunun yorumlaması gereken bir bilgi.

Yazdığımız örnek için son olarak birde HTTP Post talebi göndermeyi deneyelim. Body kısmında birde JSON içeriği olsun.

![crayz_server_json.png](/assets/images/2022/crayz_server_json.png)

Sonuç hiç de fena değil. POST ile gelen mesajın komple içeriğini yakalamış bulunuyoruz. Mesaj içeriğinde doksanlı yılların efsane filmlerinden birisine gönderme de var...Bakalım hangi filmden olduğunu tahmin edebilecek misiniz? Bu arada curl çağrılarına dönen "Empty reply from server" mesajları son derece doğal. Nitekim sunucumuz mesajları yakaladı, bir parça yorumladı ama istemci tarafa hiçbir bilgi göndermedi.

İstemci tarafına gönderilecek HTTP mesajları için yine bir veri yapısı tasarlayabiliriz. Bir HTTP cevabında durum kodunu ve örnek bir veri parçasını yollasak yeterli olur. Header kısmını şimdilik atlayalım işleri karıştırmayalım derim:) Bu amaçla response isimli bir modül açıp içeriğini aşağıdaki gibi geliştirelim.

```csharp
pub mod response {
        use std::fmt::{Display, Formatter};
        use std::io::{Result, Write};
        use std::net::TcpStream;

        pub struct Response {
            status_code: StatusCode,
            body: Option<String>,
        }

        impl Response {
            pub fn new(status_code: StatusCode, body: Option<String>) -> Self {
                Response { status_code, body }
            }

            pub fn write(&self, stream: &mut TcpStream) -> Result<()> {
                let body = match &self.body {
                    Some(b) => b,
                    None => "",
                };
                write!(
                    stream,
                    "HTTP/1.1 {} \r\n\r\n{}",
                    self.status_code.to_string(),
                    body
                )
            }
        }

        /// Birkaç HTTP statü kodunu tutan enum sabiti
        #[derive(Copy, Clone)]
        pub enum StatusCode {
            Ok = 200,
            BadRequest = 400,
            Unauthorized = 401,
            NotFound = 404,
            InternalServerError = 500,
        }

        impl Display for StatusCode {
            fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
                /*
                    enum sabitindeki sayısal değeri almak için dereference işlemi uyguladık.
                    Ancak bunu yaparken StatusCode'un Copy ve Clone trait'lerini uygulamış olması
                    gerekiyor. Nitekim buradaki move işlemi için kopyalama gerekiyor.
                */
                let code = *self as u16;
                match self {
                    Self::Ok => write!(f, "{} Ok", code),
                    Self::BadRequest => write!(f, "{} Bad request", code),
                    Self::Unauthorized => write!(f, "{} Unauthorized", code),
                    Self::NotFound => write!(f, "{} Not found", code),
                    Self::InternalServerError => write!(f, "{} Internal server error", code),
                }
            }
        }
    }
```

Response veri yapısı içerisinde durum kodu için StatusCode enum sabiti ve String eleman taşıyan Option türünden iki alan yer alıyor. StatusCode enum sabiti sembolik olarak Ok, BadRequest, Unauthorized, NotFound, InternalServerError gibi değerler barındırmakta. Response veri yapısına ait değişkenler esasında TcpStream üstünden istemciye gönderilecekleri için write isimli bir fonksiyon uyguladık. Bu fonksiyon dikkat edileceği üzere bir TcpStream alıyor ve o stream'in üstüne gerekli HTTP dönüşünü yazıyor. Böylece Server nesnesinin run fonksiyonu içerisinde istediğimiz noktalardan anlamlı HTTP cevapları dönebiliriz. run fonksiyonunu aşağıdaki gibi değiştirelim.

```rust
pub fn run(self) {           
    info!("{} başlatılıyor...", self.address());
    let listener = TcpListener::bind(&self.address());
    match listener {
        Ok(l) => {
            info!("{} başlatıldı...", self.to_string());
            loop {
                match l.accept() {
                    Ok((mut stream, addrees)) => {
                        info!("İstemci -> {}", addrees.to_string());
                        let mut buffer = [0_u8; 1024];
                        match stream.read(&mut buffer) {
                            Ok(l) => {
                                let msg = String::from_utf8(buffer[0..l].to_vec());
                                info!("Gelen bilgi -> {:?}", msg.unwrap());
                                let converted_msg = Request::try_from(&buffer[..]);
                                match converted_msg {
                                    Ok(r) => {
                                        info!(
                                            "Request dönüşümü başarılı.{}",
                                            r.to_string()
                                        );
                                        Response::new(
                                                StatusCode::Ok,
                                                Some(String::from("<h1>Would you like to play ping pong?</h1>")),
                                            ).write(&mut stream).expect("Problem var!");
                                    }
                                    Err(e) => {
                                        error!("{:?}", e);
                                        Response::new(StatusCode::BadRequest, None)
                                            .write(&mut stream)
                                            .expect("Problem var!");
                                    }
                                }
                            }
                            Err(e) => {
                                error!("Stream okumada hata -> {}", e);
                                Response::new(StatusCode::InternalServerError, None)
                                    .write(&mut stream)
                                    .expect("Problem var!");
                            }
                        }
                    }
                    Err(e) => {
                        error!("Bağlantı sağlanırken bir hata oluştu. Hata detayı -> {}", e)
                    }
                }
            }
        }
        Err(e) => {
            error!("Sunucu başlatılamadı. Hata detayı -> {}", e);
        }
    }
}
```

İlave kodlarımız response nesne örnekleri üzerinden write fonksiyonunu çağırdığımız yerler. Duruma göre geriye anlamlı bir mesaj göndermekteyiz. Örneğin gelen mesaj içeriği başarılı bir şekilde işlendiyse ya da anlaşıldıysa HTTP 200 Ok ile birlikte basit bir HTML çıktısı yolluyoruz. Kim bilir belki istemci "haydi oynayalım" der:D İşte örneğin şimdiki halinin çalışma zamanı çıktısı.

![crayz_server_last.png](/assets/images/2022/crayz_server_last.png)

Dönüş bilgisini iyileştirmek, Header bilgisi eklemek ve daha da önemlisi gelen talebe göre bir çıktı üretmek sizlere ev ödevi olsun;) Örneğin GET ile gelen taleplerde yer alan path bilgisini kullanarak sunucunun olduğu fiziki diskte yer alan static HTML sayfalarını çıktı olarak dönmeyi deneyebilirsiniz. HTTP talebinde gelen path bilgisi elimizde olduğundan bu tip bir yönlendirme (routing) nispeten kolay olacaktır. Ancak path bilgisine göre bir veri kaynağından JSON çıktı alıp göndermek farklı bir çözüm gerektirebilir. Bende pil bittiği için buraya kadar getirebildim. Gerisi sizde:) Böylece geldik bir rust pratiğimizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.