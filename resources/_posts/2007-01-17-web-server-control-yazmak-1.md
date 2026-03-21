---
layout: post
title: "Web Server Control Yazmak - 1"
date: 2007-01-17 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - asp.net
  - web-server-controls
---
Günümüz program geliştirme ortamların çoğu, geliştiricilerin (developers) daha kullanıcı dostu (user friendly) arabirimler tasarlayabilmeleri için sayısız kontrol içermektedir. Özellikle Asp.Net ortamından Asp.Net 2.0 ortamına geçildiğinde, web tabanlı uygulamaları geliştirirken kullanabileceğimiz kontrollerin sayısı oldukça artmıştır.(70den fazla kontrol) Buna rağmen var olan web sunucu kontrollerinin ihtiyacımızı karşılamadığı durumlar olabilir. Hatta ihtiyacımızı karşılayabilecek bir kontrol olmayabilir de. Bu gibi durumlarda geliştiriciler ya üçüncü parti bileşenleri satın alma ve kullanma yolunu tercih ederler yada ilgili sunucu kontrollerini (web server control) kendileri geliştirirler. Bu ilk makalemizde basit anlamda web sunucu kontrollerini nasıl geliştirebileceğimizi incelemeye başlayacağız.

Bir web sunucu kontrolünü sıfırdan yazmadan önce, var olan web kontrollerinin ne şekilde çalıştıklarını analiz etmekte fayda vardır. Web ortamı sunucu taraflı ve istemci taraflı çalışabilen kodların içerisinde yer aldığı bir teknolojidir. Asp.Net perspektifinden baktığımızda ise çoğunlukla sunucu taraflı kodlamadan ve kontrollerden bahsederiz. Hangi web teknolojisi olursa olsun istemcilerin tarayıcı pencerelerinde elde ettikleri çıktı her zaman için yorumlanabilir bir HTML çıktısı olacaktır. Örneğin aşağıdaki ekran görüntüsünde yer alan web sunucu kontrollerini göz önüne alalım.

![mk188_1.gif](/assets/images/2007/mk188_1.gif)

Sayfamızda Label, TextBox, DropDownList, Button, CheckBox, RadioButton gibi basit Asp.Net sunucu kontrolleri yer almaktadır. Bu sayfa herhangibir tarayıcı penceresi üzerinden talep edildiğinde ise aşağıdaki gibi bir HTML çıktısı elde edilir.

![mk188_2.gif](/assets/images/2007/mk188_2.gif)

Gördüğünüz gibi web sunucu kontrollerinin her birisi çalışma zamanında aslında HTML içeriğine çevrilirler. Bunu Render olarak da adlandırabiliriz. Dolayısıyla kendi Asp.Net Web sunucu kontrollerimizi yazarken düşünmemiz gereken ilk konu, tasarladığımız kontrolün istemci tarafına nasıl aktarılacağıdır. Bir başka deyişle istemci tarayıcı penceresinde gönderilecek HTML içeriğinin nasıl olacağıdır. Buda yazdığımız kontrolün Html çıktısının tarafımızca oluşturulmasına bağlıdır.

Asp.Net platformunda her sunucu kontrolü öyle yada böyle bir şekilde System.Web.UI isim alanı altında yer alan Control sınıfından türemektedir. Control sınıfı üzerinde geliştireceğimiz bir sınıf web sunucu kontrolü olarak ele alınabilir. Aslında var olan tüm web sunucu kontrolleri Control sınıfından türeyen WebControl sınıfından türer. WebControl sınıfından türetme yaparak kontrol geliştirme işlemlerini ilerleyen makalelerimizde ele almaya çalışacağız. System.Web.UI isim alanı altında yer alan Control sınıfının şu anda bizim için en önemli metodu Render metodudur. Render metodu parametre olarak HtmlTextWriter tipinden bir değişken almaktadır. Bu değişken yardımıyla kontrolün basılacağı sayfa üzerine HTML takılarını (Html tags) yazdırabiliriz. Aslında HtmlTextWriter sınıfı TextWriter isimli özet sınıftan (abstract class) türetilmiş bir sınıftır.

Özellikle windows tabanlı uygulama geliştirenler DateTimePicker kontrolünü bilirler. Ancak bu kontrolün benzeri bile, Asp.Net üzerinde yer almamaktadır. Çok basit olarak böyle bir web kontrolünü geliştirmek istediğimizi düşünelim. Kendi web kontrollerimizi geliştirirken eğer HTML çıktısının nasıl olacağını tam olarak kestiremiyorsak, kontrolümüzün içerisinde yer almasını istediğimiz elemanları diğer web sunucu kontrollerinden oluşturarak üretilen HTML çıktısını göz önüne alabiliriz. Bu biraz hileli bir yol olsada başlangıçta olayları daha kolay algılayabilmemizi sağlayacaktır. Tarih formatındaki bir kontrol temel olarak gün, ay ve yıl gibi bilgileri taşıyabilir. Bunları birer DropDownList olarakda düşünebiliriz. Bu listelerin başında ise birer Label ile global halede getirilebilecek gün, ay, yıl kelimeleride yer alabiliriz. (Global hale getirmekten kastımız ise içeriğin farklı dillere desteğinin olmasıdır. Yani localization) Tam olarak tasvir etmek istediğimiz bileşen aşağıdakine benzer olacaktır.

