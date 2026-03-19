---
layout: post
title: "Docker Üzerinde .Net Core Uygulaması Çalıştırmak"
date: 2017-11-10 06:01:00 +0300
description: "Internet üzerinde Docker ile ilgili pek çok bilgi ve kaynağa ulaştım. Ama özellikle Asiye Yiğit'in Linkedin üzerinden paylaştığı yazılar giriş seviyesinde bilgi sahibi olmamı sağladı. Çıkarttığım notlardan yararlanarak konuyu kavramak için şekilleri tekrardan ele aldım ve kısa özet çıkarttım. Öncelikle bu notlarımı temize çekeceğim. Sonrasında Ubuntu üzerine Docker kuracağım. Ardından .Net Core 2.0 için basit bir Console uygulaması yazacağım. Son adımda ise bu uygulamayı Docker üzerinde ayağa kaldıracağım. Haydi gelin başlayalım."
categories:
  - dotnet-core
tags:
  - dotnet-core
  - bash
  - csharp
  - dotnet
  - docker
  - python
  - github
---
# Docker Üzerinde .Net Core Uygulaması Çalıştırmak
![core_docker_9.gif](/assets/images/2017/core_docker_9.gif)

Merhaba Arkadaşlar,

Biliyorum epeyce geriden geliyorum yeni nesil konularda ama işler güçler derken ancak zaman bulabiliyorum. Önceki yazılarımdan da hatırlayacağınız üzere evdeki emektar dizüstü bilgisayarıma Ubuntu'nun 64bitlik sürümünü yüklemiştim (Makineye West-World adını verdim) Üzerinde ilk.Net Core denemelerimi de gerçekleştirdim. Ancak merak ettiğim konulardan birisi de Docker üzerinde bir.Net Core uygulamasının nasıl çalıştırılabileceğiydi. Bu iş sandığımdan daha zor olacaktı. Yarım yamalak bilgimle Docker'ın ne olduğunu az çok biliyordum ama tam anlamıyla da hakim değildim. En azından biraz daha fikir sahibi olmalı, kurulumunu gerçekleştirmeli ve sonrasında örnek bir.Net Core uygulamasını Dockerize ederek taze bir imaj (image) üzerinde ayağa kaldırabilmeliydim.

