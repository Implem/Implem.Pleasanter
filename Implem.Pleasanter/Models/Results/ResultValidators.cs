using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class ResultValidators
    {
        public static Error.Types OnEntry(SiteSettings ss)
        {
            return ss.HasPermission()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(SiteSettings ss)
        {
            return ss.CanRead()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(SiteSettings ss, ResultModel resultModel)
        {
            switch (resultModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        ss.CanRead()&&
                        resultModel.AccessStatus != Databases.AccessStatuses.NotFound
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

        public static Error.Types OnCreating(SiteSettings ss, ResultModel resultModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(resultModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (resultModel.Title_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (resultModel.Body_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Status":
                        if (resultModel.Status_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Manager":
                        if (resultModel.Manager_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Owner":
                        if (resultModel.Owner_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (resultModel.ClassA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (resultModel.ClassB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (resultModel.ClassC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (resultModel.ClassD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (resultModel.ClassE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (resultModel.ClassF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (resultModel.ClassG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (resultModel.ClassH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (resultModel.ClassI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (resultModel.ClassJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (resultModel.ClassK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (resultModel.ClassL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (resultModel.ClassM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (resultModel.ClassN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (resultModel.ClassO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (resultModel.ClassP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (resultModel.ClassQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (resultModel.ClassR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (resultModel.ClassS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (resultModel.ClassT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (resultModel.ClassU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (resultModel.ClassV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (resultModel.ClassW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (resultModel.ClassX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (resultModel.ClassY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (resultModel.ClassZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (resultModel.NumA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (resultModel.NumB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (resultModel.NumC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (resultModel.NumD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (resultModel.NumE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (resultModel.NumF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (resultModel.NumG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (resultModel.NumH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (resultModel.NumI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (resultModel.NumJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (resultModel.NumK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (resultModel.NumL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (resultModel.NumM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (resultModel.NumN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (resultModel.NumO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (resultModel.NumP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (resultModel.NumQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (resultModel.NumR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (resultModel.NumS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (resultModel.NumT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (resultModel.NumU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (resultModel.NumV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (resultModel.NumW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (resultModel.NumX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (resultModel.NumY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (resultModel.NumZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (resultModel.DescriptionA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (resultModel.DescriptionB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (resultModel.DescriptionC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (resultModel.DescriptionD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (resultModel.DescriptionE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (resultModel.DescriptionF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (resultModel.DescriptionG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (resultModel.DescriptionH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (resultModel.DescriptionI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (resultModel.DescriptionJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (resultModel.DescriptionK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (resultModel.DescriptionL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (resultModel.DescriptionM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (resultModel.DescriptionN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (resultModel.DescriptionO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (resultModel.DescriptionP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (resultModel.DescriptionQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (resultModel.DescriptionR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (resultModel.DescriptionS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (resultModel.DescriptionT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (resultModel.DescriptionU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (resultModel.DescriptionV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (resultModel.DescriptionW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (resultModel.DescriptionX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (resultModel.DescriptionY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (resultModel.DescriptionZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (resultModel.CheckA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (resultModel.CheckB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (resultModel.CheckC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (resultModel.CheckD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (resultModel.CheckE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (resultModel.CheckF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (resultModel.CheckG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (resultModel.CheckH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (resultModel.CheckI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (resultModel.CheckJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (resultModel.CheckK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (resultModel.CheckL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (resultModel.CheckM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (resultModel.CheckN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (resultModel.CheckO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (resultModel.CheckP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (resultModel.CheckQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (resultModel.CheckR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (resultModel.CheckS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (resultModel.CheckT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (resultModel.CheckU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (resultModel.CheckV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (resultModel.CheckW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (resultModel.CheckX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (resultModel.CheckY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (resultModel.CheckZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsA":
                        if (resultModel.AttachmentsA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsB":
                        if (resultModel.AttachmentsB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsC":
                        if (resultModel.AttachmentsC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsD":
                        if (resultModel.AttachmentsD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsE":
                        if (resultModel.AttachmentsE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsF":
                        if (resultModel.AttachmentsF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsG":
                        if (resultModel.AttachmentsG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsH":
                        if (resultModel.AttachmentsH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsI":
                        if (resultModel.AttachmentsI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsJ":
                        if (resultModel.AttachmentsJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsK":
                        if (resultModel.AttachmentsK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsL":
                        if (resultModel.AttachmentsL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsM":
                        if (resultModel.AttachmentsM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsN":
                        if (resultModel.AttachmentsN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsO":
                        if (resultModel.AttachmentsO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsP":
                        if (resultModel.AttachmentsP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsQ":
                        if (resultModel.AttachmentsQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsR":
                        if (resultModel.AttachmentsR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsS":
                        if (resultModel.AttachmentsS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsT":
                        if (resultModel.AttachmentsT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsU":
                        if (resultModel.AttachmentsU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsV":
                        if (resultModel.AttachmentsV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsW":
                        if (resultModel.AttachmentsW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsX":
                        if (resultModel.AttachmentsX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsY":
                        if (resultModel.AttachmentsY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsZ":
                        if (resultModel.AttachmentsZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (resultModel.DateA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (resultModel.DateB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (resultModel.DateC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (resultModel.DateD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (resultModel.DateE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (resultModel.DateF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (resultModel.DateG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (resultModel.DateH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (resultModel.DateI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (resultModel.DateJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (resultModel.DateK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (resultModel.DateL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (resultModel.DateM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (resultModel.DateN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (resultModel.DateO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (resultModel.DateP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (resultModel.DateQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (resultModel.DateR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (resultModel.DateS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (resultModel.DateT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (resultModel.DateU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (resultModel.DateV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (resultModel.DateW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (resultModel.DateX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (resultModel.DateY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (resultModel.DateZ_Updated(column))
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

        public static Error.Types OnUpdating(SiteSettings ss, ResultModel resultModel)
        {
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(resultModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (resultModel.Title_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Body":
                        if (resultModel.Body_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Status":
                        if (resultModel.Status_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Manager":
                        if (resultModel.Manager_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Owner":
                        if (resultModel.Owner_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassA":
                        if (resultModel.ClassA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassB":
                        if (resultModel.ClassB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassC":
                        if (resultModel.ClassC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassD":
                        if (resultModel.ClassD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassE":
                        if (resultModel.ClassE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassF":
                        if (resultModel.ClassF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassG":
                        if (resultModel.ClassG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassH":
                        if (resultModel.ClassH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassI":
                        if (resultModel.ClassI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassJ":
                        if (resultModel.ClassJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassK":
                        if (resultModel.ClassK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassL":
                        if (resultModel.ClassL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassM":
                        if (resultModel.ClassM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassN":
                        if (resultModel.ClassN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassO":
                        if (resultModel.ClassO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassP":
                        if (resultModel.ClassP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassQ":
                        if (resultModel.ClassQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassR":
                        if (resultModel.ClassR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassS":
                        if (resultModel.ClassS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassT":
                        if (resultModel.ClassT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassU":
                        if (resultModel.ClassU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassV":
                        if (resultModel.ClassV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassW":
                        if (resultModel.ClassW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassX":
                        if (resultModel.ClassX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassY":
                        if (resultModel.ClassY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassZ":
                        if (resultModel.ClassZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumA":
                        if (resultModel.NumA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumB":
                        if (resultModel.NumB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumC":
                        if (resultModel.NumC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumD":
                        if (resultModel.NumD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumE":
                        if (resultModel.NumE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumF":
                        if (resultModel.NumF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumG":
                        if (resultModel.NumG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumH":
                        if (resultModel.NumH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumI":
                        if (resultModel.NumI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumJ":
                        if (resultModel.NumJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumK":
                        if (resultModel.NumK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumL":
                        if (resultModel.NumL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumM":
                        if (resultModel.NumM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumN":
                        if (resultModel.NumN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumO":
                        if (resultModel.NumO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumP":
                        if (resultModel.NumP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumQ":
                        if (resultModel.NumQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumR":
                        if (resultModel.NumR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumS":
                        if (resultModel.NumS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumT":
                        if (resultModel.NumT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumU":
                        if (resultModel.NumU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumV":
                        if (resultModel.NumV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumW":
                        if (resultModel.NumW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumX":
                        if (resultModel.NumX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumY":
                        if (resultModel.NumY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumZ":
                        if (resultModel.NumZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateA":
                        if (resultModel.DateA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateB":
                        if (resultModel.DateB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateC":
                        if (resultModel.DateC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateD":
                        if (resultModel.DateD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateE":
                        if (resultModel.DateE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateF":
                        if (resultModel.DateF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateG":
                        if (resultModel.DateG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateH":
                        if (resultModel.DateH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateI":
                        if (resultModel.DateI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateJ":
                        if (resultModel.DateJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateK":
                        if (resultModel.DateK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateL":
                        if (resultModel.DateL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateM":
                        if (resultModel.DateM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateN":
                        if (resultModel.DateN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateO":
                        if (resultModel.DateO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateP":
                        if (resultModel.DateP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateQ":
                        if (resultModel.DateQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateR":
                        if (resultModel.DateR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateS":
                        if (resultModel.DateS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateT":
                        if (resultModel.DateT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateU":
                        if (resultModel.DateU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateV":
                        if (resultModel.DateV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateW":
                        if (resultModel.DateW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateX":
                        if (resultModel.DateX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateY":
                        if (resultModel.DateY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateZ":
                        if (resultModel.DateZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionA":
                        if (resultModel.DescriptionA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionB":
                        if (resultModel.DescriptionB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionC":
                        if (resultModel.DescriptionC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionD":
                        if (resultModel.DescriptionD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionE":
                        if (resultModel.DescriptionE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionF":
                        if (resultModel.DescriptionF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionG":
                        if (resultModel.DescriptionG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionH":
                        if (resultModel.DescriptionH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionI":
                        if (resultModel.DescriptionI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionJ":
                        if (resultModel.DescriptionJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionK":
                        if (resultModel.DescriptionK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionL":
                        if (resultModel.DescriptionL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionM":
                        if (resultModel.DescriptionM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionN":
                        if (resultModel.DescriptionN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionO":
                        if (resultModel.DescriptionO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionP":
                        if (resultModel.DescriptionP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionQ":
                        if (resultModel.DescriptionQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionR":
                        if (resultModel.DescriptionR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionS":
                        if (resultModel.DescriptionS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionT":
                        if (resultModel.DescriptionT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionU":
                        if (resultModel.DescriptionU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionV":
                        if (resultModel.DescriptionV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionW":
                        if (resultModel.DescriptionW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionX":
                        if (resultModel.DescriptionX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionY":
                        if (resultModel.DescriptionY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionZ":
                        if (resultModel.DescriptionZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckA":
                        if (resultModel.CheckA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckB":
                        if (resultModel.CheckB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckC":
                        if (resultModel.CheckC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckD":
                        if (resultModel.CheckD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckE":
                        if (resultModel.CheckE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckF":
                        if (resultModel.CheckF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckG":
                        if (resultModel.CheckG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckH":
                        if (resultModel.CheckH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckI":
                        if (resultModel.CheckI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckJ":
                        if (resultModel.CheckJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckK":
                        if (resultModel.CheckK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckL":
                        if (resultModel.CheckL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckM":
                        if (resultModel.CheckM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckN":
                        if (resultModel.CheckN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckO":
                        if (resultModel.CheckO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckP":
                        if (resultModel.CheckP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckQ":
                        if (resultModel.CheckQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckR":
                        if (resultModel.CheckR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckS":
                        if (resultModel.CheckS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckT":
                        if (resultModel.CheckT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckU":
                        if (resultModel.CheckU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckV":
                        if (resultModel.CheckV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckW":
                        if (resultModel.CheckW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckX":
                        if (resultModel.CheckX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckY":
                        if (resultModel.CheckY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckZ":
                        if (resultModel.CheckZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsA":
                        if (resultModel.AttachmentsA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsB":
                        if (resultModel.AttachmentsB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsC":
                        if (resultModel.AttachmentsC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsD":
                        if (resultModel.AttachmentsD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsE":
                        if (resultModel.AttachmentsE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsF":
                        if (resultModel.AttachmentsF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsG":
                        if (resultModel.AttachmentsG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsH":
                        if (resultModel.AttachmentsH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsI":
                        if (resultModel.AttachmentsI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsJ":
                        if (resultModel.AttachmentsJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsK":
                        if (resultModel.AttachmentsK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsL":
                        if (resultModel.AttachmentsL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsM":
                        if (resultModel.AttachmentsM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsN":
                        if (resultModel.AttachmentsN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsO":
                        if (resultModel.AttachmentsO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsP":
                        if (resultModel.AttachmentsP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsQ":
                        if (resultModel.AttachmentsQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsR":
                        if (resultModel.AttachmentsR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsS":
                        if (resultModel.AttachmentsS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsT":
                        if (resultModel.AttachmentsT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsU":
                        if (resultModel.AttachmentsU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsV":
                        if (resultModel.AttachmentsV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsW":
                        if (resultModel.AttachmentsW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsX":
                        if (resultModel.AttachmentsX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsY":
                        if (resultModel.AttachmentsY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsZ":
                        if (resultModel.AttachmentsZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate) return Error.Types.HasNotPermission;
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnMoving(SiteSettings source, SiteSettings destination)
        {
            if (!Permissions.CanMove(source, destination))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, ResultModel resultModel)
        {
            return ss.CanDelete()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnRestoring()
        {
            return Permissions.CanManageTenant()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            return ss.CanExport()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }
    }
}
