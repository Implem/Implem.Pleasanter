select 
    table_name as "name",
    column_name as "ColumnName",
    udt_name as "TypeName",
    case
        when udt_name = 'int4' then 4
        when udt_Name = 'text' then - 1
        when udt_name = 'timestamp' then 8
        when udt_name = 'bool' then 1
        else character_octet_length
    end as "max_length",
    case
        when numeric_precision IS NULL then '0,0'
        else (numeric_precision || ',' || numeric_scale)
    end as "Size",
    case
        when is_nullable = 'YES' then 1
        else 0
    end as "is_nullable",
    case
        when is_identity = 'YES' then 1 
        else 0
    end as "is_identity"
from information_schema.columns
where table_schema = 'public'
and table_name = '#TableName#'
order by table_name, ordinal_position;
