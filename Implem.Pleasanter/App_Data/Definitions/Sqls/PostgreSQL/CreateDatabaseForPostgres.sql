create database "#InitialCatalog#" owner "#Uid_Owner#";

do $$
begin
  if exists (select 'public' != '#SchemaName#') then
     revoke all on database "#InitialCatalog#" from public;
  end if;
end $$ ;

grant all privileges on database "#InitialCatalog#" to "#Uid_Owner#";
grant connect, temporary on database "#InitialCatalog#" to "#Uid_User#";
