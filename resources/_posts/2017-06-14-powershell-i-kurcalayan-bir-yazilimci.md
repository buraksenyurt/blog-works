---
layout: post
title: "Powershell'i Kurcalayan Bir Yazılımcı"
date: 2017-06-14 09:27:00 +0300
categories:
  - powershell
tags:
  - powershell
  - dotnet
  - http
---
Powershell betikleri uzun zamandır yaygın ve etkin bir şekilde Windows ailesinde yer almakta. Özellikle sunucu tarafındaki kullanımının öne çıktığını görüyoruz. Temel olarak işletim sistemi üzerinden betik dillerin avantajlarını da kullanarak pek çok işlemin yapılabilmesine olanak sağlayan bir kabuk olarak düşünülebilir. Powershell.Net Framework kütüphanelerini de doğrudan kullanabildiğinden önemli avantajlar sağlamaktadır. Klasör listeleme veya dosya kopyalama gibi çok basit işlemler dışında, n sayıda sunucuya dağıtım paketi çıkacak programcıkların geliştirilmesi benzeri operasyonları da içeren geniş bir yetkinliğe sahiptir. DevOps kültüründe Windows ailesi için değerli ve öne çıkan bir programlama ortamıdır diyebiliriz.

![powershell_1.gif](/assets/images/2017/powershell_1.gif)

Sistem yöneticilerinin biraz programlama bilgisi ile veya programcıların da biraz sistem bilgisi ile bu dili temel seviyede kullanmaları mümkündür. Elimizdeki Windows 7'den son sürüm Windows Server ailesine kadar pek çok sistemde Powershell betiklerini kullanabiliriz. Geçtiğimiz günlerde bir boşluk yakaladım ve en çok tavsiye edilen Powershell betikleri nelerdir araştırayım dedim. Uzman olmadığım bir alan olsa da tanıma mahiyetinde neler yapılabileceğini görmek istiyordum. Daha önceden Powershell ile üzerinde Gacutil olmayan bir makinede.Net Framework 1.1 ile yazılmış bir kütüphanenin register edilmesi ve eğlence amaçlı olarak Star Wars marşının çaldırılması gibi işlemler yapmıştım ama çok çok daha fazlası olduğunu biliyordum.

Makinemdeki Servisler

İlk olarak sistemde yüklü servislerin bir listesini nasıl alırım sorusunun cevabını aradım. Böylece işletim sisteminin yönetsel alanlarından birisine ait bilgilere nasıl ulaşabileceğimi görecektim. Komut satırından Powershell'i açtım ve ilk betiği aşağıdaki gibi yazdım.

```text
Get-Service
```

Komutun çıktısı aşağıdaki ekran görüntüsündeki gibi olmuştu.

![Powershell_2.gif](/assets/images/2017/Powershell_2.gif)

Makinemde yüklü olan tüm servislerin bilgisini görebiliyordum. Adları (Name), kısa açıklamaları (Display Name) ve o anki durumları (Status). Liste gözüme kalabalık görününce acaba bir sorgu atabilir miyim diye kurcalamaya başladım. Belki de adı C harfi ile başlayan servislerin listesini de çekebilirdim. Araştırınca get-service fonksiyonunu aşağıdaki gibi kullanabileceğimi öğrendim.

```text
Get-Service -name C*
```

![Powershell_3.gif](/assets/images/2017/Powershell_3.gif)

Artık elimde adının ilk harfi C olan servislerin bir listesi vardı. Asteriks kullanımı söz konusu olduğuna göre farklı benzerlikleri de sorgulayabilirdik. Örneğin içinde.NET geçen servisleri bulmak mümkündü. Ne var ki, durum bilgisine göre servisleri bir türlü sorgulayamıyordum. Aynen name parametresi gibi status parametresi üzerinden de sorgu atabileceğimi düşünmüştüm.

```text
get-service -status "running"
```

Ancak çalışma sonrası aşağıdaki hata mesajını aldım. status parametresi desteklenmiyordu.

![Powershell_4.gif](/assets/images/2017/Powershell_4.gif)

Yardım Lazımdı

Peki get-service fonksiyonunun nasıl kullanıldığının bir dokümanı yok muydu? Öğrendim ki get-help komutu ile bir başka powershell komutunun nasıl çalıştığını öğrenebilirim.

```text
get-help get-service
```

Bu komutla get-service kullanımına ait yardım içeriğine ulaştım. Komutun ne işe yaradığı, yazım biçiminin nasıl olması gerektiği, parametreleri, genel açıklması ve hatta örnekler sunan yardımcı internet adresli bile vardı.

