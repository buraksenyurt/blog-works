---
title: "Daha Etkili Profil(Profile) Yönetimi"
pubDate: 2007-10-17 12:00:00
categories:
  - Asp.Net 2.0
tags:
  - Asp.Net 2.0
  - C#
  - XML
  - JavaScript
  - .NET
  - ASP.NET
  - HTTP
  - Authentication
  - Authorization
  - Caching
  - Serialization
---

# Daha Etkili Profil(Profile) Yönetimi
Değerli Okurlarım Merhabalar,

Uzun süre önce Asp.Net 2.0 ile geliştirilen web uygulamalarında Profile API'sinin nasıl kullanıldığını kısa bir [makale](http://www.bsenyurt.com/MakaleGoster.aspx?ID=160) üzerinden incelemeye çalışmıştık. Geçtiğimiz günlerde Asp.Net 2.0 ile ilgili bilgilerimi tazelerken profil yönetiminin daha etkin bir şekilde nasıl kullanılabileceğine dair pek çok örnek ile karşılaştım. İşte bu makalemizde temel olarak profil yönetiminin daha etkin hale getirilmeye çalışması için uğraşıyor olacağız. İnceleyeceğimiz temel konu başlıklarını aşağıdaki gibi sıralayabiliriz.

- ProfileBase tipinden türetmek (Inherit).
- Profil bilgilerini kod üzerinden yönetebilmek (ProfileManager).
- İsimsiz (Anonymous) kullanıcılar için profil bilgilerini kullanabilmek.

Başlamadan önce profil kavramını kısaca tanımlamakta yarar olduğu kanısındayım. Bir web uygulamasına bağlanan kullanıcıların her biri için ortak tanımlanıp değerleri farklı olabilecek özellikler topluluğu profil bilgisini oluşturmaktadır. Bu anlamda özellikle, bir doğrulama (authentication) ve yetkilendirme (authorization) sistemine sahip olan web uygulamalarında her kullanıcı için değerleri farklı olabilecek özelliklerin tutulması ve kullanılması mümkün olabilmektedir. Bu tip bir sistemin özellikle Asp.Net 1.1 ile geliştirilmesi ekstra kodlamayı gerektirirken Asp.Net 2.0 üzerinde yer alan Profile API sayesinde son derece kolaylaşmıştır. Gelelim Profile API yeteneklerini daha etkili bir şekilde nasıl ele alabileceğimize.

ProfileBase Tipinden Türetmek (Inherit);

Normal şartlarda bir web uygulaması içerisinde profil bilgilerini kullanabilmek için web.config dosyası içerisinde profile elementinin ele alınması gerekmektedir. Nitekim bir web uygulamasında kullanılan profil bilgilerinin, başka web uygulamasında (web uygulamalarında) ele alınmasının istendiği vakkalarda mevcuttur. Bu tip bir durumda çözüm olarak, ProfileBase tipinden türetme yapılaraktan birden fazla web uygulamasında ele alınabilecek bir profil sınıfı geliştirmek mümkündür. ProfileBase sınıfının temel üyeleri aşağıdaki sınıf diagramında (Class Diagram) görüldüğü gibidir.

![mk227_1.gif](images/mk227_1.gif)

PorfileBase tipi sınıf diagramından (class diagram) da görüldüğü gibi SettingBase isimli abstract sınıftan (class) türemeketedir. ProfileBase tipine ait üyelerden bazılarının görevleri aşağıdaki tabloda belirtildiği gibidir.

Metodlar (Methods)
Açıklama

Create
Bu metod ile bir kullanıcı için profil nesne örneği oluşturulur. Özel profil tiplerinin yazılmasında veya Asp.Net ortamı dışındaki çevrelerde profil yönetimi söz konusu olduğunda ele alınmaktadır. Metod geriye ProfileBase tipinin taşıyabileceği referansları döndürür. İki farklı versiyonu vardır. Her iki versiyonda ilk parametre olarak kullanıcı adını alır. İkinci parametre bool bir değerdir ve kullanıcının isimsiz (anonymous) yada doğrulanmış (authenticated) olup olmadığını belirtir. Metodun dönüş değerinin true olması kullanıcının doğrulandığı anlamına gelmektedir.

Save
Profil bilgilerini kaydetmek amacıyla kullanılır. Herhangibir parametre almaz ve geriye değer döndürmez (void). Var olan bir ProfileBase nesne örneği üzerinden çağrılabildiği için ilgili tipe ait özelliklerde yapılan değişikliklerin kaydedilmesini sağlar. Kaydetme işlemi veri kaynağına (data source) doğru yapılmaktadır. Bu işlem sırasında IsDirty özelliği true değerini alır. İşlem tamamlandıktan sonra ise false değerini alır.

GetPropertyValue
Parametre olarak verilen özelliğin değerini object tipinden döndürür.

SetPropertyValue
İki parametre alan bu metodun ilk parametresi değeri verilecek özellik adını, ikinci parametresi ise object tipinden ilgili değeri almaktadır. Bu metod yardımıyla profil içerisindeki bir özelliğe değer atanabilmesi sağlanabilmektedir.

GetProfileGroup
Profil içerisinde yer alan özellikler istenirse grup halinde ayrılabilirler. Bunun için profile elementi içerisinde yer alan properties elementlerinde, group alt elementi kullanılmaktadır. Böyle bir durumda gruplanan özelliklerin listesini elde etmek için GetProfileGroup metodu kullanılabilir. Bu metod geriye ProfileGroupBase tipinden bir referans döndürmektedir. Bu referansın üzerinden hareket ederek grup içerisindeki özelliklere ve değerlerine erişmek mümkün olmaktadır. Aşağıdaki sınıf diagramı görüntüsünde ProfileGruopBase sınıfının üyeleri görülmektedir.
![mk227_2.gif](images/mk227_2.gif)

Özellikler (Properties)
Açıklama

