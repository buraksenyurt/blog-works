---
layout: post
title: "Entity Framework - POCO(Plain Old CLR Objects)"
date: 2010-04-30 12:01:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
---
Yandaki resimde Seychelles Blue Pigeon olarak adlandırılan ve Hint Okyanusundaki 115 adadan birisi olan Republic of Seychelles kolonisine has bir güvercin resmi yer almaktadır. Aslında bu kuş oldukça meşhurdur. Nitekim çeşitli hayvan türlerini genellikle kitap kapaklarında kullanan O'Reilly yayınlarının uzun zaman önce çıkarttığı ve Julia Lerman tarafından yazılmış olan [Programming Entity Framework](http://www.amazon.com/Programming-Entity-Framework-Julia-Lerman/dp/059652028X/ref=sr_1_1?ie=UTF8&s=books&qid=1266336933&sr=8-1-spell) baskısında bu kuşa yer verilmiştir. Kitaptan bahsetmişken...Bendeki baskısı Ado.Net Entity Framework 3.5 sürümünü içermekteydi. Doğal olarak köprünün altından çok sular geçti ve artık 4.0 sürümü ile karşı karşıyayız. Kitabın Aralık 2009 baskısında 4.0 vesiyonu içinde ek bilgiler yer almakta. Ancak sanıyorum ki yakın zamanda son sürümüne kavuşacak olan.Net Framework 4.0 ile birlikte yeni bir baskısı daha çıkacaktır.

![blg152_Giris.gif](/assets/images/2010/blg152_Giris.gif)

Aslında Ado.Net Entity Framework 4.0 tarafında yer alan önemli kavramlardan birisi de POCO nesnelerinin kullanımı. Ado.Net Entity Framework 3.5 (Service Pack 1) sürümünde net bir şekilde ele alınmayan POCO nesneleri için, 4.0 versiyonunda tam destek söz konusu. POCO (Plain Old CLR Objects),.Net Framework üzerinde herhangibir bağımlılığı olmadan tanımlanabilen nesneler olarak düşünülebilir. POCO nesneleri herhangibir sınıftan türemez (Class Inheritance), çeşitli arayüzleri uygulamaz (Interface Implementation) veya özel nitelikler (Attributes) ile işaretlenmezler. Sadece verinin üzerlerinden akmasını sağlayan hafif siklet (Lightweight) nesnelerdir. Oysaki Entity Framework tarafında üretilen tipler düşünüldüğünde, türetmelerin, nitelik işaretlemelerinin ve IPOCO olarak adlandırabileceğimiz bazı arayüz uyarlamalarının yapıldığı görülür. Bu nedenle Entity nesnelerinin, Persistence kıstasını göz ardı etmesi veya test senaryoları düşünülerek Entity Framework yapısı içerisinde ele alınmaları zorlaşmaktadır. Dolayısıyla Entity Framework tarafına getirilen POCO desteğinin önemi büyüktür.

Visual Studio 2010 ortamında geliştirilen Ado.Net Entity Framework bazlı uygulamalarda POCO nesnelerinin oluşturulması ve kullanımı son derece kolaydır. İşte bu yazımızda POCO nesnelerinin ne işe yaradığını, nasıl tanımlandıklarını ve kullanıldıklarını incelemeye çalışıyor olacağız. İşe başlamadan önce POCO'suz bir hayatın nasıl olacağına bakmamızda yarar olduğu kanısındayım. Bu amaçla örnek bir Console uygulamasını Visual Studio 2010 Ultimate RC sürümü üzerinden geliştirerek devam edebiliriz. Örneğimizde Chinook veritabanını baz alan ve aşağıdaki Entity Model diagramında görülen tiplere sahip olduğumuzu düşünelim.

![blg152_ModelDiagram.gif](/assets/images/2010/blg152_ModelDiagram.gif)

