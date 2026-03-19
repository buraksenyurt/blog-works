---
layout: post
title: "WCF RIA Services - Gerçekten WCF"
date: 2009-11-25 23:19:00 +0300
description: "Uzun ve yorucu bir geceydi...Dün gece WCF RIA Service' leri ile ilişkili görsel bir dersin hazırlıklarını yaparken sevgili Mehmet Cengiz arkadaşımın hediyesi olan tablet üzerinde aşağıdaki şekli çizdiğimi farkettim. Bu şekilde WCF RIA Service' i kullanan basit bir Silverlight uygulamasının anlaşılır hali yer almaktadır..."
categories:
  - wcf-eco-system
  - wcf-ria-services
tags:
  - wcf-eco-system
  - wcf-ria-services
  - csharp
  - xml
  - dotnet
  - aspnet
  - ado-net
  - linq
  - wcf
  - windows-forms
  - silverlight
  - http
  - authentication
---
# WCF RIA Services - Gerçekten WCF
Merhaba Arkadaşlar,

Uzun ve yorucu bir geceydi...Dün gece WCF RIA Service'leri ile ilişkili görsel bir dersin hazırlıklarını yaparken sevgili Mehmet Cengiz arkadaşımın hediyesi olan tablet üzerinde aşağıdaki şekli çizdiğimi farkettim. Bu şekilde WCF RIA Service'i kullanan basit bir Silverlight uygulamasının anlaşılır hali yer almaktadır. Web uygulamamız, içerisinde Silverlight nesnesi barındıran test sayfası ve Domain Service sınıfı, dışarıda veya aynı alanda duran Ado.Net Entity Data Model içeriği ve onun kullandığı veritabanı. Soru işareti içeren ok ve Winforms kutucuğunu ise sonradan eklemeye karar verdim ve araştırmam da işte bu şekilde başladı

![Laughing](/assets/images/2009/smiley-laughing.gif)

![blg107_Plan.gif](/assets/images/2009/blg107_Plan.gif)

Gerçektende Silverlight uygulamlarını RIA Service'lerini kullanacak şekilde oluşturduğumuzda, genellikle Silverlight nesnesini host eden bir Web uygulaması olması gerekmektedir. Bu web uygulamasının içerisinde yer alan ve Sliverlight tarafına veri hizmeti sunan RIA Service'leri, harici bir Class Library içerisinde bulunabilen bir Entity Data Model'i (yada aynı web domain içerisindekini) kullanabilir. Bu şekli çizmeye çalışırken aklıma takılan ve soru işareti bırakan husus ise şudur; Web uygulaması dışarısında yer alan bir diğer uygulamanın, Web alanı içerisinde yer alan bir RIA Service'ini kullanması mümkün olabilir mi?

