---
layout: post
title: "Entity Framework - POCO ve Lazy Loading"
date: 2010-05-09 21:55:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - language-integrated-query
---
Hatırlayacağınız üzere [bir önceki yazımızda](https://www.buraksenyurt.com/post/Entity-Framework-POCO%28Plain-Old-CLR-Objects%29) Ado.Net Entity Framework içerisinde POCO (Plain Old CLR Object) nesnelerinin kullanımını incelemeye çalışmıştık. Örneğimizde kullanmış olduğumuz LINQ sorgusu basit bir Join işlemini gerçekleştirmekteydi. Tabi Join sorgusu kullandığımız için gözden kaçırdığımız ufak ama bir o kadar da önemli bir vaka oluşmaktadır. Bu vakayı ele almak için program kodunu biraz daha değiştirdiğimizi ve aşağıdaki hale getirdiğimizi düşünelim.

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
                #region Sample 2

                // Şirket bilgisi null veya boş olmayan müşteriler
                var customers = from c in entities.Customers
                                where !String.IsNullOrEmpty(c.Company)
                                select c;

                foreach (var customer in customers)
                {
                    Console.WriteLine("Company : {0} City : {1}",customer.Company,customer.City);

                    // ve bu müşterilere ait fatura bilgileri
                    foreach (var invoice in customer.Invoices)
                    {
                        Console.WriteLine("\tInvoice Date : {0} ,Total : {1}",invoice.InvoiceDate,invoice.Total.ToString("C2"));
                    }
                }

                #endregion
            }
        }
    }
}
```

Örneğimizin bu yeni haline göre Company alanı dolu olan (Null veya Empty olmayan) müşterilerin şirket adları ile bulundukları şehir bilgileri, ayrıca bu firmaların faturalarına ait tarih ve tutarları ekrana yazdırılmaktadır. Bu kodun çalışması sırasındaki beklentimiz ise Customer ve bunlara bağlı olan Invoice bilgilerinin getirilmesidir. Ancak uygulamayı çalıştırdığımızda aşağıdaki ekran görüntüsünde yer alan sonuçları elde ettiğimizi görürüz.

![Undecided](/assets/images/2010/smiley-undecided.gif)

![blg153_Runtime1.gif](/assets/images/2010/blg153_Runtime1.gif)

Dikkat edileceği üzere sadece Customer bilgileri çekilebilmiş ancak iç foreach döngüsü tarafından o anki Customer nesnesine ait Invoice bilgileri yazdırılmamıştır. Aslında bunun sebebini anlamak için SQL Server Profiler aracı yardımıyla arka planda çalıştırılan SQL sorgularına bakmamız yeterli olacaktır. Nitekim aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaştığımızı görürüz.

![blg153_FirstProfiler.gif](/assets/images/2010/blg153_FirstProfiler.gif)

Dikkat edileceği üzere kodun çalışması sonrasında sadece Customer bilgilerinin çekildiği görülmektedir. Tabi bu noktada örneğimizin POCO nesnelerini kullandığını unutmayalım. Eğer örneğimize ait Entity Model'in otomatik olarak üretildiğini düşünürsek aynı kodun çalışma zamanında aşağıdaki sonuçları ürettiğini görebiliriz.

![blg153_Runtime2.gif](/assets/images/2010/blg153_Runtime2.gif)

Dikkat edileceği üzere fatura bilgileri ekrana yazdırılmaktadır. Tabi SQL Server Profiler aracının üzerinden çalıştırılan SQL sorgularına baktığımızda aşağıdaki ifadelerin yer aldığını da görebiliriz.

![blg153_SecondProfiler.gif](/assets/images/2010/blg153_SecondProfiler.gif)

Dikkat edileceği üzere kaç tane Customer bilgisi çekilmişse her biri için Invoice tablosundan bilgi çekmek amacıyla birer SQL sorgusu icra edilmiştir. Çok doğal olarak burada Ado.Net Entity Framework'ün 4.0 versiyonu ile birlikte otomatik olarak kazandığı Lazy Loading kabiliyetinin devreye girdiğini söyleyebiliriz. Peki POCO nesneleri için Lazy Loading işlevselliğini nasıl kazandırabiliriz? Burada biraz dikkali olunması gerekmektedir. Nitekim Lazy Loading yeteneği için yapılacak eklentiler, POCO nesnelerinden Framework'e doğru bir bağımlılık oluşturmamalıdır. Normal şartlarda Lazy Loading işlemi için akla gelen ilk yöntem Customer tipi içerisinde yer alan Invoices özelliğinin get bloğunda gerekli kodlamaları yapmaktır. Oysaki bu hamle sonucunda Framework'e bir bağımlılık oluşturulması kaçınılmazdır ki POCO nesnelerinin ilkesine aykırı bir durumdur. Bu nedenle dinamik olarak üretilen proxy tiplerinden (Dynamically Generated Proxies) yararlanılmaktadır. Bu proxy tipleri aslında POCO nesnesinden türeyen ve ilgili özellikleri override ederek Lazy Loading gibi yetenekleri içeriye enjekte eden sınıflar olarak düşünülebilir. Aslında Ado.Net Team Blog'da bu tip Proxy sınıflarının nasıl geliştirildiği ve kullanıldığından bahsedilmektedir. Ancak biz çok daha basit bir yol izliyor olacağız.

POCO nesnelerinde Lazy Loading işlemini gerçekleştirebilmemiz için üç basit kurala dikkat etmemiz yeterlidir. Buna göre Context tipine ait yapıcı metod (Constructor) içerisinde gerekli bazı üst özelliklerin etkinleştirilmesi, Navigation Property'nin virtual olarak tanımlanması ve POCO tipinin public ve sealed olmaması yeterlidir. Örneğimizin kod içeriğinin yeni hali aşağıdaki gibidir.

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
            // KURAL 1: 
            // Dinamik proxy üretimi etkinleştirilir. Varsayılan değeri true dur
            this.ContextOptions.ProxyCreationEnabled = true;
            // Çalışma zamanında Navigation Property' lere erişildiğinde Lazy Loading işleminin otomatik olarak gerçekleştirilmesi için true değeri atanır.
            this.ContextOptions.LazyLoadingEnabled = true;
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

    // KURAL 2 : POCO sınıfı public olmalı ve sealed olarak işaretlenmemelidir.
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }

        private List<Invoice> _invoices = new List<Invoice>();

        // KURAL 3 : Navigation Property özelliğinin virtual olması gerekir.
        public virtual List<Invoice> Invoices
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
        public Customer Customer { get; set; } 
    }
}
```

