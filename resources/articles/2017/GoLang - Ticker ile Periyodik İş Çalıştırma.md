---
title: "GoLang - Ticker ile Periyodik İş Çalıştırma"
pubDate: 2017-06-18 06:11:00
categories:
  - GoLang
tags:
  - GoLang
  - .NET
  - Go
  - Threading
  - Concurrency
  - Performance
---

# GoLang - Ticker ile Periyodik İş Çalıştırma
![gotimer_4.gif](images/gotimer_4.gif)

Merhaba Arkadaşlar,

GO dilinin en güçlü yanlarından birisi eş zamanlı programlama (Concurrent Programming) kabiliyetleri sayesinde sunduğu performans ve kullanım kolaylıkları. Daha önceden [şu yazıda](GoLang - Concurrency (goroutine, channel).md) Concurrency konusunu GoRoutine ve Channel kavramları üzerinden incelemiştim. Çalıştığım kaynaklarda ilerledikçe eş zamanlı programlama konusunda yeni şeyler de öğrendim. Bunlardan birisi de time tipi. Bu tipin NewTimer ve NewTicker isimli iki önemli fonksiyonu bulunuyor. Doğruyu söylemek gerekirse NewTimer ile yapacağımız işlemleri time.Sleep kullanımı ile de sağlamamız mümkün. Bu nedenle NewTimer'ı daha çok bir GoRoutine'in beklenen sürede işini yapmasını beklediğimiz, aksi hallerde ise zaman aşımı halini ele alacağımız durumlarda kullanmanın çok daha mantıklı olduğunu öğrendim. Diğer yandan NewTicker fonksiyonu daha çok dikkatimi çekti. Bu fonksiyon ile belirli periyotlar boyunca tekrar etmesini istediğimiz eş zamanlı görevler planlayabiliriz. Konuyu anlamaya çalışırken önce teorik bir örnek ile ilerlemeye çalıştım. Ardından daha pratik bir örnek geliştirdim. İlk GO kodlarımızı aşağıdaki gibi geliştirdiğimizi düşünelim.

```cpp
package main

import (
	"fmt"
	"time"
)

func main() {
	timer := time.NewTimer(time.Second * 4)

	fmt.Println("timer nesnesi tanımlandı. Kod akışına devam ediyor.")
	fmt.Println(time.Now())
	now := <-timer.C //C ile timer'ın NewTimer'a parametre olarak gelen süre sonrasındaki zaman elde edilir
	fmt.Println("Timer ile belirtilen süre doldu.")
	fmt.Println(now)

	// Bu seferki Timer, süresi dolduğu için Expire durumuna düşecek
	// Bir Timer'ı expire olmadan önce durdurmak istediğimiz senaryolarda ele alabiliriz
	timer = time.NewTimer(time.Second)
	go func() {
		<-timer.C
		fmt.Println("İkinci timer süresi geçti") // time.Second nedeniyle func içerisinde timer.C yakalanamadan Stop metoduna düşülür
	}()
	stop := timer.Stop()
	if stop {
		fmt.Println("Timer durduruldu")
	}

	// ticker ile zamanlanmış görevler hazırlayabiliriz.
	tickTime := time.NewTicker(time.Second * 2) // iki saniyede bir zaman döndürecek Ticker tanımlandı
	go func() {
		fmt.Println("İş yapıyorum...")
		for t := range tickTime.C { // C ile yukarıdaki tickTime'ın o anki süresi ele yakalandı
			fmt.Println(t)
		}
	}()
	//time.Sleep(time.Second * 12) // main thread 12 saniye duracak. Bu süre boyunca 2 saniyede bir for t:=range bloğu çalışacaktır
	//tickTime.Stop()              //Ticker durduruldu

	// Yukarıdaki kullanımdan farklı olarak şimdi kullanıcı Enter tuşuna basana kadar for t:=range bloğu çalışacaktır
	var enter string
	fmt.Println("Çıkmak için Enter tuşuna basınız")
	fmt.Scanln(&enter)
	tickTime.Stop()
}
```

Öncelikle kodun çalışma zamanı çıktısına bir bakalım.

![gotimer_1.gif](images/gotimer_1.gif)

Örnek kod parçasında iki NewTimer ve bir NewTicker örneği yer alıyor. main, 4 saniye sonrası için kurulan bir timer tanımlaması ile başlıyor. Bu tanımlama sonrası kod akışına devam edecek ve now değişkenine C ile bir bilgi geçilecektir. Aslında burada bir kanal (Channel) üzerinden o anki zamanın döndürülmesi işlemi gerçekleşmektedir. Tahmin edileceği üzere kod bu satıra gelinceye kadar bekler. Yani 4 saniyelik bir bekleme oluşur (Az öncede belirttiğim gibi bu noktada time.Sleep metodundan da yararlanılabildiği belirtiliyor) timer'ı tekrar oluşturduğumuz kod satırında ise bir saniye sonrası için planlama yapılmıştır. Hemen ardından bir GoRoutine başlatıldığı görülür. Bu fonksiyon içerisinde <-timer.C satırı ile kanaldan gelecek bilgi beklenmektedir. Ancak GoRoutine hemen çalıştığından kod bir sonraki ifadeye geçecek ve timer için Stop metodu devreye girecektir. İşte bu noktada GoRoutine için bir zaman aşımı senaryosu işletilmiş olur. Lakin Stop çağrısına gelinmeden önce NewTimer ile açılan süreden daha uzun süren işlemler söz konusu olursa, GoRoutine işleyişini de tamamlayabilecektir.

