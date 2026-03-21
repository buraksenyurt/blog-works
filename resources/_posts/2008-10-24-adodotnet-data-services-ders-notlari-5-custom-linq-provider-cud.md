---
layout: post
title: "Ado.Net Data Services Ders Notları - 5 (Custom LINQ Provider-CUD)"
date: 2008-10-24 12:00:00 +0300
categories:
  - ado-net-data-services
tags:
  - ado.net-data-services
  - wcf-data-services
  - windows-communication-foundation
---
Ado.Net Data Services konusu ile ilintili bir önceki ders notlarımızda, EDM (Entity Data Model) üzerinden CUD (CreateUpdateDelete) işlemlerinin nasıl yapılabileceğini incelemeye çalışmıştık. Ancak durum özel LINQ Provider kullanımı söz konusu olduğunda biraz daha karmaşıklaşmakta. Nitekim Custom LINQ Provider kullanılması halinde istemci tarafından gelen CUD taleplerine karşılık servis tarafında özel kodlamaların yapılması gerekiyor. Bu noktada ders notlarımız içerisinde belkide çoğumuzun korkup fazla bulaşmak istemediği bir konuya kısacada olsa değineceğimizi şimdiden ifade etmek isterim. Reflection (Yansıma):)

"Hayda brea nereden çıktı bu reflection" diyenelerimiz eminim ki vardır. Öyleyse kısaca bu kavramı hatırlamaya çalışalım. Reflection teknikleri ile çalışma zamanında (Runtime).Net CLR tiplerine ait (ister kullanıcı tanımlı ister önceden tanımlanmış tipler) metadata bilgilerine ulaşılabilmektedir. Bu açıdan bakıldığında özellikle plug-in tabanlı uygulama geliştirmelerde, IDE tasarımlarında kullanılmaktadır. Hatta çalışma zamanında tiplere ait canlı nesne örneklerinin üretilip kullanılması bile mümkündür. Söz gelimi.[Net Reflector](http://www.red-gate.com/products/reflector/) gibi araçlar Reflection teknikleri yardımıyla geliştirilirler. Peki konunun Ado.Net Data Service'ler ile olan ilişkisi nedir? Neden bu tekniklere ihtiyaç vardır?

> Reflection tekniklerinin kalbinde Type isimli tip yer alır. Bu basit tipten yararlanarak çalışma zamanında herhangibir tipe ait bilgileri elde etmek, tiplere ait nesne örnekleri oluşturmak gibi işlemler yapılabilir. Bu basit işlemler ile plug-in uygulamaları, Reflection gibi.Net Assembly'larının içeriğini gösteren programlar yazılabilir. Hatta IDE geliştirmelerindede Reflection tekniklerinden yararlanılmaktadır.

Bu sorunun sorulmasını nedeni servis tarafında Custom LINQ Provider kullanılmasıdır. İstemciler CUD işlemleri için servis tarafına nesne verisi içeren HTTP paketleri gönderirler. Servis tarafında yer alan herhangibir LINQ Provider'ın bu nesne içeriklerini ilgili veri kaynaklarına eklemesi, çıkartması yada değiştirmesi için çalışma zamanının anlayabileceği belirli kurallara uyması gerekir. Böylece herhangibir Custom LINQ Provider alt yapısının CUD işlemlerini gerçekleştirebilmesi bir standart altına alınmış olunur. Öyleyse burada servis tarafında uyulması gereken bir kurallar dizisi söz konusudur.

Bu kurallar öyle bir yapı içerisinde tanımlanmalıdırki.Net CLR'a özgü olmalıdır. İşte bu noktada nasıl bir tip olabilir sorusunu kendinize sormanız gerekmektedir? Interface (Arayüz). Bilindiği üzere bir arayüz, uygulandığı tipin uyması gereken kuralları belirtmektedir. Ancak bunun yanında çok biçimlilik özelliğine sahiptir ve bu nedenle çalışma zamanında kendisini implemente eden tiplere ait nesne örneklerini taşıyabilmektedir. Buda plug-in tabanlı mimarilerde önem arz eden bir konudur. Bu şimdiki senaryomuzda gerekli bir bilgi değildir belki ama arayüzlerin ne işe yaradığınıda anlatan güzel bir tanımlamadır.

> Interface (Arayüz) tipi sadece kendisini uyarlayan tiplerin uyması gereken üye (member) tanımlamalarını içerir. Bunun dışında iş yapan üyeler (members) içermez. Ayrıca polimorfik bir başka deyişle çok biçimliliğe destek verirler. Dolayısıyla bir arayüz tipini kullanarak çalışma zamanında kendisinden türeyen birden fazla nesneyi işaret etmek ve bunların hepsi için ortak işlemler yürütmek mümkündür. Nitekim bu ortak işlemler arayüz içerisinde tanımlanmış olup, arayüzü implemente eden tiplerde yazılma zorunluluğu bulunan ve farklı şekillerde uygulanabilen üyelerdir. (Bu noktada ah keşke şu C# derslerindeki temel konuları biraz daha araştırsaydım diyenleriniz olabilir. Vakit çok geç değil...)

Bu anlatılanlardan özet olarak şu sonucu çıkartabiliriz. Ado.Net Data Service içerisinde kullanılan taşıyıcı varlık tipinin (Container Entity Type) CUD işlemleri için belirli kurallara uyması ve bu kuralları uygulaması gerekmektedir. Bu dayatma için.Net CLR içerisinde System.Data.Services isim alanında yer alan IUpdatable adlı bir interface tipi tanımlanmıştır. Bu arayüzün üye metodları ile CUD işlemleri için gerekli uyarlamaların yapılması istenir. Bakın henüz Reflection tekniklerinden birisini kullanmaktan bahsetmediğimizi belirtelim. Bunu bir nesneyi servis tarafında örneklerken kullanıyor olacağız.

Şu anda neler hissetiğinizi biliyoru ve size hak veriyorum. Bu yazılanlar biraz sıkıcı. Aslında adım adım bir örneğe geçerek ilerlemekte yarar var. Gelin hiç vakit kaybetmeden bir WCF Service uygulaması geliştirelim ve içerisinde Custom LINQ Provider kullanan bir Ado.Net Data Service öğesi oluşturalım. Bu amaçla açılan WCF Service uygulamasına sırasıyla aşağıdaki tipleri entegre etmemiz yeterli olacaktır. İlk olarak servis tarafındaki geliştirmelerimizin genel görünümüne sınıf diagramı görüntüsünden bir bakalım.

Sınıf diagramı (Class Diagram);

![mk262_4.gif](/assets/images/2008/mk262_4.gif)

Öncelikli olarak Entity tipimizi geliştirelim. Product isimli sınıfımız basit olarak bir ürüne ait Id, ad, fiyat, stok miktari gibi bilgileri taşıyacak şekilde tasarlanmıştır.

Product sınıfı;

```csharp
using System;
using System.Linq;

namespace ServerApp
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public double ListPrice { get; set; }
        public int UnitsInStcok { get; set; }
    }
}
```

Product entity tipini içerisinde kullanan ve istemcilere sunan taşıyıcı tipimiz ise aşağıdaki gibidir. Yanlız burada dikkat edilmesi gereken en önemli nokta söz konusu tipin IUpdatable arayüzünü uygulamış olmasıdır.

ShopEntites sınıfı;

```csharp
using System;
using System.Linq;
using System.Reflection;
using System.Data.Services;
using System.Collections.Generic;

namespace ServerApp
{
    // CUD işlemlerine destek vermesi amacıyla ShopEntities tipine IUpdatable arayüzü uyarlanmıştır.
    public class ShopEntities
        :IUpdatable
    {
        // Product tipinden listeyi tutacak generic koleksiyonumuz
        static List<Product> _products;

        // Static yapıcı metod(Constructor) içerisinde bir kereliğine _product koleksiyonu örnek veriler ile doldurulur.
        static ShopEntities()
        {
            _products = new List<Product>
                {
                    new Product{ ProductID=1, Name="Dvd Player", ListPrice=100, UnitsInStcok=100},
                    new Product{ ProductID=2, Name="Mp3 Player 8 Gb", ListPrice=50, UnitsInStcok=150},
                    new Product{ ProductID=3, Name="15.4 inch LCD", ListPrice=190, UnitsInStcok=50},
                    new Product{ ProductID=4, Name="320 Gb Hdd", ListPrice=120, UnitsInStcok=200},
                    new Product{ ProductID=5, Name="1 Tb Hdd 3.5inch", ListPrice=250, UnitsInStcok=175}
                };
        }

        // İstemci tarafına sunulacak olan readonly(yanlız okunabilir) özellik
        public IQueryable<Product> Products
        {
            get
            {
                // AsQueryable<> ile generic List koleksiyonunun sorgulanabilir olması sağlanır
                return _products.AsQueryable<Product>();
            }
        }

        #region IUpdatable Members
    
        // Yeni bir Product nesnesinin oluşturulması ve eklenmesi sırasında devreye girer
        public object CreateResource(string containerName, string fullTypeName)
        {
            // Activator kullanılarak tipin full adından bir nesne oluşturulması sağlanır
            var newProduct = Activator.CreateInstance(Type.GetType(fullTypeName));
            // Oluşturulan nesne örneği Product tipine dönüştürülerek _products isimli generic koleksiyona eklenir
            _products.Add((Product)newProduct);
            // Eklenen yeni nesne örneği metoddan geriy döndürülür
            return newProduct;
        }

        // Silme işlemi sırasında devreye giren metod
        // Parametre olarak silinecek entity nesne örneği gelir
        public void DeleteResource(object targetResource)
        {
            // Gelen nesne örneği Product tipine dönüştürülür ve _products isimli koleksiyondan Remove metodu ile çıkartılır
            _products.Remove((Product)targetResource);
        }

        // Update(güncelleme) işlemi sırasında devreye giren metoddur.
        public object GetResource(IQueryable query, string fullTypeName)
        {
            object r = null;
            var numarator = query.GetEnumerator();
            while (numarator.MoveNext())
            {
                if (numarator.Current != null)
                {
                    r = numarator.Current;
                    break;
                }
            }
            return r; 
        }

        // gelen nesnenin belirtilen özelliğinin değerini geriye döndüren metoddur
        public object GetValue(object targetResource, string propertyName)
        {
            var targetType = targetResource.GetType();
            PropertyInfo targetProperty = targetType.GetProperty(propertyName);
            return targetProperty.GetValue(targetType, null);
        }

        // Gelen nesnenin ilgili özelliğine gelen değeri atayan metoddur.
        // Bu metod veri ekleme ve güncelleştirme işlemleri sırasında ilgili nesne örneğinin her bir özelliği için çalışır
        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            Type targetType = targetResource.GetType();
            PropertyInfo targetProperty = targetType.GetProperty(propertyName);
            targetProperty.SetValue(targetResource, propertyValue, null);
        }

        public object ResolveResource(object resource)
        {
            return resource;
        }

        // Değişikliklerin kaydedilmesini sağlayan metoddur.
        // Söz konusu örnekte veriler bellek üzerinden tutulduğundan uygulanmasına gerek yoktur.
        public void SaveChanges()
        {
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            throw new NotImplementedException();
        }

        public object ResetResource(object resource)
        {
            throw new NotImplementedException();
        } 

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            throw new NotImplementedException();
        }

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            throw new NotImplementedException();
        }

        public void ClearChanges()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
```

Woooww! Evet biraz korkutucu bir implemantasyon. Ama en azından Custom Membership Provider yazarkenki kadar korkutucu değil:) Aslında tüm arayüzün uyarlamasını gerçekleştirmiş değiliz. Nitekim söz konusu örnekte kullanılan Product tipi içerisinde herhangibir navigasyon özelliği bulunmamaktadır. Diğer taraftan sadece CUD işlemleri için gerekli temel metodların uygulandığını ifade edebiliriz. Herşeyden önce CreateResource metoduna dikkat etmemiz gerekiyor. Notlarımızın başında belirtmiş olduğumuz gibi Reflection tekniklerinin burada iyi bir kullanımını görüyoruz.

Nitekim ilk olarak parametre olarak gelen string ifadeden yararlanılarak bir nesne örneği oluşturuluyor ki bu noktada.Net Remoting günlerinden hatırlayacağımız meşhur Activator sınıfı kullanılmakta. Yine GetValue ve SetValue metodları içerisinde parametre olarak gelen bilgilerden yararlanılarak bir özellik değerinin set edilmesi veya elde edilmesi işlemlerinde Reflection teknikleri kullanılıyor. Tabi bizim odaklanmak istediğimiz nokta Ado.Net Data Service içerisinde Custom LINQ Provider kullanılması halinde CUD işlemlerinin nasıl gerçekleşeceği. Buraya kadar yaptıklarımız ile işin zor kısmını az da olsa aştık. Şimdi aşağıdaki svc içeriğine sahip Ado.Net Data Service öğesinide WCF projemize ekleyelim.

```csharp
using System;
using System.Data.Services;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using ServerApp;

public class ShopServices 
     : DataService<ShopEntities>
{ 
    public static void InitializeService(IDataServiceConfiguration config)
    {
        config.SetEntitySetAccessRule("*", EntitySetRights.All);
    }
}
```

CUD işlemeri gerçekleştireceğimiz için SetEntitySetAccessRule metodu içerisinde All sabit değerini kullanıyoruz. Artık istemci tarafını ve istemci için gerekli test kodlarını yazabiliriz. Her zamanki gibi basit bir Console uygulaması işimizi görecektir. Elbette servisimizide Add Service Reference seçeneği ile istemci uygulamaya eklememiz gerekiyor. İstemci tarafında yapılan servis ekleme işlemi sonrasında aşağıdaki sınıf diagramında görülen tipler oluşacaktır.

![mk262_6.gif](/assets/images/2008/mk262_6.gif)

Daha önceki ders notlarımızdan da hatırlayabileceğimiz gibi, veri ekleme işlemleri için AddToProducts metodu oluşturulmuştur. Bunun dışında DataServiceContext üzerinden UpdateObject, DeleteObject metodlarınıda kullanıyor olacağız. Nitekim bu metodlar ile, silme ve güncelleme operasyonları gerçekleştirilecektir. İstemci uygulamamızın kod içeriği ise örnek olarak aşağıdaki gibi geliştirilebilir.

İstemci uygulama kodları;

```csharp
using System;
using System.Linq;
using System.Collections.Generic;
using ClientApp.ShopServiceReference;

// Not: Kod içerisinde Exception kontrolleri yapılmamıştır
namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ShopEntities proxy = new ShopEntities(new Uri("http://buraksenyurt:1000/ServerApp/ShopServices.svc"));

            // Yeni bir Product nesne örneği oluşturulur
            Product newProduct = new Product
                {
                    ProductID = 6,
                    Name = "HP Mouse",
                    ListPrice = 12,
                    UnitsInStcok = 100
                };

            // Nesne örneği eklenir
            proxy.AddToProducts(newProduct);

            // Ekleme operasyonu servis tarafına gönderilir
            proxy.SaveChanges();

            // ProductID değeri 2 olan Product nesnesi istenir.
            var prd = (from p in proxy.Products
                            where p.ProductID==2
                            select p).First<Product>();

            // Bazı özelliklerin değerleri sembolik olarak değiştirilir
            prd.ListPrice += 10;
            prd.UnitsInStcok += 15;
            // Nesne güncellemesi yapılır
            proxy.UpdateObject(prd);

            // Update operasyonu servis tarafına gönderilir
            proxy.SaveChanges();

            // Silme işlemine örnek olması için son eklenen Product istenir
            var lastPrd = (from p in proxy.Products
                                where p.ProductID == newProduct.ProductID
                                select p).First<Product>();
        
            // Nesne silinmesi yapılır
            proxy.DeleteObject(lastPrd);
            // Delete operasyonu servis tarafına gönderilir
            proxy.SaveChanges();        
        }
    }
}
```

Örnekte ilk olarak bir Product nesne örneği oluşturulmakta ve sonra AddToProducts metodu ile ilave edilmektedir. Tabiki bu ekleme işleminin servis tarafına gönderilmesi için, SaveChanges metodunun çağırılması gerekir. Güncelleme işlemi için UpdateObject metodu kullanılmaktadır. Silme işlemi içinse DeleteObject metodu. Elbette her iki işleminde HTTP paketleri içerisine gömülerek servis tarafına gönderilmesi için SaveChanges metodu çağırılmalıdır. İstemci uygulama bu haliyle çalıştırıldığında herhangibir sorun olmadan işlemlerin gerçekleştiği görülür. (Ancak siz bu uygulamayı test ederken mutlaka Debug modda çalıştırın ve küçük bir tavsiye; WCF tarafındaki debug işlemleri için hem servis hemde istemci uygulamayı debug modda çalıştırmalısınız.)

Uygulama test edilirken eğer SaveChanges metodlarında breakpoint'ler ile ilerlenir ve arka planda Fiddler gibi bir Http Debugging Proxy aracı kullanılırsa uygulamanın tamamının çalışması sonrasında aşağıdaki sonuçların elde edildiği görülür.

> [Fiddler](http://www.fiddlertool.com/fiddler/) aracı basit bir HTTP Debugging Proxy uygulamasıdır ve IIS üzerinde 80 numaralı porta gelen ve giden tüm HTTP paketlerini izleyebilmenizi, içeriğini görebilmenizi sağlar. Lakin Asp.Net Development Server ile kullanımında bazı ön ayarlar yapılması gerekmektedir. Fiddler aracının kullanımı Ado.Net Data Services'lerde Batch Processing işlemlerinin ele alındığı [görsel dersimizde](http://www.csharpnedir.com/videoindir.asp?id=121) incelenmiştir.

HTTP Paketlerinin Fiddler aracı üzerinden incelenmesi;

![mk262_7.gif](/assets/images/2008/mk262_7.gif)

Bu durumu biraz analiz edelim. İlk olarak yeni bir Product ekleniyor. Bu ekleme işlemi istemci tarafında yapılan SaveChanges metodu çağrısı sonrasında, servis tarafına bir HTTP paketi olarak gidiyor ki bu POST metoduna göre hazırlanmış bir pakettir. Yine Fiddler yardımıyla paketin içeriğinin aşağıdaki gibi olduğu görülebilir.

Header içeriği;

![mk262_8.gif](/assets/images/2008/mk262_8.gif)

Paket içeriği;

![mk262_9.gif](/assets/images/2008/mk262_9.gif)

Sonrasında ise bir HTTP Get talebi gelmektedir. Nitekim kod içerisinde güncelleme örneği için, ProductID değeri 2 olan ürün bilgisi istenmiştir. Bunun sonucu olarak tabiki istemci tarafına da bir içerik gönderilmektedir. Yine Fiddler aracı yardımıyla bu içeriğe bakılabilir.

![mk262_10.gif](/assets/images/2008/mk262_10.gif)

ProductId değeri 2 olan ürünün istemci tarafına çekilmesinin ardından bir güncelleme işlemi gerçekleştirilir. Koda dikkat edecek olursak bu işlemler için UpdateObject ve ardından SaveChanges metodları çağırılmaktadır. SaveChanges metodu bu kez istemciden servis tarafına HTTP Merge paketi gönderir ve bu paket içerisinde güncelleştirilen yeni değerler yer alır. Buna göre servise gelen Request paketinin Header içeriği aşağıdaki gibidir.

![mk262_11.gif](/assets/images/2008/mk262_11.gif)

Paket ile gönderilen bilgiler ise yine Fiddler aracı ile görülebilir.

![mk262_12.gif](/assets/images/2008/mk262_12.gif)

Son olarak silme operasyonunda önce silinmek istenen veri servis tarafından talep edilmiştir. Bu noktada yine bir HTTP Get çağrısı gerçekleşir. Bu işlemden sonra istemci kodlarında DeleteObject ve ardından yine SaveChanges metodları çağırılmıştır. SaveChanges çağrısı sonrasında ise bu kez bir HTTP Delete talebi istemciden servis tarafına doğru gönderilecektir.

![mk262_13.gif](/assets/images/2008/mk262_13.gif)

Arka planda HTTP paketlerinde neler gittiğini gördük. Burada Fiddler aracına çoğunuzun aşık olduğunu hisseder gibiyim. Aynı SQL Server Profiler gibi son derece başarılı bir izleme aracı.

Gelelim kod tarafındaki metod işleyişlerine. Söz gelimi veri ekleme işlemi sırasında istemci tarafında AddToProducts ve SaveChanges metodlarını kullanıyoruz. Peki ya servis tarafında uyguladığımız IUpdatable arayüzüne ait hangi metodlar devreye giriyor. Bu durumu analiz etmek için Debug modda biraz dolaşmam gerektiğini ifade etmek isterim. Sonunda aşağıdaki sonuçlara ulaşabildim. Tabi söz konusu süreçler yukarıdaki geliştirdiğimiz örneğe göre işlemektedir.

Insert işlemine ait süreç;

![mk262_1.gif](/assets/images/2008/mk262_1.gif)

İstemci tarafında ilk olarak AddTo[EntityName](örneğin AddToProducts) metodu çağırılır. Sonrasında ise SaveChanges metodu yürütülür. SaveChanges metodu insert işlemi için gerekli HTTP paketinin Post metoduna göre servis tarafına gönderilmesinde rol oynar. Bunun karşılığında servis tarafında sırasıyla CreateResource, SetValue, SaveChanges ve ResolveResource metodları çağırılır. CreateResource metodu ile HTTP paketinde gelen istekte yer alan tipin, çalışma zamanında oluşturulması ve ilgili veri kaynağına eklenmesi işlemleri gerçekleştirilir. SetValue metodu ise oluşturulan tipin her özelliği için çalışır. Bir başka deyişle özelliklerin değerlerinin verilmesinde devreye girer. Örnekte veriler bellekteki koleksiyonlarda tutulduğu için geri dönüş değeri olmayan ve parametre almayan SaveChanges metodu içerisinde herhangibir işlem yapılmamıştır.

Update işlemine ait süreç;

![mk262_2.gif](/assets/images/2008/mk262_2.gif)

İstemci tarafında bu kez öncelikli olarak UpdateObject ve sonrasında SaveChanges metodları çağırılır. Servis tarafında ise öncelikli olarak güncellenecek verinin elde edilmesi için GetResource metodu devreye girer. Insert sürecine benzer bir şekilde SetValue metodu güncellenecek özellikler için tek tek çalışır ve yine sırasıyla SaveChanges, ResolveResource metodları devreye girer.

Delete işlemine ait süreç;

![mk262_3.gif](/assets/images/2008/mk262_3.gif)

Silme işleminde istemci tarafında sırasıyla DeleteObject ve SaveChanges metodları çalışır. Bunun karşılığında Delete metodunu içeren bir HTTP paketi servis tarafına gönderilir. Servis tarafında ise yine silinmek istenen verinin elde edilmesi için GetResource metodu ilk olarak devreye girer. Sonrasında ise DeleteResource ile bu verinin kaynaktan çıkartılması sağlanır. Örnekte bir koleksiyon kullanıldığından bu basit bir Remove işleminden öte değildir. Son olarak yine SaveChanges ve ResolveResource metodları çağırılır.

Görüldüğü üzere kendi geliştirdiğimiz Custom LINQ Provider'ların kullanıldığı senaryolarda en kritik nokta IUpdatable arayüzünün kullanılmasıdır. Bu arayüzün yazımızda kullanılan örnek implemantasyonu ile basit CUD işlemleri gerçekeştirilebilir. Böylece geldik Ado.Net Data Service'ler ile ilişkili bir ders notumuzun daha sonuna. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[Örneği indirmek için tıklayın](/assets/files/2008/UsingIUpdatable.rar)