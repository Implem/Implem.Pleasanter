if exists(
    select *
    from "{0}"
    where
        "{0}"."SiteId"={1}
        and "{0}"."{4}"=@{4}_#CommandCount#
        and "{0}"."{2}"<>{3}
    )
begin;
    rollback;
    select '{{"Event":"Duplicated","Id":{3},"ColumnName":"{4}"}}';
    return;
end;
