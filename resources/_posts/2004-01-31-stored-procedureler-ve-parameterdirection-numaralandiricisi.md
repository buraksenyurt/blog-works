---
layout: post
title: "Stored Procedureler ve ParameterDirection Numaralandırıcısı"
date: 2004-01-31 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado.net
  - ParameterDirection
  - sql
  - enums
  - stored-procedures
  - database
---
Bugünkü makalemizde, Sql sunucularında yazdığımız Stored Procedure'lere ilişkin parametreleri incelemeye çalışacağız. Stored Procedure'ler ile ilgili daha önceki makalelerimizde, uygulamamızdan bu procedure'lere nasıl parametre aktarılacağını incelemiştik. Parametre aktarımında yaptığımız işlem, SqlCommand nesnesimizin parametre koleksiyonuna, Stored Procedure içinde tanımladığımız parametrenin eklenmesiydi. Bunun için, SqlCommand sınıfının Parameters koleksiyonuna Add metodunu kullanarak SqlParameter sınıfı türünden bir nesne ekliyorduk. Bu parametrelere program içerisinden ilgili değerleri aktararak, bu değerlerin Stored Procedure içinede aktarılmasına imkan sağlıyorduk.

Bugünkü makalemizde ise, bir Stored Procedure'den programımıza nasıl değer (ler) döndürebileceğimizi inceleyeceğiz. Dikkat ederseniz, bir Stored Procedure'e program içinden parametre aktarabileceğimiz gibi, Stored Procedure'dende programımıza değerler aktarbildiğimizden bahsediyoruz. Dolayısıyla parametrelerin bir takım farklı davranışlar sergiliyebilmesi söz konusu. SqlParameters sınıfı, parametrelerin davranışlarını yada başka bir deyişle hangi yöne doğru hareket edeceklerini belirten bir özellik içermektedir. Bu özellik Direction özelliğidir ve C# prototipi aşağıdaki gibidir.

public virtual ParameterDirection Direction {get; set;}

Direction özelliği, ParameterDirection numaralandırıcısı tipinden değerler almaktadır. Bu değerlerin açıklaması aşağıdaki tabloda yer almaktadır.

Direction Değeri
Açıklama

Input

Bir SqlParametre'sinin varsayılan değeri budur. Program içinden Stored Procedure'e değerler gönderileceği zaman, SqlParameter nesnesinin Direction özelliği Input olarak kullanılır. Yani parametre değerinin yönü Stored Procedure'e doğrudur.

Output

Stored Procedure'den programımıza doğru değer aktarımı söz konusu ise SqlParameter nesnesinin Direction değeri Output yapılır. Bu, parametre yönünün, Stored Procedure'den programımıza doğru olduğunu göstermektedir.

ReturnValue

Bazen bir Stored Procedure'ün çalışması sonucunu değerlendirmek iseyebiliriz. Bu durumda özellikle Stored Procedure'den Return anahtar sözcüğü ile döndürülen değerler için kullanılan parametrelerin Direction değeri ReturnValue olarak belirlenir. Bu tip bir parametreyi, bir fonksiyonun geri döndürdüğü değeri işaret eden bir parametre olarak düşünebiliriz.

InputOutput

Bu durumda parametremiz hem Input hemde Output yetenklerine sahip olucaktır.

Tablo 1.ParameterDirection Numaralandırıcısının Değerleri.

Bugünkü makalemizde ağırlıklı olarak ReturnValue ve Output durumlarını incelemeye çalışacağız. Dilerseniz işe ReturnValue ile başlayalım. Bu örnekler için, Makaleler ile ilgili bilgilere sahip olan bir tablo kullanacağız. Şimdi sql sunucumuzda, bu tablo için aşağıdaki Stored Procedure nesnesini oluşturalım.

![mk49_1.gif](/assets/images/2004/mk49_1.gif)

Şekil 1. MakaleMevcutmu Stored Procedure'ümüz.

Bu Stored Procedure, programdan @MakaleID isimli bir parametre alıyor. Bu parametreyi, SELECT sorgusuna geçiyor ve Makale tablosunda ID alanı, @MakaleID parametresinin değerine eşit olan satırı seçiyor. Burada kullanılan @MakaleID parametresinin değeri program içinden belirlenecektir. Dolayısıyla bu parametremizin yönü, programımızdan Stored Procedure'ümüze doğrudur. Bu nedenle, programımızda bu parametre tanımlandığında Direction değeri Input olmalıdır. Ancak bunu program içinde belirtmeyecek yani SqlParameter nesnesinin Direction özelliğine açıkça ParameterDirection.Input değerini atamayacağız. Çünkü bir SqlParametresinin Direction özelliğinin varsayılan değeri budur.

