using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public class BaseModel
    {
        public enum MethodTypes
        {
            NotSet,
            Index,
            New,
            Edit
        }

        public FormData FormData;
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public MethodTypes MethodType = MethodTypes.NotSet;
        public Versions.VerTypes VerType = Versions.VerTypes.Latest;
        public int Ver = 1;
        public Comments Comments = new Comments();
        public User Creator = new User();
        public User Updator = new User();
        public Time CreatedTime = null;
        public Time UpdatedTime = null;
        public bool VerUp = false;
        public string Timestamp = string.Empty;
        public int DeleteCommentId;
        public int SavedVer = 1;
        public int SavedCreator = 0;
        public int SavedUpdator = 0;
        public DateTime SavedCreatedTime = 0.ToDateTime();
        public DateTime SavedUpdatedTime = 0.ToDateTime();
        public bool SavedVerUp = false;
        public string SavedTimestamp = string.Empty;
        public string SavedComments = "[]";
        public ServerScriptModelRow ServerScriptModelRow = new ServerScriptModelRow();

        public bool Ver_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Ver;
            }
            return Ver != SavedVer
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Ver);
        }

        public bool Comments_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Comments.ToJson();
            }
            return Comments.ToJson() != SavedComments && Comments.ToJson() != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Comments.ToJson());
        }

        public bool Creator_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Creator.Id;
            }
            return Creator.Id != SavedCreator
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Creator.Id);
        }

        public bool Updator_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Updator.Id;
            }
            return Updator.Id != SavedUpdator
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Updator.Id);
        }

        public Dictionary<string, string> ClassHash = new Dictionary<string, string>();
        public Dictionary<string, string> SavedClassHash = new Dictionary<string, string>();
        public Dictionary<string, Num> NumHash = new Dictionary<string, Num>();
        public Dictionary<string, decimal?> SavedNumHash = new Dictionary<string, decimal?>();
        public Dictionary<string, DateTime> DateHash = new Dictionary<string, DateTime>();
        public Dictionary<string, DateTime> SavedDateHash = new Dictionary<string, DateTime>();
        public Dictionary<string, string> DescriptionHash = new Dictionary<string, string>();
        public Dictionary<string, string> SavedDescriptionHash = new Dictionary<string, string>();
        public Dictionary<string, bool> CheckHash = new Dictionary<string, bool>();
        public Dictionary<string, bool> SavedCheckHash = new Dictionary<string, bool>();
        public Dictionary<string, Attachments> AttachmentsHash = new Dictionary<string, Attachments>();
        public Dictionary<string, string> SavedAttachmentsHash = new Dictionary<string, string>();
        public Dictionary<string, PostedFile> PostedImageHash = new Dictionary<string, PostedFile>();
        public bool ReadOnly;
        public List<string> MineCache;
        [NonSerialized]
        public Dictionary<string, StatusControl.ControlConstraintsTypes> StatusControlHash;

        public List<string> ColumnNames()
        {
            var data = new List<string>();
            data.AddRange(ClassHash.Keys);
            data.AddRange(NumHash.Keys);
            data.AddRange(DateHash.Keys);
            data.AddRange(DescriptionHash.Keys);
            data.AddRange(CheckHash.Keys);
            data.AddRange(AttachmentsHash.Keys);
            return data
                .Where(columnName => Def.ExtendedColumnTypes.ContainsKey(columnName ?? string.Empty))
                .ToList();
        }

        public string GetValue(
            Context context,
            Column column,
            bool toLocal = false)
        {
            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
            {
                case "Class":
                    return GetClass(columnName: column.ColumnName);
                case "Num":
                    return column.Nullable != true
                        ? GetNum(columnName: column.ColumnName).Value?.ToString() ?? "0"
                        : GetNum(columnName: column.ColumnName).Value?.ToString() ?? string.Empty;
                case "Date":
                    return toLocal
                        ? GetDate(columnName: column.ColumnName)
                            .ToLocal(context: context)
                            .ToString()
                        : GetDate(columnName: column.ColumnName).ToString();
                case "Description":
                    return GetDescription(columnName: column.ColumnName);
                case "Check":
                    return GetCheck(columnName: column.ColumnName).ToString();
                case "Attachments":
                    return GetAttachments(columnName: column.ColumnName).ToJson();
                default:
                    return null;
            }
        }

        public string GetSavedValue(
            Context context,
            Column column,
            bool toLocal = false)
        {
            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
            {
                case "Class":
                    return GetSavedClass(columnName: column.ColumnName);
                case "Num":
                    return column.Nullable != true
                        ? GetSavedNum(columnName: column.ColumnName).ToString() ?? "0"
                        : GetSavedNum(columnName: column.ColumnName).ToString() ?? string.Empty;
                case "Date":
                    return toLocal
                        ? GetSavedDate(columnName: column.ColumnName)
                            .ToLocal(context: context)
                            .ToString()
                        : GetDate(columnName: column.ColumnName).ToString();
                case "Description":
                    return GetSavedDescription(columnName: column.ColumnName);
                case "Check":
                    return GetSavedCheck(columnName: column.ColumnName).ToString();
                case "Attachments":
                    return GetSavedAttachments(columnName: column.ColumnName).ToJson();
                default:
                    return null;
            }
        }

        public void SetValue(
            Context context,
            Column column,
            string value,
            bool toUniversal = false)
        {
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
                        value: toUniversal
                            ? value.ToDateTime().ToUniversal(context: context)
                            : value.ToDateTime());
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

        public bool Updated()
        {
            return ClassHash.Any(o => Class_Updated(o.Key))
                || NumHash.Any(o => Num_Updated(o.Key))
                || DateHash.Any(o => Date_Updated(o.Key))
                || DescriptionHash.Any(o => Description_Updated(o.Key))
                || CheckHash.Any(o => Check_Updated(o.Key))
                || AttachmentsHash.Any(o => Attachments_Updated(o.Key));
        }

        public string GetClass(Column column)
        {
            return GetClass(columnName: column.ColumnName);
        }

        public string GetSavedClass(Column column)
        {
            return GetSavedClass(columnName: column.ColumnName);
        }

        public string GetClass(string columnName)
        {
            return ClassHash.Get(columnName) ?? string.Empty;
        }

        public string GetSavedClass(string columnName)
        {
            return SavedClassHash.Get(columnName) ?? string.Empty;
        }

        public void SetClass(Column column, string value)
        {
            SetClass(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetSavedClass(Column column, string value)
        {
            SetSavedClass(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetClass(string columnName, string value)
        {
            if (!ClassHash.ContainsKey(columnName))
            {
                ClassHash.Add(columnName, value);
            }
            else
            {
                ClassHash[columnName] = value;
            }
        }

        public void SetSavedClass(string columnName, string value)
        {
            if (!SavedClassHash.ContainsKey(columnName))
            {
                SavedClassHash.Add(columnName, value);
            }
            else
            {
                SavedClassHash[columnName] = value;
            }
        }

        public bool Class_Updated(
            string columnName,
            bool copy = false,
            Context context = null,
            Column column = null)
        {
            var value = GetClass(columnName: columnName);
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context) != value;
            }
            return value != GetSavedClass(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context) != value);
        }

        public Num GetNum(Column column)
        {
            return GetNum(columnName: column.ColumnName);
        }

        public decimal? GetSavedNum(Column column)
        {
            return GetSavedNum(columnName: column.ColumnName);
        }

        public Num GetNum(string columnName)
        {
            return NumHash.Get(columnName) ?? new Num();
        }

        public decimal? GetSavedNum(string columnName)
        {
            return SavedNumHash.Get(columnName);
        }

        public void SetNum(Column column, Num value)
        {
            SetNum(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetSavedNum(Column column, decimal? value)
        {
            SetSavedNum(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetNum(string columnName, Num value)
        {
            if (!NumHash.ContainsKey(columnName))
            {
                NumHash.Add(columnName, value);
            }
            else
            {
                NumHash[columnName] = value;
            }
        }

        public void SetSavedNum(string columnName, decimal? value)
        {
            if (!SavedNumHash.ContainsKey(columnName))
            {
                SavedNumHash.Add(columnName, value);
            }
            else
            {
                SavedNumHash[columnName] = value;
            }
        }

        public bool Num_Updated(
            string columnName,
            bool copy = false,
            Context context = null,
            Column column = null,
            bool paramDefault = false)
        {
            var value = GetNum(columnName: columnName)?.Value;
            var savedValue = GetSavedNum(columnName: columnName);
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDecimal() != value;
            }
            if (column?.Nullable !=  true)
            {
                value = value ?? 0;
                savedValue = savedValue ?? 0;
            }
            return value != savedValue
                && (paramDefault
                    || column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDecimal() != value);
        }

        public DateTime GetDate(Column column)
        {
            return GetDate(columnName: column.ColumnName);
        }

        public DateTime GetSavedDate(Column column)
        {
            return GetSavedDate(columnName: column.ColumnName);
        }

        public DateTime GetDate(string columnName)
        {
            return DateHash.ContainsKey(columnName)
                ? DateHash.Get(columnName)
                : 0.ToDateTime();
        }

        public DateTime GetSavedDate(string columnName)
        {
            return SavedDateHash.ContainsKey(columnName)
                ? SavedDateHash.Get(columnName)
                : 0.ToDateTime();
        }

        public void SetDate(Column column, DateTime value)
        {
            SetDate(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetSavedDate(Column column, DateTime value)
        {
            SetSavedDate(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetDate(string columnName, DateTime value)
        {
            if (!DateHash.ContainsKey(columnName))
            {
                DateHash.Add(columnName, value);
            }
            else
            {
                DateHash[columnName] = value;
            }
        }

        public void SetSavedDate(string columnName, DateTime value)
        {
            if (!SavedDateHash.ContainsKey(columnName))
            {
                SavedDateHash.Add(columnName, value);
            }
            else
            {
                SavedDateHash[columnName] = value;
            }
        }

        public bool Date_Updated(
            string columnName,
            bool copy = false,
            Context context = null,
            Column column = null)
        {
            var value = GetDate(columnName: columnName);
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != value;
            }
            return value != GetSavedDate(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDateTime() != value);
        }

        public string GetDescription(Column column)
        {
            return GetDescription(columnName: column.ColumnName);
        }

        public string GetSavedDescription(Column column)
        {
            return GetSavedDescription(columnName: column.ColumnName);
        }

        public string GetDescription(string columnName)
        {
            return DescriptionHash.Get(columnName) ?? string.Empty;
        }

        public string GetSavedDescription(string columnName)
        {
            return SavedDescriptionHash.Get(columnName) ?? string.Empty;
        }

        public void SetDescription(Column column, string value)
        {
            SetDescription(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetSavedDescription(Column column, string value)
        {
            SetSavedDescription(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetDescription(string columnName, string value)
        {
            if (!DescriptionHash.ContainsKey(columnName))
            {
                DescriptionHash.Add(columnName, value);
            }
            else
            {
                DescriptionHash[columnName] = value;
            }
        }

        public void SetSavedDescription(string columnName, string value)
        {
            if (!SavedDescriptionHash.ContainsKey(columnName))
            {
                SavedDescriptionHash.Add(columnName, value);
            }
            else
            {
                SavedDescriptionHash[columnName] = value;
            }
        }

        public bool Description_Updated(
            string columnName,
            bool copy = false,
            Context context = null,
            Column column = null)
        {
            var value = GetDescription(columnName: columnName);
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context) != value;
            }
            return value != GetSavedDescription(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context) != value);
        }

        public bool GetCheck(Column column)
        {
            return GetCheck(columnName: column.ColumnName);
        }

        public bool GetSavedCheck(Column column)
        {
            return GetSavedCheck(columnName: column.ColumnName);
        }

        public bool GetCheck(string columnName)
        {
            return CheckHash.Get(columnName);
        }

        public bool GetSavedCheck(string columnName)
        {
            return SavedCheckHash.Get(columnName);
        }

        public void SetCheck(Column column, bool value)
        {
            SetCheck(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetSavedCheck(Column column, bool value)
        {
            SetSavedCheck(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetCheck(string columnName, bool value)
        {
            if (!CheckHash.ContainsKey(columnName))
            {
                CheckHash.Add(columnName, value);
            }
            else
            {
                CheckHash[columnName] = value;
            }
        }

        public void SetSavedCheck(string columnName, bool value)
        {
            if (!CheckHash.ContainsKey(columnName))
            {
                SavedCheckHash.Add(columnName, value);
            }
            else
            {
                SavedCheckHash[columnName] = value;
            }
        }

        public bool Check_Updated(
            string columnName,
            bool copy = false,
            Context context = null,
            Column column = null)
        {
            var value = GetCheck(columnName: columnName);
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != value;
            }
            return value != GetSavedCheck(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != value);
        }

        public Attachments GetAttachments(Column column)
        {
            return GetAttachments(columnName: column.ColumnName);
        }

        public string GetSavedAttachments(Column column)
        {
            return GetSavedAttachments(columnName: column.ColumnName);
        }

        public Attachments GetAttachments(string columnName)
        {
            return AttachmentsHash.Get(columnName) ?? new Attachments();
        }

        public string GetSavedAttachments(string columnName)
        {
            return SavedAttachmentsHash.Get(columnName) ?? new Attachments().RecordingJson();
        }

        public void SetAttachments(Column column, Attachments value)
        {
            SetAttachments(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetSavedAttachments(Column column, string value)
        {
            SetSavedAttachments(
                columnName: column.ColumnName,
                value: value);
        }

        public void SetAttachments(string columnName, Attachments value)
        {
            if (!AttachmentsHash.ContainsKey(columnName))
            {
                AttachmentsHash.Add(columnName, value);
            }
            else
            {
                AttachmentsHash[columnName] = value;
            }
        }

        public void SetSavedAttachments(string columnName, string value)
        {
            if (!AttachmentsHash.ContainsKey(columnName))
            {
                SavedAttachmentsHash.Add(columnName, value);
            }
            else
            {
                SavedAttachmentsHash[columnName] = value;
            }
        }

        public bool Attachments_Updated(
            string columnName,
            bool copy = false,
            Context context = null,
            Column column = null)
        {
            var value = GetAttachments(columnName: columnName).RecordingJson();
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context) != value;
            }
            return value != GetSavedAttachments(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context) != value);
        }

        public void BaseFullText(
            Context context,
            Column column,
            System.Text.StringBuilder fullText)
        {
            if (column != null)
            {
                switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                {
                    case "Class":
                        GetClass(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Num":
                        GetNum(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Date":
                        GetDate(column.ColumnName).FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Description":
                        GetDescription(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Attachments":
                        GetAttachments(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                }
            }
        }

        public void SetByWhenloadingSiteSettingsServerScript(
            Context context,
            SiteSettings ss)
        {
            if (context.Id == ss.SiteId)
            {
                switch (context.Action)
                {
                    case "createbytemplate":
                    case "edit":
                    case "update":
                    case "copy":
                    case "delete":
                    case "updatesitesettings":
                        return;
                }
            }
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: null,
                view: null,
                where: script => script.WhenloadingSiteSettings == true,
                condition: "WhenloadingSiteSettings");
            SetServerScriptModelColumns(context: context,
                ss: ss,
                scriptValues: scriptValues);
        }

        public virtual void SetByAfterFormulaServerScript(Context context, SiteSettings ss)
        {
        }

        public virtual ServerScriptModelRow SetByWhenloadingRecordServerScript(
            Context context,
            SiteSettings ss)
        {
            return null;
        }

       public virtual ServerScriptModelRow SetByBeforeOpeningRowServerScript(
            Context context,
            SiteSettings ss,
            View view)
        {
            return null;
        }

        public virtual ServerScriptModelRow SetByBeforeOpeningPageServerScript(
            Context context,
            SiteSettings ss,
            View view = null,
            GridData gridData = null)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: gridData,
                itemModel: null,
                view: view,
                where: script => script.BeforeOpeningPage == true,
                condition: "BeforeOpeningPage");
            if (scriptValues != null)
            {
                SetServerScriptModelColumns(context: context,
                    ss: ss,
                    scriptValues: scriptValues);
                ServerScriptModelRow = scriptValues;
            }
            return scriptValues;
        }

        public virtual List<string> Mine(Context context)
        {
            return null;
        }

        public static void SetServerScriptModelColumns(Context context, SiteSettings ss, ServerScriptModelRow scriptValues)
        {
            scriptValues?.Columns.ForEach(scriptColumn =>
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: scriptColumn.Key);
                if (column != null)
                {
                    if (scriptColumn.Value.ChoiceHash != null)
                    {
                        // 該当項目の時のみ絞り込み文字列を適応する
                        var searchText = context.Forms.Data("DropDownSearchTarget") == column.Id
                            ? context.Forms.Data("DropDownSearchText")
                            : null;
                        var searchIndexes = searchText.SearchIndexes();
                        column.ChoiceHash = scriptColumn.Value
                            ?.ChoiceHash
                            ?.Where(o => searchIndexes?.Any() != true ||
                                searchIndexes.All(p =>
                                    o.Key.ToString() == p ||
                                    (o.Value?.ToString()).RegexLike(p).Any()))
                            ?.ToDictionary(
                                o => o.Key.ToString(),
                                o => new Choice(
                                    o.Key.ToString(),
                                    o.Value?.ToString()));
                        column.AddChoiceHashByServerScript = true;
                    }
                    column.ServerScriptModelColumn = scriptColumn.Value;
                }
            });
        }
    }

    public class BaseItemModel : BaseModel
    {
        public long SiteId = 0;
        public Title Title = new Title();
        public string Body = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public string SavedBody = string.Empty;
        public List<string> RecordPermissions;

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
                return column.GetDefaultInput(context: context).ToString() != Title.Value;
            }
            return Title.Value != SavedTitle && Title.Value != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool Body_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Body;
            }
            return Body != SavedBody && Body != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Body);
        }

        public override ServerScriptModelRow SetByWhenloadingRecordServerScript(
            Context context,
            SiteSettings ss)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.WhenloadingRecord == true,
                condition: "WhenloadingRecord");
            if (scriptValues != null)
            {
                ServerScriptModelRow = scriptValues;
            }
            return scriptValues;
        }

        public void SetByBeforeFormulaServerScript(Context context, SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.BeforeFormula == true,
                condition: "BeforeFormula");
        }

        public override void SetByAfterFormulaServerScript(Context context, SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.AfterFormula == true,
                condition: "AfterFormula");
        }

        public void SetByAfterUpdateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.AfterUpdate == true,
                condition: "AfterUpdate");
        }

        public void SetByBeforeUpdateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.BeforeUpdate == true,
                condition: "BeforeUpdate");
        }

        public void SetByAfterCreateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.AfterCreate == true,
                condition: "AfterCreate");
        }

        public void SetByBeforeCreateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.BeforeCreate == true,
                condition: "BeforeCreate");
        }

        public void SetByAfterDeleteServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.AfterDelete == true,
                condition: "AfterDelete");
        }

        public void SetByBeforeDeleteServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: null,
                where: script => script.BeforeDelete == true,
                condition: "BeforeDelete");
        }

        public override ServerScriptModelRow SetByBeforeOpeningRowServerScript(
            Context context,
            SiteSettings ss,
            View view)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: null,
                itemModel: this,
                view: view,
                where: script => script.BeforeOpeningRow == true,
                condition: "BeforeOpeningRow");
            if (scriptValues != null)
            {
                SetServerScriptModelColumns(context: context,
                    ss: ss,
                    scriptValues: scriptValues);
                ServerScriptModelRow = scriptValues;
            }
            return scriptValues;
        }

        public override ServerScriptModelRow SetByBeforeOpeningPageServerScript(
            Context context,
            SiteSettings ss,
            View view = null,
            GridData gridData = null)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                gridData: gridData,
                itemModel: this,
                view: view,
                where: script => script.BeforeOpeningPage == true,
                condition: "BeforeOpeningPage");
            if (scriptValues != null)
            {
                SetServerScriptModelColumns(
                    context: context,
                    ss: ss,
                    scriptValues: scriptValues);
                ServerScriptModelRow = scriptValues;
            }
            return scriptValues;
        }

        public void SetExtendedColumnDefaultValue(SiteSettings ss, string formulaScript, string calculationMethod)
        {
            var columns = System.Text.RegularExpressions.Regex.Matches(formulaScript, @"\[([^]]*)\]");
            foreach (var column in columns)
            {
                var columnParam = column.ToString()[1..^1];
                if (ss.FormulaColumn(columnParam, calculationMethod) != null)
                {
                    switch (Def.ExtendedColumnTypes.Get(columnParam))
                    {
                        case "Num":
                            if (GetNum(columnParam).Value == null)
                            {
                                SetNum(columnParam, new Num(0));
                            }
                            break;
                        case "Check":
                            SetCheck(columnParam, GetCheck(columnParam));
                            break;
                        case "Class":
                            SetClass(columnParam, GetClass(columnParam));
                            break;
                        case "Description":
                            SetDescription(columnParam, GetDescription(columnParam));
                            break;
                    }
                }
            }
            var columnList = ss.FormulaColumnList();
            foreach (var column in columnList)
            {
                var isMatched = System.Text.RegularExpressions.Regex.IsMatch(
                    input: formulaScript,
                    pattern: System.Text.RegularExpressions.Regex.Escape(column.LabelText)
                        + $"(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                if (isMatched)
                {
                    switch (Def.ExtendedColumnTypes.Get(column.ColumnName))
                    {
                        case "Num":
                            if (GetNum(column).Value == null)
                            {
                                SetNum(column, new Num(0));
                            }
                            break;
                        case "Check":
                            SetCheck(column, GetCheck(column));
                            break;
                        case "Class":
                            SetClass(column, GetClass(column));
                            break;
                        case "Description":
                            SetDescription(column, GetDescription(column));
                            break;
                    }
                }
            }
        }
    }
}
