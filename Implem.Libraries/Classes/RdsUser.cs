using Implem.Libraries.Utilities;
using System;
namespace Implem.Libraries.Classes
{
    [Serializable]
    public class RdsUser
    {
        public int DeptId;
        public int UserId;

        public enum UserTypes : int
        {
            System = 1,
            Anonymous = 2
        }

        public RdsUser()
        {
        }

        public RdsUser(UserTypes userType)
        {
            Constructor(userType.ToInt());
        }

        protected void Constructor(int userId = 1, int deptId = 0)
        {
            UserId = userId;
            DeptId = deptId;
        }
    }
}
