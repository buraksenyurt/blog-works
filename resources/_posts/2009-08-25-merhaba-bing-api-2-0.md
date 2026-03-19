---
layout: post
title: "Merhaba Bing API 2.0"
date: 2009-08-25 11:15:00 +0300
description: "Bir süredir WCF 4.0 ile birlikte gelen yenilikleri sizlere aktarmaya çalışıyorum. Son olarak Routing Service ile ilişkili bir giriş yazımız olmuştu. Bu konu ile ilişkili örnek en kısa sürede sizlerle olacak. Ne varki konu biraz zorlu. Undecided Bu yüzden bende yüksek lisans eğitimi aldığım yıllarda çok sevgili hocam Halil Seyidoğlu' nun bir açıklamasını uygulamaya karar verdim. Kendisi bize \"Bilimsel Araştırma ve Yazma\" dersinde şöyle seslenmişt; \"Bir tez konusunu araştırırken çok zorlu yollardan geçersiniz. Tezin bir noktasında tıkandınız mı? O zaman ara verin...Tatile çıkın...Bir süreliğine uzaklaşın...\""
categories:
  - bing
tags:
  - bing
  - csharp
  - dotnet
  - linq
  - wcf
  - xml
  - soap
  - json
  - web-service
  - http
  - javascript
  - threading
  - delegates
  - testing
---
# Merhaba Bing API 2.0
![blg72_Giris.gif](/assets/images/2009/blg72_Giris.gif)

Merhaba Arkadaşlar,

Bir süredir WCF 4.0 ile birlikte gelen yenilikleri sizlere aktarmaya çalışıyorum. Son olarak Routing Service ile ilişkili bir giriş yazımız olmuştu. Bu konu ile ilişkili örnek en kısa sürede sizlerle olacak. Ne varki konu biraz zorlu.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Bu yüzden bende yüksek lisans eğitimi aldığım yıllarda çok sevgili hocam Halil Seyidoğlu'nun bir açıklamasını uygulamaya karar verdim.

Kendisi bize "Bilimsel Araştırma ve Yazma" dersinde şöyle seslenmişt; "Bir tez konusunu araştırırken çok zorlu yollardan geçersiniz. Tezin bir noktasında tıkandınız mı? O zaman ara verin...Tatile çıkın...Bir süreliğine uzaklaşın..."

Her ne kadar WCF 4.0 ile gelen yenilikleri araştırmak bir tez hazırlamak kadar zorlu ve çetin olmasada sıkıldığım noktada hemen bir kaçış aradım ve bakım ne buldum.

Bing API 2.0

Microsoft'un arama motoru Bing'i duymayan olmamıştır sanırım. Peki Bing'in kendi uygulamalarımızda kullanılabilmesi için dışarıya bir API sunduğunu biliyor muydunuz? Ta ta ta taaaa...

![Laughing](/assets/images/2009/smiley-laughing.gif)

İşin içerisinde bir developer API'si, helede servis bazlı bir sunum olunca, değmeyin keyfime dedim ve yola koyuldum. Dolayısıyla bu yazımda sizlere Bing API'si ile ilişkili ilk izlenimlerimi ve çıkarımlarımı aktarmaya çalışacağım.

Bing API'si, kendi web sitesinden sunduğu arama özelliklerinin tamamını, farklı iletişim protokollerine göre istemci tarafına servis bazlı olarak sunmaktadır. Buna göre dilersek Bing üzerinden gerçekleştirilen arama kabiliyetlerini ve sonuçlarını, kendi uygulamalarımıza entegre ederek kullanabiliriz. Bing hizmetinden yararlanabilmek için öncelikli olarak [http://www.bing.com/developers/](http://www.bing.com/developers/) adresindeki formu doldurmamız ve yeni bir App Id almamız gerekmektedir. Nitekim Live servisi ile olan haberleşmede App Id değerinden yararlanılmaktadır. Teori oldukça basittir. Arama kutucuğundan yapılan kabliyetleri, kendi uygulamamızdan bir şekilde request olarak göndermemiz gerekmektedir. Bu noktada aslında, Bing API ile neler yapabileceğimiz kararının nasıl verildiğine bakmamızda yarar vardır. Söz konusu karar verilirken aslında aramanın tipini/modelinide belirlemiş oluruz. Yada var olan aramayı genişletmiş oluruz. İşte burada a bahsedilen arama modelleri belirlenirken SourceTypes isimli tip değerlerinden yararlanılmaktadır. SourceTypes'ın değerleri managed code tarafında verilebileceği gibi, örneğin HTTP Get metoduna bağlı olarak URL formatında da yazılabilir. Genel SourceTypes değerleri ve uygulayabileceğimiz arama modelleri aşağıdaki gibidir;

