DO $$
BEGIN
  IF NOT EXISTS (SELECT * FROM pg_user WHERE usename = '#Uid_Owner#') THEN
     CREATE USER "#Uid_Owner#" WITH LOGIN PASSWORD '#Pwd_Owner#' valid until 'infinity';
     CREATE USER "#Uid_User#" WITH LOGIN PASSWORD '#Pwd_User#' valid until 'infinity';
  END IF;
END $$
;

ALTER ROLE "#Uid_Owner#";

ALTER USER "#Uid_Owner#" SET search_path TO "#SchemaName#";
ALTER USER "#Uid_User#" SET search_path TO "#SchemaName#";

