---
layout: post
title: "Ado.Net Data Services Ders Notları - 4 (CUD Operasyonları)"
date: 2008-10-16 12:00:00 +0300
categories:
  - ado-net-data-services
tags:
  - ado-net-data-services
  - csharp
  - dotnet
  - aspnet
  - ado-net
  - linq
  - sql-server
  - wcf
  - wpf
  - http
  - generics
---
Ders notlarımızı tutmaya devam ediyoruz. Bu gün Ado.Net Data Service'ler yardımıyla istemcilerden veri ekleme (Insert), silme (Delete) ve güncelleme (Update) işlemlerinin nasıl yapılabileceğini incelemeye karar verdim. Tabiki Ado.Net Data Services konusu halen daha Astoria kod adıyla anılmakta. Dolayısıyla zaman içerisinde uygulanan metod adlarında ve kullanılış biçimlerinde değişiklikler olması muhtemel. Yine şu an itibariyle neler yapabileceğimize bakmakta yarar var nitekim bir WCF fanatiği olarak Ado.Net Data Services açılımı beni son derece heyecanlandırıyor. Bu kadar laf salatasından sonra kısaca konuya girmeye ve basit bir örnek geliştirmeye ne dersiniz?

Ado.Net Data Service operasyonlarına yapılan istemci çağrılarının HTTP bazlı olduklarını ve GET,POST,PUT,DELETE gibi metodlara göre uygulandıklarını biliyoruz. İstemci tarafından servis operasyonlarına doğru eklenmek, silinmek veya güncellenmek amacıyla gönderilen verilerin çoğunluğuda POST metoduna uygun olacak şekilde paketlenmektedir. Ancak elbetteki istemci tarafında bu paketin manuel olarak hazırlanması gibi işlemler ile uğraşmamıza gerek yoktur. Nitekim istemci tarafında oluşturulan servis örneğine ait üye metodlar yardımıyla bu paketlerin otomatik olarak hazırlanması, gönderilmesi sağlanabilmektedir. Her zamanki gibi adım adım ilerleyeceğimiz bir örnek konuyu pekiştirmek açısından çok daha yararlı olacaktır.

İlk olarak veritabanı üzerindeki hazırlıklarımızı yapalım. Örneğimizde Azon isimli (Benim seminerlerimi takip edenler bu isimdeki hayali şirketi hatırlayacaktır:)) bir veritabanını ve bunun üzerinde yer alan Kategori ve Kitap isimli tabloları kullanıyor olacağız. Şimdi hiç vakit kaybetmeden aşağıdaki SQL Script'ini SQL Management Studio üzerinde çalıştırabilir ve örnek veritabanı, tablo ve test verilerinin eklenmesini sağlayabilirsiniz.

```text
--Test veritabanı oluşturulur
Create Database Azon
GO

--Test veritabanını kullan
Use Azon
GO

-- Kategori tablosu oluşturulur
Create Table Kategori
(
    KategoriId int identity(1,1) not null,
    Ad nvarchar(20) not null,
    Constraint Pk_Kategori Primary Key(KategoriId)
)
GO

--Kategori tablosu için test verileri girilir
Insert into Kategori (Ad) Values ('Programlama');
Insert into Kategori (Ad) Values ('SOA Çözümleri');
Insert into Kategori (Ad) Values ('Web Programlama');

--Kitap tablosu oluşturulur
Create Table Kitap
(
    KitapId int Identity(1,1) not null,
    Ad nvarchar(50) not null,
    Fiyat money not null,
    StokMiktari int not null,
    KategoriId int not null,
    Constraint Pk_Kitap Primary Key(KitapId) 
)
GO

--Kitap tablosu için test verileri eklenir
Insert into Kitap (Ad,Fiyat,StokMiktari,KategoriId) Values ('Her Yönüyle C#',50,100,1);
Insert into Kitap (Ad,Fiyat,StokMiktari,KategoriId) Values ('Essential C# 3.0',80,25,1);
Insert into Kitap (Ad,Fiyat,StokMiktari,KategoriId) Values ('Programming WCF',75,120,2);
Insert into Kitap (Ad,Fiyat,StokMiktari,KategoriId) Values ('SOA For Dummies',35,45,2);
Insert into Kitap (Ad,Fiyat,StokMiktari,KategoriId) Values ('Asp.Net 3.5 Step By Step',70,80,3);

--Relation Oluşturulur
ALTER TABLE Kitap WITH CHECK ADD CONSTRAINT FK_Kitap_Kategori FOREIGN KEY(KategoriId)
REFERENCES Kategori (KategoriId)
GO

ALTER TABLE Kitap CHECK CONSTRAINT FK_Kitap_Kategori
GO
```

