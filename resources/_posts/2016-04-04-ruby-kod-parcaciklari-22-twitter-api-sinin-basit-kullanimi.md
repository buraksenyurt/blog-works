---
layout: post
title: "Ruby Kod Parçacıkları 22 - Twitter API'sinin Basit Kullanımı"
date: 2016-04-04 15:00:00 +0300
description: "Bazı günler şirketten geç çıkıyorum. Özellikle el ayak çekildiğinde ofis ortamı bir şeylere çalışmak için son derece ideal oluyor. Tam da istediğim ortam. Kendi çalışmalarıma vakit ayırabildiğim hoşuma giden zamanlar. Hele de akşam güneşi camdan içeri giriyor ve en sevdiğim sarı rengi masamın üstüne bırakıyorsa..."
categories:
  - ruby
tags:
  - ruby
  - dotnet
  - xml
  - rest
  - json
  - http
  - github
---
# Ruby Kod Parçacıkları 22 - Twitter API'sinin Basit Kullanımı
![rubytwt_7.gif](/assets/images/2016/rubytwt_7.gif)

Merhaba Arkadaşlar,

Bazı günler şirketten geç çıkıyorum. Özellikle el ayak çekildiğinde ofis ortamı bir şeylere çalışmak için son derece ideal oluyor. Tam da istediğim ortam. Kendi çalışmalarıma vakit ayırabildiğim hoşuma giden zamanlar. Hele de akşam güneşi camdan içeri giriyor ve en sevdiğim sarı rengi masamın üstüne bırakıyorsa.

