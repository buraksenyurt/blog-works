---
layout: post
title: "GoLang - Unit Test Yazmak"
date: 2017-07-26 21:31:00 +0300
description: "Aranızda birim test(Unit Test) yazmayan hala var mı? diyerek konuya giriş yapmak istiyorum. Yazdığımız atomik fonksiyonelliklerin taşınan ortamlarda başımızı ağrıtmasını istemiyorsak birim testleri mutlaka yazmalıyız. Belki birim testler uygulama geliştirme süresini uzatabilirler ancak uzun vadede kalp krizi geçirme riskini de azaltırlar. Üstelik test senaryoları sayesinde gerçekten ne yapmak istediğimizin farkında olarak da hareket edebiliriz. Eğer test güdümlü yaklaşımla ilerliyorsak bilinçli olarak yaptırılan hata sonrası kodun çalışır hale getirilmesi ve iyileştirilmesi(Refactoring) de önemli kazanımlarımızdır."
categories:
  - golang
tags:
  - golang
  - go
  - testing
  - github
---
# GoLang - Unit Test Yazmak
![gotesting_5.gif](/assets/images/2017/gotesting_5.gif)

Merhaba Arkadaşlar,

Aranızda hala birim test (Unit Test) yazmayan/yazmamış olan var mı? diyerek konuya giriş yapmak istiyorum. Yazdığımız atomik fonksiyonelliklerin taşınan ortamlarda başımızı ağrıtmasını istemiyorsak birim testlerini mutlaka yazmalıyız. Üstelik iyi yazmalıyız. Belki birim testler uygulama geliştirme süresini uzatabilirler ancak uzun vadede kalp krizi geçirme riskini de azaltırlar. Üstelik test senaryoları sayesinde gerçekten ne yapmak istediğimizin farkında olarak da hareket edebiliriz. Eğer test güdümlü yaklaşımla (Test Driven Development) ilerliyorsak bilinçli olarak yaptırılan hata sonrası kodun çalışır hale getirilmesi ve iyileştirilmesi (Refactoring) de önemli kazanımlarımızdır (Red-Green-Blue konusuna bir bakın) En önemlisi de beklenen testleri başarılı bir şekilde aşmış temiz bir kodun ortaya çıkmasıdır.

Neredeyse her programlama dilinin Unit Test yazılmasına yönelik imkanları vardır. Geliştirme IDE'lerinde pek çok kolaylık bulunmaktadır. Çoğu ortam zaten standart kütüphaneler veya paketlerler ile bizleri olabildiğince Unit Test yazmaya yönlendirir. GO tarafında bu iş için dahili paketlerden olan testing kullanılmakta. Pek tabii github üzerinden bulabileceğiniz farklı test paketleri de mevcut. Bu kısa yazımızda basit bir Unit Test'in nasıl yazılabileceğini incelemeye çalışacağız.

Önce Anlamsız İki Fonksiyon

İşe ilk olarak anlamsız iki fonksiyon içeren aşağıdaki kod parçasını yazarak başlayabiliriz (Amacımız GO tarafında Unit Test'lerin nasıl yazıldığını kurcalamak) Operations.go içerisinde daire alanı hesaplayan ve n sayıda float32 tipinden sayının toplamını bulan birer fonksiyon (Variadic) bulunmaktadır.

```cpp
package operations

import (
	"math"
)

func CircleSpace(r float32) float32 {
	return math.Pi * (r * r)
}

func Sum(numbers ...int) int {
	var total int = 0
	for _, n := range numbers {
		total += n
	}
	return total
}
```

Tasarladığımız paketteki CircleSpace ve Sum isimli operasyonlar için birer test metodu yazalım.

Test Paketinin Yazılması

GO'nun alışageldiğimiz kurallarına göre bir paket içerisinde yer alan fonksiyonların testini içeren ayrı bir dosyanın _test şeklinde isimlendirilerek oluşturulması gerektiğini söylesem sanıyorum yadırgamazsınız. Bu bana çok mantıklı geliyor. Paketlerin adlarına baktığımızda kimin test dosyası olduğunu görmemiz kolay. Anlamsal bir bütünlük oluşuyor ve herkes aynı stilde test dosya adı vermek durumunda. Güzel bir standart oluşturulduğu kesin. Örneğimize göre bu dosyanın adı operations_test.go şeklinde olmalı. Farklı bir isim verip test etmek istersek aşağıdaki gibi bir sonuçla karşılaşma ihtimalimiz oldukça yüksek.

![gotesting_3.gif](/assets/images/2017/gotesting_3.gif)

Gelelim operations_test.go içeriğine.

```cpp
package operations

import (
	"testing"
)

func TestCircleSpace(t *testing.T) {
	//var expected float32 = 314.159271
	var expected float32 = 314.15
	calculated := CircleSpace(10)
	if expected != calculated {
		t.Errorf("Test Fail : Calculated [%f]\tExpected [%f]\n", calculated, expected)
	}
}

func TestSum(t *testing.T) {
	//expected := 13
	expected := -1
	calculated := Sum(3, 4, 1, 5)
	if expected != calculated {
		t.Errorf("Test Fail : Calculated [%d]\tExptected [%d]\n", calculated, expected)
	}
}
```

Öncelikle test paketinin adının test edeceğimiz paket ile aynı olduğuna dikkat edelim. Diğer yandan testing paketini de import ediyoruz. TestCircleSpace ve TestSum isimli fonksiyonların testing.T tipinden olan değişken ile test ortamına log bildiriminde bulunmamız mümkün ki bunu Errorf fonksiyon çağrıları ile sağlamaktayız. Test akışı son derece pratik. Beklenen ve hesaplanan değerleri alıp karşılaştırıyoruz. Eğer aynı değillerse testin hatalı sonlandığını ifade edecek şekilde log çıktısı bırakıyoruz. Hepsi bu.

Sonuçlar

İlk olarak senaryomuzu beklenmeyen sonuçlar için test edelim. Bu durumda iki fonksiyon testinin de Fail olmasını bekliyoruz. LiteIDE üzerinden Test seçeneği ile veya komut satırından go test ile gerçekleştirilen işlemlerin sonucu aşağıdaki ekran görüntüsündeki gibi olacaktır.

![gotesting_1.gif](/assets/images/2017/gotesting_1.gif)

Komut satırından test yaparken o klasörde sadece go test yazmamız yeterlidir. Test dosyasının adını vermeye gerek yoktur. _Test uzantısı onu ele veriyor diyebiliriz. Buna göre bir klasörde n sayıda test dosyası varsa tamamını tek seferde çalıştırma imkanımız da olabilir.

![gotesting_4.gif](/assets/images/2017/gotesting_4.gif)

Görüldüğü gibi elde edilen sonuçlar istenen sonuçlar olmamış ve Fail bildirimleri alınmıştır. Şimdi beklediğimiz değerlerin yorum satırlarını kaldıralım. Bu durumda her iki testte başarılı olmalı. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![gotesting_2.gif](/assets/images/2017/gotesting_2.gif)

Dikkat edileceği üzere GO tarafında birim testler yazmak oldukça kolay. O zaman bundan sonraki ilk geliştirmenizde elinizdeki atomikleri önce TDD ilkelerine uyarak yazmaya gayret edin. Hatta FizzBuzz kod katasını baz alıp GO ile yazmayı deneyebilirsiniz. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.