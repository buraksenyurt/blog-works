---
layout: post
title: "Workflow Services - Custom Authorization"
date: 2010-03-22 04:30:00 +0300
description: "Bu yazımızda .Net Framework 4.0 tarafında geliştireceğimiz Workflow Service' lerde yetkilendirme işlemini nasıl sağlayabileceğimizi görmeye çalışacağız."
categories:
  - wcf-4-0
  - wcf-4-0-rc
  - wf-4-0-rc
tags:
  - wcf-4-0
  - wcf-4-0-rc
  - wf-4-0-rc
  - xml
  - csharp
  - dotnet
  - aspnet
  - workflow-foundation
  - http
  - authentication
  - authorization
  - generics
  - visual-studio
  - rc
---
# Workflow Services - Custom Authorization
![blg145_Giris.jpg](/assets/images/2010/blg145_Giris.jpg)

Merhaba Arkadaşlar,

Aşçılık zevkli ama bir o kadarda zor bir zanaattır. Hatta bazen o kadar zor bir zanaat olur ki, aşcının aldığı maaşı pek çok yazılımcı iki yılda kazanamaz. Tabi bu tip aşçılar işin ehli olan kişilerdir. Heleki tek bir mutfak değilde dünya mutfağının seçkin olanlarına ait becerileri bulunanlara paha biçilemez. Türk Mutfağından Japon mutfağına, Meksika yemeklerinden İtalyan spesiyallerine, Fransız tatlılarından Okyanus deniz ürünlerine ve daha nicelerine...

Tabi bir aşçı için olmassa olmazlardan birisi de yemeği için gerekli olan malzemelerin kalitesidir. Kaliteli zeytinyağı, hamur ve baharat ile yapılan spagettinin, kalitesiz olanlar ile yapılanı arasında dağlar kadar fark olabilir. Spagetti demişken bu günkü yazımızda neler yapacağımıza da bir bakalım dilerseniz. Aşçı olarak bu gün elimizde zor bir tarif var. Malzemelerimiz belli ama pişecek olan yemeğin yapımı biraz zahmetli. Haydi gelin hiç vakit kaybetmeden önlüğümüzü takıp klavyenin başına geçelim.

![Wink](/assets/images/2010/smiley-wink.gif)

Bu yazımızda.Net Framework 4.0 tarafında geliştireceğimiz Workflow Service'lerde yetkilendirme işlemini nasıl sağlayabileceğimizi görmeye çalışacağız. Ne yazık ki Authorization işlemini kolaylaştırmak adına hazır bir yapı mevcut değil. Bu nedenle biraz kodlama yapmamız ve çalışma zamanının işleyişine bu şekilde müdahale etmemiz gerekiyor. Hatta yapacağımız özelleştirme öylesine etkili olacak ki, aradan Doğrulamayı (Authentication) bile çıkaracağız farkına varmadan.

![Surprised](/assets/images/2010/smiley-surprised.gif)

Ama önce yemek için gerekli malzemelerimizin neler olduğuna bir bakalım.

- Bir adet Workflow Service.
- Bir adet konfigurasyon dosyası (Web.config).
- System.IdentityModel.dll referansı.
- ServiceAuthorizationManager türevli bir sınıf.
- Windows üzerinde tanımlanmış roller ve bu roller içerisinde yer alan kullanıcılar.

Tabiki malzemeleri tedarik etmek yeterli değil. Birde tarifi bilmek lazım

![Wink](/assets/images/2010/smiley-wink.gif)

Öncelikli olarak yetkilendirme işlemini üstelenen bir sınıf yazmamız gerekiyor. Çok doğal olarak bu sınıfın Workflow Service çalışma zamanı tarafından değerlendirilebilmesi için konfigurasyon dosyası üzerinde de gerekli düzenlemeleri yapmalıyız. Sonrasında ise işi istemci tarafından gelen taleplere bırakıyor olacağız. İşe ilk olarak aşağıdaki gibi bir Workflow Service projemiz olduğunu varsayarak başlayalım.

![blg145_Wf.gif](/assets/images/2010/blg145_Wf.gif)

Calculus.xamlx içeriğimiz ise aşağıdaki gibidir;(Sadece Sequence elementi içeriği verilmiştir)

