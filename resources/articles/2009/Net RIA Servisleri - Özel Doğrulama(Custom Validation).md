---
title: ".Net RIA Servisleri - Özel Doğrulama(Custom Validation)"
pubDate: 2009-05-31 04:03:00
categories:
  - .Net RIA Services
tags:
  - .Net RIA Services
  - C#
  - Silverlight
  - Authentication
---

# .Net RIA Servisleri - Özel Doğrulama(Custom Validation)
Merhaba Arkadaşlar,

Bir önceki blog yazımızda,.Net RIA Servislerin kullanıldığı Silverlight uygulamalarında doğrulama (Validation) işlemlerinin nasıl yapılabileceğini incelemeye çalışmıştık. Bu yazımızda ise, Range, Required, StringLength, RegularExpression gibi built-in niteliklerle (attribute) gerçekleştirilen doğrulamalar haricinde kalan özel durumlar için nasıl ilerleyebileceğimizi araştıracağız. Konuyu adım adım irdelersek, aşağıdaki işlemleri yapmamız gerekmektedir.

- Sunucu uygulama tarafında (Web App) shared niteliği ile işaretlenmiş bir sınıf tasarlanır ve içerisine özel doğrulama operasyonları ilave edilir.
- Özel doğrulamaların uygulanacağı sınıf veya üyelerine, CustomValidation niteliği yardımıyla geliştirilen Validator tipi bildirilir.

Gördüğünüz gibi gayet basit.

![Wink](images/smiley-wink.gif)

Önceki blog yazımızda geliştirdiğimiz örnek proje için bu adımları uygulamaya başlayabiliriz. Örnek olarak ProductName alanı için özel bir doğrulama fonksiyonelliği geliştireceğiz. Bu doğrulamaya göre, ProductName ile ilişkili veri giriş alanı içerisinde Select, Where, Delete gibi SQL kelimelerinin olmamasını sağlamaya çalışacağız. Bu tabiki konunun anlaşılması için öne sürdüğümüz bir senaryo. Şu an için önemli olan, tekniğin nasıl uygulandığıdır. Bu amaçla web projesi tarafında ProductNameValidator.shared.cs isimli bir kod dosyası oluşturarak işe başlayabiliriz. Bu kod dosyasının adında shared kelimesinin eklenmesinin geliştirme ortamı (IDE) içinde özel bir anlamı vardır. SınıfAdı.shared.cs/vb formatında yazılan dosya adı sayesinde, istemci tarafı içinde otomatik kod üretiminin gerçekleştirilmesi sağlanmış olmaktadır.

Söz konusu sınıfın kod içeriği ise aşağıdaki gibidir.

```csharp
using System.ComponentModel.DataAnnotations;
using System.Web.Ria.Data;

// Dosya adında shared kullanılmasının bir nedeni vardır. Bu isimlendirme standardı sayesinde, derleme zamanı alt yapısının istemci tarafı için otomatik dosya üretimi gerçekleştirmesi sağlanmış olunur.
namespace ValidationSystem.Web
{
    [Shared] 
    public static class ProductNameValidator
    {
        public static bool QueryCheck(string productName, ValidationContext context, out ValidationResult result)
        {
            // sembolik olarak eklenmiş kontrol değerleri
            string[] keywords = { "Select", "Where", "Delete", "Create" };

            // ürün adının, yasaklı kelimelerden herhangibirini içerip içermediğine bakılır.
            foreach (string keyword in keywords)
            {
                // Bir tane bile içeriyorsa, ValidationResult nesne örneği oluşturulurken hata mesajı bildirimi yapılır ve geriye false değer döndürülür
                if (productName.Contains(keyword))
                {
                    result = new ValidationResult("Tehlikeli kelimeler yer almakta");
                    return false;
                }
            }
            // Eğer doğrulama işlemi başarılıysa ValidationResult nesne örneğine null değer atanır ve geriye true değer döndürülür
            result = null;
            return true;
        }
    }
}
```

