select
    "index_name" as "Name",
    "column_name" as "ColumnName",
    "seq_in_index" as "No",
    case
        when "collation" = 'A' then 'asc'
        when "collation" = 'D' then 'desc'
        else ''
    end as "OrderType"
from "information_schema"."statistics"
where "table_schema" = '#InitialCatalog#' and "table_name" = '#TableName#';