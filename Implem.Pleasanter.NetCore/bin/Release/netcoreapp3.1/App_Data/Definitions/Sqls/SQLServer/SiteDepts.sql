select "Depts"."DeptId", "Depts"."DeptName"
from "Depts"
where "Depts"."DeptId" in
( 
	select "Permissions"."DeptId" from "Permissions"
	where "Permissions"."ReferenceType" = @ReferenceType_ and "Permissions"."ReferenceId" = @ReferenceId_ and DeptId > 0
);