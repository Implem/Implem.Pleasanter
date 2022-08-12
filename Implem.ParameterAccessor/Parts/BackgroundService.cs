using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class BackgroundService
    {
        public bool Reminder;
        public int ReminderIgnoreConsecutiveExceptionCount;
        public bool SyncByLdapTimer;
        public List<string> SyncByLdapTimerTime;

        public bool TimerEnabled()
        {
            //TimerBackgroundServiceを使うものをここの条件に追加
            return SyncByLdapTimer;
        }
    }
}