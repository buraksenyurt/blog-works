---
title: "DataColumn.Expression Özelliği İle Hesaplanmış Alanların Oluşturulması"
pubDate: 2003-12-06 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - Performance
  - DataTable
---

# DataColumn.Expression Özelliği İle Hesaplanmış Alanların Oluşturulması
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde bir tabloda sonradan hesaplanan alanların nasıl oluşturulacağını incelemeye çalışacağız. Her zaman olduğu gibi konuyu iyi anlayabilmek için bir örnek üzerinden gideceğiz. Bir hesaplanan alan aslında bir hesaplama ifadesidir. Örneğin var olan bir tablodaki bir alana ait her değere ortak bir işlem yaptırmak istediğimizi ve bu her veri için oluşan sonuçlarında tabloda ayrı bir alan adı altında gözükmesini istediğimizi varsayalım. Buna en güzel örneklerden birisi;

Diyelimki personelinizin maaş bilgilerinin tutulduğu tablomuz var. Bu tablomuzda yer alan maaş verilerinde değişik oranlarda artışlar uygulamak istediğinizi varsayalım. Yüzde 10, yüzde 15 vb...Bu artışlarıda sadece ekranda izlemek istediğinizi tablo üzerinde kalıcı olarak yer almasını istemediğinizi düşünün. Bu minik problemin çözümü DataColumn sınıfına ait Expression özelliğidir. Bu özelliği yapmak istediğimiz işlemin sonucunu oluşturacak ifadelerden oluştururuz.

Konuyu daha net anlayabilmek için hiç vakit kaybetmeden örneğimizde geçelim. Bu örnekte Maas isimli bir tablodaki Maas alanlarına ait değerlere kullanıcının seçtiği oranlarda artış uygulayacağız. Form tasarımımız aşağıdakine benzer olucak. Burada comboBox kontrolümüzde %5 ten %55 ‘e kadar değerler girili. Hesapla başlıklı butona basıldığında, hesaplanan alana ilişkin işlemlerimizi gerçekleştirilecek.

![mk15_1.gif](images/mk15_1.gif)

Şekil 1. Form Tasarımı

Hiç vakit kaybetmeden kodlarımıza geçelim.

```csharp
SqlConnection conFriends;
SqlDataAdapter da;
DataTable dtMaas;
/* Aşağıdaki procedure ile, Sql sunucumuzda yer alan Friends isimli veritabanına bağlanıyor, buradan Maas isimli tablodaki verileri DataTable nesnemize yüklüyoruz. Daha sonrada dataGrid kontrolümüze veri kaynağı olarak bur DataTable nesnesimizi gösterek tablonun içeriğinin görünmesini sağlıyoruz.*/

public void doldur()
{
     conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
     da=new SqlDataAdapter("Select * From Maas",conFriends);
     dtMaas=new DataTable("Maaslar");
     da.Fill(dtMaas);
     dgMaas.DataSource=dtMaas;
}
private void Form1_Load(object sender, System.EventArgs e)
{
     doldur(); /* Tablodan verilerimizi alan procedure'u çağırıyoruz*/
}

private void btnHesapla_Click(object sender, System.EventArgs e)
{
     /* Hesaplanan Alanımız için bir DataColumn nesnesi tanımlıyoruz. */
     DataColumn dcArtisAlani=new DataColumn();
     /* Expression özelliğine yapmak istediğimiz hesaplamayı giriyoruz. Buradaki hesaplamada, kullanıcının cmbAris isimli comboBox kontrolünden seçtiği oran kadar maaşlara artış uygulanıyor.*/
     dcArtisAlani.Expression="Maas + (Maas *"+cmbArtis.Text+"/100)";
     /* Yeni alanımız için anlamlı bir isim veriyoruz. Bu isimde artış oranıda yazmakta.*/
     dcArtisAlani.ColumnName="Yuzde"+cmbArtis.Text+"Artis";
     /* Daha sonra oluşturduğumuz bu hesaplanmış alanı DataTable nesnemize ekliyoruz. Böylece bellekteki Maas tablomuzda, maaslara belirli bir artış oranı uygulanmış verileri içeren DataColumn nesnemiz hazırlanmış oluyor.*/
     dtMaas.Columns.Add(dcArtisAlani);
     /* Son olarak dataGrid nesnemizi Refresh() metodu ile tazeleyerek hesaplanmış alanımızında görünmesini sağlıyoruz.*/
     dgMaas.Refresh();
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsü ile karşılaşırız. Ben örnek olarak %10 luk artış uyguladım.

![mk15_2.gif](images/mk15_2.gif)

Şekil 2. Program İlk Çalıştığında.

İşte işlemimizin sonuçları.

![mk15_3.gif](images/mk15_3.gif)

Şekil 3. Hesaplanmış Alan

Görüldüğü gibi %10 luk artışın uygulandığı yeni alanımız dataGrid’imiz içinde görülmektedir. Unutmayalımki bu oluşturduğumuz DataColumn nesnesi sadece bellekteki tablomuza eklenmiş bir görüntüdür. Veritabanımızdaki tablomuza doğrudan bir etisi yoktur. Tabiki performans açısından çok yüksek kapasiteli tablolarda çalışırken böyle bir işlemin yapılması özellikle ağ ortamlarında performans kaybınada yol açabilir. Bunun önüne geçmek için kullanabileceğimiz yöntemlerden birisi, sql sunucusunda bu hesaplamaların yaptırılması ve sonuçların view nesneleri olarak alınmasıdır. Ancak küçük boyutlu veya süzülmüş veriler üzerinde, Expression özelliğinin kullanımı sizinde gördüğünüz gibi son derece kolay ve faydalıdır. Geldik bir makalemizin sonuna daha, tekrar görüşünceye kadar hepinize mutlu günler dilerim.
