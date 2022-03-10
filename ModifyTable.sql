select * into #table from INFORMATION_SCHEMA.TABLES where TABLE_NAME like 'r30$%'

while exists(select 1 from #table) BEGIN
    DECLARE @id VARCHAR(32)
    select top 1 @id = TABLE_NAME from #table
    exec('delete ' + @id + ' where 1 = 1')
    delete #table where TABLE_NAME = @id
end

drop table #table

/*

DECLARE @ngay_gh1 SMALLDATETIME, @ngay_gh2 SMALLDATETIME
SELECT @ngay_gh1 = ngay_gh1, @ngay_gh2 = ngay_gh2 FROM dmstt
IF @ngay_gh1 IS NULL OR @ngay_gh2 IS NULL RETURN
EXEC FastBusiness$App$Dynamic$AddTable 'r30$000000', 'r30$', '', 'PRIMARY', 1, @ngay_gh1, @ngay_gh2, 1

*/

/*

    DECLARE @q nvarchar(max)  
    select @q = '
    select stt_rec, client_code into #table from m05$%Partition
    while exists(select 1 from #table) BEGIN
        DECLARE @id VARCHAR(32), @client_code varchar(6)
        select top 1 @id = stt_rec, @client_code = client_code from #table
        exec dbo.rs_PostInputInvoice ''m05$%Partition'', ''d05$%Partition'', @client_code, @id, '''', ''E05'', ''1''
        delete #table where stt_rec = @id
    end
    drop table #table'

    select * into #table2 from INFORMATION_SCHEMA.TABLES where TABLE_NAME like 'm05$%'
    while exists(select 1 from #table2) BEGIN
        DECLARE @tb VARCHAR(32), @p nvarchar(max)
        select top 1 @tb = TABLE_NAME from #table2
        
        select @p = replace(@q, '%Partition', replace(@tb, 'm05$', ''))
        EXEC sp_executesql @p
        delete #table2 where TABLE_NAME = @tb
    end

    drop table #table2

*/
