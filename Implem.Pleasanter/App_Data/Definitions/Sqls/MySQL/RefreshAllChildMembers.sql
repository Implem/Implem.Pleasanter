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
        select "groupId"
        from "groups"
        where "tenantId" = v_tenantid
        and ({{groupid_search_condition}});
    declare continue handler for not found set done = true;
    open cur;
    read_loop: loop
        fetch cur into v_groupid;
        if done then
            leave read_loop;
        end if;
        delete from "groupmembers"
        where "groupId" = v_groupid
        and "childgroup" = 1;
        -- with ... insertによるグループメンバ登録処理を書く
    end loop;
    close cur;

end;
call refresh_all_member(@TenantId#CommandCount#, @GroupsDepthMax#CommandCount#, @ipU);
drop procedure "refresh_all_member";