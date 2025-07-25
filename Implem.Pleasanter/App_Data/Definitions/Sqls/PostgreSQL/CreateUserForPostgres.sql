do $xxx$
begin
  if exists (select 'public' != '#SchemaName#') then
     revoke create on schema public from public;
  end if;
  if not exists (select * from pg_user where usename = '#Uid_Owner#') then
     create user "#Uid_Owner#" with login password '#Pwd_Owner#' valid until 'infinity';
     create user "#Uid_User#" with login password '#Pwd_User#' valid until 'infinity';
     grant "#Uid_Owner#" to "#Uid_Sa#";
  end if;
end $xxx$
;

alter role "#Uid_Owner#";

alter user "#Uid_Owner#" set search_path to "#SchemaName#", pg_catalog;
alter user "#Uid_User#" set search_path to "#SchemaName#", pg_catalog;

