---
title: "Ado.Net 2.0 ve Toplu Güncelleme İşlemleri (Batch-Updates)"
pubDate: 2004-09-18 12:00:00
categories:
  - Ado.net 2.0
tags:
  - Ado.net 2.0
  - C#
  - Bash
  - .NET
  - ADO.NET
  - Oracle
  - Performance
  - Generics
  - DataTable
---

# Ado.Net 2.0 ve Toplu Güncelleme İşlemleri (Batch-Updates)
Değerli Okurlarım Merhabalar,

Toplu güncelleştirme işlemleri, birden fazla sql ifadesinin (insert,update,delete,select gibi) arka arkaya gelecek şekilde ancak tek bir seferde çalıştırılmasını baz alan bir tekniktir. Ado.Net 2.0 ile, toplu güncelleştirme işlemlerine daha fazla fonksiyonellik kazandırılmıştır. Bu koşul elbetteki toplu güncelleştirme işlemlerini destekeleyen veritabanı sunucuları üzerinde geçerli olmaktadır. Şu an için, yönetimsel kodda yer alan Oracle ve Sql nesnelerinin desteklediği bu fonksiyonelliği kazanmak için aşağıda prototipi verilen ve SqlDataAdapter yada OracleDataAdapter sınıflarına ait olan, UpdateBatchSize özelliği kullanılmaktadır.

```csharp
public override int UpdateBatchSize {get;set;}
```

Bu özellik bir anlamda, DataAdapter nesnesinin Update komutu ile veritabanına doğru yapılacak güncelleme işlemlerinin toplu olarak hangi periyotta gerçekleştirileceğini belirtir. Örneğin, 1 varsayılan değeridir ve her bir güncelleme işleminin (insert,update veya delete) her satır için ayrı ayrı yapılacağını belirtir. Daha derin düşünecek olursa, örneğin Sql Sunucusunda yer alan sp_executesql stored procedure'ünün her bir satır için birer kez ilgili komutu (Insert gibi) çalıştıracağını belirtir.

Diğer yandan, bu özelliğe 0 değerini verdiğimizde tüm güncelleme işlemleri tek bir seferde gerçekleştirilir. Bir başka deyişle veritabanına doğru n sayıda güncelleme işlemi varsa, Sql Sunucusunda yer alan sp_executesql stored procedure'ü bu n sayıdaki işlemleri içeren toplu bir komut kümesini tek bir seferde çalıştırılacaktır. Ayrıca UpdateBatchSize özelliğine 0 ve 1 haricinde verilecek olan pozitif değerler, her bir toplu güncelleştirme işleminin kaç iç komut içereceğini belirtmektedir. Konuyu daha kolay bir şekilde anlayabilmek için basit bir Console uygulması geliştirelim.

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

#endregion

namespace BatchUpdates
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI");
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM MailList", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataRow dr;
            for (int i = 1; i <= 5; i++)
            {
                dr = dt.NewRow();
                dr["AD"] = "AD_" + i.ToString();
                dr["SOYAD"] = "SOYAD_" + i.ToString();
                dr["MAIL"] = "MAIL_" + i.ToString();
                dt.Rows.Add(dr);
            }
            da.UpdateBatchSize = 1;
            SqlCommandBuilder cm = new SqlCommandBuilder(da);
            da.Update(dt);
            Console.WriteLine("İŞLEMLERİN SONU");
            Console.ReadLine();
        }
    }
}
```

Bu uygulamada Ado.Net 1.1 ile yapabildiğimiz işlemlerden farklı bir şey yoktur. Yukon üzerinde yer alan MailList tablomuza SqlDataAdapter nesnesi vasıtasıyla 5 adet satır giriyoruz. Bizim için önemli olan UpdateBatchSize değerinin 1 olarak belirtilmesidir. Uygulamamızı çalıştırmadan önce, Sql Profiler aracını kullanarak yeni bir Trace başlatalım ve Sql Sunucumuzda gerçekleşen işlemleri izlemeye çalışalım. Trace'imiz çalışırken uygulmamamızı yürütecek olursak, Sql Sunucusu üzerinde aşağıdaki olayların gerçekleştirildiğini görürüz.

![mk94_1.gif](images/mk94_1.gif)

Şekil 1. UpdateBatchSize değeri 1 olduğunda.

Dikkat edecek olursanız, sp_executesql stored procedure'ü girilen her satır için insert komutunu birer kez çalıştırmıştır. Bunun nedeni UpdateBatchSize özelliğinin 1 değerine sahip olmasıdır. Eğer bu değeri 0 yapıp tekrar çalıştırırsak, bu takdirde kaç satır girersek girerlim tüm satırlar için geçerli olan insert komutları tek bir toplu-komut olarak işlenecek ve tek bir seferde çalıştırılacaktır. Örnek olarak, döngümüzün değerini 20 satır insert edilecek şekilde ayarladığımızı düşünürsek, UpdateBatchSize özelliğine 0 değerini vermek ile, 20 satır için parametre alacak tek bir stored procedure'ü çağırmış oluruz. Uygulamamızda bu kez tüm satırları update ettiğimizi düşünelim ve UpdateBatchSize özelliğine 0 değerini verelim.

```csharp
for (int i = 0; i < dt.Rows.Count; i++)
{
    dr = dt.Rows[i];
    dr["AD"] ="_DEGISTI";
}

da.UpdateBatchSize = 0;
```

Şimdi Sql Profiler'da Trace'imizdeki işlemlere bakacak olursa, kaç satır güncellenmiş ise her bir satır için yapılan update işlemlerinin tek bir toplu-komut kümesinde gerçekleştirildiğini görürüz.

![mk94_2.gif](images/mk94_2.gif)

Şekil 2. UpdateBatchSize özelliğine 0 değeri verildiğinde.

Elbette daha önceden bahsettiğimiz gibi UpdateBatchSize özelliğine 0 ve 1 haricinde pozitif değerlerde verebiliriz. Bu durumda toplu-komut kümeleri belirtilen sayı kadar iç komut içerecektir. Örneğimizde, UpdateBatchSize değerini 7 yaparsak, her bir sp_executesql çağrısında, içsel olarak 7 satırlık işlem içeren toplu-komut kümeleri olduğunu görürüz.

da.UpdateBatchSize = 7;

![mk94_3.gif](images/mk94_3.gif)

Şekil 3. UpdateBatchSize değerini pozitif her hangibir sayı olarak belirlediğimizde.

Örneklerdende görüldüğü gibi, Ado.Net 2.0 toplu-komut güncelleme işlemlerine daha fazla fonkisyonellik katmak amacıyla kullanışlı bir özellik kazanmıştır. Bazı durumlarda, güncelleme işlemlerinin bağlantısız katmandan, veritabanına doğru olan hareketlerinde toplu olarak yapılması network trafiğini olumlu yönde etkileyecek bir gelişmedir. Çünkü, tüm güncelleme hareketleri için veritabanına doğru sadece tek bir tur atılacaktır. Elbetteki devasa boyutlara sahip olan veri kaynakları üzerinde yapılacak büyük çaplı güncelleme işlemlerinde, toplu-komut kümelerini belirli sayılarda komut içerecek şekilde ayarlamakta performans açısından olumlu bir etki yaratacaktır.

Bu makalemizde, kısaca toplu-güncelleştirme (Batch-Update) işlemlerine değinmeye çalıştık. İlerleyen makalelerimizde, Ado.Net 2.0' ın yeni özelliklerine bakmaya devam edeceğiz. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
