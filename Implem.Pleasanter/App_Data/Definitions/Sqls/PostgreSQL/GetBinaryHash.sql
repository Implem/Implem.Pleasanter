select digest("Bin", @Algorithm)
from "Binaries"
where "Guid" = @Guid;