using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class UserValidators
    {
        public static Error.Types OnEntry(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return context.HasPermission(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return context.CanRead(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(
            Context context, SiteSettings ss, UserModel userModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            switch (userModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)&&
                        userModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? Error.Types.None
                            : Error.Types.NotFound;        
                case BaseModel.MethodTypes.New:
                    return context.CanCreate(ss: ss)
                        ? Error.Types.None
                        : Error.Types.HasNotPermission;
                default:
                    return Error.Types.NotFound;
            }
        }

        public static Error.Types OnCreating(
            Context context, SiteSettings ss, UserModel userModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (!context.CanCreate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: userModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Name":
                        if (userModel.Name_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Password":
                        if (userModel.Password_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Language":
                        if (userModel.Language_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (userModel.Body_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (userModel.ClassA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (userModel.ClassB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (userModel.ClassC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (userModel.ClassD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (userModel.ClassE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (userModel.ClassF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (userModel.ClassG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (userModel.ClassH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (userModel.ClassI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (userModel.ClassJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (userModel.ClassK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (userModel.ClassL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (userModel.ClassM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (userModel.ClassN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (userModel.ClassO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (userModel.ClassP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (userModel.ClassQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (userModel.ClassR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (userModel.ClassS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (userModel.ClassT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (userModel.ClassU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (userModel.ClassV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (userModel.ClassW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (userModel.ClassX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (userModel.ClassY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (userModel.ClassZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (userModel.NumA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (userModel.NumB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (userModel.NumC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (userModel.NumD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (userModel.NumE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (userModel.NumF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (userModel.NumG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (userModel.NumH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (userModel.NumI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (userModel.NumJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (userModel.NumK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (userModel.NumL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (userModel.NumM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (userModel.NumN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (userModel.NumO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (userModel.NumP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (userModel.NumQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (userModel.NumR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (userModel.NumS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (userModel.NumT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (userModel.NumU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (userModel.NumV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (userModel.NumW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (userModel.NumX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (userModel.NumY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (userModel.NumZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (userModel.DescriptionA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (userModel.DescriptionB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (userModel.DescriptionC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (userModel.DescriptionD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (userModel.DescriptionE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (userModel.DescriptionF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (userModel.DescriptionG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (userModel.DescriptionH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (userModel.DescriptionI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (userModel.DescriptionJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (userModel.DescriptionK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (userModel.DescriptionL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (userModel.DescriptionM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (userModel.DescriptionN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (userModel.DescriptionO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (userModel.DescriptionP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (userModel.DescriptionQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (userModel.DescriptionR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (userModel.DescriptionS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (userModel.DescriptionT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (userModel.DescriptionU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (userModel.DescriptionV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (userModel.DescriptionW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (userModel.DescriptionX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (userModel.DescriptionY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (userModel.DescriptionZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (userModel.CheckA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (userModel.CheckB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (userModel.CheckC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (userModel.CheckD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (userModel.CheckE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (userModel.CheckF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (userModel.CheckG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (userModel.CheckH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (userModel.CheckI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (userModel.CheckJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (userModel.CheckK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (userModel.CheckL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (userModel.CheckM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (userModel.CheckN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (userModel.CheckO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (userModel.CheckP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (userModel.CheckQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (userModel.CheckR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (userModel.CheckS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (userModel.CheckT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (userModel.CheckU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (userModel.CheckV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (userModel.CheckW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (userModel.CheckX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (userModel.CheckY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (userModel.CheckZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LdapSearchRoot":
                        if (userModel.LdapSearchRoot_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (userModel.DateA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (userModel.DateB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (userModel.DateC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (userModel.DateD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (userModel.DateE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (userModel.DateF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (userModel.DateG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (userModel.DateH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (userModel.DateI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (userModel.DateJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (userModel.DateK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (userModel.DateL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (userModel.DateM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (userModel.DateN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (userModel.DateO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (userModel.DateP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (userModel.DateQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (userModel.DateR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (userModel.DateS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (userModel.DateT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (userModel.DateU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (userModel.DateV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (userModel.DateW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (userModel.DateX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (userModel.DateY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (userModel.DateZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "SynchronizedTime":
                        if (userModel.SynchronizedTime_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            Context context, SiteSettings ss, UserModel userModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (Forms.Exists("Users_TenantManager") && userModel.Self(context: context))
            {
                return Error.Types.PermissionNotSelfChange;
            }
            if (!context.CanUpdate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: userModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Name":
                        if (userModel.Name_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Password":
                        if (userModel.Password_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Language":
                        if (userModel.Language_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (userModel.Body_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (userModel.ClassA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (userModel.ClassB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (userModel.ClassC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (userModel.ClassD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (userModel.ClassE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (userModel.ClassF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (userModel.ClassG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (userModel.ClassH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (userModel.ClassI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (userModel.ClassJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (userModel.ClassK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (userModel.ClassL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (userModel.ClassM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (userModel.ClassN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (userModel.ClassO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (userModel.ClassP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (userModel.ClassQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (userModel.ClassR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (userModel.ClassS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (userModel.ClassT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (userModel.ClassU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (userModel.ClassV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (userModel.ClassW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (userModel.ClassX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (userModel.ClassY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (userModel.ClassZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (userModel.NumA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (userModel.NumB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (userModel.NumC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (userModel.NumD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (userModel.NumE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (userModel.NumF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (userModel.NumG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (userModel.NumH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (userModel.NumI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (userModel.NumJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (userModel.NumK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (userModel.NumL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (userModel.NumM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (userModel.NumN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (userModel.NumO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (userModel.NumP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (userModel.NumQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (userModel.NumR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (userModel.NumS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (userModel.NumT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (userModel.NumU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (userModel.NumV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (userModel.NumW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (userModel.NumX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (userModel.NumY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (userModel.NumZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (userModel.DateA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (userModel.DateB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (userModel.DateC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (userModel.DateD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (userModel.DateE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (userModel.DateF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (userModel.DateG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (userModel.DateH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (userModel.DateI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (userModel.DateJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (userModel.DateK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (userModel.DateL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (userModel.DateM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (userModel.DateN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (userModel.DateO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (userModel.DateP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (userModel.DateQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (userModel.DateR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (userModel.DateS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (userModel.DateT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (userModel.DateU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (userModel.DateV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (userModel.DateW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (userModel.DateX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (userModel.DateY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (userModel.DateZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (userModel.DescriptionA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (userModel.DescriptionB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (userModel.DescriptionC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (userModel.DescriptionD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (userModel.DescriptionE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (userModel.DescriptionF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (userModel.DescriptionG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (userModel.DescriptionH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (userModel.DescriptionI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (userModel.DescriptionJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (userModel.DescriptionK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (userModel.DescriptionL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (userModel.DescriptionM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (userModel.DescriptionN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (userModel.DescriptionO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (userModel.DescriptionP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (userModel.DescriptionQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (userModel.DescriptionR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (userModel.DescriptionS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (userModel.DescriptionT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (userModel.DescriptionU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (userModel.DescriptionV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (userModel.DescriptionW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (userModel.DescriptionX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (userModel.DescriptionY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (userModel.DescriptionZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (userModel.CheckA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (userModel.CheckB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (userModel.CheckC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (userModel.CheckD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (userModel.CheckE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (userModel.CheckF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (userModel.CheckG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (userModel.CheckH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (userModel.CheckI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (userModel.CheckJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (userModel.CheckK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (userModel.CheckL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (userModel.CheckM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (userModel.CheckN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (userModel.CheckO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (userModel.CheckP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (userModel.CheckQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (userModel.CheckR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (userModel.CheckS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (userModel.CheckT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (userModel.CheckU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (userModel.CheckV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (userModel.CheckW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (userModel.CheckX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (userModel.CheckY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (userModel.CheckZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LdapSearchRoot":
                        if (userModel.LdapSearchRoot_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "SynchronizedTime":
                        if (userModel.SynchronizedTime_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            Context context, SiteSettings ss, UserModel userModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return context.CanDelete(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnRestoring(Context context, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return Permissions.CanManageTenant(context: context)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return context.CanExport(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnEntry(Context context, SiteSettings ss)
        {
            return Permissions.CanManageTenant(context: context)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordChanging(Context context, UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.UserId == context.UserId)
            {
                if (userModel.OldPassword == userModel.ChangedPassword)
                {
                    return Error.Types.PasswordNotChanged;
                }
                if (!userModel.GetByCredentials(
                    context: context,
                    loginId: userModel.LoginId,
                    password: userModel.OldPassword))
                {
                    return Error.Types.IncorrectCurrentPassword;
                }
            }
            else
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordChangingAtLogin(
            Context context, UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.Password == userModel.ChangedPassword)
            {
                return Error.Types.PasswordNotChanged;
            }
            else if (!userModel.GetByCredentials(
                context: context,
                loginId: userModel.LoginId,
                password: userModel.Password,
                tenantId: Forms.Int("SelectedTenantId")))
            {
                return Error.Types.IncorrectCurrentPassword;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordResetting(Context context)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (!Permissions.CanManageTenant(context: context))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnAddingMailAddress(
            Context context,
            UserModel userModel,
            string mailAddress,
            out string data)
        {
            var error = MailAddressValidators.BadMailAddress(mailAddress, out data);
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (error.Has())
            {
                return error;
            }
            if (mailAddress.Trim() == string.Empty)
            {
                return Error.Types.InputMailAddress;
            }
            if (!Permissions.CanManageTenant(context: context)
                && !userModel.Self(context: context))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnApiEditing(Context context, UserModel userModel)
        {
            if (context.ContractSettings.Api == false || !Parameters.Api.Enabled)
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Error.Types.InvalidRequest;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnApiCreating(Context context, UserModel userModel)
        {
            if (context.ContractSettings.Api == false || !Parameters.Api.Enabled)
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Error.Types.InvalidRequest;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnApiDeleting(Context context, UserModel userModel)
        {
            if (context.ContractSettings.Api == false || !Parameters.Api.Enabled)
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Error.Types.InvalidRequest;
            }
            return Error.Types.None;
        }
    }
}
