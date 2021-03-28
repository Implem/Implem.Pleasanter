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
                    from "Groups" inner join "GroupMembers" on "Groups"."GroupId"="GroupMembers"."GroupId"
                    where "Groups"."TenantId"=@ipT
                        and "Groups"."Disabled"='false'
                        and "Permissions"."GroupId"="Groups"."GroupId"
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