---
layout: post
title: "Serileştirme İşlemlerinde Vekil(Surrogate) Kullanmak"
date: 2008-07-03 12:00:00 +0300
description: "Bir önceki makalemizde WCF(Windows Communication Foundation) mimarisinde veri sözleşmelerinin(Data Contracts) karşı taraflarda farklı versiyonlarının olması durumunda oluşan vakaları incelemeye çalışmıştık. Versiyonlama(Versioning) vakaları dışında serileştirmede önem arz eden konulardan biriside, servis tarafında yayınlanan veri sözleşmeleri içerisinde serileştirilemeyen(NotSerializable) tiplerin var olmasıdır."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - xml
  - http
  - serialization
  - reflection
  - visual-studio
---
# Serileştirme İşlemlerinde Vekil(Surrogate) Kullanmak
Değerli Okurlarım Merhabalar,

Bir önceki makalemizde WCF (Windows Communication Foundation) mimarisinde veri sözleşmelerinin (Data Contracts) karşı taraflarda farklı versiyonlarının olması durumunda oluşan vakaları incelemeye çalışmıştık. Versiyonlama (Versioning) vakaları dışında serileştirmede önem arz eden konulardan biriside, servis tarafında yayınlanan veri sözleşmeleri içerisinde serileştirilemeyen (NotSerializable) tiplerin var olmasıdır. Bu durum çoğunlukla, serieştirilebilen tipin kullandığı bazı dahili tiplerin farklı assembly dosyaları içerisinde yer aldığı vakalarda ortaya çıkmaktadır. Öyleki, diğer assembly içerisinde yer alan tipe (type) geliştirici tarafından müdahale edilemeyebilir ve bu sebepten serileştirilebilmesi için DataContract veya DataMember üyeleri uygulanamayabilir. Bu durum aşağıdaki şekilde örnek bir senaryo üzerinden ifade edilmeye çalışılmaktadır.

![mk256_1.gif](/assets/images/2008/mk256_1.gif)

Dikkat edileceği üzere Urun sınıfı bir veri sözleşmesi (Data Contract) olacak şekilde, ServiceLibrary.dll assembly dosyası içerisinde yer almaktadır. Söz konusu sınıfın üyelerinden Ureten isimli özellik (Property) Uretici sınıfı tipindendir. Ne varki Uretici sınıfı, Ureticiler.dll assembly dosyası içerisinde yer almaktadır ve serileştirilebilir bir tip değildir (NotSerializable). Bu sebepten dolayı Urun sınıfının kullanıldığı bir senaryoda istemci açısından sorunlar oluşacaktır. Örneğin tipin istemci için gerekli proxy üretimi sırasında metadata içerisine gömülmesi mümkün olamayacaktır. Çözüm olarak servis tarafında serileştirme (Serializing) ve ters-serileştirme (DeSerializing), şema çıkartma (Schema Exporting) veya dahil etme (Schema Importing) işlemleri sırasında müdahalede bulunmak gerekmektedir. Ama nasıl?

> Serileştirilemeyen tipler servis tarafından istemcilere Metadata bilgileri içerisinde gönderilemezler. Bu sebepten dolayı istemci için önem arz eden proxy nesneleri içerisine konulamazlar ki buda söz konusu tiplerin istemci tarafından ele alınıp kullanılamayacağı anlamına gelir.

Bu noktada bir vekil (Surrogate) sınıf kullanılma yolu tercih edilir. Vekil sınıf serileştirilebilir olmakla birlikte, serileştirilemeyen sınıfın üyelerini taşıyacak şekilde tasarlanır. Daha sonra serileşen ve serileşemeyen sınıflar arasında köprü görevi üstelenecek ek bir sınıf daha tasarlanır. Bu ek sınıfın görevi serileştirme, ters-serileştirme, şema import ve export işlemleri sırasında, serileştirilemeyen sınıf ile eşleştirme yapılmasını sağlamaktır.

