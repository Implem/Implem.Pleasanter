using Implem.DisplayAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Exceptions;
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
        private static string ParametersPath { get; set; }

        public static List<Exception> Initialize(
            string path,
            string assemblyVersion,
            bool codeDefiner = false,
            bool pleasanterTest = false,
            bool setSaPassword = false,
            bool setRandomPassword = false)
        {
            var exceptions = new List<Exception>();
            Environments.CodeDefiner = codeDefiner;
            Environments.PleasanterTest = pleasanterTest;
            Environments.CurrentDirectoryPath = path != null
                ? path
                : GetSourcePath();
            ParametersPath = Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters");
            SetRdsPassword(setSaPassword, setRandomPassword);
            SetParameters();
            Environments.ServiceName = Parameters.Service.Name;
            SetRdsParameters();
            Environments.MachineName = $"{Environment.MachineName}:{Environment.OSVersion}";
            Environments.Application =
                Assembly.GetExecutingAssembly().ManifestModule.Name.FileNameOnly();
            Environments.AssemblyVersion = assemblyVersion;
            SetDefinitions();
            SetTimeZone();
            SetLanguage();
            SetSqls();
            DateTimes.FirstDayOfWeek = Parameters.General.FirstDayOfWeek;
            DateTimes.FirstMonth = Parameters.General.FirstMonth;
            DateTimes.MinTime = Parameters.General.MinTime;
            DateTimes.MaxTime = Parameters.General.MaxTime;
            SetBundleVersions();
            DeleteTemporaryFiles(exceptions: exceptions);
            return exceptions;
        }

        private static void SetRdsPassword(bool setRdsPassword, bool setRandomPassword)
        {
            if (setRdsPassword)
            {
                Console.WriteLine("Please enter the SA password.");
                var rdsParameters = Files.Read(JsonFilePath("Rds"));
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
                rdsParameters.Write(JsonFilePath("Rds"));
            }
        }

        public static void SetParameters()
        {
            Parameters.Env = Read<Env>(required: false);
            if (Parameters.Env?.ParametersPath.IsNullOrEmpty() == false)
            {
                ParametersPath = Parameters.Env?.ParametersPath;
            }
            Parameters.Api = Read<Api>();
            Parameters.Authentication = Read<Authentication>();
            Parameters.BackgroundService = Read<BackgroundService>();
            Parameters.BackgroundTask = Read<BackgroundTask>();
            Parameters.BinaryStorage = Read<BinaryStorage>();
            Parameters.CustomDefinitions = CustomDefinitionsHash();
            Parameters.Deleted = Read<Deleted>();
            Parameters.ExtendedAutoTestSettings = Read<AutoTestSettings>(required: false);
            Parameters.ExtendedAutoTestScenarios = ExtendedAutoTestScenarios();
            Parameters.ExtendedAutoTestOperations = ExtendedAutoTestOperations();
            Parameters.ExtendedColumnDefinitions = ExtendedColumnDefinitions();
            Parameters.ExtendedColumnsSet = ExtendedColumnsSet();
            Parameters.ExtendedFields = ExtendedFields();
            Parameters.ExtendedHtmls = ExtendedHtmls();
            Parameters.ExtendedNavigationMenus = ExtendedNavigationMenus();
            Parameters.ExtendedScripts = ExtendedScripts();
            Parameters.ExtendedServerScripts = ExtendedServerScripts();
            Parameters.ExtendedSqls = ExtendedSqls();
            Parameters.ExtendedStyles = ExtendedStyles();
            Parameters.ExtendedPlugins = ExtendedPlugins();
            Parameters.ExtendedTags = ExtendedTags();
            Parameters.General = Read<General>();
            Parameters.GroupMembers = Read<GroupMembers>();
            Parameters.History = Read<History>();
            Parameters.Version = Read<ParameterAccessor.Parts.Version>();
            Parameters.Mail = Read<Mail>();
            Parameters.Mobile = Read<Mobile>();
            Parameters.NavigationMenus = NavigationMenus();
            Parameters.Migration = Read<Migration>();
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
            Parameters.Locations = Read<Locations>();
            Parameters.Validation = Read<Validation>();
            Parameters.Dashboard = Read<Dashboard>();
            Parameters.Rds.SaConnectionString = Strings.CoalesceEmpty(
                Parameters.Rds.SaConnectionString,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Rds_SaConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Rds_ConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_{Parameters.Rds.Dbms}_SaConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_{Parameters.Rds.Dbms}_ConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_SaConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_ConnectionString"));
            Parameters.Rds.OwnerConnectionString = Strings.CoalesceEmpty(
                Parameters.Rds.OwnerConnectionString,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_OwnerConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Rds_ConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_{Parameters.Rds.Dbms}_OwnerConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_{Parameters.Rds.Dbms}_ConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_OwnerConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_ConnectionString"));
            Parameters.Rds.UserConnectionString = Strings.CoalesceEmpty(
                Parameters.Rds.UserConnectionString,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_UserConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Rds_ConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_{Parameters.Rds.Dbms}_UserConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_{Parameters.Rds.Dbms}_ConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_UserConnectionString"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Rds_ConnectionString"));
            Parameters.Mail.SmtpUserName = Strings.CoalesceEmpty(
                Parameters.Mail.SmtpUserName,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Mail_SmtpUserName"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Mail_SmtpUserName"));
            Parameters.Mail.SmtpPassword = Strings.CoalesceEmpty(
                Parameters.Mail.SmtpPassword,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Mail_SmtpPassword"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Mail_SmtpPassword"));
            Parameters.General.HtmlUrlPrefix = Strings.CoalesceEmpty(
                Parameters.General.HtmlUrlPrefix,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_HtmlUrlPrefix"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_HtmlUrlPrefix"),
                Parameters.CommercialLicense()
                    ? "ee"
                    : "com");
            if(Parameters.Security.AspNetCoreDataProtection == null)
            {
                Parameters.Security.AspNetCoreDataProtection = new AspNetCoreDataProtection();
            }
            Parameters.Security.AspNetCoreDataProtection.BlobContainerUri = Strings.CoalesceEmpty(
                Parameters.Security.AspNetCoreDataProtection.BlobContainerUri,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Security_AspNetCoreDataProtection_BlobContainerUri"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Security_AspNetCoreDataProtection_BlobContainerUri"));
            Parameters.Security.AspNetCoreDataProtection.KeyIdentifier = Strings.CoalesceEmpty(
                Parameters.Security.AspNetCoreDataProtection.KeyIdentifier,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Security_AspNetCoreDataProtection_KeyIdentifier"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Security_AspNetCoreDataProtection_KeyIdentifier"));
            Parameters.Security.AspNetCoreDataProtection.XmlAesKey = Strings.CoalesceEmpty(
                Parameters.Security.AspNetCoreDataProtection.XmlAesKey,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Security_AspNetCoreDataProtection_XmlAesKey"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Security_AspNetCoreDataProtection_XmlAesKey"),
                Parameters.Service.Name);
            Parameters.Service.DeploymentEnvironment = Strings.CoalesceEmpty(
                Parameters.Service.DeploymentEnvironment,
                Environment.GetEnvironmentVariable($"{Parameters.Service.EnvironmentName}_Service_DeploymentEnvironment"),
                Environment.GetEnvironmentVariable($"{Parameters.Service.Name}_Service_DeploymentEnvironment"));
        }

        public static void ReloadParameters()
        {
            SetParameters();
            SetRdsParameters();
        }

        private static T Read<T>(bool required = true)
        {
            var name = typeof(T).Name;
            var json = Files.Read(JsonFilePath(name));
            var data = json.Deserialize<T>();
            if (required)
            {
                if (json.IsNullOrEmpty())
                {
                    throw new ParametersNotFoundException(name + ".json");
                }
                if (!json.IsNullOrEmpty() && data == null)
                {
                    throw new ParametersIllegalSyntaxException(name + ".json");
                }
            }
            return data;
        }

        private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> CustomDefinitionsHash(
            string path = null,
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> hash = null)
        {
            hash = hash ?? new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            path = path ?? Path.Combine(
                ParametersPath,
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
                        throw new ParametersIllegalSyntaxException(file.FullName);
                    }
                }
                foreach (var sub in dir.GetDirectories())
                {
                    hash = CustomDefinitionsHash(sub.FullName, hash);
                }
            }
            return hash;
        }

        private static List<AutoTestScenario> ExtendedAutoTestScenarios()
        {
            var hash = new List<AutoTestScenario>();
            var path = Path.Combine(
                ParametersPath,
                "ExtendedAutoTest",
                "TestCases");
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var subDir in dir
                    .GetFiles("*.json", SearchOption.AllDirectories)
                    .Select((item, index) => new { item, index }))
                {
                    hash.Add(Files.Read(subDir.item.FullName).Deserialize<AutoTestScenario>());
                    hash[subDir.index].CaseName
                        = $"{Path.GetFileName(Path.GetDirectoryName(subDir.item.FullName))}/" +
                        $"{Path.GetFileName(subDir.item.FullName.Replace(".json", ""))}";
                    var testCasesPath = Path.GetDirectoryName(subDir.item.FullName)
                        .Substring(Path.GetDirectoryName(subDir.item.FullName)
                            .IndexOf("TestCases"))
                            .Replace("TestCases\\", "");
                    if (testCasesPath.Equals("TestCases"))
                    {
                        hash[subDir.index].CaseName
                            = $"\\{Path.GetFileName(subDir.item.FullName.Replace(".json", ""))}";
                    }
                    else
                    {
                        hash[subDir.index].CaseName
                            = $"\\{testCasesPath}" +
                            $"\\{Path.GetFileName(subDir.item.FullName.Replace(".json", ""))}";
                    }
                }
            }
            return hash;
        }

        private static List<AutoTestOperation> ExtendedAutoTestOperations()
        {
            var hash = new List<AutoTestOperation>();
            var path = Path.Combine(
                ParametersPath,
                "ExtendedAutoTest",
                "TestParts");
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var subDir in dir
                    .GetFiles("*.json", SearchOption.AllDirectories)
                    .Select((item, index) => new { item, index }))
                {
                    hash.Add(Files.Read(subDir.item.FullName).Deserialize<AutoTestOperation>());
                    var testPartsPath = Path.GetDirectoryName(subDir.item.FullName)
                        .Substring(Path.GetDirectoryName(subDir.item.FullName)
                            .IndexOf("TestParts"))
                            .Replace("TestParts\\", "");
                    if (testPartsPath.Equals("TestParts"))
                    {
                        hash[subDir.index].TestPartsPath
                            = $"{Path.DirectorySeparatorChar}{Path.GetFileName(subDir.item.FullName)}";
                    }
                    else
                    {
                        hash[subDir.index].TestPartsPath
                            = $"{Path.DirectorySeparatorChar}{testPartsPath}" +
                            $"{Path.DirectorySeparatorChar}{Path.GetFileName(subDir.item.FullName)}";
                    }
                }
            }
            return hash;
        }

        private static Dictionary<string, string> ExtendedColumnDefinitions()
        {
            var hash = new Dictionary<string, string>();
            var path = Path.Combine(
                ParametersPath,
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
                ParametersPath,
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
                        throw new ParametersIllegalSyntaxException(file.FullName);
                    }
                }
                foreach (var sub in dir.GetDirectories())
                {
                    list = ExtendedColumnsSet(sub.FullName, list);
                }
            }
            return list;
        }

        private static List<ExtendedField> ExtendedFields(
            string path = null, List<ExtendedField> list = null)
        {
            list = list ?? new List<ExtendedField>();
            path = path ?? Path.Combine(
                ParametersPath,
                "ExtendedFields");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.json"))
            {
                var extendedField = Files.Read(file.FullName)
                    .Deserialize<ExtendedField>();
                if (extendedField != null)
                {
                    extendedField.Path = file.FullName;
                    list.Add(extendedField);
                }
                else
                {
                    throw new ParametersIllegalSyntaxException(file.FullName);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedFields(dir.FullName, list);
            }
            return list;
        }

        private static List<ExtendedHtml> ExtendedHtmls(
            string path = null,
            List<ExtendedHtml> list = null)
        {
            list = list ?? new List<ExtendedHtml>();
            path = path ?? Path.Combine(
                ParametersPath,
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

        private static List<ExtendedNavigationMenu> ExtendedNavigationMenus(
            string path = null,
            List<ExtendedNavigationMenu> list = null)
        {
            list = list ?? new List<ExtendedNavigationMenu>();
            path = path ?? Path.Combine(
                ParametersPath,
                "ExtendedNavigationMenus");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.json"))
            {
                var extendedNavigationMenu = Files.Read(file.FullName)
                    .Deserialize<ExtendedNavigationMenu>();
                if (extendedNavigationMenu != null)
                {
                    extendedNavigationMenu.Path = file.FullName;
                    list.Add(extendedNavigationMenu);
                }
                else
                {
                    throw new ParametersIllegalSyntaxException(file.FullName);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedNavigationMenus(dir.FullName, list);
            }
            return list;
        }

        private static List<ExtendedScript> ExtendedScripts(
            string path = null, List<ExtendedScript> list = null)
        {
            list = list ?? new List<ExtendedScript>();
            path = path ?? Path.Combine(
                ParametersPath,
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
                ParametersPath,
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
                    throw new ParametersIllegalSyntaxException(file.FullName);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedServerScripts(dir.FullName, list);
            }
            return list;
        }

        private static List<ExtendedSql> ExtendedSqls(
            string path = null, List<ExtendedSql> list = null)
        {
            list = list ?? new List<ExtendedSql>();
            path = path ?? Path.Combine(
                ParametersPath,
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
                    throw new ParametersIllegalSyntaxException(file.FullName);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedSqls(dir.FullName, list);
            }
            return list;
        }

        private static List<ExtendedPlugin> ExtendedPlugins(
            string path = null, List<ExtendedPlugin> list = null)
        {
            list = list ?? new List<ExtendedPlugin>();
            path = path ?? Path.Combine(
                ParametersPath,
                "ExtendedPlugins");
            foreach (var file in new DirectoryInfo(path).GetFiles("*.json"))
            {
                var extendedLib = Files.Read(file.FullName)
                    .Deserialize<ExtendedPlugin>();
                if (extendedLib != null)
                {
                    extendedLib.Path = file.FullName;
                    list.Add(extendedLib);
                }
                else
                {
                    throw new ParametersIllegalSyntaxException(file.FullName);
                }
            }
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                list = ExtendedPlugins(dir.FullName, list);
            }
            return list;
        }

        private static List<ExtendedStyle> ExtendedStyles(
            string path = null, List<ExtendedStyle> list = null)
        {
            list = list ?? new List<ExtendedStyle>();
            path = path ?? Path.Combine(
                ParametersPath,
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

        private static Dictionary<string, string> ExtendedTags()
        {
            var hash = new Dictionary<string, string>();
            var path = Path.Combine(
                ParametersPath,
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

        private static List<NavigationMenu> NavigationMenus()
        {
            var name = "NavigationMenus";
            var filePath = JsonFilePath(name);
            var json = Files.Read(filePath);
            var data = json.Deserialize<List<NavigationMenu>>();
            if (json == null)
            {
                throw new ParametersNotFoundException(name + ".json");
            }
            if (data == null)
            {
                throw new ParametersIllegalSyntaxException(name + ".json");
            }
            return data;
        }

        private static string JsonFilePath(string name)
        {
            return Path.Combine(
                ParametersPath,
                name + ".json");
        }

        private static string GetSourcePath()
        {
            var parts = new DirectoryInfo(
                Assembly.GetEntryAssembly().Location).FullName.Split(Path.DirectorySeparatorChar);
            return new DirectoryInfo(Path.Combine(
                parts
                    .TakeWhile(part =>
                        !part.StartsWith("Implem.CodeDefiner")
                        && !part.StartsWith("Implem.PleasanterTest"))
                    .Join(Path.DirectorySeparatorChar.ToString()),
                "Implem.Pleasanter"))
                    .FullName;
        }

        public static void SetDefinitions()
        {
            Displays.DisplayHash = DisplayHash();
            Def.SetCodeDefinition();
            Def.SetColumnDefinition();
            Def.SetTemplateDefinition();
            Def.SetViewModeDefinition();
            Def.SetDemoDefinition();
            Def.SetSqlDefinition();
            SetDisplayAccessor();
            SetColumnDefinitionAccessControl();
        }

        public static XlsIo DefinitionFile(string name)
        {
            var path = name == "Demo" && Environments.PleasanterTest
                ? Path.Combine(Directories.PleasanterTest(), "Data")
                : Path.Combine(Directories.Definitions(), $"Definition_{name}");
            var xlsIo = new XlsIo(
                path: path,
                name: name);
            if (name == "Column")
            {
                SetColumnDefinitionAdditional(xlsIo);
            }
            return xlsIo;
        }

        public static ISqlDefinitionFiles DefinitionSqls(string dbms)
        {
            var def = new SqlDefinitionFiles() { FullPath = Directories.Sqls(dbms) };
            def.Read();
            return def;
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
            Environments.DeadlockRetryCount = Parameters.Rds.DeadlockRetryCount;
            Environments.DeadlockRetryInterval = Parameters.Rds.DeadlockRetryInterval;
        }

        private static void SetColumnDefinitionAdditional(XlsIo definitionFile)
        {
            var tableCopy = definitionFile.XlsSheet.ToList();
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

        private static void SetColumnDefinitionAccessControl()
        {
            if (!Parameters.User.DisableTopSiteCreation)
            {
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowCreationAtTopSite").CreateAccessControl = "ManageService";
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowCreationAtTopSite").ReadAccessControl = "ManageService";
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowCreationAtTopSite").UpdateAccessControl = "ManageService";
            }
            if (!Parameters.User.DisableGroupAdmin)
            {
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowGroupAdministration").CreateAccessControl = "ManageService";
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowGroupAdministration").ReadAccessControl = "ManageService";
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowGroupAdministration").UpdateAccessControl = "ManageService";
            }
            if (!Parameters.User.DisableGroupCreation)
            {
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowGroupCreation").CreateAccessControl = "ManageService";
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowGroupCreation").ReadAccessControl = "ManageService";
                Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                    o.Id == "Users_AllowGroupCreation").UpdateAccessControl = "ManageService";
            }
            switch (Parameters.Security.SecondaryAuthentication?.Mode)
            {
                case SecondaryAuthentication.SecondaryAuthenticationMode.None:
                    SetManageServiceToEnableSecondaryAuthentication();
                    SetManageServiceToDisableSecondaryAuthentication();
                    break;
                case SecondaryAuthentication.SecondaryAuthenticationMode.DefaultEnable:
                    SetManageServiceToEnableSecondaryAuthentication();
                    break;
                case SecondaryAuthentication.SecondaryAuthenticationMode.DefaultDisable:
                    SetManageServiceToDisableSecondaryAuthentication();
                    break;
                default:
                    SetManageServiceToEnableSecondaryAuthentication();
                    SetManageServiceToDisableSecondaryAuthentication();
                    break;
            }
            if((Parameters.Security.SecondaryAuthentication?.Mode
                is null
                or SecondaryAuthentication.SecondaryAuthenticationMode.None)
                    || Parameters.Security.SecondaryAuthentication?.NotificationType
                        != SecondaryAuthentication.SecondaryAuthenticationModeNotificationTypes.Totp)
            {
                SetManageServiceToEnableSecretKey();
            }
        }

        private static void SetManageServiceToDisableSecondaryAuthentication()
        {
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_DisableSecondaryAuthentication").CreateAccessControl = "ManageService";
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_DisableSecondaryAuthentication").ReadAccessControl = "ManageService";
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_DisableSecondaryAuthentication").UpdateAccessControl = "ManageService";
        }

        private static void SetManageServiceToEnableSecondaryAuthentication()
        {
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_EnableSecondaryAuthentication").CreateAccessControl = "ManageService";
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_EnableSecondaryAuthentication").ReadAccessControl = "ManageService";
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_EnableSecondaryAuthentication").UpdateAccessControl = "ManageService";
        }

        private static void SetManageServiceToEnableSecretKey()
        {
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_EnableSecretKey").CreateAccessControl = "ManageService";
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_EnableSecretKey").ReadAccessControl = "ManageService";
            Def.ColumnDefinitionCollection.FirstOrDefault(o =>
                o.Id == "Users_EnableSecretKey").UpdateAccessControl = "ManageService";
        }

        private static void SetTimeZone()
        {
            Environments.TimeZoneInfoDefault = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(o => o.Id == Parameters.Service.TimeZoneDefault)
                    ?? TimeZoneInfo.Local;
            Def.ColumnDefinitionCollection
                .FirstOrDefault(o => o.Id == "Users_TimeZone").Default = Environments.TimeZoneInfoDefault.Id;
        }

        private static void SetLanguage()
        {
            var language = Def.ColumnTable.Users_Language.ChoicesText
                .SplitReturn()
                .Select(o => o.Split_1st())
                .FirstOrDefault(o => o == Parameters.Service.DefaultLanguage) ?? "en";
            Def.ColumnDefinitionCollection
                .FirstOrDefault(o => o.Id == "Users_Language").Default = language;
        }

        private static void SetSqls()
        {
            Sqls.LogsPath = Directories.Logs();
            Sqls.SelectIdentity = Def.Sql.SelectIdentity;
            Sqls.BeginTransaction = Def.Sql.BeginTransaction;
            Sqls.CommitTransaction = Def.Sql.CommitTransaction;
        }

        private static void SetBundleVersions()
        {
            var generalsPath = Path.Combine(
                Directories.Wwwroot(),
                "bundles",
                "generals.js");
            if (Files.Exists(generalsPath))
            {
                Environments.BundlesVersions.Add("generals.js", Files.Read(generalsPath).Sha512Cng());
            }
            var responsivePath = Path.Combine(
                Directories.Wwwroot(),
                "content",
                "responsive.css");
            if (Files.Exists(responsivePath)) 
            {
                Environments.BundlesVersions.Add("responsive.css", Files.Read(responsivePath).Sha512Cng());
            }
            var stylesPath = Path.Combine(
                Directories.Wwwroot(),
                "content",
                "styles.css");
            if (Files.Exists(stylesPath))
            {
                Environments.BundlesVersions.Add("styles.css", Files.Read(stylesPath).Sha512Cng());
            }
        }

        public static void DeleteTemporaryFiles(List<Exception> exceptions = null)
        {
            if (exceptions == null) exceptions = new List<Exception>();
            exceptions.AddRange(Files.DeleteTemporaryFiles(
                Directories.Temp(), Parameters.General.DeleteTempOldThan));
            exceptions.AddRange(Files.DeleteTemporaryFiles(
                Directories.Histories(), Parameters.General.DeleteHistoriesOldThan));
        }
    }
}
