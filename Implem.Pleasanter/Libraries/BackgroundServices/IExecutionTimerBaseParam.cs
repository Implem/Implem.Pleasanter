using Quartz;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public interface IExecutionTimerBaseParam
    {
        bool Enabled { get; }
        JobKey JobKey { get; }
        Type JobType { get; }
        IEnumerable<string> TimeList { get; }
    }
}