Kategori ve Kitap tabloları yine bir birlerine bağlı kümeler olarak tasarlanmıştır. Böylece bir Kategori ve buna bağlı Kitap satırlarının Entity örnekleri üzerinden eklenmesinin analizi için gerekli ortam hazırlanmış olur. Her iki tablo arasındaki ilişkinin veritabanı üzerinde tanımlanmış olmasıda son derece önemlidir. Nitekim bu tanımla yapılmadığı takdirde EDM (Entity Data Model) içerisinde oluşturulan Entity tipleri arasındada bir Association en azından otomatik olarak oluşmayacaktır.

Sıradaki aşamada servisimiz için gerekli host uygulamanın yazılması gerekmektedir. Ado.Net Data Service'leri bu ana kadarki ders notlarımızda sürekli olarak WCF Service şablonları üzerinde tuttuk. Şimdilik geleneği bozmuyoruz. Yine işlerimizi kolaylaştırması açısından EDM katmanını kullanıyor olacağız. Daha önceki ders notlarımızda ve görsel derslerimizde bu konuya değindiğimiz için tekrar etmeyeceğiz ancak oluşan EDM modelinin aşağıdakine benzer olması gerektiğinide hemen vurgulayalım.(Bu aşamayı tamamlarken önceki ders notları veya görsel derslere bakmadan ilerlemeye çalışmanız sizin yararınıza olacaktır. Bildiğiniz gibi pratik mükemmelleştirir.)

![mk261_1.gif](/assets/images/2008/mk261_1.gif)

KitapServisi olarak isimlendirdiğimiz svc dosyasına ait kod içeriği ise aşağıdaki gibi olmalıdır.

```csharp
using System;
using System.Linq;
using System.Data.Services;
using System.ServiceModel.Web;
using System.Collections.Generic;
using AzonModel;

public class KitapServisi 
    : DataService<AzonEntities>
{ 
    public static void InitializeService(IDataServiceConfiguration config)
    {
        config.SetEntitySetAccessRule("*", EntitySetRights.All); 
    }
}
```

Tahmin edileceği üzere AzonEntites isimli taşıyıcı tip içerisindeki tüm Entity tipleri tüm haklar ile istemcilere açılmaktadır. Burada varsayılan olarak bulunan AllRead değerini kullanamayız. Nitekim verilerin eklenmesi, güncellenmesi ve silinmesi operasyonları için izin verilmesi gerekir. Burada kolaya kaçarak All enum sabiti değerini kullandık. Bu adıma kadar geldikten sonra yapmamız gereken ilk iş servisin çalışıp çalışmadığını test etmek olmalıdır. Bu amaçla servisin herhangibir tarayıcıda açılması yeterlidir. Eğer bir sorun yoksa aşağıdaki ekran görüntüsüne benzer bir çıktının elde edilmesi gerekir.

![mk261_2.gif](/assets/images/2008/mk261_2.gif)

