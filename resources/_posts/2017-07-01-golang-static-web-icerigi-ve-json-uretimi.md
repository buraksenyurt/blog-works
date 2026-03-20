---
layout: post
title: "GoLang - Static Web İçeriği ve JSON Üretimi"
date: 2017-07-01 10:45:00 +0300
categories:
  - golang
tags:
  - golang
  - rest
  - json
  - http
  - go
  - caching
  - github
---
Bir önceki yazımızda web programlamada önemli bir yere sahip olan yönlendiricileri (Router) tanımak adına github üzerinden sunulan httpRouter paketini kullanarak dinamik HTML içeriği ürettiğimiz bir örnek geliştirmiştik. Bu hafta içinde HttpRouter paketi hakkında öğrendiklerimi çeşitli örnekler ile pekiştirmeye çalıştım. Bu sefer hem static web içeriğinin hem de talebe göre JSON formatlı veri sunumu yapacak dinamiklerin yer alacağı konu üzerinde durdum. Web programlama konusunda acemi olduğum için örneği sonuca ulaştırırken takıldığım bir kaç nokta da oldu. Yazımızda bu konulara da değinmeye çalışacağım.

![gorouting_6.gif](/assets/images/2017/gorouting_6.gif)

Bu tip örneklerde işe yarar bir veri kümesi ile çalışmak her zaman için tercih ettiğim yöntemlerden birisi. Hatırlıyorum da Microsoft'un bu alanda oldukça başarılı veri kümeleri bulunuyordu. AdventureWorks ve Northwind veritabanları bize yıllarca yardımcı oldu. Eminim ki çoğumuzun kobay taboları vardır. Product ve Category gibi. Ben bu örneğimizde kendi hafif veri setimi bellekte konuşlandırdım. Pratik geldi:) Başrol oyuncusu olarak Star Wars dünyasından bir kaç modeli kullanmaya çalıştım. Kategori bazlı olacak şekilde bu evrendeki araçları basit nitelikleri ile ele aldım. Bu kez [şu adresten](http://www.blastr.com/2015-9-11/v-wing-millennium-falcon-50-best-star-wars-vehicles-ranked) yararlandım.

Entity İçeriğini Paket Olarak Tutmak

Önceki örnekten farklı olarak bu kez kullanacağımız entity tiplerini ayrı bir paket içerisinde toplamaya karar verdim. Sistemimde yüklü olan GOPATH bilgisine göre c:\go works\samples\src klasörü altında aşağıdaki hiyerarşiyi kurguladım.

- entity
- entity\starwars
- entity\starwars\starwars.go

Bundan sonra sistemde kullanacağım başka entity tipleri olursa burada alt klasörler içerisinde toplamayı düşünüyorum. Bir paketi oluşturduğumuzda bunun GOPATH'in tanımladığı lokasyonlarda inşa edilmesi oldukça önemli. Aksi takdirde ilgili paket sistemde bulunamaz ve dolayısıyla kullanılamaz. starwars.go içeriğini aşağıdaki gibi oluşturabiliriz. Model ve Category isimli iki yapı barındırıyor.

```cpp
package starwars

type Model struct {
	Id       int
	Title    string
	Price    float32
	Category Category
}

type Category struct {
	Id   int
	Name string
}
```

Paketi LiteIDE ile oluşturup build edebiliriz. Sonrasında pkg klasöründe build edilmiş içeriğin ikili (binary) formattaki çıktısını görebiliriz.

![gorouting_8.gif](/assets/images/2017/gorouting_8.gif)

Artık makinedeki diğer go örneklerimizde entity/starwars şeklinde paket tanımlayıp kullanabiliriz.

Örneğin Klasör Yapısı

Uygulamamızda statik web içeriğini ve REST tadındaki GET taleplerini karşılayacak yönlendirmeleri bir arada sunacağız. Burada dikkat edilmesi gereken nokta statik içeriğin bir alt alanda oluşturulması. Aksi halde joker karakter kullanımı (filepath şeklinde olan) tüm yönlendirme taleplerini karşılayacağından build işleminde hata alırız. Bu yüzden örneğin klasör yapısını aşağıdaki gibi oluşturabiliriz.

-\
-\main.go
-\static
-\static\index.html
-\static\cover.jpg
-\static\common.css

Static klasörü tahmin edeceğiniz üzere alt alanımız olarak görev yapacak.

Ön Hazırlıklar

main.go içeriğine geçmeden önce static klasöründeki index.html ve common.css içeriklerini tasarlayalım. Açılış sayfası gibi düşünebileceğimiz index.html içeriği şu şekildedir.

```text
<html>
<head>
	<title>Starwars Models</title>
	<link rel="stylesheet" href="common.css">
</head>
<body>
	<h1>My Starwars Collection</h1>
	<img src="cover.gif"/>
	<p>This is a JSON based data service<br>Try this url : <a href="/category/fighter">category/fighter</a></p>
	<a href="/category">All Categories</a>
</body>
```

ve common.css

