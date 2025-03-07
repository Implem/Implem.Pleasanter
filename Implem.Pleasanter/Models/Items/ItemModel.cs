using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ItemModel : BaseModel
    {
        public long ReferenceId = 0;
        public string ReferenceType = string.Empty;
        public long SiteId = 0;
        public string Title = string.Empty;
        public SiteModel Site = null;
        public string FullText = string.Empty;
        public DateTime SearchIndexCreatedTime = 0.ToDateTime();
        public long SavedReferenceId = 0;
        public string SavedReferenceType = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public SiteModel SavedSite = null;
        public string SavedFullText = string.Empty;
        public DateTime SavedSearchIndexCreatedTime = 0.ToDateTime();

        public bool ReferenceId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != ReferenceId;
            }
            return ReferenceId != SavedReferenceId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != ReferenceId);
        }

        public bool ReferenceType_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ReferenceType;
            }
            return ReferenceType != SavedReferenceType && ReferenceType != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ReferenceType);
        }

        public bool SiteId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != SiteId;
            }
            return SiteId != SavedSiteId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != SiteId);
        }

        public bool Title_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Title;
            }
            return Title != SavedTitle && Title != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Title);
        }

        public bool FullText_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != FullText;
            }
            return FullText != SavedFullText && FullText != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != FullText);
        }

        public bool SearchIndexCreatedTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != SearchIndexCreatedTime;
            }
            return SearchIndexCreatedTime != SavedSearchIndexCreatedTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != SearchIndexCreatedTime.Date);
        }

        public ItemModel(
            Context context,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            if (dataRow != null)
            {
                Set(
                    context: context,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public ItemModel Get(
            Context context,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            where = where ?? Rds.ItemsWhereDefault(
                context: context,
                itemModel: this);
            column = (column ?? Rds.ItemsDefaultColumns());
            join = join ?? Rds.ItemsJoinDefault();
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectItems(
                    tableType: tableType,
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public ContentResultInheritance ExportByApi(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    if (SiteId == ReferenceId)
                    {
                        return IssueUtilities.ExportByApi(
                            context: context,
                            ss: Site.SiteSettings,
                            siteModel: Site);
                    }
                    break;
                case "Results":
                    if (SiteId == ReferenceId)
                    {
                        return ResultUtilities.ExportByApi(
                            context: context,
                            ss: Site.SiteSettings,
                            siteModel: Site);
                    }
                    break;
                default:
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            return ApiResults.Get(ApiResponses.BadRequest(context: context));
        }

        public bool ExportByServerScript(Context context, string filePath)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ExportByServerScript(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        filePath: filePath);
                case "Results":
                    return ResultUtilities.ExportByServerScript(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        filePath: filePath);
                default:
                    return false;
            }
        }

        public ContentResultInheritance ImportByApi(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    if (SiteId == ReferenceId)
                    {
                        return IssueUtilities.ImportByApi(
                            context: context,
                            ss: Site.SiteSettings,
                            siteModel: Site);
                    }
                    break;
                case "Results":
                    if (SiteId == ReferenceId)
                    {
                        return ResultUtilities.ImportByApi(
                            context: context,
                            ss: Site.SiteSettings,
                            siteModel: Site);
                    }
                    break;
                default:
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            return ApiResults.Get(ApiResponses.BadRequest(context: context));
        }

        public string ImportByServerScript(Context context, string filePath)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ImportByServerScript(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        filePath: filePath);
                case "Results":
                    return ResultUtilities.ImportByServerScript(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId,
                            setSiteIntegration: true),
                        filePath: filePath);
                default:
                    return null;
            }
        }

        public string Index(Context context)
        {
            if (ReferenceId == 0)
            {
                return SiteUtilities.SiteTop(context: context);
            }
            if (ReferenceType != "Sites")
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        sysLogsStatus: 404,
                        sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.SiteMenu(context: context, siteModel: Site);
                case "Dashboards":
                    return DashboardUtilities.Index(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.Index(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.Index(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string IndexJson(Context context)
        {
            if (ReferenceType != "Sites")
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.IndexJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Dashboards":
                    return DashboardUtilities.IndexJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.IndexJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.IndexJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string TrashBox(Context context)
        {
            if (ReferenceId != 0 && ReferenceType != "Sites")
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        sysLogsStatus: 404,
                        sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                tableType: Sqls.TableTypes.Deleted);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            if (ReferenceId == 0)
            {
                if (!context.HasPrivilege)
                {
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
                }
                return SiteUtilities.TrashBox(
                    context: context,
                    ss: Site.SiteSettings);
            }
            if (!context.CanManageSite(ss: Site.SiteSettings))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        sysLogsStatus: 404,
                        sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.TrashBox(
                        context: context,
                        ss: Site.SiteSettings);
                case "Dashboards":
                    return DashboardUtilities.TrashBox(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.TrashBox(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.TrashBox(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string TrashBoxJson(Context context)
        {
            if (ReferenceId != 0 && ReferenceType != "Sites")
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                tableType: Sqls.TableTypes.Deleted);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            if (ReferenceId == 0)
            {
                if (!context.HasPrivilege)
                {
                    return Messages.ResponseNotFound(context: context).ToJson();
                }
                return SiteUtilities.TrashBoxJson(
                    context: context,
                    ss: Site.SiteSettings);
            }
            if (!context.CanManageSite(ss: Site.SiteSettings))
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.TrashBoxJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Dashboards":
                    return DashboardUtilities.TrashBoxJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.TrashBoxJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.TrashBoxJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Calendar(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Calendar(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.Calendar(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string CalendarJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            if (Site.ReferenceType != "Dashboards")
            {
                ViewModes.Set(context: context, siteId: Site.SiteId);
            }
            switch (Site.ReferenceType)
            {
                case "Dashboards":
                    return DashboardUtilities.CalendarJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.CalendarJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.CalendarJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Crosstab(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Crosstab(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.Crosstab(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string CrosstabJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.CrosstabJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.CrosstabJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string DashboardPartJson(Context context,string dashboardPartId)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            return DashboardUtilities.DashboardPartJson(
                context: context,
                ss: Site.SiteSettings,
                dashboardPartId: dashboardPartId);
        }

        public string DashboardPartLayout (Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            return DashboardUtilities.DashboardPartLayout(
                context: context,
                ss: Site.SiteSettings);
        }

        public string Gantt(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Gantt(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string GanttJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GanttJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BurnDown(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDown(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string BurnDownJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDownJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BurnDownRecordDetailsJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BurnDownRecordDetails(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string TimeSeries(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.TimeSeries(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.TimeSeries(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string TimeSeriesJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.TimeSeriesJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.TimeSeriesJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string SmartDesignJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.SmartDesignJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.SmartDesignJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }
        public string Analy(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Analy(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.Analy(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string AnalyJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.AnalyJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.AnalyJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenAnalyPartDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.OpenAnalyPartDialog(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.OpenAnalyPartDialog(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Kamban(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Kamban(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.Kamban(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string KambanJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.KambanJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.KambanJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ImageLib(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ImageLib(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.ImageLib(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string ImageLibJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            ViewModes.Set(context: context, siteId: Site.SiteId);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ImageLibJson(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.ImageLibJson(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string New(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.EditorNew(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.EditorNew(
                        context: context,
                        ss: Site.SiteSettings);
                case "Wikis":
                    return WikiUtilities.EditorNew(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string NewJson(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            var ss = Site.SiteSettings;
            var referenceType = Site.ReferenceType;
            if (!context.QueryStrings.Bool("control-auto-postback"))
            {
                Process process = null;
                if (referenceType == "Issues")
                {
                    var issueModel = new IssueModel(
                        context: context,
                        ss: ss,
                        issueId: 0);
                    process = Process.GetProcess(
                        context: context,
                        ss: ss,
                        getProcessMatchConditions: (o) => issueModel.GetProcessMatchConditions(
                            context: context,
                            ss: ss,
                            process: o));
                }
                else if (referenceType == "Results")
                {
                    var resultModel = new ResultModel(
                        context: context,
                        ss: ss,
                        resultId: 0);
                    process = Process.GetProcess(
                        context: context,
                        ss: ss,
                        getProcessMatchConditions: (o) => resultModel.GetProcessMatchConditions(
                            context: context,
                            ss: ss,
                            process: o));
                }
                return new ResponseCollection(context: context)
                    .ReplaceAll("#MainContainer", New(context: context))
                    .WindowScrollTop()
                    .FocusMainForm()
                    .ClearFormData(_using: process?.ActionType != Libraries.Settings.Process.ActionTypes.PostBack)
                    .PushState("Edit", Locations.Get(
                        context: context,
                        parts: new string[]
                        {
                            "Items",
                            ReferenceId.ToString(),
                            "New"
                        }))
                    .Events("on_editor_load")
                    .ToJson();
            }
            else
            {
                switch (referenceType)
                {
                    case "Issues":
                        return IssueUtilities.EditorJson(
                            context: context,
                            ss: ss,
                            issueId: 0);
                    case "Results":
                        return ResultUtilities.EditorJson(
                            context: context,
                            ss: ss,
                            resultId: 0);
                    case "Wikis":
                        return WikiUtilities.EditorJson(
                            context: context,
                            ss: ss,
                            wikiId: 0);
                    default:
                        return HtmlTemplates.Error(
                            context: context,
                            errorData: new ErrorData(
                                context: context,
                                type: Error.Types.NotFound,
                                sysLogsStatus: 404,
                                sysLogsDescription: Debugs.GetSysLogsDescription()));
                }
            }
        }

        public string NewOnGrid(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string CancelNewRow(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.CancelNewRow(
                        context: context,
                        ss: Site.SiteSettings,
                        id: context.Forms.Long("CancelRowId"));
                case "Results":
                    return ResultUtilities.CancelNewRow(
                        context: context,
                        ss: Site.SiteSettings,
                        id: context.Forms.Long("CancelRowId"));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Editor(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Editor(
                        context: context,
                        siteId: ReferenceId,
                        clearSessions: true);
                case "Issues":
                    return IssueUtilities.Editor(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId,
                        clearSessions: true);
                case "Results":
                    return ResultUtilities.Editor(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId,
                        clearSessions: true);
                case "Wikis":
                    return WikiUtilities.Editor(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId,
                        clearSessions: true);
                default:
                    return HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
        }

        public string SelectedIds(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    if (SiteId == ReferenceId)
                    {
                        return IssueUtilities.SelectedIds(
                            context: context,
                            ss: Site.SiteSettings);
                    }
                    break;
                case "Results":
                    if (SiteId == ReferenceId)
                    {
                        return ResultUtilities.SelectedIds(
                            context: context,
                            ss: Site.SiteSettings);
                    }
                    break;
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
            return Messages.ResponseNotFound(context: context).ToJson();
        }

        public string LinkTable(Context context)
        {
            SetSite(context: context);
            var dataTableName = context.Forms.Data("TableId");
            return new ResponseCollection(context: context)
                .ReplaceAll("#" + dataTableName, new HtmlBuilder()
                    .LinkTable(
                        context: context,
                        siteId: context.Forms.Long("TableSiteId"),
                        direction: context.Forms.Data("Direction"),
                        dataTableName: dataTableName))
                .ToJson();
        }

        public string Import(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Import(
                        context: context,
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.Import(
                        context: context,
                        siteModel: Site);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenExportSelectorDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.OpenExportSelectorDialog(
                        context: context,
                        ss: Site.SiteSettings,
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.OpenExportSelectorDialog(
                        context: context,
                        ss: Site.SiteSettings,
                        siteModel: Site);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenSetNumericRangeDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (context.HasPermission(ss: Site.SiteSettings))
            {
                var controlId = context.Forms.ControlId();
                var columnName = controlId
                    .Substring(controlId.IndexOf("__") + 2)
                    .Replace("_NumericRange", string.Empty);
                var column = Site.SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                return new ResponseCollection(context: context)
                    .Html(
                        "#SetNumericRangeDialog",
                        new HtmlBuilder().SetNumericRangeDialog(
                            context: context,
                            ss: Site.SiteSettings,
                            column: column,
                            itemfilter: true))
                    .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenSetDateRangeDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (context.HasPermission(ss: Site.SiteSettings))
            {
                var controlId = context.Forms.ControlId();
                var columnName = controlId
                    .Substring(controlId.IndexOf("__") + 2)
                    .Replace("_DateRange", string.Empty);
                var column = Site.SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                if (column == null)
                {
                    Parameters.ExtendedFields.ForEach(extendedField =>
                    {
                        if (extendedField.Name == columnName) column = new Implem.Pleasanter.Libraries.Settings.Column(extendedField.Name);
                    });
                }
                return new ResponseCollection(context: context)
                    .Html(
                        "#SetDateRangeDialog",
                        new HtmlBuilder().SetDateRangeDialog(
                            context: context,
                            ss: Site.SiteSettings,
                            column: column,
                            itemfilter: true))
                    .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ResponseFile Export(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Export(
                        context: context,
                        ss: Site.SiteSettings,
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.Export(
                        context: context,
                        ss: Site.SiteSettings,
                        siteModel: Site);
                default:
                    return null;
            }
        }

        public string ExportAndMailNotify(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            var export = Site.SiteSettings.Exports?
                .Where(exp => exp.Id == context.Forms.Int("ExportId"))?
                .FirstOrDefault();
            if(export?.ExecutionType != Libraries.Settings.Export.ExecutionTypes.MailNotify)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (MailAddressUtilities.Get(
                context: context,
                userId: context.UserId).IsNullOrEmpty())
            {
                return Messages.ResponseExportNotSetEmail(
                    context: context,
                    target: null,
                    $"{context.User.Name}<{context.User.LoginId}>").ToJson();
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ExportAndMailNotify(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.ExportAndMailNotify(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Error.Types.InvalidRequest.MessageJson(context: context);
            }
        }

        public ResponseFile ExportCrosstab(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ExportCrosstab(
                        context: context,
                        ss: Site.SiteSettings,
                        siteModel: Site);
                case "Results":
                    return ResultUtilities.ExportCrosstab(
                        context: context,
                        ss: Site.SiteSettings,
                        siteModel: Site);
                default:
                    return null;
            }
        }

        public string SearchDropDown(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            return DropDowns.SearchDropDown(
                context: context,
                ss: Site.SiteSettings);
        }

        public string RelatingDropDown(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            return DropDowns.RelatingDropDown(
                context: context,
                ss: Site.SiteSettings);
        }

        public string SelectSearchDropDown(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            return DropDowns.SelectSearchDropDown(
                context: context,
                ss: Site.SiteSettings);
        }

        public string GridRows(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Dashboards":
                    return DashboardUtilities.DashboardIndexGridRows(
                        context: context,
                        ss: Site.SiteSettings,
                        offset: context.Forms.Int("GridOffset"));
                case "Issues":
                    return IssueUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings,
                        offset: context.Forms.Int("GridOffset"));
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings,
                        offset: context.Forms.Int("GridOffset"));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ReloadRow(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            var id = context.Forms.Long("Id");
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ReloadRow(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: id);
                case "Results":
                    return ResultUtilities.ReloadRow(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: id);
                default:
                    return ItemUtilities.ClearItemDataResponse(
                        context: context,
                        ss: Site.SiteSettings,
                        id: id)
                            .Remove($"[data-id=\"{id}\"]")
                            .Message(Messages.NotFound(context: context))
                            .ToJson();
            }
        }

        public string CopyRow(Context context)
        {
            SetSite(
                context: context,
                siteOnly: true,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string TrashBoxGridRows(Context context)
        {
            if (ReferenceId != 0 && ReferenceType != "Sites")
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                tableType: Sqls.TableTypes.Deleted);
            if (ReferenceId == 0)
            {
                if (!context.HasPrivilege)
                {
                    return Messages.ResponseNotFound(context: context).ToJson();
                }
                return SiteUtilities.GridRows(
                    context: context,
                    ss: Site.SiteSettings,
                    offset: context.Forms.Int("GridOffset"),
                    action: "TrashBoxGridRows");
            }
            if (!context.CanManageSite(ss: Site.SiteSettings))
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings,
                        offset: context.Forms.Int("GridOffset"),
                        action: "TrashBoxGridRows");
                case "Issues":
                    return IssueUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings,
                        offset: context.Forms.Int("GridOffset"),
                        action: "TrashBoxGridRows");
                case "Results":
                    return ResultUtilities.GridRows(
                        context: context,
                        ss: Site.SiteSettings,
                        offset: context.Forms.Int("GridOffset"),
                        action: "TrashBoxGridRows");
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ImageLibNext(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.ImageLibNext(
                        context: context,
                        ss: Site.SiteSettings,
                        offset: context.Forms.Int("ImageLibOffset"));
                case "Results":
                    return ResultUtilities.ImageLibNext(
                        context: context,
                        ss: Site.SiteSettings,
                        offset: context.Forms.Int("ImageLibOffset"));
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ContentResultInheritance GetByApi(Context context, bool internalRequest = false, string referenceType = null)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (referenceType ?? Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.GetByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        siteId: Site.SiteId,
                        internalRequest: internalRequest);
                case "Issues":
                    return IssueUtilities.GetByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: SiteId != ReferenceId
                            ? ReferenceId
                            : 0,
                        internalRequest: internalRequest);
                case "Results":
                    return ResultUtilities.GetByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: SiteId != ReferenceId
                            ? ReferenceId
                            : 0,
                        internalRequest: internalRequest);
                case "Wikis":
                    return WikiUtilities.GetByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: SiteId != ReferenceId
                            ? ReferenceId
                            : 0,
                        internalRequest: internalRequest);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public BaseItemModel[] GetByServerScript(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return null;
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    if (SiteId == ReferenceId)
                    {
                        return IssueUtilities.GetByServerScript(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    }
                    else
                    {
                        return new[]
                        {
                            IssueUtilities.GetByServerScript(
                                context: context,
                                ss: Site.IssuesSiteSettings(
                                    context: context,
                                    referenceId: ReferenceId),
                                issueId: ReferenceId)
                        }.Where(model => model != null).ToArray();
                    }
                case "Results":
                    if (SiteId == ReferenceId)
                    {
                        return ResultUtilities.GetByServerScript(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    }
                    else
                    {
                        return new[]
                        {
                            ResultUtilities.GetByServerScript(
                                context: context,
                                ss: Site.ResultsSiteSettings(
                                    context: context,
                                    referenceId: ReferenceId),
                                resultId: ReferenceId)
                        }.Where(model => model != null).ToArray();
                    }
                case "Wikis":
                    if (SiteId == ReferenceId)
                    {
                        return WikiUtilities.GetByServerScript(
                            context: context,
                            ss: Site.WikisSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    }
                    else
                    {
                        return new[]
                        {
                            WikiUtilities.GetByServerScript(
                                context: context,
                                ss: Site.WikisSiteSettings(
                                    context: context,
                                    referenceId: ReferenceId),
                                wikiId: ReferenceId)
                        }.Where(model => model != null).ToArray();
                    }
                default:
                    return null;
            }
        }

        public BaseItemModel[] GetSiteByServerScript(Context context)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits(context: context))
            {
                return null;
            }
            return new[]
            {
                SiteUtilities.GetByServerScript(
                    context: context,
                    ss: SiteSettingsUtilities.Get(
                        context: context,
                        siteModel: Site,
                        referenceId: ReferenceId),
                    siteId: SiteId)
            }.Where(model => model != null).ToArray();
        }

        public string Create(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Create(
                        context: context,
                        parentId: Site.SiteId,
                        inheritPermission: Site.InheritPermission);
                case "Issues":
                    return IssueUtilities.Create(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.Create(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ContentResultInheritance CreateByApi(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.CreateByApi(
                        context: context,
                        parentId: Site.SiteId,
                        inheritPermission: Site.InheritPermission);
                case "Issues":
                    return IssueUtilities.CreateByApi(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.CreateByApi(
                        context: context,
                        ss: Site.SiteSettings);
                case "Wikis":
                    return WikiUtilities.CreateByApi(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public bool CreateByServerScript(Context context, object model)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits(context: context))
            {
                return false;
            }
            switch (Site.ReferenceType)
            {
                case "Sites":
                    var siteSs = Site.SitesSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string siteRequestString)
                    {
                        context.ApiRequestBody = siteRequestString;
                    }
                    else if (model is ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        context.ApiRequestBody = serverScriptModelApiModel.ToJsonString(
                            context: context,
                            ss: siteSs);
                    }
                    else
                    {
                        return false;
                    }
                    return SiteUtilities.CreateByServerScript(
                        context: context,
                        ss: siteSs,
                        model: model);
                case "Issues":
                    var issueSs = Site.IssuesSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string issueRequestString)
                    {
                        context.ApiRequestBody = issueRequestString;
                    }
                    else if (model is ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        context.ApiRequestBody = serverScriptModelApiModel.ToJsonString(
                            context: context,
                            ss: issueSs);
                    }
                    else
                    {
                        return false;
                    }
                    return IssueUtilities.CreateByServerScript(
                        context: context,
                        ss: issueSs,
                        model: model);
                case "Results":
                    var resultSs = Site.ResultsSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string resultRequestString)
                    {
                        context.ApiRequestBody = resultRequestString;
                    }
                    else if (model is ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        context.ApiRequestBody = serverScriptModelApiModel.ToJsonString(
                            context: context,
                            ss: resultSs);
                    }
                    else
                    {
                        return false;
                    }
                    return ResultUtilities.CreateByServerScript(
                        context: context,
                        ss: resultSs,
                        model: model);
                case "Wikis":
                    var wikiSs = Site.WikisSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string wikiRequestString)
                    {
                        context.ApiRequestBody = wikiRequestString;
                    }
                    else if (model is ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        context.ApiRequestBody = serverScriptModelApiModel.ToJsonString(
                            context: context,
                            ss: wikiSs);
                    }
                    else
                    {
                        return false;
                    }
                    return WikiUtilities.CreateByServerScript(
                        context: context,
                        ss: wikiSs,
                        model: model);
                default:
                    return false;
            }
        }

        public string Templates(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Templates(
                        context: context,
                        parentId: Site.SiteId,
                        inheritPermission: Site.InheritPermission);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string CreateByTemplate(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.CreateByTemplate(
                        context: context,
                        parentId: Site.SiteId,
                        inheritPermission: Site.InheritPermission);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string SiteMenu(Context context)
        {
            SetSite(context: context);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.SiteMenuJson(
                        context: context,
                        siteModel: Site);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Update(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Update(
                        context: context,
                        siteModel: Site,
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.Update(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId,
                        previousTitle: Title);
                case "Results":
                    return ResultUtilities.Update(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId,
                        previousTitle: Title);
                case "Wikis":
                    return WikiUtilities.Update(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId,
                        previousTitle: Title);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenBulkUpdateSelectorDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.OpenBulkUpdateSelectorDialog(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.OpenBulkUpdateSelectorDialog(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BulkUpdateSelectChanged(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkUpdateSelectChanged(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.BulkUpdateSelectChanged(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BulkUpdate(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.SiteSettings.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkUpdate(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.BulkUpdate(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ContentResultInheritance BulkUpsertByApi(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (Site.SiteSettings.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkUpsertByApi(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.BulkUpsertByApi(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public string UpdateByGrid(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.SiteSettings.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.UpdateByGrid(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.UpdateByGrid(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BulkProcess(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.SiteSettings.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkProcess(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.BulkProcess(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ContentResultInheritance UpdateByApi(Context context, string referenceType = null)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (referenceType ?? Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.UpdateByApi(
                        context: context,
                        siteModel: Site,
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.UpdateByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId,
                        previousTitle: Title);
                case "Results":
                    return ResultUtilities.UpdateByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId,
                        previousTitle: Title);
                case "Wikis":
                    return WikiUtilities.UpdateByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId,
                        previousTitle: Title);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public bool UpdateByServerScript(Context context, object model)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits(context: context))
            {
                return false;
            }
            switch (ReferenceType)
            {
                case "Sites":
                    var siteSs = Site.SitesSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string siteRequestString)
                    {
                        context.ApiRequestBody = siteRequestString;
                    }
                    else if (model is ServerScriptModelApiModel issueApiModel)
                    {
                        context.ApiRequestBody = issueApiModel.ToJsonString(
                            context: context,
                            ss: siteSs);
                    }
                    else
                    {
                        return false;
                    }
                    return SiteUtilities.UpdateByServerScript(
                        context: context,
                        siteModel: Site,
                        siteId: Site.SiteId,
                        model: model);
                case "Issues":
                    var issueSs = Site.IssuesSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string issueRequestString)
                    {
                        context.ApiRequestBody = issueRequestString;
                    }
                    else if (model is ServerScriptModelApiModel issueApiModel)
                    {
                        context.ApiRequestBody = issueApiModel.ToJsonString(
                            context: context,
                            ss: issueSs);
                    }
                    else
                    {
                        return false;
                    }
                    return IssueUtilities.UpdateByServerScript(
                        context: context,
                        ss: issueSs,
                        issueId: ReferenceId,
                        previousTitle: Title,
                        model: model);
                case "Results":
                    var resultSs = Site.ResultsSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string resultRequestString)
                    {
                        context.ApiRequestBody = resultRequestString;
                    }
                    else if (model is ServerScriptModelApiModel resultApiModel)
                    {
                        context.ApiRequestBody = resultApiModel.ToJsonString(
                            context: context,
                            ss: resultSs);
                    }
                    else
                    {
                        return false;
                    }
                    return ResultUtilities.UpdateByServerScript(
                        context: context,
                        ss: resultSs,
                        resultId: ReferenceId,
                        previousTitle: Title,
                        model: model);
                case "Wikis":
                    var wikiSs = Site.WikisSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string wikiRequestString)
                    {
                        context.ApiRequestBody = wikiRequestString;
                    }
                    else if (model is ServerScriptModelApiModel wikiApiModel)
                    {
                        context.ApiRequestBody = wikiApiModel.ToJsonString(
                            context: context,
                            ss: wikiSs);
                    }
                    else
                    {
                        return false;
                    }
                    return WikiUtilities.UpdateByServerScript(
                        context: context,
                        ss: wikiSs,
                        wikiId: ReferenceId,
                        previousTitle: Title,
                        model: model);
                default:
                    return false;
            }
        }

        public ContentResultInheritance UpsertByApi(Context context, string referenceType = null)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (referenceType ?? Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.UpsertByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        previousTitle: Title);
                case "Results":
                    return ResultUtilities.UpsertByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        previousTitle: Title);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public ContentResultInheritance UpdateSiteSettingsByApi(Context context, string referenceType = null)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            if (Site.SiteId == 0)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            switch (referenceType ?? Site.ReferenceType)
            {
                case "Sites":
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                    return SiteUtilities.UpdateSiteSettingsByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        siteModel: Site);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public ContentResultInheritance UpsertSiteSettingsByApi(Context context, string referenceType = null)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            if (Site.SiteId == 0)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            switch (referenceType ?? Site.ReferenceType)
            {
                case "Sites":
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                    return SiteUtilities.UpsertSiteSettingsByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        siteModel: Site);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public bool UpsertByServerScript(Context context, object model)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits(context: context))
            {
                return false;
            }
            switch (Site.ReferenceType)
            {
                case "Issues":
                    var issueSs = Site.IssuesSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string issueRequestString)
                    {
                        context.ApiRequestBody = issueRequestString;
                    }
                    else if (model is ServerScriptModelApiModel issueApiModel)
                    {
                        context.ApiRequestBody = issueApiModel.ToJsonString(
                            context: context,
                            ss: issueSs);
                    }
                    else
                    {
                        return false;
                    }
                    return IssueUtilities.UpsertByServerScript(
                        context: context,
                        ss: issueSs,
                        previousTitle: Title,
                        model: model);
                case "Results":
                    var resultSs = Site.ResultsSiteSettings(
                        context: context,
                        referenceId: ReferenceId);
                    if (model is string resultRequestString)
                    {
                        context.ApiRequestBody = resultRequestString;
                    }
                    else if (model is ServerScriptModelApiModel resultApiModel)
                    {
                        context.ApiRequestBody = resultApiModel.ToJsonString(
                            context: context,
                            ss: resultSs);
                    }
                    else
                    {
                        return false;
                    }
                    return ResultUtilities.UpsertByServerScript(
                        context: context,
                        ss: resultSs,
                        previousTitle: Title,
                        model: model);
                default:
                    return false;
            }
        }

        public string DeleteComment(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Update(
                        context: context,
                        siteModel: Site,
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.Update(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId,
                        previousTitle: Title);
                case "Results":
                    return ResultUtilities.Update(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId,
                        previousTitle: Title);
                case "Wikis":
                    return WikiUtilities.Update(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId,
                        previousTitle: Title);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Copy(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Copy(
                        context: context,
                        siteModel: Site);
                case "Issues":
                    return IssueUtilities.Copy(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Copy(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string MoveTargets(Context context)
        {
            SetSite(context: context);
            return new ResponseCollection(context: context).Html("#MoveTargets", new HtmlBuilder()
                .OptionCollection(
                    context: context,
                    optionCollection: Site.SiteSettings.MoveTargetsSelectableOptions(
                        context: context,
                        enabled: true)))
                            .ToJson();
        }

        public string Move(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Move(
                        context: context,
                        ss: Site.SiteSettings,
                    issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Move(
                        context: context,
                        ss: Site.SiteSettings,
                    resultId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string BulkMove(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkMove(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.BulkMove(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Delete(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Delete(
                        context: context,
                        ss: Site.SiteSettings,
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.Delete(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Delete(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.Delete(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ContentResultInheritance DeleteByApi(Context context, string referenceType = null)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (referenceType ?? Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.DeleteByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.DeleteByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.DeleteByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.DeleteByApi(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
        }

        public bool DeleteByServerScript(Context context)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits(context: context))
            {
                return false;
            }
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.DeleteByServerScript(
                        context: context,
                        ss: Site.SitesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.DeleteByServerScript(
                        context: context,
                        ss: Site.IssuesSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.DeleteByServerScript(
                        context: context,
                        ss: Site.ResultsSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.DeleteByServerScript(
                        context: context,
                        ss: Site.WikisSiteSettings(
                            context: context,
                            referenceId: ReferenceId),
                        wikiId: ReferenceId);
                default:
                    return false;
            }
        }

        public string BulkDelete(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkDelete(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.BulkDelete(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ContentResultInheritance BulkDeleteByApi(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            if (context.RequestDataString.Deserialize<ApiDeleteOption>()?.PhysicalDelete == true)
            {
                switch (Site.ReferenceType)
                {
                    case "Issues":
                        return IssueUtilities.PhysicalBulkDeleteByApi(
                            context: context,
                            ss: Site.SiteSettings);
                    case "Results":
                        return ResultUtilities.PhysicalBulkDeleteByApi(
                            context: context,
                            ss: Site.SiteSettings);
                    default:
                        return ApiResults.Get(ApiResponses.NotFound(context: context));
                }
            }
            else
            {
                switch (Site.ReferenceType)
                {
                    case "Issues":
                        return IssueUtilities.BulkDeleteByApi(
                            context: context,
                            ss: Site.SiteSettings);
                    case "Results":
                        return ResultUtilities.BulkDeleteByApi(
                            context: context,
                            ss: Site.SiteSettings);
                    default:
                        return ApiResults.Get(ApiResponses.NotFound(context: context));
                }
            }
        }

        public long BulkDeleteByServerScript(Context context)
        {
            SetSite(context: context);
            if (!Site.WithinApiLimits(context: context))
            {
                return 0;
            }
            if (context.RequestDataString.Deserialize<ApiDeleteOption>()?.PhysicalDelete == true)
            {
                switch (Site.ReferenceType)
                {
                    case "Issues":
                        return IssueUtilities.PhysicalBulkDeleteByServerScript(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId,
                                setSiteIntegration: true,
                                tableType: Sqls.TableTypes.Deleted));
                    case "Results":
                        return ResultUtilities.PhysicalBulkDeleteByServerScript(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId,
                                setSiteIntegration: true,
                                tableType: Sqls.TableTypes.Deleted));
                    default:
                        return 0;
                }
            }
            else
            {
                switch (Site.ReferenceType)
                {
                    case "Issues":
                        return IssueUtilities.BulkDeleteByServerScript(
                            context: context,
                            ss: Site.IssuesSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    case "Results":
                        return ResultUtilities.BulkDeleteByServerScript(
                            context: context,
                            ss: Site.ResultsSiteSettings(
                                context: context,
                                referenceId: ReferenceId));
                    default:
                        return 0;
                }
            }
        }

        public string DeleteHistory(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                tableType: Sqls.TableTypes.NormalAndHistory);
            if (SiteId == ReferenceId)
            {
                return SiteUtilities.DeleteHistory(
                    context: context,
                    ss: Site.SiteSettings,
                    siteId: ReferenceId);
            }
            else
            {
                switch (Site.ReferenceType)
                {
                    case "Issues":
                        return IssueUtilities.DeleteHistory(
                            context: context,
                            ss: Site.SiteSettings,
                            issueId: ReferenceId);
                    case "Results":
                        return ResultUtilities.DeleteHistory(
                            context: context,
                            ss: Site.SiteSettings,
                            resultId: ReferenceId);
                    case "Wikis":
                        return WikiUtilities.DeleteHistory(
                            context: context,
                            ss: Site.SiteSettings,
                            wikiId: ReferenceId);
                    default:
                        return Messages.ResponseNotFound(context: context).ToJson();
                }
            }
        }

        public string PhysicalBulkDelete(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                tableType: Sqls.TableTypes.Deleted);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.PhysicalBulkDelete(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.PhysicalBulkDelete(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.PhysicalBulkDelete(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Restore(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                tableType: Sqls.TableTypes.Deleted);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Restore(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.Restore(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.Restore(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string RestoreFromHistory(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                tableType: Sqls.TableTypes.History);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.RestoreFromHistory(
                        context: context,
                        ss: Site.SiteSettings,
                        siteId: ReferenceId);
                case "Issues":
                    return IssueUtilities.RestoreFromHistory(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.RestoreFromHistory(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.RestoreFromHistory(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string EditSeparateSettings(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.EditSeparateSettings(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Separate(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Separate(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string Histories(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                tableType: Sqls.TableTypes.NormalAndHistory);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.Histories(
                        context: context,
                        siteModel: Site);
                case "Issues":
                    return IssueUtilities.Histories(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.Histories(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.Histories(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string History(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                tableType: Sqls.TableTypes.History);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.History(
                        context: context,
                        siteModel: Site);
                case "Issues":
                    return IssueUtilities.History(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.History(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.History(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string EditorJson(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.EditorJson(
                        context: context,
                        siteModel: Site);
                case "Issues":
                    return IssueUtilities.EditorJson(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: ReferenceId);
                case "Results":
                    return ResultUtilities.EditorJson(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: ReferenceId);
                case "Wikis":
                    return WikiUtilities.EditorJson(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: ReferenceId);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UpdateByCalendar(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            switch (Site.ReferenceType)
            {
                case "Dashboards":
                    return DashboardUtilities.UpdateByCalendar(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.UpdateByCalendar(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.UpdateByCalendar(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UpdateByKamban(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            switch (Site.ReferenceType)
            {
                case "Dashboards":
                    return DashboardUtilities.UpdateByKamban(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                    return IssueUtilities.UpdateByKamban(
                        context: context,
                        ss: Site.SiteSettings);
                case "Results":
                    return ResultUtilities.UpdateByKamban(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string OpenImportSitePackageDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return Libraries.SitePackages.Utilities.OpenImportSitePackageDialog(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                    return Libraries.SitePackages.Utilities.OpenImportSitePackageDialog(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ImportSitePackage(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return Libraries.SitePackages.Utilities.ImportSitePackage(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                default:
                    throw new NotImplementedException();
            }
        }

        public string OpenExportSitePackageDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return Libraries.SitePackages.Utilities.OpenExportSitePackageDialog(
                        context: context,
                        ss: Site.SiteSettings,
                        recursive: true);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                    return Libraries.SitePackages.Utilities.OpenExportSitePackageDialog(
                        context: context,
                        ss: Site.SiteSettings,
                        recursive: false);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public ResponseFile ExportSitePackage(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return Libraries.SitePackages.Utilities.ExportSitePackage(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                    return Libraries.SitePackages.Utilities.ExportSitePackage(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return null;
            }
        }

        public ContentResultInheritance CopySitePackageByApi(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            if (!Site.WithinApiLimits(context: context))
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: Site.SiteId,
                    limitPerSite: context.ContractSettings.ApiLimit()));
            }
            switch (Site.ReferenceType)
            {
                case "Sites":
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                    var response = Libraries.SitePackages.Utilities.ImportSitePackage(
                        context: context,
                        ss: Site.SiteSettings,
                        apiData: context.RequestDataString.Deserialize<Sites.SitePackageApiModel>());
                    return ApiResults.Get(
                        statusCode: 200,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - Site.SiteSettings.ApiCount,
                        response: new
                        {
                            Data = response
                        });
                default:
                    return null;
            }
        }

        public string ImportUserTemplate(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.ImportUserTemplate(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                default:
                    throw new NotImplementedException();
            }
        }

        public string DeleteUserTemplate(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.DeleteUserTemplate(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                default:
                    throw new NotImplementedException();
            }
        }

        public string UpdateUserTemplate(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.UpdateUserTemplate(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                default:
                    throw new NotImplementedException();
            }
        }

        public string SearchUserTemplate(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.SearchUserTemplate(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                default:
                    throw new NotImplementedException();
            }
        }

        public string OpenEditUserTemplateDialog(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            switch (Site.ReferenceType)
            {
                case "Sites":
                    return SiteUtilities.OpenEditUserTemplateDialog(
                        context: context,
                        ss: Site.SiteSettings);
                case "Issues":
                case "Results":
                case "Wikis":
                case "Dashboards":
                default:
                    throw new NotImplementedException();
            }
        }

        public (Plugins.PdfData pdfData, string error) Pdf(Context context, int reportId)
        {
            SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true,
                setAllChoices: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.Pdf(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: SiteId != ReferenceId
                            ? ReferenceId
                            : 0,
                        reportId: reportId);
                case "Results":
                    return ResultUtilities.Pdf(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: SiteId != ReferenceId
                            ? ReferenceId
                            : 0,
                        reportId: reportId);
                default:
                    return (null, HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(
                            context: context,
                            type: Error.Types.NotFound,
                            sysLogsStatus: 404,
                            sysLogsDescription: Debugs.GetSysLogsDescription())));
            }
        }

        public string LockTable(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                case "Results":
                    return SiteUtilities.LockTable(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UnlockTable(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                case "Results":
                    return SiteUtilities.UnlockTable(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string ForceUnlockTable(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                case "Results":
                    return SiteUtilities.ForceUnlockTable(
                        context: context,
                        ss: Site.SiteSettings);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string UnlockRecord(Context context, long id)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            switch (Site.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.UnlockRecord(
                        context: context,
                        ss: Site.SiteSettings,
                        issueId: id);
                case "Results":
                    return ResultUtilities.UnlockRecord(
                        context: context,
                        ss: Site.SiteSettings,
                        resultId: id);
                case "Wikis":
                    return WikiUtilities.UnlockRecord(
                        context: context,
                        ss: Site.SiteSettings,
                        wikiId: id);
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        public string SynchronizeTitles(Context context)
        {
            SetSite(
                context: context,
                initSiteSettings: true);
            return SiteUtilities.SynchronizeTitles(
                context: context,
                siteModel: Site);
        }

        public string SynchronizeSummaries(Context context)
        {
            SetSite(context: context);
            var selected = context.Forms.IntList("EditSummary");
            using var exclusiveObj = new Sessions.TableExclusive(context: context);
            if (!exclusiveObj.TryLock())
            {
                return Messages.ImportLock(context: context).ToJson();
            }
            var result = SiteUtilities.SynchronizeSummaries(
                context: context,
                siteModel: Site,
                selected: selected,
                watchdog: () => exclusiveObj.Refresh());
            if (result.Type == Error.Types.None)
            {
                return Messages.ResponseSynchronizationCompleted(context: context).ToJson();
            }
            else if (result.Type == Error.Types.SelectTargets)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            else
            {
                return result.MessageJson(context: context);
            }
        }

        public ContentResultInheritance SynchronizeSummariesByApi(Context context)
        {
            SetSite(context: context);
            var selected = context.RequestDataString.Deserialize<SiteApiModel>()?.SummaryId;
            if (selected == null)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(
                        context: context,
                        type: Error.Types.BadRequest,
                        sysLogsStatus: 400,
                        sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
            using var exclusiveObj = new Sessions.TableExclusive(context: context);
            if (!exclusiveObj.TryLock())
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 429,
                    message: Messages.ImportLock(context: context).Text));
            }
            var result = SiteUtilities.SynchronizeSummaries(
                context: context,
                siteModel: Site,
                selected: selected,
                watchdog: () => exclusiveObj.Refresh());
            if (result.Type == Error.Types.SelectTargets)
            {
                return ApiResults.Success(
                    id: Site.SiteId,
                    message: Displays.SelectTargets(context: context));
            }
            else if (result.Type != Error.Types.None)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: result);
            }
            else
            {
                return ApiResults.Success(
                    id: Site.SiteId,
                    message: Displays.SynchronizationCompleted(context: context));
            }
        }

        public string SynchronizeFormulas(Context context)
        {
            SetSite(context: context);
            return SiteUtilities.SynchronizeFormulas(
                context: context,
                siteModel: Site);
        }

        public void SetSite(
            Context context,
            bool siteOnly = false,
            bool initSiteSettings = false,
            bool setSiteIntegration = false,
            bool setAllChoices = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Site = GetSite(
                context: context,
                siteOnly: siteOnly,
                initSiteSettings: initSiteSettings,
                setSiteIntegration: setSiteIntegration,
                setAllChoices: setAllChoices,
                tableType: tableType);
            SetByWhenloadingSiteSettingsServerScript(
                context: context,
                ss: Site.SiteSettings);
        }

        public SiteModel GetSite(
            Context context,
            bool siteOnly = false,
            bool initSiteSettings = false,
            bool setSiteIntegration = false,
            bool setAllChoices = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            SiteModel siteModel;
            if (ReferenceType == "Sites" && context.Forms.Exists("Ver"))
            {
                siteModel = new SiteModel();
                siteModel.Get(
                    context: context,
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(ReferenceId)
                        .Ver(context.Forms.Int("Ver")),
                    tableType: Sqls.TableTypes.NormalAndHistory);
                siteModel.VerType =  context.Forms.Bool("Latest")
                    ? Versions.VerTypes.Latest
                    : Versions.VerTypes.History;
            }
            else
            {
                siteModel = siteOnly
                    ? new SiteModel(
                        context: context,
                        siteId: ReferenceId)
                    : new SiteModel(
                        context: context,
                        siteId: ReferenceType == "Sites"
                            ? ReferenceId
                            : SiteId);
            }
            if (initSiteSettings)
            {
                siteModel.SiteSettings = SiteSettingsUtilities.Get(
                    context: context,
                    siteModel: siteModel,
                    referenceId: ReferenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices,
                    tableType: tableType);
            }
            return siteModel;
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "ReferenceId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceId = dataRow[column.ColumnName].ToLong();
                                SavedReferenceId = ReferenceId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "ReferenceType":
                            ReferenceType = dataRow[column.ColumnName].ToString();
                            SavedReferenceType = ReferenceType;
                            break;
                        case "SiteId":
                            SiteId = dataRow[column.ColumnName].ToLong();
                            SavedSiteId = SiteId;
                            break;
                        case "Title":
                            Title = dataRow[column.ColumnName].ToString();
                            SavedTitle = Title;
                            break;
                        case "FullText":
                            FullText = dataRow[column.ColumnName].ToString();
                            SavedFullText = FullText;
                            break;
                        case "SearchIndexCreatedTime":
                            SearchIndexCreatedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedSearchIndexCreatedTime = SearchIndexCreatedTime;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedClass(
                                        columnName: column.Name,
                                        value: GetClass(columnName: column.Name));
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.Name,
                                        value: new Num(
                                            dataRow: dataRow,
                                            name: column.ColumnName));
                                    SetSavedNum(
                                        columnName: column.Name,
                                        value: GetNum(columnName: column.Name).Value);
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SetSavedDate(
                                        columnName: column.Name,
                                        value: GetDate(columnName: column.Name));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedDescription(
                                        columnName: column.Name,
                                        value: GetDescription(columnName: column.Name));
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SetSavedCheck(
                                        columnName: column.Name,
                                        value: GetCheck(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SetSavedAttachments(
                                        columnName: column.Name,
                                        value: GetAttachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || ReferenceId_Updated(context: context)
                || Ver_Updated(context: context)
                || ReferenceType_Updated(context: context)
                || SiteId_Updated(context: context)
                || Title_Updated(context: context)
                || FullText_Updated(context: context)
                || SearchIndexCreatedTime_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)));
        }

        public bool Updated(Context context, SiteSettings ss)
        {
            return UpdatedWithColumn(context: context, ss: ss)
                || ReferenceId_Updated(context: context)
                || Ver_Updated(context: context)
                || ReferenceType_Updated(context: context)
                || SiteId_Updated(context: context)
                || Title_Updated(context: context)
                || FullText_Updated(context: context)
                || SearchIndexCreatedTime_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ItemModel()
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ItemModel(Context context, long referenceId)
        {
            OnConstructing(context: context);
            ReferenceId = referenceId;
            Get(
                context: context,
                join: Rds.ItemsJoin().Add(
                    new SqlJoin(
                        "\"Sites\"",
                        SqlJoin.JoinTypes.Inner,
                        $"\"Sites\".\"SiteId\" = \"Items\".\"SiteId\" and \"Sites\".\"TenantId\" = {Parameters.Parameter.SqlParameterPrefix}T")));
            OnConstructed(context: context);
        }
    }
}
