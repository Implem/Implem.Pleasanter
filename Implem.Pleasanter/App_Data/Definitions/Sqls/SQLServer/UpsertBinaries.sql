merge into "Binaries" as "target"
using
(
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
on "target"."Guid" = "temp"."Guid"
    and "target"."BinaryType" = "temp"."BinaryType"
when matched
then
    update set 
        "Bin" = concat(cast("target"."Bin" as varbinary(max)), cast("temp"."Bin" as varbinary(max)))
        ,"Updator" = @Updator
        ,"UpdatedTime" = CURRENT_TIMESTAMP
when not matched
then
    insert
    (
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
    values
    (
        @TenantId
        ,@ReferenceId
        ,@Guid
        ,1
        ,@BinaryType
        ,@Title
        ,@Bin
        ,@Creator
        ,@Updator
        ,CURRENT_TIMESTAMP
        ,CURRENT_TIMESTAMP
    );