---
layout: post
title: "The Internet Computer (Internetin Yeniden Keşfi) ve Motoko'yu Duyunca Ben"
date: 2020-10-31 22:31:00 +0300
categories:
  - motoko
tags:
  - motoko
  - bash
  - javascript
  - json
  - http
  - nodejs
  - async-await
  - visual-studio
  - github
---
Herkese açık olan interneti genişletip kendi yazılım sistemlerimizi, kurumsal IT çözümlerimizi, web sitelerimizi, dağıtık bir ortamda firewall'lara ve yedekleme sistemlerine ihtiyacı duymadan güvenli bir şekilde konuşlandırabildiğimizi düşünelim. Hatta bunu sağlayan altyapı ile internete konan bu sistemler arasında fonksiyon çağrımı yapar gibi kolayca haberleşebildiğimizi (ve tabii ki güvenli bir ortamda) hayal edelim. Biraz blockchain benzeri bir dağıtık sistem kurugusu gibi değil mi? Tam olarak olmasa da oradaki teorileri baz almışlar gibi görünüyor. The Internet Computer adlı bu proje ICP (Internet Computer Protocol) adı verilen ve herhangi bir merkezi olmayan bir protokolü baz alarak, küresel ortamdaki bağımsız veri merkezlerinin, web sitelerinin, backend hizmetlerinin vb yazılımların aynı güvenlik garantileriyle çalıştığı kapatılamaz bir alt evren vaat ediyor.

![motokon.jpg](/assets/images/2020/motokon.jpg)

Aslında ilk okumalarımda şunu anladığımı ifade edebilirim: Internete alacağımız bir hizmeti geliştirirken kodun güvenliği ve ürünün açıklarının kapatılması için çaba sarf ediliyor. Bu durum referans ettiğimiz paketler güncellendiğinde benzer kontrollerin tekrar yapılmasını gerektiriyor, lakin hacker'lar bu açıkları çok seviyor. Bağımlı olduğumuz sistemlerle belki de yeterince özgür bir ortama da sahip olamıyoruz. İşte The Internet Computer fikri, geliştirdiğimiz sistemlerin standart bir güvenlik sözleşmesi ile ayağa kalkabildiği, asla kapatılamayacak ve kurcalanamayacak bir ortamın üstünde çalışmasını garanti etme felsefesini öne sürüyor.

Birde Big Tech denilen şirketlerin internetteki neredeyse her tür SaaS (Software as as Services)'ın altından çıkmasının, topladıkları müşteri verilerini sürekli birbirleriyle paylaşmasının ve interneti sahiplenmesinin de bu projenin başlatılmasında önemi büyük (Sahibi olmayan bir internet ortamında güvenilir, kesintiye uğramayan uygulamaların geliştirilmesini sağlamak amaçlardan birisi) Proje çok yeni de değil. [DFINITY](https://dfinity.org/) adı verilen kar amacı gütmeyen bir kuruluşun 2016 yılında başlattığı bir çalışma.

## Motoko

