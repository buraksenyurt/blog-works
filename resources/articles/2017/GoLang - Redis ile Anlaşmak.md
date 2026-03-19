---
title: "GoLang - Redis ile Anlaşmak"
pubDate: 2017-08-03 21:16:00
categories:
  - GoLang
  - NoSQL
tags:
  - GoLang
  - NoSQL
  - Bash
  - .NET
  - Redis
  - REST
  - JSON
  - Go
  - Ruby
  - JavaScript
  - React
  - GitHub
---

# GoLang - Redis ile Anlaşmak
![goredis_2.gif](images/goredis_2.gif)

Merhaba Arkadaşlar,

Bir haziran gecesiydi. Dışarıda hava nemli ve sıcaktı. Bir süre önce başlayan yağmurun sesi çalışma odamın pencersinden kulaklarıma tatlı tatlı geliyordu. Biraz da toprak kokusu vardı tabii. Evde el ayak çekilmiş sakin bir ortam oluşmuştu. Bol kafein dolu bardağım elimde internetten bir şeyler okuyordum. İnsanlar çıldırmıştı. Javascript Framework'ler, yapay zeka'lar, react'ler, cordova'lar,.net core'lar, sanayi 4.0'lar, tesla'lar ve daha niceleri. Eskisinden daha hızlı bir şekilde geride kaldığımı hissediyordum. Sanırım sonum örümcek adamın amcası gibi evde bozuk ampülü tamir edip gazetede iş arayan ama bulamayan biri gibi olacaktı. Ama direniyordum. Önce Ruby, sonra Pyhton ve derken GO. Amatör seviyede başlamış biraz ilerlemiştim. Kendime bir çalışma döngüsü kurmuştum. Bir süre Ruby bakıyor, sonra Pyhton bakıyor, sonra GO bakıyor sonra bu döngüyü tekrar başa sarıyordum. GO'nun ikinci iterasyonundaydım.

