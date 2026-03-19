---
title: "Ado.Net 2.0' da Transaction Kavramı"
pubDate: 2005-04-17 12:00:00
categories:
  - Ado.net 2.0
tags:
  - Ado.net 2.0
  - C#
  - Bash
  - .NET
  - ADO.NET
  - Transactions
  - Generics
---

# Ado.Net 2.0' da Transaction Kavramı
Değerli Okurlarım, Merhabalar.

Transaction kavramı ve kullanımı veritabanı programcılığının olmazsa olmaz temellerinden birisidir. Veritabanına doğru gerçekleştirilen işlemlerin tamamının onaylanması veya içlerinden birisinde meydana gelecek bir istisna sonrası o ana kadar yapılan tüm işlerin geri alınması veri bütünlüğünü korumak açısından son derece önemlidir. Ado.Net 1.0/1.1 için transactionların kullanımı, seçilen veri sağlayıcısına göre farklı sınıfların kullanılmasını gerektirir.

Örneğin SqlClient isim alanındaki sınıfları kullandığınız bir veritabanı uygulamanız var ise, SqlTransaction sınıfını kullanırsınız. Oysa Ado.Net 2.0' da transaction mimarisi Ado.Net'ten ayrıştırılmış, bir başka deyişle provider'lardan bağımsız hale getirilmiştir. Aslında en büyük değişiklik transaction işlemlerinin artık System.Transactions isim alanı altında yer alan sınıflar ile gerçekleştirilecek olmasıdır. Bir diğer büyük değişiklik transaction'ların yazım tekniği ile ilgilidir. Ado.Net 2.0 da transaction oluşturacak ve kullanacak kodları çok daha basit biçimde yazabilirsiniz. Bunu ilerleyen paragraflarda sizde göreceksiniz.

Ado.Net 2.0' da transaction'ların kullanımı ile ilgili belkide en önemli özellik dağıtık (distributed) transaction'ların uygulanış biçimidir. Normal şartlarda Ado.Net 1.0/1.1 için dağıtık transaction'ları kullanırken System.EnterpriseServices isim alanını kullanan COM+ nesnelerini oluşturur ve ContextUtil sınıfına ait metodlar yardımıyla dağıtık transaction'ları kontrol altına alırız. Bu yapı özellikle yazımı ve oluşturulması itibariyle karmaşık olup kullanımı da zor olan bir yapıdır. Oysa ki Ado.Net 2.0 olayı çok daha akıllı bir şekilde ele alır. Ado.Net 2.0' a göre, iki tip transaction olabilir. Tek veri kaynağı üzerinde çalışan LightWeight Transaction'lar ve dağıtık transactionlar gibi davranan OleTx Transaction'lar.

LightWeight Transaction'lar tek bir uygulama alanında (application-domain) çalışan iş parçalarıdır. OleTx tipi Transaction'lar ise birden fazla uygulama alanında (application-domain) veya aynı uygulama alanında iseler de farklı veri kaynaklarını kullanan transaction'lar dır. Dolayısıyla OleTx tipi transaction'ları Distributed Transaction'lara benzetebiliriz. Ancak arada önemli farklarda vardır. İlk olarak Ado.Net 1.0/1.1' de tek veri kaynağı üzerinde çalışan bir transaction'ın nasıl uygulandığını hatırlayalım.

```csharp
SqlConnection con = new SqlConnection(connectionString);
con.Open();
SqlCommand cmd = new SqlCommand(sqlSorgusu,con);
SqlTransaction trans;

trans = con.BeginTransaction();
cmd.Transaction = trans;
try
{
    cmd.ExecuteNonQuery();
    trans.Commit();
}
catch(Exception e)
{
    trans.Rollback();
}
finally 
{
    con.Close();
}
```

Yukarıdaki örnekte yerel (local) makine üzerinde tekil olarak çalışan bir transaction için gerekli kodlar yer almaktadır. Eğer komutun çalıştırılması sırasında herhangi bir aksilik olursa, catch bloğu devreye girer ve transaction geri çekilerek (RollBack) o ana kadar yapılmış olan tüm işlemler iptal edilir. Tam tersine bir aksilik olmaz ise transaction nesnesinin Commit metodu kullanılarak işlemler onaylanır ve veritabanına yazılır. Gelelim bu tarz bir örneğin Ado.Net 2.0' da nasıl gerçekleştirileceğine. Her şeyden önce bu sefer System.Transactions isim alanını kullanmamız gerekiyor. Şu anki versiyonda bu isim alanını uygulamamıza harici olarak referans etmemiz gerekmekte.

![mk120_1.gif](images/mk120_1.gif)

Daha sonra ise aşağıdaki kodlara sahip olan Console uygulamasını oluşturalım.

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

#endregion

