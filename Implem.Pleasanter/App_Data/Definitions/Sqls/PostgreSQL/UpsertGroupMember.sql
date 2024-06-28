insert into "GroupMembers"
(
    "GroupId"
    ,"ChildGroup"
    ,"DeptId"
    ,"UserId"
    ,"Admin"
    ,"Updator"
    ,"Creator"
)
values
(
    @GroupId
    ,false
    ,@DeptId
    ,@UserId
    ,@Admin
    ,@ipU
    ,@ipU
)
on conflict
(
    "GroupId"
    ,"ChildGroup"
    ,"DeptId"
    ,"UserId"
)
do update
set
    "GroupId" = @GroupId
    ,"DeptId" = @DeptId
    ,"UserId" = @UserId
    ,"Admin" = @Admin
    ,"Updator" = @ipU
;