---
layout: post
title: "WCF 4.0 Yenilikleri - Routing Service Geliştirmek - Hello World [Beta 1]"
date: 2009-08-26 15:03:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - csharp
  - xml
  - dotnet
  - linq
  - wcf
  - soap
  - http
  - serialization
  - generics
  - visual-studio
---
Routing Service konusu ile ilişkili [bir önceki yazımızda](https://www.buraksenyurt.com/post/WCF-40-Yenilikleri-Routing-Service)modelin sunduğu alt yapıya kısaca değinmeye çalışmış ancak bir örnek geliştirme girişiminde bulunmamıştık. Bu yazımızda ise bir Hello World örneğini geliştirmeye çalışacağız. (Örneğimizi.Net Framework Beta 1 ve Visual Studio 2010 Beta 1 ile geliştirdiğimizi bir kere daha hatırlatmak isterim.) İlk olarak sizlere, örnek senaryomuzdan bahsetmek isterim.

Router servisimizin arkasında genellikle Downstream olarak adlandırılan servislerimiz yer almaktadır. Bu servislerden birisi, kullanıcı kayıt işlemlerini (Register) üstlenirken, diğeride kullanıcı adını güncelleştirme işlemini ele almaktadır.(Tabiki bu örnekteki amaç yönlendirme servisini devreye almak olduğundan sadece iki basit operasyonun, farklı servislere dağılması üzerine yoğunlaşılmıştır) İstemci uygulama, Router servisi üzerinden yeni bir kullanıcıyı kayıt etmek veya güncellemek ile ilişkili işlemler için talepte bulunabilir.

Gelen talep aslında bir SOAP paketidir. Geliştirdiğimiz örnekte, Router servis tarafında yer alan filtreleme içerisinde, SOAP Action içeriğine göre bir ayrıştırma yapılacak ve arka planda uygun olan servis metodlarına yönlendirme işlemi gerçekleştirilecektir. Buna göre Router servisimizin, istemciden gelen paketin Action değerine bakarak bir karar vereceğini söyleyebiliriz. Elbette bunu birde programatik ortamda söyleyebilmemiz gerekmektedir

![Wink](/assets/images/2009/smiley-wink.gif)

Örneği tamamladıktan sonra oluşturduğumuz mimari tasarıma bakarsak (ki burada yazımızın başında veriyorum), ne yapmak istediğimizi daha net görebiliriz.

![blg73_Architecture.gif](/assets/images/2009/blg73_Architecture.gif)

Downstream servislerimizden olan RegisterService hizmetine TCP bazlı bir iletişim ile erişilebilmektedir. UserService isimli diğer servisimiz ise Ws HTTP protokolüne göre hizmet sunmaktadır. RouterService isimli yönlendirme servisimiz ise Basic HTTP tabanlı bir iletişim kanalı sağlamaktadır. Dikkat edileceği üzere RouterService üzerinden ayrılan iki dalın içerisindeki URL adresleri birbirlerinden farklıdır. Bu adresler aslında, RegisterService ve UserService isimli hizmetlerin ortaklaşa uyguladıkları servis sözleşmesi (Service Contract) içerisinden tanımlanan Action değerleridir.

Dolayısıyla işe ilk olarak her iki servisinde uyguladığı ortak sözleşmeyi tasarlayarak başlamamız gerekmektedir. Söz konusu sözleşme ContractLibrary isimli bir sınıf kütüphanesi içerisinde tanımlanmış olup sadece Downstream servisler tarafından kullanılmaktadır. İşte servis sözleşmemiz (Service Contract).

```csharp
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ContractLibrary
{
    // Downstream servislerin tamamının uygulayacağı ortak servis sözleşmesi
    // Namespace elementinin içeriği filtrelemelerde Action kısmına yazılacak bilgiler için önem arz etmektedir.
    [ServiceContract(Name="MemberManagementService",Namespace="http://www.azon.com/Membership/Management")]
    public interface IManagementContract
    {
        // Action değerlerini biz belirliyoruz
        [OperationContract(Action = "http://www.azon.com/Registration",ReplyAction="http://www.azon.com/RegistrationResponse")] 
        string RegisterUser(User newUser);

        [OperationContract(Action = "http://www.azon.com/UpdateUser", ReplyAction = "http://www.azon.com/UpdateUserResponse")] 
        string UpdateUserName(User oldUser, string newName);
    }
    
    [DataContract]
    public class User
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Id { get; set; }
    }
}
```

IManagementContract isimli arayüz (Interface) içerisinde tanımlanan RegisterUser ve UpdateUserName metodlarının OperationContract niteliklerine dikkat edilmelidir. Bu niteliklerde yer alan Action ve ReplyAction değerleri ile, SOAP paketleri ve WSDL içerisindeki bazı tanımlamalar doğrudan etkilenmektedir. Çok doğal olarak, Router servisi içerisindeki filtreleme tablosunda yer alan Action kriterlerinde, bu operasyonlar için tanımlanan Action değerleri ele alınmalıdır. Şimdi sırasıyla RegisterService ve UserService servislerinin olduğu örnek Console projelerimizi geliştirelim.

UserManagementService projesine ait App.Config dosyası;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceMetadata httpGetEnabled="true" />
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <services>
      <service name="UserManagementService.UserService">
        <endpoint address="" binding="wsHttpBinding" contract="ContractLibrary.IManagementContract" />
        <endpoint address="Mex" kind="mexEndpoint" />
        <host>
          <baseAddresses>
            <add baseAddress="http://localhost:3445/UserService" />
          </baseAddresses>
        </host>
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

wsHttpBinding bağlayıcı tipini kullanılan bu servisin üzerinden standart mexEndpoint yardımıyla WSDL çıktısıda (Metadata Publishing) sağlanmaktadır. Aslında bu bir zorunluluk değildir. Örnekte bu özelliği açmamın nedeni, istemci için gerekli olan proxy tipinin üretimini kolaylaştırmaktır; ki proxy tipini ürettikten sonra istemcinin config dosyasını da çok farklı bir şekilde değerlendirdiğimizi söyleyebilirim. Nitekim, istemci uygulama Downstream servislerine değil, Router servisine talepte bulunmalıdır.

UserManagementService projesine ait UserService sınıfı;

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContractLibrary;
using System.ServiceModel;

namespace UserManagementService
{
    // Senaryoya göre RegisterService sadece RegisterUser operasyonunu üstlenmek üzere tasarlanmıştır.
    class UserService
        : IManagementContract
    {
        public string RegisterUser(User newUser)
        {
            throw new NotImplementedException();
        }

        public string UpdateUserName(User oldUser, string newName)
        {
            Console.WriteLine("UpdateUserName metodu başlatıldı...");
            return String.Format("{0} isimli kullanıcı adı {1} olarak değiştirildi", oldUser.Name, newName);
        }

        public UserService()
        {
            Console.WriteLine("UserService nesnesi örneklendi");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(UserService));
            host.Open();

            Console.WriteLine("UserService hazır...");
            Console.ReadLine();

            host.Close();
        }
    }
}
```

UserService sınıfı, ContractLibrary sınıf kütüphanesi içerisinde yer alan IManagementContract isimli servis sözleşmesini implemente etmektedir. Dikkat edileceği üzere RegisterUser metodu için bir implemantasyon gerçekleştirilmemiş ve hatta bilinçli olarak NotImplementedExcetion tipinden bir istisna (Exception) nesnesi fırlatılmıştır. Nitekim bu istisna mesajını hiç almayacağımızı garanti edebilirim.

![Wink](/assets/images/2009/smiley-wink.gif)

Servisimizi tamamladıktan sonra çalıştırıp WSDL çıktısına baktığımızda aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaşırız.

![blg73_Wsdl.gif](/assets/images/2009/blg73_Wsdl.gif)

Görüldüğü gibi kutucuk içine alınan bölgelerde, OperationContract niteliğinin Action özelliklerine atanan değerler yer almaktadır. Ben örnekte ilerlerken tam bu noktada bir istemci uygulama oluşturup Add Service Reference ile söz konusu WSDL çıktısının karşılığı olan Reference.cs dosyasının ürettirilmesini tercih ettim. Ancak sonrasında istemci uygulama tarafında sadece Reference.cs dosyasının içeriğini bıraktım. Yani config dosyasının içeriğini ve Service Reference klasörünün tamamını (Reference.cs hariç) sildim. Bu durumda istemci tarafının RegisterUser ve UpdateUserName metodlara çağrı yapabilmesi managed olarak mümkün hale geldi. Her neyse...Vakit kaybetmeden InternalService isimli Console uygulamamızı ve RegisterService isimli servisimizi tasarlayarak yolumuza devam edelim.

InternalService projesine ait App.config içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <services>
      <service name="InternalService.RegisterService">
        <endpoint address="" binding="netTcpBinding" contract="ContractLibrary.IManagementContract" />
        <host>
          <baseAddresses>
            <add baseAddress="net.tcp://localhost:4001/RegisterService" />
          </baseAddresses>
        </host>
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

RegisterService isimli servis netTcpBinding bağlayıcı tipini kullanmakla birlikte aynen UserService'te olduğu gibi ContractLibrary sınıf kütüphanesi içerisindeki IManagementContract arayüzünü uygulamaktadır.

InternalService projesinde yer alan RegisterService sınıfı içeriği;

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContractLibrary;
using System.ServiceModel;

namespace InternalService
{
    // Senaryoya göre RegisterService sadece RegisterUser operasyonunu üstlenmek üzere tasarlanmıştır.
    class RegisterService
        :IManagementContract
    {
        public string RegisterUser(User newUser)
        {
            Console.WriteLine("RegisterUser metodu başlatıldı...");
            return String.Format("{0} isimli kullanıcı oluşturuldu...Id = {1}", newUser.Name, Guid.NewGuid().ToString());
        }

        public string UpdateUserName(User oldUser, string newName)
        {
            throw new NotImplementedException();
        }

        public RegisterService()
        {
            Console.WriteLine("RegisterService nesnesi örneklendi");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(RegisterService));
            host.Open();

            Console.WriteLine("RegisterService hazır...");
            Console.ReadLine();

            host.Close();
        }
    }
}
```

