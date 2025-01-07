mysql -u root -p${MYSQL_ROOT_PASSWORD} -e "create user 'Implem.Pleasanter_Owner'@'%' identified by 'pw2';";
mysql -u root -p${MYSQL_ROOT_PASSWORD} -e "grant all on \`Implem.Pleasanter\`.* to 'Implem.Pleasanter_Owner'@'%' with grant option;";
