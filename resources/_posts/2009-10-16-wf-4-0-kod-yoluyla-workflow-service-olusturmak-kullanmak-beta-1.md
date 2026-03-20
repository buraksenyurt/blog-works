---
layout: post
title: "WF 4.0 - Kod Yoluyla Workflow Service Oluşturmak, Kullanmak [Beta 1]"
date: 2009-10-16 00:00:00 +0300
categories:
  - wf-4-0-beta-1
tags:
  - wf-4-0-beta-1
  - csharp
  - dotnet
  - aspnet
  - linq
  - wcf
  - workflow-foundation
  - wpf
  - windows-forms
  - xml
  - http
  - serialization
  - visual-studio
---
Yükseklik korkum olmasına rağmen her zaman yandaki gibi tırmanışta olanlara imrenmişimdir. Bu fotoğrafa konu olan kişinin tek yaptığı yoğun bir mücadele ve efor ile yukarı doğru tırmanmaktır. Bana göre sonuçta elde edilebilecek tek şey zirveye ulaşmak ve oranın eşsiz manzarasını izlemekten ibarettir. Tabi bunun birde inişi olduğunu düşünmek gerekiyor

![blg90_Giris.jpg](/assets/images/2009/blg90_Giris.jpg)

![Sealed](/assets/images/2009/smiley-sealed.gif)

Hangi açıdan bakarsak bakalım bizde hayatımızda zaman zaman böyle mücadeleler içerisine gireriz. Özellike yazılım geliştirirken

![Laughing](/assets/images/2009/smiley-laughing.gif)

