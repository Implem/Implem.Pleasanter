select sys.tables.name, sys.columns.name as "ColumnName", sys.types.name as "TypeName", sys.columns.max_length, convert(varchar, sys.columns.precision) + ',' + convert(varchar, sys.columns.scale) as "Size", sys.columns.is_nullable, sys.columns.is_identity
from sys.columns inner join sys.tables on
sys.columns.object_id = sys.tables.object_id 
inner join sys.types on
sys.columns.user_type_id = sys.types.user_type_id
where sys.tables.name = '#TableName#'
order by sys.tables.name, sys.columns.column_id;