using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelUsers
    {
        private readonly Context Context;
        private HashSet<string> ReadableColumnNames;

        public ServerScriptModelUsers(Context context)
        {
            Context = context;
        }

        private HashSet<string> GetReadableColumnNames(SiteSettings ss = null)
        {
            return ReadableColumnNames ??= ServerScriptUtilities.ReadableColumnNames(
                context: Context,
                ss: ss ?? SiteSettingsUtilities.UsersSiteSettings(context: Context));
        }

        public ServerScriptModelUserModel Get(object id)
        {
            var user = User.GetWithExtras(
                context: Context,
                tenantId: Context.TenantId,
                userId: id.ToInt());
            ServerScriptModelUserModel userModel = null;
            if (!user.Anonymous())
            {
                var readableColumnNames = GetReadableColumnNames();
                userModel = new ServerScriptModelUserModel(
                    context: Context,
                    tenantId: user.TenantId,
                    userId: user.Id,
                    deptId: user.DeptId,
                    loginId: user.LoginId,
                    name: user.Name,
                    userCode: user.UserCode,
                    tenantManager: user.TenantManager,
                    serviceManager: user.ServiceManager,
                    disabled: user.Disabled,
                    extras: user.Extras,
                    readableColumnNames: readableColumnNames);
            }
            return userModel;
        }

        public ServerScriptModelUserModel[] GetList(string view = null, int offset = 0, int pageSize = 0)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: Context);
            var readableColumnNames = GetReadableColumnNames(ss: ss);
            return UserUtilities.GetListByServerScript(
                context: Context,
                ss: ss,
                view: view,
                offset: offset,
                pageSize: pageSize)
                    .Select(user => new ServerScriptModelUserModel(
                        context: Context,
                        tenantId: user.TenantId,
                        userId: user.Id,
                        deptId: user.DeptId,
                        loginId: user.LoginId,
                        name: user.Name,
                        userCode: user.UserCode,
                        tenantManager: user.TenantManager,
                        serviceManager: user.ServiceManager,
                        disabled: user.Disabled,
                        extras: user.Extras,
                        readableColumnNames: readableColumnNames))
                    .ToArray();
        }

        public bool Create(string model)
        {
            var id = 0;
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Users",
                action: "Create",
                id: id,
                apiRequestBody: model);
            return UserUtilities.CreateByServerScript(
                context: apiContext,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: apiContext),
                userId: id);
        }

        public bool Update(object id, string model)
        {
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Users",
                action: "Update",
                id: id.ToInt(),
                apiRequestBody: model);
            return UserUtilities.UpdateByServerScript(
                context: apiContext,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: apiContext),
                userId: id.ToInt());
        }
    }
}