- Web sayfaları,
- Resimler (Image),
- Videolar (Video),
- Dil çevirileri,
- Lokasyonlar,
- Uygulamanız ile alakalı reklamlar. Güncel SDK dökümanına göre sadece US sınırlarında geçerli.(Ads),
- MSN Encarta Online Encyclopedia'den anlık cevaplar. Örneğin What is 100*37? sonucunun bulunması (Instant Answer),
- XHTML veya WML formatında Mobile cihazlar için daha az yer harcayan sonuçlar (MobileWeb),
- Güncel arama ile ilişkili olan aramalar (Related Search),
- Haber içeriklerinin aranabilmesi (News),
- Hava durumu ile ilişkili aramalar (Weather)
- vb...

Güzel. Şimdi kafamızda bir şeyler şekillenmeye başladı. En azından arama modelini nasıl seçebileceğimizi anladık. Peki talepler nasıl iletilecekler?

![Wink](/assets/images/2009/smiley-wink.gif)

İstemciler taleplerini Bing API servisine 3 farklı formatta iletebilirler.

Format
Özet
URL

JavaScript Object Notation (JSON)
Ajax tabanlı uygulamalarda kullanılması tercih edilen bu tipe göre istemciye Raw, Callback ve Function formatlarında cevap döner.
http://api.search.live.net/json.aspx?AppId=YOURAPPID&Market=en-US&Query=testing&Sources=web+spell&Web.Count=1

eXtended Markup Language (XML)
SOAP formatını desteklemeyen veya Siverlight gibi uygulamalarda tercih edilir. İstemcinin talepleri HTTP Get metoduna göre gideceğinden URL sınırı en büyük handikapı olarak görülebilir.
http://api.search.live.net/xml.aspx?AppId=YOURAPPID&Market=en-US&Query=testing&Sources=web+spell&Web.Count=1

Simple Object Access Protocol (SOAP)
XML modelindeki gibi URL sınır kısıtı yoktur. Ayrıca karmaşık tiplerin (Complex Type) ifade edilebilmesi, request/response nesne modelinin sağlanması gibi avantajları vardır. Özellikle masaüstü uygulamalar (Desktop Applications) veya servis bazlı uygulamalar için idealdir. C# gibi yüksek seviyeli dillerle kullanımı son derece kolaydır.
http://api.search.live.net/search.wsdl?AppID=YourAppId (Web Service Referansını ekleme adresidir)

Görüldüğü gibi, Bing API'si için değerlendirilecek istemci talepleri, Json formatında, HTTP Get metodunda gönderilebilmektedir. Ama burada altı çizilmesi gereken ve benimde en çok ilgimi çeken SOAP modelidir. Öyleki, bu modelin uygulanması için istemci tarafının bir XML Web Service referansını kullanması yeterlidir. Bu, istemci tarafında managed bir kodun uygulanabilmesi anlamına gelmektedir. Asenkron çağrılar gerçekleştirebilir, strong tipler kullanabilir, hatta sonuç kümeleri üzerinde LINQ sorguları dahi yapılabilir.

