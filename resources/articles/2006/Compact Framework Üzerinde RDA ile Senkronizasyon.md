---
title: "Compact Framework Üzerinde RDA ile Senkronizasyon"
pubDate: 2006-01-24 12:00:00
categories:
  - C#
tags:
  - C#
  - SQL Server
  - HTTP
  - IIS
  - Transactions
  - DataTable
---

# Compact Framework Üzerinde RDA ile Senkronizasyon
Değerli Okurlarım Merhabalar,

Mobil uygulamalarda veri senkronizasyonu son derece önemlidir. Örneğin bir kurye firmasının dağıtım elemanını göz önüne alalım. Mobil cihaz ile donatılmış olan bu personelin görevi, kendisine verilen teslimat bilgilerine göre, sipariş sahiplerine ürünlerini teslim etmektir. Her çalışma gününün başında, teslimat yapacağı kişilerin bilgilerini ana sunuculardan mobil cihazına yükleyerek teslimat için hareket geçen personel, tamamlanan teslimatları anında veya belirli bir süre sonunda topluca asıl sunuculara göndererek gerekli güncelleştirme işlemlerini yapar. Bu senaryo gereği personelin teslimat bilgilerini düzenli olarak alması, tamamlanaları işaretlemesi ve son olarak bu bilgileri asıl sunucuya göndermesi gerekir.Bu noktada senaryonun iki ucunda yer alan sunucu ve mobil tarafındaki veriler arasındaki güncelliği çeşitli senkranizasyon mimarileri ile sağlayabiliriz.

Mobile uygulamalarda uzak sunucular (Server) üzerindeki veriler ile yerel (Client/Mobil) veriler arasındaki senkronizasyon işlemini iki farklı mimaride gerçekleştirebiliriz. Bunlardan birisi RDA (Remote Data Access) diğeri ise Merge Replication'dır. RDA kullanımı Merge Replication'a göre daha kolay olan bir mimaridir. Biz bu makalemizde RDA'yı her yönüyle incelemeye çalışacağız. RDA mimarisi Sql Server 6.5 sürümü ve üstünü hedef almıştır. RDA mimarisinde, mobil platform ile uzak sunucu üzerindeki veritabanı arasında senkronizasyon 3 teknikle sağlamaktır. Bu teknikler Push, Pull ve SubmitSql teknikleridir. Bu tekniklere geçmeden önce, RDA ' nın çalışma şekli hakkında kısa bir bilgi vermekte fayda olacağı düşüncesindeyim.

![mk145_1.gif](images/mk145_1.gif)

RDA mimarisinde istemci tarafında yer alan Sql CE veritabanı, Client Agent'ı kullanarak, sunucu tarafındaki Sql Server CE Server Agent üzerinden uzak Sql Server veritabanına erişebilir. Bu erişim çift yönldür. İstemci, sunucudan veri çekebileceği gibi, sunucuya veri gönderebilir. Hatta RDA mimarisine özel olaraktan, sunucu üzerinde doğrudan sql ifadelerini çalıştırabilir. Bu aslında veri göndermeye benzer bir işlemdir. Diğer taraftan Push metodu ile, yerel veritabanında sadece değişiklik yapılmış satırların sunucuya gönderilmesi sağlanabilir. SubmitSql ile ise, doğrudan sunucu üzerindeki verilerde değişiklik yapılmaktadır.

