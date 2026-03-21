---
layout: post
title: "T-SQL ile Dinlenmece Eğlenmece"
date: 2011-12-14 16:25:00 +0300
categories:
  - t-sql
tags:
  - sql
  - stored-procedures
  - newid
  - transact-sql
  - cursor
  - database
  - adventure-works
  - sql-server
  - temp-table
---
Hiç canınızın sıkıldığı ve böyle bir buhran anına girdiğinizde SQL Server Management Studio'yu açıp T-SQL ile eğlenceli bir şeyler yapmaya çalıştığınız oldu mu?

[![fun](/assets/images/2011/fun_thumb.jpg)](/assets/images/2011/fun.jpg)


![Smile](/assets/images/2011/wlEmoticon-smile_25.png)

Açıkçası geçtiğimiz günlerde böyle sıkkın ve bıkkın bir ruh halindeyken ve konuşmak istediğim tüm arkadaşlarım yoğunken, ekranımda duran Management Studio'daki bembeyaz ve bomboş Query penceresi ile muhabbet etmeye karar verdim. Aslında amacım basitti. Daha önceki tecrübelerime dayanarak ihtiyaçlar dahilinde kullandığım T-SQL ifadelerini şöyle bir tekrar etmeye çalışacak ve siz değerli okurlarıma bir blog girdisi olarak sunacaktım. Aklıma geldikçe ihtiyaçlarımın T-SQL karşılıklarını yazmaya başladım. Düşündüğüm ilk gereksinim, sistemimde yüklü olan kaç veritabanı olduğunu ve bunlara ait bazı temel bilgileri edinmekti...İşte serüvenimiz bu ilk sorgumuz ile başlıyor.

```text
select 
    database_id [Id] 
    ,name [Database Name] 
    ,create_date [Create Date] 
    ,Case [compatibility_level] 
        when '60' then 'SQL Server 6.0' 
        when '65' then 'SQL Server 6.5' 
        when '70' then 'SQL Server 7.0' 
        when '80' then 'SQL Server 2000' 
        when '90' then 'SQL Server 2005' 
        when '100' then 'SQL Server 2008' 
        else 'unknown' 
    end as [Compatibility Level] 
    ,collation_name [Collation] 
    ,Case is_fulltext_enabled 
        when 1 then 'Enabled' 
        else 'Disabled' 
    end as [FullText] 
    ,user_access_desc [User Access] 
    ,state_desc [State] 
    ,snapshot_isolation_state_desc [Snapshot Isolation] 
    ,Case is_read_only 
        when 1 then 'Yes' 
        else 'No' 
    end as [Read Only] 
    ,Case is_broker_enabled 
        when 1 then 'Yes' 
        else 'No' 
    end as [Service Broker] 
from sys.databases 
order by [Database Name]
```

Yukarıdaki SQL sorgusunu kullanarak sistemde var olan veritabanlarına ait bazı temel bilgileri elde edebiliriz. Söz konusu ifadenin çalışma zamanındaki çıktısı aşağıdakine benzer olabilir. Bu sonuçlada pek tabi benim sistemimde yer alan veritabanları ve onlara ait bilgileri bulunmaktadır. (Ekran çıktısının orjinal halini görmek içim fotoğrafa tıklayın)

[![artcl_3_1](/assets/images/2011/artcl_3_1_thumb.gif)](/assets/images/2011/artcl_3_1.gif)

Görüldüğü üzere sistemimde var olan veritabanlarının adlarını, oluşturulma zamanlarını, SQL Server uyumluluk sürümlerini, hangi Collation’ ı kullandıklarını ve bunlara benzer bilgilerini elde etmiş bulunuyoruz (Doğruyu söylemek gerekirse SQL tarafında sys ön eki ile başlayan View’ lar içerisinde inanılmaz sürprizler bulumaktadır. İncelemediyseniz bile araştırmanızı şiddetle öneririm![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_76.png))

Gelelim sıradaki sorgumuza. Bu sefer sistemimde kullanıcı tanımlı ne kadar table, stored procedure, function, view ve trigger varsa, şema adları (Schema Name) ile birlikte elde etmek istedim. Bunun içinde yine sys ön ekli view'lardan yararlanabiliriz. İşte sorgumuz;

