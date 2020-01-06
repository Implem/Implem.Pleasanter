use [Implem.Pleasanter];
declare @@_T int;                                 set @@_T = '1';
declare @@_D int;                                 set @@_D = '3';
declare @@_U int;                                 set @@_U = '1';
declare @TenantId int;                            set @TenantId = '1';
declare @ParentId bigint;                         set @ParentId = '1';

select "Sites"."SiteId", "Sites"."Title", "Sites"."ReferenceType" 
from "Sites" as "Sites"
where ("Sites"."TenantId"=@TenantId) and ("Sites"."ParentId"=@ParentId) ;
