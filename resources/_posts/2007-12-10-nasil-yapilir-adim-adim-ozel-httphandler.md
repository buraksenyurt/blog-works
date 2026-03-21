---
layout: post
title: "Nasıl Yapılır? Adım Adım Özel HttpHandler"
date: 2007-12-10 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - asp.net
  - http-handler
---
Uzun zaman önce Asp.Net 2.0 ile ilişkili makalelerimizden birisinde HttpHandler ve HttpModule kavramlarından bahsetmeye çalışmıştık. Bu makalemizde kendi Handler sınıfımızı geliştirmek isteyebileceğimiz örnek bir senaryo üzerinde daha durmaya çalışacağız. Bu sayede HttpHandler sınıfları yazarak neler yapılabileceğinide daha net bir şekilde görmüş olacağız. Konuyu daha net kavrayabilmek adına örnek senaryomuz üzerinden adım adım ilerleyeceğiz.

Bilindiği üzere web sunucusuna istemci tarafından gelen talepler (Requests) bazı program ara yüzleri (API) tarafından karşılanır ve uygun ortamlara işletilmek üzere iletilirler. Özellikle Asp.Net ile geliştirilen web uygulamalarında, talep edilen dosya tipine göre devreye giren HttpHandler sınıfları bulunmaktadır. Söz gelimi aspx uzantılı dosyalar PageHandlerFactory isimli sınıf tarafından ele alınırlar. PageHandlerFactory ve benzer işlevselliklere sahip handler tipleri IHttpHandler arayüzünü (interface) uygularlar. Dolayısıyla geliştiriciler kendi Handler tiplerini IHttpHandler arayüzünü kullanarak yazabilirler.

Gelelim örnek senaryomuza. Sunucu üzerinde barındırılan XML (eXtensible Markup Language) tabanlı basit bir metin dosyasını ele alacak özel bir Handler tipi geliştiriyor olacağız. XML tabanlı dosya içerisinde yer alan bilgilerden yararlanılarak ekrana herhangibir sorguya ait raporlama sonuçları aktarılacak. Dosyanın uzantısının örnek olarak rapx olduğunu düşünebiliriz. Peki bu raporlama işleminde önemli bir rol oynacak olan XML içeriğinde neler olması gerekmektedir? Bunların tespiti XML dosyasının mantıksal ağaç yapısının oluşturulmasınıda kolaylaştıracaktır. Söz konusu ihtiyaçları aşağıdaki maddeler halinde sıralanabilirler.

- Hazırlanan rapor için bir başlık (Title) bilgisi tutulabilir. Hatta başlığın arka plan rengi (Background Color) verilerek zengin görünmesi sağlanabilir.
- Rapor dosyasının kaç adet sorgu cümlesi (Query) için destek vereceğine karar vermek gerekmektedir. Örneğin basit olması açısından başlangıç itibariyle sadece tek bir sorgu sonucunun ele alınmasında fayda vardır.
- Sorgu cümlesi, saklı yordam (stored procedure) veya text tabanlı ifadeleri işaret edebilir. Hatta bir veritabanı görünümünün (View) desteklenmesi bile sağlanabilir.
- Sorgu cümlesinde parametre kullanımına destek verilebilir. Bu parametrelerinin QueryString yardımıyla Url üzerinden veya XML içeriğinden alınacağına dair tanımlamalar yapılabilir.
- Sorgu cümlesinin çalışacağı sunucu (Server) ve veritabanı (Database) adı belirtilmelidir. Hatta SSPI ile bağlantıya destek verilmeside göz önüne alınmalıdır.
- Hazırlanan raporların XML içeriğinde belirtilen mail adreslerine gönderilmesi de sağlanabilir.
- Raporun kimler tarafında görülebileceğine dair tanımlamalar yapılabilir. Bu tanımlamalar kullanıcı (User) ve hatta rol (Role) bazında gerçekleştirilebilir.

Bu ihtiyaçlar dahada arttırılabilir. Bir anlamda geliştiricinin hayal gücü burada önem kazanmaktadır. Yukarıdaki maddelerde sözü geçen ihtiyaçların tamamı XML'in temel kavramları ile karşılanabilir. Bir başka deyişle eleman (element) ve nitelikler (attributes) sayesinde yukarıdaki istekler standartlara uygun olacak şekilde tasarlanabilir.

