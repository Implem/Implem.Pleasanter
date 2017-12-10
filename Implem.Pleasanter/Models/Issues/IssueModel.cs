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
        public Attachments AttachmentsA = new Attachments();
        public Attachments AttachmentsB = new Attachments();
        public Attachments AttachmentsC = new Attachments();
        public Attachments AttachmentsD = new Attachments();
        public Attachments AttachmentsE = new Attachments();
        public Attachments AttachmentsF = new Attachments();
        public Attachments AttachmentsG = new Attachments();
        public Attachments AttachmentsH = new Attachments();
        public Attachments AttachmentsI = new Attachments();
        public Attachments AttachmentsJ = new Attachments();
        public Attachments AttachmentsK = new Attachments();
        public Attachments AttachmentsL = new Attachments();
        public Attachments AttachmentsM = new Attachments();
        public Attachments AttachmentsN = new Attachments();
        public Attachments AttachmentsO = new Attachments();
        public Attachments AttachmentsP = new Attachments();
        public Attachments AttachmentsQ = new Attachments();
        public Attachments AttachmentsR = new Attachments();
        public Attachments AttachmentsS = new Attachments();
        public Attachments AttachmentsT = new Attachments();
        public Attachments AttachmentsU = new Attachments();
        public Attachments AttachmentsV = new Attachments();
        public Attachments AttachmentsW = new Attachments();
        public Attachments AttachmentsX = new Attachments();
        public Attachments AttachmentsY = new Attachments();
        public Attachments AttachmentsZ = new Attachments();
        public TitleBody TitleBody { get { return new TitleBody(IssueId, Title.Value, Title.DisplayValue, Body); } }
        public SiteTitle SiteTitle { get { return new SiteTitle(SiteId); } }
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
        public string SavedAttachmentsA = "[]";
        public string SavedAttachmentsB = "[]";
        public string SavedAttachmentsC = "[]";
        public string SavedAttachmentsD = "[]";
        public string SavedAttachmentsE = "[]";
        public string SavedAttachmentsF = "[]";
        public string SavedAttachmentsG = "[]";
        public string SavedAttachmentsH = "[]";
        public string SavedAttachmentsI = "[]";
        public string SavedAttachmentsJ = "[]";
        public string SavedAttachmentsK = "[]";
        public string SavedAttachmentsL = "[]";
        public string SavedAttachmentsM = "[]";
        public string SavedAttachmentsN = "[]";
        public string SavedAttachmentsO = "[]";
        public string SavedAttachmentsP = "[]";
        public string SavedAttachmentsQ = "[]";
        public string SavedAttachmentsR = "[]";
        public string SavedAttachmentsS = "[]";
        public string SavedAttachmentsT = "[]";
        public string SavedAttachmentsU = "[]";
        public string SavedAttachmentsV = "[]";
        public string SavedAttachmentsW = "[]";
        public string SavedAttachmentsX = "[]";
        public string SavedAttachmentsY = "[]";
        public string SavedAttachmentsZ = "[]";
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
        public bool AttachmentsA_Updated { get { return AttachmentsA.ToJson() != SavedAttachmentsA && AttachmentsA.ToJson() != null; } }
        public bool AttachmentsB_Updated { get { return AttachmentsB.ToJson() != SavedAttachmentsB && AttachmentsB.ToJson() != null; } }
        public bool AttachmentsC_Updated { get { return AttachmentsC.ToJson() != SavedAttachmentsC && AttachmentsC.ToJson() != null; } }
        public bool AttachmentsD_Updated { get { return AttachmentsD.ToJson() != SavedAttachmentsD && AttachmentsD.ToJson() != null; } }
        public bool AttachmentsE_Updated { get { return AttachmentsE.ToJson() != SavedAttachmentsE && AttachmentsE.ToJson() != null; } }
        public bool AttachmentsF_Updated { get { return AttachmentsF.ToJson() != SavedAttachmentsF && AttachmentsF.ToJson() != null; } }
        public bool AttachmentsG_Updated { get { return AttachmentsG.ToJson() != SavedAttachmentsG && AttachmentsG.ToJson() != null; } }
        public bool AttachmentsH_Updated { get { return AttachmentsH.ToJson() != SavedAttachmentsH && AttachmentsH.ToJson() != null; } }
        public bool AttachmentsI_Updated { get { return AttachmentsI.ToJson() != SavedAttachmentsI && AttachmentsI.ToJson() != null; } }
        public bool AttachmentsJ_Updated { get { return AttachmentsJ.ToJson() != SavedAttachmentsJ && AttachmentsJ.ToJson() != null; } }
        public bool AttachmentsK_Updated { get { return AttachmentsK.ToJson() != SavedAttachmentsK && AttachmentsK.ToJson() != null; } }
        public bool AttachmentsL_Updated { get { return AttachmentsL.ToJson() != SavedAttachmentsL && AttachmentsL.ToJson() != null; } }
        public bool AttachmentsM_Updated { get { return AttachmentsM.ToJson() != SavedAttachmentsM && AttachmentsM.ToJson() != null; } }
        public bool AttachmentsN_Updated { get { return AttachmentsN.ToJson() != SavedAttachmentsN && AttachmentsN.ToJson() != null; } }
        public bool AttachmentsO_Updated { get { return AttachmentsO.ToJson() != SavedAttachmentsO && AttachmentsO.ToJson() != null; } }
        public bool AttachmentsP_Updated { get { return AttachmentsP.ToJson() != SavedAttachmentsP && AttachmentsP.ToJson() != null; } }
        public bool AttachmentsQ_Updated { get { return AttachmentsQ.ToJson() != SavedAttachmentsQ && AttachmentsQ.ToJson() != null; } }
        public bool AttachmentsR_Updated { get { return AttachmentsR.ToJson() != SavedAttachmentsR && AttachmentsR.ToJson() != null; } }
        public bool AttachmentsS_Updated { get { return AttachmentsS.ToJson() != SavedAttachmentsS && AttachmentsS.ToJson() != null; } }
        public bool AttachmentsT_Updated { get { return AttachmentsT.ToJson() != SavedAttachmentsT && AttachmentsT.ToJson() != null; } }
        public bool AttachmentsU_Updated { get { return AttachmentsU.ToJson() != SavedAttachmentsU && AttachmentsU.ToJson() != null; } }
        public bool AttachmentsV_Updated { get { return AttachmentsV.ToJson() != SavedAttachmentsV && AttachmentsV.ToJson() != null; } }
        public bool AttachmentsW_Updated { get { return AttachmentsW.ToJson() != SavedAttachmentsW && AttachmentsW.ToJson() != null; } }
        public bool AttachmentsX_Updated { get { return AttachmentsX.ToJson() != SavedAttachmentsX && AttachmentsX.ToJson() != null; } }
        public bool AttachmentsY_Updated { get { return AttachmentsY.ToJson() != SavedAttachmentsY && AttachmentsY.ToJson() != null; } }
        public bool AttachmentsZ_Updated { get { return AttachmentsZ.ToJson() != SavedAttachmentsZ && AttachmentsZ.ToJson() != null; } }

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
                case "AttachmentsA": return AttachmentsA.ToJson();
                case "AttachmentsB": return AttachmentsB.ToJson();
                case "AttachmentsC": return AttachmentsC.ToJson();
                case "AttachmentsD": return AttachmentsD.ToJson();
                case "AttachmentsE": return AttachmentsE.ToJson();
                case "AttachmentsF": return AttachmentsF.ToJson();
                case "AttachmentsG": return AttachmentsG.ToJson();
                case "AttachmentsH": return AttachmentsH.ToJson();
                case "AttachmentsI": return AttachmentsI.ToJson();
                case "AttachmentsJ": return AttachmentsJ.ToJson();
                case "AttachmentsK": return AttachmentsK.ToJson();
                case "AttachmentsL": return AttachmentsL.ToJson();
                case "AttachmentsM": return AttachmentsM.ToJson();
                case "AttachmentsN": return AttachmentsN.ToJson();
                case "AttachmentsO": return AttachmentsO.ToJson();
                case "AttachmentsP": return AttachmentsP.ToJson();
                case "AttachmentsQ": return AttachmentsQ.ToJson();
                case "AttachmentsR": return AttachmentsR.ToJson();
                case "AttachmentsS": return AttachmentsS.ToJson();
                case "AttachmentsT": return AttachmentsT.ToJson();
                case "AttachmentsU": return AttachmentsU.ToJson();
                case "AttachmentsV": return AttachmentsV.ToJson();
                case "AttachmentsW": return AttachmentsW.ToJson();
                case "AttachmentsX": return AttachmentsX.ToJson();
                case "AttachmentsY": return AttachmentsY.ToJson();
                case "AttachmentsZ": return AttachmentsZ.ToJson();
                case "SiteTitle": return SiteTitle.SiteId.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return null;
            }
        }

        public Dictionary<string, string> PropertyValues(IEnumerable<string> names)
        {
            var hash = new Dictionary<string, string>();
            names?.ForEach(name =>
            {
                switch (name)
                {
                    case "SiteId":
                        hash.Add("SiteId", SiteId.ToString());
                        break;
                    case "UpdatedTime":
                        hash.Add("UpdatedTime", UpdatedTime.Value.ToString());
                        break;
                    case "IssueId":
                        hash.Add("IssueId", IssueId.ToString());
                        break;
                    case "Ver":
                        hash.Add("Ver", Ver.ToString());
                        break;
                    case "Title":
                        hash.Add("Title", Title.Value);
                        break;
                    case "Body":
                        hash.Add("Body", Body);
                        break;
                    case "TitleBody":
                        hash.Add("TitleBody", TitleBody.ToString());
                        break;
                    case "StartTime":
                        hash.Add("StartTime", StartTime.ToString());
                        break;
                    case "CompletionTime":
                        hash.Add("CompletionTime", CompletionTime.Value.ToString());
                        break;
                    case "WorkValue":
                        hash.Add("WorkValue", WorkValue.Value.ToString());
                        break;
                    case "ProgressRate":
                        hash.Add("ProgressRate", ProgressRate.Value.ToString());
                        break;
                    case "RemainingWorkValue":
                        hash.Add("RemainingWorkValue", RemainingWorkValue.ToString());
                        break;
                    case "Status":
                        hash.Add("Status", Status.Value.ToString());
                        break;
                    case "Manager":
                        hash.Add("Manager", Manager.Id.ToString());
                        break;
                    case "Owner":
                        hash.Add("Owner", Owner.Id.ToString());
                        break;
                    case "ClassA":
                        hash.Add("ClassA", ClassA);
                        break;
                    case "ClassB":
                        hash.Add("ClassB", ClassB);
                        break;
                    case "ClassC":
                        hash.Add("ClassC", ClassC);
                        break;
                    case "ClassD":
                        hash.Add("ClassD", ClassD);
                        break;
                    case "ClassE":
                        hash.Add("ClassE", ClassE);
                        break;
                    case "ClassF":
                        hash.Add("ClassF", ClassF);
                        break;
                    case "ClassG":
                        hash.Add("ClassG", ClassG);
                        break;
                    case "ClassH":
                        hash.Add("ClassH", ClassH);
                        break;
                    case "ClassI":
                        hash.Add("ClassI", ClassI);
                        break;
                    case "ClassJ":
                        hash.Add("ClassJ", ClassJ);
                        break;
                    case "ClassK":
                        hash.Add("ClassK", ClassK);
                        break;
                    case "ClassL":
                        hash.Add("ClassL", ClassL);
                        break;
                    case "ClassM":
                        hash.Add("ClassM", ClassM);
                        break;
                    case "ClassN":
                        hash.Add("ClassN", ClassN);
                        break;
                    case "ClassO":
                        hash.Add("ClassO", ClassO);
                        break;
                    case "ClassP":
                        hash.Add("ClassP", ClassP);
                        break;
                    case "ClassQ":
                        hash.Add("ClassQ", ClassQ);
                        break;
                    case "ClassR":
                        hash.Add("ClassR", ClassR);
                        break;
                    case "ClassS":
                        hash.Add("ClassS", ClassS);
                        break;
                    case "ClassT":
                        hash.Add("ClassT", ClassT);
                        break;
                    case "ClassU":
                        hash.Add("ClassU", ClassU);
                        break;
                    case "ClassV":
                        hash.Add("ClassV", ClassV);
                        break;
                    case "ClassW":
                        hash.Add("ClassW", ClassW);
                        break;
                    case "ClassX":
                        hash.Add("ClassX", ClassX);
                        break;
                    case "ClassY":
                        hash.Add("ClassY", ClassY);
                        break;
                    case "ClassZ":
                        hash.Add("ClassZ", ClassZ);
                        break;
                    case "NumA":
                        hash.Add("NumA", NumA.ToString());
                        break;
                    case "NumB":
                        hash.Add("NumB", NumB.ToString());
                        break;
                    case "NumC":
                        hash.Add("NumC", NumC.ToString());
                        break;
                    case "NumD":
                        hash.Add("NumD", NumD.ToString());
                        break;
                    case "NumE":
                        hash.Add("NumE", NumE.ToString());
                        break;
                    case "NumF":
                        hash.Add("NumF", NumF.ToString());
                        break;
                    case "NumG":
                        hash.Add("NumG", NumG.ToString());
                        break;
                    case "NumH":
                        hash.Add("NumH", NumH.ToString());
                        break;
                    case "NumI":
                        hash.Add("NumI", NumI.ToString());
                        break;
                    case "NumJ":
                        hash.Add("NumJ", NumJ.ToString());
                        break;
                    case "NumK":
                        hash.Add("NumK", NumK.ToString());
                        break;
                    case "NumL":
                        hash.Add("NumL", NumL.ToString());
                        break;
                    case "NumM":
                        hash.Add("NumM", NumM.ToString());
                        break;
                    case "NumN":
                        hash.Add("NumN", NumN.ToString());
                        break;
                    case "NumO":
                        hash.Add("NumO", NumO.ToString());
                        break;
                    case "NumP":
                        hash.Add("NumP", NumP.ToString());
                        break;
                    case "NumQ":
                        hash.Add("NumQ", NumQ.ToString());
                        break;
                    case "NumR":
                        hash.Add("NumR", NumR.ToString());
                        break;
                    case "NumS":
                        hash.Add("NumS", NumS.ToString());
                        break;
                    case "NumT":
                        hash.Add("NumT", NumT.ToString());
                        break;
                    case "NumU":
                        hash.Add("NumU", NumU.ToString());
                        break;
                    case "NumV":
                        hash.Add("NumV", NumV.ToString());
                        break;
                    case "NumW":
                        hash.Add("NumW", NumW.ToString());
                        break;
                    case "NumX":
                        hash.Add("NumX", NumX.ToString());
                        break;
                    case "NumY":
                        hash.Add("NumY", NumY.ToString());
                        break;
                    case "NumZ":
                        hash.Add("NumZ", NumZ.ToString());
                        break;
                    case "DateA":
                        hash.Add("DateA", DateA.ToString());
                        break;
                    case "DateB":
                        hash.Add("DateB", DateB.ToString());
                        break;
                    case "DateC":
                        hash.Add("DateC", DateC.ToString());
                        break;
                    case "DateD":
                        hash.Add("DateD", DateD.ToString());
                        break;
                    case "DateE":
                        hash.Add("DateE", DateE.ToString());
                        break;
                    case "DateF":
                        hash.Add("DateF", DateF.ToString());
                        break;
                    case "DateG":
                        hash.Add("DateG", DateG.ToString());
                        break;
                    case "DateH":
                        hash.Add("DateH", DateH.ToString());
                        break;
                    case "DateI":
                        hash.Add("DateI", DateI.ToString());
                        break;
                    case "DateJ":
                        hash.Add("DateJ", DateJ.ToString());
                        break;
                    case "DateK":
                        hash.Add("DateK", DateK.ToString());
                        break;
                    case "DateL":
                        hash.Add("DateL", DateL.ToString());
                        break;
                    case "DateM":
                        hash.Add("DateM", DateM.ToString());
                        break;
                    case "DateN":
                        hash.Add("DateN", DateN.ToString());
                        break;
                    case "DateO":
                        hash.Add("DateO", DateO.ToString());
                        break;
                    case "DateP":
                        hash.Add("DateP", DateP.ToString());
                        break;
                    case "DateQ":
                        hash.Add("DateQ", DateQ.ToString());
                        break;
                    case "DateR":
                        hash.Add("DateR", DateR.ToString());
                        break;
                    case "DateS":
                        hash.Add("DateS", DateS.ToString());
                        break;
                    case "DateT":
                        hash.Add("DateT", DateT.ToString());
                        break;
                    case "DateU":
                        hash.Add("DateU", DateU.ToString());
                        break;
                    case "DateV":
                        hash.Add("DateV", DateV.ToString());
                        break;
                    case "DateW":
                        hash.Add("DateW", DateW.ToString());
                        break;
                    case "DateX":
                        hash.Add("DateX", DateX.ToString());
                        break;
                    case "DateY":
                        hash.Add("DateY", DateY.ToString());
                        break;
                    case "DateZ":
                        hash.Add("DateZ", DateZ.ToString());
                        break;
                    case "DescriptionA":
                        hash.Add("DescriptionA", DescriptionA);
                        break;
                    case "DescriptionB":
                        hash.Add("DescriptionB", DescriptionB);
                        break;
                    case "DescriptionC":
                        hash.Add("DescriptionC", DescriptionC);
                        break;
                    case "DescriptionD":
                        hash.Add("DescriptionD", DescriptionD);
                        break;
                    case "DescriptionE":
                        hash.Add("DescriptionE", DescriptionE);
                        break;
                    case "DescriptionF":
                        hash.Add("DescriptionF", DescriptionF);
                        break;
                    case "DescriptionG":
                        hash.Add("DescriptionG", DescriptionG);
                        break;
                    case "DescriptionH":
                        hash.Add("DescriptionH", DescriptionH);
                        break;
                    case "DescriptionI":
                        hash.Add("DescriptionI", DescriptionI);
                        break;
                    case "DescriptionJ":
                        hash.Add("DescriptionJ", DescriptionJ);
                        break;
                    case "DescriptionK":
                        hash.Add("DescriptionK", DescriptionK);
                        break;
                    case "DescriptionL":
                        hash.Add("DescriptionL", DescriptionL);
                        break;
                    case "DescriptionM":
                        hash.Add("DescriptionM", DescriptionM);
                        break;
                    case "DescriptionN":
                        hash.Add("DescriptionN", DescriptionN);
                        break;
                    case "DescriptionO":
                        hash.Add("DescriptionO", DescriptionO);
                        break;
                    case "DescriptionP":
                        hash.Add("DescriptionP", DescriptionP);
                        break;
                    case "DescriptionQ":
                        hash.Add("DescriptionQ", DescriptionQ);
                        break;
                    case "DescriptionR":
                        hash.Add("DescriptionR", DescriptionR);
                        break;
                    case "DescriptionS":
                        hash.Add("DescriptionS", DescriptionS);
                        break;
                    case "DescriptionT":
                        hash.Add("DescriptionT", DescriptionT);
                        break;
                    case "DescriptionU":
                        hash.Add("DescriptionU", DescriptionU);
                        break;
                    case "DescriptionV":
                        hash.Add("DescriptionV", DescriptionV);
                        break;
                    case "DescriptionW":
                        hash.Add("DescriptionW", DescriptionW);
                        break;
                    case "DescriptionX":
                        hash.Add("DescriptionX", DescriptionX);
                        break;
                    case "DescriptionY":
                        hash.Add("DescriptionY", DescriptionY);
                        break;
                    case "DescriptionZ":
                        hash.Add("DescriptionZ", DescriptionZ);
                        break;
                    case "CheckA":
                        hash.Add("CheckA", CheckA.ToString());
                        break;
                    case "CheckB":
                        hash.Add("CheckB", CheckB.ToString());
                        break;
                    case "CheckC":
                        hash.Add("CheckC", CheckC.ToString());
                        break;
                    case "CheckD":
                        hash.Add("CheckD", CheckD.ToString());
                        break;
                    case "CheckE":
                        hash.Add("CheckE", CheckE.ToString());
                        break;
                    case "CheckF":
                        hash.Add("CheckF", CheckF.ToString());
                        break;
                    case "CheckG":
                        hash.Add("CheckG", CheckG.ToString());
                        break;
                    case "CheckH":
                        hash.Add("CheckH", CheckH.ToString());
                        break;
                    case "CheckI":
                        hash.Add("CheckI", CheckI.ToString());
                        break;
                    case "CheckJ":
                        hash.Add("CheckJ", CheckJ.ToString());
                        break;
                    case "CheckK":
                        hash.Add("CheckK", CheckK.ToString());
                        break;
                    case "CheckL":
                        hash.Add("CheckL", CheckL.ToString());
                        break;
                    case "CheckM":
                        hash.Add("CheckM", CheckM.ToString());
                        break;
                    case "CheckN":
                        hash.Add("CheckN", CheckN.ToString());
                        break;
                    case "CheckO":
                        hash.Add("CheckO", CheckO.ToString());
                        break;
                    case "CheckP":
                        hash.Add("CheckP", CheckP.ToString());
                        break;
                    case "CheckQ":
                        hash.Add("CheckQ", CheckQ.ToString());
                        break;
                    case "CheckR":
                        hash.Add("CheckR", CheckR.ToString());
                        break;
                    case "CheckS":
                        hash.Add("CheckS", CheckS.ToString());
                        break;
                    case "CheckT":
                        hash.Add("CheckT", CheckT.ToString());
                        break;
                    case "CheckU":
                        hash.Add("CheckU", CheckU.ToString());
                        break;
                    case "CheckV":
                        hash.Add("CheckV", CheckV.ToString());
                        break;
                    case "CheckW":
                        hash.Add("CheckW", CheckW.ToString());
                        break;
                    case "CheckX":
                        hash.Add("CheckX", CheckX.ToString());
                        break;
                    case "CheckY":
                        hash.Add("CheckY", CheckY.ToString());
                        break;
                    case "CheckZ":
                        hash.Add("CheckZ", CheckZ.ToString());
                        break;
                    case "AttachmentsA":
                        hash.Add("AttachmentsA", AttachmentsA.ToJson());
                        break;
                    case "AttachmentsB":
                        hash.Add("AttachmentsB", AttachmentsB.ToJson());
                        break;
                    case "AttachmentsC":
                        hash.Add("AttachmentsC", AttachmentsC.ToJson());
                        break;
                    case "AttachmentsD":
                        hash.Add("AttachmentsD", AttachmentsD.ToJson());
                        break;
                    case "AttachmentsE":
                        hash.Add("AttachmentsE", AttachmentsE.ToJson());
                        break;
                    case "AttachmentsF":
                        hash.Add("AttachmentsF", AttachmentsF.ToJson());
                        break;
                    case "AttachmentsG":
                        hash.Add("AttachmentsG", AttachmentsG.ToJson());
                        break;
                    case "AttachmentsH":
                        hash.Add("AttachmentsH", AttachmentsH.ToJson());
                        break;
                    case "AttachmentsI":
                        hash.Add("AttachmentsI", AttachmentsI.ToJson());
                        break;
                    case "AttachmentsJ":
                        hash.Add("AttachmentsJ", AttachmentsJ.ToJson());
                        break;
                    case "AttachmentsK":
                        hash.Add("AttachmentsK", AttachmentsK.ToJson());
                        break;
                    case "AttachmentsL":
                        hash.Add("AttachmentsL", AttachmentsL.ToJson());
                        break;
                    case "AttachmentsM":
                        hash.Add("AttachmentsM", AttachmentsM.ToJson());
                        break;
                    case "AttachmentsN":
                        hash.Add("AttachmentsN", AttachmentsN.ToJson());
                        break;
                    case "AttachmentsO":
                        hash.Add("AttachmentsO", AttachmentsO.ToJson());
                        break;
                    case "AttachmentsP":
                        hash.Add("AttachmentsP", AttachmentsP.ToJson());
                        break;
                    case "AttachmentsQ":
                        hash.Add("AttachmentsQ", AttachmentsQ.ToJson());
                        break;
                    case "AttachmentsR":
                        hash.Add("AttachmentsR", AttachmentsR.ToJson());
                        break;
                    case "AttachmentsS":
                        hash.Add("AttachmentsS", AttachmentsS.ToJson());
                        break;
                    case "AttachmentsT":
                        hash.Add("AttachmentsT", AttachmentsT.ToJson());
                        break;
                    case "AttachmentsU":
                        hash.Add("AttachmentsU", AttachmentsU.ToJson());
                        break;
                    case "AttachmentsV":
                        hash.Add("AttachmentsV", AttachmentsV.ToJson());
                        break;
                    case "AttachmentsW":
                        hash.Add("AttachmentsW", AttachmentsW.ToJson());
                        break;
                    case "AttachmentsX":
                        hash.Add("AttachmentsX", AttachmentsX.ToJson());
                        break;
                    case "AttachmentsY":
                        hash.Add("AttachmentsY", AttachmentsY.ToJson());
                        break;
                    case "AttachmentsZ":
                        hash.Add("AttachmentsZ", AttachmentsZ.ToJson());
                        break;
                    case "SiteTitle":
                        hash.Add("SiteTitle", SiteTitle.SiteId.ToString());
                        break;
                    case "Comments":
                        hash.Add("Comments", Comments.ToJson());
                        break;
                    case "Creator":
                        hash.Add("Creator", Creator.Id.ToString());
                        break;
                    case "Updator":
                        hash.Add("Updator", Updator.Id.ToString());
                        break;
                    case "CreatedTime":
                        hash.Add("CreatedTime", CreatedTime.Value.ToString());
                        break;
                    case "VerUp":
                        hash.Add("VerUp", VerUp.ToString());
                        break;
                    case "Timestamp":
                        hash.Add("Timestamp", Timestamp);
                        break;
                }
            });
            return hash;
        }

        public string CsvData(
            SiteSettings ss, Column column, ExportColumn exportColumn, List<string> mine)
        {
            var value = string.Empty;
            switch (column.Name)
            {
                case "SiteId":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? SiteId.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? UpdatedTime.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "IssueId":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? IssueId.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Ver.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Title":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Title.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Body":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Body.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "TitleBody":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? TitleBody.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "StartTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? StartTime.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CompletionTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CompletionTime.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "WorkValue":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? WorkValue.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ProgressRate":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ProgressRate.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "RemainingWorkValue":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? RemainingWorkValue.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Status":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Status.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Manager":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Manager.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Owner":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Owner.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassA.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassB.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassC.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassD.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassE.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassF.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassG.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassH.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassI.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassJ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassK.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassL.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassM.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassN.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassO.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassP.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassQ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassR.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassS.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassT.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassU.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassV.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassW.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassX.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassY.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "ClassZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? ClassZ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumA.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumB.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumC.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumD.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumE.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumF.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumG.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumH.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumI.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumJ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumK.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumL.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumM.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumN.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumO.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumP.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumQ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumR.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumS.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumT.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumU.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumV.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumW.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumX.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumY.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "NumZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? NumZ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateA.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateB.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateC.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateD.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateE.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateF.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateG.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateH.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateI.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateJ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateK.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateL.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateM.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateN.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateO.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateP.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateQ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateR.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateS.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateT.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateU.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateV.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateW.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateX.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateY.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DateZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DateZ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionA.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionB.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionC.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionD.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionE.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionF.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionG.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionH.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionI.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionJ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionK.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionL.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionM.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionN.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionO.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionP.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionQ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionR.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionS.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionT.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionU.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionV.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionW.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionX.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionY.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "DescriptionZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? DescriptionZ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckA.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckB.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckC.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckD.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckE.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckF.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckG.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckH.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckI.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckJ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckK.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckL.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckM.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckN.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckO.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckP.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckQ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckR.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckS.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckT.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckU.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckV.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckW.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckX.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckY.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CheckZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CheckZ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsA.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsB.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsC.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsD.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsE.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsF.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsG.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsH.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsI.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsJ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsK.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsL.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsM.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsN.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsO.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsP.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsQ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsR.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsS.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsT.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsU.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsV.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsW.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsX.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsY.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "AttachmentsZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? AttachmentsZ.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "SiteTitle":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? SiteTitle.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Comments.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Creator.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? Updator.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? CreatedTime.ToExport(column, exportColumn)
                        : string.Empty;
                    break;
                default: return string.Empty;
            }
            return "\"" + value?.Replace("\"", "\"\"") + "\"";
        }

        public List<long> SwitchTargets;

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
            if (setByForm) SetByForm(ss);
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
            IssueId = issueId;
            SiteId = ss.SiteId;
            Get(ss);
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm(ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public IssueModel(SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing();
            Set(ss, dataRow, tableAlias);
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
            SiteSettings ss, 
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(ss, Rds.ExecuteTable(statements: Rds.SelectIssues(
                tableType: tableType,
                column: column ?? Rds.IssuesEditorColumns(ss),
                join: join ??  Rds.IssuesJoinDefault(),
                where: where ?? Rds.IssuesWhereDefault(this),
                orderBy: orderBy,
                param: param,
                distinct: distinct,
                top: top)));
            return this;
        }

        public string FullText(
            SiteSettings ss, bool backgroundTask = false, bool onCreating = false)
        {
            if (Parameters.Search.Provider != "FullText") return null;
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
            var fullText = new List<string>();
            SiteInfo.TenantCaches[Sessions.TenantId()]
                .SiteMenu.Breadcrumb(SiteId).FullText(fullText);
            SiteId.FullText(fullText);
            UpdatedTime.FullText(fullText);
            IssueId.FullText(fullText);
            Title.FullText(fullText);
            Body.FullText(fullText);
            StartTime.FullText(fullText);
            CompletionTime.FullText(fullText);
            WorkValue.FullText(fullText);
            ProgressRate.FullText(fullText);
            Status.FullText(ss.GetColumn("Status"), fullText);
            Manager.FullText(fullText);
            Owner.FullText(fullText);
            ClassA.FullText(ss.GetColumn("ClassA"), fullText);
            ClassB.FullText(ss.GetColumn("ClassB"), fullText);
            ClassC.FullText(ss.GetColumn("ClassC"), fullText);
            ClassD.FullText(ss.GetColumn("ClassD"), fullText);
            ClassE.FullText(ss.GetColumn("ClassE"), fullText);
            ClassF.FullText(ss.GetColumn("ClassF"), fullText);
            ClassG.FullText(ss.GetColumn("ClassG"), fullText);
            ClassH.FullText(ss.GetColumn("ClassH"), fullText);
            ClassI.FullText(ss.GetColumn("ClassI"), fullText);
            ClassJ.FullText(ss.GetColumn("ClassJ"), fullText);
            ClassK.FullText(ss.GetColumn("ClassK"), fullText);
            ClassL.FullText(ss.GetColumn("ClassL"), fullText);
            ClassM.FullText(ss.GetColumn("ClassM"), fullText);
            ClassN.FullText(ss.GetColumn("ClassN"), fullText);
            ClassO.FullText(ss.GetColumn("ClassO"), fullText);
            ClassP.FullText(ss.GetColumn("ClassP"), fullText);
            ClassQ.FullText(ss.GetColumn("ClassQ"), fullText);
            ClassR.FullText(ss.GetColumn("ClassR"), fullText);
            ClassS.FullText(ss.GetColumn("ClassS"), fullText);
            ClassT.FullText(ss.GetColumn("ClassT"), fullText);
            ClassU.FullText(ss.GetColumn("ClassU"), fullText);
            ClassV.FullText(ss.GetColumn("ClassV"), fullText);
            ClassW.FullText(ss.GetColumn("ClassW"), fullText);
            ClassX.FullText(ss.GetColumn("ClassX"), fullText);
            ClassY.FullText(ss.GetColumn("ClassY"), fullText);
            ClassZ.FullText(ss.GetColumn("ClassZ"), fullText);
            NumA.FullText(fullText);
            NumB.FullText(fullText);
            NumC.FullText(fullText);
            NumD.FullText(fullText);
            NumE.FullText(fullText);
            NumF.FullText(fullText);
            NumG.FullText(fullText);
            NumH.FullText(fullText);
            NumI.FullText(fullText);
            NumJ.FullText(fullText);
            NumK.FullText(fullText);
            NumL.FullText(fullText);
            NumM.FullText(fullText);
            NumN.FullText(fullText);
            NumO.FullText(fullText);
            NumP.FullText(fullText);
            NumQ.FullText(fullText);
            NumR.FullText(fullText);
            NumS.FullText(fullText);
            NumT.FullText(fullText);
            NumU.FullText(fullText);
            NumV.FullText(fullText);
            NumW.FullText(fullText);
            NumX.FullText(fullText);
            NumY.FullText(fullText);
            NumZ.FullText(fullText);
            DateA.FullText(fullText);
            DateB.FullText(fullText);
            DateC.FullText(fullText);
            DateD.FullText(fullText);
            DateE.FullText(fullText);
            DateF.FullText(fullText);
            DateG.FullText(fullText);
            DateH.FullText(fullText);
            DateI.FullText(fullText);
            DateJ.FullText(fullText);
            DateK.FullText(fullText);
            DateL.FullText(fullText);
            DateM.FullText(fullText);
            DateN.FullText(fullText);
            DateO.FullText(fullText);
            DateP.FullText(fullText);
            DateQ.FullText(fullText);
            DateR.FullText(fullText);
            DateS.FullText(fullText);
            DateT.FullText(fullText);
            DateU.FullText(fullText);
            DateV.FullText(fullText);
            DateW.FullText(fullText);
            DateX.FullText(fullText);
            DateY.FullText(fullText);
            DateZ.FullText(fullText);
            DescriptionA.FullText(fullText);
            DescriptionB.FullText(fullText);
            DescriptionC.FullText(fullText);
            DescriptionD.FullText(fullText);
            DescriptionE.FullText(fullText);
            DescriptionF.FullText(fullText);
            DescriptionG.FullText(fullText);
            DescriptionH.FullText(fullText);
            DescriptionI.FullText(fullText);
            DescriptionJ.FullText(fullText);
            DescriptionK.FullText(fullText);
            DescriptionL.FullText(fullText);
            DescriptionM.FullText(fullText);
            DescriptionN.FullText(fullText);
            DescriptionO.FullText(fullText);
            DescriptionP.FullText(fullText);
            DescriptionQ.FullText(fullText);
            DescriptionR.FullText(fullText);
            DescriptionS.FullText(fullText);
            DescriptionT.FullText(fullText);
            DescriptionU.FullText(fullText);
            DescriptionV.FullText(fullText);
            DescriptionW.FullText(fullText);
            DescriptionX.FullText(fullText);
            DescriptionY.FullText(fullText);
            DescriptionZ.FullText(fullText);
            Comments.FullText(fullText);
            Creator.FullText(fullText);
            Updator.FullText(fullText);
            CreatedTime.FullText(fullText);
            if (!onCreating)
            {
                FullTextExtensions.OutgoingMailsFullText(fullText, "Issues", IssueId);
            }
            return fullText
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Distinct()
                .Join(" ");
        }

        public Dictionary<string, int> SearchIndexHash(SiteSettings ss)
        {
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            else
            {
                var searchIndexHash = new Dictionary<string, int>();
                SiteInfo.TenantCaches[Sessions.TenantId()]
                    .SiteMenu.Breadcrumb(SiteId).SearchIndexes(searchIndexHash, 100);
                SiteId.SearchIndexes(searchIndexHash, 200);
                UpdatedTime.SearchIndexes(searchIndexHash, 200);
                IssueId.SearchIndexes(searchIndexHash, 1);
                Title.SearchIndexes(searchIndexHash, 4);
                Body.SearchIndexes(searchIndexHash, 200);
                StartTime.SearchIndexes(searchIndexHash, 200);
                CompletionTime.SearchIndexes(searchIndexHash, 200);
                WorkValue.SearchIndexes(searchIndexHash, 200);
                ProgressRate.SearchIndexes(searchIndexHash, 200);
                Status.SearchIndexes(ss.GetColumn("Status"), searchIndexHash, 200);
                Manager.SearchIndexes(searchIndexHash, 100);
                Owner.SearchIndexes(searchIndexHash, 100);
                ClassA.SearchIndexes(ss.GetColumn("ClassA"), searchIndexHash, 200);
                ClassB.SearchIndexes(ss.GetColumn("ClassB"), searchIndexHash, 200);
                ClassC.SearchIndexes(ss.GetColumn("ClassC"), searchIndexHash, 200);
                ClassD.SearchIndexes(ss.GetColumn("ClassD"), searchIndexHash, 200);
                ClassE.SearchIndexes(ss.GetColumn("ClassE"), searchIndexHash, 200);
                ClassF.SearchIndexes(ss.GetColumn("ClassF"), searchIndexHash, 200);
                ClassG.SearchIndexes(ss.GetColumn("ClassG"), searchIndexHash, 200);
                ClassH.SearchIndexes(ss.GetColumn("ClassH"), searchIndexHash, 200);
                ClassI.SearchIndexes(ss.GetColumn("ClassI"), searchIndexHash, 200);
                ClassJ.SearchIndexes(ss.GetColumn("ClassJ"), searchIndexHash, 200);
                ClassK.SearchIndexes(ss.GetColumn("ClassK"), searchIndexHash, 200);
                ClassL.SearchIndexes(ss.GetColumn("ClassL"), searchIndexHash, 200);
                ClassM.SearchIndexes(ss.GetColumn("ClassM"), searchIndexHash, 200);
                ClassN.SearchIndexes(ss.GetColumn("ClassN"), searchIndexHash, 200);
                ClassO.SearchIndexes(ss.GetColumn("ClassO"), searchIndexHash, 200);
                ClassP.SearchIndexes(ss.GetColumn("ClassP"), searchIndexHash, 200);
                ClassQ.SearchIndexes(ss.GetColumn("ClassQ"), searchIndexHash, 200);
                ClassR.SearchIndexes(ss.GetColumn("ClassR"), searchIndexHash, 200);
                ClassS.SearchIndexes(ss.GetColumn("ClassS"), searchIndexHash, 200);
                ClassT.SearchIndexes(ss.GetColumn("ClassT"), searchIndexHash, 200);
                ClassU.SearchIndexes(ss.GetColumn("ClassU"), searchIndexHash, 200);
                ClassV.SearchIndexes(ss.GetColumn("ClassV"), searchIndexHash, 200);
                ClassW.SearchIndexes(ss.GetColumn("ClassW"), searchIndexHash, 200);
                ClassX.SearchIndexes(ss.GetColumn("ClassX"), searchIndexHash, 200);
                ClassY.SearchIndexes(ss.GetColumn("ClassY"), searchIndexHash, 200);
                ClassZ.SearchIndexes(ss.GetColumn("ClassZ"), searchIndexHash, 200);
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
            SiteSettings ss, 
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            bool paramAll = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            statements.OnCreatingExtendedSqls(SiteId, IssueId);
            CreateStatements(statements, ss, tableType, param, paramAll);
            statements.CreatePermissions(ss, ss.Columns
                .Where(o => o.UserColumn)
                .ToDictionary(o =>
                    o.ColumnName,
                    o => SiteInfo.User(PropertyValue(o.ColumnName).ToInt())));
            statements.OnCreatedExtendedSqls(SiteId, IssueId);
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            IssueId = newId != 0 ? newId : IssueId;
            if (synchronizeSummary) SynchronizeSummary(ss, forceSynchronizeSourceSummary);
            if (Contract.Notice() && notice)
            {
                SetTitle(ss);
                CheckNotificationConditions(ss);
                Notice(ss, "Created");
            }
            if (get) Get(ss);
            var fullText = FullText(ss, onCreating: true);
            Rds.ExecuteNonQuery(
                rdsUser: rdsUser,
                statements: Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(Title.DisplayValue)
                        .FullText(fullText, _using: fullText != null)
                        .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                    where: Rds.ItemsWhere().ReferenceId(IssueId)));
            Libraries.Search.Indexes.Create(ss, this);
            if (ss.PermissionForCreating != null) ss.SetPermissions(IssueId);
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            List<SqlStatement> statements,
            SiteSettings ss, 
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertItems(
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Issues")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.InsertIssues(
                    tableType: tableType,
                    param: param ?? Rds.IssuesParamDefault(
                        this, setDefault: true, paramAll: paramAll)),
                    InsertLinks(ss, selectIdentity: true),
            });
            return statements;
        }

        public Error.Types Update(
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool paramAll = false,
            bool get = true)
        {
            if (Contract.Notice() && notice)
            {
                CheckNotificationConditions(ss, before: true);
            }
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            statements.OnUpdatingExtendedSqls(SiteId, IssueId, timestamp);
            UpdateStatements(statements, timestamp, param, paramAll, additionalStatements);
            if (permissionChanged)
            {
                statements.UpdatePermissions(ss, IssueId, permissions);
            }
            statements.OnUpdatedExtendedSqls(SiteId, IssueId);
            var count = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            if (count == 0) return Error.Types.UpdateConflicts;
            if (synchronizeSummary) SynchronizeSummary(ss, forceSynchronizeSourceSummary);
            if (Contract.Notice() && notice)
            {
                CheckNotificationConditions(ss);
                Notice(ss, "Updated");
            }
            if (get) Get(ss);
            UpdateRelatedRecords(ss);
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool paramAll = false,
            List<SqlStatement> additionalStatements = null)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateIssues(
                    verUp: VerUp,
                    where: Rds.IssuesWhereDefault(this)
                        .UpdatedTime(timestamp, _using: timestamp.InRange()),
                    param: param ?? Rds.IssuesParamDefault(this, paramAll: paramAll),
                    countRecord: true)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        public void UpdateRelatedRecords(
            SiteSettings ss, 
            RdsUser rdsUser = null,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            var fullText = FullText(ss);
            Rds.ExecuteNonQuery(
                rdsUser: rdsUser,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(IssueId),
                        param: Rds.ItemsParam()
                            .SiteId(SiteId)
                            .Title(Title.DisplayValue)
                            .FullText(fullText, _using: fullText != null)
                            .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                        addUpdatedTimeParam: addUpdatedTimeParam,
                        addUpdatorParam: addUpdatorParam,
                        _using: updateItems),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(IssueId)),
                    InsertLinks(ss)
                });
            if (ss.Sources?.Any() == true)
            {
                ItemUtilities.UpdateTitles(SiteId, IssueId);
            }
            Libraries.Search.Indexes.Create(ss, this);
        }

        private SqlInsert InsertLinks(SiteSettings ss, bool selectIdentity = false)
        {
            var link = new Dictionary<long, long>();
            ss.Columns.Where(o => o.Link.ToBool()).ForEach(column =>
            {
                switch (column.Name)
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
            SiteSettings ss, 
            RdsUser rdsUser = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Issues")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertIssues(
                    selectIdentity: true,
                    where: where ?? Rds.IssuesWhereDefault(this),
                    param: param ?? Rds.IssuesParamDefault(this, setDefault: true))
            };
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            IssueId = newId != 0 ? newId : IssueId;
            Get(ss);
            Libraries.Search.Indexes.Create(ss, this);
            return Error.Types.None;
        }

        public Error.Types Move(SiteSettings ss, long siteId)
        {
            SiteId = siteId;
            var fullText = FullText(ss);
            Rds.ExecuteNonQuery(statements: new SqlStatement[]
            {
                Rds.UpdateItems(
                    where: Rds.ItemsWhere().ReferenceId(IssueId),
                    param: Rds.ItemsParam()
                        .SiteId(SiteId)
                        .FullText(fullText, _using: fullText != null)),
                Rds.UpdateIssues(
                    where: Rds.IssuesWhere().IssueId(IssueId),
                    param: Rds.IssuesParam().SiteId(SiteId))
            });
            SynchronizeSummary(ss);
            Get(ss);
            Libraries.Search.Indexes.Create(ss, this);
            return Error.Types.None;
        }

        public Error.Types Delete(SiteSettings ss, bool notice = false)
        {
            if (Contract.Notice() && notice)
            {
                CheckNotificationConditions(ss, before: true);
            }
            var statements = new List<SqlStatement>();
            statements.OnDeletingExtendedSqls(SiteId, IssueId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteItems(
                    where: Rds.ItemsWhere().ReferenceId(IssueId)),
                Rds.PhysicalDeleteLinks(
                    where: Rds.LinksWhere()
                        .Or(or: Rds.LinksWhere()
                            .DestinationId(IssueId)
                            .SourceId(IssueId))),
                Rds.DeleteIssues(
                    where: Rds.IssuesWhere().SiteId(SiteId).IssueId(IssueId))
            });
            statements.OnDeletedExtendedSqls(SiteId, IssueId);
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            SynchronizeSummary(ss);
            if (Contract.Notice() && notice)
            {
                CheckNotificationConditions(ss);
                Notice(ss, "Deleted");
            }
            Libraries.Search.Indexes.Create(ss, this);
            return Error.Types.None;
        }

        public Error.Types Restore(SiteSettings ss, long issueId)
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
            Libraries.Search.Indexes.Create(ss, this);
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            SiteSettings ss, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteIssues(
                    tableType: tableType,
                    param: Rds.IssuesParam().SiteId(SiteId).IssueId(IssueId)));
            Libraries.Search.Indexes.Create(ss, this);
            return Error.Types.None;
        }

        public void SetByForm(SiteSettings ss)
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Issues_Title": Title = new Title(IssueId, Forms.Data(controlId)); break;
                    case "Issues_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Issues_StartTime": StartTime = Forms.DateTime(controlId).ToUniversal(); ProgressRate.StartTime = StartTime; break;
                    case "Issues_CompletionTime": CompletionTime = new CompletionTime(Forms.Data(controlId).ToDateTime(), Status, byForm: true); ProgressRate.CompletionTime = CompletionTime.Value; break;
                    case "Issues_WorkValue": WorkValue = new WorkValue(ss.GetColumn("WorkValue").Round(Forms.Decimal(controlId)), ProgressRate.Value); break;
                    case "Issues_ProgressRate": ProgressRate = new ProgressRate(CreatedTime, StartTime, CompletionTime, ss.GetColumn("ProgressRate").Round(Forms.Decimal(controlId))); WorkValue.ProgressRate = ProgressRate.Value; break;
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
                    case "Issues_NumA": NumA = ss.GetColumn("NumA").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumB": NumB = ss.GetColumn("NumB").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumC": NumC = ss.GetColumn("NumC").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumD": NumD = ss.GetColumn("NumD").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumE": NumE = ss.GetColumn("NumE").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumF": NumF = ss.GetColumn("NumF").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumG": NumG = ss.GetColumn("NumG").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumH": NumH = ss.GetColumn("NumH").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumI": NumI = ss.GetColumn("NumI").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumJ": NumJ = ss.GetColumn("NumJ").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumK": NumK = ss.GetColumn("NumK").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumL": NumL = ss.GetColumn("NumL").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumM": NumM = ss.GetColumn("NumM").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumN": NumN = ss.GetColumn("NumN").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumO": NumO = ss.GetColumn("NumO").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumP": NumP = ss.GetColumn("NumP").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumQ": NumQ = ss.GetColumn("NumQ").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumR": NumR = ss.GetColumn("NumR").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumS": NumS = ss.GetColumn("NumS").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumT": NumT = ss.GetColumn("NumT").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumU": NumU = ss.GetColumn("NumU").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumV": NumV = ss.GetColumn("NumV").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumW": NumW = ss.GetColumn("NumW").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumX": NumX = ss.GetColumn("NumX").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumY": NumY = ss.GetColumn("NumY").Round(Forms.Decimal(controlId)); break;
                    case "Issues_NumZ": NumZ = ss.GetColumn("NumZ").Round(Forms.Decimal(controlId)); break;
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
                    case "Issues_AttachmentsA": AttachmentsA = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsB": AttachmentsB = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsC": AttachmentsC = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsD": AttachmentsD = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsE": AttachmentsE = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsF": AttachmentsF = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsG": AttachmentsG = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsH": AttachmentsH = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsI": AttachmentsI = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsJ": AttachmentsJ = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsK": AttachmentsK = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsL": AttachmentsL = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsM": AttachmentsM = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsN": AttachmentsN = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsO": AttachmentsO = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsP": AttachmentsP = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsQ": AttachmentsQ = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsR": AttachmentsR = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsS": AttachmentsS = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsT": AttachmentsT = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsU": AttachmentsU = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsV": AttachmentsV = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsW": AttachmentsW = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsX": AttachmentsX = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsY": AttachmentsY = new Attachments(Forms.List(controlId)); break;
                    case "Issues_AttachmentsZ": AttachmentsZ = new Attachments(Forms.List(controlId)); break;
                    case "Issues_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                controlId.Substring("Comment".Length).ToInt(),
                                Forms.Data(controlId));
                        }
                        break;
                }
            });
            SetTitle(ss);
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
            SetByFormula(ss);
        }

        private void SynchronizeSummary(
            SiteSettings ss, bool forceSynchronizeSourceSummary = false)
        {
            ss.Summaries.ForEach(summary =>
            {
                var id = SynchronizeSummaryDestinationId(summary.LinkColumn);
                var savedId = SynchronizeSummaryDestinationId(summary.LinkColumn, saved: true);
                if (id != 0)
                {
                    SynchronizeSummary(ss, summary, id);
                }
                if (savedId != 0 && id != savedId)
                {
                    SynchronizeSummary(ss, summary, savedId);
                }
            });
            SynchronizeSourceSummary(ss, forceSynchronizeSourceSummary);
        }

        private void SynchronizeSummary(SiteSettings ss, Summary summary, long id)
        {
            var destinationSs = SiteSettingsUtilities.Get(summary.SiteId);
            if (destinationSs != null)
            {
                Summaries.Synchronize(
                    ss,
                    destinationSs,
                    summary.SiteId,
                    summary.DestinationReferenceType,
                    summary.DestinationColumn,
                    destinationSs.Views?.Get(summary.DestinationCondition),
                    summary.SetZeroWhenOutOfCondition == true,
                    SiteId,
                    "Issues",
                    summary.LinkColumn,
                    summary.Type,
                    summary.SourceColumn,
                    ss.Views?.Get(summary.SourceCondition),
                    id);
            }
        }

        private void SynchronizeSourceSummary(SiteSettings ss, bool force = false)
        {
            ss.Sources.ForEach(sourceSs =>
                sourceSs.Summaries
                    .Where(o => ss.Views?.Get(o.DestinationCondition) != null || force)
                    .ForEach(summary =>
                        Summaries.Synchronize(
                            sourceSs,
                            ss,
                            summary.SiteId,
                            summary.DestinationReferenceType,
                            summary.DestinationColumn,
                            ss.Views?.Get(summary.DestinationCondition),
                            summary.SetZeroWhenOutOfCondition == true,
                            sourceSs.SiteId,
                            sourceSs.ReferenceType,
                            summary.LinkColumn,
                            summary.Type,
                            summary.SourceColumn,
                            sourceSs.Views?.Get(summary.SourceCondition),
                            IssueId)));
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

        public void UpdateFormulaColumns(SiteSettings ss, IEnumerable<int> selected = null)
        {
            SetByFormula(ss);
            var param = Rds.IssuesParam();
            ss.Formulas?
                .Where(o => selected == null || selected.Contains(o.Id))
                .ForEach(formulaSet =>
                {
                    switch (formulaSet.Target)
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

        public void SetByFormula(SiteSettings ss)
        {
            ss.Formulas?.ForEach(formulaSet =>
            {
                var columnName = formulaSet.Target;
                var formula = formulaSet.Formula;
                var view = ss.Views?.Get(formulaSet.Condition);
                if (view != null && !Matched(ss, view))
                {
                    if (formulaSet.OutOfCondition != null)
                    {
                        formula = formulaSet.OutOfCondition;
                    }
                    else
                    {
                        return;
                    }
                }
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
                switch (columnName)
                {
                    case "WorkValue":
                        WorkValue.Value = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumA":
                        NumA = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumB":
                        NumB = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumC":
                        NumC = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumD":
                        NumD = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumE":
                        NumE = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumF":
                        NumF = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumG":
                        NumG = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumH":
                        NumH = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumI":
                        NumI = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumJ":
                        NumJ = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumK":
                        NumK = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumL":
                        NumL = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumM":
                        NumM = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumN":
                        NumN = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumO":
                        NumO = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumP":
                        NumP = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumQ":
                        NumQ = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumR":
                        NumR = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumS":
                        NumS = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumT":
                        NumT = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumU":
                        NumU = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumV":
                        NumV = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumW":
                        NumW = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumX":
                        NumX = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumY":
                        NumY = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    case "NumZ":
                        NumZ = formula?.GetResult(
                            data, ss.GetColumn(columnName)) ?? 0;
                        break;
                    default: break;
                }
            });
        }

        public void SetTitle(SiteSettings ss)
        {
            Title = new Title(ss, IssueId, PropertyValues(ss.TitleColumns));
        }

        private bool Matched(SiteSettings ss, View view)
        {
            if (view.Incomplete == true && !Status.Incomplete())
            {
                return false;
            }
            var userId = Sessions.UserId();
            if (view.Own == true && !(Manager.Id == userId || Owner.Id == userId))
            {
                return false;
            }
            if (view.NearCompletionTime == true && !CompletionTime.Near(ss))
            {
                return false;
            }
            if (view.Delay == true && !ProgressRate.Delay(Status))
            {
                return false;
            }
            if (view.Overdue == true && CompletionTime.Overdue())
            {
                return false;
            }
            if (view.ColumnFilterHash != null)
            {
                foreach (var filter in view.ColumnFilterHash)
                {
                    var match = true;
                    var column = ss.GetColumn(filter.Key);
                    switch (filter.Key)
                    {
                        case "UpdatedTime": match = UpdatedTime.Value.Matched(column, filter.Value); break;
                        case "StartTime": match = StartTime.Matched(column, filter.Value); break;
                        case "CompletionTime": match = CompletionTime.Value.Matched(column, filter.Value); break;
                        case "WorkValue": match = WorkValue.Value.Matched(column, filter.Value); break;
                        case "ProgressRate": match = ProgressRate.Value.Matched(column, filter.Value); break;
                        case "Status": match = Status.Value.Matched(column, filter.Value); break;
                        case "Manager": match = Manager.Id.Matched(column, filter.Value); break;
                        case "Owner": match = Owner.Id.Matched(column, filter.Value); break;
                        case "ClassA": match = ClassA.Matched(column, filter.Value); break;
                        case "ClassB": match = ClassB.Matched(column, filter.Value); break;
                        case "ClassC": match = ClassC.Matched(column, filter.Value); break;
                        case "ClassD": match = ClassD.Matched(column, filter.Value); break;
                        case "ClassE": match = ClassE.Matched(column, filter.Value); break;
                        case "ClassF": match = ClassF.Matched(column, filter.Value); break;
                        case "ClassG": match = ClassG.Matched(column, filter.Value); break;
                        case "ClassH": match = ClassH.Matched(column, filter.Value); break;
                        case "ClassI": match = ClassI.Matched(column, filter.Value); break;
                        case "ClassJ": match = ClassJ.Matched(column, filter.Value); break;
                        case "ClassK": match = ClassK.Matched(column, filter.Value); break;
                        case "ClassL": match = ClassL.Matched(column, filter.Value); break;
                        case "ClassM": match = ClassM.Matched(column, filter.Value); break;
                        case "ClassN": match = ClassN.Matched(column, filter.Value); break;
                        case "ClassO": match = ClassO.Matched(column, filter.Value); break;
                        case "ClassP": match = ClassP.Matched(column, filter.Value); break;
                        case "ClassQ": match = ClassQ.Matched(column, filter.Value); break;
                        case "ClassR": match = ClassR.Matched(column, filter.Value); break;
                        case "ClassS": match = ClassS.Matched(column, filter.Value); break;
                        case "ClassT": match = ClassT.Matched(column, filter.Value); break;
                        case "ClassU": match = ClassU.Matched(column, filter.Value); break;
                        case "ClassV": match = ClassV.Matched(column, filter.Value); break;
                        case "ClassW": match = ClassW.Matched(column, filter.Value); break;
                        case "ClassX": match = ClassX.Matched(column, filter.Value); break;
                        case "ClassY": match = ClassY.Matched(column, filter.Value); break;
                        case "ClassZ": match = ClassZ.Matched(column, filter.Value); break;
                        case "NumA": match = NumA.Matched(column, filter.Value); break;
                        case "NumB": match = NumB.Matched(column, filter.Value); break;
                        case "NumC": match = NumC.Matched(column, filter.Value); break;
                        case "NumD": match = NumD.Matched(column, filter.Value); break;
                        case "NumE": match = NumE.Matched(column, filter.Value); break;
                        case "NumF": match = NumF.Matched(column, filter.Value); break;
                        case "NumG": match = NumG.Matched(column, filter.Value); break;
                        case "NumH": match = NumH.Matched(column, filter.Value); break;
                        case "NumI": match = NumI.Matched(column, filter.Value); break;
                        case "NumJ": match = NumJ.Matched(column, filter.Value); break;
                        case "NumK": match = NumK.Matched(column, filter.Value); break;
                        case "NumL": match = NumL.Matched(column, filter.Value); break;
                        case "NumM": match = NumM.Matched(column, filter.Value); break;
                        case "NumN": match = NumN.Matched(column, filter.Value); break;
                        case "NumO": match = NumO.Matched(column, filter.Value); break;
                        case "NumP": match = NumP.Matched(column, filter.Value); break;
                        case "NumQ": match = NumQ.Matched(column, filter.Value); break;
                        case "NumR": match = NumR.Matched(column, filter.Value); break;
                        case "NumS": match = NumS.Matched(column, filter.Value); break;
                        case "NumT": match = NumT.Matched(column, filter.Value); break;
                        case "NumU": match = NumU.Matched(column, filter.Value); break;
                        case "NumV": match = NumV.Matched(column, filter.Value); break;
                        case "NumW": match = NumW.Matched(column, filter.Value); break;
                        case "NumX": match = NumX.Matched(column, filter.Value); break;
                        case "NumY": match = NumY.Matched(column, filter.Value); break;
                        case "NumZ": match = NumZ.Matched(column, filter.Value); break;
                        case "DateA": match = DateA.Matched(column, filter.Value); break;
                        case "DateB": match = DateB.Matched(column, filter.Value); break;
                        case "DateC": match = DateC.Matched(column, filter.Value); break;
                        case "DateD": match = DateD.Matched(column, filter.Value); break;
                        case "DateE": match = DateE.Matched(column, filter.Value); break;
                        case "DateF": match = DateF.Matched(column, filter.Value); break;
                        case "DateG": match = DateG.Matched(column, filter.Value); break;
                        case "DateH": match = DateH.Matched(column, filter.Value); break;
                        case "DateI": match = DateI.Matched(column, filter.Value); break;
                        case "DateJ": match = DateJ.Matched(column, filter.Value); break;
                        case "DateK": match = DateK.Matched(column, filter.Value); break;
                        case "DateL": match = DateL.Matched(column, filter.Value); break;
                        case "DateM": match = DateM.Matched(column, filter.Value); break;
                        case "DateN": match = DateN.Matched(column, filter.Value); break;
                        case "DateO": match = DateO.Matched(column, filter.Value); break;
                        case "DateP": match = DateP.Matched(column, filter.Value); break;
                        case "DateQ": match = DateQ.Matched(column, filter.Value); break;
                        case "DateR": match = DateR.Matched(column, filter.Value); break;
                        case "DateS": match = DateS.Matched(column, filter.Value); break;
                        case "DateT": match = DateT.Matched(column, filter.Value); break;
                        case "DateU": match = DateU.Matched(column, filter.Value); break;
                        case "DateV": match = DateV.Matched(column, filter.Value); break;
                        case "DateW": match = DateW.Matched(column, filter.Value); break;
                        case "DateX": match = DateX.Matched(column, filter.Value); break;
                        case "DateY": match = DateY.Matched(column, filter.Value); break;
                        case "DateZ": match = DateZ.Matched(column, filter.Value); break;
                        case "CheckA": match = CheckA.Matched(column, filter.Value); break;
                        case "CheckB": match = CheckB.Matched(column, filter.Value); break;
                        case "CheckC": match = CheckC.Matched(column, filter.Value); break;
                        case "CheckD": match = CheckD.Matched(column, filter.Value); break;
                        case "CheckE": match = CheckE.Matched(column, filter.Value); break;
                        case "CheckF": match = CheckF.Matched(column, filter.Value); break;
                        case "CheckG": match = CheckG.Matched(column, filter.Value); break;
                        case "CheckH": match = CheckH.Matched(column, filter.Value); break;
                        case "CheckI": match = CheckI.Matched(column, filter.Value); break;
                        case "CheckJ": match = CheckJ.Matched(column, filter.Value); break;
                        case "CheckK": match = CheckK.Matched(column, filter.Value); break;
                        case "CheckL": match = CheckL.Matched(column, filter.Value); break;
                        case "CheckM": match = CheckM.Matched(column, filter.Value); break;
                        case "CheckN": match = CheckN.Matched(column, filter.Value); break;
                        case "CheckO": match = CheckO.Matched(column, filter.Value); break;
                        case "CheckP": match = CheckP.Matched(column, filter.Value); break;
                        case "CheckQ": match = CheckQ.Matched(column, filter.Value); break;
                        case "CheckR": match = CheckR.Matched(column, filter.Value); break;
                        case "CheckS": match = CheckS.Matched(column, filter.Value); break;
                        case "CheckT": match = CheckT.Matched(column, filter.Value); break;
                        case "CheckU": match = CheckU.Matched(column, filter.Value); break;
                        case "CheckV": match = CheckV.Matched(column, filter.Value); break;
                        case "CheckW": match = CheckW.Matched(column, filter.Value); break;
                        case "CheckX": match = CheckX.Matched(column, filter.Value); break;
                        case "CheckY": match = CheckY.Matched(column, filter.Value); break;
                        case "CheckZ": match = CheckZ.Matched(column, filter.Value); break;
                        case "SiteTitle": match = SiteTitle.SiteId.Matched(column, filter.Value); break;
                        case "CreatedTime": match = CreatedTime.Value.Matched(column, filter.Value); break;
                    }
                    if (!match) return false;
                }
            }
            return true;
        }

        private void CheckNotificationConditions(SiteSettings ss, bool before = false)
        {
            if (ss.Notifications.Any())
            {
                ss.Notifications?.CheckConditions(
                    views: ss.Views,
                    before: before,
                    dataSet: Rds.ExecuteDataSet(statements:
                        ss.Notifications.Select((o, i) =>
                            Rds.SelectIssues(
                                column: Rds.IssuesColumn().IssueId(),
                                where: ss.Views?.Get(before
                                    ? o.BeforeCondition
                                    : o.AfterCondition)?
                                        .Where(
                                            ss,
                                            Rds.IssuesWhere().IssueId(IssueId)) ??
                                                Rds.IssuesWhere().IssueId(IssueId)))
                                                    .ToArray()));
            }
        }

        private void Notice(SiteSettings ss, string type)
        {
            var url = Locations.ItemEditAbsoluteUri(IssueId);
            ss.Notifications.Where(o => o.Enabled).ForEach(notification =>
            {
                if (notification.HasRelatedUsers())
                {
                    var users = new List<long>();
                    Rds.ExecuteTable(statements: Rds.SelectIssues(
                        tableType: Sqls.TableTypes.All,
                        distinct: true,
                        column: Rds.IssuesColumn()
                            .Manager()
                            .Owner()
                            .Creator()
                            .Updator(),
                        where: Rds.IssuesWhere().IssueId(IssueId)))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                            {
                                users.Add(dataRow.Long("Manager"));
                                users.Add(dataRow.Long("Owner"));
                                users.Add(dataRow.Long("Creator"));
                                users.Add(dataRow.Long("Updator"));
                            });
                    notification.ReplaceRelatedUsers(users);
                }
                switch (type)
                {
                    case "Created":
                        notification.Send(
                            Displays.Created(Title.DisplayValue).ToString(),
                            url,
                            NoticeBody(ss, notification));
                        break;
                    case "Updated":
                        var body = NoticeBody(ss, notification, update: true);
                        if (body.Length > 0)
                        {
                            notification.Send(
                                Displays.Updated(Title.DisplayValue).ToString(),
                                url,
                                body);
                        }
                        break;
                    case "Deleted":
                        notification.Send(
                            Displays.Deleted(Title.DisplayValue).ToString(),
                            url,
                            NoticeBody(ss, notification));
                        break;
                }
            });
        }

        private string NoticeBody(SiteSettings ss, Notification notification, bool update = false)
        {
            var body = new System.Text.StringBuilder();
            notification.ColumnCollection(ss, update)?.ForEach(column =>
            {
                switch (column.Name)
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

        private void Set(SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
            var links = ss.GetUseSearchLinks(titleOnly: true);
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: new List<string>
                    {
                        PropertyValue(link.ColumnName)
                    }));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                SetTitle(ss);
            }
        }

        private void Set(SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SiteId = dataRow[column.ColumnName].ToLong();
                                SavedSiteId = SiteId;
                            }
                            break;
                        case "UpdatedTime":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                                SavedUpdatedTime = UpdatedTime.Value;
                            }
                            break;
                        case "IssueId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                IssueId = dataRow[column.ColumnName].ToLong();
                                SavedIssueId = IssueId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Title":
                            Title = new Title(ss, dataRow, column);
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "StartTime":
                            StartTime = dataRow[column.ColumnName].ToDateTime();
                            SavedStartTime = StartTime;
                            break;
                        case "CompletionTime":
                            CompletionTime = new CompletionTime(dataRow, column);
                            SavedCompletionTime = CompletionTime.Value;
                            break;
                        case "WorkValue":
                            WorkValue = new WorkValue(dataRow, column);
                            SavedWorkValue = WorkValue.Value;
                            break;
                        case "ProgressRate":
                            ProgressRate = new ProgressRate(dataRow, column);
                            SavedProgressRate = ProgressRate.Value;
                            break;
                        case "RemainingWorkValue":
                            RemainingWorkValue = dataRow[column.ColumnName].ToDecimal();
                            SavedRemainingWorkValue = RemainingWorkValue;
                            break;
                        case "Status":
                            Status = new Status(dataRow, column);
                            SavedStatus = Status.Value;
                            break;
                        case "Manager":
                            Manager = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedManager = Manager.Id;
                            break;
                        case "Owner":
                            Owner = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedOwner = Owner.Id;
                            break;
                        case "ClassA":
                            ClassA = dataRow[column.ColumnName].ToString();
                            SavedClassA = ClassA;
                            break;
                        case "ClassB":
                            ClassB = dataRow[column.ColumnName].ToString();
                            SavedClassB = ClassB;
                            break;
                        case "ClassC":
                            ClassC = dataRow[column.ColumnName].ToString();
                            SavedClassC = ClassC;
                            break;
                        case "ClassD":
                            ClassD = dataRow[column.ColumnName].ToString();
                            SavedClassD = ClassD;
                            break;
                        case "ClassE":
                            ClassE = dataRow[column.ColumnName].ToString();
                            SavedClassE = ClassE;
                            break;
                        case "ClassF":
                            ClassF = dataRow[column.ColumnName].ToString();
                            SavedClassF = ClassF;
                            break;
                        case "ClassG":
                            ClassG = dataRow[column.ColumnName].ToString();
                            SavedClassG = ClassG;
                            break;
                        case "ClassH":
                            ClassH = dataRow[column.ColumnName].ToString();
                            SavedClassH = ClassH;
                            break;
                        case "ClassI":
                            ClassI = dataRow[column.ColumnName].ToString();
                            SavedClassI = ClassI;
                            break;
                        case "ClassJ":
                            ClassJ = dataRow[column.ColumnName].ToString();
                            SavedClassJ = ClassJ;
                            break;
                        case "ClassK":
                            ClassK = dataRow[column.ColumnName].ToString();
                            SavedClassK = ClassK;
                            break;
                        case "ClassL":
                            ClassL = dataRow[column.ColumnName].ToString();
                            SavedClassL = ClassL;
                            break;
                        case "ClassM":
                            ClassM = dataRow[column.ColumnName].ToString();
                            SavedClassM = ClassM;
                            break;
                        case "ClassN":
                            ClassN = dataRow[column.ColumnName].ToString();
                            SavedClassN = ClassN;
                            break;
                        case "ClassO":
                            ClassO = dataRow[column.ColumnName].ToString();
                            SavedClassO = ClassO;
                            break;
                        case "ClassP":
                            ClassP = dataRow[column.ColumnName].ToString();
                            SavedClassP = ClassP;
                            break;
                        case "ClassQ":
                            ClassQ = dataRow[column.ColumnName].ToString();
                            SavedClassQ = ClassQ;
                            break;
                        case "ClassR":
                            ClassR = dataRow[column.ColumnName].ToString();
                            SavedClassR = ClassR;
                            break;
                        case "ClassS":
                            ClassS = dataRow[column.ColumnName].ToString();
                            SavedClassS = ClassS;
                            break;
                        case "ClassT":
                            ClassT = dataRow[column.ColumnName].ToString();
                            SavedClassT = ClassT;
                            break;
                        case "ClassU":
                            ClassU = dataRow[column.ColumnName].ToString();
                            SavedClassU = ClassU;
                            break;
                        case "ClassV":
                            ClassV = dataRow[column.ColumnName].ToString();
                            SavedClassV = ClassV;
                            break;
                        case "ClassW":
                            ClassW = dataRow[column.ColumnName].ToString();
                            SavedClassW = ClassW;
                            break;
                        case "ClassX":
                            ClassX = dataRow[column.ColumnName].ToString();
                            SavedClassX = ClassX;
                            break;
                        case "ClassY":
                            ClassY = dataRow[column.ColumnName].ToString();
                            SavedClassY = ClassY;
                            break;
                        case "ClassZ":
                            ClassZ = dataRow[column.ColumnName].ToString();
                            SavedClassZ = ClassZ;
                            break;
                        case "NumA":
                            NumA = dataRow[column.ColumnName].ToDecimal();
                            SavedNumA = NumA;
                            break;
                        case "NumB":
                            NumB = dataRow[column.ColumnName].ToDecimal();
                            SavedNumB = NumB;
                            break;
                        case "NumC":
                            NumC = dataRow[column.ColumnName].ToDecimal();
                            SavedNumC = NumC;
                            break;
                        case "NumD":
                            NumD = dataRow[column.ColumnName].ToDecimal();
                            SavedNumD = NumD;
                            break;
                        case "NumE":
                            NumE = dataRow[column.ColumnName].ToDecimal();
                            SavedNumE = NumE;
                            break;
                        case "NumF":
                            NumF = dataRow[column.ColumnName].ToDecimal();
                            SavedNumF = NumF;
                            break;
                        case "NumG":
                            NumG = dataRow[column.ColumnName].ToDecimal();
                            SavedNumG = NumG;
                            break;
                        case "NumH":
                            NumH = dataRow[column.ColumnName].ToDecimal();
                            SavedNumH = NumH;
                            break;
                        case "NumI":
                            NumI = dataRow[column.ColumnName].ToDecimal();
                            SavedNumI = NumI;
                            break;
                        case "NumJ":
                            NumJ = dataRow[column.ColumnName].ToDecimal();
                            SavedNumJ = NumJ;
                            break;
                        case "NumK":
                            NumK = dataRow[column.ColumnName].ToDecimal();
                            SavedNumK = NumK;
                            break;
                        case "NumL":
                            NumL = dataRow[column.ColumnName].ToDecimal();
                            SavedNumL = NumL;
                            break;
                        case "NumM":
                            NumM = dataRow[column.ColumnName].ToDecimal();
                            SavedNumM = NumM;
                            break;
                        case "NumN":
                            NumN = dataRow[column.ColumnName].ToDecimal();
                            SavedNumN = NumN;
                            break;
                        case "NumO":
                            NumO = dataRow[column.ColumnName].ToDecimal();
                            SavedNumO = NumO;
                            break;
                        case "NumP":
                            NumP = dataRow[column.ColumnName].ToDecimal();
                            SavedNumP = NumP;
                            break;
                        case "NumQ":
                            NumQ = dataRow[column.ColumnName].ToDecimal();
                            SavedNumQ = NumQ;
                            break;
                        case "NumR":
                            NumR = dataRow[column.ColumnName].ToDecimal();
                            SavedNumR = NumR;
                            break;
                        case "NumS":
                            NumS = dataRow[column.ColumnName].ToDecimal();
                            SavedNumS = NumS;
                            break;
                        case "NumT":
                            NumT = dataRow[column.ColumnName].ToDecimal();
                            SavedNumT = NumT;
                            break;
                        case "NumU":
                            NumU = dataRow[column.ColumnName].ToDecimal();
                            SavedNumU = NumU;
                            break;
                        case "NumV":
                            NumV = dataRow[column.ColumnName].ToDecimal();
                            SavedNumV = NumV;
                            break;
                        case "NumW":
                            NumW = dataRow[column.ColumnName].ToDecimal();
                            SavedNumW = NumW;
                            break;
                        case "NumX":
                            NumX = dataRow[column.ColumnName].ToDecimal();
                            SavedNumX = NumX;
                            break;
                        case "NumY":
                            NumY = dataRow[column.ColumnName].ToDecimal();
                            SavedNumY = NumY;
                            break;
                        case "NumZ":
                            NumZ = dataRow[column.ColumnName].ToDecimal();
                            SavedNumZ = NumZ;
                            break;
                        case "DateA":
                            DateA = dataRow[column.ColumnName].ToDateTime();
                            SavedDateA = DateA;
                            break;
                        case "DateB":
                            DateB = dataRow[column.ColumnName].ToDateTime();
                            SavedDateB = DateB;
                            break;
                        case "DateC":
                            DateC = dataRow[column.ColumnName].ToDateTime();
                            SavedDateC = DateC;
                            break;
                        case "DateD":
                            DateD = dataRow[column.ColumnName].ToDateTime();
                            SavedDateD = DateD;
                            break;
                        case "DateE":
                            DateE = dataRow[column.ColumnName].ToDateTime();
                            SavedDateE = DateE;
                            break;
                        case "DateF":
                            DateF = dataRow[column.ColumnName].ToDateTime();
                            SavedDateF = DateF;
                            break;
                        case "DateG":
                            DateG = dataRow[column.ColumnName].ToDateTime();
                            SavedDateG = DateG;
                            break;
                        case "DateH":
                            DateH = dataRow[column.ColumnName].ToDateTime();
                            SavedDateH = DateH;
                            break;
                        case "DateI":
                            DateI = dataRow[column.ColumnName].ToDateTime();
                            SavedDateI = DateI;
                            break;
                        case "DateJ":
                            DateJ = dataRow[column.ColumnName].ToDateTime();
                            SavedDateJ = DateJ;
                            break;
                        case "DateK":
                            DateK = dataRow[column.ColumnName].ToDateTime();
                            SavedDateK = DateK;
                            break;
                        case "DateL":
                            DateL = dataRow[column.ColumnName].ToDateTime();
                            SavedDateL = DateL;
                            break;
                        case "DateM":
                            DateM = dataRow[column.ColumnName].ToDateTime();
                            SavedDateM = DateM;
                            break;
                        case "DateN":
                            DateN = dataRow[column.ColumnName].ToDateTime();
                            SavedDateN = DateN;
                            break;
                        case "DateO":
                            DateO = dataRow[column.ColumnName].ToDateTime();
                            SavedDateO = DateO;
                            break;
                        case "DateP":
                            DateP = dataRow[column.ColumnName].ToDateTime();
                            SavedDateP = DateP;
                            break;
                        case "DateQ":
                            DateQ = dataRow[column.ColumnName].ToDateTime();
                            SavedDateQ = DateQ;
                            break;
                        case "DateR":
                            DateR = dataRow[column.ColumnName].ToDateTime();
                            SavedDateR = DateR;
                            break;
                        case "DateS":
                            DateS = dataRow[column.ColumnName].ToDateTime();
                            SavedDateS = DateS;
                            break;
                        case "DateT":
                            DateT = dataRow[column.ColumnName].ToDateTime();
                            SavedDateT = DateT;
                            break;
                        case "DateU":
                            DateU = dataRow[column.ColumnName].ToDateTime();
                            SavedDateU = DateU;
                            break;
                        case "DateV":
                            DateV = dataRow[column.ColumnName].ToDateTime();
                            SavedDateV = DateV;
                            break;
                        case "DateW":
                            DateW = dataRow[column.ColumnName].ToDateTime();
                            SavedDateW = DateW;
                            break;
                        case "DateX":
                            DateX = dataRow[column.ColumnName].ToDateTime();
                            SavedDateX = DateX;
                            break;
                        case "DateY":
                            DateY = dataRow[column.ColumnName].ToDateTime();
                            SavedDateY = DateY;
                            break;
                        case "DateZ":
                            DateZ = dataRow[column.ColumnName].ToDateTime();
                            SavedDateZ = DateZ;
                            break;
                        case "DescriptionA":
                            DescriptionA = dataRow[column.ColumnName].ToString();
                            SavedDescriptionA = DescriptionA;
                            break;
                        case "DescriptionB":
                            DescriptionB = dataRow[column.ColumnName].ToString();
                            SavedDescriptionB = DescriptionB;
                            break;
                        case "DescriptionC":
                            DescriptionC = dataRow[column.ColumnName].ToString();
                            SavedDescriptionC = DescriptionC;
                            break;
                        case "DescriptionD":
                            DescriptionD = dataRow[column.ColumnName].ToString();
                            SavedDescriptionD = DescriptionD;
                            break;
                        case "DescriptionE":
                            DescriptionE = dataRow[column.ColumnName].ToString();
                            SavedDescriptionE = DescriptionE;
                            break;
                        case "DescriptionF":
                            DescriptionF = dataRow[column.ColumnName].ToString();
                            SavedDescriptionF = DescriptionF;
                            break;
                        case "DescriptionG":
                            DescriptionG = dataRow[column.ColumnName].ToString();
                            SavedDescriptionG = DescriptionG;
                            break;
                        case "DescriptionH":
                            DescriptionH = dataRow[column.ColumnName].ToString();
                            SavedDescriptionH = DescriptionH;
                            break;
                        case "DescriptionI":
                            DescriptionI = dataRow[column.ColumnName].ToString();
                            SavedDescriptionI = DescriptionI;
                            break;
                        case "DescriptionJ":
                            DescriptionJ = dataRow[column.ColumnName].ToString();
                            SavedDescriptionJ = DescriptionJ;
                            break;
                        case "DescriptionK":
                            DescriptionK = dataRow[column.ColumnName].ToString();
                            SavedDescriptionK = DescriptionK;
                            break;
                        case "DescriptionL":
                            DescriptionL = dataRow[column.ColumnName].ToString();
                            SavedDescriptionL = DescriptionL;
                            break;
                        case "DescriptionM":
                            DescriptionM = dataRow[column.ColumnName].ToString();
                            SavedDescriptionM = DescriptionM;
                            break;
                        case "DescriptionN":
                            DescriptionN = dataRow[column.ColumnName].ToString();
                            SavedDescriptionN = DescriptionN;
                            break;
                        case "DescriptionO":
                            DescriptionO = dataRow[column.ColumnName].ToString();
                            SavedDescriptionO = DescriptionO;
                            break;
                        case "DescriptionP":
                            DescriptionP = dataRow[column.ColumnName].ToString();
                            SavedDescriptionP = DescriptionP;
                            break;
                        case "DescriptionQ":
                            DescriptionQ = dataRow[column.ColumnName].ToString();
                            SavedDescriptionQ = DescriptionQ;
                            break;
                        case "DescriptionR":
                            DescriptionR = dataRow[column.ColumnName].ToString();
                            SavedDescriptionR = DescriptionR;
                            break;
                        case "DescriptionS":
                            DescriptionS = dataRow[column.ColumnName].ToString();
                            SavedDescriptionS = DescriptionS;
                            break;
                        case "DescriptionT":
                            DescriptionT = dataRow[column.ColumnName].ToString();
                            SavedDescriptionT = DescriptionT;
                            break;
                        case "DescriptionU":
                            DescriptionU = dataRow[column.ColumnName].ToString();
                            SavedDescriptionU = DescriptionU;
                            break;
                        case "DescriptionV":
                            DescriptionV = dataRow[column.ColumnName].ToString();
                            SavedDescriptionV = DescriptionV;
                            break;
                        case "DescriptionW":
                            DescriptionW = dataRow[column.ColumnName].ToString();
                            SavedDescriptionW = DescriptionW;
                            break;
                        case "DescriptionX":
                            DescriptionX = dataRow[column.ColumnName].ToString();
                            SavedDescriptionX = DescriptionX;
                            break;
                        case "DescriptionY":
                            DescriptionY = dataRow[column.ColumnName].ToString();
                            SavedDescriptionY = DescriptionY;
                            break;
                        case "DescriptionZ":
                            DescriptionZ = dataRow[column.ColumnName].ToString();
                            SavedDescriptionZ = DescriptionZ;
                            break;
                        case "CheckA":
                            CheckA = dataRow[column.ColumnName].ToBool();
                            SavedCheckA = CheckA;
                            break;
                        case "CheckB":
                            CheckB = dataRow[column.ColumnName].ToBool();
                            SavedCheckB = CheckB;
                            break;
                        case "CheckC":
                            CheckC = dataRow[column.ColumnName].ToBool();
                            SavedCheckC = CheckC;
                            break;
                        case "CheckD":
                            CheckD = dataRow[column.ColumnName].ToBool();
                            SavedCheckD = CheckD;
                            break;
                        case "CheckE":
                            CheckE = dataRow[column.ColumnName].ToBool();
                            SavedCheckE = CheckE;
                            break;
                        case "CheckF":
                            CheckF = dataRow[column.ColumnName].ToBool();
                            SavedCheckF = CheckF;
                            break;
                        case "CheckG":
                            CheckG = dataRow[column.ColumnName].ToBool();
                            SavedCheckG = CheckG;
                            break;
                        case "CheckH":
                            CheckH = dataRow[column.ColumnName].ToBool();
                            SavedCheckH = CheckH;
                            break;
                        case "CheckI":
                            CheckI = dataRow[column.ColumnName].ToBool();
                            SavedCheckI = CheckI;
                            break;
                        case "CheckJ":
                            CheckJ = dataRow[column.ColumnName].ToBool();
                            SavedCheckJ = CheckJ;
                            break;
                        case "CheckK":
                            CheckK = dataRow[column.ColumnName].ToBool();
                            SavedCheckK = CheckK;
                            break;
                        case "CheckL":
                            CheckL = dataRow[column.ColumnName].ToBool();
                            SavedCheckL = CheckL;
                            break;
                        case "CheckM":
                            CheckM = dataRow[column.ColumnName].ToBool();
                            SavedCheckM = CheckM;
                            break;
                        case "CheckN":
                            CheckN = dataRow[column.ColumnName].ToBool();
                            SavedCheckN = CheckN;
                            break;
                        case "CheckO":
                            CheckO = dataRow[column.ColumnName].ToBool();
                            SavedCheckO = CheckO;
                            break;
                        case "CheckP":
                            CheckP = dataRow[column.ColumnName].ToBool();
                            SavedCheckP = CheckP;
                            break;
                        case "CheckQ":
                            CheckQ = dataRow[column.ColumnName].ToBool();
                            SavedCheckQ = CheckQ;
                            break;
                        case "CheckR":
                            CheckR = dataRow[column.ColumnName].ToBool();
                            SavedCheckR = CheckR;
                            break;
                        case "CheckS":
                            CheckS = dataRow[column.ColumnName].ToBool();
                            SavedCheckS = CheckS;
                            break;
                        case "CheckT":
                            CheckT = dataRow[column.ColumnName].ToBool();
                            SavedCheckT = CheckT;
                            break;
                        case "CheckU":
                            CheckU = dataRow[column.ColumnName].ToBool();
                            SavedCheckU = CheckU;
                            break;
                        case "CheckV":
                            CheckV = dataRow[column.ColumnName].ToBool();
                            SavedCheckV = CheckV;
                            break;
                        case "CheckW":
                            CheckW = dataRow[column.ColumnName].ToBool();
                            SavedCheckW = CheckW;
                            break;
                        case "CheckX":
                            CheckX = dataRow[column.ColumnName].ToBool();
                            SavedCheckX = CheckX;
                            break;
                        case "CheckY":
                            CheckY = dataRow[column.ColumnName].ToBool();
                            SavedCheckY = CheckY;
                            break;
                        case "CheckZ":
                            CheckZ = dataRow[column.ColumnName].ToBool();
                            SavedCheckZ = CheckZ;
                            break;
                        case "AttachmentsA":
                            AttachmentsA = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsA = AttachmentsA.ToJson();
                            break;
                        case "AttachmentsB":
                            AttachmentsB = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsB = AttachmentsB.ToJson();
                            break;
                        case "AttachmentsC":
                            AttachmentsC = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsC = AttachmentsC.ToJson();
                            break;
                        case "AttachmentsD":
                            AttachmentsD = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsD = AttachmentsD.ToJson();
                            break;
                        case "AttachmentsE":
                            AttachmentsE = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsE = AttachmentsE.ToJson();
                            break;
                        case "AttachmentsF":
                            AttachmentsF = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsF = AttachmentsF.ToJson();
                            break;
                        case "AttachmentsG":
                            AttachmentsG = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsG = AttachmentsG.ToJson();
                            break;
                        case "AttachmentsH":
                            AttachmentsH = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsH = AttachmentsH.ToJson();
                            break;
                        case "AttachmentsI":
                            AttachmentsI = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsI = AttachmentsI.ToJson();
                            break;
                        case "AttachmentsJ":
                            AttachmentsJ = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsJ = AttachmentsJ.ToJson();
                            break;
                        case "AttachmentsK":
                            AttachmentsK = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsK = AttachmentsK.ToJson();
                            break;
                        case "AttachmentsL":
                            AttachmentsL = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsL = AttachmentsL.ToJson();
                            break;
                        case "AttachmentsM":
                            AttachmentsM = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsM = AttachmentsM.ToJson();
                            break;
                        case "AttachmentsN":
                            AttachmentsN = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsN = AttachmentsN.ToJson();
                            break;
                        case "AttachmentsO":
                            AttachmentsO = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsO = AttachmentsO.ToJson();
                            break;
                        case "AttachmentsP":
                            AttachmentsP = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsP = AttachmentsP.ToJson();
                            break;
                        case "AttachmentsQ":
                            AttachmentsQ = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsQ = AttachmentsQ.ToJson();
                            break;
                        case "AttachmentsR":
                            AttachmentsR = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsR = AttachmentsR.ToJson();
                            break;
                        case "AttachmentsS":
                            AttachmentsS = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsS = AttachmentsS.ToJson();
                            break;
                        case "AttachmentsT":
                            AttachmentsT = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsT = AttachmentsT.ToJson();
                            break;
                        case "AttachmentsU":
                            AttachmentsU = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsU = AttachmentsU.ToJson();
                            break;
                        case "AttachmentsV":
                            AttachmentsV = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsV = AttachmentsV.ToJson();
                            break;
                        case "AttachmentsW":
                            AttachmentsW = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsW = AttachmentsW.ToJson();
                            break;
                        case "AttachmentsX":
                            AttachmentsX = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsX = AttachmentsX.ToJson();
                            break;
                        case "AttachmentsY":
                            AttachmentsY = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsY = AttachmentsY.ToJson();
                            break;
                        case "AttachmentsZ":
                            AttachmentsZ = dataRow[column.ColumnName].ToString().Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsZ = AttachmentsZ.ToJson();
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                SiteId_Updated ||
                UpdatedTime_Updated ||
                Ver_Updated ||
                Title_Updated ||
                Body_Updated ||
                StartTime_Updated ||
                CompletionTime_Updated ||
                WorkValue_Updated ||
                ProgressRate_Updated ||
                Status_Updated ||
                Manager_Updated ||
                Owner_Updated ||
                ClassA_Updated ||
                ClassB_Updated ||
                ClassC_Updated ||
                ClassD_Updated ||
                ClassE_Updated ||
                ClassF_Updated ||
                ClassG_Updated ||
                ClassH_Updated ||
                ClassI_Updated ||
                ClassJ_Updated ||
                ClassK_Updated ||
                ClassL_Updated ||
                ClassM_Updated ||
                ClassN_Updated ||
                ClassO_Updated ||
                ClassP_Updated ||
                ClassQ_Updated ||
                ClassR_Updated ||
                ClassS_Updated ||
                ClassT_Updated ||
                ClassU_Updated ||
                ClassV_Updated ||
                ClassW_Updated ||
                ClassX_Updated ||
                ClassY_Updated ||
                ClassZ_Updated ||
                NumA_Updated ||
                NumB_Updated ||
                NumC_Updated ||
                NumD_Updated ||
                NumE_Updated ||
                NumF_Updated ||
                NumG_Updated ||
                NumH_Updated ||
                NumI_Updated ||
                NumJ_Updated ||
                NumK_Updated ||
                NumL_Updated ||
                NumM_Updated ||
                NumN_Updated ||
                NumO_Updated ||
                NumP_Updated ||
                NumQ_Updated ||
                NumR_Updated ||
                NumS_Updated ||
                NumT_Updated ||
                NumU_Updated ||
                NumV_Updated ||
                NumW_Updated ||
                NumX_Updated ||
                NumY_Updated ||
                NumZ_Updated ||
                DateA_Updated ||
                DateB_Updated ||
                DateC_Updated ||
                DateD_Updated ||
                DateE_Updated ||
                DateF_Updated ||
                DateG_Updated ||
                DateH_Updated ||
                DateI_Updated ||
                DateJ_Updated ||
                DateK_Updated ||
                DateL_Updated ||
                DateM_Updated ||
                DateN_Updated ||
                DateO_Updated ||
                DateP_Updated ||
                DateQ_Updated ||
                DateR_Updated ||
                DateS_Updated ||
                DateT_Updated ||
                DateU_Updated ||
                DateV_Updated ||
                DateW_Updated ||
                DateX_Updated ||
                DateY_Updated ||
                DateZ_Updated ||
                DescriptionA_Updated ||
                DescriptionB_Updated ||
                DescriptionC_Updated ||
                DescriptionD_Updated ||
                DescriptionE_Updated ||
                DescriptionF_Updated ||
                DescriptionG_Updated ||
                DescriptionH_Updated ||
                DescriptionI_Updated ||
                DescriptionJ_Updated ||
                DescriptionK_Updated ||
                DescriptionL_Updated ||
                DescriptionM_Updated ||
                DescriptionN_Updated ||
                DescriptionO_Updated ||
                DescriptionP_Updated ||
                DescriptionQ_Updated ||
                DescriptionR_Updated ||
                DescriptionS_Updated ||
                DescriptionT_Updated ||
                DescriptionU_Updated ||
                DescriptionV_Updated ||
                DescriptionW_Updated ||
                DescriptionX_Updated ||
                DescriptionY_Updated ||
                DescriptionZ_Updated ||
                CheckA_Updated ||
                CheckB_Updated ||
                CheckC_Updated ||
                CheckD_Updated ||
                CheckE_Updated ||
                CheckF_Updated ||
                CheckG_Updated ||
                CheckH_Updated ||
                CheckI_Updated ||
                CheckJ_Updated ||
                CheckK_Updated ||
                CheckL_Updated ||
                CheckM_Updated ||
                CheckN_Updated ||
                CheckO_Updated ||
                CheckP_Updated ||
                CheckQ_Updated ||
                CheckR_Updated ||
                CheckS_Updated ||
                CheckT_Updated ||
                CheckU_Updated ||
                CheckV_Updated ||
                CheckW_Updated ||
                CheckX_Updated ||
                CheckY_Updated ||
                CheckZ_Updated ||
                AttachmentsA_Updated ||
                AttachmentsB_Updated ||
                AttachmentsC_Updated ||
                AttachmentsD_Updated ||
                AttachmentsE_Updated ||
                AttachmentsF_Updated ||
                AttachmentsG_Updated ||
                AttachmentsH_Updated ||
                AttachmentsI_Updated ||
                AttachmentsJ_Updated ||
                AttachmentsK_Updated ||
                AttachmentsL_Updated ||
                AttachmentsM_Updated ||
                AttachmentsN_Updated ||
                AttachmentsO_Updated ||
                AttachmentsP_Updated ||
                AttachmentsQ_Updated ||
                AttachmentsR_Updated ||
                AttachmentsS_Updated ||
                AttachmentsT_Updated ||
                AttachmentsU_Updated ||
                AttachmentsV_Updated ||
                AttachmentsW_Updated ||
                AttachmentsX_Updated ||
                AttachmentsY_Updated ||
                AttachmentsZ_Updated ||
                Comments_Updated ||
                Creator_Updated ||
                Updator_Updated ||
                CreatedTime_Updated;
        }

        public List<string> Mine()
        {
            var mine = new List<string>();
            var userId = Sessions.UserId();
            if (SavedManager == userId) mine.Add("Manager");
            if (SavedOwner == userId) mine.Add("Owner");
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }
    }
}
