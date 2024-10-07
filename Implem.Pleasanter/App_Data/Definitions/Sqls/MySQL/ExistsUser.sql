select distinct "user"
from "mysql"."user"
where "user" = '#Uid#'
    and "host" = 'localhost';