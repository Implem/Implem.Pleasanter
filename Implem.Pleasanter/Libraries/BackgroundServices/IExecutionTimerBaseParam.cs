using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public interface IExecutionTimerBaseParam
    {
        bool Enabled { get; }
        JobKey JobKey { get; }
        Type JobType { get; }
        string JobName { get; }
        IEnumerable<string> TimeList { get; }
        Task<bool> SetCustomTimer(IScheduler scheduler);
    }
}