using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class NavigationMenu : ExtendedBase
    {
        public string ContainerId;
        public string MenuId;
        public string Id;
        public string Css;
        public string Icon;
        public string Url;
        public string Function;
        public List<string> LinkParams;
        public string Target;
        public List<string> ReferenceTypes;
        public List<NavigationMenu> ChildMenus;
    }
}