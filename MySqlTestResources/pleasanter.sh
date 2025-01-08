#!/bin/bash
mysql -u root -p${MYSQL_ROOT_PASSWORD} -e "create user 'Implem.Pleasanter_Owner'@'%' identified by '${MYSQL_OWNER_PASSWORD}';";
mysql -u root -p${MYSQL_ROOT_PASSWORD} -e "grant all on \`Implem.Pleasanter\`.* to 'Implem.Pleasanter_Owner'@'%' with grant option;";
