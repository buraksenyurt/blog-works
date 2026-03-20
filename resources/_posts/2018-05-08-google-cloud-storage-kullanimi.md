---
layout: post
title: "Google Cloud Storage Kullanımı"
date: 2018-05-08 07:23:00 +0300
categories:
  - dotnet-core
  - gcp
tags:
  - dotnet-core
  - gcp
  - bash
  - csharp
  - dotnet
  - rest
  - json
  - authentication
  - authorization
  - python
  - java
  - ruby
  - nodejs
---
Vakti zamanında sıkı bir Instagram kullanıcısıydım. En güzel fotoğrafları yakalamaya çalışır, anı görüntüleyip tüm bağlantılarımla paylaşırdım. Derken bir gün "ne yapıyorum ben yahu?" oldum. Neden o anı ille de herkesle paylaşma ihtiyacı hissediyordum. Bazen o anın fotoğrafını çekmek gerekmiyordu. Hatta hiç çekmediğim zamanlarda aklıma nasıl kazıdığımı bile unutmuştum. Üstelik ona ayıracağım zamanı pekala başka değerli şeylere de ayırabilirdim. Örneğin yeni şeyler öğrenmeye, makale yazmaya vs...

![gcpstorage_1.gif](/assets/images/2018/gcpstorage_1.gif)

Görmekte olduğunuz fotoğraf Instagram arşivimden bir kare. O zamanlar denediğim aynasız fotoğraf makinem (Sony Alpha idi. Hafif ve kullanışlıydı) ile çekmiştim. Her zaman ki gibi büyük bir keyifle tamamladığım Ghostbusters'ın efsane arabası Ecto...Bugün işleyeceğimiz makaleye konu olacak olan fotoğraf. Haydi gelin başlayalım.

Google Cloud Platform üzerinden sunulan hizmetleri incelemeye devam ettiğim bir Cumartesi gecesi yolum Storage isimli RESTful API'ye düştü. Bu API sayesinde Google Cloud Platform üzerinde konuşlandırdığımız verilere (her tür içerik söz konusu olabilir anladığım kadarıyla) erişmemiz mümkün. Fotoğraf, makale, doküman vb içerikler depolama denince aklımıza ilk gelen çeşitler.

Storage API'sinin sunduğu fonksiyonellikler sayesinde bu tip içerikleri bize ait bucket alanlarına taşımamız ve okumamız mümkün. Hatta bu içerikleri genele veya kişilere özel olacak şekilde sunabiliriz de. Elbette Google Cloud Platform içerisinde bu API'yi uygulamalar arası değerlendirmek de mümkün ki asıl amaçlarından birisi de bu zaten. Söz gelimi Bucket'a (Kova gibi de düşünülebilir) bir fotoğraf attıktan sonra, Pub/Sub API'den de yararlanarak (geçenlerde incelemiştik hatırlarsanız) yüklenen dokümanın bir diğer akıllı Google servisi tarafından işlenmesini sağlayabiliriz. Ya da platforma taşıdığımız bir web uygulamasının css, image gibi çeşitli kaynaklarını bu alanlardan sağlayabiliriz. Bunlar tabii benim için uç senaryolar. Şimdilik tekil parçaları nasıl kullanabilirimin peşindeyim.

Storage API'sini kullanmak oldukça kolay. Komut satırından gsutil aracı (Google Cloud SDK'ye sahip olduğunuzu düşünüyorum) yardımıyla tüm temel işlemleri gerçekleştirebiliriz..Net Core, Python, Go, Ruby, Node.js, Java gibi pek çok dilin de söz konusu API'yi kullanabilmek için geliştirilmiş paketleri bulunuyor. Dolayısıyla kod tarafından da ilgili servisi kullanmak mümkün.

Ben tahmin edileceği üzere öncelikle komut satırından ve sonrasında da.Net Core tarafından söz konusu servisi kullanmaya çalışacağım. Senaryom oldukça basit. Bir bucket oluştur, buraya fotoğraf at, attığın fotoğrafı oku, bucket'ı sil vb...Şunu unutmamak lazım ki, bu servis diğer Google servisleri gibi ücretlendirmeye tabii olabilir. O nedenle denemelerden sonra oluşturulan bucket veya içeriği silmek gerekiyor.

