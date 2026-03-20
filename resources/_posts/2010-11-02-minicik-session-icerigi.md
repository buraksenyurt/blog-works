---
layout: post
title: "Minicik Session İçeriği"
date: 2010-11-02 15:10:00 +0300
categories:
  - aspnet-4-0
tags:
  - aspnet-4-0
  - csharp
  - xml
  - dotnet
  - aspnet
  - sql-server
  - serialization
  - visual-studio
  - dataset
---
Bildiğiniz üzere bir süredir Microsoft Teknoloji Günleri Akşam Sınıfı etkinliklerini gerçekleştirmekteyiz. Kasım ayının konusu ise hemen her sürümünde köklü ve önemli yenilikler ile birlikte gelen Asp.Net'in 4.0 sürümü

[![blg237_Giris](/assets/images/2010/blg237_Giris_thumb.jpg)](/assets/images/2010/blg237_Giris.jpg)


![Winking smile](/assets/images/2010/wlEmoticon-winkingsmile_7.png)

Web programlamanın gelişen ihtiyaçları nedeniyle Asp.Net alt yapısında da her major sürümde fazlasıyla yenilik bulunmakta.

Tabi yine bildiğiniz üzere bendeniz web konusunda uzman değilim. Ağırlıklı olarak servis yönelimli mimari (Service Oriented Architecture) tarafı ile ilgilenmekteyim. Ancak eğitmenlik yaptığım dönemlerden kalan bir hastalık olsa gerek,.Net'in diğer pek çok alanı ile de ilgilenmekteyim. Eğitime blog yazısını hazırladığım tarih itibaryile oldukça az bir süre kaldı. Çıraklık başvuruları, Microsoft gönüllü çalışmaları, 11 haftalık proje planı olup bizden 4 haftada bitirmemizi istedikleri kocaman POC çalışması, S (h) arp Efe derken zamanı etkin kullanmak konusunda sıkıntılar yaşamaya başladım

![Confused smile](/assets/images/2010/wlEmoticon-confusedsmile_3.png)

Aslında.Net Framework 4.0 tarafını uzun süredir incelememe rağmen, en ince detaylarına kadar girmeden konuya hakim olmanın zor olacağını da gayet iyi biliyordum. Hatta bana göre yazarak anlatmak öğrenmenin en iyi yollarından birisi. İşte bu bi dolu düşünce altında başladığım gece çalışmasının sonucu olan küçük bir blog girdisi ile karşınızdayım. Bu yazımızda Asp.Net 4.0 tarafında gelen önemli yeniliklerden birisi olan serileştirilebilir Session içeriğini ufaltmak (daha teknik bir tabirle sıkıştırmak) konusuna değiniyor olacağız.

Session bildiğiniz üzere web çalışma modelinde önemli bir yere sahip. Özellikle oturum bazlı olarak veri tutulması istenen durumlarda, sunucu tarafında kullanılabilecek seçeneklerden birisi olduğunu ifade edebiliriz. Session içeriklerini kullanmak da son derece basit. Bu anlamda Asp.Net'in başlangıcından beri var olan HttpSessionState tipinden yararlanıldığını biliyoruz. Tabi Session kullanımında dikkat edilmesi gereken bazı hususlarda var. Söz gelimi varsayılan olarak In-Proc mod adı verilen ve çalışmakta olan Asp.Net uygulamasına ait Worker Process ile ilişkili bellek alanlarında tutulan Session içeriklerinin, SQL veritabanı veya farklı bir State Service'e ait process altında tutulması da söz konusu. Bu farklı tutuluş şekillerine ilaveten object tipi ile çalışabilen bir yapıdan bahsettiğimizi de ifade etmek isterim. Yani her tür.Net nesnesini atayabilirsiniz. Hımmm

![Sarcastic smile](/assets/images/2010/wlEmoticon-sarcasticsmile_3.png)

Şimdi In-Proc mod dışındaki modları göz önüne alalım. SQL sunucusu (SQLServer modu) veya State Service (StateServer modu) kullanan modları. Bu modlar kullanıldığında Session içerisine atılan verilerin serileştirilerek tutulması söz konusudur. Bu son derece mantıklıdır çünkü object tipi içerisine çok büyük boyutlu nesnelerin dahi atılması mümkündür. Ancak bu durum zamanla SQL veya State Service modlarının kullandığı alanların önemli ölçüde şişmesine de neden olabilir. İşte Asp.Net 4.0 ile Session ayarlamaları için eklenen compressionEnabled özelliği sayesinde, söz konusu Session içeriklerinin önemli ölçüde sıkıştırılması da mümkündür. Bu noktada System.IO.Compression.GZipStream tipinin büyük bir rol oynadığını ifade edebiliriz. Nitekim bu tip sayesinde serileştirilmiş olan verinin sıkıştırılması söz konusudur.