Item
Profil içerisinde tanımlanmış olan özellik adlarını parametre olarak alabilen indeksleyici sayesinde ilgili özelliğin değerinin verilmesi (set) veya elde edilmesi (get) mümkündür. Özellik adı string tipinden verilmekte olup indeksleyicinin dönüş değeri object tipindendir.

Properties
Static olarak tanımlanmış olan bu özellik sayesinde profil özelliklerinin bir listesinin SettingsPropertyCollection koleksiyon tipinden elde edilmesi mümkündür. Bu koleksiyonun her bir elemanı SettingsProperty sınıfı tipindendir. Bu tipin üyeleri ise aşağıdaki sınıf diagramında görüldüğü gibidir. Dikkat edilecek olursa bu üyelerden yola çıkarak profilin özelliği hakkında detaylı bilgilere ulaşmak veya yönetmek mümkündür.
![mk227_3.gif](images/mk227_3.gif)

UserName
Profilin sahibi olan kullanıcı adını verir. Eğer isimsiz bir kullanıcı girişi söz konusu ise identifier değerini döndürecektir.

IsDirty
Profil özelliklerinden herhangibiri değiştiriliyorken true değerini döndürür. Aksi durumda false değerini döndürmektedir.

IsAnonymous
Eğer kullanıcı isimsiz (anonymous) ise true değerini döndürür. Aksi durumda false'dur.

Şimdi örnek bir senaryo üzerinden hareket ederek konuyu biraz daha iyi kavramaya çalışalım. Öncelikli olarak hedefimiz birden fazla web uygulamasının kullanabileceği bir Profile tipi geliştirmek olduğundan bir sınıf kütüphanesi (class library) geliştirerek işe başlanabilir. Çok doğal olarak bu sınıf kütüphanesi içerisinde ProfileBase tipi kullanılacağından ve yeri geldiğinde güncel HTTP içeriğine (HttpContext) erişilmesi gerektiğinden System.Web.dll assembly'ının ilgili sınıf kütüphanesine referans edilmesi gerekmektedir.

Özel olarak hazırlanacak sınıfın sağlaması gereken bazı özellikler vardır. İlk olarak bu sınıfın en azından XML serileştirilebilir (XML Serializable) olması gerekmektedir. Çok doğal olarak bu sınıf içerisinde kullanılacak özelliklerin veri tipleride (data types) serileştirilebilir olmalıdır. (Kendi tiplerimizden özellik türleri yazmadığımızda çoğunluklu ilkel tipleri (primitive types) kullanırız. Bu tiplerin çoğu zaten serileştirilebilir olduğundan sorun çıkma olasılığı azalmaktadır. Ancak kendi tiplerimizi ele aldığımızda serileştirilebilir olmalarına dikkat etmek gerekmektedir.) İkinci olarak özel tip içerisinde, web uygulamalarındaki kullanıcılar için gerekli profil bilgisini oluşturacak özellikler (property) ayrı ayrı tanımlanmalıdır. Bu özelliklerin kullanımı sırasında base anahtar kelimesi ile üst sınıfa aktarma yapılmasına dikkat edilmelidir. Üçünü olarak elbetteki özel sınıfın ProfileBase tipinden türetilmiş olması gerekmektedir. Bu türetmenin doğal sonucu olarak ProfileBase sınıfı içerisinde tanımlanmış bazı üyelerin ezilebileceği (override) ortadadır.

> Hatırlanacağı üzere üst sınıfta (base class) sanal (virtual) olarak tanımlanış olan üyelerin, türeyen sınıflarda (derived class) ezilme (override) zorunluluğu yoktur. Eğer bir zorunluluk getirilmesi isteniyorsa abstract üyelerin yer alabildiği abstract sınıflar veya arayüzler (interface) kullanılmalıdır.

Aşağıdaki şekilde türeyen sınıf (derived class) içerisinde ezilebilecek üyeler gösterilmektedir. Doğal olarak herkes bir Object olduğundan, object sınıfından gelen bazı virtual üyelerde bu listede yer almaktadır.

![mk227_4.gif](images/mk227_4.gif)

Örnek olarak MyProfile sınıfı aşağıdaki gibi tasarlanabilir. Sınıf içerisinde kontak bilgilerini saklamak amacıyla Contact isimli bir sınıf daha kullanılmaktadır. Bu sınıfa ait nesne örneklerini kullanan özellikler MyProfile sınıfı içerisinde ele alınarak, kullanıcı tanımlı tiplerin durumuda rahatlıkla incelenebilir.

Contact Sınıfı;

![mk227_8.gif](images/mk227_8.gif)

```csharp
public class Contact
{
    private string _ad;
    private string _soyad;
    private string _email;

    public string Email
    {
        get { return _email; }
        set { _email = value; }
    }

    public string Soyad
    {
        get { return _soyad; }
        set { _soyad = value; }
    }

    public string Ad
    {
        get { return _ad; }
        set { _ad = value; }
    }
    public Contact(string ad, string soyad, string mail)
    {
        Ad = ad;
        Soyad = soyad;
        Email = mail;
    }
    // XML Serileştirme kuralı olarak parametresiz bir yapıcı metod(constructor) olması gerekmektedir.
    public Contact()
    {
    }
}
```

Burada dikkat edilmesi gereken noktalardan biriside Contact sınıfının varsayılan yapıcı metodunun (Default Constructor) yazılmış olmasıdır. Profil bilgilerinde kendi tiplerimizi kullandığımız durumlarda varsayılan olarak XML serileştirme gerçekleştirilmektedir. Dolayısıyla yine varsayılan olarak AspNetDb veritabanındaki aspnetProfile tablosuna yazılacak olan özellik değerlerinin XML formatında serileştirilebilir olması gerekmektedir. Bu sebepten ilgili tipin varsayılan yapıcı metoda sahip olması gerekir ki bu XML serileştirmenin kurallarından birisidir. Eğer varsayılan yapıcı metodu yazmassak çalışma zamanında profil bilgisini kaydederken aşağıdaki ekran görüntüsünde yer alan istisnayı (exception) alırız.

