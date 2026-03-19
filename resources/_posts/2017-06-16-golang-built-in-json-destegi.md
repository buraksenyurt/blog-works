---
layout: post
title: "GoLang - Built-In JSON Desteği"
date: 2017-06-16 06:18:00 +0300
description: "JSON(JavaScriptObjectNotation) ile barışık yaşamayan programlama dili neredeyse yok gibi. Özellikle son on yıl zarfında geliştirilen veya ön plana çıkan ne kadar dil varsa JSON için çekirdekten destek sunuyor. GoLang için de benzer durum söz konusu. Nasıl mı? Aynen aşağıdaki kod parçasında görüldüğü gibi."
categories:
  - golang
tags:
  - golang
  - nosql
  - xml
  - rest
  - json
  - http
  - go
  - serialization
---
# GoLang - Built-In JSON Desteği
![gophergo.gif](/assets/images/2017/gophergo.gif)

Merhaba Arkadaşlar,

Mesajlaşma formatlarından çeşitli NoSQL sistemlerine, REST tabanlı veri servislerinden mobil cihazlardaki depolama kabiliyetlerine kadar pek çok alanda JSON (JavaScriptObjectNotation) standardının kullanıldığını görüyoruz. Özellikle XML (eXtensible Markup Language) kadar fazla yer tutmuyor oluşu da onu ön plana çıkartan özelliklerinden birisi. Hatta sıkıştırılmış formatının kullanıldığı ağ protokolleri bile olabiliyor. Verinin rahatça okunabildiği bu standart ile barışık olmayan programlama dili neredeyse yok gibi (Internet Engineering Task Force kurumunun JSON veri değiş-tokuş standartları ile ilgili yazısına [buradan](https://tools.ietf.org/html/rfc7159), JSON API standartları ile ilgili DevNot üzerinden yayınlanan yazıya da [şuradan](http://devnot.com/2017/json-api-standardi/) bakabilirsiniz) Özellikle son on yıl zarfında geliştirilen veya ön plana çıkan ne kadar dil varsa JSON için çekirdekten destek sunuyor. Sunmayanlar da bu sürede ek paket veya eklentilerle bu veri modelini kullandırıyor. GoLang için de benzer durum söz konusu. Nasıl mı? Aynen aşağıdaki kod parçasında olduğu gibi.

```cpp
package main

import (
	"fmt"
	"encoding/json"
	"os"
)

func main() {
	// json serileştirme için kullanacağımız Game yapısından bir tip tanımlıyoruz
	goldZone:=Game{
		5555,
		"Mohani Gezegeni Görevi",
		[]Player{
			Player{100,"deli","cevat",10.90},
			Player{102,"nadya","komenaççi",12.45},
			Player{103,"biricit","bardot",900.45},
		},
	}	
	
	jsonOutput,_:=json.Marshal(goldZone)
	fmt.Println(string(jsonOutput))
	
	var game Game	
	if err := json.Unmarshal(jsonOutput,&game); err != nil {
        	panic(err)
    	}
	
	fmt.Printf("Game : %s\n",game.Name)
	for _,player:=range game.Players{
		fmt.Println(player.Id,player.FirstName,player.Point)
	}
	
	// dilersek json sınıfının NewEncoder metodunu kullanarak
	// çıktıları farklı yerlere yönlendirebiliriz
	// işletim sistemi ekranı veya bir HTTP mesajının gövdesi gibi
	encoder := json.NewEncoder(os.Stdout)    	
    	encoder.Encode(game)    	
}

type Player struct{
	Id int `json:"PlayerId"` // İstersek bir alanın JSON çıktısında nasıl adlandırılacağını söyleyebiliriz
	FirstName string // Büyük harf public'lik anlamındadır!
	lastName string //küçük harfle başlayanlar private'lık kazanır. O yüzden json çıktısına yansımaz
	Point float32
}

type Game struct{
	Id int
	Name string
	Players []Player
}
```

Kodun çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![gojson.gif](/assets/images/2017/gojson.gif)

Primitive tiplerin JSON formatında serileştirilmesi oldukça kolay zaten. Bu nedenle örnek uygulamamızda Game ve Player isimli iki struct tipi kullanıyoruz. Game tipi içinde Player tipinden bir slice yer alıyor. JSON serileştirme ve ters-serileştirme işlemlerini Marshal ve Unmarshal metodları ile gerçekleştirebiliriz. json tipi encoding/json paketi içerisinde yer almaktadır. Marshal çıktısını aldıktan sonra ekrana basarken string tipine dönüştürme işlemi uyguladık (Bunu yapmadığımız takdirde nasıl bir sonuç elde edeceğinizi inceleyin lütfen) Unmarshal metodu sırasında oluşabilecek bir paniği kontrol altına almaya da çalıştık. Marshal ve Unmarshal çağrıları dışında NewEncoder metodunu kullanarak çıktıların bir Stream'e doğru yönlendirilmesi de sağlanabilir. Bu stream işletim sistemine ait terminali gösterebileceği gibi bir HTTP mesajının body kısmı da olabilir. Son kod satırında bu işlem ele alınmış ve çıktılar doğrudan terminale verilmiştir. İlgili özelliği kullanabilmek için os paketi koda dahil edilmiştir.

> Örnek kodu çalışırken ilk başta JSON çıktısına hiçbir alan adının yazılmadığını fark ettim. Sonrasında bunları küçük harfle yazdığımı gördüm. Küçük harf ile başlamak GO dilinde private'lık anlamına geliyor. Büyük harf ile başlandığınında ise ilgili üyeye public'lik kazandırmış oluyoruz. Bunu yeni öğrendim. Utanıyorum.

Uygulamanın ekrana bastığı JSON içeriğini Chorme gibi bir tarayıcıda göstermek isterseniz aşağıdaki sonuçları görmemiz gerekiyor (Chorme'da JSONView eklentisini kullandığımı belirteyim)

![tfi159_2.gif](/assets/images/2017/tfi159_2.gif)

Pek tabii size düşen bir kaç görev var. Örneğin bu çıktıyı fiziki bir dosyaya kaydetmeyi deneyebilirsiniz. Sonrasında ise kaydedilen dosya içeriğinden ilgili içerikleri canlandırmaya çalışabilirsiniz. Bu işi biraz daha ileri götürüp JSON formatında veriler ile çalışan basit bir NoSQL veritabanı sistemi yazabilir hatta fonksiyonelliklerini (ekleme,çıkartma,arama vb) REST API olarak dış dünyaya sunabilirsiniz. Bence bunu bir deneyin. Böylece geldik kısa bir kod parçasının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.