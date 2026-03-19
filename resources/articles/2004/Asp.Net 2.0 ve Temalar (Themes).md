---
title: "Asp.Net 2.0 ve Temalar (Themes)"
pubDate: 2004-09-03 12:00:00
categories:
  - Asp.Net 2.0
tags:
  - Asp.Net 2.0
  - .NET
  - ASP.NET
---

# Asp.Net 2.0 ve Temalar (Themes)
Değerli Okurlarım Merhabalar,

Bir internet sitesini önemli kılan özellikler, sayfalarının font, renk, nesne yerleşimleri ile kontrollere ait görsel özellikleri açısından birbirleriyle olan uyumluluklarıdır. Bu anlamda, çoğu zaman sayfalara CSS stilleri uygulanır ve web uygulamasındaki tüm görsel öğelerin aynı font, renk vb özelliklere sahip olması sağlanır. Asp.Net 2.0, bu tarz stillerin çeşitli seviyelerde uygulanmasını sağlayacak yeni bir özellik sağlamaktadır. Themes (Temalar). İşte bu makalemizde, Asp.Net 2.0'da yer alan temaların, hangi seviyelere, nasıl ve ne şekilde uygulanabileceğini incelemeye çalışacağız.

Bir tema, aslında işletim sistemlerinde uygulanan temalardan çok da farklı değildir. Pek çoğumuz kullandığı işletim sistemine temalar uygulamıştır. Uygulanan temalar, çoğunlukla sistemdeki font özelliklerini, pencerelere ait çeşitli renkleri, duvar kağıtlarını, sembolleri vb öğeleri görsel açıdan belirli bir standarta göre değiştirirler. Ancak internet projelerinin tasarlanması söz konusu olduğunda, temaların uygulanacağı seviyeler kavramı öncelikli olarak devreye girmektedir. Asp.Net 2.0 ile, makine seviyesinde, web uygulaması seviyesinde, sayfa seviyesinde ve sunucu kontrolü seviyesinde temalar uygulanabilmektedir.

![mk86_1.gif](images/mk86_1.gif)

Şekil 1 Tema Seviyeleri.

Biz bu makalemizde, temaların bu seviyelere nasıl uygulandığını incelemek amacıyla, basit bir tema oluşturacak ve bu temayı çeşitli seviyelere uygulayarak sonuçlarını incelemeye çalışacağız. Bu amaçla ilk olarak bir web uygulması açalım. Bu web uygulamamızın default.aspx sayfasını aşağıdaki ekran görüntüsüne sahip olacak şekilde oluşturalım. Bu sayfada bir kaç sunucu kontrolünü kullanacağız.

![mk86_3.gif](images/mk86_3.gif)

Şekil 2. Sayfamızın Varsayılan Hali.

Eğer sayfamızı bu haliyle tarayıcıda açarsak aşağıdaki ekran görüntüsünü elde ederiz.

![mk86_4.gif](images/mk86_4.gif)

Şekil 3. Sayfamızın ilk halinin tarayıcadaki görüntüsü.

Şimdi uygulamamıza bir tema ekleyeceğiz. Bunun için öncelikle solution'ımıza Theme isimli bir folder eklememiz gerekiyor. Bu klasör, bir önceki makalemizde bahsetmiş olduğumuz Code klasörü gibi, solution için özel anlam ifade eden bir yapıya sahiptir. Nitekim bu klasör altına, uygulamaya ait font, renk gibi stillerin nasıl olacağını belirten css dosyaları ve sunucu kontrollerine ait görsel ayarların yapıldığı skin dosyaları ile çeşitli resimler eklenebilir. Theme klasörünü oluşturduktan sonra kullanacağımız tema için bir isim belirlemeli ve bu isimle yeni bir alt klasör açmalıyız. Artık skin ve css dosyalarımızı bu klasör altında oluşturabiliriz. Böylece, uygulamamızda oluşturduğumuz alt klasör ismindeki temayı uygulayabiliriz. Klasör yapımız aşağıdaki şekildeki gibi olmalıdır. Bu hiyerarşik düzen aynı zamanda, bir solution için birden fazla tema oluşturulabileceğinide göstermektedir.

![mk86_5.gif](images/mk86_5.gif)

Şekil 4. Temamız için uygulamamız gereken klasör hiyerarşisi.

