if not exists(select * from sys.indexes where sys.indexes.name = 'ix_#IxName#')
begin
    set quoted_identifier on
    set arithabort on
    set numeric_roundabort off
    set concat_null_yields_null on
    set ansi_nulls on
    set ansi_padding on
    set ansi_warnings on
    create #Unique# nonclustered index #IxName# on dbo."#TableName#"
    (
        #IxColumns#
    ) with( statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
    alter table dbo."#TableName#" set (lock_escalation = table)
end;
