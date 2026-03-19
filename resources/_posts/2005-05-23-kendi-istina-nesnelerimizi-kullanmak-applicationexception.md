---
layout: post
title: "Kendi İstina Nesnelerimizi Kullanmak (ApplicationException)"
date: 2005-05-23 12:00:00 +0300
description: "İstisna yakalama mekanizması (Exception Handling) dotNet mimarisinde oldukça önemli bir yere sahiptir. Bu mekanizma sayesinde uygulamalarımızın kilitlenmesi ve istem dışı bir şekilde sonlandırılmaya zorlanmasının önüne geçmiş oluruz. Framework içerisinde önceden tanımlanmış pek çok istisna sınıfı mevcuttur. Bu sınıflar yardımıyla, çalışma zamanında oluşabilecek istisnai durumlar kolayca tespit edilebilmektedir."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - xml
---
# Kendi İstina Nesnelerimizi Kullanmak (ApplicationException)
Değerli Okurlarım Merhabalar,

İstisna yakalama mekanizması (Exception Handling) dotNet mimarisinde oldukça önemli bir yere sahiptir. Bu mekanizma sayesinde uygulamalarımızın kilitlenmesi ve istem dışı bir şekilde sonlandırılmaya zorlanmasının önüne geçmiş oluruz. Framework içerisinde önceden tanımlanmış pek çok istisna sınıfı mevcuttur. Bu sınıflar yardımıyla, çalışma zamanında oluşabilecek istisnai durumlar kolayca tespit edilebilmektedir.

Böylece uygulamaların, CLR tarafından denetlendiği sırada ortama fırlatılan istisnalar nedeniyle yön değiştirebilmesi ve yaşamını sürdürebilmesi sağlanmış olmaktadır. Ancak bazı durumlarda kendi istisna sınıflarımızı yazma ihtiyacı duyabiliriz. Bunun pek çok nedeni olabilir. İlk ve en temel nedeni, sistemde var olan istisna sınıfları dışındaki bir istisnayı çalışma zamanında ele almak isteyişimizdir. İşte bu makalemizde bu işlevselliği nasıl gerçekleştirebileceğimizi basit bir örnek üzerinde incelemeye çalışacağız.

.Net Framework'te var olan istisna sınıflarının tamamı System.Exception sınıfından dolaylı bir şekilde türeyerek oluşturulmuşlardır. Kendi istisna nesnelerimizi oluşturabilmek ve yakalayabilmek için kullanacağımız sınıfları System isim alanında yer alan ApplicationException sınıfından türetiriz. (Aslında ApplicationException sınıfıda Exception sınıfından türemiştir.) Bu sayede throw anahtar sözcüğü vasıtasıyla oluşturduğumuz istisna nesnelerinin ortama fırlatılabilmesini sağlamış oluruz. Ortama fırlatılan istisna nesnelerini uygun try...catch bloklarında yakalayarak hem uygulamanın çökmesini engellemiş hem de kullanıcıların anlamlı mesajlar ile uyarılmasını sağlamış oluruz. ApplicatinException, kullanıcı tanımlı istisna sınıflarının tipik Exception sınıflarının sahip olduğu üyelerini kullanabilmesini sağlar. Bu tabiki kalıtımın bir sonucudur. Diğer yandan ApplicationException sınıfı, kendisinden türetilen sınıfın bir istisna nesnesi olduğunu ve throw anahtar sözcüğü ile fırlatılabileceğini de belirtir.

Konuyu daha iyi anlayabilmek amacıyla bir örnek üzerinden gideceğiz. Senaryomuzda aşağıda kodları bulunan Kitap isimli sınıfı kullanan bir uygulama yer alacak. Amacımız bu sınıfı kullanırken istisnaya neden olabilecek noktaları tespit etmek ve bu istisnaları yönetecek sınıfı yazarak uygulama içerisinden yön verebilmektir.

```csharp
using System;

namespace KitapDukkani
{
    public class Kitap
    {
        private string m_Kitap_Yazar;
        private string m_Kitap_Baslik;
        private double m_Kitap_Fiyat;
        private DateTime m_Kitap_Basim;
        private string m_Kitap_Kategori;

        public string Yazar
        {
            get
            {
                return m_Kitap_Yazar;
            }
            set
            {
                m_Kitap_Yazar=value;
            }
        }
        public string Baslik
        {
            get
            {
                return m_Kitap_Baslik;
            }
            set
            {
                m_Kitap_Baslik=value;
            }
        }
        public double Fiyat
       {
            get
            {
                return m_Kitap_Fiyat;
            }
            set
            {
                m_Kitap_Fiyat=value;
            }
        }
        public DateTime Basim
        {
            get
            {
                return m_Kitap_Basim;
            }
            set
            {
                m_Kitap_Basim=value;
            }
        }
        public string Kategori
        {
            get
            {
                return m_Kitap_Kategori;
            }
            set
            {
                m_Kitap_Kategori=value;
            }
        }

        public Kitap(string yazar,string baslik,double fiyat,DateTime basim,string kategori)
        {
            Yazar=yazar;
            Baslik=baslik;
            Fiyat=fiyat;
            Basim=basim;
            Kategori=kategori;
        }

        public Kitap()
        {        
        }
    }
}
```

