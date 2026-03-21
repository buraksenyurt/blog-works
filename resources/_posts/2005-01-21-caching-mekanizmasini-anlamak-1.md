---
layout: post
title: "Caching Mekanizmasını Anlamak - 1"
date: 2005-01-21 08:00:00 +0300
categories:
  - aspnet
tags:
  - asp.net
  - caching
---
Bu makalemiz ile birlikte, web sayfalarının istemcilere daha hızlı bir şekilde ulaştırılmasında kullanılan tekniklerden birisi olan Caching (Ara Belleğe Alma) mekanizmasını incelemeye başlıyacağız. Akıllıca kullanıldığı takdirde web uygulamalarında istemcilere nazaran göreceli olarak performans artışına neden olan Caching (Ara Belleğe Alma) mekanizması, teorik olarak bir web sayfasının tamamının ya da bir parçasının ara belleğe alınarak belli bir süre boyunca burada tutulması prensibini temel alarak çalışır. Asp.Net uygulamaları söz konusu olduğunda bir sayfanın tamamını, belli bir veri kümesini veya sayfa üzerindeki herhangibir kontrolü ara belleğe alabiliriz. Buna göre Asp.Net uygulamalarındaki Caching (Ara Belleğe Alma) mekanizması aşağıdaki üç farklı tekniği destekler.

Asp.Net Uygulamaları İçin Caching (Ara Belleğe Alma) Teknikleri

1 - Output Caching (Çıktının Ara Belleğe Alınması)

2 - Data Caching (Verilerin Ara Belleğe Alınması)

3 - Fragment Caching (Parçaların Ara Belleğe Alınması)

Output Caching tekniğinde, bir aspx sayfasının tüm içeriği ara belleğe alınır ve belirli bir süre boyunca burada tutulur. Bu mekanizmanın çalışma biçimini aşağıdaki şekil ile daha kolay irdeleyebiliriz. Bu çalışma sistemi temel olarak diğer Caching mekanizmalarında da aynı şekilde işlemektedir. Değişen sadece ara belleğe alınan HTML görüntüsünün içeriği olacaktır.

![mk113_1.gif](/assets/images/2005/mk113_1.gif)

İlk olarak, birinci istemci web sunucusundan bir aspx sayfasını talep eder. Web sunucusu sayfanın ilişkili kodlarını çalıştırarak bir çıktı üretir ve bu sayfa için Output Caching (Çıktının Ara Belleğe Alınması) aktif ise üretilen HTML sonuçlarını istemciye gönderir. Hemen ardından gönderilen HTML içeriğini belirtilen süre boyunca (duration) tutmak üzere sunucu üzerindeki ara belleğe alır.

Şimdi ikinci bir istemcinin devamlılık süresi (Duration) içerisindeyken aynı sayfayı sunucudan talep ettiğini düşünelim. Bu durumda web sunucusu, talep edilen sayfanın Cache (Ara Bellek) üzerinde olup olmadığına bakar. Eğer ikinci kullanıcı talebini, Devamlılık Süresi (Duration) içerisindeki bir zaman diliminde iletmiş ise, web sunucusu istemciye sayfanın ara bellekteki hazır halini gönderir. Dolayısıyla sayfanın üretilmesinde çalıştırılan arka plan kodlarının hiç biri tekrardan yürütülmez. Bu elbetteki ikinci kullanıcının talep ettiği sayfa için daha kısa sürede cevap almasını sağlar.

Output Caching mekanizmasını bir aspx sayfasına uygulayabilmek için tek yapılması gereken OutputCache direktifinin sayfanın aspx kodlarının olduğu kısıma eklemek yeterlidir.

```text
<%@ OutputCache Duration="300" VaryByParam="None"%>
```

Burada, Duration özelliği ile sayfanın ilk talep edilişinden sonra 300 saniye (5 dakika) süre ile ara bellekte tutulacağını bildirmiş oluyoruz. VaryByName parametresini daha sonra inceleyeceğiz.

Şimdi dilerseniz, kısa bir örnek ile Output Caching mekanizmasının etkilerini incelemeye çalışalım. Basit bir web uygulaması geliştireceğiz. Bu uygulamada, Sql Sunucusunda yer alan Pubs isimli veri tabanındaki Authors isimli tabloya ait verileri bir DataGrid kontrolü içerisine aktaracağız.

default.aspx sayfamızın kodları;

```csharp
private void Baglan()
{
    con=new SqlConnection("data source=localhost;initial catalog=pubs;integrated security=SSPI");
    cmd=new SqlCommand("SELECT * FROM authors",con);
    con.Open();
}
private void Doldur()
{
    dr=cmd.ExecuteReader(CommandBehavior.CloseConnection);
    dgVeriler.DataSource=dr;
    dgVeriler.DataBind();
    dr.Close();
}

private void Page_Load(object sender, System.EventArgs e)
{
    Baglan();
    Doldur();
}
```

