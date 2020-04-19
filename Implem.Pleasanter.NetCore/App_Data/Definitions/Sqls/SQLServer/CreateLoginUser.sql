use "#ServiceName#";
if not exists(select * from syslogins where name='#Uid#')
begin
    create login [#Uid#] with password='#Pwd#', default_database=[#ServiceName#], check_expiration=off, check_policy=off;
end;
alter login "#Uid#" enable;
create user "#Uid#" for login "#Uid#";