Bu sınıf, IDataContractSurrogate arayüzünü (Interface) uygulayacak şekilde tanımlanır. Buna ek olarak WCF çalışma ortamına, serileştirme, ters serileştirme, şema yayınlamak gibi işlemler sırasında devreye girecek olan IDataContractSurrogate uyarlamalı tipin, davranış (behavior) olarak atanması gerekmektedir ki bu işlemlerde çoğunlukla nitelik (attribute) olarak kullanılabilecek bir sınıf içerisinde ele alınır.:) Bu karmaşık sürecin adım adım incelenmesinden önce, yukarıdaki senaryodaki gibi bir durum oluşması halinde çalışma zamanında neler olacağını irdelemekle başlamakta yarar vardır. Bu amaçla ilk olarak Uretciler.dll isimli assembly, Uretici sınıfını içerecek şekilde aşağıdaki gibi tasarlanır.

![mk256_2.gif](/assets/images/2008/mk256_2.gif)

```csharp
using System;

namespace Ureticiler
{
    public class Uretici
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Adres { get; set; }
        public Uretici(int id,string ad,string adres)
        {
            Id = id;
            Ad = ad;
            Adres = adres;
        }
    }
}
```

Uretici sınıfı içerisinde yer alan aşırı yüklenmiş yapıcı metod (Overloaded Constructor) nedeni ile, varsayılan yapıcı metod (Default Constructor) geçersiz kalmaktadır. Buda tipin serileştirilmesini engelleyecek ve istenilen sorunun oluşmasına neden olacaktır. Servis sözleşmesi (Service Contract), uygulayıcı sınıf ve veri sözleşmesini (Data Contract) içeren WCF Servis kütüphanesi (WCF Service Library) içeriği ise aşağıdaki gibidir. (Servis kütüphanesinin, Ureticiler.dll assembly dosyasını referans etmesi gerektiği unutulmamalıdır.)

![mk256_3.gif](/assets/images/2008/mk256_3.gif)

Urun sınıfı içeriği;

```csharp
using System;
using System.Runtime.Serialization;

namespace UrunYonetim
{
    [DataContract(Name="Urun")]
    public class Urun
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Ad { get; set; }
        [DataMember]
        public double BirimFiyat { get; set; }
        [DataMember]
        public int StokMiktari { get; set; }
        [DataMember]
        public Ureticiler.Uretici Ureten { get; set; }
    }
}
```

Urun sınıfı içerisinde yer alan Ureten özelliğinin (Property), Uretici tipinden olduğuna ve serileştirilemediğine dikkat edilmelidir.

IUrunYonetici arayüzü içeriği;

```csharp
using System;
using System.ServiceModel;

namespace UrunYonetim
{
    [ServiceContract(Name="UrunServisi",Namespace="http://www.bsenyurt.com/UrunServisi")]
    public interface IUrunYonetici
    {
        [OperationContract]
        void UrunEkle(Urun urun);

        [OperationContract]
        Urun UrunGuncelle(Urun urun);
    }
}
```

IUrunYonetici arayüzü içerisinde iki adet operasyon tanımlanmıştır. UrunEkle metodu, parametre olarak Urun tipinden bir nesne örneği alır. UrunGuncelle metodu ise parametre olarak alınan Urun üzerinden bir takım güncellemeler yapılmasını ve geriye döndürülmesini sağlamak üzere tanımlanmıştır. Dikkat edileceği üzere her iki operasyonda Urun isimli veri sözleşmesini kullanmaktadır.

UrunYonetici sınıfı içeriği;

```csharp
using System;

namespace UrunYonetim
{
    public class UrunYonetici
        :IUrunYonetici
    {
        #region IUrunYonetici Members

        public void UrunEkle(Urun urun)
        {
            String bilgi = String.Format("{0} numaralı {1} ürün eklenmiştir", urun.Id, urun.Ad);
            Console.WriteLine(bilgi);
        }

        public Urun UrunGuncelle(Urun urun)
        {
            urun.Ureten = new Ureticiler.Uretici(1, "Adventure Vendor", "Adventure yolu üzeri");
            urun.StokMiktari += 10;
            urun.BirimFiyat += 1.1;
            return urun;
        }

        #endregion
    }
}
```

Servis tarafındaki uygulama yine olayların basit bir şekile anlaşılabilmesi için Console projesi şeklinde tasarlanmaktadır. Servis uygulamasına ait konfigurasyon dosyası ve kod içeriği başlangıçta aşağıdaki gibidir.(Servis uygulamasının System.ServiceModel.dll ile UrunYonetim.dll assembly dosyalarını referans etmesi gerektiği unutulmamalıdır.)

