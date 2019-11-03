namespace Implem.Pleasanter.Interfaces
{
    [Newtonsoft.Json.JsonConverter(typeof(Libraries.Models.IExportModelConverter))]
    public interface IExportModel
    {
        void AddDestination(IExportModel exportModel, string columnName);
        void AddSource(IExportModel exportModel);
        long? SiteId { get; set; }
        long GetReferenceId();
        string GetReferenceType();
        void ReplaceIdHash(string columnName, System.Collections.Generic.IDictionary<long, long> idHash);
    }
}