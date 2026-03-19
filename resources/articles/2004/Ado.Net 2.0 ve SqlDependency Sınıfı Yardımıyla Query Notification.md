---
title: "Ado.Net 2.0 ve SqlDependency Sınıfı Yardımıyla Query Notification"
pubDate: 2004-10-25 21:00:00
categories:
  - Ado.net 2.0
tags:
  - Ado.net 2.0
  - C#
  - Bash
  - .NET
  - ADO.NET
  - SQL Server
  - Delegates
  - Generics
  - DataTable
---

# Ado.Net 2.0 ve SqlDependency Sınıfı Yardımıyla Query Notification
Değerli Okurlarım Merhabalar,

Çoğu zaman istemci uygulamalarda, kullanıcıya sunduğumuz verilerin yer aldığı tablolarda başka kullanıcılar tarafından gerçekleştirilen değişikliklerin anında görünmesini isteriz. SqlDependency sınıfı sayesinde artık, bir veritabanında meydana gelen değişiklikleri (şu an için Sql Server ve Yukon) anında yakalayabilme şansına sahibiz. İşte bugünkü makalemizde aslında son derece derin ve geniş bir konu olan Sql Server Notification meselesini çok basit bir örnek ve Ado.Net 2.0 ile gelen yeni sınıflardan birisi olan SqlDependency yardımıyla incelemeye çalışacağız. İlk olarak şu soruyu düşünelim.

Sunucu üzerinde ne gibi gelişmeler olduğunda anında haberdar olmak isteyebiliriz?

Verilerde meydana gelebilecek değişikler; bir başka deyişle yeni satırların eklenmesi, satırların silinmesi veya güncellenmesi.

Tablo üzerinde Drop, Alter veya Delete işlemlerinin uygulanması.

Sql sunucusunun yeniden başlatılması.

Sql sunucusu üzerinde hatalar oluşması nedeniyle sunucunun işleyişinin durması.

İşte bu gibi durumlarda, istemci uygulamaların meydana gelecek olan değişikliklerden anında haberdar olmasını isteyebiliriz. Bu durum özellikle, bağlantısız katman nesnelerinin kullanıldığı uygulamalarda büyük önem kazanmaktadır. Peki bu anında haber bildirilmesini nasıl sağlayabiliriz? Bunun için kullanılan mimari kabaca aşağıdaki şekilde olduğu gibidir.

![mk105_1.gif](images/mk105_1.gif)

Şekil 1. Mimari.

Dilerseniz kısaca mimarinin işleyişinden ve bileşenlerinden bahsedelim. SqlDependency sınıfı, bir SqlCommand ile ilişkilendirilerek kullanılır ve içsel olarak bir SqlNotificationRequest nesnesinide barındırır. (Bu sınıfi ilerleyen makalelerimizde incelemeye çalışacağız.) SqlCommand sınıfının Notification işlemine uyan bir Select sorgusu içermesi gerekir. Öyleki bu Select sorgusu şu anki versiyonlar itibariyle aşağıdaki gibi olamaz.

```csharp
Select * From Tablo
```

Select sorgusunda talep edilen alan isimleri açıkça belirtilmelidir. Bununla birlikte, Tablo'nun sahibide yani yaratıcısıda açıkça belirtilmelidir. Yani ancak aşağıdaki gibi bir sql ifadesi Notification desteğini sağlayabilir.

```csharp
Select Ad,Soyad From dbo.Tablo
```

Belirtilen Sql ifadesi çalıştırıldığında, SqlDependency nesnesi ilgili SqlCommand nesnesi için sunucuda bir Notification açılacağını sql sunucusuna bildirir. Sql sunucusu bu mesaj üzerine ilgili sql komutunu çalıştırır ve Notification nesnesini sunucuya register eder. Bu bize, çalıştırılan sql sorgusunun sunucaya ekstradan bir takım bilgiler daha gönderdiğini göstermektedir. Bu bilgiler çoğunlukla, kaydı tutulacak Notification ile ilgilidir ve SqlDependency sınıfının ilgili yapıcı metodları yardımıyla ayarlanabilir.

Notification nesnesi, sunucuya kayıt edildikten sonra, komutun çalışması ile elde edilen sonuçlar ön belleğe alınır ve ilgili Notification nesneside Server Service Broker Queue tarafından kuyruğa atılır. Buradaki ön bellekleme işlemi aslında bir temp tablosunun oluşturulmasından başka bir şey değildir. Bu noktadan itibaren Sql sunucusu, ilgili satır kümesini izlemeye başlar. Eğer satır kümesi üzerinde yukarıda bahsettiğimiz durumlardan birisi nedeni ile değişiklikler olursa, sp_DispatcherProc sistem prosedürü yardımıyla istemci uygulamanın SqlDependency sınıfına ait olan OnChange olayına bilgi mesajı gider.

Mimari her ne kadar karışık gibi görünsede, uygulanabilirliği son derece kolay ve basittir. Bu amaçla olayı daha iyi irdeleyebilmek için aşağıdaki Windows uygulamasını göz önüne alalım.

![mk105_2.gif](images/mk105_2.gif)

Şekil 2. Windows Uygulamamız

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

#endregion

