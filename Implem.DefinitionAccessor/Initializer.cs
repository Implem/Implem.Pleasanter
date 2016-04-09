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
        public static void Initialize(string modulePath, bool codeDefiner = false)
        {
            SetParameters(new DirectoryInfo(new FileInfo(modulePath).DirectoryName));
            Environments.CodeDefiner = codeDefiner;
            Environments.CurrentDirectoryPath = GetCurrentDirectoryPath(modulePath);
            Environments.ServiceName = new DirectoryInfo(Directories.ServicePath()).Name;
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

        private static void SetParameters(DirectoryInfo currentDirectory)
        {
            foreach (var sub in currentDirectory.GetDirectories())
            {
                var path = Path.Combine(
                    sub.FullName, "App_Data", "Definitions", "Parameters.json");
                if (Files.Exists(path))
                {
                    Def.Parameters = Files.Read(path).Deserialize<Parameters>();
                    return;
                }
            }
            SetParameters(currentDirectory.Parent);
        }

        private static string GetCurrentDirectoryPath(string currentDirectoryPath)
        {
            var path = string.Empty;
            var dir = new DirectoryInfo(currentDirectoryPath);
            while (dir != null)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "App_Data")))
                {
                    path = dir.FullName;
                    break;
                }
                dir = dir.Parent;
            }
            return path;
        }

        public static void SetDefinitions()
        {
            Def.SetCodeDefinition();
            Def.SetColumnDefinition();
            Def.SetCssDefinition();
            Def.SetDataViewDefinition();
            Def.SetDbDefinition();
            Def.SetJavaScriptDefinition();
            Def.SetDisplayDefinition();
            Def.SetSqlDefinition();
            SetDbDefinitionAdditional();
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

        private static void SetDbDefinitionAdditional()
        {
            Def.Db.DbSa = Strings.CoalesceEmpty(
                Environment.GetEnvironmentVariable("Implem_" + Environments.ServiceName + "_DbSa"),
                Environment.GetEnvironmentVariable("Implem_DbSa"),
                Def.Db.DbSa.Replace("#ServiceName#", Environments.ServiceName));
            Def.Db.DbOwner = Strings.CoalesceEmpty(
                Environment.GetEnvironmentVariable("Implem_" + Environments.ServiceName + "_DbOwner"),
                Environment.GetEnvironmentVariable("Implem_DbOwner"),
                Def.Db.DbOwner.Replace("#ServiceName#", Environments.ServiceName));
            Def.Db.DbUser = Strings.CoalesceEmpty(
                Environment.GetEnvironmentVariable("Implem_" + Environments.ServiceName + "_DbUser"),
                Environment.GetEnvironmentVariable("Implem_DbUser"),
                Def.Db.DbUser.Replace("#ServiceName#", Environments.ServiceName));
            switch (Def.Db.DbEnvironmentType)
            {
                case "Local": 
                    Environments.DbEnvironmentType = Sqls.DbEnvironmentTypes.Local;
                    break;
                case "Azure":
                    Environments.DbEnvironmentType = Sqls.DbEnvironmentTypes.Azure;
                    Azures.SetRetryManager(
                        Def.Parameters.SqlAzureRetryCount,
                        Def.Parameters.SqlAzureRetryInterval);
                    break;
                default: break;
            }
            Environments.DbTimeZoneInfo = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(o => o.Id == Def.Db.DbTimeZoneInfo);
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