Uygulama çalıştığında ve default.aspx sayfasının çıktısı üretildiğinde bu sayfanın son halinin bir kopyasıda ara belleğe alınır. Yeni koypa ara bellekte 300 saniye (5 dakika) boyunca kalacaktır. Aynı sayfayı başka bir tarayıcı'da açtığımızda ya da aynı tarayıcı penceresinde iken ileri geri gittiğimizde sayfanın ara bellekte bulunan halini elde ederiz. Ancak bu örnekte dikkat edilmesi gereken bir nokta vardır. Bu durumu simule etmek için sayfayı refresh edelim. Bu durumda sayfanın içeriği aşağıdakine benzer olacaktır.

![mk113_2.gif](/assets/images/2005/mk113_2.gif)

Daha sonra Duration süresi dolmadan önce ilk satırın değerini Sql Sunucusu üzerinden değiştirelim. Bu durumda sayfada Output Caching uygulanmamış olsaydı, sayfayı bir sonraki talep edişimizde Load metodu çalışacak ve tablonun en güncel hali kullanıcıya sunulacaktı. Oysaki şimdi sayfayı talep ettiğimizde, yapılan değişikliğin görünmediğini farkederiz.

Değişiklik;

![mk113_3.gif](/assets/images/2005/mk113_3.gif)

Yeni bir tarayıcı ile aynı sayfayı talep ettiğimizde;

![mk113_4.gif](/assets/images/2005/mk113_4.gif)

Bu işleyiş bazen büyük bir risk olabilir. Örneğin, sayfanın daha dinamik olduğu durumlarda ve hatta olay metodlarına cevap vermesi gerektiği durumlarda sayfanın tamamının ara belleğe alınması, aslında istemcilere sayfanın en güncel halinin gönderilmesi demek değildir. Bu gibi durumlarda çoğunlukla ya veri kümelerinin ya da sayfa üzerindeki belirli kısımların (kontrollerin) ara belleğe alınması tekniği tercih edilir. Bu teknikleri bir sonraki makalemizde inceleyeceğiz.

Bir sayfanın tamamını ara belleğe aldığımızda, sayfanın arabellekte yer alacak birden fazla kopyasına ihtiyaç duyduğumuz durumlar söz konusu olabilir. Çoğunlukla QueryString kullanımı sırasında başka sayfalara çeşitli parametreleri ve değerlerini göndeririz. İşte VaryByParam özelliği sayesinde sayfanın, gönderdiğimiz her parametre için ayrı ayrı veya sadece belirli parametreler için ayrı ayrı kopyalarını ara bellekte tutabiliriz.

Bu durumu daha yakından inceleyebilmek için pubs veritabanındaki titles tablosunu kullancağımız bir önrek geliştirelim. Uygulamanın default.aspx sayfasında bu kez titles tablosunda yer alan her bir satır için title alanının değerlerini göstereceğiz. Kullanıcı her hangibir başlığa tıkladığında bu kitap ile ilgili detaylı bilgilerin olduğu başka bir sayfaya (detay.aspx) gidecek. Detay sayfası kitabın Primary Key değerini (title_id) QueryString parametresi olarak alacak. Bu durumda, detay sayfası için ara belleğe alma işlemini gerçekleştirebiliriz. Konuyu daha iyi anlayabilmek için örnek üzerinden adım adım gidelim. İlk olarak ana sayfamızın (default.aspx) aspx içeriğini aşağıdaki gibi değiştirmeliyiz.

```text
<asp:DataGrid id="dgVeriler" style="Z-INDEX: 101; LEFT: 56px; POSITION: absolute; TOP: 88px" runat="server" AutoGenerateColumns="False" Height="160px" Width="264px" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellSpacing="2" BackColor="#DEBA84" CellPadding="3">
<FooterStyle ForeColor="#8C4510" BackColor="#F7DFB5"></FooterStyle>
<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#738A9C"></SelectedItemStyle>
<ItemStyle ForeColor="#8C4510" BackColor="#FFF7E7"></ItemStyle>
<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#A55129"></HeaderStyle>
<PagerStyle HorizontalAlign="Center" ForeColor="#8C4510" Mode="NumericPages"></PagerStyle>
<Columns>
<asp:HyperLinkColumn DataNavigateUrlField="title_id" DataNavigateUrlFormatString="Detay.aspx?title_id={0}" DataTextField="title" HeaderText="Başlık"></asp:HyperLinkColumn> 
</Columns>
</asp:DataGrid>
```