![mk227_6.gif](images/mk227_6.gif)

MyProfile Sınıfı;

![mk227_9.gif](images/mk227_9.gif)

```csharp
public class MyProfile:ProfileBase
{
    public int SonStokDurumu
    {
        get { return Convert.ToInt32(base["SonStokDurumu"]); }
        set { base["SonStokDurumu"] = value; }
    }    
    public DateTime SonGirisZamani
    {
        get { return Convert.ToDateTime(base["SonGirisZamani"]); }
        set { base["SonGirisZamani"] = value; }
    }
    public Contact Contact2
    {
        get { return (Contact)base["Contact2"]; }
        set { base["Contact2"] = value; }
    }
    public Contact Contact1
    {
        get { return (Contact)base["Contact1"]; }
        set { base["Contact1"] = value; }
    }
    public string Bayi
    {
        get { return base["Bayi"].ToString(); }
        set { base["Bayi"] = value; }
    }
}
```

Dikkat edilecek olursa sınıf içerisinde tasarlanan özelliklere ait get ve set bloklarında base anahtar kelimesi ile ProfileBase sınıfına çıkılmakta ve indeksleyici operatöründen yararlanılarak ilgili alanlara erişilmesi sağlanmaktadır. Elbetteki base ile ulaşılan referansın özellikleri object veri türü ile çalıştığından set blokları içerisinde uygun tür dönüşümlerinin bilinçli (explicit) olarak yapılması şarttır. MyProfile sınıfı içerisinde tanımlanacak özel alanlara (private fields) değer atanması durumunda veritabanına herhangibir şekilde bilgi yazılmayacaktır. Böyle bir işlem için Save metodunun bu sınıf içerisinde ezilemesi ve kodlanması gerekmektedir.

Şimdi MyProfile sınıfını test etmek amacıyla basit bir web uygulaması tasarlayalım. Bu web uygulamasında standart olarak AspNetDb veritabanı kullanılabilir. Form tabanlı doğrulama (Form Based Authentication) sisteminin yer aldığı web uygulamasında standart olarak Login.aspx sayfası kullanıcı girişi için kullanılırken, Default.aspx sayfası içerisinde örnek test kodları yer almaktadır. Web uygulamasına ait konfigurasyon dosyasının (Web.config) içeriği aşağıdaki gibi olmalıdır.

web.config;

```xml
<?xml version="1.0"?>
<configuration>
    <appSettings/>
    <connectionStrings/>
    <system.web>
        <compilation debug="true" />
        <authentication mode="Forms" />
        <authorization>
            <deny users="?"/>
        </authorization>
        <profile enabled="true" inherits="CustomProfileLibrary.MyProfile"/>
    </system.web>
</configuration>
```

profile elementi içerisinde inherits niteliğine (attribute) atanan değer ile Profile tipinin ne olduğu söylenir. Burada İsimAlanıAdı.TipAdı (NamespaceName.TypeName) notasyonu kullanılarak profile bilgilerinin yönetiminin MyProfile sınıfı tarafından yapılacağı belirtilmektedir. Form tabanlı doğrulama (Form Based Authentication) kullanıldığından authentication elementinin mode niteliğine Forms değeri atanmıştır. İsimsiz kullanıcıların (anonymous users) siteye giriş yapması istenmediğinden authorization elementi içerisinden söz konusu kullanıcılar? karakteri ile deny edilmiştir. Gelelim uygulamanın örnek web sayfasına. Sayfanın tasarımı aşağıda görüldüğü gibidir.

Default.aspx sayfası;

![mk227_10.gif](images/mk227_10.gif)

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Untitled Page</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <asp:LoginName ID="LoginName1" runat="server" /><asp:LoginStatus ID="LoginStatus1" runat="server" />
                <br />
                <table>
                    <tr>
                        <td colspan="2" valign="top">Profil Özellikleri <asp:Button ID="btnGetir" runat="server" OnClick="btnGetir_Click" Text="Bilgileri Getir" /></td>
                    </tr>
                    <tr>
                        <td style="width: 164px" valign="top">Bayi</td>
                        <td style="width: 100px"><asp:TextBox ID="txtBayi" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>    
                        <td style="width: 164px" valign="top">Kontak 1</td>
                        <td style="width: 100px">
                        <table>
                            <tr>
                                <td style="width: 100px">Ad</td>
                                <td style="width: 100px"><asp:TextBox ID="txtKontak1Ad" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td style="width: 100px">Soyad</td>
                                <td style="width: 100px"><asp:TextBox ID="txtKontak1Soyad" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td style="width: 100px">Email</td>
                                <td style="width: 100px"><asp:TextBox ID="txtKontak1Email" runat="server"></asp:TextBox></td>
                            </tr>
                        </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 164px" valign="top">Kontak 2</td>
                        <td style="width: 100px">
                        <table>
                            <tr>
                               <td style="width: 100px">Ad</td>
                                <td style="width: 100px"><asp:TextBox ID="txtKontak2Ad" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td style="width: 100px">Soyad</td>
                                <td style="width: 100px"><asp:TextBox ID="txtKontak2Soyad" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td style="width: 100px">Email</td>
                                <td style="width: 100px"><asp:TextBox ID="txtKontak2Email" runat="server"></asp:TextBox></td>
                            </tr>
                        </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 164px; height: 10px" valign="top">Son Giriş Zamanı</td>
                        <td style="width: 100px; height: 10px"><asp:Label ID="lblSonGirisZamani" runat="server" Text="Label"></asp:Label></td>
                    </tr>
                    <tr>
                        <td style="width: 164px; height: 26px" valign="top">Stok Durumu</td>
                        <td style="width: 100px; height: 26px"><asp:TextBox ID="txtStokDurumu" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2" valign="top"><asp:Button ID="btnKaydet" runat="server" OnClick="btnKaydet_Click" Text="Bilgileri Kaydet" /></td>
                    </tr>
                </table>
                <br />
                <br />
            </div>
        </form>
    </body>
