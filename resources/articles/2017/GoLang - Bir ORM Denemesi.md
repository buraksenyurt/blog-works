---
title: "GoLang - Bir ORM Denemesi"
pubDate: 2017-07-16 21:40:00
categories:
  - GoLang
tags:
  - GoLang
  - .NET
  - Entity Framework
  - SQL Server
  - Oracle
  - MySQL
  - NoSQL
  - REST
  - HTTP
  - Go
  - Transactions
  - GitHub
---

# GoLang - Bir ORM Denemesi
![gogorm_buffon.jpg](images/gogorm_buffon.jpg)

Merhaba Arkadaşlar,

Yakın zamanda bir şampiyonlar ligi finali vardı. Real Madrid ve Juventus arasında oynanan maçı eflatun beyazlılar 4-1 gibi farklı bir skorla kazandı. Aslında ilk yarı Juventus çok daha iyi paslaşıyordu lakin ikinci yarı Ronaldo faktörü ön plana çıktı. Modric'in de etkili orta saha oyunu ile İspanyol ekibi kupayı üstüste ikinci kez almayı başardı. Benim gönlüm Juventus'tan yanaydı çünkü kalede 39 yaşında olan Buffon yer alıyordu. Özellikle İtalyan kulüplerinden 40lı yaşlarına kadar oynayan çok başarılı sporcular çıkıyor (Francesco Totti, Andrea Pirlo, Roberto Baggio vb) Kendilerine iyi bakıyorlar ve özellikle de mesleklerine profesyonelce yaklaşıyorlar. Bu ilham verici bir şey. Hatta pek çok genç sporcuya da örnek olmalı diye düşünüyorum. Gerçi Buffon'a bir şekilde makalemde yer vermek istediğim için bu girişi yaptım. Gelin asıl konumuza geçelim.

