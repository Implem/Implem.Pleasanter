using Implem.Pleasanter.Libraries.DataTypes;
using System;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class KambanElement
    {
        public long Id;
        public string Title;
        public DateTime StartTime;
        public CompletionTime CompletionTime;
        public WorkValue WorkValue;
        public ProgressRate ProgressRate;
        public decimal RemainingWorkValue;
        public Status Status;
        public User Manager;
        public User Owner;
        public string GroupX;
        public string GroupY;
        public decimal Value;

        public KambanElement()
        {
        }
    }
}