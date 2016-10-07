using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class ResultValidator
    {
        public static Error.Types OnCreating(
            SiteSettings siteSettings, Permissions.Types permissionType, ResultModel resultModel)
        {
            if (!permissionType.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_SiteId":
                        if (!siteSettings.GetColumn("SiteId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_UpdatedTime":
                        if (!siteSettings.GetColumn("UpdatedTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ResultId":
                        if (!siteSettings.GetColumn("ResultId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Ver":
                        if (!siteSettings.GetColumn("Ver").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Title":
                        if (!siteSettings.GetColumn("Title").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Body":
                        if (!siteSettings.GetColumn("Body").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_TitleBody":
                        if (!siteSettings.GetColumn("TitleBody").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Status":
                        if (!siteSettings.GetColumn("Status").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Manager":
                        if (!siteSettings.GetColumn("Manager").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Owner":
                        if (!siteSettings.GetColumn("Owner").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassA":
                        if (!siteSettings.GetColumn("ClassA").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassB":
                        if (!siteSettings.GetColumn("ClassB").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassC":
                        if (!siteSettings.GetColumn("ClassC").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassD":
                        if (!siteSettings.GetColumn("ClassD").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassE":
                        if (!siteSettings.GetColumn("ClassE").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassF":
                        if (!siteSettings.GetColumn("ClassF").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassG":
                        if (!siteSettings.GetColumn("ClassG").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassH":
                        if (!siteSettings.GetColumn("ClassH").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassI":
                        if (!siteSettings.GetColumn("ClassI").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassJ":
                        if (!siteSettings.GetColumn("ClassJ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassK":
                        if (!siteSettings.GetColumn("ClassK").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassL":
                        if (!siteSettings.GetColumn("ClassL").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassM":
                        if (!siteSettings.GetColumn("ClassM").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassN":
                        if (!siteSettings.GetColumn("ClassN").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassO":
                        if (!siteSettings.GetColumn("ClassO").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassP":
                        if (!siteSettings.GetColumn("ClassP").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassQ":
                        if (!siteSettings.GetColumn("ClassQ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassR":
                        if (!siteSettings.GetColumn("ClassR").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassS":
                        if (!siteSettings.GetColumn("ClassS").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassT":
                        if (!siteSettings.GetColumn("ClassT").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassU":
                        if (!siteSettings.GetColumn("ClassU").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassV":
                        if (!siteSettings.GetColumn("ClassV").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassW":
                        if (!siteSettings.GetColumn("ClassW").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassX":
                        if (!siteSettings.GetColumn("ClassX").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassY":
                        if (!siteSettings.GetColumn("ClassY").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassZ":
                        if (!siteSettings.GetColumn("ClassZ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumA":
                        if (!siteSettings.GetColumn("NumA").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumB":
                        if (!siteSettings.GetColumn("NumB").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumC":
                        if (!siteSettings.GetColumn("NumC").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumD":
                        if (!siteSettings.GetColumn("NumD").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumE":
                        if (!siteSettings.GetColumn("NumE").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumF":
                        if (!siteSettings.GetColumn("NumF").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumG":
                        if (!siteSettings.GetColumn("NumG").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumH":
                        if (!siteSettings.GetColumn("NumH").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumI":
                        if (!siteSettings.GetColumn("NumI").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumJ":
                        if (!siteSettings.GetColumn("NumJ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumK":
                        if (!siteSettings.GetColumn("NumK").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumL":
                        if (!siteSettings.GetColumn("NumL").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumM":
                        if (!siteSettings.GetColumn("NumM").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumN":
                        if (!siteSettings.GetColumn("NumN").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumO":
                        if (!siteSettings.GetColumn("NumO").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumP":
                        if (!siteSettings.GetColumn("NumP").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumQ":
                        if (!siteSettings.GetColumn("NumQ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumR":
                        if (!siteSettings.GetColumn("NumR").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumS":
                        if (!siteSettings.GetColumn("NumS").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumT":
                        if (!siteSettings.GetColumn("NumT").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumU":
                        if (!siteSettings.GetColumn("NumU").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumV":
                        if (!siteSettings.GetColumn("NumV").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumW":
                        if (!siteSettings.GetColumn("NumW").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumX":
                        if (!siteSettings.GetColumn("NumX").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumY":
                        if (!siteSettings.GetColumn("NumY").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumZ":
                        if (!siteSettings.GetColumn("NumZ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateA":
                        if (!siteSettings.GetColumn("DateA").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateB":
                        if (!siteSettings.GetColumn("DateB").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateC":
                        if (!siteSettings.GetColumn("DateC").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateD":
                        if (!siteSettings.GetColumn("DateD").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateE":
                        if (!siteSettings.GetColumn("DateE").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateF":
                        if (!siteSettings.GetColumn("DateF").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateG":
                        if (!siteSettings.GetColumn("DateG").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateH":
                        if (!siteSettings.GetColumn("DateH").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateI":
                        if (!siteSettings.GetColumn("DateI").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateJ":
                        if (!siteSettings.GetColumn("DateJ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateK":
                        if (!siteSettings.GetColumn("DateK").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateL":
                        if (!siteSettings.GetColumn("DateL").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateM":
                        if (!siteSettings.GetColumn("DateM").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateN":
                        if (!siteSettings.GetColumn("DateN").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateO":
                        if (!siteSettings.GetColumn("DateO").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateP":
                        if (!siteSettings.GetColumn("DateP").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateQ":
                        if (!siteSettings.GetColumn("DateQ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateR":
                        if (!siteSettings.GetColumn("DateR").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateS":
                        if (!siteSettings.GetColumn("DateS").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateT":
                        if (!siteSettings.GetColumn("DateT").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateU":
                        if (!siteSettings.GetColumn("DateU").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateV":
                        if (!siteSettings.GetColumn("DateV").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateW":
                        if (!siteSettings.GetColumn("DateW").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateX":
                        if (!siteSettings.GetColumn("DateX").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateY":
                        if (!siteSettings.GetColumn("DateY").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateZ":
                        if (!siteSettings.GetColumn("DateZ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionA":
                        if (!siteSettings.GetColumn("DescriptionA").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionB":
                        if (!siteSettings.GetColumn("DescriptionB").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionC":
                        if (!siteSettings.GetColumn("DescriptionC").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionD":
                        if (!siteSettings.GetColumn("DescriptionD").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionE":
                        if (!siteSettings.GetColumn("DescriptionE").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionF":
                        if (!siteSettings.GetColumn("DescriptionF").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionG":
                        if (!siteSettings.GetColumn("DescriptionG").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionH":
                        if (!siteSettings.GetColumn("DescriptionH").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionI":
                        if (!siteSettings.GetColumn("DescriptionI").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionJ":
                        if (!siteSettings.GetColumn("DescriptionJ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionK":
                        if (!siteSettings.GetColumn("DescriptionK").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionL":
                        if (!siteSettings.GetColumn("DescriptionL").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionM":
                        if (!siteSettings.GetColumn("DescriptionM").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionN":
                        if (!siteSettings.GetColumn("DescriptionN").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionO":
                        if (!siteSettings.GetColumn("DescriptionO").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionP":
                        if (!siteSettings.GetColumn("DescriptionP").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionQ":
                        if (!siteSettings.GetColumn("DescriptionQ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionR":
                        if (!siteSettings.GetColumn("DescriptionR").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionS":
                        if (!siteSettings.GetColumn("DescriptionS").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionT":
                        if (!siteSettings.GetColumn("DescriptionT").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionU":
                        if (!siteSettings.GetColumn("DescriptionU").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionV":
                        if (!siteSettings.GetColumn("DescriptionV").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionW":
                        if (!siteSettings.GetColumn("DescriptionW").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionX":
                        if (!siteSettings.GetColumn("DescriptionX").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionY":
                        if (!siteSettings.GetColumn("DescriptionY").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionZ":
                        if (!siteSettings.GetColumn("DescriptionZ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckA":
                        if (!siteSettings.GetColumn("CheckA").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckB":
                        if (!siteSettings.GetColumn("CheckB").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckC":
                        if (!siteSettings.GetColumn("CheckC").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckD":
                        if (!siteSettings.GetColumn("CheckD").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckE":
                        if (!siteSettings.GetColumn("CheckE").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckF":
                        if (!siteSettings.GetColumn("CheckF").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckG":
                        if (!siteSettings.GetColumn("CheckG").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckH":
                        if (!siteSettings.GetColumn("CheckH").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckI":
                        if (!siteSettings.GetColumn("CheckI").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckJ":
                        if (!siteSettings.GetColumn("CheckJ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckK":
                        if (!siteSettings.GetColumn("CheckK").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckL":
                        if (!siteSettings.GetColumn("CheckL").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckM":
                        if (!siteSettings.GetColumn("CheckM").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckN":
                        if (!siteSettings.GetColumn("CheckN").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckO":
                        if (!siteSettings.GetColumn("CheckO").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckP":
                        if (!siteSettings.GetColumn("CheckP").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckQ":
                        if (!siteSettings.GetColumn("CheckQ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckR":
                        if (!siteSettings.GetColumn("CheckR").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckS":
                        if (!siteSettings.GetColumn("CheckS").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckT":
                        if (!siteSettings.GetColumn("CheckT").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckU":
                        if (!siteSettings.GetColumn("CheckU").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckV":
                        if (!siteSettings.GetColumn("CheckV").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckW":
                        if (!siteSettings.GetColumn("CheckW").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckX":
                        if (!siteSettings.GetColumn("CheckX").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckY":
                        if (!siteSettings.GetColumn("CheckY").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckZ":
                        if (!siteSettings.GetColumn("CheckZ").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Comments":
                        if (!siteSettings.GetColumn("Comments").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Creator":
                        if (!siteSettings.GetColumn("Creator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Updator":
                        if (!siteSettings.GetColumn("Updator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CreatedTime":
                        if (!siteSettings.GetColumn("CreatedTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_VerUp":
                        if (!siteSettings.GetColumn("VerUp").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Timestamp":
                        if (!siteSettings.GetColumn("Timestamp").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            SiteSettings siteSettings, Permissions.Types permissionType, ResultModel resultModel)
        {
            if (!permissionType.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_SiteId":
                        if (!siteSettings.GetColumn("SiteId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_UpdatedTime":
                        if (!siteSettings.GetColumn("UpdatedTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ResultId":
                        if (!siteSettings.GetColumn("ResultId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Ver":
                        if (!siteSettings.GetColumn("Ver").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Title":
                        if (!siteSettings.GetColumn("Title").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Body":
                        if (!siteSettings.GetColumn("Body").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_TitleBody":
                        if (!siteSettings.GetColumn("TitleBody").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Status":
                        if (!siteSettings.GetColumn("Status").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Manager":
                        if (!siteSettings.GetColumn("Manager").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Owner":
                        if (!siteSettings.GetColumn("Owner").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassA":
                        if (!siteSettings.GetColumn("ClassA").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassB":
                        if (!siteSettings.GetColumn("ClassB").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassC":
                        if (!siteSettings.GetColumn("ClassC").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassD":
                        if (!siteSettings.GetColumn("ClassD").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassE":
                        if (!siteSettings.GetColumn("ClassE").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassF":
                        if (!siteSettings.GetColumn("ClassF").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassG":
                        if (!siteSettings.GetColumn("ClassG").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassH":
                        if (!siteSettings.GetColumn("ClassH").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassI":
                        if (!siteSettings.GetColumn("ClassI").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassJ":
                        if (!siteSettings.GetColumn("ClassJ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassK":
                        if (!siteSettings.GetColumn("ClassK").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassL":
                        if (!siteSettings.GetColumn("ClassL").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassM":
                        if (!siteSettings.GetColumn("ClassM").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassN":
                        if (!siteSettings.GetColumn("ClassN").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassO":
                        if (!siteSettings.GetColumn("ClassO").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassP":
                        if (!siteSettings.GetColumn("ClassP").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassQ":
                        if (!siteSettings.GetColumn("ClassQ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassR":
                        if (!siteSettings.GetColumn("ClassR").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassS":
                        if (!siteSettings.GetColumn("ClassS").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassT":
                        if (!siteSettings.GetColumn("ClassT").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassU":
                        if (!siteSettings.GetColumn("ClassU").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassV":
                        if (!siteSettings.GetColumn("ClassV").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassW":
                        if (!siteSettings.GetColumn("ClassW").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassX":
                        if (!siteSettings.GetColumn("ClassX").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassY":
                        if (!siteSettings.GetColumn("ClassY").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_ClassZ":
                        if (!siteSettings.GetColumn("ClassZ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumA":
                        if (!siteSettings.GetColumn("NumA").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumB":
                        if (!siteSettings.GetColumn("NumB").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumC":
                        if (!siteSettings.GetColumn("NumC").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumD":
                        if (!siteSettings.GetColumn("NumD").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumE":
                        if (!siteSettings.GetColumn("NumE").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumF":
                        if (!siteSettings.GetColumn("NumF").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumG":
                        if (!siteSettings.GetColumn("NumG").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumH":
                        if (!siteSettings.GetColumn("NumH").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumI":
                        if (!siteSettings.GetColumn("NumI").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumJ":
                        if (!siteSettings.GetColumn("NumJ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumK":
                        if (!siteSettings.GetColumn("NumK").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumL":
                        if (!siteSettings.GetColumn("NumL").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumM":
                        if (!siteSettings.GetColumn("NumM").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumN":
                        if (!siteSettings.GetColumn("NumN").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumO":
                        if (!siteSettings.GetColumn("NumO").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumP":
                        if (!siteSettings.GetColumn("NumP").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumQ":
                        if (!siteSettings.GetColumn("NumQ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumR":
                        if (!siteSettings.GetColumn("NumR").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumS":
                        if (!siteSettings.GetColumn("NumS").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumT":
                        if (!siteSettings.GetColumn("NumT").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumU":
                        if (!siteSettings.GetColumn("NumU").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumV":
                        if (!siteSettings.GetColumn("NumV").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumW":
                        if (!siteSettings.GetColumn("NumW").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumX":
                        if (!siteSettings.GetColumn("NumX").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumY":
                        if (!siteSettings.GetColumn("NumY").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_NumZ":
                        if (!siteSettings.GetColumn("NumZ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateA":
                        if (!siteSettings.GetColumn("DateA").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateB":
                        if (!siteSettings.GetColumn("DateB").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateC":
                        if (!siteSettings.GetColumn("DateC").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateD":
                        if (!siteSettings.GetColumn("DateD").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateE":
                        if (!siteSettings.GetColumn("DateE").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateF":
                        if (!siteSettings.GetColumn("DateF").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateG":
                        if (!siteSettings.GetColumn("DateG").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateH":
                        if (!siteSettings.GetColumn("DateH").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateI":
                        if (!siteSettings.GetColumn("DateI").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateJ":
                        if (!siteSettings.GetColumn("DateJ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateK":
                        if (!siteSettings.GetColumn("DateK").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateL":
                        if (!siteSettings.GetColumn("DateL").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateM":
                        if (!siteSettings.GetColumn("DateM").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateN":
                        if (!siteSettings.GetColumn("DateN").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateO":
                        if (!siteSettings.GetColumn("DateO").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateP":
                        if (!siteSettings.GetColumn("DateP").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateQ":
                        if (!siteSettings.GetColumn("DateQ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateR":
                        if (!siteSettings.GetColumn("DateR").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateS":
                        if (!siteSettings.GetColumn("DateS").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateT":
                        if (!siteSettings.GetColumn("DateT").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateU":
                        if (!siteSettings.GetColumn("DateU").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateV":
                        if (!siteSettings.GetColumn("DateV").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateW":
                        if (!siteSettings.GetColumn("DateW").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateX":
                        if (!siteSettings.GetColumn("DateX").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateY":
                        if (!siteSettings.GetColumn("DateY").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DateZ":
                        if (!siteSettings.GetColumn("DateZ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionA":
                        if (!siteSettings.GetColumn("DescriptionA").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionB":
                        if (!siteSettings.GetColumn("DescriptionB").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionC":
                        if (!siteSettings.GetColumn("DescriptionC").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionD":
                        if (!siteSettings.GetColumn("DescriptionD").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionE":
                        if (!siteSettings.GetColumn("DescriptionE").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionF":
                        if (!siteSettings.GetColumn("DescriptionF").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionG":
                        if (!siteSettings.GetColumn("DescriptionG").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionH":
                        if (!siteSettings.GetColumn("DescriptionH").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionI":
                        if (!siteSettings.GetColumn("DescriptionI").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionJ":
                        if (!siteSettings.GetColumn("DescriptionJ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionK":
                        if (!siteSettings.GetColumn("DescriptionK").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionL":
                        if (!siteSettings.GetColumn("DescriptionL").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionM":
                        if (!siteSettings.GetColumn("DescriptionM").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionN":
                        if (!siteSettings.GetColumn("DescriptionN").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionO":
                        if (!siteSettings.GetColumn("DescriptionO").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionP":
                        if (!siteSettings.GetColumn("DescriptionP").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionQ":
                        if (!siteSettings.GetColumn("DescriptionQ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionR":
                        if (!siteSettings.GetColumn("DescriptionR").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionS":
                        if (!siteSettings.GetColumn("DescriptionS").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionT":
                        if (!siteSettings.GetColumn("DescriptionT").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionU":
                        if (!siteSettings.GetColumn("DescriptionU").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionV":
                        if (!siteSettings.GetColumn("DescriptionV").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionW":
                        if (!siteSettings.GetColumn("DescriptionW").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionX":
                        if (!siteSettings.GetColumn("DescriptionX").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionY":
                        if (!siteSettings.GetColumn("DescriptionY").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_DescriptionZ":
                        if (!siteSettings.GetColumn("DescriptionZ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckA":
                        if (!siteSettings.GetColumn("CheckA").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckB":
                        if (!siteSettings.GetColumn("CheckB").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckC":
                        if (!siteSettings.GetColumn("CheckC").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckD":
                        if (!siteSettings.GetColumn("CheckD").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckE":
                        if (!siteSettings.GetColumn("CheckE").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckF":
                        if (!siteSettings.GetColumn("CheckF").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckG":
                        if (!siteSettings.GetColumn("CheckG").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckH":
                        if (!siteSettings.GetColumn("CheckH").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckI":
                        if (!siteSettings.GetColumn("CheckI").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckJ":
                        if (!siteSettings.GetColumn("CheckJ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckK":
                        if (!siteSettings.GetColumn("CheckK").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckL":
                        if (!siteSettings.GetColumn("CheckL").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckM":
                        if (!siteSettings.GetColumn("CheckM").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckN":
                        if (!siteSettings.GetColumn("CheckN").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckO":
                        if (!siteSettings.GetColumn("CheckO").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckP":
                        if (!siteSettings.GetColumn("CheckP").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckQ":
                        if (!siteSettings.GetColumn("CheckQ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckR":
                        if (!siteSettings.GetColumn("CheckR").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckS":
                        if (!siteSettings.GetColumn("CheckS").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckT":
                        if (!siteSettings.GetColumn("CheckT").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckU":
                        if (!siteSettings.GetColumn("CheckU").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckV":
                        if (!siteSettings.GetColumn("CheckV").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckW":
                        if (!siteSettings.GetColumn("CheckW").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckX":
                        if (!siteSettings.GetColumn("CheckX").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckY":
                        if (!siteSettings.GetColumn("CheckY").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CheckZ":
                        if (!siteSettings.GetColumn("CheckZ").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Comments":
                        if (!siteSettings.GetColumn("Comments").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Creator":
                        if (!siteSettings.GetColumn("Creator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Updator":
                        if (!siteSettings.GetColumn("Updator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_CreatedTime":
                        if (!siteSettings.GetColumn("CreatedTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_VerUp":
                        if (!siteSettings.GetColumn("VerUp").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Results_Timestamp":
                        if (!siteSettings.GetColumn("Timestamp").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }
    }
}
