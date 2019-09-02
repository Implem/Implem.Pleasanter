select table_name as "table_name", column_name as "column_name", column_default as "column_default"
from information_schema.columns
where table_name = '#TableName#' and column_default is not null;