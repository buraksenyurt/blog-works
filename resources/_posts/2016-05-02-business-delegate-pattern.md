---
layout: post
title: "Business Delegate Pattern"
date: 2016-05-02 06:00:00 +0300
description: "Epey zamandır tasarım kalıpları tarafına bakmadığımı fark ettim. Hem kalıpları tekrar etmek hem de yeni bir şeyler var mı diye internette gezinirken JEE tarafında sıklıkla başvurulan Business Delegate isimli bir desene rastladım. Aslında delegate dediğimiz zaman bir işi başkasına devrettiğimizi düşünebiliriz (Delegasyon ile ilgili olarak internette resim ararken de işte yandaki gibi eğlenceli bir tanesine rastladım) Business Delegate kalıbı temel olarak sunum katmanı(presentation layer) ile iş katmanını(Business Layer) arasındaki iletişimde aynı isimli fonksiyonların ele alınmasında değerlendiriliyor. Zaten adından da anlaşılacağı üzere sunum katmanındaki bir fonksiyonelliğin asıl iş katmanındaki karşılığına devredilmesi söz konusu. Burada önemli noktalardan birisi talebi olan nesnenin talep ile ilgili içeriği(Context diyebiliriz) aynen ikinci bir nesneye delege etmesidir."
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - dotnet
  - async-await
  - delegates
---
# Business Delegate Pattern
![Bdpattern_4.gif](/assets/images/2016/Bdpattern_4.gif)

Merhaba Arkadaşlar,

Epey zamandır tasarım kalıpları tarafına bakmadığımı fark ettim. Hem kalıpları tekrar etmek hem de yeni bir şeyler var mı diye internette gezinirken JEE tarafında sıklıkla başvurulan Business Delegate isimli bir desene rastladım. Aslında delegate dediğimiz zaman bir işi başkasına devrettiğimizi düşünebiliriz (Delegasyon ile ilgili olarak internette resim ararken de işte yandaki gibi eğlenceli bir tanesine rastladım)

