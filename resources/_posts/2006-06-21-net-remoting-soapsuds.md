---
layout: post
title: ".Net Remoting - SoapSuds"
date: 2006-06-21 12:00:00 +0300
categories:
  - dotnet-remoting
tags:
  - .net-remoting
  - soap
---
Remoting mimarisinde, istemci ve sunucu arasında uzak nesneleri paylaşmanın dört farklı yolu vardır. İstemcilerin tek amacı sunucu üzerinde yer alan uzak nesne referanslarını kullanabilmektir. Bu açıdan bakıldığında, istemci uygulamanın uzak sunucu üzerindeki nesne referanslarının yapısını bilmesi gerektiği ortaya çıkmaktadır. Kullanılabilecek yollardan ilki uzak nesne sınıfının bulunduğu paylaşımlı bir assembly'ı tüm istemci uygulamalara dağıtmaktır. Bu istemci uygulamalar için ekstra kod yazmadan kolayca gerçekleştirilebilecek bir işlemdir. Lakin istemci uygulamalarda, uzak sınıfın tüm içeriğinin yer aldığı bir assembly'da mevcuttur. Bu da ILDASM (Intermediate Dis-Assembler Tool) ve başka üçüncü parti araçlar yardımıyla iş mantığının (business logic) istemci tarafından kolayca okunabileceği anlamına gelir. İşte bu dezavnataj nedeni ile özellikle güvenlik açısından çoğu zaman bu teknik tercih edilmez.

İkinci yöntem ve belkide en popüler olanı, uzak sınıf modelinin türetildiği bir interface tipini istemci ve sunucu arasında paylaşıma sunmaktır. Yani istemci tarafında sadece ve sadece uzak nesne modelinin bilgisini tutan bir interface tipi yer alacaktır. Bu modele göre uzak sunucu üzerindeki uzak nesne referansına polimorfizm'in bir sonucu olarak interface tipi üzerinden erişebilinir. Ama daha da önemlisi istemci tarafında, uzak nesne içerisindeki kodların kesinlikle görünmüyor oluşudur. Bu da doğal olarak iş mantığını istemciden gizleyen etkili bir tekniktir..Net Remoting uygulamalarını yazarken çoğunlukla interface kullanımı tercih edilmektedir.

Üçüncü model abstract modeli ele alır ve özellikle nesne üretiminin istemciden soyutlanması söz konusu ise Fabrika Tasarım Desenini (Factory Design Pattern) uygular. Bu model içerisinde, interface modeline benzer olarak istemci tarafından iş mantığı ve özellikle uzak nesnenin üretiliş biçimi soyutlanmaktadır. Ne yazıkki bu modelin uygulanması çoğu zaman kolay değildir. Çünkü daha ileri seviye kod yazımını gerektirmektedir. Bu modelin kullanılabilmesi için abstract mimari, tasarım desenleri gibi kavramlara aşina olmak gerekir.

Dördüncü ve son model ise istemci tarafındaki uygulamaların, kullanmak istedikleri uzak nesneye ait metadata bilgisini sağlayan SoapSuds aracını ele alır. Bu modelde, istemci tarafında uzak nesneye ait sadece tip ve üye bilgilerini içeren bir assembly söz konusudur. Yani istemci tarafında sadece uzak nesneye ait metadata bilgisi yer alır. Bu da elbetteki iş mantığını istemciden gizleyen bir modeldir. Ancak özellikle interface ve abstract modelinin etklinliği nedeni ile SoapSuds modeli değerini kaybetmektedir. Yinede SoapSuds modelini bilmekte yarar vardır. İşte bu makalemizde SoapSuds modelini incelemeye çalışacağız. Bu modeller arasında elbetteki bir takım avantaj ve dezavantajlar söz konusudur. Örneğin interface, abstract veya soapSuds modelleri bağlantısız çalışmaya izin vermezler. Bunun en büyük nedeni elbette iş yapacak nesnel kodların istemci tarafında bulunmayışıdır. Bu tip bir ihtiyaca ancak ve ancak Paylaşımlı Assembly modeli cevap verecektir. Ancak.Net Remoting uygulamalarının tasarım amacı düşünüldüğünde bu son derece uç bir örnektir. Bahsedilen dört model arasındaki temel farklılıkları ve birbirlerine olan üstünlüklerini aşağıdaki tabloda özet olarak bulabilirsiniz.

Model
Dezavantajları
Avantajları

