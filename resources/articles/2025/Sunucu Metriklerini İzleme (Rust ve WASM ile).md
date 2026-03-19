---
title: "Sunucu Metriklerini İzleme (Rust ve WASM ile)"
pubDate: 2025-01-27 14:29:00
categories:
  - Rust
tags:
  - Rust
  - Bash
  - YAML
  - JSON
  - JavaScript
  - .NET
  - REST
  - Web Service
  - HTTP
  - Docker
  - TypeScript
  - Node.js
  - Blazor
  - Async/Await
  - Threading
  - Performance
  - Serialization
  - Visual Studio
  - GitHub
  - Mutex
---

# Sunucu Metriklerini İzleme (Rust ve WASM ile)
![ferrisandwasm.png](images/ferrisandwasm.png)

WASM ya da bilinen adıyla WebAssembly tarayıcılarda yüksek performans çalışma zamanına çıkabileceğimiz ortamlar için ideal bir çatı sunar. Bu standart, düşük seviyeli binary format üzerine odaklanır ve C, C++ ve Rust gibi dillerle birlikte kullanılması hızlı bir çalışma zamanına olanak sağlar. Pek tabii WASM'ın kullanım alanı bu dillerle sınırlı değildir. Örneğin Microsoft, Blazor soyutlaması ile WASM ortamı için gerekli çıktıları üretmeyi oldukça kolaylaştırır. WASM'ın binary formattaki çıktılarının tarayıcıda çalıştırılması sırasında yine tarayıcıların sağladığı güvenlik protokolleri işletilir. Dolayısıyla tarayıcının çalıştığı sistem kaynaklarına çıkmak ve zararlı yazılım kodlarını işletmek pek mümkün değildir ("pek" diyorum çünkü hackerların sağı solu belli olmaz) Bununla birlikte sunucu tarafı ile olan iletişim klasik olarak servis çağrıları ile sağlanabilir.

Genel olarak geliştirme modeli şöyle işler; WASM destekli bir programlama diliyle gerekli kütüphaneler yazılır. Bu çıktılar bir ara işlem sonrası WASM modüllerine derlenir. İlgili modüller tarayıcı tarafına indirildikten sonra yine tarayıcı tarafında çalışır. WASM çıktısının makine koduna yakınlığı tarayıcıda yüksek performans avantajının elde edilmesinin en büyük sebebidir. Tarayıcıda çalışan WASM modülleri, Javascript tarafı ile iletişim kurabilir. Temel (primitive) veri türleri üzerinden sağlanan iletişim çift taraflı sağlanabilir. Bir başka deyişle, Javascript kodunda WASM modül fonksiyonları çağırılabileceği gibi, WASM içerisinden de önyüz tarafına belli ölçülerde müdahale edilebilir.

WASM'ın kullanılabileceği birçok senaryo var. Burada anahtar nokta tarayıcıda yüksek performans gerektiren yoğun hesaplamalı işlevleri gerekip gerekmediğidir. Söz gelimi tarayıcıda çalıştırılan oyunlar için WASM pekala ideal bir çözümdür. 3D grafik uygulamaları, yüksek matematik hesaplamalar gerektiren bilimsel çalışmalar, fizik kurallarına tabi modellemeler, büyük veri kümeleri ile ilgili analizler ve hatta legacy uygulamaların web ortamına taşınması gibi farklı senaryolarda ele alınabilir.

Bu teorik bilgileri bir kenara bırakalım ve gelin Rust dilinide işin içerisine katıp basit bir WASM uygulaması yazalım. Senaryomuzda bir sunucudan/sunuculardan CPU ve Memory istatistiklerini toplayan bir servis var. Bu REST tabanlı bir servis olarak tasarlanabilir. Söz konusu servisin çıktılarını alıp anlık istatistikler çıkartan yapımızı ise bir Rust kütüphanesi olarak tasarlayıp WASM tarafında kullandıracağız. Burada cpu ve memory değerlerine göre ileri tarihli tahminlemeler yaptığımızı ve bu hesaplamaların tarayıcıdak WASM modüllerinde gerçekleştirildiğini tahayyül edebiliriz. İstemci uygulamanın aşağıdakine benzer bir dashboard sağlayacağını düşünebiliriz.

