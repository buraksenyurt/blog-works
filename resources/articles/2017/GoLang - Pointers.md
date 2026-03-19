---
title: "GoLang - Pointers"
pubDate: 2017-03-31 21:15:00
categories:
  - GoLang
tags:
  - GoLang
  - Go
  - Pointers
---

# GoLang - Pointers
![Pointer_DePaulo_head.gif](images/Pointer_DePaulo_head.gif)

Merhaba Arkadaşlar,

1993 yılında girdiğim Matematik Mühendisliği bölümünün bilgisayar programlama ağırlıklı bir müfredatı vardı. GWBasic ile başlayan maceramızda Pascal, C, C++, Cobol gibi programlama dillerine uğradık. Sınavlarımız çoğunlukla kağıt üzerinde olurdu. Basit for döngülerini dahi kağıt üzerinde yazarak algoritma çözmeye çalışırdık. Haliyle C gibi case-sensitive kuralların katı olduğu dillerde sınavlar epey zorlu geçerdi.

Hatırladığım kadarı ile en çok zorlandığım ve hatta sonrasında kaçınmaya çalıştığım konuların başında Pointer aritmetiği geliyordu. Pointer tipini öğrenmek kolay olsa da iş aritmetiğine gelince kafam karışmıyor değildi. Zamanla yeni çıkan ve özellikle yönetimli ortamlara sahip olan dillerde bu gibi konulara pek fazla rastlayamaz olduk. Hatta C# tarafında pointer kullanımı mümkün olsa da Unsafe kod konusuna girdiği için bilinçli bir şekilde çekinerek yaklaştık.