![dikkat.gif](/assets/images/2007/dikkat.gif)
Söz konusu XML içeriğinin çalışma zamanında (run time) uygun bir standartta olduğunun garanti altına alınması için şema (Xml Schema) kullanılması yararlı olacaktır. Bu şema ile Xml içeriğinin çarpıştırılması Handler tipi içerisinde yapılabilir. Şemaya uymayan XML içerikleri için Handler, istekleri özel olarak tasarlanmış bir hata sayfasına doğru yönlendirebilir.

Yukarıdaki maddeler ışığında aşağıdaki gibi örnek bir XML içeriği göz önüne alınabilir.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Raporlar>
    <Rapor Id="">
        <Baslik ArkaPlan=""></Baslik>
        <Baglanti SSPI="">
            <Sunucu></Sunucu>
            <Veritabani></Veritabani>
            <KullaniciAdi></KullaniciAdi>
            <Sifre></Sifre>
        </Baglanti>
        <Sorgu Sp="">
            <Cumle></Cumle>
            <Parametreler Nereden="">
                <Parametre Ad="" Deger=""/>
            </Parametreler>
        </Sorgu>
        <MailListesi MailGonder="">
            <Mail></Mail>
            <Mail></Mail>
        </MailListesi>
    </Rapor>
</Raporlar>
```

Raporlar ana boğumu (Root Node) birden fazla Rapor elementi içerebilir. Biz örneğin daha kolay ele alınabilmesi amacıyla tek bir Rapor elementi kullanıyor olacağız. Raporun alınacağı sunucu (Server), veritabanı adı (Database Name), kullanıcı adı (User name) ve şifre (Password) bilgileri ise Baglanti elementi altında tutulmaktadır. Hatta SSPI kullanımınada destek verilmesi amacıyla Baglanti elementi içerisinde bir nitelik (attribute) tanımlanmaktadır. Sorguların saklı yordam (Stored Procedure) olup olmadığı Sp niteliği ile belirtilebilir. Bunun dışında sorgu içerisinde kullanılan parametreler var ise bunların nereden alınacağı Nereden isimli nitelik (attribute) ile belirlenmektedir. Öyleki raporun parametreleri, rapx dosyasına tarayıcı üzerinden yapılan çağrılarda QueryString yardımıyla gelebilir. Yada Parametre elementleri içerisindeki Deger niteliklerinde doğrudan tanımlanabilir. Üretilen raporların mail yolu ile kimlere bildirileceğine dair MailListesi elementi ve Mail alt elementleri kullanılabilir.

Burada ortaya çıkan önemli bir ihtiyaç vardır. XML bilgilerinin tutulduğu rapx uzantılı dosyaların işlenmesi, bir HTML çıktısının üretilmesi ve talepte bulunan istemcilere gönderilmesi sağlanmalıdır. Bunu sadece özel bir Handler tipi karşılayabilir.

Dilerseniz vakit kaybetmeden Handler tipini yazarak işe başlayalım. Geliştirilecek olan Handler sınıfı ilk etapta tek bir web uygulamasında kullanılacaktır. Sonrasında ise bu handler tipinin sunucu üzerindeki her Asp.Net web uygulaması için geçerli olması sağlanacaktır. Bu nedenle Handler tipinin bir sınıf kütüphanesi (Class Library) içerisinde olmasında yarar vardır. Handler sınıfının IHttpHandler arayüzünden (Interface) türetilmesi içinde sınıf kütüphanesine System.Web.dll assembly'ının referans edilmesi şarttır. Söz konusu Handler sınıfının diagram görüntüsü ve içeriği aşağıdaki gibidir.

![mk234_8.gif](/assets/images/2007/mk234_8.gif)

```csharp
using System;
using System.Web;
using System.Data;
using System.Web.Hosting;
using System.Data.SqlClient;
using System.Xml;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;

