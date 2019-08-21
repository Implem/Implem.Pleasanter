declare df cursor for
select name from sys.default_constraints 
where parent_object_id = 
(select object_id from sys.tables where name = '#TableName#');
open df;
fetch next from df into @target
while(@@fetch_status = 0)
begin
    exec(N'alter table "#TableName#" drop constraint "' + @target + '"');
    fetch next from df into @target;
end;
close df;
deallocate df;