Veri depolamanın en popüler yolu NoSQL veya RDBMS bazlı sistemler. 90lı yıllardan beri program yazan insanlar için de özellikle Microsoft SQL Server, Oracle ve sonrasında gelen MySQL ya da SQLite gibi yapılar da oldukça fazla oranda kullanılmaktalar. E tabii bildiğiniz üzere bu serüvenin ortalarında bir yerlerde SQL dili ve RDBMS yapısının, programcıların kodlama mantığına biraz ters gelişi de vuku buldu. Sonuçta SQL tarafındaki varlıkların programatik ortamda ve özellikle nesne yönelimli (Object Oriented) dünyada nasıl daha anlamlı ele alınabileceğinin yolları araştırıldı. Artık popüler olma zamanı nesne ilişkilendirmelerini sağlayan araçlardaydı. Object Relational Mapping (O/RM) konusu gündemdeydi. Neredeyse bütün programlama çatılarının bu tip araçlarla yakın ilişkisi bulunmakta. Hibernate ve Entity Framework gibi en azından ülkemizde adını sıklıkla duyduğumuz araçlar dışında farklı pek çok ürün de bulunmakta. Ben de GoLang tarafında SQLite operasyonlarını incelemeye çalışırken "bir O/RM aracı var mıdır?" sorusuna cevap ararken buldum kendimi. Murat Hoca'nın kitabı, GoLang'in resmi dokümanları, Stackoverflow tartışmaları derken [gitub üzerinden sunulan GORM](https://github.com/jinzhu/gorm) ile karşlılaştım.

Geliştirici dostu olan fantastik bir ORM aracı olarak tanımlamış kendisini. Pek çok özelliği var. Eager Loading, Transaction, Auto-Migration, Callback fonksiyonları, genişletilebilirlik, tablolar arası ilişkilerin ifade edilmesi bu özelliklerden sadece bir kaçı ([github](https://github.com/jinzhu/gorm) adresinden kaynak kodlarına da bakabilirsiniz) Tabii benim amacım çok temel düzeyde aracı nasıl kullanabileceğimi öğrenmekti. Elimde SQLite gibi hafif ama bence inanılmaz yetenekli bir veritabanı bulunuyordu. GO dili ile ilgili bilgilerim artmaktaydı. Entity modelini baştan tasarlayabilirdim. Sonrasında Gorm'un modele ait tabloları benim için oluşturması, bir kaç satır verinin insert edilmesi, belki bir update veya delete operasyonunun gerçekleştirilmesi "Hello Gorm" demek adına yeterliydi.

Modelin Kurgulanması

İşe bir Entity paketi hazırmakla başladım. Daha önceden yaptığım gibi GOPATH'in belirttiği src klasörü altında konuşlandırıp tüm GO uygulamaları tarafından erişilebilecek bir paket yazmaya karar verdim. İçinde iki tane Entity olsa da Gorm'un model tarafındaki bir kaç yeteneğini anlamak için ideal bir seçimdi. İşte Southwind (Emektar Northwind gelir hep aklıma ki [şu adresten REST](http://northwind.servicestack.net/) tabanlı servislerine de erişebilirsiniz) paketinin basit içeriği.

```cpp
package Southwind

import (
	"github.com/jinzhu/gorm"
)

type Employee struct {
	gorm.Model
	FirstName string `gorm:"not null;size:30"`
	LastName  string `gorm:"not null;size:30"`
	Emails    []Email
}

type Email struct {
	gorm.Model
	EmployeeID int    `gorm:"index"`
	Mail       string `gorm:"type:varchar(50);unique_index"`
	IsActive   bool
}
```

Öncelikle gorm'un import edilmesi gerekiyor. LiteIDE kullandığım için işim kolay. Paket bildiriminden sonra build->get komutunu vererek Gorm'un sisteme yüklenmesini sağlayabiliyoruz (Bunun her tür github go paketi için geçerli olduğunu hatırlayalım lütfen) İki yapı görüyorsunuz. Employee ve Email. Aslında bire çok (one to many) ilişkiyi kullandığımız bir modelleme söz konusu. Bir çalışanın birden fazla email adresi olabilir düşüncesi söz konusu. Her iki yapı içerisinde Model tipinden bir değişken tanımı yer alıyor. Hem Employee hem Email yapıları içerme tekniği ile bu tipi uygulamakta. Model aslında ana nesne olarak düşünülebilir. ID, CreatedAt, UpdatedAt ve DeletedAt şeklinde standart alanlar içermekte. Dolayısıyla tüm tiplerimiz bu özelliklere sahip olacaklar. ID otomatik artan bir Primary Key iken diğer alanlar oluşturulma, güncelleme ve silinme zamanı bilgileri için kullanılmaktalar. Bu alanlar istenirse ezilebilir. Bu tip özelleştirme yetenekleri Gorm içerisinde mevcut.

Bire çok ilişkiyi nesne modelinde kurgulamak için Employee içerisinde Email türünden bir slice ve Email içinde de EmployeeID isminde int türünden bir alan tanımlandığına dikkat edelim. Email nesneleri hangi Employee ile ilişkilendirildiklerini EmployeeID alanı üzerinden otomatik olarak anlayabilirler. Daha doğrusu Gorm çalışma zamanı bu ilişkiyi isimlendirmelere göre kolayca kurabilir. Tabii ` işaretlerinden sonra gelen tanımlamalar da önemli. EmployeeID tarafında bu alanın bir index olduğu belirtilmekte. Peki ya diğer bildirimler. Örneğin FirstName ve LastName alanları 30ar karakter boyutunda olabilirler ve null değer içeremezler. Mail alanı 50 karakteri geçemezken benzersiz bir içeriğe sahip olmak zorundadır. Bu tanımlamalar özellikle tablolar oluşturulurken Gorm motoru tarafından değerlendirilecektir.

Paketi bu şekilde oluşturduktan sonra önce Build sonra da Install işlemlerini gerrçekeştirmek lazım. Böylece GOPATH'in belirttiği konumda yer alan pkg klasör altında a uzantılı derlenmiş hali üretilmiş olacaktır.

Asıl Kod

Artık asıl test kodları geliştirmeye başlanabilir.

```cpp
package main

import (
	"fmt"

	"entity/southwind"

	"github.com/jinzhu/gorm"
	_ "github.com/mattn/go-sqlite3"
)

func main() {
	db, err := gorm.Open("sqlite3", "db\\southwind.sdb")
	db.LogMode(true)
	defer db.Close()
	if err == nil {
		//db.SingularTable(true)
		db.AutoMigrate(&Southwind.Employee{}, &Southwind.Email{})
		db.Model(&Southwind.Employee{}).Related(&Southwind.Email{})

		burakMails := []Southwind.Email{
			Southwind.Email{Mail: "selim@buraksenyurt.com", IsActive: true},
			Southwind.Email{Mail: "burak.senyurt@southwind.com", IsActive: false},
			Southwind.Email{Mail: "burakselimsenyurt@gmail.com", IsActive: true},
		}

		burak := Southwind.Employee{FirstName: "burak", LastName: "senyurt", Emails: burakMails}
		db.Create(&burak)

		loraMails := []Southwind.Email{
			Southwind.Email{Mail: "lora@kimbilll.moon", IsActive: true},
			Southwind.Email{Mail: "kimbill.the.black.lora@southwind.com", IsActive: true},
		}
		lora := Southwind.Employee{FirstName: "Lora", LastName: "Kimbılll", Emails: loraMails}
		db.Create(&lora)

		WriteToScreen(burak)
		WriteToScreen(lora)

		var burki Southwind.Employee
		db.Find(&burki, "ID=?", 1) //Önce
		db.Model(&burki).Update("LastName", "Selim Senyurt")
		WriteToScreen(burki)

		var buffon Southwind.Employee

		db.Model(&buffon).Where("ID=?", 2).Updates(map[string]interface{}{"FirstName": "Cianluici", "LastName": "Buffon"})
		db.First(&buffon, 2) //Direkt primary key üstünden(varsayılan olarak ID) arama yapar
		WriteToScreen(buffon)
	} else {
		fmt.Println(err.Error())
	}
}

func WriteToScreen(e Southwind.Employee) {
	fmt.Printf("%d\t%s,%s,%s\n", e.ID, e.FirstName, e.LastName, e.CreatedAt)
	for _, email := range e.Emails {
		fmt.Printf("\t%d:%s\n", email.ID, email.Mail)
	}
}
```

southwind dışında SQLite ve Gorm için gerekli paket bildirimleri ile işe başlıyoruz. gorm'un Open fonksiyonu iki parametre alıyor. İlki kullanılan veritabanı sürücüsü ki biz örneğimizde SQLite veritabanını kullanacağız. İkincisi ise veritabanı dosyasının adı. db alt klasöründe konuşlandıracağımız southwind.sdb isimli bir dosya söz konusu. LogMode fonksiyonuna true değerini atayarak model tarafında gerçekleşen işlemlerin SQL karşılıklarının debug penceresine basılmasını istiyoruz. Böylece SQLite tarafında olan biteni görme şansına da sahibiz. AutoMigrate fonksiyonuna iki parametre geçiyoruz. Employee ve Email. Bunların adreslerini geçtiğimize dikkat edelim (& ne işe yarıyordu hatırlayın). AutoMigrate eğer yoklarsa ilgili tabloların modele bakılarak oluşturulmasını sağlayacak. Güncellemeler varsa bunlar da ilgili fonksiyon tarafından ele alınmakta. Tipik bir migration işlemi gerçekleştirildiğini ifade edebiliriz ancak bir çok işlemi (tablo oluşturmak gibi) manuel de yazabilliriz.

Model fonksiyonu ile Employee ve Email yapıları arasındaki ilişkiyi tesis ediyoruz. Hatırlayacağınız gibi "bir çalışanın birden fazla email adresi olabilir" düşüncesinden yola çıkarak her iki model arasında bire çok ilişki olması gerektiğini yapıları yazarken belirtmiştik. Devam eden satırda bir Email listesi oluşturuyor ve örnek bir kaç veri üretiyoruz. Sonrasında burak isimli Employee tipinden bir nesne örnekleniyor. Bu nesnenin Emails niteliğini de burakMails değişkenine bağlıyoruz. Create fonksiyonuna parametre olarak gönderilen burak değişken adresi sonrası hem Email hem de Employee örnekleri için gerekli Insert işlemleri çalıştırılacak. Employee ve Email arasındaki ilişkiyi tesis ettiğimizden tüm Email'lerin EmployeeID değerleri otomatik olarak burak isimli çalışanın ID alanına bağlanacaklar.

Kodun ilerleyen kısmında bu kez lora isimli bir çalışan üretip bir kaç email adresi daha ekliyoruz. Yine Create fonksiyonundan yararlandığımızı ifade edelim. WriteToScreen fonksiyonu bir Employee nesnesini alıp çalışan bilgileri ve bu çalışana ait email adreslerini ekrana yazdırmaktan sorumlu. Dikkat edileceği üzere çalışanın maillerine giderken normal bir for döngüsü ve range işlevini kullanıyoruz. Sanki veritabanında değilmişiz gibi. Ama arka planda tüm bunlar SQL sorgusu haline gelmekteler.

Kodun bir sonraki kısmında Find operasyonu ile ID alanının değeri 1 olan Employee satırını yakalayıp Model ve Update fonksiyonlarını peş peşe çağırarak bir güncelleme işlemi gerçekleştirmekteyiz. Find ile bulduğumuz nesneyi &burki değişkenine çıkıyoruz. Aynı değişkeni Model fonksiyonunda kullanıp arkadan gelen Update ile LastName alanının değerini değiştiriyoruz. Burada tipik bir Update sorgusu olduğunu ifade edebiliriz ve ilerleyen kod satırlarında farklı bir sürümü daha yer alıyor. Bu kez Model üzerinden Where operasyonu'na gidip ID değeri 2 olan Email satırını yakalıyoruz. Hemen arkasından Updates isimli bir fonksiyon çağrısı daha geliyor. Bu fonksiyon ile FirstName ve LastName alanlarını güncelliyoruz. First fonksiyonu ikinci parametre de aldığı değeri otomatik olarak ilgili Entity'nin ID alanı ile ilişkilendirmekte. Yani Employee tablosunda ID alanı 2 olan satırı yakalayıp içeriğini buffon değişkenine çekmekteyiz (Elbette farklı bir where kriteri de koyabilirsiniz) Sonrasında bulduğumuz sonuçları ekrana basıyoruz. Amacımız 2 numaralı çalışanın adının değiştiğini görmek.

Sonuçlar

Uygulamanın çalışma zamanı çıktısı aşağıdaki gibi olacaktır. Adım adım üretilen SQL sorgularını incelemenizi ve GO lang çıktılarına bakmanızı öneririm. DB tarafına en ufak bir SQL sorgusu göndermeden var olan Entity örnekleri üzerinde gerçekleştirdiğimiz Insert, Update, Select gibi işlemler otomatik olarak SQLite üzerinde çalıştırılmıştır. Programcının aşina olduğu kavramları veritabanı tarafına göndermiş durumdayız.

```text
(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:21) 
[2017-06-05 00:02:57]  [182.01ms]  CREATE TABLE "employees" ("id" integer primary key autoincrement,"created_at" datetime,"updated_at" datetime,"deleted_at" datetime,"first_name" varchar(30) NOT NULL,"last_name" varchar(30) NOT NULL ) 

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:21) 
[2017-06-05 00:02:58]  [147.00ms]  CREATE INDEX idx_employees_deleted_at ON "employees"(deleted_at) 

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:21) 
[2017-06-05 00:02:58]  [155.00ms]  CREATE TABLE "emails" ("id" integer primary key autoincrement,"created_at" datetime,"updated_at" datetime,"deleted_at" datetime,"employee_id" integer,"mail" varchar(50),"is_active" bool ) 

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:21) 
[2017-06-05 00:02:58]  [170.00ms]  CREATE INDEX idx_emails_deleted_at ON "emails"(deleted_at) 

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:21) 
[2017-06-05 00:02:58]  [184.01ms]  CREATE INDEX idx_emails_employee_id ON "emails"(employee_id) 

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:21) 
[2017-06-05 00:02:58]  [115.00ms]  CREATE UNIQUE INDEX uix_emails_mail ON "emails"("mail") 

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:22) 
[2017-06-05 00:02:58]  [1.00ms]  SELECT * FROM "emails"  WHERE "emails"."deleted_at" IS NULL AND (("employee_id" = '0'))

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:31) 
[2017-06-05 00:02:58]  [1.00ms]  INSERT INTO "employees" ("created_at","updated_at","deleted_at","first_name","last_name") VALUES ('2017-06-05 00:02:58','2017-06-05 00:02:58',NULL,'burak','senyurt')

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:31) 
[2017-06-05 00:02:58]  [0.00ms]  INSERT INTO "emails" ("created_at","updated_at","deleted_at","employee_id","mail","is_active") VALUES ('2017-06-05 00:02:58','2017-06-05 00:02:58',NULL,'1','selim@buraksenyurt.com','true')

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:31) 
[2017-06-05 00:02:58]  [0.00ms]  INSERT INTO "emails" ("created_at","updated_at","deleted_at","employee_id","mail","is_active") VALUES ('2017-06-05 00:02:58','2017-06-05 00:02:58',NULL,'1','burak.senyurt@southwind.com','false')

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:31) 
[2017-06-05 00:02:58]  [1.00ms]  INSERT INTO "emails" ("created_at","updated_at","deleted_at","employee_id","mail","is_active") VALUES ('2017-06-05 00:02:58','2017-06-05 00:02:58',NULL,'1','burakselimsenyurt@gmail.com','true')

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:38) 
[2017-06-05 00:02:58]  [1.00ms]  INSERT INTO "employees" ("created_at","updated_at","deleted_at","first_name","last_name") VALUES ('2017-06-05 00:02:58','2017-06-05 00:02:58',NULL,'Lora','Kimbılll')

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:38) 
[2017-06-05 00:02:58]  [0.00ms]  INSERT INTO "emails" ("created_at","updated_at","deleted_at","employee_id","mail","is_active") VALUES ('2017-06-05 00:02:58','2017-06-05 00:02:58',NULL,'2','lora@kimbilll.moon','true')

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:38) 
[2017-06-05 00:02:58]  [0.00ms]  INSERT INTO "emails" ("created_at","updated_at","deleted_at","employee_id","mail","is_active") VALUES ('2017-06-05 00:02:58','2017-06-05 00:02:58',NULL,'2','kimbill.the.black.lora@southwind.com','true')
1	burak,senyurt,2017-06-05 00:02:58.6894696 +0300 EEST
	1:selim@buraksenyurt.com
	2:burak.senyurt@southwind.com
	3:burakselimsenyurt@gmail.com
