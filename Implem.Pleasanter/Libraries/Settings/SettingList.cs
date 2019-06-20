using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class SettingList<T> : List<T> where T : ISettingListItem
    {
        public T Get(int? id)
        {
            return this.FirstOrDefault(o => o.Id == id);
        }

        public void MoveUpOrDown(string command, IEnumerable<int> selected)
        {
            var order = this.Select(o => o.Id).ToArray();
            switch (command)
            {
                case "MoveUp":
                case "MoveDown":
                    if (command == "MoveDown") Array.Reverse(order);
                    order.Select((o, i) => new { Id = o, Index = i }).ForEach(data =>
                    {
                        if (selected.Contains(data.Id))
                        {
                            if (data.Index > 0 &&
                                !selected.Contains(order[data.Index - 1]))
                            {
                                order = Arrays.Swap(order, data.Index, data.Index - 1);
                            }
                        }
                    });
                    if (command == "MoveDown") Array.Reverse(order);
                    var _new = order
                        .Select(o => this.FirstOrDefault(p => p.Id == o))
                        .Where(o => o != null)
                        .ToList();
                    Clear();
                    _new.ForEach(o => Add(o));
                    break;
            }
        }

        public void Delete(IEnumerable<int> selected)
        {
            RemoveAll(o => selected?.Contains(o.Id) == true);
        }
    }
}