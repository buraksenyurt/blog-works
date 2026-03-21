---
layout: post
title: "Private Assembly ve Shared Assembly Kavramı"
date: 2004-04-22 09:00:00 +0300
categories:
  - csharp
tags:
  - Framework
  - assembly
  - common-language-runtime
---
Bu makalemizde,.NET'in temellerinden olan Assembly kavramının önemli bir bölümü olan Global Assembly Cache'i incelemeye çalışacağız. Net dilinde, assembly'ları private (özel) ve shared (paylaşımlı) olmak üzere iki kategoriye ayırabiliriz. Private assembly'lar oluşturulduklarında, çalıştırılabilmeleri için, uygulama ile aynı klasör altında yer almalıdırlar. Söz gelimi aşağıdaki gibi bir assembly'a sahip olduğumuzu düşünelim.

```csharp
using System;

namespace PriAsm
{
    public class Hesap
    {
        public double Topla(double a,double b)
        {    
            return a+b;
        }
    }
}
```

Visual Studio.Net ortamında bir class library olarak oluşturduğumuz bu assembly derlendiğinde, PriAsm.dll dosyasının oluşturulduğunu görürüz.

![mk64_1.gif](/assets/images/2004/mk64_1.gif)

Şekil 1. Assembly dosyamız.

Şimdi, oluşturulan bu assembly içindeki Hesap isimli sınıfımızı, başka bir klasörde yer alan bir uygulama içerisinde kullanmak istediğimizi düşünelim. Bu durumda, PriAsm.dll dosyamızı, kullanmak istediğimiz uygulamanın klasörüne kopyalamamız yeterli olucaktır. Örneğin aşağıdaki uygulamayı göz önüne alalım.

```csharp
using System;
using PriAsm;

namespace Ornek
{
    class Class1
    {
        static void Main(string[] args)
        {

        }
    }
}
```

Bu Console uygulamasını derlemeye çalıştığımızda, "The type or namespace name 'PriAsm'could not be found (are you missing a using directive or an assembly reference?)" hatasını alırız. Bu hatayı almamız son derece doğaldır. Çünkü PriAsm.dll assembly'ımız private bir assembly'dır. Bunun doğal sonucu uygulamamızın bu assembly hakkında hiç bir bilgiye sahip olmamasıdır. Uygulamamıza, PriAsm.dll assembly'ının referans edilmesi gerekmektedir. Öncelikle, PriAsm.dll dosyasını, uygulamamızın assembly'ının bulunduğu klasöre kopylamamız gerekir.

![mk64_2.gif](/assets/images/2004/mk64_2.gif)

Şekil 2. PriAsm.dll ile uygulama assemble'ı aynı yerde olmalı.

Daha sonra ise uygulamamıza PriAsm.dll assembly'ını referans etmemiz gerekir. Bu amaçla solution explorer'da assembly ismine sağ tıklanır ve Add Reference seçilir.

![mk64_3.gif](/assets/images/2004/mk64_3.gif)

Şekil 3. Add Reference.

Karşımıza gelen Add Reference penceresinden, Projects kısmına geçilir.

![mk64_4.gif](/assets/images/2004/mk64_4.gif)

Şekil 4. Projects

Burada Browse seçeneği ile, PriAsm.dll assembly'ının bulunduğu klasöre gidilir ve bu dll dosyası seçilerek referans ekleme işlemi tamamlanmış olur.

![mk64_5.gif](/assets/images/2004/mk64_5.gif)

Şekil 5. Referansın eklenmesi.

Bu noktadan sonra Solution Explorer penceresine baktığımızda, PriAsm isim alanının eklenmiş olduğunu görürüz.

![mk64_6.gif](/assets/images/2004/mk64_6.gif)

Şekil 6. PriAsm eklenmiş durumda.

Artık uygulamamızda bu assembly içindeki sınıfları kolayca kullanabiliriz. Aşağıdaki basit örnekte bunu görebiliriz.

```csharp
using System;
using PriAsm;
namespace Ornek
{ 
    class Class1
    {
        static void Main(string[] args)
        {
            Hesap h=new Hesap();
            double sonuc=h.Topla(4,5);
            Console.WriteLine(sonuc.ToString());
        }
    }
}
```

