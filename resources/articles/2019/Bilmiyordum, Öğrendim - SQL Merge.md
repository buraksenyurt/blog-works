---
title: "Bilmiyordum, Öğrendim : SQL Merge"
pubDate: 2019-01-13 13:03:00
categories:
  - T SQL
tags:
  - T SQL
  - Performance
---

# Bilmiyordum, Öğrendim : SQL Merge
![sqlmerge_0.jpg](images/sqlmerge_0.jpg)

Merhaba Arkadaşlar,

Gün geçmiyor ki çevremdeki insanlardan yeni bir şeyler daha öğrenmeyeyim. Bugün o günlerden biriydi...

İş yerinde elimizin her an üzerinde olabileceği binlerce SQL nesnemiz var. Tablolar, fonksiyonlar, sp'ler... Bazen iş biriminden gelen istekler doğrultusunda onlara müdahale etmemiz veya yenilerini yazmamız gerekiyor. Sorun şu ki 2000li yılların başından kalan ve yorum satırlarına bakıldığında üzerinden bir çok geliştiricinin geçtiği spl'lerimiz var. Bazen buradaki kalabalık sorgular arasında samanlıkta iğne ararcasına sorun çözmeye çalıştığımız oluyor. Çok motive edici bir durum değil takdir ederseniz ki. Şükür ki alanlarında yetkin ekip arkadaşlarımız var ve yeri geldiği zaman söyledikleri ufak bir ipucu ile hayatımızı kolaylaştırıyorlar (ki bu etkili yardımlaşmada agile metodolojide koşan bir takım olmamızın da büyük etkisi var)

İşte geçenlerde çok uzun sürdüğü için sorun yaratan bir sp (Stored Procedure) ile cebelleşirken değerli bir yardım geldi. Ekip arkadaşımın bir önerisi üzerine kendimi SQL Merge komutunu araştırırken/öğrenirken buldum. 2008den beri var olan benim bihaber olduğum bu komutu öğrenirken keyifli anlar da yaşadım. Normalde çok kötü bir SQLciyimdir ama Merge komutunu uygulamalı olarak denedikten sonra şirketteki o kallavi sorgunun hem daha da hızlandığını hem de daha okunur hale geldiğini gördüm. Sonunda konuyu kalem alıp paylaşmanın iyi olacağını fark ettim. Hem kendim için kayıt altına almış hem de yazıp çizerek konuyu daha iyi öğrenmiş olmam da ödülüm olacak tabii. Dilerseniz vakit kaybetmeden konumuza geçelim. Başlangıç için aşağıdaki veri içeriklerine sahip iki tablomuz olduğunu düşünelim.

Kaynak tablo içeriği (Book)

```text
BookID      Title                                              ListPrice             StockLevel
----------- -------------------------------------------------- --------------------- ----------
1           Clean Architecture                                 34,55                 5
2           Clean Code                                         20,00                 5
3           Anti-patterns explained                            15,99                 10
4           Programming C#                                     50,40                 20
```

Hedef tablo içeriği (Store)

```text
BookID      Title                                              ListPrice             StockLevel
----------- -------------------------------------------------- --------------------- ----------
1           Clean Architecture                                 34,55                 5
2           Clean Code                                         10,00                 5
3           Anti-patterns explained                            15,99                 8
6           Cloud for dummies                                  44,44                 3
```

Veritabanı ile çalışan pek çok uygulamada bu tip birleştirme odaklı tablolara rastlayabiliriz. Genellikle dışarıdan belirli periyotlarla beslenen bir tablo ve bu tablodaki veri içeriğine göre kendini sürekli olarak güncel tutan bir başka tablo olur. Aynen yukarıdaki senaryoda görülen Kaynak ve Hedef tablolar gibi. Özetle hedef tabloyu kaynak tablodaki değişikliklere göre güncel tutmak istediğimizi düşünebiliriz. Kaynaktan silinenlerin hedeften de silinmesi, güncellenenlerin aynı şekilde hedefte de güncellenmesi veya kaynağa yeni gelenlerin hedef tabloya da aktarılması gibi işlemlerden bahsediyoruz.

Book ve Store tablolarını göz önüne aldığımızda Store tablosundan silinen (4 nolu kayıt), eklenen (6 nolu kayıt), güncellenen (2 ve 3 nolu kayıtlar) ve hiç bir değişikliğe uğramayan (1 nolu kayıt) kitap bilgileri olduğunu görüyoruz. Şimdi Book tablosunu Store tablosuna göre güncellememiz gerekiyor. Elbette bunun bir çok yolu var. Örneğin Cursor açıp kaynak tabloyu baştan sona tarayarak bu işlemi gerçekleştirebiliriz. Ya da Select into ifadesi ile birlikte insert, update, delete sorgularını kullanabiliriz. Belki başka çözümler de söz konusu olabilir. Buradaki gibi az sayıda satır içeren veri kümeleri için seçilen tekniğin bir önemi yok aslında. Ancak tablo kayıt sayısı aynen şirketimizdeki senaryodaki gibi milyonlar seviyesine çıkınca performans sorunları yaşayabiliriz. Bir alternatif olarak üzerinde insert, update ve delete işlemlerini uygulamak için tek bir birleştirme maliyeti üzerinden hareket etmek çok daha verimli olabilir. Merge bu noktada devreye giriyor.

