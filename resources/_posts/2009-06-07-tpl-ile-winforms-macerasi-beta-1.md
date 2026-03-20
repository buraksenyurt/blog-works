---
layout: post
title: "TPL ile WinForms Macerası [Beta 1]"
date: 2009-06-07 11:53:00 +0300
categories:
  - tpl
tags:
  - tpl
  - csharp
  - bash
  - dotnet
  - wpf
  - windows-forms
  - task-parallel-library
  - threading
  - delegates
---
Dün gece Task Parallel Library ile ilgili olarak internette araştırma yaparken, örnekleri çoğunlukla (hatta tamamen) Console uygulamaları üzerinde geliştirdiğimi farkettim. Oysaki TPL veya PLINQ gibi alt yapıların, WinForms yada WPF (Windows Presentation Foundation) uygulamalarında nasıl kullanılabileceğide önemli bir konuydu. Özellikle Windows Form'larının TPL çalışmalarına karşı nasıl tepkilerde bulunabileceği belkide en önemli noktaydı. Biliyorsunuz TPL alt yapısında, işlemci ve çekirdek gücü sonuna kadar kullanılmakta ve arka planda coşan pek çok Thread yer almaktadır. Fakat WinForms uygulamalarında herşeyin hakimi olan ana Thread'in genellikle bencil olduğuda bilinmektedir. Bu nedenle TPL ile çekilen bir veri içeriğinin, Form üzerindeki bir kontrole doldurulması gerçekten başa bela olabilir.

![Sealed](/assets/images/2009/smiley-sealed.gif)

İşte bu düşünceler içerisinde yola çıktım ve örnek bir senaryo üzerinde durmaya çalıştım.

İlk olarak senaryodan biraz bahsedeyim; bilgisayarımda resimlerin tutulduğu klasörde yer alan jpg dosyalarından 100 KB'ın altında olanları bulup, Form üzerindeki bir FlowLayoutPanel içerisinde Button bileşenleri ile göstermek istemekteyim. Kabaca aşağıdaki ekran görüntüsünde yer alan sonuçları elde etmek istediğimizi düşünebiliriz.

![blg28_1.gif](/assets/images/2009/blg28_1.gif)

İşe ilk olarak eski stilde başladım. Yani tek bir Thread ile resimleri doldurmayı denedim. Bunun için kod içeriğini ilk etapta aşağıdaki gibi geliştirdim.

```csharp
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPLAntrenmanlari2
{
    public partial class Form1 : Form
    {
        private string imagesPath = @"C:\Users\Burak Selim Senyurt\Pictures";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {            
            flowLayoutPanel1.Controls.Clear();

            #region Single Thread Kullanılarak

            Stopwatch watch = Stopwatch.StartNew();

            foreach (string f in Directory.GetFiles(imagesPath))
            {
                FileInfo fInfo = new FileInfo(f);
                if (fInfo.Length <= 1024 * 100
                    && fInfo.Extension == ".jpg")
                {
                    Button btn = new Button();
                    btn.Width = 64;
                    btn.Height = 48;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.BackgroundImage = Image.FromFile(f);
                    flowLayoutPanel1.Controls.Add(btn);
                }
            }

            watch.Stop();
            lblElapsedTime.Text = String.Format("İşlemler {0} saniyede bitmiştir.", watch.Elapsed.TotalSeconds.ToString());

            #endregion
        }
        
    }
}
```

İlk geliştirmede, resimlerin tutulduğu klasördeki dosyalar bir foreach döngüsü yardımıyla dolaşılmaktadır. Sonrasında ise uzantısı jpg olan ve 100 KB altında olanlar belirlenmektedir. Bu kritere uyan her bir resim için bir Button kontrolü üretilmekte ve arka plan olarak bulunan resim kullanılmakadır. Tabiki son olarak söz konusu Button kontrolü, FlowLayoutPanel bileşeni içerisine eklenmektedir. Sonuçlar benim sisteminde aşağıdaki gibi gerçekleşmiştir.

![blg28_2.gif](/assets/images/2009/blg28_2.gif)

Dikkat çekici nokta işlemlerin tamamlanma süresidir. Neredeyse 20 saniye.

![Undecided](/assets/images/2009/smiley-undecided.gif)

Üstelik işlemler sırasında Form'u herhangibir yere çekiştiremediğimizi görürüz. Ayrıca, Button bileşenleri oluşturulup FlowLayoutPanel kontrolüne eklenirken Form üzerinde görsel bir hareketlilik olmadığı gözlemlenebilir. Ancak tüm işlemler bittikten sonra Button'ların görülmesi mümkün olacaktır. Tabiki isteklerimizden ilki işlemlerin daha kısa sürede bitirilmesi olarak düşünülebilir. Bu amaçla btnStart2_Click kodlarında Parallel.ForEach kullanımını tercih ettim. İşte kodun yeni hali;

