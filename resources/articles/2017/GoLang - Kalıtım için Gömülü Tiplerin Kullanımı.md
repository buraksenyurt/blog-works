---
title: "GoLang - Kalıtım için Gömülü Tiplerin Kullanımı"
pubDate: 2017-06-04 09:25:00
categories:
  - GoLang
tags:
  - GoLang
  - .NET
  - Go
  - Pointers
  - GitHub
---

# GoLang - Kalıtım için Gömülü Tiplerin Kullanımı
![zidane.gif](images/zidane.gif)

Merhaba Arkadaşlar,

Bir süredir GO dili ile ilgili çalışmalarıma ara vermiştim. Yakın zamanda ise sevgili Murat Özalp'in "GO Programlama" isimli kitabını takip etmeye başladım. Gerçekten her bölüm son derece doyurucu. Kitabı düzenli olarak hergün çalışıyorum. Bazen çok az zaman ayırsam da her gece bir kaç sayfasını okuyor ve uygulamaya çalışıyorum. Burada yaptığım günlük çalışmaları aksatmamaya gayret ediyor ve kendime göre hazırladığım örnekleri [github üzerinde](https://github.com/buraksenyurt/golangsamples) topluyorum. Hatta artık kodları yeni bir IDE üzerinde deniyorum. Yeni gözdem [LiteIDE isimli kod editörü](https://sourceforge.net/projects/liteide/) (ki Murat Hoca'nın tavsiyesidir ve çok memnun kaldığım bir geliştirme aracıdır)

Son olarak bugün gömülü tiplerin kullanımını öğrenmeye çalıştım. Bildiğiniz gibi GO dili tam anlamıyla nesne yönelimli (Object Oriented) bir dil değil. Her şeyden önce sınıf (class) gibi bir kavram olmayışı kafaları biraz da olsa karıştırıyor. Çalışmalarım sırasında struct ve interface tiplerini kullanarak GO dilinde kalıtımın nasıl ele alındığını bir ölçüde kavramıştım. Diğer yandan pek çok dilde karmaşıklığı arttırdığı için izin verilmeyen çoklu kalıtımı biraz daha güvenli bir şekilde sağlama şansımız var. Gömülü tipler (ya da struct içinde kullanılan struct tipinden değişkenler) bu noktada devreye giriyor. Aslında bir nevi Composition işlevselliğini sağlayarak bu özelliği kazanıyoruz. Nasıl mı? Kendi anladığım kadarı ile durum şöyle...

Öncelikle basit bir senaryo düşünmeye çalıştım. Futbolcu, basketbolcu, boksör ve benzeri oyuncu türlerinin ortak özelliklerini barındıracak bir yapı tasarlamaya karar verdim. Sonrasında bu oyuncuların çeşitli yeteneklerini ifade edecek bir tip daha tasarladım. Her oyuncunun takma adı, sistem için önem arz edecek bir numarası ve söyleyeceği bir şeyleri olsun istedim (bunu bir metod ile halledebilirdim. Metod derken fonksiyon değil struct ile ilişkilendirilen metod) Ayrıca oyuncuların türlerine göre farklı kabiliyetleri de olabilirdi. Tabii tüm kabiliyetleri kendisini tanımlayan bir isimden ibaret olacak şekilde basitçe ele almam benim için daha iyi olacaktı. Bu kabiliyetleri uygulayacağım bir metod da pek şık olurdu. Paragraf ile ifade etmeye çalıştığım şeyi aslında aşağıdaki grafik ile daha güzel anlatabilirim belki de.

![golng_et_1.gif](images/golng_et_1.gif)

Player ve Ability isimli yapılar FootballPlayer ve Boxer isimli diğer yapılarda gömülü tip olarak kullanılıyorlar. Buna göre her futbolcu ve boksör örneği id, nickName gibi temel bilgilere sahip olacak ve bir şeyler söyleyebilecek (saySomething metodu). Ayrıca her birisinin n sayıda kabiliyeti de bulunabilecek ve bu kabiliyetleri uygulayabilecek (useAbility metodu) Bunun için abilities niteliklerini kullanabiliriz. Gelelim bu fotoğrafın kod görüntüsüne.

