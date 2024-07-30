select 
    *
from(
    select 
        "Depts"."DeptId"
        ,"Depts"."DeptCode"
        ,0 as "UserId"
        ,'' as "UserCode"
        ,0 as "IsUser" 
    from "Depts" as "Depts"
    where 
        "Depts"."TenantId"=@TenantId
        -- グループメンバーに含まれる組織は含めない
        and "Depts"."DeptId" not in 
        (
            select 
                "GroupMembers"."DeptId" 
            from 
                "GroupMembers" as "GroupMembers"
            where 
                ("GroupMembers"."GroupId"=@GroupId) 
                and ("GroupMembers"."ChildGroup" = 0)
                -- 削除メンバーを除く
                {0}
        )
        -- 追加したメンバーは含めない
        {1}
        and 
        (
            "Depts"."DeptCode" like @SearchText
            or "Depts"."DeptName" like @SearchText
        ) 
        and "Depts"."Disabled" = 0

    union 
        select 0 as "DeptId"
        , '' as "DeptCode"
        , "Users"."UserId"
        , "Users"."UserCode"
        , 1 as "IsUser" 
    from 
        "Users" as "Users"
    left outer join "Depts" 
    on 
        "Users"."DeptId" = "Depts"."DeptId" 
    where 
        "Users"."TenantId" = @TenantId
        -- グループメンバーに含まれるユーザは含めない
        and "Users"."UserId" not in 
        (
            select 
                "GroupMembers"."UserId" 
            from 
                "GroupMembers" as "GroupMembers"
            where 
                ("GroupMembers"."GroupId"=@GroupId) 
                and ("GroupMembers"."ChildGroup" = 0)
                -- 削除メンバーを除く
                {2}
        ) 
        -- 追加したメンバーは含めない
        {3}
        and (
            "Users"."LoginId" like @SearchText
            or "Users"."Name" like @SearchText
            or "Users"."UserCode" like @SearchText
            or "Users"."Body" like @SearchText
            or "Depts"."DeptCode" like @SearchText 
            or "Depts"."DeptName" like @SearchText
            or "Depts"."Body" like @SearchText
        ) 
        and "Users"."Disabled"= 0
) as "items"
order by
    "items"."IsUser" asc
    ,"items"."DeptCode" asc
    ,"items"."DeptId" asc
    ,"items"."UserCode" asc
    ,"items"."UserId" asc
limit @PageSize offset @Offset;