Yazımızın ilerleyen kısımlarında sıkıştırmanın bu noktadaki önemini vurgulamak açısından basit bir örnek üzerinden ilerlemeye çalışıyor olacağız. Örnek senaryomuzda Session bilgilerini SQL veritabanı üzerinde tutuyor olacağız. Bu amaçla bir ön hazırlık yapmamız gerekiyor. Visual Studio 2010 Command Prompt üzerinden aspnetregsql aracını kullanarak söz konusu veritabanını (Örneğimizde SessionDB) oluşturabiliriz.

Kurulum;

C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC>aspnet_regsql -S localhost -E -ssadd -sstype c -d SessionDB

Start adding session state.

....

Finished.

To use this custom session state database in your web application, please specify it in the configuration file by using the 'allowCustomSqlDatabase'and 'sqlConnectionString'attributes in the \ section.

Bu kurulum işlemi sonrasında SessionDB içerisinde oluşturulan ASPStateTempSessions tablosunun yapısı da aşağıdaki gibi olacaktır. İki alan; SessionItemShort ve SessionItemLong şu an ki vakamız için önem arz etmektedir. Bu alanlarda serileştirilen Session içerikleri tutulmaktadır. Bununla birlikte serileşen içeriğin büyüklüğüne göre iki alandan bir tanesine veri eklenmesi de söz konusu olacaktır.

[![blg237_Table](/assets/images/2010/blg237_Table_thumb.gif)](/assets/images/2010/blg237_Table.gif)

Şimdi basit bir Asp.Net Web Application üzerinden ilerleyelim. Söz konusu web uygulamasında en önemli nokta Session kullanımına ait konfigurasyon ayarlarıdır. Default.aspx sayfamız son derece basit bir fonksiyonelliğe sahiptir. Button kontrolüne basıldığında Logo isimli bir sınıf örneğinin Session nesnesi olarak atılması söz konusudur. İşte örnek uygulama kodlarımız.

```csharp
using System; 
using System.IO; 
using System.Data.SqlClient; 
using System.Data;

namespace OldStyleSession 
{ 
    [Serializable] 
    public class Logo 
    { 
        public string FileContent { get; set; } 
        public string Name { get; set; } 
        public DateTime CreationTime { get; set; } 
    }

    public partial class _Default 
        : System.Web.UI.Page 
    { 
        protected void btnAddToSession_Click(object sender, EventArgs e) 
        { 
            Logo lg = new Logo 
            { 
                 Name="Car" 
                 , CreationTime=DateTime.Now 
                 , FileContent=File.ReadAllText(Server.MapPath("/Bilgiler.txt"))               
            }; 
            Session.Add("Logo", lg); 
        } 

        protected void btnWriteSession_Click(object sender, EventArgs e) 
        { 
            using (SqlConnection conn = new SqlConnection("data source=.;database=SessionDB;integrated security=SSPI")) 
            { 
                SqlCommand cmd = new SqlCommand("Select SessionItemShort,SessionItemLong From AspStateTempSessions Where SessionId=@SessionId", conn); 
                cmd.Parameters.AddWithValue("@SessionId", txtSessionId.Text); 
                conn.Open(); 
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection); 
                if (reader.Read()) 
                { 
                   var itemShort=reader.GetSqlBytes(0); 
                    var itemLong = reader.GetSqlBytes(1); 
                    Response.Write( 
                        String.Format("SessionItemShort length = {0}, SessionItemLong length = {1}" 
                        ,itemShort.IsNull?0:itemShort.Length 
                        ,itemLong.IsNull?0:itemLong.Length 
                        ) 
                        ); 
                } 
                reader.Close(); 
            } 
        } 
    } 
}
```

Görüldüğü üzere Session özelliğinin Add metodundan yararlanılarak lg isimli Logo nesne örneğinin ilave edilmesi söz konusudur. Diğer yandan bir de üretilen Session için yazılan SessionItemLong ve SessionItemShort alanlarının içeriklerinin byte cinsinden uzunluklarını ekrana yazdırabileceğimiz bir metodumuz da bulunmaktadır. Bu metodu üretilen Session içeriklerinin boyutlarını kıyaslamak için kullanıyor olacağız.

Örnekte kullanılan Bilgiler.txt isimli Text tabanlı dosya 328 Kb büyüklüğünde anlamsız bir içeriğe sahiptir. Burada text tabanlı bir içerik söz konusu olduğundan sıkıştırma işlemini ele alacak olan algoritma çarpıcı sonuçlar üretecektir. İlk etapta web.config dosyasının içeriğini aşağıdaki gibi tasarladığımızı düşünebiliriz.