</html>
```

Default.aspx.cs;

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CustomProfileLibrary;

public partial class _Default : System.Web.UI.Page 
{
    protected void btnGetir_Click(object sender, EventArgs e)
    {
        Getir();
    }

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        Kaydet();
    }

    private void Getir()
    {
        try
        {
            txtBayi.Text = Profile.Bayi;
            txtKontak1Ad.Text = Profile.Contact1.Ad;
            txtKontak1Email.Text = Profile.Contact1.Email;
            txtKontak1Soyad.Text = Profile.Contact1.Soyad;
            txtKontak2Ad.Text = Profile.Contact2.Ad;
            txtKontak2Email.Text = Profile.Contact2.Email;
            txtKontak2Soyad.Text = Profile.Contact2.Soyad;
            lblSonGirisZamani.Text = Profile.SonGirisZamani.ToString();
            txtStokDurumu.Text = Profile.SonStokDurumu.ToString();
        }
        catch (Exception excp)
        {
            Response.Write(excp.Message);
        }
    } 

    private void Kaydet()
    {
        Profile.Bayi = txtBayi.Text;
        Contact kontak1 = new Contact(txtKontak1Ad.Text, txtKontak1Soyad.Text, txtKontak1Email.Text);
        Profile.Contact1 = kontak1;
        Contact kontak2 = new Contact(txtKontak2Ad.Text, txtKontak2Soyad.Text, txtKontak2Email.Text);
        Profile.Contact2 = kontak2;
        Profile.SonGirisZamani = DateTime.Now;
        int stok = 1;
        Int32.TryParse(txtStokDurumu.Text, out stok);
        Profile.SonStokDurumu = stok;
        Profile.Save();
    }
}
```

Sayfa basit olarak profil bilgilerinin getirilmesi veya kaydedilmesi için gereken kodları içermektedir. Dikkat edilirse Profile tipi kullanılmaktadır. Nitekim web.config dosyasındaki bildirim nedeni ile Profile özelliği üzerinden MyProfile içerisinde tanımlanmış olan özelliklere erişilebilmektedir. Aşağıdaki şekilde bu durum daha net bir şekilde görülebilir.

![mk227_5.gif](images/mk227_5.gif)

Çok doğal olarak daha önceden bir profil bilgisi oluşturulmaması durumuna karşılık bilgiler getirilirken Null Reference Exception alınma olasılığı vardır. Bu nedenle profil bilgileri ortama çekilirken bir try...catch bloğu kullanılması yararlı olabilir. Sayfa üzerinde test bilgilerinin kaydedilmesi sonrasında aspnetprofile tablosunun içeriğine bakılırsa profil bilgisinin başarılı bir şekilde kaydedildiği görülebilir. Dikkat edileceği üzere bilgiler XML serileştirmeye uygun olacak şekilde XML formatında PropertyValueString alanına yazılmıştır.

![mk227_7.gif](images/mk227_7.gif)

Çalışma zamanında (run-time) Bilgileri Getir başlıklı düğmeye basıldığında ise söz konusu verilerin ilgili kontrollere doldurulduğu görülecektir. Aşağıda bu duruma ait örnek bir ekran çıktısı yer almaktadır.

![mk227_11.gif](images/mk227_11.gif)

Profil Bilgilerini Kod Üzerinden Yönetebilmek;

Bazı durumlarda çalışma zamanında (run-time) uygulamada kullanılan profil bilgilerinin yönetilmesi istenebilir. Söz gelimi profilde kayıtlı bilgilerin gösterilmesi, belirli bir tarihten öncekilerin kaldırılması, bir kullanıcının profil bilgisinin silinmesi, aktif olmayan profillerin elde edilmesi vb... işlemler yapılabilir. Bu aslında basit olarak veritabanına ulaşmak ve ilgili tablonun alanlarına bakmaktan başka bir şey değildir. Ne varki Asp.Net Profile API içerisinde söz konusu yönetsel işlemlerin daha kolay yapılmasını sağlayan ProfileManager sınıfı mevcuttur. Bu sınıfın diagram görüntüsü aşağıdaki gibidir.

![mk227_12.gif](images/mk227_12.gif)

Dikkat edileceği üzere ProfileManager, static bir sınıftır (static class).

> Static sınıf kavramı C# 2.0 ile birlikte gelmiştir. Static sınıflar sadece static üyeler (static members) içerebilir, türetme (Inheritance) amacıyla kullanılamaz veya örneklenemezler. Normal sınıflara göre daha hızlı ve performanslı çalıştıkları ortadadır. Bununla birlikte C# 3.0 ile birlikte gelen extension methods kavramında önemli bir yerede sahiptir.

Sınıfın önemli metodları ve yaptığı işler ise aşağıdaki tabloda görüldüğü gibidir.

Metodlar (Methods)
Açıklama

GetAllProfiles
İki versiyonu olan bu metod sayesinde bir uygulamadaki profil bilgilerinin tamamı ProfileInfoCollection tipi içerisinde olacak şekilde elde edilebilir. Her iki versiyonda ProfileAuthenticationOption enum sabiti tipinden bir parametre almaktadır. Bu parametrenin alabileceği değerler All, Anonymous, Authneticated'dır. Bir başka deyişle tüm kullanıcıların, sadece isimsiz kullanıcıların veya sadece doğrulanmış kullanıcıların profile bilgilerinin elde edilmesi sağlanabilir. Ayrıca bu metod ile sayfalamalara uygun olacak şekilde veri çekilmeside sağlanabilmektedir.

GetNumberOfProfiles
Veritabanında kayıtlı olan profil sayısının elde edilmesini sağlayan bu metod yine ProfileAuthenticationOption enum sabiti tipinden bir parametre alır. Buna göre sadece isimsiz kullanıcıların (anonymous users), doğrulanmış kullanıcıların (authenticated users) yada tüm kullanıcıların (all users) kayıtlı olan profil sayıları elde edilebilir.

