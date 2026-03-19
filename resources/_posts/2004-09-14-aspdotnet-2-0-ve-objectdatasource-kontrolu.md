---
layout: post
title: "Asp.Net 2.0 ve ObjectDataSource Kontrolü"
date: 2004-09-14 12:00:00 +0300
description: "Bu makalemizde ObjectDataSource bileşenini incelemeye çalışacağız. ObjectDataSource bileşeni Asp.Net 2.0 ile gelen yeni bileşenlerden birisidir. Görevi, 3 katlı mimarinin uygulanması halinde, verikaynağı ile sunum katmanında yer alan veri bağlı kontroller arasındaki veri alışverişinin, iş katmanı üzerindeki herhangibir nesne yardımıyla gerçekleştirilebilmesini sağlamaktır. Başka bir deyişle, veri-bağlı kontroller ile, iş katmanında yer alan iş nesnesi arasındaki iletişimi sağlamaktadır."
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - dotnet
  - aspnet
  - dataset
---
# Asp.Net 2.0 ve ObjectDataSource Kontrolü
Değerli Okurlarım Merhabalar,

Bu makalemizde ObjectDataSource bileşenini incelemeye çalışacağız. ObjectDataSource bileşeni Asp.Net 2.0 ile gelen yeni bileşenlerden birisidir. Görevi, 3 katlı mimarinin uygulanması halinde, verikaynağı ile sunum katmanında yer alan veri bağlı kontroller arasındaki veri alışverişinin, iş katmanı üzerindeki herhangibir nesne yardımıyla gerçekleştirilebilmesini sağlamaktır. Başka bir deyişle, veri-bağlı kontroller ile, iş katmanında yer alan iş nesnesi arasındaki iletişimi sağlamaktadır.

![mk92_6.gif](/assets/images/2004/mk92_6.gif)

Şekil 1. ObjectDataSource bileşenin 3 katlı mimarideki rolü.

Örneğimizi incelediğimizde ObjectDataSource nesnesinin 3 katlı mimarideki yerini daha kolay anlama fırsatı bulacağız. Şimdi şu senaryoyo göz önüne alalım. Web sitemizdeki veri bağlı kontrollerin herhangibir veri kaynağından veri çekmek, veri eklemek, veri silmek ve veri güncellemek gibi işlemler için bir sınıf ve metodlarını kullandığını düşünelim. İşte bu sınıfın sağlamış olduğu fonksiyonellikleri, sunum katmanında yer alan veri-bağlı kontroller ile kolayca gerçekleştirebilmek amacıyla ObjectDataSource nesnesini kullanabiliriz. Örnek olarak, Personel ile ilgili kayıtları tutan bir access tablomuz olduğunu düşünelim. Amacımız ilk olarak, bu tablodaki verileri alıp getirecek ve yeni bir satır veri girilmesini sağlayacak bir metoda sahip olan bir sınıf yazmak olsun. Bunun için aşağıdaki kodlara sahip bir sınıf geliştirelim.

```csharp
using System;
using System.Data;
using System.Data.OleDb;
using System.Web;

public class IsNesnesi 
{
    public DataSet VerileriAl()
    {
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;data source=" + HttpContext.Current.Server.MapPath("~/Data/Veriler.mdb"));
        OleDbDataAdapter da = new OleDbDataAdapter("Select * From Personel", con);
        DataSet ds = new DataSet();
        da.Fill(ds);
        return ds;
    }
    public int VeriGir(string ad, string soyad, string mail, string departman, DateTime giris)
    {
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;data source=" + HttpContext.Current.Server.MapPath("~/Data/Veriler.mdb"));
        OleDbCommand cmd = new OleDbCommand("Insert Into Personel (PerAd,PerSoyad,Departman,Mail,GirisTarihi) Values ('" + ad + "','" + soyad + "','" + departman + "','" + mail + "','" + giris + "')", con);
        con.Open();
        int sonuc = cmd.ExecuteNonQuery();
        con.Close();
        return sonuc;
    }
    public IsNesnesi()
    {
    }
}
```

VerileriAl isimli metodumuz, Veriler isimli Access veritabanına bağlanarak burada yer alan Personel isimli tablodan tüm satırları alıyor ve bu satırları yüklediği DataSet nesnesini geri döndürüyor. VeriGir isimli metodumuz ise, parametreler alarak, tablomuza yeni bir satır ekliyor ve OleDbCommand nesnesinin ExecuteNonQuery metodunun çalıştırılması sonucu dönen integer değeri çağırıldığı yere aktarıyor. Şimdi bu sınıfımızı kullanacak bir web uygulaması inşa edelim. Web sitemizde, iş nesnemize ait IsNesnesi sınıfını Code klasörümüzün altına yerleştireceğiz. Bununla birlikte, veritabanımızıda yine Data klasörü altına kopyalayalım.

![mk92_5.gif](/assets/images/2004/mk92_5.gif)

Şekil 2. Uygulama yapımız.

