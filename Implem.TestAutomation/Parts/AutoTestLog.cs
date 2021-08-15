using CsvHelper.Configuration;
using Implem.Pleasanter.Libraries.Requests;
using System;
namespace Implem.TestAutomation.Parts
{
    public class AutoTestLog
    {
        public DateTime Time { get; set; }
        public string Message { get; set; }
    }

    public class AutoTestLogTable : ClassMap<AutoTestLog>
    {
        private AutoTestLogTable()
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            Map(c => c.Time).Index(0).Name("Time");
            Map(c => c.Message).Index(1).Name("Message");
        }
    }
}
