use [Implem.Pleasanter];
declare @@_T int;                                 set @@_T = '1';
declare @@_D int;                                 set @@_D = '3';
declare @@_U int;                                 set @@_U = '1';
declare @ReferenceId bigint;                      set @ReferenceId = '1';
declare @BinaryType nvarchar(11);                 set @BinaryType = 'TenantImage';

select count(*) as "BinariesCount" 
from "Binaries" as "Binaries"
where ("Binaries"."ReferenceId"=@ReferenceId) and ("Binaries"."BinaryType"=@BinaryType) ;
