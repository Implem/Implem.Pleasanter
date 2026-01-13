delete top(@TopSize#CommandCount#)
from #TableName#
where #ColumnName# not in (
    select [ReferenceId] from [Items]
    union all
    select [ReferenceId] from [Items_deleted]
)