GetAllInactiveProfiles
Profil sahibi kullanıcıların LastActivityDate özelliklerinin değerlerine göre parametre olarak verilen tarih ve öncesindeki tüm profillerin elde edilmesini sağlayan metoddur. İki versiyonu vardır ve her ikisinin ilk parametresi aynıdır. İlk parametrede ProfileAuthenticationOption değeri ikincisinde ise tarih bilgisi belirlenir.

GetNumberOfInactiveProfiles
Profil sahibi kullanıcıların LastActivityDate özelliklerinin değerlerine göre parametre olarak verilen tarih ve öncesindeki tüm profillerin sayısının integer olarak elde edilmesini sağlayan metoddur.

FindProfilesByUserName
Belirlenen kullanıcı adıyla eşleşen profil bilgilerinin ProfileInfoCollection tipinden döndürülmesini sağlamaktadır. İki farklı versiyonu vardır. Her ikiside ilk iki parametresinde sırasıyla ProfileAuthenticationOption ve kullanıcı adı bilgilerini almaktadır. Sayfalama yapılmasıda sağlanabilmektedir. Burada kullanıcı adı girilirken % karakteri kullanılarak Like benzeri bir sorgulama yapılabilmektedir.

FindInactiveProfilesByUserName
Profil sahibi kullanıcıların LastActivityDate özelliklerinin değerlerine göre belirtilen tarih ve öncesinde olanların parametre olarak verilen kullanıcı adı ile eşleşenlerinin bulunmasını sağlayan metoddur. Burada kullanıcı adı girilirken % karakteri kullanılarak Like benzeri bir sorgulama yapılabilmektedir. Örneğin kullanıcı adı içerisinde ma geçenlerin elde edilip belirtilen bir tarhiten öncesine kadar aktivitesi olmayanlar listelenebilir.

DeleteProfile
Parametre olarak verilen kullanıcıya ait profile bilgisinin silinmesini sağlamaktadır. Bu metod silme işlemi başarılı ise true değerini döndürür.

DeleteProfiles
Bu metodun iki farklı versiyonu vardır. Bunlardan birisi ProfileInfoCollection tipinden diğeri ise string dizisi tipinden parametre almaktadır. Dolayısıyla profile bilgilerinin silinmesi istenen kullanıcı tipleri iki farklı şekilde yüklenebilir.

DeleteInactiveProfiles
Profil sahibi kullanıcıların LastActivityDate özelliklerinin değerlerine göre belirtilen tarih ve öncesinde olanların silinmesini sağlayan metod ProfileAuthenticationOption enum sabiti tipinden değerde almaktadır.

Şimdi bu sınıfın kullanıldığı örnek bir web sayfasını projeye dahil edelim. Sayfanın tasarım zamanındaki (design-time) görüntüsü ve kodları aşağıdaki gibidir.

![mk227_15.gif](images/mk227_15.gif)

ProfilYoneticisi.aspx;

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProfilYoneticisi.aspx.cs" Inherits="ProfilYoneticisi" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Profil Yönetim Ekranı</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                Doğrulama Tipi :<asp:DropDownList ID="ddlDogrulamaTipi" runat="server"></asp:DropDownList>
                <asp:Button ID="btnTumProfiller" runat="server" OnClick="btnTumProfiller_Click" Text="Tüm Profilleri Getir" /><br />
                Kullanıcı Adı(Benzeri) :
                <asp:TextBox ID="txtKullaniciAdi" runat="server"></asp:TextBox>
                <asp:Button ID="btnIsmeGoreGetir" runat="server" OnClick="btnIsmeGoreGetir_Click" Text="İsme Göre Getir" /><br />
                Aktivite Tarihi :
                <br />
                <asp:Calendar ID="dtTarih" runat="server"></asp:Calendar>
                <br />
                <asp:Button ID="btnPasifleriGetir" runat="server" OnClick="btnPasifleriGetir_Click" Text="Pasifleri Getir" /><br />
                Bulunan :
                <asp:Label ID="lblKullaniciSayisi" runat="server" Text="Label"></asp:Label><br />
                <br />
                <asp:GridView ID="grdTumProfiller" runat="server" OnRowCommand="grdTumProfiller_RowCommand" OnRowDeleting="grdTumProfiller_RowDeleting">
                    <Columns> 
                        <asp:CommandField ButtonType="Button" DeleteText="Sil" ShowCancelButton="False" ShowDeleteButton="True" />
                    </Columns>
                </asp:GridView>
            </div>
        </form>
    </body>