namespace ReportHandlerLibrary
{
    public class RaporHandler:IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        // Gelen talep sonrası üretilecek HTML çıktısını bu metod içerisinden veriyor olacağız
        public void ProcessRequest(HttpContext context)
        {
            // Talep edilen sayfa Request.Path ile yakalanır. 
            // VirtualPathProvider metodu ile sanal adresin karşılığı olan fiziki yoldaki dosya açılarak Stream halinde elde edilir. 
            // Stream'den yararlanılarak XML içeriği XmlDocument nesnesi içerisine alınır.
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(VirtualPathProvider.OpenFile(context.Request.Path));
            if (context.Request.QueryString["MailGonder"] == null)
            {
                // Baslik elementinden raporun başlığı bilgisi alınır
                string baslik = xDoc.SelectSingleNode("Raporlar/Rapor/Baslik").InnerText;
                string baslikArkaPlanRengi = xDoc.SelectSingleNode("Raporlar/Rapor/Baslik").Attributes["ArkaPlan"].InnerText;
        
                // Ekran tasarımı oluşturulmaya başlanır
                // Üretilen çıktı bir HTML sayfası olacağından HTML elementlerinin kullanılması gerekmektedir.
                context.Response.Write("<HTML><HEAD><TITLE>" + baslik + "</TITLE></HEAD>");
                context.Response.Write("<BODY>");
                // Table elementi oluşturulur.
                context.Response.Write("<TABLE border='1' width='100%' cellspacing='0' cellpadding='5'>");
                // Tablo 3 satırdan oluşmaktadır. İlk satırın arka plan rengi ve içerisinde yer alacak metin bilgisi Baslik elementi ve ArkaPlan niteliklerinden alınır
                context.Response.Write("<TD style='background-color:" + baslikArkaPlanRengi + "'>");
                context.Response.Write("<H3>" + baslik + "</H3>");
                context.Response.Write("</TD></TR>");
                // İkinci satır içerisinde rapor sonucu üretilen grid içeriği olmalıdır.
                context.Response.Write("<TR><TD>");

                // Bu hücreye veri ile doldurulan Grid içeriğinin HTML çıktısı yazdırılır
                context.Response.Write(GridHTMLUret(xDoc,context).ToString());
    
                context.Response.Write("</TD></TR>");
                context.Response.Write("<TR><TD>");
    
                // Mail gönderme aksiyonu için basit bir hyperLink elementi eklenir
                context.Response.Write("<a href='"+context.Request.Path + "?MailGonder=1'>" + "Raporu Mail Olarak Gönder" + "</a>");
            
                context.Response.Write("</TD></TR>");
                context.Response.Write("</BODY>");
                context.Response.Write("</HTML>");
            }
            else
            {
                bool mailGondersinmi = false;
                Boolean.TryParse(xDoc.SelectSingleNode("Raporlar/Rapor/MailListesi").Attributes["MailGonder"].Value, out mailGondersinmi);
                if (mailGondersinmi)
                    MailGonder(xDoc,context);
            }
        }

        #endregion

        // GridView kontrolünün içeriği doldurulduktan sonra HTML içeriği elde edilir ve bu içeriği taşıyan StringWriter geriye döndürülür.
        private StringWriter GridHTMLUret(XmlDocument xDoc,HttpContext ctx)
        {
            // SqlDataAdapter nesne oluşturulur
            SqlDataAdapter adapter = new SqlDataAdapter(KomutHazirla(xDoc,ctx));
            DataTable table = new DataTable();
            // DataTable doldurulur
            adapter.Fill(table);
    
            // GridView kontrolü üretilir ve veriye bağlanır
            GridView grd = new GridView();
            grd.DataSource = table;
            grd.DataBind();
    
            //GridView kontrolünün HTML çıktısı elde edilir
            StringWriter strWriter = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(strWriter);
            grd.RenderControl(writer);
            return strWriter;
        }

