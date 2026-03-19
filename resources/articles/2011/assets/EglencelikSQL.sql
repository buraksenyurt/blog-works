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
