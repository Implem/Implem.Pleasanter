select "table_name", "column_name", "column_default"
from "information_schema"."columns"
where "table_schema" = '#InitialCatalog#'
and "table_name" = '#TableName#'
and "column_default" is not null;