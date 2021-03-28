(
    exists
    (
        select *
        from "Permissions"
        where "Permissions"."ReferenceId"="InheritPermission" and
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
                    from "Groups" inner join "GroupMembers" on "Groups"."GroupId"="GroupMembers"."GroupId"
                    where "Groups"."TenantId"=@_T
                        and "Groups"."Disabled"='false'
                        and "Permissions"."GroupId"="Groups"."GroupId"
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