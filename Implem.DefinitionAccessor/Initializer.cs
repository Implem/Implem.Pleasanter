using Implem.DisplayAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
namespace Implem.DefinitionAccessor
{
    public class Initializer
    {
        public static void Initialize(
            string path,
            string assemblyVersion,
            bool codeDefiner = false,
            bool setSaPassword = false,
            bool setRandomPassword = false)
        {
            Environments.CodeDefiner = codeDefiner;
            Environments.CurrentDirectoryPath = path != null
                ? path
                : GetSourcePath();
            SetRdsPassword(setSaPassword, setRandomPassword);
            SetParameters();
            Environments.ServiceName = Parameters.Service.Name;
            SetRdsParameters();
            Environments.MachineName = Environment.MachineName;
            Environments.Application = 
                Assembly.GetExecutingAssembly().ManifestModule.Name.FileNameOnly();
            Environments.AssemblyVersion = assemblyVersion;
            SetDefinitions();
            Environments.TimeZoneInfoDefault = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(o => o.Id == Parameters.Service.TimeZoneDefault);
            SetSqls();
            DateTimes.FirstDayOfWeek = Parameters.General.FirstDayOfWeek;
            DateTimes.FirstMonth = Parameters.General.FirstMonth;
            DateTimes.MinTime = Parameters.General.MinTime;
            DateTimes.MaxTime = Parameters.General.MaxTime;
        }

        private static void SetRdsPassword(bool setRdsPassword, bool setRandomPassword)
        {
            if (setRdsPassword)
            {
                Console.WriteLine("Please enter the SA password.");
                var rdsParameters = Files.Read(ParametersPath("Rds"));
                rdsParameters = Regex.Replace(
                    rdsParameters,
                    "(?<=UID\\=sa;PWD\\=).*?(?=;)",
                    Console.ReadLine());
                if (setRandomPassword)
                {
                    rdsParameters = Regex.Replace(
                        rdsParameters,
                        "(?<=UID\\=#ServiceName#_Owner;PWD\\=).*?(?=;)",
                        Strings.NewGuid());
                    rdsParameters = Regex.Replace(
                        rdsParameters,
                        "(?<=UID\\=#ServiceName#_User;PWD\\=).*?(?=;)",
                        Strings.NewGuid());
                }
                rdsParameters.Write(ParametersPath("Rds"));
            }
        }

        public static void SetParameters()
        {
            Parameters.Api = Read<ParameterAccessor.Parts.Api>();
            Parameters.Asset = Read<ParameterAccessor.Parts.Asset>();
            Parameters.Authentication = Read<ParameterAccessor.Parts.Authentication>();
            Parameters.BackgroundTask = Read<ParameterAccessor.Parts.BackgroundTask>();
            Parameters.BinaryStorage = Read<ParameterAccessor.Parts.BinaryStorage>();
            Parameters.ExcludeColumns = Read<ParameterAccessor.Parts.ExcludeColumns>();
            Parameters.CustomDefinitions = CustomDefinitionsHash();
            Parameters.ExtendedColumnsSet = ExtendedColumnsSet();
            Parameters.ExtendedSqls = ExtendedSqls();
            Parameters.ExtendedStyles = ExtendedStyles();
            Parameters.ExtendedScripts = ExtendedScripts();
            Parameters.Formats = Read<ParameterAccessor.Parts.Formats>();
            Parameters.General = Read<ParameterAccessor.Parts.General>();
            Parameters.Health = Read<ParameterAccessor.Parts.Health>();
            Parameters.Mail = Read<ParameterAccessor.Parts.Mail>();
            Parameters.Notification = Read<ParameterAccessor.Parts.Notification>();
            Parameters.Path = Read<ParameterAccessor.Parts.Path>();
            Parameters.Permissions = Read< ParameterAccessor.Parts.Permissions>();
            Parameters.Rds = Read<ParameterAccessor.Parts.Rds>();
            Parameters.Reminder = Read<ParameterAccessor.Parts.Reminder>();
            Parameters.Search = Read<ParameterAccessor.Parts.Search>();
            Parameters.Security = Read<ParameterAccessor.Parts.Security>();
            Parameters.Service = Read<ParameterAccessor.Parts.Service>();
            Parameters.SysLog = Read<ParameterAccessor.Parts.SysLog>();
        }

