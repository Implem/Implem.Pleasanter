using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class ResultValidators
    {
        public static Error.Types OnCreating(SiteSettings ss, ResultModel resultModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_Title":
                        if (!ss.GetColumn("Title").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Body":
                        if (!ss.GetColumn("Body").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Status":
                        if (!ss.GetColumn("Status").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Manager":
                        if (!ss.GetColumn("Manager").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Owner":
                        if (!ss.GetColumn("Owner").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassA":
                        if (!ss.GetColumn("ClassA").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassB":
                        if (!ss.GetColumn("ClassB").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassC":
                        if (!ss.GetColumn("ClassC").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassD":
                        if (!ss.GetColumn("ClassD").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassE":
                        if (!ss.GetColumn("ClassE").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassF":
                        if (!ss.GetColumn("ClassF").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassG":
                        if (!ss.GetColumn("ClassG").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassH":
                        if (!ss.GetColumn("ClassH").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassI":
                        if (!ss.GetColumn("ClassI").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassJ":
                        if (!ss.GetColumn("ClassJ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassK":
                        if (!ss.GetColumn("ClassK").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassL":
                        if (!ss.GetColumn("ClassL").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassM":
                        if (!ss.GetColumn("ClassM").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassN":
                        if (!ss.GetColumn("ClassN").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassO":
                        if (!ss.GetColumn("ClassO").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassP":
                        if (!ss.GetColumn("ClassP").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassQ":
                        if (!ss.GetColumn("ClassQ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassR":
                        if (!ss.GetColumn("ClassR").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassS":
                        if (!ss.GetColumn("ClassS").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassT":
                        if (!ss.GetColumn("ClassT").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassU":
                        if (!ss.GetColumn("ClassU").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassV":
                        if (!ss.GetColumn("ClassV").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassW":
                        if (!ss.GetColumn("ClassW").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassX":
                        if (!ss.GetColumn("ClassX").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassY":
                        if (!ss.GetColumn("ClassY").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassZ":
                        if (!ss.GetColumn("ClassZ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumA":
                        if (!ss.GetColumn("NumA").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumB":
                        if (!ss.GetColumn("NumB").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumC":
                        if (!ss.GetColumn("NumC").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumD":
                        if (!ss.GetColumn("NumD").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumE":
                        if (!ss.GetColumn("NumE").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumF":
                        if (!ss.GetColumn("NumF").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumG":
                        if (!ss.GetColumn("NumG").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumH":
                        if (!ss.GetColumn("NumH").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumI":
                        if (!ss.GetColumn("NumI").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumJ":
                        if (!ss.GetColumn("NumJ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumK":
                        if (!ss.GetColumn("NumK").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumL":
                        if (!ss.GetColumn("NumL").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumM":
                        if (!ss.GetColumn("NumM").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumN":
                        if (!ss.GetColumn("NumN").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumO":
                        if (!ss.GetColumn("NumO").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumP":
                        if (!ss.GetColumn("NumP").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumQ":
                        if (!ss.GetColumn("NumQ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumR":
                        if (!ss.GetColumn("NumR").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumS":
                        if (!ss.GetColumn("NumS").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumT":
                        if (!ss.GetColumn("NumT").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumU":
                        if (!ss.GetColumn("NumU").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumV":
                        if (!ss.GetColumn("NumV").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumW":
                        if (!ss.GetColumn("NumW").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumX":
                        if (!ss.GetColumn("NumX").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumY":
                        if (!ss.GetColumn("NumY").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumZ":
                        if (!ss.GetColumn("NumZ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateA":
                        if (!ss.GetColumn("DateA").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateB":
                        if (!ss.GetColumn("DateB").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateC":
                        if (!ss.GetColumn("DateC").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateD":
                        if (!ss.GetColumn("DateD").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateE":
                        if (!ss.GetColumn("DateE").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateF":
                        if (!ss.GetColumn("DateF").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateG":
                        if (!ss.GetColumn("DateG").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateH":
                        if (!ss.GetColumn("DateH").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateI":
                        if (!ss.GetColumn("DateI").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateJ":
                        if (!ss.GetColumn("DateJ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateK":
                        if (!ss.GetColumn("DateK").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateL":
                        if (!ss.GetColumn("DateL").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateM":
                        if (!ss.GetColumn("DateM").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateN":
                        if (!ss.GetColumn("DateN").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateO":
                        if (!ss.GetColumn("DateO").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateP":
                        if (!ss.GetColumn("DateP").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateQ":
                        if (!ss.GetColumn("DateQ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateR":
                        if (!ss.GetColumn("DateR").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateS":
                        if (!ss.GetColumn("DateS").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateT":
                        if (!ss.GetColumn("DateT").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateU":
                        if (!ss.GetColumn("DateU").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateV":
                        if (!ss.GetColumn("DateV").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateW":
                        if (!ss.GetColumn("DateW").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateX":
                        if (!ss.GetColumn("DateX").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateY":
                        if (!ss.GetColumn("DateY").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateZ":
                        if (!ss.GetColumn("DateZ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionA":
                        if (!ss.GetColumn("DescriptionA").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionB":
                        if (!ss.GetColumn("DescriptionB").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionC":
                        if (!ss.GetColumn("DescriptionC").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionD":
                        if (!ss.GetColumn("DescriptionD").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionE":
                        if (!ss.GetColumn("DescriptionE").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionF":
                        if (!ss.GetColumn("DescriptionF").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionG":
                        if (!ss.GetColumn("DescriptionG").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionH":
                        if (!ss.GetColumn("DescriptionH").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionI":
                        if (!ss.GetColumn("DescriptionI").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionJ":
                        if (!ss.GetColumn("DescriptionJ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionK":
                        if (!ss.GetColumn("DescriptionK").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionL":
                        if (!ss.GetColumn("DescriptionL").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionM":
                        if (!ss.GetColumn("DescriptionM").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionN":
                        if (!ss.GetColumn("DescriptionN").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionO":
                        if (!ss.GetColumn("DescriptionO").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionP":
                        if (!ss.GetColumn("DescriptionP").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionQ":
                        if (!ss.GetColumn("DescriptionQ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionR":
                        if (!ss.GetColumn("DescriptionR").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionS":
                        if (!ss.GetColumn("DescriptionS").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionT":
                        if (!ss.GetColumn("DescriptionT").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionU":
                        if (!ss.GetColumn("DescriptionU").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionV":
                        if (!ss.GetColumn("DescriptionV").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionW":
                        if (!ss.GetColumn("DescriptionW").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionX":
                        if (!ss.GetColumn("DescriptionX").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionY":
                        if (!ss.GetColumn("DescriptionY").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionZ":
                        if (!ss.GetColumn("DescriptionZ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckA":
                        if (!ss.GetColumn("CheckA").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckB":
                        if (!ss.GetColumn("CheckB").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckC":
                        if (!ss.GetColumn("CheckC").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckD":
                        if (!ss.GetColumn("CheckD").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckE":
                        if (!ss.GetColumn("CheckE").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckF":
                        if (!ss.GetColumn("CheckF").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckG":
                        if (!ss.GetColumn("CheckG").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckH":
                        if (!ss.GetColumn("CheckH").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckI":
                        if (!ss.GetColumn("CheckI").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckJ":
                        if (!ss.GetColumn("CheckJ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckK":
                        if (!ss.GetColumn("CheckK").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckL":
                        if (!ss.GetColumn("CheckL").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckM":
                        if (!ss.GetColumn("CheckM").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckN":
                        if (!ss.GetColumn("CheckN").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckO":
                        if (!ss.GetColumn("CheckO").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckP":
                        if (!ss.GetColumn("CheckP").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckQ":
                        if (!ss.GetColumn("CheckQ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckR":
                        if (!ss.GetColumn("CheckR").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckS":
                        if (!ss.GetColumn("CheckS").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckT":
                        if (!ss.GetColumn("CheckT").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckU":
                        if (!ss.GetColumn("CheckU").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckV":
                        if (!ss.GetColumn("CheckV").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckW":
                        if (!ss.GetColumn("CheckW").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckX":
                        if (!ss.GetColumn("CheckX").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckY":
                        if (!ss.GetColumn("CheckY").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckZ":
                        if (!ss.GetColumn("CheckZ").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
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
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_Title":
                        if (!ss.GetColumn("Title").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Body":
                        if (!ss.GetColumn("Body").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Status":
                        if (!ss.GetColumn("Status").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Manager":
                        if (!ss.GetColumn("Manager").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Owner":
                        if (!ss.GetColumn("Owner").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassA":
                        if (!ss.GetColumn("ClassA").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassB":
                        if (!ss.GetColumn("ClassB").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassC":
                        if (!ss.GetColumn("ClassC").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassD":
                        if (!ss.GetColumn("ClassD").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassE":
                        if (!ss.GetColumn("ClassE").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassF":
                        if (!ss.GetColumn("ClassF").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassG":
                        if (!ss.GetColumn("ClassG").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassH":
                        if (!ss.GetColumn("ClassH").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassI":
                        if (!ss.GetColumn("ClassI").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassJ":
                        if (!ss.GetColumn("ClassJ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassK":
                        if (!ss.GetColumn("ClassK").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassL":
                        if (!ss.GetColumn("ClassL").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassM":
                        if (!ss.GetColumn("ClassM").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassN":
                        if (!ss.GetColumn("ClassN").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassO":
                        if (!ss.GetColumn("ClassO").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassP":
                        if (!ss.GetColumn("ClassP").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassQ":
                        if (!ss.GetColumn("ClassQ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassR":
                        if (!ss.GetColumn("ClassR").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassS":
                        if (!ss.GetColumn("ClassS").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassT":
                        if (!ss.GetColumn("ClassT").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassU":
                        if (!ss.GetColumn("ClassU").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassV":
                        if (!ss.GetColumn("ClassV").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassW":
                        if (!ss.GetColumn("ClassW").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassX":
                        if (!ss.GetColumn("ClassX").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassY":
                        if (!ss.GetColumn("ClassY").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassZ":
                        if (!ss.GetColumn("ClassZ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumA":
                        if (!ss.GetColumn("NumA").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumB":
                        if (!ss.GetColumn("NumB").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumC":
                        if (!ss.GetColumn("NumC").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumD":
                        if (!ss.GetColumn("NumD").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumE":
                        if (!ss.GetColumn("NumE").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumF":
                        if (!ss.GetColumn("NumF").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumG":
                        if (!ss.GetColumn("NumG").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumH":
                        if (!ss.GetColumn("NumH").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumI":
                        if (!ss.GetColumn("NumI").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumJ":
                        if (!ss.GetColumn("NumJ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumK":
                        if (!ss.GetColumn("NumK").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumL":
                        if (!ss.GetColumn("NumL").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumM":
                        if (!ss.GetColumn("NumM").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumN":
                        if (!ss.GetColumn("NumN").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumO":
                        if (!ss.GetColumn("NumO").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumP":
                        if (!ss.GetColumn("NumP").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumQ":
                        if (!ss.GetColumn("NumQ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumR":
                        if (!ss.GetColumn("NumR").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumS":
                        if (!ss.GetColumn("NumS").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumT":
                        if (!ss.GetColumn("NumT").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumU":
                        if (!ss.GetColumn("NumU").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumV":
                        if (!ss.GetColumn("NumV").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumW":
                        if (!ss.GetColumn("NumW").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumX":
                        if (!ss.GetColumn("NumX").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumY":
                        if (!ss.GetColumn("NumY").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumZ":
                        if (!ss.GetColumn("NumZ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateA":
                        if (!ss.GetColumn("DateA").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateB":
                        if (!ss.GetColumn("DateB").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateC":
                        if (!ss.GetColumn("DateC").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateD":
                        if (!ss.GetColumn("DateD").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateE":
                        if (!ss.GetColumn("DateE").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateF":
                        if (!ss.GetColumn("DateF").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateG":
                        if (!ss.GetColumn("DateG").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateH":
                        if (!ss.GetColumn("DateH").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateI":
                        if (!ss.GetColumn("DateI").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateJ":
                        if (!ss.GetColumn("DateJ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateK":
                        if (!ss.GetColumn("DateK").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateL":
                        if (!ss.GetColumn("DateL").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateM":
                        if (!ss.GetColumn("DateM").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateN":
                        if (!ss.GetColumn("DateN").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateO":
                        if (!ss.GetColumn("DateO").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateP":
                        if (!ss.GetColumn("DateP").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateQ":
                        if (!ss.GetColumn("DateQ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateR":
                        if (!ss.GetColumn("DateR").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateS":
                        if (!ss.GetColumn("DateS").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateT":
                        if (!ss.GetColumn("DateT").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateU":
                        if (!ss.GetColumn("DateU").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateV":
                        if (!ss.GetColumn("DateV").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateW":
                        if (!ss.GetColumn("DateW").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateX":
                        if (!ss.GetColumn("DateX").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateY":
                        if (!ss.GetColumn("DateY").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateZ":
                        if (!ss.GetColumn("DateZ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionA":
                        if (!ss.GetColumn("DescriptionA").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionB":
                        if (!ss.GetColumn("DescriptionB").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionC":
                        if (!ss.GetColumn("DescriptionC").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionD":
                        if (!ss.GetColumn("DescriptionD").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionE":
                        if (!ss.GetColumn("DescriptionE").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionF":
                        if (!ss.GetColumn("DescriptionF").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionG":
                        if (!ss.GetColumn("DescriptionG").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionH":
                        if (!ss.GetColumn("DescriptionH").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionI":
                        if (!ss.GetColumn("DescriptionI").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionJ":
                        if (!ss.GetColumn("DescriptionJ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionK":
                        if (!ss.GetColumn("DescriptionK").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionL":
                        if (!ss.GetColumn("DescriptionL").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionM":
                        if (!ss.GetColumn("DescriptionM").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionN":
                        if (!ss.GetColumn("DescriptionN").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionO":
                        if (!ss.GetColumn("DescriptionO").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionP":
                        if (!ss.GetColumn("DescriptionP").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionQ":
                        if (!ss.GetColumn("DescriptionQ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionR":
                        if (!ss.GetColumn("DescriptionR").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionS":
                        if (!ss.GetColumn("DescriptionS").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionT":
                        if (!ss.GetColumn("DescriptionT").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionU":
                        if (!ss.GetColumn("DescriptionU").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionV":
                        if (!ss.GetColumn("DescriptionV").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionW":
                        if (!ss.GetColumn("DescriptionW").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionX":
                        if (!ss.GetColumn("DescriptionX").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionY":
                        if (!ss.GetColumn("DescriptionY").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionZ":
                        if (!ss.GetColumn("DescriptionZ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckA":
                        if (!ss.GetColumn("CheckA").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckB":
                        if (!ss.GetColumn("CheckB").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckC":
                        if (!ss.GetColumn("CheckC").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckD":
                        if (!ss.GetColumn("CheckD").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckE":
                        if (!ss.GetColumn("CheckE").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckF":
                        if (!ss.GetColumn("CheckF").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckG":
                        if (!ss.GetColumn("CheckG").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckH":
                        if (!ss.GetColumn("CheckH").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckI":
                        if (!ss.GetColumn("CheckI").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckJ":
                        if (!ss.GetColumn("CheckJ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckK":
                        if (!ss.GetColumn("CheckK").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckL":
                        if (!ss.GetColumn("CheckL").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckM":
                        if (!ss.GetColumn("CheckM").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckN":
                        if (!ss.GetColumn("CheckN").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckO":
                        if (!ss.GetColumn("CheckO").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckP":
                        if (!ss.GetColumn("CheckP").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckQ":
                        if (!ss.GetColumn("CheckQ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckR":
                        if (!ss.GetColumn("CheckR").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckS":
                        if (!ss.GetColumn("CheckS").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckT":
                        if (!ss.GetColumn("CheckT").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckU":
                        if (!ss.GetColumn("CheckU").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckV":
                        if (!ss.GetColumn("CheckV").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckW":
                        if (!ss.GetColumn("CheckW").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckX":
                        if (!ss.GetColumn("CheckX").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckY":
                        if (!ss.GetColumn("CheckY").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckZ":
                        if (!ss.GetColumn("CheckZ").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
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

        public static Error.Types OnDeleting(SiteSettings ss, ResultModel resultModel)
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
