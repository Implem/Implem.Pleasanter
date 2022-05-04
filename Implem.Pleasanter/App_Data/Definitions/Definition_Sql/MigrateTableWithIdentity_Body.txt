declare @empty varchar(1);
set @empty = '';
set identity_insert "#DestinationTableName#" on;
insert into "#DestinationTableName#"(#DestinationColumnCollection#) select #SourceColumnCollection# from "#SourceTableName#";
set identity_insert "#DestinationTableName#" off;
execute sp_rename N'#SourceTableName#', N'_Migrated_#DestinationTableName#', 'object';
execute sp_rename N'#DestinationTableName#', N'#SourceTableName#', 'object';