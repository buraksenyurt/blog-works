---
title: ".Net Core ile Amazon DynamoDB'yi Kullanmak"
pubDate: 2018-04-16 06:00:00
categories:
  - .Net Core
  - AWS
tags:
  - .Net Core
  - AWS
  - Bash
  - C#
  - .NET
  - Entity Framework
  - NoSQL
  - Redis
  - REST
  - JSON
  - Web Service
  - HTTP
  - Generics
  - Visual Studio
  - GitHub
---

# .Net Core ile Amazon DynamoDB'yi Kullanmak
![dynamocore_halo.gif](images/dynamocore_halo.gif)

Merhaba Arkadaşlar,

Epey zamandır NoSQL veritabanı sistemlerini kurcalamıyordum. Ağırlıklı olarak.Net Framework tarafında nasıl kullanılabildiklerini incelediğimi hatırlıyorum. 2017nin son çeyreği ve 2018in tamamı için kendime koyduğum hedeflerden birisi ise.Net Core dünyasını daha yakından tanımaktı. Zaten Ubuntu üzerinde koşan West-World'ün kurulum amacı da buydu. Sonuç olarak Amazon'un DynamoDb'sini.Net Core tarafında nasıl kullanabileceğimi incelemeye karar verdim. Bir süredir AWS Console üzerinden bir şeyler araştırıyor ve Amazon Web Service ürünleri hakkında giriş niteliğinde bilgiler edinmeye çalışıyorum.

Amazon DynamoDb şemasız (schema-less) olarak kullanılabilen bir NoSQL veritabanı sistemi olarak karşımıza çıkıyor. Key-Value tipine göre çalışan (ama Document Store'a da benzeyen) bir model sunduğunu söyleyebiliriz. Belli koşullar gerçekleşinceye kadar tamamen ücretsiz kullanılabilen hızlı bir veri tabanı da aynı zamanda. Bununla birlikte veriyi hızlı SSD'ler üzerinde tuttuğuna dair bir bilgi de var. Bu açıdan bakıldığında veriyi bellekte konuşlandırmayı seçen Redis'ten ayrışıyor.

Elastic MapReduce ile entegre olabilen, otomatik olarak ölçeklenebilen, Backup sürecinde S3 hizmetlerini kullanan DynamoDb ile ilgili Bahadır Akın'un [şu adreste](http://www.bahadirakin.com/aws-dynamodb-nedir/) oldukça güzel ve detaylı bir yazısı da bulunuyor. Okumanızı tavsiye ederim. Onunla ilgili çalışmalara [bu adresten](https://aws.amazon.com/dynamodb/getting-started/) hızlıca başlayabiliriz.

Amazon Console Üzerinden Basit Bir Giriş

Amazon Console web arayüzü üzerinden DynamoDb ile ilgili pek çok işlem gerçekleştirilebilir. Bir kaç dakika içerisinde tablolar oluşturabilir, insert, update, delete gibi temel veri işlemlerini gerçekleştirebiliriz. Söz gelimi sevdiğimiz oyun karakterlerine ait sözleri tutan bir tablo tasarlayabiliriz. Bunun için Create Table bağlantısından hareket etmemiz ve sayfadaki ilgili alanları doldurmamız yeterli.

![dynamocore_1.gif](images/dynamocore_1.gif)

BEn bu deneme sonucunda aşağıdaki gibi bir ekranla karşılaştım.

![dynamocore_2.gif](images/dynamocore_2.gif)

Quote isminde, Primary Key (Partition Key) olarak Game adında string tipte alan içeren bir tablo oluştu. Partition Key alanı özellikle veritabanının ölçeklendirilmesi noktasında önem arz eden bir konu. İkinci olarak eklediğimiz Sort Key alanı hızlı bir arama işlemi için ele alınacak ancak kullanılması elbette zorunlu değil. Game ve Character alanları bir arada yeni bir hash tanımının oluşmasına da neden olmakta. Tablonun tasarımını daha da detaylandırmamız mümkün. Şunu da unutmamak gerekiyor ki; iyi bir tablo tasarımı için partition key, sort key, global secondary index ve local secondary index kavramlarını iyi derecede kavramak lazım.

Oldukça detaylı ayarların yapılabildiği bir arabirim burası. Pek çok sekmeye yabancı olduğumu itiraf edebilirim. Yeni yeni keşfetmeye çalışıyorum. İlk olarak tabloya nasıl veri ekleyeceğimizi göstermeye çalışayım. Bunun için Items sekmesinden Create Item düğmesine basmak yeterli.

![dynamocore_3.gif](images/dynamocore_3.gif)

Tree olarak adlandırılan ve ağaç görünümü sunan arabirim kullanılabileceği gibi, Text moduna geçilerek JSON Formatındaki içeriğin elle yazılması da sağlanabilir.

Tree modundaki görünüm;

![dynamocore_4.gif](images/dynamocore_4.gif)

Bunun text modundaki görünümü ise aşağıdaki gibiydi.

![dynamocore_5.gif](images/dynamocore_5.gif)

Bunun üzerine bir kaç öğe daha ekledim ve aşağıdaki içeriğin oluşmasını sağladım. Benim size önerim ücretsiz olarak sunulan REST tabanlı Quote hizmetlerinden yararlanarak veri girişi yapmanız. Bu güzel bir vaka çalışması da olabilir. Örneğin her gün bağlanıp o günün özlü sözünü aldığınız REST servis içeriğini, DynamoDB üzerindeki tablonuza aktarabilirsiniz;)