Konfigurasyon (App.config) içeriği;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <system.serviceModel>
        <behaviors>
            <serviceBehaviors>
                <behavior name="UrunServisiBehavior">
                    <serviceMetadata />
                    <serviceDebug includeExceptionDetailInFaults="true"/>
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <services>
            <service behaviorConfiguration="UrunServisiBehavior" name="UrunYonetim.UrunYonetici">
                <endpoint address="" binding="netTcpBinding" bindingConfiguration="" name="UrunServisiTcpEndPoint" contract="UrunYonetim.IUrunYonetici" />
                <endpoint address="Mex" binding="mexTcpBinding" bindingConfiguration="" name="UrunServisiMexEndPoint" contract="IMetadataExchange" />
                <host>
                    <baseAddresses>
                        <add baseAddress="net.tcp://localhost:2501/UrunServisi" />
                    </baseAddresses>
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>
```

Örnekte TCP bazlı bir EndPoint kullanılmaktadır. Bununla birlikte istemci için gerekli olan Proxy sınıfının kolay üretilebilmesi amacıyla yine TCP bazlı bir MexEndPoint kullanılmaktadır.

Servis tarafı Program içeriği;

```csharp
using System;
using System.ServiceModel;
using UrunYonetim;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(UrunYonetici));
            host.Open();
            Console.WriteLine(host.State);
            Console.WriteLine("Kapatmak için bir tuşa basın");
            Console.ReadLine();
            host.Close();
        }
    }
}
```

İstemci uygulama için gerekli olan proxy sınıfı ve konfigurasyon dosyası içeriği MEX EndPoint kullanımı nedeni ile svcutil aracı veya Visual Studio->Add Service Reference yardımıyla çekilebilir. Tabi bu işlemler sırasında servis uygulamasının çalışıyor olmasına dikkat edilmelidir. Ne varki, söz gelimi svcutil aracı ile ilgili servis üzerinden metadata bilgisi çekilmek istendiğinde aşağıdaki ekran görüntüsü ile karşılaşılır.

![mk256_4.gif](/assets/images/2008/mk256_4.gif)

Her ne kadar hata mesajı çok açık olmasada ve tutarlı bir bilgi vermesede sorun, Uretici sınıfının serileştirilemiyor olmasından kaynaklanmaktadır. Bu sebepten istemci tarafına aktarılacak olan Metadata bilgisi kesilmiştir. Nitekim serileştirilemeyen bir tipin, istemci tarafına export edilecek olan şema (Schema) içeriğine dahil edilmesi mümkün değildir. İşte sorun burada başlamaktadır. Çözüm ise ilerleyen adımlarda ele alınacaktır. Bölümün başındada belirtildiği gibi ilk olarak, serileştirilemeyen tipin yerine geçecek bir vekil sınıf yazılmalıdır ki bu çoğunlukla Surrogate Data Contract olarak anılır.

![mk256_5.gif](/assets/images/2008/mk256_5.gif)

Yukarıdaki şekle göre Uretici sınıfı için UreticiSurrogated isimli bir vekil tip tayin edilmiştir. Bu sınıfın kendisi bir veri sözleşmesi olacak şekilde tanımlanmıştır. Burada en önemli nokta vekil tipin serileştirilebilir olmasıdır. UreticiSurrogated sınıfı servis tarafında aşağıdaki gibi tasarlanabilir.

![mk256_6.gif](/assets/images/2008/mk256_6.gif)

```csharp
using System;
using System.Runtime.Serialization;

namespace UrunYonetim
{
    [DataContract(Name="Vendor")] // Bilinçli olaraktan Vendor adı verilmiştir.
    public class UreticiSurrogated
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Ad { get; set; }
        [DataMember]
        public string Adres { get; set; }
    }
}
```

Bu sınıf sadece ve sadece serileştirilemeyen Uretici sınıfı yerine kullanılacak vekil tiptir. Şimdi serileştirme, ters serileştirme, şema yayınlama gibi işlemler sırasında devreye girecek olan bir sınıfın daha tasarlanması gerekmektedir. Bu sınıfın en önemli özelliği ise IDataContractSurrogate arayüzünü (Interface) uyguluyor olmasıdır. Bu arayüze ait metodlar yukarıda bahsedilen işlemler sırasında devreye girmektedir. Söz konusu WCF servis kütüphanesi içerisinde aşağıdaki gibi tasarlanabilir.(IDataContractSurrogate arayüzünün (Interface) kullanılabilmesi için System.Runtime.Serialization.dll assembly dosyasının projeye referans edilmesi gerekmektedir.)

![mk256_7.gif](/assets/images/2008/mk256_7.gif)

```csharp
using System;
using System.Runtime.Serialization;
using Ureticiler;

