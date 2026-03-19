---
layout: post
title: "Windows Servislerinin Kontrolü -1"
date: 2004-05-05 09:00:00 +0300
description: "Bu makalemizde, windows servislerinin, bir windows uygulamasından nasıl kontrol edilebileceğini incelemeye çalışacağız. Bir önceki makalemizde, windows servislerinin nasıl oluşturulduğunu ve sisteme nasıl yüklendiklerini incelemiştik. Oluşturduğumuz windows servislerini(sistemdeki windows servislerini), SCM yardımıyla yönetibilmekteyiz. Ancak dilersek, bu yönetimi programlarımız içindende gerçekleştirebiliriz. Bunu sağlayabilmek için, System.ServiceProcess isim alanında yer alan ServiceController sınıfını ve üyelerini kullanmaktayız."
categories:
  - windows-services
tags:
  - windows-services
  - csharp
  - dotnet
  - performance
  - visual-studio
---
# Windows Servislerinin Kontrolü -1
Değerli Okurlarım, Merhabalar.

Bu makalemizde, windows servislerinin, bir windows uygulamasından nasıl kontrol edilebileceğini incelemeye çalışacağız. Bir önceki makalemizde, windows servislerinin nasıl oluşturulduğunu ve sisteme nasıl yüklendiklerini incelemiştik. Oluşturduğumuz windows servislerini (sistemdeki windows servislerini), SCM yardımıyla yönetibilmekteyiz. Ancak dilersek, bu yönetimi programlarımız içindende gerçekleştirebiliriz. Bunu sağlayabilmek için, System.ServiceProcess isim alanında yer alan ServiceController sınıfını ve üyelerini kullanmaktayız.

ServiceController sınıfı ile windows servislerine bağlanabilir ve onları kontrol edebiliriz. Örneğin servisleri başlatabilir, durdurabilir veya sistemdeki servisleri elde edebiliriz. Bu ve benzeri olanaklar dışında SCM ile yapamıyacağımız bir olayıda gerçekleştirebiliriz. Bu olay windows servislerinin OnCustomCommand metodu üzerinde işlemektedir. Bir windows servisinin OnCustomCommand metodu sayesinde servisimize standart işlevselliklerinin haricinde yeni işlevsellikler kazandırabiliriz. Prototipi aşağıdaki gibi olan OnCustomCommand metodu integer tipinden bir parametre almaktadır.

```csharp
protected virtual void OnCustomCommand(int command);
```

OnCustomCommand metodunu herhangibir windows uygulamasından çağırabilmek için, ServiceController sınıfının ExecuteMethod metodu kullanılır. Integer tipindeki parametre 128 ile 256 arasında sayısal değerler alır. Metoda gönderilen parametre değerine göre, OnCustomCommand metodunun farklı işlevsellikleri yerine getirmesini sağlayabiliriz. ServiceController sınıfı ile yapabileceklerimiz aşağıdaki şekilde kısaca özetlenmiştir.

![mk68_1.gif](/assets/images/2004/mk68_1.gif)

Şekil 1. ServiceController Sınıfı İle Yapabileceklerimiz.

Servislerimizi programatik olarak nasıl kontrol edebileceğimize geçmeden önce, konumuz ile ilgili basit bir windows servisi yazarak işe başlayalım. Bu servisimizde, kendi oluşturacağımız bir Event Log içerisinde, sistemdeki sürücülerinin boş alan kapasitelerine ait bilgileri tutacağız. Bu amaçlada, servisimize bir Performance Counter ekleyeceğiz. Ayrıca servisimize bir timer kontrolü koyacak ve bu kontrol ile belirli periyotlarda servisin, boş alan bilgilerini kontrol etmesini ve belli bir serviyenin altına inilirse Event Log'umuza bunu bir uyarı simgesi ile birlikte yazmasını sağlayacağız.

Bununla birlikte, OnCustomCommand metodunu uygulamamızdan çalıştıracak ve gönderdiğimiz parametre değerine göre servisin değişik işlevleri yerine getirmesini sağayacağız. Örneğin kullanıcının boş alan kontrol süresini ve alt sınır değerlerini programatik olarak belirleyebilmesini ve servisteki ilgili değerleri buna göre değiştirebilmesini sağlayacağız. Elbette bu değişiklikler servisin çalışma süresi boyunca geçerli olucaktır.

