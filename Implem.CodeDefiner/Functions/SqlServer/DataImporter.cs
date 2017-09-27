using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class DataImporter
    {
        internal static void Import()
        {
            Directory.EnumerateFiles(Directories.Imports(), "import_*.xlsx")
                .ForEach(filePath => ImportByXls(filePath));
            Directory.EnumerateFiles(Directories.Imports(), "import_*.csv")
                .ForEach(filePath => ImportByCsv(filePath));
        }

        private static void ImportByCsv(string filePath)
        {
            Consoles.Write(Path.GetFileName(filePath), Consoles.Types.Info);
            var tableName = new FileInfo(filePath).Name
                .Replace("import_", string.Empty).FileNameOnly();
            if (Environments.RdsProvider == "Local")
            {
                BulkInsert(filePath, tableName);
            }
        }

        private static void ImportByXls(string filePath)
        {
            Consoles.Write(Path.GetFileName(filePath), Consoles.Types.Info);
            var tempFile = new FileInfo(Files.CopyToTemp(filePath, Directories.Temp()));
            UpdateOrInsertToTable(
                new FileInfo(filePath).Name.Replace("import_", string.Empty).FileNameOnly(),
                new XlsIo(tempFile.FullName));
            tempFile.Delete();
        }

        private static void UpdateOrInsertToTable(string tableName, XlsIo xlsIo)
        {
            var sqlIo = Def.SqlIoByAdmin();
            SetSqlIo(tableName, xlsIo, sqlIo);
            sqlIo.ExecuteNonQuery();
        }

        private static void SetSqlIo(string tableName, XlsIo xlsIo, SqlIo sqlIo)
        {
            SetIdentityInsert(tableName, sqlIo, identityInsert: true);
            xlsIo.XlsSheet.ForEach(xlsRow =>
                sqlIo.AddSqlStatement(SqlStatement(tableName, xlsIo, xlsRow)));
            SetIdentityInsert(tableName, sqlIo, identityInsert: false);
        }

        private static void SetIdentityInsert(string tableName, SqlIo sqlIo, bool identityInsert)
        {
            if (Def.ExistsTable(tableName, o => o.Identity))
            {
                sqlIo.SqlContainer.SqlStatementCollection.Add(
                new SqlStatement("set identity_insert [" + tableName + "]" + (identityInsert
                    ? " on;"
                    : " off;")));
            }
        }

        private static SqlUpdateOrInsert SqlStatement(
            string tableName, XlsIo xlsIo, XlsRow xlsRow)
        {
            var sqlStatement = new SqlUpdateOrInsert();
            sqlStatement.TableBracket = "[" + tableName + "]";
            xlsIo.XlsSheet.Columns.ForEach(xlsColumn =>
                AddParam(tableName, xlsRow, sqlStatement, xlsColumn));
            return sqlStatement;
        }

        private static void AddParam(
            string tableName,
            XlsRow xlsRow,
            SqlUpdateOrInsert sqlStatement,
            string columnName)
        {
            var value = xlsRow[columnName].ToString();
            if (!value.IsNullOrEmpty())
            {
                if (value == "''") 
                {
                    value = string.Empty; 
                }
                if (IsUniqueColumn(tableName, columnName))
                {
                    AddParam_NoUpdate(tableName, sqlStatement, columnName, value);
                    AddWhere(tableName, sqlStatement, columnName, value);
                }
                else if (IsPkColumn(tableName, columnName))
                {
                    AddParam_NoUpdate(tableName, sqlStatement, columnName, value);
                    if (!HasUniqueColumn(tableName))
                    {
                        AddWhere(tableName, sqlStatement, columnName, value);
                    }
                }
                else if (IsIdentityColumn(tableName, columnName))
                {
                    AddParam_NoUpdate(tableName, sqlStatement, columnName, value);
                }
                else if (IsHashColumn(tableName, columnName))
                {
                    AddParam_Hash(tableName, sqlStatement, columnName, value);
                }
                else if (IsDateTimeColumn(tableName, columnName))
                {
                    AddParam_DateTime(tableName, sqlStatement, columnName, value);
                }
                else
                {
                    AddParam_General(tableName, sqlStatement, columnName, value);
                }
            }
        }

        private static void AddParam_NoUpdate(
            string tableName,
            SqlUpdateOrInsert sqlStatement,
            string columnName,
            string value)
        {
            sqlStatement.SqlParamCollection.Add(
                new SqlParam("[{0}].[{1}]".Params(tableName, columnName),
                columnName, value, updating: false));
        }

        private static void AddParam_Hash(
            string tableName,
            SqlUpdateOrInsert sqlStatement,
            string columnName,
            string value)
        {
            sqlStatement.SqlParamCollection.Add(
                new SqlParam("[{0}].[{1}]".Params(tableName, columnName),
                columnName,
                value.Sha512Cng()));
        }

        private static void AddParam_DateTime(
            string tableName,
            SqlUpdateOrInsert sqlStatement,
            string columnName,
            string value)
        {
            sqlStatement.SqlParamCollection.Add(
                new SqlParam("[{0}].[{1}]".Params(tableName, columnName),
                columnName,
                TimeZoneInfo.ConvertTime(
                    DateTime.FromOADate(double.Parse(value)),
                    Environments.TimeZoneInfoDefault,
                    Environments.RdsTimeZoneInfo).ToString()));
        }

        private static void AddParam_General(
            string tableName,
            SqlUpdateOrInsert sqlStatement,
            string columnName,
            string value)
        {
            sqlStatement.SqlParamCollection.Add(
                new SqlParam("[{0}].[{1}]".Params(tableName, columnName),
                columnName,
                value));
        }

        private static bool IsUniqueColumn(string tableName, string columnName)
        {
            return Def.ExistsTable(tableName, o => o.ColumnName == columnName && o.Unique);
        }

        private static bool HasUniqueColumn(string tableName)
        {
            return Def.ExistsTable(tableName, o => o.Unique);
        }

        private static bool IsPkColumn(string tableName, string columnName)
        {
            return Def.ExistsTable(tableName, o => o.ColumnName == columnName && o.Pk > 0);
        }

        private static bool IsIdentityColumn(string tableName, string columnName)
        {
            return Def.ExistsTable(tableName, o => o.ColumnName == columnName && o.Identity);
        }

        private static bool IsHashColumn(string tableName, string columnName)
        {
            return Def.ExistsTable(tableName, o => o.ColumnName == columnName && o.Hash);
        }

        private static bool IsDateTimeColumn(string tableName, string columnName)
        {
            return Def.ExistsTable(tableName, o => 
                o.ColumnName == columnName && o.TypeName == "datetime");
        }

        private static void AddWhere(
            string tableName,
            SqlUpdateOrInsert sqlStatement,
            string columnName,
            string value)
        {
            sqlStatement.SqlWhereCollection.Add(
                new SqlWhere(
                    tableName: tableName,
                    columnBrackets: new string[] { "[{0}]".Params(columnName) },
                    name: columnName,
                    value: value));
        }

        private static void BulkInsert(string importTextPath, string tableName)
        {
            var importFile = ImportFile(tableName, importTextPath);
            TruncateTable(tableName);
            Def.SqlIoByAdmin(statements: new SqlStatement
            {
                CommandText = Def.Sql.BulkInsert.Params(tableName, importFile.FullName)
            })
            .ExecuteNonQuery();
            importFile.Delete();
        }

        private static void TruncateTable(string tableName)
        {
            Def.SqlIoByAdmin(statements: new SqlStatement
            {
                CommandText = "truncate table " + tableName
            })
            .ExecuteNonQuery();
        }

        private static FileInfo ImportFile(string tableName, string importTextPath)
        {
            var csvColumnNoCollection = CsvColumnNoCollection(tableName);
            var sourceFileTemp = new FileInfo(
                Files.CopyToTemp(importTextPath, Directories.Temp()));
            var importDataBuilder = new StringBuilder();
            using (var reader = new StreamReader(
                sourceFileTemp.FullName, Encoding.GetEncoding("shift_jis")))
                {
                    while (reader.Peek() >= 0)
                    {
                        BuildImportData(csvColumnNoCollection, importDataBuilder, reader);
                    }
                }
            sourceFileTemp.Delete();
            return importDataBuilder.ToString().WriteToTemp(Directories.Temp(), "shift_jis");
        }

        private static Dictionary<string, int> CsvColumnNoCollection(string tableName)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == tableName)
                .Where(o => !o.NotUpdate)
                .Where(o => o.JoinExpression.IsNullOrEmpty())
                .OrderBy(o => o.No)
                .ToDictionary(o => o.ColumnName, o => o.Import);
        }

        private static void BuildImportData(
            Dictionary<string, int> csvColumnNumberCollection,
            StringBuilder importDataBuilder,
            StreamReader reader)
        {
            var line = reader.ReadLine();
            var dataArray = line.Split(',');
            if (line != string.Empty)
            {
                csvColumnNumberCollection.Keys.ForEach(columnName =>
                {
                    if (IsNumberSpecified(csvColumnNumberCollection, columnName))
                    {
                        BuildImportDataOfGeneral(
                            csvColumnNumberCollection,
                            importDataBuilder,
                            dataArray,
                            columnName);
                    }
                    else
                    {
                        BuildImportDataOfRdsSystemColumn(importDataBuilder, columnName);
                    }
                    BuildImportDataOfSeparator(
                        csvColumnNumberCollection, importDataBuilder, columnName);
                });
            }
        }

        private static bool IsNumberSpecified(
            Dictionary<string, int> csvColumnNumberCollection, string columnName)
        {
            return csvColumnNumberCollection[columnName] != 0;
        }

        private static void BuildImportDataOfGeneral(
            Dictionary<string, int> csvColumnNumberCollection,
            StringBuilder importDataBuilder,
            string[] dataArray,
            string columnName)
        {
            importDataBuilder
                .Append(dataArray[csvColumnNumberCollection[columnName] - 1]
                .CutBracket("\""));
        }

        private static void BuildImportDataOfRdsSystemColumn(
            StringBuilder importDataBuilder, string columnName)
        {
            switch (columnName)
            {
                case "Creator": importDataBuilder.Append("1"); break;
                case "Updator": importDataBuilder.Append("1"); break;
            }
        }

        private static void BuildImportDataOfSeparator(
            Dictionary<string, int> columnCollection,
            StringBuilder importDataBuilder,
            string columnName)
        {
            if (columnCollection.Last().Key == columnName)
            {
                importDataBuilder.Append("\r\n");
            }
            else
            {
                importDataBuilder.Append("\t");
            }
        }
    }
}
