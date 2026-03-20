---
layout: post
title: "Connection (Bağlantı) Kavramı ve OleDbConnection Sınıfı"
date: 2004-02-13 06:00:00 +0300
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - aspnet
  - sql-server
  - oracle
  - authentication
  - transactions
  - visual-studio
  - dataset
---
Bu makalemizde, ADO.NET mimarisinde temel yapı taşı olan Connection (Bağlantı) kavramına kısaca değinecek ve OleDbConnection sınıfını incelemeye çalışacağız. ADO.NET mimarisinde, pek çok sınıfın veri kaynakları ile olan iletişiminde Connection (Bağlantı) nesnelerini kullanırız. Örneğin, bir veri kayağındaki tablolara ait verileri, DataSet sınıfından bir nesne örneğine taşımak istediğimizi düşünelim. Bu dataSet nesnesini dolduracak olan DataAdapter sınıfına, sahip olduğu sql sorgusunun veya komutunun işleyeceği bir hattı belirtmemiz gerekir. İşte burada devreye Connection (Bağlantı) nesnelerimiz girer. Yada bir Command sınıfı nesnesi yardımıyla veritabanı üzerindeki bir saklı yordamı (stored procedure) çalıştırmak istediğimizi düşünelim. Bu durumda komutun çalıştırılabileceği bir hattı veri kaynağımız ile Command nesnesi arasında sağlamamız gerekir. İşte Connection (Bağlantı) nesnemizi kullanmamız için bir sebep daha.

Verdiğimiz bu basit örneklerdende anlaşıldığı gibi, Connection (bağlantı) sınıfları, veri kaynağına bir hat çekerek, ADO.NET nesnelerinin bu hat yardımıyla işlemlerini gerçekleştirmelerine imkan sağlarlar. Ancak sahip olunan veri kaynağının türüne göre, ADO.NET içerisine değişik Connection sınıfları eklenmiştir. DotNet'in ilk sürümünde OleDbConnection ve SqlConnection nesneleri ile bu hatlar temin edilirken,.NET Framework 1.1 sürümü ile birlikte, OdbcConnection ve OracleConnection sınıflarıda ADO.NET kütüphanelerine dahil edilerek Odbc ve Oracle veri kaynaklarına bağlantılar sağlanması imkanı kazandırılmıştır.

OleDbConnection sınıfı ile, bir OleDb Data Provider (veri sağlayıcısı) üzerinden, ole db destekli veri kaynaklarına erişim sağlayabiliriz. SqlConnection sınıfı Sql Sunucularına doğrudan bağlantı sağlar. Aynı şekilde OracleConnection sınıfıda Oracle veri kaynaklarına doğrudan erişim sağlar. OdbcConnection sınıfı ise odbc destekli veri kaynaklarına erişim için kullanılır. Bu makalemizde bu bağlantı sınıflarından OleDbConnection sınıfını inceleyeceğiz. OleDbConnection sınıfı, ADO.NET sınıflarının, Ole Db desteği olan veri kaynaklarına erişebilmesi amacıyla kullanılır. Veri kaynağının tipi tam olarak bilinmediği için, arada bu işlevi ayırt etmeye yarayan bir COM+ nesnesi yer almaktadır. OleDbConnection sınıfına ait bir nesne iletişim kurmak istediği veri kaynağına ait ole db veri sağlayıcısını belirtmek durumundadır. Bunu daha iyi kavramak için aşağıdaki şekle bakalım.

![mk54_1.gif](/assets/images/2004/mk54_1.gif)

Şekil 1. OleDbConnection ile Veri Kaynaklarına Bağlantı.

Görüldüğü gibi bir OleDbConnection nesnesi öncelikle bir Ole Db Data Provider (Ole Db Veri Sağlayıcısı) ile iletişim kurar. Ardından bu veri sağlayıcı istenen veri kaynağına erişerek, gerekli hattı tesis etmiş olur. Peki bu işlemi nasıl gerçekleştireceğiz. İşte tüm Connection nesnelerinin en önemli özelliği olan ConnectionString özelliği bu noktada devreye girmektedir. Kısaca ConnectionString özelliği ile, veri kaynağı ile sağlanacak olan iletişim hattının kurulum bilgileri belirlenir. OleDbConnection sınıfı için ConnectionString özelliği aşağıdaki prototipe sahiptir.