Bu kez, RegisterUser metodu uygulanmış ancak UpdateUserName metodu içerisinden NotImplementedException istisna örneğinin fırlatılması sağlanmıştır.

Artık yönlendirme servisinin yazılmasına başlanabilir. Yönlendirme servisi için belkide en önemli nokta konfigurasyon içeriğidir. Bununla birlikte yönlendirme servisinin, Downstream servislerine ait referansları bilinçli olarak (Örneğin Add Service Reference yardımıyla) eklemesine de gerek yoktur. Sadece Client Endpoint tanımlamalarını yapması yeterlidir. Bir başka deyişle hangi servise, hangi mesajlaşma tipi ile erişeceğini bilmesi yeterlidir. İşte yazımızın kalbini oluşturan yere geldik...

![Laughing](/assets/images/2009/smiley-laughing.gif)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <routing routingTableName="RTable"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <client>
      <endpoint address="http://localhost:3445/UserService" binding="wsHttpBinding" contract="*" name="UserServiceEndpoint" />
      <endpoint address="net.tcp://localhost:4001/RegisterService" binding="netTcpBinding" contract="*" name="RegisterServiceEndpoint" />
    </client>
    <services>
      <service name="System.ServiceModel.Routing.RoutingService">
        <host>
          <baseAddresses>
            <add baseAddress="http://localhost:6501/User/Management/RouterService"/>
          </baseAddresses>
        </host>
        <endpoint binding="basicHttpBinding" contract="System.ServiceModel.Routing.IRequestReplyRouter"/>
      </service>
    </services>
    <routing>
      <filters>
        <filter name="RegisterFilter" filterType="Action" filterData="http://www.azon.com/Registration"/>
        <filter name="UpdateUserFilter" filterType="Action" filterData="http://www.azon.com/UpdateUser"/>
      </filters>
      <routingTables>
        <table name="RTable">
          <entries>
            <add filterName="RegisterFilter" endpointName="RegisterServiceEndpoint"/>
            <add filterName="UpdateUserFilter" endpointName="UserServiceEndpoint"/>
          </entries>
        </table>
      </routingTables>
    </routing>
  </system.serviceModel>