```xml
<p:Sequence DisplayName="Sequential Service" sad:XamlDebuggerXmlReader.FileName="D:\Projects\Workflow Foundation 4.0\UsingCustomAuthorization\UsingCustomAuthorization\CalculusService.xamlx" sap:VirtualizedContainerService.HintSize="277,336">
    <p:Sequence.Variables>
      <p:Variable x:TypeArguments="CorrelationHandle" Name="handle" />
      <p:Variable x:TypeArguments="x:Int32" Name="X" />
      <p:Variable x:TypeArguments="x:Int32" Name="Y" />
      <p:Variable x:TypeArguments="x:Int32" Name="Sum" />
    </p:Sequence.Variables>
    <sap:WorkflowViewStateService.ViewState>
      <scg3:Dictionary x:TypeArguments="x:String, x:Object">
        <x:Boolean x:Key="IsExpanded">True</x:Boolean>
      </scg3:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <Receive x:Name="__ReferenceID0" CanCreateInstance="True" DisplayName="ReceiveRequest" sap:VirtualizedContainerService.HintSize="255,86" OperationName="SumOp" ServiceContractName="p1:IService">
      <Receive.CorrelatesOn>
        <MessageQuerySet />
      </Receive.CorrelatesOn>
      <Receive.CorrelationInitializers>
        <RequestReplyCorrelationInitializer CorrelationHandle="[handle]" />
      </Receive.CorrelationInitializers>
      <ReceiveParametersContent>
        <p:OutArgument x:TypeArguments="x:Int32" x:Key="XValue">[X]</p:OutArgument>
        <p:OutArgument x:TypeArguments="x:Int32" x:Key="YValue">[Y]</p:OutArgument>
      </ReceiveParametersContent>
    </Receive>
    <SendReply Request="{x:Reference __ReferenceID0}" DisplayName="SendResponse" sap:VirtualizedContainerService.HintSize="255,86">
      <SendParametersContent>
        <p:InArgument x:TypeArguments="x:Int32" x:Key="SumResult">[X + Y]</p:InArgument>
      </SendParametersContent>
    </SendReply>
  </p:Sequence>
```

Aslında Calculus isimli Workflow Service içerisinde yer alan SumOp isimli operasyonun görevi çok basit ve bellidir. Int32 tipinden iki sayısal değerin toplanması ve sonucunun istemci tarafına geri döndürülmesi. Bizim hedefimiz sadece yetkisi olan kişilerin bu operasyonu çalıştırmasıdır. Bu durumda güvenlik ile ilişkili olarak yetki kontrolünün özel bir sınıf tarafından yapılması gerekmektedir. Söz konusu sınıf System.ServiceModel isim alanı (Namespace) altında yer alan ServiceAuthorizationManager tipinden türetilmelidir. İçeriğini ise çok sade olarak aşağıdaki gibi tasarlayabiliriz.

```csharp
using System.Collections.Generic;
using System.Security.Principal;
using System.ServiceModel;

namespace UsingCustomAuthorization
{
    public class Authorizer
        : ServiceAuthorizationManager 
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            var authCtx = operationContext.ServiceSecurityContext.AuthorizationContext;
            var identities=(List<IIdentity>)authCtx.Properties["Identities"];

            foreach (var identity in identities)
            {
                var winIdentity = identity as WindowsIdentity;
                if (winIdentity != null)
                {
                    var winPrincipal = new WindowsPrincipal(winIdentity);
                    return winPrincipal.IsInRole("Administrators");
                }
            }

            return false;
        }
    }
}
```

Authorizer sınıfı içerisinde CheckAccessCore metodu ezilmiş ve parametre olarak gelen operationContext değişkeninden yararlanarak gerekli yetki kontrolü yapılmıştır. Buna göre Workflow Service'ten talepte bulunan bir Windows kullanıcısı, servisin host edildiği makinede tanımlı ise, Administrators rolünde olup olmadığı kontrol edilmekte ve buna göre geriye true veya false değeri döndürülmektedir. Tahmin edileceği üzere CheckAccessCore metodunun geriye false değer döndürmesi güvenlik hatasına yol açacaktır. Burada unutulmaması gereken bir noktayı da hatırlatmak yarar var. Örneğimizde konuyu son derece basit bir şekilde ele almak istediğimizden, doğrudan Administrator rolünün kontrolünü yapıp işin içinden sıyrılmaktayız. Oysaki yetki kontrolü için harici bir listeden yararlanılabilir. Bu liste web.config dosyasında appSettings kısmında tutulabileceği gibi bir Text dosya içerisinde veya veritabanı üzerindeki bir tabloda konuşlandırılabilir. Yinede varmak istediğimiz noktayı anladığınızı düşünerek devam ediyorum.

