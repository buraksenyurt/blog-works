---
layout: post
title: "VSTS 2008 için Custom Check-In Policy Geliştirmek"
date: 2009-08-07 12:56:00 +0300
categories:
  - visual-studio
tags:
  - vsts-2008
---
Bir süredir Team Foundation Server üzerinde ve doğal olarak Visual Studio Team System 2008 geliştirme ortamında çalışmaktayım. Tabi uzun yıllar Visual Source Safe ile vakit geçirmenin sonucunda, TFS ile birlikte gelen pek çok nimetin farkına çok geç varabiliyoruz. Herşeyden önce TFS'in, MSF (Microsoft Solution Framework) ve CMMI (Capability Maturity Model Integration) gibi yazılım geliştirme süreç modellerinin uygulanabildiği profesyonel bir çevre sağladığını bilmemiz gerekiyor.

![blg58_Giris.jpg](/assets/images/2009/blg58_Giris.jpg)

İşin içerisine Reporting Services'ile etkili raporlama, SharePoint ile tutarlı, ölçeklenebilir döküman yönetimi gibi pek çok yararlı üründe giriyor. Tabiki bu tip sistemlerin uygulanması her zaman kolay değildir. Her şeyden önce bir öğrenme süreci için geliştirme ekibinin ciddi zaman ayrılması şarttır. Öyleki, model içerisinde çevik (Agile) süreçlerin uygulanabilirliği söz konusudur ki bunlarda başlı başlına birer konsepttir.

Biz bu yazımızda Visual Studio Team System 2008 ile çalışırken, özel Check-In ilkelerinin (Custom Check-In Policy) nasıl geliştirilebileceğini basit bir örnek üzerinden ele almaya çalışacağız. Genellikle yazılım projelerinde görev alan geliştiricilerin, en alt kademeden en üst kademeye doğru çıktıkça (Junior -> Architect) çok daha az sayıda Check-In yaptıkları görülebilir

![Wink](/assets/images/2009/smiley-wink.gif)

Bu aslında iyi yazılım geliştirme süreçlerinde bir kural olarak değerlendirilmesi gereken durumlardandır. Söz gelimi bir projenin mesai sonunda eğer derlenemiyorsa Check-In'lenmemesi (ki bunu VSTS üzerinden vereceğiniz hazır bir Policy ile kolayca garantileyebilirsiniz), gün içinde belkide 1 en fazla 2 Check-In yapılmasına izin verilmesi, özellikle versiyon takibi açısından da son derece önemlidir. Peki VSTS tarafından bizlere sunulan hazır Check-In ilkeleri (Builds, Code Analysis, Testing Policy, Work Items) dışında kendi özel politikalarımızı nasıl geliştirebilir ve projeye uygulayabiliriz?

Aslında burada amaç, VSTS'in dış ortama sunulan bazı arayüzlerini kullanarak kendisine yeni davranışlar ekleyebilmektir. Yani bir Plug-In modeli ile karşı karşıya olduğumuzu düşünebiliriz. Dilerseniz bu fikirden yola çıkalım. İşe ilk olarak basit bir Class Library geliştirerek başlamamız gerekiyor. Söz konusu sınıf kütüphanesine, Microsoft.TeamFoundation.Version.Client isimli referansın eklenmesi gerekmektedir. Bu referans varsayılan kuruluma göre C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\PrivateAssemblies klasörü altında bulunmaktadır. Refernasların eklenmesinin arından, PolicyBase isimli tipten türetilen bir sınıfın geliştirilmesi ve bunun içerisinde gerekli üyelerin ezilmesi (Override) gerekmektedir.

Örneğimizde, TFS üzerindeki bir projeye eklenen dosyaların oluşturulduktan sonra, ne kadar süre sonra Check-In'lenebileceklerine dair bir ilke tanımlamaya çalışıyor olacağız. Eğer oluşturulma zamanı, gün değerinden küçük ise Check-In işlemi yapılırken uyarı (Warning) mesajı verilmesini sağlayacağız. Bununla birlikte istenirse Check-In için gerekli gün sayısını belirleyebileceğimiz basit bir iletişim penceremizde olacak. Böylece istenirse gün bazındaki Timeout süresi Edit seçeneği ile değiştirilebilir olacak. Bu tabiki tamamen sembolik bir örnek. (Bazı kaynaklarda Check-In lenecek dosyalar içerisinde yasaklı kelimelerin bulunmasının önüne geçilmesi veya kod standartlarından çok özel olan bazılarına uyulmadığı durumlarının ele alınması için geliştirilen ilkeler yer almaktadır. Sizde projeninizin ihtiyacı olan ilkeleri modelleyebilirsiniz.)

