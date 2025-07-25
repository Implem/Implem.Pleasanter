create user "#Uid#"@"#MySqlConnectingHost#" identified by '#Pwd#';
grant create, alter, index, drop on "#ServiceName#".* to "#Uid#"@"#MySqlConnectingHost#";
grant select, insert, update, delete, create routine, alter routine on "#ServiceName#".* to "#Uid#"@"#MySqlConnectingHost#" with grant option;