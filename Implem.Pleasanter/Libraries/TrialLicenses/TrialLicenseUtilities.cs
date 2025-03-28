using Implem.DefinitionAccessor;
using Implem.Factory;
using Implem.IRds;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Implem.Pleasanter.Libraries.TrialLicenses
{
    public class TrialLicenseUtilities
    {
        public const string LICENSE_NAME = "";
        public const int LICENSE_DAYS = 90;
        public const int LICENSE_USERS = 0;

        public static void Initialize()
        {
            if ((Parameters.GetLicenseType() & 0x04) != 0) return;
            var factory = RdsFactory.Create(Parameters.Rds.Dbms);
            var str = ReadDbRecode(
                factory: factory,
                page: "Info");
            var infoObj = Deserialize(value: str);
            if (infoObj == null) return;
            var trialLicense = new TrialLicense()
            {
                Users = infoObj.Users,
                Deadline = infoObj.Deadline < DateTime.Now.Date.AddDays(LICENSE_DAYS + 1)
                    ? infoObj.Deadline
                    : DateTime.Now.Date.AddDays(-1),
                Licensee = infoObj.Licensee
            };
            Parameters.TrialLicense = trialLicense;
            if (!trialLicense.Check()) return;
            Parameters.ExtendedColumnsSet = GetExtendedColumns(factory: factory);
            Initializer.SetDefinitionsTrial();
        }

        private static List<ExtendedColumns> GetExtendedColumns(
            ISqlObjectFactory factory)
        {
            var tables = new string[] { "Depts", "Groups", "Users", "Issues", "Results" };
            var columns = new string[] { "Class", "Num", "Date", "Description", "Check", "Attachments" };
            var regExColumn = new System.Text.RegularExpressions.Regex($"^({columns.Join("|")})([0-9]+)$");
            var list = new List<ExtendedColumns>();
            foreach (var tableName in tables)
            {
                var exColumns = new ExtendedColumns()
                {
                    TableName = tableName,
                    ReferenceType = tableName
                };
                var isSet = false;
                foreach (var column in GetCurrentColumns(
                    factory: factory,
                    tableName: tableName))
                {
                    var match = regExColumn.Match(column);
                    if (!match.Success) continue;
                    isSet = true;
                    var c = match.Groups[1].Value;
                    var n = int.Parse(match.Groups[2].Value);
                    if (c == "Class") { if (exColumns.Class < n) exColumns.Class = n; }
                    else if (c == "Num") { if (exColumns.Num < n) exColumns.Num = n; }
                    else if (c == "Date") { if (exColumns.Date < n) exColumns.Date = n; }
                    else if (c == "Description") { if (exColumns.Description < n) exColumns.Description = n; }
                    else if (c == "Check") { if (exColumns.Check < n) exColumns.Check = n; }
                    else if (c == "Attachments") { if (exColumns.Attachments < n) exColumns.Attachments = n; }
                }
                if (isSet) list.Add(exColumns);
            }
            return list;
        }

        private static IEnumerable<string> GetCurrentColumns(
            ISqlObjectFactory factory,
            string tableName)
        {
            using var sqlIo = Def.SqlIoByUser(factory: factory);
            return sqlIo.ExecuteTable(
                factory: factory,
                commandText: $"select * from \"{tableName}\" where (1=0);")
                    .Columns
                    .Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .ToArray();
        }

        private static TrialLicense Deserialize(
            string value)
        {
            if (value.IsNullOrEmpty()) return null;
            var obj = value.Deserialize<TrialLicense>();
            if (obj == null) return null;
            if (obj.Licensee != LICENSE_NAME) return null;
            if (obj.CheckSum != CalcCheckSum(obj: obj)) return null;
            return obj;
        }

        private static string CalcCheckSum(
            TrialLicense obj)
        {
            var copyObj = obj.ToJson().Deserialize<TrialLicense>();
            copyObj.CheckSum = null;
            var str = copyObj.ToJson() + "Implem.Pleasanter.TrialLicense";
            using var sha = System.Security.Cryptography.SHA256.Create();
            return System.Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        private static string ReadDbRecode(
            ISqlObjectFactory factory,
            string page)
        {
            try
            {
                // CodeDefinerと同じ書き方をしたいために、SQL文でコーディングしている
                using var sqlIo = Def.SqlIoByUser(factory: factory);
                var table = sqlIo.ExecuteTable(
                    factory: factory,
                    commandText: $"select \"Value\" from \"Sessions\" where \"SessionGuid\" = '@TrialLicense' and \"Key\" = 'KeyValue' and \"Page\" = '{page}';");
                return table.Rows.Count == 1
                    ? table.Rows[0]["Value"].ToString()
                    : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