Çok basit anlamda Customer ve Invoice tablolarına karşılık gelen Entitiy tipleri söz konusudur. Bu tipler arasında bire çok (one-to-many) ilişki mevcuttur. Entity Model tarafında üretilen sınıf kodlarına baktığımızda aşağıdaki içeriklerin oluşturulduğunu görürüz.

Customer sınıfı;

```csharp
[EdmEntityTypeAttribute(NamespaceName="ChinookModel", Name="Customer")]
[Serializable()]
[DataContractAttribute(IsReference=true)]
public partial class Customer : EntityObject
{

Invoice sınıfı;

[EdmEntityTypeAttribute(NamespaceName="ChinookModel", Name="Invoice")]
[Serializable()]
[DataContractAttribute(IsReference=true)]
public partial class Invoice : EntityObject
{
```

Aslında her iki Entity sınıfı içinde dikkat edilmesi gereken nokta, EntityObject tipinden türemeleri ve bazı nitelikler (Attribute) tarafından imzalanmış olmalarıdır. Üstelik sınıflar içerisinde yer alan özelliklere (Properties) de bazı niteliklerin (Attribute) uygulandığı görülür. Örneğin;

```csharp
[EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
[DataMemberAttribute()]
public global::System.Int32 CustomerId
{
	get
	{
		return _CustomerId;
	}
	set
	{
		if (_CustomerId != value)
		{
			OnCustomerIdChanging(value);
			ReportPropertyChanging("CustomerId");
			_CustomerId = StructuralObject.SetValidValue(value);
			ReportPropertyChanged("CustomerId");
			OnCustomerIdChanged();
		}
	}
}
private global::System.Int32 _CustomerId;
```

gibi.

Bu noktada söz konusu tiplerin bazı bağımlıklar taşıdığını düşünebiliriz. Ancak POCO nesnelerinde, yazımızın başında da belirttiğimiz üzere bu tip bağımlılıklar söz konusu değildir. (Buna göre POCO nesneleri için şu tip bir tanımlama da yapılabilir; buradaki Entity tiplerinde yer alan bağımlılıklara ihtiyaç duymayan tiplere olan ihtiyaçlarda göz önüne alınan tiplerdir ![Wink](/assets/images/2010/smiley-wink.gif)) Şimdi örneğimizde ilerlemek için, her iki tipten sadece belirli özellikleri projemizde kullanmak istediğimizi düşünelim. Gerçekten de kod tarafında söz konusu Entity tipleri içerisinde yer alan tüm özelliklere ihtiyacımız olmayabilir. Bu amaçla sembolik olarak, Entity tiplerinden bazı özellikleri (Nullable değeri true olanlar seçilirse iyi olur) silip diagramı aşağıdaki şekilde görülen hale getirelim.

![blg152_Kalan.gif](/assets/images/2010/blg152_Kalan.gif)

Senaryomuz gereği sadece bu özellikler ile ilgilendiğimiz bir vakamız olduğunu düşünüyoruz. Buna göre Console uygulamamızda çok basit bir LINQ sorgusu yazarak Entity tiplerin kullanılabilir olduğundan emin olmamızda yarar vardır. İşte örnek kod parçamız ve çalışma zamanı çıktısı...

```csharp
using System;
using System.Linq;

namespace POCODans
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities entities = new ChinookEntities())
            {
                var results = from c in entities.Customers
                              join i in entities.Invoices on c.CustomerId equals i.CustomerId
                              where c.City=="Sidney"
                              select new
                              {
                                  c.CustomerId,
                                  Name=c.FirstName+" "+c.LastName,
                                  c.Email,
                                  c.Company,
                                  c.City,
                                  c.Country,
                                  i.BillingCity,
                                  i.BillingCountry,
                                  i.InvoiceDate,
                                  i.Total
                              };

                foreach (var r in results)
                {
                    Console.WriteLine(r.ToString());
                }
            }
        }
    }
}
```

