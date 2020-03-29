use [Implem.Pleasanter];
declare @@_T int;                                 set @@_T = '1';
declare @@_D int;                                 set @@_D = '3';
declare @@_U int;                                 set @@_U = '1';
declare @SiteId bigint;                           set @SiteId = '10368';

select "Wikis"."WikiId" 
from "Wikis" as "Wikis"
where ("Wikis"."SiteId"=@SiteId) ;
