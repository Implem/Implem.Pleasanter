with to_delete as (
    select "#ColumnName#"
    from "#TableName#"
    where "#ColumnName#" not in (
        select "ReferenceId" from "Items"
        union all
        select "ReferenceId" from "Items_deleted"
    )
    limit @TopSize#CommandCount#
)
delete from "#TableName#"
where "#ColumnName#" in (select "#ColumnName#" from to_delete)