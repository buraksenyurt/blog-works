---
layout: post
title: "Gopher Olma Çalışmaları"
date: 2017-01-13 12:10:00 +0300
categories:
  - golang
tags:
  - golang
  - csharp
  - http
  - ruby
  - pointers
---
Geçtiğimiz sene [Ruby](https://www.buraksenyurt.com/category/Ruby.aspx) diliye uğraşmaya başlamıştım. Ruby dilini sevenler ve ona gönül verenlere Rubyist deniyor. Benzer bir yaklaşım meğer Go tarafında da varmış. Onlarda kendilerine logolarına esin kaynağı olan Gopher diyorlarmış. Aslında Go dilinin logosu gerçekten bir canlıdan esinlenilerek tasarlanmış. Yaklaşık 15 ila 20 cm boylarında olan gopher'lar oldukça sevimliler (Bana göre) Elbette logo çok daha sevimli. Bu arada Gopher aynı zamanda TCP/IP tabanlı HTTP öncesi bir internet protokolu olarak da geçiyor. Detaylara [Wikipedia adresinden](https://en.wikipedia.org/wiki/Gopher_(protocol)) bakabilirsiniz.

![gopher.gif](/assets/images/2017/gopher.gif)

![20170109_224829.gif](/assets/images/2017/20170109_224829.gif)

Benim 2017 hedeflerim arasında Go dilini en azından orta seviyeye kadar öğrenmek var. Oldukça uzun bir sprint olacak ancak geçtiğimiz hafta kendimle yaptığım Sprint planlama toplantısında onu To Do listesine aldım. Bu hafta itibariyle de durumunu In Progress'e çektim.

Gopher olabilir miyim bilemiyorum ama bir dili çok iyi seviyede öğrenmeden o dil hakkında ahkam kesmemek gerektiğine inanıyorum. Geçtiğimiz zaman içerisinde Go ile ilgili dil özelliklerini öğrenmeye devam ettim. Öğrendiklerimi not almaya başladım. Kısa bir Hello World uygulamasından sonra başka temel kavramları da incelemeye koyuldum. İşte en son baktığım konular

## Arrays

Belli tipteki elemanaları (herhangibir Go veri tipi olabilir) bir arada tutan (sunan) koleksiyonlara dizi diyebiliriz. Pek çok programlama dilinde olduğu gibi Go'da da kullanılan çekirdek veri yapılarından (data structures) birisidir. Diziler sabit uzunlukta tanımlanırlar. Yani içereceği eleman sayısı baştan bildirilir (Ya da dizinin tanımlandığı satırda atama işlemi yapıldığında uzunluğu belirlenir). Tabii Go söz konusu olunca ileride de göreceğimiz gibi bellek adresleri de önem kazanacak. Diziler için de birbirini takip eden bellek adresleri söz konusu. İlk eleman (bu arada diziler 0ncı indisten başlamakta) dizinin başladığı son eleman da bittiği bellek adresinde konumlanmakta. Go çok boyutlu (Multi Dimensional) dizi yapısına da sahip ve fonksiyonlara dizileri parametre olarak geçirebiliyoruz. Aşağıdaki basit kod parçasında hem diziler ile ilgili temel işlemeler yer alıyor hem de basit for döngülerine yer veriliyor.

{% raw %}

```cpp
package main

import (
	"fmt"
	"time"
	)

func main(){
	// Arrays
	fmt.Printf("Today : %s\n\n",time.Now())
	
	var points=[]float32{10.45,-30.345,55.90,60.0123}
	var names=[4]string{"sarlok","sumi","varlord","khan"}
	var numbers [7]int
	var matrix=[3][3]int{{1,2,3},{4,5,6},{7,8,9}}
	
	for i:=0;i<len(points);i++{
		fmt.Printf("%d is %f\n",i,points[i])
	}
	
	for i:=0;i<len(names);i++{
		fmt.Printf("%s\n",names[i])
	}
	
	var j int
	for j=0;j<len(numbers);j++{
		numbers[j]=j*j
		fmt.Printf("%d\t",numbers[j])
	}
	
	fmt.Printf("\nSum = %f\n",sum(points))
	
	for i:=0;i<3;i++{
		for j:=0;j<3;j++{
			fmt.Printf("%d\t",matrix[i][j])
		}
		fmt.Println("")
	}
}

func sum(nmbrs []float32) float32{
	var toplam float32=0
	for i:=0;i<len(nmbrs);i++{
		toplam+=nmbrs[i]
	}
	return toplam
}

```

{% endraw %}

Çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![gofnd_2.gif](/assets/images/2017/gofnd_2.gif)

Kodda neler olduğuna kısaca bakalım.

Sırf meraktan günün tarihini ve zamanı ekrana nasıl yazdırırım diye Google'a sordum ve bunu main fonksiyonunun ilk satırındaki ifadede denedim. Tabii kullanabilmek için time paketini import etmemiz gerektiği gözünüzden kaçmamış olsa gerek. Array veri yapısı ile çok alakası olmasa da kodu renklendirdiğini düşünüyorum. En azından biraz daha yeşillendirdi.

Örnekte points, numbers, names ve matrix isimli dört dizi kullanılmakta. points dizisi tanımlanırken eleman sayısını belirtmedik ancak süslü parantezler arasına 4 adet float32 tipinden değişken koyduk. Yani eleman sayısı belli. names dizisinde eleman sayısını da veriyoruz. string tipinden elemanlar içeren bir dizi. 7 eleman içeren numbers dizisi sadece tanımlanmış durumda. Elemanlarını bir döngü yardımıyla doldururken her bir kutuya o anki indisin karesini yerleştiriyoruz. matrix isimli dizimiz aslında çok boyutlu kullanıma basit bir örnek. İki boyutlu dizinin elemanları int tipinden. matrix içeriğini Console ekranımıza olabildiğince jan janlı yazdırmaya gayret ettik.

Döngü kullanımları oldukça basit. matrix dizisini işlerken içiçe döngü kullanıyoruz. Döngülere konu olan dizilerin eleman sayısını len fonksiyonu yardımıyla anlayabiliriz. Dizi indisleri 0 tabanlı başladığından döngü sayaçları da 0dan başlatılmakta. fmt paketindeki Printf fonksiyonu Console penceresine çeşitli formatları uygulamak için kullanılıyor. %d'yi tam sayılar, %f'i kayan noktalı sayılar, %s'i de string tipindeki elemanlar için yer tutucu olarak kullanmaktayız. Pek tabii C#taki gibi {0} {1} gibi bir kullanım burada söz konusu değil. Printf'te ki parametre sırası konumlandırma açısından önemli. \n ve \t bildiğiniz üzere escape karakterlerimiz. Yeni satıra geçmek ve tab bırakmak için kullanılıyorlar. Başka escape karakterleri de var elbette.

Karakter
Kullanım Amacı

\a
alert

\b
backspace

\f
Form feed

\r
Carriage return

\v
Vertical tab

\xhh
Hexadecimal numbers

\ooo
Octal numbers

\\
\

\'
'

\"
"

\??

Kod parçasında dikkat çekici noktalardan birisi de sum isimli fonksiyon. Parametre olarak float32 tipinden elemanlar içeren bir dizi almakta. Dizinin eleman sayısı belli değil (ki eleman sayısı belli olacak şekilde verebilirsiniz de) Fonksiyon gelen dizinin boyutuna bakarak elemanların toplamını bulmakta.

Kodu kurcalamak farklı farklı şeyler yapmaya çalışmak tamamen sizin elinizde. Örneğin matrisler için aritmetik işlemler yapan fonksiyonları tanımlayarak işe başlayabilirsiniz. Hatta eleman sayısı belli olmayan boyutlu dizileri de işin içerisine katabilirsiniz. Diğer yandan fonskiyona parametre olarak gönderdiğiniz bir dizinin elemanlarında yapacağınız değişikliğin orjinal konumdaki dizi elemanlarını etkileyip etkilemediğini de inceleyebilirsiniz. Etkilemiyorsa ve etkilemesini isterseniz ne yapmanız gerektiğinizi de bir araştırın derim.

## Fonksiyonlar

Aslında önceki yazımızda olsun bu yazımızda olsun main haricinde kendi yazdığımız bir kaç fonksiyona yer verdik. Tabii fonksiyonlar ile ilişkili başka şeyler de var. Go, fonksiyon çeşitliliği açısından zengin bir dil. Bu anlamda fonksiyonel programlama paradigmasını da desteklediğini ifade edebiliriz. Gopher olmak için bu kullanım şekillerini bilmek çok önemli diye düşünüyorum. Öğrenebildiğim bir kaç tanesini paylaşayım.

### Birden Fazla Değer Döndürmek

Bir fonksiyondan n sayıda değer döndürmemiz mümkün. Aşağıdaki kod parçasında bu durumu inceliyoruz.

```cpp
package main

import (
	"fmt"
	)

func main(){
	var a,b,c,d int
	var x,y int
	x=8
	y=2
	
	a,b,c,d=calc(x,y)
	
	fmt.Printf("%d+%d=%d\n",x,y,a)
	fmt.Printf("%d*%d=%d\n",x,y,b)
	fmt.Printf("%d/%d=%d\n",x,y,c)
	fmt.Printf("%d-%d=%d\n",x,y,d)
}

func calc(x,y int) (int,int,int,int){
	return x+y,x*y,x/y,x-y
}
```

Örneğin çalışma zamanı çıktısı aşağıdaki gibidir.

![gofnd_3.gif](/assets/images/2017/gofnd_3.gif)

calc isimli fonksiyon 4 değer döndürecek şekilde tanımlanmıştır. return ile dikkat edileceği üzere fonksiyona parametre olarak gelen x ve y değerleri için yapılan dört işlem sonuçları döndürülmektedir. main fonksiyonunda calc çağrısının yapıldığı satırda sonuçlar dört farklı değişkene tek ifade ile atanmaktadır. Aslında bu kullanım şekli Rubyist'lere oldukça tanıdık gelecektir. Bilindiği gibi Ruby'de de n sayıda değer döndürmek ve tek satırda atama yapmak aynıdır (Fonksiyonlardan dönecek olan değerleri blok içerisinde adlanrırarak kullanmamız da mümkün)

### params'ın Bir Türevi Variadic Fonksiyonlar

C#çılar bir metoda değişken sayıda parametre göndermenin yollarından birisinin params kullanımı olduğunu bilirler. Go dilinde de bu işlevsellik var. Hatta bu tip fonksiyonlar Variadic olarak ifade ediliyor. fmt paketindeki Println bu tip fonksiyonlara verilebilecek ilk örneklerden birisi. Aşağıdaki kod parçasında da geliştirici tanımlı bir Variadic fonksiyon örneği yer alıyor.

```cpp
package main

import "fmt"

func main(){
	fmt.Println(sum(1,2,3,4))
	fmt.Println(sum(4,6,77,-2,90,2))
	fmt.Println(sum(0))
}

func sum(numbers ...int)int{
	total:=0
	for _,n:=range numbers{
		total+=n
	}
	return total
}
```

![gofnd_4.gif](/assets/images/2017/gofnd_4.gif)

sum isimli fonksiyon herhangibir sayıda int eleman alacak şekilde tanımlanmıştır. Buradaki... kullanımının anlamı budur diyebiliriz. Fonksiyon içerisindeki for döngüsü mutlaka dikkatinizi çekmiştir. range ile numbers elemanlarında hareket etme kabiliyeti kazanılır. numbers'a ait her eleman döngü içerisinde n adıyla kullanılır. Fonksiyonun kullanımında farklı sayıda int tipinden değişken gönderilmiştir (bir nevi foreach yazdığımızı düşünebiliriz sanırım)

### Metod Kavramı

Gopher olmaya çalışırken metod ile fonksiyon'un Go dilinde aynı anlamda kullanılmadığını fark ettim. Yılların C# programıcısı olarak parametre alıp geriye değer döndüren fonksiyonları metod olarak isimlendirdiğim çok oldu. Hatta Visual Basic'te metod ve procedure ayrımlarına da şahit oldum. Ancak fonksiyon ve metod arasında bir ayrım olabileceği pek aklıma gelmemişti. Peki o zaman Go dilinde metod neye denir bir bakalım.

```cpp
package main

import "fmt"

type Vehicle struct{
	id int
	name string
	x,y,z int
}

func(v Vehicle) findLocation() string{
	if v.x>10 && v.x<20 {
		return "Germany"
		}
	return "France"
}

func main(){
	tank:=Vehicle{id:1,name:"Leopard",x:12,y:1,z:-100}
	fmt.Printf("%s\n",tank.findLocation())
	tank.x=5
	fmt.Printf("%s\n",tank.findLocation())
}
```

### ![gofnd_5.gif](/assets/images/2017/gofnd_5.gif)

Örnekte Vehicle isimli bir struct tanımlı. Bu veri tipine yazının ilerleyen kısımlarında değineceğiz. Vehicle içerisinde id,name,x,y,z gibi alanlar mevcut. findLocation ise bir metod (fonkisyon olarak isimlendirmiyoruz) Tanımlanma şekli normal bir fonksiyondan biraz farklı. func'dan sonra Vehicle isimli bir parametre geliyor. Sonrasında ise metodumuzun adı. Metod içerisnde v isimli değişkeni kullanarak Vehicle örneklerinin niteliklerine ulaşabiliyoruz. Aslında metod ile fonksiyon arasındaki fark kim tarafından sahiplenildiği ile anlaşılabiliyor. Metodu örnekte olduğu gibi bir veri tipine bağladık. Bu yüzden çağırılırken Vehicle tipinden bir değişken üzerinden gidebiliyoruz. Eğer findLocation metodunu main içerisinde herhangibir noktada çağırmaya kalkarsak böyle bir metodun olmadığına dair hata mesajı alırız.

![gofnd_6.gif](/assets/images/2017/gofnd_6.gif)

### Değişken Olarak Fonksiyon Kullanımı ve İsimsiz Fonksiyonlar

Go dilinde bir fonksiyonu değişkene atayabilir ve hatta bu değişkeni bir başka fonksiyona parametre olarak gönderebiliriz. Daha çok fonksiyon alan fonksiyonlarda işimize yarayabilecek bir durum olduğunu ifade edebiliriz. Go'nun hazır paketlerinde bu şekilde çalışan pek çok fonksiyon bulunur. Aşağıdaki kod parçası durumu daha iyi anlamamızı sağlayacaktır.

```cpp
package main

import (
	"fmt"
	"strings"
)

func main(){	
	f1 := func(r rune) rune {
		switch {
			case r == ' ':
				return '_'
			case r == 'b':
				return 'B'
			}
		return r
	}
	
	fmt.Println(strings.Map(f1, "bugun guzel bir gun"))
	fmt.Println(strings.Map(func(r rune) rune{
		if r>=65 && r <= 90{
			return r + 32
			}
		return r
	},"buGUN de Guzel gAlIba"))
}
```

![gofnd_10.gif](/assets/images/2017/gofnd_10.gif)

Kodda efso şeyler var aslında. strings paketinde yer alan Map fonksiyonunun tanımı ile işe başlamak lazım.

```cpp
func Map(mapping func(rune) rune, s string) string
```

Önemli olan nokta mapping isimli ilk parametre. Aslında burada rune (Unicode değeri gösteren bir int32 tipi olarak ifade ediliyor) tipinden parametre alıp yine rune tipinden değer döndüren bir fonksiyon bildirimi var. Üstelik bu bildirim Map fonksiyonunun ilk parametresi. Dolayısıyla Map fonksiyonuna belirtilen kurala uygun bir fonksiyonu parametre olarak gönderebiliriz.

f1 isimli değişken bu tanıma uyuyor. Dikkat edileceği üzere rune tipinden parametre alıp yine aynı tipten değer döndürmekte. İçerisinde ise r değişkeninin değeri kontrol edilip bir sonuç üretiliyor. Basitçe boşluk karakterini _ işaretine ve b harflerini de B'ye dönüştürmekte. Anlamlı bir iş yaptığı söylenemez. f1 değişkeni kodun ilerleyen kısımlarında strings paketi üzerinden kullanılıyor.

İkinci strings.Map kullanımı ise biraz daha enteresan. Bu kez Map fonksiyonunun ihtiyaç duyduğu ilk parametredeki fonksiyonu oradaki ifade içerisinde tanımlamaktayız. Bu arada Map benzeri bir fonksiyon yazmak istersek fonksiyonları tip olarak tanımlayıp ele alabileceğimizi de bilmemiz gerekiyor. Örneğimizde kullanıdığımız fonksiyonları aynı zamanda isimsiz fonksiyonlar olarak da düşünebiliriz.

> GO dili daha önce belirttiğimiz üzere fonksiyonel dil özelliklerine de sahip. İsimsiz fonksiyonlar, fonksiyonların değişkenlere atanabilmesi, tip olarak fonksiyon tanımlanabilmesi, fonksiyonlardan fonksiyon döndürülebilmesi bu kabiliyetler arasında yer alıyor. Bunları da ilerleyen zamanlarda makale veya tek fotoluk ipuçlarında ele almaya çalışacağım.

Aslında yeri gelmişken bir fonksiyondan başka bir fonksiyon nasıl döndürülür ve hatta bu fonksiyon içeride isimsiz olarak tanımlandığında Closure adı verilen kapama işlevselliği nasıl vuku bulur, dilerseniz inceleyelim. Aşağıdaki kod parçasını göz önüne alabiliriz.

```cpp
package main

import ( 
	"fmt"
	)

func add() func(int) int {
    total:=0
	return func(x int) int{
		total+=x
        return total
    }
}

func main(){
    var f1 = add()
    fmt.Println(f1(5))
	fmt.Println(f1(5))
	fmt.Println(f1(5))
	
    var f2 = add()
    fmt.Println(f2(4))
	fmt.Println(f2(4))
}
```

![gofnd_11.gif](/assets/images/2017/gofnd_11.gif)

Kodun yaptığı şeyin hiç bir anlamı yok biliyorsunuz değil mi? Ama Go'nun fonksiyonel kabiliyetleri ile ilgili önemli bilgiler barındırıyor. Her şeyden önce add isimli fonksiyona odaklanalım. add geriye isimsiz bir fonksiyon döndürmekte. Bu fonksiyon int tipinden tek bir parametre alıyor ve yine int tipinden değer döndürüyor. İsimsiz fonksiyon içerisindeyse add fonksiyonunun yerel değişkenine gelen parametre değeri ekleniyor.

main fonksiyonu içerisinde f1 ve f2 isimli iki değişken tanımı var. Bu değişkenler add fonksiyonunu taşıyorlar. Dolayısıyla onları kodun ilerleyen kısımlarında kullanırken aslında birer fonksiyon çağrısı gerçekleştiriyoruz. f1 ve f2'ye verilen parametre değerleri add fonksiyonundan dönen isimsiz fonksiyona gidiyorlar. f1 ve f2 kendi alanları içerisinde bu fonksiyonelliği sunuyorlar. Yani total değişkeni her add fonksiyonu atamasında, atanan değişken için 0 olarak yeniden belirleniyor.

Kafalar yandı mı? Şahsen benim epey yanmış durumda. Ha bir de fonksiyonları tip gibi tanımlama mevzusu vardı. Az önce dile getirmiştim ama arada devreler yandığı için bu cümleyi yazarken dile getirdiğimi de unuttum. Neyse. Bunu ilerleyen zamanlarda incelersek fena olmaz.

## Structure

Metod kavramını incelerken basit bir struct tipi kullandık. Orada çok fazla değinmedik ama Go dilinin önemli veri türlerinden birisi olarak karşımıza çıkıyor. Structure, kullanıcı tanımlı veri tiplerinden (user defined data type) birisi olarak düşünülebilir. Hani nesne yönelimi bir dil değil belki ama en azından kendi sınıflarımızı struct gibi tanımlayabiliriz düşüncesi güzel. Aslında çeşitli tipte elemanları barındıracak bir veri modelini tasarlayıp değişken olarak kullanıma sunuyoruz. Aşağıdaki örnek kod parçasında basit bir struct tanımı ve kullanımı söz konusu.

```cpp
package main

import "fmt"

func main(){

	phone:=Product{productId:1001,title:"Samsung J5",listPrice:245.50}
	var cpu Product
	cpu.productId=2005
	cpu.title="intel core i5 CPU"
	cpu.listPrice=120.50
	
	var products=[]Product{phone,cpu}
	
	writeToConsole(products)
}

func writeToConsole(prods []Product){
	for _,p:=range prods{
		fmt.Printf("(%d)-%s,%f\n",p.productId,p.title,p.listPrice)
	}
}

type Product struct{
	productId int
	title string
	listPrice float32
}
```

![gofnd_7.gif](/assets/images/2017/gofnd_7.gif)

Örnekte Product isimli bir yapı kullanılmakta. Yapının productId, title, listPrice isimli üyeleri mevcut. main fonksiyonu içerisinde iki struct örneği yer alıyor. Birbirlerinden farklı şekilde oluşturulduklarına dikkat etmişsinizdir. Her iki yapıyı yine Product tipinden olan bir dizide topladık. Bu diziyi writeToConsole fonksiyonuna parametre olarak da gönderiyoruz. Fonksiyon, gelen Product yapılarına ait değerleri ekrana basmakla görevli.

### Pointer Demişken

Yukarıda geliştirdiğimiz örneği baz alarak konuyu biraz değiştirelim. Önce aşağıdaki kod parçası ve sonucunu irdelememiz gerekiyor.

```cpp
package main

import "fmt"

func main(){

	phone:=Product{productId:1001,title:"Samsung J5",listPrice:245.50}
	fmt.Println(phone.listPrice)
	discount(phone,10)
	fmt.Println(phone.listPrice)
}

func discount(p Product,value float32){
	p.listPrice-=value
}

type Product struct{
	productId int
	title string
	listPrice float32
}
```

![gofnd_8.gif](/assets/images/2017/gofnd_8.gif)

discount fonksiyonu ile parametre olarak gelen ürünün liste fiyatını belli bir değerde azaltıyoruz. Fonksiyona phone isimli struct örneğini gönderiyoruz ve içerisinde listPrice değerini değiştiriyoruz. Ekran çıktısına baktığımızda fonksiyon çağrısından önceki liste fiyatı ile sonraki liste fiyatının aynı olduğunu görmekteyiz. Bu zaten beklediğimiz bir sonuç. Nitekim phone değişkeni, discount fonksiyonuna geçerken sahip olduğu değerleri ile birlikte kopyalanıyor ve blok içinde p isimli yeni bir değişken olarak muamele görüyor. Dolayısıyla fonksiyon içerisindeki değişikliker main içerisindeki değişkeni etkilemiyor. Peki etkilemesini istersek!? Yani phone değişkeninin liste fiyatını fonksiyon içerisinde değiştirebilmek istersek. İşte burada ilgili nesneyi fonksiyona referans olarak geçirmenin bir yolunu bulmamız gerekmekte. Bunun için onun bellek adresini taşımayı düşünebiliriz. Sadece iki karakter ile bu işi çözümleyebiliriz.

```cpp
package main

import "fmt"

func main(){

	phone:=Product{productId:1001,title:"Samsung J5",listPrice:245.50}
	fmt.Println(phone.listPrice)
	discount(&phone,10)
	fmt.Println(phone.listPrice)
}

func discount(p *Product,value float32){
	fmt.Println("Address is ",&p)
	p.listPrice-=value
}

type Product struct{
	productId int
	title string
	listPrice float32
}
```

![gofnd_9.gif](/assets/images/2017/gofnd_9.gif)

Dikkat edileceği üzere phone değişkeninin fonksiyon çağrısı öncesindeki fiyatı, fonksiyon çağrısı sonrası değişmiştir. Bunun sebebi fonksiyonda bir Pointer tanımlamış olmamızdır. *Product ile tanımladığımız değişkene, &phone ile phone isimli değişkenin bellek adresini taşımız oluruz. Dolayısıyla fonksiyon içerisindeki p değişkeni üzerinde yapacağımız değişiklikler aslında phone isimli değişken için geçerli olur. Kodda &p kullanımı ile gelen bellek adresini de yazdığımızı fark etmişsinizdir. Pointer kavramı oldukça derin bir konu. En sooooooonnnn 1995de üniversite ikinci sınıftayken C++ sınavına hazırlanırken bakmıştım. Dolayısıyla yeniden öğrendiğimi ifade edebilirim.

Yazdığımız kod aynı zamanda bir fonksiyona parametreleri referansları ile aktırmanın da örneğidir (Call by Reference konusu) Normalde fonksiyonlara iki tür parametre geçişi söz konusu. Call by Value ve Call by Reference şeklinde anılıyorlar. Go dilinde fonksiyonlar varsayılan olarak Call by Value yaklaşımını kullanıyor. Referans taşımaları için az önceki örnekte olduğu gibi Pointer'lardan yararlanabiliyoruz. Bir dizi söz konusu olduğunda ise Slice öneriliyor (Bu da benim için yeni bir kavram. İlerleyen zamanlarda öğreneceğim)

Şimdilik Gopher olma çalışmalarım ile ilgili aktaracaklarım bu kadar sevgili arkadaşlar. Bu yazıda dizilere, döngülere, yapılara, fonksiyonlara ve metodlara değinme fırsatımız oldu. Ucundan bir kuple de olsa Pointer dedik. Epeyce yorulduk. Şimdi kısa bir ara verme zamanı. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