Sırada çalışma zamanı için Authorizer sınıfının yetki kontrolü amacıyla kullanılacağını bildirmemiz gerekiyor. Bunun için web.config dosyasını aşağıdaki şekilde düzenlememiz yeterli olacaktır.(Bu arada projemize System.IdentityModel.dll assembly'ını referans etmeyi unutmamalıyız. Aksi takdirde authCtx üzerinden hiç bir özelliğe erişemeyiz)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.web>
    <compilation debug="true" targetFramework="4.0" />
  </system.web>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceAuthorization serviceAuthorizationManagerType="UsingCustomAuthorization.Authorizer, UsingCustomAuthorization"/>
          <serviceMetadata httpGetEnabled="true"/>
          <serviceDebug includeExceptionDetailInFaults="false"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <protocolMapping>
      <add scheme="http" binding="wsHttpBinding"/>
    </protocolMapping>
  </system.serviceModel>
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true"/>
  </system.webServer>
</configuration>
```

Dikkat edileceği üzere serviceAuthorization elementi ile yetkilendirme davranışını ele alacak Authorizer tipi belirlenmiştir. Ayrıca iletişimin güvenli olmasını sağlamak adına protocolMapping sekmesinde wsHttpBinding bağlayıcı tipinin kullanılacağı bildirilmiştir. İşte bu kadar.

![Wink](/assets/images/2010/smiley-wink.gif)

Artık yemeğimizi orta ateşte 40 dakika kadar pişirip servis edebiliriz. Tabi servis etmeden önce tadına bakmak gerekmektedir. Nasıl mı?

Öncelikli olarak Administrator rolünde olan ve olmayan iki test kullanıcımız olduğunu düşünelim. Ben, örneği geliştirmekte olduğum makinede bu amaçla bsenyurt ve runi isimli iki kullanıcı oluşturdum. Bu kullanıcılardan bsenyurt Administrator rolünde iken runi User rolü içerisinde yer almakta. Dolayısıyla test sonuçlarımıza göre runi isimli kullanıcı talebi karşılığında Access Denied hata mesajını almalı. Bakalım gerçektende böylemi oldu?

WcfTestClient uygulamamızı Run As.. komutu yardımıyla önce bsenyurt kullanıcısı ile çalıştıralım. Eğer Asp.Net Developement Server'ı Visual Studio ortamına Attach'larsak CheckAccessCore metodu içeriğini de debug edebiliriz. Buna göre debug modunda aşağıdaki sonuçlar ile karşılaşırız.

![blg145_Debug1.gif](/assets/images/2010/blg145_Debug1.gif)

Görüldüğü üzere bsenyurt kullanıcısı doğrulanmıştır. IsAuthenticated değerinin true olduğuna dikkat edelim. Şimdi WcfTestClient uygulamasının sonuç ekranına bakarsak toplama işleminin başarılı bir şekilde gerçekleştirildiğini görebiliriz.

![blg145_AccessGaranted.gif](/assets/images/2010/blg145_AccessGaranted.gif)

Şimdi de WcfTestClient aracını Runi isimli kullanıcı ile çalıştıralım. Debug modda aşağıdaki sonuçlar ile karşılaşırız.

![blg145_Debug2.gif](/assets/images/2010/blg145_Debug2.gif)

Tahmin edileceği üzere Runi isimli kullanıcı da doğrulanmıştır. Nitekim servisin çalıştırıldığı makine de tanımlı bir Windows kullanıcısıdır. Ancak WcfTestClient uygulaması üzerinden bir toplama işlemi talebinde bulunulduğunda hata mesajı ile karşılaşılacaktır.

![blg145_AccessDenied.gif](/assets/images/2010/blg145_AccessDenied.gif)

Volaaaaa!!! Bu çok doğaldır. Nitekim Runi isimli kullanıcı Administrators grubuna dahil değildir ve bu yüzden yetki kontrolünden geçememiştir.

Yapmış olduğumuz bu çalışmaya göre bir Workflow Service'ini host ettiğimiz sunucu üzerindeki Windows kullanıcılarından ve dahil oldukları grup bilgilerinden yararlanarak az bir kodlama ile doğrulama ve yetkilendirme işlemlerini gerçekleştirebiliriz. Afiyet olsun

![Wink](/assets/images/2010/smiley-wink.gif)

[UsingCustomAuthorization_RC.rar (17,67 kb)](/assets/files/2010/UsingCustomAuthorization_RC.rar) [Örnek Visual Studio 2010 Ultimate RC sürümü üzerinde geliştirilmiş ve test edilmiştir]