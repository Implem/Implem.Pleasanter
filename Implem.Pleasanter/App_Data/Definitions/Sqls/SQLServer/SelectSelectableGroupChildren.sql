select 
    *
from(
    select 
        "Groups"."GroupId"
        ,"Groups"."GroupName"
    from "Groups" as "Groups"
    where 
        "Groups"."TenantId" = @TenantId
        -- 自分自身は含めない
        and "Groups"."GroupId" <> @GroupId
        -- グループメンバーに含まれる組織は含めない
        and "Groups".GroupId not in 
        (
            select 
                "GroupChildren"."ChildId"
            from 
                "GroupChildren" as "GroupChildren"
            where 
                "GroupChildren"."GroupId"=@GroupId
                -- 削除メンバーを除く
                {0}
                -- and "GroupChildren"."ChildId" not in ( {0} )
        )
        -- 追加したメンバーは含めない
        {1}
        -- and "Groups"."Groups" not in ( {0} ) 
        and 
        (
            "Groups"."GroupName" like @SearchText
        ) 
        and "Groups"."Disabled" = 0
) as "items"
order by
    "items"."GroupName" asc
    ,"items"."GroupId" asc
offset @Offset rows 
fetch next @PageSize rows only;