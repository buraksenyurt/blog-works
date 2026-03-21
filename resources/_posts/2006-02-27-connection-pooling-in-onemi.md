---
layout: post
title: "Connection Pooling' in Önemi"
date: 2006-02-27 08:00:00 +0300
categories:
  - ado-net-2-0
tags:
  - ado.net
  - connection-pooling
---
Connectilon Pooling veritabanı programcılığında, uygulamaların performansını doğrudan etkiliyen unsurlardan birisidir. Bağlantıların bir havuza atılarak buradan kullanılmalarını sağlamaktaki en büyük amaç, çok sayıda kullanıcının bağlı olduğu veri tabanlı uygulamalarda, aynı özelliklere sahip bağlantı bilgilerinin defalarca oluşturulmasınının önüne geçmek bu sayede var olan açık bağlantıların kullanılabilmesini sağlamaktır. Temel mantık son derece basittir. Bir kullanıcı uygulaması içerisinden bir verikaynağına bağlanmak istediğinde, geçerli bir Connection nesnesi oluşturmak zorundadır. Bu Connection nesnesi eğer ilk kez talep edilmişse, veritabanı tarafında bir bağlantı havuzunun içine atılacaktır.

Ki başka bir kullanıcı aynı veri kaynağına aynı bağlantı bilgisi ile bağlanmak istediğinde, havuzdaki bağlantıyı kullanabilecektir. Burada aynı bağlantı bilgisine başvuran birden fazla kullanıcı olduğunu düşündüğümüzde bu mimarinin önemi ortaya çıkmaktadır. Özellikle web tabanlı uygulamalarda bu fark büyük performans kazanımı anlamına gelmektedir. Biz bu makalemizde Connection Pooling'in mimarisini incelemektense onu kullanırken dikkat etmemiz gereken noktalara değineceğiz. Ama herşeyden önce connection pooling'i kullanmanın faydasını göreceğimiz basit bir örnek ile yola çıkmakta fayda olacağı kanısındayım. Bunun için, Vs.2005' de aşağıdaki kodlara sahip basit bir console uygulaması yazarak işe başlayalım.

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace UsingPooling
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection("data source=MANCHESTER;database=AdventureWorks;integrated security=SSPI;Pooling=false");
            DateTime dtBaslangic = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                con.Open();
                con.Close();
            }
            DateTime dtBitis = DateTime.Now;
            TimeSpan tsGecerSure = dtBitis - dtBaslangic;
            Console.WriteLine("Geçen süre {0} milisaniyedir.", tsGecerSure.TotalMilliseconds.ToString());
        }
    }
}
```

Öncelikle bu uygulamada ne yapıyoruz kısaca bunu açıklayalım. Uygulamamız Sql Server 2005 üzerinde yer alan AdventureWorks isimli veritabanına, 10000 defa bir bağlantı açıp kapatıyor. Burada döngü içerisinde gerçekleşen olayları, sisteme bağlanan 10000 farklı kullanıcının kod kısmı olarakta düşünebilirisiniz. Aynı bağlantı bilgisi için defalarca açma ve kapatma işlemini yapıyoruz ve bu işlemler sonrası oluşan süre farkına bakıyoruz. Burada bağlantı bilgisine dikkat ederseniz Pooling özelliğinin bilerekten false olarak atandığını görürsünüz. Yani ilgili connection bilgisinin herhangibir şekilde havuza atılmayacağını (Connection Pool'da tutulmayacağını) belirtmiş oluyoruz. Bu haliyle uygulamamızı çalıştırdığımızda aşağıdaki gibi (ya da kullanıdığınız çevresel parametrelere göre bu sonuca yakın) bir süre farkı elde ederiz.

![mk149_1.gif](/assets/images/2006/mk149_1.gif)

Ancak Pooling=false özelliğini kaldırırsak (ki varsayılan hali true'dur ve pooling'in aktif olmasını sağlar) süre farkı çok daha az olacaktır.

![mk149_2.gif](/assets/images/2006/mk149_2.gif)

Görüldüğü gibi, her iki uygulamanın çalışma süreleri arasında belirgin ve aynı zamanda dikkate değer bir zaman farkı oluşmuştur. Bu, Connection Pooling'in herhangibir Connection nesnesine ait bağlantı bilgisi içerisinde hiç bir şey belirtilmesse zaten aktif olacağına şükredebileceğimiz bir durumdur. Ancak kodlama farklarından dolayı Pooling zaman zaman başımızı derde sokabilir. Bizim için sancı yaratacak iki farklı durum vardır. Şimdi kısaca bu durumlar inceleyeceğiz. İlk olarak aşağıdaki kodlara sahip olan ve Vs.2005 üzerinde geliştirilmiş basit bir Windows Uygulamasını göz önüne alalım.

![mk149_4.gif](/assets/images/2006/mk149_4.gif)

```csharp
private void btnExecute_Click(object sender, EventArgs e)
{
    try
    {
        SqlConnection con = new SqlConnection("data source=MANCHESTER;database=AdventureWorks;integrated security=SSPI;Min Pool Size=10;Max Pool Size=15"); 
        SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM Person.Contat", con);
        con.Open();
        int kontakSayisi=Convert.ToInt32(cmd.ExecuteScalar());
        con.Close();
        }
        catch(Exception err)
        {
            lstExceptions.Items.Add(err.ToString());
        }
}
```

Burada button kontrolüne basıldığında çalışan kodlarda, Person.Contat isimli tablodaki kayıt sayısını öğrenebileceğimiz bir sorgunun çalıştırılmasını görüyoruz. Burada kod, try/catch bloğuna alındığından, sql komutlarının yürütülmesi yada bağlantının açılması sırasında oluşacak hatalara karşı programı koruma altına aldığımızı düşünebiliriz. Ancak Contat (Contact olması gerekirdir) tablosu AdventureWorks isimli veritabanında mevcut değildir. Bu yüzden kod çalışma zamanında bir SqlException verecektir. Ancak bu kodu birden fazla sayıda kullanıcının tetiklediğini düşünürsek (örneğimizde butona defalarca basarak bu durumu canlandırabiliriz) başımıza korkunç işler gelebilir.

Öyleki, SqlCommand sınıfından olan cmd nesnesi execute edilirken oluşan hata, con nesnesinin kapatılmasını engellemektedir. Bu sunucu üzerindeki bağlantı havuzunda açık kalan bir bağlantı demektir. Button kontrolümüz tetiklendikçe, açık bağlantı sayısı artmaya ve bir süre sonrada bağlantı havuzundaki maksimum bağlantı sayısını aşmaya başlayacaktır. Bu durum böyle devam ederekten sonuçta uygulamanın SqlException istisnasından vazgeçerek zaman aşımına bağlı olaraktan InvalidOperationException istisnasını fırlatmasına neden olacaktır. Şu aşamada istisnanın tipinden ziyade açık kalan bağlantıları sürekli olarak artması çok tehlikeli bir durumdur. Aşağıdaki ekran görüntüsü, yukarıdaki uygulamada button kontrolüne defalarca basılaraktan elde edilmiştir.

![mk149_3.gif](/assets/images/2006/mk149_3.gif)

Bu görüntüde yer alan NumberOfReclaimedConnections sayacı (Counter),.Net 2.0 ile birlikte gelen yeni performans ölçüm değerlerinden birisidir ve havuzda yer alıpta kapatılamayan bağlantı sayılarına ilişkin bilgileri vermektedir. Grafiktende görüleceği üzere açık bağlantılar sürekli artmıştır. Uygulama bunun sonucunda SqlException'dan çıkarak zaman aşımı (Timeout) nedeni ile InvalidOperationException'a sürüklenmiştir.

![dikkat.gif](/assets/images/2006/dikkat.gif)
NumberOfReclaimedConnections,.Net 2.0 ile birlikte gelen Performans sayaçlarından (Performance Counter) birisidir. Bu sayaç.Net Data Provider For Sql Server performans nesnesi (Performance Object) altında yer almaktadır.
![mk149_5.gif](/assets/images/2006/mk149_5.gif)

Aslında hatanın nedeni son derece basittir. Connection sınıfına ait nesne örneğinin kapatılması garanti altına alınmamıştır. Bu gerçekten önemli bir hatadır. Bu durumu düzeltmek için ya finally bloğu eklenmeli ya da işlemler aşağıdaki kod parçasında olduğu gibi using blokları içerisinde gerçekleştirilmelidir. Nitekim using bloğu doğal olaraktan kullandığı nesnenin dispose edilmesini garanti altına alır.

```csharp
private void btnExecute_Click(object sender, EventArgs e)
{
    try
    {
        using (SqlConnection con = new SqlConnection("data source=MANCHESTER;database=AdventureWorks;integrated security=SSPI;Min Pool Size=10;Max Pool Size=15"))
        {
            using (SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM Person.Contat", con))
            {
                con.Open();
                int kontakSayisi = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
    catch (Exception err)
    {
        lstExceptions.Items.Add(err.ToString());
    }
}
```

![mk149_6.gif](/assets/images/2006/mk149_6.gif)

Görüldüğü gibi bu kez havuzda açık kalan hiç bir bağlantı yoktur. Bunu görmek için uygulamayı test ettiğimizde, yukarıdaki ekran görüntüsünde olduğu gibi NumberOfRecalimedConnections sayacının (Counter) sıfır olarak seyrettiğini görürüz. Dolayısıyla uygulamadaki bağlantılar için maksimum havuz boyutunun aşılması ve nihayetinde InvalidOperationException istisnasına sürüklenilmesi engellenmiştir. Herşeyden önemlisi ne kadar çok kullanıcı bu kodu çalışıtırırsa çalıştırsın, havuzda açık kalan herhangibir bağlantı olmayacaktır. Şimdi gelelim connection pooling ile ilgili diğer önemli noktaya. Aşağıdaki örnek console uygulaması bu durumu canlandırmak için geliştirilmiştir.

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Pooling_3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                for (int i = 1; i < 100; i++)
                {
                    KontakSayisiniBul();
                }
            }
            catch (SqlException err)
            {
                Console.WriteLine(err.Message.ToString());
            }
            Console.ReadLine();
        }
    
        private static void KontakSayisiniBul()
        {
            using (SqlConnection con = new SqlConnection("data source=MANCHESTER;database=AdventureWorks;integrated security=SSPI;Min Pool Size=5;Max Pool Size=15"))
            {
                SqlCommand cmd = new SqlCommand("Select Count(*) From Person.Contact", con);
                con.Open();
                int kontakSayisi = Convert.ToInt32(cmd.ExecuteScalar());
                System.Threading.Thread.Sleep(1500);
            }
        }
    }
}
```

Bu örnekte son derece anlamsız bir şekilde 100 defa Contact tablosundaki eleman sayısını hesaplatmaktayız. Bu işlemleri yaparken durumu daha iyi analiz edebilmek içinde kodu 1,5 saniye kadar duraksatıyoruz. Bu işlem sonradan Sql Server Servisini restart etmemizde bize zaman kazandıracaktır. Kodu, aynı işlemi sunucuya doğru gerçekleştiren n sayıda kullanıcı ekranına ait bir uygulamanın parçası olarakta düşünebiliriz. N sayıda kullanıcı sunucuya bağlanıp sorguyu çalıştırdıkları sürece, sunucunun başına çeşitli haller gelebilir.

Örneğin, Sql Server Servisi bir şekilde baştan başlatılmış (Restart) olabilir. (Bu çoğunlukla sql sunucusunu barındıran bilgisayarın istem dışı restart olması halinde gerçekleşebilecek bir durumdur.) Eğer böyle bir durum söz konusu olursa, havuzda duran bağlantı bilgileri servis yeniden çalışsa bile erişilemez hale gelecektir. Örneğimizi çalıştırdıktan sonra Sql Server servisini baştan başlatacak olursak, servis durup yeniden çalışmaya başladığında ilgili uygulama ortama bir SqlException istisnası fırlatacaktır.

![mk149_7.gif](/assets/images/2006/mk149_7.gif)

Bu istisnanın nedeni artık havuzda duran bağlantı bilgilerinin yapısının, servisin yeniden başlatılması nedeni ile bozulmuş olmasıdır. Bu sorunu çözebilmek için yapılabileceklerden bir tanesi ve en kolayı, havuzdaki bağlantıları sıfırlamak bir başka deyişle havuzu boşaltmaktır. Ado.Net 2.0' da Connection sınıflarına bu işlemleri kolay bir şekilde yapabilmek için iki yeni metod eklenmiştir. Bu metodların prototipleri aşağıdaki gibidir.

Metod
Kısa Açıklama

public static void ClearAllPools ()
Havudaki tüm bağlantıları boşaltır.

public static void ClearPool (SqlConnection connection)
Parametre olarak verilen bağlantıya ait havuzu boşaltır.

Bu metodların uygulanması halinde yukarıdaki çalışma zamanı hatası ile başedebiliriz. Aslında bu metodlar, bağlantı havuzunu boşaltırken ilgili bağlantı nesnelerini kapatmazlar. Sadece bunların artık kullanılmayacağını belirtirler. Yukarıdaki örnek uygulamamızı aşağıdaki gibi değiştirmemiz etkili bir çalışma zamanı çözümü olacaktır.

```csharp
try
{
    for (int i = 1; i < 100; i++)
    {
        KontakSayisiniBul();
    }
}
catch (SqlException err)
{
    if (err.Number == 233)
    {
        SqlConnection.ClearAllPools();
    }
}
```

Uygulamamızı bu haliyle çalıştırırsak çalışma zamanında herhangibir istisna almayız. Az önce değindiğimiz gibi bu sorunun daha zor olan ama daha güçlü olan bir çözümü daha vardır. Bu Failover Partner adı verilen gene bir çözümdür. Kısaca, sql sunucusunun yanında ayna (Mirror) görevi gören bir sunucu ve birde tanık (Witness) görevi gören başka bir sunucu vardır. Bu sistemin konusu makalemizin sınırlarını aşmaktadır. Bu konuya ilerleyen zamanlarda ayrıca vakit ayırmayı düşünüyorum. Görüldüğü gibi bağlantı havuzlarını kullanırken başımıza gelebilecek iki önemli tehlike üzerinde durmaya çalıştık. İlki kesin olarak kapatılmayan bağlantı nesnelerinin yol açtığı sorundu. Bunun çözümü için en etkili yöntem olarak using bloklarına başvurduk. Diğer sorunumuz ise, çalışma zamanında veritabanı sunucusunun herhangibir neden ile sıfırlanmasıyıdı. Bu sorunu ise, hata kodunu ele alıp bağlanyı havuzlarını boşaltarak çözdük. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.