Şimdi dilerseniz servisimi oluşturalım. Öncelikle vs.net ortamında, yeni bir windows service projesi açıyoruz. Projemizin adı, BosAlanTakip olsun. Ardından servisimizin özelliklerinden adını ve servise ait kodlarda yer alan Service1'i, BosAlanTakipServis olarak değiştirelim. Bununla birlikte Soluiton Explorer'da Service1.cs kod dosyamızın adınıda BosAlanTakipServis.cs olarak değiştirelim. Sonraki adımımız ise, servisin özelliklerinden AutoLog özelliğine false değerini atamak. Nitekim biz bu servisimizde kendi yazacağımız Event Log'u kullanmak istiyoruz.

![mk68_2.gif](/assets/images/2004/mk68_2.gif)

Şekil 2. Servisimizin özelliklerinin belirlenmesi.

Servisimizin kodlarını yazmadan önce, Custom Event Log'umuzu ekleyeceğiz. Bunun için, Components sekmesinden EventLog nesnesini servisimizin tasarım ekranına sürüklüyoruz.

![mk68_3.gif](/assets/images/2004/mk68_3.gif)

Şekil 3. EventLog nesnesi.

Şimdi EventLog nesnemizin özelliklerini belirleyelim.

![mk68_4.gif](/assets/images/2004/mk68_4.gif)

Şekil 4. EventLog nesnemizin özellikleri.

Artık log bilgilerini, servisimize eklediğimiz eventLog1 nesnesini kullanararak oluşturacağız. Sırada servisimize bir Performance Counter nesnesi eklemek var. Servislerimizde kullanabileceğimiz Performance Counter öğelerini, Solution Explorer pencersinde yer alan Performance Counter sekmesinden izleyebiliriz. Sistemde yüklü olan pek çok Performance Counter vardır.

![mk68_5.gif](/assets/images/2004/mk68_5.gif)

Şekil 5. Sistemdeki Performance Counter'lardan bazıları.

Biz bu örneğimizde, LogicalDisk sekmesinde yer alan Free MegaBytes öğesini kullancağız. Bu sayaç yardımıyla, servisimizden, sistemdeki bir hardisk'in boş alan bilgilerini elde edebileceğiz. Bu amaçla buradaki C: sürücüsünü servisimiz üzerine sürüklüyoruz.

![mk68_6.gif](/assets/images/2004/mk68_6.gif)

Şekil 6. Kullancağımız Performance Counter öğesi.

Diğer ihtiyacımız olan nesne bir Timer. Bunun için yine Components sekmesinden Timer nesnesini, servisimizin tasarım ekranına sürüklememiz yeterli. Daha sonra Timer nesnesinin interval özelliğinin değerini 600000 olarak değiştirelim. Bu yaklaşık olarak 10 dakikada (1000 milisaniye * 10 dakika * 60 saniye) bir, Timer nesnesinin Elapsed olayının çalıştırılılacağını belirtmektedir. Biz bu olay içerisinden C sürücüsündeki boş alan miktarını kontrol etmeyi planlıyoruz. Elbette süreyi başlangıçta 10 dakika olarak belirlememize rağmen geliştireceğimiz windows uygulamasında bu sürenin kullanıcı tarafından değiştirilebilmesini sağlıyacağız. Bu ayarlamaların ardından servisimiz için gerekli kodları yazmaya geçebiliriz.

