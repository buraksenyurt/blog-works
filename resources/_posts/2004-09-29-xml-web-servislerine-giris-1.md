---
layout: post
title: "Xml Web Servislerine Giriş - 1"
date: 2004-09-29 09:00:00 +0300
description: "Bir Web Servisi, uzak istemcilerin başvuruda bulunduğu çeşitli işlevsel metod çağırımlarını bardındırdan, çok yönlü ve merkezileştirilmiş bir ünitedir. Bir web servisi, çok sayıda istemci tarafından erişilebilen bir yapıya sahiptir. Onu diğer dağıtık nesne modellerinden farklı kılan sahip olduğu alt yapı sistemi sayesinde, platform bağımsız uygulanabilirliği sağlamasıdır. Web servislerinin geliştirilmesinde en büyük etken, özellikle bir merkezdeki uygulamalar üzerinde, ortak amaçları gerçekleştiren işlevselliklere sahip nesnelerin, geliştirildikleri ağın iletişim protokolü gibi kısıtlamaların varlığıdır."
categories:
  - xml-web-services
tags:
  - xml-web-services
  - csharp
  - bash
  - xml
  - dotnet
  - soap
  - web-service
  - http
  - iis
  - java
  - concurrency
  - visual-studio
  - asmx
---
# Xml Web Servislerine Giriş - 1
Merhaba Arkadaşlar,

Bu makalemizde, kısaca bir XML Web Servisinin ne olduğuna, ne işe yaradığına değinecek ve basit bir Xml Web Servisinin notepad ile nasıl oluşturulabileceğini incelemeye çalışacağız.

Bir Web Servisi, uzak istemcilerin başvuruda bulunduğu çeşitli işlevsel metod çağırımlarını bardındırdan, çok yönlü ve merkezileştirilmiş bir ünitedir. Bir web servisi, çok sayıda istemci tarafından erişilebilen bir yapıya sahiptir. Onu diğer dağıtık nesne modellerinden farklı kılan sahip olduğu alt yapı sistemi sayesinde, platform bağımsız uygulanabilirliği sağlamasıdır. Web servislerinin geliştirilmesinde en büyük etken, özellikle bir merkezdeki uygulamalar üzerinde, ortak amaçları gerçekleştiren işlevselliklere sahip nesnelerin, geliştirildikleri ağın iletişim protokolü gibi kısıtlamaların varlığıdır.

Bir web servisi, standart olarak HTTP iletişim protokolü üzerinden veri alışverişine izin veren bir yapıdadır. HTTP tabanlı bu sistemin bilgi otobanı XML temelleri üzerine dayandırılmıştır. XML'in bizlere sağladığı esneklik, kolay geliştirilebilirlik özelliklerinin yanı sıra, sağlamış olduğu global standartlar, platform bağımsız veri transferi kavramını dahada geliştirmiştir. Web servislerinin kullanılmasında yatan en büyük kavram xml tabanlı veri akışının belirli standartlar dahilinde gerçekleştirilmesidir. Bu, web servislerinin platform bağımsız olarak herhangibir ateş duvarına (Firewall) yakalanmadan istemciler ile haberleşebilmesini sağlar.

![mk98_1.gif](/assets/images/2004/mk98_1.gif)

Şekil 1. XML Web Servisleri Neyi İfade Eder?

Bir web servisi, tek başına bir anlam ifade etmez. Web servisini kullanan istemcilerin de olması gerekir. İstemciler internet ortamında olabileceği gibi, çalıştığımız şirketin network sisteminde yada evimizdeki makinenin yerel sunucusu üzerinde olabilir. Bir istemci, bir web servisini kullanmak istediğinde tek yapması gereken, bu web servisi ile konuşabilecek ortak bir takım standartları uygulamaktır. XML tabanlı bu standartlar sayesinde istemciler, web servisine ulaşabilir, bu servis üzerinden metodlar çağırabilir, bu metodlara parametreler gönderebilir ve metodlardan dönen değerleri örneğin veri kümelerini elde edebilir.

