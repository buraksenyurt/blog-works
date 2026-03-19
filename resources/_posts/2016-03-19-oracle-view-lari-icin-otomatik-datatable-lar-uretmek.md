---
layout: post
title: "Oracle View' ları için Otomatik DataTable' lar Üretmek"
date: 2016-03-19 18:21:00 +0300
description: "Üzerinde çalıştığımız ve uzun süredir canlı ortamda yaşamakta olan eski bir ürünümüz geçtiğimiz günlerde kod kalite taramalarından birisine girdi. Vaktinde her zaman olduğu gibi alel acele yazılmak zorunda olan kodlar bir kaç ana kategori altında çeşitli tipte ihlallere yakalandı. Bunlardan birisi de Strongly Typed DataSet kullanımına ilişkindi."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - oracle
  - visual-studio
  - dataset
  - datatable
---
# Oracle View' ları için Otomatik DataTable' lar Üretmek
Merhaba Arkadaşlar,

Üzerinde çalıştığımız ve uzun süredir canlı ortamda yaşamakta olan eski bir ürünümüz geçtiğimiz günlerde kod kalite taramalarından birisine girdi. Vaktinde her zaman olduğu gibi alel acele yazılmak zorunda olan kodlar bir kaç ana kategori altında çeşitli tipte ihlallere yakalandı. Bunlardan birisi de Strongly Typed DataSet kullanımına ilişkindi.

Ölçümleme yapan aracın metriklerinden birisi Untyped DataSet kullanımı kabul etmemekte. Bu yüzden uygulama içerisinde View'lar için kullandığımız ne kadar DataTable varsa ihlale girdi..Net tarafında kodlamaya yeni başlayanların sıklıkla düşebileceği bir hata olduğunu ifade edebilir aslında. Typed DataSet/DataTable'ler kod yazımı sırasında tip güvenliğini (Type Safety) de beraberinde getirdiğinden tercih edilmesi gereken sınıflar.

