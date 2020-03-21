insert into "#DestinationTableName#"(#DestinationColumnCollection#) select #SourceColumnCollection# from "#SourceTableName#";
alter table "#SourceTableName#" rename to "_Migrated_#DestinationTableName#";
alter table "#DestinationTableName#" rename to "#SourceTableName#";
select setval(pg_get_serial_sequence('"#SourceTableName#"', '#ColumnName#'), (select max("#ColumnName#") from "#SourceTableName#"));