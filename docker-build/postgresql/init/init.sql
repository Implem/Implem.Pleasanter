create user "Implem.Pleasanter_Owner" with password 'SetAdminsPWD';
create schema authorization "Implem.Pleasanter_Owner";
create database "Implem.Pleasanter" with owner "Implem.Pleasanter_Owner";
\c "Implem.Pleasanter";
CREATE EXTENSION IF NOT EXISTS pg_trgm;