---
layout: post
title: "WCF 4.0 Yenilikleri - Announcement Kullanımı [Beta 1]"
date: 2009-08-21 04:00:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - windows-communication-foundation
---
WCF 4.0 tarafında WS-Discovery tabanlı olarak gerçekleştirilen uygulamalarda önem arz eden noktalardan biriside, servislerin online veya offline olma durumlarını, bulundukları ağ üzerindeki dinleyicilere (Listeners) bildirmeleridir (Announce). Bildiri şeklinde yapılan yayınlamalar aslında istemcinin ağ üzerine yaydığı multicast mesajların yoğunluğunu azaltmak gibi olumlu bir etkiye de sahiptir. Şimdi bu bildirim işlemlerinin nasıl yapılacağını incelemeye çalışalım. Ad Hoc modelinin uygulanması ile ileişkili yazımızdaki örneğimizi bu amaçla devam ettirebiliriz. Servis tarafında konfigurasyon dosyasında sadece aşağıdaki eklemeleri yapmamız yeterli olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceDiscovery>
            <announcementEndpoints>
              <endpoint kind="udpAnnouncementEndpoint"/>
            </announcementEndpoints>
          </serviceDiscovery>
          <serviceMetadata/>
        </behavior>
      </serviceBehaviors>
    </behaviors>    
    <services>
      <service name="ServerApp.CalculusService">
        <endpoint address="" binding="basicHttpBinding" contract="ServerApp.ICalculus"/>
        <endpoint address="Mex" kind="mexEndpoint" />
        <endpoint name="udpDiscovery" kind="udpDiscoveryEndpoint" />
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

Konfigurasyon dosyasında, Servis tarafına yeni bir davranış (Service Behavior) eklenmiş ve bu davranış için udpAnnouncementEndpoint tipinden bir Endpoint kullanılacağı belirtmiştir. Bu endpoint tipi çalışma zamanında, servisin ağ üzerindeki dinleyicilere mesaj gönderebilmesi için gerekli alt yapının oluşturulmasını sağlamaktadır. Bir bakşa deyişle işimizi oldukça kolaylaştırmaktadır

![Wink](/assets/images/2009/smiley-wink.gif)

