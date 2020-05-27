using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Extension
    {
        public static bool Authenticate(
            Context context,
            string loginId,
            string password,
            UserModel userModel)
        {
            throw new NotImplementedException();
        }
}