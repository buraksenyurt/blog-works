---
layout: post
title: "Kod Bazlı Workflow Service Geliştirmek ve Yayınlamak"
date: 2010-10-04 16:20:00 +0300
categories:
  - wf
  - wf-4-0
tags:
  - wf
  - wf-4-0
  - csharp
  - xml
  - dotnet
  - aspnet
  - wcf
  - workflow-foundation
  - wpf
  - windows-forms
  - xaml
  - http
  - iis
  - visual-studio
---
Bildiğiniz üzere bir süredir [NedirTv?com](http://www.nedirtv.com) desteğinde ["Workflow Foundation 4.0 Öğreniyorum"](http://www.nedirtv.com/kategori/Workflow-Foundation-Ogreniyorum.aspx) isimli bir seri üzerinde çalışmaktayız. Bu seride başlangıç seviyesinden orta seviyeye kadar, bir kaç ayrı derste Workflow Foundation kavramını öğrenmeye gayret ettik. Bu seriye dahil etmek istediğim bir konu da, Workflow Service örneklerinin tamamen kod bazında yazılması ve IIS (Internet Information Services) dışındaki bir uygulama tarafından host edilmesiydi. Ancak konu biraz karmaşık olduğundan ve tabiri yerinde ise yandaki resimde görülen Puzzle'a benzemediğinden, yazı haline getirilmesinin daha iyi olacağına karar verdim. Hem böylece ben de unuttuğum zamanlarda bu yazıma bakarak hatırlayabilirim. Öyleyse derin bir nefes alalım ve yola koyulalım.

![blg193_Giris.jpg](/assets/images/2010/blg193_Giris.jpg)

İlk olarak ne yapmak istediğimizi açık ve net bir şekilde ortaya koymaya çalışalım. WCF Eco System yapısının da önemli bir parçası olan Workflow Service'ler yardımıyla iş akışlarının servis bazlı olarak dış ortama sunulması mümkündür. Bu noktada özellikle Visual Studio 2010 tarafında yer alan WCF Workflow Service Application şablonu ve Workflow Designer işlerimizi inanılmaz ölçüde kolaylaştırmaktadır. Ancak elimizin altında sadece.Net Framework 4.0 olduğunu düşünelim. Hımmm...

![Wink](/assets/images/2010/smiley-wink.gif)

Bu durumda Visual Studio 2010 gibi bir IDE'mizin olmadığını da varsayarsak bir Workflow Service örneğinin XAML içeriğini yazmak istemeyebiliriz. Dolayısıyla kod tarafında.Net tiplerinden yararlanarak ilerlemek daha kolay olabilir (Gerçi biz örneğimizdeki kod parçasını Visual Studio 2010 ile yazıyoruz ama olsun. Kimseye çaktırmayın)

Ne yapmak istediğimizi sanıyorum ki biraz daha net anlayabildik. En basit haliyle bir Workflow Service örneğini kod tarafında oluşturmak istiyoruz. Ancak bu yeterli değil. Nitekim tasarlanan bu Workflow Service örneğinin aynı zamanda host edilerek kullanıma sunulması da gerekmektedir. İşte bu noktada IIS (Internet Information Services) dışında bir uygulamayı geliştirmek istediğimizi düşünebiliriz. İlk etapta basit bir Console uygulaması işimizi görebilir. (Ancak tabiki WPF, Windows Forms hatta bir Asp.Net uygulamas dahi Workflow Service örneklerini host edip çalıştıracak şekilde tesis edilebilir) Bu Console uygulaması, WorkflowServiceHost tipinden de yararlanarak gerekli çalışma zamanını tesis edecek ve bildirilen Workflow Service örneğini dış ortama sunuyor olacaktır.

İşe başlamadan önce eğer imkanınız var ise bir Workflow Service örneğinin Visual Studio 2010 ortamında WPF tabanlı Designer yardımıyla geliştirilmesini incelemenizi öneririm. Geliştireceğimiz Workflow Service bir servis olduğundan istemciden gelecek olan çağrıları kabul etmeli ve bir iş akışı başlatarak sonuçları istemci tarafına yönlendirmelidir. WCF tabanlı bir servis söz konusu olduğuna göre istemcinin çağrıda bulunabileceği operasyonların ve dolayısıyla servis sözleşmesinin (Service Contract) önceden tanımlanmış olması gerekmektedir. Ki bu sayede istemcilerin söz konusu adresten yayınlanan servis üzerinden hangi operasyonları çağırabileceği belirlenmiş olacaktır. İşte örneğimizde kullanacağımız servis sözleşmesi.

```csharp
[ServiceContract]
public interface IHelloService
{
	[OperationContract]
	double Sum(double x, double y);
}
```

IHelloService interface tipi dikkat edileceği üzere ServiceContract niteliği ile imzalanmıştır. Diğer taraftan çok basit olarak Sum isimli bir operasyon içermektedir. Söz konusu operasyon istemci tarafına açılacak olan bir fonksiyonelliği belirtmektedir. Sum isimli operayonun aldığı double tipinden olan iki parametrede, istemci tarafından gönderilecek değişkenler olduğunu göstermektedir.

Peki Workflow Service örnekleri dış dünyadan gelen istemci taleplerini nasıl almaktadır? Diyelim ki aldılar ve işettiler. İş akışı sonucu istemci tarafına bir değer göndermek isterlerse bunu nasıl gerçekleştirebilirler? Bu noktada Receive ve SendReply isimli aktivite bileşenlerinden yararlanıldığını söyleyebiliriz. Dolayısıyla kod tarafında bu aktivite bileşenlerini ele almamız gerekmektedir. Lakin bu aktivite bileşenleri başka bir Container içerisinde yer almalıdır. Örneğin bir Sequence aktivite bileşeni Container olarak düşünülebilir. Çok doğal olarak istemcilerin gönderdiği parametrelerin Sequence içerisindeki diğer aktivite bileşenleri tarafından da kullanılması gerekebilir. Bu durumda Sequence seviyesinde Variable tanımlamalarının yapılması uygundur. İhtiyacımız olan Variable tanımlamaları ise aşağıdaki gibidir.

```csharp
Variable<CorrelationHandle> __handle=new Variable<CorrelationHandle>("Request_Handle");
Variable<double> x=new Variable<double>("X");
Variable<double> y=new Variable<double>("Y");
Variable<double> result=new Variable<double>("Result");
```

x, y ve result isimli Variable tanımlamaları tahmin edeceğiniz üzere toplama işlemi için gereklidir. Ancak burada birde handle isimli CorrelationHandle tipinden Variable tanımlaması yer almaktadır. Workflow Service örnekleri oluşturulduğunda bildiğiniz üzere istemci ile arada bir oturum (Session) oluşmaktadır. Bu noktada özellikle istemciden gelen mesajın sunucu tarafındaki hangi servis örneğine ait olduğunun anlaşılması noktasında Correlation çeşitlerinden yararlanılmaktadır. Burada tanımlanan Variable, Receive aktivite bileşeni tarafından kullanılacaktır (Detaylar için [Correlation Nedir? Yenir mi? İçilir mi?](https://www.buraksenyurt.com/admin/post/Correlation-Nedir-Beta-2.aspx))

Aslında tam bu noktada Receive aktivitesini de tanımlayabiliriz. Aşağıdaki gibi.

```csharp
Receive receive=new Receive{
                             CanCreateInstance=true,
                               OperationName="Sum",
                                ServiceContractName="IHelloService",
                                CorrelatesWith=__handle,
                                Content=ReceiveContent.Create(new Dictionary<string,OutArgument>{
                                    {"xValue",new OutArgument<double>(x)},
                                    {"yValue",new OutArgument<double>(y)},
                                }
                                )
                        };
```

Receive aktivite bileşeni içerisinde set edilmiş özellikler önemlidir. OperationName ile dikkat edileceği üzere istemciler için kullanılabilecek bir operasyon adı bildirimi yapılmaktadır. Diğer yandan Content özelliğine atanan OutArgument tipli iki parametre, az önce tanımladığımız x ve y Variable'larını kullanmaktadır. Bir başka deyişle istemciler için xValue ve yValue isimli değişkenler tanımlanmıştır. ServiceContractName özelliği ile servis sözleşmesi bildirimi yapılmaktadır. Receive aktivite bileşeni için gerekli Correlation ayarı ise CorrelatesWith özelliği ile belirtilmektedir. Aslında Receive aktivite bileşeni biraz sonra kodlayacağımız WorkflowService örneğinin Body özelliği içerisinde kullanılmak istenebilir. Ancak böyle bir durumda SendReply aktivite bileşeninin ihtiyacı olan Request özelliğinin atanacağı Receive referansı tanımlanamayacaktır. Receive aktivite bileşeninin dışarıda tanımlanmasının sebebi budur. Artık WorkflowService nesne örneğini tanımlayarak yolumuza devam edebiliriz. İşte kod içeriğimiz.

```csharp
WorkflowService wfService = new WorkflowService
{
	Name = "HelloService",
	Endpoints = {
		new Endpoint
	{
		 ServiceContractName="IHelloService",
		  Binding=new BasicHttpBinding(),
		   AddressUri=new Uri("http://localhost:5001/HelloService")
		   , Name="HelloServiceEndpoint"                       
	}
	},
	ConfigurationName="HelloServiceConfig",
	Body = new Sequence
	{
		Variables={x,y,__handle,result},
		Activities =
		{
			receive,
			new SumActivity{
				 X=x,
				 Y=y,
				Result=result
			},
			new SendReply{
				 Request=receive,
				  Content=SendContent.Create(new InArgument<double>(result))
			}
		}
	
```

WorkflowService örneği içerisinde yer alan en önemli özelliklerden birisi Endpoints'dir. Bu özelliğe göre birden fazla Endpoint bildirimi yapılabilmektedir. Örneğimizde BasicHttpBinding tabanlı, IHelloService isimli servis sözleşmesini kullanan ve http://localhost:5001/HelloService adresi üzerinden yayın yapan bir Endpoint bildirimi söz konusudur. WorkflowService örneği oluşturulurken kullanılan ConfigurationName özelliği, konfigurasyon dosyası (Örneğimizde app.config) içerisindeki bir servis bloğunu işaret etmektedir. Aslında bu blokta servisin dış dünyaya Metadata paylaşımını yapacağını bildirebiliriz. Bildiğiniz üzere Metadata Publishing sayesinde istemcilerin WSDL içeriğine ulaşması mümkündür ve bu sayede gerekli Proxy tiplerini kolayca üretebilirler. (Ancak tabiki bazı hallerde özellikle güvenlik sebebi ile Metadata bilgisini istemci tarafına indirilebiliyor olması arzu edilmeyebilir) İşte app.config dosyasının içeriği.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
       <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0,Profile=Client" />       </startup>
  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior name="HelloServiceBehavior">
          <serviceMetadata httpGetEnabled="true" httpGetUrl ="http://localhost:5001/HelloService"/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <services>
      <service name="HelloServiceConfig" behaviorConfiguration="HelloServiceBehavior"/>
    </services>
  </system.serviceModel>
</configuration>
```

WorkflowService örneğinin Body özelliği içerisinde ise sırasıyla Receive, SumActivity ve SendReply aktivite bileşenlerini içeren bir Sequence aktivite bileşeni tanımlandığı görülmektedir. Bu bileşenin Variables özelliğinde ise, tüm alt aktiviteler tarafından kullanılacak olan x,y, resultve handle isimli değişken bildirimleri yer almaktadır. Sequence aktivite bileşeninin Activities özelliğine bakıldığında ise sırasıyla Receive, SumActivity ve SendReply aktivite bileşenlerinin tanımlandığı gözlemlenmektedir. Durun bir dakika SumActivity'mi? Bu bizim tarafımızdan tanımlanmış CodeActivity türevli bir aktivite sınıfıdır ve içeriği aşağıdaki gibidir.

```csharp
public class SumActivity
        : CodeActivity<double>
    {
        public InArgument<double> X { get; set; }
        public InArgument<double> Y { get; set; }

        protected override double Execute(CodeActivityContext context)
        {
            return X.Get(context) + Y.Get(context);
        }
    }
```

SumActivity tahmin edeceğiniz üzere istemciden gelen x ve y değişkenlerini kullanmak üzere tasarlanmıştır. Elbette bu nokada küçük bir soru vardır. Receive aktivite bileşenine istemci tarafından gelen xValue ve yValue değeleri, SumActivity örneğine nasıl aktarılacaktır?

![Wink](/assets/images/2010/smiley-wink.gif)

Dikkat edileceği üzere SumActivity örneklenirken X, Y ve Result özelliklerine sırasıyla x,y ve result değerleri atanmıştır (Büyük küçük harf ayrımına dikkat edelim). Bu değişkenler Sequence seviyesindeki Variable'lardır ve Receive aktivite bileşeni içerisinde Content özelliği içerisindeki bildirim yardımıyla set edilmektedir. Çok doğal olarak Result özelliğine gelen değer, result Variable'ına aktarılmakta ve bu da SendReply aktivite bileşeni tarafından kullanılarak istemciye cevap olarak döndürülmektedir (SendReply bileşeninin Content özelliğine dikkat edelim)

Artık WorkflowService örneğini host etmek üzere gerekli kodları aşağıdaki gibi yazabiliriz.

```csharp
WorkflowServiceHost serviceHost = new WorkflowServiceHost(wfService);
serviceHost.Open();
Console.WriteLine("Servis açık. Kapatmak için bir tuşa basın");
Console.ReadLine();
serviceHost.Close();
```

WorkflowServiceHost nesnesi örneklenirken parametre olarak wfService nesne örneği verilmektedir. WorkflowServiceHost tipi Workflow Service'lerin ayağa kaldırılması, servis olarak sunulması, gerekli çalışma zamanı ortamının kurgulanması gibi işlemleri üstelenen bir tip olarak düşünülmelidir. Buna göre istemci uygulama çalıştırıldığında aşağıdaki sonuçlar ile karşılaşmamız gerekmektedir.

![blg193_Runtime1.gif](/assets/images/2010/blg193_Runtime1.gif)

Peki ya istemci tarafı? Çalışan bu servisin sunduğu Workflow Service örneğinin işe yaradığını nasıl göreceğiz? WcfTestClient uygulaması bu noktada işimizi görüyor olacaktır. Söz konusu uygulamayı Visual Studio Command Prompt üzerinden çalıştırdıktan sonra http://localhost:5001/HelloService için bir talepte bulunmamız yeterlidir. Eğer herşey yolunda giderse Sum operasyonunu test edebilir ve aşağıdakine benzer sonuçları alabiliriz.

![blg193_ClientRuntime.gif](/assets/images/2010/blg193_ClientRuntime.gif)

İşte bu kadar.

![Laughing](/assets/images/2010/smiley-laughing.gif)

Görüldüğü gibi Workflow Service örneğimizi başarılı bir şekilde çağırdık. Peki ya bundan sonrası? Örneğimizi HTTP tabanlı olarak tasaladığımızı fark etmişsiniz. Ancak sizde söz konusu örneği TCP bazlı çalışacak şekilde geliştirmeyi deneyebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WorkflowConsoleApplication1.rar (28,99 kb)](/assets/files/2010/WorkflowConsoleApplication1.rar)
