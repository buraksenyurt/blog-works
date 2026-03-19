---
title: "Ubuntu'da İlk .Net Core Adımlarım"
pubDate: 2017-11-05 10:17:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - C#
  - .NET
  - Docker
  - Go
  - Python
  - Ruby
  - Visual Studio
  - GitHub
---

# Ubuntu'da İlk .Net Core Adımlarım
![image.axd](images/image.axd)

Merhaba Arkadaşlar,

Üniversite yıllarımda internet yeni yeni yaygınlaşmaya başlayan bir ortamdı. 14400 kpbs hızındaki modemimi daha 3ncü sınıfta alabilmiştim. Bu sebepten okulun ilk yıllarında bilgisayar teknolojileri ile ilgili bilgileri öğrenebileceğim en güzel mecra aylık dergilerdi. Bazıları zaman içerisinde kapandı bazılarıysa internet üzerinden yayınlanmaya başladı. Ben ağırlıklı olarak PcWorld, PcNet, PcMagazine gibi dergileri okuduğumu hatta ay ay biriktirdiğimi hatırlıyorum.

Byte severek takip ettiğim dergilerden birisiydi. Özellikle de Jerry Pournelle'in köşesi. Her ay farklı şeylerle uğraşır, yaptığı denemelerin sonuçları paylaşır, bilim kurgu üzerine konuşur, kitap önerilerinde bulunurdu. Yeni yazılımları dener ve deneyimlerini aktarırdı. The New York Times [ilgili yazısında](https://www.nytimes.com/2017/09/15/obituaries/jerry-pournelle-science-fiction-novelist-and-computer-guide-dies-at-84.html) onu bilim kurgu yazarı ve bilgisayar rehberi olarak tanımlamıştır. Araştırmayı ve bu araştırmaları sırasında öğrendiklerini paylaşan değerli bir bilim insanıydı benim için.

Ne yazık ki 8 Eylül 2017 tarihinde aramızdan ayrılmış. Benim de bir şeyleri araştırıp bu araştırmalarla ilgili notlar almama vesile olan ve kendime göre bir gelişim metodolojisini alışkanlık haline getirmemi sağlayan insanlardan birisiydi.

Bu düşünceler eşlinde hafta sonu işlerime başladım. Evdeki yaklaşık 6 yıllık Dell marka dizüstü bilgisayarımda uzun süredir iki işletim sistemi kullanıyordum. Ubuntu ve Windows 7. Bir süredir de Ubuntu üzerinde Docker kullanarak.Net Core 2.0 uygulamaları geliştirmeyi planlıyordum. Ancak bir türlü Community Edition sürümünü yükleyemedim. Sebep Linux'ün 64bit olmamasıydı. Windows 7 üzerinde Docker kullanabilirdim ama amacım Cross Platform deneyimini.Net Core için Linux üzerinde yaşamaktı. Öyleyse ilk etapta işi Docker olmadan halledeyim dedim. Yine de 64bit bir Ubuntu bana gerekliydi. Sonunda kararımı verdim ve dizüstünü sıfırlayarak üzerine sadece Ubuntu 64bit işletim sistemini kurdum ([Şu adresteki](https://www.ubuntu.com/download/desktop/contribute?version=17.10&architecture=amd64) sürümü kullandım)

Docker deneyimlerine geçmeden önce.Net Core 2.0 ile Ubuntu'da bir Hello World demeye karar verdim. Amacım bir Linux platformu üzerine.Net Core 2.0 SDK'yi yüklemek ve C# ile yazılmış bir Console uygulamasını kullanarak yeni bir ufka merhaba demekti. Benim için önemli bir başlangıç noktası. Nitekim önümüzdeki sene içerisinde işten arta kalan zamanlarda Linux üzerinde.Net Core 2.0 ile bir şeyler yapmaya gayret edeceğim.

.Net Core'un Kurulumu

Terminal penceresini ve Microsoft'un konu ile ilgili dokümantasyonunu açtıktan sonra kurulum adımlarını takip etmeye başladım. Okuduğum kadarıyla öncelikli olarak Microsoft Product Feed listesine kayıt olmak gerekiyor. Bunu iki adımda gerçekleştiriyoruz. Önce güvenli bir anahtar sonrasında da kayıt. Sadece bir kere yapmamız yeterli. Kayıt işlemini gerçekleştirirken kullandığımız Ubuntu sürümünü de dikkate almalıyız. Ben Ubuntu 16.04 Linux Mint versiyonunu yüklediğim için buna uygun bir bildirim yaptım (Xenial ifadesinin kullanılmas sebebi) Bu arada curl komutunun genellikle internetten içerik indirmek için kullanıldığını öğrendim (Linux tarafında çok acemiyim biliyorsunuz ki)