Kodun son parçasında bir ticker üretilmektedir. NewTicker metodu ile oluşturulan tickTime, 2 saniyelik periyotları ifade eder. Hemen ardından gelen GoRoutine içerisinde ise C üzerinden yakalanan kanal içeriğinin bir takibi yapılır. Takip için for döngüsü ve range ifadesinden yararlanılır. Her iki saniyede bir kanala o anki zaman bilgisi düşeceğinden bu bir range üzerinden for döngüsü ile yakalanabilir. Kullanıcı ekrandan enter tuşuna basana kadar 2 saniyelik zaman dilimlerinde for bloğunun içerisindeki kod parçası devreye girecektir. Tabii söz konusu işlemlerin bir süre sonra tamamlanmasını da sağlayabiliriz. Bunun için yorum satırı yapılmış olan kısma dikkat edelim. Yorum satırları açıldığı takdirde 12 saniye sonrasında periyodik olarak devam eden işlemler duraksatılacaktır.

![gotimer_2.gif](images/gotimer_2.gif)

Sonra durdum ve daha akılda kalıcı bir periyodik kod parçası yazabilir miyim diye düşünmeye başladım. Aklıma belirli bir klasörün içerisindeki dosya değişikliklerini belirli zaman aralıklarında izleyecek bir örnek geldi (Bir nevi.Net dünyasından aşina olduğumuz FileSystemWatcher'ın ilkel bir halini geliştirmek istedim diyebiliriz) Amacım belirli zaman aralıklarında C:\Reports isimli klasördeki dosya içeriklerini terminale bastırmaktı. Bunun için komut satırından yürüyecek bir go programı yazabilir, içerisinde eş zamanlı olarak belirli periyotlarda tetiklenecek bir GoRoutine oluşturabilirdim. Öğrenmem gereken bir başka şey de bir klasör içerisindeki dosyaları nasıl ele alabileceğimdi. Biraz araştırma sonrası aşağıdaki kod parçası ortaya çıktı.

```cpp
package main

import (
	"fmt"
	"os"
	"path/filepath"
	"time"
)

func main() {
	var pathName string = "C:\\Reports"
	ticker := time.NewTicker(time.Second * 10)
	go func() {
		for t := range ticker.C {
			fmt.Printf("Time : %s\n", t)
			getFileList(pathName)
		}
	}()

	var enter string
	fmt.Println("Press Enter for Exit")
	fmt.Scanln(&enter)
	ticker.Stop()
}

func getFileList(pathName string) {
	fmt.Println("___", pathName, "___")
	filepath.Walk(pathName,
		func(path string, fileInfo os.FileInfo, err error) error {
			if !fileInfo.IsDir() {
				fmt.Printf("\t%s\t%d bytes\n", fileInfo.Name(), fileInfo.Size())
			}
			return nil
		})
	fmt.Println("____________________________________")
}
```

Kod 10 saniyede bir çalışacak bir Ticker tanımlaması ile başlıyor. Sonrasında gelen GoRoutine içerisinde ise for döngümüzü kurguluyoruz. C kanalı üzerinden gelen zaman bilgisini ekrana basıyor ve getFileList isimli fonksiyonu çağırıyoruz. Bu döngü 10 saniyede bir çalışacak ve her seferinde getFileList fonksiyonu çağırılacak. Fonksiyon parametre olarak gelen klasördeki dosyaları alabilmek için filePath tipinin Walk metodunu kullanıyor. İlk parametre klasör adı ama ikinci parametre biraz daha değişik. Burada WalkFunc fonksiyon tipinden bir fonksiyon bildirimi yer alıyor. Aslında klasör içerisinde yürürken her bir klasör veya dosya için ikinci parametre ile gelen fonksiyon bloğu çağırılmakta. Bu fonksiyon üç parametre almakta. Üzerinde çalışılan klasör, o anki öğe (dosya veya klasör gibi) ve varsa hata mesajı. Geriye ise bir error nesnesi döndürmekte (return nil satırının konulma sebebi de aslında bir hata olmayacağını düşünmem) Walk tüm klasör ve dosyaları dolaştığından ve senaryo gereği bana sadece dosyalar gerektiğinden IsDir çağrısı ile o anki öğenin ne olduğuna bakıyorum. Eğer klasör değilse dosya olduğuna karar verip ekrana o dosya ile ilgili bilgiler bastırıyorum (Dosya adı ve byte olarak boyutu şimdilik yeterli) Program kullanıcı enter tuşuna basana kadar çalışıyor. Denemeler sırasında C:\Reports altına bir kaç dosya attım, silme işlemi gerçekleştirdim. Sonuçlar aşağıdaki ekran görüntüsündekine benzer oldu.

![gotimer_3.gif](images/gotimer_3.gif)

Bu örnekle NewTicker ve time paketinin kullanımı kafamda biraz daha anlamlı bir yer edindi diyebilirim. Bazı konuları öğrenmeye çalışırken gerçek hayat örneklerini kullanmak çok ama çok faydalı. Senaryo daha da zenginleştirilebilir. Söz gelimi bir klasördeki dosya değişimleri gözlemlenebilir ve loglama amaçlı bir işlevsellik sağlanabilir. Aynı stratejiyi kullanarak belirli zaman aralıklarında gerçekleşmesini istediğimiz işlemler için planlamaları basitçe yapabiliriz. Böylece geldik bir GoLang maceramızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