2	Lora,Kimbılll,2017-06-05 00:02:58.8994816 +0300 EEST
	4:lora@kimbilll.moon
	5:kimbill.the.black.lora@southwind.com

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:44) 
[2017-06-05 00:02:59]  [3.00ms]  SELECT * FROM "employees"  WHERE "employees"."deleted_at" IS NULL AND ((ID='1'))

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:45) 
[2017-06-05 00:02:59]  [2.00ms]  UPDATE "employees" SET "last_name" = 'Selim Senyurt', "updated_at" = '2017-06-05 00:02:59'  WHERE "employees"."deleted_at" IS NULL AND "employees"."id" = '1'
1	burak,Selim Senyurt,2017-06-05 00:02:58.6894696 +0300 +0300

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:50) 
[2017-06-05 00:02:59]  [4.00ms]  UPDATE "employees" SET "first_name" = 'Cianluici', "last_name" = 'Buffon', "updated_at" = '2017-06-05 00:02:59'  WHERE "employees"."deleted_at" IS NULL AND ((ID='2'))

(C:/Go Works/Samples/book/Web Programming/Lesson_28/server.go:51) 
[2017-06-05 00:02:59]  [3.00ms]  SELECT * FROM "employees"  WHERE "employees"."deleted_at" IS NULL AND (("employees"."id" = '2')) ORDER BY "employees"."id" ASC LIMIT 1
2	Cianluici,Buffon,2017-06-05 00:02:58.8994816 +0300 +0300
Success: process exited with code 0.
```

Şu noktada SQLite üzerinden ilgili veritabanı açılırsa kod tarafındaki işlemlerin oraya da yansıdığını görebiliriz. Ancak dikkat çekici bir kaç nokta olduğunu da vurgulamak isterim.

![gogorm_2.gif](images/gogorm_2.gif)

Mutlaka dikkatinizi çekmiştir ki FirstName first_name, LastName last_name, EmployeeId employee_id, IsActive is_active olarak ifade edilmiş durumdalar. Yani modelde belirttiğimiz alan adlarının büyük küçük harf durumlarına göre kolon adları şekillendirilmiş. Diğer yandan tablo adlarının çoğullandığını görüyoruz. Employee yerine Employees ve Email yerine Emails şeklinde bir isimlendirme söz konusu. Elbette tüm bunlar Gorm tarafında özelleştirilebilmekte. Gelelim veri içeriğine. İşte bir kaç örnek sorgu sonucu SQLite içeriği.

![gogorm_3.gif](images/gogorm_3.gif)

Tabii Gorm aracının dokümantasyonu oldukça geniş ve zengin. Özelleştirilebilecek, ince ayarlar yapılabilecek bir çok konu var. Ben "Hello Gorm" demeye çalıştım ve istediğim bilgileri aldım sayılır. Bundan sonrasında daha önceki web sunucusu ve REST servis örneklerini Gorm paketi ile çalışacak hale getirmeyi planlıyorum. Siz bu yazıyı Gorm dokümanını inceleyerek daha da ileriye götürebilirsiniz. İşe eksik olan Delete operasyonunu ekleyerek başlayabilirsiniz. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