Web servislerinin kullanımına verilebilecek en güzel örnek, hava durumuna ilişkin bilgilerinin en güncel halleriyle, çeşitli platformlarda çalışan istemcilere sunulduğu bir sistem olabilir. Hava durumuna ilişkin çeşitli bilgileri bir veri kümesi halinde tedarik eden merkezi web servisine istemciler, bir web sayfasından, bir windows yada java uygulmasından, bir mobil uygulamadan veya başka bir platformdan kolayca erişebilir. Web servisleri merkezi uygulamalar olduklarından, verilerdeki değişiklikler bu servisleri kullanan tüm uygulamalar için de eş zamanlı ve eş güncellikte olacaktır. Web servisleri ve bu servisleri kullanan istemciler arasındaki ilişkiler, yüzeysel olarak bakıldığında aşağıdaki şekilde görüldüğü gibi değerlendirilebilir.

![mk98_2.gif](/assets/images/2004/mk98_2.gif)

Şekil 2. En basit haliyle Xml Web Servislerinin hayatımızdaki yeri.

Web servislerinin, onları kullanan istemciler ile arasındaki ilişki, şüphesiz ki bu şekilde göründüğü kadar basit değildir. Herşeyden önce, web servislerini kullanacak istemciler ile arada kurulucak ilişkinin belli standartlara dayandırılması gerekir. Her ne kadar, web servisleri HTTP üzerinden gidecek XML veri parçalarını kullanıyor olsada bunların, istemcilerin işleyebileceği ve anlayabileceği bir hale getirilmeleri gerekir. Bu noktada devreye Web Servisleri için önemli ve gerekli temellerden birisi olan SOAP (Simple Object Access Protocol - Basit Nesne Erişim Antlaşması) girer.

Bir istemci, kullanacağı web servisine ait bir takım bilgilere sahip olmak zorundadır. İstemci bu bilgileri kullanarak web servisinden SOAP protokolüne uygun olarak hazırlanan XML mesajını gönderir. Kodlanarak (Encoding) gönderilen bu mesaj, Web Servisi tarafından çözülür (Decoding), gerekli parametreler ve metod çağırım bilgileri eşliğinde bir takım işlemler gerçekleştirir. Bu işlemler sonrasında Web Servisi, istemciye döndüreceği cevap bilgileri için yine SOAP protokolüne uygun XML mesajlarını oluşturur. Bu mesajlar HTTP üzerinden istemci uygulamaya ulaşır, burada çözülür ve değerlendirilir.

Elbette şekildeki senaryoda yer alan istemci sistemler bunlar ile sınırılı değildir. Çok çeşitli platformlar da web servislerini kullanabilir. Web servisinin önünde çalışan bir Firewall (Ateş Duvarı) olması bilgilerin kolayca taşınabilmesini engellemez. Çünkü mesajlar, istemciler ve web servisleri arasında XML tabanlı bilgi parçacıkları şeklinde taşınmaktadır.

İstemcilerin kullanacakları web servisindeki bilgileri önceden bilmeleri gerekir. WSDL (Web Services Description Language - Web Servisleri Tanımlama Dili) bu noktada devreye giren bir diğer önemli unsurdur. İstemci uygulamalar WSDL yardımıyla, kullanacakları web servisine ait bilgileri önceden tedarik ederler. Bu istemcinin web servisi üzerindeki bir web metodunun varlığından haberdar olması, onu nasıl kullanacağını bilmesi anlamına gelmektedir. Web servislerinin mimarisini daha derin ve detaylı bir şekilde incelemeden önce, ilk Web Servisi uygulamamızı yazmaya başlıyoruz.

Bir Web Servisini oluşturmak için, Notepad gibi basit bir metin editorunu kullanabileceğimiz gibi, Visual Studio.NET gibi ileri seviyede bir yazılım geliştirme platformunuda kullanabiliriz. İlk önce Notepad üzerinden bir web servisinin nasıl yazılacağını göreceğiz. Bir web servisi her şeyden önce, intranet veya internet üzerinde yer alan bir sunucuda konuşlandırılmalıdır. Yerel bir makinede bu iş için, IIS (Internet Information Services) kullanılabilir. Bu nedenle ilk olarak, web servisimizi barındırıcak sanal bir klasör oluşturmakla işe başlamalıyız. Windows XP işletim sistemine sahip bir bilgisayarda, IIS altında sanal klasörümüzü oluşturabilmek için Start menüsü, Administrative Tool, Internet Informatin Services kısmına girelim.

