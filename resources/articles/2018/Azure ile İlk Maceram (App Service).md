---
title: "Azure ile İlk Maceram (App Service)"
pubDate: 2018-05-18 18:15:00
categories:
  - Azure
tags:
  - Azure
  - Bash
  - JavaScript
  - JSON
  - .NET
  - ASP.NET
  - Web API
  - HTTP
  - Java
  - Ruby
  - Node.js
  - Concurrency
  - Visual Studio
---

# Azure ile İlk Maceram (App Service)
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Sinema tarihinin en araştırmacı en gözüpek en maceraperest arkeoloğu kimdir desek herhalde aklımıza tek bir isim gelir; Indiana Jones. Geçenlerde DVD arşivimden şöyle yanında patlamış mısırla izleyeceğim güzel bir macera filmi bakıyordum. Bu yaşıma kadar aslında bir çok ünlü seriyi arşivime eklemiştim. Baba, Matrix, Mad Max, Star Wars, Back to the Future, Terminator, Lord of the Rings ve diğerleri. Derken Indiana Jones çıktı karşıma ve gecenin izlencesi belli oldu. Mısırlar patlatıldı, naneli limonatalar hazırlandı, DVD takıldı, perdeler indirildi ve seyir başladı. Pek tabii Indiana Jones'un bir profesör olmasından çok atıldığı maceralardı seyirciyi ekrana bağlayan. Ona can veren Harrison Ford'un ince esprileri de cabasıydı. Filmi büyük bir keyifle tamamladıktan sonra geçtim West-World'ün başına. Bir Indiana Jones değildim ama benim de kendi çapımda minik maceralarım vardı. Sıradaki serüven yüksek tepelerin ardında, ihtişamlı bulutları ile göz kamaştıran Azure hanedanlığına doğru olacaktı.

Bulut bilişim dünyasının başrol oyuncularını düşündüğümüzde karşımıza Amazon Web Services, Google Cloud Platform ve Microsoft Azure çıkıyor (İlk harflere göre sıralayarak yazdım:P) Neredeyse hepsinin benzer amaç, araç ve sunduğu hizmetler var (Düşünsenize hepsinde mutlaka tarayıcı üzerinde çalıştırabildiğimiz terminal konsolları bulunuyor) Bu nedenle herhangi birinde deneyimlediğimiz tecrübeleri diğerlerinde tatbik etmek de mümkün. Bu platformlarda hayatın nasıl işlediğini anlamak için sundukları dokümanlardan yararlanmaksa en mantıklısı.

Bu geceki maceramızda Azure'un App Service olarak isimlendirilen ürününü kullanarak node.js ile yazılmış bir web uygulamasını bulut üzerinde yayınlamaya çalışacağız. Daha önceden benzer senaryoları AWS üzerinde deneyimleme fırsatım olmuştu. Son zamanlarda da Plursalsight'tan Azure konulu eğitimleri izlemekteyim. Tüm bunlar beni bu yazıya itmiş durumda diyebilirim.