Normal şartlarda yukarıdaki içerikleri eşleştirmek adına pekala aşağıdaki gibi sorgular yazılabilir (Bildiğim kadarı ile yazdım. Bu konuda alternatifler için aydınlatılmaya ihtiyacım var)

```text
Update Book 
Set 
	Title=S.Title,
	ListPrice=S.ListPrice,
	StockLevel=S.StockLevel
from Store S
	inner join Book B
	on B.BookID=S.BookID
where 
	B.ListPrice<>S.ListPrice or B.Title<>S.Title or B.StockLevel <> S.StockLevel;

Delete from Book Where BookID not in 
	(Select S.BookID from Store S where S.BookID in (Select BookID from Book));
```

İlk olarak farklılıkları bulup gerçekleştirdiğimiz bir Insert işlemi var. Burada alt sorgu kullandığımızı görebilirsiniz. Güncelleme işleminde ise bir inner join kullanımına gittik. En beter sorgu da silme operasyonu için yazdığım olmalı sanıyorum ki. Bu sorguları işlettiğimizde Book ve Store tabloları eşlenecektir. Lakin bir taşla üç kuş vurabiliriz de. Şimdi konuyu merge ifadesini baz alarak ele alalım. Aşağıdaki uçtan uca sorgu işimizi görür (Ben diğer veritabanlarını kirletmemek adına LearningDb isimli ayrı bir veritabanında çalıştım)

```text
Create database LearningDb;
Use LearningDb;

Create Table Book
(
	BookID int primary key,
	Title varchar(50),
	ListPrice money,
	StockLevel smallint
)
Go
insert into Book
Values
(1,'Clean Architecture',34.55,5),
(2,'Clean Code',20.00,5),
(3,'Anti-patterns explained',15.99,10),
(4,'Programming C#',50.40,20)
Go

Create Table Store
(
	BookID int primary key,
	Title varchar(50),
	ListPrice money,
	StockLevel smallint
)
Go
insert into Store
Values
(1,'Clean Architecture',34.55,5), --Değişiklik yok
(2,'Clean Code',10.00,5), -- Fiyat değişti
(3,'Anti-patterns explained',15.99,8), --Stok seviyesi değişti
(6,'Cloud for dummies',44.44,3) -- Yeni geldi
--(4,'Programming C#',50.40,20) -- Silindi
Go

Select * from Book;
Select * from Store;

Merge Book AS T
Using Store As S
on (T.BookID=S.BookID)
When Matched and T.Title <> S.Title Or T.ListPrice<>S.ListPrice Or T.StockLevel<>S.StockLevel Then --Herhangibir güncelleme varsa
Update Set T.Title=S.Title,T.ListPrice=S.ListPrice,T.StockLevel=S.StockLevel
When Not Matched By Target Then -- Yeni eklenmiş kitaplar varsa
Insert (BookID,Title,ListPrice,StockLevel)
Values (S.BookID,S.Title,S.ListPrice,S.StockLevel)
When Not Matched By Source Then -- Silinmiş kitaplar varsa
DELETE
OUTPUT $action [Event], DELETED.BookID as [Target BookID],DELETED.Title as [Target Title],DELETED.ListPrice as [Target ListPrice],DELETED.StockLevel as [Target StockLevel],
INSERTED.BookID as [Source BookID],INSERTED.Title as [Source Title],INSERTED.ListPrice as [Source ListPrice],INSERTED.StockLevel as [Source StockLevel];

Select * from Book;
Select * from Store;
```

Merge kısmına kadar yapılan hazırlıklarda örnek bir veritabanı oluşturup içerisine Book ve Store isimli tablolarımızı açıyoruz (Buralarda if exist kullanımına gitmekte yarar olabilir ya da başlarda drop table kullanılabilir) Sonrasında ise Merge ifademiz başlıyor. Book ve Store tablolarını BookID alanı üzerinden birleştirdikten sonra When kelimesi ile başlayan üç ayrı kısım yer alıyor.

