---
layout: post
title: "WCF Uygulamalarında Enterprise Library Validation Block Kullanımı"
date: 2013-06-23 15:26:00 +0300
description: "Biz bu makalemizde WCF(Windows Communication Foundation) servislerinde, Validation Application Block’ u nasıl kullanabileceğimizi incelemeye çalışıyor olacağız. Bu block yardımıyla nitelik bazlı(Attribute Based) olacak şekilde doğrulama(Validation) kontrolleri yapılabilmektedir. Söz konusu doğrulama kontrolleri sınıfların özelliklerine uygulanan nitelikler ile yapılabileceği gibi, metodların parametreleri üzerine de enjekte olabilmektedir. Dilerseniz adım adım senaryomuzu geliştirip konuyu basit seviye de kavramaya çalışalım."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - http
  - authentication
  - caching
  - serialization
---
# WCF Uygulamalarında Enterprise Library Validation Block Kullanımı
[![lego-block-tape](/assets/images/2013/lego-block-tape_thumb.jpg)](/assets/images/2013/lego-block-tape.jpg)[Orjinal Yazım Tarihi 10.09.2012]

Merhaba Arkadaşlar,

Enterprise Library ve içerisinde yer alan Application Block’ lar çoğunlukla projelerimizde ihtiyaç duyduğumuz ve Cross-Cutting olarak geçen parçaların hızlı ve kolay bir biçimde uygulanmasında kullanılmaktadır.

Cross-Cutting’ ler özellikle birden fazla katmandan oluşan proje bazlı çözümlerde, katmanların pek çok noktasında sıklıkla kullanılabilen (ihtiyaç duyulabilen) fonksiyonelliklerdir.

Örneğin Exception Handling, Security, Cryptography, Configuration, Logging, Validation, Caching vb…Bu tip modüler yapılar çok sık kullanıldıklarından her çözüm için ayrı ayrı geliştirilmemektedir/geliştirilmemelidir. Bunun yerine yeniden kullanılabilen modüler yapılar olarak ele alınmaları daha doğru bir yaklaşımdır. Örneğin Enterprise Library

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_139.png)

Biz bu makalemizde WCF (Windows Communication Foundation) servislerinde, Validation Application Block’ u nasıl kullanabileceğimizi incelemeye çalışıyor olacağız. Bu block yardımıyla nitelik bazlı (Attribute Based) olacak şekilde doğrulama (Validation) kontrolleri yapılabilmektedir. Söz konusu doğrulama kontrolleri sınıfların özelliklerine uygulanan nitelikler ile yapılabileceği gibi, metodların parametreleri üzerine de enjekte olabilmektedir. Dilerseniz adım adım senaryomuzu geliştirip konuyu basit seviye de kavramaya çalışalım.

Örnek senaryomuzda WCF tabanlı bir servis üzerinden bir metod çağrısı ile Player isimli bir nesne örneğinin oluşturulmasını sağlıyor olacağız. Player tipinin özelliklerine ait değerler, servis operasyonuna parametre olarak gelecekler. İşte doğrulama kriterlerimiz de bu notkada devreye girecek ve bazı veri giriş ihlallerini kontrol edecekler. Haydi başlayalım

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_139.png)

İlk olarak servis tarafını geliştireceğiz. WCF Service Application şablonundan üretilen uygulamamızda, aşağıdaki ekran görüntüsünde yer alan referansların bulundurulması gerekmektedir.

[![wcfvbe_1](/assets/images/2013/wcfvbe_1_thumb.png)](/assets/images/2013/wcfvbe_1.png)

Enterprise Library’ sisteme yüklendikten sonra kurulduğu yerdeki bin klasöründen

- Microsoft.Practices.EnterpriseLibrary.Common
- Microsoft.Practices.EnterpriseLibrary.Validation
- Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF
- Microsoft.Practices.ServiceLocation

assembly’ larının yüklenmesi gerekmektedir. Bunlara ek olarak System.ComponentModel.DataAnnotations.dll assembly’ ının da ayrıca projeye ilave edilmesi gerekecektir.

İlgili Assembly’ ların referans edilmesinin ardından servis tarafındaki uygulamanın geliştirilmesine başlanabilir. Örnek senaryomuza göre doğrulama denetimi, servis operasyonundaki metod parametreleri seviyesinde yapılacaktır. Şimdi aşağıdaki sınıf diagramında görülen tipleri geliştirmeye başlayalım.

