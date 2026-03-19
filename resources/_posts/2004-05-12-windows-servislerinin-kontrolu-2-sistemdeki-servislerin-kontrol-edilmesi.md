---
layout: post
title: "Windows Servislerinin Kontrolü - 2 ( Sistemdeki Servislerin Kontrol Edilmesi )"
date: 2004-05-12 06:00:00 +0300
description: "Bu makalemizde, sistemde yer alan windows servislerini bir windows uygulamasından nasıl elde edebileceğimizi ve nasıl kontrol edebileceğimizi incelemeye çalışacağız. Önceki makalelerimizden hatırlayacağınız gibi, sistemde yer alan servislerimiz, System.ServiceProcess isim alanında yer alan ServiceController sınıf nesneleri ile temsil edilmektedir."
categories:
  - windows-services
tags:
  - windows-services
  - csharp
---
# Windows Servislerinin Kontrolü - 2 ( Sistemdeki Servislerin Kontrol Edilmesi )
Değerli Okurlarım, Merhabalar.

Bu makalemizde, sistemde yer alan windows servislerini bir windows uygulamasından nasıl elde edebileceğimizi ve nasıl kontrol edebileceğimizi incelemeye çalışacağız. Önceki makalelerimizden hatırlayacağınız gibi, sistemde yer alan servislerimiz, System.ServiceProcess isim alanında yer alan ServiceController sınıf nesneleri ile temsil edilmektedir. Eğer sistemde yer alan servisleri elde etmek istersek, aşağıda aşırı yüklenmiş iki prototipi olan, GetServices metodunu kullanabiliriz.

GetServices Metodu Prototipleri
Açıklaması

public static ServiceController[] GetServices ();
Bu prototip ile, local makinede yer alan servisler elde edilir.

public static ServiceController[] GetServices (string);
Bu prototipte, parametre olarak verilen makine bilgisine ait servisler elde edilir.

Burada görüldüğü gibi, GetServices metodu ServiceController sınıfı türünden bir diziyi geri döndürür. Ayrıca bu metod static bir metod oluduğundan, doğrudan ServiceController sınıfı üzerinden herhangibir nesne örneğine gerek duymadan çağırılabilir. Aşağıda örnek olarak sistemdeki servislerin elde ediliş tekniği gösterilmektedir.

```csharp
ServiceController[] svc;
svc=ServiceController.GetServices();
```

Elde edilen servislerin herbiri birer ServiceController nesnesi olarak, ele alınır. Elde ettiğimiz servislerin üzerinde bildiğiniz gibi, bir servisin temel davranışları olan Start,Stop,Pause,Contiune,ExecuteCommand vb.larını aşağıda prototipi verilen metodlar yardımıyla gerçekleştirebilmekteyiz.

Metod
Prototip
Açıklama

Start
public void Start ();
public void Start (string[]);
Servisi çalıştırır.

Stop
public void Stop ();
Servisi durdurur.

Pause
public void Pause ();
Çalışan servisi duraklatır.

Contiune
public void Continue ();
Duraklatılmış servisin çalışmasına devam etmesini sağlar.

ExecuteCommand
public void ExecuteCommand (int command);
Servisin OnCustomCommand metodunu çalıştırır.

Birazdan geliştireceğimiz örnek uygulamada, sistedemki servisler üzerinde yukarıda bahsettiğimiz metodları kullanarak, servislerin kontrolünü gerçekleştirmeye çalışacağız. Burada dikkat edilmesi gereken bir kaç nokta vardır. Öncelikle bir servis başlatılacaksa, bu servisin o an çalışmıyor olması başka bir deyişle Stopped konumunda olması gerekmektedir. Aynı durum bir servisi durdururkende geçerlidir. Böyle bir durumdada, Stop edilecek servisin, Running konumunda olması gerekmektedir. İşte bu nedenlerden dolayı, sistemdeki servislerin çalışmaya başlaması veya çalışanların durdurulması gibi hallerde, servisin o anki durumunu kontrol etmemiz gerekmektedir. Bu amaçla ServiceController sınıfının aşağıda prototipi verilen Status özelliğini kullanırız.

```csharp
public ServiceControllerStatus Status {get;}
```

Burada görüldüğü gibi Status özelliği ServiceControllerStatus numaralandırıcısı türünden değerler alabilmektedir. Bu numaralandırıcının alacağı değerler servisin o anki durumu hakkında bilgi vermektedir. Status özelliği sadece get bloğuna sahip olduğundan, yanlızca servisin o anki durumu hakkında bilgi vermektedir. ServiceControllerStatus numaralandırıcısının alabileceği değerler aşağıdaki tabloda belirtilmektedir.

ServiceControllerStatus Değeri
Açıklama

ContinuePending
Duraksatılan servis devam ettirilmeyi beklerken.

