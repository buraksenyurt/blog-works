---
layout: post
title: "Distributed Transactions"
date: 2003-11-19 12:00:00 +0300
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - dotnet
  - concurrency
  - transactions
  - visual-studio
---
Bildiğiniz gibi bir önceki makalemizde Transaction kavramından bahsetmiş, ancak birden fazla veritabanı için geçerli olucak Transaction işlemlerinin Dağıtık Transaction’lar olarak adlandırıldığından sözetmiştik. Bu makalemizde Dağıtık Transaction’ları inceleyecek ve her zaman olduğu gibi konuyu açıklayıcı basit bir örnek geliştireceğiz.

İş uygulamalarında, Online Transaction Processing (OLTP) dediğimiz olay çok sık kullanılmaktadır. Buna verilecek en güzel örnek bankaların ATM uygulamalarıdır. Veriler eş zamanlı olarak aynı anda bir bütün halinde işlenmekte ve güncellenmektedir. Bu tarz projelerin uygulanmasında OLTP tekniği yaygın bir biçimde kullanılmaktadır. Bu tekniğin uygulanabilmesi Dağıtık Transaction’ların kullanılmasını gerektirir..NET ile Dağıtık Transaction’lar yazmak için Component Services’ı kullanmamız gerekmektedir. Özellikle,çok katlı mimari dediğimiz iş uygulamalarında Dağıtık Transaction’ları çok sık kullanırız. Burada Dağıtık Transaction’lar başka componentler tarafından üstlenilir ve Sunu Katmanı ile Veritabanları arasındaki işlemlerin gerçekleştirilmesinde rol oynarlar. Bu component’lerin bulunduğu katman İş Katmanı olarakda adlandırılmaktadır.

Nitekim Componentler aslında Transaction başlatıp gerekli metodları çalıştırarak veriler üzerindeki bütünsel işlevleri yerine getirir ve transaction’ı sonlandırırlar. Yani Sunum Katmanı’ nda yer alan uygulamalar sadece gerekli verileri parametre olarak iş katmanında yer alan componentlere gönderirler. Dolayısıyla üzerlerinde ilgili veritabanı verileri için herhangibir fonksiyon veya metodun çalıştırılmasına gerek yoktur. Bütün sorumluluk Component Services ‘ da yer alan COM+ componentindedir. Burada veirtabanlarına bağlanılır, gerekli düzenlemeler bir Transaction içersinde gerçekleştirilir ve sorunsuz bir transaction tüm iş parçacıkları ile teyid edilerek gerekli düzenlemeler veritabanlarına yansıtılır.

En basit haliyle 3 katlı mimaride, Sunum Katmanı ile Veritabanları arasındaki transaction işlemlerini COM+ Component’leri ile gerçekleştirebiliriz. Bu component’ler Windows 2000’ den itibaren Component Service olarak adlandırılan bir servis altında yer almaktadırlar. Elbetteki bu component’i biz geliştireceğiz. Component’in görevi, transaction işlemlerinin otomatik yada manuel olarak gerçekleştirilmesini sağlamaktır. Bir dll dosyası haline getirilen bu component’leri istenilen Sunum Katmanı uygulamasına ekleyerek kullanabiliriz.

Yazacağımız component içinde Transaction işlemlerini kullanabilmek amacıyla.NET içerisinde yer alan System.EnterpriseServices sınıfının metodlarını kullanırız. Oluşturulan component’i örneklerimizde de göreceğiniz gibi bir Strong Name haline getirmemizde gerekmektedir. Örneğimizi yazarken bunları daha iyi anlıyacaksınız.Üç katlı mimaride, Dağıtık Transaction uygulamalarının aşağıdaki şekil ile zihnimizde daha berrak canlanacağı kanısındayım.

![mk7_1.gif](/assets/images/2003/mk7_1.gif)

Şekil 1. 3 Katlı Mimari için COM+ Kullanımı

Özetlemek gerekirse, Dağıtık Transaction’ ların kullanıldığı uygulamalarda Component Services kullanılması gerekmektedir. Bir Dağıtık Transaction Component’i yazdığımızda, transaction işlemlerini otomotik olarak Component Service’a yaptırabiliriz. Bunun yanında ContexrUtil adı verilen nesneyi ve bu nesneye ait SetComplete (), SetAbort () gibi metodları kullanarak Transaction işlmelerini elle de yapılandırabiliriz. Bu makalemizde otomatik olanı seçtim. Bir sonraki makalede işlemleri manuel olarak yapıcağız.Dilerseniz örneğimize geçelim. Bu uygulamızda 2 veritabanı üzerindeki 2 farklı tablo için, bir Transaction içerisinde basit veri giriş işlemleri gerçekleştireceğiz. Öncelikle tablolarımın yapısını ve databaselerimizi belirtelim. Firends isimli bir Veritabanı’ nda kullanacağımız basit bir tablo var. Satislar isimli bu tabloda Ad,Soyad ve SatisTutari alanları yer almakta.