Static olarak tanımlanan sınıfın shared niteliği ile imzalandığına dikkat edilmelidir. Diğer taraftan doğrulama işlemi için kullanılacak olan metod (metodlar), ilk parametre olarak doğrulanacak veri içeriğini taşıyabilecek tipte bir değişken kullanırlar. ProductName alanı tablo üzerinde nvarchar tipinden tanımlanmış ve bu nedenle Entity içerisinde string olarak ele alınmıştır. Dolayısıyla ilk parametrenin string tipinden tasarlanmış olması doğru bir tercihtir. Diğer taraftan ikinci parametre olarak ValidationContext ve üçüncü parametre olarakta ValidationResult tiplerinden değişkenler tanımlanmıştır. Her ne kadar örneğimizde ValidationContext parametresini kullanmamış olsakta, çalışma zamanında doğrulamaya tabi olan içeriğin sahibi tipe ait bilgileri içerdiğini söyleyebiliriz. Dolayısıyla bu değişken ile, doğrulamaya tabi olan ProductName değerine sahip Products nesne örneğine ulaşabilir ve doğrulamayı farklı açılardan ele alabiliriz. Aşağıdaki ekran görüntüsünde bu durum daha net bir şekilde görülebilmektedir.

![blg25_1.gif](images/blg25_1.gif)

Gelelim ValidationResult tipine. Sonuç olarak doğrulamanın başarılı veya başarısız olma durumu söz konusudur. Başarısız olunması halinde, istemci tarafında hata mesajı gibi bilgileri içeren bir nesne örneğinin var olması gerekmektedir. İşte ValidationResult nesne örneğinin üretilmesi ile, doğrulamanın başarısız olması durumunda geriye nasıl bir bilgi döndürüleceği belirtilmektedir. Tabi metodun böyle bir durumda geriye false değer döndürmeside gerekmektedir. Elbetteki doğrulama işleminin başarılı olması halinde geriye true değer döndürülmesi ve ayrıca ValidationResult nesne örneğinin null olarak aktarılması sağlanmalıdır.

Sırada ikinci adım var. Geliştirilen bu doğrulama tipinin, çalışma zamanı tarafından ele alınması gerekmektedir. Tabiki hal böyle olunca devreye niteliklerin (attribute) girmeside kaçınılmazdır. Neyseki kendi niteliklerimizi yazmak yerine, herhangibir validator tipini, istediğimiz özellik veya sınıfa uygulamamızı sağlayan tek bir built-in nitelik mevcuttur.

![Wink](images/smiley-wink.gif)

CustomValidation. Dolayısıyla metadata dosyası içerisinde, ProductName özelliğinin aşağıdaki hale getirilmesi yeterli olacaktır.

```csharp
[Required(ErrorMessage="Lütfen ürün adını giriniz")]
[CustomValidation(typeof(ProductNameValidator),"QueryCheck")]
public string ProductName;
```

CustomValidation niteliği ilk parametre olarak doğrulama tipini almaktadır. İkinci parametrede ise, takip öden özelliğin (veya sınıfın) kontrolünü gerçekleştirecek olan metod adı belirtilmektedir. Uygulama bu son haliyle derlendiğinde, istemci projesindede aşağıdaki şekilde görülen ek dosyanında üretildiği gözlemlenebilir.

![blg25_2.gif](images/blg25_2.gif)

Artık uygulamayı test etmeye başlayabiliriz. Bu amaçla herhangibir ürünün güncellenmeye çalışıldığını düşünelim ve ürün adında Delete kelimesini kullandığımızı varsayalım. İşte sonuç...

![blg25_3.gif](images/blg25_3.gif)

Tataaaa!!!

![Laughing](images/smiley-laughing.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ValidationSystem2.rar (1,86 mb)](assets/ValidationSystem2.rar)