![mk98_3.gif](/assets/images/2004/mk98_3.gif)

Şekil 3. IIS

Ardından Default Web Site (yada web sunucusunun adı) kısmında sağ menü tuşuna basıp New kısmından Virtual Directory'yi seçelim.

![mk98_4.gif](/assets/images/2004/mk98_4.gif)

Şekil 4. Virtual Directory.

Karışımıza çıkacak olan sihirbazda Alias kısmına Geometri girelim. Klasörümüzü ise, C:\Inetpub\wwwroot\Geometri olarak oluşturalım. Buradaki adımları tamamladıktan sonra, web servisimizi bu klasör altında oluşturabiliriz. Web servisleri, asmx uzantılı dosyalar olarak oluşturulurlar. Bu nedenle, asmx uzantılı dosyamızı, yerel web sunucumuzda oluşturduğumuz sanal klasörün işaret ettiği fiziki klasörde aşağıdaki kodlar ile hazırlayalım.

```text
<% @ WebService Language="C#" CodeBehind="GeoMat.asmx.cs" class="GeoWebServis.TemelIsler" %>
```

Hazırladığımız bu dosyayı, GeoMat.asmx uzantısı ile kaydedelim. Kodlarda görüldüğü gibi, web servisimizin asıl işlevselliğini, GeoMat.asmx.cs isimli Code-Behind dosyasında gerçekleştireceğiz. Burada kullanacağımız programlama dilinide Language özelliğine C# değerini atayarak belirledik. Ancak en önemlisi kaydedilen bu asmx dosyasının bir web servisi olarak değerlendirileceğini belirten WebService anahtar sözcüğünün kullanılmasıdır. Şimdi Code-Behind dosyamızı oluşturalım.

```csharp
using System;
using System.Web;
using System.Web.Services;

namespace GeoWebServis
{

    [WebService(Namespace="http://ilk/servis/",Description="Geometrik Hesaplamalar Üzerine Metodlar İçerir. Ucgen, Dortgen gibi şekillere yönelik alan ve çevre hesaplamaları.",Name="Geometrik Hesaplamalar")]
    public class TemelIsler : System.Web.Services.WebService
    {
        private const double pi=3.14;

        [WebMethod(Description="Daire Alan Hesabı Yapar")]
        public double DaireAlan( double r)
        {
            return (r*r)*pi;
        }

        [WebMethod(Description="Daire Çevre Hesabı Yapar.")]
        public double DaireCevre( double r)
        {
            return 2*pi*r;
        }
    }
}
```

Yazdığımız bu dosyayı GeoMat.asmx.cs ismi ile asmx dosyamızın bulunduğu klasöre kayıt edelim. Kodları kısaca incelediğimizde ilk olarak, köşeli parantezler içerisindeki ifadeler dikkatimizi çekmektedir. Bu ifadeler birer nitelik (attribute) olup, web servisindeki sınıf ve metodlara ilişkin bir takım bilgileri, bu servisi kullanacak olan istemcilere sağlarlar.

Örneğin WebService niteliğinde, Namespace özelliği ile, bu servisin tanımını içerecek XML dökümanında kullanılacak Namespace'i belirtmiş oluruz. Description özelliği ile, web servisindeki bu sınıfın neler yaptığını özetleyen kısa bilgileri belirleriz. Name özelliği ilede, bu sınıfa bir isim vermiş oluruz. Bu bilgiler özellikle web servisini keşfettiğimizde (Discovery), oldukça işe yaramaktadır. WebService niteliğine benzer olarak WebMethod niteliği, izleyen metodun bir web servisi metodu olduğunu belirtmek için kullanılır. Burada iki web servisi metodumuz yer almaktadır. Her birinin ne iş yaptığına dair kısa bilgileride WebMethod niteliğinin Description özelliği ile belirtebiliriz.

