using CsvHelper;
using Implem.CodeDefiner.Functions.Rds.Parts;
using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Implem.CodeDefiner.Functions.Rds
{
    internal class Configurator
    {
        internal static bool Configure(ISqlObjectFactory factory, bool force, bool noInput)
        {
            if (Environments.RdsProvider == "Local")
            {
                UsersConfigurator.KillTask(factory: factory);
                RdsConfigurator.Configure(factory: factory);
                UsersConfigurator.Configure(factory: factory);
                SchemaConfigurator.Configure(factory: factory);
            }
            if (CheckColumnsShrinkage(
                factory: factory,
                force: force,
                noInput: noInput))
            {
                TablesConfigurator.Configure(factory: factory);
                if (Environments.RdsProvider == "Local")
                {
                    PrivilegeConfigurator.Configure(factory: factory);
                }
                return true;
            }
            return false;
        }

        private static bool CheckColumnsShrinkage(
            ISqlObjectFactory factory,
            bool force = false,
            bool noInput = false)
        {
            OutputLicenseInfo();
            var defIssuesColumns = ExtractColumnsFromColumnDefinitionCollection("Issues");
            var defResultsColumns = ExtractColumnsFromColumnDefinitionCollection("Results");
            var currentIssuesColumns = GetCurrentColumns(
                factory: factory,
                tableName: "Issues");
            var currentResultsColumns = GetCurrentColumns(
                factory: factory,
                tableName: "Results");
            var issuesShrinked = HasColumnsChanges(
                defColumns: defIssuesColumns,
                currentColumns: currentIssuesColumns,
                tableName: "Issues");
            var resultsShrinked = HasColumnsChanges(
                defColumns: defResultsColumns,
                currentColumns: currentResultsColumns,
                tableName: "Results");
            if ((issuesShrinked || resultsShrinked) && !force)
            {
                Consoles.Write(
                    text: DisplayAccessor.Displays.Get("CodeDefinerErrorColumnsShrinked"),
                    type: Consoles.Types.Error);
                return false;
            }
            if (noInput)
            {
                Consoles.Write(
                    text: DisplayAccessor.Displays.Get("CodeDefinerSkipUserInput"),
                    type: Consoles.Types.Info);
                return true;
            }
            Console.WriteLine(DisplayAccessor.Displays.Get("CodeDefinerInputYesOrNo"));
            var inputKey = Console.ReadLine().ToLower();
            if (inputKey != "y" && inputKey != "yes")
            {
                Consoles.Write(
                    text: DisplayAccessor.Displays.Get("CodeDefinerRdsCanceled"),
                    type: Consoles.Types.Error);
                return false;
            }
            return true;
        }

        private static List<string> ExtractColumnsFromColumnDefinitionCollection(string tableName)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == tableName)
                .Select(o => o["ColumnName"].ToString())
                .ToList();
        }

        private static List<string> GetCurrentColumns(ISqlObjectFactory factory, string tableName)
        {
            return Columns
                .Get(factory: factory, sourceTableName: tableName)
                .Select(o => o["ColumnName"].ToString())
                .ToList();
        }

        private static void OutputLicenseInfo()
        {
            string serverName = string.Empty;
            string database = string.Empty;
            switch (Parameters.Rds.Dbms)
            {
                case "SQLServer":
                    serverName = new SqlConnectionStringBuilder(Parameters.Rds.SaConnectionString).DataSource;
                    database = new SqlConnectionStringBuilder(Parameters.Rds.SaConnectionString).InitialCatalog;
                    break;
                case "PostgreSQL":
                    serverName = new NpgsqlConnectionStringBuilder(Parameters.Rds.SaConnectionString).Host;
                    database = new NpgsqlConnectionStringBuilder(Parameters.Rds.SaConnectionString).Database;
                    break;
                default:
                    serverName = new SqlConnectionStringBuilder(Parameters.Rds.SaConnectionString).DataSource;
                    database = new SqlConnectionStringBuilder(Parameters.Rds.SaConnectionString).InitialCatalog;
                    break;
            }
            Consoles.Write(
                string.Format(
                    DisplayAccessor.Displays.Get("CodeDefinerLicenseInfo"),
                    serverName,
                    database,
                    Parameters.LicenseDeadline().ToString("d"),
                    Parameters.Licensee() ?? String.Empty,
                    Parameters.LicensedUsers()),
                Consoles.Types.Info);
            if (Parameters.LicenseDeadline() == DateTime.MinValue
                && Parameters.Licensee().IsNullOrEmpty()
                && Parameters.LicensedUsers() == 0)
            {
                Consoles.Write(
                    DisplayAccessor.Displays.Get("CodeDefinerCommunityEdition"),
                    Consoles.Types.Info);
                return;
            }
            if (Parameters.LicenseDeadline() < DateTime.Now)
            {
                Consoles.Write(
                    DisplayAccessor.Displays.Get("CodeDefinerIssueNewLicense"),
                    Consoles.Types.Info);
                return;
            }
            if (Parameters.CommercialLicense())
            {
                Consoles.Write(
                    DisplayAccessor.Displays.Get("CodeDefinerEnterpriseEdition"),
                    Consoles.Types.Info);
                return;
            }
        }

        private static bool HasColumnsChanges(
            List<string> defColumns,
            List<string> currentColumns,
            string tableName)
        {
            var reducedColumns = currentColumns
                .Where(column => !defColumns.Contains(column))
                .ToList();
            if (reducedColumns.Any())
            {
                Consoles.Write(
                    string.Format(
                        DisplayAccessor.Displays.Get("CodeDefinerReducedColumnList"),
                        tableName),
                    Consoles.Types.Info);
                reducedColumns.ForEach(column => { Console.WriteLine(column); });
                return true;
            }
            return false;
        }
    }
}