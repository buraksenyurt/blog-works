---
layout: post
title: "GoLang - Kalıtımsız Bir Dünya"
date: 2017-02-25 21:38:00 +0300
categories:
  - golang
tags:
  - golang
  - inheritance
  - composition
  - struct
  - structure
  - pointer
  - interface
  - gopher
---
Go hızlı ve performanslı bir programlama dili olarak anılıyor. Diğer yandan nesne yönelimli dil özelliklerini büyük ölçüde içermediği gibi bir gerçek de var ortada. Tasarımı sırasında kalıtım (Inheritance) gibi yönetimin sonradan zorlaşabildiği ve bakım gerektiren çatıların performansı olumsuz yönde etkilediği düşüncesi hakim olmuş. Bu nedenle nesne yönelimli düşünce tarzını az da olsa kenara bırakarak ilerlemek gerekiyor. Kalıtım doğrudan desteklenmese de nesne kompozisyonu (Composition) mevcut. Hatta belli bir ölçüde çok biçimliliği (Polymorphism) de uygulayabiliriz gibi.

![gocomp_2.gif](/assets/images/2017/gocomp_2.gif)

Doğruyu söylemek gerekirse yıllarca nesne yönelimli dillerle çalışmış birisinin tekrardan okula dönüp alışık olduğu alan yapısı ve kurallar dizisinin dışına çıkması pek kolay değil. Bunu sancısını Go dilini öğrenmeye çalıştığım şu günlerde yoğun olarak yaşamaktayım. Gelin kalıtımı ve çok biçimliliği tam olarak karşılamasa da Go dilinde benzer kurguların nasıl yapılabileceğine bir bakalım. İşe aşağıdaki örnek kod parçasını geliştirerek başlayabiliriz.

## Composition

```cpp
package main

// Inheritance ve Composition'a baslangic kodu
import (
	"fmt"
	)

func main(){
	buentap:=new(Gorlog)
	buentap.nick="buentap"
	buentap.level=4857
	buentap.color="red"
	writeGorlog(buentap)

	zulu:=new(Molag)
	zulu.nick="zulurak"
	zulu.level=3450
	writeMolag(zulu)
}

type Player struct{
	nick string
	level int
}

// Gorlog'lar insan irkindan gelir. Ten renkleri vardir.
type Gorlog struct{
	Player
	color string
}

func writeGorlog(g *Gorlog){
	fmt.Printf("%s - %d('%s')\n",g.nick,g.level,g.color)
}

// Molag'lar renksiz ruhlardir
type Molag struct{
	Player
}

func writeMolag(m *Molag){
	fmt.Printf("%s - %d\n",m.nick,m.level)
}
```

Kodun çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![gocomp_3.gif](/assets/images/2017/gocomp_3.gif)

Şuraya da çakma bir Object Composition çizelgesi koyalım. Player tipi esas itibariyle Gorlog ve Molag'ın birer parçasıdır. Nesneleri bu şekilde birleştirdiğimizi düşünebiliriz.

![composition.gif](/assets/images/2017/composition.gif)

Kod içerisinde üç structure tanımlandığını görmektesiniz. Player tipinde nick ve level isimli üyeler mevcut. Diğer yandan Gorlog ve Molag olarak adlandırdığımız orta dünya tipleri içerisinde Player isimli bir tanımlama yer alıyor. Bir başka deyişle bu tipler aslında birer Player olarak düşünülebilirler. Nitekim bir Player'ın üyelerini barındıracak şekilde tasarlandılar. Farklılık olması açısından insan ırkından gelen Gorlog'ların renkleri de var (color isimli üye)

main fonksiyonunda üretilen nesne örneklerinin üretim şekilleri haricinde noktadan sonra Player yapısında gelen üyelerine değerler atadığımıza dikkat edelim. Yani bir Gorlog nesne örneği veya Molag nesne örneği üzerinden Player üyelerine erişebiliyoruz. Alın size kalıtımsal bir yaklaşım. Her tür aslında birer Player olarak düşünülebilir diyebilir miyiz?

Her iki tür için ekrana bilgi yazan birer fonksiyon da mevcut. writeGorlog, Gorlog tipinden, writeMolag ise Molag tipinden bir pointer değişkenini parametre olarak kullanıyor. Böylece ilgili fonksiyonlardan Player yapısında o nesne örnekleri için tanımlı nick ve level değerlerine ulaşabiliyoruz.

## Interface Kullanımı

