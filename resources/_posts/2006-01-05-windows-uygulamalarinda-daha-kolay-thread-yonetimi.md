---
layout: post
title: "Windows Uygulamalarında Daha Kolay Thread Yönetimi"
date: 2006-01-05 08:00:00 +0300
categories:
  - windows-forms
tags:
  - windows-forms
  - thread
  - backgroundworker
---
Windows uygulamalarında, arka planda çalışan iş parçalarının (process) çok uzun sürdüğü durumlar ile sıkça karşılaşırız. Bu gibi durumlarda genellikle kullanıcı ekranı (UI - User Interface) kısa süreliğine de olsa arka plan işlemleri tamamlanıncaya kadar donar. Bunun önüne geçmek için Thread sınıflarını kullanabiliriz. Ancak.Net 2.0 ile birlikte windows uygulamalarında arka planda asenkron olarak çalıştırılabilecek tipteki işlemleri kolayca yönetebileceğimiz BackgroundWorker isimli yeni bir görsel bileşen gelmektedir. Bu makalemizde bu bileşen yardımıyla, UI kitlenmelerine neden olacak tarzdaki süreçleri nasıl yönetebileceğimizi görmeye çalışacağız.

> BackgroundWorker bileşeni çalışma zamanında, asenkron olarak işlem yürütebilmemizi ve bu sayede kullanıcı ekranının gereksiz şekilde donmasını (freeze) engellemek amacıyla kolay ve güçlü bir süreç yönetimi sunar.

Herşeyden önce, bir windows uygulamasında özellikle kullanıcı arayüzünü (user interface) uzun süre duraksatabilecek, bir başka deyişle kullanıcının arabirim üzerindeki herhangibir kontrol ile etkileşimini geçici süre engelleyebilecek durumların neler olabileceğini düşünmekte fayda vardır. Duraksatmaya neden olacak örnek durumlar aşağıdaki tabloda verilmiştir.

Ekran Donmalarına Neden Olabilecek Durumlar

Yoğun veritabanı işlemleri. Örneğin CRUD işlemleri sırasında.

Dosyalara ilişkin download ve upload işlemlerinde.

Web servislerine ait metodların yürütülmesinde.

Resim dosyalarının uygulama ortamına yüklenmesinde.

BackgroundWorker bileşeni, ilgili iş parçasını asenkron olarak yürütebilme ve anlık olarak durum bilgisini verebilme (progress status) gibi imkanlar sunmaktadır. Bu nesneye ait 3 önemli olay bulunmaktadır. Bunlar DoWork, ProgressChanged, RunWorkerCompleted olaylarıdır. DoWork olayı, asenkron olarak yürütülecek kodların yer aldığı işlemleri ele alır. Asenkron olarak yürütülen işlemler çalışmaya devam ederken, kullanıcı arayüzü (UI) herhangibir şekilde donmaz ve kullanıcı aktivitelerine cevap vermeye devam eder. Yürütülen iş parçası sonlandığında ise, RunWorkerCompleted olayı tetiklenir. Bunu, asenkron olarak yürütülen süreç sonlandığında devreye giren callback metodu olarakta düşünebilirsiniz. RunWorkerCompleted metodu sadece işlemler tamamlandığında değil, iptal edildiğinde (cancel) veya bir exception sonucu işlemler kesildiğinde de devreye girer. ProgressChanged olayı ise, asenkron olarak yürütülen iş parçasının anlık durumunu bildirmek amacıyla kullanılabilir.

BackgrounWorker bileşeninin en önemli metodu RunWorkerAsync'dir. Bu metod, DoWork olayını tetikleyerek, asenkron olarak yürütülmek istenen komutların devreye girmesini sağlamak gibi önemli bir rolu üstlenir. Dilersek RunWorkerAsync metodundan, DoWork olay metoduna ortam parametresi taşıyabiliriz. Bu önemlidir çünkü çoğu zaman, asenkron yürütülecek iş parçalarına ait kodlar, dış parametreler bağımlıdır. Benzer şekikde DoWork olayından da işlemler bittiğinde tetiklenen RunWorkerCompleted olayına parametre aktarımını yapabiliriz. (Örneğin süreç sona erdiğinde elde edilen sonuçların aktarılması gibi.)

