update "GroupMembers"
set
    "GroupId" = @GroupId
    ,"DeptId" = @DeptId
    ,"UserId" = @UserId
    ,"Admin" = @Admin
    ,"Updator" = @ipU
where "GroupId" = @GroupId
    and "ChildGroup" = 0
    and "DeptId" = @DeptId
    and "UserId" = @UserId;
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
select * from
(
    select
        @GroupId as "GroupId"
        ,0 as "ChildGroup"
        ,@DeptId as "DeptId"
        ,@UserId as "UserId"
        ,@Admin as "Admin"
        ,@ipU as "Updator"
        ,@ipU as "Creator"
)
as tmp
where not exists
(
    select 1
    from "GroupMembers"
    where "GroupId"= @GroupId
        and "ChildGroup" = 0
        and "DeptId" = @DeptId
        and "UserId" = @UserId
);