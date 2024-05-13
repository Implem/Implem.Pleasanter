merge into "GroupMembers" as "target"
using
(
    select
        @GroupId as "GroupId"
        ,@DeptId as "DeptId"
        ,@UserId as "UserId"
    
) as item
on 
    "target"."GroupId" = "item"."GroupId"
    and "target"."DeptId" = "item"."DeptId"
    and "target"."UserId" = "item"."UserId"
    and "target"."ChildGroup" = 0
when matched 
then
    update set
        "GroupId" = "item"."GroupId"
        ,"DeptId" = "item"."DeptId"
        ,"UserId" = "item"."UserId"
        ,"Admin" = @Admin
        ,"Updator" = @_U
when not matched 
then
    insert 
    (
        "GroupId",
        "DeptId",
        "UserId",
        "Admin",
        "Updator",
        "Creator"
    )
    values
    (
        "item"."GroupId"
        ,"item"."DeptId"
        ,"item"."UserId"
        ,@Admin
        ,@_U
        ,@_U
    );