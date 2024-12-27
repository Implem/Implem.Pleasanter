using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModel : IDisposable
    {
        public readonly ServerScriptModelGrid Grid;
        public readonly ExpandoObject Model = new ExpandoObject();
        public readonly ExpandoObject Saved = new ExpandoObject();
        public readonly ServerScriptModelDepts Depts;
        public readonly ServerScriptModelGroups Groups;
        public readonly ServerScriptModelUsers Users;
        public readonly ExpandoObject Columns = new ExpandoObject();
        public readonly ServerScriptModelContext Context;
        public readonly ServerScriptModelSiteSettings SiteSettings;
        public readonly ServerScriptModelView View;
        public readonly ServerScriptModelApiItems Items;
        public ServerScriptModelHidden Hidden;
        public ServerScriptModelResponses Responses;
        public ServerScriptElements Elements;
        public ServerScriptModelExtendedSql ExtendedSql;
        public ServerScriptModelNotification Notification;
        public ServerScriptModelHttpClient HttpClient;
        public readonly ServerScriptModelUtilities Utilities;
        public readonly ServerScriptModelLogs Logs;
        public readonly ServerScriptFile File;
        public readonly ServerScriptCsv Csv;
        public bool Debug;
        private DateTime TimeOut;
        private readonly List<string> ChangeItemNames = new List<string>();

        public enum ServerScriptConditions
        {
            None,
            WhenViewProcessing,
            WhenloadingSiteSettings,
            BeforeOpeningPage,
            BeforeOpeningRow,
            WhenloadingRecord,
            BeforeFormula,
            AfterFormula,
            AfterUpdate,
            BeforeUpdate,
            AfterCreate,
            BeforeCreate,
            AfterDelete,
            BeforeDelete,
            BackgroundServerScript
        }

        public ServerScriptModel(
            Context context,
            SiteSettings ss,
            GridData gridData,
            IEnumerable<(string Name, object Value)> data,
            IEnumerable<(string Name, object Value)> saved,
            IEnumerable<(string Name, ServerScriptModelColumn Value)> columns,
            View view,
            ServerScriptConditions condition,
            DateTime timeOut,
            bool debug,
            bool onTesting)
        {
            Grid = new ServerScriptModelGrid(
                context: context,
                ss: ss,
                totalCount: gridData?.TotalCount ?? 0);
            data?.ForEach(datam => ((IDictionary<string, object>)Model)[datam.Name] = datam.Value);
            saved?.ForEach(datam => ((IDictionary<string, object>)Saved)[datam.Name] = datam.Value);
            Depts = new ServerScriptModelDepts(context: context);
            Groups = new ServerScriptModelGroups(context: context);
            Users = new ServerScriptModelUsers(context: context);
            columns?.ForEach(
                datam => ((IDictionary<string, object>)Columns)[datam.Name] = datam.Value);
            View = new ServerScriptModelView(view != null
                ? view.Id
                : 0);
            view?.ColumnFilterHash?.ForEach(columnFilter =>
                ((IDictionary<string, object>)View.Filters)[columnFilter.Key] = columnFilter.Value);
            view?.ColumnFilterSearchTypes?.ForEach(columnFilterSearchType =>
                ((IDictionary<string, object>)View.SearchTypes)[columnFilterSearchType.Key] = columnFilterSearchType.Value);
            view?.ColumnSorterHash?.ForEach(columnSorter =>
                ((IDictionary<string, object>)View.Sorters)[columnSorter.Key] = Enum.GetName(
                    typeof(SqlOrderBy.Types),
                    columnSorter.Value));
            View.SetInitialized();
            ((INotifyPropertyChanged)Model).PropertyChanged += DataPropertyChanged;
            Context = new ServerScriptModelContext(
                context: context,
                logBuilder: context.LogBuilder,
                userData: context.UserData,
                responseCollection: context.ResponseCollection,
                messages: context.Messages,
                errorData: context.ErrorData,
                redirectData: context.RedirectData,
                formStringRaw: context.FormStringRaw,
                formString: context.FormString,
                ajax: context.Ajax,
                mobile: context.Mobile,
                applicationPath: context.ApplicationPath,
                absoluteUri: context.AbsoluteUri,
                absolutePath: context.AbsolutePath,
                url: context.Url,
                urlReferrer: context.UrlReferrer,
                controller: context.Controller,
                query: context.Query,
                action: context.Action,
                tenantId: context.TenantId,
                siteId: context.SiteId,
                id: context.Id,
                groupIds: context.Groups,
                tenantTitle: context.TenantTitle,
                siteTitle: context.SiteTitle,
                recordTitle: context.RecordTitle,
                deptId: context.DeptId,
                userId: context.UserId,
                loginId: context.LoginId,
                language: context.Language,
                timeZoneInfo: context.TimeZoneInfo?.ToString(),
                hasPrivilege: context.HasPrivilege,
                apiVersion: context.ApiVersion,
                apiRequestBody: context.ApiRequestBody,
                requestDataString: context.RequestDataString,
                contentType: context.ContentType,
                onTesting: onTesting,
                scriptDepth: context.ServerScriptDepth,
                controlId: context.Forms.ControlId(),
                condition: condition.ToString());
            SiteSettings = new ServerScriptModelSiteSettings(
                context: context,
                ss: ss);
            Items = new ServerScriptModelApiItems(
                context: context,
                onTesting: onTesting);
            Hidden = new ServerScriptModelHidden();
            Responses = new ServerScriptModelResponses();
            Elements = new ServerScriptElements();
            ExtendedSql = new ServerScriptModelExtendedSql(
                context: context,
                onTesting: onTesting);
            Notification = new ServerScriptModelNotification(
                context: context,
                ss: ss);
            HttpClient = new ServerScriptModelHttpClient();
            Utilities = new ServerScriptModelUtilities(
                context: context,
                ss: ss);
            Logs = new ServerScriptModelLogs(
                context: context,
                ss: ss);
            File = new ServerScriptFile(
                context: context);
            Csv = new ServerScriptCsv();
            Debug = debug;
            TimeOut = timeOut;
        }

        private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangeItemNames.Add(e.PropertyName);
        }

        public IReadOnlyCollection<string> GetChangeItemNames()
        {
            return ChangeItemNames.ToArray();
        }

        public void Dispose()
        {
            ((INotifyPropertyChanged)Model).PropertyChanged -= DataPropertyChanged;
        }

        public bool ContinuationCallback()
        {
            return Debug
                ? true
                : TimeOut > DateTime.Now;
        }

        public class ServerScriptModelColumn
        {
            private string _labelText;
            private bool _labelTextChanged;
            private string _labelRaw;
            private bool _labelRawChanged;
            private string _rawText;
            private bool _rawTextChanged;
            private bool _readOnly;
            private bool _readOnlyChanged;
            private bool _hide;
            private bool _hideChanged;
            private bool _validateRequired;
            private bool _validateRequiredChanged;
            private string _extendedFieldCss;
            private bool _extendedFieldCssChanged;
            private string _extendedControlCss;
            private bool _extendedControlCssChanged;
            private string _extendedCellCss;
            private bool _extendedCellCssChanged;
            private string _extendedHtmlBeforeField;
            private bool _extendedHtmlBeforeFieldChanged;
            private string _extendedHtmlBeforeLabel;
            private bool _extendedHtmlBeforeLabelChanged;
            private string _extendedHtmlBetweenLabelAndControl;
            private bool _extendedHtmlBetweenLabelAndControlChanged;
            private string _extendedHtmlAfterControl;
            private bool _extendedHtmlAfterControlChanged;
            private string _extendedHtmlAfterField;
            private bool _extendedHtmlAfterFieldChanged;
            private bool _choiceHashChanged;

            public string LabelText { get { return _labelText; } set { _labelText = value; _labelTextChanged = true; } }
            public string LabelRaw { get { return _labelRaw; } set { _labelRaw = value; _labelRawChanged = true; } }
            public string RawText { get { return _rawText; } set { _rawText = value; _rawTextChanged = true; } }
            public bool ReadOnly { get { return _readOnly; } set { _readOnly = value; _readOnlyChanged = true; } }
            public bool Hide { get { return _hide; } set { _hide = value; _hideChanged = true; } }
            public bool ValidateRequired { get { return _validateRequired; } set { _validateRequired = value; _validateRequiredChanged = true; } }
            public string ExtendedFieldCss { get { return _extendedFieldCss; } set { _extendedFieldCss = value; _extendedFieldCssChanged = true; } }
            public string ExtendedControlCss { get { return _extendedControlCss; } set { _extendedControlCss = value; _extendedControlCssChanged = true; } }
            public string ExtendedCellCss { get { return _extendedCellCss; } set { _extendedCellCss = value; _extendedCellCssChanged = true; } }
            public string ExtendedHtmlBeforeField { get { return _extendedHtmlBeforeField; } set { _extendedHtmlBeforeField = value; _extendedHtmlBeforeFieldChanged = true; } }
            public string ExtendedHtmlBeforeLabel { get { return _extendedHtmlBeforeLabel; } set { _extendedHtmlBeforeLabel = value; _extendedHtmlBeforeLabelChanged = true; } }
            public string ExtendedHtmlBetweenLabelAndControl { get { return _extendedHtmlBetweenLabelAndControl; } set { _extendedHtmlBetweenLabelAndControl = value; _extendedHtmlBetweenLabelAndControlChanged = true; } }
            public string ExtendedHtmlAfterControl { get { return _extendedHtmlAfterControl; } set { _extendedHtmlAfterControl = value; _extendedHtmlAfterControlChanged = true; } }
            public string ExtendedHtmlAfterField { get { return _extendedHtmlAfterField; } set { _extendedHtmlAfterField = value; _extendedHtmlAfterFieldChanged = true; } }
            public Dictionary<object, object> ChoiceHash { get; set; }

            public ServerScriptModelColumn(
                string labelText,
                string labelRaw,
                string rawText,
                bool readOnly,
                bool hide,
                bool validateRequired,
                string extendedFieldCss,
                string extendedControlCss,
                string extendedCellCss,
                string extendedHtmlBeforeField,
                string extendedHtmlBeforeLabel,
                string extendedHtmlBetweenLabelAndControl,
                string extendedHtmlAfterControl,
                string extendedHtmlAfterField)
            {
                _labelText = labelText;
                _labelRaw = labelRaw;
                _rawText = rawText;
                _readOnly = readOnly;
                _hide = hide;
                _validateRequired = validateRequired;
                _extendedFieldCss = extendedFieldCss;
                _extendedControlCss = extendedControlCss;
                _extendedCellCss = extendedCellCss;
                _extendedHtmlBeforeField = extendedHtmlBeforeField;
                _extendedHtmlBeforeLabel = extendedHtmlBeforeLabel;
                _extendedHtmlBetweenLabelAndControl = extendedHtmlBetweenLabelAndControl;
                _extendedHtmlAfterControl = extendedHtmlAfterControl;
                _extendedHtmlAfterField = extendedHtmlAfterField;
            }

            public void AddChoiceHash(object key, object value)
            {
                if (ChoiceHash == null)
                {
                    ChoiceHash = new Dictionary<object, object>();
                }
                ChoiceHash.Add(key, value);
                _choiceHashChanged = true;
            }

            public void ClearChoiceHash()
            {
                if (ChoiceHash == null)
                {
                    ChoiceHash = new Dictionary<object, object>();
                }
                ChoiceHash.Clear();
                _choiceHashChanged = true;
            }

            public bool Changed()
            {
                return _labelTextChanged
                    || _labelRawChanged
                    || _rawTextChanged
                    || _readOnlyChanged
                    || _hideChanged
                    || _validateRequiredChanged
                    || _extendedFieldCssChanged
                    || _extendedControlCssChanged
                    || _extendedCellCssChanged
                    || _extendedHtmlBeforeFieldChanged
                    || _extendedHtmlBeforeLabelChanged
                    || _extendedHtmlBetweenLabelAndControlChanged
                    || _extendedHtmlAfterControlChanged
                    || _extendedHtmlAfterFieldChanged
                    || _choiceHashChanged;
            }

            public bool? GetReadOnly()
            {
                return _readOnlyChanged
                    ? (bool?)ReadOnly
                    : null;
            }

            public bool? GetValidateRequired()
            {
                return _validateRequiredChanged
                    ? (bool?)ValidateRequired
                    : null;
            }

            public bool? GetHide()
            {
                return _hide
                    ? (bool?)Hide
                    : null;
            }

            public bool HideChanged => _hideChanged;
        }

        public class ServerScriptModelRow
        {
            public string ExtendedRowCss { get; set; }
            public string ExtendedRowData { get; set; }
            public Dictionary<string, ServerScriptModelColumn> Columns { get; set; }
            public Dictionary<string, string> Hidden { get; set; }
            public ServerScriptModelResponses Responses { get; set; }
            public ServerScriptElements Elements { get; set; }
            private List<string> ReplaceFieldColumnsCache { get; set; }

            public List<string> ReplaceFieldColumns(Context context)
            {
                if (ReplaceFieldColumnsCache == null)
                {
                    var targetColumns = Columns
                        ?.Where(o => o.Value.Changed())
                        .Select(o => o.Key)
                        .ToList();
                    ReplaceFieldColumnsCache = context.Forms.List("ReplaceFieldColumns")
                        .ToList();
                    if (targetColumns != null)
                    {
                        ReplaceFieldColumnsCache = ReplaceFieldColumnsCache
                            ?.Concat(targetColumns)
                            .Distinct()
                            .ToList();
                    }
                }
                return ReplaceFieldColumnsCache;
            }
        }
    }
}