[![wcfvbe_8](/assets/images/2013/wcfvbe_8_thumb.png)](/assets/images/2013/wcfvbe_8.png)

Player tipinin içeriği aşağıdaki gibidir.

```csharp
using System.Runtime.Serialization;

namespace MyCompanyServiceApp 
{ 
    [DataContract] 
    public class Player 
    { 
        [DataMember]        
        public int PlayerId { get; set; } 
        
        [DataMember]                
        public string Nickname { get; set; }

        [DataMember] 
        public string Password { get; set; }

        [DataMember]        
        public string Country { get; set; } 
        
        [DataMember] 
        public string EMail{ get; set; }

        [DataMember]        
        public int Score{ get; set; } 
    } 
}
```

Servis sözleşmesini ise aşağıdaki gibi tasarlayacağız.

```csharp
using Microsoft.Practices.EnterpriseLibrary.Validation; 
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF; 
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators; 
using System.ServiceModel;

namespace MyCompanyServiceApp 
{ 
    [ServiceContract] 
    public interface IPlayerService 
    { 
        [OperationContract] 
        [FaultContract(typeof(ValidationFault))] 
        Player AddPlayer(

            [ValidatorComposition(CompositionType.And)] 
            [StringLengthValidator(5, 10, MessageTemplate = "Nickname en az 5 en fazla 10 karakter olabilir")]            
            [NotNullValidator()] 
            string nickname,

            string password, 
            
            [ValidatorComposition(CompositionType.And)] 
            [StringLengthValidator(3, 30, MessageTemplate = "Ülke bilgisi en az 3 en fazla 30 karakter olabilir")] 
            [NotNullValidator()] 
            string country,

            [RegexValidator(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$")] 
            string email,

            [RangeValidator(0, RangeBoundaryType.Inclusive, 1000, RangeBoundaryType.Inclusive, MessageTemplate = "Score bilgisi 0 ile 1000 arasında olabilir")]                
            int score,

            [StringLengthValidator( 
                10 
                , 100 
                , MessageTemplate = "Notlar en az 10 en fazla 100 karakter uzunluğunda olabilir" 
                , ErrorMessageResourceName="Notes" 
                ,ErrorMessageResourceType=typeof(string) 
                )] 
            string notes 
            ); 
    } 
}
```

Dikkat edileceği üzere metod parametrelerinde bazı nitelikler (Attribute) kullanılmıştır. Validator kelimesi ile biten bu nitelik sınıfları yardımıyla bazı doğrulama kriteleri set edilmiştir. Örneğin nickname bilgisinin string uzunluğu 5 ile 10 karakter arasında olmalıdır. Benzer durum notes ve country parametreleri için de geçerlidir. NotNullValidator, tahmin edileceği üzere uygulandığı parametreinin null olmamasını gerektirmektedir. Özellikle referans tiplerinin (örneğin string) null geçilmemesi bir doğrulama kriteri olarak sunulabilir.

Kullanımı etkin olan doğrulama tiplerinden birisi de RegexValidator’ dur. String tipinden parametrelere uygulanmakta olup, verinin bir RegEx desenine göre doğruluğunun kontrol edilmesini sağlamaktadır. Örnekte email verisinin geçerli bir elektronik posta adresi olup olmadığının kontrolü yapılmaktadır.

Birden fazla doğrulama kriteri uygulanmak istediğinde ise bu and’ li veya or’ lu bir biçimde enjekte edilebilir. Bunun için ValidatorComposition niteliğinin kullanılması yeterlidir. Örneklerimizde And opsiyonu etkinleştirilmiştir. Yani takip eden tüm Validator’ ların aynı anda true olması halinde bir ihlal söz konusu olmayacaktır.

AddPlayer isimli servis operasyonuna uygulanan bir diğer nitelikte FaultContract’ tır. Nitelik, Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF isim alanı (namespace) altında yer alan ValidationFault tipini kullanmaktadır. Dolayısıyla bir hata sözleşmesi olarak WCF’ in Validation Application Block için üretilmiş olan ValidationFault tipi kullanılacak ve istemci tarafında gönderilecektir.

Servis sözleşmesinin uygulandığı sınıf kodları ise aşağıdaki gibidir.

