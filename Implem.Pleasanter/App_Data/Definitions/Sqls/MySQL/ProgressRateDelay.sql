(
  case
    when #TableBracket#."CompletionTime" < current_timestamp(3) then
      100
    else
      case 
        when #TableBracket#."StartTime" is null then
          case
            when timediff(#TableBracket#."CreatedTime", #TableBracket#."CompletionTime") <> 0 then
              cast(timediff(#TableBracket#."CreatedTime", current_timestamp(3)) as float) /
              cast(timediff(#TableBracket#."CreatedTime", #TableBracket#."CompletionTime") as float) * 100
            else
              0
          end
        else
          case
            when timediff(#TableBracket#."StartTime", #TableBracket#."CompletionTime") <> 0 then
              cast(timediff(#TableBracket#."StartTime", current_timestamp(3)) as float) /
              cast(timediff(#TableBracket#."StartTime", #TableBracket#."CompletionTime") as float) * 100
            else
              0
          end
      end
  end
)