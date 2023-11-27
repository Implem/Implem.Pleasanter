insert into "GroupMembers"
(
    "GroupId"
    ,"DeptId"
    ,"UserId"
    ,"Admin"
    ,"Updator"
    ,"Creator"
)
values
(
    @GroupId
    ,@DeptId
    ,@UserId
    ,@Admin
    ,@ipU
    ,@ipU
)
on conflict
(
    "GroupId"
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