        // Raporun üretilmesi için gerekli SqlCommand hazırlanıyor.
        private SqlCommand KomutHazirla(XmlDocument xDoc,HttpContext ctx)
        {
            bool sp = false;
            // Sorgu cümlesinin Stored Procedure olup olmadığı belirlenir.
            Boolean.TryParse(xDoc.SelectSingleNode("Raporlar/Rapor/Sorgu").Attributes["Sp"].Value, out sp);
            
            // SqlCommand tipi hazırlanır
            SqlCommand cmd = new SqlCommand();
            // Sorgu cümlesi alınır
            cmd.CommandText = xDoc.SelectSingleNode("Raporlar/Rapor/Sorgu/Cumle").InnerText.Trim();
            // Bağlantı cümlesi BaglantiCumlesiOlustur metodundan elde edilir ve Command için gerekli SqlConnection hazırlanır.
            cmd.Connection = new SqlConnection(BaglantiCumlesiOlustur(xDoc));
            // Eğer cümle Stored Procedure adını işaret ediyorsa CommandType için StoredProcedure enum sabiti değeri verilir
            if (sp)
                cmd.CommandType = CommandType.StoredProcedure;
    
            // Eğer girilmiş parametreler varsa bu parametreler Command nesnesine AddWithValue metodu ile teker teker eklenir.
            if (xDoc.SelectSingleNode("Raporlar/Rapor/Sorgu/Parametreler").ChildNodes.Count > 0)
            {
                string parametreNereden = xDoc.SelectSingleNode("Raporlar/Rapor/Sorgu/Parametreler").Attributes["Nereden"].Value;
                XmlNodeList parametreler = xDoc.SelectSingleNode("Raporlar/Rapor/Sorgu/Parametreler").ChildNodes;
                foreach (XmlNode parametre in parametreler)
                {
                    if(parametreNereden=="Xml")
                        cmd.Parameters.AddWithValue(parametre.Attributes["Ad"].Value, parametre.Attributes["Deger"].Value);
                    else if(parametreNereden=="QueryString")
                        cmd.Parameters.AddWithValue(parametre.Attributes["Ad"].Value, ctx.Request.QueryString[parametre.Attributes["Ad"].Value.Substring(1, parametre.Attributes["Ad"].Value.Length - 1)]);
                }
            }
            // Oluşturulan Command nesnesi geri döndürülür.
            return cmd;
        }

        // Sorguların çalıştırılması için gerekli Bağlantı cümlesini oluşturan metod
        private string BaglantiCumlesiOlustur(XmlDocument xDoc)
        {
            bool sspi = false;
    
            // Sql bağlantısı için gerekli bağlantı cümlesi(Connection String) SqlConnectionStringBuilder sınıfı yardımıyla oluşturulur.
            SqlConnectionStringBuilder conStrBuilder = new SqlConnectionStringBuilder();
        
            // Sunucu ve veritabanı bilgileri XPath ifadeleri ile alınır.
            conStrBuilder.DataSource = xDoc.SelectSingleNode("Raporlar/Rapor/Baglanti/Sunucu").InnerText;
            conStrBuilder.InitialCatalog = xDoc.SelectSingleNode("Raporlar/Rapor/Baglanti/Veritabani").InnerText;
            Boolean.TryParse(xDoc.SelectSingleNode("Raporlar/Rapor/Baglanti").Attributes["Sspi"].Value, out sspi);
            if (!sspi) // Eğer integrated security ile bağlanılmıyorsa kullanıcı adı(UserID) ve şifre(Password) bilgileri alınır.
            {
                conStrBuilder.UserID = xDoc.SelectSingleNode("Raporlar/Rapor/Baglanti/KullaniciAdi").InnerText;
                conStrBuilder.Password = xDoc.SelectSingleNode("Raporlar/Rapor/Baglanti/Sifre").InnerText;
            }
            else
                conStrBuilder.IntegratedSecurity = true;

            // Oluşturulan bağlantı cümlesi(Connection String) geri döndürülür.
            return conStrBuilder.ConnectionString;
        }

