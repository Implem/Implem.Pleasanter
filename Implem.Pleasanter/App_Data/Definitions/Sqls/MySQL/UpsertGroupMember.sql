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
    ,0
    ,@DeptId
    ,@UserId
    ,@Admin
    ,@ipU
    ,@ipU
)
on duplicate key update
    "GroupId" = @GroupId
    ,"DeptId" = @DeptId
    ,"UserId" = @UserId
    ,"Admin" = @Admin
    ,"Updator" = @ipU;