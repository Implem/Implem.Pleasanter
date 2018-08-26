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
        public static Error.Types OnEntry(Context context, SiteSettings ss)
        {
            return context.HasPermission(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(Context context, SiteSettings ss)
        {
            return context.CanRead(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(
            Context context, SiteSettings ss, ResultModel resultModel)
        {
            switch (resultModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)&&
                        resultModel.AccessStatus != Databases.AccessStatuses.NotFound
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
            Context context, SiteSettings ss, ResultModel resultModel)
        {
            if (!context.CanCreate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: resultModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (resultModel.Title_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (resultModel.Body_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Status":
                        if (resultModel.Status_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Manager":
                        if (resultModel.Manager_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Owner":
                        if (resultModel.Owner_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (resultModel.ClassA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (resultModel.ClassB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (resultModel.ClassC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (resultModel.ClassD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (resultModel.ClassE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (resultModel.ClassF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (resultModel.ClassG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (resultModel.ClassH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (resultModel.ClassI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (resultModel.ClassJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (resultModel.ClassK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (resultModel.ClassL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (resultModel.ClassM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (resultModel.ClassN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (resultModel.ClassO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (resultModel.ClassP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (resultModel.ClassQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (resultModel.ClassR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (resultModel.ClassS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (resultModel.ClassT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (resultModel.ClassU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (resultModel.ClassV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (resultModel.ClassW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (resultModel.ClassX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (resultModel.ClassY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (resultModel.ClassZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (resultModel.NumA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (resultModel.NumB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (resultModel.NumC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (resultModel.NumD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (resultModel.NumE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (resultModel.NumF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (resultModel.NumG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (resultModel.NumH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (resultModel.NumI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (resultModel.NumJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (resultModel.NumK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (resultModel.NumL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (resultModel.NumM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (resultModel.NumN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (resultModel.NumO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (resultModel.NumP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (resultModel.NumQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (resultModel.NumR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (resultModel.NumS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (resultModel.NumT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (resultModel.NumU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (resultModel.NumV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (resultModel.NumW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (resultModel.NumX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (resultModel.NumY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (resultModel.NumZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (resultModel.DescriptionA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (resultModel.DescriptionB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (resultModel.DescriptionC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (resultModel.DescriptionD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (resultModel.DescriptionE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (resultModel.DescriptionF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (resultModel.DescriptionG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (resultModel.DescriptionH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (resultModel.DescriptionI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (resultModel.DescriptionJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (resultModel.DescriptionK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (resultModel.DescriptionL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (resultModel.DescriptionM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (resultModel.DescriptionN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (resultModel.DescriptionO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (resultModel.DescriptionP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (resultModel.DescriptionQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (resultModel.DescriptionR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (resultModel.DescriptionS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (resultModel.DescriptionT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (resultModel.DescriptionU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (resultModel.DescriptionV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (resultModel.DescriptionW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (resultModel.DescriptionX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (resultModel.DescriptionY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (resultModel.DescriptionZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (resultModel.CheckA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (resultModel.CheckB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (resultModel.CheckC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (resultModel.CheckD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (resultModel.CheckE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (resultModel.CheckF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (resultModel.CheckG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (resultModel.CheckH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (resultModel.CheckI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (resultModel.CheckJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (resultModel.CheckK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (resultModel.CheckL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (resultModel.CheckM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (resultModel.CheckN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (resultModel.CheckO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (resultModel.CheckP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (resultModel.CheckQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (resultModel.CheckR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (resultModel.CheckS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (resultModel.CheckT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (resultModel.CheckU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (resultModel.CheckV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (resultModel.CheckW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (resultModel.CheckX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (resultModel.CheckY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (resultModel.CheckZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsA":
                        if (resultModel.AttachmentsA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsB":
                        if (resultModel.AttachmentsB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsC":
                        if (resultModel.AttachmentsC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsD":
                        if (resultModel.AttachmentsD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsE":
                        if (resultModel.AttachmentsE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsF":
                        if (resultModel.AttachmentsF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsG":
                        if (resultModel.AttachmentsG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsH":
                        if (resultModel.AttachmentsH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsI":
                        if (resultModel.AttachmentsI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsJ":
                        if (resultModel.AttachmentsJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsK":
                        if (resultModel.AttachmentsK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsL":
                        if (resultModel.AttachmentsL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsM":
                        if (resultModel.AttachmentsM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsN":
                        if (resultModel.AttachmentsN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsO":
                        if (resultModel.AttachmentsO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsP":
                        if (resultModel.AttachmentsP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsQ":
                        if (resultModel.AttachmentsQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsR":
                        if (resultModel.AttachmentsR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsS":
                        if (resultModel.AttachmentsS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsT":
                        if (resultModel.AttachmentsT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsU":
                        if (resultModel.AttachmentsU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsV":
                        if (resultModel.AttachmentsV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsW":
                        if (resultModel.AttachmentsW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsX":
                        if (resultModel.AttachmentsX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsY":
                        if (resultModel.AttachmentsY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsZ":
                        if (resultModel.AttachmentsZ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (resultModel.DateA_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (resultModel.DateB_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (resultModel.DateC_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (resultModel.DateD_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (resultModel.DateE_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (resultModel.DateF_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (resultModel.DateG_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (resultModel.DateH_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (resultModel.DateI_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (resultModel.DateJ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (resultModel.DateK_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (resultModel.DateL_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (resultModel.DateM_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (resultModel.DateN_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (resultModel.DateO_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (resultModel.DateP_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (resultModel.DateQ_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (resultModel.DateR_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (resultModel.DateS_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (resultModel.DateT_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (resultModel.DateU_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (resultModel.DateV_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (resultModel.DateW_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (resultModel.DateX_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (resultModel.DateY_Updated(context: context, column: column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (resultModel.DateZ_Updated(context: context, column: column))
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
            Context context, SiteSettings ss, ResultModel resultModel)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: resultModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (resultModel.Title_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (resultModel.Body_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Status":
                        if (resultModel.Status_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Manager":
                        if (resultModel.Manager_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Owner":
                        if (resultModel.Owner_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassA":
                        if (resultModel.ClassA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassB":
                        if (resultModel.ClassB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassC":
                        if (resultModel.ClassC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassD":
                        if (resultModel.ClassD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassE":
                        if (resultModel.ClassE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassF":
                        if (resultModel.ClassF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassG":
                        if (resultModel.ClassG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassH":
                        if (resultModel.ClassH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassI":
                        if (resultModel.ClassI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassJ":
                        if (resultModel.ClassJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassK":
                        if (resultModel.ClassK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassL":
                        if (resultModel.ClassL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassM":
                        if (resultModel.ClassM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassN":
                        if (resultModel.ClassN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassO":
                        if (resultModel.ClassO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassP":
                        if (resultModel.ClassP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassQ":
                        if (resultModel.ClassQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassR":
                        if (resultModel.ClassR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassS":
                        if (resultModel.ClassS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassT":
                        if (resultModel.ClassT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassU":
                        if (resultModel.ClassU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassV":
                        if (resultModel.ClassV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassW":
                        if (resultModel.ClassW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassX":
                        if (resultModel.ClassX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassY":
                        if (resultModel.ClassY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ClassZ":
                        if (resultModel.ClassZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumA":
                        if (resultModel.NumA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumB":
                        if (resultModel.NumB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumC":
                        if (resultModel.NumC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumD":
                        if (resultModel.NumD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumE":
                        if (resultModel.NumE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumF":
                        if (resultModel.NumF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumG":
                        if (resultModel.NumG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumH":
                        if (resultModel.NumH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumI":
                        if (resultModel.NumI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumJ":
                        if (resultModel.NumJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumK":
                        if (resultModel.NumK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumL":
                        if (resultModel.NumL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumM":
                        if (resultModel.NumM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumN":
                        if (resultModel.NumN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumO":
                        if (resultModel.NumO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumP":
                        if (resultModel.NumP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumQ":
                        if (resultModel.NumQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumR":
                        if (resultModel.NumR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumS":
                        if (resultModel.NumS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumT":
                        if (resultModel.NumT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumU":
                        if (resultModel.NumU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumV":
                        if (resultModel.NumV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumW":
                        if (resultModel.NumW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumX":
                        if (resultModel.NumX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumY":
                        if (resultModel.NumY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumZ":
                        if (resultModel.NumZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateA":
                        if (resultModel.DateA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateB":
                        if (resultModel.DateB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateC":
                        if (resultModel.DateC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateD":
                        if (resultModel.DateD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateE":
                        if (resultModel.DateE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateF":
                        if (resultModel.DateF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateG":
                        if (resultModel.DateG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateH":
                        if (resultModel.DateH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateI":
                        if (resultModel.DateI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateJ":
                        if (resultModel.DateJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateK":
                        if (resultModel.DateK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateL":
                        if (resultModel.DateL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateM":
                        if (resultModel.DateM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateN":
                        if (resultModel.DateN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateO":
                        if (resultModel.DateO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateP":
                        if (resultModel.DateP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateQ":
                        if (resultModel.DateQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateR":
                        if (resultModel.DateR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateS":
                        if (resultModel.DateS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateT":
                        if (resultModel.DateT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateU":
                        if (resultModel.DateU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateV":
                        if (resultModel.DateV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateW":
                        if (resultModel.DateW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateX":
                        if (resultModel.DateX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateY":
                        if (resultModel.DateY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DateZ":
                        if (resultModel.DateZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionA":
                        if (resultModel.DescriptionA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionB":
                        if (resultModel.DescriptionB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionC":
                        if (resultModel.DescriptionC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionD":
                        if (resultModel.DescriptionD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionE":
                        if (resultModel.DescriptionE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionF":
                        if (resultModel.DescriptionF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionG":
                        if (resultModel.DescriptionG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionH":
                        if (resultModel.DescriptionH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionI":
                        if (resultModel.DescriptionI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionJ":
                        if (resultModel.DescriptionJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionK":
                        if (resultModel.DescriptionK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionL":
                        if (resultModel.DescriptionL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionM":
                        if (resultModel.DescriptionM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionN":
                        if (resultModel.DescriptionN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionO":
                        if (resultModel.DescriptionO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionP":
                        if (resultModel.DescriptionP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionQ":
                        if (resultModel.DescriptionQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionR":
                        if (resultModel.DescriptionR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionS":
                        if (resultModel.DescriptionS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionT":
                        if (resultModel.DescriptionT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionU":
                        if (resultModel.DescriptionU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionV":
                        if (resultModel.DescriptionV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionW":
                        if (resultModel.DescriptionW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionX":
                        if (resultModel.DescriptionX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionY":
                        if (resultModel.DescriptionY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DescriptionZ":
                        if (resultModel.DescriptionZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckA":
                        if (resultModel.CheckA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckB":
                        if (resultModel.CheckB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckC":
                        if (resultModel.CheckC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckD":
                        if (resultModel.CheckD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckE":
                        if (resultModel.CheckE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckF":
                        if (resultModel.CheckF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckG":
                        if (resultModel.CheckG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckH":
                        if (resultModel.CheckH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckI":
                        if (resultModel.CheckI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckJ":
                        if (resultModel.CheckJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckK":
                        if (resultModel.CheckK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckL":
                        if (resultModel.CheckL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckM":
                        if (resultModel.CheckM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckN":
                        if (resultModel.CheckN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckO":
                        if (resultModel.CheckO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckP":
                        if (resultModel.CheckP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckQ":
                        if (resultModel.CheckQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckR":
                        if (resultModel.CheckR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckS":
                        if (resultModel.CheckS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckT":
                        if (resultModel.CheckT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckU":
                        if (resultModel.CheckU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckV":
                        if (resultModel.CheckV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckW":
                        if (resultModel.CheckW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckX":
                        if (resultModel.CheckX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckY":
                        if (resultModel.CheckY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "CheckZ":
                        if (resultModel.CheckZ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsA":
                        if (resultModel.AttachmentsA_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsB":
                        if (resultModel.AttachmentsB_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsC":
                        if (resultModel.AttachmentsC_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsD":
                        if (resultModel.AttachmentsD_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsE":
                        if (resultModel.AttachmentsE_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsF":
                        if (resultModel.AttachmentsF_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsG":
                        if (resultModel.AttachmentsG_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsH":
                        if (resultModel.AttachmentsH_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsI":
                        if (resultModel.AttachmentsI_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsJ":
                        if (resultModel.AttachmentsJ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsK":
                        if (resultModel.AttachmentsK_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsL":
                        if (resultModel.AttachmentsL_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsM":
                        if (resultModel.AttachmentsM_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsN":
                        if (resultModel.AttachmentsN_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsO":
                        if (resultModel.AttachmentsO_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsP":
                        if (resultModel.AttachmentsP_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsQ":
                        if (resultModel.AttachmentsQ_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsR":
                        if (resultModel.AttachmentsR_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsS":
                        if (resultModel.AttachmentsS_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsT":
                        if (resultModel.AttachmentsT_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsU":
                        if (resultModel.AttachmentsU_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsV":
                        if (resultModel.AttachmentsV_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsW":
                        if (resultModel.AttachmentsW_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsX":
                        if (resultModel.AttachmentsX_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsY":
                        if (resultModel.AttachmentsY_Updated(context: context))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "AttachmentsZ":
                        if (resultModel.AttachmentsZ_Updated(context: context))
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
            Context context, SiteSettings ss, ResultModel resultModel)
        {
            return context.CanDelete(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnRestoring(Context context)
        {
            return Permissions.CanManageTenant(context: context)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(Context context, SiteSettings ss)
        {
            return context.CanExport(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }
    }
}
