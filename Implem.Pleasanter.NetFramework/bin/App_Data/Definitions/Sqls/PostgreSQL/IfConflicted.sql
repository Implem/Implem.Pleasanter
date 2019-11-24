set @ipC = @@rowcount;
if @ipC = 0
begin
    rollback;
    select '{{"Event":"Conflicted","Id":{0},"Count":' + convert(nvarchar,@ipC) + '}}';
    return;
end
else
begin
    select '{{"Id":{0},"Count":' + convert(nvarchar,@ipC) + '}}';
end
