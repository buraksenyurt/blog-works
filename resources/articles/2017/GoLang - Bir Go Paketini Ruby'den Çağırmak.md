---
title: "GoLang - Bir Go Paketini Ruby'den Çağırmak"
pubDate: 2017-06-09 12:33:00
categories:
  - GoLang
tags:
  - GoLang
  - Bash
  - Go
  - Python
  - Java
  - Ruby
---

# GoLang - Bir Go Paketini Ruby'den Çağırmak
![gofromruby_1.gif](images/gofromruby_1.gif)

Merhaba Arkadaşlar,

Pek çok kaynak GO dilinin ileride C'nin yerini alabileceği yönünde görüşler belirtmekte. Özellikle IoT alanında bu dilin daha fazla ön plana çıkacağı vurgulanıyor. Bu düşüncenin haklı olabileceği yönünde bazı tespitlerim var.

Söz gelimi GO ile yazılmış paketleri başka dillere ait ortamlarda kullanabilmemiz mümkün. Bir başka deyişle bir GO paketini C, Python, Java ve Ruby gibi dillerde kullanabiliriz. Sonuçta yazılan GO kodları C-Style API şeklinde ifade edebileceğimiz derlenmiş kütüphanelere dönüştürülebilmekte.

Yapılması gereken tek şey, yazılan GO kodlarının C Shared Library türünde derlenmesi. Bu modda yapılan derleme sonucu GO fonksiyonlarını dışarıya açabileceğimiz Shared Object Binary File içerikleri üretiliyor. İşte bu yazımızda basit bir GO kütüphanesini bahsettiğimiz formatta derleyip örnek olarak Ruby ile yazılmış bir kod üzerinden kullanmaya çalışacağız. Dilerseniz vakit kaybetmeden işe koyulalım.

Sistem

Örneği Ubuntu platformu üzerinde denedim. Aslında ilk olarak Windows 7 yüklü makinemde denedim lakin Shared Library oluşturulmasında 64bitlik ortamım sorun çıkarttı. Hemen emektar Ubuntu'ya döndüm ve örneği orada yazmaya karar verdim. Ubuntu tarafında Ruby (2.1.5 i386) ve Go (1.8.3 i386) versiyonları yüklü. Yazıyı yazdığım tarihler itibariyle en son sürümler bunlardı. Ruby tarafı için ihtiyaç duyacağımız FFI isimli bir gem paketi var. Bu paket ile GO tarafında üretilecek olan derlenmiş kod dosyalarını ruby ortamına yükleyip, arayüzden sunulan fonksiyonları kullanabileceğiz. Sonuçta kodlar GO ile yazılmış C kütüphaneleri de olsa bir şekilde diğer çalışma zamanı ortamlarına yüklenerek değerlendirilebilirler. FFI isimli paketi yüklemek için terminalden gem install komutunu kullanabiliriz (Kullanıcım root haklarına sahip olmadığından sudo komutu ile yükleme işlemini gerçekleştirdim)

```cpp
sudo gem install ffi
```

GO Paketinin Yazılması

İşe aşağıdaki GO paketini yazarak başlayabiliriz.

```cpp
package main

import "C"

import (
	"math"
)

//export CircleSpace
func CircleSpace(r float64) float64 {
	return math.Pi * math.Pow(r, 2)
}

func main() {}
```

SomeMath.go ismiyle kaydedeceğimiz kod dosyasında daire alanı hesaplaması yapan tek bir fonksiyon bulunuyor. Siz örneğinizi geliştirirken farklı tipler ile çalışan fonksiyonları da işin içerisine katabilirsiniz. İçerikte dikkat edilmesi gereken bir kaç nokta var. Her şeyden önce C isimli bir GO paketini kullanmaktayız. Bunun haricinde yine bir main fonksiyonu (entry point) bildirimi söz konusu ancak içerisinde iş yapan hiçbir kod parçası bulunmuyor. //export şeklinde belirtilen yorum satırı ise önemli. Nitekim sadece export ile işaretlenmiş fonksiyonlar dış dünyaya açılabiliyorlar. Diğer yandan paylaşımlı olarak kullanılacak kütüphanenin mutlaka main paketi şeklinde düzenlenmesi gerekiyor.

Derleme İşlemi

GO uzantılı kaynak kod dosyası hazır. Bunu direkt go derleyicisi ile derlersek bildiğiniz üzere çalıştırılabilir bir exe üretilir. Oysaki ihtiyacımız olan diğer dillerin ele alabileceği paylaşılabilir bir nesne olmalı (Shared Object) Bu nedenle yazılan paketin komut satırından aşağıdaki gibi derlenmesi gerekiyor.

