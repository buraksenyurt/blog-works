---
layout: post
title: "GoLang - Interface Kavramı"
date: 2017-05-07 10:57:00 +0300
categories:
  - golang
tags:
  - golang
  - dotnet
  - go
  - python
  - ruby
  - pointers
  - github
---
Geçenlerde bilgisayarımın başında oturmuş sıkılmakla meşguldüm. Her ne kadar bloğumu zinde tutmaya çalışsam da arada sırada böyle durağan dönemlere de denk geliyorum. Küçük tatiller diyelim. Derken enteresan bir Tweet yakaladım. Apollo 11'in Command ve Lunar modüllerine ait Assembler kodları github üzerinden yayına açılmış. [Şu adresten bakabilirsiniz](https://github.com/chrislgarry/Apollo-11).

![margaret_hamilton.gif](/assets/images/2017/margaret_hamilton.gif)

Yandaki fotoğrafta yer alan kadınsa Apollo 11 programının yazılım mühendisliği direktörü Margaret Hamilton. Yanında durduğu print çıktısınsa sözü geçen Assembler kodlarına ait olduğu ifade ediliyor. MIT talebeleri bu print çıktılarını üşenmeyip github'da bakılabilir hale getirmişler. Konu hakkında detaylı bir makaleye de [şu adresten ulaşabilirsiniz](https://qz.com/726338/the-code-that-took-america-to-the-moon-was-just-published-to-github-and-its-like-a-1960s-time-capsule/).

Yazıyı okuduktan ve Üniversitede ders olarak gördüğüm ve orada bıraktığım Assembler kodlarına baktıktan sonra gözünü seveyim senin C#, Ruby, Go ve Python dedim. Sonrasında bu senenin planında yer alan Gopher olma çalışmalarıma devam edeyim dedim. Haydi başlayalım.

Interface metod şablonlarının koleksiyonunu tutmak için kullanılan bir veri türüdür. Burada iki önemli nokta vardır. Birincisi gövdesi olmayan metod tanımlamalarını içermesi, ikincisi ise kendisinin bir veri türü olmasıdır. Normalde.Net tarafındaki interface kavramını düşündüğümüzde nesne yönelimli programlama dillerinin temel özelliklerinden olan çok biçimlilik (Polymorphysm) ve kalıtımı (Inheritance) destekleyen bir tip olarak kullanıldığını görürüz. Bu açıdan bakıldığında bir arayüz içerisinde onu uygulayan diğer tiplerin sahip olması gereken özelliklerin ve yapması gereken aksiyonların tanımlanması söz konusudur. Ancak Go, nesne yönelimli bir dil değildir ve interface onun tip sisteminin önemli bir karakteristiğini yansıtmaktadır: Bir tipin hangi verilere sahip olması gerektiğinden ziyade hangi aksiyonları icra etmesi gerektiğinin soyutlanması. Bu yazımızda interface tipinin kullanımını basitçe ele almaya çalışacağız.

Hello World

Aşağıdaki kod parçası ile işe başlayalım.

```cpp
package main

import (
	"fmt"
	)
	
func main(){
	actors:=[]Actor{Tank{"T-80",100},Player{"Gun Ball"}}
	for _,a:=range actors{
		a.SaySomething("hello")
		a.Move("left")
	}
}

type Actor interface{
	Move(direction string)
	SaySomething(speach string)
}

type Tank struct{
	model string
	power int
}
func(t Tank) SaySomething(s string){
	fmt.Printf("'%s' says : %s\n",t.model,s)
}
func(t Tank) Move(d string){
	fmt.Printf("'%s' move to %s\n",t.model,d)
}

type Player struct{
	name string
}
func(p Player) Move(d string){
	fmt.Printf("'%s' move to %s\n",p.name,d)
}
func(p Player) SaySomething(s string){
	fmt.Printf("'%s' says %s\n",p.name,s)
}
```

Kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![interfaces_2.gif](/assets/images/2017/interfaces_2.gif)

Örnekte bir oyun sahnesindeki çeşitli aktörleri tanımlayan iki struct ve bir interface tipi yer almaktadır. Actor tipinde iki fonksiyon tanımına yer veriyoruz. Tank ve Player struct'larında kendilerine özgü bir kaç alan bulunuyor. Dikkat edilmesi gereken nokta bu iki struct için Actor interface'in de belirtilen metodların yazılmış olması. Sözdizimi olarak metodların ilk parametreleri uygulanacakları tipe ait. for döngüsü ile actors isimli slice elemanlarında dolaşıyor ve her biri için Move ile SaySomething metodlarını çağırıyoruz. Aslında kullanılan slice içerisindeki elemanlarda dolaşırken Go çalışma zamanı motoru interface{} değişkenine dönüştürme işlemini otomatik olarak gerçekleştirmekte. Fakat bu durum biraz sonra göreceğimiz vakka da biraz daha ilginçleşecek.

