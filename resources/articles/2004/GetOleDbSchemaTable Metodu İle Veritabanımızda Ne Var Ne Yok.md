---
title: "GetOleDbSchemaTable Metodu İle Veritabanımızda Ne Var Ne Yok"
pubDate: 2004-02-12 12:00:00
categories:
  - Ado.Net
tags:
  - Ado.Net
  - C#
  - .NET
  - DataTable
---

# GetOleDbSchemaTable Metodu İle Veritabanımızda Ne Var Ne Yok
Değerli Okurlarım, Merhabalar.

Bu makalemizde, OleDbConnection sınıfına ati olan GetOleDbSchemaTable metodu sayesinde, Sql Veritabanımızdaki varlıklara ait şema bilgilerini nasıl temin edebileceğimizi incelemeye çalışacağız. Çoğu zaman programlarımızda, bağlandığımız veritabanında yer alan tabloların (Tables), görünümlerin (Views), saklı yordamların (Stored Procedures) ve daha pek çok veritabanı nesnesinin bir listesine sahip olmak isteriz. ADO.NET'te yer alan OleDbConnection nesnesine ait GetOleDbSchemaTable metodunu kullanarak bu istediğimiz sonuca varabiliriz.GetOleDbSchema metodu aşağıdaki prototipe sahiptir.

```csharp
public DataTable GetOleDbSchemaTable(Guid schema,object[] restrictions);
```

Dikkat edecek olursanız metodun geri dönüş değeri DataTable tipindedir. Metodun isminde Table kullanılmasının nedenide zaten budur. Yani dönen şema bilgileri bir DataTable nesnesine aktarılacaktır. Metod iki önemli parametreye sahiptir. İlk parametremiz, OleDbSchemaGuid sınıfı türünden bir numaralandırıcı değeri almaktadır. Bu parametreye vereceğimiz değer ile, veritabanından elde etmek istediğimiz şema tipini belirleriz. Örneğin veritabanında yer alan tabloları elde etmek için, OleDbSchemaGuid.Tables değerini veririz. Bunun yanında bu parametreye verebileceğimiz başka önemli değerlerde şunlardır.

OleDbSchemaGuid Özelliği
Açıklaması

OleDbSchemaGuid.Columns
Tablo veya tablolara ait sütun yapısını sağlar.

OleDbSchemaGuid.Procedures
OleDbConnection nesnesinin bağlı olduğu veritabanında yer alan saklı yordamların listesini sağlar.

OleDbSchemaGuid.Views
OleDbConnection nesnesinin bağlı olduğu veritabanında yer alan görünümlerin listesini sağlar.

OleDbSchemaGuid.Indexes
Belirtilen Catalog'da yer alan indexlerin listesini sağlar.

OleDbSchemaGuid.Primary_Keys
Belirtilen tablo veya tablolardaki birincil anahtarların listesini verir.

Tablo 1. OleDbSchemaGuid Üyelerinin Bir Kısmı

OleDbSchemaGuid sınıfı yukarıdaki örnek üyelerinin yanısıra pek çok üyeye sahiptir. Bu üyeler ile ilgili daha geniş bilgi için MSDN kaynaklarından faydalanmanızı tavsiye ederim. GetOleDbSchemaTable metodumuz ikinci bir parametre daha almaktadır. Bu parametre ile şema bilgisi üzerindeki sınırlamaları belirleriz. Bu sayede, ilk parametrede istediğimiz schema bilgilerinin sadece belirli bir tablo içinmi, yada veritabanının tamamı içinmi oluşturulacağına dair sonuçları elde etmiş oluruz. Örneklerimizi incelediğimizde bu parametrelerin ne işe yaradıklarını daha iyi anlayacağınızı düşünüyorum. Şimdi ilk örneğimizi geliştirelim. Basit bir Console uygulaması olarak geliştireceğimiz bu örnekte, Sql sunucumuzda yer alan, Friends isimli veritabanındaki tüm tabloların bir listesini elde edeceğiz. İşte program kodlarımız.

