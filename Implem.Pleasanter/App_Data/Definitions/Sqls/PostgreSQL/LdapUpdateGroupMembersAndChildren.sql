create or replace function pg_temp.LdapUpdateGroupMemberAndChildren(
    v_tenantid integer
    , v_ldap_object_guid text
	, v_is_member_insert boolean
	, v_is_child_insert boolean
	, v_is_first_time boolean
	, v_ipu integer	
) 
returns integer as $$
-- 対象のグループID取得
declare v_group_id integer;  
begin
	select
	 "GroupId" into v_group_id
	from
	 "Groups" as "t1"
	where (1=1)
	 and "t1"."TenantId" = v_tenantid
	 and "t1"."LdapGuid" = v_ldap_object_guid
	;
	-- グループメンバーを削除
    if v_is_first_time = true
    then
    begin
	    delete from
	      "GroupMembers"
	    where
	      "GroupId" = v_group_id
	    ;
	end;
	end if;

	-- グループメンバーを追加
	if v_is_member_insert = true
	then
	begin
		insert into
		  "GroupMembers" ("GroupId","UserId","Creator","Updator")
		  select v_group_id, "t3"."UserId", v_ipu, v_ipu
		  from "Users" as "t3"
		  where (1=1)
		    and "t3"."TenantId" = v_tenantid
		    and {{userLoginIds_condition}}
		  ;
	end;
	end if;

	-- 子グループを削除
    if v_is_first_time = true
    then
    begin
	    delete from
	      "GroupChildren"
	    where
	      "GroupId" = v_group_id
	    ;
	end;
	end if;
	-- 子グループを追加
	if v_is_child_insert = true
	then
	begin
		insert into
		  "GroupChildren" ("GroupId","ChildId","Creator","Updator")
		  select v_group_id, "t5"."GroupId", v_ipu, v_ipu
		  from "Groups" as "t5"
		  where (1=1)
		    and "t5"."TenantId" = v_tenantid
		    and {{groupGuids_condition}}
		  ;
	end;
	end if;
	return 0;
end;
$$ LANGUAGE plpgsql;
select pg_temp.LdapUpdateGroupMemberAndChildren(
	@TenantId#CommandCount#,
	@LdapObjectGUID#CommandCount#,
	@isMemberInsert#CommandCount#,
	@isChildInsert#CommandCount#,
    @isFirstTime#CommandCount#,
	@ipU);
drop function if exists pg_temp.LdapUpdateGroupMemberAndChildren(integer,text,boolean,boolean,boolean,integer);
