using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class JoinStack
    {
        public string Title;
        public long DestinationId;
        public long SourceId;
        public string ColumnName;
        public string Direction;
        public JoinStack Next;

        public JoinStack(
            string title,
            long destinationId,
            long sourceId,
            string columnName,
            string direction,
            JoinStack next)
        {
            Title = title;
            DestinationId = destinationId;
            SourceId = sourceId;
            ColumnName = columnName;
            Direction = direction;
            Next = next;
        }

        public string TableName()
        {
            switch (Direction)
            {
                case "Destinations":
                    return Stacks(this.ToSingleList(), reverce: true)
                        .Select(o => $"{o.ColumnName}~{o.DestinationId}")
                        .Join("-");
                case "Sources":
                    return Stacks(this.ToSingleList(), reverce: true)
                        .Select(o => $"{o.ColumnName}~~{o.SourceId}")
                        .Join("-");
                default:
                    return null;
            }
        }

        public string DisplayName(string currentTitle)
        {
            switch (Direction)
            {
                case "Destinations":
                    var destinations = Stacks(this.ToSingleList());
                    return destinations
                        .Select((o, i) => new { Stack = o, Index = i })
                        .Select(o => o.Index == 0
                            ? "[" + o.Stack.Title + "]"
                            : o.Stack.Title)
                        .Join(" -< ") + " -< " + currentTitle;
                case "Sources":
                    var sources = Stacks(this.ToSingleList(), reverce: true);
                    return currentTitle + " -< "
                        + sources
                            .Select((o, i) => new { Stack = o, Index = i })
                            .Select(o => o.Index + 1 == sources.Count()
                                ? "[" + o.Stack.Title + "]"
                                : o.Stack.Title)
                            .Join(" -< ");
                default:
                    return null;
            }
        }

        private List<JoinStack> Stacks(List<JoinStack> stacks, bool reverce = false)
        {
            if (Next != null)
            {
                if (reverce)
                {
                    stacks.Insert(0, Next);
                }
                else
                {
                    stacks.Add(Next);
                }
                Next.Stacks(stacks, reverce);
            }
            return stacks;
        }
    }
}