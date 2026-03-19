---
title: "Tek Fotoluk İpucu 128 - IFormattable ile Sihir"
pubDate: 2016-01-13 19:00:00
categories:
  - C#
tags:
  - C#
  - .NET
  - XML
  - JSON
  - JavaScript
  - Serialization
---

# Tek Fotoluk İpucu 128 - IFormattable ile Sihir
Merhaba Arkadaşlar,

Bildiğiniz üzere.Net Framework çatısı altında bir çok arayüz (Interface) vardır. Özellikle plug-in tabanlı geliştirmelerde sıklıkla başvurduğumuz bu arayüzleri zaman zaman inceliyor ve nerelerde kullanılabileceğine bakıyorum. Yine bu araştırmaları yaptığım bir gün IFormattable arayüzünün kullanımına ilişkin örnekler ile karşılaştım.

Senaryomuz şu; bir nesne örneğini String sınıfının Format metodu ile kullanırken {0:JSON} gibi bir ifade kullanabilmek ve çalışma zamanı içeriğini JSON (JavaScript Object Notation) formatında elde etmek istiyoruz. Hatta {0:ALL}, {0:ID,Title} gibi ifadelere de yer vermek istiyoruz.

İşte bu ifadede {0: dan sonra gelen parça, IFormattable arayüzünü kullanarak String sınıfının Format metoduna öğretebileceğimiz kısım. Nasıl mı? Aynen aşağıdaki fotoğrafta görüldüğü gibi.

![TFI_127.gif](images/TFI_127.gif)

Kodun çalışma prensibi oldukça basit. Product sınıfı IFormattable arayüzünü (Interface) uyguladığında ToString metodunun ezilmesi gerekiyor. Tabi ezilen bu ToString metodu String.Format tarafından kullanılmakta (Lütfen kodu debug edip içerisinde basitçe gezinin) ToString metoduna gelen format değişkeni {0: dan sonraki kısmı işaret etmektedir. Buna gelen değerlere göre bir switch bloğu çalışmış ve istenen string içerikler geriye döndürülmüştür.

Tabii kod içerisinde dikkat edilmesi gereken bir takım hususlar var. Örneğin Object tipinden gelen ToString metodu kullanılmak istendiğinde çalışma zamanı nasıl bir çıktı üretir. Yani product nesne örneği üzerinden ToString () çağırırsak ne olur? Bir şey olmazsa olması için ne yapılabilir? case "XML": sonrasında bir XML serileştirme yapılabilir. Tabii geriye string olarak dönülmelidir. {0: sonrası için farklı ifadeler de ele alınabilir. Bu kutsal görevi de siz değerli okurlarıma bırakıyorum. Böylece geldik bir ipucunun daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