</html>
```

ProfilYoneticisi.aspx.cs;

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Profile;

public partial class ProfilYoneticisi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // ProfileAuthenticationOption enum sabitinin içeriğini Enum sınıfın GetNames metodu ile alarak DropDownList kontrolüne dolduruyoruz.
            ddlDogrulamaTipi.DataSource = Enum.GetNames(typeof(ProfileAuthenticationOption));
            ddlDogrulamaTipi.DataBind();
        }
    }

    // Bu metod var olan tüm profil bilgilerini getirmek üzere tasarlanmıştır
    private void TumProfilleriGetir()
    {
        // DropDownList kontrolünden seçilen doğrulama kriterinin enum sabitindeki karşılığı seçiliyor.
        ProfileAuthenticationOption dogrulamaKriteri=(ProfileAuthenticationOption)Enum.Parse(typeof(ProfileAuthenticationOption), ddlDogrulamaTipi.SelectedValue);
        // GetAllProfiles metodu ile doğrulama kriterine uygun olan profil bilgileri ProfileInfoCollection tipinden bir koleksiyona aktarılır.
        ProfileInfoCollection tumProfiller = ProfileManager.GetAllProfiles(dogrulamaKriteri);
        grdTumProfiller.DataSource = tumProfiller;
        grdTumProfiller.DataBind();
        // Söz konusu doğrulama kriterine uyan profillerin sayısı elde edilir.
        lblKullaniciSayisi.Text = ProfileManager.GetNumberOfProfiles(dogrulamaKriteri).ToString();
    }

    // GridView kontrolünde Delete işlemi gerçekleştirildiğinde çalışan olay metodudur.
    protected void grdTumProfiller_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
            int rowIndex=Convert.ToInt32(e.CommandArgument);
            string userName=grdTumProfiller.Rows[rowIndex].Cells[1].Text;
            // DeleteProfile metodu ile seçilen satırdaki UserName bilgisine ait Profil silinir.
            ProfileManager.DeleteProfile(userName);
            TumProfilleriGetir();
        }
    }
    protected void btnTumProfiller_Click(object sender, EventArgs e)
    {
        TumProfilleriGetir(); 
    }

    // TextBox içerisine girilen kullanıcı adına benzer olanların Profil bilgilerini getirir.
    protected void btnIsmeGoreGetir_Click(object sender, EventArgs e)
    {
        ProfileAuthenticationOption dogrulamaKriteri = (ProfileAuthenticationOption)Enum.Parse(typeof(ProfileAuthenticationOption), ddlDogrulamaTipi.SelectedValue);
        // % kullanılması sayesinde içerisinde TextBox' taki bilgi geçen kullanıcı adlarını elde ederiz.
        ProfileInfoCollection profiller = ProfileManager.FindProfilesByUserName(dogrulamaKriteri, "%" + txtKullaniciAdi.Text + "%");
        grdTumProfiller.DataSource = profiller;
        grdTumProfiller.DataBind();
        lblKullaniciSayisi.Text = profiller.Count.ToString(); 
    }
    protected void grdTumProfiller_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        
    }

    /* aspnet_Users tablosunda LastActivityDate değeri, seçilen tarihten daha önce olanların elde edilmesini sağlar. Bunun için GetAllInactiveProfiles metodunun ikinci parametresi kullanılır. */
    protected void btnPasifleriGetir_Click(object sender, EventArgs e)
    {
        ProfileAuthenticationOption dogrulamaKriteri = (ProfileAuthenticationOption)Enum.Parse(typeof(ProfileAuthenticationOption), ddlDogrulamaTipi.SelectedValue);
        grdTumProfiller.DataSource=ProfileManager.GetAllInactiveProfiles(dogrulamaKriteri, dtTarih.SelectedDate);
        grdTumProfiller.DataBind();
        lblKullaniciSayisi.Text=ProfileManager.GetNumberOfInactiveProfiles(dogrulamaKriteri, dtTarih.SelectedDate).ToString();
    }
}
```

Söz konusu örnekte profil yönetimi adına basit işlemler yapılmaktadır. Örnek olması açısında var olan tüm profillerin elde edilmesi, seçilen profillerden herhangibirinin silinmesi, içerisinde belirtilen ada benzer olan profillerin çekilmesi veya belirli bir tarihten öncesine kadar aktivitesi olmayan profillerin getirilmesi gibi işlemler yapılmaktadır. Örneğin tüm profillerin listelenmesi istendiğinde aşağıdakine benzer bir ekran görüntüsü ile karşılaşılır.

![mk227_16.gif](images/mk227_16.gif)

Kullanıcı adı benzer olanların listelenmesine örnek olarak aşağıdaki ekran görüntüsünde olduğu gibi içerisinde ma kelimesi geçenler listelenebilir.

![mk227_17.gif](images/mk227_17.gif)

Belirli bir süreden öncesine kadar pasif olan kullanıcıların profil listesini elde etmek istediğimizde ise aspnetUser tablosunda yer alan LastActivityDate değeri baz alınır. Bir başka deyişle aktivite durumuna göre hareket eden ProfileManager sınıfı metodları tarih parametresinin değerlerini, söz konusu tablodaki alan ile kıyaslayarak çalışmaktadır. Bu tabloya ait örnek bir ekran görüntüsü aşağıda görüldüğü gibidir.

![mk227_13.gif](images/mk227_13.gif)

Çalışma zamanında örneğin 16.09.2007 tarihi ve öncesindeki profil bilgileri elde edilmek istendiğinde aşağıdakine benzer bir ekran görüntüsü ile karşılaşırız.

![mk227_14.gif](images/mk227_14.gif)

İsimsiz (Anonymous) kullanıcılar için profil bilgilerini kullanabilmek;

Bazı durumlarda siteyi ziyaret eden isimsiz kullanıcılar (anonymous users) içinde profil bilgilerinin tutulması ve saklanması istenebilir. Profile API, isimsiz kullanıcılar için oldukça güçlü bir hizmet sağlamaktadır. Sistemin çalışması aslında son derece basittir. Gerekli konfigurasyon ayarlarının yapılmasının ardından siteye bağlanan isimsiz kullanıcılar için birer GUID üretilmektedir. Bu GUID (Global Unique IDentifier) değerleri bir çerez (cookie) yardımıyla istemcinin bilgisayarında saklanırlar. Böylece isimsiz kullanıcıların sunucu tarafında tespit edilebilmesi kolay bir şekilde sağlanmaktadır. Dikkat edilmesi gereken durumlardan birisi istemcilerin oturum (session) açışları arasında farklı tarayıcılar kullanabilecek olmasıdır. Bu durumu ilerleyen kısımlarda analiz etmeye çalışacağız. İsimsiz kullanıcılara destek sağlayabilmek için web.config dosyası içerisinde anonymousIdentification elementinin ilgili özelliklerinin değerlerinin atanması gerekmektedir. Bir önceki örnek ile karışmaması açısından bu kez yeni bir web uygulaması ile devam edelim. Bu seferki örnekte profil özelliklerini web.config dosyası içerisinde tanımlıyor olacağız. Bu amaçla web.config dosyasının içeriğini aşağıdaki gibi tasarladığımızı varsayalım.

