use [Implem.Pleasanter];
declare @@_T int;                                 set @@_T = '1';
declare @@_D int;                                 set @@_D = '3';
declare @@_U int;                                 set @@_U = '1';
declare @TenantId1 int;                           set @TenantId1 = '1';
declare @Guid1 nvarchar(32);                      set @Guid1 = 'd6984e4cb7504859ac8c04c9d3d86c88';
declare @TenantId2 int;                           set @TenantId2 = '1';
declare @Guid2 nvarchar(32);                      set @Guid2 = 'd6984e4cb7504859ac8c04c9d3d86c88';

select "Binaries"."Guid", "Binaries"."BinaryType", "Binaries"."Bin",
"Binaries"."FileName", "Binaries"."ContentType" 
from "Binaries" as "Binaries"

inner join "Items" on "Binaries"."ReferenceId"="Items"."ReferenceId" 
inner join "Sites" on "Items"."SiteId"="Sites"."SiteId" 
where ("Binaries"."TenantId"=@TenantId1) and ("Binaries"."Guid"=@Guid1) and ("Sites"."TenantId"=@TenantId1) and ((exists(
select top 1 "Permissions"."PermissionType" 
from "Permissions" 
where "Permissions"."ReferenceId" = "InheritPermission" and "Permissions"."PermissionType" & 1 = 1 and (("Permissions"."DeptId" = @_D and @_D <> 0) or ("Permissions"."UserId" = @_U and @_U <> 0) or ("Permissions"."UserId" = -1) or (exists(
select * 
from "GroupMembers" 
where "Permissions"."GroupId"="GroupMembers"."GroupId" and (("GroupMembers"."DeptId" = @_D and @_D <> 0) or ("GroupMembers"."UserId" = @_U and @_U <> 0))))))) or ((exists(
select * 
from "Permissions" 
where "Permissions"."ReferenceId"="Binaries"."ReferenceId" and ("Permissions"."PermissionType" & 1 = 1) and ("Permissions"."GroupId" in (
select "GroupMembers"."GroupId" 
from "GroupMembers" as "GroupMembers"

where ((@_D<>0 and "GroupMembers"."DeptId"=@_D)or(@_U<>0 and "GroupMembers"."UserId"=@_U)or("GroupMembers"."UserId"=-1)) ) or ((@_D<>0 and "Permissions"."DeptId"=@_D)or(@_U<>0 and "Permissions"."UserId"=@_U)or("Permissions"."UserId"=-1)) ) ))) )  union all 
select "Binaries"."Guid", "Binaries"."BinaryType", "Binaries"."Bin",
"Binaries"."FileName", "Binaries"."ContentType" 
from "Binaries" as "Binaries"

inner join "Items" on "Binaries"."ReferenceId"="Items"."ReferenceId" 
inner join "Sites" on "Items"."SiteId"="Sites"."SiteId" 
where ("Binaries"."TenantId"=@TenantId2) and ("Binaries"."Guid"=@Guid2) and ("Binaries"."CreatedTime"="Binaries"."UpdatedTime" and "Binaries"."Creator"=1) ;