```bash
go build -o SomeMath.so -buildmode=c-shared SomeMath.go
```

Derleme işlemi sonrasında SomeMath.h isimli C Header ve SomeMath.so isimli bir Shared Object dosyası oluşur. Shared Object içeriği itibariyle daha büyüktür nitekim GO runtime ve gerekli paketler barındırmaktadır. Header dosyasına baktığımızda fonksiyon ve tip bazından bir eşleştirme bilgisi bulundurduğunu görebiliriz. Yaptığımız derleme işlemi sonrası oluşan header içeriği aşağıdaki gibidir.

```cpp
/* Created by "go tool cgo" - DO NOT EDIT. */

/* package command-line-arguments */
/* Start of preamble from import "C" comments.  */
/* End of preamble from import "C" comments.  */
/* Start of boilerplate cgo prologue.  */
#line 1 "cgo-gcc-export-header-prolog"

#ifndef GO_CGO_PROLOGUE_H
#define GO_CGO_PROLOGUE_H

typedef signed char GoInt8;
typedef unsigned char GoUint8;
typedef short GoInt16;
typedef unsigned short GoUint16;
typedef int GoInt32;
typedef unsigned int GoUint32;
typedef long long GoInt64;
typedef unsigned long long GoUint64;
typedef GoInt32 GoInt;
typedef GoUint32 GoUint;
typedef __SIZE_TYPE__ GoUintptr;
typedef float GoFloat32;
typedef double GoFloat64;
typedef float _Complex GoComplex64;
typedef double _Complex GoComplex128;

/*
  static assertion to make sure the file is being used on architecture
  at least with matching size of GoInt.
*/
typedef char _check_for_32_bit_pointer_matching_GoInt[sizeof(void*)==32/8 ? 1:-1];

typedef struct { const char *p; GoInt n; } GoString;
typedef void *GoMap;
typedef void *GoChan;
typedef struct { void *t; void *v; } GoInterface;
typedef struct { void *data; GoInt len; GoInt cap; } GoSlice;

#endif

/* End of boilerplate cgo prologue.  */

#ifdef __cplusplus
extern "C" {
#endif

extern GoFloat64 CircleSpace(GoFloat64 p0);

#ifdef __cplusplus
}
#endif
```

Bu dosya en baştaki yorum satırında da belirtildiği üzere değiştirilmememlidir. Kodun son kısmında CircleSpace fonksiyonuna ait bir bildirim olduğu da gözden kaçmamalıdır. typedef tanımlamalarına bakıldığında ilgili GoFloat64 tipi için double eşleştirilmesi yapıldığını da görebiliriz.

Ruby Tarafından Çağırım

Yazılan GO paketini Ruby tarafından çağırmak için aşağıdaki basit kod parçasını kullanabiliriz.

```text
require 'ffi'

module ShapeMath
	extend FFI::Library
	ffi_lib './SomeMath.so'
	attach_function :CircleSpace, [:double], :double
end

puts ShapeMath.CircleSpace(10)
```

magic.rb isimli dosya ffi gem paketini kullanıyor. ShapeMath isimli bir module tanımı içermekte. Bu modül, ffi_lib metoduna gönderilen SomeMath.so dosyasını yükleyip içerisindeki CircleSpace isimli fonksiyonu çalışma zamanına ekleme işlemini gerçekleştirmekte. Son kod satırında ise CircleSpace isimli metodun çıktısının ekrana basıldığı bir komut yer alıyor. attach_function bildiriminde Ruby dünyası için geçerli olan veri tipleri söz konusu. Go tarafında float64 olarak ifade ettiğimiz tipleri burada double olarak ele almaktayız. İşte çalışma zamanı sonuçları.

![gowithruby_2.gif](images/gowithruby_2.gif)

Elbette işler her zaman bu kadar basit olmayabilir. GO'da var olan bir takım türlerin çağırılmak istenen programlama dilinde karşılığı olmayabilir. Örneğin bir Slice ya da map kulladığımız fonksiyonlar ya da bizim tarafımızdan tanımlanmış yapılar (struct) nasıl eşleştirilmelidir. Ruby'de, Pyhton'da, C# tarafında bu tipler nasıl ele alınmalıdır. Doğru dönüşümleri yapabilmek bu açıdan önemli. Bu "Hello World" tadındaki örnek sadece bu işin yapılabildiğini gösterir niteliktedir. Daha fazlası için Google:) Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
