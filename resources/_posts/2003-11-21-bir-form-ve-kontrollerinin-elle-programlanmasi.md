---
layout: post
title: "Bir Form ve Kontrollerinin Elle Programlanması"
date: 2003-11-21 12:00:00 +0300
categories:
  - windows-forms
tags:
  - windows-forms
---
Bugünkü makalemizde, bir Formu kodla nasıl oluşturacağımızı, bu form üstüne nasıl kontroller ekleyeciğimizi, bu kontoller için nasıl olaylar yazacağımızı vb. konuları işlemeye çalışacağız. Bildiğiniz gibi Visual Studio.NET gibi grafiksel ortamlar ile Form ve Form nesnelerini görsel olarak, kolay ve hızlı bir şekilde oluşturabilmekteyiz. Bu bizim programlama için ayıracağımız sürede, ekran tasarımlarının daha hızlı yapılabilmesine olanak sağlamaktadır.

Ancak bazen elimizde sadece csc gibi bir C# derleyicisi ve.Net Framework vardır. İşte böyle bir durumda, Windows Form’larını tasarlamak için manuel olarak kodlama yapmamız gerekmektedir. Ayrıca, iyi ve uzman bir programcı olabilmek için özellikle Visual ortamlarda Windows Form, Button, TextBox gibi kontrollerin nasıl oluşturulduğunu, nasıl kodlandığını olaylara nasıl ve ne şekilde bağlandığını bilmek, oldukça faydalıdır. Bu aynı zamanda kontrolün bizde olmasını da sağlayan bir unsur olarak karşııza çıkmar ve kendimize olan güvenimizi dahada arttırır.

