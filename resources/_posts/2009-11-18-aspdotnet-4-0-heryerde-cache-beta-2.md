---
layout: post
title: "Asp.Net 4.0 - Heryerde Cache [Beta 2]"
date: 2009-11-18 04:20:00 +0300
categories:
  - aspnet-4-0-beta-2
tags:
  - asp.net
---
Kronometrelerinizi hazır edin! Bu yazımızda Absolute ve Sliding Expiration modelinde ön bellekleme işlemlerini Windows tabanlı bir uygulama üzerinde gerçekleştiriyor olacağız. Durun bir dakika...Windows mu? Evet evet yanlış duymadınız Windows. Aslında başlıktaki konu ile tamamen tezat bir durum. Gerçekten de öyle mi acaba? Gelin şu meseleyi açıklığa kavuşturalım.

![blg103_Giris.jpg](/assets/images/2009/blg103_Giris.jpg)

![Smile](/assets/images/2009/smiley-smile.gif)

Ön bellekleme işlemleri ile performans arttırımı çoğunlukla web uygulamalarında akla gelen bir konudur. Ancak gerçek hayat uygulamaları sadece Web tabanlı değildir. Windows Forms, WPF gibi masaüstü uygulamalarından tutunda, katmanları ifade eden Class Library'lere kadar çok çeşitli ürünler yer almaktadır. Dolayısıyla performans kazanımı, iş yükünün hafifletilmesi için ön bellekleme işleminin sadece Web uygulamalarına bağımlı kalması düşünülemez. Peki Web ortamı dışında ön bellekleme (Caching) teknikleri için hangi imkanlar bulunmaktadır?

Aslında listenin başında System.Web.Caching.dll assembly'ının Web dışındaki uygulamalara referans edilerek kullanılmasının yer aldığını söyleyebiliriz. Ne varki bir Windows uygulamasına Web alanına ait bir Assembly'ın referans edilmeside son derece gariptir.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Daha etkili bir yöntem olarak Enterprise Library içerisinde yer alan Caching bloğunun kullanılması tercih edilebilir. Bunların haricinde henüz incelediğim ve yakında sizinle ilk bilgilerimi paylaşacağım Velocity projesi de yer almaktadır ki dağıtık ön bellekleme (Distributed Caching) ve dolayısıyla Cache üzerinde Load Balancing vb imkanlar sunmakta olan bir projedir. Ancak Asp.Net 4.0' ın duyurulması ile birlikte özellikle Overivew dökümünında belirtilen yeni bir koz daha bulunmaktadır. System.Runtime.Caching.dll assembly'ı. Biz bu gün geliştireceğimiz örnekte ObjectCache tipinden yararlanarak ön bellekleme işlemlerini yapmaya çalışacağız.

![blg103_Browse.gif](/assets/images/2009/blg103_Browse.gif)

Bu assembly içerisinde yer alan tiplerden yararlanarak özel Cache sağlayıcılar, tipler geliştirebileceğimiz gibi In-Memory ön bellekleme yeteneklerini web dışındaki uygulamalarda da kullanabiliriz. Üstelik.Net Framework içerisine dahil edilmiş olması, Enterprise Library gibi üçüncü parti kurulumlara ihtiyaç duymayışı, kullanımının Web tarafındaki Cache nesnesi ile neredeyse aynı olmasıda avantaj olarak görülebilir. Bu yazımızda söz konusu ön bellekleme tekniklerine sadece iki açıdan bakıyor olacağız. Sliding ve Absolute Expiration tipli ön bellekleme işlemleri.

Bilindiği üzere ön bellekte tutulacak olan verinin ne kadar süre ile duracağının belirtilmesi Absolute Expiration tekniğini ilgilendiren bir meseledir. Diğer yandan belirtilen süre içerisinde ön bellekte tutulan Cache nesnesine talep gelmesi sonrası, o andan itibaren tekrardan belirtilen süre kadar yaşam ömrünün uzatılması da Sliding Expiration tekniğini ilgilendiren bir konudur. Bunlara ek olarak ön belleklemede popüler olarak kullanılan bağımlılık (Dependency) gibi tekniklerde bulunmaktadır. Buna görede Cache nesnesinin içeriğinin tazelenmesinin bir sebebe bağımlı hale getirilmesi sağlanabilir. Bu sebep basit bir dosyadaki değişiklik olabilir örneğin.

