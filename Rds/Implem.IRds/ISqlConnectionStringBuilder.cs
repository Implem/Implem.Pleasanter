namespace Implem.IRds
{
    public interface ISqlConnectionStringBuilder
    {
        string InitialCatalog { get; set; }
        string ConnectionString { get; set; }
    }
}
