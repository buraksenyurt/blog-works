---
layout: post
title: "Rust Pratikleri - OOP"
date: 2022-04-24 09:00:00 +0300
categories:
  - rust
tags:
  - rust
  - bash
  - csharp
  - dotnet
  - java
  - memory-management
  - generics
  - dependency-injection
  - github
  - shared-state
  - ownership
  - borrow-checker
  - dependency-management
---
Yılların.Net geliştiricisi olunca insan ki sanıyorum Java tarafından gelse de durum değişmeyecektir, ister istemez Rust, Go gibi dillerde nesne yönelimli dünyanın karşılıklarını arıyor. Ortak özellikleri toplayacağım üst tipler yok mu, peki ya bağımlılıkları soyutlamak için başvuracağım interface türevleri, bukalemun varlıklara ne demeli. Kısacası bir nesne yönelimli dilin öne çıkan en belirgin özellikleri encapsulation, Inheritance, Polymorphism gibi detaylara bakıyoruz.

![oop_cover.png](/assets/images/2022/oop_cover.png)

Nesne yönelimli dil yaklaşımlarının bu çekirdek özelliklerinin Rust tarafındaki olası karşılıklarını araştırırken birkaç şey öğrendim tabii ki ve C# ile yazdığım bazı şeyleri Rust tarafında nasıl yapabilirim öğrenmek istedim. Sonuçta ortaya şu an okumakta olduğunuz pratik çıktı. Ben daha çok kalıtım ve çok biçimli nesne davranışı konularına eğilmeye çalıştım. Dilerseniz hiç vakit kaybetmeden örneklerimize geçelim. İşe basit bir C# kurgusu ile başlamakta yarar var. Detayları sade bir şekilde görmek için Console uygulaması biçilmiş kaftan.

```bash
dotnet new console -n sharpy
```

Program kodlarımızı aşağıdaki gibi yazarak devam edelim.

```csharp
var tars = new Robot("TARS", 80);
Console.WriteLine(tars.ToString());
tars.LoadFuel(10);
Console.WriteLine(tars.ToString());
tars.Walk();
Console.WriteLine(tars.ToString());

public enum State
{
    Online,
    OutOfService,
    OnTheMove,
    Destroyed
}
class Robot
{
    public string Name { get; set; }
    public float FuelLevel { get; set; }
    private State State { get; set; }

    public Robot(string name, float fuel)
    {
        Name = name;
        FuelLevel = fuel;
        State = State.Online;
    }

    public void LoadFuel(float amount)
    {
        Console.WriteLine($"{amount} litre yakıt yükleniyor...");
        this.FuelLevel += amount;
    }

    public void Walk()
    {
        Console.WriteLine("Hareket halinde");
        this.State = State.OnTheMove;
    }

    public override string ToString()
    {
        return $"{this.Name}. Yakıt {this.FuelLevel}. Durum {this.State}";
    }
}
```

Örneğimizde bir oyun sahasındaki robotu modellemeye çalışıyoruz. Bu sınıfının iki public ve bir de private özelliği var. Auto-Property şeklinde tanımlandıklarından get ve set blokları dotnet tarafından ara kodda otomatik olarak tamamlanıyor. Parametreler ile overload edilmiş bir yapıcı metot da var (constructor). Ayrıca Walk ve LoadFuel isimli deneysel fonksiyonlara sahip. Object sınıfından gelen ToString metotu bu sınıf için yeniden yazılmış durumda (override) Bilindiği üzere Object tipi dotnet camiasındaki ata sınıf. Virtual olarak sahip olduğu ToString fonksiyonunun varsayılan bir davranış zaten mevcıt ancak istersek kendi tiplerimiz için bunu değiştirebiliriz. Robotun çalışma zamanındaki anlık durumunu tutmak için State isimli bir Enum sabiti kullanmaktayız. Bu örneğin çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![oop_1.png](/assets/images/2022/oop_1.png)

Peki ya benzer bir modeli Rust tarafında kurgulamak istersek. Sınıf, override, private erişimler, nesne fonksiyonları gibi temel modelleme konularını bakalım nasıl karşılayacağiz. Bu örnek özelinde de bir terminal uygulaması işimizi görecektir.

