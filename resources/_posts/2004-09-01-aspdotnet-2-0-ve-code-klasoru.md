---
layout: post
title: "Asp.Net 2.0 ve Code Klasörü"
date: 2004-09-01 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - dotnet
  - aspnet
  - xml
  - web-service
  - visual-studio
  - dataset
---
Bu makalemizde, Asp.Net 2.0 ile gelen tanımlanmış klasörlerden (defined folders), Code klasörünün nasıl kullanıldığını incelemeye çalışacağız. Asp.Net 1.0/1.1 ile uygulama geliştirirken, solution içindeki herşey bir dll içinde (assembly) toplanır. Asp.Net 2.0 ise, dosya tabanlı (file-based) yaklaşım adı verilen yeni bir teknik kullanır. Bu tekniğe göre, solution, dosyalar ve klasörler sisteminden oluşmaktadır. Bu sistemin faydası, otomatik derleme özelliğine sahip olmasıdır.

Yani, solution içerisine herhangibir dosya eklenmesi halinde (örneğin bir sınıf), Visual Studio.Net 2005 bu dosyayı otomatik olarak derler ve solution'ın her zaman dinamik olarak güncel kalmasını sağlar. Bir başka deyişle yeni eklenen dosya için, solution'ın baştan derlenmesi gerekmez. Bu yüzden, Asp.Net 2.0 ile uygulama geliştirirken ilk dikkati çeken unsur, oluşturulan sanal klasör altında eskiden olduğu gibi bin klasörünün ve bir dll dosyasının olmayışıdır.

Asp.Net 2.0 ile geliştirilen bu yeni teknikte, çeşitli önceden tanımlanmış (predefined folders) özel klasör seçenekleri mevcuttur. Bunlardan biriside Code klasörüdür. Code klasörü, sınıf dosyalarımız, web servisleri için kullanılan wsdl dosyalarımız, türlendirilmiş dataset'ler (typed datasets) için kullanılan xml şemaları ve Data Component'ler tipindeki dosyaları barındırılabilir. Code klasörü içerisine konulan bu dosyalar, Visual Studio.Net 2005 ortamında otomatik olarak tanınır. Ayrıca, Visual Studio.Net 2005, bu dosyaları kullanarak, sınıflara derler, proxy sınıflarını veya türlendirilmiş veri sınıflarını oluşturur. Bu yapıyı aşağıdaki şekil ile daha kolay anlayabiliriz.

![mk85_10.gif](/assets/images/2004/mk85_10.gif)

Şekil 1. Genel yapı.

Dilerseniz konuyu daha iyi anlayabilmek amacıyla basit bir örnek geliştirelim. Bunun için Visual Studio.Net 2005' de yeni bir Web Site açıyoruz. Daha sonra Solution Explorer'da solution'ımız üzerine sağ tıklıyor ve New Folder seçeneğine basarak, yeni bir klasör oluşturuyoruz. Klasörümüze Code ismini verdiğimizde, şeklinin normal klasörlerden biraz daha farklı olduğunu hemen farkedebiliriz. Nitekim Code klasörünün Solution için özel bir anlamı vardır.

![mk85_1.gif](/assets/images/2004/mk85_1.gif)

Şekil 2. Code klasörünün eklenmesi.

Şimdi default.aspx form'unuda aşağıdaki gibi oluşturalım. Bu web sayfasında basit olarak, yarıçapı verilen bir dairenin alanı hesap edilecek. Bu hesaplama işlemini yapan metodumuzu barındıracak bir sınıfımız olacak ve bu sınıfımız, Code klasörü içerisinde yer alacak.

![mk85_2.gif](/assets/images/2004/mk85_2.gif)

Şekil 3. Form tasarımımız.

Şimdi, Code klasöründe sağ tıklayalım ve Add New Item'i seçelim. Karşımıza aşağıdaki dialog penceresi çıkacaktır.

![mk85_3.gif](/assets/images/2004/mk85_3.gif)

Şekil 4. Code klasörü için yeni bir öğe eklemek.

Bu dialog pencersinde, Code klasörüne ekleyebileceğimiz dosya tipleri yer almaktadır. Biz class tipini seçeceğiz. AlanHesap.cs dosyamızın kodları aşağıda görüldüğü gibidir.