Paylaşımlı Assembly (Shared Assembly)
İstemci tarafından iş mantığının görülebilmesi.
Geliştirme kolaylığı. Bağlantılı ve bağlantısız çalışma desteği.

Interface
Bağlantılı ve bağlantısız katman modeline cevap verememesi.
İstemci tarafından iş mantığının gizlenmesi (security).

Abstract
Bağlantılı ve bağlantısız katman modeline cevap verememesi.

Kodlamanın zor oluşu.
İş mantığının gizlenmesi (security) ve uzak nesne üretim işlemlerinin soyutlanması.

SoapSuds
Bağlantılı ve bağlantısız katman modeline cevap verememesi.

Wrapped proxy seçeneğinde sadece HTTP desteği olması.
İş mantığının istemci tarafından gizlenmesi. (security).

SoapSuds modelinde, istemci tarafında uzak nesneye ait metadata'yı içeren bir assembly üretimi söz konusudur. Framework ile gelen SoapSuds aracının en temel amacı bu assembly'ı üretmektir. Burada üretilen assembly aslında fiziki bir proxy nesnesi görevini üstlenir. Proxy'nin iki farklı üretiliş şekli vardır. Wrapped Proxy yada Non-Wrapped Proxy. Wrapped Proxy tipinde, sadece SOAP ve HTTP protokolü desteklenmektedir. Bunun dışında bu modeli uygularken istemci tarafında channel, port gibi konfigurasyon ayarlarının yapılmasına gerek kalınmaz. Çünkü bu tip bilgiler WSDL talebi sonucu üretilen Proxy Assembly'ın içerisine kaydedilmektedir. Non-Wrapped Proxy modeli ise hem HTTP hem de TCP protokolüne destek verebilmektedir (Teorik Olarak). Non-Wrapped Proxy modelinde,Wrapped Proxy Assembly'da yapılmayan konfigurasyon ayarlarının da yapılması gerekir. Hangi tip olursa olsun sonuç itibariyle SoapSuds modeli, istemcinin uzak nesneyi kullanabilmesi için gerekli bilgileri içeren bir metadata sağlar ve bunu kullanarak bir proxy assembly üretir. Şimdi dilerseniz SoapSuds modelini örnekler üzerinde incelemeye çalışalım. İlk olarak örneklerimizde kullanacağımız uzak nesneye ait sınıfımızı ve sunucu uygulamamızı tasarlayarak işe başlayacağız.

Uzak nesne sınıfı kodları;

![mk164_1.gif](/assets/images/2006/mk164_1.gif)

```csharp
namespace RemoteObjects
{
    public class Matematik:MarshalByRefObject
    {
        public Matematik()
        {
            Console.WriteLine("Uzak nesne yapıcı metod çağırıldı...");
        }
        public double Toplam(double deger1, double deger2)
        {
            return deger1 + deger2;
        }
    }
}
```

Sunucu Uygulama;

```csharp
class Program
{
    static void Main(string[] args)
    {
        RemotingConfiguration.Configure("..\\..\\App.config",false);
        Console.WriteLine("Sunucu dinlemede...");
        Console.ReadLine();
    }
}
```

Sunucu Konfigurasyon bilgisi;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.runtime.remoting>
        <application>
            <channels>
                <channel ref="Http Server" port="9800"/>
            </channels>
            <service>
                <wellknown type="RemoteObjects.Matematik,RemoteObjects" objectUri="Matematik.soap" mode="Singleton"/>
            </service>
        </application>
    </system.runtime.remoting>
