---
title: "Eclipse Üzerinden Java ile TFS Client Object Model Konuşuyor"
pubDate: 2013-05-12 10:10:00
categories:
  - Team Foundation Server
tags:
  - Team Foundation Server
  - C#
  - .NET
  - HTTP
  - Java
  - Threading
  - Visual Studio
---

# Eclipse Üzerinden Java ile TFS Client Object Model Konuşuyor
[![developers](images/developers_thumb.jpg)](images/developers.jpg) Merhaba Arkadaşlar,

Çok değil daha bir kaç sene öncesine kadar (Özellikle.Net’ in duyurulduğu yıllarda ve izleyen bir kaç senede) yazılım dünyasında neredeyse yandaki resimdekine benzer bir kavga vardı (Benzetmeyi biraz abartmış olabilirim)

Java’ cılar, C#’ çıları pek sevmez iken tam tersi durum da pekala geçerliydi. Ben hiç bir zaman birisinin fanatiği olmadım. Hatta Java ile ufak çaplı bir kaç deneyimim bile oldu.

Peki gerçek dünya böyle mi? Özellikle kalabalık yazılım ekiplerinin olduğu, çok fazla sayıda ürünün koştuğu dünyalarda, sadece Java’ cıları, C#’ çıları değil, daha pek çok programlama dili geliştiricilerini bir arada görmekteyiz. Öneğin ben bulunduğum konum itibariyle C’ cilerin, Assembler’ cıların, PowerBuilder’ cıların,.Net’ çilerin, Java’ cıların ve hatta Cobol’ cuların arasında yaşamaktayım.

Hepsi kendi dünyalarını kullanarak ürünler geliştiriyor olsalar da, zaman içerisinde birbirleriyle konuşması gereken uygulamalar bütününün de bir parçası olmaktan kaçamıyorlar. Özellikle işin içerisine bir ALM (Application LifeCycle Management) aracı girdiğinde. İşte bu günkü konumuzda buna itaf edilecek

![Winking smile](images/wlEmoticon-winkingsmile_194.png)

Haydi gelin başlayalım.

Bildiğiniz üzere bir süredir Team Foundation Server’ ın çevre dünya ile olan etkileşimini incelemeye çalışıyorum. Açıkçası TFS’ in gerek servis yapısı gerek Client Object Model gibi kütüphaneleri sayesinde, dış dünya ile olan entegrasyonu son derece kolay. Bu gün buna bir kere daha inandım. Çünkü bir Java uygulaması içerisinde TFS Client Object Model’ i kullanarak, bir Team Project’ in Work Item listesini sorguladım

![Winking smile](images/wlEmoticon-winkingsmile_194.png)

Nasıl yaptığımı merak ediyorsanız okumaya devam edin. Tabi bu işte de çok önemli bir yardımcım vardı. O da Microsoft tarafından geliştirilen ve ücretsiz olarak sunulan Client Object Model SDK’sı. Ama Java için olan sürümü.