```text
public virtual string ConnectionString {get; set;}
```

ConnectionString özelliği belirlenmiş bir OleDbConnection sınıfı nesne örneğini açtığımızda, yani veri kaynağına olan hattı kullanılabilir hale getirdiğimizde, bu özellik yanlız-okunabilir (read-only) hale gelir. Dolayısıyla açık bir OleDbConnection nesnesinin ConnectionString özelliğini değiştiremezsiniz. Bunun için bu bağlantıyı tekrardan kapatmanız gerekecektir. ConnectionString özelliği, bir takım anahtar-değer çiftlerinin noktalı virgül ile ayırlmasından oluşturulan string bir bilgi topluluğudur. ConnectionString özelliği içinde kullanabileceğimiz bu anahtar-değer çiftlerinin en önemlisi Provider anahtarıdır. Bu anahtara vereceğimiz değer, hangi tip ole db veri sağlayıcısını kullanmak istediğimizi belirtmektedir.

Örneğin Sql sunucusuna, Sql Ole Db Provider ile bağlanmak istersek, Provider anahtarına, SQLOLEDB değerini atarız. Provider anahtarı mutlaka belirtilir. Daha sonraki anahtar-değer çiftleri ise bu Provider seçimine bağlı olarak değişiklik gösterecektir. Veri kaynağına hangi tip Ole Db Veri Sağlayıcısından bağlandığımızı seçtikten sonra, bağlanmak istediğimiz veri kaynağıda belli olmuş olucaktır. Sırada bu veri kaynağının adını veya adresine belirteceğimiz anahtar-değer çiftlerinin belirlenmesi vardır. Örneğin bir Sql Sunucusuna bağlanıyorsak, sunucu adınıda Ole Db Data Provider (Veri Sağlyacısı) 'na bildirmemiz gerekir. Bunun için Data Source anahtarını kullanırız. Bununla birlikte bağlandığımız veri kaynağı, sql yada oracle gibi bir veritabanı yönetim sistemi değilde, Access gibi bir tablolama sistemi ise, Data Source anahtarı, tablonun fiziki adresini alır. Sql ve Oracle gibi sunuculara yapılacak bağlantılarda Provider ve Data Source seçiminin yanında hangi veritabanına bağlanılacağınıda Initial Catalog anahtarı yada Database anahtarı ile belirleriz. Bunların dışında veri kaynağına yapılacak olan bağlantının güvenlik ayarlarınıda belirtiriz. Çoğunlukla Integrated Security gibi bir anahtara True değerinin atandığını görürüz. Bu anahtar, veri kaynağına bağlanmak istenen uygulama için, makinenin windows authentication ayarlarına bakıldığını belirtir. Dolayısıyla sql sunucusuna bağlanma yetkisi olan her windows kullanıcısı bu bağlantıyı sağlayabilir. Ancak istersek belli bir kullanıcı adı veya şifresi ilede bir veritabanına bağlantı açılmasını sağlayabiliriz. Bunun için ise, User ID ve Password anahtarlarını kullanırız.

Buraya kadar bahsettiklerimiz kavramsal açıklamalardır. Dilerseniz basit örnekler ile konuyu daha iyi açıklamaya çalışalım. Örneklerimizi Console uygulamaları şeklinde gerçekleştireceğiz. İlk örneğimizde, Sql Sunucusundaki veritabanı için, bir bağlantı hattı oluşturup açacağız.