Dilerseniz konuyu anlamak için basit ama etkili bir örnekle başlayalım. Bu örneğimizde basit olarak boş bir Form oluşturacağız ve bunu csc.exe (C# Compiler) ile derleyeceğiz. Bir Windows Formu aslında System.Windows.Forms sınıfından türeyen bir nesnedir. Bu nedenle oluşturacağımız C# sınıfı içersinde bu bildirimi gerçekleştirmeliyiz. Ayrıca sınıfımızın Form nesnesine ait elemanlarıda kullanabilmesi için, Form sınıfından türetmeliyiz (Inherting). Bunlara ek olarak Formumuzu ekranda gösterebilmek için Application nesnesini ve buna bağlı Run metodunu kullanacağız. Hemen bir text editor açalım ve burada aşağıdaki kodları girelim.

```csharp
using System.Windows.Forms; 

public class BirForm:Form // Form sınıfının elemanlarını kalıtısal olarak devralıyoruz.
{ 
     public static void Main() // Programın başladığı nokta.
     {
          BirForm yeni1=new BirForm(); // BirForm sınıfından bir nesne tanımlıyoruz. 
          Application.Run(yeni1);  
          /* yeni1 isimli Form nesnemiz Application nesnesi tarafından görüntülenir. Bu noktadan itibaren programın işleyişi bir döngüye girer. Bu döngüde Application nesnesi sürekli olarak programın işleyişini sonlandıracak bir olayın tetiklenip tetiklenmediğini de kontrol eder. Bu arada tabi yazılan diğer olaylar ve metodlar çalışır. Ancak program sonlandırma ile ilgili ( örneğin Close metodu ) bir kod ile karşılaşıldığında veya kullanıcı Form’un varsa kapatma simgesine tıkladığında (veya ALT+F4 yaptığında) Application nesnesi artık programın işleyişini durdurur. */
     }
}
```

Yazdığımız bu dosyayı cs uzantısı ile kaydetmeyi unutmayalım. Şimdi bu dosyayı csc.exe ile derleyelim. Programı derlerken dikkat etmemiz gereken bir nokta var. System.Windows.Forms ‘ un programa referans edilmesi gerekir. Bunu sağlamak için derleme komutumuzun nasıl yazıldığına dikkat edelim. /reference: burada devreye girmektedir.

![mk6_1.gif](/assets/images/2003/mk6_1.gif)

Şekil 1. İlk Derleme

Görüldüğü gibi, csc dosyamızı derlemiş ve CreateForm.exe dosyasını olşturmuştur. Burada /t:winexe programı Windows işletim sistemine, " Ben bir WinForm’um " olarak tanıtmaktadır. Şimdi bu dosyayı komut satırından çalıştıracak olursak aşağıdaki şekilde görülen sonucu elde ederiz.

![mk6_2.gif](/assets/images/2003/mk6_2.gif)

Şekil 2. Oluşturulan Form Nesnemiz.

Şekil 2.'de oluşturulumuş olduğumuz Form nesnesini görebilirsiniz. Yazmış olduğumuz kodlarda bu Form nesnesine ait özellikleri değiştirerek farklı Form görünümleride elde edebiliriz. Bu noktada size Form oluşturma olaylarının kodlama tekniğinin aslen nasıl yapılması gerektiğini göstermek isterim. Bu bir tarzdır yada uygulanan bir formattır ancak en uygun şekildir. Nitekim Visual Studio.NET ortamında bir Windows Application geliştirdiğinizde, uygulama kodlarına bakıcak olursanız bahsetmiş olduğumuz formasyonun uygulanmış olduğunu göreceksiniz.

```csharp
using System.Windows.Forms; 
public class BirForm:Form
{ 
     public BirForm() // Constructor(Yapıcı) metodumuz.
     {
          InitializeComponent(); /* BirForm sınıfından bir Form nesnesi üretildiğinde new yapılandırıcısı bu constructora bakar ve InitializeComponent metodunu çağırır. */
     } 
     private void InitializeComponent()
     {
          /* Burada Form'a ait özellikler ve Form üzerinde yer alacak nesneler tanılanır. Tanılanan nesneler aynı zamanda Form'a burada eklenir ve özellikleri belirlenir. */
     }
     public static void Main()
     {
          Application.Run(new BirForm()); 
     }
}
```

Bu yazım tekniği daha anlamlı değil mi? Kodumuzu bu şekilde değiştirip çalıştırdığımızda yine aynı sonucu elde ederiz. Ancak dilersek bu kodu şu şekilde de yazabiliriz.

```csharp
using System.Windows.Forms; 

public class BirForm:Form
{ 
     public BirForm()
     {
          NesneleriAyarla();
     } 
     private void NesneleriAyarla()
     {
     }
     public static void Main()
     {
          Application.Run(new BirForm());  
     }
} 
```

Yine aynı sonucu alırız.Şimdi Formumuza biraz renk katalım ve üstünede bir kaç nesne ekleyelim.

```csharp
using System;
using System.Windows.Forms;
using System.Drawing; 

public class BirForm:Form
{
     /* Kullanacağımız nesneler tanımlanıyor. Iki adet Label nesnesi, iki adet TextBox nesnesi ve birde Button kontrolü. */
     private Label lbl1;
     private Label lbl2;
     private TextBox txtUsername;
     private TextBox txtPassword;
     private Button btnOK; 
     public BirForm()
     {
          NesneleriAyarla();
     } 

     private void NesneleriAyarla()
     {
          this.Text="Yeni Bir Form Sayfası"; /* this anahtar kelimesi oluşturulan Form nesnesini temsil eder.*/
          this.BackColor=Color.Silver; /* Formun arka plan rengini belirliyoruz. Color Enumaration'ınını kullanabilmek için Drawing sınıfının eklenmiş olması gereklidir. */
          this.StartPosition=FormStartPosition.CenterScreen; /* Form oluşturulduğunda ekranın ortasında görünmesi sağlanıyor. */
          this.FormBorderStyle=FormBorderStyle.Fixed3D; /* Formun border çizgileri 3 boyutlu ve sabit olarak belirleniyor. */ 
          /* Label nesnelerini oluşturuyor ve özelliklerini ayarlıyoruz. */ 
          lbl1=new Label();
          lbl2=new Label(); 
          lbl1.Text="Username";
          lbl1.Location=new Point(50,50); /* lbl1 nesnesini 50 birim sağa 50 birim aşağıya konumlandırıyoruz */
          lbl1.AutoSize=true; /* Label'ın boyutunun text uzunluğuna göre otomatik olarak ayarlanmasını sağlıyoruz. */
          lbl2.Text="Password";
          lbl2.Location=new Point(50,100); /* Bu kez 50 birim sağa, 100 birim aşağıya yerleştiriyoruz. */
          lbl2.AutoSize=true; 
          /* TextBox nesnelerini oluşturuyor ve özelliklerini ayarlıyoruz. */ 
          txtUsername=new TextBox();
          txtPassword=new TextBox(); 
          txtUsername.Text="";
          txtUsername.Location=new Point(lbl1.PreferredWidth+50,50); /* Textbox nesnemizi lbl1 in uzunluğundan 50 birim fazla olucak şekilde sağa ve 50 birim aşağıya konumlandırıyoruz. */ 
          txtPassword.Text="";
          txtPassword.Location=new Point(lbl2.PreferredWidth+50,100); 
          /* Button nesnemizi oluşturuyor ve özelliklerini belirliyoruz */
          btnOK=new Button();
          btnOK.Text="TAMAM";
          btnOK.Location=new Point(0,0); 
          /* Buraya btnOK nesnesi için olay procedure tanımı eklenecek. */ 
          /* Şimdi kontrollerimizi Formumuza ekleyelim . Bunun için Form sınıfına ait Controls koleksiyonunu ve Add metodunu kullanıyoruz. */ 
          this.Controls.Add(lbl1);
          this.Controls.Add(lbl2);
          this.Controls.Add(txtUsername);
          this.Controls.Add(txtPassword);
          this.Controls.Add(btnOK); 
          this.Width=lbl1.PreferredWidth+txtUsername.Width+200; /* Son olarak formun genişliğini ve yüksekliğini ayarlıyoruz. */
          this.Height=lbl1.PreferredWidth+lbl2.PreferredWidth+200;
     }

     /* Buraya btnOK için Click olay procedure kodları eklenecek. */ 
     public static void Main()
     {
          Application.Run(new BirForm());  
     }
} 
```

Evet bu kodu derleyip çalıştırdığımızda aşağıdaki Form görüntüsünü elde etmiş oluruz.

![mk6_3.gif](/assets/images/2003/mk6_3.gif)

Şekil 3. Form tasarıını geliştiriyoruz.

Şimdi işin en önemli kısılarından birine geldi sıra. Oda olay güdümlü kodları yazmak. Yani kullanıcı etkilerine tepki vericek kodları yazmak. Kısaca Event-Handler. Kullanıcı bir eylem gerçekleştirdiğinde programın hangi tepkileri vereceğini belirtmek durumundayız. Bunun için şu syntax’ı kullanırız.

```csharp
protected void metodunAdi(object sender,System.EventArgs e)  
```

Burada metodumuzun hangi nesne için çalıştırılacağını sender anahtar kelimesi belirtir. Metod protected tanımlanır. Yani bulunduğu sınıf ve bulunduğu sınıftan türetilen sınıflarda kullanılabilir. Geri dönüş değeri yoktur. Bu genel formasyonla tanımlanan bir olay procedure’ünü nesne ile ilişkilendirmek için System.EventHandler delegesi kullanılır.

```csharp
nesneAdi.OlayinTanılayiciBilgisi+= new System.EventHandler(this.metodunAdi)
```

Yukarıdaki örneğimizde klasik örnek olarak, Button nesnemize tıklandığında çalıştırılması için bir Click olay procedure’ü ekleyelim.

Şimdi

/ * Buraya btnOK nesnesi için olay procedure tanımı eklenecek * / yazan yere, aşağıdaki kod satırını ekliyoruz.

```csharp
btnOK.Click+=new System.EventHandler(this.btnOK_Tiklandi);
```

Bu satır ile btnOK nesnesine tıklandığında btnOK_Tiklandi isimli procedure’ün çalıştırılacağını belirtiyoruz. / * Buraya btnOK için Click olay procedure kodları eklenecek * / yazan yere ise olay procedure’ümüzün ve kodlarını ekliyoruz.

```csharp
protected void btnOK_Tiklandi(object sender,System.EventArgs e)
{
     MessageBox.Show(txtUsername.Text+" "+txtPassword.Text);
}  
```

Şimdi programı tekrar derleyip çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk6_4.gif](/assets/images/2003/mk6_4.gif)

![mk6_5.gif](/assets/images/2003/mk6_5.gif)

Şekil 4. Event-Handler sonucu.

Evet geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu ve huzurlu günler dilerim.