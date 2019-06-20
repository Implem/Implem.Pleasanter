using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
namespace Implem.Pleasanter.Libraries.Models
{
    public class ImageLibData
    {
        public EnumerableRowCollection<DataRow> DataRows;
        public int TotalCount;

        public ImageLibData(
            Context context,
            SiteSettings ss,
            View view,
            int offset = 0,
            int pageSize = 0)
        {
            var idColumnBracket = $"[{Rds.IdColumn(ss.ReferenceType)}]";
            var column = new SqlColumnCollection()
                .Add(
                    columnBracket: idColumnBracket,
                    tableName: ss.ReferenceType,
                    _as: "Id")
                .ItemTitle(ss.ReferenceType)
                .Add(tableName: "Binaries", columnBracket: "[Guid]");
            var where = view.Where(
                context: context,
                ss: ss)
                    .Binaries_BinaryType("Images");
            var orderBy = view.OrderBy(
                context: context,
                ss: ss);
            var joinExpression = $"[Binaries].[ReferenceId]=[{ss.ReferenceType}].{idColumnBracket}";
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    column,
                    where,
                    orderBy
                })
                    .Add(
                        tableName: "Binaries",
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: joinExpression);
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.Select(
                        tableName: ss.ReferenceType,
                        dataTableName: "Main",
                        column: column,
                        join: join,
                        where: where,
                        orderBy: orderBy,
                        offset: offset,
                        pageSize: pageSize),
                    Rds.SelectCount(
                        tableName: ss.ReferenceType,
                        join: join,
                        where: where)
                });
            DataRows = dataSet.Tables["Main"].AsEnumerable();
            TotalCount = Rds.Count(dataSet);
        }
    }
}