```csharp
using System;

using System.Data.OleDb; /* OleDbConnection sınıfı, Data.OleDb isim uzayında yer almaktadır. */

namespace OleDbCon1
{
     class Class1
     {
          static void Main(string[] args)
          {
               OleDbConnection conFriends=new OleDbConnection(); /* OleDbConnection nesnemiz oluşturuluyor. */

               /* ConnectionString özelliği belirleniyor. Provider (Sağlayıcımız) SQLOLEDB. Bu bir sql sunucusuna bağlanmak istediğimizi belirtir. Data Source anahtarına localhost değerini atayarak, sunucunun yerel makinede olduğunu belirtiyoruz. Ancak buraya başka bir adreste girilebilir. Sunucunuz nerede ise oranın adresi. Database ile, bağlantı hattının açılacağı veritabanını belirliyoruz. Burada sql sunucumuzda yer alan Friends veritabanına bağlantı hattı açıyoruz. Son olarak Integrated Security=SSPI anahtar-değer çifti sayesinde Windows Doğrulaması ile sunucuya bağlanabileceğimizi belirtiyoruz. Yani sql sunucusuna bağlanma yetkisi olan her windows kullanıcısı bu hattı tesis edebilecek.*/

               conFriends.ConnectionString="Provider=SQLOLEDB;Data Source=localhost;Database=Friends;Integrated Security=SSPI";

               try
               {
                    conFriends.Open(); /* Open metodu ile oluşturduğumuz iletişim hattını kullanıma açıyoruz. */
                    Console.WriteLine("Bağlantı açıldı...");
                    conFriends.Close(); /* Close metodu ilede oluşturulan iletişim hattını kapatıyoruz. */   
                    Console.WriteLine("Bağlantı kapatıldı...");
               }
               catch(Exception hata)
               {
                    Console.WriteLine(hata.Message.ToString());
               }
          }
     }
}
```

![mk54_2.gif](/assets/images/2004/mk54_2.gif)

Şekil 2. Uygulamanın Çalışmasının Sonucu.

Aynı örnekte bu kez belli bir kullanıcı ile bağlanmak istediğimizi düşünelim. Bu durumda ConnectionString'imizi aşağıdaki şekilde değiştirmemiz gerekir. Bu durumda User ID ve Password anahtarlarına gerekli kullanıcı değerlerini atarız.

```csharp
conFriends.ConnectionString="Provider=SQLOLEDB;Data Source=localhost;Database=Friends;User Id=sa;Password=CucP??80.";
```

Şimdide bir Access tablosuna nasıl bağlanabileceğimizi görelim. Bunun için ConnectionString özelliğimizi aşağıdaki gibi yazarız.

```csharp
OleDbConnection conYazarlar=new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;data source=c:\\Authors.mdb");

try
{
     conYazarlar.Open();
     Console.WriteLine("Yazarlar Access veritabanına bağlantı açıldı...");
     conYazarlar.Close();
     Console.WriteLine("Yazarlar Access veritabanına olan bağlantı kapatıldı...");
}
catch(Exception hata)
{
     Console.WriteLine(hata.Message.ToString());
}
```

![mk54_3.gif](/assets/images/2004/mk54_3.gif)

Şekil 3. Access Veritabanına Bağlatı.

Bu örnekte dikkat ederseniz ConnectionString özelliğini, OleDbConnection nesnemizin diğer yapıcı metodu içerisinde parametre olarak belirledik. Ayıraca, Provider olarak bu kez Microsoft.Jet.OLEDB.4.0 'ı seçerek, bir Access veritabanına bağlanmak istediğimizi bu Ole Db Provider'a bildirmiş olduk. Daha sonra bu veri sağlayıcı componenti, Data Source anahtarındaki değere bakarak, ilgili adresteki veritabanına bir hat çekti.

Başka bir ConnectionString anahtarıda File Name dir. Bu anahtara bir udl uzantılı dosya adresi vererek, bağlantı hattının bu dosyadaki ayarlar üzerinden gerçekleştirilmesini sağlayabiliriz. Bir udl dosyası Data Link Properties (veri linki özellikleri) özelliklerini belirler. Böyle bir dosya oluşturmak son derece basittir. Bir text editor ile boş bir dosya açın ve onu udl uzantısı ile kaydedin. Bu durumda dosyamızın görünümü şu şekilde olucaktır.

![mk54_4.gif](/assets/images/2004/mk54_4.gif)

Şekil 4. Bir udl dosyasının görüntüsü.

Bu dosyayı açtığımızda hepimizin aşina olduğu veritabanı bağlantı seçenekleri ile karşılaşırız.

![mk54_5.gif](/assets/images/2004/mk54_5.gif)

Şekil 5. Connection ayarları.

Bu penceredeki adımları takip ederek bir ConnectionString'i bir udl dosyasına kaydedebilir ve OleDbConnection nesnemiz için sadece File Name anahtarına bu değeri vererek ilgili bağlantının, bu udl dosyasındaki ayarlar üzerinden gerçekleştirilmesini sağlayabiliriz. İlk yapmamız gereken ConnectionString özelliğinde olduğu gibi, Provider (Sağlayıcı) seçimidir.