</configuration>
```

Aslında bu konfigurasyon içeriğine bir kaç dakika gözle bakmakta ve kafamızda gerekli bağlantıları yaparak neyin ne olduğunu anlamaya çalışmakta yarar olduğu kanısındayım. İlk olarak bir routing davranışını belirlendiğini hemen görebiliriz. Bu davranışın routingTableName niteliğine atanan değer ile, hangi filtreleme tablosuna bakılacağı belirlenmektedir. Yönlendirme ile ilgili eşleştirmelerin tamamı, routing elementi içerisinde yapılır. Dikkat edileceği üzere iki adet filtre belirlenmiştir. Bunların her ikiside Action tipindedir. Yani filterData niteliğine atanan değer, gelen taleplerin SOAP Action kısımlarında aranır. Peki bulunduklarında ne olur?

table elementi altında yer alan entries alt boğumunda, bir filtrenin belirttiği kritere uyulması halinde hangi istemci endPoint noktasının devreye sokulacağı belirtilmektedir. Çok doğal olarak çalışma zamanı, gelen Action bilgisinin eş düştüğü Endpoint'i bulduktan sonra, yönlendirmeyi hangi Downstream tipine doğru yapacağını kolayca bilecektir. Önemli olan noktalardan bir diğeride, servisin Endpoint bilgisidir. Burada görüleceği üzere daha önceki yazımızdan da hatırlayacağınız built-in routing sözleşmelerinden birisi seçilmiştir. Buna göre operasyonlarımızda request/reply modeli söz konusu olduğundan IRequestRepyleRouter servis sözleşmesinden yararlanılmaktadır. Artık yönlendirme servisinin kodlarını aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Routing;

namespace Router
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(RoutingService));
            host.Open();
            Console.WriteLine("Routing Service hazır...");
            Console.ReadLine();
            host.Close();

        }
    }
}
```

