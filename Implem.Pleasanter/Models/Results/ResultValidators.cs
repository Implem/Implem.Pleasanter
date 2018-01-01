using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
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
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_Title":
                        if (!ss.GetColumn("Title").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_Body":
                        if (!ss.GetColumn("Body").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_Status":
                        if (!ss.GetColumn("Status").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_Manager":
                        if (!ss.GetColumn("Manager").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_Owner":
                        if (!ss.GetColumn("Owner").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassA":
                        if (!ss.GetColumn("ClassA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassB":
                        if (!ss.GetColumn("ClassB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassC":
                        if (!ss.GetColumn("ClassC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassD":
                        if (!ss.GetColumn("ClassD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassE":
                        if (!ss.GetColumn("ClassE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassF":
                        if (!ss.GetColumn("ClassF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassG":
                        if (!ss.GetColumn("ClassG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassH":
                        if (!ss.GetColumn("ClassH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassI":
                        if (!ss.GetColumn("ClassI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassJ":
                        if (!ss.GetColumn("ClassJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassK":
                        if (!ss.GetColumn("ClassK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassL":
                        if (!ss.GetColumn("ClassL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassM":
                        if (!ss.GetColumn("ClassM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassN":
                        if (!ss.GetColumn("ClassN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassO":
                        if (!ss.GetColumn("ClassO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassP":
                        if (!ss.GetColumn("ClassP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassQ":
                        if (!ss.GetColumn("ClassQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassR":
                        if (!ss.GetColumn("ClassR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassS":
                        if (!ss.GetColumn("ClassS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassT":
                        if (!ss.GetColumn("ClassT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassU":
                        if (!ss.GetColumn("ClassU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassV":
                        if (!ss.GetColumn("ClassV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassW":
                        if (!ss.GetColumn("ClassW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassX":
                        if (!ss.GetColumn("ClassX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassY":
                        if (!ss.GetColumn("ClassY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassZ":
                        if (!ss.GetColumn("ClassZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumA":
                        if (!ss.GetColumn("NumA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumB":
                        if (!ss.GetColumn("NumB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumC":
                        if (!ss.GetColumn("NumC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumD":
                        if (!ss.GetColumn("NumD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumE":
                        if (!ss.GetColumn("NumE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumF":
                        if (!ss.GetColumn("NumF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumG":
                        if (!ss.GetColumn("NumG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumH":
                        if (!ss.GetColumn("NumH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumI":
                        if (!ss.GetColumn("NumI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumJ":
                        if (!ss.GetColumn("NumJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumK":
                        if (!ss.GetColumn("NumK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumL":
                        if (!ss.GetColumn("NumL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumM":
                        if (!ss.GetColumn("NumM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumN":
                        if (!ss.GetColumn("NumN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumO":
                        if (!ss.GetColumn("NumO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumP":
                        if (!ss.GetColumn("NumP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumQ":
                        if (!ss.GetColumn("NumQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumR":
                        if (!ss.GetColumn("NumR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumS":
                        if (!ss.GetColumn("NumS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumT":
                        if (!ss.GetColumn("NumT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumU":
                        if (!ss.GetColumn("NumU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumV":
                        if (!ss.GetColumn("NumV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumW":
                        if (!ss.GetColumn("NumW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumX":
                        if (!ss.GetColumn("NumX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumY":
                        if (!ss.GetColumn("NumY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumZ":
                        if (!ss.GetColumn("NumZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateA":
                        if (!ss.GetColumn("DateA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateB":
                        if (!ss.GetColumn("DateB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateC":
                        if (!ss.GetColumn("DateC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateD":
                        if (!ss.GetColumn("DateD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateE":
                        if (!ss.GetColumn("DateE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateF":
                        if (!ss.GetColumn("DateF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateG":
                        if (!ss.GetColumn("DateG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateH":
                        if (!ss.GetColumn("DateH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateI":
                        if (!ss.GetColumn("DateI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateJ":
                        if (!ss.GetColumn("DateJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateK":
                        if (!ss.GetColumn("DateK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateL":
                        if (!ss.GetColumn("DateL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateM":
                        if (!ss.GetColumn("DateM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateN":
                        if (!ss.GetColumn("DateN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateO":
                        if (!ss.GetColumn("DateO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateP":
                        if (!ss.GetColumn("DateP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateQ":
                        if (!ss.GetColumn("DateQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateR":
                        if (!ss.GetColumn("DateR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateS":
                        if (!ss.GetColumn("DateS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateT":
                        if (!ss.GetColumn("DateT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateU":
                        if (!ss.GetColumn("DateU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateV":
                        if (!ss.GetColumn("DateV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateW":
                        if (!ss.GetColumn("DateW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateX":
                        if (!ss.GetColumn("DateX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateY":
                        if (!ss.GetColumn("DateY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateZ":
                        if (!ss.GetColumn("DateZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionA":
                        if (!ss.GetColumn("DescriptionA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionB":
                        if (!ss.GetColumn("DescriptionB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionC":
                        if (!ss.GetColumn("DescriptionC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionD":
                        if (!ss.GetColumn("DescriptionD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionE":
                        if (!ss.GetColumn("DescriptionE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionF":
                        if (!ss.GetColumn("DescriptionF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionG":
                        if (!ss.GetColumn("DescriptionG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionH":
                        if (!ss.GetColumn("DescriptionH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionI":
                        if (!ss.GetColumn("DescriptionI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionJ":
                        if (!ss.GetColumn("DescriptionJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionK":
                        if (!ss.GetColumn("DescriptionK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionL":
                        if (!ss.GetColumn("DescriptionL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionM":
                        if (!ss.GetColumn("DescriptionM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionN":
                        if (!ss.GetColumn("DescriptionN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionO":
                        if (!ss.GetColumn("DescriptionO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionP":
                        if (!ss.GetColumn("DescriptionP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionQ":
                        if (!ss.GetColumn("DescriptionQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionR":
                        if (!ss.GetColumn("DescriptionR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionS":
                        if (!ss.GetColumn("DescriptionS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionT":
                        if (!ss.GetColumn("DescriptionT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionU":
                        if (!ss.GetColumn("DescriptionU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionV":
                        if (!ss.GetColumn("DescriptionV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionW":
                        if (!ss.GetColumn("DescriptionW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionX":
                        if (!ss.GetColumn("DescriptionX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionY":
                        if (!ss.GetColumn("DescriptionY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionZ":
                        if (!ss.GetColumn("DescriptionZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckA":
                        if (!ss.GetColumn("CheckA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckB":
                        if (!ss.GetColumn("CheckB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckC":
                        if (!ss.GetColumn("CheckC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckD":
                        if (!ss.GetColumn("CheckD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckE":
                        if (!ss.GetColumn("CheckE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckF":
                        if (!ss.GetColumn("CheckF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckG":
                        if (!ss.GetColumn("CheckG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckH":
                        if (!ss.GetColumn("CheckH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckI":
                        if (!ss.GetColumn("CheckI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckJ":
                        if (!ss.GetColumn("CheckJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckK":
                        if (!ss.GetColumn("CheckK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckL":
                        if (!ss.GetColumn("CheckL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckM":
                        if (!ss.GetColumn("CheckM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckN":
                        if (!ss.GetColumn("CheckN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckO":
                        if (!ss.GetColumn("CheckO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckP":
                        if (!ss.GetColumn("CheckP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckQ":
                        if (!ss.GetColumn("CheckQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckR":
                        if (!ss.GetColumn("CheckR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckS":
                        if (!ss.GetColumn("CheckS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckT":
                        if (!ss.GetColumn("CheckT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckU":
                        if (!ss.GetColumn("CheckU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckV":
                        if (!ss.GetColumn("CheckV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckW":
                        if (!ss.GetColumn("CheckW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckX":
                        if (!ss.GetColumn("CheckX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckY":
                        if (!ss.GetColumn("CheckY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckZ":
                        if (!ss.GetColumn("CheckZ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsA":
                        if (!ss.GetColumn("AttachmentsA").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsB":
                        if (!ss.GetColumn("AttachmentsB").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsC":
                        if (!ss.GetColumn("AttachmentsC").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsD":
                        if (!ss.GetColumn("AttachmentsD").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsE":
                        if (!ss.GetColumn("AttachmentsE").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsF":
                        if (!ss.GetColumn("AttachmentsF").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsG":
                        if (!ss.GetColumn("AttachmentsG").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsH":
                        if (!ss.GetColumn("AttachmentsH").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsI":
                        if (!ss.GetColumn("AttachmentsI").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsJ":
                        if (!ss.GetColumn("AttachmentsJ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsK":
                        if (!ss.GetColumn("AttachmentsK").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsL":
                        if (!ss.GetColumn("AttachmentsL").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsM":
                        if (!ss.GetColumn("AttachmentsM").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsN":
                        if (!ss.GetColumn("AttachmentsN").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsO":
                        if (!ss.GetColumn("AttachmentsO").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsP":
                        if (!ss.GetColumn("AttachmentsP").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsQ":
                        if (!ss.GetColumn("AttachmentsQ").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsR":
                        if (!ss.GetColumn("AttachmentsR").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsS":
                        if (!ss.GetColumn("AttachmentsS").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsT":
                        if (!ss.GetColumn("AttachmentsT").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsU":
                        if (!ss.GetColumn("AttachmentsU").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsV":
                        if (!ss.GetColumn("AttachmentsV").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsW":
                        if (!ss.GetColumn("AttachmentsW").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsX":
                        if (!ss.GetColumn("AttachmentsX").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsY":
                        if (!ss.GetColumn("AttachmentsY").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsZ":
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

        public static Error.Types OnUpdating(SiteSettings ss, ResultModel resultModel)
        {
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(resultModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_Title":
                        if (resultModel.Title_Updated() &&
                            !ss.GetColumn("Title").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_Body":
                        if (resultModel.Body_Updated() &&
                            !ss.GetColumn("Body").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_Status":
                        if (resultModel.Status_Updated() &&
                            !ss.GetColumn("Status").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_Manager":
                        if (resultModel.Manager_Updated() &&
                            !ss.GetColumn("Manager").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_Owner":
                        if (resultModel.Owner_Updated() &&
                            !ss.GetColumn("Owner").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassA":
                        if (resultModel.ClassA_Updated() &&
                            !ss.GetColumn("ClassA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassB":
                        if (resultModel.ClassB_Updated() &&
                            !ss.GetColumn("ClassB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassC":
                        if (resultModel.ClassC_Updated() &&
                            !ss.GetColumn("ClassC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassD":
                        if (resultModel.ClassD_Updated() &&
                            !ss.GetColumn("ClassD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassE":
                        if (resultModel.ClassE_Updated() &&
                            !ss.GetColumn("ClassE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassF":
                        if (resultModel.ClassF_Updated() &&
                            !ss.GetColumn("ClassF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassG":
                        if (resultModel.ClassG_Updated() &&
                            !ss.GetColumn("ClassG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassH":
                        if (resultModel.ClassH_Updated() &&
                            !ss.GetColumn("ClassH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassI":
                        if (resultModel.ClassI_Updated() &&
                            !ss.GetColumn("ClassI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassJ":
                        if (resultModel.ClassJ_Updated() &&
                            !ss.GetColumn("ClassJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassK":
                        if (resultModel.ClassK_Updated() &&
                            !ss.GetColumn("ClassK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassL":
                        if (resultModel.ClassL_Updated() &&
                            !ss.GetColumn("ClassL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassM":
                        if (resultModel.ClassM_Updated() &&
                            !ss.GetColumn("ClassM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassN":
                        if (resultModel.ClassN_Updated() &&
                            !ss.GetColumn("ClassN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassO":
                        if (resultModel.ClassO_Updated() &&
                            !ss.GetColumn("ClassO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassP":
                        if (resultModel.ClassP_Updated() &&
                            !ss.GetColumn("ClassP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassQ":
                        if (resultModel.ClassQ_Updated() &&
                            !ss.GetColumn("ClassQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassR":
                        if (resultModel.ClassR_Updated() &&
                            !ss.GetColumn("ClassR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassS":
                        if (resultModel.ClassS_Updated() &&
                            !ss.GetColumn("ClassS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassT":
                        if (resultModel.ClassT_Updated() &&
                            !ss.GetColumn("ClassT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassU":
                        if (resultModel.ClassU_Updated() &&
                            !ss.GetColumn("ClassU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassV":
                        if (resultModel.ClassV_Updated() &&
                            !ss.GetColumn("ClassV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassW":
                        if (resultModel.ClassW_Updated() &&
                            !ss.GetColumn("ClassW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassX":
                        if (resultModel.ClassX_Updated() &&
                            !ss.GetColumn("ClassX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassY":
                        if (resultModel.ClassY_Updated() &&
                            !ss.GetColumn("ClassY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_ClassZ":
                        if (resultModel.ClassZ_Updated() &&
                            !ss.GetColumn("ClassZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumA":
                        if (resultModel.NumA_Updated() &&
                            !ss.GetColumn("NumA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumB":
                        if (resultModel.NumB_Updated() &&
                            !ss.GetColumn("NumB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumC":
                        if (resultModel.NumC_Updated() &&
                            !ss.GetColumn("NumC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumD":
                        if (resultModel.NumD_Updated() &&
                            !ss.GetColumn("NumD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumE":
                        if (resultModel.NumE_Updated() &&
                            !ss.GetColumn("NumE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumF":
                        if (resultModel.NumF_Updated() &&
                            !ss.GetColumn("NumF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumG":
                        if (resultModel.NumG_Updated() &&
                            !ss.GetColumn("NumG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumH":
                        if (resultModel.NumH_Updated() &&
                            !ss.GetColumn("NumH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumI":
                        if (resultModel.NumI_Updated() &&
                            !ss.GetColumn("NumI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumJ":
                        if (resultModel.NumJ_Updated() &&
                            !ss.GetColumn("NumJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumK":
                        if (resultModel.NumK_Updated() &&
                            !ss.GetColumn("NumK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumL":
                        if (resultModel.NumL_Updated() &&
                            !ss.GetColumn("NumL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumM":
                        if (resultModel.NumM_Updated() &&
                            !ss.GetColumn("NumM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumN":
                        if (resultModel.NumN_Updated() &&
                            !ss.GetColumn("NumN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumO":
                        if (resultModel.NumO_Updated() &&
                            !ss.GetColumn("NumO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumP":
                        if (resultModel.NumP_Updated() &&
                            !ss.GetColumn("NumP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumQ":
                        if (resultModel.NumQ_Updated() &&
                            !ss.GetColumn("NumQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumR":
                        if (resultModel.NumR_Updated() &&
                            !ss.GetColumn("NumR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumS":
                        if (resultModel.NumS_Updated() &&
                            !ss.GetColumn("NumS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumT":
                        if (resultModel.NumT_Updated() &&
                            !ss.GetColumn("NumT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumU":
                        if (resultModel.NumU_Updated() &&
                            !ss.GetColumn("NumU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumV":
                        if (resultModel.NumV_Updated() &&
                            !ss.GetColumn("NumV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumW":
                        if (resultModel.NumW_Updated() &&
                            !ss.GetColumn("NumW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumX":
                        if (resultModel.NumX_Updated() &&
                            !ss.GetColumn("NumX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumY":
                        if (resultModel.NumY_Updated() &&
                            !ss.GetColumn("NumY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_NumZ":
                        if (resultModel.NumZ_Updated() &&
                            !ss.GetColumn("NumZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateA":
                        if (resultModel.DateA_Updated() &&
                            !ss.GetColumn("DateA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateB":
                        if (resultModel.DateB_Updated() &&
                            !ss.GetColumn("DateB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateC":
                        if (resultModel.DateC_Updated() &&
                            !ss.GetColumn("DateC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateD":
                        if (resultModel.DateD_Updated() &&
                            !ss.GetColumn("DateD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateE":
                        if (resultModel.DateE_Updated() &&
                            !ss.GetColumn("DateE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateF":
                        if (resultModel.DateF_Updated() &&
                            !ss.GetColumn("DateF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateG":
                        if (resultModel.DateG_Updated() &&
                            !ss.GetColumn("DateG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateH":
                        if (resultModel.DateH_Updated() &&
                            !ss.GetColumn("DateH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateI":
                        if (resultModel.DateI_Updated() &&
                            !ss.GetColumn("DateI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateJ":
                        if (resultModel.DateJ_Updated() &&
                            !ss.GetColumn("DateJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateK":
                        if (resultModel.DateK_Updated() &&
                            !ss.GetColumn("DateK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateL":
                        if (resultModel.DateL_Updated() &&
                            !ss.GetColumn("DateL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateM":
                        if (resultModel.DateM_Updated() &&
                            !ss.GetColumn("DateM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateN":
                        if (resultModel.DateN_Updated() &&
                            !ss.GetColumn("DateN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateO":
                        if (resultModel.DateO_Updated() &&
                            !ss.GetColumn("DateO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateP":
                        if (resultModel.DateP_Updated() &&
                            !ss.GetColumn("DateP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateQ":
                        if (resultModel.DateQ_Updated() &&
                            !ss.GetColumn("DateQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateR":
                        if (resultModel.DateR_Updated() &&
                            !ss.GetColumn("DateR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateS":
                        if (resultModel.DateS_Updated() &&
                            !ss.GetColumn("DateS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateT":
                        if (resultModel.DateT_Updated() &&
                            !ss.GetColumn("DateT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateU":
                        if (resultModel.DateU_Updated() &&
                            !ss.GetColumn("DateU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateV":
                        if (resultModel.DateV_Updated() &&
                            !ss.GetColumn("DateV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateW":
                        if (resultModel.DateW_Updated() &&
                            !ss.GetColumn("DateW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateX":
                        if (resultModel.DateX_Updated() &&
                            !ss.GetColumn("DateX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateY":
                        if (resultModel.DateY_Updated() &&
                            !ss.GetColumn("DateY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DateZ":
                        if (resultModel.DateZ_Updated() &&
                            !ss.GetColumn("DateZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionA":
                        if (resultModel.DescriptionA_Updated() &&
                            !ss.GetColumn("DescriptionA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionB":
                        if (resultModel.DescriptionB_Updated() &&
                            !ss.GetColumn("DescriptionB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionC":
                        if (resultModel.DescriptionC_Updated() &&
                            !ss.GetColumn("DescriptionC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionD":
                        if (resultModel.DescriptionD_Updated() &&
                            !ss.GetColumn("DescriptionD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionE":
                        if (resultModel.DescriptionE_Updated() &&
                            !ss.GetColumn("DescriptionE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionF":
                        if (resultModel.DescriptionF_Updated() &&
                            !ss.GetColumn("DescriptionF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionG":
                        if (resultModel.DescriptionG_Updated() &&
                            !ss.GetColumn("DescriptionG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionH":
                        if (resultModel.DescriptionH_Updated() &&
                            !ss.GetColumn("DescriptionH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionI":
                        if (resultModel.DescriptionI_Updated() &&
                            !ss.GetColumn("DescriptionI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionJ":
                        if (resultModel.DescriptionJ_Updated() &&
                            !ss.GetColumn("DescriptionJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionK":
                        if (resultModel.DescriptionK_Updated() &&
                            !ss.GetColumn("DescriptionK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionL":
                        if (resultModel.DescriptionL_Updated() &&
                            !ss.GetColumn("DescriptionL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionM":
                        if (resultModel.DescriptionM_Updated() &&
                            !ss.GetColumn("DescriptionM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionN":
                        if (resultModel.DescriptionN_Updated() &&
                            !ss.GetColumn("DescriptionN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionO":
                        if (resultModel.DescriptionO_Updated() &&
                            !ss.GetColumn("DescriptionO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionP":
                        if (resultModel.DescriptionP_Updated() &&
                            !ss.GetColumn("DescriptionP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionQ":
                        if (resultModel.DescriptionQ_Updated() &&
                            !ss.GetColumn("DescriptionQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionR":
                        if (resultModel.DescriptionR_Updated() &&
                            !ss.GetColumn("DescriptionR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionS":
                        if (resultModel.DescriptionS_Updated() &&
                            !ss.GetColumn("DescriptionS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionT":
                        if (resultModel.DescriptionT_Updated() &&
                            !ss.GetColumn("DescriptionT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionU":
                        if (resultModel.DescriptionU_Updated() &&
                            !ss.GetColumn("DescriptionU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionV":
                        if (resultModel.DescriptionV_Updated() &&
                            !ss.GetColumn("DescriptionV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionW":
                        if (resultModel.DescriptionW_Updated() &&
                            !ss.GetColumn("DescriptionW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionX":
                        if (resultModel.DescriptionX_Updated() &&
                            !ss.GetColumn("DescriptionX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionY":
                        if (resultModel.DescriptionY_Updated() &&
                            !ss.GetColumn("DescriptionY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_DescriptionZ":
                        if (resultModel.DescriptionZ_Updated() &&
                            !ss.GetColumn("DescriptionZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckA":
                        if (resultModel.CheckA_Updated() &&
                            !ss.GetColumn("CheckA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckB":
                        if (resultModel.CheckB_Updated() &&
                            !ss.GetColumn("CheckB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckC":
                        if (resultModel.CheckC_Updated() &&
                            !ss.GetColumn("CheckC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckD":
                        if (resultModel.CheckD_Updated() &&
                            !ss.GetColumn("CheckD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckE":
                        if (resultModel.CheckE_Updated() &&
                            !ss.GetColumn("CheckE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckF":
                        if (resultModel.CheckF_Updated() &&
                            !ss.GetColumn("CheckF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckG":
                        if (resultModel.CheckG_Updated() &&
                            !ss.GetColumn("CheckG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckH":
                        if (resultModel.CheckH_Updated() &&
                            !ss.GetColumn("CheckH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckI":
                        if (resultModel.CheckI_Updated() &&
                            !ss.GetColumn("CheckI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckJ":
                        if (resultModel.CheckJ_Updated() &&
                            !ss.GetColumn("CheckJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckK":
                        if (resultModel.CheckK_Updated() &&
                            !ss.GetColumn("CheckK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckL":
                        if (resultModel.CheckL_Updated() &&
                            !ss.GetColumn("CheckL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckM":
                        if (resultModel.CheckM_Updated() &&
                            !ss.GetColumn("CheckM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckN":
                        if (resultModel.CheckN_Updated() &&
                            !ss.GetColumn("CheckN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckO":
                        if (resultModel.CheckO_Updated() &&
                            !ss.GetColumn("CheckO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckP":
                        if (resultModel.CheckP_Updated() &&
                            !ss.GetColumn("CheckP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckQ":
                        if (resultModel.CheckQ_Updated() &&
                            !ss.GetColumn("CheckQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckR":
                        if (resultModel.CheckR_Updated() &&
                            !ss.GetColumn("CheckR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckS":
                        if (resultModel.CheckS_Updated() &&
                            !ss.GetColumn("CheckS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckT":
                        if (resultModel.CheckT_Updated() &&
                            !ss.GetColumn("CheckT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckU":
                        if (resultModel.CheckU_Updated() &&
                            !ss.GetColumn("CheckU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckV":
                        if (resultModel.CheckV_Updated() &&
                            !ss.GetColumn("CheckV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckW":
                        if (resultModel.CheckW_Updated() &&
                            !ss.GetColumn("CheckW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckX":
                        if (resultModel.CheckX_Updated() &&
                            !ss.GetColumn("CheckX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckY":
                        if (resultModel.CheckY_Updated() &&
                            !ss.GetColumn("CheckY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_CheckZ":
                        if (resultModel.CheckZ_Updated() &&
                            !ss.GetColumn("CheckZ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsA":
                        if (resultModel.AttachmentsA_Updated() &&
                            !ss.GetColumn("AttachmentsA").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsB":
                        if (resultModel.AttachmentsB_Updated() &&
                            !ss.GetColumn("AttachmentsB").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsC":
                        if (resultModel.AttachmentsC_Updated() &&
                            !ss.GetColumn("AttachmentsC").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsD":
                        if (resultModel.AttachmentsD_Updated() &&
                            !ss.GetColumn("AttachmentsD").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsE":
                        if (resultModel.AttachmentsE_Updated() &&
                            !ss.GetColumn("AttachmentsE").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsF":
                        if (resultModel.AttachmentsF_Updated() &&
                            !ss.GetColumn("AttachmentsF").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsG":
                        if (resultModel.AttachmentsG_Updated() &&
                            !ss.GetColumn("AttachmentsG").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsH":
                        if (resultModel.AttachmentsH_Updated() &&
                            !ss.GetColumn("AttachmentsH").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsI":
                        if (resultModel.AttachmentsI_Updated() &&
                            !ss.GetColumn("AttachmentsI").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsJ":
                        if (resultModel.AttachmentsJ_Updated() &&
                            !ss.GetColumn("AttachmentsJ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsK":
                        if (resultModel.AttachmentsK_Updated() &&
                            !ss.GetColumn("AttachmentsK").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsL":
                        if (resultModel.AttachmentsL_Updated() &&
                            !ss.GetColumn("AttachmentsL").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsM":
                        if (resultModel.AttachmentsM_Updated() &&
                            !ss.GetColumn("AttachmentsM").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsN":
                        if (resultModel.AttachmentsN_Updated() &&
                            !ss.GetColumn("AttachmentsN").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsO":
                        if (resultModel.AttachmentsO_Updated() &&
                            !ss.GetColumn("AttachmentsO").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsP":
                        if (resultModel.AttachmentsP_Updated() &&
                            !ss.GetColumn("AttachmentsP").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsQ":
                        if (resultModel.AttachmentsQ_Updated() &&
                            !ss.GetColumn("AttachmentsQ").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsR":
                        if (resultModel.AttachmentsR_Updated() &&
                            !ss.GetColumn("AttachmentsR").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsS":
                        if (resultModel.AttachmentsS_Updated() &&
                            !ss.GetColumn("AttachmentsS").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsT":
                        if (resultModel.AttachmentsT_Updated() &&
                            !ss.GetColumn("AttachmentsT").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsU":
                        if (resultModel.AttachmentsU_Updated() &&
                            !ss.GetColumn("AttachmentsU").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsV":
                        if (resultModel.AttachmentsV_Updated() &&
                            !ss.GetColumn("AttachmentsV").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsW":
                        if (resultModel.AttachmentsW_Updated() &&
                            !ss.GetColumn("AttachmentsW").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsX":
                        if (resultModel.AttachmentsX_Updated() &&
                            !ss.GetColumn("AttachmentsX").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsY":
                        if (resultModel.AttachmentsY_Updated() &&
                            !ss.GetColumn("AttachmentsY").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Results_AttachmentsZ":
                        if (resultModel.AttachmentsZ_Updated() &&
                            !ss.GetColumn("AttachmentsZ").CanUpdate)
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
