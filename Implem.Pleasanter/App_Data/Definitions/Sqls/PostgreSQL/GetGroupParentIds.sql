with recursive "GroupsParentIdsNotInSelfId"("Lv", "GroupId", "ChildId", "Disabled") as ( 
    -- 親GroupIdリスト(自ID含む)
    select
        1 as "Lv"
        ,"t2"."GroupId"
        ,"t2"."ChildId"
        ,"t3"."Disabled" 
    from
        "GroupChildren" as "t2"
        inner join "Groups" as "t3" on "t2"."GroupId" = "t3"."GroupId"
    where "t3"."TenantId" = @ipT
        and "t2"."ChildId" in ({{GroupsStartIdP}})
    union all 
    select
        "t1"."Lv" + 1
        ,"t2"."GroupId"
        ,"t2"."ChildId"
        ,"t3"."Disabled" 
    from
        "GroupsParentIdsNotInSelfId" as "t1"
        ,"GroupChildren" as "t2" 
        inner join "Groups" as "t3" on "t2"."GroupId" = "t3"."GroupId"
    where "t3"."TenantId" = @ipT
        and "t1"."Lv" < @GroupsDepthMax 
        and "t3"."Disabled" = false 
        and "t1"."GroupId" = "t2"."ChildId"
) 
select
    distinct 
    "GroupId"
from
    "GroupsParentIdsNotInSelfId"
order by 
    "GroupId";