Eğer bir eşleşme var ve tabloların Title, ListPrice, StockLevel alanlarının en azn birisinde veya tümünde farklılıklar söz konusuysa Then kelimesinden sonra gelen Update ifadesi çalıştırılıyor. Update ifadesinde T ile belirtilen hedef tablo alanlarının S ile belirtilen kaynak tablo alanları ile beslendiğine dikkat edelim. Eğer hedef tabloda kaynaktaki satırlar ile BookID üzerinden bir eşleşme yoksa 'When not matched by Target Then'sonrasında gelen Insert sorgusu çalışıyor. Burada da kaynak tablodaki alan değerlerinin eklendiğine dikkat edelim. Son olarak hedefte olduğu halde kaynakta olmayan satırlar varsa 'When not matched by source then'sonrasındaki Delete ifadesi çalışıyor ve hedef tablodaki ilgili kayıtlar siliniyor.

Merge sorgusunun tamamlanması için mutlaka; işareti ile ifadeyi bitirmemiz gerekiyor. Bunu yapmadan önce meydana gelen değişiklikleri takip edebilmek adına output ifadesini çalıştırıyoruz. Burada $action değişkeni ile meydana gelen olay yakalanıyor (o satır için insert, update, delete olaylarından hangisi olduysa) DELETED ve INSERTED isimli hazır tabloları kullanaraktan da hangi tabloda ne gibi bir alan değişikliği olduğunu rahatlıkla görebiliyoruz. Sonuçlar aşağıdaki gibi olacaktır.

```text
Event      Target BookID Target Title                                       Target ListPrice      Target StockLevel Source BookID Source Title                                       Source ListPrice      Source StockLevel
---------- ------------- -------------------------------------------------- --------------------- ----------------- ------------- -------------------------------------------------- --------------------- -----------------
UPDATE     2             Clean Code                                         20,00                 5                 2             Clean Code                                         10,00                 5
UPDATE     3             Anti-patterns explained                            15,99                 10                3             Anti-patterns explained                            15,99                 8
DELETE     4             Programming C#                                     50,40                 20                NULL          NULL                                               NULL                  NULL
INSERT     NULL          NULL                                               NULL                  NULL              6             Cloud for dummies                                  44,44                 3

(4 row(s) affected)

BookID      Title                                              ListPrice             StockLevel
----------- -------------------------------------------------- --------------------- ----------
1           Clean Architecture                                 34,55                 5
2           Clean Code                                         10,00                 5
3           Anti-patterns explained                            15,99                 8
6           Cloud for dummies                                  44,44                 3

(4 row(s) affected)

BookID      Title                                              ListPrice             StockLevel
----------- -------------------------------------------------- --------------------- ----------
1           Clean Architecture                                 34,55                 5
2           Clean Code                                         10,00                 5
3           Anti-patterns explained                            15,99                 8
6           Cloud for dummies                                  44,44                 3

(4 row(s) affected)
```

Artık her iki tablonun verileri de eş.

Gece yayınevlerinden son listeleri alan servis çalıştığında Store tablosunda yapılan değişiklikler, yukarıdaki sorgu sayesinde Book tablosuna da yansıtılacak ve o günün bayilerinin bakacağı asıl içerik eşleştirilmiş olacak. Bu senaryoyu bir düşünüp kurgulamaya çalışın derim. Görüldüğü üzere merge esasında oldukça pratik bir kullanıma sahip ve birleştirme senaryoları için ideal. Pek tabii kurumun iş kuralları gereği bir merge işlemi her zaman için buradaki using ifadesi kadar sade olmayabilir. Örneğimizde doğrudan primary key alanlar üzerinden bir eşleşme yaptık ancak farklı senaryolar olduğu takdirde using ifadesine parantez açılıp daha karmaşık select ifadelerine ait sonuçların kaynak olarak gösterilmesi de sağlanabilir. Lakin maliyeti yüksek olduğu için kaçınmaya çalıştığımız çeşitli sorguları (sub query'ler, çok sayıda tablolu join'ler vb) buraya almanın çok önemli bir pozitif katkısı olmayabilir. Sonuç itibariyle büyük veri kümelerini kullanarak performans testlerini yapmakta ve execution planlara bakıp gerekli müdahaleleri yapmakta yarar var. Bizim senaryomuz için çalışma zamanı planlarına baktığımızda en azından üç iş yerine tek seferlik bir maliyetin altına girdiğimizi görebiliriz.

İlk uygulama biçimimiz için aşağıdaki gibi bir plan oluşur.

![sqlmerge_1.gif](images/sqlmerge_1.gif)

Table Spool maliyetleri biraz yüksek görüldüğü üzere. Merge çalışma planında ise durum aşağıdaki gibidir. Şekilde görülmese de %25lik bir Full Outer Join maliyeti var.

![sqlmerge_2.gif](images/sqlmerge_2.gif)

İşin aslı konuyu SQL performans yönetimi konusunda uzman birisinin incelemesi daha doğru olabilir. Genellikle şirketlerin veritabanı operasyon ekipleri perfomans arttırımı gerektiren sorgular için destek oluyorlar. Yine de iş oraya gelmeden önce gerekli ön tedbirleri alıp performans iyileştirmelerini yapmak da biz geliştiricilere düşen önemli bir görevdir. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