![mk188_3.gif](/assets/images/2007/mk188_3.gif)

Burada kullanılan Label bileşenleri Html çıktısına birer span takısı (span tag) içerisinde alınmaktadır. DropDownList kontrolleri ise birer select elementi olarak geçecektir. DropDownList içerisindeki öğelerimiz ise, select elementinin içerisinde birer alt element olarak option takıları arasında gösterilecektir. Öyleyse istemci tarafına çıkartmamız gereken HTML içeriği aşağı yukarı bellidir. Peki bu içeriği kendi kontrolümüz içerisinde nasıl çizdireceğiz?

Her şeyden önce temel olarak Control sınıfından türeyen bir tipe ihtiyacımız olduğunu belirtmiştik. Eğer bu tipi bir sınıf kütüphanesi içerisinde (class library) ele alırsak, Visual Studio içerisindeki ToolBox'a ekleyebilir ve başka web projeleri içerisindede kullanılmasını sağlayabiliriz. O halde bir Class Library (sınıf kütüphanesi) projesi açarak işe başlayalım. Normal şartlarda kendi web sunucu kontrollerimizi barındıracak kütüphane tipi olarak Web Control Library tercih edebiliriz. Bu proje tipi içerisinde bir web kontrolü örneği, gerekli nitelikleri (attributes) ve referansları (örneğin System.Web.dll gibi) ile birlikte hazır bir şablon olarak gelmektedir. Ancak web kontrol geliştirme sanatını temelinden öğrenmeye çalıştığımızdan, standard bir Class Library üzerinden devam etmek çok daha öğretici olacaktır. Bu nedenle makalemizde Web Control Library seçeneğini şu an için kullanmayacağız.

Yazdığımız web sunucu kontrollerini bir sınıf kütüphanesi içerisinde tutarsak, Visual Studio IDE'si içerisinde yer alan ToolBox bölümüne ekleyebilir ve başka web projeleri içerisinden de kullanabiliriz. Hatta aynı solution içerisindeki web projelerinde ilgili kontroller otomatik olarak ToolBox içerisinde görünecektir.

Sınıf kütüphanemiz içerisinde TarihKontrolum isimli bir sınıf (class) geliştireceğiz. Yanlız bu sınıfın System.Web.UI isim alanı içerisindeki Control sınıfından türetilebilmesi için, sınıf kütüphanemizin System.Web.dll assembly'ına açık bir referansta bulunması gerekmektedir.

![mk188_4.gif](/assets/images/2007/mk188_4.gif)

