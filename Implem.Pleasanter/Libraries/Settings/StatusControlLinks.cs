using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    /// <summary>
    /// 状況による制御でリンク(子サイト)を対象にした制御を扱う
    /// ColumnHashのキーに"_Links-{子サイトID}"を使用する
    /// ReadOnly: 作成ボタンを押せなくする
    /// Hidden: 作成ボタンを表示しない
    /// Required: 子のレコードが1件もない場合は更新時にエラーとする
    /// </summary>
    public static class StatusControlLinks
    {
        public static bool HasLinkControls(SiteSettings ss)
        {
            return ss.StatusControls?.Any(statusControl =>
                statusControl.Disabled != true
                && statusControl.ColumnHash?.Keys.Any(columnName =>
                    ss.LinkId(columnName) > 0) == true) == true;
        }

        public static StatusControl.ControlConstraintsTypes ControlType(
            SiteSettings ss,
            Dictionary<string, StatusControl.ControlConstraintsTypes> statusControlHash,
            long sourceId)
        {
            return statusControlHash?.Get(ss.LinkId(sourceId))
                ?? StatusControl.ControlConstraintsTypes.None;
        }

        public static ErrorData OnUpdating(
            Context context,
            SiteSettings ss,
            long id,
            Dictionary<string, StatusControl.ControlConstraintsTypes> statusControlHash,
            bool api = false)
        {
            var sourceSs = statusControlHash?
                .Where(o => o.Value == StatusControl.ControlConstraintsTypes.Required)
                .Select(o => ss.Sources?.Get(ss.LinkId(o.Key)))
                .Where(o => o != null)
                .FirstOrDefault(o => Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectLinks(
                        column: Rds.LinksColumn().LinksCount(),
                        join: Rds.LinksJoinDefault(),
                        where: Rds.LinksWhere()
                            .DestinationId(id)
                            .SiteId(o.SiteId))) == 0);
            return sourceSs != null
                ? new ErrorData(
                    context: context,
                    type: Error.Types.LinkedRecordRequired,
                    api: api,
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription(),
                    data: sourceSs.Title)
                : new ErrorData(type: Error.Types.None);
        }
    }
}
