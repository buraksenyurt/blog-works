---
title: "AWS Lambda Üzerinde .Net Core Koşturmak"
pubDate: 2018-01-11 21:30:00
categories:
  - .Net Core
  - AWS
tags:
  - .Net Core
  - AWS
  - Bash
  - C#
  - XML
  - .NET
  - ASP.NET
  - ASP.NET Core
  - JSON
  - Web API
  - Web Service
  - HTTP
  - IIS
  - Docker
  - Python
  - Ruby
  - Node.js
  - React
  - Serialization
  - Generics
  - Microservices
  - Visual Studio
---

# AWS Lambda Üzerinde .Net Core Koşturmak
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Çok yeni bir dünyanın içerisindeyiz uzun zamandır. Cloud Computing ile başladı. C#'ın Linux, MacOSX üzerinde çalıştığına şahit olurken, React Native'ın dünyayı sarsan yükselişine tanık olduk. Oysa ki daha bir süre öncesine kadar Scrum metodolojilerine alışmaya çalışıyor, TFS'e nasıl plug-in yazarıza bakıyorduk. Üniversite yıllarımızda internet bağlantısı bile olmayan bilgisayarlarda yazdığımız faktöryel hesaplama fonksiyonlarını, doğrudan sahibi olmadığımız Quantum bilgisayarlara yaptırmak için Serverless sistemlerden yararlanabileceğimiz bir dünya söz konusu artık. Şirketin hantallaşan iş alanlarını birer microservice haline getirip docker üzerinden host ettiğimiz gezegenler var artık. Çok hızlı dediğimiz Apache sunucularının yerini alan NGinx'e bakarken IIS'i unutup gidiyoruz belki de. Gelişiyoruz, değişiyoruz...Ve bu ikisini sürekli yapıyoruz. Adapte olmak zorundayız.

Konumuz Serverless, AWS Lambda ve.Net Core.

Günümüz bulut sistemleri göz önüne alındığında Microsoft Azure, Amazon Web Services ve Google Cloud Platform ilk aklımıza gelen ürünler oluyorlar sanıyorum ki. Neredeyse tamamının serverless olarak bildiğimiz yetenekleri de bulunuyor ki son yılın belki de en moda kavramlarından birisi bu. Kısaca Backend As A Services (BaaS) ya da Function As A Services (FaaS) şeklinde anılan Serverless teknolojisini halen daha anlamaya çalışıyorum. İşin aslı bu yeni yaklaşımdaki amaç, elimizdeki iş fonksiyonelliklerini sunucuların bakım, ölçekleme gibi ihtiyaçlarını düşünmeden sürekli dağıtım çarkının içerisine kolayca dahil edebilmek, hizmette oldukları süre boyunca parasını ödemek, böylece maliyetlerimizi mümkün olduğunca azaltmak.

Araştırmalarım şiddetini arttırınca gecenin bu saatlerinde internetten okuduğum bir yazıdaki şekli de aşağıdaki gibi renklendirmeye çalıştım. Şekil bir Web API uygulamasını AWS üzerine aldığımızda bilinen kullanımıyla yeni yaklaşım arasında nasıl bir fark oluştuğunu betimlemekte. Kullanıcı talepleri önce Amazon Web Service üzerindeki API Gateway'e geliyor. Talepler, Lambda üzerinden geçerek.Net Core'un çalışma zamanına inip oradan ilgili fonksiyonun işletilmesi, üretilen cevabın da geriye döndürülmesi işlemleri gerçekleşiyor. Özetle tanıdık olduğum senaryolardaki IIS ve NGinX gibi talebi karşılayan uygulamaların yerini AWS alıyor diyebiliriz.

![image.axd](images/image.axd)

AWS Lambda uzun zamandır ilgimi çeken bir üründü. Nitekim çok geniş bir uygulama desteği bulunuyor. Node Js, Python, Go, Ruby ve tabii.Net Core (Tek üzücü olan şey konuya hazırlandığım 2017 Aralık ayı itibariyle AWS'nin.Net Core 1.0.4 SDK'sını desteklemesiydi) Önümüzdeki aylar için yapılacaklar listeme eklediğim ödevlerden birisi de.Net Core ile yazdığım bir uygulamayı Lambda üzerinden yayınlamaktı.