gsUtil ile Storage İşlemleri

İlk olarak gsutil ile söz konusu işlemleri nasıl yapabileceğimize bir bakalım. Tabii işe başlamadan önce Google Cloud Platform üzerinde bir proje oluşturmamız ve özellikle Storage API'yi kullanacak geçerli bir servis kullanıcısı üreterek bu kullanıcıya ait Credential bilgilerini taşıyan json formatlı içeriği kendi ortamımızda işaret etmemiz gerekiyor.

Ben bu işlemlere önceki makalelerde değindiğim için tekrar üstünden geçmiyorum. Diğer yandan Google dokümantasyonuna göre sitemde Python'un en azından 2.7 sürümünün yüklü olması gerekiyor. West-World'de bu sürüm mevcut. Ben çalışmam sırasında aşağıdaki komutları denedim.

```bash
gsutil mb gs://article-images-bucket/
gsutil cp legom.jpg gs://article-images-bucket
gsutil ls gs://article-images-bucket
gsutil ls -l gs://article-images-bucket
gsutil acl ch -u AllUsers:R gs://article-images-bucket/legom.jpg gsutil acl ch -d AllUsers gs://article-images-bucket/legom.jpg
gsutil rm gs://article-images-bucket/legom.jpg
gsutil rm -r gs://article-images-bucket
```

![gcpstorage_4.gif](/assets/images/2018/gcpstorage_4.gif)

mb komutu ile Storage üzerinde article-images-bucket isimli bir bucket oluşturuyoruz. Bu işlem sonrasında Google kontrol paneline gittiğimde bucket örneğinin oluşturulduğunu da gördüm (Sevindirdi)

![gcstorage_2.gif](/assets/images/2018/gcstorage_2.gif)

cp parametresini kullanarak bir bucket'a dosya yüklememiz de mümkün. Ben legom.jpg dosyasını yükledim (farklı dosya tiplerinden içerikler de kullanılabilir) Sonuç aşağıdaki resimde görüldüğü gibiydi.

![gcpstorage_3.gif](/assets/images/2018/gcpstorage_3.gif)

ls parametresi ile bucket içerisindeki dosyaları görmek mümkün. Eğer bu dosyaların detay bilgisine ulaşmak istiyorsak bu durumda -l anahtarını kullanmak gerekiyor. acl kullanılan kısımda internetteki tüm kullancıların legom.jpg dosyasını okuyabileceğini belirtiyoruz. Bir nevi Access Control List için AllUsers rolüne Read hakkı verdiğimizi ifade edebiliriz. Burada belirli kullanıcılara çeşitli haklar vermemiz de mümkün. Söz gelimi ilgli Storage içerisindeki dosyaları startup takımızdaki kişilerin kullanımına açabiliriz. Nasıl yapılabileceğini bulmaya ne dersiniz?;)

rm kullanılan komutlar ile bucket içerisinden öğe veya bucket'ın kendisini silebiliriz. Ben fiyatlandırma korkusu nedeniyle, denememi yapar yapmaz ilgili içerikleri sildim:)

Credential Mevzusu

Bir Google Cloud servisini kullanacağımız zaman, bu servis özelinde çalışacak bir servis kullanıcısı oluşturulması ve üretilen json dosyasının kullanılması öneriliyor. Bunu API Services -> Credentials kısmından New Service Account ile yapabiliriz. Önemli olan oluşturulan veya var olan kullanıcı için Storage servisi (ya da hangi servisi kullandırtmak istiyorsak onun için) bir role belirlenmesidir.

![gcpstorage_5.gif](/assets/images/2018/gcpstorage_5.gif)

Ben bu senaryo için my-storage-master isimli bir kullanıcı oluşturup Storage API servisinde Admin rolü ile ilişkilendirdim. Kullanıcı oluşturulduktan sonra da yetkilendirme bilgilerini taşıyan JSON formatlı dosyayı ürettirdim.

