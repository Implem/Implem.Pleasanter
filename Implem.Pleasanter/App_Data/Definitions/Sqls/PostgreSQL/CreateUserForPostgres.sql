do $$
begin
  if not exists (select * from pg_user where usename = '#Uid_Owner#') then
     create user "#Uid_Owner#" with login password '#Pwd_Owner#' valid until 'infinity';
     create user "#Uid_User#" with login password '#Pwd_User#' valid until 'infinity';
  end if;
end $$
;

alter role "#Uid_Owner#";

alter user "#Uid_Owner#" set search_path to "#SchemaName#";
alter user "#Uid_User#" set search_path to "#SchemaName#";