Kitap sınıfımız tipik olarak bir kitabın temel özelliklerini sunan bir yapıya sahiptir. Bu sınıfı kullanacak olan bir yazılım geliştiricinin dikkat edeceği bir takım noktalar olacaktır. Örneğin, kitabın fiyatının negatif değer almaması, harf yada karakterlerden oluşmaması, basım tarihinin mutlaka tarihsel formatta olması gerekliliği vb. Bunlar uygulamanın çalışması esnasında hataya neden olabilecek durumlardır. Var olan istisna sınıfları yardımıyla bu tip hataları çalışma zamanında bertaraf ederek uygulamanın yaşamına devam etmesini sağlayabiliriz. Bunun yanında bu sınıfı kullanacak olan yazılım geliştirici kullanıcının hataya neden olacak girişlerini engelleyecek tedbirleri elbette göz önüne alacaktır ve uygulayacaktır. Örneğin aşağıdaki windows uygulamasında kullanıcı girişlerinde oluşabilecek veri girişi hataları ele alınmaya çalışılmıştır.

![mk122_1.gif](/assets/images/2005/mk122_1.gif)

İlk olarak alanlara girilecek olan karakter sayısı sınırlandırılarak çok uzun verilerin girilmeye çalışılması engellenebilir. Tarih girişlerinde oluşabilecek hataların önüne geçmek bir windows uygulaması için son derece kolaydır. DateTimePicker bileşeni bu kontrolü bizim için fazlasıyla sağlamaktadır. Kategori seçiminde ise ComboBox bileşeni kullanılabilir. Kategori veriside aslında belirli bir listeden alınmak zorundadır. Bu liste bir veritabanından alınabileceği gibi, bir XML dosyasından da alınabililir vb...Bizim için hataya neden olabilecek bir diğer durumda Fiyat alanına girilecek sayısal değerin karakter olarak girilmeye çalışılmasıdır. Bunu engelleyebilmek için sadece sayısal karakter girişine izin verecek bir metod kullanamız gerekir. Bunu gerçekleştirebileceğimiz en güzel yer ilgili TextBox kontrolünün KeyPress olay metodudur.

```csharp
private void txtFiyat_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
{
    if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && ((int)e.KeyChar!=8)) 
    {
        e.Handled=true;
    }
}
```

Ama halen daha yazılım geliştiricinin bilmediği ve bu gibi durumlarda kullanıcının uyarılmasını istediğimiz istisnai durumlar olabilir. Örneğin fiyat alanına her ne kadar negatif değer girilemiyecek olsada, fiyat alanındaki güncel değer belli bir oranda azaltılmaya çalışıldığında negatif değerin oluşabileceği görülebilir. Örneğin fiyat azaltımı için aşağıdaki metodu uyguladığımız varsayalım.

```csharp
private void btnIndirim_Click(object sender, System.EventArgs e)
{
    if(kitap!=null)
    {
        kitap.Fiyat-=10;
        txtFiyat.Text=kitap.Fiyat.ToString();
    }
}
```

Bu durumda ekran görüntüsü aşağıdaki gibi olacaktır.

![mk122_2.gif](/assets/images/2005/mk122_2.gif)

Görüldüğü gibi normal şartlar altında TextBox içerisine - karakterini basamasakta Kitap nesnemizi oluşturduktan sonra Fiyat özelliğinin değerinde yapacağımız 10 birimlik azaltmalar sonucu - değer görünebilmektedir. Bu bir bug olarak değerlendirilebilse de önüne geçmemiz gereken bir durumdur. Dahası Kitap sınıfını kullanan yazılım geliştirici bu tip bir kontrolü hiç yapmayadabilir. İşte böyle bir durumda en azından girilen değerin negatif olması durumunda ortama bir istisnanın fırlatılmasını sağlayabiliriz. Diğer yandan girilen Fiyat değerinin belli bir değerin üstünde olmamasını da isteyebiliriz. İşte bu iki basit nedeni ele alarak kendi istisna sınıfımızı yazabiliriz. İlk başta uygulamamızın Kitap nesnesi oluşturan kodunu standart try...catch yapsısı içinde kullanamayı deneyelim.

