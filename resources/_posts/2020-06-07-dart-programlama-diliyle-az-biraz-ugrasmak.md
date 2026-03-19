---
layout: post
title: "DART Programlama Diliyle Az Biraz Uğraşmak"
date: 2020-06-07 14:14:00 +0300
description: "Çok duydum, çok bahsedildi. Hep Flutter arkasında kaldı. Aslında yıllardır vardı. Birazcık bakmamın zamanı gelip de geçmişti. Haydi dedim o zaman. Önce onu Heimdall(Ubuntu-20.04) yüklemem gerekiyordu tabii. Bu adresteki talimatları takip ederek kurulumu gerçekleştirdim ve birkaç örnek kod parçasını bol yorum satırlarıyla önce skynet reposuna ardından da bloga bırakmaya karar verdim. Sizde Dart diline şöyle bir bakmak temel özelliklerini öğrenmek istiyorsanız aşağıdaki kronolojiyi takip ederek bana katılabilirsiniz. Eğer Java veya C# kökenliyseniz DART diline alışmanız da çok zor değil."
categories:
  - dart
tags:
  - dart
  - java
  - csharp
  - bash
  - rest
  - json
  - http
  - javascript
  - async-await
  - threading
  - concurrency
  - performance
  - visual-studio
  - github
---
# DART Programlama Diliyle Az Biraz Uğraşmak
![dart.jpg](/assets/images/2020/dart.jpg)

Çok duydum, çok bahsedildi. Hep Flutter arkasında kaldı. Aslında yıllardır vardı. Birazcık bakmamın zamanı gelip de geçmişti. Haydi dedim o zaman. Önce onu Heimdall (Ubuntu-20.04) yüklemem gerekiyordu tabii. [Bu adresteki talimatları](https://dart.dev/get-dart) takip ederek kurulumu gerçekleştirdim ve birkaç örnek kod parçasını bol yorum satırlarıyla önce skynet reposuna ardından da bloga bırakmaya karar verdim. Sizde Dart diline şöyle bir bakmak temel özelliklerini öğrenmek istiyorsanız aşağıdaki kronolojiyi takip ederek bana katılabilirsiniz. Eğer Java veya C# kökenliyseniz DART diline alışmanız da çok zor değil. Tabii başlamadan önce adettendir dil ile ilgili sözü geçen genel özellikleri şöyle bir sıralayalım.

- Google tarafından yazılmış çok amaçlı bir programlama dilidir. A,B,C,D,E,F,G vitaminlerinin hepsini...Pardon:D Onunla web, mobil, masaüstü ve sunucu bazlı uygulamalar geliştirilebilir. Pek tabii google içindeki birçok uygulama onunla yazılmıştır (Adwords,AngularDart, Fuchsia ve tabii ki Flutter)
- Merak edenler için...2011 de GOTO konferansında duyurulmuş ve ilk sürümü 2013'te çıkmıştır. Amaç web uygulamalarını daha kolay bir şekilde geliştirmektir.
- Nesne yönelimlidir, sınıf tabanlıdır, açık kaynaktır ve C türevli söz dizimine sahiptir.
- Statik tip desteği vardır ama "var" da vardır:) Yani tip tahmini (type inference) özelliğini de sunar. Her ikisinin birleşimi = optionally typed olarak ifade edilmektedir.
- Fonksiyonel ve Reactive programlama paradigmalarını gayet güzel destekler.
- Native (mobil taraf için çok kıymetli) veya Javascript olarak (bu enteresan ve bakılası bir bilgi) derlenebilir.
- Garbage Collector mekanizması vardır.
- Flutter ile çok anılır ama flutter bilmeyen birisi bence önce Dart diline aşina olmalıdır. Ayrıca Flutter'ın performansında önem arz eden Hot Loading konusunu da doğrudan destekler. Zaten Ahead-Of-Time ve Just-In-Time gibi derleme opsiyonları sunar.
- Denilene göre TIOBE endeksinde ilk yirmide bile değil ancak diyoruz ya; önce dart sonra flutter. Hem sıralamada değil diye de öğrenme merakımıza ket mi vuralım!?:)
- Öğrenilmesi kolay bir dildir ve "hangi dille programlamaya başlayayım?" konusunda iyi bir alternatif olabilir.

