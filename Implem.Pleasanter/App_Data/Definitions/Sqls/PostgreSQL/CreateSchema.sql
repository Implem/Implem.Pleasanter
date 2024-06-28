create schema "#SchemaName#" authorization "#Uid_Owner#";
grant usage on schema "#SchemaName#" to "#Uid_User#";
create extension if not exists pg_trgm;
create extension if not exists pgcrypto;