![mk54_6.gif](/assets/images/2004/mk54_6.gif)

Şekil 6. Provider Seçimi.

Burada örnek olarak Sql Server Provider'ımızı seçelim. Sonraki adımda ise sırasıyla sunucu adımızı (Server Name), sunucuya hangi kimlik doğrulaması ile bağlanacağımızı ve veritabanımızın adını seçeriz. Son olarakta bu dosyamızı kaydedelim.

![mk54_7.gif](/assets/images/2004/mk54_7.gif)

Şekil 7. Connection Özelliklerinin belirlenmesi.

Şimdi uygulamızı buna göre değiştirelim.

```csharp
OleDbConnection con=new OleDbConnection();
con.ConnectionString="File Name=C:\\baglanti.udl";

try
{
     con.Open();
     Console.WriteLine("Bağlantı açıldı...");
     con.Close();
     Console.WriteLine("Bağlantı kapatıldı...");
}
catch(Exception hata)
{
     Console.WriteLine(hata.Message.ToString());
}
```

![mk54_8.gif](/assets/images/2004/mk54_8.gif)

Şekil 8. Udl dosyası üzerinden bağlantı açmak ve kapatmak.

Tabiki burada Visual Studio.Net'in bağlantı sağlamak için bize sunduğu görsel nimetlerden sözetmedende geçemeyiz. Visual Studio.NET ortamında, OleDbConnection oluşturmak için kullanabileceğimiz Server Explorer sekmesi yer almaktadır.

![mk54_9.gif](/assets/images/2004/mk54_9.gif)

Şekil 9. Server Explorer

Burada servers sekmesinde Sql sunucularına erişebiliriz. Buradan tablolara, görünümlere, saklı yordamlara hatta tabloa alanlarına ulaşabiliriz. Server Explorer'ın sağladığı pek çok işlevsellik vardır. Örneğin bu pencereden, bir sql tablosu yaratabilirsiniz veya bir saklı yordam oluşturabilirsiniz. Hatta bir tabloyu formunuza sürüklüyerek, gerekli olan tüm ADO.NET nesnelerinin otomatik olarak oluşturulmasınıda sağlayabilirsiniz. Bu makalemizde OleDbConnection nesnesini incelediğimiz için bu tip bir bağlantıyı Server Explorer yardımıyla nasıl gerçekleştireceğimizi inceleyeceğiz. Bunun için

![mk54_10.gif](/assets/images/2004/mk54_10.gif)

(Connect To Database) aracını kullanacağız. Bu butona bastığımızda karşımıza tanıdık Data Link Properties penceresi gelecektir. Süratli bir şekilde burada gerekli bağlantı ayarlarını yaptıktan sonra, Server Explorer aşağıdaki görünümü alıcaktır. Burada örnek olarak bir Access veritabanına bağlantı sağlanmıştır.

![mk54_11.gif](/assets/images/2004/mk54_11.gif)

Şekil 10. Bağlantı oluşturuldu.

Dikkat ederseniz bağlantımız üzerinden, veri kaynağındaki tablolara ve alanlarına ulaşabiliyoruz. Şimdi bu bağlantıyı, bir windows uygulamasında veya bir asp.net uygulamasında forma sürüklediğimizde bir OleDbConnection nesnesinin otomatik olarak oluşturulduğunu göreceksiniz. İşte bu kadar basit. Artık bu connection nesnesini kullanabilirsiniz. Diğer yandan Server Explorer'da oluşturulan bu bağlantıyı başka uygulamalarda da hazır olarak görebilirsiniz. Data Connection sekmesi uygulamalarda kullanacağımız veri bağlantılarını hazır olarak bulundurmak için ideal bir yerdir.

![mk54_12.gif](/assets/images/2004/mk54_12.gif)

Şekil 11. OleDbConnection1 nesnesinin Server Explorer yarıdımıyla oluşturulması.

