using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
namespace Implem.Pleasanter.Libraries.Models
{
    public class ImageLibData
    {
        public EnumerableRowCollection<DataRow> DataRows;
        public int TotalCount;

        public ImageLibData(SiteSettings ss, View view, int offset = 0, int pageSize = 0)
        {
            var idColumnBracket = $"[{Rds.IdColumn(ss.ReferenceType)}]";
            var dataSet = Rds.ExecuteDataSet(statements:
                Rds.Select(
                    tableName: ss.ReferenceType,
                    dataTableName: "Main",
                    column: new SqlColumnCollection()
                        .Add(
                            columnBracket: idColumnBracket,
                            tableName: ss.ReferenceType,
                            _as: "Id")
                        .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType))
                        .Add(tableName: "Binaries", columnBracket: "[Guid]"),
                    join: new SqlJoinCollection()
                        .Add(
                            tableName: "Binaries",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: $"[Binaries].[ReferenceId]=[{ss.ReferenceType}].{idColumnBracket}"),
                    where: view.Where(ss: ss),
                    orderBy: view.OrderBy(ss: ss, pageSize: pageSize),
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: true));
            DataRows = dataSet.Tables["Main"].AsEnumerable();
            TotalCount = Rds.Count(dataSet);
        }
    }
}