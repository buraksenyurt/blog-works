---
title: "DataGrid Denetimi Üzerinde Sıralama(Sorting) İşlemi"
pubDate: 2003-12-17 10:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - Performance
  - DataTable
---

# DataGrid Denetimi Üzerinde Sıralama(Sorting) İşlemi
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, bir Web Sayfası üzerinde yer alan DataGrid kontrolü üzerinde tıklanan kolon başlığına gore sıralama işleminin manuel olarak nasıl yapılacağını işleyeceğiz. Konu teknik ağırlığa sahip olduğu için hemen kodlara geçmek istiyorum.

Uygulamamız, C# ile yazılmış bir Web Application. Bir adet herhangibir özelliği belirlenmemiş DataGrid kontrolü içermekte. Aspx sayfamızın kodlarına göz atıcak olursak, DataBound tagları içerisinde yer alan SortExpression ifadeleri ve DataGrid tagında yer alan, OnSortCommand ifadesi bizim anahtar terimlerimizdir. SortExpression ifadesi, kolon başlığına tıklandığında ilgili veri kümesinin hangi alan adını göz önüne alacağını belirlemek için kullanılır. OnSortCommand değeri ise, SortExpression ifadesinin işlenerek sıralamanın yapılacağı kodları içeren procedure adına işaret etmektedir. Bu bilgiler ışığında izleyeceğimiz yol şudur;

1- DataBound tagları içinde SortExpression değerlerini belirlemek.

2- DataGrid tagı içinde, OnSortCommand olayı için metodu belirlemek.

3- OnSortCommand olayı için ilgili metodu geliştirmek.

Şimdi öncelikle default.aspx sayfamızın içeriğine bir bakalım.

![mk21_4.gif](images/mk21_4.gif)

Şimdi ise code-behind kısmında yer alan default.aspx.cs dosyamızın içeriğine bir bakalım.

```csharp
SqlConnection conFriends;
SqlDataAdapter da;
DataTable dtKitaplar;
DataView dvKitaplar;
/* Sql sunucumuzda yer alan Friends isimli veritabanına bağlanıyoruz. Buradan Kitaplar isimli tablodaki verileri SqlDataAdapter nesnemiz ile alıp dataTable nesnemizin bellekte işaret ettiği yere aktarıyoruz. Daha sonra ise dataTable nesnemizin defaultView metodunu kullanarak, dataView nesnemizi varsayılan tablo görünümü ile dolduruyoruz. Eğer sayfalarımızda sadece görsel amaçlı dataGrid'ler kullanacaksak yada başka bir deyişle bilgilendirme amaçlı veri kümelerini sunacaksak DataView nesnelerini kullanmak performans açısından fayda sağlıyacaktır.*/

public void Baglan()
{
     conFriends =new SqlConnection("Data source=localhost;integrated security=sspi;initial catalog=Friends");
     da=new SqlDataAdapter("Select ID,Adi,Yazar,BasimEvi,Fiyat From Kitaplar",conFriends);
     dtKitaplar=new DataTable("Kitap Listesi");
     da.Fill(dtKitaplar);
     dvKitaplar=dtKitaplar.DefaultView;
     DataGrid1.AutoGenerateColumns=false; /* DataGrid nesnemizin içereceği kolonları kendimiz belirlemek istediğimizden AutoGenerateColumns özelliğine false değerini atadık.*/
     DataGrid1.AllowSorting=true; /* AllowSorting özelliğine true değerini aktardığımızda, DataGrid'in başlık kısmında yer alan kolon isimlerine tıkladığımızda bu alanlara göre sıralama yapabilmesini sağlamış oluyoruz. */
}
/* Sirala isimli metodumuz, DataGrid tagında OnSortCommand için belirttiğimiz metoddur. Bu metod ile , bir kolon başlığına tıklandığında yapılacak sıralama işlemlerini belirtiyoruz. Bu metod, DataGridSortCommandEventArgs tipinde bir parametre almaktadır. Bu parametremizin SortExpression değeri, tıklanan kolon başlığının dataGrid tagında,bu alan ile ilgili olan DataBound sekmesinde yer alan SortExpression ifadesine atanan değerdir. Biz bu değeri alarak DataView nesnemizin Sort metoduna gönderiyoruz. Böylece DataView nesnesinin bellekte işaret ettiği veri kümesini e.SortExpression özelliğinin değerine göre yani seçilen alana göre sıralatmış oluyoruz. Daha sonra ise yaptığımız işlem DataGrid kontrolümüzü tekrar bu veri kümesine bağlamak oluyor.*/

public void Sirala(object sender,DataGridSortCommandEventArgs e)
{
     lblSiralamaKriteri.Text="Sıralama Kriteri : "+e.SortExpression.ToString();
     dvKitaplar.Sort=e.SortExpression;
     DataGrid1.DataSource=dvKitaplar;
     DataGrid1.DataBind(); 
}
private void Page_Load(object sender, System.EventArgs e)
{
     Baglan();
     DataGrid1.DataSource=dvKitaplar;
     DataGrid1.DataBind();
}
```

Şimdi uygulamamızı çalıştıralım ve kolon başlıklarına tıklayarak sonuçları izleyelim. İşte örnek ekran görüntüleri.

![mk21_1.gif](images/mk21_1.gif)

Şekil 1. Kitap Adına gore sıralanmış hali.

![mk21_2.gif](images/mk21_2.gif)

Şekil 2. ID alanına gore sıralanmış hali.

![mk21_3.gif](images/mk21_3.gif)

Şekil 3. Yazar adına gore sıralanmış hali.

Geldik bir makalemizin daha sonuna, bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
