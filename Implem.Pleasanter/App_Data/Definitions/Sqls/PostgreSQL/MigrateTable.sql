insert into "#DestinationTableName#"(#DestinationColumnCollection#) select #SourceColumnCollection# from "#SourceTableName#";
alter table "#SourceTableName#" rename TO "_Migrated_#DestinationTableName#";
alter table "#DestinationTableName#" rename TO "#SourceTableName#";