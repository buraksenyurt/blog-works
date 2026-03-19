---
title: "GoLang - Concurrency (goroutine, channel)"
pubDate: 2017-05-15 11:52:00
categories:
  - GoLang
tags:
  - GoLang
  - .NET
  - HTTP
  - Go
  - Ruby
  - Threading
  - Concurrency
---

# GoLang - Concurrency (goroutine, channel)
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Yazılım ürünlerinde eş zamanlı çalışma modeli oldukça önemli. Uygulamalarda yer alan süreçler çoğu zaman alt iş parçalarından oluşmakta ve bu parçalar uygun koşullarda eş zamanlı olarak yürütülebilmekte. Eş zamanlılık için bir çok dilde destek mevcut. Go dili için de öne çıkan kavramlarından birisi aslında. Concurrency denildiğinde aklımıza bir sürece ait n sayıda görevin (Task) aynı anda çalışması gelmeli. Okuduğum kaynakta buna güzel bir örnek veriliyor: Web Sunucusu.

Bir web sunucusu istemcilerden gelen talepleri (Request) ait oldukları uygulamalara yönlendirip işleten bir çalışma mekaniğine sahiptir. Hiç bir talep için bir diğerini bekleme söz konusu değil. Web sunucusu bu görevleri eş zamanlı olarak yürütüyor. Concurrency'deki temel amaç da bu zaten. Görevleri aynı anda işletebilmek. Go dilinde goroutine fonksiyonu ve channel yöntemi ile Concurrency işlemlerini gerçekleştirebiliriz. İlk olarak goroutine fonksiyonunu inceleyecek sonrasında channel kavramına değineceğiz.

goroutine