        // Mail gönderme seçeneği aktif ise postaların gönderilme işlemini gerçekleştirecek olan metod.
        private void MailGonder(XmlDocument doc,HttpContext ctx)
        {
            // Mail listesi MailListesi elementinin alt elementlerinden çekilir.
            XmlNodeList mailler = doc.SelectSingleNode("Raporlar/Rapor/MailListesi").ChildNodes;
            foreach (XmlNode mail in mailler)
            {
                //TODO: Bu noktada raporun mail olarak gönderilmesine ait işlemler yapılacaktır.
                ctx.Response.Write(mail.InnerText + " adresine rapor mail olarak gönderildi<br/>");
            }
        }
    }
}
```

Geliştirilen sınıf içerisinde parçaları daha kolay ele alabilmek adına yardımcı metodlar yer almaktadır. Dikkat edileceği üzere rapx içeriğinin XML formatında tasarlanması, işlemleri son derece kolaylaştırmaktadır. Nitekim içeriğin XmlDocument sınıfına ait bir nesne örneği ile belleğe alınması, içerisindeki elementlerin veya niteliklerin XPath ifadeleri ile yakalanması son derece kolaydır. Üstelik XML, platform bağımsızlık sunduğundan bu dökümanın başka bir ortama gönderilerek ele alınmasının sağlanması daha kolaydır.

Bu sebepten dolayı günümüzün popüler kavramlarından olan Reporting Services veya LINQ To SQL içerisinde yer alan Database Markup Language (dbml) gibi yapılar XML üzerine oturmaktadır. Sınıf içerisinde dikkat edilmesi gereken noktalardan bir diğeri ise, HttpContext tipinin ektin şekilde kullanımıdır. HttpContext üzerinden ele alınan üyeler ile, üretilecek HTML içeriğinin tasarlanması veya talep ile gelen QueryString'lerin yakalanması söz konusudur. Hatta talep edilen sayfanın sanal yolu (Virtual Path) elde edilip XML içeriğinin VirtualPathProvider sınıfının OpenFile metodu ile kolay bir şekilde Stream'e dönüştürülmeside sağlanmaktadır. İşlemleri kolaylaştıran noktalardan biriside web kontrollerinin RenderControl metodudur. Bu metod ile kompleks bir GridView kontrolünün veri dolu içeriğinin HTML çıktısını almak son derece kolaylaşmaktadır.

Şimdi geliştirilen Handler tipini örnek bir web uygulamasında test edebiliriz. İlk olarak tek bir web uygulamasına özel olacak şekilde Handler tipinin kullanılmasını ele alacağız. Böyle bir durumda öncelikli olarak web uygulamasının Handler tipini içeren sınıf kütüphanesini (Class Library) referans etmesi gerekmektedir. Örnek olarak OzelHandlerKullanimi isimli web uygulaması aşağıdaki ekran görüntüsünden de anlaşılacağı üzere ReportHandlerLibrary isimli assembly'ı referans etmektedir.

![mk234_1.gif](/assets/images/2007/mk234_1.gif)

Bunun dışında web uygulamasına ait web.config dosyası içerisinde aşağıda görüldüğü gibi gerekli bildirimler yapılmalıdır.

```xml
<?xml version="1.0"?>
<configuration>
    <appSettings/>
    <connectionStrings/>
    <system.web>
        <httpHandlers>
            <add path="*.rapx" type="ReportHandlerLibrary.RaporHandler,ReportHandlerLibrary" verb="*" validate="true"/>
        </httpHandlers>
        <compilation debug="true"/>
        <authentication mode="Windows"/>
    </system.web>
