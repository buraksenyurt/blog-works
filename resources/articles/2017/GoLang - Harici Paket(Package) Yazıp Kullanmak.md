---
title: "GoLang - Harici Paket(Package) Yazıp Kullanmak"
pubDate: 2017-01-28 14:42:00
categories:
  - GoLang
tags:
  - GoLang
  - JSON
  - HTTP
  - Go
  - GitHub
---

# GoLang - Harici Paket(Package) Yazıp Kullanmak
![gopckg_9.gif](images/gopckg_9.gif)

Merhaba Arkadaşlar,

Go dilinin paketler üzerine kurulu bir yapısı olduğunu biliyoruz. fmt, math, strings, net/http, time, log, encoding/json ve benzerleri şu kısa go geçmişimde kullandıklarımdan sadece birkaçı. Geliştirdiğimiz ürünlerde ortak sorumlulukları barındıran fonksiyonellikleri aynı paketler içerisinde toplamak son derece mantıklı. Bu sayede aynı alana ait fonksiyonellikleri bir paket içerisinde toplayıp kullanabilme şansına sahip oluyoruz. Paketler, kodun yeniden kullanılabilirliği (Code Reusability) noktasında da değer bulan bir kavram.

Go dilini sistemimize yüklediğimizde zaten beraberinde pek çok paket geliyor ve bu paketler GOPATH ile belirtilen konumdaki src klasörü altında konuşlandırılıyor. Örneklerimizde çok sık kullandığımız fmt paketine ait kod dosyalarını src klasörü altından görebilir ve kod içeriğine bakıp Println fonksiyonunun nasıl çalıştığını inceleyebiliriz.

> Go programlarının çalışmaya başladığı main fonksiyonunun da main isimli paket (Package) içerisinde yer aldığını hatırlayalım.

![gopckg_1.gif](images/gopckg_1.gif)

## Kendi Paketimizi Yazalım

Buna göre kendi yazdığımız paketleri src klasörü altına atıp kullanmaya başlayabiliriz. Gelin bu vakayı basit bir örnekle incelemeye çalışalım. Tabii ilk olarak bir paket yazarak işe başlamamız gerekiyor:) Sözgelimi içerisinde bir kaç metrik dönüştürme işlemi barındıran mtrcvrt (Metric Converter diye isimlendirebiliriz) adlı bir paket geliştirdiğimizi düşünelim.

```text
package mtrcvrt

//Fahrenheit to Celsius
func FahToCel(f float64) float64{
	return (f-32)/1.8
}

// Celsius to Fahrenheit
func CelToFah(c float64) float64{
	return (c*1.8)+32
}

// Feet to Meter
func FeetToMeter(feet float64) float64{
	return feet/3.2808
}

// Kg to Pound
func KgToPound(kg float64) float64{
	return kg*2.2046
}

// Kilometers to Mile
func KmToMiles(km float64) float64{
	return km*0.621371192
}
```

Yazdığımız paket içerisinde beş fonksiyon bulunuyor. Fahrenheit'dan Celsius'a, Kilogram'dan Pound'a, Kilometre'den Mil'e, Feet'ten metreye ve son olarak Celcius'dan Fahrenheit'a dönüşüm işlemlerini içeriyor. Kod paket adıyla başlıyor. Kullandığımız fonksiyonlar float64 tipinden birer parametre almakta ve gerekli formüller işletildikten sonra geriye yine float64 tipinden değer döndürmekteler. mtrcvrt'yi sistemde herhangibir main paketinden kullanabilmek için az önce bahsettiğimiz src klasörü içerisine atmamız yeterli. Pek tabi kaynak kodun mtrcvrt klasörü altında konuşlandırıldığına dikkat edelim (Aynı diğer built-in paketler gibi)

![gopckg_2.gif](images/gopckg_2.gif)

> Şunu unutmayalım ki paket içerisindeki fonksiyonların adları ve özellikle ilk harfleri çok önemli. Eğer büyük harfle başlamazlarsa paket dışından kullanılamazlar.

Şimdi örnek bir program dosyası oluşturup yazdığımız pakete ait fonksiyonellikleri test edelim.