```xml
<?xml version="1.0"?> 
<configuration> 
  <system.web> 
    <sessionState allowCustomSqlDatabase="true" sqlConnectionString="data source=.;database=SessionDB;integrated security=sspi" mode="SQLServer" compressionEnabled="false"/> 
    <compilation debug="true" targetFramework="4.0"> 
    </compilation> 
  </system.web> 
</configuration>
```

SessionDB isimli bir veritabanı adı kullandığımızdan allowCustomSqlDatabase özelliğine true değeri atanmıştır. sqlConnectionString bilgisinde ise Session bilgilerinin yazılacağı veritabanı bağlantısı belirtilmektedir. mode niteliğine atanan değer ile Session içeriklerinin SQL veritabanı üzerinde (sqlConnectionString ile belirtilen bağlantıya doğru) tutulacağı ifade edilmektedir. Şu an için compressionEnabled özelliğine false değeri atanmıştır. Bu şekilde aslında Asp.Net 4.0 öncesinde olduğu gibi standart bir Session tutma işlemi yapılacağı belirtilmektedir. Örneği bu şekilde test ettiğimizde ASPStateTempSessions tablosunda üretilen Session satırına ait sorgulama sonucu, SessionItemLong alanına binary bir içeriğin serileştirilmiş olduğu görülecektir.

Sql tarafında Compression kullanılmadığı haldeki durum;

İlk olarak sıkıştırma işlemini kullanmadığımız durumu ele alalım. Add Session işleminden sonra, üretilen SessionID değerini web sayfamız üzerinde kullanırsak aşağıdaki sonuçlar ile karşılaşırız.

[![blg237_FirstCase](/assets/images/2010/blg237_FirstCase_thumb.gif)](/assets/images/2010/blg237_FirstCase.gif)

Dikkat edileceği üzere 335141 uzunluğunda bir byte içeriği söz konusudur.

Gelelim sıkıştırılma durumuna. Bu sefer compressionEnabled özelliğine true değerini vermemiz yeterli olacaktır.

```xml
<?xml version="1.0"?> 
<configuration> 
  <system.web> 
    <sessionState allowCustomSqlDatabase="true" sqlConnectionString="data source=.;database=SessionDB;integrated security=sspi" mode="SQLServer" compressionEnabled="true"/> 
    <compilation debug="true" targetFramework="4.0"> 
    </compilation> 
  </system.web> 
</configuration>
```

Sql tarafında Compression kullanılması haldeki durum ise aşağıdaki gibi olacaktır;

[![blg237_SecondCase](/assets/images/2010/blg237_SecondCase_thumb.gif)](/assets/images/2010/blg237_SecondCase.gif)

Görüldüğü üzere bir önceki vakanın tersine serileştirilebilir içerik SessionItemLong alanı yerine SessionItemShort içerisine eklenmiştir. Bununla birlikte söz konusu sıkıştırılmış verinin içeriği 3686 dır

![Disappointed smile](/assets/images/2010/wlEmoticon-disappointedsmile_1.png)

Sıkıştırma algoritmasının uygulanması sonucu neredeyse standart session içeriğinin %0,0109983559158682 üne kadar verinin küçültülmesi söz konusudur. Elbette burada text tabanlı bir içerik kullanıldığından söz konusu farkın oluşması doğaldır. Özellikle binary içerikten oluşan (örneğin resim formatı gibi) bir verinin serileştirilmesi esnasında veri boyutlarında önemli bir fark olmayadabilir. Bu durumu analiz etmek için aşağıdaki kod parçasını göz önüne alalım.

```csharp
protected void btnAddDataTable_Click(object sender, EventArgs e) 
{ 
    using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI")) 
    { 
        using (SqlDataAdapter adapter = new SqlDataAdapter("select * from Production.ProductPhoto", conn)) 
        { 
            DataSet ds = new DataSet("ProductPhoto"); 
            adapter.Fill(ds);

            Session.Add("ProductPhoto", ds); 
        } 
    } 
}
```

Bu kez ProductPhoto isimli tablonun tüm içeriğini çektiğimiz bir DataSet örneğini Session'a ekliyoruz. Yine ilk olarak compressionEnabled niteliğinin false değere sahip olduğu ve sonrasında true olması halini göz önüne alalım. Bu durumda aşağıdaki örnek çıktılar elde edilecektir.

Sıkıştırılma kapalı iken;

[![blg237_ThirdCase](/assets/images/2010/blg237_ThirdCase_thumb.gif)](/assets/images/2010/blg237_ThirdCase.gif)