## Şimdi Reklamlar

Pardon örnekler:) Sırayla kodu tanımak adına yazdığımız örnekler üzerinden bir geçelim derim. Kodları Visual Studio Code üstünde geliştirebilirsiniz. Arabirim, dart uzantısını görünce otomatikman gerekli eklentileri de yükleyecektir. İlk Hello World örneğimizin kodlarını aşağıdaki gibi geliştirebiliriz (Takip eden kodlardaki yorum satırlarını okumayı ihmal etmeyin)

intro.dart;

```java
// Basit bir sınıf tanımıyla işe başlayalım
class Player {
  // birkaç sınıf değişkeni. İlk etapta integer, string ve bool kullanabildim.
  // ek olarak double, list gibi veri türleri de var.
  int id;
  String nickName;
  int level;

  // burada read-only bir instance variable (bana göre özellikl) tanımı var
  bool _online = false;
  bool get online => _online; // sadece getter

  // Constructor tanımı. this ile doğrudan iç değişkenlere değer aktarımı sağlanır
  Player(this.id, this.nickName, this.level);
  
  // Yukarıdaki constructor bu şekilde de tanımlanabilir
  /*
    Player(id, nickName, level) {
    this.id = id;
    this.nickName = nickName;
    this.level = level;
  }
  */

  // toString metodunun ezilişi. toString çağrılabilen yerlerde bizim istediğim şekilde dönüş olacak
  @override
  String toString() =>
      "$id, $nickName, $level"; // fat-arrow notation'ına göre tek satırda fonksiyon tanımı

  // değer döndürmeyen bir iki metot
  void getOnline() {
    // if possible
    if (!_online) _online = true;
  }

  void getOffline() {
    if (_online) _online = false;
  }

  // String döndüren bir metot
  String getState() {
    if (!online)
      return "Active";
    else
      return "Passive";
  }
}

// Programın giriş noktası (entry point)
void main() {
  // var wilmort=Player();

  // Player tipinden bir nesne örneği tanımı. new operatörü de kullanılabilir ama şart değil.
  var runi = Player(1, "Ruynildson", 125);
  
  // runi'nin toString metodunu override etmiştik.
  print(runi);

  // Ternary operatörünün olmadığı bir programlama dili var mı acaba? :D
  print("Current state is ${runi.online == true ? "Active" : "Passive"}");

  runi.getOnline();
  print(runi.getState());

  runi.getOffline();
  print(runi.getState());

  // runi.online=false; // instance variable read-only tanımlandığı için burada derleme hatası oluşur
}
```

![skynet_18_Screenshot_1.png](/assets/images/2020/skynet_18_Screenshot_1.png)

intro2.dart isimli ikinci örneğimizde opsiyonel parametre kullanımına bakıyoruz.

```java
class Vehicle {
  int x;
  int y;
  bool engineIsOn;
  String name;

  // Yine yapıcı metot tanımımız var.
  // Bu sefer opsiyonel parametre de kullanıyoruz.
  // Böylece varsayılan yapıcı metot dahil toplamda dört versiyonla nesne örneği oluşturabiliriz
  Vehicle(
      {this.name = "anonymous",
      this.x = 0,
      this.y = 10,
      this.engineIsOn = false});

  // Kolaylık olsun diye toString metodunun varsayılan davranışını ezdik
  @override
  String toString() => "$name ($x:$y) - $engineIsOn";
}

void main(List<String> args) {
  var ghost = Vehicle(); //parametresiz constructor çağırılır
  print(ghost);

  var leopard = Vehicle(
      name: "leopard",
      x: 15,
      y: 20,
      engineIsOn: true); // opsiyonel parametre değerleri atanmalıdır
  print(leopard);

  var tiger = Vehicle(
      y: -45,
      name:
          "tiger t-10"); // sadece name ve y değişkenine değer atadığımız durum
  print(tiger);
}
```

![skynet_18_Screenshot_2.png](/assets/images/2020/skynet_18_Screenshot_2.png)

Üçüncü örneğimiz data.dart isimli bir kütüphaneyi kullanıyor ve basit bir Abstract Factory desenini uyarlıyor. Dolayısıyla intro3.dart'tan önce data.dart dosyasını kodlamamız lazım.

