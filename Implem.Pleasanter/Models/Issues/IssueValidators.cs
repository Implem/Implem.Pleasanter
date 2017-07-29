using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
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
                    case "Issues_AttachmentA":
                        if (!ss.GetColumn("AttachmentA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentB":
                        if (!ss.GetColumn("AttachmentB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentC":
                        if (!ss.GetColumn("AttachmentC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentD":
                        if (!ss.GetColumn("AttachmentD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentE":
                        if (!ss.GetColumn("AttachmentE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentF":
                        if (!ss.GetColumn("AttachmentF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentG":
                        if (!ss.GetColumn("AttachmentG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentH":
                        if (!ss.GetColumn("AttachmentH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentI":
                        if (!ss.GetColumn("AttachmentI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentJ":
                        if (!ss.GetColumn("AttachmentJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentK":
                        if (!ss.GetColumn("AttachmentK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentL":
                        if (!ss.GetColumn("AttachmentL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentM":
                        if (!ss.GetColumn("AttachmentM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentN":
                        if (!ss.GetColumn("AttachmentN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentO":
                        if (!ss.GetColumn("AttachmentO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentP":
                        if (!ss.GetColumn("AttachmentP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentQ":
                        if (!ss.GetColumn("AttachmentQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentR":
                        if (!ss.GetColumn("AttachmentR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentS":
                        if (!ss.GetColumn("AttachmentS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentT":
                        if (!ss.GetColumn("AttachmentT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentU":
                        if (!ss.GetColumn("AttachmentU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentV":
                        if (!ss.GetColumn("AttachmentV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentW":
                        if (!ss.GetColumn("AttachmentW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentX":
                        if (!ss.GetColumn("AttachmentX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentY":
                        if (!ss.GetColumn("AttachmentY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentZ":
                        if (!ss.GetColumn("AttachmentZ").CanCreate)
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
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Issues_Title":
                        if (issueModel.Title_Updated &&
                            !ss.GetColumn("Title").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Body":
                        if (issueModel.Body_Updated &&
                            !ss.GetColumn("Body").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_StartTime":
                        if (issueModel.StartTime_Updated &&
                            !ss.GetColumn("StartTime").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CompletionTime":
                        if (issueModel.CompletionTime_Updated &&
                            !ss.GetColumn("CompletionTime").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_WorkValue":
                        if (issueModel.WorkValue_Updated &&
                            !ss.GetColumn("WorkValue").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ProgressRate":
                        if (issueModel.ProgressRate_Updated &&
                            !ss.GetColumn("ProgressRate").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Status":
                        if (issueModel.Status_Updated &&
                            !ss.GetColumn("Status").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Manager":
                        if (issueModel.Manager_Updated &&
                            !ss.GetColumn("Manager").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Owner":
                        if (issueModel.Owner_Updated &&
                            !ss.GetColumn("Owner").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassA":
                        if (issueModel.ClassA_Updated &&
                            !ss.GetColumn("ClassA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassB":
                        if (issueModel.ClassB_Updated &&
                            !ss.GetColumn("ClassB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassC":
                        if (issueModel.ClassC_Updated &&
                            !ss.GetColumn("ClassC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassD":
                        if (issueModel.ClassD_Updated &&
                            !ss.GetColumn("ClassD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassE":
                        if (issueModel.ClassE_Updated &&
                            !ss.GetColumn("ClassE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassF":
                        if (issueModel.ClassF_Updated &&
                            !ss.GetColumn("ClassF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassG":
                        if (issueModel.ClassG_Updated &&
                            !ss.GetColumn("ClassG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassH":
                        if (issueModel.ClassH_Updated &&
                            !ss.GetColumn("ClassH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassI":
                        if (issueModel.ClassI_Updated &&
                            !ss.GetColumn("ClassI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassJ":
                        if (issueModel.ClassJ_Updated &&
                            !ss.GetColumn("ClassJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassK":
                        if (issueModel.ClassK_Updated &&
                            !ss.GetColumn("ClassK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassL":
                        if (issueModel.ClassL_Updated &&
                            !ss.GetColumn("ClassL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassM":
                        if (issueModel.ClassM_Updated &&
                            !ss.GetColumn("ClassM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassN":
                        if (issueModel.ClassN_Updated &&
                            !ss.GetColumn("ClassN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassO":
                        if (issueModel.ClassO_Updated &&
                            !ss.GetColumn("ClassO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassP":
                        if (issueModel.ClassP_Updated &&
                            !ss.GetColumn("ClassP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassQ":
                        if (issueModel.ClassQ_Updated &&
                            !ss.GetColumn("ClassQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassR":
                        if (issueModel.ClassR_Updated &&
                            !ss.GetColumn("ClassR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassS":
                        if (issueModel.ClassS_Updated &&
                            !ss.GetColumn("ClassS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassT":
                        if (issueModel.ClassT_Updated &&
                            !ss.GetColumn("ClassT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassU":
                        if (issueModel.ClassU_Updated &&
                            !ss.GetColumn("ClassU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassV":
                        if (issueModel.ClassV_Updated &&
                            !ss.GetColumn("ClassV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassW":
                        if (issueModel.ClassW_Updated &&
                            !ss.GetColumn("ClassW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassX":
                        if (issueModel.ClassX_Updated &&
                            !ss.GetColumn("ClassX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassY":
                        if (issueModel.ClassY_Updated &&
                            !ss.GetColumn("ClassY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassZ":
                        if (issueModel.ClassZ_Updated &&
                            !ss.GetColumn("ClassZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumA":
                        if (issueModel.NumA_Updated &&
                            !ss.GetColumn("NumA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumB":
                        if (issueModel.NumB_Updated &&
                            !ss.GetColumn("NumB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumC":
                        if (issueModel.NumC_Updated &&
                            !ss.GetColumn("NumC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumD":
                        if (issueModel.NumD_Updated &&
                            !ss.GetColumn("NumD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumE":
                        if (issueModel.NumE_Updated &&
                            !ss.GetColumn("NumE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumF":
                        if (issueModel.NumF_Updated &&
                            !ss.GetColumn("NumF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumG":
                        if (issueModel.NumG_Updated &&
                            !ss.GetColumn("NumG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumH":
                        if (issueModel.NumH_Updated &&
                            !ss.GetColumn("NumH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumI":
                        if (issueModel.NumI_Updated &&
                            !ss.GetColumn("NumI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumJ":
                        if (issueModel.NumJ_Updated &&
                            !ss.GetColumn("NumJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumK":
                        if (issueModel.NumK_Updated &&
                            !ss.GetColumn("NumK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumL":
                        if (issueModel.NumL_Updated &&
                            !ss.GetColumn("NumL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumM":
                        if (issueModel.NumM_Updated &&
                            !ss.GetColumn("NumM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumN":
                        if (issueModel.NumN_Updated &&
                            !ss.GetColumn("NumN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumO":
                        if (issueModel.NumO_Updated &&
                            !ss.GetColumn("NumO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumP":
                        if (issueModel.NumP_Updated &&
                            !ss.GetColumn("NumP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumQ":
                        if (issueModel.NumQ_Updated &&
                            !ss.GetColumn("NumQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumR":
                        if (issueModel.NumR_Updated &&
                            !ss.GetColumn("NumR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumS":
                        if (issueModel.NumS_Updated &&
                            !ss.GetColumn("NumS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumT":
                        if (issueModel.NumT_Updated &&
                            !ss.GetColumn("NumT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumU":
                        if (issueModel.NumU_Updated &&
                            !ss.GetColumn("NumU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumV":
                        if (issueModel.NumV_Updated &&
                            !ss.GetColumn("NumV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumW":
                        if (issueModel.NumW_Updated &&
                            !ss.GetColumn("NumW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumX":
                        if (issueModel.NumX_Updated &&
                            !ss.GetColumn("NumX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumY":
                        if (issueModel.NumY_Updated &&
                            !ss.GetColumn("NumY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumZ":
                        if (issueModel.NumZ_Updated &&
                            !ss.GetColumn("NumZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateA":
                        if (issueModel.DateA_Updated &&
                            !ss.GetColumn("DateA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateB":
                        if (issueModel.DateB_Updated &&
                            !ss.GetColumn("DateB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateC":
                        if (issueModel.DateC_Updated &&
                            !ss.GetColumn("DateC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateD":
                        if (issueModel.DateD_Updated &&
                            !ss.GetColumn("DateD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateE":
                        if (issueModel.DateE_Updated &&
                            !ss.GetColumn("DateE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateF":
                        if (issueModel.DateF_Updated &&
                            !ss.GetColumn("DateF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateG":
                        if (issueModel.DateG_Updated &&
                            !ss.GetColumn("DateG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateH":
                        if (issueModel.DateH_Updated &&
                            !ss.GetColumn("DateH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateI":
                        if (issueModel.DateI_Updated &&
                            !ss.GetColumn("DateI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateJ":
                        if (issueModel.DateJ_Updated &&
                            !ss.GetColumn("DateJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateK":
                        if (issueModel.DateK_Updated &&
                            !ss.GetColumn("DateK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateL":
                        if (issueModel.DateL_Updated &&
                            !ss.GetColumn("DateL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateM":
                        if (issueModel.DateM_Updated &&
                            !ss.GetColumn("DateM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateN":
                        if (issueModel.DateN_Updated &&
                            !ss.GetColumn("DateN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateO":
                        if (issueModel.DateO_Updated &&
                            !ss.GetColumn("DateO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateP":
                        if (issueModel.DateP_Updated &&
                            !ss.GetColumn("DateP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateQ":
                        if (issueModel.DateQ_Updated &&
                            !ss.GetColumn("DateQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateR":
                        if (issueModel.DateR_Updated &&
                            !ss.GetColumn("DateR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateS":
                        if (issueModel.DateS_Updated &&
                            !ss.GetColumn("DateS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateT":
                        if (issueModel.DateT_Updated &&
                            !ss.GetColumn("DateT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateU":
                        if (issueModel.DateU_Updated &&
                            !ss.GetColumn("DateU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateV":
                        if (issueModel.DateV_Updated &&
                            !ss.GetColumn("DateV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateW":
                        if (issueModel.DateW_Updated &&
                            !ss.GetColumn("DateW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateX":
                        if (issueModel.DateX_Updated &&
                            !ss.GetColumn("DateX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateY":
                        if (issueModel.DateY_Updated &&
                            !ss.GetColumn("DateY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateZ":
                        if (issueModel.DateZ_Updated &&
                            !ss.GetColumn("DateZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionA":
                        if (issueModel.DescriptionA_Updated &&
                            !ss.GetColumn("DescriptionA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionB":
                        if (issueModel.DescriptionB_Updated &&
                            !ss.GetColumn("DescriptionB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionC":
                        if (issueModel.DescriptionC_Updated &&
                            !ss.GetColumn("DescriptionC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionD":
                        if (issueModel.DescriptionD_Updated &&
                            !ss.GetColumn("DescriptionD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionE":
                        if (issueModel.DescriptionE_Updated &&
                            !ss.GetColumn("DescriptionE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionF":
                        if (issueModel.DescriptionF_Updated &&
                            !ss.GetColumn("DescriptionF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionG":
                        if (issueModel.DescriptionG_Updated &&
                            !ss.GetColumn("DescriptionG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionH":
                        if (issueModel.DescriptionH_Updated &&
                            !ss.GetColumn("DescriptionH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionI":
                        if (issueModel.DescriptionI_Updated &&
                            !ss.GetColumn("DescriptionI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionJ":
                        if (issueModel.DescriptionJ_Updated &&
                            !ss.GetColumn("DescriptionJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionK":
                        if (issueModel.DescriptionK_Updated &&
                            !ss.GetColumn("DescriptionK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionL":
                        if (issueModel.DescriptionL_Updated &&
                            !ss.GetColumn("DescriptionL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionM":
                        if (issueModel.DescriptionM_Updated &&
                            !ss.GetColumn("DescriptionM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionN":
                        if (issueModel.DescriptionN_Updated &&
                            !ss.GetColumn("DescriptionN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionO":
                        if (issueModel.DescriptionO_Updated &&
                            !ss.GetColumn("DescriptionO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionP":
                        if (issueModel.DescriptionP_Updated &&
                            !ss.GetColumn("DescriptionP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionQ":
                        if (issueModel.DescriptionQ_Updated &&
                            !ss.GetColumn("DescriptionQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionR":
                        if (issueModel.DescriptionR_Updated &&
                            !ss.GetColumn("DescriptionR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionS":
                        if (issueModel.DescriptionS_Updated &&
                            !ss.GetColumn("DescriptionS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionT":
                        if (issueModel.DescriptionT_Updated &&
                            !ss.GetColumn("DescriptionT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionU":
                        if (issueModel.DescriptionU_Updated &&
                            !ss.GetColumn("DescriptionU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionV":
                        if (issueModel.DescriptionV_Updated &&
                            !ss.GetColumn("DescriptionV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionW":
                        if (issueModel.DescriptionW_Updated &&
                            !ss.GetColumn("DescriptionW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionX":
                        if (issueModel.DescriptionX_Updated &&
                            !ss.GetColumn("DescriptionX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionY":
                        if (issueModel.DescriptionY_Updated &&
                            !ss.GetColumn("DescriptionY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionZ":
                        if (issueModel.DescriptionZ_Updated &&
                            !ss.GetColumn("DescriptionZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckA":
                        if (issueModel.CheckA_Updated &&
                            !ss.GetColumn("CheckA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckB":
                        if (issueModel.CheckB_Updated &&
                            !ss.GetColumn("CheckB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckC":
                        if (issueModel.CheckC_Updated &&
                            !ss.GetColumn("CheckC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckD":
                        if (issueModel.CheckD_Updated &&
                            !ss.GetColumn("CheckD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckE":
                        if (issueModel.CheckE_Updated &&
                            !ss.GetColumn("CheckE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckF":
                        if (issueModel.CheckF_Updated &&
                            !ss.GetColumn("CheckF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckG":
                        if (issueModel.CheckG_Updated &&
                            !ss.GetColumn("CheckG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckH":
                        if (issueModel.CheckH_Updated &&
                            !ss.GetColumn("CheckH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckI":
                        if (issueModel.CheckI_Updated &&
                            !ss.GetColumn("CheckI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckJ":
                        if (issueModel.CheckJ_Updated &&
                            !ss.GetColumn("CheckJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckK":
                        if (issueModel.CheckK_Updated &&
                            !ss.GetColumn("CheckK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckL":
                        if (issueModel.CheckL_Updated &&
                            !ss.GetColumn("CheckL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckM":
                        if (issueModel.CheckM_Updated &&
                            !ss.GetColumn("CheckM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckN":
                        if (issueModel.CheckN_Updated &&
                            !ss.GetColumn("CheckN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckO":
                        if (issueModel.CheckO_Updated &&
                            !ss.GetColumn("CheckO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckP":
                        if (issueModel.CheckP_Updated &&
                            !ss.GetColumn("CheckP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckQ":
                        if (issueModel.CheckQ_Updated &&
                            !ss.GetColumn("CheckQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckR":
                        if (issueModel.CheckR_Updated &&
                            !ss.GetColumn("CheckR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckS":
                        if (issueModel.CheckS_Updated &&
                            !ss.GetColumn("CheckS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckT":
                        if (issueModel.CheckT_Updated &&
                            !ss.GetColumn("CheckT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckU":
                        if (issueModel.CheckU_Updated &&
                            !ss.GetColumn("CheckU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckV":
                        if (issueModel.CheckV_Updated &&
                            !ss.GetColumn("CheckV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckW":
                        if (issueModel.CheckW_Updated &&
                            !ss.GetColumn("CheckW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckX":
                        if (issueModel.CheckX_Updated &&
                            !ss.GetColumn("CheckX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckY":
                        if (issueModel.CheckY_Updated &&
                            !ss.GetColumn("CheckY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckZ":
                        if (issueModel.CheckZ_Updated &&
                            !ss.GetColumn("CheckZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentA":
                        if (issueModel.AttachmentA_Updated &&
                            !ss.GetColumn("AttachmentA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentB":
                        if (issueModel.AttachmentB_Updated &&
                            !ss.GetColumn("AttachmentB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentC":
                        if (issueModel.AttachmentC_Updated &&
                            !ss.GetColumn("AttachmentC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentD":
                        if (issueModel.AttachmentD_Updated &&
                            !ss.GetColumn("AttachmentD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentE":
                        if (issueModel.AttachmentE_Updated &&
                            !ss.GetColumn("AttachmentE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentF":
                        if (issueModel.AttachmentF_Updated &&
                            !ss.GetColumn("AttachmentF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentG":
                        if (issueModel.AttachmentG_Updated &&
                            !ss.GetColumn("AttachmentG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentH":
                        if (issueModel.AttachmentH_Updated &&
                            !ss.GetColumn("AttachmentH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentI":
                        if (issueModel.AttachmentI_Updated &&
                            !ss.GetColumn("AttachmentI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentJ":
                        if (issueModel.AttachmentJ_Updated &&
                            !ss.GetColumn("AttachmentJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentK":
                        if (issueModel.AttachmentK_Updated &&
                            !ss.GetColumn("AttachmentK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentL":
                        if (issueModel.AttachmentL_Updated &&
                            !ss.GetColumn("AttachmentL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentM":
                        if (issueModel.AttachmentM_Updated &&
                            !ss.GetColumn("AttachmentM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentN":
                        if (issueModel.AttachmentN_Updated &&
                            !ss.GetColumn("AttachmentN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentO":
                        if (issueModel.AttachmentO_Updated &&
                            !ss.GetColumn("AttachmentO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentP":
                        if (issueModel.AttachmentP_Updated &&
                            !ss.GetColumn("AttachmentP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentQ":
                        if (issueModel.AttachmentQ_Updated &&
                            !ss.GetColumn("AttachmentQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentR":
                        if (issueModel.AttachmentR_Updated &&
                            !ss.GetColumn("AttachmentR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentS":
                        if (issueModel.AttachmentS_Updated &&
                            !ss.GetColumn("AttachmentS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentT":
                        if (issueModel.AttachmentT_Updated &&
                            !ss.GetColumn("AttachmentT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentU":
                        if (issueModel.AttachmentU_Updated &&
                            !ss.GetColumn("AttachmentU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentV":
                        if (issueModel.AttachmentV_Updated &&
                            !ss.GetColumn("AttachmentV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentW":
                        if (issueModel.AttachmentW_Updated &&
                            !ss.GetColumn("AttachmentW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentX":
                        if (issueModel.AttachmentX_Updated &&
                            !ss.GetColumn("AttachmentX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentY":
                        if (issueModel.AttachmentY_Updated &&
                            !ss.GetColumn("AttachmentY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_AttachmentZ":
                        if (issueModel.AttachmentZ_Updated &&
                            !ss.GetColumn("AttachmentZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
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