Private assembly'ları bu şekilde kullanmak her zaman tercih edilen bir yol değildir. Nitekim, PriAsm.dll assembly'ının erişebilmek bu dll'in, uygulamanın assembly'ı ile aynı klasörde olması gerekmektedir. (Nitekim, PriAsm.dll assembly'ının uygulamanın debug klasöründen başka bir yere taşınması, uygulamanın derleme zamanında hata vermesine yol açar. Çünkü belirtilen adresteki referans dosyası yerinde değildir.) Buna karşılık olarak bazen, oluşturduğumuz assembly'a birden fazla uygulamanın aynı yerden erişmesini isteyebiliriz. İşte bu durumda devreye Global Assembly Cahce girmektedir. GAC.NET uygulamaları tarafından paylaşılan bileşenlerin yer aldığı bir veri deposudur. Birden fazla uygulamanın ortak olarak kullanacağı assembly'lar sistemdeki Global Assembly Cache 'e yüklenerek paylaşılmış (shared) assembly'lar oluşturabiliriz. GAC 'da tutulan assembly'ların görüntüsüne, bir Win XP sisteminde C:\WINDOWS\assembly klasöründen ulaşabiliriz. Burada yer alan assembly'lar, C# kodu ilk olarak yürütüldüğünde anında derlenir ve GAC önbelleğinde tutulurlar.

![mk64_7.gif](/assets/images/2004/mk64_7.gif)

Şekil 7. GAC Assembly'ları.

Burada dikkat edicek olursanız örneğin, System.Data Assembly'ından iki adet bulunmaktadır. Bu iki assembly'ın sistemde sorunsuz bir şekilde çalışması, assembly'ların kimliklerinin farklılığı sayesinde mümkün olmaktadır. Bu farklılığı yaratan GAC içine kurulan her bir assembly'ın farklı strong name'lere sahip olmasıdır. Bir strong name, bir assembly'ın adı, versiyon numarası, dil bilgileri, dijital imzaları ve public anahtar değeri bilgilerinden oluşur. Örneğin System.Data assembly'ının iki versiyonu arasındaki farklar aşağıdaki şekilde görüldüğü gibidir.

![mk64_8.gif](/assets/images/2004/mk64_8.gif)

![mk64_9.gif](/assets/images/2004/mk64_9.gif)

Şekil 8. Farklılıklar.

Bir assembly'ın yukarıda bahsedilen bilgileri AssemblyInfo isimli dosyada tutulmaktadır.Şimdi dilerseniz geliştirmiş olduğumuz PriAsm.dll assembly'ını GAC'e nasıl kayıt edeceğimizi incelemeye çalışalım. İlk olarak bize bu assembly'ı sistem için benzersiz (unique) yapacak bir strong name gerekli. Bir strong name üretmek için,.Net FrameWork'un sn.exe tool'unu kullanabiliriz.

![mk64_10.gif](/assets/images/2004/mk64_10.gif)

Şekil 9. Strong Name'in oluşturulması.

Bu işlemin ardından oluşan Anahtar.sif isimli dosyayı, assembly'ımıza bildirmemiz gerekiyor. Bunun için AssemblyInfo dosyasındaki AssemblyKeyFile niteliğini kullanacağız. (Burada dosya uzantısının sif olmasının özel bir nedeni yok. Ben sifre'nin sif'ini kullandım. Sonuç olarak dosya binary yazılacağı için herhangibir format verilebilir.)

```csharp
[assembly: AssemblyKeyFile("D:\\vssamples\\PriAsm\\bin\\debug\\Anahtar.sif")]
```

Artık assembly'ımız bir strong name'e sahip. Dolayısıyla GAC içindeki assembly'lardan ve sonradan gelibilecek olan assembly'lardan tamamıyle farklı bir yapıya büründü. Artık oluşturduğumuz bu assembly'ı GAC'e alabiliriz. Bunu iki yolla gerçekleştirebiliriz. İlk olarak,.Net Framework'ün GACUtil.exe tool'unu bu iş için kullanabiliriz.

![mk64_11.gif](/assets/images/2004/mk64_11.gif)

Şekil 10. Assembly'ın GAC'e kurulması.

Bu işlemin ardından GAC klasörüne baktığımızda, PriAsm assembly'ımızın eklenmiş olduğunu görürüz.

![mk64_12.gif](/assets/images/2004/mk64_12.gif)

Şekil 11. Assembly'ın GAC'e eklenmesi.

Diğer yandan aynı işlemi, PriAsm.dll dosyasını bu klasöre sürükleyerekte gerçekleştirebiliriz. Artık bu noktadan itibaren, PriAsm assembly'ına ve içindeki sınıflara, herhangibir.net uygulamasından kolayca erişebiliriz. Örneğin, herhangibir.net uygulamasından PriAsm assembly'ına ulaşmak için tek yapmamız gereken assembly'ı uygulamamıza aynı private assembly'larda olduğu gibi referans etmektir. Fakat bu sefer, PriAsm.dll assembly'ını uygulamamızın PE (Portable Executable) dosyasının bulunduğu debug klasörüne almak gibi bir zorunluluğumuz yoktur. Çünkü, programın ilk derlenişinde, PriAsm.dll, GAC'e alınır ve burada tutulur. Dolayısıyla aşağıdaki örnek kodların yer aldığı Console Uygulamasının bulunduğu debug klasörüne PriAsm.dll dosyasının yüklenmesi gerekmez.

```csharp
using System;
using PriAsm;
namespace ConsoleApplication3
{
    class Class1
    {
        static void Main(string[] args)
        {
            Hesap h=new Hesap();
            double sonuc=h.Topla(1,2);
        }
    }
}
```

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.