Sıradaki adımımızda istemci uygulamanın oluşturulması ve servis referansının eklenmesi yer almaktadır. Yine çok sevineceğiniz ve yazarken büyük keyif alacağınız bir Console Application:) geliştiriyor olacağız. Console uygulamamıza aynı solution içerisinde oluşturduğumuz Ado.Net Data Service örneğini ise Add Service Reference seçeneği ile ekleyeceğiz. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![mk261_3.gif](/assets/images/2008/mk261_3.gif)

Bu işlemin ardından solution içeriğinin aşağıdakine benzer olmasını bekleyebiliriz.

![mk261_4.gif](/assets/images/2008/mk261_4.gif)

Artık birazda kod yazalım. İlk kod örneğinde toplu bir insert işleminide gerçekleştiriyor olacağız. İlk önce bir Kategori örneğini oluşturup istemci tarafındaki Entity nesnesine ilave edecek ve bu değişikliği veritabanına doğru göndereceğiz. Sonrasında ise bu Kategori altında olacak Kitap nesnelerini örneklerinin değerlerini veri tabanına göndereceğiz. İşte kodlarımız;

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientApp.AzonSpace;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Proxy nesnesi örneklenir
            AzonEntities proxy = new AzonEntities(new Uri("http://localhost:1630/HostApp/KitapServisi.svc"));

            // Yeni bir kategori nesnesi örneklenir
            Kategori windowsClient = new Kategori { Ad = "Windows Programlama" };

            // Oluşturulan Kategori nesne örneği bellek üzerindeki Entity örneğine ilave edilir
            proxy.AddToKategori(windowsClient);
            // Yeni kategorinin veritabanındaki tabloya eklenmesi için SaveChanges metodu çağırılır.
            // (Bu noktada SaveChanges çağırılması şart değildir. Bu sadece KategoriId' nin tablodan elde edilmesini sağlamada rol oynamaktadır)
            proxy.SaveChanges();
            Console.WriteLine("{0} ID si ile {1} Kategorisi eklendi",windowsClient.KategoriId.ToString(),windowsClient.Ad);

            // Generic bir List koleksiyonunda tutulacak şekilde Kitap nesne örnekleri oluşturulur.
            List<Kitap> kitaplar = new List<Kitap>()
                {
                    new Kitap { Ad = "Windows Form 2.0 Programming", Fiyat = 90, StokMiktari = 10 },
                    new Kitap { Ad = "Pro WPF", Fiyat = 75, StokMiktari = 12 },
                    new Kitap { Ad = "Core Windows Programming", Fiyat = 90, StokMiktari = 16 }
                };

            // Tüm kitaplar dolaşılır
            foreach (Kitap k in kitaplar)
            {
                // Her bir Kitap nesne örneği ilgili Entity örneğine ilave edilir.
                proxy.AddToKitap(k); 
                // O andaki kitap ile yukarıda oluşturulan Kategori arasındaki ilişki kurulur
                proxy.AddLink(windowsClient, "Kitap", k);
            }

            // Değişiklikler veritabanına gönderilir
            // Batch enum sabit değeri ile tüm isteklerin tek bir HTTP paketinde gönderilmesi sağlanır
            proxy.SaveChanges(System.Data.Services.Client.SaveChangesOptions.Batch);

            // Eklenen kategori servis tarafından talep edilir
            var eklenenKategori = (from k in proxy.Kategori
                                            where k.KategoriId == windowsClient.KategoriId
                                            select k).First();
            // Elde edilen kategoriye ait kitap bilgilerinin yüklenmesi istenir
            proxy.LoadProperty(eklenenKategori, "Kitap");
    
            Console.WriteLine("\n{0} kategorisine eklenen kitaplar\n",eklenenKategori.Ad);
            // Eklenen kategoriye bağlı kitaplar listelenir
            foreach (Kitap k in eklenenKategori.Kitap)
            { 
                Console.WriteLine("{0} {1} {2} {3}",k.KitapId.ToString(),k.Ad,k.StokMiktari.ToString(),k.Fiyat.ToString("C2"));
            }
        }
    }
}
```

Kodlarda özellikle üzerinde durmamız gereken nokta bir Entity nesne örneğinin oluşturulması sonrasında kullanılan AddTo[EntityAdı], AddLink ve SaveChanges isimli metodlardır. AddTo[EntityAdı] metodları, servis referansının eklenmesi sırasında istemci tarafında oluşturulan Entity tiplerinin her birisi için service tipine eklenir. Söz gelimi Kitap için AddToKitap, Kategori içinse AddToKategori. Bunu sınıf diagramındanda görebiliriz.

![mk261_7.gif](/assets/images/2008/mk261_7.gif)

AddTo[EntityAdı] metodları parametre olarak aldıkları Entity nesne örneklerini taşıyıcı servis tipinin takip etmesinde rol oynarlar. Aslında kendi içlerinde AdoToObject metodunu çağırmaktadırlar. Bu izleme işlemi aslında SaveChanges metodu için önem arz etmektedir. Nitekim eklenen, silinen veya güncellenen nesne değerlerinin veritabanına doğru yansıtılması işlemini gerçekleştirmektedir. Bu noktada durup kodu debug ederek ilerlemenizi öneririm. Özellikle SaveChanges metodu çağırılmadan önce proxy nesne örneği içerisinde Kategori veya Kitap özellikleri içeriği ile SaveChanges çağrısı sonrası içeriklere bakıldığında durum daha net bir şekilde görülebilir. Söz gelimi ben testleri yaparken aşağıdaki sonuçları elde ettim.

SaveChanges çağrısı öncesi;

![mk261_8.gif](/assets/images/2008/mk261_8.gif)

SaveChanges çağrısı sonrası;

![mk261_9.gif](/assets/images/2008/mk261_9.gif)

SaveChanges metodunun çağırılması sırasında birde System.Data.Services.Client.SaveChangesOptions.Batch enum sabiti değeri kullanılmıştır. Bu değer ile birden fazla HTTP paketinin yerine tüm isteğin tek bir HTTP paketi içerisinde gönderilmesi sağlanabilmektedir bu servis ile istemci arasındaki trafik akışının yoğunluğunu azaltıcı bir etkendir. (Bu konu ile ilişkili olarak (Batching) bir görsel ders hazırlıyor olacağım.)

Tabi SaveChanges çağrısı sırasında sunucu tarafındaki veri kaynağı üzerindede bir takım SQL sorgu ifadeleri çalışacaktır. Burada SQL Server Profiler aracının kullanmanızı şiddetle tavsiye ederim. Örneğin veri ekleme testleri sırasında benim yakaladığım örnek sql ifadeleri aşağıdaki gibi olumuştur.

```text
-- Kategori için Insert çağrısı
exec sp_executesql N'insert [dbo].[Kategori]([Ad])
values (@0)
select [KategoriId]
from [dbo].[Kategori]
where @@ROWCOUNT > 0 and [KategoriId] = scope_identity()',N'@0 nvarchar(19)',@0=N'Windows Programlama'
-- İlk Kitap için Insert çağrısı
exec sp_executesql N'insert [dbo].[Kitap]([Ad], [Fiyat], [StokMiktari], [KategoriId])
values (@0, @1, @2, @3)
select [KitapId]
from [dbo].[Kitap]
where @@ROWCOUNT > 0 and [KitapId] = scope_identity()',N'@0 nvarchar(28),@1 decimal(19,4),@2 int,@3 int',@0=N'Windows Form 2.0 Programming',@1=90.0000,@2=10,@3=27

