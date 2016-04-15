using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace Implem.DefinitionAccessor
{
    public class Initializer
    {
        public static void Initialize(string path, bool codeDefiner = false)
        {
            Environments.CodeDefiner = codeDefiner;
            Environments.CurrentDirectoryPath = GetCurrentDirectoryPath(
                new FileInfo(path).Directory);
            SetParameters();
            Environments.ServiceName = Def.Parameters.ServiceName;
            SetRdsParameters();
            Environments.MachineName = Environment.MachineName;
            Environments.Application = 
                Assembly.GetExecutingAssembly().ManifestModule.Name.FileNameOnly();
            Environments.AssemblyVersion = 
                Assembly.GetExecutingAssembly().GetName().Version.ToString();
            SetDefinitions();
            Environments.TimeZoneInfoDefault = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(o => o.Id == Def.Parameters.TimeZoneDefault);
            SetSqls();
        }

        private static void SetParameters()
        {
            var path = Path.Combine(
                Environments.CurrentDirectoryPath, "App_Data", "Definitions", "Parameters.json");
            if (Files.Exists(path))
            {
                Def.Parameters = Files.Read(path).Deserialize<Parameters>();
            }
        }

        private static string GetCurrentDirectoryPath(DirectoryInfo currentDirectory)
        {
            foreach (var sub in currentDirectory.GetDirectories())
            {
                var path = Path.Combine(
                    sub.FullName, "App_Data", "Definitions", "Parameters.json");
                if (Files.Exists(path))
                {
                    return new FileInfo(path).Directory.Parent.Parent.FullName;
                }
            }
            return GetCurrentDirectoryPath(currentDirectory.Parent);
        }

        public static void SetDefinitions()
        {
            Def.SetCodeDefinition();
            Def.SetColumnDefinition();
            Def.SetCssDefinition();
            Def.SetDataViewDefinition();
            Def.SetJavaScriptDefinition();
            Def.SetDisplayDefinition();
            Def.SetSqlDefinition();
            SetDisplayDefinitionAdditional();
        }

        public static XlsIo DefinitionFile(string fileName)
        {
            var tempFile = new FileInfo(Files.CopyToTemp(
                Directories.Definitions(fileName), Directories.Temp()));
            var xlsIo = new XlsIo(tempFile.FullName);
            tempFile.Delete();
            if (fileName == "definition_Column.xlsm")
            {
                SetColumnDefinitionAdditional(xlsIo);
            }
            return xlsIo;
        }

        private static void SetRdsParameters()
        {
            Def.Parameters.RdsSaConnectionString = 
                Def.Parameters.RdsSaConnectionString.Replace(
                    "#ServiceName#", Environments.ServiceName);
            Def.Parameters.RdsOwnerConnectionString = 
                Def.Parameters.RdsOwnerConnectionString.Replace(
                    "#ServiceName#", Environments.ServiceName);
            Def.Parameters.RdsUserConnectionString = 
                Def.Parameters.RdsUserConnectionString.Replace(
                    "#ServiceName#", Environments.ServiceName);
            switch (Def.Parameters.RdsType)
            {
                case "Local": 
                    Environments.RdsProvider = Sqls.RdsProviders.Local;
                    break;
                case "Azure":
                    Environments.RdsProvider = Sqls.RdsProviders.Azure;
                    Azures.SetRetryManager(
                        Def.Parameters.SqlAzureRetryCount,
                        Def.Parameters.SqlAzureRetryInterval);
                    break;
                default: break;
            }
            Environments.RdsTimeZoneInfo = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(o => o.Id == Def.Parameters.RdsTimeZoneInfo);
        }

        private static void SetColumnDefinitionAdditional(XlsIo definitionFile)
        {
            var tableCopy = definitionFile.XlsSheet.ToList<XlsRow>();
            var sheet = definitionFile.XlsSheet;
            definitionFile.XlsSheet.Select(o => new
            {
                ModelName = o["ModelName"].ToStr(),
                TableName = o["TableName"].ToStr(),
                Label = o["Label"].ToStr(),
                Base = o["Base"].ToBool()
            })
                .Where(o => !o.Base && o.TableName != "string")
                .Distinct()
                .ForEach(column =>
                    sheet.Where(o => o["Base"].ToBool()).ForEach(commonColumnDefinition =>
                    {
                        if (IsTargetColumn(sheet, commonColumnDefinition, column.TableName) && 
                            IsNotExists(tableCopy, commonColumnDefinition, column.TableName))
                        {
                            var copyColumnDefinition = new XlsRow();
                            definitionFile.XlsSheet.Columns.ForEach(xcolumn =>
                                copyColumnDefinition[xcolumn] = commonColumnDefinition[xcolumn]);
                            copyColumnDefinition["Id"] =
                                column.TableName + "_" + copyColumnDefinition["ColumnName"];
                            copyColumnDefinition["ModelName"] = column.ModelName;
                            copyColumnDefinition["TableName"] = column.TableName;
                            copyColumnDefinition["Label"] = column.Label;
                            copyColumnDefinition["Base"] = "0";
                            copyColumnDefinition["ItemId"] = "0";
                            tableCopy.Add(copyColumnDefinition);
                        }
                    }));
            definitionFile.XlsSheet = new XlsSheet(tableCopy, definitionFile.XlsSheet.Columns);
        }

        private static bool IsTargetColumn(
            XlsSheet sheet, XlsRow commonColumnDefinition, string tableName)
        {
            return commonColumnDefinition["ItemId"].ToInt() == 0 ||
                sheet.Any(o => o["TableName"].ToString() == tableName &&
                    o["ItemId"].ToInt() > 0);
        }

        private static bool IsNotExists(
            List<XlsRow> tableCopy, XlsRow commonColumnDefinition, string tableName)
        {
            return !tableCopy.Any(o => o["TableName"].ToString() == tableName &&
                o["ColumnName"].ToString() == commonColumnDefinition["ColumnName"].ToString());
        }

        private static void SetDisplayDefinitionAdditional()
        {
            Def.ColumnDefinitionCollection
                .Where(o => !o.Base)
                .Select(o => new { Id = o.Id, Content = o.ColumnLabel })
                .Union(Def.ColumnDefinitionCollection
                .Where(o => !o.Base)
                .Select(o => new { Id = o.TableName, Content = o.Label })
                .Distinct())
                .Where(o => !Def.DisplayDefinitionCollection.Any(p => p.Id == o.Id))
                .ForEach(data => Def.DisplayDefinitionCollection.Add(new DisplayDefinition()
                {
                    Id = data.Id,
                    Content = data.Content
                }));
        }

        private static void SetSqls()
        {
            Sqls.LogsPath = Directories.Logs();
            Sqls.BeginTransaction = Def.Sql.BeginTransaction;
            Sqls.CommitTransaction = Def.Sql.CommitTransaction;
        }
    }
}