Gelelim RETURN kısmına. Burada @@RowCount isimli sql anahtar kelimesi kullanıyoruz. Bu anahtar kelime Stored Procedure çalıştırıldıktan sonra, Select sorgusu sonucu dönen satır sayısını vermektedir. If koşulumuz ile, procedure içinden, bu select sorgusu sonucunun değerine bakıyoruz. Eğer PrimaryKey olan ID alanımızda, @MakaleID parametresine atadığımız değer mevcutsa, @@RowCount, 1 değerini döndürecektir. Daha sonra, RETURN anahtar kelimesini kullanarak, Stored Procedure'den programa değer döndürüyoruz. İşte bu değerler, programımızdaki SqlParameter nesnesi için, Direction özelliğinin ParameterDirection.ReturnValue olmasını gerektirir. Örneğimizi tamamladığımızda bu durumu daha iyi anlayacağınıza inanıyorum.

Şimdi aşağıdaki Form tasarımımızı yapalım ve kodlarımızı yazalım. Program basit bir şekilde, bu Stored Procedure'e kullanıcının girdiği Makale numarasını gönderecek ve sonuçta bu makalenin var olup olmadığını bize belirtecek. Bu arada şunuda belirtmekte fayda var. Bu tip bir işlemi elbette, SqlCommand nesnesine, bir Select sorgusu girerek de gerçekleştirebilirdik. Ancak Stored Procedure'lerin en önemli özelliklerinin, derlenmiş sql nesneleri oldukları için, sağladıkları performans ve hız artışı kazanımları olduğunuda görmezden gelemeyiz. Bununla birlikte güvenlik açısındanda daha verimlidirler. Özelliklede web uygulamaları için. Şimdi Formumuzu tasarlayalım. Bunun için basit bir windows uygulaması geliştireceğiz.

![mk49_2.gif](/assets/images/2004/mk49_2.gif)

Şekil 2. Form Tasarımımız.

```csharp
private void button1_Click(object sender, System.EventArgs e)
{
     conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi"); /* Sql sunucumuza olan bağlantımızı gerçekleştiriyoruz. */
 
     SqlCommand cmd=new SqlCommand("MakaleMevcutmu",conFriends); /* SqlCommand nesnemizi oluşturuyoruz. SqlCommand nesnemize, Stored Procedure'ümüzün adını parametre olarak veriyoruz. */

     cmd.CommandType=CommandType.StoredProcedure; /* SqlCommand nesnemiz bir Stored Procedure çalıştıracağı için CommandType özelliği CommandType.StoredProcedure olarak belirlenir.*/

     /* Burada, Stored Procedure'ümüzüden Return ile dönen değeri işaret edicek SqlParameter nesnemizi ,SqlCommand nesnemizin Parameters koleksiyonuna Add metodu ile ekliyoruz. Return anahtar sözcüğü ile geri dönen değeri işaret edicek paramterenin adı herhangibir isim olabilir. Ancak her sql parametresinde olduğu gibi başında @ işaretini kullanmayı unutmamalıyız.*/
     cmd.Parameters.Add("@DonenDeger",SqlDbType.Int);
     cmd.Parameters["@DonenDeger"].Direction=ParameterDirection.ReturnValue; /* Parametrenin, Stored Procedure'den, programa doğru olduğunu ve Return anahtar sözcüğü ile geriye dönen bir değeri işaret ettiğini belirtmek için, Direction özelliğine, ParameterDirection.ReturnValue değerini veriyoruz.*/

     /* Burada programımızda kullanıcının girdiği Makale numarasını, Stored Procedure'ümüze doğru gönderecek SqlParameter nesnemizi tanımlanıyoruz. Bu parametre Input tipindedir. Bu nedenle, adının, Stored Procedure'ümüzdeki ile aynı olmasına dikkat etmeliyiz. */
     cmd.Parameters.Add("@MakaleID",SqlDbType.Int);
     cmd.Parameters["@MakaleID"].Value=txtMakaleNo.Text; /* Parametremizin değeri veriliyor.*/

     /* Güvenli bloğumuzda, öncelikle sql sunucumuza olan bağlantımızı SqlConnection yardımıyla açıyoruz ve ardından SqlCommand nesnemizin referans ettiği Stored Procedure nesnemizi çalıştırıyoruz.*/
     try
     {
          conFriends.Open();
          cmd.ExecuteNonQuery();
          int Sonuc;
          /* Stored Procedure'ümüzden Return anahtar sözcüğü ile dönen değeri @DonenDeger SqlParameter nesnesi ile alıyor ve sonuc ismindeki integer tipteki değişkenimize atıyoruz. SqlCommand nesnesinin, Parameters koleksiyonunda yer alan SqlParameter nesnelerinin Value özelliği geriye Object tipinden değer döndürdüğü için, bu değeri integer tipine dönüştürme işlemide uyguladığımıza dikkat edelim. */

         Sonuc=Convert.ToInt32(cmd.Parameters["@DonenDeger"].Value);

          /* Burada dönen değeri değerlendirerek kullanıcının girdiği ID'ye sahip bir Makale olup olmadığını belirliyoruz.*/
               if(Sonuc==1)
               {
                    MessageBox.Show("MAKALE BULUNDU...");
               }
               else if(Sonuc==0)
               {
                    MessageBox.Show("MAKALE BULUNAMADI...");
               }
     }
     catch(Exception hata)
     {
          MessageBox.Show("Hata:"+hata.Message.ToString());
     }
     finally
     {
          conFriends.Close(); /* Bağlantımızı kapatıyoruz. */
     }
}
```

