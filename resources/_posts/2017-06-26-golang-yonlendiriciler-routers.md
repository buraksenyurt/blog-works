---
layout: post
title: "GoLang - Yönlendiriciler (Routers)"
date: 2017-06-26 10:30:00 +0300
description: "Yönlendirme(Routers) mekanizmaları özellikle anlamlı http taleplerinde önemli bir rol oynar. Bir tarayıcının URL satırından gelen ifadelerin sunucu tarafında ele alınması sırasında işleri kolaylaştırıcı kütüphaneler de bulunmaktadır. Sevgili Murat Özalp'ın kitabında ilerlerken GO'nun built-in yönlendirme mekanizmaları dışında github üzerinden sunulan pek çok basit ve kullanışlı çatının daha olduğunu öğrendim. Tabii burada bahsedilen kütüphaneler ağırlıklı olarak web taleplerinin bir eşleştirme koduna göre uygun fonksiyonlara yönlendirilmesi ve cevaplandırılması ile alakalıydı."
categories:
  - golang
tags:
  - golang
  - nosql
  - rest
  - json
  - http
  - go
  - concurrency
  - performance
  - github
---
# GoLang - Yönlendiriciler (Routers)
![gorouting_1.gif](/assets/images/2017/gorouting_1.gif)

Merhaba Arkadaşlar,

