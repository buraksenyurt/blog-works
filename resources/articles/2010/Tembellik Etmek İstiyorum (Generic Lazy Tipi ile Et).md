---
title: "Tembellik Etmek İstiyorum (Generic Lazy Tipi ile Et)"
pubDate: 2010-10-11 12:20:00
categories:
  - C# 4.0
tags:
  - C# 4.0
  - C#
  - .NET
  - Threading
  - Delegates
  - Generics
  - Visual Studio
  - Thread Safety
---

# Tembellik Etmek İstiyorum (Generic Lazy Tipi ile Et)
![blg194_Giris.jpg](images/blg194_Giris.jpg)

Merhaba Arkadaşlar,

Yaz günlerinde pek çok geliştirici tembellik yapmak ister. Hatta benim gibi kocaman bir Üniversite Kampüsü içerisinde yer alan çalışma ortamınız var ise ve kampüsünüzün çimleri üzerinde yatıp şöyle beş on dakika kestirmeye müsaitse. Tabi tembelliğin çeşitli türevleri vardır. Çimler üzerinde uzanmak bunlardan sadece birisi.

Aslında geliştiriciler için Lazy olmanın başka manaları da vardır. Olay sadece çimler üzerinde uzanmaktan ibaret değildir anlayacağınız. İlk akla gelen ORM (Object Relational Mapping) araçlarının birincil özelliklerinden birisi olan Lazy Loading kavramıdır. Kısaca Entity tabanlı nesnelerin/koleksiyonlarının gerektiğinde yüklenmesi şeklinde açıklayabiliriz. Ne varki.Net Framework 4.0 sürümü ile birlikte hayatımıza bir de Lazy tipi girmektedir. Base Class Library içerisine dahil edilen bu yeni generic sınıf sayesinde T tipi için Lazy Initialization işlemi gerçekleştirilebilmektedir.

Bu nokta da Lazy tipinin iki önemli özelliği (Property) olduğunu ifade etmemiz gerekiyor. Value ve IsValueCreated. Aslında Value özelliği ve ToString () metodu çağırılana kadar T tipi ile ilişkili bir yükleme işlemi yapılmadığını söylersek sanıyorum ki olay daha net bir şekilde anlaşılabilir. Ancak konuyu kod yardımıyla irdelemek elbetteki en iyi yoldur.

![Wink](images/smiley-wink.gif)

İlk etapta aşağıdaki Console uygulaması kodlarına sahip olduğumuzu düşünelim.

```csharp
using System.Collections.Generic;

namespace LazyInitializations
{
 class Program
 {
  static void Main(string[] args)
  {
   var productList = GetProducts();
   IncreaseListPrice(productList);
  }

  static void IncreaseListPrice(List<Product> products)
  {
   Console.WriteLine("Liste Fiyatları 1 birim arttırılacak");
   foreach (Product product in products)
   {
    product.ListPrice += 1; 
   }
  }
 
  static List<Product> GetProducts()
  {
   Console.WriteLine("\tProduct listesi oluşturulacak");
   return new List<Product>
   {
    new Product{ProductId=2,Name="Avaya IP Phone", ListPrice=100.99},
    new Product{ProductId=3,Name="Toshiba 106 inc Tv", ListPrice=1200.99},
    new Product{ProductId=4,Name="Hp 6730b Laptop", ListPrice=980.49}
   };
  }
 }
 
 class Product
 {
  public int ProductId { get; set; }
  public string Name { get; set; }
  public double ListPrice { get; set; }
 }
}
```

Bu kod parçasında yer alan GetProducts isimli metod Product tipinden nesne örneklerinden oluşan bir List koleksiyonunu geriye döndürmektedir. Diğer yandan IncreaseListPrice isimli metod da, parametre olarak gelen Product listesindeki her bir ürünün birim fiyatını 1 birim arttırmaktadır. Main metodu içerisinde kod işleyiş sırasına baktığımızda önce List tipinden olan koleksiyonunun doldurulduğunu, sonrasında ise IncreaseListPrice isimli metodun çağırıldığını görebiliriz. Bu örnek uygulamada aslında olağanüstü veya şaşırtıcı bir durum söz konusu değildir. Ancak bizi Lazy Initialization'a götürecek bir nedende olabilir. Söz gelimi bu kod parçasına göre ürün listesinin elde edilmesi beklenenden uzun sürüyor olabilir ve özellikle IncreaseListPrice metod çağrısı öncesinde başka işlerin yapıldığı da düşünülebilir. Çok basit bir şekilde sembolize etmek istersek;

```csharp
var productList = GetProducts();
// Bir takım Business çağrılar yapıldığını düşünelim
IncreaseListPrice(productList);
```

Buna göre aslında ürün listesinin (List) sadece işlem yapılacağı yerde yüklenmesini, başlangıçta yüklenerek zaman kaybına neden olunmamasını isteyebiliriz ki bu durumda Business çağrıların bekletilmemesi de sağlanmış olacaktır. İşte böyle bir durumda Lazy tipine ait bir nesne örneği gerekli tembelliği bize sunabilir. Nasıl mı?