-- İkinci kitap için Insert çağrısı
exec sp_executesql N'insert [dbo].[Kitap]([Ad], [Fiyat], [StokMiktari], [KategoriId])
values (@0, @1, @2, @3)
select [KitapId]
from [dbo].[Kitap]
where @@ROWCOUNT > 0 and [KitapId] = scope_identity()',N'@0 nvarchar(7),@1 decimal(19,4),@2 int,@3 int',@0=N'Pro WPF',@1=75.0000,@2=12,@3=27

-- Üçüncü kitap için Insert çağrısı
exec sp_executesql N'insert [dbo].[Kitap]([Ad], [Fiyat], [StokMiktari], [KategoriId])
values (@0, @1, @2, @3)
select [KitapId]
from [dbo].[Kitap]
where @@ROWCOUNT > 0 and [KitapId] = scope_identity()',N'@0 nvarchar(24),@1 decimal(19,4),@2 int,@3 int',@0=N'Core Windows Programming',@1=90.0000,@2=16,@3=27
```

İlk kodda dikkat çekici noktalardan biriside Kategori nesne örneği eklendikten sonra Kitap nesne örneklerinden oluşan bir koleksiyonun nasıl ilave edildiğidir. Burada döngü içerisinde çağırılan AddToKitap metodu haricinde AddLink isimli bir fonksiyon yer almaktadır. Bu metod, o anki Kitap nesne örneğinin hangi Kategori'ye bağlı olacağının belirlenmesinde rol oynamaktadır. Bu sebepten dolayıda kodun SQL tarafındaki üretiminde 3 Kitap için çalıştırılan Insert sorgularında KategoriID değerleri otomatik olarak set edilmiştir. AddLink metodu çağırılmadığı takdirde bu ilişkinin sağlanması mümkün olmamaktadır. İlk örnek kodumuzu tamamlamış bulunuyor. İşte testler sırasında oluşan örnek bir ekran çıktısı.

![mk261_5.gif](/assets/images/2008/mk261_5.gif)

Sırada güncelleştirme işlemleri var. Bu amaçla aşağıdaki örnek kod satırlarını göz önüne alabiliriz;

```csharp
// Güncellenecek veri kümesi çekilir.
// Örneğin KategoriId değeri 1 olan Kitaplar çekilir
var tumKitaplar = from k in proxy.Kitap
                            where k.Kategori.KategoriId==1
                            select k;

