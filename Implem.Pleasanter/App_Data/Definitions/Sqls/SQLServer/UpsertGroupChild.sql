merge into "GroupChildren" as "target"
using
(
    select
        @GroupId as "GroupId"
        ,@ChildId as "ChildId"
    
) as item
on 
    "target"."GroupId" = "item"."GroupId"
    and "target"."ChildId" = "item"."ChildId"
when matched 
then
    update set
        "GroupId" = "item"."GroupId"
        ,"ChildId" = "item"."ChildId"
        ,"Updator" = @_U
when not matched 
then
    insert 
    (
        "GroupId",
        "ChildId",
        "Updator",
        "Creator"
    )
    values
    (
        "item"."GroupId"
        ,"item"."ChildId"
        ,@_U
        ,@_U
    );