---
title: "Strongly Typed DataSet - 2 (Kuvvetle Türlendirilmiş Veri Kümeleri)"
pubDate: 2004-02-05 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - Visual Studio
  - DataSet
  - DataTable
---

# Strongly Typed DataSet - 2 (Kuvvetle Türlendirilmiş Veri Kümeleri)
Değerli Okurlarım, Merhabalar.

Bir önceki makalemizde, Kuvvetle Türlendirilmiş Veri Kümelerinin ne olduğunu ve nasıl oluşturulduğunu incelemiştik. Bu makalemizde ise, bir türlendirilmiş veri kümesi yardımıyla satır ekleme, arama, düzenleme ve silme gibi işlemlerin nasıl yapılacağını inceleyeceğiz. Bu amaçla işe basit bir windows uygulaması ile başlıyoruz. Bu uygulamamızda kolaylık olması açısından Kuvvetle Türlendirilmiş Veri Kümemizi, Visual Studio.NET ortamında oluşturdum. Uygulamamızda, Makale isimli sql tablomuzu kullanacağız. Uygulamamızın formu izleyen şekildeki gibi olucak.

![mk51_1.gif](images/mk51_1.gif)

Şekil 1. Form Tasarımımız.

Kullanıcı, Ekle başlıklı butona tıkladığında, textBox kontrollerine girmiş olduğu değerlerden oluşan yeni satırımız, dataTable'ımıza eklenmiş olucak. Bu işlemi gerçekleştiren kodlar aşağıda verilmiştir.

```csharp
private void btnEkle_Click(object sender, System.EventArgs e)
{
     dsMakale.MakaleRow dr; /* Yeni bir satır tanımlanıyor. MakaleRow bir DataRow tipidir ve bizim dsMakale isimli Kuvvetle Türlendirilmiş Veri Kümesi sınıfımızda yer almaktadır.*/
     dr=mk.Makale.NewMakaleRow(); /* MakalNewRow ile dr isimli MakaleRow nesnemizin, Makale isimli DataTable'ımızda yeni ve boş bir satıra referans etmesi sağlanıyor. */
     dr.Konu=txtKonu.Text; /* Veriler yeni satırımızın ilgili alanlarına yerleştiriliyor. */
     dr.Tarih=Convert.ToDateTime(txtTarih.Text);
     dr.Adres=txtAdres.Text;
     mk.Makale.AddMakaleRow(dr); /* Son olarak AddMakaleRow metodu ile oluşturulan yeni satır dataTable'ımıza ekleniyor.*/
}
```

Bu teknikte dikkat edicek olursanız Kuvvetle Türlendirilmiş Veri Kümemize ait metodlar ve nesneler kullanılmıştır. Örneğin, bir DataTable nesnesi ile referans edilen bellek bölgesindeki tabloya, yeni bir satır eklemek için öncelikle yeni bir DataRow nesnesi tanımlarız. Sonra bu DataRow nesnesini boş bir satır olacak şekilde DataTable nesnemiz için oluştururuz. Daha sonra bu DataRow nesnesinde her bir alan için yeni verileri ekleriz. Son olarakta bu oluşturulan yeni DataRow nesnesini DataTable'ımıza ekleriz. Böylece, DataTable nesnemizin işaret ettiği tabloya yeni bir satır eklemiş oluruz. Bu teknik klasik olarak Türlendirilmemiş Veri Kümelerini kullandığımız örneklerde aşağıdaki kodlar ile gerçekleştirimektedir.

```csharp
DataRow drKlasik;
drKlasik=ds.Tables[0].NewRow();
drKlasik[1]=txtKonu.Text;
drKlasik[2]=Convert.ToDateTime(txtTarih.Text);
drKlasik[3]=txtAdres.Text;
ds.Tables[0].Rows.Add(drKlasik);
```

Gördüğünüz gibi teknik olarak iki yaklaşımda aynıdır. Ancak, aralarındaki farkı anlamak için kullanılan ifadelere yakından bakmamız yeterlidir. Herşeyden önce Kuvvetle Türlendirilmiş Veri Kümelerini kullanmak, kod yazarken programcıya daha anlaşılır gelmektedir. Dilerseniz, bu iki tekniği aşağıdaki tablo ile karşılaştıralım.

UnTyped Dataset Tekniği
Typed DataSet Tekniği

Yeni Bir Satır Tanımlamak

DataRow drKlasik;

dsMakale.MakaleRow dr;
![mk51_2.gif](images/mk51_2.gif)
Görüldüğü gibi intelliSense özelliği sayesinde, dsMakale dataSet'inden sonra yeni bir DataRow nesnesi oluşturmak için gerekli söz dizimini bulmak ve anlamak son derece kolay.

