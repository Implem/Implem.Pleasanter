using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
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
    public class IssueModel : BaseItemModel
    {
        public long Id { get { return IssueId; } }
        public override long UrlId { get { return IssueId; } }
        public long IssueId = 0;
        public DateTime StartTime = 0.ToDateTime();
        public CompletionTime CompletionTime = new CompletionTime();
        public WorkValue WorkValue = new WorkValue();
        public ProgressRate ProgressRate = new ProgressRate();
        public decimal RemainingWorkValue = 0;
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
        public TitleBody TitleBody { get { return new TitleBody(IssueId, Title.Value, Title.DisplayValue, Body); } }
        public long SavedIssueId = 0;
        public DateTime SavedStartTime = 0.ToDateTime();
        public DateTime SavedCompletionTime = 0.ToDateTime();
        public decimal SavedWorkValue = 0;
        public decimal SavedProgressRate = 0;
        public decimal SavedRemainingWorkValue = 0;
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
        public bool StartTime_Updated { get { return StartTime != SavedStartTime && StartTime != null; } }
        public bool CompletionTime_Updated { get { return CompletionTime.Value != SavedCompletionTime && CompletionTime.Value != null; } }
        public bool WorkValue_Updated { get { return WorkValue.Value != SavedWorkValue; } }
        public bool ProgressRate_Updated { get { return ProgressRate.Value != SavedProgressRate; } }
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
                case "IssueId": return IssueId.ToString();
                case "Ver": return Ver.ToString();
                case "Title": return Title.Value;
                case "Body": return Body;
                case "TitleBody": return TitleBody.ToString();
                case "StartTime": return StartTime.ToString();
                case "CompletionTime": return CompletionTime.Value.ToString();
                case "WorkValue": return WorkValue.Value.ToString();
                case "ProgressRate": return ProgressRate.Value.ToString();
                case "RemainingWorkValue": return RemainingWorkValue.ToString();
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

        public IssueModel(SiteSettings ss)
        {
            SiteSettings = ss;
        }

        public IssueModel(long issueId, long siteId, bool setByForm = false)
        {
            IssueId = issueId;
            SiteId = siteId;
            if (setByForm) SetByForm();
            Get();
        }

        public IssueModel()
        {
        }

        public IssueModel(
            SiteSettings ss, 
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteId = ss.SiteId;
            Manager = SiteInfo.User(Sessions.UserId());
            Owner = SiteInfo.User(Sessions.UserId());
            SiteSettings = ss;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public IssueModel(
            SiteSettings ss, 
            long issueId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = ss;
            IssueId = issueId;
            SiteId = ss.SiteId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public IssueModel(
            SiteSettings ss, 
            Permissions.Types pt,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = ss;
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

        public IssueModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectIssues(
                tableType: tableType,
                column: column ?? Rds.IssuesEditorColumns(SiteSettings),
                join: join ??  Rds.IssuesJoinDefault(),
                where: where ?? Rds.IssuesWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public Dictionary<string, int> SearchIndexHash()
        {
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            else
            {
                var searchIndexHash = new Dictionary<string, int>();
                SiteInfo.SiteMenu.Breadcrumb(SiteId).SearchIndexes(searchIndexHash, 100);
                SiteId.SearchIndexes(searchIndexHash, 200);
                UpdatedTime.SearchIndexes(searchIndexHash, 200);
                IssueId.SearchIndexes(searchIndexHash, 1);
                Title.SearchIndexes(searchIndexHash, 4);
                Body.SearchIndexes(searchIndexHash, 200);
                StartTime.SearchIndexes(searchIndexHash, 200);
                CompletionTime.SearchIndexes(searchIndexHash, 200);
                WorkValue.SearchIndexes(searchIndexHash, 200);
                ProgressRate.SearchIndexes(searchIndexHash, 200);
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
                SearchIndexExtensions.OutgoingMailsSearchIndexes(
                    searchIndexHash, "Issues", IssueId);
                return searchIndexHash;
            }
        }

        public Error.Types Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool notice = false,
            bool paramAll = false)
        {
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Issues")
                            .SiteId(SiteId)
                            .Title(Title.Value)),
                    Rds.InsertIssues(
                        tableType: tableType,
                        param: param ?? Rds.IssuesParamDefault(
                            this, setDefault: true, paramAll: paramAll)),
                    InsertLinks(SiteSettings, selectIdentity: true),
                });
            IssueId = newId != 0 ? newId : IssueId;
            SynchronizeSummary();
            if (notice) Notice("Created");
            Get();
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(IssueUtilities.TitleDisplayValue(SiteSettings, this)),
                    where: Rds.ItemsWhere().ReferenceId(IssueId)));
            return Error.Types.None;
        }

        public Error.Types Update(bool notice = false, bool paramAll = false)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateIssues(
                        verUp: VerUp,
                        where: Rds.IssuesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: Rds.IssuesParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return Error.Types.UpdateConflicts;
            SynchronizeSummary();
            if (notice) Notice("Updated");
            Get();
            UpdateRelatedRecords();
            return Error.Types.None;
        }

        public void UpdateRelatedRecords(
            bool addUpdatedTimeParam = true, bool addUpdatorParam = true)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(IssueId),
                        param: Rds.ItemsParam()
                            .SiteId(SiteId)
                            .Title(IssueUtilities.TitleDisplayValue(SiteSettings, this))
                            .MaintenanceTarget(true),
                        addUpdatedTimeParam: addUpdatedTimeParam,
                        addUpdatorParam: addUpdatorParam),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(IssueId)),
                    InsertLinks(SiteSettings)
                });
        }

        private SqlInsert InsertLinks(
            SiteSettings ss, bool selectIdentity = false)
        {
            var link = new Dictionary<long, long>();
            ss.ColumnCollection.Where(o => o.Link.ToBool()).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "ClassA": if (ClassA.ToLong() != 0 && !link.ContainsKey(ClassA.ToLong())) link.Add(ClassA.ToLong(), IssueId); break;
                    case "ClassB": if (ClassB.ToLong() != 0 && !link.ContainsKey(ClassB.ToLong())) link.Add(ClassB.ToLong(), IssueId); break;
                    case "ClassC": if (ClassC.ToLong() != 0 && !link.ContainsKey(ClassC.ToLong())) link.Add(ClassC.ToLong(), IssueId); break;
                    case "ClassD": if (ClassD.ToLong() != 0 && !link.ContainsKey(ClassD.ToLong())) link.Add(ClassD.ToLong(), IssueId); break;
                    case "ClassE": if (ClassE.ToLong() != 0 && !link.ContainsKey(ClassE.ToLong())) link.Add(ClassE.ToLong(), IssueId); break;
                    case "ClassF": if (ClassF.ToLong() != 0 && !link.ContainsKey(ClassF.ToLong())) link.Add(ClassF.ToLong(), IssueId); break;
                    case "ClassG": if (ClassG.ToLong() != 0 && !link.ContainsKey(ClassG.ToLong())) link.Add(ClassG.ToLong(), IssueId); break;
                    case "ClassH": if (ClassH.ToLong() != 0 && !link.ContainsKey(ClassH.ToLong())) link.Add(ClassH.ToLong(), IssueId); break;
                    case "ClassI": if (ClassI.ToLong() != 0 && !link.ContainsKey(ClassI.ToLong())) link.Add(ClassI.ToLong(), IssueId); break;
                    case "ClassJ": if (ClassJ.ToLong() != 0 && !link.ContainsKey(ClassJ.ToLong())) link.Add(ClassJ.ToLong(), IssueId); break;
                    case "ClassK": if (ClassK.ToLong() != 0 && !link.ContainsKey(ClassK.ToLong())) link.Add(ClassK.ToLong(), IssueId); break;
                    case "ClassL": if (ClassL.ToLong() != 0 && !link.ContainsKey(ClassL.ToLong())) link.Add(ClassL.ToLong(), IssueId); break;
                    case "ClassM": if (ClassM.ToLong() != 0 && !link.ContainsKey(ClassM.ToLong())) link.Add(ClassM.ToLong(), IssueId); break;
                    case "ClassN": if (ClassN.ToLong() != 0 && !link.ContainsKey(ClassN.ToLong())) link.Add(ClassN.ToLong(), IssueId); break;
                    case "ClassO": if (ClassO.ToLong() != 0 && !link.ContainsKey(ClassO.ToLong())) link.Add(ClassO.ToLong(), IssueId); break;
                    case "ClassP": if (ClassP.ToLong() != 0 && !link.ContainsKey(ClassP.ToLong())) link.Add(ClassP.ToLong(), IssueId); break;
                    case "ClassQ": if (ClassQ.ToLong() != 0 && !link.ContainsKey(ClassQ.ToLong())) link.Add(ClassQ.ToLong(), IssueId); break;
                    case "ClassR": if (ClassR.ToLong() != 0 && !link.ContainsKey(ClassR.ToLong())) link.Add(ClassR.ToLong(), IssueId); break;
                    case "ClassS": if (ClassS.ToLong() != 0 && !link.ContainsKey(ClassS.ToLong())) link.Add(ClassS.ToLong(), IssueId); break;
                    case "ClassT": if (ClassT.ToLong() != 0 && !link.ContainsKey(ClassT.ToLong())) link.Add(ClassT.ToLong(), IssueId); break;
                    case "ClassU": if (ClassU.ToLong() != 0 && !link.ContainsKey(ClassU.ToLong())) link.Add(ClassU.ToLong(), IssueId); break;
                    case "ClassV": if (ClassV.ToLong() != 0 && !link.ContainsKey(ClassV.ToLong())) link.Add(ClassV.ToLong(), IssueId); break;
                    case "ClassW": if (ClassW.ToLong() != 0 && !link.ContainsKey(ClassW.ToLong())) link.Add(ClassW.ToLong(), IssueId); break;
                    case "ClassX": if (ClassX.ToLong() != 0 && !link.ContainsKey(ClassX.ToLong())) link.Add(ClassX.ToLong(), IssueId); break;
                    case "ClassY": if (ClassY.ToLong() != 0 && !link.ContainsKey(ClassY.ToLong())) link.Add(ClassY.ToLong(), IssueId); break;
                    case "ClassZ": if (ClassZ.ToLong() != 0 && !link.ContainsKey(ClassZ.ToLong())) link.Add(ClassZ.ToLong(), IssueId); break;
                    default: break;
                }
            });
            return LinkUtilities.Insert(link, selectIdentity);
        }

        public Error.Types UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Issues")
                            .SiteId(SiteId)
                            .Title(Title.Value)),
                    Rds.UpdateOrInsertIssues(
                        selectIdentity: true,
                        where: where ?? Rds.IssuesWhereDefault(this),
                        param: param ?? Rds.IssuesParamDefault(this, setDefault: true))
                });
            IssueId = newId != 0 ? newId : IssueId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Move(long siteId)
        {
            SiteId = siteId;
            Rds.ExecuteNonQuery(statements: new SqlStatement[]
            {
                Rds.UpdateItems(
                    where: Rds.ItemsWhere().ReferenceId(IssueId),
                    param: Rds.ItemsParam().SiteId(SiteId)),
                Rds.UpdateIssues(
                    where: Rds.IssuesWhere().IssueId(IssueId),
                    param: Rds.IssuesParam().SiteId(SiteId))
            });
            SiteSettings = new SiteModel(siteId).IssuesSiteSettings();
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete(bool notice = false)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().ReferenceId(IssueId)),
                    Rds.DeleteIssues(
                        where: Rds.IssuesWhere().SiteId(SiteId).IssueId(IssueId))
                });
            SynchronizeSummary();
            if (notice) Notice("Deleted");
            return Error.Types.None;
        }

        public Error.Types Restore(long issueId)
        {
            IssueId = issueId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(IssueId)),
                    Rds.RestoreIssues(
                        where: Rds.IssuesWhere().IssueId(IssueId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteIssues(
                    tableType: tableType,
                    param: Rds.IssuesParam().SiteId(SiteId).IssueId(IssueId)));
            return Error.Types.None;
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
                "Issues",
                summary.LinkColumn,
                summary.Type,
                summary.SourceColumn,
                id);
            FormulaUtilities.Update(id);
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
            var param = Rds.IssuesParam();
            SiteSettings.FormulaHash.Keys.ForEach(columnName =>
            {
                switch (columnName)
                {
                    case "WorkValue": param.WorkValue(WorkValue.Value); break;
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
                Rds.UpdateIssues(
                    param: param,
                    where: Rds.IssuesWhereDefault(this),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Issues_Title": Title = new Title(IssueId, Forms.Data(controlId)); break;
                    case "Issues_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Issues_StartTime": StartTime = Forms.DateTime(controlId).ToUniversal(); ProgressRate.StartTime = StartTime; break;
                    case "Issues_CompletionTime": CompletionTime = new CompletionTime(Forms.Data(controlId).ToDateTime(), Status, byForm: true); ProgressRate.CompletionTime = CompletionTime.Value; break;
                    case "Issues_WorkValue": WorkValue = new WorkValue(SiteSettings.GetColumn("WorkValue").Round(Forms.Decimal(controlId)), ProgressRate.Value); break;
                    case "Issues_ProgressRate": ProgressRate = new ProgressRate(CreatedTime, StartTime, CompletionTime, SiteSettings.GetColumn("ProgressRate").Round(Forms.Decimal(controlId))); WorkValue.ProgressRate = ProgressRate.Value; break;
                    case "Issues_Status": Status = new Status(Forms.Data(controlId).ToInt()); CompletionTime.Status = Status; break;
                    case "Issues_Manager": Manager = SiteInfo.User(Forms.Int(controlId)); break;
                    case "Issues_Owner": Owner = SiteInfo.User(Forms.Int(controlId)); break;
                    case "Issues_ClassA": ClassA = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassB": ClassB = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassC": ClassC = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassD": ClassD = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassE": ClassE = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassF": ClassF = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassG": ClassG = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassH": ClassH = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassI": ClassI = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassJ": ClassJ = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassK": ClassK = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassL": ClassL = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassM": ClassM = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassN": ClassN = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassO": ClassO = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassP": ClassP = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassQ": ClassQ = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassR": ClassR = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassS": ClassS = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassT": ClassT = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassU": ClassU = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassV": ClassV = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassW": ClassW = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassX": ClassX = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassY": ClassY = Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassZ": ClassZ = Forms.Data(controlId).ToString(); break;
                    case "Issues_NumA": NumA = SiteSettings.GetColumn("NumA").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumB": NumB = SiteSettings.GetColumn("NumB").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumC": NumC = SiteSettings.GetColumn("NumC").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumD": NumD = SiteSettings.GetColumn("NumD").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumE": NumE = SiteSettings.GetColumn("NumE").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumF": NumF = SiteSettings.GetColumn("NumF").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumG": NumG = SiteSettings.GetColumn("NumG").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumH": NumH = SiteSettings.GetColumn("NumH").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumI": NumI = SiteSettings.GetColumn("NumI").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumJ": NumJ = SiteSettings.GetColumn("NumJ").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumK": NumK = SiteSettings.GetColumn("NumK").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumL": NumL = SiteSettings.GetColumn("NumL").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumM": NumM = SiteSettings.GetColumn("NumM").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumN": NumN = SiteSettings.GetColumn("NumN").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumO": NumO = SiteSettings.GetColumn("NumO").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumP": NumP = SiteSettings.GetColumn("NumP").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumQ": NumQ = SiteSettings.GetColumn("NumQ").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumR": NumR = SiteSettings.GetColumn("NumR").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumS": NumS = SiteSettings.GetColumn("NumS").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumT": NumT = SiteSettings.GetColumn("NumT").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumU": NumU = SiteSettings.GetColumn("NumU").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumV": NumV = SiteSettings.GetColumn("NumV").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumW": NumW = SiteSettings.GetColumn("NumW").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumX": NumX = SiteSettings.GetColumn("NumX").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumY": NumY = SiteSettings.GetColumn("NumY").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumZ": NumZ = SiteSettings.GetColumn("NumZ").Round(Forms.Decimal(controlId)); break;
                    case "Issues_DateA": DateA = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateB": DateB = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateC": DateC = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateD": DateD = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateE": DateE = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateF": DateF = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateG": DateG = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateH": DateH = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateI": DateI = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateJ": DateJ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateK": DateK = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateL": DateL = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateM": DateM = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateN": DateN = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateO": DateO = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateP": DateP = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateQ": DateQ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateR": DateR = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateS": DateS = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateT": DateT = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateU": DateU = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateV": DateV = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateW": DateW = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateX": DateX = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateY": DateY = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateZ": DateZ = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DescriptionA": DescriptionA = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionB": DescriptionB = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionC": DescriptionC = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionD": DescriptionD = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionE": DescriptionE = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionF": DescriptionF = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionG": DescriptionG = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionH": DescriptionH = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionI": DescriptionI = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionJ": DescriptionJ = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionK": DescriptionK = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionL": DescriptionL = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionM": DescriptionM = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionN": DescriptionN = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionO": DescriptionO = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionP": DescriptionP = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionQ": DescriptionQ = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionR": DescriptionR = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionS": DescriptionS = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionT": DescriptionT = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionU": DescriptionU = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionV": DescriptionV = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionW": DescriptionW = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionX": DescriptionX = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionY": DescriptionY = Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionZ": DescriptionZ = Forms.Data(controlId).ToString(); break;
                    case "Issues_CheckA": CheckA = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckB": CheckB = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckC": CheckC = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckD": CheckD = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckE": CheckE = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckF": CheckF = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckG": CheckG = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckH": CheckH = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckI": CheckI = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckJ": CheckJ = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckK": CheckK = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckL": CheckL = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckM": CheckM = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckN": CheckN = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckO": CheckO = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckP": CheckP = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckQ": CheckQ = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckR": CheckR = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckS": CheckS = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckT": CheckT = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckU": CheckU = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckV": CheckV = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckW": CheckW = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckX": CheckX = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckY": CheckY = Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckZ": CheckZ = Forms.Data(controlId).ToBool(); break;
                    case "Issues_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default: break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.ControlId().Split(',')._2nd().ToInt();
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
                    { "WorkValue", WorkValue.Value },
                    { "RemainingWorkValue", RemainingWorkValue },
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
                        case "WorkValue": WorkValue.Value = SiteSettings.FormulaResult(columnName, data); break;
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

        private void Notice(string type)
        {
            var title = IssueUtilities.TitleDisplayValue(SiteSettings, this);
            var url = Url.AbsoluteUri().Replace(
                Url.AbsolutePath(), Navigations.ItemEdit(IssueId));
            switch (type)
            {
                case "Created":
                    SiteSettings.Notifications.ForEach(notification =>
                        notification.Send(
                            Displays.Created(title).ToString(),
                            url,
                            NoticeBody(notification)));
                    break;
                case "Updated":
                    SiteSettings.Notifications.ForEach(notification =>
                    {
                        var body = NoticeBody(notification, update: true);
                        if (body.Length > 0)
                        {
                            notification.Send(
                                Displays.Updated(title).ToString(),
                                url,
                                body);
                        }
                    });
                    break;
                case "Deleted":
                    SiteSettings.Notifications.ForEach(notification =>
                        notification.Send(
                            Displays.Deleted(title).ToString(),
                            url,
                            NoticeBody(notification)));
                    break;
            }
        }

        private string NoticeBody(Notification notification, bool update = false)
        {
            var body = new System.Text.StringBuilder();
            notification.MonitorChangesColumnCollection(SiteSettings).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "Title": body.Append(Title.ToNotice(SavedTitle, column, Title_Updated, update)); break;
                    case "Body": body.Append(Body.ToNotice(SavedBody, column, Body_Updated, update)); break;
                    case "StartTime": body.Append(StartTime.ToNotice(SavedStartTime, column, StartTime_Updated, update)); break;
                    case "CompletionTime": body.Append(CompletionTime.ToNotice(SavedCompletionTime, column, CompletionTime_Updated, update)); break;
                    case "WorkValue": body.Append(WorkValue.ToNotice(SavedWorkValue, column, WorkValue_Updated, update)); break;
                    case "ProgressRate": body.Append(ProgressRate.ToNotice(SavedProgressRate, column, ProgressRate_Updated, update)); break;
                    case "Status": body.Append(Status.ToNotice(SavedStatus, column, Status_Updated, update)); break;
                    case "Manager": body.Append(Manager.ToNotice(SavedManager, column, Manager_Updated, update)); break;
                    case "Owner": body.Append(Owner.ToNotice(SavedOwner, column, Owner_Updated, update)); break;
                    case "ClassA": body.Append(ClassA.ToNotice(SavedClassA, column, ClassA_Updated, update)); break;
                    case "ClassB": body.Append(ClassB.ToNotice(SavedClassB, column, ClassB_Updated, update)); break;
                    case "ClassC": body.Append(ClassC.ToNotice(SavedClassC, column, ClassC_Updated, update)); break;
                    case "ClassD": body.Append(ClassD.ToNotice(SavedClassD, column, ClassD_Updated, update)); break;
                    case "ClassE": body.Append(ClassE.ToNotice(SavedClassE, column, ClassE_Updated, update)); break;
                    case "ClassF": body.Append(ClassF.ToNotice(SavedClassF, column, ClassF_Updated, update)); break;
                    case "ClassG": body.Append(ClassG.ToNotice(SavedClassG, column, ClassG_Updated, update)); break;
                    case "ClassH": body.Append(ClassH.ToNotice(SavedClassH, column, ClassH_Updated, update)); break;
                    case "ClassI": body.Append(ClassI.ToNotice(SavedClassI, column, ClassI_Updated, update)); break;
                    case "ClassJ": body.Append(ClassJ.ToNotice(SavedClassJ, column, ClassJ_Updated, update)); break;
                    case "ClassK": body.Append(ClassK.ToNotice(SavedClassK, column, ClassK_Updated, update)); break;
                    case "ClassL": body.Append(ClassL.ToNotice(SavedClassL, column, ClassL_Updated, update)); break;
                    case "ClassM": body.Append(ClassM.ToNotice(SavedClassM, column, ClassM_Updated, update)); break;
                    case "ClassN": body.Append(ClassN.ToNotice(SavedClassN, column, ClassN_Updated, update)); break;
                    case "ClassO": body.Append(ClassO.ToNotice(SavedClassO, column, ClassO_Updated, update)); break;
                    case "ClassP": body.Append(ClassP.ToNotice(SavedClassP, column, ClassP_Updated, update)); break;
                    case "ClassQ": body.Append(ClassQ.ToNotice(SavedClassQ, column, ClassQ_Updated, update)); break;
                    case "ClassR": body.Append(ClassR.ToNotice(SavedClassR, column, ClassR_Updated, update)); break;
                    case "ClassS": body.Append(ClassS.ToNotice(SavedClassS, column, ClassS_Updated, update)); break;
                    case "ClassT": body.Append(ClassT.ToNotice(SavedClassT, column, ClassT_Updated, update)); break;
                    case "ClassU": body.Append(ClassU.ToNotice(SavedClassU, column, ClassU_Updated, update)); break;
                    case "ClassV": body.Append(ClassV.ToNotice(SavedClassV, column, ClassV_Updated, update)); break;
                    case "ClassW": body.Append(ClassW.ToNotice(SavedClassW, column, ClassW_Updated, update)); break;
                    case "ClassX": body.Append(ClassX.ToNotice(SavedClassX, column, ClassX_Updated, update)); break;
                    case "ClassY": body.Append(ClassY.ToNotice(SavedClassY, column, ClassY_Updated, update)); break;
                    case "ClassZ": body.Append(ClassZ.ToNotice(SavedClassZ, column, ClassZ_Updated, update)); break;
                    case "NumA": body.Append(NumA.ToNotice(SavedNumA, column, NumA_Updated, update)); break;
                    case "NumB": body.Append(NumB.ToNotice(SavedNumB, column, NumB_Updated, update)); break;
                    case "NumC": body.Append(NumC.ToNotice(SavedNumC, column, NumC_Updated, update)); break;
                    case "NumD": body.Append(NumD.ToNotice(SavedNumD, column, NumD_Updated, update)); break;
                    case "NumE": body.Append(NumE.ToNotice(SavedNumE, column, NumE_Updated, update)); break;
                    case "NumF": body.Append(NumF.ToNotice(SavedNumF, column, NumF_Updated, update)); break;
                    case "NumG": body.Append(NumG.ToNotice(SavedNumG, column, NumG_Updated, update)); break;
                    case "NumH": body.Append(NumH.ToNotice(SavedNumH, column, NumH_Updated, update)); break;
                    case "NumI": body.Append(NumI.ToNotice(SavedNumI, column, NumI_Updated, update)); break;
                    case "NumJ": body.Append(NumJ.ToNotice(SavedNumJ, column, NumJ_Updated, update)); break;
                    case "NumK": body.Append(NumK.ToNotice(SavedNumK, column, NumK_Updated, update)); break;
                    case "NumL": body.Append(NumL.ToNotice(SavedNumL, column, NumL_Updated, update)); break;
                    case "NumM": body.Append(NumM.ToNotice(SavedNumM, column, NumM_Updated, update)); break;
                    case "NumN": body.Append(NumN.ToNotice(SavedNumN, column, NumN_Updated, update)); break;
                    case "NumO": body.Append(NumO.ToNotice(SavedNumO, column, NumO_Updated, update)); break;
                    case "NumP": body.Append(NumP.ToNotice(SavedNumP, column, NumP_Updated, update)); break;
                    case "NumQ": body.Append(NumQ.ToNotice(SavedNumQ, column, NumQ_Updated, update)); break;
                    case "NumR": body.Append(NumR.ToNotice(SavedNumR, column, NumR_Updated, update)); break;
                    case "NumS": body.Append(NumS.ToNotice(SavedNumS, column, NumS_Updated, update)); break;
                    case "NumT": body.Append(NumT.ToNotice(SavedNumT, column, NumT_Updated, update)); break;
                    case "NumU": body.Append(NumU.ToNotice(SavedNumU, column, NumU_Updated, update)); break;
                    case "NumV": body.Append(NumV.ToNotice(SavedNumV, column, NumV_Updated, update)); break;
                    case "NumW": body.Append(NumW.ToNotice(SavedNumW, column, NumW_Updated, update)); break;
                    case "NumX": body.Append(NumX.ToNotice(SavedNumX, column, NumX_Updated, update)); break;
                    case "NumY": body.Append(NumY.ToNotice(SavedNumY, column, NumY_Updated, update)); break;
                    case "NumZ": body.Append(NumZ.ToNotice(SavedNumZ, column, NumZ_Updated, update)); break;
                    case "DateA": body.Append(DateA.ToNotice(SavedDateA, column, DateA_Updated, update)); break;
                    case "DateB": body.Append(DateB.ToNotice(SavedDateB, column, DateB_Updated, update)); break;
                    case "DateC": body.Append(DateC.ToNotice(SavedDateC, column, DateC_Updated, update)); break;
                    case "DateD": body.Append(DateD.ToNotice(SavedDateD, column, DateD_Updated, update)); break;
                    case "DateE": body.Append(DateE.ToNotice(SavedDateE, column, DateE_Updated, update)); break;
                    case "DateF": body.Append(DateF.ToNotice(SavedDateF, column, DateF_Updated, update)); break;
                    case "DateG": body.Append(DateG.ToNotice(SavedDateG, column, DateG_Updated, update)); break;
                    case "DateH": body.Append(DateH.ToNotice(SavedDateH, column, DateH_Updated, update)); break;
                    case "DateI": body.Append(DateI.ToNotice(SavedDateI, column, DateI_Updated, update)); break;
                    case "DateJ": body.Append(DateJ.ToNotice(SavedDateJ, column, DateJ_Updated, update)); break;
                    case "DateK": body.Append(DateK.ToNotice(SavedDateK, column, DateK_Updated, update)); break;
                    case "DateL": body.Append(DateL.ToNotice(SavedDateL, column, DateL_Updated, update)); break;
                    case "DateM": body.Append(DateM.ToNotice(SavedDateM, column, DateM_Updated, update)); break;
                    case "DateN": body.Append(DateN.ToNotice(SavedDateN, column, DateN_Updated, update)); break;
                    case "DateO": body.Append(DateO.ToNotice(SavedDateO, column, DateO_Updated, update)); break;
                    case "DateP": body.Append(DateP.ToNotice(SavedDateP, column, DateP_Updated, update)); break;
                    case "DateQ": body.Append(DateQ.ToNotice(SavedDateQ, column, DateQ_Updated, update)); break;
                    case "DateR": body.Append(DateR.ToNotice(SavedDateR, column, DateR_Updated, update)); break;
                    case "DateS": body.Append(DateS.ToNotice(SavedDateS, column, DateS_Updated, update)); break;
                    case "DateT": body.Append(DateT.ToNotice(SavedDateT, column, DateT_Updated, update)); break;
                    case "DateU": body.Append(DateU.ToNotice(SavedDateU, column, DateU_Updated, update)); break;
                    case "DateV": body.Append(DateV.ToNotice(SavedDateV, column, DateV_Updated, update)); break;
                    case "DateW": body.Append(DateW.ToNotice(SavedDateW, column, DateW_Updated, update)); break;
                    case "DateX": body.Append(DateX.ToNotice(SavedDateX, column, DateX_Updated, update)); break;
                    case "DateY": body.Append(DateY.ToNotice(SavedDateY, column, DateY_Updated, update)); break;
                    case "DateZ": body.Append(DateZ.ToNotice(SavedDateZ, column, DateZ_Updated, update)); break;
                    case "DescriptionA": body.Append(DescriptionA.ToNotice(SavedDescriptionA, column, DescriptionA_Updated, update)); break;
                    case "DescriptionB": body.Append(DescriptionB.ToNotice(SavedDescriptionB, column, DescriptionB_Updated, update)); break;
                    case "DescriptionC": body.Append(DescriptionC.ToNotice(SavedDescriptionC, column, DescriptionC_Updated, update)); break;
                    case "DescriptionD": body.Append(DescriptionD.ToNotice(SavedDescriptionD, column, DescriptionD_Updated, update)); break;
                    case "DescriptionE": body.Append(DescriptionE.ToNotice(SavedDescriptionE, column, DescriptionE_Updated, update)); break;
                    case "DescriptionF": body.Append(DescriptionF.ToNotice(SavedDescriptionF, column, DescriptionF_Updated, update)); break;
                    case "DescriptionG": body.Append(DescriptionG.ToNotice(SavedDescriptionG, column, DescriptionG_Updated, update)); break;
                    case "DescriptionH": body.Append(DescriptionH.ToNotice(SavedDescriptionH, column, DescriptionH_Updated, update)); break;
                    case "DescriptionI": body.Append(DescriptionI.ToNotice(SavedDescriptionI, column, DescriptionI_Updated, update)); break;
                    case "DescriptionJ": body.Append(DescriptionJ.ToNotice(SavedDescriptionJ, column, DescriptionJ_Updated, update)); break;
                    case "DescriptionK": body.Append(DescriptionK.ToNotice(SavedDescriptionK, column, DescriptionK_Updated, update)); break;
                    case "DescriptionL": body.Append(DescriptionL.ToNotice(SavedDescriptionL, column, DescriptionL_Updated, update)); break;
                    case "DescriptionM": body.Append(DescriptionM.ToNotice(SavedDescriptionM, column, DescriptionM_Updated, update)); break;
                    case "DescriptionN": body.Append(DescriptionN.ToNotice(SavedDescriptionN, column, DescriptionN_Updated, update)); break;
                    case "DescriptionO": body.Append(DescriptionO.ToNotice(SavedDescriptionO, column, DescriptionO_Updated, update)); break;
                    case "DescriptionP": body.Append(DescriptionP.ToNotice(SavedDescriptionP, column, DescriptionP_Updated, update)); break;
                    case "DescriptionQ": body.Append(DescriptionQ.ToNotice(SavedDescriptionQ, column, DescriptionQ_Updated, update)); break;
                    case "DescriptionR": body.Append(DescriptionR.ToNotice(SavedDescriptionR, column, DescriptionR_Updated, update)); break;
                    case "DescriptionS": body.Append(DescriptionS.ToNotice(SavedDescriptionS, column, DescriptionS_Updated, update)); break;
                    case "DescriptionT": body.Append(DescriptionT.ToNotice(SavedDescriptionT, column, DescriptionT_Updated, update)); break;
                    case "DescriptionU": body.Append(DescriptionU.ToNotice(SavedDescriptionU, column, DescriptionU_Updated, update)); break;
                    case "DescriptionV": body.Append(DescriptionV.ToNotice(SavedDescriptionV, column, DescriptionV_Updated, update)); break;
                    case "DescriptionW": body.Append(DescriptionW.ToNotice(SavedDescriptionW, column, DescriptionW_Updated, update)); break;
                    case "DescriptionX": body.Append(DescriptionX.ToNotice(SavedDescriptionX, column, DescriptionX_Updated, update)); break;
                    case "DescriptionY": body.Append(DescriptionY.ToNotice(SavedDescriptionY, column, DescriptionY_Updated, update)); break;
                    case "DescriptionZ": body.Append(DescriptionZ.ToNotice(SavedDescriptionZ, column, DescriptionZ_Updated, update)); break;
                    case "CheckA": body.Append(CheckA.ToNotice(SavedCheckA, column, CheckA_Updated, update)); break;
                    case "CheckB": body.Append(CheckB.ToNotice(SavedCheckB, column, CheckB_Updated, update)); break;
                    case "CheckC": body.Append(CheckC.ToNotice(SavedCheckC, column, CheckC_Updated, update)); break;
                    case "CheckD": body.Append(CheckD.ToNotice(SavedCheckD, column, CheckD_Updated, update)); break;
                    case "CheckE": body.Append(CheckE.ToNotice(SavedCheckE, column, CheckE_Updated, update)); break;
                    case "CheckF": body.Append(CheckF.ToNotice(SavedCheckF, column, CheckF_Updated, update)); break;
                    case "CheckG": body.Append(CheckG.ToNotice(SavedCheckG, column, CheckG_Updated, update)); break;
                    case "CheckH": body.Append(CheckH.ToNotice(SavedCheckH, column, CheckH_Updated, update)); break;
                    case "CheckI": body.Append(CheckI.ToNotice(SavedCheckI, column, CheckI_Updated, update)); break;
                    case "CheckJ": body.Append(CheckJ.ToNotice(SavedCheckJ, column, CheckJ_Updated, update)); break;
                    case "CheckK": body.Append(CheckK.ToNotice(SavedCheckK, column, CheckK_Updated, update)); break;
                    case "CheckL": body.Append(CheckL.ToNotice(SavedCheckL, column, CheckL_Updated, update)); break;
                    case "CheckM": body.Append(CheckM.ToNotice(SavedCheckM, column, CheckM_Updated, update)); break;
                    case "CheckN": body.Append(CheckN.ToNotice(SavedCheckN, column, CheckN_Updated, update)); break;
                    case "CheckO": body.Append(CheckO.ToNotice(SavedCheckO, column, CheckO_Updated, update)); break;
                    case "CheckP": body.Append(CheckP.ToNotice(SavedCheckP, column, CheckP_Updated, update)); break;
                    case "CheckQ": body.Append(CheckQ.ToNotice(SavedCheckQ, column, CheckQ_Updated, update)); break;
                    case "CheckR": body.Append(CheckR.ToNotice(SavedCheckR, column, CheckR_Updated, update)); break;
                    case "CheckS": body.Append(CheckS.ToNotice(SavedCheckS, column, CheckS_Updated, update)); break;
                    case "CheckT": body.Append(CheckT.ToNotice(SavedCheckT, column, CheckT_Updated, update)); break;
                    case "CheckU": body.Append(CheckU.ToNotice(SavedCheckU, column, CheckU_Updated, update)); break;
                    case "CheckV": body.Append(CheckV.ToNotice(SavedCheckV, column, CheckV_Updated, update)); break;
                    case "CheckW": body.Append(CheckW.ToNotice(SavedCheckW, column, CheckW_Updated, update)); break;
                    case "CheckX": body.Append(CheckX.ToNotice(SavedCheckX, column, CheckX_Updated, update)); break;
                    case "CheckY": body.Append(CheckY.ToNotice(SavedCheckY, column, CheckY_Updated, update)); break;
                    case "CheckZ": body.Append(CheckZ.ToNotice(SavedCheckZ, column, CheckZ_Updated, update)); break;
                    case "Comments": body.Append(Comments.ToNotice(SavedComments, column, Comments_Updated, update)); break;
                    case "Creator": body.Append(Creator.ToNotice(SavedCreator, column, Creator_Updated, update)); break;
                    case "Updator": body.Append(Updator.ToNotice(SavedUpdator, column, Updator_Updated, update)); break;
                    case "CreatedTime": body.Append(CreatedTime.ToNotice(SavedCreatedTime, column, CreatedTime_Updated, update)); break;
                }
            });
            return body.ToString();
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
                    case "IssueId": if (dataRow[name] != DBNull.Value) { IssueId = dataRow[name].ToLong(); SavedIssueId = IssueId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "Title": Title = new Title(dataRow, "IssueId"); SavedTitle = Title.Value; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "StartTime": StartTime = dataRow[name].ToDateTime(); SavedStartTime = StartTime; break;
                    case "CompletionTime": CompletionTime = new CompletionTime(dataRow, "CompletionTime"); SavedCompletionTime = CompletionTime.Value; break;
                    case "WorkValue": WorkValue = new WorkValue(dataRow); SavedWorkValue = WorkValue.Value; break;
                    case "ProgressRate": ProgressRate = new ProgressRate(dataRow); SavedProgressRate = ProgressRate.Value; break;
                    case "RemainingWorkValue": RemainingWorkValue = dataRow[name].ToDecimal(); SavedRemainingWorkValue = RemainingWorkValue; break;
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
                Title.DisplayValue = IssueUtilities.TitleDisplayValue(SiteSettings, this);
            }
        }
    }
}