Gelelim OleDbConnection nesnesinin diğer kullanışlı üyelerine. Buraya kadar bir bağlantı hattını oluşturmak için ConnectionString özelliğinin nasıl kullanıldığını inceledik. Bununla birlikte var olan bir bağlantı hattını açmak için Open metodunu, bu bağlantı hattını kapatmak içinde Close metodunun kullanıldığını gördük. OleDbConnection nesnesinin diğer bir özelliğide ConnectionTimeout değeridir. ConnectionTimeout özelliği, bir bağlantının sağlanması için gerekli süreyi belirtir. Bu süre boyunca bağlantı sağlanamaması bir istisnanın fırlatılmasına neden olucaktır. Bu özellik yanlız-okunabilir (read-only) bir özellik olduğundan, değerini doğrudan değiştiremeyiz. Bunun için, bu özelliği ConnectionString içerisinde belirlememiz gerekir. Örneğin aşağıdaki kodlarda, Sql sunucusuna bağlanabilmek için gerekli süre 10 saniye olarak belirlenmiştir. Şimdi ben Sql Server servisimizi durduracağım ve uygulamayı çalıştıracağım. Bakalım 10 saniye sonunda ne olucak.

```csharp
OleDbConnection conFriends=new OleDbConnection();
conFriends.ConnectionString="Provider=SQLOLEDB;Data Source=localhost;Database=Friends;User Id=sa;Password=CucP??80.;Connect Timeout=10";
try
{
     conFriends.Open();
     Console.WriteLine("Baglanti açildi...");
     conFriends.Close();
     Console.WriteLine("Baglanti kapatildi...");
}
catch(Exception hata)
{
     Console.WriteLine(hata.Message.ToString());
}
```

Bu durumda aşağıdaki hata mesajını alırız.

![mk54_13.gif](/assets/images/2004/mk54_13.gif)

Şekil 12. Sql sunucusunun olmadığını gösteren istisna.

OleDbConnection sınıfının Open ve Close metodları dışındada faydalı metodları vardır. Örneğin ChangeDatabase metodu. Bu metod ile açık olan bir bağlantı üzerinden, veri kaynağındaki seçili veritabanını değiştirmemiz sağlanır. Yani hattın ucu başka bir veritabanına yönlendirilebilir. Bu tabiki Oracle ve Sql gibi veritabanı sistemlerinde özellikle işe yarar. Örneğin, Friends veritabanına bağlıyken, açık olan bağlantımız üzerinden hattımızı, pubs veritabanına yönlendirelim.

```csharp
OleDbConnection conFr=new OleDbConnection(); /* OleDbConnection nesnemiz oluşturuluyor. */
conFr.ConnectionString="Provider=SQLOLEDB;Data Source=localhost;Database=Friends;Integrate/d Security=SSPI"; /* Bağlantı hattımız için gerekli bilgiler giriliyor. Sql sunucumuzda yer alan Friends isimli veritabanına bağlandık. */
conFr.Open(); /* Bağlantımız açılıyor. */
Console.WriteLine("Veritabanı "+conFr.Database.ToString()); /* Şuandaki bağlantının hangi veritabanına yapıldığını OleDbConnection sınıfının Database özelliği ile öğreniyoruz. */
conFr.ChangeDatabase("pubs"); /* ChangeDatabase metodu ile bağlantı hattımızı yönlendirmek istediğimiz veritabanının adını giriyoruz. */
Console.WriteLine("Şimdiki veritabanı "+conFr.Database.ToString()); /* Bağlantı hattının şu an yönlendirilmiş olduğu veritabanının adını Database özelliği ile elde ediyoruz.*/
conFr.Close(); /* Bağlantımızı kapatıyoruz. */
```

![mk54_14.gif](/assets/images/2004/mk54_14.gif)

Şekil 13. ChangeDatabase metodunun çalışmasının sonucu.

Diğer yararlı bir metod ise GetOleDbSchemaTable metodudur ki bunu bir önceki makalemizde incelemiştik. Bunun dışında bir OleDbCommand nesnesini oluşturmaya yarayan CreateCommand metodu, bir Transaction'ın başlatılması için kullanılan BeginTransaction metodu, OleDbConnection'a ait kaynakları serbest bırakan Dispose metodu'da faydalı diğer metodlar olarak sayılabilir. Bu metodlardan ilerliyen makalelerde yeri geldikçe bahsedeceğiz. OleDbConnection nesnesinin sadece 3 adet olayı vardır. Bunlar, StateChange, Disposed ve InfoMessage olaylarıdır. Bunlardan en çok, StateChange olayını kullanırız. Bu olay, OleDbConnection nesnesinin bağlantı durumunda bir değişiklik olduğunda oluşur. Bu olayın prototipi aşağıdaki şekildedir.

