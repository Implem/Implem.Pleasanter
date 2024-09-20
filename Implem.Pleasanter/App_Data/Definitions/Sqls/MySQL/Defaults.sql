select "table_name", "column_name", "column_default", "data_type"
from "information_schema"."columns"
where "table_schema" = '#InitialCatalog#'
and "table_name" = '#TableName#'
and "column_default" is not null;