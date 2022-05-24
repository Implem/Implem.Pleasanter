(
  case
    when #TableBracket#."CompletionTime" < getdate() then
      100
    else
      case 
        when #TableBracket#."StartTime" is null then
          case
            when datediff(second, #TableBracket#."CreatedTime", #TableBracket#."CompletionTime") <> 0 then
              cast(datediff(second, #TableBracket#."CreatedTime", getdate()) as float) /
              cast(datediff(second, #TableBracket#."CreatedTime", #TableBracket#."CompletionTime") as float) * 100
            else
              0
          end
        else
          case
            when datediff(second, #TableBracket#."StartTime", #TableBracket#."CompletionTime") <> 0 then
              cast(datediff(second, #TableBracket#."StartTime", getdate()) as float) /
              cast(datediff(second, #TableBracket#."StartTime", #TableBracket#."CompletionTime") as float) * 100
            else
              0
          end
      end
  end
)