![dynamocore_6.gif](images/dynamocore_6.gif)

Sonra bir filtreleme yapmaya çalıştım. Text alanı içerisinde "I" kelimesi geçenleri bulmayı denedim.

![dynamocore_7.gif](images/dynamocore_7.gif)

Ardından Halo 2 oyununda karakterin adı G harfi ile başlayanların sözlerini nasıl süzebileceğime bir baktım.

![dynamocore_8.gif](images/dynamocore_8.gif)

Tabii benim yaptığım sadece arabirimi tanımaya çalışmak. Tablo tasarımı aslına bakarsanız yanlış. Söz gelimi bir oyuna bir karakter için n sayıda söz ekleyemeyiz. Dolayısıyla tasarımı bir oyunun birbirinden farklı karakterlerine ait en iyi sözlerin tutulduğu bir depo gibi düşünebiliriz. Eğer aynı oyuna aynı karakterden bir söz daha girmeye çalışırsak şu uzun ifadeye benzer hata mesajı ile karşılaşmanız muhtemel.

"The conditional request failed (Service: AmazonDynamoDBv2; Status Code: 400; Error Code: ConditionalCheckFailedException; Request ID: KP8D9MU06E36D9AH50IF9AV99RVV4KQNSO5AEMVJF66Q9ASUAAJG)

Tablo oluşturma, içine veri atma, veriyi sorgulama gibi basit işlemlerin Amazon Console arabirimi ile nasıl yapılabileceğini gördük. Ancak amacımız başta da belirttiğimiz gibi bir.Net Core uygulaması üzerinden Amazon DynamoDb'yi kullanmak.

.Net Core Tarafını Geliştiriyoruz

