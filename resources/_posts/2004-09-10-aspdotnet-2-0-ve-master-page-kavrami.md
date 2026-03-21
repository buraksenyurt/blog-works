---
layout: post
title: "Asp.Net 2.0 ve Master Page Kavramı"
date: 2004-09-10 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - asp.net
  - master-page
---
Bu makalemizde, Master Pages kavramına giriş yapacak ve web uygulamalarının geliştirmesinde yaşamımıza getirdiği kolaylıkları incelemeye çalışacağız. Internet sitelerini göz önüne aldığımızda, siteye ait sayfaların sıklıkla aynı şablonları kullandığını görürüz. Özellike, header, footer, navigasyon ve advertisement alanları, çoğunlukla siteye ait tüm sayfalarda aynı yerlerde kullanılır. Bu, siteye ait sayfaların standart olarak aynı görünümde olmasını sağlamakla kalmaz, değişen içeriğinde ortak bir şablon üzerinde durmasına imkan tanır. Peki Asp.Net 2.0' ın bu kullanım için getirdiği yaklaşıma gelene kadar, sayfalarda ortak olarak kullanılan ve tasarımsal olarak sayfa koordinatlarında aynı yerlerde yer alan bu unsurlar hangi teknikler ile oluşturulmuştur?

Html'in ilk zamanlarında, bu tarz işlemleri gerçekleştirmek için, ortak olan alanlar kopyalanarak diğer sayfalara yapıştırılırdı. Yada, ana şablonu ihtiva eden bir sayfadan diğer sayfalar "save as" metodu ile oluşturulur ve içerikleri değiştirilirdi. Bu tekniğin en büyük handikapı, şablondaki herhangibir değişikliğin diğer sayfalara yansıtılması sırasında ortaya çıkmaktadır. Nitekim 100' lerce alt sayfaya aynı şablonu uygulamışsak bu gerçekten büyük bir problemdir.

Çözüm Asp ile gelmiştir. Asp, include takılarını kullanarak, sayfalarda tekrar eden içeriklerin kolayca kullanılabilmesini ve değişikliklerin tüm sayfalarda görünebilmesini sağlamıştır. Ancak elbetteki include takısınında bir takım sorunları vardır. Bunlardan birisi, tasarım zamanında include takısının işaret ettiği içeriğin görünememesidir. Dolayısıyla sayfanın bütünün nasıl göründüğünü inceleyebilmek için mutlaka çalıştırmak gerekmektedir. Diğer yandan, include tekniği takılar üzerine kurulu olduğundan, özellikle açık unutulan takılar sayfalarda istenmeyen Html çıktılarının oluşmasına yol açmaktadır.

Asp.Net, bu tip ortak içeriklerin kullanılmasına daha güçlü ve etkin bir yaklaşımı getirmiştir. User Controls. Kullanıcı tanımlı kontroller, normal aspx içeriğine sahip olabilmekte ve.net mimarisinin güçlü özelliklerini kullanabilmektedir. Her ne kadar etkili bir teknik olsada, user control'ler içinde tek bir sorun öne çıkmaktadır. Tasarım zamanında, user control içeriğinin görülememesi.

Asp.Net 2.0, Master Page yaklaşımı ile, yukarıda bahsedilen dezavantajları ortadan kaldırmayı başarmıştır. Bir Master Page, uygulandığı diğer aspx sayfalarının nasıl görünmesi gerektiğine karar veren bir şablona gibidir. Ancak, sağladığı ContentPlaceHolder bileşeni sayesinde, Master Page'leri uygulayan diğer aspx sayfalarının, istenilen içeriğe sahip olmasınıda sağlamaktadır. En güzel yanı ise, normal bir aspx sayfası gibi tasarlanabilmesi, yani html, image, server control gibi üyeleri içerebilmesidir. Bunlara ek olarak, olay güdümlü programlama modelinide destekler. Dolayısıyla bir Master Page aslında bir aspx sayfasından farksızdır.

Ancak asıl fark, bir Master Page bir aspx sayfasına uygulandığında ortaya çıkar. Master Page'i uygulayan bir aspx sayfası tarayıcıda açıldığında tarayıcıya gelen sayfa, Master Page ile aspx sayfasının birleştirilmesi sonucu ortaya çıkan başka bir aspx sayfasıdır. Bu,.net framework'ün getirdiği partial class tekniği sayesinde gerçekleşebilmektedir. Bunun, Master Page'i uygulayan aspx sayfalarına getirdiği değişik kodlama etkileride vardır.

![mk91_1.gif](/assets/images/2004/mk91_1.gif)

Şekil 1. Master Page ve aspx sayfalarının ortak çalışma mimarisi.

Bu kısa açıklamalardan sonra, Master Page'lerin ne olduğunu ve nasıl kullanıldıklarını anlamak amacıyla basit bir örnek geliştirelim. İlk olarak, Visual Studio.Net 2005' te bir web sitesi açalım. Sitemize Master Page eklemek için tek yapmamız gereken, Solution'ımıza sağ tıklamak ve Add New Item'den gelen pencerede, Master Page tipini seçmektir. Master Page'ler master uzantılı dosyalardır.

