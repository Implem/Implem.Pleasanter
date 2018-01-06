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
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Issues_Title":
                        if (!ss.GetColumn("Title").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Body":
                        if (!ss.GetColumn("Body").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_StartTime":
                        if (!ss.GetColumn("StartTime").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CompletionTime":
                        if (!ss.GetColumn("CompletionTime").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_WorkValue":
                        if (!ss.GetColumn("WorkValue").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ProgressRate":
                        if (!ss.GetColumn("ProgressRate").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Status":
                        if (!ss.GetColumn("Status").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Manager":
                        if (!ss.GetColumn("Manager").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Owner":
                        if (!ss.GetColumn("Owner").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassA":
                        if (!ss.GetColumn("ClassA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassB":
                        if (!ss.GetColumn("ClassB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassC":
                        if (!ss.GetColumn("ClassC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassD":
                        if (!ss.GetColumn("ClassD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassE":
                        if (!ss.GetColumn("ClassE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassF":
                        if (!ss.GetColumn("ClassF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassG":
                        if (!ss.GetColumn("ClassG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassH":
                        if (!ss.GetColumn("ClassH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassI":
                        if (!ss.GetColumn("ClassI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassJ":
                        if (!ss.GetColumn("ClassJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassK":
                        if (!ss.GetColumn("ClassK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassL":
                        if (!ss.GetColumn("ClassL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassM":
                        if (!ss.GetColumn("ClassM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassN":
                        if (!ss.GetColumn("ClassN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassO":
                        if (!ss.GetColumn("ClassO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassP":
                        if (!ss.GetColumn("ClassP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassQ":
                        if (!ss.GetColumn("ClassQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassR":
                        if (!ss.GetColumn("ClassR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassS":
                        if (!ss.GetColumn("ClassS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassT":
                        if (!ss.GetColumn("ClassT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassU":
                        if (!ss.GetColumn("ClassU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassV":
                        if (!ss.GetColumn("ClassV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassW":
                        if (!ss.GetColumn("ClassW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassX":
                        if (!ss.GetColumn("ClassX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassY":
                        if (!ss.GetColumn("ClassY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassZ":
                        if (!ss.GetColumn("ClassZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumA":
                        if (!ss.GetColumn("NumA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumB":
                        if (!ss.GetColumn("NumB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumC":
                        if (!ss.GetColumn("NumC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumD":
                        if (!ss.GetColumn("NumD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumE":
                        if (!ss.GetColumn("NumE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumF":
                        if (!ss.GetColumn("NumF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumG":
                        if (!ss.GetColumn("NumG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumH":
                        if (!ss.GetColumn("NumH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumI":
                        if (!ss.GetColumn("NumI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumJ":
                        if (!ss.GetColumn("NumJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumK":
                        if (!ss.GetColumn("NumK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumL":
                        if (!ss.GetColumn("NumL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumM":
                        if (!ss.GetColumn("NumM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumN":
                        if (!ss.GetColumn("NumN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumO":
                        if (!ss.GetColumn("NumO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumP":
                        if (!ss.GetColumn("NumP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumQ":
                        if (!ss.GetColumn("NumQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumR":
                        if (!ss.GetColumn("NumR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumS":
                        if (!ss.GetColumn("NumS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumT":
                        if (!ss.GetColumn("NumT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumU":
                        if (!ss.GetColumn("NumU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumV":
                        if (!ss.GetColumn("NumV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumW":
                        if (!ss.GetColumn("NumW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumX":
                        if (!ss.GetColumn("NumX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumY":
                        if (!ss.GetColumn("NumY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumZ":
                        if (!ss.GetColumn("NumZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateA":
                        if (!ss.GetColumn("DateA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateB":
                        if (!ss.GetColumn("DateB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateC":
                        if (!ss.GetColumn("DateC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateD":
                        if (!ss.GetColumn("DateD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateE":
                        if (!ss.GetColumn("DateE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateF":
                        if (!ss.GetColumn("DateF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateG":
                        if (!ss.GetColumn("DateG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateH":
                        if (!ss.GetColumn("DateH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateI":
                        if (!ss.GetColumn("DateI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateJ":
                        if (!ss.GetColumn("DateJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateK":
                        if (!ss.GetColumn("DateK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateL":
                        if (!ss.GetColumn("DateL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateM":
                        if (!ss.GetColumn("DateM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateN":
                        if (!ss.GetColumn("DateN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateO":
                        if (!ss.GetColumn("DateO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateP":
                        if (!ss.GetColumn("DateP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateQ":
                        if (!ss.GetColumn("DateQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateR":
                        if (!ss.GetColumn("DateR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateS":
                        if (!ss.GetColumn("DateS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateT":
                        if (!ss.GetColumn("DateT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateU":
                        if (!ss.GetColumn("DateU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateV":
                        if (!ss.GetColumn("DateV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateW":
                        if (!ss.GetColumn("DateW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateX":
                        if (!ss.GetColumn("DateX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateY":
                        if (!ss.GetColumn("DateY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateZ":
                        if (!ss.GetColumn("DateZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionA":
                        if (!ss.GetColumn("DescriptionA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionB":
                        if (!ss.GetColumn("DescriptionB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionC":
                        if (!ss.GetColumn("DescriptionC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionD":
                        if (!ss.GetColumn("DescriptionD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionE":
                        if (!ss.GetColumn("DescriptionE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionF":
                        if (!ss.GetColumn("DescriptionF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionG":
                        if (!ss.GetColumn("DescriptionG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionH":
                        if (!ss.GetColumn("DescriptionH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionI":
                        if (!ss.GetColumn("DescriptionI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionJ":
                        if (!ss.GetColumn("DescriptionJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionK":
                        if (!ss.GetColumn("DescriptionK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionL":
                        if (!ss.GetColumn("DescriptionL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionM":
                        if (!ss.GetColumn("DescriptionM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionN":
                        if (!ss.GetColumn("DescriptionN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionO":
                        if (!ss.GetColumn("DescriptionO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionP":
                        if (!ss.GetColumn("DescriptionP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionQ":
                        if (!ss.GetColumn("DescriptionQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionR":
                        if (!ss.GetColumn("DescriptionR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionS":
                        if (!ss.GetColumn("DescriptionS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionT":
                        if (!ss.GetColumn("DescriptionT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionU":
                        if (!ss.GetColumn("DescriptionU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionV":
                        if (!ss.GetColumn("DescriptionV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionW":
                        if (!ss.GetColumn("DescriptionW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionX":
                        if (!ss.GetColumn("DescriptionX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionY":
                        if (!ss.GetColumn("DescriptionY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionZ":
                        if (!ss.GetColumn("DescriptionZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckA":
                        if (!ss.GetColumn("CheckA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckB":
                        if (!ss.GetColumn("CheckB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckC":
                        if (!ss.GetColumn("CheckC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckD":
                        if (!ss.GetColumn("CheckD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckE":
                        if (!ss.GetColumn("CheckE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckF":
                        if (!ss.GetColumn("CheckF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckG":
                        if (!ss.GetColumn("CheckG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckH":
                        if (!ss.GetColumn("CheckH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckI":
                        if (!ss.GetColumn("CheckI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckJ":
                        if (!ss.GetColumn("CheckJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckK":
                        if (!ss.GetColumn("CheckK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckL":
                        if (!ss.GetColumn("CheckL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckM":
                        if (!ss.GetColumn("CheckM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckN":
                        if (!ss.GetColumn("CheckN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckO":
                        if (!ss.GetColumn("CheckO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckP":
                        if (!ss.GetColumn("CheckP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckQ":
                        if (!ss.GetColumn("CheckQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckR":
                        if (!ss.GetColumn("CheckR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckS":
                        if (!ss.GetColumn("CheckS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckT":
                        if (!ss.GetColumn("CheckT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckU":
                        if (!ss.GetColumn("CheckU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckV":
                        if (!ss.GetColumn("CheckV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckW":
                        if (!ss.GetColumn("CheckW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckX":
                        if (!ss.GetColumn("CheckX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckY":
                        if (!ss.GetColumn("CheckY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckZ":
                        if (!ss.GetColumn("CheckZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsA":
                        if (!ss.GetColumn("AttachmentsA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsB":
                        if (!ss.GetColumn("AttachmentsB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsC":
                        if (!ss.GetColumn("AttachmentsC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsD":
                        if (!ss.GetColumn("AttachmentsD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsE":
                        if (!ss.GetColumn("AttachmentsE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsF":
                        if (!ss.GetColumn("AttachmentsF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsG":
                        if (!ss.GetColumn("AttachmentsG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsH":
                        if (!ss.GetColumn("AttachmentsH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsI":
                        if (!ss.GetColumn("AttachmentsI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsJ":
                        if (!ss.GetColumn("AttachmentsJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsK":
                        if (!ss.GetColumn("AttachmentsK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsL":
                        if (!ss.GetColumn("AttachmentsL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsM":
                        if (!ss.GetColumn("AttachmentsM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsN":
                        if (!ss.GetColumn("AttachmentsN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsO":
                        if (!ss.GetColumn("AttachmentsO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsP":
                        if (!ss.GetColumn("AttachmentsP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsQ":
                        if (!ss.GetColumn("AttachmentsQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsR":
                        if (!ss.GetColumn("AttachmentsR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsS":
                        if (!ss.GetColumn("AttachmentsS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsT":
                        if (!ss.GetColumn("AttachmentsT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsU":
                        if (!ss.GetColumn("AttachmentsU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsV":
                        if (!ss.GetColumn("AttachmentsV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsW":
                        if (!ss.GetColumn("AttachmentsW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsX":
                        if (!ss.GetColumn("AttachmentsX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsY":
                        if (!ss.GetColumn("AttachmentsY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentsZ":
                        if (!ss.GetColumn("AttachmentsZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
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
            foreach (var column in ss.Columns.Where(o => !o.CanUpdate))
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
