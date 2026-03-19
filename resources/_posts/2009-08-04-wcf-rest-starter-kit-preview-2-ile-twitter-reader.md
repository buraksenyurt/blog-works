---
layout: post
title: "WCF Rest Starter Kit Preview 2 ile Twitter Reader"
date: 2009-08-04 23:47:00 +0300
description: "Örneğin, XML içeriğini managed tarafta kolayca ele alabilmemiz için gerekli tiplerin üretimini kolaylaştıran Paste XML As Types Laughing Örneği geliştirebilmek için çok sık kullanmasamda Twitter' da bir hesap oluşturdum ve bildiğim geliştiricilerin Tweet' lerini takip etmeye başladım. İşe başlamadan önce, Twitter'da ne olup bittiğine bir bakayım dedim."
categories:
  - wcf
tags:
  - wcf
  - csharp
  - dotnet
  - wpf
  - xml
  - rest
  - http
  - serialization
  - visual-studio
---
# WCF Rest Starter Kit Preview 2 ile Twitter Reader
![blg56_giris.gif](/assets/images/2009/blg56_giris.gif)

Merhaba Arkadaşlar,

Minik bir çocukken Televizyon bağımlılığı (Malesef bu aptal kutuda çok vakiy geçirebiliyor çocuklar ![Frown](/assets/images/2009/smiley-frown.gif)) nedeniyle pek çok çizgi filmi izlediğimi hatırlıyorum. Voltran, Transformers, Red Kit ve Daltonlar, Denver The Last Dinasour, Clementine filan derken arada sıradada "Bi kedi gördüm sanki" diyen Tweety

![Laughing](/assets/images/2009/smiley-laughing.gif)

Şimdi bu konuya nereden geldiğimi düşünebilirsiniz. Şu sıralarda sık sık takip ettiğim geekswithblogs sitesinde twitter'da (Sanırım niye cikcik diyerek yazımıza başladığımızı anlamışsınızdır ![Laughing](/assets/images/2009/smiley-laughing.gif)) yayınlanan içeriklerin WCF Rest Starter Kit Preview 2 ile birlikte gelen HttpClient sınıfı yardımıyla nasıl kolayca ele alınabileceğine dair bazı yazılar gördüm.