Şimdi progamımızı çalıştıralım ve bir ID değeri girelim. Bu noktada kullanıcının girdiği ID değeri, @MakaleID isimli SqlParameter nesnemiz yardımıyla, Stored Procedure'ümüze aktarılacak ve Stored Procedure'ün çalışması sonucu geri dönen değerde @DonenDeger isimli SqlParameter nesnesi yardımıyla uygulamamızda değerlendirilecek.

![mk49_3.gif](/assets/images/2004/mk49_3.gif)

Şekil 3. Programın Çalışması Sonucu. Makale bulunduğunda.

![mk49_4.gif](/assets/images/2004/mk49_4.gif)

Şekil 4. Makale bulunamadığında.

Elbette bu örnek bize pek bir şey ifade etmiyor. Nitekim makalenin var olup olmadığının farkına vardık o kadar. Ama Makale tablosundaki belli alanlarıda görmek istediğimizi varsaylım. İşte bu, Output tipindeki SqlParameter nesnelerini kullanmak için güzel bir fırsat. Şimdi bu Stored Procedure nesnemizin sql kodunu biraz değiştireceğiz.

![mk49_5.gif](/assets/images/2004/mk49_5.gif)

Şekil 5. Output Tipi Sql Parametreleri.

Burada Select sorgusundaki @MakaleKonusu=Konu ifadesine dikkatinizi çekmek isterim. Eğer @MakaleID parametresinin değerinde bir Makale satırı var ise, bu satırın Konu alanının değerini, @MakaleKonusu isimli parametreye aktarıyoruz. İşte bu parametre, programımıza doğru dönen Output tipinde bir parametredir. Diğer yandan Output parametrelerini kullanırken, sql yazımı içindede bu parametre değişkeninin OUTPUT anahtar sözcüğü ile belirtilmesi gerekmektedir. Yeni duruma göre program kodlarımız aşağıdaki gibi olmalıdır.

```csharp
private void button1_Click(object sender, System.EventArgs e)
{
     conFriends=new SqlConnection("data source=localhost;initial catalog=Friends;integrated security=sspi");
     SqlCommand cmd=new SqlCommand("MakaleMevcutmu",conFriends);
     cmd.CommandType=CommandType.StoredProcedure;
     cmd.Parameters.Add("@DonenDeger",SqlDbType.Int);
     cmd.Parameters["@DonenDeger"].Direction=ParameterDirection.ReturnValue;
  cmd.Parameters.Add("@MakaleKonusu",SqlDbType.NVarChar,255);
     cmd.Parameters["@MakaleKonusu"].Direction=ParameterDirection.Output;
     cmd.Parameters.Add("@MakaleID",SqlDbType.Int);
     cmd.Parameters["@MakaleID"].Value=txtMakaleNo.Text;
     try
     {
          conFriends.Open();
          cmd.ExecuteNonQuery();
          int Sonuc;
          string MakaleAdi;
         Sonuc=Convert.ToInt32(cmd.Parameters["@DonenDeger"].Value);
          MakaleAdi=cmd.Parameters["@MakaleKonusu"].Value.ToString();

          if(Sonuc==1)
          {
               MessageBox.Show("MAKALE BULUNDU...");
               lblMakaleKonusu.Text=MakaleAdi;
          }
          else if(Sonuc==0)
          {
               MessageBox.Show("MAKALE BULUNAMADI...");
          }
     }
     catch(Exception hata)
     {
          MessageBox.Show("Hata:"+hata.Message.ToString());
     }
     finally
     {
          conFriends.Close();
     }
}
```

Şimdi uygulamamızı çalıştıralım. Aşağıdaki sonucu elde ederiz.

![mk49_6.gif](/assets/images/2004/mk49_6.gif)

Şekil 6. Makalemizin Konu isimli alanının değeri döndürüldü.

Değerli okurlarım. Geldik bir makalemizin daha sonuna. Umarım yararlı bir makale olmuştur. Hepinize mutlu günler dilerim.