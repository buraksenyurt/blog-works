---
title: "gRPC Nedir, Nasıl Uygulanır?"
pubDate: 2019-04-24 07:06:00
categories:
  - Node.js
tags:
  - Node.js
  - JavaScript
  - .NET
  - MongoDB
  - WCF
  - REST
  - JSON
  - Web Service
  - HTTP
  - gRPC
  - Performance
  - Serialization
  - Microservices
  - GitHub
---

# gRPC Nedir, Nasıl Uygulanır?
![image.axd](images/image.axd)

Oturduğum masanın hemen sağındaki pencerede ufak bir aralık kalmış olmalı ki "Vuuuu..." diye öten rüzgarın sesini fark ettim bir anda. Aslında işime konstantre olmuştum ama o sesle birlikte olduğum yerden uzaklaşmıştım. Dışarısı soğuktu. Havan kapalı ve biraz da kasvetliydi. Yağmur damlalarının cama vuruşunu fark ettim sonrasında. Gözlerim uzaklara daldı ve rüzgarın sesi ile birikte Lise yıllarında buldum kendimi. Doğal gaz İstanbul'a henüz gelmiş ve bizimki gibi kimi sobalı ev kalorifer petekleriyle tanışmıştı. Geçmiş yılların ardından bu yeni konfor sayesinde sabahları üşüyerek uyanmıyorduk artık. Benim en büyük keyiflerimden birisi olan öğle sonrası uykularım içinse yeni mekanımı bulmuştum bile. Okuldan her geldiğimde önce karnımı doyuruyor sonra o köşeye kuruluyordum. Minderler camın altındaki peteğin yanına seriliyor, üste hafif bir battaniye alınıyor, zaten sessiz olan sokağın sükunetini bozmak isteyen yağmur damlalarının cama vuruşu dinlenerek derin bir uykuya dalınıyor. Bazen de o sessizliğe eski sokak kapısının altındaki süngerden kaçan "Vuuuu" sesi ortak olurdu...

