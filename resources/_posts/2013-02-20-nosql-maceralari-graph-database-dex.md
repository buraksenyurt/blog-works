---
layout: post
title: "NoSQL Maceraları - Graph Database DEX"
date: 2013-02-20 04:00:00 +0300
description: "Sparsity firmasının bir ürünü olan DEX, Community kullanımında 1milyon nesneye(Objects) kadar ücretsiz olarak yararlanılabilen bir veritabanı sunmaktadır. Veritabanının en önemli özelliği ise içeriği nesnel olarak Graph teorisine göre tutuyor olmasıdır. (Graph teorisi hakkında Wikipedia bağlantısından özet bir bilgi alabilirsiniz)"
categories:
  - nosql
tags:
  - nosql
  - csharp
  - bash
  - dotnet
  - aspnet
  - sql-server
  - ravendb
  - cassandra
  - wpf
  - http
  - java
---
# NoSQL Maceraları - Graph Database DEX
[![dex_7](/assets/images/2013/dex_7_thumb.png)](/assets/images/2013/dex_7.png) Merhaba Arkadaşlar,

Eminim pek çoğunuzun hastası/fanatiği olduğu yerli veya yabancı diziler vardır. Küçük bir çocukken çizgi filmlere olan düşkünlüğümüz kadar olmasa da, hemen her bölümünü heyecanla beklediklerimiz mutlaka vardır (Hatta ülkemizde geç yayınlanıyor diye ilgili dizileri internetten indirenlerimizde vardır)

Bilişim alanında görev alanların ağırlıkla CNBC-E gibi kanallarda yer alan dizilere olan bağımlılığı da aslında su götürmez bir gerçektir. Örneğin benim fanatiği olduğum dizilerden birisi Dexter ve ne tesadüftür ki bu gün yazımızda ele alacağımız ürünün adı da onun lakabı ile eş: DEX ![Laughing out loud](/assets/images/2013/wlEmoticon-laughingoutloud_4.png)

