---
layout: post
title: "Asp.Net’ ten HTTPS Tabanlı WCF Çağrısı Gerçekleştirmek"
date: 2014-08-13 12:00:00 +0300
categories:
  - wcf
  - wcf-4-5
tags: []
---
Özel Ajan Oso! Son yılımda Disney Channel’ de sıklıkla maruz kaldığım bir çizgi karakter. Aslında bu sakar ve bir o kadar da maharetli ve sevimli çizgi dizi kahramının görevi son derece basit. Sadece 3 adımda çocuklara yol gösterici nitelikte yardımcı olmaya çalışmak.

![381eed443562d941546485cc9e2decf4_1316198153](/assets/images/2014/381eed443562d941546485cc9e2decf4_1316198153_thumb.jpg)


Ajanımızın dizi de bir de yöneticisi var. Aynı Mission Impossible’ da olduğu gibi. Onun adı “Bay Dost”

Her bölüm Özel Ajan Oso’ ya bir görev veriliyor. Söz gelimi çocuklardan birisi yanlışlıkla kumbarasının bir ayağını kırmış olsun. Bunu nasıl tamir eder? 3 adımda. Dizi de olay şöyle ilerler.

Adım 1: Kırılan kısma tutkal sür

Adım 2: Kumbara ile kırılan kısmı birleştir

Adım 3: 7 saniye boyunca birleşik şekilde tut

gibi

Bazen biz geliştiriciler de bir işi icra edebilmek adına bu şekilde basit ve az sayıda adıma ihtiyaç duyarız. İşte bugünkü makalemizde kendimizi Özel Ajan Oso yerine koyacak ve şu senaryoyu gerçekleştirmeye çalışıyor olacağız.

Adım 0: Senaryo

Development ortamında geliştirme yapmaktayız. IIS üzerinde host edilen bir WCF Servis uygulamamız var. Bu servis uygulaması WS Security standartlarında ve SSL tabanlı bir hizmet sunmakta. Bir başka deyişle servise https üzerinden talep gönderebiliyoruz. Servisin WSDL içeriğinin elde edilebildiği adres de aslında HTTPS tabanlı.

İstemci tarafı ise Asp.Net tipinden bir web uygulaması. Bu uygulamanın söz konusu servise HTTPS tabanlı olarak talep gönderebilmesi ve cevap alabilmesi isteniyor. Development ortamında çalışıldığından gerçek bir sertifika yerine, Microsoft’ un test amaçlı X509 sertifikasının kullanılması planlanıyor.

O halde ne duruyoruz! Haydi bilgisayar başına.

Adım 1: WCF Servis Uygulamasını Oluştur

Öncelikli olarak bir WCF Service Application projesi oluşturarak işe başlayabiliriz. Test amaçlı kodlar içerecek olan servisin dahilindeki tiplerden ziyade konfigurasyon tarafında yapılan ayarlar senaryomuz gereği çok daha önemli. İlk olarak sınıf diagramımıza bakalım ve bizim için gerekli tipleri üretelim.

![wcfhttps_8](/assets/images/2014/wcfhttps_8_thumb.png)

Servis sözleşmesi;

```csharp
using System.ServiceModel;

namespace EmployeeService 
{ 
   [ServiceContract] 
    public interface IEntryService 
    { 
        [OperationContract] 
       Employee InsertEmployee(Employee newEmployee); 
    } 
}
```

Bir Employee tipinin sembolik olarak üretimi ve doldurulan Id değeri ile geri döndürülmesini üstlenen servis operasyonu söz konusudur.

Sözleşme uygulayıcısı;

```csharp
using System;

namespace EmployeeService 
{ 
    public class EntryService 
       : IEntryService 
    { 
        public Employee InsertEmployee(Employee newEmployee) 
        { 
            newEmployee.EmployeeId = 1903; 
            return newEmployee; 
        } 
    } 
}
```

Employee tipi

```csharp
namespace EmployeeService 
{ 
    public class Employee 
    { 
        public int EmployeeId { get; set; } 
        public string Title { get; set; } 
        public string FirstName { get; set; } 
        public string MiddleName { get; set; } 
        public string LastName { get; set; } 
        public int Level { get; set; } 
    } 
}
```

ve en önemli kısım, servis tarafındaki konfigurasyon içeriği

![Winking smile](/assets/images/2014/wlEmoticon-winkingsmile_136.png)

