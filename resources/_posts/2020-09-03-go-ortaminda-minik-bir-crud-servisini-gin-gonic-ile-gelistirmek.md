---
layout: post
title: "GO Ortamında Minik Bir CRUD Servisini Gin-Gonic ile Geliştirmek"
date: 2020-09-03 13:00:00 +0300
categories:
  - golang
tags:
  - golang
  - gin-gonic
  - rest-api
  - swagger
  - http
  - http-web-framework
---
Gin-Gonic hafif siklet sayılan ama yüksek performansı ile öne çıkan (Muadili olan martini'den çok daha hızlı olduğu ifade ediliyor. Bu arada farklı Http Web Framework'ler için [şu yazıyı](https://deepsource.io/blog/go-web-frameworks/) inceleyebilirsiniz) bir HTTP-Web framework. Elbette açık kaynak bir çatı. Middleware tarafında (Yani Request ve Response'lar arasında) Recover ve Log desteği sunuyor. Tabii kendi middleware bileşenimizi yazıp ekleyebiliriz de. Recovery modülü en başından beri ekli olduğundan paniklemeyen bir framework diyebiliriz:) Yani Go çalışma zamanında HTTP talepleri ile ilgili olarak bir panic oluştuğunda uygun bir 500 cevabı verebiliyor.

![gingonic.png](/assets/images/2020/gingonic.png)

Söylentilere göre bu özelliği sayesinde söz konusu servis her an ayakta ve çalışır durumda kalıyor (muş). Bahseliden bu yeteneklere ilaveten yönlendirmeleri (routes) gruplandırabiliyoruz ki bu da örneğin versiyonlamayı kolaylaştırıyor. Bu kısa notlar şimdilik yeterli. Sahada deneyimlemek lazım. İşte bu [SkyNet](https://github.com/buraksenyurt/skynet) derlemesindeki amacımız MongoDb üzerinde basit CRUD (Create Read Update Delete) işlemlerini yaparken gin-gonic üstüne kurulmuş golang tabanlı bir servis geliştirmek. Tahmin edeceğiniz üzere ben örneği Heimdall (Ubuntu-20.04) üstünde geliştiriyorum. MongoDb tarafı içinde bir Docker imajını kullanmayı tercih edeceğim. Dilerseniz vakit kaybetmeden idmanımıza başlayalım. İşte ilk terminal adımlarımız.

```bash
# Ana klasörümüz ve gerekli go dosyaları oluşturulur
mkdir book-worm-api
cd book-worm-api
touch main.go

# gin-gonic ve diğer modüllerin yönetimi için
# mod uzantılı bir dosya oluşacaktır. Burada yüklediğimiz paket bilgilerini görebiliriz. 
# Genel isimlendirme standardı olarak github.com/buraksenyurt/book-worm-api kullanımı da tercih edilebilir
go mod init book-worm-api

# gin-gonic ve mongodb için gerekli go paketlerinin yüklenmesi
go get -u github.com/gin-gonic/gin go.mongodb.org/mongo-driver

# MongoDB tarafıyla eşlecek entity için
touch quote.go

# CRUD Operasyonları için
touch quotedata.go

# Servis metotlarındaki annotation bildirimlerinin Swagger 2.0 destekli olarak dokümante edilmesi için gerekli modülün eklenmesi
go get -u github.com/swaggo/swag/cmd/swag

# Bu arada kod tarafındaki annotation bölümleri tamamlandıktan sonra Swagger dokümanının üretilmesi için aşağıdaki komutu çalıştırmalıyız
swag init _ "book-worm-api/docs"

# DOCKER TARAFI

# mongodb docker container'ının çalıştırılması ve veritabanının oluşturulması
# bookworms isimli bir veritabanı oluşturuyoruz ve root user ile password bilgisi de veriyoruz (Bunu production'da yapmayın tabii)
sudo docker run --name mongodb -e MONGO_INITDB_ROOT_USERNAME=scoth -e MONGO_INITDB_ROOT_PASSWORD=tiger -e MONGO_INITDB_DATABASE=bookworms -p 27017:27017 -d mongo:latest
```

Gelelim kod tarafında yaptıklarımıza. Entity tipimiz olan quote yapısını (struct) aşağıdaki şekilde geliştirebiliriz.

```cpp
package main

import "go.mongodb.org/mongo-driver/bson/primitive"

/*
	quotation yapısı mongodb'deki dokümanın GO tarafındaki izdüşümü.
	ID, mongodb tarafı için bson'un primitive tiplerinden birisi ile ifade edilebilir
*/
type quote struct {
	ID          primitive.ObjectID
	Description string
	Writer      string
	Book        string
}
```

CRUD operasyonlarını ihtiva eden ve MongoDB tarafı ile haberleşen fonksiyonları içeren quoteData.go dosyasının içeriğini de aşağıdaki şekilde hazırlayabiliriz.

```cpp
package main

/*
	Burada MongoDB ile ilgili CRUD Operasyonları yer alıyor.
*/

import (
	"context"
	"log"

	"go.mongodb.org/mongo-driver/bson/primitive"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

// getConn fonksiyonu ile mongodb bağlantı nesnesini ve context'i döndürüyoruz
func getConn() (*mongo.Client, context.Context) {

	/*
		NewClient nesnesini docker ile ayağa kalkan mongodb adresine göre oluşturuyoruz
		bir error olması durumu da aşağıya doğru sürekli kontrol edilmekte
	*/
	client, err := mongo.NewClient(options.Client().ApplyURI("mongodb://scoth:tiger@localhost:27017")) // ilgili bağlantı bilgisini kullanarak yeni bir client nesnesi oluşturuldu
	ctx := context.Background()

	err = client.Connect(ctx)
	if err != nil {
		log.Printf("Bağlanırken hata alındı: %v", err)
	}

	// Bağlantıyı kullanarak mongodb'yi ping'liyoruz
	err = client.Ping(ctx, nil)
	if err != nil {
		log.Printf("Ping yapılamadı: %v", err)
	}

	return client, ctx // geriye Context nesnesini de dönüyoruz. İzleyen metotlardaki defer kullanımlarına dikkat!
}

// AddQuote tahmin edileceği üzere bookworm veritabanındaki quotes koleksiyonuna yeni bir quote eklemek için kullanılıyor
func AddQuote(q *quote) (primitive.ObjectID, error) {
	log.Println("Add Quote")
	log.Println(q)

	client, ctx := getConn()
	defer client.Disconnect(ctx)   // AddQuote işleyişinin sonunda mongodb bağlantısının kapatılmasını garanti ediyoruz
	q.ID = primitive.NewObjectID() // eklenecej doküman için yeni bir Object ID üretiliyor ve parametre olarak gelen quote değişkenine yapıştırılıyor.

	// InsertOne ile q ile gelen quote değişkenini yolluyoruz. Eğer bir sorun olursa err parametresi hata bilgisini taşıyacaktır
	result, err := client.Database("bookworms").Collection("quotes").InsertOne(ctx, q)
	if err != nil { // Eğer hata olmuşsa bunu metottan geriye nil object ID ile birlikte dönüyoruz. Hatayı gin-gonic metotları değerlendirip uygun HTTP mesajını döndürecektir.
		log.Printf("Alıntı eklenmeye çalışırken hata oluştu %v", err)
		return primitive.NilObjectID, err
	}
	id := result.InsertedID.(primitive.ObjectID)
	return id, nil // Eğer sorun yoksa eklenen Object ID bilgisini dönüyor. Bu noktada hata olmadığı için ikinci output değişkeni nil olarak atanıyor
}

// GetQuotes metodu ile bookworm veritabanındaki quotes koleksiyonunda yer alan tüm dokümanları çekiyoruz
// Basit bir veri çekme metodu da olsa her ihtimale karşı hata kontrolümüz de var
func GetQuotes() ([]*quote, error) {
	var quotes []*quote // Döndüreceğimiz array

	client, ctx := getConn()     // MongoDb bağlantı bilgilerini aldık
	defer client.Disconnect(ctx) // Panik olsa da olmasa da metot tamalanırken Disconnect olalım

	db := client.Database("bookworms")            //veritabanı nesnesi
	collection := db.Collection("quotes")         // koleksiyon nesnesi
	cursor, err := collection.Find(ctx, bson.D{}) // quotes koleksiyonundaki tüm dokümanları çekmek için kullanılan fonksiyon
	if err != nil {                               // Find metodunun da error dönüşü var o yüzden kontrol etmekte fayda var
		return nil, err
	}
	defer cursor.Close(ctx)        // Eğer hata almadan geldiysek sonraki hata durumuna karşın Find ile açılan cursor'u kapattırır
	err = cursor.All(ctx, "es) // Koleksiyonu quotes'a alıyoruz
	if err != nil {                // All metodunun da error dönüşü var. Kontrol etmek iyi fikir
		log.Println(err)
		return nil, err
	}
	return quotes, nil // Her şey yolunda gittiyse ;)
}
```

Gelelim tüm bunları kullanacağımız ana yüklenici main modülünün içeriğine. Swagger için annotation'lar kullandığımız dikkatinizden kaçmasın.

```cpp
package main

// gin-gonic modülünü ekledik
// Ayrıca Swagger desteği için gin-swagger modülü de eklendi
import (
	"log"
	"net/http"

	ginSwagger "github.com/swaggo/gin-swagger"
	"github.com/swaggo/gin-swagger/swaggerFiles"

	_ "book-worm-api/docs" //swag init ile eklenen dokümanın bildirimi
	// Bunu eklememin sebebi çalışma zamanında swagger UI'a gidince aldığım Failed to load spec. hatası

	"github.com/gin-gonic/gin"
)

// @title BookWorm Swagger API
// @version 1.0
// @description Servis kullanım rehberidir
// @termsOfService http://swagger.io/terms/

// @contact.name Burak Selim Şenyurt
// @contact.email selim@buraksenyurt.com
// @contact.url https://www.buraksenyurt.com

// @BasePath /api/v1

// @host localhost:5003
func main() {
	router := gin.Default()      // gin nesnesini örnekledik
	api := router.Group("/api")  //ve api'nin
	v1 := api.Group("/v1")       // v1 sürümü için
	quotes := v1.Group("/quote") //quotes isimli bir route tanımladık

	// Bu route için kök adrese HTTP Get talebi gelirse tüm alıntıları listeyecek operasyonu çalıştırıyoruz
	quotes.GET("/", GetAll)

	// Kök adrese bir Post talebi gelirse bu sefer yeni bir alıntının eklenmesini sağlıyoruz
	quotes.POST("/", Create)

	/*
		Swagger API dokümantasyon desteği için ekledik.
		metot başlarında yer alan yorum satırları da Swagger UI tarafında değerlenecek
	*/
	router.GET("/swagger/*any", ginSwagger.WrapHandler(swaggerFiles.Handler))

	_ = router.Run(":5003") // sunucuyu 5003 numaralı porttan hizmete açtık
}

/*
	@Summary operasyon için açıklama kısmıdır.
	@Produce ve @Accept ile HTTP operasyonun hangi tür içerikle çalıştığını söylüyoruz ki örnekte json.
	@Param kısmında Body içinden quote tipinden bir değişken beklediğimizi ve zorunlu olduğunu ifade ederiz
	@Sucess ile operasyon başarılı ise HTTP 200 dönüldüğünü ifade ederiz
	@Failure kısımlarında metottan hangi tür HTTP hatalarının dönebileceğini ifade ediyoruz
	@Router kısmında operasyon adresini HTTP POST metodu ile tetiklendiğini ifade ediyoruz
*/

// Create godoc
// @Summary Yeni bir kitap alıntısı ekler
// @Produce json
// @Accept json
// @Param quote body quote true "Alıntı Bilgileri"
// @Success 200
// @Failure 400
// @Failure 500
// @Router /quote/ [post]
func Create(c *gin.Context) {
	var q quote
	// Gelen JSON içeriğinin quote yapısına eşlenip eşlenemediği kontrol ediliyor
	if err := c.ShouldBindJSON(&q); err != nil {
		log.Print(err)
		c.JSON(http.StatusBadRequest, gin.H{"msg": err}) // Eğer JSON içeriğinde sıkıntı varsa hata mesajı ile birlikte geriye HTTP 400 Bad Request dönüyoruz
		return
	}
	id, err := AddQuote(&q) // Add metodu ile eklemeyi gerçekleştiriyoruz
	if err != nil {         // eğer hata varsa bunu da değerlendirip geriye uygun bir HTTP durum kodu ile döndürüyoruz
		c.JSON(http.StatusInternalServerError, gin.H{"msg": err})
		return
	}
	q.ID = id
	c.JSON(http.StatusOK, gin.H{"added": q}) // Her şey yolundaysa HTTP 200 Ok
}

/*
	{array} ile bir quote listesinin döneceğini belirtiyoruz
*/

// GetAll godoc
// @Summary Tüm kitap alıntılarını döndürür
// @Produce json
// @Success 200 {array} quote
// @Failure 500
// @Router /quote/ [get]
func GetAll(c *gin.Context) {
	// mongodb ile iletişim kuran quotedata içerisindeki ilgili metodu çağırdık
	var quoteList, err = GetQuotes()
	// Her ihtimale karşı listeyi çeken metot bir hata döndürmüş mü bakalım
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"msg": err})
		return
	}
	c.JSON(http.StatusOK, gin.H{"quotes": quoteList}) // HTTP 200 OK ile birlikte çekilen listeyi geri yolluyoruz.
}
```

Bu bir servis uygulaması olduğu için çalıştırdıktan sonra bir şekilde tüketmemiz de gerekiyor ki sonuçları görebilelim. curl, postman gibi araçlar bu amaçla kullanılabilir veya bir client uygulama yazılabilir (yazabilirsiniz) Şimdi aşağıdaki terminal komutları ile önce uygulamamızı build edelim ve ardından servisimizi çalıştıralım. Tabii bu işler sırasında MongoDb docker container'ının çalıştığından emin olalım.

```bash
# önce bir build etmek lazım
go build

# sonrasında çalıştırabiliriz
./book-worm-api
```

İlk testler için aşağıdaki curl komutlarını kullanabiliriz.

```bash
# Örnek birkaç alıntı girelim

curl --location --request POST 'http://localhost:5003/api/v1/quote/' \
--header 'Content-Type: application/json' \
--data-raw '{
"Description": "Bizler bugüne kadar inşa edilmiş en iyi zaman makineleriyiz.",
"Writer": "Dean Buonomono",
"Book": "Beyniniz: Bir Zaman Makinesi"
}'

curl --location --request POST 'http://localhost:5003/api/v1/quote/' \
--header 'Content-Type: application/json' \
--data-raw '{
"Description": "Böylece, günler geçiyor.",
"Writer": "Albert Camus",
"Book": "Tersi ve Yüzü"
}'

curl --location --request POST 'http://localhost:5003/api/v1/quote/' \
--header 'Content-Type: application/json' \
--data-raw '{
"Description": "Herkes hayatının bir devresinde şu veya bu şekilde talihinin şuuruna erer.",
"Writer": "Ahmet Hamdi Tanpınar",
"Book": "Saatleri Ayarlama Enstitüsü"
}'

# Şimdi de listeleme yapalım
curl http://localhost:5003/api/v1/quote/
```

İşte çalışma zamanından birkaç görüntü.

![skynet_25_Screenshot_1.png](/assets/images/2020/skynet_25_Screenshot_1.png)

![skynet_25_Screenshot_2.png](/assets/images/2020/skynet_25_Screenshot_2.png)

Uygulamamıza swagger entegrasyonu yaptığımızdan detaylı bir dokümantasyona da sahibiz. Bu servis ne yapıyor, hangi operasyonları nasıl test ederim, test için bana vereceği curl çıktıları neler vb şeklindeki soruların karşılığı olan dokümantasyonu hatırlayacağınız üzere aşağıdaki terminal komutu ile üretmiştik. Tekrardan hatırlatmakta yarar var.

```bash
# İlgili annotation bölümleri tamamlandıktan sonra dokümanın üretilmesi için aşağıdaki komutu çalıştırmalıyız.
swag init _ "book-worm-api/docs"
```

Bu işlem docs klasörünün oluşmasını sağlar. Klasördeki swagger.json ve yaml içeriklerini bir inceleyin derim;) Swagger implementasyonu sonrası çalışma zamanına ait birkaç ekran görüntüsünü de aşağıda bulabilirsiniz. İşte localhost:5003/swagger/index.html den erişebileceğimi ana sayfadan bir görüntü. Gayet şık değil mi?:)

![skynet_25_Screenshot_3.png](/assets/images/2020/skynet_25_Screenshot_3.png)

Yeni bir kitap alıntısı eklerken ki görüntü.

![skynet_25_Screenshot_4.png](/assets/images/2020/skynet_25_Screenshot_4.png)

Post işleminin sonucuna ait bir görüntü.

![skynet_25_Screenshot_5.png](/assets/images/2020/skynet_25_Screenshot_5.png)

ve son olarak tüm kitap alıntılarını çektiğimiz HTTP Get talebine ait görüntü.

![skynet_25_Screenshot_6.png](/assets/images/2020/skynet_25_Screenshot_6.png)

Örnek servisimiz MongoDb'yi gin-gonic çatısı üstünden kullanan bir Go uyglaması. Swagger desteği de bulunmakta. Eğer Update ve Delete operasyonlarını da işin içerisine katarsak, çalıştığımız kurumlar için hafif/orta siklet sayılabilecek REST Api'leri kolaylıkla geliştirebiliriz. Böylece geldik bir SkyNet derlemesinin daha sonuna. Örnek uygulama kodlarına [github reposu üzerinden](https://github.com/buraksenyurt/skynet/tree/master/No%2025%20-%20Hello%20Gin-Gonic) erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