Dilerseniz hiç vakit kaybetmeden Visual Studio 2010 Ultimate Beta 2 üzerinde basit bir Windows uygulması geliştirerek yolumuza devam edelim. Örneğimizde Product tipinden nesne örnekleri tutan generic bir List koleksiyonunun içeriğinin Absolute ve Sliding olarak ön belleklenmesi işlemlerini göz önüne alıyor olacağız. Tabi ilk etapta System.Runtime.Caching assembly'ının Win Forms uygulamasına referans edilmesi gerekmektedir.

![blg103_AddReference.gif](/assets/images/2009/blg103_AddReference.gif)

WinForms uygulamamızın basit ekran görüntüsü ise aşağıdaki gibidir.

![blg103_Form.gif](/assets/images/2009/blg103_Form.gif)

Gelelim kodlarımıza;

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Windows.Forms;

namespace UsingObjectCache
{
    public partial class Form1
        : Form
    {
        // Cache nesnelerine erişmek için kullanılan tip tanımlanır
        ObjectCache cachedObject;

        public Form1()
        {
            InitializeComponent();

            // Varsayılan MemoryCache referansı elde edilir
            cachedObject = MemoryCache.Default;
        }

        // Cache üzerinden Products Key değerine sahip içeriği getirmek için kullanılan olay metodudur
        private void btnGet_Click(object sender, EventArgs e)
        {
            // Eğer ProductList Key değerine sahip ön belleklenmiş bir içerik var ise
            if (cachedObject["Products"] != null)
            {
                // Indeksleyici yardımıyla ilgili Cache nesnesi çekilir
                lstProducts.DataSource = cachedObject["Products"] as List<Product>;
                lblInformation.Text = "Ürün listesi içeriği ön bellekten getirildi";
            }
            else
            {
                lblInformation.Text = "Cache içerisinde söz konusu liste yok\n Bilgiler tekrardan üretilecek";
                lstProducts.DataSource = CreateProductList();
            }
        }

        // Cache' lenen veriyi belirli bir süreliğine tutmak için gerekli ayarlamaları yapan olay metodudur.
        private void btnAbsoluteExpiration_Click(object sender, EventArgs e)
        {
            // Zaten Products isimli bir Cache içeriği yoksa, bu Key değerine sahip olan bir Cache içeriği oluşturulur ve o andan itibaren 30 saniye süreyle ön bellekleneceği belirtilir.
            if (cachedObject["Products"] == null)
                if (cachedObject.Add("Products", CreateProductList(), new DateTimeOffset(DateTime.Now.AddSeconds(30))))
                    lblInformation.Text = "Absolute Expiration(30 Seconds)";
        }

        // Kayan süreli Cache' leme için gerekli işlemleri yapan olay metodudur.
        // Buna göre 30 saniyelik süre dolmadan gelen talepler, Cache' te tutulmas süresini o andan itibaren tekrar 30 saniye ileri götürürler
        private void btnSlidingExpiration_Click(object sender, EventArgs e)
        {
            // Absolute Expiration, Sliding Expiration, Dependency gibi Cache çeşitlerini istersek ilke(Policy) olarak tanımlayabiliriz.
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = TimeSpan.FromSeconds(30);

            if (cachedObject["Products"] == null)
                if (cachedObject.Add("Products", CreateProductList(), policy))
                    lblInformation.Text = "Sliding Expiration(30 Seconds)";
        }

        // Maliyetli olduğu varsayılan ve içeriği ön bellekte tutulabilecek olan sembolik bir fonksiyonellik
        private List<Product> CreateProductList()
        {
            return new List<Product>
                {
                    new Product{ ProductId=10, Name="Bardak", ListPrice=1.23M},
                    new Product{ ProductId=11, Name="Tabak", ListPrice=1.25M},
                    new Product{ ProductId=12, Name="Çatal", ListPrice=0.55M}
                };
        }
    }

    // Yardımcı tip
    class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal ListPrice { get; set; }

