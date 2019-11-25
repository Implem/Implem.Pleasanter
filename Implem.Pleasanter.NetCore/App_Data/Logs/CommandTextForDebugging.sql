use [Implem.Pleasanter];
declare @@_T int;                                 set @@_T = '1';
declare @@_D int;                                 set @@_D = '3';
declare @@_U int;                                 set @@_U = '1';
declare @ReferenceId bigint;                      set @ReferenceId = '382';
declare @BinaryType nvarchar(9);                  set @BinaryType = 'SiteImage';

select max("Binaries"."UpdatedTime") as "UpdatedTimeMax" 
from "Binaries" as "Binaries"
where ("Binaries"."ReferenceId"=@ReferenceId) and ("Binaries"."BinaryType"=@BinaryType) ;
