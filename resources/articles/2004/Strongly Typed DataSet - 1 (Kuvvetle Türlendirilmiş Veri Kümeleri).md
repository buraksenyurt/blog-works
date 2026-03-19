---
title: "Strongly Typed DataSet - 1 (Kuvvetle Türlendirilmiş Veri Kümeleri)"
pubDate: 2004-02-04 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - XML
  - Visual Studio
  - DataSet
  - DataTable
---

# Strongly Typed DataSet - 1 (Kuvvetle Türlendirilmiş Veri Kümeleri)
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde kuvvetle türlendirilmiş veri kümelerinin ne olduğunu ve nasıl oluşturulduklarını incelemeye çalışacağız. Kuvvetle türlendirilmiş veri kümelerini tanımlamadan önce, aşağıdaki kod satırının incelemekle işe başlayalım.

```csharp
textBox1.Text=dsMakale1.Tables["Makale"].Rows[3]["Konu"].ToString();
```

Bu satır ile, dsMakale isimli dataSet nesnemizin bellekte işaret ettiği veri bölgesinde yer alan DataTable'lardan Makale tablosuna işaret edenin, 4ncü satırındaki Konu alanının değeri alınarak, TextBox kontrolümüzün text özelliğine atanmaktadır. Şimdide aşağıdaki kod satırını ele alalım.

```csharp
textBox1.Text=rsMakale.Fields("Konu").Value
```

Bu ifade eski dostumuz ADO daki rsMakale isimli recordSet'i kullanarak, Konu isimli alanın değerine ulaşmıştır. Dikkat edicek olursanız bu iki ifade arasında uzunluk açısından belirgin bir fark vardır. İkinci yazım daha kolaydır. Zaten bu nedenle, ADO.NET'i öğrenen programcıların ilk başta en çok karşılaştıkları zorluk, bu kod yazımının uzunluğu olmuştur. Bununla birlikte, ADO.NET'in XML tabanlı bir mimariye sahip olması, karşımıza Kuvvetle Türlendirilmiş Veri Kümelerini çıkarmaktadır. Microsfot.NET mimarları, programlarımızda aşağıdakine benzer daha kısa ifadelerin kullanılabilmesi amacıyla, Kuvvetle Güçlendirilmiş Veri Kümeleri kavramını ADO.NET'e yerleştirmiştir.

```csharp
textBox1.Text=dsTypedMakale.Makale[3].Konu.ToString();
```

Bu ifade ilk yazdığımız ifadeye göre çok daha kısadır. Peki bu nasıl sağlanmıştır. dsTypedMakale isimli DataSet nesnemiz aslında Kuvvetle Türlendirilmiş Veri Kümemizin ta kendisidir. Bu noktada Kuvvetle Türlendirilmiş Veri Kümesi'nin ne olduğunu tanımlayabiliriz.

Bir Strongly Typed DataSet (Kuvvetle Türlendirilmiş Veri Kümesi), DataSet sınıfından türetilmiş, programcı tarafından belirtilen bir xml schema sınıfını baz alan, veri bağlantılarının özelleştirilip yukarıdaki gibi kısa yazımlar ile erişimlere imkan sağlayan, özelleştirilmiş bir DataSet sınıfıdır.