Ve örneğimizin çalışma zamanı hali;

![blg153_Runtime3.gif](/assets/images/2010/blg153_Runtime3.gif)

Volaaaa!!!

![Laughing](/assets/images/2010/smiley-laughing.gif)

Üstelik SQL Server Profiler aracında da tam istediğimiz şekilde sorguların çalıştırıldığını görebiliriz.

![blg153_LastProfiler.gif](/assets/images/2010/blg153_LastProfiler.gif)

Tabi buradaki kuralların neden var olduğuna dikkat edilmelidir. Söz gelimi LazyLoadingEnabled, ProxyCreationEnabled değerlerinin true olması çalışma zamanındaki Entity motoru için gereklidir. Bu değerlerin false olması halinde bir hata mesajı alınmaz ancak istenen çalışma zamanı çıktıları da elde edilmez. Diğer taraftan POCO tipinin sealed olması halinde zaten virtual eleman içermesi ile ilişkili olarak derleme zamanı hatası alınacaktır.

![blg153_Error2.gif](/assets/images/2010/blg153_Error2.gif)

Buna ek olarak derleme zamanı POCO nesnesinin public olmasını beklemektedir. Public olarak tanımlanmadığı takdirde yine bir hata mesajı üretilecektir.

![blg153_Error1.gif](/assets/images/2010/blg153_Error1.gif)

Navigation Property'nin virtual olarak tanımlanmaması derleme veya çalışma zamanında bir hata mesajı üretmezken Lazy Loading işleminin de çalışmamasına neden olmaktadır. İşte bu kadar. Bu yazımızda POCO nesnelerinin kullanıldığı vakalarda Lazy Loading işlemlerinin nasıl ektinleştirilebileceğini incelemeye çalıştık. Unuttuğumuz bir şey var mı? Aslında olabilir.

![Wink](/assets/images/2010/smiley-wink.gif)

Eğer öyleyse de ilerleyen yazılarımızda ele almaya çalışacağız. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[POCOandLazyLoading_RTM.rar (90,73 kb)](/assets/files/2010/POCOandLazyLoading_RTM.rar) [Örnek uygulama Visual Studio 2010 Ultimate RTM sürümü üzerinde geliştirilmiş test edilmiştir]