---
layout: post
title: "Ruby Kod Parçacıkları 19 - SQLite ile Basit Veritabanı İşlemleri"
date: 2016-02-07 15:00:00 +0300
categories:
  - ruby
tags:
  - ruby-lang
  - sqlite
  - gem
  - sql
  - rdbms
  - scripts
---
Sanıyorum bir programlama dilini öğrenirken en çok merak edilen konulardan birisi de veritabanı işlemleridir. "Hani bir uygulama yazabilsem de bilgileri veritabanına kayıt edebilsem ve oradan okuyabilsem süper olur" benzeri bir cümleyi eminim hepimiz kurmuşuzdur. Günümüzde geliştirilen uygulamalar mutlak suretle verileri kalıcı olarak saklamaya ihtiyaç duyar.

![Ultra-Lightweight-Carbon-Fiber-Blackbraid-Bicycle-0.jpg](/assets/images/2016/Ultra-Lightweight-Carbon-Fiber-Blackbraid-Bicycle-0.jpg)

İster ilişkisel veritabanı ister NoSQL tabanlı bir sistem olsun verinin kalıcı olarak saklanması gerekir. Ruby tarafında da veritabanı işlemleri için farklı çözümler kullanılabilir. NoSQL çözümleri dışında RDMBS tarafı için Lightweight olarak düşünebileceğimiz SQLite bu veritabanı sistemlerinden birisidir.

Bu yazımızda SQLite veritabanı ile nasıl çalışabileceğimizi çok basit bir kod parçası ile incelemeye çalışacağız. Sistemimizde Ruby kurulu olduğunu düşünecek olursak tek yapmamız gereken SQLite'a ait gem paketini yüklemek olacaktır. Bunun için komut satırından aşağıdaki ifadeyi yürütebiliriz.

gem install SQLite3

> SQLite dünya üzerinde oldukça fazla dağıtımı olan C/C++ ile geliştirilmiş açık kaynak kodlu RDBMS-Relational Database Management System modelinde bir veritabanıdır. Linux, Windows, MacOS platformlarında kullanılabilir ve Basic'ten Smaltalk'a, C#'dan Ruby'ye pek çok dil tarafından desteklenir. Daha geniş bilgi için [bu adrese](http://sqlite.org/) bakabilirsiniz.

Eğer yükleme işlemi başarılı olduysa komut satırından SQLite3 yazarak veritabanı ortamına geçebilir ve aşağıdaki ekran görüntüsünde olduğu gibi bazı temel işlemleri yapabiliriz.

![sqlite1.gif](/assets/images/2016/sqlite1.gif)

Öncelikle komut satırında neler yaptığımıza kısaca bakalım.

sqlite3 AdventureWorks.db (Microsoft'un kobay veritabanının adını da yaad etmiş olduk) satırı ile AdventureWorks isimli bir veritabanı oluşturuluyor. Bu veritabanı komut satırını çalıştırdığımız klasörde yer alacak. Sonrasında.databases komutu ile oluşturulan veritabanını listeliyoruz (Tabii sistemde farklı veritabanları var ise onları da görmemiz gerekir) Devam eden ifade ile Category isimli bir tablo yaratıyoruz. Bu tabloda categoryid isimli otomatik olarak artan integer tipinden ve name isimli text tipinden birer alan (column) yer alıyor. Her iki alan da null değer almayacak şekilde belirlenmiş durumda..tables komutu ile oluşturulan tabloların listesini alabiliriz. Şu an için veritabanımızdaki tek tablo Category. Tablo oluşturulduktan sonra aynen SQL'de olduğu gibi bir kaç Insert işlemi gerçekleştiriyoruz. Bu işlmeler ile Books, Computers ve Toys isimli kategorileri veritabanımıza eklemiş bulunuyoruz. Select işlemleri ile de eklenen kategorileri komut satırına basıyoruz. Eğer komut satırı ekranında daha düzenli bir görünüm elde etmek istiyorsak (örneğin kolon adlarının çıkması veya tab'lı aralıklar bırakılması gibi) headers ve mode ortam değerlerini değiştirmemiz yeterli.

Görüldüğü üzere SQL tarafında çalışmış olanlar için SQLite'ın kullanımını anlamak ve öğrenmek son derece basit. Peki Ruby tarafında nasıl kullanabiliriz? İşte örnek bir Ruby kod parçacığı.

```text
require 'sqlite3'

begin
	db=SQLite3::Database.open "AdventureWorks.db"
	db.execute "create table if not exists Product(product_id integer primary key autoincrement,title text not null,list_price int not null)"

	db.execute "insert into Product (title,list_price) values ('Polo Lanc 1.2 TSI',65000)"
	db.execute "insert into Product (title,list_price) values ('Golf Sport 2.0 TDI',90000)"
	db.execute "insert into Product (title,list_price) values ('Subaru impreza 4x4',120000)"

	select=db.prepare "Select product_id,title,list_price from Product"
	resultSet=select.execute

	resultSet.each{ |row|
	puts row.join "\s"
	}
rescue SQLite3::Exception => excp
	puts excp
ensure
	select.close if select
	db.close if db
end
```

Kod dosyamızı UsingSQLite.rb ismiyle kayıt edebiliriz.

Öncelikle üzerinde çalışacağımız veritabanını açmak için SQLite3::Database.open ifadesinden yararlanmaktayız. Bu satır ile veritabanı nesnesini elde ediyoruz. db değişkeni üzerinden execute metodunu kullanarak bazı SQL ifadeleri yürütüyoruz. Product isimli bir tabloyu oluşturmak için Create script'inden yararlanıyoruz. Buna ek olarak bir kaç satır veri ekliyoruz. Bu işlemler için execute metodunu kullanmamız ve parametre olarak insert ifadesini vermemiz yeterli. Select sorgusunu ise öncelikle prepare metodu ile hazırlıyoruz. Elde edilen ifade üzerindense yine execute fonksiyonunu kullanmaktayız. Tahmin edileceği üzere resultSet değişkeni üzerinden each metodu ile ilerleyebilir ve elde edilen tüm satırları dolaşabiliriz. Bu işlemi yaparken her bir alan arasına birer boşluk bırakarak komut satırına yazdırma işlemini gerçekleştiriyoruz.

Kod parçasında bir hata kontrol mekanizmamızda bulunuyor. ensure bloğu içerisinde hata alsak da almasak da çalıştıracağımız kod parçaları mevcut. Çok doğal olarak burada da veritabanı bağlantısını açık bırakmamak gerekiyor. Hepsi bu kadar basit. İşte çalışma zamanı çıktıları.

![sqlite2.gif](/assets/images/2016/sqlite2.gif)

Eklediğimiz tüm ürünler Product tablosuna yazılmıştır. Görüldüğü üzere Ruby tarafında SQLite kullanımı oldukça basit. Elbette işleri daha da ileri götürmelisiniz. Söz gelimi Ruby tarafında popüler olan Sinatra isimli Web Framework'ü kullanarak AdventureWorks üzerinde yürütülebilecek temel CRUD (Create Read Update Delete) operasyonlarını basit bir REST servis üzerinden sunmayı deneyebilirsiniz. Antrenman yapmaya devam. Böylece geldik bir yazımızın daha sonuna. Bir başka kod parçasında görüşünceye dek hepinize mutlu günler dilerim.
