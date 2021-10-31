using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class PasswordHistory
    {
        public string Password;
        public int Creator;
        public DateTime CreatedTime;
    }
}