> Bir zamanlar.Net üzerine eğitmenlik yapardım. İlk yıllarımda.Net 1.0 vardı ve Xml Web Service konusunda gerçek hayat örnekleri bulmakta zorlanırdık. Genellikle kendi servislerimizi yazar, çağırır ve ele alırdık. Yada popüler hava durumu servisi örneği. Ama gerçek hayat senaryolarında, çok basit olan ve bizim tarafımızdan yazılmamış bir Web Servisi nasıl değer kazanabilir,artık pek çok örneği ile görebilmekteyiz. İşte küçük bir örnek, Bing API tarafından kullanılan Live servisi...

Öyleyse hiç vakit kaybetmeden acele acele bir örnek yapalım.

![Smile](/assets/images/2009/smiley-smile.gif)

Bu acele örneğimizde basit bir Windows uygulamasına, aradığımız kritere uyan 20 resmi çekmeye çalışacağız. Bir başka deyişle SourceTypes.Image tipinden bir arama gerçekleştireceğiz. Yapmamız gereken ilk şey, Live Search servisine ait Xml Web Service referansını uygulamamıza eklemek olmalıdır. Aşağıdaki görüntüde olduğu gibi. Dilerseniz benim yaptığım gibi Web reference name alanının değerini aynen bırakabilirsiniz.

![blg72_AddWebReference.gif](/assets/images/2009/blg72_AddWebReference.gif)

Kişisel Not: Referans eklemesinden sonra Class Diagram görüntüsüne bakmanızı öneririm ![Sealed](/assets/images/2009/smiley-sealed.gif)

Uygulamamızın Form tasarımını aşağıdaki gibi düzenleyebiliriz. Ben arama kutucuğunun sonucu olarak gelecek resim bilgilerini, alt tarafta yer alan FlowLayoutPanel bileşeni içerisinde PictureBox kontrolleri ile ifade etmeyi tercih ettim.

![blg72_FormDesign.gif](/assets/images/2009/blg72_FormDesign.gif)

PictureBox kontrolümüzde, resmin arama sonuçlarından gelen tüm bilgilerinide saklamak istediğimden, aşağıdaki kod parçasında görülen ThumbImage isimli bir bileşen kullanmayı uygun gördüm.

```csharp
using System.Windows.Forms;
using WinClient.net.live.search.api;

namespace WinClient
{
    class ThumbImage
        :PictureBox
    {
        public ImageResult Result { get; set; }
    }
}
```

Dikkat edileceği üzere ImageResult tipinden bir özellik yer almaktadır. Bu özellik (Property), arama sonucu servisden gelen sonuç kümesinde yer alan resim bilgilerini taşıyan tiptir. Kendi içerisinde, resmin Thumbnail Url, Media Url, Title, Width, Height vb... bilgilerini taşımaktadır. Biz bu bilgilerden faydalanıyor olacağız. Peki ama nasıl? İşte Form sınıfımızın tüm kod içeriği;

```csharp
using System;
using System.Windows.Forms;
// Varsayılan olarak SOAP tabanlı Live servisinin eklenmesi ile gelen namespace
using WinClient.net.live.search.api;

namespace WinClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            pnlImages.Controls.Clear();

            if (!String.IsNullOrEmpty(txtSearch.Text))
            {
                using (LiveSearchService searchService = new LiveSearchService())
                {
                    #region Arama talebi oluşturulur

                    SearchRequest request = new SearchRequest
                    {
                        AppId = "{Size verilen AppId değeri}",
                        Query = txtSearch.Text,
                        Sources = new SourceType[] { SourceType.Image },
                        Adult = AdultOption.Strict,
                        AdultSpecified = true,
                        Image = new ImageRequest { Count = 20, CountSpecified = true, Offset = 0, OffsetSpecified = true }
                    };

                    #endregion

                    #region Arama sonucunun değerlendirilmesi

                    SearchResponse response = searchService.Search(request);

                    if (response.Image!=null &&
                        response.Image.Results.Length > 0)
                    {
                        foreach (ImageResult imgResult in response.Image.Results)
                        {
                            ThumbImage img = new ThumbImage
                            {
                                Result = imgResult,
                                ImageLocation = imgResult.Thumbnail.Url
                            };

                            img.Click += delegate(object obj, EventArgs args)
                            {
                                Form frm = new Form()
                                {
                                    ControlBox=true
                                    , MaximizeBox=false
                                    ,MinimizeBox=false
                                    , Text=String.Format("{0} X {1} / {2} / {3} bytes",img.Result.Width,img.Result.Height,img.Result.Title,img.Result.FileSize)
                                };
                                PictureBox pb = new PictureBox { 
                                    ImageLocation =img.Result.MediaUrl
                                    ,Dock= DockStyle.Fill
                                };
                                
                                frm.Controls.Add(pb);
                                frm.Show();
                            };

                            pnlImages.Controls.Add(img);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Herhangibir sonuç bulunamadı", "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    #endregion
                }
            }
            else
            {
                MessageBox.Show("Lütfen aradığınız resim ile ilişkili bir bilgi giriniz","Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
```