</configuration>
```

httpHandlers elementi system.web elementi içerisinde tanımlanmalıdır. Örnekte rapx uzantılı dosyalara gelecek herhangibir talebin (Get, Post vb...) type niteliğinde (attribute) belirtilen RaporHandler sınıfına ait nesne örneği tarafından karşılanacağı belirtilmektedir. Bu adım tek başına yeterli değildir. Ayrıca IIS (Internet Information Services) üzerinden, rapx uzantılı dosyalara gelecek olan talepler için AspNetIsapi.dll'inin devreye gireceğinin belirtilmesi gerekmektedir. Örnek senaryo IIS 7.0 üzerinde geliştirilmektedir. IIS 7.0 üzerinden OzelHandlerKullanimi isimli web uygulaması adına rapx-AspNetIsapi.dll eşleştirmesi için öncelikli olaran InetMgr aracı üzerinden ilgili web uygulamasına geçilmeli ve Handler Mappings kısmı aşağıdaki ekran görüntüsünde olduğu gibi seçilmelidir.

![mk234_2.gif](/assets/images/2007/mk234_2.gif)

Handler Mappings kısmında varsayılan ve izin verilen uzantı eşleştirmeleri yer almaktadır. Bu kısımda sağ tıklanarak açılan menüden Add Script Map seçilmeli ve gerekli eşleştirme IIS tarafına bildirilmelidir.

![mk234_3.gif](/assets/images/2007/mk234_3.gif)

Add Script Map ile açılan iletişim kutusunda ise aşağıdaki ayarların yapılması gerekmektedir.

![mk234_4.gif](/assets/images/2007/mk234_4.gif)

Buna göre rapx uzantılı taleplerin.Net 2.0 çalışma zamanında yer alan AspNetIsapi.dll tarafından ele alınacağı belirtilmektedir. Uygulamaya ait web.config dosyası içerisinde gerekli Handler tanımlamaları yapıldığı için AspNetIsapi.dll program arayüzü, gelen talep (Request) için RaporHandler sınıfının devreye girmesini sağlayacaktır. Bu işlemin ardından Handler Mappings kısmına aşağıdaki ekran görüntüsünde de görüldüğü gibi rapx uzantısı için gerekli eşleştirme eklenecektir.

![mk234_5.gif](/assets/images/2007/mk234_5.gif)

Yapılan bu değişikliker sonrasında web uygulmasında ait web.config dosyası içerisindede aşağıdaki gibi yeni bir tanımlama oluşacaktır. Bir başka deyişle IIS (Internet Information Services) üzerinde yapılan eşleştirmeye ait bilgiler system.webServer elementi içerisindeki handlers kısmına eklenmektedir.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <appSettings />
    <connectionStrings />
    <system.web>
        <httpHandlers>
            <add path="*.rapx" type="ReportHandlerLibrary.RaporHandler,ReportHandlerLibrary" verb="*" validate="true" />
        </httpHandlers>
        <compilation debug="true" />
        <authentication mode="Windows" />
    </system.web>
    <system.webServer>
        <handlers>
            <add name="Rapor X" path="*.rapx" verb="*" modules="IsapiModule" scriptProcessor="C:\Windows\Microsoft.NET\ Framework\v2.0.50727\aspnet_isapi.dll" resourceType="File" />
        </handlers>
    </system.webServer>
</configuration>
```

Artık test amacıyla web uygulamasına rapx uzantılı örnek bir içerik eklenebilir. Örnek olarak AdventureWorks veritabanında yer alan Products tablosu ile ilişkili bir rapor düşünülebilir. Bu raporun stok seviyesi belirli bir değerin altında olan ürünlerin kategori bazlı sayılarını verdiğini düşünelim. Buna göre web uygulaması altında tutulacak olan rapx uzantılı metin dosyasının içeriği aşağıdaki gibi tasarlanabilir.(Bu dosyayı eklerken Add New Item-Text File seçerek KategoriBazliUrunler.rapx gibi bir seçim yapılması gerektiğine dikkat edilmelidir.)

KategoriBazliUrunler.rapx;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Raporlar>
    <Rapor Id="1">
        <Baslik ArkaPlan="#FFCC11">Kategori Bazlı Ürün Sayıları</Baslik>
        <Baglanti Sspi="true">
            <Sunucu>localhost</Sunucu>
            <Veritabani>AdventureWorks</Veritabani>
            <KullaniciAdi></KullaniciAdi>
            <Sifre></Sifre>
        </Baglanti>
        <Sorgu Sp="false">
            <Cumle> Select Count(P.ProductID) [Toplam Ürün Sayısı],PSC.Name [Kategori Adı] From Production.Product P Join Production.ProductSubCategory PSC On P.ProductSubCategoryID=PSC.ProductSubCategoryID Group By PSC.Name,SafetyStockLevel Having P.SafetyStockLevel<@StockLevel
            </Cumle>
            <Parametreler Nereden="Xml">
                <Parametre Ad="@StockLevel" Deger="10"/>
            </Parametreler>
        </Sorgu>
        <MailListesi MailGonder="true">
            <Mail>selim(at)buraksenyurt.com</Mail>
            <Mail>bsenyurt@csharpnedir.com</Mail>
        </MailListesi>
    </Rapor>
