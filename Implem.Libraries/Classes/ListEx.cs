using System.Collections.Generic;
namespace Implem.Libraries.Classes
{
    public class ListEx<T> : List<T>
    {
        protected new ListEx<T> Add(T item)
        {
            base.Add(item);
            return this;
        }
    }
}