Bu geniş kavramı anlamak için elbette en kolay yol örneklendirmek olucak. Ancak bundan önce Kuvvetle Türlendirilmiş Veri Kümemizin nasıl oluşturacağımızı görelim. Bunun için elimizde iki yol var. Yollardan birisi, komut satırından XSD.exe XML Schema Defination Tool'unu kullanmak. Bu yol biraz daha zahmetli olmakla birlikte Visual Studio.NET'e olan gereksinimi kaldırdığı için zaman zaman tercih edilir. Diğer yolumuz ise Kuvvetle Türlendirilmiş Veri Kümemizi Visual Studio.NET ortamında oluşturmak. Yollardan hangisini seçersek seçelim, Kuvvetle Türlendirilmiş Veri Kümemizin oluşturulması için bize mutlaka bir xml schema dosyası (xsd uzantılı) gerekiyor. Çünkü Kuvvetle Türlendirilmiş Veri Kümemiz, bir xml schema dosyası baz alınarak oluşturulmaktadır. Şimdi dilerseniz komut satırı yardımıyla bir Kuvvetle Türlendirilmiş Veri Kümesinin nasıl oluşturulacağını inceleyelim. Bunun için öncelikle biraz kod yazmamız gerekecek. İzleyen kodlar ile, bir DataSet'i gerekli tablo bilgileri ile yükleyecek ve daha sonra bu DataSet'e ait xml schema bilgilerini, DataSet sınıfına ait WriteXmlSchema metodu ile bir xsd dosyasına aktaracağız. Örneğin basit olması amacıyla bir console uygulaması oluşturalım.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace TypeDataSet2
{
     class Class1
     {
          static void Main(string[] args)
          {
               SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
               SqlDataAdapter da=new SqlDataAdapter("Select * From Makale",conFriends);
               DataSet dsMakale=new DataSet("Makaleler");
               conFriends.Open();
da.FillSchema(dsMakale,SchemaType.Source,"Makale");
               conFriends.Close();
               dsMakale.WriteXmlSchema("Makaleler.xsd");
          }
     }
}
```

Yukarıdaki kodları kısaca açıklayalım. Burada Sql sunucumuzda yer alan Friends isimli veritabanına bağlanıyor ve Makale isimli tabloya ait schema bilgilerini DataSet nesnemize yüklüyoruz. Daha sonra ise, DataSet nesnemizin WriteXmlSchema metodu ile, DataSet'in yapısını xml tabanlı schema dosyası olarak (Makaleler.xsd) sisteme kaydediyoruz. İşte Kuvvetle Türlendirişmiş Veri Kümemizi bu xsd uzantılı schema dosyası yardımıyla oluşturacağız. Sistemizde uygulamamızı oluşturduğumuz yerdeki Debug klasörüne baktığımızda, aşağıdaki görüntüyü elde ederiz.

![mk50_1.gif](images/mk50_1.gif)

Şekil 1. XML Schema Dosyamız.

Şimdi.NET Framework'ün XSD.exe aracını kullanarak, bu schema dosyasından Kuvvetle Türlendirilmiş Veri Kümemizi temsil edicek sınıfı oluşturalım. Bunun için, komut satırında aşağıdaki satırı yazarız.

![mk50_2.gif](images/mk50_2.gif)

Şekil 2. Kuvvetle Türlendirilmiş Veri Kümesi sınıfının oluşturulması.

Burada görüldüğü gibi, xsd schema dosyamızdan, Kuvvetle Türlendirilmiş Veri Kümesi sınıfımız (Makaleler.cs) oluşturulmuştur. xsd aracındaki /d parametresi, sınıfımızın DataSet sınıfından türetileceğini belirtmektedir. Şimdi Debug klasörümüze tekrar bakıcak olursak, Kuvvetle Türlendirilmiş Veri Kümesi sınıfımızın oluşturulmuş olduğunu görürüz.

![mk50_3.gif](images/mk50_3.gif)

Şekil 3. Kuvvetle Türlendirilmiş Veri Kümesi Sınıfımız.

Peki bu oluşturduğumuz Kuvvetle Türlendirilmiş Veri Kümesi sınıfını uygulamamızda nasıl kullanacağız. Bu oldukça basit. Yeni DataSet nesnemizi, Makaleler.cs sınıfından türeteceğiz. Aşağıdaki kodu inceleyelim.

```csharp
using System;
using System.Data;