```xml
<?xml version="1.0"?> 
<configuration> 
  <system.serviceModel> 
    <bindings> 
      <wsHttpBinding> 
        <binding name="TransportSecurity"> 
          <security mode="Transport"> 
            <transport clientCredentialType="None"/> 
          </security> 
        </binding> 
      </wsHttpBinding> 
    </bindings> 
    <behaviors> 
      <serviceBehaviors> 
        <behavior name="EntryServiceBehavior"> 
          <serviceMetadata httpsGetEnabled="true"/> 
          <serviceDebug includeExceptionDetailInFaults="true"/> 
        </behavior> 
      </serviceBehaviors> 
    </behaviors> 
    <services> 
      <service 
        name="EmployeeService.EntryService" 
        behaviorConfiguration="EntryServiceBehavior"> 
        <host> 
          <baseAddresses> 
            <add baseAddress="https://localhost/EmployeeService/"/> 
          </baseAddresses> 
        </host> 
        <endpoint 
          address="EntryService.svc" 
          binding="wsHttpBinding" 
          bindingConfiguration="TransportSecurity" 
          contract="EmployeeService.IEntryService" 
        /> 
        <endpoint 
          address="mex" 
          binding="mexHttpsBinding" 
          contract="IMetadataExchange" 
      /> 
      </service> 
    </services> 
  </system.serviceModel> 
  <system.web> 
    <compilation debug="true"/> 
  </system.web> 
</configuration>
```

Senaryomuza göre WCF servis uygulamamız https tabanlı olarak hizmet veriyor olacak. Bir başka deyişle WS-I HTTP standartlarını kullanan ve transport seviyesinde güvenlik sunan bir servis çalışması söz konusu. Bu nedenle wsHttpBinding tipinin kullanılması, servisin iletişim sırasında kullanacağı güvenlik çeşidinin transport olarak belirlenmesi gerekiyor. Ayrıca servisin metadata bilgisinin de https üzerinden elde edilmesini istediğimizden, mexHttpsBinding bağlayıcı tipinin kullanılması ve davranışsal özelliklerde httpsGetEnabled niteliğinin true olarak set edilmesi gerekiyor.

> Bu arada servis uygulamasının IIS üzerinde host edilecek şekilde oluşturulduğunu ifade edelim. İlk etapta base adresin https olarak belirtilmesine rağmen IIS tarafındaki Virtual Directory’ nin HTTP tabanlı çalıştığını düşünüyoruz. Bu haliyle servisi çalıştırmaya kalkarsak, sertifikasyon ile ilişkili hatalar almamız muhtemeldir. Dolayısıyla çözüm olarak bir test sertfikası üretecek, web uygulamasının http için oluşturulan Virtual Directory’ sini IIS Manager yardımıyla ele alacak ve SSL kullanımını etkinleştireceğiz.

Adım 2: Makecert ile Test Sertifikasının Oluşturulması

Windows SDK ile birlikte gelen makecert aracını kullanarak X509 tabanlı test sertifikalarının oluşturulması mümkündür. Özellikle development süreçlerinde bu aracı kullanarak SSL tabanlı senaryoların ele alınması son derece kolaydır. Senaryomuz için örnek bir test sertifikasını aşağıdaki ekran görüntüsünde olduğu gibi basitçe üretebiliriz.

> makecert –r –pe –n “CN=makineadı" –b 01/01/2000 –e 01/01/2100 –eku 1.3.6.1.5.5.7.3.1 –ss my –sr localmachine –sky exchange –sp “Microsoft RSA SChannel Cryptographic Provider” –sy 12

[![wcfhttps_1](/assets/images/2014/wcfhttps_1_thumb.png)](/assets/images/2014/wcfhttps_1.png)

Adım 3: IIS Tarafında SSL Kullanımını Etkinleştirmek

Https tabanlı iletişimi IIS üzerinde oluşturulan web sayfalarında uygulayabilmemiz için öncelikli olarak site seviyesinde SSL kullanımının etkinleştirilmiş olması gerekmektedir. Bunun için https tipinin Bindings olarak ilave edilmesi yeterli olacaktır. SSL Settings-> Bindings –> Add kısmından aşağıdaki ekran görüntüsünde yer alan adımları takip ederek işlemlerimize devam edelim.

![wcfhttps_3](/assets/images/2014/wcfhttps_3_thumb.png)

https tipinin eklenmesi sırasında bir de SSL sertfikası sorulacaktır. Bu sertifikaların seçimi sırasında 2nci adımda üretilen sertifikanın da listelendiğini görebiliriz.

![wcfhttps_5](/assets/images/2014/wcfhttps_5_thumb.png)

Adım 4: Publish Edilmiş Virtual Directory için SSL Etkinleştirilmesi

