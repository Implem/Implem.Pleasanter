using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Dynamic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelApiModel : DynamicObject
    {
        private static readonly string[] MethodNames = new string[]
        {
            "Create",
            "Delete",
            "Get",
            "New",
            "Update",
        };

        private readonly Context Context;
        public BaseItemModel Model;
        private readonly bool OnTesting;

        public ServerScriptModelApiModel(Context context, BaseItemModel model, bool onTesting)
        {
            Context = context;
            Model = model ?? new BaseItemModel();
            OnTesting = onTesting;
        }

        public override bool TryGetIndex(
            GetIndexBinder binder,
            object[] indexes,
            out object result)
        {
            if (indexes.Length == 1)
            {
                var name = indexes[0].ToString();
                if (MethodNames.Contains(name))
                {
                    result = null;
                    return false;
                }
                switch (Def.ExtendedColumnTypes.Get(name ?? string.Empty))
                {
                    case "Class":
                        result = Model.GetClass(name);
                        return true;
                    case "Num":
                        result = Model.GetNum(name).Value;
                        return true;
                    case "Date":
                        result = Model.GetDate(name);
                        return true;
                    case "Description":
                        result = Model.GetDescription(name);
                        return true;
                    case "Check":
                        result = Model.GetCheck(name);
                        return true;
                    case "Attachments":
                        result = Model.GetAttachments(name);
                        return true;
                }
                switch (name)
                {
                    case nameof(BaseItemModel.Ver):
                        result = Model.Ver;
                        return true;
                    case nameof(BaseItemModel.Creator):
                        result = Model.Creator.Id;
                        return true;
                    case nameof(BaseItemModel.Updator):
                        result = Model.Updator.Id;
                        return true;
                    case nameof(BaseItemModel.CreatedTime):
                        result = Model.CreatedTime.Value;
                        return true;
                    case nameof(BaseItemModel.UpdatedTime):
                        result = Model.UpdatedTime.Value;
                        return true;
                    case nameof(BaseItemModel.VerUp):
                        result = Model.VerUp;
                        return true;
                    case nameof(BaseItemModel.SiteId):
                        result = Model.SiteId;
                        return true;
                    case nameof(BaseItemModel.Body):
                        result = Model.Body;
                        return true;
                    case nameof(BaseItemModel.Title):
                        result = Model.Title.Value;
                        return true;
                }
                if (Model is SiteModel siteModel)
                {
                    switch (name)
                    {
                        case nameof(siteModel.SiteId):
                            result = siteModel.SiteId;
                            return true;
                        case nameof(siteModel.SiteName):
                            result = siteModel.SiteName;
                            return true;
                        case nameof(siteModel.SiteGroupName):
                            result = siteModel.SiteGroupName;
                            return true;
                        case nameof(siteModel.GridGuide):
                            result = siteModel.GridGuide;
                            return true;
                        case nameof(siteModel.EditorGuide):
                            result = siteModel.EditorGuide;
                            return true;
                        case nameof(siteModel.CalendarGuide):
                            result = siteModel.CalendarGuide;
                            return true;
                        case nameof(siteModel.CrosstabGuide):
                            result = siteModel.CrosstabGuide;
                            return true;
                        case nameof(siteModel.GanttGuide):
                            result = siteModel.GanttGuide;
                            return true;
                        case nameof(siteModel.BurnDownGuide):
                            result = siteModel.BurnDownGuide;
                            return true;
                        case nameof(siteModel.TimeSeriesGuide):
                            result = siteModel.TimeSeriesGuide;
                            return true;
                        case nameof(siteModel.AnalyGuide):
                            result = siteModel.AnalyGuide;
                            return true;
                        case nameof(siteModel.KambanGuide):
                            result = siteModel.KambanGuide;
                            return true;
                        case nameof(siteModel.ImageLibGuide):
                            result = siteModel.ImageLibGuide;
                            return true;
                        case nameof(siteModel.ReferenceType):
                            result = siteModel.ReferenceType;
                            return true;
                        case nameof(siteModel.ParentId):
                            result = siteModel.ParentId;
                            return true;
                        case nameof(siteModel.InheritPermission):
                            result = siteModel.InheritPermission;
                            return true;
                        case nameof(siteModel.SiteSettings):
                            result = siteModel.SiteSettings;
                            return true;
                        case nameof(siteModel.Publish):
                            result = siteModel.Publish;
                            return true;
                        case nameof(siteModel.DisableCrossSearch):
                            result = siteModel.DisableCrossSearch;
                            return true;
                        case nameof(siteModel.LockedTime):
                            result = siteModel.LockedTime;
                            return true;
                        case nameof(siteModel.LockedUser):
                            result = siteModel.LockedUser;
                            return true;
                        case nameof(siteModel.ApiCountDate):
                            result = siteModel.ApiCountDate;
                            return true;
                        case nameof(siteModel.ApiCount):
                            result = siteModel.ApiCount;
                            return true;
                    }
                }
                if (Model is IssueModel issueModel)
                {
                    switch (name)
                    {
                        case nameof(IssueModel.IssueId):
                            result = issueModel.IssueId;
                            return true;
                        case nameof(IssueModel.StartTime):
                            result = issueModel.StartTime;
                            return true;
                        case nameof(IssueModel.CompletionTime):
                            result = issueModel.CompletionTime.Value;
                            return true;
                        case nameof(IssueModel.WorkValue):
                            result = issueModel.WorkValue.Value;
                            return true;
                        case nameof(IssueModel.ProgressRate):
                            result = issueModel.ProgressRate.Value;
                            return true;
                        case nameof(IssueModel.RemainingWorkValue):
                            result = issueModel.RemainingWorkValue;
                            return true;
                        case nameof(IssueModel.Status):
                            result = issueModel.Status.Value;
                            return true;
                        case nameof(IssueModel.Manager):
                            result = issueModel.Manager.Id;
                            return true;
                        case nameof(IssueModel.Owner):
                            result = issueModel.Owner.Id;
                            return true;
                        case nameof(IssueModel.Locked):
                            result = issueModel.Locked;
                            return true;
                        case nameof(IssueModel.ReadOnly):
                            result = issueModel.ReadOnly;
                            return true;
                    }
                }
                else if (Model is ResultModel resultModel)
                {
                    switch (name)
                    {
                        case nameof(ResultModel.ResultId):
                            result = resultModel.ResultId;
                            return true;
                        case nameof(ResultModel.Status):
                            result = resultModel.Status.Value;
                            return true;
                        case nameof(ResultModel.Manager):
                            result = resultModel.Manager.Id;
                            return true;
                        case nameof(ResultModel.Owner):
                            result = resultModel.Owner.Id;
                            return true;
                        case nameof(ResultModel.Locked):
                            result = resultModel.Locked;
                            return true;
                        case nameof(ResultModel.ReadOnly):
                            result = resultModel.ReadOnly;
                            return true;
                    }
                }
            }
            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return base.TrySetIndex(binder, indexes, value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name;
            Model.SetValue(
                context: Context,
                column: new Column(name),
                value: value.ToStr());
            switch (name)
            {
                case nameof(BaseModel.VerUp):
                    Model.VerUp = value.ToBool();
                    return true;
                case nameof(BaseItemModel.Body):
                    Model.Body = value.ToStr();
                    return true;
                case nameof(BaseItemModel.Title):
                    Model.Title.Value = value.ToStr();
                    return true;
            }
            if (Model is SiteModel siteModel)
            {
                switch (name)
                {
                    case nameof(siteModel.SiteName):
                        siteModel.SiteName = value.ToStr();
                        return true;
                    case nameof(siteModel.SiteGroupName):
                        siteModel.SiteGroupName = value.ToStr();
                        return true;
                    case nameof(SiteModel.GridGuide):
                        siteModel.GridGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.EditorGuide):
                        siteModel.EditorGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.CalendarGuide):
                        siteModel.CalendarGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.CrosstabGuide):
                        siteModel.CrosstabGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.GanttGuide):
                        siteModel.GanttGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.BurnDownGuide):
                        siteModel.BurnDownGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.TimeSeriesGuide):
                        siteModel.TimeSeriesGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.AnalyGuide):
                        siteModel.AnalyGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.KambanGuide):
                        siteModel.KambanGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.ImageLibGuide):
                        siteModel.ImageLibGuide = value.ToStr();
                        return true;
                    case nameof(SiteModel.ReferenceType):
                        siteModel.ReferenceType = value.ToStr();
                        return true;
                    case nameof(SiteModel.ParentId):
                        siteModel.ParentId = value.ToInt();
                        return true;
                    case nameof(SiteModel.InheritPermission):
                        siteModel.InheritPermission = value.ToInt();
                        return true;
                    case nameof(SiteModel.SiteSettings):
                        siteModel.SiteSettings = value.ToStr().Deserialize<SiteSettings>() ?? new SiteSettings();
                        siteModel.SiteSettings.Init(context: Context);
                        return true;
                    case nameof(SiteModel.Publish):
                        siteModel.Publish = value.ToBool();
                        return true;
                    case nameof(SiteModel.DisableCrossSearch):
                        siteModel.DisableCrossSearch = value.ToBool();
                        return true;
                    case nameof(SiteModel.LockedTime):
                        siteModel.LockedTime = new DataTypes.Time();
                        return true;
                    case nameof(SiteModel.LockedUser):
                        siteModel.LockedUser = SiteInfo.User(
                            context: Context,
                            userId: value.ToInt());
                        return true;
                    case nameof(SiteModel.ApiCountDate):
                        siteModel.ApiCountDate = value.ToDateTime();
                        return true;
                    case nameof(SiteModel.ApiCount):
                        siteModel.ApiCount = value.ToInt();
                        return true;
                }
            }
            if (Model is IssueModel issueModel)
            {
                switch (name)
                {
                    case nameof(IssueModel.StartTime):
                        issueModel.StartTime = Date(value);
                        return true;
                    case nameof(IssueModel.CompletionTime):
                        issueModel.CompletionTime.Value = Date(value);
                        issueModel.CompletionTime.DisplayValue = Date(value).ToLocal(context: Context);
                        return true;
                    case nameof(IssueModel.WorkValue):
                        issueModel.WorkValue.Value = value.ToDecimal();
                        return true;
                    case nameof(IssueModel.ProgressRate):
                        issueModel.ProgressRate.Value = value.ToDecimal();
                        return true;
                    case nameof(IssueModel.Status):
                        issueModel.Status.Value = value.ToInt();
                        return true;
                    case nameof(IssueModel.Manager):
                        issueModel.Manager = SiteInfo.User(
                            context: Context,
                            userId: value.ToInt());
                        return true;
                    case nameof(IssueModel.Owner):
                        issueModel.Owner = SiteInfo.User(
                            context: Context,
                            userId: value.ToInt());
                        return true;
                    case nameof(IssueModel.Locked):
                        issueModel.Locked = value.ToBool();
                        return true;
                    case nameof(IssueModel.ReadOnly):
                        issueModel.ReadOnly = value.ToBool();
                        return true;
                }
            }
            else if (Model is ResultModel resultModel)
            {
                switch (name)
                {
                    case nameof(ResultModel.Status):
                        resultModel.Status.Value = value.ToInt();
                        return true;
                    case nameof(ResultModel.Manager):
                        resultModel.Manager = SiteInfo.User(
                            context: Context,
                            userId: value.ToInt());
                        return true;
                    case nameof(ResultModel.Owner):
                        resultModel.Owner = SiteInfo.User(
                            context: Context,
                            userId: value.ToInt());
                        return true;
                    case nameof(ResultModel.Locked):
                        resultModel.Locked = value.ToBool();
                        return true;
                    case nameof(ResultModel.ReadOnly):
                        resultModel.ReadOnly = value.ToBool();
                        return true;
                }
            }
            return true;
        }

        private static DateTime Date(object value)
        {
            return value is DateTime dateTime
                ? TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local)
                : Types.ToDateTime(0);
        }

        public string ToJsonString(Context context, SiteSettings ss)
        {
            if (Model.CreatedTime == null)
            {
                Model.CreatedTime = new DataTypes.Time();
            }
            if (Model.UpdatedTime == null)
            {
                Model.UpdatedTime = new DataTypes.Time();
            }
            if (Model is SiteModel siteModel)
            {
                var apiModel = siteModel.GetByApi(context: context);
                apiModel.Comments = null;
                return apiModel.ToJson();
            }
            else if (Model is IssueModel issueModel)
            {
                var apiModel = issueModel.GetByApi(
                    context: context,
                    ss: ss);
                apiModel.Comments = null;
                return apiModel.ToJson();
            }
            else if (Model is ResultModel resultModel)
            {
                var apiModel = resultModel.GetByApi(
                    context: context,
                    ss: ss);
                apiModel.Comments = null;
                return apiModel.ToJson();
            }
            else if (Model is WikiModel wikiModel)
            {
                var apiModel = wikiModel.GetByApi(
                    context: context,
                    ss: ss);
                apiModel.Comments = null;
                return apiModel.ToJson();
            }
            return null;
        }

        public bool Create(object siteId)
        {
            var serverScript = new ServerScriptModelApiItems(
                context: Context,
                onTesting: OnTesting);
            return serverScript.Create(siteId, this);
        }

        public bool Update()
        {
            var serverScript = new ServerScriptModelApiItems(context: Context, onTesting: OnTesting);
            if (Model is SiteModel siteModel)
            {
                return serverScript.Update(siteModel.SiteId, this);
            }
            else if (Model is IssueModel issueModel)
            {
                return serverScript.Update(issueModel.IssueId, this);
            }
            else if (Model is ResultModel resultModel)
            {
                return serverScript.Update(resultModel.ResultId, this);
            }
            else if (Model is WikiModel wikiModel)
            {
                return serverScript.Update(wikiModel.WikiId, this);
            }
            else
            {
                return false;
            }
        }

        public bool Delete()
        {
            var serverScript = new ServerScriptModelApiItems(context: Context, onTesting: OnTesting);
            if (Model is SiteModel siteModel)
            {
                return serverScript.Delete(siteModel.SiteId);
            }
            else if (Model is IssueModel issueModel)
            {
                return serverScript.Delete(issueModel.IssueId);
            }
            else if (Model is ResultModel resultModel)
            {
                return serverScript.Delete(resultModel.ResultId);
            }
            else if (Model is WikiModel wikiModel)
            {
                return serverScript.Delete(wikiModel.WikiId);
            }
            else
            {
                return false;
            }
        }
    }
}