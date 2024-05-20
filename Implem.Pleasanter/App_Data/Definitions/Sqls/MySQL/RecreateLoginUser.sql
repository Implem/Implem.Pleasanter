if exists(select * from syslogins where name='#Uid#')
begin
    drop login "#Uid#";
end;
create login "#Uid#" with password='#Pwd#', default_database="#ServiceName#", check_expiration=off, check_policy=off;
alter login "#Uid#" enable;
use "#ServiceName#";
if exists(select * from sysusers where name='#Uid#')
begin
    drop user "#Uid#";
end;
create user "#Uid#" for login "#Uid#";
alter role "db_datareader" add member "#Uid#";
alter role "db_datawriter" add member "#Uid#";