Temelde AWS üzerinde 3 modelde uygulama geliştirebiliriz. Plain Lambda Function, Serverless Application ve Asp.Net Core App as Serverless Application (Çizim buna ait) Plain Lambda Function modeli ile bir C# sınıfının fonksiyonlarını AWS üzerinden tetiklenebilir hale getiriyoruz. İkinci modelde, yazılan uygulamanın API Gateway arkasında çalışacak şekilde CloudFormation (YAML veya JSON formatında belli bir şablon kuralına göre taşımaya ait bilgileri yazdığımız dil olarak düşünülebilir) kullanılarak taşınması söz konusu. Son modelde ise Asp.Net Core uygulamasının tek bir Lambda fonksiyonu haline getirilerek AWS üzerine taşınması söz konusu. Bu durumda API Gateway bir Proxy görevini üstleniyor diyebiliriz. Benim bu yazıdaki amacım ise bir Handler sınıfı ve içerisindeki fonksiyonları Lambda üzerine taşımak.

> Serverless yaklaşımı ile ilgili olarak iki yakın arkadaşımın harika birer yazısı var. Deniz İrgin'in "Sunucusuz Bir Dünya Mümkün mü?" yazısına [bu adresten](https://medium.com/codefiction/serverless-architecture-sunucusuz-bir-d%C3%BCnya-m%C3%BCmk%C3%BCn-m%C3%BC-f7abab6ea0c8), Arda Çetinkaya'nın da "Nedir bu Serverless?" isimli makalesine [şu adresten](http://www.minepla.net/2016/09/nedir-bu-serverless/) ulaşabilirsiniz.

Pek tabii her şeyin başında AWS'de bir hesap açmak gerekiyor. Bu hesap açma işlemi sırasında sizden bir de kredi kartı istenecek (Ne yazık ki) Free Plan'ı seçmeniz benimki gibi demo uygulamalarınız için yeterli olacaktır. Kritik noktalardan birisi AWS hesabını açtıktan sonra Identity and Access Management kısmından bir kullanıcı oluşturmamızın gerekliliği. Bu kullanıcıyı çeşitli yetkiler ile donattıktan sonra (Genelde Administrator olarak) bir Application ID ve Secret Key değeri üretilecek. Bu değeleri serverless (biraz sonra değineceğim) çatısına aşağıdaki komutlar ile bildirmek gerekiyor ki taşıma (Deployment) işlemleri sırasında AWS tarafındaki authenticate adımları başarılı sonuçlansın.

```bash
serverless config credentials --provider aws --key Key_Değeri_Gelecek --secret Secret_Key_Değeri_Gelecek
```

[Serverless](https://serverless.com/framework/), Node.js ile yazılmış olan bir CLI (Command Language Interface) aracı. Bu aracı kullanarak AWS Lambda tarafında uygulama geliştirme ve taşıma gibi çeşitli operasyonları gerçekleştirebiliriz. Bu benim için oldukça ideal bir yaklaşım. Nitekim West-World üzerinde Visual Studio bulunmuyor (Bu sene sadece Visual Studio Code kullanacağım) Dolayısıyla AWS Lambda uygulamaları için gerekli hazır proje şablonlarını yükleyebileceğim bir ortam yok. Ama CLI üzerinden serverless çatısını kullanarak gerekli geliştirmeleri pekala yapabilirim. İlk etapta Nodejs'in son sürümünün yüklü olması gerektiğini söylemeliyim. Bu yüzden aşağıdaki komutlar ile Nodejs'i sisteme yüklemek gerekiyor.

```bash
curl -sL https://deb.nodesource.com/setup_6.x | sudo -E bash -
sudo apt-get install -y nodejs
```

Yüklemenin ardından aşağıdaki komut ile serverless çatısını sisteme kurabiliriz.

```bash
sudo npm install serverless -g
```

Eğer her şey yolunda giderse aşağıdaki komutların sonucu olarak versiyon numarasını ve create komutu ile kullanabileceğimiz şablonları görebilmemiz gerekir.

```bash
serverless --version
serverless create --help
```

![image.axd](images/image.axd)

Serverless çatısının başarılı bir şekilde yüklenmesinin ardından ben, hellolambda isimli bir klasör oluşturdum ve sonrasında içerisinde aşağıdaki komutu çalıştırarak Lambda dünyasına "Hello World" dedim.

```bash
serverless create --template aws-csharp --name bssdemo
```

![image.axd](images/image.axd)

Ekran görütüsünden de görüleceği üzere C# için bir şablon otomatik olarak oluşturuldu. Buradaki Handler.cs, serverless.yml gibi içerikler hiç bozulmadan AWS'deki hesabımız ile ilişkilendirilip kullanılabilirler. Handler sınıfı içerisinde Hello isimli bir metod yer almakta. Bu metod Lambda tarafındaki fonksiyon olarak da düşünülebilir. Kabaca API Gateway'e gelecek olan talep sonrası işletilecek fonksiyon olduğunu söyleyebiliriz. Peki "Hello" isimli fonksiyonu sistem nereden bilecek? İşte serverless.yml içerisindeki aşağıdaki kısım burada devreye giriyor.

```csharp
functions:
  hello:
    handler: CsharpHandlers::AwsDotnetCsharp.Handler::Hello
```

Tahmin edeceğiniz üzere Handler sınıfının kendisinin de içerisindeki fonksiyonun da adını değiştirebilir hatta n sayıda fonksiyonu sisteme dahil edebiliriz. Birden fazla handler'da burada söz konusu olabilir. İşin sırrı YAML içeriğinde gizli (Biraz sonra değiştireceğiz)

Tam build işlemlerine başlamıştım ki bir hatayla karşılaştım. global.json dosyasındaki SDK versiyonu 1.0.4ü gösteriyordu. West-World ise.Net Core'un 2.0 versiyonunu kullanıyordu. Dolayısıyla AWS'nin şu an desteklediği.Net Core 1.0.4 SDK'sı makinede yüklü değildi. Hemen bu versiyonu yüklemeye karar verdim. Ama endişelerim de vardı. Ya 2.0.0 ortamı bozulursa?!

```bash
sudo apt-get install dotnet-dev-1.0.4
```

Bu komutla sisteme.Net Core 1.0.4 ortamını da yüklemiş oldum. Sonrasında paketi yüklemeyi tekrar denedim. Bulunduğum klasör itibariyle dotnet aracı.Net Core 1.0.4'ı dikkate almaya başlamıştı (Bu arada build ve restore işlemlerinde.Net 1.0'ın 2.0'a göre acayip yavaş olduğunu belirtmek isterim)

Derken ilk deploy işlemi sırasında "ServerlessError: The security token included in the request is invalid." şeklinde bir hata aldım. Sorun AWS hesabıma ait Credential bilgilerinin makinem için ayarlanmamasından kaynaklanıyormuş (Thanks a lot Stackoverflow) Bunun üzerine önce aşağıdaki komutlarla gerekli kayıt işlemlerini gerçekleştirdim (Siyah ile boyalı kısımları söylemicim)

```bash
export AWS_ACCESS_KEY_ID=application_id gelecek
export AWS_SECRET_ACCESS_KEY=Secret_key gelcek
serverless deploy
```

![image.axd](images/image.axd)

Sonrasında tekrardan build ve deploy işlemlerini gerçekleştirdim.

```bash
sh build.sh
serverless deploy -v
```

![image.axd](images/image.axd)

Bir yerlere varıyor gibiydim. Küçük bir deneme yaptım. serverless çatısının invoke fonksiyonunu kullanarak şablon ile birlikte gelen hello operasyonunu çağırmayı denedim. Aşağıdaki gibi.

```bash
serverless invoke -f hello -l
```

![image.axd](images/image.axd)

Sanki uygulama Lambda üzerinden çalıştırılmış gibiydi. Bu tabii standart şablon uygulaması. Değiştirmekte yarar var. Öncelikle HTTP taleplerine cevap vermesi için Amazon.Lambda. APIGatewayeEvents paketinin çözüme dahil edilmesi gerekiyor. Aşağıdaki komut ile bu işlemi yapabiliriz.

```bash
dotnet add package Amazon.Lambda.APIGatewayEvents -v:"1.1.0"
```

Sonrasında Handler.cs dosyasının hem adını hem de içeriğini aşağıdaki gibi değiştirdim.

```csharp
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Net;
using System.Collections.Generic;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace AwsDotnetCsharp
{
    public class SampleHandler
    {
        public APIGatewayProxyResponse GetGreetingsMessage(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "{ \"Motto \": \"Merhaba ahbap. Nasıl gidiyor bakalım? :)\" }",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        public APIGatewayProxyResponse GetLocalWeatherCondition(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "{ \"Weather \": \"Sıcaklık 29 Derece. Hava güneşli\" }",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }
    }
}
```

SampleHandler sınıfı içerisinde iki fonksiyon yer almakta. Yaptıkları çok önemsiz. Ancak parametreleri ve dönüş tiplerine dikkat etmek lazım. Her ikisi de HTTP Get taleplerine cevap verip JSON formatında içerik döndürmekteler. Tabii eklenen bu iki fonksiyon için dağıtım kanalına da bilgi vermemiz gerekiyor. Bunun için de yml içeriğini aşağıdaki gibi düzenledim (service: bssdemo aslında benim AWS üzerinden oluşturduğum bucket'a verdiğim ad. Geliştireceğimiz.Net uygulamasını bu hizmetin olduğu yerde konuşlandıracağız)

```xml
service: bssdemo

provider:
  name: aws
  runtime: dotnetcore1.0

package:
  artifact: bin/release/netcoreapp1.0/deploy-package.zip

functions:
  greetings:
    handler: CsharpHandlers::AwsDotnetCsharp.SampleHandler::GetGreetingsMessage

    events:
      - http:
          path: greetings/hi
          method: get

  utility:
    handler: CsharpHandlers::AwsDotnetCsharp.SampleHandler::GetLocalWeatherCondition

    events:
      - http:
          path: utility/weather
          method: get
```

(yml içeriğindeki girintiler oldukça önemlidir. Ben weather için yazdığım path ifadesini http ile aynı hizaya koyduğumda, serverless aracı ilgili path sekmesini bulamadığını söyledi)

sevice, provider, package ve functions. AWS özellikle bu kısımlara bakacak. provider tarafında aws ve.Net Core 1.0 kullanılacağı belirtiliyor. package tarafında build işlemi sonrası oluşan artifact işaret edilmekte. functions kısmında hangi operasyonları sunacaksak onlara ait bilgiler bulunuyor. handler'lar tip ve metod adlarını ifade ederken http sonrası gelen bilgiler de URL'in şekillenmesi için. get bildirimi tahmin edeceğiniz üzere talebin HTTP Get olacağını belirtmekte.

Kodun düzenlenmesini tamamladıktan sonra tekrardan build ve deploy işlemlerini gerçekleştirmek lazım. Aslında n tane fonksiyona yeni bir fonksiyon eklediysek sadece bu fonksiyonu deploy edebiliriz de (Nasıl olabilir bir araştırın bakalım)

![image.axd](images/image.axd)

Artık API Gateway'in tetikleyeceği iki Lambda fonksiyonu söz konusu. Doğruyu söylemek gerekirse Postman aracı ile denemeleri yaptığımda gördüğüm sonuçlar beni mutlu etti.

İlk önce greetings/hi adresine HTTP Get talebi gönderdim.

![image.axd](images/image.axd)

Ardından utility/weather için bir talep daha.

![image.axd](images/image.axd)

Sonuçlar tatmin ediciydi. Daha önceden yazdığımız bir çok Web API'yi buraya entegre edebiliriz diye düşünüyorum. Pek tabii fonksiyonlar tamamen deneme amaçlı geliştirilmiş durumdalar. Siz içeriklerini istediğiniz gibi genişletebilirsiniz. AWS dünyası oldukça kapsamlı. Henüz MSDN rahatlığını bulabilmiş değilim dökümantasyonlarında. Bazen kayboluyorum ama ilgi çekici olduğunu da ifade edebilirim. Kavramlar yeni yeni oturmaya başladılar. [Codefiction](http://www.codefiction.tech/) gibi web sitenizi AWS üzerine alabilirisiniz ya da şirketinizin elektronik ticaret alt yapısına ait fonksiyonellikleri burada barındırabilirsiniz. Bunları bir düşünün:) Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
