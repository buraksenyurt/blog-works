---
layout: post
title: "Sunucu Bazlı Blazor Uygulaması ve Firestore Kullanımı"
date: 2019-07-05 10:20:00 +0300
description: "Blazor client-side web framework olarak düşünülebilir(Component ve DOM etkileşiminin aynı process içerisinde gerçekleşmesi) ancak process'lerin ayrılması konusunda da esnektir. Öyle ki, Blazor'u bir Web Worker içinde çalıştırıp UI thread'inden ayrıştırılabileceği ifade edilmektedir. Diğer yandan 0.5 sürümü ile birlikte Blazor uygulamalarının sunucu tarafında çalıştırılması mümkün hale gelmiştir. Yani .Net Core ile etkileşimde olacak şekilde Blazor bileşenlerini(component) sunucu tarafında çalıştırabiliriz. Burada .Net tarafı WebAssembly yerine CoreCLR üzerinde koşar ve .NET ekosisteminin tüm nimetlerinden(JIT, debugging vb) yararlanabilir. UI tarafı ile etkileşimde olayların ele alınması ve Javascript Interop çağrıları için SignalR bağlantılarından faydalanılır."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - aspnet
  - linq
  - json
  - web-api
  - http
  - javascript
  - vue
  - blazor
  - async-await
  - threading
  - serialization
  - generics
  - debugging
  - visual-studio
  - github
---
# Sunucu Bazlı Blazor Uygulaması ve Firestore Kullanımı
![simpscss.png](/assets/images/2019/simpscss.png)

Mavi renkli teknoloji firmasına henüz yeni başlamıştım. Yaş ve önceki dönem tecrübeleri nedeniyle standart olarak uygulanan oryantasyon hızlıca atlanmış ve 2002 yılında geliştirilmeye başlanmış Web Forms kurugulu ERP uygulamasından ilk görevimi almıştım. Henüz çevik dönüşüme başlanmamıştı. Elimde tek sayfalık bir analiz dokümanı bulunuyordu. Otomotiv tarafındaki iş bilgim az olduğundan dokümanda yer almayan şeyler hakkında pek bir fikrim yoktu. Görevim kağıt üstünde oldukça basitti. Popup pencere açtırıp içerisinde bir araca ait veriler gösterecektim. Ne kadar zor olabilirdi ki:))