```csharp
using System;

/* OleDb isim uzayını ve Data isim uzayını ekliyoruz. */

using System.Data;
using System.Data.OleDb;
namespace GetOleDbSchemaTables1
{
     class Class1
     {
          static void Main(string[] args)
          {
               /* OleDbConnection nesnemizi oluşturuyoruz. SQLOLEDB provider'ını kullanarak, Sql sunucumuzda yer alan Friends isimli veritabanına bir bağlantı nesnesi tanımlıyoruz. */
               OleDbConnection con=new OleDbConnection("provider=SQLOLEDB;data source=localhost;initial catalog=Friends;Integrated Security=sspi");
               con.Open(); /* Bağlantımızı açıyoruz. */
               DataTable tblTabloListesi; /* GetOleDbSchemaTable metodunun sonucunu tutacak DataTable nesnemizi tanımlıyoruz.*/

               tblTabloListesi=con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,null); /* OleDbConnection nesnemizin, GetOleDbSchemaTable metodunu çalıştırıyoruz ve elde edilen şema bilgisini (ki bu örnekte Friends isimli veritabanındaki tüm tabloların listesini alıyor.) alıyoruz ve DataTable nesnemizin bellete referans ettiği alana aktarıyoruz.*/

               foreach(DataRow dr in tblTabloListesi.Rows) /* DataTable'ımız içindeki satırlarda gezinebileceğimiz bir döngü oluşturuyoruz ve bu döngü içinde her bir satırın TABLE_NAME alanının değerini, dolayısıyla Friends veritabanındaki tablo adlarını ekrana yazdırıyoruz. */
               {
                    Console.WriteLine(dr["TABLE_NAME"]);
               }
          }
     }
}
```

Şimdi uygulamamızı çalıştırdığımızda Friends isimli veritabanında yer alan tüm tabloların ekrana yazıldığını görürsünüz.

![mk53_1.gif](images/mk53_1.gif)

Şekil 1. Friends veritabanındaki tabloların listesi.

Şimdi kodlarımızdaki blTabloListesi=con.GetOleDbSchemaTable (OleDbSchemaGuid.Tables,null); satırını daha yakından incelemeye çalışalım. İlk parametremiz OleDbSchemaGuid.Tables, con isimli OleDbConnection nesnemizin bağlandığı Sql sunucusundaki Friends veritabanından sadece tablo bilgilerini elde etmek istediğimizi göstermektedir. Sınırlandırıcı özelliğe sahip olan ikinci parametremize null değerini vererek tüm tabloların şemaya dahil edilmesini ve DataTable nesnesine aktarılmasını sağlamış olduk. Bu noktada bu iki parametrenin birbirleri ile ilişkil olduklarını söyleyebiliriz. Çünkü, OleDbSchemaGuid parametresinin vereceği tabloların şema yapısı, ikinci parametreye bağlıdır. Şöyleki;

Sınırlama Alanı
Açıklaması

TABLE_CATALOG
Katalog adı. Eğer provider'ımız katalogları desteklemiyorsa null değeri verilir.

TABLE_SCHEMA
Şema adı. Yine provider'ımız şemaları desteklemiyorsa null değeri verilir.

TABLE_NAME
Belirli bir tabloya ait şema bilgileri kullanılacaksa, örneğin bu tabloya ait sütun bilgileri alınıcaksa kullanılır.

TABLE_TYPE
Veritabanından hangi tipteki tabloları alacağımızı belirtmek için kullanılır.

Tablo 2. OleDbSchemaGuid.Tables İçin Sınırlama Parametresi Elemanları

Ancak farzedelimki sadece kullanıcı tanımlı taboların listesine ihtiyacımız var. İşte bu durumda sınırlandırıcı parametremiz devreye girecek. Bunun için, sınırlandırıcı parametremizin son elemanının değerini belirlememiz yeterli olucak. O halde bu işlemi nasıl gerçekleştirebiliriz? Bunun için uygulamamızın kodlarını aşağıdaki şekilde değiştirmemiz yeterlidir.

```csharp
object[] objSinirlama;
objSinirlama=new object[]{null,null,null,"TABLE"};
tblTabloListesi=con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, objSinirlama);
```

![mk53_2.gif](images/mk53_2.gif)

Şekil 2. Sadece Kullanıcı Tanımlı Tabloların Listesi