Örnek LINQ sorgusuna göre Customer ve Invoice verileri CustomerId alanı üzerinden birleştirilerek, Sidney'de yaşayan müşteri ve fatura bilgileri isimsiz tip (Anonymous Type) içerisinde toplamakta ve ekrana yazdırmaktadır. Modelimizi test ettiğimizde aşağıdaki sonuçları aldığımızı dolayısıyla çalıştığını görürüz.

![blg152_Runtime.gif](/assets/images/2010/blg152_Runtime.gif)

Buraya kadar yaptıklarımızı özetlediğimizde, aslında Wizard yardımıyla bir modelin oluşturulduğunu fark edebiliriz. Bu otomatik süreç sonrasında Storage, Conceptual ve Mapping modellerinin üretildiğini ve bunların edmx dosyası içerisine aktarıldığını da anlayabiliriz. Peki bu otomatik üretim hattı göz önüne alındığında, herhangibir bağımlılığı bulunmayan, dolayısıyla otomatik olarak üretilmemesi gereken ve sadece veri aktarımı amacıyla kullanılacak olan bir nesnenin (POCO) entegrasyonunu nasıl gerçekleştirebiliriz?

Öncelikli olarak otomatik kod üretimi seçeneğini pasifleştirmeliyiz. Bu amaçla aşağıdaki şekilden de görüleceği üzere, EDMX'in özelliklerinden Code Generation Strategy değerini None olarak set etmeliyiz.

![blg152_CodeGStrategy.gif](/assets/images/2010/blg152_CodeGStrategy.gif)

Bu işlemin sonuçlarını aslında hemen görebiliriz. Uygulamanın build edilmesinin ardından ChinookModel.Designer.cs içeriğine baktığımızda aşağıdaki ekran görüntüsünde yer alan çıktı ile karşılaşırız. Dikkat edileceği üzere normalde var olması gereken tiplerin hiç birisi mevcut değildir.

![blg152_AutoGClose.gif](/assets/images/2010/blg152_AutoGClose.gif)

Şimdi işin en önemli kısımlarından birisine geldik. Entity Model üzerinde görülen Customer ve Invoice tiplerinin POCO versiyonlarını eklemek. Bunun için yapacağımız tek şey birer sınıf oluşturup içerisine gerekli özellikleri koymak olacaktır. Bunu zaten yapmamız gerekmektedir nitekim otomatik üretimi kapattığımız için elimizde artık Customer ve Invoice isimli Entity tipleri de bulunamamaktadır.

![Wink](/assets/images/2010/smiley-wink.gif)

Bu amaçla uygulamamıza aşağıdaki sınıfları eklediğimizi düşünelim.

```csharp
using System;
using System.Collections.Generic;

// Namespace adının ChinookModel olması önemlidir.
namespace ChinookModel
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        private List<Invoice> _invoices = new List<Invoice>();// Bir Customer' ın birden fazla faturası olabilir

        public List<Invoice> Invoices
        {
            get { return _invoices; }
            set { _invoices = value; }
        }
    }

    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public DateTime InvoiceDate{ get; set; }
        public string BillingCity { get; set; }
        public string BillingCountry { get; set; }
        public decimal Total { get; set; }
        public Customer Customer { get; set; } // Birden fazla fatura tek bir Customer ile ilişkilidir.
    }
}
```

Bir anlamda otomatik olarak üretilen Entitiy sınıflarını yazdığımızı düşünebiliriz ancak çok önemli bir fark bulunmaktadır. Bağımlılıklar. Hiç bir sınıf türetmesi, arayüz uygulaması veya nitelik işaretlemesi mevcut değildir dikkat edeceğiniz üzere. Elbette Customer ve Invoice sınıflarının yazılmış olmaları yeterli değildir. Birde bu tiplerin çalışma zamanındaki yönetimini üstelenebilecek bir içerik (Context) tipi olmalıdır. Bu amaçla uygulamaya aşağıdaki sınıfı eklememiz yeterli olacaktır.

