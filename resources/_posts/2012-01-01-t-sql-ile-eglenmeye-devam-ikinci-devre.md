---
layout: post
title: "T-SQL ile Eğlenmeye Devam(İkinci Devre)"
date: 2012-01-01 14:06:00 +0300
categories:
  - t-sql
tags:
  - t-sql
---
Hatırlayacağınız üzere geçtiğimiz günlerde kafayı T-SQL ile bozmuş ve can sıkıntısından eğlenceli ifadeler yazmaya çalışmıştım. Sanırım söz konusu bu eğlence sonraki günlere de sirayet etti ve yine bir kaç eğlenceli T-SQL sorgusu ile karşınızdayım (İnsan ne oldum dememeli ne olacağım demeli belki de…Ben ki SQL’ den nefret eden bir birey olarak bu hale geldiysem… ![Smile](/assets/images/2012/wlEmoticon-smile_26.png))

[![fun2](/assets/images/2012/fun2_thumb.jpg)](/assets/images/2012/fun2.jpg)


Aslında hiç vakit kaybetmeden sorgularımızı incelemeye başlayalım dilerseniz. Elbetteki yine merak ettiğim ve aklıma gelen bazı ihtiyaçlar dahilinde bu sorgular ortaya çıkmakta. Örneğin sakin sakin otururken ilk aklıma gelen T-SQL tarafında bizim söyleyeceğimiz bazı kriterlere göre rastgele şifre üretecek bir fonksiyon yazmak oldu

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

Bunun için aşağıdaki T-SQL betiğini yazdım.

```text
Use AdventureWorks 
Go 
-- Function içerisinde Rand() fonksiyonunu kullanamayız(Invalid use of side-effecting or time-dependent operator in 'rand' within a function.) hatası alırız. Bu yüzden bir hile yapacağız ve rastgele sayıyı bir view içerisinden alacağız ;) Steve Kass' ın güzide çözümlerinden birisidir. 

create view ViewRandomNumbers 
as 
   select rand( ) as Number 
go 

create Function ufnGeneratePassword( 
   @PasswordLength int -- Kaç karakterlik password oluşturacağız 
   ,@StartChar tinyint -- başlangıç karakterinin ascii karşılığı sayısal değeri 
   ,@CharRange tinyint -- Son karakterin ascii karşılığı sayısal değeri 
   ,@ExcludedChars varchar(50) -- şifre içerisinde bulunmaması istenen karakterler 
   ) 
returns varchar(50) 
as 
begin 
   Declare @Password varchar(50)='' 
   Declare @char char -- Belirtilen aralıkta üretilen karakteri tutan değişken

   while @PasswordLength> 0 begin 
       -- Önce @StartChar' dan itibaren @CharRange mesafesine kadarlık bir alan içerisinde rastgele bir char üretilir 
      select @char = char(round((Select Number from dbo.ViewRandomNumbers) * @StartChar + @CharRange, 0)) 
      -- şifrede bulunması istenmeyen karakter olup olmama durumuna göre şifre üretilir ve sayac 1 azaltılır 
      if charindex(@char, @ExcludedChars) = 0 begin 
          set @Password =@Password+ @char 
          set @PasswordLength = @PasswordLength - 1 
     end 
   end

   return(@Password)

end 
Go
```

Burada tanımlamış olduğumuz ufnGeneratePassword isimli fonksiyon parametre olarak üretilecek şifre uzunluğunu, bu şifrenin ASCII tablosundaki hangi değer aralığında olacağını ve şifre içerisinde olmasını istemediğimiz karakterleri almaktadır. Fonksiyon kendi içerisinde söz konusu ASCII değer aralığında bir üretim gerçekleştirmek için de, rastgele sayı üretme işini üstlenen bir View’ dan yararlanmaktadır. Fonksiyonu aşağıdaki T-SQL ifadesinde olduğu gibi kullanıp bir kaç kez test ettiğimizde başarılı sonuçlar elde ettiğimizi görebiliriz.

```text
Declare @Password1 nvarchar(10) 
Set @Password1= dbo.ufnGeneratePassword(10,65,29,'abcdefg') 
Select @Password1 [Password]

Declare @Password2 nvarchar(10) 
Set @Password2= dbo.ufnGeneratePassword(10,30,50,'/.+-|@') 
Select @Password2 [Password]

Declare @Password3 nvarchar(10) 
Set @Password3= dbo.ufnGeneratePassword(10,30,150,'0?*/&^#>é!') 
Select @Password3 [Password]
```