namespace NotifySample
{
    partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection con;
        SqlCommand cmd;
        SqlDependency dep;
        SqlDataAdapter da;
        DataTable dt;

        private void btnVeriCek_Click(object sender, EventArgs e)
        {
            con = new SqlConnection("data source=localhost;initial catalog=Dukkan;integrated security=SSPI");
            con.Open();
            cmd = new SqlCommand("Select DetayID,PersonelID,Ad,Soyad,Mail From dbo.PersonelDetay", con);
            dep = new SqlDependency(cmd);
               SqlDependency.Start("data source=localhost;initial catalog=Dukkan;integrated security=SSPI");
            dep.OnChange+=new OnChangeEventHandler(dep_OnChange);
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dgVeriler.DataSource = dt;
        }

        void dep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            MessageBox.Show("YENI VERILER EKLENDI...TAZELEME YAPIN");
        }
    }
}
```

Uygulamamız, Yukon üzerinde Dukkan isimle veritabanında yer alan PersonelDetay isimli tabloya ait verileri kullanmaktadır. Kullanıcı Veri Çek başlıklı butona bastığında ilgili tabloya ait tüm satırlar DataGridView kontrolüne yüklenmektedir. Şimdi gelelim, burada önemli olan kod satırlarına. Dikkat ederseniz, SqlDependency sınıfına ait nesne örneğimizi aşağıdaki kod satırı ile oluşturuyoruz. Böylece ilgili komut için Notification servisinin kullanılacağını belirtmiş oluyoruz.

```csharp
dep = new SqlDependency(cmd);
```

Bu arada Select sorgumuzun kurallara uygun bir yapıda olduğuna dikkat edelim. Yani, alan adlarını ve tablonun sahibini açıkça belirtiyoruz. Daha sonra ise, SqlDependency sınıfımıza OnChange olayını aşağıdaki kod satırı ile ekliyoruz.

```csharp
dep.OnChange+=new OnChangeEventHandler(dep_OnChange);
```

Bu olayın bizim için büyük bir önemi var. Nitekim, sunucu üzerinde ön belleklenen veri kümesinde değişiklikler olduğunda, sql sunucusu tarafından uygulmaya bir Notification mesajı gönderilecektir. İşte bu mesajın ele alınacağı olay SqlDependency sınıfına ait OnChange'dır. Bu olayın prototipi aşağıdaki gibidir.

```csharp
public event OnChangeEventHandler OnChange;
```

Gördüğünüz gibi bu olay OnChangeEventHandler temsilcisinin tanımladığı metodları çalıştırabilir. OnChangeEventHandler temsilcisinin prototipi ise aşağıdaki gibidir.

```csharp
public sealed delegate void OnChangeEventHandler( object sender, SqlNotificationEventArgs e );
```

Bu temsilcinin işaret edeceği metodlara ait ikinci parametre SqlNotificationEventArgs sınıfı tipinden olup oldukça önemlidir. Nitekim bu parametre yardımıyla, gelen bildirinin hangi olaya istinaden oluştuğunu ayırt edebilir ve işlemlerimize yön verebiliriz. Bu sınıfıda detaylı bir şekilde ilerleyen makalelerimizde inceleyeceğiz. Olay metodumuzda ise basit olarak herhangibir Notification mesajı alındığında bir MessageBox'ın çıkmasını sağlıyoruz.

```csharp
void dep_OnChange(object sender, SqlNotificationEventArgs e)
{
     MessageBox.Show("YENI VERILER EKLENDI...TAZELEME YAPIN");
} 
```

Şimdi uygulamamızı çalıştıralım ve verileri çekelim. Yazdığımız kodlar gereği, Sql suncusu üzerinde Select sorgumuza istinaden bir Notification nesnesi register edilecektir. Şimdi biz bu noktada doğrudan Yukon üzerinden yeni bir satır gireceğiz. (Aslında aynı uygulamanın başka bir örneğinden veya başka bir uygulamadanda bu tablo üzerinde değişiklikler yapabiliriz) Bunu uygulamamız çalışıyorken yapacağız.

![mk105_3.gif](images/mk105_3.gif)

Şekil 3. Yukon üzerinden tablomuza yeni bir satır ekliyoruz.

Bu işlemi gerçekleştirdiğimiz anda, uygulamamıza bir bildiri mesajı iletilir ve tabloda değişiklik olduğu söylenir. Bunun sonucu olarakta, SqlDependency nesnemiz ile ilişkilendirdiğimiz OnChange olayı aktif hale gelecektir. Sonuç aşağıdaki gibi olacaktır. Yeni satır eklenir eklenmez uygulama OnChange olayını çalıştıracaktır.

![mk105_4.gif](images/mk105_4.gif)

Şekil 4. Notification mesajı uygulamaya gönderildi.

Dolayısıyla biz, veri çekme işlemimizi bu olay içerisine alarak istemci uygulamanın daima güncel veriler ile konuşmasını sağlayabiliriz. SqlDependency sınıfının pek çok üyesi ve önemli özelliği vardır. Bir sonraki makalemizde, bu sınıfı daha detaylı bir şekilde incelemeye çalışırken, gerçekleşen değişikliğin tipine göre nasıl hareket edebileceğimizide göreceğiz. Tekrardan görüşünceye dek sağlıcakla kalın.