```text
body{
	border:3px solid #BA4A00;
	border-radius: 16px;
	border-width: 10px;
	text-align: center;
	width: 420px;
	height: 500px;
	margin: auto;
	font-family:Tahoma, sans-serif;
	font-size:18px;
	color:#212F3D;
}
```

Sonuçta aşağıdaki ekran görüntüsündeki gibi bir içerik oluşturmaya çalışıyoruz. Benim için örneği biraz daha keyifli hale getirdiğini söyleyebilirim.

![gorouting_9.gif](/assets/images/2017/gorouting_9.gif)

Dikkat edilmesi gereken nokta index.html'e ulaşırken http://localhost:4569/static adresi üzerinden gidiyor olmamız. main paketinde buna göre bir kodlama yapacağız.

main.go

Nihayet yazımızın en önemli kısmına geldik. İşte main paketimize ait kodlarımız.

```cpp
package main

import (
	"encoding/json"
	"entity/starwars"
	"fmt"
	"net/http"
	"strings"

	"github.com/julienschmidt/httprouter"
)

func main() {
	router := httprouter.New()

	router.ServeFiles("/static/*filepath", http.Dir("static"))
	router.GET("/category", GetCategories)
	router.GET("/category/:name", GetModelsByCategoryName)

	http.ListenAndServe(":4569", router)
}

func GetCategories(response http.ResponseWriter, request *http.Request, params httprouter.Params) {
	c, _ := loadDataSet()
	cJson, _ := json.Marshal(c)
	response.Header().Set("Content-Type", "application/json")
	response.WriteHeader(200)
	fmt.Fprintf(response, "%s", cJson)
}

func GetModelsByCategoryName(response http.ResponseWriter, request *http.Request, params httprouter.Params) {
	_, models := loadDataSet()
	var result []starwars.Model
	for _, m := range models {
		if strings.ToLower(m.Category.Name) == strings.ToLower(params.ByName("name")) {
			result = append(result, m)
		}
	}
	cJson, _ := json.Marshal(result)
	response.Header().Set("Content-Type", "application/json")
	response.WriteHeader(200)
	fmt.Fprintf(response, "%s", cJson)
}

func loadDataSet() (categories []starwars.Category, models []starwars.Model) {
	fighter := starwars.Category{Id: 1, Name: "Fighter"}
	cruiser := starwars.Category{Id: 2, Name: "Cruiser"}

	vwing := starwars.Model{Id: 1, Title: "V-Wing Fighter", Price: 45.50, Category: fighter}
	n1 := starwars.Model{Id: 2, Title: "Naboo N-1 Starfighter", Price: 250.45, Category: fighter}
	republicCruiser := starwars.Model{Id: 3, Title: "Republic Cruiser", Price: 450.00, Category: cruiser}
	attackCruiser := starwars.Model{Id: 4, Title: "Republic Attack Cruiser", Price: 950.00, Category: cruiser}
	eta2 := starwars.Model{Id: 5, Title: "ETA-2 Jedi Starfighter", Price: 650.50, Category: fighter}
	delta7 := starwars.Model{Id: 6, Title: "Delta-7 Jedi Starfighter", Price: 250.35, Category: fighter}
	bwing := starwars.Model{Id: 7, Title: "B-Wing", Price: 195.50, Category: fighter}
	ywing := starwars.Model{Id: 8, Title: "Y-Wing", Price: 45.50, Category: fighter}
	monCalamari := starwars.Model{Id: 9, Title: "Mon Calamari Star Crusier", Price: 1500.00, Category: cruiser}

	categories = append(categories, fighter, cruiser)
	models = append(models, vwing, n1, republicCruiser, attackCruiser, eta2, delta7, bwing, ywing, monCalamari)

	return categories, models
}
```

JSON çıktısı üreteceğimiz için encoding/json, web sunucusu dinlemesi yapacağımız için net/http, yönlendirme işlemleri için github.com/julienschmidt/httprouter, küçük harfe çevirerek kıyaslama yapmak için strings, star wars tiplerini kullanmak için entity/starwars ve son olarak Fprintf fonksiyonlliği ile HTML çıktısını oluşturmak için fmt paketlerini kullandığımızı söyleyelim.

