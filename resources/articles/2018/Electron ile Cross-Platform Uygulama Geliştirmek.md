---
title: "Electron ile Cross-Platform Uygulama Geliştirmek"
pubDate: 2018-11-30 05:06:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - JSON
  - JavaScript
  - Windows Forms
  - Node.js
  - GitHub
---

# Electron ile Cross-Platform Uygulama Geliştirmek
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Bu aralar işler doğruyu söylemek gerekirse biraz can sıkıcı. KVKK (Kişisel Verilerin Korunması Kanunu) olarak bilinen ve müşteri verilerinin anonimleştirilmesini gerektiren bir çalışma içerisindeyiz. Verinin dağınık olması, hacimsel büyüklüğü, kurum içi süreçlerin karmaşıklığı, monolitleşmiş ERP uygulamamız üzerindeki etkilerinin çıkartılmasındaki zorluklar, araya giren başla işler nedeniyle yavaş ilerliyoruz. Kurumun tamamını ilgilendiren bir regülasyon söz konusu olduğu için de biraz fazlasıyla statik elektrik yüklüyüz. Yani şöyle bir Galvatron çıksa içimizden ya da Megatron, Electron ortalığı kasıp kavura...Imm, şey...Ne diyorum ben yahu. Electron'da nereden çıktı:) Aslında tesadüfen karşıma çıktı ve bu karşılaşma sayesinde kendimi ilginç ve zevkli bir maceranın içerisinde buldum.

West-World'de masaüstü uygulama yazabileceğimi ve bunu hem MacOS hem de Windows platformunda çalıştırabileceğimi öğrendim. Bu yeni bir şey değil ve hatta aklımıza Mono, Miguel de Icaza, Xamarin Forms geliyor ancak, bunu gerçekleştirmek için HTML, Node.js ve CSS kullanarak ilerleyebileceğimiz bir yol daha var.

