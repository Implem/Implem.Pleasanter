﻿using Implem.DefinitionAccessor;
using System.Collections.Generic;
using System.Linq;

namespace Implem.ParameterAccessor.Parts
{
    public class BackgroundService
    {
        public List<string> EnvironmentVariables;
        public bool Reminder;
        public int ReminderIgnoreConsecutiveExceptionCount;
        public bool SyncByLdap;
        public List<string> SyncByLdapTime;
        public bool DeleteSysLogs;
        public List<string> DeleteSysLogsTime;
        public bool DeleteTemporaryFiles;
        public List<string> DeleteTemporaryFilesTime;
        public bool DeleteTrashBox;
        public List<string> DeleteTrashBoxTime;
        public int DeleteTrashBoxRetentionPeriod;

        public bool TimerEnabled(string deploymentEnvironment)
        {
            //TimerBackgroundServiceを使うものをここの条件に追加
            return ServiceEnabled(deploymentEnvironment)
                && (SyncByLdap
                || DeleteSysLogs
                || DeleteTemporaryFiles
                || DeleteTrashBox);
        }

        public bool ReminderEnabled(string deploymentEnvironment)
        {
            return Reminder && ServiceEnabled(deploymentEnvironment);
        }

        private bool ServiceEnabled(string deploymentEnvironment)
        {
            if(EnvironmentVariables == null)
            {
                return true;
            }
            return EnvironmentVariables.Any(o => o == deploymentEnvironment);
        }
    }
}