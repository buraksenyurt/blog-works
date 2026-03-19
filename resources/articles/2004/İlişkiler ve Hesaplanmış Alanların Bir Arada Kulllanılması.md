---
title: "İlişkiler ve Hesaplanmış Alanların Bir Arada Kulllanılması"
pubDate: 2004-04-08 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - DataSet
  - DataTable
---

# İlişkiler ve Hesaplanmış Alanların Bir Arada Kulllanılması
Değerli Okurlarım, Merhabalar.

Bu makalemizde aralarında bire-çok (one-to-many) ilişki olan tablolar için hesaplanmış alanların, (yani DataColumn sınıfının Expression özelliği ile oluşturduğumuz sütunların) tablolar arasındaki ilişkiler ile nasıl bir arada kullanılabileceğini incelemeye çalışacağız. Burada bir arada kullanımdan kastım, örnek olarak; ebevyn (parent) tabloda fiziki olarak var olmayan ancak uygulamanın çalışması sırasında oluşturulacak bir sütundan, detay tablosundaki ilişkili alanlar üzerinden toplam, ortalama, miktar gibi Aggregate ifadelerinin çalıştırılmasından ve sonuçların yine parent tabloya yansıtılmasından bahsediyorum.

Konumuzun ana problemini daha iyi anlamak için şu örneği göz önünde bulunduralım. Internet üzerinden ticaret yapan sitemizde kullanıcıların temel bilgileri ile, vermiş oldukları sipariş bilgilerinin ayrı iki tabloda tutulduğunu ve bu tablolar arasında bire-çok ilişki olduğunu varsayalım. Kendimize ait yönetici ekranlarında, her bir üye için, bu güne kadar vermiş olduğu siparişlerin toplam sayısını ve bu siparişlerin hepsine ödemiş olduğu toplam tutarları anlık olarak görmek istediğimizi varsayalım. Burada bize, ebevyn tabloda bir hesaplanmış alan gerekmektedir. Ancak hesaplanmış alan değerleri, detay tablosundaki veriler üzerinden gerçekleştirilmek zorundadır. İşte bu noktada devreye iki tablo arasında tanımlamış olduğumuz bire-çok ilişki girer.

Problemi ve ne yapmak istediğimizi kısaca anlattıktan sonra dilerseniz bunu gerçek bir uygulama üzerinde incelemeye başlayalım. Bu uygulamada, web sitesi üyeleri için tasarlanmış olan Uyeler tablosu ve bu üyelerin satın alım bilgilerini tutan Siparisler isimli tablolara bir windows uygulaması üzerinden erişmek istediğimizi varsayalım. Tablolarımızın sahip olacağı alanlar ve aralarındaki ilişki aşağıdaki şekilde yer almaktadır.

![mk63_1.gif](images/mk63_1.gif)

Şekil 1. Tablolarımızın Yapısı ve Aralarındaki Bire-Çok İlişki.

Bu iki tabloyu göz önüne aldığımızda, bir üyeye ait birden fazla siparişin olabileceğini görürüz. Uygulamamız bittiğinde, DataGrid nesnemizde, her bir üyenin bu güne kadar vermiş olduğu siparişlerin toplam sayısını ve ödemiş olduğu toplam miktarları gösterecek iki yeni sütunumuz olucak. Hiç vakit kaybetmeden uygulamamızın kodlarını yazmaya başlayalım. Önce, aşağıdaki gibi bir Form tasarlayalım.

![mk63_2.gif](images/mk63_2.gif)

Şekil 2. Form Tasarımımız.

Uygulamamızın kodlarına gelince.

```csharp
SqlConnection con;
SqlDataAdapter da;
DataTable dtUyeler;
DataTable dtSiparisleri;
DataSet ds;

private void btnGetir_Click(object sender, System.EventArgs e)
{
    /* Sql sunucumuza olan bağlantımız oluşturuluyor. */
    con=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=SSPI");

    ds=new DataSet(); /* DataSet nesnemiz oluşturuluyor. */

    /* Önce, Uyeler tablomuzdaki verileri alıyor , dtUyeler DataTable'ına yüklüyor ve oluşan veri kümesini temsil eden bu DataTable nesnesinide DataSet nesnemizin tables koleksiyonuna ekliyoruz.*/
    da=new SqlDataAdapter("Select * From Uyeler",con); 
    dtUyeler=new DataTable();
    da.Fill(dtUyeler);
    ds.Tables.Add(dtUyeler);

    /* Aynı işlemi Siparisleri tablosu için yapıyoruz.*/
    da=new SqlDataAdapter("Select * From Siparisleri",con);
    dtSiparisleri=new DataTable();
    da.Fill(dtSiparisleri);
    ds.Tables.Add(dtSiparisleri); 

    /* Uyeler tablosundan Siparisleri tablosuna olan (dolayısıyla dtUyeler DataTable nesnesinin bellekte işaret ettiği bölgedeki veri satırlarından, dtSiparisleri dataTable nesnesinin bellekte temsil ettiği bölgedeki veri kümesine olan) bire-çok ilişkiyi tanımlıyoruz. */
    ds.Relations.Add("Uyeler_Siparisleri",dtUyeler.Columns["UyeID"],dtSiparisleri.Columns["UyeID"],false);
    
    dtSiparisleri.Columns.Add("ToplamTutar",typeof(Decimal),"Miktar*BirimFiyat");/* Bu satır ile, dtSiparisleri tablomuzda, her bir satır için Miktar ve BirimFiyat alanlarının değerlerini çarpıyoruz. Çıkan sonuçları ToplamTutar isimli yeni tanımladığımız bir alana içinde tutacak şekilde dtSiparisler DataTable nesnesinin Columns koleksiyonuna ekliyoruz. */
    
    dtUyeler.Columns.Add("Siparis Sayisi",typeof(int),"COUNT(Child.UyeID)");/* Burada ise, dtUyeler tablosunda Siparis Sayisi isimli yeni bir alan oluşturuyoruz. Bu alan, ilişkili tablo olan detay tablosundaki UyeId alanlarının sayısını COUNT ile hesaplıyor. Ancak bunu yaparken Child nesnesini kullanıyor. Nitekim burada Child nesnesi, Uyeler tablosundaki her bir uyenin, Siparisleri tablosunda karşılık gelen satırlarını temsil ediyor. */
    
    dtUyeler.Columns.Add("Toplam Ödeme",typeof(Decimal),"SUM(Child.ToplamTutar)");/* Burada ise, Child nesnesini kullanarak, var olan ilişki üzerinden, Siparisleri tablosuna gidiyor ve her bir üye için, az önce hesapladığımız ToplamTutar alanlarının toplamını SUM aggregate fonksiyonu ile hesaplıyoruz. Sonuçlarını ise, Toplam Ödeme isimli yeni bir alan olarak Uyeler tablomuza ekliyoruz.*/

    dgUyeler.DataSource=ds.Tables[0];
}
```

Uygulamamızda Child isimli nesneyi nasıl kullandığımıza ve bu sayede, iki tablo arasındaki ilişki yardımıyla, child tablodaki veriler üzerindeki hesaplamaları bir bütün halinde, parent tabloya hesaplanmış alan olarak nasıl eklediğimize lütfen dikkat ediniz. Uygulamamızı çalıştırdığımızda aşağıdaki sonucu elde ederiz. Gördüğünüz gibi her bir üye için, yapılmış olan toplam siparis sayısına ve ödemiş oldukları toplam miktarlara ulaşabildik.

![mk63_3.gif](images/mk63_3.gif)

Şekil 3. Uygulamanın Çalışmasının Sonucu.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
