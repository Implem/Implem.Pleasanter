using System.Collections.Generic;

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
        public bool DeleteTenant = false;
        public List<string> DeleteTenantTime = new List<string>();
        public int DeleteTenantRetentionPeriod = 30;
        public int DeleteTenantChunkSize = 1000;
        public int DeleteTenantBinariesChunkSize = 10;
        public int WarmupTimeoutSeconds = 180;
        public int WarmupFailureShutdownDelaySeconds = 60;
        public bool DeleteBackgroundJobs = false;
        public List<string> DeleteBackgroundJobsTime = new List<string> { "04:00" };
        public int BackgroundJobsRetentionPeriod = 7;

        public bool TimerEnabled(
            string deploymentEnvironment,
            bool backgroundQueueEnabled = false)
        {
            return EnvironmentVariablesUtilities.IsMatchedEnvironment(
                environmentVariables: EnvironmentVariables,
                deploymentEnvironment: deploymentEnvironment)
                && (SyncByLdap
                || DeleteSysLogs
                || DeleteTemporaryFiles
                || DeleteTrashBox
                || Reminder
                || DeleteUnusedRecord
                || DeleteMcpLogs
                || DeleteTenant
                || backgroundQueueEnabled
                || DeleteBackgroundJobs);
        }

        public bool BackgroundServerScriptEnabled(
            string deploymentEnvironment,
            bool serverScript,
            bool backgroundServerScript)
        {
            return serverScript
                && backgroundServerScript
                && EnvironmentVariablesUtilities.IsMatchedEnvironment(
                    environmentVariables: EnvironmentVariables,
                    deploymentEnvironment: deploymentEnvironment);
        }
    }
}