![mk7_2.gif](/assets/images/2003/mk7_2.gif)

Şekil 2. Satislar tablosunun yapısı.

İkinci Veritabanımız ise IstanbulMerkez. Tablolamuzun adı Primler. Bu tabloda ise yine Ad,Soyad ve Prim bilgisi yer alıyor.

![mk7_3.gif](/assets/images/2003/mk7_3.gif)

Şekil 3. Primler tablosunun yapısı.

Uygulamamızda Satislar isimli tabloya bilgi girildikten sonra SatisTutari’nin %10’ u üzerinden prim hesaplanıcak ve aynı anda Primler tablosuna bu bilgiler eklenecek. Tabiki bu iki basit veritabanı işlemi bir Transaction içinde gerçekleştirilecek. Uygulamamızı tasarlamaya başlayalım. Önce yeni bir C# ile yeni bir Windows Application oluşturalım. Bu uygulamanın içerdiği Form Sunum Katmanı’ nda yer alan veri giriş ekranımız olucaktır. Formu aşağıdakine benzer veya aynı şekilde tasarlayalım.

![mk7_4.gif](/assets/images/2003/mk7_4.gif)

Şekil 4. Formun yapısı.

Kullanıcı bu ekrandan Ad,Soyad ve Satış Tutarı bilgilerini girecek. Girilen bu bilgiler, yazacağımız COM+ Componentindeki metoda parametre olarak gidicek ve bu metod içinde işlenerek veritabanlarındaki tablolarda gerekli düzenlemeler yapılıcak. Eğer tüm işlmeler başarılı olursa ve metod tam olarak çalışırsa geriyede işlemlerin dolayısıyla Transaction’ın başarıl olduğuna dair bir string bilgi gönderecek. Evet şimdi uygulamanın en önemli kısmına sıra geldi. Componentin tasarlanmasına. İlk önce, Project menüsünden Add Component komutunu vererek component’ imizi uygulamamıza ekliyoruz.

![mk7_5.gif](/assets/images/2003/mk7_5.gif)

Şekil 5. Component Eklemek.

Ben componentimize SatisPrimEkle adini verdim. Bu durumda Solution’ımıza SatisPrimEkle.cs isimli dosya eklenir ve Visual Studio.NET IDE’de aşağıdaki gibi görünür.

![mk7_6.gif](/assets/images/2003/mk7_6.gif)

Şekil 6. Componentin ilk eklendiğinde IDE’ de durum.

Şimdi ise bu component içersinde yer alıcak dağıtık transaction işlemleri için gerekli olan referansımızın projemize eklememize gerekiyor. Daha öncede System.EnterpriseServices olarak bahsettiğimiz bu sınıfı eklemek için yine, Project menüsünden, Add Reference komutunu veriyoruz. Burada ise.NET sekmesinden System.EnterpriseServices sınıfını ekliyoruz.

![mk7_7.gif](/assets/images/2003/mk7_7.gif)

Şekil 7. System.EnterpriseServices Sınıfının eklenmesi.

Şimdi Componentimizin kodlarını yazmaya başlayabiliriz. To Switch To Code Window linkine tıklayarak component’ imizin kodlarına geçiş yapıyoruz. İlk haliye kodlar aşağıdaki gibidir.

```csharp
using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics; 
namespace distrans
{
     /// <summary>
     /// Summary description for SatisPrimEkle.
     /// </summary>
     public class SatisPrimEkle : System.ComponentModel.Component
     {
          /// <summary>
          /// Required designer variable.
          /// </summary>
          private System.ComponentModel.Container components = null;
           public SatisPrimEkle(System.ComponentModel.IContainer container)
          {
               ///
               /// Required for Windows.Forms Class Composition Designer support
               ///
               container.Add(this);
               InitializeComponent();
                //
               // TODO: Add any constructor code after InitializeComponent call
               //
          } 
          public SatisPrimEkle()
          {
               ///
               /// Required for Windows.Forms Class Composition Designer support
               ///
               InitializeComponent();
               //
               // TODO: Add any constructor code after InitializeComponent call
               //
          } 
          /// <summary>
          /// Clean up any resources being used.
          /// </summary>
          protected override void Dispose( bool disposing )
          {
               if( disposing )
               {
                    if(components != null)
                    {
                         components.Dispose();
                    }
               }
               base.Dispose( disposing );
          } 
          #region Component Designer generated code
          /// <summary>
          /// Required method for Designer support - do not modify
          /// the contents of this method with the code editor.
          /// </summary>
          private void InitializeComponent()
          {
               components = new System.ComponentModel.Container();
          }
          #endregion
     }
}
```