        private static T Read<T>()
        {
            var name = typeof(T).Name;
            var data = Files.Read(ParametersPath(name)).Deserialize<T>();
            if (data == null)
            {
                Parameters.SyntaxErrors.Add(name + ".json");
            }
            return data;
        }

        private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> CustomDefinitionsHash(
            string path = null,
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> hash = null)
        {
            hash = hash ?? new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            path = path ?? Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "CustomDefinitions");
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles("*.json"))
                {
                    var customDefinitions = Files.Read(file.FullName)
                        .Deserialize<Dictionary<string, Dictionary<string, string>>>();
                    if (customDefinitions != null)
                    {
                        hash.Add(Path.ChangeExtension(file.Name, null), customDefinitions);
                    }
                    else
                    {
                        Parameters.SyntaxErrors.Add(file.Name);
                    }
                }
                foreach (var sub in dir.GetDirectories())
                {
                    hash = CustomDefinitionsHash(sub.FullName, hash);
                }
            }
            return hash;
        }

        private static List<ParameterAccessor.Parts.ExtendedColumns> ExtendedColumnsSet(
            string path = null, List<ParameterAccessor.Parts.ExtendedColumns> list = null)
        {
            list = list ?? new List<ParameterAccessor.Parts.ExtendedColumns>();
            path = path ?? Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedColumns");
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles("*.json"))
                {
                    var extendedColumns = Files.Read(file.FullName)
                        .Deserialize<ParameterAccessor.Parts.ExtendedColumns>();
                    if (extendedColumns != null)
                    {
                        list.Add(extendedColumns);
                    }
                    else
                    {
                        Parameters.SyntaxErrors.Add(file.Name);
                    }
                }
                foreach (var sub in dir.GetDirectories())
                {
                    list = ExtendedColumnsSet(sub.FullName, list);
                }
            }
            return list;
        }

        private static List<ParameterAccessor.Parts.ExtendedSql> ExtendedSqls(
            string path = null, List<ParameterAccessor.Parts.ExtendedSql> list = null)
        {
            list = list ?? new List<ParameterAccessor.Parts.ExtendedSql>();
            path = path ?? Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedSqls");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.json"))
            {
                var extendedSql = Files.Read(file.FullName)
                    .Deserialize<ParameterAccessor.Parts.ExtendedSql>();
                if (extendedSql != null)
                {
                    extendedSql.Path = file.FullName;
                    var sqlPath = file.FullName + ".sql";
                    if (Files.Exists(sqlPath))
                    {
                        extendedSql.CommandText = Files.Read(sqlPath);
                    }
                    list.Add(extendedSql);
                }
                else
                {
                    Parameters.SyntaxErrors.Add(file.Name);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedSqls(dir.FullName, list);
            }
            return list;
        }

        private static List<string> ExtendedStyles(string path = null, List<string> list = null)
        {
            list = list ?? new List<string>();
            path = path ?? Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedStyles");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.css"))
            {
                var style = Files.Read(file.FullName);
                if (style != null)
                {
                    list.Add(style);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedStyles(dir.FullName, list);
            }
            return list;
        }

        private static List<string> ExtendedScripts(string path = null, List<string> list = null)
        {
            list = list ?? new List<string>();
            path = path ?? Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedScripts");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.js"))
            {
                var script = Files.Read(file.FullName);
                if (script != null)
                {
                    list.Add(script);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedScripts(dir.FullName, list);
            }
            return list;
        }

        private static string ParametersPath(string name)
        {
            return Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                name + ".json");
        }

        private static string GetSourcePath()
        {
            var parts = new DirectoryInfo(
                Assembly.GetEntryAssembly().Location).FullName.Split('\\');
            return new DirectoryInfo(Path.Combine(
                parts.Take(Array.IndexOf(parts, "Implem.CodeDefiner")).Join("\\"),
                "Implem.Pleasanter"))
                    .FullName;
        }

        public static void SetDefinitions()
        {
            Def.SetCodeDefinition();
            Def.SetColumnDefinition();
            Def.SetCssDefinition();
            Def.SetTemplateDefinition();
            Def.SetViewModeDefinition();
            Def.SetDemoDefinition();
            Def.SetSqlDefinition();
            SetDisplayAccessor();
            if (Parameters.Enterprise)
            {
                SetExtendedColumnDefinitions();
            }
        }

        private static void SetExtendedColumnDefinitions()
        {
            Parameters.ExtendedColumnsSet.ForEach(extendedColumns =>
            {
                var data = new Dictionary<string, int>
                {
                    { "Class", extendedColumns.Class },
                    { "Num", extendedColumns.Num },
                    { "Date", extendedColumns.Date },
                    { "Description", extendedColumns.Description },
                    { "Check", extendedColumns.Check },
                    { "Attachments", extendedColumns.Attachments }
                };
                data.ForEach(part =>
                {
                    for (var i = 1; i <= part.Value; i++)
                    {
                        var id = string.Format("{0:D3}", i);
                        var columnName = part.Key + id;
                        var def = Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == extendedColumns.ReferenceType)
                            .FirstOrDefault(o => o.ColumnName == part.Key + "A")
                            .Copy();
                        def.Id = $"{extendedColumns.TableName}_{columnName}";
                        def.TableName = extendedColumns.TableName;
                        def.Label = extendedColumns.Label ?? def.Label;
                        def.ColumnName = columnName;
                        def.LabelText = def.LabelText
                            .Substring(0, def.LabelText.Length -1) + id;
                        Def.ColumnDefinitionCollection.Add(def);
                    }
                });
            });
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
            Parameters.Rds.SaConnectionString = 
                Parameters.Rds.SaConnectionString.Replace(
                    "#ServiceName#", Environments.ServiceName);
            Parameters.Rds.OwnerConnectionString = 
                Parameters.Rds.OwnerConnectionString.Replace(
                    "#ServiceName#", Environments.ServiceName);
            Parameters.Rds.UserConnectionString = 
                Parameters.Rds.UserConnectionString.Replace(
                    "#ServiceName#", Environments.ServiceName);
            switch (Parameters.Rds.Provider)
            {
                case "Azure":
                    Environments.RdsProvider = "Azure";
                    Azures.SetRetryManager(
                        Parameters.Rds.SqlAzureRetryCount,
                        Parameters.Rds.SqlAzureRetryInterval);
                    break;
                default:
                    Environments.RdsProvider = "Local";
                    break;
            }
            Environments.RdsTimeZoneInfo = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(o => o.Id == Parameters.Rds.TimeZoneInfo);
            Environments.DeadlockRetryCount = Parameters.Rds.DeadlockRetryCount;
            Environments.DeadlockRetryInterval = Parameters.Rds.DeadlockRetryInterval;
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

        private static void SetDisplayAccessor()
        {
            Displays.DisplayHash = DisplayHash();
            Def.ColumnDefinitionCollection
                .Where(o => !o.Base)
                .Select(o => new { o.Id, Body = o.LabelText })
                .Union(Def.ColumnDefinitionCollection
                    .Where(o => !o.Base)
                    .Select(o => new { Id = o.TableName, Body = o.Label })
                    .Distinct())
                .Where(o => !Displays.DisplayHash.ContainsKey(o.Id))
                .ForEach(o => Displays.DisplayHash.Add(
                    o.Id, new Display
                    {
                        Id = o.Id,
                        Languages = new List<DisplayElement>
                        {
                            new DisplayElement { Body = o.Body }
                        }
                    }));
        }

        private static Dictionary<string, Display> DisplayHash()
        {
            var hash = new Dictionary<string, Display>();
            new DirectoryInfo(Directories.Displays()).GetFiles("*.json").ForEach(file =>
            {
                var data = Files.Read(file.FullName).Deserialize<Display>();
                hash.Add(data.Id, data);
            });
            return hash;
        }

        private static void SetSqls()
        {
            Sqls.LogsPath = Directories.Logs();
            Sqls.SelectIdentity = Def.Sql.SelectIdentity;
            Sqls.BeginTransaction = Def.Sql.BeginTransaction;
            Sqls.CommitTransaction = Def.Sql.CommitTransaction;
        }
    }
}
