do
$$
declare
    r record;
begin
    for r in select  schemaname,tablename from pg_tables where tableowner='#Oid#' and schemaname='#SchemaName#'
    loop
        execute 'grant select, insert, update, delete on table "' || r.schemaname || '"."' || r.tablename || '" to "#Uid#"';
    end loop;
end
$$;
