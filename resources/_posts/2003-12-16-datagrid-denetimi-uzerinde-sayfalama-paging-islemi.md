---
layout: post
title: "DataGrid Denetimi Üzerinde Sayfalama(Paging) İşlemi"
date: 2003-12-16 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - datagrid
  - paging
---
Bugünkü makalemizde, bir ASP.NET sayfasında yer alan DataGrid kontrolümüzde nasıl sayfalama işlemlerini gerçekleştireceğimizi göreceğiz. Uygulamamız, sql sunucusundaki veritabanımızdan bir tablo ile ile ilgili bilgileri ekranda gösterecek. Ancak çok sayıda kayıt olduğu için biz bunları, dataGrid kontrolümüzde 10’ar 10’ar göstereceğiz. Olayı anlayabilmek için doğrudan kodlama ile işe başlayalım diyorum. Öncelikle VS.NET ile bir ASP.NET Web Application oluşturalım ve WebForm1.aspx sayfamızın adını default.aspx olarak değiştirelim. Şimdi öncelikle bir DataGrid nesnesini sayfamıza yerleştirelim ve hiç bir özelliğini ayarlamayalım. Bunları default.aspx sayfasının html görünümünde elle kodlayacağız. Şu an için DataGrid kontrolümüze ait aspx dataGrid tag'ımızın hali şöyledir.

```text
<asp:DataGrid id="dgKitap" style="Z-INDEX: 101; LEFT: 56px; POSITION: absolute; TOP: 56px" runat="server"></asp:DataGrid>
```

Şimdi, code-behind kısmında yer alıcak kodları yazalım. Sql sunucumuza bir bağlantı oluşturacağız, Friends veritabanımızda yer alan Kitaplar tablosundaki satırları bir DataTable nesnesine yükleyip daha sonra dataGrid kontrolümüze bağlayacağız. Bunu sağlayacak olan code-behind kodlarımız ise şu şekilde olucaktır;

```csharp
SqlConnection conFriends;
SqlDataAdapter da;
DataTable dtKitaplar;

public void Baglan()
{
     conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
     da=new SqlDataAdapter("select ID,Kategori,Adi,Yazar,BasimEvi,BasimTarihi,Fiyat from kitaplar order by Adi",conFriends);
     dtKitaplar=new DataTable("Tum Kitaplar");
     da.Fill(dtKitaplar);
}
private void Page_Load(object sender, System.EventArgs e)
{
     Baglan();
     dgKitap.AutoGenerateColumns=false; /* DataGrid kontrolümüzde yer alıcak kolonları kendimiz ayarlayacığımız için bu özelliğe false değerini aktardık.*/
     dgKitap.DataSource=dtKitaplar; /*DataGrid kontrolümüze veri kaynağı olarak dtKitaplar isimli DataTable nesnemizin bellekte işaret ettiği veri kümesini gösteriyoruz.*/
     dgKitap.DataBind(); /* DataGrid kontrolündeki kolonları (bizim yazdığımız ve ayarladığımız kolonları) veri kaynağındaki ilgili alanlara bağlıyoruz.*/
} 
```

Şimdi sayfamızda yer alan DataGrid tag'ındaki düzenlemelerimizi yapalım. Burada Columns isimli taglar arasında, dataGrid kontrolümüzde görünmesini istediğimiz BoundColumn tipindeki sütunları belirleyeceğimiz tagları yazacağız. Bu sayede DataGrid kontrolüne ait DataBind metodu çağırıldığında, bizim bu taglarda belirttiğimiz alanlar DataGrid kontrolümüzün kolonları olacak şekilde ilgili veri alanlarına bağlanacak. Gelin şimdi buradaki düzenlemeleri gerçekleştirelim. Unutmadan, kendi DataGrid kolonlarınızı ayarlayabilmeniz için AutoGenerateColumns özelliğine false değerini aktarmanız gerekmektedir. Aksi takdirde ayarladğınız kolonların hemen arkasından, otomatik olarak DataTable'da yer alan tüm kolonlar tekrardan gelir. Yaptığımız son güncellemeleri ile DataGrid tag'ımızın yeni hali şu şekildedir.

![mk20_3.gif](/assets/images/2003/mk20_3.gif)

Burada görüldüğü gibi, DataGird kontrolümüzde görnümesini istediğim tablo alanlarını birer BoundColumn olarak, DataGrid tagları arasına ekledik. Kısaca bahsetmek gerekirse hepsi için, DataField özelliği ile tablodaki hangi alana ait verileri göstereceklerini, HeaderText özelliği ile sütun başlıklarında ne yazacağını, ReadOnly özelliği ile sadece okunabilir alanlar olduklarını belirliyoruz.Bu haliyle uygulamamızı çalıştırırsak aşağıdakine benzer bir ekran görüntüsü ile karşılaşırız.

