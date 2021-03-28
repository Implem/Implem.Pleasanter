"Sites"."TenantId"=@_T
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
                    where "Depts"."TenantId"=@_T
                        and "Depts"."DeptId"=@_D
                        and "Depts"."Disabled"='false'
                        and "Permissions"."DeptId"="Depts"."DeptId"
                        and @_D<>0
                )
                or
                (
                    exists
                    (
                        select *
                        from "GroupMembers" inner join "Groups" on "GroupMembers"."GroupId"="Groups"."GroupId"
                        where "Groups"."TenantId"=@_T
                            and "Groups"."Disabled"='false'
                            and "Permissions"."GroupId"="GroupMembers"."GroupId"
                            and
                            (
                                exists
                                (
                                    select * from "Depts"
                                    where "Depts"."TenantId"=@_T
                                        and "Depts"."DeptId"=@_D
                                        and "Depts"."Disabled"='false'
                                        and "GroupMembers"."DeptId"="Depts"."DeptId"
                                        and @_D<>0
                                )
                                or
                                (
                                    "GroupMembers"."UserId"=@_U
                                    and @_U<>0
                                )
                            )
                    )
                )
                or
                (
                    "Permissions"."UserId"=@_U
                    and @_U<>0
                )
                or
                (
                    "Permissions"."UserId"=-1
                )
            )
    )
)