Artık http adresi temelli oluşturulan Virtual Directory için SSL kullanımını etkinleştirebiliriz. Bunun için WCF uygulamasının SSL Settings kısmında aşağıdaki işaretlemeleri yapmamız yeterli olacaktır. Require SSL ve istemci tarafı sertifika için Accept.

![wcfhttps_4](/assets/images/2014/wcfhttps_4_thumb.png)

Özellikle Require SSL seçeneğinin aktif olabilmesi bir önceki adımlarda yaptığımız IIS tabanlı SSL etkinleştirilmesine bağlıdır.

> Örnekte kullandığımız IIS (Internet Information Services) versiyonu 7.5 dir uygulama Windows 7 Enterprise işletim sistemi üzerinde geliştirilmektedir.

Adım 5: WCF Servis Uygulamasında HTTPS Adres Bilgisinin Kullanılması

Önceden de belirttiğimiz üzere eğer IIS tarafında SSL tabanlı bir sertifikasyon söz konusu değilse WCF uygulamasının da https tabanlı bir proje adresini kullanabilmesi söz konusu değildir. Ancak önceki adımlar ile bu sorunu aşmış bulunuyoruz. Dolayısıyla aşağıdaki ekran görüntüsündeki gibi Project Url kısmında https protokolünü kullanacağımızı belirtebiliriz.

[![wcfhttps_2](/assets/images/2014/wcfhttps_2_thumb.png)](/assets/images/2014/wcfhttps_2.png)

Adım 5.5: Test

Ara adımda EntryService.svc sayfasını bir tarayıcı üzerinden açmayı deneyebiliriz. Bu durumda aşağıdaki ekran görüntüsüne ulaşmamız gerekmektedir. Dikkat edileceği üzere https tabanlı bir açılış söz konusu olmuştur.

[![wcfhttps_6](/assets/images/2014/wcfhttps_6_thumb.png)](/assets/images/2014/wcfhttps_6.png)

Ayrıca WSDL (Web Service Description Language) erişim adreslerinin de https tabanlı olduğu rahatlıkla gözlemlenebilir

![Winking smile](/assets/images/2014/wlEmoticon-winkingsmile_136.png)

Adım 6: İstemci Uygulamaya Servis Referanasının Eklenmesi

Senaryomuza göre istemci uygulamamız ASP.Net tabanlı bir web uygulamasıdır. Visual Studio ile bir Asp.Net Empty Web Application oluşturarak bu adıma başlayabiliriz. Bundan sonraki en önemli kısım ise servis referansının projeye dahil edilmesidir. Add Service Reference kısmında https tabanlı WSDL adresine talepte bulunursak, aşağıdaki ekran görüntüsünde yer alan uyarı mesajı ile karşılaşırız.

[![wcfhttps_7](/assets/images/2014/wcfhttps_7_thumb.png)](/assets/images/2014/wcfhttps_7.png)

Bu iletişim penceresinde Yes seçeneğini işaretleyerek ilerleyelim. Sonuç olarak servis tarafına ait proxy tipinin istemci tarafında üretildiğini görürüz. Client için söz konusu olan konfigurasyon içeriği ise aşağıdaki gibi olacaktır.

```xml
<?xml version="1.0"?> 
<configuration> 
    <system.web> 
      <compilation debug="true" targetFramework="4.5" /> 
      <httpRuntime targetFramework="4.5" /> 
    </system.web> 
    <system.serviceModel> 
        <bindings> 
            <wsHttpBinding> 
                <binding name="WSHttpBinding_IEntryService"> 
                    <security mode="Transport"> 
                        <transport clientCredentialType="None" /> 
                    </security> 
                </binding> 
            </wsHttpBinding> 
        </bindings> 
        <client> 
            <endpoint 
                address=https://domainName.com.tr/EmployeeService/EntryService.svc/EntryService.svc 
                binding="wsHttpBinding" 
                bindingConfiguration="WSHttpBinding_IEntryService" 
                contract="EmployeeRef.IEntryService" 
                name="WSHttpBinding_IEntryService" 
                /> 
        </client> 
    </system.serviceModel> 
</configuration>
```

Dikkat edileceği üzere wsHttpBinding tipine ait güvenlik modu Transport olarak gelmiştir. Ayrıca sunucu tarafında belirttiğimiz gibi clientCredentialType niteliği None olarak atanmıştır.

Adım 7: Test Sayfasının Oluşturulması ve Kodlanması

Bu amaçla aşağıdaki basit Web formunu hazırladığımızı düşünelim.