namespace UrunYonetim
{
    public class UreticiSurrogater
        :IDataContractSurrogate
    {
        #region IDataContractSurrogate Members

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
             return null;
        }

        public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
             return null;
        }

        // Bu metod serileştirme(Serialization), ters-serileştirme(DeSerialization), schema import ve export işlemleri sırasında devreye girer.
        // Serileşemeyen tip ile Surrogate tip arasındakai eşleştirmeyi yapar.
        // type isimli metod parametresi serileştirilmiş, ters serileştirilmiş, şeması import veya export edilmiş tipi işaret etmektedir.
        public Type GetDataContractType(Type type)
        {
            if (typeof(Uretici).IsAssignableFrom(type))
                return typeof(UreticiSurrogated);
            return type;
        }

        // Bu metod surrogate tip örneğinin orjinal tip örneğine dönüştürülmesi sırasında kullanılır.
        // Ters-serileştirme(Deserialization) işlemi sırasında çalışır.
        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (obj is UreticiSurrogated)
            {
                UreticiSurrogated surrogated = (UreticiSurrogated)obj;
                Uretici uretici = new Uretici(surrogated.Id,surrogated.Ad,surrogated.Adres);
                return uretici;
            }
            return obj;
        }

        public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
        {
        }

        // Orjinal tip örneğini, surrogate tip örneğine dönüştürmek için kullanılır.
        // Serileştirme işlemi için bu metod gereklidir.
        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (obj is Uretici)
            {
                Uretici uretici = (Uretici)obj;
                UreticiSurrogated surrogated = new UreticiSurrogated();
                surrogated.Ad = uretici.Ad;
                surrogated.Id = uretici.Id;
                surrogated.Adres = uretici.Adres;
                return surrogated;
            }
            return obj;
        }

        // Schema import' u sırasında bu metod çalıştırılır.
        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            if (typeName == "UreticiSurrogated")
                return typeof(Uretici);
            return null;
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
```

Bu işlemin ardından servis tarafında davranışların (Behavior) Surrogater tipine göre özelleştirilmesi ve Metadata Export işlemleri için gerekli ek kodlamaların yapılması gerekmektedir. İlk olarak Surrogate implemantasyonunun nasıl yapılacağı ele alınmalıdır. Bu amaçla kod tarafında aşağıdaki adımlar izlenmelidir.

- İlk olarak ServiceHost nesnesi üzerinden tüm ServiceEndPoint nesneleri dolaşılır.
- Her bir ServiceEndpoint örneği içerisinden o andaki EndPoint bileşenine ait OperationDescription nesneleri bulunur.
- Bulunan OperationDescription nesneleri içerisinde DataContractSerializerOperationBehavior örnekleri Find metodu ile aranır.
- Eğer DataContractSerializerOperationBehavior örnekleri bulunursa, DataContractSurrogate özelliğine geliştirilen Surrogater nesne örneği atanır. Böylece ilgili operasyon çağrısında, orjinal tip ile Surrogate tipin eşleştirmelerini yapacak, bir birleri arasında serileştirme geçişlerini sağlayacak olan tip, WCF Runtime ortamına bildirilmiş olunur.
- Opsiyonel olarak eğer söz konusu davranış bulunamassa, DataContractSerializerOperationBehavior örneğinin oluşturulması, yine DataContractSurrogate özelliğie ilgili atamanın yapılması ve bukez davranışın ilgili operasyona eklenmesi gerekmektedir.

Söz konusu adımlara göre aşağıdaki gibi bir kod parçası geliştirilebilir.

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using UrunYonetim;

namespace Sunucu
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(UrunYonetici));
            SurrogaterEkle(host);
            host.Open();
            Console.WriteLine(host.State);
            Console.WriteLine("Kapatmak için bir tuşa basın");
            Console.ReadLine();
            host.Close();
        }

        private static void SurrogaterEkle(ServiceHost host)
        {
            foreach (ServiceEndpoint endPoint in host.Description.Endpoints)
            {
                foreach (OperationDescription oprDesc in endPoint.Contract.Operations)
                {
                    DataContractSerializerOperationBehavior dcBehavior = (DataContractSerializerOperationBehavior)oprDesc.Behaviors.Find<DataContractSerializerOperationBehavior>();
                    if (oprDesc.Behaviors.Find<DataContractSerializerOperationBehavior>() != null)
                        dcBehavior.DataContractSurrogate = new UreticiSurrogater();
                }
            }
        }
    }
}
```