```csharp
private void btnOlustur_Click(object sender, System.EventArgs e)
{
    try
    {
        kitap=new Kitap();
        kitap.Baslik=txtBaslik.Text;
        kitap.Yazar=txtYazar.Text;
        kitap.Basim=dtpBasim.Value;
        kitap.Fiyat=Convert.ToDouble(txtFiyat.Text);
        kitap.Kategori=cmbKategori.SelectedText; 
    }
    catch(System.Exception err)
    {
        MessageBox.Show(err.Message,"Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
    }
}
```

Bu haliyle uygulamayı çalıştırdığımızda ve fiyat alanı için eksi değere geçtiğimizde ortama herhangi bir istisna nesnesinin fırlatılmadığını görürüz. Çünkü oluşacak istisnai durum henüz tarafımızdan yaratılmamıştır. Dolayısıyla ApplicationException sınıfından türeteceğimiz bir exception sınıfı yazmamız gerekmektedir. İşte bu amacımızı sağlayan FiyatException isimli istisna sınıfımızın kodları;

```csharp
using System;

namespace KitapDukkani
{
    /// <summary>
    /// Bir kitabın fiyatı ile ilgili istisna sınıfıdır.
    /// </summary>
    public class FiyatException:ApplicationException
    {
        /// <summary>
        /// İstisnaya neden olan fiyat değerini tutan özel field.
        /// </summary>
        private double m_Fiyat;
        /// <summary>
        /// İstisna ile ilgili kısa açıklama
        /// </summary>
        private string m_Mesaj;

        /// <summary>
        /// İstisna nesnesi oluşturulurken hataya neden olan fiyat değeri alınıp m_Fiyat özel alanına eşitlenir.
        /// </summary>
        /// <param name="fiyat">İstisnaya neden olan fiyat alanının değeridir.</param>
        public FiyatException(double fiyat,string mesaj)
        {
            m_Fiyat=fiyat;
            m_Mesaj=mesaj;
        }

        /// <summary>
        /// ApplicationException sınıfından gelen Message özelliği override edilmiştir. Geriye özel bir hata mesajı döndürmektedir.
        /// </summary>
        public override string Message
        {
            get
            {
                return "Kitaba ait fiyat değeri "+m_Fiyat+"belirtilen kriterlere uygun değildir."+" '"+m_Mesaj+"'";
            }
        }
    }
}
```

Görüldüğü gibi kendi yazdığımız istisna sınıflarının normal bir sınıfı yazmaktan hiç bir farkı yoktur. Kendi üyelerimizi ekleyebilir ve Exception sınıfından devralınan Message, TargetSite, InnerException gibi virtual özellikleri override edebiliriz. Sınıf tasarımını yaparken bu sınıfa ait nesne örneklerinin ne amaçla kullanılacağını belirlemeliyiz. FiyatException sınıfına ait bir nesne örneği, herhangi bir Kitap nesnesinin fiyatının bazı kriterlere uymaması durumunda fırlatılacaktır.

![dikkat.gif](/assets/images/2005/dikkat.gif).Net içinde önceden tanımlanmış olan istisna sınıfları Exception kelimesi ile biterler. Kendi istisna sınıflarımızın isimlerinin de aynı şekilde yazılması kod standardizasyonu açısından önemlidir. Örneğin FiyatException gibi...

Dolayısıyla kritere uymayacak Fiyat değerini taşıması önemlidir. Ayrıca kriterin çeşitliliğine göre kullanıcıya verilmesi istenen mesaj içeriğide bir field olarak saklanmalıdır. Bu iki field'ın alacağı değerleri ise constructor metodumuz içerisinde sağlayabiliriz. Oluşturduğumuz istisna nesnesinin tipik bir Exception nesnesi gibi davranabilmesini sağlamak amacıyla örnek olarak Message özelliği override edilmiştir. FiyatException sınıfı artık uygulama içerisinde yakalanabilecek tipik bir istisna halini almıştır. Ancak henüz uygulanmamıştır. Herşeyden önce istisnanın fırlatılmasını istediğimiz yerlerde bunları kodlamamız gerekecektir. Kitap nesnemizi göz önüne aldığımızda Fiyat özelliğinin değerinin verildiği set bloğu bu iş için biçilmiş kaftandır. Buradaki kodları aşağıdaki gibi düzenleyebiliriz.

```csharp
public double Fiyat
{
    get
    {
        return m_Kitap_Fiyat;
    }
    set
    {
        if(value>150)
            throw new FiyatException(value,"Fiyat 150 YTL' den yüksek olamaz");
        if(value<0)
            throw new FiyatException(value,"Fiyat negatif değer olamaz");
        m_Kitap_Fiyat=value;
    }
}
```

