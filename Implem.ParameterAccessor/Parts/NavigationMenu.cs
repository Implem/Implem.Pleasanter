using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class NavigationMenu
    {
        public string ContainerId;
        public string MenuId;
        public string Id;
        public string Name;
        public string Css;
        public string Icon;
        public string Url;
        public string Function;
        public List<string> LinkParams;
        public string Target;
        public List<string> Controllers;
        public List<string> ReferenceTypes;
        public List<string> Actions;
        public List<NavigationMenu> ChildMenus;
        public bool Disabled;
    }
}