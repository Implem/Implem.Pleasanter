-- テナントの全ての子ユーザを作り直す。
-- pgsqlのdoブロック内は@TenantId等のパラメータが渡せないため、一時functionを使用する。
create or replace function pg_temp.refresh_all_member( 
    v_tenantid integer
    , v_depth_max integer
    , v_ipu integer
)
returns integer as $$
declare cur cursor for 
    select
        "GroupId" 
    from
        "Groups" 
    where
        "TenantId" = v_tenantid
		and ({{groupid_search_condition}}); 
declare v_groupid integer; 
declare v_total integer := 0; 
declare v_row_cnt integer; 
begin                                           
    -- GroupIdでループ
    for c in cur loop v_groupid := c."GroupId"; 
		delete 
		from
			"GroupMembers" 
		where
			"GroupId" = v_groupid
			and "ChildGroup" = true; 
        with recursive "GroupsChildIsNotInSelfId" ("Lv", "GroupId", "ChildId") as ( 
            select
                1 as "Lv"
                , "t2"."GroupId"
                , "t2"."ChildId" 
            from
				"GroupChildren" as "t2"
				inner join "Groups" as "t3" on "t2"."GroupId"="t3"."GroupId"
            where
				("t3"."Disabled" = false)
                and "t2"."GroupId" = c."GroupId" 
            union all 
            select
                "t1"."Lv" + 1 as "Lv"
                , "t2"."GroupId"
                , "t2"."ChildId" 
            from
                "GroupsChildIsNotInSelfId" as "t1" 
				, "GroupChildren" as "t2"
				inner join "Groups" as "t3" on "t2"."GroupId"="t3"."GroupId"
            where
				"t1"."Lv" < v_depth_max 
				and "t3"."Disabled" = false
				and "t1"."ChildId" = "t2"."GroupId"
        ) 
        insert 
        into "GroupMembers" ( 
            "GroupId"
            , "DeptId"
            , "UserId"
            , "ChildGroup"
            , "Creator"
            , "Updator"
        ) 
        select
            distinct
            v_groupid as "GroupId"
            , "DeptId"
            , "UserId"
            , true as "ChildGroup"
            , v_ipU as "Creator"
            , v_ipU as "Updator" 
        from
            "GroupMembers" as "t4" 
        where
			"t4"."ChildGroup" = false
            and "t4"."GroupId" in (
                select "GroupId"
                from "Groups"
                where (1=1)
                    and "Disabled" = false
                    and "GroupId" in (select "ChildId" from "GroupsChildIsNotInSelfId") 
            );
		get diagnostics v_row_cnt = row_count;
		v_total := v_total + v_row_cnt;
    end loop; 
    return v_total; 
end $$ language plpgsql; 

select
    pg_temp.refresh_all_member(@TenantId#CommandCount#, @GroupsDepthMax#CommandCount#, @ipU) as refresh_all_member; 

drop function if exists pg_temp.refresh_all_member(integer, integer, integer);