Tanımlanan Yeni Satırı Oluşturmak

drKlasik=ds.Tables[0].NewRow ();
![mk51_4.gif](images/mk51_4.gif)

dr=mk.Makale.NewMakaleRow ();
![mk51_3.gif](images/mk51_3.gif)
Görüldüğü gibi, Typed DataSet sınıfımız yardımıyla tanımlanan yeni satırı oluşturmak için kullanacağımız söz dizimi çok daha okunaklı ve anlamlı.

Alanlara Verileri Aktarmak

drKlasik[1]=txtKonu.Text;
drKlasik[2]=Convert.ToDateTime (txtTarih.Text);
drKlasik[3]=txtAdres.Text;
![mk51_6.gif](images/mk51_6.gif)

dr.Konu=txtKonu.Text;
dr.Tarih=Convert.ToDateTime (txtTarih.Text);
dr.Adres=txtAdres.Text;
![mk51_5.gif](images/mk51_5.gif)
Bir Type DataSet üzerinden oluşturduğumuz DataRow nesnesinin alanlarına veri aktarırken hangi alanların olduğunu kolayca gözlemleyebiliriz. Üstelik bu biçim, kodumuza daha kolay okunurluk ve anlam kazandırır.

Oluşturulan Satırın Tabloya Eklenmesi

ds.Tables[0].Rows.Add (drKlasik);
![mk51_8.gif](images/mk51_8.gif)

mk.Makale.AddMakaleRow (dr);
![mk51_7.gif](images/mk51_7.gif)
NewMakaleRow metoduna ulaşmak gördüğünüz gibi daha kolaydır.

Tablo 1. Türlendirilmiş (Typed) ve Türlendirilmemiş (UnTyped) Veri Kümelerinde Satır Ekleme İşlemlerinin Karşılaştırılması.

Her iki teknik arasında kavramsal olarak fark bulunmamasına rağmen, uygulanabilirlik ve kolaylık açısından farklar olduğunu görüyoruz. Bu durum verileri ararken, düzenlerken veya silerkende karşımıza çıkmaktadır. Şimdi uygulamamızda belli bir satırı nasıl bulacağımızı inceleyelim. Kullanıcı Bul başlıklı butona tıkladığında txtMakaleID textBox kontrolüne girdiği değerdeki satır bulunacak ve bulunan satıra ait alan verileri, formumuzdaki diğer textBox kontrollerine yüklencek. Arama işleminin PrimaryKey üzerinden yapıldığınıda belirtelim. Şimdi kodlarımızı yazalım.

```csharp
private void btnBul_Click(object sender, System.EventArgs e)
{
     dsMakale.MakaleRow drBulunan; /* Arama sonucu bulunan satırı tutacak DataRow nesnemiz tanımlanıyor. */
     drBulunan=mk.Makale.FindByID(Convert.ToInt32(txtMakaleID.Text)); /* FindByID metodu, Türlendirilimiş Veri Kümemizdeki tablomuzun Primary Key alanı üzerinden arama yapıyor ve sonucu drBulunan DataRow(MakaleRow) nesnemize atıyor. */
 
     if(drBulunan!=null) /* Eğer aranan satır bulunursa drBulunan değeri null olmayacaktır. */
     {
          txtKonu.Text=drBulunan.Konu; /* Bulunan satıra ait alan verileri ilgili kontrollere atanıyor. */
          txtTarih.Text=drBulunan.Tarih.ToString();
          txtAdres.Text=drBulunan.Adres;
     }
     else
     {
          MessageBox.Show("Aranan Makale Bulunamadı");
     }
}
```

Bu arama tekniğinin, türlendirilmemiş veri kümelerindeki arama tekniğine göre çok farklı bir özelliği vardır. Klasik yöntemde bir DataTable üzerinden arama yaparken Find metodunu kullanırız.

![mk51_11.gif](images/mk51_11.gif)

Şekil 3. Klasik Find metodu.

Görüldüğü gibi Find metodu, PrimaryKey değerini alır ve bu değeri kullanarak, tablonun PrimaryKey alanı üzeriden arama yapar. Oysa örneğimizde kullandığımız Türlendirilmiş Veri Kümesine ait Makale DataTable nesnesinin Find metodu isim değişikliğine uğrayrak FindByID haline dönüşmüştür. Nitekim, bu yeni DataTable nesnesi hangi alanın PrimaryKey olduğunu bilir ve metodun ismini buna göre değiştirerek arama kodunu yazmamızı son derece kolaylaştırır.

