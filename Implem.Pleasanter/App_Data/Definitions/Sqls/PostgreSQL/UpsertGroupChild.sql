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
on conflict
(
    "GroupId"
    ,"ChildId"
)
do update
set
    "GroupId" = @GroupId
    ,"ChildId" = @ChildId
    ,"Updator" = @ipU;