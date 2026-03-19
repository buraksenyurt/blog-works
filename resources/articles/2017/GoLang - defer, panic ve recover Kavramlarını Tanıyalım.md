---
title: "GoLang - defer, panic ve recover Kavramlarını Tanıyalım"
pubDate: 2017-03-09 21:32:00
categories:
  - GoLang
tags:
  - GoLang
  - Go
---

# GoLang - defer, panic ve recover Kavramlarını Tanıyalım
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Gopher'ın Go diline kattığı sevimlilik ortada. Sadece maskotu değil bazı kavramları da oldukça motive edici bu dilin. Bir.Netçi olarak ortama hata fırlatmak istediğim de kullandığımız throw new Exception gibi bir terminoloji yerine panic şeklinde bir anahtar kelimenin kullanılması aslında kasttettiğim. Hatta go dilinin resmi dokümanlarında "Panic is a built-in function that stops the ordinary flow of control and begins panicking." şeklinde bir cümle ile bu terim hoş bir şekilde ifade edilmiş. En azından beni tebessüm ettirdi fotoğraftaki minion'u ise biraz ürküttü.

Geçtiğimiz günlerde panic fonksiyonunu incelerken aslında recover ve defer kavramları ile birlikte kullanımının daha anlamlı olduğunu öğrendim. Aslında amacım ortama bir istisnanın nasıl fırlatılabileceğini görmek ve hata yönetimini incelemekti. Derken kendimi defer ifadesi ile panic ve recover fonksiyonlarını araştırırken buldum. İlk etapta bu üç kavramın kod akışını kontrol etmek için kullanıldığını söyleyebiliriz. Şimdi bu kavramları örneklendirerek kısaca incelemeye çalışalım.

defer

.Net kökenli yazılımcılar için finally operasyonları amacıyla kullanılır dersek sanırım yerinde olacaktır. defer ifadesi ile işaret edilen fonksiyon, program çalışması sırasında mutlak suretle devreye girmesi istenen operasyonlarda kullanılır. Üzerinde işlem yapılmış bir dosyanın, açılan bir veritabanı bağlantısının, haberleşilen bir soket ile olan iletişimin kapatılması veya belleğe alınan ama işleri biten nesnelerin serbest bırakılması gibi genelleyebileceğimiz işlemler bu operasyonlara örnek olarak verilebilir. Tabii burada dikkat çekici nokta defer ifadesinde bildirilen fonksiyonun kodun akışında bir hata olması halinde de devreye girmesidir. Bir başka deyişle runtime panic olarak isimlendirilen çalışma zamanı hatalarının oluştuğu durumlarda defer edilen fonksiyonların çalışması söz konusudur. Kaynaklarda sıklıkla geçen dosya işlemlerinden basit bir tanesini bu bağlamda ele alalım.

```cpp
package main 

import (
	"fmt" 
	"os"
)

func main() { 
	saveToFile("sometext.txt","this is gonna be the best day of my life")
}

func saveToFile(name string,content string) {
	fmt.Println("creating...")
	file,error:=os.Create(name)
	if error==nil{
		defer closeFile(file) // dogrudan defer file.Close() da denenmeli
		fmt.Fprintln(file,content)
	}else{
		return
	}
}

func closeFile(file *os.File){
	fmt.Println("closing...")
	file.Close()
	fmt.Println("closed")
}
```

![image.axd](images/image.axd)

saveToFile fonksiyonu sistem üzerinde bir dosya açıp bunun içerisinde belirtilen içeriğin yazılması ile ilgili bir işlem gerçekleştirmekte. Dosyayı oluşturmak için create operasyonundan yararlanıyoruz. Create fonksiyonundan oluşan dosya ve bir hata değişkeni dönmekte (fonksiyondan dönen değerler için çoklu atama yapıldığını fark etmişsinizdir) Eğer hata yoksa fmt paketinin FPrintln fonksiyonu ile dosyanın içerisine basit bir metin yazıyoruz. Fonksiyon ilk parametre ile dosyayı, ikinci parametre ile de içeriği alıyor.