Sonuç itibariyle WCF alt yapısı üzerine oturan RIA Service'leri web tabanlı olarak host edilmekte ve hizmet sunarken WCF çalışma zamanı motoru tarafından yürütülmektedirler. Bu nedenle belkide dışarıya WSDL içeriği sunabilir ve Silverlight gibi Rich Internet Application'lar dışındaki uygulamalar tarafından da kullanılabilirler. İşte bu sorunun cevabını ararken [Brad Abrams'ın blog yazısı](http://blogs.msdn.com/brada/archive/2009/11/22/ria-services-a-domainservice-is-a-wcf-service-add-service-reference.aspx) ile karşılaştım. Brad Abrams blog yazısında Silverlight Business Application tipinden geliştirdiği bir proje üzerinden ilgili konuyu irdeleyerek RIA Service'lerin aslında birer WCF Service olduğunu ispat etmektedir. Konuyu bende bu haliyle incelemek istediğimden aynı örneğin benzerini bu kez Silverlight Application tipinden olan bir projede yapmayı denedim. Ancak çok küçük bir özelliği kullanmamam nedeni ile hata ile karşılaştım. Bu hatanın sebebini ve çözümünü bulmam biraz zamanımı aldı. Dolayısıyla Brad Abrams'ın kullandığı proje şablonu yerine normal bir Silverlight Application üzerinden benzer bir senaryoyu uygulamaya karar verdim. İşte bu tecrübenin hikayesi;

İlk olarak aşağıdaki şekilde görülen tipte bir Solution içeriği oluşturarak işe başlamaya karar verdim.

![blg107_Solution.gif](/assets/images/2009/blg107_Solution.gif)

Silverlight Application şablonunda bir Solution açılmasına rağmen aslında ChinookMusicSotreApp isimli proje hiç kullanılmayacaktır.

![Undecided](/assets/images/2009/smiley-undecided.gif)

ChinookMusicStoreApp.Web isimli web projesi, ChinookEDM isimli sınıf kütüphanesini referans etmektedir. Bu şekilde Albums ve Artist isimli Chinook tablolarına ait Entity karşılıklarını içermekte olan Ado.Net Entity Data Model öğesini kullanabilmektedir. Diğer yandan Web tarafına eklenen Domain Service sınıfı içeriğinde Album tipi üzerinden Insert, Update, Delete işlemleri yapılmasına izin verecek şekilde geliştirmeler yapılmıştır. Bu adımları tamamladıktan sonra ChinookDomainService sınıfının içeriği biraz değiştirerek aşağıdaki hale getirdim. Aslında tek yaptığım GetAlbumsByFirstLetter isimli metodu eklemek oldu.

```csharp
namespace ChinookMusicStoreApp.Web
{
    using System.Data;
    using System.Linq;
    using System.Web.DomainServices.Providers;
    using System.Web.Ria;
    using ChinookEDM;

    [EnableClientAccess()]
    public class ChinookDomainService 
        : LinqToEntitiesDomainService<ChinookEntities>
    {
        // firstLetter parametresi ile başlayan Album' lerin getirilmesini sağlar
        public IQueryable<Album> GetAlbumsByFirstLetter(string firstLetter)
        {
            return from album in ObjectContext.Albums
                   where album.Title.StartsWith(firstLetter)
                   orderby album.Title
                   select album;
        }

        // Tüm Artist' lerin getirilmesini sağlar
        public IQueryable<Artist> GetArtists()
        {
            return this.ObjectContext.Artists;
        }

        // Yeni bir Album eklenmesi için kullanılır
        public void InsertAlbum(Album album)
        {
            if ((album.EntityState != EntityState.Added))
            {
                if ((album.EntityState != EntityState.Detached))
                {
                    this.ObjectContext.ObjectStateManager.ChangeObjectState(album, EntityState.Added);
                }
                else
                {
                    this.ObjectContext.AddToAlbums(album);
                }
            }
        }

        // Bir Album' ü güncelleştirmek için kullanılır
        public void UpdateAlbum(Album currentAlbum)
        {
            if ((currentAlbum.EntityState == EntityState.Detached))
            {
                this.ObjectContext.AttachAsModified(currentAlbum, this.ChangeSet.GetOriginal(currentAlbum));
            }
        }

        // Bir Albumu silmek için kullanılır
        public void DeleteAlbum(Album album)
        {
            if ((album.EntityState == EntityState.Detached))
            {
                this.ObjectContext.Attach(album);
            }
            this.ObjectContext.DeleteObject(album);
        }
    }
}
```

Domain Service sınıfı bu şekilde hazırlandıktan sonra artık asıl işimize odaklanabiliriz. İlk hedefimiz ChinookDomainService isimli sınıfın aslında çalışma zamanı için bir WCF Service olduğunu göstermektir. Buna göre Web üzerinden svc uzantılı olarak erişilebiliyor olması gerekmektedir. Peki WCF çalışma zamanı için herhangibir yerde bir tanımlama yer almakta mıdır? Aslında web.config dosyası içeriğine bakıldığında system.ServiceModel elementinin aşağıdaki gibi eklenmiş olduğu görülecektir.

![blg107_Webconfig.gif](/assets/images/2009/blg107_Webconfig.gif)

> Önemli Not: Örneğimizde EDM'yi Web uygulamamıza referans ettiğimiz bir Class Library içerisinde tuttuğumuz için, bu kütüphanenin App.config dosyası içerisine yazılan connectionString bilgisinin, Web uygulamasının web.config dosyasına eklenmesi gerekmektedir. Aksi takdirde çalışma zamanında hata alınacaktır.

Hımmm...O halde Asp.net development server'ın çalıştırılmasını sağlayıp herhangibir tarayıcıdan örneğimize göre http://localhost:4977/ChinookMusicStoreApp-Web-ChinookDomainService.svc adresini talep ettiğimde bir Service ekranı ile karşılaşmam gerekmektedir. Oysaki bu denemenin ardından ben aşağıdaki ekran görüntüsü ile karşılaştım.

![Frown](/assets/images/2009/smiley-frown.gif)

![blg107_Error.gif](/assets/images/2009/blg107_Error.gif)

Oysaki Brad Abrams örneğinde svc uzantısından sonra servis içeriği görüntülenmiş hatta WSDL çıktısı bile elde edilebilmiştir. Acaba sorun nerededir?

Yaptığım araştırmalar sonucunda bu servise erişmek için bir Authentication ayarlamasının yapılması gerektiğini öğrendim. Bu çok doğaldı çünkü RIA Service'in kullanılmak istendiği Web Domain'i dışarısına açılması gibi bir durum söz konusuydu. Dolayısıyla web.config dosyası içerisinde system.Web elementi altında authentication için gerekli tanımlamaların yapılması gerekmekteydi. Bu örnekte herhangibir doğrulama kullanmadığımdan (Forms, Passport vb...) mode değerini None olarak bırakmayı tercih ettim.

```xml
<?xml version="1.0"?>
<configuration>

    <system.web>
        <httpModules>
            <add name="DomainServiceModule" type="System.Web.Ria.Services.DomainServiceHttpModule, System.Web.Ria, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
        </httpModules>
        <compilation debug="true" targetFramework="4.0" />
      <authentication mode="None"/>
    </system.web>
.
.
.
```

Bu işlemin ardından http://localhost:4977/ChinookMusicStoreApp-Web-ChinookDomainService.svc adresine tekrardan talepte bulunduğumda aşağıdaki görüntü ile karşılaştım.

![blg107_Runtime.gif](/assets/images/2009/blg107_Runtime.gif)

Hatta WSDL talebi sonrası söz konusu servisin Description içeriğinin de geldiğini gördüm.

![blg107_Wsdl.gif](/assets/images/2009/blg107_Wsdl.gif)

Demekki Domain Service sınıfı yada geliştirdiğimiz RIA Service gerçektende bir WCF Service'miş.

![Wink](/assets/images/2009/smiley-wink.gif)

Ancak ispatı tamamlamak için söz konusu servisi örnek bir istemcide kullanabiliyor olmam da gerekmekteydi. Bu nedenle Solution içerisinde yer alan Win Forms uygulamasına Add Service Reference ile http://localhost:4977/ChinookMusicStoreApp-Web-ChinookDomainService.svc adresinden gerekli Proxy üretiminin gerçekleştirilmesi yeterli olacaktı. Ben bu şekilde yoluma devam ettim ve aşağıdaki ekran görüntüsünde olduğu ilgili servis içeriğini izin verilen operasyonları ile (SubmitChanges metodunun da olduğuna dikkat edelim) üretilebildiğini gördüm.

![blg107_AddServiceRef.gif](/assets/images/2009/blg107_AddServiceRef.gif)

İlgili ekleme işlemi sonrasında app.config dosyası içeriğinin aşağıdaki gibi üretildiğini farkettim.

![blg107_Appconfig.gif](/assets/images/2009/blg107_Appconfig.gif)

Dikkat edileceği üzere iki adet EndPoint tanımlaması yapıldığı görülmektedir. Ben yazının bundan sonraki kısmında aşağıdaki ekran görüntüsüne sahip bir Form geliştirerek ilerlemeyi tercih etmekteyim.

![blg107_Form.gif](/assets/images/2009/blg107_Form.gif)

Aslında bir önceki yazımızda yaptığımız gibi A'dan Z'ye Button bileşenlerimiz bulunmaktadır ve herhangibirine basıldığında bu harf ile başlayan albümler listelenmektedir. Buna ek olarak yeni bir Album ekleme özelliğide bulunmaktadır. Yeni bir Album nesnesi örneklendiğinde Title ve ArtistId değerlerinin mutlaka girilmesi gerekmektedir. ArtistId içinse kullanıcının var olan Artist'lerden herhangibirini seçmesi sağlanmaktadır. İşte Form uygulamamıza ait kodlarımız.

```csharp
using System;
using System.Windows.Forms;
using ChinookClient.ChinookRef;

namespace ChinookClient
{
    public partial class Form1 
        : Form
    {
        ChinookDomainServiceClient proxy;

        public Form1()
        {
            InitializeComponent();

            proxy = new ChinookDomainServiceClient("BasicHttpBinding_ChinookDomainService");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 65; i < 91; i++)
            {
                Button btn = new Button();
                btn.Width = 24;
                btn.Height = 24;
                btn.Text = ((char)i).ToString();
                btn.Click += (snd, ea) =>
                    {
                        grdAlbums.DataSource=proxy.GetAlbumsByFirstLetter(btn.Text).RootResults;
                    };
                pnlButtons.Controls.Add(btn);
            }

            cmbArtists.DataSource=proxy.GetArtists().RootResults;
        }

        private void btnAddNewAlbum_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtTitle.Text))
            {
                Album albm = new Album
                {
                    Title = txtTitle.Text,
                     ArtistId=((Artist)cmbArtists.SelectedItem).ArtistId
                };

                ChangeSetEntry[] entry = new ChangeSetEntry[]{
                    new ChangeSetEntry {
                      Entity=albm,
                       Operation= DomainOperation.Insert
                }
                };

                proxy.SubmitChanges(entry);
            }
        }
    }
}
```

Album'lerin alfabetik olarak elde edilmesi veya Artist listesinin getirilmesi bir yana, en önemli noktalardan biriside yeni bir Album nesnesinin nasıl eklendiğidir. Dikkat edileceği üzere bu işlem için ChangeSetEntry tipinden bir dizi kullanılmıştır. Aslında Insert, Update ve Delete işlemlerinde bu tipten yararlanılmakta ve toplu olarak değişikliklerin bildirilmesi sağlanabilmektedir. Örnekte bir Album nesnesinin eklenmesi istendiğinden ChangeSetEntry oluşturulurken Operation özelliğine DomainOperation.Insert sabit değeri atanmıştır. Buna göre albm isimli Album nesne örneği için bir Insert işlemi yapılacağı vurgulanmaktadır. ChangeSetEntry dizisi içerisinde yer alan insert, update ve delete işlemlerinin servis tarafına gönderilmesi içinse SubmitChanges metodunun kullanılması yeterlidir.

> Önemli Not: Artist bilgilerinin ComboBox içerisinde görülebilmesi için istemci tarafına atılan Artist sıfınında ToString metodu override edilmiştir.
> Bilindiği üzere Windows Forms uygulamalarında List kontrollerinin içeriklerine Object türünden herhangibir referans atanabilmektedir. Örneğimizde de Artist nesne örneklerinin atanması söz konusudur. Ancak ComboBox içerisindeki öğelerde hangi bilgilerin görüneceği ToString metodunun ezilmesi ile sağlanabilir. Tabi bu durumda bir sorunda ortaya çıkmaktadır. Service'te yapılacak değişiklikler sonrasında istemci tarafındaki referans güncellenirse Entity sınıfları tekrardan oluşturulacağından ezilen (Override) ToString metodunun uçacağı görülecektir. Bu duruma dikkat etmek gerekir.

İşte çalışma zamanına ait örnek bir görüntü.

![blg107_NewAlbum.gif](/assets/images/2009/blg107_NewAlbum.gif)

Test olarak Van Halen isimli grub için Benim Şarkılarım isimli yeni bir albüm eklenmeye çalışılmıştır. Bu işlemin ardınan SQL tarafına bakıldığında Album isimli tabloya da ilgili satırın eklendiği gözlemlenebilir.

![blg107_Sql.gif](/assets/images/2009/blg107_Sql.gif)

İşte bu kadar. WCF RIA Service'ler ile ilişkili araştırmalarıma devam etmekteyim. Yeni bilgiler edindikçe sizlerle paylaşıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[ChinookMusicStoreApp.rar (590,82 kb)](/assets/files/2009/ChinookMusicStoreApp.rar)

![blg107_Override.gif](/assets/images/2009/blg107_Override.gif)