Bu ön hazırlıklardan sonra, default.aspx sayfamızıda aşağıdaki kontrolleri barındıracak şekilde oluşturalım.

![mk92_1.gif](/assets/images/2004/mk92_1.gif)

Şekil 3. default.aspx sayfamızın yapısı.

Burada iş nesnesi ile formda yer alan bağlı kontroller arasındaki ilişkiyi sağlamak üzere bir ObjectDataSource nesnemiz yer almaktadır. İlk önce, ObjectDataSource bileşenimizin aspx kodlarını aşağıdaki gibi tamamlayalım.

```text
<asp:ObjectDataSource ID="ObjectDataSource1" Runat="server" TypeName="IsNesnesi"
SelectMethod="VerileriAl" InsertMethod="VeriGir">
    <InsertParameters>
        <asp:Parameter Name="ad"></asp:Parameter>
        <asp:Parameter Name="soyad"></asp:Parameter>
        <asp:Parameter Name="mail"></asp:Parameter>
        <asp:Parameter Name="departman"></asp:Parameter>
        <asp:Parameter Name="giris" Type="DateTime"></asp:Parameter>
    </InsertParameters>
</asp:ObjectDataSource>
```

ObjectDataSource bileşenimizin başarılı bir şekilde işleyebilmesi için en önemli gereklilikler, select, insert, update ve delete işlemleri için iş nesnesi üzerindeki hangi metodların kullanacağının bildirilmesidir. Biz örneğimizde ye alan iş nesnesine ait sınıf içerisinde, veri çekmek ve veri girişi için iki metod tanımladık. ObjectDataSource bileşeninin SelectMethod ve InsertMethod özelliklerine, ilgili Select ve Insert işlevlerini gerçekleştiren iş nesnesi metodlarının adlarını aktarıyoruz. Artık ObjectDataSource ile veriye bağlanacak olan nesneler, veri çekmek ve veri girişi için burada belirtilen metodları kullanacaklardır. Tabiki, ObjectDataSource'un bu bilgiler dışında ilgili metodları içieren sınıf hakkında da bilgiye sahip olması gerekir. İşte bunun içinde, TypeName özelliğine ilgili iş nesnesinin sınıf adı atanır.

Şunu hemen belirtelim ki, burada tanımladığımız sınıf başka bir isim alanı altında olabilir. Bu durumda, bu sınıfa tam olarak ulaşılacak tip bilgisinin TypeName özelliğinde belirtilmesi gerekir. Örneğin, sınıfımız IsKatmani isimli bir isim alanında bulunuyorsa bu durumda TypeName özelliğine IsKatmani.IsNesnesi değeri girilmelidir.

Diğer DataSource bileşenlerinde olduğu gibi, Insert, Update ve Delete gibi işlemler parametreler yolu ile gerşekleştirilmektedir. Bizimde burada, Insert metodumuzda kullandığımız bazı parametreler mevcuttur. İşte bu parametreleri temsil etmesi için, ObjectDataSource kontrolünün InsertParameters koleksiyonu kullanılmıştır. Delete ve Update işlemlerinde de DeleteParameters ve UpdateParameters koleksiyonları kullanılır.

Gelelim, formumuzda yer alan veri bağlı kontrollere. Veri çekme işlemi sonrasında, tablomuzda yer alan satırları GridView kontrolünde göstermek istediğimiz için, GridView kontrolüne ait aspx kodlarını aşağıdaki gibi yazmalıyız.

```text
<asp:GridView ID="GridView1" Runat="server" DataSourceID="ObjectDataSource1" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField DataField="PerAd" HeaderText="Personel Adı"></asp:BoundField>
        <asp:BoundField DataField="PerSoyad" HeaderText="Personel Soyadı"></asp:BoundField>
        <asp:BoundField DataField="Departman" HeaderText="Departmanı"></asp:BoundField>
        <asp:BoundField DataField="GirisTarihi" DataFormatString="{0:dd.mm.yyyy}" HeaderText="İşe Başlama Tarihi"></asp:BoundField>
        <asp:BoundField DataField="Mail" HeaderText="Mail Adresi"></asp:BoundField>
    </Columns>
</asp:GridView>
```

GridView kontrolümüzde DataSourceID özelliğine ObjectDataSource bileşenimizin ID değerini atamak ile, çalışma zamanında GridView kontrolünde, ObjectDataSource'un iş nesnesi üzerinden çalıştırdığı Select metodundan dönen DataSet'e ait satırların gösterilmesini sağlamış oluyoruz. Eğer sayfamızı bu haliyle çalıştıracak olursak aşağıdaki ekran görüntüsünü elde ederiz.

![mk92_2.gif](/assets/images/2004/mk92_2.gif)

Şekil 4. Select metodunun çalışması sonucu.

Gelelim Insert işlevinin nasıl gerçekleştirileceğine. Yeni bir satır eklemek için Ekle başlıklı button kontrolümüze bir kaç satır kod yazacağız.