Fiyat özelliğine değer atanırken eğer girilen değer 0' dan küçük ise buna uygun mesaja sahip bir istisna nesnesi, Fiyat 150 YTL'den büyük ise buna uygun mesaja sahip bir istisna nesnesi ortama fırlatılmaktadır. Böylece catch bloğunda FiyatException nesne örneklerini yakalayabiliriz. Kitap sınıfının yapıcı metodunda parametre üzerinden, sınıf içindeki alanlara değer ataması yapılmaktadır. Ancak burada da parametre değerlerini direkt olarak özelliklere atadığımızda set blokları devreye girmektedir. Bu, Fiyat özelliğinin değeri için gerekli istisna kontrolünü yapıcı metod içerisinde de gerçekleştirebilmemizi sağlar. Kısacası bir taşla iki kuş vurmuş oluruz. Artık windows uygulamamızdaki kodları aşağıdaki gibi düzenleyebiliriz.

```csharp
private void btnOlustur_Click(object sender, System.EventArgs e)
{
    try
    {
        kitap=new Kitap();
        kitap.Baslik=txtBaslik.Text;
        kitap.Yazar=txtYazar.Text;
        kitap.Basim=dtpBasim.Value;
        kitap.Fiyat=Convert.ToDouble(txtFiyat.Text);
        kitap.Kategori=cmbKategori.SelectedText; 
    }
    catch(FiyatException err)
    {
        MessageBox.Show(err.Message,"Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
    }
    catch(System.Exception err)
    {
        MessageBox.Show(err.Message,"Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
    }
}
```

Örneğin Kitap sınıfına ait nesne örneğini oluştururken Fiyat alanının değerini 175 olarak girelim.

![mk122_3.gif](/assets/images/2005/mk122_3.gif)

Görüldüğü gibi oluşturduğumuz istisna sınıfına ait nesne örneği, Kitap nesnesi oluşturulmaya çalışıldığında ortama fırlatılmıştır. Bir de Fiyat değerini 10' ar birim azalttığımızda eksi değere geçtiğimiz bir metodumuz vardı. Kitap nesnesinin istisnasız oluşturup fiyatını negatif değere çektiğimizde uygulamanın FiyatException istisnası nedeni ile kesilerek sonlandırıldığını görürüz.

![mk122_4.gif](/assets/images/2005/mk122_4.gif)

Uygulamanın istisnayı yakalayamamasının sebebi fiyat azaltımı yapan metodumuzun ilgili istisnayı yakalayacak bir try...catch yapısını kullanmayışıdır. İster kendi istisna nesnelerimiz olsun ister sistemde var olan istisna nesneleri olsun, bunların yakalanarak uygulamanın sonlandırılmadan yaşamaya devam edebilmesi için uygun catch blokları ile yakalanmaları şarttır. Dolayısıyla fiyat arttırma ve azaltma metodlarımızı aşağıdaki gibi yenilememiz gerekmektedir.

```csharp
private void btnIndirim_Click(object sender, System.EventArgs e)
{
    try
    {
        if(kitap!=null)
        {
            kitap.Fiyat-=10;
            txtFiyat.Text=kitap.Fiyat.ToString();
        }
    }
    catch(FiyatException err)
    {
        MessageBox.Show(err.Message,"Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
    }
}

private void btnArttir_Click(object sender, System.EventArgs e)
{
    try
    {
        if(kitap!=null)
        {
            kitap.Fiyat+=10;
            txtFiyat.Text=kitap.Fiyat.ToString();
        }
    }
    catch(FiyatException err)
    {
        MessageBox.Show(err.Message,"Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
    } 
}
```

![mk122_5.gif](/assets/images/2005/mk122_5.gif)

Artık Fiyat alanı için eksi değer oluşmamaktadır. Burada dikkat etmemiz gereken bir diğer noktada catch bloklarında kendi tanımladığımız istisna sınıflarını yakalayabilmek için ilede o istisna sınıfına ait nesne örneğini belirtme zorunluluğumuzun olmayışıdır. Yani

```csharp
catch(FiyatException err)
{
    MessageBox.Show(err.Message,"Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
}
```

yerine,

```csharp
catch(Exception err)
{    MessageBox.Show(err.Message,"Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
}
```

formunuda kullanabiliriz. Yine aynı istisna mesajlarını yakalayacağızdır.

Görüldüğü gibi, uygulamalarımızda düşündüğümüz istisnai durumları yakalayabilmemizi sağlayacak sınıfları tasarlamak son derece kolaydır. Önemli olan nokta, istisna nesnemizin gerçekten gerekli olup olmadığıdır. Eğer böyle bir gereklilik var ise ve kendi istisna sınıflarımızı oluşturduysak bunlara ait nesne örneklerini uygun yerlerde ortama fırlatmalıyız. Son olarak fırlattığımız istisna nesnelerimizi catch blokları ile yakalayarak uygulamaya yön vermeliyiz. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın.](/assets/files/2005/Kitap.rar)