En büyük yardımcımız [şu adresten sunulan DynamoDbv2 isimli NuGet](https://www.nuget.org/packages/AWSSDK.DynamoDBv2/) paketimiz olacak. Tabii birde Amazon servisine erişirken kullanacağımız geçerli bir kimlik bilgimizin (Credential) olması lazım. Dolayısıyla [IAM adresine giderek](https://console.aws.amazon.com/iam/home#/home) bu uygulama için bir kullanıcı oluşturabilirsiniz. Ben daha önceden oluşturduğum AdministratorAccess rolündeki westworld-buraksenyurt kullanıcısına ait Access Key ID ve Secret Access Key değerlerini kullanacağım. Bu değerler bildiğiniz üzere IAM üzerinden kullanıcı oluşturduğunuzda size verilmekte.

İşe bir Console projesi oluşturarak başlayabiliriz.

```bash
dotnet new console -o HowToDynamoDb
```

Bu işlemin ardından DynamoDb NuGet paketini projeye eklememiz gerekiyor.

```bash
dotnet add package AWSSDK.DynamoDBv2 --version 3.3.5
```

İlk olarak DynamoDb üzerindeki tablo listesini çekmeye çalışalım. Ben deneme olması için önce Amazon Console'dan 3 tablo oluşturdum ve Program dosyasına aşağıdaki kodları yazdım (Visual Studio Code kullanıyorum)

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace HowToDynamoDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var utl=new DynamoDBUtility();
            var tableNames = utl.GetTables();
            foreach (var tableName in tableNames)
            {
                Console.WriteLine($"{tableName}");
            }
            
        }
    }

    class DynamoDBUtility
    {
        AmazonDynamoDBClient aws;
        public DynamoDBUtility()
        {
            var myCredentials=new BasicAWSCredentials("Application Key ID","Secret Access Key");
            aws=new AmazonDynamoDBClient(myCredentials,RegionEndpoint.USEast2);
            
        }
        public List<string> GetTables()
        {
            var response = aws.ListTablesAsync();
            return response.Result.TableNames;
        }
    }
}
```

Öncelikle geçerli bir Credential bilgisi oluşturmalıyız. Bunun için BasicAWSCredentials sınıfını kullanabiliriz (Siz kendi oluşturduğunuz kullanıcıya ait Key bilgilerini girmelisiniz ki bu bilgiler konfigurasyondan da gelebilirler) Sonrasında AmazonDynamoDBClient türünden bir örnek oluşturuyoruz. Ben US-East-2 bölgesini kullandığım için ikinci parametrede RegionEndpoint.USEast2 değerini verdim. GetTables isimli metodun yaptığı işlem oldukça basit. ListTablesAsync metodu ile elde edilen tablo adlarını geriye döndürüyor. Sonuçlar aşağıdaki ekran görüntüsüne benzer olmalı (AmazonDynamoDBClient üzerindeki pek çok operasyon awaitable nitelikte. Dolayısıyla tamamen asenkron olarak kullanılabilirler de)

![dynamocore_9.gif](images/dynamocore_9.gif)

Peki kod ile sıfırdan bir tabloyu nasıl oluşturabiliriz? Kodu aşağıdaki gibi değiştirelim.

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace HowToDynamoDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var utl = new DynamoDBUtility();
            utl.CreateTable("GameQuotes","QuoteID", ScalarAttributeType.N);
            utl.CreateTable("Players","Nickname", ScalarAttributeType.S);

            var tableNames = utl.GetTables();
            foreach (var tableName in tableNames)
            {
                Console.WriteLine($"{tableName}");
            }
        }
    }

    class DynamoDBUtility
    {
        AmazonDynamoDBClient aws;
        public DynamoDBUtility()
        {
            var myCredentials=new BasicAWSCredentials("Application Key ID","Secret Access Key");
            aws = new AmazonDynamoDBClient(myCredentials, RegionEndpoint.USEast2);
        }
        public List<string> GetTables()
        {
            var response = aws.ListTablesAsync();
            return response.Result.TableNames;
        }

        public void CreateTable(string tableName,string partionKeyName,ScalarAttributeType partitionKeyType)
        {
            var tableResponse = GetTables();
            if (!tableResponse.Contains(tableName))
            {
                var response = aws.CreateTableAsync(new CreateTableRequest
                {
                    TableName = tableName,
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = partionKeyName,
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition {
                            AttributeName = partionKeyName,
                            AttributeType=partitionKeyType
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 3
                    },
                });
                Console.WriteLine($"HTTP Response : {response.Result.HttpStatusCode}");
            }
            else
            {
                Console.WriteLine($"{tableName} isimli tablo zaten var");
            }
        }
    }
}
```

