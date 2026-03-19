---
layout: post
title: "WCF Data Services – Reflection Provider Kullanımı"
date: 2012-05-01 01:25:00 +0300
description: "Windows Communication Foundation(WCF) yıllardır Microsoft mimarisinde önemli bir yere sahip. Özellikle servis bazlı uygulama geliştirme modeline kazandırdığı pek çok yeni yaklaşım sayesinde, var olan ve gelecek Microsoft ürünlerinin de pek çok noktasında kullanılmaya başlandı. Bana göre WCF modeli özellikle 3.5 sürümü ile başlayan gelişmeler ile birlikte programlama modelini önemli derece de geliştirdi. Bu gün mimari programlama modeline kuşbakışı baktığımızda aşağıdaki çizelge de görülen servis çeşitlerinin kullanımda olduğunu biliyoruz."
categories:
  - wcf-data-services
tags:
  - wcf-data-services
  - csharp
  - dotnet
  - aspnet
  - entity-framework
  - linq
  - sql-server
  - wcf
  - workflow-foundation
  - silverlight
  - xml
  - soap
  - rest
  - json
  - web-api
  - web-service
  - http
  - reflection
  - generics
---
# WCF Data Services – Reflection Provider Kullanımı
![1316795_bicycle.jpg](/assets/images/2012/1316795_bicycle.jpg)

Merhaba Arkadaşlar,

Bundan bir kaç yıl öncesiydi. Daha dün gibi hatırlıyorum..Net Framework 3.0 sürümünde çıkması beklenen yeni Foundation alt yapıları üzerine Microsoft ekibinden gelen elektronik bir postayı okuyordum. O zamana kadar XML Web Service'leri ve özellikle.Net Remoting ile yakın ilişkiler içerisinde bulunduğumdan, dikkatimi ilk çeken Windows Communication Foundation isimli konsept olmuştu. Kısaca WCF olarak adlandırılıyordu. Mutlaka üzerine eğilmem gerektiğini düşündüğüm bir konuydu. Benim için yeni bir macera başlıyordu.

Hem heyecanlıydım hem de biraz korkmuştum

![Undecided](/assets/images/2012/smiley-undecided.gif)

