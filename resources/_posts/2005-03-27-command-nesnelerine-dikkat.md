---
layout: post
title: "Command Nesnelerine Dikkat!"
date: 2005-03-27 09:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - sqlcommand
---
Bu makalemizde, Command nesnelerini kullanırken performans arttırıcı, kod okunurluğunu kolaylaştırıcı, güvenlik riskini azaltıcı etkenler üzerinde duracağız ve bu kazanımlar için gerekli teknikleri göreceğiz. Örneklerimizi SqlCommand sınıfına ait nesneler üzerinden geliştireceğiz. Bildiğiniz gibi Command nesneleri yardımıyla veritabanına doğru yürütmek istediğimiz sorguları çalıştırmaktayız. Bu sorgular basit Select, Insert, Update, Delete sorguları olabileceği gibi saklı yordamlar (Stored Procedures) veya tablolarda olabilir.

Command nesneleri ayrıca diğer Ado.Net nesnelerinin işletilmelerinde de etkin rol oynamaktadır. Örneğin bağlantısız katman (disconnected layer) nesnelerinin doldurulması veya güncellenmesi için kullanılan DataAdapter nesneleri veya bağlantılı katman (connected layer) üzerinde çalışan DataReader nesneleri gibi. Dolayısıyla Command nesnelerine bağımlı olarak çalışan programların performans, güvenlik ve kod okunurluğu yönünden uygulaması tavsiye edilen bazı teknikler vardır. Command nesnelerinin hazırlanışı ve kullanılması sırasında dikkat etmemiz gereken noktalar aşağıdaki dört madde ile özetlenmiştir.

SqlCommand Nesneleri İçin Pozitif Yaklaşımlar

Parametrik sorguların kullanımı.

Sorguların yeniden kullanım için hazırlanması (Prepare Tekniği)

En etkin constructor ile nesne örneğinin oluşturulması.

Tek değerlik dönüşler için ExecuteScalar metodunun tercih edilmesi.

Şimdi bu maddelerimizi tek tek incelemeye başlayalım.

Parametrik Sorguların Kullanımı.

Parametrik sorgular diğer türlerine göre daha hızlı çalışır. Ayrıca Sql Injection'a karşı daha yüksek güvenlik sağlar. Son olarak, parametrik sorgularda örneğin string değerler için tek tırnak kullanma zorunluluğunda kalmazsınız ki bu kodunuzun okunulabilirliğini arttırır. Örneğin aşağıdaki kod parçasını inceleyelim. Bu örneğimizde tabloya veri girişi için INSERT sorgusu kullanılıyor. Sorgumuza ait Sql cümleciğine dikkat edecek olursanız TextBox kontrollerinden değerler almakta. İfade okunurluk açısından oldukça zorlayıcı. Ayrıca tek tırnak kullanılması gerektiğinden yazımında büyük dikkat gerektiriyor.

```csharp
SqlConnection con=new SqlConnection("data source=BURKI;database=Work;integrated security=SSPI");
string insertText="INSERT INTO Maaslar (ADSOYAD,DOGUMTARIHI,MAAS) VALUES ('"+txtADSOYAD.Text.ToString()+"','"+txtDOGUMTARIHI.Text.ToString()+"',"+txtMAAS.Text.ToString()+")";
SqlCommand cmd=new SqlCommand(insertText,con);
con.Open();
cmd.ExecuteNonQuery();
con.Close();
```

Oysaki bu tip bir kullanım yerine sorguya giren dış değerleri parametrik olarak tanımlamak çok daha avantajlıdır. Sürat, kolay okunurluk, tek tırnak'dan bağımsız olmak. Aynı kod parçasını şimdi aşağıdaki gibi değiştirelim. Bu sefer sorgumuza alacağımız değerleri SqlCommand nesnesine birer parametre olarak ekledik. ADSOYAD alanımız nvarchar, DOGUMTARIHI alanımız DateTime, MAAS alanımız ise Money tipindedir. Bu nedenle parametrelerde uygun SqlDbType değerlerini seçtik. Daha sonra parametrelerimiz için gerekli değerlerimizi Form üzerindeki kontrollerimizden alıyor. Dikkat ederseniz textBox kontrollerinden veri alırken herhangi bir tür dönüşümü işlemi uygulamadık.