> Bu yazıdaki örneği kendiniz denemek isterseniz [Azure](https://portal.azure.com)'da bir aboneliğinizin olması gerektiğini hatırlatmak isterim. Ben, Free Tier adı verilen ücretsiz hesap ile söz konusu örneği geliştirmekteyim. Sizde ilk deneyimleriniz için bu planı değerlendirebilirsiniz.

Yazımızdaki temel amacımız Azure'un desteklediği dillerden birisini kullanarak geliştirilen uygulamayı buluta alıp yayınlamaktan ibaret. Konuyu araştırdığım tarih itibariyle PHP, Java, Ruby, Go ve pek tabii.Net Core için destek sunuluyor. Ben elimin bir süredir de sıcak durduğu Node.js dilini seçtim. Azure'un App Service hizmetini Linux tabanlı bir ortam üzerinde deneyimleyeceğiz. Aslında App Service bir web hosting hizmeti olarak düşünülebilir. Kurulumu oldukça kolaydır ve bir plana bağlandığında dağıtım gibi işlemlerde basittir. App Service üzerine Azure'da bir çok hazır şablon bulunmaktadır. Mobil uygulamalardan, medya hizmetlerine, Joomla menşeli blog alt yapısından Asp.Net başlangıç paketine kadar bir çok kullanıma hazır uygulama servisini bu kaynak altında bulabiliriz. Bu detayları bir kenara bırakarak devam edelim.

Gelelim işlemlerimizi nerede yapacağımıza? Evet son derece aptal bir web uygulamamız olacak ama işin en önemli kısmını Azure üzerinde yapacağız. Buradaki operasyonel işlemler için portal üzerindeki Cloud Shell isimli terminalden faydalanacağız. Kodun kendisi local makinemizde yazacağız (Benim için West-World oluyor) Yerel bilgisayardaki web uygulamasını Azure App Service üzerine kuracağımız ortama alınması içinse git'ten yararlanacağız.

Sıralı bir şekilde gittiğimiz takdirde çok da kafa karıştırıcı olmayan basit işlemler icra edeceğimizi ifade edebilirim. Haydi gelin işe Cloud Shell'i açarak başlayalım. Tabii öncelikle portal'a girmemiz ve geçerli bir abonelik üzerinden oturum açmamız gerekiyor. Cloud Shell iki seçenek sunan bir terminal arabirimi. Bash Shell veya Windows Powershell kullanabiliriz. Ben Bash Shell seçeneğini tercih ettim. Bu durumda aşağıdaki ekran görüntüsü ile karşılaşmalıyız.

![image.axd](images/image.axd)

> Yazıdaki işlemlerimizi Cloud Shell aracılığı ile yapacağız ama Azure'un komut satırı aracını yerel makine üzerinden de kullanabiliriz. Bunun için [şu adrese](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) uğramanızı önerebilirim.

Operasyonel işlemlerimiz için az isimli komut satırı programından yararlanacağız. Yazıyı hazırladığım tarih itibariyle Common Language Interface'in 2.0 sürümü kullanılıyordu. Cloud Shell tahmin edeceğiniz üzere online bir terminal ve üzerinde çalışmakta olduğumuz sanal bir makine. Yapacağımız ilk iş aşağıdaki terminal komutunu vererek dağıtım operasyonunu üstlenecek bir kullanıcı oluşturmak.

```bash
az webapp deployment user set --user-name abi-wan-kenobi --password <<buraya okkalı bir şifre girin>>
```

![image.axd](images/image.axd)

Bir deployment kullanıcısı oluşturduk. Bu kullanıcı yerel makinedeki kodları Azure platformuna git üzerinden aktarırken gerekli olacak. Ancak tek yol git kullanmak da değil. FTP bağlantısı yaparak da uygulamamızı taşımamız mümkün ki bu senaryoda da yukarıda oluşturulan kullanıcıya ihtiyacımız olacaktır. Diğer platformlardakine benzer olarak kullanıcılar ve kullanıcıların yetkileri önemli. Gerçekten belli işler için sadece yapacaklarına ait yetkileri taşıyan kullanıcılar oluşturma alışkanlığını kazanmak gerekiyor. Kısacası her şeyi yapabilen tek bir süper kullanıcı kullanmamalıyız. İşte bu yüzden senaryomuza sadece dağıtım işinden sorumlu olacak bir kullanıcı tanımı ile başladık.

Sıradaki adımda bir Resource Group oluşturacağız. Bunu n sayıda kaynağı barındıran bir taşıyıcı (container) olarak düşünebiliriz. Azure üzerindeki enstrümanlar birer kaynak (resource) olarak tanımlanmakta. Sanal makine (Virtual Machine), veri tabanı (database), dağıtım yapan kullanıcı (deployment user), depolama alanı (disk storage), bu örnekteki web uygulaması (web app) vs. Senaryoya göre bir kaynak grubunun tanımlanmasının yönetimsel açıdan avantajları bulunuyor. Kaynak gruplarını tanımlarken önemli kriterlerden birisi da lokasyon. Aslında kaynaklar Microsoft'un dünya üzerindeki farklı lokasyonlarında konuşlandırılmış da olabilirler. Dolayısıyla bir Resource Group farklı lokasyonlarda duran kaynakları içerebilir. Aslında içermekten kasıt sadece metadata'sında bu kaynakların bilgilerini tutmasıdır. Sonuçta Resource Group'un sahip olduğu metadata'nın da bir yerlerde duruyor olması gerekir. Kullanılabilecek lokasyonların listesini Cloud Shell'den öğrenmek de mümkün. Örneğin aşağıdaki terminal komutunun çıktısı olarak Linux tabanlı ve App Service desteği sunan lokasyonları görebiliriz.

```bash
az appservice list-locations --sku S1 --linux-workers-enabled
```

![image.axd](images/image.axd)

Ben East US 2 bölgesinde milano-rg isimli bir kaynak grubu tanımlamaya karar verdim. Bunun için aşağıdaki terminal komutundan yararlanılabilir.

```bash
az group create --name milano-rg --location "East US 2"
```

![image.axd](images/image.axd)

(az terminal komutlarının çıktısı dikkat edeceğiniz üzere JSON formatında) Son terminal komutundaki çıktya göre provisioningState için Succeeded değeri dönüldü. Yani grup başarılı bir şekilde oluşturuldu.

Şimdi App Service için bir plan oluşturacağız. Bir plan içerisinde genellikle ücretlendirme modeli ve taşıyıcı tipi gibi bilgiler bulunur. Söz gelimi bu örnek kapsamında en uygun ve ucuz olan kiralama modeline sahip sanal makineyi seçip taşıyıcı tipi olarak da Linux çekirdekli bir ortamı tercih etmek istersek aşağıdaki terminal komutunu çalıştırmamız yeterli olacaktır.

```bash
az appservice plan create --name milano-app-plan --resource-group milano-rg --sku S1 --is-linux
```

--sku S1 ile S1 kodlu fiyatlandırma modelini kullanacağımızı, --is-linux ile de Linux Container üzerinde çalışacağımızı belirtmiş olduk. Özellikle planları oluştururken gereken ücretlendirme modellerine bakmakta yarar var. [Şu adresten](https://azure.microsoft.com/en-us/pricing/details/app-service/) gerekli bilgilere ulaşabilirsiniz. Bir çok plan söz konusudur. Planlarda belirtilen sku'larda makinenin çekirdek sayısı, günlük yedek alma miktarı, kaç Gb Ram'e sahip olacağı, disk kapasitesi, hangi diğer uygulama servislerini sunacağı (SQL, Biztalk vs), eş zamanlı instance değerleri gibi özellikler tanımlıdır. App Service ile App Service Plan arasında kritik bir ilişki de vardır. Birden fazla App Service'in aynı App Service Plan'a bağlanması mümkündür. Yani farklı uygulamaları barındıran farklı App Service örneklerini aynı servis planı ile ilişkilendirebiliriz. Bunun ölçeklemelerde önemli bir artısı vardır. Tek bir planı yukarı (Scale Up) veya aşağı (Scale Down) çekerek kendisine bağlı olan tüm uygulamaların bu ölçeklemeden aynı anda yararlanmasını sağlayabiliriz. Burada ister yukarı ister aşağı yönlü ölçekleme olsun, ilgili App Service Plan'a ait makine örneklerinin sayısının arttırılması veya azaltılması durumu söz konusudur.

> Araya bir ekran görüntüsü koyarsam sanırım daha anlaşılır olabilir. Node.js Starter Kit tipinden bir App Service ve buna bağlı bir plan seçerken Azure...
> ![image.axd](images/image.axd)

Planımızı komut satırından oluşturarak devam edelim.

![image.axd](images/image.axd)

Şu ana kadar bir Deployment User, Resource Group ve App Service Plan oluşturduk. Peki uygulama nerede? Öncelikle Node.js'in çalışabileceği bir imaja ihityacımız var. Aslında Azure'casını ifade edersek bir Web App üretmemiz gerekiyor. Dilersek Azure'un desteklediği Linux tabanlı Web App çalışma zamanlarına bakabiliriz. Bunun için aşağıdaki terminal komutunu kullanmamız yeterli.

```bash
az webapp list-runtimes --linux
```

![image.axd](images/image.axd)

Ruby, Node'un epey bir sürümü, PHP,.Net Core (2.1 olmaması yazıyı hazırladığım tarih itibariyle ilginçti), Java ve Go...Node'un 9.4 versiyonunu destekleyecek bir Web App oluşturmak için terminalden aşağıdaki komutu vermek yeterli.

```bash
az webapp create --resource-group milano-rg --plan milano-app-plan --name FishingServices --runtime "NODE|9.4" --deployment-local-git
```

Komutta kullandığımız bir kaç parametre var. Resource Group, App Service Plan, uygulamanın adı (ki örneğimizde FishingServices olarak geçiyor), çalışma zamanı (node 9.4'ü seçtik) ve deployment seçeneği (bu da Git olarak belirtildi) Buna göre uzun bir JSON çıktısına sahip olacağız ancak içerisinde bizim için önemli bilgiler var.

![image.axd](images/image.axd)

Birisi deploymentLocalGitUrl ve diğeri de defaultHostName. Çıktıya dikkat edilecek olursa fishingservices.azurewebsites.net isimli bir adres yer alıyor. Eğer bu adrese gidersek aşağıdakine benzer bir çıktı ile karşılaşmamız olası (İlk talepte cevap süresi biraz uzun olabilir)

![image.axd](images/image.axd)

Bu hazır bir şablon ancak şu noktada Azure App Service üzerinden bir Web Hosting işlemi gerçekleştirdiğimizi söyleyebiliriz. Tabii amacımız buraya kendi yazdığımız Node.js uygulamasını taşımak. Ben bunun için West-World'deki Visual Studio Code'u kullanarak aşağıdaki içeriğe sahip basit bir index.js dosyası oluşturdum. Internet'te konu ile ilgili örnek dokümanlara baktığınızda da benzer kod parçaları ile karşılaşabilirsiniz. Temel amacımızın Azure tarafı olduğunu hatırlatalım.

```javascript
var http = require('http');

var server = http.createServer(function(request, response) {

    response.writeHead(200, {"Content-Type": "text/html"});
    response.end("<h1>Fishing services and utilities.</h1><p>Under Construction</p>");

});

var port = process.env.PORT || 4454;
server.listen(port);

console.log("Server is online http://localhost:%d", port);
```

Ekrana çok düz bir HTML içeriği basılıyor. Bunun için createServer metoduna alınan callback fonksiyonundan yararlanılmakta. writeHead ile istemciye HTTP 200 bilgisini döndürüyoruz. Yani her şey yolunda. end fonksiyonunun içerisindeyse tahmin edeceğiniz üzere HTML içeriğimiz yer alıyor. Oluşturulan server nesnesinin listen fonksiyonu ile de 4454 numaralı porttan yayın hizmet vereceğimizi ifade ediyoruz. Tabii bu lokal makine için geçerli. Uygulamayı Azure ortamına taşıdığımızda port bilgisi process.env.PORT üzerinden otomatik olarak elde edilecek. Bu arada kodun olduğu klasörde

```text
npm init
```

ile package.json dosyasını oluşturup içeriğini aşağıdaki gibi düzenleyebiliriz.

```json
{
  "name": "fishing-app",
  "version": "1.0.0",
  "description": "sample azure app",
  "main": "index.js",
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1",
    "start": "node index.js"
  },
  "author": "burak selim senyurt",
  "license": "ISC"
}
```

start elementini elle eklememiz gerekiyor. Bildiğiniz gibi bu sayede söz konusu uygulamayı terminalde

```text
npm start
```

komutunu vererek başlatabiliyoruz ki bu aynı zamanda Azure tarafındaki ortam için de gerekli.

Peki şimdi ne olacak? Azure'a uygulamayı taşıyabileceğimiz bir Web App ekledik. Lokal makinemizde de Node.js ile yazılmış ve çalışan bir programımız var. Hazırlıklarımıza göre geliştirici makinesindeki uygulamayı git ile Azure'a alabiliriz. Bunun için ilk adım olarak yerel git deposunu Azure'a bağlamamız gerekiyor. West-World için bu işlem aşağıdaki terminal komutu ile sağlanabildi.

```bash
git remote add milano https://abi-wan-kenobi@fishingservices.scm.azurewebsites.net/FishingServices.git
```

Azure tarafında üretilen git adresini uzak bir bağlantı seçeneği olarak ekliyoruz. Bundan sonra tek yapılması gerekense

```bash
git push milano master
```

komutunu çalıştırmak. Yani kodlarımızı milano olarak isimlendirdiğimiz uzak adrese doğru aktarmak. Bu işlem sırasında bir şifre de sorulacaktır. Bilin bakalım bu şifreyi ne zaman ve nerede belirledik:)

![image.axd](images/image.axd)

Dağıtım işlemi başarılı bir şekilde tamamlandıktan sonra fishingservices.azurewebsites.net adresine tekrar gidersek içeriğin değiştiğini ve Node.js ile yazdığımız uygulamanın çalıştığını rahatlıkla görebiliriz.

![image.axd](images/image.axd)

Uygulama kodunda değişiklikler yapacak olursak standart commit işlemini uygulayıp ardından tekrardan push ile dağıtımı yapmamız gerektiğini hatırlatayım (Bunu bir deneyin derim. Hatta uygulamayı bir Web API servisi haline getirip commit'lemeyi ve bu şekilde dağıtmayı deneyebilirsiniz. Özellikle bu durumda uygulamanın bağımlı olduğu npm paketleri varsa bunları karşı tarafa da aktarmak gerekebilir. Acaba burada nasıl bir yol izlenmelidir?) Bu arada eğer portal üzerinden kaynaklara gidersek FishingServices isimli App Service örneğimizi de görebiliriz. Aşağıdaki ekran görüntüsünde kendi hesabımdaki anlık durum yer alıyor.

![image.axd](images/image.axd)

Sonuçlar oldukça tatmin edici öyle değil mi? Biraz fazla terminal komutu kullandık ama adım adım oluşumu anladık diye düşünüyorum (En azından benim kafamda biraz daha netleşti) Pek tabii bu ücretsiz planlar bir süre sonra başa dert olabilirler. O nedenle oluşturduklarımızı silersek iyi olabilir ki Microsoft'un kendi öğreti dokümanlarında da bu önerilmekte. İşte bu nokta bir Resource Group oluşturmanın faydasını da göreceğiz. Aşağıdaki terminal komutunu Cloud Shell'den çalıştırdığımızda milano-rg ile ilişkili olarak oluşturulan ne kadar kaynak varsa otomatik olarak silinecek.

```bash
az group delete --name milano-rg
```

Ve tabii Fishing Services isimli balıkçı malzemeleri hizmeti veren firmanın sitesi de aşağıdaki hale gelecek.

![image.axd](images/image.axd)

Hepsi bu kadar:)

Bu yazımızda kendi bilgisayarımızdaki bir Node.js uygulamasının Azure App Service üzerine nasıl dağıtılabileceğini incelemeye çalıştık. Ağırlıklı olarak (hatta tamamen) terminal komutlarından yararlandık. Önce dağıtım işlemini üstlenen bir kullanıcı oluşturduk. Kaynaklara ait bilgileri içeren bir Resource Group tanımlaması ile devam ettik. App Service için gerekli planımızı belirledik ve bir Web App oluşturulmasını sağladık. Son olarak yazdığımız basit Node.js uygulamasını taşımak için git aracından faydalandık. Pekala aynı işlemleri Azure Portal üzerinden görsel olarak da gerçekleştirebiliriz. Sizler örneği çok daha uç noktalara taşıyabilirsiniz. Eğer kendi bilgisayarınızda geliştirdiğiniz güzel bir web uygulamanız varsa (hatta bloğunuz) bunu Azure üzerinde konuşlandırmanız son derece kolay. Ama bunu yaparken kiralama modellerine (planlara) bakmayı da ihmal etmeyin. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