Burada kullandığımız isim alanı ve sınıf adlarının, asmx dosyasında belirtiğimiz class tanımlamalarındaki ile aynı olduğuna dikkat edelim. Bir web servisi yazdığımızda, bu web servisini oluşturacak olan sınıfın, System.Web.Services.WebService isim alanından türetilmesi gerekir. Yazmış olduğumuz bu web servisi temel olarak iki metoda sahiptir ve bu metodların herbiri double türünden birer parametre alarak, yine double türünden sonuçlar üretmekte ve metodun çağırıldığı yere döndürmektedirler. Burada kullanılan parametreler istemci bilgisayarların, web servisindeki ilgili metodları çağırımları sırasında kullanılır. Dönüş değerleri ise, ilgili metodları çağıran istemcilere gönderilir.

Web servisimize ait asmx dosyamızı ve Code-Behind dosyamızı oluşturduktan sonra, Code-Behind dosyamızı dll kütüphanesi olarak derleyip, bin isimli bir klasör içerisine koymalıyız. Bu amaçla komut satırından aşağıdaki komutu vererek, Code-Behind dosyamızı bir sınıf kütüphanesi olacak şekilde csc yardımıyla derliyoruz.

```bash
csc /target:library GeoMat.asmx.cs
```

Oluşturulan dll dosyasını asmx dosyamızın blunduğu klasör altındaki bin isimli bir klasör içerisine taşıdıktan sonra web servisimize herhangibir tarayıcıdan rahatlıkla erişebiliriz. Bunun için, Internet Explorer penceresinde adres satırına

```bash
http://localhost/Geometri/GeoMat.asmx
```

url bilgisini yazmamız yeterli olucaktır. Bu işlem sonrasında web servisimizin çalışır hali aşağıdaki gibi olacaktır.

![mk98_5.gif](/assets/images/2004/mk98_5.gif)

Şekil 5. Xml Web Servisinin tarayıcıdan talep edilmesinin sonucu.

Görüldüğü gibi asmx uzantılı dosyamızı kullanarak, geliştirmiş olduğumuz web servisine ait bilgilere eriştik. Burada yazılan bilgilerin, WebService ve WebMethod niteliklerinde belirtmiş olduğumuz değerlerin aynısı olduğu dikkatinizi çekmiştir. Dilersek bu pencerede servisimizi deniyebiliriz. Web servisimizde geliştirdiğimiz metodlara ait linklerden hernangibirisine tıkladığımızda, bu metodu çağırmamız için kullanabileceğimiz bir sayfa ile karşılaşırız.

![mk98_6.gif](/assets/images/2004/mk98_6.gif)

Şekil 6. Bir Web Metodunun çağırılması.

Bu ekranda, value ile belirtilen bir metin kutusu olduğuna dikkat edin. Bu metin kutusu metodumuzun dışarıdan aldığı double tipten parametreye istinaden oluşturulmuştur. Buraya bir değer girerek metodun çağırılmasını sağlayabiliriz. Bu durumda metodun çalıştırılması sonucu elde edilecek sonuç (lar) bir XML bilgisi şeklinde elde edilecek ve tarayıcı penceresinde aşağıda olduğu gibi görünecektir.

![mk98_7.gif](/assets/images/2004/mk98_7.gif)

Web servisimizin ana sayfasında Service Description isimli bir bağlantı vardır. Bu bağlantıya tıkladığımızda aşağıdaki gibi uzun bir XML bilgisi elde ederiz.

