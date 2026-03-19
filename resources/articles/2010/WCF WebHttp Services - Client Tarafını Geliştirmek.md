---
title: "WCF WebHttp Services - Client Tarafını Geliştirmek"
pubDate: 2010-02-05 00:45:00
categories:
  - WCF Eco System
  - WCF WebHttp Services
tags:
  - WCF Eco System
  - WCF WebHttp Services
  - C#
  - .NET
  - LINQ
  - WCF
  - Windows Forms
  - XML
  - REST
  - JSON
  - HTTP
  - Generics
  - Visual Studio
  - Rc
---

# WCF WebHttp Services - Client Tarafını Geliştirmek
![blg128_Giris.jpg](images/blg128_Giris.jpg)

Merhaba Arkadaşlar,

Sanırım pek çoğumuz piyangodan veya diğer şans oyunlarından kendilerine tonlarca para çıksa ne yapacağını düşünmüş veya hayal etmiştir. Açıkası kendi adıma hayat etmediğimi dile getirsem yalan söylemiş olurum. Ancak ben pek çoğumuz gibi yan yana bir kaç Ferrari'yi dizmektense bir kaç yere yatırım yapmayı hayal etmişimdir hep. Örneğin dünyanın sayılı bir kaç futbol kulübünün (Barcelona, Manchester United vb...) ve yazılım şirketinin (Microsoft, IBM vb...) hisselerinden satın alır ve şöyle güzel bir fon sepeti oluştururum. Neyse...Sözü niye piyangodan açtığımıza gelince...

Hatırlayacağınız üzere bir önceki yazımızda WCF WebHttp Service'leri ile tanımaya çalışmış ve konuyu pekiştirmek amacıyla basit bir Merhaba Dünya uygulaması geliştirmiştik. Tabi bu örneğimizde HTTP protokolünün yanlızca Get metodunu kullanmıştık. Dolayısıyla operasyonlarımızda sadece WebGet niteliklerinin uygulandığına şahit olduk. Ancak HTTP protokolüne göre Get dışında Post, Put ve Delete metodlarını da kullanabileceğimizi biliyoruz. Dikkat çekici bir diğer noktada örneğimizde Get metoduna göre talepte bulunurken basit bir tarayıcı uygulamadan faydalanmış olmamızdı. Oysaki kendi istemci uygulamamızı yazmak isteyebiliriz. Bu durumda istemci tarafından HTTP protokolünün Get, Post, Put ve Delete metodlarına uygun talepleri nasıl gerçekleştirebiliriz? Aslında olay servis tarafının istediği mesaj paketlerini istemci tarafında oluşturup göndermekten başka bir şey değildir. Yani talebin (Request) içeriğini hazırlamak ve dönen cevabı (Response) değerlendirmek.

İşte bu yazımızda söz konusu durumları ele alaraktan hem Post, Put, Delete metodlarının kullanımına bir örnek verecek hemde istemci tarafını geliştirmeye çalışacağız. Tabi öncesinde servis tarafını hazırlamamız gerekiyor. Bu örneğimizde herhangibir işe yaramasada konuyu anlamamızı kolaylaştıracak bir senaryomuz da olacak. Senaryomuza göre bir Piyango servisi tasarlayacağız.

![Wink](images/smiley-wink.gif)

Bu servis, istemcilerin yeni bir piyango bileti üretebilmesine, var olan piyango biletlerini çekebilmelerine, isterlerse biletlerini silmelerine veya güncellemelerine izin veren operasyonlar içerecek. Tabiki bu operasyonlarda HTTP protokolünün Get, Post, Put ve Delete metodları göz önüne alınıyor olacak. Dilerseniz hiç vakit kaybetmeden WCF REST Service Application uygulamasını oluşturarak işe başlayalım. Uygulamamızda bilet bilgilerinin saklanması ve depolanması amacıyla basit text dosyasından yararlandığımızı belirtmek isterim. Diğer taraftan bilet bilgileri için Ticket isimli yardımcı bir sınıfımızda yer almaktadır.

```csharp
using System;

namespace Lesson2
{
    public class Ticket
    {
        public string Number { get; set; }
        public string Owner { get; set; }
        public DateTime TicketDate { get; set; }
        public bool NewOrUpdated { get; set; }

        public override string ToString()
        {
            return String.Format("{0}|{1}|{2}|Is New? {3}", Number, Owner, TicketDate.ToString(),NewOrUpdated.ToString());
        }
    }
}
```

