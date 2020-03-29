select
    sysindexes.name as "Name",
    sysindexes.status,
    syscolumns.name as "ColumnName"
from
    sysindexkeys ,sysobjects ,syscolumns ,sysindexes
where
    sysindexkeys.id = sysobjects.id and
    sysindexkeys.id = syscolumns.id and
    sysindexkeys.colid = syscolumns.colid and
    sysindexkeys.id = sysindexes.id and
    sysindexkeys.indid = sysindexes.indid and
    sysobjects.xtype = 'U' and
    sysobjects.name = '#TableName#'
order by
    sysindexes.name,
    sysindexkeys.id,
    sysindexkeys.indid,
    sysindexkeys.keyno;