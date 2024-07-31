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
                    and "Depts"."Disabled"=0
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
                        and "Groups"."Disabled"=0
                        and "Permissions"."GroupId"="Groups"."GroupId"
                        and
                        (
                            exists
                            (
                                select * from "Depts"
                                where "Depts"."TenantId"=@ipT
                                    and "Depts"."DeptId"=@ipD
                                    and "Depts"."Disabled"=0
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