LotteryService isimli WCF WebHttp Service içeriği ise aşağıda görüldüğü gibidir.

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace Lesson2
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class LotteryService
    {
        string filePath = HttpContext.Current.Server.MapPath("~\\Tickets.txt");

        [WebGet(UriTemplate = "Lottery/{Name}/{LastName}")]
        public List<string> GetMyTickets(string Name,string LastName)
        {
            return (from line in File.ReadAllLines(filePath)
                    where line.Contains(Name + LastName)
                    select line).ToList();
        }

        [WebInvoke(UriTemplate = "Lottery/Create/{Name}/{Surname}", Method = "POST")]
        public Ticket CreateTicket(string Name,string Surname)
        {
            Ticket createdTicket=new Ticket
            {
                Number = Guid.NewGuid().ToString(),
                Owner = String.Format("{0}{1}",Name,Surname),
                TicketDate = DateTime.Now
            };

            File.AppendAllLines(filePath,new String[]{createdTicket.ToString()});
            return createdTicket;
        }

        [WebInvoke(UriTemplate = "Lottery/Update/{TicketNumber}", Method = "PUT")]
        public string UpdateMyTicketNumber(string TicketNumber)
        {
            string ticket = (from line in File.ReadAllLines(filePath)
                         where line.Contains(TicketNumber)
                         select line).First();
            string[] infos=ticket.Split('|');

            string updatedTicket=string.Join("|",Guid.NewGuid().ToString(),infos[1],infos[2],"Is New? ",true.ToString());

            File.AppendAllLines(filePath, new string[]{updatedTicket});

            return updatedTicket;
        }

        [WebInvoke(UriTemplate = "Lottery/Delete/{TicketNumber}", Method = "DELETE")]
        public void DeleteMyTicket(string TicketNumber)
        {
            string[] newLines = (from line in File.ReadAllLines(filePath)
                           where !line.Contains(TicketNumber)
                           select line).ToArray();

            File.WriteAllLines(filePath, newLines);
        }
    }
}
```

Kod parçamızda HTTP Get,Post,Put ve Delete metodlarının kullanımlarına örnek olması açısından çeşitli servis operasyonlarının yer aldığı görülmektedir. Kritik olan noktalar WebGet ve WebInvoke niteliklerinin nasıl kullanıldığıdır. Servisimizin yardım sayfasına bakıldığında, istemci tarafında oluşturulması gereken Request paketlerinin nasıl olacağıda kolaylıkla görülebilir. Tabi Post ve Put metodlarında bir Request Body kullanılmamıştır. Bir başka deyişle Put ve Post işlemleri için gerekli bilgiler servis tarafına URL satırından gönderilmektedir.

![blg128_HelpPage2.gif](images/blg128_HelpPage2.gif)

Gelelim istemci tarafına.

İstemciyi basit bir WinForms uygulaması olarak tasarlayacağız. Önemli olan nokta ise, az önce tasarlanan WCF WebHttp Service'ini nasıl kullanabileceğimiz. Sonuçta HTTP Get, Post, Put ve Delete metodlarının istemci tarafından hazırlanması ve gönderilmesi gerekmekte. Üstelik servise ait bir WSDL içeriği ve dolayısıyla Proxy üretimi de söz konusu değil. Bu noktada WebChannelFactory, HttpWebRequest ve WebClient tiplerinden yararlanabileceğimizi biliyoruz. Ne varki [WCF Rest Starter Kit Preview 2](http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=24644) ile birlikte gelen HttpClient sınıfı tamda bu tip servislerin tüketilmesi için geliştirilmiş durumda. Elbette bu kit içeriğinin,.Net Framework 4.0' ın final sürümü ile birlikte içeriye doğrudan dahil edileceğini tahmin etmekteyiz. Şimdilik Starter Kit ile gelen tipi kullanacağız. Bu sebepten Windows uygulamamıza gerekli referansları aşağıdaki şekildende görüleceği üzere eklememiz gerekiyor.

![blg128_References2.gif](images/blg128_References2.gif)

Dikkat edilmesi gereken noktalardan biriside istemci uygulamanın hedeflediği Framework profilidir. Söz konusu Starter Kit referansları ile çalışabilmek için istemci tarafının hedef profilinin.Net Framework 4.0 Client Profile değil (ki varsayılanı budur).Net Framework 4.0 olması gerekmektedir.

![blg128_TargetFramework.gif](images/blg128_TargetFramework.gif)

Artık istemci için gerekli tüm ön hazırlıklar yapılmıştır. Şimdi dilerseniz servis fonksiyonelliklerini icra edebilmek amacıyla Form içeriğini aşağıdaki gibi düzenleyelim.

![blg128_Form.gif](images/blg128_Form.gif)

Form üzerindeki kontrolleri kullanarak bilet üretebilecek, bir bileti silip güncelleyebilecek yada var olan biletlerimizi görebileceğiz. Tabiki tüm bu fonksiyonellikler LotteryService isimli WCF HttpWeb Service üzerinden gerçekleştiriliyor olacak. İlk olarak bir kişinin sahip olduğu tüm biletleri listlemeye çalışalım. Bu noktada servis tarafındaki GetMyTickets operasyonu için bir çağrı yapılması gerekiyor. Söz konusu çağrı örneğin http://localhost:16088/LotteryService/Lottery/Coni/Vayt şeklinde olabilir. Nitekim WebGet niteliğinde belirtilen URI bilgisi bu şekildedir. Bu durumda istemci tarafında aşağıdaki kodlamayı yapabiliriz.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Http;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetMyTickets_Click(object sender, EventArgs e)
        {
            // REST Starter Kit Preview 2' den gelen HttpClient tipi oluşturulur.
            // Parametre olarak WCF WebHttp Service' in base address bilgisi verilir.
            using (HttpClient client = new HttpClient("http://localhost:16088/LotteryService/"))
            {
                // Get talebi için gerekli URI bilgisi oluşturulur
                string requestUri = String.Format("Lottery/{0}/{1}", txtName.Text, txtSurname.Text);
                // Get metodu ile ilgili talep oluşturulur.
                HttpResponseMessage responseMessage=client.Get(requestUri);
                // Operasyonun başarılı olmaması halinde bir exception fırlatılması isteniyorsa EnsureStatusIsSuccessful metodu kullanılmalıdır.
                responseMessage.EnsureStatusIsSuccessful();
                // İçerik XML tipinden geldiği için bunu kod tarafında kolayca ele almak adına ReadAsXElement genişletme metodu(Extension Method) kullanılabilir. Lakin bu metod için System.Xml.Linq isim alanının referans edilmesi gerekir.
                XElement response = responseMessage.Content.ReadAsXElement();
            }
        }
    }
}
```

