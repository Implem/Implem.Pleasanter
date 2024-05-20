(
  case
    when #TableBracket#."CompletionTime" < CURRENT_TIMESTAMP then
      100
    else
      case 
        when #TableBracket#."StartTime" is null then
          case
            when (EXTRACT(epoch from #TableBracket#."CompletionTime")-EXTRACT(epoch from #TableBracket#."CreatedTime")) <> 0 then
                (EXTRACT(epoch from CURRENT_TIMESTAMP) - EXTRACT(epoch from #TableBracket#."CreatedTime")) /
                (EXTRACT(epoch from #TableBracket#."CompletionTime") - EXTRACT(epoch from #TableBracket#."CreatedTime")) * 100
            else
              0
          end
        else
          case
            when (EXTRACT(epoch from #TableBracket#."CompletionTime")-EXTRACT(epoch from #TableBracket#."StartTime")) <> 0 then
                (EXTRACT(epoch from CURRENT_TIMESTAMP) - EXTRACT(epoch from #TableBracket#."StartTime")) /
                (EXTRACT(epoch from #TableBracket#."CompletionTime") - EXTRACT(epoch from #TableBracket#."StartTime")) * 100
            else
              0
          end
      end
  end
)