Kısa süreli bu seyahatin ardından tekrar bilgisayarıma döndüm ve araştırdığım konu üzerinde ilerlemeye başladım. Son zamanlarda micro service mimarisinin uygulanması ile ilgili bir çok doküman okumaktayım. Şirket bünyesinde bu tip bir oluşuma gidilmesi de araştırmalarımda beni motive etmekte. Tabii uygulama pratiklerine, çeşitli desenlerin kullanılışına baktıkça bilmediğim bir çok şey görüyorum. Bugüne kadar özellikle microservisler seviyesinde uçlar arasındaki iletişimde hep REST/HTTP1 tabanlı haberleşildiğini düşünüyordum. Oysa ki HTTP/2 ile stream desteği veren, TCP soket haberleşmesini benimseyen, binary serileştirmeyi kullanan ve REST/HTTP1 e göre 2.5 kat daha hızlı olduğu söylenen gRPC isimli bir çatı da varmış (Gerçi.Net'in en başından beri var olan arkadaşlarım, TCP, Binary serileştirme denilince.Net Remoting konusunu ve onun WCF içerisindeki evrimini gayet iyi hatırlayacaklardır) Bende bunu görünce gRPC'nin ne olduğunu araştırmaya başladım.

İşte bu yazımızda Google'ın protobuf protokolü üzerine kurguladığı ve Remote Procedure Call modelinde hizmet sunan gRPC isimli çatısını inclemeye çalışacağız. Özellikle dağıtık sistemlerde taraflar arası haberleşmede TCP bazlı binary serileştirme ilkelerine dayanan bu protokol REST'in standart iletişim teknikleri yerine daha çok tercih edilmeye (önerilmeye) başlanmış görünüyor. Google'ın bu çatıyı geliştirmekteki temel amacının, binary serileştirmenin performans avantajını kullanıp Remote Procedure Call tekniğini microservice sistemlerde uygulayabilmek olduğun düşünüyorum.

İşin temelleri oldukça basit aslında. RPC'ye göre istemciler, uzak sunucudaki metodları sanki kendi ortamlarının birer parçasıymış gibi çağırabilirler. Taraflar farklı makinelerde dolayısıyla farklı domain sınıflarında olabilirler. Bu nedenle dağıtık sistem kurgularında ideal bir uygulama senaryosu olarak ele alınabileceğini ifade edebiliriz. Aşağıdaki şekille konuyu özetlemek mümkün. Node.js ile geliştirilmiş örnek bir gRPC sunucusu ile protoBuf bazlı mesajlarla anlaşan farklı dillerde istemcier veya servisler. Aslında oldukça bilindik ve tanıdık bir senaryo.

![image.axd](images/image.axd)

Peki bu çatıyı sahada nasıl kullanabiliriz?

Öncelikle her iki taraf için ortak olan bir Proto dosyasına ihtiyacımız var (.Net Remoting zamanlarındaki Marshall By Value günlerimiz geldi aklıma) Bu dosya aslında bir servis sözleşmesi ve kullanılacak tiplere ait bilgileri içerecek. Buradaki sözleşme sunucu tarafında uygulanırken, istemci tarafından sadece uzak fonksiyon çağrısı yapabilmek amacıyla ele alınıyor. Gelin basit bir örnek ile konuyu anlamaya çalışalım. Kurgumuzda klasör yapısını aşağıdaki gibi oluşturabiliriz. Aynı makine üzerinden test yapacağız ancak rahatlıkla dağıtık senaryoları deneyebilirsiniz.

client (Folder)
index.js
product.proto
server (Folder)
index.js
Product.js
product.proto
package.json
nodemodules (Folder)

Bu arada node.js tarafında gRPC kullanımını kolaylaştırabilmek için grpc modülünün npm ile sisteme yüklenmesi lazım. İstemcinin tipine göre tabii farklı bir paket kullanmak gerekebilir. Örneğin.Net Core tarafında Grpc.Core isimli nuget paketini kullanmak gerekiyor. Gerekli modülü kurduktan sonra proto uzantılı servis sözleşmesini hazırlayarak devam edebiliriz.

```javascript
syntax = "proto3"; //Specify proto3 version.

package products; //benzersiz bir paket ismi

//Service. GRPC sunucusunun istemci tarafına sundugu servis sözlesmesi
service ProductService {
  rpc List (Empty) returns (ProductList);
  rpc Insert (Product) returns (Empty);
  rpc Get (ProductId) returns (Product);  
}

// Serviste kullanilan mesaj tipi
message Product {
  int32 id = 1;
  string name = 2;
  double listPrice = 3;
}

// Ornek bir liste
message ProductList {
  repeated Product Product = 1;
}

message ProductId {
  int32 id = 1;
}

message Empty {}
```

product.proto isimli dosya içerisinde ProductService isimli bir arayüz tanımlandığını görebilirsiniz. Bu arayüz ile üç operasyon sunuyoruz. Her biri Remote Procedure Call tipinden. Yani uzaktan tetiklenebilecek operasyon bildirimleri. List fonksiyonu bir parametre almıyor (bunu Empty isimli message tipi ile tanımladık) ve ProductList türünden sonuç döndürüyor. ProductList bir message tipi ve içinde tekrarlı sayıda Product mesajı içerebiliyor. Product mesajı id, name ve listPrice gibi özellikler içeren bir tip olarak ifade edilmekte. Kısaca bu dosya içeriği ile bir servisin operasyonları ile birlikte kullandığı tipleri tanımlayabiliyoruz. Web servislerine ait Service Description Language (WSDL) dokümanlarına benzetebiliriz. Senaryomuz gereği bu dosyanın hem sunucu hem de istemci tarafında olması lazım. Bu pek tabii servis içeriğinin güncellenmesi halinde istemci tarafının ne yapacağı sorusunu da akla getiriyor (Doğru bir cevap verebimek için gRPC ile ilgili vakaları incelemeye devam ediyorum)

Sunucu tarafında bir ürünü bulunduğu programlama ortamında ifade edebilmek için Product isimli entity tipimiz de var. Hani makaledeki örneği uyguladıktan sonra belki içerideki ürünleri MongoDB gibi bir ortama almak veya oradan çekmek isterseniz sizlere kolaylık sağlasın diye:)

```javascript
let product = class Product {
	constructor(id, name, listPrice) {
		this._id = id;
		this._name = name;
		this._listPrice = listPrice;
	}
	get ProductId() {
		return this._id;
	}
	get Name() {
		return this._name;
	}
	get ListPrice() {
		return this._listPrice;
	}
	set Id(value) {
		this._Id = value;
	}
	set Name(value) {
		this._name = value;
	}
	set ListPrice(value) {
		this._listPrice = value;
	}
}

module.exports = product;
```

Ürün numarası, ismi ve liste fiyatını tanımlayan bir node.js sınıfı söz konusu. Gelelim en baba kodlarımızdan birisine. Sunucu tarafındaki index.js'i aşağıdaki gibi geliştirebiliriz.

```javascript
const grpc = require('grpc');
const proto = grpc.load('./product.proto');
const server = new grpc.Server();
const product = require('./Product');

function allProducts() {
	console.log('[Server]:List all product');

	var products = [
		{ id: 1009, name: "lego mind storm", listPrice: 1499 },
		{ id: 1010, name: "star wars bardak altığı", listPrice: 35 },
		{ id: 1011, name: "ışıldak 40w", listPrice: 85.50 },
		{ id: 1012, name: "A4 X 100 adet", listPrice: 5 }
	];
	return products;
}
function singleProduct(productId) {
	console.log('[Server]:Get single product');
	console.log('[Server]:Incoming product id ' + productId);

	var product = {
		id: 1009,
		name: "lego mind storm",
		listPrice: 55
	};
	return product;
}
function addProduct(call) {
	console.log('[Server]:Insert new product');

	let p = new product(
		call.request.id,
		call.request.name,
		call.request.listPrice,
	);
	console.log(p);
}
function list(call, callback) {
	callback(null, allProducts());
}
function single(call, callback) {
	callback(null, singleProduct(call.request.id));
}

function insert(call, callback) {
	callback(null, addProduct(call));
}

server.addService(proto.products.ProductService.service, {
	List: list,
	Insert: insert,
	Get: single
});

server.bind('0.0.0.0:7500', grpc.ServerCredentials.createInsecure());
server.start();
console.log('grpc server is live', '0.0.0.0:7500');
```

