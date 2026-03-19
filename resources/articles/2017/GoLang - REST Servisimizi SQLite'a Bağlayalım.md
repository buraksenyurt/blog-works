---
title: "GoLang - REST Servisimizi SQLite'a Bağlayalım"
pubDate: 2017-07-10 09:05:00
categories:
  - GoLang
tags:
  - GoLang
  - .NET
  - Oracle
  - MySQL
  - REST
  - JSON
  - HTTP
  - Go
  - Pointers
  - GitHub
---

# GoLang - REST Servisimizi SQLite'a Bağlayalım
![gosqlite_7.gif](images/gosqlite_7.gif)

Merhaba Arkadaşlar,

Son yazılarımızda GoLang ile web uygulamalarının geliştirilmesi üzerinde durduk. Yeni şeyler öğrendikçe bunları farklı örnekler üzerinden denemeye gayret ediyorum. Bu sefer HTTP yönlendiricimizi SQLite ile çalışan basit bir REST servisi için kullanmaya çalışacağız. Kodlara geçmeden önce sisteminize SQLite yüklemiş olduğunuzdan emin olun (Kendi sisteminiz için uygun sürümü SQLite'ın [şuradaki resmi adresinden](https://sqlite.org/download.html) bulup indirebilirsiniz) SQLite yazımızın kapsamı dışında ama bizim için hafif bir veri saklamak fonksiyonelliğini sunacağını ifade edebiliriz. Bu tipteki veritabanları fiziki birer dosya olarak tutulmaktalar. Bu nedenle geliştireceğimiz Go örneğinin erişebileceği bir konumda ilgili veritabanı dosyasının bulunması yeterli.

Ben örnek veri kümesi için bir önceki yazıda kullandığım Star Wars çözümünü baz aldım. Yani Category ve buna bağlı Model isimli birer tablo söz konusu. Tablolar arasında bire-çok ilişkili CategoryId alanı üzerinden sağlayabiliriz. Böylece bir kategori altındaki tüm modelleri ele alacağımız basit bir senaryo üzerinde de durabiliriz. Tabii öncelikle komut satırından aşağıdaki betikler yardımıyla starwars.sdb isimli veritabanını oluşturmamız gerekiyor (Aslında istediğiniz bir uzantıyı kullanabilirsiniz nitekim SQLite dosyaları ikili-binary formatta tutulan içeriklere sahiptirler. Yani uzantısının ne olduğunu bir önemi yok) Sonrasında Category ve Model isimli iki tablo ekleyip örnek veriler ile doldurursak da güzel olabilir.

Veritabanı ve Tabloların Hazırlanması

```text
sqlite3 starwars.sdb

.databases

.open starwars.sdb

create table Category(
Id number primary key,
Name varchar(30)
);
insert into Category (Id,Name) values (1,"fighter");
insert into Category (Id,Name) values (2,"cruiser");
select * from Category;

create table Model(
Id number primary key,
Title varchar(50),
ListPrice real,
CategoryId number
);

insert into Model values (1,"V-Wing Fighter",45.50,1);
insert into Model values (2,"Naboo N-1 Starfighter",250.45,1);
insert into Model values (3,"Republic Cruiser",450.00,2);
insert into Model values (4,"Republic Attack Cruiser",950.00,2);
insert into Model values (5,"ETA-2 Jedi Starfighter",650.50,1);
insert into Model values (6,"Delta-7 Jedi Starfighter",250.35,1);
insert into Model values (7,"B-Wing",195.50,1);
insert into Model values (8,"Y-Wing",45.50,1);
insert into Model values (9,"Mon Calamari Star Cruiser",1500.50,1);
select * from Model where CategoryId=1;
```

Eğer SQLite kurulumunuzda bir sorun yoksa yukarıdaki komutların hatasız çalışması gerekir. Aynen aşağıdaki ekran görüntüsündekine benzer olacak şekilde.

![gosqlite_1.gif](images/gosqlite_1.gif)

Servis Tarafı

Önceki yazılarımızda olduğu gibi yönlendirme işlemlerimiz için Julien Schmidt'in (bu soyadını tek seferde asla yazamadım) httpRouter paketinden yararlanacağız. Diğer yandan SQLite veritabanını kullanacağımız için yardımcı bir kütüphaneyi daha işin içine katacağız. github.com/mattn/go-sqlite3 adresinde yer alan paket SQLite üzerinde gerçekleştireceğimiz işlemlerde bize kolaylıklar sağlayacak (Paketin yazarı Japon'ya Osaka'dan. Henüz ingilizceye çeviremediğim ama oldukça merak ettiğim [blog adresi de burada](http://mattn.kaoriya.net/)) Aynen httpRouter paketinin elde edilişinde olduğu gibi LiteIDE'nin Build->Get komutunu kullanarak ilgili kütüphanenin sisteme yüklenmesini sağlayabilirsiniz. (Ben yükleme işlemi sırasında 64Bit Windows'umdaki farklı MinGW ve GCC sürümleri nedeniyle hatalarla karşılatım ve güncel versiyonunu yükleyerek sorunu aştım. [Şu adrese](http://tdm-gcc.tdragon.net/download) uğramanız gerekebilir) Şimdi Server.go isimli dosyamızın içeriğini aşağıdaki gibi oluşturalım.

```cpp
package main

import (
	"database/sql"
	"encoding/json"
	"entity/starwars"
	"fmt"
	"log"
	"net/http"
	"strconv"

	"github.com/julienschmidt/httprouter"
	_ "github.com/mattn/go-sqlite3"
)

func main() {
	router := httprouter.New()

	router.GET("/", home)
	router.GET("/categories", getCategories)
	router.GET("/categories/:categoryId", getModelsByCategoryId)
	router.GET("/models/:firstLetter", getModelsByFirstLetter)
	router.POST("/newCategory", createCategory)

	http.ListenAndServe(":4571", router)
}

func createCategory(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
	category := starwars.Category{}
	json.NewDecoder(r.Body).Decode(&category)
	log.Printf("Insert request. %d,%s\n", category.Id, category.Name)
	conn, _ := sql.Open("sqlite3", "starwars.sdb")
	defer conn.Close()
	_, err := conn.Exec("Insert into Category values (?,?)", category.Id, category.Name)
	if err == nil {
		render(w, category)
	} else {
		log.Println(err.Error())
		//404 basılabilir
	}
}

func getCategories(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
	conn, _ := sql.Open("sqlite3", "starwars.sdb")
	defer conn.Close()
	rows, _ := conn.Query("Select Id,Name from Category order by Name")
	defer rows.Close()
	categories := make([]*starwars.Category, 0)
	for rows.Next() {
		category := new(starwars.Category)
		rows.Scan(&category.Id, &category.Name)
		categories = append(categories, category)
	}
	render(w, categories)
}

func getModelsByCategoryId(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	conn, _ := sql.Open("sqlite3", "starwars.sdb")
	defer conn.Close()
	id, _ := strconv.Atoi(params.ByName("categoryId"))
	cRow := conn.QueryRow("Select * from Category Where Id=?", id)
	ctgry := new(starwars.Category)
	if cRow != nil {
		cRow.Scan(&ctgry.Id, &ctgry.Name)
		rows, _ := conn.Query("Select Id,Title,ListPrice from Model where CategoryId=?", id)
		defer rows.Close()
		models := make([]*starwars.Model, 0)
		for rows.Next() {
			model := new(starwars.Model)
			model.Category = starwars.Category{Id: ctgry.Id, Name: ctgry.Name}
			rows.Scan(&model.Id, &model.Title, &model.Price)
			models = append(models, model)
		}
		render(w, models)
	}
}

func getModelsByFirstLetter(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	conn, _ := sql.Open("sqlite3", "starwars.sdb")
	defer conn.Close()
	statement := fmt.Sprintf("Select Id,Title,ListPrice from Model where Title like '%s%%'", params.ByName("firstLetter"))
	rows, _ := conn.Query(statement)
	defer rows.Close()
	models := make([]*starwars.Model, 0)
	for rows.Next() {
		model := new(starwars.Model)
		rows.Scan(&model.Id, &model.Title, &model.Price)
		models = append(models, model)
	}
	render(w, models)
}

func home(rWriter http.ResponseWriter, request *http.Request, _ httprouter.Params) {
	fmt.Fprintf(rWriter, "Star Wars universe!")
}

func render(w http.ResponseWriter, d data) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(200)
	jContent, _ := json.Marshal(d)
	fmt.Fprintf(w, "%s", jContent)
}

type data interface {
}
```

Bu kez kodlarımız biraz karmaşık gibi (Kendime not: Kod tekrarlarını azalt) Uygulamamız temel olarak aşağıdaki taleplere cevap verecek şekilde geliştirildi.

HTTP Get; /; root taleb
HTTP Get; /categories; kategorileri getirecek
HTTP Get; /categories/:categoryId;belli bir kateogori numarasındaki modelleri listeleyecek (models/:categoryId de olabilirdi belki)
HTTP Get; /models/:firstLetter; modellerin baş harfine göre listelenmesini sağlayacak
HTTP Post; /newCategory; yeni bir kategorinin sisteme eklenmesi için kullanılacak

Aslında önceki yazılarımızdan farklı olarak bir tane HTTP Post metodumuz bulunduğunu ifade edebiliriz. /newCategory talebimiz ile yeni bir kategoriyi Category tablosuna eklemeyi planlıyoruz. main fonksiyonumuz yönlendirici nesnenin yukarıdaki adresler için eşleştireceği metod bildirimleri ile başlıyor. Sunucumuz localhost adresinden ve 4571 nolu port üzerinden hizmet verecek şekilde ayarlanıyor.

## Yeni Bir Kategori Eklemek

İlk olarak createCategory fonksiyonunu ele alalım. SQLite veritabanımızdaki Category tablosuna yeni bir kategori eklemek istiyoruz. İstemci taleplerini HTTP Post metodu ile ve JSON formatında olacak şekilde kabul edeceğiz. Category yapısı önceki yazılarımızdan da hatırlayacağınız gibi sistemde ayrı bir paket içerisinde duruyor (entity/starwars) İstemciden gönderilecek JSON içeriğinde Id ve Name alanlarının olması yeterli. NewDecoder ile üretilen çözümleyici, Request nesnesinden gelen Body içeriğini ayrıştırıyor ve sonuçları category değişkenine aktarıyor. & ile bir adres aldığımıza dikkat edelim (Pointer'ları hatırlayalım). Decode fonskiyonunun parametre yapısına baktığınızda interface kabul ettiğini göreceksiniz. Bu teoriyi kodun ilerleyen kısımlarında biz de değerlendireceğiz.

Gelen bilgileri logladıktan sonra SQLite operasyonumuza başlıyoruz. Open fonksiyonu ile sqlite3 veritabanı sürücüsünü kullanarak, sunucu ile aynı adreste yer alan starwars.sdb isimli veritabanını belleğe açıyoruz. Exec fonksiyonu basit bir Insert sorgusu içeriyor. Bu sorgunun parametrik olduğuna ve parametre bildirimleri için soru işareti kullanıldığına dikkat edelim. Eğer Exec fonksiyonunu bir hata döndürmediyse insert işleminin başarılı olduğunu düşünerek gelen JSON içeriğine göre oluşturulan category değişkenini bu kez render fonksiyonu üzerinden istemciye basıyoruz. Bu render işlemlerini diğer fonksiyonlarda da kullanacağımız için kod tekrarını biraz olsun önlemek amacıyla geliştirdik. Yanlız fonksiyonun ikinci parametresine bilhassa dikkat edelim. data isimli bir interface tipi almakta. Aslında bu sayede render fonksiyonuna JSON olarak serileşebilecek herhangibir tipi aktarabiliriz. Bu tip tek bir Category örneği olabileceği gibi Model örnekleri içeren bir slice'da olabilir. Fonksiyon Header bilgisini JSON formatında işaretleyip HTTP 200 kodunu da ekleyerek bir çıktı oluşturuyor. Bu çıktı ilk parametre ile gelen ResponseWriter üzerinden istemciye gönderiliyor. Çalışma zamanında [Postman](https://www.getpostman.com/) veya muadili bir uygulamayı kullanarak yeni bir kategori oluşturmayı deneyebiliriz. Aşağıdaki örnek bir POST çağrısı görüyorsunuz.

![gosqlite_3.gif](images/gosqlite_3.gif)

Buna göre kategorileri getiren talebi yaptığımızda aşağıdaki gibi Destroyer sınıfının da eklendiğini görebiliriz.

![gosqlite_4.gif](images/gosqlite_4.gif)

## Kategori Listesi Nasıl Geliyor?

Tüm kategorilerin istendiği talebe karşılık gelen fonksiyonumuz getCategories. /categories şeklinde gelecek bir istemci talebi sonrası devreye giriyor. Fonksiyon yine SQLite veritabanını açarak işe başlıyor. Bu sefer Category tablosunun içeriğini Name alanına göre alfabetik sırada talep ediyoruz. Select sorgusu için Query fonksiyonu çalıştırılıyor. Fonksiyonun geriye döndürüğü sonuç kümesi üzerinde bir for döngüsü ile hareket ediyoruz. Next fonksiyonu satırlarda ileri doğru hareket etmemizi sağlıyor. Döngü öncesinde starwars.Category tipinden oluşturulan bir slice örneği mevcut. Tüm kategoriler bu listeye ekleniyorlar. Ekleme işlemi sırasında dikkat edilmesi gereken nokta ise Scan isimli fonksiyonun kullanılması. & ile adresleri üzerinden yakaladığımız Id ve Name alanlarını sorgu sonucu gelen kolon değerleri ile eşleştiriyoruz. append fonksiyonu da slice içeriğine ilgili satırı eklememizi sağlıyor. defer çağrıları ile fonksiyon sonlanırken gerekli kapatma işlemlerinin yapılmasını bildiriyoruz.

## Belli Bir Kategorideki Ürünlerin Çekilmesi

4571/categories/1 şeklinde gelecek bir talebe karşılık CategoryID alanının değeri 1 olan modelleri listelemek niyetindeyiz. Tüm kategorileri getirmekten farklı olarak params ile yakaladığımız categoryId değerinin SQL'e ait where ifadesinde parametrik kullanılması söz konusu diyebiliriz. Küçük bir de problemimiz var. Nesne modeli ilişkisinde modelleri kategorileri ile bağlarken tip kullandık. Yani bir Model aslında CategoryId değil Category nesne örneğini içeriyor. Veritabanında ki modelimizde ise bir modeli kategori numarası üzerinden ilişkilendirdik. Bu sebepten fonksiyon öncelikle ilgili kategori numarasına bağlı Category satırını buluyor. Eğer böyle bir kategori varsa nesne olarak örnekleyip bulunan modeller ile ilişkilendiriyor. Sonrasında üretilen models içeriğinin render fonksiyonuna gönderilerek JSON formatında istemciye gönderilmesi söz konusu. Aşağıda çalışma zamanına ait örnek bir görüntü yer alıyor. Aslında çekilen Model nesne topluluğu için Category nesnelerini doldurmak zorunda değiliz. İşin aslı bize güzel bir ORM (Object Relational Mapping) sistemi lazım. İlerleyen yazılarımızda bu konuyu da ele almaya çalışacağım.

![gosqlite_5.gif](images/gosqlite_5.gif)

## Baş Harfi "A" Olan Modelleri Bulalım

getModelsByFirstLetter fonksiyonunun görevi bu. Baş harfine göre modellerin listesini JSON formatında döndürmek için çalışıyor. Aslında model depomuz oldukça fakir diyebilirim. Aynı harf ile başlayan modeller yok gibi. Ancak siz model üretmek için yazacağınız POST temelli yeni operasyonunuz ile bu test içeriklerini kolaylıkla üretebilirsiniz. Hatta belki bir.Net arabiriminden bu servisi çağırarak modelleri oluşturmayı daha kolay hale de getirebilirsiniz..Net olmak zorunda değil, basit bir HTML sayfası bile olabilir. Sanırım size çaktırmadan bir görev verdim:) Fonksiyonumuza geri dönelim. Buradaki SQL sorgusu içerisinde Like kullanımı söz konusu. Nitekim baş harfi 'şununla'başlayanları çek gibi bir şey demek istiyoruz. Bu nedenle gelen parametreyi 'A%' gibi bir formata dönüştürmek gerekiyor. Bunun için fmt paketinin Sprintf fonksiyonundan yararlanıyoruz. Kodun kalan kısmı öncekilere benziyor. Next fonksiyonu ile dolaştığımız veri içeriğini ekrana basıyoruz. Tabii kategori ile ilgili sorunumuz var. İçeriği varsayılan değerleri ile geliyor ki bu son derece normal. Çünkü CategoryId ye karşılık gelen Category içeriğini bulup yüklemedik. Ah Burak ah:[] Demek ki bir Id'ye bağlı kategoriyi bulup geriye Category örneği olarak döndürecek bir fonksiyonellik burada işimize yarayabilir. Nitekim iki yerde ihtiyacımız oldu. Bu eklemeyi benim için yaparsınız değil mi?

![gosqlite_6.gif](images/gosqlite_6.gif)

Sonuç

Bu kısa araştırma yazımızın amacı REST (Representational State Transfer) tabanlı bir GO servisinde SQLite için gerekli bağlantıları nasıl tesis edebileceğimizi ilkel bir örnek üzerinden görmekti. SQLite dışında elbette farklı veri depolama ürünlerini de kullanmak isteyebiliriz. GitHub üzerinden yayınlanan [şu adreste](https://github.com/golang/go/wiki/SQLDrivers) oldukça geniş bir kütüphane topluluğu bulunuyor. Firebird'ten DB2'ya, MySQL'den Oracle'a, YQL (Yahoo Query Language)'ten, SQLite'a kadar geniş bir yelpaze söz konusu diyebilirim. Örneğimizde HTTP Get taleplerinden farklı olarak HTTP Post metoduna da yer verdik. Pek tabii Push, Delete gibi operasyonlar da söz konusu. Örneğin bu kodun üstüne bir kategori veya ürünü silmek için gerekli HTTP Delete operasyonunu ilave edebilirsiniz. Başlangıç benden devam ettirmesi sizden. REST testleri yapmamız da oldukça kolay. Chrome tarayıcısına eklenti olarak da gelen Postman aracı ekran görüntülerinde de gördüğünüz üzere oldukça pratik ve basit. Post gibi HTTP gövdesinden bir şeyler göndermemiz gereken senaryoları ele almak için ideal. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
