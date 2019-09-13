alter table "#TableName#" add constraint "#PkName#" primary key (#PkColumnsWithoutOrderType#);
create unique index "Ix#PkName#" on "#TableName#" (#PkColumns#);