```csharp
using System;

namespace MyCompanyServiceApp 
{ 
    public class PlayerService 
        : IPlayerService 
    { 
        public Player AddPlayer( 
            string nickname, 
            string country, 
            string password, 
            string email, 
            int score,            
            string notes) 
        { 
            Random rnd = new Random(); 
            return new Player 
            { 
                PlayerId=rnd.Next(1,100), 
                Nickname=nickname, 
                Password=password, 
                EMail = email, 
                Score=score, 
                Country=country 
            }; 
        } 
    } 
}
```

PlayerService sınıfı içerisinde yer alan AddPlayer metodu içerisinde çok olağanüstü bir çalışma yoktur. Sadecebir Player nesnesi örneklenmekte ve istemci tarafına geri gönderilmektedir. Tabi herhangibir doğrulama kriterine takılınmadıysa.

Buraya kadar yapılan hazırlıklar ne yazık ki yeterli değildir. WCF çalışma zamanının da söz konusu Validation Application Block ile kullanılacağının bir şekilde belirtilmesi gerekir. Bu aslında servis endpoint’ i için ilave bir davranış (Behavior) belirtilmesinden başka bir şey değildir. Bunun için WCF Service Configuration Editor’ ü kullanabiliriz. Şimdi bu adımlarımızı sırasıyla gerçekleştirelim.

Adım 1

[![wcfvbe_2](/assets/images/2013/wcfvbe_2_thumb.png)](/assets/images/2013/wcfvbe_2.png)

İlk olarak Advanced->Extensions->behavior element extensions kısmına gidilir ve buradan New düğmesine basıldıktan sonra çıkan iletişim pencersine geçilir. Name özelliğine bir değer verdikten sonra ise Type özelliğinin karşısında bulunan 3 nokta düğmesine basılır.

Adım 2

[![wcfvbe_3](/assets/images/2013/wcfvbe_3_thumb.png)](/assets/images/2013/wcfvbe_3.png)

Üç nokta düğmesine basıldıktan sonra ise, projenin bin klasörüne eklenmiş olan Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF.dll assembly’ ı seçilir.

Adım 3

[![wcfvbe_4](/assets/images/2013/wcfvbe_4_thumb.png)](/assets/images/2013/wcfvbe_4.png)

Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF.dll assembly’ nın seçilmesini takiben gelen ekrandaki tek tip olan ValidationElement işaretleniz. Bu durumda aşağıdaki ekran görüntüsünde yer aldığı gibi ilgili elementin, behavior element extensions kısmına eklenmiş olduğu görülür.

[![wcfvbe_5](/assets/images/2013/wcfvbe_5_thumb.png)](/assets/images/2013/wcfvbe_5.png)

Adım 4

[![wcfvbe_6](/assets/images/2013/wcfvbe_6_thumb.png)](/assets/images/2013/wcfvbe_6.png)

Sıradaki adımda ilgili ValidationExtension elementinin bir EndPoint davranışı haline getirilmesi yer almaktadır. Bunun için Advanced->Endpoint Behaviors kısmına yeni bir davranış ilave edilmelidir. Davranışa örnek bir isim verildikten sonra ise Add düğmesi yardımıyla biraz önce sisteme dahil edilmiş olan ValidationExtension tipinin eklenmesi sağlanır.

Adım 5

Son olarak yeni Endpoint davranışının, servise ait Endpoint ile ilişkilendirilmesi yeterli olacaktır. Bunun için Services->[Service Adı]->Endpoints->[Endpoint Adı] kısmından gelen özelliklerden BehaviorConfiguration’ a ValidationBehavior değerinin atanması yeterlidir.

[![wcfvbe_7](/assets/images/2013/wcfvbe_7_thumb.png)](/assets/images/2013/wcfvbe_7.png)

Sonuç olarak servis tarafına ait konfigurasyon dosyası içeriği aşağıdaki gibi olacaktır.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<configuration> 
    <system.serviceModel> 
        <extensions> 
            <behaviorExtensions> 
                <add name="ValidationExtension" type="Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF.ValidationElement, Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF, Version=5.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" /> 
            </behaviorExtensions> 
        </extensions> 
        <services> 
            <service behaviorConfiguration="PlayerServiceBehavior" name="MyCompanyServiceApp.PlayerService"> 
                <endpoint address="http://localhost:50511/PlayerService.svc" 
                    behaviorConfiguration="ValidationExtensionBehavior" binding="basicHttpBinding" 
                    name="PlayerServiceHttpEndpoint" contract="MyCompanyServiceApp.IPlayerService" /> 
            </service> 
        </services> 
        <behaviors> 
            <endpointBehaviors> 
               <behavior name="ValidationExtensionBehavior"> 
                    <ValidationExtension /> 
                </behavior> 
            </endpointBehaviors> 
            <serviceBehaviors> 
                <behavior name="PlayerServiceBehavior"> 
                    <serviceMetadata httpGetEnabled="true" /> 
                    <serviceDebug includeExceptionDetailInFaults="false" /> 
                </behavior> 
            </serviceBehaviors> 
        </behaviors> 
    </system.serviceModel> 