```csharp
/*
basit bir library örneği. 
Library'yi namespace veya paket gibi düşünmek mümkün
burada çok basit bir Abstract Factory tasarım kalıbı örneği uygulanmaktadır.
Dart'ın birkaç nesne yönelimli davranışını anlamakttır amaç.
senaryo: Departmana göre rapor isteyen bir object user varmış. İlgilendiği raporların üretilme detayları onun için önemli değil.
Bu üretim işini fabrika nesneleri üstlenecek. Budget, Performance ve Employees örnek raporlar. Bunları üretme işi ise ReportFactory 
türevli fabrika nesnelerinde.
*/

library data;

// abstrat class
abstract class Report {
  void publish();
}

// concrete class
class Budget implements Report {
  void publish() {
    print("Bütçe raporu dağıtılıyor");
  }
}

// concrete class
class Performance implements Report {
  void publish() {
    print("DB Performans raporu dağıtılıyor");
  }
}

// concrete class
class Employees implements Report {
  void publish() {
    print("Çalışan performans raporu dağıtılıyor");
  }
}

// factory
// rapor üretme işini tanımlayan sözleşme
abstract class ReportFactory {
  Report
      CreateReport(); //Report döndürüyor. Yani Budget, Performance ve Employees için kullanılabilir
}

//concrete factory sınıfları
// Bütçe raporu üretme işini üstlenen fabrika
class BudgetReportFactory implements ReportFactory {
  Budget CreateReport() {
    return Budget();
  }
}

class PerformanceReportFactory implements ReportFactory {
  Performance CreateReport() {
    return Performance();
  }
}

class EmployeesReportFactory implements ReportFactory {
  Employees CreateReport() {
    return Employees();
  }
}
```

ve bu paketi kullanan intro3.dart kodlarımız.

```java
/*
Bu örnek data.dart isimli kütüphaneyi (library) kullanıyor.
data kütüphanesinde basit bir Abstract Factory deseni uyarlaması var.
Hem kendi kütüphanemizi nasıl kullanırız hem de abstract sınıftan türetme gibi OOP
özelliklerini nasıl uygularız diye bakıyoruz.
*/
import 'data.dart'; // Kendi yazdığımız kütüphane bildirimi

// List<String> şaşırtmasın. Komut satırından parametre alabiliyoruz
main(List<String> args) {
  var option = args[0]; //terminal penceresinden ilk parametreyi alıyoruz.

  // rapor üretici fabrika değişkeni tanımı
  ReportFactory factory;

  // bir seçime göre gerekli factory nesnesi örnekleniyor
  switch (option) {
    case "b":
      factory = new BudgetReportFactory();
      break;
    case "e":
      factory = new EmployeesReportFactory();
      break;
    case "p":
      factory = new PerformanceReportFactory();
      break;
    default:
      print("ne desem bilemedim");
  }

  // Asıl istediğimiz raporu ürettiriyoruz
  var report = factory.CreateReport();
  // ve hangi rapordan istediysek onun publish metodunun çıktısını ekranda görmeyi bekliyoruz
  report.publish();
}
```

![skynet_18_Screenshot_3.png](/assets/images/2020/skynet_18_Screenshot_3.png)

Dördüncü örnekte dahili bir dart kütüphanesinin kullanımına yer veriliyor.

intro4.dart;

```java
/*
internal library kullanımı örneği.
dart.convert içerisinde JSON ve UTF8 için faydalı dönüştürme operasyonları var.
*/
import 'dart:convert';

main(List<String> args) {
  var someData = '''[
    {"id": 1, "color": "blue", "size": 10.50},
    {"id": 2, "color": "red", "size": 19},
    {"id": 3, "color": "green", "size": 50.987854}
  ]
  '''; // örnek bir JSON içeriği var.

  var decoded =
      jsonDecode(someData); // decode edip aşağıdaki gibi kullanabiliriz

  for (var d in decoded) {
    print(
        "${d['id']} , ${d['color']}, ${d['size']}"); // String Interpolation sağolsun
  }

  print(decoded[1][
      'color']); // decode edilmiş dizideki birinci elemanın color niteliğinin değerini yazdırır

  //print(decoded);
}
```