Burada Servis uygulaması üzerinde SurrogaterEkle metodu ile söz konusu işlemler gerçekleştirilmektedir. Sunucu uygulama bu haliyle çalıştırıldığında ve debug penceresinde ServiceHost nesnesi Quick Watch ile izlendiğinde aşağıdaki ekran görüntüsü yakalanabilir.

![mk256_8.gif](/assets/images/2008/mk256_8.gif)

Bu ekran görüntüsünde, 0 indisli EndPoint üzerinde tanımlı olan IUrunYonetici sözleşmesine ait UrunEkle operasyonunun davranışlarına dikkat edilmelidir. DataContractSerializer tipinden olan davranışın içerisinde yer alan DataContractSurrogate özelliğinin değerinin, UreticiSurrogater olarak set edilmiş olduğu açık bir şekilde görülmektedir.

Surrogate kullanımında Microsoft tarafından önerilen Best Practices ise, bu tip operasyon davranışı atamaları ve şema export işlemleri için özel olarak yazılmış bir niteliğin (Attribute) kullanılmasını önermektedir. Yazıda geliştirilen örnek düşünüldüğü takdirde bu niteliğin servis sözleşmesinde (Service Contract) kullanılması, IContractBehavior, IOperationBehavior ve IWsdlExportExtension arayüzlerini uygulaması gerekmektedir. IContractBehavior ve IOperationBehavior arayüzlerinin bazı üyeleri kullanılarak yukarıdaki kod parçasında yapılan davranış tanımlamaları gerçekleştirilebilmektedir. Diğer taraftan IWsdlExportExtension arayüzü ile gelen metodlar sayesinde, Metadata yayınlamasının Surrogate tipine göre yapılabilmesi sağlanabilmektedir. Söz konusu nitelik aşağıdaki gibi geliştirilebilir.

![mk256_9.gif](/assets/images/2008/mk256_9.gif)

```csharp
using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace UrunYonetim
{
    public class SurrogaterAttribute 
        : Attribute
        , IContractBehavior
        , IOperationBehavior
        , IWsdlExportExtension
    {
        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime proxy)
        {
            foreach (OperationDescription oprDesc in description.Operations)
            {
                SurrogateUygula(oprDesc);
            }
        }

        public void ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatch)
        {
            foreach (OperationDescription oprDesc in description.Operations)
            {
                SurrogateUygula(oprDesc);
            }
        }

        public void Validate(ContractDescription description, ServiceEndpoint endpoint)
        {
        }

        #endregion

        #region IWsdlExportExtension Members
    
        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            if (exporter == null)
                throw new ArgumentNullException("exporter");

            object dataContractExporter;
            XsdDataContractExporter xsdDCExporter;
            if (!exporter.State.TryGetValue(typeof(XsdDataContractExporter), out dataContractExporter))
            {
                xsdDCExporter = new XsdDataContractExporter(exporter.GeneratedXmlSchemas);
                exporter.State.Add(typeof(XsdDataContractExporter), xsdDCExporter);
            }
            else
            {
                xsdDCExporter = (XsdDataContractExporter)dataContractExporter;
            }
            if (xsdDCExporter.Options == null)
                xsdDCExporter.Options = new ExportOptions();
    
            if (xsdDCExporter.Options.DataContractSurrogate == null)
                xsdDCExporter.Options.DataContractSurrogate = new UreticiSurrogater();
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
        }

        #endregion

        #region IOperationBehavior Members
    
        public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
        {
            SurrogateUygula(description);
        }

        public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
        {
            SurrogateUygula(description);
        }

        public void Validate(OperationDescription description)
        {
        }

        #endregion

        private static void SurrogateUygula(OperationDescription description)
         {
            DataContractSerializerOperationBehavior dcsOperationBehavior = description.Behaviors.Find<DataContractSerializerOperationBehavior>();
            if (dcsOperationBehavior != null)
            {
                if (dcsOperationBehavior.DataContractSurrogate == null)
                    dcsOperationBehavior.DataContractSurrogate = new UreticiSurrogater();
            }
        }
    }
}
```

