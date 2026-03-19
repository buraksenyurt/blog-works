---
title: "MongoDB ile Bir GO Uygulamasını Konuşturmak"
pubDate: 2019-12-18 08:00:00
categories:
  - GoLang
tags:
  - GoLang
  - MongoDB
  - Redis
  - REST
  - JSON
  - HTTP
  - gRPC
  - Docker
  - Go
  - Performance
  - Serialization
  - Testing
  - Visual Studio
  - GitHub
---

# MongoDB ile Bir GO Uygulamasını Konuşturmak
![screenshot_9.png](images/screenshot_9.png)

Teknoloji baş döndüren bir hızla ilerlerken beynimizin tembelleştiğini de kabul etmemiz gerekiyor. Artık pek çok işimiz otonomlaştırıldığından zihnimiz eski egzersizleri yapmıyor. Yıllar önce İngiltere'de yapılan bir araştırmada çocukların hesap makinesi kullanması sebebiyle temel dört işlem matematiğinde sorunlar yaşadığı tespit edilmişti. Yine Kanada'da yapılan bir araştırma insanların dikkat dağılma sürelerinin 8 saniyelere kadar indiğini gösterdi. Hafızamızı dinç tutma noktasında Japon balıkları ile yarışır bir konumda olduğumuz da aşikar. Kaçımız aklından ezbere 4 telefon numarasını sayabilir (Üç haneliler yasak) Otonomlaşan dünya sebebiyle tembelleşen ve dış uyarıcılar yüzünden sürekli dikkati dağılan zihnimiz...Gerçekten de dikkatimizi dağıtan, odaklanmamızı bozan o kadar çok şey var ki. Dolayısıyla kendimizi yetiştirmek istediğimiz konulara çalışırken ne kadar verimli olabiliyoruz bir bakmak gerekiyor. Tekrar satın alınamayacak olan zamanın ne kadar kıymetli olduğunu düşünürsek verimli çalışmanın ilerleyen yaşlarda çok çok önemli bir mesele haline geldiğini vurgulamak isterim.