![skynet_18_Screenshot_4.png](/assets/images/2020/skynet_18_Screenshot_4.png)

İzleyen beşinci örnekte Dart'ın temel fonksiyonel dil özelliklerini incelemeye çalışıyoruz.

```java
/* 
DART, fonksiyonel dil özelliklerini de bünyesinde barındırır.
Yani, fonksiyonları başka fonksiyonlara parametre olarak geçebilir, fonksiyonları değişkenlere atayabilir, isimsiz fonksiyonlar (anonymous'u hatırlayalım) yazabilir,
currying yapabiliriz (çok parametreli bir fonksiyonu tek parametreli hale getirip kullandırtmak)...
Bu örnekte birkaç fonksiyonel yaklaşımın nasıl kullanıldığına yer verilmektedir.
*/

import 'dart:math'; //Matematik kütüphanesinden karekök fonksiyonunu kullanmak için

main(List<String> args) {
  // kobay bir sayı listesi
  var numbers = [4, 2, 9, 8, 6, 7, 6, 5, 1, 3, 3, 1, 6, 8];

  // Mesela 3ncü ile 8nci arasındakilerinde bir işlem yapıp bunu imperative yaklaşımla ekrana yazdırmak istesek
  for (var i = 3; i < 8; i++) {
    print(sqrt(numbers[i]));
  }

  print("----");
  // Şimdi yukarıdaki işlemi fonksiyonel yaklaşımla yazalım
  numbers.skip(3).take(5).map(sqrt).forEach(print);

  // Birkaç fat-arrow function tanımlaması
  var sum = (x, y) => x + y;
  print(sum(3, 4));

  var multi = (num x, num y) => x + y; // bu sefer parametre tipi belirttik
  print(multi(3.56, -4.213));

  num div(num x, num y) =>
      x / y; // Hem dönüş hep parametre tipleri açıkça belirtiliyor
  print(div(3, 9));

  // Böyle güzel şeyler de yazabiliyoruz. Parametre olarak gelen
  // herhangi bir String listenin elemanlarının karakter sayısının toplamını bulan fonkisyon
  num findCharCount(List<String> names) =>
      names.map((name) => name.length).fold(0, (num a, num b) => a + b);
  print(findCharCount(["bir", "iki", "üç", "dört"]));
  print(findCharCount(
      ["black", "window", "currying", "livinsteineinenkovskiyeviç"]));
}
```

![skynet_18_Screenshot_5.png](/assets/images/2020/skynet_18_Screenshot_5.png)

Gelelim altıncı örneğe. Burada asenkron kullanım denenmektedir.

```java
/*
  Dart dilinde de asenkron fonksiyon kullanımları mümkündür. 
  Özellikle uzun süren işlerin ana iş parçacığını bekletmesini istemediğimiz durumlarda işe yarar.
  future bu anlamda önemli bir terimdir. Asenkron tasarlanan bir fonksiyon
  anında bir future döndürür. O anda feature Uncompleted pozisyonundadır.
  Asenkron çağırılan fonksiyon başarılı şekilde tamamlandığında future,
  Completed pozisyonuna geçer. Exception oluşursa da Completed pozisyonda kalamaz tabii.
  future dediğimiz şey esasında Future sınıfının bir nesne örneğidir.
  Asenkron bir fonksiyon değer dönebilir veya void olarak tanımlanabilir. 
  Future<void> ve Future<int> gibi.
  Declerative yaklaşıma göre asenkron olan fonksiyonlarda async kelimesi kullanılır.
  Asenkron fonksiyonun işini bitirdikten sonra sonucunu almak için await kullanılır.
*/
import 'reporter.dart';

main(List<String> args) async {
  var watson = Reporter();
  Counting(10);
  var value = await watson
      .GetReportResult(); //işlemlerin tamamlanmasını await ile bekletiyoruz
  print("Hesaplamalara göre risk değeri ${value}");
}

void Counting(int max) {
  for (var i = 1; i <= max; i++) {
    Future.delayed(Duration(seconds: i), () => print(i));
  }
}
```

