using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class IssueValidators
    {
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
                        if (!ss.GetColumn("Title").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Body":
                        if (!ss.GetColumn("Body").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_StartTime":
                        if (!ss.GetColumn("StartTime").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CompletionTime":
                        if (!ss.GetColumn("CompletionTime").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_WorkValue":
                        if (!ss.GetColumn("WorkValue").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ProgressRate":
                        if (!ss.GetColumn("ProgressRate").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Status":
                        if (!ss.GetColumn("Status").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Manager":
                        if (!ss.GetColumn("Manager").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_Owner":
                        if (!ss.GetColumn("Owner").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassA":
                        if (!ss.GetColumn("ClassA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassB":
                        if (!ss.GetColumn("ClassB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassC":
                        if (!ss.GetColumn("ClassC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassD":
                        if (!ss.GetColumn("ClassD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassE":
                        if (!ss.GetColumn("ClassE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassF":
                        if (!ss.GetColumn("ClassF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassG":
                        if (!ss.GetColumn("ClassG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassH":
                        if (!ss.GetColumn("ClassH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassI":
                        if (!ss.GetColumn("ClassI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassJ":
                        if (!ss.GetColumn("ClassJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassK":
                        if (!ss.GetColumn("ClassK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassL":
                        if (!ss.GetColumn("ClassL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassM":
                        if (!ss.GetColumn("ClassM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassN":
                        if (!ss.GetColumn("ClassN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassO":
                        if (!ss.GetColumn("ClassO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassP":
                        if (!ss.GetColumn("ClassP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassQ":
                        if (!ss.GetColumn("ClassQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassR":
                        if (!ss.GetColumn("ClassR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassS":
                        if (!ss.GetColumn("ClassS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassT":
                        if (!ss.GetColumn("ClassT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassU":
                        if (!ss.GetColumn("ClassU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassV":
                        if (!ss.GetColumn("ClassV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassW":
                        if (!ss.GetColumn("ClassW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassX":
                        if (!ss.GetColumn("ClassX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassY":
                        if (!ss.GetColumn("ClassY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_ClassZ":
                        if (!ss.GetColumn("ClassZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumA":
                        if (!ss.GetColumn("NumA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumB":
                        if (!ss.GetColumn("NumB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumC":
                        if (!ss.GetColumn("NumC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumD":
                        if (!ss.GetColumn("NumD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumE":
                        if (!ss.GetColumn("NumE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumF":
                        if (!ss.GetColumn("NumF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumG":
                        if (!ss.GetColumn("NumG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumH":
                        if (!ss.GetColumn("NumH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumI":
                        if (!ss.GetColumn("NumI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumJ":
                        if (!ss.GetColumn("NumJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumK":
                        if (!ss.GetColumn("NumK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumL":
                        if (!ss.GetColumn("NumL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumM":
                        if (!ss.GetColumn("NumM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumN":
                        if (!ss.GetColumn("NumN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumO":
                        if (!ss.GetColumn("NumO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumP":
                        if (!ss.GetColumn("NumP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumQ":
                        if (!ss.GetColumn("NumQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumR":
                        if (!ss.GetColumn("NumR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumS":
                        if (!ss.GetColumn("NumS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumT":
                        if (!ss.GetColumn("NumT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumU":
                        if (!ss.GetColumn("NumU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumV":
                        if (!ss.GetColumn("NumV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumW":
                        if (!ss.GetColumn("NumW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumX":
                        if (!ss.GetColumn("NumX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumY":
                        if (!ss.GetColumn("NumY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_NumZ":
                        if (!ss.GetColumn("NumZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateA":
                        if (!ss.GetColumn("DateA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateB":
                        if (!ss.GetColumn("DateB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateC":
                        if (!ss.GetColumn("DateC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateD":
                        if (!ss.GetColumn("DateD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateE":
                        if (!ss.GetColumn("DateE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateF":
                        if (!ss.GetColumn("DateF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateG":
                        if (!ss.GetColumn("DateG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateH":
                        if (!ss.GetColumn("DateH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateI":
                        if (!ss.GetColumn("DateI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateJ":
                        if (!ss.GetColumn("DateJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateK":
                        if (!ss.GetColumn("DateK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateL":
                        if (!ss.GetColumn("DateL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateM":
                        if (!ss.GetColumn("DateM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateN":
                        if (!ss.GetColumn("DateN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateO":
                        if (!ss.GetColumn("DateO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateP":
                        if (!ss.GetColumn("DateP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateQ":
                        if (!ss.GetColumn("DateQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateR":
                        if (!ss.GetColumn("DateR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateS":
                        if (!ss.GetColumn("DateS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateT":
                        if (!ss.GetColumn("DateT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateU":
                        if (!ss.GetColumn("DateU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateV":
                        if (!ss.GetColumn("DateV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateW":
                        if (!ss.GetColumn("DateW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateX":
                        if (!ss.GetColumn("DateX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateY":
                        if (!ss.GetColumn("DateY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DateZ":
                        if (!ss.GetColumn("DateZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionA":
                        if (!ss.GetColumn("DescriptionA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionB":
                        if (!ss.GetColumn("DescriptionB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionC":
                        if (!ss.GetColumn("DescriptionC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionD":
                        if (!ss.GetColumn("DescriptionD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionE":
                        if (!ss.GetColumn("DescriptionE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionF":
                        if (!ss.GetColumn("DescriptionF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionG":
                        if (!ss.GetColumn("DescriptionG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionH":
                        if (!ss.GetColumn("DescriptionH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionI":
                        if (!ss.GetColumn("DescriptionI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionJ":
                        if (!ss.GetColumn("DescriptionJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionK":
                        if (!ss.GetColumn("DescriptionK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionL":
                        if (!ss.GetColumn("DescriptionL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionM":
                        if (!ss.GetColumn("DescriptionM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionN":
                        if (!ss.GetColumn("DescriptionN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionO":
                        if (!ss.GetColumn("DescriptionO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionP":
                        if (!ss.GetColumn("DescriptionP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionQ":
                        if (!ss.GetColumn("DescriptionQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionR":
                        if (!ss.GetColumn("DescriptionR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionS":
                        if (!ss.GetColumn("DescriptionS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionT":
                        if (!ss.GetColumn("DescriptionT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionU":
                        if (!ss.GetColumn("DescriptionU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionV":
                        if (!ss.GetColumn("DescriptionV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionW":
                        if (!ss.GetColumn("DescriptionW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionX":
                        if (!ss.GetColumn("DescriptionX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionY":
                        if (!ss.GetColumn("DescriptionY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_DescriptionZ":
                        if (!ss.GetColumn("DescriptionZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckA":
                        if (!ss.GetColumn("CheckA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckB":
                        if (!ss.GetColumn("CheckB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckC":
                        if (!ss.GetColumn("CheckC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckD":
                        if (!ss.GetColumn("CheckD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckE":
                        if (!ss.GetColumn("CheckE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckF":
                        if (!ss.GetColumn("CheckF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckG":
                        if (!ss.GetColumn("CheckG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckH":
                        if (!ss.GetColumn("CheckH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckI":
                        if (!ss.GetColumn("CheckI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckJ":
                        if (!ss.GetColumn("CheckJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckK":
                        if (!ss.GetColumn("CheckK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckL":
                        if (!ss.GetColumn("CheckL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckM":
                        if (!ss.GetColumn("CheckM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckN":
                        if (!ss.GetColumn("CheckN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckO":
                        if (!ss.GetColumn("CheckO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckP":
                        if (!ss.GetColumn("CheckP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckQ":
                        if (!ss.GetColumn("CheckQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckR":
                        if (!ss.GetColumn("CheckR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckS":
                        if (!ss.GetColumn("CheckS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckT":
                        if (!ss.GetColumn("CheckT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckU":
                        if (!ss.GetColumn("CheckU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckV":
                        if (!ss.GetColumn("CheckV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckW":
                        if (!ss.GetColumn("CheckW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckX":
                        if (!ss.GetColumn("CheckX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckY":
                        if (!ss.GetColumn("CheckY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Issues_CheckZ":
                        if (!ss.GetColumn("CheckZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "SearchPermissionElements":
                    case "OpenPermissionsDialog":
                    case "AddPermissions":
                    case "DeletePermissions":
                    case "InheritPermission":
                        if (!ss.CanManagePermission())
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
            if (!ss.CanDelete())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.CanManageTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            if (!ss.CanExport())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
