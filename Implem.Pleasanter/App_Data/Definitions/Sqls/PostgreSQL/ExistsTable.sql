select table_name
from information_schema.tables
where table_schema = '#SchemaName#' and table_name = '#TableName#';