```bash
cargo new rusty
```

Kodlarımızı da şöyle yazabiliriz.

```rust
use std::fmt::{Display, Formatter};

fn main() {
    // Robot nesnesi üstünde değişikliker yapacağımız için mutable olması gerekir
    // Sonrası C# tarafı ile oldukça benzerdir.
    let mut tars = Robot::new(String::from("TARS"), 80.0);
    println!("{}", tars);
    tars.load_fuel(10.0);
    println!("{}", tars);
    tars.walk(24.0, -50.9);
    println!("{}", tars);
}

// C# Örneğindeki gibi robotun anlık durumu için burada da bir enum kullanıyoruz.
// Tabii rust için enum bir veri yapısıdır. Zenginleştirilebilir. OnTheMove alanında olduğu gibi.
#[allow(dead_code)]
enum State {
    Online,
    OutOfService,
    OnTheMove(Location), // Bonus :) Rust enum veri yapısının zenginliğini kullandık. OnTheMove state'indeyken örneğin lokasyonunu da tutalım dedik.
    Destroyed,
}

struct Location {
    pub x: f32,
    pub y: f32,
}

// C# tarafındaki Robot sınıfı burada bir struct olarak tanımlanır.
// Malum Rust tarafında class diye bir kavram yok.
struct Robot {
    pub name: String,
    pub fuel_level: f32,
    state: State,
}

// C# tarafında Robot nesnesinin metotları(yapıcı metot dahil) sınıf tanım blokları içerisindedir.
// Rust fonksiyonel paradigmayı benimser ve aşağıdaki usülde ilerlenir.
impl Robot {
    // yapıcı metot karşılğı. Self ile çalışma zamanındaki Robot nesnesini ifade ederiz.
    pub fn new(name: String, fuel_level: f32) -> Self {
        Self {
            name,
            fuel_level,
            state: State::Online,
        }
    }
    // yakıt seviyesini artıran fonksiyon.
    // Tabii rust tarafında her değişken aksi belirtilmediği sürece immutable olduğundan,
    // self için &mut kullanılır.
    pub fn load_fuel(&mut self, amount: f32) {
        println!("{} litre yakıt yükleniyor.", amount);
        self.fuel_level += amount
    }

    // Rust örneğinde bonus olarak enum veri yapısını zenginleştirebileceğimizi göstermek istedim.
    pub fn walk(&mut self, x: f32, y: f32) {
        println!("Hareket halinde");
        self.state = State::OnTheMove(Location { x, y });
    }
}

// C# örneğinde ToString metodunu override edip Robot nesneleri için bu davranışı değiştirmiştik.
// Rust tarafında bunun için Display trait'ini Robot nesnesi için implemente edebiliriz.
impl Display for Robot {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{}. Yakıt {}. Durum {}",
            self.name, self.fuel_level, self.state
        )
    }
}

// Tabii Rust tarafında şöyle bir sorun olacaktır. Robot veri yapısının kullandığı
// State enum türü için de Display trait'ini uygulamamız gerekir.
impl Display for State {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        let state = match self {
            State::Online => "Çalışıyor.".to_string(),
            State::Destroyed => "Yok edildi".to_string(),
            State::OutOfService => "Servis dışı".to_string(),
            State::OnTheMove(l) => format!("{},{} noktasında hareket halinde.", l.x, l.y),
        };
        write!(f, "{}", state)
    }
}
```

Rust bellek yönetimi ve güvenliği adına derleyici odaklı (Compiler Oriented) çalışan bir dildir. Ownership ve Borrow Checker mekanizmaları bunu garanti etmek üzerine kurgulanmıştır. Geniş bir enstrüman desteği olsa da özellikle Heap alanını doyasıya kullanan sınıf gibi kavramları yoktur. Üstelik fonksiyonel dil paradigmalarını biraz daha öne çıkarır ve örneğin her değişkenin varsayılan halde değiştirilemez (immutable) olduğunu kabul eder. Zaten bu yüzden koddaki tars, mut anahtar kelimesi ile açık bir şekilde değiştirilebilir olarak tanımlanmıştır. Önceki örneğin karşılığı olarak burada Robot isimli bir struct kullanılmaktadır. ToString muadili olarak sistemde Display trait'i tanımlı olan davranış Robot nesnesi için yeniden uyarlanmıştır. Struct içindeki alanlar varsayılan olarak private erişim belirleyicisine sahiptir. Yani herkesin erişebilmesi adına bilinçli olarak public'e çekilmelidir. Aynen C# örneğindeki gibi burada da Struct modeline fonksiyonlar bağlanabilir. Diğer yandan Rust'ın enum türü oldukça zengindir. Örnekte bonus olarak bir enum değerinin farklı bir veri türünü kullanması da ele alınmıştır.

