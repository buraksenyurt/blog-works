-- Tüm denemeler SQL Server 2008 tabanlý yerel makine üzerinden yapýlmýþtýr

-- 1 : Baðlý olunan sunucuda yüklü bulunan veritabanlarýna ait bazý temel bilgilerin elde edilmesi 
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
 
 -- 2 : Sistemde kullanýcý tanýmlý ne kadar tablo,stored procedure, function, view , trigger varsa, þema adlarý ile birlikte elde edilmeleri  
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

-- 3 : Sistemde yer alan tablolarda kullanýlan toplam Field sayýlarýnýn tespit edilmesi 
select 
    T.Name
    ,Count(C.column_id) [Total Column Count] 
    from sys.tables T 
    join sys.columns C on T.object_id=C.object_id 
    where T.type='U' 
group by T.Name
order by Count(C.Column_id) desc

-- 4 : Ýçeriðinde örneðin @@IDENTITY komutunu içeren Stored Procedure' lerin bulunmasý 
Use AdventureWorks
Go
select 
	SPECIFIC_CATALOG
	,SPECIFIC_SCHEMA+'.'+SPECIFIC_NAME [SP NAME]
	,ROUTINE_DEFINITION
 from INFORMATION_SCHEMA.ROUTINES Routines 
 where ROUTINE_TYPE='PROCEDURE' and ROUTINE_DEFINITION like '%UPDATE%'
 
-- 5 : sp_spaceused ile bir tablonun boyutsal olarak kullaným alan bilgilerinin elde edilmesi 
Use AdventureWorks
Go
sp_spaceused 'Production.Product'

-- Tabi bu noktada sistemde ne kadar tablo varsa bunlarýn boyutsal bilgilerini öðrenmek istiyor olabiliriz de ;)
-- 6 : Tablolar arasýnda en çok yer tutanlarýn tespit edilmesi 
EXEC sp_MSforeachtable @command1="EXEC sp_spaceused '?'"

-- 7 : Ama iþi daha da ,ileri götürebiliriz. Özellikle yukarýda çalýþan sorgunun ekran çýktýsý hoþumuza gitmediyse :) 
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

select * from #TempTable Order by [Table Name] drop table #TempTable

-- 8 : Sistemdeki kullanýcý tanýmlý tablo adlarýný tek bir hücre içerisinde aralarýnda virgül koyarak elde etmek 
DECLARE @Names VARCHAR(8000)
SELECT @Names = COALESCE(COALESCE(@Names + ',', '') + Name, @Names) 
    FROM sys.tables
    where type='U'
select @Names

-- 9 : Sistem yer alan veritabanlarýndan hangilerinin en son ne zaman yenilendiðinin ve hangilerinin hiç yedeklenmediðinin öðrenilmesi 
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

-- 10 : Bir tablodan rastgele alanlar çekmek. Örneðin günün hediye daðýtýlacak þanslý üyelerinin bulunmasýnda kullanabilir.
Select 
    Top 5 NewId() Id
    ,EmployeeID
    ,Title
    ,ManagerID
    ,VacationHours
from HumanResources.Employee
order by 1

Select 
    Top 5 NewId() Id
    ,EmployeeID
    ,Title
    ,ManagerID
    ,VacationHours
from HumanResources.Employee
order by 1

Select 
    Top 5 NewId() Id
    ,EmployeeID
    ,Title
    ,ManagerID
    ,VacationHours
from HumanResources.Employee
order by 1

-- 11 : Kullanýcý tanýmlý tablolarda Clustered Index kullanýlmayanlarýn öðrenilmesi.
select
    S.name+'.'+T.name AS [TableName]
from sys.tables T
inner join sys.schemas S
on S.schema_id = T.schema_id 
where OBJECTPROPERTY(OBJECT_ID,'TableHasClustIndex') =0 and T.Type='U'
order by[TableName] ASC

-- 12 : Çevresel sunucu bilgilerinin elde edilmesi.
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
 
 -- 13 : Rastgele Þifre Üretmek
 Use AdventureWorks
 Go
-- Function içerisinde Rand() fonksiyonunu kullanamayýz(Invalid use of side-effecting or time-dependent operator in 'rand' within a function.) hatasý alýrýz. Bu yüzden bir hile yapacaðýz ve rastgele sayýyý bir view içerisinden alacaðýz ;) Steve Kass' ýn güzide çözümlerinden birisidir.
--drop view ViewRandomNumbers
create view ViewRandomNumbers
as
   select rand( ) as Number
go
--drop function ufnGeneratePassword
create Function ufnGeneratePassword(
   @PasswordLength int -- Kaç karakterlik password oluþturacaðýz
   ,@StartChar tinyint -- baþlangýç karakterinin ascii karþýlýðý sayýsal deðeri
   ,@CharRange tinyint -- Son karakterin ascii karþýlýðý sayýsal deðeri
   ,@ExcludedChars varchar(50) -- þifre içerisinde bulunmamasý istenen karakterler
   )
returns varchar(50)
as
begin
   Declare @Password varchar(50)=''
   Declare @char char -- Belirtilen aralýkta üretilen karakteri tutan deðiþken

   while @PasswordLength> 0 begin
       -- Önce @StartChar' dan itibaren @CharRange mesafesine kadarlýk bir alan içerisinde rastgele bir char üretilir
      select @char = char(round((Select Number from dbo.ViewRandomNumbers) * @StartChar + @CharRange, 0))
      -- þifrede bulunmasý istenmeyen karakter olup olmama durumuna göre þifre üretilir ve sayac 1 azaltýlýr
      if charindex(@char, @ExcludedChars) = 0 begin
          set @Password =@Password+ @char
          set @PasswordLength = @PasswordLength - 1
      end
   end

   return(@Password)