```text
package main

import (
	"fmt"
	"mtrcvrt"
	)
	
func main(){
	var (
		iFahrenheit=89.0
		iCelsius=36.5
		iFeet=100.0
		iKg=83.50
		iKm=450.0
		)
	
	oCelcius:=mtrcvrt.FahToCel(iFahrenheit)
	fmt.Printf("%f\n",oCelcius)
	
	oFahrenheit:=mtrcvrt.CelToFah(iCelsius)
	fmt.Printf("%f\n",oFahrenheit)
	
	oMeter:=mtrcvrt.FeetToMeter(iFeet)
	fmt.Printf("%f\n",oMeter)
	
	oPound:=mtrcvrt.KgToPound(iKg)
	fmt.Printf("%f\n",oPound)
	
	oMiles:=mtrcvrt.KmToMiles(iKm)
	fmt.Printf("%f\n",oMiles)
}
```

import sekmesinde paketin adını belirttik ve main içerisinde sahip olduğu fonskiyonları birer örnekle denedik. var ifadesinde n sayıda değişkene değer ataması gerçekleştiriyoruz ki bunu ilk kez kullandığımı söyleyebilirim. Kodun çalışma zamanı çıktısı ise aşağıdaki gibi olacaktır.

![gopckg_3.gif](images/gopckg_3.gif)

Yazdığımız paketlerin src klasörü altında olması şart değil. Aslında Go dili klasör temelli bir çalışma ortamını (Workspace) baz alıyor. Bu ortamda src, bin ve pkg klasörleri standart olarak bulunuyor. Paket kodları src klasörü altında konuşlandırılmakta. Go ortamı sisteme yüklendiğinde bu klasör hiyerarşisini kullanıyor. Diğer yandan github gibi önemli bir paket deposu da var. Pek çok programlama dilinde olduğu gibi Go ile yazılmış paketleri github'a atabilir ve tüm geliştiricilerin kullanımına açabiliriz.

## Github ile Entegrasyon

Kaynaklardan öğrendiğim kadarı ile Workspace'imizde github için bir klasör ağacı oluşturup senkronize bir şekilde çalışabiliriz de. Böylece commit, rollback gibi standart kaynak kod yönetimi işlemlerini gerçekleştirebiliriz. Ben işleri kolaylaştırmak adına [Github Desktop for Windows](https://desktop.github.com/) sürümünü kullandım. Sistemimde yüklü olan src klasöründe mtrk olarak isimlendirdiğim paket için bir repo oluşturdum. Sonrasında kodu hazırlayıp Commit ederek github'a atılmasını sağladım. Bu işlem sonrasında mtrk klasörü içerisinde github ile ilgili dosyalar da otomatik olarak oluşturuldu.

![gopckg_4.gif](images/gopckg_4.gif)

Ardında mtrk.go kod dosyasının commit ederek github'a yüklenmesini sağladım.

![gopckg_5.gif](images/gopckg_5.gif)

Artık bilgisayarımdaki workspace ile github eşleşmiş durumda. Bu yeni yapıdaki paketi kullanmak için tek yapılması gereken import ifadesini uygun bir şekilde değiştirmek.

```text
package main

import (
	"fmt"
	"github.com/buraksenyurt/mtrk"
	)
```

## Peki ya diğer kullanıcılar?

Onlar bu paketi nasıl kullanabilirler? Yazıya konu ettiğimiz örnek için go get komutunu kullanmamız yeterli aslında. Emin olmak adına sistemimdeki Go Repo'sunu sildim. Daha sonra komut satırından go get komutunu aşağıdaki ekran görüntüsünde olduğu gibi kullandım. Src/github.com/buraksenyurt/mtrk klasörünün oluşuturulduğunu ve içerisine go dosyasının indirildiğini fark ettim.

![gopckg_6.gif](images/gopckg_6.gif)

Ayrıca pkg klasöründe paketin derlenmiş dosyasının da oluşturulduğunu gördüm.

![gopckg_7.gif](images/gopckg_7.gif)

Uygulama kodunu tekrar çalıştırdığımda mtrk paketi ve fonksiyonelliklerinin başarılı bir şekilde yürütüldüğünü gördüm. İşin sağlamasını yapmak için mtrk.go dosyasını silip deneyebilirsiniz. Benim karşılaştığım sonucu alacaksınız.

![gopckg_8.gif](images/gopckg_8.gif)

Görüldüğü üzere Go dilinde paketler ile çalışmak oldukça basit. Atladığım pek çok konu olabilir. Örneğin paketler yüklenirken yapılmasını istediğiniz işlemler varsa bunları init fonksiyonu içerisinde gerçekleştirebilirsiniz ki örneğimizde buna değinmedik. Paket yönetimi için github'ı uzak repository olarak kullanabileceğimizi ve local workspace ile eşleştirerek ilerleyebileceğimizi gördük. Bunlara ek olarak bin,src ve pkg klasör hiyerarşisinden azda olsa bahsettik. Böylece geldik bir Gopher olma maceramızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
