---
layout: post
title: "WCF 4.0 Yenilikleri - Routing Service - Hata Yönetimi [Beta 1]"
date: 2009-09-09 13:02:00 +0300
categories:
  - wcf-4-0-beta-1
tags:
  - wcf-4-0-beta-1
  - csharp
  - xml
  - wcf
  - http
---
Bir önceki [blog yazımızda](https://www.buraksenyurt.com/post/WCF-40-Yenilikleri-Routing-Service-Gelistirmek-Hello-World)WCF 4.0 ile basit bir yönlendirme servisinin (Router Service) nasıl yazılabileceğini incelemeye çalışmıtık. Tabi bu tip bir sistemde dikkat edilmesi gereken vakalardan biriside, Downstream servislerde istisnaların (Exceptions) oluşması halinde nasıl davranılacağıdır. Peki ne gibi durumlardan bahsediyoruz? Örneğin, Router servisine gelen paketin yönlendirildiği bir alt servis çalışmıyor olabilir.

![blg74_Giris.jpg](/assets/images/2009/blg74_Giris.jpg)

Bu durumda bir TimeoutException oluşması muhtemeldir. Benzer şekilde CommunicationException ve türevi olan istisna tiplerinin fırlatılmasıda söz konusudur. Bu gibi istisnaların ortaya çıkması halinde en azından işleyişin devamlılığını sağlamak ve sistemin çökmesini engellemek için, WCF 4.0 tarafı konfigurasyon dosyasında Alternatif Endpoint tanımlamaları yapılmasına izin vermektedir.

Buna göre Downstream servislerinden bahsedilen tipteki istisnalardan birisi alınırsa, istemciden gelen talebin karşılanmak üzere alternatif olarak tanımlanmış olan bir servise yönlendirilmesi sağlanmış olunur. Bu alternatif servise olan yönlendirme tamamen çalışma zamanında ve router servisin yönetimi altında gerçekleşmektedir. Konuyu daha net kavrayabilmek adına bir önceki blogumuzda yazdığımız örneği aşağıdaki vakaya göre test ettiğimizi düşünelim.

İlk etapta Router Servisimiz ile tüm DownStream servislerimiz çalıştırılır. Sonrasında istemci uygulama açıkken ve henüz taleplerini iletmeden önce Downstream servislerinden herhangibiri kapatılır. Örneğin UserService servisinin kapatıldığını düşünebiliriz. Bu durumda istemci tarafının bir CommunicationException istisnası ile sonlanması gerekmektedir.

Örneği kolay bir şekilde canlandırabilmek için Router servisimizde includeExceptionDetailInFaults özelliğine true değeri atayıp, istemci tarafındaki kod içeriğini ise aşağıdaki gibi güncelleştirmemiz yerinde olacaktır.

```csharp
using System;
using System.ServiceModel;
using ClientApp.MemberManagementSpace;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("İstemci hazır...Başlamak için tuşa basınız");
            Console.ReadLine();

            try
            {
                MemberManagementServiceClient client = new MemberManagementServiceClient();

                User burak = new User { Name = "Burak Selim Şenyurt" };

                string registerResult = client.RegisterUser(burak);
                Console.WriteLine(registerResult);

                string updateResult = client.UpdateUserName(burak, "Burki");
                Console.WriteLine(updateResult);

                Console.ReadLine();
                client.Close();
            }
            catch (CommunicationException excp)
            {
                Console.WriteLine(excp.Message);
            }
        }
    }
}
```

Test sonrasında aşağıdaki gibi bir sonuçla karşılaşmamız muhtemeldir.

![blg74_Case.gif](/assets/images/2009/blg74_Case.gif)

Görüldüğü gibi RegisterService üzerinden yapılan kullanıcı kayıt operasyonu başarılı bir şekilde çalışmış ancak, UserService üzerinden yapılan çağrı için ortama bir CommunicationException döndürülmüştür. Bu son derece doğaldır, nitekim söz konusu servis kapalıdır.

![Sealed](/assets/images/2009/smiley-sealed.gif)

Peki alternatif bir yolumuz var mıdır? Yazımızın başındada belirttiğimiz gibi, Downstream servislerinden birisinin çökmesi halinde en azından istemci talebinin bir başka yedek servis üzerine pas edilmesi sağlanabilir. İlk önce bu yedek servisi (Backup Service) geliştireceğiz. Sonrasında ise, Router servisimize ait konfigurasyon dosyasında bazı değişiklikler yapmamız gerekmektedir. Örneğimizin yeni modeli grafiksel olarak aşağıdaki gibi düşünülebilir.

![blg74_Architecture.gif](/assets/images/2009/blg74_Architecture.gif)

UserBackupService isimli yedek servisimizin konfigurasyon dosyası ve kod yapısı aşağıdaki gibidir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <services>
      <service name="UserBackupService.UserService">
        <endpoint address="" binding="wsHttpBinding" contract="ContractLibrary.IManagementContract" />
        <host>
          <baseAddresses>
            <add baseAddress="http://localhost:3446/UserBackupService" />
          </baseAddresses>
        </host>
      </service>
    </services>
  </system.serviceModel>
</configuration>
```

ve kod içeriği;

```csharp
using System;
using ContractLibrary;
using System.ServiceModel;

namespace UserBackupService
{
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
            Console.WriteLine("UserBACKUPService nesnesi örneklendi");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(UserService));
            host.Open();

            Console.WriteLine("UserBACKUPService hazır...");
            Console.ReadLine();

            host.Close();
        }
    }
}
```

Aslında UserService'in bire bir kopyası olan sadece farklı bir port üzerinden sunulan bir servis geliştirmiş bulunuyoruz. Elbetteki bu servisi farklı bir makine üzerinde farklı Endpoint kuralları ilede sunabilir ve alternatif Endpoint olarak kullanabiliriz. Gelelim bu yazının en can alıcı noktasına. Router servisine ait konfigurasyon dosyasının içeriği...

![Wink](/assets/images/2009/smiley-wink.gif)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <routing routingTableName="RTable"/>
          <serviceDebug includeExceptionDetailInFaults="true"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <client>
      <endpoint address="http://localhost:3445/UserService" binding="wsHttpBinding" contract="*" name="UserServiceEndpoint" />
      <endpoint address="http://localhost:3446/UserBackupService" binding="wsHttpBinding" contract="*" name="UserBackupServiceEndpoint"/>
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
            <add filterName="UpdateUserFilter" endpointName="UserServiceEndpoint" alternateEndpoints="alternateEndpointList"/>
          </entries>
        </table>
      </routingTables>
      <alternateEndpoints>
        <list name="alternateEndpointList">
          <endpoints>
            <add endpointName="UserBackupServiceEndpoint"/>
          </endpoints>
        </list>
      </alternateEndpoints>
    </routing>
  </system.serviceModel>
</configuration>
```

