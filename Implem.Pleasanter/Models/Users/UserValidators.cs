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
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnEntry(SiteSettings ss)
        {
            return Permissions.CanManageTenant()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(SiteSettings ss)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return ss.CanRead()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(SiteSettings ss, UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            switch (userModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        ss.CanRead()&&
                        userModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? Error.Types.None
                            : Error.Types.NotFound;        
                case BaseModel.MethodTypes.New:
                    return ss.CanCreate()
                        ? Error.Types.None
                        : Error.Types.HasNotPermission;
                default:
                    return Error.Types.NotFound;
            }
        }

        public static Error.Types OnCreating(SiteSettings ss, UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(userModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Name":
                        if (userModel.Name_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Password":
                        if (userModel.Password_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Language":
                        if (userModel.Language_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (userModel.Body_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (userModel.ClassA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (userModel.ClassB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (userModel.ClassC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (userModel.ClassD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (userModel.ClassE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (userModel.ClassF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (userModel.ClassG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (userModel.ClassH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (userModel.ClassI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (userModel.ClassJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (userModel.ClassK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (userModel.ClassL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (userModel.ClassM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (userModel.ClassN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (userModel.ClassO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (userModel.ClassP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (userModel.ClassQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (userModel.ClassR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (userModel.ClassS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (userModel.ClassT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (userModel.ClassU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (userModel.ClassV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (userModel.ClassW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (userModel.ClassX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (userModel.ClassY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (userModel.ClassZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (userModel.NumA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (userModel.NumB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (userModel.NumC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (userModel.NumD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (userModel.NumE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (userModel.NumF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (userModel.NumG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (userModel.NumH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (userModel.NumI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (userModel.NumJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (userModel.NumK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (userModel.NumL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (userModel.NumM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (userModel.NumN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (userModel.NumO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (userModel.NumP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (userModel.NumQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (userModel.NumR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (userModel.NumS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (userModel.NumT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (userModel.NumU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (userModel.NumV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (userModel.NumW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (userModel.NumX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (userModel.NumY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (userModel.NumZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (userModel.DescriptionA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (userModel.DescriptionB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (userModel.DescriptionC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (userModel.DescriptionD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (userModel.DescriptionE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (userModel.DescriptionF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (userModel.DescriptionG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (userModel.DescriptionH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (userModel.DescriptionI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (userModel.DescriptionJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (userModel.DescriptionK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (userModel.DescriptionL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (userModel.DescriptionM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (userModel.DescriptionN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (userModel.DescriptionO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (userModel.DescriptionP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (userModel.DescriptionQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (userModel.DescriptionR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (userModel.DescriptionS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (userModel.DescriptionT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (userModel.DescriptionU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (userModel.DescriptionV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (userModel.DescriptionW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (userModel.DescriptionX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (userModel.DescriptionY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (userModel.DescriptionZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (userModel.CheckA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (userModel.CheckB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (userModel.CheckC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (userModel.CheckD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (userModel.CheckE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (userModel.CheckF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (userModel.CheckG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (userModel.CheckH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (userModel.CheckI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (userModel.CheckJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (userModel.CheckK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (userModel.CheckL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (userModel.CheckM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (userModel.CheckN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (userModel.CheckO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (userModel.CheckP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (userModel.CheckQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (userModel.CheckR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (userModel.CheckS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (userModel.CheckT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (userModel.CheckU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (userModel.CheckV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (userModel.CheckW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (userModel.CheckX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (userModel.CheckY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (userModel.CheckZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (userModel.DateA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (userModel.DateB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (userModel.DateC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (userModel.DateD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (userModel.DateE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (userModel.DateF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (userModel.DateG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (userModel.DateH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (userModel.DateI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (userModel.DateJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (userModel.DateK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (userModel.DateL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (userModel.DateM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (userModel.DateN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (userModel.DateO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (userModel.DateP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (userModel.DateQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (userModel.DateR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (userModel.DateS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (userModel.DateT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (userModel.DateU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (userModel.DateV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (userModel.DateW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (userModel.DateX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (userModel.DateY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (userModel.DateZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate) return Error.Types.HasNotPermission;
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss, UserModel userModel)
        {
            if (Forms.Exists("Users_TenantManager") && userModel.Self())
            {
                return Error.Types.PermissionNotSelfChange;
            }
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(userModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Name":
                        if (userModel.Name_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Password":
                        if (userModel.Password_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Language":
                        if (userModel.Language_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Body":
                        if (userModel.Body_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassA":
                        if (userModel.ClassA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassB":
                        if (userModel.ClassB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassC":
                        if (userModel.ClassC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassD":
                        if (userModel.ClassD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassE":
                        if (userModel.ClassE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassF":
                        if (userModel.ClassF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassG":
                        if (userModel.ClassG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassH":
                        if (userModel.ClassH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassI":
                        if (userModel.ClassI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassJ":
                        if (userModel.ClassJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassK":
                        if (userModel.ClassK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassL":
                        if (userModel.ClassL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassM":
                        if (userModel.ClassM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassN":
                        if (userModel.ClassN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassO":
                        if (userModel.ClassO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassP":
                        if (userModel.ClassP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassQ":
                        if (userModel.ClassQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassR":
                        if (userModel.ClassR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassS":
                        if (userModel.ClassS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassT":
                        if (userModel.ClassT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassU":
                        if (userModel.ClassU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassV":
                        if (userModel.ClassV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassW":
                        if (userModel.ClassW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassX":
                        if (userModel.ClassX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassY":
                        if (userModel.ClassY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassZ":
                        if (userModel.ClassZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumA":
                        if (userModel.NumA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumB":
                        if (userModel.NumB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumC":
                        if (userModel.NumC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumD":
                        if (userModel.NumD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumE":
                        if (userModel.NumE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumF":
                        if (userModel.NumF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumG":
                        if (userModel.NumG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumH":
                        if (userModel.NumH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumI":
                        if (userModel.NumI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumJ":
                        if (userModel.NumJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumK":
                        if (userModel.NumK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumL":
                        if (userModel.NumL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumM":
                        if (userModel.NumM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumN":
                        if (userModel.NumN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumO":
                        if (userModel.NumO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumP":
                        if (userModel.NumP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumQ":
                        if (userModel.NumQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumR":
                        if (userModel.NumR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumS":
                        if (userModel.NumS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumT":
                        if (userModel.NumT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumU":
                        if (userModel.NumU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumV":
                        if (userModel.NumV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumW":
                        if (userModel.NumW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumX":
                        if (userModel.NumX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumY":
                        if (userModel.NumY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumZ":
                        if (userModel.NumZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateA":
                        if (userModel.DateA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateB":
                        if (userModel.DateB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateC":
                        if (userModel.DateC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateD":
                        if (userModel.DateD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateE":
                        if (userModel.DateE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateF":
                        if (userModel.DateF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateG":
                        if (userModel.DateG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateH":
                        if (userModel.DateH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateI":
                        if (userModel.DateI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateJ":
                        if (userModel.DateJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateK":
                        if (userModel.DateK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateL":
                        if (userModel.DateL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateM":
                        if (userModel.DateM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateN":
                        if (userModel.DateN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateO":
                        if (userModel.DateO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateP":
                        if (userModel.DateP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateQ":
                        if (userModel.DateQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateR":
                        if (userModel.DateR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateS":
                        if (userModel.DateS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateT":
                        if (userModel.DateT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateU":
                        if (userModel.DateU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateV":
                        if (userModel.DateV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateW":
                        if (userModel.DateW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateX":
                        if (userModel.DateX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateY":
                        if (userModel.DateY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateZ":
                        if (userModel.DateZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionA":
                        if (userModel.DescriptionA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionB":
                        if (userModel.DescriptionB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionC":
                        if (userModel.DescriptionC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionD":
                        if (userModel.DescriptionD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionE":
                        if (userModel.DescriptionE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionF":
                        if (userModel.DescriptionF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionG":
                        if (userModel.DescriptionG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionH":
                        if (userModel.DescriptionH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionI":
                        if (userModel.DescriptionI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionJ":
                        if (userModel.DescriptionJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionK":
                        if (userModel.DescriptionK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionL":
                        if (userModel.DescriptionL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionM":
                        if (userModel.DescriptionM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionN":
                        if (userModel.DescriptionN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionO":
                        if (userModel.DescriptionO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionP":
                        if (userModel.DescriptionP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionQ":
                        if (userModel.DescriptionQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionR":
                        if (userModel.DescriptionR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionS":
                        if (userModel.DescriptionS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionT":
                        if (userModel.DescriptionT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionU":
                        if (userModel.DescriptionU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionV":
                        if (userModel.DescriptionV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionW":
                        if (userModel.DescriptionW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionX":
                        if (userModel.DescriptionX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionY":
                        if (userModel.DescriptionY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionZ":
                        if (userModel.DescriptionZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckA":
                        if (userModel.CheckA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckB":
                        if (userModel.CheckB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckC":
                        if (userModel.CheckC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckD":
                        if (userModel.CheckD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckE":
                        if (userModel.CheckE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckF":
                        if (userModel.CheckF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckG":
                        if (userModel.CheckG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckH":
                        if (userModel.CheckH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckI":
                        if (userModel.CheckI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckJ":
                        if (userModel.CheckJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckK":
                        if (userModel.CheckK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckL":
                        if (userModel.CheckL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckM":
                        if (userModel.CheckM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckN":
                        if (userModel.CheckN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckO":
                        if (userModel.CheckO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckP":
                        if (userModel.CheckP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckQ":
                        if (userModel.CheckQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckR":
                        if (userModel.CheckR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckS":
                        if (userModel.CheckS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckT":
                        if (userModel.CheckT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckU":
                        if (userModel.CheckU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckV":
                        if (userModel.CheckV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckW":
                        if (userModel.CheckW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckX":
                        if (userModel.CheckX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckY":
                        if (userModel.CheckY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckZ":
                        if (userModel.CheckZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate) return Error.Types.HasNotPermission;
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return ss.CanDelete()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnRestoring()
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return Permissions.CanManageTenant()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return ss.CanExport()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordChanging(UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.UserId == Sessions.UserId())
            {
                if (userModel.OldPassword == userModel.ChangedPassword)
                {
                    return Error.Types.PasswordNotChanged;
                }
                if (!userModel.GetByCredentials(userModel.LoginId, userModel.OldPassword))
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
        public static Error.Types OnPasswordChangingAtLogin(UserModel userModel)
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
                userModel.LoginId, userModel.Password, Forms.Int("SelectedTenantId")))
            {
                return Error.Types.IncorrectCurrentPassword;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordResetting()
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (!Permissions.CanManageTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnAddingMailAddress(
            SiteSettings ss,
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
            if (!Permissions.CanManageTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnApiEditing(UserModel userModel)
        {
            if (!Contract.Api())
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
        public static Error.Types OnApiCreating(UserModel userModel)
        {
            if (!Contract.Api())
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
        public static Error.Types OnApiDeleting(UserModel userModel)
        {
            if (!Contract.Api())
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
