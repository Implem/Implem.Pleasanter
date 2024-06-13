insert into "#DestinationTableName#"(#DestinationColumnCollection#) select #SourceColumnCollection# from "#SourceTableName#";
alter table "#SourceTableName#" rename to "_Migrated_#DestinationTableName#";
alter table "#DestinationTableName#" rename to "#SourceTableName#";