        public override string ToString()
        {
            return String.Format("{0}|{1} {2}", ProductId.ToString(), Name, ListPrice.ToString("C2"));
        }
    }
}
```

Cache yönetimi için MemoryCache.Default özelliğinin ürettiği ObjectCache nesne referansı kullanılmaktadır. ObjectCache tipinin indeksleyicisinden yararlanılarak ön bellekte tutulan bir Key değerine ve doğal olarak işaret ettiği Object tipinden içeriğe ulaşmak mümkündür. Diğer yandan Add ve Set gibi metodlar yardımıyla ön belleğe veri atılmasıda sağlanabilir. Burada dikkat çekici noktalardan biriside ön bellekleme tipi için ilkelerden (Policy) yararlanılabilmesidir. Örneğimizde Sliding Expiration için CacheItemPolicy nesne örneği tanımlanmıştır. Bu nesne örneği aslında ön bellekleme ilkesini ifade etmektedir. Önbelleğe atılacak nesne içeriğinin bu ilkeye göre tutulacağını belirtmek içinse Add metoduna parametre olarak verilmesi yeterli olmuştur. Aslında Add metodunun aşırı yüklenmiş versiyonlarından birisi de Absolute Expiration tekniğine göre parametre almaktadır ki buda örneğimizde kullanılmıştır.

Gelelim testlerimize. Hatırlarsanız kronometrelerinizi hazır tutmanızı söylemiştim. Bu tabiki işin şakası

![Wink](/assets/images/2009/smiley-wink.gif)

Nitekim kodun içerisinde de test amaçlı olarak bir kronometre kullanılabilir. Çalışma zamanındaki testlerimizi aşağıdaki adımlarda olduğu gibi geliştirelim.

1 - Uygulama başlatıldıktan sonra Get başlıklı düğmeye basılır ve aşağıdaki ekran görüntüsü ile karşılaşılır.

![blg103_Test1.gif](/assets/images/2009/blg103_Test1.gif)

Get içerisinde yapılan çağrıda ObjectCache içerisinde Products isimli bir Key bulunmadığından ürün listesinin maliyeti yüksek olduğu düşünülen bir metod ile üretilmesi gerçekleştirilir.

2 - 1nci testten sonra Absolute Expiration veya Sliding Expiration düğmelerinden birisi kullanılır. Absolute Expiration başlıklı düğme tıklandığında aşağıdaki ekran görüntüsü ile karşılaşılmalıdır.

![blg103_Absolute.gif](/assets/images/2009/blg103_Absolute.gif)

Buna göre Product listesi ön bellekte 30 saniye süreyle saklanacaktır. Süre dolmadan Get düğmesine basılırsa aşağıdaki ekran görüntüsü ile karşılaşılmalıdır.

![blg103_AbsoluteTest.gif](/assets/images/2009/blg103_AbsoluteTest.gif)

Şimdi 30 saniyelik süre sona erdikten sonra tekrar Get düğmesine basılırsa artık ön bellekte bir veri tutulmadığından ürün listesi üretim işleminin tekrar yapıldığı görülmelidir.

3 - 2nci testin bitmesinden sonra seçiminize göre diğer ön bellekleme tekniğini ele alabilirsiniz. Benim sırama göre şimdi Sliding Expiration testinin yapılması gerekmektedir. Program açık iken Sliding Expiration başlıklı düğmeye asarsanız aşağıdaki ekran görüntüsü ile karşılaşırsınız.

![blg103_Sliding.gif](/assets/images/2009/blg103_Sliding.gif)

Bu adımdan sonra 30 saniyelik süre dolmadan tekrar Get düğmesine basarsanız içeriğin ön bellekten getirildiğini görebilirsiniz. Ancak önemli olan nokta şudur; Ön bellekte durma süresi, 30 saniyelik süre içerisinde Get düğmesine bastığınız andan itibaren 30 saniye sonrasına uzamasıdır.

4 - Uygulamayı herhangibir Cache tekniği uygulandıktan sonra ilgili süreler aşılmadan kapatıp tekrar açınız ve yine Get düğmesine basınız. Bu durumda içeriğin ön bellekten değil tekrardan üretim ile geldiğini görmelisiniz ki bu son derece doğaldır. Çünkü uygulama sonlandırılmış ve kendisi için ayrılan bellek içeriği bir sonraki uygulama örneği için geçersiz hale gelmiştir. Hımmm...!!! Aslında bu çokda istediğimiz bir vaka olmayabilir.

Tüm bunlar bir yana en iyi test yöntemlerinden biriside uygulmayı Debug ederek incelemeniz olacaktır ki bunu yapmanızı şiddetle tavsiye ederim. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[UsingObjectCache.rar (42,82 kb)](/assets/files/2009/UsingObjectCache.rar)