[![artcl_4_1](/assets/images/2012/artcl_4_1_thumb.gif)](/assets/images/2012/artcl_4_1.gif)

Bu ilginç ama bana göre oldukça işe yarayacak T-SQL ifadesinden sonra bir başkası ile devam edelim. Söz gelimi veritabanınızda yer alan belirli bir Şemaya (schema) ait tablolarınızı yeni bir schema adına taşımak istiyorsunuz (İstemem demeyin ![Smile](/assets/images/2012/wlEmoticon-smile_26.png)) Bu durumda ne yaparsınız?

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

Aslında geçtiğimiz yazımızdaki örneklerimizde kullandığımız system view nesnelerini göz önüne alırsak; öncelikli olarak ilgili şemadaki tabloları bulmamız ve her biri için dinamik bir T-SQL ifadesini yürütmemiz gerektiği ortadadır. Temel olarak şema transferi için aşağıdaki gibi bir T-SQL ifadesi kullanılabilir.

```text
alter schema YeniSchemaAdi transfer [HumanResources].[Employee]
```

Söz gelimi bu ifade ile Employee tablosunun HumanResources şemasından YeniSchameAdi şemasına transfer edilmesi sağlanmaktadır. Ancak işi zorlaştıran kısım bu T-SQL ifadesinin dinamik olarak oluşturulması ve yürütülmesi sırasında ortaya çıkmaktadır. Dolayısıyla bir cursor kullanımı ve söz konusu şemaya ait tablolar üzerinden dolaşılması, diğer yandan her bir tablo için ilgili şema transfer işini üstlenen T-SQL ifadesinin dinamik olarak oluşturulması ve bu ifadeninde dinamik olarak çalıştırılması gerekmektedir. Aynen aşağıdaki T-SQL betik bloğunda görüldüğü gibi

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

```text
-- Önce yeni bir schema üretelim 
create schema HumanResourcesNew 
go

declare @NewSchemaName sysname 
declare @CursorObject sysname 
declare @SqlExpression nvarchar(1000) 
set @NewSchemaName = quotename('HumanResourcesNew')

-- sys.objects içerisinde dolaşıp HumanResources şemasına ait tüm kullanıcı tanımlı tabloları dolaşacak bir Cursor açıyoruz 
declare crsr cursor for select quotename([name])from sys.objects where schema_id = schema_id('HumanResources') and type in ('U') 
open crsr 
fetch from crsr into @CursorObject

    while @@fetch_status=0 begin 
        --her bir tablo için gerekli şema transfer etme T-SQL ifadesini üretiyoruz 
        set @SqlExpression = 'alter schema '+@NewSchemaName+' transfer [HumanResources].'+@CursorObject 
        print @SqlExpression 
        -- üretilen T-SQL ifadesini sp_executeSQL Stored Procedure' ü yardımıyla çalıştırıyoruz 
        exec sp_executeSQL @SqlExpression 
    fetch next from crsr into @CursorObject 
end

close crsr 
deallocate crsr
```

bu T-SQL ifadesini yürüttüğümüzde HumanResources şemasındaki kullanıcı tanımlı tabloların, HumanResourcesNew şemasına taşındığını görürüz.

[![artcl_4_2](/assets/images/2012/artcl_4_2_thumb.gif)](/assets/images/2012/artcl_4_2.gif)

Sizi bilmem ama ben çok eğleniyorum. Hız kesmeden farklı bir T-SQL ifadesi ve ihtiyacı ile devam edelim. Bu kez merak ettiğim şuydu. Acaba sistemimde ki veritabanlarında yer alan toplam kullanıcı tanımlı tablo sayıları ne kadardı?

Tüm veritabanlarını gezmek için yine sistem SP’ lerinden birisi olan sp_MSforeachdb’ den yararlanabilirdim. Hatta daha önceden yaptığımız gibi bir temp tablo kullanıp tüm sonuçları buraya da aktarabilirdim. Hımm…Beni biraz uğraştıran bir sorgu oldu aslında. Nitekim toplam tablo sayısını bulmak için öncelikli olarak her bir veritabanı bağlantısı altında çalıştırılacak T-SQL ifadeleri gerekiyordu. Bir başka deyişle yine dinamik olarak üretilecek ve her bir veritabanı için çalıştırılacak bir T-SQL ifadesi söz konusuydu

