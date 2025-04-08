using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
using static Implem.Pleasanter.Libraries.Settings.Column;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable()]
    public class ColumnApiSettingModel {
        public bool IsNewEnable;
        public string Id { get; set; }
        public string ColumnName { get; set; }
        public string LabelText { get; set; }
        public string GridLabelText { get; set; }
        public string Description { get; set; }
        public string InputGuide { get; set; }
        public string ChoicesText { get; set; }
        public string DefaultInput { get; set; }
        public string GridFormat { get; set; }
        public string EditorFormat { get; set; }
        public string ControlType { get; set; }
        public string ChoicesControlType { get; set; }
        public string Format { get; set; }
        public bool? NoWrap { get; set; }
        public bool? ValidateRequired { get; set; }
        public decimal? MaxLength { get; set; }
        public int? DecimalPlaces { get; set; }
        public bool? Nullable { get; set; }
        public string Unit { get; set; }
        public SiteSettings.RoundingTypes? RoundingType { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? Step { get; set; }
        public bool? NoDuplication { get; set; }
        public bool? EditorReadOnly { get; set; }
        public bool? AllowDeleteAttachments { get; set; }
        public bool? Link { get; set; }
        public bool? AllowImage { get; set; }
        public string FieldCss { get; set; }
        public ViewerSwitchingTypes? ViewerSwitchingType { get; set; }
        public SiteSettings.TextAlignTypes? TextAlign { get; set; }
        public decimal? LimitQuantity { get; set; }
        public decimal? LimitSize { get; set; }
        public decimal? TotalLimitSize { get; set; }
        public decimal? ThumbnailLimitSize { get; set; }
        public int? DateTimeStep { get; set; }

        //D&D用のパラメータを追加
        public DragParamsApiSettingModel DragParams;

        public ColumnApiSettingModel()
        {
            DragParams = new DragParamsApiSettingModel();
        }
    }
}