> Aslında interface kullanımında Duck Typing söz konusudur. Nasıl bir şey olduğunu öğrenmek isterseniz [şu yazıya göz](/2017/01/19/duck-typing-nedir/)atabilirsiniz.

interface{} Tipi

Go dilinde hiç bir fonksiyon tanımı bulundurmayan interface isimli bir tip de mevcuttur. Bu tip fonksiyon parametresi olarak kullanılabilir. Böyle bir durumda fonksiyona herhangibir tip atanabilir. Go çalışma zamanı burada bir dönüştürme işlemi gerçekleştirir. Gelen değişken interface tipine dönüştürülür. Şimdi aşağıdaki gibi bir kod yazdığımızı düşünelim.

```cpp
func main(){
	actors:=[]Actor{Tank{"T-80",100},Player{"Gun Ball"}}
	DoIt(actors)
}

func DoIt(objects []interface{}){	
	for _,obj:=range objects{		
		obj.Move("Forward")
		obj.SaySomething("kuniciva")		
    }	
}
```

Burada actors isimli slice içeriğini DoIt fonksiyonuna interface dizisi olarak geçiyoruz. DoIt fonksiyonunda tüm nesneleri dolaşıyor ve Move ile SaySomething metodlarını sırasıyla çağırıyoruz. Bir önceki kodda yer alan for döngüsü çalıştığına göre bu fonksiyonun da çalışması gerekiyor. Oysaki Go çalışma zamanı bizi dönüştürme işleminin yapılamadığı konusunda uyaracak.

![interfaces_4.gif](/assets/images/2017/interfaces_4.gif)

Şimdi kodun doğru halini yazalım.

```cpp
func main(){
	actors:=[]Actor{Tank{"T-80",100},Player{"Gun Ball"}}
	values:=[]interface{}{actors[0],actors[1]}
	DoIt(values)
}

func DoIt(objects []interface{}){	
	for _,obj:=range objects{
		if act, ok := obj.(Actor); ok {
			act.Move("Forward")
			act.SaySomething("kuniciva")
		}
    }	
}
```

Öncelikle values isimli bir slice tanımı var ve interface{} tipinden oluşacağını belirtiyoruz. Elemanları ise actors'den geliyor. Bu sayede DoIt fonksiyonuna actors içeriğini interface{} tipi olarak atayabiliriz. for döngüsü içerisinde obj.(Actor) şeklinde bir çağrım var. Bu çağrım o anki interface{} tipinin bir Actor olup olmadığını kontrol etmekte. Eğer ok cevabını alırsak Move ve SaySomething metodlarını çağırabiliriz. İşte çalışma zamanı çıktısı.

![interfaces_3.gif](/assets/images/2017/interfaces_3.gif)

Şimdi örneğimizi biraz daha geliştirelim. Actor tiplerinin yanına örneğin int tipinden bir değişken daha koyalım ve onun için geliştireceğimiz bir metodu kullanmaya çalışalım.

```cpp
package main

import (
	"fmt"
	)
	
func main(){
	actors:=[]Actor{Tank{"T-80",100},Player{"Gun Ball"}}
	no:=Number(100)
	utl:=Utility(no)	
	values:=[]interface{}{actors[0],actors[1],utl}
	DoIt(values)
}

func DoIt(objects []interface{}){	
	for _,obj:=range objects{		
		switch t := obj.(type) {
			case Actor:
				t.Move("Forward")
				t.SaySomething("kuniciva")
			case Number:
				fmt.Println(t.IsEven())
		}		
    }	
}
type Utility interface{
	IsEven() bool
}
type Number int32

func(n Number) IsEven() bool{
	if int(n)%2==0{
		return true
	}else{
		return false
	}
}
```