Burada sınırlandırıcı nesnemizin nasıl tanımlandığına dikkat edin. Bu tanımlama biraz karışık gelebilir. Ancak OleDbSchemaGuid özelliğinin aldığı değere göre şema bilgileri üzerinde bir sınırlama koymak istiyorsak, bu üyenin kullanabileceği değerleri bilmemiz gerekir. Burada object dizimizin son elemanın TABLE değeri atanmıştır. TABLE değeri şema bilgisine sadece kullanıcı tarafından tanımlanmış taboların alınacağını belirtir. Diğer yandan bu dördüncü elemana, ALIAS, SYSTEM TABLE, VIEW, SYSTEM VIEW, SYNONYM, TEMPORARY, LOCAL TEMPORARY değerlerinide verebiliriz. Hepsi, OleDbSchemaGuid.Tables parametresi sonucunda oluşturulacak tablo şema bilgilerinin yapısınıda değiştirecektir. Örneğin, SYSTEM TABLE değerini vermemiz halinde, Friends tablosundaki system tablolarının listesini elde etmiş oluruz. Yada VIEW değerini verdiğimizde kullanıcı tanımlı görünüm nesnelerinin listesini elde etmiş olurduk.

Yukarıdaki örneğimizde, veritabanımızda yer alan tablolara ait schema bilgilerini aldık. Şimdi ise belirli bir tablonun sütun bilgilerini almak istediğimizi farzedelim. Burada sütun bilgileri için, OleDbSchemaGuid.Columns parametresini kullanmamız gerekiyor. Bu durumda sınırlandırıcımızıda bu parametreye göre değiştirmek durumundayız. Eğer ilk örneğimizdeki gibi null değerini verirsek veritabanındaki tüm tabloların kolonlarına ait şema bilgilerini elde etmiş oluruz. Oysaki belirli bir tabloya ait alanlar için şema bilgisi alıcaksak,OleDbSchemaGuid.Columns parametresinin sınırlandırıcı koşullarını değiştirmemiz gerekecektir. İşte bu noktada sınırlandırıcı değişkenimizin üçüncü elemanı olan TABLE_NAME elemanını kullanırız. Bu amaçla kodlarımızı aşağıdaki şekilde değiştirmemiz yeterli olucaktır.

```csharp
object[] objSinirlama;

objSinirlama=new object[]{null,null,"Kitap",null};
tblTabloListesi=con.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, objSinirlama); /* OleDbConnection nesnemizin, GetOleDbSchemaTable metodunu çalıştırıyoruz. Bu kez, Columns değeri sayesinde, sınırlama nesnemizin üçüncü elemanında belirttiğimiz tablo adına ait alanların listesini elde ediyoruz. */
foreach(DataRow dr in tblTabloListesi.Rows)
{
     Console.WriteLine(dr["COLUMN_NAME"]);
}
```

![mk53_3.gif](images/mk53_3.gif)

Şekil 3. Tablomuza ait alanların adları.

Böylece Kitap isimli tablomuzun alanlarına ait schema bilgilerini elde etmiş olduk. Bu örnekte ve bir önceki örnekte dikkat ederseniz, DataTable nesnesindeki belirli bir alanın değerini ekrana yazdırdık. Bu örnekte, COLUMN_NAME, önceki örnekte ise, TABLE_NAME. Elbette elde ettiğimiz şema bilgilerinde sadece bu alanlar yer almıyor. Örneğin Friends veritabanındaki kullanıcı tanımlı tablolara ait başka ne tür bilgilerin şemaya alındığına bir bakalım. Bu amaçla, DataTable nesnemize aktarılan şema bilgilerine ait satırlardaki tüm alanları bir döngü ile gezmemiz yeterli olucaktır. Bunu aşağıdaki kodlar ile gerçekleştirebiliriz.

```csharp
object[] objSinirlama;

objSinirlama=new object[]{null,null,null,"TABLE"};
tblTabloListesi=con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, objSinirlama);
foreach(DataRow dr in tblTabloListesi.Rows)
{
     for(int i=0;i<=tblTabloListesi.Rows.Count;++i)
     {
          Console.Write(dr[i]);
          Console.Write("---");
     }
     Console.WriteLine("");
}
```

![mk53_4.gif](images/mk53_4.gif)

Şekil 4. Şemadaki diğer bilgiler.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
