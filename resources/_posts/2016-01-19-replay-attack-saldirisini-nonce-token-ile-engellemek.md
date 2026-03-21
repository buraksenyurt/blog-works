---
layout: post
title: "Replay Attack Saldırısını Nonce Token ile Engellemek"
date: 2016-01-19 00:00:00 +0300
categories:
  - wcf
tags:
  - windows-communication-foundation
  - ddos
  - security
  - wcf-service-security
  - replay-attack
  - jquery
  - nonce-token
  - rijndael
  - encryption
  - decryption
---
Bir zamanlar WCF servisleri ile ilişkili epeyce çalışma yapmış ve öğrendiklerimi kaleme almaya çalışmıştım. En çok zorlandığım konulardan birisi ise servislerin güvenliğini sağlamaktı. (Mesaj içeriklerinin korunmasından tutun, uç noktalar arası haberleşmenin güvenilik olmasına kadar dikkat edilmesi gereken pek çok nokta var)

![Viking_minion.gif](/assets/images/2016/Viking_minion.gif)

Ne yazık ki internet ortamında sürüsüne bereket saldırı biçimi var. Bunların önüne geçmek için WCF tarafında WS- standartlarına uygun kanal yapıları kullanmak tercih edilen yöntemlerden birisi. Örneğin popüler saldırı çeşitlerinden olan Replay Attack etkisini hafifletmek için Custom Binding'ler kullanılıp, Reliable oturumlar açılması ve iletişimin SSL üzerinden gerçekleştirilmesi uygulanan teknikler arasında. (Şu an okudukça sıkıldığım 2007 menşeeli o uzun makalede [bu konuya](/2007/11/07/wcf-replay-attack-etkisini-hafifletmek/) değinmişim)

Replay Attack vakalarını önlemek için daha basit bir yol da mevcut aslında. Nonce Token adı verilen yöntemde GUID ve Timestamp bilgilerini kullanarak saldırıların önüne geçebiliyoruz. Tabii tek yol bu değil. Secure Shell, IPSec, Random TCP Sequence Number gibi teknikler ile de bu saldırıların önüne geçmek mümkün. Biz bu yazımızda Nonce Token kullanımına bakmaya çalışacağız.

> Replay Attack nedir kısaca hatırlayalım. Saldırganlar, servis ile tüketici arasına girerek, gönderilen paketleri yakalar ve sahibine geri yollar. Bunu sıklıkla tekrar ederler. Bu durumda paketin göndericisi paketi gönderemediğini düşünür ve yeniden göndermeye çalışır. Dolayısıyla paketin göndericisi sürekli meşgul durumda kalarak hizmet veremez hale gelir.

Senaryoda bir web uygulaması ve aynı alan içerisinde konuşlandırılmış bir WCF servisi bulunuyor. Servis tüketicisi ve web uygulaması arasında oluşabilecek Replay Attack vakasını engellemek amacıyla aradaki iletişimde Nonce Token (GUID + Timestamp) kullanacağız. Konuyu basit şekilde anlamak için Visual Studio 2012 şablonlarından ASP.NET Empty Web Application'ı seçerek işe başlayalım. Uygulamda Query.aspx isimli bir web sayfası, QueryService isimli bir WCF servisi ve Token bilgisini şifrelemek/çözümlemek için kullanacağımız yardımcı bir sınıf bulunacak.

## Encryption/Decryption Sınıfı

Servis ve web sayfası arasındaki iletişimde kullanılacak olan Nonce Token içeriğini şifrelemek/çözümlemek için Rijndael algoritmasını ele alan aşağıdaki yardımcı sınıfı yazarak işe başlayalım. Gerçek hayat senaryosunda daha farklı bir şifreleme sistemi de kullanılabilir tabi ama Token bilgisinin mutlaka şifrelenmesi gerekiyor.

```csharp
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NonceTokenSample
{
    public class RijndaelManagedEncryption
    {
        private const string Inputkey = "E18AA2C3-4320-4826-BE4E-07020BB962E2";
        private RijndaelManaged manager;

        public RijndaelManagedEncryption(string salt)
        {
            manager=new RijndaelManaged();
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes(Inputkey, saltBytes);

            manager.Key = key.GetBytes(manager.KeySize / 8);
            manager.IV = key.GetBytes(manager.BlockSize / 8);
        }
        public string Encrypt(string text)
        {
            string encryptedText = string.Empty;
            var encryptor = manager.CreateEncryptor(manager.Key, manager.IV);
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sWriter = new StreamWriter(cStream))
                    {
                        sWriter.Write(text);                        
                    }
                }
                encryptedText = Convert.ToBase64String(mStream.ToArray());
            }
            return encryptedText;
        }

        public string Decrypt(string cipherText)
        {
            string decryptedText=string.Empty;
            var decryptor = manager.CreateDecryptor(manager.Key, manager.IV);
            byte[] cipher = Convert.FromBase64String(cipherText);

            using (MemoryStream mStream = new MemoryStream(cipher))
            {
                using (CryptoStream cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sReader = new StreamReader(cStream))
                    {
                        decryptedText = sReader.ReadToEnd();
                    }
                }
            }
            return decryptedText;
        }
    }
}
```