end
Go
Declare @Password1 nvarchar(10)
Set @Password1= dbo.ufnGeneratePassword(10,65,29,'abcdefg')
Select @Password1 [Password]

Declare @Password2 nvarchar(10)
Set @Password2= dbo.ufnGeneratePassword(10,30,50,'/.+-|@')
Select @Password2 [Password]

Declare @Password3 nvarchar(10)
Set @Password3= dbo.ufnGeneratePassword(10,30,150,'0?*/&^#>é!')
Select @Password3 [Password]

--14 : Belirli bir þemadaki tablolarýn þema adlarýnýn deðiþtirilmesi

-- Önce yeni bir schema üretelim
create schema HumanResourcesNew
go

declare @NewSchemaName sysname
declare @CursorObject sysname
declare @SqlExpression nvarchar(1000)
set @NewSchemaName = quotename('HumanResources')

-- sys.objects içerisinde dolaþýp HumanResources þemasýna ait tüm kullanýcý tanýmlý tablolarý dolaþacak bir Cursor açýyoruz
declare crsr cursor for select quotename([name])from sys.objects where schema_id = schema_id('HumanResourcesNew') and type in ('U')
open crsr
fetch from crsr into @CursorObject

	while @@fetch_status=0 begin
		--her bir tablo için gerekli þema transfer etme T-SQL ifadesini üretiyoruz
		set @SqlExpression = 'alter schema '+@NewSchemaName+' transfer [HumanResourcesNew].'+@CursorObject
		print @SqlExpression
		-- üretilen T-SQL ifadesini sp_executeSQL Stored Procedure' ü yardýmýyla çalýþtýrýyoruz
		exec sp_executeSQL @SqlExpression
	fetch next from crsr into @CursorObject
end

close crsr
deallocate crsr

--15 : sp_MSforeachdb sistem SP' sini kullanýp tüm veritabanlarýnda gezmek ve veritabaný baþýna düþen toplam tablo sayýlarýný bir Temp tablo içerisinde toplamak
-- Önce veritabaný adý ve buradaki toplam tablo sayýsýný tutacak olan Temp tabloyu üretelim
Create Table #AllTables
(
   DbName varchar(50)
   ,TableCount int
)

--sp_Msforeachdb SP' sinden yararlanarak tüm veritabanlarýný dolaþalým
EXEC sp_MSforeachdb '
USE
?

Declare @TableCount int
Set @TableCount=(Select Count(name) from sys.objects where type=''U'')

Insert into #AllTables Values (''?'',@TableCount)
' -- Her bir veritabaný için USE ile o veritabaný alanýna geçiyor ve sys.objects' den yararlanarak toplam tablo sayýlarýný bulup @TableCount isimli deðiþkende tuttuðumuz bu sayýlarý ve güncel veritabaný adýný insert sorgusu ile temp tabloya alýyoruz

Select * from #AllTables order by TableCount desc

Drop Table #AllTables

--16 Insert iþlemleri sýrasýnda Output kullanmak
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

Use OziRestoran
Go
INSERT INTO Siparis( Aciklama, Tarih )
OUTPUT INSERTED.SiparisId, Inserted.Aciklama,Inserted.Tarih,'bsenyurt'
	INTO SiparisTarihce
	(
		SiparisId,
		Aciklama,
		Tarih,
		Onaylayan
	)
VALUES ( 'Bir adet LG marka laptop sipariþ edildi',GETDATE())
Go

Select * from Siparis
Select * from SiparisTarihce

--17 Rastgele veri üretmek
Use OziRestoran
Go

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
Insert into Adlar values ('Nazým')
Insert into Adlar values ('Cansu')

Insert into Soyadlar values ('Þenyurt')
Insert into Soyadlar values ('Kýrmýzý')
Insert into Soyadlar values ('Sucu')
Insert into Soyadlar values ('Salimoðlu')
Insert into Soyadlar values ('Arabacý')
Insert into Soyadlar values ('Kýsakol')
Insert into Soyadlar values ('Odabaþý')
Insert into Soyadlar values ('Þamil')
Insert into Soyadlar values ('Limoncu')
Insert into Soyadlar values ('Kurtaran')

Insert into Sehirler values ('Ýstanbul')
Insert into Sehirler values ('Ýzmir')
Insert into Sehirler values ('Ankara')
Insert into Sehirler values ('Eskiþehir')
Insert into Sehirler values ('Trabzon')
Insert into Sehirler values ('Antalya')
Insert into Sehirler values ('Gaziantep')
Insert into Sehirler values ('Manchester')
Insert into Sehirler values ('New York')
Insert into Sehirler values ('Samsun')
Insert into Sehirler values ('Aydýn')
Insert into Sehirler values ('Moskova')

SELECT 
	Ad
	,Soyad
	,Sehir
	,Maas=ROUND(ABS(CHECKSUM(NEWID()))/10000,0)
	,Level=ABS(CHECKSUM(NewId())) % 14
	INTO PersonelTestTable FROM Adlar 
		CROSS JOIN Soyadlar 
		CROSS JOIN Sehirler 
	
Select * From PersonelTestTable

--Drop table Adlar
--Drop table Soyadlar
--Drop table Sehirler
--Drop table PersonelTestTable