```csharp
using System;
using System.Collections.Generic;
using System.Data.Objects;

namespace ChinookModel
{
    public class ChinookEntities
        :ObjectContext
    {
        private ObjectSet<Customer> _customers;
        private ObjectSet<Invoice> _invoices;
        
        public ChinookEntities()
            :base("name=ChinookEntities")
        {
        }

        public ObjectSet<Customer> Customers
        {
            get
            {
                if (_customers == null)
                {
                    _customers = base.CreateObjectSet<Customer>();
                }
                return _customers;
            }
        }

        public ObjectSet<Invoice> Invoices
        {
            get
            {
                if (_invoices == null)
                {
                    _invoices = base.CreateObjectSet<Invoice>();
                }
                return _invoices;
            }
        }
    }
}
```

ObjectContext tipinden türeyen ChinookEntities sınıfı içerisinde Customer ve Invoice sınıflarını kullanan ObjectSet tipli özellikler yer almaktadır. Dikkat edilmesi gereken noktalardan birisi de yapıcı metodun (Constructor) base çağrısı yardımıyla ObjectContext tipinin yapıcısına App.config dosyasında yer alan Connection String bilgisini gönderiyor olmasıdır.

![blg152_Appconfig.gif](/assets/images/2010/blg152_Appconfig.gif)

ChinookEntities isimli sınıfı ChinookModel isim alanı (Namespace) içerisinde tasarladığımızdan, Main metodunda gerekli bildirimi yapmamız, biraz önceki kodun değiştirilmeden çalışması için yeterli olacaktır.

```csharp
using System;
using System.Linq;
using ChinookModel;

namespace POCODans
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ChinookEntities entities = new ChinookEntities())
            {
                ...
```

![blg152_Runtime2.gif](/assets/images/2010/blg152_Runtime2.gif)

Volaaaa!!! Dikkat edecek olursanız var olan kodu bozmadan ama bu kez POCO nesnelerinden yararlanarak çalıştırmayı başardık.

![Laughing](/assets/images/2010/smiley-laughing.gif)

Bu işlemlerin ardından uygulamamızın tip yapısını Class Diagram üzerinden incelediğimizde, aşağıdaki gibi bir oluşumun söz konusu olduğunu görebiliriz.

![blg152_ClassDia2.gif](/assets/images/2010/blg152_ClassDia2.gif)

Bu yazımızda POCO (Plain Old CLR Objects) nesnelerinin ne olduğunu kısaca tanımaya çalışırken, çok basit bir örnek geliştirerek yolumuza devam ettik. Diğer yandan POCO nesnelerinin, Entity Framework'ün otomatik üretim aracı tarafından oluşturulan Entitiy tipleri ile olan farklarını anlamaya çalıştık. Artık Ado.Net Entity Framework tabanlı projelerimizde POCO nesnelerini kullanma ihtiyacını duyduğumuzda, nasıl hareket etmemiz gerektiğini az çok öğrenmiş olduğumuzu düşünüyorum. Tabi POCO nesnelerini kullanmanın getirdiği bazı dezavantajlar da yok değil. Herşeyden önce EntityObject içerisindeki elemanların türetme olmayışı nedeniyle kullanılamayışı, veritabanı tablolarındaki alan adları ile bir mapping işleminin yapılamayışı başlıca dezavantajlar olarak sayılabilir.

POCO ile ilişkili yeni bilgiler öğrendikçe sizlerle paylaşıyor olacağım. Özellikle bu yazıda henüz değerlendirmediğimiz bir durum var o da Lazy Loading durumlarında POCO nesnelerinin nasıl hazırlanması gerektiği? Bunu bir sonraki yazımızda aynı örnek üzerinden test ederek incelemeye çalışacağız. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[POCO_RTM.rar (44,97 kb)](/assets/files/2010/POCO_RTM.rar) [Örnek Visual Studio 2010 Ultimate RTM sürümü üzerinde geliştirilmiş ve test edilmiştir]
