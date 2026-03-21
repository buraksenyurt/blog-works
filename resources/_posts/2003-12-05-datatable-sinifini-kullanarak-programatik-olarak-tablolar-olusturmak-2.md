---
layout: post
title: "DataTable Sınıfını Kullanarak Programatik Olarak Tablolar Oluşturmak-2"
date: 2003-12-05 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - datatable
---
Hatırlayacağınız gibi yazı dizimizin ilk bölümünde, DataTable sınıfını kullanarak bellekte bir tablonun ve bu tabloya ait alanların nasıl yaratıldığını işlemiştik. Bugünkü makalemizde oluşturmuş olduğumuz bu tabloya kayıtlar ekleyeceğiz ve sonra bu DataTable nesnesini bir DataSet’e aktarıp içerisindeki verileri bir DataGrid kontrolünde göstereceğiz.

Bir dataTable nesnesinin bellekte temsil ettiği tabloya yeni satırlar başka bir deyişle kayıtlar eklemek için, DataRow sınıfından nesneleri kullanacağız. Dilerseniz hiç vakit kaybetmeden uygulamamıza başlayalım. İlk örneğimizin devamı niteliğinde olucak bu çalışmamızda kullanıcının oluşturduğu tablodaki alan sayısı kadar textBox nesnesinide label nesneleri ile birlikte programatik olarak oluşturacağız. İşte programımızın kodları,

