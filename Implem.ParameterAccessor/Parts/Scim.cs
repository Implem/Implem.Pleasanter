namespace Implem.ParameterAccessor.Parts
{
    public class Scim
    {
        public bool Enabled;
        public bool SwaggerEnabled;
        public ScimExtendedAttributes ExtendedAttributes;
    }

    public class ScimExtendedAttributes
    {
        public System.Collections.Generic.List<ScimExtendedAttribute> Users;
        public System.Collections.Generic.List<ScimExtendedAttribute> Groups;
    }

    public class ScimExtendedAttribute
    {
        public string Schema;
        public string Name;
        public string ColumnName;
    }
}