Bileşenin kullanımını daha kolay anlayabilmek için basit bir örnek üzerinden makalemize devam edeceğiz. Bir windows uygulamamız olduğunu ve arka planda son derece uzun sürebilecek bir operasyonu gerçekleştirmeye çalıştığımızı düşünelim. Örneğin matematiksel bir uygulamada 1' den 1000' e kadar olan elemanların karelerinin toplamına ihtiyacımız olduğunu düşünelim.

> Özellikle Asenkron olarak yürütülecek bir veya daha fazla sql komutunun söz konusu olduğu veritabanı operasyonlarında bir windows uygulaması kullanılıyor ise, BackgroundWorker bileşeni yerine, asenkron komut yürütme tekniklerinin (polling, callback, wait modelleri) ele alınması tercih edilmelidir. Nitekim BackgroundWorker nesne örnekleri ile, sql komutlarının asenkron yönetimi sanıldığı kadar kolay değildir.

Konuyu en basit haliyle anlamak için aşağıdaki örneği geliştirelim. Windows uygulamamız aşağıdaki ekran görüntüsüne sahip olan tek bir Form'dan oluşmaktadır. Hesapla başlıklı butona basıldığında, 1' den 1000' e kadar olan sayıların karelerinin toplamının bulunduğu süreç, arka planda çalıştırılmaktadır. İptal başlıklı button kontrolümüz ise, çalışmakta olan arka plan işlemini iptal etmek için kullanılır. Uygulamamızda bir adet TextBox ve ProgressBar kontrolümüz yer alıyor. Bu kontrollerden TextBox kontrolümüzü, işlemler hesap edilirken kullanıcının ekran ile etkileşebildiğini göstermek amacıyla kullanmaktayız. ProgressBar kontrolümüzü ise, arka planda yapılan hesaplamanın hangi konumda olduğunu yüzdesel olmamakla birlikte vurgulamaya çalışmak için kullanıyoruz. Elbette, 1' den 1000'e kadar olan sayıların karelerinin toplamını hesap etmek anlık bir işlemdir. Biz süreci uzatmak için küçük bir hile yapacağız. Bu hilede amacımız süreci yeterince uzatmak olduğu için, hesaplamar arasında 10 milisaniyelik gecikme süreleri koyacağız.

![mk143_1.gif](/assets/images/2006/mk143_1.gif)