```xml
<?xml version="1.0" encoding="utf-8" ?>
 <definitions xmlns:http="http://schemas.xmlsoap.org/wsdl/http/" xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/" xmlns:s="http://www.w3.org/2001/XMLSchema" xmlns:s0="http://ilk/servis/" xmlns:soapenc="http://schemas.xmlsoap.org/soap/encoding/" xmlns:tm="http://microsoft.com/wsdl/mime/textMatching/" xmlns:mime="http://schemas.xmlsoap.org/wsdl/mime/" targetNamespace="http://ilk/servis/" xmlns="http://schemas.xmlsoap.org/wsdl/">
 <types>
 <s:schema elementFormDefault="qualified" targetNamespace="http://ilk/servis/">
 <s:element name="DaireAlan">
 <s:complexType>
 <s:sequence>
  <s:element minOccurs="1" maxOccurs="1" name="r" type="s:double" />
  </s:sequence>
  </s:complexType>
  </s:element>
 <s:element name="DaireAlanResponse">
 <s:complexType>
 <s:sequence>
  <s:element minOccurs="1" maxOccurs="1" name="DaireAlanResult" type="s:double" />
  </s:sequence>
  </s:complexType>
  </s:element>
 <s:element name="DaireCevre">
 <s:complexType>
 <s:sequence>
  <s:element minOccurs="1" maxOccurs="1" name="r" type="s:double" />
  </s:sequence>
  </s:complexType>
  </s:element>
 <s:element name="DaireCevreResponse">
 <s:complexType>
 <s:sequence>
  <s:element minOccurs="1" maxOccurs="1" name="DaireCevreResult" type="s:double" />
  </s:sequence>
  </s:complexType>
  </s:element>
  </s:schema>
  </types>
 <message name="DaireAlanSoapIn">
  <part name="parameters" element="s0:DaireAlan" />
  </message>
 <message name="DaireAlanSoapOut">
  <part name="parameters" element="s0:DaireAlanResponse" />
  </message>
 <message name="DaireCevreSoapIn">
  <part name="parameters" element="s0:DaireCevre" />
  </message>
 <message name="DaireCevreSoapOut">
  <part name="parameters" element="s0:DaireCevreResponse" />
  </message>
 <portType name="Geometrik_x0020_HesaplamalarSoap">
 <operation name="DaireAlan">
  <documentation>Daire Alan Hesabi Yapar</documentation>
  <input message="s0:DaireAlanSoapIn" />
  <output message="s0:DaireAlanSoapOut" />
  </operation>
 <operation name="DaireCevre">
  <documentation>Daire Çevre Hesabi Yapar.</documentation>
  <input message="s0:DaireCevreSoapIn" />
  <output message="s0:DaireCevreSoapOut" />
  </operation>
  </portType>
 <binding name="Geometrik_x0020_HesaplamalarSoap" type="s0:Geometrik_x0020_HesaplamalarSoap">
  <soap:binding transport="http://schemas.xmlsoap.org/soap/http" style="document" />
 <operation name="DaireAlan">
  <soap:operation soapAction="http://ilk/servis/DaireAlan" style="document" />
 <input>
  <soap:body use="literal" />
  </input>
 <output>
  <soap:body use="literal" />
  </output>
  </operation>
 <operation name="DaireCevre">
  <soap:operation soapAction="http://ilk/servis/DaireCevre" style="document" />
 <input>
  <soap:body use="literal" />
  </input>
 <output>
  <soap:body use="literal" />
  </output>
  </operation>
  </binding>
 <service name="Geometrik_x0020_Hesaplamalar">
  <documentation>Geometrik Hesaplamalar Üzerine Metodlar Içerir. Ucgen, Dortgen gibi sekillere yönelik alan ve çevre hesaplamalari.</documentation>
 <port name="Geometrik_x0020_HesaplamalarSoap" binding="s0:Geometrik_x0020_HesaplamalarSoap">
  <soap:address location="http://localhost/Geometri/GeoMat.asmx" />
  </port>
  </service>
</definitions>
```

Burada görüldüğü gibi bir XML belgesine neden ihtiyacımız olabilir? Dikkat edilecek olursa, bu XML belgesinde, Web Servisimize ait bir takım bilgiler yer almaktadır. Örneğin, Metodlara ilişkin parametre bilgileri veya WebService ve WebMethod niteliklerinde belirttiğimiz tanımlamalar gibi. İşte istemci bilgisayarlar bu XML çıktısını kullanarak, iletişim kuracakları web servsileri hakkında bilgi sahibi olurlar. Ancak burada asıl önemli olan nokta, Service Description bağlantısının, aşağıdaki şekilde oluşudur. Bu aslında, bir istemcinin, herhangibir web servisi hakkındaki bilgilere nasıl ulaşabileceğini göstermektedir.

```bash
http://localhost/Geometri/GeoMat.asmx?WSDL
```

Bir Web Servisini Visual Studio.NET ortamında geliştirmek, şu ana kadar yaptıklarımızdan pek farklı değildir. Ancak Visual Studio.Net ortamının sağladığı avantajlar nedeniyle çok daha kolaydır. Bir sonraki makalemizde bir web servisinin Visual Studio.Net ortamında nasıl geliştirileceğini ve bu web servisine erişecek bir istemcinin nasıl yazılacağını incelemeye çalışacağız. Hepinize mutlu günler dilerim.