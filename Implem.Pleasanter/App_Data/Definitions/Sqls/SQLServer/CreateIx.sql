create #Unique# nonclustered index #IxName# on dbo."#TableName#"
(
    #IxColumns#
) with( statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
alter table dbo."#TableName#" set (lock_escalation = table)