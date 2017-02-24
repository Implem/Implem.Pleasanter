using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class ResultValidators
    {
        public static Error.Types OnCreating(
            SiteSettings ss, Permissions.Types pt, ResultModel resultModel)
        {
            if (!pt.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_Title":
                        if (!ss.GetColumn("Title").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Body":
                        if (!ss.GetColumn("Body").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Status":
                        if (!ss.GetColumn("Status").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Manager":
                        if (!ss.GetColumn("Manager").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Owner":
                        if (!ss.GetColumn("Owner").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassA":
                        if (!ss.GetColumn("ClassA").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassB":
                        if (!ss.GetColumn("ClassB").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassC":
                        if (!ss.GetColumn("ClassC").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassD":
                        if (!ss.GetColumn("ClassD").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassE":
                        if (!ss.GetColumn("ClassE").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassF":
                        if (!ss.GetColumn("ClassF").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassG":
                        if (!ss.GetColumn("ClassG").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassH":
                        if (!ss.GetColumn("ClassH").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassI":
                        if (!ss.GetColumn("ClassI").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassJ":
                        if (!ss.GetColumn("ClassJ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassK":
                        if (!ss.GetColumn("ClassK").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassL":
                        if (!ss.GetColumn("ClassL").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassM":
                        if (!ss.GetColumn("ClassM").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassN":
                        if (!ss.GetColumn("ClassN").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassO":
                        if (!ss.GetColumn("ClassO").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassP":
                        if (!ss.GetColumn("ClassP").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassQ":
                        if (!ss.GetColumn("ClassQ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassR":
                        if (!ss.GetColumn("ClassR").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassS":
                        if (!ss.GetColumn("ClassS").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassT":
                        if (!ss.GetColumn("ClassT").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassU":
                        if (!ss.GetColumn("ClassU").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassV":
                        if (!ss.GetColumn("ClassV").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassW":
                        if (!ss.GetColumn("ClassW").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassX":
                        if (!ss.GetColumn("ClassX").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassY":
                        if (!ss.GetColumn("ClassY").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassZ":
                        if (!ss.GetColumn("ClassZ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumA":
                        if (!ss.GetColumn("NumA").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumB":
                        if (!ss.GetColumn("NumB").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumC":
                        if (!ss.GetColumn("NumC").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumD":
                        if (!ss.GetColumn("NumD").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumE":
                        if (!ss.GetColumn("NumE").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumF":
                        if (!ss.GetColumn("NumF").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumG":
                        if (!ss.GetColumn("NumG").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumH":
                        if (!ss.GetColumn("NumH").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumI":
                        if (!ss.GetColumn("NumI").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumJ":
                        if (!ss.GetColumn("NumJ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumK":
                        if (!ss.GetColumn("NumK").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumL":
                        if (!ss.GetColumn("NumL").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumM":
                        if (!ss.GetColumn("NumM").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumN":
                        if (!ss.GetColumn("NumN").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumO":
                        if (!ss.GetColumn("NumO").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumP":
                        if (!ss.GetColumn("NumP").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumQ":
                        if (!ss.GetColumn("NumQ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumR":
                        if (!ss.GetColumn("NumR").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumS":
                        if (!ss.GetColumn("NumS").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumT":
                        if (!ss.GetColumn("NumT").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumU":
                        if (!ss.GetColumn("NumU").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumV":
                        if (!ss.GetColumn("NumV").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumW":
                        if (!ss.GetColumn("NumW").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumX":
                        if (!ss.GetColumn("NumX").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumY":
                        if (!ss.GetColumn("NumY").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumZ":
                        if (!ss.GetColumn("NumZ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateA":
                        if (!ss.GetColumn("DateA").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateB":
                        if (!ss.GetColumn("DateB").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateC":
                        if (!ss.GetColumn("DateC").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateD":
                        if (!ss.GetColumn("DateD").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateE":
                        if (!ss.GetColumn("DateE").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateF":
                        if (!ss.GetColumn("DateF").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateG":
                        if (!ss.GetColumn("DateG").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateH":
                        if (!ss.GetColumn("DateH").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateI":
                        if (!ss.GetColumn("DateI").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateJ":
                        if (!ss.GetColumn("DateJ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateK":
                        if (!ss.GetColumn("DateK").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateL":
                        if (!ss.GetColumn("DateL").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateM":
                        if (!ss.GetColumn("DateM").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateN":
                        if (!ss.GetColumn("DateN").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateO":
                        if (!ss.GetColumn("DateO").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateP":
                        if (!ss.GetColumn("DateP").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateQ":
                        if (!ss.GetColumn("DateQ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateR":
                        if (!ss.GetColumn("DateR").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateS":
                        if (!ss.GetColumn("DateS").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateT":
                        if (!ss.GetColumn("DateT").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateU":
                        if (!ss.GetColumn("DateU").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateV":
                        if (!ss.GetColumn("DateV").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateW":
                        if (!ss.GetColumn("DateW").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateX":
                        if (!ss.GetColumn("DateX").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateY":
                        if (!ss.GetColumn("DateY").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateZ":
                        if (!ss.GetColumn("DateZ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionA":
                        if (!ss.GetColumn("DescriptionA").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionB":
                        if (!ss.GetColumn("DescriptionB").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionC":
                        if (!ss.GetColumn("DescriptionC").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionD":
                        if (!ss.GetColumn("DescriptionD").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionE":
                        if (!ss.GetColumn("DescriptionE").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionF":
                        if (!ss.GetColumn("DescriptionF").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionG":
                        if (!ss.GetColumn("DescriptionG").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionH":
                        if (!ss.GetColumn("DescriptionH").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionI":
                        if (!ss.GetColumn("DescriptionI").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionJ":
                        if (!ss.GetColumn("DescriptionJ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionK":
                        if (!ss.GetColumn("DescriptionK").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionL":
                        if (!ss.GetColumn("DescriptionL").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionM":
                        if (!ss.GetColumn("DescriptionM").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionN":
                        if (!ss.GetColumn("DescriptionN").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionO":
                        if (!ss.GetColumn("DescriptionO").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionP":
                        if (!ss.GetColumn("DescriptionP").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionQ":
                        if (!ss.GetColumn("DescriptionQ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionR":
                        if (!ss.GetColumn("DescriptionR").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionS":
                        if (!ss.GetColumn("DescriptionS").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionT":
                        if (!ss.GetColumn("DescriptionT").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionU":
                        if (!ss.GetColumn("DescriptionU").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionV":
                        if (!ss.GetColumn("DescriptionV").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionW":
                        if (!ss.GetColumn("DescriptionW").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionX":
                        if (!ss.GetColumn("DescriptionX").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionY":
                        if (!ss.GetColumn("DescriptionY").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionZ":
                        if (!ss.GetColumn("DescriptionZ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckA":
                        if (!ss.GetColumn("CheckA").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckB":
                        if (!ss.GetColumn("CheckB").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckC":
                        if (!ss.GetColumn("CheckC").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckD":
                        if (!ss.GetColumn("CheckD").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckE":
                        if (!ss.GetColumn("CheckE").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckF":
                        if (!ss.GetColumn("CheckF").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckG":
                        if (!ss.GetColumn("CheckG").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckH":
                        if (!ss.GetColumn("CheckH").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckI":
                        if (!ss.GetColumn("CheckI").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckJ":
                        if (!ss.GetColumn("CheckJ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckK":
                        if (!ss.GetColumn("CheckK").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckL":
                        if (!ss.GetColumn("CheckL").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckM":
                        if (!ss.GetColumn("CheckM").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckN":
                        if (!ss.GetColumn("CheckN").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckO":
                        if (!ss.GetColumn("CheckO").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckP":
                        if (!ss.GetColumn("CheckP").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckQ":
                        if (!ss.GetColumn("CheckQ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckR":
                        if (!ss.GetColumn("CheckR").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckS":
                        if (!ss.GetColumn("CheckS").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckT":
                        if (!ss.GetColumn("CheckT").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckU":
                        if (!ss.GetColumn("CheckU").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckV":
                        if (!ss.GetColumn("CheckV").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckW":
                        if (!ss.GetColumn("CheckW").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckX":
                        if (!ss.GetColumn("CheckX").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckY":
                        if (!ss.GetColumn("CheckY").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckZ":
                        if (!ss.GetColumn("CheckZ").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            SiteSettings ss, Permissions.Types pt, ResultModel resultModel)
        {
            if (!pt.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_Title":
                        if (!ss.GetColumn("Title").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Body":
                        if (!ss.GetColumn("Body").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Status":
                        if (!ss.GetColumn("Status").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Manager":
                        if (!ss.GetColumn("Manager").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Owner":
                        if (!ss.GetColumn("Owner").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassA":
                        if (!ss.GetColumn("ClassA").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassB":
                        if (!ss.GetColumn("ClassB").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassC":
                        if (!ss.GetColumn("ClassC").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassD":
                        if (!ss.GetColumn("ClassD").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassE":
                        if (!ss.GetColumn("ClassE").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassF":
                        if (!ss.GetColumn("ClassF").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassG":
                        if (!ss.GetColumn("ClassG").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassH":
                        if (!ss.GetColumn("ClassH").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassI":
                        if (!ss.GetColumn("ClassI").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassJ":
                        if (!ss.GetColumn("ClassJ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassK":
                        if (!ss.GetColumn("ClassK").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassL":
                        if (!ss.GetColumn("ClassL").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassM":
                        if (!ss.GetColumn("ClassM").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassN":
                        if (!ss.GetColumn("ClassN").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassO":
                        if (!ss.GetColumn("ClassO").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassP":
                        if (!ss.GetColumn("ClassP").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassQ":
                        if (!ss.GetColumn("ClassQ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassR":
                        if (!ss.GetColumn("ClassR").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassS":
                        if (!ss.GetColumn("ClassS").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassT":
                        if (!ss.GetColumn("ClassT").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassU":
                        if (!ss.GetColumn("ClassU").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassV":
                        if (!ss.GetColumn("ClassV").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassW":
                        if (!ss.GetColumn("ClassW").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassX":
                        if (!ss.GetColumn("ClassX").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassY":
                        if (!ss.GetColumn("ClassY").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassZ":
                        if (!ss.GetColumn("ClassZ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumA":
                        if (!ss.GetColumn("NumA").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumB":
                        if (!ss.GetColumn("NumB").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumC":
                        if (!ss.GetColumn("NumC").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumD":
                        if (!ss.GetColumn("NumD").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumE":
                        if (!ss.GetColumn("NumE").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumF":
                        if (!ss.GetColumn("NumF").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumG":
                        if (!ss.GetColumn("NumG").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumH":
                        if (!ss.GetColumn("NumH").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumI":
                        if (!ss.GetColumn("NumI").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumJ":
                        if (!ss.GetColumn("NumJ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumK":
                        if (!ss.GetColumn("NumK").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumL":
                        if (!ss.GetColumn("NumL").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumM":
                        if (!ss.GetColumn("NumM").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumN":
                        if (!ss.GetColumn("NumN").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumO":
                        if (!ss.GetColumn("NumO").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumP":
                        if (!ss.GetColumn("NumP").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumQ":
                        if (!ss.GetColumn("NumQ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumR":
                        if (!ss.GetColumn("NumR").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumS":
                        if (!ss.GetColumn("NumS").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumT":
                        if (!ss.GetColumn("NumT").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumU":
                        if (!ss.GetColumn("NumU").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumV":
                        if (!ss.GetColumn("NumV").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumW":
                        if (!ss.GetColumn("NumW").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumX":
                        if (!ss.GetColumn("NumX").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumY":
                        if (!ss.GetColumn("NumY").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumZ":
                        if (!ss.GetColumn("NumZ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateA":
                        if (!ss.GetColumn("DateA").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateB":
                        if (!ss.GetColumn("DateB").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateC":
                        if (!ss.GetColumn("DateC").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateD":
                        if (!ss.GetColumn("DateD").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateE":
                        if (!ss.GetColumn("DateE").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateF":
                        if (!ss.GetColumn("DateF").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateG":
                        if (!ss.GetColumn("DateG").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateH":
                        if (!ss.GetColumn("DateH").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateI":
                        if (!ss.GetColumn("DateI").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateJ":
                        if (!ss.GetColumn("DateJ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateK":
                        if (!ss.GetColumn("DateK").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateL":
                        if (!ss.GetColumn("DateL").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateM":
                        if (!ss.GetColumn("DateM").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateN":
                        if (!ss.GetColumn("DateN").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateO":
                        if (!ss.GetColumn("DateO").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateP":
                        if (!ss.GetColumn("DateP").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateQ":
                        if (!ss.GetColumn("DateQ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateR":
                        if (!ss.GetColumn("DateR").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateS":
                        if (!ss.GetColumn("DateS").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateT":
                        if (!ss.GetColumn("DateT").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateU":
                        if (!ss.GetColumn("DateU").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateV":
                        if (!ss.GetColumn("DateV").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateW":
                        if (!ss.GetColumn("DateW").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateX":
                        if (!ss.GetColumn("DateX").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateY":
                        if (!ss.GetColumn("DateY").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateZ":
                        if (!ss.GetColumn("DateZ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionA":
                        if (!ss.GetColumn("DescriptionA").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionB":
                        if (!ss.GetColumn("DescriptionB").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionC":
                        if (!ss.GetColumn("DescriptionC").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionD":
                        if (!ss.GetColumn("DescriptionD").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionE":
                        if (!ss.GetColumn("DescriptionE").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionF":
                        if (!ss.GetColumn("DescriptionF").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionG":
                        if (!ss.GetColumn("DescriptionG").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionH":
                        if (!ss.GetColumn("DescriptionH").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionI":
                        if (!ss.GetColumn("DescriptionI").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionJ":
                        if (!ss.GetColumn("DescriptionJ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionK":
                        if (!ss.GetColumn("DescriptionK").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionL":
                        if (!ss.GetColumn("DescriptionL").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionM":
                        if (!ss.GetColumn("DescriptionM").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionN":
                        if (!ss.GetColumn("DescriptionN").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionO":
                        if (!ss.GetColumn("DescriptionO").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionP":
                        if (!ss.GetColumn("DescriptionP").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionQ":
                        if (!ss.GetColumn("DescriptionQ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionR":
                        if (!ss.GetColumn("DescriptionR").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionS":
                        if (!ss.GetColumn("DescriptionS").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionT":
                        if (!ss.GetColumn("DescriptionT").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionU":
                        if (!ss.GetColumn("DescriptionU").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionV":
                        if (!ss.GetColumn("DescriptionV").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionW":
                        if (!ss.GetColumn("DescriptionW").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionX":
                        if (!ss.GetColumn("DescriptionX").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionY":
                        if (!ss.GetColumn("DescriptionY").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionZ":
                        if (!ss.GetColumn("DescriptionZ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckA":
                        if (!ss.GetColumn("CheckA").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckB":
                        if (!ss.GetColumn("CheckB").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckC":
                        if (!ss.GetColumn("CheckC").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckD":
                        if (!ss.GetColumn("CheckD").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckE":
                        if (!ss.GetColumn("CheckE").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckF":
                        if (!ss.GetColumn("CheckF").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckG":
                        if (!ss.GetColumn("CheckG").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckH":
                        if (!ss.GetColumn("CheckH").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckI":
                        if (!ss.GetColumn("CheckI").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckJ":
                        if (!ss.GetColumn("CheckJ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckK":
                        if (!ss.GetColumn("CheckK").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckL":
                        if (!ss.GetColumn("CheckL").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckM":
                        if (!ss.GetColumn("CheckM").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckN":
                        if (!ss.GetColumn("CheckN").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckO":
                        if (!ss.GetColumn("CheckO").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckP":
                        if (!ss.GetColumn("CheckP").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckQ":
                        if (!ss.GetColumn("CheckQ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckR":
                        if (!ss.GetColumn("CheckR").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckS":
                        if (!ss.GetColumn("CheckS").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckT":
                        if (!ss.GetColumn("CheckT").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckU":
                        if (!ss.GetColumn("CheckU").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckV":
                        if (!ss.GetColumn("CheckV").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckW":
                        if (!ss.GetColumn("CheckW").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckX":
                        if (!ss.GetColumn("CheckX").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckY":
                        if (!ss.GetColumn("CheckY").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckZ":
                        if (!ss.GetColumn("CheckZ").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnMoving(Permissions.Types source, Permissions.Types destination)
        {
            if (!Permissions.CanMove(source, destination))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            SiteSettings ss, Permissions.Types pt, ResultModel resultModel)
        {
            if (!pt.CanDelete())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.Admins().CanManageTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
