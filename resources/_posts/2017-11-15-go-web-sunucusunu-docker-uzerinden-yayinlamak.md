---
layout: post
title: "Go Web Sunucusunu Docker Üzerinden Yayınlamak"
date: 2017-11-15 10:00:00 +0300
categories:
  - golang
tags:
  - golang
  - docker
  - container
  - web-hosting
---
Gondor'da bir şeyler araştırmak için harika bir zaman. Çünkü elimdeki işler bitti. Böyle vakitleri kendi araştırmalarıma ayırmak hoşuma gidiyor, kim ne derse desin. Yeni gözdem Linux makinem de (Gondor) önümde durduğuna göre kısa bir süre onun üzerinde çalışabileceğimi düşünüyorum.

![go_docker5.gif](/assets/images/2017/go_docker5.gif)

Aklıma gelen ilk şey ise, Go diliyle yazılmış ilkel bir web sunucusunu Docker üzerinden kullanabilmek. Önce web sunucusunu geliştirmek, başarılı bir şekilde çalıştığından emin olmak, sonrasında bir Docker imajı hazırlamak lazım. Ardından oluşturulan imajdan yararlanarak bir Container başlatıp web sunucusunun bu taşıyıcı örneği üzerinden çalışıp çalışmadığını test etmek senaryonun tamamlanması açısından yeterli. Tahminlerime göre 15 dakikayı aşmayacak bir iş gibi duruyor. Haydi başlayalım.

Gondor'da açtığım SimpleWebServer klasörüne aşağıdaki kod parçasını içeren main.go isimli bir dosya ekleyerek ilerliyorum. Tabii buradaki ortamda GOPATH tanımlamalarını değiştirmiştim. $home\goprojects altında konuşlandırıyorum (Geliştirici arabirimi olarak Visual Studio Code'tan faydalanıyorum. Her zaman ki gibi çok keyifli bir geliştirici deneyimi sunuyor. Size de tavsiye ederim)

```cpp
package main

import(
    "fmt"
    "net/http"
    "runtime"
    "math/rand"
    "time"
)

func indexHandler(w http.ResponseWriter,r *http.Request){
    t:=time.Now()   
    w.Header().Set("Content-Type","text/html; charset=utf-8")
    fmt.Fprintf(w,"Gondor time : <b>(%s)</b><br/>We are running on <b>%s</b> with an <b>%s</b> CPU<br/><i>Your lucky number</i> <b>%d</b>",t.Format(time.RFC1123),runtime.GOOS,runtime.GOARCH,rand.Intn(100))
}

func main(){
    http.HandleFunc("/",indexHandler)
    fmt.Println("listening...")
    http.ListenAndServe(":8087",nil)    
}
```

Uzun zamandır Go ile kod yazmıyordum. Özlediğimi ifade edebilirim. Özellikle de kurallarını ve basitliğini. Ana paketteki programın başlangıç noktası olan main içerisinde HandleFunc isimli fonksiyondan yararlanarak root adrese gelecek olan talepleri indexHandler isimli operasyona yönlendiriyoruz. indexHandler içerisinde ise çok basit bir HTML içeriği bastırmaktayız. Elle tutulur bir şeyler olması açısından güncel zaman bilgisini, işletim sistemini, işlemcinin türevini yazdırdıktan sonra 0 ile 100 arasında üretilecek rastgele bir sayı da basıyoruz. İçeriğin HTML tipinden olacağını Header'a ait Set fonksiyonu ile belirtmekteyiz. Böylece istemciye ulaşacak paketin html olarak yorumlanılması gerektiğini de söylemiş oluyoruz. main içerisinde yer alan ListenAndServe fonksiyonu da 8087 (istediğiniz bir portu kullanabilirsiniz tabii) nolu porttan yayın yapılmasını sağlamakta.

İlk olarak kodun bu şekilde çalışıp çalışmadığını test etmek lazım. Terminalden

```bash
go run main.go
```

komutunu vererek bu denemeyi yaşayabiliriz. Aşağıdaki ekran görüntüsünde örnek bir çalışma zamanına yer veriliyor. Ben bu görüntüyü aldığımda yine gülümsedim hafifçe.

![godocker_1.gif](/assets/images/2017/godocker_1.gif)

