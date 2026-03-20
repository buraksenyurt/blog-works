---
layout: post
title: "Bağlantısız Katmanda Concurrency Violation Durumu"
date: 2005-03-06 10:00:00 +0300
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - sql-server
  - concurrency
  - performance
  - dataset
  - datatable
---
Bağlantısız katman nesneleri ile çalışırken karşılaşabileceğimiz problemlerden bir tanesi güncelleme işlemleri sırasında oluşabilecek DBConcurrencyException istisnasıdır. Bu makalemizde, bu hatanın fırlatılış nedenini inceleyecek ve alabileceğimiz tedbirleri ele almaya çalışacağız. Öncelikle istisnanın ne olduğunu anlamak ile işe başlayalım. Bir DataAdapter nesnesine ait Update metodu güncelleme işlemleri için Optimistic (iyimser) yaklaşımı kullanan sql sorgularını çalıştırıyorsa DBConcurrencyException istisnasının ortama fırlatılması, başka bir deyişle Concurrency Violation (eş zamanlı uyumsuzluk) durumunun oluşması son derece doğaldır.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Optimistic yaklaşım modeli, Pessimistic yaklaşım modelinin aksine güncellenecek satırları kilitlemez. Buda sunucunun kilit açma, takip ve kapatma gibi işlemleri yapmaması dolayısıyla performansının artması anlamına gelir. Özellikle bağlantısız katman mimarisinde kullanılan optimistic yaklaşım modelinde tek sorun, güncelleme işlemlerini gerçekleştiren kullanıcıların bu işleri birbirlerinden habersiz şekilde yapmaları sonucu ortaya çıkabilecek durumlardır.

Örneğin belli bir satıra ait verileri güncellemek için kullanabileceğimiz aşağıdaki Sql sorgusunu ele alalım.

```text
UPDATE MAILS SET AD=@AD,SOYAD=@SOYAD,EMAIL=@EMAIL WHERE ID=@ORGID AND AD=@ORGAD AND SOYAD=@ORGSOYAD AND EMAIL=@ORGEMAIL
```

