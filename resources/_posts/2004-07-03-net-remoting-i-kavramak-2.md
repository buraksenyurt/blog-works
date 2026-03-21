---
layout: post
title: "NET Remoting' i Kavramak - 2"
date: 2004-07-03 09:00:00 +0300
categories:
  - dotnet-remoting
tags:
  - .net-remoting
  - tcp
  - rpc
  - distributed-programming
---
Bu makalemizde, daha önceden değinmiş olduğumuz.net remoting ile ilgili olarak çok basit bir örnek geliştirmeye çalışacağız. Remoting'de amaç, istemcilerin uzak nesnelere ait üyelere erişebilmelerini ve kullanabilmelerini sağlamaktır. Dolayısıyla, remoting sistemi söz konusu olduğunda, remote object, server channels ve client channels kavramları önem kazanır. Olaya bu açıdan baktığımızda ilk olarak bir remote object (uzak nesne) geliştirmemiz gerektiği ortadadır. Daha sonra, bu nesneyi kullanmak isteyecek istemcileri dinleyecek bir server programını yazmamız gerekir. Bu server programı aslında, remote object'i barındıran (host) bir hizmet programı olacaktır. İstemcilerin tek yapması gereken uzak nesneye ait bir örneği, çalıştıkları sistemde kullanarak bir proxy nesnesi oluşturmak ve bu nesne üzerinden server programa istekte bulunarak ilgili uzak nesneye ait metodları çalıştırmaktır. İşte bu noktada server ve client uygulamalardaki channel nesneleri önem kazanır.