Konu esasında çok çok derin görünüyor. Detaylar için [şu adrese bir uğrayın](https://dfinity.org/) derim. Pek tabii benim derdim nasıl geliştirme yapıldığı. Bu platformun da bir SDK'sı (Canister Software Development Kit olarak geçiyor:)) ve programlama dili var. Motoko, bahsedilen uygulamaları geliştirmek için kullanılan yazılım dili. Aslında benimde bu merak çalışmasındaki amacım Motoko'yu Heimdall (Ubuntu-20.04) ile tanıştırmak.

## Ön Gereksinim ve Kurulumlar

Öyleyse hiç vakit kaybetmeden maceramıza başlayalım. Sistemde eğer önyüz geliştirmeleri de yapacaksak Node.js'in yüklü olması bekleniyor. CSDK'yi yüklemek içinse aşağıdaki terminal komutu yeterli.

```bash
sh -ci "$(curl -fsSL https://sdk.dfinity.org/install.sh)"

# SDK'in doğru yüklenip yüklenmediğini anlamak için versiyona bakmak yeterli
dfx --version

# Yeni bir hello world projesi oluşturmak için
dfx new freedom
```

Motoko için bir Visual Studio Extension'da mevcut. Üstelik new ile yeni proje oluşturduktan sonraki terminal görüntüsü de çok tatlı.

![skynet_33_Screenshot_01.png](/assets/images/2020/skynet_33_Screenshot_01.png)

Gelelim freedom içerisindeki kodlarımıza. Burada iki dosyaya dokunduğumu söyleyebilirim. Birisi index.js.

```javascript
import freedom from 'ic:canisters/freedom';

freedom.sayHello(window.prompt("En sevdiğin renk")).then(lovelyColor => {
  window.alert(lovelyColor);
});
```

ve diğeri de main.mo

```javascript
actor {
    public func sayHello(color : Text) : async Text {
        return "Hımmm...Demek en sevdiğin renk " # color # "!";
    };
};
```

## İlk Örneğin Çalışma Zamanı

Yine terminal penceresinden ilerlemek lazım. Ben src altındaki main.mo ve asset'lerdeki index.js'i biraz kurcaladım ve ilişkilerini anlamaya çalıştım. Aslında motoko kodları main.mo içerisinde yer alıyor. Asset dediğimiz örneğin önyüz tarafı da public altındaki index.js. Index.js içinden main.mo'daki bir fonksiyonu (sayHello) çağırabiliyoruz.

```bash
# Önce makinedeki veya uzaktan erişilebilen bir Internet Computer ağına bağlanmak gerekiyor
# Bunu projenin package.json dosyasının olduğu klasörde yapmak lazım
dfx start

# Network oluşturulduktan sonra uygulamamız için buraya benzersiz bir Canister Id ile kayıt olmamız gerekiyor
# Bunu da yine package.json'ın olduğu yerde aşağıdaki komutla yapabiliriz 
dfx canister create --all

# Şimdi gerekli npm paketlerinin yüklenmesi lazım
npm install

# Ve ardından bizim uygulamamızın build edilmesi
dfx build

# Build işlemi başarılı bir şekilde tamamlandıysa bunu az önce oluşturduğumuz
# Local Internet Network'üne dağıtmamız gerekiyor
dfx canister install --all

# Bu işlemlerin arından yazılan program fonksiyonunu terminalden anından test edebiliriz
# freedom uygulamasındaki sayHello fonksiyonunu Black parametresi ile çalıştır
dfx canister call freedom sayHello Black

# İşlerimiz bittikten sonra Network'ü kapatmak içinse;
dfx stop
```

Ama birde node.js ön yüzümüz vardı;) Onu da tarayıcıya gidip localhost:8080 arkasına, uygulama için üretilen Canister ID bilgisini dahil ederek test edebiliriz.

```text
http://127.0.0.1:8000/?canisterId=cxeji-wacaa-aaaaa-aaaaa-aaaaa-aaaaa-aaaaa-q
```

Canister register, build ve deploy işlemlerine ait bir görüntüyü paylaşarak devam edelim.

![skynet_33_Screenshot_02.png](/assets/images/2020/skynet_33_Screenshot_02.png)

ve çalışma zamanına ait iki görüntüyü de buraya bırakalım.

![skynet_33_Screenshot_03.png](/assets/images/2020/skynet_33_Screenshot_03.png)

![skynet_33_Screenshot_04.png](/assets/images/2020/skynet_33_Screenshot_04.png)

## Bazı Tespitler

Bu ilk örneği geliştirirken bir takım tespitlerim oldu. Onları şu maddelerle ifade edebilirim;

