namespace Implem.Pleasanter.Interfaces
{
    public interface IExportModel
    {
        void AddDestination(IExportModel exportModel, string columnName);
        void AddSource(IExportModel exportModel);
    }
}