Örnekte asenkron olarak çağırılabilen bir sınıfımız var. Reporter isimli bu sınıfı aşağıdaki gibi kodlayabiliriz.

```java
class Reporter {
  // asenkron çağırılabilecek şekilde tasarlanan fonksiyonumuz. async ve await kullanımlarına dikkat.
  // await fonksiyonları sadece async fonksiyonlar içerisinde kullanılır
  Future<num> GetReportResult() async {
    print("Hesaplanıyor...");
    var value =
        await _calculateRisk(); // Uzun süren fonksiyone awaitable. O bitene kadar kodu duraksattık
    print("Hesaplandı");
    return value;
  }

  // Risk değeri hesap eden bütçe fonksiyonumuzun uzun sürdüğünü varsayalım
  // delayed ile 5 saniyelik suni bir gecikme yaratıyoruz
  // 5 saniye sonrasında ise fonksiyona geriye 0.17 değerini taşıyan bir future döndürüyor
  Future<num> _calculateRisk() => Future.delayed(
        Duration(seconds: 5),
        () => 0.17,
      );

  // Bu arada metot adındaki _ işareti onu private yapar. Yani bu library dışında erişilemez. Mesela intro6.dart içinden.
  // Yani bir library içindeki metodun ya da alanın private olması isteniyorsa adının başına _ işareti konur
}
```

![skynet_18_Screenshot_6.png](/assets/images/2020/skynet_18_Screenshot_6.png)

Asenkron çalışma konusuna baktıysak Concurrency'ye bakmadan olmaz:) İşte Dart dilinde Concurrency'nin temel uygulanış biçimi.

```java
/*
  Dart dilinde Concurrency konusu da desteklenir.
  Yani birden çok işin eş zamanlı olarak başlatılması sağlanabilir.
  Üstelik her biri gerçekten de kendi bellek bölgesinde ve thread'i içinde çalışır.
  Yani tam bağımsız çalışan işçilerdir. Aralarında mesajlaşarak haberleşebilirler. Bu açıdan Javascrip tarafındaki Web Worker'lara benzetilirler.
  Bunun için isolate kütüphanesindeki Isolate sınıfı kullanılır.
  Örneği üstüste birkaç kez çalıştırmakta yarar var. Çağırılan metot sıraları farklılık gösterecektir. 
  Isolate metotlarını her zaman aynı sırada başlatmaz.
*/

import 'dart:io';
import 'dart:isolate';

main(List<String> args) {
  //spawn ile eş zamanlı çalışacak 3 fonksiyon çağrısı tanımlandı.
  // spawn Function<T> tipinden parametreler alır.
  // Geriye Future döndürür. Doğal olarak asenkron yapıdadır.
  // Spawn static veya top level fonksiyonları işaret edebilir
  Isolate.spawn(Worker.CalculateTime, null);
  Isolate.spawn(Worker.GetPlayerStatistics, "Jordan");
  Isolate.spawn(Worker.TrashGarbage, null);

  print("İşler tetiklendi");
  sleep(Duration(seconds: 3)); // Ekran kapanmadan diğer işler bitsin diye eklendi.
}

class Worker {
  // static metotlar çağırılırken tanımlandığını sınıfa ait nesne örneği gerektirmez
  // arg değeri spawn metodunun ikinci parametresi olarak gelir. Genellikle concurrent operasyonu bir nesne taşımak için kullanılır.
  static void GetPlayerStatistics(var arg) {
    print("${arg} için istatistikler çekiliyor");
    print("Oyuncu istatistikleri hazır");
  }

  static void CalculateTime(var arg) {
    print("Zaman değerleri hesaplanıyor");
    print("Hesaplandı");
  }

  static void TrashGarbage(var arg) {
    print("Atıl nesneler atılıyor");
    print("Atıldılar...");
  }
}
```

![Screenshot_7.png](/assets/images/2020/Screenshot_7.png)

Sekizinci örnekte asenkron çağrımlarda ele alınan stream nesnesine bir bakıyoruz. Bu sayede asenkron çağırımlar sırasında olayların arasına nasıl girebileceğimizi irdeliyoruz.