```csharp
private void btnStart2_Click(object sender, EventArgs e)
{
	flowLayoutPanel1.Controls.Clear();

	#region Parallel.ForEach kullanımı

	Stopwatch watch = Stopwatch.StartNew();

	Parallel.ForEach(Directory.GetFiles(imagesPath), f =>
	{
		FileInfo fInfo = new FileInfo(f);
		if (fInfo.Length <= 1024 * 100
			&& fInfo.Extension == ".jpg")
		{
			Button btn = new Button();
			btn.Width = 64;
			btn.Height = 48;
			btn.BackgroundImageLayout = ImageLayout.Stretch;
			btn.BackgroundImage = Image.FromFile(f);
			
			flowLayoutPanel1.Controls.Add(btn); // Exception: Cross-thread operation not valid: Control 'flowLayoutPanel1' accessed from a thread other than the thread it was created on.
		}
	}
	);

	watch.Stop();
	lblElapsedTime.Text = String.Format("İşlemler {0} saniyede bitmiştir.", watch.Elapsed.TotalSeconds.ToString());

	#endregion
}
```

Görüldüğü gibi tek fark Parallel.ForEach kullanımıdır. Bu sayede, ForEach içerisinde yer alan işlemlerin paralel iş parçalarına bölünerek gerçekleştirilmesi mümkün olacaktı. Ancak ortam Console değildi. Artık WinForms ortamındaydık. Çevresel faktörler daha farklıydı. Dolayısıyla sonuç aşağıdaki gibi oldu.

![blg28_3.gif](/assets/images/2009/blg28_3.gif)

İşte beklenen hayalet.

![Sealed](/assets/images/2009/smiley-sealed.gif)

Durumu şu şekilde açıklayabiliriz. Windows uygulaması çalıştırıldığıda yürümekte olan ana Thread, kendisini Form üzerindeki tüm kontrollerin sahibi olarak ilan etmiştir. Bu nedenle farklı bir Thread içerisinden, sahibi olduğu bir kontrole ulaşılmasına izin vermez. Çözüm için pek çok farklı yol vardır. Ben bu yollardan birisi olan Invoker'lardan faydalanmaya karar verdim. İşte kodun son hali.

```csharp
private void btnStart2_Click(object sender, EventArgs e)
{
	flowLayoutPanel1.Controls.Clear();

	#region Parallel.ForEach kullanımı

	Stopwatch watch = Stopwatch.StartNew();

	Parallel.ForEach(Directory.GetFiles(imagesPath), f =>
	{
		FileInfo fInfo = new FileInfo(f);
		if (fInfo.Length <= 1024 * 100
			&& fInfo.Extension == ".jpg")
		{
			Button btn = new Button();
			btn.Width = 64;
			btn.Height = 48;
			btn.BackgroundImageLayout = ImageLayout.Stretch;
			btn.BackgroundImage = Image.FromFile(f);
			AddToPanel(btn);
			// flowLayoutPanel1.Controls.Add(btn); // Exception: Cross-thread operation not valid: Control 'flowLayoutPanel1' accessed from a thread other than the thread it was created on.
		}
	}
	);

	watch.Stop();
	lblElapsedTime.Text = String.Format("İşlemler {0} saniyede bitmiştir.", watch.Elapsed.TotalSeconds.ToString());

	#endregion
}

#region  Cross-thread operation not valid hatasına karşı mücadele

private delegate void AddControlHandler(Button pb);
private void AddToPanel(Button pb)
{
	if (flowLayoutPanel1.InvokeRequired)
		flowLayoutPanel1.BeginInvoke(new AddControlHandler(RealAddToPanel), new object[] { pb });
	else
		RealAddToPanel(pb);
}
private void RealAddToPanel(Button pb)
{
	flowLayoutPanel1.Controls.Add(pb);
}

#endregion
```

Bu durumda kendi sistemimde aşağıdaki sonuçlar ile karşılaştığımı gördüm.

![blg28_4.gif](/assets/images/2009/blg28_4.gif)

Evett...Durumu bir değerlendirelim. 20 saniyelik sürelerden yaklaşık 8 saniyelik sürelere indik. Bu çift çekirdekli bir sistem için iyi bir sonuç olarak görünüyor. (Tabi kodu daha fazla çekirdek sayısı bir sistemde ne yazıkki test edemedim. Ama siz değerli okurlarımdan test etme fırsatı olan olursa sonuçları paylaşmasını rica edeceğim.) Yinede herşey istediğimiz gibi değildir. Süre azalmasına rağmen, Form'u işlemler sırasında harekete ettiremediğimizi görürüz. Benzer şekilde resimleri içeren Button kontrolleri yine üretildikçe değil tüm işlemler bittikten sonra bir anda ekranda gösterilmektedir. Dolayısıyla Parallel.ForEach'in tam anlamıyla yeterli gelmediğini söyleyebiliriz. Çözüm olarak ThreadPool sınıfından yararlanabiliriz aslında. Şimdi kodu aşağıdaki gibi değiştirdiğimizi düşünelim.

