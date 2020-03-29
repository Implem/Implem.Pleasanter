with RECURSIVE "CTE"(
    "SiteId",
    "ParentId",
    "ReferenceType",
    "Title",
    "Lv") as
(
    select
        "Sites"."SiteId",
        "Sites"."ParentId",
        "Sites"."ReferenceType",
        "Sites"."Title",
        1
    from
        "Sites" inner join "Permissions" on
        "Sites"."InheritPermission" = "Permissions"."ReferenceId"
    where
        "Sites"."TenantId" = @TenantId_ and
        "Sites"."ReferenceType" = @ReferenceType_ and
        "Sites"."SiteId" <> @SiteId_ and
        (
            @HasPrivilege_ = 1 or
            (exists(select * from "Permissions" where "Permissions"."ReferenceId" = "InheritPermission" and "Permissions"."PermissionType" & 4 = 4 and (("Permissions"."UserId" = @ipU and @ipU <> 0) or ("Permissions"."DeptId" = @ipD and @ipD <> 0) or (exists(select * from "GroupMembers" where "Permissions"."GroupId"="GroupMembers"."GroupId" and (("GroupMembers"."DeptId" = @ipD and @ipD <> 0) or ("GroupMembers"."UserId" = @ipU and @ipU <> 0)))))))
        )

    union all

    select
        "t1"."SiteId",
        "t1"."ParentId",
        "t1"."ReferenceType",
        "t1"."Title",
        "CTE"."Lv" + 1
    from
        "Sites" as "t1" inner join "CTE" on
        "t1"."SiteId" = "CTE"."ParentId"
)
select distinct
    "CTE"."SiteId",
    "CTE"."ParentId",
    "CTE"."ReferenceType",
    "CTE"."Title"
from "CTE"
where
    "CTE"."ReferenceType" <> 'Wikis';