Örneğimizde yer alan Timeout isimli sınıfa ait kod yapısı aşağıda görüldüğü gibidir.

```csharp
using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Windows.Forms;

namespace TimeoutPolicy
{
    [Serializable] // Serileştirilebilir olma şartı
    public class Timeout
        : PolicyBase // Custom policy yazmak için PolicyBase tipinden türetme yapmak gerekmektedir
    {
        // Minimum Check-In süresi        
        public int TimeoutDay { get; set; }

        public Timeout()
        {
            TimeoutDay=1;
        }

        public override string Description
        {
            get { return "Bir Check-In işlemi yapılması için, dosyanın ilk oluşturulmasından sonra geçmesi gereken minimum gün süresi kontrolünü yapar"; }
        }

        // Policy içerisinde yer alan gün değeri istenirse editlenebilir
        public override bool Edit(IPolicyEditArgs policyEditArgs)
        {
            // Gün değerini almak için InputDayForm açılır
            // InputDayForm sınıfının yapıcı metoduna o anki Timeout sınıfının referansı gönderilir. Böylece form içerisinde this referansına ait gün değeri set edilebilir.
            InputDayForm inputForm = new InputDayForm(this);
            inputForm.ShowDialog();
            return true;
        }

        // Policy kontrolünün yapıldığı yerdir. Geriye var ise hata bildirimlerini döndürür
        public override PolicyFailure[] Evaluate()
        {
            List<PolicyFailure> failures = new List<PolicyFailure>();

            // Şu an beklemede olan tüm PendingChange referansları dolaşılır
            foreach (var item in PendingCheckin.PendingChanges.AllPendingChanges)
            {                
                // Eğer değişiklik örneğin bir dosya ile ilgiliyse
               if(item.ItemType== ItemType.File)
               {                   
                   // Dosyanın oluşturulma tarihi ile güncel tarih arasındaki farka bakılır
                   TimeSpan distance = DateTime.Now - item.CreationDate;
                   // Fark var ise
                   if (distance.TotalDays < TimeoutDay)
                   {                       
                       failures.Add(
                           new PolicyFailure(String.Format("{0} tarihli {1} için Check-In süresi dolmamış. Lütfen {2} gün bekleyiniz",item.CreationDate,item.FileName,TimeoutDay), this)
                           ); // Bir PolicyFailure nesnesi örneklenir ve mesaj bilgisi yazdırırılır.
                   }
                }
            }

            if (failures.Count > 0)
                return failures.ToArray();
            else
                return null;
        }

        public override string Type
        {
            get { return "Chech-In Timeout(Day)"; }
        }

        public override string TypeDescription
        {
            get { return "Gün bazlı Check-In süresi"; }
        }
    }
}
```

Timeout sınıfı serileştirilebilir (Serializable) olarak tanımlanmalıdır. Ayrıca PolicyBase tipinden türemelidir. Türeme sonucu ezilmesi (Override) gereken bazı üyeler olduğu görülmektedir. Bunlardan belkide en önemlisi Evaluate isimli metoddur. Bu metod içerisinde, senaryoda yer alan ilkenin uygulanması ve ilkenin aşılması halinde geriye PolicyFailure tipinden uyarı mesajlarının bir dizi şeklinde döndürülmesi sağlanmaktadır. Bizim örneğimizde dikkat edileceği üzere beklemede olan tüm Check-In'ler içerisinde tipi File olanlar ele alınmakta ve CreationDate özelliklerinin değerlerine bakılmaktadır. Eğer bu değer Timeout sınıfının kendi özelliği olan TimeoutDay değerinden küçük ise PolicyFailure oluşturulmaktadır. Ezilen diğer bir metodda Edit fonskiyonudur. Bu fonksiyon ile ilkenin düzenlenebilip düzenlemeyeceğine karar verilebilir. Biz örneğimizde, TimeoutDay özelliğinin değerinin değiştirilebileceği basit bir Windows Form kullanıyoruz. InputDayForm isimli bu Windows Form'un görüntüsü ve kod içeriği ise şu şekildedir;

