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
	"GroupsChildIdsInSelfId"("Lv", "GroupId", "ChildId") as ( 
		-- 子GroupIdリスト(自ID含まない)
		select
			1 as "Lv"
			, "t2"."GroupId"
			, "t2"."ChildId"
		from
			"GroupChildren" as "t2"
			inner join "Groups" as "t3" on "t2"."GroupId"="t3"."GroupId"
		where
			("t3"."Disabled" = 0)
			and "t2"."GroupId" = @v_group2
		union all 
		select
			"t1"."Lv" + 1
			, "t2"."GroupId"
			, "t2"."ChildId"
		from
			"GroupsChildIdsInSelfId" as "t1"
			, "GroupChildren" as "t2" 
			inner join "Groups" as "t3" on "t2"."GroupId"="t3"."GroupId"
		where
			"t1"."Lv" < @GroupsDepthMax#CommandCount# 
			and "t3"."Disabled" = 0 
			and "t1"."ChildId" = "t2"."GroupId"
	) 
		MERGE INTO
			"GroupMembers" as "tgt"
		USING
			(
			SELECT 
			  DISTINCT
			  @v_group2 as "GroupId"
			  ,"DeptId"
			  ,"UserId"
			  ,1 as "ChildGroup"
			FROM
				"GroupMembers" as "t9"
			WHERE (1=1)
				and "t9"."ChildGroup" = 0
				and "t9"."GroupId" in (
					select
						DISTINCT
						"ChildId" as "GroupId"
					from
						"GroupsChildIdsInSelfId"
				)
			) AS "src"
		ON
			(
			  "tgt"."GroupId" = "src"."GroupId"
			  AND "tgt"."DeptId" = "src"."DeptId"
			  AND "tgt"."UserId" = "src"."UserId"
			  AND "tgt"."ChildGroup" = "src"."ChildGroup"
			)
		WHEN NOT MATCHED BY TARGET THEN
			-- レコードを挿入
			INSERT
			(
			  "GroupId"
			  ,"DeptId"
			  ,"UserId"
			  ,"ChildGroup"
			  ,"Creator"
			  ,"Updator"
			)
			VALUES
			(
			  "src"."GroupId"
			  ,"src"."DeptId"
			  ,"src"."UserId"
			  ,"src"."ChildGroup"
			  ,@_U
			  ,@_U
			)
		WHEN NOT MATCHED BY SOURCE
			-- レコードを削除
			and ("tgt"."GroupId" = @v_group2 AND "tgt"."ChildGroup" <> 0)
			THEN
				DELETE
		;
	fetch next 
		from
			v_cursor 
		into @v_group1; 
end close v_cursor; 
deallocate v_cursor;
