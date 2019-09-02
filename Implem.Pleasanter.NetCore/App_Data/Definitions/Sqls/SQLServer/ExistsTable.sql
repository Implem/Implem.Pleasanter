select name
from dbo.sysobjects
where id = object_id('#TableName#') and objectproperty(id, N'IsUserTable') = 1;