Servisin çalıştığını varsaydığımızda yukarıdaki metodun icra edilmesi sonucu debug zamanında aşağıdaki sonuçlara ulaştığımızı görebiliriz. (Tickets.txt dosyasında bazı ticket bilgileri olduğunu ve Name için Coni, Surname için Vayt bilgilerinin girildiğini varsayıyoruz)

![blg128_DebugTime2.gif](images/blg128_DebugTime2.gif)

Dikkat edileceği üzere servis tarafındaki operasyonda List tipinden olan operasyon dönüş tipi istemci tarafına, ArrayOfstring ve string isimli alt elementlerden oluşan bir XML içeriği olarak aktarılmıştır. Elbette XElement içeriğinin bu şekilde elde edilebiliyor olması yeterli değildir. Bu içeriğin ListBox kontrolü içerisine serpiştirilmesini de bekliyoruz. Dolayısıyla aşağıdaki kod ilavesini de yapmamız gerekiyor.

```csharp
XElement response = responseMessage.Content.ReadAsXElement();

lstMyTickets.Items.Clear();
var tickets = from node in response.Elements()
                  select node.Value;

foreach (var ticket in tickets)
{
    lstMyTickets.Items.Add(ticket);
}
```

Dikkat edileceği üzere basit bir XLINQ sorgusu ile tüm elementlerin Value değerleri çekilmiştir. Elbette bu XLINQ sorgu ifadesini belirleyen kriter, servis tarafından dönen XML içeriğinin şemasıdır. Çalışma zamanında örnek bir kullanıcı için Get sorgusunu gerçekleştirdiğimizde aşağıdaki ekran görüntüsündekine benzer sonuçları elde ettiğimizi görürüz.

![blg128_Runtime1Last.gif](images/blg128_Runtime1Last.gif)

Şimdi yeni bir biletin oluşturulması için gerekli kodları yazalım.

```csharp
private void btnCreateTicket_Click(object sender, EventArgs e)
{
	using (HttpClient client = new HttpClient("http://localhost:16088/LotteryService/"))
	{
		string requestUri = String.Format("Lottery/Create/{0}/{1}", txtName.Text, txtSurname.Text);

		// Yeni bir Ticket oluşturmak için gerekli istek HTTP Post metoduna göre yapılmaktadır. Bu sebepten HttpClient tipinin Post metodu kullanılmıştır.
		// Gönderilen talepte herhangibir Request Body içeriği olmadığından HttpContent tipinin CreateEmpty metodu kullanılmıştır.
		HttpResponseMessage responseMessage = client.Post(requestUri, HttpContent.CreateEmpty());
		responseMessage.EnsureStatusIsSuccessful();
		// Oluşturulan yeni Ticket bilgisi istemci tarafına yine bir XML içeriği olarak dönmektedir. Üretilen içerik bilgi amaçlı olarak kullanıcıya gösterilir.
		XElement createdTicket = responseMessage.Content.ReadAsXElement();
		MessageBox.Show(createdTicket.ToString());
	}
}
```

