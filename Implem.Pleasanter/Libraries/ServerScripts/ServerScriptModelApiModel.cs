using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Dynamic;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
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
        private readonly BaseItemModel Model;
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
                switch (Def.ExtendedColumnTypes.Get(name))
                {
                    case "Class":
                        result = Model.Class(name);
                        return true;
                    case "Num":
                        result = Model.Num(name);
                        return true;
                    case "Date":
                        result = Model.Date(name);
                        return true;
                    case "Description":
                        result = Model.Description(name);
                        return true;
                    case "Check":
                        result = Model.Check(name);
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
            Model.Value(
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
            if (Model is IssueModel issueModel)
            {
                switch (name)
                {
                    case nameof(IssueModel.StartTime):
                        issueModel.StartTime = value.ToDateTime();
                        return true;
                    case nameof(IssueModel.CompletionTime):
                        issueModel.CompletionTime.Value = value.ToDateTime();
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
            if (Model is IssueModel issueModel)
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
            return null;
        }

        public bool Create()
        {
            var serverScript = new ServerScriptModelApiItems(context: Context, onTesting: OnTesting);
            if (Model is IssueModel issueModel)
            {
                return serverScript.Create(issueModel.IssueId, this);
            }
            else if (Model is ResultModel resultModel)
            {
                return serverScript.Create(resultModel.ResultId, this);
            }
            else
            {
                return false;
            }
        }

        public bool Update()
        {
            var serverScript = new ServerScriptModelApiItems(context: Context, onTesting: OnTesting);
            if (Model is IssueModel issueModel)
            {
                return serverScript.Update(issueModel.IssueId, this);
            }
            else if (Model is ResultModel resultModel)
            {
                return serverScript.Update(resultModel.ResultId, this);
            }
            else
            {
                return false;
            }
        }

        public bool Delete()
        {
            var serverScript = new ServerScriptModelApiItems(context: Context, onTesting: OnTesting);
            if (Model is IssueModel issueModel)
            {
                return serverScript.Delete(issueModel.IssueId);
            }
            else if (Model is ResultModel resultModel)
            {
                return serverScript.Delete(resultModel.ResultId);
            }
            else
            {
                return false;
            }
        }
    }
}