Çünkü var olan tüm servis yaklaşımlarını tek bir çatı altında toplayacak bir modelden bahsediliyordu (Onun şimdiki halini ve yaygınlığını düşündüğümüzde, Microsoft'un bu vizyonunda gerçekten de haklı olduğunu bir kez daha görebiliyorum) Hemen araştırmalara başlamalıydım. Internet Explorer'ımı açtım ve WCF kelimesini googleladım. Bir de ne göreyim. Karşıma Dünya Bisiklet Federasyonu (World Cycling Federations) ile ilişkili sonuçlar çıktı. Yandaki fotoğrafın anlamı bundandır

![Tongue out](/assets/images/2012/smiley-tongue-out.gif)

Geyiği bir kenara bırakıp konumuza dönelim.

Windows Communication Foundation (WCF) yıllardır Microsoft mimarisinde önemli bir yere sahip. Özellikle servis bazlı uygulama geliştirme modeline kazandırdığı pek çok yeni yaklaşım sayesinde, var olan ve gelecek Microsoft ürünlerinin de pek çok noktasında kullanılmaya başlandı. Bana göre WCF modeli özellikle 3.5 sürümü ile başlayan gelişmeler ile birlikte programlama modelini önemli derece de geliştirdi. Bu gün mimari programlama modeline kuşbakışı baktığımızda aşağıdaki çizelge de görülen servis çeşitlerinin kullanımda olduğunu biliyoruz.

[![dsrp_1](/assets/images/2012/dsrp_1_thumb.png)](/assets/images/2012/dsrp_1.png)

Standart SOAP (Simple Object Access Protocol) servisleri (klasik XML Web Service yaklaşımı olarak da düşünebiliriz), bügünlerde ASP.NET takımınca ele alınan ve Asp.Net Web API olarak adlandırılan HTTP bazlı REST (Representational State Transfer) modeli, özellikle veri odaklı (Data-Centric) çalışmak üzere tasarlanmış Entity Framework odaklı Data Service’ ler, Silverlight tarafında kullanılan RIA (Rich Internet Application) çeşidi ve tabiki Workflow Foundation içerisinde yerini almış Workflow Service’ ler.

Bu geniş servis yelpazesinin uygulandığı alanlar göz önüne alındığında, SQL Server, Sharepoint, BizTalk gibi pek çok ürün grubu işin içerisine giriyor ki buna bir de Cloud üzerindeki WCF uç noktalarını etkilediğimizde modelin ne kadar etkili bir alana yayıldığını daha iyi görebiliyoruz. Dolayısıyla WCF, etkisini pek çok noktadan pozitif anlamda hissettiriyor. Peki biz bu yazımızda neyi inceliyor olacağız?

![Smile](/assets/images/2012/smiley-smile.gif)

Çoğunluka Entity Framework odaklı olarak kullanılan Data Service’ ler, basit HTTP protokolü üzerinden, Querystring bazlı olacak şekilde içerik yayınlanmasına izin vermektedir. Bu anlamda HTTP protokolünün basit Get, Post, Put, Delete metodları ile çalışabilen, XML, JSON veya OData standardında Entity çıktılarını verebilen bir servis yapısı söz konusudur. Burada kafamıza takılan veya çok fazla ilişmediğimiz konulardan birisi ise,

> Data Service’ leri her zaman Entity Framework tabanlı olarak kullanmak zorunda olup olmadığımızdır?

Güzel soru

![Sealed](/assets/images/2012/smiley-sealed.gif)

Acaba var olan Data Service çalışma modeli esnetilip Entity Framework yerine kendi veri sağlayıcılarımız (Provider) ile çalışabilir miyiz? Bir başka deyişle, örneğin ASP.NET üzerindeki Membership Provider, Profile Provider gibi yapılarda uygulanabilen özelleştirme mantığını, Data Service’ ler tarafında da yapabilir miyiz? İşte bu yazımızda bu konuyu ele alıyor olacağız.

Olayı kolay ve hızlı bir şekilde kavrayabilmek adına İşe basit bir Console uygulaması açıp, çözüme aşağıdaki şekilde görülen.Net assembly’ larını referans ederek başlamalıyız.

[![dsrp_3](/assets/images/2012/dsrp_3_thumb.png)](/assets/images/2012/dsrp_3.png)

Burada görüldüğü üzere uygulamamızda DataService tipini kullanarak bir Self-Host işlemi gerçekleştiriyor olacağız. Diğer yandan kendi provider’ ımızı yazacağımız için de bazı assembly’ lara ihtiyacımız var

- System.Data.Services
- System.Data.Services.Client
- System.ServiceModel
- System.ServiceModel.Web

Örnek uygulamamızda felsefeyi anlamaya odaklanacağımız için çok basit bir Text dosyasını veri kaynağı olarak kullanıyor olacağız. Bu text dosya içerisinde | işaretleri ile ayrılmış şekilde bir personel verisini tutmayı planlıyoruz. Personel numarası, adı, soyadı, ünvanı ve bulunduğu ülke örnek veri alanlarımız olarak düşünülebilir.

> Pek tabi çok daha farklı bir veri kaynağını da senaryoya dahil edebilirsiniz. Örneğin donanımsal bir arayüzün verilerini bu şekilde Data Service ‘ler üzerinden sunabileceğinizi düşünün. Söz gelimi bu bir mobil cihaz üzerinde bir süredir çalışmakta olan GPS servisinin topladığı içerik olabilir
>
> ![Wink](/assets/images/2012/smiley-wink.gif)

[![dsrp_4](/assets/images/2012/dsrp_4_thumb.png)](/assets/images/2012/dsrp_4.png)

Gelelim uygulamamız içerisindeki asıl kodlara. Önce Class Diagram’ a bir bakalım.

[![dsrp_2](/assets/images/2012/dsrp_2_thumb.png)](/assets/images/2012/dsrp_2.png)

Person sınıfı aslında bizim için POCO (Plain Old CLR Object) niteliği gösteren bir tip olarak düşünülebilir. Bu tipi, text dosya içerisindeki satırların nesnel karşılıklarını ifade etmek için kullanacağız.

```csharp
using System.Data.Services.Common;

namespace ReflectionProvider 
{ 
    [DataServiceKey("PersonId")] 
    public class Person 
    { 
        public int PersonId { get; set; } 
        public string Name { get; set; } 
        public string Surname { get; set; } 
        public string Title { get; set; } 
        public string Country { get; set; } 
    } 
}
```

Tabi bu tip içerisinde sınıf seviyesinde uygulanan DataServiceKey niteliğine dikkat etmemiz gerekiyor. Bu nitelik ile Person tiplerinin Unique’ liğini PersonId özelliklerine bağladığımızı ifade etmiş oluyoruz. Nitekim Data Service sınıfları, Entity Framework Entity tipleri ile çalışırken ilgili sınıflarda bu niteliğin kullanılmasını beklemektedir. Aynı davranışın burada da uyarlanması gerekmektedir. Gelelim Bag sınıfına.

```csharp
using System; 
using System.Collections.Generic; 
using System.Configuration; 
using System.IO; 
using System.Linq;

namespace ReflectionProvider 
{ 
    public class Bag 
    { 
       private static List<Person> persons = new List<Person>();

        static Bag() 
        { 
            string[] personLines=File.ReadAllLines(ConfigurationManager.AppSettings["FilePath"]); 
            foreach (string personLine in personLines) 
            { 
                string[] columns=personLine.Split('|'); 
                Person person = new Person(); 
                person.PersonId = Int32.Parse(columns[0]); 
                person.Name = columns[1]; 
                person.Surname = columns[2]; 
                person.Title = columns[3]; 
                person.Country = columns[4]; 
                persons.Add(person); 
            } 
        }

        public IQueryable<Person> Persons { 
           get 
            { 
                return persons.AsQueryable(); 
            } 
        } 
    } 
}
```

Bag sınıfı içerisinde text tabanlı dosya içeriğinin okunması ve Person tipinden generic bir List koleksiyonuna atanması işlemleri yapılmaktadır. Bu işlemler static yapıcı metod (Constructor) içerisinde gerçekleştirilmektedir. Diğer yandan sınıfın olabilecek belki de en önemli özelliği Persons üyesidir. Dikkat edileceği üzere bu üye geriye IQueryable tipinden bir referans döndürmektedir. Bu sayede biz Data Service host ortamına, URL bazlı olarak sorgulanabilir bir içerik sağlayacağımızı da belirtmiş olacağız (Bir başka deyişle EntitySet tipini ifade etmiş olmaktayız) Tabi bu belirtme işlemini asıl DataService sınıfı içerisinde gerçekleştireceğiz. Aynen aşağıda olduğu gibi

![Wink](/assets/images/2012/smiley-wink.gif)

```csharp
using System.Data.Services;

namespace ReflectionProvider 
{ 
    public class BagDataService 
        :DataService<Bag> 
    { 
        public static void InitializeService(IDataServiceConfiguration configuration) 
        { 
            configuration.SetEntitySetAccessRule("Persons", EntitySetRights.All); 
        } 
    } 
}
```

InitializeService metodu, servisi host edeceğimiz ortamda tetikleniyor olacak. Bu metod içerisinde standart Data Service şablonunda da yapıldığı gibi gerekli erişim kuralları belirlenmektedir. Tabi bu sefer Bag tipi içerisinde yer alan Persons isimli özellik için full erişim açılması söz konusudur. Bir başka deyişle GET, POST, PUT ve DELETE metodlarına hizmet edecektir. (Ancak POST, PUT, DELETE gibi metod etkileşimleri için Bag tipine IUpdatable arayüzünün de uygulanması gerekmektedir ki bu yazımızda bu konu ele alınmamıştır. Bir sonraki yazımızda kısmetse)

Artık tek yapılması gereken DataService tipinden yararlanarak bir Host örneğini oluşturmak ve belirli bir URL üzerinden istemci taleplerini dinleyecek şekilde ayağa kaldırmaktan ibarettir. Bu amaçla Console uygulamamıza ait Main metodu içeriğini aşağıdaki gibi geliştirebiliriz.

```csharp
using System; 
using System.Data.Services;

namespace ReflectionProvider 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Type serviceType = typeof(BagDataService); 
            Uri baseAddress = new Uri("http://localhost:8080"); 
           Uri[] baseAddresses = new Uri[] { baseAddress };

            DataServiceHost host = new DataServiceHost( 
                serviceType, 
               baseAddresses);

            host.Open(); 
            Console.WriteLine("Host durumu {0}. Kapatmak için bir tuşa basın",host.State); 
            Console.ReadLine(); 
            host.Close(); 
            Console.WriteLine("Host duruumu {0}. Güle güle!",host.State); 
        } 
    } 
}
```

Şimdi buraya kadar yaptıklarımızı bir test edelim. İlk olarak Console uygulamamızı çalıştıralım. Sonrasında ise herhanbiri tarayıcı üzerinden uygulamamıza sorgulama talepleri gönderelim.

İlk sorgumuzda http://localhost:8080/Persons talebini deneyebiliriz.

![dsrp_6.png](/assets/images/2012/dsrp_6.png)

Dikkat edileceği üzere Text dosyamız içerisinde yer alan tüm içerik XML formatında tarayıcı uygulamaya gelmiştir.

İkinci sorgumuzda ise belirli bir PersonId değerine sahip içeriği getirmeye çalışacağız. Hatırlayacağınız gibi Person tipine uyguladığımız DataServiceKey niteliği ile, PersonId özelliğinin unique anahtar olduğunu belirtmiştik. Bu amaçla http://localhost:8080/Persons (10002) URL'ini deneyebiliriz. İşte sonuç

![Wink](/assets/images/2012/smiley-wink.gif)

![dsrp_7.png](/assets/images/2012/dsrp_7.png)

Çok doğal olarak servise başka sorgular da gerçekleştirilebilir. Örneğin Country özelliğinin değerine göre alfabetik sırada listenin elde edilmesi sağlanabilir. Bunun için orderby anahtar kelimesini kullanmak yeterlidir. Tabi dikkat edilmesi gereken noktalardan birisi de, sorgu içerisinde yer alan özellik adlarının case-sensitive olarak ele alınması gerekliliğidir. Bir başka deyişle, Country yerine country yazmak hataya neden olacak ve bir sonuç kümesi döndürülmeyecektir. Dolayısıyla http://localhost:8080/Persons?$orderby=Country şeklindeki URL ifadesi aşağıdaki sonucun üretilmesini sağlayacaktır.

![dsrp_8.png](/assets/images/2012/dsrp_8.png)

Görüldüğü gibi WCF Data Service'lerde Reflection Provider'larını kullanarak Entity Framework dışındaki kaynakları kullanmak son derece kolaydır. Bu yazıda geliştirdiğimiz örnekte, POST, PUT ve DELETE sorguları için gerekli destek yoktur. Bunun için IUpdatable arayüzünün ve onunla birlikte tanımlanan üyelerin ezilmesi (override) gerekmektedir. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Wink](/assets/images/2012/smiley-wink.gif)

[ReflectionProvider.rar (36,11 kb)](/assets/files/2012/ReflectionProvider.rar)