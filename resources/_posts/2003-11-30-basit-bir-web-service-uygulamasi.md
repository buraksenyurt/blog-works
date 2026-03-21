---
layout: post
title: "Basit Bir Web Service Uygulaması"
date: 2003-11-30 14:00:00 +0300
categories:
  - xml-web-services
tags:
  - xml-web-service
  - soap
  - soap-based-service
---
Bugünkü makalemizde web servislerinin nasıl kullanıldığını göreceğiz. Her zaman olduğu gibi konuyu açıklayıcı basit bir örnek üzerinde çalışacağız. Öncelikle web servisi nedir, ne işe yarar bunu açıklamaya çalışalım. Web servisi, internet üzerinden erişilebilen, her türlü platform ile bağlantı kurabileceğimiz, geriye sonuç döndüren (döndürmeye) fonksiyonelliklere ve hizmetlere sahip olan bir uygulama parçasıdır. Aşağıdaki şekil ile konuyu zihnimizde daha kolay canlandırabiliriz.

![mk10_1.gif](/assets/images/2003/mk10_1.gif)

Şekil 1. Web Servislerinin yapısı

lŞekildende görüldüğü gibi SOAP isminde bir yapıdan bahsediyoruz. SOAP Simple Object Access Protocol anlamına gelen XML tabanlı bir haberleşme teknolojisidir. Web servisi ile bu web servisinin kullanan uygulamalar arasındaki iletişimi sağlar. Dolayısıyla, web servisleri ve bu servisleri kullanan uygulamaların birbirlerini anlayabilmesini sağlar. Web servislerinden uygulamalara giden veri paketleri ve uygulamalardan web servislerine giden veri paketleri bu standart protokolü kullanmaktadır. Web servislerinin yazıldığı dil ile bunlaru kullanan uygulamaların (bir windows application, bir web application vb...) yazıldığı diller farklı olabilir. SOAP aradaki iletişim standartlaştırdığından farklı diller sorun yaratmaz. Tabi bunu sağlayan en büyük etken SOAP’ın XML tabanlı teknolojisidir.

Bir web servis uygulaması yaratıldığında, bu web servisi kullanacak olan uygulamaların web servisinin nerede olduğunu öncelikle bilmesi gerekir. Bunu sağlayan ise web proxy’lerdir. Bir web proxy yazmak çoğunlukla kafa karıştırıcı kodlardan oluşmaktadır. Ancak VS.Net gibi bir ortamda bunu hazırlamakta oldukça kolaydır. Uygulamaya, web servisin yeri Web Proxy dosyası ile bildirildikten sonra, uygulamamızdan bu web servis üzerindeki metodlara kolayca erişebiliriz.

Şimdi, basit bir web servis uygulaması geliştireceğiz. Öncelikle web servis’imizi yazacağız. Daha sonra ise, bu web servisini kullanacağımız bir windows application geliştireceğiz. Normalde birde web proxy uygulaması geliştirmemiz gerekiyor. Ancak bu işi VS.NET’e bırakacağız.

Basit olması açısından web servis’imiz, bulunduğu sunucu üzerindeki bir sql sunucusunda yer alan bir veritabanından, bir tabloya ait veri kümesini DataSet nesnesi olarak, servisi çağıran uygulamaya geçirecek. Geliştirdiğim uygulamam aynı makine üzerindeki bir web servisini kullanıyor. Öncelikle web servisimizi oluşturalım. Bunun için New Project’ten ASP.NET Web Service’ı seçiyoruz. Görüldüğü gibi uygulama otomatik olarak internet information server’ın kurulu olduğu sanal web sunucusunda oluşturuluyor. Dolayısıyla oluşturulan bu web servis’e bir browser yardımıylada erişebiliriz.

![mk10_3.jpg](/assets/images/2003/mk10_3.jpg)

Şekil 2. Web Service

Bu işlemi yaptığımız takdirde Service1.asmx isimli bir dosya içeren bir web servis uygulamasının oluştuğunu görürüz. Web servislerinin dosya uzantısı asmx dir. Bu uzantı çalışma zamanında veya bir tarayıcı penceresinde bu dosyanın bir web servis olduğunu belirtir.

![mk10_4.jpg](/assets/images/2003/mk10_4.jpg)

Şekil 3. Web servisleri asmx uzantısına sahiptir.

lTo Switch Code Window ile kod penceresine geçtiğimizde aşağıdaki kodları görürüz. (Burada, Service1 ismi SrvKitap ile değiştirilmiş aynı zamanda Constructor’un adı ve Solution Explorer’da yer alan dosya adıda SrvKitap yapılmıştır.)

```csharp
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services; 
namespace KitapServis
{
      /// <summary>
      /// Summary description for Service1.
      /// </summary>
      public class SrvKitap : System.Web.Services.WebService
      {
            public SrvKitap()
            {
                  //CODEGEN: This call is required by the ASP.NET Web Services Designer
                  InitializeComponent();
            }
 
            #region Component Designer generated code
            //Required by the Web Services Designer
            private IContainer components = null;
            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {

            }

             /// <summary>
            /// Clean up any resources being used.
            /// </summary>
            protected override void Dispose( bool disposing )
            {
                  if(disposing && components != null)
                  {
                        components.Dispose();
                  }
                  base.Dispose(disposing);
            }

            #endregion

            // WEB SERVICE EXAMPLE
            // The HelloWorld() example service returns the string Hello World
            // To build, uncomment the following lines then save and build the project
            // To test this web service, press F5

            // [WebMethod]
            // public string HelloWorld()
            // {
            // return "Hello World";
            // }
      }
}
```

