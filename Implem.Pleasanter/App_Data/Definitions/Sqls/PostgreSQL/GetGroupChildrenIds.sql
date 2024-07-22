with recursive "GroupsChildIsNotInSelfId"("Lv", "GroupId", "ChildId", "Disabled") as ( 
    -- 子GroupIdリスト(自ID含まない)
    select
        1 as "Lv"
        ,"t2"."GroupId"
        ,"t2"."ChildId"
        ,"t3"."Disabled" 
    from
        "GroupChildren" as "t2"
        inner join "Groups" as "t3" on "t2"."GroupId" = "t3"."GroupId"
    where "t3"."TenantId" = @ipT
        and "t2"."GroupId" in ({{GroupsStartIdP}})
    union all 
    select
        "t1"."Lv" + 1
        ,"t2"."GroupId"
        ,"t2"."ChildId"
        ,"t3"."Disabled" 
    from
        "GroupsChildIsNotInSelfId" as "t1"
        ,"GroupChildren" as "t2" 
            inner join "Groups" as "t3" on "t2"."GroupId" = "t3"."GroupId"
    where "t3"."TenantId" = @ipT
        and "t1"."Lv" < @GroupsDepthMax 
        and "t3"."Disabled" = false 
        and "t1"."ChildId" = "t2"."GroupId"
) 
select
    distinct 
    "ChildId" as "GroupId"
from
    "GroupsChildIsNotInSelfId"
order by 
    "GroupId";