İlk etapta, Backup servisi içinde bir Endpoint bildirimi yapıldığı ve yedek servisin işaret edildiği farkedilmektedir. Diğer yandan routingTables içerisinde yapılan entries bildirimlerinden UpdateUserFilter isimli olanında alternateEndpoints isimli bir nitelik (attribute) dikkati çekmektedir. Bu nitelik, dosyanın ilerleyen kısımlarında yer alan alternateEndpoints elementi altındaki listeyi işaret etmektedir.

Bu liste içerisinde n sayıda alternatif endPoint ismi belirtilebilir. Bir başka deyişle bir endPoint'in karşılayamadığı istekleri, birden fazla endPoint noktasına denenmek üzere aktarabiliriz. Tabi bu durumu henüz test etme şansım olmadı. Ki beklenen sırasıyla servislerin denenmesi ve başarılı olandan sonrakilere geçilmemesi yönünde olmalıdır. Ancak entries/add elementi içerisinde priority isimli bir nitelikte bulunmakta ve öncelik seviyesini belirlemektedir. İşte size bir garajda araştırma ödevi.

![Cool](/assets/images/2009/smiley-cool.gif)

Artık vakamızı tekrardan test edebiliriz. Yine tüm servisleri (Backup servisimiz dahil) çalıştıracak, ancak istemci talepte bulunmadan önce UserService'ini kapatacağız. İşte sonuçlar;

![blg74_Result.gif](/assets/images/2009/blg74_Result.gif)

Görüldüğü gibi, UserService'in kapalı olması ve Exception üretmesi durumunda, Router servisimiz talebi bu kez alternatif endPoint listesinde belirtilen UserBackupService isimli yedek servise doğru yönlendirmiş ve istemcinin talebinin buradan karşılanmasını sağlamıştır. Tabiki burada ele alınan alternatif Endpoint'lerin işaret ettiği servisler farklı makinelereden, farklı bağlayıcı tiplerle (Binding Types), farklı iletişim protokolleri ile dağıtılabilir. Bu tamamen yedek servis stratejimize bağlıdır. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Router Project 2.rar (128,25 kb)](https://www.buraksenyurt.com/pics/2009%2f8%2fRouter+Project+2.rar)