</configuration>
```

Şimdi ilk olarak SopaSuds aracını sunucu üzerinde çalıştırıyor ve Wrapped Proxy'ımızı üretiyoruz. Bunun için Visual Studio.2005 Command Prompt'u kullanabiliriz. SoapSuds aracını kullanırken Proxy Assembly'ı üretebilmemiz için Sunucu uygulamanın mutlaka çalışıyor olması gerekmektedir. Bu arada yazdığımız komut satırındaki SOAP uzantısına ve WSDL (Web Service Description Language) talebine dikkat edelim. Burada oa anahtarı Output Assembly anlamına gelmektedir.

Komut: SoapSuds -url:http://manchester:9800/Matematik.soap?wsdl -oa:MatMetaData.dll

![mk164_2.gif](/assets/images/2006/mk164_2.gif)

Komutu uygularken dikkat ederseniz?wsdl takısı kullanılıyor. Bunun sebebi Metadata'nın SOAP üzerinden elde edilen WSDL dökümanına bakılarak çıkartılmasıdır. Eğer tarayıcı penceresinden http://manchester:9800/Matematik.soap?wsdl yazarsanız aşağıdakine bezner bir ekran görüntüsü elde edersiniz. Bu web servislerinden aşina olduğumuz WSDL dökümanıdır.

![mk164_5.gif](/assets/images/2006/mk164_5.gif)

Bu işlemin ardından komutu çalıştırdığımız klasörde MatMetaData.dll isimli bir assembly oluşturulduğunu görebiliriz. Bu Wrapped Proxy türündendir. Şimdi ILDASM aracı yardımıyla MatMetaData.dll ve RemoteObjects.dll'lerini birbirleriyle karşılaştıralım.

![mk164_3.gif](/assets/images/2006/mk164_3.gif)

İlk olarak çıktıların aynı olmadığı hemen göze çarpmaktadır. SoapSuds ürettiği assembly içerisine WSDL'i kullanarak uzak nesneye nasıl erişeceğine dair bilgiler içeren ek üyeler atmıştır. Dahası Matematik sınıfı içerisindeki Constructor ve Toplam metodlarının MsIL kodlarına bakacak olursak, üye içi kodların yansıtılmadığını kolayca görebiliriz. Buda istemcinin, uzak nesne iş mantığına ait kodları asla göremeyeceği anlamına gelmektedir.

MatMetaData.dll'i içindeki Matematik sınıfı için Constructor içeriği;

```text
.method public hidebysig specialname rtspecialname 
instance void .ctor() cil managed
{
// Code size 28 (0x1c)
.maxstack 8
IL_0000: ldarg.0
IL_0001: call instance void [System.Runtime.Remoting]System.Runtime.Remoting.Services.RemotingClientProxy::.ctor()
IL_0006: nop
IL_0007: nop
IL_0008: ldarg.0
IL_0009: ldarg.0
IL_000a: call instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
IL_000f: ldstr "http://manchester:9800/Matematik.soap"
IL_0014: call instance void [System.Runtime.Remoting]System.Runtime.Remoting.Services.RemotingClientProxy::ConfigureProxy(class [mscorlib]System.Type,
string)
IL_0019: nop
IL_001a: nop
IL_001b: ret
} // end of method Matematik::.ctor
```

RemoteObjects.dll içindeki Matematik sınıfı için Constructor içeriği

```text
.method public hidebysig specialname rtspecialname 
instance void .ctor() cil managed
{
// Code size 21 (0x15)
.maxstack 8
IL_0000: ldarg.0
IL_0001: call instance void [mscorlib]System.MarshalByRefObject::.ctor()
IL_0006: nop
IL_0007: nop
IL_0008: ldstr bytearray (55 00 7A 00 61 00 6B 00 20 00 6E 00 65 00 73 00 // U.z.a.k. .n.e.s.
6E 00 65 00 20 00 79 00 61 00 70 00 31 01 63 00 // n.e. .y.a.p.1.c.
31 01 20 00 6D 00 65 00 74 00 6F 00 64 00 20 00 // 1. .m.e.t.o.d. .
E7 00 61 00 1F 01 31 01 72 00 31 01 6C 00 64 00 // ..a...1.r.1.l.d.
31 01 2E 00 2E 00 2E 00 ) // 1.......
IL_000d: call void [mscorlib]System.Console::WriteLine(string)
IL_0012: nop
IL_0013: nop
IL_0014: ret
} // end of method Matematik::.ctor
```

MatMetData.dll içindeki Matematik sınıfı için Toplam metodu içeriği;

```text
.method public hidebysig instance float64 
Toplam(float64 deger1,
float64 deger2) cil managed
{
.custom instance void [mscorlib]System.Runtime.Remoting.Metadata.SoapMethodAttribute::.ctor() = ( 01 00 01 00 54 0E 0A 53 6F 61 70 41 63 74 69 6F // ....T..SoapActio
6E 55 68 74 74 70 3A 2F 2F 73 63 68 65 6D 61 73 // nUhttp://schemas
2E 6D 69 63 72 6F 73 6F 66 74 2E 63 6F 6D 2F 63 // .microsoft.com/c
6C 72 2F 6E 73 61 73 73 65 6D 2F 52 65 6D 6F 74 // lr/nsassem/Remot
65 4F 62 6A 65 63 74 73 2E 4D 61 74 65 6D 61 74 // eObjects.Matemat
69 6B 2F 52 65 6D 6F 74 65 4F 62 6A 65 63 74 73 // ik/RemoteObjects
23 54 6F 70 6C 61 6D ) // #Toplam
// Code size 24 (0x18)
.maxstack 3
.locals init ([0] float64 CS$1$0000)
IL_0000: nop
IL_0001: ldarg.0
IL_0002: ldfld object [System.Runtime.Remoting]System.Runtime.Remoting.Services.RemotingClientProxy::_tp
IL_0007: castclass RemoteObjects.Matematik
IL_000c: ldarg.1
IL_000d: ldarg.2
IL_000e: callvirt instance float64 RemoteObjects.Matematik::Toplam(float64,
float64)
IL_0013: stloc.0
IL_0014: br.s IL_0016
IL_0016: ldloc.0
IL_0017: ret
} // end of method Matematik::Toplamt
```

RemoteObjects.dll içindeki Matematik sınıfı için Toplam metodu içeriği;

```text
.method public hidebysig instance float64 
Toplam(float64 deger1,
float64 deger2) cil managed
{
// Code size 9 (0x9)
.maxstack 2
.locals init ([0] float64 CS$1$0000)
IL_0000: nop
IL_0001: ldarg.1
IL_0002: ldarg.2
IL_0003: add
IL_0004: stloc.0
IL_0005: br.s IL_0007
IL_0007: ldloc.0
IL_0008: ret
} // end of method Matematik::Toplam
```

Dikkat ederseniz, üretilen Wrapped Proxy içerisinde SOAP üzerinden HTTP protokolünü kullanarak yapılacak üye çağrıları için gerekli bilgiler yer almaktadır. Şimdi üretilen bu Wrapped Proxy'ı örnek bir istemcide kullanalım. Öncelikle yapmamız gereken SoapSuds ile üretilen Wrapped Proxy'ı istemci uygulamamıza referans etmek olacaktır.

![mk164_4.gif](/assets/images/2006/mk164_4.gif)

İstemci uygulama;

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using RemoteObjects;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Matematik mt = new Matematik();
            Console.WriteLine(mt.Toplam(3, 4).ToString());
            Console.ReadLine();
        }
    }
}
```