Örneğin çalışma zamanı çıktısı da aşağıdaki gibi olacaktır.

![oop_2.png](/assets/images/2022/oop_2.png)

## Kalıtım (Inheritance) Durumu

Gelelim nesne yönelimli programlamanın önemli kavramlarından birisi olan türetmeye. Bunu için dotnet tarafındaki uygulamamızı değiştirerek ilereyelim. Örneğin Robot haricinde Submarine isimli bir başka sınıf daha kullanalım. Her ikisinin ortak özellik ve fonksiyonlarını da bir üst sınıfta toplayalım. Üst tür olarak tasarlanan Vehicle tipik bir abstract sınıf rolünü üstlenmekte. Robot ve Submarine sınıfları, sahip oldukları ortak özellikler ve yapıcı metot için bu nesneye gelecekler.

```csharp
Robot tars = new Robot("TARS", 80);
Console.WriteLine(tars.ToString());
tars.LoadFuel(10);
Console.WriteLine(tars.ToString());
tars.Walk(23, -51);
Console.WriteLine(tars.ToString());

Submarine u12 = new Submarine("u12", 1200);
Console.WriteLine(u12.ToString());
u12.LoadFuel(10);
Console.WriteLine(u12.ToString());
u12.Dive(800);
Console.WriteLine(u12.ToString());

enum State
{
    Online,
    OutOfService,
    OnTheMove,
    Dive,
    Destroyed
}
class Vehicle
{
    public string Name { get; set; }
    public float FuelLevel { get; set; }
    protected State State { get; set; }

    public Vehicle(string name, float fuelLevel)
    {
        Name = name;
        FuelLevel = fuelLevel;
        State = State.Online;
    }
    public override string ToString()
    {
        return $"{this.Name}. Yakıt {this.FuelLevel}. Durum {this.State}";
    }
    public void LoadFuel(float amount)
    {
        Console.WriteLine($"{amount} litre yakıt yükleniyor...");
        this.FuelLevel += amount;
    }
}

class Robot
    : Vehicle
{
    public Robot(string name, float fuel)
        : base(name, fuel)
    {
    }

    public void Walk(float x, float y)
    {
        Console.WriteLine($"{x},{y} noktasında hareket halinde");
        this.State = State.OnTheMove;
    }
}

class Submarine
    : Vehicle
{
    public Submarine(string name, float fuel)
        : base(name, fuel)
    {
    }
    public void Dive(int depth)
    {
        Console.WriteLine($"{depth} metreye dalıyor");
        this.State = State.Dive;
    }
}
```

Aslında Java ve C# tarafından gelenler için gayet sade ve anlaşılır bir düzenek olduğunu belirtebiliriz. Alt tiplerin yapıcı metotlarından üst tiplere base anahtar kelimesi yardımıyla veri gönderilebilmektedir. ToString fonksiyonu ortak özellikleri tuttuğu için Vehicle sınıfında yeniden yazılmıştır. Ayrıca LoadFuel operasyonu bu ortak tipe taşınmıştır. Bu örneği çalıştırdığımızda aşağıdaki ekran görüntüsündekine benzer sonuçlar alırız.

![oop_3.png](/assets/images/2022/oop_3.png)

Kalıtımı kullanmanın sebepleri arasında tekrarlı kod bloklarını engellemeyi, türler için ortak olan özellik ve fonksiyonellikleri bir noktada toplamayı vs sayabiliriz. Gerçi ben sınıf bazlı kalıtım yerine Interface kullanmaktan ve yine de gerekliyse ortak veri ve fonksiyonları tutan sınıfların alt sınıflarda birer özellik gibi kullanılmasından yanayım (Bir nevi Composition diyebilir miyiz?) Elbette bu tartışmaya açık bir konu. Biz şimdi rust cephesinden duruma bakalım.

