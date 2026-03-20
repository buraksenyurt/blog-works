---
layout: post
title: "Caching Application Block Merakı"
date: 2009-06-20 17:52:00 +0300
categories:
  - enterprise-library
tags:
  - enterprise-library
  - csharp
  - xml
  - dotnet
  - aspnet
  - linq
  - wcf
  - windows-forms
  - http
  - authentication
  - performance
  - caching
  - serialization
  - generics
  - visual-studio
---
Az önce 1966 yılında çevrilmiş olan ve küçüklüğümde bol bol izlediğim nefis bir filmi belkide 179ncu kez tekrardan seyrettim. Eskiler aşağıdaki resimden hangi film olabileceğini tahmin edebilirler. Yeni nesilden seyretmeyen varsa eğer [The Good, The Bad and The Ugly](http://en.wikipedia.org/wiki/The_Good,_the_Bad_and_the_Ugly) filmini mutlaka tedarik edip izlesinler. Peki bunun anlatacağım konu ile bir ilgisi var mı? Hayır yok.

![blg34_1.jpg](/assets/images/2009/blg34_1.jpg)

![Cool](/assets/images/2009/smiley-cool.gif)

Sadece off-topic bir giriş yapmak istedim.

Bu yazımda sizlere bahsetmek istediğim konu bir süredir boş vakitlerimde araştırıp incelediğim Caching Application Block yapısıdır. Açık kaynak olarak sunulan [Enterprise Library](http://www.microsoft.com/Downloads/details.aspx?familyid=1643758B-2986-47F7-B529-3E41584B6CE5&displaylang=en)ürün ailesinin bir parçası olan bu bloğu, uygulamalarımızda performansı arttırmak adına ele alabiliriz. Bilindiği üzere günümüz uygulamalarında sıklıkla tekrar eden pek çok kıstas (Concern) bulunmaktadır. Örneğin hata yönetimi (Error Handling), loglama (Logging), şifreleme işlemleri (Cryptography), güvenlik (Security), doğrulama (Validation) veya veri erişim işlemleri (Data Access) bu kıstaslara örnek olarak gösterilebilir. Haliyle bu kavramlar çoğunlukla uygulamadan bağımsız olmaktadır. Nitekim uygulama çeşidi değişse bile, bu kıstasların bir kısmını veya tamamını kullanmak zorunda kalabiliriz.

Öyleyse uygulamalara bu kıstasların enjekte edilmesi sırasında gerekli hazırlıkları tekrar tekrar yapmamıza gerek bırakmayacak modellere ihtiyacımız vardır. (Aspect Oriented Programming modelininde çözüm getirdiği sorunlardan birisidir bu aslında.) Diğer taraftan, Enterprise Library gibi kütüphaneleri ele alaraktanda, bu kıstasların tamamını veya bir kısmını istediğimiz uygulamalarda değerlendirebiliriz. Bu sayede sürekli tekrar eden temel işlemlerle uğraşmak zorunda kalmayarak tamamen iş mantığına odaklanmamız mümkün olabilir. Üstelik Enterprise Library açık kaynak kodlu olduğundan, dilersek genişletebilir veya özelleştirebiliriz.

Caching Application Block'un nasıl kullanıldığını anlatmak için oldukça hevesliyim.(Dilerseniz benden çok daha iyi bir öğretici olan [Hands-On-Lab'](http://www.microsoft.com/Downloads/details.aspx?familyid=AB3F2168-FEA1-4FC2-B40C-7867D99D4B6A&displaylang=en)leride kullanabilirsiniz ki şiddetle tavsiye ederim) İlk etapta bize örnek bir senaryo gerekiyor. Ben elimde bir WCF servisi olduğunu, bu servisin XML tabanlı bir veri kaynağını kullanarak istemcilere ürün bilgilerini verdiğini düşündüm. Bununla birlikte, ürünlerin resim bilgileride fiziki dosyalarda ve sunucu tarafında yer almaktadır. İstemciler, listeleri çektikten sonra dilerlerse istedikleri bir ürünün resmini servisten talep edebilmektedir. Tabiki bu senaryoda performansı etkiliyen farklı faktörler vardır.

Herşeyden önce, servis yönelimli bir uygulama söz konusu olduğundan, istemci ve sunucu arasında taşınacak resim ve boyutu iletişim hızını ve performansını doğrudan etkileyecektir. (Burada performans için resmin MIME protokolüne göre taşınması veya parçalara bölünerek aktarılması düşünülebilir.) Diğer taraftan, istemcinin bir ürün resmini talep etmesi durumunda, sunucu tarafında resmin fiziki dosyadan tedarik edilerek istemci tarafına gönderilecek şekilde hazırlanması durumunda da performans kaybı söz konusu olacaktır. İşte biz Caching Application Block yapısını, servis tarafında resmin hazırlanması aşamasında ele alabiliriz. Yinede şunu vurgulamakta yarar vardır. WCF servisi web tabanlı olarak yayınlandığında pekala Asp.Net motorunun var olan önbellekleme modüllerinide kullanılabilir. Dolayısıyla senaryomuzu sadece Caching Application Block'un kullanımını öğrenmek amacıyla geliştirdiğimizi göz önüne almamızda yarar vardır. Hatta istemcinin asenkron olarak erişmesi veya servis tarafında paralel hesaplamalar yapılması gibi senaryoları hiç işin içerisine katmıyoruz bile.

Önce servis tarafını ele alalım. Servis tarafında veri kaynağı olarak Products.xml isimli bir dosya kullanılmaktadır.

![blg34_2.gif](/assets/images/2009/blg34_2.gif)

XML içeriğinde basit olarak ürünün Id değeri, adı, liste fiyatı ve resim dosyası adı bilgileri tutulmaktadır. Resimler ise WCF Service uygulamasında ProductImages isimli bir fiziki klasörde yer almaktadır.

![blg34_3.gif](/assets/images/2009/blg34_3.gif)

XML içeriğini istemci tarafında sunarken Product isimli serileştirilebilir bir tipten yararlanıyor olacağız.

```csharp
using System.Runtime.Serialization;

namespace ProductServices
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public decimal ListPrice { get; set; }
    }
}
```

Servis sözleşmesi ise aşağıdaki kod parçasında olduğu gibidir.

```csharp
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;

namespace ProductServices
{
    [ServiceContract]
    public interface IPhotoService
    {
        [OperationContract]
        List<Product> GetProducts();

        [OperationContract]
        byte[] GetPhoto(int productId);
    }
}
```

Sözleşmeyi uygulayan tipimizin kodları ise şu şekildedir.

```csharp
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ProductServices
{
    public class PhotoService 
        : IPhotoService
    {
        public List<Product> GetProducts()
        {            
            XDocument doc = XDocument.Load(ConfigurationManager.AppSettings["XmlSourcePath"]);

            List<Product> products = (from p in doc.Element("Products").Elements("Product")
                          select new Product
                          {
                              ProductId=Convert.ToInt32(p.Element("Id").Value),
                              Name=p.Element("Name").Value,
                              ListPrice=Convert.ToDecimal(p.Element("ListPrice").Value)
                          }).ToList<Product>();

            return products;
        }

        #region IPhotoService Members

        public byte[] GetPhoto(int productId)
        {
            XDocument doc=XDocument.Load(ConfigurationManager.AppSettings["XmlSourcePath"]);

            string imageFileName = (from p in doc.Element("Products").Elements("Product")
                                   where p.Element("Id").Value == productId.ToString()
                                   select p.Element("ImageFileName").Value).Single();

            string imagePath = Path.Combine(ConfigurationManager.AppSettings["ImagesPath"], imageFileName);

            return File.ReadAllBytes(imagePath);
        }

        #endregion
    }
}
```

Görüldüğü üzere, XML içeriğini sorgulama kısımlarında XLINQ ifadelerinden yararlanılmaktadır. GetProducts metodu, List koleksiyonu tipinden bir referans döndürmektedir. Bununla birlikte herhangibir ürünün resmi, GetPhoto metodu yardımıyla byte[] dizisi olacak şekilde üretilmektedir. Products.xml dosyası ve resimlerin kök adres bilgileri web.config dosyasında appSettings kısmında tutulmaktadır. Bu nedenle söz konusu konfigurasyon bilgilerin alınabilmesi için, ConfigurationManager tipinden yararlanıldığı görülmektedir. Servisimiz basicHttpBinding bağlayıcı tipi üzerinden sunulmaktadır. Dolayısıyla web.config dosyasındaki servis ayarları aşağıda görüldüğü gibidir.

![blg34_4.gif](/assets/images/2009/blg34_4.gif)

Peki ya istemci tarafı? Bu uygulamayı aşağıdaki tasarıma sahip basit bir WinForms programı olarak düşünebiliriz aslında.

![blg34_5.gif](/assets/images/2009/blg34_5.gif)

İstemci uygulamaya servis referansının eklenmesinin ardından kodlarıda aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClientApp.ProductServicesRef;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        PhotoServiceClient proxy = null;

        public Form1()
        {
            InitializeComponent();
            proxy = new PhotoServiceClient();
        }

        private void btnGetProductList_Click(object sender, EventArgs e)
        {
            grdProducts.DataSource=proxy.GetProducts();
        }

        private void grdProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int productId = Convert.ToInt32(grdProducts[2, e.RowIndex].Value);

            byte[] byteArray= proxy.GetPhoto(productId);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                pcbImage.Image = Image.FromStream(stream);
            }
        }
    }
}
```

Kullanıcılar GetProductList düğmesini kullanarak servisten ürün listeni çekmektedir. Çekilen ürünler DataGridView kontrolüne aktarılmaktadır. Grid üzerinden herhangibir satıra tıklanıldığında ise seçilen ürünün ProductId değerine göre servisten resim bilgisi talep edilmektedir. Aşağıda çalışma zamanındaki örnek sonuçlardan birisi yer almaktadır.

![blg34_6.gif](/assets/images/2009/blg34_6.gif)

Burada hemen şu noktayı vurgulamak isterim. İstemci tarafına gönderilen byte dizisinin boyutu, yine istemci tarafındaki app.config dosyasında, readerQuotas elementi içerisindeki maxArrayLength özelliği ile sınırlandırılmıştır. Resimlerin boyutlarına göre bu değerin arttırılması gerekebilir ki benim örneğimde bu arttırım yapılmak zorunda kalınmıştır.

![Wink](/assets/images/2009/smiley-wink.gif)

Nihayetinde ön hazırlıklarımız tamamlanmıştır. Artık servis tarafında kullanmak istediğimiz Caching Application Block ile ilişkili hazırlıklara başlayabiliriz. Öncelikle servis uygulamasına kullanılmak istenen Enterprise Library bloğu ile ilgili assembly referansının eklenmesi gerekmektedir.

![blg34_7.gif](/assets/images/2009/blg34_7.gif)

Böylece kod içerisinde, Caching Application Block ile ilişkili yönetimli kodlar ele alabileceğiz. Caching Application Block, ön belleğe alma işlemlerinde varsayılan olarak kullanıldığı host uygulamanın çalıştığı sistem belleğini ele almaktadır. Ancak istenirse saklama işlemleri için farklı bir kaynağın (örneğin fiziki disk) kullanılması sağlanabilir. Bunula birlikte, ön bellekte tutulacak maksimum eleman sayısınıda belirleyebiliriz. İyi ama bu ayarları nerede yapacağız?

![Undecided](/assets/images/2009/smiley-undecided.gif)

Tahmin edeceğiniz üzere host uygulamanın konfigurasyon dosyası içerisinde. Neyseki Enterprise Library kurulumlarından sonra, Visual Studio 2008 için görsel bir arabirim gelmektedir. Böylece gerekli ayarları kolayca yapabiliriz.

İlk etapta servis uygulamasındaki web.config dosyasını Edit Enterprise Library Configuration ile açalım. (İstenirse tüm ayarlamalar konfigurasyon dosyası içerisinden ellede yapılabilir.)

![blg34_8.gif](/assets/images/2009/blg34_8.gif)

Sonrasında ise Caching Application Block için gerekli XML sekmesini aşağıdaki şekildende görülebileceği gibi kolayca ilave edebiliriz.

![blg34_9.gif](/assets/images/2009/blg34_9.gif)

Bu işlemlerin ardından varsayılan olarak web.config dosyasının içeriği aşağıdaki gibi olacaktır.

```xml
<cachingConfiguration defaultCacheManager="Cache Manager">
    <cacheManagers>
      <add expirationPollFrequencyInSeconds="60" maximumElementsInCacheBeforeScavenging="1000"
        numberToRemoveWhenScavenging="10" backingStoreName="Null Storage"
        type="Microsoft.Practices.EnterpriseLibrary.Caching.CacheManager, Microsoft.Practices.EnterpriseLibrary.Caching, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
        name="Cache Manager" />
    </cacheManagers>
    <backingStores>
      <add encryptionProviderName="" type="Microsoft.Practices.EnterpriseLibrary.Caching.BackingStoreImplementations.NullBackingStore, Microsoft.Practices.EnterpriseLibrary.Caching, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
        name="Null Storage" />
    </backingStores>
  </cachingConfiguration>