</configuration>
```

Şimdi örnek bir istemci uygulama geliştirerek senaryomuzu teste çıkabiliriz. Basit bir Console uygulaması pekala işimizi görecektir

![Sarcastic smile](/assets/images/2013/wlEmoticon-sarcasticsmile_9.png)

Console uygulamasına servis referansını ekledikten sonra, aşağıdaki kodları geliştirdiğimizi düşünelim.

> İstemci uygulamada ValidationFault tipinin kullanılabilmesi için, Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF.dll assemby’ ının da projeye referans edilmesi gerekmektedir. Bu Assembly ne yazık ki Add Service Reference seçeneği sonrası otomatik olarak eklenmemektedir.

```csharp
using ClientApp.Company; 
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF; 
using System; 
using System.ServiceModel;

namespace ClientApp 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            PlayerServiceClient proxy = new PlayerServiceClient("PlayerServiceHttpEndpoint");

            try 
            { 
                Player newPlayer = proxy.AddPlayer( 
                    nickname: "kısa" 
                    , password: "şifre" 
                    , country: null 
                   , email: "deneme" 
                    , score: -10 
                    , notes: "kısa not" 
                   );

                //Player newPlayer = proxy.AddPlayer( 
                //        nickname: "burkicik" 
                //        , password: "şifre" 
                //        , country: "Birleşik Krallık" 
                //        , email: "selim@buraksenyurt.com" 
                //        , score: 128 
                //        , notes: "oyuna yeni katılmış bir oyuncudur ve ilk seferinde turnayı gözünde vurmuştur" 
                //        );

            } 
           catch (FaultException<ValidationFault> excp) 
            { 
                foreach (ValidationDetail validationDetail in excp.Detail.Details) 
                { 
                    Console.WriteLine("{0} : {1}\n", validationDetail.Tag, validationDetail.Message); 
                } 
            } 
            catch (Exception excp) 
            { 
                Console.WriteLine(excp.Message); 
            } 
            finally 
            { 
                if (proxy.State == CommunicationState.Opened) 
                    proxy.Close(); 
            } 
        } 
    } 
}
```

Dikkat edileceği üzere doğrulama kurallarına takılacak şekilde test verileri girilmeye çalışılmıştır. Oluşması muhtemel ne kadar doğrulama hatası varsa, bunların tamamı FaultException ile gelen exception nesne örneğinin Detail özelliğinin Details koleksiyonunda toplanacaktır. Dolayısıyla bu koleksiyon içeriği dolaşılarak ilgili doğrulama ihlallerine ait bir takım bilgilere ulaşılabilinir.

Uygulamanın çalışma zamanı çıktısı aşağıdaki ekran görüntüsündeki gibi olacaktır.

[![wcfvbe_9](/assets/images/2013/wcfvbe_9_thumb.png)](/assets/images/2013/wcfvbe_9.png)

nickname, country, email, score ve notes metod parametrelerinde tanımlanan doğrulama ihlalleri istemci tarafına bu şekilde yansımıştır. Tabi yorum satırları altına alınmış Player’ ın oluşturulmasını denersek, bu durumda herhangibir doğrulama kriterine takılmadan ekleme işleminin yapılabildiğine şahit oluruz.

Görüldüğü gibi Enterprise Library Validation Application Block’ u kullanarak, WCF tarafında operasyon bazında doğrulama kriterleri uygulanabilmektedir. Burada işin önemli yanlarından birisi ise, nitelik bazlı yapılan bu tanımlamaların servis operasyonları üzerinde gerçekleştirilmesidir. Dolayısıyla bu nitelikler değiştirilse bile, istemci tarafı için yeniden Proxy tipinin üretilmesine gerek yoktur

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_139.png)

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WCFandValidationBlock.zip (1,92 mb)](/assets/files/2013/WCFandValidationBlock.zip)

[Orjinal Yazım Tarihi 10.09.2012]