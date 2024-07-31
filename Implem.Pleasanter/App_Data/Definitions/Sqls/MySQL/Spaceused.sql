select concat(cast(round((sum(data_length + index_length) /1024 /1024),2) as char),' MB') as "database_size"
from information_schema.tables
where table_schema = '#InitialCatalog#';