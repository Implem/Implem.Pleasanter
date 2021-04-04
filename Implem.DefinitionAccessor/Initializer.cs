﻿using Implem.DisplayAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
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
            DeleteTemporaryFiles();
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
            Parameters.Api = Read<Api>();
            Parameters.Authentication = Read<Authentication>();
            Parameters.BackgroundTask = Read<BackgroundTask>();
            Parameters.BinaryStorage = Read<BinaryStorage>();
            Parameters.CustomDefinitions = CustomDefinitionsHash();
            Parameters.Deleted = Read<Deleted>();
            Parameters.ExtendedColumnDefinitions = ExtendedColumnDefinitions();
            Parameters.ExtendedColumnsSet = ExtendedColumnsSet();
            Parameters.ExtendedSqls = ExtendedSqls();
            Parameters.ExtendedStyles = ExtendedStyles();
            Parameters.ExtendedScripts = ExtendedScripts();
            Parameters.ExtendedServerScripts = ExtendedServerScripts();
            Parameters.ExtendedHtmls = ExtendedHtmls();
            Parameters.ExtendedTags = ExtendedTags();
            Parameters.General = Read<General>();
            Parameters.History = Read<History>();
            Parameters.Version = Read<ParameterAccessor.Parts.Version>();
            Parameters.Mail = Read<Mail>();
            Parameters.Notification = Read<Notification>();
            Parameters.Permissions = Read<Permissions>();
            Parameters.Rds = Read<Rds>();
            Parameters.Registration = Read<Registration>();
            Parameters.Reminder = Read<Reminder>();
            Parameters.Script = Read<Script>();
            Parameters.Search = Read<Search>();
            Parameters.Security = Read<Security>();
            Parameters.Service = Read<Service>();
            Parameters.Session = Read<Session>();
            Parameters.Site = Read<Site>();
            Parameters.SitePackage = Read<SitePackage>();
            Parameters.SysLog = Read<SysLog>();
            Parameters.User = Read<User>();
            Parameters.Parameter = Read<Parameter>();
            Parameters.Locations= Read<Locations>();
            Parameters.Validation = Read<Validation>();
        }

        public static void ReloadParameters()
        {
            SetParameters();
            SetRdsParameters();
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

        private static Dictionary<string, string> ExtendedColumnDefinitions()
        {
            var hash = new Dictionary<string, string>();
            var path = Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedColumnDefinitions");
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles("*.json"))
                {
                    hash.Add(Files.FileNameOnly(file.Name), Files.Read(file.FullName));
                }
            }
            return hash;
        }

        private static List<ExtendedColumns> ExtendedColumnsSet(
            string path = null, List<ExtendedColumns> list = null)
        {
            list = list ?? new List<ExtendedColumns>();
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
                        .Deserialize<ExtendedColumns>();
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

        private static List<ExtendedSql> ExtendedSqls(
            string path = null, List<ExtendedSql> list = null)
        {
            list = list ?? new List<ExtendedSql>();
            path = path ?? Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedSqls");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.json"))
            {
                var extendedSql = Files.Read(file.FullName)
                    .Deserialize<ExtendedSql>();
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

        private static List<ExtendedStyle> ExtendedStyles(
            string path = null, List<ExtendedStyle> list = null)
        {
            list = list ?? new List<ExtendedStyle>();
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
                    list.Add(new ExtendedStyle()
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        Style = style
                    });
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedStyles(dir.FullName, list);
            }
            return list;
        }

        private static List<ExtendedScript> ExtendedScripts(
            string path = null, List<ExtendedScript> list = null)
        {
            list = list ?? new List<ExtendedScript>();
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
                    list.Add(new ExtendedScript()
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        Script = script
                    });
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedScripts(dir.FullName, list);
            }
            return list;
        }

        private static List<ExtendedServerScript> ExtendedServerScripts(
            string path = null, List<ExtendedServerScript> list = null)
        {
            list = list ?? new List<ExtendedServerScript>();
            path = path ?? Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedServerScripts");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.json"))
            {
                var extendedServerScript = Files.Read(file.FullName)
                    .Deserialize<ExtendedServerScript>();
                if (extendedServerScript != null)
                {
                    extendedServerScript.Path = file.FullName;
                    var sqlPath = file.FullName + ".js";
                    if (Files.Exists(sqlPath))
                    {
                        extendedServerScript.Body = Files.Read(sqlPath);
                    }
                    list.Add(extendedServerScript);
                }
                else
                {
                    Parameters.SyntaxErrors.Add(file.Name);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedServerScripts(dir.FullName, list);
            }
            return list;
        }

        private static List<ExtendedHtml> ExtendedHtmls(
            string path = null,
            List<ExtendedHtml> list = null)
        {
            list = list ?? new List<ExtendedHtml>();
            path = path ?? Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedHtmls");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.html"))
            {
                var extendedHtml = Files.Read(file.FullName);
                if (!extendedHtml.IsNullOrEmpty())
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
                    var displayElement = new DisplayElement
                    {
                        Language = fileNameWithoutExtension?.Split('_').Skip(1).LastOrDefault(),
                        Body = extendedHtml
                    };
                    var name = displayElement.Language.IsNullOrEmpty()
                        ? fileNameWithoutExtension
                        : fileNameWithoutExtension?.Substring(
                            0,
                            fileNameWithoutExtension.Length - displayElement.Language.Length - 1);
                    var listDisplay = new Dictionary<string, List<DisplayElement>>();
                    listDisplay
                        .AddIfNotConainsKey(
                            key: name,
                            value: new List<DisplayElement>())
                        .Get(name)
                        .Add(displayElement);
                    list.Add(new ExtendedHtml()
                    {
                        Html = listDisplay
                    });
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedHtmls(dir.FullName, list);
            }
            return list;
        }

        private static Dictionary<string, string> ExtendedTags()
        {
            var hash = new Dictionary<string, string>();
            var path = Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedTags");
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles("*.html"))
                {
                    hash.Add(Files.FileNameOnly(file.Name), Files.Read(file.FullName));
                }
            }
            return hash;
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
            Displays.DisplayHash = DisplayHash();
            Def.SetCodeDefinition();
            Def.SetColumnDefinition();
            Def.SetCssDefinition();
            Def.SetTemplateDefinition();
            Def.SetViewModeDefinition();
            Def.SetDemoDefinition();
            Def.SetSqlDefinition();
            SetDisplayAccessor();
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

        public static void SetRdsParameters()
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
            Def.ColumnDefinitionCollection
                .Where(o => !o.Base)
                .Select(o => new
                {
                    o.Id,
                    En = o.LabelText_en,
                    Zh = o.LabelText_zh,
                    Ja = o.LabelText,
                    De = o.LabelText_de,
                    Ko = o.LabelText_ko,
                    Es = o.LabelText_es,
                    Vn = o.LabelText_vn
                })
                .Union(Def.ColumnDefinitionCollection
                    .Where(o => !o.Base)
                    .Select(o => new
                    {
                        Id = o.TableName,
                        En = o.TableName,
                        Zh = o.TableName,
                        Ja = o.Label,
                        De = o.TableName,
                        Ko = o.TableName,
                        Es = o.TableName,
                        Vn = o.TableName
                    })
                    .Distinct())
                .Where(o => !Displays.DisplayHash.ContainsKey(o.Id))
                .ForEach(o => Displays.DisplayHash.UpdateOrAdd(
                    o.Id, new Display
                    {
                        Id = o.Id,
                        Languages = new List<DisplayElement>
                        {
                            new DisplayElement
                            {
                                Body = o.En
                            },
                            new DisplayElement
                            {
                                Language = "zh",
                                Body = o.Zh
                            },
                            new DisplayElement
                            {
                                Language = "ja",
                                Body = o.Ja
                            },
                            new DisplayElement
                            {
                                Language = "de",
                                Body = o.De
                            },
                            new DisplayElement
                            {
                                Language = "ko",
                                Body = o.Ko
                            },
                            new DisplayElement
                            {
                                Language = "es",
                                Body = o.Es
                            },
                            new DisplayElement
                            {
                                Language = "vn",
                                Body = o.Vn
                            }
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

        private static void DeleteTemporaryFiles()
        {
            Files.DeleteTemporaryFiles(
                Directories.Temp(), Parameters.General.DeleteTempOldThan);
            Files.DeleteTemporaryFiles(
                Directories.Histories(), Parameters.General.DeleteHistoriesOldThan);
        }
    }
}