Örnekte bakmamız gereken kısım defer ifadesinin olduğu yer aslında. closeFile isimli bir fonksiyonu işaret ediyor. Buna göre dosyanın oluşturulması ve yazılması sırasında bir hata oluşsa bile kapatma operasyonu otomatik olarak devreye girecek. Tanımlama dosya yazma işleminin öncesinde yapıldı. Bu o anda çağırılacağı anlamına gelmiyor. Aslında standart bir fonkisyon çağrısı değil burada kastedilen. Bir nevi kapatma operasyonunu garanti altına aldığımızı ifade edebiliriz.

## LIFO Durumu

defer ifadesi son giren ilk çıkar mantığına göre çalışır (Last In First Out). Buna göre bir fonksiyon içerisinde kullanılan ne kadar defer ifadesi varsa son girenden ilk eklenene göre teker teker çalıştırılır. Bu durumu anlamak için aşağıdaki kod parçasını göz önüne alalım.

```cpp
package main

import(
    "fmt"
)

func main() {
    doSomething()
}

func doSomething(){
    defer subProc(100)
    defer subProc(200)
    defer subProc(300)
    
    numbers:=[]int{4,5,1,9,8}
    for _,n:=range numbers{
        fmt.Println(n)
    }
}

func subProc(i int){
    fmt.Println(i)
}
```

![image.axd](images/image.axd)

doSomething içerisinde defer ifadeleri haricinde bir slice içerisindeki elemanlarda dolaşılmaktadır. subProc içinse 3 defer ifadesi tanımlanmıştır. doSomething normal işleyişini tamamladıktan sonra içerisinde defer edilen fonksiyonlar ters sırada çalışmıştır.

panic ve recover

Yazdığımız uygulama kodunda meydana gelebilecek bazı hatalar ortama panic olarak yansır. Bir dizinin olmayan elemanına erişilmeye çalışılması, açılmak istenen dosyanın ilgili klasörde olmaması, başaltılmadan (initialize edilmeden) bir slice içeriğinin kullanılmaya çalışılması gibi durumlar bu hatalara örnek olarak verilebilir. Geliştirici isterse çalışma zamanı için bilinçli olarak panik havası da estirebilir. Her iki durumda da paniğin oluştuğu fonksiyonun çalışması durdurulur, varsa defer edilmiş fonksiyonlar çalıştırılır ve ardından fonksiyonun sahibi olan konuma (function caller) dönülür ama devam eden kod satırları işletilmez. Oluşan panikten sakin bir şekilde çıkılması için recover fonksiyonundan yararlanılır. Ne var ki recover çağrımlarının anlamlı olması için defer iile kullanımı gerekir.

Şu ana kadar ki kodlarımızda zaman zaman da olsa panik havası esmedi değil aslında. Söz gelimi aşağıdaki kod parçası çalışma zamanında bir panik oluşmasına (panic: runtime error: index out of range) neden olur.

```cpp
package main

func main(){
	numbers:=make([]int,5)
	numbers[6]=10
}
```

![image.axd](images/image.axd)

Az önce recover ile bu tip çalışma zamanı paniklerini yatıştırabileceğimize değinmiştik. Ben tabii konuyu öğrenirken balıklama şöyle bir kod parçasını denedim.

```cpp
numbers:=make([]int,5)
numbers[6]=10
err:=recover()
fmt.Println(err)
```

ama sonuç değişmedi. O anda recover'ın neden defer ile birlikte kullanıldığını daha iyi anlamaya başladım. Aynı kod parçasını aşağıdaki gibi düzenleyerek ilerleyelim.

