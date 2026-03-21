---
layout: post
title: "Ruby Kod Parçacıkları 21 - Basit Web Server Geliştirmek"
date: 2016-02-22 14:00:00 +0300
categories:
  - ruby
tags:
  - tcpserver
  - socket
  - ruby-lang
  - socket-programming
  - http
  - http-header
  - get
  - tcp
  - iis
---
Web tabanlı çalışan uygulamalar genellikle IIS (Internet Information Services) ve benzeri ortamlar üzerinde barındırılırlar. Aslında IIS basitçe TCP soket haberleşmesi gerçekleştiren bir ürün olarak düşünülebilir. Üzerinde konuşlandırılan kaynaklara olan erişimde ise genellikle HTTP protokolü ve Get, Post vb metodları kullanılır. Gelen taleplere göre uygun servislerin devreye girmesi söz konusudur.

![web-server.gif](/assets/images/2016/web-server.gif)

İlgili taleplere (Request) verilecek cevaplar (Response) da bu hizmetler üzerinden karşılanır. Örneğin.Net çalışma zamanı aspx, svc, asmx, html gibi uzantılara gelecek talepleri ele alarak uygun cevapları döndürür. İstenirse handler'lar veya modul'ler özelleştirilerek farklı uzantılar için farklı çalışma şekilleri de düzenlenebilir. Ana fikirde ise evrensel bir standarda göre gelen mesajların irdelenip uygun içeriklerin döndürüldüğü sunucu uygulamaları söz konusudur.

Pek tabii kendi web sunucularımızı da geliştirebiliriz. İşin temel noktası TCP soket haberleşmesini uygulayabilmek ve istemci taleplerine doğru cevapları dönebilmektir. İşte bu yazımızda Ruby dilini kullanarak bir web sunucusunu geliştirmek için gerekli bilgilere merhaba demeye çalışacağız. İlk olarak IIS üzerinden host edilen bir web sayfasına gönderdiğimiz talebe karşılık nasıl bir HTTP haberleşmesi gerçekleştirildiğine bakalım.