![Confused smile](/assets/images/2012/wlEmoticon-confusedsmile_15.png)

Ancak biraz uğraştıktan ve epey bir hata aldıktan sonra aşağıdaki T-SQL sorgusunu yazmayı başarabilmiştim.

```text
-- Önce veritabanı adı ve buradaki toplam tablo sayısını tutacak olan Temp tabloyu üretelim 
Create Table #AllTables 
( 
   DbName varchar(50) 
   ,TableCount int 
)

--sp_Msforeachdb SP' sinden yararlanarak tüm veritabanlarını dolaşalım 
EXEC sp_MSforeachdb ' 
USE 
?

Declare @TableCount int 
Set @TableCount=(Select Count(name) from sys.objects where type=''U'')

Insert into #AllTables Values (''?'',@TableCount) 
' -- Her bir veritabanı için USE ile o veritabanı alanına geçiyor ve sys.objects' den yararlanarak toplam tablo sayılarını bulup @TableCount isimli değişkende tuttuğumuz bu sayıları ve güncel veritabanı adını insert sorgusu ile temp tabloya alıyoruz

Select * from #AllTables order by TableCount desc

Drop Table #AllTables
```

ve işte sonuçlar

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

[![artcl_4_3](/assets/images/2012/artcl_4_3_thumb.gif)](/assets/images/2012/artcl_4_3.gif)

Merak ettiğim bir diğer konu ise Insert işlemlerine ilişkindi. Bazı hallerde bir Insert işlemi gerçekleştirildiğinde, insert edilen verilerin başka bir tabloya da aktarılması istenebilir. Söz gelimi bir tablo için gerçekleştirilen Insert işlemi sırasında, History bilgisini tutan başka bir tabloya da veri aktarımı yapılması sırasında... Burada aslında output anahtar kelimesi ve Inserted elemanının kullanıldığı bir ifade dizimi söz konusudur. Çoğumuz Insert işlemini bu tip bir şekilde çok fazla kullanmamışızdır eminim ki

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

Senaryo gereği OziRestoran isimli bir veritabanı oluşturup üzerine Siparis ve SiparisTarihce isimli iki tablo ekledim.

```text
Create database OziRestoran 
go

Create table Siparis 
( 
    SiparisId int identity(1,1) primary key 
    ,Aciklama nvarchar(250) 
    ,Tarih date 
) 
Create table SiparisTarihce 
( 
    SiparisTarihceId int identity(1,1) primary key 
    ,SiparisId int 
    ,Aciklama nvarchar(250) 
    ,Tarih date 
    ,Onaylayan nvarchar(20) 
) 
Go
```

Insert işlemimizde şunu yapmak istediğimizi düşünelim; Siparis tablosuna bir satır eklenirken, üretilen otomatik SiparisId, Aciklama ve Tarih alanları değerlerinin, siparisi onaylayan kişi bilgisi ile birlikte tarihçe tablosuna yazdırılmasını istiyoruz

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

İşte Insert sorgumuz.

```text
Use OziRestoran 
Go 
insert into Siparis( Aciklama, Tarih ) 
output inserted.SiparisId, inserted.Aciklama,inserted.Tarih,'bsenyurt' 
    into SiparisTarihce 
   ( 
        SiparisId, 
        Aciklama, 
        Tarih, 
        Onaylayan 
    ) 
values ( 'Bir adet LG marka laptop sipariş edildi',GETDATE()) 
Go

Select * from Siparis 
Select * from SiparisTarihce
```

Görüldüğü üzere Insert ifadesi yazılırken output anahtar kelimesinden itibaren SiparisTarihce içerisine de veri aktarımının yapılacağı belirtilmektedir. Sonrasında values anahtar kelimesini takip eden kısımda, asıl Siparis tablosu için eklenecek içerik set edilmektedir. Sonuçlar aşağıdaki gibi olacaktır.

[![artcl_4_4](/assets/images/2012/artcl_4_4_thumb.gif)](/assets/images/2012/artcl_4_4.gif)