![mk20_1.gif](/assets/images/2003/mk20_1.gif)

Şekil 1.Programın İlk Hali.

Görüldüğü gibi kitap listesi uzayıp gitmektedir. Bizim amacımız bu listeyi 10’arlı gruplar halinde göstermek. Bunun için yapılacak hareket gayet basit gözüksede ince bir teknik kullanmamızı gerektiriyor. Öncelikle dataGrid kontrolümüzün, bir takım özelliklerini belirlemeliyiz. Bu amaçla code-behind kısmında yer alan Page_Load procedure’unde bir takım değişiklikler yaptık.

```csharp
private void Page_Load(object sender, System.EventArgs e)
{ 
     if(!Page.IsPostBack) /* Sayfa ilk kez yükleniyorsa dataGrid'e ait özellikler belirlensin. Diğer yüklemelerde tekrardan bu işlemler yapılmasın istediğimiz için...*/
     {
          dgKitap.AllowPaging=true; /* DataGrid kontrolümüzde sayfalama yapılabilmesini sağlıyoruz.*/

          dgKitap.PagerStyle.Mode=PagerMode.NumericPages; /* Sayfalama sistemi sayısal olucak. Yani 1 den başlayıp kaç kayıtlık sayfa oluştuysa o kadar sayıda bir buton dizesi dataGrid kontrolünün en altında yer alıcak.*/

          dgKitap.AutoGenerateColumns=false; /* DataGrid kontrolümüzde yer alıcak kolonları kendimiz ayarlayacığımız için bu özelliğe false değerini aktadık.*/

     }
     Baglan();
     dgKitap.DataSource=dtKitaplar; /*DataGrid kontrolümüze veri kaynağı olarak dtKitaplar isimli DataTable nesnemizin bellekte işaret ettiği veri kümesini gösteriyoruz.*/
     dgKitap.DataBind(); /* DataGrid kontrolündeki kolonları (bizim yazdığımız ve ayarladığımız kolonları) veri kaynağındaki ilgili alanlara bağlıyoruz.*/
}
```

Şimdi kodumuzu yeniden çalıştırırsak bu kez DataGrid kontrolümüzüm alt kısmında sayfa linklerinin oluştuğunu görürüz.

![mk20_2.gif](/assets/images/2003/mk20_2.gif)

Şekil 2. Sayfa Linkleri

Ancak bu linklerden herhangibirine bastığımızda ilgili sayfaya gidemediğimizi aynı sayfanın gerisin geriye geldiğini görürüz. İşte pek çoğumuzun zorlandığı ve gözden kaçırdığı teknik burada kendini gösterir. Aslında her şey yolunda gözükmektedir ve sistem çalışmalıdır. Ama çalışmamaktadır. Yapacağımız bu sayfalama işlemini gerçekleştirecek bir metod yazmak ve son olarakta bu metodu DataGrid tag'ına yazacağımız OnPageIndexChanges olay procedure'ü ile ilişkilendirmektir. OnPageIndexChanges olayı DataGrid kontolünde yer alan sayfalama linklerinden birine basıldığında çalışacak kodları içerir. Bu durumda DataGrid tag'ımızın son hali aşağıdaki gibi olur.

![mk20_4.gif](/assets/images/2003/mk20_4.gif)

Şimdide code_behind kısmında Sayfa_Guncelle metodumuzu ekleyelim.

```csharp
Public void Sayfa_Guncelle(object sender , DataGridPageChangedEventArgs e)
{
     dgKitap.CurrentPageIndex=e.NewPageIndex; /* İşte olayı bitiren hamle. CurrentPageIndex özelliğine basılan linkin temsil ettiği sayfanın index nosu aktarılıyor. Böylece belirtilen sayfaya geçilmiş oluyor. Ancak iş bununla bitmiyor. Veritabanından verilerin tekrardan yüklenmesi ve dataGrid kontrolümüze bağlanması gerekli.*/

     Baglan();
     dgKitap.DataSource=dtKitaplar;
     dgKitap.DataBind();
} 
```

Şimdi uygulamamızı çalıştırısak eğer, sayfalar arasında rahatça gezebildiğimizi görürüz. Geldik bir makalemizin daha sonuna, bir sonraki makalemizde, yine DataGrid kontrolünü inceleyeğiz. Bu defa, kolonlar üzerinden sıralama işlemlerinin nasıl yapıldığını incelemeye çalışacağız. Umuyorumki hepiniz için faydalı bir makale olmuştur. Hepinize mutlu günler dilerim.