goroutine bir fonksiyon aslında. Eş zamanlı çalışacak fonksiyonları çağırmak için kullanılıyor. Goroutine çalışma modeline göre belleğin stack adı verilen bölgesinde başlangıç için 2Kb kadar yer ayrıldığı ve bu alanın gerektiğinde büyüdüğü ifade ediliyor. Bir Thread için bu alan 1 Mb civarında. Thread oluşturma ve yönetmedeki pek çok karmaşık detay goroutine tasarımına dahil edilmemiş. Goroutine'ler işletim sistemi seviyesinde çoklu thread'de çalışabiliyor. Bu nedenle bir goroutine bloklansa bile diğeri (diğerleri) çalışmasına devam edebilir. Son olarak kullanım maliyeti düşük, hafif ve hızlı bir tasarıma sahip olduklarını belirtebiliriz (goroutine'lerin çalışma prensipleri ve Thread kavramı ile arasındaki farkları incelemek için [şu yazıya bakmanızı](http://blog.nindalf.com/how-goroutines-work/) şiddetle tavsiye ederim)

Şimdi basit bir örnekle konuyu anlamaya çalışalım.

```cpp
package main

import (
	"fmt"
	"time"
	"sort"
	"math/rand"
	)
	
func main(){
	names:=[]string{"captain kirk","barbara","nik","jon calloway","rici ric","sem vitmor"}
	go sort.Strings(names)
	
	go SaySomething("Hello Concurrency")
	
	//gorouting anonymous func sample
	go func(value int){
		fmt.Printf("%d part is going to go\n",value)
		time.Sleep(time.Second*2)
	}(1000)
	
	for i:=0;i<5;i++{
		start:=rand.Intn(250)
		go Calculate(start,start+1000,time.Second*1)
	}
	
	var userInput string
	fmt.Scanln(&userInput)
	fmt.Println("All is well")
}

func Calculate(start int,stop int,sleep time.Duration){
	for i:=start;i<=stop;i++{
		time.Sleep(sleep)
		fmt.Printf("%d...",i)
	}
}

func SaySomething(message string){
	fmt.Println("Saying...")
	time.Sleep(time.Second*3)
	fmt.Println(message)
}
```

Örnek kod parçasında goroutine'lerin farklı kullanımlarına ait birer örnek verilmiştir.

İlk kullanımda var olan bir Go fonksiyonunun (sort.Strings) eş zamanlı çalıştırılması örneklenmiştir. SaySomething fonksiyonu geliştirici tarafından yazılmıştır ve yine go komutu ile eş zamanlı yürütülür. Üçüncü kullanımda anonymous fonksiyon söz konusudur. Fonksiyon tanımından sonra parantez içerisinde verilen 1000 rakımı, value değişkenin değeridir. Ayrıca bu isimsiz fonksiyon içerisinde 2 saniyelik bir duraksatma yapılmıştır (Bizim.net dünyasından aşina olduğumuz Thread.Sleep gibi)

for döngüsündeki kullanım şekli de oldukça şıktır. 5 defa Calculate isimli fonksiyonun farklı parametre değerlerini alarak eş zamanlı yürütülmesi işlemi örneklenmiştir. Dikkat edilmesi gereken noktalardan birisi de main fonksiyonunun sonunda ekrandan giriş beklenmesidir. Eğer bunu yapmazsak tahmin edeceğiniz üzere program kodu anında sonlanır. Örnek kodun çalışma zamanı çıktısı aşağıdaki ekran görüntüsündekine benzer olacaktır.

![image.axd](images/image.axd)

channel

Aslında goroutine'ler pratik olsalar da tamamlandıklarında sinyal vermemeleri gibi bir sorunları da vardır. Sessizce işlerini tamamlayıp kaynaklarını iade ederler. İşte bu noktada channel yöntemi devreye girmektedir. Temel olarak bir channel ile goroutine'ler arasında iletişim kurabilir ve eş zamanlı çalışan iş parçaları arasında senkronizasyonu sağlayabiliriz. Channel konusu içerisinde bir çok alt konu da bulunuyor. Öğrenmeye çalışırken biraz zorlandığımı itiraf edebilirim ve konunun çok daha fazla derinliği var.

## İlk Örnek

Basit bir kod parçası ile başlayalım ve kanallar nasıl kullanılıyor ele alalım.

```cpp
package main

import(
	"fmt"
	"time"
	)

func main(){
	payload:=make(chan string)
	
	go Foo(payload,"code:1234")
	go Bar(payload)
	
	var userInput string
	fmt.Scanln(&userInput)
	fmt.Println("All is well")
}

func Foo(channel chan string,content string){
	time.Sleep(time.Second*3)
	fmt.Println("Foo...")
	channel<-content
}

func Bar(channel chan string){
	fmt.Println("Bar...")
	ctx:=<-channel
	fmt.Println(ctx)
}
```

Örnek kod parçasında iki goroutine arasında kanal açıp veri transferi gerçekleştirmekteyiz. Foo ve Bar isimli fonksiyonlar chan string tipinden parametre alıyorlar. Burada chan ifadesinden sonra gelen string, kanalda hangi tipten veri taşınacağını ifade etmekte. Dolayısıyla farklı veri tiplerini de bir kanal üzerinden eş zamanlı iş parçacıkları arasında taşıyabiliriz. Bir kanalı oluşturmak için make fonksiyonu kullanılıyor. İşin güzel yanı ise <- operatörü (Bir anda Ruby'deki << geldi aklıma) Bu operatör ile kanala veri bırakıp kanaldaki veriyi alma işlemlerini gerçekleştiriyoruz. Akla son derece yatkın. Operatörün sağından soluna doğru bir işlem akışı gerçekleşmekte.

Foo fonksiyonda belli bir süre duraksatma yapmaktayız (Olayı biraz daha dramatize edelim diye) Pek tabi Foo ve Bar fonksiyonları birer iş parçacığı olarak çağırılıyor. Yani goroutine haline getirilmişlerdir. Sonrasında payload isimli channel tipi bu parçacıklar tarafından kullanılarak veri transferi işlemi gerçekleştirilmiştir. Burada büyüleyici olan eş zamanlı çalışan iki fonksiyona arasında bir kanal açarak veri akışı sağlanmış olmasıdır. Bu ilkel kodun çalışma zamanı çıktısı aşağıdaki gibi olacaktır.

![image.axd](images/image.axd)

## Senkronizasyon

Kanalları kullanarak eş zamanlı çalışan iş parçaları arasında senkronizasyon da yapılabilir. Bir başka deyişle goroutine olarak başlatılan bir işin sonunda kanala işaret bırakılıp (true veya 1 gibi bir değer örneğin) diğer goroutine'in ilgili işareti alana kadar bekletilmesi sağlanabilir. Örneğin ana fonksiyon içerisinden başlatılan ve uzun sürecek bir iş sonlanmadan uygulamanın kapanmasını engellemek istediğimi durumlarda bu tekniği kullanabiliriz. Aşağıdaki basit kod parçasında bu durum örneklenmektedir.

```cpp
package main  

import
(
	"fmt" 
	"time"
)

func main() {     
    chnFlag:=make(chan int,1)    
    go DoHeavyWork(chnFlag)    
    <-chnFlag    
}

func DoHeavyWork(flag chan int){
    fmt.Println("Start...")
    time.Sleep(time.Second*5)
    fmt.Println("Done...")
    flag<-1
}
```

Kodun çalışma zamanı çıktısı aşağıdaki gibidir.

![image.axd](images/image.axd)

Burada kritik nokta main fonksiyonundaki <-chnFlag ifadesidir. Bu satırı kaldırınca kod durmadan akacak ve program sonlanacaktır.

![image.axd](images/image.axd)

## Yönlendirme (Direction)

Kanalları fonksiyon parametresi olarak kullanabiliyoruz. Bu durumda tip güvenliği adına kanalın çalışma yönünü de belirleyebiliriz. Yani fonksiyon parametresi olan bir kanalın sadece alıcı veya verici olması garanti edilebilir. Aşağıdaki kod parçasında bu kullanıma bir örnek verilmektedir.

```cpp
package main  

import(
        "fmt"
    )

func sender(channel chan<- string, message string) {
    channel <- message
}

func comm(receiver <-chan string, sender chan<- string) {
    message := <-receiver
    sender <- message    
}

func main() {    
    scott := make(chan string, 1)
    tiger := make(chan string, 1)
    sender(scott, "My name is Bond.James Bond.")
    comm(scott, tiger)
    fmt.Println(<-tiger)
}
```

sender isimli fonksiyon sadece mesaj gönderme özelliğine sahip bir kanal ile çalışır. İkinci parametre ile gelen string bilgi ilk parametredeki kanala yazılır. comm isimli fonksiyonun ilk parametresi mesaj okuma yeteneği olan bir kanaldır. İkinci parametre yine mesaj gönderme amacıyla kullanılabilecek bir kanalı ifade etmektedir. İlk parametre ile okunan mesaj ikinci parametre ile gelen kanala aktarılır. Kodun çalışmasında muazzam bir şey yoktur aslında. Öğrenmemiz gereken kanalların alıcı veya verici şeklinde sabit yönlerde kullanılmaya zorlanabilmesidir.

![image.axd](images/image.axd)

## buffer Kullanımı

Kanallar normalde senrkon çalışırlar. Yani mesajı gönderen taraf ile mesajı alacak olan taraf birbirlerini beklerler. Buffer kullanan kanallar inşa ederek birbirleriyle asenkron çalışmalarını sağlayabiliriz (Varsayılan olarak buffer kullanılmamaktadır) Buffer kullanacak bir kanal ile kanal tipinden kaç adet göndereceğimizi de belirtiriz. Yani kanal üzerinden akacak içeriği sayı bazında sınırlandırabiliriz. Bu kısıtlama bir anlamda semaphore tekniği uygulamak olarak da düşünülebilir. Aşağıda buffer kullanmına ilişkin basit bir kod parçası yer almaktadır.

```cpp
package main  

import "fmt" 

func main() {             
    channel:=make(chan string,3)
    channel<-"dam"
    channel<-"van dam"    
    channel<-"cloud van dam"
    channel<-"jan cloud van dam"
    
    fmt.Println(<-channel)
    fmt.Println(<-channel)
    fmt.Println(<-channel)
}
```

![image.axd](images/image.axd)

Oluşturulan channel tipi 3 string içeriği taşıyacak kapasitede tanımlanmıştır. Pek tabii kapasitesinden fazla mesaj atamaya çalışırsak Deadlock oluşmasına neden oluruz. Aşağıdaki ekran görüntüsünde olduğu gibi.

![image.axd](images/image.axd)

## select ifadesi

n sayıda goroutine çalıştırdığımızda kanallar ve select ifadesini kullanarak işi bitenlerin sonuçlarını almayı başarabiliriz. Bir nevi wait any hali diyelim. Aşağıdaki kod parçasını ele alalım.

```cpp
package main  

import 
    (
        "fmt" 
        "time"
    )

func CalculateOne(channel chan string){
    fmt.Println("Calculation phase one...")
    time.Sleep(time.Second*2)
    channel<-"phase one is done"
}

func CalculateTwo(channel chan string){
    fmt.Println("Calculation phase two...")
    time.Sleep(time.Second*5)
    channel<-"phase two is done"
}

func EvaluateTestData(channel chan string){
    fmt.Println("Creting test data...")
    time.Sleep(time.Second*3)
    channel<-"Evaluation is done"
}

func main() {    
    channelOne:=make(chan string,1)
    channelTwo:=make(chan string,1)
    channelEval:=make(chan string,1)
    
    go CalculateOne(channelOne)
    go CalculateTwo(channelTwo)
    go EvaluateTestData(channelEval)
    
    for i:=0;i<3;i++{
        select{
         case messageOne := <-channelOne:
            fmt.Println(messageOne)
        case messageTwo := <-channelTwo:
            fmt.Println(messageTwo)   
        case messageEval:=<-channelEval:
            fmt.Println(messageEval)
        }
    }
}
```

Örnekte üç farklı fonksiyon bulunmaktadır. Her birisinde uzun süren işler olduğunu göstermek için time.Sleep fonksiyonundan yararlanılmaktadır. Fonksiyonlar birer goroutine haline getirilip eş zamanlı olarak çalıştırılmaya başladıktan sonra select ifadesi ile kontrol altına alınırlar. 3 goroutine olduğundan for döngüsü de 3 iterasyon ilerleyecektir. Her bir case bloğunda ilgili kanalın dönüşünün olup olmadığına bakılır. Senkronizasyon örneğinde olduğu gibi her fonksiyonun sonunda kanala bırakılan bir sinyal vardır. Bu mesajlar case ifadelerinde ele alınırlar. Tahmin edileceği üzere görevler bittikçe ekrana kanaldan gelen mesajlar basılacaktır. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![image.axd](images/image.axd)

Böylece geldik bir gopher olma çalışmamızın daha sonuna. Bu yazımızda Concurrency konusunun iki önemli kavramına değinmeye çalıştık. Go dili ile ilgili bir şeyler öğrendikçe yazmaya devam edeceğim. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
