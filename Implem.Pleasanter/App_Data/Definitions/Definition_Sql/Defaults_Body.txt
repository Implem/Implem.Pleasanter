select "table_name", "column_name", "column_default"
from "#InitialCatalog#"."information_schema"."columns"
where "table_name" = '#TableName#' and "column_default" is not null;