Web.config;

```xml
<?xml version="1.0"?>
<configuration>
    <appSettings/>
    <connectionStrings/>
    <system.web>
        <compilation debug="true"/>
        <authentication mode="Forms"/>
        <authorization>
            <allow users="*"/>
        </authorization>
        <anonymousIdentification enabled="true" cookieSlidingExpiration="true" cookieTimeout="3"/>
        <profile enabled="true">
            <properties>
                <add name="SonGirisZamani" type="System.DateTime" allowAnonymous="true"/>
                <group name="Urun">
                    <add name="UrunAdi" type="System.String" allowAnonymous="true"/>
                    <add name="UrunFiyati" type="System.Double" allowAnonymous="true"/>
                </group>
            </properties>
        </profile>
    </system.web>
</configuration>
```

anonymousIdentification elementi içerisinde tanımlanabilecek pek çok nitelik (attribute) yer almaktadır. Bu niteliklerden bazıları yukarıdaki örnekte kullanılmıştır. cookieSlidingExpiration niteliğine true değeri atandığı için istemciler, timeout süresi içerisinde talepte bulundukça çerezlerin (cookies) istemci bilgisayarda kalma süresi uzamaya devam edecektir. (Bu tipik olarak Caching mimarisinde kullanılan Sliding Expiration çalışma sistemi ile aynıdır.) Bununla birlikte cookieTimeout niteliğine atanan 3 değeri ile çerezin istemci bilgisayarda 3 dakika süreyle saklanacağı belirtilmektedir. (Uygulamanın doğrulanmış kullanıcılar (authenticated users) ile birlikte isimsiz kullanıcılarada (anonymous users) hizmet vermesi için authorization elementi içerisinde bilinçli olaraktan tüm kullanıcılar () allow edilmiştir.)

İlk olarak isimsiz kullanıcılara ait profil bilgilerinin saklandığını ispat etmeye çalışalım. Bu amaçla default.aspx sayfasının içeriğini ve kodlarını aşağıdaki gibi tasarladığımızı düşünelim.

![mk227_18.gif](images/mk227_18.gif)

Default.aspx;

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Untitled Page</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <asp:LoginName ID="LoginName1" runat="server" />
                <asp:LoginStatus ID="LoginStatus1" runat="server" />
            <br />
            <br />
            Son Giriş Zamanı :<asp:Label ID="lblSonGirisZamani" runat="server" Text="Label"></asp:Label><br />
            <br />
            Urun Adı :<asp:TextBox ID="txtUrunAdi" runat="server"></asp:TextBox><br />
            <br />
            Urun Fiyatı : <asp:TextBox ID="txtUrunFiyati" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btnProfilGetir" runat="server" OnClick="btnProfilGetir_Click" Text="Profil Getir" />
            <asp:Button ID="btnProfilKaydet" runat="server" OnClick="btnProfilKaydet_Click" Text="Profil Kaydet" /></div>
        </form>
    </body>
</html>
```

Default.aspx.cs;

```csharp
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page 
{
    protected void btnProfilGetir_Click(object sender, EventArgs e)
    {
        try
        {
            lblSonGirisZamani.Text = Profile.SonGirisZamani.ToString();
            txtUrunAdi.Text = Profile.Urun.UrunAdi;
            txtUrunFiyati.Text = Profile.Urun.UrunFiyati.ToString("C2");
        }
        catch (Exception exp)
        {
            Response.Write(exp);
        }
    }
    protected void btnProfilKaydet_Click(object sender, EventArgs e)
    {
        Profile.SonGirisZamani = DateTime.Now;
        Profile.Urun.UrunAdi = txtUrunAdi.Text;
        double urunFiyati = 1;
        Double.TryParse(txtUrunFiyati.Text, out urunFiyati);
        Profile.Urun.UrunFiyati = urunFiyati;
        Profile.Save();
    }
}
```

Uygulamayı çalıştırdığımızda isimsiz bir kullanıcı ile default.aspx sayfasını açabildiğimizi görürüz. Bu aşamada örnek bir kaç veri girip Profil Kaydet başlıklı düğmeye bastığımızda ise bilgilerin başarılı bir şekilde kaydedildiğini görebiliriz. Örneğin aşağıdaki veriler girildiğinde veritabanındaki aspnetprofile tablosunada bir satır eklendiği görülecektir.

![mk227_19.gif](images/mk227_19.gif)

![mk227_20.gif](images/mk227_20.gif)

Dahası kullanıcı siteye 3 dakikalık süre zarfı içerisinde yeni bir talepte (request) bulunduğunda ve Profil Getir başlıklı düğmeye bastığında ilgili bilgileri elde edebiliecektir.

![mk227_21.gif](images/mk227_21.gif)

Çok doğal olaraktan timeout süresi sona erdikten sonra profil bilgileri getirilmek istenirse aşağıdaki gibi bir ekran görüntüsü alınacaktır.

![mk227_22.gif](images/mk227_22.gif)

Bir başka deyişle istemci bilgisayardaki çerez (cookie) silindiğinden sunucuda eşleşen bir GUID bulunamamaktadır. (GUID'in bulunamaması isimsiz kullanıcının aspnetusers tablosundan silindiği anlamına gelmemektedir) Buda istenen bilgilerin elde edilemeyeceği anlamına gelir. Elbetteki veriler aspnetprofile tablosunda durmaya devam edecektir. Bu noktada ProfileManager sınıfının ilgili metodlarından yararlanarak belirli periyodlarda söz konusu verilerin temizlenmesi sağlanabilir.

Gelelim kullanıcının aynı uygulamaya isimsiz olarak farklı tarayıcılar ile erişmesi durumuna. Bu amaçla örneği önce Internet Exlplorer ile açıp profil bilgilerini kaydediyor olacağız. Sonrasında ise 3 dakikalık zaman dilimi sona ermeden Firefox ile aynı sayfayı yeniden talep edeceğiz. Örnek olarak ürün adını Vazo, ürün fiyatını 4.5 olarak girdiğimizi düşünelim.

![mk227_23.gif](images/mk227_23.gif)

Dikkat edileceği üzere Internet Explorer kullanılıp kaydedilen profil bilgilerine 3 dakikalık süre zarfı içerisinde başka bir tarayıcı program olan Firefox Mozilla içerisinde erişilememiştir. Tersi durumda söz konusudur. Buna göre farklı tarayıcı pencereleri ile gelen taleplerde (request) sunucunun farklı bir GUID üreteceğini ve istemci bilgisayara çerez olarak yazacağını göz önüne almalıyız.

İsimsiz kullanıcılar (anonymous users) ile ilgili bir diğer durumda var olan doğrulanmış bir kullanıcı ile birleştirilmeleridir (Migration). Bu durumu daha kolay anlayabilmek için [amazon.com](http://www.amazon.com) sitesinin işleyiş şeklini göz önüne alabiliriz. Bu siteye giren kayıtlı bir kullanıcı login olmadan sepete ürünler ekleyebilmektedir. Diğer taraftan kullanıcı alışveriş safhasına geçip sayfaya Login olduğunda, isimsiz kullanıcı olarak sepete attığı bilgiler var olan kullanıcı hesabındaki sepet bilgilerine eklenebilmektedir. Asp.Net 2.0 mimarisinde bu tarz bir işlemi belirli bir ölçüde kontrollü olarak gerçekleştirebilmek için için Global.asax.cs dosyasında ProfileOnMigrateAnonymous olay metodunun yüklenmesi yeterlidir. Bu metodun aldığı ProfileMigrateEventArgs tipinden parametre sayesinde isimsiz kullanıcı (anonymous user) için üretilen GUID değerine erişilebilir ve ilgili değerlerin alınarak, login olan kullanıcıya aktarılması sağlanabilir. Yanlız bu noktada daha öncede login olup profil bilgisi kaydedilmiş bir kullanıcının bilgileri üzerine yazılmamaya çalışılmasına özen gösterilmelidir. Bu durumu analiz edebilmek için global.asax.cs dosyasına aşağıdaki kod parçasını eklediğimizi göz önüne alabiliriz.

```javascript
<%@ Application Language="C#" %>