```csharp
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;

namespace BosAlanTakip
{
    public class BosAlanTakipServis : System.ServiceProcess.ServiceBase
    {
        ...   
        private long BosMiktar;
        private long Sinir;

        /* Servisimiz çalıştırıldığında(start),durdurulduğunda(stop), duraksatıldığında(pause) ve yeniden çalıştırıldığında(continue) , zaman bilgisini ve performanceCounter1 nesnesinin RawValue özelliğini kullanarak C sürücüsündeki boş alan miktarını, oluşturduğumuz Event Log'a WriteEntry metodu ile yazıyoruz. Servisin durum bilgisini ise metoda gönderdiğimiz string türünden parametre ile elde ediyoruz.*/
        private void Bilgilendir(string durum)
        {
            eventLog1.WriteEntry(durum+" "+DateTime.Now.ToShortDateString()+ " C:"+performanceCounter1.RawValue.ToString()+" mb");
        }
        protected override void OnStart(string[] args)
        {
            Bilgilendir("START");
            BosMiktar=performanceCounter1.RawValue; /* Servis çalıştırıldığında, C sürücüsündeki boş alan miktarını, BosMiktar isimli long türünden değişkenimize aktarıyoruz. performanceCounter'ımızın RawValue özelliği burada seçtiğimiz sayaç kriteri gereği sonucu megabyte cinsinden döndürmektedir.*/
            Sinir=3300; // Yaklaşık olarak 3.3 GB.
            timer1.Enabled=true;  // Timer nesnemizi çalıştırıyoruz.
        }    
        protected override void OnStop() 
        {
            Bilgilendir("STOP");
        }
        protected override void OnPause()
        {
            Bilgilendir("PAUSE");
        }
        protected override void OnContinue()
        {
            Bilgilendir("CONTINUE");
        }
        /* Özel komutumuzu yazıyoruz. */
        protected override void OnCustomCommand(int command)
        {
            /* if koşullarında OnCustomCommand'a gönderilecek parametre değerine göre, boş alan uyarısı için gerekli alt sınır değeri belirleniyor. Ayrıca, timer nesnemizin interval değeride gelen parametre değerine göre belirleniyor ve böylece timer nesnesinin Elapsed olayının çalışma aralığı belirlenmiş oluyor.*/
            if(command==200)
            {
                Sinir=3000; // Yaklaşık olarak 3gb.
            }
            else if(command==201)
            {
                Sinir=2000; // Yaklaşık olarak 2gb.
            }
            else if(command==202)
            {
                Sinir=4000; // Yaklaşık olarak 4gb.
            }
            else if(command==203)
            {
                timer1.Enabled=false;
                timer1.Interval=1800000; // 30 dakikada bir.
                timer1.Enabled=true;
            }
            else if(command==204)
            {
                timer1.Enabled=false;
                timer1.Interval=3600000; // Saatte bir.
                timer1.Enabled=true;
            }
            else if(command==205)
            {
                timer1.Enabled=false;
                timer1.Interval=3000; // 3 Saniyede bir.
                timer1.Enabled=true;
            }
        }     
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(BosMiktar<=Sinir)
            {
                /* Eğer C sürücüsündeki boş alan mikarı Sinir değişkeninin sahip olduğu değerin altına düşerse, bu bilgi Event Log'umuza bir warning singesi ile birlikte eklenecek. */
                string bilgi=DateTime.Now.ToShortDateString()+ " C: Surucusu Bos Alan : "+performanceCounter1.RawValue.ToString();
            eventLog1.WriteEntry(bilgi,EventLogEntryType.Warning);
            }
            else
            {
                Bilgilendir("");
            }
        }
    }
}
```

Servisimizin kodlarınıda böylece hazırlamış olduk. Şimdi servisimiz için gerekli olan installer'larımızı servisimizin tasarım ekranında sağ tuşla açtığımız menüden, Add Installer öğesini seçerek ekleyelim. Önce ServiceInstaller1'in özelliklerinden Display Name özelliğinin değerini, Bos Alan Takip olarak değiştirelim. Böylece servisimizin, services sekmesinde görünen ismini belirlemiş oluruz. Ardından ServiceProcessInstaller1 nesnemizin, Account özelliğinin değerini, LocalSystem olarak değiştirelim. Böylece sistemi açan herkes bu servisi kullanabilecek.

