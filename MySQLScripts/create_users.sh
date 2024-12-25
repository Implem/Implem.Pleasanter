# GitHub Actionsのdb起動時に自動実行するSQL群
mysql -u root -p$MYSQL_ROOT_PASSWORD -e $"create user 'Implem.Pleasanter_Owner'@'%' identified by '$MYSQL_OWNER_PASSWORD';"
mysql -u root -p$MYSQL_ROOT_PASSWORD -e $"grant all on `Implem.Pleasanter`.* to 'Implem.Pleasanter_Owner'@'%' with grant option;"
mysql -u root -p$MYSQL_ROOT_PASSWORD -e $"create user 'Implem.Pleasanter_User'@'%' identified by '$MYSQL_USER_PASSWORD';"
mysql -u root -p$MYSQL_ROOT_PASSWORD -e $"grant select, insert, update, delete, create routine, alter routine on `Implem.Pleasanter`.* to 'Implem.Pleasanter_User'@'%';"