Bu değişiklik sayesinde, dataGrid kontrolümüzden Detay.aspx sayfasına title_id alanının değerini parametre olarak gönderebileceğiz. Şimdi Detay.aspx sayfamının kodlarını aşağıdaki gibi geliştirelim.

```csharp
private void Baglan(string Id)
{
    con=new SqlConnection("data source=localhost;initial catalog=pubs;integrated security=SSPI");
    cmd=new SqlCommand("SELECT title_id,title,price,pubdate FROM titles WHERE title_id='"+Id+"'",con);
    con.Open();
}
private void Doldur()
{
    dr=cmd.ExecuteReader(CommandBehavior.CloseConnection);
    dgDetaylar.DataSource=dr;
    dgDetaylar.DataBind(); 
    dr.Close();
}

private void Page_Load(object sender, System.EventArgs e)
{ 
    string id=Request.Params["title_id"].ToString();
    Baglan(id);
    Doldur();
}
```

Burada ara belleğe alma işlemini Detay.aspx sayfası üzerinde uyguluyoruz. Çünkü detay.aspx, dinamik olarak içeriği gelen parametre değerine göre değişen bir sayfadır. Biz gelen parametre değerine göre üretilen sayfaların html çıktılarını ara belleğe almak istiyoruz. İşte bunun için yine aspx kodlarının başına OutputCache direktifini aşağıdaki gibi eklememiz gerekiyor.

```text
<%@ OutputCache Duration="300" VaryByParam="title_id"%>
```

Böylece detay.aspx sayfası her çalıştırıldığında gelen title_id değeri baz alınarak ara bellekte 300 saniye süreyle duracak olan html çıktılarının oluşturulmasına imkan sağlıyoruz. Olayı şu şekilde irdelersek daha anlaşılır olacaktır;

Parametre tabanlı OutputCache işlemi

İstemci title_id değeri BU1032 olan satırı detay.aspx sayfasından ister. Bu durumda detay.aspx sayfasının içeriği ara belleğe alınır. Bu sayfayı A olarak düşünelim.
detay.aspx in A koypası oluşturulur.

Başka bir İstemci title_id değeri MC2222 olan sayfayı talep eder. Bu durumda detay.aspx sayfasının yeni halinin kopyası ara belleğe alınır.
detay.aspx in B koypası oluşturulur.

Başka bir İstemci title_id değeri MC2222 olan sayfayı talep eder. Eğer duration süresi dolmadıysa talep edilen bu sayfanın çıktısı ara bellekte zaten olduğundan istemciye direkt olarak bu çıktı gönderilir.
detay.aspx in var olan B kopyası döner.

Başka bir İstemci title_id değeri BU1032 olan sayfayı talep eder. Eğer duration süresi dolmadıysa talep edilen bu sayfanın çıktısı ara bellekte zaten olduğundan istemciye direkt olarak bu çıktı gönderilir.
detay.aspx in var olan A kopyası döner.

Duration süreleri dolduğunda ise gelen talebe göre ara bellekteki sayfa çıktıları tekrardan oluşturulur.

Görüldüğü gibi VaryByParam özelliği ile, bir sayfanın kendisine QeuryString ile gelen parametrelerinin değerine göre farklı ara bellek görüntülerini elde edebilmekteyiz. Bazı durumlarda talep edilen sayfaya birden fazla parametre gönderildiği veya hiç parametre gönderilmeden çağrıldığıda olur. Bu durumu karşılamak için VaryByParam özelliğine * değerini atayabiliriz.

```text
<%@ OutputCache Duration="300" VaryByParam="*" %>
```

Her ne kadar bir sayfanın Output Cache tekniği ile ara belleğe alınması avantajlı görünsede, özellikle olay kodlamalı sayfaların işleyişinde bu kullanım sorunlara yol açabilir. Her şeyden önce sayfa içinde postback'e neden olan kodlamalar var ise, sayfa ilk çağrıldıktan sonra ara belleğe alınacağından bu kod satırları duration süresi sonlanana kadar yürütülmeyecektir. Diğer yandan zaman zaman, sayfalarımızın içeriği dinamik olarak değişmek zorunda kalabilir. Bu gibi durumlarda da sayfanın tamamının ara belleğe alınması iyi bir yöntem değildir. Çözüm, sayfanın belirli parçalarının veya sayfadaki herhangibir veri kümesinin (kümelerinin) ara belleğe alınmasıdır. Data Caching ve Fragment Caching tekniklerini bir sonraki makalemizde inceleyeceğiz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın.](/assets/files/2005/Caching1.rar)