Uygulama kodlarımız ve açıklamaları aşağıda yer almaktadır.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UsingBackGroundWorkerProcess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Hesaplama işlemini yaptığımız metod. İptal işlemini burada ele alabilmek için DoWork olayından buraya DoWorkEventArgs parametresinide gönderiyoruz.
        private double Compute(int endValue, DoWorkEventArgs e)
        {
            double Total = 0;
            for (int i = 1; i <= endValue; i++)
            {
                // Eğer iptal işlemi için btnComputeCancel kontrolüne basılmışsa, BackgroundWorker sınıfının CancelAsync metodu çalıştırılmış demektir. Bu durumda, BackgroundWorker nesnesine bir iptal talebi (Cancel Request) gider. Bu if yapısında bunu kontrol ederek hesaplama işleminin iptalini gerçekleştiriyoruz.
                if (bgwProcess.CancellationPending == true)
                {
                    e.Cancel = true;
                }
                else
                {
                    // Hesaplama işleminden sonra, süreci biraz daha uzatabilmek için arka planda çalışan thread' imizi 10 milisanine kadar duraksatıyoruz. Bu sadece olayları izlemeyi kolaylaştırmak için yapılmış bir hile. Nitekim bize gerçekten arka planda uzun süren bir işlem gerekiyor.
                    Total += i * i;
                    System.Threading.Thread.Sleep(10);
                    // ReportProgress metodu ile hesaplanan değeri, raporlama amacı ile ProgressChanged olayına gönderiyoruz.
                    bgwProcess.ReportProgress(i);
                }
            }
            return Total;
        }

        // DoWork olayımız, hesaplamaların arka planda yürütülmesini ve işlem sonucunun DoWorkEventArgs parametresinin Result özelliğine atanmasını sağlar. Süreç sona erdiğinde, RunWorkerCompleted metodunda bu result özelliğinin değerini alabiliriz.
        private void bgwProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result=Compute(Convert.ToInt32(e.Argument),e);
        }

        // RunWorkerCompleted olayı, arka plan süreci tamamlandığında devreye girmektedir. Burada basit olarak işleme ait iptal talebi (Cancel Request) olup olmadığı kontrol edilir ve yoksa sonuç kullanıcıya bir MessageBox yardımıyla bildirilir. Bu metodu asenkron olarak çalışan bir işlemin tamamlanması sonrası devreye giren Callback metodu olarak düşünebilirsiniz.
        private void bgwProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("İşleminiz iptal edildi...");
                pgbStatus.Value = 0;
                txtFreeZone.Text = "";
            }
            else
            {
                MessageBox.Show(e.Result.ToString());
            } 
            btnComputeCancel.Enabled = false;
            btnComputeStart.Enabled = true;
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            pgbStatus.Maximum = 1000;
            pgbStatus.Minimum = 0;
            pgbStatus.Value = 0;
            // Hesaplama işleminin arka planda asenkron olarak yürütülmesini başlatan metod çağırılır. Dikkat ederseniz metodumuza birde parametrik değer gönderiyoruz. 
            bgwProcess.RunWorkerAsync(1000);
            btnComputeCancel.Enabled = true;
            btnComputeStart.Enabled = false;
        }

        private void btnComputeCancel_Click(object sender, EventArgs e)
        {
            // Arka planda yürütülen işleme, iptal talebini (cancel request) CancelAsync metodu ile göndermekteyiz.
            bgwProcess.CancelAsync();
            btnComputeCancel.Enabled = false;
            btnComputeStart.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnComputeCancel.Enabled = false;
        }

        // Bu olay ile, anlık olarak arka planda devam eden işlemin hangi aşamada olduğunu ele alabiliriz. İlgili değeri ProgressChangedEventArgs parametresinin ProgressPercentage özelliği ile alıyoruz. Bu değer aslında Compute fonksiyonu içerisinde, ReportProgress metodu ile gönderdiğimiz değerin aynısıdır. İstenirse, burada bu değere göre yüzdesel gösterimler de yapılabilir.
        private void bgwProcess_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbStatus.Value = e.ProgressPercentage;
        }
    }
}
```

Uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki Flash videosunda görülen sonuçları elde ederiz.

(Not: Aşağıdaki görüntüyü seyredebilmek için tarayıcınızda Flash Player'ın son sürümünün olması tavsiye edilir. Eğer sisteminizde XP Service Pack 2 yüklüyse ilgili uyarıyı dikkate alıp içeriğe izin vermelisiniz. (Allow Blocked Content). Videoyu yönetmek için sağ tıklayıp çıkan menüyü kullanabilirsiniz.)

Gördüğünüz gibi, Windows uygulamalarında arka planda çalışacak işlemleri yönetmek için BackgroundWorker bileşeni oldukça büyük kolaylıklar sağlamaktadır. Aslında şu bir gerçektir ki, aynı işlevselliği Thread nesnelerini kullanaraktanda yapabilmekteyiz.

Aslında Windows uygulamalarını hedef alan bu gelişmenin, thread yönetimini kolaylaştırıcı bir yenilik olduğunu düşünebiliriz. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.