Biz bugünkü örneğimizde, TCP protokolü üzerinden haberleşen bir remoting uygulaması geliştirmeye çalışacağız. öncelikle işe, remote object (uzak nesne) sınıfını tasarlayarak başlayalım. Konuyu daha net anlayabilmek için, uygulamalarımızı editoründe geliştireceğiz. Şimdi aşağıdaki kodlardan oluşan, UzakNesne.cs kod dosyasını oluşturalım.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace UzakNesne
{
    public class Musteriler:System.MarshalByRefObject
    {
        public string MusteriBul(string CustID)
        {
SqlConnection con=new SqlConnection("data source=localhost;initial 
catalog=Northwind;integrated security=SSPI");
SqlCommand cmd=new SqlCommand("SELECT * FROM Customers WHERE 
CustomerID='"+CustID+"'",con);
con.Open();
SqlDataReader dr=cmd.ExecuteReader();
dr.Read();
string Bulunan=dr["CompanyName"]+" "+dr["ContactName"]+" "+dr["Phone"];
con.Close();
return Bulunan;
        }
    }
}
```

Burada görüldüğü gibi remote object (uzak nesne) sınıfı için en önemli unsur, bu sınıfın System.MarshalByRefObject sınıfından türetilmiş olmasıdır. Normal şartlarda geliştirdiğimiz bu nesneye, bulunduğu sistemin application domain'i içerisinden direkt olarak erişebiliri. Ancak, bu nesnenin application domain'in dışındaki bir domainden kullanılabilmesini sağlamak yani remoting desteğini vermek için MarshalByRefObject sınıfından türetmemiz gerekmektedir. Bu sayede istemci uygulama, bu nesneye ait proxy nesnesi üzerinden mesaj gönderebilecek ve alabilecektir. Şimdi geliştirdiğimiz bu assembly'ı bir dll olarak aşağıdaki gibi derleyeylim.

![mk76_1.gif](/assets/images/2004/mk76_1.gif)

Artık uzak nesnemizede sahip olduğumuza göre, bu nesneyi kullanacak istemcileri takip edecek, bir başka deyişle dinleyecek (listening) bir server (sunucu) uygulaması yazabiliriz. Bir sunucu uygulaması için en önemli unsurlar, dinleme işlemi için hangi kanalın, hangi ayarlar ile kullanılacağı ve remote object (uzak nesne) sınıfına ait bilgilerin ne olduğundan oluşmaktadır. Server (sunucu) için gerekli bu bilgileri uygulama içerisinden programatik olarak belirleyeceğimiz gibi, xml tabanlı bir konfigurasyon dosyası yardımıylada tanımlayabiliriz. XML tabanlı konfigurasyon dosyasının, programatik olan tekniğe göre en büyük avantajı, kanal ve remote object (uzak nesne) ile ilgili düzenlemelerde, kodun yeniden derlenmesine gerek kalınmamasıdır. Bir konfigurasyon dosyası, config uzantılıdır. Bu makalemizde kullancağımız Server (sunucu) uygulamaya ait konfigurasyon dosyası Sunucu.config isminde olup, aşağıdaki satırlardan oluşmaktadır.

```xml
<configuration>
    <system.runtime.remoting>
        <application name="MusteriUygulama">
            <service>
                <wellknown mode="SingleCall" type="UzakNesne.Musteriler,UzakNesne" objectUri="UzakNesne"/>
            </service>
            <channels>
                <channel ref="tcp server" port="1000"/>
            </channels>
        </application>
    </system.runtime.remoting>
</configuration>
```

Şimdi bu xml dosyasının içeriğini kısaca incelemeye çalışalım. öncelikle ana eleman dır. Burada server (sunucu) uygulamasına ait remote object (uzak nesne) ve channel (kanal) ayarlamaları mutlaka, elemanı içinde yapılmalıdır. elemanı içinde name özelliği ile server (sunucu) uygulamamızın adını belirtiyoruz.

Asıl önemli olan iki eleman elemanı altında yer alan ve elemanlarıdır. elemanı altında
type niteliğinde remote object (uzak nesne) için, bu tipin hangi isim alanı ve sınıfa ait olduğu belirtilir. type niteliğindeki ikinci parametre ise, uzak nesnenin bulunduğu assembly'ın adıdır. objectUri ile, istemcilerin uzak nesne için kullanacakları endpoint (son nokta) ismi belirtilmektedir. elemanı altında önceden tanımlanmış remoting protokolleri yer almaktadır.

Burada Machine.config içindeki tcp server protokolüne ait tanımlamada, System.Runtime.Remoting.Channels.Tcp. TcpServerChannel tipininde belirtildiğine dikkat edelim. Konfigurasyon dosyasının hazırlanması ile birlikte, artık sunucu uygulamamızı yazabiliriz. Sunucu uygulama, bu konfigurasyon dosyasını kullanarak, belirtilen protokol ve port üzerinden istemci taleplerini (mesajlarını) dinlemeye alacak ve gelen bu talepleri, yine konfigurasyon dosyasında belirtilen uzak nesneye ait örneğe yönlendirecektir. Nesneden dönen cevaplarda, yine bu konfigurasyon dosyasında belirtilen protokol ve port ile istemcilere gönderilecektir. Gelelim server (sunucu) uygulamamızın kodlarına.

Burada görüldüğü gibi, sunucu uygulamamız konfigurasyon dosyasındaki bilgilere RemotingConfiguration sınıfının Configure metodu ile ulaşmaktadır. İzleyen satırlardaki WriteLine ve ReadLine metodları ile, uygulamanın biz son verene kadar 1000 nolu Tcp Server portunu dinlemesi sağlanmıştır. Şimdi yazdığımız bu uygulamayı exe olarak derleyelim.
![mk76_2.gif](/assets/images/2004/mk76_2.gif)
Bu işlemlerin ardından, sunucu nesnemizin, sunucu uygulamamızın ve konfigurasyon dosyamızın aynı klasörde olmalarına dikkat edelim.
![mk76_3.gif](/assets/images/2004/mk76_3.gif)
Artık sunucu tarafındaki işlemlerimizi bitirmiş olduk. Şimdi sunucu tarafındaki nesneyi kullanacak istemci uygulamayı tasarlayalım. Elbetteki, istemci uygulamada, mesajlarını Tcp protokolünü baz alarak bir kanal üzerinden göndermek zorundadır. çünkü sunucu uygulama Tcp protokolünü baz alarak dinleme gerçekleştirecektir. Bunu sağlamak için öncelikle, istemci tarafında, client channel (istemci kanal) nesnesine ihtiyacımız vardır. İlave olarak, kullanacağımız uzak nesneninde bir koypasını, istemci uygulamanın assembly'ının bulunduğu klasöre koymamız gerekiyor. Bir istemci uygulama için önemli olan unsurlar, kanal nesnesi ayarları ve uzak nesneye ait ayarlardır. Bu ayarlamaları programatik olarak yapabileceğimiz gibi bir konfigurasyon dosyasında da yapabiliriz. Bu amaçla öncelikle aşağıdaki istemci.config dosyasını oluşturalım.

elemanında ise, client channel nesnesi belirtilmiştir. Bu nesne yine ref niteliği ile, sistemde machine.config dosyasında daha önceden tanımlanmış tcp client niteliğini referans etmektedir. Artık istemci uygulamamızıda yazabiliriz.

Görüldüğü gibi istemci uygulamada, konfigurasyon ayarlarını almak için RemotingConfiguration sınıfının Configure metodunu kullanır. Bundan sonra tek yapılan uzak nesne örneğini oluşturmak ve ilgili metodu çağırmaktır. Yazdığımız bu istemci.cs dosyasını aşağıdaki şekilde derleyeylim.
![mk76_4.gif](/assets/images/2004/mk76_4.gif)
Burada dikkat edilecek olursa, uzaknesne.dll dosyası assembly'a referans olarak bildirilmiştir. Dolayısıyla, uzaknesne.dll'inin bir kopyasının istemci uygulamanın bulunduğu klasörde olması gerektiğine dikkat etmeliyiz.
![mk76_5.gif](/assets/images/2004/mk76_5.gif)
Şimdi remoting sistemimizi bir test edelim. öncelikle sunucu uygulamamızı, istemcilerden gelecek talepleri dinlemek amacıyla başlatmamız gerekiyor.
![mk76_6.gif](/assets/images/2004/mk76_6.gif)
Bu işlemin ardından istemci uygulamamızıda çalıştıralım. Sonuçlar aşağıdaki gibi olacak ve sql sunucusunda yer alan Northwind veritabanındaki Customers tablosundaki CustomerID değeri ALFKI alan satır bilgileri elde edilecektir.
![mk76_7.gif](/assets/images/2004/mk76_7.gif)
Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.