insert into "GroupChildren"
(
    "GroupId"
    ,"ChildId"
    ,"Updator"
    ,"Creator"
)
values
(
    @GroupId
    ,@ChildId
    ,@ipU
    ,@ipU
)
on duplicate key update
    "GroupId" = @GroupId
    ,"ChildId" = @ChildId
    ,"Updator" = @ipU;