RijndaelManagedEncryption sınıfı temel olarak şifreleme ve çözümleme işlerini yapmakta. Encrypt ve Decrypt metodları bu amaçla kullanılıyor. Basit olarak metodlara gelen içerikler Base64 dönüşümlerini tamamlamalarını takiben üretiliyor veya çözümleniyor. Yazımızın konusu şifreleme olmadığında bu sınıf üzerinde çok fazla durmaya gerek yok. Gelelim servis tarafına.

## Servis Tarafı

IQueryService isim servis sözleşmesini (Service Contract) aşağıdaki gibi tasarlayabiliriz.

```csharp
using System.ServiceModel;

namespace NonceTokenSample
{
    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        void DoSomething(string token);
    }
}
```

QueryService içeriği

```csharp
using System;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace NonceTokenSample
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class QueryService 
        : IQueryService
    {
        RijndaelManagedEncryption davinci = new RijndaelManagedEncryption("P@ssw0rd");

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public void DoSomething(string token)
        {
            if (IsValidToken(token))
            {
                // Giriş başarılı ise bir şeyler yapacağız
            }
            else
            {
                // Değil ise şüpheli bir durum var. Başka bişi yap.
            }
        }

        private bool IsValidToken(string token)
        {
            string decryptedToken = davinci.Decrypt(token); 
            string guidFromCookie =HttpContext.Current.Request.Cookies["SecureToken"].Value;

            string[] tokenParts = decryptedToken.Split(new char[] { '|' },StringSplitOptions.RemoveEmptyEntries);
            DateTime requestTimestamp = Convert.ToDateTime(tokenParts[1]);

            if (tokenParts[0].Equals(guidFromCookie, StringComparison.OrdinalIgnoreCase)
                  && (DateTime.UtcNow - requestTimestamp).TotalMinutes <= 1)
                return true;

            return false;
        }
    }
}
```

Servisimize jQuery'den yararlanarak bir Ajax çağrısı gerçekleştireceğiz. POST metodunu kullanacağız ve mesaj içeriklerinin JSON (JavaScriptObjectNotation) formatında gidip gelmesini sağlayacağız. Bu yüzden DoSomething metodunda WebInvoke niteliğinden yararlanmaktayız. IsValidToken metodu gelen parametreyi öncelikle deşifre ediyor. Sonrasında ise o anki HttpContext.Current.Request üzerinden yakalanan SecureToken bilgisini alıyor.

> Servis kodunda HttpContext.Current içeriğine erişebilmek için web.config dosyasında bulunan serviceHostingEnvironment elementindeki aspNetCompatibilityEnabled niteliğinin true değerine sahip olması gerekiyor.

Çözümlenen token içeriği bir pipe işaret ile ikiye ayrılmış durumda. İlk parçada GUID bilgisi, ikinci kısımda ise Timestamp bilgisi bulunuyor. Eğer cookie içeriğinde yer alan token bilgisi ile deşifre edilen token bilgisi aynı ise ve talep son 1 dakikalık zamanı dilimi içerisinde gelmişse bir Replay Attack olmadığı sonucuna varıp true döndürüyoruz. Dönen true değerine göre ise program akışı şekilleniyor (ki burada şekillendirmedik)

## Konfigurasyon İçeriği

Uygulamanın web.config içeriğini de aşağıdaki gibi geliştirmemiz gerekiyor.

```xml
<?xml version="1.0"?>

<configuration>
    <system.web>
      <compilation debug="true" targetFramework="4.5" />
      <httpRuntime targetFramework="4.5" />
    </system.web>

  <system.serviceModel>
    <services>
      <service name="NonceTokenSample.QueryService"
        behaviorConfiguration="srvBehavior">
        <endpoint address="" binding="webHttpBinding" behaviorConfiguration="epBehavior" name="webEndPoint" contract="NonceTokenSample.IQueryService"/>
      </service>
    </services>

    <behaviors>
      <serviceBehaviors>
        <behavior name="srvBehavior">
          <serviceMetadata httpGetEnabled="true"/>
          <serviceDebug includeExceptionDetailInFaults="true"/>
        </behavior>
      </serviceBehaviors>

      <endpointBehaviors>
        <behavior name="epBehavior">
          <webHttp />
        </behavior>
      </endpointBehaviors>
    </behaviors>

    <serviceHostingEnvironment multipleSiteBindingsEnabled="true" aspNetCompatibilityEnabled="true"/>
  </system.serviceModel>
</configuration>
```

