select "table_name"
from "information_schema"."tables"
where "table_schema" = '#InitialCatalog#' and "table_name" = '#TableName#';