// Elde edilen sonuç kümesindeki her bir Kitap nesne örneği üzerinde basit bir güncelleştirme yapılır
foreach (Kitap k in tumKitaplar)
{
    Console.WriteLine("Güncelleştirme öncesi {0} için Fiyat {1}",k.Ad,k.Fiyat.ToString("C2"));
    k.Fiyat += 10;
    // Yapılan güncellemeler entity üzerinde onaylanır
    proxy.UpdateObject(k);
}
// Değişiklikler veritabanına gönderilir
proxy.SaveChanges();

// Sonuçları test etmek için servis tarafından 1 numaralı kategoriye bağlı kitaplar tekrar istenir
Console.WriteLine("\nDeğişiklikler Sonrası Liste\n");
var kategori1Kitaplari = from k in proxy.Kitap
                                        where k.Kategori.KategoriId == 1
                                            select k;
// Her bir kitabın bilgisi ekrana yazdırılır
foreach (Kitap k in tumKitaplar)
{
    Console.WriteLine("Güncelleştirme öncesi {0} için Fiyat {1}", k.Ad, k.Fiyat.ToString("C2"));
}
```

Bu kod parçasında örnek olarak KategoriId değeri 1 olan Kategoriye bağlı Kitap nesnelerinin fiyatlarının 10 birim arttırılması sağlanmaktadır. Bizim için bu kod parçasında dikkat edilmesi gereken fonksiyonellikler UpdateObject ve yine SaveChanges metodlarıdır. SaveChanges metodu SQL tarafında aşağıdaki sorgu ifadelerinin oluşmasına neden olur.

```text
-- İki Update yakalanır. Nitekim 1 numaralı kategoride sadece iki Kitap vardır.
exec sp_executesql N'update [dbo].[Kitap]
set [Ad] = @0, [Fiyat] = @1, [StokMiktari] = @2
where ([KitapId] = @3)
',N'@0 nvarchar(14),@1 decimal(19,4),@2 int,@3 int',@0=N'Her Yönüyle C#',@1=90.0000,@2=100,@3=1

