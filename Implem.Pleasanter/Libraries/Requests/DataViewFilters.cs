using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class DataViewFilters
    {
        public static SqlWhereCollection Get(
            SiteSettings siteSettings,
            string tableName,
            FormData formData,
            SqlWhereCollection where)
        {
            return where
                .Generals(siteSettings: siteSettings, tableName: tableName, formData: formData)
                .Columns(formData: formData);
        }

        private static SqlWhereCollection Generals(
            this SqlWhereCollection sqlWhereCollection,
            SiteSettings siteSettings,
            string tableName,
            FormData formData)
        {
            var tableBracket = "[" + tableName + "]";
            formData.Where(o => o.Value.ToBool()).Select(o => o.Key).ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "DataViewFilters_Incomplete":
                        sqlWhereCollection.Add(
                            columnBrackets: new string[] { "[t0].[Status]" },
                            name: "_U",
                            _operator: " <> {0}".Params(Def.Parameters.CompletionCode));
                        break;
                    case "DataViewFilters_Own":
                        sqlWhereCollection.Add(
                            columnBrackets: new string[] { "[t0].[Manager]", "[t0].[Owner]" },
                            name: "_U",
                            value: Sessions.UserId());
                        break;
                    case "DataViewFilters_NearDeadline":
                        sqlWhereCollection.Add(
                            columnBrackets: new string[] { "[t0].[CompletionTime]" },
                            _operator: " between '{0}' and '{1}' ".Params(
                                DateTime.Now.ToLocal().Date
                                    .AddDays(siteSettings.NearDeadlineBeforeDays.ToInt() * (-1)),
                                DateTime.Now.ToLocal().Date
                                    .AddDays(siteSettings.NearDeadlineAfterDays.ToInt() + 1)
                                    .AddMilliseconds(-1)
                                    .ToString("yyyy/M/d H:m:s.fff")));
                        break;
                    case "DataViewFilters_Delay":
                        sqlWhereCollection
                            .Add(
                                columnBrackets: new string[] { "[t0].[Status]" },
                                name: "_U",
                                _operator: " <> {0}".Params(Def.Parameters.CompletionCode))
                            .Add(
                                columnBrackets: new string[] { "[t0].[ProgressRate]" },
                                _operator: "<",
                                raw: Def.Sql.ProgressRateDelay
                                    .Replace("#TableName#", tableName));
                        break;
                    case "DataViewFilters_Overdue":
                        sqlWhereCollection
                            .Add(
                                columnBrackets: new string[] { "[t0].[Status]" },
                                name: "_U",
                                _operator: " <> {0}".Params(Def.Parameters.CompletionCode))
                            .Add(
                                columnBrackets: new string[] { "[t0].[CompletionTime]" },
                                _operator: " < getdate() ");
                        break;
                    default: break;
                }
            });
            return sqlWhereCollection;
        }

        public static FormData SessionFormData(long? siteId = null)
        {
            var key = siteId == null
                 ? "DataView_" + SiteInfo.PageKey()
                 : "DataView_" + siteId;
            if (HttpContext.Current.Session[key] != null)
            {
                return (HttpContext.Current.Session[key] as FormData)
                    .Update(HttpContext.Current.Request.Form)
                    .RemoveEmpty();
            }
            else
            {
                var formData = new FormData(HttpContext.Current.Request.Form);
                HttpContext.Current.Session[key] = formData;
                return formData.RemoveEmpty();
            }
        }

        private static SqlWhereCollection Columns(
            this SqlWhereCollection where,
            FormData formData)
        {
            formData.Keys.ForEach(key =>
            {
                switch (key)
                {
                    case "DataViewFilters_Tenants_TenantId": where.Tenants_TenantId(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_Ver": where.Tenants_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_TenantName": where.Tenants_TenantName(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_Title": where.Tenants_Title(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_Body": where.Tenants_Body(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_Comments": where.Tenants_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_Creator": where.Tenants_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_Updator": where.Tenants_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_CreatedTime": where.Tenants_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Tenants_UpdatedTime": where.Tenants_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_CreatedTime": where.SysLogs_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_SysLogId": where.SysLogs_SysLogId(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Ver": where.SysLogs_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_SysLogType": where.SysLogs_SysLogType(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_MachineName": where.SysLogs_MachineName(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_ServiceName": where.SysLogs_ServiceName(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_TenantName": where.SysLogs_TenantName(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Application": where.SysLogs_Application(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Class": where.SysLogs_Class(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Method": where.SysLogs_Method(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_RequestData": where.SysLogs_RequestData(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_HttpMethod": where.SysLogs_HttpMethod(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_RequestSize": where.SysLogs_RequestSize(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_ResponseSize": where.SysLogs_ResponseSize(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Elapsed": where.SysLogs_Elapsed(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_ApplicationAge": where.SysLogs_ApplicationAge(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_ApplicationRequestInterval": where.SysLogs_ApplicationRequestInterval(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_SessionAge": where.SysLogs_SessionAge(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_SessionRequestInterval": where.SysLogs_SessionRequestInterval(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_WorkingSet64": where.SysLogs_WorkingSet64(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_VirtualMemorySize64": where.SysLogs_VirtualMemorySize64(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_ProcessId": where.SysLogs_ProcessId(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_ProcessName": where.SysLogs_ProcessName(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_BasePriority": where.SysLogs_BasePriority(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Url": where.SysLogs_Url(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_UrlReferer": where.SysLogs_UrlReferer(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_UserHostName": where.SysLogs_UserHostName(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_UserHostAddress": where.SysLogs_UserHostAddress(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_UserLanguage": where.SysLogs_UserLanguage(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_UserAgent": where.SysLogs_UserAgent(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_SessionGuid": where.SysLogs_SessionGuid(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_ErrMessage": where.SysLogs_ErrMessage(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_ErrStackTrace": where.SysLogs_ErrStackTrace(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_AssemblyVersion": where.SysLogs_AssemblyVersion(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Comments": where.SysLogs_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Creator": where.SysLogs_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_Updator": where.SysLogs_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_UpdatedTime": where.SysLogs_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_OnAzure": if (formData[key].ToBool()) where.SysLogs_OnAzure(formData[key], "t0"); break;
                    case "DataViewFilters_SysLogs_InDebug": if (formData[key].ToBool()) where.SysLogs_InDebug(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_TenantId": where.Depts_TenantId(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_DeptId": where.Depts_DeptId(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_Ver": where.Depts_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_ParentDeptId": where.Depts_ParentDeptId(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_ParentDept": where.Depts_ParentDept(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_DeptCode": where.Depts_DeptCode(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_Dept": where.Depts_Dept(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_DeptName": where.Depts_DeptName(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_Body": where.Depts_Body(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_Comments": where.Depts_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_Creator": where.Depts_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_Updator": where.Depts_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_CreatedTime": where.Depts_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Depts_UpdatedTime": where.Depts_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Users_TenantId": where.Users_TenantId(formData[key], "t0"); break;
                    case "DataViewFilters_Users_UserId": where.Users_UserId(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Ver": where.Users_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Users_LoginId": where.Users_LoginId(formData[key], "t0"); break;
                    case "DataViewFilters_Users_UserCode": where.Users_UserCode(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Password": where.Users_Password(formData[key], "t0"); break;
                    case "DataViewFilters_Users_LastName": where.Users_LastName(formData[key], "t0"); break;
                    case "DataViewFilters_Users_FirstName": where.Users_FirstName(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Birthday": where.Users_Birthday(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Sex": where.Users_Sex(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Language": where.Users_Language(formData[key], "t0"); break;
                    case "DataViewFilters_Users_TimeZone": where.Users_TimeZone(formData[key], "t0"); break;
                    case "DataViewFilters_Users_DeptId": where.Users_DeptId(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Dept": where.Users_Dept(formData[key], "t0"); break;
                    case "DataViewFilters_Users_FirstAndLastNameOrder": where.Users_FirstAndLastNameOrder(formData[key], "t0"); break;
                    case "DataViewFilters_Users_LastLoginTime": where.Users_LastLoginTime(formData[key], "t0"); break;
                    case "DataViewFilters_Users_PasswordChangeTime": where.Users_PasswordChangeTime(formData[key], "t0"); break;
                    case "DataViewFilters_Users_NumberOfLogins": where.Users_NumberOfLogins(formData[key], "t0"); break;
                    case "DataViewFilters_Users_NumberOfDenial": where.Users_NumberOfDenial(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Comments": where.Users_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Creator": where.Users_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Updator": where.Users_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Users_CreatedTime": where.Users_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Users_UpdatedTime": where.Users_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Disabled": if (formData[key].ToBool()) where.Users_Disabled(formData[key], "t0"); break;
                    case "DataViewFilters_Users_TenantAdmin": if (formData[key].ToBool()) where.Users_TenantAdmin(formData[key], "t0"); break;
                    case "DataViewFilters_Users_ServiceAdmin": if (formData[key].ToBool()) where.Users_ServiceAdmin(formData[key], "t0"); break;
                    case "DataViewFilters_Users_Developer": if (formData[key].ToBool()) where.Users_Developer(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_OwnerId": where.MailAddresses_OwnerId(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_OwnerType": where.MailAddresses_OwnerType(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_MailAddressId": where.MailAddresses_MailAddressId(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_Ver": where.MailAddresses_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_MailAddress": where.MailAddresses_MailAddress(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_Comments": where.MailAddresses_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_Creator": where.MailAddresses_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_Updator": where.MailAddresses_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_CreatedTime": where.MailAddresses_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_MailAddresses_UpdatedTime": where.MailAddresses_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_ReferenceType": where.Permissions_ReferenceType(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_ReferenceId": where.Permissions_ReferenceId(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_DeptId": where.Permissions_DeptId(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_UserId": where.Permissions_UserId(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_Ver": where.Permissions_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_DeptName": where.Permissions_DeptName(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_FullName1": where.Permissions_FullName1(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_FullName2": where.Permissions_FullName2(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_FirstAndLastNameOrder": where.Permissions_FirstAndLastNameOrder(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_PermissionType": where.Permissions_PermissionType(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_Comments": where.Permissions_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_Creator": where.Permissions_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_Updator": where.Permissions_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_CreatedTime": where.Permissions_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Permissions_UpdatedTime": where.Permissions_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_ReferenceType": where.OutgoingMails_ReferenceType(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_ReferenceId": where.OutgoingMails_ReferenceId(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_ReferenceVer": where.OutgoingMails_ReferenceVer(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_OutgoingMailId": where.OutgoingMails_OutgoingMailId(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Ver": where.OutgoingMails_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Host": where.OutgoingMails_Host(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Port": where.OutgoingMails_Port(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_From": where.OutgoingMails_From(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_To": where.OutgoingMails_To(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Cc": where.OutgoingMails_Cc(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Bcc": where.OutgoingMails_Bcc(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Title": where.OutgoingMails_Title(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Body": where.OutgoingMails_Body(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_SentTime": where.OutgoingMails_SentTime(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Comments": where.OutgoingMails_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Creator": where.OutgoingMails_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_Updator": where.OutgoingMails_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_CreatedTime": where.OutgoingMails_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_OutgoingMails_UpdatedTime": where.OutgoingMails_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_Word": where.SearchIndexes_Word(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_ReferenceId": where.SearchIndexes_ReferenceId(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_Ver": where.SearchIndexes_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_Priority": where.SearchIndexes_Priority(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_ReferenceType": where.SearchIndexes_ReferenceType(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_Title": where.SearchIndexes_Title(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_Subset": where.SearchIndexes_Subset(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_PermissionType": where.SearchIndexes_PermissionType(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_Comments": where.SearchIndexes_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_Creator": where.SearchIndexes_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_Updator": where.SearchIndexes_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_CreatedTime": where.SearchIndexes_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_SearchIndexes_UpdatedTime": where.SearchIndexes_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Items_ReferenceId": where.Items_ReferenceId(formData[key], "t0"); break;
                    case "DataViewFilters_Items_Ver": where.Items_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Items_ReferenceType": where.Items_ReferenceType(formData[key], "t0"); break;
                    case "DataViewFilters_Items_SiteId": where.Items_SiteId(formData[key], "t0"); break;
                    case "DataViewFilters_Items_Title": where.Items_Title(formData[key], "t0"); break;
                    case "DataViewFilters_Items_Subset": where.Items_Subset(formData[key], "t0"); break;
                    case "DataViewFilters_Items_Comments": where.Items_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Items_Creator": where.Items_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Items_Updator": where.Items_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Items_CreatedTime": where.Items_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Items_UpdatedTime": where.Items_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Items_UpdateTarget": if (formData[key].ToBool()) where.Items_UpdateTarget(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_TenantId": where.Sites_TenantId(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_SiteId": where.Sites_SiteId(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_UpdatedTime": where.Sites_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_Ver": where.Sites_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_Title": where.Sites_Title(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_Body": where.Sites_Body(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_TitleBody": where.Sites_TitleBody(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_ReferenceType": where.Sites_ReferenceType(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_ParentId": where.Sites_ParentId(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_InheritPermission": where.Sites_InheritPermission(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_PermissionType": where.Sites_PermissionType(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_SiteSettings": where.Sites_SiteSettings(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_Comments": where.Sites_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_Creator": where.Sites_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_Updator": where.Sites_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Sites_CreatedTime": where.Sites_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_ReferenceId": where.Orders_ReferenceId(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_ReferenceType": where.Orders_ReferenceType(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_OwnerId": where.Orders_OwnerId(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_Ver": where.Orders_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_Data": where.Orders_Data(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_Comments": where.Orders_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_Creator": where.Orders_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_Updator": where.Orders_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_CreatedTime": where.Orders_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Orders_UpdatedTime": where.Orders_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_ReferenceType": where.ExportSettings_ReferenceType(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_ReferenceId": where.ExportSettings_ReferenceId(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_Title": where.ExportSettings_Title(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_ExportSettingId": where.ExportSettings_ExportSettingId(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_Ver": where.ExportSettings_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_ExportColumns": where.ExportSettings_ExportColumns(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_Comments": where.ExportSettings_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_Creator": where.ExportSettings_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_Updator": where.ExportSettings_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_CreatedTime": where.ExportSettings_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_UpdatedTime": where.ExportSettings_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_ExportSettings_AddHeader": if (formData[key].ToBool()) where.ExportSettings_AddHeader(formData[key], "t0"); break;
                    case "DataViewFilters_Links_DestinationId": where.Links_DestinationId(formData[key], "t0"); break;
                    case "DataViewFilters_Links_SourceId": where.Links_SourceId(formData[key], "t0"); break;
                    case "DataViewFilters_Links_Ver": where.Links_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Links_ReferenceType": where.Links_ReferenceType(formData[key], "t0"); break;
                    case "DataViewFilters_Links_SiteId": where.Links_SiteId(formData[key], "t0"); break;
                    case "DataViewFilters_Links_Title": where.Links_Title(formData[key], "t0"); break;
                    case "DataViewFilters_Links_Subset": where.Links_Subset(formData[key], "t0"); break;
                    case "DataViewFilters_Links_SiteTitle": where.Links_SiteTitle(formData[key], "t0"); break;
                    case "DataViewFilters_Links_Comments": where.Links_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Links_Creator": where.Links_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Links_Updator": where.Links_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Links_CreatedTime": where.Links_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Links_UpdatedTime": where.Links_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_ReferenceId": where.Binaries_ReferenceId(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_BinaryId": where.Binaries_BinaryId(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Ver": where.Binaries_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_BinaryType": where.Binaries_BinaryType(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Title": where.Binaries_Title(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Body": where.Binaries_Body(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Bin": where.Binaries_Bin(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Thumbnail": where.Binaries_Thumbnail(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Icon": where.Binaries_Icon(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_FileName": where.Binaries_FileName(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Extension": where.Binaries_Extension(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Size": where.Binaries_Size(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_BinarySettings": where.Binaries_BinarySettings(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Comments": where.Binaries_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Creator": where.Binaries_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_Updator": where.Binaries_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_CreatedTime": where.Binaries_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Binaries_UpdatedTime": where.Binaries_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_SiteId": where.Issues_SiteId(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_UpdatedTime": where.Issues_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_IssueId": where.Issues_IssueId(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Ver": where.Issues_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Title": where.Issues_Title(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Body": where.Issues_Body(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_TitleBody": where.Issues_TitleBody(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_StartTime": where.Issues_StartTime(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CompletionTime": where.Issues_CompletionTime(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_WorkValue": where.Issues_WorkValue(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ProgressRate": where.Issues_ProgressRate(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_RemainingWorkValue": where.Issues_RemainingWorkValue(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Status": where.Issues_Status(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Manager": where.Issues_Manager(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Owner": where.Issues_Owner(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassA": where.Issues_ClassA(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassB": where.Issues_ClassB(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassC": where.Issues_ClassC(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassD": where.Issues_ClassD(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassE": where.Issues_ClassE(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassF": where.Issues_ClassF(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassG": where.Issues_ClassG(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassH": where.Issues_ClassH(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassI": where.Issues_ClassI(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassJ": where.Issues_ClassJ(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassK": where.Issues_ClassK(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassL": where.Issues_ClassL(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassM": where.Issues_ClassM(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassN": where.Issues_ClassN(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassO": where.Issues_ClassO(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_ClassP": where.Issues_ClassP(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_NumA": where.Issues_NumA(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_NumB": where.Issues_NumB(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_NumC": where.Issues_NumC(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_NumD": where.Issues_NumD(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_NumE": where.Issues_NumE(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_NumF": where.Issues_NumF(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_NumG": where.Issues_NumG(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_NumH": where.Issues_NumH(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DateA": where.Issues_DateA(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DateB": where.Issues_DateB(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DateC": where.Issues_DateC(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DateD": where.Issues_DateD(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DateE": where.Issues_DateE(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DateF": where.Issues_DateF(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DateG": where.Issues_DateG(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DateH": where.Issues_DateH(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DescriptionA": where.Issues_DescriptionA(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DescriptionB": where.Issues_DescriptionB(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DescriptionC": where.Issues_DescriptionC(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DescriptionD": where.Issues_DescriptionD(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DescriptionE": where.Issues_DescriptionE(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DescriptionF": where.Issues_DescriptionF(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DescriptionG": where.Issues_DescriptionG(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_DescriptionH": where.Issues_DescriptionH(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Comments": where.Issues_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Creator": where.Issues_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_Updator": where.Issues_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CreatedTime": where.Issues_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CheckA": if (formData[key].ToBool()) where.Issues_CheckA(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CheckB": if (formData[key].ToBool()) where.Issues_CheckB(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CheckC": if (formData[key].ToBool()) where.Issues_CheckC(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CheckD": if (formData[key].ToBool()) where.Issues_CheckD(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CheckE": if (formData[key].ToBool()) where.Issues_CheckE(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CheckF": if (formData[key].ToBool()) where.Issues_CheckF(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CheckG": if (formData[key].ToBool()) where.Issues_CheckG(formData[key], "t0"); break;
                    case "DataViewFilters_Issues_CheckH": if (formData[key].ToBool()) where.Issues_CheckH(formData[key], "t0"); break;
                    case "DataViewFilters_Results_SiteId": where.Results_SiteId(formData[key], "t0"); break;
                    case "DataViewFilters_Results_UpdatedTime": where.Results_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ResultId": where.Results_ResultId(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Ver": where.Results_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Title": where.Results_Title(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Body": where.Results_Body(formData[key], "t0"); break;
                    case "DataViewFilters_Results_TitleBody": where.Results_TitleBody(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Status": where.Results_Status(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Manager": where.Results_Manager(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Owner": where.Results_Owner(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassA": where.Results_ClassA(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassB": where.Results_ClassB(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassC": where.Results_ClassC(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassD": where.Results_ClassD(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassE": where.Results_ClassE(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassF": where.Results_ClassF(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassG": where.Results_ClassG(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassH": where.Results_ClassH(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassI": where.Results_ClassI(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassJ": where.Results_ClassJ(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassK": where.Results_ClassK(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassL": where.Results_ClassL(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassM": where.Results_ClassM(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassN": where.Results_ClassN(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassO": where.Results_ClassO(formData[key], "t0"); break;
                    case "DataViewFilters_Results_ClassP": where.Results_ClassP(formData[key], "t0"); break;
                    case "DataViewFilters_Results_NumA": where.Results_NumA(formData[key], "t0"); break;
                    case "DataViewFilters_Results_NumB": where.Results_NumB(formData[key], "t0"); break;
                    case "DataViewFilters_Results_NumC": where.Results_NumC(formData[key], "t0"); break;
                    case "DataViewFilters_Results_NumD": where.Results_NumD(formData[key], "t0"); break;
                    case "DataViewFilters_Results_NumE": where.Results_NumE(formData[key], "t0"); break;
                    case "DataViewFilters_Results_NumF": where.Results_NumF(formData[key], "t0"); break;
                    case "DataViewFilters_Results_NumG": where.Results_NumG(formData[key], "t0"); break;
                    case "DataViewFilters_Results_NumH": where.Results_NumH(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DateA": where.Results_DateA(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DateB": where.Results_DateB(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DateC": where.Results_DateC(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DateD": where.Results_DateD(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DateE": where.Results_DateE(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DateF": where.Results_DateF(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DateG": where.Results_DateG(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DateH": where.Results_DateH(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DescriptionA": where.Results_DescriptionA(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DescriptionB": where.Results_DescriptionB(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DescriptionC": where.Results_DescriptionC(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DescriptionD": where.Results_DescriptionD(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DescriptionE": where.Results_DescriptionE(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DescriptionF": where.Results_DescriptionF(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DescriptionG": where.Results_DescriptionG(formData[key], "t0"); break;
                    case "DataViewFilters_Results_DescriptionH": where.Results_DescriptionH(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Comments": where.Results_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Creator": where.Results_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Results_Updator": where.Results_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CreatedTime": where.Results_CreatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CheckA": if (formData[key].ToBool()) where.Results_CheckA(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CheckB": if (formData[key].ToBool()) where.Results_CheckB(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CheckC": if (formData[key].ToBool()) where.Results_CheckC(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CheckD": if (formData[key].ToBool()) where.Results_CheckD(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CheckE": if (formData[key].ToBool()) where.Results_CheckE(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CheckF": if (formData[key].ToBool()) where.Results_CheckF(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CheckG": if (formData[key].ToBool()) where.Results_CheckG(formData[key], "t0"); break;
                    case "DataViewFilters_Results_CheckH": if (formData[key].ToBool()) where.Results_CheckH(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_SiteId": where.Wikis_SiteId(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_UpdatedTime": where.Wikis_UpdatedTime(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_WikiId": where.Wikis_WikiId(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_Ver": where.Wikis_Ver(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_Title": where.Wikis_Title(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_Body": where.Wikis_Body(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_TitleBody": where.Wikis_TitleBody(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_Comments": where.Wikis_Comments(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_Creator": where.Wikis_Creator(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_Updator": where.Wikis_Updator(formData[key], "t0"); break;
                    case "DataViewFilters_Wikis_CreatedTime": where.Wikis_CreatedTime(formData[key], "t0"); break;
                    default: break;
                }
            });
            return where;
        }
    }
}