Burada dikkat ederseniz sıradan istemci uygulamalarındaki remoting için gerekli hiç bir konfigurasyon ayarı yoktur. Oysaki kanal bilgisinin (channel) ya da SAO (Server Activated Object) veya CAO (Client Activated Object) için gerekli kayıt işlemlerinin yapılmış olması gerekmektedir. Bunların yapılmayışının nedeni, uzak nesneyi kullanabilmek için gerekli erişim bilgilerinin, SoapSuds ile üretilen Wrapped Proxy içerisinde olmasıdır. Şimdi remote uygulamamızı test edebiliriz. (Aşağıdaki videoyu izleyebilmek için Flash Player gereklidir.)

Görüldüğü gibi, istemci uygulama çalıştığında sunucu üzerinde Matematik sınıfına ait yapıcı metod çalıştırılmıştır. Bu, istemcinin gerçekten sunucu üzerindeki bir referansı kullandığının kanıtıdır. Dahası, sunucuyu çalıştırmadan istemci uygulamayı çalıştırmayı denerseniz, "System.Net.Sockets.SocketException: No connection could be made becaus
e the target machine actively refused it" istisnasını alırsınız ki buda remote sistemin gerçekten tamalandığı anlamına gelir.

Gelelim Non-Wrapped Proxy modeline. Bu modelde, üretilen assembly içerisinde sadece sınıfa ait metadata bilgisi bulunur. Dolayısıyla, Wrapped Proxy'lerde olduğu gibi sadece HTTP protokolüne bağlı değildir. TCP protokolünüde ele alabiliriz. Non-Wrapped Proxy kullanırken istemci tarafında gerekli konfigurasyon ayaları da yapılmalıdır. Bu sayede sunucu üzerinde meydana gelecek kanal ve port değişikliklerini istemci tarafınada yansıtabiliriz. Oysaki Wrapped proxy tipine göre SoapSuds aracı ile metadata'yı içeren assembly'ı yeniden üretmemiz ve dağıtmamız gerekecektir. Yukarıdaki örneğimizi ele aldığımızda, Non-Wrapped Proxy'imizi oluşturmak için SoapSuds aracını aşağıdaki şekilde kullanmamız yeterlidir. SoapSuds aracı yardımıyla Assembly'ın üretilebilmesi için önceden sunucu uygulamanın çalışıyor olması gerektiğini lütfen unutmayınız. Non-Wrapped Proxy üretimi için SoapSuds aracında -nowp anahtarı kullanılır.