![mk51_10.gif](images/mk51_10.gif)

Şekil 4. FindByID metodu;

Diğer yandan, Klasik Find metodumuz, key parametresini object türünden alırken, FindByID metodumuz PrimaryKey alanının değeri ne ise, parametreyi o tipten alır. Buda FindByID metodunun, klasik Find metodundanki object türünden dolayı, daha performanslı çalışmasına neden olur.

![mk51_12.gif](images/mk51_12.gif)

Şekil 5. FindByID metodunda parametre tipi.

Diğer yandan aynı işi yapan bu metodlar arasındaki fark birden fazla Primary Key alanına sahip olan tablolarda daha çok belirginleşir. Söz gelimi, Sql sunucusunda yer alan Northwind veri tabanındaki, Order Details tablosunun hem OrderID alanı hemde ProductID alanı Primary Key'dir. Dolayısıyla bu iki alan üzerinden arama yapmak istediğimizde klasik Find metodunu aşağıdaki haliyle kullanırız.

```csharp
DataRow drBulunan;
drBulunan = dt.Find(new objcet[]{10756,9);
```

Oysaki bu tabloyu Türlendirilmiş Veri Kümesi üzerinden kullanırsak kod satırları aşağıdakine dönüşür.

```csharp
drBulunan=ds.SiparisDetay.FindByOrderIDProductID(10756,9);
```

Gelelim verilerin düzenlenmesi işlemine. Şimdi uygulamamızda bulduğumuz satıra ait verileri textBox kontrollerine aktarıldıktan sonra, üzerlerinde değişiklik yaptığımızda bu değişikliklerin DataTable'ımıza nasıl yansıtacağımıza bakalım. Normal şartlarda, Türlendirilmemiş Veri Kümesi üzerindeki bir DataTable'a ait herhangibir satırda yapılan değşiklikler için, güncel DataRow nesnesine ait BeginEdit ve EndEdit metodları kullanılmaktadır. Oysaki Türlendirilmiş Veri Kümelerindenki satırlara ait alanlar birer özellik olarak tutulduklarından, sadece bu özelliklere yeni değerlerini atamamız yeterli olmaktadır. Bu veri kümesinin bir sınıf şeklinde tutuluyor olmasının sağlamış olduğu güçtür. Bu nedenle Türlendirilmiş Veri Kümeleri Strongly takısını haketmektedir. Dilerseniz uygulamamızda arama sonucu elde ettiğimiz bir satıra ait alan değerlerini güncelleyeceğimiz kod satırlarını yazalım.

```csharp
private void btnDegistir_Click(object sender, System.EventArgs e)
{
     drBulunan.Konu=txtKonu.Text;
     drBulunan.Adres=txtAdres.Text;
     drBulunan.Tarih=Convert.ToDateTime(txtTarih.Text);
}
```

Şimdi uygulamamızı çalıştıralım. ID değeri 6 olan satırı bulalım. Daha sonra bu satırdaki bazı veileri değiştirelim ve Degistir başlıklı butona tıklayalım. Değişikliklerin hemen gerçekleştiğini ve DataGrid kontrolünede yansıdığını görürüz.

![mk51_13.gif](images/mk51_13.gif)

Şekil 6. ID=6 olan satır bulunuyor.

![mk51_14.gif](images/mk51_14.gif)

Şekil 7. Güncel Satır Verileri Değiştiriliyor.

Son olarak satır silme işlemini inceleyelim. Bu amaçla RemoveMakaleRow metodunu kullanacağız. Elbette bu metod, örneğimizde oluşturduğumuz Kuvvetle Türlendirilmiş Veri Kümemizin sınıfı içinde yeniden yazılmış bir metoddur ve sınıfımız içindeki prototipide şu şekildedir.

```csharp
public void RemoveMakaleRow(MakaleRow row)
{
     this.Rows.Remove(row);
}
```

Açıkçası yaptığı işlem sınıfın Rows koleksiyonundan row parameteresi ile gelen satırı çıkartmaktır. Uygulamamızda ise bu metodu aşağıdaki şekilde kullanırız.

```csharp
private void btnSil_Click(object sender, System.EventArgs e)
{
     mk.Makale.RemoveMakaleRow(drBulunan);
}
```

Bu metod parametre olarak aldığı satırı tablodan çıkartır.

Böylece Kuvvetle Türlendirilmiş Veri Kümeleri üzerinden satır ekleme, arama, düzenleme ve silme işlemlerinin nasıl yapılacağını incelemiş olduk. Umuyorumki hepiniz için faydalı bir makale olmuştur. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
