DECLARE @table_name nvarchar(256)
DECLARE @col_name nvarchar(256)
DECLARE @Command  nvarchar(1000)

SET @table_name = N'Athletes'
SET @col_name = N'ExternalId'

SELECT @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + d.name
    FROM sys.tables t
    JOIN sys.indexes d ON d.object_id = t.object_id AND d.type = 2 AND d.is_unique=1
    JOIN sys.index_columns ic on d.index_id = ic.index_id AND ic.object_id = t.object_id
    JOIN sys.columns c on ic.column_id = c.column_id AND c.object_id = t.object_id
    WHERE t.name = @table_name AND c.name = @col_name;

EXEC sp_executesql @Command;