Rust tarafında bu tip bir kalıtım formasyonu mevcut değil ama ortak fonksiyonellikleri birer davranış gibi düşünürsek trait enstrümanını kullanabiliriz. Buna göre Submarine ve Robot veri yapılarının ortak özellikleri için farklı bir yaklaşıma gitmeliyiz. Bunun için Composition göz önüne alınabilir. Aynen aşağıdaki kod parçasında olduğu gibi.

```rust
use std::fmt::{Display, Formatter};

fn main() {
    let mut tars = Robot::new(String::from("TARS"), 80.0);
    // Tabii ortak özellikleri vehicle alanında tutuyoruz ve onun Display özelliğini kullanmalıyız.
    // Bu nedenle tars.vehicle şeklinde bir kullanım söz konusu.
    println!("{}", tars.vehicle);
    tars.vehicle.load_fuel(10.0);
    println!("{}", tars.vehicle);
    tars.walk(24.0, -50.9);
    println!("{}", tars.vehicle);

    let mut u12 = Submarine::new(String::from("u12"), 1400.10);
    println!("{}", u12.vehicle);
    u12.vehicle.load_fuel(100.90);
    println!("{}", u12.vehicle);
    u12.dive(800);
    println!("{}", u12.vehicle);
}

// C# Örneğindeki gibi robotun anlık durumu için burada da bir enum kullanıyoruz.
// Tabii rust için enum bir veri yapısıdır. Zenginleştirilebilir. OnTheMove ve Dive alanlarında olduğu gibi.
#[allow(dead_code)]
enum State {
    Online,
    OutOfService,
    OnTheMove(Location), // Bonus :) Rust enum veri yapısının zenginliğini kullandık. OnTheMove state'indeyken örneğin lokasyonunu da tutalım dedik.
    Dive(i32),           // Bonus
    Destroyed,
}

struct Location {
    pub x: f32,
    pub y: f32,
}
struct Vehicle {
    pub name: String,
    pub fuel_level: f32,
    state: State,
}
impl Vehicle {
    pub fn new(name: String, fuel_level: f32) -> Self {
        Self {
            name,
            fuel_level,
            state: State::Online,
        }
    }
}

// Hem Robot hem de Submarine için ortak olan load_fuel fonksiyonelliğini bir trait olarak tanımladık
trait Fuel {
    fn load_fuel(&mut self, amount: f32);
}

// Tanımladığımız trait'i Vehicle tipi için uyguladık.
impl Fuel for Vehicle {
    fn load_fuel(&mut self, amount: f32) {
        println!("{} litre yakıt yükleniyor.", amount);
        self.fuel_level += amount
    }
}

impl Display for Vehicle {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{}. Yakıt {}. Durum {}",
            self.name, self.fuel_level, self.state
        )
    }
}

// Robot veri yapısı Vehicle türünden bir özellik barındırıyor.
// Bu Submarine için de uygulanıyor. Composition yaptığımızı düşünebiliriz.
struct Robot {
    pub vehicle: Vehicle,
}

impl Robot {
    // C# tarafındaki base constructor'ı çağırma işlevselliğini uygulamaya çalıştık diyebiliriz.
    // Aslında Robot'un içerdiği Vehicle nesnesini örnekleyip onu taşıyan bir Robot değişkeni dönüyoruz.
    pub fn new(name: String, fuel_level: f32) -> Self {
        Self {
            vehicle: Vehicle::new(name, fuel_level),
        }
    }

    // Bu robota has bir fonksiyon.
    pub fn walk(&mut self, x: f32, y: f32) {
        println!("Hareket halinde");
        self.vehicle.state = State::OnTheMove(Location { x, y });
    }
}

struct Submarine {
    pub vehicle: Vehicle,
}

impl Submarine {
    pub fn new(name: String, fuel_level: f32) -> Self {
        Self {
            vehicle: Vehicle::new(name, fuel_level),
        }
    }

    // Submarine'e özgün fonksiyon
    pub fn dive(&mut self, depth: i32) {
        println!("{} metreye dalıyor", depth);
        self.vehicle.state = State::Dive(depth);
    }
}

impl Display for State {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        let state = match self {
            State::Online => "Çalışıyor.".to_string(),
            State::Destroyed => "Yok edildi".to_string(),
            State::OutOfService => "Servis dışı".to_string(),
            State::OnTheMove(l) => format!("{},{} noktasında hareket halinde.", l.x, l.y),
            State::Dive(m) => format!("{} metreye iniyor.", m),
        };
        write!(f, "{}", state)
    }
}
```