Diğer yandan oluşturduğumuz Custom Event Log içinde bir installer oluşturmamız gerekiyor ki installUtil tool'u ile servisimizi sisteme kurduğumuzda, sistedeki Event Log'lar içerisine, oluşturduğumuz Custom Event Log'da kurulabilsin. Bu amaçla, eventLog1 nesnemiz üzerinde sağ tuşa basıp çıkan menüden Add Installer'ı seçiyoruz. Bu sayede, Event Log'umuz için, bir adet eventLogInstaller'ın oluşturulduğunu görürüz. Bu işemlerin ardından windows servis uygulamamızı derleyelim ve servisimizi sisteme yükleyebilmek için installUtil, vs.net tool'unu aşağıdaki gibi kullanalım.

```csharp
installUtil BosAlanTakip.exe
```

Bu işlemlerin ardından servisimiz başarı ile yüklendiyse, server explorer'daki services sekmesinde görünecektir. Servisimizi bu aşamada denemek amacı ile, başlatıp durdurmayı deneyelim. Ancak bu noktada servisimizin pause ve continue olaylarının geçersiz olduğunu görürüz. Bunun sebebi, servisimizin OnPauseContinue özelliğinin değerinin false olmasıdır. Şimdi bu değeri true yapalım. Servis uygulamamızı tekrar derleyelim. Ardından servisimizi sisteme yeniden yükleyelim. Şimdi server explorer'dan servisimizi çalıştırıp deneyelim.

![mk68_7.gif](/assets/images/2004/mk68_7.gif)

Şekil 7. Servisin Çalışan Hali.

Ben servisi test etmek için, C sürücüsündeki boş alanı biraz daha düşürdüm. Görüldüğü gibi servis başarılı bir şekilde çalışıyor. Şu an için her 10 dakikada bir timer nesnesinin Elapsed olayı devreye giriyor ve C sürücüsündeki boş alan miktarının 3.3 gb'ın altına düşüp düşmediği kontrol ediliyor. Servisimiz bunu tespit ettiği anlarda Event Log'umuza bir uyarı işareti ekleyecektir.

Şimdi yazmış olduğumuz bu servisi başka bir windows uygulaması içerisinden nasıl yönetebileceğimizi incelemeye başlayalım. Burada bizim için anahtar nokta, windows uygulamamızda, System.ServiceProcess isim alanını kullanmak. Oluşturmuş olduğumuz servise ait OnCustomCommand metodunu çalıştırmak için, bu isim alanında yer alan ServiceController sınıfının ExecuteCommand metodunu kullanacağız. Öncelikle aşağıdaki gibi bir form tasarlayarak işe başlayalım.

![mk68_8.gif](/assets/images/2004/mk68_8.gif)

Şekil 8. Form Tasarımımız.

Şimdi ServiceProcess isim alanını kullanabilmek için Add Referance kısmından uygulamamıza eklememiz gerekiyor.

![mk68_9.gif](/assets/images/2004/mk68_9.gif)

Şekil 9. System.ServiceProcess isim alanının uygulamaya eklenmesi.

Öncelikle servisimizi temsil edicek ServiceController sınıfı türünden bir nesne oluşturacağız. ServiceController sınıfından nesnemizi oluştururken aşağıda prototipi verilen yapıcı metodu kullanıyoruz. Bu yapıcı, servisin tam adını parametre olarak string türünden almaktadır.

```csharp
public ServiceController(string serviceName);
```

ServiceController sınıfına ait nesne örneğini kullanarak servisimize ilişkin pek çok bilgiyi elde edebiliriz. Örneğin, servisin görünen adını DisplayName özelliği ile, servisin çalıştığı makine adını MachineName özelliği ile elde edebiliriz. ServiceController sınıfının üyeleri yardımıyla sistemimizde kurulu olan servislere ait pek çok bilgiyi temin edebiliriz. Bu konuyu bir sonraki makalemizde incelemeye çalışacağız. Şimdi dilerseniz servisimizi kontrol ediceğimiz kısa windows uygulamamızın kodlarını yazalım.

