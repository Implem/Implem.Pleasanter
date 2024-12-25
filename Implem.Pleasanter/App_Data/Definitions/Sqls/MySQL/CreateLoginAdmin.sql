create user "#Uid#"@"localhost" identified by '#Pwd#';
grant all on "#ServiceName#".* to "#Uid#"@"localhost" with grant option;
create user "#Uid#"@"%" identified by '#Pwd#';
grant all on "#ServiceName#".* to "#Uid#"@"%" with grant option;