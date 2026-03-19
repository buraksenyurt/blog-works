---
title: "GoLang - Web Programlamaya Giriş"
pubDate: 2017-05-27 19:03:00
categories:
  - GoLang
tags:
  - GoLang
  - HTTP
  - Go
  - Python
  - Ruby
  - Concurrency
  - Performance
  - GitHub
---

# GoLang - Web Programlamaya Giriş
![goweb_3.gif](images/goweb_3.gif)

Merhaba Arkadaşlar,

Bir web uygulamasının temel malzemeleri nelerdir? Sunucu tarafında çalışan güçlü bir çatı (Framework), içeriklerin gösterildiği statik veya dinamik web sayfaları, iyi tasarım, görsel zenginlik katan materyaller (resimler,css'ler vb), veri depolama enstrümanları ve diğerleri. Aslında internet programcılığının ilk yılları düşünüldüğünde basit HTML sayfalarının neredeyse her tür ihtiyacı karşılayacağı düşünülüyordu belkide. Zaman geçtikçe programlama dillerinin dinamik web sayfaları ile olan etkileşimi, istemci taraflı çalışan betiklerin sunucu taraflı kullanılabilmesi de gündeme geldi. Modern programlama dillerinin neredeyse tamamı web uygulamaları geliştirebilmek için gerekli temel donanıma sahip. Asıl amacı back-end tarafındaki büyük ölçekli sistemlerde yüksek performans sunmak olan, eş zamanlı programlamada öne çıkan GO ile de web tabanlı uygulamalar geliştirebilmemiz mümkün. Go dilinin network haberleşme üzerine sunduğu basitlik ve yüksek performans da göz önüne alındığında web programlama oldukça ilgi çekici bir konu haline geliyor.

Ruby tarafında Ruby on Rails, Python tarafında Django gibi çatılar profesyonel anlamda web uygulamalarının geliştirilmesini kolaylaştırıyor. Go tarafında da bu tip çatılar mevcut hatta Beego bunlar arasında en popülerlerinden birisi. Ne var ki basit düzeyde de dahili paketlerden yararlanarak web uygulamaları geliştirebiliriz. Çünkü teori her zaman basittir. tcp/ip üzerinden belli bir porta gelen çeşitli talepler ve bu taleplere karşılık istemcilerin yorumlayabileceği HTML içerikler. Bu düşünce yapısından yola çıkarak net/http ve html/template kütüphanelerini kullanarak iki sayfadan oluşan basit bir web sitesi yazacağız. İşe ilk olarak aşağıdaki klasör ve dosya yapısını tasarlayarak başlayabiliriz.

- server.go (main fonksiyonunu içeren go kod dosyamız)
- \Pages
- \Pages\index.html (ana sayfamız olarak düşünebiliriz)
- \Pages\players.html (Oyuncu listesini gösterecek olan sayfamız)
- \Pages\common.css (biraz görsellik katmak için kullanacağız)
- \Pages\stronger.gif (siz istediğiniz bir resmi kullanabilirsiniz)

> Bu arada çeşitli kitaplardan ve kaynaklardan çalışırken bilgisayarımın başında yazdığım örnekleri [github üzerinde](https://github.com/buraksenyurt/golangsamples) toplamaya çalışıyorum. Ders01,02,03... mantığında ilerleyen bir içerik söz konusu. İlk dersle başlayarak Go kod pratiklerinizi geliştirebilir dili orta seviyeye kadar öğrenebilirsiniz.

Kök dizinimizde main fonksiyonunu içeren go kod dosyamız bulunuyor. Bu tahmin edileceği üzere uygulamanın giriş noktasını içeren dosya. Pages alt klasöründeyse html sayfalarımızı ve css, gif gibi kaynaklarımızı barındırıyoruz. common.css tahmin edeceğiniz üzere sayfalarımızdaki görselliği daha keyifli hale getirmek için kullanacağımız bir stil (cascading style sheets) dosyası. Ben içeriğini aşağıdaki gibi yazarak body,table,table-th (başlık),table-td (hücre) düzenini renklendirmeye çalıştım.

Amatörce Bir CSS Dosyası

```text
body{
	border:3px solid cyan;
	border-radius: 24px;
	border-width: 20px;
	text-align: center;
	width: 400px;
	height: 400px;
	margin: auto;
	font-family:Tahoma, sans-serif;
	font-size:16px;
	color:541460;
}
table.gridtable {
	font-family: verdana,arial,sans-serif;
	font-size:16px;
	color:#581845;
	border-width: 1px;
	border-color: #666666;
	border-collapse: collapse;
}
table.gridtable th {
	border-width: 3px;
	padding: 8px;
	border-style: solid;
	border-color: #666666;
	background-color: #FF80E8;
}
table.gridtable td {
	border-width: 3px;
	padding: 8px;
	border-style: solid;
	border-color: #666666;
	background-color: #98A1FC;
}
```

index.html web uygulamasının giriş sayfası. Aslında kod içerisinde belirleyeceğimiz kök adrese gelen taleplerin doğrudan karşılanacağı sayfa olarak düşünülebilir. CSS uygulamak dışında bir kaç bilgi ve /players şeklinde bir adrese bağlantı içermekte.

Karşılama Sayfası index.html

```text
<html>
<head>
	<title>Blizert World Game Entertienment</title>
	<link rel="stylesheet" href="/common.css">
</head>
<body>
	<h1>Wellcome to BliZerT</h1>
	<p>The World's best card players are here. <br/>
	Want to learn some more details? <br/> 
	What are you waiting for. Go Go Go...</p>
	<img src="/stronger.gif"/>
	<p><a href="/players">Famous Hearthstone Players</a></p>
</body>
</html>
```

Kullanıcılar örneğin http://localhost:8085 gibi bir adrese geldiklerinde bu sayfanın sunulmasını arzu ediyoruz. Eğer /players yönlendirmesini yapan linke basılırsa da players.html isimli sayfaya yönlendirme yapacağız. Bu sayfanın içeriği ise aşağıdaki gibidir.

Oyuncuları gösteren Players.html sayfası

```text
<html>
<head>
	<title>Gold Players</title>
	<link rel="stylesheet" href="/common.css">
</head>
<body>
	<h1>Gold Players</h1>
	<table class="gridtable" align="center">
		<tr>
			<th>Id</th>
			<th>Player No</th>
			<th>Nickname</th>
			<th>Level</th>
		</tr>
		{{ range $index,$player := . }}
			<tr>
				<td>{{$index}}</td>
				<td>{{$player.Id}}</td>
				<td>{{$player.Nickname}}</td>
				<td>{{$player.Level}}</td>
			</tr>
		{{end}}
	</table>
	<br/>
	<a href="/">Go Back</a>
</body>
```

Hımmmm...Bir dakika...Burada oldukça enteresan ifadeler var. Bir back-end geliştiricisi de olsak az buçuk HTML'den çakmadığımızı kim söyleyebilir. Peki bu ikişer küme parantezleri de neyin nesi oluyor? range anahtar kelimesi bir yerlerden tanıdık geliyor aslında. Sözü fazla uzatmayalım. {{ ile }} arasında yazdığımız ifadeler Go diline ait kod satırlarını içeren kısımlar. Yani HTML sayfası içerisine GO kodlarını gömdüğümüzü ifade edebiliriz. Önemli olan kısımsa range ile başlayan satırlar. Burada nokta ile sayfaya gelen veri kaynağı üzerinde index ve nesne çiftleri olarak hareket ediyoruz ({{ }} arasında GO kodundan gelen bir değişkene ulaşmak istediğimizde adının başına nokta koymamız gerekiyor) Bir döngü söz konusu ama key:value çiftlerini dönüyor. Peki bu key:value çiftlerinin kaynağı kim? Döngü tarafından işlenecek olan veriyi server.go içerisinden göndereceğiz. Göndereceğimiz nesne topluluğundaki öğelerin de Id, Nickname ve Level isimli alanları olacak. Alanları HTML içerisine yedirmek için $index ve $player gibi ifadeler kullanıldığına dikkat edelim. Örneğin döngünün o anki nesnesine gelen Player içerisindeki Nickname alanına ulaşmak için $player.Nickname ifadesini, bu ifadenin sonucunu td takıları arasına yazmak için de {{ }} bloğunu kullanıyoruz. E öyleyse gelelim server.go içeriğine.

Başrol Oyuncusu Server.go

```cpp
package main

import (
	"html/template"
	"log"
	"net/http"
)

func main() {
	http.Handle("/", http.FileServer(http.Dir("pages")))
	http.HandleFunc("/players", getPlayers)
	log.Println("Web server is active at port 8045")
	http.ListenAndServe(":8045", nil)
}

func getPlayers(response http.ResponseWriter, request *http.Request) {
	log.Println("Get Request for Players")
	players := loadPlayers()
	log.Println("Players loaded")
	t, err := template.ParseFiles("pages/players.html")
	if err == nil {
		t.Execute(response, players)
	} else {
		log.Println(err.Error())
	}
}

type Player struct {
	Id       int
	Nickname string
	Level    int
}

func loadPlayers() []Player {
	return []Player{
		Player{1001, "Molfiryin", 2},
		Player{1002, "Gul'dan", 21},
		Player{1003, "Anduin", 12},
		Player{1004, "Lexar", 5},
		Player{1005, "Turol", 34},
	}
}
```

Dosyanın genel yapısına baktığımızda Player tipinden bir struct içerdiğini, main dışında loadPlayers ve getPlayers isimli iki fonksiyona daha sahip olduğunu görüyoruz. Bunun dışında html/template, log ve net/http paketlerinin kullanıldığını görüyoruz. localhost üzerinden 8045 nolu adrese gelecek varsayılan talepler doğrudan pages klasörüne yönlendirilmekte. Bu yönlendirmeyi main fonksiyonunun ilk satırındaki HandlFunc ile belirtiyoruz. Genelde tüm web sunucuları varsayılan sayfaları bilirler. Index.html aranan ilk sayfalardan birisidir. Haliyle index.html göreve hazır olacağından bu kök adres çağrısının sonucu aşağıdaki ekran görüntüsündeki gibi bir olacaktır.

![goweb_1.gif](images/goweb_1.gif)

Famous Hearthstone Players başlıklı linke tıklandığında ise main fonksiyonundaki http.HandleFunc bildirimlerinden ikincisi devreye girer. Nitekim bağlantı /players şeklinde bir çağrı yapmaktadır. Bu adrese gelecek olan talepler getPlayers isimli fonksiyona yönlendirilmektedir. Bu fonksiyon iki kritik parametre alır. İstemciye gönderilecek cevabın yazılmasında kullanılacak olan ResponseWriter ve gelen talebe ait bilgiler içeren Request (Örnekte Request nesnesi kullanılmamıştır ancak [şu adresteki yazıdan](https://www.buraksenyurt.com/post/go-ile-basit-http-web-server-yapimi) kendisi hakkında ek bilgi edinebilirsiniz) getPlayers fonksiyonunda önce terminale log basılarak işlemlere başlanır. Arından Player isimli struct tipinden nesne örnekleri barındıran slice içeriğini elde edeceğimiz loadPlayers fonksiyonu tetiklenir. İşte bu slice içerisindeki Player nesne örnekleri, players.html sayfasındaki ilgili konumlara basılacaktır. Ama nasıl?

Önce template tipinin ParseFiles fonksiyonu ile pages klasöründeki players.html şablonu yakalanır. ParseFiles pek çok Go fonksiyonu gibi iki değer döndürür. İlki kullanacağımız Template, ikincisi de Error tipidir. Eğer bir hata yoksa t isimli Template tipi üzerinden Execute fonksiyonu çalıştırılır. Exceute fonksiyonu ResponseWriter örneğini ve players içeriğini alıp players.html üzerinde gerekli işlemleri gerçekleştirir. Aslında players.html sayfası belleğe alınıp, {{ }} konumları players dizisi içeriği ile beslenip gerekli HTML çıktısının üretilmesi ve bu içeriğin ilgili ResponseWriter'ın sahiplendiği stream üzerinden ağa yazdırılması söz konusudur diyebiliriz. Sonuç aşağıdaki ekran görüntüsündeki gibi olacaktır.

![goweb_2.gif](images/goweb_2.gif)

Ta ta ta taaa...Pek çok deneyimli ya da amatör web tasarımcısı için berbat dizaynlar olsa da benim için önemli bir adım diyebilirim. Bu basit örnekte hem statik hem de dinamik web sayfalarını nasıl kullanabileceğimizi incelemeye çalıştık. Statik HTML içeriklerinde iyi bir CSS bilgisi basit bir tasarım çok çok işe yarayabilir. Bunun dışında dinamik olarak bir şeyler sunacak web sayfalarında Template kullanarak sunucu tarafında üretilen verilerin HTML içerisine basılması da mümkündür. Yani arayüz tarafı ile Go kodları etkileşime geçebilir. Go ile Web Programlama oldukça sade ve basit gibi görünüyor. Sevgili Murat Özalp'ın Go Programlama kitabı bu konuda bana büyük vizyon katıyor diyebilirim. Öğrendikçe bilgilerimin pekişmesi için de yazmaya devam edeceğim. Siz de örneği biraz daha geliştirmeye çalışabilirsiniz. Örneğin kullanıcıdan girdi alabileceğiniz bir web sayfası söz konusu olsa girilen içerikleri sunucuya post ettiğinizde bunları Go kodunda yakalayıp nasıl işleyebilirsiniz? Araştırın. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