```

Tabiki özellikler penceresindende pek çok ayarlama yapılabilir. Söz gelimi Protection Provider ile ön bellekte tutulacak nesnelerin şifrelenmesi için hangi sağlayıcının kullanılacağı belirlenebilir (şu andaki örneğimizde herhangibir şifreleme sağlayıcısı kullanılmamaktadır). Peki bunlar yeterli midir? Elbetteki değildir. Ön bellekte kimi tutacağız? Ön bellekte tutmak istediğimiz nesne referansını nasıl ekleyecek veya nasıl çekeceğiz? Bu durumda GetPhoto metodunun içeriğini aşağıdaki gibi düzenlememiz yeterli olacaktır.

```csharp
public byte[] GetPhoto(int productId)
{
	XDocument doc=XDocument.Load(ConfigurationManager.AppSettings["XmlSourcePath"]);

	string imageFileName = (from p in doc.Element("Products").Elements("Product")
						   where p.Element("Id").Value == productId.ToString()
						   select p.Element("ImageFileName").Value).Single();

	string imagePath = Path.Combine(ConfigurationManager.AppSettings["ImagesPath"], imageFileName);

	byte[] imageBytes = null;

	// İlk olarak çalışma zamanında, CacheManager referansı çekilir. Bu noktada fabrika tipinden yararlanılmaktadır.
	ICacheManager cacheManager = CacheFactory.GetCacheManager();

	// Eğer Cache koleksiyonunda, productId ile belirtilen bir referans tutulmuyorsa
	if (cacheManager[productId.ToString()] == null)
	{
		imageBytes = File.ReadAllBytes(imagePath);
		cacheManager.Add(productId.ToString(), imageBytes);
	}
	else // Eğer Cache koleksiyonunda productId anahtarına sahip bir referans var ise getir
		imageBytes = (byte[])cacheManager[productId.ToString()];

	return imageBytes;
}
```

Peki sistem nasıl çalışmaktadır?

![Undecided](/assets/images/2009/smiley-undecided.gif)

İstemci bir ürün resmi talep ettiğinde, kod parçasına göre öncelikle ön bellekte olup olmadığına bakılır. Eğer ön bellekte değilse Add metodu yardımıyla ön belleğe ekleme işlemi yapılır. Eğer nesne ön bellekte ise, indeksleyiciden yararlanılarak resmin byte[] dizisine cast edilerek elde edilmesi sağlanır. Burada önemli olan noktalardan biriside şudur; servise ait host uygulama açık olduğu sürece productId bazlı resimler ön bellekte saklanmaya ve korunmaya devam edecektir. Ancak host uygulamanın kapatılması durumunda, ön bellek koleksiyonuda otomatik olarak temizlenmektedir. Diğer yandan ön bellekte tutulan nesnelerin tamamını bilinçli bir şekilde temizlemek istersek, ICacheManager referansı üzerinden Flush metodunun çağırılması yeterli olacaktır. Yazımı sonlandırmadan önce son olarak şu noktayada değinmek isterim; senaryomuzda Caching bloğunu sunucu tarafındaki servis uygulaması için ele almış bulunmaktayız.

Buna göre istemci aynı resimleri talep ettiği ve servis uygulamasıda ayakta olduğu sürece, resimler ön bellekten tedarik edilecektir. Bu işlem resmin istemciye hızlı bir şekilde iletilmesini sağlamak üzere yapılmamıştır. Buna lütfen dikkat edelim. Aksine servis tarafındaki gereksiz resim okuma işlemini ekarte etmek amacıyla kullanılmıştır (Bu bloğun başka ne tip senaryolarda kullanılabileceğini düşünmenizi tavsiye ederim) Senaryomuzda elbetteki eksik kısımlar mevcuttur. Örneğin istisna yönetimi (Exception Handling) sıfırdır.

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

Asenkron erişim ile ilişkili istemci tarafında hiç bir şey yapılmamıştır.

![Sealed](/assets/images/2009/smiley-sealed.gif)

Diğer yandan resmin değişmesi halinde cache içeriğinin güncellenmesi ile ilgili bir çalışmada yapılmamıştır ki yapmaya çalışmanızı öneririm.

![Wink](/assets/images/2009/smiley-wink.gif)

Şimdilik benden bu kadar. Yeni bir western filmi sonrasında tekrardan Enterprise Library konulu bir örnek ile görüşmek üzere...

[HelloCachingBlock.rar (5,35 mb)](/assets/files/2009/HelloCachingBlock.rar)
