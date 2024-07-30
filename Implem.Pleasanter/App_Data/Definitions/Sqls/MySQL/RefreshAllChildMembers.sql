drop procedure if exists refresh_all_member;
create procedure "refresh_all_member"(
    in "v_tenantid" int,
    in "v_depth_max" int,
    in "v_ipu" int)
begin
    declare done int default false;
    declare v_groupid int;
    declare v_row_cnt int;
    declare cur cursor for
        select "GroupId" 
        from "Groups"
        where "TenantId" = v_tenantid
        and ({{groupid_search_condition}});
    declare continue handler for not found set done = 1;
    open cur;
    read_loop: loop
        fetch cur into v_groupid;
        if done = 1 then
            leave read_loop;
        end if;
        delete from "GroupMembers" where "GroupId" = v_groupid and "ChildGroup" = 1;
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
            with recursive "GroupsChildIsNotInSelfId" ("Lv", "GroupId", "ChildId") as (
                select
                    1 as "Lv"
                    ,"t2"."GroupId"
                    ,"t2"."ChildId"
                from
                    "GroupChildren" as "t2"
                    inner join "Groups" as "t3" on "t2"."GroupId"="t3"."GroupId"
                where "t3"."TenantId" = v_tenantid
                    and "t3"."Disabled" = 0
                    and "t2"."GroupId" = v_groupid
                union all
                select
                    "t1"."Lv" + 1 as "Lv"
                    ,"t2"."GroupId"
                    ,"t2"."ChildId"
                from
                    "GroupsChildIsNotInSelfId" as "t1"
                    ,"GroupChildren" as "t2"
                        inner join "Groups" as "t3" on "t2"."GroupId"="t3"."GroupId"
                where "t3"."TenantId" = v_tenantid
                    and "t1"."Lv" < v_depth_max
                    and "t3"."Disabled" = 0
                    and "t1"."ChildId" = "t2"."GroupId"
            )
            select
                distinct
                v_groupid as "GroupId"
                ,"DeptId"
                ,"UserId"
                ,1 as "ChildGroup"
                ,v_ipU as "Creator"
                ,v_ipU as "Updator"
            from
                "GroupMembers" as "t4"
            where
                "t4"."ChildGroup" = 0
                and "t4"."GroupId" in (
                    select "GroupId"
                    from "Groups"
                    where "TenantId" = v_tenantid
                        and "Disabled" = 0
                        and "GroupId" in (select "ChildId" from "GroupsChildIsNotInSelfId")
                )
        );
    end loop;
    close cur;
end;
call refresh_all_member(@TenantId#CommandCount#, @GroupsDepthMax#CommandCount#, @ipU);
drop procedure "refresh_all_member";