![mk91_2.gif](/assets/images/2004/mk91_2.gif)

Şekil 2. Solution'a Master Page eklenmesi

Bu işlemin ardından Master Page'in standart olarak aşağıdaki gibi oluşturulduğunu görürüz.

![mk91_3.gif](/assets/images/2004/mk91_3.gif)

Şekil 3. Varsayılan Master Page.

İşte burada ContentPlaceHolder1 bileşenimiz, bu Master Page'i uygulayacak olan sayfaların serbestçe erişebilecekleri ve içerik oluşturabilecekleri alanları tanımlamaktadır. Elbetteki bir Master Page'in bu şekilde olması beklenemez. Bu nedenle Master Page'imizi aşağıdaki ekran görüntüsünde olduğu gibi tasarlayabiliriz. Dikkat edecek olursanız, Master Page'lerde, normal aspx sayfaları gibi tasarlanabilirler. Bir başka deyişle, Html kodları, aspx bileşenleri vb. içerebilirler.

![mk91_6.gif](/assets/images/2004/mk91_6.gif)

Şekil 4. Master Page sayfamızının tasarımı.

Burada standart olarak bir web sayfasının tasarlanmasından farklı bir işlem yapılmamıştır. En önemli nokta Master Page'i uygulayacak sayfaların içeriklerini yazabilecekleri ContentPlaceHolder bileşeninin kullanılmasıdır. Dilersek bir Master Page içinde, birden fazla ContentPlaceHolder bileşeninede yer verebiliriz. Master Page'in aspx kodlarına baktığımızda normal aspx sayfalarına göre en önemli değişik page direktifi yerine master direktifinin kullanılmasıdır. Master direktifi sayfanın bir Master Page olduğunu belirtmektedir.

```text
<%@ Master Language="C#" CompileWith="AnaSablon.master.cs" ClassName="AnaSablon_master" %>
```

ContentPlaceHolder bileşeni ise,

```text
<asp:ContentPlaceHolder ID="ContentPlaceHolder1" Runat="server"></asp:ContentPlaceHolder>
```

aspx kodları ile tanımlanır. Sayfanın dikkate değer bir özelliğide, form takılarını içermesidir. Nitekim biraz sonra göreceğimiz gibi bunun, Master Page'i uygulayan aspx sayfalarına etkisi vardır. Şimdi dilerseniz, yeni bir aspx sayfasına oluşturduğumuz Master Page'i nasıl uygulayacağımıza bir bakalım. Öncelikle, Add New Item iletişim kutusunu açalım ve dosya tipi olarak Web Form'u seçelim. Ardından, sayfamıza uygulamak istediğimiz Master Page'i seçebilmek amacıyla, Select Master Page kutucuğunu işaretleyelim.

![mk91_7.gif](/assets/images/2004/mk91_7.gif)

Şekil 5. Yeni bir web formun Master Page tabanlı oluşturulması.

Bu durumda Add buttonuna bastığımızda, sayfamıza uygulamak istediğimiz Master Page'i seçeceğimiz iletişim kutusu ekrana gelecektir.

![mk91_8.gif](/assets/images/2004/mk91_8.gif)

Şekil 6. Master Page'in seçilmesi.

Bu adımıda tamamladığımızda, default.aspx sayfamız aşağıdaki gibi oluşturulacaktır. Dikkat edecek olursanız, sadece Master Page'deki ContentPlaceHolder bileşeninin bulunduğu alan düzenlenebilir yapıdadır. Diğer kısımlar için düzenleme ve değişitirme gibi işlemleri gerçekleştirme imkanımız yoktur. Bu sayede web formunun, Master Page'in izin verdiği görünümde olması ve kendisine ayrılan alanda istediği içeriği oluşturmasına izni verilmiş olunur.

![mk91_9.gif](/assets/images/2004/mk91_9.gif)

Şekil 7. Master Page'in bir web formuna uygulanması sonucu.

Bu noktada, web formumuzun aspx kodlarına bakcak olursak sadece tek bir satırın olduğunu görürüz.

```text
<%@ Page Language="C#" MasterPageFile="~/AnaSablon.master" CompileWith="Default.aspx.cs" ClassName="Default_aspx" Title="Untitled Page" %>
```

Bu tek satır aslında çok şey ifade etmektedir. Herşeyden önce, MasterPageFile özelliği, sayfaya uygulanan Master Page'in yolunu belirtir. Bu, formun bir Master Page'i uyguladığını başka bir deyişle master page'den türetilerek üretildiğini gösterir. Kendi sınıfı ve code-behind dosyası vardır.

Eğer sayfadaki Content alanı içerisinde düzenleme yapmak istersek bunu şu an için gerçekleştiremediğimizi görürüz. Bunu sağlayabilmek için, Master Page'de yer alan ContentPlaceHolder bileşeninin, bu sayfada bir Content bileşeni ile eşleştirilmesi gerekmektedir. Bunun için, web formumuza aşağıdaki aspx kodlarını yazmamız yeterli olacaktır. Content bileşeninin, ContentPlaceHolderID özelliği, uygulanan Master Page'deki hangi ContentPlaceHolder bileşenini eşleştireceğini belirtmektedir. Bu özelliğin değeri, birden falza ContentPlaceHolder'ın, Master Page'i uygulayan sayfalarda eşleştirilmesinde önem kazanır.