```csharp
static void Main(string[] args)
{
 // var productList = GetProducts();
 Lazy<List<Product>> lazyProductList = new Lazy<List<Product>>(
    () => GetProducts()
   );
 // Bir takım Business çağrılar yapıldığını düşünelim
 Console.WriteLine("Product listesi yüklendi mi?  {0}",lazyProductList.IsValueCreated?"Evet":"Hayır");
 IncreaseListPrice(lazyProductList.Value);
 Console.WriteLine("Product listesi yüklendi mi? {0}", lazyProductList.IsValueCreated ? "Evet" : "Hayır");
 // IncreaseListPrice(productList);
}
```

İlk olarak Lazy> tipinden bir nesne örneği oluşturulduğu görülmektedir. Buna göre T olarak belirtilen List tipinden nesne örneği döndürecek bir operasyon için Lazy Initialization işleminin uygulanacağı bildirilmektedir. Lazy tipinin bir kaç aşırı yüklenmiş yüklenici metodu (Overloaded Constructor) bulunmaktadır. Örnekte Func temsilcisini (Delegate) kullanan versiyon ele alınmıştır. Tahmin edileceği üzere burada GetProducts isimli metod çağırısı gerçekleştirilmektedir. İzleyen satırda bir takım Business işlemlerin yapıldığı düşünüldüğünde Product listesinin yüklenmesi işlemlerinin, bu operasyonu engellemediği düşünülebilir. Nitekim henüz Product listesi yüklenmemiştir. Bunu kod tarafında anlamanın yolu Lazy nesne örneğinin IsValueCreated özelliğine bakmtaktır. Diğer yandan uygulamayı Debug ettiğimizde tam bu noktada durduğumuz takdirde aşağıdaki ekran görüntüsünü elde ederiz.

![blg194_Debug1.gif](images/blg194_Debug1.gif)

IsValueCreatted özelliği, Lazy nesne örneğinin Value özelliği kullanılana kadar False değere sahiptir. Value özelliğinin çağırılması artık yapıcı metod içerisinde Func temsilcisi ile bildirilen metodun icra edilmesi anlamına gelmektedir. Bir başka deyişle söz konusu Product listesinin yüklenmesi işlemi gerçekleştirilecektir. Bu durumda kod tarafında IsValueCreated özelliği true değere sahip olacaktır. Yine Debug penceresinden olaya baktığımızda aşağıdaki ekran çıktısı ile karşılaştığımızı görebiliriz.

![blg194_Debug2.gif](images/blg194_Debug2.gif)

Dikkat edileceği üzere Value özelliği 3 adet Product nesne örneğini işaret etmektedir.

Lazy kullanımında dikkate değer noktalardan biriside Value özelliğine ilk çağrıdan sonra tekrar erişilmesi halidir. Durumu aşağıdaki kod parçası ile irdeleyebiliriz.

```csharp
...
IncreaseListPrice(lazyProductList.Value);
Console.WriteLine("Product listesi yüklendi mi? {0}", lazyProductList.IsValueCreated ? "Evet" : "Hayır");
// IncreaseListPrice(productList);
var productList = lazyProductList.Value;
IncreaseListPrice(productList);
```

Burada IncreaseListPrice metodunun parametresinde Lazy nesne örneği için bir Value çağrısı yapılmaktadır. Bu Value özelliği için yapılan ilk çağrı olduğundan GetProducts () metodunun çağırılması işlemi bu noktada yapılmaktadır. Ancak son iki satırda Value özelliği tekrardan kullanılmıştır. İlk olarak productList isimli bir değişkene değer ataması gerçekleştirilmiş arından elde edilen değer IncreaseListPrice metoduna parametre olarak geçirilmiştir. Burada Value özelliğine bir çağrı yapılması nedeni ile GetProducts isimli metodun bir kere daha çalıştırılacağı düşünülebilir. Ama böyle olmayacaktır. Bunu debug sırasında görebileceğimiz gibi kodun çalışma zamanı çıktısında da fark edebiliriz.

![blg194_Runtime.gif](images/blg194_Runtime.gif)

Görüldüğü gibi "Product Listesi Oluşturulacak" ifadesi sadece bir kere çağırılmıştır. Son olarak Lazy tipinin Thread Safety'yi varsayılan olarak sunduğunu bu nedenle Concurrent operasyonlarda ele alınabileceğini de belirtelim. Base Class Library içerisine.Net Framework 4.0 ile birlikte gelen pek çok yeni tip söz konusudur. Lazy bunlardan sadece bir tanesiydi. İlerleyen zamanlarda diğer yenilikleri de incelemeye çalışıyor olacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[LazyInitializations_.rar (27,13 kb)](assets/LazyInitializations_.rar) [Örnek Visual Studio 2010 Ultimate sürümü ile geliştirilmiş ve test edilmiştir]