Referansımızı ekledikten sonra sınıfımızı Control sınıfından türetebiliriz. Eklenen referans, sınıfımız içerisinde web ortamına (örneğin ViewState'ler, post-back olayları) müdahele etmemizide sağlayacaktır.

```csharp
using System;
using System.Web.UI;

namespace BenimWebKontrollerim
{
    public class TarihKontrolum:Control
    {
    }
}
```

Tasarlamaya çalıştığımız web kontrolünün bazı parçaları otomatik olarak hem tasarım zamanında (design time) hemde çalışma zamanında (run time) değiştirilebilmelidir. Eğer sınıfımız bu tip değişebilir değerleri barındıracaksa ve bunların desteğini hem tasarım zamanında hem geliştirme zamanında sağlayacaksa, nasıl bir üyeye ihtiyacımız olabilir? Cevap oldukça basittir. Özellik (Property). O halde sınıfımızı aşağıdaki gibi geliştirelim.

![mk188_7.gif](/assets/images/2007/mk188_7.gif)

```csharp
using System;
using System.Web.UI;

namespace BenimWebKontrollerim
{ 
    public class TarihKontrolum:Control
    {
        private string _gunMetin;
        private string _yilMetin;
        private string _ayMetin;

        public string GunMetin
        {
            get { return _gunMetin; }
            set { _gunMetin = value; }
        }
        public string AyMetin
        {
            get { return _ayMetin; }
            set { _ayMetin = value; }
        }
        public string YilMetin
        {
            get { return _yilMetin; }
            set { _yilMetin = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // Gün yazısını tutacak Label için Html tarafına bir span elementi atılır.
            writer.Write("<span id='lblGun'>" + GunMetin + "</span>");
            // iki boşluk bırakılır.
            writer.Write("  ");
            // Gün değerlerini tutacak (1...31 e kadar) select elementi oluşturulur.
            writer.Write("<select name='Gun' id='Gun'>");
            // Select elementi içerisinde, her gün için bir option elementi oluşturulur value ve text değerleri i olarak set edilir.
            for (int i = 1; i <= 31; i++)
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            // Açılan Select takısı kapatılır.
            writer.Write("</select>"); 
            // İki boşluk bırakılır
            writer.Write("  ");
            // Ay metnini taşıyan Label kontrolü için span takısı atılır.
            writer.Write("<span id='lblAy'>" + AyMetin + "</span>");
            // İki boşluk bırakılır
            writer.Write("  ");
            // Aylar için gerekli select elementi oluşturulur.
            writer.Write("<select name='Ay' id='Ay'>");
            // Her bir Ay için select elementi içerisine birer option elementi açılır.
            for (int i = 1; i <= 12; i++)
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            // select elementi kapatılır.
            writer.Write("</select>");
            // İki boşluk bırakılır.
            writer.Write("  ");
            // Yil metni için span elementi oluşturulur.
            writer.Write("<span id='lblYil'>" + YilMetin + "</span>");
            // İki boşluk bırakılır
            writer.Write("  ");
            // Yillar için select elementi oluşturulur
            writer.Write("<select name='Yil' id='Yil'>");
            // 1900 ile 2050 arası tarih aralığındaki her bir yıl için select elementi içerisine birer option elementi açılır.
            for (int i = 1950; i <= 2050; i++)
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            // select elementi kapatılır.
            writer.Write("</select>");
            base.Render(writer);
        }
    }
}
```

Sınıfımızın belkide en can alıcı noktası Render metodunun içeriğidir. Burada dikkat ederseniz, kontrolümüzün HTML çıktısını HtmlTextWriter nesnesi yardımıyla en başından oluşturuyoruz. Label'larımız için birer span ve liste kutularımız içinde birer select elementi. Bu noktadan sonra sınıf kütüphanemiz (class library) içerisinde geliştirdiğimiz TarihKontrolum isimli bileşenin, herhangibir web projesindeki herhangibir sayfanın tasarım anında ToolBox'a eklendiğini görebiliriz.

> Geliştirilen web sunucu kontrollerini içerisinde barındırdan kütüphaneleri, private assembly şeklinde kullanabileceğimiz gibi (yani web uygulaması ile birlikte taşınacak şekilde), shared (public) olarak GAC'a (Global Assembly Cache) atıp kullanabilirizde. GAC üzerinde tutmamız halinde Assembly'ın bir strong name'e sahip olması gerektiğini unutmayalım.

![mk188_5.gif](/assets/images/2007/mk188_5.gif)

Şimdi bu kontrolü herhangibir web sayfası üzerine sürükleyip bırakalım. Bu durumda sayfamızın ekran görüntüsü aşağıdaki gibi olacaktır.

![mk188_6.gif](/assets/images/2007/mk188_6.gif)

Sayfanın kaynak tarafına geçtiğimizde ise kontrol içerisinde çizilen liste kutuları veya başlıkların hiç birisinin gözükmediğini, bunun yerine asp.net sunucu kontrollerine benzer tek bir elementin oluşturulduğunu görebiliriz. Bu nokta, kontrol geliştirme metodolojisinde bilgi saklama (informatin hiding) olarak adlandırılmaktadır. Öyleki, geliştirdiğimiz sunucu kontrolünün arka tarafında olanlar sayfa tasarımcısından gizlenemiştir. Bu sayfa tasarımcılarının ilgili kontrolleri daha kolay kullanabilmesini ve kod karmaşasından uzaklaşılmasını sağlar. Diğer yandan geliştiriciler (developers) bizim kontrolümüzü türeterek genişletebilirler.

> Geliştiricilerin bizim yazdığımız kontrolleride genişletebilmeleri için kontrol içerisinde var olan üyelerin, virtual (sanal) olarak tanımlanması düşünülebilir. Böylece türetilen tipler isterlerse bu üyeleri override (ezip) ederek kendi istedikleri biçimde çalışmalarını sağlayabilirler. Lütfen hatırlayalım, bu konular OOP (Object Oriented Programming - Nesneye Dayalı Programlama) temellerindendir.

Tekrar konumuza dönecek olursak.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="BenimWebKontrollerim" Namespace="BenimWebKontrollerim" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <cc1:tarihkontrolum id="TarihKontrolum1" runat="server"></cc1:tarihkontrolum>
    </form>
</body>
</html>
```

Sayfanın arka planına baktığımızda dikkati çeken ilk noktalardan birisi BenimWebKontrollerim.dll'ine ait referans bilgisinin Register direktifi ile sayfaya eklenmiş oluşudur. Register elementi içerisinde yer alan TagPrefix niteliği, TarihKontrolum bileşeninin sayfaya eklenmesi halinde kullanılacak takı ön ekini belirtmektedir. Hatırlayacağınız gibi kontrol sınıfımız içerisine GunMetin,AyMetin ve YilMetin isimli özellikler dahil etmiştik. Bu özellikler, kontrolümüz seçildiğinde properties penceresinde aşağıdaki gibi görünecektir.

![mk188_10.gif](/assets/images/2007/mk188_10.gif)

Bu özelliklere örnek değerler atayıp sayfamızı tarayıcı penceresinde açtığımızda, kontrolümüzün aşağıdaki ekran görüntüsünde olduğu gibi eklendiğini ve HTML çıktısının Render metodunda tasarladığımız şekilde üretildiğini görürüz.

Sayfa Çıktısı;

![mk188_8.gif](/assets/images/2007/mk188_8.gif)

Çalışma zamanında oluşturulan HTML çıktısı ise aşağıdakine benzer olacaktır.

![mk188_9.gif](/assets/images/2007/mk188_9.gif)

Artık elimizde kendi yazdığımız bir web sunucu kontrolü var. Bunu elbetteki biraz daha fazla geliştirmemiz gerekecek. Örneğin formumuz üzerine bir button ekleyip sayfanın istemciden sunucu tarafına gitmesine neden olacak bir işlem yaparsak (post-back) liste kutularının içeriğinin başlangıç değerlerine set edildiğini görürüz.

Dikkat ettiyseniz değerleri değiştirmemize rağmen gün, ay ve yıl değerleri ilk değerlerine set edilmiştir. Bunun sebebi son derece doğaldır. Geliştirdiğimiz web kontrolü, kullanıldığı web sayfasının bir parçasıdır. Sayfa ilk talep edildiğinde oluşturulur ve render edilen içeriğe HTML çıktıları dahil edilir. Bundan sonra zaten sunucu tarafında, Asp.Net sayfasına ait nesne örneği yok edilmektedir. Bu bir Asp.Net sayfasının yaşam döngüsünün (life cycle) doğal sonucudur. Dolayısıyla sayfa içerisindeki kontrollerde Dispose edilecektir.

O halde ikinci talepte, yani post-back işleminden sonra, aynı işlemler tekrarlanacaktır. Sayfaya ait nesne örneği ve içerideki tüm nesne örnekleri tekrardan oluşturulacak ve özellikle kontrollerin içerisinde yer alan özellikler de ilk değerlerine set edilecektir. Dolayısıyla kontrolümüzün, post-back işlemleri sonrasında bir şekilde istemcinin seçtiği liste öğelerini hatırlaması ve içeride işlemesi gerekecektir. İşte bu kontrolümüzü daha fazla geliştireceğimiz (geliştirmemiz gerektiği) anlamına gelmektedir. Diğer taraftan, geliştirdiğimiz kontrolün tasarım zamanındaki yeteneklerinide arttırabiliriz. Tüm bu gerekenleri yazı dizimizin ikinci makalesinde ele almaya çalışacağız.

Bu makalemizde bir web sunucu kontrolünü geliştirmek için gereken ilk adımları attık. Yanımıza kar olarak kalanları kısaca aşağıda maddeler halinde bulabilirsiniz.

Şu Ana Kadar Hatırda Kalanlar

Kullanıcı tarafından bir web kontrolü geliştirmek nihayetinde, System.Web.UI.Control sınıfından bir sınıf türetmektir. (Sonradan bu işlem için sadece WebControl sınıfını tercih edeceğiz.)

Özel olarak yazılacak bir web kontrolünü iki taraflı olarak düşünmek gerekir. İstemci tarafından ve sunucu tarafından. İstemci tarafında düşünülecek olanlar, kontrolün üreteceği HTML çıktısının nasıl olması gerektiğidir. Sunucu tarafında düşünülmesi gerekenler ise, istemcide seçilen verilerin nasıl hatırlanacağını ve istemci için gerekli HTML içeriğinin nasıl hazırlanacağıdır.

Kullanıcı tarafından geliştirilen sunucu kontrolleri aynı zamanda, Html çıktısının hazırlanması, viewstate ve post-back işlemlerinin ele alınması gibi kavramların, sayfa geliştiricisinden soyutlanmasına yardımcı olur. (information hiding)

Bir web sunucu kontrolünü, birden fazla web projesinde hatta Visual Studio IDE'si içerisinden kullanabilmek için bir class library içerisinde tutulmasında fayda vardır. Ortak kullanım amacıyla bu kütüphane GAC (Global Assembly Cache) üzerinde de tutulabilir.

Geliştirilen web sunucu kontrolünün, tasarım veya çalışma zamanında değiştirilebilecek üyeleri var ise bunları birer özellik olarak tasarlamak gerekir.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.