```text
select 
    S.name+'.'+O.name [Object] 
    ,object_id [Id] 
    ,type 
    ,type_desc 
    ,create_date [Create Date] 
    ,modify_date [Modify Date] 
from sys.all_objects O 
join sys.schemas S on O.schema_id=S.schema_id 
where type in ('U','V','TR','FN','P') 
order by [Object]
```

Dikkat edileceği üzere sys.schema ve sys.allobjects isimli sistem görünümlerinden yararlanmaktayız. sys.allobjects tüm veritabanı nesnelerini tutan bir görünüm sunmaktadır. Aslında sadece belirli bir veritabanı bağlantısı ile ilişkili olan nesnelere gitmek istersek sys.objects görünümünden de yararlanabiliriz. Söz konusu T-SQL ifademizin benim sistemimde ürettiği sonuçlar ise aşağıdaki gibidir.

[![artcl_3_2](/assets/images/2011/artcl_3_2_thumb.gif)](/assets/images/2011/artcl_3_2.gif)

Eğlenceli değil mi?

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_76.png)

O zaman hıs kesmeden devam edelim. Çalıştığım sırada aklıma gelen ve merak ettiğim sorgulardan birisi de şuydu: Acaba sistemimde yer alan tablolarda kullanılan alanların (Columns) tablo bazlı toplam sayıları neydi? Çok doğal olarak hangi tabloda kaç alan kullanıldığını bilmek istiyordum. Bunun için basit bir SQL ifadesi yeterli olacaktı. Aynen aşağıdaki gibi.

```text
select 
    T.Name 
    ,Count(C.column_id) [Total Column Count] 
    from sys.tables T 
    join sys.columns C on T.object_id=C.object_id 
    where T.type='U' 
group by T.Name 
order by Count(C.Column_id) desc
```

Bu kez sys.tables ve sys.columns view nesnelerini ele alıp ve kullanıcı tanımlı tabloları adlarına göre gruplandırarak sonuca ulaşmaya çalıştım. Tabi kendi sistemimde bu tabloyu yürüttüğümde aşağıdaki ekran çıktısında yer alan sonuçları elde ettim.

[![artcl_3_3](/assets/images/2011/artcl_3_3_thumb.gif)](/assets/images/2011/artcl_3_3.gif)

Tabi kendi sistemimde gayet makul seviyelerde rakamlara ulaştığımı ifade edebilirim. Nasıl ki kod yazarken bazı metrikleri uyguluyor ve örneğin satır sayısı 25i geçen metodları tespit edip kod standartları açısından denetlemeler yapıyoruz, benzer şekilde SQL tarafında da bu tip metrikleri uygulayabiliriz. Bu sorgu söz konusu metriklerden birisi olarak düşünülebilir. Tabi çalışmakta olduğum bankada aynı sorguyu denediğim de piuvvvvv

![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_20.png)

Ehem…Ehem…Tekrar sistemime döneyim.

Bu kez aklımda şöyle bir soru vardı: Acaba sistemimde yer alan AdventureWorks veritabanında, hangi Stored Procedure’ ler içerisinde Update anahtar kelimesi (Keyword) kullanılmaktaydı

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_76.png)