> Bunu kolayca anlayabilmek için [Live HTTP Header](https://chrome.google.com/webstore/detail/live-http-headers/iaiioopjkcekapmldfgbebdclcnpgnlo) isimli bir Chrome eklentisine başvurdum. Malum şirket bilgisayarlarına Fiddler gibi gelişmiş dinleyiciler kurabilmek için bir çok prosedürü atlatmanız lazım. Ancak chrome eklentisi bir şekilde kurulabildi. Şşştt, kimseye söylemeyin.

IIS üzerinde host edilen Asp.Net ile geliştirilmiş WorksTodo isimli bir web uygulamamız olduğunu ve main.aspx isimli sayfasına tarayıcı üzerinden bir talepte bulunduğumuzu düşünelim. Bu durumda taraflar arasında aşağıdaki HTTP Header paketlerinin hareket ettiğini görebiliriz.

http://localhost/workstodo/main.aspx talebi için sunucuya aşağıdaki paket gider.

GET /workstodo/main.aspx HTTP/1.1
Host: localhost
Accept-Encoding: gzip, deflate, sdch
Accept-Language: en-US,en;q=0.8,cs;q=0.6,de;q=0.4,ja;q=0.2,tr;q=0.2
Cookie: SID={COOKIE BİLGİSİ KALDIRILDI}:
User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.157 Safari/537.36
X-Client-Data: CKO2yQEIxLbJAQj9lcoB

IIS deki Asp.Net çalışma zamanı ise şu cevabı döner.

HTTP/1.1 200 OK
Cache-Control: private
Content-Length: 1022
Content-Type: text/html; charset=utf-8
Date: Tue, 23 Feb 2016 08:09:21 GMT
Server: Microsoft-IIS/7.5
X-AspNet-Version: 4.0.30319
X-Powered-By: ASP.NET

Şimdi kendimizi web sunucusu yerine koyalım ve konuyu anlamaya çalışalım. İlk olarak kulağımız sürekli açık olmalı. Bize gelen tüm mesajları dinleyebilmeliyiz. Tabi gelen mesajların hedefi bsenyurt (ya da localhost veya 127.0.0.1) isimli makine ve portuda 8080 olmalı. Şu an için sadece bu adres:port üzerine gelen fısıltıları dinliyorum çünkü.

Tam bu konumda iken birisi kulağıma workstodo/main.aspx adresini içeren bir cümle fısıldıyor. Cümlede yer alan bilgileri ayrıştırıp istenen kaynağın sahibi olan çalışma zamanını tespit ediyorum. Sevgili dostum.Net CLR. Talebi ona gönderiyorum ve buna göre bana bir cevap hazırlamasını bekliyorum. Vereceğim cevapta ise önem arz eden bazı hususlar var.

Her şeyden önce içerik uzunluğunu (Content-Length) karşı tarafa iletmeliyim. Tabi talebin çalışması sonucu dostum bana her şey yolunda mesajını veriyorsa, karşı tarafa OK diyebilmeliyim. Bunu derken de karşımdakinin lisanını önemsemeden evrensel bir terimle konuşmalıyım. HTTP 200 OK gibi. Son olarak önemli olan bir diğer şey de karşı tarafa ne tip bir içerik gönderdiğimi ifade edebilmem. Ona HTML tipinde bir text içeriği gönderdiğimi söylersem o da beni anlayıp gelen içeriği yorumlayabilir. Tabii tüm bunlara ek olarak dostumun karşı tarafa iletmek istediği asıl içeriği de yollamam gerekiyor. Yani HTML içeriğinin kendisini.

Teorik olarak hikaye bu kadar basit aslında. Elbette IIS gibi gelişmiş web suncuları yazmak sanıldığı kadar kolay değil. Hatta gerek de olmayabilir. Yine de LightWeight tadında web sunucuları yazmak istediğimiz durumlar söz konusu olabilir. Ruby'de bu iş oldukça basit. Aşağıdaki kod parçasında basit bir HTTP Web Server yazmış bulunuyoruz.

```bash
#Bsait bir "Hello World" HTTP Server

require "socket" #TCPServer ve TCPSocket siniflari burada yer almakta

server=TCPServer.new('localhost',8082)

begin

while(session=server.accept)
	request=session.gets #gelen talebin ilk satiri okunuyor
	STDERR.puts request #log' lar console' a yazilacak
	
	response="<b1>Hello Rubyist!</b1><br/><i>This is xion control. Wellcome home</i>"
	session.print "HTTP/1.1 200 OK\r\n" +
				"Content-Type: text/html\r\n" +
				"Content-Length: #{response.bytesize}\r\n" +
				"Connection: close\r\n"
	session.print "\r\n"	#HTTP protokolu geregi bir alt satira gecilmesi gerekiyor
	session.print response #asil body mesaji yazdiriliyor
	session.close #sokect baglantisi kapatiliyor
end

rescue Errno::EPIPE
  STDERR.puts "Connection broke!"
  end
```

Öncelikli olarak uygulamamızı test edelim. Ruby kod dosyasını çalıştırdıktan sonra localhost:8082 adresine herhangi bir talepte bulunmamız yeterlidir.

![rubywebserver_1.gif](/assets/images/2016/rubywebserver_1.gif)

Neler yaptığımıza kısaca bakalım. Her şeyden önce TCP protokolüne göre basit bir soket haberleşmesi söz konsuudur. Bu nedenle socket modülünde bulunan TCPServer sınıfından yararlanıyoruz. Sınıfa ait nesne örneğini oluştururken verdiğimiz iki parametre ile makine adını ve port bilgisini bildiriyoruz. Örneğimize göre localhost:8082 adresini ele almaktayız. Sonrasında istemciden gelen talepler olduğu sürece devam edecek bir while döngümüz bulunuyor (Burada sonsuz bir döngü de söz konusu olabilir tabii)

Gelen talepler sonucu istemci ile aramızdaki iletişimi bir oturum (Session) olarak da düşünebiliriz. session.gets çağrısı kodun kritik olan ifadelerinden (pek kullanmasakta). Burada gelen HTTP Header bilgisinin ilk satırını okuyoruz ki bu bilgilerden yararlanarak pek çok işlem yapabiliriz. Örneğin talebin ne tip bir kaynağa yapıldığını ve HTTP metodunun ne olduğunu öğrenebiliriz. Örnek kodumuzda ise bu bilgiyi sadece console penceresine basıyoruz.

Bir diğer önemli kısım ise istemciye döndürülecek olan cevap. HTTP response'a ait bilgileri case-sensitive olarak eksiksiz bir şekilde hazırlamalıyız. HTTP 200 OK dışında, Content-Type ve Content-Length değerleri de çok önemli. Oluşturulan Header bilgisinin arkasınada response'u basıyoruz. O da basit bir HTML içeriği aslında. Döngümüzün son satırında ise session'ı kapatıyoruz. Tüm kod parçası bir hata kontrolü içerisinde yer almakta. Özellikle iletişim kanalının kopması gibi bir sorun oluşabilir (ki ben denemelerim sırasında buna rastladım) Bu nedenle EPIPE durumunu kontrol altına almaya çalıştık ancak gerçek hayat senaryosunda servisin tekrardan ayağa kaldırılması da gerekecektir.

Kodumuz ilgili adrese nasıl bir talep gelirse gelsin hep aynı cevabı verecektir aslında. Nitekim gelen HTTP Header bilgisini herhangi bir şekilde ayrıştırmış ve anlamaya çalışmış değiliz. Oysa ki gelen talebin hedef olarak gösterdiği adres bilgisine göre bir aksiyon alabiliriz. Söz gelimi bir pdf talebi geliyorsa buna uygun bir çıktı üretmeyi sağlayabiliriz. Dilerseniz bu durumu ele alaraktan örnek kodumuzu biraz daha geliştirelim ve aşağıdaki hale getirelim.

```text
require 'socket'
require 'uri'

server=TCPServer.new('localhost',8082) #taleplerin dinlenecegi makine:port

begin

	while(session=server.accept) #talep geldigi surece devam
		request=session.gets #Header bilgisini al
		STDERR.puts request	#ekrana bas
		request_uri  = request.split(" ")[1] #bosluklara gore ayirip talep edilen dosyayi bul
		path         = URI.unescape(URI(request_uri).path) #escape karakterleri cikart
		File.join('c:\\docs', path) #Fiziki yolu belirle
		ext = File.extname(path).split(".").last #dosya uzantisini al
		
		if File.exist?(path) && ext=="jpg" && !File.directory?(path) #dosya varsa uzanti dogruysa
			File.open(path, "rb") do |file| #dosyayi ac ve HTTP 200 OK paketini hazirla
				session.print "HTTP/1.1 200 OK\r\n" +
					   "Content-Type: image/jpeg\r\n" +
					   "Content-Length: #{file.size}\r\n" +
					   "Connection: close\r\n"
				session.print "\r\n"
				IO.copy_stream(file, session) #Resim icerigine ait byte array'i gonder
			end
		else #dosya bulunamadiysa HTTP 404 hatasini don
			message = "File not found\n"		
			session.print "HTTP/1.1 404 Not Found\r\n" +
						 "Content-Type: text/plain\r\n" +
						 "Content-Length: #{message.size}\r\n" +
						 "Connection: close\r\n"
			session.print "\r\n"
			session.print message
		end
		session.close #oturumu kapat
	end

rescue Errno::EPIPE
  STDERR.puts "Connection broke!"
  end
```

Bu sefer localhost:8082 adresinde docs/[bir dosya adı].jpg ile gelen talepleri değerlendirdiğimiz bir web sunucusu geliştirdik. Yani localhost:8082/docs/resim1.jpg gibi talepleri ele alan ve dosya varsa ilgili resim içeriğini tarayıcıya basan bir kod söz konusu. Gelen talepler c:\\docs klasöründeki jpg uzantılı dosyalar ile ilişkilendirilmeye çalışılmakta. Çalışma zamanında aşağıdakine benzer sonuçlar elde edebiliriz.

Var olan bir dosyanın talep edilmesi sonucu

![rubywebserver_2.gif](/assets/images/2016/rubywebserver_2.gif)

Olmayan bir dosyanın talep edilmesi sonucu

![rubywebserver_3.gif](/assets/images/2016/rubywebserver_3.gif)

jpg dışı bir uzantının talep edilmesi sonucu

![rubywebserver_4.gif](/assets/images/2016/rubywebserver_4.gif)

Örnek biraz daha geliştirilebilir. Söz gelimi sadece jpg uzantılı değil normal bir web sunucusu gibi farklı tipte içerikleri ele alacak hale de getirilebilir (Bu noktada farklı content-type'ların nasıl ele alınması gerektiğine bakılabilir) Ayrıca sadece Get değil Post gibi taleplerin ele alınması da söz konusu olabilir. Sunucu servis bazlı içerikleri de barındırıp buna uygun çalışabilir vb... Tahmin edileceği üzere bu güzel araştırma konularını siz değerli okurlarıma bırakıyorum. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
