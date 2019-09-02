select table_name
from information_schema.columns
where table_schema = 'public' and table_name = '#TableName#';