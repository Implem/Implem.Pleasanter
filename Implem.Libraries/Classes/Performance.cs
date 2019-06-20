using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Libraries.Classes
{
    public class Performance
    {
        public DateTime StartTime;
        public DateTime StopTime;
        public double TimeElapsed = 0;
        public bool Running = false;

        public Performance()
        {
        }

        public void Start()
        {
            StartTime = DateTime.Now;
            Running = true;
        }

        public void Stop()
        {
            StopTime = DateTime.Now;
            if (StartTime.ToOADate() == 0 || StopTime.ToOADate() == 0)
            {
                TimeElapsed = -99999;
            }
            else
            {
                TimeElapsed += (StopTime - StartTime).TotalMilliseconds;
            }
            Running = false;
        }

        public void Restart()
        {
            StartTime = DateTime.Now;
            StopTime = new DateTime();
            Running = true;
        }
    }

    [Serializable]
    public class PerformanceCollection : Dictionary<string, Performance>
    {
        public PerformanceCollection()
        {
        }

        protected PerformanceCollection(
            SerializationInfo serializationInfo, 
            StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { 
        }

        public override void GetObjectData(
            SerializationInfo serializationInfo, 
            StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }

        [Conditional("DEBUG")]
        public void Record(string name)
        {
            if (ContainsKey(name))
            {
                if (this[name].Running)
                {
                    this[name].Stop();
                }
                else
                {
                    this[name].Restart();
                }
            }
            else
            {
                var performance = new Performance();
                performance.Start();
                Add(name, performance);
            }
        }

        [Conditional("DEBUG")]
        public void Save(string logsPath)
        {
            var csvLineCollection = new SortedSet<string>();
            this.ForEach(data => csvLineCollection.Add("{0, 16}".Params(
                data.Value.TimeElapsed.ToString("#,0.0000")) + "\t" + data.Key));
            var csv = csvLineCollection.Reverse().JoinReturn();
            Consoles.Write("\r\n" + csv, Consoles.Types.Info);
            csv.Write(Path.Combine(logsPath, "Performance" + Strings.NewGuid() + ".csv"));
        }
    }
}
