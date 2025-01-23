#nullable enable
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MimeKit.Cryptography;
using Implem.DisplayAccessor;
using Implem.DefinitionAccessor;

namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ExtensionApiModel
    {
        public decimal ApiVersion { get; set; } = Parameters.Api.Version;
        //public bool? VerUp { get; set; } //Extension_historyのPKがVerのみなため、使用しない（Indexを削除・追加の再作成は基本的に行わない方針）
        public int? ExtensionId { get; set; }
        public int? TenantId { get; set; }
        public int? Ver { get; set; }
        public string ExtensionType { get; set; } = string.Empty;
        public string ExtensionName { get; set; } = string.Empty;
        public dynamic? ExtensionSettings { get; set; }
        public string Body { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Disabled { get; set; } = false;
        public string? Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        private bool invalidExtensionSSettings = false;

        public bool HasInvalidExtensionSettings()
        {
            return invalidExtensionSSettings;
        }

        public object? ObjectValue(string columnName)
        {
            return columnName switch
            {
                "ExtensionId" => ExtensionId,
                "TenantId" => TenantId,
                "Ver" => Ver,
                "ExtensionType" => ExtensionType,
                "ExtensionName" => ExtensionName,
                "ExtensionSettings" => ExtensionSettings,
                "Body" => Body,
                "Description" => Description,
                "Disabled" => Disabled,
                "Comments" => Comments,
                "Creator" => Creator,
                "Updator" => Updator,
                "CreatedTime" => CreatedTime,
                "UpdatedTime" => UpdatedTime,
                _ => null
            };
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            string jsonString = Convert.ToString(ExtensionSettings??"");
            if (ExtensionType == "Fields" && jsonString.Deserialize<ExtensionSettingsForField>() is null)
            {
                invalidExtensionSSettings = true;
            }
            else if (ExtensionType == "Html" && jsonString.Deserialize<ExtensionSettingsForHtml>() is null)
            {
                invalidExtensionSSettings = true;
            }
            else if (ExtensionType == "NavigationMenu" && jsonString.Deserialize<ExtensionSettingsForNavigationMenu>() is null)
            {
                invalidExtensionSSettings = true;
            }
            else if (ExtensionType == "Script" && jsonString.Deserialize<ExtensionSettingsForScript>() is null)
            {
                invalidExtensionSSettings = true;
            }
            else if (ExtensionType == "ServerScript" && jsonString.Deserialize<ExtensionSettingsForServerScript>() is null)
            {
                invalidExtensionSSettings = true;
            }
            else if (ExtensionType == "Sql" && jsonString.Deserialize<ExtensionSettingsForSql>() is null)
            {
                invalidExtensionSSettings = true;
            }
            else if (ExtensionType == "Style" && jsonString.Deserialize<ExtensionSettingsForStyle>() is null)
            {
                invalidExtensionSSettings = true;
            }
            else if (ExtensionType == "Plugin" && jsonString.Deserialize<ExtensionSettingsForPlugin>() is null)
            {
                invalidExtensionSSettings = true;
            }
        }

        class ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedBase を参考
            public string? Name { get; set; }
            public bool? SpecifyByName { get; set; }
            public string? Path { get; set; }
            public string? Description { get; set; }
            public bool? Disabled { get; set; }  
            public List<int>? DeptIdList { get; set; }
            public List<int>? GroupIdList { get; set; }
            public List<int>? UserIdList { get; set; }
            public List<long>? SiteIdList { get; set; }
            public List<long>? IdList { get; set; }
            public List<string>? Controllers { get; set; }
            public List<string>? Actions { get; set; }
            public List<string>? ColumnList { get; set; }
        }

        class ExtensionSettingsForField : ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedField を参考
            public string? FieldType { get; set; }
            public string? TypeName { get; set; }
            public string? LabelText { get; set; }
            public string? ChoicesText { get; set; }
            public string? DefaultInput { get; set; }
            public string? EditorFormat { get; set; }
            public string? ControlType { get; set; }
            public bool? ValidateRequired { get; set; }
            public bool? ValidateNumber { get; set; }
            public bool? ValidateDate { get; set; }
            public bool? ValidateEmail { get; set; }
            public decimal? MaxLength { get; set; }
            public string? ValidateEqualTo { get; set; }
            public int? ValidateMaxLength { get; set; }
            public int? DecimalPlaces { get; set; }
            public bool? Nullable { get; set; }
            public string? Unit { get; set; }
            public decimal? Min { get; set; }
            public decimal? Max { get; set; }
            public decimal? Step { get; set; }
            public bool? AutoPostBack { get; set; }
            public string? FieldCss { get; set; }
            public int? CheckFilterControlType { get; set; }
            public int? DateTimeStep { get; set; }
            public string? ControlCss { get; set; }
            public string? After { get; set; }
            public bool? SqlParam { get; set; }
        }

        class ExtensionSettingsForHtml : ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedHtml を参考
            public string? Language { get; set; }
            public Dictionary<string, List<DisplayElement>>? Html { get; set; }  //TODO: 必要か確認
        }

        class ExtensionSettingsForNavigationMenu : ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedNavigationMenu を参考
            public string? TargetId { get; set; }
            public string? Action { get; set; }
            public List<NavigationMenu>? NavigationMenus { get; set; }  //TODO: 必要か確認
        }

        class NavigationMenu : ExtensionSettingsBase  //TODO: 必要か確認
        {
            // Implem.ParameterAccessor.Parts.NavigationMenu を参考
            public string? ContainerId { get; set; }
            public string? MenuId { get; set; }
            public string? Id { get; set; }
            public string? Css { get; set; }
            public string? Icon { get; set; }
            public string? Url { get; set; }
            public string? Function { get; set; }
            public List<string>? LinkParams { get; set; }
            public string? Target { get; set; }
            public List<string>? ReferenceTypes { get; set; }
            public List<NavigationMenu>? ChildMenus { get; set; }
        }

        class ExtensionSettingsForScript : ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedScript を参考
            //public string? Script { get; set; }
        }

        class ExtensionSettingsForServerScript : ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedServerScript を参考
            public bool? WhenloadingSiteSettings { get; set; }
            public bool? WhenViewProcessing { get; set; }
            public bool? WhenloadingRecord { get; set; }
            public bool? BeforeFormula { get; set; }
            public bool? AfterFormula { get; set; }
            public bool? BeforeCreate { get; set; }
            public bool? AfterCreate { get; set; }
            public bool? BeforeUpdate { get; set; }
            public bool? AfterUpdate { get; set; }
            public bool? BeforeDelete { get; set; }
            public bool? AfterDelete { get; set; }
            public bool? BeforeOpeningPage { get; set; }
            public bool? BeforeOpeningRow { get; set; }
            public bool? Shared { get; set; }
            //public string? Body{ get; set; }
        }


        class ExtensionSettingsForSql : ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedSql を参考

            public bool? Api { get; set; }
            public string? DbUser { get; set; }
            public bool? Html { get; set; }
            public bool? OnCreating { get; set; }
            public bool? OnCreated { get; set; }
            public bool? OnUpdating { get; set; }
            public bool? OnUpdated { get; set; }
            public bool? OnUpdatingByGrid { get; set; }
            public bool? OnUpdatedByGrid { get; set; }
            public bool? OnDeleting { get; set; }
            public bool? OnDeleted { get; set; }
            public bool? OnBulkUpdating { get; set; }
            public bool? OnBulkUpdated { get; set; }
            public bool? OnBulkDeleting { get; set; }
            public bool? OnBulkDeleted { get; set; }
            public bool? OnImporting { get; set; }
            public bool? OnImported { get; set; }
            public bool? OnSelectingColumn { get; set; }
            public bool? OnSelectingWhere { get; set; }
            public bool? OnSelectingWherePermissionsDepts { get; set; }
            public bool? OnSelectingWherePermissionsGroups { get; set; }
            public bool? OnSelectingWherePermissionsUsers { get; set; }
            public List<string>? OnSelectingWhereParams { get; set; }
            public bool? OnSelectingOrderBy { get; set; }
            public List<string>? OnSelectingOrderByParams { get; set; }
            public bool? OnUseSecondaryAuthentication { get; set; }
            public string? CommandText { get; set; }
            public List<string>? OnSelectingColumnParams { get; set; }
        }
        class ExtensionSettingsForStyle : ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedStyle を参考
            //public string? Style { get; set; }
        }

        class ExtensionSettingsForPlugin : ExtensionSettingsBase
        {
            // Implem.ParameterAccessor.Parts.ExtendedPlugin を参考
            public PluginTypes? PluginType { get; set; }
            public string? LibraryPath { get; set; }

            public enum PluginTypes
            {
                Pdf
            }
        }
    }
}