```cpp
/*
 Lesson 09
 Embedded type kullanımı
 gömülü türlerden yararlanarak çoklu türetme özelliğini kullanabiliriz
*/
package main

import (
	"fmt"
)

func main() {
	var zidane FootballPlayer
	zidane.self = Player{id: 10, nickName: "Zinadine Zidane"}
	zidane.position = "Midfield"
	zidane.abilities = []Ability{
		Ability{name: "shoot", power: 92},
		Ability{name: "high pass", power: 84},
	}
	zidane.abilities[1].useAbility()
	zidane.self.saySomething("What can I do sometimes. This is football.")
	zidane.abilities[0].useAbility()

	var tayson Boxer
	tayson.self = Player{id: 88, nickName: "Bulldog"}
	tayson.knockdownCount = 32
	tayson.abilities = []Ability{
		Ability{name: "defense", power: 76}, //virgül koymayınca derleme hatası verir ;)
	}
	tayson.self.saySomething("I will win this game")
	tayson.abilities[0].useAbility()
}

// oyuncuların ortak niteliklerini barındıran bir struct
type Player struct {
	id       int
	nickName string
}

// player yapısına monte edilmiş saySomething metodu
// oyuncunun bir şeyler söylemesi için kullanılabilecek bir metod
func (player *Player) saySomething(message string) {
	fmt.Printf("%s says that '%s'\n", player.nickName, message)
}

// oyuncuların farklı yeteneklerini tanımlayacak olan Ability isimli yapı
type Ability struct {
	name  string
	power int
}

// Ability yapısına monte edilmiş olan useAbility isimli bir metod
// oyuncunun bir yeteneğini kullandırmak için
func (ability *Ability) useAbility() {
	fmt.Printf("[%s] yeteneği kullanılıyor. Güç %d\n", ability.name, ability.power)
}

// Player ve Ability yapılarını gömülü tip olarak kullanan ve
// futbolcuları tanımlayan yapı
type FootballPlayer struct {
	position  string
	self      Player
	abilities []Ability
}

// farklı bir oyuncu tipi
type Boxer struct {
	knockdownCount int
	self           Player
	abilities      []Ability
}
```

main fonksiyonunda zidane (makale fotoğrafının sebebini de özetlemiş olduk) ve tayson isimli iki değişken kullanılmakta. zidane isimli değişken FootballPlayer yapısı tipinden. tayson ise Boxer tipinden. Kodun akışında her birisinin adını, numarasını belirliyor, bir şeyler söylemelerini sağlıyor ve farklı kabiliyetler ekleyerek bunları uygulayışlarını izliyoruz. Dikkat edilmesi gereken ve kendime söylediğim bir kaç nokta da var. Oyuncuların kabiliyetlerini tutan abilities isimli nitelikleri Ability türünden bir Slice olarak tanımladık. Bir oyuncunun belli bir yeteneğini uygulamak için ilgili Slice öğesine gitmeli ve sonrasında useAbility metodunu çağırmalıyız. Metodlar hatırlanacağı üzere yapılara fonksiyonellik kazandırmak üzere kullanılıyorlar. useAbility ve saySomething isimli metodlar sırasıyla Ability ve Player yapıları ile ilişkilendirilmiş durumdalar. Yazımları sırasında metod adından önceki parantezlerde hangi struct için kullanılacakları belirtilmekte. * işaretine yani pointer kullanıldığına dikkat de edilmeli.

Kodu çalıştırdığımızda aşağıdaki ekran görüntüsündekine benzer bir sonuçla karşılaşmamız gerekir.

![golng_et_2.gif](images/golng_et_2.gif)

Gömülü tipleri kullandığımız bu örnek kod parçasında bir yapının başka yapıları kullanarak çoklu kalıtımı nasıl ele alabileceğini incelemeye çalıştık. Anahtar nokta struct tiplerini içerme şeklinde değerlendirmekten ibaret. Felsefe olarak, genişletmek istediğimiz türün içereceği nitelikleri yapılar içerisinde toplamak gerekiyor. Eğer gömülü tip üzerinden uygulanması beklenen ortak fonksiyonellikler de söz konusu ise bunların metod şeklinde tanımlanması türeyen tip için yeterli. Böylece geldik kısa bir GO turumuzun daha sonuna. Bir başka makalemizde görüşünceye dek hepinize mutlu günler dilerim.
