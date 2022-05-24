declare @empty varchar(1);
set @empty = '';
insert into "#DestinationTableName#"(#DestinationColumnCollection#) select #SourceColumnCollection# from "#SourceTableName#";
execute sp_rename N'#SourceTableName#', N'_Migrated_#DestinationTableName#', 'object';
execute sp_rename N'#DestinationTableName#', N'#SourceTableName#', 'object';