![ServerStatsRuntime.png](images/ServerStatsRuntime.png)

Adım adım ilerleyelim.

## Ön Gereklilikler (Prerequisites)

Sistemimizde Rust'ın yüklü olduğunu varsayarak devam edebiliriz ama değilse [şuraya uğramakta](https://rust-lang.org/) yarar var.

1. wasm-pack: Bu araç wasm paketlerinin oluşturulması ve wasm uyumlu rust projesi oluşturulmasında yardımcı olacak bir araç. cargo aracı ile sistemimize kurabiliriz.

```bash
cargo install wasm-pack
```

2. Node.js and npm: Sonuçta bir web arayüzü var ve bunun servis edilmesi için bir runtime gerekiyor. Burada node.js ve npm işimize yarar. Bunların kurulumları içinse nereye bakmanız gerektiğini biliyorsunuz;) [İşte buraya](https://nodejs.org/).

3. Webpack: Birde web paketlerinin build edilmesi, sunucunun başlatılması gibi işlerde bize yardımcı olacak webpack aracına ihtiyacımız var. Bunu projede www klasörünü oluşturduktan sonra kuracağız. Yani aşağıdaki adımları takip etmeye devam edelim.

## CPU/Memory Veri Toplama Servisi

Öncelikle işlemci ve bellek kullanımlarına ait bilgileri toplayan servis tarafını yazmaya başlayalım. Bu servisi de Rust dilini kullanarak geliştirebiliriz.

```bash
cargo new cpu-mem-service

# Gerekli Crate'lerin yüklenmesi
cargo add actix-web actix-cors sysinfo serde_json chrono
cargo add serde -F derive
```

actix-web küfesini web framework olarak kullanacağız ve belki CORS hatası olabilir diye actix-cors'u da ele alacağız. Programın çalıştığı sistemdeki bilgileri toparlamak için sysinfo kütüphanesi (crate) kullanılabilir. REST tabanlı bir servis olduğundan servis çıktılarını JSON formatından serileştirebiliriz. Burada da serdejson ve serde kütüphaneleri işimizi kolaylaştıracaktır. Tarih ve zaman bilgisi ilgili işleri kolaylaştırmak içinse chrono kütüphanesi ele alınmaktadır. Esasında sunucu tarafı kodlarımız oldukça basit ve aşağıdaki gibi geliştirilebilir.

```rust
use actix_cors::Cors;
use actix_web::{web, App, HttpServer, Responder};
use serde::Serialize;
use std::sync::Mutex;
use sysinfo::System;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    let app_state = web::Data::new(AppState {
        history: Mutex::new(Vec::new()),
    });

    let address = "0.0.0.0:8080";
    println!("Starting server on {}", address);

    HttpServer::new(move || {
        App::new()
            .app_data(app_state.clone())
            .route("/machine/stats", web::get().to(get_sys_stats))
            .wrap(
                Cors::default()
                    .allow_any_origin()
                    .allow_any_header()
                    .allow_any_method(),
            )
    })
    .bind(address)?
    .run()
    .await?;

    Ok(())
}

struct AppState {
    history: Mutex<Vec<SystemStats>>,
}

#[derive(Serialize, Clone)]
struct SystemStats {
    timestamp: u64,
    cpu_usage: f32,
    memory_total: u64,
    memory_used: u64,
}

async fn get_sys_stats(data: web::Data<AppState>) -> impl Responder {
    let mut system = System::new_all();
    system.refresh_all();

    let cpu_usage = system.global_cpu_usage();
    let memory_total = system.total_memory();
    let memory_used = memory_total - system.available_memory();
    let timestamp = chrono::Utc::now().timestamp() as u64;

    let stats = SystemStats {
        timestamp,
        cpu_usage,
        memory_total,
        memory_used,
    };

    let mut history = data.history.lock().unwrap();
    history.push(stats.clone());
    if history.len() > 50 {
        history.remove(0);
    }

    web::Json(history.clone())
}
```

getsysstats isimli fonksiyon işlemci ve bellek kullanımlarını SystemStats isimli nesneden oluşan bir vektörde topluyor. Pek tabii bu bir web servisi olduğundan ve aynı andan birden fazla request geleceğinden verinin kendisini talepler arasında thread safe olarak koruma altına almak lazım. Mutex kullanmamızın bir sebebi de bu.

Servis tarafını tamamladıktan sonra en azından bir kere test etmekte yarar var. Bunun için tarayıcıdan localhost:8080/machine/stats adresine gidebiliriz. Sayfayı her yenilediğimizde JSON içeriğinin artması (max 50 kayıt tutar) gerekiyor. Böylece istemci tarafının sunucudaki son 50 istatistik bilgisini baz alarak işletilmesi sağlanabilir. Çalışma zamanı takriben aşağıdakine benzer olmalıdır. (Ekran görüntüsü docker geçişi sonrası alındığında port bilgisi 6501'dir)

![MachineStats.png](images/MachineStats.png)

Pek tabii veriler servis çalıştığı sürece tutulur, zira bellekte mutex ile koruma altına alınmış bir vector kullanılmıştır. Kalıcı olmasını sağlamak adına fiziki bir depolama alanı (dosya sistemi, veritabanı veya cloud provider) düşünülebilir. Bu durumda tutulan verinin örnek kümesi daha da artırılabilir. Burada şu soruyu sorabilirsiniz; "madem verileri sunucu tarafı sağlıyor, istatistik hesaplamasını da bir zahmet orası yapıp versin":D Çok haklısınız bu benimde kafamı kurcalayan birşey ama Rust ve WASM kullanımına hello world demek için ideal bir giriş senaryosu gibi de duruyor;)

## Opsiyonel Adım (Servisin Dockerize Edilmesi)

Deneysel amaçla geliştirilmiş bu servis istenirse Dockerize edilerek container ortamında da işletilebilir. Bunun için projeye aşağıdaki Dockerfile ve docker-compose.yml içeriklerini eklemek yeterlidir.

Dockerfile dosyası

```text
FROM rust:1.84.0 as builder

WORKDIR /app
COPY . .
RUN cargo build --release

FROM debian:bookworm-slim
RUN apt-get update && apt-get install -y libssl-dev ca-certificates && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=builder /app/target/release/cpu-mem-service /app/

CMD ["./cpu-mem-service"]
```

ve docker-compose.yml dosyası.

```yml
services:
  cpu-mem-service:
    build: .
    ports:
      - "6501:8080"
```

Buna göre docker çalışma zamanında localhost:6501 portuna gelen istekler, container içindeki 8080 portuna doğru yönlendirilecektir. Pek tabii programın docker container olarak hayat geçmesi için iki küçük adımımız var. Aşağıdaki terminal komutları ile bu işlemleri gerçekleştirebiliriz.

```bash
# Docker build işlemi
docker-compose build

# Çalıştırmak içinse
docker-compose up
```

Artık istemci tarafının geliştirilmesine başlayabiliriz.

## WASM Uygulamasının Geliştirilmesi

Rust tarafında WASM ile ilgili işleri kolaylaştırmak için wasm-pack aracından yararlanılabilir. Ön gereksinimler kısmında sistemimize bu aracı nasıl yükleyeceğimizi belirtmiştik.

```bash
wasm-pack new mach-dash-app
cd mach-dash-app

# json serileştirme desteği için serde_json
# asenkron fonksiyon desteği içinse wasm-bindgen-futures
cargo add serde_json wasm-bindgen-futures
```

Bu işlemin ardından Cargo.toml dosyasını kontrol etmekte yarar var. Özellikle dependencies kısmında wasm-bindgen modülünün eklenmiş olması gerekiyor. Bu modül WASM ve Javascript arasındaki iletişimde önemli bir role sahip.

```text
[dependencies]
wasm-bindgen = "0.2"
```

lib.rs dosyasının içeriğini de aşağıdaki gibi oluşturabiliriz.

```rust
use wasm_bindgen::prelude::*;

#[wasm_bindgen]
pub async fn analyze_stats(json_data: &str) -> JsValue {
    let data: Vec<serde_json::Value> = serde_json::from_str(json_data).unwrap();

    let cpu_usages: Vec<f32> = data
        .iter()
        .map(|entry| entry["cpu_usage"].as_f64().unwrap() as f32)
        .collect();
    let avg_cpu_usage = cpu_usages.iter().sum::<f32>() / cpu_usages.len() as f32;

    let memory_usages: Vec<u64> = data
        .iter()
        .map(|entry| entry["memory_used"].as_u64().unwrap())
        .collect();
    let avg_memory_used = memory_usages.iter().sum::<u64>() / memory_usages.len() as u64;

    let result = serde_json::json!({
        "avg_cpu_usage": avg_cpu_usage,
        "avg_memory_used": avg_memory_used
    }).to_string();

    JsValue::from(&result)
}
```

Buradaki fonksiyon json olarak gelen veri içeriğini alıp içinden çeşitli istatistiki değerleri okuyor ve ortalama kullanım değerlerini hesaplıyor. Fonksiyonumuz esasında sadece bu kadar ancak siz buraya daha gelişmiş bir özellik katabilirsiniz. Örneğin bir regresyan analizi işletilebilir. Sonuçta buradaki hesaplamalar tarayıcı tarafında gerçekleştirilecektir. Kodlarımızı tamamladıktan sonra ise wasm-pack ile gerekli paketleri oluşturmamız gerekiyor. Bu paket istemci tarafına indirilecek içeriği barındırmakta.

```bash
wasm-pack build --target web
```

Build işlemi sonrasında pkg isimli bir klasör oluşacaktır. Bu klasörde wasm, js (javascript) ve ts (typescript) uzantılı dosyalar olduğuna dikkat edelim. Webpack gerekli içeriklerin deployment ortamına alınması sırasında işimizi kolaylaştıracaktır.

## Frontend Geliştirmeleri

WASM modüllerini kullanacak önyüz tarafının geliştirilmesi ile yazımıza devam edelim. Bu amaçla root klasör altında www isimli başka bir alt klasör oluşturup içerisinde nodejs ortamının hazırlanması gerekiyor.

```bash
mkdir www
cd www

# node initialize işlemleri (Bir Package.json oluşturur)
npm init -y

# Webpack modüllerinin yüklenmesi
npm install --save-dev webpack webpack-cli webpack-dev-server copy-webpack-plugin
```

Az öncede belirttiğimiz üzere webpack, deployment işlerini kolaylaştırmak için ele aldığımız bir araç. Bu adımlardan sonra package.json dosyasının aşağıdaki gibi olmasını sağlayalım.

```json
{
  "name": "mach-dash-app",
  "version": "1.0.0",
  "description": "Machine CPU Memory Measurement Dashboard",
  "main": "index.js",
  "scripts": {
    "start": "webpack serve --open",
    "build": "webpack"
  },
  "author": "Burak Selim Senyurt",
  "license": "MIT",
  "devDependencies": {
    "webpack": "^5.97.1",
    "webpack-cli": "^6.0.1",
    "webpack-dev-server": "^5.2.0"
  },
  "dependencies": {
    "copy-webpack-plugin": "^12.0.2",
    "chart.js": "^4.4.7"
  }
}
```

start ve build, terminalden işleteceğimiz komutları işaret etmekte. Buna göre build işlemi webpack aracını devreye alacakken, start komutu webpack üzerinden hot reload özelliği ile birlikte sunucu tarafını otomatik olarak başlatacaktır. Şimdi www klasörü altında webpack.config.js isimli bir dosya oluşturup içeriğini aşağıdaki gibi oluşturalım.

```javascript
const path = require("path");
const CopyWebpackPlugin = require("copy-webpack-plugin");

module.exports = {
    entry: "./index.js",
    output: {
        path: path.resolve(__dirname, "dist"),
        filename: "bundle.js",
    },
    mode: "development",
    plugins: [
        new CopyWebpackPlugin({
            patterns: [{from: "index.html", to: "index.html"}],
        }),
    ],
    devServer: {
        static: "./dist",
        port: 6502,
        open: true,
    },
    experiments: {
        asyncWebAssembly: true,
    },
};
```

Burada belki de en önemli detay experiments sekmesindeki asyncWebAssembly özelliği. Bu önceki sürümlerde varsayılan olarak açık geliyordu diye hatırlıyorum ancak güncel sürümde Web Assembly desteğini sunmak için açıkça belirtilmesi bekleniyor. Diğer yandan uygulamamız development ortamında 6502 nolu porttan hizmet verecek. Ayrıca index.js içeriği bundle.js olarak paketlenecek. Genellikle birden fazla js dosyası varsa bundle.js gibi tek bir çıktı içerisine paketlenmesi öneriliyor.

Örneğimizde son kullanıcı açısından belki de en kıymetli kısım istatistiklerin grafiksel gösterimi elbette. Bunun için chart.js isimli açık kaynak javascript kütüphanesini kullanabiliriz. Pek tabii bu tip açık kaynak kütüphaneleri şirket çözümlerinden kullanırken dikkatli olmakta fayda var zira güvenlik açıkları barındırabilirler. Böyle olmasa bile her yama (patch) çıktığında denetlenmeleri ve çalışma ortamını bozmadıklarından emin olunması gerekir. Şimdi diyeceğim ki kendimiz yazalım ama bununla da uğraşılmaz doğrusu:D

Artık önyüz tarafını tasarlayabiliriz. Bu amaçla index.js ve index.html içeriklerini sırasıyla aşağıdaki gibi geliştirelim.

index.html;

```text
<html>
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Server System Statistics - Trend Analysis</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</head>
<body>
    <div class="container mt-5">
        <h1 class="text-center">Server System Statistics - Trend Analysis</h1>
        <div class="row">
            <div class="col-md-12">
                <canvas id="trendChart"></canvas>
            </div>
        </div>
        <div class="row mt-5">
            <div class="col-md-6">
                <h4>Average Memory Used: <span id="avgMemory"></span> MB</h4>
            </div>
            <div class="col-md-6">
                <h4>Average CPU Usage: <span id="avgCpu"></span>%</h4>
            </div>
        </div>
    </div>

    <script type="module" src="./bundle.js"></script>
</body>
</html>
```

HTML tasarımından ziyade esasında Javascript içeriği daha önemli. İlk satırda dikkat edileceği üzere mackdashapp.js isimli dosyaya bir referans eklenmiş durumda. Bu referans kendi içerisinde derlenmiş wasm modülünü kullanır. Bir başka deyişle, önyüz tarafı için wasm modülünün bir soyutlamasını (abstraction) sağlar ki bu da wasm içerisine aldığımız fonksiyonları kullanmamızı kolaylaştırır.

```javascript
import init, { analyze_stats } from '../pkg/mach_dash_app.js';

await init();

const trendChart = new Chart(document.getElementById('trendChart'), {
    type: 'line',
    data: {
        labels: [],
        datasets: [
            {
                label: 'CPU Usage (%)',
                data: [],
                borderColor: 'rgba(75, 192, 192, 1)',
                tension: 0.1,
            },
            {
                label: 'Memory Used (MB)',
                data: [],
                borderColor: 'rgba(153, 102, 255, 1)',
                tension: 0.1,
            },
        ],
    },
});

async function updateStats() {
    const response = await fetch('http://localhost:6501/machine/stats');
    const data = await response.json();

    const analysis = await analyze_stats(JSON.stringify(data));
    const result = JSON.parse(analysis);

    document.getElementById('avgCpu').textContent = result.avg_cpu_usage.toFixed(2);
    document.getElementById('avgMemory').textContent = (result.avg_memory_used / 1024).toFixed(2);

    trendChart.data.labels = data.map((d) =>
        new Date(d.timestamp * 1000).toLocaleTimeString()
    );
    trendChart.data.datasets[0].data = data.map((d) => d.cpu_usage);
    trendChart.data.datasets[1].data = data.map((d) => d.memory_used / 1024);
    trendChart.update();
}

setInterval(updateStats, 5000);
updateStats();
```

Koda göre updateStats fonksiyonu her beş saniyede bir REST servisine çağrıda bulunur, güncel makine kullanım istatistiklerini içeren veriyi çeker, bunu gerekli hesaplamaların yapılması için WASM modülündeki analyzestats fonksiyonuna gönderir ve en nihayetinde fonksiyon çıktısı da grafik kütüphanesine girdi haline getirilir. I know, I know... WASM'ın esprisi bu senaryoda sadece analyzestats fonksiyonunun kullanımı ile ilgili oldu. Ancak sanıyorum gidiş yolunu anladınız. Öyleyse artık projeyi derleme (build) ve çalıştırma (run) adımlarına geçebiliriz.

```bash
# www klasöründeyken
# build işlemi
npm run build

# Projenin çalıştırılması
npm run start
```

Eğer build aşamasında bir sorun olmadıysa localhost:6502 adresine gittiğimizde işlemci ve bellek kullanım oranlarının yakalandığı bir ekran görüntüsü alabilmeliyiz. Ayrıca bu grafik her beş saniyede bir yenilenmeli. Ben tabii sistemde işlemci kullanımını tavan noktalara çekecek veya belleği aşırı tüketecek işlemleri yakalayamadım ve düz bir çizgi elde ettim ve buda yazdıklarımı kontrol etmeniz için bir sebep;)