Ne var ki Pointer kavramı halen geçerliliğini koruyor (Google'ladığınızda bu sevimli köpek türü de karşınıza çıkıyor) Google'ın daha çok kendisi için geliştirdiği, sistem seviyesinde yüksek performanslı işler yapmak için kullandığı Go dilinde de Pointer tipi mevcut. Tek fark aritmetiğinin olmaması diyebiliriz. Gelin bu kısa yazımızda Pointer tipini tanımaya çalışalım.

Pointer bir değişkenin bellek adresini gösterir. Gösterdiği değişkenin tipinde tanımlanır. Pointer denildiğinde * ve & operatörleri ile new fonksiyonu önem kazanır. & operatörü değişken adreslerini bulmakta kullanılır. * operatörü ile Pointer'ın işaret ettiği değer alınır (Bu ise de-refere yada deference olarak anılır) new, Go ile gelen built-in fonksiyonlardan birisidir. Parametre olarak bir tip alır ve bellekte bu tipin sığacağı kadar yer ayırıp adresini geriye döndürür. Dolayısıyla içereceği değeri sonradan belli olacak bir alanın bellekte tahsis edilmesi ve adresinin yakalanması mümkündür. Elbette akıllara gelen sorulardan birisi tahsis edilen bu bellek bölgesinin işi bitince temizlenmeye ihtiyacı olup olmayacağıdır. GO çalışma zamanında bir garbage collector mekanizması söz konusudur ve bunu düşünmemize gerek yoktur.

Merhaba Pointer

İlk olarak aşağıdaki basit bir kod parçası ile işe başlayalım.

```cpp
package main

import (
	"fmt"
	)
	
func main(){
	age:=22	
	pAge:=&age
	fmt.Println("Address of age",pAge)
	fmt.Println("Age(from pointer)",*pAge)
	*pAge=41
	fmt.Println("Age=",age)
	fmt.Println("Age(from pointer)",*pAge)

	point:=1000
	pPoint:=&point
	*pPoint=*pPoint*2
	fmt.Println("point=",point)
}
```

Olayı irdelemek için çalışma zamanı sonuçlarına bakalım.

![gopointer_1.gif](images/gopointer_1.gif)

pAge bir pointer oldu. Eşitliğin sağ tarafındaki & operatörü izleyen age değişkeninin bellek adresini yakalıyor. Bu nedenle pAge içeriğini ekrana bastığımızda bellek adresini görebiliyoruz. pAge isimli Pointer'ın işerat ettiği adres bölgesindeki değeri almak için * operatörü devreye giriyor. Sonrasında dikkat çekici bir durum söz konusu. * pAge değerinde bir değişiklik yapıp yaşa 41 sayısını atıyoruz. pAge bir pointer olduğundan üzerinden yapılan değişiklik aslında asıl işaret ettiği değişkenin değerinde gerçekleşmekte. Bu nedenle age değişkeni de artık 41 değerine sahip oluyor. point yine bir tamsayı değişkeni ve bellek adresini pPoint üzerinde taşıyoruz. * operatörü pointer'ın işaret ettiği değişken değerini verdiğinden matematik işlemlerine de dahip edebiliyoruz. * pPoint'in 2 ile çarpımında bu özellik ele alınıyor. Çok doğal olarak çarpma işlemi point değişkenini etkilemekte.

Fonksiyon Parametresi Olarak Pointer Kullanımı

İkinci örneğimizde ise pointer'ların fonksiyon parametrelerine olan etkisini inceleyeceğiz. Normalde fonksiyon parametreleri değer kopyalaması yöntemi ile kullanılırlar. Yani bir fonksiyona dışarıdan geçilen parametre değişkeni fonksiyon bloğu içerisinde kopyalanarak değerlendirirlir. Bu nedenle fonksiyon çağrımı yapılandan farklı bir değişken üzerinde işlemler yapılır. Pointer kullanıldığında ise fonksiyona geçen parametrenin adresi taşınır. Konuyu aşağıdaki örnek kod parçası ile daha iyi anlayabiliriz.

```cpp
package main

import(
	"fmt"
	)
	
func main(){
	age:=41
	fmt.Println("main-age = ",age)
	fmt.Println("main-address of age",&age)
	call(age)
	fmt.Println("main-age = ",age)
	callWithPointer(&age)
	fmt.Println("main-age = ",age)
}

func call(value int){
	value+=1
	fmt.Println("call-value = ",value)
	fmt.Println("call-address of value",&value)
}

func callWithPointer(value *int){
	*value+=1
	fmt.Println("callWithPointer-value = ",*value)
	fmt.Println("callWithPointer-address of value",value)
}
```

Olaylar main fonksiyonunda tanımlı age isimli değişken üzerinde gelişiyor. call parametre olarak değer bazlı değişken kullanmakta. Gelen age call fonksiyonunda değiştirilse bile main'deki age değerini etkilemiyor. callWithPointer içinse durum böyle değil. callWithPointer içerisine main fonksiyonundaki age değişkeninin adresi taşınıyor. Nitekim parametre bir pointer. Bu nedenle fonksiyon içerisinde yapılan değişim main içerisindeki age değişkeninde yapılmış oluyor. Konunun iyi anlaşılması için main'deki age ve fonksiyonlardaki parametre adreslerine dikkat etmenizi öneririm. Kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![gopointer_2.gif](images/gopointer_2.gif)

new ile Pointer Oluşturulması

Son olarak new fonksiyonunun kullanımına bir bakalım.

```cpp
package main

import (
	"fmt"
	)
	
func main(){
	nickP:=new(string)
	fmt.Println("Address ",nickP)
	fmt.Println("Current value",*nickP)
	*nickP="speedy gonzales"
	fmt.Printf("After assignment. '%s'\n",*nickP)
}
```

Örnekte new ile string tipinden nickP isimli bir Pointer tanımlanmıştır. Sadece bellekte bunun için yer ayrılmıştır. Bu nedenle atama öncesi güncel değeri yoktur. "Speedy Gonzales" atamasından sonra ise adres bölgesi veri ile doldurulmuştur. Kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![goPointer_3.gif](images/goPointer_3.gif)

Immutable Struct

Immutable kavramı pek çok dilde yer alır. Bir değişkenin durumunun değiştirilemez olduğu hali temsil eder. Go dilinde örneğin string değişkenler immutable tip olarak geçerler. Yani oluşturulduktan sonra içerikleri değiştirilemez. Struct tipi de immutable özellik taşımaktadır ancak fonksiyon parametresi olarak kullanıldıklarında durum değişir. Önceden de bahsettiğimiz gibi fonksiyonlara taşınan parametreler içeride kopyalanırlar. Bu yüzden struct tipinden bir değişkeni bir Go fonksiyonuna geçtiğimizde immutable özelliğini kaybeder. Aşağıdaki kod parçasını göz önüne alalım.

```cpp
package main

import (
	"fmt"
	)

func main(){
	goBook:=Book{Title:"Learning GO",Category:"Programming Languages",ListPrice:35.00}
	fmt.Printf("%s(%s),%f\n",goBook.Title,goBook.Category,newPrice(goBook))
	fmt.Printf("%f\n",goBook.ListPrice)
}

func newPrice(book Book) float32 {
	book.ListPrice+=10
	return book.ListPrice
}

type Book struct{
	Title,Category string
	ListPrice float32
}
```

Örnekte Book isimli bir struct kullanılıyor. Title, Category ve ListPrice isimli özellikleri var. newPrice fonksiyonu parametre olarak gelen bir kitabın liste fiyatını 10 birim arttırmakta. Dikkat edilmesi gereken nokta newPrice fonksiyonu içerisinde ListPrice değerinin değiştirilmesine rağmen myBook değişkenindeki liste fiyatının değerini korumuş olmasıdır (parametre değer türü olarak taşınmıştır)

![gopointer_4.gif](images/gopointer_4.gif)

Ancak tanımlanan struct değişkeni pointer olarak taşınırsa durum daha farklı olacaktır. * ve & operatörlerini devreye katarak kodu aşağıdaki hale getirelim.

```cpp
func main(){
	goBook:=Book{Title:"Learning GO",Category:"Programming Languages",ListPrice:35.00}
	fmt.Printf("%s(%s),%f\n",goBook.Title,goBook.Category,newPrice(&goBook))
	fmt.Printf("%f\n",goBook.ListPrice)
}

func newPrice(book *Book) float32 {
	book.ListPrice+=10
	return book.ListPrice
}
```

ve sonuç.

![gopointer_5.gif](images/gopointer_5.gif)

Görüldüğü gibi Pointer tipinin tanımlanması ve kullanımı oldukça basittir. Böylece geldik bir Gopher olma çalışmamızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