IsEven metodu geriye bool değer döndüren bir metoddur ve Utility interface tipi içerisinde şablon olarak tanımlanmıştır. Bu metodu int32'den inşa edilen Number isimli yeni bir tipe uygulamaktayız. Yaptığı tek şey sayının çift olup olmadığını true veya false olarak döndürmek. DoIt fonksiyonunun kullanımı sırasında utl isimli bir değişkenin eklendiği gözden kaçmamalıdır. Aslında burada Number tipinin Utility interface{} tipine dönüştürülmesi söz konusudur. Sonrasında DoIt fonksiyonuna parametre olarak geçilir. DoIt fonksiyonu içerisinde bu kez bir switch bloğuna yer verilmiştir. switch bloğunda yaptığımız şey tipe bakıp akışı yönlendirmekten ibaret. Gelen tip bir Actor ise Move ve SaySomething metodları çağırılır. Gelen tip Number ise de IsEven. Kodun çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![interfaces_5.gif](/assets/images/2017/interfaces_5.gif)

interface veri tipi esasında iki parçadan oluşur. Parçalardan biri veriyi tutan değişkeni işaret eden bir pointer barındırır. Diğer parçaysa interface'in bu ilişkili tip üzerinden çağırabileceği fonksiyon bilgisini barındıran veri tablosunu işaret eder.

Pointer Kullanımı

interface içinde tanımlı metodları ilgili tipler için yazarken parametrelerinde Pointer da kullanabiliriz. İlk yazdığımız örneği göz önüne alırsak metod parametrelerinde Tank ve Player tipleri için * operatörü ile bu struct'lara ait nesne örneklerini interface tipine dönüştürdüğümüz yerde & operatörünü kullanmamız yeterlidir ('ın & kullanımı halinde zorunlu olmadığını da biraz sonra göreceğiz) İlk örnek kodumuzu aşağıdaki hale getirerek ilerleyelim.

```cpp
package main

import (
	"fmt"
	)
	
func main(){
	actors:=[]Actor{&Tank{"T-80",100},&Player{"Gun Ball"}}
	for _,a:=range actors{
		a.SaySomething("hello")
		a.Move("left")
	}
}

type Actor interface{
	Move(direction string)
	SaySomething(speach string)
}

type Tank struct{
	model string
	power int
}
func(t *Tank) SaySomething(s string){
	fmt.Printf("'%s' says : %s\n",t.model,s)
}
func(t *Tank) Move(d string){
	fmt.Printf("'%s' move to %s\n",t.model,d)
}

type Player struct{
	name string
}
func(p *Player) Move(d string){
	fmt.Printf("'%s' move to %s\n",p.name,d)
}
func(p *Player) SaySomething(s string){
	fmt.Printf("'%s' says %s\n",p.name,s)
}
```

Move ve SaySomething metodlarında *Tank ve * Player şeklinde Pointer kabul eden parametreler kullanıyoruz. Ayrıca actors isimli slice içerisindeki atamalarda & operatöründe yararlandık. Bu sayede adres bilgisini metodlara göndermiş oluyoruz. Çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![gointerfaces_6.gif](/assets/images/2017/gointerfaces_6.gif)

Eğer & operatörünü kullanmazsak çalışma zamanında hata mesajı alırız.

![interfaces_7.gif](/assets/images/2017/interfaces_7.gif)

Bu son derece doğaldır nitekim Move ve SaySomething metodları birer Pointer beklemektedir. Ancak biz değer göndermeye çalışıyoruz. Şimdi de tam tersi durumu ele alalım. Yani metod şablonlarında Pointer kullanımından vazgeçip sadece slice içerisinde & ile adres ataması gerçekleştirelim. Çalışma zamanında hata oluşmayacak ve kod başarılı bir şekilde çalışacaktır. Bunun sebebi Pointer tipinin ilişkilendirildiği tipin üyelerine (bu örnekte Move ve SaySomething metodlar) erişebilmesidir.

![interfaces_8.gif](/assets/images/2017/interfaces_8.gif)

Bu son iki örnekteki farkları anlamak önemlidir. Go dilinde varsayılan olarak fonksiyon parametreleri veri kopyalama yöntemi ile kullanılırlar. Yani çağrım yapılan yerden gönderilen parametre verisi, fonksiyon içinde kullanılmak için kopyalanır. Bu nedenle Pointer parametre alan fonksiyona değer türü şeklinde atama yaptığımızda hata alırız. Çünkü beklenen Tank veya Player tipinden bir değişken adresidir. Diğer yandan Pointer tipinden parametre almayan fonksiyona & operatörü ile veri gönderdiğimizde adres kopyalaması söz konusu olacağından, interface tipinin tanımlı üyelerine (Move ve SaySomething) erişebiliriz.

Görüldüğü gibi interface kavramı basit görünen ama detayına inildikçe dikkat edilmesi gereken özellikler taşıyan bir kavramdır. Go dili ile ilgili kavramları çalıştıktça paylaşmaya devam edeceğim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