using System.Data.SqlClient;
namespace TypeDataSet2
{
     class Class1
     {
          static void Main(string[] args)
          {
               SqlConnection conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
               SqlDataAdapter da=new SqlDataAdapter("Select * From Makale",conFriends);
               conFriends.Open();
               Makaleler dsTypedMakale=new Makaleler(); /* Kuvvetle Türlendirilmiş Veri Kümesi sınıfımızdan bir DataSet nesnesi türetiyoruz.*/
               da.Fill(dsTypedMakale.Makale); /* SqlDataAdapter nesnemiz yardımıyla, yeni dataSet'imizdeki Makale isimli tablomuzu Sql sunucumuzda yer alan Makale isimli tablonun verileri ile yüklüyoruz. */
Console.WriteLine(dsTypedMakale.Makale[3].Konu.ToString()); /* Yeni DataSet nesnemizdeki Makale tablosunun 4ncü satırındaki Konu alanının değerine erişiyoruz. */
               conFriends.Close();
          }
     }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonu elde ederiz.

![mk50_4.gif](images/mk50_4.gif)

Şekil 4. Uygulamanın sonucu.

Görüldüğü gibi bir Kuvvetle Türlendirilmiş Veri Kümesi oluşturmak ve kullanmak son derece basit. Dilerseniz xsd aracı ile oluşturduğumuz Makaleler.cs isimli Kuvvetle Türlendirilmiş Veri Kümesi sınıfımızı ele alalım. Bu sınıfı oluşturduğunuzda mutlaka incelemenizi tavsiye ederim. Çok uzun bir dosya olduğunundan tüm kod satırlarına burada yer vermiyorum. Ancak dikkatimizi çekicek bir kaç üyeyi göstereceğim. Öncelikle sınıf tanımımıza bir göz atalım.

```csharp
public class Makaleler : DataSet
{
 // Bir takım kodlar
}
```

Daha öncedende söylediğimiz gibi Kuvvetle Türlendirilmiş Veri Kümesi sınıfları DataSet sınıfından türetilmektedir. Yeni dataSet sınıfımız, içereceği tablo, alan, kısıtlama, ilişki gibi bilgileri bir xsd dosyasından almaktaydı. Biz xsd dosyasını oluştururken, DataSet'e Makale isimli tablonun yapısını yüklemiştik. Bu durumda, Kuvvetle Türlendirilmiş Veri Kümesi sınıfımız içinde, Makale isimli tablomuzu belirten yeni bir DataTable üyesi kullanılır.

```csharp
private MakaleDataTable tableMakale;
```

Burada MakaleDataTable, DataTable sınfının özelleştirilmiş bir haline sunar.

```csharp
protected Makaleler(SerializationInfo info, StreamingContext context)
{
     string strSchema = ((string)(info.GetValue("XmlSchema", typeof(string))));
     if ((strSchema != null))
     {
          DataSet ds = new DataSet();

          ds.ReadXmlSchema(new XmlTextReader(new System.IO.StringReader(strSchema)));
               if ((ds.Tables["Makale"] != null))
               {
                    this.Tables.Add(new MakaleDataTable(ds.Tables["Makale"]));
               }              
...
}

...
public MakaleDataTable Makale
{
     get
     {
          return this.tableMakale;
     }

}

...
```

Görüldüğü gibi MakaleDataTable isminde yeni bir sınıfımız vardır. Bu sınıf Makale isimli tabloyu tüm elemanları ile tanımlar. Bunun için, bu sınıf DataTable sınıfından türetilir. Makale tablosundaki her bir alan bu sınıf içerisinde birer özellik haline gelir. Bununla birlikte bu yeni DataTable sınıfı, alan ekleme, alan silme gibi metodlar ve pek çok olayıda tanımlar. Diğer yandan pek çok DataTable metoduda bu sınıf içinde override edilir.

```csharp
public class MakaleDataTable : DataTable, System.Collections.IEnumerable
{
     private DataColumn columnID;
     private DataColumn columnKonu;
     private DataColumn columnTarih;
     private DataColumn columnAdres;
     ...
     public int Count
     {
          get
          {
               return this.Rows.Count;
          }
     }
     internal DataColumn IDColumn
     {
          get
          {
               return this.columnID;
          }
     }
     public MakaleRow this[int index]
     {
          get
          {
               return ((MakaleRow)(this.Rows[index]));
          }
     }
     public event MakaleRowChangeEventHandler MakaleRowChanged;
...
     public void AddMakaleRow(MakaleRow row)
     {
          this.Rows.Add(row);
     }
     public MakaleRow AddMakaleRow(string Konu, System.DateTime Tarih, string Adres)
     {
          MakaleRow rowMakaleRow = ((MakaleRow)(this.NewRow()));
          rowMakaleRow.ItemArray = new object[] {null,Konu,Tarih,Adres};
          this.Rows.Add(rowMakaleRow);
          return rowMakaleRow;
     }
...
}
```

Buraya kadar, komut satırı yardımıyla ve kod yazarak bir Kuvvetle Türlendirilmiş Veri Kümesinin nasıl oluşturulacağını gördük. Bu teknikteki adımları gözden geçirmek gerekirse izlememiz gereken yol şöyle olucaktır.

![mk50_5.gif](images/mk50_5.gif)

Şekil 5. Kuvvetle Türlendirilmiş Veri Kümesi Oluşturulma Aşamaları

Şimdide, Kuvvetle Türlendirilmiş Veri Kümesi sınıfını, Visual Studio.NET ortamında nasıl geliştireceğimizi görelim. Bu daha kolay bir yoldur. Öncelikle Visual Studio.Net ortamında bir Windows Uygulaması açalım. Daha sonra, Server Explorer penceresinden Makale isimli tablomuzu Formumuza sürükleyelim.

![mk50_6.gif](images/mk50_6.gif)

Şekil 6. Server Explorer ile Makale Tablosunun Forma Alınması

Bu durumda formumuzda otomatik olarak SqlConnection ve SqlDataAdapter nesnelerimiz oluşur.

![mk50_7.gif](images/mk50_7.gif)

Şekil 7. SqlConnection ve SqlDataAdapter otomatik olarak oluşturulur.

Şimdi SqlDataAdapter nesnemizin özelliklerinden Generate DataSet'e tıkladığımızda karşımıza aşağıdaki pencere çıkacaktır.

![mk50_8.gif](images/mk50_8.gif)

Şekil 8. DataSet oluşturuluyor.

Burada DataSet nesnemiz otomatik olarak oluşturulacaktır. DataSet'imize dsMakale adını verelim ve OK başlıklı butona basalım. Şimdi Solution Explorer'da Show All Files seçeneğine tıklarsak, Kuvvetle Türlendirilmiş Veri Kümesi sınıfımızın oluşturulduğunu görürüz.

![mk50_9.gif](images/mk50_9.gif)

Şekil 9. Kuvvetle Türlendirilmiş Veri Kümesi

Artık uygulamamızda bu sınıfı kullanabiliriz. İşte örnek kod satırlarımız.

```csharp
private void Form1_Load(object sender, System.EventArgs e)
{
     dsMakale mk=new dsMakale();
     sqlDataAdapter1.Fill(mk.Makale);
     textBox1.Text=mk.Makale[3].Konu.ToString();
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk50_10.gif](images/mk50_10.gif)

Şekil 10. Uygulamanın Sonucu.

Bu makalemizde, Kuvvetle Türlendirilmiş Veri Kümelerinin ne olduğunu, nasıl oluşturulacağını ve nasıl kullanılacağını gördük. İzleyen makalemizde bu konuya devam edicek ve Satır Ekleme, Satır Düzenleme, Satır Silme gibi işlemlerin nasıl yapılacağını inceleyeceğiz. Hepinize mutlu günler dilerim.
