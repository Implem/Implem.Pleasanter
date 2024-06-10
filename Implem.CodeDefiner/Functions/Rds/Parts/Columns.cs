using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Implem.Libraries.DataSources.SqlServer.Sqls;
namespace Implem.CodeDefiner.Functions.Rds.Parts
{
    internal static class Columns
    {
        internal static EnumerableRowCollection<DataRow> Get(ISqlObjectFactory factory, string sourceTableName)
        {
            return Def.SqlIoByAdmin(factory: factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.Columns
                    .Replace("#TableName#", sourceTableName)
                    .Replace("#SchemaName#", factory.SqlDefinitionSetting.SchemaName)
                    .Replace("#InitialCatalog#", Environments.ServiceName))
                .AsEnumerable();
        }

        internal static bool HasChanges(
            ISqlObjectFactory factory,
            string sourceTableName, 
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            EnumerableRowCollection<DataRow> rdsColumnCollection)
        {
            return columnDefinitionCollection
                .Select((o, i) => new { ColumnDefinition = o, Count = i })
                .Any(data => HasChanges(
                    factory,
                    sourceTableName,
                    rdsColumnCollection.ToList()[data.Count],
                    data.ColumnDefinition));
        }

        private static bool HasChanges(
            ISqlObjectFactory factory,
            string sourceTableName,
            DataRow rdsColumn,
            ColumnDefinition columnDefinition)
        {
            if (!(rdsColumn["ColumnName"].ToString() == columnDefinition.ColumnName))
            {
                return true;
            }
            if (!(factory.SqlDataType.ConvertBack(rdsColumn["TypeName"].ToString()) == columnDefinition.TypeName))
            {
                return true;
            }
            if (ColumnSize.HasChanges(factory: factory, rdsColumn: rdsColumn, columnDefinition: columnDefinition))
            {
                return true;
            }
            if (ColumnSize.HasChangesMySql(factory: factory, rdsColumn: rdsColumn, columnDefinition: columnDefinition))
            {
                return true;
            }
            if (rdsColumn["is_nullable"].ToBool() != columnDefinition.Nullable)
            {
                return true;
            }
            if (!sourceTableName.EndsWith("_history")
                && !sourceTableName.EndsWith("_deleted")
                && rdsColumn["is_identity"].ToBool() != columnDefinition.Identity)
            {
                return true;
            }
            return false;
        }

        internal static void CreateColumn(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            var sqlCreateColumnCollection = new List<string>();
            columnDefinitionCollection.ForEach(columnDefinition =>
                sqlCreateColumnCollection.Add(Sql_Create(
                    factory,
                    columnDefinition,
                    noIdentity:
                        sourceTableName.EndsWith("_history")
                        || sourceTableName.EndsWith("_deleted"))));
            sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                "#Columns#", sqlCreateColumnCollection.Join(","));
        }

        private static string Sql_Create(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition,
            bool noIdentity)
        {
            var fullTypeText = columnDefinition.TypeName;
            if (Parameters.Rds.Dbms == "MySQL" &&
                columnDefinition.TypeName == "nvarchar" &&
                columnDefinition.MaxLength >= 1024)
            {
                //MySQLにおいてtext型を指定するとエラーになる1024以上のカラムはvarchar(760)に変換する。
                fullTypeText = NeedReduce(factory: factory, columnDefinition: columnDefinition)
                    ? "varchar(760)"
                    : "text";
            }
            else if (columnDefinition.MaxLength == -1)
            {
                fullTypeText += "(max)";
            }
            else if (columnDefinition.TypeName == "decimal")
            {
                fullTypeText += "(" + columnDefinition.Size + ")";
            }
            else
            {
                if (columnDefinition.MaxLength != 0)
                {
                    fullTypeText += "({0})".Params(columnDefinition.MaxLength);
                }
            }
            var commandText = "\"{0}\" {1}".Params(columnDefinition.ColumnName, fullTypeText);
            //MySQLの場合はここではなくCreateModifyColumnで、identityに代えてauto_incrementを設定する。
            if (Parameters.Rds.Dbms != "MySQL" && !noIdentity && columnDefinition.Identity)
            {
                commandText += factory.Sqls.GenerateIdentity.Params(columnDefinition.Seed == 0 ? 1 : columnDefinition.Seed);
            }
            if (columnDefinition.Nullable)
            {
                commandText += " null";
            }
            else
            {
                commandText += " not null";
            }
            return factory.SqlDataType.Convert(commandText);
        }

        internal static bool NeedReduce(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition)
        {
            //MySQLにおいてtext型を指定するとエラーになる条件に当てはまるか判定する
            if (!columnDefinition.Default.IsNullOrEmpty()) return true;
            //_deleteおよび_historyでデータ型の差異が生じないよう通常テーブルのIndex情報を参照する
            foreach (IndexInfo i in Indexes.IndexInfoCollection(
                factory: factory,
                generalTableName: columnDefinition.TableName,
                sourceTableName: columnDefinition.TableName,
                tableType: TableTypes.Normal))
            {
                foreach (IndexInfo.Column c in i.ColumnCollection)
                {
                    if (c.ColumnName == columnDefinition.ColumnName) return true;
                }
            }
            return false;
        }

        internal static void CreateModifyColumn(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            bool NeedsDefault(ColumnDefinition o) {
                return !o.Default.IsNullOrEmpty() &&
                    !(sourceTableName.EndsWith("_history") && o.ColumnName == "Ver");
            }
            bool NeedsAutoIncrement(ColumnDefinition o)
            {
                return o.Identity &&
                    !sourceTableName.EndsWith("_history") &&
                    !sourceTableName.EndsWith("_deleted");
            }
            string SetSeed(ColumnDefinition o)
            {
                var seed = o.Seed == 0 ? 1 : o.Seed;
                return $"\r\nalter table \"#TableName#\" auto_increment = {seed};";
            }
            //MySQL専用のSQLコマンド文字列を生成する。
            if (Parameters.Rds.Dbms != "MySQL") return;
            sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                "#ModifyColumn#", columnDefinitionCollection
                    .Where(o => NeedsDefault(o) || NeedsAutoIncrement(o))
                    .Select(o => Def.Sql.ModifyColumn
                        .Replace("#ColumnDefinition#", Sql_Create(
                            factory: factory,
                            columnDefinition: o,
                            noIdentity: false))
                        .Replace("#Default#", NeedsDefault(o)
                                ? " default " + Constraints.DefaultDefinition(factory, o)
                                : string.Empty)
                        .Replace("#AutoIncrement#", NeedsAutoIncrement(o)
                                ? " auto_increment"
                                : string.Empty)
                        .Replace("#SetSeed#", NeedsAutoIncrement(o)
                                ? SetSeed(o)
                                : string.Empty))
                    .JoinReturn());
        }
    }
}
