using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class SamlAttributes : Dictionary<string, string>
    {
        public new string this[string key]
        {
            get => TryGetValue(key, out string value) ? value : null;
            set => base[key] = value;
        }

        public string UserName 
        {   
            get
            {
                if (!string.IsNullOrEmpty(this["Name"]))
                {
                    return this["Name"];
                }
                if (string.IsNullOrEmpty(this["FirstName"]) && string.IsNullOrEmpty(this["LastName"]))
                {
                    return null;
                }
                if (string.IsNullOrEmpty(this["FirstName"]))
                {
                    return this["LastName"];
                }
                if (string.IsNullOrEmpty(this["LastName"]))
                {
                    return this["FirstName"];
                }
                var nameOrder = this["FirstAndLastNameOrder"];
                return nameOrder == "1"
                    ? this["FirstName"] + " " + this["LastName"] 
                    : this["LastName"] + " " + this["FirstName"];
            }
        }

        public bool TenantManager
        {
            get
            {
                return this["TenantManager"]?.ToLower() == "true" ? true : false;
            }
        }
    }
}