```csharp
DataTable dt; /* DataTable nesnemizi uygulama boyunca kullanabilmek için tüm metodlarin disinda tanimladik. */ 
private void btnTabloOlustur_Click(object sender, System.EventArgs e)
{
     dt=new DataTable(txtTabloAdi.Text);
     MessageBox.Show(dt.TableName.ToString()+" TABLOSU BELLEKTE OLUSTURULDU");
     lblTabloAdi.Text=dt.TableName.ToString(); /* TableName özelligi DataTable nesnesinin bellekte temsil ettigi tablonun adini vermektedir.*/ 

     /*Tablo olusturuldugunda otomatik olarak bir ID alani ekleyelim. Bu alan benzersiz bir alan olucak yani içerdigi veriler tekrar etmiyecek. 1 den baslayarak birer birer otomatik olarak articak. Ayni zamanda primary key olucak. Primary key oldugu zaman arama gibi islemleri yaparken bu alani kullanacagiz.*/ 
     DataColumn dcID=new DataColumn(); /* DataColumn nesnesi olusturulur.*/
     dcID.ColumnName="ID"; /* Alanin adi veriliyor*/
     dcID.DataType=Type.GetType("System.Int32"); /* Alanin veritipi belirleniyor*/

     dcID.Unique=true;/* Alanin içerdigi verilerin tekrar etmeyecegi söyleniyor.*/
     dcID.AutoIncrement=true;/* Alanin degerlerinin otomatik olarak artacagi söyleniyor.*/
     dcID.AutoIncrementSeed=1;/* Ilk degeri 1 olucak.*/
     dcID.AutoIncrementStep=1;/* Alanin degerleri 1'er articak.*/
     dt.Columns.Add(dcID);/* Yukarida özellikleri belirtilen ID alani tablomuza ekleniyor. */ 
     /* Asagidaki kod satirlari ile ID isimli alani Primary Key olarak belirliyoruz. */
     DataColumn[] anahtarlar=new DataColumn[1];
     anahtarlar[0]=dt.Columns["ID"];
     dt.PrimaryKey=anahtarlar; 
     lstAlanlar.Items.Add(dt.Columns["ID"].ColumnName.ToString()+" Primary Key"); 
} 
private void btnAlanEkle_Click(object sender, System.EventArgs e)
{
     /*Önce yeni alanimiz için bir DataColumn nesnesi olusturulur*/
     DataColumn dc=new DataColumn();
     /*Simdi ise DataTable nesnemizin Columns koleksiyonuna olusturulan alanimizi ekliyoruz. Ilk parametre, alanin adini temsil ederken, ikinci parametre ise alanin veri türünü belirtmektedir. Add metodu bu özellikleri ile olusturulan DataColumn nesnesini dataTable'a ekler. Bu sayede tabloda alanimiz olusturulmus olur.*/
     dt.Columns.Add(txtAlanAdi.Text,Type.GetType("System."+cmbAlanTuru.Text));
     lstAlanlar.Items.Add("Alan Adi: "+dt.Columns[txtAlanAdi.Text].ColumnName.ToString()+" Veri Tipi: "+dt.Columns[txtAlanAdi.Text].DataType.ToString());
     /* ColumnName özelligi ile eklenen alanin adini, DataType özelligi ilede bu alanin veri türünü ögreniyoruz.*/
} 

public void KontrolOlustur(string alanAdi,int index)
{
     /* Aşağıdaki kodları asıl konumuzdan uzaklaşmadan,açıklamak istiyorum. DataTable'daki her bir alan için bir TextBox nesnesi ve Label nesnesi oluşturulup formumuza ekleniyor. Top özelliğinin ayarlanışına dikkatinizi çekmek isterin. Top özelliğini bu metoda gönderdiğimiz index paramteresi ile çarpıyoruz. Böylece, o sırada hangi indexli datacolumn nesnesinde isek ilgili kontrolün formun üst noktasından olan uzaklığı o kadar KAT artıyor. Elbette en önemli özellik kontrollerin adlarının verilemsi. Tabi her bir kontroü this.Controls.Add syntax'ı formumuza eklemeyi unutmuyoruz. */
     System.Windows.Forms.TextBox txtAlan=new System.Windows.Forms.TextBox();
     txtAlan.Text="";
     txtAlan.Left=275;
     txtAlan.Top=30*index;
     txtAlan.Name="txt"+alanAdi.ToString();
     txtAlan.Width=100;
     txtAlan.Height=30;
     this.Controls.Add(txtAlan); 
     System.Windows.Forms.Label lblAlan=new System.Windows.Forms.Label();
     lblAlan.Text=alanAdi.ToUpper();
     lblAlan.Left=200;
     lblAlan.Top=30*index;
     lblAlan.AutoSize=true;
     this.Controls.Add(lblAlan);
} 
private void btnKontrol_Click(object sender, System.EventArgs e)
{
     /* Aşağıdaki döngü ile dataTable nesnemizdeki DataColumn sayısı kadar dönecek bir döngü oluşturuyoruz. Yanlık ID alanımız (0 indexli alan) değeri otomatik olarak atandığı için bu kontrolü oluşturmamıza gerek yok. Bu nedenle döngümüz 1 den başlıyor. Döngü her bir DataColumn nesnesi için, bu alanın adını parametre olarak alan ve ilgili textBox ve label nesnesini oluşturacak olan KontrolOlustur isimli metodu çağırıyor.*/
     for(int i=1;i<dt.Columns.Count;++i)
     {
          KontrolOlustur(dt.Columns[i].ColumnName.ToString(),i);
     } 
     dgKayitlar.DataSource=dt; /* Burada dataGrid nesnemizin DataSource özelliğini DataTable nesnemiz ile ilişkilendirerek dataGrid'i oluşturduğumuz tabloya bağlamış oluyoruz. */ 
} 
private void btnKayıtEkle_Click(object sender, System.EventArgs e)
{
     /* Bu butona tıklandığında kullanıcının oluşturmuş olduğu kontrollere girdiği değerler, tablonun ilgili alanlarına ekleniyor ve sonuçlar DataGrid nesnemizde gösteriliyor. */ 
     string kontrol;
     /* Öncelikle yeni bir dataRow nesnesi tanımlıyoruz ve DataTable sınıfına ait NewRow metodunu kullanarak dataTable'ımızın bellekte işaret ettiği tabloda, veriler ile doldurulmak üzere boş bir satır açıyoruz. */
     DataRow dr;
     dr=dt.NewRow();
     /* Aşağıdaki döngü gözünüze korkutucu gelmesin. Yaptığımız işlem DataTable'daki alan sayısı kadar sürecek bir döngü. Her bir alana, kullanıcının ilgili textBox'ta girdiği değeri eklemek için, dr[i] satırını kullanıyoruz. dr[i] dataRow nesnesinin temsil ettiği i indexli alana işaret ediyor. Peki ya i indexli bu DataRow alanının dataTable'daki hangi alana işaret ettiği nereden belli. İşte dt.NewRow() dediğimizde, dataTable'daki alanlardan oluşan bir DataRow yani satır oluşturmuş oluyoruz. Daha sonra yapmamız gereken ise textBox kontrolündeki veriyi almak ve bunu DataRow nesnesindeki ilgili alana aktarmak. Bunun için formdaki tüm kontroller taranıyor ve her bir kontrol acaba alanlar için oluşturulmuş TextBox nesnesi mi? ona bakılıyor. Eğer öyleyse dataRow nesnesinin i indexli alanına bu kontrolün içerdiği text aktarılıyor.*/
     for(int i=1;i<dt.Columns.Count;++i)
     {
          kontrol="txt"+dt.Columns[i].ColumnName.ToString();
          for(int j=0;j<this.Controls.Count;++j)
          {
               if(this.Controls[j].Name==kontrol)
               {
                    dr[i]=this.Controls[j].Text;
               }
          }
     }
     /* Artık elimizde içindeki alanları bizim girdiğimiz veriler ile dolu bir DataRow nesne örneği var. Tek yapmamız gereken bunu dataTable nesnemizin Rows koleksiyonuna eklemek. Daha sonra ise dataGrid nesnemizi tazeleyerek görüntünün yenilenmesini ve girdiğimiz satırın burada görünmesini sağlıyoruz. */
     dt.Rows.Add(dr);
     .Refresh();
} 
```

Şimdi programımızı deneyelim. Ben örnek olarak Ad,Soyad ve Birim alanlarından oluşan bir tablo oluşturdum ve iki kayıt girdi. İşte sonuç,

![mk14_1.gif](/assets/images/2003/mk14_1.gif)

Şekil 1. Programın çalışmasının sonucu.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde, DataRelation nesnesi yardımı ile birbirleri ilişkili tabloların nasıl oluşturulacağını göreceğiz. Hepinize mutlu günler dilerim.