Internet üzerinde Docker ile ilgili pek çok bilgi ve kaynağa ulaştım. Ama özellikle [Asiye Yiğit'in Linkedin üzerinden paylaştığı yazılar](https://tr.linkedin.com/pulse/docker-asiye-yigit) önemli bilgiler edinmemi sağladılar. Bunun haricinde DevOps tarafında oldukça yetenekli olan arkadaşım (ki hemen solumda oturur) Alpay Bilgiç, beni aydınlatan bilgiler verdi. Ne sorsam cevapladı. Çıkarttığım notlardan yararlanarak konuyu kavramak için şekilleri tekrardan ele aldım. Öncelikle ilgili notları bu blog yazısı aracılığıyla temize çekeceğim ki yarın öbür gün nasıl oluyordu bu iş dediğimde dönüp bakabileyim. Sonrasında Ubuntu üzerine Docker kuracağım. Ardından.Net Core 2.0 için basit bir Console uygulaması yazacağım. Son adımda ise bu uygulamayı Docker üzerinde ayağa kaldıracağım. Haydi gelin başlayalım.

Docker'dan Anladığım

Aslında her şey farklı platformlarda çalışabilecek uygulamaların ölçek büyüdükçe daha çok makineye ve kuruluma ihtiyaç duyması sonrasında başlamış gibi duruyor. Yeni makine demek, yeni kurulumlar, yeni lisans ücretleri, yeni yönetim sorumlulukları, yeni dağıtım süreçleri, yeni elemanlar demek. Durum böyle olunca maliyetlerin artması da kaçınılmaz hale gelmiş. Benim üniversite yıllarında da tanık olduğum o eski yaklaşım kabaca aşağıdaki şekilde görüldüğü gibiydi.

![h8+Eae6lnKfwwAAAABJRU5ErkJggg==](data:image/png;base64,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)

İlk dünya yukarıdaki gibiydi. Sonrasında ise Hyper-V (Fiziki bir makinede birden fazla sunucu rolünü bağımsız sanal roller içerisinde çalıştırımamızı sağlayan Microsoft ürünü de diyebiliriz) gibi isimler duymaya başladık. Bir başka deyişle sanallaştırma kavramları ile içli dışlı olmaya başladık.

![core_docker_2.gif](/assets/images/2017/core_docker_2.gif)

Sanallaştırma sayesinde tek bir fiziki sunucu üzerinde farklı işletim sistemlerini konuşlandırabilmekte. Bu teknikle özellikle dağıtım süreçlerinin hızlandığını ve yeni fiziksel sunucular almak zorudan kalmadığımız için maliyet avantajları sağlandığını ifade edebiliriz. Pek tabii ölçekleme maliyetleri de azalıyor. Ancak her ziyaretçi işletim sistemi (Guest OS) için ayı bir işletim sistemi barındırmak durumunda da kalıyoruz ki bu negatif bir özellik olarak karşımıza çıkıyor. Diğer yandan uygulamalarının taşınabilirliği yeteri kadar esnek olmuyor. Diğer bir dezavantaj.

Derken karşımıza Docker diye bir şey çıktı. Go dili ile geliştirildiği söylenen bu yeni yaklaşımın özeti kabaca aşağıdaki şekilde görüldüğü gibi.

![core_docker_3.gif](/assets/images/2017/core_docker_3.gif)

Docker gibi Container araçları sayesinde uygulamalarımızı sadece ihtiyaç duydukları kütüphaneler (paketler) ile birlikte birbirlerinden izole olacak şekilde çalışabilir halde sunabiliyoruz. Dağıtımın kolaylaşması dışında taşınabilirlik de kolaylaşıyor. En önemli artılarından birisi ise uygulamalara has çalışma zamanlarının birbirlerinden tam anlamıyla izole edilebiliyor olması.

Aşağıdaki şekil Docker'ın temel çalışma mimarisi özetlenmeye çalışılmakta.

![core_docker_4.gif](/assets/images/2017/core_docker_4.gif)

Docker temel olarak istemci-sunucu mimarisine uygun olarak geliştirilmiştir. GO dili ile yazıldığını sanıyorum belirtmiştik. Kullanıcılar esas itibariyle Docker Client üzerinden Demaon ile iletişim kuruyorlar. Build, Pull ve Run gibi komutlar Docker Client aracılığıyla, Deamon üzerinden işletilmekteler. Docker Demaon devamlı olarak çalışan bir Process (Sanırım Windows Service'e benzetebiliriz) Container’lar aslında birer çalışma zamanı nesnesi ve uygulamaların yürütülmesi için gerekli ne varsa (betikler, paketler vs) barındırıyorlar. Image öğeleri de Container’ların oluşturulması için kullanılan yalnızca okunabilir şablonlar olarak tasarlanmışlar. Şekilde Build, Pull ve Run operasyonlarının temel çalışma prensiplerini görebiliriz (Okların renklerine dikkat edelim)

Özetleyecek olursak

- Docker Store: Güvenilir ve kurumsal seviyedeki imajların kayıt altına alındığı yer.
- Docker Client: Deamon ile iletişim kuran komut satırı aracı.
- Docker Deamon: Container'ların inşa edilmesi, çalıştırılması, dağıtılması gibi operasyonları üstlenen arka plan servisi.
- Image: Uygulamalar için gerekli konfigurasyon ve dosya sistemi ayarlarını taşıyan ve Container'ların oluşturulması için kullanılan nesneler. Docker dünyasında base,child,official ve user tipinden imajlar bulunuyor. base tipindekiler tahmin edileceği üzere linux,macos,windows gibi OS imajları.child imajlar base'lerden türetilip zenginleştiriliyor. Docker'ın official imajları (pyhton, alpine, nginx vb) dışında kullanıcıların depoya aldığı docleağrulanmış imajlarda söz konusu.
- Container: Image'ların çalışan birer nesne örneği. Bir Container çalışan uygulama için gerekli tüm bağımlılıkları bünyesinde barındırır. Kendi çekirdeğini (Kernel) diğer Container'lar ile de paylaşır. Tamamen izole edilmiş process üzerinde çalışır.

Docker'ın Kurulumu

Kurulum işlemlerinde halen tam olarak anlamadığım adımlar olsa da benim için önemli olan West-World'e Docker'ın başarılı bir şekilde yüklenmesiydi. Aşağıdaki adımları izleyerek bu işlemi gerçekleştirdim.

Her ihtimale karşı işe başlarken paket indeksini güncellemek gerekiyor.

```bash
sudo apt-get update
```

Sonrasında https üzerinden repository kullanımı için gerekli paket ilavelerinin yapılması lazım (Ben önceden yapmışım o yüzden yeni bir şey eklemedi)

```bash
sudo apt-get install apt-transport-https ca-certificates curl software-properties-common
```

Bu işlerin ardından Docker'ın (GNU Privacy Guard-gpg) anahtarının sisteme eklenmesi gerekiyor.

```bash
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
```

Docker dökümanına göre anahtarın kontrol edilmesinde yarar var. Harfiyen dediklerine uyuyorum ve FingerPrint bilgisinin son 8 değerine bakıyorum.

```bash
sudo apt-key fingerprint 0EBFCD88
```

Bilgi doğrulanıyor. Artık Repository'yi ekleyebilirim. West-World, Ubuntu'nun Xenail türevli işletim sistemine sahip. Bu sebeple uygun repo seçimi yapılmalı. Siz sisteminiz için uygun olan sürümü yüklemelisiniz (Diğer yandan production ortamlarında sürüm numarası belirterek de yükleme yapılabiliyor)

```bash
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
```

Kurulum öncesi yine paket indeksini güncellemekte yarar var. Nitekim docker repository paketleri yüklenecek.

```bash
sudo apt-get update
```

Bu ön hazırlıklardan sonra docker'ın kurulumuna başladım. Aşağıdaki komut ile sisteme Community Edition sürümü yüklendi.

```bash
sudo apt-get install docker-ce
```

Kurulumdan emin olmanın yolu standart bir imajı test etmekten geçiyor. Bunun için hello-world imajını kullanmamız öneriliyor.

```bash
sudo docker run hello-world
```

Tabii Docker yeni kurulduğu için hello-world imajı sistemde bulunmuyor. Bu yüzden run komutu sonrası ilgili paketin son sürümü indirilecek ve sonrasında da çalıştırılacaktır. "Hello from Docker!" cümlesini görmek yeterli.

![core_docker_5.gif](/assets/images/2017/core_docker_5.gif)

Eğer adımlara dikkat edilecek olursa yukarıdaki çalışma şeklinde bahsedilen işlemlerin yapıldığı da görülebilir. İlk olarak Docker Client, Docker Deamon'a bağlanıyor. Sonrasında Deamon, hello-world imajını Hub'dan çekiyor ([Hub'da sayısız imaj olduğunu belirtelim](https://hub.docker.com/explore/)) İmaj çekildikten sonra bir Container oluşturulup çalıştırılıyor. Sonuçlar da istemciye sunuluyor. Artık West-World'de Docker kurulmuş vaziyette. Terminalde docker kullanımı ile ilgili daha fazla bili almak için --help anahtarını da kullanabiliriz.

```bash
docker --help

docker pull --help
```

gibi

Basit Bir.Net Core Console Application

Docker üzerinde host etmek için deneme amaçlı bir Console uygulaması yazarak devam etmeliyim. Terminalden aşağıdaki komutu kullanarak şanslı sayı üretmesini planladığım uygulamayı oluşturdum.

```bash
dotnet new console -o LuckyNum
```

Ardından Program.cs içeriğini aşağıdaki gibi değiştirdim.

```csharp
using System;

namespace LuckyNum
{
    class Program
    {
        static void Main(string[] args)
        {
            Random randomizer=new Random();
            var num=randomizer.Next(1,100);
            Console.WriteLine("Merhaba\nBugünkü şanslı numaran\n{0}",num);
        }
    }
}
```

Program çalıştırıldığında bizim için rastgele bir sayı üretiyor. İçeriği aslında çok da önemli değil. Amacım uygulamayı Docker üzerinden yürütmek. İlerlemeden önce programın çalıştığından emin olmakta yarar var tabii.

![core_docker_6.gif](/assets/images/2017/core_docker_6.gif)

Sıradaki adımsa uygulamanın publish edilmesi. Terminalden aşağıdaki komutu kullanarak bu işlem gerçekleştirilebilir.

```bash
dotnet publish
```

Sonuçta LuckyNum.dll ve diğer gerekli dosyalar bin/debug/netcoreapp2.0/publish klasörü altına gelmiş olmalı.

![core_docker_7.gif](/assets/images/2017/core_docker_7.gif)

Console Uygulamasını Docker'a Almak

Nihayet son adıma geldim. Kodların olduğu klasöre gidip Dockerfile isimli bir dosya oluşturmak gerekiyor (Uzantısı olmayan bir dosya. DockerFile gibi değil Dockerfile şeklinde olmalı. Nitekim docker bunu ele alırken Case-sensitive hassasiyeti gösterdi. Epey bir deneme yapmak zorunda kaldım) Dosya içerisinde bir takım komutlar olacak. Bu komutlar aslında Linux temelli.

```text
FROM microsoft/dotnet:2.0-sdk
WORKDIR /app

COPY /bin/Debug/netcoreapp2.0/publish/ .

ENTRYPOINT ["dotnet", "LuckyNum.dll"]
```

Bütün Dockerfile içerikleri mutlaka FROM komutu ile başlar. Burada base image bilgisini veriyoruz ki örnekte bu microsoft hesabına ait dotnet:2.0-sdk oluyor. Dosyayı oluşturduktan sonra bir build işlemi gerçekleştirmek ve imajı inşa etmek lazım.

```text
sudo docker build -t lucky .
```

Bu komutla Deamon Hub üzerinden microsoft/dotnet:2.0-sdk imajı indirilmeye başlanacak. Dockerfile içerisindeki ilk satırda bunu ifade ediyoruz. Sonrasında basit dosya kopyalama işlemi yapılacak ve çalışam zamanındaki giriş noktası gösterilecek.

> DotNet Tarafı için kullanılabilecek Docker Image listesine [buradan](https://hub.docker.com/r/microsoft/dotnet/) ulaşabilirsiniz. Hem Linux hem Windows Container'ları için gerekli bilgiler yer alıyor.

Artık elimde lucky isimli bir imaj var. Bu imajı doğrudan çalıştırabileceğimiz gibi bu imajdan başka bir tane çıkartıp onu da yürütebiliriz. Aşağıdaki kodda bu işlem gerçekleştirilmekte. Tabii luckynumber'ın kalıcı olması için commit işlemi uygulanması da gerekebilir. Docker'ın komutları ve neler yapılabileceği şimdilik yazının dışında kalıyor ama ara ara bakmaya çalışacağım.

> Bu arada oluşturulan imajları isterseniz cloud.docker.com adresinden kendi hesabınızla da ilişkilendirebilirsiniz. [Şu adresteki](https://github.com/docker/labs/blob/master/beginner/chapters/webapps.md) python örneğini adım adım yapın derim;)

```text
sudo docker run --name luckynumber lucky
```

![core_docker_8.gif](/assets/images/2017/core_docker_8.gif)

Ben yazıyı hazırlarken bir kaç deneme yaptığım için Docker build işleminin çıktısı sizinkinden farklı olabilir. Nitekim dotnetcore imajının indirilmesi ile ilgili adımlar da yer almaktaydı. Sisteme yüklü olan imajların listesini de görebiliriz. Hatta kaldırmak istediklerimiz olursa rm veya rmi komutlarını da kullanabiliriz. Bunlar örneğe çalışırken işime yarayan komutlardı.

> Docker'ın çalışma prensiplerini daha iyi kavramak ve örneklerle uygulamalı olarak onun felsefesini anlamak için [GitHub üzerindeki şu adrese](https://github.com/docker/labs/blob/master/beginner/chapters/alpine.md) uğramanızı tavsiye ederim.

West-World şimdi biraz daha mutlu. Çünkü.Net Core 2.0 ile yazılmış bir programı dockerize etmenin nasıl bir şey olduğunu öğrendi. Ben de tabii. Elbette docker'ın gücünü anlamak için farklı açılardan da bakmak gerekli. Söz gelimi official imajlardan olan python'un çekip üretilen container üzerinde doğrudan python ile kodlama yapmaya başlayabiliriz. Aşağıdaki ekran görüntüsüne dikkat edin. Sistem python yüklememize gerek yok. pyhton çalışmak için gerekli herşeyin yer aldığı bir imajı çekip başlatılan container üzerinden kodlama yapabiliriz.

![core_docker_11.gif](/assets/images/2017/core_docker_11.gif)

Docker,.Net Core gibi konular önümüzdeki yıllarda geliştiricilerin iyi şekilde hakim olması gereken konular arasında yer alıyor. Vakit ayırıp planlı bir şekilde çalışmak lazım. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.