İlk haftanın sonunda popup açılıyor ancak engin front-end bilgim nedeniyle ekranın üstündeki nesnelerin hiç biri olması gerektiği yerde durmuyordu. Back-end servisinin kodlanması, Data Access Layer tarafı, veri tabanı nesneleri...Hepsini kısa sürede halledebilmiştim ama işte o önyüz tarafı yok mu? O görselliği arttıran CSS ayarlamaları yok mu? (CSS demişken yandaki Simpson karakterlerinin Chris Pattle tarafından nasıl yazıldığına bir bakmak ister misiniz? [Şöyle buyrun öyleyse](https://pattle.github.io/simpsons-in-css/))

Sıfırdan bir şeyler yazacak olsam hiç zorlanmayacaktım belki ama yaşayan, belli kuralları ve sırları bulunan bu ürün içinde epey mücadele vermiştim. Bir tam gün boyunca ekrandaki o düğmenin olması gerektiği yere gelmesi için Chrome DevTools'ta gözlerimi kanatacak kadar uğraştığımı hatırlıyorum. İç içe gelen master page'lerin, sayısız div'in arasında bir o yana bir bu yana savrulup durmaktaydım.

Tabii zaman hızla aktı. Aradan aylar geçti. Hem bu yaşlı ürüne hem yeni nesil programlara aşina oldukça gereksinimleri bir öncekine göre daha hızlı karşılayabildiğimi fark ettim. Ne varki yeni nesil ürünlerde de en çok zorlandığım şey işte bu popup pencereleriydi. Vue tabanlı olanda olsun Angular tabanlı olan da olsun gözümün önünde yapılmış örnekleri bile varken onlara bakmadan gelişitirmekte halen zorlanmaktayım. Web API tarafı tamam, veri tabanı nesneleri tamam, aradaki iletişim için gerekli köprüleri kurmak tamam...Ama işte o önyüz yok mu? Bence beynimdeki front-end hücreleri tamamen yanmış:D Sıradaki [cumartesi gecesi derlememiz](https://github.com/buraksenyurt/saturday-night-works)de de bir Popup var ama bu kez çok fazla zorlanmadım diyebilirim. Nitekim odağımız Blazor kıyıları olacak.

Blazor çoğunlukla client-side web framework olarak düşünülmekte. Bu kabaca, Component ve DOM etkileşiminin aynı process içerisinde olması anlamına geliyor ancak process'lerin ayrılması konusunda esnek bir çatı. Öyle ki Blazor'un bir Web Worker içinde çalıştırılıp UI (User Interface) thread'inden ayrıştırılabileceği ifade edilmekte. Diğer yandan 0.5 sürümü ile birlikte Blazor uygulamalarının sunucu tarafında çalıştırılması mümkün hale gelmiş. Yani.Net Core ile etkileşimde olacak şekilde Blazor bileşenlerini sunucu tarafında çalıştırabiliriz. Bu senaryoda.Net tarafı WebAssembly yerine CoreCLR üzerinde koşmakta ve.NET ekosisteminin pek çok nimetinden (JIT, debugging vb) yararlanabilmekte. Kullanıcı önyüz tarafı ile etkileşimde olayların yakalanması ve Javascript Interop çağrıları içinse SignalR ele alınmakta. Aşağıdaki kötü çizim konuyla ilgili olarak size bir parça daha fikir verebilir.

![09_35_credit_10.png](/assets/images/2019/09_35_credit_10.png)

Benim bu çalışmadaki amacım Server Side tipinden Blazor uygulamalarının Ubuntu gibi bir platformda nasıl geliştirilebileceğini öğrenmek ve bunu yaparken de Google Cloud Firestore'u kullanarak basit CRUD (Create Read Update Delete) operasyonları içeren bir ürün tasarlamaktı. Araştırmalarıma göre Server Side Blazor modelinin belli başlı avantajları bulunuyor. Bunları şöyle sıralayabiliriz.

- Uygulamanın indirme boyutu nispeten küçülür
- Blazor bileşenleri (component).Net Core uyumlu sunucu kabiliyetlerinin tamamını kullanabilir
- Debugging ve JIT Compilation imkanlarına sahip olunur
- Server-Side Blazor tarafı Mono WebAssembly yerine.Net Core process'i içinde çalışır ve WebAssembly desteği olmayan tarayıcılar için de bir açık kapı bırakır
- UI tarafının güncellemeleri SignalR ile gerçekleşir ve gereksiz sayfa yenilemeleri olmaz

Belki dezavantaj olarak arayüz etkileşimi için SignalR kullanılmasının ağ üzerinde ekstra hareketlilik anlamına geleceğini belirtebiliriz. Bu bilgilerden sonra gelin örneğimizi geliştirmeye başlayım.

## Ön Gereksinimler

Visual Studio Code'un olduğu WestWorld'de (Ubuntu 18.04,64bit) Visual Studio 2017/2019 nimetleri olmasa da Server Side Blazor uygulamaları geliştirebiliyorum. Sizin için de geçerli bir durum. Bunun için terminalden aşağıdaki komutu vermek yeterli.

```bash
sudo dotnet new --install "Microsoft.AspNetCore.Blazor.Templates"
dotnet new --help
```

![09_35_credit_1.png](/assets/images/2019/09_35_credit_1.png)

Görüldüğü gibi dotnet aracının new şablonlarına Blazor eklentileri gelmiş durumda.

## Cloud Firestore Tarafının Hazırlanması

Kod tarafına geçmeden önce Google Cloud Platform üzerindeki veri tabanı hazırlıklarımızı gerçekleştirelim. Önce [Firebase Console'a gidelim](https://console.firebase.google.com/) ve yeni bir proje oluşturalım. Ben aşağıdaki özelliklere sahip enbiey (NBA) isimli bir proje oluşturdum.

![09_35_credit_2.png](/assets/images/2019/09_35_credit_2.png)

Ardından database sekmesinden Create Database seçeneği ile ilerleyip Security rules for Cloud Firestore penceresindeki Start in locked mode seçeneğini işaretli bırakalım.

![09_35_credit_3.png](/assets/images/2019/09_35_credit_3.png)

Varsayılan olarak Cloud Firestore tipinden bir veri tabanı oluşturacağız (Realtime Database tipini de kullanabilirsiniz) Sonrasında bir koleksiyon (collection) ve örnek doküman (document) ile ilk veri girişimizi yapabiliriz. Söz gelimi players isimli koleksiyonu açıp,

![09_35_credit_4.png](/assets/images/2019/09_35_credit_4.png)

içine fullname, length, position ve someinfo alanlarından oluşan örnek bir oyuncuyu ekleyebiliriz.

![09_35_credit_5.png](/assets/images/2019/09_35_credit_5.png)

Sonuçta aşağıdakine benzer bir dokümanımızın olması gerekiyor.

![09_35_credit_6.png](/assets/images/2019/09_35_credit_6.png)

Yazılacak Blazor uygulamasının (başka uygulamalar içinde benzer durum söz konusu aslında) Firestore veri tabanını kullanabilmesi için Credential ayarlamalarını da yapmalıyız. Yeni açılan projenin Service Account'u için bir key dosyası üretmemiz lazım. Öncelikle [Google IAM adresine](https://console.cloud.google.com/iam-admin/) gidip projemizi seçelim ve ardından istediğimiz service account'u işaretleyip üç nokta düğmesini kullanarak Create Key tuşuna basalım.

![09_35_credit_7.png](/assets/images/2019/09_35_credit_7.png)

Gelen penceredeki varsayılan JSON seçimini olduğu gibi bırakalım.

![09_35_credit_8.png](/assets/images/2019/09_35_credit_8.png)

![09_35_credit_9.png](/assets/images/2019/09_35_credit_9.png)

İndirilen JSON uzantılı dosya içeriği Blazor uygulaması için gerekli olacak, unutmayın.

## Server Side Blazor Uygulamasının İnşası

Veri tabanı hazırlıklarımız tamam. Artık Blazor uygulamasının omurgasını hazırlayıp gerekli kodları yazmaya geçebiliriz. Terminalden aşağıdaki komutu vererek Hosted in ASP.NET Server tipindeki blazor çözümünü inşa ederek işlemlerimize devam edelim.

```bash
dotnet new blazorhosted -o NBAWorld
```

Komutun çalışması sonrası üç adet proje oluşur. Shared kütüphanesi, Client ve Server projeleri tarafından ortaklaşa kullanılmaktadır. Client projesi Server tarafına da referans edilmiştir (csproj dosyalarını kontrol ediniz) ve tarayıcıda gösterilecek bileşenleri içerir. Firestore'a erişeceğimiz API Controller tarafı Server projesinde bulunur. Yani back-end'in server projesi olduğunu düşünebiliriz. Model sınıfları gibi hem istemci hem sunucu projelerince paylaşılacak tiplerse Shared uygulamasında bulunur. Shared ve Server projeleri Google Cloud Firestore ile çalışacaklar. Bu nedenle her iki projeye de Google.Cloud.Firestore nuget paketini eklememiz gerekiyor. Bunu aşağıdaki terminal komutu ile ilgili uygulama klasörlerinde yapabiliriz.

```bash
dotnet add package Google.Cloud.Firestore --version 1.0.0-beta19
```

> Örneği çalıştığım tarihte bu paket sürümü mevcuttu. Siz denerken güncel versiyona bir bakın.

## Kodlayalım

Artık kodlarımızı geliştirmeye başlayabiliriz. NBAWorld.Shared isimli projede Models klasörü açıp içine aşağıda kodları yer alan Player sınıfını ekleyelim.

```csharp
using System;
using Google.Cloud.Firestore;

/*
    Firestore tarafındaki players koleksiyonundaki her bir 
    dokümanın kod tarafındaki karşılığını ifade eden sınıfımız

    Koleksiyon eşleştirmesi için FirestoreData kullanıldı.
    Sadece FirestoreProperty niteliği ile işaretlenen özellikler
    Firestore tarafında işleme alınır.
 */
namespace NBAWorld.Shared.Models
{
    [FirestoreData]
    public class Player{
        public string DocumentId{get;set;}
        [FirestoreProperty]
        public string Fullname { get; set; }
        [FirestoreProperty]
        public string Length { get; set; }
        [FirestoreProperty]
        public string Position { get; set; }
        [FirestoreProperty]
        public string SomeInfo { get; set; }

    }
}
```

NBAWorld.Server projesinde de Data isimli bir klasör açıp Firestore tarafındaki players koleksiyonu için Data Access Layer görevini üstlenecek PlayerDAL sınıfını ekleyelim.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBAWorld.Shared.Models;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace NBAWorld.Server.Data
{
    /*
    Google Cloud Firestore ile iletişimde kullanılan
    Data Access Layer sınıfı.
     */
    public class PlayerDAL
    {
        string projecId = "enbiey-94b53"; // Firebase proje id
        FirestoreDb db;

        /*
            Firestore veri tabanı nesnesini, proje id ve credential 
            bilgileri ile üretmek için sınıfın yapıcı metodu oldukça
            uygun bir yer.
         */
        public PlayerDAL()
        {
            // Client iletişimi için gerekli Credential bilgisini taşıyan dosya. Firebase'den indirmiştik hatırlayın.
            // Siz tabii dosyayı hangi adrese koyduysanız orayı ele almalısınız
            string credentialFile = "/home/burakselyum/enbiey.json";
            // Environment parametrelerine GOOGLE_APPLICATION_CREDENTIALS bilgisini ekliyoruz
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialFile);
            // FirebaseDb nesnesini projeId ile oluşturuyoruz
            db = FirestoreDb.Create(projecId);
        }

        /*
        Oyuncu listesini getirecek olan metot
         */
        public async Task<List<Player>> GetPlayers()
        {
            // Koleksiyon için sorguyu hazırlıyoruz
            Query selectAll = db.Collection("players");
            // Snapshot nedir?
            QuerySnapshot selectAllSnapshot = await selectAll.GetSnapshotAsync();
            var players = new List<Player>();

            // Tüm dokümanları dolaşıyoruz
            foreach (var doc in selectAllSnapshot.Documents)
            {
                // Eğer doküman varsa
                if (doc.Exists)
                {
                    // koleksiyondaki dokümanı bir dictionary'ye al
                    Dictionary<string, object> playerDoc = doc.ToDictionary();
                    // json formatında serialize et
                    string json = JsonConvert.SerializeObject(playerDoc);
                    // gelen JSON içeriğini player örneğine çevir
                    Player player = JsonConvert.DeserializeObject<Player>(json);
                    player.DocumentId = doc.Id; //Delete ve Update işlemlerinde Firestore tarafındaki Document ID değerine ihtiyacımız olacak
                    // List koleksiyonuna ekle
                    players.Add(player);
                }
            }

            // Listeyi döndür
            return players;
        }

        /*
        Firestore'a doküman olarak yeni bir oyuncu ekleyen fonksiyonumuz
         */
        public async void NewPlayer(Player player)
        {
            // players koleksiyonuna ait referansı al
            CollectionReference collRef = db.Collection("players");
            // awaitable AddAsync metodu ile ekle
            await collRef.AddAsync(player);
        }

        /*
        Firestore'dan doküman silme işlemini üstlenen metodumuz
         */
        public async void DeletePlayer(string documentId)
        {
            // documentId bilgisini kullanarak players koleksiyonda ilgili dokümanı bul
            DocumentReference document = db.Collection("players").Document(documentId);
            // bulunan dokümanı sil
            if (document != null)
            {
                await document.DeleteAsync();
            }
        }

        /*
        Firestore'dan bir dokümanı güncellemek için kullanılan metodumuz
         */
        public async void UpdatePlayer(Player player)
        {
            // Önce parametre olarak gelen oyuncunun referansını bulmaya çalış
            DocumentReference document = db.Collection("players").Document(player.DocumentId);
            if (document != null) //eğer bulduysan
            {
                // Overwite seçeneği ile üstüne yaz
                await document.SetAsync(player, SetOptions.Overwrite);
            }
        }

        /*
        Tek bir oyuncu bilgisini dokümand ıd değerine göre çeken fonksiyonumuz
         */
        public async Task<Player> GetPlayerById(string documentId)
        {
            // Doküman referansını bulup
            DocumentReference document = db.Collection("players").Document(documentId);
            // bir görüntüsünü çekiyoruz
            DocumentSnapshot snapshot = await document.GetSnapshotAsync();
            Player player = new Player();

            if (snapshot.Exists) // Eğer snapshot içeriği mevcutsa
            {
                player.DocumentId = snapshot.Id;
                // oyuncu bilgilerini dokümandan GetValue ile alıyoruz
                player.Fullname=snapshot.GetValue<string>("Fullname");
                player.Position=snapshot.GetValue<string>("Position");
                player.SomeInfo=snapshot.GetValue<string>("SomeInfo");
                player.Length=snapshot.GetValue<string>("Length");
            }

            return player;
        }
    }
}
```

Ardından Controller klasörüne API Controller görevini üstlenen PlayersController isimli aşağıdaki bileşeni ilave ederek devam edelim.

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NBAWorld.Server.Data;
using NBAWorld.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace NBAWorld.Server.Controllers
{
    /*
        İstemci tarafına CRUD operasyon desteği sunacak olan API servisimiz.
     */
    [Route("api/[controller]")]
    public class PlayersController
        : Controller
    {
        PlayerDAL playerDAL = new PlayerDAL();

        // Tüm oyuncu listesini döndüren HTTP Get metodumuz
        [HttpGet]
        public Task<List<Player>> Get()
        {
            return playerDAL.GetPlayers();
        }

        /*
        HTTP Post çağrısı ile yeni bir oyuncuyu Firestore'a eklemek için kullandığımız servis metodu.
        Mesaj gövdesinden JSON formatında gelen oyuncu içeriğini kullanır.
        DAL'daki ilgili metodu çağırır. Firestore'a asıl ekleme işini PlayerDAL içindeki metod gerçekleştirir.
         */
        [HttpPost]
        public void Post([FromBody]Player player)
        {
            playerDAL.NewPlayer(player);
        }

        /*
        Silme işlemini üstlenen metodumuz.
        Querystring ile gelen id değerini kullanır.
        Data Access Layer nesnesindeki DeletePlayer metodunu çağırır.
         */
        [HttpDelete("{documentId}")]
        public void Delete(string documentId)
        {
            playerDAL.DeletePlayer(documentId);
        }

        /*
        Güncelleme işlemini üstlenen API metodumuz.
        HTTP Put ile çalışır.
        Request Body ile gelen içerik kullanılır.
         */
        [HttpPut]
        public void Upate([FromBody]Player player)
        {
            playerDAL.UpdatePlayer(player);
        }

        /*
        Tek bir dokümanı almak için kullanılan metodumuz.
        Bunu var olan oyuncu bilgilerini güncelleme akışında kullanıyoruz.
         */
        [HttpGet("{documentId}")]
        public Task<Player> Get(string documentId)
        {
            return playerDAL.GetPlayerById(documentId);
        }
    }
}
```

Gelelim istemci rolünü üstlenen NBAWorld.Client projesine. Öncelikle proje oluşturulduğunda varsayılan olarak gelen bazı dosyalar (Counter, Fetch Data vb) göreceksiniz. Bunlara ihtiyacımız olmadığından silebiliriz. Projenin Pages klasörüne PlayerData (Tüm oyuncuları gösteren bileşenimiz) ve NewPlayer (Yeni oyuncu ekleme işini üstlenen bileşenimiz) isimli razor sayfalarını ekleyeceğiz. İçeriklerini aşağıdaki gibi geliştirebiliriz.

PlayerData.cshtml

```text
<!--
    Razor sayfamızın adı playerspage. Navigasyonda bu ismi kullanıyoruz.
    Kullandığı model PlayerDataModel isimli BlazorComponent türevli bileşen.
    playerList, component sınıfı içerisindeki bir özellik.
-->

@page "/playerspage"
@inherits PlayerDataModel

<h1>Efsane Oyuncularımın Listesi</h1>

@if (playerList == null)
{
    <p><em>Yükleniyor...</em></p>
}
else
{
<table class='table'>
        <thead class="thead-dark">
            <tr>
                <th>Adı</th>
                <th>Boyu</th>
                <th>Mevkisi</th>
                <th>Hakkında</th>
                <th></th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            <!--
                Eğer playerList hazırsa tüm içeriğini dolaşıyoruz.
                Ve özelliklerini TD hücrelerine yazdırıyoruz.
                Sağ tarafa yer alan ve silme işlemini üstlenen bir button kontrolü var.
                onclick olay metodunda bileşendeki DeletePlayer fonksiyonu çağırılıyor ve
                döngü ile kontroller bağlanırken güncel p değişkeninin sahip olduğu
                DocumentId bilgisi yollanıyor.
                Güncelleme operasyonları için modal popup kullanılmakta.
                Bu popup'a ulaşırken GetPlayerForEdit metodu kullanılarak güncel değerleri de çekiliyor.
                Modal Popup, yine bu sayfa içerisinde tanımlı bir div elementi. data-toggle ve data-target niteliklerine 
                atanan değerlerle, button kontrolü arasında ilişki kuruluyor.
                -->
            @foreach (var p in playerList)
            {
                <tr>
                    <td>@p.Fullname</td>
                    <td>@p.Length</td>
                    <td>@p.Position</td>
                    <td>@p.SomeInfo</td>  
                    <td><button class="btn btn-outline-danger" 
                        onclick="@(async () => await DeletePlayer(@p.DocumentId))">
                        Sil</button>
                    </td>   
                    <td>
                        <button class="btn btn-outline-primary" data-toggle="modal" data-target="#EditPlayerModal" 
                        onclick="@(async()=>await GetPlayerForEdit(@p.DocumentId))">
                        Güncelle</button>
                    </td>               
                </tr>
            }
        </tbody>
    </table>
}

<!--
Modal popup bileşenimiz.
ID bilgisini button kontrolü kullanmakta.
Bir bootstrap modal penceresi genelde üç ana kısımdan oluşuyor.
Başlık ve X işareti gibi bilgileri içeren modal-header,
Asıl içeriği bulunduran modal-body
ve kaydet, vazgeç gibi button kontrollerini veya özet bilgileri bulunduran modal-footer.
Edit işlemi yapılırken documentId bilgisi ile elde edilen oyuncu verisi,
Razor bileşenindeki currentPlayer değişkeninde yer almakta. Dolayısıyla modal
kontrollerini bu değişkene bind ediyoruz.

Modal popup kullanabilmek için jquery ve bootstrap javascript kütüphanelerine ihtiyacımız var.
Bunları index.js içerisinde bildirdik.
-->
<div class="modal fade" id="EditPlayerModal">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">Bilgileri güncelleyebilirsin</h3>
                <button type="button" class="close" data-dismiss="modal">
                    <span aria-hidden="true">X</span>
                </button>
            </div>
            <div class="modal-body">
                <form>
                    <div class="form-group">
                        <label class="control-label">Adı</label>
                        <input class="form-control" bind="@currentPlayer.Fullname"/>
                    </div>
                    <div class="form-group">
                        <label class="control-label">Boyu</label>
                        <input class="form-control" bind="@currentPlayer.Length"/>
                    </div>
                    <div class="form-group">
                        <label class="control-label">Mevkisi</label>
                        <input class="form-control" bind="@currentPlayer.Position"/>
                    </div>
                    <div class="form-group">
                        <label class="control-label">Hakkında</label>
                        <textarea class="form-control" rows="4" cols="30" bind="@currentPlayer.SomeInfo" />
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" 
                onclick="@(async ()=> await UpdatePlayer())" 
                data-dismiss="modal">Kaydet</button>
            </div>
        </div>
    </div>
</div>
```

PlayerData.cshtml.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NBAWorld.Shared.Models;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;

namespace NBAWorld.Client.Pages
{
    /*
    Razor sayfamız tarafından kullanılan Blazor bileşeni.
    Doğrudan PlayersController APIsi ile konuşur.
     */
    public class PlayerDataModel
    : BlazorComponent
    {
        /*
        API servisine göndereceğimiz talepleri ele alan HttpClient nesnesini
        Property Injection ile içeriye alıyoruz.
         */
        [Inject]
        protected HttpClient Http { get; set; }
        protected List<Player> playerList = new List<Player>();
        protected Player currentPlayer = new Player();

        protected override async Task OnInitAsync()
        {
            await GetAllPlayers();
        }
        protected async Task GetAllPlayers()
        {
            // api/Players tahmin edileceği üzere PlayersController'a yapılan bir çağrıdır
            playerList = await Http.GetJsonAsync<List<Player>>("api/Players");
        }

        /*
            bir dokümanı (yani oyuncuyu) silmek için kullandığımız fonksiyon
         */
        protected async Task DeletePlayer(string documentId)
        {
            // Doğrudan HTTP delete tipinden bir çağrı yapıyoruz
            // QueryString parametresi olarak arayüzden gelen doküman Id bilgisini kullanıyoruz
            await Http.DeleteAsync($"/api/Players/{documentId}");
            // Silme işlemi sonrası listeyi tekrar güncellemekte yarar var.
            await GetAllPlayers();
        }

        /*
        Güncelleme işleminden önce documentId ile oyuncu bilgilerini
        bulmaya çalıştığımız metod.
         */
        protected async Task GetPlayerForEdit(string documentId)
        {
            // Web API tarafına bir HTTP Get çağrısı yapıyoruz.
            // adresin son kısmında doküman id bilgisi bulunuyor.
            currentPlayer = await Http.GetJsonAsync<Player>("/api/Players/" + documentId);
        }

        /*
        Oyuncu bilgilerini güncellemek için kullanılan metodumuz.
        Parametre almadığına bakmayın. Razor sayfasındaki bileşenlere bağlanan
        currentPlayer içeriği kullanılıyor. Bu değişken güncelleme için
        açılan Modal Popup tarafından değiştirilebilmekte.
         */
        protected async Task UpdatePlayer()
        {
            // Web API tarafına HTTP Put metodu ile bir çağrı yapıyoruz
            // Request Body'de currentPlayer içeriği yollanıyor.
            await Http.SendJsonAsync(HttpMethod.Put, "api/players/", currentPlayer);
            await GetAllPlayers();
        }
    }
}
```

NewPlayer.cshtml

```text
<!--Razor sayfamızın adı newplayer. Navigasyonda bu ismi kullanıyoruz.
    Kullandığı model NewPlayerModel isimli BlazorComponent türevli bileşen.
    Kontrolleri, bileşendeki player isimli değişkene player.Özellike Adı 
    notasyonu ile bağlıyoruz.Button kontrolüne basıldığında onclick niteliği ile belirttiğimiz 
    kod parçası çalışıyor ve bileşendeki AddPlayer metodu tetikleniyor.
-->

@page "/newplayer"
@inherits NewPlayerModel

<h1>Yeni bir efsane eklemek istersen doldur, gönder...</h1>

<table class='table'>
    <tbody>
        <tr>
            <td><p>Adı</p></td>
            <td><input class="form-control" bind="@player.Fullname" /></td>
        </tr>
        <tr>
            <td><p>Boyu</p></td>
            <td><input class="form-control" bind="@player.Length" /></td>
        </tr>
        <tr>
            <td><p>Mevkisi</p></td>
            <td><input class="form-control" bind="@player.Position" /></td>
        </tr>
        <tr>
            <td><p>Hakkında</p></td>
            <td><textarea class="form-control" rows="4" cols="30" bind="@player.SomeInfo" /></td>
        </tr>
        <tr>
            <td colspan="2"><button class="btn btn-primary" onclick="@(async () => await AddPlayer())">Ekle</button></td>
        </tr>
    </tbody>
</table>
```

NewPlayer.cshtml.cs

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using NBAWorld.Shared.Models;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;

namespace NBAWorld.Client.Pages
{
    /*
    Razor sayfamız tarafından kullanılan Blazor bileşeni.
    Doğrudan PlayersController APIsi ile konuşur.
    Temel görevi yeni bir oyuncuyu eklemektir. (Firestore veri tabanına)
     */
    public class NewPlayerModel 
    : BlazorComponent
    {
        /*
        API servisine göndereceğimiz talepleri ele alan HttpClient nesnesini
        Property Injection ile içeriye alıyoruz.
         */
        [Inject]
        protected HttpClient Http { get; set; }
        // Önyüzdeki HTML elementlerini bu özelliğe bağlayacağız (bind)
        protected Player player = new Player();

        protected async Task AddPlayer()
        {
            /* api/Players tahmin edileceği üzere PlayersController'a yapılan bir çağrıdır
            HTTP Post tipinden bir çağrı söz konusu ve parametre olarak player bilgisini gönderiyoruz.
            Dolayısıyla API tarafındaki Post isimli metot (farklı bir isimde verilebilir, HttpMethod.Post ile karıştırmayın) çağırılacaktır.
            player değişkeni, önyüz tarafına bind edildiği için, kontrollerin verisini içerecektir.
            */
            await Http.SendJsonAsync(HttpMethod.Post, "/api/Players/", player);            
        }
    }
}
```

Piuvvvv:) Çok kod yazdık belki ama sıkın dişinizi az kaldı! Son değişikliklerimiz ana sayfa ve navigasyon çubuğu ile ilgili. Ön yüz tarafında bootstrap kullandığımız dikkatinizi çekmiştir. Bunun tüm bileşenler için etkin olmasını wwwroot klasöründeki index.cshtml içerisindeki gerekli js kütüphane bildirimleri ile sağlayabiliriz. Diğer kısımlarda çok ufak tefek değişiklikler var.

```text
<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width">
    <title>NBAWorld</title>
    <base href="/" />
    <!--CDN veya diğer URL adreslerinden de bootstrap ve jquery için link verilebilir.
        Ben local'e indirdiklerimi kullandım.
    -->
    <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="css/site.css" rel="stylesheet" />
    <script src="js/jquery.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
</head>

<body>
    <app>Loading...</app>

    <script src="_framework/blazor.webassembly.js"></script>
</body>

</html>
```

NavMenu.cshtml dosyasına da yeni razor sayfaları için gerekli linkleri eklemeliyiz.

```text
<div class="top-row pl-4 navbar navbar-dark">
    <a class="navbar-brand" href="">NBA World</a>
    <button class="navbar-toggler" onclick=@ToggleNavMenu>
        <span class="navbar-toggler-icon"></span>
    </button>
</div>

<div class=@(collapseNavMenu ? "collapse" : null) onclick=@ToggleNavMenu>
    <ul class="nav flex-column">
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="playerspage">
                <span class="oi oi-list-rich" aria-hidden="true"></span> Oyuncular
            </NavLink>
        </li>        
    </ul>
        <ul class="nav flex-column">
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="newplayer">
                <span class="oi oi-list-rich" aria-hidden="true"></span> Yeni Oyuncu
            </NavLink>
        </li>        
    </ul>
</div>

@functions {
    bool collapseNavMenu = true;

    void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}
```

## Çalışma Zamanı

Artık yazdığımız ürünü test etmeye hazırız. Uygulamayı Visual Studio Code ile geliştirdik ve önümüzde bir Solution var. Visual Studio Code'da NBAWorld klasörünü ayrıca açıp F5 tuşuna bastığımızda bize çözümü hangi derleyici ile debug etmek istediğimiz sorulacaktır..Net Core seçeneğini işaretlersek ilgili Debug ayarları JSON dosyasına eklenir ve Build işlemi başlar. Ardından uygulama ayağa kalkıp (ki oraya gelene kadar aldığım hataları düzelttim) http://localhost:5888/ adresinden yayına başlar. Sizin de aşağıdakine benzer bir görüntü elde etmeniz gerekiyor.

![09_35_credit_11.png](/assets/images/2019/09_35_credit_11.png)

Oyuncular linkine basıldığında da aşağıdaki gibi...

![09_35_credit_12.png](/assets/images/2019/09_35_credit_12.png)

Yeni bir efsane eklemek istersek NewPlayer sayfasını kullanabiliriz.

![credit_13.png](/assets/images/2019/credit_13.png)

Güncelleme fonksiyonelliğini ekledikten sonraki durum da şöyle olacaktır. Görüldüğü üzere bir popup ile gerekli düzenlemeleri yapabiliyoruz (Bootstrap ile modal popup tasarlamak gerçekten kolay)

![09_35_credit_14.png](/assets/images/2019/09_35_credit_14.png)

## Ben Neler Öğrendim?

Hepsi bu kadar. Çok temel seviyede basit CRUD operasyonlarını gerçekleştiren bir Blazor uygulamasını inşa etmeyi başardık. Üstelik veriler Google Firestore üzerinde tutuluyor. Örneği geliştirmek pekala elinizde. Çok daha şık tasarıma sahip kendi alanınızla ilgili veri yönetim işlemlerini yapabileceğiniz bir uygulama haline getirebilirsiniz. Gelelim benim bu çalışmadan neler öğrendiğime.

- Blazor proje şablonlarını Ubuntu gibi bir platformda.Net Core için nasıl kullanabileceğimi
- Google Cloud üzerinde Firestore veri tabanı oluşturmayı
- Credential dosyasının ne işe yaradığını
- Basit Blazor bileşenleri yazmayı
- Blazor bileşeni ile Razor sayfasının nasıl etkileştiğini
- FirestoreData ve FirestoreProperty niteliklerinin kullanımını
- Ortak kütüphanede model sınıfı (Entity tipi olarak da düşünebiliriz) oluşturmayı
- Server Side tarafında Firestore ile haberleşen bir Data Access nesnesi yazmayı
- Firestore tarafındaki asıl CRUD operasyonlarını yapan DAL nesnesine önyüzden, API Controller yardımıyla nasıl gelinebileceğini
- Bir Bootstrap Modal Popup bileşeninin nasıl tasarlanabileceğini (jquery.min.js ve bootstrap.min.js ler olmadan işletemediğimi)

Böylece geldik birinci faza ait [35nci bölüm](https://github.com/buraksenyurt/saturday-night-works/tree/master/No%2035%20-%20Server%20Side%20Blazor%20with%20Firestore) derlemesinin sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.