</Raporlar>
```

Burada basit olarak Product ve ProductSubCategory tablolarının Join ile birleştirilmiş hali üzerinden bir gruplama sorgusu gerçekleştirilmektedir. Sorgular StockLevel parametresinin değeri 10'dan küçük olanlar için yapılmaktadır. Parametrenin değerinin XML içerisinden geleceğini belirtmek için Nereden niteliğine XML değeri atanmıştır. Söz konusu rapor için MailListesinde belirtilen kişilere mail gönderme opsiyonu açık bırakılmıştır. Rapor, localhost isimli sunucudaki AdventureWorks veritabanı üzerinden SSPI ile gerçekleştirilen bir bağlantı üzerinden alınmaktadır. Artık OzelHandlerKullanimi adresi üzerinden KategoriBazliUrunler.rapx dosyasına bir talepte bulunulursa aşağıdaki gibi bir sonuç ortaya çıkacaktır.

![mk234_6.gif](/assets/images/2007/mk234_6.gif)

Görüldüğü gibi sorgu sonucu elde edilen rapor ekrana basit bir tablo olarak basılmıştır. Mail gönderme seçeneği aktif olduğu içinde Raporu Mail Olarak Gönder başlıklı linkte sayfa çıktısında yer almaktadır. Bu linke basıldığı takdirde sunucuya yeni bir talep daha gönderilecektir. Şimdilik sadece ilgili MailListesi elementinin içeriği değerlendirilmiştir. Aşağıdaki ekran görüntüsünde bu durum gösterilmektedir.

![mk234_12.gif](/assets/images/2007/mk234_12.gif)

Yeni bir test ile devam edelim. Bu kez raporun parametrelerine ait değerlerin QueryString yardımıyla geldiğini göz önüne alalım. Ayrıca raporun sonuçlarının Northwind veritabanı altında yer alan SalesByCategory isimli saklı yordam (stored procedure) üzerinden elde edildiğini düşünelim. MailGonderme seçeneği yine aktif olsun. Bu durumda rapx dosyasının içeriğinin aşağıdaki gibi tasarlanması gerekecektir.

KategoriBazliSatislar.rapx;

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Raporlar>
    <Rapor Id="1">
        <Baslik ArkaPlan="#CCBB77">Kategori Bazlı Satışlar</Baslik>
        <Baglanti Sspi="true">
            <Sunucu>localhost</Sunucu>
            <Veritabani>Northwind</Veritabani>
            <KullaniciAdi></KullaniciAdi>
            <Sifre></Sifre>
        </Baglanti>
        <Sorgu Sp="true">
            <Cumle>SalesByCategory</Cumle>
            <Parametreler Nereden="QueryString">
                <Parametre Ad="@CategoryName"/>
                <Parametre Ad="@OrdYear"/>
            </Parametreler>
        </Sorgu>
        <MailListesi MailGonder="false">
            <Mail>selim(at)buraksenyurt.com</Mail>
            <Mail>bsenyurt@csharpnedir.com</Mail>
        </MailListesi>
    </Rapor>
</Raporlar>
```

Buna göre tarayıcı penceresinden http://localhost/OzelHandlerKullanimi/KategoriBazliSatislar.rapx?CategoryName=Beverages&OrdYear=1999 adresi talep edilirse aşağıdakine benzer bir ekran görüntüsü ile karşılaşılacaktır.

![mk234_7.gif](/assets/images/2007/mk234_7.gif)

Elbette uygulamada pek çok hata göz ardı edilmektedir. Söz gelimi KategoriBazliSatislar.rapx için QueryString parametreleri kullanılmassa çalışma zamanı hataları (Run time exceptions) alınması çok doğaldır. Bu gibi hataların ele alınması bir başka deyişle kodun tekrardan revize edilerek düzenlenmesi gerekmektedir.

Gelelim Handler tipinin sunucu üzerinde nasıl ele alınabileceğine. Bunun için geliştirilen Handler tipini içeren assembly'ın Global Assembly Cache (GAC) içerisine atılması ve root web.config dosyası içerisindeki httpHandlers elementi altında bildirilmesi yeterlidir. Örneğimizde geliştirdiğimiz ReportHandlerLibraray.dll isimli assembly'ı GAC'a atmadan önce Visual Studio 2005 ortamında aşağıdaki gibi Strong Name ile imzalamamız gerekmektedir. (İstenirse bu imzalama işlemi komut satırından sn.exe aracı ilede gerçekleştirilebilir.)