Sıkıştırılma açık iken;

[![blg237_FourthCase](/assets/images/2010/blg237_FourthCase_thumb.gif)](/assets/images/2010/blg237_FourthCase.gif)

Sıkıştırılmama durumunda 2712191 iken sıkıştırılma durumunda 2711426. Yani sadece % 1,000282139361355 oradanın bir sıkıştırma söz konusu olmakta.

![Crying face](/assets/images/2010/wlEmoticon-cryingface_1.png)

Değer mi? Değmez. Buna göre serileştirilebilir içeriklerin sıkıştırılabilir olmaları da önem kazanmaktadır. (Nitekim bir PDF veya JPEG dosyasını sıkıştırdığınızda önemli ölçüde bir sıkıştırma olmadığını görürüz) İnandırıcı geldi mi?

![Annoyed](/assets/images/2010/wlEmoticon-annoyed_1.png)

Varsayımsal Yaklaşım

Tahmini olarak Asp.Net çalışma zamanının Session verisini nasıl işlediğini düşünelim. Sıkıştırma modu kapalı iken, binary formatta serileştirme işlemi yapılıp verinin içeriye atılıyor olması söz konusu olabilir. Diğer yandan sıkıştırma modu açık olduğunda, serileşen içeriğin olduğu gibi yazılmadan önce belki de bir GZipStream işleminden geçirilmesi ve sıkıştırılmaya çalışılması söz konusu olabilir.

Bu varsayım altında şu şekilde ilerliyor olacağız. DataSet nesnesinin kendisini standart bir binary serileştirme işlemine tutacağız. Sonrasında ise bu içeriği GZipStream ile sıkıştırmayı deneyip içeriklerin boyutlarına bakacağız. İspat için bu tekniği kullanıyor olacağız. Haydi parmakları sıvayalım. İşte örnek kod parçamız.

```csharp
using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI")) 
{ 
    using (SqlDataAdapter adapter = new SqlDataAdapter("select * from Production.ProductPhoto", conn)) 
    { 
        DataSet ds = new DataSet("ProductPhoto"); 
        adapter.Fill(ds);

        BinaryFormatter formatter = new BinaryFormatter(); 
        FileStream fs= new FileStream("c:\\DataSet.bin",FileMode.OpenOrCreate,FileAccess.Write); 
        formatter.Serialize(fs, ds); 
        fs.Close(); 
        
        FileStream fsZip= new FileStream("c:\\DataSetZip.bin",FileMode.OpenOrCreate,FileAccess.Write); 
        GZipStream gStream = new GZipStream(fsZip, CompressionMode.Compress); 
        byte[] dataSetBytes=File.ReadAllBytes("C:\\DataSet.bin"); 
        gStream.Write(dataSetBytes, 0,dataSetBytes.Length); 
        gStream.Close(); 
        fsZip.Close(); 
    } 
}
```

Kod parçasından görüldüğü üzere ilk olarak Binary formatta bir DataSet içeriğini serlişetirmekteyiz. Bu zaten Session tarafında SessionItemLong veya SessionItemShort alanına atılan serileştirilebilir içeriktir. Kodun ilerleyen kısımlarında ise serileştirilen içeriği GZipStream sınıfından yararlanarak sıkıştırmaya çalışıyoruz. Bunun sonucu olarak üretilen dosya içeriklerinin boyutlarına baktığımızda ise aşağıdaki ekran görüntüsünde yer alan sonuçları elde ederiz.

[![blg237_Last](/assets/images/2010/blg237_Last_thumb.gif)](/assets/images/2010/blg237_Last.gif)

Dikkat edileceği üzere serileştirilmiş içerik ile serileştirilmiş içeriğin sıkıştırılmış versiyonları arasında boyut olarak pek bir fark yoktur. Bu da DataSet tipinin ve özellikle ProductionPhoto içerisindeki binary alanlarının iyi bir şekilde sıkıştırılamıyor olmalarından kaynaklanmaktadır. Son geliştirdiğimiz örnek tamamen ve tamamen GZipStream ile sıkıştırma tekniğinin her zaman işe yaramayacağını ve veri boyutunda daima önemli ölçüde bir değişikliğe neden olmayacağını göstermek üzere ele alınmıştır bunu unutmayalım. Sanıyorum ki iyi kullanıldığı takdirde Session sıkıştırması özellikle çok fazla oturumun açıldığı web uygulamalarında, SQL Server veya State Server mod kullanılması halinde önemli yer kazancı sağlayacak şekilde fayda getirmektedir. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[OldStyleSession.rar (137,04 kb)](/assets/files/2010/OldStyleSession.rar)