Tek dikkat edilmesi gereken, System.ServiceModel.Routing assembly'ından gelen RoutingService tipinin kullanılmış olmasıdır. Bu sayede çalışma zamanında yönlendirme işlemleri için gerekli alt yapının oluşturulması sağlanacaktır. Artık geriye istemci tarafını tamamlamaktan başka bir şey kalmamaktadır. Yupiiii!!!

![Cool](/assets/images/2009/smiley-cool.gif)

İşte istemci tarafının App.config dosyası içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
      <client>
        <endpoint address="http://localhost:6501/User/Management/RouterService"
                  contract="MemberManagementSpace.MemberManagementService"
                  binding="basicHttpBinding"/>
      </client>
    </system.serviceModel>
</configuration>
```

Görüldüğü üzere Endpoint tanımlamasında, RoutingService adresi belirlenmiş ve sözleşme tipi olarak daha önceden projeye eklediğimiz Reference.cs içerisine otomatik olarak üretilen MemberManagementSpace.MemberManagementService atanmıştır.

ve Main metodu kodları;

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientApp.MemberManagementSpace;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("İstemci hazır...Başlamak için tuşa basınız");
            Console.ReadLine();

            MemberManagementServiceClient client = new MemberManagementServiceClient();

            User burak = new User { Name = "Burak Selim Şenyurt" };
            
            string registerResult=client.RegisterUser(burak);
            Console.WriteLine(registerResult);

            string updateResult=client.UpdateUserName(burak, "Burki");
            Console.WriteLine(updateResult);

            Console.ReadLine();
            client.Close();
        }
    }
}
```

