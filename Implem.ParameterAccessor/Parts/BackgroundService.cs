using System.Collections.Generic;
using System.Linq;

namespace Implem.ParameterAccessor.Parts
{
    public class BackgroundService
    {
        public List<string> EnvironmentVariables;
        public bool Reminder;
        public int ReminderCheckIntervalSeconds = 60;
        public bool SyncByLdap;
        public List<string> SyncByLdapTime;
        public bool DeleteSysLogs;
        public int DeleteSysLogsChunkSize;
        public List<string> DeleteSysLogsTime;
        public bool DeleteTemporaryFiles;
        public List<string> DeleteTemporaryFilesTime;
        public bool DeleteTrashBox;
        public List<string> DeleteTrashBoxTime;
        public int DeleteTrashBoxRetentionPeriod;
        public bool DeleteUnusedRecord;
        public List<string> DeleteUnusedRecordTime;
        public int DeleteUnusedRecordChunkSize;
        public bool DeleteMcpLogs = false;
        public List<string> DeleteMcpLogsTime = new List<string>();
        public int McpLogsRetentionPeriod = 90;
        public int DeleteMcpLogsChunkSize;
        public bool DeleteBackgroundJobs = false;
        public List<string> DeleteBackgroundJobsTime = new List<string> { "04:00" };
        public int BackgroundJobsRetentionPeriod = 7;

        public bool TimerEnabled(
            string deploymentEnvironment,
            bool backgroundQueueEnabled = false)
        {
            return ServiceEnabled(deploymentEnvironment)
                && (SyncByLdap
                || DeleteSysLogs
                || DeleteTemporaryFiles
                || DeleteTrashBox
                || Reminder
                || DeleteUnusedRecord
                || DeleteMcpLogs
                || backgroundQueueEnabled
                || DeleteBackgroundJobs);
        }

        private bool ServiceEnabled(string deploymentEnvironment)
        {
            if (EnvironmentVariables == null)
            {
                return true;
            }
            return EnvironmentVariables.Any(o => o == deploymentEnvironment);
        }
    }
}
