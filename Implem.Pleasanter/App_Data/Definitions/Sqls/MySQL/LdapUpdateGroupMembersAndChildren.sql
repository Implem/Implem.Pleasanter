drop procedure if exists "LdapUpdateGroupMembersAndChildren";
create procedure "LdapUpdateGroupMembersAndChildren"(
    in v_tenantid int
    ,in v_ldap_object_guid text
    ,in v_is_member_insert boolean
    ,in v_is_child_insert boolean
    ,in v_is_first_time boolean
    ,in v_ipu int
)
begin
    declare v_group_id int;
    select
        "GroupId" into v_group_id
    from
        "Groups" as "t1"
    where "t1"."TenantId" = v_tenantid
        and "t1"."LdapGuid" = v_ldap_object_guid;
    if v_is_first_time = true then
        delete from
            "GroupMembers"
        where
            "GroupId" = v_group_id;
    end if;
    if v_is_member_insert = true then
        insert into
            "GroupMembers" ("GroupId","UserId","Creator","Updator")
            select v_group_id, "t3"."UserId", v_ipu, v_ipu
            from "Users" as "t3"
            where "t3"."TenantId" = v_tenantid
                and {{userLoginIds_condition}};
    end if;
    if v_is_first_time = true then
        delete from
            "GroupChildren"
        where
            "GroupId" = v_group_id;
    end if;
    if v_is_child_insert = true then
        insert into
            "GroupChildren" ("GroupId","ChildId","Creator","Updator")
            select v_group_id, "t5"."GroupId", v_ipu, v_ipu
            from "Groups" as "t5"
            where "t5"."TenantId" = v_tenantid
                and {{groupGuids_condition}};
    end if;
end;
call LdapUpdateGroupMembersAndChildren(
    @TenantId#CommandCount#,
    @LdapObjectGUID#CommandCount#,
    @isMemberInsert#CommandCount#,
    @isChildInsert#CommandCount#,
    @isFirstTime#CommandCount#,
    @ipU);
drop procedure "LdapUpdateGroupMembersAndChildren";