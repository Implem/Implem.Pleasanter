using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class IssueValidators
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

        public static Error.Types OnEditing(SiteSettings ss, IssueModel issueModel)
        {
            switch (issueModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        ss.CanRead()&&
                        issueModel.AccessStatus != Databases.AccessStatuses.NotFound
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

        public static Error.Types OnCreating(SiteSettings ss, IssueModel issueModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(issueModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (issueModel.Title_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (issueModel.Body_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "WorkValue":
                        if (issueModel.WorkValue_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ProgressRate":
                        if (issueModel.ProgressRate_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Status":
                        if (issueModel.Status_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Manager":
                        if (issueModel.Manager_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Owner":
                        if (issueModel.Owner_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (issueModel.ClassA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (issueModel.ClassB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (issueModel.ClassC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (issueModel.ClassD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (issueModel.ClassE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (issueModel.ClassF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (issueModel.ClassG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (issueModel.ClassH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (issueModel.ClassI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (issueModel.ClassJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (issueModel.ClassK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (issueModel.ClassL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (issueModel.ClassM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (issueModel.ClassN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (issueModel.ClassO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (issueModel.ClassP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (issueModel.ClassQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (issueModel.ClassR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (issueModel.ClassS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (issueModel.ClassT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (issueModel.ClassU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (issueModel.ClassV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (issueModel.ClassW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (issueModel.ClassX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (issueModel.ClassY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (issueModel.ClassZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (issueModel.NumA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (issueModel.NumB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (issueModel.NumC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (issueModel.NumD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (issueModel.NumE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (issueModel.NumF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (issueModel.NumG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (issueModel.NumH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (issueModel.NumI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (issueModel.NumJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (issueModel.NumK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (issueModel.NumL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (issueModel.NumM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (issueModel.NumN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (issueModel.NumO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (issueModel.NumP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (issueModel.NumQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (issueModel.NumR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (issueModel.NumS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (issueModel.NumT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (issueModel.NumU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (issueModel.NumV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (issueModel.NumW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (issueModel.NumX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (issueModel.NumY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (issueModel.NumZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (issueModel.DescriptionA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (issueModel.DescriptionB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (issueModel.DescriptionC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (issueModel.DescriptionD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (issueModel.DescriptionE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (issueModel.DescriptionF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (issueModel.DescriptionG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (issueModel.DescriptionH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (issueModel.DescriptionI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (issueModel.DescriptionJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (issueModel.DescriptionK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (issueModel.DescriptionL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (issueModel.DescriptionM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (issueModel.DescriptionN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (issueModel.DescriptionO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (issueModel.DescriptionP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (issueModel.DescriptionQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (issueModel.DescriptionR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (issueModel.DescriptionS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (issueModel.DescriptionT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (issueModel.DescriptionU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (issueModel.DescriptionV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (issueModel.DescriptionW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (issueModel.DescriptionX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (issueModel.DescriptionY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (issueModel.DescriptionZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (issueModel.CheckA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (issueModel.CheckB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (issueModel.CheckC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (issueModel.CheckD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (issueModel.CheckE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (issueModel.CheckF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (issueModel.CheckG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (issueModel.CheckH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (issueModel.CheckI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (issueModel.CheckJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (issueModel.CheckK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (issueModel.CheckL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (issueModel.CheckM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (issueModel.CheckN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (issueModel.CheckO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (issueModel.CheckP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (issueModel.CheckQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (issueModel.CheckR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (issueModel.CheckS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (issueModel.CheckT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (issueModel.CheckU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (issueModel.CheckV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (issueModel.CheckW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (issueModel.CheckX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (issueModel.CheckY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (issueModel.CheckZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsA":
                        if (issueModel.AttachmentsA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsB":
                        if (issueModel.AttachmentsB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsC":
                        if (issueModel.AttachmentsC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsD":
                        if (issueModel.AttachmentsD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsE":
                        if (issueModel.AttachmentsE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsF":
                        if (issueModel.AttachmentsF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsG":
                        if (issueModel.AttachmentsG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsH":
                        if (issueModel.AttachmentsH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsI":
                        if (issueModel.AttachmentsI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsJ":
                        if (issueModel.AttachmentsJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsK":
                        if (issueModel.AttachmentsK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsL":
                        if (issueModel.AttachmentsL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsM":
                        if (issueModel.AttachmentsM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsN":
                        if (issueModel.AttachmentsN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsO":
                        if (issueModel.AttachmentsO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsP":
                        if (issueModel.AttachmentsP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsQ":
                        if (issueModel.AttachmentsQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsR":
                        if (issueModel.AttachmentsR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsS":
                        if (issueModel.AttachmentsS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsT":
                        if (issueModel.AttachmentsT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsU":
                        if (issueModel.AttachmentsU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsV":
                        if (issueModel.AttachmentsV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsW":
                        if (issueModel.AttachmentsW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsX":
                        if (issueModel.AttachmentsX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsY":
                        if (issueModel.AttachmentsY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsZ":
                        if (issueModel.AttachmentsZ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "StartTime":
                        if (issueModel.StartTime_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CompletionTime":
                        if (issueModel.CompletionTime_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (issueModel.DateA_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (issueModel.DateB_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (issueModel.DateC_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (issueModel.DateD_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (issueModel.DateE_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (issueModel.DateF_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (issueModel.DateG_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (issueModel.DateH_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (issueModel.DateI_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (issueModel.DateJ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (issueModel.DateK_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (issueModel.DateL_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (issueModel.DateM_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (issueModel.DateN_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (issueModel.DateO_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (issueModel.DateP_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (issueModel.DateQ_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (issueModel.DateR_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (issueModel.DateS_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (issueModel.DateT_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (issueModel.DateU_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (issueModel.DateV_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (issueModel.DateW_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (issueModel.DateX_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (issueModel.DateY_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (issueModel.DateZ_Updated(column))
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

        public static Error.Types OnUpdating(SiteSettings ss, IssueModel issueModel)
        {
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(issueModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (issueModel.Title_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Body":
                        if (issueModel.Body_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "StartTime":
                        if (issueModel.StartTime_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CompletionTime":
                        if (issueModel.CompletionTime_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "WorkValue":
                        if (issueModel.WorkValue_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ProgressRate":
                        if (issueModel.ProgressRate_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Status":
                        if (issueModel.Status_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Manager":
                        if (issueModel.Manager_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Owner":
                        if (issueModel.Owner_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassA":
                        if (issueModel.ClassA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassB":
                        if (issueModel.ClassB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassC":
                        if (issueModel.ClassC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassD":
                        if (issueModel.ClassD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassE":
                        if (issueModel.ClassE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassF":
                        if (issueModel.ClassF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassG":
                        if (issueModel.ClassG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassH":
                        if (issueModel.ClassH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassI":
                        if (issueModel.ClassI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassJ":
                        if (issueModel.ClassJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassK":
                        if (issueModel.ClassK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassL":
                        if (issueModel.ClassL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassM":
                        if (issueModel.ClassM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassN":
                        if (issueModel.ClassN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassO":
                        if (issueModel.ClassO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassP":
                        if (issueModel.ClassP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassQ":
                        if (issueModel.ClassQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassR":
                        if (issueModel.ClassR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassS":
                        if (issueModel.ClassS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassT":
                        if (issueModel.ClassT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassU":
                        if (issueModel.ClassU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassV":
                        if (issueModel.ClassV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassW":
                        if (issueModel.ClassW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassX":
                        if (issueModel.ClassX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassY":
                        if (issueModel.ClassY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassZ":
                        if (issueModel.ClassZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumA":
                        if (issueModel.NumA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumB":
                        if (issueModel.NumB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumC":
                        if (issueModel.NumC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumD":
                        if (issueModel.NumD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumE":
                        if (issueModel.NumE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumF":
                        if (issueModel.NumF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumG":
                        if (issueModel.NumG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumH":
                        if (issueModel.NumH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumI":
                        if (issueModel.NumI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumJ":
                        if (issueModel.NumJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumK":
                        if (issueModel.NumK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumL":
                        if (issueModel.NumL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumM":
                        if (issueModel.NumM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumN":
                        if (issueModel.NumN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumO":
                        if (issueModel.NumO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumP":
                        if (issueModel.NumP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumQ":
                        if (issueModel.NumQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumR":
                        if (issueModel.NumR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumS":
                        if (issueModel.NumS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumT":
                        if (issueModel.NumT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumU":
                        if (issueModel.NumU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumV":
                        if (issueModel.NumV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumW":
                        if (issueModel.NumW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumX":
                        if (issueModel.NumX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumY":
                        if (issueModel.NumY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumZ":
                        if (issueModel.NumZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateA":
                        if (issueModel.DateA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateB":
                        if (issueModel.DateB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateC":
                        if (issueModel.DateC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateD":
                        if (issueModel.DateD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateE":
                        if (issueModel.DateE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateF":
                        if (issueModel.DateF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateG":
                        if (issueModel.DateG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateH":
                        if (issueModel.DateH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateI":
                        if (issueModel.DateI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateJ":
                        if (issueModel.DateJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateK":
                        if (issueModel.DateK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateL":
                        if (issueModel.DateL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateM":
                        if (issueModel.DateM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateN":
                        if (issueModel.DateN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateO":
                        if (issueModel.DateO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateP":
                        if (issueModel.DateP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateQ":
                        if (issueModel.DateQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateR":
                        if (issueModel.DateR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateS":
                        if (issueModel.DateS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateT":
                        if (issueModel.DateT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateU":
                        if (issueModel.DateU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateV":
                        if (issueModel.DateV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateW":
                        if (issueModel.DateW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateX":
                        if (issueModel.DateX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateY":
                        if (issueModel.DateY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateZ":
                        if (issueModel.DateZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionA":
                        if (issueModel.DescriptionA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionB":
                        if (issueModel.DescriptionB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionC":
                        if (issueModel.DescriptionC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionD":
                        if (issueModel.DescriptionD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionE":
                        if (issueModel.DescriptionE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionF":
                        if (issueModel.DescriptionF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionG":
                        if (issueModel.DescriptionG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionH":
                        if (issueModel.DescriptionH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionI":
                        if (issueModel.DescriptionI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionJ":
                        if (issueModel.DescriptionJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionK":
                        if (issueModel.DescriptionK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionL":
                        if (issueModel.DescriptionL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionM":
                        if (issueModel.DescriptionM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionN":
                        if (issueModel.DescriptionN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionO":
                        if (issueModel.DescriptionO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionP":
                        if (issueModel.DescriptionP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionQ":
                        if (issueModel.DescriptionQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionR":
                        if (issueModel.DescriptionR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionS":
                        if (issueModel.DescriptionS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionT":
                        if (issueModel.DescriptionT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionU":
                        if (issueModel.DescriptionU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionV":
                        if (issueModel.DescriptionV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionW":
                        if (issueModel.DescriptionW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionX":
                        if (issueModel.DescriptionX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionY":
                        if (issueModel.DescriptionY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionZ":
                        if (issueModel.DescriptionZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckA":
                        if (issueModel.CheckA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckB":
                        if (issueModel.CheckB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckC":
                        if (issueModel.CheckC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckD":
                        if (issueModel.CheckD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckE":
                        if (issueModel.CheckE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckF":
                        if (issueModel.CheckF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckG":
                        if (issueModel.CheckG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckH":
                        if (issueModel.CheckH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckI":
                        if (issueModel.CheckI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckJ":
                        if (issueModel.CheckJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckK":
                        if (issueModel.CheckK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckL":
                        if (issueModel.CheckL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckM":
                        if (issueModel.CheckM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckN":
                        if (issueModel.CheckN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckO":
                        if (issueModel.CheckO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckP":
                        if (issueModel.CheckP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckQ":
                        if (issueModel.CheckQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckR":
                        if (issueModel.CheckR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckS":
                        if (issueModel.CheckS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckT":
                        if (issueModel.CheckT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckU":
                        if (issueModel.CheckU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckV":
                        if (issueModel.CheckV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckW":
                        if (issueModel.CheckW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckX":
                        if (issueModel.CheckX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckY":
                        if (issueModel.CheckY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckZ":
                        if (issueModel.CheckZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsA":
                        if (issueModel.AttachmentsA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsB":
                        if (issueModel.AttachmentsB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsC":
                        if (issueModel.AttachmentsC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsD":
                        if (issueModel.AttachmentsD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsE":
                        if (issueModel.AttachmentsE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsF":
                        if (issueModel.AttachmentsF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsG":
                        if (issueModel.AttachmentsG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsH":
                        if (issueModel.AttachmentsH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsI":
                        if (issueModel.AttachmentsI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsJ":
                        if (issueModel.AttachmentsJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsK":
                        if (issueModel.AttachmentsK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsL":
                        if (issueModel.AttachmentsL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsM":
                        if (issueModel.AttachmentsM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsN":
                        if (issueModel.AttachmentsN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsO":
                        if (issueModel.AttachmentsO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsP":
                        if (issueModel.AttachmentsP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsQ":
                        if (issueModel.AttachmentsQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsR":
                        if (issueModel.AttachmentsR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsS":
                        if (issueModel.AttachmentsS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsT":
                        if (issueModel.AttachmentsT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsU":
                        if (issueModel.AttachmentsU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsV":
                        if (issueModel.AttachmentsV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsW":
                        if (issueModel.AttachmentsW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsX":
                        if (issueModel.AttachmentsX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsY":
                        if (issueModel.AttachmentsY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsZ":
                        if (issueModel.AttachmentsZ_Updated()) return Error.Types.HasNotPermission;
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

        public static Error.Types OnDeleting(SiteSettings ss, IssueModel issueModel)
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