İşte bu huzur ve sakinlik içerisinde bu akşam mesaiden sonra şöyle oturayım da iki satır Ruby kodu çalışayım dedim. Aklıma bir kaç gem kütüphanesini araştırıp kullanmaya çalışmak geldi. İlk gözüme kestirdiğim ise [twitter gem](http://www.rubydoc.info/gems/twitter) oldu. Bu gem'i kullanarak tweet'lerimi okumak ve hatta bir tweet'i ruby kodundan paylaşmak amacındaydım.

İlk yapmam gereken bir ara hasbelkader kurmayı başardığım Ruby ortamıma ilgili gem'i yüklemekti. Bu aslında.Net ortamına bir NuGet paketini indirmekten farksız bir işlemdi. Ne varki şirket güvenlik politikaları gereği ne Nuget ne de ruby gem sunucularına erişemiyordum. Tüm hevesim kursağımda kalmıştı. Başımı öne eğip çalışma odama geçmekten başka çarem yoktu. Ama yılmadım ve bu tweet'leri çekeceğim dedim. Azmettim ve sonunda bu yazı ortaya çıktı.

## Twitter Gem'in Kurulumu

İlk olarak Ruby komut satırından twitter kütüphanesini sistemime yükledim. Eğer sisteminizde ruby ve [ruby development kit](https://github.com/oneclick/rubyinstaller/wiki/Development-Kit) yüklü ise gem'lerin sorunsuz bir şekilde indirilip kurulabiliyor olması gerekir. Aynen aşağıdaki ekran görüntüsünde olduğu gibi.

![rubytwt_1.gif](/assets/images/2016/rubytwt_1.gif)

## Twitter API için Gerekli Kaydın Yapılması

Servis tabanlı çalışan pek çok API'de olduğu gibi Twitter'ın geliştiricilere açılan hizmetlerini kullanabilmek için öncelikle uygulama bazında kayıt yaptırmamız gerekir. Bunun için [https://apps.twitter.com/](https://apps.twitter.com/) adresine gidip yeni bir uygulama oluşturarak işe başlamalıyız. Ben gerekli girişleri yaparak bir deneme uygulaması oluşturdum.

![rubytwt_5.gif](/assets/images/2016/rubytwt_5.gif)

Uygulama oluşturulurken dikkat edilmesi gereken noktalardan birisi de Access Token'larının ürettirilmesi (Varsayılan olan bu token bilgileri oluşturulmuyor) Twitter'ın REST servislerinin kullanılabilmesi için consumer key, consumer secret, access token ve access token secret değerlerine ihtiyaç var. Bu değerler twitter kütüphanesi içerisinde yer alan istemci sınıfının oluşturulmasında kullanılacak.

## İlk Olarak Tweet'lerimi Çektim

İlk olarak kendime ait tweet'leri çekmek ve hatta bunları YAML formatlı bir içerik olarak kaydetmek istedim. Bunun üzerine aşağıdaki kod satırlarını geliştirerek işe başladım.

```text
require 'twitter'
require 'yaml'

client=Twitter::REST::Client.new do |config|
             config.consumer_key="."
             config.consumer_secret="."
             config.access_token=".."
             config.access_token_secret="."
end

# Bir kullanciya ait tweet'lerin cekilmesi ve kalici olarak dosyaya yazilmasi

myTweets=client.user_timeline('burakselyum',count:3)
myTweets.each{|t| puts "\n"+t.full_text}
File.write('LastTweets.yml',YAML.dump(myTweets))rr
```

Kod parçasında ilk olarak gerekli gem bildirimleri yapılıyor. ardından Twitter içerisindeki REST hizmetine ait bir nesne örneği oluşturuluyor. Nesne örneği oluşturulurken dikkat edilmesi gereken nokta ise verilen konfigurasyon parametre değerleri (Siz tabii ki kendi uygulamanıza ait key ve secret değerlerinizi kullanmalısınız) Sonrasında client örneği üzerinden usertimeline isimli fonksiyonu çağırmaktayız. Bu fonksiyona gelen ilk parametre tweet'leri alınacak kullanıcı hesabını işaret ediyor. İkinci parametre ile kaç tane tweet'ini alacağımızı ifade etmekteyiz. each metodunu kullanarak her bir tweet'in metinsel içeriğini fulltext niteliğini kullanarak komut satırına basmaktayız. Son olarak File sınıfının write metodunu çağırıyor ve tüm tweet içeriğini LastTweets.yml isimli dosyaya YAML formatında yazdırıyoruz.

> Kodu ilk denediğinizde aynı benim gibi bir SSL sertifika hatası alabilirsiniz. Bu, Windows işletim sisteminde ilgili sertifikanın yüklü olmayışından ve sisteme doğru path ile bildirilmeyişinden kaynaklanıyor olabilir. Çözüm için [bu adresteki adımları izleyebilirsiniz](https://superdevresources.com/ssl-error-ruby-gems-windows/). Şahsen ben öyle yaptım.

Sertifika hatasını aşınca ve Ruby kodunu çalıştırınca ta taaa...

![rubytwt_2.gif](/assets/images/2016/rubytwt_2.gif)

Görüldüğü üzere o gün girdiğim son üç tweet'imi başarılı bir şekilde alabildim. Hatta LastTweets.yml isimli dosya da başarıl bir şekilde oluştu.

![rubytwt_3.gif](/assets/images/2016/rubytwt_3.gif)

Aslında bu şekilde tweet'lerin tamamını fiziki olarak indirebiliriz. Dolayısıyla basit arşivleme yapabiliriz.

## ve Tabii İlk Tweet'imi Attım

Macera yeni yeni heyecan kazanıyordu. Sıradaki hedefim ise koddan bir Tweet gönderebilmekti. Bu da oldukça basit bir işlemdi. Tek yapılması gereken update metodunu aşağıdaki gibi kullanmaktı.

```text
client.update("Bu tweet, Twitter gem kullanilarak Ruby kodundan atilmistir.")
```

Kodu tekrar çalıştırdıktan sonra heyecanla twitter hesabımı açtım ve aşağıdaki sonucu gördüm.

![rubytwt_4.gif](/assets/images/2016/rubytwt_4.gif)

Vuhu huuuu:) client nesne örneği üzerinden çağırılan update metodu, twitter uygulamasının sahibi olan kişi adına parametre olarak gelen içeriği göndermişti.

## Ek Bilgi

Peki kayıt ettiğimiz YAML içeriğini nasıl okuyabiliriz? Bunun için aşağıdaki basit kod parçasını ele alabiliriz.

```text
loadedTweets=YAML.load_file('LastTweets.yml')
loadedTweets.each{|t| puts "\n"+t.full_text}
```

İlk olarak YAML sınıfı üzerinden loadfile metodunu çağırıyor ve parametre olarak gelen içeriğin loadedTweets değişkenine aktarılmasını sağlıyoruz. Hemen ardından yine bir each iterasyonuna gidiyor ve ilgili tweet içeriklerini ekrana basıyoruz.

İçeriğin YAML formatında tutulması elbette şart değil. Pekala JSON veya XML gibi popüler formatları da kullanabiliriz. Diğer yandan twitter REST API fonksiyonları update ve usertimeline operasyonlarından da ibaret değil. Yapılabilecek pek çok şey var. Daha fazla detay için [twitter'ın ilgili sitesine](https://dev.twitter.com/rest/public) bakmanızda yarar olacağı kanısındayım. Örneğin tweet'ler üzerinde arama yaptırabilir, popüler hashtag konularını çekebilir, istediğiniz arkadaş listesinin favori tweet'lerini alabilir, aradığınız bir konuya istinaden otomatik tweet'ler atabilir ve hatta çeşitli SEO çalışmaları kapsamında robot görevi üstelenen uygulamalar da yazabilirsiniz.

Böylece geldik bir Ruby kod parçacığımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.