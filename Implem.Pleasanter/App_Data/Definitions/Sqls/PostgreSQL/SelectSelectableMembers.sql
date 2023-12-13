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
                -- 削除メンバーを除く
                {0}
                -- and "GroupMembers"."DeptId" not in ( {0} )
        )
        -- 追加したメンバーは含めない
        {1}
        -- and "Depts"."DeptId" not in ( {0} ) 
        and 
        (
            "Depts"."DeptCode" ilike @SearchText
            or "Depts"."DeptName" ilike @SearchText
        ) 
        and "Depts"."Disabled" = 'false'

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
                "GroupMembers"."GroupId"= @GroupId 
                -- 削除メンバーを除く
                {2}
                -- and "GroupMembers"."UserId" not in ( {0} ) 
        ) 
        -- 追加したメンバーは含めない
        {3}
        -- and "Users"."UserId" not in ( {0} ) 
        and (
            "Users"."LoginId" ilike @SearchText
            or "Users"."Name" ilike @SearchText
            or "Users"."UserCode" ilike @SearchText
            or "Users"."Body" ilike @SearchText
            or "Depts"."DeptCode" ilike @SearchText 
            or "Depts"."DeptName" ilike @SearchText
            or "Depts"."Body" ilike @SearchText
        ) 
        and "Users"."Disabled"= 'false'
) as "items"
order by
    "items"."IsUser" asc
    ,"items"."DeptCode" asc
    ,"items"."DeptId" asc
    , "items"."UserCode" asc
    , "items"."UserId" asc
limit @PageSize offset @Offset;