```text
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="server">
</asp:Content>
```

Artık, içeriği istediğimiz gibi değiştirebiliriz. Sunucu elemanları, html kodları, resimler vesaire. Örneğin, bir Access tablosundan buradaki Content alanı içindeki bir GridView bileşenini dolduralım.

![mk91_10.gif](/assets/images/2004/mk91_10.gif)

Şekil 8. Content alanını bir aspx sayfası düzenlermişcesine kullanabiliriz.

Sayfamızın kodlarına bakacak olursak.

```text
<%@ Page Language="C#" MasterPageFile="~/AnaSablon.master" CompileWith="Default.aspx.cs" ClassName="Default_aspx" Title="Untitled Page" %>
    <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="server">
        <b>Mail Listemize Son Üye Olan Okurlar </b>
        <asp:AccessDataSource ID="AccessDataSource1"
Runat="server" DataFile="~/Data/veriler.mdb" SelectCommand="SELECT [ID], [Ad], [Soyad], [Mail] FROM [MailListesi]">
        </asp:AccessDataSource>
            <asp:GridView ID="GridView1" Runat="server" ForeColor="Black" BorderWidth="1px" BorderColor="Tan"
BackColor="LightGoldenrodYellow" DataSourceID="AccessDataSource1" DataKeyNames="ID"
AutoGenerateColumns="False" GridLines="None" CellPadding="2" AutoGenerateEditButton="True">
                <FooterStyle BackColor="Tan"></FooterStyle>
                <PagerStyle ForeColor="DarkSlateBlue" HorizontalAlign="Center" BackColor="PaleGoldenrod"></PagerStyle>
                <HeaderStyle Font-Bold="True" BackColor="Tan"></HeaderStyle>
                <AlternatingRowStyle BackColor="PaleGoldenrod"></AlternatingRowStyle>
                <Columns>
                    <asp:BoundField ReadOnly="True" HeaderText="ID" InsertVisible="False" DataField="ID"
SortExpression="ID"></asp:BoundField>
                    <asp:BoundField HeaderText="Ad" DataField="Ad" SortExpression="Ad"></asp:BoundField>
                    <asp:BoundField HeaderText="Soyad" DataField="Soyad" SortExpression="Soyad"></asp:BoundField>
                    <asp:BoundField HeaderText="Mail" DataField="Mail" SortExpression="Mail"></asp:BoundField>
                </Columns>
                <SelectedRowStyle ForeColor="GhostWhite" BackColor="DarkSlateBlue"></SelectedRowStyle>
            </asp:GridView> 
    </asp:Content>
```

Dikkat edecek olursanız, eklemiş olduğumu tüm kontroller ve diğer içerik, Content bileşenimize ait takılar içerisinde yer almaktadır. Diğer taraftan Content takıları dışında herhangibir içerik oluşturmamızı sağlayacak bileşenleri kullanma imkanımız yoktur. Bir diğer önemli özellikte, içeriğin herhangibir form takısı içerisinde yer almıyor oluşudur. Bunun sebebi, form takısının zaten Master Page'de uygulanmış olmasıdır. Dolayısıyla web formumuz, Master Page'den kalıtımsal olarak türetildiği için form takılarının burada kullanılmasına gerek yoktur.

Oluşan default.aspx sayfası aslında, Master Page'deki ContentPlaceHolder alanının içerik eklenmiş halidir. Bunun anlamı, çalışma zamanında içeriğin yer aldığı default.aspx sayfası ile AnaSablon.Master sayfasının birleştirilerek yeni bir default.aspx sayfasının oluşturulması ve son kullanıcıya sunulmasıdır. Eğer uygulamamızı çalıştıracak olursak tarayıcımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk91_11.gif](/assets/images/2004/mk91_11.gif)

Şekil 9. default.aspx'in çalışan hali.

Dilersek, bir web uygulaması içerisinde birden fazla Master Page tanımlayabilir ve istediğimiz sayfalara uygulayabiliriz. Örneğin, uygulamamıza AnaSablon2.Master isimli aşağıdaki form yapısına sahip yeni bir Master Page eklediğimizi farz edelim.

![mk91_12.gif](/assets/images/2004/mk91_12.gif)

Şekil 10. AnaSablon2.Master

Bu Master Page'i başka bir sayfaya kolayca uygulayabiliriz. Örneğin Kitap.aspx isimli bir web formu oluşturalım ve formumuzu AnaSablon2.master'dan türetelim. Böylece, uygulamamız içerisinde iki farklı Master Page yapısını kullanan sayfalara izin vermiş oluyoruz.

![mk91_13.gif](/assets/images/2004/mk91_13.gif)

Şekil 11. Bir uygulamada farklı Master Page'lerin kullanılması.

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kısaca, Master Page'lerin, uygulandıkları aspx sayfaları ile kombine bir şekilde nasıl çalıştıklarını incelemeye çalıştık. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.