![gcpstorage_6.gif](/assets/images/2018/gcpstorage_6.gif)

Bu dosyayı kod tarafındaki servise ait Credential bilgilerini yüklemek için kullanacağız.

> Aslında Credentials vakasına baktığımızda kullanılabilecek bir çok yol mevcut. Sistemin Google_Application_Credentials anahtar değerine sahip path bilgisini bu dosya ile eşleştirebileceğimiz gibi kod tarafında farklı şekillerde Credential bilgisini yüklememiz de mümkün. Detaylar için [buradaki yazıya bakmanızı](https://cloud.google.com/docs/authentication/production) öneririm.

.Net Core Tarafı

Görüldüğü üzere komut satırında gsutil aracını kullanarak Storage API ile konuşmak oldukça basit ve pratik. Neler yapabileceğimizi az çok anladık. Şimdi kod yoluyla Storage API'sini nasıl kullanabileceğimizi incelemeye çalışalım. Her zaman ki gibi konuyu basit şekilde ele almak adına bir Console uygulaması oluşturarak işe başlayabiliriz. Sonrasında Google.Cloud.Storage.V1 paketini uygulamaya dahil etmek gerekiyor. Bu paket REST modelli Storage servisi ile konuşmamızı kolaylaştıracak (Düşündüm de günümüzde her yer RESTful servis sanki) Dolayısıyla ilk terminal komutlarımız şunlar...

```bash
dotnet new console -o howtostorage
dotnet add package Google.Cloud.Storage.V1
```

Program.cs içeriğini aşağıdaki gibi geliştirebiliriz.

```csharp
using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace howtostorage
{
    class Program
    {
        static StorageClient storageClient;

        static void Main(string[] args)
        {
            var credential = GoogleCredential.FromFile("my-starwars-game-project-credentials.json");
            storageClient = StorageClient.Create(credential);
            var projectId = "subtle-seer-193315";
            var bucketName = "article-images-bucket";
            CreateBucket(projectId, bucketName);
            WriteBucketList(projectId);
            UploadObject(bucketName, "legom.jpg", "legom");
            UploadObject(bucketName, "pinkfloyd.jpg", "pink-floyd");
            WriteBucketObjects(bucketName);
            DownloadObject(bucketName, "legom","lego-from-google.jpg");
            DownloadObject(bucketName, "pink-floyd","pink-floyd-from-google.jpg");
            
            Console.WriteLine("Yüklenen nesneler silinecek");
            Console.ReadLine();

            DeleteObject(bucketName, "pink-floyd");
            DeleteObject(bucketName, "legom");
            DeleteBucket(bucketName);
        }
        static void CreateBucket(string projectId, string bucketName)
        {
            try
            {
                storageClient.CreateBucket(projectId, bucketName);
                Console.WriteLine($"{bucketName} oluşturuldu.");
            }
            catch (Google.GoogleApiException e)
            when (e.Error.Code == 409)
            {
                Console.WriteLine(e.Error.Message);
            }
        }
        static void WriteBucketList(string projectId)
        {
            foreach (var bucket in storageClient.ListBuckets(projectId))
            {
                Console.WriteLine($"{bucket.Name},{bucket.TimeCreated}");
            }
        }
        static void UploadObject(string bucketName, string filePath,string objectName = null)
        {
            using (var stream = File.OpenRead(filePath))
            {
                objectName = objectName ?? Path.GetFileName(filePath);
                storageClient.UploadObject(bucketName, objectName, null, stream);
                Console.WriteLine($"{objectName} yüklendi.");
            }
        }
        static void WriteBucketObjects(string bucketName)
        {
            foreach (var obj in storageClient.ListObjects(bucketName, ""))
            {
                Console.WriteLine($"{obj.Name}({obj.Size})");
            }
        }
        static void DownloadObject(string bucketName, string objectName,string filePath = null)
        {
            filePath = filePath ?? Path.GetFileName(objectName);
            using (var stream = File.OpenWrite(filePath))
            {
                storageClient.DownloadObject(bucketName, objectName, stream);
            }
            Console.WriteLine($"{objectName}, {filePath} olarak indirildi.");
        }
        static void DeleteObject(string bucketName, string objectName)
        {
            storageClient.DeleteObject(bucketName, objectName);
            Console.WriteLine($"{objectName} silindi.");
        }
        static void DeleteBucket(string bucketName)
        {
            storageClient.DeleteBucket(bucketName);
            Console.WriteLine($"{bucketName} silindi.");
        }
    }
}
```

Neler yaptığımıza bir bakalım. Kodun akışı Credential bilgilerini içeren dosyanın okunması ve elde edilen güvenlik kriterleri ile StorageClient nesnesinin örneklenmesi ile başlıyor. Sonrasında projeId bilgisi ve örnek bir bucket adı kullanılarak işlemlere başlanıyor. Bucket'ın oluşturulması, güncel bucket listesine bakılması, iki resim dosyasının yüklenmesi, bucket içerisindeki nesnelerin çekilmesi, bir resim dosyasının Google'dan West-World'e getirilmesi ve son olarak da tüm nesnelerin platformdan silinmesi işlemleri gerçekleştiriliyor.

Aslında tüm operasyon StorageClient nesnesinin metodları ile icra edilmekte. Google'ın diğer API servisleri için geliştirilmiş Client kütüphanelerinde de benzer standartlar mevcut. Bu nedenle birisini öğrendikten sonra diğerlerini kullanmak da kolay olacaktır diye düşünüyorum.

CreateBucket metodu ile plaform üzerinde bir bucket oluşturulması sağlanıyor. Parametre olarak hangi projeyi kullanacaksak onun ID bilgisi ve bir de bucket adı veriliyor. Pek tabii oluşturulmak istenen bucket zaten varsa 409 kodlu bir Exception alınmakta. Platform üzerinde oluşturulan bucket listesini ListBuckets metodu ile çekebiliriz. Parametre olarak projeID bilgisini vermek yeterli.

Bucket içerisine nesne atma işi aslında Stream temelli. Nitekim yükleyeceğimiz içerikler en nihayetinde bir byte dizisine tekabül etmekte. UploadObject fonksiyonunun son parametresi de bu stream nesnesi. Bucket içerisindeki nesneleri ListObjects fonksiyonu yardımıyla yakalamamız mümkün. Örnek kod parçasında bu nesnelerin isim ve boyutlarını alıp ekrana yazdırmaktayız. Nesne silme işlemi de oldukça kolay. DeleteObject ve DeleteBucket fonksiyonlarından yararlanarak bir bucket nesnesini veya içerisindeki bir öğeyi silmek mümkün.

Programı çalıştırdıktan sonra iki aşamalı olarak elde ettiğim sonuçlara baktım. Öncelikle oluşturulan bucket'a iki resim dosyasının da yüklendiğini gözlemledim.

![gcpstorage_7.gif](/assets/images/2018/gcpstorage_7.gif)

Tuşa basıp ilerlediğimdeyse hem yüklediğim dosyaların hem de bucket'ın kendisinin silindiğini gördüm.

![gcpstorage_8.gif](/assets/images/2018/gcpstorage_8.gif)

Görüldüğü üzere Google Cloud Platform'un kullanışlı API hizmetlerinden birisi olan Storage servisini kullanmak oldukça kolay. Artık kendi ortamınızdaki nesneleri Storage'a taşıyabilir buradan tetikleteceğiniz olaylar ile başka süreçlerin devreye girmesini sağlayabilirsiniz (diye düşünüyorum) Böylece geldik bir araştırmamızın daha sonuna. Doğruyu söylemek gerekirse Google Cloud Platform'un sunduğu API'lerle oynamak epey keyifli. Azure, AWS tarafından sunulan fonksiyonellikler için de aynı durumun söz konusu olduğunu belirtmek isterim. Bu tip fonksiyonellikleri deneyimleyerek bulut platformların imkanlarını daha iyi kavrayabilir ve çözüm üretme noktasında daha rahat hareket edebiliriz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