```csharp
using System;

public class AlanHesap
{
    public AlanHesap()
    {

    }
    public double DaireAlan(double yaricap)
    {
        return 3.14 * (yaricap * yaricap);
    }
}
```

Bu noktadan sonra, solution'ımızı hiç derlemeden, AlanHesap sınıfımızı kullanabilir, bu sınıftan nesne örnekleri yaratabilir daha da önemlisi intelli-sense özelliğinden derhal faydalanabiliriz.

![mk85_4.gif](/assets/images/2004/mk85_4.gif)

Şekil 5. Intelli-Sense Özelliği.

Bu noktadan sonra, uygulamamızı oluşturduğumuz klasöre bakarsak aşağıdaki yapıda olduğunu farkederiz. Dikkat edecek olursanız Asp.Net 1.0/1.1' deki gibi kalabalık bir topluluk yoktur. En önemlisi Bin klasörünü veya tüm uygulamanın tiplerine ait manifesto bilgilerini ve kodları barındıran bir dll görememekteyiz.

![mk85_5.gif](/assets/images/2004/mk85_5.gif)

Şekil 6. Klasör Azlığı.

Code klasörü için dikkat edilecek noktalardan birisi, buradaki dosyaların tamamının single assembly olarak ele alınmalarıdır. Yani, bu klasör altındaki tüm dosyalar aynı dil ile yazılmış olmalıdır. Nitekim, klasörümüze vb.net ile yazılmış aşağıdaki class dosyasını eklediğimizi düşünelim.

```text
Imports Microsoft.VisualBasic

Public Class VbSinif

    Public Function Deneme(ByVal yaricap As String) As String
        Return (yaricap)
    End Function

End Class
```

Bu durumda solution'ı derlediğimizde aşağıdaki derleme zamanı hata mesajını alırız.

![mk85_6.gif](/assets/images/2004/mk85_6.gif)

Şekil 7. Hata Mesajı.

Peki çözüm nedir? Büyük çaplı projelerde, farklı.net dilleri kullanılarak geliştirilen sınıfların aynı solution içerisinde kullanılması için ne yapabiliriz? Bunun için, öncelikle Code klasörü içinde her bir dile yönelik olarak ayrı klasörler açmamız gerekir. Aşağıdaki şekilde olduğu gibi.

![mk85_7.gif](/assets/images/2004/mk85_7.gif)

Şekil 8. Farklı diller için farklı klasörler.

Buradaki alt klasörleri isimlendirmek için belirli bir kural yoktur. Ancak buradaki isimlendirmelerin aynısını Web.Config dosyasındaki node'unda kullanmamız gerekmektedir. Nitekim, otomatik olarak yapılan önceden derleme işlemlerinde, hangi alt klasörlerin kullanılacağının sitenin konfigurasyon ayarlarına yansıtılması gereklidir. Bunun için Web.Config dosyasındaki,

```text
<compilation debug="true">

</compilation>
```

kısmını aşağıdaki ile değiştirmemiz yeterli olacaktır.

```text
<compilation debug="true">
    <codeSubDirectories>
        <add directoryName ="CSharp"/>
        <add directoryName ="VbDotNet"/>
    </codeSubDirectories>
</compilation>
```

Şimdi bu işlemlerin ardından default.aspx sayfasına geçtiğimizde, vb.net ile yazdığımız sınıfa erişebildiğimizi ve kullanabildiğimizi görürüz.

![mk85_8.gif](/assets/images/2004/mk85_8.gif)

Şekil 9. Farklı dil ile yazılmış sınıfa erişim.

Şimdi kodumuzu aşağıdaki gibi geliştirelim ve sayfamızı çalıştıralım.

```csharp
void btnHesapla_Click(object sender, EventArgs e)
{
    AlanHesap ah = new AlanHesap();
    lblSonuc.Text = ah.DaireAlan(Convert.ToDouble(txtYaricap.Text)).ToString();

    VbSinif s = new VbSinif();
    string deger = s.Deneme(txtYaricap.Text.ToString());
    lblDeger.Text = deger;
}
```

![mk85_9.gif](/assets/images/2004/mk85_9.gif)

Şekil 10. Farklı dil ile yazılmış sınıflar bir arada çalışıyor.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.