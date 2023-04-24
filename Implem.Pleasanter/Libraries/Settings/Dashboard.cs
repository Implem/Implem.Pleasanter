using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.Settings
{

    public class Dashboard : ISettingListItem 
    {
        public int Id { get; set; }
        public DashboardType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<long> Sites { get; set; }

        public Dashboard GetRecordingData()
        {
            var dashboard = new Dashboard();
            dashboard.Id = Id;
            dashboard.Width = Width;
            dashboard.Height = Height;
            dashboard.X = X;
            dashboard.Y = Y;
            dashboard.Sites = Sites;
            return dashboard;
        }
    }
}