exec sp_executesql N'update [dbo].[Kitap]
set [Ad] = @0, [Fiyat] = @1, [StokMiktari] = @2
where ([KitapId] = @3)
',N'@0 nvarchar(16),@1 decimal(19,4),@2 int,@3 int',@0=N'Essential C# 3.0',@1=120.0000,@2=25,@3=2
```

Burada iki adet Update sorgusunun oluşturulması son derece doğaldır. Nitekim 1 numaralı Kategori'ye bağlı sadece iki adet Kitap bulunmaktadır. Kodun çalıştırılması sonrasında ise programın ekran görüntüsü aşağıdakine benzer olacaktır.

![mk261_6.gif](/assets/images/2008/mk261_6.gif)

Burada hemen bir test yapmanızı öneririm. İlk Console.WriteLine çağrısını UpdateObject metodunun sonrasına koyduğunuz takdirde Kitap nesne örneklerinin değerlerinin anında güncellenip güncellenmediğini (Entity tarafında) analiz edebilirsiniz.

Son olarak basit bir silme operasyonu işlemini ele alıyor olacağız. Bu son kod parçasındaki amacımız bir Kategori ve buna bağlı Kitap verilerinin silinmesini sağlamak. İşte örnek kod parçamız;

```csharp
// Önce kullanıcıya Kategori listesi sunulur
Console.WriteLine("\nSilme Operasyonu\n");
var kategoriler = from k in proxy.Kategori
                        select k;
foreach (Kategori kategori in kategoriler)
{
    Console.WriteLine("{0} {1}",kategori.KategoriId.ToString(),kategori.Ad);
}
// Kullanıcıdan silmek istediği kategorinin KategoriId değeri istenir
Console.WriteLine("Silmek istediğini kategori id' yi seçin");
int secilenKategoriId;

// Eğer ekrandan alınan değer Int32' ye Parse edilebilirse
if (Int32.TryParse(Console.ReadLine(),out secilenKategoriId))
{
    Kategori secilenKategori = null;
    try
    {
        // Ekrandan girilen ID değerine ait Kategori nesne örneği talep edilir
        secilenKategori = (from k in proxy.Kategori
                                        where k.KategoriId == secilenKategoriId
                                            select k).First<Kategori>();

        // Önce bu Kategorinin KategoriId değerine sahip Kitap listesi alınır
        var kitapListesi = from k in proxy.Kitap
                                        where k.Kategori.KategoriId == secilenKategoriId
                                            select k;
        // Elde edilen her bir Kitap nesne örneği DeleteObject metodu ile çıkartılır
        foreach (Kitap kitap in kitapListesi)
        {
            proxy.DeleteObject(kitap);
            Console.WriteLine("{0} çıkartılacak",kitap.Ad);
        }

        // Son olarak seçilmiş olan Kategori nesnesi çıkartılır
        proxy.DeleteObject(secilenKategori);
        Console.WriteLine("{0} kategorisi çıkartılacak", secilenKategori.Ad);
    
        // Değişikliklerin veri kaynağı üzerinde de yapılması için SaveChanges metodu çağırılır.
        proxy.SaveChanges(System.Data.Services.Client.SaveChangesOptions.Batch);
        Console.WriteLine("Değişiklikler gönderildi...");
    }
    catch
    {
    }
}
```

Kodda öncelikli olarak kullanıcıya var olan Kategori listesi gösterilir ve silmek istediği Kategoriye ait KategoriId değerini girmesi istenir. Bunun sonrasında söz konusu Kategori ve buna bağlı Kitaplar bulunur. Önce Kitap nesne örnekleri tek tek DeleteObject metodu ile çıkartılmak üzere işaretlenir. Sonrasında ise aynı işlem seçilen Kategori için yapılır. Son olarak tüm işlemlerin SaveChanges metodu ile veritabanına gönderilmesi sağlanır. Bu noktada SQL tarafında oluşturulan sorgu ifadeleri aşağıdakilere benzer olacaktır. (Bu ifadelerin yakalanması için SQL Server Profiler aracını kullandığımızı hatırlayalım)

```text
-- 43 nolu KategoriId değerine sahip Kitap verileri silinir
exec sp_executesql N'delete [dbo].[Kitap]
where (([KitapId] = @0) and ([KategoriId] = @1))',N'@0 int,@1 int',@0=75,@1=43

