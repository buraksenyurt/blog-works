---
title: "Apache Üzerinden Bir Web API Hizmeti Sunmak"
pubDate: 2018-03-22 03:13:00
categories:
  - .Net Core
tags:
  - .Net Core
  - Bash
  - XML
  - C#
  - .NET
  - Web API
  - HTTP
---

# Apache Üzerinden Bir Web API Hizmeti Sunmak
![apachecore_12.gif](images/apachecore_12.gif)

Merhaba Arkadaşlar,

Küçük bir çocukken her pazar sabahı TRT ekranlarında kovboy (Western) filmleri olurdu. Rahmetli babam ile severek geçirdiğimiz nadide vakitlerdendi. Sanıyorum son yıllarda yine Pazar sabahları ekranlarımızı süslüyorlar (Bakamıyorum çünkü sabahlarımız basketbol antrenmanları ile geçiyor) Kızılderililer ile süvarilerin sıklıkla karşı karşıya geldiği, batının en hızlı silahşörlerinin yer aldığı filmlere ne hikmetse çok bağlanmıştım.

Oysa ki okuduğumuz tarih kitapları o zamanlardaki olayların pek de filmlerde gördüğümüz gibi olmadığını yazıyor. Özellikle de kızılderili yerlilerin durumu düşünüldüğünde. O zamanların pek çok yerli kabilesi günümüzün pek çok teknolojisine de isim kaynağı oldu aslında. Bunlardan en popüler olanlarından birisi de sanıyorum ki Apache'dir (Şu isimler de eminim ki çağrışım yapacaktır;Siu, Cheyenn, Comanche...) Ünlü savaş helikopteri dışında hepimiz onun yazılım dünyasındaki ününü de duymuşuzdur. Gelelim bugün inceleyeceğimiz konuya.

Geçenlerde bir süredir West-World üzerinde denemek istediğim vakaların üstünden geçtim. Gerek iş değişikliği gerek Salı,Perşembe günleri veteranlar olarak yaptığımız basketbol maçlarının yoğunluğu sebebiyle kuyrukta baya bir araştırma konusu birikmiş durumda. Listeden sırayla geçerken kafada hemen bir ağırlıklandırma yaptım (Scrum içimize işlemiş) En yüksek öncelik puanı "Apache üzerinde bir.Net Core uygulamasının nasıl host ederim?" cümlesine aitti. Microsoft'un ilgili dokümanında CentOS tabanlı sitemler için bir anlatım bulunuyordu ki bu benim Ubuntu 16.04 dünyasında farklı şekilde ele alınabilirdi (ki öylede oldu) Gelin vakit kaybetmeden bu haftasonu West-World üzerinde neler olmuş bir bakalım.

Öncelikle sisteme Apache kurulumunu gerçekleştirdim. Konum ile çok alakalı olmasa da Firewall ayarlarını yapıp, Apache'nin belirli komutlarını inceleyerek devam ettim. Ardından Apache üzerinde yönlendirme (Redirect) gerçekleştirilebilmesi için konfigurasyon ayarlamaları yaptım. Standard Web API servisini geliştirip aynen NGinX odaklı yaklaşımda olduğu gibi sisteme bir Service dosyası atıp testleri gerçekleştirdim. İlk adımla başlayalım.

Apache Kurulumu

Kurulum için standart olarak öncelikle bir sistem güncellemesi yapmak, ardından da apache2'yi yüklemek doğru olacaktı. West-World dünyasında her şeyin başı apt-get update idi.

```bash
sudo apt-get update
sudo apt-get install apache2
```

Yükleme sonrası apache'nin kullanılabileceği bir kaç profil, firewall listesine ilave edilmekte (edilirmiş) Benim makinemdeki UFW (Uncomplicated Firewall) listesini çektiğimde aşağıdaki sonuçlarla karşılaştım.

```bash
sudo ufw app list
```

Bu arada CUPS, Common Unix Printing Systems isimli bir servisin kısaltmasıymış. Bunu da öğrenmiş oldum.

![apachecore_1.gif](images/apachecore_1.gif)

Önceden NginX ile ilgili denemeler de yaptığım için onlar da listede kendine yer bulmuştu (West-World bir süre sonra çarpık kentleşme nedeniyle tekrardan yapılandırılmalı sanıyorum ki) Apache için üç farklı profil söz konusu. Apache isimli profil sadece 80 portunun açık olduğu ve şifresiz bir trafik imkanı sunmakta. Apache Full, Apache profiline ek olarak 443 nolu porttan şifrelenmiş web trafiğine imkan tanır (Yani TLS/SSL desteği verir) Apache Secure ise sadece 443 portu kullanılacak şekilde şifrelenmiş web trafiği sağlar. Benim örneğim için 80 portunu kullandırmak yeterliydi. Bu nedenle aşağıdaki komutu kullanarak gerekli etkinleştirmeyi yaptım.

