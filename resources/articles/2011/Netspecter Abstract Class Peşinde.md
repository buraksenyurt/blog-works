---
title: "Netspecter Abstract Class Peşinde"
pubDate: 2011-04-07 09:18:00
categories:
  - C#
tags:
  - C#
---

# Netspecter Abstract Class Peşinde
[![blg230_Giris](images/blg230_Giris_thumb.jpg)](images/blg230_Giris.jpg) Yağmur şiddetini giderek arttırıyordu. Karanlık ara sokakta gizemli bir pardesü ise ağır ağır ilerlemekteydi. Etraftaki pis kokunun hemen sokağın başındaki çöp konteynerlerinden geldiği ap açık ortadaydı. Ancak gizemli pardesü bu kokuyu umursamıyordu bile. Bir an durdu ve şüpheli bir şekilde arkasına baktı.

Karanlık içerisinde sadece gözleri belli oluyordu. Kaşlarını çattı ve bir anda irkilerek koşmaya başladı. O kadar paniklemişti ki, koşarken teneke çöp kutularını fark etmedi bile. Önce yere düştü, bir süre yuvarlandı. Çevredeki kediler sağ sola kaçışırken, kalkmaya çalıştı ama önündeki metal iskeleyi hesaplayamadı. Kafasını sert bir şekilde demire çarptı.

Bir kaç saniye sonra gökyüzünden düşen yağmur damlalarını görebiliyor ama seslerini duyamıyordu. Onun yerine kafasında bir uğultu vardı. Etrafında dans eden kod parçları görüyordu. Gözleri yavaş yavaş kararmaya başlamıştı. Derken başında beliren kişiyi gördü…

- Gizemli Pardesü: Netspecter!!! Sen haaa …

- Netspecter: Evet yaaa. Ben…Fazla uzağa kaçamadın eski dostum Abstract Class

![Sarcastic smile](images/wlEmoticon-sarcasticsmile.png)

Merhaba Arkadaşlar,

Netspecter bu kez bir abstract sınıfın peşinde. Sizin içinde eğlenceli bir deneyim olacağına inandığım enteresan bir vakayı analiz etmeye çalışıyor olacağız. Çoğunlukla kod geliştirirken pek fark etmediğimiz bir hata ama hemen çözüm üretebiliyorz. Lakin bu çözümü üretirken istediğimizin dışında bir sonuca da neden olabiliyoruz. Dilerseniz hiç vakit kaybetmeden konumuza geçelim. İlk olarak aşağıdaki kod içeriğini göz önüne alarak başlamamızda yarar olacağı kanısındayım.

[![blg230_BeginingDiagram](images/blg230_BeginingDiagram_thumb.gif)](images/blg230_BeginingDiagram.gif)

```csharp
internal class Composer 
{ 
    public int Duration { get; set; } 
    public string Description { get; set; } 
}

public interface IWriter 
{ 
    Composer CreateComposer(int Duration, string Description); 
}
```

Örnek kod parçasında IWriter isimli bir arayüz (interface) tipi olduğunu görmekteyiz. Bildiğiniz üzere arayüzler Plug-In tabanlı programlamada, servis yönelimli mimarilerde sıklıkla kullanılmaktadır. Arayüzlerin tipik özelliklerinden birisi uygulandıkları tipler için zorlayıcı kuralları bildiriyor olmalarıdır. Ayrıca çok biçimli davranışta gösterebilirler. Bir başka deyişle kendisinde türeyen tipleri taşıyabilir ve çalışma zamanında onlara ait olan fonksiyonellikleri yürüterek kılık değşitirebilirler.

Örneğimizde yer alan IWriter arayüzü ise özel bir duruma neden olmaktadır. Dikkat edileceği üzere CreateComposer metodu geriye Composer tipinden bir nesne örneği döndürmektedir. Söz konusu nesne tipinin erişim belirleyici Internal’ dır. Bu koşullar altında kodu derlediğimizde aşağıdaki ekran görüntüsünde yer alan hatanın oluştuğunu görebiliriz.

[![blg230_FirstError](images/blg230_FirstError_thumb.gif)](images/blg230_FirstError.gif)

Dikkat edileceği üzere IWriter arayüzünün internal olan Composer tipini kullanan bir üyeye sahip olması mümkün değildir. Sorunun çözümü aslında basittir. Hatta her developer hemen bunu yapacaktır. IWriter arayüzünün internal olarak tanımlanması halinde herhangibir derleme hatası ile karşılaşılmayacaktır.

```csharp
internal class Composer 
{ 
    public int Duration { get; set; } 
    public string Description { get; set; } 
}

internal interface IWriter 
{ 
    Composer CreateComposer(int Duration, string Description); 
    // ...kurallara göre bu zaten Public bir üyedir. Ama şu noktada Internal olan bir tipi geriye döndürmektedir. Şüpheli bir durum mudur acaba? 
}
```

Burada yine de şüphe uyandırıcı bir gelişme vardır. Bilindiği üzere Interface üyeleri doğal olarak public’ tir. Buna rağmen doğal olarak public olan CreateComposer metodu Internal erişim belirleyicisine sahip bir örneği döndürmektedir. Oysaki public olan IWriter arayüzünün internal olan Composer tipini kullanmasına izin verilmemiştir

![Confused smile](images/wlEmoticon-confusedsmile.png)