> Microsoft Team Foundation Server 2012 Software Development Kit for Java içeriğini [bu adresten](http://www.microsoft.com/en-us/download/details.aspx?id=22616) indirebilirsiniz (Makaleyi hazırladığım tarih itibariyle 15 Şubat 2013’ te yayınlanmış güncel bir sürümü bulunmaktaydı)

Senaryo

Senaryomuz esas itibariyle yine bir Hello World uygulaması olacak.

![Embarrassed smile](images/wlEmoticon-embarrassedsmile_5.png)

Console ekranına belirli bir Team Project içerisinde yer alan Work Item bilgilerini (Product Backlog Item, Bug, Task gibi) yazdırmaya çalışacağız. Örneğin Work Item’ ın başlığını (Title), tipini (WorkItem Type), numarasını (ID) düzgün bir sırada çekmeyi deneyebiliriz. Java kodlaması yapacağımız için Eclipse gibi bir IDE tercih edilebilir ki ben öyle yaptım

![Winking smile](images/wlEmoticon-winkingsmile_194.png)

Bebek Adımları

## Adım 0

İlk olarak Eclipse üzerinde yeni bir Java Projesi oluşturarak işe başlayalım (File->New->Java Project). Ben UsingClientObjectModel olarak adlandırdığım projeyi, sistemimde kurulu olan Workspace içerisinde oluşturdum. Runtime olarak 1.6 sürümünü kullanmayı tercih ediyorum. Bu nedenle daha önceki projelerden varsayılan olarak kalan Use an execution environment JRE değerini olduğu gibi bıraktım.

[![jtfs_1](images/jtfs_1_thumb.png)](images/jtfs_1.png)

Bu üretim işlemi sonrasında Eclipse IDE’ sinde aşağıdakine benzer bir içerik oluştuğunu görmeliyiz. (Elbette sisteminizde Java Runtime Environement’ in yüklü olduğu fiziki adresler daha farklı olabilir.)

[![jtfs_2](images/jtfs_2_thumb.png)](images/jtfs_2.png)

## Adım 1

İkinci adım ise main metodunu içerecek olan ve asıl kodlarımızı yazacağımız sınıfı eklemek olacaktır. Visual Studio tarafından kalma bir alışkanlık nedeniyle Program olarak isimlendirdiğim sınıf, tfs.clientobjectmodel.application isimli paket içerisinde konuşlandırılacak (Siz kendi istediğiniz paket tanımlamasını yapabilirsiniz) Krtik noktalardan birisi de, public static void main (String[] args) seçeneğinin işaretli olmasıdır. Bu, tahmin edeceğiniz üzere programın giriş noktası olan main metodudur.

> C# tarafından da bildiğiniz üzere static Main metodu, exe tipindeki uygulamaların giriş noktasıdır. Aynı durum Java uygulamaları için de geçerlidir. Tek fark Java tarafında isimlendirme standartı gereği küçük m harfi ile başlanmasıdır. Dikkat edileceği üzere her iki dilde metoda parametre olarak string tipinden bir dizi alır. Programa dış ortamdan parametre aktarabilmek amacıyla
>
> ![Winking smile](images/wlEmoticon-winkingsmile_194.png)

[![jtfs_3](images/jtfs_3_thumb.png)](images/jtfs_3.png)

Bu işlemin sonucunda Eclipse IDE’ si bizim için aşağıdaki ekran görüntüsünde yer alan içeriği üretmelidir.

[![jtfs_4](images/jtfs_4_thumb.png)](images/jtfs_4.png)

## Adım 2

Üçüncü adımda, bilgisayarımıza indirip açtığımız TFS SDK’ sını referans ediyor olacağız. Bunun için ilgili JAR dosyasının uygulamaya bildirilmesi (bir başka deyişle referans edilmesi) gerekmektedir. Söz konusu bildirim için proje özelliklerinden Java Build Path kısmına gelmeli ve Add External JARs düğmesini kullanarak com.microsoft.tfs.sdk-11.0.0 isimli JAR dosyasını seçmeliyiz.

[![jtfs_5](images/jtfs_5_thumb.png)](images/jtfs_5.png)

[![jtfs_6](images/jtfs_6_thumb.png)](images/jtfs_6.png)

> Konu ile ilişkili olarak yaptığım araştırmalarda şöyle bir kullanıma da rastladım. Hazırlanan Java uygulamasının dağıtılabileceği de düşünülürse SDK’ nın Redistributable olan parçalarının proje klasörü altına kopyalanması ve ilgili path bildirimlerinin bu fiziki adresleri gösterecek şekilde yapılması yolu da tercih edilebilir. Böyle bir durumda Add JARs düğmesinden hareket edilir.

Sonuç olarak Java Build Path kısmının aşağıdaki ekran görüntüsündeki gibi oluşması gerekmektedir.

[![jtfs_7](images/jtfs_7_thumb.png)](images/jtfs_7.png)

Ne varki bu yeterli değildir. Bir de eklenen JAR dosyası için Native Libary Location değerinin set edilmesi gerekmektedir. Bunun için aşağıdaki ekran görüntüsünde görüldüğü üzere, SDK’ nın açılması sonucu oluşan redist\native klasörüne kadar inilmeli ve platforma uygun olan klasör seçilmelidir. Ben x86 işlemcili win32 tabanlı bir sistem de çalıştığımdan buna uygun olan klasörü seçtim (Native klasörü altında Linux, MacOSX, Solaris gibi pek çok sistem için gerekli kütüphaneler bulunmaktadır)

[![jtfs_8](images/jtfs_8_thumb.png)](images/jtfs_8.png)

Buraya kadar ki işlemlerimiz sonrasında Eclipse IDE’ sinde projemize ait olan son görünüm de aşağıdaki gibi olacaktır. Dikkat edileceği üzere TFS SDK’ sı, Referenced Libraries kısmında görülmektedir.

[![jtfs_9](images/jtfs_9_thumb.png)](images/jtfs_9.png)

Örnek kodlar

Şimdi kod tarafını geliştirmeye başlayabiliriz. Aslında.Net tarafında Client Object Model’ in kullanımından çok da farklı bir işlem yapmamıza gerek yoktur. TFS Client Object Model’ in nesne yapısının hemen hemen aynısı burada da inşa edilmiştir. Şimdi Program sınıfına ait kodlarımızı aşağıdaki gibi geliştirelim.

package tfs.clientobjectmodel.application;

```csharp
import java.net.URI; 
import java.net.URISyntaxException;

import com.microsoft.tfs.core.TFSTeamProjectCollection; 
import com.microsoft.tfs.core.clients.workitem.WorkItem; 
import com.microsoft.tfs.core.clients.workitem.WorkItemClient; 
import com.microsoft.tfs.core.clients.workitem.query.WorkItemCollection; 
import com.microsoft.tfs.core.httpclient.Credentials; 
import com.microsoft.tfs.core.httpclient.HttpException;

public class Program {

    public static void main(String[] args) 
            throws HttpException, URISyntaxException { 
        
        System.setProperty("com.microsoft.tfs.jni.native.base-directory" 
               , "C:\\Program Files\\TFS-SDK-11.0.0.1302\\TFS-SDK-11.0.0\\redist\\native"); 
        
        URI uri=new URI("http://tfsserver:8080/tfs/defaultcollection"); 
        Credentials user=new com.microsoft.tfs.core.httpclient.DefaultNTCredentials(); 
        
        TFSTeamProjectCollection collection=new TFSTeamProjectCollection(uri,user); 
            
        WorkItemClient wiClient=collection.getWorkItemClient(); 
                
        WorkItemCollection workItems=wiClient 
                .query("Select ID,Title from WorkItems " + 
                        "where ([Team Project]='ARGE') order by [Work Item Type]"); 
        System.out.println("Active statusunde olan toplam "+ 
                workItems.size()+" work item bulunmustur");        
        
        for(int i=0;i<workItems.size();i++) 
        { 
            WorkItem workItem=workItems.getWorkItem(i); 
            System.out.println(workItem.getTitle() 
                    +" "+workItem.getType().getName() 
                    +" "+workItem.getID() 
                    +" "+workItem.getProject().getName() 
                    ); 
        } 
    } 
}
```

İlk dikkat edilmesi gereken nokta System özelliklerine bir key-value eklenmiş olmasıdır. com.microsoft.tfs.jni.native.base-directory isimli key için C:\\Program Files\\TFS-SDK-11.0.0.1302\\TFS-SDK-11.0.0\\redist\\native adresi value olarak verilmiştir. Normal şartlarda bu bildirim komut satırından java.exe uygulaması kullanılarak da yapılabilir. Söz konusu sistem özelliğinin bir kere set edilmesi yeterlidir. Bu bildirim ile aslında native loader’ ın sisteme tanıtılması işlemi gerçekleştirilir. Eğer ilgili bildirim yapılmazsa, çalışma zamanında aşağıdaki hata mesajı ile karşılaşılması muhtemeldir.

```text
Exception in thread "main" java.lang.UnsatisfiedLinkError: com.microsoft.tfs.jni.internal.platformmisc.NativePlatformMisc.nativeGetEnvironmentVariable (Ljava/lang/String;)Ljava/lang/String; 
    at com.microsoft.tfs.jni.internal.platformmisc.NativePlatformMisc.nativeGetEnvironmentVariable (Native Method) 
    at com.microsoft.tfs.jni.internal.platformmisc.NativePlatformMisc.getEnvironmentVariable (NativePlatformMisc.java:134) 
    at com.microsoft.tfs.jni.PlatformMiscUtils.getEnvironmentVariable (PlatformMiscUtils.java:52) 
    at com.microsoft.tfs.core.config.httpclient.DefaultHTTPClientFactory.shouldAcceptUntrustedCertificates (DefaultHTTPClientFactory.java:288) 
    at com.microsoft.tfs.core.config.httpclient.DefaultHTTPClientFactory.configureClientParams (DefaultHTTPClientFactory.java:324) 
    at com.microsoft.tfs.core.config.httpclient.DefaultHTTPClientFactory.newHTTPClient (DefaultHTTPClientFactory.java:137) 
    at com.microsoft.tfs.core.TFSConnection.getHTTPClient (TFSConnection.java:1041) 
    at com.microsoft.tfs.core.TFSConnection.getWebService (TFSConnection.java:874) 
    at com.microsoft.tfs.core.config.client.DefaultClientFactory$9.newClient (DefaultClientFactory.java:265) 
    at com.microsoft.tfs.core.config.client.DefaultClientFactory.newClient (DefaultClientFactory.java:90) 
    at com.microsoft.tfs.core.TFSConnection.getClient (TFSConnection.java:1470) 
    at com.microsoft.tfs.core.TFSTeamProjectCollection.getWorkItemClient (TFSTeamProjectCollection.java:370) 
    at tfs.clientobjectmodel.application.Program.main (Program.java:26)
```

Bundan sonraki kısımda ilk olarak TFSTeamProjectCollection tipinden bir nesne örneklendiği görülmektedir. Örnekleme sırasında ilk parametre olarak DefaultCollection’ a ait url adresi verilmiştir (Programda TFS sunucusunun kurulu olduğu makinedeki varsayılan Team Project Collection kullanılmaktadır). URI sınıfından yapılan bu bildirimi Credentials tipinden bir parametre takip etmektedir. Makineyi açan kullanıcının Credential bilgisi ile TFS’ e bağlanılmak istendiğinden DefaultNTCredentials tipinden bir nesne örneği ele alınmıştır. Ancak bir kullanıcı adı ve şifre ile gidilmek istenirse, UsernamePasswordCredentials sınıfından da yararlanılabilir (Elbette ilgili kullanıcıların söz konusu TFS sunucusuna ve koleksiyona erişebildiğini, bir başka deyişle gerekli yetkilere sahip olduğunu var sayıyoruz)

Kod parçasında ARGE isimli projeye ait Work Item’ lar çekilmeye çalışılmaktadır. Bu sebepten WorkItemStore servisine erişilmesi gerekmektedir. İlgili servisi kullanabilmek için getWorkItemClient fonksiyonu çağırılmaktadır. Bu metod geriye WorkItemClient tipinden bir referans döndürmektedir. Bu referans üzerinden çağırılan query fonksiyonuna girilen WIQL (WorkItem Query Language) sorgusu ile de WorkItemCollection elde edilir. Bu koleksiyon sorguya uygun bir Work Item içeriğini taşıyacaktır.

Pek tabi Java tarafında,.Net’ te olduğu gibi Property isimli bir tip üyesi (Type Member) bulunmamaktadır. Ancak.Net’ ten aşina olduğumuz WorkItem özelliklerine get ön ekli metodlar yardımıyla ulaşabiliriz. Örnekte Work Item’ ın Title değeri için getTitle (), Work Item tipinin adı için getType ().getName (), sistem de kayıtlı olan ID bilgisi için de getID () metodu kullanılmıştır.

Sonuç olarak ARGE isimli projedeki Work Item’ ların ID,Title değerlerinin, Work Item tipine göre sıralanarak elde edilmesi işlemi icra edilmektedir. Uygulamanın çalışma zamanı sonuçları aşağıda görüldüğü gibidir (Farklı WIQL sorguları ile örneği zenginleştirmeyi denemenizi öneririm ![Winking smile](images/wlEmoticon-winkingsmile_194.png))

[![jtfs_10](images/jtfs_10_thumb.png)](images/jtfs_10.png)

Senaryoyu işlettiğim sistemde test amaçlı olarak kullandığım ARGE isimli Team Project, Scrum 2.0 şablonunu kullanmaktaydı. Bu nedenle Product Backlog Item, Task ve Bug gibi Work Item öğelerini barındırmaktadır.

Biraz Daha

Dilerseniz örnek kod parçasını biraz daha geliştirmeye çalışalım. Örneğin yeni bir Work Item nasıl eklenir ona bakalım..Net tarafından biraz farklı olarak yeni bir WorkItem nesnesinin örneklenmesi için Project sınıfı üzerinden WorkItemClient referansına ulaşışması ve newWorkItem metodunun çağırılması gerekmektedir (Yani WorkItem sınıfını doğrudan bir yapıcı metod-Constructor ile örnekleyemiyoruz) Aynen aşağıdaki kod parçasında olduğu gibi.

```csharp
package tfs.clientobjectmodel.application;

import java.net.URI; 
import java.net.URISyntaxException;

import com.microsoft.tfs.core.TFSTeamProjectCollection; 
import com.microsoft.tfs.core.clients.workitem.WorkItem; 
import com.microsoft.tfs.core.clients.workitem.WorkItemClient; 
import com.microsoft.tfs.core.clients.workitem.project.Project; 
import com.microsoft.tfs.core.clients.workitem.wittype.WorkItemType; 
import com.microsoft.tfs.core.httpclient.Credentials; 
import com.microsoft.tfs.core.httpclient.HttpException;

public class Program {

    public static void main(String[] args) 
            throws HttpException, URISyntaxException { 
         
        URI uri=new URI("http://tfsserver:8080/tfs/defaultcollection"); 
        Credentials user=new com.microsoft.tfs.core.httpclient.DefaultNTCredentials(); 
        
        TFSTeamProjectCollection collection=new TFSTeamProjectCollection(uri,user); 
            
        WorkItemClient wiClient=collection.getWorkItemClient(); 
         
        Project argeProject=wiClient.getProjects().get("ARGE"); 
        WorkItemType pbi=argeProject.getWorkItemTypes().get("Product Backlog Item"); 
        
        WorkItem newWorkItem=argeProject.getWorkItemClient().newWorkItem(pbi); 
        newWorkItem.setTitle("Backoffice ekranlarin icin wire frame tasarim calismalari"); 
        newWorkItem.save(); 
        System.out.println(newWorkItem.getID()+" numarasi ile bir Product Backlog Item olusturuldu"); 
    } 
}
```

Dikkat edilmesi gereken noktalardan birisi de newWorkItem metoduna parametre olarak üretilmek istenen work item tipinin verilmesidir. Bu bildirim için WorkItemType sınıfına ait bir nesne örneği kullanılmaktadır. WorkItemType üretimi için projeye ait nesne örneği üzerinden önce var olan WorkItem tiplerinin elde edilmesi işlemi gerçekleştirilmiş, ardından ise Product Backlog Item tipi çekilmiştir. Bu son derece mantıklıdır, nitekim ilgili projenin şablonu (ki örneğimizde Scrum 2.0 söz konusudur) tarafından kullanılan Work Item tipleri ne ise, onlara ait Work Item nesneleri örneklenebilir. Üretilen Product Backclog Item için bir Title değeri verilmiş ve sonrasında Save metodu kullanılarak kayıt işlemi gerçekleştirilmiştir. Uygulama çalıştırıldığında hem Console penceresinden hem de ilgili projeye ait Backlog’ da bir öğenin oluşturulduğu gözlemlenecektir.

[![jtfs_11](images/jtfs_11_thumb.png)](images/jtfs_11.png)

Görüldüğü üzere Team Foundation Server Client Object Model’ in, Java tarafında kullanılması da son derece kolaydır. Örnek, tahmin edeceğiniz üzere Hello World formatındadır. Ancak daha önceki yazılarımızı baz alarak,.Net tarafındaki Client Object Model kodlarını, Java tarafına taşımayı deneyebilirsiniz. Kim demiş Java, Microsoft’ u, Microsoft’ ta Java’ yı umursamıyor diye

![Winking smile](images/wlEmoticon-winkingsmile_194.png)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