- Önyüz için Assets klasörü altındaki index.js'i kurcalamak lazım.
- Önyüzün kullandığı fonksiyonlar main.mo altında tutuluyor ancak bu şart değil. İkinci örnekte başka bir mo kaynağını kullanıyoruz.
- Local Network adres ve port için tanımlama dfx.json altında bulunuyor.
- Frontend tarafının entrypoint bilgisi ile main programlarının bildirimleri de dfx.json içerisinde.
- Dağıtım tarafında webpack kullanılmış. Paket bağımlılıkları ise package.json üstünde tutuluyor.
- Proje geliştirildikten sonra bir Local Network başlattık ve bu ortam için benzersiz bir Canister ID ürettik. Üretilen bu değeri başlatılan ağa kaydettik, projeyi build ettik ve sonrasında build olan projeyi bu ağa install ettik (ki burada WebAssembly içerisinde deploy oluyormuş) En nihayetinde örneği test edip çalıştırdık.

Bu arada build işlemi sonrası eğer terminalden aşağıdaki komutu girersek,

```bash
ls -l .dfx/local/canisters/freedom/
```

WebAssembly oluşumlarını da (wasm uzantılı dosya) görebiliriz.

![skynet_33_Screenshot_05.png](/assets/images/2020/skynet_33_Screenshot_05.png)

Diğer yandan örnekleri çoğaltmaya başlayıp Internet Computer Network ortamına yeni Canister'lar eklendikçe şöyle bir arabirimle de karşılaştım.

![skynet_33_Screenshot_06.png](/assets/images/2020/skynet_33_Screenshot_06.png)

## İkinci Örnek

Derken Motoko'yu tanımak için bir örnek daha yapayım dedim. Bu sefer algebra isimli bir proje oluşturdum. main.mo yanına einstein isimli yeni bir actor ekledim ve dfx.json'dan main.mo yerine bunu kullanacağımı belirttim. einstein.mo içerisinde tek bir fonksiyon bulunuyor. İki integer değer aralığındaki sayıların toplamını buluyor.

```javascript
actor einstein {
    public func gauss_sum(x:Int,y:Int) : async Int {
        var total:Int=0;
        var counter=x;
        while(counter<=y)
        {
            total+=counter;
            counter+=1;
        };
        return total;
  };
  
};
```

dfx.json'daki kısım;

```json
{
  "canisters": {
    "algebra": {
      "main": "src/algebra/einstein.mo",
      "type": "motoko"
    },
// Kod devam ediyor
```

Sonrasında uygulamayı aşağıdaki adımlardan geçirerek test ettim.

```bash
# Birinc terminalde (hepsi algebra klasörü altında yapılmalı)
dfx start

# Ağ başlatıldıktan sonra ikinci terminalde sırasıyla aşağıdaki işlemleri yaptım
dfx canister create --all
dfx build algebra
dfx canister install algebra

# ve komut satırından denememi yaptım
dfx canister call algebra gauss_sum '(1,100)'
```

İşte 1 ile 100 arasındaki sayıların toplamının bağımsız internet bilgisayarındaki ağda bulunması:)

![skynet_33_Screenshot_07.png](/assets/images/2020/skynet_33_Screenshot_07.png)

CanisterId'yi kullanarak aynı uygulamayı otomatik olarak üretilen web sayfasıyla da test edebiliriz. Benim örneğimde bu adres http://127.0.0.1:8000/candid?canisterId=75hes-oqbaa-aaaaa-aaaaa-aaaaa-aaaaa-aaaaa-q şeklindeydi.

![skynet_33_Screenshot_08.png](/assets/images/2020/skynet_33_Screenshot_08.png)

Motoko ve The Internet Computer'un geleceği ne olur bilinmez ama [Umut Özel](http://www.umutozel.com/) ile bir sohbetimiz sırasında ortaya çıkan bu kavramı şöyle bir kurcalama fırsatı bulduğum için kendi adıma memnunum. Önümüzdeki yıl bu alandaki gelişmeleri takip etmek istiyorum. Bu ilginç araştırmaya ait kodları [skynet github reposu](https://github.com/buraksenyurt/skynet/tree/master/No%2033%20-%20Independent%20Internet%20Computer)nda bulabilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