```java
/*
  Dart asenkron programlamada Stream adı verilen bir mevzu da var.
  Stream ile asenkron olarak çalışan olaylarda araya girebiliyoruz.
  Stream'ler verinin bir noktadan diğerine akarken kullanılan kanalı referans ediyorlar.
  Bu alana girerek akan veri üzerinde çeşitli işlemler yapabiliriz.
  Bu arada iki tür Stream var. Broadcast ve Single Subscription ki henüz ne olduklarını tam olarak öğrenemedim :(
*/

import 'dart:async';
import 'dart:io';

main() async {
  //awaitable çağrım içeriyor
  var stream = calculate(5); //stream nesnesini alıyoruz
  var total = await lookInsideStream(stream);
  print("\nToplam : ${total}");
}

// Asenkron çalışan bir operasyon ama geriye iteratif bir Stream döndürüyor.
// lookAtTheStream metodu bu dönen stream'i kullanıyor
Stream<num> calculate(num max) async* {
  for (int i = 1; i <= max; i++) {
    stdout.write("yield ${i}...");
    yield i;
  }
}

// Parametre olarak calculate çağrısı sonucu üretilen Stream'i almakta
// Buna göre kanal(channel) üstündeki her çağrımda devreye girecek.
Future<num> lookInsideStream(Stream<num> stream) async {
  var sum = 0;
  await for (var value in stream) { //kanaldaki verilerde hareket edebiliriz
    stdout.write(" (${value}) "); // o an yakaladığımız veri üstünde istediğimiz işlemi yapmamız mümkün
    sum += value;
  }
  return sum;
}
```

![Screenshot_8.png](/assets/images/2020/Screenshot_8.png)

ve geldik bu çalışmadaki son iki örneğimize. İlk olarak çok ilkel bir web sunucusunun nasıl yazıldığına bakıyoruz.

```java
/*
  Bu sefer ki örnekte ilkel bir web server nasıl yazılabilir onu anlamaya çalışacağız.
  Daha önceki örneklerde gördüğümüz async, await, Future ve hatta concurrent ile ilişkili olan
  Stream'ler burada daha anlamlı hale geliyor.
  Aşağıdaki örnek gelen HTTP Get taleplerinin hepsine standart bir HTML içeriği döndürüyor.
  Harici taleplerde ise (HTTP Post,Put,Delete gibi) MethodNotAllowed (HTTP 405) scevabı verilmekte.
*/

import 'dart:io';

Future main(List<String> args) async {
  // localhost:8887 nolu adresi kendisine bağlayarak sunucu nesnesini oluşturduk
  // loopbackIPv4 localhost veya 127.0.0.1'i işaret edecektir. Portu tamamen keyfi seçtik.
  var server = await HttpServer.bind(
      InternetAddress.loopbackIPv4, 8887); //awaitable bir çağrıdır

  print("Sunucu ${server.port} üstünden dinlemede");

  // server üzerine gelen http talepleri eş zamanlı olarak dinleniyor
  // HttpServer sınıfı Stream'leri kullandığından aşağıdaki gibi await for yazarak gelen her talep sonrası araya girmek mümkün oluyor.
  await for (HttpRequest request in server) {
    // Taleple ilgili birkaç bilgi yazdırabiliriz. HTTP Metodu, gelen talep adresi vs...
    print("Gelen talep, ${request.method}\n Uri : ${request.requestedUri}");

    if (request.method == "GET") {
      // Gelen talep Get metodu ise bunları bunları yap
      // .. ile response nesnesi üzerinden aynı ifadede hem özellik değeri atayabilir hem de metot çağrısı gerçekleştirebiliriz
      request.response
        ..statusCode = HttpStatus.ok // HTTP Statü kodu olarak 200 dönüyoruz
        ..headers.contentType = ContentType
            .html // Döndürdüğümüz içeriğin HTML formatında olduğu Header ile belirtiyoruz
        ..write(
            "<h2>Sunucu zamanı : ${DateTime.now().toString()}</h2>") // basit bir HTML içerik döndürüyoruz
        ..write(
            "<p>Şu anda ${request.requestedUri.path} adresine talepte bulundunuz</p>")
        ..close(); // response diğer dillerde olduğu gibi kapatılmalı
    } else {
      //değilse istemciye söz konusu metot çağrısına izin verilmediğini söyle
      request.response
        ..statusCode = HttpStatus.methodNotAllowed
        ..write(
            "${request.method} metodu bu sunucu tarafından desteklenmemektedir.")
        ..close();
    }
  }
}
```