Get kullanımına benzer olmakla birlikte bu kez HttpClient tipinin Post metodundan yararlanılmaktadır. Post ve Put gibi metodlarda Request Body'sinin olması gerekebilir. Ancak bizim servis operasyonlarımız Request Body kullanmamaktadır. Bu nedenle ilgili parametreler HttpContent.CreateEmpty () metodu ile geçilmektedir. Bu kod parçasına göre çalışma zamanında bir bilet üretmek istediğimizde geriye aşağıdakine benzer sonuçların aktarıldığını görebiliriz.

![blg128_Runtime2.gif](images/blg128_Runtime2.gif)

Peki ya silme ve güncelleme işlemlerinden ne haber? Bu operasyonlar için istemci tarafında aşağıdaki kodları yazmamız yeterli olacaktır.

```csharp
private void btnDeleteTicket_Click(object sender, EventArgs e)
{
	// Öncelikle ListBox' ta seçili bir öğe olup olmadığına bakılır
	if (lstMyTickets.SelectedItem != null)
	{
		// Biletin numarası yani GUID bilgisi alınır.
		string ticketNumber = lstMyTickets.SelectedItem.ToString().Substring(0, 36);
		using (HttpClient client = new HttpClient("http://localhost:16088/LotteryService/"))
		{
			// HTTP Delete metoduna göre bir talepte bulunulur.
			string requestUri = String.Format("Lottery/Delete/{0}", ticketNumber);
			// Delete talebi için HttpClient tipinin Delete metodundan yararlanılır. Bu metodun kullanımına göre herhangibir HTTP Request Body içeriği bildirilmesi gerekli değildir.
			HttpResponseMessage responseMessage = client.Delete(requestUri);
			responseMessage.EnsureStatusIsSuccessful();
			
		}
	}
}

private void btnUpdateTicket_Click(object sender, EventArgs e)
{
	if (lstMyTickets.SelectedItem != null)
	{
		string ticketNumber = lstMyTickets.SelectedItem.ToString().Substring(0, 36);
		using (HttpClient client = new HttpClient("http://localhost:16088/LotteryService/"))
		{
			// Update talebi hazırlanır
			string requestUri = String.Format("Lottery/Update/{0}", ticketNumber);
			// Güncelleme isteği aslında HTTP Put metoduna karşılık gelmektedir. Bunun için HttpClient tipinin Put metodundan yararlanılır.
			HttpResponseMessage responseMessage = client.Put(requestUri, HttpContent.CreateEmpty());
			responseMessage.EnsureStatusIsSuccessful();
			// Put metodunun çalıştırılması sonucu üretilen çıktı bu kez string bazlı olacak şekilde bir MessageBox aracılığıyla gösterilir.
			MessageBox.Show(responseMessage.Content.ReadAsString());
		}
	}
}
```

Örneğin var olan bir biletimizi güncellemek istediğimizi düşünelim. Örnek çalışma zamanı görüntüsü aşağıdakine benzer olacaktır.

![blg128_Runtime3.gif](images/blg128_Runtime3.gif)

Tabi uygulamamızın pek çok yerinde bug ve iş mantığı hatası vardır. Üstelik tam anlamıyla bir istisna yönetimide (Exception Handling) yapılmamaktadır. Ancak odaklanmamız gereken yada dikkat etmemiz gereken noktalar WCF WebHttp Service üzerinden Post, Put, Delete operasyonlarının nasıl sunulduğu ve istemci tarafında bunların nasıl ele alındığıdır. Özellikle istemci tarafında kullandığımız tekniklere baktığımızda şu sonuçlara varabiliriz;

- Herhangibir proxy tipi kullanılmamıştır. Bilindiği üzere servisleri tüketmenin yollarından birisi istemci tarafında gerekli proxy tipinin üretilmesidir. HTTP Get,Post,Put ve Delete metodlarının kullanıldığı teknikte ise sadece paketlerin oluşturulup gönderilmesi ve cevapların değerlendirilmesi gerekmektedir.
- Sonuçların ilgili formata göre istemci tarafına gönderilmesi söz konusudur. Varsayılan olarak XML tipinden içerik döndürülmektedir. Ancak JSON formatında da dönüşler olabilir.
- Get metodundan elde edilen XML formatlı içerikler istemci tarafında XElement tipi ile ele alınabilir ve XLINQ kullanılarak ayrıştırılabilir.
- İstemci tarafından yapılan güncelleştirme çağrıları için Put, ekleme işlemlerine ait talepler için Post, silme işlemlerine ait talepler için Delete metodları kullanılır.
- Post ve Put metodları parametre olarak eğer gerekliyse bir Body içeriği sunmak zorunda olabilirler. Eğer sunmuyorlarsa HttpContent.CreateEmpty () metodu ile boş içerik gönderileceğinin belirtilmesi gerekir.

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[Lesson2.rar (196,61 kb)](assets/Lesson2.rar) [Örnek Visual Studio 2010 Ultimate Beta 2 Sürümünde geliştirilmiş ancak RC sürümü üzerinde de test edilmiştir]
