delete from "GroupMembers"
where "ChildGroup" = 1
    and "GroupId" in (
        select "GroupId"
        from "Groups"
        where "TenantId" = @TenantId#CommandCount#
            and ({{groupid_search_condition}})
    );
insert
into "GroupMembers" (
    "GroupId"
    ,"DeptId"
    ,"UserId"
    ,"ChildGroup"
    ,"Creator"
    ,"Updator"
)
(
    with recursive "TargetGroups" ("GroupId") as (
        select "GroupId"
        from "Groups"
        where "TenantId" = @TenantId#CommandCount#
            and "Disabled" = 0
            and ({{groupid_search_condition}})
    ),
    "GroupsChildIsNotInSelfId" ("Lv", "RootGroupId", "GroupId", "ChildId") as (
        select
            1 as "Lv"
            ,"t1"."GroupId" as "RootGroupId"
            ,"t2"."GroupId"
            ,"t2"."ChildId"
        from
            "TargetGroups" as "t1"
            inner join "GroupChildren" as "t2" on "t1"."GroupId" = "t2"."GroupId"
        union all
        select
            "t1"."Lv" + 1 as "Lv"
            ,"t1"."RootGroupId"
            ,"t2"."GroupId"
            ,"t2"."ChildId"
        from
            "GroupsChildIsNotInSelfId" as "t1"
            inner join "GroupChildren" as "t2" on "t1"."ChildId" = "t2"."GroupId"
            inner join "Groups" as "t3" on "t2"."GroupId" = "t3"."GroupId"
        where "t3"."TenantId" = @TenantId#CommandCount#
            and "t1"."Lv" < @GroupsDepthMax#CommandCount#
            and "t3"."Disabled" = 0
    )
    select
        distinct
        "t1"."RootGroupId" as "GroupId"
        ,"t4"."DeptId"
        ,"t4"."UserId"
        ,1 as "ChildGroup"
        ,@ipU as "Creator"
        ,@ipU as "Updator"
    from
        "GroupsChildIsNotInSelfId" as "t1"
        inner join "Groups" as "t3" on "t1"."ChildId" = "t3"."GroupId"
        inner join "GroupMembers" as "t4" on "t1"."ChildId" = "t4"."GroupId"
    where "t3"."TenantId" = @TenantId#CommandCount#
        and "t3"."Disabled" = 0
        and "t4"."ChildGroup" = 0
);