Şimdi sunucu kontrollerimizin nasıl görüneceğini belirleyeceğimiz özellikleri taşıyacak bir skin dosyası oluşturacağız. Bu dosyamızı Temam klasörü altında geliştireceğiz. Skinler özelikle temalar için tasarlanmış tipteki dosyalardır. Ancak, Asp.Net 2.0' ın Beta sürümünde bu dosya tipi tanımlanmamıştır. Dolayısıyla, klasörümüz altında Add New Item seçeneği ile skin eklemek istiyorsak, Text File tipini seçmemiz ve dosya ismimizin uzantısı skin olacak şekilde belirtmemiz gerekmektedir.

![mk86_6.gif](images/mk86_6.gif)

Şekil 5. Skin eklenmesi.

Bir skin dosyası, aslında sunucu kontrollerinin görsel açıdan özelleştirilmiş halini içerir. Dolayısıyla, bir web uygulamasına bir temanın uygulanması sonucu, bu uygulamdaki sunucu kontrolleri için skin dosyaları içerisindeki sunucu kontrollerinin görsel özellikleri uygulanır. Bunu aşağıdaki kod satırlarından daha iyi anlayabiliriz. Aşağıdaki kod satırları, Gorsel.skin dosyamıza ait olup çeşitli sunucu kontrollerine görsel açıdan zenginlik katacak özelliklere sahiptir.

```text
<asp:Label  Runat="server" Font-Bold="False" Font-Names="Forte" Font-Size="Large" ForeColor="Red"></asp:Label>
<asp:TextBox  Runat="server" Font-Names="Verdana" Font-Size="Small" ForeColor="#C000C0" BackColor="#FFC080" BorderColor="#C00000" BorderStyle="Solid" BorderWidth="2px" ></asp:TextBox>
<asp:Button  Runat="server"  Font-Bold="True" ForeColor="White" BackColor="OrangeRed" BorderColor="#C04000" BorderStyle="Solid" BorderWidth="3px" />
<asp:DropDownList  Runat="server" Font-Names="Bookman Old Style" Font-Size="Small" BackColor="#FFE0C0" Font-Italic="True"></asp:DropDownList>
<asp:ListBox  Runat="server" Width="100px" Height="70px" Font-Bold="True" ForeColor="Gold" BackColor="SteelBlue"></asp:ListBox>
```

Bu kodlara dikkatlice bakacak olursanız, Label, Button, TextBox, DropDownList, ListBox gibi kontrollerin çeşitli font, renk özellikleri ile tanımlanmış olduklarını göreceksiniz. İşte bu noktadan sonra, her hangibir tema seviyesinde, Temam temasını uygularsak, sunucu kontrolleri burada belirtilen görsel özelliklere sahip olacaktır. Bu dosyanın oluşturulması ile birlikte ilk temamızı hazırlamış oluyoruz. Şimdi bu temanın, bahsetmiş olduğumuz seviyeler için nasıl uygulanacağını inceleyelim.

Öncelikle sayfa seviyesinde bu temayı default.aspx için nasıl uygulayacağımıza bakalım. Bunun için, default.aspx sayfasının, page direktifine ait, Theme özelliğinden yararlanacağız. Tek yapmamız gereken, theme özelliğine, oluşturduğumuz temanın bulunduğu klasörün ismini atamak olacaktır. (Dolayısıyla temanın ismini.)

```text
<%@ Page Language="C#" CompileWith="Default.aspx.cs" ClassName="Default_aspx" Theme="Temam" %>
```

Şimdi default.aspx sayfamızın tarayıcıdaki görünümüne tekrar bakacak olursak, aşağıdaki ekran görüntüsünü elde ederiz.

![mk86_7.gif](images/mk86_7.gif)

Şekil 6. Temamızın sayfaya uygulanmış hali.

Görüldüğü gibi, default.aspx çalıştığında, page direktifindeki theme özelliğinin sahip olduğu değerin belirttiği klasör, web uygulamasının Themes klasörü altında aranmış ve bulunduktan sonra, ilgili skin dosyasındaki kontrollere ait font ve renk özellikleri yüklenmiştir. Bu örneğimizde temamız, web sayfası seviyesindedir. Yani, uygulamamıza aşağıdaki ekran görüntüsüne sahip ikinci bir form eklediğimizde, page direktifinin theme özelliğine temamızı atamadığımız sürece, sunucu kontrollerinin varsayılan görünümlerine sahip olduğunu farkederiz.

![mk86_8.gif](images/mk86_8.gif)

Şekil 7. Theme özelliği ayarlanmamış sayfanın tasarım hali.

![mk86_9.gif](images/mk86_9.gif)

Şekil 8. Theme özelliği ayarlanmamış sayfanın çalışan hali.