```cpp
package main  
import (
    "fmt" 
    )

func main() { 
    numbers:=make([]int,5)
    defer easy(numbers)
    numbers[6]=10
    fmt.Println("have fun")
}
func easy(n []int){    
    if err := recover(); err != nil {
        fmt.Printf("slice length is : %d\n",len(n))
        fmt.Printf("don't panic it's just an error\n%s",err)
	}
}
```

![image.axd](images/image.axd)

Dikkat edileceği üzere defer ifadesi ile main fonksiyonunda olası bir panik durumunda gidilebilecek bir başka fonksiyonu işaret ediyoruz. easy içerisinde recovery fonksiyonundan yararlanarak oluşan hatayı yakalayıp (eğer varsa) program akışının kontrol altına alınmasını sağlıyoruz. easy fonskiyonuna main içerisinde defer tanımını yaparken parametre geçişi de yapmaktayız (Bunu sadece parametre geçirebileceğimizi göstermek için yazdık) Bu arada defer fonksiyonunu istersek closure olarak da yazabiliriz ki yaygın kullanım şekli budur. Aynen aşağıdaki kod parçasında görüldüğü gibi.

```cpp
package main  
import (
    "fmt" 
    )

func main() { 
    numbers:=make([]int,5)
    defer func(){    
        if err := recover(); err != nil {
            fmt.Printf("slice length is : %d\n",len(numbers))
            fmt.Printf("don't panic it's just an error\n%s",err)
        }
    }()
    numbers[6]=10
    fmt.Println("have fun")
}
```

Ancak gözden kaçmaması gereken bir nokta daha var. numbers[6]=10 ataması sonrası oluşan hata yakalanmış olsa da devam eden kod satırı işletilmedi! Yani program cidden çakıldı ve biz paniği sessiz sedasız defer ettiğimiz fonksiyon üzerinden soğukkanlı bir şekilde yatıştırdık. Olay main'de ceyeran ettiği için programdan çıkılmış olması normal. Bir.Netçi olarak bildiğimiz try...catch...finally yapısından oldukça farklı bir çalışma şekli gibi duruyor. Sanki Go dili programıcının ciddi anlamda çalışma zamanı hatası yaptıracak kod yazmasını istemiyor gibi. Aşağıdaki kod parçasını göz önüne alarak bu durumu biraz daha açalım.

```cpp
package main

import(
    "fmt"
)

func main() {
    launch()
    fmt.Println("to be continued...")
}

func launch() {
    defer func() {
        if err := recover(); err != nil {
            fmt.Println("There's something wrong:", err)
        }
    }()
    fmt.Println("Start Engine ")
    startEngine()
    fmt.Println("After recovery ")
}

func startEngine() {
    panic("aaa Houston! We have a problem.")
}
```

![image.axd](images/image.axd)

main içerisinde launch isimli bir fonksiyon çağırıyoruz. Bu fonksiyon roketimizin motorlarını çalıştıran bir operasyonu kullanıyor ama başında defer ettiğimiz bir panik kontrol odası da var. startEngine içerisindeki panic fonksiyonu bilinçli olarak çalışma ortamına hata yollamak için kullanılıyor. Bu hata, defer edilen fonksiyon içerisinde yakalanıyor. İşte burası önemli. Ekrana "After recovery..." yazılmadı ama main'deki "to be continued..." basıldı. Yani hata üreten fonksiyonun çağırıcısındaki defer operasyonu devreye girdikten sonra launch işleyişinin tamamen sonlanması ve program kontrolünün main'e dönmesi söz konusu. Buna göre defer edilen fonksiyonların bir zincire eklendiğini de düşünebiliriz.

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda Go programlama dilinde program kontrol akışını değiştirmek için kullanılan defer ifadesi ile built-in gelen panic ve recover fonksiyonlarını incelemeye çalıştık. Bugüne kadar Go dile ile ilgili çalışmalardan gördüğüm kadarı ile programcının oldukça titiz kodlama yapması ve kavramlara aşina olması için oldukça fazla pratik yapması gerekiyor. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