Normal şartlarda standart olarak kabul gören Gangs of Four (Erich Gamma, Richard Helm, Ralph Johnson ve John Vlissides) desenlerinde yer almıyor ancak genel yazılım tasarım kalıpları içerisinde yer verilmiş. (Tüm deseneler ile ilişkili olarak [Wikipedia adresine](https://en.wikipedia.org/wiki/Design_Patterns) giderseniz diğer desenler kısmında yer aldığını görebilirsiniz)

> GoF un tanımladığı tasarım desneleri haricinde literatüre giren farklı kalıplar da bulunuyor. Bu tip kalıpları araştırmak, hangi problemelere çözüm olarak ele alındıklarını öğrenmek, kişisel gelişim açısından önemli.

Business Delegate kalıbı temel olarak sunum katmanı (presentation layer) ile iş katmanını (Business Layer) arasındaki iletişimde aynı isimli fonksiyonların ele alınmasında değerlendiriliyor. Zaten adından da anlaşılacağı üzere sunum katmanındaki bir fonksiyonelliğin asıl iş katmanındaki karşılığına devredilmesi söz konusu. Burada önemli noktalardan birisi talebi olan nesnenin talep ile ilgili içeriği (Context diyebiliriz) aynen ikinci bir nesneye delege etmesidir.

Teorik olarak aşağıdaki gibi bir çizelge desenin kullanımı ile ilişkili bir ipucu verebilir. Aslında Adapter ve Bridge tasarım kalıpları da tercih edilir. Kullanım alanı olarak.Net tarafındaki asenkron tabanlı (async-based) programlama ihtiyaçlarında kullanılabilir (Örneğin bir servis çağrısının asenkron olarak gerçekleştirilmesi ve işlemin tamamlanmasını takiben geriye dönen Task'ın işaret ettiği fonksiyonun çağırılması)

![Bdpattern_3.gif](/assets/images/2016/Bdpattern_3.gif)

Temel olarak desende yer alan aktörler şöyledir.

- Caller (Client): Talep edilen metod çağrısını gerçekleştiren asıl nesne olarak düşünülebilir. Arayüz tarafındaki bir kod parçası olabilir. Sunum katmanında yer alır.
- Business Delegate: Talepte bulunan nesne çağrılarının asıl iş servisi metodlarına geçişisini sağlayan temsilcidir. Genellikle tek bir giriş noktası (endPoint olarak isimlendirelim) sunar.
- LookUp Service: İlişkili iş nesnelerinin üretiminden sorumludur. Çoğunlukla dışarıdan verilen bilgiye göre Business Service Interface türevli bir nesnenin üretimini gerçekleştirir (Service Locator gibi de düşünebiliriz)
- Business Service Interface: Asıl iş fonksiyonelliklerini bulunduran sınıfların şema tanımlamasını içeren arayüzdür (Interface).

Şimdi basit bir örnek ile bu deseni incelemeye çalışalım. Aşağıdaki sınıf çizelgesinde yer alan tiplerin yer aldığı Console uygulamasını geliştirerek ilerleyebiliriz.

![Bdpattern_1.gif](/assets/images/2016/Bdpattern_1.gif)

```csharp
using System;

namespace TheBusinessDelegatePattern
{
    class Program
    {
        static void Main(string[] args)
        {
            BusinessMessagingDelegate bsnDelegate = new BusinessMessagingDelegate();
            bsnDelegate.ServiceType = "SMS";

            Caller client = new Caller(bsnDelegate);
            string smsMesssage = "Son 4 hanesi 1111 olan kartınız dönem borcu -1000 Liradır. Bu kez ödeme bizden.";
            string emailMessage = "Son 4 hanesi 1111 olan kartınızı dönem ekstresi ektedir. Açınız şaşırınız.";
            client.Do(smsMesssage);

            bsnDelegate.ServiceType = "EMAIL";
            client.Do(emailMessage);
        }
    }

    public interface IMessagingService
    {
        void SendMessage(string message);
    }

    public class SMSMessagingService
    : IMessagingService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class MailMessagingService
    : IMessagingService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class BusinessServiceLookUp
    {
        public IMessagingService GetBusinessService(String serviceType)
        {
            if (serviceType.Equals("SMS"))
            {
                return new SMSMessagingService();
            }
            else
            {
                return new MailMessagingService();
            }
        }
    }

    public class BusinessMessagingDelegate
    {
        BusinessServiceLookUp lookupService = new BusinessServiceLookUp();
        IMessagingService businessService;
        public string ServiceType { get; set; }

        public void Do(string message)
        {
            businessService = lookupService.GetBusinessService(ServiceType);
            businessService.SendMessage(message);
        }
    }

    public class Caller
    {
        BusinessMessagingDelegate _businessService;

        public Caller(BusinessMessagingDelegate businessService)
        {
            _businessService = businessService;
        }

        public void Do(string message)
        {
            _businessService.Do(message);
        }
    }
}
```

Dilerseniz kodda neler yaptığımızı kısace değinelim. Temel olarak SMS ve EMail gönderim işlemlerinin ele alındığını mesajlaşma hizmetleri söz konusu. SMSMessagingService ve MailMessagingService sınıflarının ortak özelliği IMessagingService arayüzünden türemiş olmaları. Bu sınıfların iş katmanındaki asıl tipler olduğunu varsayabiliriz.

Caller sınıfı talepte bulunan arayüz nesnes kullanıcısı olarak düşünülebilir. Arayüz katmanında olduğunu varsaydığımız bu tipin önemli kabiliyetlerinden birisi, hangi iş hizmetini kullanacağını bilmesidir. Bunun için yapıcı metoduna (Constructor) parametre olarak gelen BusinessMessagingDeletage tipinden yararlanır. Bu tipin ServiceType özelliği, delegasyonun yapılacağı iş biriminin üretiminde kullanılır. Üretim aşamasında ise BusinessServiceLookUp isimli sınıf devreye girmekte ve BusinessMessageDelegate sınıfının ServiceType özelliğine göre bir IMessagingService uyumlu referansı kullanımı için vermektedir.

Çalışan program kodunda Caller nesne örneği üzerinden Do metoduna gerçekleştirilen iki farklı çağrı söz konusudur. Kod temsilcinin üretim biçimine göre uygun olan iş birimi hizmetine yönlenir. Kodun çalışma zamanı çıktısı aşağıdaki ekran görüntüsündeki gibi olacaktır.

![Bdpattern_2.gif](/assets/images/2016/Bdpattern_2.gif)

Pek tabii gerçek hayat senaryolarında asıl iş birimi metodlarının string parametre yerine bir içerik tipi ile (örneğin Context isimli bir sınıf) çalışması ve geriye bir referans döndürmesi muhtemeldir. Hatta bu tipler genellikle sunum ve iş katmanı arasında hareket eden transfer nesneleri de olabilir.

Bu yazımızda pek göz önünde olmayan tasarım kalıplarından birisine değinmeye çalıştık. Bir başka makalemizde görüşünceye dek hepinize mutlu günler dilerim.