```csharp
/* Connection oluşturulur. */
SqlConnection con=new SqlConnection("data source=BURKI;database=Work;integrated security=SSPI");
/* Sorgu cümleciği oluşturulur.*/
string insertText="INSERT INTO Maaslar (ADSOYAD,DOGUMTARIHI,MAAS) VALUES (@ADSOYAD,@DOGUMTARIHI,@MAAS)";
/* Command nesnesi oluşturulur */
SqlCommand cmd=new SqlCommand(insertText,con);
/* Komut için gerekli parametreler tanımlanır. */
cmd.Parameters.Add("@ADSOYAD",SqlDbType.NVarChar,50);
cmd.Parameters.Add("@DOGUMTARIHI",SqlDbType.DateTime);
cmd.Parameters.Add("@MAAS",SqlDbType.Money);
/* Parametre değerleri verilir. */
cmd.Parameters["@ADSOYAD"].Value=txtADSOYAD.Text;
cmd.Parameters["@DOGUMTARIHI"].Value=txtDOGUMTARIHI.Text;
cmd.Parameters["@MAAS"].Value=txtMAAS.Text;
/* Bağlantı açılır komut çalıştırılır ve bağlantı kapatılır. */
con.Open();
cmd.ExecuteNonQuery();
con.Close();
```

Sorguların Yeniden Kullanım için Hazırlanması (Prepare Tekniği)

Stored Procedure'lerin hızlı olmalarının en büyük nedeni sql sorgularına ait planlarının ara bellekte tutulmasıdır. Aynı işlevselliği uygulamalarımızda sık kullanılan sorgu cümleleri içinde gerçekleyebiliriz. Bunun için SqlCommand nesnesinin Prepare metodu kullanılır. Bu metod yardımıyla ilgili sql sorgusuna ait planın Sql sunucusu için ara bellekte tutulması sağlanmış olur. Böylece sorgunun ilk çalıştırılışından sonraki yürütmelerin daha hızlı olması sağlanmış olur. Aşağıdaki kod parçasını ele alalım. Bu sefer arka arkaya 3 satır girişi işlemini gerçekleştiriyoruz.

```csharp
SqlConnection con=new SqlConnection("data source=BURKI;database=Work;integrated security=SSPI");
string insertText="INSERT INTO Maaslar (ADSOYAD,DOGUMTARIHI,MAAS) VALUES (@ADSOYAD,@DOGUMTARIHI,@MAAS)"; con.Open();
SqlCommand cmd=new SqlCommand(insertText,con);
cmd.Parameters.Add("@ADSOYAD",SqlDbType.NVarChar,50);
cmd.Parameters.Add("@DOGUMTARIHI",SqlDbType.DateTime);
cmd.Parameters.Add("@MAAS",SqlDbType.Money);
// İlk veri girişi
cmd.Parameters["@ADSOYAD"].Value="Burak";
cmd.Parameters["@DOGUMTARIHI"].Value="12.04.1976";
cmd.Parameters["@MAAS"].Value=1000;
cmd.ExecuteNonQuery();

// İkinci veri girişi
cmd.Parameters["@ADSOYAD"].Value="Bili";
cmd.Parameters["@DOGUMTARIHI"].Value="10.04.1965";
cmd.ExecuteNonQuery();

// Üçüncü veri girişi
cmd.Parameters["@ADSOYAD"].Value="Ali";
cmd.Parameters["@DOGUMTARIHI"].Value="09.04.1980";
cmd.ExecuteNonQuery();
con.Close();
```

Bu tarz bir kullanım yerine, aşağıdaki kullanım özellikle ağ ortamında işletilecek olan sorgulamalarda daha yüksek performans sağlayacaktır. Tek yapmamız gereken SqlCommand nesnesini ilk kez Execute edilmeden önce Prepare metodu ile Sql Sunucusu için ara belleğe aldırmaktır. Yani;

```csharp
SqlConnection con=new SqlConnection("data source=BURKI;database=Work;integrated security=SSPI");
string insertText="INSERT INTO Maaslar (ADSOYAD,DOGUMTARIHI,MAAS) VALUES (@ADSOYAD,@DOGUMTARIHI,@MAAS)"; con.Open();
SqlCommand cmd=new SqlCommand(insertText,con);
cmd.Parameters.Add("@ADSOYAD",SqlDbType.NVarChar,50);
cmd.Parameters.Add("@DOGUMTARIHI",SqlDbType.DateTime);
cmd.Parameters.Add("@MAAS",SqlDbType.Money);
// İlk veri girişi
cmd.Parameters["@ADSOYAD"].Value="Burak";
cmd.Parameters["@DOGUMTARIHI"].Value="12.04.1976";
cmd.Parameters["@MAAS"].Value=1000;
cmd.Prepare();
cmd.ExecuteNonQuery();

// İkinci veri girişi
cmd.Parameters["@ADSOYAD"].Value="Bili";
cmd.Parameters["@DOGUMTARIHI"].Value="10.04.1965";
cmd.ExecuteNonQuery();

// Üçüncü veri girişi
cmd.Parameters["@ADSOYAD"].Value="Ali";
cmd.Parameters["@DOGUMTARIHI"].Value="09.04.1980";
cmd.ExecuteNonQuery();
con.Close();
```

En Etkin Constructor ile Nesne Örneğinin Oluşturulması.

Bir SqlCommand nesnesinin oluşturulması sırasında kullanılacak Constructor metodun seçimi özellikle kod okunurluğu açısından önemlidir. Örneğin aşağıdaki kod kullanımını ele alalım.