```csharp
ServiceController sc; /* Servisimizi kontrol edicek olan ServiceController sınıf nesnemiz tanımlanıyor.*/

private void Form1_Load(object sender, System.EventArgs e)
{
    sc=new ServiceController("BosAlanTakipServis"); /* ServiceController nesnemiz, kullanmak istediğimiz servisin adını parametre olarak almak suretiyle oluşturuluyor.*/
    lblServisAdi.Text=sc.DisplayName.ToString()+"/"+sc.MachineName.ToString(); /* Servisimizin , sistemdeki services kısmında görünen ismi ve üzerinde çalıştığı makine adı elde ediliyor ve label kontrolümüze ekleniyor.*/
}

private void btnPeriyod_Click(object sender, System.EventArgs e)
{
    if(lbPeriyod.SelectedIndex==0)
    {
        sc.ExecuteCommand(203); /* Servisimizdeki OnCustomCommand metodu çalıştırılıyor ve bu metoda parametre değeri olarak 203 gönderiliyor. Artık servisimiz, 203 parametre değerine göre bir takım işlevler gerçekleştirecek. 203 değeri karşılığında servisimizdeki OnCustomCommand metodu, log tutma süresini yarım saat olarak belirleyecektir.*/
    }
    else if(lbPeriyod.SelectedIndex==1)
    {
        sc.ExecuteCommand(204);
    }
    else if(lbPeriyod.SelectedIndex==2)
    {
        sc.ExecuteCommand(205);
    } 
}

private void btnAltSinirAyarla_Click(object sender, System.EventArgs e)
{
    if(lbAltSinir.SelectedIndex==0) 
    {
        sc.ExecuteCommand(201);
    }
    else if(lbAltSinir.SelectedIndex==1)
    {
        sc.ExecuteCommand(200);
    }
    else if(lbAltSinir.SelectedIndex==2)
    {
        sc.ExecuteCommand(202);
    }
}
```

Şimdi uygulamamızı çalıştıralım ve ilk olarak süreyi 3 saniyede 1 seçerek Event Log'ların tutuluşunu izleyelim. Windows uygulamamızı başlatmadan önce servisimizi manuel olarak başlatmayı unutmayalım. Diğer yandan bunu dilersek SCM yardımıyla otomatik hale getirebilir ve sistem açıldığında servisin otomatik olarak başlatılmasını sağlayabiliriz. Event Log süresini 3 saniye olarak belirleyip Ayarla başlıklı butona tıkladığımızda, servisimizi temsil eden ServiceController nesnesinin ExecuteCommand metodu devreye girerek, ListBox'tan seçilen öğenin indeksine göre bir değeri, temsil ettiği servis içindeki OnCustomCommand metoduna parametre olarak gönderir. Bunun sonucu olarak Event Log'umuzda 3 saniyede bir durum bilgisinin yazılıyor olması gerekir.

![mk68_10.gif](/assets/images/2004/mk68_10.gif)

Şekil 10. Servisin Event Log tutma süresinin ayarlanması.

Görüldüğü gibi servisimiz bu yeni ayarlamadan sonra, kendi oluşturduğumuz Event Log'a 3 saniyede 1 bilgi yazmaktadır. Servisimizde, C sürücüsü için belli bir miktar kapasitenin altına düşüldüğünde, Event Log'a yazılan mesaj bir uyarı simgesi ile birlikte yazılıyor. Şu an için, belirtilen kapasitenin altında olduğumuzdan warning simgesi, servisin timer nesnesinin elapsed olayında değerlendiriliyor ve Event Log'umuza yazılıyor. Şimdi bu kapasiteyi 2 GB yapalım ve durumu gözlemleyelim.

![mk68_11.gif](/assets/images/2004/mk68_11.gif)

Şekil 11. Alt Sinir değerinin değiştirilmesi.

Görüldüğü gibi, servisimizin alt sınıra göre Event Log'ların simgesini belirleyen davranışını değiştirmeyi başardık. Bu işlemi ExecuteCommand metodu ile servisimizdeki OnCustomCommand'a gönderdiğimiz parametrenin değerini ele alarak gerçekleştirdik.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde sistemimizde yer alan servisleri, ServiceController sınıfındaki metod ve özellikler ile nasıl kontrol edebileceğimizi incelemeye çalışacağız. Bu makalede görüşünceye dek hepinize mutlu günler dilerim.