Örneğin her zaman elimizin altında Visual Studio IDE'sinin sunduğu gibi gelişmiş arayüzler bulunmayabilir. Örneğin Visual Studio 2010 Beta 1 üzerinde yaşadığım sorunlardan birisi WPF tabanlı Designer'ı Workflow uygulamaları için kullanamıyor oluşumdu. Bu gerçekten çok üzücü bir durum.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Ama çaresiz değiliz. Çaresizliğin çözümü bazı işlemleri basit bir Console uygulamasında, gereklilikleri fark ederek (örneğin hangi Assembly'ların referans edilmesinin gerektiğinin bilinmesi...), kod bazında yapmaktan ibarettir. Bunun bize sağlayacağı pek çok fayda bulunmaktadır.

Kod tarafında her ne kadar mücadeleci bir yol izlesekte, neyin nasıl oluşturulması gerektiğini, hangi durumlarda ne gibi istisnalara (Exceptions) düşebileceğimizi, nelere ihtiyaç duyduğumuzu ve arka planda aslında işlemlerin nasıl değerlendirildiğini görmek açısından yararlı bir yoldur. Bu kadar cümleyi elbetteki sizi yazının kalanına motive etmek için sarfettiğimi düşünebilirsiniz.

![Cool](/assets/images/2009/smiley-cool.gif)

Haydi gelin kamera arkasına bakalım.

> Kişisel Not: Aslında Beta 2 sürümünde (ki henüz public olarak yayınlanmadığını biliyorsunuz), designer tarafındaki sorunların aşıldığını ifade edebilirim. Bizzat tecrübe ile sabitlenmiştir
>
> ![Smile](/assets/images/2009/smiley-smile.gif)
>
> Hatta bu konu ile ilişkili bir yazımı söz konusu sürüm public hale geldikten sonra yayınlıyor olacağım.

Bu yazımızda.Net Framework 4.0 Beta 1 ile bir Workflow Service'in oluşturulması, host edilmesi ve bir istemci tarafından kullanılması konusu irdelenmeye çalışılacaktır. Workflow Service tek yönlü bir operasyon (One Way) için hizmet vermekte olup istemci tarafına bir geri bildirimde bulunmamaktadır. Her iki uygulamada birer Console Application olarak tasarlanmıştır. İstemci tarafında Workflow Service'in kullanılabilmesi için gerekli Proxy nesnesi yine kod yardımıyla (WSDL dökümanından yararlanmadan) oluşturulmaktadır. Aslında tamamlanmış olan uygulamalara baktığımızda servis ve istemci tarafı için gerekli olan referans Assembly'ların aşağıdaki şekilde görüldüğü gibi olduğunu fark edebiliriz.

![blg90_RequiredReferences.gif](/assets/images/2009/blg90_RequiredReferences.gif)

Tamam çok güzel ama biz bu yazımızda tam olarak neyi hedeflemekteyiz?

Yapmak istediğimiz ilk şey WF 4.0 bazlı olarak bir Workflow Service geliştirmek olacaktır. Bilindiği üzere Workflow Service'ler, istemcilerin uzaktan erişerek başlatabileceği akışları içerebilecek hizmetler olarak düşünülebilir. Öyleyse Workflow Service'in istemciden gelecek talepleri alabilmesi, gerektiğinde istemciye dönüş yapabilmesi ve kendi içerisinde bir akışı barındırması gerekmektedir.

Tahmin edileceği üzere istemci üzerinden gelecek taleplerin alınması veya cevapların gönderilmesi sırasında hazır aktivite bileşenlerinden yararlanılır. Örneğin Receive veya SendReply...Elbette bu tip bir servisin geliştirilmesi yeterli değildir. Bu servisin istemcilere hizmet verebilmesi için ayrıca host edilmesi de gerekmektedir. İstemci tarafı ise standart bir Console, WinForms, WPF, Asp.Net ucu olabileceği gibi başka bir Workflow Service de olabilir. Biz işe ilk olarak Workflow Service tarafını geliştirerek başlayacağız. Bu amaçla WithWCF isimli Console uygulamamızın kodlarını aşağıdaki gibi geliştirdiğimizi düşünebiliriz.

Workflow Service tarafı kodları;

```csharp
using System;
using System.Activities;
using System.Activities.Statements;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.Xml.Linq;

namespace WithWCF
{
    class Program
    {
        static void Main(string[] args)
        {
            // Örnekte yer alan akış tek-yönlü bir Workflow Service' idir. Sequence tipinden olan bu akış, Receive aktivitesi ile başlamakta olup istemciden gelen double tipinden değişkenin karekökünü hesaplamakta ama istemci tarafına bir bilgilendirmede bulunmamaktadır.

            #region Sequence Aktivitesi Oluşturulma İşlemleri

            // Sequence tipinden bir aktivite örneklenir
            Sequence squareRootFlow = new Sequence();

            // Aktivitede kullanılacak olan Variable tanımlamaları yapılır
            Variable<double> number = new Variable<double>(); // Receive aktivitesi tarafından alınan değişken
            Variable<double> square = new Variable<double>(); // Sonuç değişkeni
            Variable<CorrelationHandle> corHandle = new Variable<CorrelationHandle>();
            // Xml Namespace tanımlaması yapılır. XNamespace kullanılmadığı takdirde örneğin Receive aktitivesinin ServiceContractName özelliğine adres bilgisi text tabanlı olarak atandığında şekilde görülen exception üretilir.
            XNamespace ns = "http://www.buraksenyurt.com/WF4";

            // Variable tanımlamaları Sequence aktivitesinin Variables koleksiyonuna dahil edilir.
            squareRootFlow.Variables.Add(number);
            squareRootFlow.Variables.Add(square);
            squareRootFlow.Variables.Add(corHandle);

            // Akışın içerisinde yer alan ilk aktivite Receive tipindendir.
            Receive receive1 = new Receive
            {
                 OperationName="SquareRoot",
                 DisplayName="Square Root Calculation",
                 ServiceContractName = ns + "CalculatorService",
                 CanCreateInstance=true,
                 Value=new OutArgument<double>(number), 
                 AdditionalCorrelations={
                      {"ChannelBasedCorellation",new InArgument<CorrelationHandle>(corHandle)}
                  }
            };

            // Receive aktivitesi ile istemciden gelen sayısal değer number değişkenine alındıktan sonra çalışan InvokeMethod aktivitesi ile Calculator isimli sınıf içerisindeki FindSquareRoot metodu çağırılır.             
            InvokeMethod<double> invokeMethod1 = new InvokeMethod<double>
            {
                 TargetType=typeof(Calculator),
                 MethodName = "FindSquareRoot",
                 Result=new OutArgument<double>(square) // FindSquareRoot metodunun çalıştırılması sonucu elde edilen sonuç square isimli değişken tarafında yakalanabilecektir.
            };
            // Metoda parametre olarak number değişkeninin değeri gönderilir. Bu değer Receive aktivitesi içerisinde set edilmiş olup istemci tarafından gelmektedir.
            invokeMethod1.Parameters.Add(new InArgument<double>(number));

            // Ekrana bilgilendirme yapılır. Bu bilgilendirmede istemciden akışa gelen sayının karekökü yazdırılmaktadır.
            WriteLine writeLine1 = new WriteLine
            {
                Text=new InArgument<string>(e=>String.Format("Sonuç {0}",square.Get(e).ToString()))
            };

            // Aktivitelere sırasıyla Sequence aktivitesi içerisine ilave edilir
            squareRootFlow.Activities.Add(receive1);
            squareRootFlow.Activities.Add(invokeMethod1);
            squareRootFlow.Activities.Add(writeLine1);
            
            #endregion

            #region Workflow Servis Oluşturma İşlemleri

            // Workflow' un Service olarak host edilmesi için gerekli hazırlıklar başlar.
            // İlk olarak bir Service nesne örneği oluşturulur
            Service service = new Service();

            // Service nesne örneğinin Implementation özelliğine WorkflowServiceImplementation türünden bir referans atanırken, Body özelliğine yukarıda oluşturulan Workflow aktivitesi bildirilir
            service.Implementation = new WorkflowServiceImplementation
            {
                Name = ns+"CalculatorService",
                Body=squareRootFlow                
            };

            // Sonuçta Workflow bir WCF servisi olarak host edileceğinden bir Endpoint bilgisine sahip olmalıdır
            // Bu nedenle basit bir Endpoint bildirimi yapılır
            // Örnekte Tcp bazlı servis iletişimi tercih edilmiştir
            service.Endpoints.Add(new Endpoint
            {                
                Uri = new Uri("SquareRoot", UriKind.Relative), //Address bilgisi
                Binding=new NetTcpBinding(), // Binding bilgisi
                ServiceContractName = ns + "CalculatorService" // Contract bilgisi(Burada verilen isim ile Receive aktivitesine ait ServiceContractName özelliğindeki değerler aynı olmalıdır. Aksi halde Exception2 alınır)
            });

            #endregion

            // Workflow Service' i çalıştıracak olan WorkflowServiceHost nesnesi örneklenir
            // İlk parametre host edilecek olan servistir. İkinci parametre ise servisin host edileceği adres bilgisidir.
            WorkflowServiceHost host = new WorkflowServiceHost(service, new Uri("net.tcp://localhost:4501/Calculator/Workflows/"));
            host.Open(); // Host açılır

            Console.WriteLine(host.State.ToString()); //Hostun durumu hakkında bilgilendirme yapılır
            Console.WriteLine("Kapatmak için bir tuşa basınız.");
            Console.ReadLine();

            host.Close(); //Host kapatılır
        }
    }

    class Calculator
    {
        public double FindSquareRoot(double number)
        {
            return Math.Sqrt(number);
        }
    }
}
```

Kod içerisinde işleyiş ile ilişkili gerekli açıklamalar bulunmaktadır.

Ancak geliştirme sırasında dikkat edilmesi gereken bazı durumlar da vardır. Örneğin XNamespace tipinin kullanılmaması, bunun yerine URL bilgisinin text tabanlı olarak atanması halinde çalışma zamanında aşağıdaki ekran görüntüsünde yer alan XmlException istisnası alınacaktır.

Exception 1 (Xml Namespace kullanılmaması halinde);

![blg90_Exception.gif](/assets/images/2009/blg90_Exception.gif)

Diğer yandan ServiceContractName değerinin hem Endpoint hemde Receive aktivitesi için aynı olması gerekmektedir ki aksi durumda aşağıdaki ekran görüntüsünde yer alan çalışma zamanı istisnası fırlatılmaktadır.

Exception 2 (ServiceContractName'lerin Receive aktivitesi ve Endpoint içerisinde farklı olmaları halinde);

![blg90_Exception2.gif](/assets/images/2009/blg90_Exception2.gif)

Gelelim istemci tarafına. Bu noktada kendimizi yine zora sokuyor olacağız.

![Yell](/assets/images/2009/smiley-yell.gif)

Elimizde servisin Publish edilen bir WSDL dökümanı olmadığını ve herhangibir şekilde Proxy üretimi için bir araç kullanmadığımızı farz edeceğiz. Bu durumda istemci tarafındaki proxy sınıfının ve hatta istemciden servis tarafına gönderilecek olan mesaj sözleşmesinin manuel kod ile oluşturulması gerekmektedir. Aynen aşağıda olduğu gibi;

İstemci tarafı sınıf diagramı;

![blg90_ClientDiagram.gif](/assets/images/2009/blg90_ClientDiagram.gif)

ve kodları;

```csharp
using System;
using System.ServiceModel;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Başlamak için bir tuşa basınız");
            Console.ReadLine();

            // Binding oluşturulur
            NetTcpBinding binding = new NetTcpBinding();
            // Endpoint tanımlaması yapılır. Adres bilgisinin servisin yeri ve çağırılmak istenen operasyonun adıo yer almaktadır. Bu bilgileri service tarafındakiler ile aynı olmalıdır
            EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:4501/Calculator/Workflows/SquareRoot");
            // Kanal fabrikası üretilir. İlk parametre bağlayıcı ikinci parametre ise EndPoint bilgisidir
            ChannelFactory<CalculatorService> factory = new ChannelFactory<CalculatorService>(binding, endpoint);
            // Kanal fabrikasından yararlanılarak istemcinin kullanacağı Transparant Proxy nesnesi üretilir.
            CalculatorService proxy = factory.CreateChannel();

            // Talep oluşturulur ve parametre olarak servis tarafındaki akışın ilk aktivitesi olan Receive aktivitesinin alacağı sayısal değer bildirilir
            SquareRootRequest request = new SquareRootRequest()
            {
                Number = 16
            };
            // Operasyon çağırılır
            proxy.SquareRoot(request);

            Console.WriteLine("İşlemler tamamlandı. Kapatmak için bir tuşa basınız.");
            Console.ReadLine();
        }
    }

    // Talep olarak gidecek mesaj sözleşmesi tanımlanır
    [MessageContract(IsWrapped = false)]
    public class SquareRootRequest
    {
        [MessageBodyMember(Namespace= "http://schemas.microsoft.com/2003/10/Serialization/",Name = "double")]
        public double Number { get; set; }
    }

    // Proxy tanımlaması yapılır
    // Workflow Servisi varsayılan isim alanı(namespace) ile sunulmadığından Namespace bildiriminin istemci tarafındaki proxy için açık bir şekilde yapılması gerekmektedir.
    [ServiceContract(Namespace = "http://www.buraksenyurt.com/WF4")]
    interface CalculatorService
    {
        [OperationContract(IsOneWay=true)] // Operasyonun OneWay olduğunu belirtmessek Exception3' teki çalışma zamanı hatasını alırız.
        void SquareRoot(SquareRootRequest request);
    }
}
```

Yine istemci tarafı açısından olaya baktığımızda dikkat etmemiz gereken bazı hususlar olduğu ortadadır. Örneğin, Workflow Service içerisinden geriye bir dönüş yapılmamaktadır. Bir başka deyişle tek yönlü bir istek söz konusu olabilir. Bu nedenle istemci tarafındaki SquareRoot metoduna uygulanan OperationContract niteliğinde IsOneWay özelliğine true değeri atanmıştır. Bu yapılmadığı takdirde çalışma zamanında aşağıdaki istisna mesajı ile karşılaşılacaktır.

Exception 3 (Service operasyonunun OneWay olduğunu belirtmediğimiz durumda);

![blg90_Exception3.gif](/assets/images/2009/blg90_Exception3.gif)

Piuuuuuvvvvv!!!

![Smile](/assets/images/2009/smiley-smile.gif)

Biraz uğraştık ama faydalı bir çalışma oldu sanıyorum ki. Gerçi elde edeceğimiz sonuç hiç bir anlam ifade etmesede, bir Workflow Service'in kod yardımıyla nasıl oluşturulabileceğini ve kullanılacağını görmüş olduk. İşte bir yazılımcı olarak tırmandığımız dağın zirvesindeki görüntü...

![blg90_Runtime.gif](/assets/images/2009/blg90_Runtime.gif)

Ne kadar muhteşem değil mi?

![Tongue out](/assets/images/2009/smiley-tongue-out.gif)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WithWCF.rar (42,98 kb)](/assets/files/2009/WithWCF.rar)

> Kişisel Not: Örnekler bildiğiniz üzere Visual Studio 2010 Beta 1 sürümünde ve.Net Framework Beta 1 üzerinde geliştirilmektedir. Beta 2 ile arasında farklılıklar olabilir. Hatta Relase sürümde çok daha fazla farklılık görülebilir.
