using Implem.SupportTools.SysLogViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.SupportTools.SysLogViewer.ViewModel
{
    public class DetailWindowViewModel
    {
        public SysLogModel Item { get; }
        public DetailWindowViewModel(SysLogModel sysLogModel)
        {
            Item = sysLogModel;
        }
    }
}
