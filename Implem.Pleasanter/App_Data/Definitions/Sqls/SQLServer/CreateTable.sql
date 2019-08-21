declare @target nvarchar(256);
set ansi_nulls on;
set quoted_identifier on;
set ansi_padding on;
if not exists(select * from sys.tables where name = '#TableName#')
begin
#DropConstraint#
    create table "dbo"."#TableName#"
    (
#Columns#
#Pks#
    );
#Defaults#
end;