İlk olarak LiveSearchService nesnesi örneklenir. Bu örnek tahmin edileceği üzere Search operasyonunu yerine getirecek olan proxy tipimizdir. Diğer yandan arama işlemi için başlangıç kriterlerinin belirtilmesi gerekir. Bu amaçla SearchRequest tipinden bir nesne örneği oluşturulmaktadır. Dikkat edileceği üzere Image tipinden bir arama istendiği belirtilmiş ve buna göre Image özelliğine yeni bir ImageRequest nesnesi atanmıştır. ImageRequest nesnesinde 20 resimlik bir sonuç kümesinin talep edildiği belirtilmektedir. SearchRequest sınıfı örneklenirken App Id değeri verilmektedir.

Hatırlayınız, bu değeri siz formu doldurduktan sonra alıyorsunuz. Önemli atamalardan biriside Query özelliği için yapılandır. Bu özelliğin değeri aranacak içeriği taşımaktadır. Bundan sonrası son derece kolaydır. LiveSearchService nesne örneğinin Search metoduna parametre olarak SearchRequest referansı atanır. Sonuçlar SearchResponse nesne örneğine gelir. Ardından SearchResponse nesne örneğinin Image özelliğinin Results koleksiyonundaki her bir ImageResult değerlendirilerek resim bilgilerinin alınması sağlanır. Elde edilen sonuçların her biri için bir ThumbImage bileşeni oluşturulur ve FlowLayoutPanel bileşeninin Controls koleksiyonuna eklenir. Uygulamanın çalışma zamanındaki örnek çıktısı aşağıda görüldüğü gibidir. Ben Ferrari kelimesi ile ilişkili resim dosyalarını arattım

![Cool](/assets/images/2009/smiley-cool.gif)

.

![blg72_SampleRuntime.gif](/assets/images/2009/blg72_SampleRuntime.gif)

Görüldüğü gibi minik resimlerden herhangirine tıklandığında orjinal halide yeni bir Form içerisinde gösterilebilmektedir. Buna ek olarak resim ile ilişkili bir kaç basit bilgide Form'un başlığında gösterilmektedir. Resmin boyutları, başlığı ve büyüklüğü. Ne kadar basit öğle değil mi?

![Wink](/assets/images/2009/smiley-wink.gif)

Bu arada Bing API ile ilişkili dökümanı indirdiğinizde içerisinde JSON, XML ve SOAP modellerinin her biri için ayrı ayrı yapılmış detaylı örnek anlatımları ve projeler olduğunu göreceksiniz. Bunları incelemenizi şiddetle tavsiye ederim. Peki bu acele örnekte yapmadıklarımız?

- Exception kontrolü (Örneğin bağlantı problemelerinde yada resmin elde edilememesinde yaşanabilecek sıkıntıları handle etmek gerekir)
- Asenkron arama metodu uygulanabilir ama bu durumda Illegal Cross Thread Exception hatasından kaçınmak gerekir.

Bunlarda size görev olsun. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.