```csharp
void btnEkle_Click(object sender, EventArgs e)
{
    ObjectDataSource1.InsertParameters["ad"].DefaultValue = txtAd.Text;
    ObjectDataSource1.InsertParameters["soyad"].DefaultValue = txtSoyad.Text;
    ObjectDataSource1.InsertParameters["mail"].DefaultValue = txtMail.Text;
    ObjectDataSource1.InsertParameters["departman"].DefaultValue = txtDepartman.Text;
    ObjectDataSource1.InsertParameters["giris"].DefaultValue = txtGiris.Text;
    ObjectDataSource1.Insert();
}
```

ObjectDataSource kontrolümüz, iş nesnesi üzerindeki Insert işlevini gerçekleştirecek metodu bilmektedir. Ayrıca, bu metoda göndermesi gereken parametrelerin ne olacağınıda bilir. Bununla birlikte, Insert işlevini gerçekleştirecek olan iş nesnesi metodu için gerekli parametre değerlerinin bir şekilde gönderilmesi gerekmektedir. Bu amaçla, ObjectDataSource nesnesinin InsertParameters koleksiyonundaki her bir parametreye gerekli değerler gönderilir. Daha sonra ise bu parametrelerin değerleri, iş nesnesindeki metod içinde var olan karşılıklarına gönderilir. Bu gönderme emrini ise Insert metodu verir.

Başka bir deyişle, Insert metodu, ObjectDataSource kontrolünün InsertParameters koleksiyonunda yer alan parametre değerlerini, iş nesnesi üzerindeki ilgili metoda gönderir ve bu metodu çalıştırır. Tabi burada ObjectDataSource bileşenindeki InsertParameters koleksiyonunda yer alan parametrelerinin Name özelliklerinin değerlerinin, iş nesnesindeki VeriGir metodundaki parametre isimleri ile aynı olduğunda dikkat etmemiz gerekmektedir. Sonuç olarak veri giriş işlemi gerçekleşmiş olur. Şimdi uygulamamızı denersek, yeni satırlar ekleyebildiğimizi görürüz.

![mk92_3.gif](/assets/images/2004/mk92_3.gif)

Şekil 5. Yeni satırı eklemeden önce.

![mk92_4.gif](/assets/images/2004/mk92_4.gif)

Şekil 6. Yeni satır girildikten sonra.

Insert işleminde dikkat ederseniz, TextBox kontrollerine girdiğimiz değerleri InsertParameters koleksiyonundaki ilgili parametrelere aktardık. Dilersek, parametre değerlerinin aktarımını ilgili kontroller üzerinden doğrudanda gerçekleştirebiliriz. Bunun için, InsertParameters koleksiyonunda Parameter nesneleri yerine, ilgil parametre değerini taşıyan kontrolü temsil edecek ControlParameter nesneleri kullanılır.

```text
<asp:ObjectDataSource ID="ObjectDataSource1" Runat="server" TypeName="IsNesnesi"
SelectMethod="VerileriAl" InsertMethod="VeriGir">
    <InsertParameters>
        <asp:ControlParameter ControlID="txtAd" PropertyName="Text" Name="ad"></asp:ControlParameter>
        <asp:ControlParameter ControlID="txtSoyad" PropertyName="Text" Name="soyad"></asp:ControlParameter>
        <asp:ControlParameter ControlID="txtMail" PropertyName="Text" Name="mail"></asp:ControlParameter>
        <asp:ControlParameter ControlID="txtDepartman" PropertyName="Text" Name="departman"></asp:ControlParameter>
        <asp:ControlParameter ControlID="txtGiris" PropertyName="Text" Type="DateTime" Name="giris"></asp:ControlParameter> 
    </InsertParameters>
</asp:ObjectDataSource>
```

Dikkat edecek olursanız, ControlID ile hangi kontrolün kullanacağı, PropertyName özelliği ilede bu kontrolün hangi özelliğinin değerinin alınacağı belirtilir. Name özelliği ise, iş nesnesindeki ilgili insert metodunda kullanılacak parametreyi işaret etmektedir ve iş nesnesindeki metod parametresindeki isim ile aynıdır. Bu durumda, insert işlevini gerçekleştirebilmek için tek yapmamız gereken ObjectDataSource bileşenine ait Insert metodunu çağırmak olacaktır. Insert metodu çalıştırıldığında ObjectDataSource bileşeni, InsertParameters koleksiyonunda belirtilen kontroller üzerindeki değerleri alıp ilgili insert metoduna göndermektedir.

```csharp
void btnEkle_Click(object sender, EventArgs e)
{
    ObjectDataSource1.Insert();
}
```

Delete ve Update işlevleride Insert işlevine benzer şekilde gerçekleştirilir. Bu kez, ObjectDataSource bileşeni için DeleteCommand ve UpdateCommand özellikleri bildirilir ve ilgili parametre koleksiyonları eklenir. Bu makalemizde kısaca ObjectDataSource bileşenini incelemeye çalıştık. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.