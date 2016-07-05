using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public class ResultModel : BaseItemModel
    {
        public long Id { get { return ResultId; } }
        public override long UrlId { get { return ResultId; } }
        public long ResultId = 0;
        public Status Status = new Status();
        public User Manager = new User();
        public User Owner = new User();
        public string ClassA = string.Empty;
        public string ClassB = string.Empty;
        public string ClassC = string.Empty;
        public string ClassD = string.Empty;
        public string ClassE = string.Empty;
        public string ClassF = string.Empty;
        public string ClassG = string.Empty;
        public string ClassH = string.Empty;
        public string ClassI = string.Empty;
        public string ClassJ = string.Empty;
        public string ClassK = string.Empty;
        public string ClassL = string.Empty;
        public string ClassM = string.Empty;
        public string ClassN = string.Empty;
        public string ClassO = string.Empty;
        public string ClassP = string.Empty;
        public string ClassQ = string.Empty;
        public string ClassR = string.Empty;
        public string ClassS = string.Empty;
        public string ClassT = string.Empty;
        public string ClassU = string.Empty;
        public string ClassV = string.Empty;
        public string ClassW = string.Empty;
        public string ClassX = string.Empty;
        public string ClassY = string.Empty;
        public string ClassZ = string.Empty;
        public decimal NumA = 0;
        public decimal NumB = 0;
        public decimal NumC = 0;
        public decimal NumD = 0;
        public decimal NumE = 0;
        public decimal NumF = 0;
        public decimal NumG = 0;
        public decimal NumH = 0;
        public decimal NumI = 0;
        public decimal NumJ = 0;
        public decimal NumK = 0;
        public decimal NumL = 0;
        public decimal NumM = 0;
        public decimal NumN = 0;
        public decimal NumO = 0;
        public decimal NumP = 0;
        public decimal NumQ = 0;
        public decimal NumR = 0;
        public decimal NumS = 0;
        public decimal NumT = 0;
        public decimal NumU = 0;
        public decimal NumV = 0;
        public decimal NumW = 0;
        public decimal NumX = 0;
        public decimal NumY = 0;
        public decimal NumZ = 0;
        public DateTime DateA = 0.ToDateTime();
        public DateTime DateB = 0.ToDateTime();
        public DateTime DateC = 0.ToDateTime();
        public DateTime DateD = 0.ToDateTime();
        public DateTime DateE = 0.ToDateTime();
        public DateTime DateF = 0.ToDateTime();
        public DateTime DateG = 0.ToDateTime();
        public DateTime DateH = 0.ToDateTime();
        public DateTime DateI = 0.ToDateTime();
        public DateTime DateJ = 0.ToDateTime();
        public DateTime DateK = 0.ToDateTime();
        public DateTime DateL = 0.ToDateTime();
        public DateTime DateM = 0.ToDateTime();
        public DateTime DateN = 0.ToDateTime();
        public DateTime DateO = 0.ToDateTime();
        public DateTime DateP = 0.ToDateTime();
        public DateTime DateQ = 0.ToDateTime();
        public DateTime DateR = 0.ToDateTime();
        public DateTime DateS = 0.ToDateTime();
        public DateTime DateT = 0.ToDateTime();
        public DateTime DateU = 0.ToDateTime();
        public DateTime DateV = 0.ToDateTime();
        public DateTime DateW = 0.ToDateTime();
        public DateTime DateX = 0.ToDateTime();
        public DateTime DateY = 0.ToDateTime();
        public DateTime DateZ = 0.ToDateTime();
        public string DescriptionA = string.Empty;
        public string DescriptionB = string.Empty;
        public string DescriptionC = string.Empty;
        public string DescriptionD = string.Empty;
        public string DescriptionE = string.Empty;
        public string DescriptionF = string.Empty;
        public string DescriptionG = string.Empty;
        public string DescriptionH = string.Empty;
        public string DescriptionI = string.Empty;
        public string DescriptionJ = string.Empty;
        public string DescriptionK = string.Empty;
        public string DescriptionL = string.Empty;
        public string DescriptionM = string.Empty;
        public string DescriptionN = string.Empty;
        public string DescriptionO = string.Empty;
        public string DescriptionP = string.Empty;
        public string DescriptionQ = string.Empty;
        public string DescriptionR = string.Empty;
        public string DescriptionS = string.Empty;
        public string DescriptionT = string.Empty;
        public string DescriptionU = string.Empty;
        public string DescriptionV = string.Empty;
        public string DescriptionW = string.Empty;
        public string DescriptionX = string.Empty;
        public string DescriptionY = string.Empty;
        public string DescriptionZ = string.Empty;
        public bool CheckA = false;
        public bool CheckB = false;
        public bool CheckC = false;
        public bool CheckD = false;
        public bool CheckE = false;
        public bool CheckF = false;
        public bool CheckG = false;
        public bool CheckH = false;
        public bool CheckI = false;
        public bool CheckJ = false;
        public bool CheckK = false;
        public bool CheckL = false;
        public bool CheckM = false;
        public bool CheckN = false;
        public bool CheckO = false;
        public bool CheckP = false;
        public bool CheckQ = false;
        public bool CheckR = false;
        public bool CheckS = false;
        public bool CheckT = false;
        public bool CheckU = false;
        public bool CheckV = false;
        public bool CheckW = false;
        public bool CheckX = false;
        public bool CheckY = false;
        public bool CheckZ = false;
        public TitleBody TitleBody { get { return new TitleBody(ResultId, Title.Value, Title.DisplayValue, Body); } }
        public long SavedResultId = 0;
        public int SavedStatus = 100;
        public int SavedManager = 0;
        public int SavedOwner = 0;
        public string SavedClassA = string.Empty;
        public string SavedClassB = string.Empty;
        public string SavedClassC = string.Empty;
        public string SavedClassD = string.Empty;
        public string SavedClassE = string.Empty;
        public string SavedClassF = string.Empty;
        public string SavedClassG = string.Empty;
        public string SavedClassH = string.Empty;
        public string SavedClassI = string.Empty;
        public string SavedClassJ = string.Empty;
        public string SavedClassK = string.Empty;
        public string SavedClassL = string.Empty;
        public string SavedClassM = string.Empty;
        public string SavedClassN = string.Empty;
        public string SavedClassO = string.Empty;
        public string SavedClassP = string.Empty;
        public string SavedClassQ = string.Empty;
        public string SavedClassR = string.Empty;
        public string SavedClassS = string.Empty;
        public string SavedClassT = string.Empty;
        public string SavedClassU = string.Empty;
        public string SavedClassV = string.Empty;
        public string SavedClassW = string.Empty;
        public string SavedClassX = string.Empty;
        public string SavedClassY = string.Empty;
        public string SavedClassZ = string.Empty;
        public decimal SavedNumA = 0;
        public decimal SavedNumB = 0;
        public decimal SavedNumC = 0;
        public decimal SavedNumD = 0;
        public decimal SavedNumE = 0;
        public decimal SavedNumF = 0;
        public decimal SavedNumG = 0;
        public decimal SavedNumH = 0;
        public decimal SavedNumI = 0;
        public decimal SavedNumJ = 0;
        public decimal SavedNumK = 0;
        public decimal SavedNumL = 0;
        public decimal SavedNumM = 0;
        public decimal SavedNumN = 0;
        public decimal SavedNumO = 0;
        public decimal SavedNumP = 0;
        public decimal SavedNumQ = 0;
        public decimal SavedNumR = 0;
        public decimal SavedNumS = 0;
        public decimal SavedNumT = 0;
        public decimal SavedNumU = 0;
        public decimal SavedNumV = 0;
        public decimal SavedNumW = 0;
        public decimal SavedNumX = 0;
        public decimal SavedNumY = 0;
        public decimal SavedNumZ = 0;
        public DateTime SavedDateA = 0.ToDateTime();
        public DateTime SavedDateB = 0.ToDateTime();
        public DateTime SavedDateC = 0.ToDateTime();
        public DateTime SavedDateD = 0.ToDateTime();
        public DateTime SavedDateE = 0.ToDateTime();
        public DateTime SavedDateF = 0.ToDateTime();
        public DateTime SavedDateG = 0.ToDateTime();
        public DateTime SavedDateH = 0.ToDateTime();
        public DateTime SavedDateI = 0.ToDateTime();
        public DateTime SavedDateJ = 0.ToDateTime();
        public DateTime SavedDateK = 0.ToDateTime();
        public DateTime SavedDateL = 0.ToDateTime();
        public DateTime SavedDateM = 0.ToDateTime();
        public DateTime SavedDateN = 0.ToDateTime();
        public DateTime SavedDateO = 0.ToDateTime();
        public DateTime SavedDateP = 0.ToDateTime();
        public DateTime SavedDateQ = 0.ToDateTime();
        public DateTime SavedDateR = 0.ToDateTime();
        public DateTime SavedDateS = 0.ToDateTime();
        public DateTime SavedDateT = 0.ToDateTime();
        public DateTime SavedDateU = 0.ToDateTime();
        public DateTime SavedDateV = 0.ToDateTime();
        public DateTime SavedDateW = 0.ToDateTime();
        public DateTime SavedDateX = 0.ToDateTime();
        public DateTime SavedDateY = 0.ToDateTime();
        public DateTime SavedDateZ = 0.ToDateTime();
        public string SavedDescriptionA = string.Empty;
        public string SavedDescriptionB = string.Empty;
        public string SavedDescriptionC = string.Empty;
        public string SavedDescriptionD = string.Empty;
        public string SavedDescriptionE = string.Empty;
        public string SavedDescriptionF = string.Empty;
        public string SavedDescriptionG = string.Empty;
        public string SavedDescriptionH = string.Empty;
        public string SavedDescriptionI = string.Empty;
        public string SavedDescriptionJ = string.Empty;
        public string SavedDescriptionK = string.Empty;
        public string SavedDescriptionL = string.Empty;
        public string SavedDescriptionM = string.Empty;
        public string SavedDescriptionN = string.Empty;
        public string SavedDescriptionO = string.Empty;
        public string SavedDescriptionP = string.Empty;
        public string SavedDescriptionQ = string.Empty;
        public string SavedDescriptionR = string.Empty;
        public string SavedDescriptionS = string.Empty;
        public string SavedDescriptionT = string.Empty;
        public string SavedDescriptionU = string.Empty;
        public string SavedDescriptionV = string.Empty;
        public string SavedDescriptionW = string.Empty;
        public string SavedDescriptionX = string.Empty;
        public string SavedDescriptionY = string.Empty;
        public string SavedDescriptionZ = string.Empty;
        public bool SavedCheckA = false;
        public bool SavedCheckB = false;
        public bool SavedCheckC = false;
        public bool SavedCheckD = false;
        public bool SavedCheckE = false;
        public bool SavedCheckF = false;
        public bool SavedCheckG = false;
        public bool SavedCheckH = false;
        public bool SavedCheckI = false;
        public bool SavedCheckJ = false;
        public bool SavedCheckK = false;
        public bool SavedCheckL = false;
        public bool SavedCheckM = false;
        public bool SavedCheckN = false;
        public bool SavedCheckO = false;
        public bool SavedCheckP = false;
        public bool SavedCheckQ = false;
        public bool SavedCheckR = false;
        public bool SavedCheckS = false;
        public bool SavedCheckT = false;
        public bool SavedCheckU = false;
        public bool SavedCheckV = false;
        public bool SavedCheckW = false;
        public bool SavedCheckX = false;
        public bool SavedCheckY = false;
        public bool SavedCheckZ = false;
        public bool Status_Updated { get { return Status.Value != SavedStatus; } }
        public bool Manager_Updated { get { return Manager.Id != SavedManager; } }
        public bool Owner_Updated { get { return Owner.Id != SavedOwner; } }
        public bool ClassA_Updated { get { return ClassA != SavedClassA && ClassA != null; } }
        public bool ClassB_Updated { get { return ClassB != SavedClassB && ClassB != null; } }
        public bool ClassC_Updated { get { return ClassC != SavedClassC && ClassC != null; } }
        public bool ClassD_Updated { get { return ClassD != SavedClassD && ClassD != null; } }
        public bool ClassE_Updated { get { return ClassE != SavedClassE && ClassE != null; } }
        public bool ClassF_Updated { get { return ClassF != SavedClassF && ClassF != null; } }
        public bool ClassG_Updated { get { return ClassG != SavedClassG && ClassG != null; } }
        public bool ClassH_Updated { get { return ClassH != SavedClassH && ClassH != null; } }
        public bool ClassI_Updated { get { return ClassI != SavedClassI && ClassI != null; } }
        public bool ClassJ_Updated { get { return ClassJ != SavedClassJ && ClassJ != null; } }
        public bool ClassK_Updated { get { return ClassK != SavedClassK && ClassK != null; } }
        public bool ClassL_Updated { get { return ClassL != SavedClassL && ClassL != null; } }
        public bool ClassM_Updated { get { return ClassM != SavedClassM && ClassM != null; } }
        public bool ClassN_Updated { get { return ClassN != SavedClassN && ClassN != null; } }
        public bool ClassO_Updated { get { return ClassO != SavedClassO && ClassO != null; } }
        public bool ClassP_Updated { get { return ClassP != SavedClassP && ClassP != null; } }
        public bool ClassQ_Updated { get { return ClassQ != SavedClassQ && ClassQ != null; } }
        public bool ClassR_Updated { get { return ClassR != SavedClassR && ClassR != null; } }
        public bool ClassS_Updated { get { return ClassS != SavedClassS && ClassS != null; } }
        public bool ClassT_Updated { get { return ClassT != SavedClassT && ClassT != null; } }
        public bool ClassU_Updated { get { return ClassU != SavedClassU && ClassU != null; } }
        public bool ClassV_Updated { get { return ClassV != SavedClassV && ClassV != null; } }
        public bool ClassW_Updated { get { return ClassW != SavedClassW && ClassW != null; } }
        public bool ClassX_Updated { get { return ClassX != SavedClassX && ClassX != null; } }
        public bool ClassY_Updated { get { return ClassY != SavedClassY && ClassY != null; } }
        public bool ClassZ_Updated { get { return ClassZ != SavedClassZ && ClassZ != null; } }
        public bool NumA_Updated { get { return NumA != SavedNumA; } }
        public bool NumB_Updated { get { return NumB != SavedNumB; } }
        public bool NumC_Updated { get { return NumC != SavedNumC; } }
        public bool NumD_Updated { get { return NumD != SavedNumD; } }
        public bool NumE_Updated { get { return NumE != SavedNumE; } }
        public bool NumF_Updated { get { return NumF != SavedNumF; } }
        public bool NumG_Updated { get { return NumG != SavedNumG; } }
        public bool NumH_Updated { get { return NumH != SavedNumH; } }
        public bool NumI_Updated { get { return NumI != SavedNumI; } }
        public bool NumJ_Updated { get { return NumJ != SavedNumJ; } }
        public bool NumK_Updated { get { return NumK != SavedNumK; } }
        public bool NumL_Updated { get { return NumL != SavedNumL; } }
        public bool NumM_Updated { get { return NumM != SavedNumM; } }
        public bool NumN_Updated { get { return NumN != SavedNumN; } }
        public bool NumO_Updated { get { return NumO != SavedNumO; } }
        public bool NumP_Updated { get { return NumP != SavedNumP; } }
        public bool NumQ_Updated { get { return NumQ != SavedNumQ; } }
        public bool NumR_Updated { get { return NumR != SavedNumR; } }
        public bool NumS_Updated { get { return NumS != SavedNumS; } }
        public bool NumT_Updated { get { return NumT != SavedNumT; } }
        public bool NumU_Updated { get { return NumU != SavedNumU; } }
        public bool NumV_Updated { get { return NumV != SavedNumV; } }
        public bool NumW_Updated { get { return NumW != SavedNumW; } }
        public bool NumX_Updated { get { return NumX != SavedNumX; } }
        public bool NumY_Updated { get { return NumY != SavedNumY; } }
        public bool NumZ_Updated { get { return NumZ != SavedNumZ; } }
        public bool DateA_Updated { get { return DateA != SavedDateA && DateA != null; } }
        public bool DateB_Updated { get { return DateB != SavedDateB && DateB != null; } }
        public bool DateC_Updated { get { return DateC != SavedDateC && DateC != null; } }
        public bool DateD_Updated { get { return DateD != SavedDateD && DateD != null; } }
        public bool DateE_Updated { get { return DateE != SavedDateE && DateE != null; } }
        public bool DateF_Updated { get { return DateF != SavedDateF && DateF != null; } }
        public bool DateG_Updated { get { return DateG != SavedDateG && DateG != null; } }
        public bool DateH_Updated { get { return DateH != SavedDateH && DateH != null; } }
        public bool DateI_Updated { get { return DateI != SavedDateI && DateI != null; } }
        public bool DateJ_Updated { get { return DateJ != SavedDateJ && DateJ != null; } }
        public bool DateK_Updated { get { return DateK != SavedDateK && DateK != null; } }
        public bool DateL_Updated { get { return DateL != SavedDateL && DateL != null; } }
        public bool DateM_Updated { get { return DateM != SavedDateM && DateM != null; } }
        public bool DateN_Updated { get { return DateN != SavedDateN && DateN != null; } }
        public bool DateO_Updated { get { return DateO != SavedDateO && DateO != null; } }
        public bool DateP_Updated { get { return DateP != SavedDateP && DateP != null; } }
        public bool DateQ_Updated { get { return DateQ != SavedDateQ && DateQ != null; } }
        public bool DateR_Updated { get { return DateR != SavedDateR && DateR != null; } }
        public bool DateS_Updated { get { return DateS != SavedDateS && DateS != null; } }
        public bool DateT_Updated { get { return DateT != SavedDateT && DateT != null; } }
        public bool DateU_Updated { get { return DateU != SavedDateU && DateU != null; } }
        public bool DateV_Updated { get { return DateV != SavedDateV && DateV != null; } }
        public bool DateW_Updated { get { return DateW != SavedDateW && DateW != null; } }
        public bool DateX_Updated { get { return DateX != SavedDateX && DateX != null; } }
        public bool DateY_Updated { get { return DateY != SavedDateY && DateY != null; } }
        public bool DateZ_Updated { get { return DateZ != SavedDateZ && DateZ != null; } }
        public bool DescriptionA_Updated { get { return DescriptionA != SavedDescriptionA && DescriptionA != null; } }
        public bool DescriptionB_Updated { get { return DescriptionB != SavedDescriptionB && DescriptionB != null; } }
        public bool DescriptionC_Updated { get { return DescriptionC != SavedDescriptionC && DescriptionC != null; } }
        public bool DescriptionD_Updated { get { return DescriptionD != SavedDescriptionD && DescriptionD != null; } }
        public bool DescriptionE_Updated { get { return DescriptionE != SavedDescriptionE && DescriptionE != null; } }
        public bool DescriptionF_Updated { get { return DescriptionF != SavedDescriptionF && DescriptionF != null; } }
        public bool DescriptionG_Updated { get { return DescriptionG != SavedDescriptionG && DescriptionG != null; } }
        public bool DescriptionH_Updated { get { return DescriptionH != SavedDescriptionH && DescriptionH != null; } }
        public bool DescriptionI_Updated { get { return DescriptionI != SavedDescriptionI && DescriptionI != null; } }
        public bool DescriptionJ_Updated { get { return DescriptionJ != SavedDescriptionJ && DescriptionJ != null; } }
        public bool DescriptionK_Updated { get { return DescriptionK != SavedDescriptionK && DescriptionK != null; } }
        public bool DescriptionL_Updated { get { return DescriptionL != SavedDescriptionL && DescriptionL != null; } }
        public bool DescriptionM_Updated { get { return DescriptionM != SavedDescriptionM && DescriptionM != null; } }
        public bool DescriptionN_Updated { get { return DescriptionN != SavedDescriptionN && DescriptionN != null; } }
        public bool DescriptionO_Updated { get { return DescriptionO != SavedDescriptionO && DescriptionO != null; } }
        public bool DescriptionP_Updated { get { return DescriptionP != SavedDescriptionP && DescriptionP != null; } }
        public bool DescriptionQ_Updated { get { return DescriptionQ != SavedDescriptionQ && DescriptionQ != null; } }
        public bool DescriptionR_Updated { get { return DescriptionR != SavedDescriptionR && DescriptionR != null; } }
        public bool DescriptionS_Updated { get { return DescriptionS != SavedDescriptionS && DescriptionS != null; } }
        public bool DescriptionT_Updated { get { return DescriptionT != SavedDescriptionT && DescriptionT != null; } }
        public bool DescriptionU_Updated { get { return DescriptionU != SavedDescriptionU && DescriptionU != null; } }
        public bool DescriptionV_Updated { get { return DescriptionV != SavedDescriptionV && DescriptionV != null; } }
        public bool DescriptionW_Updated { get { return DescriptionW != SavedDescriptionW && DescriptionW != null; } }
        public bool DescriptionX_Updated { get { return DescriptionX != SavedDescriptionX && DescriptionX != null; } }
        public bool DescriptionY_Updated { get { return DescriptionY != SavedDescriptionY && DescriptionY != null; } }
        public bool DescriptionZ_Updated { get { return DescriptionZ != SavedDescriptionZ && DescriptionZ != null; } }
        public bool CheckA_Updated { get { return CheckA != SavedCheckA; } }
        public bool CheckB_Updated { get { return CheckB != SavedCheckB; } }
        public bool CheckC_Updated { get { return CheckC != SavedCheckC; } }
        public bool CheckD_Updated { get { return CheckD != SavedCheckD; } }
        public bool CheckE_Updated { get { return CheckE != SavedCheckE; } }
        public bool CheckF_Updated { get { return CheckF != SavedCheckF; } }
        public bool CheckG_Updated { get { return CheckG != SavedCheckG; } }
        public bool CheckH_Updated { get { return CheckH != SavedCheckH; } }
        public bool CheckI_Updated { get { return CheckI != SavedCheckI; } }
        public bool CheckJ_Updated { get { return CheckJ != SavedCheckJ; } }
        public bool CheckK_Updated { get { return CheckK != SavedCheckK; } }
        public bool CheckL_Updated { get { return CheckL != SavedCheckL; } }
        public bool CheckM_Updated { get { return CheckM != SavedCheckM; } }
        public bool CheckN_Updated { get { return CheckN != SavedCheckN; } }
        public bool CheckO_Updated { get { return CheckO != SavedCheckO; } }
        public bool CheckP_Updated { get { return CheckP != SavedCheckP; } }
        public bool CheckQ_Updated { get { return CheckQ != SavedCheckQ; } }
        public bool CheckR_Updated { get { return CheckR != SavedCheckR; } }
        public bool CheckS_Updated { get { return CheckS != SavedCheckS; } }
        public bool CheckT_Updated { get { return CheckT != SavedCheckT; } }
        public bool CheckU_Updated { get { return CheckU != SavedCheckU; } }
        public bool CheckV_Updated { get { return CheckV != SavedCheckV; } }
        public bool CheckW_Updated { get { return CheckW != SavedCheckW; } }
        public bool CheckX_Updated { get { return CheckX != SavedCheckX; } }
        public bool CheckY_Updated { get { return CheckY != SavedCheckY; } }
        public bool CheckZ_Updated { get { return CheckZ != SavedCheckZ; } }

        public string PropertyValue(string name)
        {
            switch (name)
            {
                case "SiteId": return SiteId.ToString();
                case "UpdatedTime": return UpdatedTime.Value.ToString();
                case "ResultId": return ResultId.ToString();
                case "Ver": return Ver.ToString();
                case "Title": return Title.Value;
                case "Body": return Body;
                case "TitleBody": return TitleBody.ToString();
                case "Status": return Status.Value.ToString();
                case "Manager": return Manager.Id.ToString();
                case "Owner": return Owner.Id.ToString();
                case "ClassA": return ClassA;
                case "ClassB": return ClassB;
                case "ClassC": return ClassC;
                case "ClassD": return ClassD;
                case "ClassE": return ClassE;
                case "ClassF": return ClassF;
                case "ClassG": return ClassG;
                case "ClassH": return ClassH;
                case "ClassI": return ClassI;
                case "ClassJ": return ClassJ;
                case "ClassK": return ClassK;
                case "ClassL": return ClassL;
                case "ClassM": return ClassM;
                case "ClassN": return ClassN;
                case "ClassO": return ClassO;
                case "ClassP": return ClassP;
                case "ClassQ": return ClassQ;
                case "ClassR": return ClassR;
                case "ClassS": return ClassS;
                case "ClassT": return ClassT;
                case "ClassU": return ClassU;
                case "ClassV": return ClassV;
                case "ClassW": return ClassW;
                case "ClassX": return ClassX;
                case "ClassY": return ClassY;
                case "ClassZ": return ClassZ;
                case "NumA": return NumA.ToString();
                case "NumB": return NumB.ToString();
                case "NumC": return NumC.ToString();
                case "NumD": return NumD.ToString();
                case "NumE": return NumE.ToString();
                case "NumF": return NumF.ToString();
                case "NumG": return NumG.ToString();
                case "NumH": return NumH.ToString();
                case "NumI": return NumI.ToString();
                case "NumJ": return NumJ.ToString();
                case "NumK": return NumK.ToString();
                case "NumL": return NumL.ToString();
                case "NumM": return NumM.ToString();
                case "NumN": return NumN.ToString();
                case "NumO": return NumO.ToString();
                case "NumP": return NumP.ToString();
                case "NumQ": return NumQ.ToString();
                case "NumR": return NumR.ToString();
                case "NumS": return NumS.ToString();
                case "NumT": return NumT.ToString();
                case "NumU": return NumU.ToString();
                case "NumV": return NumV.ToString();
                case "NumW": return NumW.ToString();
                case "NumX": return NumX.ToString();
                case "NumY": return NumY.ToString();
                case "NumZ": return NumZ.ToString();
                case "DateA": return DateA.ToString();
                case "DateB": return DateB.ToString();
                case "DateC": return DateC.ToString();
                case "DateD": return DateD.ToString();
                case "DateE": return DateE.ToString();
                case "DateF": return DateF.ToString();
                case "DateG": return DateG.ToString();
                case "DateH": return DateH.ToString();
                case "DateI": return DateI.ToString();
                case "DateJ": return DateJ.ToString();
                case "DateK": return DateK.ToString();
                case "DateL": return DateL.ToString();
                case "DateM": return DateM.ToString();
                case "DateN": return DateN.ToString();
                case "DateO": return DateO.ToString();
                case "DateP": return DateP.ToString();
                case "DateQ": return DateQ.ToString();
                case "DateR": return DateR.ToString();
                case "DateS": return DateS.ToString();
                case "DateT": return DateT.ToString();
                case "DateU": return DateU.ToString();
                case "DateV": return DateV.ToString();
                case "DateW": return DateW.ToString();
                case "DateX": return DateX.ToString();
                case "DateY": return DateY.ToString();
                case "DateZ": return DateZ.ToString();
                case "DescriptionA": return DescriptionA;
                case "DescriptionB": return DescriptionB;
                case "DescriptionC": return DescriptionC;
                case "DescriptionD": return DescriptionD;
                case "DescriptionE": return DescriptionE;
                case "DescriptionF": return DescriptionF;
                case "DescriptionG": return DescriptionG;
                case "DescriptionH": return DescriptionH;
                case "DescriptionI": return DescriptionI;
                case "DescriptionJ": return DescriptionJ;
                case "DescriptionK": return DescriptionK;
                case "DescriptionL": return DescriptionL;
                case "DescriptionM": return DescriptionM;
                case "DescriptionN": return DescriptionN;
                case "DescriptionO": return DescriptionO;
                case "DescriptionP": return DescriptionP;
                case "DescriptionQ": return DescriptionQ;
                case "DescriptionR": return DescriptionR;
                case "DescriptionS": return DescriptionS;
                case "DescriptionT": return DescriptionT;
                case "DescriptionU": return DescriptionU;
                case "DescriptionV": return DescriptionV;
                case "DescriptionW": return DescriptionW;
                case "DescriptionX": return DescriptionX;
                case "DescriptionY": return DescriptionY;
                case "DescriptionZ": return DescriptionZ;
                case "CheckA": return CheckA.ToString();
                case "CheckB": return CheckB.ToString();
                case "CheckC": return CheckC.ToString();
                case "CheckD": return CheckD.ToString();
                case "CheckE": return CheckE.ToString();
                case "CheckF": return CheckF.ToString();
                case "CheckG": return CheckG.ToString();
                case "CheckH": return CheckH.ToString();
                case "CheckI": return CheckI.ToString();
                case "CheckJ": return CheckJ.ToString();
                case "CheckK": return CheckK.ToString();
                case "CheckL": return CheckL.ToString();
                case "CheckM": return CheckM.ToString();
                case "CheckN": return CheckN.ToString();
                case "CheckO": return CheckO.ToString();
                case "CheckP": return CheckP.ToString();
                case "CheckQ": return CheckQ.ToString();
                case "CheckR": return CheckR.ToString();
                case "CheckS": return CheckS.ToString();
                case "CheckT": return CheckT.ToString();
                case "CheckU": return CheckU.ToString();
                case "CheckV": return CheckV.ToString();
                case "CheckW": return CheckW.ToString();
                case "CheckX": return CheckX.ToString();
                case "CheckY": return CheckY.ToString();
                case "CheckZ": return CheckZ.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return null;
            }
        }

        public List<long> SwitchTargets;

        public ResultModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public ResultModel(long resultId, long siteId, bool setByForm = false)
        {
            ResultId = resultId;
            SiteId = siteId;
            if (setByForm) SetByForm();
            Get();
        }

        public ResultModel()
        {
        }

        public ResultModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            Manager = SiteInfo.User(Sessions.UserId());
            Owner = SiteInfo.User(Sessions.UserId());
            SiteSettings = siteSettings;
            PermissionType = permissionType;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public ResultModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            long resultId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            ResultId = resultId;
            PermissionType = permissionType;
            SiteId = SiteSettings.SiteId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public ResultModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            Set(dataRow);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        private void OnConstructed()
        {
        }

        public void ClearSessions()
        {
        }

        public ResultModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectResults(
                tableType: tableType,
                column: column ?? Rds.ResultsColumnDefault(),
                join: join ??  Rds.ResultsJoinDefault(),
                where: where ?? Rds.ResultsWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public string Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var error = ValidateBeforeCreate();
            if (error != null) return error;
            OnCreating();
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Results")
                            .SiteId(SiteId)
                            .Title(Title.Value)),
                    Rds.InsertResults(
                        tableType: tableType,
                        param: param ?? Rds.ResultsParamDefault(
                            this, setDefault: true, paramAll: paramAll)),
                    InsertLinks(SiteSettings, selectIdentity: true),
                });
            ResultId = newId != 0 ? newId : ResultId;
            SynchronizeSummary();
            Get();
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(ResultsUtility.TitleDisplayValue(SiteSettings, this))
                        .Subset(Jsons.ToJson(new ResultSubset(this, SiteSettings))),
                    where: Rds.ItemsWhere().ReferenceId(ResultId)));
            OnCreated();
            return RecordResponse(this, Messages.Created(Title.ToString()));
        }

        private void OnCreating()
        {
        }

        private void OnCreated()
        {
        }

        private string ValidateBeforeCreate()
        {
            if (!PermissionType.CanCreate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_SiteId": if (!SiteSettings.AllColumn("SiteId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ResultId": if (!SiteSettings.AllColumn("ResultId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Body": if (!SiteSettings.AllColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_TitleBody": if (!SiteSettings.AllColumn("TitleBody").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Status": if (!SiteSettings.AllColumn("Status").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Manager": if (!SiteSettings.AllColumn("Manager").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Owner": if (!SiteSettings.AllColumn("Owner").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassA": if (!SiteSettings.AllColumn("ClassA").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassB": if (!SiteSettings.AllColumn("ClassB").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassC": if (!SiteSettings.AllColumn("ClassC").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassD": if (!SiteSettings.AllColumn("ClassD").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassE": if (!SiteSettings.AllColumn("ClassE").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassF": if (!SiteSettings.AllColumn("ClassF").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassG": if (!SiteSettings.AllColumn("ClassG").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassH": if (!SiteSettings.AllColumn("ClassH").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassI": if (!SiteSettings.AllColumn("ClassI").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassJ": if (!SiteSettings.AllColumn("ClassJ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassK": if (!SiteSettings.AllColumn("ClassK").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassL": if (!SiteSettings.AllColumn("ClassL").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassM": if (!SiteSettings.AllColumn("ClassM").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassN": if (!SiteSettings.AllColumn("ClassN").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassO": if (!SiteSettings.AllColumn("ClassO").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassP": if (!SiteSettings.AllColumn("ClassP").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassQ": if (!SiteSettings.AllColumn("ClassQ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassR": if (!SiteSettings.AllColumn("ClassR").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassS": if (!SiteSettings.AllColumn("ClassS").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassT": if (!SiteSettings.AllColumn("ClassT").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassU": if (!SiteSettings.AllColumn("ClassU").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassV": if (!SiteSettings.AllColumn("ClassV").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassW": if (!SiteSettings.AllColumn("ClassW").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassX": if (!SiteSettings.AllColumn("ClassX").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassY": if (!SiteSettings.AllColumn("ClassY").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassZ": if (!SiteSettings.AllColumn("ClassZ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumA": if (!SiteSettings.AllColumn("NumA").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumB": if (!SiteSettings.AllColumn("NumB").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumC": if (!SiteSettings.AllColumn("NumC").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumD": if (!SiteSettings.AllColumn("NumD").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumE": if (!SiteSettings.AllColumn("NumE").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumF": if (!SiteSettings.AllColumn("NumF").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumG": if (!SiteSettings.AllColumn("NumG").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumH": if (!SiteSettings.AllColumn("NumH").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumI": if (!SiteSettings.AllColumn("NumI").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumJ": if (!SiteSettings.AllColumn("NumJ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumK": if (!SiteSettings.AllColumn("NumK").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumL": if (!SiteSettings.AllColumn("NumL").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumM": if (!SiteSettings.AllColumn("NumM").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumN": if (!SiteSettings.AllColumn("NumN").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumO": if (!SiteSettings.AllColumn("NumO").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumP": if (!SiteSettings.AllColumn("NumP").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumQ": if (!SiteSettings.AllColumn("NumQ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumR": if (!SiteSettings.AllColumn("NumR").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumS": if (!SiteSettings.AllColumn("NumS").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumT": if (!SiteSettings.AllColumn("NumT").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumU": if (!SiteSettings.AllColumn("NumU").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumV": if (!SiteSettings.AllColumn("NumV").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumW": if (!SiteSettings.AllColumn("NumW").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumX": if (!SiteSettings.AllColumn("NumX").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumY": if (!SiteSettings.AllColumn("NumY").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumZ": if (!SiteSettings.AllColumn("NumZ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateA": if (!SiteSettings.AllColumn("DateA").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateB": if (!SiteSettings.AllColumn("DateB").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateC": if (!SiteSettings.AllColumn("DateC").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateD": if (!SiteSettings.AllColumn("DateD").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateE": if (!SiteSettings.AllColumn("DateE").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateF": if (!SiteSettings.AllColumn("DateF").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateG": if (!SiteSettings.AllColumn("DateG").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateH": if (!SiteSettings.AllColumn("DateH").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateI": if (!SiteSettings.AllColumn("DateI").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateJ": if (!SiteSettings.AllColumn("DateJ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateK": if (!SiteSettings.AllColumn("DateK").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateL": if (!SiteSettings.AllColumn("DateL").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateM": if (!SiteSettings.AllColumn("DateM").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateN": if (!SiteSettings.AllColumn("DateN").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateO": if (!SiteSettings.AllColumn("DateO").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateP": if (!SiteSettings.AllColumn("DateP").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateQ": if (!SiteSettings.AllColumn("DateQ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateR": if (!SiteSettings.AllColumn("DateR").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateS": if (!SiteSettings.AllColumn("DateS").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateT": if (!SiteSettings.AllColumn("DateT").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateU": if (!SiteSettings.AllColumn("DateU").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateV": if (!SiteSettings.AllColumn("DateV").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateW": if (!SiteSettings.AllColumn("DateW").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateX": if (!SiteSettings.AllColumn("DateX").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateY": if (!SiteSettings.AllColumn("DateY").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateZ": if (!SiteSettings.AllColumn("DateZ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionA": if (!SiteSettings.AllColumn("DescriptionA").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionB": if (!SiteSettings.AllColumn("DescriptionB").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionC": if (!SiteSettings.AllColumn("DescriptionC").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionD": if (!SiteSettings.AllColumn("DescriptionD").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionE": if (!SiteSettings.AllColumn("DescriptionE").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionF": if (!SiteSettings.AllColumn("DescriptionF").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionG": if (!SiteSettings.AllColumn("DescriptionG").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionH": if (!SiteSettings.AllColumn("DescriptionH").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionI": if (!SiteSettings.AllColumn("DescriptionI").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionJ": if (!SiteSettings.AllColumn("DescriptionJ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionK": if (!SiteSettings.AllColumn("DescriptionK").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionL": if (!SiteSettings.AllColumn("DescriptionL").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionM": if (!SiteSettings.AllColumn("DescriptionM").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionN": if (!SiteSettings.AllColumn("DescriptionN").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionO": if (!SiteSettings.AllColumn("DescriptionO").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionP": if (!SiteSettings.AllColumn("DescriptionP").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionQ": if (!SiteSettings.AllColumn("DescriptionQ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionR": if (!SiteSettings.AllColumn("DescriptionR").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionS": if (!SiteSettings.AllColumn("DescriptionS").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionT": if (!SiteSettings.AllColumn("DescriptionT").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionU": if (!SiteSettings.AllColumn("DescriptionU").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionV": if (!SiteSettings.AllColumn("DescriptionV").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionW": if (!SiteSettings.AllColumn("DescriptionW").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionX": if (!SiteSettings.AllColumn("DescriptionX").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionY": if (!SiteSettings.AllColumn("DescriptionY").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionZ": if (!SiteSettings.AllColumn("DescriptionZ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckA": if (!SiteSettings.AllColumn("CheckA").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckB": if (!SiteSettings.AllColumn("CheckB").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckC": if (!SiteSettings.AllColumn("CheckC").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckD": if (!SiteSettings.AllColumn("CheckD").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckE": if (!SiteSettings.AllColumn("CheckE").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckF": if (!SiteSettings.AllColumn("CheckF").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckG": if (!SiteSettings.AllColumn("CheckG").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckH": if (!SiteSettings.AllColumn("CheckH").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckI": if (!SiteSettings.AllColumn("CheckI").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckJ": if (!SiteSettings.AllColumn("CheckJ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckK": if (!SiteSettings.AllColumn("CheckK").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckL": if (!SiteSettings.AllColumn("CheckL").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckM": if (!SiteSettings.AllColumn("CheckM").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckN": if (!SiteSettings.AllColumn("CheckN").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckO": if (!SiteSettings.AllColumn("CheckO").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckP": if (!SiteSettings.AllColumn("CheckP").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckQ": if (!SiteSettings.AllColumn("CheckQ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckR": if (!SiteSettings.AllColumn("CheckR").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckS": if (!SiteSettings.AllColumn("CheckS").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckT": if (!SiteSettings.AllColumn("CheckT").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckU": if (!SiteSettings.AllColumn("CheckU").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckV": if (!SiteSettings.AllColumn("CheckV").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckW": if (!SiteSettings.AllColumn("CheckW").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckX": if (!SiteSettings.AllColumn("CheckX").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckY": if (!SiteSettings.AllColumn("CheckY").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckZ": if (!SiteSettings.AllColumn("CheckZ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        public string Update(SqlParamCollection param = null, bool paramAll = false)
        {
            var error = ValidateBeforeUpdate();
            if (error != null) return error;
            SetBySession();
            OnUpdating(ref param);
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateResults(
                        verUp: VerUp,
                        where: Rds.ResultsWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.ResultsParamDefault(this, paramAll: paramAll),
                        countRecord: true),
                    Rds.If("@@rowcount = 1"),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(ResultId),
                        param: Rds.ItemsParam()
                            .SiteId(SiteId)
                            .Title(ResultsUtility.TitleDisplayValue(SiteSettings, this))
                            .Subset(Jsons.ToJson(new ResultSubset(this, SiteSettings)))
                            .MaintenanceTarget(true)),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(ResultId)),
                    InsertLinks(SiteSettings),
                    Rds.End()
                });
            if (count == 0) return ResponseConflicts();
            SynchronizeSummary();
            Get();
            var responseCollection = new ResultsResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private SqlInsert InsertLinks(
            SiteSettings siteSettings, bool selectIdentity = false)
        {
            var link = new Dictionary<long, long>();
            siteSettings.ColumnCollection.Where(o => o.Link.ToBool()).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "ClassA": if (ClassA.ToLong() != 0 && !link.ContainsKey(ClassA.ToLong())) link.Add(ClassA.ToLong(), ResultId); break;
                    case "ClassB": if (ClassB.ToLong() != 0 && !link.ContainsKey(ClassB.ToLong())) link.Add(ClassB.ToLong(), ResultId); break;
                    case "ClassC": if (ClassC.ToLong() != 0 && !link.ContainsKey(ClassC.ToLong())) link.Add(ClassC.ToLong(), ResultId); break;
                    case "ClassD": if (ClassD.ToLong() != 0 && !link.ContainsKey(ClassD.ToLong())) link.Add(ClassD.ToLong(), ResultId); break;
                    case "ClassE": if (ClassE.ToLong() != 0 && !link.ContainsKey(ClassE.ToLong())) link.Add(ClassE.ToLong(), ResultId); break;
                    case "ClassF": if (ClassF.ToLong() != 0 && !link.ContainsKey(ClassF.ToLong())) link.Add(ClassF.ToLong(), ResultId); break;
                    case "ClassG": if (ClassG.ToLong() != 0 && !link.ContainsKey(ClassG.ToLong())) link.Add(ClassG.ToLong(), ResultId); break;
                    case "ClassH": if (ClassH.ToLong() != 0 && !link.ContainsKey(ClassH.ToLong())) link.Add(ClassH.ToLong(), ResultId); break;
                    case "ClassI": if (ClassI.ToLong() != 0 && !link.ContainsKey(ClassI.ToLong())) link.Add(ClassI.ToLong(), ResultId); break;
                    case "ClassJ": if (ClassJ.ToLong() != 0 && !link.ContainsKey(ClassJ.ToLong())) link.Add(ClassJ.ToLong(), ResultId); break;
                    case "ClassK": if (ClassK.ToLong() != 0 && !link.ContainsKey(ClassK.ToLong())) link.Add(ClassK.ToLong(), ResultId); break;
                    case "ClassL": if (ClassL.ToLong() != 0 && !link.ContainsKey(ClassL.ToLong())) link.Add(ClassL.ToLong(), ResultId); break;
                    case "ClassM": if (ClassM.ToLong() != 0 && !link.ContainsKey(ClassM.ToLong())) link.Add(ClassM.ToLong(), ResultId); break;
                    case "ClassN": if (ClassN.ToLong() != 0 && !link.ContainsKey(ClassN.ToLong())) link.Add(ClassN.ToLong(), ResultId); break;
                    case "ClassO": if (ClassO.ToLong() != 0 && !link.ContainsKey(ClassO.ToLong())) link.Add(ClassO.ToLong(), ResultId); break;
                    case "ClassP": if (ClassP.ToLong() != 0 && !link.ContainsKey(ClassP.ToLong())) link.Add(ClassP.ToLong(), ResultId); break;
                    case "ClassQ": if (ClassQ.ToLong() != 0 && !link.ContainsKey(ClassQ.ToLong())) link.Add(ClassQ.ToLong(), ResultId); break;
                    case "ClassR": if (ClassR.ToLong() != 0 && !link.ContainsKey(ClassR.ToLong())) link.Add(ClassR.ToLong(), ResultId); break;
                    case "ClassS": if (ClassS.ToLong() != 0 && !link.ContainsKey(ClassS.ToLong())) link.Add(ClassS.ToLong(), ResultId); break;
                    case "ClassT": if (ClassT.ToLong() != 0 && !link.ContainsKey(ClassT.ToLong())) link.Add(ClassT.ToLong(), ResultId); break;
                    case "ClassU": if (ClassU.ToLong() != 0 && !link.ContainsKey(ClassU.ToLong())) link.Add(ClassU.ToLong(), ResultId); break;
                    case "ClassV": if (ClassV.ToLong() != 0 && !link.ContainsKey(ClassV.ToLong())) link.Add(ClassV.ToLong(), ResultId); break;
                    case "ClassW": if (ClassW.ToLong() != 0 && !link.ContainsKey(ClassW.ToLong())) link.Add(ClassW.ToLong(), ResultId); break;
                    case "ClassX": if (ClassX.ToLong() != 0 && !link.ContainsKey(ClassX.ToLong())) link.Add(ClassX.ToLong(), ResultId); break;
                    case "ClassY": if (ClassY.ToLong() != 0 && !link.ContainsKey(ClassY.ToLong())) link.Add(ClassY.ToLong(), ResultId); break;
                    case "ClassZ": if (ClassZ.ToLong() != 0 && !link.ContainsKey(ClassZ.ToLong())) link.Add(ClassZ.ToLong(), ResultId); break;
                    default: break;
                }
            });
            return LinksUtility.Insert(link, selectIdentity);
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref ResultsResponseCollection responseCollection)
        {
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanUpdate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Results_SiteId": if (!SiteSettings.AllColumn("SiteId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ResultId": if (!SiteSettings.AllColumn("ResultId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Body": if (!SiteSettings.AllColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_TitleBody": if (!SiteSettings.AllColumn("TitleBody").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Status": if (!SiteSettings.AllColumn("Status").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Manager": if (!SiteSettings.AllColumn("Manager").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Owner": if (!SiteSettings.AllColumn("Owner").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassA": if (!SiteSettings.AllColumn("ClassA").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassB": if (!SiteSettings.AllColumn("ClassB").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassC": if (!SiteSettings.AllColumn("ClassC").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassD": if (!SiteSettings.AllColumn("ClassD").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassE": if (!SiteSettings.AllColumn("ClassE").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassF": if (!SiteSettings.AllColumn("ClassF").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassG": if (!SiteSettings.AllColumn("ClassG").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassH": if (!SiteSettings.AllColumn("ClassH").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassI": if (!SiteSettings.AllColumn("ClassI").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassJ": if (!SiteSettings.AllColumn("ClassJ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassK": if (!SiteSettings.AllColumn("ClassK").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassL": if (!SiteSettings.AllColumn("ClassL").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassM": if (!SiteSettings.AllColumn("ClassM").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassN": if (!SiteSettings.AllColumn("ClassN").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassO": if (!SiteSettings.AllColumn("ClassO").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassP": if (!SiteSettings.AllColumn("ClassP").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassQ": if (!SiteSettings.AllColumn("ClassQ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassR": if (!SiteSettings.AllColumn("ClassR").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassS": if (!SiteSettings.AllColumn("ClassS").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassT": if (!SiteSettings.AllColumn("ClassT").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassU": if (!SiteSettings.AllColumn("ClassU").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassV": if (!SiteSettings.AllColumn("ClassV").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassW": if (!SiteSettings.AllColumn("ClassW").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassX": if (!SiteSettings.AllColumn("ClassX").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassY": if (!SiteSettings.AllColumn("ClassY").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_ClassZ": if (!SiteSettings.AllColumn("ClassZ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumA": if (!SiteSettings.AllColumn("NumA").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumB": if (!SiteSettings.AllColumn("NumB").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumC": if (!SiteSettings.AllColumn("NumC").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumD": if (!SiteSettings.AllColumn("NumD").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumE": if (!SiteSettings.AllColumn("NumE").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumF": if (!SiteSettings.AllColumn("NumF").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumG": if (!SiteSettings.AllColumn("NumG").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumH": if (!SiteSettings.AllColumn("NumH").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumI": if (!SiteSettings.AllColumn("NumI").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumJ": if (!SiteSettings.AllColumn("NumJ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumK": if (!SiteSettings.AllColumn("NumK").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumL": if (!SiteSettings.AllColumn("NumL").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumM": if (!SiteSettings.AllColumn("NumM").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumN": if (!SiteSettings.AllColumn("NumN").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumO": if (!SiteSettings.AllColumn("NumO").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumP": if (!SiteSettings.AllColumn("NumP").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumQ": if (!SiteSettings.AllColumn("NumQ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumR": if (!SiteSettings.AllColumn("NumR").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumS": if (!SiteSettings.AllColumn("NumS").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumT": if (!SiteSettings.AllColumn("NumT").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumU": if (!SiteSettings.AllColumn("NumU").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumV": if (!SiteSettings.AllColumn("NumV").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumW": if (!SiteSettings.AllColumn("NumW").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumX": if (!SiteSettings.AllColumn("NumX").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumY": if (!SiteSettings.AllColumn("NumY").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_NumZ": if (!SiteSettings.AllColumn("NumZ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateA": if (!SiteSettings.AllColumn("DateA").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateB": if (!SiteSettings.AllColumn("DateB").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateC": if (!SiteSettings.AllColumn("DateC").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateD": if (!SiteSettings.AllColumn("DateD").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateE": if (!SiteSettings.AllColumn("DateE").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateF": if (!SiteSettings.AllColumn("DateF").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateG": if (!SiteSettings.AllColumn("DateG").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateH": if (!SiteSettings.AllColumn("DateH").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateI": if (!SiteSettings.AllColumn("DateI").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateJ": if (!SiteSettings.AllColumn("DateJ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateK": if (!SiteSettings.AllColumn("DateK").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateL": if (!SiteSettings.AllColumn("DateL").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateM": if (!SiteSettings.AllColumn("DateM").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateN": if (!SiteSettings.AllColumn("DateN").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateO": if (!SiteSettings.AllColumn("DateO").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateP": if (!SiteSettings.AllColumn("DateP").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateQ": if (!SiteSettings.AllColumn("DateQ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateR": if (!SiteSettings.AllColumn("DateR").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateS": if (!SiteSettings.AllColumn("DateS").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateT": if (!SiteSettings.AllColumn("DateT").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateU": if (!SiteSettings.AllColumn("DateU").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateV": if (!SiteSettings.AllColumn("DateV").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateW": if (!SiteSettings.AllColumn("DateW").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateX": if (!SiteSettings.AllColumn("DateX").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateY": if (!SiteSettings.AllColumn("DateY").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DateZ": if (!SiteSettings.AllColumn("DateZ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionA": if (!SiteSettings.AllColumn("DescriptionA").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionB": if (!SiteSettings.AllColumn("DescriptionB").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionC": if (!SiteSettings.AllColumn("DescriptionC").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionD": if (!SiteSettings.AllColumn("DescriptionD").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionE": if (!SiteSettings.AllColumn("DescriptionE").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionF": if (!SiteSettings.AllColumn("DescriptionF").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionG": if (!SiteSettings.AllColumn("DescriptionG").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionH": if (!SiteSettings.AllColumn("DescriptionH").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionI": if (!SiteSettings.AllColumn("DescriptionI").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionJ": if (!SiteSettings.AllColumn("DescriptionJ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionK": if (!SiteSettings.AllColumn("DescriptionK").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionL": if (!SiteSettings.AllColumn("DescriptionL").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionM": if (!SiteSettings.AllColumn("DescriptionM").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionN": if (!SiteSettings.AllColumn("DescriptionN").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionO": if (!SiteSettings.AllColumn("DescriptionO").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionP": if (!SiteSettings.AllColumn("DescriptionP").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionQ": if (!SiteSettings.AllColumn("DescriptionQ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionR": if (!SiteSettings.AllColumn("DescriptionR").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionS": if (!SiteSettings.AllColumn("DescriptionS").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionT": if (!SiteSettings.AllColumn("DescriptionT").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionU": if (!SiteSettings.AllColumn("DescriptionU").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionV": if (!SiteSettings.AllColumn("DescriptionV").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionW": if (!SiteSettings.AllColumn("DescriptionW").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionX": if (!SiteSettings.AllColumn("DescriptionX").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionY": if (!SiteSettings.AllColumn("DescriptionY").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_DescriptionZ": if (!SiteSettings.AllColumn("DescriptionZ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckA": if (!SiteSettings.AllColumn("CheckA").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckB": if (!SiteSettings.AllColumn("CheckB").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckC": if (!SiteSettings.AllColumn("CheckC").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckD": if (!SiteSettings.AllColumn("CheckD").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckE": if (!SiteSettings.AllColumn("CheckE").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckF": if (!SiteSettings.AllColumn("CheckF").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckG": if (!SiteSettings.AllColumn("CheckG").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckH": if (!SiteSettings.AllColumn("CheckH").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckI": if (!SiteSettings.AllColumn("CheckI").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckJ": if (!SiteSettings.AllColumn("CheckJ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckK": if (!SiteSettings.AllColumn("CheckK").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckL": if (!SiteSettings.AllColumn("CheckL").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckM": if (!SiteSettings.AllColumn("CheckM").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckN": if (!SiteSettings.AllColumn("CheckN").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckO": if (!SiteSettings.AllColumn("CheckO").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckP": if (!SiteSettings.AllColumn("CheckP").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckQ": if (!SiteSettings.AllColumn("CheckQ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckR": if (!SiteSettings.AllColumn("CheckR").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckS": if (!SiteSettings.AllColumn("CheckS").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckT": if (!SiteSettings.AllColumn("CheckT").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckU": if (!SiteSettings.AllColumn("CheckU").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckV": if (!SiteSettings.AllColumn("CheckV").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckW": if (!SiteSettings.AllColumn("CheckW").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckX": if (!SiteSettings.AllColumn("CheckX").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckY": if (!SiteSettings.AllColumn("CheckY").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CheckZ": if (!SiteSettings.AllColumn("CheckZ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Results_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(ResultsResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(this)
                .Formula(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.DisplayValue)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "Results"))
                .Html("#Links", new HtmlBuilder().Links(ResultId))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanUpdate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Results")
                            .SiteId(SiteId)
                            .Title(Title.Value)),
                    Rds.UpdateOrInsertResults(
                        selectIdentity: true,
                        where: where ?? Rds.ResultsWhereDefault(this),
                        param: param ?? Rds.ResultsParamDefault(this, setDefault: true))
                });
            ResultId = newId != 0 ? newId : ResultId;
            Get();
            var responseCollection = new ResultsResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref ResultsResponseCollection responseCollection)
        {
        }

        public string Copy()
        {
            ResultId = 0;
            if (SiteSettings.AllColumn("Title").EditorVisible.ToBool())
            {
                Title.Value += Displays.SuffixCopy();
            }
            if (!Forms.Data("Dialog_ConfirmCopy_WithComments").ToBool())
            {
                Comments.Clear();
            }
            return Create(paramAll: true);
        }

        public string Move()
        {
            var siteId = Forms.Long("Dialog_MoveTargets");
            if (siteId == 0 || !Permissions.CanMove(SiteId, siteId))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SiteId = siteId;
            Rds.ExecuteNonQuery(statements: new SqlStatement[]
            {
                Rds.UpdateItems(
                    where: Rds.ItemsWhere().ReferenceId(ResultId),
                    param: Rds.ItemsParam().SiteId(SiteId)),
                Rds.UpdateResults(
                    where: Rds.ResultsWhere().ResultId(ResultId),
                    param: Rds.ResultsParam().SiteId(SiteId))
            });
            return Editor();
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().ReferenceId(ResultId)),
                    Rds.DeleteResults(
                        where: Rds.ResultsWhere().SiteId(SiteId).ResultId(ResultId))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new ResultsResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.ItemIndex(SiteId));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref ResultsResponseCollection responseCollection)
        {
        }

        public string Restore(long resultId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            ResultId = resultId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(ResultId)),
                    Rds.RestoreResults(
                        where: Rds.ResultsWhere().ResultId(ResultId))
                });
            return new ResponseCollection().ToJson();
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteResults(
                    tableType: tableType,
                    param: Rds.ResultsParam().SiteId(SiteId).ResultId(ResultId)));
            var responseCollection = new ResultsResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref ResultsResponseCollection responseCollection)
        {
        }

        private void SynchronizeSummary()
        {
            SiteSettings.SummaryCollection.ForEach(summary =>
            {
                var id = SynchronizeSummaryDestinationId(summary.LinkColumn);
                var savedId = SynchronizeSummaryDestinationId(summary.LinkColumn, saved: true);
                if (id != 0)
                {
                    SynchronizeSummary(summary, id);
                }
                if (savedId != 0 && id != savedId)
                {
                    SynchronizeSummary(summary, savedId);
                }
            });
        }

        private void SynchronizeSummary(Summary summary, long id)
        {
            Summaries.Synchronize(
                summary.SiteId,
                summary.DestinationReferenceType,
                summary.DestinationColumn,
                SiteId,
                "Results",
                summary.LinkColumn,
                summary.Type,
                summary.SourceColumn,
                id);
            Formulas.Update(id);
        }

        private long SynchronizeSummaryDestinationId(string linkColumn, bool saved = false)
        {
            switch (linkColumn)
            {
                case "ClassA": return saved ? SavedClassA.ToLong() : ClassA.ToLong();
                case "ClassB": return saved ? SavedClassB.ToLong() : ClassB.ToLong();
                case "ClassC": return saved ? SavedClassC.ToLong() : ClassC.ToLong();
                case "ClassD": return saved ? SavedClassD.ToLong() : ClassD.ToLong();
                case "ClassE": return saved ? SavedClassE.ToLong() : ClassE.ToLong();
                case "ClassF": return saved ? SavedClassF.ToLong() : ClassF.ToLong();
                case "ClassG": return saved ? SavedClassG.ToLong() : ClassG.ToLong();
                case "ClassH": return saved ? SavedClassH.ToLong() : ClassH.ToLong();
                case "ClassI": return saved ? SavedClassI.ToLong() : ClassI.ToLong();
                case "ClassJ": return saved ? SavedClassJ.ToLong() : ClassJ.ToLong();
                case "ClassK": return saved ? SavedClassK.ToLong() : ClassK.ToLong();
                case "ClassL": return saved ? SavedClassL.ToLong() : ClassL.ToLong();
                case "ClassM": return saved ? SavedClassM.ToLong() : ClassM.ToLong();
                case "ClassN": return saved ? SavedClassN.ToLong() : ClassN.ToLong();
                case "ClassO": return saved ? SavedClassO.ToLong() : ClassO.ToLong();
                case "ClassP": return saved ? SavedClassP.ToLong() : ClassP.ToLong();
                case "ClassQ": return saved ? SavedClassQ.ToLong() : ClassQ.ToLong();
                case "ClassR": return saved ? SavedClassR.ToLong() : ClassR.ToLong();
                case "ClassS": return saved ? SavedClassS.ToLong() : ClassS.ToLong();
                case "ClassT": return saved ? SavedClassT.ToLong() : ClassT.ToLong();
                case "ClassU": return saved ? SavedClassU.ToLong() : ClassU.ToLong();
                case "ClassV": return saved ? SavedClassV.ToLong() : ClassV.ToLong();
                case "ClassW": return saved ? SavedClassW.ToLong() : ClassW.ToLong();
                case "ClassX": return saved ? SavedClassX.ToLong() : ClassX.ToLong();
                case "ClassY": return saved ? SavedClassY.ToLong() : ClassY.ToLong();
                case "ClassZ": return saved ? SavedClassZ.ToLong() : ClassZ.ToLong();
                default: return 0;
            }
        }

        public void UpdateFormulaColumns()
        {
            SetByFormula();
            var param = Rds.ResultsParam();
            SiteSettings.FormulaHash.Keys.ForEach(columnName =>
            {
                switch (columnName)
                {
                    case "NumA": param.NumA(NumA); break;
                    case "NumB": param.NumB(NumB); break;
                    case "NumC": param.NumC(NumC); break;
                    case "NumD": param.NumD(NumD); break;
                    case "NumE": param.NumE(NumE); break;
                    case "NumF": param.NumF(NumF); break;
                    case "NumG": param.NumG(NumG); break;
                    case "NumH": param.NumH(NumH); break;
                    case "NumI": param.NumI(NumI); break;
                    case "NumJ": param.NumJ(NumJ); break;
                    case "NumK": param.NumK(NumK); break;
                    case "NumL": param.NumL(NumL); break;
                    case "NumM": param.NumM(NumM); break;
                    case "NumN": param.NumN(NumN); break;
                    case "NumO": param.NumO(NumO); break;
                    case "NumP": param.NumP(NumP); break;
                    case "NumQ": param.NumQ(NumQ); break;
                    case "NumR": param.NumR(NumR); break;
                    case "NumS": param.NumS(NumS); break;
                    case "NumT": param.NumT(NumT); break;
                    case "NumU": param.NumU(NumU); break;
                    case "NumV": param.NumV(NumV); break;
                    case "NumW": param.NumW(NumW); break;
                    case "NumX": param.NumX(NumX); break;
                    case "NumY": param.NumY(NumY); break;
                    case "NumZ": param.NumZ(NumZ); break;
                    default: break;
                }
            });
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateResults(
                    param: param,
                    where: Rds.ResultsWhereDefault(this),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        public string Histories()
        {
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () =>
                {
                    hb.GridHeader(
                        columnCollection: SiteSettings.HistoryColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new ResultCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.ResultsWhere().ResultId(ResultId),
                        orderBy: Rds.ResultsOrderBy().Ver(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(resultModel => hb
                            .Tr(
                                attributes: new HtmlAttributes()
                                    .Class("grid-row history not-link")
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-ver", resultModel.Ver)
                                    .Add("data-latest", 1, _using: resultModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryColumnCollection().ForEach(column =>
                                        hb.TdValue(column, resultModel))));
                });
            return new ResultsResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.ResultsWhere()
                    .ResultId(ResultId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            SwitchTargets = ResultsUtility.GetSwitchTargets(SiteSettings, SiteId);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = ResultsUtility.GetSwitchTargets(SiteSettings, SiteId);
            var resultModel = new ResultModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                resultId: switchTargets.Previous(ResultId),
                switchTargets: switchTargets);
            return RecordResponse(resultModel);
        }

        public string Next()
        {
            var switchTargets = ResultsUtility.GetSwitchTargets(SiteSettings, SiteId);
            var resultModel = new ResultModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                resultId: switchTargets.Next(ResultId),
                switchTargets: switchTargets);
            return RecordResponse(resultModel);
        }

        public string Reload()
        {
            SwitchTargets = ResultsUtility.GetSwitchTargets(SiteSettings, SiteId);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            ResultModel resultModel, Message message = null, bool pushState = true)
        {
            var siteModel = new SiteModel(SiteId);
            resultModel.MethodType = BaseModel.MethodTypes.Edit;
            return new ResultsResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    resultModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? ResultsUtility.Editor(siteModel, resultModel)
                        : ResultsUtility.Editor(siteModel, this))
                .Message(message)
                .PushState(
                    "Edit",
                    Navigations.ItemEdit(resultModel.ResultId),
                    _using: pushState)
                .ClearFormData()
                .ToJson();
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Results_Title": Title = new Title(ResultId, Forms.Data(controlId)); break;
                    case "Results_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Results_Status": Status = new Status(Forms.Data(controlId).ToInt()); break;
                    case "Results_Manager": Manager = SiteInfo.User(Forms.Int(controlId)); break;
                    case "Results_Owner": Owner = SiteInfo.User(Forms.Int(controlId)); break;
                    case "Results_ClassA": ClassA = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassB": ClassB = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassC": ClassC = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassD": ClassD = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassE": ClassE = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassF": ClassF = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassG": ClassG = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassH": ClassH = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassI": ClassI = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassJ": ClassJ = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassK": ClassK = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassL": ClassL = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassM": ClassM = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassN": ClassN = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassO": ClassO = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassP": ClassP = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassQ": ClassQ = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassR": ClassR = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassS": ClassS = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassT": ClassT = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassU": ClassU = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassV": ClassV = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassW": ClassW = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassX": ClassX = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassY": ClassY = Forms.Data(controlId).ToString(); break;
                    case "Results_ClassZ": ClassZ = Forms.Data(controlId).ToString(); break;
                    case "Results_NumA": NumA = SiteSettings.AllColumn("NumA").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumB": NumB = SiteSettings.AllColumn("NumB").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumC": NumC = SiteSettings.AllColumn("NumC").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumD": NumD = SiteSettings.AllColumn("NumD").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumE": NumE = SiteSettings.AllColumn("NumE").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumF": NumF = SiteSettings.AllColumn("NumF").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumG": NumG = SiteSettings.AllColumn("NumG").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumH": NumH = SiteSettings.AllColumn("NumH").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumI": NumI = SiteSettings.AllColumn("NumI").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumJ": NumJ = SiteSettings.AllColumn("NumJ").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumK": NumK = SiteSettings.AllColumn("NumK").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumL": NumL = SiteSettings.AllColumn("NumL").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumM": NumM = SiteSettings.AllColumn("NumM").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumN": NumN = SiteSettings.AllColumn("NumN").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumO": NumO = SiteSettings.AllColumn("NumO").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumP": NumP = SiteSettings.AllColumn("NumP").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumQ": NumQ = SiteSettings.AllColumn("NumQ").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumR": NumR = SiteSettings.AllColumn("NumR").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumS": NumS = SiteSettings.AllColumn("NumS").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumT": NumT = SiteSettings.AllColumn("NumT").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumU": NumU = SiteSettings.AllColumn("NumU").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumV": NumV = SiteSettings.AllColumn("NumV").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumW": NumW = SiteSettings.AllColumn("NumW").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumX": NumX = SiteSettings.AllColumn("NumX").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumY": NumY = SiteSettings.AllColumn("NumY").Round(Forms.Decimal(controlId)); break;
                    case "Results_NumZ": NumZ = SiteSettings.AllColumn("NumZ").Round(Forms.Decimal(controlId)); break;
                    case "Results_DateA": DateA = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateB": DateB = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateC": DateC = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateD": DateD = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateE": DateE = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateF": DateF = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateG": DateG = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateH": DateH = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateI": DateI = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateJ": DateJ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateK": DateK = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateL": DateL = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateM": DateM = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateN": DateN = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateO": DateO = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateP": DateP = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateQ": DateQ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateR": DateR = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateS": DateS = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateT": DateT = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateU": DateU = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateV": DateV = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateW": DateW = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateX": DateX = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateY": DateY = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DateZ": DateZ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Results_DescriptionA": DescriptionA = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionB": DescriptionB = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionC": DescriptionC = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionD": DescriptionD = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionE": DescriptionE = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionF": DescriptionF = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionG": DescriptionG = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionH": DescriptionH = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionI": DescriptionI = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionJ": DescriptionJ = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionK": DescriptionK = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionL": DescriptionL = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionM": DescriptionM = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionN": DescriptionN = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionO": DescriptionO = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionP": DescriptionP = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionQ": DescriptionQ = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionR": DescriptionR = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionS": DescriptionS = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionT": DescriptionT = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionU": DescriptionU = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionV": DescriptionV = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionW": DescriptionW = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionX": DescriptionX = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionY": DescriptionY = Forms.Data(controlId).ToString(); break;
                    case "Results_DescriptionZ": DescriptionZ = Forms.Data(controlId).ToString(); break;
                    case "Results_CheckA": CheckA = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckB": CheckB = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckC": CheckC = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckD": CheckD = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckE": CheckE = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckF": CheckF = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckG": CheckG = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckH": CheckH = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckI": CheckI = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckJ": CheckJ = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckK": CheckK = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckL": CheckL = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckM": CheckM = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckN": CheckN = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckO": CheckO = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckP": CheckP = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckQ": CheckQ = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckR": CheckR = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckS": CheckS = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckT": CheckT = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckU": CheckU = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckV": CheckV = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckW": CheckW = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckX": CheckX = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckY": CheckY = Forms.Data(controlId).ToBool(); break;
                    case "Results_CheckZ": CheckZ = Forms.Data(controlId).ToBool(); break;
                    case "Results_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default: break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.Data("ControlId").Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    default: break;
                }
            });
            SetByFormula();
        }

        private void SetByFormula()
        {
            if (SiteSettings.FormulaHash?.Count > 0)
            {
                var data = new Dictionary<string, decimal>
                {
                    { "NumA", NumA },
                    { "NumB", NumB },
                    { "NumC", NumC },
                    { "NumD", NumD },
                    { "NumE", NumE },
                    { "NumF", NumF },
                    { "NumG", NumG },
                    { "NumH", NumH },
                    { "NumI", NumI },
                    { "NumJ", NumJ },
                    { "NumK", NumK },
                    { "NumL", NumL },
                    { "NumM", NumM },
                    { "NumN", NumN },
                    { "NumO", NumO },
                    { "NumP", NumP },
                    { "NumQ", NumQ },
                    { "NumR", NumR },
                    { "NumS", NumS },
                    { "NumT", NumT },
                    { "NumU", NumU },
                    { "NumV", NumV },
                    { "NumW", NumW },
                    { "NumX", NumX },
                    { "NumY", NumY },
                    { "NumZ", NumZ }
                };
                SiteSettings.FormulaHash.Keys.ForEach(columnName =>
                {
                    switch (columnName)
                    {
                        case "NumA": NumA = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumB": NumB = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumC": NumC = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumD": NumD = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumE": NumE = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumF": NumF = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumG": NumG = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumH": NumH = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumI": NumI = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumJ": NumJ = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumK": NumK = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumL": NumL = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumM": NumM = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumN": NumN = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumO": NumO = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumP": NumP = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumQ": NumQ = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumR": NumR = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumS": NumS = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumT": NumT = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumU": NumU = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumV": NumV = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumW": NumW = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumX": NumX = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumY": NumY = SiteSettings.FormulaResult(columnName, data); break;
                        case "NumZ": NumZ = SiteSettings.FormulaResult(columnName, data); break;
                        default: break;
                    }
                });
            }
        }

        private void SetBySession()
        {
        }

        private void Set(DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(DataRow dataRow)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var name = dataColumn.ColumnName;
                switch(name)
                {
                    case "SiteId": if (dataRow[name] != DBNull.Value) { SiteId = dataRow[name].ToLong(); SavedSiteId = SiteId; } break;
                    case "UpdatedTime": if (dataRow[name] != DBNull.Value) { UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; } break;
                    case "ResultId": if (dataRow[name] != DBNull.Value) { ResultId = dataRow[name].ToLong(); SavedResultId = ResultId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "Title": Title = new Title(dataRow, "ResultId"); SavedTitle = Title.Value; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "Status": Status = new Status(dataRow, "Status"); SavedStatus = Status.Value; break;
                    case "Manager": Manager = SiteInfo.User(dataRow.Int(name)); SavedManager = Manager.Id; break;
                    case "Owner": Owner = SiteInfo.User(dataRow.Int(name)); SavedOwner = Owner.Id; break;
                    case "ClassA": ClassA = dataRow[name].ToString(); SavedClassA = ClassA; break;
                    case "ClassB": ClassB = dataRow[name].ToString(); SavedClassB = ClassB; break;
                    case "ClassC": ClassC = dataRow[name].ToString(); SavedClassC = ClassC; break;
                    case "ClassD": ClassD = dataRow[name].ToString(); SavedClassD = ClassD; break;
                    case "ClassE": ClassE = dataRow[name].ToString(); SavedClassE = ClassE; break;
                    case "ClassF": ClassF = dataRow[name].ToString(); SavedClassF = ClassF; break;
                    case "ClassG": ClassG = dataRow[name].ToString(); SavedClassG = ClassG; break;
                    case "ClassH": ClassH = dataRow[name].ToString(); SavedClassH = ClassH; break;
                    case "ClassI": ClassI = dataRow[name].ToString(); SavedClassI = ClassI; break;
                    case "ClassJ": ClassJ = dataRow[name].ToString(); SavedClassJ = ClassJ; break;
                    case "ClassK": ClassK = dataRow[name].ToString(); SavedClassK = ClassK; break;
                    case "ClassL": ClassL = dataRow[name].ToString(); SavedClassL = ClassL; break;
                    case "ClassM": ClassM = dataRow[name].ToString(); SavedClassM = ClassM; break;
                    case "ClassN": ClassN = dataRow[name].ToString(); SavedClassN = ClassN; break;
                    case "ClassO": ClassO = dataRow[name].ToString(); SavedClassO = ClassO; break;
                    case "ClassP": ClassP = dataRow[name].ToString(); SavedClassP = ClassP; break;
                    case "ClassQ": ClassQ = dataRow[name].ToString(); SavedClassQ = ClassQ; break;
                    case "ClassR": ClassR = dataRow[name].ToString(); SavedClassR = ClassR; break;
                    case "ClassS": ClassS = dataRow[name].ToString(); SavedClassS = ClassS; break;
                    case "ClassT": ClassT = dataRow[name].ToString(); SavedClassT = ClassT; break;
                    case "ClassU": ClassU = dataRow[name].ToString(); SavedClassU = ClassU; break;
                    case "ClassV": ClassV = dataRow[name].ToString(); SavedClassV = ClassV; break;
                    case "ClassW": ClassW = dataRow[name].ToString(); SavedClassW = ClassW; break;
                    case "ClassX": ClassX = dataRow[name].ToString(); SavedClassX = ClassX; break;
                    case "ClassY": ClassY = dataRow[name].ToString(); SavedClassY = ClassY; break;
                    case "ClassZ": ClassZ = dataRow[name].ToString(); SavedClassZ = ClassZ; break;
                    case "NumA": NumA = dataRow[name].ToDecimal(); SavedNumA = NumA; break;
                    case "NumB": NumB = dataRow[name].ToDecimal(); SavedNumB = NumB; break;
                    case "NumC": NumC = dataRow[name].ToDecimal(); SavedNumC = NumC; break;
                    case "NumD": NumD = dataRow[name].ToDecimal(); SavedNumD = NumD; break;
                    case "NumE": NumE = dataRow[name].ToDecimal(); SavedNumE = NumE; break;
                    case "NumF": NumF = dataRow[name].ToDecimal(); SavedNumF = NumF; break;
                    case "NumG": NumG = dataRow[name].ToDecimal(); SavedNumG = NumG; break;
                    case "NumH": NumH = dataRow[name].ToDecimal(); SavedNumH = NumH; break;
                    case "NumI": NumI = dataRow[name].ToDecimal(); SavedNumI = NumI; break;
                    case "NumJ": NumJ = dataRow[name].ToDecimal(); SavedNumJ = NumJ; break;
                    case "NumK": NumK = dataRow[name].ToDecimal(); SavedNumK = NumK; break;
                    case "NumL": NumL = dataRow[name].ToDecimal(); SavedNumL = NumL; break;
                    case "NumM": NumM = dataRow[name].ToDecimal(); SavedNumM = NumM; break;
                    case "NumN": NumN = dataRow[name].ToDecimal(); SavedNumN = NumN; break;
                    case "NumO": NumO = dataRow[name].ToDecimal(); SavedNumO = NumO; break;
                    case "NumP": NumP = dataRow[name].ToDecimal(); SavedNumP = NumP; break;
                    case "NumQ": NumQ = dataRow[name].ToDecimal(); SavedNumQ = NumQ; break;
                    case "NumR": NumR = dataRow[name].ToDecimal(); SavedNumR = NumR; break;
                    case "NumS": NumS = dataRow[name].ToDecimal(); SavedNumS = NumS; break;
                    case "NumT": NumT = dataRow[name].ToDecimal(); SavedNumT = NumT; break;
                    case "NumU": NumU = dataRow[name].ToDecimal(); SavedNumU = NumU; break;
                    case "NumV": NumV = dataRow[name].ToDecimal(); SavedNumV = NumV; break;
                    case "NumW": NumW = dataRow[name].ToDecimal(); SavedNumW = NumW; break;
                    case "NumX": NumX = dataRow[name].ToDecimal(); SavedNumX = NumX; break;
                    case "NumY": NumY = dataRow[name].ToDecimal(); SavedNumY = NumY; break;
                    case "NumZ": NumZ = dataRow[name].ToDecimal(); SavedNumZ = NumZ; break;
                    case "DateA": DateA = dataRow[name].ToDateTime(); SavedDateA = DateA; break;
                    case "DateB": DateB = dataRow[name].ToDateTime(); SavedDateB = DateB; break;
                    case "DateC": DateC = dataRow[name].ToDateTime(); SavedDateC = DateC; break;
                    case "DateD": DateD = dataRow[name].ToDateTime(); SavedDateD = DateD; break;
                    case "DateE": DateE = dataRow[name].ToDateTime(); SavedDateE = DateE; break;
                    case "DateF": DateF = dataRow[name].ToDateTime(); SavedDateF = DateF; break;
                    case "DateG": DateG = dataRow[name].ToDateTime(); SavedDateG = DateG; break;
                    case "DateH": DateH = dataRow[name].ToDateTime(); SavedDateH = DateH; break;
                    case "DateI": DateI = dataRow[name].ToDateTime(); SavedDateI = DateI; break;
                    case "DateJ": DateJ = dataRow[name].ToDateTime(); SavedDateJ = DateJ; break;
                    case "DateK": DateK = dataRow[name].ToDateTime(); SavedDateK = DateK; break;
                    case "DateL": DateL = dataRow[name].ToDateTime(); SavedDateL = DateL; break;
                    case "DateM": DateM = dataRow[name].ToDateTime(); SavedDateM = DateM; break;
                    case "DateN": DateN = dataRow[name].ToDateTime(); SavedDateN = DateN; break;
                    case "DateO": DateO = dataRow[name].ToDateTime(); SavedDateO = DateO; break;
                    case "DateP": DateP = dataRow[name].ToDateTime(); SavedDateP = DateP; break;
                    case "DateQ": DateQ = dataRow[name].ToDateTime(); SavedDateQ = DateQ; break;
                    case "DateR": DateR = dataRow[name].ToDateTime(); SavedDateR = DateR; break;
                    case "DateS": DateS = dataRow[name].ToDateTime(); SavedDateS = DateS; break;
                    case "DateT": DateT = dataRow[name].ToDateTime(); SavedDateT = DateT; break;
                    case "DateU": DateU = dataRow[name].ToDateTime(); SavedDateU = DateU; break;
                    case "DateV": DateV = dataRow[name].ToDateTime(); SavedDateV = DateV; break;
                    case "DateW": DateW = dataRow[name].ToDateTime(); SavedDateW = DateW; break;
                    case "DateX": DateX = dataRow[name].ToDateTime(); SavedDateX = DateX; break;
                    case "DateY": DateY = dataRow[name].ToDateTime(); SavedDateY = DateY; break;
                    case "DateZ": DateZ = dataRow[name].ToDateTime(); SavedDateZ = DateZ; break;
                    case "DescriptionA": DescriptionA = dataRow[name].ToString(); SavedDescriptionA = DescriptionA; break;
                    case "DescriptionB": DescriptionB = dataRow[name].ToString(); SavedDescriptionB = DescriptionB; break;
                    case "DescriptionC": DescriptionC = dataRow[name].ToString(); SavedDescriptionC = DescriptionC; break;
                    case "DescriptionD": DescriptionD = dataRow[name].ToString(); SavedDescriptionD = DescriptionD; break;
                    case "DescriptionE": DescriptionE = dataRow[name].ToString(); SavedDescriptionE = DescriptionE; break;
                    case "DescriptionF": DescriptionF = dataRow[name].ToString(); SavedDescriptionF = DescriptionF; break;
                    case "DescriptionG": DescriptionG = dataRow[name].ToString(); SavedDescriptionG = DescriptionG; break;
                    case "DescriptionH": DescriptionH = dataRow[name].ToString(); SavedDescriptionH = DescriptionH; break;
                    case "DescriptionI": DescriptionI = dataRow[name].ToString(); SavedDescriptionI = DescriptionI; break;
                    case "DescriptionJ": DescriptionJ = dataRow[name].ToString(); SavedDescriptionJ = DescriptionJ; break;
                    case "DescriptionK": DescriptionK = dataRow[name].ToString(); SavedDescriptionK = DescriptionK; break;
                    case "DescriptionL": DescriptionL = dataRow[name].ToString(); SavedDescriptionL = DescriptionL; break;
                    case "DescriptionM": DescriptionM = dataRow[name].ToString(); SavedDescriptionM = DescriptionM; break;
                    case "DescriptionN": DescriptionN = dataRow[name].ToString(); SavedDescriptionN = DescriptionN; break;
                    case "DescriptionO": DescriptionO = dataRow[name].ToString(); SavedDescriptionO = DescriptionO; break;
                    case "DescriptionP": DescriptionP = dataRow[name].ToString(); SavedDescriptionP = DescriptionP; break;
                    case "DescriptionQ": DescriptionQ = dataRow[name].ToString(); SavedDescriptionQ = DescriptionQ; break;
                    case "DescriptionR": DescriptionR = dataRow[name].ToString(); SavedDescriptionR = DescriptionR; break;
                    case "DescriptionS": DescriptionS = dataRow[name].ToString(); SavedDescriptionS = DescriptionS; break;
                    case "DescriptionT": DescriptionT = dataRow[name].ToString(); SavedDescriptionT = DescriptionT; break;
                    case "DescriptionU": DescriptionU = dataRow[name].ToString(); SavedDescriptionU = DescriptionU; break;
                    case "DescriptionV": DescriptionV = dataRow[name].ToString(); SavedDescriptionV = DescriptionV; break;
                    case "DescriptionW": DescriptionW = dataRow[name].ToString(); SavedDescriptionW = DescriptionW; break;
                    case "DescriptionX": DescriptionX = dataRow[name].ToString(); SavedDescriptionX = DescriptionX; break;
                    case "DescriptionY": DescriptionY = dataRow[name].ToString(); SavedDescriptionY = DescriptionY; break;
                    case "DescriptionZ": DescriptionZ = dataRow[name].ToString(); SavedDescriptionZ = DescriptionZ; break;
                    case "CheckA": CheckA = dataRow[name].ToBool(); SavedCheckA = CheckA; break;
                    case "CheckB": CheckB = dataRow[name].ToBool(); SavedCheckB = CheckB; break;
                    case "CheckC": CheckC = dataRow[name].ToBool(); SavedCheckC = CheckC; break;
                    case "CheckD": CheckD = dataRow[name].ToBool(); SavedCheckD = CheckD; break;
                    case "CheckE": CheckE = dataRow[name].ToBool(); SavedCheckE = CheckE; break;
                    case "CheckF": CheckF = dataRow[name].ToBool(); SavedCheckF = CheckF; break;
                    case "CheckG": CheckG = dataRow[name].ToBool(); SavedCheckG = CheckG; break;
                    case "CheckH": CheckH = dataRow[name].ToBool(); SavedCheckH = CheckH; break;
                    case "CheckI": CheckI = dataRow[name].ToBool(); SavedCheckI = CheckI; break;
                    case "CheckJ": CheckJ = dataRow[name].ToBool(); SavedCheckJ = CheckJ; break;
                    case "CheckK": CheckK = dataRow[name].ToBool(); SavedCheckK = CheckK; break;
                    case "CheckL": CheckL = dataRow[name].ToBool(); SavedCheckL = CheckL; break;
                    case "CheckM": CheckM = dataRow[name].ToBool(); SavedCheckM = CheckM; break;
                    case "CheckN": CheckN = dataRow[name].ToBool(); SavedCheckN = CheckN; break;
                    case "CheckO": CheckO = dataRow[name].ToBool(); SavedCheckO = CheckO; break;
                    case "CheckP": CheckP = dataRow[name].ToBool(); SavedCheckP = CheckP; break;
                    case "CheckQ": CheckQ = dataRow[name].ToBool(); SavedCheckQ = CheckQ; break;
                    case "CheckR": CheckR = dataRow[name].ToBool(); SavedCheckR = CheckR; break;
                    case "CheckS": CheckS = dataRow[name].ToBool(); SavedCheckS = CheckS; break;
                    case "CheckT": CheckT = dataRow[name].ToBool(); SavedCheckT = CheckT; break;
                    case "CheckU": CheckU = dataRow[name].ToBool(); SavedCheckU = CheckU; break;
                    case "CheckV": CheckV = dataRow[name].ToBool(); SavedCheckV = CheckV; break;
                    case "CheckW": CheckW = dataRow[name].ToBool(); SavedCheckW = CheckW; break;
                    case "CheckX": CheckX = dataRow[name].ToBool(); SavedCheckX = CheckX; break;
                    case "CheckY": CheckY = dataRow[name].ToBool(); SavedCheckY = CheckY; break;
                    case "CheckZ": CheckZ = dataRow[name].ToBool(); SavedCheckZ = CheckZ; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
            if (SiteSettings != null)
            {
                Title.DisplayValue = ResultsUtility.TitleDisplayValue(SiteSettings, this);
            }
        }

        private string Editor()
        {
            var siteModel = new SiteModel(SiteId);
            return new ResultsResponseCollection(this)
                .Html("#MainContainer", ResultsUtility.Editor(siteModel, this))
                .ToJson();
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }
    }

    public class ResultSubset
    {
        public long SiteId;
        public string SiteId_LabelText;
        public Time UpdatedTime;
        public string UpdatedTime_LabelText;
        public long ResultId;
        public string ResultId_LabelText;
        public int Ver;
        public string Ver_LabelText;
        public Title Title;
        public string Title_LabelText;
        public string Body;
        public string Body_LabelText;
        public TitleBody TitleBody;
        public string TitleBody_LabelText;
        public Status Status;
        public string Status_LabelText;
        public User Manager;
        public string Manager_LabelText;
        public User Owner;
        public string Owner_LabelText;
        public string ClassA;
        public string ClassA_LabelText;
        public string ClassB;
        public string ClassB_LabelText;
        public string ClassC;
        public string ClassC_LabelText;
        public string ClassD;
        public string ClassD_LabelText;
        public string ClassE;
        public string ClassE_LabelText;
        public string ClassF;
        public string ClassF_LabelText;
        public string ClassG;
        public string ClassG_LabelText;
        public string ClassH;
        public string ClassH_LabelText;
        public string ClassI;
        public string ClassI_LabelText;
        public string ClassJ;
        public string ClassJ_LabelText;
        public string ClassK;
        public string ClassK_LabelText;
        public string ClassL;
        public string ClassL_LabelText;
        public string ClassM;
        public string ClassM_LabelText;
        public string ClassN;
        public string ClassN_LabelText;
        public string ClassO;
        public string ClassO_LabelText;
        public string ClassP;
        public string ClassP_LabelText;
        public string ClassQ;
        public string ClassQ_LabelText;
        public string ClassR;
        public string ClassR_LabelText;
        public string ClassS;
        public string ClassS_LabelText;
        public string ClassT;
        public string ClassT_LabelText;
        public string ClassU;
        public string ClassU_LabelText;
        public string ClassV;
        public string ClassV_LabelText;
        public string ClassW;
        public string ClassW_LabelText;
        public string ClassX;
        public string ClassX_LabelText;
        public string ClassY;
        public string ClassY_LabelText;
        public string ClassZ;
        public string ClassZ_LabelText;
        public decimal NumA;
        public string NumA_LabelText;
        public decimal NumB;
        public string NumB_LabelText;
        public decimal NumC;
        public string NumC_LabelText;
        public decimal NumD;
        public string NumD_LabelText;
        public decimal NumE;
        public string NumE_LabelText;
        public decimal NumF;
        public string NumF_LabelText;
        public decimal NumG;
        public string NumG_LabelText;
        public decimal NumH;
        public string NumH_LabelText;
        public decimal NumI;
        public string NumI_LabelText;
        public decimal NumJ;
        public string NumJ_LabelText;
        public decimal NumK;
        public string NumK_LabelText;
        public decimal NumL;
        public string NumL_LabelText;
        public decimal NumM;
        public string NumM_LabelText;
        public decimal NumN;
        public string NumN_LabelText;
        public decimal NumO;
        public string NumO_LabelText;
        public decimal NumP;
        public string NumP_LabelText;
        public decimal NumQ;
        public string NumQ_LabelText;
        public decimal NumR;
        public string NumR_LabelText;
        public decimal NumS;
        public string NumS_LabelText;
        public decimal NumT;
        public string NumT_LabelText;
        public decimal NumU;
        public string NumU_LabelText;
        public decimal NumV;
        public string NumV_LabelText;
        public decimal NumW;
        public string NumW_LabelText;
        public decimal NumX;
        public string NumX_LabelText;
        public decimal NumY;
        public string NumY_LabelText;
        public decimal NumZ;
        public string NumZ_LabelText;
        public DateTime DateA;
        public string DateA_LabelText;
        public DateTime DateB;
        public string DateB_LabelText;
        public DateTime DateC;
        public string DateC_LabelText;
        public DateTime DateD;
        public string DateD_LabelText;
        public DateTime DateE;
        public string DateE_LabelText;
        public DateTime DateF;
        public string DateF_LabelText;
        public DateTime DateG;
        public string DateG_LabelText;
        public DateTime DateH;
        public string DateH_LabelText;
        public DateTime DateI;
        public string DateI_LabelText;
        public DateTime DateJ;
        public string DateJ_LabelText;
        public DateTime DateK;
        public string DateK_LabelText;
        public DateTime DateL;
        public string DateL_LabelText;
        public DateTime DateM;
        public string DateM_LabelText;
        public DateTime DateN;
        public string DateN_LabelText;
        public DateTime DateO;
        public string DateO_LabelText;
        public DateTime DateP;
        public string DateP_LabelText;
        public DateTime DateQ;
        public string DateQ_LabelText;
        public DateTime DateR;
        public string DateR_LabelText;
        public DateTime DateS;
        public string DateS_LabelText;
        public DateTime DateT;
        public string DateT_LabelText;
        public DateTime DateU;
        public string DateU_LabelText;
        public DateTime DateV;
        public string DateV_LabelText;
        public DateTime DateW;
        public string DateW_LabelText;
        public DateTime DateX;
        public string DateX_LabelText;
        public DateTime DateY;
        public string DateY_LabelText;
        public DateTime DateZ;
        public string DateZ_LabelText;
        public string DescriptionA;
        public string DescriptionA_LabelText;
        public string DescriptionB;
        public string DescriptionB_LabelText;
        public string DescriptionC;
        public string DescriptionC_LabelText;
        public string DescriptionD;
        public string DescriptionD_LabelText;
        public string DescriptionE;
        public string DescriptionE_LabelText;
        public string DescriptionF;
        public string DescriptionF_LabelText;
        public string DescriptionG;
        public string DescriptionG_LabelText;
        public string DescriptionH;
        public string DescriptionH_LabelText;
        public string DescriptionI;
        public string DescriptionI_LabelText;
        public string DescriptionJ;
        public string DescriptionJ_LabelText;
        public string DescriptionK;
        public string DescriptionK_LabelText;
        public string DescriptionL;
        public string DescriptionL_LabelText;
        public string DescriptionM;
        public string DescriptionM_LabelText;
        public string DescriptionN;
        public string DescriptionN_LabelText;
        public string DescriptionO;
        public string DescriptionO_LabelText;
        public string DescriptionP;
        public string DescriptionP_LabelText;
        public string DescriptionQ;
        public string DescriptionQ_LabelText;
        public string DescriptionR;
        public string DescriptionR_LabelText;
        public string DescriptionS;
        public string DescriptionS_LabelText;
        public string DescriptionT;
        public string DescriptionT_LabelText;
        public string DescriptionU;
        public string DescriptionU_LabelText;
        public string DescriptionV;
        public string DescriptionV_LabelText;
        public string DescriptionW;
        public string DescriptionW_LabelText;
        public string DescriptionX;
        public string DescriptionX_LabelText;
        public string DescriptionY;
        public string DescriptionY_LabelText;
        public string DescriptionZ;
        public string DescriptionZ_LabelText;
        public bool CheckA;
        public string CheckA_LabelText;
        public bool CheckB;
        public string CheckB_LabelText;
        public bool CheckC;
        public string CheckC_LabelText;
        public bool CheckD;
        public string CheckD_LabelText;
        public bool CheckE;
        public string CheckE_LabelText;
        public bool CheckF;
        public string CheckF_LabelText;
        public bool CheckG;
        public string CheckG_LabelText;
        public bool CheckH;
        public string CheckH_LabelText;
        public bool CheckI;
        public string CheckI_LabelText;
        public bool CheckJ;
        public string CheckJ_LabelText;
        public bool CheckK;
        public string CheckK_LabelText;
        public bool CheckL;
        public string CheckL_LabelText;
        public bool CheckM;
        public string CheckM_LabelText;
        public bool CheckN;
        public string CheckN_LabelText;
        public bool CheckO;
        public string CheckO_LabelText;
        public bool CheckP;
        public string CheckP_LabelText;
        public bool CheckQ;
        public string CheckQ_LabelText;
        public bool CheckR;
        public string CheckR_LabelText;
        public bool CheckS;
        public string CheckS_LabelText;
        public bool CheckT;
        public string CheckT_LabelText;
        public bool CheckU;
        public string CheckU_LabelText;
        public bool CheckV;
        public string CheckV_LabelText;
        public bool CheckW;
        public string CheckW_LabelText;
        public bool CheckX;
        public string CheckX_LabelText;
        public bool CheckY;
        public string CheckY_LabelText;
        public bool CheckZ;
        public string CheckZ_LabelText;
        public Comments Comments;
        public string Comments_LabelText;
        public User Creator;
        public string Creator_LabelText;
        public User Updator;
        public string Updator_LabelText;
        public Time CreatedTime;
        public string CreatedTime_LabelText;
        public bool VerUp;
        public string VerUp_LabelText;
        public string Timestamp;
        public string Timestamp_LabelText;

        public ResultSubset()
        {
        }

        public ResultSubset(ResultModel resultModel, SiteSettings siteSettings)
        {
            SiteId = resultModel.SiteId;
            SiteId_LabelText = siteSettings.EditorColumn("SiteId")?.LabelText;
            UpdatedTime = resultModel.UpdatedTime;
            UpdatedTime_LabelText = siteSettings.EditorColumn("UpdatedTime")?.LabelText;
            ResultId = resultModel.ResultId;
            ResultId_LabelText = siteSettings.EditorColumn("ResultId")?.LabelText;
            Ver = resultModel.Ver;
            Ver_LabelText = siteSettings.EditorColumn("Ver")?.LabelText;
            Title = resultModel.Title;
            Title_LabelText = siteSettings.EditorColumn("Title")?.LabelText;
            Body = resultModel.Body;
            Body_LabelText = siteSettings.EditorColumn("Body")?.LabelText;
            TitleBody = resultModel.TitleBody;
            TitleBody_LabelText = siteSettings.EditorColumn("TitleBody")?.LabelText;
            Status = resultModel.Status;
            Status_LabelText = siteSettings.EditorColumn("Status")?.LabelText;
            Manager = resultModel.Manager;
            Manager_LabelText = siteSettings.EditorColumn("Manager")?.LabelText;
            Owner = resultModel.Owner;
            Owner_LabelText = siteSettings.EditorColumn("Owner")?.LabelText;
            ClassA = resultModel.ClassA;
            ClassA_LabelText = siteSettings.EditorColumn("ClassA")?.LabelText;
            ClassB = resultModel.ClassB;
            ClassB_LabelText = siteSettings.EditorColumn("ClassB")?.LabelText;
            ClassC = resultModel.ClassC;
            ClassC_LabelText = siteSettings.EditorColumn("ClassC")?.LabelText;
            ClassD = resultModel.ClassD;
            ClassD_LabelText = siteSettings.EditorColumn("ClassD")?.LabelText;
            ClassE = resultModel.ClassE;
            ClassE_LabelText = siteSettings.EditorColumn("ClassE")?.LabelText;
            ClassF = resultModel.ClassF;
            ClassF_LabelText = siteSettings.EditorColumn("ClassF")?.LabelText;
            ClassG = resultModel.ClassG;
            ClassG_LabelText = siteSettings.EditorColumn("ClassG")?.LabelText;
            ClassH = resultModel.ClassH;
            ClassH_LabelText = siteSettings.EditorColumn("ClassH")?.LabelText;
            ClassI = resultModel.ClassI;
            ClassI_LabelText = siteSettings.EditorColumn("ClassI")?.LabelText;
            ClassJ = resultModel.ClassJ;
            ClassJ_LabelText = siteSettings.EditorColumn("ClassJ")?.LabelText;
            ClassK = resultModel.ClassK;
            ClassK_LabelText = siteSettings.EditorColumn("ClassK")?.LabelText;
            ClassL = resultModel.ClassL;
            ClassL_LabelText = siteSettings.EditorColumn("ClassL")?.LabelText;
            ClassM = resultModel.ClassM;
            ClassM_LabelText = siteSettings.EditorColumn("ClassM")?.LabelText;
            ClassN = resultModel.ClassN;
            ClassN_LabelText = siteSettings.EditorColumn("ClassN")?.LabelText;
            ClassO = resultModel.ClassO;
            ClassO_LabelText = siteSettings.EditorColumn("ClassO")?.LabelText;
            ClassP = resultModel.ClassP;
            ClassP_LabelText = siteSettings.EditorColumn("ClassP")?.LabelText;
            ClassQ = resultModel.ClassQ;
            ClassQ_LabelText = siteSettings.EditorColumn("ClassQ")?.LabelText;
            ClassR = resultModel.ClassR;
            ClassR_LabelText = siteSettings.EditorColumn("ClassR")?.LabelText;
            ClassS = resultModel.ClassS;
            ClassS_LabelText = siteSettings.EditorColumn("ClassS")?.LabelText;
            ClassT = resultModel.ClassT;
            ClassT_LabelText = siteSettings.EditorColumn("ClassT")?.LabelText;
            ClassU = resultModel.ClassU;
            ClassU_LabelText = siteSettings.EditorColumn("ClassU")?.LabelText;
            ClassV = resultModel.ClassV;
            ClassV_LabelText = siteSettings.EditorColumn("ClassV")?.LabelText;
            ClassW = resultModel.ClassW;
            ClassW_LabelText = siteSettings.EditorColumn("ClassW")?.LabelText;
            ClassX = resultModel.ClassX;
            ClassX_LabelText = siteSettings.EditorColumn("ClassX")?.LabelText;
            ClassY = resultModel.ClassY;
            ClassY_LabelText = siteSettings.EditorColumn("ClassY")?.LabelText;
            ClassZ = resultModel.ClassZ;
            ClassZ_LabelText = siteSettings.EditorColumn("ClassZ")?.LabelText;
            NumA = resultModel.NumA;
            NumA_LabelText = siteSettings.EditorColumn("NumA")?.LabelText;
            NumB = resultModel.NumB;
            NumB_LabelText = siteSettings.EditorColumn("NumB")?.LabelText;
            NumC = resultModel.NumC;
            NumC_LabelText = siteSettings.EditorColumn("NumC")?.LabelText;
            NumD = resultModel.NumD;
            NumD_LabelText = siteSettings.EditorColumn("NumD")?.LabelText;
            NumE = resultModel.NumE;
            NumE_LabelText = siteSettings.EditorColumn("NumE")?.LabelText;
            NumF = resultModel.NumF;
            NumF_LabelText = siteSettings.EditorColumn("NumF")?.LabelText;
            NumG = resultModel.NumG;
            NumG_LabelText = siteSettings.EditorColumn("NumG")?.LabelText;
            NumH = resultModel.NumH;
            NumH_LabelText = siteSettings.EditorColumn("NumH")?.LabelText;
            NumI = resultModel.NumI;
            NumI_LabelText = siteSettings.EditorColumn("NumI")?.LabelText;
            NumJ = resultModel.NumJ;
            NumJ_LabelText = siteSettings.EditorColumn("NumJ")?.LabelText;
            NumK = resultModel.NumK;
            NumK_LabelText = siteSettings.EditorColumn("NumK")?.LabelText;
            NumL = resultModel.NumL;
            NumL_LabelText = siteSettings.EditorColumn("NumL")?.LabelText;
            NumM = resultModel.NumM;
            NumM_LabelText = siteSettings.EditorColumn("NumM")?.LabelText;
            NumN = resultModel.NumN;
            NumN_LabelText = siteSettings.EditorColumn("NumN")?.LabelText;
            NumO = resultModel.NumO;
            NumO_LabelText = siteSettings.EditorColumn("NumO")?.LabelText;
            NumP = resultModel.NumP;
            NumP_LabelText = siteSettings.EditorColumn("NumP")?.LabelText;
            NumQ = resultModel.NumQ;
            NumQ_LabelText = siteSettings.EditorColumn("NumQ")?.LabelText;
            NumR = resultModel.NumR;
            NumR_LabelText = siteSettings.EditorColumn("NumR")?.LabelText;
            NumS = resultModel.NumS;
            NumS_LabelText = siteSettings.EditorColumn("NumS")?.LabelText;
            NumT = resultModel.NumT;
            NumT_LabelText = siteSettings.EditorColumn("NumT")?.LabelText;
            NumU = resultModel.NumU;
            NumU_LabelText = siteSettings.EditorColumn("NumU")?.LabelText;
            NumV = resultModel.NumV;
            NumV_LabelText = siteSettings.EditorColumn("NumV")?.LabelText;
            NumW = resultModel.NumW;
            NumW_LabelText = siteSettings.EditorColumn("NumW")?.LabelText;
            NumX = resultModel.NumX;
            NumX_LabelText = siteSettings.EditorColumn("NumX")?.LabelText;
            NumY = resultModel.NumY;
            NumY_LabelText = siteSettings.EditorColumn("NumY")?.LabelText;
            NumZ = resultModel.NumZ;
            NumZ_LabelText = siteSettings.EditorColumn("NumZ")?.LabelText;
            DateA = resultModel.DateA;
            DateA_LabelText = siteSettings.EditorColumn("DateA")?.LabelText;
            DateB = resultModel.DateB;
            DateB_LabelText = siteSettings.EditorColumn("DateB")?.LabelText;
            DateC = resultModel.DateC;
            DateC_LabelText = siteSettings.EditorColumn("DateC")?.LabelText;
            DateD = resultModel.DateD;
            DateD_LabelText = siteSettings.EditorColumn("DateD")?.LabelText;
            DateE = resultModel.DateE;
            DateE_LabelText = siteSettings.EditorColumn("DateE")?.LabelText;
            DateF = resultModel.DateF;
            DateF_LabelText = siteSettings.EditorColumn("DateF")?.LabelText;
            DateG = resultModel.DateG;
            DateG_LabelText = siteSettings.EditorColumn("DateG")?.LabelText;
            DateH = resultModel.DateH;
            DateH_LabelText = siteSettings.EditorColumn("DateH")?.LabelText;
            DateI = resultModel.DateI;
            DateI_LabelText = siteSettings.EditorColumn("DateI")?.LabelText;
            DateJ = resultModel.DateJ;
            DateJ_LabelText = siteSettings.EditorColumn("DateJ")?.LabelText;
            DateK = resultModel.DateK;
            DateK_LabelText = siteSettings.EditorColumn("DateK")?.LabelText;
            DateL = resultModel.DateL;
            DateL_LabelText = siteSettings.EditorColumn("DateL")?.LabelText;
            DateM = resultModel.DateM;
            DateM_LabelText = siteSettings.EditorColumn("DateM")?.LabelText;
            DateN = resultModel.DateN;
            DateN_LabelText = siteSettings.EditorColumn("DateN")?.LabelText;
            DateO = resultModel.DateO;
            DateO_LabelText = siteSettings.EditorColumn("DateO")?.LabelText;
            DateP = resultModel.DateP;
            DateP_LabelText = siteSettings.EditorColumn("DateP")?.LabelText;
            DateQ = resultModel.DateQ;
            DateQ_LabelText = siteSettings.EditorColumn("DateQ")?.LabelText;
            DateR = resultModel.DateR;
            DateR_LabelText = siteSettings.EditorColumn("DateR")?.LabelText;
            DateS = resultModel.DateS;
            DateS_LabelText = siteSettings.EditorColumn("DateS")?.LabelText;
            DateT = resultModel.DateT;
            DateT_LabelText = siteSettings.EditorColumn("DateT")?.LabelText;
            DateU = resultModel.DateU;
            DateU_LabelText = siteSettings.EditorColumn("DateU")?.LabelText;
            DateV = resultModel.DateV;
            DateV_LabelText = siteSettings.EditorColumn("DateV")?.LabelText;
            DateW = resultModel.DateW;
            DateW_LabelText = siteSettings.EditorColumn("DateW")?.LabelText;
            DateX = resultModel.DateX;
            DateX_LabelText = siteSettings.EditorColumn("DateX")?.LabelText;
            DateY = resultModel.DateY;
            DateY_LabelText = siteSettings.EditorColumn("DateY")?.LabelText;
            DateZ = resultModel.DateZ;
            DateZ_LabelText = siteSettings.EditorColumn("DateZ")?.LabelText;
            DescriptionA = resultModel.DescriptionA;
            DescriptionA_LabelText = siteSettings.EditorColumn("DescriptionA")?.LabelText;
            DescriptionB = resultModel.DescriptionB;
            DescriptionB_LabelText = siteSettings.EditorColumn("DescriptionB")?.LabelText;
            DescriptionC = resultModel.DescriptionC;
            DescriptionC_LabelText = siteSettings.EditorColumn("DescriptionC")?.LabelText;
            DescriptionD = resultModel.DescriptionD;
            DescriptionD_LabelText = siteSettings.EditorColumn("DescriptionD")?.LabelText;
            DescriptionE = resultModel.DescriptionE;
            DescriptionE_LabelText = siteSettings.EditorColumn("DescriptionE")?.LabelText;
            DescriptionF = resultModel.DescriptionF;
            DescriptionF_LabelText = siteSettings.EditorColumn("DescriptionF")?.LabelText;
            DescriptionG = resultModel.DescriptionG;
            DescriptionG_LabelText = siteSettings.EditorColumn("DescriptionG")?.LabelText;
            DescriptionH = resultModel.DescriptionH;
            DescriptionH_LabelText = siteSettings.EditorColumn("DescriptionH")?.LabelText;
            DescriptionI = resultModel.DescriptionI;
            DescriptionI_LabelText = siteSettings.EditorColumn("DescriptionI")?.LabelText;
            DescriptionJ = resultModel.DescriptionJ;
            DescriptionJ_LabelText = siteSettings.EditorColumn("DescriptionJ")?.LabelText;
            DescriptionK = resultModel.DescriptionK;
            DescriptionK_LabelText = siteSettings.EditorColumn("DescriptionK")?.LabelText;
            DescriptionL = resultModel.DescriptionL;
            DescriptionL_LabelText = siteSettings.EditorColumn("DescriptionL")?.LabelText;
            DescriptionM = resultModel.DescriptionM;
            DescriptionM_LabelText = siteSettings.EditorColumn("DescriptionM")?.LabelText;
            DescriptionN = resultModel.DescriptionN;
            DescriptionN_LabelText = siteSettings.EditorColumn("DescriptionN")?.LabelText;
            DescriptionO = resultModel.DescriptionO;
            DescriptionO_LabelText = siteSettings.EditorColumn("DescriptionO")?.LabelText;
            DescriptionP = resultModel.DescriptionP;
            DescriptionP_LabelText = siteSettings.EditorColumn("DescriptionP")?.LabelText;
            DescriptionQ = resultModel.DescriptionQ;
            DescriptionQ_LabelText = siteSettings.EditorColumn("DescriptionQ")?.LabelText;
            DescriptionR = resultModel.DescriptionR;
            DescriptionR_LabelText = siteSettings.EditorColumn("DescriptionR")?.LabelText;
            DescriptionS = resultModel.DescriptionS;
            DescriptionS_LabelText = siteSettings.EditorColumn("DescriptionS")?.LabelText;
            DescriptionT = resultModel.DescriptionT;
            DescriptionT_LabelText = siteSettings.EditorColumn("DescriptionT")?.LabelText;
            DescriptionU = resultModel.DescriptionU;
            DescriptionU_LabelText = siteSettings.EditorColumn("DescriptionU")?.LabelText;
            DescriptionV = resultModel.DescriptionV;
            DescriptionV_LabelText = siteSettings.EditorColumn("DescriptionV")?.LabelText;
            DescriptionW = resultModel.DescriptionW;
            DescriptionW_LabelText = siteSettings.EditorColumn("DescriptionW")?.LabelText;
            DescriptionX = resultModel.DescriptionX;
            DescriptionX_LabelText = siteSettings.EditorColumn("DescriptionX")?.LabelText;
            DescriptionY = resultModel.DescriptionY;
            DescriptionY_LabelText = siteSettings.EditorColumn("DescriptionY")?.LabelText;
            DescriptionZ = resultModel.DescriptionZ;
            DescriptionZ_LabelText = siteSettings.EditorColumn("DescriptionZ")?.LabelText;
            CheckA = resultModel.CheckA;
            CheckA_LabelText = siteSettings.EditorColumn("CheckA")?.LabelText;
            CheckB = resultModel.CheckB;
            CheckB_LabelText = siteSettings.EditorColumn("CheckB")?.LabelText;
            CheckC = resultModel.CheckC;
            CheckC_LabelText = siteSettings.EditorColumn("CheckC")?.LabelText;
            CheckD = resultModel.CheckD;
            CheckD_LabelText = siteSettings.EditorColumn("CheckD")?.LabelText;
            CheckE = resultModel.CheckE;
            CheckE_LabelText = siteSettings.EditorColumn("CheckE")?.LabelText;
            CheckF = resultModel.CheckF;
            CheckF_LabelText = siteSettings.EditorColumn("CheckF")?.LabelText;
            CheckG = resultModel.CheckG;
            CheckG_LabelText = siteSettings.EditorColumn("CheckG")?.LabelText;
            CheckH = resultModel.CheckH;
            CheckH_LabelText = siteSettings.EditorColumn("CheckH")?.LabelText;
            CheckI = resultModel.CheckI;
            CheckI_LabelText = siteSettings.EditorColumn("CheckI")?.LabelText;
            CheckJ = resultModel.CheckJ;
            CheckJ_LabelText = siteSettings.EditorColumn("CheckJ")?.LabelText;
            CheckK = resultModel.CheckK;
            CheckK_LabelText = siteSettings.EditorColumn("CheckK")?.LabelText;
            CheckL = resultModel.CheckL;
            CheckL_LabelText = siteSettings.EditorColumn("CheckL")?.LabelText;
            CheckM = resultModel.CheckM;
            CheckM_LabelText = siteSettings.EditorColumn("CheckM")?.LabelText;
            CheckN = resultModel.CheckN;
            CheckN_LabelText = siteSettings.EditorColumn("CheckN")?.LabelText;
            CheckO = resultModel.CheckO;
            CheckO_LabelText = siteSettings.EditorColumn("CheckO")?.LabelText;
            CheckP = resultModel.CheckP;
            CheckP_LabelText = siteSettings.EditorColumn("CheckP")?.LabelText;
            CheckQ = resultModel.CheckQ;
            CheckQ_LabelText = siteSettings.EditorColumn("CheckQ")?.LabelText;
            CheckR = resultModel.CheckR;
            CheckR_LabelText = siteSettings.EditorColumn("CheckR")?.LabelText;
            CheckS = resultModel.CheckS;
            CheckS_LabelText = siteSettings.EditorColumn("CheckS")?.LabelText;
            CheckT = resultModel.CheckT;
            CheckT_LabelText = siteSettings.EditorColumn("CheckT")?.LabelText;
            CheckU = resultModel.CheckU;
            CheckU_LabelText = siteSettings.EditorColumn("CheckU")?.LabelText;
            CheckV = resultModel.CheckV;
            CheckV_LabelText = siteSettings.EditorColumn("CheckV")?.LabelText;
            CheckW = resultModel.CheckW;
            CheckW_LabelText = siteSettings.EditorColumn("CheckW")?.LabelText;
            CheckX = resultModel.CheckX;
            CheckX_LabelText = siteSettings.EditorColumn("CheckX")?.LabelText;
            CheckY = resultModel.CheckY;
            CheckY_LabelText = siteSettings.EditorColumn("CheckY")?.LabelText;
            CheckZ = resultModel.CheckZ;
            CheckZ_LabelText = siteSettings.EditorColumn("CheckZ")?.LabelText;
            Comments = resultModel.Comments;
            Comments_LabelText = siteSettings.EditorColumn("Comments")?.LabelText;
            Creator = resultModel.Creator;
            Creator_LabelText = siteSettings.EditorColumn("Creator")?.LabelText;
            Updator = resultModel.Updator;
            Updator_LabelText = siteSettings.EditorColumn("Updator")?.LabelText;
            CreatedTime = resultModel.CreatedTime;
            CreatedTime_LabelText = siteSettings.EditorColumn("CreatedTime")?.LabelText;
            VerUp = resultModel.VerUp;
            VerUp_LabelText = siteSettings.EditorColumn("VerUp")?.LabelText;
            Timestamp = resultModel.Timestamp;
            Timestamp_LabelText = siteSettings.EditorColumn("Timestamp")?.LabelText;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
            SiteId.SearchIndexes(searchIndexHash, 200);
            UpdatedTime.SearchIndexes(searchIndexHash, 200);
            ResultId.SearchIndexes(searchIndexHash, 1);
            Title.SearchIndexes(searchIndexHash, 4);
            Body.SearchIndexes(searchIndexHash, 200);
            Status.SearchIndexes(searchIndexHash, 200);
            Manager.SearchIndexes(searchIndexHash, 100);
            Owner.SearchIndexes(searchIndexHash, 100);
            ClassA.SearchIndexes(searchIndexHash, 200);
            ClassB.SearchIndexes(searchIndexHash, 200);
            ClassC.SearchIndexes(searchIndexHash, 200);
            ClassD.SearchIndexes(searchIndexHash, 200);
            ClassE.SearchIndexes(searchIndexHash, 200);
            ClassF.SearchIndexes(searchIndexHash, 200);
            ClassG.SearchIndexes(searchIndexHash, 200);
            ClassH.SearchIndexes(searchIndexHash, 200);
            ClassI.SearchIndexes(searchIndexHash, 200);
            ClassJ.SearchIndexes(searchIndexHash, 200);
            ClassK.SearchIndexes(searchIndexHash, 200);
            ClassL.SearchIndexes(searchIndexHash, 200);
            ClassM.SearchIndexes(searchIndexHash, 200);
            ClassN.SearchIndexes(searchIndexHash, 200);
            ClassO.SearchIndexes(searchIndexHash, 200);
            ClassP.SearchIndexes(searchIndexHash, 200);
            ClassQ.SearchIndexes(searchIndexHash, 200);
            ClassR.SearchIndexes(searchIndexHash, 200);
            ClassS.SearchIndexes(searchIndexHash, 200);
            ClassT.SearchIndexes(searchIndexHash, 200);
            ClassU.SearchIndexes(searchIndexHash, 200);
            ClassV.SearchIndexes(searchIndexHash, 200);
            ClassW.SearchIndexes(searchIndexHash, 200);
            ClassX.SearchIndexes(searchIndexHash, 200);
            ClassY.SearchIndexes(searchIndexHash, 200);
            ClassZ.SearchIndexes(searchIndexHash, 200);
            NumA.SearchIndexes(searchIndexHash, 200);
            NumB.SearchIndexes(searchIndexHash, 200);
            NumC.SearchIndexes(searchIndexHash, 200);
            NumD.SearchIndexes(searchIndexHash, 200);
            NumE.SearchIndexes(searchIndexHash, 200);
            NumF.SearchIndexes(searchIndexHash, 200);
            NumG.SearchIndexes(searchIndexHash, 200);
            NumH.SearchIndexes(searchIndexHash, 200);
            NumI.SearchIndexes(searchIndexHash, 200);
            NumJ.SearchIndexes(searchIndexHash, 200);
            NumK.SearchIndexes(searchIndexHash, 200);
            NumL.SearchIndexes(searchIndexHash, 200);
            NumM.SearchIndexes(searchIndexHash, 200);
            NumN.SearchIndexes(searchIndexHash, 200);
            NumO.SearchIndexes(searchIndexHash, 200);
            NumP.SearchIndexes(searchIndexHash, 200);
            NumQ.SearchIndexes(searchIndexHash, 200);
            NumR.SearchIndexes(searchIndexHash, 200);
            NumS.SearchIndexes(searchIndexHash, 200);
            NumT.SearchIndexes(searchIndexHash, 200);
            NumU.SearchIndexes(searchIndexHash, 200);
            NumV.SearchIndexes(searchIndexHash, 200);
            NumW.SearchIndexes(searchIndexHash, 200);
            NumX.SearchIndexes(searchIndexHash, 200);
            NumY.SearchIndexes(searchIndexHash, 200);
            NumZ.SearchIndexes(searchIndexHash, 200);
            DateA.SearchIndexes(searchIndexHash, 200);
            DateB.SearchIndexes(searchIndexHash, 200);
            DateC.SearchIndexes(searchIndexHash, 200);
            DateD.SearchIndexes(searchIndexHash, 200);
            DateE.SearchIndexes(searchIndexHash, 200);
            DateF.SearchIndexes(searchIndexHash, 200);
            DateG.SearchIndexes(searchIndexHash, 200);
            DateH.SearchIndexes(searchIndexHash, 200);
            DateI.SearchIndexes(searchIndexHash, 200);
            DateJ.SearchIndexes(searchIndexHash, 200);
            DateK.SearchIndexes(searchIndexHash, 200);
            DateL.SearchIndexes(searchIndexHash, 200);
            DateM.SearchIndexes(searchIndexHash, 200);
            DateN.SearchIndexes(searchIndexHash, 200);
            DateO.SearchIndexes(searchIndexHash, 200);
            DateP.SearchIndexes(searchIndexHash, 200);
            DateQ.SearchIndexes(searchIndexHash, 200);
            DateR.SearchIndexes(searchIndexHash, 200);
            DateS.SearchIndexes(searchIndexHash, 200);
            DateT.SearchIndexes(searchIndexHash, 200);
            DateU.SearchIndexes(searchIndexHash, 200);
            DateV.SearchIndexes(searchIndexHash, 200);
            DateW.SearchIndexes(searchIndexHash, 200);
            DateX.SearchIndexes(searchIndexHash, 200);
            DateY.SearchIndexes(searchIndexHash, 200);
            DateZ.SearchIndexes(searchIndexHash, 200);
            DescriptionA.SearchIndexes(searchIndexHash, 200);
            DescriptionB.SearchIndexes(searchIndexHash, 200);
            DescriptionC.SearchIndexes(searchIndexHash, 200);
            DescriptionD.SearchIndexes(searchIndexHash, 200);
            DescriptionE.SearchIndexes(searchIndexHash, 200);
            DescriptionF.SearchIndexes(searchIndexHash, 200);
            DescriptionG.SearchIndexes(searchIndexHash, 200);
            DescriptionH.SearchIndexes(searchIndexHash, 200);
            DescriptionI.SearchIndexes(searchIndexHash, 200);
            DescriptionJ.SearchIndexes(searchIndexHash, 200);
            DescriptionK.SearchIndexes(searchIndexHash, 200);
            DescriptionL.SearchIndexes(searchIndexHash, 200);
            DescriptionM.SearchIndexes(searchIndexHash, 200);
            DescriptionN.SearchIndexes(searchIndexHash, 200);
            DescriptionO.SearchIndexes(searchIndexHash, 200);
            DescriptionP.SearchIndexes(searchIndexHash, 200);
            DescriptionQ.SearchIndexes(searchIndexHash, 200);
            DescriptionR.SearchIndexes(searchIndexHash, 200);
            DescriptionS.SearchIndexes(searchIndexHash, 200);
            DescriptionT.SearchIndexes(searchIndexHash, 200);
            DescriptionU.SearchIndexes(searchIndexHash, 200);
            DescriptionV.SearchIndexes(searchIndexHash, 200);
            DescriptionW.SearchIndexes(searchIndexHash, 200);
            DescriptionX.SearchIndexes(searchIndexHash, 200);
            DescriptionY.SearchIndexes(searchIndexHash, 200);
            DescriptionZ.SearchIndexes(searchIndexHash, 200);
            Comments.SearchIndexes(searchIndexHash, 200);
            Creator.SearchIndexes(searchIndexHash, 100);
            Updator.SearchIndexes(searchIndexHash, 100);
            CreatedTime.SearchIndexes(searchIndexHash, 200);
            SearchIndexExtensions.OutgoingMailsSearchIndexes(searchIndexHash, "Results", ResultId);
            return searchIndexHash;
        }
    }

    public class ResultCollection : List<ResultModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public ResultCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null,
            bool get = true)
        {
            if (get)
            {
                Set(siteSettings, permissionType, Get(
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord,
                    aggregationCollection: aggregationCollection));
            }
        }

        public ResultCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private ResultCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new ResultModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public ResultCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            string commandText,
            SqlParamCollection param = null)
        {
            Set(siteSettings, permissionType, Get(commandText, param));
        }

        private DataTable Get(
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool history = false,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null)
        {
            var statements = new List<SqlStatement>
            {
                Rds.SelectResults(
                    dataTableName: "Main",
                    column: column ?? Rds.ResultsColumnDefault(),
                    join: join ??  Rds.ResultsJoinDefault(),
                    where: where ?? null,
                    orderBy: orderBy ?? null,
                    param: param ?? null,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord)
            };
            if (aggregationCollection != null)
            {
                statements.AddRange(Rds.ResultsAggregations(aggregationCollection, where));
            }
            var dataSet = Rds.ExecuteDataSet(
                transactional: false,
                statements: statements.ToArray());
            Aggregations.Set(dataSet, aggregationCollection);
            return dataSet.Tables["Main"];
        }

        private DataTable Get(string commandText, SqlParamCollection param = null)
        {
            return Rds.ExecuteTable(
                transactional: false,
                statements: Rds.ResultsStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class ResultsUtility
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                referenceId: "Results",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: Libraries.Scripts.JavaScripts.DataView(
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                userStyle: siteSettings.GridStyle,
                userScript: siteSettings.GridScript,
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id_Css("ResultsForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "Results",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: resultCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    resultCollection: resultCollection,
                                    siteSettings: siteSettings,
                                    permissionType: permissionType,
                                    formData: formData,
                                    dataViewName: dataViewName))
                            .MainCommands(
                                siteId: siteSettings.SiteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest,
                                backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Results")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .Dialog_Move("items", siteSettings.SiteId, bulk: true)
                    .Dialog_ImportSettings()
                    .Div(attributes: new HtmlAttributes()
                        .Id_Css("Dialog_ExportSettings", "dialog")
                        .Title(Displays.ExportSettings()))).ToString();
        }

        private static ResultCollection ResultCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new ResultCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Results",
                    formData: formData,
                    where: Rds.ResultsWhere().SiteId(siteSettings.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.ResultsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            ResultCollection resultCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                case "TimeSeries":
                    return hb.TimeSeries(
                        siteSettings: siteSettings,
                        permissionType: permissionType,
                        formData: formData,
                        bodyOnly: false);
                case "Kamban":
                    return hb.Kamban(
                        siteSettings: siteSettings,
                        permissionType: permissionType,
                        formData: formData,
                        bodyOnly: false);
                default: return hb.Grid(
                    resultCollection: resultCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DataView(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            switch (DataViewSelectors.Get(siteSettings.SiteId))
            {
                case "TimeSeries": return TimeSeriesResponse(siteSettings, permissionType);
                case "Kamban": return KambanResponse(siteSettings, permissionType);
                default: return Grid(siteSettings: siteSettings, permissionType: permissionType);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResultCollection resultCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id_Css("Grid", "grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            resultCollection: resultCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == resultCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    resultCollection: resultCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: resultCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        public static string GridRows(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResponseCollection responseCollection = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    resultCollection: resultCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: resultCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, resultCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            ResultCollection resultCollection,
            FormData formData,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            if (addHeader)
            {
                hb.GridHeader(
                    columnCollection: siteSettings.GridColumnCollection(), 
                    formData: formData,
                    checkAll: checkAll);
            }
            resultCollection.ForEach(resultModel => hb
                .Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(resultModel.ResultId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: resultModel.ResultId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    resultModel: resultModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.ResultsColumn()
                .SiteId()
                .ResultId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "UpdatedTime": select.UpdatedTime(); break;
                    case "ResultId": select.ResultId(); break;
                    case "Ver": select.Ver(); break;
                    case "Title": select.Title(); break;
                    case "Body": select.Body(); break;
                    case "TitleBody": select.TitleBody(); break;
                    case "Status": select.Status(); break;
                    case "Manager": select.Manager(); break;
                    case "Owner": select.Owner(); break;
                    case "ClassA": select.ClassA(); break;
                    case "ClassB": select.ClassB(); break;
                    case "ClassC": select.ClassC(); break;
                    case "ClassD": select.ClassD(); break;
                    case "ClassE": select.ClassE(); break;
                    case "ClassF": select.ClassF(); break;
                    case "ClassG": select.ClassG(); break;
                    case "ClassH": select.ClassH(); break;
                    case "ClassI": select.ClassI(); break;
                    case "ClassJ": select.ClassJ(); break;
                    case "ClassK": select.ClassK(); break;
                    case "ClassL": select.ClassL(); break;
                    case "ClassM": select.ClassM(); break;
                    case "ClassN": select.ClassN(); break;
                    case "ClassO": select.ClassO(); break;
                    case "ClassP": select.ClassP(); break;
                    case "ClassQ": select.ClassQ(); break;
                    case "ClassR": select.ClassR(); break;
                    case "ClassS": select.ClassS(); break;
                    case "ClassT": select.ClassT(); break;
                    case "ClassU": select.ClassU(); break;
                    case "ClassV": select.ClassV(); break;
                    case "ClassW": select.ClassW(); break;
                    case "ClassX": select.ClassX(); break;
                    case "ClassY": select.ClassY(); break;
                    case "ClassZ": select.ClassZ(); break;
                    case "NumA": select.NumA(); break;
                    case "NumB": select.NumB(); break;
                    case "NumC": select.NumC(); break;
                    case "NumD": select.NumD(); break;
                    case "NumE": select.NumE(); break;
                    case "NumF": select.NumF(); break;
                    case "NumG": select.NumG(); break;
                    case "NumH": select.NumH(); break;
                    case "NumI": select.NumI(); break;
                    case "NumJ": select.NumJ(); break;
                    case "NumK": select.NumK(); break;
                    case "NumL": select.NumL(); break;
                    case "NumM": select.NumM(); break;
                    case "NumN": select.NumN(); break;
                    case "NumO": select.NumO(); break;
                    case "NumP": select.NumP(); break;
                    case "NumQ": select.NumQ(); break;
                    case "NumR": select.NumR(); break;
                    case "NumS": select.NumS(); break;
                    case "NumT": select.NumT(); break;
                    case "NumU": select.NumU(); break;
                    case "NumV": select.NumV(); break;
                    case "NumW": select.NumW(); break;
                    case "NumX": select.NumX(); break;
                    case "NumY": select.NumY(); break;
                    case "NumZ": select.NumZ(); break;
                    case "DateA": select.DateA(); break;
                    case "DateB": select.DateB(); break;
                    case "DateC": select.DateC(); break;
                    case "DateD": select.DateD(); break;
                    case "DateE": select.DateE(); break;
                    case "DateF": select.DateF(); break;
                    case "DateG": select.DateG(); break;
                    case "DateH": select.DateH(); break;
                    case "DateI": select.DateI(); break;
                    case "DateJ": select.DateJ(); break;
                    case "DateK": select.DateK(); break;
                    case "DateL": select.DateL(); break;
                    case "DateM": select.DateM(); break;
                    case "DateN": select.DateN(); break;
                    case "DateO": select.DateO(); break;
                    case "DateP": select.DateP(); break;
                    case "DateQ": select.DateQ(); break;
                    case "DateR": select.DateR(); break;
                    case "DateS": select.DateS(); break;
                    case "DateT": select.DateT(); break;
                    case "DateU": select.DateU(); break;
                    case "DateV": select.DateV(); break;
                    case "DateW": select.DateW(); break;
                    case "DateX": select.DateX(); break;
                    case "DateY": select.DateY(); break;
                    case "DateZ": select.DateZ(); break;
                    case "DescriptionA": select.DescriptionA(); break;
                    case "DescriptionB": select.DescriptionB(); break;
                    case "DescriptionC": select.DescriptionC(); break;
                    case "DescriptionD": select.DescriptionD(); break;
                    case "DescriptionE": select.DescriptionE(); break;
                    case "DescriptionF": select.DescriptionF(); break;
                    case "DescriptionG": select.DescriptionG(); break;
                    case "DescriptionH": select.DescriptionH(); break;
                    case "DescriptionI": select.DescriptionI(); break;
                    case "DescriptionJ": select.DescriptionJ(); break;
                    case "DescriptionK": select.DescriptionK(); break;
                    case "DescriptionL": select.DescriptionL(); break;
                    case "DescriptionM": select.DescriptionM(); break;
                    case "DescriptionN": select.DescriptionN(); break;
                    case "DescriptionO": select.DescriptionO(); break;
                    case "DescriptionP": select.DescriptionP(); break;
                    case "DescriptionQ": select.DescriptionQ(); break;
                    case "DescriptionR": select.DescriptionR(); break;
                    case "DescriptionS": select.DescriptionS(); break;
                    case "DescriptionT": select.DescriptionT(); break;
                    case "DescriptionU": select.DescriptionU(); break;
                    case "DescriptionV": select.DescriptionV(); break;
                    case "DescriptionW": select.DescriptionW(); break;
                    case "DescriptionX": select.DescriptionX(); break;
                    case "DescriptionY": select.DescriptionY(); break;
                    case "DescriptionZ": select.DescriptionZ(); break;
                    case "CheckA": select.CheckA(); break;
                    case "CheckB": select.CheckB(); break;
                    case "CheckC": select.CheckC(); break;
                    case "CheckD": select.CheckD(); break;
                    case "CheckE": select.CheckE(); break;
                    case "CheckF": select.CheckF(); break;
                    case "CheckG": select.CheckG(); break;
                    case "CheckH": select.CheckH(); break;
                    case "CheckI": select.CheckI(); break;
                    case "CheckJ": select.CheckJ(); break;
                    case "CheckK": select.CheckK(); break;
                    case "CheckL": select.CheckL(); break;
                    case "CheckM": select.CheckM(); break;
                    case "CheckN": select.CheckN(); break;
                    case "CheckO": select.CheckO(); break;
                    case "CheckP": select.CheckP(); break;
                    case "CheckQ": select.CheckQ(); break;
                    case "CheckR": select.CheckR(); break;
                    case "CheckS": select.CheckS(); break;
                    case "CheckT": select.CheckT(); break;
                    case "CheckU": select.CheckU(); break;
                    case "CheckV": select.CheckV(); break;
                    case "CheckW": select.CheckW(); break;
                    case "CheckX": select.CheckX(); break;
                    case "CheckY": select.CheckY(); break;
                    case "CheckZ": select.CheckZ(); break;
                    case "Comments": select.Comments(); break;
                    case "Creator": select.Creator(); break;
                    case "Updator": select.Updator(); break;
                    case "CreatedTime": select.CreatedTime(); break;
                }
            });
            return select;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, ResultModel resultModel)
        {
            switch (column.ColumnName)
            {
                case "SiteId": return hb.Td(column: column, value: resultModel.SiteId);
                case "UpdatedTime": return hb.Td(column: column, value: resultModel.UpdatedTime);
                case "ResultId": return hb.Td(column: column, value: resultModel.ResultId);
                case "Ver": return hb.Td(column: column, value: resultModel.Ver);
                case "Title": return hb.Td(column: column, value: resultModel.Title);
                case "Body": return hb.Td(column: column, value: resultModel.Body);
                case "TitleBody": return hb.Td(column: column, value: resultModel.TitleBody);
                case "Status": return hb.Td(column: column, value: resultModel.Status);
                case "Manager": return hb.Td(column: column, value: resultModel.Manager);
                case "Owner": return hb.Td(column: column, value: resultModel.Owner);
                case "ClassA": return hb.Td(column: column, value: resultModel.ClassA);
                case "ClassB": return hb.Td(column: column, value: resultModel.ClassB);
                case "ClassC": return hb.Td(column: column, value: resultModel.ClassC);
                case "ClassD": return hb.Td(column: column, value: resultModel.ClassD);
                case "ClassE": return hb.Td(column: column, value: resultModel.ClassE);
                case "ClassF": return hb.Td(column: column, value: resultModel.ClassF);
                case "ClassG": return hb.Td(column: column, value: resultModel.ClassG);
                case "ClassH": return hb.Td(column: column, value: resultModel.ClassH);
                case "ClassI": return hb.Td(column: column, value: resultModel.ClassI);
                case "ClassJ": return hb.Td(column: column, value: resultModel.ClassJ);
                case "ClassK": return hb.Td(column: column, value: resultModel.ClassK);
                case "ClassL": return hb.Td(column: column, value: resultModel.ClassL);
                case "ClassM": return hb.Td(column: column, value: resultModel.ClassM);
                case "ClassN": return hb.Td(column: column, value: resultModel.ClassN);
                case "ClassO": return hb.Td(column: column, value: resultModel.ClassO);
                case "ClassP": return hb.Td(column: column, value: resultModel.ClassP);
                case "ClassQ": return hb.Td(column: column, value: resultModel.ClassQ);
                case "ClassR": return hb.Td(column: column, value: resultModel.ClassR);
                case "ClassS": return hb.Td(column: column, value: resultModel.ClassS);
                case "ClassT": return hb.Td(column: column, value: resultModel.ClassT);
                case "ClassU": return hb.Td(column: column, value: resultModel.ClassU);
                case "ClassV": return hb.Td(column: column, value: resultModel.ClassV);
                case "ClassW": return hb.Td(column: column, value: resultModel.ClassW);
                case "ClassX": return hb.Td(column: column, value: resultModel.ClassX);
                case "ClassY": return hb.Td(column: column, value: resultModel.ClassY);
                case "ClassZ": return hb.Td(column: column, value: resultModel.ClassZ);
                case "NumA": return hb.Td(column: column, value: resultModel.NumA);
                case "NumB": return hb.Td(column: column, value: resultModel.NumB);
                case "NumC": return hb.Td(column: column, value: resultModel.NumC);
                case "NumD": return hb.Td(column: column, value: resultModel.NumD);
                case "NumE": return hb.Td(column: column, value: resultModel.NumE);
                case "NumF": return hb.Td(column: column, value: resultModel.NumF);
                case "NumG": return hb.Td(column: column, value: resultModel.NumG);
                case "NumH": return hb.Td(column: column, value: resultModel.NumH);
                case "NumI": return hb.Td(column: column, value: resultModel.NumI);
                case "NumJ": return hb.Td(column: column, value: resultModel.NumJ);
                case "NumK": return hb.Td(column: column, value: resultModel.NumK);
                case "NumL": return hb.Td(column: column, value: resultModel.NumL);
                case "NumM": return hb.Td(column: column, value: resultModel.NumM);
                case "NumN": return hb.Td(column: column, value: resultModel.NumN);
                case "NumO": return hb.Td(column: column, value: resultModel.NumO);
                case "NumP": return hb.Td(column: column, value: resultModel.NumP);
                case "NumQ": return hb.Td(column: column, value: resultModel.NumQ);
                case "NumR": return hb.Td(column: column, value: resultModel.NumR);
                case "NumS": return hb.Td(column: column, value: resultModel.NumS);
                case "NumT": return hb.Td(column: column, value: resultModel.NumT);
                case "NumU": return hb.Td(column: column, value: resultModel.NumU);
                case "NumV": return hb.Td(column: column, value: resultModel.NumV);
                case "NumW": return hb.Td(column: column, value: resultModel.NumW);
                case "NumX": return hb.Td(column: column, value: resultModel.NumX);
                case "NumY": return hb.Td(column: column, value: resultModel.NumY);
                case "NumZ": return hb.Td(column: column, value: resultModel.NumZ);
                case "DateA": return hb.Td(column: column, value: resultModel.DateA);
                case "DateB": return hb.Td(column: column, value: resultModel.DateB);
                case "DateC": return hb.Td(column: column, value: resultModel.DateC);
                case "DateD": return hb.Td(column: column, value: resultModel.DateD);
                case "DateE": return hb.Td(column: column, value: resultModel.DateE);
                case "DateF": return hb.Td(column: column, value: resultModel.DateF);
                case "DateG": return hb.Td(column: column, value: resultModel.DateG);
                case "DateH": return hb.Td(column: column, value: resultModel.DateH);
                case "DateI": return hb.Td(column: column, value: resultModel.DateI);
                case "DateJ": return hb.Td(column: column, value: resultModel.DateJ);
                case "DateK": return hb.Td(column: column, value: resultModel.DateK);
                case "DateL": return hb.Td(column: column, value: resultModel.DateL);
                case "DateM": return hb.Td(column: column, value: resultModel.DateM);
                case "DateN": return hb.Td(column: column, value: resultModel.DateN);
                case "DateO": return hb.Td(column: column, value: resultModel.DateO);
                case "DateP": return hb.Td(column: column, value: resultModel.DateP);
                case "DateQ": return hb.Td(column: column, value: resultModel.DateQ);
                case "DateR": return hb.Td(column: column, value: resultModel.DateR);
                case "DateS": return hb.Td(column: column, value: resultModel.DateS);
                case "DateT": return hb.Td(column: column, value: resultModel.DateT);
                case "DateU": return hb.Td(column: column, value: resultModel.DateU);
                case "DateV": return hb.Td(column: column, value: resultModel.DateV);
                case "DateW": return hb.Td(column: column, value: resultModel.DateW);
                case "DateX": return hb.Td(column: column, value: resultModel.DateX);
                case "DateY": return hb.Td(column: column, value: resultModel.DateY);
                case "DateZ": return hb.Td(column: column, value: resultModel.DateZ);
                case "DescriptionA": return hb.Td(column: column, value: resultModel.DescriptionA);
                case "DescriptionB": return hb.Td(column: column, value: resultModel.DescriptionB);
                case "DescriptionC": return hb.Td(column: column, value: resultModel.DescriptionC);
                case "DescriptionD": return hb.Td(column: column, value: resultModel.DescriptionD);
                case "DescriptionE": return hb.Td(column: column, value: resultModel.DescriptionE);
                case "DescriptionF": return hb.Td(column: column, value: resultModel.DescriptionF);
                case "DescriptionG": return hb.Td(column: column, value: resultModel.DescriptionG);
                case "DescriptionH": return hb.Td(column: column, value: resultModel.DescriptionH);
                case "DescriptionI": return hb.Td(column: column, value: resultModel.DescriptionI);
                case "DescriptionJ": return hb.Td(column: column, value: resultModel.DescriptionJ);
                case "DescriptionK": return hb.Td(column: column, value: resultModel.DescriptionK);
                case "DescriptionL": return hb.Td(column: column, value: resultModel.DescriptionL);
                case "DescriptionM": return hb.Td(column: column, value: resultModel.DescriptionM);
                case "DescriptionN": return hb.Td(column: column, value: resultModel.DescriptionN);
                case "DescriptionO": return hb.Td(column: column, value: resultModel.DescriptionO);
                case "DescriptionP": return hb.Td(column: column, value: resultModel.DescriptionP);
                case "DescriptionQ": return hb.Td(column: column, value: resultModel.DescriptionQ);
                case "DescriptionR": return hb.Td(column: column, value: resultModel.DescriptionR);
                case "DescriptionS": return hb.Td(column: column, value: resultModel.DescriptionS);
                case "DescriptionT": return hb.Td(column: column, value: resultModel.DescriptionT);
                case "DescriptionU": return hb.Td(column: column, value: resultModel.DescriptionU);
                case "DescriptionV": return hb.Td(column: column, value: resultModel.DescriptionV);
                case "DescriptionW": return hb.Td(column: column, value: resultModel.DescriptionW);
                case "DescriptionX": return hb.Td(column: column, value: resultModel.DescriptionX);
                case "DescriptionY": return hb.Td(column: column, value: resultModel.DescriptionY);
                case "DescriptionZ": return hb.Td(column: column, value: resultModel.DescriptionZ);
                case "CheckA": return hb.Td(column: column, value: resultModel.CheckA);
                case "CheckB": return hb.Td(column: column, value: resultModel.CheckB);
                case "CheckC": return hb.Td(column: column, value: resultModel.CheckC);
                case "CheckD": return hb.Td(column: column, value: resultModel.CheckD);
                case "CheckE": return hb.Td(column: column, value: resultModel.CheckE);
                case "CheckF": return hb.Td(column: column, value: resultModel.CheckF);
                case "CheckG": return hb.Td(column: column, value: resultModel.CheckG);
                case "CheckH": return hb.Td(column: column, value: resultModel.CheckH);
                case "CheckI": return hb.Td(column: column, value: resultModel.CheckI);
                case "CheckJ": return hb.Td(column: column, value: resultModel.CheckJ);
                case "CheckK": return hb.Td(column: column, value: resultModel.CheckK);
                case "CheckL": return hb.Td(column: column, value: resultModel.CheckL);
                case "CheckM": return hb.Td(column: column, value: resultModel.CheckM);
                case "CheckN": return hb.Td(column: column, value: resultModel.CheckN);
                case "CheckO": return hb.Td(column: column, value: resultModel.CheckO);
                case "CheckP": return hb.Td(column: column, value: resultModel.CheckP);
                case "CheckQ": return hb.Td(column: column, value: resultModel.CheckQ);
                case "CheckR": return hb.Td(column: column, value: resultModel.CheckR);
                case "CheckS": return hb.Td(column: column, value: resultModel.CheckS);
                case "CheckT": return hb.Td(column: column, value: resultModel.CheckT);
                case "CheckU": return hb.Td(column: column, value: resultModel.CheckU);
                case "CheckV": return hb.Td(column: column, value: resultModel.CheckV);
                case "CheckW": return hb.Td(column: column, value: resultModel.CheckW);
                case "CheckX": return hb.Td(column: column, value: resultModel.CheckX);
                case "CheckY": return hb.Td(column: column, value: resultModel.CheckY);
                case "CheckZ": return hb.Td(column: column, value: resultModel.CheckZ);
                case "Comments": return hb.Td(column: column, value: resultModel.Comments);
                case "Creator": return hb.Td(column: column, value: resultModel.Creator);
                case "Updator": return hb.Td(column: column, value: resultModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: resultModel.CreatedTime);
                default: return hb;
            }
        }

        public static string EditorNew(SiteModel siteModel, long siteId)
        {
            return Editor(
                siteModel,
                new ResultModel(
                    siteModel.ResultsSiteSettings(),
                    siteModel.PermissionType,
                    methodType: BaseModel.MethodTypes.New)
                {
                    SiteId = siteId
                });
        }

        public static string Editor(SiteModel siteModel, long resultId, bool clearSessions)
        {
            var siteSettings = siteModel.ResultsSiteSettings();
            var resultModel = new ResultModel(
                siteSettings: siteSettings,
                permissionType: siteModel.PermissionType,
                resultId: resultId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            resultModel.SwitchTargets = ResultsUtility.GetSwitchTargets(
                siteSettings, resultModel.SiteId);
            return Editor(siteModel, resultModel);
        }

        public static string Editor(SiteModel siteModel, ResultModel resultModel)
        {
            var hb = new HtmlBuilder();
            resultModel.SiteSettings.SetLinks();
            return hb.Template(
                siteId: siteModel.SiteId,
                referenceId: "Results",
                title: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? siteModel.Title.DisplayValue + " - " + Displays.New()
                    : resultModel.Title.DisplayValue,
                permissionType: resultModel.PermissionType,
                verType: resultModel.VerType,
                methodType: resultModel.MethodType,
                allowAccess:
                    resultModel.PermissionType.CanRead() &&
                    resultModel.AccessStatus != Databases.AccessStatuses.NotFound,
                userStyle: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? resultModel.SiteSettings.NewStyle
                    : resultModel.SiteSettings.EditStyle,
                userScript: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? resultModel.SiteSettings.NewScript
                    : resultModel.SiteSettings.EditScript,
                action: () =>
                {
                    hb
                        .Editor(
                            siteSettings: resultModel.SiteSettings,
                            resultModel: resultModel,
                            siteModel: siteModel)
                        .Hidden(controlId: "TableName", value: "Results")
                        .Hidden(controlId: "Id", value: resultModel.ResultId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteModel siteModel,
            SiteSettings siteSettings,
            ResultModel resultModel)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id_Css("ResultForm", "main-form")
                        .Action(Navigations.ItemAction(resultModel.ResultId != 0 
                            ? resultModel.ResultId
                            : siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            id: resultModel.ResultId,
                            baseModel: resultModel,
                            tableName: "Results",
                            switchTargets: resultModel.SwitchTargets)
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: resultModel.Comments,
                                verType: resultModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(resultModel: resultModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: siteModel.PermissionType,
                                resultModel: resultModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: resultModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: siteModel.SiteId,
                                permissionType: siteModel.PermissionType,
                                verType: resultModel.VerType,
                                backUrl: Navigations.ItemIndex(siteSettings.SiteId),
                                referenceType: "items",
                                referenceId: resultModel.ResultId,
                                updateButton: true,
                                copyButton: true,
                                moveButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        resultModel: resultModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: resultModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Results_Timestamp",
                            css: "must-transport",
                            value: resultModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: resultModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Results", resultModel.ResultId, resultModel.Ver)
                .Dialog_Copy("items", resultModel.ResultId)
                .Dialog_Move("items", resultModel.ResultId)
                .Dialog_OutgoingMail()
                .EditorExtensions(resultModel: resultModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, ResultModel resultModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: resultModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            ResultModel resultModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.ColumnCollection
                    .Where(o => o.EditorVisible.ToBool())
                    .OrderBy(o => siteSettings.EditorColumnsOrder.IndexOf(o.ColumnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "ResultId": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ResultId.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "Ver": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Ver.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "Title": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Title.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "Body": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Body.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "Status": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Status.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "Manager": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Manager.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "Owner": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.Owner.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "ClassZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.ClassZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "NumZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.NumZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DateZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DateZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "DescriptionZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.DescriptionZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckA": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckA.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckB": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckB.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckC": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckC.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckD": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckD.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckE": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckE.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckF": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckF.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckG": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckG.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckH": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckH.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckI": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckI.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckJ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckJ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckK": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckK.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckL": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckL.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckM": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckM.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckN": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckN.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckO": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckO.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckP": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckP.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckQ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckQ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckR": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckR.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckS": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckS.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckT": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckT.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckU": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckU.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckV": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckV.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckW": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckW.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckX": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckX.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckY": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckY.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                            case "CheckZ": hb.Field(siteSettings, column, resultModel.MethodType, resultModel.CheckZ.ToControl(column, permissionType), column.ColumnPermissionType(permissionType)); break;
                        }
                    });
                hb
                    .VerUpCheckBox(resultModel)
                    .Div(id: "LinkCreations", css: "links", action: () => hb
                        .LinkCreations(
                            siteSettings: siteSettings,
                            linkId: resultModel.ResultId,
                            methodType: resultModel.MethodType))
                    .Div(id: "Links", css: "links", action: () => hb
                        .Links(linkId: resultModel.ResultId));
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            ResultModel resultModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            ResultModel resultModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<long> GetSwitchTargets(SiteSettings siteSettings, long siteId)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData(siteId);
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectResults(
                        column: Rds.ResultsColumn().ResultId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Results",
                            formData: formData,
                            where: Rds.ResultsWhere().SiteId(siteId)),
                        orderBy: GridSorters.Get(
                            formData, Rds.ResultsOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["ResultId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, ResultModel resultModel)
        {
            Forms.All().Keys.ForEach(key =>
            {
                switch (key)
                {
                    case "Results_NumA": responseCollection.Val("#" + key, resultModel.NumA.ToControl(resultModel.SiteSettings.AllColumn("NumA"), resultModel.PermissionType)); break;
                    case "Results_NumB": responseCollection.Val("#" + key, resultModel.NumB.ToControl(resultModel.SiteSettings.AllColumn("NumB"), resultModel.PermissionType)); break;
                    case "Results_NumC": responseCollection.Val("#" + key, resultModel.NumC.ToControl(resultModel.SiteSettings.AllColumn("NumC"), resultModel.PermissionType)); break;
                    case "Results_NumD": responseCollection.Val("#" + key, resultModel.NumD.ToControl(resultModel.SiteSettings.AllColumn("NumD"), resultModel.PermissionType)); break;
                    case "Results_NumE": responseCollection.Val("#" + key, resultModel.NumE.ToControl(resultModel.SiteSettings.AllColumn("NumE"), resultModel.PermissionType)); break;
                    case "Results_NumF": responseCollection.Val("#" + key, resultModel.NumF.ToControl(resultModel.SiteSettings.AllColumn("NumF"), resultModel.PermissionType)); break;
                    case "Results_NumG": responseCollection.Val("#" + key, resultModel.NumG.ToControl(resultModel.SiteSettings.AllColumn("NumG"), resultModel.PermissionType)); break;
                    case "Results_NumH": responseCollection.Val("#" + key, resultModel.NumH.ToControl(resultModel.SiteSettings.AllColumn("NumH"), resultModel.PermissionType)); break;
                    case "Results_NumI": responseCollection.Val("#" + key, resultModel.NumI.ToControl(resultModel.SiteSettings.AllColumn("NumI"), resultModel.PermissionType)); break;
                    case "Results_NumJ": responseCollection.Val("#" + key, resultModel.NumJ.ToControl(resultModel.SiteSettings.AllColumn("NumJ"), resultModel.PermissionType)); break;
                    case "Results_NumK": responseCollection.Val("#" + key, resultModel.NumK.ToControl(resultModel.SiteSettings.AllColumn("NumK"), resultModel.PermissionType)); break;
                    case "Results_NumL": responseCollection.Val("#" + key, resultModel.NumL.ToControl(resultModel.SiteSettings.AllColumn("NumL"), resultModel.PermissionType)); break;
                    case "Results_NumM": responseCollection.Val("#" + key, resultModel.NumM.ToControl(resultModel.SiteSettings.AllColumn("NumM"), resultModel.PermissionType)); break;
                    case "Results_NumN": responseCollection.Val("#" + key, resultModel.NumN.ToControl(resultModel.SiteSettings.AllColumn("NumN"), resultModel.PermissionType)); break;
                    case "Results_NumO": responseCollection.Val("#" + key, resultModel.NumO.ToControl(resultModel.SiteSettings.AllColumn("NumO"), resultModel.PermissionType)); break;
                    case "Results_NumP": responseCollection.Val("#" + key, resultModel.NumP.ToControl(resultModel.SiteSettings.AllColumn("NumP"), resultModel.PermissionType)); break;
                    case "Results_NumQ": responseCollection.Val("#" + key, resultModel.NumQ.ToControl(resultModel.SiteSettings.AllColumn("NumQ"), resultModel.PermissionType)); break;
                    case "Results_NumR": responseCollection.Val("#" + key, resultModel.NumR.ToControl(resultModel.SiteSettings.AllColumn("NumR"), resultModel.PermissionType)); break;
                    case "Results_NumS": responseCollection.Val("#" + key, resultModel.NumS.ToControl(resultModel.SiteSettings.AllColumn("NumS"), resultModel.PermissionType)); break;
                    case "Results_NumT": responseCollection.Val("#" + key, resultModel.NumT.ToControl(resultModel.SiteSettings.AllColumn("NumT"), resultModel.PermissionType)); break;
                    case "Results_NumU": responseCollection.Val("#" + key, resultModel.NumU.ToControl(resultModel.SiteSettings.AllColumn("NumU"), resultModel.PermissionType)); break;
                    case "Results_NumV": responseCollection.Val("#" + key, resultModel.NumV.ToControl(resultModel.SiteSettings.AllColumn("NumV"), resultModel.PermissionType)); break;
                    case "Results_NumW": responseCollection.Val("#" + key, resultModel.NumW.ToControl(resultModel.SiteSettings.AllColumn("NumW"), resultModel.PermissionType)); break;
                    case "Results_NumX": responseCollection.Val("#" + key, resultModel.NumX.ToControl(resultModel.SiteSettings.AllColumn("NumX"), resultModel.PermissionType)); break;
                    case "Results_NumY": responseCollection.Val("#" + key, resultModel.NumY.ToControl(resultModel.SiteSettings.AllColumn("NumY"), resultModel.PermissionType)); break;
                    case "Results_NumZ": responseCollection.Val("#" + key, resultModel.NumZ.ToControl(resultModel.SiteSettings.AllColumn("NumZ"), resultModel.PermissionType)); break;
                    default: break;
                }
            });
            return responseCollection;
        }

        public static ResponseCollection Formula(
            this ResponseCollection responseCollection, ResultModel resultModel)
        {
            resultModel.SiteSettings.FormulaHash?.Keys.ForEach(columnName =>
            {
                var column = resultModel.SiteSettings.AllColumn(columnName);
                switch (columnName)
                {
                    case "NumA": responseCollection.Val("#Results_NumA", resultModel.NumA.ToControl(column, resultModel.PermissionType)); break;
                    case "NumB": responseCollection.Val("#Results_NumB", resultModel.NumB.ToControl(column, resultModel.PermissionType)); break;
                    case "NumC": responseCollection.Val("#Results_NumC", resultModel.NumC.ToControl(column, resultModel.PermissionType)); break;
                    case "NumD": responseCollection.Val("#Results_NumD", resultModel.NumD.ToControl(column, resultModel.PermissionType)); break;
                    case "NumE": responseCollection.Val("#Results_NumE", resultModel.NumE.ToControl(column, resultModel.PermissionType)); break;
                    case "NumF": responseCollection.Val("#Results_NumF", resultModel.NumF.ToControl(column, resultModel.PermissionType)); break;
                    case "NumG": responseCollection.Val("#Results_NumG", resultModel.NumG.ToControl(column, resultModel.PermissionType)); break;
                    case "NumH": responseCollection.Val("#Results_NumH", resultModel.NumH.ToControl(column, resultModel.PermissionType)); break;
                    case "NumI": responseCollection.Val("#Results_NumI", resultModel.NumI.ToControl(column, resultModel.PermissionType)); break;
                    case "NumJ": responseCollection.Val("#Results_NumJ", resultModel.NumJ.ToControl(column, resultModel.PermissionType)); break;
                    case "NumK": responseCollection.Val("#Results_NumK", resultModel.NumK.ToControl(column, resultModel.PermissionType)); break;
                    case "NumL": responseCollection.Val("#Results_NumL", resultModel.NumL.ToControl(column, resultModel.PermissionType)); break;
                    case "NumM": responseCollection.Val("#Results_NumM", resultModel.NumM.ToControl(column, resultModel.PermissionType)); break;
                    case "NumN": responseCollection.Val("#Results_NumN", resultModel.NumN.ToControl(column, resultModel.PermissionType)); break;
                    case "NumO": responseCollection.Val("#Results_NumO", resultModel.NumO.ToControl(column, resultModel.PermissionType)); break;
                    case "NumP": responseCollection.Val("#Results_NumP", resultModel.NumP.ToControl(column, resultModel.PermissionType)); break;
                    case "NumQ": responseCollection.Val("#Results_NumQ", resultModel.NumQ.ToControl(column, resultModel.PermissionType)); break;
                    case "NumR": responseCollection.Val("#Results_NumR", resultModel.NumR.ToControl(column, resultModel.PermissionType)); break;
                    case "NumS": responseCollection.Val("#Results_NumS", resultModel.NumS.ToControl(column, resultModel.PermissionType)); break;
                    case "NumT": responseCollection.Val("#Results_NumT", resultModel.NumT.ToControl(column, resultModel.PermissionType)); break;
                    case "NumU": responseCollection.Val("#Results_NumU", resultModel.NumU.ToControl(column, resultModel.PermissionType)); break;
                    case "NumV": responseCollection.Val("#Results_NumV", resultModel.NumV.ToControl(column, resultModel.PermissionType)); break;
                    case "NumW": responseCollection.Val("#Results_NumW", resultModel.NumW.ToControl(column, resultModel.PermissionType)); break;
                    case "NumX": responseCollection.Val("#Results_NumX", resultModel.NumX.ToControl(column, resultModel.PermissionType)); break;
                    case "NumY": responseCollection.Val("#Results_NumY", resultModel.NumY.ToControl(column, resultModel.PermissionType)); break;
                    case "NumZ": responseCollection.Val("#Results_NumZ", resultModel.NumZ.ToControl(column, resultModel.PermissionType)); break;
                    default: break;
                }
            });
            return responseCollection;
        }

        public static string BulkMove(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var siteId = Forms.Long("Dialog_MoveTargets");
            if (Permissions.CanMove(siteSettings.SiteId, siteId))
            {
                var count = 0;
                if (Forms.Bool("GridCheckAll"))
                {
                    count = BulkMove(
                        siteId,
                        siteSettings,
                        GridItems("GridUnCheckedItems"),
                        negative: true);
                }
                else
                {
                    var checkedItems = GridItems("GridCheckedItems");
                    if (checkedItems.Count() > 0)
                    {
                        count = BulkMove(
                            siteId,
                            siteSettings,
                            checkedItems);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                return GridRows(
                    siteSettings,
                    permissionType,
                    clearCheck: true,
                    message: Messages.BulkMoved(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkMove(
            long siteId,
            SiteSettings siteSettings,
            IEnumerable<long> checkedItems,
            bool negative = false)
        {
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateResults(
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Results",
                            formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                            where: Rds.ResultsWhere()
                                .SiteId(siteSettings.SiteId)
                                .ResultId_In(
                                    value: checkedItems,
                                    negative: negative,
                                    _using: checkedItems.Count() > 0)),
                        param: Rds.ResultsParam().SiteId(siteId),
                        countRecord: true),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    where: Rds.ResultsWhere().SiteId(siteId)))
                            .SiteId(siteId, _operator: "<>"),
                        param: Rds.ItemsParam().SiteId(siteId))
                });
        }

        public static string BulkDelete(
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            if (permissionType.CanDelete())
            {
                var count = 0;
                if (Forms.Bool("GridCheckAll"))
                {
                    count = BulkDelete(
                        siteSettings,
                        GridItems("GridUnCheckedItems"),
                        negative: true);
                }
                else
                {
                    var checkedItems = GridItems("GridCheckedItems");
                    if (checkedItems.Count() > 0)
                    {
                        count = BulkDelete(
                            siteSettings,
                            checkedItems);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                return GridRows(
                    siteSettings,
                    permissionType,
                    clearCheck: true,
                    message: Messages.BulkDeleted(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkDelete(
            SiteSettings siteSettings,
            IEnumerable<long> checkedItems,
            bool negative = false)
        {
            var where = DataViewFilters.Get(
                siteSettings: siteSettings,
                tableName: "Results",
                formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                where: Rds.ResultsWhere()
                    .SiteId(siteSettings.SiteId)
                    .ResultId_In(
                        value: checkedItems,
                        negative: negative,
                        _using: checkedItems.Count() > 0));
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    where: where))),
                    Rds.DeleteResults(
                        where: where, 
                        countRecord: true)
                });
        }

        private static IEnumerable<long> GridItems(string name)
        {
            return Forms.Data(name)
                .Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .Distinct();
        }

        public static string Import(SiteModel siteModel)
        {
            if (!siteModel.PermissionType.CanCreate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var responseCollection = new ResponseCollection();
            Csv csv;
            try
            {
                csv = new Csv(Forms.File("Import"), Forms.Data("Encoding"));
            }
            catch
            {
                return Messages.ResponseFailedReadFile().ToJson();
            }
            if (csv != null && csv.Rows.Count() != 0)
            {
                var siteSettings = siteModel.ResultsSiteSettings();
                var columnHash = new Dictionary<int, Column>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = siteSettings.ColumnCollection
                        .Where(o => o.LabelText == data.Header)
                        .FirstOrDefault();
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var error = Imports.ColumnValidate(siteSettings, columnHash.Values
                    .Select(o => o.ColumnName));
                if (error != null) return error;
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.ResultsParam();
                    param.ResultId(raw: Def.Sql.Identity);
                    param.SiteId(siteModel.SiteId);
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            column.Value, data.Row[column.Key], siteModel.InheritPermission);
                        if (!param.Any(o => o.Name == column.Value.ColumnName))
                        {
                            switch (column.Value.ColumnName)
                            {
                                case "Title": param.Title(recordingData, _using: recordingData != null); break;
                                case "Body": param.Body(recordingData, _using: recordingData != null); break;
                                case "Status": param.Status(recordingData, _using: recordingData != null); break;
                                case "Manager": param.Manager(recordingData, _using: recordingData != null); break;
                                case "Owner": param.Owner(recordingData, _using: recordingData != null); break;
                                case "ClassA": param.ClassA(recordingData, _using: recordingData != null); break;
                                case "ClassB": param.ClassB(recordingData, _using: recordingData != null); break;
                                case "ClassC": param.ClassC(recordingData, _using: recordingData != null); break;
                                case "ClassD": param.ClassD(recordingData, _using: recordingData != null); break;
                                case "ClassE": param.ClassE(recordingData, _using: recordingData != null); break;
                                case "ClassF": param.ClassF(recordingData, _using: recordingData != null); break;
                                case "ClassG": param.ClassG(recordingData, _using: recordingData != null); break;
                                case "ClassH": param.ClassH(recordingData, _using: recordingData != null); break;
                                case "ClassI": param.ClassI(recordingData, _using: recordingData != null); break;
                                case "ClassJ": param.ClassJ(recordingData, _using: recordingData != null); break;
                                case "ClassK": param.ClassK(recordingData, _using: recordingData != null); break;
                                case "ClassL": param.ClassL(recordingData, _using: recordingData != null); break;
                                case "ClassM": param.ClassM(recordingData, _using: recordingData != null); break;
                                case "ClassN": param.ClassN(recordingData, _using: recordingData != null); break;
                                case "ClassO": param.ClassO(recordingData, _using: recordingData != null); break;
                                case "ClassP": param.ClassP(recordingData, _using: recordingData != null); break;
                                case "ClassQ": param.ClassQ(recordingData, _using: recordingData != null); break;
                                case "ClassR": param.ClassR(recordingData, _using: recordingData != null); break;
                                case "ClassS": param.ClassS(recordingData, _using: recordingData != null); break;
                                case "ClassT": param.ClassT(recordingData, _using: recordingData != null); break;
                                case "ClassU": param.ClassU(recordingData, _using: recordingData != null); break;
                                case "ClassV": param.ClassV(recordingData, _using: recordingData != null); break;
                                case "ClassW": param.ClassW(recordingData, _using: recordingData != null); break;
                                case "ClassX": param.ClassX(recordingData, _using: recordingData != null); break;
                                case "ClassY": param.ClassY(recordingData, _using: recordingData != null); break;
                                case "ClassZ": param.ClassZ(recordingData, _using: recordingData != null); break;
                                case "NumA": param.NumA(recordingData, _using: recordingData != null); break;
                                case "NumB": param.NumB(recordingData, _using: recordingData != null); break;
                                case "NumC": param.NumC(recordingData, _using: recordingData != null); break;
                                case "NumD": param.NumD(recordingData, _using: recordingData != null); break;
                                case "NumE": param.NumE(recordingData, _using: recordingData != null); break;
                                case "NumF": param.NumF(recordingData, _using: recordingData != null); break;
                                case "NumG": param.NumG(recordingData, _using: recordingData != null); break;
                                case "NumH": param.NumH(recordingData, _using: recordingData != null); break;
                                case "NumI": param.NumI(recordingData, _using: recordingData != null); break;
                                case "NumJ": param.NumJ(recordingData, _using: recordingData != null); break;
                                case "NumK": param.NumK(recordingData, _using: recordingData != null); break;
                                case "NumL": param.NumL(recordingData, _using: recordingData != null); break;
                                case "NumM": param.NumM(recordingData, _using: recordingData != null); break;
                                case "NumN": param.NumN(recordingData, _using: recordingData != null); break;
                                case "NumO": param.NumO(recordingData, _using: recordingData != null); break;
                                case "NumP": param.NumP(recordingData, _using: recordingData != null); break;
                                case "NumQ": param.NumQ(recordingData, _using: recordingData != null); break;
                                case "NumR": param.NumR(recordingData, _using: recordingData != null); break;
                                case "NumS": param.NumS(recordingData, _using: recordingData != null); break;
                                case "NumT": param.NumT(recordingData, _using: recordingData != null); break;
                                case "NumU": param.NumU(recordingData, _using: recordingData != null); break;
                                case "NumV": param.NumV(recordingData, _using: recordingData != null); break;
                                case "NumW": param.NumW(recordingData, _using: recordingData != null); break;
                                case "NumX": param.NumX(recordingData, _using: recordingData != null); break;
                                case "NumY": param.NumY(recordingData, _using: recordingData != null); break;
                                case "NumZ": param.NumZ(recordingData, _using: recordingData != null); break;
                                case "DateA": param.DateA(recordingData, _using: recordingData != null); break;
                                case "DateB": param.DateB(recordingData, _using: recordingData != null); break;
                                case "DateC": param.DateC(recordingData, _using: recordingData != null); break;
                                case "DateD": param.DateD(recordingData, _using: recordingData != null); break;
                                case "DateE": param.DateE(recordingData, _using: recordingData != null); break;
                                case "DateF": param.DateF(recordingData, _using: recordingData != null); break;
                                case "DateG": param.DateG(recordingData, _using: recordingData != null); break;
                                case "DateH": param.DateH(recordingData, _using: recordingData != null); break;
                                case "DateI": param.DateI(recordingData, _using: recordingData != null); break;
                                case "DateJ": param.DateJ(recordingData, _using: recordingData != null); break;
                                case "DateK": param.DateK(recordingData, _using: recordingData != null); break;
                                case "DateL": param.DateL(recordingData, _using: recordingData != null); break;
                                case "DateM": param.DateM(recordingData, _using: recordingData != null); break;
                                case "DateN": param.DateN(recordingData, _using: recordingData != null); break;
                                case "DateO": param.DateO(recordingData, _using: recordingData != null); break;
                                case "DateP": param.DateP(recordingData, _using: recordingData != null); break;
                                case "DateQ": param.DateQ(recordingData, _using: recordingData != null); break;
                                case "DateR": param.DateR(recordingData, _using: recordingData != null); break;
                                case "DateS": param.DateS(recordingData, _using: recordingData != null); break;
                                case "DateT": param.DateT(recordingData, _using: recordingData != null); break;
                                case "DateU": param.DateU(recordingData, _using: recordingData != null); break;
                                case "DateV": param.DateV(recordingData, _using: recordingData != null); break;
                                case "DateW": param.DateW(recordingData, _using: recordingData != null); break;
                                case "DateX": param.DateX(recordingData, _using: recordingData != null); break;
                                case "DateY": param.DateY(recordingData, _using: recordingData != null); break;
                                case "DateZ": param.DateZ(recordingData, _using: recordingData != null); break;
                                case "DescriptionA": param.DescriptionA(recordingData, _using: recordingData != null); break;
                                case "DescriptionB": param.DescriptionB(recordingData, _using: recordingData != null); break;
                                case "DescriptionC": param.DescriptionC(recordingData, _using: recordingData != null); break;
                                case "DescriptionD": param.DescriptionD(recordingData, _using: recordingData != null); break;
                                case "DescriptionE": param.DescriptionE(recordingData, _using: recordingData != null); break;
                                case "DescriptionF": param.DescriptionF(recordingData, _using: recordingData != null); break;
                                case "DescriptionG": param.DescriptionG(recordingData, _using: recordingData != null); break;
                                case "DescriptionH": param.DescriptionH(recordingData, _using: recordingData != null); break;
                                case "DescriptionI": param.DescriptionI(recordingData, _using: recordingData != null); break;
                                case "DescriptionJ": param.DescriptionJ(recordingData, _using: recordingData != null); break;
                                case "DescriptionK": param.DescriptionK(recordingData, _using: recordingData != null); break;
                                case "DescriptionL": param.DescriptionL(recordingData, _using: recordingData != null); break;
                                case "DescriptionM": param.DescriptionM(recordingData, _using: recordingData != null); break;
                                case "DescriptionN": param.DescriptionN(recordingData, _using: recordingData != null); break;
                                case "DescriptionO": param.DescriptionO(recordingData, _using: recordingData != null); break;
                                case "DescriptionP": param.DescriptionP(recordingData, _using: recordingData != null); break;
                                case "DescriptionQ": param.DescriptionQ(recordingData, _using: recordingData != null); break;
                                case "DescriptionR": param.DescriptionR(recordingData, _using: recordingData != null); break;
                                case "DescriptionS": param.DescriptionS(recordingData, _using: recordingData != null); break;
                                case "DescriptionT": param.DescriptionT(recordingData, _using: recordingData != null); break;
                                case "DescriptionU": param.DescriptionU(recordingData, _using: recordingData != null); break;
                                case "DescriptionV": param.DescriptionV(recordingData, _using: recordingData != null); break;
                                case "DescriptionW": param.DescriptionW(recordingData, _using: recordingData != null); break;
                                case "DescriptionX": param.DescriptionX(recordingData, _using: recordingData != null); break;
                                case "DescriptionY": param.DescriptionY(recordingData, _using: recordingData != null); break;
                                case "DescriptionZ": param.DescriptionZ(recordingData, _using: recordingData != null); break;
                                case "CheckA": param.CheckA(recordingData, _using: recordingData != null); break;
                                case "CheckB": param.CheckB(recordingData, _using: recordingData != null); break;
                                case "CheckC": param.CheckC(recordingData, _using: recordingData != null); break;
                                case "CheckD": param.CheckD(recordingData, _using: recordingData != null); break;
                                case "CheckE": param.CheckE(recordingData, _using: recordingData != null); break;
                                case "CheckF": param.CheckF(recordingData, _using: recordingData != null); break;
                                case "CheckG": param.CheckG(recordingData, _using: recordingData != null); break;
                                case "CheckH": param.CheckH(recordingData, _using: recordingData != null); break;
                                case "CheckI": param.CheckI(recordingData, _using: recordingData != null); break;
                                case "CheckJ": param.CheckJ(recordingData, _using: recordingData != null); break;
                                case "CheckK": param.CheckK(recordingData, _using: recordingData != null); break;
                                case "CheckL": param.CheckL(recordingData, _using: recordingData != null); break;
                                case "CheckM": param.CheckM(recordingData, _using: recordingData != null); break;
                                case "CheckN": param.CheckN(recordingData, _using: recordingData != null); break;
                                case "CheckO": param.CheckO(recordingData, _using: recordingData != null); break;
                                case "CheckP": param.CheckP(recordingData, _using: recordingData != null); break;
                                case "CheckQ": param.CheckQ(recordingData, _using: recordingData != null); break;
                                case "CheckR": param.CheckR(recordingData, _using: recordingData != null); break;
                                case "CheckS": param.CheckS(recordingData, _using: recordingData != null); break;
                                case "CheckT": param.CheckT(recordingData, _using: recordingData != null); break;
                                case "CheckU": param.CheckU(recordingData, _using: recordingData != null); break;
                                case "CheckV": param.CheckV(recordingData, _using: recordingData != null); break;
                                case "CheckW": param.CheckW(recordingData, _using: recordingData != null); break;
                                case "CheckX": param.CheckX(recordingData, _using: recordingData != null); break;
                                case "CheckY": param.CheckY(recordingData, _using: recordingData != null); break;
                                case "CheckZ": param.CheckZ(recordingData, _using: recordingData != null); break;
                                case "Comments": param.Comments(recordingData, _using: recordingData != null); break;
                            }
                        }
                    });
                    paramHash.Add(data.Index, param);
                });
                paramHash.Values.ForEach(param =>
                    new ResultModel(siteSettings, siteModel.PermissionType)
                    {
                        SiteId = siteModel.SiteId,
                        Title = new Title(param.FirstOrDefault(o =>
                            o.Name == "Title").Value.ToString()),
                        SiteSettings = siteSettings
                    }.Create(param: param));
                return GridRows(siteSettings, siteModel.PermissionType, responseCollection
                    .WindowScrollTop()
                    .CloseDialog("#Dialog_ImportSettings")
                    .Message(Messages.Imported(csv.Rows.Count().ToString())));
            }
            else
            {
                return Messages.ResponseFileNotFound().ToJson();
            }
        }

        private static object ImportRecordingData(
            Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(value, inheritPermission);
            return recordingData;
        }

        public static ResponseFile Export(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            SiteModel siteModel)
        {
            siteModel.SiteSettings.SetLinks();
            var formData = DataViewFilters.SessionFormData(siteModel.SiteId);
            var resultCollection = new ResultCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                where: DataViewFilters.Get(
                    siteSettings: siteModel.SiteSettings,
                    tableName: "Results",
                    formData: formData,
                    where: Rds.ResultsWhere().SiteId(siteModel.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.ResultsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)));
            var csv = new System.Text.StringBuilder();
            var exportColumns = (Sessions.PageSession(
                siteModel.Id, 
                "ExportSettings_ExportColumns").ToString().Deserialize<ExportColumns>());
            var columnHash = exportColumns.ColumnHash(siteModel.ResultsSiteSettings());
            if (Sessions.PageSession(siteModel.Id, "ExportSettings_AddHeader").ToBool())
            {
                var header = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn => header.Add(
                        "\"" + columnHash[exportColumn.Key].LabelText + "\""));
                csv.Append(header.Join(","), "\n");
            }
            resultCollection.ForEach(resultModel =>
            {
                var row = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            resultModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key])));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(
            ResultModel resultModel, string columnName, Column column)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId": value = resultModel.SiteId.ToExport(column); break;
                case "UpdatedTime": value = resultModel.UpdatedTime.ToExport(column); break;
                case "ResultId": value = resultModel.ResultId.ToExport(column); break;
                case "Ver": value = resultModel.Ver.ToExport(column); break;
                case "Title": value = resultModel.Title.ToExport(column); break;
                case "Body": value = resultModel.Body.ToExport(column); break;
                case "TitleBody": value = resultModel.TitleBody.ToExport(column); break;
                case "Status": value = resultModel.Status.ToExport(column); break;
                case "Manager": value = resultModel.Manager.ToExport(column); break;
                case "Owner": value = resultModel.Owner.ToExport(column); break;
                case "ClassA": value = resultModel.ClassA.ToExport(column); break;
                case "ClassB": value = resultModel.ClassB.ToExport(column); break;
                case "ClassC": value = resultModel.ClassC.ToExport(column); break;
                case "ClassD": value = resultModel.ClassD.ToExport(column); break;
                case "ClassE": value = resultModel.ClassE.ToExport(column); break;
                case "ClassF": value = resultModel.ClassF.ToExport(column); break;
                case "ClassG": value = resultModel.ClassG.ToExport(column); break;
                case "ClassH": value = resultModel.ClassH.ToExport(column); break;
                case "ClassI": value = resultModel.ClassI.ToExport(column); break;
                case "ClassJ": value = resultModel.ClassJ.ToExport(column); break;
                case "ClassK": value = resultModel.ClassK.ToExport(column); break;
                case "ClassL": value = resultModel.ClassL.ToExport(column); break;
                case "ClassM": value = resultModel.ClassM.ToExport(column); break;
                case "ClassN": value = resultModel.ClassN.ToExport(column); break;
                case "ClassO": value = resultModel.ClassO.ToExport(column); break;
                case "ClassP": value = resultModel.ClassP.ToExport(column); break;
                case "ClassQ": value = resultModel.ClassQ.ToExport(column); break;
                case "ClassR": value = resultModel.ClassR.ToExport(column); break;
                case "ClassS": value = resultModel.ClassS.ToExport(column); break;
                case "ClassT": value = resultModel.ClassT.ToExport(column); break;
                case "ClassU": value = resultModel.ClassU.ToExport(column); break;
                case "ClassV": value = resultModel.ClassV.ToExport(column); break;
                case "ClassW": value = resultModel.ClassW.ToExport(column); break;
                case "ClassX": value = resultModel.ClassX.ToExport(column); break;
                case "ClassY": value = resultModel.ClassY.ToExport(column); break;
                case "ClassZ": value = resultModel.ClassZ.ToExport(column); break;
                case "NumA": value = resultModel.NumA.ToExport(column); break;
                case "NumB": value = resultModel.NumB.ToExport(column); break;
                case "NumC": value = resultModel.NumC.ToExport(column); break;
                case "NumD": value = resultModel.NumD.ToExport(column); break;
                case "NumE": value = resultModel.NumE.ToExport(column); break;
                case "NumF": value = resultModel.NumF.ToExport(column); break;
                case "NumG": value = resultModel.NumG.ToExport(column); break;
                case "NumH": value = resultModel.NumH.ToExport(column); break;
                case "NumI": value = resultModel.NumI.ToExport(column); break;
                case "NumJ": value = resultModel.NumJ.ToExport(column); break;
                case "NumK": value = resultModel.NumK.ToExport(column); break;
                case "NumL": value = resultModel.NumL.ToExport(column); break;
                case "NumM": value = resultModel.NumM.ToExport(column); break;
                case "NumN": value = resultModel.NumN.ToExport(column); break;
                case "NumO": value = resultModel.NumO.ToExport(column); break;
                case "NumP": value = resultModel.NumP.ToExport(column); break;
                case "NumQ": value = resultModel.NumQ.ToExport(column); break;
                case "NumR": value = resultModel.NumR.ToExport(column); break;
                case "NumS": value = resultModel.NumS.ToExport(column); break;
                case "NumT": value = resultModel.NumT.ToExport(column); break;
                case "NumU": value = resultModel.NumU.ToExport(column); break;
                case "NumV": value = resultModel.NumV.ToExport(column); break;
                case "NumW": value = resultModel.NumW.ToExport(column); break;
                case "NumX": value = resultModel.NumX.ToExport(column); break;
                case "NumY": value = resultModel.NumY.ToExport(column); break;
                case "NumZ": value = resultModel.NumZ.ToExport(column); break;
                case "DateA": value = resultModel.DateA.ToExport(column); break;
                case "DateB": value = resultModel.DateB.ToExport(column); break;
                case "DateC": value = resultModel.DateC.ToExport(column); break;
                case "DateD": value = resultModel.DateD.ToExport(column); break;
                case "DateE": value = resultModel.DateE.ToExport(column); break;
                case "DateF": value = resultModel.DateF.ToExport(column); break;
                case "DateG": value = resultModel.DateG.ToExport(column); break;
                case "DateH": value = resultModel.DateH.ToExport(column); break;
                case "DateI": value = resultModel.DateI.ToExport(column); break;
                case "DateJ": value = resultModel.DateJ.ToExport(column); break;
                case "DateK": value = resultModel.DateK.ToExport(column); break;
                case "DateL": value = resultModel.DateL.ToExport(column); break;
                case "DateM": value = resultModel.DateM.ToExport(column); break;
                case "DateN": value = resultModel.DateN.ToExport(column); break;
                case "DateO": value = resultModel.DateO.ToExport(column); break;
                case "DateP": value = resultModel.DateP.ToExport(column); break;
                case "DateQ": value = resultModel.DateQ.ToExport(column); break;
                case "DateR": value = resultModel.DateR.ToExport(column); break;
                case "DateS": value = resultModel.DateS.ToExport(column); break;
                case "DateT": value = resultModel.DateT.ToExport(column); break;
                case "DateU": value = resultModel.DateU.ToExport(column); break;
                case "DateV": value = resultModel.DateV.ToExport(column); break;
                case "DateW": value = resultModel.DateW.ToExport(column); break;
                case "DateX": value = resultModel.DateX.ToExport(column); break;
                case "DateY": value = resultModel.DateY.ToExport(column); break;
                case "DateZ": value = resultModel.DateZ.ToExport(column); break;
                case "DescriptionA": value = resultModel.DescriptionA.ToExport(column); break;
                case "DescriptionB": value = resultModel.DescriptionB.ToExport(column); break;
                case "DescriptionC": value = resultModel.DescriptionC.ToExport(column); break;
                case "DescriptionD": value = resultModel.DescriptionD.ToExport(column); break;
                case "DescriptionE": value = resultModel.DescriptionE.ToExport(column); break;
                case "DescriptionF": value = resultModel.DescriptionF.ToExport(column); break;
                case "DescriptionG": value = resultModel.DescriptionG.ToExport(column); break;
                case "DescriptionH": value = resultModel.DescriptionH.ToExport(column); break;
                case "DescriptionI": value = resultModel.DescriptionI.ToExport(column); break;
                case "DescriptionJ": value = resultModel.DescriptionJ.ToExport(column); break;
                case "DescriptionK": value = resultModel.DescriptionK.ToExport(column); break;
                case "DescriptionL": value = resultModel.DescriptionL.ToExport(column); break;
                case "DescriptionM": value = resultModel.DescriptionM.ToExport(column); break;
                case "DescriptionN": value = resultModel.DescriptionN.ToExport(column); break;
                case "DescriptionO": value = resultModel.DescriptionO.ToExport(column); break;
                case "DescriptionP": value = resultModel.DescriptionP.ToExport(column); break;
                case "DescriptionQ": value = resultModel.DescriptionQ.ToExport(column); break;
                case "DescriptionR": value = resultModel.DescriptionR.ToExport(column); break;
                case "DescriptionS": value = resultModel.DescriptionS.ToExport(column); break;
                case "DescriptionT": value = resultModel.DescriptionT.ToExport(column); break;
                case "DescriptionU": value = resultModel.DescriptionU.ToExport(column); break;
                case "DescriptionV": value = resultModel.DescriptionV.ToExport(column); break;
                case "DescriptionW": value = resultModel.DescriptionW.ToExport(column); break;
                case "DescriptionX": value = resultModel.DescriptionX.ToExport(column); break;
                case "DescriptionY": value = resultModel.DescriptionY.ToExport(column); break;
                case "DescriptionZ": value = resultModel.DescriptionZ.ToExport(column); break;
                case "CheckA": value = resultModel.CheckA.ToExport(column); break;
                case "CheckB": value = resultModel.CheckB.ToExport(column); break;
                case "CheckC": value = resultModel.CheckC.ToExport(column); break;
                case "CheckD": value = resultModel.CheckD.ToExport(column); break;
                case "CheckE": value = resultModel.CheckE.ToExport(column); break;
                case "CheckF": value = resultModel.CheckF.ToExport(column); break;
                case "CheckG": value = resultModel.CheckG.ToExport(column); break;
                case "CheckH": value = resultModel.CheckH.ToExport(column); break;
                case "CheckI": value = resultModel.CheckI.ToExport(column); break;
                case "CheckJ": value = resultModel.CheckJ.ToExport(column); break;
                case "CheckK": value = resultModel.CheckK.ToExport(column); break;
                case "CheckL": value = resultModel.CheckL.ToExport(column); break;
                case "CheckM": value = resultModel.CheckM.ToExport(column); break;
                case "CheckN": value = resultModel.CheckN.ToExport(column); break;
                case "CheckO": value = resultModel.CheckO.ToExport(column); break;
                case "CheckP": value = resultModel.CheckP.ToExport(column); break;
                case "CheckQ": value = resultModel.CheckQ.ToExport(column); break;
                case "CheckR": value = resultModel.CheckR.ToExport(column); break;
                case "CheckS": value = resultModel.CheckS.ToExport(column); break;
                case "CheckT": value = resultModel.CheckT.ToExport(column); break;
                case "CheckU": value = resultModel.CheckU.ToExport(column); break;
                case "CheckV": value = resultModel.CheckV.ToExport(column); break;
                case "CheckW": value = resultModel.CheckW.ToExport(column); break;
                case "CheckX": value = resultModel.CheckX.ToExport(column); break;
                case "CheckY": value = resultModel.CheckY.ToExport(column); break;
                case "CheckZ": value = resultModel.CheckZ.ToExport(column); break;
                case "Comments": value = resultModel.Comments.ToExport(column); break;
                case "Creator": value = resultModel.Creator.ToExport(column); break;
                case "Updator": value = resultModel.Updator.ToExport(column); break;
                case "CreatedTime": value = resultModel.CreatedTime.ToExport(column); break;
                default: return string.Empty;
            }
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, ResultModel resultModel)
        {
            var displayValue = siteSettings.ColumnCollection
                .Where(o => o.TitleVisible.ToBool())
                .OrderBy(o => siteSettings.TitleColumnsOrder.IndexOf(o.ColumnName))
                .Select(column => TitleDisplayValue(column, resultModel))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, ResultModel resultModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(resultModel.Title.Value).Text()
                    : resultModel.Title.Value;
                case "ClassA": return column.HasChoices()
                    ? column.Choice(resultModel.ClassA).Text()
                    : resultModel.ClassA;
                case "ClassB": return column.HasChoices()
                    ? column.Choice(resultModel.ClassB).Text()
                    : resultModel.ClassB;
                case "ClassC": return column.HasChoices()
                    ? column.Choice(resultModel.ClassC).Text()
                    : resultModel.ClassC;
                case "ClassD": return column.HasChoices()
                    ? column.Choice(resultModel.ClassD).Text()
                    : resultModel.ClassD;
                case "ClassE": return column.HasChoices()
                    ? column.Choice(resultModel.ClassE).Text()
                    : resultModel.ClassE;
                case "ClassF": return column.HasChoices()
                    ? column.Choice(resultModel.ClassF).Text()
                    : resultModel.ClassF;
                case "ClassG": return column.HasChoices()
                    ? column.Choice(resultModel.ClassG).Text()
                    : resultModel.ClassG;
                case "ClassH": return column.HasChoices()
                    ? column.Choice(resultModel.ClassH).Text()
                    : resultModel.ClassH;
                case "ClassI": return column.HasChoices()
                    ? column.Choice(resultModel.ClassI).Text()
                    : resultModel.ClassI;
                case "ClassJ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassJ).Text()
                    : resultModel.ClassJ;
                case "ClassK": return column.HasChoices()
                    ? column.Choice(resultModel.ClassK).Text()
                    : resultModel.ClassK;
                case "ClassL": return column.HasChoices()
                    ? column.Choice(resultModel.ClassL).Text()
                    : resultModel.ClassL;
                case "ClassM": return column.HasChoices()
                    ? column.Choice(resultModel.ClassM).Text()
                    : resultModel.ClassM;
                case "ClassN": return column.HasChoices()
                    ? column.Choice(resultModel.ClassN).Text()
                    : resultModel.ClassN;
                case "ClassO": return column.HasChoices()
                    ? column.Choice(resultModel.ClassO).Text()
                    : resultModel.ClassO;
                case "ClassP": return column.HasChoices()
                    ? column.Choice(resultModel.ClassP).Text()
                    : resultModel.ClassP;
                case "ClassQ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassQ).Text()
                    : resultModel.ClassQ;
                case "ClassR": return column.HasChoices()
                    ? column.Choice(resultModel.ClassR).Text()
                    : resultModel.ClassR;
                case "ClassS": return column.HasChoices()
                    ? column.Choice(resultModel.ClassS).Text()
                    : resultModel.ClassS;
                case "ClassT": return column.HasChoices()
                    ? column.Choice(resultModel.ClassT).Text()
                    : resultModel.ClassT;
                case "ClassU": return column.HasChoices()
                    ? column.Choice(resultModel.ClassU).Text()
                    : resultModel.ClassU;
                case "ClassV": return column.HasChoices()
                    ? column.Choice(resultModel.ClassV).Text()
                    : resultModel.ClassV;
                case "ClassW": return column.HasChoices()
                    ? column.Choice(resultModel.ClassW).Text()
                    : resultModel.ClassW;
                case "ClassX": return column.HasChoices()
                    ? column.Choice(resultModel.ClassX).Text()
                    : resultModel.ClassX;
                case "ClassY": return column.HasChoices()
                    ? column.Choice(resultModel.ClassY).Text()
                    : resultModel.ClassY;
                case "ClassZ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassZ).Text()
                    : resultModel.ClassZ;
                default: return string.Empty;
            }
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, DataRow dataRow)
        {
            var displayValue = siteSettings.ColumnCollection
                .Where(o => o.TitleVisible.ToBool())
                .OrderBy(o => siteSettings.TitleColumnsOrder.IndexOf(o.ColumnName))
                .Select(column => TitleDisplayValue(column, dataRow))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, DataRow dataRow)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(dataRow["Title"].ToString()).Text()
                    : dataRow["Title"].ToString();
                case "ClassA": return column.HasChoices()
                    ? column.Choice(dataRow["ClassA"].ToString()).Text()
                    : dataRow["ClassA"].ToString();
                case "ClassB": return column.HasChoices()
                    ? column.Choice(dataRow["ClassB"].ToString()).Text()
                    : dataRow["ClassB"].ToString();
                case "ClassC": return column.HasChoices()
                    ? column.Choice(dataRow["ClassC"].ToString()).Text()
                    : dataRow["ClassC"].ToString();
                case "ClassD": return column.HasChoices()
                    ? column.Choice(dataRow["ClassD"].ToString()).Text()
                    : dataRow["ClassD"].ToString();
                case "ClassE": return column.HasChoices()
                    ? column.Choice(dataRow["ClassE"].ToString()).Text()
                    : dataRow["ClassE"].ToString();
                case "ClassF": return column.HasChoices()
                    ? column.Choice(dataRow["ClassF"].ToString()).Text()
                    : dataRow["ClassF"].ToString();
                case "ClassG": return column.HasChoices()
                    ? column.Choice(dataRow["ClassG"].ToString()).Text()
                    : dataRow["ClassG"].ToString();
                case "ClassH": return column.HasChoices()
                    ? column.Choice(dataRow["ClassH"].ToString()).Text()
                    : dataRow["ClassH"].ToString();
                case "ClassI": return column.HasChoices()
                    ? column.Choice(dataRow["ClassI"].ToString()).Text()
                    : dataRow["ClassI"].ToString();
                case "ClassJ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassJ"].ToString()).Text()
                    : dataRow["ClassJ"].ToString();
                case "ClassK": return column.HasChoices()
                    ? column.Choice(dataRow["ClassK"].ToString()).Text()
                    : dataRow["ClassK"].ToString();
                case "ClassL": return column.HasChoices()
                    ? column.Choice(dataRow["ClassL"].ToString()).Text()
                    : dataRow["ClassL"].ToString();
                case "ClassM": return column.HasChoices()
                    ? column.Choice(dataRow["ClassM"].ToString()).Text()
                    : dataRow["ClassM"].ToString();
                case "ClassN": return column.HasChoices()
                    ? column.Choice(dataRow["ClassN"].ToString()).Text()
                    : dataRow["ClassN"].ToString();
                case "ClassO": return column.HasChoices()
                    ? column.Choice(dataRow["ClassO"].ToString()).Text()
                    : dataRow["ClassO"].ToString();
                case "ClassP": return column.HasChoices()
                    ? column.Choice(dataRow["ClassP"].ToString()).Text()
                    : dataRow["ClassP"].ToString();
                case "ClassQ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassQ"].ToString()).Text()
                    : dataRow["ClassQ"].ToString();
                case "ClassR": return column.HasChoices()
                    ? column.Choice(dataRow["ClassR"].ToString()).Text()
                    : dataRow["ClassR"].ToString();
                case "ClassS": return column.HasChoices()
                    ? column.Choice(dataRow["ClassS"].ToString()).Text()
                    : dataRow["ClassS"].ToString();
                case "ClassT": return column.HasChoices()
                    ? column.Choice(dataRow["ClassT"].ToString()).Text()
                    : dataRow["ClassT"].ToString();
                case "ClassU": return column.HasChoices()
                    ? column.Choice(dataRow["ClassU"].ToString()).Text()
                    : dataRow["ClassU"].ToString();
                case "ClassV": return column.HasChoices()
                    ? column.Choice(dataRow["ClassV"].ToString()).Text()
                    : dataRow["ClassV"].ToString();
                case "ClassW": return column.HasChoices()
                    ? column.Choice(dataRow["ClassW"].ToString()).Text()
                    : dataRow["ClassW"].ToString();
                case "ClassX": return column.HasChoices()
                    ? column.Choice(dataRow["ClassX"].ToString()).Text()
                    : dataRow["ClassX"].ToString();
                case "ClassY": return column.HasChoices()
                    ? column.Choice(dataRow["ClassY"].ToString()).Text()
                    : dataRow["ClassY"].ToString();
                case "ClassZ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassZ"].ToString()).Text()
                    : dataRow["ClassZ"].ToString();
                default: return string.Empty;
            }
        }

        public static string UpdateByKamban(SiteModel siteModel)
        {
            var siteSettings = siteModel.ResultsSiteSettings();
            var resultModel = new ResultModel(
                siteSettings,
                siteModel.PermissionType,
                Forms.Long("KambanId"),
                setByForm: true);
            resultModel.VerUp = Versions.MustVerUp(resultModel);
            resultModel.Update();
            return KambanResponse(siteSettings, siteModel.PermissionType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TimeSeriesResponse(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            var bodyOnly = Forms.Data("ControlId").StartsWith("TimeSeries");
            return new ResponseCollection()
                .Html(
                    !bodyOnly ? "#DataViewContainer" : "#TimeSeriesChart",
                    new HtmlBuilder().TimeSeries(
                        siteSettings: siteSettings,
                        permissionType: permissionType,
                        formData: formData,
                        bodyOnly: bodyOnly))
                .Html(
                    "#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: resultCollection.Aggregations,
                    container: false))
                .Func("drawTimeSeries")
                .WindowScrollTop().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TimeSeries(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            bool bodyOnly)
        {
            var groupByColumn = formData.Keys.Contains("TimeSeriesGroupByColumn")
                ? formData["TimeSeriesGroupByColumn"].Value
                : "Owner";
            var aggregateType = formData.Keys.Contains("TimeSeriesAggregateType")
                ? formData["TimeSeriesAggregateType"].Value
                : "Count";
            var valueColumn = formData.Keys.Contains("TimeSeriesValueColumn")
                ? formData["TimeSeriesValueColumn"].Value
                : "NumA";
            var dataRows = TimeSeriesDataRows(
                siteSettings: siteSettings,
                formData: formData,
                groupByColumn: groupByColumn,
                valueColumn: valueColumn);
            return !bodyOnly
                ? hb.TimeSeries(
                    siteSettings: siteSettings,
                    groupByColumn: groupByColumn,
                    aggregateType: aggregateType,
                    valueColumn: valueColumn,
                    permissionType: permissionType,
                    dataRows: dataRows)
                : hb.TimeSeriesChart(
                    siteSettings: siteSettings,
                    groupByColumn: groupByColumn,
                    aggregateType: aggregateType,
                    valueColumn: valueColumn,
                    dataRows: dataRows);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> TimeSeriesDataRows(
            SiteSettings siteSettings, FormData formData, string groupByColumn, string valueColumn)
        {
            return groupByColumn != string.Empty && valueColumn != string.Empty
                ? Rds.ExecuteTable(statements:
                    Rds.SelectResults(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.ResultsColumn()
                            .ResultId(_as: "Id")
                            .Ver()
                            .UpdatedTime()
                            .ResultsColumn(groupByColumn, _as: "Index")
                            .ResultsColumn(valueColumn, _as: "Value"),
                        where: DataViewFilters.Get(
                            siteSettings,
                            "Results",
                            formData,
                            Rds.ResultsWhere().SiteId(siteSettings.SiteId))))
                                .AsEnumerable()
                : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string KambanResponse(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var resultCollection = ResultCollection(siteSettings, permissionType, formData);
            var bodyOnly = Forms.Data("ControlId").StartsWith("Kamban");
            return new ResponseCollection()
                .Html(
                    !bodyOnly ? "#DataViewContainer" : "#KambanChart",
                    new HtmlBuilder().Kamban(
                        siteSettings: siteSettings,
                        permissionType: permissionType,
                        formData: formData,
                        bodyOnly: bodyOnly))
                .Html(
                    "#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: resultCollection.Aggregations,
                    container: false))
                .Func("setKamban")
                .WindowScrollTop().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Kamban(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            bool bodyOnly)
        {
            var groupByColumn = formData.Keys.Contains("KambanGroupByColumn")
                ? formData["KambanGroupByColumn"].Value
                : "Status";
            var aggregateType = formData.Keys.Contains("KambanAggregateType")
                ? formData["KambanAggregateType"].Value
                : "Total";
            var valueColumn = formData.Keys.Contains("KambanValueColumn")
                ? formData["KambanValueColumn"].Value
                : "NumA";
            var column = Rds.ResultsColumn()
                .ResultId()
                .Manager()
                .Owner();
            siteSettings.TitleColumnCollection().ForEach(titleColumn =>
                column.ResultsColumn(titleColumn.ColumnName));
            column.ResultsColumn(groupByColumn);
            column.ResultsColumn(valueColumn);
            var data = new ResultCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: column,
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Results",
                    formData: formData,
                    where: Rds.ResultsWhere().SiteId(siteSettings.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.IssuesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)))
                        .Select(o => new Libraries.Charts.KambanElement()
                        {
                            Id = o.Id,
                            Title = o.Title.DisplayValue,
                            Manager = o.Manager,
                            Owner = o.Owner,
                            Group = o.PropertyValue(groupByColumn),
                            Value = o.PropertyValue(valueColumn).ToDecimal()
                        });
            return !bodyOnly
                ? hb.Kamban(
                    siteSettings: siteSettings,
                    groupByColumn: groupByColumn,
                    aggregateType: aggregateType,
                    valueColumn: valueColumn,
                    permissionType: permissionType,
                    data: data)
                : hb.KambanGrid(
                    siteSettings: siteSettings,
                    groupByColumn: siteSettings.AllColumn(groupByColumn),
                    aggregateType: aggregateType,
                    valueColumn: siteSettings.AllColumn(valueColumn),
                    data: data);
        }
    }
}
