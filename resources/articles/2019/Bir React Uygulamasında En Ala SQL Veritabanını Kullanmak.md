---
title: "Bir React Uygulamasında En Ala SQL Veritabanını Kullanmak"
pubDate: 2019-11-11 10:30:00
categories:
  - React
tags:
  - React
  - Bash
  - JSON
  - NoSQL
  - HTTP
  - JavaScript
  - GitHub
---

# Bir React Uygulamasında En Ala SQL Veritabanını Kullanmak
![image.axd](images/image.axd)

İngilizcede bazen gemi kaptanlarına Captain yerine Skipper dendiğini biliyor muydunuz? Aslında Hollandalıların schipper, schip en nihayetinde de ship kelimelerinden türeyerek gelmiş bir ifade. Her ikisi de kaptanı ifade etmekte ama Skipper daha çok bir hitap şekli. Hatta yer yer takım kaptanları veya uçak pilotları için de kullanılıyor. Skipper kelimesinin kullanıldığı yerleri düşününce aklıma The Hunt For Red October filminde USS Dallas kaptanı Mancuso'nun CIA'den Jack Ryan'a "That's right? Skipper's Ramius?" demesi geliyor.

Esasında bu hitap şeklinin bana anımsattığı daha güzel şeyler var. Blizzard geliştiricilerinin Warcraft II'sini oynadığım zamanlarda insan kuvvetlerindeki gemilere Skipper diye sesleniliyordu. Karakterlerin o müthiş ses efektleri hala aklımda. "Ay ay sör", "Ayy keptın", "Set seyıl", "Sıkipp?", "Andır veyy!":D Yazılı olarak seslendirmeye çalıştım ama dinleseniz çok daha iyi olabilir. Diğer pek çok karakterin sesi de harikaydı. Mesela köylülerin "Yeş mi lord" diyişindeki şirinlik ya da okçuların tonlamasındaki keskinlik. Youtube'dan silinene kadar [şu adresten dinleyebilir](https://www.youtube.com/watch?v=6wkc4uCaLpw) veya aratabilirsiniz. Bugünkü konumuza gündem olmasının sebebi ise skipper isimli bir nesne dizisini kullanacak olmamız. Öyleyse başlayalım.

Öğrenecek bir çok şeyler araştırırken (ki samimi olmak gerekirse 24 saat uykusuz kalıp bir şeyleri öğrensek bile zamanın yetmeyeceği ve güneşe daha uzak bir gezegende yaşamamız gerektiği ortaya çıkıyor) [AlaSQL](http://alasql.org/) isimli bir çalışma ile karşılaştım. Tarayıcı üzerinde çalışabilen istemci taraflı bir In-Memory veritabanı olarak geçiyor. Tamamen saf Javascript ile yazılmış. Geleneksel ilişkisel veritabanı özelliklerinin çoğunu barındırıyor. Group, join, union gibi fonksiyonellikleri karşılıyor. In-Memory tutulan veriyi kalıcı olarak saklamakta mümkün. Hatta bu noktada localStorage olarak ifade edilen yerel depolama alanlarından veri okunup tekrar yazılabiliyor. IndexedDB veya Excel gibi ürünleri fiziki repository olarak kullanabiliyor. Ayrıca JSON nesnelerle çalışabiliyoruz ki bu da NoSQL desteği anlamına gelmekte. Bu nedenle SQL ve NoSQL rahatlığını bir arada sunan hafif bir veritabanı gibi düşünülebilir.

Açık kaynak kodlu, dokümantasyonu oldukça zengin bir proje. Yine de endüstriyel anlamda olgunlaştığına dair emareler görülmeden canlı ortamlarda kullanmak riskli olabilir diye düşünürken github projesinden çıkıp org alan adına geçerek biraz daha ciddiye alınmaya başladığını fark ettim. Yine de deneysel çalışmalarda ele almakta yarar var. Benim [25nci cumartesi gecesi çalışması](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2025%20-%20AlaSQL%20on%20React)ndaki amacım onu yalın bir React uygulamasında deneyimlemeye çalışmaktı. Öyleyse gelin notlarımızı toparlamaya başlayalım.

## Kurulum ve Hazırlıklar

Ben her zaman olduğu gibi örneğimi WestWorld (Ubuntu 18.04, 64bit) üzerinde deneyimledim. Ancak komutlar plaform bağımsız olarak ele alınabilir. Ah bu arada sisteminizde node'un yüklü olduğunu varsayıyorum. React uygulamasını kolayca oluşturabilmek için aşağıdaki terminal komutunu kullanabiliriz.

```bash
npx create-react-app submarine
```

![image.axd](images/image.axd)

AlaSQL'i kullanabilmek içinse uygulama klasöründe gerekli npm paketinin yüklenmesi yeterli olacaktır. Ayrıca görselliği zenginleştirmek için ben Bootstrap'i kullanmayı tercih ettim.

```bash
cd submarine
npm install --save-dev alasql bootstrap
```

React şablonu aslında senaryomuzdan bağımsız bir çok gereksiz dosya içerebilir. Bunları silip manifest.json içeriğini bir parça değiştirebiliriz. Örneğin uygulamanın tanımlayıcısı olan short_name ve name değerlerini aşağıdaki hale getirebiliriz.

```json
{
  "short_name": "Tac-War-Mag",
  "name": "Tactical World Magazine",
// diğer kısımlar
```

Pek tabii en önemli kısım App.js dosyasında yapılanlar. Burası uygulamanın ayağa kalktıktan sonra oluşturulan ana bileşeni (component) Ana sayfanın HTML içeriği ile birlikte SQL ilişkili kodlarını barındırmakta. Ben mümkün mertebe içeriği yorum satırları ile zenginleştirerek açıklamaya çalıştım.

```text
import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css'; // az biraz bootstrap ile görselliği düzeltelim
import * as alasql from 'alasql'; // alasql ile konuşmamızı sağlayacak modül bildirimimiz

class App extends Component {

  /* yapıcı metod gibi düşünebiliriz sanırım
  genellikle local state değişkenlerini başlatmak ve onlara değer atamak
  için kullanılır
  */
  constructor(props) {
    super(props);

    /* 
    state'i değişebilir veriler için kullanırız. state değişikliğinde
    bileşenin otomatik olarak yeniden render edilmesi söz konusu olur
    */
    this.state = { skippers: [] };
  }

  /*
  componentWillMount metodu, ilgili bileşen Document Object Model'e bağlanmadan
  önce çalışır.
  Bizim örneğimizde veritabanını ve tablo kontrolünün yapılması ve
  yoklarsa yaratılmaları için ideal bir yerdir
  */
  componentWillMount() {
    /*
    Klasik SQL ifadeleri ile TacticalWorldDb isimli bir veritabanı olup
    olmadığını kontrol ediyor ve eğer yoksa oluşturup onu kullanacağımızı belirtiyoruz.
    SQL ifadelerini çalıştırmak için alasql metodunu çağırmak yeterli.
    */
    alasql(`
            CREATE LOCALSTORAGE DATABASE IF NOT EXISTS TacticalWorldDb;
            ATTACH LOCALSTORAGE DATABASE TacticalWorldDb;
            USE TacticalWorldDb;            
            `);

    /*  
      Şimdi tablomuzu ele alalım. Submarine isimli tablomuzda
      id, name, displacement ve country alanları yer alıyor. 
      Id alanı için otomatik artan bir primary key'de kullandık.
      Örneği abartmamak adına alan dozajını belli bir seviyede tuttuk.
    */
    alasql(`
            CREATE TABLE IF NOT EXISTS Submarine (
              id INT AUTOINCREMENT PRIMARY KEY,
              name VARCHAR(25) NOT NULL,
              displacement NUMBER NOT NULL,
              country VARCHAR(25) NOT NULL
            );
          `);
  }
  /*
  İlk satırda yer alan alasql komutu ile Submarine tablosundaki verileri displacement değerine
  göre büyükten küçüğe sıralı olacak şekilde çekiyoruz.
  Ardından state içeriğini bu tablo verisiyle ilişkilendiriyoruz.
  */
  getAll() {
    const submarineTable = alasql('SELECT * FROM Submarine ORDER BY displacement DESC');
    this.setState({ skippers: submarineTable });
    // console.log(submarineTable); // Kontrol amaçlı açıp F12 ile geçilecek kısımda Console sekmesinden takip edebiliriz. Bir JSON array olmasını bekliyoruz
  }

  /*
  Bileşen DOM nesnesine bağlandıktan sonra çalışan metodumuzdur.
  Burası örneğin tablo içeriğini çekip state nesnesine almak için
  son derece ideal bir yerdir.
  */
  componentDidMount() {
    this.getAll();
  }

  /*
  Yeni bir satır eklemek için aşağıdaki metodu kullanacağız.
  denizaltının adı, tonajı ve menşei gibi bilgileri
  this.refs özelliği üzerinden yakalyabiliriz. this.refs DOM
  elemanlarına erişmek için kullanılmakta. Bu şekilde 
  formdaki input kontrollerini yakalayıp value niteliklerini
  okuyarak gerekli veriyi çekebiliriz

  Insert sorgusu için yine alasaql nesnesinden yararlanıyoruz.
  Bu sefer parametre içeriğini tek ? içerisinde yollamaktayız.
  Parametre değerleri aslında bir json nesnesi içinden yollanıyor.
  key olarak kolon adını, value olarak da refs üzerinden gelen bileşene ait value özelliğini veriyoruz.
  Id alanının otomatik arttırmak içinse autoval fonksiyonu devreye girmekte.

  Pek tabii yeni eklenen kayıt nedeniyle bileşeni güncellemek lazım.
  getAll metodu burada devreye girmekte
  */
  addSkipper() {
    const { name, displacement, country } = this.refs;
    if (!name.value) return;
    // console.log(dicplacement.value); // Kontrol amaçlı. Browser'dan F12 ile değerlere bakılabilir
    alasql('INSERT INTO Submarine VALUES ?',
      [{
        id: alasql.autoval('Submarine', 'id', true),
        name: name.value,
        displacement: displacement.value,
        country: country.value
      }]
    );
    this.getAll();
  }

  /*
  Silme operasyonunu yapan metodumuz.
  Parametre olarak gelen id değerine göre bir DELETE ifadesi çağırılı
  ve tüm liste tekrardan çekilir.
  */
  deleteSkipper(id) {
    alasql('DELETE FROM Submarine WHERE id = ?', id);
    this.getAll();
  }

  /* 
    State değişikliği gibi durumlarda bileşen güncellenmiş demektir. 
    Bu durumda render fonkisyonu devreye girer.

    render metodu bir HTML içeriği döndürmektedir.
    form sınıfındaki input kontrollerinin ref niteliklerine dikkat edelim.
    Bunları addSkipper metodunda this.refs ile alıyoruz.

    iki button bileşenimiz var ve her ikisinin onClick metodları ilgili fonksiyonları
    işaret ediyor.

    HTML sayfası iki kısımdan oluşuyor. Yeni bir veri girişi yaptığımız form ve tablo verisini
    gösteren bölüm. Tablo içeriğini birer satır olarak ekrana basmak için map fonksiyonundan
    yararlanıyoruz. map fonksiyonu lambda görünümlü blok içerisine sırası gelen satır bilgisini
    atıyor. Örnekte ship isimli değişken bu taşıyıcı rolünü üstlenmekte. ship değişkeni üzerinden
    tablo kolon adlarını kullanarak asıl verilere ulaşıyoruz.
  */
  render() {

    const { skippers } = this.state;

    return (
      <main className="container">
        <h2 className="mt-4">En Büyük Denizlatılar</h2>
        <div className="row mt-4">
          <form>
            <div className="form-group mx-sm-3 mb-2">
              <input type="text" ref="name" className="form-control" id="inputName" placeholder="Sınıfı" />
            </div>
            <div className="form-group mx-sm-3 mb-2">
              <input type="text" ref="displacement" className="form-control" id="inputDisplacement" placeholder="Tonajı" />
            </div>
            <div className="form-group mx-sm-3 mb-2">
              <input type="text" ref="country" className="form-control" id="inputCountry" placeholder="Sahibi..." />
            </div>
            <div className="form-group mx-sm-3 mb-2">
              <button type="button" className="bnt btn-primary mb-2" onClick={e => this.addSkipper()}>Ekle</button>
            </div>
          </form>
        </div>

        <div>
          <table className="table table-primary table-striped">
            <thead>
              <tr>
                <th scope="col">Sınıfı</th>
                <th scope="col">Tonajı</th>
                <th scope="col">Ülkesi</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {
                skippers.length === 0 && <tr>
                  <td colSpan="5">Henüz veri yok</td>
                </tr>
              }
              {
                skippers.length > 0 && skippers.map(ship => (
                  <tr>
                    <td>{ship.name}</td>
                    <td>{ship.displacement}</td>
                    <td>{ship.country}</td>
                    <td>
                      <button className="btn btn-danger" onClick={e => this.deleteSkipper(ship.id)}>Sil</button>
                    </td>
                  </tr>
                ))
              }
            </tbody>
          </table>
        </div>
      </main >
    );
  }
}

export default App;
```

Aslında CRUD (Create Read Update Delete) operasyonlarını sunmaya çalıştığımız bir arayüz var. Senaryomuzda dünyanın en büyük denizaltılarını listelediğimiz, ekleyip çıkarttığımız bir web bileşeni söz konusu. Tahmin edeceğiniz üzere benim hep atladığım bir şey daha var...Güncelleme eksik:| Artık o kısmını da siz değerli okurlarıma bırakıyorum.

Öyleyse aşağıdaki terminal komutunu vererek uygulamamızı çalıştıralım (Bu arada react uygulamasını şablondan oluşturduğumuz için package.json içerisindeki scripts kısmı otomatik olarak ayarlanmıştır. start anahtar kelimesini kullanmamızın sebebi bu)

```bash
npm run start
```

İşte çalışma zamanına ait bir kaç görüntüsü.

Her şey yeni başlarken ve hiç veri yokken ana sayfa aşağıdaki gibi açılmalıdır.

![image.axd](images/image.axd)

Bir kaç satır ekledikten sonraki durum ise şöyle olacaktır.

![image.axd](images/image.axd)

## Local Storage Nerede?

Peki veriyi tarayıcımız nerede tutuyor? Sonuçta In-Memory bir veritabanı olduğundan bahsediyoruz. Lakin içerik tarayıcı tarafında bir alanda konumlanıyor. Varsayılan senaryoda veri Local Storage bölümünde depolanmakta. Uygulamayı çalıştırdıktan sonra Chrome DevTools'a geçip Application sekmesine giderek içeriğini görebiliriz. Dikkat edileceği üzere TacticalWorldDb.Submarine isimli bir tablo bulunuyor ve verilerimiz içerisinde JSON nesneler olarak tutuluyor (Diğer yandan depolama alanı olarak componentWillAmount metodu içerisindeki SQL komutumuzda LocalStorage'ı ifade ettiğimizi hatırlayalım)

![image.axd](images/image.axd)

Storage sekmesine bakarsak Local Storage dışında IndexedDB seçeneği de bulunmaktadır. Eğer bu alanı kullanmak istersek

```text
ATTACH INDEXEDDB DATABASE TacticalWorldDB
```

gibi bir SQL ifadesinden yararlanmamız gerekiyor. Tabii bu arada önemli bir soru da gündeme geliyor. Uygulamayı kapattığımızda veriye ne olacak? Bunu cevaplayabilmeniz için kodu buraya kadar geliştirmiş olmanız gerekiyor:)