Hazır Insert işlemlerinden konu açılmışken acaba içeriğini rastgele test verisi ile dolduracağımız devasa boyutlu tabloları nasıl oluşturabiliriz sorusu aklıma geldi

![Smile](/assets/images/2012/wlEmoticon-smile_26.png)

Aslında bu konuda bir önceki çalıştığım firmada Database Developer arkadaşlarımın yaptığı önemli çalışmalar vardı. Milyonlarca anlamlı veri yığını oluşturuyorlardı. Onların eline su dökemem belki ama en azından kendi çapımda bir şeyler yapabilirim diye düşündüm. İşe basit bir senaryo ile başladım. Örneğin rastgele Ad,Sodad,Şehir,Maaş ve Seviye bilgilerinden oluşacak bir veri tablosunu üretmeye çalıştım. Bu amaçla aşağıdaki gibi bir sorgu oluşturdum.

```text
Create Table Adlar 
( 
    Ad nvarchar(50) 
) 
Create Table Soyadlar 
( 
    Soyad nvarchar(50) 
) 
Create Table Sehirler 
( 
    Sehir nvarchar(50) 
)

Go 
Insert into Adlar values ('Burak') 
Insert into Adlar values ('Kamil') 
Insert into Adlar values ('Burcu') 
Insert into Adlar values ('Elif') 
Insert into Adlar values ('Sinem') 
Insert into Adlar values ('Hakan') 
Insert into Adlar values ('Bill') 
Insert into Adlar values ('Murat') 
Insert into Adlar values ('Nazım') 
Insert into Adlar values ('Cansu')

Insert into Soyadlar values ('Şenyurt') 
Insert into Soyadlar values ('Kırmızı') 
Insert into Soyadlar values ('Sucu') 
Insert into Soyadlar values ('Salimoğlu') 
Insert into Soyadlar values ('Arabacı') 
Insert into Soyadlar values ('Kısakol') 
Insert into Soyadlar values ('Odabaşı') 
Insert into Soyadlar values ('Şamil') 
Insert into Soyadlar values ('Limoncu') 
Insert into Soyadlar values ('Kurtaran')

Insert into Sehirler values ('İstanbul') 
Insert into Sehirler values ('İzmir') 
Insert into Sehirler values ('Ankara') 
Insert into Sehirler values ('Eskişehir') 
Insert into Sehirler values ('Trabzon') 
Insert into Sehirler values ('Antalya') 
Insert into Sehirler values ('Gaziantep') 
Insert into Sehirler values ('Manchester') 
Insert into Sehirler values ('New York') 
Insert into Sehirler values ('Samsun') 
Insert into Sehirler values ('Aydın') 
Insert into Sehirler values ('Moskova')

select 
    Ad 
    ,Soyad 
    ,Sehir 
    ,Maas=ROUND(ABS(CHECKSUM(NEWID()))/10000,0) 
    ,Level=ABS(CHECKSUM(NewId())) % 14 
    into PersonelTestTable FROM Adlar  
        cross join Soyadlar  
        cross join Sehirler 
    
Select * From PersonelTestTable
```

Şimdi burada işin püf noktası Adlar, Soyadlar ve Sehirler tablolarının CROSS JOIN ile birleştirilmesi ve PersonelTestTable içerisine atılması işlemidir. Çok doğal olarak ortaya 1200 satırlık (10 Ad X 10 Soyad X 12 Şehir) veri kümesi çıkacaktır

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

[![artcl_4_5](/assets/images/2012/artcl_4_5_thumb.gif)](/assets/images/2012/artcl_4_5.gif)

Eğer kombinasyon sayısını arttırırsanız kısa sürede milyonlarca satırdan oluşabilecek devasa test verileri üretebilirsiniz. Örneği geliştirmek sizin elinizde

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

Böylece geldik bir yazımızın daha sonuna. Bu yazımızda sadece 5 çeşit T-SQL ifadesine değindik ancak inanıyorum ki ilerleyen zamanlarda bunlara yenilerini ekliyor olacağım. Çünkü bu iş çok eğlenceli olmaya başladı. Merak işte

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_77.png)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[EglencelikSQL_2.sql (11,65 kb)](/assets/files/2012/EglencelikSQL_2.sql)