CreateTable isimli operayon DynamoDB üzerinde bir tablo oluşturmak için kullanılıyor. Üç parametresi var. Tablo ve Partition Key adları ile, Partition Key'in veri türünü alıyor. ScalarAttributeType üzerinden N (number), S (string) ve B (Binary) şeklinde anahtar türünün ne olacağını belirtebiliriz. Tablo oluşturma işlemini asenkron olarak kullanılabilen (ki burada senkron bir işleyiş var) CreateTableAsync fonksiyonu gerçekleştirmekte. Tabii tabloyu oluşturmadan önce var olup olmadığını da kontrol ediyoruz. Ben GameQuotes ve Players isimli iki tabloyu oluşturmayı denedim. İşlemler sorunsuz şekilde tamamlandıktan sonra AWS Console üzerinden de oluşturulan tabloları görebildim.

![dynamocore_10.gif](images/dynamocore_10.gif)

Şimdi tablolardan birisine veri eklemeye çalışalım. Henüz elimizde bir Entity örneği bulunmuyor. Öncelikle bu varlığa ait sınıfı tasarlamak lazım. GameQuotes tablomuz için aşağıdaki gibi bir sınıf yazabiliriz.

```csharp
using Amazon.DynamoDBv2.DataModel;

namespace HowToDynamoDb
{
    [DynamoDBTable("GameQuotes")]
    public class Quote
    {
        [DynamoDBHashKey]
        public int QuoteID { get; set; }
        public QuoteInfo QuoteInfo { get; set; }
    }
    public class QuoteInfo
    {
        public string Character { get; set; }
        public int Like { get; set; }
        public string Game { get; set; }
        public string Text { get; set; }
    }
}
```

Quote isimli sınıfın başında DynamoDBTable isimli bir nitelik (attribite) yer alıyor. Bu nitelikteki isim biraz önce oluşturduğumuz tablo adı ile aynı. Ayrıca QuoteID isimli bir Parition Key tanımlamıştık. Bu anahtarı Entity tarafında DynamoDBHashKey niteliği yardımıyla işaretliyoruz. Quote sınıfı, QuoteInfo tipinden bir özellik de barındırıyor. Bu sınıfta oyunun adını, sözü söyleyen karakteri, beğeni sayısını ve tabii sözün kendisini tutuyoruz. Aslında QuoteID ile ilişkilendirdiğimiz bir içeriğin söz konusu olduğunu ifade edebiliriz. Tam bir key-value ilişkisi. JSONca düşündüğümüzde de içiçe bir tip yapısı söz konusu.

Bir Quote nesne örneğini DynamoDB üzerindeki ilgili tabloya yazmak için Utility sınıfına aşağıdaki metodu ekleyerek ilerleyelim.

```csharp
public void InsertQuote(Quote quote)
{
    var context = new DynamoDBContext(aws);           
    context.SaveAsync<Quote>(quote).Wait();            
}
```

Entity Framework dünyasındaki Context kullanımına ne kadar benziyor değil mi?:) Tek yapmamız gereken AmazonDynamoDBClient nesnesi ile ilişkilendirilmiş DynamoDBContext örneği üzerinden SaveAsync metodunu çağırmak. Generic parametre olarak Quote tipini kullandığımıza dikkat edelim.

Artık bir deneme yapabiliriz. Main metodu içerisinde aşağıdaki gibi bir kod parçası işimizi görecektir.

```csharp
var utl = new DynamoDBUtility();
Quote quote=new Quote{
QuoteID=1001,
QuoteInfo=new QuoteInfo{
    Character="Cortana",
    Game="Halo 2",
    Like=192834,
    Text="Child of my enemy, why have you come? I offer no forgiveness, a father's sins, passed to his son."
}
};
utl.InsertQuote(quote);
```