Submarine ve Robot veri yapılarının ortak özellikleri Vehicle veri yapısına alındı. Bu nedenle hem Robot hem Submarine, bu türden vehicle isimli birer alana sahip. Bir nevi Composition gibi düşünebiliriz. Tabii C# örneğinde ortak fonksiyon olarak kullanılan load_fuel metodunu üst türde konuşlandırmıştık. Rust ile yazdığımız kod parçasında bu iş için bir trait kullanmaktayız. İlgili trait'i Vehicle için yazdığımızda Submarine ve Robot için de kullanabilir hale gelmekte. Nitekim her ikisi de Vehicle'a sahip ve dolayısıyla Fuel trait'i istenen yakıt yükleme davranışını pekala uygulayabilir. Ancak şunu hatırlatmakta yarar var; Rust tam anlamıyla nesne yönelimli paradigmayı hedeflemiş bir dil değildir. Yine de basit bir has-a ilişkisi ve trait'leri kullanarak benzer işlevsellikleri sağladık diyebiliriz. İşte son koduyla birlikte örneğin çalışma zamanı çıktısı.

![oop_4.png](/assets/images/2022/oop_4.png)

## Polymorphism Konusu

Çok biçimlilikte üst sınıfa ait nesne örneklerinin yeri gelince kendisinden türeyen alt sınıf örnekleri gibi hareket edebilmesi söz konusudur. Özellikle Interface tipi kullanıldığında, bağımlılıkların soyutlanması oldukça kolaylaşır ve bu bizi Dependency Injection Container'larının kullanıldığı büyük ölçekli çözümlere de götürür. Çalışma zamanında çözümlenen nesne örnekleri söz konusu olduğunda bu kabiliyet epey işe yaramaktadır. İlk olarak C# tarafından olaya bakalım.

```csharp
Robot tars = new Robot("TARS", 80);
Submarine u12 = new Submarine("u12", 1200);
Submarine alpha = new Submarine("Alpha", 5000);

var vehicles = new List<IAbility> { tars, u12, alpha };
foreach (var v in vehicles)
{
    v.SetTools();
}

enum State
{
    Online,
    OutOfService,
    OnTheMove,
    Dive,
    Destroyed
}

interface IAbility
{
    void SetTools();
}

abstract class Vehicle
{
    public string Name { get; set; }
    public float FuelLevel { get; set; }
    protected State State { get; set; }

    public Vehicle(string name, float fuelLevel)
    {
        Name = name;
        FuelLevel = fuelLevel;
        State = State.Online;
    }
    public override string ToString()
    {
        return $"{this.Name}. Yakıt {this.FuelLevel}. Durum {this.State}";
    }
    public void LoadFuel(float amount)
    {
        Console.WriteLine($"{amount} litre yakıt yükleniyor...");
        this.FuelLevel += amount;
    }
}

class Robot
    : Vehicle, IAbility
{
    public Robot(string name, float fuel)
        : base(name, fuel)
    {
    }

    public void SetTools()
    {
        Console.WriteLine($"{base.Name} için termal görüş sistemi, oksijen seviyesi ölçer yükleniyor.");
    }

    public void Walk(float x, float y)
    {
        Console.WriteLine($"{x},{y} noktasında hareket halinde");
        this.State = State.OnTheMove;
    }
}

class Submarine
    : Vehicle, IAbility
{
    public Submarine(string name, float fuel)
        : base(name, fuel)
    {
    }
    public void Dive(int depth)
    {
        Console.WriteLine($"{depth} metreye dalıyor");
        this.State = State.Dive;
    }

    public void SetTools()
    {
        Console.WriteLine($"{base.Name} için sonar, derinlik ölçer, ek batarya yükleniyor.");
    }
}
```