En sonda yer alan loadDataSet fonksiyonu Category ve Model tipinden slice örnekleri oluşturup döndürüyor. Onunla ilgili olarak söyleyebileceğimiz en güzel şey n sayıda parametre döndüren fonksiyonlara bir örnek olması. append çağrıları ile n sayıda tipi ilave ettiğimiz de dikkatten kaçmamalı. main fonksiyonu içerisinde Router nesnesini örnekleyerek işe başıyoruz. Bu sefer bir önceki yazımızda ele aldığımız GET çağrıları dışında ServeFiles isimli bir kullanım da söz konusu. Bu fonksiyon ilk parametre olarak statik sayfalarımızı tuttuğumuz adreslemeyi alıyor. *filepath kullanımına dikkat etmek lazım. Case-Sensitive bir ifade olduğunu belirtelim. * işareti nedeniyle ikinci parametre ile bildirilen klasördeki her dosyanın ele alınacağını bildiriyoruz. Bu tanımlama ile static adresine gelecek taleplerin hangi fiziki adresten karşılanacağını belirtmiş olduk. ServeFiles bildiriminde daha geniş imkanlara da sahibiz. Nitekim statik dosyaların ele alınması sırasında araya girip HTTP paketine müdahale edebilir Header kısımlarını kurcalayabiliriz ([Şu adresteki tartışmayı](https://github.com/julienschmidt/httprouter/issues/40) incelemenizi öneririm. Statik sayfalarda Cache-Control header bilgisinin nasıl ilave edilebileceği incelenmiş)

> Eğer /static/ *filepath bildirimi yerine /* filepath şeklinde bir tanımlama yaparsak kodun derlenmesi sırasında aşağıdaki hataları alırız.
> ```text
> panic: '/category' in new path '/category' conflicts with existing wildcard '/*filepath' in existing prefix '/*filepath'
>
> goroutine 1 [running]:
> panic(0x5fd040, 0x125624d0)
> 	C:/Go/src/runtime/panic.go:500 +0x331
> github.com/julienschmidt/httprouter.(*node).addRoute(0x1254a630, 0x6379cc, 0x9, 0x65ead8)
> 	c:/go works/samples/src/github.com/julienschmidt/httprouter/tree.go:162 +0x6db
> github.com/julienschmidt/httprouter.(*Router).Handle(0x125d0340, 0x636829, 0x3, 0x6379cc, 0x9, 0x65ead8)
> 	c:/go works/samples/src/github.com/julienschmidt/httprouter/router.go:236 +0x1b2
> github.com/julienschmidt/httprouter.(*Router).GET(0x125d0340, 0x6379cc, 0x9, 0x65ead8)
> 	c:/go works/samples/src/github.com/julienschmidt/httprouter/router.go:180 +0x4a
> main.main()
> 	C:/Go Works/Samples/book/Web Programming/Lesson_26/Server.go:25 +0xe2
> Error: process exited with code 2.
> ```

GET ile yapılan fonksiyon yönlendirmelerinde JSON üretimi için gerekli adımlar atılıyor. JSON çıktısı için Marshal fonksiyonunu kullanmamız yeterli. Bunların dışında çıktıyı üretirken Header'a içerik tipinin JSON formatında olduğunu belirtiyoruz ki istemciler gelen içeriğin ne olduğunu anlayabilsinler. WriteHeader fonksiyonuna verilen 200 değeri tahmin edeceğiniz üzere HTTP 200 kodunu belirtmekte. JSON çıktısını cevap olarak yazmak için Fprintf fonksiyonunu ele alıyoruz. GetModelsByCategoryName fonksiyonunda parametreye gelen kategori adını kullanarak bir sonuç kümesi oluşturmaktayız. Kategori adını params.ByName çağrısını kullanarak yakalıyoruz. Buna göre belli bir kategorideki modelleri elde ederek JSON çıktısı üretiyoruz.

Sonuçlar

Eğer doğrudan / lokasyonuna gidersek pek tabii HTTP 404 not found hatası alırız. Nitekim bu adres için bir yönlendirme yapmadık. Statik içeriklerimiz /static altında yer alıyor. Diğer yandan index.html'deki All Categories bağlantısına basarsak veya URL bilgisi olarak /category şeklinde bir talep gönderirsek aşağıdaki JSON içeriğini elde ettiğimizi görebiliriz.

![gorouting_10.gif](/assets/images/2017/gorouting_10.gif)

Eğer fighter ya da cruiser kategorisindeki modelleri görmek istersek göndereceğimiz taleplere karşın aşağıdaki sonuçları alırız.

/category/fighter

![gorouting_11.gif](/assets/images/2017/gorouting_11.gif)

/category/cruiser için

![gorouting_12.gif](/assets/images/2017/gorouting_12.gif)

Pek tabii olmayan kategori için slice içeriği boş olacağından null bir JSON çıktısına ulaşırız.

![gorouting_13.gif](/assets/images/2017/gorouting_13.gif)

Bu noktada belki de çok daha şık bir HTML hata sayfasına yönlendirme yaptırabiliriz ne dersiniz? Görüldüğü gibi Router nesne örnekleri üzerinden ServeFiles, GET gibi fonksiyonları bir arada kullanarak static içerik sunabilen ve REST davranış gösterip HTTP taleplerine JSON çıktılarla cevap veren bir web uygulaması geliştirmek oldukça kolay. Bu tekniği kullanarak veri içeriği sunan basit REST servisleri help sayfaları ile birlikte geliştirmeniz mümkün. Yine de kaçırdığım çok şey olduğundan adım gibi eminim. Şu konuda HTTP Post, Put, Delete, Patch gibi fonksiyonellikleri bir deneyimlemek lazım. Bunları da ilerleyen zamanlarda incelemeye çalışacağım. Şimdilik bu kadar. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