Gelelim bir diğer noktaya. Diyelim ki Gorlog ve Molag türleri için ortak bir takım işlevesellikler söz konusu. Örneğin hareket ve ateş etme kabiliyetleri olduğunu varsayalım. Normal şartlarda nesne yönelimli bir dil ile geliştirme yapıyor olsak kuvvetle muhtemel bir interface tanımlar ve ilgili tipleri bu interface tipinden türeterek üyeleri ezmeye zorlardık. Ancak bu kadar nesne odaklı bir dünyada değiliz. Yine de elimizde bir takım çözümler var. İlk olarak Go dilinde de interface tipi olduğunu belirtelim. Bu tip içerisinde tanımlanacak fonksiyonları, uygulamasını istediğimiz yapılar için yazabilir ve az da olsa çok biçimlilik sunan fonksiyonellikler sağlayabiliriz. Yukarıdaki örneğimize aşağıdaki değişikliler ile devam edelim.

```cpp
package main
// Inheritance ve Composition'a baslangic kodu
import (
	"fmt"
	)
	
func main(){
	buentap:=new(Gorlog)
	buentap.nick="buentap"
	buentap.level=4857
	buentap.color="red"	
	writeGorlog(buentap)
	
	zulu:=new(Molag)
	zulu.nick="zulurak"
	zulu.level=3450
	writeMolag(zulu)
	
	moveAndFire(buentap,"korusant","tatuyin")
	moveAndFire(zulu,"tatuyin","korusant")
}
func moveAndFire(p IPlayer,moveL string,fireL string){
	p.move(moveL)
	p.fire(fireL)
}
type IPlayer interface{
	move(location string) bool
	fire(location string) bool
}

type Player struct{
	nick string
	level int
}

// Gorlog'lar insan irkindan gelir. Ten renkleri vardir.
type Gorlog struct{
	Player	
	color string
}
func (g Gorlog) move(location string) bool{
	fmt.Printf("%s:Move to %s\n",g.nick,location)
	return true
}
func (g Gorlog) fire(location string) bool{
	fmt.Printf("%s:Fire to the %s\n",g.nick,location)
	return true
}
func writeGorlog(g *Gorlog){
	fmt.Printf("%s - %d('%s')\n",g.nick,g.level,g.color)
}

// Molag'lar renksiz ruhlardir
type Molag struct{
	Player		
}
func (m Molag) move(location string) bool{
	fmt.Printf("%s:Move to %s\n",m.nick,location)
	return true
}
func (m Molag) fire(location string) bool{
	fmt.Printf("%s:Fire to the %s\n",m.nick,location)
	return true
}
func writeMolag(m *Molag){
	fmt.Printf("%s - %d\n",m.nick,m.level)
}
```

![gocomp4.gif](/assets/images/2017/gocomp4.gif)

Eski alışkanlık olsa gerek IPlayer şeklinde isimlendirdiğimiz bir interface tipi mevcut. Bu tip içerisinde move ve fire isimli iki fonksiyon yer alıyor. Biz bu fonksiyonellikleri hangi yapılara kazandırmak istersek onlara uygulamalıyız. Uygulama şekli aslında aynı isimli fonksiyonları uygulanacak tip için yazmak. move ve fire fonksiyonlarının tanımlanış şekillerinde ilk parantezler arasında uygulanacağı yapıyı belirtiyoruz. Böylece ilgili nesne örneklerinin Player içerisinde tanımlı üylelerine ve kendi özel niteliklerine erişebiliriz. Fonksiyonlar için önemli olan nokta girdi ve çıktı için kullanılan parametre yapılarının IPlayer arayüzünde belirtildiği şekilde olması.

Peki nerede nesne yönelimliye yakınlaşıyoruz? Go'nun kod yazım kuralları haricinde dikkat etmemiz gereken nokta MoveAndFire isimli fonksiyon. İlk parametre IPlayer tipinden (Bir ışık yandı mı sevgili C#çı, Javacı arkadaşım) Gorlog ve Molag isimli yapılar için tanımlanan move ve fire fonksiyonları, IPlayer tarafından tanımlanan şablona uygun olacak şekilde yazıldılar. Dolayısıyla IPlayer'a atanan tipin içerisinde bu fonksiyonların aynı desende yazılmış olması (tabii ilk parantez içlerine dikkat) yeterli. main fonksiyonunda yaptığımız moveAndFire çağırımlarında bu fonksiyona hangi nesne örneğini gönderdiysek o nesne örneğine ait fire ve move fonksiyonlarının çalıştırılması söz konusu.

Görüldüğü gibi Composition kuramını ve interface tipini kullanarak belli ölçüde nesne yönelimlilik sağladık. moveAndFire fonksiyonunun bir çeşit çok biçimlilik sağladığını da gördük. Bakalım Go dili ile ilgili olarak ilerleyen zamanlarda daha neler neler göreceğiz? Yazıyı hazırlarken [goder isimli blog'dan](http://www.goder.co/sample-golang-ile-inheritance-ve-interface-kullanimi/) oldukça faydalandığımı da ifade etmek isterim. Ufkumu açan çok değerli bir Türkçe kaynak. Gopher olmak isteyenlerin takip etmesini şiddetle tavsiye derim. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