Sorgumuz basitçe, MAILS isimli veritabanındaki AD, SOYAD ve EMAIL alanlarının değerlerini güncellemektedir. Bunu yaparkende optimistic (iyimser) yaklaşımını kullanır. Bu nedenle, Where koşulunda tabloya ait primary key alanı (ID) dahil olmak üzere tüm alanlar kullanılmaktadır. Böylece tüm alanların eşleştirme için kullanıldığı bir sorgu ortaya çıkar.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Optimistic yaklaşımda ele alınan yukarıdaki sorgu modeli için Sql Server ve benzeri veritabanı sistemlerinde daha etkili yöntemlerde vardır. Örneğin Sql üzerinde timestamp tipinden (yada uniqueIdentifier tipinden) alanlar kullanılabilir. Timestamp türünden olan alanlar satır üzerinde yapılacak herhangibir güncelleme işleme sonrasında sistem tarafından otomatik olarak benzersiz bir karakter dizisi ile değiştirilen alanlardır. Böylece yukarıdaki sorgunun yaptığı işin aynısını aşağıdaki gibide yapabiliriz. (Buradaki Kontrol alanı tipi timestamp tipindendir.
Update Mails Set Ad=@Ad,Soyad=@Soyad,Email=@Email Where Id=@OrgId And Kontrol=@Kontrol
Bu ifadenin bize sağladığı en büyük avantaj elbetteki n sayıda alan içeren bir tabloda where ifadesinden sonra sadece iki alan kontrolü ile (primary key ve timestamp alanı) Concurrency Violation durumunu irdeleyebilecek olmamızdır. Biz makalemizde daha uzun olan yolu incelemeye çalışacağız. Lakin gerçek hayat modellerinde timestamp veya uniqueidentifier ve benzeri tipten alanların karşılaştırma işlemi için ele alınması daha doğru ve güçlü bir yaklaşım olacaktır.

Böyle bir sorgunun neden olacağı istisnai durumu anlayabilmek için aşağıdaki senaryoyu göz önüne almakta fayda olacağı inancındayım. Senaryomuzda en az iki kullanıcı rol almaktadır. Bu kullanıcılarımıza A ve B takma isimlerini verdiğimizi düşünelim. Her iki kullanıcıda database'den MAILS tablosundaki verileri bağlantısız katmana DataAdapter sınıfına ait nesne örneği vasıtasıyla almaktadır.

![mk116_2.gif](/assets/images/2005/mk116_2.gif)

A ve B verileri çektikten sonra, A kullanıcısı herhangibir satır üzerinde güncelleme işlemini uygular. Bu durumda DataAdapter nesnesinin UpdateCommand özelliğine karşılık gelen SqlCommand nesnesi, yukarıda yazdığımız sorguyu çalıştıracaktır. Bu sorguda, satırların orjinal değerleri ile veritabanındaki halleri aynı olacağından güncelleme işlemi başarılı bir şekilde gerçekleştirilecektir. Lakin B kullanıcısı şu anda, A'nın güncellemiş olduğu veri kümesinin eski haline bakmaktadır. Eğer B kullanıcısı, A kullanıcısının biraz önce güncellemiş olduğu satırı tekrar güncellemek isterse ne olacaktır?

İşte bu durumda, sorgu içindeki where koşuluna giren alan değerlerinin bağlantısız katmandaki orjinal halleri (yani DataRowVersion numaralandırıcısı tipinden Original olan değerleri), veritabanındaki tabloda az önce güncelleştirilmiş olan alanlara ait yeni değerler ile eşleşmeyeceğinden ilgili satır bulunamayacaktır. Bu da B kullanıcısının satırı update edememesine neden olur. Bu noktada CLR, DbConcurrencyException türünden bir istisnayı process içine fırlatacaktır. Dilerseniz bu hatayı basit bir uygulama yardımıyla elde etmeye çalışalım. Uygulamamız şimdilik sadece Update işlevini ele alacaktır. İlk olarak basit bir windows uygulaması açarak aşağıdakine benzer bir form ekranı oluşturalım.

![mk116_3.gif](/assets/images/2005/mk116_3.gif)

Uygulamamız aşağıdaki field yapısına sahip olan ve Sql sunucusu üzerinde barındırdığımız MAILS tablosunu kullanacaktır. Tablomuzdaki ID alanı otomatik artan bir primary key olarak tanımlanmıştır.

![mk116_4.gif](/assets/images/2005/mk116_4.gif)

Şimdide uygulama kodlarımızı yazalım.

```csharp
SqlConnection con;
SqlDataAdapter da;
DataSet ds;

/* SqlConnection nesnemizi oluşturduğumuz metodumuz. Bu metotda bağlantı bilgisini App.Config dosyasında tuttuğumuz connectionString isimli key' e ait value özelliğinden alıyoruz.*/
private void BaglantiHazirla()
{
    try
    {
        con=new SqlConnection(ConfigurationSettings.AppSettings["connectionString"].ToString());
    }
    catch(SqlException hata)
    {
        MessageBox.Show(hata.Message);
    }
}

/* Verileri yükleyen metodumuz parametre olarak aldığı string bilgiyi kullanan bir SqlDataAdapter nesnesi oluşturuyor. Daha sonra bu nesne yardımıyla DataSet' imiz dolduruluyor. Son olarak DataSet içindeki tablomuza ait primary key kolonu belirleniyor.*/
private void VerileriYukle(string sorguCumlesi)
{
    BaglantiHazirla();
    da=new SqlDataAdapter(sorguCumlesi,con);
    ds=new DataSet();
    da.Fill(ds);
    ds.Tables[0].PrimaryKey=new DataColumn[]{ds.Tables[0].Columns["ID"]};
}

/* Veri Çek başlıklı butona tıklandığında, MAILS tablosundaki tüm verileri çekeceğimiz sorguyu çalıştıracak VerileriYukle metodunu çağırıyor ve sonuç kümesini DataGrid kontrolümüze bağlıyoruz. Ardından eğer SqlConnection nesnemiz açık ise kapatıyoruz.*/
private void btnVeriCek_Click(object sender, System.EventArgs e)
{
    VerileriYukle("SELECT * FROM MAILS");
    dgVeriler.DataSource=ds.Tables[0];
    if(con.State==ConnectionState.Open)
    {
        con.Close();
    }
}

/* VeriGüncelle metodu Update sorgusunu bizim tanımladığımız SqlDataAdapter nesnesini kullanarak güncelleme işlemini gerçekleştiriyor.*/
private void VeriGuncelle()
{
    try
    {
        string guncellemeCumlesi="UPDATE MAILS SET AD=@AD,SOYAD=@SOYAD,EMAIL=@EMAIL WHERE ID=@ORGID AND AD=@ORGAD AND SOYAD=@ORGSOYAD AND EMAIL=@ORGEMAIL";
        // Timestamp alanı olduğunda : Update Mails Set Ad=@Ad,Soyad=@Soyad,Email=@Email Where Id=@ORGID AND KONTROL=@KONTROL
        SqlCommand cmdUpdate=new SqlCommand(guncellemeCumlesi,con);
        /* Sorgumuz için gerekli parametreleri ekliyoruz. Parametre adlarını, veri tiplerini, boyutlarını ve DataTable daki hangi alanı source olarak alacaklarını belirliyoruz.*/
        cmdUpdate.Parameters.Add("@AD",SqlDbType.NVarChar,50,"AD");
        cmdUpdate.Parameters.Add("@SOYAD",SqlDbType.NVarChar,50,"SOYAD");
        cmdUpdate.Parameters.Add("@EMAIL",SqlDbType.NVarChar,50,"EMAIL");

        /* WHERE koşulunda kullanılan parametreleri giriyoruz. Burada parametre değerlerimiz field' ların orjinal değerleri olacak. Bunu sağlamak için SourceVersion özelliğine DataRowVersion numaralandırıcısının Original değerini atıyoruz.*/
        cmdUpdate.Parameters.Add("@ORGID",SqlDbType.NVarChar,50,"ID");
        cmdUpdate.Parameters["@ORGID"].SourceVersion=DataRowVersion.Original;
        cmdUpdate.Parameters.Add("@ORGAD",SqlDbType.NVarChar,50,"AD");
        cmdUpdate.Parameters["@ORGAD"].SourceVersion=DataRowVersion.Original;
        cmdUpdate.Parameters.Add("@ORGSOYAD",SqlDbType.NVarChar,50,"SOYAD");
        cmdUpdate.Parameters["@ORGSOYAD"].SourceVersion=DataRowVersion.Original;
        cmdUpdate.Parameters.Add("@ORGEMAIL",SqlDbType.NVarChar,50,"EMAIL");
        cmdUpdate.Parameters["@ORGEMAIL"].SourceVersion=DataRowVersion.Original;
// Where cümleciğinden timestamp veya uniqueIdentifier kullanıldığında yukarudaki parametre tanımlamaları yerine sadece Kontrol alanı için tek bir parametre tanımlamasının yapılması yeterli olacaktır.
        // cmdUpdate.Parameters.Add("@KONTROL",SqlDbType.Timestamp,8,"KONTROL");
        // cmdUpdate.Parameters["@KONTROL"].SourceVersion=DataRowVersion.Original;

        da.UpdateCommand=cmdUpdate;

        /* Son olarak Update metodunu çalıştırıyoruz.*/
        da.Update(ds);
    }
    catch(SqlException hata)
    {
        MessageBox.Show(hata.Message);
    } 
}

private void btnGuncelle_Click(object sender, System.EventArgs e)
{
    VeriGuncelle();
}
```

Uygulamamızdan iki tane çalıştırdığımızı ve örneğin AD alanı A ve SOYAD alanı B olan satırların değerlerini sırasıyla ALİ ile VELİ olarak değiştirdiğimizi düşünelim. Eğer Güncelle butonuna tıklarsak işlemin başarılı bir şekilde gerçekleştirildiğini görürüz.

![mk116_1.gif](/assets/images/2005/mk116_1.gif)

Şimdi ikinci kullanıcımız aynı satırın verilerini değiştirsin ve yine Güncelle butonuna bassın. Bu durumda aşağıdaki gibi bir istisna mesajını alırız.

![mk116_5.gif](/assets/images/2005/mk116_5.gif)

Görüldüğü gibi ikinci kullanıcı update işlemini gerçekleştirmeye çalıştığında Concurrency Violation (eş zamanlı uyumsuzluk) durumu oluşacaktır. Bu belirleyici olarak DBConcurrencyException türünden bir istisnadır. Peki oluşan bu istisnai durumun üstesinden nasıl gelebiliriz? İlk akla gelen yöntem, istisna yakalandığında kullanıcıların verilerin en güncel hallerini elde etmeleri konusunda uyarılmalarını sağlamak olacaktır. Ancak bağlantısız katman üzerinde çalışırken, ikinci kullanıcılar bu örnekte olduğu gibi tek bir satırı güncellemek dışında yeni satır girişleri, satır silmeler ve hatta başka satır güncellemeleri gibi birden fazla sayıda işlemi gerçekleştirmiş olabilirler.

Eğer güncelleme yapılan kod satırlarını istisna yakalama mekanizmaları ile izlemez ve DbConcurrencyException hatasını yakalamazsak, kullanıcının o ana kadar yaptığı tüm değişiklikler uygulamanın istem dışı sonlanması nedeni ile kaybolacaktır. Bu elbetteki istenen bir durum değildir. Alternatif bir yol olarak, DataAdapter nesnesinin ContinueUpdateOnError özelliğine true değeri verilebilir. Bu durumda Update işlemi sırasında oluşacak olan hatalar göz ardı edilecektir. Yani Concurrency'ye neden olan satırlar var ise, bunların oluşturdukları istisnalar ortama fırlatılmayacaktır. Örneğimize bu durumu simüle edebileceğimiz bir checkBox kontrolü koyalım. Kullanıcı bu kutucuğu işaretler ise update işlemi sırasında oluşacak olan Concurrency Violation (eş zamanlı uyumsuzluk) istisnası görmezden gelinecektir. İlgili metodumuza ait kodlarımızı aşağıdaki gibi değiştirelim.

```csharp
private void btnGuncelle_Click(object sender, System.EventArgs e)
{
    if(chkContinueUpdateOnError.Checked==true)
    {
        da.ContinueUpdateOnError=true;
        VeriGuncelle();
    }
    else if(chkContinueUpdateOnError.Checked==false)
    {
        da.ContinueUpdateOnError=false;
        VeriGuncelle();
    }
}
```

Şimdi, yine Concurrency olayına neden olacak şekilde değişiklikler yapalım. Yani her iki kullanıcımızda verileri çektikten sonra, birinci kullanıcımız belli bir satırı güncellesin. Ardından ikinci kullanıcımız aynı satırı tekrar güncellemeye çalışsın. Bu durumda her hangibir istisna fırlatılmaz ve uygulama istem dışı bir şekilde sonlanmaz. Dahası, ikinci kullanıcının yaptığı başka değişiklikler eğer var ise veritabanına başarılı bir şekilde yansıtılır.

Ancak halen daha sorunlu olan satıra ait kullanıcı yeterli bilgiye sahip değildir. (Her ne kadar DataGrid bunu ünlem işaretleriyle belirtsede başka kontroller için bu özelliği sağlayamayabiliriz.) Örneğin kullanıcıyı hangi satırların Concurrency Violation (eş zamanlı uyumsuzluk) istisnasına neden olduğu konusunda daha detaylı bir şekilde uyarabiliriz. Burada DBConcurrencyException sınıfının prototipi aşağıdaki gibi olan Row özelliği işimize yarayabilir.

```csharp
public DataRow Row {get; set;}
```

Bu özellik geriye hataya neden olan satırı işaret edebilecek bir DataRow nesne örneği döndürür. Böylece ilgili satıra ait detaylı bilgilere ulaşabiliriz. Ancak, istisnai durum Concurrency'e neden olan ilk satır görüldüğünde devreye girmektedir. Dolayısıyla ikinci kullanıcının elinde Concurrency istisnasına neden olacak birden fazla satır varsa tüm bu satırları yakalamak için alternatif bir yol uygulamamız gerekmektedir. Ado.Net mimarisinde yer alan DataSet, DataTable ve DataRow sınıflarının HasErrors özellikleri bu noktada bizim işimize yarayabilir.

```csharp
public bool HasErrors {get;}
```

Bu özellik bool tipinden olup, herhangibir hata var ise geriye true değerini döndürecektir. Concurrency durumunu bu hatalar arasında sayabiliriz. Şimdi uygulama kodlarımıza aşağıdaki metodu ekleyelim.

```csharp
private void SonHaliAl()
{
    string satirBilgi;
    if(ds.Tables[0].HasErrors)
    {
        foreach(DataRow dr in ds.Tables[0].Rows)
        {
            if(dr.HasErrors)
            {
                satirBilgi=dr["AD"].ToString()+" "+dr["SOYAD"].ToString()+" Başkası tarafından değiştirilmiş. Satırın son halini elde etmek ister misiniz?";
                if(MessageBox.Show(satirBilgi,"Son hali al",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
                {
                    SqlCommand cmdSonHaliAl=new SqlCommand("SELECT * FROM MAILS WHERE ID="+(int)dr["ID"],con);
                    if(con.State==ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlDataReader drGuncelSatir=cmdSonHaliAl.ExecuteReader(CommandBehavior.SingleRow);
                    drGuncelSatir.Read(); 
                    dr.BeginEdit();
                    dr["ID"]=drGuncelSatir["ID"];
                    dr["AD"]=drGuncelSatir["AD"];
                    dr["SOYAD"]=drGuncelSatir["SOYAD"];
                    dr["EMAIL"]=drGuncelSatir["EMAIL"];
                    // Timestamp veya uniqueIdentifier tipinden bir alan kullandıysak (örneğimizdeki KONTROL alanı gibi) onuda güncellememiz gerekir.
                    // dr["KONTROL"]=drGuncelSatir["KONTROL"];
                    dr.EndEdit();
                    con.Close();
                }
            }
        }
        ds.Tables[0].AcceptChanges();
    }
}
```

Bu metod ile ilk olarak dataTable'ın HasErrors özelliğine bakıyoruz. Eğer bir hata var ise, her bir satırı taramaya başlıyoruz. Her bir satırın HasErrors özelliğinin değerine bakarak hatalı satırları, bir başka deyişle Concurrency Violation (eş zamanlı uyumsuzluk)' a neden olanları buluyoruz. Sonra, hatalı satırın primary key olduğunu bildiğimiz ID değerini kullanarak ilgili satırın birinci kullanıcı tarafından güncellenmiş olan halini çekiyoruz. Bunu yaparkende SqlCommand ve SqlDataReader nesnelerimizi kullanıyoruz. Burada ID alanı primary key olduğundan ve benzersiz olarak satırları işaret edebildiğinden tek satır döneceğinden eminiz. Bu nedenle CommandBehavior numaralandırıcısının SingleRow değerini kullandık.

Bu bize performans açısından ekstra zaman kazandıracaktır. Ardından Concurrency Violation (eş zamanlı uyumsuzluk) içinde kalan satırın alanlarına ait değerleri, asıl veritabanından çektiklerimiz ile değiştiriyoruz. İşte bu noktadan sonra eğer kullanıcı tekrarda aynı satırları update eder ise hiç bir problem ile karşılaşmayacaktır. Nitekim, satırların DataRowVersion.Original değerleri veritabanındaki en güncel halleri ile değiştirilmiş olacaktır. Dilersek, ikinci kullanıcının o ana kadar yapmış olduğu ve Concurrency Violation (eş zamanlı uyumsuzluk) altında kalan değişikliklerin tekrardan yazılmasını sağlayabiliriz.

Tek yapmamız gereken Concurrency Violation (eş zamanlı uyumsuzluk)' de kalan alanların o anki değerlerini bir şekilde saklamak, alanların güncel hallerini çekerek orjinal değerleri yeni hallerine set etmek ve son olarak sakladığımız alan değerlerini tekrardan veritabanına göndermektir. Yazdığımız SonHaliAl isimli metodu catch bloğu içerisinde çağırmaktayız. Nitekim Concurrency Violation (eş zamanlı uyumsuzluk) durumları ancak SqlDataAdapter nesnemizin Update metodunu çağırdıktan sonra ortaya çıkan istisna içerisinde ele alınabilir.

```csharp
private void VeriGuncelle()
{
    try    
    {
        // diğer kod satırları
        da.Update(ds);        
    }
    catch(DBConcurrencyException)
    {
        SonHaliAl();
    } 
}
```

Böylece geldik bir makalemizin daha sonuna. Bu makalemizde kısaca bağlantısız katmanda meydana gelebilecek eş zamanlı çakışmaları nasıl ele alabileceğimizi incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hoşçakalın.