Bir başka deyişle hangi SP'ler içerisinde güncelleme ile ilişkili işlemler yapıldığını görmek istiyordum. Bu tip bir ihtiyaç pek çok durumda gerekebilir. Özellikle SQL tarafına yıkılmış iş süreçlerinde değişiklikler yapmanız gerektiği durumlarda kullanabileceğiniz bir tespit yöntemidir. Söz gelimi bir tablonun adının değişmesi sonucu ilgili SP'lerde de geçtiği yerlerde de pansumanlar yapmak gerekecektir (SQL Tarafında Visual Studio'da olduğu gibi Refactor-Rename özelliği olsaydı fena olmazı aslında) Bunu öğrenmek için aşağıdaki sorguyu kullandım.

```text
Use AdventureWorks 
Go 
select 
    SPECIFIC_CATALOG 
    ,SPECIFIC_SCHEMA+'.'+SPECIFIC_NAME [SP NAME] 
    ,ROUTINE_DEFINITION 
from INFORMATION_SCHEMA.ROUTINES
where ROUTINE_TYPE='PROCEDURE' and ROUTINE_DEFINITION like '%UPDATE%'
```

Görüldüğü üzere farklı bir View içerisinde Stored Procedure’ lerin T-SQL içerikleri de tutulmaktadır. Sorgunun sonucu olarak aşağıdaki ekran görüntüsünde yer alan çıktıları elde ettim. (Size bir antrenman önerebilirim. Eğer çok sayıda veritabanı ve çok sayıda SP ile karmaşık iş süreçlerini barındıran bir sistemde görev alıyorsanız, örneğin içerisinde @@IDENTITY, BEGIN TRANSACTION gibi kritik terimleri içeren SP’ leri araştırmayı deneyebilirisiniz ![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_76.png))

[![artcl_3_4](/assets/images/2011/artcl_3_4_thumb.gif)](/assets/images/2011/artcl_3_4.gif)

Aklıma sorgu geldikçe geliyordu. Karşımdaki SQL Query pencersi iyi bir arkadaştım. Ne sorsam cevap veriyordu (Yani sayılır). Gerçi bazen ırım kırım ediyor naz yapıyordu ama olsun

![Smile](/assets/images/2011/wlEmoticon-smile_25.png)

Şimdi merak ettiğim sorgu ise şuydu: Acaba bir veritabanında veya daha da iyisi tüm sistemde yer alan tabloların kapladıkları alanların boyut bilgileri nelerdi? Burada çözüme giderken biraz sıkıntı çektiğimi ifade etmek isterim. Query pencersi ile bir türlü mutabakat sağlayamadık. Önce tek ve herhangibir tablo için bunu öğrenmeye çalıştım. Bunun için tasarlanmış özel bir sistem SP’ si mevcuttu nitekim (Sanırım Tüme varım yöntemi ile hareket edeceğim)

```text
Use AdventureWorks 
Go 
sp_spaceused 'Production.Product'
```

Sonuç ise şöyleydi.

[![artcl_3_5](/assets/images/2011/artcl_3_5_thumb.gif)](/assets/images/2011/artcl_3_5.gif)

Şimdi işi bir adım daha ileri götürmeliydim. Çünkü asıl amacım sistemde ne kadar tablo varsa her birinin boyutsal özelliklerini öğrenmekti (Yani ne kadar alanı reserve ettikleri, bu alanın ne kadarını kullandıkları vb) Bunun içinde aslında bir for each ifadesini çalıştırmam gerekiyordu. Yani her bir tabloyu gezmeli ve her biri için spspaceused SP’ sini çalıştırmalıydım. Bu foreach içinde aslında sistem de yer alan güzel bir SP bulunmaktadır.

![Winking smile](/assets/images/2011/wlEmoticon-winkingsmile_76.png)

```text
EXEC sp_MSforeachtable @command1="EXEC sp_spaceused '?'"
```

ve işte sonuç;

[![artcl_3_6](/assets/images/2011/artcl_3_6_thumb.gif)](/assets/images/2011/artcl_3_6.gif)

Aslına bakarsanız istediğim bilgileri elde etmiştim. Ancak görüntü pek hoş değildi. Keşke tablo bazlı bir ızgara çıktısı (Grid View) elde edebilseydim

![Confused smile](/assets/images/2011/wlEmoticon-confusedsmile_14.png)

Ama çaresiz değildim. Biraz Cursor, biraz Temp tablo işimi görebilirdi pekala. Kolları sıvadım ve uzun bir uğraştan sonra aşağıdaki SQL ifadesini yazmayı başardım.

```text
Use AdventureWorks 
Go 
declare @TableName nvarchar(100) 
create table #TempTable 
( 
    [Table Name] nvarchar(100), 
    [Row Count] varchar(100), 
    [Reserved Size] varchar(50), 
    [Data Size] varchar(50), 
    [Index Size] varchar(50), 
    [Unused Size] varchar(50) 
)

declare tableCursor cursor forward_only 
for 
    select S.name+'.'+T.[name] 
    from sys.tables T 
    join sys.schemas S on T.Schema_id=S.Schema_id 
    where T.type='U' 
for read only

open tableCursor 
    while (1=1) 
    begin 
        fetch next from tableCursor into @TableName 
            if(@@FETCH_STATUS<>0) 
                break; 
            insert #TempTable exec sp_spaceused @TableName 
    end

close tableCursor 
deallocate tableCursor

select * from #TempTable Order by [Table Name] 
drop table #TempTable
```

Aslında teori basitti. Tablo ve Şema adlarını elde ettikten sonra her birisi için spspaceused SP’ ini çalıştıracak ama sonuçlarını bir Temp tabloya ekleyecektim. Şimdi sonuçlar ve elde edilen görüntü çok daha güzeldi.

[![artcl_3_7](/assets/images/2011/artcl_3_7_thumb.gif)](/assets/images/2011/artcl_3_7.gif)

Tam bu sorguyu bitirmiştim ki aklıma başka bir ihtiyaç geldi. Acaba sistemde yer alan tablo adlarının tamamını, aralarına virgül koyarak tek bir hücreye indirgiyebilir miydim? Hımm…Eğer kod tarafında olsaydık bu benim çocuk oyuncağı sayılırdı. Ama SQL özürlü birisi olarak biraz araştırma yapmam gerekecekti. Sonuçta COALESCE fonksiyonundan yararlanarak bu isteği karşılayabileceğimi gördüm. Nasıl mı?

```text
DECLARE @Names VARCHAR(8000) 
SELECT @Names = COALESCE(COALESCE(@Names + ',', '') + Name, @Names) 
    FROM sys.tables 
    where type='U' 
select @Names
```

ve sonuç

[![artcl_3_8](/assets/images/2011/artcl_3_8_thumb.gif)](/assets/images/2011/artcl_3_8.gif)

Query Explorer ile olan sohbetim harika ilerliyordu. Bu kez ondan bana sistem de yer alan veritabanlarının ne zaman yedeklendiğini (ve hatta yedeklenmediğini) söylemesini istiyordum. Aslına bakarsanız bu önemli bir sorguydu. Çünkü ilk çalıştırdığımda AdventureWorks için hiç bir zaman Backup almadığımı fark etmiştim

![Confused smile](/assets/images/2011/wlEmoticon-confusedsmile_14.png)

```text
SELECT 
    D.name [Database Name] 
    ,case when MAX(b.backup_finish_date) is NULL 
    then 'Bakcup Yok' 
    else Convert(varchar(100), MAX(b.backup_finish_date)) 
    end AS [Last Backup Time] 
FROM sys.databases D 
LEFT JOIN msdb.dbo.backupset B ON D.name = B.database_name AND B.type = 'D' 
WHERE D.database_id NOT IN (2) 
GROUP BY D.name 
ORDER BY [Database Name] DESC
```

Tabi burada anahtar nokta backupset içeriğinden yararlanılmasıydı. İlk sonuçlarda hiç backup almamış olduğumu görünce, hemen bir tane ürettirdim ve yeni sonuçlara baktığımda aşağıdaki ekran görüntüsünde yer alan çıktıyı elde ettim.

[![artcl_3_9](/assets/images/2011/artcl_3_9_thumb.gif)](/assets/images/2011/artcl_3_9.gif)

Derken aklıma biraz daha eğlenceli bir sorgu geldi. Söz gelimi çalışanlarımızdan (Hani o anda koca bir fabrikanın sahibi olduğunu düşündüm de) rastgele 5 farklı kişiyi getirip onlara hediye dağıtmak istediğimi düşündüm. (Bu o gün indirimli olarak satılacak rastgele 10 ürün de olabilirdi). Eğlenceli bir sorguydu. NewId () fonksiyonu burada işi eğlenceli hale getiren kişiydi. Örnek olarak Employee tablosu için şanslı 5 kişiyi bulmaya çalıştım.

```text
Select 
    Top 5 NewId() Id 
    ,EmployeeID 
    ,Title 
    ,BirthDate 
    ,ManagerID 
    ,VacationHours 
from HumanResources.Employee 
order by 1
```

ve aşağıdaki ekran görüntüsünde yer alan sonuçları elde ettim. Tabi ki her çalıştırılmada farklı sonuçlar elde edilmesi garantiydi.

[![artcl_3_10](/assets/images/2011/artcl_3_10_thumb.gif)](/assets/images/2011/artcl_3_10.gif)

Gerçi şimdi fark ettim ki 114 numaralı çalışan oldukça şanslıymış. Çünkü ilk iki sorguda tesadüfen çıkmış

![Smile](/assets/images/2011/wlEmoticon-smile_25.png)

Buna tabi bir tedbir almak gerektiği kanısındayım. Aslında bu tedbiri size bırakıyorum. En azından hediye çıkmış işçileri bir flag ile işaretlemeye veya farklı bir tabloda belirli süreliğine saklayarak tekrardan sorgu sonuçlarında çıkmalarını engellemeyi düşünebilirsiniz.

SQL sorgularını denediğim sırada arka planda çalışmakta olan diğer SQL penceresine gözüm ilişmişti. Aslında arada sırada oraya bakmak zorundaydım. Nitekim Test ortamında yer alan bir veritabanı üzerinde bazı işlemler yapılması gerekiyordu. Ne varki ilgili sistemde yer alan tablo 32 milyon satırlık veri içerdiğinden ve test makinesi nuhnebiden kalma bir Pentium III olduğundan miniminnacık sıkıntılar vardı. O anda aklıma acaba index kullanılmayan (örneğin Clustered Index) tablolar var mıdır acaba sorusu geldi? Hemen local sistemimde bunu araştırmak için aşağıdaki sorgu ifadesini hazırladım.

```text
select 
    S.name+'.'+T.name AS [TableName] 
from sys.tables T 
inner join sys.schemas S 
on S.schema_id = T.schema_id 
where OBJECTPROPERTY(OBJECT_ID,'TableHasClustIndex') =0 and T.Type='U' 
order by[TableName] ASC
```

Bir de ne göreyim

![Smile](/assets/images/2011/wlEmoticon-smile_25.png)

[![artcl_3_11](/assets/images/2011/artcl_3_11_thumb.gif)](/assets/images/2011/artcl_3_11.gif)

AdventureWorks veritabanındaki ProductProductPhoto tablosunda ClusteredIndex yok…Bak bak baaakk

![Smile](/assets/images/2011/wlEmoticon-smile_25.png)

Tabi bu işin şaka tarafı ama performans araştırmaları yaparken belki de işe yarayacak bir sorgu olarak düşünülebilir.

İşler gayet eğlenceli gidiyordu ama enerjim de bitmek üzereydi. Son olarak basit bir sorgu yardımıyla Query penceresi ile olan muhabbetime son vereyim istemiştim. Bu sefer merak ettiğim, çevrede var olan SQL sunucularının hangileri olduğuydu. Aşağıdaki sorgu bunu karşılıyordu.

```text
Select 
    server_id Id 
    ,name [Server Name] 
    ,product [Product Type] 
    ,provider [Provider Name] 
    ,data_source [Data Source] 
    ,catalog 
    ,case is_data_access_enabled 
    when 1 then 'Enabled' 
    else 'Disabled' 
    end as [Data Access] 
from sys.servers
```

İşte sonuçlar,

[![artcl_3_12](/assets/images/2011/artcl_3_12_thumb.gif)](/assets/images/2011/artcl_3_12.gif)

Elbette ben yerel makinemden sadece tek bir veri sunucusuna bağlandığımdan cılız bir sonuç çıkmıştı. Ancak aynı sorguyu arka planda çalışmakta olduğum test makinesinde yürüttüğümde piuvvvvv!!!

![Smile](/assets/images/2011/wlEmoticon-smile_25.png)

Buraya kadar yazılmış olan sorguları eğlence amaçlı olarak veya ciddi manada göz önüne alarak çalışmakta olduğunuz gerçek hayat SQL sunucuları üzerinde de deneyebilirsiniz. Çok ilginç sonuçlar elde edeceğinizi ama oldukça faydalı bilgiler alabileceğinizi belirtmek isterim.

Peki ya bundan sonrasında ne olacak? Aslında bakarsanız burada yazılmış olan pek çok SQL ifadesi birer View haline dönüştürülüp sunucu üzerindeki farklı bir veritabanında saklanabilirler. Hatta bu veritabanının karşılığı olan bir Entity Framework kütüphanesi üretilip ilgili raporların örneğin bir WCF Data Service yardımıyla dış ortama sunulması da sağlanabilir. Tabi hassas veriler söz konusu olduğundan bu pek de iyi bir fikir değildir. Ama dilerseniz basit bir WCF (Windows Communication Foundation) Servisini güvenli hale getirerek ilgili içerikleri dış dünyaya servis bazlı olarak sunabilirsiniz. Sanırım bir sonraki makalemde hangi konuyu/senaryoyu ele alacağımı anlamışsınızdır

![Open-mouthed smile](/assets/images/2011/wlEmoticon-openmouthedsmile_20.png)

Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[EglencelikSQL.sql (5,39 kb)](/assets/files/2011/EglencelikSQL.sql)