namespace Transactions
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TransactionScope tsScope = new TransactionScope())
            {
                using (SqlConnection con = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Personel (AD,SOYAD,MAIL) VALUES ('Burak Selim','ŞENYURT','selim(at)buraksenyurt.com')", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    tsScope.Complete();
                }
            }
        }
    }
}
```

Görüldüğü gibi Ado.Net 1.0/1.1' e göre oldukça farklı bir yapı kullanılmıştır. TransactionScope sınıfına ait nesne örnekleri kendisinden sonra açılan bağlantıları otomatik olarak bir transaction scope (faaliyet alanı) içerisine alır. Buradaki temel mantık bir veya daha fazla transaction'ı kullanacak olan bir scope (faaliyet alanı) oluşturmak ve hepsi için gereki bir takım özelliklerin ortak olarak belirlenmesini sağlamaktır. TransactionScope sınıfına ait nesne örneklerini oluşturabileceğimiz pek çok aşırı yüklenmiş (overload) yapıcı (constructor) metod mevcuttur.

Bu yapıcı metotlar yardımıyla, scope (faaliyet alanı) için pek çok özellik tanımlayabilirisiniz. TransactionScope, IDisposable arayüzünü (interface) uygulayan bir sınıftır. Bir TransactionScope nesnesi oluşturululduğunda ve bu nesnenin oluşturduğu faaliyet alanına ilk transaction eklendiğinde devam eden komutlara ilişkin transaction'larda otomatik olarak var olan bu scope (faaliyet alanı) içerisinde gerçekleşmektedir. Bu elbetteki varsayılan durumdur. Ancak dilerseniz TransactionScopeOption numaralandırıcısı (enumerator) yardımıyla, yeni açılan transaction'ların var olan scope'a (faaliyet alanına) dahil edilip edilmeyeceğini belirleyebilirsiniz.

Eğer veritabanına doğru çalışan komutlarda herhangi bir aksaklık olursa uygulama otomatik olarak using bloğunu terk edecektir. Bu durumda son satırdaki Complete metodu çağırılabilir hale gelecektir. Bu da transaction içerisindeki işlemlerin commit edilebileceği anlamına gelir. Bu yeni teknik, eskisine göre özellikle kod yazımını hem kolaylaştırmış hem de profesyonelleştirmiştir. Bununla birlikte var olan alışkanlıklarımızdan birisi meydana gelecek aksaklık nedeni ile kullanıcının bir istisna mekanizması ile uyarılabilmesini sağlamak veya başka işlemleri yaptırmaktır. Dolayısıyla aynı örneği aşağıdaki haliyle de yazabiliriz.

```csharp
using (TransactionScope tsScope = new TransactionScope())
{
    SqlConnection con = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI");
    SqlCommand cmd = new SqlCommand("INSERT INTO Personel (AD,SOYAD,MAIL) VALUES ('Burak Selim','ŞENYURT','selim(at)buraksenyurt.com')", con);
    try
    {
        con.Open();
        cmd.ExecuteNonQuery();
        tsScope.Complete();
    }
    catch (TransactionException hata)
    {
        Console.WriteLine(hata.Message.ToString());
    }
    finally
    {
        con.Close();
    }
}
```

Burada oluşabilecek istisnayı yakalamak istediğimiz için bir try-catch-finally bloğunu kullandık. Ancak dikkat ederseniz TransactionScope nesnemiz yine using bloğu içerisinde kullanılmıştır ve transaction'ı commit etmek için Complete metodu çağırılmıştır. Her iki örnekte LightWeight Transaction tipindedir. Çünkü tek bir connection ve yine tek bir application domain mevcuttur. Elbette birden fazla komutun yer aldığı transaction'larda aynı teknik kullanılarak oluşturulabilir. Ancak farklı veritabanlarına bağlanan aksiyonlar söz konusu ise Transaction'ların oluşturulması ve arka planda gerçekleşen olaylar biraz farklıdır. Şimdi bu durumu örneklemek için aşağıdaki Console uygulamasını oluşturalım.

```csharp
using (TransactionScope tsScope = new TransactionScope())
{
    using (SqlConnection conAdventureWorks = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
    {
        SqlCommand cmdAdvPersonel = new SqlCommand("INSERT INTO Personel (AD,SOYAD,MAIL) VALUES ('Burak Selim','ŞENYURT','selim(at)buraksenyurt.com')", conAdventureWorks);
        conAdventureWorks.Open();
        cmdAdvPersonel.ExecuteNonQuery();
        using (SqlConnection conNorthwind = new SqlConnection("data source=localhost;database=Northwind;integrated security=SSPI"))
        {
            conNorthwind.Open();
            SqlCommand cmdNrtPersonel = new SqlCommand("UPDATE Personel SET AD='Gustavo' WHERE ID=1", conNorthwind);
            cmdNrtPersonel.ExecuteNonQuery();
        }
    }
    tsScope.Complete();
}
```

Görüldüğü gibi ilk yazdığımız örnekten pek farkı yok. Sadece iç içe geçmiş (nested) bir yapı var. Aynı application domain'e ait iki farklı database bağlantısı ihtiyacı olduğu için burada bir distributed transaction kullanılması gerekiyor. Normalde DTC (Distributed Transaction Coordinator) kontrolünde ele alınacak bu transaction'ları oluşturmak için Ado.Net 1.0/1.1' de bayağı uğraşmamız gerekecekti. Oysa ki Ado.Net 2.0' da herhangi bir şey yapmamıza gerek yok. Çünkü Ado.Net 2.0 otomatik olarak OleTx tipinde bir transaction oluşturacaktır. Nasıl mı? Bunu gözlemlemenin en iyi yolu, uygulama koduna breakpoint koymak ve Administrative Tool->Component Services'dan açılacak olan transaction'ları izlemekle olacaktır. İlk olarak kodumuza bir breakpoint koyalım ve adım adım uygulamamızda ilerleyelim.

![mk120_3.gif](images/mk120_3.gif)

Sarı noktaya gelinceye kadar ve conNorthwind isimli bağlantı Open metodu ile açılıncaya kadar aktif olan tek bir Connection nesnesi vardır. Buraya kadar transaction'ımız LightWeight tipindedir. Ancak ikinci bağlantıda açıldıktan sonra bu bağlantı TransactionScope nesnemize otomatik olarak eklenecektir. İşte sarı noktada iken Administrative Tool->Component Services'a bakarsak, TransactionList içerisinde, DTC kontrolü altında kullanıcı tanımlı bir transaction'ın otomatik olarak açıldığını görürüz.

![mk120_2.gif](images/mk120_2.gif)

Artık her iki connection üzerinde çalışan komutlar DTC altında oluşturulan bu Connection'ın kontrolü altındadır. İşlemler başarılı bir şekilde tamamlanırsa TransactionScope nesnesine ait using bloğunun sonundaki kod satırı çalışacaktır. Yani Complete metodu yürütülecektir. Bu sayede işlemler Commit edilir ve böylece tüm işlemler onaylanarak veritabanlarına yazılır. Using bloğundan çıkıldıktan sonra ise, DTC kontrolü altındaki bu transaction otomatik olarak kaldırılır. DTC kontrolü altında oluşturulan transaction'lar her zaman unique bir ID değerine sahip olur. Böylece sunucu üzerinde aynı anda çalışan birden fazla distributed transaction var ise, bunların birbirlerinden ayırt edilmeleri ve uygun olan application domain'ler tarafından ele alınmaları sağlanmış olur.

TransactionScope nesnesinin belirlediği scope (faaliyet alanı) altında açılan transaction'lar bir takım özelliklere sahiptir. Örneğin eskiden olduğu gibi IsolationLevel değerleri veya TimeOut süreleri vardır. Dilersek oluşturulacak bir TransactionScope nesnesinin ilgili değerlerini önceden manuel olarak ayarlayabilir böylece bu scope (faaliyet alanı) içindeki transaction'ların ortak özelliklerini belirleyebiliriz. Bunun için TransactionOptions sınıfına ait nesne örnekleri kullanılır.

```csharp
TransactionOptions trOptions = new TransactionOptions();
trOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
trOptions.Timeout = new TimeSpan(0, 0, 30);
using (TransactionScope tsScope = new TransactionScope(TransactionScopeOption.RequiresNew,trOptions))
{
    using (SqlConnection con = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
    {
        SqlCommand cmd = new SqlCommand("INSERT INTO Personel (AD,SOYAD,MAIL) VALUES ('Burak Selim','ŞENYURT','selim(at)buraksenyurt.com')", con);
        con.Open();
        cmd.ExecuteNonQuery();
        tsScope.Complete();
    }
}
```

Yukarıdaki örnekte, oluşturulacak olan transaction'ın izolasyon seviyesi ve zaman aşımı süreleri belirlenmiş ve TransactionScope nesnemiz bu opsiyonlar çerçevesinde aşağıdaki overload metot versiyonu ile oluşturulmuştur.

```csharp
TransactionScope tsScope = new TransactionScope(TransactionScopeOption.RequiresNew,trOptions)
```

Burada ilk parametre birden fazla TransactionScope nesnesinin yer aldığı iç içe geçmiş yapılarda büyük önem arzetmektedir. Bu seçenek ile yeni açılan transaction scope'un (faaliyet alanının) var olan önceki bir transaction faaliyet alanına katılıp katılmayacağı gibi seçenekler belirlenir. Örneğin aşağıdaki basit yapıyı ele alalım. Burada ilk using bloğu ile bir Transaction Scope oluşturulur. İkinci using ifadesine gelindiğinde yeni transaction scope'un önceki transaction scope'a ilave edileceği TransactionScopeOption parametresi ile belirlenir. Nitekim Required değeri, yeni scope'u var olan önceki scope'a ekler. Eğer var olan bir scope yok ise yeni bir tane oluşturur. Elbetteki burada akla gelen soru scope içindeki transaction'ların kimin tarafından onaylanacağıdır. Burada root scope kim ise ona ait Complete metodu devreye girecektir.

```csharp
using(TransactionScope faaliyetAlani1 = new TransactionScope())
{
    ...
    using(TransactionScope faaliyetAlani2 = new TransactionScope(TransactionScopeOption.Required))
    {
        ...
    }
}
```

![mk120_4.gif](images/mk120_4.gif)

Şimdi yukarıdaki nested scope yapısı içine üçüncü bir scope daha ilave edelim. Ancak yeni TransactionScope için TransactionScopeOption değerini RequiresNew olarak belirleyelim.

```csharp
using(TransactionScope faaliyetAlani1 = new TransactionScope())
{
    ...
    using(TransactionScope faaliyetAlani2 = new TransactionScope(TransactionScopeOption.Required))
    {
        ...
    }
    using(TransactionScope faaliyetAlani3 = new TransactionScope(TransactionScopeOption.RequiresNew))
    {
        ...
    }
}
```

Bu durumda yapımız aşağıdaki gibi olacaktır.

![mk120_5.gif](images/mk120_5.gif)

Dilersek transaction'ları açıkça (explicit) kendimizde manüel olarak oluşturabiliriz. Şu ana kadar yaptığımız örneklerde implicit bir yaklaşım izledik. Yani ilgili transaction ve bunlara ait kaynakların otomatik olarak oluşturulmasını sağladık. Örneğin aşağıdaki kodlarda transaction'lar manuel olarak oluşturulmuştur. (Örnek.Net 2.0 Beta sürümünde denenmiştir.)

```csharp
ICommittableTransaction trans = Transaction.Create();
try
{
    using (SqlConnection conNorthwind = new SqlConnection("data source=localhost;database=Northwind;integrated security=SSPI"))
    {
        SqlCommand cmdInsert = new SqlCommand("INSERT INTO Personel (AD,SOYAD) VALUES ('Burak Selim','Şenyurt')", conNorthwind);
        conNorthwind.Open();
        conNorthwind.EnlistTransaction(trans);
        cmdInsert.ExecuteNonQuery();
    }
    using (SqlConnection conAdv = new SqlConnection("data source=localhost;database=AdventureWorks;integrated security=SSPI"))
    {
        SqlCommand cmdInsert = new SqlCommand("INSERT INTO Personel (AD,SOYAD,MAIL) VALUES ('Cimi','Keri','cimi@keri.com')", conAdv);
        conAdv.Open();
        conAdv.EnlistTransaction(trans);
        cmdInsert.ExecuteNonQuery();
    }
    trans.Commit();
}
catch
{
    trans.Rollback();
}
```

ICommittableTransaction arayüzü bir Transaction Scope'un oluşturulmasını sağlar. Bunun için Transaction sınıfına ait Create metodu kullanılır. Create metodu varsayılan ayarları ile birlikte bir Scope oluşturacaktır. Eğer bu Scope'a transaction'lar eklemek istersek, ilgili bağlantıları temsil eden Connection nesnelerinin EnlistTransaction metodunu kullanırız. EnlistTransaction metodu parametre olarak transaction Scope'u temsil eden ICommittableTransaction arayüzü tipinden nesne örneğini alır. Elbette arayüze eklenen transaction'lara ait işlemlerin onaylanmasını sağlamak için arayüze ait Commit metodu kullanılır. Tam tersine bir sorun çıkar ve veritabanına doğru yapılan işlemlerden birisi gerçekleştirilemez ise o ana kadar yapılan işlemlerin geri alınması ICommittableTransaction arayüzüne ait RollBack metodu ile sağlanmış olur.

Bu makalemizde Transaction mimarisinin Ado.Net 2.0' daki yüzünü incelemeye çalıştık. Görüldüğü gibi kod yazımının basitleştirilmesinin yanında, özellikle EnterpriceServices bağımlılığından kurtularak Distributed Transaction'ların otomatik hale getirilmesi ve Transaction Scope kavramının getirilmesi göze çarpan önemli özellikler. Burada bahsedilen özellikler teorik olarak fazla bir değişikliğe uğramayacaktır. Ancak bazı üyelerin isimlerin değişiklik beklenmektedir. Örneğin ICommittableTransaction arayüzü yerine CommittableTransaction sınıfının geleceği düşünülmektedir. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