```bash
#region  Cross-thread operation not valid hatasına karşı mücadele

private delegate void AddControlHandler(Button pb);
private void AddToPanel(Button pb)
{
	if (flowLayoutPanel1.InvokeRequired)
		flowLayoutPanel1.BeginInvoke(new AddControlHandler(RealAddToPanel), new object[] { pb });
	else
		RealAddToPanel(pb);
}
private void RealAddToPanel(Button pb)
{
	flowLayoutPanel1.Controls.Add(pb);
}

#endregion

private void FillImages(object state)
{
	Parallel.ForEach(Directory.GetFiles(imagesPath), f =>
	{
		FileInfo fInfo = new FileInfo(f);
		if (fInfo.Length <= 1024 * 100
			&& fInfo.Extension == ".jpg")
		{
			Thread.Sleep(100); // Bunu koymadığımızda UI istediğimiz gibi reaksiyon vermiyor.
			Button btn = new Button();
			btn.Width = 64;
			btn.Height = 48;
			btn.BackgroundImageLayout = ImageLayout.Stretch;
			btn.BackgroundImage = Image.FromFile(f);
			AddToPanel(btn);
		}
	}
	);
}

private void btnStart3_Click(object sender, EventArgs e)
{
	flowLayoutPanel1.Controls.Clear();

	#region Parallel.ForEach kullanımı

	Stopwatch watch = Stopwatch.StartNew();

	ThreadPool.QueueUserWorkItem(new WaitCallback(FillImages));

	watch.Stop();
	lblElapsedTime.Text = String.Format("İşlemler {0} saniyede bitmiştir.", watch.Elapsed.TotalSeconds.ToString());

	#endregion
}
```

QueueUserWorkItem metodu parametre olarak WaitCallback temsilcisini kullanmaktadır. Bu temsilci ise FillImages metodunu işaret etmektedir. İşlemler FillImages metodu içerisinde yapılmaktadır. Bu durumsa sonuçlar çok daha ilginç olacaktır. Button kontrolleri oluşturuldukça FlowLayoutPanel kontrolü içerisindede görünür hale gelecektir. Ayrıca, Form'u işlmeler sırasında sürükleyebildiğimizi veya oluşturulan Button'lara tıklayabildiğimizide görebiliriz.

Ancak zaman ilerlemiştir ve artık Task sınıfı ve üyeleri ile aynı işlemi nasıl gerçekleştirebileceğimize bakmamız gerekmektedir. Sonuç itibaryle.Net 4.0 için aynı işleyişi aşağıdaki kod parçası ile gerçekleştirebiliriz.

```csharp
private void btnStart4_Click(object sender, EventArgs e)
{
	flowLayoutPanel1.Controls.Clear();
	Stopwatch watch = Stopwatch.StartNew();

	Task.Factory.StartNew(() => FillImages(null));

	watch.Stop();
	lblElapsedTime.Text = String.Format("İşlemler {0} saniyede bitmiştir.", watch.Elapsed.TotalSeconds.ToString());
}
```

Bir önceki yazımızdan hatırlayacağınız gibi Task sınıfı üzerinden StartNew metodunu kullanarak paralel görevlerin başlatılması sağlanabilmektedir. Burada metoda parametre olarak Action temsilcisinin işaret edebileceği FillImage fonksiyonu verilmiştir. Sonuçlar yine yukarıdaki Flash animasyonundakine benzer olacaktır. Kullanıcılar, resimleri gösteren Button kontrolleri yüklenirken, Formun diğer alanları ile etkileşimde bulunubilmektedir. Ayrıca Button bileşenleri oluşturuldukça FlowLayoutPanel içerisinde görülebilmektedir. Ancak kod içerisinde küçük bir hile yaptığımı belirtmek isterim.

![Embarassed](/assets/images/2009/smiley-embarassed.gif)

Dikkat ederseniz FillImages metodu içerisinde o anki Thread için 100 milisaniye kadar bir duraksatma yapılmaktadır. Bu yapılmadığı takdirde Button bileşenlerinin oluşturuldukça FlowLayoutPanel içerisinde gösterilmelerinde bir sıkıntı olduğu gözlemlenir. Açıkçası Tutarsız bir çalışma olmaktadır. Ancak şimdilik bu gecikmenin olmasında bir sakınca yoktur. Nitekim, kullanıcı zaten paralel süreç içerisindeki işlemlerden her biri bittikçe, tamamlanan o işi ele alabilmektedir. Yani işlemlerin tamamalanmasının beklenmesine gerek kalınmadan çalışmaya devam edilebilmektedir.

Böylece geldik bir blog yazımızın daha sonuna. İlerleyen dönemlerde aynı senaryoyu bir WPF uygulaması için ele almaya çalışıyor olacağım. Görüşmek dileğiyle.

[TPLAntrenmanlari2.rar (40,63 kb)](/assets/files/2009/TPLAntrenmanlari2.rar)