Biz buradaki kodları aşağıdaki şekliyle düzenleyecek ve yukarıda yazılı çoğu kodu çıkartacağız. Haydi başlayalım. Öncelikle using kısmına,

```csharp
using System.Data.SqlClient;
using System.EnterpriseServices;
```

sınıflarını eklememiz gerekiyor. Çünkü Transaction işlemleri için EnterpriseServices sınıfını ve veritabanı işlemlerimiz içinde SqlClient sınıfında yer alan nesnelerimizi kullanacağız. İkinci köklü değişimiz ise SatisPrimEkle isimli sınıfımızın ServicedComponent sınfından türetilmiş olması. Bu değişikliği ve diğer fazlalıkları çıkarttığımız takdirde, kodlarımızın son hali aşağıdaki gibi olucaktır.

```csharp
using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;
using System.EnterpriseServices; 
namespace distrans
{
     public class SatisPrimEkle : ServicedComponent
     {   
     }
}
```

Şimdi metodumuzu ekliyelim ve gerekli kodlamaları yazalım.

```csharp
using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;
using System.EnterpriseServices; 
namespace distrans
{
     /* [Transaction(TransactionOption.Required)] satırı ile belirtilen şudur. Component’ imiz var olan Transaction içerisinde çalıştırılacaktır. Ancak eğer oluşturulmuş yani başlatılmış bir transaction yoksa, bu component’ imiz için yeni bir tane oluşturulması sağlanacaktır. Burada,

     TransactionOption'ın sahip olabileceği diğer değerler Disabled, NotSupported, RequiresNew ve Supported dır.

     Disabled durumunda, transaction özelliği görmezden gelinir. Default olarak bu değer kabul edilir. Bu durumda Transaction başlatılması gibi işlemler manuel olarak yapılır.

     Not Supported durumunda ise Component’ imiz bir transaction var olsa bile bu transaction'ın dışında çalışıcaktır.

     RequiresNew durumunda, Component’ imiz için bir transaction var olsada olmasada mutlaka yeni bir transaction başlatılacaktır.

     Supported durumu ise , var olan bir transaction olması durumunda, Component’ imizin bu transaction'a katılmasını sağlar.

     Biz uygulamamızda otomatik transaction tekniğini kullandığımız için Required seçeneğini kullanıyoruz.

     */

     [Transaction(TransactionOption.Required)]public class SatisPrimEkle : ServicedComponent
     {
     /* AutoComplete() satırı izleyen metodun bir transaction içerisinde yer alacağını ve transaction işlemlerinin başlatılması ve bitirilmesini Component Services 'ın üstleneceğini belirtir. Dolayısıyla Component’ imizin bu metodunu çalıştırdığımızda bir transaction başlatılır ve ContexUtil nesnesi ile manuel olarak yapacağımız SetComplete (Commit) ve SetAbort(Rollback) hareketlerini COM+ Servisi kendisi yapar. */ 
          [AutoComplete()]public string VeriGonder(string ad,string soyad,double satisTutari)
          {
               SqlConnection conFriends = new SqlConnection("initial catalog=Friends;data source=127.0.0.1;integrated security=sspi;packet size=4096");
               SqlConnection conIstanbulMerkez = new SqlConnection("initial catalog=IstanbulMerkez;data source=127.0.0.1;integrated security=sspi;packet size=4096"); 
               /* Yukarıdaki SqlConnection nesneleri tanımlanırken data source özelliklerine sql sunucusunun bulunduğu ip adresi girildi. Bu farklı ip'lere sahip sunucular söz konusu olduğunda farklı veritabanlarınıda kullanabiliriz anlamına gelmektedir. Uygulamamızı aynı sunucu üzerinde gerçekleştirmek zorunda olduğum için aynı ip adreslerini verdim.*/ 
               /* Aşğıdaki satırlarda veri girişi için gerekli sql cümlelerini hazırladık ve bunları SqlCommand nesneleri ile ilişkilendirip çalıştırdık. */
               string sql1="INSERT INTO Satislar (Ad,Soyad,SatisTutari) VALUES ('"+ad+"','"+soyad+"',"+satisTutari+")";
               double prim=satisTutari*0.10; 
               string sql2="INSERT INTO Primler (Ad,Soyad,Prim) VALUES ('"+ad+"','"+soyad+"',"+prim+")"; 
               SqlCommand cmdSatisGir=new SqlCommand(sql1,conFriends);
               SqlCommand cmdPrimGir=new SqlCommand(sql2,conIstanbulMerkez);
               conFriends.Open();
               conIstanbulMerkez.Open(); 
               cmdSatisGir.ExecuteNonQuery();
               cmdPrimGir.ExecuteNonQuery(); 
               return "ISLEM TAMAM"; /* Metod başarılı bir şekilde çalıştığında, COM+ sevisi transaction'ı otomatik olarak sonlandırır ve metodumuz geriye ISLEM TAMAM stringini döndürür. */ 
          }
     }
}
```