Hedefimiz şu. Bu uygulamayı başlatıldığı zaman ayağa kaldıracak bir Docker imajı oluşturmak. Bunun yolu bildiğiniz gibi ilgili komutları içerecek bir Dockerfile oluşturmaktan geçiyor. main.go ile aynı lokasyona aşağıdaki içeriğe sahip docker dosyasını ekleyerek ilerleyebiliriz.

```text
FROM golang

ADD . /go/src/GoWebServer
RUN go install GoWebServer
ENTRYPOINT [ "/go/bin/GoWebServer" ]

EXPOSE 8087
```

Öncelikle resmi golang imajından feyz aldığımızı belirtelim. İlk satırda bunu belirtmekteyiz. Sonrasında kaynak kodun tamamını oluşturulan imaj içerisindeki /go/src/GoWebServer adresine taşıyoruz. Tahmin edileceği üzere golang imajındaki GOROOT ve GOPATH ortam ayarlamalarına uyan taşımalar yapılmakta. Bu path tanımlamaları ata imajda hazırlanmış. GOPATH tanımına göre kaynak kodları go/src altına atmamız gerekiyor. Sonrasında GoWebServer içeriğini sisteme yüklüyoruz (Go Deployment) RUN ifadesinden sonra gelen komut bu işi yapmakta. Container başlatıldığında giriş noktası olarak kurulumun gerçekleştiği go/bin/GoWebServer klasörünü işaret ediyoruz. Son olarak Container'ı localhost:8087 portu üzerinden yayına açıyoruz.

Dosya hazırlandıktan sonra imajın oluşturulmasına başlayabiliriz. Bu tipik olarak Docker'ın Build operasyonu. Terminalden aşağıdaki komutu kullanarak inşayı başlatabiliriz.

```bash
sudo docker build -t go_rnd_server .
```

![godocker_22.gif](/assets/images/2017/godocker_22.gif)

go_rnd_server isimli bir imaj oluşturduk. Eğer indirilmesi gereken içerikler varsa sisteme yüklenmesi için bir süre beklemek gerekebilir. Herhangibir hata alınmadıysa oluşturulan imajı kullanarak yeni bir Container başlatabiliriz. Bunun için docker'ın run komutundan yararlanmak gerekiyor. -p den sonra gelen adres ile yayınlamanın hangi adresten yapılacağını da belirtmiş oluyoruz.

```bash
sudo docker run -p 8087:8087 go_rnd_server
```

![go_docker33.gif](/assets/images/2017/go_docker33.gif)

Ekran görüntüsünden de görüleceği gibi artık docker üzerinde konuşlandırdığımız Web sunucusuna gidebiliyoruz (Biraz daha gülümsüyorum) Örneği genişletmek tabii ki sizin elinizde. Burada Go ile yazılmış bir REST servis de sunulabilir. Hatta veri için Redis gibi bir yapı kullanılarak senaryo daha da heyecanlı hale getirilebilir (En sevdiğiniz filmdeki sözlerin içeren bir veri kümesinden kullanıcıya rastgele sözler yolladığınız bir örnek üzerinde çalışabilirsiniz) Bunlara ek olarak söz konusu Container'lardan bir kaçının farklı port'lardan başlatılması da denenebilir. Bu mümkün olabilir mi şimdilik bilmiyorum ama denemek de istiyorum. Özellikle ortak veri kullanımları veya verinin tüm Container'lar için eşleştirilmesi gibi epik senaryoları kafamda şekillendirmekte henüz zorlanıyorum.

Makaleme son vermeden önce çalışmakta olan Container'ı nasıl durduracağımızı da söyleyeyim. Eğer bir Deploy işlemi gerçekleştireceksek bunun öncesinde ilgili Container örneğinin durdurulması önerilmiş. İlk olarak var olan Container örneklerini görelim. Terminalden vereceğimiz aşağıdaki komutu bunu sağlayacaktır.

```bash
sudo docker ps -a
```

![go_docker4.gif](/assets/images/2017/go_docker4.gif)

Çalışmakta olan bir Container örneğini durdurmak için de ID değerini kullanabiliriz. Aşağıdaki gibi.

```bash
sudo docker stop fb3936d9f595
```

Böylece geldik kısa süreli faydası olan bir öğle arasının daha sonuna. Artık işin sırrı öğrenildi diyebilirim. Hakim olduğunuz bir dille basit bir Web sunucusu veya REST servisi yazıp Docker ile sunmayı deneyebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