Ben Halo 2 oyunundan Cortana isimli karaktere ait bulduğum bir sözü girdim. Programı çalıştırdıktan sonra hemen Amazon Console'a gittim ve eklenen yeni satırın aşağıdaki ekran görüntüsünde olduğu gibi eklendiğini gördüm.

![dynamocore_11.gif](images/dynamocore_11.gif)

Peki kod tarafında bu içeriği nasıl çekebiliriz? Gelin aşağıdaki fonksiyonu Utility sınıfına dahil ederek devam edelim.

```csharp
public Quote FindQuoteByID(int quoteID)
{
    var context = new DynamoDBContext(aws);  
    List<ScanCondition> queryConditions = new List<ScanCondition>();
    queryConditions.Add(new ScanCondition("QuoteID", ScanOperator.Equal, quoteID));
    var queryResult = context.ScanAsync<Quote>(queryConditions).GetRemainingAsync();
    return queryResult.Result.FirstOrDefault();
}
```

Aslında bir arama koşulu listesi oluşturmaktayız. Senaryomuzda sadece bir kriter var o da Parition Key alanı olarak belirlediğimiz QuoteID değerine göre arama yapmak. ScanCondition ile belirlenen kriterde karşılaştırma koşulu, kullanılacak alan ve aranan değer bilgileri parametre olarak verilmekte. Sonrasında DynamoDBContext nesne örneği üzerinden hareket ederek ScanAsync operasyonunu kullanıyor ve içeriğe ulaşmaya çalışıyoruz. Main metodunda bu fonkisyonu kullanarak az önce eklediğimiz 1001 numaralı Quote içeriğine ulaşabiliriz.

```csharp
var utl = new DynamoDBUtility();
var findingQuote=utl.FindQuoteByID(1001);
Console.WriteLine($"{findingQuote.QuoteInfo.Character}\n{findingQuote.QuoteInfo.Text}");
```

![dynamocore_12.gif](images/dynamocore_12.gif)

Siz farklı arama ifadelerini bir araya getirerek değişik denemeler de yapabilirsiniz. Örneğin eklediğiniz n sayıda Quote içerisinden, belli bir oyuna ait olup beğeni değerleri 1000 ve üzerinde olanları çekmeyi deneyebilirsiniz.

Daha neler neler yapılabilir?

Örneğin oluşturduğumuz tabloların silinmesi, alan içeriklerinin güncellenmesi vb standart operasyonları deneyimleyebilirsiniz. Biraz fonksiyonellikleri araştırmanızda yarar var. NuGet paketinde gelen fonksiyonellikler Amazon DynamoDB'nin sunduğu imkanlara göre inşa edilmiş durumdalar. NuGet paketi üzerinden gelen operayonları asenkron olarak çalıştırmayı denemenizi öneririm. Benim örnek tamamen senkron çalışıyor (Rezalet!) Görsel arayüzü olan bir uygulamada awaitable operayonları dikkate alarak arayüzün donmasını engelleyecek şekilde değişiklikler yapmanız yerinde olur. Ah bir de tabii benim gibi God Object anti-pattern'inin yolunu açan, Single Responsibility ilkesini ihlal etmiş kirli bir Utility sınıfı kullanmayın:)

Böylece geldik bir makalemizin daha sonuna. Bu yazımızda Amazon'un NoSQL veritabanlarından olan key-value türevli DynamoDB'sini bir.Net Core uygulamasından nasıl kullanabileceğimizi incelemeye çalıştık. Fiyatlandırma kritlerini göz önüne alarak kendi ürünleriniz için kullanabileceğinizi düşünüyorum. Ben her ihtimale karşı onları sildim:) Geliştirmesi oldukça kolay. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Yazıdaki kodlara github adresimden de erişebilirsiniz.](https://github.com/buraksenyurt/dotnetcore/tree/master/HowToDynamoDb)