Component’ imizi oluşturduktan sonar bunu Sunum Katmanındaki uygulamalarda kullanabilmek ve COM+ Servisi’nede eklenmesini sağlamak için bir Strong Name Key dosyası oluşturmamız gerekiyor. Bu IDE dışından ve sn.exe isimli dosya ile yapılan bir işlemdir. Bunun için D:\Program Files\Microsoft.NET\FrameworkSDK\Bin\sn.exe dosyasını kullanacağız. İşte komutun çalıştırılışı;

![mk7_8.gif](/assets/images/2003/mk7_8.gif)

Şekil 8. sn.exe ile snk (strong name key) dosyasının oluşturulması.

Görüldüğü gibi snk uzantılı dosyamız oluşturuldu. Şimdi Formumuzda bu Component’ imize ait metodu kullanabilmek için oluşturulan bu snk uzantılı dosyayı Solution’ımıza Add Exciting Item seçeneği ile eklememiz gerekiyor. Aşağıdaki 3 şekilde bu adımları görebilirsiniz.

![mk7_9.gif](/assets/images/2003/mk7_9.gif)

![mk7_10.gif](/assets/images/2003/mk7_10.gif)

![mk7_11.gif](/assets/images/2003/mk7_11.gif)

Şekil 9. SatisPrimEkle.snk dosyasının projemize eklenmesi.

Formumuzda Component’ imize ait metodu kullanabilmek için yapmamız gereken bir adım daha var. Oda uygulamanın AssemblyInfo.cs dosyasına aşağıdaki kod satırını eklemek.

```csharp
[assembly: AssemblyKeyFile("..\\..\\SatisPrimEkle.snk")]
```

Şimdi formumuzdaki kodları inceleyelim. Öncelikle SatisPrimEkle tipinde bir nesne tanımlıyoruz. Şimdi bu nesnesin VeriGonder isimli metoduna eriştiğimizde aşağıdaki şekilde gördüğünüz gibi IntelliSense özelliği bize kullanabileceğimiz parametreleride gösteriyor.

![mk7_12.gif](/assets/images/2003/mk7_12.gif)

Şekil 10. Metodun kullanım.

Şimdi tüm kodumuzu tamamlayalım ve örneğimizi çalıştıralım.

```csharp
private void btnGonder_Click(object sender, System.EventArgs e)
{
     SatisPrimEkle comp=new SatisPrimEkle();
     double st=System.Convert.ToDouble(txtSatisTutari.Text); 
     try
     {
MessageBox.Show(comp.VeriGonder(txtAd.Text,txtSoyad.Text,st)); 
     }
     catch(Exception hata)
     {
          MessageBox.Show(hata.Source + ":" + hata.Message);
     }
}
```

Şimdi örneğimizi çalıştıralım.

![mk7_13.gif](/assets/images/2003/mk7_13.gif)

![mk7_14.gif](/assets/images/2003/mk7_14.gif)

Şekil 11. ISLEM TAMAM

Görüldüğü gibi, metodumuz başarılı bir şekilde çalıştırıldı. Hemen tablolarımızı kontrol edelim.

![mk7_15.gif](/assets/images/2003/mk7_15.gif)

Şekil 12. IstanbulMerkez veritabanındaki Primler Tablosu.

![mk7_16.gif](/assets/images/2003/mk7_16.gif)

Şekil 13. Friends veritabanındaki Satislar tablosu.

Şimdi Component Services’a bakıcak olursak oluşturmuş olduğumuz distrans isimli Component Application’ı ve içerdiği SatisPrimEkle isimli Component’i, burada da görebiliriz.

![mk7_17.gif](/assets/images/2003/mk7_17.gif)

Şekil 14. Componet Services.

Bir sonraki makalemizde aynı örneği Manuel yöntemelerle ve dolayısıyla ContextUtil sınıfının metodları ile gerçekleştireceğiz. Hepinize mutlu günler dilerim.