![wcfhttps_10](/assets/images/2014/wcfhttps_10_thumb.png)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewEmployee.aspx.cs" Inherits="ClientApp.NewEmployee" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
    <table> 
        <tr> 
            <td> 
                Title : 
            </td> 
            <td> 
                <asp:TextBox ID="txtTitle" runat="server" /> 
            </td> 
        </tr> 
                <tr> 
            <td> 
                First Name : 
            </td> 
            <td> 
                <asp:TextBox ID="txtFirstName" runat="server" /> 
            </td> 
        </tr> 
                <tr> 
            <td> 
                Middle Name : 
            </td> 
            <td> 
                <asp:TextBox ID="txtMiddleName" runat="server" /> 
            </td> 
        </tr> 
                <tr> 
            <td> 
                Last Name : 
            </td> 
            <td> 
                <asp:TextBox ID="txtLastName" runat="server" /> 
            </td> 
        </tr> 
                <tr> 
            <td> 
                Level : 
            </td> 
            <td> 
                <asp:TextBox ID="txtLevel" runat="server" /> 
            </td> 
        </tr> 
                <tr> 
            <td> 
                
            </td> 
            <td> 
                <asp:Button ID="btnSend" runat="server" Text="Insert new Employee" OnClick="btnSend_Click" /> 
            </td> 
        </tr> 
                <tr> 
            <td colspan="2"> 
                <asp:Label ID="lblResult" runat="server" /> 
            </td> 
        </tr> 
    </table> 
    </div> 
    </form> 
</body> 
</html>
```

Web sayfasından Employee tipine ait Title, FirstName, MiddleName, LastName ve Level bilgileri alınmaktadır. Bu bilgiler aşağıdaki kod parçası ile de sunucuya gönderilmekte ve sonuç olarak sunucu tarafından üretilen EmployeeId değeri Label bileşenine basılmaktadır.

```csharp
using ClientApp.EmployeeRef; 
using System; 
using System.Net; 
using System.Net.Security; 
using System.Security.Cryptography.X509Certificates;

namespace ClientApp 
{ 
    public partial class NewEmployee 
        : System.Web.UI.Page 
    { 
        protected void Page_Load(object sender, EventArgs e) 
        { 
            
        }

        protected void btnSend_Click(object sender, EventArgs e) 
        { 
            ServicePointManager.ServerCertificateValidationCallback = 
                new RemoteCertificateValidationCallback(IgnoreCertificationError);

            EntryServiceClient proxy = new EntryServiceClient("WSHttpBinding_IEntryService"); 
            Employee newEmployee=proxy.InsertEmployee(new Employee 
            { 
                FirstName = txtFirstName.Text, 
                MiddleName = txtMiddleName.Text, 
                LastName = txtLastName.Text, 
                Level = Convert.ToInt32(txtLevel.Text), 
                Title = txtTitle.Text 
            } 
            ); 
            lblResult.Text = newEmployee.EmployeeId.ToString(); 
            proxy.Close(); 
        }

        public static bool IgnoreCertificationError(object sender, 
      X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
        { 
            return true; 
        } 
    } 
}
```

Kodun belki de en önemli kısmı ServerCertificateValidationCallback tipinin kullanılması ve IgnoreCertificationError isimli metod içerisinden daima true değeri döndürülmesidir. Bunun sebebi aslında bir test sertifikası kullanmamız ve söz konusu sertifikanın validasyona tabi tutulması halinde çalışma zamanı hatası alacak olmamızdır.

[![wcfhttps_11](/assets/images/2014/wcfhttps_11_thumb.png)](/assets/images/2014/wcfhttps_11.png)

Bir başka deyişle oluşacak olan hata sürkülase edilmiştir. Malum development ortamında geliştirme yaptığımızdan bu tip görmezden gelmeleri çözümümüze katabiliriz. Birazcık hile yaptık anlayacağınız.

Artık uygulamayı çalıştırıp test edebiliriz. Eğer adımlarımızda bir sorun yoksa aşağıdaki ekran görüntüsünde olduğu gibi 1903 sonucunu alıyor olmamız gerekmektedir.

[![wcfhttps_9](/assets/images/2014/wcfhttps_9_thumb.png)](/assets/images/2014/wcfhttps_9.png)

WCF tarafında HTTPS tabanlı geliştirme ortamının hazırlanması daha önceki yıllarda biraz daha zorluydu. Ancak yeni nesil ortamlarımızda bu işlemleri gerçekleştirmek, adımlarımızda da görüldüğü üzere daha kolay. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_WCFandHTTPS.zip (63,74 kb)](/assets/files/2014/HowTo_WCFandHTTPS.zip)