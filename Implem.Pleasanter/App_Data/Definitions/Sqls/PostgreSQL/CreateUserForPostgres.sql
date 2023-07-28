CREATE USER "#Uid_Owner#" WITH LOGIN PASSWORD '#Pwd_Owner#' valid until 'infinity';

ALTER ROLE "#Uid_Owner#";
CREATE USER "#Uid_User#" WITH LOGIN PASSWORD '#Pwd_User#' valid until 'infinity';

ALTER USER "#Uid_Owner#" SET search_path TO "#SchemaName#";
ALTER USER "#Uid_User#" SET search_path TO "#SchemaName#";
