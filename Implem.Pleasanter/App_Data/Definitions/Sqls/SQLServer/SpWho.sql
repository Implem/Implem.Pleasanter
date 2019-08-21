if exists(select * from syslogins where name='#Uid#')
begin
    exec sp_who '#Uid#';
end;