## Ben Neler Öğrendim?

Bazen sıfırdan başlanacak bir ürün için ya da lisans maliyetleri ve diğer sebepler nedeniyle modernize edilecek bir proje için verinin nerede neyle tutulacağını araştırmak isteyebiliriz. Böyle durumlarda alternatifleri POC (Proof of Concept) tadındaki deneysel programlarda denemek faydalıdır. Bende buna istinaden AlaSQL'i incelemiştim. Yanıma kar olarak kalanlarsa şöyle.

- Hazır bir react uygulama iskeletinin nasıl oluşturulduğunu
- React sayfası içerisindeki yaşam döngüsüne dahil olan componentWillMount, componentDidMount ve render metodlarının hangi aşamalarda devreye girdiğini
- Alasql paketinin react uygulamasına nasıl dahil edildiğini ve temel SQL ifadelerini (veritabanı nesnelerini oluşturmak, insert ve delete sorgularını çalıştırmak vb)
- state özelliğini ne amaçla kullanabileceğimi
- Veritabanından çekilen JSON dizisinin map fonksiyonu ile nasıl etkileştiğini
- refs özelliği ile kontrollerin metotlarda nasıl ele alınabildiğini

Böylece geldik bir [saturday-night-works](https://github.com/buraksenyurt/saturday-night-works) notu derlemesinin daha sonuna. Yolun açık olsun Skipper:) Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