```text
PS C:\Users\buraksenyurt> get-help get-service

NAME
    Get-Service

SYNOPSIS
    Gets the services on a local or remote computer.

SYNTAX
    Get-Service [[-Name] <string[]>] [-ComputerName <string[]>] [-DependentServices] [-Exclude <string[]>] [-Include <s
    tring[]>] [-RequiredServices] [<CommonParameters>]

    Get-Service -DisplayName <string[]> [-ComputerName <string[]>] [-DependentServices] [-Exclude <string[]>] [-Include
     <string[]>] [-RequiredServices] [<CommonParameters>]

    Get-Service [-InputObject <ServiceController[]>] [-ComputerName <string[]>] [-DependentServices] [-Exclude <string[
    ]>] [-Include <string[]>] [-RequiredServices] [<CommonParameters>]

DESCRIPTION
    The Get-Service cmdlet gets objects that represent the services on a local computer or on a remote computer, includ
    ing running and stopped services.

    You can direct Get-Service to get only particular services by specifying the service name or display name of the se
    rvices, or you can pipe service objects to Get-Service.

RELATED LINKS
    Online version: http://go.microsoft.com/fwlink/?LinkID=113332
    Start-Service
    Stop-Service
    Restart-Service
    Resume-Service
    Suspend-Service
    Set-Service
    New-Service

REMARKS
    To see the examples, type: "get-help Get-Service -examples".
    For more information, type: "get-help Get-Service -detailed".
    For technical information, type: "get-help Get-Service -full".

PS C:\Users\buraksenyurt>
```

Where ile Sorguyu Genişlettim

Gerçekten de get-service komutunun -status gibi bir parametresi mevcut değildi ancak -InputObject parametre kullanımı dikkat çekiyordu. Biraz araştırmadan sonra aşağıdaki gibi bir komut ile DisplayName bilgisinde "Service" kelimesi geçenler hizmetlerden durmuş olanları alabileceğimi gördüm.

```text
Get-Service * | Where-Object {$_.DisplayName -like "*Service*" -and $_.Status -eq "Stopped"}
```

![Powershell_5.gif](/assets/images/2017/Powershell_5.gif)

Bu komutu anlamak oldukça kolaydı. * ile Get-Service'in döndüreceği tüm servis listesi üzerinde işlem yapacağımızı belirmiştik. | arkasından bir where koşulu geliyordu. Süslü parantezler belli ki bu komutun alacağı kod bloğunu taşıyacaktı. $_. ile başlayan değişkenler ile DisplayName ve az önce alamadığımız Status alanlarına ulaşabiliyorduk. -like benzer, -and mantıksal ve, -eq ise eşitlik anlamında kullanılan komut parametreleriydi (Bu durumda büyüktür veya küçüktürü hatta veyayı nasıl ifade edebileceğimizi de çözmüş oluyoruz)

Döngü Kullanımını Merak Edince

Internet kaynaklarını tararken dilin pek çok diğer dilde olan temel özelliklere sahip olduğunu görmek kaçınılmazdı. Değişken tanımlamaları, koşullu ifadeler, döngüler vb Hazır elim değmişken bir de döngü yazayım istedim. Örneğin şu an makinede çalışmakta olan Process'lere bakıp Id ve Name bilgilerini ekrana bastırmaya çalıştım.

```text
Get-Process| ForEach-Object{[string]::Format("{0} - {1}",$_.id,$_.name)}
```

![Powershell_6.gif](/assets/images/2017/Powershell_6.gif)

Burada dikkat çekici noktalardan birisi de.Net kütüphanesinden bir fonksiyona erişilmesiydi. String sınıfının Format metodunu kullandığımız gözünüzden kaçmamış olmalı. [Sınıf Adı]::[Metod Adı] notasyonu ile static tanımlanmış üyelere erişmek mümkün. ForEach-Object komutu ise Get-Process'in ürettiği her bir process nesnesini dolaşmakta. Döngü içerisinde o anki Process'in id ve name bilgilerine nasıl eriştiğimize dikkat edelim.

Çıktıları Dosyaya Basalım

Yaptığımız denemelerin sonuçları hep komut satırına basıldı. Anlık olarak iyi olsa da bazen bu çıktıları bir dosyaya basmak ve bir yerlere göndermek de isteyebiliriz (Hatta inanıyorum ki bir powershell işlem çıktısının ürettiği dosyayı otomatik mail ile bir yerlere yollayabilir veya ortak bir ağ klasörüne kopyalatabiliriz. Bir deneyin) Bunun için out-file parametresini eklemek yeterli.

```text
Get-Service * | Where-Object {$_.DisplayName -like "*Service*" -and $_.Status -eq "Stopped"} | out-file "ServiceReport.txt"
```

![Powershell_7.gif](/assets/images/2017/Powershell_7.gif)

Bunları Fonksiyon Haline Getiremez miydim?

Servisler için uyguladığım betiğin çıktısını metin tabanlı bir dosyaya basmış oldum. Pratik görünüyor değil mi? Aslında tam olarak değil. Betikleri fonksiyonelleştirmek yeniden kullanılabilirlik anlamında çok daha mantıklı olabilir. Örneğin servis raporunu çekerken like, status ve dosya kriterlerini parametrik olarak bir fonksiyona almak çok daha işe yarardı. Sonunda aşağıdaki gibi bir şeyler yazılabileceğini gördüm.

```text
function Service-Report ($name="A*",$status="Running",$fileName="Service_Report.txt")
{
	Get-Service * | Where-Object {$_.Name -like $name -and $_.Status -eq $status} | out-file $fileName
}
```

