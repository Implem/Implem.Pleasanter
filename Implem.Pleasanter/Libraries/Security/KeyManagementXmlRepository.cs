using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Implem.Pleasanter.Libraries.Security
{
    public class KeyManagementXmlRepository : IXmlRepository
    {
        private const string SessionGuid = "@AspNetCoreDataProtectionKeys";

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var context = GetContext();
            var cnt = GetWaitCount();
            new SysLogModel(
                context: context,
                method: nameof(KeyManagementXmlRepository) + "_" + nameof(GetAllElements),
                message: new { cmd = "KeyManagementXmlRepository.GetAllElements()", loop_cnt = cnt }.ToJson(),
                sysLogType: SysLogModel.SysLogTypes.Info);
            // 初回起動時で複数Webサーバ同時起動で複数個Keyを作成しないためにWebサーバ毎に乱数でWaitし一個しか作らないようにする。
            for (var i = 0; i < cnt; i++)
            {
                if (i != 0) Task.Delay(500).Wait();
                var list = GetKeyList(context);
                if (list.Count != 0)
                {
                    return list
                            .Select(v => XElement.Parse(v.Value))
                            .ToList();
                }
            }
            return new List<XElement>();
        }

        private int GetWaitCount()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode() % 30);
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
                method: nameof(KeyManagementXmlRepository) + "_" + nameof(StoreElement),
                message: new { cmd = $"KeyManagementXmlRepository.StoreElement(); friendlyName={friendlyName}" }.ToJson(),
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
                Controller = "KeyManagementXmlRepository.cs"
            };
        }
    }
}