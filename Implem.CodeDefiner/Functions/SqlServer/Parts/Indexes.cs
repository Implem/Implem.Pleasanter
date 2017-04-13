using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer.Parts
{
    internal static class Indexes
    {
        internal static IEnumerable<IndexInfo> IndexInfoCollection(
            string generalTableName, string sourceTableName, bool old)
        {
            var tableIndexCollection = new List<IndexInfo>();
            if (!old)
            {
                if (Def.ColumnDefinitionCollection.Any(o =>
                    o.TableName == generalTableName && o.Pk > 0))
                {
                    tableIndexCollection.Add(new IndexInfo(
                        sourceTableName,
                        IndexInfo.Types.Pk,
                        "Pk",
                        Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == generalTableName)
                            .Where(o => o.Pk > 0)
                            .OrderBy(o => o.Pk)
                            .Select(o => new IndexInfo.Column(
                                o.ColumnName, o.Pk, o.PkOrderBy))
                            .ToList()));
                }
                if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix1 > 0))
                {
                    tableIndexCollection.Add(new IndexInfo(
                        sourceTableName,
                        IndexInfo.Types.Ix,
                        "Ix1",
                        Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == generalTableName)
                            .Where(o => o.Ix1 > 0)
                            .OrderBy(o => o.Ix1)
                            .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix1, o.Ix1OrderBy, o.Unique))
                            .ToList()));
                }
                if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix2 > 0))
                {
                    tableIndexCollection.Add(new IndexInfo(
                        sourceTableName,
                        IndexInfo.Types.Ix,
                        "Ix2",
                        Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == generalTableName)
                            .Where(o => o.Ix2 > 0)
                            .OrderBy(o => o.Ix2)
                            .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix2, o.Ix2OrderBy, o.Unique))
                            .ToList()));
                }
                if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix3 > 0))
                {
                    tableIndexCollection.Add(new IndexInfo(
                        sourceTableName,
                        IndexInfo.Types.Ix,
                        "Ix3",
                        Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == generalTableName)
                            .Where(o => o.Ix3 > 0)
                            .OrderBy(o => o.Ix3)
                            .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix3, o.Ix3OrderBy, o.Unique))
                            .ToList()));
                }
            }
            else
            {
                if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Pk > 0))
                {
                    tableIndexCollection.Add(new IndexInfo(
                        sourceTableName,
                        IndexInfo.Types.Pk,
                        "PkHistory",
                        Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == generalTableName)
                            .Where(o => o.History > 0)
                            .Where(o => o.PkHistory > 0)
                            .OrderBy(o => o.PkHistory)
                            .Select(o => new IndexInfo.Column(
                                o.ColumnName, o.PkHistory, o.PkHistoryOrderBy))
                            .ToList()));
                }
            }
            return tableIndexCollection;
        }

        public static IEnumerable<string> Get(string sourceTableName)
        {
            return Def.SqlIoByAdmin().ExecuteTable(
                Def.Sql.Indexes.Replace("#TableName#", sourceTableName))
                    .AsEnumerable()
                    .Select(o => o["Name"].ToString())
                    .Distinct()
                    .OrderBy(o => o);
        }

        internal static bool HasChanges(
            string generalTableName,
            string sourceTableName,
            bool old,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            return IndexInfoCollection(generalTableName, sourceTableName, old)
                .Select(o => o.IndexName())
                .Distinct()
                .OrderBy(o => o)
                .Join(",") != Get(sourceTableName).Join(",");
        }

        private static string Sql_CreateIx(
            string sourceTableName,
            IndexInfo tableIndex,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            if (tableIndex != null)
            {
                return tableIndex.ColumnCollection
                    .Where(o => o.No > 0)
                    .Select(o => "[" + o.ColumnName + "] " + o.OrderType.ToString())
                    .Join(", ");
            }
            else
            {
                return string.Empty;
            }
        }

        internal static void CreatePk(
            this SqlStatement sqlStatement,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection)
        {
            var tableIndex = tableIndexCollection
                .Where(o => o.Type == IndexInfo.Types.Pk).FirstOrDefault();
            if (tableIndex != null)
            {
                sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                    "#Pks#", Def.Sql.CreatePk
                        .Replace("#PkName#", tableIndex.IndexName())
                        .Replace("#PkColumns#", string.Join(",", Sql_CreateIx(
                            sourceTableName, tableIndex, columnDefinitionCollection))));
            }
            else
            {
                sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                    "#Pks#", string.Empty);
            }
        }

        internal static void CreateIx(
            this SqlStatement sqlStatement,
            string generalTableName,
            string sourceTableName,
            bool old,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            IndexInfoCollection(generalTableName, sourceTableName, old)
                .Where(o => o.Type == IndexInfo.Types.Ix)
                .ForEach(tableIndex => 
                    sqlStatement.CreateIx(sourceTableName, columnDefinitionCollection, tableIndex));
        }

        private static void CreateIx(
            this SqlStatement sqlStatement,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IndexInfo tableIndex)
        {
            sqlStatement.CommandText += Def.Sql.CreateIx
                .Replace("#IxName#", tableIndex.IndexName())
                .Replace("#IxColumns#", Sql_CreateIx(sourceTableName, tableIndex, columnDefinitionCollection))
                .Replace("#Unique#", Sql_CreateIxUnique(tableIndex));
        }

        private static string Sql_CreateIxUnique(IndexInfo tableIndex)
        {
            if (tableIndex.ColumnCollection.All(o => o.Unique))
            {
                return "unique";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
