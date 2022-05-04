set @_C = @@rowcount;
if @_C = 0
begin
    rollback;
    select '{{"Event":"Conflicted","Id":{0},"Count":' + convert(nvarchar,@_C) + '}}';
    return;
end
else
begin
    select '{{"Id":{0},"Count":' + convert(nvarchar,@_C) + '}}';
end
