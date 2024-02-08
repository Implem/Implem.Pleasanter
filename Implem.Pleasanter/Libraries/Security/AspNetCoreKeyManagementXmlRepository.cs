using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Implem.Pleasanter.Libraries.Security
{
    public class AspNetCoreKeyManagementXmlRepository : IXmlRepository
    {
        private const string SessionGuid = "@AspNetCoreDataProtectionKeys";

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var context = GetContext();
            new SysLogModel(
                context: context,
                method: nameof(AspNetCoreKeyManagementXmlRepository) + "_" + nameof(GetAllElements),
                message: new { cmd = "AspNetCoreKeyManagementXmlRepository.GetAllElements()" }.ToJson(),
                sysLogType: SysLogModel.SysLogTypes.Info);
            return GetKeyList(context)
                .Select(v => XElement.Parse(v.Value))
                .ToList();
        }

        private static IDictionary<string, string> GetKeyList(Context context)
        {
            return SessionUtilities.Get(
                context: context,
                sessionGuid: SessionGuid);
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var context = GetContext();
            new SysLogModel(
                context: context,
                method: nameof(AspNetCoreKeyManagementXmlRepository) + "_" + nameof(StoreElement),
                message: new { cmd = $"AspNetCoreKeyManagementXmlRepository.StoreElement(); friendlyName={friendlyName}" }.ToJson(),
                sysLogType: SysLogModel.SysLogTypes.Info);
            DeleteOldSessions(context);
            SessionUtilities.Set(
                context: context,
                key: friendlyName,
                value: element.ToString(SaveOptions.DisableFormatting),
                sessionGuid: SessionGuid);
        }

        private void DeleteOldSessions(Context context)
        {
            // 期限切れのキーを削除する。(expirationDateより、更に90日余裕を持たせる)
            var list = GetKeyList(context)
                .Where(v => (XElement.Parse(v.Value).Element("expirationDate")?.Value?.ToDateTime().AddDays(90) ?? DateTime.Now) < DateTime.Now)
                .Select(v => v.Key)
                .ToList();
            foreach (var item in list)
            {
                SessionUtilities.Remove(
                    context: context,
                    key: item,
                    page: false,
                    sessionGuid: SessionGuid);
            }
        }

        private Context GetContext()
        {
            return new Context(
                request: false,
                sessionData: false,
                sessionStatus: false,
                user: false,
                setPermissions: false)
            {
                Controller = "AspNetCoreKeyManagementXmlRepository.cs"
            };
        }
    }
}