Aslında geliştirme adımları itibariyle biraz karışık görünüyor değil mi? Bence de öyle:D Microsoft'un Blazor ile Visual Studio IDE'si üstünden ya da proje şablonları yardımıyla dotnet komut satırı aracından sağladığı kolaylığı henüz bu ortamda bulabilmiş değilim. Yine de amacımız bu rahatlıktan ziyade Rust ile WASM'ın bir arada ele alındığı ve gerçek hayat senaryolarına yakın bir örneği ele almaktı. Bu senaryoyu genişletmek tamamen sizin elinizde. Örneğin chart kütüphanesini terk edip çizdirme işlemini WASM modülü içerisine almayı deneyebilirsiniz;) Ya da gerçekten regresyon analizini WASM modülündeki fonksiyonda yaptıracak bir geliştirme yapabilirsiniz. Ayrıca local storage'da izin verilen bölgelere çıkıp belki son birkaç 50 parçalık istatistiği tarayıcı tarafında önbelleğe alarak ilerlemeyi deneyebilirsiniz. Bana kalırsa bu işin nirvanası tarayıcıda çalışan bir oyun yazma. [Tamda şurada olduğu gibi](https://github.com/buraksenyurt/rust-farm/tree/main/handson/running_rectangle):D

Umarım bilgi verici olmuştur. Bir başka yazıda görüşünceye dek hepinize mutlu günler dilerim.

Proje kodlarına [github reposundan](https://github.com/buraksenyurt/friday-night-programmer/tree/main/src) ulaşabilirsiniz (cpu-mem-service ve mach-dash-app isimli klasörler)