![blg58_WinForm.gif](/assets/images/2009/blg58_WinForm.gif)

```csharp
using System;
using System.Windows.Forms;

namespace TimeoutPolicy
{
    public partial class InputDayForm : Form
    {
        private Timeout _timeOut;

        public int Day
        {
            get { return Convert.ToInt16(nupDay.Value); }
        }

        public InputDayForm(Timeout timeOut)
        {
            InitializeComponent();
            _timeOut = timeOut;
            nupDay.Value = _timeOut.TimeoutDay;
            btnClose.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _timeOut.TimeoutDay = (Int16)nupDay.Value;
        }
    }
}
```

Formumuz oluşturulurken, Timeout referansını almaktadır. Böylece Form üzerindeki NumericUpDown kontrolünde yapılan değişimlere göre, Timeout içerisindeki TimeoutDay değeri dinamik olarak değiştirilebilir.

İşlemlerimiz bununla bitmiyor tabiki. Geliştirdiğimiz sınıf kütüphanesinin, VSTS 2008 arabiriminde Team Exlporer üzerinden kullanılabilmesi için Registry'de ufak bir ekleme yapmamız gerekmektedir

![Undecided](/assets/images/2009/smiley-undecided.gif)

Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![blg58_RegEditContent.gif](/assets/images/2009/blg58_RegEditContent.gif)

Bu ekleme işlemi sırasında dikkat edilmesi gereken noktalardan birisi Value adının assembly adı ile aynı olmasıdır. Value Data kısmında ise dikat edileceği üzere geliştirdiğimiz sınıf kütüphanesinin fiziki adresi bulunmaktadır. Peki şimdi elimize ne geçti.

![Cool](/assets/images/2009/smiley-cool.gif)

VSTS 2008 üzerinde Source Settings kısmını açıp (aşağıdaki ekran görüntüsünde olduğu gibi),

![blg58_SourceControlSettings.gif](/assets/images/2009/blg58_SourceControlSettings.gif)

yeni bir Check-In Policy eklemek istediğimizde aşağıdaki durum ile karşılaşırız.

![blg58_AddCheckInPolicy.gif](/assets/images/2009/blg58_AddCheckInPolicy.gif)

Görüldüğü gibi az önce geliştirdiğimiz Custom Check-In Policy tipi burada yer almaktadır. Policy'yi ekledikten sonra dilersek Edit düğmesinide kullanabilir ve TimeoutDay özelliğinin değerini değiştirebiliriz.

![blg58_SourceControl.gif](/assets/images/2009/blg58_SourceControl.gif)

Süper. Artık durumu test edebiliriz. Ben TFS üzerindeki örnek bir projede bu ayarları yaptıktan ve yeni eklediğim bazı dosyaları Check-In'lemek istedikten sonra aşağıdaki ekran görüntüsünde yer alan uyarılar ile karşılaştım.

![blg58_CheckInPolicyWarnings.gif](/assets/images/2009/blg58_CheckInPolicyWarnings.gif)

TimeoutDay özelliğini 7 gün olarak set ettikten sonra başıma neler gelmiş neler?

![Sealed](/assets/images/2009/smiley-sealed.gif)

Elbette buradaki ilkeleri ezip geçebiliyoruz bildiğiniz üzere ama izleri kalıyor... Bu örnek ilkeyi uygulanızı önermem. Ancak sanıyorumki artık kendi özel Check-In ilkelerinizi nasıl geliştirebileceğinizi gördünüz. Böylece geldik bir yazımızın daha sonuna. Tekraradan görüşünceye dek hepinize mutlu günler dilerim.

[TimeoutPolicy.rar (30,07 kb)](/assets/files/2009/TimeoutPolicy.rar)