Kodları kısaca inceleyecek olursak, web servislere ait özellikleri kullanabilmek için System.Web.Services namespace’inin eklendiğini, SrvKitap isimli sınıfın bir web servisin niteliklerine sahip olması için, System.Web.Services sınıfından türetildiğini görürüz. Ayrıca yorum satırı olarak belirtilen yerde VS.NET bize hazır olarak HelloWorld isimli bir web metodu sunmaktadır. Bu metodun başındada dikkat edicek olursanız [WebMothod] satırı yer alıyor. Bu anahtar sözcük, izleyen metodun bir web servis metodu olduğunu belirtmektedir. Şimdi biz kendi web servis metodumuzu buraya ekleyelim.

```csharp
[WebMethod]
public DataSet KitapListesi()
{
      SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
      SqlDataAdapter da=new SqlDataAdapter("Select Adi,Fiyat From Kitaplar",conFriends);
      DataSet ds=new DataSet();
      da.Fill(ds);
      return ds;
}
```

Eklediğimiz bu web metod sadece Sql sunucusu üzerinde yer alan Friends isimli veritabanına bağlanmakta ve burada Kitaplar isimli tablodan Adi ve Fiyat bilgilerini alarak, sonuç kümesini bir DataSet içine aktarmaktadır. Sonra metod bu DataSet’I geri döndürmektedir.Şimdi uygulamamızı derleyelim ve Internet Explorer’ı açarak adres satırına şunu girelim

http://localhost/kitapservis/SrvKitap.asmx

bu durumda, aşağıdaki ekran görüntüsünü alırız.

![mk10_5.jpg](/assets/images/2003/mk10_5.jpg)

Şekil 4. Internet Explorer penceresinde SrvKitap.asmx’in görüntüsü.

SrvKitap isimli web servisimiz burada görülmektedir. Şimdi KitapListesi adlı linke tıklarsak (ki bu link oluşturduğumuz KitapListesi adlı web servis metoduna işaret etmektedir)

![mk10_6.jpg](/assets/images/2003/mk10_6.jpg)

Şekil 5. Invoke

Invoke butonuna basarak web servisimizi test edebiliriz. Bunun sonucunda metod çalıştırılır ve sonuçlar xml olarak gösterilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
 <DataSet xmlns="http://tempuri.org/">
 <xs:schema id="NewDataSet" xmlns="" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
 <xs:element name="NewDataSet" msdata:IsDataSet="true" msdata:Locale="tr-TR">
 <xs:complexType>
 <xs:choice maxOccurs="unbounded">
 <xs:element name="Table">
 <xs:complexType>
 <xs:sequence>
  <xs:element name="Adi" type="xs:string" minOccurs="0" />
  <xs:element name="Fiyat" type="xs:decimal" minOccurs="0" />
  </xs:sequence>
  </xs:complexType>
  </xs:element>
  </xs:choice>
  </xs:complexType>
  </xs:element>
  </xs:schema>
<diffgr:diffgram xmlns:msdata="urn:schemas-microsoft-com:xml-msdata" xmlns:diffgr="urn:schemas-microsoft-com:xml-diffgram-v1">
 <NewDataSet xmlns="">
 <Table diffgr:id="Table1" msdata:rowOrder="0">
  <Adi>Delphi 5'e Bakış</Adi>
  <Fiyat>100.0000</Fiyat>
  </Table>
 <Table diffgr:id="Table2" msdata:rowOrder="1">
  <Adi>Delphi 5 Uygulama Geliştirme Kılavuzu</Adi>
  <Fiyat>250.0000</Fiyat>
  </Table>
 <Table diffgr:id="Table3" msdata:rowOrder="2">
  <Adi>Delphi 5 Kullanım Kılavuzu</Adi>
  <Fiyat>50.0000</Fiyat>
  </Table>
 <Table diffgr:id="Table4" msdata:rowOrder="3">
  <Adi>Microsoft Visual Basic 6.0 Geliştirmek Ustalaşma Dizisi</Adi>
  <Fiyat>75.0000</Fiyat>
  </Table>
 <Table diffgr:id="Table5" msdata:rowOrder="4">
  <Adi>Visual Basic 6 Temel Başlangıç Kılavuzu</Adi>
  <Fiyat>80.0000</Fiyat>
  </Table>
 <Table diffgr:id="Table6" msdata:rowOrder="5">
  <Adi>Microsoft Visual Basic 6 Temel Kullanım Kılavuzu Herkes İçin!</Adi>
  <Fiyat>15.0000</Fiyat>
  </Table>
 <Table diffgr:id="Table7" msdata:rowOrder="6">
  <Adi>ASP ile E-Ticaret Programcılığı</Adi>
  <Fiyat>25.0000</Fiyat>
  </Table>
 <Table diffgr:id="Table8" msdata:rowOrder="7">
  <Adi>ASP 3.0 Active Server Pages Web Programcılığı Temel Başlangıç Kılavuzu</Adi>
  <Fiyat>150.0000</Fiyat>
  </Table>
