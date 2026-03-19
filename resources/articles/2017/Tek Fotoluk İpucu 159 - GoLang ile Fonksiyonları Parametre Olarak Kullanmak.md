---
title: "Tek Fotoluk İpucu 159 - GoLang ile Fonksiyonları Parametre Olarak Kullanmak"
pubDate: 2017-05-22 14:30:00
categories:
  - GoLang
tags:
  - GoLang
  - .NET
  - Go
  - Ruby
  - Performance
  - Reflection
  - Delegates
  - Generics
  - GitHub
---

# Tek Fotoluk İpucu 159 - GoLang ile Fonksiyonları Parametre Olarak Kullanmak
Merhaba Arkadaşlar,

GoLang fonksiyonel programlama konusunda oldukça fazla kabiliyete sahip. Birinci sınıf fonksiyonlar (first-class functions), yüksek öncelikli fonksiyonlar (higher-order functions), closures (çeviremedi:)), birden fazla değer döndüren fonksiyonlar (multiple return values), literals ve kullanıcı tanımlı fonksiyon tipleri (user defined function types) bunlar arasında sayılabilir. Neredeyse her gün GO dili ile ilgili bir şeylere bakmaya çalışırken geçenlerde strings paketinde yer alan FiledsFunc fonksiyonunu ile karşılaştım. Derken kendimi ikinci parametresini nasıl kullanıyoruzu anlamaya çalışırken buldum. FieldsFunc fonksiyonu

```cpp
func FieldsFunc(s string, f func(rune) bool) []string
```

şeklinde bir parametre yapısına sahipti. f isimli parametre rune tipinden (aslında int32 ama karakterlerin sayısal ifadesi için kullanılıyor) değer alan ve bool döndüren bir fonksiyondu. Ruby gibi dillerde fonksiyonlara parametre olarak kod bloklarını aktarabileceğimizi, hatta kod bloklarını değişken olarak tanımlayıp kullanabileceğimizi görmüştüm. Go dilinde de benzer fonksiyonellik sunuluyordu. Hatta FieldsFunc ile ilgili github kodlarına baktığımda nasıl uygulandığını da gördüm ([Şu adresten](https://github.com/golang/go/blob/master/src/strings/strings.go) incelemenizi öneririm) Peki bu tip bir fonksiyonelliği ben nasıl yazabilirdim? İşin püf noktası ilgili fonksiyon parametresini bir tip (type) olarak tanımlamak ve parametre olarak fonksiyonda kullanmaktı. Derken aşağıdaki fotoğrafta görülen basit örneği geliştirdim (Üşenmeyin GO'yu sisteme kurup LiteIDE'yi de indirip deneyin)

![tfi159.gif](images/tfi159.gif)

İlk olarak predicate isimli bir fonksiyon tipi tanımladığımızı görebilirsiniz. Bu tip string parametre alıp bool değer döndürecek fonksiyonları ifade edecek şekilde tanımlandı. Select isimli fonksiyon string tipinden bir slice alıyor. İkinci parametre ise predicate tipinden. Dolayısıyla bu fonksiyona parametre olarak string alan ve bool döndüren bir fonksiyonu geçebiliriz. Select içerisinde dönüş içeriğini barındıracak bir slice daha yer alıyor ama asıl önemli olan for döngüsü içerisinde yaptığımız f (word) çağrısı. Bu döngüde words içerisindeki her bir string veriyi f değişkeni ile işaret edilen fonksiyona gönderiyoruz. Sonuç true ise o anki string veriyi yeni slice'a ekleyeceğiz. Bu durumda Select fonksiyonunu kullandığımız yerlere odaklanabiliriz. İki örnek ile ilerledik. İlkinde harf sayısı 5 ve daha fazla olan renkleri buluyoruz. Bu kullanımda Select çağrısını yaparken ikinci parametre ile de fonksiyon bloğunu geçiyoruz. İkinci kullanımda ise strings paketinde yer alan HasPrefix fonksiyonunu kullanarak "g" harfi ile başlayan renkleri yakalıyoruz. Bu kullanımda farklı olan şey ilgili fonksiyonu g isimli bir değişken olarak tanımlamış olmamız.

> Bir.Net geliştiricisi için Predicate, Func, Action temsilcileri (delegate) benzeri imkanlar sunan fonksiyonellikleri yazdığımızı düşünebiliriz.

Tabii kodu daha iyi hale getirmek gerekiyor. Örneğin bu kod parçasındaki Select fonksiyonu sadece string tipinden slice'lar ile çalışmakta. Halbuki bu Select fonksiyonunu daha generic hale getirebiliriz. Bu noktada Interface tiplerini ve reflection konusunu devreye almak gerekiyor. Tabii reflection ve generic bir yapının kullanımı performans üzerine negatif etkilere de sahip olabilir. Bu sevimli araştırmayı siz değerli okurlarıma bırakıyorum. Bir başka ipucunda görüşmek dileğiyle.
