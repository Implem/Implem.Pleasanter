using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class BackgroundService
    {
        public bool Reminder;
        public int ReminderIgnoreConsecutiveExceptionCount;
        public bool SyncByLdap;
        public List<string> SyncByLdapTime;
        public bool DeleteSysLog;
        public List<string> DeleteSysLogTime;
        public bool DeleteTemporaryFiles;
        public List<string> DeleteTemporaryFilesTime;

        public bool TimerEnabled()
        {
            //TimerBackgroundServiceを使うものをここの条件に追加
            return SyncByLdap
                || DeleteSysLog
                || DeleteTemporaryFiles;
        }
    }
}