```bash
sudo ufw allow 'Apache'
```

Güncel duruma baktığımda ilgili tanımın UFW listesine eklendiğini de gördüm.

```bash
sudo ufw status
```

![apachecore_2.gif](images/apachecore_2.gif)

Bu işlemler sonrasında yapmam gereken apache sunucusunun ayağa kalkıp kalkmadığını denetlemekti. Terminalden

```bash
sudo systemctl status apache2
```

komutunu kullandığımda servisin başarılı bir şekilde yüklendiğini ve hizmet vermeye başladığını gördüm.

![apachecore_3.gif](images/apachecore_3.gif)

active (running) yazısını görmek önemli ama yeterli değil. Hani hepimizin aşina olduğu o Apache'nin varsayılan giriş sayfası var ya...Onu görmek lazımdı. localhost'a talep gönderdiğimde o yalın sayfa karşımdaydı.

![apachecore_4.gif](images/apachecore_4.gif)

## Birkaç Apache Komutu

Apache sunucusu ile çalışırken elbette bir takım yönetimsel işlemlere gereksinim duyulabilir. Sunucuyu durdurmak, yeniden başlatmak, konfigurasyon değişikliklerini tekrardan yüklemek ve diğerleri. Bu işlemler için aşağıdaki komutlardan yararlanılabilir.

Durdurmak için,

```bash
sudo systemctl stop apache2
```

![apachecore_5.gif](images/apachecore_5.gif)

Başlatmak için,

```bash
sudo systemctl start apache2
```

![apachecore_6.gif](images/apachecore_6.gif)

Servisi durdurup tekrar başlatmak için,

```bash
sudo systemctl restart apache2
```

Konfigurasyon değişikliklerini bağlantıyı kopartmadan yüklemek içinse

```bash
sudo systemctl reload apache2
```

ModProxy Özelliğini Etkinleştiriyoruz

Artık West-World'de gezinen bir Apache olduğuna göre basit bir.Net Core Web API hizmeti yazıp bunu apache üzerinden host etmeyi deneyebilirdim. Aslında olay NginX senaryosundakine çok benziyor. Apache'yi reverse proxy server rolünde çalışacak hale getirip localhost 80 portuna gelen taleplerin kestrel'e yönlendirilmesini sağlamak işin anafikri diyebiliriz (Tam tersi istikamette söz konusu tabii) İlk adım ise modproxy'yi etkinleştirmek. Bu sayede apache sunucumu HTTP taleplerini yönlendirecek kıvama getirebileceğim. Bunun için terminalden aşağıdaki komutu vermek gerekiyor.

```bash
sudo a2enmod proxy proxy_http proxy_html
```

![apachecore_8.gif](images/apachecore_8.gif)

Gerekli aktivasyonu sağladıktan sonra konfigurasyon değişikliği yapıp yönlendirme tanımlarını sisteme ilave ettim. Söz konusu konfigurasyon değişiklikleri için env/apache2/sites-enabled altındaki 000-default.conf dosyasının içeriğini düzenlemek gerekiyor. Bu varsayılan site dosyası. Aslında bu klasöre farklı sanal host tanımlama bilgileri içeren conf uzantılı dosyalar yükleyebiliyoruz. Bu dosyalar apache sunucusu tarafından otomatik olarak değerlendirilmekte. Örneğin merak ettiğim konulardan birisi localhost'un farklı bir portu için sanal host konfigurasyon dosyası tanımlamak ve yine yönlendirmeler yaparak Kestrel'i ayağa kaldırmak (Bunu bir araştırmam lazım)

```xml
<VirtualHost *:80>
ServerAdmin webmaster@localhost
DocumentRoot /var/www/html

ProxyPreserveHost On
ProxyPass / http://localhost:5558/
ProxyPassReverse / http://localhost:5558/
ErrorLog ${APACHE_LOG_DIR}/error.log
CustomLog ${APACHE_LOG_DIR}/access.log combined
</VirtualHost>

# vim: syntax=apache ts=4 sw=4 sts=4 sr noet
```

Tahmin edileceği üzere bir sanal host tanımı var. 80 portu ile Kestrel'in standart 5000 yerine bu örnek özelinde tercih ettiğim 5558 portu arasında reverse proxy kullanımı olacağı belirtiliyor. Bu değişiklikten sonra apache servisini tekrardan başlattım (restart) ve güncel durum bilgisini çektim.

```bash
sudo service apache2 restart
sudo service apache2 status
```

Şimdilik bir sorun görünmüyordu.

![apachecore_7.gif](images/apachecore_7.gif)

Web API ve Apache Service Dosyasının Oluşturulması