Bu sefer elimde oldukça güzel de bir Türkçe kaynak vardı (Murat Özalp'in Go Programlama isimli kitabını şiddetle tavsiye ederim) Kitabın son sayfalarına gelmiştim. Döngünün bir sonraki adımına geçmeden önce biraz daha uygulama yapmam gerekiyordu. Nitekim dile hakim olabilmek için bol bol kod yazmam şarttı. Mesela GoLangWeekly'de yayımlanan başlıklara göz gezdirdiğimde çoğunu anlamakta güçlük çekiyordum. Bu yüzden seçmece ilerlemek durumundaydım. Aslında programlama dilini ufak ufak kavramaya başlamıştım ama örnek senaryolar işleterek ilerlemem gerektiğini de biliyordum. Gelişebilmem için bu şarttı. Bu karman çorman düşünceler altında devam ederken aklıma bir pratik geldi. Eskiden.Net tarafında kullandığım NoSQL sistemlerinden olan Redis ile ilgili bir şeyler yapmak istiyordum.

> 2014 yılında onu kısaca incelemeye çalışmış ve [bir şeyler karalamıştım](https://www.buraksenyurt.com/post/NoSQL-Maceralarc4b1e28093Redis-ile-Hello-World). Hatta Redis'in genel özelliklerini oradan okuyarak hatırlamaya çalıştım. Bu yazı için kısaca özetlemek gerekirse bellekte çalışan, key-value tipinde ve dağıtık yapıda sunulabilen bir NoSQL sistemi olduğunu ifade edebiliriz. Redis'in key-value tipinde bir veri tabanı olması bizi yanıltmamalıdır. key'ler string olsa da value olarak kullanabileceğimiz beş temel tip bulunur. string, hash, list, set ve sortedSet. Dolayısıyla oldukça geniş bir veri yapısını kullanma şansımız vardır.

Zaman hızla geçmiş bir çok şey de değişmişti tabii. Bir süredir gözde dillerimden birisi olan Go'da Redis'i nasıl kullanabileceğimi merak ediyordum. O gece Windows makinesinde çalışmaktaydım. Aradan geçen zamana rağmen Redis'in Windows sürümü halen çalışmaktaydı. Yeni Redis versiyonlarına göre yeni sürümleri de yayınlanıyordu. Dolayısıyla siz de Windows sürümündeyseniz [şu adresten](https://github.com/MSOpenTech/redis/releases) uygun MSI ile yüklemeyi yapabilir ve yazının kalanında bana eşlik edebilirsiniz.

Komut Satırında Kısa Bir Tur

Yükleme işlemi sonrası hemen komut satırına geçtim ve redis-server.exe'yi kurulumun yapıldığı lokasyondan çalıştırdım. Sonra bir kaç deneme yapmak için redis-client ile açılan kısma geçtim. Redis varsayılan olarak yerel makinede 6379 numaralı porttan hizmet veren bir servis. Tabii farklı node'lar söz konusu olursa farklı port'lar ile de haberleşebilmemiz mümkün. Örneğimiz şimdilik tek bir sunucu örneği üzerinden çalışıyor.

Saatler ilerlerken konunun verdiği heyecanla komut satırından bir kaç deneme yapmayı da ihmal etmedim.

![goredis_1.gif](images/goredis_1.gif)

İlk olarak redis ile ping-pong oynadım:) Siz ping yazdığınızda O da PONG diyorsa bu konuşabildiğiniz anlamına gelir. Ardından ilk iş players:reksar isimli bir key oluşturmak oldu. Değeri ise JSON formatında bir içerikten ibaretti. get komutunu kullanarak belleğe atılan bu key içeriğini okuyabiliriz.

Sonrasında hmset ile bir hash üretmeye çalıştım. hmset ile bir key için n sayıda değer içerecek alanlar (fields) tanımlayabiliriz. language:go isimli key bu şekilde yazıldı. Oluştururken bir field bir value, bir field bir value şeklinde ilerlemek gerekiyor. Daha sonra hmget ile language:go içeriğini almaya çalıştım. Ancak ilk denemede hata yaptım. Nitekim bu komut ile bir hash içerisindeki belli bir alanın değerini almaktayız. type alanının değerini okuduktan sonra tüm alanların içeriğini hgetall komutu ile elde etmeyi başardım. Dilersek sadece key değerlerini de yakalayabiliriz ki hkeys bu noktada devreye girmekte.

İlerleyen satırlarda basit bir liste oluşturup ona elemanlar eklemeyi ve tüm içeriği ekrana yazdırmayı denedim. Bu amaçla lpush ve lrange isimli komutlardan yararlandım. Bu şekilde komut satırından çalışmaya da devam edilebilirdi tabii ama hedefim bunu Go ile gerçekleştirmekti.

GoLang Zamanı

Diğer platformlarda olduğu gibi Redis'i GoLang ile kullanmak için harici bir paketten destek almam işleri kolaylaştırıyor. Aslında bu şart değil. Sonuçta servis bazlı bir veritabanı motoru söz konusu ama şimdilik paket ile ilerlemek benim için daha iyi. Bir kaç araştırma ve blog yazısından sonra [şu adresten yayınlanan bir go paketi](https://github.com/mediocregopher/radix.v2) buldum. LiteIDE'nin Get seçeneği ile ya da komut satırından ilgili paketi kolaylıkla sistemimize alabiliriz.

```bash
go get github.com/mediocregopher/radix.v2
```

## Önce Basit Bir String Ekleyelim

Gelelim örnek kod parçalarına. İlk olarak basit string türünden bir veri eklemeye çalıştım. Value olarak da JSON içeriği kullanmaya karar verdim.

```cpp
package main

import (
	"fmt"

	"github.com/mediocregopher/radix.v2/redis"
)

func main() {
	AddLudwig()
}

func AddLudwig() {
	conn, err := redis.Dial("tcp", "localhost:6379")
	if err != nil {
		fmt.Println(err.Error())
	} else {
		defer conn.Close()
		pong := conn.Cmd("ping")
		fmt.Println(pong.String())

		response := conn.Cmd("set", "players:ludwig", "{\"nick\":ludwig,\"genre\":classic,\"SongCount\":98}")
		if response.Err != nil {
			fmt.Println(response.Err)
		}
		fmt.Println(response.String())
	}
}
```

Fonksiyon redis tipinin Dial metodu ile başlıyor. TCP protokolü ile localhost üzerindeki 6379 nolu porta bağlanacağımızı ifade ediyoruz. Yani Redis sunucusuna. Eğer bağlanabiliyorsak (ki err nesnesi nil ise bağlanıyoruz diyebiliriz) önce ping pong oynuyor ve sonrasında players:ludwig isimli bir key gönderiyoruz. Değer olarak da JSON formatında bir içerik söz konusu. Ludwig'in takma adını, bestelediği şarkı türünü ve toplam parça sayısını tutan saçma bir verimiz var. Bu kodda en kritik nokta az önce terminalden yazdığımız redis komutlarının Cmd metodunda kullanılması. İlk çağrıda ping diğerinde ise set komutunu göndermekteyiz. defer ettiğimiz Close metodu fonksiyondan çıkarken redis bağlantısını kapatacak. Çalışma zamanı sonuçlarını aşağıda görebilirsiniz. Koddan eklediğimiz veriyi redis komut satırından da elde edebildik.

![goredis_3.gif](images/goredis_3.gif)

## Birde Hash Üretip Okuyalım

Kodları biraz daha ilerletmeye çalıştım. Acaba bir Hash nasıl üretilebilirdi ve hatta alanlarının değerlerini kod tarafından nasıl okuyabilirdim? Aşağıdaki gibi bir fonksiyon işime yarayacaktı.

```cpp
func AddAndReadHash() {
	conn, err := redis.Dial("tcp", "localhost:6379")
	if err != nil {
		fmt.Println(err.Error())
	} else {
		defer conn.Close()
		response := conn.Cmd("HMSET", "card:93", "nickName", "murlock", "greetings", "I'am ready, I'am not ready", "price", 5, "attack", 4, "defense", 4, "owner", "shammon")
		if response.Err != nil {
			fmt.Println(response.Err)
		}
		fmt.Println(response.String())
		read, _ := conn.Cmd("HGETALL", "card:93").Map()
		for k, v := range read {
			fmt.Printf("%s\t%s\n", k, v)
		}
	}
}
```

Bu kez HMSET komutunu kullanarak bir hash üretiliyor. card:93 olarak belirtilmiş bir key söz konusu. Bu verinin nickName, greetings, price, attack, defense ve owner isimli alanları bulunuyor. Bir takım test verileri koyarak redis'e gönderiyoruz. Okuma kısmında ise HGETALL komutunun çağırılması söz konusu. Ancak dikkat çekici nokta bu seferki çağrım sonrası Map isimli metodun kullanılması. Bu sayede hash içerisindeki key ve value bilgilerini dolaşabileceğimiz map türünden bir nesneyi elde edebiliyoruz. Sonrasında range fonksiyonunu kullanarak ilgili key:value çiftlerini ekrana yazdırıyoruz. İşi eğlenceli hale getirmek için farklı bir şekilde renklendirdiğim komut satırının çalışma zamanı çıktısı aşağıdaki gibi.

![goredis_4.gif](images/goredis_4.gif)

## Go Tarafında Veriyi Yapı (Struct) Olarak Ele Alsak

Lakin bir şeyler eksik gibi. Sakladığımız verinin kendisini belki de Go tarafından başka şekilde ifade edebiliriz. Örneğin oyun kartlarına ait bilgileri taşıyan bir yapı (struct) tasarlayıp onu bu senaryoda ele almamız daha doğru olabilir. O zaman kodları aşağıdaki hale getirerek yolumuza devam edelim.

```cpp
package main

import (
	"fmt"
	"strconv"

	"github.com/mediocregopher/radix.v2/redis"
)

func main() {
	aragorn := Card{NickName: "Aragorn", Greetings: "Well Met!", Price: 9, Attack: 10, Defense: 12, Owner: "Luktar"}
	AddCard(aragorn, "card:45")
	card := GetCard("card:45")
	card.ToString()
}

func AddCard(card Card, id string) {
	conn, err := redis.Dial("tcp", "localhost:6379")
	if err != nil {
		fmt.Println(err.Error())
	} else {
		defer conn.Close()

		response := conn.Cmd("HMSET", id, "nickName", card.NickName, "greetings", card.Greetings, "price", card.Price, "attack", card.Attack, "defense", card.Defense, "owner", card.Owner)
		if response.Err != nil {
			fmt.Println(response.Err)
		}
		fmt.Println(response.String())
	}
}

func GetCard(id string) *Card {
	card := new(Card)
	conn, err := redis.Dial("tcp", "localhost:6379")
	if err != nil {
		fmt.Println(err.Error())
	} else {
		defer conn.Close()
		response, _ := conn.Cmd("HGETALL", id).Map()
		card.NickName = response["nickName"]
		card.Greetings = response["greetings"]
		card.Owner = response["owner"]
		card.Attack, _ = strconv.Atoi(response["attack"])
		card.Price, _ = strconv.Atoi(response["price"])
		card.Defense, _ = strconv.Atoi(response["defense"])
	}
	return card
}

func (card *Card) ToString() {
	fmt.Printf("Nickname:%s\n", card.NickName)
	fmt.Printf("Greetings:%s\n", card.Greetings)
	fmt.Printf("Owner:%s\n", card.Owner)
	fmt.Printf("Price:%d\n", card.Price)
	fmt.Printf("Attack:%d\n", card.Price)
	fmt.Printf("Defense:%d\n", card.Defense)
}

type Card struct {
	NickName  string
	Greetings string
	Price     int
	Attack    int
	Defense   int
	Owner     string
}
```

İlk olarak Card isimli bir struct tasarladığımızı söyleyelim. İçerisinde Redis'teki Hash içeriğine karşılık gelen alanları barındırmakta. AddCard fonksiyonu parametre olarak gelen bir Card nesnesinin içeriğini kullanarak Redis üzerinde yeni bir Hash oluşturma işini üstleniyor. Fonksiyonun bir önceki örnekteki ekleme operasyonundan tek farkı değerleri almak için parametre olarak gelen Card örneğini kullanılması. card:45 benzeri key değeri için de id isimli bir parametre kullanmaktayız. GetCard metodu id bilgisine göre Redis üzerinden bir Card içeriğini çekmek üzere tasarlanmış durumda. Cmd üzerinden gidilen Map fonksiyonu ile redis tarafında tutulan içeriği almaktayız. Gelen içerikteki değerler string içerikte olacaktır. Bu nedenle strconv paketinden gerekli dönüştürme operasyonlarını kullanmamız gerekebilir. Card tipinin Price, Attack ve Defense gibi alanları int tipinden olduğu için Atoi tür dönüştürme metodundan yararlandık. Bulunan içeriğe göre değerleri atanan Card nesnesi olarak geriye döndürüyoruz. Card yapısına uygulanan ToString metodu ile de içeriği ekrana bastırıyoruz. İşte örnek çalışma zamanı çıktısı.

![goredis_5.gif](images/goredis_5.gif)

Pek tabii olmayan bir key değerini almaya çalışırsak içeriği boş bir yapı örneği elde ederiz. Söz gelimi card:100 sistemimizde bulunmuyor. Bu anahtar için program çıktısı aşağıdaki gibi olacaktır.

![goredis_6.gif](images/goredis_6.gif)

Demek ki

Teorimiz oldukça basit. Redis komutlarını çalıştırmak için paketin Cmd fonksiyonundan yararlanılabilir. Basit bir string kullanımından hash, list, set, sortedSet gibi veri yapılarına kadar gidilebilir. Dolayısıyla temel CRUD operasyonlarını basitçe ele alabiliriz. Bu noktada Redis'in komutlarını incelemekte ve detaylı bir şekilde öğrenmekte yarar olduğu kanısındayım. Go ile olan entegrasyonda işleri kolaylaştırmak ve programatik alanda veri türlerini tip bazında ifade etmek için yapılardan (struct) ve bu yapılara uygulanan yardımcı metodlardan yararlanılabilir. Yazımızda geçen örneği daha da geliştirmek elinizde. Söz gelimi Go'nun web programlama kabiliyetlerini baz alarak MVC (Model View Controller) yapısına uygun bir programı Redis ile çalışacak şekilde tasarlayabilirsiniz. Ya da arayüzle uğraşmak istemiyorsanız bir REST servis geliştirip temel veri operasyonlarının bu servis üzerinden gerçekleştirilmesini deneyebilirsiniz. Denemeye değer. Ben şimdilik dinlenmeye çekileceğim ama konuyu buradan alıp ileriye taşımak sizin elinizde. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