Yeni sürümde IAbility isimli bir arayüz (interface) bulunuyor. İçerisinde sadece SetTools isimli bir fonksiyon yer alıyor ve uygulandığı araç için bir takım alet edavatları yükleme işini üstlenecek davranışı tanımlıyor. Bu arada davranış (Behavior) Rust tarafı için anahtar kelime de olabilir. Robot ve Submarine türleri bu arayüzü uyguladığından CallTools isimli bir metot yazmamız pekala mümkün. Parametre olarak IAbility arayüzünü uygulayan her nesne bu fonksiyona girebilir ve kendisi için yazılmış SetTools metodunu icra edebilir. İşte çalışma zamanı çıktımız.

![oop_5.png](/assets/images/2022/oop_5.png)

Şimdi olayı Rust tarafında aşağıdaki kod parçaları ile değerlendirmeye çalışalım.

```rust
use std::fmt::{Display, Formatter};

fn main() {
    let tars = Robot::new(String::from("TARS"), 80.0);
    let u12 = Submarine::new(String::from("u12"), 1400.10);
    let alpha = Submarine::new(String::from("alpha"), 2000.0);
    call_tools(tars);
    call_tools(u12);
    call_tools(alpha);
}

// Burada trait'in generic sürümüne başvurulur.
// Ability trait'ini uygulamış türler bu fonksiyona girebilir.
// Lakin Rust derleyicisi yukarıdaki call_tools çağrılarına göre her tip için aşağıdaki fonksiyonu yeniden yazıp derlenmiş koda gömer.
// Bunun tabii bir maliyeti olacaktır.
fn call_tools<T: Ability>(mut a: T) {
    a.set_tools();
}

#[allow(dead_code)]
enum State {
    Online,
    OutOfService,
    OnTheMove(Location),
    Dive(i32),
    Destroyed,
}

struct Location {
    pub x: f32,
    pub y: f32,
}
struct Vehicle {
    pub name: String,
    pub fuel_level: f32,
    state: State,
}
impl Vehicle {
    pub fn new(name: String, fuel_level: f32) -> Self {
        Self {
            name,
            fuel_level,
            state: State::Online,
        }
    }
}

// Araçların çeşitli alet ve edavatları yüklemesi için kodlayabilecekleri bir fonksiyon tanımladık.
trait Ability {
    fn set_tools(&mut self);
}

struct Robot {
    pub vehicle: Vehicle,
}

impl Robot {
    pub fn new(name: String, fuel_level: f32) -> Self {
        Self {
            vehicle: Vehicle::new(name, fuel_level),
        }
    }
}

// Ability trait'ini Robot türü için uyarladık
impl Ability for Robot {
    fn set_tools(&mut self) {
        println!(
            "{} için termal görüş sistemi, oksijen seviyesi ölçer yükleniyor.",
            self.vehicle.name
        );
    }
}

struct Submarine {
    pub vehicle: Vehicle,
}

impl Submarine {
    pub fn new(name: String, fuel_level: f32) -> Self {
        Self {
            vehicle: Vehicle::new(name, fuel_level),
        }
    }
}

// Ability trait'ini Submarine türü için uyarladık
impl Ability for Submarine {
    fn set_tools(&mut self) {
        println!(
            "{} için sonar, derinlik ölçer, ek batarya yükleniyor.",
            self.vehicle.name
        );
    }
}
```

Interface yok ama benzer kabiliyetleri sergileyecek trait'imiz var. Dolayısıyla Ability isimli bir enstrüman tanımladık. Bu davranışı Robot ve Submarine veri yapıları için de uyarladık. call_tools fonksiyon dikkat edileceği üzere generic parametre almakta. Rust tarafında generic kavramı yok zannediyorsanız yanılıyorsunuz:) Örnekteki generic tipin en önemli özelliği ise Ability trait'ini uygulama zorunluluğu getirmesi. Esasında bu ilk örnekte static dispatch adı verilen görünmez bir kullanım şekli söz konusu. Bu tekniğe göre call_tools fonksiyonu için derleyici Ability'nin uygulandığı her nesne adına birer call_tools fonksiyonu hazırlar ve içinde uygun olan nesnenin set_tools fonksiyonunu çağırır.