Bu durumda karşımıza şöyle bir soru çıkar. Temalarımızı, web uygulamasının içerdiği tüm sayfalara uygulamak istersek ne olacak? İşte bu sorunun cevabı her zaman olduğu gibi, uygulamanın tamamına ait ayarlamaları yapabileceğimiz web.config dosyası ile ilgilidir. Sorunun çözümü için, web.config dosyasına pages takısını aşağıdaki syntax'ı ile eklememiz gerekir.

```text
<system.web>
<pages theme="Temam"></pages>
.
.
.
</system.web>
```

Böylece, web uygulamasındaki tüm sayfaların, Temam isimli temayı uygulamasını sağlamış oluruz. Başka bir deyişle web uygulaması seviyesinde tema uygulatmış oluruz. Bu noktadan sonra default2.aspx sayfamızın çalışmasına bakarsak, sunucu kontrollerinin temayı uyguladığını görürüz.

![mk86_10.gif](images/mk86_10.gif)

Şekil 9. Uygulama seviyesinde tema kullanılmasının sonucu.

Diğer bir seviye olarakta, makine seviyesinin olduğundan bahsetmiştik. Makine seviyesinde yapılan tema uyarlamaları, sistemde geliştirilen tüm web uygulamalarına, bu uygulamalardaki tüm web sayfalarına ve bu sayfalardaki tüm sunucu kontrollerine uygulanmaktadır. Makine seviyesinde tema uyarlamasını gerçekleştirmek için, windows 2000 işletim sistemleri için C:\WINNT\Microsoft.NET\Framework\v2.0.40607\CONFIG adresinde yer alan machine.config dosyasında yine pages takısındaki theme özelliğini aşağıdaki syntax'ta olduğu gibi ayarlamamız gerekmektedir.

```text
<pages theme="Temam">
.
.
.
</pages>
```

Ancak burada dikkat etmemiz gereken özel bir nokta vardır. Machine.config doysasına ekelenen temanın (temaların) sistemdeki tüm web uygulamalarına uyarlanabilmesi için C:\Inetpub\wwwroot\aspnet_client\system_web\2_0_40607 \Themes adresi altında oluşturulması veya var olan temaların buraya kopyalanması gerekmektedir. Temam isimli klasörümüzü içeriği ile birlikte buraya kopyalarsak ve machine.config'de yukarıda bahsettiğimiz ayarlamayı yaparsak, sistemdeki tüm web uygulamalarının bu temayı uyguladığını görürüz.

![mk86_11.gif](images/mk86_11.gif)

Şekil 10. Temamızı global kullanıma açtığımız klasör.

Temalar ile ilgili önemli bir diğer noktada, üst seviyedeki temaların geçersiz kılınması işlemidir. Makine seviyesinde tema uyguladığımızı düşünürsek, herhangibir web uygulamasında bu temayı uygulamak istemiyebiliriz. Bu durumda tek yapmamız gereken, web.config dosyasındaki page takısının theme özelliğine ya başka bir temayı atamak yada boş bırakmak olacaktır.

```text
<pages theme=""></pages>
```

Böylece web uygulaması, makine seviyesinde belirtilen temayı geçersiz kılacaktır. Eğer sayfa seviyesinde, uygulama seviyesindeki yada makine seviyesindeki bir temayı geçersiz kılmak istersek, bu kez ilgili sayfanın page direktifindeki theme özelliğinde ya başka bir temayı belirtmemiz yada EnableTheming özelliğine false değerini vermemiz gerekecektir.

```text
<%@ Page Language="C#" EnableTheming="false"%>
```

EnableTheming özelliği, sunucu kontrolleri içinde geçerlidir. Dolayısıyla, bir temayı uygulayan bir web sayfasındaki herhangibir kontrol için tema özelliklerini geçersiz kılabiliriz. Bunun için tek yapmamız gerken sunucu kontrolünün EnableTheming özelliğine false değerini atamaktır. Örneğin, geliştirdiğimiz web uygulmasındaki Temam temasını göz önüne alalım. Eğer button kontrolünün EnableTheming özelliğini false yaparsak sayfamızı çalıştırdığımızda aşağıdaki ekran görüntüsünün oluştuğunu ve Button kontrolüne tema özelliklerinin uygulanmadığını görürüz.

```text
<asp:Button ID="Button1" Runat="server" Text="Göster" EnableTheming="false"/>
```

![mk86_12.gif](images/mk86_12.gif)

Şekil 11. Bir kontrol için temanın geçersiz kılınması.

Böylece Asp.Net 2.0 ile gelen tema tekniğinin nasıl kullanıldığını kısaca incelemiş olduk. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