Konunun içerisinde REST bazlı iletişim ve WCF söz konusu olunca hemen kolları sıvadım ve Windows tabanlı basit bir örnek geliştirmeye karar verdim. Tabi başlamadan önce projemizin amacından biraz bahsetmek isterim. [Twitter](http://twitter.com/)üzerinde yayınlanan girişleri HTTP üzerinden GET metodu ile çekmeyi, buna göre eklenen güncel içerikleri uygulamamızda göstermeyi ve yenilerinide kendi Twitter hesabımız üzerinden, HTTP Post metodu ile ekleyebilmeyi planlıyoruz. Aslında olay bir RSS Reader yazmak kadar basit. Diğer yandan burada bahsettiğimiz işlevsellikleri geliştirmek için elimizde WCF Rest Starter Kit Preview 2 olmasına da gerek yoktur. Ancak Kit'in bize sağladığı bazı avantajlar ve kolaylıklar bulunmaktadır.

Örneğin, XML içeriğini managed tarafta kolayca ele alabilmemiz için gerekli tiplerin üretimini kolaylaştıran Paste XML As Types ![Laughing](/assets/images/2009/smiley-laughing.gif) Örneği geliştirebilmek için çok sık kullanmasamda Twitter'da bir hesap oluşturdum ve bildiğim geliştiricilerin Tweet'lerini takip etmeye başladım. İşe başlamadan önce, Twitter'da ne olup bittiğine bir bakayım dedim.

![blg56_Twitter.gif](/assets/images/2009/blg56_Twitter.gif)

İşte buradaki içeriği Windows Uygulamasına çekmeyi hedefliyoruz. Rest Starter Kit'in nimetlerinden yararlanabilmek için, Windows uygulamasını oluşturulduktan sonra, WCF Rest Starter Kit Preview 2 ile birlikte gelen Microsoft.Http ve Microsoft.Http.Extensions assembly'larının projeye referans edilmesi gerekmektedir.

![blg56_WindowsReferences.gif](/assets/images/2009/blg56_WindowsReferences.gif)

Referansların eklenmesinden sonra yolumuza, Twitter'da yayınlanan Feed içeriğinin managed taraftaki karşılıklarını oluşturarak devam edebiliriz. Bu noktada, [http://twitter.com/statuses/friends_timeline.xml](http://twitter.com/statuses/friends_timeline.xml) adresinde keni twitter hesabım ile baktığımda aşağıdaki ekran görüntüsünde yer alan XML içeriği ile karşılaştığımı gördüm.

![blg56_TwitterTimelineXML.gif](/assets/images/2009/blg56_TwitterTimelineXML.gif)

Bu içeriği sayfanın View Source kısmını kullanarak kopyalayıp, Paste XML as Types seçeneğini yardımıyla (WCF Rest Starter Kit Preview 2 ile Visual Studio 2008 ortamına eklenmiştir) yönetimli kod tarafına dönüştürebiliriz. (Burada XML içeriğinden managed tipleri oluştururken dikkat edilmesi gereken noktalardan birisi, https değil http ile talepte bulunmamızdır. Aksi durumda View Source seçeneği çalışmamaktadır.) Diğer taraftan bu adres talebi sonrası var olan twitter hesabımız ile giriş yapmamız gerekmektedir. İşlem başarılı bir şekilde tamamlandığında sınıf diagramında aşağıdaki tiplerin oluştuğunu gözlemledim.

![blg56_ClassDiagram.gif](/assets/images/2009/blg56_ClassDiagram.gif)

Harika! Görüldüğü üzere XML içeriğinin karşılığı olan sınıflar başarılı bir şekilde oluşturulmuştur. Dikkat edilmesi gereken noktalardan birisi, XML deki Child Node bağlantılarının sınıf bazında nasıl ifade edildiğidir. Örneğin statuses sınıfı içerisinde statusesStatus tipinden bir dizi olarak tanımlanmış status özelliği... Bu özelliği kod tarafında değerlendirerek, tweet içeriğini giren User'a dahi ulaşabiliriz. Nitekim bunun için statusesStatus sınıfı içerisinde, statusesStatusUser tipinden user isimli bir özellik tanımlanmıştır. Zaten işin en önemli kısımlarından biriside bu XML içeriğinin, kod tarafınaki ifade şekli değil midir? Teşekkürler Paste Xml As Types

![Laughing](/assets/images/2009/smiley-laughing.gif)

Managed tiplerde oluşturulduğuna göre artık arka plan kodlarımızı geliştirebiliriz. (Hemen şunu hatırlatalım.Paste Xml As Types ile üretilen sınıf ve üyelerinin adlarını dilediğiniz gibi değiştirebilirsiniz. Özellikle yazım standartlarına uygun-CamelCasing isimlendirmeler yapılmasında yarar vardır. Şu an örneği hızlı bir şekilde geliştirme istediğinde bu noktaları atlamış bulunuyorum) Windows Form'unu aşağıdaki gibi tasarlayabiliriz.

![blg56_Form.gif](/assets/images/2009/blg56_Form.gif)

ve kodlarımız;

```csharp
using System;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;  // ReadAsXmlSerializable<> genişletme metodunun çıkması için eklenmelidir.
using Microsoft.Http;// HttpClient için gerekli olan isim alanı.
using System.Drawing;

namespace TwitterReader
{
    public partial class Form1 : Form
    {
        // Siz kendi Twiteer kullanıcı adı ve şifrenizi kullanmalısınız.
        private string username = "sizin kullanıcı adınız"; 
        private string password = "sizin şifreniz";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetFeeds_Click(object sender, EventArgs e)
        {
            try
            {
                pnlFeeds.Controls.Clear();

                // Öncelikle HttpClient nesne örneği oluşturulur.
                HttpClient client = new HttpClient();
                // Xml verisini yukarıdaki adresten çekebilmek için geçerli bir Twitter hesabı ile erişmemiz gerekecektir. Bu nedenle NetworkCredential oluşturulur ve Credentials koleksiyonuna eklenir.
                client.TransportSettings.Credentials = new NetworkCredential(username, password);
                ServicePointManager.Expect100Continue = false;

                // XML içeriğine, HttpClient nesne örneği üzerinden GET talebinde bulunulur. Sonuç HttpResponseMessage nesne örneğine aktarılır.
                HttpResponseMessage response = client.Get("http://twitter.com/statuses/friends_timeline.xml");
                // İşlemin başarılı olunduğundan emin olunması sağlanır. HTTP 200 OK Kontrolu
                response.EnsureStatusIsSuccessful();

                // XML İçeriği okunur ve statuses tipinden nesne örneğine aktarılır.
                statuses stats = response.Content.ReadAsXmlSerializable<statuses>();

                // statuses nesne örneğinin status özelliği ile işaret edilen statusesStatus tipinden dizinin her bir elemanı dolaşılır.
                foreach (statusesStatus s in stats.status)
                {
                    // Örnek içeriği göstermek amacıyla her bir statusesStatus için birer Label üretilir

                    Label lbl = new Label();
                    lbl.AutoSize = false;
                    lbl.Width = pnlFeeds.Width - 25;
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                    lbl.Height = 75;
                    // Örnek bilgi olarak bilginin ne zaman eklendiği, içeriği ve kim tarafından oluşturulduğu ele alınır
                    lbl.Text = string.Format("{0} \n {1} ({2})", s.created_at, s.text, s.user.name);

                    pnlFeeds.Controls.Add(lbl);
                }
            }
            catch
            {
                lblStatus.Text = "Error!";
            }
        }

        private void btnPostNew_Click(object sender, EventArgs e)
        {
            try
            {
                // Öncelikle HttpClient nesne örneği oluşturulur. Parametre olarak takip edeceğimiz twitter adresi girilir.
                HttpClient client = new HttpClient("http://twitter.com/statuses/");
                // Xml verisini yukarıdaki adresten çekebilmek için geçerli bir Twitter hesabı ile erişmemiz gerekecektir. Bu nedenle NetworkCredential oluşturulur ve Credentials koleksiyonuna eklenir.
                client.TransportSettings.Credentials = new NetworkCredential(username, password);
                ServicePointManager.Expect100Continue = false;

                HttpResponseMessage response = client.Get("friends_timeline.xml");

                // Yeni bilgi girişi için bir form oluşturulur
                HttpUrlEncodedForm form = new HttpUrlEncodedForm();
                // status için TextBox1 kontrolünün içeriğinin girileceği belirtilir
                form.Add("status", txtEntry.Text);
                // Bu kez HTTP Post metoduna göre update.xml adresine form içeriği gönderilir
                response = client.Post("update.xml", form.CreateHttpContent());
                // İşlemin başarılı olduğundan emin olunur
                response.EnsureStatusIsSuccessful();

                lblStatus.Text = "Entry posted.";
            }
            catch
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "Aaa...Houston...We have a problem...";
            }
        }
    }
}
```

Örneğimizi çalıştırıp Get Feed başlıklı Button kontrolüne tıkladığımda aşağıdakine benzer sonuçlar ile karşılaştım.

![blg56_GetFeeds.gif](/assets/images/2009/blg56_GetFeeds.gif)

Görüldüğü üzere örneği geliştirdiğim sıradaki tüm Tweet girişlerini elde edebilmiştim. Evet, tasarım biraz kötü

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

Hatta çok kötü

![Laughing](/assets/images/2009/smiley-laughing.gif)

Dahada güzelleştirilmesini size bırakıyorum.

Peki yeni bir Tweet girdiğimizde. Örneğin aşağıdaki ekran görüntüsündeki gibi,

![blg56_PostNewFeed.gif](/assets/images/2009/blg56_PostNewFeed.gif)

İşlem başarılı olduktan sonra Twitter sitesine baktığımda aşağıdaki gibi yeni Tweet'in eklenmiş olduğunu gördüm. Bu arada tweet'in kaynağı olarak from API yazdığına dikkat edin. Bu dış ortama sunulan Twitter API yardımıyla bir giriş yaptığımızı ifade etmektedir. Peki biz bu API için projemize bir referans ekledik mi? Hayır. Nitekim API'yi REST bazlı olarak kullanıyoruz. İşte işin güzel yanlarından birisi daha.

![blg56_NewTweetLast.gif](/assets/images/2009/blg56_NewTweetLast.gif)

Windows uygulamasında tekrardan Get Feed düğmesini kullandığımda aşağıdaki gibi son eklenen Tweet bilgisinin de geldiğini gördüm.

![blg56_PostNewFeedWin.gif](/assets/images/2009/blg56_PostNewFeedWin.gif)

Aslında web üzerinden takip edilen Tweet içeriğinin bir Windows uygulaması yerine zengin görselliğe sahip bir WPF uygulamasında ele alınması çok daha şık sonuçlar doğurabilir. Bu örnekte bizim için dikkate değer olan noktalardan biriside, WCF Rest Starter Kit Preview 2 ile birlikte gelen HttpClient, Paste Xml As Types gibi yeniliklerin, gerçek hayat senaryosunda başarılı bir şekilde ele alınabilmiş olmasıdır.

Peki örnekte yapmadıklarım neler?

Her şeyden önce asenkron bir erişim söz konusu değildir. Bu nedenle verilerin çekilmesi veya yeni bir Tweet'in eklenmesi sırasında ekranda donmalar olmaktadır. Diğer taraftan çok güçlü bir Exception yönetimiz yok. Belki bir loglama sistemi koyarark Exception'ların uygulamayı geliştirenler için saklanması sağlanabilir. Örneği geliştirirken bir Exception almamış olmama rağmen Tweet bilgilerinin çekilmesi sırasında bağlantıdaki aksaklıklar nedeni ile Time out istisnalarına düşülebileceğini tahmin ediyorum. Bunu kod içerisinde kontrollü bir şekilde ele almak yerinde olacaktır. Diğer taraftan Button kontrolü yardımıyla veri çekmek yerine, kullanıcının kendisinin set edebileceği zaman dilimleri içerisinde veri çekilmesi sağlanabilir. Bu işi bir Timer bileşeni kolayca halledebilir. Birde tasarım konusunda beni takip etmemenizi öneririm

![Wink](/assets/images/2009/smiley-wink.gif)

Bunları deneyin ve çok daha iyisini yapmaya çalışın. Umarım yararlı bir yazı olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[TwitterReader.rar (187,66 kb)](/assets/files/2009/TwitterReader.rar)