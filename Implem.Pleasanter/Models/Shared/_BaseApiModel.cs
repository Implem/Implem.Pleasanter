using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Models.Shared
{
    [Serializable]
    public class _BaseApiModel
    {
        public decimal ApiVersion { get; set; } = 1.000M;
        public Dictionary<string, string> ClassHash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, decimal> NumHash { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, DateTime> DateHash { get; set; } = new Dictionary<string, DateTime>();
        public Dictionary<string, string> DescriptionHash { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> CheckHash { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, Attachments> AttachmentsHash { get; set; } = new Dictionary<string, Attachments>();
        public bool? VerUp;
        public string ClassA { get; set; }
        public string ClassB { get; set; }
        public string ClassC { get; set; }
        public string ClassD { get; set; }
        public string ClassE { get; set; }
        public string ClassF { get; set; }
        public string ClassG { get; set; }
        public string ClassH { get; set; }
        public string ClassI { get; set; }
        public string ClassJ { get; set; }
        public string ClassK { get; set; }
        public string ClassL { get; set; }
        public string ClassM { get; set; }
        public string ClassN { get; set; }
        public string ClassO { get; set; }
        public string ClassP { get; set; }
        public string ClassQ { get; set; }
        public string ClassR { get; set; }
        public string ClassS { get; set; }
        public string ClassT { get; set; }
        public string ClassU { get; set; }
        public string ClassV { get; set; }
        public string ClassW { get; set; }
        public string ClassX { get; set; }
        public string ClassY { get; set; }
        public string ClassZ { get; set; }
        public string Class001 { get; set; }
        public string Class002 { get; set; }
        public string Class003 { get; set; }
        public string Class004 { get; set; }
        public string Class005 { get; set; }
        public string Class006 { get; set; }
        public string Class007 { get; set; }
        public string Class008 { get; set; }
        public string Class009 { get; set; }
        public string Class010 { get; set; }
        public string Class011 { get; set; }
        public string Class012 { get; set; }
        public string Class013 { get; set; }
        public string Class014 { get; set; }
        public string Class015 { get; set; }
        public string Class016 { get; set; }
        public string Class017 { get; set; }
        public string Class018 { get; set; }
        public string Class019 { get; set; }
        public string Class020 { get; set; }
        public string Class021 { get; set; }
        public string Class022 { get; set; }
        public string Class023 { get; set; }
        public string Class024 { get; set; }
        public string Class025 { get; set; }
        public string Class026 { get; set; }
        public string Class027 { get; set; }
        public string Class028 { get; set; }
        public string Class029 { get; set; }
        public string Class030 { get; set; }
        public string Class031 { get; set; }
        public string Class032 { get; set; }
        public string Class033 { get; set; }
        public string Class034 { get; set; }
        public string Class035 { get; set; }
        public string Class036 { get; set; }
        public string Class037 { get; set; }
        public string Class038 { get; set; }
        public string Class039 { get; set; }
        public string Class040 { get; set; }
        public string Class041 { get; set; }
        public string Class042 { get; set; }
        public string Class043 { get; set; }
        public string Class044 { get; set; }
        public string Class045 { get; set; }
        public string Class046 { get; set; }
        public string Class047 { get; set; }
        public string Class048 { get; set; }
        public string Class049 { get; set; }
        public string Class050 { get; set; }
        public string Class051 { get; set; }
        public string Class052 { get; set; }
        public string Class053 { get; set; }
        public string Class054 { get; set; }
        public string Class055 { get; set; }
        public string Class056 { get; set; }
        public string Class057 { get; set; }
        public string Class058 { get; set; }
        public string Class059 { get; set; }
        public string Class060 { get; set; }
        public string Class061 { get; set; }
        public string Class062 { get; set; }
        public string Class063 { get; set; }
        public string Class064 { get; set; }
        public string Class065 { get; set; }
        public string Class066 { get; set; }
        public string Class067 { get; set; }
        public string Class068 { get; set; }
        public string Class069 { get; set; }
        public string Class070 { get; set; }
        public string Class071 { get; set; }
        public string Class072 { get; set; }
        public string Class073 { get; set; }
        public string Class074 { get; set; }
        public string Class075 { get; set; }
        public string Class076 { get; set; }
        public string Class077 { get; set; }
        public string Class078 { get; set; }
        public string Class079 { get; set; }
        public string Class080 { get; set; }
        public string Class081 { get; set; }
        public string Class082 { get; set; }
        public string Class083 { get; set; }
        public string Class084 { get; set; }
        public string Class085 { get; set; }
        public string Class086 { get; set; }
        public string Class087 { get; set; }
        public string Class088 { get; set; }
        public string Class089 { get; set; }
        public string Class090 { get; set; }
        public string Class091 { get; set; }
        public string Class092 { get; set; }
        public string Class093 { get; set; }
        public string Class094 { get; set; }
        public string Class095 { get; set; }
        public string Class096 { get; set; }
        public string Class097 { get; set; }
        public string Class098 { get; set; }
        public string Class099 { get; set; }
        public string Class100 { get; set; }
        public decimal? NumA { get; set; }
        public decimal? NumB { get; set; }
        public decimal? NumC { get; set; }
        public decimal? NumD { get; set; }
        public decimal? NumE { get; set; }
        public decimal? NumF { get; set; }
        public decimal? NumG { get; set; }
        public decimal? NumH { get; set; }
        public decimal? NumI { get; set; }
        public decimal? NumJ { get; set; }
        public decimal? NumK { get; set; }
        public decimal? NumL { get; set; }
        public decimal? NumM { get; set; }
        public decimal? NumN { get; set; }
        public decimal? NumO { get; set; }
        public decimal? NumP { get; set; }
        public decimal? NumQ { get; set; }
        public decimal? NumR { get; set; }
        public decimal? NumS { get; set; }
        public decimal? NumT { get; set; }
        public decimal? NumU { get; set; }
        public decimal? NumV { get; set; }
        public decimal? NumW { get; set; }
        public decimal? NumX { get; set; }
        public decimal? NumY { get; set; }
        public decimal? NumZ { get; set; }
        public decimal? Num001 { get; set; }
        public decimal? Num002 { get; set; }
        public decimal? Num003 { get; set; }
        public decimal? Num004 { get; set; }
        public decimal? Num005 { get; set; }
        public decimal? Num006 { get; set; }
        public decimal? Num007 { get; set; }
        public decimal? Num008 { get; set; }
        public decimal? Num009 { get; set; }
        public decimal? Num010 { get; set; }
        public decimal? Num011 { get; set; }
        public decimal? Num012 { get; set; }
        public decimal? Num013 { get; set; }
        public decimal? Num014 { get; set; }
        public decimal? Num015 { get; set; }
        public decimal? Num016 { get; set; }
        public decimal? Num017 { get; set; }
        public decimal? Num018 { get; set; }
        public decimal? Num019 { get; set; }
        public decimal? Num020 { get; set; }
        public decimal? Num021 { get; set; }
        public decimal? Num022 { get; set; }
        public decimal? Num023 { get; set; }
        public decimal? Num024 { get; set; }
        public decimal? Num025 { get; set; }
        public decimal? Num026 { get; set; }
        public decimal? Num027 { get; set; }
        public decimal? Num028 { get; set; }
        public decimal? Num029 { get; set; }
        public decimal? Num030 { get; set; }
        public decimal? Num031 { get; set; }
        public decimal? Num032 { get; set; }
        public decimal? Num033 { get; set; }
        public decimal? Num034 { get; set; }
        public decimal? Num035 { get; set; }
        public decimal? Num036 { get; set; }
        public decimal? Num037 { get; set; }
        public decimal? Num038 { get; set; }
        public decimal? Num039 { get; set; }
        public decimal? Num040 { get; set; }
        public decimal? Num041 { get; set; }
        public decimal? Num042 { get; set; }
        public decimal? Num043 { get; set; }
        public decimal? Num044 { get; set; }
        public decimal? Num045 { get; set; }
        public decimal? Num046 { get; set; }
        public decimal? Num047 { get; set; }
        public decimal? Num048 { get; set; }
        public decimal? Num049 { get; set; }
        public decimal? Num050 { get; set; }
        public decimal? Num051 { get; set; }
        public decimal? Num052 { get; set; }
        public decimal? Num053 { get; set; }
        public decimal? Num054 { get; set; }
        public decimal? Num055 { get; set; }
        public decimal? Num056 { get; set; }
        public decimal? Num057 { get; set; }
        public decimal? Num058 { get; set; }
        public decimal? Num059 { get; set; }
        public decimal? Num060 { get; set; }
        public decimal? Num061 { get; set; }
        public decimal? Num062 { get; set; }
        public decimal? Num063 { get; set; }
        public decimal? Num064 { get; set; }
        public decimal? Num065 { get; set; }
        public decimal? Num066 { get; set; }
        public decimal? Num067 { get; set; }
        public decimal? Num068 { get; set; }
        public decimal? Num069 { get; set; }
        public decimal? Num070 { get; set; }
        public decimal? Num071 { get; set; }
        public decimal? Num072 { get; set; }
        public decimal? Num073 { get; set; }
        public decimal? Num074 { get; set; }
        public decimal? Num075 { get; set; }
        public decimal? Num076 { get; set; }
        public decimal? Num077 { get; set; }
        public decimal? Num078 { get; set; }
        public decimal? Num079 { get; set; }
        public decimal? Num080 { get; set; }
        public decimal? Num081 { get; set; }
        public decimal? Num082 { get; set; }
        public decimal? Num083 { get; set; }
        public decimal? Num084 { get; set; }
        public decimal? Num085 { get; set; }
        public decimal? Num086 { get; set; }
        public decimal? Num087 { get; set; }
        public decimal? Num088 { get; set; }
        public decimal? Num089 { get; set; }
        public decimal? Num090 { get; set; }
        public decimal? Num091 { get; set; }
        public decimal? Num092 { get; set; }
        public decimal? Num093 { get; set; }
        public decimal? Num094 { get; set; }
        public decimal? Num095 { get; set; }
        public decimal? Num096 { get; set; }
        public decimal? Num097 { get; set; }
        public decimal? Num098 { get; set; }
        public decimal? Num099 { get; set; }
        public decimal? Num100 { get; set; }
        public DateTime? DateA { get; set; }
        public DateTime? DateB { get; set; }
        public DateTime? DateC { get; set; }
        public DateTime? DateD { get; set; }
        public DateTime? DateE { get; set; }
        public DateTime? DateF { get; set; }
        public DateTime? DateG { get; set; }
        public DateTime? DateH { get; set; }
        public DateTime? DateI { get; set; }
        public DateTime? DateJ { get; set; }
        public DateTime? DateK { get; set; }
        public DateTime? DateL { get; set; }
        public DateTime? DateM { get; set; }
        public DateTime? DateN { get; set; }
        public DateTime? DateO { get; set; }
        public DateTime? DateP { get; set; }
        public DateTime? DateQ { get; set; }
        public DateTime? DateR { get; set; }
        public DateTime? DateS { get; set; }
        public DateTime? DateT { get; set; }
        public DateTime? DateU { get; set; }
        public DateTime? DateV { get; set; }
        public DateTime? DateW { get; set; }
        public DateTime? DateX { get; set; }
        public DateTime? DateY { get; set; }
        public DateTime? DateZ { get; set; }
        public DateTime? Date001 { get; set; }
        public DateTime? Date002 { get; set; }
        public DateTime? Date003 { get; set; }
        public DateTime? Date004 { get; set; }
        public DateTime? Date005 { get; set; }
        public DateTime? Date006 { get; set; }
        public DateTime? Date007 { get; set; }
        public DateTime? Date008 { get; set; }
        public DateTime? Date009 { get; set; }
        public DateTime? Date010 { get; set; }
        public DateTime? Date011 { get; set; }
        public DateTime? Date012 { get; set; }
        public DateTime? Date013 { get; set; }
        public DateTime? Date014 { get; set; }
        public DateTime? Date015 { get; set; }
        public DateTime? Date016 { get; set; }
        public DateTime? Date017 { get; set; }
        public DateTime? Date018 { get; set; }
        public DateTime? Date019 { get; set; }
        public DateTime? Date020 { get; set; }
        public DateTime? Date021 { get; set; }
        public DateTime? Date022 { get; set; }
        public DateTime? Date023 { get; set; }
        public DateTime? Date024 { get; set; }
        public DateTime? Date025 { get; set; }
        public DateTime? Date026 { get; set; }
        public DateTime? Date027 { get; set; }
        public DateTime? Date028 { get; set; }
        public DateTime? Date029 { get; set; }
        public DateTime? Date030 { get; set; }
        public DateTime? Date031 { get; set; }
        public DateTime? Date032 { get; set; }
        public DateTime? Date033 { get; set; }
        public DateTime? Date034 { get; set; }
        public DateTime? Date035 { get; set; }
        public DateTime? Date036 { get; set; }
        public DateTime? Date037 { get; set; }
        public DateTime? Date038 { get; set; }
        public DateTime? Date039 { get; set; }
        public DateTime? Date040 { get; set; }
        public DateTime? Date041 { get; set; }
        public DateTime? Date042 { get; set; }
        public DateTime? Date043 { get; set; }
        public DateTime? Date044 { get; set; }
        public DateTime? Date045 { get; set; }
        public DateTime? Date046 { get; set; }
        public DateTime? Date047 { get; set; }
        public DateTime? Date048 { get; set; }
        public DateTime? Date049 { get; set; }
        public DateTime? Date050 { get; set; }
        public DateTime? Date051 { get; set; }
        public DateTime? Date052 { get; set; }
        public DateTime? Date053 { get; set; }
        public DateTime? Date054 { get; set; }
        public DateTime? Date055 { get; set; }
        public DateTime? Date056 { get; set; }
        public DateTime? Date057 { get; set; }
        public DateTime? Date058 { get; set; }
        public DateTime? Date059 { get; set; }
        public DateTime? Date060 { get; set; }
        public DateTime? Date061 { get; set; }
        public DateTime? Date062 { get; set; }
        public DateTime? Date063 { get; set; }
        public DateTime? Date064 { get; set; }
        public DateTime? Date065 { get; set; }
        public DateTime? Date066 { get; set; }
        public DateTime? Date067 { get; set; }
        public DateTime? Date068 { get; set; }
        public DateTime? Date069 { get; set; }
        public DateTime? Date070 { get; set; }
        public DateTime? Date071 { get; set; }
        public DateTime? Date072 { get; set; }
        public DateTime? Date073 { get; set; }
        public DateTime? Date074 { get; set; }
        public DateTime? Date075 { get; set; }
        public DateTime? Date076 { get; set; }
        public DateTime? Date077 { get; set; }
        public DateTime? Date078 { get; set; }
        public DateTime? Date079 { get; set; }
        public DateTime? Date080 { get; set; }
        public DateTime? Date081 { get; set; }
        public DateTime? Date082 { get; set; }
        public DateTime? Date083 { get; set; }
        public DateTime? Date084 { get; set; }
        public DateTime? Date085 { get; set; }
        public DateTime? Date086 { get; set; }
        public DateTime? Date087 { get; set; }
        public DateTime? Date088 { get; set; }
        public DateTime? Date089 { get; set; }
        public DateTime? Date090 { get; set; }
        public DateTime? Date091 { get; set; }
        public DateTime? Date092 { get; set; }
        public DateTime? Date093 { get; set; }
        public DateTime? Date094 { get; set; }
        public DateTime? Date095 { get; set; }
        public DateTime? Date096 { get; set; }
        public DateTime? Date097 { get; set; }
        public DateTime? Date098 { get; set; }
        public DateTime? Date099 { get; set; }
        public DateTime? Date100 { get; set; }
        public string DescriptionA { get; set; }
        public string DescriptionB { get; set; }
        public string DescriptionC { get; set; }
        public string DescriptionD { get; set; }
        public string DescriptionE { get; set; }
        public string DescriptionF { get; set; }
        public string DescriptionG { get; set; }
        public string DescriptionH { get; set; }
        public string DescriptionI { get; set; }
        public string DescriptionJ { get; set; }
        public string DescriptionK { get; set; }
        public string DescriptionL { get; set; }
        public string DescriptionM { get; set; }
        public string DescriptionN { get; set; }
        public string DescriptionO { get; set; }
        public string DescriptionP { get; set; }
        public string DescriptionQ { get; set; }
        public string DescriptionR { get; set; }
        public string DescriptionS { get; set; }
        public string DescriptionT { get; set; }
        public string DescriptionU { get; set; }
        public string DescriptionV { get; set; }
        public string DescriptionW { get; set; }
        public string DescriptionX { get; set; }
        public string DescriptionY { get; set; }
        public string DescriptionZ { get; set; }
        public string Description001 { get; set; }
        public string Description002 { get; set; }
        public string Description003 { get; set; }
        public string Description004 { get; set; }
        public string Description005 { get; set; }
        public string Description006 { get; set; }
        public string Description007 { get; set; }
        public string Description008 { get; set; }
        public string Description009 { get; set; }
        public string Description010 { get; set; }
        public string Description011 { get; set; }
        public string Description012 { get; set; }
        public string Description013 { get; set; }
        public string Description014 { get; set; }
        public string Description015 { get; set; }
        public string Description016 { get; set; }
        public string Description017 { get; set; }
        public string Description018 { get; set; }
        public string Description019 { get; set; }
        public string Description020 { get; set; }
        public string Description021 { get; set; }
        public string Description022 { get; set; }
        public string Description023 { get; set; }
        public string Description024 { get; set; }
        public string Description025 { get; set; }
        public string Description026 { get; set; }
        public string Description027 { get; set; }
        public string Description028 { get; set; }
        public string Description029 { get; set; }
        public string Description030 { get; set; }
        public string Description031 { get; set; }
        public string Description032 { get; set; }
        public string Description033 { get; set; }
        public string Description034 { get; set; }
        public string Description035 { get; set; }
        public string Description036 { get; set; }
        public string Description037 { get; set; }
        public string Description038 { get; set; }
        public string Description039 { get; set; }
        public string Description040 { get; set; }
        public string Description041 { get; set; }
        public string Description042 { get; set; }
        public string Description043 { get; set; }
        public string Description044 { get; set; }
        public string Description045 { get; set; }
        public string Description046 { get; set; }
        public string Description047 { get; set; }
        public string Description048 { get; set; }
        public string Description049 { get; set; }
        public string Description050 { get; set; }
        public string Description051 { get; set; }
        public string Description052 { get; set; }
        public string Description053 { get; set; }
        public string Description054 { get; set; }
        public string Description055 { get; set; }
        public string Description056 { get; set; }
        public string Description057 { get; set; }
        public string Description058 { get; set; }
        public string Description059 { get; set; }
        public string Description060 { get; set; }
        public string Description061 { get; set; }
        public string Description062 { get; set; }
        public string Description063 { get; set; }
        public string Description064 { get; set; }
        public string Description065 { get; set; }
        public string Description066 { get; set; }
        public string Description067 { get; set; }
        public string Description068 { get; set; }
        public string Description069 { get; set; }
        public string Description070 { get; set; }
        public string Description071 { get; set; }
        public string Description072 { get; set; }
        public string Description073 { get; set; }
        public string Description074 { get; set; }
        public string Description075 { get; set; }
        public string Description076 { get; set; }
        public string Description077 { get; set; }
        public string Description078 { get; set; }
        public string Description079 { get; set; }
        public string Description080 { get; set; }
        public string Description081 { get; set; }
        public string Description082 { get; set; }
        public string Description083 { get; set; }
        public string Description084 { get; set; }
        public string Description085 { get; set; }
        public string Description086 { get; set; }
        public string Description087 { get; set; }
        public string Description088 { get; set; }
        public string Description089 { get; set; }
        public string Description090 { get; set; }
        public string Description091 { get; set; }
        public string Description092 { get; set; }
        public string Description093 { get; set; }
        public string Description094 { get; set; }
        public string Description095 { get; set; }
        public string Description096 { get; set; }
        public string Description097 { get; set; }
        public string Description098 { get; set; }
        public string Description099 { get; set; }
        public string Description100 { get; set; }
        public bool? CheckA { get; set; }
        public bool? CheckB { get; set; }
        public bool? CheckC { get; set; }
        public bool? CheckD { get; set; }
        public bool? CheckE { get; set; }
        public bool? CheckF { get; set; }
        public bool? CheckG { get; set; }
        public bool? CheckH { get; set; }
        public bool? CheckI { get; set; }
        public bool? CheckJ { get; set; }
        public bool? CheckK { get; set; }
        public bool? CheckL { get; set; }
        public bool? CheckM { get; set; }
        public bool? CheckN { get; set; }
        public bool? CheckO { get; set; }
        public bool? CheckP { get; set; }
        public bool? CheckQ { get; set; }
        public bool? CheckR { get; set; }
        public bool? CheckS { get; set; }
        public bool? CheckT { get; set; }
        public bool? CheckU { get; set; }
        public bool? CheckV { get; set; }
        public bool? CheckW { get; set; }
        public bool? CheckX { get; set; }
        public bool? CheckY { get; set; }
        public bool? CheckZ { get; set; }
        public bool? Check001 { get; set; }
        public bool? Check002 { get; set; }
        public bool? Check003 { get; set; }
        public bool? Check004 { get; set; }
        public bool? Check005 { get; set; }
        public bool? Check006 { get; set; }
        public bool? Check007 { get; set; }
        public bool? Check008 { get; set; }
        public bool? Check009 { get; set; }
        public bool? Check010 { get; set; }
        public bool? Check011 { get; set; }
        public bool? Check012 { get; set; }
        public bool? Check013 { get; set; }
        public bool? Check014 { get; set; }
        public bool? Check015 { get; set; }
        public bool? Check016 { get; set; }
        public bool? Check017 { get; set; }
        public bool? Check018 { get; set; }
        public bool? Check019 { get; set; }
        public bool? Check020 { get; set; }
        public bool? Check021 { get; set; }
        public bool? Check022 { get; set; }
        public bool? Check023 { get; set; }
        public bool? Check024 { get; set; }
        public bool? Check025 { get; set; }
        public bool? Check026 { get; set; }
        public bool? Check027 { get; set; }
        public bool? Check028 { get; set; }
        public bool? Check029 { get; set; }
        public bool? Check030 { get; set; }
        public bool? Check031 { get; set; }
        public bool? Check032 { get; set; }
        public bool? Check033 { get; set; }
        public bool? Check034 { get; set; }
        public bool? Check035 { get; set; }
        public bool? Check036 { get; set; }
        public bool? Check037 { get; set; }
        public bool? Check038 { get; set; }
        public bool? Check039 { get; set; }
        public bool? Check040 { get; set; }
        public bool? Check041 { get; set; }
        public bool? Check042 { get; set; }
        public bool? Check043 { get; set; }
        public bool? Check044 { get; set; }
        public bool? Check045 { get; set; }
        public bool? Check046 { get; set; }
        public bool? Check047 { get; set; }
        public bool? Check048 { get; set; }
        public bool? Check049 { get; set; }
        public bool? Check050 { get; set; }
        public bool? Check051 { get; set; }
        public bool? Check052 { get; set; }
        public bool? Check053 { get; set; }
        public bool? Check054 { get; set; }
        public bool? Check055 { get; set; }
        public bool? Check056 { get; set; }
        public bool? Check057 { get; set; }
        public bool? Check058 { get; set; }
        public bool? Check059 { get; set; }
        public bool? Check060 { get; set; }
        public bool? Check061 { get; set; }
        public bool? Check062 { get; set; }
        public bool? Check063 { get; set; }
        public bool? Check064 { get; set; }
        public bool? Check065 { get; set; }
        public bool? Check066 { get; set; }
        public bool? Check067 { get; set; }
        public bool? Check068 { get; set; }
        public bool? Check069 { get; set; }
        public bool? Check070 { get; set; }
        public bool? Check071 { get; set; }
        public bool? Check072 { get; set; }
        public bool? Check073 { get; set; }
        public bool? Check074 { get; set; }
        public bool? Check075 { get; set; }
        public bool? Check076 { get; set; }
        public bool? Check077 { get; set; }
        public bool? Check078 { get; set; }
        public bool? Check079 { get; set; }
        public bool? Check080 { get; set; }
        public bool? Check081 { get; set; }
        public bool? Check082 { get; set; }
        public bool? Check083 { get; set; }
        public bool? Check084 { get; set; }
        public bool? Check085 { get; set; }
        public bool? Check086 { get; set; }
        public bool? Check087 { get; set; }
        public bool? Check088 { get; set; }
        public bool? Check089 { get; set; }
        public bool? Check090 { get; set; }
        public bool? Check091 { get; set; }
        public bool? Check092 { get; set; }
        public bool? Check093 { get; set; }
        public bool? Check094 { get; set; }
        public bool? Check095 { get; set; }
        public bool? Check096 { get; set; }
        public bool? Check097 { get; set; }
        public bool? Check098 { get; set; }
        public bool? Check099 { get; set; }
        public bool? Check100 { get; set; }
        public string AttachmentsA { get; set; }
        public string AttachmentsB { get; set; }
        public string AttachmentsC { get; set; }
        public string AttachmentsD { get; set; }
        public string AttachmentsE { get; set; }
        public string AttachmentsF { get; set; }
        public string AttachmentsG { get; set; }
        public string AttachmentsH { get; set; }
        public string AttachmentsI { get; set; }
        public string AttachmentsJ { get; set; }
        public string AttachmentsK { get; set; }
        public string AttachmentsL { get; set; }
        public string AttachmentsM { get; set; }
        public string AttachmentsN { get; set; }
        public string AttachmentsO { get; set; }
        public string AttachmentsP { get; set; }
        public string AttachmentsQ { get; set; }
        public string AttachmentsR { get; set; }
        public string AttachmentsS { get; set; }
        public string AttachmentsT { get; set; }
        public string AttachmentsU { get; set; }
        public string AttachmentsV { get; set; }
        public string AttachmentsW { get; set; }
        public string AttachmentsX { get; set; }
        public string AttachmentsY { get; set; }
        public string AttachmentsZ { get; set; }
        public string Attachments001 { get; set; }
        public string Attachments002 { get; set; }
        public string Attachments003 { get; set; }
        public string Attachments004 { get; set; }
        public string Attachments005 { get; set; }
        public string Attachments006 { get; set; }
        public string Attachments007 { get; set; }
        public string Attachments008 { get; set; }
        public string Attachments009 { get; set; }
        public string Attachments010 { get; set; }
        public string Attachments011 { get; set; }
        public string Attachments012 { get; set; }
        public string Attachments013 { get; set; }
        public string Attachments014 { get; set; }
        public string Attachments015 { get; set; }
        public string Attachments016 { get; set; }
        public string Attachments017 { get; set; }
        public string Attachments018 { get; set; }
        public string Attachments019 { get; set; }
        public string Attachments020 { get; set; }
        public string Attachments021 { get; set; }
        public string Attachments022 { get; set; }
        public string Attachments023 { get; set; }
        public string Attachments024 { get; set; }
        public string Attachments025 { get; set; }
        public string Attachments026 { get; set; }
        public string Attachments027 { get; set; }
        public string Attachments028 { get; set; }
        public string Attachments029 { get; set; }
        public string Attachments030 { get; set; }
        public string Attachments031 { get; set; }
        public string Attachments032 { get; set; }
        public string Attachments033 { get; set; }
        public string Attachments034 { get; set; }
        public string Attachments035 { get; set; }
        public string Attachments036 { get; set; }
        public string Attachments037 { get; set; }
        public string Attachments038 { get; set; }
        public string Attachments039 { get; set; }
        public string Attachments040 { get; set; }
        public string Attachments041 { get; set; }
        public string Attachments042 { get; set; }
        public string Attachments043 { get; set; }
        public string Attachments044 { get; set; }
        public string Attachments045 { get; set; }
        public string Attachments046 { get; set; }
        public string Attachments047 { get; set; }
        public string Attachments048 { get; set; }
        public string Attachments049 { get; set; }
        public string Attachments050 { get; set; }
        public string Attachments051 { get; set; }
        public string Attachments052 { get; set; }
        public string Attachments053 { get; set; }
        public string Attachments054 { get; set; }
        public string Attachments055 { get; set; }
        public string Attachments056 { get; set; }
        public string Attachments057 { get; set; }
        public string Attachments058 { get; set; }
        public string Attachments059 { get; set; }
        public string Attachments060 { get; set; }
        public string Attachments061 { get; set; }
        public string Attachments062 { get; set; }
        public string Attachments063 { get; set; }
        public string Attachments064 { get; set; }
        public string Attachments065 { get; set; }
        public string Attachments066 { get; set; }
        public string Attachments067 { get; set; }
        public string Attachments068 { get; set; }
        public string Attachments069 { get; set; }
        public string Attachments070 { get; set; }
        public string Attachments071 { get; set; }
        public string Attachments072 { get; set; }
        public string Attachments073 { get; set; }
        public string Attachments074 { get; set; }
        public string Attachments075 { get; set; }
        public string Attachments076 { get; set; }
        public string Attachments077 { get; set; }
        public string Attachments078 { get; set; }
        public string Attachments079 { get; set; }
        public string Attachments080 { get; set; }
        public string Attachments081 { get; set; }
        public string Attachments082 { get; set; }
        public string Attachments083 { get; set; }
        public string Attachments084 { get; set; }
        public string Attachments085 { get; set; }
        public string Attachments086 { get; set; }
        public string Attachments087 { get; set; }
        public string Attachments088 { get; set; }
        public string Attachments089 { get; set; }
        public string Attachments090 { get; set; }
        public string Attachments091 { get; set; }
        public string Attachments092 { get; set; }
        public string Attachments093 { get; set; }
        public string Attachments094 { get; set; }
        public string Attachments095 { get; set; }
        public string Attachments096 { get; set; }
        public string Attachments097 { get; set; }
        public string Attachments098 { get; set; }
        public string Attachments099 { get; set; }
        public string Attachments100 { get; set; }

        public _BaseApiModel()
        {
        }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            if (ApiVersion < 1.100M)
            {
                if (ClassHash.ContainsKey("ClassA")) ClassA = Class(columnName: "ClassA");
                if (ClassHash.ContainsKey("ClassB")) ClassB = Class(columnName: "ClassB");
                if (ClassHash.ContainsKey("ClassC")) ClassC = Class(columnName: "ClassC");
                if (ClassHash.ContainsKey("ClassD")) ClassD = Class(columnName: "ClassD");
                if (ClassHash.ContainsKey("ClassE")) ClassE = Class(columnName: "ClassE");
                if (ClassHash.ContainsKey("ClassF")) ClassF = Class(columnName: "ClassF");
                if (ClassHash.ContainsKey("ClassG")) ClassG = Class(columnName: "ClassG");
                if (ClassHash.ContainsKey("ClassH")) ClassH = Class(columnName: "ClassH");
                if (ClassHash.ContainsKey("ClassI")) ClassI = Class(columnName: "ClassI");
                if (ClassHash.ContainsKey("ClassJ")) ClassJ = Class(columnName: "ClassJ");
                if (ClassHash.ContainsKey("ClassK")) ClassK = Class(columnName: "ClassK");
                if (ClassHash.ContainsKey("ClassL")) ClassL = Class(columnName: "ClassL");
                if (ClassHash.ContainsKey("ClassM")) ClassM = Class(columnName: "ClassM");
                if (ClassHash.ContainsKey("ClassN")) ClassN = Class(columnName: "ClassN");
                if (ClassHash.ContainsKey("ClassO")) ClassO = Class(columnName: "ClassO");
                if (ClassHash.ContainsKey("ClassP")) ClassP = Class(columnName: "ClassP");
                if (ClassHash.ContainsKey("ClassQ")) ClassQ = Class(columnName: "ClassQ");
                if (ClassHash.ContainsKey("ClassR")) ClassR = Class(columnName: "ClassR");
                if (ClassHash.ContainsKey("ClassS")) ClassS = Class(columnName: "ClassS");
                if (ClassHash.ContainsKey("ClassT")) ClassT = Class(columnName: "ClassT");
                if (ClassHash.ContainsKey("ClassU")) ClassU = Class(columnName: "ClassU");
                if (ClassHash.ContainsKey("ClassV")) ClassV = Class(columnName: "ClassV");
                if (ClassHash.ContainsKey("ClassW")) ClassW = Class(columnName: "ClassW");
                if (ClassHash.ContainsKey("ClassX")) ClassX = Class(columnName: "ClassX");
                if (ClassHash.ContainsKey("ClassY")) ClassY = Class(columnName: "ClassY");
                if (ClassHash.ContainsKey("ClassZ")) ClassZ = Class(columnName: "ClassZ");
                if (ClassHash.ContainsKey("Class001")) Class001 = Class(columnName: "Class001");
                if (ClassHash.ContainsKey("Class002")) Class002 = Class(columnName: "Class002");
                if (ClassHash.ContainsKey("Class003")) Class003 = Class(columnName: "Class003");
                if (ClassHash.ContainsKey("Class004")) Class004 = Class(columnName: "Class004");
                if (ClassHash.ContainsKey("Class005")) Class005 = Class(columnName: "Class005");
                if (ClassHash.ContainsKey("Class006")) Class006 = Class(columnName: "Class006");
                if (ClassHash.ContainsKey("Class007")) Class007 = Class(columnName: "Class007");
                if (ClassHash.ContainsKey("Class008")) Class008 = Class(columnName: "Class008");
                if (ClassHash.ContainsKey("Class009")) Class009 = Class(columnName: "Class009");
                if (ClassHash.ContainsKey("Class010")) Class010 = Class(columnName: "Class010");
                if (ClassHash.ContainsKey("Class011")) Class011 = Class(columnName: "Class011");
                if (ClassHash.ContainsKey("Class012")) Class012 = Class(columnName: "Class012");
                if (ClassHash.ContainsKey("Class013")) Class013 = Class(columnName: "Class013");
                if (ClassHash.ContainsKey("Class014")) Class014 = Class(columnName: "Class014");
                if (ClassHash.ContainsKey("Class015")) Class015 = Class(columnName: "Class015");
                if (ClassHash.ContainsKey("Class016")) Class016 = Class(columnName: "Class016");
                if (ClassHash.ContainsKey("Class017")) Class017 = Class(columnName: "Class017");
                if (ClassHash.ContainsKey("Class018")) Class018 = Class(columnName: "Class018");
                if (ClassHash.ContainsKey("Class019")) Class019 = Class(columnName: "Class019");
                if (ClassHash.ContainsKey("Class020")) Class020 = Class(columnName: "Class020");
                if (ClassHash.ContainsKey("Class021")) Class021 = Class(columnName: "Class021");
                if (ClassHash.ContainsKey("Class022")) Class022 = Class(columnName: "Class022");
                if (ClassHash.ContainsKey("Class023")) Class023 = Class(columnName: "Class023");
                if (ClassHash.ContainsKey("Class024")) Class024 = Class(columnName: "Class024");
                if (ClassHash.ContainsKey("Class025")) Class025 = Class(columnName: "Class025");
                if (ClassHash.ContainsKey("Class026")) Class026 = Class(columnName: "Class026");
                if (ClassHash.ContainsKey("Class027")) Class027 = Class(columnName: "Class027");
                if (ClassHash.ContainsKey("Class028")) Class028 = Class(columnName: "Class028");
                if (ClassHash.ContainsKey("Class029")) Class029 = Class(columnName: "Class029");
                if (ClassHash.ContainsKey("Class030")) Class030 = Class(columnName: "Class030");
                if (ClassHash.ContainsKey("Class031")) Class031 = Class(columnName: "Class031");
                if (ClassHash.ContainsKey("Class032")) Class032 = Class(columnName: "Class032");
                if (ClassHash.ContainsKey("Class033")) Class033 = Class(columnName: "Class033");
                if (ClassHash.ContainsKey("Class034")) Class034 = Class(columnName: "Class034");
                if (ClassHash.ContainsKey("Class035")) Class035 = Class(columnName: "Class035");
                if (ClassHash.ContainsKey("Class036")) Class036 = Class(columnName: "Class036");
                if (ClassHash.ContainsKey("Class037")) Class037 = Class(columnName: "Class037");
                if (ClassHash.ContainsKey("Class038")) Class038 = Class(columnName: "Class038");
                if (ClassHash.ContainsKey("Class039")) Class039 = Class(columnName: "Class039");
                if (ClassHash.ContainsKey("Class040")) Class040 = Class(columnName: "Class040");
                if (ClassHash.ContainsKey("Class041")) Class041 = Class(columnName: "Class041");
                if (ClassHash.ContainsKey("Class042")) Class042 = Class(columnName: "Class042");
                if (ClassHash.ContainsKey("Class043")) Class043 = Class(columnName: "Class043");
                if (ClassHash.ContainsKey("Class044")) Class044 = Class(columnName: "Class044");
                if (ClassHash.ContainsKey("Class045")) Class045 = Class(columnName: "Class045");
                if (ClassHash.ContainsKey("Class046")) Class046 = Class(columnName: "Class046");
                if (ClassHash.ContainsKey("Class047")) Class047 = Class(columnName: "Class047");
                if (ClassHash.ContainsKey("Class048")) Class048 = Class(columnName: "Class048");
                if (ClassHash.ContainsKey("Class049")) Class049 = Class(columnName: "Class049");
                if (ClassHash.ContainsKey("Class050")) Class050 = Class(columnName: "Class050");
                if (ClassHash.ContainsKey("Class051")) Class051 = Class(columnName: "Class051");
                if (ClassHash.ContainsKey("Class052")) Class052 = Class(columnName: "Class052");
                if (ClassHash.ContainsKey("Class053")) Class053 = Class(columnName: "Class053");
                if (ClassHash.ContainsKey("Class054")) Class054 = Class(columnName: "Class054");
                if (ClassHash.ContainsKey("Class055")) Class055 = Class(columnName: "Class055");
                if (ClassHash.ContainsKey("Class056")) Class056 = Class(columnName: "Class056");
                if (ClassHash.ContainsKey("Class057")) Class057 = Class(columnName: "Class057");
                if (ClassHash.ContainsKey("Class058")) Class058 = Class(columnName: "Class058");
                if (ClassHash.ContainsKey("Class059")) Class059 = Class(columnName: "Class059");
                if (ClassHash.ContainsKey("Class060")) Class060 = Class(columnName: "Class060");
                if (ClassHash.ContainsKey("Class061")) Class061 = Class(columnName: "Class061");
                if (ClassHash.ContainsKey("Class062")) Class062 = Class(columnName: "Class062");
                if (ClassHash.ContainsKey("Class063")) Class063 = Class(columnName: "Class063");
                if (ClassHash.ContainsKey("Class064")) Class064 = Class(columnName: "Class064");
                if (ClassHash.ContainsKey("Class065")) Class065 = Class(columnName: "Class065");
                if (ClassHash.ContainsKey("Class066")) Class066 = Class(columnName: "Class066");
                if (ClassHash.ContainsKey("Class067")) Class067 = Class(columnName: "Class067");
                if (ClassHash.ContainsKey("Class068")) Class068 = Class(columnName: "Class068");
                if (ClassHash.ContainsKey("Class069")) Class069 = Class(columnName: "Class069");
                if (ClassHash.ContainsKey("Class070")) Class070 = Class(columnName: "Class070");
                if (ClassHash.ContainsKey("Class071")) Class071 = Class(columnName: "Class071");
                if (ClassHash.ContainsKey("Class072")) Class072 = Class(columnName: "Class072");
                if (ClassHash.ContainsKey("Class073")) Class073 = Class(columnName: "Class073");
                if (ClassHash.ContainsKey("Class074")) Class074 = Class(columnName: "Class074");
                if (ClassHash.ContainsKey("Class075")) Class075 = Class(columnName: "Class075");
                if (ClassHash.ContainsKey("Class076")) Class076 = Class(columnName: "Class076");
                if (ClassHash.ContainsKey("Class077")) Class077 = Class(columnName: "Class077");
                if (ClassHash.ContainsKey("Class078")) Class078 = Class(columnName: "Class078");
                if (ClassHash.ContainsKey("Class079")) Class079 = Class(columnName: "Class079");
                if (ClassHash.ContainsKey("Class080")) Class080 = Class(columnName: "Class080");
                if (ClassHash.ContainsKey("Class081")) Class081 = Class(columnName: "Class081");
                if (ClassHash.ContainsKey("Class082")) Class082 = Class(columnName: "Class082");
                if (ClassHash.ContainsKey("Class083")) Class083 = Class(columnName: "Class083");
                if (ClassHash.ContainsKey("Class084")) Class084 = Class(columnName: "Class084");
                if (ClassHash.ContainsKey("Class085")) Class085 = Class(columnName: "Class085");
                if (ClassHash.ContainsKey("Class086")) Class086 = Class(columnName: "Class086");
                if (ClassHash.ContainsKey("Class087")) Class087 = Class(columnName: "Class087");
                if (ClassHash.ContainsKey("Class088")) Class088 = Class(columnName: "Class088");
                if (ClassHash.ContainsKey("Class089")) Class089 = Class(columnName: "Class089");
                if (ClassHash.ContainsKey("Class090")) Class090 = Class(columnName: "Class090");
                if (ClassHash.ContainsKey("Class091")) Class091 = Class(columnName: "Class091");
                if (ClassHash.ContainsKey("Class092")) Class092 = Class(columnName: "Class092");
                if (ClassHash.ContainsKey("Class093")) Class093 = Class(columnName: "Class093");
                if (ClassHash.ContainsKey("Class094")) Class094 = Class(columnName: "Class094");
                if (ClassHash.ContainsKey("Class095")) Class095 = Class(columnName: "Class095");
                if (ClassHash.ContainsKey("Class096")) Class096 = Class(columnName: "Class096");
                if (ClassHash.ContainsKey("Class097")) Class097 = Class(columnName: "Class097");
                if (ClassHash.ContainsKey("Class098")) Class098 = Class(columnName: "Class098");
                if (ClassHash.ContainsKey("Class099")) Class099 = Class(columnName: "Class099");
                if (ClassHash.ContainsKey("Class100")) Class100 = Class(columnName: "Class100");
                if (NumHash.ContainsKey("NumA")) NumA = Num(columnName: "NumA");
                if (NumHash.ContainsKey("NumB")) NumB = Num(columnName: "NumB");
                if (NumHash.ContainsKey("NumC")) NumC = Num(columnName: "NumC");
                if (NumHash.ContainsKey("NumD")) NumD = Num(columnName: "NumD");
                if (NumHash.ContainsKey("NumE")) NumE = Num(columnName: "NumE");
                if (NumHash.ContainsKey("NumF")) NumF = Num(columnName: "NumF");
                if (NumHash.ContainsKey("NumG")) NumG = Num(columnName: "NumG");
                if (NumHash.ContainsKey("NumH")) NumH = Num(columnName: "NumH");
                if (NumHash.ContainsKey("NumI")) NumI = Num(columnName: "NumI");
                if (NumHash.ContainsKey("NumJ")) NumJ = Num(columnName: "NumJ");
                if (NumHash.ContainsKey("NumK")) NumK = Num(columnName: "NumK");
                if (NumHash.ContainsKey("NumL")) NumL = Num(columnName: "NumL");
                if (NumHash.ContainsKey("NumM")) NumM = Num(columnName: "NumM");
                if (NumHash.ContainsKey("NumN")) NumN = Num(columnName: "NumN");
                if (NumHash.ContainsKey("NumO")) NumO = Num(columnName: "NumO");
                if (NumHash.ContainsKey("NumP")) NumP = Num(columnName: "NumP");
                if (NumHash.ContainsKey("NumQ")) NumQ = Num(columnName: "NumQ");
                if (NumHash.ContainsKey("NumR")) NumR = Num(columnName: "NumR");
                if (NumHash.ContainsKey("NumS")) NumS = Num(columnName: "NumS");
                if (NumHash.ContainsKey("NumT")) NumT = Num(columnName: "NumT");
                if (NumHash.ContainsKey("NumU")) NumU = Num(columnName: "NumU");
                if (NumHash.ContainsKey("NumV")) NumV = Num(columnName: "NumV");
                if (NumHash.ContainsKey("NumW")) NumW = Num(columnName: "NumW");
                if (NumHash.ContainsKey("NumX")) NumX = Num(columnName: "NumX");
                if (NumHash.ContainsKey("NumY")) NumY = Num(columnName: "NumY");
                if (NumHash.ContainsKey("NumZ")) NumZ = Num(columnName: "NumZ");
                if (NumHash.ContainsKey("Num001")) Num001 = Num(columnName: "Num001");
                if (NumHash.ContainsKey("Num002")) Num002 = Num(columnName: "Num002");
                if (NumHash.ContainsKey("Num003")) Num003 = Num(columnName: "Num003");
                if (NumHash.ContainsKey("Num004")) Num004 = Num(columnName: "Num004");
                if (NumHash.ContainsKey("Num005")) Num005 = Num(columnName: "Num005");
                if (NumHash.ContainsKey("Num006")) Num006 = Num(columnName: "Num006");
                if (NumHash.ContainsKey("Num007")) Num007 = Num(columnName: "Num007");
                if (NumHash.ContainsKey("Num008")) Num008 = Num(columnName: "Num008");
                if (NumHash.ContainsKey("Num009")) Num009 = Num(columnName: "Num009");
                if (NumHash.ContainsKey("Num010")) Num010 = Num(columnName: "Num010");
                if (NumHash.ContainsKey("Num011")) Num011 = Num(columnName: "Num011");
                if (NumHash.ContainsKey("Num012")) Num012 = Num(columnName: "Num012");
                if (NumHash.ContainsKey("Num013")) Num013 = Num(columnName: "Num013");
                if (NumHash.ContainsKey("Num014")) Num014 = Num(columnName: "Num014");
                if (NumHash.ContainsKey("Num015")) Num015 = Num(columnName: "Num015");
                if (NumHash.ContainsKey("Num016")) Num016 = Num(columnName: "Num016");
                if (NumHash.ContainsKey("Num017")) Num017 = Num(columnName: "Num017");
                if (NumHash.ContainsKey("Num018")) Num018 = Num(columnName: "Num018");
                if (NumHash.ContainsKey("Num019")) Num019 = Num(columnName: "Num019");
                if (NumHash.ContainsKey("Num020")) Num020 = Num(columnName: "Num020");
                if (NumHash.ContainsKey("Num021")) Num021 = Num(columnName: "Num021");
                if (NumHash.ContainsKey("Num022")) Num022 = Num(columnName: "Num022");
                if (NumHash.ContainsKey("Num023")) Num023 = Num(columnName: "Num023");
                if (NumHash.ContainsKey("Num024")) Num024 = Num(columnName: "Num024");
                if (NumHash.ContainsKey("Num025")) Num025 = Num(columnName: "Num025");
                if (NumHash.ContainsKey("Num026")) Num026 = Num(columnName: "Num026");
                if (NumHash.ContainsKey("Num027")) Num027 = Num(columnName: "Num027");
                if (NumHash.ContainsKey("Num028")) Num028 = Num(columnName: "Num028");
                if (NumHash.ContainsKey("Num029")) Num029 = Num(columnName: "Num029");
                if (NumHash.ContainsKey("Num030")) Num030 = Num(columnName: "Num030");
                if (NumHash.ContainsKey("Num031")) Num031 = Num(columnName: "Num031");
                if (NumHash.ContainsKey("Num032")) Num032 = Num(columnName: "Num032");
                if (NumHash.ContainsKey("Num033")) Num033 = Num(columnName: "Num033");
                if (NumHash.ContainsKey("Num034")) Num034 = Num(columnName: "Num034");
                if (NumHash.ContainsKey("Num035")) Num035 = Num(columnName: "Num035");
                if (NumHash.ContainsKey("Num036")) Num036 = Num(columnName: "Num036");
                if (NumHash.ContainsKey("Num037")) Num037 = Num(columnName: "Num037");
                if (NumHash.ContainsKey("Num038")) Num038 = Num(columnName: "Num038");
                if (NumHash.ContainsKey("Num039")) Num039 = Num(columnName: "Num039");
                if (NumHash.ContainsKey("Num040")) Num040 = Num(columnName: "Num040");
                if (NumHash.ContainsKey("Num041")) Num041 = Num(columnName: "Num041");
                if (NumHash.ContainsKey("Num042")) Num042 = Num(columnName: "Num042");
                if (NumHash.ContainsKey("Num043")) Num043 = Num(columnName: "Num043");
                if (NumHash.ContainsKey("Num044")) Num044 = Num(columnName: "Num044");
                if (NumHash.ContainsKey("Num045")) Num045 = Num(columnName: "Num045");
                if (NumHash.ContainsKey("Num046")) Num046 = Num(columnName: "Num046");
                if (NumHash.ContainsKey("Num047")) Num047 = Num(columnName: "Num047");
                if (NumHash.ContainsKey("Num048")) Num048 = Num(columnName: "Num048");
                if (NumHash.ContainsKey("Num049")) Num049 = Num(columnName: "Num049");
                if (NumHash.ContainsKey("Num050")) Num050 = Num(columnName: "Num050");
                if (NumHash.ContainsKey("Num051")) Num051 = Num(columnName: "Num051");
                if (NumHash.ContainsKey("Num052")) Num052 = Num(columnName: "Num052");
                if (NumHash.ContainsKey("Num053")) Num053 = Num(columnName: "Num053");
                if (NumHash.ContainsKey("Num054")) Num054 = Num(columnName: "Num054");
                if (NumHash.ContainsKey("Num055")) Num055 = Num(columnName: "Num055");
                if (NumHash.ContainsKey("Num056")) Num056 = Num(columnName: "Num056");
                if (NumHash.ContainsKey("Num057")) Num057 = Num(columnName: "Num057");
                if (NumHash.ContainsKey("Num058")) Num058 = Num(columnName: "Num058");
                if (NumHash.ContainsKey("Num059")) Num059 = Num(columnName: "Num059");
                if (NumHash.ContainsKey("Num060")) Num060 = Num(columnName: "Num060");
                if (NumHash.ContainsKey("Num061")) Num061 = Num(columnName: "Num061");
                if (NumHash.ContainsKey("Num062")) Num062 = Num(columnName: "Num062");
                if (NumHash.ContainsKey("Num063")) Num063 = Num(columnName: "Num063");
                if (NumHash.ContainsKey("Num064")) Num064 = Num(columnName: "Num064");
                if (NumHash.ContainsKey("Num065")) Num065 = Num(columnName: "Num065");
                if (NumHash.ContainsKey("Num066")) Num066 = Num(columnName: "Num066");
                if (NumHash.ContainsKey("Num067")) Num067 = Num(columnName: "Num067");
                if (NumHash.ContainsKey("Num068")) Num068 = Num(columnName: "Num068");
                if (NumHash.ContainsKey("Num069")) Num069 = Num(columnName: "Num069");
                if (NumHash.ContainsKey("Num070")) Num070 = Num(columnName: "Num070");
                if (NumHash.ContainsKey("Num071")) Num071 = Num(columnName: "Num071");
                if (NumHash.ContainsKey("Num072")) Num072 = Num(columnName: "Num072");
                if (NumHash.ContainsKey("Num073")) Num073 = Num(columnName: "Num073");
                if (NumHash.ContainsKey("Num074")) Num074 = Num(columnName: "Num074");
                if (NumHash.ContainsKey("Num075")) Num075 = Num(columnName: "Num075");
                if (NumHash.ContainsKey("Num076")) Num076 = Num(columnName: "Num076");
                if (NumHash.ContainsKey("Num077")) Num077 = Num(columnName: "Num077");
                if (NumHash.ContainsKey("Num078")) Num078 = Num(columnName: "Num078");
                if (NumHash.ContainsKey("Num079")) Num079 = Num(columnName: "Num079");
                if (NumHash.ContainsKey("Num080")) Num080 = Num(columnName: "Num080");
                if (NumHash.ContainsKey("Num081")) Num081 = Num(columnName: "Num081");
                if (NumHash.ContainsKey("Num082")) Num082 = Num(columnName: "Num082");
                if (NumHash.ContainsKey("Num083")) Num083 = Num(columnName: "Num083");
                if (NumHash.ContainsKey("Num084")) Num084 = Num(columnName: "Num084");
                if (NumHash.ContainsKey("Num085")) Num085 = Num(columnName: "Num085");
                if (NumHash.ContainsKey("Num086")) Num086 = Num(columnName: "Num086");
                if (NumHash.ContainsKey("Num087")) Num087 = Num(columnName: "Num087");
                if (NumHash.ContainsKey("Num088")) Num088 = Num(columnName: "Num088");
                if (NumHash.ContainsKey("Num089")) Num089 = Num(columnName: "Num089");
                if (NumHash.ContainsKey("Num090")) Num090 = Num(columnName: "Num090");
                if (NumHash.ContainsKey("Num091")) Num091 = Num(columnName: "Num091");
                if (NumHash.ContainsKey("Num092")) Num092 = Num(columnName: "Num092");
                if (NumHash.ContainsKey("Num093")) Num093 = Num(columnName: "Num093");
                if (NumHash.ContainsKey("Num094")) Num094 = Num(columnName: "Num094");
                if (NumHash.ContainsKey("Num095")) Num095 = Num(columnName: "Num095");
                if (NumHash.ContainsKey("Num096")) Num096 = Num(columnName: "Num096");
                if (NumHash.ContainsKey("Num097")) Num097 = Num(columnName: "Num097");
                if (NumHash.ContainsKey("Num098")) Num098 = Num(columnName: "Num098");
                if (NumHash.ContainsKey("Num099")) Num099 = Num(columnName: "Num099");
                if (NumHash.ContainsKey("Num100")) Num100 = Num(columnName: "Num100");
                if (DateHash.ContainsKey("DateA")) DateA = Date(columnName: "DateA");
                if (DateHash.ContainsKey("DateB")) DateB = Date(columnName: "DateB");
                if (DateHash.ContainsKey("DateC")) DateC = Date(columnName: "DateC");
                if (DateHash.ContainsKey("DateD")) DateD = Date(columnName: "DateD");
                if (DateHash.ContainsKey("DateE")) DateE = Date(columnName: "DateE");
                if (DateHash.ContainsKey("DateF")) DateF = Date(columnName: "DateF");
                if (DateHash.ContainsKey("DateG")) DateG = Date(columnName: "DateG");
                if (DateHash.ContainsKey("DateH")) DateH = Date(columnName: "DateH");
                if (DateHash.ContainsKey("DateI")) DateI = Date(columnName: "DateI");
                if (DateHash.ContainsKey("DateJ")) DateJ = Date(columnName: "DateJ");
                if (DateHash.ContainsKey("DateK")) DateK = Date(columnName: "DateK");
                if (DateHash.ContainsKey("DateL")) DateL = Date(columnName: "DateL");
                if (DateHash.ContainsKey("DateM")) DateM = Date(columnName: "DateM");
                if (DateHash.ContainsKey("DateN")) DateN = Date(columnName: "DateN");
                if (DateHash.ContainsKey("DateO")) DateO = Date(columnName: "DateO");
                if (DateHash.ContainsKey("DateP")) DateP = Date(columnName: "DateP");
                if (DateHash.ContainsKey("DateQ")) DateQ = Date(columnName: "DateQ");
                if (DateHash.ContainsKey("DateR")) DateR = Date(columnName: "DateR");
                if (DateHash.ContainsKey("DateS")) DateS = Date(columnName: "DateS");
                if (DateHash.ContainsKey("DateT")) DateT = Date(columnName: "DateT");
                if (DateHash.ContainsKey("DateU")) DateU = Date(columnName: "DateU");
                if (DateHash.ContainsKey("DateV")) DateV = Date(columnName: "DateV");
                if (DateHash.ContainsKey("DateW")) DateW = Date(columnName: "DateW");
                if (DateHash.ContainsKey("DateX")) DateX = Date(columnName: "DateX");
                if (DateHash.ContainsKey("DateY")) DateY = Date(columnName: "DateY");
                if (DateHash.ContainsKey("DateZ")) DateZ = Date(columnName: "DateZ");
                if (DateHash.ContainsKey("Date001")) Date001 = Date(columnName: "Date001");
                if (DateHash.ContainsKey("Date002")) Date002 = Date(columnName: "Date002");
                if (DateHash.ContainsKey("Date003")) Date003 = Date(columnName: "Date003");
                if (DateHash.ContainsKey("Date004")) Date004 = Date(columnName: "Date004");
                if (DateHash.ContainsKey("Date005")) Date005 = Date(columnName: "Date005");
                if (DateHash.ContainsKey("Date006")) Date006 = Date(columnName: "Date006");
                if (DateHash.ContainsKey("Date007")) Date007 = Date(columnName: "Date007");
                if (DateHash.ContainsKey("Date008")) Date008 = Date(columnName: "Date008");
                if (DateHash.ContainsKey("Date009")) Date009 = Date(columnName: "Date009");
                if (DateHash.ContainsKey("Date010")) Date010 = Date(columnName: "Date010");
                if (DateHash.ContainsKey("Date011")) Date011 = Date(columnName: "Date011");
                if (DateHash.ContainsKey("Date012")) Date012 = Date(columnName: "Date012");
                if (DateHash.ContainsKey("Date013")) Date013 = Date(columnName: "Date013");
                if (DateHash.ContainsKey("Date014")) Date014 = Date(columnName: "Date014");
                if (DateHash.ContainsKey("Date015")) Date015 = Date(columnName: "Date015");
                if (DateHash.ContainsKey("Date016")) Date016 = Date(columnName: "Date016");
                if (DateHash.ContainsKey("Date017")) Date017 = Date(columnName: "Date017");
                if (DateHash.ContainsKey("Date018")) Date018 = Date(columnName: "Date018");
                if (DateHash.ContainsKey("Date019")) Date019 = Date(columnName: "Date019");
                if (DateHash.ContainsKey("Date020")) Date020 = Date(columnName: "Date020");
                if (DateHash.ContainsKey("Date021")) Date021 = Date(columnName: "Date021");
                if (DateHash.ContainsKey("Date022")) Date022 = Date(columnName: "Date022");
                if (DateHash.ContainsKey("Date023")) Date023 = Date(columnName: "Date023");
                if (DateHash.ContainsKey("Date024")) Date024 = Date(columnName: "Date024");
                if (DateHash.ContainsKey("Date025")) Date025 = Date(columnName: "Date025");
                if (DateHash.ContainsKey("Date026")) Date026 = Date(columnName: "Date026");
                if (DateHash.ContainsKey("Date027")) Date027 = Date(columnName: "Date027");
                if (DateHash.ContainsKey("Date028")) Date028 = Date(columnName: "Date028");
                if (DateHash.ContainsKey("Date029")) Date029 = Date(columnName: "Date029");
                if (DateHash.ContainsKey("Date030")) Date030 = Date(columnName: "Date030");
                if (DateHash.ContainsKey("Date031")) Date031 = Date(columnName: "Date031");
                if (DateHash.ContainsKey("Date032")) Date032 = Date(columnName: "Date032");
                if (DateHash.ContainsKey("Date033")) Date033 = Date(columnName: "Date033");
                if (DateHash.ContainsKey("Date034")) Date034 = Date(columnName: "Date034");
                if (DateHash.ContainsKey("Date035")) Date035 = Date(columnName: "Date035");
                if (DateHash.ContainsKey("Date036")) Date036 = Date(columnName: "Date036");
                if (DateHash.ContainsKey("Date037")) Date037 = Date(columnName: "Date037");
                if (DateHash.ContainsKey("Date038")) Date038 = Date(columnName: "Date038");
                if (DateHash.ContainsKey("Date039")) Date039 = Date(columnName: "Date039");
                if (DateHash.ContainsKey("Date040")) Date040 = Date(columnName: "Date040");
                if (DateHash.ContainsKey("Date041")) Date041 = Date(columnName: "Date041");
                if (DateHash.ContainsKey("Date042")) Date042 = Date(columnName: "Date042");
                if (DateHash.ContainsKey("Date043")) Date043 = Date(columnName: "Date043");
                if (DateHash.ContainsKey("Date044")) Date044 = Date(columnName: "Date044");
                if (DateHash.ContainsKey("Date045")) Date045 = Date(columnName: "Date045");
                if (DateHash.ContainsKey("Date046")) Date046 = Date(columnName: "Date046");
                if (DateHash.ContainsKey("Date047")) Date047 = Date(columnName: "Date047");
                if (DateHash.ContainsKey("Date048")) Date048 = Date(columnName: "Date048");
                if (DateHash.ContainsKey("Date049")) Date049 = Date(columnName: "Date049");
                if (DateHash.ContainsKey("Date050")) Date050 = Date(columnName: "Date050");
                if (DateHash.ContainsKey("Date051")) Date051 = Date(columnName: "Date051");
                if (DateHash.ContainsKey("Date052")) Date052 = Date(columnName: "Date052");
                if (DateHash.ContainsKey("Date053")) Date053 = Date(columnName: "Date053");
                if (DateHash.ContainsKey("Date054")) Date054 = Date(columnName: "Date054");
                if (DateHash.ContainsKey("Date055")) Date055 = Date(columnName: "Date055");
                if (DateHash.ContainsKey("Date056")) Date056 = Date(columnName: "Date056");
                if (DateHash.ContainsKey("Date057")) Date057 = Date(columnName: "Date057");
                if (DateHash.ContainsKey("Date058")) Date058 = Date(columnName: "Date058");
                if (DateHash.ContainsKey("Date059")) Date059 = Date(columnName: "Date059");
                if (DateHash.ContainsKey("Date060")) Date060 = Date(columnName: "Date060");
                if (DateHash.ContainsKey("Date061")) Date061 = Date(columnName: "Date061");
                if (DateHash.ContainsKey("Date062")) Date062 = Date(columnName: "Date062");
                if (DateHash.ContainsKey("Date063")) Date063 = Date(columnName: "Date063");
                if (DateHash.ContainsKey("Date064")) Date064 = Date(columnName: "Date064");
                if (DateHash.ContainsKey("Date065")) Date065 = Date(columnName: "Date065");
                if (DateHash.ContainsKey("Date066")) Date066 = Date(columnName: "Date066");
                if (DateHash.ContainsKey("Date067")) Date067 = Date(columnName: "Date067");
                if (DateHash.ContainsKey("Date068")) Date068 = Date(columnName: "Date068");
                if (DateHash.ContainsKey("Date069")) Date069 = Date(columnName: "Date069");
                if (DateHash.ContainsKey("Date070")) Date070 = Date(columnName: "Date070");
                if (DateHash.ContainsKey("Date071")) Date071 = Date(columnName: "Date071");
                if (DateHash.ContainsKey("Date072")) Date072 = Date(columnName: "Date072");
                if (DateHash.ContainsKey("Date073")) Date073 = Date(columnName: "Date073");
                if (DateHash.ContainsKey("Date074")) Date074 = Date(columnName: "Date074");
                if (DateHash.ContainsKey("Date075")) Date075 = Date(columnName: "Date075");
                if (DateHash.ContainsKey("Date076")) Date076 = Date(columnName: "Date076");
                if (DateHash.ContainsKey("Date077")) Date077 = Date(columnName: "Date077");
                if (DateHash.ContainsKey("Date078")) Date078 = Date(columnName: "Date078");
                if (DateHash.ContainsKey("Date079")) Date079 = Date(columnName: "Date079");
                if (DateHash.ContainsKey("Date080")) Date080 = Date(columnName: "Date080");
                if (DateHash.ContainsKey("Date081")) Date081 = Date(columnName: "Date081");
                if (DateHash.ContainsKey("Date082")) Date082 = Date(columnName: "Date082");
                if (DateHash.ContainsKey("Date083")) Date083 = Date(columnName: "Date083");
                if (DateHash.ContainsKey("Date084")) Date084 = Date(columnName: "Date084");
                if (DateHash.ContainsKey("Date085")) Date085 = Date(columnName: "Date085");
                if (DateHash.ContainsKey("Date086")) Date086 = Date(columnName: "Date086");
                if (DateHash.ContainsKey("Date087")) Date087 = Date(columnName: "Date087");
                if (DateHash.ContainsKey("Date088")) Date088 = Date(columnName: "Date088");
                if (DateHash.ContainsKey("Date089")) Date089 = Date(columnName: "Date089");
                if (DateHash.ContainsKey("Date090")) Date090 = Date(columnName: "Date090");
                if (DateHash.ContainsKey("Date091")) Date091 = Date(columnName: "Date091");
                if (DateHash.ContainsKey("Date092")) Date092 = Date(columnName: "Date092");
                if (DateHash.ContainsKey("Date093")) Date093 = Date(columnName: "Date093");
                if (DateHash.ContainsKey("Date094")) Date094 = Date(columnName: "Date094");
                if (DateHash.ContainsKey("Date095")) Date095 = Date(columnName: "Date095");
                if (DateHash.ContainsKey("Date096")) Date096 = Date(columnName: "Date096");
                if (DateHash.ContainsKey("Date097")) Date097 = Date(columnName: "Date097");
                if (DateHash.ContainsKey("Date098")) Date098 = Date(columnName: "Date098");
                if (DateHash.ContainsKey("Date099")) Date099 = Date(columnName: "Date099");
                if (DateHash.ContainsKey("Date100")) Date100 = Date(columnName: "Date100");
                if (DescriptionHash.ContainsKey("DescriptionA")) DescriptionA = Description(columnName: "DescriptionA");
                if (DescriptionHash.ContainsKey("DescriptionB")) DescriptionB = Description(columnName: "DescriptionB");
                if (DescriptionHash.ContainsKey("DescriptionC")) DescriptionC = Description(columnName: "DescriptionC");
                if (DescriptionHash.ContainsKey("DescriptionD")) DescriptionD = Description(columnName: "DescriptionD");
                if (DescriptionHash.ContainsKey("DescriptionE")) DescriptionE = Description(columnName: "DescriptionE");
                if (DescriptionHash.ContainsKey("DescriptionF")) DescriptionF = Description(columnName: "DescriptionF");
                if (DescriptionHash.ContainsKey("DescriptionG")) DescriptionG = Description(columnName: "DescriptionG");
                if (DescriptionHash.ContainsKey("DescriptionH")) DescriptionH = Description(columnName: "DescriptionH");
                if (DescriptionHash.ContainsKey("DescriptionI")) DescriptionI = Description(columnName: "DescriptionI");
                if (DescriptionHash.ContainsKey("DescriptionJ")) DescriptionJ = Description(columnName: "DescriptionJ");
                if (DescriptionHash.ContainsKey("DescriptionK")) DescriptionK = Description(columnName: "DescriptionK");
                if (DescriptionHash.ContainsKey("DescriptionL")) DescriptionL = Description(columnName: "DescriptionL");
                if (DescriptionHash.ContainsKey("DescriptionM")) DescriptionM = Description(columnName: "DescriptionM");
                if (DescriptionHash.ContainsKey("DescriptionN")) DescriptionN = Description(columnName: "DescriptionN");
                if (DescriptionHash.ContainsKey("DescriptionO")) DescriptionO = Description(columnName: "DescriptionO");
                if (DescriptionHash.ContainsKey("DescriptionP")) DescriptionP = Description(columnName: "DescriptionP");
                if (DescriptionHash.ContainsKey("DescriptionQ")) DescriptionQ = Description(columnName: "DescriptionQ");
                if (DescriptionHash.ContainsKey("DescriptionR")) DescriptionR = Description(columnName: "DescriptionR");
                if (DescriptionHash.ContainsKey("DescriptionS")) DescriptionS = Description(columnName: "DescriptionS");
                if (DescriptionHash.ContainsKey("DescriptionT")) DescriptionT = Description(columnName: "DescriptionT");
                if (DescriptionHash.ContainsKey("DescriptionU")) DescriptionU = Description(columnName: "DescriptionU");
                if (DescriptionHash.ContainsKey("DescriptionV")) DescriptionV = Description(columnName: "DescriptionV");
                if (DescriptionHash.ContainsKey("DescriptionW")) DescriptionW = Description(columnName: "DescriptionW");
                if (DescriptionHash.ContainsKey("DescriptionX")) DescriptionX = Description(columnName: "DescriptionX");
                if (DescriptionHash.ContainsKey("DescriptionY")) DescriptionY = Description(columnName: "DescriptionY");
                if (DescriptionHash.ContainsKey("DescriptionZ")) DescriptionZ = Description(columnName: "DescriptionZ");
                if (DescriptionHash.ContainsKey("Description001")) Description001 = Description(columnName: "Description001");
                if (DescriptionHash.ContainsKey("Description002")) Description002 = Description(columnName: "Description002");
                if (DescriptionHash.ContainsKey("Description003")) Description003 = Description(columnName: "Description003");
                if (DescriptionHash.ContainsKey("Description004")) Description004 = Description(columnName: "Description004");
                if (DescriptionHash.ContainsKey("Description005")) Description005 = Description(columnName: "Description005");
                if (DescriptionHash.ContainsKey("Description006")) Description006 = Description(columnName: "Description006");
                if (DescriptionHash.ContainsKey("Description007")) Description007 = Description(columnName: "Description007");
                if (DescriptionHash.ContainsKey("Description008")) Description008 = Description(columnName: "Description008");
                if (DescriptionHash.ContainsKey("Description009")) Description009 = Description(columnName: "Description009");
                if (DescriptionHash.ContainsKey("Description010")) Description010 = Description(columnName: "Description010");
                if (DescriptionHash.ContainsKey("Description011")) Description011 = Description(columnName: "Description011");
                if (DescriptionHash.ContainsKey("Description012")) Description012 = Description(columnName: "Description012");
                if (DescriptionHash.ContainsKey("Description013")) Description013 = Description(columnName: "Description013");
                if (DescriptionHash.ContainsKey("Description014")) Description014 = Description(columnName: "Description014");
                if (DescriptionHash.ContainsKey("Description015")) Description015 = Description(columnName: "Description015");
                if (DescriptionHash.ContainsKey("Description016")) Description016 = Description(columnName: "Description016");
                if (DescriptionHash.ContainsKey("Description017")) Description017 = Description(columnName: "Description017");
                if (DescriptionHash.ContainsKey("Description018")) Description018 = Description(columnName: "Description018");
                if (DescriptionHash.ContainsKey("Description019")) Description019 = Description(columnName: "Description019");
                if (DescriptionHash.ContainsKey("Description020")) Description020 = Description(columnName: "Description020");
                if (DescriptionHash.ContainsKey("Description021")) Description021 = Description(columnName: "Description021");
                if (DescriptionHash.ContainsKey("Description022")) Description022 = Description(columnName: "Description022");
                if (DescriptionHash.ContainsKey("Description023")) Description023 = Description(columnName: "Description023");
                if (DescriptionHash.ContainsKey("Description024")) Description024 = Description(columnName: "Description024");
                if (DescriptionHash.ContainsKey("Description025")) Description025 = Description(columnName: "Description025");
                if (DescriptionHash.ContainsKey("Description026")) Description026 = Description(columnName: "Description026");
                if (DescriptionHash.ContainsKey("Description027")) Description027 = Description(columnName: "Description027");
                if (DescriptionHash.ContainsKey("Description028")) Description028 = Description(columnName: "Description028");
                if (DescriptionHash.ContainsKey("Description029")) Description029 = Description(columnName: "Description029");
                if (DescriptionHash.ContainsKey("Description030")) Description030 = Description(columnName: "Description030");
                if (DescriptionHash.ContainsKey("Description031")) Description031 = Description(columnName: "Description031");
                if (DescriptionHash.ContainsKey("Description032")) Description032 = Description(columnName: "Description032");
                if (DescriptionHash.ContainsKey("Description033")) Description033 = Description(columnName: "Description033");
                if (DescriptionHash.ContainsKey("Description034")) Description034 = Description(columnName: "Description034");
                if (DescriptionHash.ContainsKey("Description035")) Description035 = Description(columnName: "Description035");
                if (DescriptionHash.ContainsKey("Description036")) Description036 = Description(columnName: "Description036");
                if (DescriptionHash.ContainsKey("Description037")) Description037 = Description(columnName: "Description037");
                if (DescriptionHash.ContainsKey("Description038")) Description038 = Description(columnName: "Description038");
                if (DescriptionHash.ContainsKey("Description039")) Description039 = Description(columnName: "Description039");
                if (DescriptionHash.ContainsKey("Description040")) Description040 = Description(columnName: "Description040");
                if (DescriptionHash.ContainsKey("Description041")) Description041 = Description(columnName: "Description041");
                if (DescriptionHash.ContainsKey("Description042")) Description042 = Description(columnName: "Description042");
                if (DescriptionHash.ContainsKey("Description043")) Description043 = Description(columnName: "Description043");
                if (DescriptionHash.ContainsKey("Description044")) Description044 = Description(columnName: "Description044");
                if (DescriptionHash.ContainsKey("Description045")) Description045 = Description(columnName: "Description045");
                if (DescriptionHash.ContainsKey("Description046")) Description046 = Description(columnName: "Description046");
                if (DescriptionHash.ContainsKey("Description047")) Description047 = Description(columnName: "Description047");
                if (DescriptionHash.ContainsKey("Description048")) Description048 = Description(columnName: "Description048");
                if (DescriptionHash.ContainsKey("Description049")) Description049 = Description(columnName: "Description049");
                if (DescriptionHash.ContainsKey("Description050")) Description050 = Description(columnName: "Description050");
                if (DescriptionHash.ContainsKey("Description051")) Description051 = Description(columnName: "Description051");
                if (DescriptionHash.ContainsKey("Description052")) Description052 = Description(columnName: "Description052");
                if (DescriptionHash.ContainsKey("Description053")) Description053 = Description(columnName: "Description053");
                if (DescriptionHash.ContainsKey("Description054")) Description054 = Description(columnName: "Description054");
                if (DescriptionHash.ContainsKey("Description055")) Description055 = Description(columnName: "Description055");
                if (DescriptionHash.ContainsKey("Description056")) Description056 = Description(columnName: "Description056");
                if (DescriptionHash.ContainsKey("Description057")) Description057 = Description(columnName: "Description057");
                if (DescriptionHash.ContainsKey("Description058")) Description058 = Description(columnName: "Description058");
                if (DescriptionHash.ContainsKey("Description059")) Description059 = Description(columnName: "Description059");
                if (DescriptionHash.ContainsKey("Description060")) Description060 = Description(columnName: "Description060");
                if (DescriptionHash.ContainsKey("Description061")) Description061 = Description(columnName: "Description061");
                if (DescriptionHash.ContainsKey("Description062")) Description062 = Description(columnName: "Description062");
                if (DescriptionHash.ContainsKey("Description063")) Description063 = Description(columnName: "Description063");
                if (DescriptionHash.ContainsKey("Description064")) Description064 = Description(columnName: "Description064");
                if (DescriptionHash.ContainsKey("Description065")) Description065 = Description(columnName: "Description065");
                if (DescriptionHash.ContainsKey("Description066")) Description066 = Description(columnName: "Description066");
                if (DescriptionHash.ContainsKey("Description067")) Description067 = Description(columnName: "Description067");
                if (DescriptionHash.ContainsKey("Description068")) Description068 = Description(columnName: "Description068");
                if (DescriptionHash.ContainsKey("Description069")) Description069 = Description(columnName: "Description069");
                if (DescriptionHash.ContainsKey("Description070")) Description070 = Description(columnName: "Description070");
                if (DescriptionHash.ContainsKey("Description071")) Description071 = Description(columnName: "Description071");
                if (DescriptionHash.ContainsKey("Description072")) Description072 = Description(columnName: "Description072");
                if (DescriptionHash.ContainsKey("Description073")) Description073 = Description(columnName: "Description073");
                if (DescriptionHash.ContainsKey("Description074")) Description074 = Description(columnName: "Description074");
                if (DescriptionHash.ContainsKey("Description075")) Description075 = Description(columnName: "Description075");
                if (DescriptionHash.ContainsKey("Description076")) Description076 = Description(columnName: "Description076");
                if (DescriptionHash.ContainsKey("Description077")) Description077 = Description(columnName: "Description077");
                if (DescriptionHash.ContainsKey("Description078")) Description078 = Description(columnName: "Description078");
                if (DescriptionHash.ContainsKey("Description079")) Description079 = Description(columnName: "Description079");
                if (DescriptionHash.ContainsKey("Description080")) Description080 = Description(columnName: "Description080");
                if (DescriptionHash.ContainsKey("Description081")) Description081 = Description(columnName: "Description081");
                if (DescriptionHash.ContainsKey("Description082")) Description082 = Description(columnName: "Description082");
                if (DescriptionHash.ContainsKey("Description083")) Description083 = Description(columnName: "Description083");
                if (DescriptionHash.ContainsKey("Description084")) Description084 = Description(columnName: "Description084");
                if (DescriptionHash.ContainsKey("Description085")) Description085 = Description(columnName: "Description085");
                if (DescriptionHash.ContainsKey("Description086")) Description086 = Description(columnName: "Description086");
                if (DescriptionHash.ContainsKey("Description087")) Description087 = Description(columnName: "Description087");
                if (DescriptionHash.ContainsKey("Description088")) Description088 = Description(columnName: "Description088");
                if (DescriptionHash.ContainsKey("Description089")) Description089 = Description(columnName: "Description089");
                if (DescriptionHash.ContainsKey("Description090")) Description090 = Description(columnName: "Description090");
                if (DescriptionHash.ContainsKey("Description091")) Description091 = Description(columnName: "Description091");
                if (DescriptionHash.ContainsKey("Description092")) Description092 = Description(columnName: "Description092");
                if (DescriptionHash.ContainsKey("Description093")) Description093 = Description(columnName: "Description093");
                if (DescriptionHash.ContainsKey("Description094")) Description094 = Description(columnName: "Description094");
                if (DescriptionHash.ContainsKey("Description095")) Description095 = Description(columnName: "Description095");
                if (DescriptionHash.ContainsKey("Description096")) Description096 = Description(columnName: "Description096");
                if (DescriptionHash.ContainsKey("Description097")) Description097 = Description(columnName: "Description097");
                if (DescriptionHash.ContainsKey("Description098")) Description098 = Description(columnName: "Description098");
                if (DescriptionHash.ContainsKey("Description099")) Description099 = Description(columnName: "Description099");
                if (DescriptionHash.ContainsKey("Description100")) Description100 = Description(columnName: "Description100");
                if (CheckHash.ContainsKey("CheckA")) CheckA = Check(columnName: "CheckA");
                if (CheckHash.ContainsKey("CheckB")) CheckB = Check(columnName: "CheckB");
                if (CheckHash.ContainsKey("CheckC")) CheckC = Check(columnName: "CheckC");
                if (CheckHash.ContainsKey("CheckD")) CheckD = Check(columnName: "CheckD");
                if (CheckHash.ContainsKey("CheckE")) CheckE = Check(columnName: "CheckE");
                if (CheckHash.ContainsKey("CheckF")) CheckF = Check(columnName: "CheckF");
                if (CheckHash.ContainsKey("CheckG")) CheckG = Check(columnName: "CheckG");
                if (CheckHash.ContainsKey("CheckH")) CheckH = Check(columnName: "CheckH");
                if (CheckHash.ContainsKey("CheckI")) CheckI = Check(columnName: "CheckI");
                if (CheckHash.ContainsKey("CheckJ")) CheckJ = Check(columnName: "CheckJ");
                if (CheckHash.ContainsKey("CheckK")) CheckK = Check(columnName: "CheckK");
                if (CheckHash.ContainsKey("CheckL")) CheckL = Check(columnName: "CheckL");
                if (CheckHash.ContainsKey("CheckM")) CheckM = Check(columnName: "CheckM");
                if (CheckHash.ContainsKey("CheckN")) CheckN = Check(columnName: "CheckN");
                if (CheckHash.ContainsKey("CheckO")) CheckO = Check(columnName: "CheckO");
                if (CheckHash.ContainsKey("CheckP")) CheckP = Check(columnName: "CheckP");
                if (CheckHash.ContainsKey("CheckQ")) CheckQ = Check(columnName: "CheckQ");
                if (CheckHash.ContainsKey("CheckR")) CheckR = Check(columnName: "CheckR");
                if (CheckHash.ContainsKey("CheckS")) CheckS = Check(columnName: "CheckS");
                if (CheckHash.ContainsKey("CheckT")) CheckT = Check(columnName: "CheckT");
                if (CheckHash.ContainsKey("CheckU")) CheckU = Check(columnName: "CheckU");
                if (CheckHash.ContainsKey("CheckV")) CheckV = Check(columnName: "CheckV");
                if (CheckHash.ContainsKey("CheckW")) CheckW = Check(columnName: "CheckW");
                if (CheckHash.ContainsKey("CheckX")) CheckX = Check(columnName: "CheckX");
                if (CheckHash.ContainsKey("CheckY")) CheckY = Check(columnName: "CheckY");
                if (CheckHash.ContainsKey("CheckZ")) CheckZ = Check(columnName: "CheckZ");
                if (CheckHash.ContainsKey("Check001")) Check001 = Check(columnName: "Check001");
                if (CheckHash.ContainsKey("Check002")) Check002 = Check(columnName: "Check002");
                if (CheckHash.ContainsKey("Check003")) Check003 = Check(columnName: "Check003");
                if (CheckHash.ContainsKey("Check004")) Check004 = Check(columnName: "Check004");
                if (CheckHash.ContainsKey("Check005")) Check005 = Check(columnName: "Check005");
                if (CheckHash.ContainsKey("Check006")) Check006 = Check(columnName: "Check006");
                if (CheckHash.ContainsKey("Check007")) Check007 = Check(columnName: "Check007");
                if (CheckHash.ContainsKey("Check008")) Check008 = Check(columnName: "Check008");
                if (CheckHash.ContainsKey("Check009")) Check009 = Check(columnName: "Check009");
                if (CheckHash.ContainsKey("Check010")) Check010 = Check(columnName: "Check010");
                if (CheckHash.ContainsKey("Check011")) Check011 = Check(columnName: "Check011");
                if (CheckHash.ContainsKey("Check012")) Check012 = Check(columnName: "Check012");
                if (CheckHash.ContainsKey("Check013")) Check013 = Check(columnName: "Check013");
                if (CheckHash.ContainsKey("Check014")) Check014 = Check(columnName: "Check014");
                if (CheckHash.ContainsKey("Check015")) Check015 = Check(columnName: "Check015");
                if (CheckHash.ContainsKey("Check016")) Check016 = Check(columnName: "Check016");
                if (CheckHash.ContainsKey("Check017")) Check017 = Check(columnName: "Check017");
                if (CheckHash.ContainsKey("Check018")) Check018 = Check(columnName: "Check018");
                if (CheckHash.ContainsKey("Check019")) Check019 = Check(columnName: "Check019");
                if (CheckHash.ContainsKey("Check020")) Check020 = Check(columnName: "Check020");
                if (CheckHash.ContainsKey("Check021")) Check021 = Check(columnName: "Check021");
                if (CheckHash.ContainsKey("Check022")) Check022 = Check(columnName: "Check022");
                if (CheckHash.ContainsKey("Check023")) Check023 = Check(columnName: "Check023");
                if (CheckHash.ContainsKey("Check024")) Check024 = Check(columnName: "Check024");
                if (CheckHash.ContainsKey("Check025")) Check025 = Check(columnName: "Check025");
                if (CheckHash.ContainsKey("Check026")) Check026 = Check(columnName: "Check026");
                if (CheckHash.ContainsKey("Check027")) Check027 = Check(columnName: "Check027");
                if (CheckHash.ContainsKey("Check028")) Check028 = Check(columnName: "Check028");
                if (CheckHash.ContainsKey("Check029")) Check029 = Check(columnName: "Check029");
                if (CheckHash.ContainsKey("Check030")) Check030 = Check(columnName: "Check030");
                if (CheckHash.ContainsKey("Check031")) Check031 = Check(columnName: "Check031");
                if (CheckHash.ContainsKey("Check032")) Check032 = Check(columnName: "Check032");
                if (CheckHash.ContainsKey("Check033")) Check033 = Check(columnName: "Check033");
                if (CheckHash.ContainsKey("Check034")) Check034 = Check(columnName: "Check034");
                if (CheckHash.ContainsKey("Check035")) Check035 = Check(columnName: "Check035");
                if (CheckHash.ContainsKey("Check036")) Check036 = Check(columnName: "Check036");
                if (CheckHash.ContainsKey("Check037")) Check037 = Check(columnName: "Check037");
                if (CheckHash.ContainsKey("Check038")) Check038 = Check(columnName: "Check038");
                if (CheckHash.ContainsKey("Check039")) Check039 = Check(columnName: "Check039");
                if (CheckHash.ContainsKey("Check040")) Check040 = Check(columnName: "Check040");
                if (CheckHash.ContainsKey("Check041")) Check041 = Check(columnName: "Check041");
                if (CheckHash.ContainsKey("Check042")) Check042 = Check(columnName: "Check042");
                if (CheckHash.ContainsKey("Check043")) Check043 = Check(columnName: "Check043");
                if (CheckHash.ContainsKey("Check044")) Check044 = Check(columnName: "Check044");
                if (CheckHash.ContainsKey("Check045")) Check045 = Check(columnName: "Check045");
                if (CheckHash.ContainsKey("Check046")) Check046 = Check(columnName: "Check046");
                if (CheckHash.ContainsKey("Check047")) Check047 = Check(columnName: "Check047");
                if (CheckHash.ContainsKey("Check048")) Check048 = Check(columnName: "Check048");
                if (CheckHash.ContainsKey("Check049")) Check049 = Check(columnName: "Check049");
                if (CheckHash.ContainsKey("Check050")) Check050 = Check(columnName: "Check050");
                if (CheckHash.ContainsKey("Check051")) Check051 = Check(columnName: "Check051");
                if (CheckHash.ContainsKey("Check052")) Check052 = Check(columnName: "Check052");
                if (CheckHash.ContainsKey("Check053")) Check053 = Check(columnName: "Check053");
                if (CheckHash.ContainsKey("Check054")) Check054 = Check(columnName: "Check054");
                if (CheckHash.ContainsKey("Check055")) Check055 = Check(columnName: "Check055");
                if (CheckHash.ContainsKey("Check056")) Check056 = Check(columnName: "Check056");
                if (CheckHash.ContainsKey("Check057")) Check057 = Check(columnName: "Check057");
                if (CheckHash.ContainsKey("Check058")) Check058 = Check(columnName: "Check058");
                if (CheckHash.ContainsKey("Check059")) Check059 = Check(columnName: "Check059");
                if (CheckHash.ContainsKey("Check060")) Check060 = Check(columnName: "Check060");
                if (CheckHash.ContainsKey("Check061")) Check061 = Check(columnName: "Check061");
                if (CheckHash.ContainsKey("Check062")) Check062 = Check(columnName: "Check062");
                if (CheckHash.ContainsKey("Check063")) Check063 = Check(columnName: "Check063");
                if (CheckHash.ContainsKey("Check064")) Check064 = Check(columnName: "Check064");
                if (CheckHash.ContainsKey("Check065")) Check065 = Check(columnName: "Check065");
                if (CheckHash.ContainsKey("Check066")) Check066 = Check(columnName: "Check066");
                if (CheckHash.ContainsKey("Check067")) Check067 = Check(columnName: "Check067");
                if (CheckHash.ContainsKey("Check068")) Check068 = Check(columnName: "Check068");
                if (CheckHash.ContainsKey("Check069")) Check069 = Check(columnName: "Check069");
                if (CheckHash.ContainsKey("Check070")) Check070 = Check(columnName: "Check070");
                if (CheckHash.ContainsKey("Check071")) Check071 = Check(columnName: "Check071");
                if (CheckHash.ContainsKey("Check072")) Check072 = Check(columnName: "Check072");
                if (CheckHash.ContainsKey("Check073")) Check073 = Check(columnName: "Check073");
                if (CheckHash.ContainsKey("Check074")) Check074 = Check(columnName: "Check074");
                if (CheckHash.ContainsKey("Check075")) Check075 = Check(columnName: "Check075");
                if (CheckHash.ContainsKey("Check076")) Check076 = Check(columnName: "Check076");
                if (CheckHash.ContainsKey("Check077")) Check077 = Check(columnName: "Check077");
                if (CheckHash.ContainsKey("Check078")) Check078 = Check(columnName: "Check078");
                if (CheckHash.ContainsKey("Check079")) Check079 = Check(columnName: "Check079");
                if (CheckHash.ContainsKey("Check080")) Check080 = Check(columnName: "Check080");
                if (CheckHash.ContainsKey("Check081")) Check081 = Check(columnName: "Check081");
                if (CheckHash.ContainsKey("Check082")) Check082 = Check(columnName: "Check082");
                if (CheckHash.ContainsKey("Check083")) Check083 = Check(columnName: "Check083");
                if (CheckHash.ContainsKey("Check084")) Check084 = Check(columnName: "Check084");
                if (CheckHash.ContainsKey("Check085")) Check085 = Check(columnName: "Check085");
                if (CheckHash.ContainsKey("Check086")) Check086 = Check(columnName: "Check086");
                if (CheckHash.ContainsKey("Check087")) Check087 = Check(columnName: "Check087");
                if (CheckHash.ContainsKey("Check088")) Check088 = Check(columnName: "Check088");
                if (CheckHash.ContainsKey("Check089")) Check089 = Check(columnName: "Check089");
                if (CheckHash.ContainsKey("Check090")) Check090 = Check(columnName: "Check090");
                if (CheckHash.ContainsKey("Check091")) Check091 = Check(columnName: "Check091");
                if (CheckHash.ContainsKey("Check092")) Check092 = Check(columnName: "Check092");
                if (CheckHash.ContainsKey("Check093")) Check093 = Check(columnName: "Check093");
                if (CheckHash.ContainsKey("Check094")) Check094 = Check(columnName: "Check094");
                if (CheckHash.ContainsKey("Check095")) Check095 = Check(columnName: "Check095");
                if (CheckHash.ContainsKey("Check096")) Check096 = Check(columnName: "Check096");
                if (CheckHash.ContainsKey("Check097")) Check097 = Check(columnName: "Check097");
                if (CheckHash.ContainsKey("Check098")) Check098 = Check(columnName: "Check098");
                if (CheckHash.ContainsKey("Check099")) Check099 = Check(columnName: "Check099");
                if (CheckHash.ContainsKey("Check100")) Check100 = Check(columnName: "Check100");
                if (AttachmentsHash.ContainsKey("AttachmentsA")) AttachmentsA = Attachments(columnName: "AttachmentsA").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsB")) AttachmentsB = Attachments(columnName: "AttachmentsB").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsC")) AttachmentsC = Attachments(columnName: "AttachmentsC").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsD")) AttachmentsD = Attachments(columnName: "AttachmentsD").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsE")) AttachmentsE = Attachments(columnName: "AttachmentsE").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsF")) AttachmentsF = Attachments(columnName: "AttachmentsF").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsG")) AttachmentsG = Attachments(columnName: "AttachmentsG").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsH")) AttachmentsH = Attachments(columnName: "AttachmentsH").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsI")) AttachmentsI = Attachments(columnName: "AttachmentsI").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsJ")) AttachmentsJ = Attachments(columnName: "AttachmentsJ").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsK")) AttachmentsK = Attachments(columnName: "AttachmentsK").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsL")) AttachmentsL = Attachments(columnName: "AttachmentsL").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsM")) AttachmentsM = Attachments(columnName: "AttachmentsM").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsN")) AttachmentsN = Attachments(columnName: "AttachmentsN").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsO")) AttachmentsO = Attachments(columnName: "AttachmentsO").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsP")) AttachmentsP = Attachments(columnName: "AttachmentsP").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsQ")) AttachmentsQ = Attachments(columnName: "AttachmentsQ").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsR")) AttachmentsR = Attachments(columnName: "AttachmentsR").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsS")) AttachmentsS = Attachments(columnName: "AttachmentsS").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsT")) AttachmentsT = Attachments(columnName: "AttachmentsT").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsU")) AttachmentsU = Attachments(columnName: "AttachmentsU").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsV")) AttachmentsV = Attachments(columnName: "AttachmentsV").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsW")) AttachmentsW = Attachments(columnName: "AttachmentsW").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsX")) AttachmentsX = Attachments(columnName: "AttachmentsX").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsY")) AttachmentsY = Attachments(columnName: "AttachmentsY").RecordingJson();
                if (AttachmentsHash.ContainsKey("AttachmentsZ")) AttachmentsZ = Attachments(columnName: "AttachmentsZ").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments001")) Attachments001 = Attachments(columnName: "Attachments001").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments002")) Attachments002 = Attachments(columnName: "Attachments002").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments003")) Attachments003 = Attachments(columnName: "Attachments003").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments004")) Attachments004 = Attachments(columnName: "Attachments004").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments005")) Attachments005 = Attachments(columnName: "Attachments005").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments006")) Attachments006 = Attachments(columnName: "Attachments006").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments007")) Attachments007 = Attachments(columnName: "Attachments007").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments008")) Attachments008 = Attachments(columnName: "Attachments008").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments009")) Attachments009 = Attachments(columnName: "Attachments009").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments010")) Attachments010 = Attachments(columnName: "Attachments010").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments011")) Attachments011 = Attachments(columnName: "Attachments011").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments012")) Attachments012 = Attachments(columnName: "Attachments012").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments013")) Attachments013 = Attachments(columnName: "Attachments013").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments014")) Attachments014 = Attachments(columnName: "Attachments014").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments015")) Attachments015 = Attachments(columnName: "Attachments015").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments016")) Attachments016 = Attachments(columnName: "Attachments016").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments017")) Attachments017 = Attachments(columnName: "Attachments017").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments018")) Attachments018 = Attachments(columnName: "Attachments018").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments019")) Attachments019 = Attachments(columnName: "Attachments019").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments020")) Attachments020 = Attachments(columnName: "Attachments020").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments021")) Attachments021 = Attachments(columnName: "Attachments021").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments022")) Attachments022 = Attachments(columnName: "Attachments022").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments023")) Attachments023 = Attachments(columnName: "Attachments023").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments024")) Attachments024 = Attachments(columnName: "Attachments024").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments025")) Attachments025 = Attachments(columnName: "Attachments025").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments026")) Attachments026 = Attachments(columnName: "Attachments026").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments027")) Attachments027 = Attachments(columnName: "Attachments027").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments028")) Attachments028 = Attachments(columnName: "Attachments028").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments029")) Attachments029 = Attachments(columnName: "Attachments029").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments030")) Attachments030 = Attachments(columnName: "Attachments030").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments031")) Attachments031 = Attachments(columnName: "Attachments031").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments032")) Attachments032 = Attachments(columnName: "Attachments032").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments033")) Attachments033 = Attachments(columnName: "Attachments033").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments034")) Attachments034 = Attachments(columnName: "Attachments034").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments035")) Attachments035 = Attachments(columnName: "Attachments035").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments036")) Attachments036 = Attachments(columnName: "Attachments036").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments037")) Attachments037 = Attachments(columnName: "Attachments037").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments038")) Attachments038 = Attachments(columnName: "Attachments038").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments039")) Attachments039 = Attachments(columnName: "Attachments039").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments040")) Attachments040 = Attachments(columnName: "Attachments040").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments041")) Attachments041 = Attachments(columnName: "Attachments041").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments042")) Attachments042 = Attachments(columnName: "Attachments042").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments043")) Attachments043 = Attachments(columnName: "Attachments043").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments044")) Attachments044 = Attachments(columnName: "Attachments044").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments045")) Attachments045 = Attachments(columnName: "Attachments045").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments046")) Attachments046 = Attachments(columnName: "Attachments046").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments047")) Attachments047 = Attachments(columnName: "Attachments047").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments048")) Attachments048 = Attachments(columnName: "Attachments048").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments049")) Attachments049 = Attachments(columnName: "Attachments049").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments050")) Attachments050 = Attachments(columnName: "Attachments050").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments051")) Attachments051 = Attachments(columnName: "Attachments051").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments052")) Attachments052 = Attachments(columnName: "Attachments052").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments053")) Attachments053 = Attachments(columnName: "Attachments053").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments054")) Attachments054 = Attachments(columnName: "Attachments054").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments055")) Attachments055 = Attachments(columnName: "Attachments055").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments056")) Attachments056 = Attachments(columnName: "Attachments056").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments057")) Attachments057 = Attachments(columnName: "Attachments057").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments058")) Attachments058 = Attachments(columnName: "Attachments058").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments059")) Attachments059 = Attachments(columnName: "Attachments059").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments060")) Attachments060 = Attachments(columnName: "Attachments060").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments061")) Attachments061 = Attachments(columnName: "Attachments061").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments062")) Attachments062 = Attachments(columnName: "Attachments062").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments063")) Attachments063 = Attachments(columnName: "Attachments063").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments064")) Attachments064 = Attachments(columnName: "Attachments064").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments065")) Attachments065 = Attachments(columnName: "Attachments065").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments066")) Attachments066 = Attachments(columnName: "Attachments066").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments067")) Attachments067 = Attachments(columnName: "Attachments067").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments068")) Attachments068 = Attachments(columnName: "Attachments068").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments069")) Attachments069 = Attachments(columnName: "Attachments069").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments070")) Attachments070 = Attachments(columnName: "Attachments070").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments071")) Attachments071 = Attachments(columnName: "Attachments071").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments072")) Attachments072 = Attachments(columnName: "Attachments072").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments073")) Attachments073 = Attachments(columnName: "Attachments073").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments074")) Attachments074 = Attachments(columnName: "Attachments074").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments075")) Attachments075 = Attachments(columnName: "Attachments075").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments076")) Attachments076 = Attachments(columnName: "Attachments076").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments077")) Attachments077 = Attachments(columnName: "Attachments077").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments078")) Attachments078 = Attachments(columnName: "Attachments078").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments079")) Attachments079 = Attachments(columnName: "Attachments079").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments080")) Attachments080 = Attachments(columnName: "Attachments080").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments081")) Attachments081 = Attachments(columnName: "Attachments081").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments082")) Attachments082 = Attachments(columnName: "Attachments082").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments083")) Attachments083 = Attachments(columnName: "Attachments083").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments084")) Attachments084 = Attachments(columnName: "Attachments084").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments085")) Attachments085 = Attachments(columnName: "Attachments085").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments086")) Attachments086 = Attachments(columnName: "Attachments086").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments087")) Attachments087 = Attachments(columnName: "Attachments087").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments088")) Attachments088 = Attachments(columnName: "Attachments088").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments089")) Attachments089 = Attachments(columnName: "Attachments089").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments090")) Attachments090 = Attachments(columnName: "Attachments090").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments091")) Attachments091 = Attachments(columnName: "Attachments091").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments092")) Attachments092 = Attachments(columnName: "Attachments092").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments093")) Attachments093 = Attachments(columnName: "Attachments093").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments094")) Attachments094 = Attachments(columnName: "Attachments094").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments095")) Attachments095 = Attachments(columnName: "Attachments095").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments096")) Attachments096 = Attachments(columnName: "Attachments096").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments097")) Attachments097 = Attachments(columnName: "Attachments097").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments098")) Attachments098 = Attachments(columnName: "Attachments098").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments099")) Attachments099 = Attachments(columnName: "Attachments099").RecordingJson();
                if (AttachmentsHash.ContainsKey("Attachments100")) Attachments100 = Attachments(columnName: "Attachments100").RecordingJson();
                ClassHash = null;
                NumHash = null;
                DateHash = null;
                DescriptionHash = null;
                CheckHash = null;
                AttachmentsHash = null;
            }
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (ApiVersion < 1.100M)
            {
                if (ClassA != null) Class(columnName: "ClassA", value: ClassA); ClassA = null;
                if (ClassB != null) Class(columnName: "ClassB", value: ClassB); ClassB = null;
                if (ClassC != null) Class(columnName: "ClassC", value: ClassC); ClassC = null;
                if (ClassD != null) Class(columnName: "ClassD", value: ClassD); ClassD = null;
                if (ClassE != null) Class(columnName: "ClassE", value: ClassE); ClassE = null;
                if (ClassF != null) Class(columnName: "ClassF", value: ClassF); ClassF = null;
                if (ClassG != null) Class(columnName: "ClassG", value: ClassG); ClassG = null;
                if (ClassH != null) Class(columnName: "ClassH", value: ClassH); ClassH = null;
                if (ClassI != null) Class(columnName: "ClassI", value: ClassI); ClassI = null;
                if (ClassJ != null) Class(columnName: "ClassJ", value: ClassJ); ClassJ = null;
                if (ClassK != null) Class(columnName: "ClassK", value: ClassK); ClassK = null;
                if (ClassL != null) Class(columnName: "ClassL", value: ClassL); ClassL = null;
                if (ClassM != null) Class(columnName: "ClassM", value: ClassM); ClassM = null;
                if (ClassN != null) Class(columnName: "ClassN", value: ClassN); ClassN = null;
                if (ClassO != null) Class(columnName: "ClassO", value: ClassO); ClassO = null;
                if (ClassP != null) Class(columnName: "ClassP", value: ClassP); ClassP = null;
                if (ClassQ != null) Class(columnName: "ClassQ", value: ClassQ); ClassQ = null;
                if (ClassR != null) Class(columnName: "ClassR", value: ClassR); ClassR = null;
                if (ClassS != null) Class(columnName: "ClassS", value: ClassS); ClassS = null;
                if (ClassT != null) Class(columnName: "ClassT", value: ClassT); ClassT = null;
                if (ClassU != null) Class(columnName: "ClassU", value: ClassU); ClassU = null;
                if (ClassV != null) Class(columnName: "ClassV", value: ClassV); ClassV = null;
                if (ClassW != null) Class(columnName: "ClassW", value: ClassW); ClassW = null;
                if (ClassX != null) Class(columnName: "ClassX", value: ClassX); ClassX = null;
                if (ClassY != null) Class(columnName: "ClassY", value: ClassY); ClassY = null;
                if (ClassZ != null) Class(columnName: "ClassZ", value: ClassZ); ClassZ = null;
                if (Class001 != null) Class(columnName: "Class001", value: Class001); Class001 = null;
                if (Class002 != null) Class(columnName: "Class002", value: Class002); Class002 = null;
                if (Class003 != null) Class(columnName: "Class003", value: Class003); Class003 = null;
                if (Class004 != null) Class(columnName: "Class004", value: Class004); Class004 = null;
                if (Class005 != null) Class(columnName: "Class005", value: Class005); Class005 = null;
                if (Class006 != null) Class(columnName: "Class006", value: Class006); Class006 = null;
                if (Class007 != null) Class(columnName: "Class007", value: Class007); Class007 = null;
                if (Class008 != null) Class(columnName: "Class008", value: Class008); Class008 = null;
                if (Class009 != null) Class(columnName: "Class009", value: Class009); Class009 = null;
                if (Class010 != null) Class(columnName: "Class010", value: Class010); Class010 = null;
                if (Class011 != null) Class(columnName: "Class011", value: Class011); Class011 = null;
                if (Class012 != null) Class(columnName: "Class012", value: Class012); Class012 = null;
                if (Class013 != null) Class(columnName: "Class013", value: Class013); Class013 = null;
                if (Class014 != null) Class(columnName: "Class014", value: Class014); Class014 = null;
                if (Class015 != null) Class(columnName: "Class015", value: Class015); Class015 = null;
                if (Class016 != null) Class(columnName: "Class016", value: Class016); Class016 = null;
                if (Class017 != null) Class(columnName: "Class017", value: Class017); Class017 = null;
                if (Class018 != null) Class(columnName: "Class018", value: Class018); Class018 = null;
                if (Class019 != null) Class(columnName: "Class019", value: Class019); Class019 = null;
                if (Class020 != null) Class(columnName: "Class020", value: Class020); Class020 = null;
                if (Class021 != null) Class(columnName: "Class021", value: Class021); Class021 = null;
                if (Class022 != null) Class(columnName: "Class022", value: Class022); Class022 = null;
                if (Class023 != null) Class(columnName: "Class023", value: Class023); Class023 = null;
                if (Class024 != null) Class(columnName: "Class024", value: Class024); Class024 = null;
                if (Class025 != null) Class(columnName: "Class025", value: Class025); Class025 = null;
                if (Class026 != null) Class(columnName: "Class026", value: Class026); Class026 = null;
                if (Class027 != null) Class(columnName: "Class027", value: Class027); Class027 = null;
                if (Class028 != null) Class(columnName: "Class028", value: Class028); Class028 = null;
                if (Class029 != null) Class(columnName: "Class029", value: Class029); Class029 = null;
                if (Class030 != null) Class(columnName: "Class030", value: Class030); Class030 = null;
                if (Class031 != null) Class(columnName: "Class031", value: Class031); Class031 = null;
                if (Class032 != null) Class(columnName: "Class032", value: Class032); Class032 = null;
                if (Class033 != null) Class(columnName: "Class033", value: Class033); Class033 = null;
                if (Class034 != null) Class(columnName: "Class034", value: Class034); Class034 = null;
                if (Class035 != null) Class(columnName: "Class035", value: Class035); Class035 = null;
                if (Class036 != null) Class(columnName: "Class036", value: Class036); Class036 = null;
                if (Class037 != null) Class(columnName: "Class037", value: Class037); Class037 = null;
                if (Class038 != null) Class(columnName: "Class038", value: Class038); Class038 = null;
                if (Class039 != null) Class(columnName: "Class039", value: Class039); Class039 = null;
                if (Class040 != null) Class(columnName: "Class040", value: Class040); Class040 = null;
                if (Class041 != null) Class(columnName: "Class041", value: Class041); Class041 = null;
                if (Class042 != null) Class(columnName: "Class042", value: Class042); Class042 = null;
                if (Class043 != null) Class(columnName: "Class043", value: Class043); Class043 = null;
                if (Class044 != null) Class(columnName: "Class044", value: Class044); Class044 = null;
                if (Class045 != null) Class(columnName: "Class045", value: Class045); Class045 = null;
                if (Class046 != null) Class(columnName: "Class046", value: Class046); Class046 = null;
                if (Class047 != null) Class(columnName: "Class047", value: Class047); Class047 = null;
                if (Class048 != null) Class(columnName: "Class048", value: Class048); Class048 = null;
                if (Class049 != null) Class(columnName: "Class049", value: Class049); Class049 = null;
                if (Class050 != null) Class(columnName: "Class050", value: Class050); Class050 = null;
                if (Class051 != null) Class(columnName: "Class051", value: Class051); Class051 = null;
                if (Class052 != null) Class(columnName: "Class052", value: Class052); Class052 = null;
                if (Class053 != null) Class(columnName: "Class053", value: Class053); Class053 = null;
                if (Class054 != null) Class(columnName: "Class054", value: Class054); Class054 = null;
                if (Class055 != null) Class(columnName: "Class055", value: Class055); Class055 = null;
                if (Class056 != null) Class(columnName: "Class056", value: Class056); Class056 = null;
                if (Class057 != null) Class(columnName: "Class057", value: Class057); Class057 = null;
                if (Class058 != null) Class(columnName: "Class058", value: Class058); Class058 = null;
                if (Class059 != null) Class(columnName: "Class059", value: Class059); Class059 = null;
                if (Class060 != null) Class(columnName: "Class060", value: Class060); Class060 = null;
                if (Class061 != null) Class(columnName: "Class061", value: Class061); Class061 = null;
                if (Class062 != null) Class(columnName: "Class062", value: Class062); Class062 = null;
                if (Class063 != null) Class(columnName: "Class063", value: Class063); Class063 = null;
                if (Class064 != null) Class(columnName: "Class064", value: Class064); Class064 = null;
                if (Class065 != null) Class(columnName: "Class065", value: Class065); Class065 = null;
                if (Class066 != null) Class(columnName: "Class066", value: Class066); Class066 = null;
                if (Class067 != null) Class(columnName: "Class067", value: Class067); Class067 = null;
                if (Class068 != null) Class(columnName: "Class068", value: Class068); Class068 = null;
                if (Class069 != null) Class(columnName: "Class069", value: Class069); Class069 = null;
                if (Class070 != null) Class(columnName: "Class070", value: Class070); Class070 = null;
                if (Class071 != null) Class(columnName: "Class071", value: Class071); Class071 = null;
                if (Class072 != null) Class(columnName: "Class072", value: Class072); Class072 = null;
                if (Class073 != null) Class(columnName: "Class073", value: Class073); Class073 = null;
                if (Class074 != null) Class(columnName: "Class074", value: Class074); Class074 = null;
                if (Class075 != null) Class(columnName: "Class075", value: Class075); Class075 = null;
                if (Class076 != null) Class(columnName: "Class076", value: Class076); Class076 = null;
                if (Class077 != null) Class(columnName: "Class077", value: Class077); Class077 = null;
                if (Class078 != null) Class(columnName: "Class078", value: Class078); Class078 = null;
                if (Class079 != null) Class(columnName: "Class079", value: Class079); Class079 = null;
                if (Class080 != null) Class(columnName: "Class080", value: Class080); Class080 = null;
                if (Class081 != null) Class(columnName: "Class081", value: Class081); Class081 = null;
                if (Class082 != null) Class(columnName: "Class082", value: Class082); Class082 = null;
                if (Class083 != null) Class(columnName: "Class083", value: Class083); Class083 = null;
                if (Class084 != null) Class(columnName: "Class084", value: Class084); Class084 = null;
                if (Class085 != null) Class(columnName: "Class085", value: Class085); Class085 = null;
                if (Class086 != null) Class(columnName: "Class086", value: Class086); Class086 = null;
                if (Class087 != null) Class(columnName: "Class087", value: Class087); Class087 = null;
                if (Class088 != null) Class(columnName: "Class088", value: Class088); Class088 = null;
                if (Class089 != null) Class(columnName: "Class089", value: Class089); Class089 = null;
                if (Class090 != null) Class(columnName: "Class090", value: Class090); Class090 = null;
                if (Class091 != null) Class(columnName: "Class091", value: Class091); Class091 = null;
                if (Class092 != null) Class(columnName: "Class092", value: Class092); Class092 = null;
                if (Class093 != null) Class(columnName: "Class093", value: Class093); Class093 = null;
                if (Class094 != null) Class(columnName: "Class094", value: Class094); Class094 = null;
                if (Class095 != null) Class(columnName: "Class095", value: Class095); Class095 = null;
                if (Class096 != null) Class(columnName: "Class096", value: Class096); Class096 = null;
                if (Class097 != null) Class(columnName: "Class097", value: Class097); Class097 = null;
                if (Class098 != null) Class(columnName: "Class098", value: Class098); Class098 = null;
                if (Class099 != null) Class(columnName: "Class099", value: Class099); Class099 = null;
                if (Class100 != null) Class(columnName: "Class100", value: Class100); Class100 = null;
                if (NumA != null) Num(columnName: "NumA", value: NumA.ToDecimal()); NumA = null;
                if (NumB != null) Num(columnName: "NumB", value: NumB.ToDecimal()); NumB = null;
                if (NumC != null) Num(columnName: "NumC", value: NumC.ToDecimal()); NumC = null;
                if (NumD != null) Num(columnName: "NumD", value: NumD.ToDecimal()); NumD = null;
                if (NumE != null) Num(columnName: "NumE", value: NumE.ToDecimal()); NumE = null;
                if (NumF != null) Num(columnName: "NumF", value: NumF.ToDecimal()); NumF = null;
                if (NumG != null) Num(columnName: "NumG", value: NumG.ToDecimal()); NumG = null;
                if (NumH != null) Num(columnName: "NumH", value: NumH.ToDecimal()); NumH = null;
                if (NumI != null) Num(columnName: "NumI", value: NumI.ToDecimal()); NumI = null;
                if (NumJ != null) Num(columnName: "NumJ", value: NumJ.ToDecimal()); NumJ = null;
                if (NumK != null) Num(columnName: "NumK", value: NumK.ToDecimal()); NumK = null;
                if (NumL != null) Num(columnName: "NumL", value: NumL.ToDecimal()); NumL = null;
                if (NumM != null) Num(columnName: "NumM", value: NumM.ToDecimal()); NumM = null;
                if (NumN != null) Num(columnName: "NumN", value: NumN.ToDecimal()); NumN = null;
                if (NumO != null) Num(columnName: "NumO", value: NumO.ToDecimal()); NumO = null;
                if (NumP != null) Num(columnName: "NumP", value: NumP.ToDecimal()); NumP = null;
                if (NumQ != null) Num(columnName: "NumQ", value: NumQ.ToDecimal()); NumQ = null;
                if (NumR != null) Num(columnName: "NumR", value: NumR.ToDecimal()); NumR = null;
                if (NumS != null) Num(columnName: "NumS", value: NumS.ToDecimal()); NumS = null;
                if (NumT != null) Num(columnName: "NumT", value: NumT.ToDecimal()); NumT = null;
                if (NumU != null) Num(columnName: "NumU", value: NumU.ToDecimal()); NumU = null;
                if (NumV != null) Num(columnName: "NumV", value: NumV.ToDecimal()); NumV = null;
                if (NumW != null) Num(columnName: "NumW", value: NumW.ToDecimal()); NumW = null;
                if (NumX != null) Num(columnName: "NumX", value: NumX.ToDecimal()); NumX = null;
                if (NumY != null) Num(columnName: "NumY", value: NumY.ToDecimal()); NumY = null;
                if (NumZ != null) Num(columnName: "NumZ", value: NumZ.ToDecimal()); NumZ = null;
                if (Num001 != null) Num(columnName: "Num001", value: Num001.ToDecimal()); Num001 = null;
                if (Num002 != null) Num(columnName: "Num002", value: Num002.ToDecimal()); Num002 = null;
                if (Num003 != null) Num(columnName: "Num003", value: Num003.ToDecimal()); Num003 = null;
                if (Num004 != null) Num(columnName: "Num004", value: Num004.ToDecimal()); Num004 = null;
                if (Num005 != null) Num(columnName: "Num005", value: Num005.ToDecimal()); Num005 = null;
                if (Num006 != null) Num(columnName: "Num006", value: Num006.ToDecimal()); Num006 = null;
                if (Num007 != null) Num(columnName: "Num007", value: Num007.ToDecimal()); Num007 = null;
                if (Num008 != null) Num(columnName: "Num008", value: Num008.ToDecimal()); Num008 = null;
                if (Num009 != null) Num(columnName: "Num009", value: Num009.ToDecimal()); Num009 = null;
                if (Num010 != null) Num(columnName: "Num010", value: Num010.ToDecimal()); Num010 = null;
                if (Num011 != null) Num(columnName: "Num011", value: Num011.ToDecimal()); Num011 = null;
                if (Num012 != null) Num(columnName: "Num012", value: Num012.ToDecimal()); Num012 = null;
                if (Num013 != null) Num(columnName: "Num013", value: Num013.ToDecimal()); Num013 = null;
                if (Num014 != null) Num(columnName: "Num014", value: Num014.ToDecimal()); Num014 = null;
                if (Num015 != null) Num(columnName: "Num015", value: Num015.ToDecimal()); Num015 = null;
                if (Num016 != null) Num(columnName: "Num016", value: Num016.ToDecimal()); Num016 = null;
                if (Num017 != null) Num(columnName: "Num017", value: Num017.ToDecimal()); Num017 = null;
                if (Num018 != null) Num(columnName: "Num018", value: Num018.ToDecimal()); Num018 = null;
                if (Num019 != null) Num(columnName: "Num019", value: Num019.ToDecimal()); Num019 = null;
                if (Num020 != null) Num(columnName: "Num020", value: Num020.ToDecimal()); Num020 = null;
                if (Num021 != null) Num(columnName: "Num021", value: Num021.ToDecimal()); Num021 = null;
                if (Num022 != null) Num(columnName: "Num022", value: Num022.ToDecimal()); Num022 = null;
                if (Num023 != null) Num(columnName: "Num023", value: Num023.ToDecimal()); Num023 = null;
                if (Num024 != null) Num(columnName: "Num024", value: Num024.ToDecimal()); Num024 = null;
                if (Num025 != null) Num(columnName: "Num025", value: Num025.ToDecimal()); Num025 = null;
                if (Num026 != null) Num(columnName: "Num026", value: Num026.ToDecimal()); Num026 = null;
                if (Num027 != null) Num(columnName: "Num027", value: Num027.ToDecimal()); Num027 = null;
                if (Num028 != null) Num(columnName: "Num028", value: Num028.ToDecimal()); Num028 = null;
                if (Num029 != null) Num(columnName: "Num029", value: Num029.ToDecimal()); Num029 = null;
                if (Num030 != null) Num(columnName: "Num030", value: Num030.ToDecimal()); Num030 = null;
                if (Num031 != null) Num(columnName: "Num031", value: Num031.ToDecimal()); Num031 = null;
                if (Num032 != null) Num(columnName: "Num032", value: Num032.ToDecimal()); Num032 = null;
                if (Num033 != null) Num(columnName: "Num033", value: Num033.ToDecimal()); Num033 = null;
                if (Num034 != null) Num(columnName: "Num034", value: Num034.ToDecimal()); Num034 = null;
                if (Num035 != null) Num(columnName: "Num035", value: Num035.ToDecimal()); Num035 = null;
                if (Num036 != null) Num(columnName: "Num036", value: Num036.ToDecimal()); Num036 = null;
                if (Num037 != null) Num(columnName: "Num037", value: Num037.ToDecimal()); Num037 = null;
                if (Num038 != null) Num(columnName: "Num038", value: Num038.ToDecimal()); Num038 = null;
                if (Num039 != null) Num(columnName: "Num039", value: Num039.ToDecimal()); Num039 = null;
                if (Num040 != null) Num(columnName: "Num040", value: Num040.ToDecimal()); Num040 = null;
                if (Num041 != null) Num(columnName: "Num041", value: Num041.ToDecimal()); Num041 = null;
                if (Num042 != null) Num(columnName: "Num042", value: Num042.ToDecimal()); Num042 = null;
                if (Num043 != null) Num(columnName: "Num043", value: Num043.ToDecimal()); Num043 = null;
                if (Num044 != null) Num(columnName: "Num044", value: Num044.ToDecimal()); Num044 = null;
                if (Num045 != null) Num(columnName: "Num045", value: Num045.ToDecimal()); Num045 = null;
                if (Num046 != null) Num(columnName: "Num046", value: Num046.ToDecimal()); Num046 = null;
                if (Num047 != null) Num(columnName: "Num047", value: Num047.ToDecimal()); Num047 = null;
                if (Num048 != null) Num(columnName: "Num048", value: Num048.ToDecimal()); Num048 = null;
                if (Num049 != null) Num(columnName: "Num049", value: Num049.ToDecimal()); Num049 = null;
                if (Num050 != null) Num(columnName: "Num050", value: Num050.ToDecimal()); Num050 = null;
                if (Num051 != null) Num(columnName: "Num051", value: Num051.ToDecimal()); Num051 = null;
                if (Num052 != null) Num(columnName: "Num052", value: Num052.ToDecimal()); Num052 = null;
                if (Num053 != null) Num(columnName: "Num053", value: Num053.ToDecimal()); Num053 = null;
                if (Num054 != null) Num(columnName: "Num054", value: Num054.ToDecimal()); Num054 = null;
                if (Num055 != null) Num(columnName: "Num055", value: Num055.ToDecimal()); Num055 = null;
                if (Num056 != null) Num(columnName: "Num056", value: Num056.ToDecimal()); Num056 = null;
                if (Num057 != null) Num(columnName: "Num057", value: Num057.ToDecimal()); Num057 = null;
                if (Num058 != null) Num(columnName: "Num058", value: Num058.ToDecimal()); Num058 = null;
                if (Num059 != null) Num(columnName: "Num059", value: Num059.ToDecimal()); Num059 = null;
                if (Num060 != null) Num(columnName: "Num060", value: Num060.ToDecimal()); Num060 = null;
                if (Num061 != null) Num(columnName: "Num061", value: Num061.ToDecimal()); Num061 = null;
                if (Num062 != null) Num(columnName: "Num062", value: Num062.ToDecimal()); Num062 = null;
                if (Num063 != null) Num(columnName: "Num063", value: Num063.ToDecimal()); Num063 = null;
                if (Num064 != null) Num(columnName: "Num064", value: Num064.ToDecimal()); Num064 = null;
                if (Num065 != null) Num(columnName: "Num065", value: Num065.ToDecimal()); Num065 = null;
                if (Num066 != null) Num(columnName: "Num066", value: Num066.ToDecimal()); Num066 = null;
                if (Num067 != null) Num(columnName: "Num067", value: Num067.ToDecimal()); Num067 = null;
                if (Num068 != null) Num(columnName: "Num068", value: Num068.ToDecimal()); Num068 = null;
                if (Num069 != null) Num(columnName: "Num069", value: Num069.ToDecimal()); Num069 = null;
                if (Num070 != null) Num(columnName: "Num070", value: Num070.ToDecimal()); Num070 = null;
                if (Num071 != null) Num(columnName: "Num071", value: Num071.ToDecimal()); Num071 = null;
                if (Num072 != null) Num(columnName: "Num072", value: Num072.ToDecimal()); Num072 = null;
                if (Num073 != null) Num(columnName: "Num073", value: Num073.ToDecimal()); Num073 = null;
                if (Num074 != null) Num(columnName: "Num074", value: Num074.ToDecimal()); Num074 = null;
                if (Num075 != null) Num(columnName: "Num075", value: Num075.ToDecimal()); Num075 = null;
                if (Num076 != null) Num(columnName: "Num076", value: Num076.ToDecimal()); Num076 = null;
                if (Num077 != null) Num(columnName: "Num077", value: Num077.ToDecimal()); Num077 = null;
                if (Num078 != null) Num(columnName: "Num078", value: Num078.ToDecimal()); Num078 = null;
                if (Num079 != null) Num(columnName: "Num079", value: Num079.ToDecimal()); Num079 = null;
                if (Num080 != null) Num(columnName: "Num080", value: Num080.ToDecimal()); Num080 = null;
                if (Num081 != null) Num(columnName: "Num081", value: Num081.ToDecimal()); Num081 = null;
                if (Num082 != null) Num(columnName: "Num082", value: Num082.ToDecimal()); Num082 = null;
                if (Num083 != null) Num(columnName: "Num083", value: Num083.ToDecimal()); Num083 = null;
                if (Num084 != null) Num(columnName: "Num084", value: Num084.ToDecimal()); Num084 = null;
                if (Num085 != null) Num(columnName: "Num085", value: Num085.ToDecimal()); Num085 = null;
                if (Num086 != null) Num(columnName: "Num086", value: Num086.ToDecimal()); Num086 = null;
                if (Num087 != null) Num(columnName: "Num087", value: Num087.ToDecimal()); Num087 = null;
                if (Num088 != null) Num(columnName: "Num088", value: Num088.ToDecimal()); Num088 = null;
                if (Num089 != null) Num(columnName: "Num089", value: Num089.ToDecimal()); Num089 = null;
                if (Num090 != null) Num(columnName: "Num090", value: Num090.ToDecimal()); Num090 = null;
                if (Num091 != null) Num(columnName: "Num091", value: Num091.ToDecimal()); Num091 = null;
                if (Num092 != null) Num(columnName: "Num092", value: Num092.ToDecimal()); Num092 = null;
                if (Num093 != null) Num(columnName: "Num093", value: Num093.ToDecimal()); Num093 = null;
                if (Num094 != null) Num(columnName: "Num094", value: Num094.ToDecimal()); Num094 = null;
                if (Num095 != null) Num(columnName: "Num095", value: Num095.ToDecimal()); Num095 = null;
                if (Num096 != null) Num(columnName: "Num096", value: Num096.ToDecimal()); Num096 = null;
                if (Num097 != null) Num(columnName: "Num097", value: Num097.ToDecimal()); Num097 = null;
                if (Num098 != null) Num(columnName: "Num098", value: Num098.ToDecimal()); Num098 = null;
                if (Num099 != null) Num(columnName: "Num099", value: Num099.ToDecimal()); Num099 = null;
                if (Num100 != null) Num(columnName: "Num100", value: Num100.ToDecimal()); Num100 = null;
                if (DateA != null) Date(columnName: "DateA", value: DateA.ToDateTime()); DateA = null;
                if (DateB != null) Date(columnName: "DateB", value: DateB.ToDateTime()); DateB = null;
                if (DateC != null) Date(columnName: "DateC", value: DateC.ToDateTime()); DateC = null;
                if (DateD != null) Date(columnName: "DateD", value: DateD.ToDateTime()); DateD = null;
                if (DateE != null) Date(columnName: "DateE", value: DateE.ToDateTime()); DateE = null;
                if (DateF != null) Date(columnName: "DateF", value: DateF.ToDateTime()); DateF = null;
                if (DateG != null) Date(columnName: "DateG", value: DateG.ToDateTime()); DateG = null;
                if (DateH != null) Date(columnName: "DateH", value: DateH.ToDateTime()); DateH = null;
                if (DateI != null) Date(columnName: "DateI", value: DateI.ToDateTime()); DateI = null;
                if (DateJ != null) Date(columnName: "DateJ", value: DateJ.ToDateTime()); DateJ = null;
                if (DateK != null) Date(columnName: "DateK", value: DateK.ToDateTime()); DateK = null;
                if (DateL != null) Date(columnName: "DateL", value: DateL.ToDateTime()); DateL = null;
                if (DateM != null) Date(columnName: "DateM", value: DateM.ToDateTime()); DateM = null;
                if (DateN != null) Date(columnName: "DateN", value: DateN.ToDateTime()); DateN = null;
                if (DateO != null) Date(columnName: "DateO", value: DateO.ToDateTime()); DateO = null;
                if (DateP != null) Date(columnName: "DateP", value: DateP.ToDateTime()); DateP = null;
                if (DateQ != null) Date(columnName: "DateQ", value: DateQ.ToDateTime()); DateQ = null;
                if (DateR != null) Date(columnName: "DateR", value: DateR.ToDateTime()); DateR = null;
                if (DateS != null) Date(columnName: "DateS", value: DateS.ToDateTime()); DateS = null;
                if (DateT != null) Date(columnName: "DateT", value: DateT.ToDateTime()); DateT = null;
                if (DateU != null) Date(columnName: "DateU", value: DateU.ToDateTime()); DateU = null;
                if (DateV != null) Date(columnName: "DateV", value: DateV.ToDateTime()); DateV = null;
                if (DateW != null) Date(columnName: "DateW", value: DateW.ToDateTime()); DateW = null;
                if (DateX != null) Date(columnName: "DateX", value: DateX.ToDateTime()); DateX = null;
                if (DateY != null) Date(columnName: "DateY", value: DateY.ToDateTime()); DateY = null;
                if (DateZ != null) Date(columnName: "DateZ", value: DateZ.ToDateTime()); DateZ = null;
                if (Date001 != null) Date(columnName: "Date001", value: Date001.ToDateTime()); Date001 = null;
                if (Date002 != null) Date(columnName: "Date002", value: Date002.ToDateTime()); Date002 = null;
                if (Date003 != null) Date(columnName: "Date003", value: Date003.ToDateTime()); Date003 = null;
                if (Date004 != null) Date(columnName: "Date004", value: Date004.ToDateTime()); Date004 = null;
                if (Date005 != null) Date(columnName: "Date005", value: Date005.ToDateTime()); Date005 = null;
                if (Date006 != null) Date(columnName: "Date006", value: Date006.ToDateTime()); Date006 = null;
                if (Date007 != null) Date(columnName: "Date007", value: Date007.ToDateTime()); Date007 = null;
                if (Date008 != null) Date(columnName: "Date008", value: Date008.ToDateTime()); Date008 = null;
                if (Date009 != null) Date(columnName: "Date009", value: Date009.ToDateTime()); Date009 = null;
                if (Date010 != null) Date(columnName: "Date010", value: Date010.ToDateTime()); Date010 = null;
                if (Date011 != null) Date(columnName: "Date011", value: Date011.ToDateTime()); Date011 = null;
                if (Date012 != null) Date(columnName: "Date012", value: Date012.ToDateTime()); Date012 = null;
                if (Date013 != null) Date(columnName: "Date013", value: Date013.ToDateTime()); Date013 = null;
                if (Date014 != null) Date(columnName: "Date014", value: Date014.ToDateTime()); Date014 = null;
                if (Date015 != null) Date(columnName: "Date015", value: Date015.ToDateTime()); Date015 = null;
                if (Date016 != null) Date(columnName: "Date016", value: Date016.ToDateTime()); Date016 = null;
                if (Date017 != null) Date(columnName: "Date017", value: Date017.ToDateTime()); Date017 = null;
                if (Date018 != null) Date(columnName: "Date018", value: Date018.ToDateTime()); Date018 = null;
                if (Date019 != null) Date(columnName: "Date019", value: Date019.ToDateTime()); Date019 = null;
                if (Date020 != null) Date(columnName: "Date020", value: Date020.ToDateTime()); Date020 = null;
                if (Date021 != null) Date(columnName: "Date021", value: Date021.ToDateTime()); Date021 = null;
                if (Date022 != null) Date(columnName: "Date022", value: Date022.ToDateTime()); Date022 = null;
                if (Date023 != null) Date(columnName: "Date023", value: Date023.ToDateTime()); Date023 = null;
                if (Date024 != null) Date(columnName: "Date024", value: Date024.ToDateTime()); Date024 = null;
                if (Date025 != null) Date(columnName: "Date025", value: Date025.ToDateTime()); Date025 = null;
                if (Date026 != null) Date(columnName: "Date026", value: Date026.ToDateTime()); Date026 = null;
                if (Date027 != null) Date(columnName: "Date027", value: Date027.ToDateTime()); Date027 = null;
                if (Date028 != null) Date(columnName: "Date028", value: Date028.ToDateTime()); Date028 = null;
                if (Date029 != null) Date(columnName: "Date029", value: Date029.ToDateTime()); Date029 = null;
                if (Date030 != null) Date(columnName: "Date030", value: Date030.ToDateTime()); Date030 = null;
                if (Date031 != null) Date(columnName: "Date031", value: Date031.ToDateTime()); Date031 = null;
                if (Date032 != null) Date(columnName: "Date032", value: Date032.ToDateTime()); Date032 = null;
                if (Date033 != null) Date(columnName: "Date033", value: Date033.ToDateTime()); Date033 = null;
                if (Date034 != null) Date(columnName: "Date034", value: Date034.ToDateTime()); Date034 = null;
                if (Date035 != null) Date(columnName: "Date035", value: Date035.ToDateTime()); Date035 = null;
                if (Date036 != null) Date(columnName: "Date036", value: Date036.ToDateTime()); Date036 = null;
                if (Date037 != null) Date(columnName: "Date037", value: Date037.ToDateTime()); Date037 = null;
                if (Date038 != null) Date(columnName: "Date038", value: Date038.ToDateTime()); Date038 = null;
                if (Date039 != null) Date(columnName: "Date039", value: Date039.ToDateTime()); Date039 = null;
                if (Date040 != null) Date(columnName: "Date040", value: Date040.ToDateTime()); Date040 = null;
                if (Date041 != null) Date(columnName: "Date041", value: Date041.ToDateTime()); Date041 = null;
                if (Date042 != null) Date(columnName: "Date042", value: Date042.ToDateTime()); Date042 = null;
                if (Date043 != null) Date(columnName: "Date043", value: Date043.ToDateTime()); Date043 = null;
                if (Date044 != null) Date(columnName: "Date044", value: Date044.ToDateTime()); Date044 = null;
                if (Date045 != null) Date(columnName: "Date045", value: Date045.ToDateTime()); Date045 = null;
                if (Date046 != null) Date(columnName: "Date046", value: Date046.ToDateTime()); Date046 = null;
                if (Date047 != null) Date(columnName: "Date047", value: Date047.ToDateTime()); Date047 = null;
                if (Date048 != null) Date(columnName: "Date048", value: Date048.ToDateTime()); Date048 = null;
                if (Date049 != null) Date(columnName: "Date049", value: Date049.ToDateTime()); Date049 = null;
                if (Date050 != null) Date(columnName: "Date050", value: Date050.ToDateTime()); Date050 = null;
                if (Date051 != null) Date(columnName: "Date051", value: Date051.ToDateTime()); Date051 = null;
                if (Date052 != null) Date(columnName: "Date052", value: Date052.ToDateTime()); Date052 = null;
                if (Date053 != null) Date(columnName: "Date053", value: Date053.ToDateTime()); Date053 = null;
                if (Date054 != null) Date(columnName: "Date054", value: Date054.ToDateTime()); Date054 = null;
                if (Date055 != null) Date(columnName: "Date055", value: Date055.ToDateTime()); Date055 = null;
                if (Date056 != null) Date(columnName: "Date056", value: Date056.ToDateTime()); Date056 = null;
                if (Date057 != null) Date(columnName: "Date057", value: Date057.ToDateTime()); Date057 = null;
                if (Date058 != null) Date(columnName: "Date058", value: Date058.ToDateTime()); Date058 = null;
                if (Date059 != null) Date(columnName: "Date059", value: Date059.ToDateTime()); Date059 = null;
                if (Date060 != null) Date(columnName: "Date060", value: Date060.ToDateTime()); Date060 = null;
                if (Date061 != null) Date(columnName: "Date061", value: Date061.ToDateTime()); Date061 = null;
                if (Date062 != null) Date(columnName: "Date062", value: Date062.ToDateTime()); Date062 = null;
                if (Date063 != null) Date(columnName: "Date063", value: Date063.ToDateTime()); Date063 = null;
                if (Date064 != null) Date(columnName: "Date064", value: Date064.ToDateTime()); Date064 = null;
                if (Date065 != null) Date(columnName: "Date065", value: Date065.ToDateTime()); Date065 = null;
                if (Date066 != null) Date(columnName: "Date066", value: Date066.ToDateTime()); Date066 = null;
                if (Date067 != null) Date(columnName: "Date067", value: Date067.ToDateTime()); Date067 = null;
                if (Date068 != null) Date(columnName: "Date068", value: Date068.ToDateTime()); Date068 = null;
                if (Date069 != null) Date(columnName: "Date069", value: Date069.ToDateTime()); Date069 = null;
                if (Date070 != null) Date(columnName: "Date070", value: Date070.ToDateTime()); Date070 = null;
                if (Date071 != null) Date(columnName: "Date071", value: Date071.ToDateTime()); Date071 = null;
                if (Date072 != null) Date(columnName: "Date072", value: Date072.ToDateTime()); Date072 = null;
                if (Date073 != null) Date(columnName: "Date073", value: Date073.ToDateTime()); Date073 = null;
                if (Date074 != null) Date(columnName: "Date074", value: Date074.ToDateTime()); Date074 = null;
                if (Date075 != null) Date(columnName: "Date075", value: Date075.ToDateTime()); Date075 = null;
                if (Date076 != null) Date(columnName: "Date076", value: Date076.ToDateTime()); Date076 = null;
                if (Date077 != null) Date(columnName: "Date077", value: Date077.ToDateTime()); Date077 = null;
                if (Date078 != null) Date(columnName: "Date078", value: Date078.ToDateTime()); Date078 = null;
                if (Date079 != null) Date(columnName: "Date079", value: Date079.ToDateTime()); Date079 = null;
                if (Date080 != null) Date(columnName: "Date080", value: Date080.ToDateTime()); Date080 = null;
                if (Date081 != null) Date(columnName: "Date081", value: Date081.ToDateTime()); Date081 = null;
                if (Date082 != null) Date(columnName: "Date082", value: Date082.ToDateTime()); Date082 = null;
                if (Date083 != null) Date(columnName: "Date083", value: Date083.ToDateTime()); Date083 = null;
                if (Date084 != null) Date(columnName: "Date084", value: Date084.ToDateTime()); Date084 = null;
                if (Date085 != null) Date(columnName: "Date085", value: Date085.ToDateTime()); Date085 = null;
                if (Date086 != null) Date(columnName: "Date086", value: Date086.ToDateTime()); Date086 = null;
                if (Date087 != null) Date(columnName: "Date087", value: Date087.ToDateTime()); Date087 = null;
                if (Date088 != null) Date(columnName: "Date088", value: Date088.ToDateTime()); Date088 = null;
                if (Date089 != null) Date(columnName: "Date089", value: Date089.ToDateTime()); Date089 = null;
                if (Date090 != null) Date(columnName: "Date090", value: Date090.ToDateTime()); Date090 = null;
                if (Date091 != null) Date(columnName: "Date091", value: Date091.ToDateTime()); Date091 = null;
                if (Date092 != null) Date(columnName: "Date092", value: Date092.ToDateTime()); Date092 = null;
                if (Date093 != null) Date(columnName: "Date093", value: Date093.ToDateTime()); Date093 = null;
                if (Date094 != null) Date(columnName: "Date094", value: Date094.ToDateTime()); Date094 = null;
                if (Date095 != null) Date(columnName: "Date095", value: Date095.ToDateTime()); Date095 = null;
                if (Date096 != null) Date(columnName: "Date096", value: Date096.ToDateTime()); Date096 = null;
                if (Date097 != null) Date(columnName: "Date097", value: Date097.ToDateTime()); Date097 = null;
                if (Date098 != null) Date(columnName: "Date098", value: Date098.ToDateTime()); Date098 = null;
                if (Date099 != null) Date(columnName: "Date099", value: Date099.ToDateTime()); Date099 = null;
                if (Date100 != null) Date(columnName: "Date100", value: Date100.ToDateTime()); Date100 = null;
                if (DescriptionA != null) Description(columnName: "DescriptionA", value: DescriptionA); DescriptionA = null;
                if (DescriptionB != null) Description(columnName: "DescriptionB", value: DescriptionB); DescriptionB = null;
                if (DescriptionC != null) Description(columnName: "DescriptionC", value: DescriptionC); DescriptionC = null;
                if (DescriptionD != null) Description(columnName: "DescriptionD", value: DescriptionD); DescriptionD = null;
                if (DescriptionE != null) Description(columnName: "DescriptionE", value: DescriptionE); DescriptionE = null;
                if (DescriptionF != null) Description(columnName: "DescriptionF", value: DescriptionF); DescriptionF = null;
                if (DescriptionG != null) Description(columnName: "DescriptionG", value: DescriptionG); DescriptionG = null;
                if (DescriptionH != null) Description(columnName: "DescriptionH", value: DescriptionH); DescriptionH = null;
                if (DescriptionI != null) Description(columnName: "DescriptionI", value: DescriptionI); DescriptionI = null;
                if (DescriptionJ != null) Description(columnName: "DescriptionJ", value: DescriptionJ); DescriptionJ = null;
                if (DescriptionK != null) Description(columnName: "DescriptionK", value: DescriptionK); DescriptionK = null;
                if (DescriptionL != null) Description(columnName: "DescriptionL", value: DescriptionL); DescriptionL = null;
                if (DescriptionM != null) Description(columnName: "DescriptionM", value: DescriptionM); DescriptionM = null;
                if (DescriptionN != null) Description(columnName: "DescriptionN", value: DescriptionN); DescriptionN = null;
                if (DescriptionO != null) Description(columnName: "DescriptionO", value: DescriptionO); DescriptionO = null;
                if (DescriptionP != null) Description(columnName: "DescriptionP", value: DescriptionP); DescriptionP = null;
                if (DescriptionQ != null) Description(columnName: "DescriptionQ", value: DescriptionQ); DescriptionQ = null;
                if (DescriptionR != null) Description(columnName: "DescriptionR", value: DescriptionR); DescriptionR = null;
                if (DescriptionS != null) Description(columnName: "DescriptionS", value: DescriptionS); DescriptionS = null;
                if (DescriptionT != null) Description(columnName: "DescriptionT", value: DescriptionT); DescriptionT = null;
                if (DescriptionU != null) Description(columnName: "DescriptionU", value: DescriptionU); DescriptionU = null;
                if (DescriptionV != null) Description(columnName: "DescriptionV", value: DescriptionV); DescriptionV = null;
                if (DescriptionW != null) Description(columnName: "DescriptionW", value: DescriptionW); DescriptionW = null;
                if (DescriptionX != null) Description(columnName: "DescriptionX", value: DescriptionX); DescriptionX = null;
                if (DescriptionY != null) Description(columnName: "DescriptionY", value: DescriptionY); DescriptionY = null;
                if (DescriptionZ != null) Description(columnName: "DescriptionZ", value: DescriptionZ); DescriptionZ = null;
                if (Description001 != null) Description(columnName: "Description001", value: Description001); Description001 = null;
                if (Description002 != null) Description(columnName: "Description002", value: Description002); Description002 = null;
                if (Description003 != null) Description(columnName: "Description003", value: Description003); Description003 = null;
                if (Description004 != null) Description(columnName: "Description004", value: Description004); Description004 = null;
                if (Description005 != null) Description(columnName: "Description005", value: Description005); Description005 = null;
                if (Description006 != null) Description(columnName: "Description006", value: Description006); Description006 = null;
                if (Description007 != null) Description(columnName: "Description007", value: Description007); Description007 = null;
                if (Description008 != null) Description(columnName: "Description008", value: Description008); Description008 = null;
                if (Description009 != null) Description(columnName: "Description009", value: Description009); Description009 = null;
                if (Description010 != null) Description(columnName: "Description010", value: Description010); Description010 = null;
                if (Description011 != null) Description(columnName: "Description011", value: Description011); Description011 = null;
                if (Description012 != null) Description(columnName: "Description012", value: Description012); Description012 = null;
                if (Description013 != null) Description(columnName: "Description013", value: Description013); Description013 = null;
                if (Description014 != null) Description(columnName: "Description014", value: Description014); Description014 = null;
                if (Description015 != null) Description(columnName: "Description015", value: Description015); Description015 = null;
                if (Description016 != null) Description(columnName: "Description016", value: Description016); Description016 = null;
                if (Description017 != null) Description(columnName: "Description017", value: Description017); Description017 = null;
                if (Description018 != null) Description(columnName: "Description018", value: Description018); Description018 = null;
                if (Description019 != null) Description(columnName: "Description019", value: Description019); Description019 = null;
                if (Description020 != null) Description(columnName: "Description020", value: Description020); Description020 = null;
                if (Description021 != null) Description(columnName: "Description021", value: Description021); Description021 = null;
                if (Description022 != null) Description(columnName: "Description022", value: Description022); Description022 = null;
                if (Description023 != null) Description(columnName: "Description023", value: Description023); Description023 = null;
                if (Description024 != null) Description(columnName: "Description024", value: Description024); Description024 = null;
                if (Description025 != null) Description(columnName: "Description025", value: Description025); Description025 = null;
                if (Description026 != null) Description(columnName: "Description026", value: Description026); Description026 = null;
                if (Description027 != null) Description(columnName: "Description027", value: Description027); Description027 = null;
                if (Description028 != null) Description(columnName: "Description028", value: Description028); Description028 = null;
                if (Description029 != null) Description(columnName: "Description029", value: Description029); Description029 = null;
                if (Description030 != null) Description(columnName: "Description030", value: Description030); Description030 = null;
                if (Description031 != null) Description(columnName: "Description031", value: Description031); Description031 = null;
                if (Description032 != null) Description(columnName: "Description032", value: Description032); Description032 = null;
                if (Description033 != null) Description(columnName: "Description033", value: Description033); Description033 = null;
                if (Description034 != null) Description(columnName: "Description034", value: Description034); Description034 = null;
                if (Description035 != null) Description(columnName: "Description035", value: Description035); Description035 = null;
                if (Description036 != null) Description(columnName: "Description036", value: Description036); Description036 = null;
                if (Description037 != null) Description(columnName: "Description037", value: Description037); Description037 = null;
                if (Description038 != null) Description(columnName: "Description038", value: Description038); Description038 = null;
                if (Description039 != null) Description(columnName: "Description039", value: Description039); Description039 = null;
                if (Description040 != null) Description(columnName: "Description040", value: Description040); Description040 = null;
                if (Description041 != null) Description(columnName: "Description041", value: Description041); Description041 = null;
                if (Description042 != null) Description(columnName: "Description042", value: Description042); Description042 = null;
                if (Description043 != null) Description(columnName: "Description043", value: Description043); Description043 = null;
                if (Description044 != null) Description(columnName: "Description044", value: Description044); Description044 = null;
                if (Description045 != null) Description(columnName: "Description045", value: Description045); Description045 = null;
                if (Description046 != null) Description(columnName: "Description046", value: Description046); Description046 = null;
                if (Description047 != null) Description(columnName: "Description047", value: Description047); Description047 = null;
                if (Description048 != null) Description(columnName: "Description048", value: Description048); Description048 = null;
                if (Description049 != null) Description(columnName: "Description049", value: Description049); Description049 = null;
                if (Description050 != null) Description(columnName: "Description050", value: Description050); Description050 = null;
                if (Description051 != null) Description(columnName: "Description051", value: Description051); Description051 = null;
                if (Description052 != null) Description(columnName: "Description052", value: Description052); Description052 = null;
                if (Description053 != null) Description(columnName: "Description053", value: Description053); Description053 = null;
                if (Description054 != null) Description(columnName: "Description054", value: Description054); Description054 = null;
                if (Description055 != null) Description(columnName: "Description055", value: Description055); Description055 = null;
                if (Description056 != null) Description(columnName: "Description056", value: Description056); Description056 = null;
                if (Description057 != null) Description(columnName: "Description057", value: Description057); Description057 = null;
                if (Description058 != null) Description(columnName: "Description058", value: Description058); Description058 = null;
                if (Description059 != null) Description(columnName: "Description059", value: Description059); Description059 = null;
                if (Description060 != null) Description(columnName: "Description060", value: Description060); Description060 = null;
                if (Description061 != null) Description(columnName: "Description061", value: Description061); Description061 = null;
                if (Description062 != null) Description(columnName: "Description062", value: Description062); Description062 = null;
                if (Description063 != null) Description(columnName: "Description063", value: Description063); Description063 = null;
                if (Description064 != null) Description(columnName: "Description064", value: Description064); Description064 = null;
                if (Description065 != null) Description(columnName: "Description065", value: Description065); Description065 = null;
                if (Description066 != null) Description(columnName: "Description066", value: Description066); Description066 = null;
                if (Description067 != null) Description(columnName: "Description067", value: Description067); Description067 = null;
                if (Description068 != null) Description(columnName: "Description068", value: Description068); Description068 = null;
                if (Description069 != null) Description(columnName: "Description069", value: Description069); Description069 = null;
                if (Description070 != null) Description(columnName: "Description070", value: Description070); Description070 = null;
                if (Description071 != null) Description(columnName: "Description071", value: Description071); Description071 = null;
                if (Description072 != null) Description(columnName: "Description072", value: Description072); Description072 = null;
                if (Description073 != null) Description(columnName: "Description073", value: Description073); Description073 = null;
                if (Description074 != null) Description(columnName: "Description074", value: Description074); Description074 = null;
                if (Description075 != null) Description(columnName: "Description075", value: Description075); Description075 = null;
                if (Description076 != null) Description(columnName: "Description076", value: Description076); Description076 = null;
                if (Description077 != null) Description(columnName: "Description077", value: Description077); Description077 = null;
                if (Description078 != null) Description(columnName: "Description078", value: Description078); Description078 = null;
                if (Description079 != null) Description(columnName: "Description079", value: Description079); Description079 = null;
                if (Description080 != null) Description(columnName: "Description080", value: Description080); Description080 = null;
                if (Description081 != null) Description(columnName: "Description081", value: Description081); Description081 = null;
                if (Description082 != null) Description(columnName: "Description082", value: Description082); Description082 = null;
                if (Description083 != null) Description(columnName: "Description083", value: Description083); Description083 = null;
                if (Description084 != null) Description(columnName: "Description084", value: Description084); Description084 = null;
                if (Description085 != null) Description(columnName: "Description085", value: Description085); Description085 = null;
                if (Description086 != null) Description(columnName: "Description086", value: Description086); Description086 = null;
                if (Description087 != null) Description(columnName: "Description087", value: Description087); Description087 = null;
                if (Description088 != null) Description(columnName: "Description088", value: Description088); Description088 = null;
                if (Description089 != null) Description(columnName: "Description089", value: Description089); Description089 = null;
                if (Description090 != null) Description(columnName: "Description090", value: Description090); Description090 = null;
                if (Description091 != null) Description(columnName: "Description091", value: Description091); Description091 = null;
                if (Description092 != null) Description(columnName: "Description092", value: Description092); Description092 = null;
                if (Description093 != null) Description(columnName: "Description093", value: Description093); Description093 = null;
                if (Description094 != null) Description(columnName: "Description094", value: Description094); Description094 = null;
                if (Description095 != null) Description(columnName: "Description095", value: Description095); Description095 = null;
                if (Description096 != null) Description(columnName: "Description096", value: Description096); Description096 = null;
                if (Description097 != null) Description(columnName: "Description097", value: Description097); Description097 = null;
                if (Description098 != null) Description(columnName: "Description098", value: Description098); Description098 = null;
                if (Description099 != null) Description(columnName: "Description099", value: Description099); Description099 = null;
                if (Description100 != null) Description(columnName: "Description100", value: Description100); Description100 = null;
                if (CheckA != null) Check(columnName: "CheckA", value: CheckA.ToBool()); CheckA = null;
                if (CheckB != null) Check(columnName: "CheckB", value: CheckB.ToBool()); CheckB = null;
                if (CheckC != null) Check(columnName: "CheckC", value: CheckC.ToBool()); CheckC = null;
                if (CheckD != null) Check(columnName: "CheckD", value: CheckD.ToBool()); CheckD = null;
                if (CheckE != null) Check(columnName: "CheckE", value: CheckE.ToBool()); CheckE = null;
                if (CheckF != null) Check(columnName: "CheckF", value: CheckF.ToBool()); CheckF = null;
                if (CheckG != null) Check(columnName: "CheckG", value: CheckG.ToBool()); CheckG = null;
                if (CheckH != null) Check(columnName: "CheckH", value: CheckH.ToBool()); CheckH = null;
                if (CheckI != null) Check(columnName: "CheckI", value: CheckI.ToBool()); CheckI = null;
                if (CheckJ != null) Check(columnName: "CheckJ", value: CheckJ.ToBool()); CheckJ = null;
                if (CheckK != null) Check(columnName: "CheckK", value: CheckK.ToBool()); CheckK = null;
                if (CheckL != null) Check(columnName: "CheckL", value: CheckL.ToBool()); CheckL = null;
                if (CheckM != null) Check(columnName: "CheckM", value: CheckM.ToBool()); CheckM = null;
                if (CheckN != null) Check(columnName: "CheckN", value: CheckN.ToBool()); CheckN = null;
                if (CheckO != null) Check(columnName: "CheckO", value: CheckO.ToBool()); CheckO = null;
                if (CheckP != null) Check(columnName: "CheckP", value: CheckP.ToBool()); CheckP = null;
                if (CheckQ != null) Check(columnName: "CheckQ", value: CheckQ.ToBool()); CheckQ = null;
                if (CheckR != null) Check(columnName: "CheckR", value: CheckR.ToBool()); CheckR = null;
                if (CheckS != null) Check(columnName: "CheckS", value: CheckS.ToBool()); CheckS = null;
                if (CheckT != null) Check(columnName: "CheckT", value: CheckT.ToBool()); CheckT = null;
                if (CheckU != null) Check(columnName: "CheckU", value: CheckU.ToBool()); CheckU = null;
                if (CheckV != null) Check(columnName: "CheckV", value: CheckV.ToBool()); CheckV = null;
                if (CheckW != null) Check(columnName: "CheckW", value: CheckW.ToBool()); CheckW = null;
                if (CheckX != null) Check(columnName: "CheckX", value: CheckX.ToBool()); CheckX = null;
                if (CheckY != null) Check(columnName: "CheckY", value: CheckY.ToBool()); CheckY = null;
                if (CheckZ != null) Check(columnName: "CheckZ", value: CheckZ.ToBool()); CheckZ = null;
                if (Check001 != null) Check(columnName: "Check001", value: Check001.ToBool()); Check001 = null;
                if (Check002 != null) Check(columnName: "Check002", value: Check002.ToBool()); Check002 = null;
                if (Check003 != null) Check(columnName: "Check003", value: Check003.ToBool()); Check003 = null;
                if (Check004 != null) Check(columnName: "Check004", value: Check004.ToBool()); Check004 = null;
                if (Check005 != null) Check(columnName: "Check005", value: Check005.ToBool()); Check005 = null;
                if (Check006 != null) Check(columnName: "Check006", value: Check006.ToBool()); Check006 = null;
                if (Check007 != null) Check(columnName: "Check007", value: Check007.ToBool()); Check007 = null;
                if (Check008 != null) Check(columnName: "Check008", value: Check008.ToBool()); Check008 = null;
                if (Check009 != null) Check(columnName: "Check009", value: Check009.ToBool()); Check009 = null;
                if (Check010 != null) Check(columnName: "Check010", value: Check010.ToBool()); Check010 = null;
                if (Check011 != null) Check(columnName: "Check011", value: Check011.ToBool()); Check011 = null;
                if (Check012 != null) Check(columnName: "Check012", value: Check012.ToBool()); Check012 = null;
                if (Check013 != null) Check(columnName: "Check013", value: Check013.ToBool()); Check013 = null;
                if (Check014 != null) Check(columnName: "Check014", value: Check014.ToBool()); Check014 = null;
                if (Check015 != null) Check(columnName: "Check015", value: Check015.ToBool()); Check015 = null;
                if (Check016 != null) Check(columnName: "Check016", value: Check016.ToBool()); Check016 = null;
                if (Check017 != null) Check(columnName: "Check017", value: Check017.ToBool()); Check017 = null;
                if (Check018 != null) Check(columnName: "Check018", value: Check018.ToBool()); Check018 = null;
                if (Check019 != null) Check(columnName: "Check019", value: Check019.ToBool()); Check019 = null;
                if (Check020 != null) Check(columnName: "Check020", value: Check020.ToBool()); Check020 = null;
                if (Check021 != null) Check(columnName: "Check021", value: Check021.ToBool()); Check021 = null;
                if (Check022 != null) Check(columnName: "Check022", value: Check022.ToBool()); Check022 = null;
                if (Check023 != null) Check(columnName: "Check023", value: Check023.ToBool()); Check023 = null;
                if (Check024 != null) Check(columnName: "Check024", value: Check024.ToBool()); Check024 = null;
                if (Check025 != null) Check(columnName: "Check025", value: Check025.ToBool()); Check025 = null;
                if (Check026 != null) Check(columnName: "Check026", value: Check026.ToBool()); Check026 = null;
                if (Check027 != null) Check(columnName: "Check027", value: Check027.ToBool()); Check027 = null;
                if (Check028 != null) Check(columnName: "Check028", value: Check028.ToBool()); Check028 = null;
                if (Check029 != null) Check(columnName: "Check029", value: Check029.ToBool()); Check029 = null;
                if (Check030 != null) Check(columnName: "Check030", value: Check030.ToBool()); Check030 = null;
                if (Check031 != null) Check(columnName: "Check031", value: Check031.ToBool()); Check031 = null;
                if (Check032 != null) Check(columnName: "Check032", value: Check032.ToBool()); Check032 = null;
                if (Check033 != null) Check(columnName: "Check033", value: Check033.ToBool()); Check033 = null;
                if (Check034 != null) Check(columnName: "Check034", value: Check034.ToBool()); Check034 = null;
                if (Check035 != null) Check(columnName: "Check035", value: Check035.ToBool()); Check035 = null;
                if (Check036 != null) Check(columnName: "Check036", value: Check036.ToBool()); Check036 = null;
                if (Check037 != null) Check(columnName: "Check037", value: Check037.ToBool()); Check037 = null;
                if (Check038 != null) Check(columnName: "Check038", value: Check038.ToBool()); Check038 = null;
                if (Check039 != null) Check(columnName: "Check039", value: Check039.ToBool()); Check039 = null;
                if (Check040 != null) Check(columnName: "Check040", value: Check040.ToBool()); Check040 = null;
                if (Check041 != null) Check(columnName: "Check041", value: Check041.ToBool()); Check041 = null;
                if (Check042 != null) Check(columnName: "Check042", value: Check042.ToBool()); Check042 = null;
                if (Check043 != null) Check(columnName: "Check043", value: Check043.ToBool()); Check043 = null;
                if (Check044 != null) Check(columnName: "Check044", value: Check044.ToBool()); Check044 = null;
                if (Check045 != null) Check(columnName: "Check045", value: Check045.ToBool()); Check045 = null;
                if (Check046 != null) Check(columnName: "Check046", value: Check046.ToBool()); Check046 = null;
                if (Check047 != null) Check(columnName: "Check047", value: Check047.ToBool()); Check047 = null;
                if (Check048 != null) Check(columnName: "Check048", value: Check048.ToBool()); Check048 = null;
                if (Check049 != null) Check(columnName: "Check049", value: Check049.ToBool()); Check049 = null;
                if (Check050 != null) Check(columnName: "Check050", value: Check050.ToBool()); Check050 = null;
                if (Check051 != null) Check(columnName: "Check051", value: Check051.ToBool()); Check051 = null;
                if (Check052 != null) Check(columnName: "Check052", value: Check052.ToBool()); Check052 = null;
                if (Check053 != null) Check(columnName: "Check053", value: Check053.ToBool()); Check053 = null;
                if (Check054 != null) Check(columnName: "Check054", value: Check054.ToBool()); Check054 = null;
                if (Check055 != null) Check(columnName: "Check055", value: Check055.ToBool()); Check055 = null;
                if (Check056 != null) Check(columnName: "Check056", value: Check056.ToBool()); Check056 = null;
                if (Check057 != null) Check(columnName: "Check057", value: Check057.ToBool()); Check057 = null;
                if (Check058 != null) Check(columnName: "Check058", value: Check058.ToBool()); Check058 = null;
                if (Check059 != null) Check(columnName: "Check059", value: Check059.ToBool()); Check059 = null;
                if (Check060 != null) Check(columnName: "Check060", value: Check060.ToBool()); Check060 = null;
                if (Check061 != null) Check(columnName: "Check061", value: Check061.ToBool()); Check061 = null;
                if (Check062 != null) Check(columnName: "Check062", value: Check062.ToBool()); Check062 = null;
                if (Check063 != null) Check(columnName: "Check063", value: Check063.ToBool()); Check063 = null;
                if (Check064 != null) Check(columnName: "Check064", value: Check064.ToBool()); Check064 = null;
                if (Check065 != null) Check(columnName: "Check065", value: Check065.ToBool()); Check065 = null;
                if (Check066 != null) Check(columnName: "Check066", value: Check066.ToBool()); Check066 = null;
                if (Check067 != null) Check(columnName: "Check067", value: Check067.ToBool()); Check067 = null;
                if (Check068 != null) Check(columnName: "Check068", value: Check068.ToBool()); Check068 = null;
                if (Check069 != null) Check(columnName: "Check069", value: Check069.ToBool()); Check069 = null;
                if (Check070 != null) Check(columnName: "Check070", value: Check070.ToBool()); Check070 = null;
                if (Check071 != null) Check(columnName: "Check071", value: Check071.ToBool()); Check071 = null;
                if (Check072 != null) Check(columnName: "Check072", value: Check072.ToBool()); Check072 = null;
                if (Check073 != null) Check(columnName: "Check073", value: Check073.ToBool()); Check073 = null;
                if (Check074 != null) Check(columnName: "Check074", value: Check074.ToBool()); Check074 = null;
                if (Check075 != null) Check(columnName: "Check075", value: Check075.ToBool()); Check075 = null;
                if (Check076 != null) Check(columnName: "Check076", value: Check076.ToBool()); Check076 = null;
                if (Check077 != null) Check(columnName: "Check077", value: Check077.ToBool()); Check077 = null;
                if (Check078 != null) Check(columnName: "Check078", value: Check078.ToBool()); Check078 = null;
                if (Check079 != null) Check(columnName: "Check079", value: Check079.ToBool()); Check079 = null;
                if (Check080 != null) Check(columnName: "Check080", value: Check080.ToBool()); Check080 = null;
                if (Check081 != null) Check(columnName: "Check081", value: Check081.ToBool()); Check081 = null;
                if (Check082 != null) Check(columnName: "Check082", value: Check082.ToBool()); Check082 = null;
                if (Check083 != null) Check(columnName: "Check083", value: Check083.ToBool()); Check083 = null;
                if (Check084 != null) Check(columnName: "Check084", value: Check084.ToBool()); Check084 = null;
                if (Check085 != null) Check(columnName: "Check085", value: Check085.ToBool()); Check085 = null;
                if (Check086 != null) Check(columnName: "Check086", value: Check086.ToBool()); Check086 = null;
                if (Check087 != null) Check(columnName: "Check087", value: Check087.ToBool()); Check087 = null;
                if (Check088 != null) Check(columnName: "Check088", value: Check088.ToBool()); Check088 = null;
                if (Check089 != null) Check(columnName: "Check089", value: Check089.ToBool()); Check089 = null;
                if (Check090 != null) Check(columnName: "Check090", value: Check090.ToBool()); Check090 = null;
                if (Check091 != null) Check(columnName: "Check091", value: Check091.ToBool()); Check091 = null;
                if (Check092 != null) Check(columnName: "Check092", value: Check092.ToBool()); Check092 = null;
                if (Check093 != null) Check(columnName: "Check093", value: Check093.ToBool()); Check093 = null;
                if (Check094 != null) Check(columnName: "Check094", value: Check094.ToBool()); Check094 = null;
                if (Check095 != null) Check(columnName: "Check095", value: Check095.ToBool()); Check095 = null;
                if (Check096 != null) Check(columnName: "Check096", value: Check096.ToBool()); Check096 = null;
                if (Check097 != null) Check(columnName: "Check097", value: Check097.ToBool()); Check097 = null;
                if (Check098 != null) Check(columnName: "Check098", value: Check098.ToBool()); Check098 = null;
                if (Check099 != null) Check(columnName: "Check099", value: Check099.ToBool()); Check099 = null;
                if (Check100 != null) Check(columnName: "Check100", value: Check100.ToBool()); Check100 = null;
                if (AttachmentsA != null) Attachments(columnName: "AttachmentsA", value: AttachmentsA.Deserialize<Attachments>()); AttachmentsA = null;
                if (AttachmentsB != null) Attachments(columnName: "AttachmentsB", value: AttachmentsB.Deserialize<Attachments>()); AttachmentsB = null;
                if (AttachmentsC != null) Attachments(columnName: "AttachmentsC", value: AttachmentsC.Deserialize<Attachments>()); AttachmentsC = null;
                if (AttachmentsD != null) Attachments(columnName: "AttachmentsD", value: AttachmentsD.Deserialize<Attachments>()); AttachmentsD = null;
                if (AttachmentsE != null) Attachments(columnName: "AttachmentsE", value: AttachmentsE.Deserialize<Attachments>()); AttachmentsE = null;
                if (AttachmentsF != null) Attachments(columnName: "AttachmentsF", value: AttachmentsF.Deserialize<Attachments>()); AttachmentsF = null;
                if (AttachmentsG != null) Attachments(columnName: "AttachmentsG", value: AttachmentsG.Deserialize<Attachments>()); AttachmentsG = null;
                if (AttachmentsH != null) Attachments(columnName: "AttachmentsH", value: AttachmentsH.Deserialize<Attachments>()); AttachmentsH = null;
                if (AttachmentsI != null) Attachments(columnName: "AttachmentsI", value: AttachmentsI.Deserialize<Attachments>()); AttachmentsI = null;
                if (AttachmentsJ != null) Attachments(columnName: "AttachmentsJ", value: AttachmentsJ.Deserialize<Attachments>()); AttachmentsJ = null;
                if (AttachmentsK != null) Attachments(columnName: "AttachmentsK", value: AttachmentsK.Deserialize<Attachments>()); AttachmentsK = null;
                if (AttachmentsL != null) Attachments(columnName: "AttachmentsL", value: AttachmentsL.Deserialize<Attachments>()); AttachmentsL = null;
                if (AttachmentsM != null) Attachments(columnName: "AttachmentsM", value: AttachmentsM.Deserialize<Attachments>()); AttachmentsM = null;
                if (AttachmentsN != null) Attachments(columnName: "AttachmentsN", value: AttachmentsN.Deserialize<Attachments>()); AttachmentsN = null;
                if (AttachmentsO != null) Attachments(columnName: "AttachmentsO", value: AttachmentsO.Deserialize<Attachments>()); AttachmentsO = null;
                if (AttachmentsP != null) Attachments(columnName: "AttachmentsP", value: AttachmentsP.Deserialize<Attachments>()); AttachmentsP = null;
                if (AttachmentsQ != null) Attachments(columnName: "AttachmentsQ", value: AttachmentsQ.Deserialize<Attachments>()); AttachmentsQ = null;
                if (AttachmentsR != null) Attachments(columnName: "AttachmentsR", value: AttachmentsR.Deserialize<Attachments>()); AttachmentsR = null;
                if (AttachmentsS != null) Attachments(columnName: "AttachmentsS", value: AttachmentsS.Deserialize<Attachments>()); AttachmentsS = null;
                if (AttachmentsT != null) Attachments(columnName: "AttachmentsT", value: AttachmentsT.Deserialize<Attachments>()); AttachmentsT = null;
                if (AttachmentsU != null) Attachments(columnName: "AttachmentsU", value: AttachmentsU.Deserialize<Attachments>()); AttachmentsU = null;
                if (AttachmentsV != null) Attachments(columnName: "AttachmentsV", value: AttachmentsV.Deserialize<Attachments>()); AttachmentsV = null;
                if (AttachmentsW != null) Attachments(columnName: "AttachmentsW", value: AttachmentsW.Deserialize<Attachments>()); AttachmentsW = null;
                if (AttachmentsX != null) Attachments(columnName: "AttachmentsX", value: AttachmentsX.Deserialize<Attachments>()); AttachmentsX = null;
                if (AttachmentsY != null) Attachments(columnName: "AttachmentsY", value: AttachmentsY.Deserialize<Attachments>()); AttachmentsY = null;
                if (AttachmentsZ != null) Attachments(columnName: "AttachmentsZ", value: AttachmentsZ.Deserialize<Attachments>()); AttachmentsZ = null;
                if (Attachments001 != null) Attachments(columnName: "Attachments001", value: Attachments001.Deserialize<Attachments>()); Attachments001 = null;
                if (Attachments002 != null) Attachments(columnName: "Attachments002", value: Attachments002.Deserialize<Attachments>()); Attachments002 = null;
                if (Attachments003 != null) Attachments(columnName: "Attachments003", value: Attachments003.Deserialize<Attachments>()); Attachments003 = null;
                if (Attachments004 != null) Attachments(columnName: "Attachments004", value: Attachments004.Deserialize<Attachments>()); Attachments004 = null;
                if (Attachments005 != null) Attachments(columnName: "Attachments005", value: Attachments005.Deserialize<Attachments>()); Attachments005 = null;
                if (Attachments006 != null) Attachments(columnName: "Attachments006", value: Attachments006.Deserialize<Attachments>()); Attachments006 = null;
                if (Attachments007 != null) Attachments(columnName: "Attachments007", value: Attachments007.Deserialize<Attachments>()); Attachments007 = null;
                if (Attachments008 != null) Attachments(columnName: "Attachments008", value: Attachments008.Deserialize<Attachments>()); Attachments008 = null;
                if (Attachments009 != null) Attachments(columnName: "Attachments009", value: Attachments009.Deserialize<Attachments>()); Attachments009 = null;
                if (Attachments010 != null) Attachments(columnName: "Attachments010", value: Attachments010.Deserialize<Attachments>()); Attachments010 = null;
                if (Attachments011 != null) Attachments(columnName: "Attachments011", value: Attachments011.Deserialize<Attachments>()); Attachments011 = null;
                if (Attachments012 != null) Attachments(columnName: "Attachments012", value: Attachments012.Deserialize<Attachments>()); Attachments012 = null;
                if (Attachments013 != null) Attachments(columnName: "Attachments013", value: Attachments013.Deserialize<Attachments>()); Attachments013 = null;
                if (Attachments014 != null) Attachments(columnName: "Attachments014", value: Attachments014.Deserialize<Attachments>()); Attachments014 = null;
                if (Attachments015 != null) Attachments(columnName: "Attachments015", value: Attachments015.Deserialize<Attachments>()); Attachments015 = null;
                if (Attachments016 != null) Attachments(columnName: "Attachments016", value: Attachments016.Deserialize<Attachments>()); Attachments016 = null;
                if (Attachments017 != null) Attachments(columnName: "Attachments017", value: Attachments017.Deserialize<Attachments>()); Attachments017 = null;
                if (Attachments018 != null) Attachments(columnName: "Attachments018", value: Attachments018.Deserialize<Attachments>()); Attachments018 = null;
                if (Attachments019 != null) Attachments(columnName: "Attachments019", value: Attachments019.Deserialize<Attachments>()); Attachments019 = null;
                if (Attachments020 != null) Attachments(columnName: "Attachments020", value: Attachments020.Deserialize<Attachments>()); Attachments020 = null;
                if (Attachments021 != null) Attachments(columnName: "Attachments021", value: Attachments021.Deserialize<Attachments>()); Attachments021 = null;
                if (Attachments022 != null) Attachments(columnName: "Attachments022", value: Attachments022.Deserialize<Attachments>()); Attachments022 = null;
                if (Attachments023 != null) Attachments(columnName: "Attachments023", value: Attachments023.Deserialize<Attachments>()); Attachments023 = null;
                if (Attachments024 != null) Attachments(columnName: "Attachments024", value: Attachments024.Deserialize<Attachments>()); Attachments024 = null;
                if (Attachments025 != null) Attachments(columnName: "Attachments025", value: Attachments025.Deserialize<Attachments>()); Attachments025 = null;
                if (Attachments026 != null) Attachments(columnName: "Attachments026", value: Attachments026.Deserialize<Attachments>()); Attachments026 = null;
                if (Attachments027 != null) Attachments(columnName: "Attachments027", value: Attachments027.Deserialize<Attachments>()); Attachments027 = null;
                if (Attachments028 != null) Attachments(columnName: "Attachments028", value: Attachments028.Deserialize<Attachments>()); Attachments028 = null;
                if (Attachments029 != null) Attachments(columnName: "Attachments029", value: Attachments029.Deserialize<Attachments>()); Attachments029 = null;
                if (Attachments030 != null) Attachments(columnName: "Attachments030", value: Attachments030.Deserialize<Attachments>()); Attachments030 = null;
                if (Attachments031 != null) Attachments(columnName: "Attachments031", value: Attachments031.Deserialize<Attachments>()); Attachments031 = null;
                if (Attachments032 != null) Attachments(columnName: "Attachments032", value: Attachments032.Deserialize<Attachments>()); Attachments032 = null;
                if (Attachments033 != null) Attachments(columnName: "Attachments033", value: Attachments033.Deserialize<Attachments>()); Attachments033 = null;
                if (Attachments034 != null) Attachments(columnName: "Attachments034", value: Attachments034.Deserialize<Attachments>()); Attachments034 = null;
                if (Attachments035 != null) Attachments(columnName: "Attachments035", value: Attachments035.Deserialize<Attachments>()); Attachments035 = null;
                if (Attachments036 != null) Attachments(columnName: "Attachments036", value: Attachments036.Deserialize<Attachments>()); Attachments036 = null;
                if (Attachments037 != null) Attachments(columnName: "Attachments037", value: Attachments037.Deserialize<Attachments>()); Attachments037 = null;
                if (Attachments038 != null) Attachments(columnName: "Attachments038", value: Attachments038.Deserialize<Attachments>()); Attachments038 = null;
                if (Attachments039 != null) Attachments(columnName: "Attachments039", value: Attachments039.Deserialize<Attachments>()); Attachments039 = null;
                if (Attachments040 != null) Attachments(columnName: "Attachments040", value: Attachments040.Deserialize<Attachments>()); Attachments040 = null;
                if (Attachments041 != null) Attachments(columnName: "Attachments041", value: Attachments041.Deserialize<Attachments>()); Attachments041 = null;
                if (Attachments042 != null) Attachments(columnName: "Attachments042", value: Attachments042.Deserialize<Attachments>()); Attachments042 = null;
                if (Attachments043 != null) Attachments(columnName: "Attachments043", value: Attachments043.Deserialize<Attachments>()); Attachments043 = null;
                if (Attachments044 != null) Attachments(columnName: "Attachments044", value: Attachments044.Deserialize<Attachments>()); Attachments044 = null;
                if (Attachments045 != null) Attachments(columnName: "Attachments045", value: Attachments045.Deserialize<Attachments>()); Attachments045 = null;
                if (Attachments046 != null) Attachments(columnName: "Attachments046", value: Attachments046.Deserialize<Attachments>()); Attachments046 = null;
                if (Attachments047 != null) Attachments(columnName: "Attachments047", value: Attachments047.Deserialize<Attachments>()); Attachments047 = null;
                if (Attachments048 != null) Attachments(columnName: "Attachments048", value: Attachments048.Deserialize<Attachments>()); Attachments048 = null;
                if (Attachments049 != null) Attachments(columnName: "Attachments049", value: Attachments049.Deserialize<Attachments>()); Attachments049 = null;
                if (Attachments050 != null) Attachments(columnName: "Attachments050", value: Attachments050.Deserialize<Attachments>()); Attachments050 = null;
                if (Attachments051 != null) Attachments(columnName: "Attachments051", value: Attachments051.Deserialize<Attachments>()); Attachments051 = null;
                if (Attachments052 != null) Attachments(columnName: "Attachments052", value: Attachments052.Deserialize<Attachments>()); Attachments052 = null;
                if (Attachments053 != null) Attachments(columnName: "Attachments053", value: Attachments053.Deserialize<Attachments>()); Attachments053 = null;
                if (Attachments054 != null) Attachments(columnName: "Attachments054", value: Attachments054.Deserialize<Attachments>()); Attachments054 = null;
                if (Attachments055 != null) Attachments(columnName: "Attachments055", value: Attachments055.Deserialize<Attachments>()); Attachments055 = null;
                if (Attachments056 != null) Attachments(columnName: "Attachments056", value: Attachments056.Deserialize<Attachments>()); Attachments056 = null;
                if (Attachments057 != null) Attachments(columnName: "Attachments057", value: Attachments057.Deserialize<Attachments>()); Attachments057 = null;
                if (Attachments058 != null) Attachments(columnName: "Attachments058", value: Attachments058.Deserialize<Attachments>()); Attachments058 = null;
                if (Attachments059 != null) Attachments(columnName: "Attachments059", value: Attachments059.Deserialize<Attachments>()); Attachments059 = null;
                if (Attachments060 != null) Attachments(columnName: "Attachments060", value: Attachments060.Deserialize<Attachments>()); Attachments060 = null;
                if (Attachments061 != null) Attachments(columnName: "Attachments061", value: Attachments061.Deserialize<Attachments>()); Attachments061 = null;
                if (Attachments062 != null) Attachments(columnName: "Attachments062", value: Attachments062.Deserialize<Attachments>()); Attachments062 = null;
                if (Attachments063 != null) Attachments(columnName: "Attachments063", value: Attachments063.Deserialize<Attachments>()); Attachments063 = null;
                if (Attachments064 != null) Attachments(columnName: "Attachments064", value: Attachments064.Deserialize<Attachments>()); Attachments064 = null;
                if (Attachments065 != null) Attachments(columnName: "Attachments065", value: Attachments065.Deserialize<Attachments>()); Attachments065 = null;
                if (Attachments066 != null) Attachments(columnName: "Attachments066", value: Attachments066.Deserialize<Attachments>()); Attachments066 = null;
                if (Attachments067 != null) Attachments(columnName: "Attachments067", value: Attachments067.Deserialize<Attachments>()); Attachments067 = null;
                if (Attachments068 != null) Attachments(columnName: "Attachments068", value: Attachments068.Deserialize<Attachments>()); Attachments068 = null;
                if (Attachments069 != null) Attachments(columnName: "Attachments069", value: Attachments069.Deserialize<Attachments>()); Attachments069 = null;
                if (Attachments070 != null) Attachments(columnName: "Attachments070", value: Attachments070.Deserialize<Attachments>()); Attachments070 = null;
                if (Attachments071 != null) Attachments(columnName: "Attachments071", value: Attachments071.Deserialize<Attachments>()); Attachments071 = null;
                if (Attachments072 != null) Attachments(columnName: "Attachments072", value: Attachments072.Deserialize<Attachments>()); Attachments072 = null;
                if (Attachments073 != null) Attachments(columnName: "Attachments073", value: Attachments073.Deserialize<Attachments>()); Attachments073 = null;
                if (Attachments074 != null) Attachments(columnName: "Attachments074", value: Attachments074.Deserialize<Attachments>()); Attachments074 = null;
                if (Attachments075 != null) Attachments(columnName: "Attachments075", value: Attachments075.Deserialize<Attachments>()); Attachments075 = null;
                if (Attachments076 != null) Attachments(columnName: "Attachments076", value: Attachments076.Deserialize<Attachments>()); Attachments076 = null;
                if (Attachments077 != null) Attachments(columnName: "Attachments077", value: Attachments077.Deserialize<Attachments>()); Attachments077 = null;
                if (Attachments078 != null) Attachments(columnName: "Attachments078", value: Attachments078.Deserialize<Attachments>()); Attachments078 = null;
                if (Attachments079 != null) Attachments(columnName: "Attachments079", value: Attachments079.Deserialize<Attachments>()); Attachments079 = null;
                if (Attachments080 != null) Attachments(columnName: "Attachments080", value: Attachments080.Deserialize<Attachments>()); Attachments080 = null;
                if (Attachments081 != null) Attachments(columnName: "Attachments081", value: Attachments081.Deserialize<Attachments>()); Attachments081 = null;
                if (Attachments082 != null) Attachments(columnName: "Attachments082", value: Attachments082.Deserialize<Attachments>()); Attachments082 = null;
                if (Attachments083 != null) Attachments(columnName: "Attachments083", value: Attachments083.Deserialize<Attachments>()); Attachments083 = null;
                if (Attachments084 != null) Attachments(columnName: "Attachments084", value: Attachments084.Deserialize<Attachments>()); Attachments084 = null;
                if (Attachments085 != null) Attachments(columnName: "Attachments085", value: Attachments085.Deserialize<Attachments>()); Attachments085 = null;
                if (Attachments086 != null) Attachments(columnName: "Attachments086", value: Attachments086.Deserialize<Attachments>()); Attachments086 = null;
                if (Attachments087 != null) Attachments(columnName: "Attachments087", value: Attachments087.Deserialize<Attachments>()); Attachments087 = null;
                if (Attachments088 != null) Attachments(columnName: "Attachments088", value: Attachments088.Deserialize<Attachments>()); Attachments088 = null;
                if (Attachments089 != null) Attachments(columnName: "Attachments089", value: Attachments089.Deserialize<Attachments>()); Attachments089 = null;
                if (Attachments090 != null) Attachments(columnName: "Attachments090", value: Attachments090.Deserialize<Attachments>()); Attachments090 = null;
                if (Attachments091 != null) Attachments(columnName: "Attachments091", value: Attachments091.Deserialize<Attachments>()); Attachments091 = null;
                if (Attachments092 != null) Attachments(columnName: "Attachments092", value: Attachments092.Deserialize<Attachments>()); Attachments092 = null;
                if (Attachments093 != null) Attachments(columnName: "Attachments093", value: Attachments093.Deserialize<Attachments>()); Attachments093 = null;
                if (Attachments094 != null) Attachments(columnName: "Attachments094", value: Attachments094.Deserialize<Attachments>()); Attachments094 = null;
                if (Attachments095 != null) Attachments(columnName: "Attachments095", value: Attachments095.Deserialize<Attachments>()); Attachments095 = null;
                if (Attachments096 != null) Attachments(columnName: "Attachments096", value: Attachments096.Deserialize<Attachments>()); Attachments096 = null;
                if (Attachments097 != null) Attachments(columnName: "Attachments097", value: Attachments097.Deserialize<Attachments>()); Attachments097 = null;
                if (Attachments098 != null) Attachments(columnName: "Attachments098", value: Attachments098.Deserialize<Attachments>()); Attachments098 = null;
                if (Attachments099 != null) Attachments(columnName: "Attachments099", value: Attachments099.Deserialize<Attachments>()); Attachments099 = null;
                if (Attachments100 != null) Attachments(columnName: "Attachments100", value: Attachments100.Deserialize<Attachments>()); Attachments100 = null;
            }
        }

        public string Value(
            Context context,
            Column column,
            bool toLocal = false)
        {
            return Value(
                context: context,
                columnName: column.ColumnName,
                toLocal: toLocal);
        }

        public string Value(
            Context context,
            string columnName,
            bool toLocal = false)
        {
            switch (Def.ExtendedColumnTypes.Get(columnName))
            {
                case "Class":
                    return Class(columnName: columnName);
                case "Num":
                    return Num(columnName: columnName).ToString();
                case "Date":
                    return toLocal
                        ? Date(columnName: columnName)
                            .ToLocal(context: context)
                            .ToString()
                        : Date(columnName: columnName).ToString();
                case "Description":
                    return Description(columnName: columnName);
                case "Check":
                    return Check(columnName: columnName).ToString();
                case "Attachments":
                    return Attachments(columnName: columnName).ToJson();
                default:
                    return null;
            }
        }

        public void Value(
            Context context,
            Column column,
            string value,
            bool toUniversal = false)
        {
            Value(
                context: context,
                columnName: column.ColumnName,
                value: value,
                toUniversal: toUniversal);
        }

        public void Value(
            Context context,
            string columnName,
            string value,
            bool toUniversal = false)
        {
            switch (Def.ExtendedColumnTypes.Get(columnName))
            {
                case "Class":
                    Class(
                        columnName: columnName,
                        value: value);
                    break;
                case "Num":
                    Num(
                        columnName: columnName,
                        value: value.ToDecimal());
                    break;
                case "Date":
                    Date(
                        columnName: columnName,
                        value: toUniversal
                            ? value.ToDateTime().ToUniversal(context: context)
                            : value.ToDateTime());
                    break;
                case "Description":
                    Description(
                        columnName: columnName,
                        value: value);
                    break;
                case "Check":
                    Check(
                        columnName: columnName,
                        value: value.ToBool());
                    break;
                case "Attachments":
                    Attachments(
                        columnName: columnName,
                        value: value.Deserialize<Attachments>());
                    break;
            }
        }

        public string Class(Column column)
        {
            return ClassHash.Get(column.ColumnName);
        }

        public string Class(string columnName)
        {
            return ClassHash.Get(columnName);
        }

        public void Class(Column column, string value)
        {
            Class(
                columnName: column.ColumnName,
                value: value);
        }

        public void Class(string columnName, string value)
        {
            if (!ClassHash.ContainsKey(columnName))
            {
                ClassHash.Add(columnName, value);
            }
            else
            {
                ClassHash[columnName] = value;
            }
        }

        public decimal Num(Column column)
        {
            return Num(columnName: column.ColumnName);
        }

        public decimal Num(string columnName)
        {
            return NumHash.Get(columnName);
        }

        public void Num(Column column, decimal value)
        {
            Num(
                columnName: column.ColumnName,
                value: value);
        }

        public void Num(string columnName, decimal value)
        {
            if (!NumHash.ContainsKey(columnName))
            {
                NumHash.Add(columnName, value);
            }
            else
            {
                NumHash[columnName] = value;
            }
        }

        public DateTime Date(Column column)
        {
            return Date(columnName: column.ColumnName);
        }

        public DateTime Date(string columnName)
        {
            return DateHash.Get(columnName);
        }

        public void Date(Column column, DateTime value)
        {
            Date(
                columnName: column.ColumnName,
                value: value);
        }

        public void Date(string columnName, DateTime value)
        {
            if (!DateHash.ContainsKey(columnName))
            {
                DateHash.Add(columnName, value);
            }
            else
            {
                DateHash[columnName] = value;
            }
        }

        public string Description(Column column)
        {
            return Description(columnName: column.ColumnName);
        }

        public string Description(string columnName)
        {
            return DescriptionHash.Get(columnName);
        }

        public void Description(Column column, string value)
        {
            Description(
                columnName: column.ColumnName,
                value: value);
        }

        public void Description(string columnName, string value)
        {
            if (!DescriptionHash.ContainsKey(columnName))
            {
                DescriptionHash.Add(columnName, value);
            }
            else
            {
                DescriptionHash[columnName] = value;
            }
        }

        public bool Check(Column column)
        {
            return Check(columnName: column.ColumnName);
        }

        public bool Check(string columnName)
        {
            return CheckHash.Get(columnName);
        }

        public void Check(Column column, bool value)
        {
            Check(
                columnName: column.ColumnName,
                value: value);
        }

        public void Check(string columnName, bool value)
        {
            if (!CheckHash.ContainsKey(columnName))
            {
                CheckHash.Add(columnName, value);
            }
            else
            {
                CheckHash[columnName] = value;
            }
        }

        public bool Check_Updated(string columnName)
        {
            return CheckHash.Get(columnName) != CheckHash.Get(columnName);
        }

        public Attachments Attachments(Column column)
        {
            return Attachments(columnName: column.ColumnName);
        }

        public Attachments Attachments(string columnName)
        {
            return AttachmentsHash.Get(columnName);
        }

        public void Attachments(Column column, Attachments value)
        {
            Attachments(
                columnName: column.ColumnName,
                value: value);
        }

        public void Attachments(string columnName, Attachments value)
        {
            if (!AttachmentsHash.ContainsKey(columnName))
            {
                AttachmentsHash.Add(columnName, value);
            }
            else
            {
                AttachmentsHash[columnName] = value;
            }
        }
    }
}