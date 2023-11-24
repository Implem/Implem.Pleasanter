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
    ,@_U
    ,@_U
)
on confrict
(   "GroupId"
    ,"DeptId"
    ,"UserId"
)
do update
set
    "GroupId" = @GroupId
    ,"DeptId" = @DeptId
    ,"UserId" = @UserId
    ,"Admin" = @Admin
    ,"Updator" = @_U
;