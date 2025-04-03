using Fare;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class DragStateApiSettingModel
    {
        public int? Edit { get; set; }
        public int? Grid { get; set; }
        public int? Filter { get; set; }
        //D&D用のパラメータを追加

        public DragStateApiSettingModel()
        {
        }
    }
}