Paused
Servis duraksatıldığında.

PausePending
Servis duraksatılmayı beklerken.

Running
Servis çalışırken.

StartPending
Servis çalıştırılmayı beklerken.

Stopped
Servis durdurulduğunda.

StopPending
Servis durdurulmayı beklerken.

Burada en çok kafa karıştırıcı haller Pending halleridir. Bunu daha iyi anlamak için örnek olarak StopPending durumunu aşağıdaki şekil üzerinden incelemeye çalışalım.

![mk69_1.gif](/assets/images/2004/mk69_1.gif)

Şekil 1. Pending Durumları.

Burada görüldüğü gibi çalışan bir servise, Stop emri verildiğinde, bu servisin durumu Stopped oluncaya kadar geçen sürede, servis StopPending durumundadır. Aynı mantık, StartPending, PausePending ve ContinuePending durumları içinde geçerlidir. Pending durumlarını daha çok, bu zaman sürelerinin tamamlanıp verilen emrin başarılı bir şekilde uygulandığının izlenmesinde kullanbiliriz. Bu amaçla, ServiceController sınıfı bize, aşağıda prototipleri verilen WaitForStatus metodunu sunmaktadır. Bu metod, parametre olarak aldığı ServiceControllerStatus değeri sağlanıncaya kadar uygulamayı duraksatmaktadır.

Prototipler
Açıklama

public void WaitForStatus (ServiceControllerStatus desiredStatus);
Servisin ServiceControllerStatus numaralandırıcısı ile belirtilen duruma geçmesi için, uygulamayı bekletir.

public void WaitForStatus (ServiceControllerStatus desiredStatus,
TimeSpan timeout);
Servisin ServiceControllerStatus numaralandırıcısı ile belirtilen duruma geçmesi için, TimeSpan ile belirtilen süre kadar uygulamanın beklemesini sağlar.

Biz uygulamamızda WaitForStatus metodunu, bir servise Start,Stop,Pause ve Continue emirlerini verdiğimizde, pending işlemlerinin sona ermesi ile birlikte servisin başarılı bir şekilde istenen konuma geçtiğini anlamak amacıyla kullancağız. Bu aslında SCM yardımıyla bir servisin durdurulması veya başlatılmasında oluşan bekleme işlemi ile alakalı bir durumdur. Örneğin,

![mk69_2.gif](/assets/images/2004/mk69_2.gif)

Şekil 2. Servisin çalıştırılması sırasındaki bekleme işlemi.

SCM yardımıyla bir servisi çalıştırdığımızda (Start) yukarıdaki gibi bir pencere ile karşılaşırız. Burada servis, StartPending konumundadır. Servis çalışmaya başladığında yani Running konumuna geldiğinde, artık StartPending konumundan çıkar. Lakin bu süre zarfında SCM uygulamasının bir süre duraksadığını görürüz. Bu duraksama servis Running halini alıncaya kadar sürer. İşte bunu sağlayan aslında WaitForStatus mekanizmasından başka bir şey değildir. Bu sayede servis Running moduna geçtiğinde, güncelenen listede servisin durumu Running olarak yazacaktır. Eğer WaitForStatus tekniği kullanılmamış olsaydı, bu durumda servis Start edildiğinde ve liste güncellendiğinde ilk aşamada Servisin durumu StartPending olucak ve ancak sonraki liste güncellemesinde Running yazacaktı. Bizde uygulamamızda WaitForStatus metodunu bu amaçla kullanacağız. Yani servis kesin olarak istediğimiz duruma ulaştığında, listemizi güncelliyeceğiz.

ServiceController sınıfı için kullanılacak diğer önemli bir özellikte CanPauseAndContinue özelliğidir. Bazı servislerin bu özellik değeri false olduğu için, bu servislerin Pause ve Continue emirlerine cevap vermesi olanaksızdır. İşte uygulamamızda bizde bir servisin bu durumunu kontrol edicek ve ona göre Pause ve Continue emirlerine izin vereceğiz. CanPauseAndContinue özelliğinin prototipi aşağıdaki gibidir.

```csharp
public bool CanPauseAndContinue {get;}
```

ServiceController sınıfı için kullanabilceğimiz diğer önemli bir özellik ise, bir servisin bağlı olduğu diğer servislerin listesini elde etmemizi sağlıyan, ServicesDependOn özelliğidir. Prototipi aşağıdaki gibi olan bu özellik ile, bir servisin çalışması için gerekli olan diğer servislerin listesini elde edebiliriz.

```csharp
public ServiceController[] ServicesDependedOn {get;}
```

Prototipten de görüldüğü gibi bu özellik, ServiceController sınıfı türünden bir dizi geriye döndürmektedir. Dolayısıyla bu özellik yardımıyla sistemdeki servislerin bağlı oldukları servisleride elde etme imkanına sahibiz.

