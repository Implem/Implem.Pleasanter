﻿using Implem.DefinitionAccessor;
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

        public Context Context;
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
        public IList<ServerScriptModelRow> ServerScriptModelRows = new List<ServerScriptModelRow>();

        public bool Ver_Updated(Context context, Column column = null)
        {
            return Ver != SavedVer &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Ver);
        }

        public bool Comments_Updated(Context context, Column column = null)
        {
            return Comments.ToJson() != SavedComments && Comments.ToJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Comments.ToJson());
        }

        public bool Creator_Updated(Context context, Column column = null)
        {
            return Creator.Id != SavedCreator &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Creator.Id);
        }

        public bool Updator_Updated(Context context, Column column = null)
        {
            return Updator.Id != SavedUpdator &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Updator.Id);
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
        public bool ReadOnly;

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
                .Where(columnName => Def.ExtendedColumnTypes.ContainsKey(columnName))
                .ToList();
        }

        public string Value(
            Context context,
            Column column,
            bool toLocal = false)
        {
            switch (Def.ExtendedColumnTypes.Get(column.ColumnName))
            {
                case "Class":
                    return Class(columnName: column.ColumnName);
                case "Num":
                    return !column.Nullable != true
                        ? Num(columnName: column.ColumnName).Value?.ToString() ?? "0"
                        : Num(columnName: column.ColumnName).Value?.ToString() ?? string.Empty;
                case "Date":
                    return toLocal
                        ? Date(columnName: column.ColumnName)
                            .ToLocal(context: context)
                            .ToString()
                        : Date(columnName: column.ColumnName).ToString();
                case "Description":
                    return Description(columnName: column.ColumnName);
                case "Check":
                    return Check(columnName: column.ColumnName).ToString();
                case "Attachments":
                    return Attachments(columnName: column.ColumnName).ToJson();
                default:
                    return null;
            }
        }

        public void Value(
            Context context,
            Column column,
            string value,
            bool toUniversal = false)
        {
            switch (Def.ExtendedColumnTypes.Get(column.ColumnName))
            {
                case "Class":
                    Class(
                        columnName: column.ColumnName,
                        value: value);
                    break;
                case "Num":
                    Num(
                        columnName: column.ColumnName,
                        value: new Num(
                            context: context,
                            column: column,
                            value: value));
                    break;
                case "Date":
                    Date(
                        columnName: column.ColumnName,
                        value: toUniversal
                            ? value.ToDateTime().ToUniversal(context: context)
                            : value.ToDateTime());
                    break;
                case "Description":
                    Description(
                        columnName: column.ColumnName,
                        value: value);
                    break;
                case "Check":
                    Check(
                        columnName: column.ColumnName,
                        value: value.ToBool());
                    break;
                case "Attachments":
                    Attachments(
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

        public string Class(Column column)
        {
            return Class(columnName: column.ColumnName);
        }

        public string SavedClass(Column column)
        {
            return SavedClass(columnName: column.ColumnName);
        }

        public string Class(string columnName)
        {
            return ClassHash.Get(columnName) ?? string.Empty;
        }

        public string SavedClass(string columnName)
        {
            return SavedClassHash.Get(columnName) ?? string.Empty;
        }

        public void Class(Column column, string value)
        {
            Class(
                columnName: column.ColumnName,
                value: value);
        }

        public void SavedClass(Column column, string value)
        {
            SavedClass(
                columnName: column.ColumnName,
                value: value);
        }

        public void Class(string columnName, string value)
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

        public void SavedClass(string columnName, string value)
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
            Context context = null,
            Column column = null)
        {
            var value = Class(columnName: columnName);
            return value != SavedClass(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context) != value);
        }

        public Num Num(Column column)
        {
            return Num(columnName: column.ColumnName);
        }

        public decimal? SavedNum(Column column)
        {
            return SavedNum(columnName: column.ColumnName);
        }

        public Num Num(string columnName)
        {
            return NumHash.Get(columnName) ?? new Num();
        }

        public decimal? SavedNum(string columnName)
        {
            return SavedNumHash.Get(columnName);
        }

        public void Num(Column column, Num value)
        {
            Num(
                columnName: column.ColumnName,
                value: value);
        }

        public void SavedNum(Column column, decimal? value)
        {
            SavedNum(
                columnName: column.ColumnName,
                value: value);
        }

        public void Num(string columnName, Num value)
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

        public void SavedNum(string columnName, decimal? value)
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
            Context context = null,
            Column column = null)
        {
            var value = Num(columnName: columnName)?.Value;
            return value != SavedNum(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDecimal() != value);
        }

        public DateTime Date(Column column)
        {
            return Date(columnName: column.ColumnName);
        }

        public DateTime SavedDate(Column column)
        {
            return SavedDate(columnName: column.ColumnName);
        }

        public DateTime Date(string columnName)
        {
            return DateHash.ContainsKey(columnName)
                ? DateHash.Get(columnName)
                : 0.ToDateTime();
        }

        public DateTime SavedDate(string columnName)
        {
            return SavedDateHash.ContainsKey(columnName)
                ? SavedDateHash.Get(columnName)
                : 0.ToDateTime();
        }

        public void Date(Column column, DateTime value)
        {
            Date(
                columnName: column.ColumnName,
                value: value);
        }

        public void SavedDate(Column column, DateTime value)
        {
            SavedDate(
                columnName: column.ColumnName,
                value: value);
        }

        public void Date(string columnName, DateTime value)
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

        public void SavedDate(string columnName, DateTime value)
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
            Context context = null,
            Column column = null)
        {
            var value = Date(columnName: columnName);
            return value != SavedDate(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDateTime() != value);
        }

        public string Description(Column column)
        {
            return Description(columnName: column.ColumnName);
        }

        public string SavedDescription(Column column)
        {
            return SavedDescription(columnName: column.ColumnName);
        }

        public string Description(string columnName)
        {
            return DescriptionHash.Get(columnName) ?? string.Empty;
        }

        public string SavedDescription(string columnName)
        {
            return SavedDescriptionHash.Get(columnName) ?? string.Empty;
        }

        public void Description(Column column, string value)
        {
            Description(
                columnName: column.ColumnName,
                value: value);
        }

        public void SavedDescription(Column column, string value)
        {
            SavedDescription(
                columnName: column.ColumnName,
                value: value);
        }

        public void Description(string columnName, string value)
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

        public void SavedDescription(string columnName, string value)
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
            Context context = null,
            Column column = null)
        {
            var value = Description(columnName: columnName);
            return value != SavedDescription(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context) != value);
        }

        public bool Check(Column column)
        {
            return Check(columnName: column.ColumnName);
        }

        public bool SavedCheck(Column column)
        {
            return SavedCheck(columnName: column.ColumnName);
        }

        public bool Check(string columnName)
        {
            return CheckHash.Get(columnName);
        }

        public bool SavedCheck(string columnName)
        {
            return SavedCheckHash.Get(columnName);
        }

        public void Check(Column column, bool value)
        {
            Check(
                columnName: column.ColumnName,
                value: value);
        }

        public void SavedCheck(Column column, bool value)
        {
            SavedCheck(
                columnName: column.ColumnName,
                value: value);
        }

        public void Check(string columnName, bool value)
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

        public void SavedCheck(string columnName, bool value)
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
            Context context = null,
            Column column = null)
        {
            var value = Check(columnName: columnName);
            return value != SavedCheck(columnName: columnName)
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != value);
        }

        public Attachments Attachments(Column column)
        {
            return Attachments(columnName: column.ColumnName);
        }

        public string SavedAttachments(Column column)
        {
            return SavedAttachments(columnName: column.ColumnName);
        }

        public Attachments Attachments(string columnName)
        {
            return AttachmentsHash.Get(columnName) ?? new Attachments();
        }

        public string SavedAttachments(string columnName)
        {
            return SavedAttachmentsHash.Get(columnName) ?? new Attachments().RecordingJson();
        }

        public void Attachments(Column column, Attachments value)
        {
            Attachments(
                columnName: column.ColumnName,
                value: value);
        }

        public void SavedAttachments(Column column, string value)
        {
            SavedAttachments(
                columnName: column.ColumnName,
                value: value);
        }

        public void Attachments(string columnName, Attachments value)
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

        public void SavedAttachments(string columnName, string value)
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
            Context context = null,
            Column column = null)
        {
            var value = Attachments(columnName: columnName).RecordingJson();
            return value != SavedAttachments(columnName: columnName)
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
                switch (Def.ExtendedColumnTypes.Get(column.ColumnName))
                {
                    case "Class":
                        Class(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Num":
                        Num(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Date":
                        Date(column.ColumnName).FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Description":
                        Description(column.ColumnName)?.FullText(
                            context: context,
                            column: column,
                            fullText: fullText);
                        break;
                    case "Attachments":
                        Attachments(column.ColumnName)?.FullText(
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
                        return;
                }
            }
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: null,
                view: null,
                where: script => script.WhenloadingSiteSettings == true);
            scriptValues?.Columns
                .Where(column => column.Value?.ChoiceHash != null)
                .ForEach(column =>
                {
                    var ssColumn = ss.GetColumn(
                        context: context,
                        columnName: column.Key);
                    if (ssColumn != null)
                    {
                        ssColumn.ChoiceHash = column.Value
                            ?.ChoiceHash
                            ?.ToDictionary(
                                o => o.Key.ToString(),
                                o => new Choice(
                                    o.Key.ToString(),
                                    o.Value?.ToString()));
                    }
                });
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
            SiteSettings ss)
        {
            return null;
        }

        public virtual ServerScriptModelRow SetByBeforeOpeningPageServerScript(
            Context context,
            SiteSettings ss)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: null,
                view: null,
                where: script => script.BeforeOpeningPage == true);
            if (scriptValues != null)
            {
                ServerScriptModelRows.Add(scriptValues);
            }
            return scriptValues;
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

        public bool SiteId_Updated(Context context, Column column = null)
        {
            return SiteId != SavedSiteId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != SiteId);
        }

        public bool Title_Updated(Context context, Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public override ServerScriptModelRow SetByWhenloadingRecordServerScript(
            Context context,
            SiteSettings ss)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.WhenloadingRecord == true);
            if (scriptValues != null)
            {
                ServerScriptModelRows.Add(scriptValues);
            }
            return scriptValues;
        }

        public void SetByBeforeFormulaServerScript(Context context, SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeFormula == true);
        }

        public override void SetByAfterFormulaServerScript(Context context, SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.AfterFormula == true);
        }

        public void SetByAfterUpdateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.AfterUpdate == true);
        }

        public void SetByBeforeUpdateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeUpdate == true);
        }

        public void SetByAfterCreateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.AfterCreate == true);
        }

        public void SetByBeforeCreateServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeCreate == true);
        }

        public void SetByAfterDeleteServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.AfterDelete == true);
        }

        public void SetByBeforeDeleteServerScript(
            Context context,
            SiteSettings ss)
        {
            ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeDelete == true);
        }

        public override ServerScriptModelRow SetByBeforeOpeningRowServerScript(
            Context context,
            SiteSettings ss)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeOpeningRow == true);
            if (scriptValues != null)
            {
                ServerScriptModelRows.Add(scriptValues);
            }
            return scriptValues;
        }

        public override ServerScriptModelRow SetByBeforeOpeningPageServerScript(
            Context context,
            SiteSettings ss)
        {
            var scriptValues = ServerScriptUtilities.Execute(
                context: context,
                ss: ss,
                itemModel: this,
                view: null,
                where: script => script.BeforeOpeningPage == true);
            if (scriptValues != null)
            {
                ServerScriptModelRows.Add(scriptValues);
            }
            return scriptValues;
        }
    }
}
