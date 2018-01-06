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
            foreach (var column in ss.Columns.Where(o => !o.CanUpdate))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (resultModel.Title_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Body":
                        if (resultModel.Body_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Status":
                        if (resultModel.Status_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Manager":
                        if (resultModel.Manager_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Owner":
                        if (resultModel.Owner_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassA":
                        if (resultModel.ClassA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassB":
                        if (resultModel.ClassB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassC":
                        if (resultModel.ClassC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassD":
                        if (resultModel.ClassD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassE":
                        if (resultModel.ClassE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassF":
                        if (resultModel.ClassF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassG":
                        if (resultModel.ClassG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassH":
                        if (resultModel.ClassH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassI":
                        if (resultModel.ClassI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassJ":
                        if (resultModel.ClassJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassK":
                        if (resultModel.ClassK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassL":
                        if (resultModel.ClassL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassM":
                        if (resultModel.ClassM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassN":
                        if (resultModel.ClassN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassO":
                        if (resultModel.ClassO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassP":
                        if (resultModel.ClassP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassQ":
                        if (resultModel.ClassQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassR":
                        if (resultModel.ClassR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassS":
                        if (resultModel.ClassS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassT":
                        if (resultModel.ClassT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassU":
                        if (resultModel.ClassU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassV":
                        if (resultModel.ClassV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassW":
                        if (resultModel.ClassW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassX":
                        if (resultModel.ClassX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassY":
                        if (resultModel.ClassY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ClassZ":
                        if (resultModel.ClassZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumA":
                        if (resultModel.NumA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumB":
                        if (resultModel.NumB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumC":
                        if (resultModel.NumC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumD":
                        if (resultModel.NumD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumE":
                        if (resultModel.NumE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumF":
                        if (resultModel.NumF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumG":
                        if (resultModel.NumG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumH":
                        if (resultModel.NumH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumI":
                        if (resultModel.NumI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumJ":
                        if (resultModel.NumJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumK":
                        if (resultModel.NumK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumL":
                        if (resultModel.NumL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumM":
                        if (resultModel.NumM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumN":
                        if (resultModel.NumN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumO":
                        if (resultModel.NumO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumP":
                        if (resultModel.NumP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumQ":
                        if (resultModel.NumQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumR":
                        if (resultModel.NumR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumS":
                        if (resultModel.NumS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumT":
                        if (resultModel.NumT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumU":
                        if (resultModel.NumU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumV":
                        if (resultModel.NumV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumW":
                        if (resultModel.NumW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumX":
                        if (resultModel.NumX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumY":
                        if (resultModel.NumY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumZ":
                        if (resultModel.NumZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateA":
                        if (resultModel.DateA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateB":
                        if (resultModel.DateB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateC":
                        if (resultModel.DateC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateD":
                        if (resultModel.DateD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateE":
                        if (resultModel.DateE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateF":
                        if (resultModel.DateF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateG":
                        if (resultModel.DateG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateH":
                        if (resultModel.DateH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateI":
                        if (resultModel.DateI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateJ":
                        if (resultModel.DateJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateK":
                        if (resultModel.DateK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateL":
                        if (resultModel.DateL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateM":
                        if (resultModel.DateM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateN":
                        if (resultModel.DateN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateO":
                        if (resultModel.DateO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateP":
                        if (resultModel.DateP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateQ":
                        if (resultModel.DateQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateR":
                        if (resultModel.DateR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateS":
                        if (resultModel.DateS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateT":
                        if (resultModel.DateT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateU":
                        if (resultModel.DateU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateV":
                        if (resultModel.DateV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateW":
                        if (resultModel.DateW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateX":
                        if (resultModel.DateX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateY":
                        if (resultModel.DateY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DateZ":
                        if (resultModel.DateZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionA":
                        if (resultModel.DescriptionA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionB":
                        if (resultModel.DescriptionB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionC":
                        if (resultModel.DescriptionC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionD":
                        if (resultModel.DescriptionD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionE":
                        if (resultModel.DescriptionE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionF":
                        if (resultModel.DescriptionF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionG":
                        if (resultModel.DescriptionG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionH":
                        if (resultModel.DescriptionH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionI":
                        if (resultModel.DescriptionI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionJ":
                        if (resultModel.DescriptionJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionK":
                        if (resultModel.DescriptionK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionL":
                        if (resultModel.DescriptionL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionM":
                        if (resultModel.DescriptionM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionN":
                        if (resultModel.DescriptionN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionO":
                        if (resultModel.DescriptionO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionP":
                        if (resultModel.DescriptionP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionQ":
                        if (resultModel.DescriptionQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionR":
                        if (resultModel.DescriptionR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionS":
                        if (resultModel.DescriptionS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionT":
                        if (resultModel.DescriptionT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionU":
                        if (resultModel.DescriptionU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionV":
                        if (resultModel.DescriptionV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionW":
                        if (resultModel.DescriptionW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionX":
                        if (resultModel.DescriptionX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionY":
                        if (resultModel.DescriptionY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DescriptionZ":
                        if (resultModel.DescriptionZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckA":
                        if (resultModel.CheckA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckB":
                        if (resultModel.CheckB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckC":
                        if (resultModel.CheckC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckD":
                        if (resultModel.CheckD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckE":
                        if (resultModel.CheckE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckF":
                        if (resultModel.CheckF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckG":
                        if (resultModel.CheckG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckH":
                        if (resultModel.CheckH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckI":
                        if (resultModel.CheckI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckJ":
                        if (resultModel.CheckJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckK":
                        if (resultModel.CheckK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckL":
                        if (resultModel.CheckL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckM":
                        if (resultModel.CheckM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckN":
                        if (resultModel.CheckN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckO":
                        if (resultModel.CheckO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckP":
                        if (resultModel.CheckP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckQ":
                        if (resultModel.CheckQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckR":
                        if (resultModel.CheckR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckS":
                        if (resultModel.CheckS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckT":
                        if (resultModel.CheckT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckU":
                        if (resultModel.CheckU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckV":
                        if (resultModel.CheckV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckW":
                        if (resultModel.CheckW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckX":
                        if (resultModel.CheckX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckY":
                        if (resultModel.CheckY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "CheckZ":
                        if (resultModel.CheckZ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsA":
                        if (resultModel.AttachmentsA_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsB":
                        if (resultModel.AttachmentsB_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsC":
                        if (resultModel.AttachmentsC_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsD":
                        if (resultModel.AttachmentsD_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsE":
                        if (resultModel.AttachmentsE_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsF":
                        if (resultModel.AttachmentsF_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsG":
                        if (resultModel.AttachmentsG_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsH":
                        if (resultModel.AttachmentsH_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsI":
                        if (resultModel.AttachmentsI_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsJ":
                        if (resultModel.AttachmentsJ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsK":
                        if (resultModel.AttachmentsK_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsL":
                        if (resultModel.AttachmentsL_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsM":
                        if (resultModel.AttachmentsM_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsN":
                        if (resultModel.AttachmentsN_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsO":
                        if (resultModel.AttachmentsO_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsP":
                        if (resultModel.AttachmentsP_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsQ":
                        if (resultModel.AttachmentsQ_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsR":
                        if (resultModel.AttachmentsR_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsS":
                        if (resultModel.AttachmentsS_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsT":
                        if (resultModel.AttachmentsT_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsU":
                        if (resultModel.AttachmentsU_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsV":
                        if (resultModel.AttachmentsV_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsW":
                        if (resultModel.AttachmentsW_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsX":
                        if (resultModel.AttachmentsX_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsY":
                        if (resultModel.AttachmentsY_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "AttachmentsZ":
                        if (resultModel.AttachmentsZ_Updated()) return Error.Types.HasNotPermission;
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
