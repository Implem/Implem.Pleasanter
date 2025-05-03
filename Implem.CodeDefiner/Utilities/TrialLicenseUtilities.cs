using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using System;
using System.Linq;
using System.Text;

namespace Implem.CodeDefiner.Utilities
{
    internal class TrialLicenseUtilities
    {
        public const string LICENSE_NAME = "";
        public const int LICENSE_DAYS = 90;
        public const int LICENSE_USERS = 0;

        public static Status GetStatus()
        {
            var licenseType = Parameters.GetLicenseType();
            if (licenseType == 0x04) return Status.CommercialLicenseActive;
            if (licenseType == 0x05) return Status.CommercialLicenseExpired;
            if (licenseType == 0x08) return Status.TrialLicenseActive;
            if (licenseType == 0x09) return Status.TrialLicenseExpired;
            return Status.CommunityLicenseActive;
        }

        public enum Status : int
        {
            CommunityLicenseActive,
            CommercialLicenseActive,
            CommercialLicenseExpired,
            TrialLicenseActive,
            TrialLicenseExpired
        }

        internal static void Initialize(
            ISqlObjectFactory factory)
        {
            if ((Parameters.GetLicenseType() & 0x04) != 0) return;
            var str = ReadDbRecode(factory: factory, page: "Info");
            Parameters.TrialLicense = Deserialize(value: str);
        }

        internal static bool Registration(
            ISqlObjectFactory factory,
            bool useExColumnsFile)
        {
            const string page = "Info";
            ClearDBRecode(
                factory: factory,
                page: page);
            var dbObj = new TrialLicense()
            {
                CreatedTime = DateTime.Now,
                Deadline = DateTime.Now.AddDays(LICENSE_DAYS).Date,
                Users = LICENSE_USERS,
                Licensee = LICENSE_NAME,
            };
            dbObj.CheckSum = CalcCheckSum(obj: dbObj);
            return InsertDBRecode(
                factory: factory,
                page: page,
                value: dbObj.ToJson()) == 1;
        }

        internal static void SetTrialLicenseExtendedColumns(
            ISqlObjectFactory factory,
            bool useExColumnsFile)
        {
            if (!useExColumnsFile)
            {
                Parameters.ExtendedColumnsSet = GetDefaultExtendedColumns().ToList();
            }
            Initializer.SetDefinitionsTrial();
        }

        private static TrialLicense Deserialize(
            string value)
        {
            if (value.IsNullOrEmpty()) return null;
            var o = value.Deserialize<TrialLicense>();
            if (o == null) return null;
            if (o.Licensee != LICENSE_NAME) return null;
            if (o.CheckSum != CalcCheckSum(o)) return null;
            return o;
        }

        private static string CalcCheckSum(
            TrialLicense obj)
        {
            var copyObj = obj.ToJson().Deserialize<TrialLicense>();
            copyObj.CheckSum = null;
            var str = copyObj.ToJson() + "Implem.Pleasanter.TrialLicense";
            using var sha = System.Security.Cryptography.SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        private static int ClearDBRecode(
            ISqlObjectFactory factory,
            string page)
        {
            try
            {
                using var sqlIo = Def.SqlIoByAdmin(factory: factory);
                var commandText = "delete from \"Sessions\" where \"SessionGuid\" = '@TrialLicense' and \"Key\" = 'KeyValue' and \"Page\" = @Page;";
                sqlIo.SqlContainer.SqlStatementCollection.Add(new SqlStatement(commandText));
                sqlIo.SqlCommand.Parameters_AddWithValue("Page", page);
                return sqlIo.ExecuteNonQuery(
                    factory: factory,
                    dbTransaction: null,
                    dbConnection: null);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static int InsertDBRecode(
            ISqlObjectFactory factory,
            string page,
            string value)
        {
            using var sqlIo = Def.SqlIoByAdmin(factory: factory);
            var commandText = "insert into \"Sessions\" (\"SessionGuid\", \"Key\", \"Page\", \"Value\", \"Creator\", \"Updator\") values ('@TrialLicense', 'KeyValue', @Page, @Value, 0, 0);";
            sqlIo.SqlContainer.SqlStatementCollection.Add(new SqlStatement(commandText));
            sqlIo.SqlCommand.Parameters_AddWithValue("Page", page);
            sqlIo.SqlCommand.Parameters_AddWithValue("Value", value);
            return sqlIo.ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null);
        }

        private static string ReadDbRecode(
            ISqlObjectFactory factory,
            string page)
        {
            try
            {
                using var sqlIo = Def.SqlIoByUser(factory: factory);
                var t = sqlIo.ExecuteTable(
                    factory: factory,
                    commandText: $"select \"Value\" from \"Sessions\" where \"SessionGuid\" = '@TrialLicense' and \"Key\" = 'KeyValue' and \"Page\" = '{page}';");
                return t.Rows.Count == 1
                    ? t.Rows[0]["Value"].ToString()
                    : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static ExtendedColumns[] GetDefaultExtendedColumns()
        {
            var columns = new ExtendedColumns[]
            {
                new ExtendedColumns(){
                    TableName="Results",
                    ReferenceType="Results",
                    Class= 100,
                    Num= 0,
                    Date= 0,
                    Description= 0,
                    Check= 0,
                    Attachments= 0
                },
                new ExtendedColumns(){
                    TableName="Issues",
                    ReferenceType="Issues",
                    Class= 100,
                    Num= 0,
                    Date= 0,
                    Description= 0,
                    Check= 0,
                    Attachments= 0
                }
            };
            return columns;
        }
    }
}