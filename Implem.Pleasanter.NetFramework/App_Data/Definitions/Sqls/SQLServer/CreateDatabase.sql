if not exists(select * from sysdatabases where name = '#InitialCatalog#')
begin
    create database "#InitialCatalog#"
    collate japanese_90_ci_as_ks
end;