Yönlendirme (Routers) mekanizmaları özellikle anlamlı HTTP taleplerinde önemli bir rol oynar. Bir tarayıcının adres satırından gelen ifadelerin sunucu tarafında ele alınması sırasında işleri kolaylaştırıcı kütüphaneler de bulunmaktadır. Sevgili Murat Özalp'ın kitabında ilerlerken GO'nun built-in yönlendirme mekanizmaları dışında github üzerinden sunulan pek çok basit ve kullanışlı çatının olduğunu öğrendim. Tabii burada bahsedilen kütüphaneler ağırlıklı olarak web taleplerinin bir eşleştirme koduna göre uygun fonksiyonlara yönlendirilmesi ve cevaplandırılması ile alakalıydı. Bazıları performans açısından öne çıkarken bazıları komple bir web çatısını sunma kabiliyetine sahipti. Güncel listeye [şu adresten bakabilirsiniz.](https://github.com/naoina/go-http-routing-benchmark) Yeni paketler geldikçe veya var olanlarda değişiklikler oldukça benchmark sonuçları da etkilenecektir. Bu nedenle ara ara uğramakta yarar olduğu kanısındayım.

Bende kitabın sıkı bir takipçisi olarak örnek üzerinden ilerlemeye başladım ve bahsedilen [httpRouter](https://github.com/julienschmidt/httprouter) kütüphanesini kullanmayı denedim. Pek tabii aynı örneği değil de konuyu kendim için eğlenceli hale getirecek bir versiyonunu yapmaya çalıştım. Aklıma en sevdiğim film serilerinden olan Star Wars'taki gezegenler geldi. Bir kaç gezegeni ve önemli şehirlerini şimdilik bellekte tutacağım bir yapı (struct) ilişkisi oluşturup aşağıdaki HTTP taleplerini karşılayacak bir web sunucusu yazmak eğlenceli olabilirdi (Yandaki fotoğrafta görülen galaksinin detaylarına ve çok daha kapsamlı bir sunumuna [şu adresten ulaşabilirsiniz](http://starwars.wikia.com/wiki/Star_Wars:_Complete_Locations). Adamlar üşenmemişler koskoca bir evreni hayal edip kurgulamışlar. Bu kurgulamayı Star Trek serisinde de görmekteyiz.)

/ ile bir karşılama sayfasına yönlendirilip
/planets ile gezegenler listesini gösterip
/planets/:name ile de bir gezegendeki şehirleri verebilirdim.

3ncü tanımlamada dikkat edileceği üzere bir parametre de söz konusu. Böylece HTTP Get talebinde gelen bir parametreyi alıp nasıl kullanabileceğimi de görmüş olacaktım.

Kodlar

Kod içeriğini genel hatları ile şöyle özetleyebiliriz. Planet ve City isimli iki yapı bulunuyor. Bu yapılara ait test içeriklerinin yüklendiği bir fonksiyonumuz da var. github adresinden referans edilen httpRouter paketinin nimetlerinden yararlanaraktan localhost:4568 adresinden bir sunucu ayağa kaldırıyoruz. Sunucu yukarıdaki 3 temel talebi alıp işleyecek şekilde çalışıyor. Nihayi sonuçta kullanıcılara gezegenleri ve bu gezegenlerdeki önemli şehirleri göstermeyi planlıyoruz. Tüm kod içeriğini aşağıda görebilirsiniz.

```cpp
package main

import (
	"fmt"
	"net/http"
	"strings"

	"github.com/julienschmidt/httprouter"
)

func main() {
	router := httprouter.New()              
	router.GET("/", Index)                  
	router.GET("/planets", GetPlanets)      
	router.GET("/planets/:name", GetCities) 
	http.ListenAndServe(":4568", router)    
}

func GetCities(response http.ResponseWriter, request *http.Request, params httprouter.Params) {
	planetName := params.ByName("name") 
	planets := LoadSomeData()
	fmt.Fprintf(response, `<html><head><title>%s</title></head><body><h1>%s</h1>`, planetName, planetName)
	for _, planet := range planets {
		if strings.ToLower(planet.Name) == strings.ToLower(planetName) {
			for _, city := range planet.Cities {
				fmt.Fprintf(response, `<p><b>%s</b>-%s</p>`, city.Name, city.Affiliation)
			}
			break
		}
	}
	fmt.Fprintf(response, `</body></html>`)
}

func GetPlanets(response http.ResponseWriter, request *http.Request, params httprouter.Params) {
	planets := LoadSomeData() 
	fmt.Fprintf(response, `<html><head><title>Planets</title></head><body><h1>Planets</h1>`)
	for _, planet := range planets {
		fmt.Fprintf(response, `<p><a href='planets/%s'>%s-%s (%d)</p>`, planet.Name, planet.Name, planet.Sector, planet.Population)
	}
	fmt.Fprintf(response, `</body></html>`)
}

func Index(response http.ResponseWriter, request *http.Request, params httprouter.Params) {
	fmt.Fprintf(response, `<html>
		<body>
		<head>
			<title>Star Wars Planets</title>
		</head>
		<body>
			<h1>Star Wars Planets</h1>
			<a href="/planets">Planets</a><br/>
			<p>Planet list updates every morning with new planets</p>
		</body>
		</html>`)
}

func LoadSomeData() []Planet {
	var planets []Planet

	planets = append(planets, Planet{Name: "Naboo", Sector: "Chommel", Population: 4500000,
		Cities: []City{
			City{Id: 1, Name: "Theed", Affiliation: "Galactic Empire"},
			City{Id: 2, Name: "Umberbool City", Affiliation: "Gungan Grand Army"},
			City{Id: 3, Name: "Spinnaker", Affiliation: "Galactic Empire"},
			City{Id: 4, Name: "Otoh Gunga", Affiliation: "Trade Federation"},
		}})

	planets = append(planets, Planet{Name: "Coruscant", Sector: "Corusca", Population: 1000000000,
		Cities: []City{
			City{Id: 1, Name: "Galactic City", Affiliation: "Rebellian"},
		}})

	planets = append(planets, Planet{Name: "Mustafar", Sector: "Atravis", Population: 20000,
		Cities: []City{
			City{Id: 1, Name: "Fralideja", Affiliation: "Rise of Empire"},
		}})

	return planets
}

type Planet struct {
	Name       string
	Sector     string
	Population int64
	Cities     []City
}

type City struct {
	Id          int
	Name        string
	Affiliation string
}
```

Kodda Neler Oluyor?

Öncelikle bu go dosyasının github üzerinden bir paketi import ettiğini belirtelim. LiteIDE kullananlar bu anlamda şanslılar. Nitekim paketin adını doğru şekilde yazdıysak, Build-Get menü seçeneğini kullanarak referans edilen paketlerin sisteme otomatik olarak indirilmesini ve kurulmasını sağlayabiliyoruz (Hatta LiteIDE'nin otomatik formatlama özelliğinin github paketinin önüne boş bir satır koyduğunu fark ettim. Ben üste çıkartsam da o boşluk konuldu. Fark ettim ki bu şekilde bakınca built-in paketler ile harici paketleri gözle ayırt etmek çok kolaylaşıyor. İnce ve güzel düşünülmüş) main fonksiyonunda ilk olarak bir Router nesnesi örnekleniyor. Bu nesnenin GET, POST, PUT, DELETE, PATCH gibi çeşitli HTTP taleplerine cevap verebilecek fonksiyonları bulunuyor. Örnekte sadece GET talepleri ele alınıyor ki bu sayede basit bir REST servisinin yolu da açılmış oluyor. Lakin biz örneğimizde istemcilere HTML çıktısı vereceğiz.

Yukarıda bahsettiğimiz tüm adresleri GET fonksiyonunun ilk parametresi olarak kullanıyoruz. Eğer dinamik bir url parametresi söz konusu ise:ParametreAdı notasyonunu kullanıyoruz. Bu notasyon tahmin edileceği üzere yönlendirmenin yapıldığı fonksiyonda ele alınacak. İkinci parametreler ile Index, GetPlanets ve GetCities fonksiyonlarına yönlendirmeler yapmaktayız. Pek tabii son olarak yerel makinedeki bir portu dinlemek üzere sunucuyu ayağa kaldırıyoruz. Burada standart http.ListenAndServe fonksiyonunu kullanmaktayız. Dikkat edilmesi gereken nokta ikinci parametreye router değişkeninin yazılmış olması. Dolayısıyla ayağa kalkan sunucuya gelen taleplerin Router nesne örneği tarafından ele alınacağını bildirmiş oluyoruz (Basit bir injection yapıldı sanki?)

Index, GetPlanets ve GetCities fonksiyonlarının parametre yapıları aynı. 3ncü parametrelerde url üzerinden gelecek:ParametreAdi formasyonundaki değişkenleri yakalayabiliyoruz ki GetCities fonksiyonunda bir gezegen adını alıp o gezegendeki şehirleri listelemek için kullanmaktayız. Index fonksiyonu çok çok basit bir HTML içeriğini fmt paketinin Fprintf yordamı ile ResponseWriter üzerinden istemciye basmakta. Burada esprilektüel bir şey yok diyebiliriz. GetPlanets fonksiyonunda ise, LoadSomeData ile yüklediğimiz slice içeriğini for döngüsü yardımıyla hazırlayarak istemciye göndermekteyiz. for döngüsünün önünde ve sonrasında tipik olarak HTML paketini hazırlıyoruz (ki çok daha şekilli hale getirilebilir. CSS giydirmeyi deneyin) Döngü içerisinde ise her bir gezegenin adını, hangi sektörde bulunduğunu ve toplam nüfusunu yazdırıyoruz. Bunu yaparken de bir hyperlink haline getiriyoruz. Link elementinin bağlantısı ise dikkate değer. planets/[gezegenAdı] şeklinde bir yazım söz konusu. Bu yazım ile GetCities fonksiyonu tarafından ele alınacak url bilgisini oluşturmaktayız. GetCities fonksiyonunda, gezegen adını url'den belirtildiği şekilde almak için params.GetName fonskiyonuna başvurmaktayız. Yine for döngüsü ile slice içeriğini dolaşıp aranan gezegene geldikten sonra ilgili gezegene ait şehirler dizisinde dolaşıyor ve belirli HTML elementlerini üretip ekrana bastırıyoruz.

Çalışma Zamanı

Geldik işin eğlenceli kısmına. Önce go uygulamasını build edip çalıştıralım. Sonrasında ilk talebi 4568 nolu porta göndererek ilerleyelim. Aşağıdakine benzer bir sonuçla karşılaşmalıyız.

![gorouting_2.gif](/assets/images/2017/gorouting_2.gif)

Görüldüğü gibi Index sayfasının içeriğini başarılı bir şekilde yolladık. Planets bağlantısına basarsak bu kez GetPlanets fonksiyonunun devreye girip aşağıdaki çıktıyı ürettiğini görürüz. Örnek gezegenlerimizin tamamı geldi.

![gorouting3.gif](/assets/images/2017/gorouting3.gif)

Bu gezegen linklerine bastığımızda da önemli şehirlerinin listesini getirecek GetCities fonksiyonunun çıktıları ile karşılaşırız. Örnek iki tanesini aşağıda görebilirsiniz.

Naboo

![gorouting4.gif](/assets/images/2017/gorouting4.gif)

ve Mustafar

![gorouting5.gif](/assets/images/2017/gorouting5.gif)

Elbette olmayan bir gezegen adını bilinçli olarak girersek aşağıdaki çıktıyı

![goroutine6.gif](/assets/images/2017/goroutine6.gif)

ya da /Planets yerine hatalı bir giriş yaparsak da "404 page not found" cevabını alırız.

![goroutine7.gif](/assets/images/2017/goroutine7.gif)

Neler Eksik?

Aslında kurguladığımız düzenek modern (querystring kullanmayan, okunaklı url içerikleri olarak düşünebiliriz) HTTP Get taleplerini alıp HTML içerikleri üreten basit bir web sunucusu. Çok doğal olarak eş zamanlı gelecek milyonlarca talep söz konusu olduğunda bunu farklı bir şekilde ele almak gerekir. Kodda kullandığımız Planet-City ilişkili yapılar bellekte tutulan nesnel koleksiyonlar. Bu içeriklerin bir veritabanı sisteminden (SQLite olur, NoSQL tabanlı bir küme olur, Google'ın Cloud çözümlerinden Firebase olur vs) alınması daha çok tercih edilir bir durumdur. SQLite ile ilgili çalışmaları da öğrenmeye başladım. Umarım burada paylaşabilirim. Kullanılan HTML içeriklerinin dinamik üretimleri için şablonlardan (Templates) faydalanıp görselliğin CSS'ler ile daha da keyifli hale getirilmesi sağlanabilir. Gezegen ve şehir fotoğraflarının koyulması bile acayip fark yaratabilir. Bu yönlendirme tekniklerinden yola çıkarak tamamen veri-odaklı bir REST servisinin geliştirilmesi de mümkündür. Veriler pekala JSON formatında kolayca basılabilir. Bu güzel konuları siz değerli okurlarıma armağan ediyorum. Bir başka makalemizde görüşmek üzere hepinize mutlu günler dilerim.