Typed DataSet/DataTable üretimi Visual Studio ortamında çok da zor değil. (Hatta komut satırından da kolayca yapılabilir. [Şu adresten bilgi](https://msdn.microsoft.com/en-us/library/wha85tzb(v=vs.110).aspx) alabilirsiniz) Ancak yapılan üretim sonrası bizim için çok kalabalık kod parçaları oluştuğunu ifade etmek isterim. Oysaki tek ihtiyacımız olan View'ların karşılığı olacak ve Lookup Table gibi kullanılıp sadece veriyi gösterme amaçlı kullanılacak DataTable tipleri idi. Araştırmalarımız sonucu aşağıdaki gibi bir yapının Typed DataTable kullanımı için yeterli olduğunu gördük.

```csharp
    public class ProductTable
        :DataTable
    {
        public DataColumn ID { get; set; }
        public DataColumn URUN_ADI { get; set; }
        public DataColumn URUN_FIYATI { get; set; }
    }
```

Dikkat edilmesi gereken ilk nokta ProductTable tipinin DataTable sınıfından türemiş olmasıdır. Diğer önemli bir nokta ise özelliklerin DataColumn tipinden tanımlanmasıdır. Özellik adları aslında Oracle tarafındaki nesnenin (Table veya View olabilir) kolon adları ile birebir aynıdır.

Tahmn edileceği üzere bir sonraki adım Oracle View'larının her biri için bu tip DataTable türevli tipler üretmekti. Ancak View'ların sayısı oldukça fazlaydı. Bu sınıfları otomatik üretecek bir kod parçası geliştirmek çok daha mantıklıydı. Dolayısıyla aşağıdaki gibi bir kod parçasını kullanamaya ve ilgili sınıfları otomatik olarak ürettirmeye karar verdik. Unutmadan burada Oracle DataAccess Client'ın - ODP.Net'in 4.0 sürümünü kullandığımızı ifade edelim.

```csharp
using Oracle.DataAccess.Client;
using System.Configuration;
using System.IO;
using System.Text;

namespace GenerateDataTables
{
    class Program
    {
        static void Main(string[] args)
        {
            string conStr = ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString;
            string rootPath = ConfigurationManager.AppSettings["RootPath"];
            string getUserViewsQuery = "select view_name from user_views";
            string getColumnNamesQuery = "SELECT column_name FROM user_tab_columns WHERE table_name = :table_name ORDER BY column_id";
            StringBuilder builder = new StringBuilder();

            OracleConnection conn = new OracleConnection(conStr);
            OracleCommand cmdGetViews = new OracleCommand(getUserViewsQuery, conn);
            OracleCommand cmdGetColumnNames = new OracleCommand(getColumnNamesQuery, conn);
            cmdGetColumnNames.Parameters.Add(":table_name", OracleDbType.Varchar2);

            conn.Open();
            OracleDataReader viewReader = cmdGetViews.ExecuteReader();
            while (viewReader.Read())
            {
                builder.AppendLine("using System.Data;\n");
                builder.AppendLine("namespace DBTables{\n");
                builder.AppendFormat("\tpublic partial class {0}DataTable \n\t: DataTable {{ \n", viewReader["view_name"].ToString());
                cmdGetColumnNames.Parameters[":table_name"].Value = viewReader["view_name"].ToString();
                OracleDataReader columnsReader = cmdGetColumnNames.ExecuteReader();
                while (columnsReader.Read())
                {
                    builder.AppendFormat("\t\tpublic DataColumn {0} {{get;set;}}\n", columnsReader["column_name"].ToString());
                }
                columnsReader.Close();
                builder.AppendFormat("\t}}\n");
                builder.AppendFormat("}}");
                File.WriteAllText(string.Format("{0}{1}.cs",rootPath,viewReader["view_name"].ToString()), builder.ToString());
                builder.Clear();
            }
            viewReader.Close();
            conn.Close();
        }
    }
}
```

Kısaca kodda neler yaptığımıza bir bakalım dilerseniz. En önemli nokta kullandığımız iki select sorgusu. İlk sorgumuz ile Connection String'de (app.config dosyasından alıyoruz) yer alan Oracle şemasının erişim yetkisi dahilinde olan View'ları elde etmekteyiz. Bunun için userviews nesnesi kullanılmakta. İkinci Select sorgusu ise her bir View'un kolon adlarını döndürmekte. Bunun için de usertabcolumns db nesnesine gidiyor ve parametre olarak view adını veriyoruz. Böylece üretilecek DataTable türevli sınıfların özelliklerini elde etmiş oluyoruz. StringBuiler sınıfından yararlanarak da DataTable türevli sınıfların içeriğini yazdırmaktayız. Aynı, bir kod editöründe C# sınıfı yazar gibi düşünerek hareket etmemiz önemli. Örneğin kod dosyasının başında using System.Data bildiriminin olması, özelliklerde System.Data.DataColumn yerine DataColumn kullanabilmemize olanak tanıyor. Sonuç olarak Oracle tarafındaki View'lara karşılık gelecek basit DataTable türevli tipleri oluşturmuş bulunuyoruz. Bu işlemlerin arından tek yaptığımız ilgili sınıfları ayrı bir sınıf kütüphanesi altında toplamak oldu.

Yine hatırlatmak gerekirse bu tipler veriyi Oracle tarafından çektikten sonra tutmak ve arayüzlerde göstermek amacıyla geliştirilmiş durumda. Normal şartlarda araçlar ile üretilen Typed DataSet/DataTable'ler çok daha fazla fonksiyonellik (önreğin Insert, Update, Delete işlemleri için) içeriyor. Eğer bu fonksiyonellikere de ihtiyaç varsa ilgili kod üretim parçasını buna göre düzenlemekte yarar var. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutu günler dilerim.