![oop_6.png](/assets/images/2022/oop_6.png)

Static Dispatch dışında ikinci bir kullanım şekli de dynamic dispatch yöntemidir. Bu yöntem özellikle kütüphanelerin kullanıldığı senaryolar için daha uygundur. Nitekim çok biçimli olması muhtemel trait'lerin uygulandığı tipler derleme zamanında belli olamayacağından bu bağlamları çalışma zamanında icra etmek gerekecektir. Bunun için kodu aşağıdaki gibi değiştirerek ilerleyelim.

```rust
fn main() {
    let mut tars = Robot::new(String::from("TARS"), 80.0);
    let mut u12 = Submarine::new(String::from("u12"), 1400.10);
    let mut alpha = Submarine::new(String::from("alpha"), 2000.0);
    // static dispatch
    // call_tools(tars);
    // call_tools(u12);
    // call_tools(alpha);

    // dynamic dispatch
    call_tools_dynamic(&mut tars);
    call_tools_dynamic(&mut u12);
    call_tools_dynamic(&mut alpha);
}

// Static Dispatch
// Burada trait'in generic sürümüne başvurulur.
// Ability trait'ini uygulamış türler bu fonksiyona girebilir.
// Lakin Rust derleyicisi yukarıdaki call_tools çağrılarına göre her tip için aşağıdaki fonksiyonu yeniden yazıp derlenmiş koda gömer.
// fn call_tools<T: Ability>(mut a: T) {
//     a.set_tools();
// }

// Bir diğer alternatif yolda Dynamic Dispatch kullanımıdır.
// Özellikle library geliştiriyorsak Ability trait'ini asıl uygulayan tipi bilemeyebiliriz. Bunu runtime'da çözümlemek adına
// dyn anahtar kelimesinden yararlanırız.
fn call_tools_dynamic(a: &mut dyn Ability) {
    a.set_tools();
}
```

Hem static hem dynamic dispatch kullanımları aynı sonuçları verecektir.

![oop_7.png](/assets/images/2022/oop_7.png)

Esasında üst türden nesne kullanan koleksiyonlar nesne yönelimli dünyada sıklıkla kullanılırlar. Örneğin C# tarafında aşağıdaki gibi bir kullanım pekala mümkündür ve yazımı oldukça kolaydır.

```csharp
List<IAbility> abilities=new List<IAbility>{tars,u12,alpha};
foreach (var a in abilities)
{
    a.SetTools();
}
```

abilities isimli generic liste koleksiyonu IAbility arayüzünü uyarlayan nesneleri taşıyabilir. Arayüz kendisini uygulayan nesneyi taşıyabildiği için onun uyguladığı gerçek fonksiyonu da pekala kullanabilir. Bu nedenle foreach döngüsündeki gibi bir çalışma mümkündür. Döngünün her bir iterasyonunda o anki nesne örneği kimse, ona ait set_tools fonksiyonu yürütülür. Aynı işlevselliği dynamic dispatch'in kullanıldığı rust senaryosunda aşağıdaki gibi yapabiliriz. Peki sizce bunu static dispatch uygulandığı durumda da yapabilir miyiz? İşte size güzel bir ödev;)

```rust
fn main() {
    let mut tars = Robot::new(String::from("TARS"), 80.0);
    let mut u12 = Submarine::new(String::from("u12"), 1400.10);
    let mut alpha = Submarine::new(String::from("alpha"), 2000.0);

    let mut abilities: Vec<&mut dyn Ability> = vec![];
    abilities.push(&mut tars);
    abilities.push(&mut u12);
    abilities.push(&mut alpha);

    for a in abilities {
        a.set_tools();
    }
}
```

Böylece geldik bir Rust pratiğinin daha sonuna. Bu pratikte nesne yönelimli dillerin önemli kabiliyetleri arasında yer alan kalıtım ve çok biçimli olma hallerine hem C# hem de Rust cephesinden bakmaya çalıştık. Örnek kodlara her zaman olduğu gibi [github reposu üzerinden erişebilirsiniz](https://github.com/buraksenyurt/rust-farm/tree/main/Practices/oop). Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