Şimdilik bunu düşünerek kafamızı karıştırmayalım. Çünkü asıl amacımız internal’ a çekmek zorunda kaldığımız IWriter arayüz tipinin public olarak kullanılmasının istenmesidir. Açık olmak gerekirse şu an için bunu da bir kenara bırakalım ve IWriter arayüzünü bir sınıfa bu haliyle uygulamak istediğimizi düşünelim. Örneğin aşağıdaki kod parçasında görüldüğü gibi.

```csharp
public class SpaceWriter 
        : IWriter 
    { 
        #region IWriter Members

        public Composer CreateComposer(int Duration, string Description) 
        { 
            throw new System.NotImplementedException(); 
        }

        #endregion 
    }
```

Görüldüğü üzere SpaceWriter isimli sınıf, IWriter interface tipini implemente etmektedir. Buna göre de CreateComposer metodunu override etmiştir. Herşey yolunda görünmesine rağmen kodu derlediğimizde aşağıdaki hata mesajını almamız kaçınılmazdır.

[![blg230_SecondError](images/blg230_SecondError_thumb.gif)](images/blg230_SecondError.gif)

Yine yine yine…Inconsistent Accessibility hatası

![Annoyed](images/wlEmoticon-annoyed.png)

Burada arayüz tipinin bilinçsiz (Implicitly) olarak uygulandığı görülmektedir. İşte hani bazen interface tiplerini implemente ederken Ctrl+Shift+F10 tuşlarına bastığımızda çıkan seçenekler arasında bir de Explicit olanı vardır ya

![Winking smile](images/wlEmoticon-winkingsmile.png)

İşte o açık bildirim burada bir çözüm olmaktadır.

[![blg230_ExplicitImplementation](images/blg230_ExplicitImplementation_thumb.gif)](images/blg230_ExplicitImplementation.gif)

Yani arayüz implementasyonunu aşağıdaki hale getirirsek kod sorunsuz bir şekilde derleniyor olacaktır.

```csharp
public class SpaceWriter 
    : IWriter 
{ 
    #region IWriter Members

    Composer IWriter.CreateComposer(int Duration, string Description) 
    { 
        throw new System.NotImplementedException(); 
    }

    #endregion 
}
```

Dikkat edileceği üzere metod adının bildirimi sırasında IWriter.CreateComposer (InterfaceName.InterfaceMemberName) isimlendirme notasyonu devreye girmiştir ve derleme hatası ortadan kalkmıştır. Yine de sorun devam etmektedir

![Annoyed](images/wlEmoticon-annoyed.png)

Mecburen IWriter arayüzü internal erişim belirleyicisini kullanmak zorunda kalmıştır. İşte sevgili kahramanımız Netspecter’ ın peşinden koştuğu abstract class bize bu tip bir vaka için çözüm getirebilir. Nasıl mı? İşte sınıf çizelgemiz (Class Diagramı) ve örnek kod parçamız.

[![blg230_FinalDiagram](images/blg230_FinalDiagram_thumb.gif)](images/blg230_FinalDiagram.gif)

```csharp
internal class Composer 
{ 
    public int Duration { get; set; } 
    public string Description { get; set; } 
}

public abstract class AbstractWriter 
{ 
    internal abstract Composer CreateComposer(int Duration, string Description); 
}

public class LogWriter 
    : AbstractWriter 
{ 
    internal override Composer CreateComposer(int Duration, string Description) 
    { 
        throw new System.NotImplementedException(); 
    } 
}
```

![Open-mouthed smile](images/wlEmoticon-openmouthedsmile.png)

Volaaaa… Evet evet biliyorum interface tipi kullanımından vazgeçtik ve abstract class kullanımına geçtik. Ancak bir açıdan baktığımızda aynı amaca hizmet ettiklerini ifade ebiliriz. Söz gelimi gerek interface gerek abstract sınıflar örneklenemezler. Yani new operatörü ile initialize edilemezler

![Winking smile](images/wlEmoticon-winkingsmile.png)

Üstelik her ikiside çok biçimli davranış gösterebilirler. Türetme amacıyla kullanılabilirler ve kendisinden türeyen tiplerin mutlaka uyması gereken kuralları belirtebilirler. Sadece nasıl belirttikleri farklıdır. Tabi arada başka farklarda bulunmaktadır. Söz gelimi arayüz tiplerinde iş yapan bloklara sahip üye bildirimleri yapılamaz veya bu üyeler için erişim belirleyicisi kullanılamaz. Ancak abstract tiplerde durum tam tersidir.

Örnek vakamızın son halinde AbstractWriter abstract sınıfı içerisinde internal olarak işaretlenen CreateComposer metodu olduğu görülmetkedir. Üstelik bu metodun geriye dönüş tipi de internal olan Composer sınıfıdır. Diğer yandan en önemli sonuç AbstractWriter tipinin public olarak işaretlenebilmiş olmasıdır. Yani çok biçimlilik gösteren tipimiz dış ortama public olarak sunulabilmektedir.

Bu vaka çalışmasında her zaman karşımıza çıkmayacak ama hangi durumlarda abstract sınıf kullanımını tercih etmemizi belirleyebilecek bir konuyu ele almaya çalıştık. Sanıyorum artık bir interface tipini implemente ederken Explicitly olan versiyonuna daha bir anlamlı bakıyor olacağız

![Winking smile](images/wlEmoticon-winkingsmile.png)

Tekrardan görüşünceye ve Netspecter’ ın farklı bir macerasında buluşuncaya dek hepinize mutlu günler dilerim.
