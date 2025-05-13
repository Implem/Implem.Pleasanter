using System.Collections.Generic;
using System.Linq;

namespace Implem.ParameterAccessor.Parts
{
    public class BackgroundService
    {
        public List<string> EnvironmentVariables;
        public bool Reminder;
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

        public bool TimerEnabled(string deploymentEnvironment)
        {
            return ServiceEnabled(deploymentEnvironment)
                && (SyncByLdap
                || DeleteSysLogs
                || DeleteTemporaryFiles
                || DeleteTrashBox
                || Reminder);
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