Proto dosyasını ortama yüklemek için grpc modülünün load fonksiyonundan yararlanıyoruz. Kodun en önemli kısımlarından birisi server nesnesinin addService metoduna ait içerikte yer alıyor. İlk parametre ile grpc sunucusunun hangi servis sözleşmesini kullanacağını belirtiyoruz. İkinci parametrede yer alan eşleştirmelere dikkat etmemiz lazım. Sol taraf servis sözleşmesindeki metod adları ile aynı olmalı. Sağ taraftaki atamalarla, uzak çağrının yapıldığı servis operasyonu için, sunucu tarafında hangi metodun tetikleneceğini belirtiyoruz. Örneğin List operasyonuna yapılan çağrı list isimli metod tarafından ele alınıyor.

grpc sunucusunu 0.0.0.0:7500 portu üzerinden herhangi bir güvenlik protoklü uygulatmadan yayına alıyoruz. Bu işlem için bind ve start metodlarını kullanmaktayız. Kodun üst kısımlarında yer alan allProducts, singleProduct ve addProduct gibi metodlar basit işlemler gerçekleştirmekteler. Ancak dikkate değer kısımları var. İstemciye içerik döndüren operasyonlar hep JSON formatında veri üretiyorlar. Diğer yandan istemciden gelen payload içeriğini yakalamak için call.request değişkenini kullanıyoruz (addProduct metodunu inceleyin)

Sunucu tarafındaki işlerimiz şimdilik bu kadar. Artık istemci tarafını geliştirmeye başlayabilriz. Client klasöründe de product.proto dosyasının olması gerektiğini önceden belirtmiştik. Ayrıca node.js tabanlı geliştirilen istemcinin de grpc modülüne ihtiyacı olacak. Kod tarafını aşağıdaki gibi yazabiliriz.

```javascript
const grpc = require('grpc');
const proto = grpc.load('./product.proto');
const client = new proto.products.ProductService('localhost:7500', grpc.credentials.createInsecure());

client.List({}, (error, response) => {
	if (!error) {
		console.log("Response : ", response)
	}
	else {
		console.log("Error:", error.message);
	}
});

client.get({ id: 1001 }, (error, response) => {
	if (!error) {
		console.log("Response : ", response)
	}
	else {
		console.log("Error:", error.message);
	}
});

client.Insert({ id: 1001, name: "Scrum Post-It Kağıdı", listPrice: 5 }, (error, response) => {
	if (!error) {
		console.log("Response : ", response)
	}
	else {
		console.log("Error:", error.message);
	}
});
```

Burada öncelikle proto dosyasını yüklüyor ve sonrasında client isimli ProductService nesnesini örnekliyoruz. Sunucu tarafını aynı makinede 7500 nolu port üzerinden yayınladığımız için burada da aynı adres bilgisini kullanmamız gerekiyor. Sonrasında servis sözleşmesi ile sunulan operasyonları tek tek deniyoruz. client üzerinden List, get ve Insert isimli metodlara çağrı yapmaktayız. Basit olması açısından herhangi bir hata varsa bunu gösteriyor yoksa sunucudan döndürülen mesaj içeriğini ekrana bastırıyoruz.Tabii giden veya gelen mesajların JSON babında ele alınması gerektiğini bir kez daha hatırlatalım.

Yaptıklarımızı test etmek için önce sunucu tarafını, ardından istemci tarafını çalıştırmamız yeterli. Ben aşağıdaki ekran görüntüsünde yer alan sonuçları elde ettim.

![image.axd](images/image.axd)

Görüldüğü gibi istemci uygulama, uzak sunucu üzerinden sunulan servis operasyonlarını başarılı bir şekilde kullandı. Benim gRPC ile ilgili olarak ilk öğrendiklerim kısac bunlar. Aslında Microservice mimarisinde REST alternatifi bir iletişim tekniği olduğunu görmek benim için güzel oldu. Diğer yandan gRPC'nin yüksek performanslı, TCP soket haberleşmesi üzerinden binary serileştirme kullanan ve uygulanması kolay bir çatı olduğunu söyleyebiliriz. Performans konusunda bende Google'ın yalancısıyım dolayısıyla gerçek ölçümlemelere bakmakta yarar var. Bununla birlikte özellikle stream kullanımı örneğini de bir bakın derim ki [burada güzel bir anlatımı var](https://grpc.io/docs/tutorials/basic/node.html). gRPC'yi farklı dillerde uygulamak isterseniz google'ın [bu adresteki pratiklerine](https://grpc.io/docs/tutorials/) bakabilirsiniz. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Github'dan Örnek Kodları Alabilirsiniz](https://github.com/buraksenyurt/nodejs-tutorials/tree/master/Day10)
