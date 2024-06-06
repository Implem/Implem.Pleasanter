select
    "cl"."table_name" as "name",
    "cl"."column_name" as "ColumnName",
    "cl"."data_type" as "TypeName",
    case
        when "cl"."data_type" in ('char', 'varchar') then "cl"."character_maximum_length"
        when "cl"."data_type" = 'tinyint' then 1
        else 0
    end as "max_length",
    case
        when "cl"."data_type" = 'decimal' then concat("cl"."numeric_precision",',',"cl"."numeric_scale")
        else '0,0'
    end as "Size",
    case
        when "cl"."is_nullable" = 'YES' then 1
        else 0
    end as "is_nullable",
    case
        when "cl"."column_name" = "tmp_identity_info"."column_name" then 1
        else 0
    end as "is_identity"
from
    "information_schema"."columns" as "cl"
left outer join
    (select "tmp_auto_increment"."table_name", "tmp_pk"."column_name"
     from 
         (select "table_name" from "information_schema"."tables" where "table_schema" = '#InitialCatalog#' and "auto_increment" is not null) as "tmp_auto_increment"
     inner join
         (select "table_name", "column_name" from "information_schema"."statistics" where "table_schema" = '#InitialCatalog#' and index_name = 'primary' and seq_in_index = 1) as "tmp_pk"
     where "tmp_auto_increment"."table_name" = "tmp_pk"."table_name") as "tmp_identity_info"
on "cl"."table_name" = "tmp_identity_info"."table_name"
where "cl"."table_schema" = '#InitialCatalog#'
and "cl"."table_name" = '#TableName#'
order by "cl"."table_name", "cl"."ordinal_position";