![mk234_9.gif](/assets/images/2007/mk234_9.gif)

Bu işlemin ardından ReportHandlerLibrary'in gacutil.exe aracı yardımıyla yada sürükle bırak tekniği ile Windows\Assembly klasörüne atılarak GAC'a eklenmesi yeterlidir.

![dikkat.gif](/assets/images/2007/dikkat.gif)
Burada ReportHandlerLibrary projesinin Relase modda üretiminin yapılarak, Output Path olarak çıktının [C:\Windows\Microsoft.NET\Framework\v2.0.50727\](file:///C:/Windows/Microsoft.NET/Framework/v2.0.50727/) klasörünü işaret edecek şekilde düzenlenmesi ve daha sonra buradan GAC'a atılmasıda tercih edilebilir.

Sonuç olarak Windows\Assembly klasörü altına ReportHandlerLibrary'si aşağıdaki ekran görüntüsünde olduğu gibi eklenmiş olacaktır.

![mk234_10.gif](/assets/images/2007/mk234_10.gif)

Şimdi tek yapılması gereken bu assembly'ın root web.config içerisinde bildirilmesidir. Bunun için C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG klasöründe yer alan web.config dosyasına aşağıdaki gibi handler bildiriminin yapılması gerekmektedir.

![mk234_11.gif](/assets/images/2007/mk234_11.gif)

Bu işlemlerin ardından IIS üzerinden rapx uzantılı dosyalara gelecek olan taleplerin AspNetIsapi.dll tarafından ele alınacağınında belirtilmesi gerekir. Makalemizin başında OzelHandlerKullanimi isimli web sitesi için yaptığımız bu işlemin aynısı bu kez root web sitesi için benzer şekilde yapılarak gerçekleştirilmelidir. Sonuç itibariyle sunucu üzerinde Asp.Net 2.0 ile geliştirilen herhangibir web sitesi altında tasarlanacak olan tüm rapx uzantılı dosyalar RaporHandler sınıfı tarafından ele alınabilecektir.

> Kendi handler tipimizi root web.config dosyasında tanımladığımızda, web uygulaması içerisinde yer alan web.config dosyası içerisinde bırakılan handler tanımlamaları nedeni ile çalışma zamanı hataları (Run time exceptions) alınır. Bu nedenle makaledeki örnekte yer alan OzelHandlerKullanimi sitesindeki rapx dosyaları, root web.config'de yapılan handler bildirimleri sonrası çalışmayacaktır.
> ![mk234_13.gif](/assets/images/2007/mk234_13.gif)
> Sebep handler tanımlamasının hem root web.config hemde site içerisindeki web.config'de iki kez yapılmış olmasıdır. Bunu düzeltmek için OzelHandlerKullanimi sitesine ait web.config içerisinde yapılan handler bildirimlerini kaldırmak gerekmektedir.

Kendi HttpHandler tiplerimizi tasarlamak için ele aldığımız örnek senaryo çok daha fazla geliştirilebilir. Hatta bir web sunucusu üzerinde konuşlandırılacak olan rapx dosyalarının daha kolay hazırlanabilmesini sağlamak için görsel bir arabirimde geliştirilebilir. Sadece Rapx uzantılı dosyaları barındıracak olan bir Asp.Net uygulaması geliştirilerek tüm raporların tek bir merkezde toplanması sağlanabilir. Hatta bu işi dahada ileriye götürecek olursak, raporların yetki tabanlı olacak şekilde ele alınabilmesi için gerekli hazırlıklarda yapılabilir. Bu noktada, tasarlanan bu sistemin Reporting Service alt yapısına ne kadar benzediğini tartışmakta yarar vardır. Sistemin herhangibir veritabanına destek verecek şekilde ele alınması ise çok daha etkili bir raporlama arayüzü oluşturulmasına ön ayak olacaktır.

Bu makalemizde daha önceden ele aldığımız ancak geçerli bir ihtiyaç veya senaryo üzerine oturtamadığımız HttpHandler kavramını bir örnek üzerinde adım adım incelemeye çalıştık. Sizlerde farklı senaryoları göz önüne alarak ve makalede yer alan örneği dahada geliştirirek son derece etkili sonuçlara varabilirsiniz. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2007/OzelHandler.rar)