Tüm bunlar yeterli değil. Apache'nin bu konfigurasyon dosyası bilgisine göre, 80 portuna gelen talep için Kestrel çalışma zamanını da nasıl işleteceğini bilmesi lazım. Bir service dosyası oluşturmalı ve içerisinde gerekli ortam bilgilendirmelerini belirtmeliyiz. Ama her şeyden önce bir Web API projesi oluştursam hiç fena olmazdı. Bunun yolunu artık siz de benim kadar iyi biliyorsunuz.

```bash
dotnet new webapi -o apacheler
```

Kodda yaptığım tek değişiklik sunucu adresini ayarlamak oldu. Program.cs içerisindeki BuilWebHost fonksiyonunu aşağıdaki gibi düzenledim.

```csharp
public static IWebHost BuildWebHost(string[] args) =>
WebHost.CreateDefaultBuilder(args)
.UseStartup<Startup>()
.UseUrls("http://localhost:5558")
.Build();
```

Kod düzenlemesi sonrası projeyi publish ettim.

```bash
dotnet publish -c Release
```

Apaçiler'in kültürel özelliklerine ait bir çok sırrı sunacağını hayal ettiğim web api servisimiz için /etc/systemd/system klasörü altında kestrel-apacheler.service isimli bir dosya oluşturup içeriğini aşağıdaki hale getirdim.

```text
[Unit]
Description=Apaçiler hakkında gizemli bilgiler sunan web api

[Service]
WorkingDirectory=/home/burakselyum/dotnetcore/apacheler
ExecStart=/usr/share/dotnet/dotnet /home/burakselyum/dotnetcore/apacheler/bin/Release/netcoreapp2.0/publish/apacheler.dll
Restart=always
RestartSec=30
SyslogIdentifier=apachelerlog
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

Dosyanın üç parçası bulunuyor. Unit kısmında servise ait bir açıklamaya yer verilmekte. Servisin çalışacağı klasör WorkingDirectory bilgisi ile belirtilmekte. ExecStart, dotnet.exe'yi (West World için geçerli olan bu lokasyon sizin ortamınızda farklı olabilir) kullanarak publish edilen apacheler hizmetini başlatmakta. Bir nevi dotnet start işlemini gerçekleştirdiğini ifade edebiliriz. Hata olması halinde her zaman restart işlemi uygulanacağını da belirtiyoruz. Buradaki dayanma süremiz de 30 saniye. Servis hizmete girdikten sonra sistemden log'ları incelemek isteyebiliriz. SyslogIdentifier'a atanan apachelerlog kelimesi ile bu içerikleri daha kolay ayırt etme şansımız var. Kullanıcı olarak ben root'a yetki verdim ama bunu alana özel bir kullanıcı bazında kullandırmak daha doğru olabilir.

Testler

Servis dosyası artık hazır. Şimdi bunu etkinleştirip devreye almak gerekiyor. Devreye aldıktan sonra da servis durumunu gözlemlemekte yarar var. Terminalden systemctl komutu kullanılarak bu işlemler gerçekleştirilebilir. Önce etkinleştir (enable), sonra başlat (start) ve güncel durumu izle (status).

```bash
sudo systemctl enable kestrel-apacheler.service
sudo systemctl start kestrel-apacheler.service
sudo systemctl status kestrel-apacheler.service
```

![apachecore_9.gif](images/apachecore_9.gif)

Gözlemlediğim kadarı ile kestrel-apacheler.service içeriği geçerliydi ve çalışır konumdaydı. Bunu gördükten sonra Firefox'tan http://localhost/api/values adresine gitmeyi denedim. Son ayların en popüler değer listesine erişebilmiştim.

![apachecore_10.gif](images/apachecore_10.gif)

Gözlerime inanamıyordum. Emin olmak için onları bir kaç kez kırptım. Sonrasında daha gerçekçi olmaya karar verdim ve service dosyasını durdurup aynı talebi tekrar gönderdim.

```bash
sudo systemctl stop kestrel-apacheler.service
```

![apachecore_11.gif](images/apachecore_11.gif)

Bu çok sevindirici bir gelişmeydi (İnsanın Service Unavailable yazısını görünce sevinçten gözleri yaşarı mı?) West-World'de Apache'ler ile barış sağlandığına göre artık dinlenmeye çekilebilirdim. Tabii siz bu yazıdan ilham alarak konuyu geliştirmeyi deneyebilirsiniz. Söz gelimi 000-default.conf yerine aynı klasörde farklı bir conf dosyasını kullanarak ilgili yönlendirmenin nasıl yapılabileceğini araştırabilirsiniz. Özellikle 80 yerine farklı bir Apache portu kullandırtmayı deneyebilirsiniz. Böylece geldik bir makalemizin daha sonuna. Bu yazımızda Ubuntu 16.04 üzerinde kurduğumuz apache sunucusuna gelen talepleri, Kestrel tarafında host edilen bir Web API hizmetine yönlendirmeye çalıştık. Umarım yararlı bir makale olmuştur. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
