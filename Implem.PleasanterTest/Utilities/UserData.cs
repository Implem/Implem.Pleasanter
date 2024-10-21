using Implem.Libraries.Utilities;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    public static class UserData
    {
        public enum UserTypes
        {
            Anonymous,
            TenantManager1,
            TenantManager2,
            General1,
            General2,
            General3,
            General4,
            General5,
            DisabledDept,
            DisabledGroup,
            Privileged,
            Lockout,
            Disabled,
            UserDeptsGroups
        }

        public static UserModel Get(int userId)
        {
            var userModel = Initializer.Users.Get(userId);
            return userModel;
        }

        public static UserModel Get(UserTypes userType)
        {
            UserModel userModel = null;
            switch (userType)
            {
                case UserTypes.TenantManager1:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User1"));
                    break;
                case UserTypes.TenantManager2:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User11"));
                    break;
                case UserTypes.General1:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User2"));
                    break;
                case UserTypes.General2:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User3"));
                    break;
                case UserTypes.General3:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User4"));
                    break;
                case UserTypes.General4:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User5"));
                    break;
                case UserTypes.General5:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User6"));
                    break;
                case UserTypes.DisabledDept:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User16"));
                    break;
                case UserTypes.DisabledGroup:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User17"));
                    break;
                case UserTypes.Disabled:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User18"));
                    break;
                case UserTypes.Lockout:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User19"));
                    break;
                case UserTypes.Privileged:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User20"));
                    break;
                case UserTypes.UserDeptsGroups:
                    userModel = Initializer.Users.Values.FirstOrDefault(o => o.LoginId.EndsWith("User23_Site_SScript_UsersDeptsGroups"));
                    break;
            }
            return userModel;
        }

        public static IEnumerable<UserTypes> GetUserTypePatterns()
        {
            yield return UserTypes.TenantManager1;
            yield return UserTypes.General1;
            yield return UserTypes.DisabledDept;
            yield return UserTypes.DisabledGroup;
            yield return UserTypes.Disabled;
            yield return UserTypes.Lockout;
            yield return UserTypes.Privileged;
        }
    }
}
