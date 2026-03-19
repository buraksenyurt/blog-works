---
layout: post
title: "Rest Tabanlı WCF Servisleri için İstemci Yazmak"
date: 2009-04-23 12:12:00 +0300
description: "Bir önceki blog yazımızda, koleksiyon bazlı WCF servislerinin REST modeline göre geliştirilmesini incelemeye çalışmış ve REST Starter Kit' in sağladığı kolaylıklara değinmiştik. Belkide yazının en zor kısımlarından biriside CUD(CreateUpdateDelete) işlemlerinin test edilmesiydi..."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - dotnet
  - aspnet
  - linq
  - windows-forms
  - xml
  - soap
  - rest
  - web-service
  - http
  - serialization
  - generics
  - visual-studio
---
# Rest Tabanlı WCF Servisleri için İstemci Yazmak
Merhaba Arkadaşlar,

Bir önceki blog [yazımızda](https://www.buraksenyurt.com/post/Koleksiyon-Bazl%C4%B1-WCF-Rest-Servisleri), koleksiyon bazlı WCF servislerinin REST modeline göre geliştirilmesini incelemeye çalışmış ve REST Starter Kit'in sağladığı kolaylıklara değinmiştik. Belkide yazının en zor kısımlarından biriside CUD (CreateUpdateDelete) işlemlerinin test edilmesiydi. Nitekim burada istemciden gönderilecek Request paketlerinin HTTP protokolünün uygun olan POST, PUT, DELETE metodlarından birisine göre hazırlanıp iletilmesi gerekmekteydi. Bu nedenle, Fiddler aracını kullanarak talepleri oluşturmuş ve testleri gerçekleştirmiştik. Aslında, sadece veri çekilmesi işleminde (HTTP GET) işimiz nispeten çok daha kolay olmaktadır. Basit bir tarayıcı uygulama bu iş için yeterlidir. Peki ya istemci, bir geliştirici tarafından yazılacak ve söz konusu REST bazlı koleksiyon servisini tüketecek bir uygulama olacaksa...

![Undecided](/assets/images/2009/smiley-undecided.gif)

Bir geliştirici olarak olayı son derece basit bir şekilde düşünebiliriz. Nitekim Fiddler veya Internet Explorer gibi bir tarayıcının yaptıkları, içeride gereki HTTP paketinin hazırlanması ve karşı tarafa gönderilmesidir. Çok çok eskiden galaksinin uzak bir diyarında (a long time ago in a galaxy far far away), Web servislerinin kullanılmasında SOAP paketlerinin manuel olarak nasıl hazırlanıp gönderilebileceğini ve servisten dönen cevapların nasıl ele alınabileceğini incelemiştim (2006 yılında). O örnekte de SOAP zarflarının (SOAP Envelope).Net tipleri yardımıyla manuel olarak hazırlanıp gönderilmesi söz konusuydu. E tabi aradan yıllar geçer, WCF gibi çok güçlü bir SOA (Service Oriented Architecture) çözümü ortaya çıkar.

Bildiğiniz gibi.Net Framework 3.5 ile birlikte WCF'in kazandığı web programlama modeli sayesinde de, REST bazlı geliştirmelerin yapılabilmesi olanaklı hale gelmiştir. Ayrıca, WCF Rest Starter Kit ile işlemlerin biraz daha kolaylaştırılması mümkündür. İstemci tarafını geliştirirkende bu kit ile birlikte gelen yardımcı tipler ve belkide en önemlisi genişletme metodlarından (Extension Methods) yararlanılmaktadır. Öyleyse yeni bir maceraya yelken açalım ve bir önceki yazımızda geliştirdiğimiz WCF Rest Collection Service projesi içerisinde yayımlanan hizmeti, basit bir WinForms uygulamasından tüketmeye çalışalım.

Bu amaçla ilk olarak bir Windows projesi açarak yola koyuluyoruz. Sonrasında ise projemiz için gerekli olan bazı referansları eklememiz gerekiyor.

![blg6_2.gif](/assets/images/2009/blg6_2.gif)

Burada görülen Microsoft.Http.dll, Microsoft.Http.Extension.dll ve Microsoft.ServiceModel.Web.dll assmebly'lar, WCF Rest Stater Kit ile birlikte gelen ve istemci tarafından REST bazlı WCF servislerinin tüketilmesinde kullanılan pek çok yardımcı tipi ve üyeyi içermektedir. Bu muhakkakki geliştrici olarak bizleri sevindiren bir gelişmedir.

![Cool](/assets/images/2009/smiley-cool.gif)

Sonraki adımda ise servis tarafından yayımlanan Product tipi ve buna ait örnekleri içeren koleksiyon bazlı listenin istemci tarafında bir şekilde temsil edilmesi gerekmektedir. Nitekim, istemciden gidecek talep sonrası (örneğin tüm ürün listesinin istenmesi) servisten gelecek cevap içeriği XML tabanlı olacaktır ve kod tarafında kolay bir şekilde yönetilebilmesi arzu edilir. İşte bu noktada da WCF Rest Starter Kit kurulumu sonrası Visual Studio 2008' e eklenen Paste XML as Types menü seçeneği dikkati çekmektedir. Aslında yapacağımız tek şey, istemci tarafında boş bir namespace oluşturmak (adını NorthwindV2 olarak verebiliriz), servisi bir kere kullanıp tüm ürün listesini istedikten sonra üretilen XML içeriğini tamamıyla kopyalamak ve Paste XML as Types menü seçeneği ile yapıştırmaktır.

![Laughing](/assets/images/2009/smiley-laughing.gif)

Bunun sonucunda istemci uygulama tarafında aşağıdaki sınıf diagramında görülen tipler otomatik olarak oluşturulacaktır.

![blg6_3.gif](/assets/images/2009/blg6_3.gif)

Görüldüğü gibi istemci tarafında, servisten gelen koleksiyon bazlı içeriği yönetimli kod (Managed Code) tarafında temsil edebilmemiz için gerekli tüm tipler oluşturulmuştur. Ama Oz büyücüsünün yardımları sadece buraya kadardır. Artık developer olarak bizim direksiyonun başına geçmemiz gerekiyor. Peki ama neden? Öncelikli olarak amacımızı belirtelim. İlk etapta tüm ürün listesini istemci uygulama tarafına çekebilmek istiyoruz. Bu amaçla aşağıdaki ekran görütüsüne sahip basit bir WinForm ' umuz olduğunu düşünelim.

![blg6_4.gif](/assets/images/2009/blg6_4.gif)

Form üzerindeki Button kullanıldığında, servis tarafından talep edilen ürün listesini DataGridView kontrolünde göstermeyi ilk hedefimiz olarak seçebiliriz. Burada önemli olan noktalardan birisi talebin (Request) oluşturulması, servise GET metoduna göre gönderilmesi ve gelen cevap (Response) içerisinde yer alan XML içeriğinin yönetimli kod tarafında ItemInfoListItemInfoItem tipine kadar indirgenebilmesidir. Öyleyse kod içeriğini aşağıdaki gibi oluşturalım.

```csharp
using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Http;
using NorthwindV2;

namespace NorthwindClient
{
    public partial class Form1 : Form
    {
        // İstemci talepleri için kullanacağımız sınıf HttpClient
        HttpClient client = null;
        // Servis adresi (sondaki / işaretini unutmadım bu sefer)
        string serviceUri="http://localhost:1000/Service.svc/";

        public Form1()
        {
            InitializeComponent();
            // HttpClient nesnemizi örnekliyoruz
            client = new HttpClient();            
        }

        private void btnGetProducts_Click(object sender, EventArgs e)
        {
            // IDisposable interface' ini implemente eden HttpResponseMessage nesne örneğinin içeriğinin HttpClient tipinin Get metodu yardımıyla elde ediyoruz. Get metodu parametre olarak servis adresini almakta.
            using (HttpResponseMessage response = client.Get(serviceUri))
            {                
                // Eğer servis tarafından Http 200(yani OK) cevabı gelirse işlemler devam edecektir. Aksi durumda istisna mesajı fırlatılacaktır.
                response.EnsureStatusIs(HttpStatusCode.OK);

                // cevap olarak gelen paket içeriği XML formatlıdır. Bu içeriğin ItemInfoList tipine cast edilmesi ve yönetimli kod ile ele alınabilmesi için ReadAsXmlSerializable<T> generic metodu kullanılır.
                ItemInfoList products=response.Content.ReadAsXmlSerializable<ItemInfoList>();

                // ilk olarak tüm ürünleri listeleriz. ToList metoduna generic parametre olarak, servis tarafındaki Product tipinin istemcide otomatik üretilen karşılığı olan ItemInfoListItemInfoItem sınıfı verilmiştir, buna dikkat edelim.
                grdProducts.DataSource = (from p in products.ItemInfo
                                          select p.Item).ToList<ItemInfoListItemInfoItem>();               
            }
        }
    }
}
```

Burada dikkat edilmesi gereken noktalardan biriside HttpClient tipine ait nesne örneği yardımıyla Get metodunun kullanılışıdır. Parametrenin servis adresini gösteriyor olması aslında, HTTP Get metoduna göre http://localhost:1000/Service.svc/ adresine bir talep gönderiliyor olması anlamına gelmektedir. Eğer istemci tarafından Post, Put veya Delete talepleri gönderilmek isteniyorsa yine HttpClient nesne örneği üzerinden aynı isimli metodlar kullanılabilir. Bu metodlar aslında aşağıdaki şekildende görüldüğü üzere,

![blg6_8.gif](/assets/images/2009/blg6_8.gif)

HttpMethodExtension sınıfı içerisinde yazılmış genişletme metodlarıdır.

Form uygulamamızı ilk haliyle çalıştırıp testlerimize başlayabilir. Tabi burada küçük bir ayrıntıyı gözden kaçırmamak gerekiyor. Servis örneğinin çalışıyor olmasına dikkat etmeliyiz. Örnekte geliştirdiğim NorthwindV2 servis uygulaması, Asp.Net Development Server üzerinden Host edildiğinden manuel olarak çalıştırılıyor olması gerekebilir. İşte ilk sonuçlar;

![blg6_5.gif](/assets/images/2009/blg6_5.gif)

Görüldüğü üzere tüm Product bilgileri istemci tarafına gelmiştir. Tabiki koleksiyon içeriğini istemci tarafına indirdikten sonra sorgularıda istediğimiz şekilde değiştirebiliriz. Örneğin;

```csharp
 grdProducts.DataSource = (from p in products.ItemInfo
                                          where p.Item.CategoryID == 1
                                          select p.Item).ToList<ItemInfoListItemInfoItem>();
```

kodunu denediğimizde kategorisi 1 olan ürünlerin getirilmesi sağlanacaktır.

![blg6_6.gif](/assets/images/2009/blg6_6.gif)

Hatta istersek anoymous type (isimsiz tip) kullanımıda söz konusu olabilir. Söz gelimi

```csharp
grdProducts.DataSource = (from p in products.ItemInfo
                                          where p.Item.CategoryID == 1
                                          select new
                                          {
                                              Key = p.Item.ProductID,
                                              p.Item.ProductName,
                                              p.Item.CategoryID,
                                              p.Item.UnitsInStock
                                          }).ToList();
```

kodu ile sadece ProductID, ProductName, CategoryID ve UnitsInStock alanlarını içeren bir isimsiz tip topluluğunu DataGridView içerisinde gösterilmesi sağlanabilir.

![blg6_7.gif](/assets/images/2009/blg6_7.gif)

Gayet kolay gördüğünüz gibi. Artık hedefimiz Post, Put ve Delete metodlarını istemci tarafından gönderip ele alabilmek. Yazıyı sonlandırmadan önce aslında bu modelin ne gibi bir farkı olduğuna bakmakta yarar var. Dikkat edileceği üzere istemci tarafı için ürettiğimiz herhangibir Proxy tipi bulunmamaktadır. (Hiç Add Service Reference dediğimi duydunuz mu?

![Wink](/assets/images/2009/smiley-wink.gif)

)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.