Daha önceden hatırlayacağınız üzere şuradaki makalede [Apache Cassandra](https://www.buraksenyurt.com/post/Apache-Cassandra-ve-Net)’ yı, oradaki makalede ise [RavedDB](https://www.buraksenyurt.com/post/RavenDB-ile-Hello-World)’ yi incelemeye çalışmıştık. Bu yazımızda ise yine NoSQL veritabanı çeşitlerinden birisi olup Graph teorisini baz alan DEX isimli ürünü incelemeye çalışıyor olacağız.

[Sparsity firmasının bir ürünü olan DEX](http://www.sparsity-technologies.com/dex), Community kullanımında 1milyon nesneye (Objects) kadar ücretsiz olarak yararlanılabilen bir veritabanı sunmaktadır. Veritabanının en önemli özelliği ise içeriği nesnel olarak Graph teorisine göre tutuyor olmasıdır. ([Graph teorisi hakkında Wikipedia](http://en.wikipedia.org/wiki/Graph_theory) bağlantısından özet bir bilgi alabilirsiniz)

Kısaca özetlemek gerekirse Graph veritabanlarında Node, Attribute ve Edge adı verilen nesneler söz konusudur. Her bir Node ve Edge nesnesinin attribute’ lar ile tanımlanabilen özellikleri mevcuttur. Graph veritabanlarında, node’ lar arası ilişkiler Edge örnekleri ile tanımlanmaktadır. Facebook, Twitter, Linkedin gibi popüler sosyal ağların veri ambarlarının tasarlanması noktasında son derece isabetli bir seçimdir. Nitekim node’ lar arası en kısa yolu bulmak veya ilişkileri ortaya çıkarmak, Graph teorisi nedeniyle oldukça kolay, tutarlı ve hızlıdır. Bu sebepten sadece sosyal ağlar da değil IMDB, Wikipedia tarzı oluşumlarda, Lojistik, Telekom ağları gibi daha endüstüriyel çözümlerde de değerlendirilebilmektedir. (Aslına bakarsanız Graph teorisini uygulayabileceğiniz ne kadar veri bazlı çözüm var ise DEX gibi sistemleri göz önüne alabilrsiniz)

DEX veritabanı C++ ile yazılmıştır. Java,.Net, C++, Blueprints Interface API desteği bulunmaktadır. Dolayısıyla pek çok farklı platform tarafından da kullanılabilir bir üründür.

Şimdi dilerseniz fazla vakit kaybetmeden basit bir Hello World uygulaması geliştirmeye çalışalım. Tabi ilk olarak bir senaryoyu göz önüne almamız gerekiyor. Senaryomuza ait basit Graph çizimimiz aşağıdaki gibidir.

[![dex_1](/assets/images/2013/dex_1_thumb.png)](/assets/images/2013/dex_1.png)

Bu şekli biraz inceleyelim

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_170.png)

Basketbol oyuncuları, takım koçları ve takımların yer aldığı bir şema görmekteyiz. Ayrıca bu karakterlerin bazı özellikleri de bulunmaktadır. Örneğin isimler, ülkeler ve benzersiz olmalarını sağlayan sayısal numaralar gibi. Ayrıca bu karakterler arasında belirli bir yöne doğru çizilmiş ilişkiler olduğu görülmektedir. Tüm bunları birleştirdiğimizde şekle bakarak aşağıdaki cümleleri ve benzerlerini sarf edebilmekteyiz.

- Simone Pianigiani koç olarak Fenerbahçe Ülker’ i yönetmekte olup Montepaschi Siena’ yı da önceki bir dönemde çalıştırmıştır.
- Gasper Vidmar şu anda Beşiktaş takımında oynamakta olup daha önceden Fenerbahçe Ülker’ de de oynamıştır.
- Semih Erden şu anda Beşiktaş takımında oynamakta olup, daha önceden de Fenerbahçe Ülker formasını giymiştir.
- Amerikalı Power Forward Marcus Goree, daha önceden Erman Kunter’ in çalıştırdığı Fransız Cholet basketbol takımının formasını giymektedir.

> Örnekler çoğaltılabilir. Aslında Facebook’ de yer alan arkadaşlarınızı, dahil olduğunuz grupları, bu teori ışığında kağıda dökmeyi deneyerek kendi örneğiniz üzerinden de ilerleyebilirsiniz.

Peki bu cümleleri, bir başka deyişle şekilde görülen Graph unsurlarını bilgisayar ortamında nasıl saklayabiliriz?

![I don't know smile](/assets/images/2013/wlEmoticon-idontknowsmile_1.png)

Bu amaçla indirdiğimiz DEX ürününü kullanıyor olacağız. Ağız alışkanlığı nedeniyle bir veritabanı olarak tanımladığımız DEX aslında aşağıda şekilde görülen bir kaç DLL ile birlikte gelmektedir. Yani daha önceden incelediğimiz RavenDb gibi bir Server uygulamasına veya arayüze sahip değildir. Yine de kavramsal olarak tuttuğu içerik bir veri kümesini ifade etmektedir. Daha çok bir API olduğunu ifade edebiliriz. Veriyi disk üzerinde bir dosya şeklinde tutmaktadır.

[![dex_2](/assets/images/2013/dex_2_thumb.png)](/assets/images/2013/dex_2.png)

Dexnet.dll bizim kullanacağımız Wrapper’ dır. Yani projeye referans etmemiz gereken Assembly’ dır.

[![dex_3](/assets/images/2013/dex_3_thumb.png)](/assets/images/2013/dex_3.png)

Ancak bu yeterli değildir. Diğer dll dosyalarının, Dexnet.dll ve uygulamaya ait exe dosyası ile aynı klasör altında bulundurulmaları gerekmektedir. Yani dex.dll, dexnetwrap.dll ve stlport.dll dosyalarının da exe çıktısının olduğu klasöre kopyalanması gerekmektedir.

[![dex_4](/assets/images/2013/dex_4_thumb.png)](/assets/images/2013/dex_4.png)

Bu işlem yapılmadığı takdirde çalışma zamanında Platform Invoke ile ilişkili bir istisna (Exception) alınacaktır.

[![dex_5](/assets/images/2013/dex_5_thumb.png)](/assets/images/2013/dex_5.png)

Bu hazırlıkların ardından örnek kodlarımızı yazmaya başlayabiliriz. Ben uygulamayı bir Console projesi olarak geliştireceğim ve sadece ilk kullanımlarını göstermeye çalışacağım. Bir gerçek hayat senaryosunda şema (Schema) oluşturulması gibi adımların tek seferde yapılmasını garanti etmeye çalışmalısınız. Hatta şemaların kolayca yapılmasını sağlamak amacıyla ayrı bir arabirim dahi geliştirilebilir (SQL Server Management Studio tarzı bir şey olmasa da işe yarar bir arayüz pekala çok isabetli bir tercih olabilir)

İlk olarak veritabanını ve Graph nesnelerine ait şemaları tanımlayacağımız kodları yazarak işe başlayabiliriz.

```csharp
Database database = null; 
DexConfig cfg = new DexConfig(); 
cfg.SetLogFile("Azon.log"); 
cfg.SetCacheMaxSize(1024); 
//cfg.SetLicense("lisans numarası"); 
Dex dexter = new Dex(cfg); 
database = dexter.Create("AzonGraphDb.dex", "Azon");
```

Yukarıdaki kod parçasında bir veritabanının oluşturulma adımları örneklenmektedir. Önemli olan noktalardan birisi Dex tipini örneklerken bazı konfigurasyon ayarları için DexConfig sınıfından yararlanılmasıdır. Örneğin ürünün lisanslı olan bir sürümü alınırsa bunun SetLicence metodu ile bildirilmesi gerekecektir. Veritabanı nesne örneği oluşturulduktan sonra Node, Attribute ve Edge tanımlalarını yaparak şemayı da oluşturabiliriz.

```csharp
Session session= database.NewSession(); 
Graph g = session.GetGraph();

#endregion

#region Schema' nın Oluşturulması

int teamType = g.NewNodeType("Takim"); 
int teamIdType = g.NewAttribute(teamType, "TakimId", DataType.Long, AttributeKind.Unique); 
int teamNameType = g.NewAttribute(teamType, "Ad", DataType.String, AttributeKind.Indexed); 
int teamCountryType = g.NewAttribute(teamType, "Ulke", DataType.String, AttributeKind.Indexed);

int staffType = g.NewNodeType("Eleman"); 
int staffIdType = g.NewAttribute(staffType, "ElemanId", DataType.Long, AttributeKind.Unique); 
int staffNameType = g.NewAttribute(staffType, "Ad", DataType.String, AttributeKind.Indexed); 
int staffTitleType = g.NewAttribute(staffType, "Unvan", DataType.String, AttributeKind.Indexed); 
int staffCountryType = g.NewAttribute(staffType, "Ulke", DataType.String, AttributeKind.Indexed);

#endregion Schema' nın Oluşturulması

#region Edge Tanımlamaları

// Bu senaryoda sadece Directed Edge kullanılmıştır. Normalde hem Tail hem de Head rolü üstlenen bir node söz konusu ise UnDirected Edge kullanılması gerekir. 
// Directed Edge' ler de bir Tail' den Head' e doğru giden bir ilişki ifade edilir

int roleType = g.NewEdgeType("Rol", false, false); 
int roleTitleType = g.NewAttribute(roleType, "Title", DataType.String, AttributeKind.Basic);

#endregion Edge Tanımlamaları
```

Şema oluşturma işlemlerinde olayın kahramanı Graph tipinden olan nesne örneğidir. Bir Graph nesnesini örneklemek için veritabanına ait oturumdan yararlanılmalıdır. Bu sebepten bir Session örneği oluşturulmuştur. Session açık olduğu süre zarfında Graph örneği üzerinden yapılan tüm işlemler (şema oluşturma, veri ekleme, güncelleme vb) bu oturuma ait olacak şekilde gerçekleşecektir.

Dikkat edileceği üzere takımlar ve oyuncular ile antrenörleri ifade eden elemanlar için birer Node üretilmiştir. Her Node’ un kendine has bir takım nitelikleri vardır. Örneğin her elemanın bir adı, uyruğu ve ünvanı gibi. Bir Node üretilirken bir veri tipinin belirtildiğine ve AttributeKind ile şekillendirildiğine de dikkat edelim. Indexed olarak işaretlenenler sorgulanabilir olduklarını göstermektedir. Unique, tahmin edileceği üzere ilgili niteliğin değerinin benzersiz olmasını sağlamaktadır. Hatta ElemanId ve TakimId bu anlamda Primary Key olmuşlardır. AttributeKind ile belirtilebilecek bir diğer değer de Basic’ tir. Basic tipindeki nitelikler sorgulamalarda (Query) kullanılamazlar.

Node tanımlalarının ardından dikkat edileceği üzere bir Edge örneği de üretilmiş ve bu kendisine bir nitelik de eklenmiştir. Bir tane Edge şu andaki senaryomuz için yeterlidir. Bu Edge’ in ilgili niteliğinden yararlanarak Oynuyor, Oynadı, Yönetti, Yönetiyor şeklindeki ilişkileri tesis edebiliriz. Pek tabiki bir gerçek hayat senaryosunda bu tip sabit değerleri bir Enum tipi içerisinde toplamak daha doğru bir yaklaşım olabilir.

DEX, Edge tanımlamalarını iki şekilde değerlendirmektedir. Directed ve Undirected. Directed ilişkisinde bir kaynak Node (Tail olarak adlandırılmakta) ve bir de hedef Node vardır (Head olarak adlandırılmaktadır). Undirected ilişkilerde de ise Node'lar hem Tail hem de Head rolündedir. Örneğimizde Undirected ilişki senaryosu ele alınmamıştır. Ancak DEX’ e ait teknik dökümantasyonda örnek bir kullanımı mevcuttur.

Artık şema tanımlamalarımızı yaptığımıza göre örnek verilerin eklenmesi işlemini gerçekleştirebiliriz. Yapacağımız veri eklemeleri ile temel hedefimiz Graph görselindeki ilişkileri ve değerleri üretmektir. İşte kodlarımız.

```bash
#region Örnek Veri Eklenmesi

Value value = new Value();

long marcusGoree = g.NewNode(staffType); 
g.SetAttribute(marcusGoree, staffIdType, value.SetLong(1)); 
g.SetAttribute(marcusGoree, staffNameType, value.SetString("Marcus Goree")); 
g.SetAttribute(marcusGoree, staffTitleType, value.SetString("Power Forward")); 
g.SetAttribute(marcusGoree, staffCountryType, value.SetString("USA"));

long rudyGobert = g.NewNode(staffType); 
g.SetAttribute(rudyGobert, staffIdType, value.SetLong(2)); 
g.SetAttribute(rudyGobert, staffNameType, value.SetString("Rudy Gobert")); 
g.SetAttribute(rudyGobert, staffTitleType, value.SetString("Power Forward")); 
g.SetAttribute(rudyGobert, staffCountryType, value.SetString("FR"));

long gasperVidmar = g.NewNode(staffType); 
g.SetAttribute(gasperVidmar, staffIdType, value.SetLong(3)); 
g.SetAttribute(gasperVidmar, staffNameType, value.SetString("Gasper Vidmar")); 
g.SetAttribute(gasperVidmar, staffTitleType, value.SetString("Center")); 
g.SetAttribute(gasperVidmar, staffCountryType, value.SetString("SL"));

long semihErden = g.NewNode(staffType); 
g.SetAttribute(semihErden, staffIdType, value.SetLong(4)); 
g.SetAttribute(semihErden, staffNameType, value.SetString("Semih Erden")); 
g.SetAttribute(semihErden, staffTitleType, value.SetString("Center")); 
g.SetAttribute(semihErden, staffCountryType, value.SetString("TR"));

long oguzSavas = g.NewNode(staffType); 
g.SetAttribute(oguzSavas, staffIdType, value.SetLong(5)); 
g.SetAttribute(oguzSavas, staffNameType, value.SetString("Oğuz Savaş")); 
g.SetAttribute(oguzSavas, staffTitleType, value.SetString("Center")); 
g.SetAttribute(oguzSavas, staffCountryType, value.SetString("TR"));

long simonePianigiani = g.NewNode(staffType); 
g.SetAttribute(simonePianigiani, staffIdType, value.SetLong(6)); 
g.SetAttribute(simonePianigiani, staffNameType, value.SetString("Simone Pianigiani")); 
g.SetAttribute(simonePianigiani, staffTitleType, value.SetString("Coach")); 
g.SetAttribute(simonePianigiani, staffCountryType, value.SetString("IT"));

long ermanKunter = g.NewNode(staffType); 
g.SetAttribute(simonePianigiani, staffIdType, value.SetLong(7)); 
g.SetAttribute(simonePianigiani, staffNameType, value.SetString("Erman Kunter")); 
g.SetAttribute(simonePianigiani, staffTitleType, value.SetString("Coach")); 
g.SetAttribute(simonePianigiani, staffCountryType, value.SetString("IT"));

long besiktas = g.NewNode(teamType); 
g.SetAttribute(besiktas, teamIdType, value.SetLong(8)); 
g.SetAttribute(besiktas, teamNameType, value.SetString("Beşiktaş")); 
g.SetAttribute(besiktas, teamCountryType, value.SetString("TR"));

long cholet = g.NewNode(teamType); 
g.SetAttribute(cholet, teamIdType, value.SetLong(9)); 
g.SetAttribute(cholet, teamNameType, value.SetString("Cholet")); 
g.SetAttribute(cholet, teamCountryType, value.SetString("FR"));

long fenerbahceUlker = g.NewNode(teamType); 
g.SetAttribute(fenerbahceUlker, teamIdType, value.SetLong(10)); 
g.SetAttribute(fenerbahceUlker, teamNameType, value.SetString("Fenerbahçe Ülker")); 
g.SetAttribute(fenerbahceUlker, teamCountryType, value.SetString("TR"));

long montepaschiSiena = g.NewNode(teamType); 
g.SetAttribute(montepaschiSiena, teamIdType, value.SetLong(11)); 
g.SetAttribute(montepaschiSiena, teamNameType, value.SetString("Montepaschi Siena")); 
g.SetAttribute(montepaschiSiena, teamCountryType, value.SetString("IT"));

long edge; 
edge = g.NewEdge(roleType,marcusGoree,cholet); 
g.SetAttribute(edge, roleTitleType, value.SetString("Oynuyor"));

edge = g.NewEdge(roleType, rudyGobert, cholet); 
g.SetAttribute(edge, roleTitleType, value.SetString("Oynuyor"));

edge = g.NewEdge(roleType, ermanKunter, cholet); 
g.SetAttribute(edge, roleTitleType, value.SetString("Yönetti"));

edge = g.NewEdge(roleType, ermanKunter, besiktas); 
g.SetAttribute(edge, roleTitleType, value.SetString("Yönetiyor"));

edge = g.NewEdge(roleType, gasperVidmar, besiktas); 
g.SetAttribute(edge, roleTitleType, value.SetString("Oynuyor"));

edge = g.NewEdge(roleType, gasperVidmar, fenerbahceUlker); 
g.SetAttribute(edge, roleTitleType, value.SetString("Oynadı"));

edge = g.NewEdge(roleType, semihErden, besiktas); 
g.SetAttribute(edge, roleTitleType, value.SetString("Oynuyor"));

edge = g.NewEdge(roleType, semihErden, fenerbahceUlker); 
g.SetAttribute(edge, roleTitleType, value.SetString("Oynadı"));

edge = g.NewEdge(roleType, oguzSavas, fenerbahceUlker); 
g.SetAttribute(edge, roleTitleType, value.SetString("Oynuyor"));

edge = g.NewEdge(roleType, simonePianigiani, fenerbahceUlker); 
g.SetAttribute(edge, roleTitleType, value.SetString("Yönetiyor"));

edge = g.NewEdge(roleType, simonePianigiani, montepaschiSiena); 
g.SetAttribute(edge, roleTitleType, value.SetString("Yönetti"));

#endregion Örnek Veri Eklenmesi
```

Kod satırlarının uzun görünmesine aldırış etmeyin. Temel olarak icar ettirdiğimiz iki fonksiyonel akış söz konusudur. Node ve Edge oluşturmak. Bir Node üretilirken hangi tipten olduğu belirtilir. Ardından elde edilen nesne örneği için söz konusu tipin içerisinde tanımlanan niteliklere (Attribute) değer atamaları gerçekleştirilir.

Edge örnekleri oluşturulurken de ilk olarak Edge tipi belirtilmektedir. Tip belirtildikten sonra ise yine Node oluşturulmasına benzer olacak şekilde nitelik değerlerinin verilmesi söz konusudur. Her iki kullanımda da değerlerin atanması için Value tipinden ve ilgili Set fonksiyonundan yararlanılmaktadır. Örneğin long tipinden olan ElemanId için value nesne örneğinin SetLong metodundan yararlanılırken, string tipte olan takım adları için SetString fonksiyonu kullanılmaktadır. Tanımlanan her Edge ile Graph görselinde yer alan ilişkilerin tanımlandığına dikkat edilmelidir.

> Siz tabiki makaleyi okuyup kullanım tekniklerini öğrendikten sonra şöyle güzel janjanlı WPF/Asp.Net ekranları hazırlayarak bu işi daha zevkli hale getirebilirsiniz
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_170.png)

Veri ekleme işlemlerini tamamladığımıza göre basit bir arama işlemi ile devam edebiliriz. Örneğin Semih Erden’ in bağlı olduğu boğumları bulalım.

```csharp
Objects trace = g.Neighbors(semihErden, roleType, EdgesDirection.Outgoing);

ObjectsIterator iterator = trace.Iterator();

Value nameValue = new Value(); 
Value countryValue = new Value(); 
Console.WriteLine("Semih Erden bağlantıları\n");

while (iterator.HasNext()) 
{ 
    long objectId = iterator.Next(); 
   g.GetAttribute(objectId, teamNameType, nameValue); 
    g.GetAttribute(objectId, teamCountryType, countryValue);

    Console.WriteLine("Takım {0}, Ülke {1}", 
        nameValue.GetString(), 
        countryValue.GetString()); 
}
```

Yine Graph nesne örneğinden yararlanılmaktadır. İlk olarak Neighbors metodu ile semihErden örneğinin roleType’ a göre dışarıya doğru olan komşularına gidilmektedir. roleType bildiğiniz üzere bir Edge örneğidir. Tabi n sayıda sonuç dönebileceğinden ileri yönlü bir iterasyona ihtiyaç vardır. Bu sebepten ObjectsIterator tipinden bir nesne örneklenmiş ve while döngüsüne başvurulmuştur. HasNext’ in true döndürdüğü sürece devam eden döngü içerisinde ise GetAttribute metodundan yararlanılarak elde edilen Node’ un bazı değerleri okunmaktadır. Takım adı ve bulunduğu ülke.

Peki iki Node arasındaki Edge örneğini nasıl yakalayabiliriz?

![Who me?](/assets/images/2013/wlEmoticon-whome_4.png)

Bunun için örnek bir kullanım aşağıdaki kod parçasında görüldüğü gibidir.

```bash
#region Edge değeri okumak

Value roleTitleValue = new Value();

Console.WriteLine("\nÇeşitli Edge değerleri\n");

long edgeId=g.FindEdge(roleType, semihErden, besiktas);            
g.GetAttribute(edgeId, roleTitleType, roleTitleValue); 
Console.WriteLine("Semih Erden - ({0}) -> Beşiktaş",roleTitleValue );

edgeId = g.FindEdge(roleType, simonePianigiani, montepaschiSiena); 
g.GetAttribute(edgeId, roleTitleType, roleTitleValue); 
Console.WriteLine("Simone Pianigiani - ({0}) -> Montepaschi Siena", roleTitleValue);

edgeId = g.FindEdge(roleType, oguzSavas, fenerbahceUlker); 
g.GetAttribute(edgeId, roleTitleType, roleTitleValue); 
Console.WriteLine("Oguz Savas - ({0}) -> Fenerbahçe Ülker", roleTitleValue);

#endregion Edge değeri okumak
```

İlk sorguda, FindEdge metodu ile semihErden ve besiktas isimli Node örnekleri arasında yer alan roleType tipinden olan Edge örneğinin nesne numarası alınmaktadır. Bu nesne numarası GetAttribute metodunda kullanılır ve roleTitleType tipinden olan niteliğin taşıdığı değer yakalanır. Bu senaryo takip eden sorgulamalarda simonePianigiani ve montepaschiSiena ile oguzSavas ve fenerbahceUlker için de yapılmıştır.

Yapılan tüm bu işlemler sonrasında ise Database, Graph, Objects ve ObjectsIterator gibi nesne örneklerinin kapatılması gerekmektedir. Yani bu nesne örneklerine ait Close metodlarına çağrıda bulunulmalıdır.

```csharp
iterator.Close(); 
trace.Close(); 
session.Close();            
database.Close();
```

Uygulamanın çalışma zamanı çıktısına baktığımızda aşağıdaki ekran görüntüsünde yer alan sonuçlar ile karşılaşırız.

[![dex_6](/assets/images/2013/dex_6_thumb.png)](/assets/images/2013/dex_6.png)

Görüldüğü üzere Graph teorisine bağlı kalaraktan, DEX API’ sinden de yararlanarak tüm Euroelague takımları ve oyuncuları için (hatta bunların içerisine başka nesneleri de katabiliriz) kocaman bir veri içeriğini oluşturmamız mümkündür. Tabi böyle bir içerik kuvvetle muhtemel 1milyon nesneyi aşabilir ve dolayısıyla lisans satın alınması gerekebilir. DEX gibi başka pek çok Graph veritabanı mevcuttur. Örneğin Trinity, BigData vb…Bunları da fırsatım olursa incelemeye çalışıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_170.png)

[HowTo_DEX.zip (785,18 kb)](/assets/files/2013/HowTo_DEX.zip)