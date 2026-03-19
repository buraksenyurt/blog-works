---
title: "NLog için Oracle Database Kullanmak"
pubDate: 2014-07-17 07:45:00
categories:
  - C#
tags:
  - C#
  - XML
  - .NET
  - ASP.NET
  - SQL Server
  - Oracle
  - HTTP
  - Threading
  - Transactions
---

# NLog için Oracle Database Kullanmak
[![marlin-dory](images/marlin-dory_thumb.jpg)](images/marlin-dory.jpg) Merhaba Arkadaşlar,

Animasyon film meraklısı olupta [Pixar’ ın 2003 yapımı Nemo’ sunu](http://www.imdb.com/title/tt0266543/) izlemeyen sanırım yoktur. Kayıp oğlu Nemo’ yu bulmak isteyen Marlin, uzun ve zorlu yolculuğu sırasında oldukça unutkan olan ve aslında bu özelliği ile balık olduğunu adeta tüm izleyenlere ispat eden Dory ile seyahat etmektedir. Dory neredeyse bir saniye önce söylediğini hatırlamakta zorlanan bir balıktır.

Aslında geliştirmekte olduğumuz uygulamaların da buna benzer handikapları vardır. Bir şeyler hatırlamak zorundadırlar ve bu yüzden çeşitli depolama ortamlarını kullanırlar. Bu depolar ile olan iletişimlerinde çeşitli stratejiler uygularlar. Özellikle web tabanlı uygulamalar söz konusu olduğunda istemciler ile olan iletişimde de unutkanlık halleri baş gösterir. Web, doğası gereği çoğu zaman State tutmakta zorlanan bir ortamdır.

Unutkanlığın baş gösterdiği bir diğer yer ise uygulamalar üzerindeki işlem hareketliliklerdir. Öyleki genel performansın gözlemlenmesinde, erken uyarı sistemlerinin oluşturulmasında, hataların kodun içerisine girilmeden analiz edilmesinde, müşterilerin geçmişe yönelik hareketlerinin kayıt altına alınmasında ve benzer bazı senaryolarda uygulamanın o anki hafızasının kayıt altına alınması gerekebilir. Ne yazık ki uygulamalar bunu otomatik olarak yapmazlar. Bunu gerçekleştirebilmeleri için bir Log mekanizmasına sahip olmaları ve unutkanlıklarını kayıt altına alabilmeleri gerekir. Tabi ki geliştiricilerin yardımıyla.

Daha önceden [Log4Net](https://www.buraksenyurt.com/post/Log4Nete28099-i-Tanc4b1yalc4b1m) aracını incelemiş ve pek çok projede kullanmıştım ama hayat bizi farklı kaynaklarla çalışmaya da itebiliyor. Öyle ki yakın zamanda popüler loglama araçlarından olan NLog kütüphanesini kullanma fırsatı buldum. Ve bu sefer gerek kayıt altına alınacak bilgiler gerekse logun yazılacağı ortam biraz farklıydı. Log’ ların veritabanına, kurumun Audit mekanizmasına uygun kurallar dahilinde yazılması zorunluydu. Yıllarca alışkın olduğum SQL Server yerine bu kez karşımda Oracle vardı. Ve sonuçta bir vaka çalışması ortaya çıktı. Haydi gelin senaryomuz ile makalemize başlayalım.

Senaryo

Log içeriklerinin nereye yazılacaği önemlidir. Pek çok kurumsal projede veri kaynağı olarak SQL Server veya Oracle gibi RDBMS (Relational Database Management System) hizmetleri kullanılır. Bu çaptaki projelerde veri, hacimsel olarak büyüktür ve transaction sayıları oldukça yüksektir. Dolayısıyla sistem üzerindeki hareketliliklerin izlenmesi noktasında log bilgilerinin bir text dosyasına yazılması tercih edilmez. Bunun yerine söz konusu log içeriklerinin veri tabanı üzerinde konuşlandırılması gerekir. Nitekim oluşan log bilgileri gelecek zamanlarda hayati önem taşıyacağından kolayca sorgulanabilir olmalıdır.

> Özellikle SOA dünyasında, servislerin birbirleriyle ve diğer farklı uygulamalarla çokça konuştukları ortamlarda, Log bilgilerinden yararlanılarak sistemin genel performansı ölçümlenebilir ve hatta önceden uyarı verecek alarm sistemleri tasarlanabilir. Söz gelimi bir servisin giderek daha yavaş cevap vermesi log’a yazılan bilgilere bakılarak önceden tahmin edilebilir. Dolayısıyla Log diyip geçmemek gerekir.

İşte bu felsefeden yola çıkarak örnek senaryomuzda basit bir Asp.Net Web Application üzerindeki hareketlilikleri, Oracle veritabanında oluşturulan bir tablo içerisine NLog paketi yardımıyla atmaya çalışacağız. Öncelikle web uygulamasını oluşturup gerekli kütüphane ve konfigurasyon içeriklerini dahil ederek işe başlamakta yarar var.

Ön Hazırlıklar

Senaryoya göre NLog ile ilişkili kütüphanelerin ve gerekli konfigurasyon dosyalarının web uygulamasına referans edilmesi gerekmektedir. NLog'un Oracle veri tabanı yönünde log atabilmesi için bir Provider bildiriminin yapılması gerekmektedir. Özetle web uygulamasının referansları ve kullanacağı konfigurasyon içerikleri aşağıdaki gibidir.

- NLog.dll (Çekirdek log kütüphanemiz)
- NLog.config (Çekirdek log mekanizmasına ait konfigurasyon ayarlarını barındıran dosyadır. Copy to output Directory özelliğinin Copy If Newer yapılmasında fayda bulunmaktadır)
- NLog.xsd (NLog.config dosyası için gerekli olan XML şema içeriğidir. Özellikle NLog.config içerisine yazılan XML içeriğinin denetlenmesinde rol almaktadır)
- NLog.Extended.dll (${aspnet-sessionid} gibi genişletilmiş Layout'ların kullanılmasını sağlamaktadır)

NLog ve NLog.Extended kütüphaneleri, NuGet paket yönetim aracı üzerinden uygulamaya kolayca yüklenebilirler.

[![lto_1](images/lto_1_thumb.png)](images/lto_1.png) [![lto_2](images/lto_2_thumb.png)](images/lto_2.png)

Oracle Database Hazırlıkları

Oracle tarafında aşağıdaki şema yapısına sahip bir tablo ve trigger kullanılabilir.

```text
CREATE TABLE ApplicationLogs 
    (TraceTime                  TIMESTAMP (7), 
    LogLevel                    VARCHAR2(50 BYTE), 
    Logger                      VARCHAR2(250 BYTE), 
    Message                     VARCHAR2(1000 BYTE), 
    MachineName                 VARCHAR2(20 BYTE), 
    UserName                    VARCHAR2(30 BYTE), 
    CallSite                    VARCHAR2(1000 BYTE), 
    ThreadId                    VARCHAR2(10 BYTE), 
    ExceptionMessage            VARCHAR2(1000 BYTE), 
    StackTrace                  VARCHAR2(1000 BYTE), 
    SessionID                   VARCHAR2(50 BYTE) 
    );

CREATE OR REPLACE TRIGGER ApplicationLogsTrigger 
BEFORE 
  INSERT 
ON ApplicationLogs 
REFERENCING NEW AS NEW OLD AS OLD 
FOR EACH ROW 
BEGIN 
    :new.TraceTime:= SYSTIMESTAMP; 
END;
```

ApplicationLogs tablosu içerisinde log için kullanılabilecek bir kaç alan bulunmaktadır. Message alanının içeriği tahmin edileceği üzere kod tarafında geliştirici tarafından üretilmektedir. Diğer yandan temel bir kaç çevre değişkeni de NLog mekanizması tarafından doldurulmaktadır. Örneğin Info, Warn, Exception gibi log seviyeleri için LogLevel alanı, Web uygulamasında oturum açan kullanıcıyı benzersiz şekilde tanımlayan Session bilgisi için SessionID alanı, bir çalışma zamanı istisnası oluşması halinde ele alınabilecek ExceptionMessage alanı vb. ApplicationLogsTrigger isimli Trigger'ın temel amacı ise log'un oluşturulma zamanının TraceTime alanına yazılmasını sağlamaktır. Burada SysTimeStamp fonksiyonundan yararlanılmıştır.

> Özellikle Web uygulamalarının log’lanmasında güncel oturum bilgisi önemlidir. Nitekim SessionID, log yığını içerisinde oturumun benzersiz bir tanımlayıcısı şeklinde ele alınabilir ki bu sayede her oturumun log hikayesinin farklılaştırılması mümkün hale gelir.

NLog.Config İçeriği

Gelelim NLog konfigurasyon ayarlarına. Dosya içeriğinin aşağıdaki gibi oluşturulması gerekmektedir.

```xml
<?xml version="1.0" encoding="utf-8" ?> 
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" 
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
      internalLogLevel="Debug"      
       internalLogFile="c:\InteralLogs.txt" 
      > 
  <extensions> 
    <add assembly="NLog.Extended" /> 
  </extensions> 
  <targets>    
    <target name="database" xsi:type="Database" keepConnection="false" useTransactions="true" 
          connectionStringName="ConStr" 
          commandText="INSERT INTO APPLICATIONLOGS (LOGLEVEL,LOGGER,MESSAGE,MACHINENAME,USERNAME,CALLSITE, THREADID,EXCEPTIONMESSAGE,STACKTRACE,SESSIONID) 
              VALUES (:pLEVEL,:pLOGGER,:pMESSAGE,:pMACHINENAME,:pUSERNAME, :pCALLSITE,:pTHREADID,:pEXCEPTIONMESSAGE,:pSTACKTRACE,:pSESSIONID)"> 
      <parameter name="pLEVEL" layout="${level}"/> 
      <parameter name="pLOGGER" layout="${logger}"/> 
      <parameter name="pMESSAGE" layout="${message}"/> 
      <parameter name="pMACHINENAME" layout="${machinename}"/> 
      <parameter name="pUSERNAME" layout="${windows-identity:domain=true}"/> 
      <parameter name="pCALLSITE" layout="${callsite:filename=true}"/> 
      <parameter name="pTHREADID" layout="${threadid}"/> 
      <parameter name="pEXCEPTIONMESSAGE" layout="${exception}"/> 
      <parameter name="pSTACKTRACE" layout="${stacktrace}"/> 
      <parameter name="pSESSIONID" layout="${aspnet-sessionid}"/> 
    </target> 
  </targets> 
  <rules> 
    <logger name="*" minlevel="Trace" writeTo="database" /> 
  </rules> 
</nlog>
```

Konfigurasyon içeriğinde dikkat edilmesi gereken bir kaç nokta bulunmaktadır. Bunları aşağıdaki maddeler ile özetleyebiliriz.

- Her ihtimale karşı sistemi ayağa kaldırırken NLog bazı sorunlar yaşayabilir ve bu sebepten beklediğimiz şekilde log atmayabilir. Örneğin Oracle tarafından gelecek bir hata sonrası (geçersiz bir kolon adı kullanılması, eksik parametre yazılması vb) NLog kendi içinde exception'lar verebilir. Bu yüzden internalLogLevel ve internalLogFile isimli nitelikler belirlenmiştir. Sistemin çalışırlığından emin olduktan sonra bu değerler kaldırılabilir.
- extensions elementi altında yapılan bildirim ile NLog.Extended kütüphanesi içerisindeki tiplerin kullanılabilmesi sağlanır. Örnekte Asp.Net SessionID değerinin alınması sırasında kullanılan layout için bu bildirimin yapılması gerekmektedir.
- Elbette en can alıcı nokta target kısmıdır. Burada, hangi bağlantı üzerinden veri tabanına gildileceği ve çalıştırılacak olan SQL ifadesi belirtilmektedir.(Database Target’ da kullanılabilecek alternatiler için [şu adrese bakabilirsiniz](http://nlog-project.org/documentation/v2.0.1/html/T_NLog_Targets_DatabaseTarget.htm))
- connectionStringName özelliğine atanan değer, web.config dosyası içerisindeki connectionString elementini işaret etmektedir. Uygulamada kullanılan web.config içeriği aşağıdaki gibidir.

Dikkat edilmesi gereken en önemli ayrıntı providerName bildirimidir. Bu bildirim yapılmadığı takdirde log atma işlemi gerçekleşmeyecektir. Durumu daha net kavramak adına providerName niteliğini kaldırıp InternalLog dosyasında oluşan içeriğe bakılabilir. Bu durumda aşağıdaki ekran görüntüsünde yer aldığı gibi Provider üretimi sırasında bir exception fırlatıldığı fark edilecektir. Bunun doğal sonucu uygulamanın log atamayacak olmasıdır.
[![lto_4](images/lto_4_thumb.png)](images/lto_4.png)
- commandText niteliğine atanan ifade de Insert parametreleri kullanılmıştır. Parametre bildirimleri için parameter elementinden yararlanılmaktadır. Her elementin layout özelliğinde $ ile başlayan birer NLog anahtar kelimesi ele alınmaktadır.
- loglama mekanizması için hedef hizmet rules elementi içerisinde bildirilmektedir. Burada writeTo niteliğine atanan değerin target elementinde bir karşılığı vardır.

> Örnekte connectionString bilgisinin okunabilir şekilde yazıldığı görülmektedir. Pek tabi kurumsal çaptaki bir proje de bu bilginin şifrelenmiş şekilde durması tercih edilir. Nitekim ürünün dahil olduğu Audit mekanizmaları böyle bir kullanıma müsade edemez.
> Peki şifrelenmiş bir bağlantıyı NLog mekanizması nasıl kullanabilir? Eğer özel bir şifreleme algoritması ile oluşturulmuş bir içerik söz konusu ise, Decryption işini üstlenen kütüphane NLog çalışma zamanına nasıl bildirilebilir? İşte size güzel bir araştırma konusu.
> Başlangıç olarak, Logger tipinin üzerinden veritabanına ilişkin bağlantı bilgisine kod bazında ulaşılabildiğini belirtelim. Dolayısıyla config içerisinde Encrypt edilmiş bir bağlantı bilgisi, Logger tipinin hazırlanacağı bir fonksiyonellikte Decrypt edilip atanabilir. Söz gelimi aşağıdaki kod parçası bu anlamda size bir ip ucu verebilir.
> Logger logger = LogManager.GetCurrentClassLogger ();
> var db = (DatabaseTarget) logger.Factory.Configuration.AllTargets.Where (t => t.Name == "database").FirstOrDefault ();
> db.ConnectionString = ConnectionInitializer.GetConnectionString ();

Gelelim web uygulamasının içeriğine.

Web Uygulamasının Geliştirilmesi

Asp.Net uygulaması içerisinde tamamen senaryonun amacına hizmet eden sembolik bir işleyiş söz konusudur. Default.aspx içeriği bu anlamda aşağıdaki gibi oluşturulabilir.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CookShop.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server"> 
    <title></title> 
</head> 
<body> 
    <form id="form1" runat="server"> 
    <div> 
        <asp:Label ID="lblEmail" runat="server" Text="Email" /> 
        <asp:TextBox ID="txtEmail" runat="server" /> 
        <br /> 
        <asp:Label ID="lblRecipeName" runat="server" Text="Recipe Name" /> 
        <asp:TextBox ID="txtRecipeName" runat="server" /> 
        <br /> 
        <asp:Button ID="btnGetRecipe" runat="server" Text="GetRecipe" OnClick="btnGetRecipe_Click" /> 
    </div> 
    </form> 
</body> 
</html>
```

Kod içeriği ise aşağıdaki gibi yazılabilir.

```csharp
using System; 
using System.Diagnostics; 
using System.Threading; 
using NLog;

namespace CookShop 
{ 
    public partial class Default 
: System.Web.UI.Page 
    { 
        Logger nemo = LogManager.GetCurrentClassLogger(); 

        protected void btnGetRecipe_Click(object sender, EventArgs e) 
        { 
            try 
            { 
                nemo.Info("Yemek reçetisi tedarik süreci başladı"); 
                if (ValidateRecipe(txtRecipeName.Text)) 
                { 
                    FindRecipe(txtRecipeName.Text); 
                    SendRecipeToUser(txtEmail.Text); 
                }

                // Sembolik olarak bir Exception fırlatıldı 
                throw new OutOfMemoryException(); 
            } 
            catch (Exception excp) 
            { 
               nemo.ErrorException("Reçetenin elde edilmesi sürecinde hata oluştu.", excp);       
            }

            nemo.Info("İşlemler tamamlandı"); 
        }

        private void SendRecipeToUser(string EmailAddress) 
        {   
            nemo.Info(string.Format("{0} adresine yemek tarifi gönderilecek." 
                , string.IsNullOrEmpty(EmailAddress)?"sendWithSMS" :EmailAddress)); 
        }

        private void FindRecipe(string RecipeName) 
        { 
            Stopwatch watcher = new Stopwatch(); 
            watcher.Start(); 
            Thread.Sleep(3250); // Sembolik olarak bir gecikme uygulattık 
            watcher.Stop(); 
            nemo.Info(string.Format("{0}:{1} yemek reçetesinin bulunması için geçen toplam süre.",watcher.Elapsed.TotalSeconds,RecipeName)); 
        }

        private bool ValidateRecipe(string RecipeName) 
        { 
            // Sembolik olarak RecipeName' in boş geçilmesini 
            if (string.IsNullOrEmpty(RecipeName)) 
            { 
                nemo.Warn("Yemeğin adı boş geçilmiş"); 
                return false; 
            } 
            else 
            { 
                nemo.Info(string.Format("{0} doğrulandı.", RecipeName)); 
                return true; 
            } 
        } 
    } 
}
```

Log yazma operasyonunu nemo isimli Logger nesne örneği üstlenmektedir. Oluşturulması için LogManager sınıfının static GetCurrentClassLogger metodundan yararlanılmaktadır. Bu, varsayılan olarak NLog.config içerisindeki ayarları göz önüne alacak şekilde log hizmetini ayağa kaldıracaktır. Kodun çeşitli kısımlarında Info, Warn, ErrorException gibi fonksiyon çağrıları yapılarak örnek log bilgilerinin yazdırılması sağlanmaktadır.

Çalışma Zamanı

Uygulamanın çalışmasından ve butona basılarak bir takım işlemlerin icra edilmesinden ziyade Oracle tarafındaki ApplicationLogs içeriğinin dolması daha önemlidir. Eğer konfigurasyon ayarları sorunsuz yapıldıysa, Oracle ile uygulamanın konuşması ve Insert işlemlerini icra etmesi noktasında bir yetki problemi yoksa, çalışma zamanı için aşağıdakine benzer sonuçların elde edilmesi gerekmektedir.

[![lto_3](images/lto_3_thumb.png)](images/lto_3.png)

Sonuçlar

Görüldüğü üzere NLog mekanizmasında loglama yönünü Oracle tarafına çekmek oldukça kolaydır. Dikkat edilmesi gereken noktaların başında, doğru provider sürümünün kullanılması gelir. Şu unutulmamalıdır ki, geliştirici ortamı ile test, preprod, prod ortamları farklılıklar gösterebilir. Yerel makinede seçilen database provider tipi ile sorunsuz çalışılması bir production ortamında geçerli olmayabilir.

Diğer bir husus da, uygulamanın hangi noktasından, hangi seviyede ne çeşit bilgilerin Log olarak yazdırılacağıdır. Log’ un içeriğini belirlemek bu makalenin konusu olmamakla birlikte ileride devasa şekilde büyüyecek veri kümesinin araştırılabilir olarak tasarlanması gerektiği de akıldan çıkartılmamalıdır.

Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