İşte bu sebepten birkaç haftadır [Saturday-Night-Works](https://github.com/buraksenyurt/saturday-night-works) çalışmalarım sırasında pomodoro tekniğini neden kullanmadığımı karar kara düşünmekteyim. Oysa ki çok verimli bir çalışma pratiği. Atladığım bu önemli detayı SkyNet çalışmalarımın ilk gününden itibaren uygulamaya karar verdim. Genellikle gece 22:00 sularında başlayarak 4X25 dakikalık seanslar halinde ilerliyorum. Her seans arasında 5er dakikalık standart molalar var. Tabii bu tekniği uygularken en önemli kural çalışmayı bölecek unsurları mutlak suretle dışarıda bırakmak. Cep/ev telefonu, televizyon, radyo, e-mail programı ve benzeri odak dağıtıcı ne kadar şey varsa kapatmak gerekiyor. Bunun faydasını epeyce gördüğümü ve 25 dakikalık zaman dilimlerindeki çalışmalardan iyi seviyede verim aldığımı ifade edebilirim. Aranızda uygulamayanlar varsa bir göz atsınlar derim;)

> Pomodoro tekniğini uygularken size akıllı bir kronometre gerekecek. Tarayıcıda çalışan [Tomato-Timer](https://tomato-timer.com/) tam size göre. Hatta Visual Studio Code için eklentisi bile var;)

Gelelim SkyNet'te geçirdiğim ikinci güne. Elimizdeki malzemeleri sayalım. MongoDB için bir docker imajı, gRPC ve GoLang. Bu üçünü kullanarak CRUD (Create Read Update Delete) operasyonlarını icra eden basit bir uygulama geliştirmek niyetindeyim. Bir önceki öğretide Redis docker container'dan yararlanmıştım. Kaynakları kıymetli olan Ahch-To sistemini kirletmemek adına MongoDB için de benzer şekilde hareket edeceğim. Açıkçası GoLang bilgim epey paslanmış durumda ve sistemde yüklü olup olmadığını dahi bilmiyorum.

terminal komutu da bana yüklü olmadığını söylüyor. Dolayısıyla ilk adım onu MacOS üzerine kurmak.

## İlk Hazırlıklar (Go Kurulumu ve MongoDB)

GoLang'i Ahch-To adasına yüklemek için [şu adrese](https://golang.org/dl/) gidip Apple macOS sürümünü indirmem gerekti. Ben öğretiyi hazırlarken go1.13.4.darwin-amd64.pkg dosyasını kullandım. Kurulum işlemini tamamladıktan sonra komut satırından go versiyonunu sorgulattım ve aşağıdaki çıktıyı elde ettim.

![11_02_screenshot_1.png](images/11_02_screenshot_1.png)

Pek tabii içim rahat değildi. Versiyon bilgisi gelmişti ama bir "hello world" uygulamasını çalışır halde görmeliydim ki kurulumun sorunsuz olduğundan emin olayım. Hemen resmi dokümanı takip ederek $HOME\go\src\ altında helloworld isimli bir klasör açıp aşağıdaki kod parçasını içeren helloworld.go dosyasını oluşturdum (Visual Studio Code kullandığım için editörün önerdiği go ile ilgili extension'ları yüklemeyi de ihmal etmedim)

Terminalden aşağıdaki komutları işlettikten sonra çıktıyı görebildim.

![11_02_screenshot_2.png](images/11_02_screenshot_2.png)

Go ile kod yazabildiğime göre MongoDB docker imajını indirip bir deneme turuna çıkabilirim. İşte terminal komutları.

![11_02_screenshot_3.png](images/11_02_screenshot_3.png)

İlk komutla mongo imajı çekiliyor. İzleyen komut docker container'ını varsayılan portları ile sistemin kullanımına açmak için. Container listesinde göründüğüne göre sorun yok. MongoDB veritabanını container üzerinden test etmek amacıyla içine girmek lazım. 4ncü komutu bu işe yarıyor. Ardından mongo shell'e geçip bir kaç işlem gerçekleştirilebilir. Önce var olan veritabanlarını listeliyor sonra AdventureWorks isimli yeni bir tane oluşturuyoruz. Devam eden kısımda category isimli bir koleksiyona iki doküman ekleniyor ve tümünü güzel bir formatta listeliyoruz. Arka arkaya gelen iki exit komutunu fark etmişsinizdir. İlki mongo shell'den, ikincisi de container içinden çıkmak için.

Ah çok önemli bir detayı unuttum! Örnekte gRPC protokolünü kullanacağız. Bu da bir proto dosyamız olacağı ve Golang için gerekli stub içeriğine derleyeceğimiz anlamına geliyor. Dolayısıyla sistemde protobuf ve go için gerekli derleyici eklentisine ihtiyacım var. brew ile bunları sisteme yüklemek oldukça kolay.

Kod tarafına geçmeye hazırız ama öncesinde ufak bir bilgi.

## gRPC Hakkında Azıcık Bilgi

gRPC, HTTP2 bazlı modern bir iletişim protokolü ve JSON yerine ProtoBuffers olarak isimlendirilen kuvvetle türlendirilmiş bir ikili veri formatını kullanmakta (strongly-typed binary data format) JSON özellikle REST tabanlı servislerde popüler bir format olmasına rağmen serileştirme sırasında CPU'yu yoran bir performans sergiliyor. HTTP/2 özelliklerini iyi kullanan gRPC ise 5 ile 25 kata kadar daha hızlı. Bu noktada hatırlamak için bile olsa gRPC ile REST'i kıyaslamakta yarar var. İşte karşılaştırma tablosu.

REST Tarafı
gRPC Tarafı

HTTP 1.1 nedeniyle gecikme yüksek
HTTP/2 sebebiyle daha düşük gecikme

Sadece Request/Response
Stream desteği (Örneğimizde bir kullanımı var)

CRUD odaklı servisler için
API odaklı (Burada CRUD odaklı yapacağız çaktırmayın)

HTTP Get,Post,Put,Delete gibi fiil tabanlı
RPC tabanlı, sunucu üzerinden fonksiyon çağırabilme özelliği

Sadece Client->Server yönlü talepler
Çift yönlü ve asenkron iletişim

JSON kullanıyor (serileşme yavaş, boyut büyük)
Protobuffer kullanıyor (veri daha küçük boyutta ve serileşme hızlı)

Örnek Uygulama

Gelelim kod tarafına... Uygulamanın temel klasör yapısını aşağıdaki gibi oluşturabiliriz. Ben bu işlemleri $HOME\go\src\ altında gerçekleştirdim.

playerserver ve clientapp tahmin edileceği üzere sunucu ve istemci uygulama görevini üstleniyorlar. proto klasöründe yer alan player.proto, gRPC mesaj sözleşmesine ait tanımlamaları içermekte. Servis metodları, parametre tipleri ve içerikleri bu dosyada aşağıdaki gibi bildiriliyor.

Bu içeriği Go tarafında kullanabilmek için derlememiz lazım. Derlemeyi aşağıdaki terminal komutu ile gerçekleştirebiliriz (proto dosyasını VS Code tarafında daha kolay düzenlemek için vscode-proto3 isimli extension'ı kullandım)

Proto dosyasının tamamlanmasını takiben playerserver klasöründeki main.go dosyasını yazmaya başlayabiliriz. Biraz uzun bir kod dosyası ama sabırla yazıp, yorum satırlarını da okuyarak neler yaptığımızı anlamaya çalışmakta yarar var.

Sunucu tarafındaki kodlama tamamlandıktan sonra istemci tarafı için clientapp altında tester.go isimli bir başka dosya oluşturarak ilerleyelim. Burada komut satırından temel CRUD operasyonlarını icra edeceğiz. Yeni bir oyuncunun eklenmesi, bir oyuncu bilgisinin çekilmesi, tüm oyuncuların listesinin alınması vb

Piuvvv!!! Uzun bir yol oldu. Öyleyse çalışma zamanı sonuçlarımıza bakalım mı?

## Çalışma Zamanı

İlk gün çalışmasının meyveleri pek fena değil. server ve client tarafa ait go dosyalarını kendi klasörlerinde aşağıdaki terminal komutları ile derledikten sonra

önce sunucu ardından istemci programlarını çalıştırıp kodlaması ilk önce biten AddPlayer fonksiyonunu deneme şansı buldum. Birkaç oyuncu verisini girdikten sonra mongodb container'ına ait shell'e bağlanıp gerçekten de yeni dokümanların player koleksiyonuna eklenip eklenmediğine baktım. Sonuç tebessüm ettiriciydi:) İstemci uygulama gRPC üzerinden sunucuya mesaj göndermiş, sunucuya gelen içerik docker container üzerinde duran mongodb veritabanına yazılmıştı.

![11_02_screenshot_4.png](images/11_02_screenshot_4.png)

İkinci gün tüm oyuncu listesini gRPC üzerinden istemciye döndüren süreci yazmaya çalıştım. İlk başta yaptığım bir hata nedeniyle epey vakit kaybettim. GetPlayerList metodunu protobuffer dosyasında stream döndürecek şekilde tasarlamamıştım. Büyük bir veri kümesini filtresiz çektiğimizde bu ağ trafiğinin sağlıklı çalışması açısından sorun olabilir. Oyuncuları sunucudan istemciye doğru bir stream üzerinden tek tek göndermek çok daha mantıklı (Burada REST ile gRPC arasındaki farkları hatırlayalım) Sonunda servis sözleşmesini değiştirip gerekli düzenlemeleri yaptıktan sonra aşağıdaki ekran görüntüsünde yer alan mutlu sona ulaşmayı başardım.

![11_02_screenshot_5.png](images/11_02_screenshot_5.png)

Devam eden gün bir öncekine göre daha zorlu geçti. FindOne metodunu player_id değerine göre çalıştırmayı bir türlü başaramadım. Neredeyse 4 pomodoro periyodu uğraştım. Hatta pomodoro süreci bittikten sonra farkında olmadan saatlerce bilgisayar başında kaldım. Sorunu araştırırken vakit nasıl geçti anlamamışım. Sonuçta işe 3 saatlik uykuyla gittim. Ertesi gün Ahch-To'nun tuşuna bile basmadım. Bir günlük ara, problemi çözmem için beni sakinleştirmeye yeterdi. Nihayetinde sorunu buldum. İstemci aradığı ID değerini girip sunucuya çağrı yaptığında, servis metoduna gelen ID bilgisinin sonunda boşluk ve alt satıra geçme karakterleri de geliyordu. Trim fonksiyonu ile bu durumun oluşmasını engelledikten sonra silme operasyonunu da işin içerisine dahil ettim ve güncelleme operasyonu hariç komple bir test yaptım. Sonuçlar ekran görüntüsünde olduğu gibi tatmin ediciydi.

![11_02_screenshot_6.png](images/11_02_screenshot_6.png)

Silme operasyonuna ilişkin çalışmaya ait örnek bir ekran görüntüsü de aşağıdaki gibi.

![11_02_screenshot_7.png](images/11_02_screenshot_7.png)

## Neler Öğrendim?

Elbette SkyNet'te geçirdiğim bugünün de bana öğrettiği bir sürü şey oldu. Bunları aşağıda yer alan maddelerle ifade etmeye çalıştım.

- Bir protobuf dosyası nasıl hazırlanır ve Go tarafında kullanılabilmesi için nasıl derlenir,
- Go tarafından MongoDB ile nasıl haberleşilir,
- MongoDB docker container'ına ait shell üstünde nasıl çalışılır,
- Temel mongodb komutları nelerdir,
- Sunucudan istemciye stream açarak tek tek mongo db dokümanı nasıl döndürülür (main.go'daki GetPlayerList metoduna bakın)

## Eksikliği Hissedilen Konular

Her ne kadar pomodoro tekniği ile çalışmalarımı olabildiğince verimli hale getirsem de ister istemez yaşlı zihnim yoruluyor. Dolayısıyla şunları da yapabilsem iyi olurdu dediğim şeyler var. Bunları da şu iki madde ile sıralayabilirim.

- İstemci tarafını Go tabanlı bir web client olarak geliştirmeyi deneyebiliriz. Terminalden hallice daha iyidir. En azından çalışma sırasında yaşadığım Trim ihlali oluşmaz.
- Bir çok sunucu metodunda hata kontrolü var ancak bunların çalışıp çalışmadığı test etmek gerekiyor. Yani Code Coverage değerimizi neredeyse 0. Yazıyla sıfır:) Bir Go uygulamasındaki fonksiyonlar için Unit Test'ler nasıl yazılır öğrenmem lazım.

## Görev Listeniz

Ve tabii kabul ederseniz sizin için iki güzel görevim var:)

- Select * from players where fullname like 'A%' gibi bir sorguya karşılık gelecek mongodb fonksiyonunu geliştirip uygulamaya ekleyin.
- Güncelleme fonksiyonunu tamamlayın.

Böylece geldik SkyNet'te bir günün daha sonuna. Sonraki çalışmada Wails paketini kullanarak Go ile yazılmış bir masaüstü programı geliştirmek niyetindeyim. O zaman dek hepinize mutlu günler dilerim.