Service-Report bir fonksiyon olarak 3 parametre ile çalışmakta. $ işareti ile tanımlanan parametreler metod bloğunda aynı şekilde bir değişken olarak ele alınıyorlar. Dikkat çekici nokta parametrelere varsayılan değer verilebilmesi. Yani name, status ve fileName bilgileri boş geçilirse varsayılan değerleri kullanılacak. Komut satırından bu fonksiyonu çalıştırmak da oldukça kolay.

```text
Service-Report "C*" "Stopped" "Reports.txt"
```

Reports.txt dosyasının içinde artık adı C harfi ile başlayan durdurulmuş servislerin bir çıktısı yer alıyor.

![Powershell_8.gif](/assets/images/2017/Powershell_8.gif)

ISE Diye Sevimli Bir IDE

Bu işlerle uğraşırken komut satırının hem sevimli hem de sevimsiz olduğunu fark ettim. Terminalden çalışmak her zaman için daha keyifliydi ama iş bir betik hazırlamaya geldiğinde basit bir editör hayat kurtarabilirdi. En azından Notepad++ düşünülebilirdi. Çok daha iyisini buldum. Powershell'in ISE (Integrated Scripting Environment) adında bir de kod editörü varmış. Bunun üzerine ISE editörünü açıp içerisine iki fonksiyonellik katıp tools.ps1 adıyla bulunduğum klasöre kayıt ettim.

```text
function Service-Report ($name="A*",$status="Running",$fileName="Service_Report.txt")
{
	Get-Service * | Where-Object {$_.Name -like $name -and $_.Status -eq $status} | out-file $fileName
}

function Process-Report($name,$cpuRate,$fileName="Process.txt")
{
    Get-Process | where-object {$_.name -eq $name -and $_.CPU -gt $cpuRate} |Format-List * -Force | out-file $fileName
}
```

Artık fonksiyonellikleri bir betik dosyası içerisine konumlandırıp komut satırından bu şekilde çalıştırabilirdim. ISE yetenekli bir editör. Betiği yazabildiğimiz arayüzü dışında yine kendi üstünde yer alan komut satırı ile çalışmaların anında denenmesi mümkün. Üstelik çağırılan komutların geriye doğru log'unun tutulduğu bir penceresi de mevcut.

> Bu arada bir ps1 dosyasının yüklenmesi sırasında "...cannot be loaded because the execution of scripts is disabled on this system" şeklinde bir hata alınıyorsa ExecutionPolicy değerinin RemoteSigned olarak değişitirilmesi çözüm olabilir.
> ![Powershell_9.gif](/assets/images/2017/Powershell_9.gif)

ISE editöründe tools.sp1 dosyası içerisindeki fonksiyonların kullanılabilmesi için öncelikde dosyanın sistem çalışma zamanına yüklenmesi gerekiyor. Bir program çalıştırıyor gibi F5 ile veya Debug menüsünden Run/Continue komutları sayesinde bu işlem gerçekleştirilebilir. Dosya başarılı bir şekilde yüklendikten sonra içerisindeki fonksiyonlar kullanılabilir hale gelir. Söz gelimi tools.ps1'i yükledikten sonra sistemde o an açık olan chrome sekmelerinden CPU tüketimi belli bir değerin üstünde olanları bulmak için aşağıdaki ekran görüntüsüne olduğu gibi ilerlememiz yeterlidir.

![Powershell_10.gif](/assets/images/2017/Powershell_10.gif)

Kimbilir Powershell betikleri ile daha neler neler yapılabilir? Mesela çok fazla bellek tüketen process'lerin belirli kriterlere uyanlarını öldürebilecek, durmuş bir servisi tekrardan ayağa kaldırabilecek betikler yazabilir, ağ üzerinden ulaşılabilecek uzak sunucular hakkında anlık durum bilgilerine sahibi olabiliriz. Sistem üzerinden gerçekleştirilecek tüm bu operasyonlarda sadece bu alana özel yazılmış bir betik dilin programlama özelliklerine sahip olmak da işin cabası.

Bu yazıdaki anafikre göre kısa zamanda öğrenilebilecek bir betik dil ile işletim sistemi üzerinde hakimiyet sağlayıp pek çok işi otomatize edecek geliştirmeler yapabileceğimizi düşünebiliriz. İşte böyle sevgili okuyucu. Bir boşluğu iyi değerlendirmek adına Powershell ile yaptığım bir kaç saatli maceralarımı kısaca anlatmaya çalıştım. İşi daha da ileri götürmek mümkün. Ara ara burada betikler yazılabilir. [Amazon'da konu ile ilgili pek çok kitap](https://www.amazon.com/s/ref=sr_st_date-desc-rank?keywords=powershell+scripting&rh=i%3Aaps%2Ck%3Apowershell+scripting&qid=1497268080&sort=date-desc-rank) var ama şuradaki [online tutorial seti](http://powershelltutorial.net/) de epey işe yarar görünüyor. Bir başka yazımızda görüşmek dileğiyle hepinize mutlu günler dilerim.