```text
public event StateChangeEventHandler StateChange;
```

Bu olay StateChangeEventArgs tipinden bir argüman almaktadır. Bu argüman iki özelliğe sahiptir. Bunlar CurrentState ve OriginalState özellikleridir. CurrentState bağlantının o anki drumunu belirtir. OriginalState ise son değişiklikten önceki halini gösterir. Her iki özellikde ConnectionState numaralandırıcısı türünden değerlere işaret ederler. Bu değerler şunlardır.

ConnectionState Değeri
Açıklaması

ConnectionState.Open
Bağlantı açık ise bu değer geçerlidir.

ConnectionState.Closed
Bağlantı kapandığında bu değer geçerlidir.

ConnectionState.Connecting
Bağlantı hattı iletişime açılırken bu değer geçerlidir.

ConnectionState.Broken
Bağlantı hattı açıkken herhangibir nedenle bir kopma meydana gelmesi ve hattın işlevselliğini kaybetmesi durumunda oluşur.

ConnectionState.Executing
Bağlantı nesnemiz bir komut çalıştırırken oluşur.

ConnectionState.Fetching
Bağlantı hattı üzerinden veriler alınırken bur değer geçerlidir.

Tablo 1. ConnectionState numaralandırıcısının değerleri.

ConnectionState numaralandırıcısı aynı zamanda, State özelliği içinde kullanılabilir. State özelliği, OleDbConnection nesnesinin o anki durumunu, ConnectionState numaralandırıcısı tipinde saklar. State özelliğini uygulamalarımızda, var olan bağlantının durumun kontrol ederek hareket etmek için kullanabiliriz. Örneğin bir bağlantı nesnesini uygulamamızın bir yerinde tekrardan açmak istediğimizi varsayalım. Bu bağlantı nesnesinin durumu zaten Open ise yani açık ise, tekrardan açma işlemi uygulamamız gerekmez. Dilerseniz makalemizin sonunda StateChange olayına ilişkin bir örnek geliştirelim.

```csharp
OleDbConnection con; /* OleDbConnection nesnemiz tanımlanıyor.*/

private void Form1_Load(object sender, System.EventArgs e)
{
     lstDurum.Items.Clear();
     con=new OleDbConnection("Provider=SQLOLEDB;Data source=localhost;initial catalog=Friends;Integrated Security=sspi"); /* Bağlantı hattımız oluşturuluyor. */
     con.StateChange+=new StateChangeEventHandler(con_DurumDegisti); /* OleDbConnection nesnemiz için StateChange olayımız ekleniyor. Olay meydana geldiğinde con_DurumDegisti isimli metod çalıştırılıcak.*/
}

private void btnAc_Click(object sender, System.EventArgs e)
{
     if(con.State==ConnectionState.Closed) /* Kullanıcı açık olan bir bağlantı üzerinden tekrar bu butona basarak bir bağlantı açmak isterse bunun önüne geçmek için ilgili OleDbConnection nesnesinin durumuna bakıyoruz. Eğer con nesnesi kapalı ise, açılabilmesini sağlıyoruz.*/
     {
          con.Open(); /* Bağlantımız açılıyor. İşte bu anda StateChange olayı çalıştırılır.*/
     }
}
private void btnKapat_Click(object sender, System.EventArgs e)
{
     if(con.State==ConnectionState.Open) /* Eğer açık bir bağlantı varsa kapatma işlemini uyguluyoruz.*/
     {
          con.Close(); /* Bağlantı kapanıyor. StateChange olayı bir kez daha çalışır. */
     }
}
private void con_DurumDegisti(object sender,StateChangeEventArgs e)
{
     lstDurum.Items.Add("Bağlantı durumu "+e.OriginalState.ToString()+" idi."); /* Bağlantımızın hangi halde olduğunu alıyoruz.*/

     lstDurum.Items.Add("Artık bağlantı durumu "+e.CurrentState.ToString()); /* Ve bağlantımızın yeni halini alıyoruz.*/
}
```

![mk54_15.gif](/assets/images/2004/mk54_15.gif)

Şekil 14. StateChange olayı.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.