Şimdi dilerseniz buraya kadar işlediğimiz yanları ile, ServiceController sınıfını kullandığımız bir örnek geliştirmeye çalışalım. Bu örneğimizde, sistemimizdeki servislerin listesini elde edeceğiz. Seçtiğimiz bir servis üzerinde Start, Stop, Pause, Continue gibi emirleri yerine getireceğiz ve bir servise bağlı olan servislerin listesine bakabileceğiz. Bu amaçla öncelikle aşağıdakine benzer bir windows uygulama formu oluşturarak işe başlayalım.

![mk69_3.gif](/assets/images/2004/mk69_3.gif)

Şekil 3. Uygulamamızın Form Görüntüsü.

Şimdide program kodlarımızı oluşturalım.

```csharp
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.ServiceProcess; /* ServiceController sınıfını kullanabilmek için bu isim alanını ekliyoruz.*/

namespace ServisBakim
{
    public class Form1 : System.Windows.Forms.Form
    {
    .
    .

    .
         /* Sistemde yüklü olan servislerin listesini elde edeceğimiz bir metod geliştiriyoruz. Bu metodu hem servis listesini alırken hemde bir servis ile ilgili Start,Stop,Pause,Continue emirleri verildikten sonra, ListView kontrolündeki listeyi güncel tutmak amacıyla kullanacağız.*/
        private void ServisListeGuncelle()
        {
            ServiceController[] svc; /*ServiceController sınıfından bir dizi tanımlıyoruz.*/
            svc=ServiceController.GetServices(); /* Bu diziye ServiceController sınıfının static GetServices metodu yardımıyla sistemde yüklü olan servisleri birer ServiceController nesnesi olarak aktarıyoruz.*/

            lvServisler.Items.Clear(); /* ListView kontrolümüzün içeriğini temizliyoruz.*/
            /* Elde ettiğimiz servisler arasında gezinmek için, Servis sayısı kadar ötelenecek bir for döngüsü oluşturuyoruz. Servis sayısını ise, svc isimli ServiceController tipinden dizimizin Length özelliği ile elde ediyoruz.*/
            for(int i=0;i<svc.Length;i++) 
            {
                lvServisler.Items.Add(svc[i].ServiceName.ToString()); /* ListView kontrolündeki ilk öğeye yani ServisAdı kısmına i indisli ServiceController tipinden svc nesnesinin ismini ekliyoruz. Bunun için ServiceName özelliğini kullanıyoruz.*/
                lvServisler.Items[i].SubItems.Add(svc[i].Status.ToString()); /*ListView kontrolündeki alt öğeye, servisimizin durumunu Status özelliği yardımıyla ekliyoruz.*/
            }
        }

        private void btnServisAl_Click(object sender, System.EventArgs e)
        {
            ServisListeGuncelle();
        }

        private void btnStart_Click(object sender, System.EventArgs e)
        { 
            /* Öncelikle başlatmak istediğimiz servis bilgisini ListView kontrolünden almalıyız. Bunun için ListView kontrolünün FocusedItem özelliğini kullanıyoruz. Böylece kullanıcının ListView'da seçtiği servisin adını elde edebiliriz.*/
            string servis;
            servis=lvServisler.FocusedItem.Text;
            /*Seçilen servis adını ele alarak bir ServiceController nesnesi oluşturuyoruz.*/
            ServiceController curSer=new ServiceController(servis);
            /* Servis başlatma işlemini eğer seçilen servisin durumu Stopped ise gerçekleştirebilmeliyiz. Bu amaçla, önce seçilen servise ait ServiceController nesnesinin Status özelliğine bakıyoruz. Değerin Stopped olup olmadığını denetlerken ServiceControllerStatus numaralandırıcısını kullanıyoruz.*/
            if(curSer.Status==ServiceControllerStatus.Stopped)
            {
                curSer.Start(); /* Servisi çalıştırıyoruz.*/
                curSer.WaitForStatus(ServiceControllerStatus.Running); /* Servisimiz Running konumunu alıncaya dek bir süre uygulamayı bekletiyoruz ve bu işlemin ardından servislerin listesini güncelliyoruz. Eğer WaitForStatus metodunu kullanmassak, servis listesini güncellediğimizde, ilgili servis için StartPending konumu yazılıcaktır.*/
                ServisListeGuncelle();
            }
            else /* Servis zaten çalışıyorsa yani durumu(Status) Running ise bunu kullanıcıya belirtiyoruz.*/
            {
                MessageBox.Show(curSer.ServiceName.ToString()+" ZATEN CALISIYOR");
            }
        }

        private void btnStop_Click(object sender, System.EventArgs e)
        {
            /* Bir servisi durdurmak için yapacağımız işlemler, başlatmak için yapacaklarımız ile aynı. Sadece bu kez, servisin Running konumunda olup olmadığını denetliyor ve öyleyse, Stop komutunu veriyoruz.*/
            string servis;
            servis=lvServisler.FocusedItem.Text;
            ServiceController curSer=new ServiceController(servis);
            if(curSer.Status==ServiceControllerStatus.Running)
            {
                curSer.Stop(); /* Servis durduruluyor. */
                curSer.WaitForStatus(ServiceControllerStatus.Stopped);
                ServisListeGuncelle();
            }
            else
            {
                MessageBox.Show(curSer.ServiceName.ToString()+" ZATEN CALISMIYOR");
            }
        }

        private void btnPause_Click(object sender, System.EventArgs e)
        {
            /* Bir servisi duraksatacağımız zaman, bu servisin, CanPauseAndContinue özelliğinin değeri önem kazanır. Eğer bu değer false ise, servis üzerinde Pause veya Continue emirlerini uygulayamayız. Bu nedenle burada ilgili servise ati CanPauseAndContinue özelliğinin değeri kontrol edilir ve ona göre Pause emri verilir.*/
            string servis;
            servis=lvServisler.FocusedItem.Text;
            ServiceController curSer=new ServiceController(servis);
            if(curSer.CanPauseAndContinue==true) /* CanPauseAndContinue true ise, Pause işlemini uygulama hakkına sahibiz.*/
            {
                if(curSer.Status!=ServiceControllerStatus.Paused) /* Servis eğer Paused konumunda değilse Pause emri uygulanır. Bir başka deyişle servis çalışır durumdaysa.*/
                {
                    curSer.Pause(); /* Servis duraksatılıyor.*/
                    curSer.WaitForStatus(ServiceControllerStatus.Paused); /* Servisin Paused konumuna geçmesi için bekleniyor. */
                    ServisListeGuncelle();
                }
            } 
            else
            {
                MessageBox.Show("SERVISIN PAUSE&CONTINUE YETKISI YOK");
            } 
        }

        private void btnContinue_Click(object sender, System.EventArgs e)
        {
            string servis;
            servis=lvServisler.FocusedItem.Text;
            ServiceController curSer=new ServiceController(servis);
            if(curSer.CanPauseAndContinue==true) /* CanPauseAndContinue true ise, Continue işlemini uygulama hakkına sahibiz.*/
            {
                if(curSer.Status==ServiceControllerStatus.Paused) /* ServiceControllerStatus eğer Paused konumunda ise Continue işlemi uygulanır.*/
                {
                    curSer.Continue(); /* Duraksatılan servis çalışmasına devam ediyor. */
                    curSer.WaitForStatus(ServiceControllerStatus.Running); /* Servisin Running konumuna geçmesi için bekleniyor. */
                    ServisListeGuncelle();
                }
            } 
            else
            {
                MessageBox.Show("SERVISIN PAUSE&CONTINUE YETKISI YOK");
            } 
        }

        private void btnBagliServisler_Click(object sender, System.EventArgs e)
        { 
            string servis;
            servis=lvServisler.FocusedItem.Text;
            ServiceController curSer=new ServiceController(servis);
            ServiceController[] bagliServisler=curSer.ServicesDependedOn; /* Bir servise bağlı servislerin listesini elde etmek için, güncel ServiceController nesnesinin, ServiceDependedOn özelliği kullanılır. */

            lbBagliServisler.Items.Clear();
            /* Bağlı servisler arasında gezinmek için foreach döngüsünü kullanıyoruz.*/
            foreach(ServiceController sc in bagliServisler)
            {
                lbBagliServisler.Items.Add(sc.ServiceName.ToString());
            }
        }
   }
}
```

Uygulamamızı çalıştıralım, sistemdeki servisleri elde edelim ve örneğin Stopped konumunda olan servislerden Alerter servisini çalıştırıp, bu servise bağlı servis (ler) varsa bunların listesini tedarik edelim.

![mk69_4.gif](/assets/images/2004/mk69_4.gif)

Şekil 4. Alerter Servisi Stopped konumunda.

![mk69_5.gif](/assets/images/2004/mk69_5.gif)

Şekil 5. Alerter servisi çalıştırıldı ve bağlı olan servis elde edildi.

Geliştirmiş olduğumuz uygulamada olaşabilecek pek çok hata var. Örneğin listede hiç bir servis seçili değilken oluşabilecek istisnalar gibi. Bu tarz istisnaların ele alınmasını siz değerli okurlarıma bırakıyorum. Böylece geldik bir makalemizin daha sonuna. İlerleyen makalelerimizde görüşmek dileğiyle hepinize mutlu günler dilerim.