.
.
.
  </NewDataSet>
  </diffgr:diffgram>
  </DataSet>
```

Sıra geldi bu web servisini kullanacak olan uygulamamızı yazmaya. Örneğin bir Windows Uyulamasından bu servise erişelim. Yeni bir Windows Application oluşturalım ve Formumuzu aşağıdakine benzer bir şekilde tasarlayalım.

![mk10_7.jpg](/assets/images/2003/mk10_7.jpg)

Şekil 6 Form Tasarımı.

Formumuz bir adet datagrid nesnesi ve bir adet button nesnesi içeriyor. Button nesnemize tıkladığımızda, web servisimizdeki KitapListesi adlı metodumuzu çağıracak ve dataGrid’imizi bu metoddan dönen dataSet’e bağlıyacağız.Ancak öncelikle uygulamamıza, web servisin yerini belirtmeliyiz ki onu kullanabilelim.Bunun için Solution Explorer penceresinde Add Web Reference seçimi yapıyoruz.

![mk10_8.jpg](/assets/images/2003/mk10_8.jpg)

Şekil 7. Uygulamaya bir Web Reference eklemek.

Karşımıza gelen pencerede adres satırına http:\\localhost\KitapServis\SrvKitap.asmx (yani web servisimizin adresi) yazıyor ve Go tuşuna basıyoruz. Web service bulunduğunda bu bize1 Service Found ifadesi ile belirtiliyor.

![mk10_9.jpg](/assets/images/2003/mk10_9.jpg)

Şekil 8. Servisin eklenmesi.

Add Reference diyerek servisimizi uygulamamıza ekliyoruz. Bu durumda Solution Explorer’dan uygulamamızın içerdiği dosyalara baktığımızda yeni dosyaların eklendiğini görüyoruz.

![mk10_10.jpg](/assets/images/2003/mk10_10.jpg)

Şekil 9. Reference.cs adlı dosyasına dikkat.

Burada Reference.cs isimli dosya işte bizim yazmaktan çekindiğimi Web Proxy dosyasının ta kendisi oluyor. Bu dosyanın kodları ise aşağıdaki gibidir.

```csharp
//------------------------------------------------------------------------------
// <autogenerated>
// This code was generated by a tool.
// Runtime Version: 1.1.4322.573
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

//
// This source code was auto-generated by Microsoft.VSDesigner, Version 1.1.4322.573.
//
namespace KitapFiyatlari.localhost
{
      using System.Diagnostics;
      using System.Xml.Serialization¤
      using System;
      using System.Web.Services.Protocols;
      using System.ComponentModel;
      using System.Web.Services;
      /// <remarks/>
      [System.Diagnostics.DebuggerStepThroughAttribute()]
      [System.ComponentModel.DesignerCategoryAttribute("code")]
      [System.Web.Services.WebServiceBindingAttribute(Name="SrvKitapSoap", Namespace="http://tempuri.org/")]
      public class SrvKitap : System.Web.Services.Protocols.SoapHttpClientProtocol
      {
            /// <remarks/>
            public SrvKitap()
            {
                  this.Url = "http://localhost/KitapServis/SrvKitap.asmx";
            }
            /// <remarks/>
            [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/KitapListesi", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
            public System.Data.DataSet KitapListesi()
            {
                  object[] results = this.Invoke("KitapListesi", new object[0]);
                  return ((System.Data.DataSet)(results[0]));
            }
            /// <remarks/>
            public System.IAsyncResult BeginKitapListesi(System.AsyncCallback callback, object asyncState)
            {
                  return this.BeginInvoke("KitapListesi", new object[0], callback, asyncState);
            }
            /// <remarks/>
            public System.Data.DataSet EndKitapListesi(System.IAsyncResult asyncResult)
            {
                  object[] results = this.EndInvoke(asyncResult);
                  return ((System.Data.DataSet)(results[0]));
            }
       }
}
```

Karmaşık olduğu gözlenen bu kodların açıklamasını ilerleyen makalerimde işyeceğim. Artık web servisimizi uygulamamıza eklediğimize gore, bunu kullanmaya ne dersiniz. İşte button nesnemize tıklandığında çalıştırılacak kodlar.

```csharp
private void button1_Click(object sender, System.EventArgs e)

{

      localhost.SrvKitap srv=new localhost.SrvKitap(); /* Servisimizi kullanabilmek için bu servisten bir örnek nesne yaratıyoruz*/

      DataSet ds=new DataSet();

      ds=srv.KitapListesi(); /* Servisteki KitapListesi isimli metodumuzu çağırıyoruz.*/

      dataGrid1.DataSource=ds;

}
```

![mk10_11.jpg](/assets/images/2003/mk10_11.jpg)

Şekil 10. Sonuç.

Evet geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinizze mutlu günler dilerim.