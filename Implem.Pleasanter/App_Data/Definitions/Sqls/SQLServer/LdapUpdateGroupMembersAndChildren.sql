begin;
	-- 対象のグループID取得
	declare @v_group_id#CommandCount# int;  
	select
	 @v_group_id#CommandCount#  = "GroupId"
	from
	 "Groups" as "t1"
	where (1=1)
	 and "t1"."TenantId" = @TenantId#CommandCount#
	 and "t1"."LdapGuid" = @LdapObjectGUID#CommandCount#
	;

	-- グループメンバーを削除
    if @isFirstTime#CommandCount# = 'True'
    begin
	    delete from
	      "GroupMembers"
	    where
	      "GroupId" = @v_group_id#CommandCount#
	    ;
    end
	-- グループメンバーを追加
	if @isMemberInsert#CommandCount# = 'True'
	begin
		insert into
		  "GroupMembers" ("GroupId","UserId","Creator","Updator")
		  select @v_group_id#CommandCount#, "t3"."UserId", @_U, @_U
		  from "Users" as "t3"
		  where (1=1)
		    and "t3"."TenantId" = @TenantId#CommandCount#
		    and {{userLoginIds_condition}}
		  ;
	end
	-- 子グループを削除
    if @isFirstTime#CommandCount# = 'True'
    begin
	    delete from
	      "GroupChildren"
	    where
	      "GroupId" = @v_group_id#CommandCount#
	    ;
    end
	-- 子グループを追加
	if @isChildInsert#CommandCount# = 'True'
	begin
		insert into
		  "GroupChildren" ("GroupId","ChildId","Creator","Updator")
		  select @v_group_id#CommandCount#, "t5"."GroupId", @_U, @_U
		  from "Groups" as "t5"
		  where (1=1)
		    and "t5"."TenantId" = @TenantId#CommandCount#
		    and {{groupGuids_condition}}
		  ;
	end
end;