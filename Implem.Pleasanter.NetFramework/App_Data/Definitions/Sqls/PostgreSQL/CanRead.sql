"Sites"."TenantId"=@ipT
and
(
    exists
    (
        select *
        from "Permissions"
        where "Permissions"."ReferenceId" in
        (
            "InheritPermission","Items"."ReferenceId"
        )
            and "Permissions"."PermissionType" & 1=1
            and
            (
                exists
                (
                    select * from "Depts"
                    where "Depts"."TenantId"=@ipT
                        and "Depts"."DeptId"=@ipD
                        and "Depts"."Disabled"='false'
                        and "Permissions"."DeptId"="Depts"."DeptId"
                        and @ipD<>0
                )
                or
                (
                    exists
                    (
                        select *
                        from "GroupMembers" inner join "Groups" on "GroupMembers"."GroupId"="Groups"."GroupId"
                        where "Groups"."TenantId"=@ipT
                            and "Groups"."Disabled"='false'
                            and "Permissions"."GroupId"="GroupMembers"."GroupId"
                            and
                            (
                                exists
                                (
                                    select * from "Depts"
                                    where "Depts"."TenantId"=@ipT
                                        and "Depts"."DeptId"=@ipD
                                        and "Depts"."Disabled"='false'
                                        and "GroupMembers"."DeptId"="Depts"."DeptId"
                                        and @ipD<>0
                                )
                                or
                                (
                                    "GroupMembers"."UserId"=@ipU
                                    and @ipU<>0
                                )
                            )
                    )
                )
                or
                (
                    "Permissions"."UserId"=@ipU
                    and @ipU<>0
                )
                or
                (
                    "Permissions"."UserId"=-1
                )
            )
    )
)