RDA mimarisinde işlemleri başlatan her zaman mobil tarafındaki uygulamadır. Diğer taraftan RDA, HTTP üzerinden IIS'i kullanarak Sql Server sunucularına gerekli yetkiler dahilinde erişebilir. RDA mimarisini kullanabilmek için sunucu tarafında Sql Server CE Server Agent servisinin kurulu olması ve gerekli yapılandırma ayarlarının oluşturulması gerekmektedir. Bu servis aslında IIS üzerinde bulunan ve sscesa20.dll (Sql Server Compact Edition Server Agent 2.0) isimli IIS API dll'ini kullanır. Sunucu üzerinde çalışan bu servis, mobil tarafında çalışan Sql Server Client Agent ile karşılıklı iletişimin sağlanmasında önemli bir görev üstlenmektedir. Dolayısılay gerekli senkronizasyonun sağlanmasında önemli bir rol üstlenir. Sql Server CE Server Agent'ın sisteme kurulması bir takım basit ayarlamalar gerektirir. Bu ayarlamaların nasıl yapıldığına ilişkin olarak sevgili Tolga Güler'in daha önceki [makalesinden](http://www.csharpnedir.com/makalegoster.asp?MId=216) yararlanabilirsiniz. Gerekli sistem entegrasyonu sağlandıktan sonra RDA işlemleri gerçekleştirilebilir.

RDA işlemleri için yönetimli kod (Managed Code) tarafında System.Data.SqlServerCe isim alanında yer alan SqlCeRemoteDataAccess sınıfı kullanılmaktadır. Bu sınıfa ait metodları kullanarak veri alma, veri çekme gibi işlemler gerçekleştirilebilir. Bu işlemler sırasında elbetteki sürekli bir bağlantı gerekmektedir. Ancak bu süreklilik RDA mimarisinin sürekli bir bağlantı gerektirdiğini göstermez. Nitekim senkronizasyonun asıl amaçlarından biriside, aralında belirli aralıklarla bağlantı sağlanan yapılarda, istemci ve sunucu arasındaki veri bütünlüğünü sağlamaktır. Teorik olarak RDA mimarisine göre, veri aktarımı sırasında bağlantının kesildiği durumlarda sistem, bağlantı tekrardan sağlayıncaya kadar veri paketinin son gönderildiği yeri aklında tutar. Bağlantı tekrardan sağlanır sağlanmaz veriler kaldığı yerden gönderilmeye devam eder.

RDA mimarisinde istemci tarafına veri çekmek için Pull metodu, sunucu tarafına veri göndermek için Push metodu, sunucu üzerinde doğrudan sql cümlesi çalıştırmak için ise SubmitSql metodu kullanılır. SqlCeRemoteDataAccess sınıfına ait olan bu metodlara ilişkin temel özellikleri ve dikkat edilmesi gereken noktaları şöyle sıralayabiliriz.

Pull
Push
SubmitSql

LookUp tabloların mobil tarafa aktarılmasında kullanılır.(Sadece LookUp tablolar kullanılacağı zaman Pull metodu ile güncelleme işlemi gerekmez.)
Mobil tarafından sunucuya veri gönderilmesinde bir başka deyişle var olan mobil bilgilerin sunucuda güncellenmesinde kullanılır.
Doğrudan sunucu üzerinde sql cümlesi çalıştırabilir.

Pull işleminden önce mobil tarafta yer alan ilgili tablo silinmelidir. Aksi durumda istisna (Exception) alınır.
Pull metodu ile çekilen veriller için Tracking özelliği aktif ise sadece değişiklikleri göndererek kaynak kullanımını azaltır.
Insert, Update, Delete işlemleri dışında geriye sonuç kümesi döndürmeyen sp'leride çalıştırabilir.

Eğer Pull ile alınan tabloda güncelleme yapılacak ve Push ile geri gönderiecek ise Tracking özelliği aktif hale getirilmelidir.
Sunucu tarafında tracking işlemi yapılmayacağından, güncelleme işleminden sonra verilerin son hali için tekrardan Pull metodunu uygulamak gerekebilir
Veri değişiklikleri doğrudan sunucu tarafında yapılır.

Mobil tarafa alınacak her bir tablo için ayrı ayrı Pull metodları çağırılmalıdır.

Şimdi RDA işlemlerini basit örnekler ile incelemeye çalışalım. Öncelikle tüm işlemlerimiz için SqlCERemoteDataAccess sınıfına ait nesne örneğinin oluşturulması gerekmektedir. Bu nesne örneğinin InternetURL özelliği ile, uzak sunucu üzerinde sscesa20.dll dosyasını hizmete sunan http adresi belirtilir. Bir başka deyişle Sql Server CE Server Agent'a hangi adres ile erişileceği belirtilmektedir. Bu mutlaka yapılmalıdır.

![dikkat.gif](images/dikkat.gif)
RDA mimarisinde güvenlik IIS tarafında ve Sql Server tarafında önem kazanır. Eğer Sql Server CE Server Agent servisine erişme yönetim olarak Anonymous Users güvenliği seçilmemiş ise, belirli bir kullanıcı adı ve şifre ile bağlanabilmek için InternetLogin ve InternetPassword özellikleride set edilmelidir.

LocalConnectionString özelliği ilede, mobil tarafında kullanılacak olan bağlantı bilgisi tanımlanır. Bu bağlantı bilgisi tipik olarak mobil cihaz üzerindek Sql Server CE veritabanına bağlanmak için gereken bağlantı bilgisidir. Buna göre SqlCERemoteDataAccess sınıfına ait bir nesne örneğini aşağıdaki gibi oluşturabiliriz.

```csharp
private SqlCeRemoteDataAccess rda;

private void CreateRDAObject()
{
    rda=new SqlCeRemoteDataAccess();
    rda.InternetUrl="http://192.168.7.3/SQLCeRemote/sscesa20.dll";
    rda.LocalConnectionString="Data Source=Ambar.sdf";
}
```

Bu örnek kod parçasında InternetURL bilgisinin ve LocalConnectionString bilgisinin nasıl verildiğine dikkat edin. (Buradaki sscesa20.dll'e ait url bilgisi ile yerele veritabanı adı ver yolu kendi sistemlerinizde daha farklı olabilir.) SqlCeRemoteDataAccess nesnesinin oluşturulmasından sonra, Pull, Push ve SubmitSql metodlarını kullanabiliriz. Bu metodları kullanmak için, mutlaka uzak sunucudaki veritabanına erişilecek bağlantı bilgisi verilmelidir. Bu metodların örnek kullanımlarını aşağıdaki kod parçasında görebilirsiniz.

Pull;

```csharp
private void Pull()
{
    // Mobile taraftaki veritabanı yok ise oluşturulur. var ise silinip oluşturulur.
    
    rda.Pull("Musteriler","SELECT * From Customers","Provider=SqlOleDb;data source=192.168.7.3;database=Northwind;uid=sa;password=1234",RdaTrackOption.TrackingOn);
    rda.Dispose();
}
```

Pull metodunun uygulanışını gördüğünüz yukarıdaki kod parçasında, ilk parametre ile mobile tarafta yer alan veritabanındaki tablo adı belirtilir. İkinci parametre uzak sunucudaki tablodan veri çekmek için çalıştırılacak olan sql cümlesidir. Üçüncü parametrede ise, bu sorguyu çalıştırabilmek için gerekli uzak sunucu bağlantı bilgisi verilmektedir. (Bağlantı bilgisinin tanımlarken SqlOleDb provider'ının seçtiğimize dikkat edin.) Son olarak pull işleminden sonra eğer güncelleme amacıyla Push işlemi uygulanacak ise, mobil tabloda olan değişikliklerin gönderilebilmesi amacıyla izleme özelliği aktif hale getirilmiştir.

Push;

```csharp
private void Push()
{
    // Mobile taraftaki veritabanı üzerinde çeşitli güncelleme işlemleri yapılır.

    rda.Push("Musteriler","Provider=SqlOleDb;data source=192.168.7.3;database=Northwind;uid=sa;password=1234",RdaBatchOption.BatchingOn);
    rda.Dispose();
}
```

Push metodunun uygulanışını gördüğünüz yukarıdaki kod parçasında, ilk parametre ile yerel tablo belirtilir. Bu tablo, mobil cihaz üzerinde yer alan veritabanı uzarındaki tablo işaret edilir. İkinci parametre uzak sunucu üzerindeki veritabanına erişebilmek için gerekli bağlantı bilgisini içerir. Son parametre ise, tracking işlemi ile izlenen yerel tablo değişikliklerinin, sunucuya toplu olarak tek bir transaction içerisinde gönderilip gönderilemiyeceğini belirtmektedir.

SubmitSql;

```csharp
private void SubmitSql()
{
    rda.SubmitSql("Insert Into Personel (Ad,Maas) Values ('Burak',1000)","Provider=SqlOleDb;data source=192.168.7.3;database=Northwind;uid=sa;password=1234");
    rda.Dispose();
}
```

SubmitSql metodunun kullanılışını gösteren yukarıdaki örnek, bağlantı bilgisi ile belirilenen uzak veritabanı üzerinde basit bir insert işlemi gerçekleştirmektedir. Aşağıdaki kod parçası RDA kullanımına ilişkin genel bir örneği sunmaktadır. Bu örnekte, Northwind veritabanında yer alan Region isimli tablodan Pull tekniği ile veri çekilmesi, çekilen verinin Mobil form üzerindeki bir ListBox'a bağlanması, belli bir satır üzerinde güncelleme işleminin yapılarak Push metodu ile sunucuya gönderilmesi ve doğrudan sunucu üzerinde örnek bir insert işleminin gerçekleştirilmesine ilişkin kodlamalar yapılmıştır. Uygulamamız Pocket PC 2002 (2003) Emulatorleri üzerinde test edilmiş olan bir Smart Device Windows Application dır.

![mk145_2.gif](images/mk145_2.gif)

Uygulama Kodlarımız;

```csharp
using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlServerCe;
using System.Data.Common;

namespace UsingRDA
{
    public class Form1 : System.Windows.Forms.Form
    {
        private SqlCeRemoteDataAccess rda;
        private System.Windows.Forms.ListBox lstRegions;
        private System.Windows.Forms.Button btnPush;
        private System.Windows.Forms.Button btnSubmitSql;
        private SqlCeEngine cEngine;

        private void CreateLocalDatabase()
        {
            // RDA Mimarisine özel olarak Pull işlemlerinden önce yerel veri tabanı dosyasının silinmesi gerekmektedir. Bu silme işlemi için standart System.IO isim alanının File sınıfını kullanabiliriz.
            if(System.IO.File.Exists("Ambar.sdf"))
            {
                System.IO.File.Delete("Ambar.sdf");
            }
            // Mobil taraftaki Sql Server Ce veritabanını oluşturmak için System.Data.SqlServerCe isim alanında yer alan SqlCeEngine sınıfını kullanıyoruz. Bu sınıf ile var sayılan olarak mobil cihazın root klasöründe Ambar.sdf isimli veritabanı dosyamızı oluşturmaktayız.
            cEngine=new SqlCeEngine("data source=Ambar.sdf");
            cEngine.CreateDatabase();
        }

        private void CreateRDAObject()
        {
            // SqlCeRemoteDataAccess nesnemizi oluşturuyoruz. InternetURL özelliğine Server Agen t' ın adresini, LocalConnectionString özelliğine ise mobil veritabanının bağlantı     bilgisini veriyoruz.
            rda=new SqlCeRemoteDataAccess();
            rda.InternetUrl="http://192.168.7.3/CESrvAgent/sscesa20.dll";
            rda.LocalConnectionString="Data Source=Ambar.sdf";
        }

        private void Pull()
        {
            CreateRDAObject();
            // Pull işleminden önce mobil taraftaki veritabanımızı silip(var ise) oluşturuyoruz.
            CreateLocalDatabase();
            // Pull metodumuzu çalıştırıyoruz.
            rda.Pull("Bolgeler","SELECT RegionID,RegionDescription From Region","Provider=SqlOleDb;data         source=192.168.7.3;database=Northwind;uid=sa;password=1234",RdaTrackOption.TrackingOn);
            // SqlCeRemoteDataAccess sınıfımıza ait nesne örneğimize ait bellek kaynağını serbest bırakıyoruz.
            rda.Dispose();
        }

        private DataTable LoadToDataTable()
        {
            // Yerel veritabanına yüklenen Regions tablosuna ait satırları bir DataTable nesnesine aktarıyoruz. Burada amacımız gelen verileri Form üzerindeki ListBox kontroünde     göstermek. Bunun için basit olarak SqlCeDataAdapter sınıfını kullanılıyor.
            DataTable dtRegion=new DataTable();
            SqlCeDataAdapter daRegion=new SqlCeDataAdapter("Select RegionID,RegionDescription From Bolgeler","data source=Ambar.sdf");
            daRegion.Fill(dtRegion); // NOT : Fill ve diğer SqlCeDataAdapter üyelerini kullanabilmemiz için uygulamanıza System.Data.Common isim alanını açıkça referans etmemiz     gerektiğini unutmayınız. 
            return dtRegion;
        }

        private void LocalChange()
        {
            // Ambar.sdf isimli yerel veritabanı üzerinden Bolgeler isimli tablodaki 2 numaralı satırın içeriğini değiştirecek bir update sorgusu çalıştırıyoruz. Bu sorgu ile yaptığımız yerel güncelleme işlemini daha sonradan Push metodu ile sunucuyada göndereceğiz.
            SqlCeConnection conn=new SqlCeConnection("data source=Ambar.sdf");
            SqlCeCommand cmdUpdate=new SqlCeCommand("UPDATE Bolgeler SET RegionDescription='Western (Batı)' WHERE RegionID=2",conn);
            try
            {
                conn.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            catch(SqlCeException excp)
            {
                MessageBox.Show(excp.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        } 

        private void Push()
        {
            CreateRDAObject();
            // Simulasynon amacıyla Yerel değişikliklerimizi yapıyoruz.
            LocalChange();
            // Yerel olarak yapılan değişiklikleri uzak sunucu üzerindeki veri kaynağına aktarıyoruz. Bolgeler isimli yerel tablodan, yapılan değişiklikleri ki (sadece 2 numaralı satıra ait update işlemi var), 192.168.7.3 ip adresindeki Northwind veritabanına aktarıyoruz.
            rda.Push("Bolgeler","Provider=SqlOleDb;data source=192.168.7.3;database=Northwind;uid=sa;password=1234",RdaBatchOption.BatchingOn);
            // SqlCeRemoteDataAccess sınıfımıza ait nesne örneğimize ait bellek kaynağını serbest bırakıyoruz.
            rda.Dispose();
        }

        private void SubmitSql()
        {
            CreateRDAObject();
            // Doğrudan uzak sunucuya erişip, Region isimli tabloya yeni bir satır ekleyecek şekilde bir sql sorgusunu uzak sunucu üzerinde çalıştırıyoruz.
            rda.SubmitSql("Insert Into Region (RegionID,RegionDescription) Values (5,'Marmara')","Provider=SqlOleDb;data source=192.168.7.3;database=Northwind;uid=sa;password=1234");
            // SqlCeRemoteDataAccess sınıfımıza ait nesne örneğimize ait bellek kaynağını serbest bırakıyoruz.
            rda.Dispose();
        }

        private void BindToListBox()
        {
            lstRegions.DataSource=LoadToDataTable();
            lstRegions.DisplayMember="RegionDescription";
            lstRegions.ValueMember="RegionID";
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            Pull();
    
            // Pull metodu ile çektiğimiz verileri yerel Sql Server Ce database' inden okuyup bir DataTable' a aktardığımız metodu kullanarak, listBox kontrolüne bağlıyoruz.
            BindToListBox();
        }

        private void btnPush_Click(object sender, System.EventArgs e)
        {
            // Önce yerel değişiklikleri sunucuya gönderiyoruz. Daha sonra sunucuda olan son hali tekrardan mobil tarafa alıyoruz.
            Push();
            Pull();
            BindToListBox();
        }

        private void btnSubmitSql_Click(object sender, System.EventArgs e)
        {
            //Önce doğrudan uzak sunucu üzerinde veri değişikliği yapıyoruz. Sonra uzak sunucudan son hali mobil tarafa alıyoruz.
            SubmitSql();
            Pull();
            BindToListBox();
        }

        // Form ile ilişkili işlemler. Main, InitializeComponent vs...
    }
}
```

Örnek, RDA mimarisinin kolay anlaşılabilmesi amacıyla çok fazla detay girmemiştir. Örneğin, try...catch ile exception yönetim mekanizması daha da genişletilebilir, Region tablosuna girilecek verilere ait bilgiler form üzerindeki kontrollerden alınabilir...vb...

> ![download.gif](images/download.gif)
>
> RDA mimarisinin uygulanışına ilişkin geniş bir örneğe ait Smart Device Application Solution'ı buradan [indirebilirsiniz](assets/UsingRDA.rar).

Buraya kadar anlatıklarımızdan yola çıkaraktan RDA mimarisinin belirli avantajlarını ve dezavantajlarını şu şekilde sıralayabiliriz.

Avantajlar
DezAvantajlar

Sunucu tarafındaki veri kaynları daha az kullanılır. (Bu fark özellikle Merge Replication mimarisine göre belirgin derecede azdır.
Sunucu üzerindeki değişiklikler izlenmemektedir. Bunun yerine sadece istemci tarafındaki değişikler izlenir.

Sql Server 6.5 ve üzeri sql sistemlerini destekler.
Özellikle Pull işleminden önce istemci tarafındaki (Sql CE üzerindeki) tablolar silinmek zorundadır.

RDA için gerekli sistem entegrasyonunun yapılması kolaydır.
İstemci tarafına alınacak her bir tablo için ayrı ayrı Pulling işlemi gerektirir.

Böylece geldik bir makalemizin daha sonuna. İzleyen makalelerimizde, Merge Replication mimarisini incelemeye çalışacağız. Uygulanması ve hazırlığı RDA mimarisine göre biraz daha zor olan Merge Replication mimarisinin avantajlarını ve dezavantjlarını göreceğiz. Bu makalemizde görüşünceye dek hepinize mutlu günler dilerim.
