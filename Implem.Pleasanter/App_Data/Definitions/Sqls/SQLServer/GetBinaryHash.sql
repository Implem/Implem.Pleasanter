select hashbytes(@Algorithm, cast("Bin" as varbinary(max)))
from "Binaries"
where "Guid" = @Guid;