Görüldüğü gibi istemci uygulama çok normal olarak servislerin uyguladığı ortak sözleşme şemasına sadık kalacak şekilde taleplerde bulunur. RegisterUser ve UpdateUserName metod çağırıları. Ancak hangisi olursa olsun, tüm bu operasyon çağrılarına ilişkin talepler Routing Servisine uğrayacaktır. Görüldüğü gibi istemcinin, Downstream servislerini bilmesine gerek yoktur. Zaten istemci uygulamada, söz konusu Downstream servislerine ait referansların ve config içeriğinin olmayışı bunu kanıtlamaktadır. Action talepleri, yönlendirme servisi tarafından değerlendirilip alt servislere iletildikten sonra, üretilen cevaplar yine Routing servisi üzerinden istemci tarafına gönderilir.

Artık örneği test etmeye ne dersiniz? İşte benim aldığım sonuçlar;

![blg73_Runtime.gif](/assets/images/2009/blg73_Runtime.gif)

Umarım sizlerde benzer sonuçları elde edebilirsiniz. Herşey yolunda görünüyor.

![Smile](/assets/images/2009/smiley-smile.gif)

Örnekte özetle neler yaptık?

A

DownStream Services

UserManagementService

Adres

http://localhost:3445/UserService

Binding

wsHttpBinding

Metadata Publishing

true (Sadece istemci için gerekli Reference içeriğinin kolay elde edilmesi için açıldı. Kapatılabilir)

Sözleşme

IManagementContract (ContractLibrary içerisindeki servis sözleşmesidir)

Action

http://www.azon.com/UpdateUser

InternalService

Adres

net.tcp://localhost:4001/RegisterService

Binding

netTcpBinding

Sözleşme

IManagementContract (ContractLibrary içerisindeki servis sözleşmesidir)

Action

http://www.azon.com/RegisterUser

B

Router Service

(DownStream servislerinin Endpoint bilgilerini barındırır ama bu servislere ait referanslar tiplerini içermez)

Service Endpoint Adresi

http://localhost:6501/User/Management/RouterService

Binding

basicHttpBinding

Client Endpoint Adresleri

http://localhost:3445/UserService
net.tcp://localhost:4001/RegisterService

C

Client Uygulama

Sadece Action bilgilerine sahip olan proxy tipini içerir.

Proxy tipinin üretimi için UserManagementService üzerinden açılan WSDL içeriğinden yararlanılmıştır. Ama bilindiği üzere ellede üretimi yapılıp oluşturulan sınıfın istemci tarafına verilmesi yolu da tercih edilebilir.

Downstream servislerin ait Endpoint bilgilerini içermez, bunun yerine Router servise ait Endpoint bilgisini içerir.

Proxy nesnesi üzerinde Register ve UpdateUser çağrılarını, Router servise doğru gerçekleştirir.

Elbetteki bu örnekte en kritik noktalardan birisi filtrelemelerdir. Biz örneğimizde Action içeriğine bakarak bir filtreleme işlemi gerçekleştirdik. Ancak XPath kullanımı gibi senaryolarında mümkün olduğundan bahsetmiştik. Yani talebe ait içerik üzerinden XPath sorguları ile koşula uyan durumlarıda yönlendirme işlemlerinde kullanabiliriz. Bu gibi ince noktalarıda ilerleyen yazılarımızda sizlere aktarmaya çalışıyor olacağım. Şimdilik bu kadar. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Router Project.rar (104,09 kb)](https://www.buraksenyurt.com/pics/2009%2f8%2fRouter+Project.rar)