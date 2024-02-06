using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class RegistrationModel : BaseModel
    {
        public int TenantId = 0;
        public int RegistrationId = 0;
        public string MailAddress = string.Empty;
        public int Invitee = 0;
        public string InviteeName = string.Empty;
        public string LoginId = string.Empty;
        public string Name = string.Empty;
        public string Password = string.Empty;
        public string PasswordValidate = string.Empty;
        public string Language = "ja";
        public string Passphrase = string.Empty;
        public string Invitingflg = string.Empty;
        public int UserId = 0;
        public int DeptId = 0;
        public int GroupId = 0;
        public int SavedTenantId = 0;
        public int SavedRegistrationId = 0;
        public string SavedMailAddress = string.Empty;
        public int SavedInvitee = 0;
        public string SavedInviteeName = string.Empty;
        public string SavedLoginId = string.Empty;
        public string SavedName = string.Empty;
        public string SavedPassword = string.Empty;
        public string SavedPasswordValidate = string.Empty;
        public string SavedLanguage = "ja";
        public string SavedPassphrase = string.Empty;
        public string SavedInvitingflg = string.Empty;
        public int SavedUserId = 0;
        public int SavedDeptId = 0;
        public int SavedGroupId = 0;

        public bool TenantId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != TenantId;
            }
            return TenantId != SavedTenantId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool RegistrationId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != RegistrationId;
            }
            return RegistrationId != SavedRegistrationId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != RegistrationId);
        }

        public bool MailAddress_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != MailAddress;
            }
            return MailAddress != SavedMailAddress && MailAddress != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != MailAddress);
        }

        public bool Invitee_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Invitee;
            }
            return Invitee != SavedInvitee
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Invitee);
        }

        public bool InviteeName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != InviteeName;
            }
            return InviteeName != SavedInviteeName && InviteeName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != InviteeName);
        }

        public bool LoginId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != LoginId;
            }
            return LoginId != SavedLoginId && LoginId != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != LoginId);
        }

        public bool Name_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Name;
            }
            return Name != SavedName && Name != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Name);
        }

        public bool Password_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Password;
            }
            return Password != SavedPassword && Password != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Password);
        }

        public bool Language_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Language;
            }
            return Language != SavedLanguage && Language != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Language);
        }

        public bool Passphrase_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Passphrase;
            }
            return Passphrase != SavedPassphrase && Passphrase != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Passphrase);
        }

        public bool Invitingflg_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Invitingflg;
            }
            return Invitingflg != SavedInvitingflg && Invitingflg != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Invitingflg);
        }

        public bool UserId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != UserId;
            }
            return UserId != SavedUserId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != UserId);
        }

        public bool DeptId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != DeptId;
            }
            return DeptId != SavedDeptId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != DeptId);
        }

        public bool GroupId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != GroupId;
            }
            return GroupId != SavedGroupId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != GroupId);
        }

        public List<int> SwitchTargets;

        public RegistrationModel()
        {
        }

        public RegistrationModel(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData = null,
            RegistrationApiModel registrationApiModel = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (registrationApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: registrationApiModel);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public RegistrationModel(
            Context context,
            SiteSettings ss,
            int registrationId,
            Dictionary<string, string> formData = null,
            RegistrationApiModel registrationApiModel = null,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            RegistrationId = registrationId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.RegistrationsWhereDefault(
                        context: context,
                        registrationModel: this)
                            .Registrations_Ver(context.QueryStrings.Int("ver")), ss: ss);
            }
            else
            {
                Get(
                    context: context,
                    ss: ss,
                    column: column);
            }
            if (clearSessions) ClearSessions(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (registrationApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: registrationApiModel);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public RegistrationModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public RegistrationModel Get(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            where = where ?? Rds.RegistrationsWhereDefault(
                context: context,
                registrationModel: this);
            column = (column ?? Rds.RegistrationsDefaultColumns());
            join = join ?? Rds.RegistrationsJoinDefault();
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectRegistrations(
                    tableType: tableType,
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public RegistrationApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new RegistrationApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(context: context, noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "TenantId": data.TenantId = TenantId; break;
                    case "RegistrationId": data.RegistrationId = RegistrationId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "MailAddress": data.MailAddress = MailAddress; break;
                    case "Invitee": data.Invitee = Invitee; break;
                    case "InviteeName": data.InviteeName = InviteeName; break;
                    case "LoginId": data.LoginId = LoginId; break;
                    case "Name": data.Name = Name; break;
                    case "Password": data.Password = Password; break;
                    case "Language": data.Language = Language; break;
                    case "Passphrase": data.Passphrase = Passphrase; break;
                    case "Invitingflg": data.Invitingflg = Invitingflg; break;
                    case "UserId": data.UserId = UserId; break;
                    case "DeptId": data.DeptId = DeptId; break;
                    case "GroupId": data.GroupId = GroupId; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                    default: 
                        data.Value(
                            context: context,
                            column: column,
                            value: GetValue(
                                context: context,
                                column: column,
                                toLocal: true));
                        break;
                }
            });
            return data;
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "RegistrationId":
                    return RegistrationId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "MailAddress":
                    return MailAddress.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Invitee":
                    return Invitee.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "InviteeName":
                    return InviteeName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginId":
                    return LoginId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Name":
                    return Name.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Password":
                    return Password.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordValidate":
                    return PasswordValidate.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Language":
                    return Language.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Passphrase":
                    return Passphrase.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Invitingflg":
                    return Invitingflg.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserId":
                    return UserId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptId":
                    return DeptId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupId":
                    return GroupId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "TenantId":
                    return TenantId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RegistrationId":
                    return RegistrationId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MailAddress":
                    return MailAddress.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Invitee":
                    return Invitee.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "InviteeName":
                    return InviteeName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginId":
                    return LoginId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Name":
                    return Name.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Password":
                    return Password.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordValidate":
                    return PasswordValidate.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Language":
                    return Language.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Passphrase":
                    return Passphrase.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Invitingflg":
                    return Invitingflg.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserId":
                    return UserId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptId":
                    return DeptId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupId":
                    return GroupId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "TenantId":
                    return TenantId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RegistrationId":
                    return RegistrationId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MailAddress":
                    return MailAddress.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Invitee":
                    return Invitee.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "InviteeName":
                    return InviteeName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginId":
                    return LoginId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Name":
                    return Name.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Password":
                    return Password.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordValidate":
                    return PasswordValidate.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Language":
                    return Language.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Passphrase":
                    return Passphrase.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Invitingflg":
                    return Invitingflg.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserId":
                    return UserId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptId":
                    return DeptId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupId":
                    return GroupId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public ErrorData Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            string noticeType = "Created",
            bool otherInitValue = false,
            bool get = true)
        {
            TenantId = context.TenantId;
            var statements = new List<SqlStatement>();
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            RegistrationId = (response.Id ?? RegistrationId).ToInt();
            if (get) Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertRegistrations(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.RegistrationsParamDefault(
                        context: context,
                        ss: ss,
                        registrationModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
            });
            return statements;
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true,
            bool checkConflict = true)
        {
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            var verUp = Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp);
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements,
                checkConflict: checkConflict,
                verUp: verUp));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: RegistrationId);
            }
            if (get)
            {
                Get(context: context, ss: ss);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null,
            bool checkConflict = true,
            bool verUp = false)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.RegistrationsWhereDefault(
                context: context,
                registrationModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.RegistrationsCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateRegistrations(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.RegistrationsParamDefault(
                        context: context,
                        ss: ss,
                        registrationModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(RegistrationId))
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = RegistrationId
                },
            };
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertRegistrations(
                    where: where ?? Rds.RegistrationsWhereDefault(
                        context: context,
                        registrationModel: this),
                    param: param ?? Rds.RegistrationsParamDefault(
                        context: context,
                        ss: ss,
                        registrationModel: this,
                        setDefault: true)),
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            RegistrationId = (response.Id ?? RegistrationId).ToInt();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.RegistrationsWhere().RegistrationId(RegistrationId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteRegistrations(
                    factory: context,
                    where: where),
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,int registrationId)
        {
            RegistrationId = registrationId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreRegistrations(
                        factory: context,
                        where: Rds.RegistrationsWhere().RegistrationId(RegistrationId)),
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteRegistrations(
                    tableType: tableType,
                    param: Rds.RegistrationsParam().RegistrationId(RegistrationId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData)
        {
            SetByFormData(
                context: context,
                ss: ss,
                formData: formData);
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Ver = context.QueryStrings.Int("ver");
            }
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        private void SetByFormData(Context context, SiteSettings ss, Dictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Registrations_MailAddress": MailAddress = value.ToString(); break;
                    case "Registrations_Invitee": Invitee = value.ToInt(); break;
                    case "Registrations_InviteeName": InviteeName = value.ToString(); break;
                    case "Registrations_LoginId": LoginId = value.ToString(); break;
                    case "Registrations_Name": Name = value.ToString(); break;
                    case "Registrations_Password": Password = value.ToString().Sha512Cng(); break;
                    case "Registrations_PasswordValidate": PasswordValidate = value.ToString().Sha512Cng(); break;
                    case "Registrations_Language": Language = value.ToString(); break;
                    case "Registrations_Passphrase": Passphrase = value.ToString(); break;
                    case "Registrations_Invitingflg": Invitingflg = value.ToString(); break;
                    case "Registrations_UserId": UserId = value.ToInt(); break;
                    case "Registrations_DeptId": DeptId = value.ToInt(); break;
                    case "Registrations_GroupId": GroupId = value.ToInt(); break;
                    case "Registrations_Timestamp": Timestamp = value.ToString(); break;
                    case "Comments": Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: value); break;
                    case "VerUp": VerUp = value.ToBool(); break;
                    default:
                        if (key.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: key.Substring("Comment".Length).ToInt(),
                                body: value);
                        }
                        else
                        {
                            var column = ss.GetColumn(
                                context: context,
                                columnName: key.Split_2nd('_'));
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.ColumnName,
                                        value: new Num(
                                            context: context,
                                            column: column,
                                            value: value));
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
        }

        public void SetByModel(RegistrationModel registrationModel)
        {
            TenantId = registrationModel.TenantId;
            MailAddress = registrationModel.MailAddress;
            Invitee = registrationModel.Invitee;
            InviteeName = registrationModel.InviteeName;
            LoginId = registrationModel.LoginId;
            Name = registrationModel.Name;
            Password = registrationModel.Password;
            PasswordValidate = registrationModel.PasswordValidate;
            Language = registrationModel.Language;
            Passphrase = registrationModel.Passphrase;
            Invitingflg = registrationModel.Invitingflg;
            UserId = registrationModel.UserId;
            DeptId = registrationModel.DeptId;
            GroupId = registrationModel.GroupId;
            Comments = registrationModel.Comments;
            Creator = registrationModel.Creator;
            Updator = registrationModel.Updator;
            CreatedTime = registrationModel.CreatedTime;
            UpdatedTime = registrationModel.UpdatedTime;
            VerUp = registrationModel.VerUp;
            Comments = registrationModel.Comments;
            ClassHash = registrationModel.ClassHash;
            NumHash = registrationModel.NumHash;
            DateHash = registrationModel.DateHash;
            DescriptionHash = registrationModel.DescriptionHash;
            CheckHash = registrationModel.CheckHash;
            AttachmentsHash = registrationModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss, RegistrationApiModel data)
        {
            if (data.MailAddress != null) MailAddress = data.MailAddress.ToString().ToString();
            if (data.Invitee != null) Invitee = data.Invitee.ToInt().ToInt();
            if (data.InviteeName != null) InviteeName = data.InviteeName.ToString().ToString();
            if (data.LoginId != null) LoginId = data.LoginId.ToString().ToString();
            if (data.Name != null) Name = data.Name.ToString().ToString();
            if (data.Password != null) Password = data.Password.ToString().ToString().Sha512Cng();
            if (data.Language != null) Language = data.Language.ToString().ToString();
            if (data.Passphrase != null) Passphrase = data.Passphrase.ToString().ToString();
            if (data.Invitingflg != null) Invitingflg = data.Invitingflg.ToString().ToString();
            if (data.UserId != null) UserId = data.UserId.ToInt().ToInt();
            if (data.DeptId != null) DeptId = data.DeptId.ToInt().ToInt();
            if (data.GroupId != null) GroupId = data.GroupId.ToInt().ToInt();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => SetClass(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => SetNum(
                columnName: o.Key,
                value: new Num(
                    context: context,
                    column: ss.GetColumn(
                        context: context,
                        columnName: o.Key),
                    value: o.Value.ToString())));
            data.DateHash?.ForEach(o => SetDate(
                columnName: o.Key,
                value: o.Value.ToDateTime().ToUniversal(context: context)));
            data.DescriptionHash?.ForEach(o => SetDescription(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash?.ForEach(o => SetCheck(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash?.ForEach(o =>
            {
                string columnName = o.Key;
                Attachments newAttachments = o.Value;
                Attachments oldAttachments;
                if (columnName == "Attachments#Uploading")
                {
                    var kvp = AttachmentsHash
                        .FirstOrDefault(x => x.Value
                            .Any(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st()));
                    columnName = kvp.Key;
                    oldAttachments = kvp.Value;
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    if (column.OverwriteSameFileName == true)
                    {
                        var oldAtt = oldAttachments
                            .FirstOrDefault(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st());
                        if (oldAtt != null)
                        {
                            oldAtt.Deleted = true;
                            oldAtt.Overwritten = true;
                        }
                    }
                    newAttachments.ForEach(att => att.Guid = att.Guid.Split_2nd());
                }
                else
                {
                    oldAttachments = AttachmentsHash.Get(columnName);
                }
                if (oldAttachments != null)
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    var newGuidSet = new HashSet<string>(newAttachments.Select(x => x.Guid).Distinct());
                    var newNameSet = new HashSet<string>(newAttachments.Select(x => x.Name).Distinct());
                    newAttachments.ForEach(newAttachment =>
                    {
                        newAttachment.AttachmentAction(
                            context: context,
                            column: column,
                            oldAttachments: oldAttachments);
                    });
                    if (column.OverwriteSameFileName == true)
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) =>
                                !newGuidSet.Contains(oldvalue.Guid) &&
                                !newNameSet.Contains(oldvalue.Name)));
                    }
                    else
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) => !newGuidSet.Contains(oldvalue.Guid)));
                    }
                }
                SetAttachments(columnName: columnName, value: newAttachments);
            });
        }

        public string ReplacedDisplayValues(
            Context context,
            SiteSettings ss,
            string value)
        {
            ss.IncludedColumns(value: value).ForEach(column =>
                value = value.Replace(
                    $"[{column.ColumnName}]",
                    ToDisplay(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: Mine(context: context))));
            value = ReplacedContextValues(context, value);
            return value;
        }

        private string ReplacedContextValues(Context context, string value)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: RegistrationId);
            var mailAddress = MailAddressUtilities.Get(
                context: context,
                userId: context.UserId);
            value = value
                .Replace("{Url}", url)
                .Replace("{LoginId}", context.User.LoginId)
                .Replace("{UserName}", context.User.Name)
                .Replace("{MailAddress}", mailAddress);
            return value;
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "TenantId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                TenantId = dataRow[column.ColumnName].ToInt();
                                SavedTenantId = TenantId;
                            }
                            break;
                        case "RegistrationId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                RegistrationId = dataRow[column.ColumnName].ToInt();
                                SavedRegistrationId = RegistrationId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "MailAddress":
                            MailAddress = dataRow[column.ColumnName].ToString();
                            SavedMailAddress = MailAddress;
                            break;
                        case "Invitee":
                            Invitee = dataRow[column.ColumnName].ToInt();
                            SavedInvitee = Invitee;
                            break;
                        case "InviteeName":
                            InviteeName = dataRow[column.ColumnName].ToString();
                            SavedInviteeName = InviteeName;
                            break;
                        case "LoginId":
                            LoginId = dataRow[column.ColumnName].ToString();
                            SavedLoginId = LoginId;
                            break;
                        case "Name":
                            Name = dataRow[column.ColumnName].ToString();
                            SavedName = Name;
                            break;
                        case "Password":
                            Password = dataRow[column.ColumnName].ToString();
                            SavedPassword = Password;
                            break;
                        case "Language":
                            Language = dataRow[column.ColumnName].ToString();
                            SavedLanguage = Language;
                            break;
                        case "Passphrase":
                            Passphrase = dataRow[column.ColumnName].ToString();
                            SavedPassphrase = Passphrase;
                            break;
                        case "Invitingflg":
                            Invitingflg = dataRow[column.ColumnName].ToString();
                            SavedInvitingflg = Invitingflg;
                            break;
                        case "UserId":
                            UserId = dataRow[column.ColumnName].ToInt();
                            SavedUserId = UserId;
                            break;
                        case "DeptId":
                            DeptId = dataRow[column.ColumnName].ToInt();
                            SavedDeptId = DeptId;
                            break;
                        case "GroupId":
                            GroupId = dataRow[column.ColumnName].ToInt();
                            SavedGroupId = GroupId;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedClass(
                                        columnName: column.Name,
                                        value: GetClass(columnName: column.Name));
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.Name,
                                        value: new Num(
                                            dataRow: dataRow,
                                            name: column.ColumnName));
                                    SetSavedNum(
                                        columnName: column.Name,
                                        value: GetNum(columnName: column.Name).Value);
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SetSavedDate(
                                        columnName: column.Name,
                                        value: GetDate(columnName: column.Name));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedDescription(
                                        columnName: column.Name,
                                        value: GetDescription(columnName: column.Name));
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SetSavedCheck(
                                        columnName: column.Name,
                                        value: GetCheck(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SetSavedAttachments(
                                        columnName: column.Name,
                                        value: GetAttachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || TenantId_Updated(context: context)
                || RegistrationId_Updated(context: context)
                || Ver_Updated(context: context)
                || MailAddress_Updated(context: context)
                || Invitee_Updated(context: context)
                || InviteeName_Updated(context: context)
                || LoginId_Updated(context: context)
                || Name_Updated(context: context)
                || Password_Updated(context: context)
                || Language_Updated(context: context)
                || Passphrase_Updated(context: context)
                || Invitingflg_Updated(context: context)
                || UserId_Updated(context: context)
                || DeptId_Updated(context: context)
                || GroupId_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)));
        }

         public bool Updated(Context context, SiteSettings ss)
        {
            return UpdatedWithColumn(context: context, ss: ss)
                || TenantId_Updated(context: context)
                || RegistrationId_Updated(context: context)
                || Ver_Updated(context: context)
                || MailAddress_Updated(context: context)
                || Invitee_Updated(context: context)
                || InviteeName_Updated(context: context)
                || LoginId_Updated(context: context)
                || Name_Updated(context: context)
                || Password_Updated(context: context)
                || Language_Updated(context: context)
                || Passphrase_Updated(context: context)
                || Invitingflg_Updated(context: context)
                || UserId_Updated(context: context)
                || DeptId_Updated(context: context)
                || GroupId_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public override List<string> Mine(Context context)
        {
            if (MineCache == null)
            {
                var mine = new List<string>();
                var userId = context.UserId;
                if (SavedCreator == userId) mine.Add("Creator");
                if (SavedUpdator == userId) mine.Add("Updator");
                MineCache = mine;
            }
            return MineCache;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Title Title
        {
            get
            {
                return new Title(MailAddress);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void Approval(Context context)
        {
            Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertUsers(
                        selectIdentity: true,
                        param: Rds.UsersParam()
                            .TenantId(TenantId)
                            .LoginId(LoginId)
                            .Password(Password)
                            .Name(Name)
                            .Language(Language)
                            ),
                    Rds.InsertMailAddresses(
                        param: Rds.MailAddressesParam()
                            .OwnerId(raw: Def.Sql.Identity)
                            .OwnerType("Users")
                            .MailAddress(MailAddress))
                }).Id.ToInt();
            SiteInfo.Reflesh(
                context: context,
                force: true);
        }
    }
}
