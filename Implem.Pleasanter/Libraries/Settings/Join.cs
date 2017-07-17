using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Join : List<Link>
    {
        [NonSerialized]
        public List<string> Title;

        public Join(string title)
        {
            Title = new List<string>() { title };
        }

        public Join(List<Link> links)
        {
            links.ForEach(link => Add(link));
        }

        public void Add(Link link, string title)
        {
            Add(link);
            Title.Insert(0, title);
        }

        public Join ToList()
        {
            return new Join(this) { Title = Title.ToList() };
        }
    }
}