Artık tek yapılması gereken söz konusu niteliğin servis sözleşmesinde aşağıdaki gibi uygulanmasıdır.

```csharp
[ServiceContract(Name="UrunServisi",Namespace="http://www.bsenyurt.com/UrunServisi")] 
[Surrogater]
public interface IUrunYonetici
{
    [OperationContract]
    void UrunEkle(Urun urun);

    [OperationContract]
    Urun UrunGuncelle(Urun urun);
}
```

Bu işlemlerin tamamlanması ile birlikte istemci için gerekli olan proxy ve config üretimleri gerçekleştirilebilir. Servis uygulaması çalışıyorken svcutil aracı kullanılırsa, aşağıdaki ekran görüntüsünde olduğu gibi ilgili üretimlerin başarılı bir şekilde yapıldığı görülebilir.

![mk256_10.gif](/assets/images/2008/mk256_10.gif)

Bu adımların arından istemci için üretilen proxy (UrunYonetici.cs) ve konfigurasyon (output.config) dosyaları örnek bir Console uygulamasında aşağıdaki kod parçasında olduğu gibi kullanılabilir.

```csharp
using System;
using UrunYonetim;

namespace Istemci
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Başlamak için bir tuşa basınız");
            Console.ReadLine();
            UrunServisiClient client = new UrunServisiClient("UrunServisiTcpEndPoint");
            Urun urn = new Urun()
            {
                Ad = "Mouse",
                BirimFiyat = 12,
                Id = 1,
                StokMiktari = 50,
                Ureten = new Vendor()
                {
                    Id = 1001,
                    Ad = "Adventure 1",
                    Adres = "Adventure 1 Yolu Üzeri Minesotta"
                }
            };
            client.UrunEkle(urn);
            Urun guncellenen = client.UrunGuncelle(urn);
            Console.WriteLine("{0} guncellendi", guncellenen.Ad);
        }
    }
}
```

UrunServisiClient isimli nesne konfigurasyonda dosyasından belirtilen ilgili EndPoint noktasına göre oluşturulduktan sonra, Urun sınıfına ait bir nesne C# 3.0 object initializer ile örneklenmektedir. Bu örnekleme sırasında Ureten özelliği Vendor isimli tipe ait bir örnek almaktadır. (Burada Vendor için varsayılan yapıcı metodun var olduğuna, oysaki serileştirilemeyen Uretici tipinde böyle yapıcının yazılmadığına dikkat edilmelidir.) Sonrasında ise servis üzerinden önce UrunEkle ardından UrunGuncelle metodları çağırılmaktadır. Önce servis uygulaması ardındanda istemci uygulama çalıştırılırsa aşağıdakine benzer bir ekran çıktısı alınır.

![mk256_11.gif](/assets/images/2008/mk256_11.gif)

İstemci uygulama için üretilen proxy dosyası içeriğine bakıldığında ise, aşağıdaki sınıf diagramında (Class Diagram) görüldüğü gibi Vendor isimli bir tipin üretildiği farkedilebilir.

![mk256_12.gif](/assets/images/2008/mk256_12.gif)

Vendor adı tesadüfi değildir nitekim, UreticiSurrogated sınıfında kullanılan DataContract niteliğinde, Name özelliğine bu değer verilmiştir. Bu sebepten dolayı istemci tarafına Metadata aktarımı sırasında taşınan tipin adıda Vendor olarak set edilmektedir. Vendor isimli sınıf aslında serileştirilemeyen Uretici tipinin yerine istemci tarafında kullanılan vekildir. Buraya kadar anlatılanlar ile serileştirilemeyen bir veri sözleşmesinin, Surrogate teknikleri sayesinde istemci tarafına nasıl aktarılabileceği ve servis ile olan iletişimde nasıl kullanılabileceği incelenmiştir. Surrogate kullanımı ile ilgili olaraktan daha detaylı bilgi için [http://msdn.microsoft.com/en-us/library/ms733064.aspx](http://msdn.microsoft.com/en-us/library/ms733064.aspx) adresindeki makaleyi takip etmenizi; ayrıca örneği daha kavrayabilmek için mutlaka breakpoint'ler ile incelemenizi öneririm. Böylece geldik bir makalemizin daha sonuna.Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/Surrogate.rar)