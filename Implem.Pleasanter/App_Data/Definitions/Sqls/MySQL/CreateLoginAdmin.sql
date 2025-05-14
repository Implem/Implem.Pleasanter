create user "#Uid#"@"#MySqlConnectingHost#" identified by '#Pwd#';
grant all on "#ServiceName#".* to "#Uid#"@"#MySqlConnectingHost#" with grant option;