Burada webHttp davranışının eklenmiş olması önemli. includeExceptionDetailInFaults niteliğinin değeri ise her şey yolunda gittiyse false olarak değiştirilebilir. Servis HTTP Metadata'sı açık şekilde sunuluyor ama bunu canlı ortama geçirdiğimiz vakalarda kapatabiliriz. Bağlayıcı olarak webHttpBinding kullanılmakta.

## Web Sayfası

Servis ile etkileşimde bulunacak olan web sayfası içeriğini ve kod tarafını aşağıdaki gibi geliştirebiliriz.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Query.aspx.cs" Inherits="NonceTokenSample.Query" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js" type="text/javascript"></script>
            <script type="text/javascript">
                $(document).ready(function () {
                    $("#btnCallService").click(function () {
                        $.ajax({
                            type: "POST",
                            url: "QueryService.svc/DoSomething",
                            data: JSON.stringify($('#hdnTokenField').val()),
                            contentType: "application/json; charset=utf-8",
                            success: function () {
                                alert('success');
                            },
                            error: function (result) {
                                // Bir sorun olabilir
                            }
                        });
                    });
                });
            </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
           
            <input type="hidden" id="hdnTokenField" runat="server" />
            <input type="button" id="btnCallService" value="Servisi Çağır" />
        </div>
    </form>
</body>
</html>
```

> Örneğimizde servis ile tüketicisi olan aspx sayfası aynı web uygulama alanı içerisinde yer alıyor. Farklı adreste yer alan bir servis çağrısı söz konusu ise Cross Domain Policy kurallarının uygulanması gerekebilir.

Burada jQuery betiklerinden yararlanarak aynı web uygulaması içerisinde yer alan QueryService isimli servisin DoSomething metoduna bir çağrı gerçekleştirilmektedir. Çağrı, POST metoduna göre yapılmakta olup gönderilecek içerik JSON formatında üretilir. Dikkat edilmesi gereken nokta data kısmında hdnTokenField içeriğinin gönderilmesidir. Bu içerik tahmin edeceğiniz üzere DoSomething metoduna gelen parametredir ve PageLoad içerisinde doldurulmaktadır. Nasıl mı?

```csharp
using System;
using System.Web;

namespace NonceTokenSample
{
    public partial class Query : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateNonceToken();
        }

        private void CreateNonceToken()
        {
            RijndaelManagedEncryption davinci = new RijndaelManagedEncryption("P@ssw0rd");

            string guid = Guid.NewGuid().ToString();
            Response.Cookies.Add(new HttpCookie("SecureToken")
            {
                Value = guid
            });
            string token = String.Format("{0}|{1}",guid, DateTime.UtcNow);
            string encryptedToken = davinci.Encrypt(token);
            hdnTokenField.Value = encryptedToken;
        }
    }
}
```

Dikkat edileceği üzere şifrelenen GUID bilgisi SecureToken ismiyle Cookie olarak yazılır. GUID ile birleştirilen Timestamp içeriği ise (Nonce Token) yine şifrelenerek hdnTokenField isimli Hidden Field içerisine alınır. Dolayısıyla servis çağrısı gerçekleştirildiğinde metod parametresi olarak GUID|Timestamp içeriğinin şifrelenmiş hali yollanır. Karşılaştırma için kullanılacak GUID içeriği ise yine şifrelenmiş halde Cookie üzerinde taşınır. Sonuç olarak servis metodu içerisinde Cookie bilgisinden ve gelen parametreden yararlanarak benzersiz bir ID değeri ve zaman kontrolü ile Replay Attack durumu oluşup oluşmadığı kontrol edilir. Eğer her şey yolundaysa en azından aşağıdaki sonucun alınmış olması gerekir.

![noncetoken_1.gif](/assets/images/2016/noncetoken_1.gif)

Biraz uzun ve yorucu bir makale oldu gibi. Özellikle servisin tesis edilmesi ve jQuery içeriğinin oluşturulması sırasında pek çok problemle karşılaştığımı ifade etmek isterim. Konfigurasyon ayarlarının eksiksiz olması, servisin Web HTTP bazlı geliştirilmesi, jQuery betiğinde # karakterinin unutulmaması vb bir çok kritere dikkat etmek gerekiyor. Kodun çalışma prensibini daha iyi anlayabilmek için mutlaka breakpoint'ler koyarak debug işlemleri gerçekleştirmenizi öneririm. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
