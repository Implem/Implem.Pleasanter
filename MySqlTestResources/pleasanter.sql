create user 'Implem.Pleasanter_Owner'@'%' identified by '$MMm56@W57C&sN';
grant all on `Implem.Pleasanter`.* to 'Implem.Pleasanter_Owner'@'%' with grant option;
create user 'Implem.Pleasanter_User'@'%' identified by 'qasgo%v5MzTJZt';
grant select, insert, update, delete, create routine, alter routine on `Implem.Pleasanter`.* to 'Implem.Pleasanter_User'@'%';