![Screenshot_9.png](/assets/images/2020/Screenshot_9.png)

![skynet_18_Screenshot_10.png](/assets/images/2020/skynet_18_Screenshot_10.png)

Tabii bu ilkel web sunucusu arkasına şöyle JSON içeriği sunan güzel bir REST servis koysak hiç de fena olmaz diyor ve devam ediyoruz.

```java
/*
  Web server maceralarına devam.
  Bu seferki örnekte fiziki bir JSON dosyasının içeriğini geriye döndürüyoruz.
  Bunu, sadece HTTP Get metodunda ve /heros path'ine gelen talepler karşılığında yapıyoruz.
  Buradan yola çıkarak çok basit bir CRUD Rest servisine kadar gidilebilir.
*/
import 'dart:io';

Future main(List<String> args) async {
  // localhost ve 8888 portunu server nesnesine bağladık
  var server = await HttpServer.bind(InternetAddress.loopbackIPv4, 8888);

  print("${server.port} nolu portan dinlemedeyiz");

  // server ile açılan stream üstünde gerçekleşen istekleri dinliyoruz
  await for (HttpRequest req in server) {
    print("${req.method}, ${req.uri}");

    // Eğer talep GET metodundaysa ve path /heros ise
    if (req.method == "GET" && req.requestedUri.path == "/heroes") {
      // JSON formatında içerik döneceğimiz için ContentType'a uygun değeri atadık
      req.response.headers.contentType = ContentType.json;
      // Statü kodunu güncelledik
      req.response.statusCode = HttpStatus.ok;
      // JSON verisi için kaynak -> https://gist.github.com/mariodev12/a923f2b651a005ca3ca7f851141efcbc

      // İlginç bir kod parçası değil mi?
      // File nesnesini bir değişkene atamadan kullanıyoruz.
      await new File('superHeroes.json') // json dosyasının içeriğini
          .readAsString() //string formatta oku
          .then((content) => req.response.write(
              content)); //okuma tamamlandığında response üstüne yaz (Zaten saf json içeriği olduğundan decode etmemiz gerek yok)

    } else {
      // Farklı bir talep gelirse 404 Not Found muamelesi göster
      req.response.statusCode = HttpStatus.notFound;
      // Nezaketen de bir cevap yaz :D
      req.response.write(
          "${req.method} ve ${req.requestedUri.path} kullanılabilir değil.");
    }
    await req.response
        .close(); // buffer'a alınmış bir şeyler olma ihtimaline karşın tüm içeriği yazdırmayı garantile
  }
}
```

![skynet_18_Screenshot_11.png](/assets/images/2020/skynet_18_Screenshot_11.png)

Eğer bu noktaya kadar kodları tatbik edip benzer sonuçlar elde ettiyseniz Dart diline giriş yapmışsınız demektir. Elbette üstüne katarak devam etmek tamamen sizin elinizde. Bu arada örnekleri nasıl çalıştıracağınızı söylemeyi unutmuş olabilirim. Aşağıdaki terminal komutu bunun için yeterli.

```bash
dart intro.dart
```

## Ödevler

Pratik yapmak amacıyla yukarıdaki örnekleri tamamladıktan sonra aşağıdaki görevleri yerine getirmeye çalışabilirsiniz.

- Dart ile en az iki tasarım kalıbını uygulamaya çalışın.
- Komut satırından çalışan ve basit dört işlem yapan bir hesap makinesi geliştirin (hesaplama komutları ayrı bir kütüphanede olsun)
- Bir şirket çalışanına ait bilgileri içeren bir sınıfa ait nesne dizisini JSON formatından serileştirmeyi deneyin.
- intro9 örneğinden ilham alarak sadece HTTP Get ve Post operasyonlarını içeren ve sadece JSON tipiyle çalışan basit bir REST Api sunucusu geliştirin.

Yazımızda yer alan örneklerin tamamına [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2018%20-%20Introduction%20for%20DARTlang) üzerinden de erişebilirsiniz. Böylece geldik bir derlemenin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.