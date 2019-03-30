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
    public static class IssueValidators
    {
        public static Error.Types OnEntry(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
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
            return context.CanRead(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            switch (issueModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss) &&
                        issueModel.AccessStatus != Databases.AccessStatuses.NotFound
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
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!context.CanCreate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: issueModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (issueModel.Title_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (issueModel.Body_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "WorkValue":
                        if (issueModel.WorkValue_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ProgressRate":
                        if (issueModel.ProgressRate_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Status":
                        if (issueModel.Status_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Manager":
                        if (issueModel.Manager_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Owner":
                        if (issueModel.Owner_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (issueModel.ClassA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (issueModel.ClassB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (issueModel.ClassC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (issueModel.ClassD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (issueModel.ClassE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (issueModel.ClassF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (issueModel.ClassG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (issueModel.ClassH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (issueModel.ClassI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (issueModel.ClassJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (issueModel.ClassK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (issueModel.ClassL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (issueModel.ClassM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (issueModel.ClassN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (issueModel.ClassO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (issueModel.ClassP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (issueModel.ClassQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (issueModel.ClassR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (issueModel.ClassS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (issueModel.ClassT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (issueModel.ClassU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (issueModel.ClassV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (issueModel.ClassW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (issueModel.ClassX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (issueModel.ClassY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (issueModel.ClassZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (issueModel.NumA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (issueModel.NumB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (issueModel.NumC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (issueModel.NumD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (issueModel.NumE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (issueModel.NumF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (issueModel.NumG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (issueModel.NumH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (issueModel.NumI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (issueModel.NumJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (issueModel.NumK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (issueModel.NumL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (issueModel.NumM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (issueModel.NumN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (issueModel.NumO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (issueModel.NumP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (issueModel.NumQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (issueModel.NumR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (issueModel.NumS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (issueModel.NumT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (issueModel.NumU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (issueModel.NumV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (issueModel.NumW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (issueModel.NumX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (issueModel.NumY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (issueModel.NumZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (issueModel.DescriptionA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (issueModel.DescriptionB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (issueModel.DescriptionC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (issueModel.DescriptionD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (issueModel.DescriptionE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (issueModel.DescriptionF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (issueModel.DescriptionG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (issueModel.DescriptionH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (issueModel.DescriptionI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (issueModel.DescriptionJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (issueModel.DescriptionK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (issueModel.DescriptionL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (issueModel.DescriptionM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (issueModel.DescriptionN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (issueModel.DescriptionO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (issueModel.DescriptionP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (issueModel.DescriptionQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (issueModel.DescriptionR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (issueModel.DescriptionS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (issueModel.DescriptionT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (issueModel.DescriptionU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (issueModel.DescriptionV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (issueModel.DescriptionW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (issueModel.DescriptionX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (issueModel.DescriptionY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (issueModel.DescriptionZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (issueModel.CheckA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (issueModel.CheckB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (issueModel.CheckC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (issueModel.CheckD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (issueModel.CheckE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (issueModel.CheckF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (issueModel.CheckG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (issueModel.CheckH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (issueModel.CheckI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (issueModel.CheckJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (issueModel.CheckK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (issueModel.CheckL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (issueModel.CheckM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (issueModel.CheckN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (issueModel.CheckO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (issueModel.CheckP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (issueModel.CheckQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (issueModel.CheckR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (issueModel.CheckS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (issueModel.CheckT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (issueModel.CheckU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (issueModel.CheckV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (issueModel.CheckW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (issueModel.CheckX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (issueModel.CheckY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (issueModel.CheckZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsA":
                        if (issueModel.AttachmentsA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsB":
                        if (issueModel.AttachmentsB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsC":
                        if (issueModel.AttachmentsC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsD":
                        if (issueModel.AttachmentsD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsE":
                        if (issueModel.AttachmentsE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsF":
                        if (issueModel.AttachmentsF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsG":
                        if (issueModel.AttachmentsG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsH":
                        if (issueModel.AttachmentsH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsI":
                        if (issueModel.AttachmentsI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsJ":
                        if (issueModel.AttachmentsJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsK":
                        if (issueModel.AttachmentsK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsL":
                        if (issueModel.AttachmentsL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsM":
                        if (issueModel.AttachmentsM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsN":
                        if (issueModel.AttachmentsN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsO":
                        if (issueModel.AttachmentsO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsP":
                        if (issueModel.AttachmentsP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsQ":
                        if (issueModel.AttachmentsQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsR":
                        if (issueModel.AttachmentsR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsS":
                        if (issueModel.AttachmentsS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsT":
                        if (issueModel.AttachmentsT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsU":
                        if (issueModel.AttachmentsU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsV":
                        if (issueModel.AttachmentsV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsW":
                        if (issueModel.AttachmentsW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsX":
                        if (issueModel.AttachmentsX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsY":
                        if (issueModel.AttachmentsY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsZ":
                        if (issueModel.AttachmentsZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "StartTime":
                        if (issueModel.StartTime_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CompletionTime":
                        if (issueModel.CompletionTime_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (issueModel.DateA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (issueModel.DateB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (issueModel.DateC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (issueModel.DateD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (issueModel.DateE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (issueModel.DateF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (issueModel.DateG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (issueModel.DateH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (issueModel.DateI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (issueModel.DateJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (issueModel.DateK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (issueModel.DateL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (issueModel.DateM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (issueModel.DateN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (issueModel.DateO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (issueModel.DateP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (issueModel.DateQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (issueModel.DateR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (issueModel.DateS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (issueModel.DateT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (issueModel.DateU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (issueModel.DateV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (issueModel.DateW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (issueModel.DateX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (issueModel.DateY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (issueModel.DateZ_Updated(context: context, column: column))
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
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return Error.Types.InvalidRequest;
            }
            if (!context.CanUpdate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: issueModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (issueModel.Title_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (issueModel.Body_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "StartTime":
                        if (issueModel.StartTime_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CompletionTime":
                        if (issueModel.CompletionTime_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "WorkValue":
                        if (issueModel.WorkValue_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ProgressRate":
                        if (issueModel.ProgressRate_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Status":
                        if (issueModel.Status_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Manager":
                        if (issueModel.Manager_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Owner":
                        if (issueModel.Owner_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (issueModel.ClassA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (issueModel.ClassB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (issueModel.ClassC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (issueModel.ClassD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (issueModel.ClassE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (issueModel.ClassF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (issueModel.ClassG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (issueModel.ClassH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (issueModel.ClassI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (issueModel.ClassJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (issueModel.ClassK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (issueModel.ClassL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (issueModel.ClassM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (issueModel.ClassN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (issueModel.ClassO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (issueModel.ClassP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (issueModel.ClassQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (issueModel.ClassR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (issueModel.ClassS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (issueModel.ClassT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (issueModel.ClassU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (issueModel.ClassV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (issueModel.ClassW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (issueModel.ClassX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (issueModel.ClassY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (issueModel.ClassZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (issueModel.NumA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (issueModel.NumB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (issueModel.NumC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (issueModel.NumD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (issueModel.NumE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (issueModel.NumF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (issueModel.NumG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (issueModel.NumH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (issueModel.NumI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (issueModel.NumJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (issueModel.NumK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (issueModel.NumL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (issueModel.NumM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (issueModel.NumN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (issueModel.NumO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (issueModel.NumP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (issueModel.NumQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (issueModel.NumR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (issueModel.NumS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (issueModel.NumT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (issueModel.NumU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (issueModel.NumV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (issueModel.NumW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (issueModel.NumX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (issueModel.NumY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (issueModel.NumZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (issueModel.DateA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (issueModel.DateB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (issueModel.DateC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (issueModel.DateD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (issueModel.DateE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (issueModel.DateF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (issueModel.DateG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (issueModel.DateH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (issueModel.DateI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (issueModel.DateJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (issueModel.DateK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (issueModel.DateL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (issueModel.DateM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (issueModel.DateN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (issueModel.DateO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (issueModel.DateP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (issueModel.DateQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (issueModel.DateR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (issueModel.DateS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (issueModel.DateT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (issueModel.DateU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (issueModel.DateV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (issueModel.DateW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (issueModel.DateX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (issueModel.DateY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (issueModel.DateZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (issueModel.DescriptionA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (issueModel.DescriptionB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (issueModel.DescriptionC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (issueModel.DescriptionD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (issueModel.DescriptionE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (issueModel.DescriptionF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (issueModel.DescriptionG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (issueModel.DescriptionH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (issueModel.DescriptionI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (issueModel.DescriptionJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (issueModel.DescriptionK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (issueModel.DescriptionL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (issueModel.DescriptionM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (issueModel.DescriptionN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (issueModel.DescriptionO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (issueModel.DescriptionP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (issueModel.DescriptionQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (issueModel.DescriptionR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (issueModel.DescriptionS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (issueModel.DescriptionT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (issueModel.DescriptionU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (issueModel.DescriptionV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (issueModel.DescriptionW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (issueModel.DescriptionX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (issueModel.DescriptionY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (issueModel.DescriptionZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (issueModel.CheckA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (issueModel.CheckB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (issueModel.CheckC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (issueModel.CheckD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (issueModel.CheckE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (issueModel.CheckF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (issueModel.CheckG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (issueModel.CheckH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (issueModel.CheckI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (issueModel.CheckJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (issueModel.CheckK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (issueModel.CheckL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (issueModel.CheckM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (issueModel.CheckN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (issueModel.CheckO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (issueModel.CheckP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (issueModel.CheckQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (issueModel.CheckR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (issueModel.CheckS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (issueModel.CheckT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (issueModel.CheckU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (issueModel.CheckV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (issueModel.CheckW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (issueModel.CheckX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (issueModel.CheckY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (issueModel.CheckZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsA":
                        if (issueModel.AttachmentsA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsB":
                        if (issueModel.AttachmentsB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsC":
                        if (issueModel.AttachmentsC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsD":
                        if (issueModel.AttachmentsD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsE":
                        if (issueModel.AttachmentsE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsF":
                        if (issueModel.AttachmentsF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsG":
                        if (issueModel.AttachmentsG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsH":
                        if (issueModel.AttachmentsH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsI":
                        if (issueModel.AttachmentsI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsJ":
                        if (issueModel.AttachmentsJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsK":
                        if (issueModel.AttachmentsK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsL":
                        if (issueModel.AttachmentsL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsM":
                        if (issueModel.AttachmentsM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsN":
                        if (issueModel.AttachmentsN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsO":
                        if (issueModel.AttachmentsO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsP":
                        if (issueModel.AttachmentsP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsQ":
                        if (issueModel.AttachmentsQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsR":
                        if (issueModel.AttachmentsR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsS":
                        if (issueModel.AttachmentsS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsT":
                        if (issueModel.AttachmentsT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsU":
                        if (issueModel.AttachmentsU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsV":
                        if (issueModel.AttachmentsV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsW":
                        if (issueModel.AttachmentsW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsX":
                        if (issueModel.AttachmentsX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsY":
                        if (issueModel.AttachmentsY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsZ":
                        if (issueModel.AttachmentsZ_Updated(context: context))
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

        public static Error.Types OnMoving(
            Context context, SiteSettings source, SiteSettings destination)
        {
            if (!Permissions.CanMove(context: context, source: source, destination: destination))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
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
            return context.CanExport(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }
    }
}
