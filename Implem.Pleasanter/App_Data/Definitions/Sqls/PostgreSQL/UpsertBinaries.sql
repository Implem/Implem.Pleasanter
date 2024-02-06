update "Binaries" as "target"
set
    "Bin" = "target"."Bin" || "temp"."Bin"
    ,"Updator" = "temp"."Updator"
    ,"UpdatedTime" = current_timestamp
from (
    select
        @TenantId as "TenantId"
        ,@ReferenceId as "ReferenceId"
        ,@Guid as "Guid"
        ,@BinaryType as "BinaryType"
        ,@Title as "Title"
        ,@Bin as "Bin"
        ,@Creator as "Creator"
        ,@Updator as "Updator"
) as "temp"
where "target"."Guid" = "temp"."Guid"
    and target."BinaryType" = "temp"."BinaryType";

insert into "Binaries" (
    "TenantId"
    ,"ReferenceId"
    ,"Guid"
    ,"Ver"
    ,"BinaryType"
    ,"Title"
    ,"Bin"
    ,"Creator"
    ,"Updator"
    ,"CreatedTime"
    ,"UpdatedTime"
)
select
    "temp"."TenantId"
    ,"temp"."ReferenceId"
    ,"temp"."Guid"
    ,1 as "Ver"
    ,"temp"."BinaryType"
    ,"temp"."Title"
    ,"temp"."Bin"
    ,"temp"."Creator"
    ,"temp"."Updator"
    ,current_timestamp as "CreatedTime"
    ,current_timestamp as "UpdatedTime"
from (
    select
        @TenantId as "TenantId"
        ,@ReferenceId as "ReferenceId"
        ,@Guid as "Guid"
        ,@BinaryType as "BinaryType"
        ,@Title as "Title"
        ,@Bin as "Bin"
        ,@Creator as "Creator"
        ,@Updator as "Updator"
) as "temp"
left join "Binaries" as "target" 
on "target"."Guid" = "temp"."Guid"
    and "target"."BinaryType" = "temp"."BinaryType"
where "target"."Guid" is null;