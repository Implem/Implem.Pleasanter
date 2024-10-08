select "index_name"
from "information_schema"."statistics"
where "table_schema" = '#InitialCatalog#'
and "table_name" = 'Items'
and "index_name" = 'ftx';