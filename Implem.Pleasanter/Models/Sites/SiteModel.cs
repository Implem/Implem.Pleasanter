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
    public class SiteModel : BaseItemModel
    {
        public SiteSettings SiteSettings;
        public int TenantId = 0;
        public string SiteName = string.Empty;
        public string SiteGroupName = string.Empty;
        public string GridGuide = string.Empty;
        public string EditorGuide = string.Empty;
        public string CalendarGuide = string.Empty;
        public string CrosstabGuide = string.Empty;
        public string GanttGuide = string.Empty;
        public string BurnDownGuide = string.Empty;
        public string TimeSeriesGuide = string.Empty;
        public string AnalyGuide = string.Empty;
        public string KambanGuide = string.Empty;
        public string ImageLibGuide = string.Empty;
        public string ReferenceType = "Sites";
        public long ParentId = 0;
        public long InheritPermission = 0;
        public bool Publish = false;
        public bool DisableCrossSearch = false;
        public Time LockedTime = new Time();
        public SiteCollection Ancestors = null;
        public User LockedUser = new User();
        public int SiteMenu = 0;
        public List<string> MonitorChangesColumns = null;
        public List<string> TitleColumns = null;
        public Export Export = null;
        public DateTime ApiCountDate = 0.ToDateTime();
        public int ApiCount = 0;
        public bool DisableSiteCreatorPermission = false;

        public TitleBody TitleBody
        {
            get
            {
                return new TitleBody(SiteId, Ver, VerType == Versions.VerTypes.History, Title.Value, Title.DisplayValue, Body);
            }
        }

        public int SavedTenantId = 0;
        public string SavedSiteName = string.Empty;
        public string SavedSiteGroupName = string.Empty;
        public string SavedGridGuide = string.Empty;
        public string SavedEditorGuide = string.Empty;
        public string SavedCalendarGuide = string.Empty;
        public string SavedCrosstabGuide = string.Empty;
        public string SavedGanttGuide = string.Empty;
        public string SavedBurnDownGuide = string.Empty;
        public string SavedTimeSeriesGuide = string.Empty;
        public string SavedAnalyGuide = string.Empty;
        public string SavedKambanGuide = string.Empty;
        public string SavedImageLibGuide = string.Empty;
        public string SavedReferenceType = "Sites";
        public long SavedParentId = 0;
        public long SavedInheritPermission = 0;
        public string SavedSiteSettings = string.Empty;
        public bool SavedPublish = false;
        public bool SavedDisableCrossSearch = false;
        public DateTime SavedLockedTime = 0.ToDateTime();
        public SiteCollection SavedAncestors = null;
        public int SavedLockedUser = 0;
        public int SavedSiteMenu = 0;
        public List<string> SavedMonitorChangesColumns = null;
        public List<string> SavedTitleColumns = null;
        public Export SavedExport = null;
        public DateTime SavedApiCountDate = 0.ToDateTime();
        public int SavedApiCount = 0;
        public bool SavedDisableSiteCreatorPermission = false;

        public bool TenantId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != TenantId;
            }
            return TenantId != SavedTenantId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool SiteName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != SiteName;
            }
            return SiteName != SavedSiteName && SiteName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != SiteName);
        }

        public bool SiteGroupName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != SiteGroupName;
            }
            return SiteGroupName != SavedSiteGroupName && SiteGroupName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != SiteGroupName);
        }

        public bool GridGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != GridGuide;
            }
            return GridGuide != SavedGridGuide && GridGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != GridGuide);
        }

        public bool EditorGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != EditorGuide;
            }
            return EditorGuide != SavedEditorGuide && EditorGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != EditorGuide);
        }

        public bool CalendarGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != CalendarGuide;
            }
            return CalendarGuide != SavedCalendarGuide && CalendarGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != CalendarGuide);
        }

        public bool CrosstabGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != CrosstabGuide;
            }
            return CrosstabGuide != SavedCrosstabGuide && CrosstabGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != CrosstabGuide);
        }

        public bool GanttGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != GanttGuide;
            }
            return GanttGuide != SavedGanttGuide && GanttGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != GanttGuide);
        }

        public bool BurnDownGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != BurnDownGuide;
            }
            return BurnDownGuide != SavedBurnDownGuide && BurnDownGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != BurnDownGuide);
        }

        public bool TimeSeriesGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TimeSeriesGuide;
            }
            return TimeSeriesGuide != SavedTimeSeriesGuide && TimeSeriesGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TimeSeriesGuide);
        }

        public bool AnalyGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != AnalyGuide;
            }
            return AnalyGuide != SavedAnalyGuide && AnalyGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != AnalyGuide);
        }

        public bool KambanGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != KambanGuide;
            }
            return KambanGuide != SavedKambanGuide && KambanGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != KambanGuide);
        }

        public bool ImageLibGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ImageLibGuide;
            }
            return ImageLibGuide != SavedImageLibGuide && ImageLibGuide != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ImageLibGuide);
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

        public bool ParentId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != ParentId;
            }
            return ParentId != SavedParentId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != ParentId);
        }

        public bool InheritPermission_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != InheritPermission;
            }
            return InheritPermission != SavedInheritPermission
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != InheritPermission);
        }

        public bool SiteSettings_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != SiteSettings.RecordingJson(context: context);
            }
            return SiteSettings.RecordingJson(context: context) != SavedSiteSettings && SiteSettings.RecordingJson(context: context) != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != SiteSettings.RecordingJson(context: context));
        }

        public bool Publish_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != Publish;
            }
            return Publish != SavedPublish
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != Publish);
        }

        public bool DisableCrossSearch_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != DisableCrossSearch;
            }
            return DisableCrossSearch != SavedDisableCrossSearch
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != DisableCrossSearch);
        }

        public bool LockedUser_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != LockedUser.Id;
            }
            return LockedUser.Id != SavedLockedUser
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != LockedUser.Id);
        }

        public bool ApiCount_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != ApiCount;
            }
            return ApiCount != SavedApiCount
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != ApiCount);
        }

        public bool LockedTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != LockedTime.Value;
            }
            return LockedTime.Value != SavedLockedTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != LockedTime.Value.Date);
        }

        public bool ApiCountDate_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != ApiCountDate;
            }
            return ApiCountDate != SavedApiCountDate
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != ApiCountDate.Date);
        }

        public SiteSettings Session_SiteSettings(Context context)
        {
            return context.SessionData.Get("SiteSettings") != null
                ? context.SessionData.Get("SiteSettings")?.ToString().DeserializeSiteSettings(context: context) ?? new SiteSettings(context: context, referenceType: ReferenceType)
                : SiteSettings;
        }

        public void Session_SiteSettings(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "SiteSettings",
                value: value,
                page: true);
        }

        public List<string> Session_MonitorChangesColumns(Context context)
        {
            return context.SessionData.Get("MonitorChangesColumns") != null
                ? context.SessionData.Get("MonitorChangesColumns").Deserialize<List<string>>() ?? new List<string>()
                : MonitorChangesColumns;
        }

        public void Session_MonitorChangesColumns(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "MonitorChangesColumns",
                value: value,
                page: true);
        }

        public List<string> Session_TitleColumns(Context context)
        {
            return context.SessionData.Get("TitleColumns") != null
                ? context.SessionData.Get("TitleColumns").Deserialize<List<string>>() ?? new List<string>()
                : TitleColumns;
        }

        public void Session_TitleColumns(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "TitleColumns",
                value: value,
                page: true);
        }

        public Export Session_Export(Context context)
        {
            return context.SessionData.Get("Export") != null
                ? context.SessionData.Get("Export").Deserialize<Export>() ?? new Export()
                : Export;
        }

        public void Session_Export(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "Export",
                value: value,
                page: true);
        }

        public bool Session_DisableSiteCreatorPermission(Context context)
        {
            return context.SessionData.Get("DisableSiteCreatorPermission") != null
                ? context.SessionData.Get("DisableSiteCreatorPermission").ToBool()
                : DisableSiteCreatorPermission;
        }

        public void Session_DisableSiteCreatorPermission(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "DisableSiteCreatorPermission",
                value: value,
                page: true);
        }

        public string PropertyValue(Context context, Column column)
        {
            switch (column?.ColumnName)
            {
                case "TenantId": return TenantId.ToString();
                case "SiteId": return SiteId.ToString();
                case "UpdatedTime": return UpdatedTime.Value.ToString();
                case "Ver": return Ver.ToString();
                case "Title": return Title.Value;
                case "Body": return Body;
                case "TitleBody": return TitleBody.ToString();
                case "SiteName": return SiteName;
                case "SiteGroupName": return SiteGroupName;
                case "GridGuide": return GridGuide;
                case "EditorGuide": return EditorGuide;
                case "CalendarGuide": return CalendarGuide;
                case "CrosstabGuide": return CrosstabGuide;
                case "GanttGuide": return GanttGuide;
                case "BurnDownGuide": return BurnDownGuide;
                case "TimeSeriesGuide": return TimeSeriesGuide;
                case "AnalyGuide": return AnalyGuide;
                case "KambanGuide": return KambanGuide;
                case "ImageLibGuide": return ImageLibGuide;
                case "ReferenceType": return ReferenceType;
                case "ParentId": return ParentId.ToString();
                case "InheritPermission": return InheritPermission.ToString();
                case "SiteSettings": return SiteSettings.RecordingJson(context: context);
                case "Publish": return Publish.ToString();
                case "DisableCrossSearch": return DisableCrossSearch.ToString();
                case "LockedTime": return LockedTime.Value.ToString();
                case "Ancestors": return Ancestors.ToString();
                case "LockedUser": return LockedUser.Id.ToString();
                case "SiteMenu": return SiteMenu.ToString();
                case "MonitorChangesColumns": return MonitorChangesColumns.ToString();
                case "TitleColumns": return TitleColumns.ToString();
                case "Export": return Export.ToString();
                case "ApiCountDate": return ApiCountDate.ToString();
                case "ApiCount": return ApiCount.ToString();
                case "DisableSiteCreatorPermission": return DisableSiteCreatorPermission.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return GetValue(
                    context: context,
                    column: column);
            }
        }

        public string SavedPropertyValue(Context context, Column column)
        {
            switch (column?.ColumnName)
            {
                case "TenantId": return SavedTenantId.ToString();
                case "SiteId": return SavedSiteId.ToString();
                case "UpdatedTime": return SavedUpdatedTime.ToString();
                case "Ver": return SavedVer.ToString();
                case "Title": return SavedTitle;
                case "Body": return SavedBody;
                case "SiteName": return SavedSiteName;
                case "SiteGroupName": return SavedSiteGroupName;
                case "GridGuide": return SavedGridGuide;
                case "EditorGuide": return SavedEditorGuide;
                case "CalendarGuide": return SavedCalendarGuide;
                case "CrosstabGuide": return SavedCrosstabGuide;
                case "GanttGuide": return SavedGanttGuide;
                case "BurnDownGuide": return SavedBurnDownGuide;
                case "TimeSeriesGuide": return SavedTimeSeriesGuide;
                case "AnalyGuide": return SavedAnalyGuide;
                case "KambanGuide": return SavedKambanGuide;
                case "ImageLibGuide": return SavedImageLibGuide;
                case "ReferenceType": return SavedReferenceType;
                case "ParentId": return SavedParentId.ToString();
                case "InheritPermission": return SavedInheritPermission.ToString();
                case "SiteSettings": return SavedSiteSettings;
                case "Publish": return SavedPublish.ToString();
                case "DisableCrossSearch": return SavedDisableCrossSearch.ToString();
                case "LockedTime": return SavedLockedTime.ToString();
                case "LockedUser": return SavedLockedUser.ToString();
                case "ApiCountDate": return SavedApiCountDate.ToString();
                case "ApiCount": return SavedApiCount.ToString();
                case "Comments": return SavedComments;
                case "Creator": return SavedCreator.ToString();
                case "Updator": return SavedUpdator.ToString();
                case "CreatedTime": return SavedCreatedTime.ToString();
                default: return GetSavedValue(
                    context: context,
                    column: column);
            }
        }

        public Dictionary<string, string> PropertyValues(Context context, List<Column> columns)
        {
            var hash = new Dictionary<string, string>();
            columns?
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "TenantId":
                            hash.Add("TenantId", TenantId.ToString());
                            break;
                        case "SiteId":
                            hash.Add("SiteId", SiteId.ToString());
                            break;
                        case "UpdatedTime":
                            hash.Add("UpdatedTime", UpdatedTime.Value.ToString());
                            break;
                        case "Ver":
                            hash.Add("Ver", Ver.ToString());
                            break;
                        case "Title":
                            hash.Add("Title", Title.Value);
                            break;
                        case "Body":
                            hash.Add("Body", Body);
                            break;
                        case "TitleBody":
                            hash.Add("TitleBody", TitleBody.ToString());
                            break;
                        case "SiteName":
                            hash.Add("SiteName", SiteName);
                            break;
                        case "SiteGroupName":
                            hash.Add("SiteGroupName", SiteGroupName);
                            break;
                        case "GridGuide":
                            hash.Add("GridGuide", GridGuide);
                            break;
                        case "EditorGuide":
                            hash.Add("EditorGuide", EditorGuide);
                            break;
                        case "CalendarGuide":
                            hash.Add("CalendarGuide", CalendarGuide);
                            break;
                        case "CrosstabGuide":
                            hash.Add("CrosstabGuide", CrosstabGuide);
                            break;
                        case "GanttGuide":
                            hash.Add("GanttGuide", GanttGuide);
                            break;
                        case "BurnDownGuide":
                            hash.Add("BurnDownGuide", BurnDownGuide);
                            break;
                        case "TimeSeriesGuide":
                            hash.Add("TimeSeriesGuide", TimeSeriesGuide);
                            break;
                        case "AnalyGuide":
                            hash.Add("AnalyGuide", AnalyGuide);
                            break;
                        case "KambanGuide":
                            hash.Add("KambanGuide", KambanGuide);
                            break;
                        case "ImageLibGuide":
                            hash.Add("ImageLibGuide", ImageLibGuide);
                            break;
                        case "ReferenceType":
                            hash.Add("ReferenceType", ReferenceType);
                            break;
                        case "ParentId":
                            hash.Add("ParentId", ParentId.ToString());
                            break;
                        case "InheritPermission":
                            hash.Add("InheritPermission", InheritPermission.ToString());
                            break;
                        case "SiteSettings":
                            hash.Add("SiteSettings", SiteSettings.RecordingJson(context: context));
                            break;
                        case "Publish":
                            hash.Add("Publish", Publish.ToString());
                            break;
                        case "DisableCrossSearch":
                            hash.Add("DisableCrossSearch", DisableCrossSearch.ToString());
                            break;
                        case "LockedTime":
                            hash.Add("LockedTime", LockedTime.Value.ToString());
                            break;
                        case "Ancestors":
                            hash.Add("Ancestors", Ancestors.ToString());
                            break;
                        case "LockedUser":
                            hash.Add("LockedUser", LockedUser.Id.ToString());
                            break;
                        case "SiteMenu":
                            hash.Add("SiteMenu", SiteMenu.ToString());
                            break;
                        case "MonitorChangesColumns":
                            hash.Add("MonitorChangesColumns", MonitorChangesColumns.ToString());
                            break;
                        case "TitleColumns":
                            hash.Add("TitleColumns", TitleColumns.ToString());
                            break;
                        case "Export":
                            hash.Add("Export", Export.ToString());
                            break;
                        case "ApiCountDate":
                            hash.Add("ApiCountDate", ApiCountDate.ToString());
                            break;
                        case "ApiCount":
                            hash.Add("ApiCount", ApiCount.ToString());
                            break;
                        case "DisableSiteCreatorPermission":
                            hash.Add("DisableSiteCreatorPermission", DisableSiteCreatorPermission.ToString());
                            break;
                        case "Comments":
                            hash.Add("Comments", Comments.ToJson());
                            break;
                        case "Creator":
                            hash.Add("Creator", Creator.Id.ToString());
                            break;
                        case "Updator":
                            hash.Add("Updator", Updator.Id.ToString());
                            break;
                        case "CreatedTime":
                            hash.Add("CreatedTime", CreatedTime.Value.ToString());
                            break;
                        case "VerUp":
                            hash.Add("VerUp", VerUp.ToString());
                            break;
                        case "Timestamp":
                            hash.Add("Timestamp", Timestamp);
                            break;
                        default:
                            hash.Add(column.ColumnName, GetValue(
                                context: context,
                                column: column));
                            break;
                    }
                });
            return hash;
        }

        public bool PropertyUpdated(Context context, string name)
        {
            switch (name)
            {
                case "TenantId": return TenantId_Updated(context: context);
                case "Ver": return Ver_Updated(context: context);
                case "Title": return Title_Updated(context: context);
                case "Body": return Body_Updated(context: context);
                case "SiteName": return SiteName_Updated(context: context);
                case "SiteGroupName": return SiteGroupName_Updated(context: context);
                case "GridGuide": return GridGuide_Updated(context: context);
                case "EditorGuide": return EditorGuide_Updated(context: context);
                case "CalendarGuide": return CalendarGuide_Updated(context: context);
                case "CrosstabGuide": return CrosstabGuide_Updated(context: context);
                case "GanttGuide": return GanttGuide_Updated(context: context);
                case "BurnDownGuide": return BurnDownGuide_Updated(context: context);
                case "TimeSeriesGuide": return TimeSeriesGuide_Updated(context: context);
                case "AnalyGuide": return AnalyGuide_Updated(context: context);
                case "KambanGuide": return KambanGuide_Updated(context: context);
                case "ImageLibGuide": return ImageLibGuide_Updated(context: context);
                case "ReferenceType": return ReferenceType_Updated(context: context);
                case "ParentId": return ParentId_Updated(context: context);
                case "InheritPermission": return InheritPermission_Updated(context: context);
                case "SiteSettings": return SiteSettings_Updated(context: context);
                case "Publish": return Publish_Updated(context: context);
                case "DisableCrossSearch": return DisableCrossSearch_Updated(context: context);
                case "LockedTime": return LockedTime_Updated(context: context);
                case "LockedUser": return LockedUser_Updated(context: context);
                case "ApiCountDate": return ApiCountDate_Updated(context: context);
                case "ApiCount": return ApiCount_Updated(context: context);
                case "Comments": return Comments_Updated(context: context);
                case "Creator": return Creator_Updated(context: context);
                case "Updator": return Updator_Updated(context: context);
                default: 
                    switch (Def.ExtendedColumnTypes.Get(name ?? string.Empty))
                    {
                        case "Class": return Class_Updated(name);
                        case "Num": return Num_Updated(name);
                        case "Date": return Date_Updated(name);
                        case "Description": return Description_Updated(name);
                        case "Check": return Check_Updated(name);
                        case "Attachments": return Attachments_Updated(name);
                    }
                    break;
            }
            return false;
        }

        public List<long> SwitchTargets;

        public SiteModel()
        {
        }

        public SiteModel(
            Context context,
            long parentId,
            long inheritPermission,
            Dictionary<string, string> formData = null,
            SiteApiModel siteApiModel = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            ParentId = parentId;
            InheritPermission = inheritPermission;
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    formData: formData);
            }
            if (siteApiModel != null)
            {
                SetByApi(
                    context: context,
                    ss: new SiteSettings(),
                    data: siteApiModel);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public SiteModel(
            Context context,
            long siteId,
            Dictionary<string, string> formData = null,
            SiteApiModel siteApiModel = null,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            SiteId = siteId;
            Get(context: context);
            if (clearSessions) ClearSessions(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    formData: formData);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public SiteModel(
            Context context,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    formData: formData);
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
            Session_SiteSettings(context: context, value: null);
            Session_MonitorChangesColumns(context: context, value: null);
            Session_TitleColumns(context: context, value: null);
            Session_Export(context: context, value: null);
            Session_DisableSiteCreatorPermission(context: context, value: null);
        }

        public SiteModel Get(
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
            where = where ?? Rds.SitesWhereDefault(
                context: context,
                siteModel: this);
            column = (column ?? Rds.SitesDefaultColumns());
            join = join ?? Rds.SitesJoinDefault();
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    tableType: tableType,
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            SetSiteSettingsProperties(context: context);
            return this;
        }

        public string ToValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            return PropertyValue(
                context: context,
                column: column);
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "SiteId":
                    return SiteId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SiteName":
                    return SiteName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SiteGroupName":
                    return SiteGroupName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "GridGuide":
                    return GridGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "EditorGuide":
                    return EditorGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "CalendarGuide":
                    return CalendarGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "CrosstabGuide":
                    return CrosstabGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "GanttGuide":
                    return GanttGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "BurnDownGuide":
                    return BurnDownGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TimeSeriesGuide":
                    return TimeSeriesGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AnalyGuide":
                    return AnalyGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "KambanGuide":
                    return KambanGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ImageLibGuide":
                    return ImageLibGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ReferenceType":
                    return ReferenceType.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "InheritPermission":
                    return InheritPermission.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Publish":
                    return Publish.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DisableCrossSearch":
                    return DisableCrossSearch.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TitleBody":
                    return TitleBody.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public string FullText(
            Context context,
            SiteSettings ss,
            bool backgroundTask = false,
            bool onCreating = false)
        {
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
            if (ReferenceType == "Wikis") return null;
            var fullText = new System.Text.StringBuilder();
            if (ss.FullTextIncludeBreadcrumb == true)
            {
                SiteInfo.TenantCaches
                    .Get(context.TenantId)?
                    .SiteMenu.Breadcrumb(
                        context: context,
                        siteId: SiteId)
                    .FullText(
                        context: context,
                        fullText: fullText);
            }
            if (ss.FullTextIncludeSiteId == true)
            {
                fullText.Append($" {ss.SiteId}");
            }
            if (ss.FullTextIncludeSiteTitle == true)
            {
                fullText.Append($" {ss.Title}");
            }
            ss.GetEditorColumnNames(
                context: context,
                columnOnly: true)
                    .Select(columnName => ss.GetColumn(
                        context: context,
                        columnName: columnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "Title":
                                Title.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Body":
                                Body.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            case "Comments":
                                Comments.FullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                            default:
                                BaseFullText(
                                    context: context,
                                    column: column,
                                    fullText: fullText);
                                break;
                        }
                    });
            Creator.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "Creator"),
                fullText);
            Updator.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "Updator"),
                fullText);
            CreatedTime.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "CreatedTime"),
                fullText);
            UpdatedTime.FullText(
                context,
                column: ss.GetColumn(
                    context: context,
                    columnName: "UpdatedTime"),
                fullText);
            if (!onCreating)
            {
                FullTextExtensions.OutgoingMailsFullText(
                    context: context,
                    ss: ss,
                    fullText: fullText,
                    referenceType: "Sites",
                    referenceId: SiteId);
            }
            return fullText
                .ToString()
                .Replace("　", " ")
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .Distinct()
                .Join(" ");
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true,
            bool checkConflict = true)
        {
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            var verUp = Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp);
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements,
                checkConflict: checkConflict,
                verUp: verUp));
            statements.AddRange(GetReminderSchedulesStatements(context: context));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: SiteId);
            }
            if (get)
            {
                Get(context: context);
            }
            UpdateRelatedRecords(
                context: context,
                get: get,
                addUpdatedTimeParam: true,
                addUpdatorParam: true,
                updateItems: true);
            SiteInfo.Reflesh(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null,
            bool checkConflict = true,
            bool verUp = false)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.SitesWhereDefault(
                context: context,
                siteModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.SitesCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            if (RecordPermissions != null)
            {
                statements.UpdatePermissions(
                    context: context,
                    ss: ss,
                    referenceId: SiteId,
                    permissions: RecordPermissions,
                    site: true);
            }
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: this,
                referenceId: SiteId);
            return new List<SqlStatement>
            {
                Rds.UpdateSites(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.SitesParamDefault(
                        context: context,
                        ss: ss,
                        siteModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement()
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = SiteId
                }
            };
        }

        public void UpdateRelatedRecords(
            Context context,
            bool get = false,
            string previousTitle = null,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: UpdateRelatedRecordsStatements(
                    context: context,
                    addUpdatedTimeParam: addUpdatedTimeParam,
                    addUpdatorParam: addUpdatorParam,
                    updateItems: updateItems)
                        .ToArray());
        }

        public List<SqlStatement> UpdateRelatedRecordsStatements(
            Context context,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            var fullText = FullText(context, SiteSettings);
            var statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                where: Rds.ItemsWhere().ReferenceId(SiteId),
                param: Rds.ItemsParam()
                    .SiteId(SiteId)
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                addUpdatedTimeParam: addUpdatedTimeParam,
                addUpdatorParam: addUpdatorParam,
                _using: updateItems));
            statements.Add(Rds.PhysicalDeleteLinks(
                where: Rds.LinksWhere().SourceId(SiteId)));
            statements.Add(LinkUtilities.Insert(SiteSettings.Links
                ?.Where(o => o.SiteId > 0)
                .Select(o => o.SiteId)
                .Distinct()
                .ToDictionary(o => o, o => SiteId)));
            return statements;
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    dataTableName: dataTableName,
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Sites")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertSites(
                    where: where ?? Rds.SitesWhereDefault(
                        context: context,
                        siteModel: this),
                    param: param ?? Rds.SitesParamDefault(
                        context: context,
                        ss: ss,
                        siteModel: this,
                        setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            SiteId = (response.Id ?? SiteId).ToLong();
            Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss)
        {
            var siteMenu = SiteInfo.TenantCaches.Get(TenantId)?
                .SiteMenu
                .Children(
                    context: context,
                    siteId: ss.SiteId,
                    withParent: true);
            var siteIds = siteMenu
                .Select(o => o.SiteId)
                .ToList();
            var outside = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .InheritPermission_In(siteIds)))
                            .AsEnumerable()
                            .FirstOrDefault(o => !siteMenu.Any(p => p.SiteId == o.Long("SiteId")));
            if (outside != null)
            {
                return new ErrorData(
                    type: Error.Types.CannotDeletePermissionInherited,
                    data: $"{outside.Long("SiteId")} {outside.String("Title")}");
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        factory: context,
                        where: Rds.ItemsWhere().SiteId_In(siteIds)),
                    Rds.DeleteDashboards(
                        factory: context,
                        where: Rds.DashboardsWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Dashboards")
                            .Select(o => o.SiteId))),
                    Rds.DeleteIssues(
                        factory: context,
                        where: Rds.IssuesWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Issues")
                            .Select(o => o.SiteId))),
                    Rds.DeleteResults(
                        factory: context,
                        where: Rds.ResultsWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Results")
                            .Select(o => o.SiteId))),
                    Rds.DeleteWikis(
                        factory: context,
                        where: Rds.WikisWhere().SiteId_In(siteMenu
                            .Where(o => o.ReferenceType == "Wikis")
                            .Select(o => o.SiteId))),
                    Rds.DeleteSites(
                        factory: context,
                        where: Rds.SitesWhere()
                            .TenantId(TenantId)
                            .SiteId_In(siteIds))
                });
            SiteInfo.DeleteSiteCaches(
                context: context,
                siteIds: siteIds);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, long siteId)
        {
            SiteId = siteId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere().ReferenceId(SiteId)),
                    Rds.RestoreSites(
                        factory: context,
                        where: Rds.SitesWhere().SiteId(SiteId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteSites(
                    tableType: tableType,
                    where: Rds.SitesWhere().TenantId(TenantId).SiteId(SiteId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            Dictionary<string, string> formData)
        {
            var ss = new SiteSettings();
            SetByFormData(
                context: context,
                ss: ss,
                formData: formData);
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Ver = context.QueryStrings.Int("ver");
            }
            SetSiteSettings(context: context);
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        private void SetByFormData(Context context, SiteSettings ss, Dictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Sites_Title": Title = new Title(SiteId, value); break;
                    case "Sites_Body": Body = value.ToString(); break;
                    case "Sites_SiteName": SiteName = value.ToString(); break;
                    case "Sites_SiteGroupName": SiteGroupName = value.ToString(); break;
                    case "Sites_GridGuide": GridGuide = value.ToString(); break;
                    case "Sites_EditorGuide": EditorGuide = value.ToString(); break;
                    case "Sites_CalendarGuide": CalendarGuide = value.ToString(); break;
                    case "Sites_CrosstabGuide": CrosstabGuide = value.ToString(); break;
                    case "Sites_GanttGuide": GanttGuide = value.ToString(); break;
                    case "Sites_BurnDownGuide": BurnDownGuide = value.ToString(); break;
                    case "Sites_TimeSeriesGuide": TimeSeriesGuide = value.ToString(); break;
                    case "Sites_AnalyGuide": AnalyGuide = value.ToString(); break;
                    case "Sites_KambanGuide": KambanGuide = value.ToString(); break;
                    case "Sites_ImageLibGuide": ImageLibGuide = value.ToString(); break;
                    case "Sites_ReferenceType": ReferenceType = value.ToString(); break;
                    case "Sites_InheritPermission": InheritPermission = value.ToLong(); break;
                    case "Sites_Publish": Publish = value.ToBool(); break;
                    case "Sites_DisableCrossSearch": DisableCrossSearch = value.ToBool(); break;
                    case "Sites_Timestamp": Timestamp = value.ToString(); break;
                    case "Comments": Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: value); break;
                    case "VerUp": VerUp = value.ToBool(); break;
                    case "CurrentPermissionsAll":
                        RecordPermissions = context.Forms.List("CurrentPermissionsAll");
                        // アクセス権を継承する場合にはPermissionsテーブルのレコードを削除する
                        // CurrentPermissionsAllよりInheritPermissionが先に処理された場合の対策
                        if (context.Forms.ContainsKey("InheritPermission")
                            && context.Forms.Long("InheritPermission") != SiteId)
                        {
                            RecordPermissions = new List<string>();
                        }
                        break;
                    case "InheritPermission":
                        // アクセス権を継承する場合にはPermissionsテーブルのレコードを削除する
                        if (context.Forms.Long("InheritPermission") != SiteId)
                        {
                            RecordPermissions = new List<string>();
                        }
                        break;
                    default:
                        if (key.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: key.Substring("Comment".Length).ToInt(),
                                body: value);
                        }
                        else
                        {
                            var column = ss.GetColumn(
                                context: context,
                                columnName: key.Split_2nd('_'));
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.ColumnName,
                                        value: new Num(
                                            context: context,
                                            column: column,
                                            value: value));
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
        }

        public void SetByModel(SiteModel siteModel)
        {
            TenantId = siteModel.TenantId;
            UpdatedTime = siteModel.UpdatedTime;
            Title = siteModel.Title;
            Body = siteModel.Body;
            SiteName = siteModel.SiteName;
            SiteGroupName = siteModel.SiteGroupName;
            GridGuide = siteModel.GridGuide;
            EditorGuide = siteModel.EditorGuide;
            CalendarGuide = siteModel.CalendarGuide;
            CrosstabGuide = siteModel.CrosstabGuide;
            GanttGuide = siteModel.GanttGuide;
            BurnDownGuide = siteModel.BurnDownGuide;
            TimeSeriesGuide = siteModel.TimeSeriesGuide;
            AnalyGuide = siteModel.AnalyGuide;
            KambanGuide = siteModel.KambanGuide;
            ImageLibGuide = siteModel.ImageLibGuide;
            ReferenceType = siteModel.ReferenceType;
            ParentId = siteModel.ParentId;
            InheritPermission = siteModel.InheritPermission;
            SiteSettings = siteModel.SiteSettings;
            Publish = siteModel.Publish;
            DisableCrossSearch = siteModel.DisableCrossSearch;
            LockedTime = siteModel.LockedTime;
            Ancestors = siteModel.Ancestors;
            LockedUser = siteModel.LockedUser;
            SiteMenu = siteModel.SiteMenu;
            MonitorChangesColumns = siteModel.MonitorChangesColumns;
            TitleColumns = siteModel.TitleColumns;
            Export = siteModel.Export;
            ApiCountDate = siteModel.ApiCountDate;
            ApiCount = siteModel.ApiCount;
            DisableSiteCreatorPermission = siteModel.DisableSiteCreatorPermission;
            Comments = siteModel.Comments;
            Creator = siteModel.Creator;
            Updator = siteModel.Updator;
            CreatedTime = siteModel.CreatedTime;
            VerUp = siteModel.VerUp;
            Comments = siteModel.Comments;
            ClassHash = siteModel.ClassHash;
            NumHash = siteModel.NumHash;
            DateHash = siteModel.DateHash;
            DescriptionHash = siteModel.DescriptionHash;
            CheckHash = siteModel.CheckHash;
            AttachmentsHash = siteModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss, SiteApiModel data)
        {
            if (data.Title != null) Title = new Title(SiteId, data.Title);
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.SiteName != null) SiteName = data.SiteName.ToString().ToString();
            if (data.SiteGroupName != null) SiteGroupName = data.SiteGroupName.ToString().ToString();
            if (data.GridGuide != null) GridGuide = data.GridGuide.ToString().ToString();
            if (data.EditorGuide != null) EditorGuide = data.EditorGuide.ToString().ToString();
            if (data.CalendarGuide != null) CalendarGuide = data.CalendarGuide.ToString().ToString();
            if (data.CrosstabGuide != null) CrosstabGuide = data.CrosstabGuide.ToString().ToString();
            if (data.GanttGuide != null) GanttGuide = data.GanttGuide.ToString().ToString();
            if (data.BurnDownGuide != null) BurnDownGuide = data.BurnDownGuide.ToString().ToString();
            if (data.TimeSeriesGuide != null) TimeSeriesGuide = data.TimeSeriesGuide.ToString().ToString();
            if (data.AnalyGuide != null) AnalyGuide = data.AnalyGuide.ToString().ToString();
            if (data.KambanGuide != null) KambanGuide = data.KambanGuide.ToString().ToString();
            if (data.ImageLibGuide != null) ImageLibGuide = data.ImageLibGuide.ToString().ToString();
            if (data.ReferenceType != null) ReferenceType = data.ReferenceType.ToString().ToString();
            if (data.InheritPermission != null) InheritPermission = data.InheritPermission.ToLong().ToLong();
            if (data.Publish != null) Publish = data.Publish.ToBool().ToBool();
            if (data.DisableCrossSearch != null) DisableCrossSearch = data.DisableCrossSearch.ToBool().ToBool();
            if (data.Permissions != null) RecordPermissions = data.Permissions;
            if (data.SiteSettings != null) SiteSettings = data.SiteSettings;
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => SetClass(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => SetNum(
                columnName: o.Key,
                value: new Num(
                    context: context,
                    column: ss.GetColumn(
                        context: context,
                        columnName: o.Key),
                    value: o.Value.ToString())));
            data.DateHash?.ForEach(o => SetDate(
                columnName: o.Key,
                value: o.Value.ToDateTime().ToUniversal(context: context)));
            data.DescriptionHash?.ForEach(o => SetDescription(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash?.ForEach(o => SetCheck(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash?.ForEach(o =>
            {
                string columnName = o.Key;
                Attachments newAttachments = o.Value;
                Attachments oldAttachments;
                if (columnName == "Attachments#Uploading")
                {
                    var kvp = AttachmentsHash
                        .FirstOrDefault(x => x.Value
                            .Any(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st()));
                    columnName = kvp.Key;
                    oldAttachments = kvp.Value;
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    if (column.OverwriteSameFileName == true)
                    {
                        var oldAtt = oldAttachments
                            .FirstOrDefault(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st());
                        if (oldAtt != null)
                        {
                            oldAtt.Deleted = true;
                            oldAtt.Overwritten = true;
                        }
                    }
                    newAttachments.ForEach(att => att.Guid = att.Guid.Split_2nd());
                }
                else
                {
                    oldAttachments = AttachmentsHash.Get(columnName);
                }
                if (oldAttachments != null)
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    var newGuidSet = new HashSet<string>(newAttachments.Select(x => x.Guid).Distinct());
                    var newNameSet = new HashSet<string>(newAttachments.Select(x => x.Name).Distinct());
                    newAttachments.ForEach(newAttachment =>
                    {
                        newAttachment.AttachmentAction(
                            context: context,
                            column: column,
                            oldAttachments: oldAttachments);
                    });
                    if (column.OverwriteSameFileName == true)
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) =>
                                !newGuidSet.Contains(oldvalue.Guid) &&
                                !newNameSet.Contains(oldvalue.Name)));
                    }
                    else
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) => !newGuidSet.Contains(oldvalue.Guid)));
                    }
                }
                SetAttachments(columnName: columnName, value: newAttachments);
            });
            if (data.DisableSiteCreatorPermission != null) DisableSiteCreatorPermission = data.DisableSiteCreatorPermission.ToBool();
            SetSiteSettings(
                context: context,
                setSiteSettingsPropertiesBySession: false);
        }

        public string ReplacedDisplayValues(
            Context context,
            SiteSettings ss,
            string value)
        {
            ss.IncludedColumns(value: value).ForEach(column =>
                value = value.Replace(
                    $"[{column.ColumnName}]",
                    ToDisplay(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: Mine(context: context))));
            value = ReplacedContextValues(context, value);
            return value;
        }

        private string ReplacedContextValues(Context context, string value)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: SiteId);
            var mailAddress = MailAddressUtilities.Get(
                context: context,
                userId: context.UserId);
            value = value
                .Replace("{Url}", url)
                .Replace("{LoginId}", context.User.LoginId)
                .Replace("{UserName}", context.User.Name)
                .Replace("{MailAddress}", mailAddress);
            return value;
        }

        private void SetBySession(Context context)
        {
            if (!context.Forms.Exists("Sites_SiteSettings")) SiteSettings = Session_SiteSettings(context: context);
            if (!context.Forms.Exists("Sites_MonitorChangesColumns")) MonitorChangesColumns = Session_MonitorChangesColumns(context: context);
            if (!context.Forms.Exists("Sites_TitleColumns")) TitleColumns = Session_TitleColumns(context: context);
            if (!context.Forms.Exists("Sites_Export")) Export = Session_Export(context: context);
            if (!context.Forms.Exists("Sites_DisableSiteCreatorPermission")) DisableSiteCreatorPermission = Session_DisableSiteCreatorPermission(context: context);
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
                        case "TenantId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                TenantId = dataRow[column.ColumnName].ToInt();
                                SavedTenantId = TenantId;
                            }
                            break;
                        case "SiteId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SiteId = dataRow[column.ColumnName].ToLong();
                                SavedSiteId = SiteId;
                            }
                            break;
                        case "UpdatedTime":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                                SavedUpdatedTime = UpdatedTime.Value;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Title":
                            Title = new Title(dataRow, "SiteId");
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "SiteName":
                            SiteName = dataRow[column.ColumnName].ToString();
                            SavedSiteName = SiteName;
                            break;
                        case "SiteGroupName":
                            SiteGroupName = dataRow[column.ColumnName].ToString();
                            SavedSiteGroupName = SiteGroupName;
                            break;
                        case "GridGuide":
                            GridGuide = dataRow[column.ColumnName].ToString();
                            SavedGridGuide = GridGuide;
                            break;
                        case "EditorGuide":
                            EditorGuide = dataRow[column.ColumnName].ToString();
                            SavedEditorGuide = EditorGuide;
                            break;
                        case "CalendarGuide":
                            CalendarGuide = dataRow[column.ColumnName].ToString();
                            SavedCalendarGuide = CalendarGuide;
                            break;
                        case "CrosstabGuide":
                            CrosstabGuide = dataRow[column.ColumnName].ToString();
                            SavedCrosstabGuide = CrosstabGuide;
                            break;
                        case "GanttGuide":
                            GanttGuide = dataRow[column.ColumnName].ToString();
                            SavedGanttGuide = GanttGuide;
                            break;
                        case "BurnDownGuide":
                            BurnDownGuide = dataRow[column.ColumnName].ToString();
                            SavedBurnDownGuide = BurnDownGuide;
                            break;
                        case "TimeSeriesGuide":
                            TimeSeriesGuide = dataRow[column.ColumnName].ToString();
                            SavedTimeSeriesGuide = TimeSeriesGuide;
                            break;
                        case "AnalyGuide":
                            AnalyGuide = dataRow[column.ColumnName].ToString();
                            SavedAnalyGuide = AnalyGuide;
                            break;
                        case "KambanGuide":
                            KambanGuide = dataRow[column.ColumnName].ToString();
                            SavedKambanGuide = KambanGuide;
                            break;
                        case "ImageLibGuide":
                            ImageLibGuide = dataRow[column.ColumnName].ToString();
                            SavedImageLibGuide = ImageLibGuide;
                            break;
                        case "ReferenceType":
                            ReferenceType = dataRow[column.ColumnName].ToString();
                            SavedReferenceType = ReferenceType;
                            break;
                        case "ParentId":
                            ParentId = dataRow[column.ColumnName].ToLong();
                            SavedParentId = ParentId;
                            break;
                        case "InheritPermission":
                            InheritPermission = dataRow[column.ColumnName].ToLong();
                            SavedInheritPermission = InheritPermission;
                            break;
                        case "SiteSettings":
                            SiteSettings = GetSiteSettings(context: context, dataRow: dataRow);
                            SavedSiteSettings = SiteSettings.RecordingJson(context: context);
                            break;
                        case "Publish":
                            Publish = dataRow[column.ColumnName].ToBool();
                            SavedPublish = Publish;
                            break;
                        case "DisableCrossSearch":
                            DisableCrossSearch = dataRow[column.ColumnName].ToBool();
                            SavedDisableCrossSearch = DisableCrossSearch;
                            break;
                        case "LockedTime":
                            LockedTime = new Time(context, dataRow, column.ColumnName);
                            SavedLockedTime = LockedTime.Value;
                            break;
                        case "LockedUser":
                            LockedUser = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedLockedUser = LockedUser.Id;
                            break;
                        case "ApiCountDate":
                            ApiCountDate = dataRow[column.ColumnName].ToDateTime();
                            SavedApiCountDate = ApiCountDate;
                            break;
                        case "ApiCount":
                            ApiCount = dataRow[column.ColumnName].ToInt();
                            SavedApiCount = ApiCount;
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
                || TenantId_Updated(context: context)
                || Ver_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || SiteName_Updated(context: context)
                || SiteGroupName_Updated(context: context)
                || GridGuide_Updated(context: context)
                || EditorGuide_Updated(context: context)
                || CalendarGuide_Updated(context: context)
                || CrosstabGuide_Updated(context: context)
                || GanttGuide_Updated(context: context)
                || BurnDownGuide_Updated(context: context)
                || TimeSeriesGuide_Updated(context: context)
                || AnalyGuide_Updated(context: context)
                || KambanGuide_Updated(context: context)
                || ImageLibGuide_Updated(context: context)
                || ReferenceType_Updated(context: context)
                || ParentId_Updated(context: context)
                || InheritPermission_Updated(context: context)
                || SiteSettings_Updated(context: context)
                || Publish_Updated(context: context)
                || DisableCrossSearch_Updated(context: context)
                || LockedTime_Updated(context: context)
                || LockedUser_Updated(context: context)
                || ApiCountDate_Updated(context: context)
                || ApiCount_Updated(context: context)
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
                || TenantId_Updated(context: context)
                || Ver_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || SiteName_Updated(context: context)
                || SiteGroupName_Updated(context: context)
                || GridGuide_Updated(context: context)
                || EditorGuide_Updated(context: context)
                || CalendarGuide_Updated(context: context)
                || CrosstabGuide_Updated(context: context)
                || GanttGuide_Updated(context: context)
                || BurnDownGuide_Updated(context: context)
                || TimeSeriesGuide_Updated(context: context)
                || AnalyGuide_Updated(context: context)
                || KambanGuide_Updated(context: context)
                || ImageLibGuide_Updated(context: context)
                || ReferenceType_Updated(context: context)
                || ParentId_Updated(context: context)
                || InheritPermission_Updated(context: context)
                || SiteSettings_Updated(context: context)
                || Publish_Updated(context: context)
                || DisableCrossSearch_Updated(context: context)
                || LockedTime_Updated(context: context)
                || LockedUser_Updated(context: context)
                || ApiCountDate_Updated(context: context)
                || ApiCount_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public override List<string> Mine(Context context)
        {
            if (MineCache == null)
            {
                var mine = new List<string>();
                var userId = context.UserId;
                if (SavedCreator == userId) mine.Add("Creator");
                if (SavedUpdator == userId) mine.Add("Updator");
                MineCache = mine;
            }
            return MineCache;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SiteApiModel GetByApi(Context context)
        {
            var data = new SiteApiModel();
            data.ApiVersion = context.ApiVersion;
            data.TenantId = TenantId;
            data.SiteId = SiteId;
            data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context);
            data.Ver = Ver;
            data.Title = Title.Value;
            data.Body = Body;
            data.SiteName = SiteName;
            data.SiteGroupName = SiteGroupName;
            data.GridGuide = GridGuide;
            data.EditorGuide = EditorGuide;
            data.CalendarGuide = CalendarGuide;
            data.CrosstabGuide = CrosstabGuide;
            data.GanttGuide = GanttGuide;
            data.BurnDownGuide = BurnDownGuide;
            data.TimeSeriesGuide = TimeSeriesGuide;
            data.AnalyGuide = AnalyGuide;
            data.KambanGuide = KambanGuide;
            data.ImageLibGuide = ImageLibGuide;
            data.ReferenceType = ReferenceType;
            data.ParentId = ParentId;
            data.InheritPermission = InheritPermission;
            if (context.CanManagePermission(ss: SiteSettings))
            {
                data.Permissions = PermissionUtilities.CurrentCollection(
                    context: context,
                    referenceId: SiteId)
                        .Select(permission => $"{permission.Name},{permission.Id},{(long)permission.Type}")
                        .ToList();
            }
            data.SiteSettings = SiteSettings.RecordingData(context: context);
            data.Publish = Publish;
            data.DisableCrossSearch = DisableCrossSearch;
            data.LockedTime = LockedTime.Value.ToLocal(context: context);
            data.LockedUser = LockedUser.Id;
            data.ApiCountDate = ApiCountDate;
            data.ApiCount = ApiCount;
            data.Comments = Comments.ToLocal(context: context).ToJson();
            data.Creator = Creator.Id;
            data.Updator = Updator.Id;
            data.CreatedTime = CreatedTime.Value.ToLocal(context: context);
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ErrorData Create(Context context, bool otherInitValue = false)
        {
            if (!otherInitValue)
            {
                SiteSettings = new SiteSettings(context: context, referenceType: ReferenceType);
            }
            TenantId = context.TenantId;
            var notInheritPermission = InheritPermission == 0 || RecordPermissions != null;
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Sites")
                            .Title(Title.Value.MaxLength(1024))),
                    Rds.InsertSites(
                        param: Rds.SitesParam()
                            .SiteId(raw: Def.Sql.Identity)
                            .TenantId(TenantId)
                            .Title(Title.Value.MaxLength(1024))
                            .Body(Body)
                            .SiteName(SiteName)
                            .SiteGroupName(SiteGroupName)
                            .GridGuide(GridGuide)
                            .EditorGuide(EditorGuide)
                            .CalendarGuide(CalendarGuide)
                            .CrosstabGuide(CrosstabGuide)
                            .GanttGuide(GanttGuide)
                            .BurnDownGuide(BurnDownGuide)
                            .TimeSeriesGuide(TimeSeriesGuide)
                            .AnalyGuide(TimeSeriesGuide)
                            .KambanGuide(KambanGuide)
                            .ImageLibGuide(ImageLibGuide)
                            .ReferenceType(ReferenceType.MaxLength(32))
                            .ParentId(ParentId)
                            .InheritPermission(raw: notInheritPermission
                                ? Def.Sql.Identity
                                : InheritPermission.ToString())
                            .SiteSettings(SiteSettings.RecordingJson(context: context))
                            .Publish(Publish)
                            .DisableCrossSearch(DisableCrossSearch)
                            .Comments(Comments.ToJson())),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(raw: Def.Sql.Identity),
                        param: Rds.ItemsParam().SiteId(raw: Def.Sql.Identity))
                });
            SiteId = response.Id ?? SiteId;
            Get(context: context);
            SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: this, referenceId: SiteId);
            var statements = new List<SqlStatement>();
            if (RecordPermissions != null)
            {
                statements.UpdatePermissions(
                    context: context,
                    ss: SiteSettings,
                    referenceId: SiteId,
                    permissions: RecordPermissions,
                    site: true);
            }
            statements.Add(Rds.InsertPermissions(
                param: Rds.PermissionsParam()
                    .ReferenceId(SiteId)
                    .DeptId(0)
                    .UserId(context.UserId)
                    .PermissionType(Permissions.Manager()),
                _using: notInheritPermission && !DisableSiteCreatorPermission));
            if (statements.Any(o => o.Using))
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: statements.ToArray());
            }
            switch (ReferenceType)
            {
                case "Wikis":
                    var wikiModel = new WikiModel(context: context, ss: SiteSettings)
                    {
                        SiteId = SiteId,
                        Title = Title,
                        Body = Body,
                        Comments = Comments
                    };
                    wikiModel.Create(context: context, ss: SiteSettings);
                    break;
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SiteSettings GetSiteSettings(Context context, DataRow dataRow)
        {
            return dataRow.String("SiteSettings").DeserializeSiteSettings(context: context) ??
                new SiteSettings(context: context, referenceType: ReferenceType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsPropertiesBySession(Context context)
        {
            SiteSettings = Session_SiteSettings(context: context);
            SetSiteSettingsProperties(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSiteSettingsProperties(Context context)
        {
            if (SiteSettings == null)
            {
                SiteSettings = SiteSettingsUtilities.SitesSiteSettings(
                    context: context, siteId: SiteId);
            }
            SiteSettings.SiteId = SiteId;
            SiteSettings.Title = Title.Value;
            SiteSettings.Body = Body;
            SiteSettings.ParentId = ParentId;
            SiteSettings.InheritPermission = InheritPermission;
            SiteSettings.AccessStatus = AccessStatus;
            SiteSettings.SetLinkedSiteSettings(context: context);
            SiteSettings.SetPermissions(
                context: context,
                referenceId: context.Id);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SetSiteSettings(
            Context context,
            bool setSiteSettingsPropertiesBySession = true)
        {
            var invalidFormat = string.Empty;
            var invalid = SiteValidators.OnSetSiteSettings(
                context: context,
                ss: SiteSettingsUtilities.Get(
                    context: context, siteModel: this, referenceId: SiteId),
                data: out invalidFormat);
            switch (invalid.Type)
            {
                case Error.Types.BadFormat:
                    return Messages.ResponseBadFormat(
                        context: context,
                        data: invalidFormat).ToJson();
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var res = new SitesResponseCollection(
                context: context,
                siteModel: this);
            if (setSiteSettingsPropertiesBySession)
            {
                SetSiteSettingsPropertiesBySession(context: context);
            }
            SetSiteSettings(context: context, res: res);
            Session_SiteSettings(
                context: context,
                value: SiteSettings.RecordingJson(context: context));
            return res
                .SetMemory("formChanged", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void UpsertServerScriptByApi(
            SiteSettings siteSetting,
            List<ApiSiteSettings.ServerScriptApiSettingModel> serverScriptsApiSiteSetting)
        {
            List<int> deleteSelected = new List<int>();
            serverScriptsApiSiteSetting.ForEach(ssApiSetting =>
            {
                var currentServerScript = siteSetting.ServerScripts?.FirstOrDefault(o =>
                    o.Id == ssApiSetting.Id.ToInt());
                if (ssApiSetting.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                {
                    deleteSelected.Add(ssApiSetting.Id.ToInt());
                }
                else
                {
                    if (currentServerScript != null)
                    {
                        // Update ServerScript site setting
                        currentServerScript.Update(
                            title: ssApiSetting.Title,
                            name: ssApiSetting.Name,
                            whenloadingSiteSettings: ssApiSetting.ServerScriptWhenloadingSiteSettings,
                            whenViewProcessing: ssApiSetting.ServerScriptWhenViewProcessing,
                            whenloadingRecord: ssApiSetting.ServerScriptWhenloadingRecord,
                            beforeFormula: ssApiSetting.ServerScriptBeforeFormula,
                            afterFormula: ssApiSetting.ServerScriptAfterFormula,
                            beforeCreate: ssApiSetting.ServerScriptBeforeCreate,
                            afterCreate: ssApiSetting.ServerScriptAfterCreate,
                            beforeUpdate: ssApiSetting.ServerScriptBeforeUpdate,
                            afterUpdate: ssApiSetting.ServerScriptAfterUpdate,
                            beforeDelete: ssApiSetting.ServerScriptBeforeDelete,
                            afterDelete: ssApiSetting.ServerScriptAfterDelete,
                            beforeOpeningPage: ssApiSetting.ServerScriptBeforeOpeningPage,
                            beforeOpeningRow: ssApiSetting.ServerScriptBeforeOpeningRow,
                            shared: ssApiSetting.ServerScriptShared,
                            background: default,
                            body: ssApiSetting.Body,
                            timeOut: default);
                    }
                    else
                    {
                        // Create new ServerScript site setting
                        SiteSettings.ServerScripts.Add(new ServerScript(
                            id: ssApiSetting.Id,
                            title: ssApiSetting.Title,
                            name: ssApiSetting.Name,
                            whenloadingSiteSettings: ssApiSetting.ServerScriptWhenloadingSiteSettings,
                            whenViewProcessing: ssApiSetting.ServerScriptWhenViewProcessing,
                            whenloadingRecord: ssApiSetting.ServerScriptWhenloadingRecord,
                            beforeFormula: ssApiSetting.ServerScriptBeforeFormula,
                            afterFormula: ssApiSetting.ServerScriptAfterFormula,
                            beforeCreate: ssApiSetting.ServerScriptBeforeCreate,
                            afterCreate: ssApiSetting.ServerScriptAfterCreate,
                            beforeUpdate: ssApiSetting.ServerScriptBeforeUpdate,
                            afterUpdate: ssApiSetting.ServerScriptAfterUpdate,
                            beforeDelete: ssApiSetting.ServerScriptBeforeDelete,
                            afterDelete: ssApiSetting.ServerScriptAfterDelete,
                            beforeOpeningPage: ssApiSetting.ServerScriptBeforeOpeningPage,
                            beforeOpeningRow: ssApiSetting.ServerScriptBeforeOpeningRow,
                            shared: ssApiSetting.ServerScriptShared,
                            body: ssApiSetting.Body,
                            background: default,
                            timeOut: default));
                    }
                }
            });
            // Check has deleted
            if (deleteSelected.Count() != 0)
            {
                siteSetting.ServerScripts.Delete(deleteSelected);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void UpsertScriptByApi(
            SiteSettings siteSetting,
            List<ApiSiteSettings.ScriptApiSettingModel> scriptsApiSiteSetting)
        {
            List<int> deleteSelected = new List<int>();
            scriptsApiSiteSetting.ForEach(scApiSiteSetting =>
            {
                var currentScript = siteSetting.Scripts?.
                     FirstOrDefault(o => o.Id == scApiSiteSetting.Id.ToInt());
                if (scApiSiteSetting.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                {
                    deleteSelected.Add(scApiSiteSetting.Id.ToInt());
                }
                else
                {
                    if (currentScript != null)
                    {
                        // Update Script site setting
                        currentScript.Update(
                            title: scApiSiteSetting.Title,
                            all: scApiSiteSetting.ScriptAll,
                            _new: scApiSiteSetting.ScriptNew,
                            edit: scApiSiteSetting.ScriptEdit,
                            index: scApiSiteSetting.ScriptIndex,
                            calendar: scApiSiteSetting.ScriptCalendar,
                            crosstab: scApiSiteSetting.ScriptCrosstab,
                            gantt: scApiSiteSetting.ScriptGantt,
                            burnDown: scApiSiteSetting.ScriptBurnDown,
                            timeSeries: scApiSiteSetting.ScriptTimeSeries,
                            analy: scApiSiteSetting.ScriptAnaly,
                            kamban: scApiSiteSetting.ScriptKamban,
                            imageLib: scApiSiteSetting.ScriptImageLib,
                            disabled: scApiSiteSetting.Disabled,
                            body: scApiSiteSetting.Body);
                    }
                    else
                    {
                        // Create new Script site setting
                        SiteSettings.Scripts.Add(new Script(
                            id: scApiSiteSetting.Id,
                            title: scApiSiteSetting.Title,
                            all: scApiSiteSetting.ScriptAll,
                            _new: scApiSiteSetting.ScriptNew,
                            edit: scApiSiteSetting.ScriptEdit,
                            index: scApiSiteSetting.ScriptIndex,
                            calendar: scApiSiteSetting.ScriptCalendar,
                            crosstab: scApiSiteSetting.ScriptCrosstab,
                            gantt: scApiSiteSetting.ScriptGantt,
                            burnDown: scApiSiteSetting.ScriptBurnDown,
                            timeSeries: scApiSiteSetting.ScriptTimeSeries,
                            analy: scApiSiteSetting.ScriptAnaly,
                            kamban: scApiSiteSetting.ScriptKamban,
                            imageLib: scApiSiteSetting.ScriptImageLib,
                            disabled: scApiSiteSetting.Disabled,
                            body: scApiSiteSetting.Body));
                    }
                }
            });
            // Check has deleted
            if (deleteSelected.Count() != 0)
            {
                siteSetting.Scripts.Delete(deleteSelected);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void UpsertStyleByApi(
            SiteSettings siteSetting,
            List<ApiSiteSettings.StyleApiSettingModel> styleApiSiteSetting)
        {
            List<int> deleteSelected = new List<int>();
            styleApiSiteSetting.ForEach(stApiSiteSetting =>
            {
                var currentStyle = siteSetting.Styles?.
                     FirstOrDefault(o => o.Id == stApiSiteSetting.Id.ToInt());
                if (stApiSiteSetting.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                {
                    deleteSelected.Add(stApiSiteSetting.Id.ToInt());
                }
                else
                {
                    if (currentStyle != null)
                    {
                        // Update Style site setting
                        currentStyle.Update(
                            title: stApiSiteSetting.Title,
                            all: stApiSiteSetting.StyleAll,
                            _new: stApiSiteSetting.StyleNew,
                            edit: stApiSiteSetting.StyleEdit,
                            index: stApiSiteSetting.StyleIndex,
                            calendar: stApiSiteSetting.StyleCalendar,
                            crosstab: stApiSiteSetting.StyleCrosstab,
                            gantt: stApiSiteSetting.StyleGantt,
                            burnDown: stApiSiteSetting.StyleBurnDown,
                            timeSeries: stApiSiteSetting.StyleTimeSeries,
                            analy: stApiSiteSetting.StyleAnaly,
                            kamban: stApiSiteSetting.StyleKamban,
                            imageLib: stApiSiteSetting.StyleImageLib,
                            disabled: stApiSiteSetting.Disabled,
                            body: stApiSiteSetting.Body);
                    }
                    else
                    {
                        // Add new Style site setting
                        SiteSettings.Styles.Add(new Style(
                            id: stApiSiteSetting.Id,
                            title: stApiSiteSetting.Title,
                            all: stApiSiteSetting.StyleAll,
                            _new: stApiSiteSetting.StyleNew,
                            edit: stApiSiteSetting.StyleEdit,
                            index: stApiSiteSetting.StyleIndex,
                            calendar: stApiSiteSetting.StyleCalendar,
                            crosstab: stApiSiteSetting.StyleCrosstab,
                            gantt: stApiSiteSetting.StyleGantt,
                            burnDown: stApiSiteSetting.StyleBurnDown,
                            timeSeries: stApiSiteSetting.StyleTimeSeries,
                            analy: stApiSiteSetting.StyleAnaly,
                            kamban: stApiSiteSetting.StyleKamban,
                            imageLib: stApiSiteSetting.StyleImageLib,
                            disabled: stApiSiteSetting.Disabled,
                            body: stApiSiteSetting.Body));
                    }
                }
            });
            // Check has deleted
            if (deleteSelected.Count() != 0)
            {
                siteSetting.Styles.Delete(deleteSelected);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void UpsertHtmlByApi(
            SiteSettings siteSetting,
            List<ApiSiteSettings.HtmlApiSettingModel> htmlsApiSiteSetting)
        {
            List<int> deleteSelected = new List<int>();
            htmlsApiSiteSetting.ForEach(htmlApiSiteSetting =>
            {
                var currentHtml = siteSetting.Htmls?.
                     FirstOrDefault(o => o.Id == htmlApiSiteSetting.Id.ToInt());
                if (htmlApiSiteSetting.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                {
                    deleteSelected.Add(htmlApiSiteSetting.Id.ToInt());
                }
                else
                {
                    if (currentHtml != null)
                    {
                        // Update html site setting
                        currentHtml.Update(
                            title: htmlApiSiteSetting.Title,
                            positionType: htmlApiSiteSetting.HtmlPositionType.ToEnum<Html.PositionTypes>(),
                            all: htmlApiSiteSetting.HtmlAll,
                            _new: htmlApiSiteSetting.HtmlNew,
                            edit: htmlApiSiteSetting.HtmlEdit,
                            index: htmlApiSiteSetting.HtmlIndex,
                            calendar: htmlApiSiteSetting.HtmlCalendar,
                            crosstab: htmlApiSiteSetting.HtmlCrosstab,
                            gantt: htmlApiSiteSetting.HtmlGantt,
                            burnDown: htmlApiSiteSetting.HtmlBurnDown,
                            timeSeries: htmlApiSiteSetting.HtmlTimeSeries,
                            analy: htmlApiSiteSetting.HtmlAnaly,
                            kamban: htmlApiSiteSetting.HtmlKamban,
                            imageLib: htmlApiSiteSetting.HtmlImageLib,
                            disabled: htmlApiSiteSetting.Disabled,
                            body: htmlApiSiteSetting.Body);
                    }
                    else
                    {
                        // Add new html site setting
                        SiteSettings.Htmls.Add(new Html(
                            id: htmlApiSiteSetting.Id,
                            title: htmlApiSiteSetting.Title,
                            positionType: htmlApiSiteSetting.HtmlPositionType.ToEnum<Html.PositionTypes>(),
                            all: htmlApiSiteSetting.HtmlAll,
                            _new: htmlApiSiteSetting.HtmlNew,
                            edit: htmlApiSiteSetting.HtmlEdit,
                            index: htmlApiSiteSetting.HtmlIndex,
                            calendar: htmlApiSiteSetting.HtmlCalendar,
                            crosstab: htmlApiSiteSetting.HtmlCrosstab,
                            gantt: htmlApiSiteSetting.HtmlGantt,
                            burnDown: htmlApiSiteSetting.HtmlBurnDown,
                            timeSeries: htmlApiSiteSetting.HtmlTimeSeries,
                            analy: htmlApiSiteSetting.HtmlAnaly,
                            kamban: htmlApiSiteSetting.HtmlKamban,
                            imageLib: htmlApiSiteSetting.HtmlImageLib,
                            disabled: htmlApiSiteSetting.Disabled,
                            body: htmlApiSiteSetting.Body));
                    }
                }
            });
            // Check has deleted
            if (deleteSelected.Count() != 0)
            {
                siteSetting.Htmls.Delete(deleteSelected);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void UpsertProcessByApi(
            SiteSettings siteSetting,
            List<ApiSiteSettings.ProcessApiSettingModel> processesApiSiteSetting,
            Context context)
        {
            List<int> deleteSelected = new List<int>();
            processesApiSiteSetting.ForEach(processApiSiteSetting =>
            {
                var currentProcess = siteSetting.Processes?.
                     FirstOrDefault(o => o.Id == processApiSiteSetting.Id.ToInt());
                if (processApiSiteSetting.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                {
                    deleteSelected.Add(processApiSiteSetting.Id.ToInt());
                }
                else if (currentProcess != null)
                {
                    currentProcess.Update(
                        name: processApiSiteSetting.Name,
                        displayName: processApiSiteSetting.DisplayName,
                        screenType: processApiSiteSetting.ScreenType?.ToString().ToEnum<Process.ScreenTypes>(),
                        currentStatus: processApiSiteSetting.CurrentStatus,
                        changedStatus: processApiSiteSetting.ChangedStatus,
                        description: processApiSiteSetting.Description,
                        tooltip: processApiSiteSetting.Tooltip,
                        confirmationMessage: processApiSiteSetting.ConfirmationMessage,
                        successMessage: processApiSiteSetting.SuccessMessage,
                        onClick: processApiSiteSetting.OnClick,
                        executionType: processApiSiteSetting.ExecutionType?.ToString().ToEnum<Process.ExecutionTypes>(),
                        actionType: processApiSiteSetting.ActionType?.ToString().ToEnum<Process.ActionTypes>(),
                        allowBulkProcessing: processApiSiteSetting.AllowBulkProcessing,
                        validationType: processApiSiteSetting.ValidationType?.ToString().ToEnum<Process.ValidationTypes>(),
                        validateInputs: ParseValidateInputs(
                            validateInputs: processApiSiteSetting.ValidateInputs,
                            process: currentProcess),
                        permissions: processApiSiteSetting.Permission != null ? ParsePermissions(
                            apiSettingPermission: processApiSiteSetting.Permission,
                            ss: siteSetting,
                            target: currentProcess) : null,
                        view: processApiSiteSetting.View,
                        errorMessage: processApiSiteSetting.ErrorMessage,
                        dataChanges: ParseDataChanges(
                            dataChanges: processApiSiteSetting.DataChanges,
                            process: currentProcess),
                        autoNumbering: processApiSiteSetting.AutoNumbering,
                        notifications: ParseNotifications(
                            notifications: processApiSiteSetting.Notifications,
                            process: currentProcess));
                }
                else
                {
                    SiteSettings.Processes.Add(new Process(
                        id: processApiSiteSetting.Id,
                        name: processApiSiteSetting.Name,
                        displayName: processApiSiteSetting.DisplayName,
                        screenType: processApiSiteSetting.ScreenType?.ToString().ToEnum<Process.ScreenTypes>(),
                        currentStatus: processApiSiteSetting.CurrentStatus != null ? (int)processApiSiteSetting.CurrentStatus : default,
                        changedStatus: processApiSiteSetting.ChangedStatus != null ? (int)processApiSiteSetting.ChangedStatus : default,
                        description: processApiSiteSetting.Description,
                        tooltip: processApiSiteSetting.Tooltip,
                        confirmationMessage: processApiSiteSetting.ConfirmationMessage,
                        successMessage: processApiSiteSetting.SuccessMessage,
                        onClick: processApiSiteSetting.OnClick,
                        executionType: processApiSiteSetting.ExecutionType?.ToString().ToEnum<Process.ExecutionTypes>(),
                        actionType: processApiSiteSetting.ActionType?.ToString().ToEnum<Process.ActionTypes>(),
                        allowBulkProcessing: processApiSiteSetting.AllowBulkProcessing,
                        validationType: processApiSiteSetting.ValidationType?.ToString().ToEnum<Process.ValidationTypes>(),
                        validateInputs: ParseValidateInputs(
                            validateInputs: processApiSiteSetting.ValidateInputs,
                            process: currentProcess),
                        permissions: ParsePermissions(
                            apiSettingPermission: processApiSiteSetting.Permission,
                            ss: siteSetting),
                        view: processApiSiteSetting.View,
                        errorMessage: processApiSiteSetting.ErrorMessage,
                        dataChanges: ParseDataChanges(
                            dataChanges: processApiSiteSetting.DataChanges,
                            process: currentProcess),
                        autoNumbering: processApiSiteSetting.AutoNumbering,
                        notifications: ParseNotifications(
                            notifications: processApiSiteSetting.Notifications,
                            process: currentProcess)));
                }
            });
            if (deleteSelected.Count() != 0)
            {
                siteSetting.Processes.Delete(deleteSelected);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void UpsertStatusControlByApi(
        SiteSettings siteSetting,
            List<ApiSiteSettings.StatusControlApiSettingModel> statusControlSettings,
            Context context)
        {
            List<int> deleteSelected = new List<int>();
            statusControlSettings.ForEach(statusControlSetting =>
            {
                var statusControl = siteSetting.StatusControls?.
                     FirstOrDefault(o => o.Id == statusControlSetting.Id.ToInt());
                if (statusControlSetting.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                {
                    deleteSelected.Add(statusControlSetting.Id.ToInt());
                }
                else if (statusControl != null)
                {
                    statusControl.Update(
                     name: statusControlSetting.Name,
                     description: statusControlSetting.Description,
                     status: statusControlSetting.Status,
                     readOnly: statusControlSetting.ReadOnly,
                     view: statusControlSetting.View,
                     columnHash: statusControlSetting.ColumnHash,
                     permissions: statusControlSetting.Permission != null ? ParsePermissions(
                         apiSettingPermission: statusControlSetting.Permission,
                         ss: siteSetting,
                         target: statusControl) : null);
                }
                else
                {
                    SiteSettings.StatusControls.Add(new StatusControl(
                     id: statusControlSetting.Id,
                     name: statusControlSetting.Name,
                     description: statusControlSetting.Description,
                     status: statusControlSetting.Status,
                     readOnly: statusControlSetting.ReadOnly,
                     view: statusControlSetting.View,
                     columnHash: statusControlSetting.ColumnHash,
                     permissions: ParsePermissions(
                         apiSettingPermission: statusControlSetting.Permission,
                         ss: siteSetting)));
                }
            });
            if (deleteSelected.Count() != 0)
            {
                siteSetting.StatusControls.Delete(deleteSelected);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteSettings(Context context, ResponseCollection res)
        {
            var controlId = context.Forms.ControlId();
            switch (controlId)
            {
                case "OpenGridColumnDialog":
                    OpenGridColumnDialog(
                        context: context,
                        res: res);
                    break;
                case "SetGridColumn":
                    SetGridColumn(
                        context: context,
                        res: res);
                    break;
                case "GridJoin":
                    SetGridColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "OpenFilterColumnDialog":
                    OpenFilterColumnDialog(
                        context: context,
                        res: res);
                    break;
                case "SetFilterColumn":
                    SetFilterColumn(
                        context: context,
                        res: res);
                    break;
                case "FilterJoin":
                    SetFilterColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "OpenAggregationDetailsDialog":
                    OpenAggregationDetailsDialog(
                        context: context,
                        res: res);
                    break;
                case "AddAggregations":
                case "DeleteAggregations":
                    SetAggregations(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "SetAggregationDetails":
                    SetAggregationDetails(
                        context: context,
                        res: res);
                    break;
                case "OpenEditorColumnDialog":
                    OpenEditorColumnDialog(
                        context: context,
                        res: res);
                    break;
                case "OpenEditorOtherColumnDialog":
                    OpenEditorColumnDialog(
                        context: context,
                        res: res,
                        editorOtherColumn: true);
                    break;
                case "SetEditorColumn":
                    SetEditorColumn(
                        context: context,
                        res: res);
                    break;
                case "ResetEditorColumn":
                    ResetEditorColumn(
                        context: context,
                        res: res);
                    break;
                case "ToDisableEditorColumns":
                    DeleteEditorColumns(
                        context: context,
                        res: res);
                    break;
                case "EditorColumnsTabs":
                    SetEditorColumnsTabsSelectable(
                        context: context,
                        res: res);
                    break;
                case "EditorSourceColumnsType":
                    SetEditorSourceColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "MoveUpTabs":
                case "MoveDownTabs":
                    SetTabsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewTabDialog":
                case "EditTabDialog":
                    OpenTabDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddTab":
                    AddTab(
                        context: context,
                        res: res);
                    break;
                case "UpdateTab":
                    UpdateTab(
                        context: context,
                        res: res);
                    break;
                case "DeleteTabs":
                    DeleteTabs(
                        context: context,
                        res: res);
                    break;
                case "ToEnableEditorColumns":
                    AddEditorColumns(
                        context: context,
                        res: res);
                    break;
                case "UpdateSection":
                    UpdateSection(
                        context: context,
                        res: res);
                    break;
                case "UpdateLink":
                    UpdateLink(
                        context: context,
                        res: res);
                    break;
                case "MoveUpSummaries":
                case "MoveDownSummaries":
                    SetSummariesOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewSummary":
                case "EditSummary":
                    OpenSummaryDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "SummarySiteId":
                    SetSummarySiteId(
                        context: context,
                        res: res);
                    break;
                case "SummaryType":
                    SetSummaryType(
                        context: context,
                        res: res);
                    break;
                case "AddSummary":
                    AddSummary(
                        context: context,
                        res: res);
                    break;
                case "UpdateSummary":
                    UpdateSummary(
                        context: context,
                        res: res);
                    break;
                case "CopySummaries":
                    CopySummaries(
                        context: context,
                        res: res);
                    break;
                case "DeleteSummaries":
                    DeleteSummaries(
                        context: context,
                        res: res);
                    break;
                case "MoveUpFormulas":
                case "MoveDownFormulas":
                    SetFormulasOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewFormula":
                case "EditFormula":
                    OpenFormulaDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddFormula":
                    AddFormula(
                        context: context,
                        res: res);
                    break;
                case "UpdateFormula":
                    UpdateFormula(
                        context: context,
                        res: res);
                    break;
                case "CopyFormulas":
                    CopyFormulas(
                        context: context,
                        res: res);
                    break;
                case "DeleteFormulas":
                    DeleteFormulas(
                        context: context,
                        res: res);
                    break;
                case "FormulaCalculationMethod":
                    ChangeFormulaCalculationMethod(
                        context: context,
                        res: res);
                    break;
                case "MoveUpProcesses":
                case "MoveDownProcesses":
                    SetProcessesOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewProcess":
                case "EditProcess":
                    OpenProcessDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddProcess":
                    AddProcess(
                        context: context,
                        res: res);
                    break;
                case "UpdateProcess":
                    UpdateProcess(
                        context: context,
                        res: res);
                    break;
                case "CopyProcesses":
                    CopyProcesses(
                        context: context,
                        res: res);
                    break;
                case "DeleteProcesses":
                    DeleteProcess(
                        context: context,
                        res: res);
                    break;
                case "MoveUpProcessValidateInputs":
                case "MoveDownProcessValidateInputs":
                    SetProcessValidateInputsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewProcessValidateInput":
                case "EditProcessValidateInput":
                    OpenProcessValidateInputDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddProcessValidateInput":
                    AddProcessValidateInput(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateProcessValidateInput":
                    UpdateProcessValidateInput(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteProcessValidateInputs":
                    DeleteProcessValidateInputs(
                        context: context,
                        res: res);
                    break;
                case "AddProcessViewFilter":
                    AddViewFilter(
                        context: context,
                        res: res,
                        prefix: "Process");
                    break;
                case "SearchProcessAccessControl":
                    SearchProcessAccessControl(
                        context: context,
                        res: res);
                    break;
                case "MoveUpProcessDataChanges":
                case "MoveDownProcessDataChanges":
                    SetProcessDataChangesOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewProcessDataChange":
                case "EditProcessDataChange":
                    OpenProcessDataChangeDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddProcessDataChange":
                    AddProcessDataChange(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateProcessDataChange":
                    UpdateProcessDataChange(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyProcessDataChanges":
                    CopyProcessDataChanges(
                        context: context,
                        res: res);
                    break;
                case "DeleteProcessDataChanges":
                    DeleteProcessDataChanges(
                        context: context,
                        res: res);
                    break;
                case "MoveUpProcessNotifications":
                case "MoveDownProcessNotifications":
                    SetProcessNotificationsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewProcessNotification":
                case "EditProcessNotification":
                    OpenProcessNotificationDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddProcessNotification":
                    AddProcessNotification(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateProcessNotification":
                    UpdateProcessNotification(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteProcessNotifications":
                    DeleteProcessNotifications(
                        context: context,
                        res: res);
                    break;
                case "MoveUpStatusControls":
                case "MoveDownStatusControls":
                    SetStatusControlsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewStatusControl":
                case "EditStatusControl":
                    OpenStatusControlDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddStatusControl":
                    AddStatusControl(
                        context: context,
                        res: res);
                    break;
                case "UpdateStatusControl":
                    UpdateStatusControl(
                        context: context,
                        res: res);
                    break;
                case "CopyStatusControls":
                    CopyStatusControls(
                        context: context,
                        res: res);
                    break;
                case "DeleteStatusControls":
                    DeleteStatusControl(
                        context: context,
                        res: res);
                    break;
                case "AddStatusControlViewFilter":
                    AddViewFilter(
                        context: context,
                        res: res,
                        prefix: "StatusControl");
                    break;
                case "SearchStatusControlAccessControl":
                    SearchStatusControlAccessControl(
                        context: context,
                        res: res);
                    break;
                case "NewView":
                case "EditView":
                    OpenViewDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddViewFilter":
                    AddViewFilter(
                        context: context,
                        res: res);
                    break;
                case "SearchViewAccessControl":
                    SearchViewAccessControl(
                        context: context,
                        res: res);
                    break;
                case "AddView":
                    AddView(
                        context: context,
                        res: res);
                    break;
                case "UpdateView":
                    UpdateView(
                        context: context,
                        res: res);
                    break;
                case "CopyViews":
                    CopyViews(
                        context: context,
                        res: res);
                    break;
                case "DeleteViews":
                    DeleteViews(
                        context: context,
                        res: res);
                    break;
                case "ViewGridJoin":
                    SetViewGridColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "ViewFiltersFilterJoin":
                    SetFilterColumnsSelectable(
                        context: context,
                        res: res,
                        prefix: "ViewFilters");
                    break;
                case "MoveUpNotifications":
                case "MoveDownNotifications":
                    SetNotificationsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewNotification":
                case "EditNotification":
                    OpenNotificationDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddNotification":
                    AddNotification(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateNotification":
                    UpdateNotification(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyNotifications":
                    CopyNotifications(
                        context: context,
                        res: res);
                    break;
                case "DeleteNotifications":
                    DeleteNotifications(
                        context: context,
                        res: res);
                    break;
                case "NewReminder":
                case "EditReminder":
                    OpenReminderDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddReminder":
                    AddReminder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateReminder":
                    UpdateReminder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyReminders":
                    CopyReminders(
                        context: context,
                        res: res);
                    break;
                case "DeleteReminders":
                    DeleteReminders(
                        context: context,
                        res: res);
                    break;
                case "MoveUpReminders":
                case "MoveDownReminders":
                    SetRemindersOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "TestReminders":
                    TestReminders(
                        context: context,
                        res: res);
                    break;
                case "MoveUpExports":
                case "MoveDownExports":
                    SetExportsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewExport":
                case "EditExport":
                    OpenExportDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddExport":
                    AddExport(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateExport":
                    UpdateExport(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyExports":
                    CopyExports(
                        context: context,
                        res: res);
                    break;
                case "DeleteExports":
                    DeleteExports(
                        context: context,
                        res: res);
                    break;
                case "ExportJoin":
                    SetExportColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                case "OpenExportColumnsDialog":
                    OpenExportColumnsDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateExportColumn":
                    UpdateExportColumns(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "SearchExportAccessControl":
                    SearchExportAccessControl(
                        context: context,
                        res: res);
                    break;
                case "MoveUpStyles":
                case "MoveDownStyles":
                    SetStylesOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewStyle":
                case "EditStyle":
                    OpenStyleDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddStyle":
                    AddStyle(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateStyle":
                    UpdateStyle(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyStyles":
                    CopyStyles(
                        context: context,
                        res: res);
                    break;
                case "DeleteStyles":
                    DeleteStyles(
                        context: context,
                        res: res);
                    break;
                case "MoveUpScripts":
                case "MoveDownScripts":
                    SetScriptsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewScript":
                case "EditScript":
                    OpenScriptDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddScript":
                    AddScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateScript":
                    UpdateScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyScripts":
                    CopyScripts(
                        context: context,
                        res: res);
                    break;
                case "DeleteScripts":
                    DeleteScripts(
                        context: context,
                        res: res);
                    break;
                case "MoveUpHtmls":
                case "MoveDownHtmls":
                    SetHtmlsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewHtml":
                case "EditHtml":
                    OpenHtmlDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddHtml":
                    AddHtml(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateHtml":
                    UpdateHtml(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyHtmls":
                    CopyHtmls(
                        context: context,
                        res: res);
                    break;
                case "DeleteHtmls":
                    DeleteHtmls(
                        context: context,
                        res: res);
                    break;
                case "MoveUpServerScripts":
                case "MoveDownServerScripts":
                    SetServerScriptsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewServerScript":
                case "EditServerScript":
                    OpenServerScriptDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddServerScript":
                    AddServerScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateServerScript":
                    UpdateServerScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyServerScripts":
                    CopyServerScripts(
                        context: context,
                        res: res);
                    break;
                case "DeleteServerScripts":
                    DeleteServerScripts(
                        context: context,
                        res: res);
                    break;
                case "MoveUpBulkUpdateColumns":
                case "MoveDownBulkUpdateColumns":
                    SetBulkUpdateColumnsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewBulkUpdateColumn":
                case "EditBulkUpdateColumns":
                    OpenBulkUpdateColumnDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "OpenBulkUpdateColumnDetailDialog":
                    OpenBulkUpdateColumnDetailDialog(
                        context: context,
                        res: res);
                    break;
                case "AddBulkUpdateColumn":
                    AddBulkUpdateColumn(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateBulkUpdateColumn":
                    UpdateBulkUpdateColumn(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyBulkUpdateColumns":
                    CopyBulkUpdateColumns(
                        context: context,
                        res: res);
                    break;
                case "DeleteBulkUpdateColumns":
                    DeleteBulkUpdateColumns(
                        context: context,
                        res: res);
                    break;
                case "UpdateBulkUpdateColumnDetail":
                    UpdateBulkUpdateColumnDetail(
                        context: context,
                        res: res);
                    break;
                case "MoveUpRelatingColumns":
                case "MoveDownRelatingColumns":
                    SetRelatingColumnsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewRelatingColumn":
                case "EditRelatingColumns":
                    OpenRelatingColumnDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddRelatingColumn":
                    AddRelatingColumn(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateRelatingColumn":
                    UpdateRelatingColumn(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "DeleteRelatingColumns":
                    DeleteRelatingColumns(
                        context: context,
                        res: res);
                    break;
                case "MoveUpDashboardParts":
                case "MoveDownDashboardParts":
                    SetDashboardPartsOrder(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "NewDashboardPart":
                case "EditDashboardPart":
                    OpenDashboardPartDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddDashboardPart":
                    AddDashboardPart(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateDashboardPart":
                    UpdateDashboardPart(
                        context: context,
                        res: res);
                    break;
                case "CopyDashboardParts":
                    CopyDashboardPart(
                        context: context,
                        res: res);
                    break;
                case "DeleteDashboardParts":
                    DeleteDashboardPart(
                        context: context,
                        res: res);
                    break;
                case "AddDashboardPartViewFilter":
                    long BaseSiteId = 0;
                    switch ((DashboardPartType)context.Forms.Long("DashboardPartType"))
                    {
                        case DashboardPartType.TimeLine:
                            BaseSiteId = context.Forms.Long("DashboardPartBaseSiteId");
                            break;
                        case DashboardPartType.Calendar:
                            BaseSiteId = context.Forms.Long("DashboardPartCalendarBaseSiteId");
                            break;
                        case DashboardPartType.Kamban:
                            BaseSiteId = context.Forms.Long("DashboardPartKambanBaseSiteId");
                            break;
                        case DashboardPartType.Index:
                            BaseSiteId = context.Forms.Long("DashboardPartIndexBaseSiteId");
                            break;
                    }
                    var ss = SiteSettingsUtilities.Get(
                        context: context,
                        siteId: BaseSiteId);
                    AddViewFilter(
                        context: context,
                        res: res,
                        prefix: "DashboardPart",
                        ss: ss);
                    break;
                case "EditTimeLineSites":
                    OpenDashboardPartTimeLineSitesDialog(
                        context: context,
                        res: res);
                    break;
                case "UpdateDashboardPartTimeLineSites":
                    UpdateDashboardPartTimeLineSites(
                        context: context,
                        res: res);
                    break;
                case "EditCalendarSites":
                    OpenDashboardPartCalendarSitesDialog(
                        context: context,
                        res: res);
                    break;
                case "UpdateDashboardPartCalendarSites":
                    UpdateDashboardPartCalendarSites(
                        context: context,
                        res: res);
                    break;
                case "EditKambanSites":
                    OpenDashboardPartKambanSitesDialog(
                        context: context,
                        res: res);
                    break;
                case "UpdateDashboardPartKambanSites":
                    UpdateDashboardPartKambanSites(
                        context: context,
                        res: res);
                    break;
                case "EditIndexSites":
                    OpenDashboardPartIndexSitesDialog(
                        context: context,
                        res: res);
                    break;
                case "UpdateDashboardPartIndexSites":
                    UpdateDashboardPartIndexSites(
                        context: context,
                        res: res);
                    break;
                case "ClearDashboardView":
                    ClearDashboardView(
                        context: context,
                        res: res);
                    break;
                case "ClearDashboardCalendarView":
                    ClearDashboardCalendarView(
                        context: context,
                        res: res);
                    break;
                case "ClearDashboardKambanView":
                    ClearDashboardKambanView(
                        context: context,
                        res: res);
                    break;
                case "ClearDashboardIndexView":
                    ClearDashboardIndexView(
                        context: context,
                        res: res);
                    break;
                case "UpdateDashboardPartLayouts":
                    UpdatedashboardPartLayouts(context: context);
                    break;
                case "SearchDashboardPartAccessControl":
                    SearchDashboardPartAccessControl(
                        context: context,
                        res: res);
                    break;
                case "OpenSearchEditorColumnDialog":
                    OpenSearchEditorColumnDialog(
                        context: context,
                        res: res);
                    break;
                case "SearchEditorColumnDialogInput":
                    FilterSourceColumnsSelectable(
                        context: context,
                        res: res);
                    break;
                default:
                    if (controlId.Contains("_NumericRange"))
                    {
                        OpenSetNumericRangeDialog(
                            context: context,
                            res: res,
                            controlId: controlId);
                    }
                    else if (controlId.Contains("_DateRange"))
                    {
                        OpenSetDateRangeDialog(
                            context: context,
                            res: res,
                            controlId: controlId);
                    }
                    else
                    {
                        context.Forms
                            .Where(o => o.Key != controlId)
                            .ForEach(data =>
                                SiteSettings.Set(
                                    context: context,
                                    propertyName: data.Key,
                                    value: data.Value));
                    }
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder ReplaceSiteMenu(Context context, long sourceId, long destinationId)
        {
            var siteMenu = SiteInfo.TenantCaches.Get(TenantId).SiteMenu;
            return new HtmlBuilder().SiteMenu(
                context: context,
                ss: SiteSettings,
                currentSs: null,
                siteId: destinationId,
                referenceType: ReferenceType,
                title: siteMenu.Get(destinationId).Title,
                siteConditions: siteMenu.SiteConditions(context: context, ss: SiteSettings));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenGridColumnDialog(Context context, ResponseCollection res)
        {
            var selectedColumns = context.Forms.List("GridColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest(context: context));
                }
                else if (column.Joined)
                {
                    res.Message(Messages.CanNotPerformed(context: context));
                }
                else
                {
                    SiteSettings.GridColumns = context.Forms.List("GridColumnsAll");
                    res.Html(
                        "#GridColumnDialog",
                        SiteUtilities.GridColumnDialog(
                            context: context,
                            ss: SiteSettings,
                            column: column));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetGridColumn(Context context, ResponseCollection res)
        {
            var columnName = context.Forms.Data("GridColumnName");
            var column = SiteSettings.GridColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                context.Forms.ForEach(control => SiteSettings.SetColumnProperty(
                    context: context,
                    column: column,
                    propertyName: control.Key,
                    value: GridColumnValue(
                        context: context,
                        name: control.Key,
                        value: control.Value)));
                res
                    .Html("#GridColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.GridSelectableOptions(context: context),
                        selectedValueTextCollection: columnName.ToSingleList()))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string GridColumnValue(Context context, string name, string value)
        {
            switch (name)
            {
                case "GridDesign":
                    return context.Forms.Bool("UseGridDesign")
                        ? value
                        : null;
                default:
                    return value;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetGridColumnsSelectable(Context context, ResponseCollection res)
        {
            SiteSettings.GridColumns = context.Forms.List("GridColumnsAll");
            var listItemCollection = SiteSettings.GridSelectableOptions(
                context: context, enabled: false, join: context.Forms.Data("GridJoin"));
            if (!listItemCollection.Any())
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                res.Html("#GridSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: listItemCollection));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenFilterColumnDialog(Context context, ResponseCollection res)
        {
            var selectedColumns = context.Forms.List("FilterColumns");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: selectedColumns.FirstOrDefault());
                if (column == null)
                {
                    res.Message(Messages.InvalidRequest(context: context));
                }
                else if (column.Joined)
                {
                    res.Message(Messages.CanNotPerformed(context: context));
                }
                {
                    SiteSettings.FilterColumns = context.Forms.List("FilterColumnsAll");
                    res.Html(
                        "#FilterColumnDialog",
                        SiteUtilities.FilterColumnDialog(
                            context: context,
                            ss: SiteSettings,
                            column: column));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFilterColumn(Context context, ResponseCollection res)
        {
            var columnName = context.Forms.Data("FilterColumnName");
            var column = SiteSettings.FilterColumn(columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                context.Forms.ForEach(control => SiteSettings.SetColumnProperty(
                    context: context,
                    column: column,
                    propertyName: control.Key,
                    value: control.Value));
                res
                    .Html("#FilterColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.FilterSelectableOptions(context: context),
                        selectedValueTextCollection: columnName.ToSingleList()))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFilterColumnsSelectable(
            Context context,
            ResponseCollection res,
            string prefix = "")
        {
            SiteSettings.FilterColumns = context.Forms.List($"{prefix}FilterColumnsAll");
            var listItemCollection = SiteSettings.FilterSelectableOptions(
                context: context, enabled: false, join: context.Forms.Data($"{prefix}FilterJoin"));
            if (!listItemCollection.Any())
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                res.Html($"#{prefix}FilterSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: listItemCollection));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenAggregationDetailsDialog(Context context, ResponseCollection res)
        {
            var selectedColumns = context.Forms.List("AggregationDestination");
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var aggregation = SiteSettings.Aggregations
                    .FirstOrDefault(o => o.Id == selectedColumns.First().ToLong());
                if (aggregation == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    res.Html(
                        "#AggregationDetailsDialog",
                        new HtmlBuilder().AggregationDetailsDialog(
                            context: context,
                            ss: SiteSettings,
                            aggregation: aggregation));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetAggregations(Context context, ResponseCollection res, string controlId)
        {
            var selectedColumns = context.Forms.List("AggregationDestination");
            var selectedSourceColumns = context.Forms.List("AggregationSource");
            if (selectedColumns.Any() || selectedSourceColumns.Any())
            {
                SiteSettings.Aggregations = SiteSettings.GetAggregations(context: context);
                selectedColumns = SiteSettings.SetAggregations(
                    controlId,
                    selectedColumns,
                    selectedSourceColumns);
                res
                    .Html("#AggregationDestination", new HtmlBuilder()
                        .SelectableItems(
                            listItemCollection: SiteSettings
                                .AggregationDestination(context: context),
                            selectedValueTextCollection: selectedColumns))
                    .SetFormData("AggregationDestination", selectedColumns?.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetAggregationDetails(Context context, ResponseCollection res)
        {
            Aggregation.Types type;
            Enum.TryParse(context.Forms.Data("AggregationType"), out type);
            var target = type != Aggregation.Types.Count
                ? context.Forms.Data("AggregationTarget")
                : string.Empty;
            var id = context.Forms.Long("SelectedAggregation");
            var aggregation = SiteSettings.Aggregations
                .FirstOrDefault(o => o.Id == id);
            if (aggregation == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                aggregation.Type = type;
                aggregation.Target = type != Aggregation.Types.Count
                    ? target
                    : null;
                res
                    .Html("#AggregationDestination", new HtmlBuilder()
                        .SelectableItems(
                            listItemCollection: SiteSettings
                                .AggregationDestination(context: context),
                            selectedValueTextCollection: id.ToString().ToSingleList()))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenEditorColumnDialog(
            Context context,
            ResponseCollection res,
            bool editorOtherColumn = false)
        {
            var selectedColumns = !editorOtherColumn
                ? context.Forms.List("EditorColumns")
                : context.Forms.Data("EditorOtherColumn").ToSingleList();
            if (selectedColumns.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: selectedColumns.FirstOrDefault());
                var section = SiteSettings.Sections.Get(SiteSettings.SectionId(selectedColumns
                    .FirstOrDefault()));
                var linkId = SiteSettings.LinkId(selectedColumns.FirstOrDefault());
                if (column == null && section == null && linkId == 0)
                {
                    res.Message(Messages.InvalidRequest(context: context));
                }
                else
                {
                    var titleColumns = SiteSettings.TitleColumns;
                    if (column?.ColumnName == "Title")
                    {
                        Session_TitleColumns(
                            context: context,
                            value: titleColumns.ToJson());
                    }
                    AddOrUpdateEditorColumnHash(context: context);
                    if (column != null)
                    {
                        res.Html(
                            "#EditorColumnDialog",
                            SiteUtilities.EditorColumnDialog(
                                context: context,
                                ss: SiteSettings,
                                column: column,
                                titleColumns: titleColumns));
                    }
                    else if (section != null)
                    {
                        res.Html("#EditorColumnDialog", SiteUtilities.SectionDialog(
                            context: context,
                            ss: SiteSettings,
                            controlId: context.Forms.ControlId(),
                            section: section));
                    }
                    else if (linkId != 0)
                    {
                        res.Html("#EditorColumnDialog", SiteUtilities.LinkDialog(
                            context: context,
                            ss: SiteSettings,
                            controlId: context.Forms.ControlId()));
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumn(Context context, ResponseCollection res)
        {
            var columnName = context.Forms.Data("EditorColumnName");
            var column = SiteSettings.GetColumn(
                context: context,
                columnName: columnName);
            if (column == null)
            {
                res.Message(Messages.InvalidRequest(context: context));
            }
            else
            {
                if (context.Forms.Bool("ResetEditorColumnData"))
                {
                    column = SiteSettings.ResetColumn(
                        context: context,
                        columnName: column.ColumnName);
                }
                var editorOtherColumn = false;
                switch (column.ColumnName)
                {
                    case "Creator":
                    case "Updator":
                    case "CreatedTime":
                    case "UpdatedTime":
                        editorOtherColumn = true;
                        break;
                    case "Title":
                        SiteSettings.TitleColumns = context.Forms.List("TitleColumnsAll");
                        break;
                }
                context.Forms.ForEach(control =>
                    SiteSettings.SetColumnProperty(
                        context: context,
                        column: column,
                        propertyName: control.Key,
                        value: control.Value));
                if (!editorOtherColumn)
                {
                    res.Html("#EditorColumns", new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings.EditorSelectableOptions(context: context),
                        selectedValueTextCollection: columnName.ToSingleList()));
                }
                else
                {
                    res.Html("#EditorOtherColumn", new HtmlBuilder().EditorOtherColumn(
                        context: context,
                        ss: SiteSettings,
                        selectedValue: column.ColumnName));
                }
                res.CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ResetEditorColumn(Context context, ResponseCollection res)
        {
            var ss = new SiteSettings(
                context: context,
                referenceType: ReferenceType);
            var column = ss.GetColumn(
                context: context,
                columnName: context.Forms.Data("EditorColumnName"));
            res
                .Html(
                    "#EditorColumnDialog",
                    SiteUtilities.EditorColumnDialog(
                        context: context,
                        ss: SiteSettings,
                        column: column,
                        titleColumns: ss.TitleColumns))
                .Val("#ResetEditorColumnData", "1");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddOrUpdateEditorColumnHash(Context context)
        {
            SiteSettings
                .AddOrUpdateEditorColumnHash(
                    editorColumnsAll: context.Forms.List("EditorColumnsAll"),
                    editorColumnsTabsTarget: context
                        .Forms
                        .Data("EditorColumnsTabsTarget"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteEditorColumns(Context context, ResponseCollection res)
        {
            AddOrUpdateEditorColumnHash(context: context);
            var selected = context.Forms.List("EditorColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.EditorColumnHash?.ForEach(o => o
                    .Value
                    ?.RemoveAll(columnName => selected.Contains(columnName)));
                res.EditorColumnsResponses(
                    context: context,
                    ss: SiteSettings);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorColumnsTabsSelectable(Context context, ResponseCollection res)
        {
            AddOrUpdateEditorColumnHash(context: context);
            res.Html("#EditorColumns", new HtmlBuilder().SelectableItems(
                listItemCollection: SiteSettings.EditorSelectableOptions(
                    context: context,
                    tabId: context
                        .Forms
                        .Data(key: "EditorColumnsTabs")
                        .ToInt())))
                .Val(
                    "#EditorColumnsTabsTarget",
                    context.Forms.Data("EditorColumnsTabs"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetEditorSourceColumnsSelectable(Context context, ResponseCollection res)
        {
            AddOrUpdateEditorColumnHash(context: context);
            res.EditorSourceColumnsResponses(
                context: context,
                ss: SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetTabsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("Tabs");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Tabs?.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res
                    .TabResponses(
                        context: context,
                        ss: SiteSettings,
                        selected: selected)
                    .EditorColumnsResponses(
                        context: context,
                        ss: SiteSettings);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenTabDialog(Context context, ResponseCollection res, string controlId)
        {
            Libraries.Settings.Tab tab;
            if (controlId == "NewTabDialog")
            {
                OpenTabDialog(context: context, res: res, tab: null);
            }
            else
            {
                var idList = context.Forms.IntList("Tabs");
                if (idList.Count() != 1)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    tab = idList.First() == 0
                        ? new Tab
                        {
                            Id = 0,
                            LabelText = SiteSettings.GeneralTabLabelText
                        }
                        : SiteSettings.Tabs?.Get(idList.First());
                    if (tab == null)
                    {
                        OpenDialogError(res, Messages.SelectOne(context: context));
                    }
                    else
                    {
                        SiteSettings.Tabs = SiteSettings.Tabs?.Join(
                            context
                                .Forms
                                .List("TabsAll")
                                .Select((val, key) => new
                                {
                                    Key = key,
                                    Val = val
                                }),
                            v => v.Id, l => l.Val.ToInt(),
                            (v, l) => new { Tabs = v, OrderNo = l.Key })
                                .OrderBy(v => v.OrderNo)
                                .Select(v => v.Tabs)
                                .Aggregate(
                                    new SettingList<Tab>(),
                                    (list, data) => { list.Add(data); return list; });
                        OpenTabDialog(context: context, res: res, tab: tab);
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenTabDialog(
            Context context,
            ResponseCollection res,
            Libraries.Settings.Tab tab)
        {
            AddOrUpdateEditorColumnHash(context: context);
            SiteSettings.SetChoiceHash(context: context);
            res.Html("#TabDialog", SiteUtilities.Tab(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                tab: tab));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddTab(Context context, ResponseCollection res)
        {
            SiteSettings
                .AddTab(new Libraries.Settings.Tab(
                    context: context,
                    ss: SiteSettings));
            res
                .TabResponses(
                    context: context,
                    ss: SiteSettings,
                    selected: new List<int>
                    {
                        SiteSettings.TabLatestId.ToInt()
                    })
                .CloseDialog()
                .EditorColumnsResponses(
                    context: context,
                    ss: SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateTab(Context context, ResponseCollection res)
        {
            var selected = context.Forms.Int("TabId");
            var tab = SiteSettings.Tabs?.Get(selected);
            if (selected == 0)
            {
                SiteSettings.GeneralTabLabelText = context.Forms.Data("LabelText");
                res
                    .TabResponses(
                        context: context,
                        ss: SiteSettings,
                        selected: new List<int> { selected })
                    .CloseDialog()
                    .EditorColumnsResponses(
                        context: context,
                        ss: SiteSettings);
            }
            else if (tab == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                tab.SetByForm(
                    context: context,
                    ss: SiteSettings);
                res
                    .TabResponses(
                        context: context,
                        ss: SiteSettings,
                        selected: new List<int> { selected })
                    .CloseDialog()
                    .EditorColumnsResponses(
                        context: context,
                        ss: SiteSettings);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteTabs(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("Tabs");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else if (selected.Contains(0))
            {
                res.Message(Messages.CanNotDelete(
                    context: context,
                    Displays.General(context: context))).ToJson();
            }
            else
            {
                SiteSettings.Tabs?.RemoveAll(o => selected.Contains(o.Id));
                SiteSettings.EditorColumnHash?.RemoveAll((key, value) => SiteSettings
                    .TabId(key) != 0
                        && selected.Contains(SiteSettings.TabId(key)));
                res
                    .TabResponses(
                        context: context,
                        ss: SiteSettings)
                    .EditorColumnsResponses(
                        context: context,
                        ss: SiteSettings);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddEditorColumns(Context context, ResponseCollection res)
        {
            switch (context.Forms.Data("EditorSourceColumnsType"))
            {
                case "Others":
                    if (context.Forms.List("EditorSourceColumns")?.FirstOrDefault()?.StartsWith("_Section-") == true)
                    {
                        var sectionName = SiteSettings.SectionName(SiteSettings.AddSection(new Section
                        {
                            LabelText = Displays.Section(context: context)
                        }).Id);
                        var tab = SiteSettings
                            .EditorColumnHash
                            .Get(SiteSettings.TabName(context.Forms.Int("EditorColumnsTabsTarget")));
                        if (tab == null)
                        {
                            tab = new List<string>();
                            SiteSettings.AddOrUpdateEditorColumnHash(
                                editorColumnsAll: tab,
                                editorColumnsTabsTarget: context
                                    .Forms
                                    .Int("EditorColumnsTabsTarget")
                                    .ToStr());
                        }
                        tab.Add(sectionName);
                        res.Html(
                            "#EditorColumns",
                            new HtmlBuilder().SelectableItems(
                                listItemCollection: SiteSettings
                                    .EditorSelectableOptions(
                                        context: context,
                                        tabId: context.Forms.Int("EditorColumnsTabs")),
                                selectedValueTextCollection: new List<string> { sectionName }));
                    }
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateSection(Context context, ResponseCollection res)
        {
            var selected = context.Forms.Int("SectionId");
            var section = SiteSettings.Sections.Get(selected);
            var sectionName = SiteSettings.SectionName(section?.Id);
            if (section == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                section.SetByForm(
                    context: context,
                    ss: SiteSettings);
                res.Html(
                    "#EditorColumns",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: SiteSettings
                            .EditorSelectableOptions(
                                context: context,
                                tabId: SiteSettings
                                    .TabId(SiteSettings
                                        .EditorColumnHash
                                        .Where(o => o
                                            .Value?
                                            .Contains(sectionName) == true)
                                        .Select(o => o.Key)
                                        .FirstOrDefault()))))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateLink(Context context, ResponseCollection res)
        {
            res.CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummariesOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditSummary");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Summaries.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditSummary", new HtmlBuilder()
                    .EditSummary(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenSummaryDialog(Context context, ResponseCollection res, string controlId)
        {
            if (SiteSettings.Destinations?.Any() != true)
            {
                res.Message(Messages.NoLinks(context: context));
            }
            else
            {
                if (controlId == "NewSummary")
                {
                    OpenSummaryDialog(
                        context: context,
                        res: res,
                        summary: new Summary(SiteSettings
                            .Destinations
                            .Values
                            .FirstOrDefault()
                            .SiteId));
                }
                else
                {
                    var summary = SiteSettings.Summaries?.Get(context.Forms.Int("SummaryId"));
                    if (summary == null)
                    {
                        OpenDialogError(res, Messages.SelectOne(context: context));
                    }
                    else
                    {
                        SiteSettingsUtilities.Get(
                            context: context, siteModel: this, referenceId: SiteId);
                        OpenSummaryDialog(
                            context: context,
                            res: res,
                            summary: summary);
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenSummaryDialog(Context context, ResponseCollection res, Summary summary)
        {
            res.Html("#SummaryDialog", SiteUtilities.SummaryDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                summary: summary));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummarySiteId(Context context, ResponseCollection res)
        {
            var siteId = context.Forms.Long("SummarySiteId");
            var destinationSiteModel = new SiteModel(context: context, siteId: siteId);
            res
                .ReplaceAll("#SummaryDestinationColumnField", new HtmlBuilder()
                    .SummaryDestinationColumn(
                        context: context,
                        destinationSs: destinationSiteModel.SiteSettings))
                .ReplaceAll("#SummaryLinkColumnField", new HtmlBuilder()
                    .SummaryLinkColumn(
                        context: context,
                        ss: SiteSettings,
                        siteId: siteId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSummaryType(Context context, ResponseCollection res)
        {
            res.ReplaceAll("#SummarySourceColumnField", new HtmlBuilder()
                .SummarySourceColumn(
                    context: context,
                    ss: SiteSettings,
                    type: context.Forms.Data("SummaryType")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddSummary(Context context, ResponseCollection res)
        {
            var siteId = context.Forms.Long("SummarySiteId");
            var destinationSs = SiteSettings.Destinations.Get(siteId);
            int? destinationCondition = context.Forms.Int("SummaryDestinationCondition");
            int? sourceCondition = context.Forms.Int("SummarySourceCondition");
            var error = SiteSettings.AddSummary(
                siteId,
                new SiteModel(context: context, siteId: context.Forms.Long("SummarySiteId")).ReferenceType,
                context.Forms.Data("SummaryDestinationColumn"),
                destinationSs?.Views?.Get(destinationCondition)?.Id,
                context.Forms.Bool("SummarySetZeroWhenOutOfCondition"),
                context.Forms.Data("SummaryLinkColumn"),
                context.Forms.Data("SummaryType"),
                context.Forms.Data("SummarySourceColumn"),
                SiteSettings.Views?.Get(sourceCondition)?.Id);
            if (error.Has())
            {
                res.Message(error.Message(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditSummary", new HtmlBuilder()
                        .EditSummary(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateSummary(Context context, ResponseCollection res)
        {
            var siteId = context.Forms.Long("SummarySiteId");
            var destinationSs = SiteSettings.Destinations.Get(siteId);
            int? destinationCondition = context.Forms.Int("SummaryDestinationCondition");
            int? sourceCondition = context.Forms.Int("SummarySourceCondition");
            var outOfCondition = context.Forms.Data("SummaryOutOfCondition").Trim();
            var error = SiteSettings.UpdateSummary(
                context.Forms.Int("SummaryId"),
                siteId,
                new SiteModel(context: context, siteId: context.Forms.Long("SummarySiteId")).ReferenceType,
                context.Forms.Data("SummaryDestinationColumn"),
                destinationSs?.Views?.Get(destinationCondition)?.Id,
                context.Forms.Bool("SummarySetZeroWhenOutOfCondition"),
                context.Forms.Data("SummaryLinkColumn"),
                context.Forms.Data("SummaryType"),
                context.Forms.Data("SummarySourceColumn"),
                SiteSettings.Views?.Get(sourceCondition)?.Id);
            if (error.Has())
            {
                res.Message(error.Message(context: context));
            }
            else
            {
                res
                    .Html("#EditSummary", new HtmlBuilder()
                        .EditSummary(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopySummaries(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditSummary");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Summaries.Copy(selected);
                res.ReplaceAll("#EditSummary", new HtmlBuilder()
                    .EditSummary(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteSummaries(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditSummary");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Summaries.Delete(selected);
                res.ReplaceAll("#EditSummary", new HtmlBuilder()
                    .EditSummary(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetFormulasOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Formulas.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditFormula", new HtmlBuilder()
                    .EditFormula(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenFormulaDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewFormula")
            {
                var formulaSet = new FormulaSet();
                OpenFormulaDialog(
                    context: context,
                    res: res,
                    formulaSet: formulaSet);
            }
            else
            {
                var formulaSet = SiteSettings.Formulas?.Get(context.Forms.Int("FormulaId"));
                if (formulaSet == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenFormulaDialog(
                        context: context,
                        res: res,
                        formulaSet: formulaSet);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenFormulaDialog(
            Context context, ResponseCollection res, FormulaSet formulaSet)
        {
            res.Html("#FormulaDialog", SiteUtilities.FormulaDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                formulaSet: formulaSet));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddFormula(Context context, ResponseCollection res)
        {
            var outOfCondition = context.Forms.Data("FormulaOutOfCondition").Trim();
            var error = SiteSettings.AddFormula(
                context.Forms.Data("FormulaCalculationMethod"),
                context.Forms.Data("FormulaTarget"),
                context.Forms.Int("FormulaCondition"),
                context.Forms.Data("Formula"),
                context.Forms.Bool("NotUseDisplayName"),
                context.Forms.Bool("IsDisplayError"),
                outOfCondition != string.Empty
                    ? outOfCondition
                    : null);
            if (error.Has())
            {
                res.Message(error.Message(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditFormula", new HtmlBuilder()
                        .EditFormula(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateFormula(Context context, ResponseCollection res)
        {
            var id = context.Forms.Int("FormulaId");
            var outOfCondition = context.Forms.Data("FormulaOutOfCondition").Trim();
            var error = SiteSettings.UpdateFormula(
                id,
                context.Forms.Data("FormulaCalculationMethod"),
                context.Forms.Data("FormulaTarget"),
                context.Forms.Int("FormulaCondition"),
                context.Forms.Data("Formula"),
                context.Forms.Bool("NotUseDisplayName"),
                context.Forms.Bool("IsDisplayError"),
                outOfCondition != string.Empty
                    ? outOfCondition
                    : null);
            if (error.Has())
            {
                res.Message(error.Message(context: context));
            }
            else
            {
                res
                    .Html("#EditFormula", new HtmlBuilder()
                        .EditFormula(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyFormulas(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Formulas.Copy(selected);
                res.ReplaceAll("#EditFormula", new HtmlBuilder()
                    .EditFormula(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteFormulas(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Formulas.Delete(selected);
                res.ReplaceAll("#EditFormula", new HtmlBuilder()
                    .EditFormula(context: context, ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetProcessesOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditProcess");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Processes.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                SiteSettings.GetColumn(context, "Status").SetChoiceHash(context, SiteSettings.SiteId);
                res.Html("#EditProcess", new HtmlBuilder()
                    .EditProcess(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenProcessDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewProcess")
            {
                var process = new Process();
                OpenProcessDialog(
                    context: context,
                    res: res,
                    process: process);
            }
            else
            {
                var process = SiteSettings.Processes?.Get(context.Forms.Int("ProcessId"));
                if (process == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context,
                        siteModel: this,
                        referenceId: SiteId);
                    OpenProcessDialog(
                        context: context,
                        res: res,
                        process: process);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenProcessDialog(
            Context context, ResponseCollection res, Process process)
        {
            res.Html("#ProcessDialog", SiteUtilities.ProcessDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                process: process));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddProcess(Context context, ResponseCollection res)
        {
            SiteSettings.SetChoiceHash(context: context);
            var process = new Process(
                id: SiteSettings.Processes.MaxOrDefault(o => o.Id) + 1,
                name: context.Forms.Data("ProcessName"),
                displayName: context.Forms.Data("ProcessDisplayName"),
                screenType: (Process.ScreenTypes)context.Forms.Int("ProcessScreenType"),
                currentStatus: context.Forms.Int("ProcessCurrentStatus"),
                changedStatus: context.Forms.Int("ProcessChangedStatus"),
                description: context.Forms.Data("ProcessDescription"),
                tooltip: context.Forms.Data("ProcessTooltip"),
                confirmationMessage: context.Forms.Data("ProcessConfirmationMessage"),
                successMessage: SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessSuccessMessage")),
                onClick: context.Forms.Data("ProcessOnClick"),
                executionType: (Process.ExecutionTypes)context.Forms.Int("ProcessExecutionType"),
                actionType: (Process.ActionTypes)context.Forms.Int("ProcessActionType"),
                allowBulkProcessing: context.Forms.Bool("ProcessAllowBulkProcessing"),
                validationType: (Process.ValidationTypes)context.Forms.Int("ProcessValidationType"),
                validateInputs: context.Forms.Data("ProcessValidateInputs").Deserialize<SettingList<ValidateInput>>(),
                permissions: ProcessPermissions(context: context),
                view: new View(
                    context: context,
                    ss: SiteSettings,
                    prefix: "Process"),
                errorMessage: SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessErrorMessage")),
                dataChanges: context.Forms.Data("ProcessDataChanges").Deserialize<SettingList<DataChange>>(),
                autoNumbering: new AutoNumbering()
                {
                    ColumnName = context.Forms.Data("ProcessAutoNumberingColumnName"),
                    Format = SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessAutoNumberingFormat")),
                    ResetType = (Column.AutoNumberingResetTypes)context.Forms.Int("ProcessAutoNumberingResetType"),
                    Default = context.Forms.Int("ProcessAutoNumberingDefault"),
                    Step = context.Forms.Int("ProcessAutoNumberingStep")
                },
                notifications: context.Forms.Data("ProcessNotifications").Deserialize<SettingList<Notification>>());
            SiteSettings.Processes.Add(process);
            res
                .ReplaceAll("#EditProcess", new HtmlBuilder()
                    .EditProcess(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateProcess(Context context, ResponseCollection res)
        {
            var process = SiteSettings.Processes.Get(context.Forms.Int("ProcessId"));
            if (process == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                SiteSettings.SetChoiceHash(context: context);
                var view = process.View ?? new View();
                view.SetByForm(
                    context: context,
                    ss: SiteSettings,
                    prefix: "Process");
                process.Update(
                    name: context.Forms.Data("ProcessName"),
                    displayName: context.Forms.Data("ProcessDisplayName"),
                    screenType: (Process.ScreenTypes)context.Forms.Int("ProcessScreenType"),
                    currentStatus: context.Forms.Int("ProcessCurrentStatus"),
                    changedStatus: context.Forms.Int("ProcessChangedStatus"),
                    description: context.Forms.Data("ProcessDescription"),
                    tooltip: context.Forms.Data("ProcessTooltip"),
                    confirmationMessage: context.Forms.Data("ProcessConfirmationMessage"),
                    successMessage: SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessSuccessMessage")),
                    onClick: context.Forms.Data("ProcessOnClick"),
                    executionType: (Process.ExecutionTypes)context.Forms.Int("ProcessExecutionType"),
                    actionType: (Process.ActionTypes)context.Forms.Int("ProcessActionType"),
                    allowBulkProcessing: context.Forms.Bool("ProcessAllowBulkProcessing"),
                    validationType: (Process.ValidationTypes)context.Forms.Int("ProcessValidationType"),
                    validateInputs: context.Forms.Data("ProcessValidateInputs").Deserialize<SettingList<ValidateInput>>(),
                    permissions: ProcessPermissions(context: context),
                    view: view,
                    errorMessage: SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessErrorMessage")),
                    dataChanges: context.Forms.Data("ProcessDataChanges").Deserialize<SettingList<DataChange>>(),
                    autoNumbering: new AutoNumbering()
                    {
                        ColumnName = context.Forms.Data("ProcessAutoNumberingColumnName"),
                        Format = SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessAutoNumberingFormat")),
                        ResetType = (Column.AutoNumberingResetTypes)context.Forms.Int("ProcessAutoNumberingResetType"),
                        Default = context.Forms.Int("ProcessAutoNumberingDefault"),
                        Step = context.Forms.Int("ProcessAutoNumberingStep")
                    },
                    notifications: context.Forms.Data("ProcessNotifications").Deserialize<SettingList<Notification>>());
                res
                    .ReplaceAll("#EditProcess", new HtmlBuilder()
                        .EditProcess(
                            context: context,
                            ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyProcesses(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditProcess");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Processes.Copy(selected);
                res.ReplaceAll("#EditProcess", new HtmlBuilder()
                    .EditProcess(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteProcess(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditProcess");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Processes.Delete(selected);
                res.ReplaceAll("#EditProcess", new HtmlBuilder()
                    .EditProcess(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetProcessValidateInputsOrder(
            Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditProcessValidateInput");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                var validateInputs = context.Forms.Data("ProcessValidateInputs").Deserialize<SettingList<ValidateInput>>();
                validateInputs.MoveUpOrDown(ColumnUtilities.ChangeCommand(controlId), selected);
                res
                    .Html("#EditProcessValidateInput", new HtmlBuilder()
                        .EditProcessValidateInput(
                            context: context,
                            ss: SiteSettings,
                            validateInputs: validateInputs))
                    .Val(
                        "#ProcessValidateInputs",
                        validateInputs.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenProcessValidateInputDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewProcessValidateInput")
            {
                OpenProcessValidateInputDialog(
                    context: context,
                    res: res,
                    validateInput: new ValidateInput());
            }
            else
            {
                var validateInputs = context.Forms.Data("ProcessValidateInputs").Deserialize<SettingList<ValidateInput>>();
                var validateInput = validateInputs?.Get(context.Forms.Int("ProcessValidateInputId"));
                if (validateInput == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenProcessValidateInputDialog(
                        context: context,
                        res: res,
                        validateInput: validateInput);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenProcessValidateInputDialog(
            Context context, ResponseCollection res, ValidateInput validateInput)
        {
            res
                .Html("#ProcessValidateInputDialog", SiteUtilities.ProcessValidateInputDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: context.Forms.ControlId(),
                    validateInput: validateInput))
                .Invoke(methodName: "setProcessValidateInputDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddProcessValidateInput(Context context, ResponseCollection res, string controlId)
        {
            var validateInputs = context.Forms.Data("ProcessValidateInputsTemp").Deserialize<SettingList<ValidateInput>>()
                ?? new SettingList<ValidateInput>();
            validateInputs.Add(new ValidateInput()
            {
                Id = validateInputs.MaxOrDefault(o => o.Id) + 1,
                ColumnName = context.Forms.Data("ProcessValidateInputColumnName"),
                Required = context.Forms.Bool("ProcessValidateInputRequired"),
                ClientRegexValidation = context.Forms.Data("ProcessValidateInputClientRegexValidation"),
                ServerRegexValidation = context.Forms.Data("ProcessValidateInputServerRegexValidation"),
                RegexValidationMessage = context.Forms.Data("ProcessValidateInputRegexValidationMessage"),
                Min = context.Forms.Decimal(
                    context: context,
                    key: "ProcessValidateInputMin",
                    nullable: true),
                Max = context.Forms.Decimal(
                    context: context,
                    key: "ProcessValidateInputMax",
                    nullable: true)
            });
            res
                .ReplaceAll("#EditProcessValidateInput", new HtmlBuilder()
                    .EditProcessValidateInput(
                        context: context,
                        ss: SiteSettings,
                        validateInputs: validateInputs))
                .Val(
                    "#ProcessValidateInputs",
                    validateInputs.ToJson())
                .CloseDialog(target: "#ProcessValidateInputDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateProcessValidateInput(Context context, ResponseCollection res, string controlId)
        {
            var validateInputs = context.Forms.Data("ProcessValidateInputsTemp").Deserialize<SettingList<ValidateInput>>();
            var validateInput = validateInputs?.Get(context.Forms.Int("ProcessValidateInputIdTemp"));
            validateInput.ColumnName = context.Forms.Data("ProcessValidateInputColumnName");
            validateInput.Required = context.Forms.Bool("ProcessValidateInputRequired");
            validateInput.ClientRegexValidation = context.Forms.Data("ProcessValidateInputClientRegexValidation");
            validateInput.ServerRegexValidation = context.Forms.Data("ProcessValidateInputServerRegexValidation");
            validateInput.RegexValidationMessage = context.Forms.Data("ProcessValidateInputRegexValidationMessage");
            validateInput.Min = context.Forms.Decimal(
                context: context,
                key: "ProcessValidateInputMin",
                nullable: true);
            validateInput.Max = context.Forms.Decimal(
                context: context,
                key: "ProcessValidateInputMax",
                nullable: true);
            res
                .ReplaceAll("#EditProcessValidateInput", new HtmlBuilder()
                    .EditProcessValidateInput(
                        context: context,
                        ss: SiteSettings,
                        validateInputs: validateInputs))
                .Val(
                    "#ProcessValidateInputs",
                    validateInputs.ToJson())
                .CloseDialog(target: "#ProcessValidateInputDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteProcessValidateInputs(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditProcessValidateInput");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                var validateInputs = context.Forms.Data("ProcessValidateInputs").Deserialize<SettingList<ValidateInput>>();
                validateInputs.Delete(selected);
                res
                    .Html("#EditProcessValidateInput", new HtmlBuilder()
                        .EditProcessValidateInput(
                            context: context,
                            ss: SiteSettings,
                            validateInputs: validateInputs))
                    .Val(
                        "#ProcessValidateInputs",
                        validateInputs.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private List<Permission> ProcessPermissions(Context context)
        {
            return context.Forms.List("CurrentProcessAccessControlAll")
                .Select(data => new Permission(
                    name: data.Split_1st(),
                    id: data.Split_2nd().ToInt(),
                    type: Permissions.Types.NotSet))
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SearchProcessAccessControl(Context context, ResponseCollection res)
        {
            var process = SiteSettings.Processes.Get(context.Forms.Int("ProcessId"))
                ?? new Process();
            var currentPermissions = process.GetPermissions(ss: SiteSettings);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: SiteSettings,
                searchText: context.Forms.Data("SearchProcessAccessControl"),
                currentPermissions: currentPermissions,
                allUsers: false);
            return res
                .Html("#SourceProcessAccessControl", PermissionUtilities.PermissionListItem(
                    context: context,
                    ss: SiteSettings,
                    permissions: sourcePermissions.Page(0),
                    selectedValueTextCollection: context.Forms.Data("SourceProcessAccessControl")
                        .Deserialize<List<string>>()?
                        .Where(o => o != string.Empty),
                    withType: false))
                .Val("#SourceProcessAccessControlOffset", Parameters.Permissions.PageSize)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetProcessDataChangesOrder(
            Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditProcessDataChange");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                var dataChanges = context.Forms.Data("ProcessDataChanges").Deserialize<SettingList<DataChange>>();
                dataChanges.MoveUpOrDown(ColumnUtilities.ChangeCommand(controlId), selected);
                res
                    .Html("#EditProcessDataChange", new HtmlBuilder()
                        .EditProcessDataChange(
                            context: context,
                            ss: SiteSettings,
                            dataChanges: dataChanges))
                    .Val(
                        "#ProcessDataChanges",
                        dataChanges.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenProcessDataChangeDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewProcessDataChange")
            {
                OpenProcessDataChangeDialog(
                    context: context,
                    res: res,
                    dataChange: new DataChange() { Type = DataChange.Types.CopyValue });
            }
            else
            {
                var dataChanges = context.Forms.Data("ProcessDataChanges").Deserialize<SettingList<DataChange>>();
                var dataChange = dataChanges?.Get(context.Forms.Int("ProcessDataChangeId"));
                if (dataChange == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenProcessDataChangeDialog(
                        context: context,
                        res: res,
                        dataChange: dataChange);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenProcessDataChangeDialog(
            Context context, ResponseCollection res, DataChange dataChange)
        {
            res.Html("#ProcessDataChangeDialog", SiteUtilities.ProcessDataChangeDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                dataChange: dataChange));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddProcessDataChange(Context context, ResponseCollection res, string controlId)
        {
            var dataChanges = context.Forms.Data("ProcessDataChangesTemp").Deserialize<SettingList<DataChange>>()
                ?? new SettingList<DataChange>();
            dataChanges.Add(new DataChange(
                id: dataChanges.MaxOrDefault(o => o.Id) + 1,
                type: context.Forms.Data("ProcessDataChangeType").ToEnum<DataChange.Types>(),
                columnName: context.Forms.Data("ProcessDataChangeColumnName"),
                baseDateTime: context.Forms.Data("ProcessDataChangeBaseDateTime"),
                value: ProcessDataChangeValue(context: context),
                valueFormulaNotUseDisplayName: context.Forms.Bool("ProcessDataChangeValueFormulaNotUseDisplayName"),
                valueFormulaIsDisplayError: context.Forms.Bool("ProcessDataChangeValueFormulaIsDisplayError")));
            res
                .ReplaceAll("#EditProcessDataChange", new HtmlBuilder()
                    .EditProcessDataChange(
                        context: context,
                        ss: SiteSettings,
                        dataChanges: dataChanges))
                .Val(
                    "#ProcessDataChanges",
                    dataChanges.ToJson())
                .CloseDialog(target: "#ProcessDataChangeDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateProcessDataChange(Context context, ResponseCollection res, string controlId)
        {
            var dataChanges = context.Forms.Data("ProcessDataChangesTemp").Deserialize<SettingList<DataChange>>();
            var dataChange = dataChanges?.Get(context.Forms.Int("ProcessDataChangeIdTemp"));
            dataChange.Update(
                type: context.Forms.Data("ProcessDataChangeType").ToEnum<DataChange.Types>(),
                columnName: context.Forms.Data("ProcessDataChangeColumnName"),
                baseDateTime: context.Forms.Data("ProcessDataChangeBaseDateTime"),
                value: ProcessDataChangeValue(context: context),
                valueFormulaNotUseDisplayName: context.Forms.Bool("ProcessDataChangeValueFormulaNotUseDisplayName"),
                valueFormulaIsDisplayError: context.Forms.Bool("ProcessDataChangeValueFormulaIsDisplayError"));
            res
                .ReplaceAll("#EditProcessDataChange", new HtmlBuilder()
                    .EditProcessDataChange(
                        context: context,
                        ss: SiteSettings,
                        dataChanges: dataChanges))
                .Val(
                    "#ProcessDataChanges",
                    dataChanges.ToJson())
                .CloseDialog(target: "#ProcessDataChangeDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string ProcessDataChangeValue(Context context)
        {
            var dataChange = new DataChange()
            {
                Type = context.Forms.Data("ProcessDataChangeType").ToEnum<DataChange.Types>()
            };
            if (dataChange.Visible(type: "Column"))
            {
                return context.Forms.Data("ProcessDataChangeValueColumnName");
            }
            else if (dataChange.Visible(type: "Value"))
            {
                return SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessDataChangeValue"));
            }
            else if (dataChange.Visible(type: "ValueFormula"))
            {
                return FormulaBuilder.ParseFormulaColumnName(
                    ss: SiteSettings,
                    formulaScript: context.Forms.Data("ProcessDataChangeValueFormula"));
            }
            else if (dataChange.Visible(type: "DateTime"))
            {
                return context.Forms.Int("ProcessDataChangeValueDateTime")
                    + "," + context.Forms.Data("ProcessDataChangeValueDateTimePeriod");
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyProcessDataChanges(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditProcessDataChange");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                var dataChanges = context.Forms.Data("ProcessDataChanges").Deserialize<SettingList<DataChange>>();
                dataChanges.Copy(selected);
                res
                    .Html("#EditProcessDataChange", new HtmlBuilder()
                        .EditProcessDataChange(
                            context: context,
                            ss: SiteSettings,
                            dataChanges: dataChanges))
                    .Val(
                        "#ProcessDataChanges",
                        dataChanges.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteProcessDataChanges(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditProcessDataChange");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                var dataChanges = context.Forms.Data("ProcessDataChanges").Deserialize<SettingList<DataChange>>();
                dataChanges.Delete(selected);
                res
                    .Html("#EditProcessDataChange", new HtmlBuilder()
                        .EditProcessDataChange(
                            context: context,
                            ss: SiteSettings,
                            dataChanges: dataChanges))
                    .Val(
                        "#ProcessDataChanges",
                        dataChanges.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetProcessNotificationsOrder(
            Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditProcessNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    var notifications = context.Forms.Data("ProcessNotifications").Deserialize<SettingList<Notification>>();
                    notifications.MoveUpOrDown(ColumnUtilities.ChangeCommand(controlId), selected);
                    res
                        .Html("#EditProcessNotification", new HtmlBuilder()
                            .EditProcessNotification(
                                context: context,
                                ss: SiteSettings,
                                notifications: notifications))
                        .Val(
                            "#ProcessNotifications",
                            notifications.ToJson());
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenProcessNotificationDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewProcessNotification")
            {
                OpenProcessNotificationDialog(
                    context: context,
                    res: res,
                    notification: new Notification(
                        type: Notification.Types.Mail,
                        monitorChangesColumns: SiteSettings
                            .ColumnDefinitionHash
                            .MonitorChangesDefinitions()
                            .Select(o => o.ColumnName)
                            .Where(o => SiteSettings.GetEditorColumnNames().Contains(o)
                                || o == "Comments")
                            .ToList()));
            }
            else
            {
                var notifications = context.Forms.Data("ProcessNotifications").Deserialize<SettingList<Notification>>();
                var notification = notifications?.Get(context.Forms.Int("ProcessNotificationId"));
                if (notification == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenProcessNotificationDialog(
                        context: context,
                        res: res,
                        notification: notification);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenProcessNotificationDialog(
            Context context, ResponseCollection res, Notification notification)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res.Html("#ProcessNotificationDialog", SiteUtilities.ProcessNotificationDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: context.Forms.ControlId(),
                    notification: notification));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddProcessNotification(Context context, ResponseCollection res, string controlId)
        {
            var invalid = SiteValidators.SetProcessNotification(
                context: context,
                ss: SiteSettings);
            switch (invalid.Type)
            {
                case Error.Types.None:
                    var notifications = context.Forms.Data("ProcessNotificationsTemp").Deserialize<SettingList<Notification>>()
                        ?? new SettingList<Notification>();
                    notifications.Add(new Notification()
                    {
                        Id = notifications.MaxOrDefault(o => o.Id) + 1,
                        Type = (Notification.Types)context.Forms.Int("ProcessNotificationType"),
                        Subject = SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessNotificationSubject")),
                        Address = SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessNotificationAddress")),
                        Token = context.Forms.Data("ProcessNotificationToken"),
                        Body = SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessNotificationBody"))
                    });
                    res
                        .ReplaceAll("#EditProcessNotification", new HtmlBuilder()
                            .EditProcessNotification(
                                context: context,
                                ss: SiteSettings,
                                notifications: notifications))
                        .Val(
                            "#ProcessNotifications",
                            notifications.ToJson())
                        .CloseDialog(target: "#ProcessNotificationDialog");
                    break;
                default:
                    res.Message(
                        message: invalid.Message(context: context),
                        target: "#ProcessNotificationDialog .message-dialog");
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateProcessNotification(Context context, ResponseCollection res, string controlId)
        {
            var invalid = SiteValidators.SetProcessNotification(
                context: context,
                ss: SiteSettings);
            switch (invalid.Type)
            {
                case Error.Types.None:
                    var notifications = context.Forms.Data("ProcessNotificationsTemp").Deserialize<SettingList<Notification>>();
                    var notification = notifications?.Get(context.Forms.Int("ProcessNotificationIdTemp"));
                    notification.Type = (Notification.Types)context.Forms.Int("ProcessNotificationType");
                    notification.Subject = SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessNotificationSubject"));
                    notification.Address = SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessNotificationAddress"));
                    notification.Token = context.Forms.Data("ProcessNotificationToken");
                    notification.Body = SiteSettings.LabelTextToColumnName(context.Forms.Data("ProcessNotificationBody"));
                    res
                        .ReplaceAll("#EditProcessNotification", new HtmlBuilder()
                            .EditProcessNotification(
                                context: context,
                                ss: SiteSettings,
                                notifications: notifications))
                        .Val(
                            "#ProcessNotifications",
                            notifications.ToJson())
                        .CloseDialog(target: "#ProcessNotificationDialog");
                    break;
                default:
                    res.Message(
                        message: invalid.Message(context: context),
                        target: "#ProcessNotificationDialog .message-dialog");
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteProcessNotifications(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditProcessNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    var notifications = context.Forms.Data("ProcessNotifications").Deserialize<SettingList<Notification>>();
                    notifications.Delete(selected);
                    res
                        .Html("#EditProcessNotification", new HtmlBuilder()
                            .EditProcessNotification(
                                context: context,
                                ss: SiteSettings,
                                notifications: notifications))
                        .Val(
                            "#ProcessNotifications",
                            notifications.ToJson());
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetStatusControlsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditStatusControl");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.StatusControls.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                SiteSettings.GetColumn(context, "Status").SetChoiceHash(context, SiteSettings.SiteId);
                res.Html("#EditStatusControl", new HtmlBuilder()
                    .EditStatusControl(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenStatusControlDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewStatusControl")
            {
                var statusControl = new StatusControl();
                OpenStatusControlDialog(
                    context: context,
                    res: res,
                    statusControl: statusControl);
            }
            else
            {
                var statusControl = SiteSettings.StatusControls?.Get(context.Forms.Int("StatusControlId"));
                if (statusControl == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context,
                        siteModel: this,
                        referenceId: SiteId);
                    OpenStatusControlDialog(
                        context: context,
                        res: res,
                        statusControl: statusControl);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenStatusControlDialog(
            Context context, ResponseCollection res, StatusControl statusControl)
        {
            res.Html("#StatusControlDialog", SiteUtilities.StatusControlDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                statusControl: statusControl));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddStatusControl(Context context, ResponseCollection res)
        {
            SiteSettings.SetChoiceHash(context: context);
            var statusControl = new StatusControl(
                id: SiteSettings.StatusControls.MaxOrDefault(o => o.Id) + 1,
                name: context.Forms.Data("StatusControlName"),
                description: context.Forms.Data("StatusControlDescription"),
                status: context.Forms.Int("StatusControlStatus"),
                readOnly: context.Forms.Bool("StatusControlReadOnly"),
                columnHash: StatusControlColumnHash(context: context),
                view: new View(
                    context: context,
                    ss: SiteSettings,
                    prefix: "StatusControl"),
                permissions: StatusControlPermissions(context: context));
            SiteSettings.StatusControls.Add(statusControl);
            res
                .ReplaceAll("#EditStatusControl", new HtmlBuilder()
                    .EditStatusControl(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateStatusControl(Context context, ResponseCollection res)
        {
            var statusControl = SiteSettings.StatusControls.Get(context.Forms.Int("StatusControlId"));
            if (statusControl == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                SiteSettings.SetChoiceHash(context: context);
                var view = statusControl.View ?? new View();
                view.SetByForm(
                    context: context,
                    ss: SiteSettings,
                    prefix: "StatusControl");
                statusControl.Update(
                    name: context.Forms.Data("StatusControlName"),
                    description: context.Forms.Data("StatusControlDescription"),
                    status: context.Forms.Int("StatusControlStatus"),
                    readOnly: context.Forms.Bool("StatusControlReadOnly"),
                    columnHash: StatusControlColumnHash(context: context),
                    view: view,
                    permissions: StatusControlPermissions(context: context));
                res
                    .ReplaceAll("#EditStatusControl", new HtmlBuilder()
                        .EditStatusControl(
                            context: context,
                            ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyStatusControls(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditStatusControl");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.StatusControls.Copy(selected);
                res.ReplaceAll("#EditStatusControl", new HtmlBuilder()
                    .EditStatusControl(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteStatusControl(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditStatusControl");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.StatusControls.Delete(selected);
                res.ReplaceAll("#EditStatusControl", new HtmlBuilder()
                    .EditStatusControl(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private Dictionary<string, StatusControl.ControlConstraintsTypes> StatusControlColumnHash(Context context)
        {
            var ret = context.Forms.List("StatusControlColumnHashAll")
                .ToDictionary(
                    o => o.Split_1st(),
                    o => StatusControl.GetControlType(controlType: o.Split_2nd()))
                .Where(o => o.Value != StatusControl.ControlConstraintsTypes.None)
                .ToDictionary(
                    o => o.Key,
                    o => o.Value);
            return ret;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private List<Permission> StatusControlPermissions(Context context)
        {
            return context.Forms.List("CurrentStatusControlAccessControlAll")
                .Select(data => new Permission(
                    name: data.Split_1st(),
                    id: data.Split_2nd().ToInt(),
                    type: Permissions.Types.NotSet))
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SearchStatusControlAccessControl(Context context, ResponseCollection res)
        {
            var statusControl = SiteSettings.StatusControls.Get(context.Forms.Int("StatusControlId"))
                ?? new StatusControl();
            var currentPermissions = statusControl.GetPermissions(ss: SiteSettings);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: SiteSettings,
                searchText: context.Forms.Data("SearchStatusControlAccessControl"),
                currentPermissions: currentPermissions,
                allUsers: false);
            return res
                .Html("#SourceStatusControlAccessControl", PermissionUtilities.PermissionListItem(
                    context: context,
                    ss: SiteSettings,
                    permissions: sourcePermissions.Page(0),
                    selectedValueTextCollection: context.Forms.Data("SourceStatusControlAccessControl")
                        .Deserialize<List<string>>()?
                        .Where(o => o != string.Empty),
                    withType: false))
                .Val("#SourceStatusControlAccessControlOffset", Parameters.Permissions.PageSize)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenViewDialog(Context context, ResponseCollection res, string controlId)
        {
            View view;
            if (controlId == "NewView")
            {
                view = new View(context: context, ss: SiteSettings);
                OpenViewDialog(context: context, res: res, view: view);
            }
            else
            {
                var idList = context.Forms.IntList("Views");
                if (idList.Count() != 1)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    view = SiteSettings.Views?.Get(idList.First());
                    if (view == null)
                    {
                        OpenDialogError(res, Messages.SelectOne(context: context));
                    }
                    else
                    {
                        SiteSettings.Views = SiteSettings.Views.Join(
                            context.Forms.List("ViewsAll").Select((val, key) => new { Key = key, Val = val }), v => v.Id, l => l.Val.ToInt(),
                                (v, l) => new { Views = v, OrderNo = l.Key })
                                .OrderBy(v => v.OrderNo)
                                .Select(v => v.Views)
                                .ToList();
                        OpenViewDialog(context: context, res: res, view: view);
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenViewDialog(Context context, ResponseCollection res, View view)
        {
            SiteSettings.SetChoiceHash(context: context);
            res.Html("#ViewDialog", SiteUtilities.ViewDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                view: view));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddViewFilter(
            Context context,
            ResponseCollection res,
            string prefix = "",
            SiteSettings ss = null)
        {
            ss = ss ?? SiteSettings;
            ss.SetChoiceHash(context: context);
            var column = ss.GetColumn(
                context: context,
                columnName: context.Forms.Data($"{prefix}ViewFilterSelector"));
            if (column != null)
            {
                res
                    .Append(
                        $"#{prefix}ViewFiltersTab .items",
                        new HtmlBuilder().ViewFilter(
                            context: context,
                            ss: ss,
                            column: column,
                            prefix: prefix))
                    .Remove($"#{prefix}ViewFilterSelector option:selected");
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SearchViewAccessControl(Context context, ResponseCollection res)
        {
            var view = SiteSettings.Views.Get(context.Forms.Int("ViewId"))
                ?? new View(context: context, ss: SiteSettings);
            var currentPermissions = view.GetPermissions(ss: SiteSettings);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: SiteSettings,
                searchText: context.Forms.Data("SearchViewAccessControl"),
                currentPermissions: currentPermissions,
                allUsers: false);
            return res
                .Html("#SourceViewAccessControl", PermissionUtilities.PermissionListItem(
                    context: context,
                    ss: SiteSettings,
                    permissions: sourcePermissions.Page(0),
                    selectedValueTextCollection: context.Forms.Data("SourceViewAccessControl")
                        .Deserialize<List<string>>()?
                        .Where(o => o != string.Empty),
                    withType: false))
                .Val("#SourceViewAccessControlOffset", Parameters.Permissions.PageSize)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddView(Context context, ResponseCollection res)
        {
            var view = new View(
                context: context,
                ss: SiteSettings);
            view.SetPermissions(permissions: ViewPermissions(context: context));
            SiteSettings.AddView(view: view);
            res
                .ViewResponses(SiteSettings, new List<int>
                {
                    SiteSettings.ViewLatestId.ToInt()
                })
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateView(Context context, ResponseCollection res)
        {
            var selected = context.Forms.Int("ViewId");
            var view = SiteSettings.Views?.Get(selected);
            if (view == null)
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                view.SetByForm(
                    context: context,
                    ss: SiteSettings);
                view.SetPermissions(permissions: ViewPermissions(context: context));
                res
                    .ViewResponses(SiteSettings, new List<int> { selected })
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyViews(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("Views");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            if (SiteSettings.Views != null)
            {
                var views = new List<View>();
                foreach (var view in SiteSettings.Views.Where(o => selected.Contains(o.Id)))
                {
                    var copied = view.ToJson().Deserialize<View>();
                    SiteSettings.ViewLatestId++;
                    copied.Id = SiteSettings.ViewLatestId.ToInt();
                    views.Add(copied);
                }
                SiteSettings.Views.AddRange(views);
            }
            res.ViewResponses(
                ss: SiteSettings,
                selected: selected);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteViews(Context context, ResponseCollection res)
        {
            SiteSettings.Views?.RemoveAll(o =>
                context.Forms.IntList("Views").Contains(o.Id));
            res.ViewResponses(SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private List<Permission> ViewPermissions(Context context)
        {
            return context.Forms.List("CurrentViewAccessControlAll")
                .Select(data => new Permission(
                    name: data.Split_1st(),
                    id: data.Split_2nd().ToInt(),
                    type: Permissions.Types.NotSet))
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetViewGridColumnsSelectable(Context context, ResponseCollection res)
        {
            var gridColumns = context.Forms.List("ViewGridColumnsAll");
            var listItemCollection = SiteSettings.ViewGridSelectableOptions(
                context: context,
                gridColumns: gridColumns,
                enabled: false,
                join: context.Forms.Data("ViewGridJoin"));
            if (!listItemCollection.Any())
            {
                res.Message(Messages.NotFound(context: context));
            }
            else
            {
                res.Html("#ViewGridSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: listItemCollection));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenNotificationDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewNotification")
            {
                OpenNotificationDialog(
                    context: context,
                    res: res,
                    notification: new Notification(
                        type: Notification.Types.Mail,
                        monitorChangesColumns: SiteSettings
                            .ColumnDefinitionHash
                            .MonitorChangesDefinitions()
                            .Select(o => o.ColumnName)
                            .Where(o => SiteSettings.GetEditorColumnNames().Contains(o)
                                || o == "Comments")
                            .ToList()));
            }
            else
            {
                var notification = SiteSettings.Notifications?.Get(context.Forms.Int("NotificationId"));
                if (notification == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenNotificationDialog(
                        context: context,
                        res: res,
                        notification: notification);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenNotificationDialog(
            Context context, ResponseCollection res, Notification notification)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                Session_MonitorChangesColumns(
                    context: context,
                    value: notification.MonitorChangesColumns.ToJson());
                res.Html("#NotificationDialog", SiteUtilities.NotificationDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: context.Forms.ControlId(),
                    notification: notification));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddNotification(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var invalid = SiteValidators.SetNotification(
                    context: context,
                    ss: SiteSettings);
                switch (invalid.Type)
                {
                    case Error.Types.None:
                        SiteSettings.Notifications.Add(new Notification(
                            id: SiteSettings.Notifications.MaxOrDefault(o => o.Id) + 1,
                            type: (Notification.Types)context.Forms.Int("NotificationType"),
                            prefix: context.Forms.Data("NotificationPrefix"),
                            subject: SiteSettings.LabelTextToColumnName(context.Forms.Data("NotificationSubject")),
                            address: SiteSettings.LabelTextToColumnName(context.Forms.Data("NotificationAddress")),
                            token: context.Forms.Data("NotificationToken"),
                            methodType: (Notification.MethodTypes)context.Forms.Int("NotificationMethodType"),
                            encoding: context.Forms.Data("NotificationEncoding"),
                            mediaType: context.Forms.Data("NotificationMediaType"),
                            headers: context.Forms.Data("NotificationRequestHeaders"),
                            useCustomFormat: context.Forms.Bool("NotificationUseCustomFormat"),
                            format: SiteSettings.LabelTextToColumnName(context.Forms.Data("NotificationFormat")),
                            monitorChangesColumns: context.Forms.List("MonitorChangesColumnsAll"),
                            beforeCondition: context.Forms.Int("BeforeCondition"),
                            afterCondition: context.Forms.Int("AfterCondition"),
                            expression: (Notification.Expressions)context.Forms.Int("Expression"),
                            afterCreate: context.Forms.Bool("NotificationAfterCreate"),
                            afterUpdate: context.Forms.Bool("NotificationAfterUpdate"),
                            afterDelete: context.Forms.Bool("NotificationAfterDelete"),
                            afterCopy: context.Forms.Bool("NotificationAfterCopy"),
                            afterBulkUpdate: context.Forms.Bool("NotificationAfterBulkUpdate"),
                            afterBulkDelete: context.Forms.Bool("NotificationAfterBulkDelete"),
                            afterImport: context.Forms.Bool("NotificationAfterImport"),
                            disabled: context.Forms.Bool("NotificationDisabled")));
                        SetNotificationsResponseCollection(context: context, res: res);
                        break;
                    default:
                        res.Message(invalid.Message(context: context));
                        break;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateNotification(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var notification = SiteSettings.Notifications.Get(context.Forms.Int("NotificationId"));
                if (notification == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    var invalid = SiteValidators.SetNotification(
                        context: context,
                        ss: SiteSettings);
                    switch (invalid.Type)
                    {
                        case Error.Types.None:
                            notification.Update(
                                type: (Notification.Types)context.Forms.Int("NotificationType"),
                                prefix: context.Forms.Data("NotificationPrefix"),
                                subject: SiteSettings.LabelTextToColumnName(context.Forms.Data("NotificationSubject")),
                                address: SiteSettings.LabelTextToColumnName(context.Forms.Data("NotificationAddress")),
                                token: context.Forms.Data("NotificationToken"),
                                methodType: (Notification.MethodTypes)context.Forms.Int("NotificationMethodType"),
                                encoding: context.Forms.Data("NotificationEncoding"),
                                mediaType: context.Forms.Data("NotificationMediaType"),
                                headers: context.Forms.Data("NotificationRequestHeaders"),
                                useCustomFormat: context.Forms.Bool("NotificationUseCustomFormat"),
                                format: SiteSettings.LabelTextToColumnName(context.Forms.Data("NotificationFormat")),
                                monitorChangesColumns: context.Forms.List("MonitorChangesColumnsAll"),
                                beforeCondition: context.Forms.Int("BeforeCondition"),
                                afterCondition: context.Forms.Int("AfterCondition"),
                                expression: (Notification.Expressions)context.Forms.Int("Expression"),
                                afterCreate: context.Forms.Bool("NotificationAfterCreate"),
                                afterUpdate: context.Forms.Bool("NotificationAfterUpdate"),
                                afterDelete: context.Forms.Bool("NotificationAfterDelete"),
                                afterCopy: context.Forms.Bool("NotificationAfterCopy"),
                                afterBulkUpdate: context.Forms.Bool("NotificationAfterBulkUpdate"),
                                afterBulkDelete: context.Forms.Bool("NotificationAfterBulkDelete"),
                                afterImport: context.Forms.Bool("NotificationAfterImport"),
                                disabled: context.Forms.Bool("NotificationDisabled"));
                            SetNotificationsResponseCollection(context: context, res: res);
                            break;
                        default:
                            res.Message(invalid.Message(context: context));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetNotificationsOrder(
            Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Notifications.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditNotification", new HtmlBuilder()
                        .EditNotification(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetNotificationsResponseCollection(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditNotification", new HtmlBuilder()
                        .EditNotification(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyNotifications(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Notifications.Copy(selected);
                    res.ReplaceAll("#EditNotification", new HtmlBuilder()
                        .EditNotification(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteNotifications(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Notice == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditNotification");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Notifications.Delete(selected);
                    res.ReplaceAll("#EditNotification", new HtmlBuilder()
                        .EditNotification(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenReminderDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewReminder")
            {
                OpenReminderDialog(
                    context: context,
                    res: res,
                    reminder: new Reminder(context: context)
                    {
                        ReminderType = ReminderUtilities.Types(context: context)
                            .Select(o => (Reminder.ReminderTypes)o.Key.ToInt())
                            .FirstOrDefault(),
                        Subject = Title.Value
                    });
            }
            else
            {
                var reminder = SiteSettings.Reminders?.Get(context.Forms.Int("ReminderId"));
                if (reminder == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context,
                        siteModel: this,
                        referenceId: SiteId);
                    OpenReminderDialog(
                        context: context,
                        res: res,
                        reminder: reminder);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenReminderDialog(
            Context context, ResponseCollection res, Reminder reminder)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res.Html("#ReminderDialog", SiteUtilities.ReminderDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: context.Forms.ControlId(),
                    reminder: reminder));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddReminder(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var invalid = SiteValidators.SetReminder(
                    context: context,
                    ss: SiteSettings);
                switch (invalid.Type)
                {
                    case Error.Types.None:
                        SiteSettings.Reminders.Add(new Reminder(
                            id: SiteSettings.Reminders.MaxOrDefault(o => o.Id) + 1,
                            reminderType: (Reminder.ReminderTypes)context.Forms.Int("ReminderType"),
                            subject: SiteSettings.LabelTextToColumnName(
                                context.Forms.Data("ReminderSubject")),
                            body: SiteSettings.LabelTextToColumnName(
                                context.Forms.Data("ReminderBody")),
                            line: SiteSettings.LabelTextToColumnName(
                                context.Forms.Data("ReminderLine")),
                            from: context.Forms.Data("ReminderFrom"),
                            to: SiteSettings.LabelTextToColumnName(
                                context.Forms.Data("ReminderTo")),
                            token: context.Forms.Data("ReminderToken"),
                            column: context.Forms.Data("ReminderColumn"),
                            startDateTime: context.Forms.DateTime("ReminderStartDateTime"),
                            type: (Times.RepeatTypes)context.Forms.Int("ReminderRepeatType"),
                            range: context.Forms.Int("ReminderRange"),
                            sendCompletedInPast: context.Forms.Bool("ReminderSendCompletedInPast"),
                            notSendIfNotApplicable: context.Forms.Bool("ReminderNotSendIfNotApplicable"),
                            notSendHyperLink: context.Forms.Bool("ReminderNotSendHyperLink"),
                            excludeOverdue: context.Forms.Bool("ReminderExcludeOverdue"),
                            condition: context.Forms.Int("ReminderCondition"),
                            disabled: context.Forms.Bool("ReminderDisabled")));
                        SetRemindersResponseCollection(context: context, res: res);
                        break;
                    default:
                        res.Message(invalid.Message(context: context));
                        break;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateReminder(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var reminder = SiteSettings.Reminders.Get(context.Forms.Int("ReminderId"));
                if (reminder == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    var invalid = SiteValidators.SetReminder(
                        context: context,
                        ss: SiteSettings);
                    switch (invalid.Type)
                    {
                        case Error.Types.None:
                            reminder.Update(
                                reminderType: (Reminder.ReminderTypes)context.Forms.Int("ReminderType"),
                                subject: SiteSettings.LabelTextToColumnName(
                                    context.Forms.Data("ReminderSubject")),
                                body: SiteSettings.LabelTextToColumnName(
                                    context.Forms.Data("ReminderBody")),
                                line: SiteSettings.LabelTextToColumnName(
                                    context.Forms.Data("ReminderLine")),
                                from: context.Forms.Data("ReminderFrom"),
                                to: SiteSettings.LabelTextToColumnName(
                                    context.Forms.Data("ReminderTo")),
                                token: context.Forms.Data("ReminderToken"),
                                column: context.Forms.Data("ReminderColumn"),
                                startDateTime: context.Forms.DateTime("ReminderStartDateTime"),
                                type: (Times.RepeatTypes)context.Forms.Int("ReminderRepeatType"),
                                range: context.Forms.Int("ReminderRange"),
                                sendCompletedInPast: context.Forms.Bool("ReminderSendCompletedInPast"),
                                notSendIfNotApplicable: context.Forms.Bool("ReminderNotSendIfNotApplicable"),
                                notSendHyperLink: context.Forms.Bool("ReminderNotSendHyperLink"),
                                excludeOverdue: context.Forms.Bool("ReminderExcludeOverdue"),
                                condition: context.Forms.Int("ReminderCondition"),
                                disabled: context.Forms.Bool("ReminderDisabled"));
                            SetRemindersResponseCollection(context: context, res: res);
                            break;
                        default:
                            res.Message(invalid.Message(context: context));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRemindersOrder(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Reminders.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRemindersResponseCollection(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyReminders(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Reminders.Copy(selected);
                    res.ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteReminders(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Reminders.Delete(selected);
                    res.ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void TestReminders(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Remind == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditReminder");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    context.ServerScriptDisabled = true;
                    SiteSettings.Remind(
                        context: context,
                        idList: selected,
                        scheduledTime: DateTime.Now,
                        test: true);
                    res.ReplaceAll("#EditReminder", new HtmlBuilder()
                        .EditReminder(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewExport")
            {
                OpenExportDialog(context: context, res: res, export: new Export(SiteSettings
                    .DefaultExportColumns(context: context)));
            }
            else
            {
                var export = SiteSettings.Exports?.Get(context.Forms.Int("ExportId"));
                if (export == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    export.SetColumns(
                        context: context,
                        ss: SiteSettings);
                    OpenExportDialog(context: context, res: res, export: export);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportDialog(Context context, ResponseCollection res, Export export)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                Session_Export(
                    context: context,
                    value: export.ToJson());
                res.Html("#ExportDialog", SiteUtilities.ExportDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: context.Forms.ControlId(),
                    export: export));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddExport(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                Export = Session_Export(context: context);
                if (Export == null && controlId == "EditExport")
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    Export.SetColumns(
                        context: context,
                        ss: SiteSettings);
                    int id = 1;
                    var columns = new List<ExportColumn>();
                    context.Forms.List("ExportColumnsAll").ForEach(o =>
                    {
                        var exp = o.Deserialize<ExportColumn>()
                            ?? Session_Export(context: context)
                                .Columns
                                .Where(c => c.Id.ToString() == o)
                                .FirstOrDefault();
                        columns.Add(new ExportColumn()
                        {
                            Id = id++,
                            ColumnName = exp.ColumnName,
                            LabelText = exp.LabelText,
                            Type = exp.Type,
                            Format = exp.Format,
                            OutputClassColumn = exp.OutputClassColumn,
                            SiteTitle = exp.SiteTitle,
                            Column = exp.Column,
                        });
                    });
                    Export.Columns = columns;
                    Export.Id = SiteSettings.Exports.MaxOrDefault(o => o.Id) + 1;
                    Export.Name = context.Forms.Data("ExportName");
                    Export.Type = (Export.Types)context.Forms.Int("ExportType");
                    Export.Header = context.Forms.Bool("ExportHeader");
                    Export.DelimiterType = (Export.DelimiterTypes)context.Forms.Int("DelimiterType");
                    Export.EncloseDoubleQuotes = context.Forms.Bool("EncloseDoubleQuotes");
                    Export.ExecutionType = (Export.ExecutionTypes)context.Forms.Int("ExecutionType");
                    Export.SetPermissions(permissions: ExportPermissions(context: context));
                    SiteSettings.Exports.Add(Export);
                    SetExportsResponseCollection(context: context, res: res);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateExport(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var export = SiteSettings.Exports.Get(context.Forms.Int("ExportId"));
                if (export == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    int id = 1;
                    var columns = new List<ExportColumn>();
                    context.Forms.List("ExportColumnsAll").ForEach(o =>
                    {
                        var exp = o.Deserialize<ExportColumn>()
                            ?? Session_Export(context: context)
                                .Columns
                                .Where(c => c.Id.ToString() == o)
                                .FirstOrDefault();
                        columns.Add(new ExportColumn()
                        {
                            Id = id++,
                            ColumnName = exp.ColumnName,
                            LabelText = exp.LabelText,
                            Type = exp.Type,
                            Format = exp.Format,
                            OutputClassColumn = exp.OutputClassColumn,
                            SiteTitle = exp.SiteTitle,
                            Column = exp.Column
                        });
                    });
                    export.SetColumns(
                        context: context,
                        ss: SiteSettings);
                    export.Update(
                        name: context.Forms.Data("ExportName"),
                        type: (Export.Types)context.Forms.Int("ExportType"),
                        header: context.Forms.Bool("ExportHeader"),
                        columns: columns,
                        delimiterType: (Export.DelimiterTypes)context.Forms.Int("DelimiterType"),
                        encloseDoubleQuotes: context.Forms.Bool("EncloseDoubleQuotes"),
                        executionType: (Export.ExecutionTypes)context.Forms.Int("ExecutionType"),
                        permissions: ExportPermissions(context: context));
                    SetExportsResponseCollection(context: context, res: res);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportsOrder(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditExport");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Exports.MoveUpOrDown(
                        ColumnUtilities.ChangeCommand(controlId), selected);
                    res.Html("#EditExport", new HtmlBuilder()
                        .EditExport(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportsResponseCollection(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res
                    .ReplaceAll("#EditExport", new HtmlBuilder()
                        .EditExport(context: context, ss: SiteSettings))
                    .CloseDialog();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetExportColumnsSelectable(Context context, ResponseCollection res)
        {
            var join = context.Forms.Data("ExportJoin");
            res
                .Html("#ExportSourceColumns", new HtmlBuilder()
                    .SelectableItems(listItemCollection: ExportUtilities
                        .ColumnOptions(columns: SiteSettings.ExportColumns(
                            context: context,
                            join: join))))
                .SetFormData("ExportSourceColumns", "[]");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyExports(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditExport");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Exports.Copy(selected);
                    res.ReplaceAll("#EditExport", new HtmlBuilder()
                        .EditExport(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteExports(Context context, ResponseCollection res)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                var selected = context.Forms.IntList("EditExport");
                if (selected?.Any() != true)
                {
                    res.Message(Messages.SelectTargets(context: context)).ToJson();
                }
                else
                {
                    SiteSettings.Exports.Delete(selected);
                    res.ReplaceAll("#EditExport", new HtmlBuilder()
                        .EditExport(context: context, ss: SiteSettings));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportColumnsDialog(
            Context context, ResponseCollection res, string controlId)
        {
            Export = Session_Export(context: context);
            var selected = context.Forms.List("ExportColumns");
            if (selected.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                int id = 1;
                var columns = new List<ExportColumn>();
                var selectedNewId = "";
                context.Forms.List("ExportColumnsAll").ForEach(o =>
                {
                    var export = o.Deserialize<ExportColumn>()
                        ?? Export.Columns.Where(c => c.ToJson() == o).FirstOrDefault();
                    if (export.ToJson() == selected[0]) selectedNewId = id.ToString();
                    columns.Add(new ExportColumn()
                    {
                        Id = id++,
                        ColumnName = export.ColumnName,
                        LabelText = export.LabelText,
                        Type = export.Type,
                        Format = export.Format,
                        OutputClassColumn = export.OutputClassColumn,
                        SiteTitle = export.SiteTitle,
                        Column = export.Column
                    });
                });
                Export.Columns = columns;
                Export.SetColumns(
                    context: context,
                    ss: SiteSettings);
                Session_Export(
                    context: context,
                    value: Export.ToJson());
                var column = Export.Columns.FirstOrDefault(o =>
                    o.Id.ToString() == selectedNewId);
                if (column == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    OpenExportColumnsDialog(
                        context: context,
                        res: res,
                        column: column);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenExportColumnsDialog(
            Context context, ResponseCollection res, ExportColumn column)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                res.Html("#ExportColumnsDialog", SiteUtilities.ExportColumnsDialog(
                    context: context,
                    ss: SiteSettings,
                    controlId: context.Forms.ControlId(),
                    exportColumn: column));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateExportColumns(Context context, ResponseCollection res, string controlId)
        {
            if (context.ContractSettings.Export == false)
            {
                res.Message(Messages.Restricted(context: context));
            }
            else
            {
                Export = Session_Export(context: context);
                Export.SetColumns(
                    context: context,
                    ss: SiteSettings);
                var column = Export.Columns.FirstOrDefault(o =>
                    o.Id == context.Forms.Int("ExportColumnId"));
                if (column == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                else
                {
                    var selected = new List<string> { column.Id.ToString() };
                    column.Update(
                        labelText: context.Forms.Data("ExportColumnLabelText"),
                        type: (ExportColumn.Types)context.Forms.Int("ExportColumnType"),
                        format: context.Forms.Data("ExportFormat"),
                        outputClassColumn: context.Forms.Bool("ExportColumnOutputClassColumn"));
                    Session_Export(
                        context: context,
                        value: Export.ToJson());
                    res
                        .Html("#ExportColumns", new HtmlBuilder().SelectableItems(
                            listItemCollection: ExportUtilities
                                .ColumnOptions(Export.Columns),
                            selectedValueTextCollection: selected))
                        .SetFormData("ExportColumns", selected.ToJson())
                        .CloseDialog("#ExportColumnsDialog");
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SearchExportAccessControl(Context context, ResponseCollection res)
        {
            var export = SiteSettings.Exports.Get(context.Forms.Int("ExportId"))
                ?? new Export();
            var currentPermissions = export.GetPermissions(ss: SiteSettings);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: SiteSettings,
                searchText: context.Forms.Data("SearchExportAccessControl"),
                currentPermissions: currentPermissions,
                allUsers: false);
            return res
                .Html("#SourceExportAccessControl", PermissionUtilities.PermissionListItem(
                    context: context,
                    ss: SiteSettings,
                    permissions: sourcePermissions.Page(0),
                    selectedValueTextCollection: context.Forms.Data("SourceExportAccessControl")
                        .Deserialize<List<string>>()
                        ?.Where(o => o != string.Empty),
                    withType: false))
                .Val("#SourceExportAccessControlOffset", Parameters.Permissions.PageSize)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private List<Permission> ExportPermissions(Context context)
        {
            return context.Forms.List("CurrentExportAccessControlAll")
                .Select(data => new Permission(
                    name: data.Split_1st(),
                    id: data.Split_2nd().ToInt(),
                    type: Permissions.Types.NotSet))
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetStylesOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditStyle");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Styles.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditStyle", new HtmlBuilder()
                    .EditStyle(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenStyleDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewStyle")
            {
                var style = new Style() { All = true };
                OpenStyleDialog(
                    context: context,
                    res: res,
                    style: style);
            }
            else
            {
                var style = SiteSettings.Styles?.Get(context.Forms.Int("StyleId"));
                if (style == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenStyleDialog(
                        context: context,
                        res: res,
                        style: style);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenStyleDialog(Context context, ResponseCollection res, Style style)
        {
            res.Html("#StyleDialog", SiteUtilities.StyleDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                style: style));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddStyle(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.Styles.Add(new Style(
                id: SiteSettings.Styles.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("StyleTitle"),
                all: context.Forms.Bool("StyleAll"),
                _new: context.Forms.Bool("StyleNew"),
                edit: context.Forms.Bool("StyleEdit"),
                index: context.Forms.Bool("StyleIndex"),
                calendar: context.Forms.Bool("StyleCalendar"),
                crosstab: context.Forms.Bool("StyleCrosstab"),
                gantt: context.Forms.Bool("StyleGantt"),
                burnDown: context.Forms.Bool("StyleBurnDown"),
                timeSeries: context.Forms.Bool("StyleTimeSeries"),
                analy: context.Forms.Bool("StyleAnaly"),
                kamban: context.Forms.Bool("StyleKamban"),
                imageLib: context.Forms.Bool("StyleImageLib"),
                disabled: context.Forms.Bool("StyleDisabled"),
                body: context.Forms.Data("StyleBody")));
            res
                .ReplaceAll("#EditStyle", new HtmlBuilder()
                    .EditStyle(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateStyle(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.Styles?
                .FirstOrDefault(o => o.Id == context.Forms.Int("StyleId"))?
                .Update(
                    title: context.Forms.Data("StyleTitle"),
                    all: context.Forms.Bool("StyleAll"),
                    _new: context.Forms.Bool("StyleNew"),
                    edit: context.Forms.Bool("StyleEdit"),
                    index: context.Forms.Bool("StyleIndex"),
                    calendar: context.Forms.Bool("StyleCalendar"),
                    crosstab: context.Forms.Bool("StyleCrosstab"),
                    gantt: context.Forms.Bool("StyleGantt"),
                    burnDown: context.Forms.Bool("StyleBurnDown"),
                    timeSeries: context.Forms.Bool("StyleTimeSeries"),
                    analy: context.Forms.Bool("StyleAnaly"),
                    kamban: context.Forms.Bool("StyleKamban"),
                    imageLib: context.Forms.Bool("StyleImageLib"),
                    disabled: context.Forms.Bool("StyleDisabled"),
                    body: context.Forms.Data("StyleBody"));
            res
                .Html("#EditStyle", new HtmlBuilder()
                    .EditStyle(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyStyles(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditStyle");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Styles.Copy(selected);
                res.ReplaceAll("#EditStyle", new HtmlBuilder()
                    .EditStyle(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteStyles(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditStyle");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Styles.Delete(selected);
                res.ReplaceAll("#EditStyle", new HtmlBuilder()
                    .EditStyle(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetScriptsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Scripts.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditScript", new HtmlBuilder()
                    .EditScript(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenScriptDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewScript")
            {
                var script = new Script() { All = true };
                OpenScriptDialog(
                    context: context,
                    res: res,
                    script: script);
            }
            else
            {
                var script = SiteSettings.Scripts?.Get(context.Forms.Int("ScriptId"));
                if (script == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenScriptDialog(
                        context: context,
                        res: res,
                        script: script);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenScriptDialog(Context context, ResponseCollection res, Script script)
        {
            res.Html("#ScriptDialog", SiteUtilities.ScriptDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                script: script));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddScript(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.Scripts.Add(new Script(
                id: SiteSettings.Scripts.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("ScriptTitle"),
                all: context.Forms.Bool("ScriptAll"),
                _new: context.Forms.Bool("ScriptNew"),
                edit: context.Forms.Bool("ScriptEdit"),
                index: context.Forms.Bool("ScriptIndex"),
                calendar: context.Forms.Bool("ScriptCalendar"),
                crosstab: context.Forms.Bool("ScriptCrosstab"),
                gantt: context.Forms.Bool("ScriptGantt"),
                burnDown: context.Forms.Bool("ScriptBurnDown"),
                timeSeries: context.Forms.Bool("ScriptTimeSeries"),
                analy: context.Forms.Bool("ScriptAnaly"),
                kamban: context.Forms.Bool("ScriptKamban"),
                imageLib: context.Forms.Bool("ScriptImageLib"),
                disabled: context.Forms.Bool("ScriptDisabled"),
                body: context.Forms.Data("ScriptBody")));
            res
                .ReplaceAll("#EditScript", new HtmlBuilder()
                    .EditScript(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateScript(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.Scripts?
                .FirstOrDefault(o => o.Id == context.Forms.Int("ScriptId"))?
                .Update(
                    title: context.Forms.Data("ScriptTitle"),
                    all: context.Forms.Bool("ScriptAll"),
                    _new: context.Forms.Bool("ScriptNew"),
                    edit: context.Forms.Bool("ScriptEdit"),
                    index: context.Forms.Bool("ScriptIndex"),
                    calendar: context.Forms.Bool("ScriptCalendar"),
                    crosstab: context.Forms.Bool("ScriptCrosstab"),
                    gantt: context.Forms.Bool("ScriptGantt"),
                    burnDown: context.Forms.Bool("ScriptBurnDown"),
                    timeSeries: context.Forms.Bool("ScriptTimeSeries"),
                    analy: context.Forms.Bool("ScriptAnaly"),
                    kamban: context.Forms.Bool("ScriptKamban"),
                    imageLib: context.Forms.Bool("ScriptImageLib"),
                    disabled: context.Forms.Bool("ScriptDisabled"),
                    body: context.Forms.Data("ScriptBody"));
            res
                .Html("#EditScript", new HtmlBuilder()
                    .EditScript(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyScripts(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Scripts.Copy(selected);
                res.ReplaceAll("#EditScript", new HtmlBuilder()
                    .EditScript(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteScripts(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Scripts.Delete(selected);
                res.ReplaceAll("#EditScript", new HtmlBuilder()
                    .EditScript(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetHtmlsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditHtml");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Htmls.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditHtml", new HtmlBuilder()
                    .EditHtml(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenHtmlDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewHtml")
            {
                var html = new Html() { All = true };
                OpenHtmlDialog(
                    context: context,
                    res: res,
                    html: html);
            }
            else
            {
                var html = SiteSettings.Htmls?.Get(context.Forms.Int("HtmlId"));
                if (html == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenHtmlDialog(
                        context: context,
                        res: res,
                        html: html);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenHtmlDialog(Context context, ResponseCollection res, Html html)
        {
            res.Html("#HtmlDialog", SiteUtilities.HtmlDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                html: html));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddHtml(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.Htmls.Add(new Html(
                id: SiteSettings.Htmls.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("HtmlTitle"),
                positionType: context.Forms.Data("HtmlPositionType").ToEnum<Html.PositionTypes>(),
                all: context.Forms.Bool("HtmlAll"),
                _new: context.Forms.Bool("HtmlNew"),
                edit: context.Forms.Bool("HtmlEdit"),
                index: context.Forms.Bool("HtmlIndex"),
                calendar: context.Forms.Bool("HtmlCalendar"),
                crosstab: context.Forms.Bool("HtmlCrosstab"),
                gantt: context.Forms.Bool("HtmlGantt"),
                burnDown: context.Forms.Bool("HtmlBurnDown"),
                timeSeries: context.Forms.Bool("HtmlTimeSeries"),
                analy: context.Forms.Bool("HtmlAnaly"),
                kamban: context.Forms.Bool("HtmlKamban"),
                imageLib: context.Forms.Bool("HtmlImageLib"),
                disabled: context.Forms.Bool("HtmlDisabled"),
                body: context.Forms.Data("HtmlBody")));
            res
                .ReplaceAll("#EditHtml", new HtmlBuilder()
                    .EditHtml(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateHtml(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.Htmls?
                .FirstOrDefault(o => o.Id == context.Forms.Int("HtmlId"))?
                .Update(
                    title: context.Forms.Data("HtmlTitle"),
                    positionType: context.Forms.Data("HtmlPositionType").ToEnum<Html.PositionTypes>(),
                    all: context.Forms.Bool("HtmlAll"),
                    _new: context.Forms.Bool("HtmlNew"),
                    edit: context.Forms.Bool("HtmlEdit"),
                    index: context.Forms.Bool("HtmlIndex"),
                    calendar: context.Forms.Bool("HtmlCalendar"),
                    crosstab: context.Forms.Bool("HtmlCrosstab"),
                    gantt: context.Forms.Bool("HtmlGantt"),
                    burnDown: context.Forms.Bool("HtmlBurnDown"),
                    timeSeries: context.Forms.Bool("HtmlTimeSeries"),
                    analy: context.Forms.Bool("HtmlAnaly"),
                    kamban: context.Forms.Bool("HtmlKamban"),
                    imageLib: context.Forms.Bool("HtmlImageLib"),
                    disabled: context.Forms.Bool("HtmlDisabled"),
                    body: context.Forms.Data("HtmlBody"));
            res
                .Html("#EditHtml", new HtmlBuilder()
                    .EditHtml(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyHtmls(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditHtml");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Htmls.Copy(selected);
                res.ReplaceAll("#EditHtml", new HtmlBuilder()
                    .EditHtml(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteHtmls(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditHtml");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.Htmls.Delete(selected);
                res.ReplaceAll("#EditHtml", new HtmlBuilder()
                    .EditHtml(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetServerScriptsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditServerScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.ServerScripts.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenServerScriptDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewServerScript")
            {
                var script = new ServerScript();
                OpenServerScriptDialog(
                    context: context,
                    res: res,
                    script: script);
            }
            else
            {
                var script = SiteSettings.ServerScripts?.Get(context.Forms.Int("ServerScriptId"));
                if (script == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenServerScriptDialog(
                        context: context,
                        res: res,
                        script: script);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenServerScriptDialog(Context context, ResponseCollection res, ServerScript script)
        {
            res.Html("#ServerScriptDialog", SiteUtilities.ServerScriptDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                script: script));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddServerScript(Context context, ResponseCollection res, string controlId)
        {
            var script = new ServerScript(
                id: context.Forms.Int("ServerScriptId"),
                title: context.Forms.Data("ServerScriptTitle"),
                name: context.Forms.Data("ServerScriptName"),
                whenloadingSiteSettings: context.Forms.Bool("ServerScriptWhenloadingSiteSettings"),
                whenViewProcessing: context.Forms.Bool("ServerScriptWhenViewProcessing"),
                whenloadingRecord: context.Forms.Bool("ServerScriptWhenloadingRecord"),
                beforeFormula: context.Forms.Bool("ServerScriptBeforeFormula"),
                afterFormula: context.Forms.Bool("ServerScriptAfterFormula"),
                beforeCreate: context.Forms.Bool("ServerScriptBeforeCreate"),
                afterCreate: context.Forms.Bool("ServerScriptAfterCreate"),
                beforeUpdate: context.Forms.Bool("ServerScriptBeforeUpdate"),
                afterUpdate: context.Forms.Bool("ServerScriptAfterUpdate"),
                beforeDelete: context.Forms.Bool("ServerScriptBeforeDelete"),
                afterDelete: context.Forms.Bool("ServerScriptAfterDelete"),
                beforeOpeningPage: context.Forms.Bool("ServerScriptBeforeOpeningPage"),
                beforeOpeningRow: context.Forms.Bool("ServerScriptBeforeOpeningRow"),
                shared: context.Forms.Bool("ServerScriptShared"),
                background: false,
                body: context.Forms.Data("ServerScriptBody"),
                timeOut: GetServerScriptTimeOutValue(context: context));
            var invalid = ServerScriptValidators.OnCreating(
                context: context,
                serverScript: script);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    res.Message(invalid.Message(context: context));
                    return;
            }
            SiteSettings.ServerScripts.Add(new ServerScript(
                id: SiteSettings.ServerScripts.MaxOrDefault(o => o.Id) + 1,
                title: script.Title,
                name: script.Name,
                whenloadingSiteSettings: script.WhenloadingSiteSettings ?? default,
                whenViewProcessing: script.WhenViewProcessing ?? default,
                whenloadingRecord: script.WhenloadingRecord ?? default,
                beforeFormula: script.BeforeFormula ?? default,
                afterFormula: script.AfterFormula ?? default,
                beforeCreate: script.BeforeCreate ?? default,
                afterCreate: script.AfterCreate ?? default,
                beforeUpdate: script.BeforeUpdate ?? default,
                afterUpdate: script.AfterUpdate ?? default,
                beforeDelete: script.BeforeDelete ?? default,
                afterDelete: script.AfterDelete ?? default,
                beforeOpeningPage: script.BeforeOpeningPage ?? default,
                beforeOpeningRow: script.BeforeOpeningRow ?? default,
                shared: script.Shared ?? default,
                background: script.Background ?? default,
                body: script.Body,
                timeOut: script.TimeOut));
            res
                .ReplaceAll("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateServerScript(Context context, ResponseCollection res, string controlId)
        {
            var script = new ServerScript(
                id: context.Forms.Int("ServerScriptId"),
                title: context.Forms.Data("ServerScriptTitle"),
                name: context.Forms.Data("ServerScriptName"),
                whenloadingSiteSettings: context.Forms.Bool("ServerScriptWhenloadingSiteSettings"),
                whenViewProcessing: context.Forms.Bool("ServerScriptWhenViewProcessing"),
                whenloadingRecord: context.Forms.Bool("ServerScriptWhenloadingRecord"),
                beforeFormula: context.Forms.Bool("ServerScriptBeforeFormula"),
                afterFormula: context.Forms.Bool("ServerScriptAfterFormula"),
                beforeCreate: context.Forms.Bool("ServerScriptBeforeCreate"),
                afterCreate: context.Forms.Bool("ServerScriptAfterCreate"),
                beforeUpdate: context.Forms.Bool("ServerScriptBeforeUpdate"),
                afterUpdate: context.Forms.Bool("ServerScriptAfterUpdate"),
                beforeDelete: context.Forms.Bool("ServerScriptBeforeDelete"),
                afterDelete: context.Forms.Bool("ServerScriptAfterDelete"),
                beforeOpeningPage: context.Forms.Bool("ServerScriptBeforeOpeningPage"),
                beforeOpeningRow: context.Forms.Bool("ServerScriptBeforeOpeningRow"),
                shared: context.Forms.Bool("ServerScriptShared"),
                background: false,
                body: context.Forms.Data("ServerScriptBody"),
                timeOut: GetServerScriptTimeOutValue(context: context));
            var invalid = ServerScriptValidators.OnUpdating(
                context: context,
                serverScript: script);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    res.Message(invalid.Message(context: context));
                    return;
            }
            SiteSettings.ServerScripts?
                .FirstOrDefault(o => o.Id == script.Id)?
                .Update(
                    title: script.Title,
                    name: script.Name,
                    whenloadingSiteSettings: script.WhenloadingSiteSettings ?? default,
                    whenViewProcessing: script.WhenViewProcessing ?? default,
                    whenloadingRecord: script.WhenloadingRecord ?? default,
                    beforeFormula: script.BeforeFormula ?? default,
                    afterFormula: script.AfterFormula ?? default,
                    beforeCreate: script.BeforeCreate ?? default,
                    afterCreate: script.AfterCreate ?? default,
                    beforeUpdate: script.BeforeUpdate ?? default,
                    afterUpdate: script.AfterUpdate ?? default,
                    beforeDelete: script.BeforeDelete ?? default,
                    afterDelete: script.AfterDelete ?? default,
                    beforeOpeningPage: script.BeforeOpeningPage ?? default,
                    beforeOpeningRow: script.BeforeOpeningRow ?? default,
                    shared: script.Shared ?? default,
                    background: script.Background ?? default,
                    body: script.Body,
                    timeOut: script.TimeOut);
            res
                .Html("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private int? GetServerScriptTimeOutValue(Context context)
        {
            int timeOut;
            return int.TryParse(context.Forms.Data("ServerScriptTimeOut"), out timeOut)
                && timeOut <= Parameters.Script.ServerScriptTimeOutMax && timeOut >= Parameters.Script.ServerScriptTimeOutMin
                    ? timeOut
                    : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyServerScripts(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditServerScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.ServerScripts.Copy(selected);
                res.ReplaceAll("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteServerScripts(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditServerScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.ServerScripts.Delete(selected);
                res.ReplaceAll("#EditServerScript", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool NotFound()
        {
            return SiteId != 0 && AccessStatus != Databases.AccessStatuses.Selected;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SiteModel InheritSite(Context context)
        {
            return new SiteModel(context: context, siteId: InheritPermission);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Move(List<string> sources, List<string> destinations)
        {
            destinations.Clear();
            destinations.AddRange(sources);
            sources.Clear();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDialogError(ResponseCollection res, Message message)
        {
            res
                .CloseDialog()
                .Message(message);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetBulkUpdateColumnsOrder(
            Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditBulkUpdateColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.BulkUpdateColumns.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditBulkUpdateColumns", new HtmlBuilder()
                    .EditBulkUpdateColumns(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenBulkUpdateColumnDialog(
            Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewBulkUpdateColumn")
            {
                var bulkUpdateColumn = new BulkUpdateColumn() { };
                OpenBulkUpdateColumnDialog(
                    context: context,
                    res: res,
                    bulkUpdateColumn: bulkUpdateColumn);
            }
            else
            {
                var BulkUpdateColumn = SiteSettings.BulkUpdateColumns?.Get(context.Forms.Int("BulkUpdateColumnId"));
                if (BulkUpdateColumn == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenBulkUpdateColumnDialog(
                        context: context,
                        res: res,
                        bulkUpdateColumn: BulkUpdateColumn);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenBulkUpdateColumnDialog(
            Context context, ResponseCollection res, BulkUpdateColumn bulkUpdateColumn)
        {
            res.Html("#BulkUpdateColumnDialog", SiteUtilities.BulkUpdateColumnDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                bulkUpdateColumn: bulkUpdateColumn));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddBulkUpdateColumn(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.BulkUpdateColumns.Add(new BulkUpdateColumn(
                id: SiteSettings.BulkUpdateColumns.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("BulkUpdateColumnTitle"),
                columns: context.Forms.List("BulkUpdateColumnColumnsAll"),
                details: context.Forms.Data("BulkUpdateColumnDetails")?.Deserialize<Dictionary<string, BulkUpdateColumnDetail>>()));
            res
                .ReplaceAll("#EditBulkUpdateColumns", new HtmlBuilder()
                    .EditBulkUpdateColumns(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateBulkUpdateColumn(
            Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.BulkUpdateColumns?
                .FirstOrDefault(o => o.Id == context.Forms.Int("BulkUpdateColumnId"))?
                .Update(
                    title: context.Forms.Data("BulkUpdateColumnTitle"),
                    columns: context.Forms.List("BulkUpdateColumnColumnsAll"),
                    details: context.Forms.Data("BulkUpdateColumnDetails")?.Deserialize<Dictionary<string, BulkUpdateColumnDetail>>());
            res
                .Html("#EditBulkUpdateColumns", new HtmlBuilder()
                    .EditBulkUpdateColumns(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyBulkUpdateColumns(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditBulkUpdateColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.BulkUpdateColumns.Copy(selected);
                res.ReplaceAll("#EditBulkUpdateColumns", new HtmlBuilder()
                    .EditBulkUpdateColumns(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteBulkUpdateColumns(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditBulkUpdateColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.BulkUpdateColumns.Delete(selected);
                res.ReplaceAll("#EditBulkUpdateColumns", new HtmlBuilder()
                    .EditBulkUpdateColumns(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenBulkUpdateColumnDetailDialog(
            Context context,
            ResponseCollection res)
        {
            var selected = context.Forms.List("BulkUpdateColumnColumns");
            if (selected.Count() != 1)
            {
                res.Message(Messages.SelectOne(context: context));
            }
            else
            {
                var columnName = selected.FirstOrDefault();
                SiteSettings.SetChoiceHash(context: context);
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                if (column == null)
                {
                    res.Message(Messages.NotFound(context: context));
                }
                var detail = context.Forms.Data("BulkUpdateColumnDetails")
                    ?.Deserialize<Dictionary<string, BulkUpdateColumnDetail>>()
                    ?.Get(columnName)
                        ?? new BulkUpdateColumnDetail();
                res.Html("#BulkUpdateColumnDetailDialog", SiteUtilities.BulkUpdateColumnDetailDialog(
                    context: context,
                    ss: SiteSettings,
                    column: column,
                    detail: detail));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateBulkUpdateColumnDetail(Context context, ResponseCollection res)
        {
            var columnName = context.Forms.Data("BulkUpdateColumnDetailColumnName");
            var details = context.Forms.Data("BulkUpdateColumnDetailsTemp")?.Deserialize<Dictionary<string, BulkUpdateColumnDetail>>()
                ?? new Dictionary<string, BulkUpdateColumnDetail>();
            var defaultInput = context.Forms.Data("BulkUpdateColumnDetailDefaultInput");
            var validateRequired = context.Forms.Bool("BulkUpdateColumnDetailValidateRequired");
            var editorReadOnly = context.Forms.Bool("BulkUpdateColumnDetailEditorReadOnly");
            details.AddOrUpdate(columnName, new BulkUpdateColumnDetail()
            {
                DefaultInput = defaultInput,
                ValidateRequired = validateRequired,
                EditorReadOnly = editorReadOnly
            });
            res
                .Val("#BulkUpdateColumnDetails", details.ToJson())
                .CloseDialog("#BulkUpdateColumnDetailDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetRelatingColumnsOrder(
            Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditRelatingColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.RelatingColumns.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditRelatingColumns", new HtmlBuilder()
                    .EditRelatingColumns(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenRelatingColumnDialog(
            Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewRelatingColumn")
            {
                var relatingColumn = new RelatingColumn() { };
                OpenRelatingColumnDialog(
                    context: context,
                    res: res,
                    relatingColumn: relatingColumn);
            }
            else
            {
                var RelatingColumn = SiteSettings.RelatingColumns?.Get(context.Forms.Int("RelatingColumnId"));
                if (RelatingColumn == null)
                {
                    OpenDialogError(res, Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenRelatingColumnDialog(
                        context: context,
                        res: res,
                        relatingColumn: RelatingColumn);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenRelatingColumnDialog(
            Context context, ResponseCollection res, RelatingColumn relatingColumn)
        {
            res.Html("#RelatingColumnDialog", SiteUtilities.RelatingColumnDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                relatingColumn: relatingColumn));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddRelatingColumn(Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.RelatingColumns.Add(new RelatingColumn(
                id: SiteSettings.RelatingColumns.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("RelatingColumnTitle"),
                columns: context.Forms.List("RelatingColumnColumnsAll")));
            res
                .ReplaceAll("#EditRelatingColumns", new HtmlBuilder()
                    .EditRelatingColumns(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateRelatingColumn(
            Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.RelatingColumns?
                .FirstOrDefault(o => o.Id == context.Forms.Int("RelatingColumnId"))?
                .Update(
                    title: context.Forms.Data("RelatingColumnTitle"),
                    columns: context.Forms.List("RelatingColumnColumnsAll"));
            res
                .Html("#EditRelatingColumns", new HtmlBuilder()
                    .EditRelatingColumns(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteRelatingColumns(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditRelatingColumns");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.RelatingColumns.Delete(selected);
                res.ReplaceAll("#EditRelatingColumns", new HtmlBuilder()
                    .EditRelatingColumns(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetDashboardPartsOrder(Context context, ResponseCollection res, string controlId)
        {
            var selected = context.Forms.IntList("EditDashboardPart");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.DashboardParts.MoveUpOrDown(
                    ColumnUtilities.ChangeCommand(controlId), selected);
                res.Html("#EditDashboardPart", new HtmlBuilder()
                    .EditDashboardPart(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDashboardPartDialog(Context context, ResponseCollection res, DashboardPart dashboardPart)
        {
            res.Html("#DashboardPartDialog", SiteUtilities.DashboardPartDialog(
                context: context,
                ss: SiteSettings,
                controlId: context.Forms.ControlId(),
                dashboardPart: dashboardPart));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDashboardPartTimeLineSitesDialog(Context context, ResponseCollection res)
        {
            res.Html("#DashboardPartTimeLineSitesDialog", SiteUtilities.DashboardPartTimeLineSitesDialog(
                context: context,
                ss: SiteSettings,
                dashboardPartId: context.Forms.Int("DashboardPartId"),
                dashboardTimeLineSites: context.Forms.Data("DashboardPartTimeLineSites")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDashboardPartCalendarSitesDialog(Context context, ResponseCollection res)
        {
            res.Html("#DashboardPartCalendarSitesDialog", SiteUtilities.DashboardPartCalendarSitesDialog(
                context: context,
                ss: SiteSettings,
                dashboardPartId: context.Forms.Int("DashboardPartId"),
                dashboardCalendarSites: context.Forms.Data("DashboardPartCalendarSites")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDashboardPartKambanSitesDialog(Context context, ResponseCollection res)
        {
            res.Html("#DashboardPartKambanSitesDialog", SiteUtilities.DashboardPartKambanSitesDialog(
                context: context,
                ss: SiteSettings,
                dashboardPartId: context.Forms.Int("DashboardPartId"),
                dashboardKambanSites: context.Forms.Data("DashboardPartKambanSites")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDashboardPartIndexSitesDialog(Context context, ResponseCollection res)
        {
            res.Html("#DashboardPartIndexSitesDialog", SiteUtilities.DashboardPartIndexSitesDialog(
                context: context,
                ss: SiteSettings,
                dashboardPartId: context.Forms.Int("DashboardPartId"),
                dashboardIndexSites: context.Forms.Data("DashboardPartIndexSites")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDashboardPartDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewDashboardPart")
            {
                var dashboardPart = new DashboardPart();
                OpenDashboardPartDialog(
                    context: context,
                    res: res,
                    dashboardPart: dashboardPart);
            }
            else
            {
                var dashboardPart = SiteSettings.DashboardParts?.Get(context.Forms.Int("DashboardPartId"));
                if (dashboardPart == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context));
                }
                else
                {
                    SiteSettingsUtilities.Get(
                        context: context, siteModel: this, referenceId: SiteId);
                    OpenDashboardPartDialog(
                        context: context,
                        res: res,
                        dashboardPart: dashboardPart);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddDashboardPart(Context context, ResponseCollection res, string controlId)
        {
            var dashboardPart = DashboardPart.Create(
                context: context,
                id: SiteSettings.DashboardParts.MaxOrDefault(o => o.Id) + 1,
                title: context.Forms.Data("DashboardPartTitle"),
                showTitle: context.Forms.Bool("DashboardPartShowTitle"),
                type: context.Forms.Data("DashboardPartType").ToEnum<DashboardPartType>(),
                quickAccessSites: context.Forms.Data("DashboardPartQuickAccessSites"),
                quickAccessLayout: context.Forms.Data("DashboardPartQuickAccessLayout").ToEnum<QuickAccessLayout>(),
                timeLineSites: context.Forms.Data("DashboardPartTimeLineSites"),
                timeLineTitle: SiteSettings.LabelTextToColumnName(
                    text: context.Forms.Data("DashboardPartTimeLineTitle")),
                timeLineBody: SiteSettings.LabelTextToColumnName(
                    text: context.Forms.Data("DashboardPartTimeLineBody")),
                timeLineItemCount: context.Forms.Int("DashboardPartTimeLineItemCount"),
                content: context.Forms.Data("DashboardPartContent"),
                htmlContent: context.Forms.Data("DashboardPartHtmlContent"),
                timeLineDisplayType: context.Forms.Data("DashboardPartTimeLineDisplayType").ToEnum<TimeLineDisplayType>(),
                calendarSites: context.Forms.Data("DashboardPartCalendarSites"),
                calendarSitesData: context.Forms.Data("DashboardPartCalendarSites").Split(',').ToList(),
                calendarGroupBy: context.Forms.Data("DashboardPartCalendarGroupBy"),
                calendarTimePeriod: context.Forms.Data("DashboardPartCalendarTimePeriod"),
                calendarFromTo: context.Forms.Data("DashboardPartCalendarFromTo"),
                calendarShowStatus: context.Forms.Bool("CalendarShowStatus"),
                calendarType: context.Forms.Data("DashboardPartCalendarType").ToEnum<SiteSettings.CalendarTypes>(),
                kambanSites: context.Forms.Data("DashboardPartKambanSites"),
                kambanSitesData: context.Forms.Data("DashboardPartKambanSites").Split(',').ToList(),
                kambanGroupByX: context.Forms.Data("DashboardPartKambanGroupByX"),
                kambanGroupByY: context.Forms.Data("DashboardPartKambanGroupByY"),
                kambanAggregateType: context.Forms.Data("DashboardPartKambanAggregateType"),
                kambanValue: context.Forms.Data("DashboardPartKambanValue"),
                kambanColumns: context.Forms.Data("DashboardPartKambanColumns"),
                kambanAggregationView: context.Forms.Bool("DashboardPartKambanAggregationView"),
                kambanShowStatus: context.Forms.Bool("DashboardPartKambanShowStatus"),
                indexSites: context.Forms.Data("DashboardPartIndexSites"),
                indexSitesData: context.Forms.Data("DashboardPartIndexSites").Split(',').ToList(),
                extendedCss: context.Forms.Data("DashboardPartExtendedCss"),
                disableAsynchronousLoading: context.Forms.Bool("DisableAsynchronousLoading"),
                permissions: DashboardPartPermissions(context: context));
            SiteSettings.DashboardParts.Add(dashboardPart);
            res
                .ReplaceAll("#EditDashboardPart", new HtmlBuilder()
                    .EditDashboardPart(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static View GetDashboardPartView(Context context, SiteSettings ss, View view)
        {
            view = view ?? new View();
            if (ss == null)
            {
                return view;
            }
            view.SetByForm(
                context: context,
                ss: ss,
                prefix: "DashboardPart");
            return view;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateDashboardPart(Context context, ResponseCollection res)
        {
            var dashboardPart = SiteSettings.DashboardParts?
                .FirstOrDefault(o => o.Id == context.Forms.Int("DashboardPartId"));
            if (dashboardPart == null)
            {
                return;
            }
            dashboardPart.Update(
                context: context,
                title: context.Forms.Data("DashboardPartTitle"),
                showTitle: context.Forms.Bool("DashboardPartShowTitle"),
                type: context.Forms.Data("DashboardPartType").ToEnum<DashboardPartType>(),
                x: dashboardPart.X,
                y: dashboardPart.Y,
                width: dashboardPart.Width,
                height: dashboardPart.Height,
                quickAccessSites: context.Forms.Data("DashboardPartQuickAccessSites"),
                quickAccessLayout: context.Forms.Data("DashboardPartQuickAccessLayout").ToEnum<QuickAccessLayout>(),
                timeLineSites: context.Forms.Data("DashboardPartTimeLineSites"),
                timeLineTitle: SiteSettings.LabelTextToColumnName(
                    text: context.Forms.Data("DashboardPartTimeLineTitle")),
                timeLineBody: SiteSettings.LabelTextToColumnName(
                    text: context.Forms.Data("DashboardPartTimeLineBody")),
                timeLineItemCount: context.Forms.Int("DashboardPartTimeLineItemCount"),
                content: context.Forms.Data("DashboardPartContent"),
                htmlContent: context.Forms.Data("DashboardPartHtmlContent"),
                timeLineDisplayType: context.Forms.Data("DashboardPartTimeLineDisplayType").ToEnum<TimeLineDisplayType>(),
                calendarSites: context.Forms.Data("DashboardPartCalendarSites"),
                calendarSitesData: context.Forms.Data("DashboardPartCalendarSites").Split(',').ToList(),
                calendarGroupBy: context.Forms.Data("DashboardPartCalendarGroupBy"),
                calendarTimePeriod: context.Forms.Data("DashboardPartCalendarTimePeriod"),
                calendarFromTo: context.Forms.Data("DashboardPartCalendarFromTo"),
                calendarShowStatus: context.Forms.Bool("CalendarShowStatus"),
                calendarType: context.Forms.Data("DashboardPartCalendarType").ToEnum<SiteSettings.CalendarTypes>(),
                kambanSites: context.Forms.Data("DashboardPartKambanSites"),
                kambanSitesData: context.Forms.Data("DashboardPartKambanSites").Split(',').ToList(),
                kambanGroupByX: context.Forms.Data("DashboardPartKambanGroupByX"),
                kambanGroupByY: context.Forms.Data("DashboardPartKambanGroupByY"),
                kambanAggregateType: context.Forms.Data("DashboardPartKambanAggregateType"),
                kambanValue: context.Forms.Data("DashboardPartKambanValue"),
                kambanColumns: context.Forms.Data("DashboardPartKambanColumns"),
                kambanAggregationView: context.Forms.Bool("DashboardPartKambanAggregationView"),
                kambanShowStatus: context.Forms.Bool("DashboardPartKambanShowStatus"),
                indexSites: context.Forms.Data("DashboardPartIndexSites"),
                indexSitesData: context.Forms.Data("DashboardPartIndexSites").Split(',').ToList(),
                extendedCss: context.Forms.Data("DashboardPartExtendedCss"),
                disableAsynchronousLoading: context.Forms.Bool("DisableAsynchronousLoading"),
                permissions: DashboardPartPermissions(context: context));
            res
                .Html("#EditDashboardPart", new HtmlBuilder()
                    .EditDashboardPart(
                        context: context,
                        ss: SiteSettings))
                .CloseDialog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private List<Permission> DashboardPartPermissions(Context context)
        {
            return context.Forms.List("CurrentDashboardPartAccessControlAll")
                .Select(data => new Permission(
                    name: data.Split_1st(),
                    id: data.Split_2nd().ToInt(),
                    type: Permissions.Types.NotSet))
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SearchDashboardPartAccessControl(Context context, ResponseCollection res)
        {
            var process = SiteSettings.Processes.Get(context.Forms.Int("ProcessId"))
                ?? new Process();
            var currentPermissions = process.GetPermissions(ss: SiteSettings);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: SiteSettings,
                searchText: context.Forms.Data("SearchDashboardPartAccessControl"),
                currentPermissions: currentPermissions,
                allUsers: false);
            return res
                .Html("#SourceDashboardPartAccessControl", PermissionUtilities.PermissionListItem(
                    context: context,
                    ss: SiteSettings,
                    permissions: sourcePermissions.Page(0),
                    selectedValueTextCollection: context.Forms.Data("SourceDashboardPartAccessControl")
                        .Deserialize<List<string>>()?
                        .Where(o => o != string.Empty),
                    withType: false))
                .Val("#SourceDashboardPartAccessControlOffset", Parameters.Permissions.PageSize)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdatedashboardPartLayouts(Context context)
        {
            var layouts = context.Forms.Data("DashboardPartLayouts")
                .Deserialize<DashboardPartLayout[]>();
            foreach (var dashboardPart in SiteSettings.DashboardParts)
            {
                var layout = layouts.FirstOrDefault(o => o.Id == dashboardPart.Id);
                if (layout != null)
                {
                    dashboardPart.X = layout.X;
                    dashboardPart.Y = layout.Y;
                    dashboardPart.Width = layout.W;
                    dashboardPart.Height = layout.H;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void CopyDashboardPart(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditDashboardPart");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.DashboardParts.Copy(selected);
                res.ReplaceAll("#EditDashboardPart", new HtmlBuilder()
                    .EditDashboardPart(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void DeleteDashboardPart(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditDashboardPart");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                SiteSettings.DashboardParts.Delete(selected);
                res.ReplaceAll("#EditDashboardPart", new HtmlBuilder()
                    .EditDashboardPart(
                        context: context,
                        ss: SiteSettings));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateDashboardPartTimeLineSites(Context context, ResponseCollection res)
        {
            var savedTimeLineSites = context.Forms.Data("SavedDashboardPartTimeLineSites");
            var timeLineSites = context.Forms.Data("DashboardPartTimeLineSitesEdit");
            var savedSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                sitesString: savedTimeLineSites);
            var currentSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                sitesString: timeLineSites);
            if (currentSs == null || currentSs.SiteId == 0)
            {
                res.Message(
                    message: new Message(
                        id: "InvalidTimeLineSites",
                        text: Displays.InvalidTimeLineSites(context: context),
                        css: "alert-error"),
                    target: "#DashboardPartTimeLineSitesMessage");
            }
            else if (savedSs == null || savedSs?.SiteId == 0 || savedSs?.SiteId == currentSs?.SiteId)
            {
                res
                    .Set(
                        target: "#DashboardPartTimeLineSites",
                        value: timeLineSites)
                    .Set(
                        target: "#DashboardPartBaseSiteId",
                        value: currentSs.SiteId)
                    .Add(
                        method: "SetValue",
                        target: "#DashboardPartTimeLineSitesValue",
                        value: timeLineSites)
                    .CloseDialog(
                        target: "#DashboardPartTimeLineSitesDialog");
                if (savedSs == null || savedSs?.SiteId == 0)
                {
                    ClearDashboardView(context: context, res: res);
                }
            }
            else
            {
                res
                    .Invoke(
                        methodName: "confirmTimeLineSites",
                        args: new
                        {
                            timeLineSites,
                            baseSiteId = currentSs.SiteId
                        }.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateDashboardPartCalendarSites(Context context, ResponseCollection res)
        {
            var savedCalendarSites = context.Forms.Data("SavedDashboardPartCalendarSites");
            var calendarSites = context.Forms.Data("DashboardPartCalendarSitesEdit");
            var savedSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                sitesString: savedCalendarSites);
            var currentSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                sitesString: calendarSites);
            if (currentSs == null || currentSs.SiteId == 0)
            {
                res.Message(
                    message: new Message(
                        id: "InvalidCalendarSites",
                        text: Displays.InvalidTimeLineSites(context: context),
                        css: "alert-error"),
                    target: "#DashboardPartCalendarSitesMessage");
            }
            else if (savedSs == null || savedSs?.SiteId == 0 || savedSs?.SiteId == currentSs?.SiteId)
            {
                res
                    .Set(
                        target: "#DashboardPartCalendarSites",
                        value: calendarSites)
                    .Set(
                        target: "#DashboardPartCalendarBaseSiteId",
                        value: currentSs.SiteId)
                    .Add(
                        method: "SetValue",
                        target: "#DashboardPartCalendarSitesValue",
                        value: calendarSites)
                    .CloseDialog(
                        target: "#DashboardPartCalendarSitesDialog");
                if (savedSs == null || savedSs?.SiteId == 0)
                {
                    ClearDashboardCalendarView(context: context, res: res);
                }
            }
            else
            {
                res
                    .Html(
                        target: "#DashboardPartCalendarGroupBy",
                        value: new HtmlBuilder()
                            .OptionCollection(
                                context: context,
                                optionCollection: currentSs.CalendarGroupByOptions(context: context)?.ToDictionary(
                                o => o.Key, o => new ControlData(o.Value)),
                                insertBlank: true))
                    .Html(
                        target: "#DashboardPartCalendarFromTo",
                        value: new HtmlBuilder()
                            .OptionCollection(
                                context: context,
                                optionCollection: currentSs.CalendarColumnOptions(context: context)?.ToDictionary(
                                o => o.Key, o => new ControlData(o.Value))))
                    .Invoke(
                        methodName: "confirmCalendarSites",
                        args: new
                        {
                            calendarSites,
                            baseSiteId = currentSs.SiteId
                        }.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateDashboardPartKambanSites(Context context, ResponseCollection res)
        {
            var savedKambanSites = context.Forms.Data("SavedDashboardPartKambanSites");
            var kambanSites = context.Forms.Data("DashboardPartKambanSitesEdit");
            var savedSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                sitesString: savedKambanSites);
            var currentSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                sitesString: kambanSites);
            if (currentSs == null || currentSs.SiteId == 0)
            {
                res.Message(
                    message: new Message(
                        id: "InvalidKambanSites",
                        text: Displays.InvalidTimeLineSites(context: context),
                        css: "alert-error"),
                    target: "#DashboardPartKambanSitesMessage");
            }
            else if (savedSs == null || savedSs?.SiteId == 0 || savedSs?.SiteId == currentSs?.SiteId)
            {
                res
                    .Set(
                        target: "#DashboardPartKambanSites",
                        value: kambanSites)
                    .Set(
                        target: "#DashboardPartKambanBaseSiteId",
                        value: currentSs.SiteId)
                    .Add(
                        method: "SetValue",
                        target: "#DashboardPartKambanSitesValue",
                        value: kambanSites)
                    .CloseDialog(
                        target: "#DashboardPartKambanSitesDialog");
                if (savedSs == null || savedSs?.SiteId == 0)
                {
                    ClearDashboardKambanView(context: context, res: res);
                }
            }
            else
            {
                res
                    .Invoke(
                        methodName: "confirmKambanSites",
                        args: new
                        {
                            kambanSites,
                            baseSiteId = currentSs.SiteId
                        }.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateDashboardPartIndexSites(Context context, ResponseCollection res)
        {
            var savedIndexSites = context.Forms.Data("SavedDashboardPartIndexSites");
            var indexSites = context.Forms.Data("DashboardPartIndexSitesEdit");
            var savedSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                sitesString: savedIndexSites);
            var currentSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                sitesString: indexSites);
            if (currentSs == null || currentSs.SiteId == 0)
            {
                res.Message(
                    message: new Message(
                        id: "InvalidIndexSites",
                        text: Displays.InvalidTimeLineSites(context: context),
                        css: "alert-error"),
                    target: "#DashboardPartIndexSitesMessage");
            }
            else if (savedSs == null || savedSs?.SiteId == 0 || savedSs?.SiteId == currentSs?.SiteId)
            {
                res
                    .Set(
                        target: "#DashboardPartIndexSites",
                        value: indexSites)
                    .Set(
                        target: "#DashboardPartIndexBaseSiteId",
                        value: currentSs.SiteId)
                    .Add(
                        method: "SetValue",
                        target: "#DashboardPartIndexSitesValue",
                        value: indexSites)
                    .CloseDialog(
                        target: "#DashboardPartIndexSitesDialog");
                if (savedSs == null || savedSs?.SiteId == 0)
                {
                    ClearDashboardIndexView(context: context, res: res);
                }
            }
            else
            {
                res.Invoke(
                    methodName: "confirmIndexSites",
                    args: new
                    {
                        indexSites,
                        baseSiteId = currentSs.SiteId
                    }.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ClearDashboardView(Context context, ResponseCollection res)
        {
            var currentSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                context.Forms.Data("DashboardPartTimeLineSitesEdit"));
            if (currentSs == null)
            {
                res.Message(
                    new Message(
                        "InvalidTimeLineSites",
                        Displays.InvalidTimeLineSites(context: context),
                        "alert-error"));
                return;
            }
            var dashboardPart = SiteSettings.DashboardParts?
                .FirstOrDefault(o => o.Id == context.Forms.Int("DashboardPartId"));
            if (dashboardPart != null)
            {
                dashboardPart.View = new View();
            }
            res
                .Html(
                    "#DashboardPartViewFiltersTabContainer",
                    new HtmlBuilder()
                        .ViewFiltersTab(
                            context: context,
                            ss: currentSs,
                            view: new View(),
                            prefix: "DashboardPart",
                            currentTableOnly: true))
                .Html(
                    "#DashboardPartViewSortersTabContainer",
                    new HtmlBuilder()
                        .ViewSortersTab(
                            context: context,
                            ss: currentSs,
                            view: new View(),
                            prefix: "DashboardPart",
                            usekeepSorterState: false,
                            currentTableOnly: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ClearDashboardCalendarView(Context context, ResponseCollection res)
        {
            var currentSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                context.Forms.Data("DashboardPartCalendarSitesEdit"));
            if (currentSs == null)
            {
                res.Message(
                   new Message(
                       "InvalidCalendarSites",
                       Displays.InvalidTimeLineSites(context: context),
                       "alert-error"));
                return;
            }
            var dashboardPart = SiteSettings.DashboardParts?
                .FirstOrDefault(o => o.Id == context.Forms.Int("DashboardPartId"));
            if (dashboardPart != null)
            {
                dashboardPart.View = new View();
            }
            res
                .Html(
                    "#DashboardPartViewFiltersTabContainer",
                    new HtmlBuilder()
                        .ViewFiltersTab(
                            context: context,
                            ss: currentSs,
                            view: new View(),
                            prefix: "DashboardPart",
                            currentTableOnly: true))
                .Html(
                    target: "#DashboardPartCalendarGroupBy",
                    value: new HtmlBuilder()
                        .OptionCollection(
                            context: context,
                            optionCollection: currentSs.CalendarGroupByOptions(context: context)?.ToDictionary(
                            o => o.Key, o => new ControlData(o.Value)),
                            insertBlank: true))
                .Html(
                    target: "#DashboardPartCalendarFromTo",
                    value: new HtmlBuilder()
                        .OptionCollection(
                            context: context,
                            optionCollection: currentSs.CalendarColumnOptions(context: context)?.ToDictionary(
                            o => o.Key, o => new ControlData(o.Value))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ClearDashboardKambanView(Context context, ResponseCollection res)
        {
            var currentSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                context.Forms.Data("DashboardPartKambanSitesEdit"));
            var kambanGroupByOptions = currentSs.KambanGroupByOptions(
                context: context,
                addNothing: true)?
                    .ToDictionary(o => o.Key, o => new ControlData(o.Value));
            if (currentSs == null)
            {
                res.Message(
                   new Message(
                       "InvalidKambanSites",
                       Displays.InvalidTimeLineSites(context: context),
                       "alert-error"));
                return;
            }
            var dashboardPart = SiteSettings.DashboardParts?
                .FirstOrDefault(o => o.Id == context.Forms.Int("DashboardPartId"));
            if (dashboardPart != null)
            {
                dashboardPart.View = new View();
            }
            res
                .Html(
                    "#DashboardPartViewFiltersTabContainer",
                    new HtmlBuilder()
                        .ViewFiltersTab(
                            context: context,
                            ss: currentSs,
                            view: new View(),
                            prefix: "DashboardPart",
                            currentTableOnly: true))
                .Html(
                    target: "#DashboardPartKambanGroupByX",
                    value: new HtmlBuilder()
                        .OptionCollection(
                            context: context,
                            optionCollection: kambanGroupByOptions,
                            selectedValue: kambanGroupByOptions?.ContainsKey("Status") == true
                                ? "Status"
                                : null))
                .Html(
                    target: "#DashboardPartKambanGroupByY",
                    value: new HtmlBuilder()
                        .OptionCollection(
                            context: context,
                            optionCollection: kambanGroupByOptions,
                            selectedValue: kambanGroupByOptions?.ContainsKey("Owner") == true
                                ? "Owner"
                                : null))
                .Html(
                    target: "#DashboardPartKambanValue",
                    value: new HtmlBuilder()
                        .OptionCollection(
                            context: context,
                            optionCollection: currentSs.KambanValueOptions(context: context)?.ToDictionary(
                            o => o.Key, o => new ControlData(o.Value)),
                            selectedValue: currentSs.ReferenceType == "Issues" ? "RemainingWorkValue" : "NumA"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ClearDashboardIndexView(Context context, ResponseCollection res)
        {
            var currentSs = DashboardPart.GetBaseSiteSettings(
                context: context,
                context.Forms.Data("DashboardPartIndexSitesEdit"));
            if (currentSs == null)
            {
                res.Message(
                    new Message(
                        "InvalidIndexSites",
                        Displays.InvalidTimeLineSites(context: context),
                        "alert-error"));
                return;
            }
            var dashboardPart = SiteSettings.DashboardParts?
                .FirstOrDefault(o => o.Id == context.Forms.Int("DashboardPartId"));
            if (dashboardPart != null)
            {
                dashboardPart.View = new View();
            }
            res
                .Html(
                    "#DashboardPartViewGridTabContainer",
                    new HtmlBuilder()
                        .ViewGridTab(
                            context: context,
                            ss: currentSs,
                            view: new View(),
                            prefix: "DashboardPart",
                            hasNotInner: true))
                .Html(
                    "#DashboardPartViewFiltersTabContainer",
                    new HtmlBuilder()
                        .ViewFiltersTab(
                            context: context,
                            ss: currentSs,
                            view: new View(),
                            prefix: "DashboardPart",
                            currentTableOnly: true))
                .Html(
                    "#DashboardPartViewSortersTabContainer",
                    new HtmlBuilder()
                        .ViewSortersTab(
                            context: context,
                            ss: currentSs,
                            view: new View(),
                            prefix: "DashboardPart",
                            usekeepSorterState: false,
                            currentTableOnly: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string OpenSetNumericRangeDialog(
            Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.SetPermissions(
                context: context,
                referenceId: SiteId);
            if (context.CanRead(SiteSettings))
            {
                var columnName = controlId
                    .Substring(controlId.IndexOf("__") + 2)
                    .Replace("_NumericRange", string.Empty);
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                return res.Html(
                    target: "#SetNumericRangeDialog",
                    value: new HtmlBuilder().SetNumericRangeDialog(
                        context: context,
                        ss: SiteSettings,
                        column: column))
                            .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string OpenSetDateRangeDialog(
            Context context, ResponseCollection res, string controlId)
        {
            SiteSettings.SetPermissions(
                context: context,
                referenceId: SiteId);
            if (context.CanRead(SiteSettings))
            {
                var columnName = controlId
                    .Substring(controlId.IndexOf("__") + 2)
                    .Replace("_DateRange", string.Empty);
                var column = SiteSettings.GetColumn(
                    context: context,
                    columnName: columnName);
                return res.Html(
                    target: "#SetDateRangeDialog",
                    value: new HtmlBuilder().SetDateRangeDialog(
                        context: context,
                        ss: SiteSettings,
                        column: column))
                            .ToJson();
            }
            else
            {
                return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool WithinApiLimits(Context context)
        {
            var limit = context.ContractSettings.ApiLimit();
            var reset = false;
            var beforeResetCount = ApiCount;
            if (limit > 0)
            {
                var today = DateTime.Now.ToDateTime().ToLocal(context: context).Date;
                if (ApiCountDate.Date < today)
                {
                    ApiCountDate = today;
                    ApiCount = 0;
                    reset = true;
                }
                ApiCount++;
                if (ApiCount > limit)
                {
                    return false;
                }
                UpdateApiCount(context: context);
                if (reset)
                {
                    LogApiCountReset(
                        context: context,
                        beforeResetCount: beforeResetCount,
                        apiCount: ApiCount);
                }
                return true;
            }
            return true;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateApiCount(Context context)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateSites(
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(SiteId),
                    param: Rds.SitesParam()
                        .ApiCountDate(ApiCountDate)
                        .ApiCount(ApiCount),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void LogApiCountReset(Context context, int beforeResetCount, int apiCount)
        {
            new SysLogModel(
                context: context,
                method: nameof(LogApiCountReset),
                message: Displays.ApiCountReset(
                    context: context,
                    new string[]
                    {
                        beforeResetCount.ToString(),
                        apiCount.ToString()
                    }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public List<SqlStatement> GetReminderSchedulesStatements(Context context)
        {
            var statements = new List<SqlStatement>();
            statements.Add(Rds.PhysicalDeleteReminderSchedules(
                where: Rds.ReminderSchedulesWhere()
                    .SiteId(SiteId)));
            SiteSettings.Reminders?.ForEach(reminder =>
                statements.Add(Rds.UpdateOrInsertReminderSchedules(
                    param: Rds.ReminderSchedulesParam()
                        .SiteId(SiteId)
                        .Id(reminder.Id)
                        .ScheduledTime(reminder.StartDateTime.Next(
                            context: context,
                            type: reminder.Type)),
                    where: Rds.ReminderSchedulesWhere()
                        .SiteId(SiteId)
                        .Id(reminder.Id))));
            return statements;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ChangeFormulaCalculationMethod(Context context, ResponseCollection res)
        {
            if (!context.CanUpdate(ss: SiteSettings))
            {
                res.Message(Messages.HasNotPermission(context: context)).ToJson();
            }
            else
            {
                res
                    .Html(
                        "#FormulaTarget",
                        new HtmlBuilder().FormulaCalculationMethod(
                            context: context,
                            ss: SiteSettings,
                            target: context.Forms.Data("CalculationMethod")))
                    .ClearFormData()
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private List<Permission> ParsePermissions(ApiSiteSettingPermission apiSettingPermission, SiteSettings ss, object target = null)
        {
            var permissions = new List<Permission>();
            if (apiSettingPermission == null) {
                return permissions;
            }
            apiSettingPermission.Users?.ForEach(id => permissions.Add(new Permission(
                ss: ss,
                name: "User",
                id: id)));
            apiSettingPermission.Groups?.ForEach(id => permissions.Add(new Permission(
                ss: ss,
                name: "Group",
                id: id)));
            apiSettingPermission.Depts?.ForEach(id => permissions.Add(new Permission(
                ss: ss,
                name: "Dept",
                id: id)));
            switch (target)
            {
                case Process process when target.GetType().Name == nameof(Process):
                    if (process.Users != null && apiSettingPermission.Users == null) {
                        process.Users.ForEach(id => permissions.Add(new Permission(
                            ss: ss,
                            name: "User",
                            id: id)));
                    }
                    if (process.Depts != null && apiSettingPermission.Depts == null)
                    {
                        process.Depts.ForEach(id => permissions.Add(new Permission(
                            ss: ss,
                            name: "Dept",
                            id: id)));
                    }
                    if (process.Groups != null && apiSettingPermission.Groups == null)
                    {
                        process.Groups.ForEach(id => permissions.Add(new Permission(
                            ss: ss,
                            name: "Group",
                            id: id)));
                    }
                    break;
                case StatusControl statusControll when target.GetType().Name == nameof(StatusControl):
                    if (statusControll.Users != null && apiSettingPermission.Users == null)
                    {
                        statusControll.Users.ForEach(id => permissions.Add(new Permission(
                            ss: ss,
                            name: "User",
                            id: id)));
                    }
                    if (statusControll.Depts != null && apiSettingPermission.Depts == null)
                    {
                        statusControll.Depts.ForEach(id => permissions.Add(new Permission(
                            ss: ss,
                            name: "Dept",
                            id: id)));
                    }
                    if (statusControll.Groups != null && apiSettingPermission.Groups == null)
                    {
                        statusControll.Groups.ForEach(id => permissions.Add(new Permission(
                            ss: ss,
                            name: "Group",
                            id: id)));
                    }
                    break;
            }
            return permissions;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SettingList<ValidateInput> ParseValidateInputs(SettingList<ValidateInput> validateInputs, Process process)
        {
            var data = new SettingList<ValidateInput>();
            if (validateInputs == null) {
                return null;
            }
            validateInputs.ForEach(o => {
                if (o.Delete != 1)
                {
                    data.Add(o);
                }
            });
            if (process?.ValidateInputs != null) {
                var requestIds = validateInputs.Select(o => o.Id).ToArray();
                process.ValidateInputs.ForEach(o => {
                    if (!requestIds.Contains(o.Id))
                    {
                        data.Add(o);
                    }
                });
            }
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SettingList<DataChange> ParseDataChanges(SettingList<DataChange> dataChanges, Process process)
        {
            var data = new SettingList<DataChange>();
            if (dataChanges == null)
            {
                return null;
            }
            dataChanges.ForEach(o => {
                if (o.Delete != 1)
                {
                    data.Add(o);
                }
            });
            if (process?.DataChanges != null)
            {
                var requestIds = dataChanges.Select(o => o.Id).ToArray();
                process.DataChanges.ForEach(o => {
                    if (!requestIds.Contains(o.Id))
                    {
                        data.Add(o);
                    }
                });
            }
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SettingList<Notification> ParseNotifications(SettingList<Notification> notifications, Process process)
        {
            var data = new SettingList<Notification>();
            if (notifications == null)
            {
                return null;
            }
            notifications.ForEach(o => {
                if (o.Delete != 1)
                {
                    data.Add(o);
                }
            });
            if (process?.Notifications != null)
            {
                var requestIds = notifications.Select(o => o.Id).ToArray();
                process.Notifications.ForEach(o => {
                    if (!requestIds.Contains(o.Id))
                    {
                        data.Add(o);
                    }
                });
            }
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenSearchEditorColumnDialog(
            Context context,
            ResponseCollection res)
        {
            switch (context.Forms.Data("EditorSourceColumnsType"))
            {
                case "Links":
                case "Others":
                    res.Message(Messages.CanNotPerformed(context: context));
                    break;
                case "Columns":
                    res.Html("#SearchEditorColumnDialog", SiteUtilities.SearchEditorColumnDialog(
                        context: context,
                        ss: SiteSettings));
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void FilterSourceColumnsSelectable(
            Context context,
            ResponseCollection res)
        {
            AddOrUpdateEditorColumnHash(context: context);
            SiteUtilities.FilterSourceColumnsSelectable(res, context, SiteSettings)
            .SetData("#EditorSourceColumns")
            .CloseDialog();
        }
    }
}