```csharp
// ConnectionString tanımlanır.
string conStr="data source=BURKI;database=Work;integrated security=SSPI";
// Select sorgu cümlesi tanımlanır.
string selectText="SELECT * FROM Ogrenciler";
// SqlConnection nesnesi oluşturulur.
SqlConnection con=new SqlConnection();
// SqlConnection nesnesi için Connection String atanır.
con.ConnectionString=conStr;
// Connection açılır.
con.Open();
// Yeni bir SqlTransaction nesnesi başlatılır.
SqlTransaction trans=con.BeginTransaction();
// SqlCommand nesnesi tanımlanır.
SqlCommand cmd=new SqlCommand();
// SqlCommand nesnesinin kullanacağı SqlConnection belirlenir.
cmd.Connection=con;
// SqlCommand nesnesinin kullanacağı SqlTransaction belirlenir.
cmd.Transaction=trans;
// SqlCommand nesnesinin yürüteceği sorgu cümlesi belirlenir.
cmd.CommandText=selectText;
```

Bu kod aşağıdaki gibi daha etkin bir biçimde yazılabilir.

```csharp
// ConnectionString tanımlanır.
string conStr="data source=BURKI;database=Work;integrated security=SSPI";
// Select sorgu cümlesi tanımlanır.
string selectText="SELECT * FROM Ogrenciler";
// SqlConnection nesnesi oluşturulur ve açılır.
SqlConnection con=new SqlConnection(conStr);
con.Open();
// Yeni bir SqlTransaction nesnesi başlatılır.
SqlTransaction trans=con.BeginTransaction();
// SqlCommand nesnesi tanımlanır.
SqlCommand cmd=new SqlCommand(selectText,con,trans);
```

Tek Değerlik Dönüşler için ExecuteScalar Metodunun Tercih Edilmesi.

Bazen sorgularımızda Aggregate fonksiyonlarını kullanırız. Örneğin bir tablodaki satır sayısının öğrenilmesi için Count fonksiyonunun kullanılması veya belli bir alandaki değerlerin ortalamasının hesaplanması için AVG (ortalama) fonksiyonu vb. Aggregate fonksiyonlarının kullanıldığı durumlarda iki alternatifimiz vardır. Bu alternatiflerden birisi aşağıdaki gibi SqlDataReader nesnesinin ilgili SqlCommand nesnesi ile birlikte kullanılmasıdır.

```csharp
// ConnectionString tanımlanır.
string conStr="data source=BURKI;database=Work;integrated security=SSPI";
// Select sorgu cümlesi tanımlanır.
string selectText="SELECT COUNT(*) FROM Ogrenciler";
// SqlConnection nesnesi oluşturulur ve açılır.
SqlConnection con=new SqlConnection(conStr);
con.Open();
// SqlCommand nesnesi tanımlanır.
SqlCommand cmd=new SqlCommand(selectText,con);
// SqlDataReader nesnesi satır sayısını almak amacıyla oluşturulur.
SqlDataReader dr=cmd.ExecuteReader(CommandBehavior.SingleResult);
// Elde edilen sonuç okunur.
dr.Read();
// Hücre değeri ekrana yazdırılır.
Console.WriteLine("Öğrenci sayısı "+dr[0].ToString());
// SqlDataReader ve SqlConnection kaynakları kapatılır.
dr.Close();
con.Close();
```

Bu teknikte aggregate fonksiyonun çalıştırılmasından dönen değeri elde edebilmek için SqlDataReader nesnesi kullanılmıştır. Ancak SqlCommand nesnesinin bu iş için tasarlanmış olan ExecuteScalar metodu yukarıdaki tekniğe göre daha yüksek bir performans sağlamaktadır. Çünkü çalıştırılması sırasında bir SqlDataReader nesnesine ihtiyaç duymaz. Bu da SqlDataReader nesnesinin kullanmak için harcadığı sistem kaynaklarının var olmaması anlamına gelmektedir. Dolayısıyla yukarıdaki örnekteki kodları aşağıdaki gibi kullanmak daha etkilidir.

```csharp
// ConnectionString tanımlanır.
string conStr="data source=BURKI;database=Work;integrated security=SSPI";
// Select sorgu cümlesi tanımlanır.
string selectText="SELECT COUNT(*) FROM Ogrenciler";
// SqlConnection nesnesi oluşturulur ve açılır.
SqlConnection con=new SqlConnection(conStr);
con.Open();
// SqlCommand nesnesi tanımlanır ve ExecuteScalar ile sonuç anında elde edilir.
SqlCommand cmd=new SqlCommand(selectText,con);
Console.WriteLine("Satır sayısı "+cmd.ExecuteScalar().ToString());
```

Bu makalemizde, Command nesnelerini kullanırken bize performans, hız, güvenlik kod okunurluğu açısından avantajlar sağlayacak teknikleri incelemeye çalıştık. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.