İşin aslı vakti zamanında GitHub tarafından açık kaynak olarak geliştirilen [electron](https://electronjs.org/docs/tutorial/about) isimli çatı sayesinde platform bağımsız masaüstü uygulamaları yazmak mümkün (Hemde epey zamandır) Bu oluşumda Chromium ile Node.js alt yapısının hazırladığı bir senaryo söz konusu. Chromium ile HTML bazlı sayfaları bir masaüstü formu gibi sunmak mümkün. Node.js sayesinde alt seviye işletim sistemi işlemleri de yapabileceğimiz için web platformunun geliştirme olanaklarını masaüstüne taşıyabildiğimizi düşünebiliriz.

Sonunda hızlı bir deneyim için interneti taramaya ve basit bir "Hello World" programı yazmaya karar verdim. Zaten ara ara Linux üzerinde masaüstü uygulamaları yazabilmek için ne yapabileceğimi araştırıyordum. En önemli motivasyonumu, West-World (Ubuntu) üzerinde yazacağım masaüstü uygulamasını MacOS ve Windows platformlarında çalıştırabilmek şeklinde belirledim.

## West-World Üzerindeki Geliştirmeler

İşe Ubuntu üzerinde yapacağım geliştirmelerle başladım. Ortamda Node.js olması yeterliydi. Öncelikle bir klasör hazırladım ve npm ortamını tesis ettikten sonra electron çatısına ait npm paketini sisteme yüklettim.

```bash
npm init
npm install --save-dev electron
```

npm init ile gelen sorulara bir kaç cevap verip package.json içeriğini aşağıdaki hale getirdim.

```json
{
  "name": "helloforms",
  "version": "1.0.0",
  "description": "a simple desktop application for cross platform",
  "main": "main.js",
  "scripts": {
    "start": "electron .",
    "test": "echo \"Error: no test specified\" && exit 1"
  },
  "author": "burak selim senyurt",
  "license": "ISC",
  "devDependencies": {
    "electron": "^3.0.5"
  }
}
```

start kısmındaki electron. ifadesi çalışma zamanı için gerekli. Uygulamanın giriş noktası olarak main.js dosyasını kullanacağız. Aslında electron, chromium ile bir ana process başlatacak ve biz istersek diğer alt process'ler ile (Renderer Process olarak ifade ediliyor) IPC (Inter Process Communication) protokolü üzerinden mesajlaşabileceğiz. Bu şu anlama geliyor; ana process ve açılacak başka alt process'ler (örneğin ekranlar veya diğer arka plan işlerini yapan javascript kodları) IPC üzerinden haberleşebilirler. Bu sayede örneğin main.js dışındaki bir ekrandaki düğmeye tıklandığında tetiklenen olaydan, main.js'teki bir dinleyiciye mesaj gönderebilir ve dönüş alabiliriz. İşleyiş esasında bir Windows Forms uygulamasının yaşam döngüsünden pek de farklı değil. Bu durumu örnekte anlatmaya çalışacağım (Öğrenebildiğim kadarıyla) Main.js, index.html ve index.js isimli dosyalara ihtiyacımız var. Main.js içeriğini yazarak işe başlayalım.

```javascript
const { app, BrowserWindow, ipcMain } = require('electron')
console.log(process.platform)

let win

function createWindow() {
    win = new BrowserWindow({ width: 640, height: 480 })
    win.loadFile('index.html')
    //win.webContents.openDevTools()
    win.on('closed', () => {
        win = null
    })
}

app.on('ready', (createWindow))

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit()
    }
})

app.on('activate', () => {
    if (win === null) {
        createWindow()
    }
})

ipcMain.on("btnclick", (event, arg) => {
    var response = "Hello " + arg + ".How are you today?"
    event.sender.send("btnclick-task-finished", response);
})
```

electron modülünden app, BrowserWindow ve ipcMain tiplerini tanımlayarak işe başlıyoruz. Sonrasında gelen satırda çalıştığımız platformu console ekranına yazdırmaktayız. Bunu Windows, Linux veya MacOS'da çalışıp çalışmadığımdan emin olmak için ekledim. win isimli değişkenimiz BrowserWindow tipinden. Tahmin edeceğiniz gibi bu nesne ile bir ekran (pencere, form artık nasıl düşünmek isterseniz) tanımlıyoruz. createWindow fonksiyonunda ekranın nasıl oluşturulduğunu görebilirsiniz. Genişlik ve yükseklik belirttikten sonra index.html dosyasının bu pencere içerisine açılacağını belirtiyoruz. Sanki Windows Forms'da bir tarayıcı kontrolü içerisinde yerel bir HTML dosyasını gösteriyormuşuz gibi düşünebiliriz sanırım. Başlangıçta yorum satırı olarak belirttiğimiz openDevTools fonksiyon çağrısı ile Chrome Developer penceresi açılıyor. Şimdilik bu özellik kapalı.

win değişkeni üzerinden ve ilerleyen satırlardaki app, ipcMain nesneleri tarafından da çağırılan on isimli metodlar, belli olayların tetiklenmesi sonrası devreye giren fonksiyonlar. Örneğin açılan pencere için closed olayı tetiklenirse win nesnesi bellekten atılmak üzere null değerle işaretleniyor. Ya da uygulama hazır olduğunda createWindow fonksiyonu çağırılıyor. Birden fazla pencerenin olması da muhtemel tabii. Bu nedenle tüm pencerelerin kapatılması sonucu tetiklenen window-all-closed sonrası eğer üzerinde olduğumuz platform darwin (yani macOS) değilse quit çağrısı ile uygulama kapatılıyor.

Kodun son kısmında yer alan ve ipcMain nesnesi üzerinden çağırılan btnclick olay kontrolü, index.js içeriğini yazdığımız zaman anlam kazanacak. Şimdilik btnclick isimli bir olayın tetiklenmesi sonrası arg değişkeni ile gelen veriyi tekrardan geriye yolladığımızı söyleyebiliriz (Tabii geriye derken nereye, tahmininiz var mı?)

Gelelim index.html ve index.js içeriklerine.

```text
<!DOCTYPE html>
<html>

<head>
    <meta charset="UTF-8">
    <title>Electron Sample - I</title>
</head>

<body>
    <h1>Hello Electron! I am MEGATRON :P</h1>
    <div>
        <label>What's your name?</label>
        <input type="text" id="text-name" placeholder="name">
    </div>
    <div>
        <button id="button-hello">Say Something</button>
    </div>    
    <div id="div-response"></div>
    <script src="index.js"></script>
</body>

</html>
```

HTML sayfamızda basit bir tasarımımız var. Normalde CSS ile zenginleştirilebilir ama ben kısa yoldan öğrenmenin ve olanları anlamanın peşinde olduğumdan bu durumu es geçtim (CSS bilmemem de işin cabası tabii) HTMLce okuduğumuzda kullanıcının adını sorduğumuz ve düğmeye basmasını beklediğimiz bir pencere olduğunu söyleyebiliriz. Tabii text kontrolünün içeriğini alabilmek için id niteliğinden yararlanacağız. button-hello ve div-response kimlikleri de index.js içerisinde anlam kazanacaklar. index.html ile alakalı javascirpt içeriklerini gömülü olarak da yazabiliriz ama ben index.js ismiyle ayrı bir dosyada tutmaya karar verdim. Html ve javascript dosyaları arasındaki ilişkiyi sonlarda yer alan script bloğunda kurduğumuza da dikkat edelim (Bu arada kişisel tavsiyem ön yüz tarafında Bootstrap gibi bir yapıyı kullanarak ilerlemeniz. Çok daha şık ve her cihaza cevap verebilir arayüzler oluşturabilirsiniz. Github'daki electron repoda Bootstrap ile ilgili örnek yer alıyor)

```javascript
const ipcRenderer = require('electron').ipcRenderer;
var btnClick = document.getElementById('button-hello');

btnClick.addEventListener('click', () => {
    var name = document.getElementById('text-name').value
    ipcRenderer.send("btnclick", name)
})

ipcRenderer.on('btnclick-task-finished', (event, param) => {
    var div = document.getElementById('div-response')
    div.innerText = param
});
```

index.js kodları electron'dan ipcRenderer nesnesini alarak işe başlıyor. Hatırlarsanız Main.js içerisinde ipcMain isimli nesneden yararlanmıştık. ipcRenderer, Main process ile ilişkili alt process'ler için kullanılıyor. Ekrandaki button bileşenini klasik getElementById ile yakaladıktan sonra, click isimli bir eventListener ekliyoruz. Bu olay metodu içerisinde text-name kontrolünün veri içeriğini yakalayıp, ipcRenderer'ın send fonksiyonu üzerinden btnclick takısı ile bir yere gönderiyoruz...Sizce nereye göndermiş olabiliriz? Sanırım şu anda ipcMain kullanarak Main.js içerisinde yakaladığımız btnclick takma adlı olay metodu daha da anlam kazandı değil mi? Son olarak ipcRenderer üzerinden bu kez btnclick-task-finished takılı bir olay metodunu kodladığımızı görebilirsiniz. Buraya akan mesaj, main process'den geliyor. Yakalanan mesajı div-response isimli div kontrolü üzerine basıyoruz.

Özetle ekrandan yazılan metni main process'e gönderiyor ve oradan da index.js için açılan alt process'e karşılık veriyoruz. Bunu pekala ekrandan main process üzerinde çalıştırılacak paralel görevlerin icra edilmesi gibi işlemlerde ele alabiliriz. Ancak işin önemli kısmı bir tane main process'in olduğu ve diğer alt process'lerin main process ile IPC üzerinden iletişim kurduğudur (Bunun mutlaka başka faydaları da vardır ama neler olduğunu öğrenmem lazım. Araştırmaya devam Burak)

Aslında örnek kodlarım tamamen bu kadardı. Hemen west-world komut satırından

```bash
npm start
```

ile programı çalıştırdım ve aşağıdaki ekran görüntüsü ile karşılaştım. Bir masaüstü uygulamam olmuştu artık.

![image.axd](images/image.axd)

Hatta developer modu etkinleştirince (win.webContents.openDevTools () satırını açaraktan) çok tanıdık olduğumuz bir ekranla karşılaştım. Chrome üzerinden Debug yapıp bir şeyleri keşfetmeye çalışanlar aşinadır.

![image.axd](images/image.axd)

## Şirketteki Windows'ta Olanlar

West-World ile yaptığım çalışmaları bitirdikten sonra değişiklikleri github reposuna push edip uyku moduna geçtim. Ertesi sabah işe gider gitmez ilk olarak repodaki kodları indirdim. Çok doğal olarak ilk çalıştırma da hata aldım. Çünkü electron npm paketi sistemde yüklü değildi. Bu beraberinde başka bir konuyu gündeme getirdi. Aslında uygulamayı West-World'de yazdıktan sonra paketleyebilmeliydim. Bunu araştıracaktım lakin Windows üzerindeki sonuçları da çok merak ediyordum. electron paketini yükledim ve yine

```bash
npm start
```

ile programı çalıştırdım. Volaaaa...

![image.axd](images/image.axd)

Sonuç oldukça tatmin ediciydi benim için. Console loguna göre Windows platformunda olduğum aşikardı. Diğer taraftan metin kutusuna yazdığım bilgi, butona bastıktan sonra da işlenmişti. Önümde tek bir engel kalıyordu. Aynı kod parçasını bir Apple bulup MacOS üzerinde deneyimlemek.

## Gökan'ın MacOS'unda Yapılanlar

Sevgili dostum Gökhan sağolsun emektar Apple'ını kullanmama izin verdi. Kendisi her ne kadar yazılımcılıkla uğraşmasa da bilgisayarını tereddütsüz emrime amade etti. Pek tabii makinede pek bir şey yoktu. Node.js'i ve git'i kurmam gerekti. Sonrasında versiyon kontrollerini yaptım ve github adresime attığım projeyi yerel bilgisayara klonladım. electron paketinin olmaması ihtimaline karşın onu da npm üzerinden yükledim. Kabaca aşağıdaki ekran görüntüsündekine benzer bir durum oluştu diyebilirim.

```bash
node --version
npm --version
git --version
git clone https://github.com/buraksenyurt/nodejs-tutorials.git
cd Day09
npm i --save-dev electron
```

![image.axd](images/image.axd)

Ve derhal npm start komutunu vererek uygulamanın çalışıp çalışmadığını kontrol ettim.

![image.axd](images/image.axd)

Mutluluk!!!

West-World üzerinde electron kullanarak geliştirilen Chromium tabanlı bir masaüstü uygulamasını, hem Windows hem de MacOS'da sorunsuz şekilde çalıştırabildim. Üstelik çok az eforla. Cross-Platform adına Xamarin sonrası gördüğüm önemli çatılardan birisi oldu electron. Özellikle işin arkasında GitHub'ın olması, açık kaynak topluluk desteği, versiyonlardaki sürekli kararlılık, node.js'in gücü, HTML ve CSS kullanarak aynen web uygulaması geliştiriyormuşçasına ilerleyebilme imkanı cezbedici unsurlar diye düşünüyorum. Evdeki Linux, şirketteki Windows, komşudaki MacOS. Hepsinde github reposundan çekip çalıştırdığım aynı arayüze sahip masaüstü uygulamaları koşuyor. Eğer bunu paketleyerek dağıtmayı da öğrenirsem pek şukela olacak diye düşünüyorum. Ben bunu araştırıyorken siz de boş durmayın öğrenin. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Ayrıca electron ile ilgili Tutorial tadındaki örnek kodlara github'dan ulaşabilirsiniz](https://github.com/buraksenyurt/electron)[Bu uygulama kodlarını da buradan klonlayabilirsiniz (Day09)](https://github.com/buraksenyurt/nodejs-tutorials)[Electron Github projesi](https://github.com/electron)[Eğer bir sonraki adım ne olmalı derseniz ben şuradaki yazıyı takip edeceğim derim](https://codeburst.io/build-a-todo-app-with-electron-d6c61f58b55a)
