update "GroupChildren"
set
    "GroupId" = @GroupId
    ,"ChildId" = @ChildId
    ,"Updator" = @ipU
where "GroupId" = @GroupId
    and "ChildId" = @ChildId;
insert into "GroupChildren"
(
    "GroupId"
    ,"ChildId"
    ,"Updator"
    ,"Creator"
)
select * from 
(
    select
        @GroupId as "GroupId"
        ,@ChildId as "ChildId"
        ,@ipU as "Updator"
        ,@ipU as "Creator"
)
as tmp
where not exists
(
    select 1
    from "GroupChildren"
    where "GroupId" = @GroupId
        and "ChildId" = @ChildId
);