Ancak istemci tarafında biraz kod eforu sarfedilmelidir. İstemci tarafı bir dinleyici olarak, servisin ortama gönderdiği "ben geldim" veya "ben gittim" tadındaki mesajları yakaladığında devreye girecek olan iki olay metodunu ele alabilmelidir. Tabi bunlardan daha önemlisi çalışma zamanı için gerekli alt yapı hazırlıklarınıda gerçekleştirmelidir. Şimdi istemci tarafındaki kodlarımızı aşağıdaki gibi geliştirdiğimizi düşünelim.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace ClientV2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Servisin online/offline olma durumlarında yaptığı bildirimleri yakalayan nesne örneği
            AnnouncementService aService = new AnnouncementService();

            // Servis online olduğunda devreye giren olay metodu
            aService.OnlineAnnouncementReceived += delegate(object sender, AnnouncementEventArgs e)
            {    
                // Online olan servisin etkinleştirilmiş Endpoint noktalarının Address bilgileri ve o anki mesajın numarası yazdırılır. 
                Console.WriteLine("\nMessage No : {0}\n\t{1} adresli EndPoint ONLINE oldu",
                    e.AnnouncementMessage.MessageSequence.MessageNumber,
                    e.AnnouncementMessage.EndpointDiscoveryMetadata.Address.ToString());

                // Etkinleşen Endpoint' ler üzerinden sunulan servis sözleşmeleri yazdırılır
                Console.WriteLine("Contracts ");
                foreach (var contractType in e.AnnouncementMessage.EndpointDiscoveryMetadata.ContractTypeNames)
                {
                    Console.WriteLine("\t{0}",contractType.Name);
                }
            };

            // Servis offline olduğunda devreye giren olay metodu
            aService.OfflineAnnouncementReceived += delegate(object sender, AnnouncementEventArgs e)
            {
                // Kapatılan servis üzerindeki Endpoint bilgileri yazdırılır.
                Console.WriteLine("\nMessage No : {0}\n\t{1} adresli EndPoint OFFLINE oldu",
                                    e.AnnouncementMessage.MessageSequence.MessageNumber,
                                    e.AnnouncementMessage.EndpointDiscoveryMetadata.Address.ToString());
            };

            // AnnouncementService örneği kullanılarak bir ServiceHost nesnesi örneklenir
            ServiceHost host = new ServiceHost(aService);
            // Service yeni bir UdpAnnouncementEndpoint eklenir
            host.AddServiceEndpoint(new UdpAnnouncementEndpoint());
            // Dinleme işlemleri için servis açılır
            host.Open();

            Console.WriteLine("Dinlemedeyiz...Çıkmak için bir tuşa basınız");
            Console.ReadLine();

            // Servis kapatılır
            host.Close();
        }
    }
}
```

Her ne kadar istemci tarafını geliştiriyor olsakta pek istemci tarzında olmadığını eminimki farketmişsinizdir.

![Wink](/assets/images/2009/smiley-wink.gif)

Nitekim istemci tarafında ServiceHost nesnesi örneklenmekte ve kullanılmaktadır. Aslında bu son derece doğaldır. Nitekim online veya offline olan servislerin, istemciler üzerinde tetikleyebildiği iki olay söz konusudur. Buda istemcinin bir anlamda servis gibide davranış gösterebilmesini gerektirmektedir. (Normal şartlar altında servisin, istemciler üzerinde olay tetikletmesi gerektiği durumlarda özellikle.Net Remoting gibi modellerde çok kafa karıştırıcı kodlamalar yapılması gerektiğini hatırlatmak isterim.![Undecided](/assets/images/2009/smiley-undecided.gif))

WCF 4.0 tarafında ise tek yapmamız gereken bu iş yükünü AnnouncementService tipine atmaktır. Dikkat edileceği üzere ServiceHost nesnesi örneklenirken parametre olarak AnnouncementService referansı verilmektedir. Sonrasında ise ServiceHost nesnesine, UpdAnnouncementEndpoint tipinden bir Endpoint ilave edilmiştir. Örnekle ilişkili ilginç noktalardan biriside istemci tarafında App.config dosyasının bulunmayışıdır.(Örnekten bu dosyası bilinçli bir şekilde çıkarttığımı belirtmek isterim)

İstemci uygulama dinlemede kaldığı süre boyunca, online veya offline olan tüm endPoint noktalarına ait announce mesajlarını yakalayabilmektedir. Bunlara ek olarak, istemcinin belirli bir servise odaklanmadığı da görülmektedir. Yerel ağ üzerindeki herhangibir servisten gelen announce mesajlarını dinleyebilmektedir. Modeli test etmek için istemci uygulama açık iken, bir veya daha fazla servisin (tabiki bunların WS-Discovery tabanlı olarak geliştirilmiş olma şartları vardır) kapatılıp açılması yeterlidir. Ben test sırasında aşağıdaki ekran görüntüsünde yer alan sonuçları aldım.

![blg69_Runtime.gif](/assets/images/2009/blg69_Runtime.gif)

Görüldüğü üzere servisin bir kaç kere açılması ve kapatılmasının ardından istemci tarafındaki OnlineAnnouncementReceived ve OfflineAnnouncementReceived olayları tetiklenmiş ve gerekli bildirimler yakalanmıştır. Artık bu noktadan sonra istemcinin sadece online olan Endpoint noktalarına göre proxy nesnelerini oluşturması ve kullanması yeterli olacaktır.

Bir sonraki yazımızda Ad Hoc modelini terkedip, Managed Discovery modelini incelemeye çalışacağız. Bildiğiniz üzere Ad Hoc modelde yerel/alt ağlar söz konusudur ve ağın ötesine geçilmesi halinde proxy tabanlı bir sistemin kullanılması gerekmektedir. Bakalım bizi ne gibi sürprizler bekliyor olacak...

![Wink](/assets/images/2009/smiley-wink.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[AdHocDiscoveryForAnnouncement.rar (82,45 kb)](/assets/files/2009/AdHocDiscoveryForAnnouncement.rar)