exec sp_executesql N'delete [dbo].[Kitap]
where (([KitapId] = @0) and ([KategoriId] = @1))',N'@0 int,@1 int',@0=76,@1=43

exec sp_executesql N'delete [dbo].[Kitap]
where (([KitapId] = @0) and ([KategoriId] = @1))',N'@0 int,@1 int',@0=77,@1=43

-- 43 nolu KategoriId değerine sahip Kategori silinir
exec sp_executesql N'delete [dbo].[Kategori]
where ([KategoriId] = @0)',N'@0 int',@0=43
```

Sonuç olarak örnek ekran çıktısı aşağıdaki gibi olacaktır.

![mk261_10.gif](/assets/images/2008/mk261_10.gif)

Burada hemen bir noktayı vurgulayalım. Normal şartlar altında aynı silme operasyonunu veritabanı üzerinde gerçekleştirmek istesek, önce Kitap verilerini sonra ise Kategori verilerini silmemiz gerekirdi. Bildiğinin gibi bunun nedeni Kategori ve Kitap tabloları arasında tanımlı olan Foreign Key bağımlılığı ve bunun sonucu olan Constraint tir.Aynı mantık kod tarafında şart değildir. Bir başka deyişle önce Kategori için DeleteObject sonrasında ise Kitap topluluğu için DeleteObject metodu çağırılabilir. Nitekim organizasyon ve doğru SQL çalıştırma sırası SaveChanges metodu sonrasında oluşmaktadır. Bunu kod üzerinde deneyerek test etmenizi öneririm.

Buraya kadar yaptıklarımızı kısaca özetlersek eğer veri ekleme, güncelleme ve silme işlemleri sırasında SaveChanges metodunun asıl işi yüklendiğini ve veri kaynağında sorgu ifadelerinin oluşturulması için gerekli HTTP paketlerini hazırladığını düşünebiliriz. İstemci tarafında nesne örneği eklemek için AddTo[EntityAdı] yada AddToObject metodlarını kullanabileceğimizi gördük (Sonuçta AddTo[EntityAdı] kendi içinde AddToObject metodunu çağırmakta). Ayrıca silme işlemlerinde DeleteObject ve güncelleştirme işlemlerinde ise UpdateObject fonksiyonlarını ele aldık.

Diğer taraftan ilişkisel nesnelerin bağlanması içinse AddLink fonksiyonelliğinin ele alınması gerektiğini öğrendik. Nihayetinde bir ders notumuzun sonunda daha geldik. Ancak akıllarda (en azından benim aklımda ve biraz sonra sizin aklınızda) şöyle bir soru oluşabilir. Eğer servis tarafında özel bir LINQ Provider ve buna bağlı tipler kullanılıyorsa Insert,Update ve Delete işlemleri nasıl gerçekleştirilebilir? Nitekim EDM modelinde veri kaynağında SQL olduğundan bu işlemler için SQL sorgu ifadeleri kullanılmaktadır. İşte bu analizi bir sonraki ders notlarımızda inceliyor olacağız. Böylece geldik bir makalemizin daha sonuna. Makalemizde yer alan CRUD işlemlerini [şu](http://www.csharpnedir.com/videoindir.asp?id=116) ve [bu](http://www.csharpnedir.com/videoindir.asp?id=117) görsel derslerden de inceleyebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Örneği indirmek için tıklayın](/assets/files/2008/InsertUpdateDelete.rar)