Komut: SoapSuds -nowp -url:http://localhost:9800/Matematik.soap?wsdl -oa:MatMetaDataNoWp.dll

![mk164_6.gif](/assets/images/2006/mk164_6.gif)

Şimdi oluşan MatMetaDataNoWp.dll isimli assembly'ımızı yine ILDASM aracı yardımıyla inceleyelim.

![mk164_7.gif](/assets/images/2006/mk164_7.gif)

Görüldüğü gibi tek fark Wrapped Proxy'de yer alan get_RemotingReference metodu ile RemotingReference nesnesinin Non-Wrapped Proxy içerisinde olmayışıdır. Dolayısıyla, istemci tarafındaki uygulamamızda gerekli konfigurasyon ayarlarını yapmamız gerekecektir. Bu amaçla istemci tarafına aşağıdaki konfigurasyon dosyasını ekleyelim. Elbette type parametresini belirtirken Assembly adı olarak oluşturulan Non-Wrapped Proxy Assembly'ının adını vermemiz gerekecektir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.runtime.remoting>
        <application>
            <channels>
                <channel ref="Http Client"/>
            </channels>
            <client>
                <wellknown type="RemoteObjects.Matematik,MatMetaDataNoWp" url="http://manchester:9800/Matematik.soap"/>
            </client>
        </application>
    </system.runtime.remoting>
</configuration>
```

Daha sonra ise, yeni proxy assembly'ımızın dll dosyasını istemci uygulamamıza referans etmemiz gerekiyor. (Önceki örneğimizde üretilen Assembly'da aynı isim alanını ve sınıfı içerdiğinden karşılık olmaması amacıyla MatMetaData.dll referansı bu örnek için kaldırılmıştır.)

![mk164_8.gif](/assets/images/2006/mk164_8.gif)

Şimdi istemcimize ait kodları aşağıdaki gibi değiştirelim.

```csharp
RemotingConfiguration.Configure("..\\..\\App.config",false);
Matematik mt = new Matematik();
Console.WriteLine(mt.Toplam(3, 4).ToString());
```

Uygulamamızı test ettiğimizde remoting sisteminin başarılı bir şekilde çalıştığını görebiliriz. Her zamanki gibi uzaktan erişimi ispat etmek için, sunucu uygulamayı çalıştırmadan istemci uygulamayı çalıştırmanızı öneririm. Görüldüğü gibi SoapSuds modelinde Wrapped Proxy ya da Non-Wrapped Proxy'lerin bir birlerine göre temel bazı farklılıkları bulunmaktadır. Son olarak SoapSuds aracı yardımıyla uzak nesne sınıfına ait bir kaynak kodun istemci tarafına taşınabileceğinide berlirtmek istiyorum. Bunun için -gc (Generate Class) anahtarını kullanmak yeterlidir. Örneğin sunucu uygulamamız çalışırken komut satırından aşağıdaki satırı çalıştıralım.

Komut: SoapSuds -nowp -url:http://localhost:9800/Matematik.soap?wsdl -gc

Bunun sonucu olarak cs uzantılı bir kaynak kod dosyası oluşur. Dosyanın içeriği aşağıdakine benzer olacaktır.

```csharp
using System;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.InteropServices;
namespace RemoteObjects 
{
    [Serializable, SoapType(XmlNamespace=@"http://schemas.microsoft.com/clr/nsassem/RemoteObjects/RemoteObjects%2C%20Version%3D1.0.0.0%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull", XmlTypeNamespace=@"http://schemas.microsoft.com/clr/nsassem/RemoteObjects/RemoteObjects%2C%20Version%3D1.0.0.0%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull")][ComVisible(true)]
    public class Matematik : System.MarshalByRefObject
    {
        [SoapMethod(SoapAction=@"http://schemas.microsoft.com/clr/nsassem/RemoteObjects.Matematik/
RemoteObjects#Toplam")]
        public Double Toplam(Double deger1, Double deger2)
        {
            return((Double) (Object) null);
        }
    }
}
```

Dolayısıyla istemci tarafından oluşan bu isim alanı ve içeriğini doğrudan kullanabilir yada bu kaynak kod dosyasında bir assembly üretip onu kullanmayı tercih edebiliriz. Tabi böyle bir durumda aynen Non-Wrapped Proxy modelinde olduğu gibi istemci tarafı için gerekli konfigurasyon ayarlarının bildirilmesi gerekmektedir. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](/assets/files/2006/Sample_13_SoapSuds.rar)