<script runat="server">

    void Profile_OnMigrateAnonymous(object sender, ProfileMigrateEventArgs e)
    {
        /* Önce isimsiz kullanıcının profile bilgisi elde ediliri. GetProfile metodunun dönüş değeri ProfileCommon tipinden olacağı için buna göre bir atama yapılır. */
        ProfileCommon isimsizProfil = Profile.GetProfile(e.AnonymousID);
        /* Login olan doğrulanmış kullanıcının halen aktiv olup olmadığı öğrenilir. Eğer aktiv ise profil bilgileri mevcut demektir ve üstüne yazılması istenmez. Bu nedenle LastActivityDate değerine bakılarak bir kontrol yapılması gerekmektedir. */
        if (Profile.LastActivityDate.Date == DateTime.MinValue.Date)
        {
            // İsimsiz kullanıcının profil özelliklerinin değerleri ile login olan doğrulanmış kullanıcının profil özellikleri eşleştirilir
            Profile.SonGirisZamani = isimsizProfil.SonGirisZamani;
            Profile.Urun.UrunAdi = isimsizProfil.Urun.UrunAdi;
            Profile.Urun.UrunFiyati = isimsizProfil.Urun.UrunFiyati;
            Profile.Save(); // Login olan kullanıcı için değiştirilen profil değerleri kaydedilir.
        }
        AnonymousIdentificationModule.ClearAnonymousIdentifier();// isimsiz kullanıcı için üretilen çerezin silinmesi sağlanırki bu olay tekrar tetiklenmesin
        ProfileManager.DeleteProfile(e.AnonymousID); //isimsiz kullanıcıya ait profil değerleri veritabanından silinir.
    }

</script>
```

Yukarıdaki örnek kod parçasında öncelikli olarak isimsiz kullanıcının profil bilgileri elde edilmektedir. Bu amaçla GetProfile metodu kullanılmıştır. Sonrasında ise, o an Login olmuş kullanıcının bir profil bilgisi olup olmadığı tespit edilir. Bu kontrolde LastActivityDate özelliğinden yararlanılır. Burada amaç isimsiz kullanıcının (anonymous users) profil bilgilerini, login olmuş kullanıcının var olan profil bilgileri üzerine yazmamaktır. (Burada global bir kontrol yapılıp istemciden bilgilerin üstüne yazmak isteyip istemeyeceği ele alınaraktanda ilerlenebilir.

Bu tamamen uygulamanın veya sürecin ihtiyaçları doğrultusunda verilebilecek bir karardır.) Sonrasında login olan kullanıcının profile kaydettiği bir bilgi yoksa isimsiz kullanıcı için oluşturulan değerlerin ataması yapılır ve kaydetme işlemi (save) gerçekleştirilir. Son olarak olay metodunun tekrardan tetiklenmemesi amacıyla isimsiz kullanıcıya ait bilgi veritabanından silinir. Ayrıca istemcideki çerez bilgiside AnonymousIdentificationModule sınıfının static ClearAnonymousIdentifier metodu ile geçersiz kılınır. Sonuç olarak isimsiz kullanıcı giriş yapıp profil bilgisini kaydettikten sonra daha önceden profil bilgisi bulunmayan bir kullanıcı gibi Login olursa, var olan bilgileri doğrulanmış kullanıcınınkine aktarılacaktır.

Böylece geldik uzun bir makalemizin daha sonuna. Bu makalemizde profile yönetimini biraz daha güçlü ve esnek hale getirmek için neler yapabileceğimizi incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](assets/ProfileManagement.rar) (Dosya boyutunun küçük olması amacıyla Aspnetdb veritabanları ve log dosyaları çıkartılmıştır)
