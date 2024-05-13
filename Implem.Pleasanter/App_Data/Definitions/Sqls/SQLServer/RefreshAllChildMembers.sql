declare @v_group1 int; 
declare @v_group2 int;

declare v_cursor cursor for 
    select
        "GroupId" 
    from
        "Groups" 
    where
        ("TenantId" = @TenantId#CommandCount#) and ({{groupid_search_condition}}); 

open v_cursor; 

fetch next 
    from
        v_cursor 
    into @v_group1 while @@fetch_status = 0
begin;
    set
        @v_group2 = @v_group1; 
    with
    "GroupsChildIsNotInSelfId"("Lv", "GroupId", "ChildId") as ( 
        -- 子GroupIdリスト(自ID含まない)
        select
            1 as "Lv"
            , "t2"."GroupId"
            , "t2"."ChildId"
        from
            "GroupChildren" as "t2"
            inner join "Groups" as "t3" on "t2"."GroupId"="t3"."GroupId"
        where "t3"."TenantId" = @TenantId#CommandCount#
            and "t3"."Disabled" = 0
            and "t2"."GroupId" = @v_group2
        union all 
        select
            "t1"."Lv" + 1
            , "t2"."GroupId"
            , "t2"."ChildId"
        from
            "GroupsChildIsNotInSelfId" as "t1"
            , "GroupChildren" as "t2" 
            inner join "Groups" as "t3" on "t2"."GroupId"="t3"."GroupId"
        where "t3"."TenantId" = @TenantId#CommandCount#
            and "t1"."Lv" < @GroupsDepthMax#CommandCount# 
            and "t3"."Disabled" = 0 
            and "t1"."ChildId" = "t2"."GroupId"
    ) 
    merge into
        "GroupMembers" as "tgt"
    using
        (
        select 
            distinct
            @v_group2 as "GroupId"
            ,"DeptId"
            ,"UserId"
            ,1 as "ChildGroup"
        from
            "GroupMembers" as "t9"
        where "t9"."ChildGroup" = 0
            and "t9"."GroupId" in (
                select "GroupId"
                from "Groups"
                where  "TenantId" = @TenantId#CommandCount#
                    and "Disabled" = 0
                    and "GroupId" in (select "ChildId" from "GroupsChildIsNotInSelfId") 
            )
        ) as "src"
    on
        (
            "tgt"."GroupId" = "src"."GroupId"
            and "tgt"."DeptId" = "src"."DeptId"
            and "tgt"."UserId" = "src"."UserId"
            and "tgt"."ChildGroup" = "src"."ChildGroup"
        )
    when not matched by target then
        -- レコードを挿入
        insert
        (
            "GroupId"
            ,"DeptId"
            ,"UserId"
            ,"ChildGroup"
            ,"Creator"
            ,"Updator"
        )
        values
        (
            "src"."GroupId"
            ,"src"."DeptId"
            ,"src"."UserId"
            ,"src"."ChildGroup"
            ,@_U
            ,@_U
        )
    when not matched by source
        -- レコードを削除
        and ("tgt"."GroupId" = @v_group2 AND "tgt"."ChildGroup" <> 0)
        then
            delete
    ;
    fetch next 
        from
            v_cursor 
        into @v_group1; 
end close v_cursor; 
deallocate v_cursor;