```bash
curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg

sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg

sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'
```

![image.axd](images/image.axd)

Sorunsuz geçen bu sürecin ardından sistemi güncelleyip.Net Core 2.0 sürümünü indirmeye başladım. Tabii alışılageldiği üzere önce bir güncelleme yapmakta ve eksik paketlerin yüklenmesini sağlamakta yarar var.

```bash
sudo apt-get update

sudo apt-get install dotnet-sdk-2.0.0
```

Eğer bir probem yoksa terminalden dotnet komutunu kullanarak versiyon numarasını ve güncel sürüm bilgilerini aşağıdaki gibi görebilmemiz gerekiyor.

```bash
dotnet --version

dotnet --info
```

![image.axd](images/image.axd)

Console Uygulamasının Oluşturulması

Bildiğiniz gibi dotnet new komutu ile önceden tanımlı şablonlardan yararlanarak projelerimizi oluşturabiliyoruz (Neler oluşturabileceğimizi görmek için dotnet new --help ifadesinden yararlanabilirsiniz) Benim ilk hedefim terminal pencersinde basit bir şeyler yazdırıp Hello World diyebilmek. Bu sebepten oluşturduğum Development klasörü üzerinde aşağıdaki ifadeyi kullanarak HelloWorld isimli bir Console projesi oluşturdum.

```bash
dotnet new console -o HelloWorld
```

![image.axd](images/image.axd)

Oluşan Program.cs içeriğini biraz değiştirdim ve Algebra.cs isimli bir sınıfı da klasöre dahil ettim.

Algebra.cs içeriği;

```csharp
using System;

namespace Utility
{
    public class Algebra
    {
        public double Sum(double x,double y)
        {
            return x+y;
        }
    }
}
```

Program.cs içeriği;

```csharp
using System;
using Utility;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Merhaba");
            double x=1.23;
            double y=Math.PI;
            Algebra einstein=new Algebra();
            Console.WriteLine("{0}+{1}={2}",x,y,einstein.Sum(x,y));
        }
    }
}
```

Tahmin edeceğiniz üzere iki double sayının toplamını hesap eden Sum fonksiyonunu içeren Algebra isimli sınıfın basit bir kullanımı söz konusu. Projeyi derleyip çalıştırmak içinse build ve run parametrelerinden yararlanıyoruz.

```bash
dotnet build

dotnet run
```

![image.axd](images/image.axd)

Belki de pek çoğunuz için etkileyici bir sonuç değil ama benim oldukça hoşuma gitti. Vakti zamanında sadece Windows platformunda.Net ile kod yazabildiğimi düşününce, kurduğum Linux makinede bu ortamı kullanarak C# ile kod geliştirebilmek harika bir deneyim.

Visual Studio Code Kurulumu

Bu arada kodları nasıl düzenlediğimi merak etmiş de olabilirsiniz. Tahmin edeceğiniz gibi Visual Studio Code'u kullandım. Bu uygulamayı da terminalden aşağıdaki komutları kullanarak yükleyebilirsiniz. Yükleme işleminden sonra HelloWorld uygulamasını açtığımızda C# için gerekli Extension'da öneri olarak sunulacaktır.

```bash
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/vscode stable main" > /etc/apt/sources.list.d/vscode.list'

sudo apt-get update

sudo apt-get install code
```

![image.axd](images/image.axd)

Eğer komut satırından kurulum zor geliyorsa basit kurulum için [şu adresten de](https://code.visualstudio.com/Download?wt.mc_id=DotNet_Home&dotnetid=572034740.1488954905) yararlanabilirsiniz.

Şimdi Ne Olacak?

Artık emektar dizüstü bilgisayarımda Ubuntu 64bit sürümü yüklü. Tertemiz bir ortamım var. Bu ortamda şu an için.Net Core 2.0 ve Visual Studio Code bulunuyor. Ama yapmam gereken başka işler var. GitHub ile entegre olmalıyım. Diğer yandan Docker kurulumunu gerçekleştirip bir.Net Core 2.0 imajı üzerinde çalışmalar yapmalıyım. Ayrıca GoLang, Ruby ve Python için de ortamı hazırlamalıyım. Bu arada Linux terminal için biraz komut çalışsam hiç de fena olmaz. Python tarafında sudo, apt-get gibi ifadelere aşina olsam da curl gibi yeni karşılaştığım komutların farkında olmalıyım.

Görüldüğü gibi evinizdeki bilgisayarımıza Linux kurup üzerinde.Net Core 2.0 ile Hello World dememiz oldukça basit. Daha kolayı sanıyorum ki Docker Container kullanımı. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
