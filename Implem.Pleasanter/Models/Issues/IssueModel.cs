using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
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
    [Serializable]
    public class IssueModel : BaseItemModel
    {
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

        public TitleBody TitleBody
        {
            get
            {
                return new TitleBody(IssueId, Title.Value, Title.DisplayValue, Body);
            }
        }

        public SiteTitle SiteTitle
        {
            get
            {
                return new SiteTitle(SiteId);
            }
        }

        [NonSerialized] public long SavedIssueId = 0;
        [NonSerialized] public DateTime SavedStartTime = 0.ToDateTime();
        [NonSerialized] public DateTime SavedCompletionTime = 0.ToDateTime();
        [NonSerialized] public decimal SavedWorkValue = 0;
        [NonSerialized] public decimal SavedProgressRate = 0;
        [NonSerialized] public decimal SavedRemainingWorkValue = 0;
        [NonSerialized] public int SavedStatus = 0;
        [NonSerialized] public int SavedManager = 0;
        [NonSerialized] public int SavedOwner = 0;
        [NonSerialized] public string SavedClassA = string.Empty;
        [NonSerialized] public string SavedClassB = string.Empty;
        [NonSerialized] public string SavedClassC = string.Empty;
        [NonSerialized] public string SavedClassD = string.Empty;
        [NonSerialized] public string SavedClassE = string.Empty;
        [NonSerialized] public string SavedClassF = string.Empty;
        [NonSerialized] public string SavedClassG = string.Empty;
        [NonSerialized] public string SavedClassH = string.Empty;
        [NonSerialized] public string SavedClassI = string.Empty;
        [NonSerialized] public string SavedClassJ = string.Empty;
        [NonSerialized] public string SavedClassK = string.Empty;
        [NonSerialized] public string SavedClassL = string.Empty;
        [NonSerialized] public string SavedClassM = string.Empty;
        [NonSerialized] public string SavedClassN = string.Empty;
        [NonSerialized] public string SavedClassO = string.Empty;
        [NonSerialized] public string SavedClassP = string.Empty;
        [NonSerialized] public string SavedClassQ = string.Empty;
        [NonSerialized] public string SavedClassR = string.Empty;
        [NonSerialized] public string SavedClassS = string.Empty;
        [NonSerialized] public string SavedClassT = string.Empty;
        [NonSerialized] public string SavedClassU = string.Empty;
        [NonSerialized] public string SavedClassV = string.Empty;
        [NonSerialized] public string SavedClassW = string.Empty;
        [NonSerialized] public string SavedClassX = string.Empty;
        [NonSerialized] public string SavedClassY = string.Empty;
        [NonSerialized] public string SavedClassZ = string.Empty;
        [NonSerialized] public decimal SavedNumA = 0;
        [NonSerialized] public decimal SavedNumB = 0;
        [NonSerialized] public decimal SavedNumC = 0;
        [NonSerialized] public decimal SavedNumD = 0;
        [NonSerialized] public decimal SavedNumE = 0;
        [NonSerialized] public decimal SavedNumF = 0;
        [NonSerialized] public decimal SavedNumG = 0;
        [NonSerialized] public decimal SavedNumH = 0;
        [NonSerialized] public decimal SavedNumI = 0;
        [NonSerialized] public decimal SavedNumJ = 0;
        [NonSerialized] public decimal SavedNumK = 0;
        [NonSerialized] public decimal SavedNumL = 0;
        [NonSerialized] public decimal SavedNumM = 0;
        [NonSerialized] public decimal SavedNumN = 0;
        [NonSerialized] public decimal SavedNumO = 0;
        [NonSerialized] public decimal SavedNumP = 0;
        [NonSerialized] public decimal SavedNumQ = 0;
        [NonSerialized] public decimal SavedNumR = 0;
        [NonSerialized] public decimal SavedNumS = 0;
        [NonSerialized] public decimal SavedNumT = 0;
        [NonSerialized] public decimal SavedNumU = 0;
        [NonSerialized] public decimal SavedNumV = 0;
        [NonSerialized] public decimal SavedNumW = 0;
        [NonSerialized] public decimal SavedNumX = 0;
        [NonSerialized] public decimal SavedNumY = 0;
        [NonSerialized] public decimal SavedNumZ = 0;
        [NonSerialized] public DateTime SavedDateA = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateB = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateC = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateD = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateE = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateF = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateG = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateH = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateI = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateJ = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateK = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateL = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateM = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateN = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateO = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateP = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateQ = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateR = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateS = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateT = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateU = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateV = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateW = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateX = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateY = 0.ToDateTime();
        [NonSerialized] public DateTime SavedDateZ = 0.ToDateTime();
        [NonSerialized] public string SavedDescriptionA = string.Empty;
        [NonSerialized] public string SavedDescriptionB = string.Empty;
        [NonSerialized] public string SavedDescriptionC = string.Empty;
        [NonSerialized] public string SavedDescriptionD = string.Empty;
        [NonSerialized] public string SavedDescriptionE = string.Empty;
        [NonSerialized] public string SavedDescriptionF = string.Empty;
        [NonSerialized] public string SavedDescriptionG = string.Empty;
        [NonSerialized] public string SavedDescriptionH = string.Empty;
        [NonSerialized] public string SavedDescriptionI = string.Empty;
        [NonSerialized] public string SavedDescriptionJ = string.Empty;
        [NonSerialized] public string SavedDescriptionK = string.Empty;
        [NonSerialized] public string SavedDescriptionL = string.Empty;
        [NonSerialized] public string SavedDescriptionM = string.Empty;
        [NonSerialized] public string SavedDescriptionN = string.Empty;
        [NonSerialized] public string SavedDescriptionO = string.Empty;
        [NonSerialized] public string SavedDescriptionP = string.Empty;
        [NonSerialized] public string SavedDescriptionQ = string.Empty;
        [NonSerialized] public string SavedDescriptionR = string.Empty;
        [NonSerialized] public string SavedDescriptionS = string.Empty;
        [NonSerialized] public string SavedDescriptionT = string.Empty;
        [NonSerialized] public string SavedDescriptionU = string.Empty;
        [NonSerialized] public string SavedDescriptionV = string.Empty;
        [NonSerialized] public string SavedDescriptionW = string.Empty;
        [NonSerialized] public string SavedDescriptionX = string.Empty;
        [NonSerialized] public string SavedDescriptionY = string.Empty;
        [NonSerialized] public string SavedDescriptionZ = string.Empty;
        [NonSerialized] public bool SavedCheckA = false;
        [NonSerialized] public bool SavedCheckB = false;
        [NonSerialized] public bool SavedCheckC = false;
        [NonSerialized] public bool SavedCheckD = false;
        [NonSerialized] public bool SavedCheckE = false;
        [NonSerialized] public bool SavedCheckF = false;
        [NonSerialized] public bool SavedCheckG = false;
        [NonSerialized] public bool SavedCheckH = false;
        [NonSerialized] public bool SavedCheckI = false;
        [NonSerialized] public bool SavedCheckJ = false;
        [NonSerialized] public bool SavedCheckK = false;
        [NonSerialized] public bool SavedCheckL = false;
        [NonSerialized] public bool SavedCheckM = false;
        [NonSerialized] public bool SavedCheckN = false;
        [NonSerialized] public bool SavedCheckO = false;
        [NonSerialized] public bool SavedCheckP = false;
        [NonSerialized] public bool SavedCheckQ = false;
        [NonSerialized] public bool SavedCheckR = false;
        [NonSerialized] public bool SavedCheckS = false;
        [NonSerialized] public bool SavedCheckT = false;
        [NonSerialized] public bool SavedCheckU = false;
        [NonSerialized] public bool SavedCheckV = false;
        [NonSerialized] public bool SavedCheckW = false;
        [NonSerialized] public bool SavedCheckX = false;
        [NonSerialized] public bool SavedCheckY = false;
        [NonSerialized] public bool SavedCheckZ = false;
        [NonSerialized] public string SavedAttachmentsA = "[]";
        [NonSerialized] public string SavedAttachmentsB = "[]";
        [NonSerialized] public string SavedAttachmentsC = "[]";
        [NonSerialized] public string SavedAttachmentsD = "[]";
        [NonSerialized] public string SavedAttachmentsE = "[]";
        [NonSerialized] public string SavedAttachmentsF = "[]";
        [NonSerialized] public string SavedAttachmentsG = "[]";
        [NonSerialized] public string SavedAttachmentsH = "[]";
        [NonSerialized] public string SavedAttachmentsI = "[]";
        [NonSerialized] public string SavedAttachmentsJ = "[]";
        [NonSerialized] public string SavedAttachmentsK = "[]";
        [NonSerialized] public string SavedAttachmentsL = "[]";
        [NonSerialized] public string SavedAttachmentsM = "[]";
        [NonSerialized] public string SavedAttachmentsN = "[]";
        [NonSerialized] public string SavedAttachmentsO = "[]";
        [NonSerialized] public string SavedAttachmentsP = "[]";
        [NonSerialized] public string SavedAttachmentsQ = "[]";
        [NonSerialized] public string SavedAttachmentsR = "[]";
        [NonSerialized] public string SavedAttachmentsS = "[]";
        [NonSerialized] public string SavedAttachmentsT = "[]";
        [NonSerialized] public string SavedAttachmentsU = "[]";
        [NonSerialized] public string SavedAttachmentsV = "[]";
        [NonSerialized] public string SavedAttachmentsW = "[]";
        [NonSerialized] public string SavedAttachmentsX = "[]";
        [NonSerialized] public string SavedAttachmentsY = "[]";
        [NonSerialized] public string SavedAttachmentsZ = "[]";

        public bool WorkValue_Updated(Context context, Column column = null)
        {
            return WorkValue.Value != SavedWorkValue &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != WorkValue.Value);
        }

        public bool ProgressRate_Updated(Context context, Column column = null)
        {
            return ProgressRate.Value != SavedProgressRate &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != ProgressRate.Value);
        }

        public bool Status_Updated(Context context, Column column = null)
        {
            return Status.Value != SavedStatus &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Status.Value);
        }

        public bool Manager_Updated(Context context, Column column = null)
        {
            return Manager.Id != SavedManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Manager.Id);
        }

        public bool Owner_Updated(Context context, Column column = null)
        {
            return Owner.Id != SavedOwner &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Owner.Id);
        }

        public bool ClassA_Updated(Context context, Column column = null)
        {
            return ClassA != SavedClassA && ClassA != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassA);
        }

        public bool ClassB_Updated(Context context, Column column = null)
        {
            return ClassB != SavedClassB && ClassB != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassB);
        }

        public bool ClassC_Updated(Context context, Column column = null)
        {
            return ClassC != SavedClassC && ClassC != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassC);
        }

        public bool ClassD_Updated(Context context, Column column = null)
        {
            return ClassD != SavedClassD && ClassD != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassD);
        }

        public bool ClassE_Updated(Context context, Column column = null)
        {
            return ClassE != SavedClassE && ClassE != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassE);
        }

        public bool ClassF_Updated(Context context, Column column = null)
        {
            return ClassF != SavedClassF && ClassF != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassF);
        }

        public bool ClassG_Updated(Context context, Column column = null)
        {
            return ClassG != SavedClassG && ClassG != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassG);
        }

        public bool ClassH_Updated(Context context, Column column = null)
        {
            return ClassH != SavedClassH && ClassH != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassH);
        }

        public bool ClassI_Updated(Context context, Column column = null)
        {
            return ClassI != SavedClassI && ClassI != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassI);
        }

        public bool ClassJ_Updated(Context context, Column column = null)
        {
            return ClassJ != SavedClassJ && ClassJ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassJ);
        }

        public bool ClassK_Updated(Context context, Column column = null)
        {
            return ClassK != SavedClassK && ClassK != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassK);
        }

        public bool ClassL_Updated(Context context, Column column = null)
        {
            return ClassL != SavedClassL && ClassL != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassL);
        }

        public bool ClassM_Updated(Context context, Column column = null)
        {
            return ClassM != SavedClassM && ClassM != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassM);
        }

        public bool ClassN_Updated(Context context, Column column = null)
        {
            return ClassN != SavedClassN && ClassN != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassN);
        }

        public bool ClassO_Updated(Context context, Column column = null)
        {
            return ClassO != SavedClassO && ClassO != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassO);
        }

        public bool ClassP_Updated(Context context, Column column = null)
        {
            return ClassP != SavedClassP && ClassP != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassP);
        }

        public bool ClassQ_Updated(Context context, Column column = null)
        {
            return ClassQ != SavedClassQ && ClassQ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassQ);
        }

        public bool ClassR_Updated(Context context, Column column = null)
        {
            return ClassR != SavedClassR && ClassR != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassR);
        }

        public bool ClassS_Updated(Context context, Column column = null)
        {
            return ClassS != SavedClassS && ClassS != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassS);
        }

        public bool ClassT_Updated(Context context, Column column = null)
        {
            return ClassT != SavedClassT && ClassT != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassT);
        }

        public bool ClassU_Updated(Context context, Column column = null)
        {
            return ClassU != SavedClassU && ClassU != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassU);
        }

        public bool ClassV_Updated(Context context, Column column = null)
        {
            return ClassV != SavedClassV && ClassV != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassV);
        }

        public bool ClassW_Updated(Context context, Column column = null)
        {
            return ClassW != SavedClassW && ClassW != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassW);
        }

        public bool ClassX_Updated(Context context, Column column = null)
        {
            return ClassX != SavedClassX && ClassX != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassX);
        }

        public bool ClassY_Updated(Context context, Column column = null)
        {
            return ClassY != SavedClassY && ClassY != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassY);
        }

        public bool ClassZ_Updated(Context context, Column column = null)
        {
            return ClassZ != SavedClassZ && ClassZ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ClassZ);
        }

        public bool NumA_Updated(Context context, Column column = null)
        {
            return NumA != SavedNumA &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumA);
        }

        public bool NumB_Updated(Context context, Column column = null)
        {
            return NumB != SavedNumB &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumB);
        }

        public bool NumC_Updated(Context context, Column column = null)
        {
            return NumC != SavedNumC &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumC);
        }

        public bool NumD_Updated(Context context, Column column = null)
        {
            return NumD != SavedNumD &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumD);
        }

        public bool NumE_Updated(Context context, Column column = null)
        {
            return NumE != SavedNumE &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumE);
        }

        public bool NumF_Updated(Context context, Column column = null)
        {
            return NumF != SavedNumF &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumF);
        }

        public bool NumG_Updated(Context context, Column column = null)
        {
            return NumG != SavedNumG &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumG);
        }

        public bool NumH_Updated(Context context, Column column = null)
        {
            return NumH != SavedNumH &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumH);
        }

        public bool NumI_Updated(Context context, Column column = null)
        {
            return NumI != SavedNumI &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumI);
        }

        public bool NumJ_Updated(Context context, Column column = null)
        {
            return NumJ != SavedNumJ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumJ);
        }

        public bool NumK_Updated(Context context, Column column = null)
        {
            return NumK != SavedNumK &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumK);
        }

        public bool NumL_Updated(Context context, Column column = null)
        {
            return NumL != SavedNumL &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumL);
        }

        public bool NumM_Updated(Context context, Column column = null)
        {
            return NumM != SavedNumM &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumM);
        }

        public bool NumN_Updated(Context context, Column column = null)
        {
            return NumN != SavedNumN &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumN);
        }

        public bool NumO_Updated(Context context, Column column = null)
        {
            return NumO != SavedNumO &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumO);
        }

        public bool NumP_Updated(Context context, Column column = null)
        {
            return NumP != SavedNumP &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumP);
        }

        public bool NumQ_Updated(Context context, Column column = null)
        {
            return NumQ != SavedNumQ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumQ);
        }

        public bool NumR_Updated(Context context, Column column = null)
        {
            return NumR != SavedNumR &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumR);
        }

        public bool NumS_Updated(Context context, Column column = null)
        {
            return NumS != SavedNumS &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumS);
        }

        public bool NumT_Updated(Context context, Column column = null)
        {
            return NumT != SavedNumT &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumT);
        }

        public bool NumU_Updated(Context context, Column column = null)
        {
            return NumU != SavedNumU &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumU);
        }

        public bool NumV_Updated(Context context, Column column = null)
        {
            return NumV != SavedNumV &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumV);
        }

        public bool NumW_Updated(Context context, Column column = null)
        {
            return NumW != SavedNumW &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumW);
        }

        public bool NumX_Updated(Context context, Column column = null)
        {
            return NumX != SavedNumX &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumX);
        }

        public bool NumY_Updated(Context context, Column column = null)
        {
            return NumY != SavedNumY &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumY);
        }

        public bool NumZ_Updated(Context context, Column column = null)
        {
            return NumZ != SavedNumZ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDecimal() != NumZ);
        }

        public bool DescriptionA_Updated(Context context, Column column = null)
        {
            return DescriptionA != SavedDescriptionA && DescriptionA != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionA);
        }

        public bool DescriptionB_Updated(Context context, Column column = null)
        {
            return DescriptionB != SavedDescriptionB && DescriptionB != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionB);
        }

        public bool DescriptionC_Updated(Context context, Column column = null)
        {
            return DescriptionC != SavedDescriptionC && DescriptionC != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionC);
        }

        public bool DescriptionD_Updated(Context context, Column column = null)
        {
            return DescriptionD != SavedDescriptionD && DescriptionD != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionD);
        }

        public bool DescriptionE_Updated(Context context, Column column = null)
        {
            return DescriptionE != SavedDescriptionE && DescriptionE != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionE);
        }

        public bool DescriptionF_Updated(Context context, Column column = null)
        {
            return DescriptionF != SavedDescriptionF && DescriptionF != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionF);
        }

        public bool DescriptionG_Updated(Context context, Column column = null)
        {
            return DescriptionG != SavedDescriptionG && DescriptionG != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionG);
        }

        public bool DescriptionH_Updated(Context context, Column column = null)
        {
            return DescriptionH != SavedDescriptionH && DescriptionH != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionH);
        }

        public bool DescriptionI_Updated(Context context, Column column = null)
        {
            return DescriptionI != SavedDescriptionI && DescriptionI != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionI);
        }

        public bool DescriptionJ_Updated(Context context, Column column = null)
        {
            return DescriptionJ != SavedDescriptionJ && DescriptionJ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionJ);
        }

        public bool DescriptionK_Updated(Context context, Column column = null)
        {
            return DescriptionK != SavedDescriptionK && DescriptionK != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionK);
        }

        public bool DescriptionL_Updated(Context context, Column column = null)
        {
            return DescriptionL != SavedDescriptionL && DescriptionL != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionL);
        }

        public bool DescriptionM_Updated(Context context, Column column = null)
        {
            return DescriptionM != SavedDescriptionM && DescriptionM != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionM);
        }

        public bool DescriptionN_Updated(Context context, Column column = null)
        {
            return DescriptionN != SavedDescriptionN && DescriptionN != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionN);
        }

        public bool DescriptionO_Updated(Context context, Column column = null)
        {
            return DescriptionO != SavedDescriptionO && DescriptionO != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionO);
        }

        public bool DescriptionP_Updated(Context context, Column column = null)
        {
            return DescriptionP != SavedDescriptionP && DescriptionP != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionP);
        }

        public bool DescriptionQ_Updated(Context context, Column column = null)
        {
            return DescriptionQ != SavedDescriptionQ && DescriptionQ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionQ);
        }

        public bool DescriptionR_Updated(Context context, Column column = null)
        {
            return DescriptionR != SavedDescriptionR && DescriptionR != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionR);
        }

        public bool DescriptionS_Updated(Context context, Column column = null)
        {
            return DescriptionS != SavedDescriptionS && DescriptionS != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionS);
        }

        public bool DescriptionT_Updated(Context context, Column column = null)
        {
            return DescriptionT != SavedDescriptionT && DescriptionT != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionT);
        }

        public bool DescriptionU_Updated(Context context, Column column = null)
        {
            return DescriptionU != SavedDescriptionU && DescriptionU != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionU);
        }

        public bool DescriptionV_Updated(Context context, Column column = null)
        {
            return DescriptionV != SavedDescriptionV && DescriptionV != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionV);
        }

        public bool DescriptionW_Updated(Context context, Column column = null)
        {
            return DescriptionW != SavedDescriptionW && DescriptionW != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionW);
        }

        public bool DescriptionX_Updated(Context context, Column column = null)
        {
            return DescriptionX != SavedDescriptionX && DescriptionX != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionX);
        }

        public bool DescriptionY_Updated(Context context, Column column = null)
        {
            return DescriptionY != SavedDescriptionY && DescriptionY != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionY);
        }

        public bool DescriptionZ_Updated(Context context, Column column = null)
        {
            return DescriptionZ != SavedDescriptionZ && DescriptionZ != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DescriptionZ);
        }

        public bool CheckA_Updated(Context context, Column column = null)
        {
            return CheckA != SavedCheckA &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckA);
        }

        public bool CheckB_Updated(Context context, Column column = null)
        {
            return CheckB != SavedCheckB &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckB);
        }

        public bool CheckC_Updated(Context context, Column column = null)
        {
            return CheckC != SavedCheckC &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckC);
        }

        public bool CheckD_Updated(Context context, Column column = null)
        {
            return CheckD != SavedCheckD &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckD);
        }

        public bool CheckE_Updated(Context context, Column column = null)
        {
            return CheckE != SavedCheckE &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckE);
        }

        public bool CheckF_Updated(Context context, Column column = null)
        {
            return CheckF != SavedCheckF &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckF);
        }

        public bool CheckG_Updated(Context context, Column column = null)
        {
            return CheckG != SavedCheckG &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckG);
        }

        public bool CheckH_Updated(Context context, Column column = null)
        {
            return CheckH != SavedCheckH &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckH);
        }

        public bool CheckI_Updated(Context context, Column column = null)
        {
            return CheckI != SavedCheckI &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckI);
        }

        public bool CheckJ_Updated(Context context, Column column = null)
        {
            return CheckJ != SavedCheckJ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckJ);
        }

        public bool CheckK_Updated(Context context, Column column = null)
        {
            return CheckK != SavedCheckK &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckK);
        }

        public bool CheckL_Updated(Context context, Column column = null)
        {
            return CheckL != SavedCheckL &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckL);
        }

        public bool CheckM_Updated(Context context, Column column = null)
        {
            return CheckM != SavedCheckM &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckM);
        }

        public bool CheckN_Updated(Context context, Column column = null)
        {
            return CheckN != SavedCheckN &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckN);
        }

        public bool CheckO_Updated(Context context, Column column = null)
        {
            return CheckO != SavedCheckO &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckO);
        }

        public bool CheckP_Updated(Context context, Column column = null)
        {
            return CheckP != SavedCheckP &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckP);
        }

        public bool CheckQ_Updated(Context context, Column column = null)
        {
            return CheckQ != SavedCheckQ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckQ);
        }

        public bool CheckR_Updated(Context context, Column column = null)
        {
            return CheckR != SavedCheckR &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckR);
        }

        public bool CheckS_Updated(Context context, Column column = null)
        {
            return CheckS != SavedCheckS &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckS);
        }

        public bool CheckT_Updated(Context context, Column column = null)
        {
            return CheckT != SavedCheckT &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckT);
        }

        public bool CheckU_Updated(Context context, Column column = null)
        {
            return CheckU != SavedCheckU &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckU);
        }

        public bool CheckV_Updated(Context context, Column column = null)
        {
            return CheckV != SavedCheckV &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckV);
        }

        public bool CheckW_Updated(Context context, Column column = null)
        {
            return CheckW != SavedCheckW &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckW);
        }

        public bool CheckX_Updated(Context context, Column column = null)
        {
            return CheckX != SavedCheckX &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckX);
        }

        public bool CheckY_Updated(Context context, Column column = null)
        {
            return CheckY != SavedCheckY &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckY);
        }

        public bool CheckZ_Updated(Context context, Column column = null)
        {
            return CheckZ != SavedCheckZ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != CheckZ);
        }

        public bool AttachmentsA_Updated(Context context, Column column = null)
        {
            return AttachmentsA.RecordingJson() != SavedAttachmentsA && AttachmentsA.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsA.RecordingJson());
        }

        public bool AttachmentsB_Updated(Context context, Column column = null)
        {
            return AttachmentsB.RecordingJson() != SavedAttachmentsB && AttachmentsB.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsB.RecordingJson());
        }

        public bool AttachmentsC_Updated(Context context, Column column = null)
        {
            return AttachmentsC.RecordingJson() != SavedAttachmentsC && AttachmentsC.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsC.RecordingJson());
        }

        public bool AttachmentsD_Updated(Context context, Column column = null)
        {
            return AttachmentsD.RecordingJson() != SavedAttachmentsD && AttachmentsD.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsD.RecordingJson());
        }

        public bool AttachmentsE_Updated(Context context, Column column = null)
        {
            return AttachmentsE.RecordingJson() != SavedAttachmentsE && AttachmentsE.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsE.RecordingJson());
        }

        public bool AttachmentsF_Updated(Context context, Column column = null)
        {
            return AttachmentsF.RecordingJson() != SavedAttachmentsF && AttachmentsF.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsF.RecordingJson());
        }

        public bool AttachmentsG_Updated(Context context, Column column = null)
        {
            return AttachmentsG.RecordingJson() != SavedAttachmentsG && AttachmentsG.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsG.RecordingJson());
        }

        public bool AttachmentsH_Updated(Context context, Column column = null)
        {
            return AttachmentsH.RecordingJson() != SavedAttachmentsH && AttachmentsH.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsH.RecordingJson());
        }

        public bool AttachmentsI_Updated(Context context, Column column = null)
        {
            return AttachmentsI.RecordingJson() != SavedAttachmentsI && AttachmentsI.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsI.RecordingJson());
        }

        public bool AttachmentsJ_Updated(Context context, Column column = null)
        {
            return AttachmentsJ.RecordingJson() != SavedAttachmentsJ && AttachmentsJ.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsJ.RecordingJson());
        }

        public bool AttachmentsK_Updated(Context context, Column column = null)
        {
            return AttachmentsK.RecordingJson() != SavedAttachmentsK && AttachmentsK.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsK.RecordingJson());
        }

        public bool AttachmentsL_Updated(Context context, Column column = null)
        {
            return AttachmentsL.RecordingJson() != SavedAttachmentsL && AttachmentsL.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsL.RecordingJson());
        }

        public bool AttachmentsM_Updated(Context context, Column column = null)
        {
            return AttachmentsM.RecordingJson() != SavedAttachmentsM && AttachmentsM.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsM.RecordingJson());
        }

        public bool AttachmentsN_Updated(Context context, Column column = null)
        {
            return AttachmentsN.RecordingJson() != SavedAttachmentsN && AttachmentsN.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsN.RecordingJson());
        }

        public bool AttachmentsO_Updated(Context context, Column column = null)
        {
            return AttachmentsO.RecordingJson() != SavedAttachmentsO && AttachmentsO.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsO.RecordingJson());
        }

        public bool AttachmentsP_Updated(Context context, Column column = null)
        {
            return AttachmentsP.RecordingJson() != SavedAttachmentsP && AttachmentsP.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsP.RecordingJson());
        }

        public bool AttachmentsQ_Updated(Context context, Column column = null)
        {
            return AttachmentsQ.RecordingJson() != SavedAttachmentsQ && AttachmentsQ.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsQ.RecordingJson());
        }

        public bool AttachmentsR_Updated(Context context, Column column = null)
        {
            return AttachmentsR.RecordingJson() != SavedAttachmentsR && AttachmentsR.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsR.RecordingJson());
        }

        public bool AttachmentsS_Updated(Context context, Column column = null)
        {
            return AttachmentsS.RecordingJson() != SavedAttachmentsS && AttachmentsS.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsS.RecordingJson());
        }

        public bool AttachmentsT_Updated(Context context, Column column = null)
        {
            return AttachmentsT.RecordingJson() != SavedAttachmentsT && AttachmentsT.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsT.RecordingJson());
        }

        public bool AttachmentsU_Updated(Context context, Column column = null)
        {
            return AttachmentsU.RecordingJson() != SavedAttachmentsU && AttachmentsU.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsU.RecordingJson());
        }

        public bool AttachmentsV_Updated(Context context, Column column = null)
        {
            return AttachmentsV.RecordingJson() != SavedAttachmentsV && AttachmentsV.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsV.RecordingJson());
        }

        public bool AttachmentsW_Updated(Context context, Column column = null)
        {
            return AttachmentsW.RecordingJson() != SavedAttachmentsW && AttachmentsW.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsW.RecordingJson());
        }

        public bool AttachmentsX_Updated(Context context, Column column = null)
        {
            return AttachmentsX.RecordingJson() != SavedAttachmentsX && AttachmentsX.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsX.RecordingJson());
        }

        public bool AttachmentsY_Updated(Context context, Column column = null)
        {
            return AttachmentsY.RecordingJson() != SavedAttachmentsY && AttachmentsY.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsY.RecordingJson());
        }

        public bool AttachmentsZ_Updated(Context context, Column column = null)
        {
            return AttachmentsZ.RecordingJson() != SavedAttachmentsZ && AttachmentsZ.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AttachmentsZ.RecordingJson());
        }

        public bool StartTime_Updated(Context context, Column column = null)
        {
            return StartTime != SavedStartTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != StartTime.Date);
        }

        public bool CompletionTime_Updated(Context context, Column column = null)
        {
            return CompletionTime.Value != SavedCompletionTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != CompletionTime.Value.Date);
        }

        public bool DateA_Updated(Context context, Column column = null)
        {
            return DateA != SavedDateA &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateA.Date);
        }

        public bool DateB_Updated(Context context, Column column = null)
        {
            return DateB != SavedDateB &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateB.Date);
        }

        public bool DateC_Updated(Context context, Column column = null)
        {
            return DateC != SavedDateC &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateC.Date);
        }

        public bool DateD_Updated(Context context, Column column = null)
        {
            return DateD != SavedDateD &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateD.Date);
        }

        public bool DateE_Updated(Context context, Column column = null)
        {
            return DateE != SavedDateE &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateE.Date);
        }

        public bool DateF_Updated(Context context, Column column = null)
        {
            return DateF != SavedDateF &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateF.Date);
        }

        public bool DateG_Updated(Context context, Column column = null)
        {
            return DateG != SavedDateG &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateG.Date);
        }

        public bool DateH_Updated(Context context, Column column = null)
        {
            return DateH != SavedDateH &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateH.Date);
        }

        public bool DateI_Updated(Context context, Column column = null)
        {
            return DateI != SavedDateI &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateI.Date);
        }

        public bool DateJ_Updated(Context context, Column column = null)
        {
            return DateJ != SavedDateJ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateJ.Date);
        }

        public bool DateK_Updated(Context context, Column column = null)
        {
            return DateK != SavedDateK &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateK.Date);
        }

        public bool DateL_Updated(Context context, Column column = null)
        {
            return DateL != SavedDateL &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateL.Date);
        }

        public bool DateM_Updated(Context context, Column column = null)
        {
            return DateM != SavedDateM &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateM.Date);
        }

        public bool DateN_Updated(Context context, Column column = null)
        {
            return DateN != SavedDateN &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateN.Date);
        }

        public bool DateO_Updated(Context context, Column column = null)
        {
            return DateO != SavedDateO &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateO.Date);
        }

        public bool DateP_Updated(Context context, Column column = null)
        {
            return DateP != SavedDateP &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateP.Date);
        }

        public bool DateQ_Updated(Context context, Column column = null)
        {
            return DateQ != SavedDateQ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateQ.Date);
        }

        public bool DateR_Updated(Context context, Column column = null)
        {
            return DateR != SavedDateR &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateR.Date);
        }

        public bool DateS_Updated(Context context, Column column = null)
        {
            return DateS != SavedDateS &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateS.Date);
        }

        public bool DateT_Updated(Context context, Column column = null)
        {
            return DateT != SavedDateT &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateT.Date);
        }

        public bool DateU_Updated(Context context, Column column = null)
        {
            return DateU != SavedDateU &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateU.Date);
        }

        public bool DateV_Updated(Context context, Column column = null)
        {
            return DateV != SavedDateV &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateV.Date);
        }

        public bool DateW_Updated(Context context, Column column = null)
        {
            return DateW != SavedDateW &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateW.Date);
        }

        public bool DateX_Updated(Context context, Column column = null)
        {
            return DateX != SavedDateX &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateX.Date);
        }

        public bool DateY_Updated(Context context, Column column = null)
        {
            return DateY != SavedDateY &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateY.Date);
        }

        public bool DateZ_Updated(Context context, Column column = null)
        {
            return DateZ != SavedDateZ &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != DateZ.Date);
        }

        public string PropertyValue(Context context, string name)
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
                case "AttachmentsA": return AttachmentsA.RecordingJson();
                case "AttachmentsB": return AttachmentsB.RecordingJson();
                case "AttachmentsC": return AttachmentsC.RecordingJson();
                case "AttachmentsD": return AttachmentsD.RecordingJson();
                case "AttachmentsE": return AttachmentsE.RecordingJson();
                case "AttachmentsF": return AttachmentsF.RecordingJson();
                case "AttachmentsG": return AttachmentsG.RecordingJson();
                case "AttachmentsH": return AttachmentsH.RecordingJson();
                case "AttachmentsI": return AttachmentsI.RecordingJson();
                case "AttachmentsJ": return AttachmentsJ.RecordingJson();
                case "AttachmentsK": return AttachmentsK.RecordingJson();
                case "AttachmentsL": return AttachmentsL.RecordingJson();
                case "AttachmentsM": return AttachmentsM.RecordingJson();
                case "AttachmentsN": return AttachmentsN.RecordingJson();
                case "AttachmentsO": return AttachmentsO.RecordingJson();
                case "AttachmentsP": return AttachmentsP.RecordingJson();
                case "AttachmentsQ": return AttachmentsQ.RecordingJson();
                case "AttachmentsR": return AttachmentsR.RecordingJson();
                case "AttachmentsS": return AttachmentsS.RecordingJson();
                case "AttachmentsT": return AttachmentsT.RecordingJson();
                case "AttachmentsU": return AttachmentsU.RecordingJson();
                case "AttachmentsV": return AttachmentsV.RecordingJson();
                case "AttachmentsW": return AttachmentsW.RecordingJson();
                case "AttachmentsX": return AttachmentsX.RecordingJson();
                case "AttachmentsY": return AttachmentsY.RecordingJson();
                case "AttachmentsZ": return AttachmentsZ.RecordingJson();
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

        public Dictionary<string, string> PropertyValues(Context context, IEnumerable<string> names)
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
                        hash.Add("AttachmentsA", AttachmentsA.RecordingJson());
                        break;
                    case "AttachmentsB":
                        hash.Add("AttachmentsB", AttachmentsB.RecordingJson());
                        break;
                    case "AttachmentsC":
                        hash.Add("AttachmentsC", AttachmentsC.RecordingJson());
                        break;
                    case "AttachmentsD":
                        hash.Add("AttachmentsD", AttachmentsD.RecordingJson());
                        break;
                    case "AttachmentsE":
                        hash.Add("AttachmentsE", AttachmentsE.RecordingJson());
                        break;
                    case "AttachmentsF":
                        hash.Add("AttachmentsF", AttachmentsF.RecordingJson());
                        break;
                    case "AttachmentsG":
                        hash.Add("AttachmentsG", AttachmentsG.RecordingJson());
                        break;
                    case "AttachmentsH":
                        hash.Add("AttachmentsH", AttachmentsH.RecordingJson());
                        break;
                    case "AttachmentsI":
                        hash.Add("AttachmentsI", AttachmentsI.RecordingJson());
                        break;
                    case "AttachmentsJ":
                        hash.Add("AttachmentsJ", AttachmentsJ.RecordingJson());
                        break;
                    case "AttachmentsK":
                        hash.Add("AttachmentsK", AttachmentsK.RecordingJson());
                        break;
                    case "AttachmentsL":
                        hash.Add("AttachmentsL", AttachmentsL.RecordingJson());
                        break;
                    case "AttachmentsM":
                        hash.Add("AttachmentsM", AttachmentsM.RecordingJson());
                        break;
                    case "AttachmentsN":
                        hash.Add("AttachmentsN", AttachmentsN.RecordingJson());
                        break;
                    case "AttachmentsO":
                        hash.Add("AttachmentsO", AttachmentsO.RecordingJson());
                        break;
                    case "AttachmentsP":
                        hash.Add("AttachmentsP", AttachmentsP.RecordingJson());
                        break;
                    case "AttachmentsQ":
                        hash.Add("AttachmentsQ", AttachmentsQ.RecordingJson());
                        break;
                    case "AttachmentsR":
                        hash.Add("AttachmentsR", AttachmentsR.RecordingJson());
                        break;
                    case "AttachmentsS":
                        hash.Add("AttachmentsS", AttachmentsS.RecordingJson());
                        break;
                    case "AttachmentsT":
                        hash.Add("AttachmentsT", AttachmentsT.RecordingJson());
                        break;
                    case "AttachmentsU":
                        hash.Add("AttachmentsU", AttachmentsU.RecordingJson());
                        break;
                    case "AttachmentsV":
                        hash.Add("AttachmentsV", AttachmentsV.RecordingJson());
                        break;
                    case "AttachmentsW":
                        hash.Add("AttachmentsW", AttachmentsW.RecordingJson());
                        break;
                    case "AttachmentsX":
                        hash.Add("AttachmentsX", AttachmentsX.RecordingJson());
                        break;
                    case "AttachmentsY":
                        hash.Add("AttachmentsY", AttachmentsY.RecordingJson());
                        break;
                    case "AttachmentsZ":
                        hash.Add("AttachmentsZ", AttachmentsZ.RecordingJson());
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
            Context context,
            SiteSettings ss,
            Column column,
            ExportColumn exportColumn,
            List<string> mine)
        {
            var value = string.Empty;
            switch (column.Name)
            {
                case "SiteId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? SiteId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? UpdatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "IssueId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? IssueId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Ver.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Title":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Title.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Body":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Body.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TitleBody":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? TitleBody.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "StartTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? StartTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CompletionTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CompletionTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "WorkValue":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? WorkValue.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ProgressRate":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ProgressRate.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "RemainingWorkValue":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? RemainingWorkValue.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Status":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Status.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Manager":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Manager.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Owner":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Owner.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassA":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassA.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassB":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassB.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassC":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassC.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassD":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassD.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassE":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassE.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassF":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassF.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassG":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassG.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassH":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassH.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassI":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassI.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassJ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassJ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassK":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassK.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassL":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassL.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassM":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassM.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassN":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassN.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassO":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassO.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassP":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassP.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassQ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassQ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassR":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassR.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassS":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassS.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassT":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassT.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassU":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassU.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassV":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassV.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassW":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassW.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassX":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassX.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassY":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassY.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClassZ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? ClassZ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumA":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumA.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumB":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumB.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumC":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumC.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumD":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumD.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumE":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumE.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumF":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumF.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumG":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumG.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumH":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumH.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumI":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumI.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumJ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumJ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumK":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumK.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumL":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumL.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumM":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumM.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumN":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumN.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumO":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumO.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumP":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumP.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumQ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumQ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumR":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumR.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumS":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumS.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumT":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumT.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumU":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumU.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumV":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumV.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumW":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumW.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumX":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumX.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumY":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumY.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumZ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? NumZ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateA":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateA.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateB":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateB.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateC":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateC.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateD":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateD.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateE":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateE.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateF":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateF.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateG":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateG.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateH":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateH.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateI":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateI.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateJ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateJ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateK":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateK.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateL":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateL.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateM":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateM.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateN":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateN.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateO":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateO.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateP":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateP.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateQ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateQ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateR":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateR.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateS":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateS.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateT":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateT.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateU":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateU.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateV":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateV.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateW":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateW.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateX":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateX.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateY":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateY.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DateZ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DateZ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionA":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionA.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionB":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionB.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionC":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionC.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionD":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionD.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionE":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionE.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionF":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionF.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionG":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionG.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionH":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionH.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionI":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionI.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionJ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionJ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionK":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionK.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionL":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionL.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionM":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionM.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionN":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionN.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionO":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionO.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionP":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionP.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionQ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionQ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionR":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionR.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionS":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionS.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionT":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionT.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionU":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionU.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionV":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionV.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionW":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionW.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionX":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionX.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionY":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionY.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DescriptionZ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? DescriptionZ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckA":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckA.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckB":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckB.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckC":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckC.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckD":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckD.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckE":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckE.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckF":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckF.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckG":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckG.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckH":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckH.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckI":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckI.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckJ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckJ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckK":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckK.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckL":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckL.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckM":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckM.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckN":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckN.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckO":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckO.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckP":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckP.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckQ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckQ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckR":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckR.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckS":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckS.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckT":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckT.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckU":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckU.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckV":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckV.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckW":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckW.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckX":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckX.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckY":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckY.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CheckZ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CheckZ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsA":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsA.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsB":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsB.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsC":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsC.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsD":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsD.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsE":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsE.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsF":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsF.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsG":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsG.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsH":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsH.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsI":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsI.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsJ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsJ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsK":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsK.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsL":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsL.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsM":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsM.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsN":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsN.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsO":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsO.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsP":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsP.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsQ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsQ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsR":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsR.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsS":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsS.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsT":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsT.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsU":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsU.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsV":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsV.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsW":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsW.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsX":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsX.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsY":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsY.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AttachmentsZ":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? AttachmentsZ.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SiteTitle":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? SiteTitle.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Comments.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Creator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? Updator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
                        mine: mine)
                            ? CreatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
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
            Context context,
            SiteSettings ss,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            SiteId = ss.SiteId;
            if (IssueId == 0) SetDefault(context: context, ss: ss);
            if (setByForm) SetByForm(context: context, ss: ss);
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public IssueModel(
            Context context,
            SiteSettings ss,
            long issueId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            IssueId = issueId;
            SiteId = ss.SiteId;
            Get(context: context, ss: ss);
            if (clearSessions) ClearSessions(context: context);
            if (IssueId == 0) SetDefault(context: context, ss: ss);
            if (setByForm) SetByForm(context: context, ss: ss);
            if (setByApi) SetByApi(context: context, ss: ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public IssueModel(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null) Set(context, ss, dataRow, tableAlias);
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public IssueModel Get(
            Context context,
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
            Set(context, ss, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
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

        public IssueApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new IssueApiModel();
            ss.ReadableColumns(noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "SiteId": data.SiteId = SiteId; break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "IssueId": data.IssueId = IssueId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "Title": data.Title = Title.Value; break;
                    case "Body": data.Body = Body; break;
                    case "StartTime": data.StartTime = StartTime.ToLocal(context: context); break;
                    case "CompletionTime": data.CompletionTime = CompletionTime.Value.ToLocal(context: context); break;
                    case "WorkValue": data.WorkValue = WorkValue.Value; break;
                    case "ProgressRate": data.ProgressRate = ProgressRate.Value; break;
                    case "RemainingWorkValue": data.RemainingWorkValue = RemainingWorkValue; break;
                    case "Status": data.Status = Status.Value; break;
                    case "Manager": data.Manager = Manager.Id; break;
                    case "Owner": data.Owner = Owner.Id; break;
                    case "ClassA": data.ClassA = ClassA; break;
                    case "ClassB": data.ClassB = ClassB; break;
                    case "ClassC": data.ClassC = ClassC; break;
                    case "ClassD": data.ClassD = ClassD; break;
                    case "ClassE": data.ClassE = ClassE; break;
                    case "ClassF": data.ClassF = ClassF; break;
                    case "ClassG": data.ClassG = ClassG; break;
                    case "ClassH": data.ClassH = ClassH; break;
                    case "ClassI": data.ClassI = ClassI; break;
                    case "ClassJ": data.ClassJ = ClassJ; break;
                    case "ClassK": data.ClassK = ClassK; break;
                    case "ClassL": data.ClassL = ClassL; break;
                    case "ClassM": data.ClassM = ClassM; break;
                    case "ClassN": data.ClassN = ClassN; break;
                    case "ClassO": data.ClassO = ClassO; break;
                    case "ClassP": data.ClassP = ClassP; break;
                    case "ClassQ": data.ClassQ = ClassQ; break;
                    case "ClassR": data.ClassR = ClassR; break;
                    case "ClassS": data.ClassS = ClassS; break;
                    case "ClassT": data.ClassT = ClassT; break;
                    case "ClassU": data.ClassU = ClassU; break;
                    case "ClassV": data.ClassV = ClassV; break;
                    case "ClassW": data.ClassW = ClassW; break;
                    case "ClassX": data.ClassX = ClassX; break;
                    case "ClassY": data.ClassY = ClassY; break;
                    case "ClassZ": data.ClassZ = ClassZ; break;
                    case "NumA": data.NumA = NumA; break;
                    case "NumB": data.NumB = NumB; break;
                    case "NumC": data.NumC = NumC; break;
                    case "NumD": data.NumD = NumD; break;
                    case "NumE": data.NumE = NumE; break;
                    case "NumF": data.NumF = NumF; break;
                    case "NumG": data.NumG = NumG; break;
                    case "NumH": data.NumH = NumH; break;
                    case "NumI": data.NumI = NumI; break;
                    case "NumJ": data.NumJ = NumJ; break;
                    case "NumK": data.NumK = NumK; break;
                    case "NumL": data.NumL = NumL; break;
                    case "NumM": data.NumM = NumM; break;
                    case "NumN": data.NumN = NumN; break;
                    case "NumO": data.NumO = NumO; break;
                    case "NumP": data.NumP = NumP; break;
                    case "NumQ": data.NumQ = NumQ; break;
                    case "NumR": data.NumR = NumR; break;
                    case "NumS": data.NumS = NumS; break;
                    case "NumT": data.NumT = NumT; break;
                    case "NumU": data.NumU = NumU; break;
                    case "NumV": data.NumV = NumV; break;
                    case "NumW": data.NumW = NumW; break;
                    case "NumX": data.NumX = NumX; break;
                    case "NumY": data.NumY = NumY; break;
                    case "NumZ": data.NumZ = NumZ; break;
                    case "DateA": data.DateA = DateA.ToLocal(context: context); break;
                    case "DateB": data.DateB = DateB.ToLocal(context: context); break;
                    case "DateC": data.DateC = DateC.ToLocal(context: context); break;
                    case "DateD": data.DateD = DateD.ToLocal(context: context); break;
                    case "DateE": data.DateE = DateE.ToLocal(context: context); break;
                    case "DateF": data.DateF = DateF.ToLocal(context: context); break;
                    case "DateG": data.DateG = DateG.ToLocal(context: context); break;
                    case "DateH": data.DateH = DateH.ToLocal(context: context); break;
                    case "DateI": data.DateI = DateI.ToLocal(context: context); break;
                    case "DateJ": data.DateJ = DateJ.ToLocal(context: context); break;
                    case "DateK": data.DateK = DateK.ToLocal(context: context); break;
                    case "DateL": data.DateL = DateL.ToLocal(context: context); break;
                    case "DateM": data.DateM = DateM.ToLocal(context: context); break;
                    case "DateN": data.DateN = DateN.ToLocal(context: context); break;
                    case "DateO": data.DateO = DateO.ToLocal(context: context); break;
                    case "DateP": data.DateP = DateP.ToLocal(context: context); break;
                    case "DateQ": data.DateQ = DateQ.ToLocal(context: context); break;
                    case "DateR": data.DateR = DateR.ToLocal(context: context); break;
                    case "DateS": data.DateS = DateS.ToLocal(context: context); break;
                    case "DateT": data.DateT = DateT.ToLocal(context: context); break;
                    case "DateU": data.DateU = DateU.ToLocal(context: context); break;
                    case "DateV": data.DateV = DateV.ToLocal(context: context); break;
                    case "DateW": data.DateW = DateW.ToLocal(context: context); break;
                    case "DateX": data.DateX = DateX.ToLocal(context: context); break;
                    case "DateY": data.DateY = DateY.ToLocal(context: context); break;
                    case "DateZ": data.DateZ = DateZ.ToLocal(context: context); break;
                    case "DescriptionA": data.DescriptionA = DescriptionA; break;
                    case "DescriptionB": data.DescriptionB = DescriptionB; break;
                    case "DescriptionC": data.DescriptionC = DescriptionC; break;
                    case "DescriptionD": data.DescriptionD = DescriptionD; break;
                    case "DescriptionE": data.DescriptionE = DescriptionE; break;
                    case "DescriptionF": data.DescriptionF = DescriptionF; break;
                    case "DescriptionG": data.DescriptionG = DescriptionG; break;
                    case "DescriptionH": data.DescriptionH = DescriptionH; break;
                    case "DescriptionI": data.DescriptionI = DescriptionI; break;
                    case "DescriptionJ": data.DescriptionJ = DescriptionJ; break;
                    case "DescriptionK": data.DescriptionK = DescriptionK; break;
                    case "DescriptionL": data.DescriptionL = DescriptionL; break;
                    case "DescriptionM": data.DescriptionM = DescriptionM; break;
                    case "DescriptionN": data.DescriptionN = DescriptionN; break;
                    case "DescriptionO": data.DescriptionO = DescriptionO; break;
                    case "DescriptionP": data.DescriptionP = DescriptionP; break;
                    case "DescriptionQ": data.DescriptionQ = DescriptionQ; break;
                    case "DescriptionR": data.DescriptionR = DescriptionR; break;
                    case "DescriptionS": data.DescriptionS = DescriptionS; break;
                    case "DescriptionT": data.DescriptionT = DescriptionT; break;
                    case "DescriptionU": data.DescriptionU = DescriptionU; break;
                    case "DescriptionV": data.DescriptionV = DescriptionV; break;
                    case "DescriptionW": data.DescriptionW = DescriptionW; break;
                    case "DescriptionX": data.DescriptionX = DescriptionX; break;
                    case "DescriptionY": data.DescriptionY = DescriptionY; break;
                    case "DescriptionZ": data.DescriptionZ = DescriptionZ; break;
                    case "CheckA": data.CheckA = CheckA; break;
                    case "CheckB": data.CheckB = CheckB; break;
                    case "CheckC": data.CheckC = CheckC; break;
                    case "CheckD": data.CheckD = CheckD; break;
                    case "CheckE": data.CheckE = CheckE; break;
                    case "CheckF": data.CheckF = CheckF; break;
                    case "CheckG": data.CheckG = CheckG; break;
                    case "CheckH": data.CheckH = CheckH; break;
                    case "CheckI": data.CheckI = CheckI; break;
                    case "CheckJ": data.CheckJ = CheckJ; break;
                    case "CheckK": data.CheckK = CheckK; break;
                    case "CheckL": data.CheckL = CheckL; break;
                    case "CheckM": data.CheckM = CheckM; break;
                    case "CheckN": data.CheckN = CheckN; break;
                    case "CheckO": data.CheckO = CheckO; break;
                    case "CheckP": data.CheckP = CheckP; break;
                    case "CheckQ": data.CheckQ = CheckQ; break;
                    case "CheckR": data.CheckR = CheckR; break;
                    case "CheckS": data.CheckS = CheckS; break;
                    case "CheckT": data.CheckT = CheckT; break;
                    case "CheckU": data.CheckU = CheckU; break;
                    case "CheckV": data.CheckV = CheckV; break;
                    case "CheckW": data.CheckW = CheckW; break;
                    case "CheckX": data.CheckX = CheckX; break;
                    case "CheckY": data.CheckY = CheckY; break;
                    case "CheckZ": data.CheckZ = CheckZ; break;
                    case "AttachmentsA": data.AttachmentsA = AttachmentsA.RecordingJson(); break;
                    case "AttachmentsB": data.AttachmentsB = AttachmentsB.RecordingJson(); break;
                    case "AttachmentsC": data.AttachmentsC = AttachmentsC.RecordingJson(); break;
                    case "AttachmentsD": data.AttachmentsD = AttachmentsD.RecordingJson(); break;
                    case "AttachmentsE": data.AttachmentsE = AttachmentsE.RecordingJson(); break;
                    case "AttachmentsF": data.AttachmentsF = AttachmentsF.RecordingJson(); break;
                    case "AttachmentsG": data.AttachmentsG = AttachmentsG.RecordingJson(); break;
                    case "AttachmentsH": data.AttachmentsH = AttachmentsH.RecordingJson(); break;
                    case "AttachmentsI": data.AttachmentsI = AttachmentsI.RecordingJson(); break;
                    case "AttachmentsJ": data.AttachmentsJ = AttachmentsJ.RecordingJson(); break;
                    case "AttachmentsK": data.AttachmentsK = AttachmentsK.RecordingJson(); break;
                    case "AttachmentsL": data.AttachmentsL = AttachmentsL.RecordingJson(); break;
                    case "AttachmentsM": data.AttachmentsM = AttachmentsM.RecordingJson(); break;
                    case "AttachmentsN": data.AttachmentsN = AttachmentsN.RecordingJson(); break;
                    case "AttachmentsO": data.AttachmentsO = AttachmentsO.RecordingJson(); break;
                    case "AttachmentsP": data.AttachmentsP = AttachmentsP.RecordingJson(); break;
                    case "AttachmentsQ": data.AttachmentsQ = AttachmentsQ.RecordingJson(); break;
                    case "AttachmentsR": data.AttachmentsR = AttachmentsR.RecordingJson(); break;
                    case "AttachmentsS": data.AttachmentsS = AttachmentsS.RecordingJson(); break;
                    case "AttachmentsT": data.AttachmentsT = AttachmentsT.RecordingJson(); break;
                    case "AttachmentsU": data.AttachmentsU = AttachmentsU.RecordingJson(); break;
                    case "AttachmentsV": data.AttachmentsV = AttachmentsV.RecordingJson(); break;
                    case "AttachmentsW": data.AttachmentsW = AttachmentsW.RecordingJson(); break;
                    case "AttachmentsX": data.AttachmentsX = AttachmentsX.RecordingJson(); break;
                    case "AttachmentsY": data.AttachmentsY = AttachmentsY.RecordingJson(); break;
                    case "AttachmentsZ": data.AttachmentsZ = AttachmentsZ.RecordingJson(); break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                }
            });
            data.ItemTitle = Title.DisplayValue;
            return data;
        }

        public string FullText(
            Context context,
            SiteSettings ss,
            bool backgroundTask = false,
            bool onCreating = false)
        {
            if (Parameters.Search.Provider != "FullText") return null;
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
            var fullText = new List<string>();
            SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu.Breadcrumb(context: context, siteId: SiteId)
                .FullText(context, fullText);
            SiteId.FullText(context, fullText);
            ss.EditorColumns.ForEach(columnName =>
            {
                switch (columnName)
                {
                    case "IssueId":
                        IssueId.FullText(context, fullText);
                        break;
                    case "Title":
                        Title.FullText(context, fullText);
                        break;
                    case "Body":
                        Body.FullText(context, fullText);
                        break;
                    case "StartTime":
                        StartTime.FullText(context, fullText);
                        break;
                    case "CompletionTime":
                        CompletionTime.FullText(context, fullText);
                        break;
                    case "WorkValue":
                        WorkValue.FullText(context, fullText);
                        break;
                    case "ProgressRate":
                        ProgressRate.FullText(context, fullText);
                        break;
                    case "Status":
                        Status.FullText(context, ss.GetColumn(context: context, columnName: "Status"), fullText);
                        break;
                    case "Manager":
                        Manager.FullText(context, fullText);
                        break;
                    case "Owner":
                        Owner.FullText(context, fullText);
                        break;
                    case "ClassA":
                        ClassA.FullText(context, ss.GetColumn(context: context, columnName: "ClassA"), fullText);
                        break;
                    case "ClassB":
                        ClassB.FullText(context, ss.GetColumn(context: context, columnName: "ClassB"), fullText);
                        break;
                    case "ClassC":
                        ClassC.FullText(context, ss.GetColumn(context: context, columnName: "ClassC"), fullText);
                        break;
                    case "ClassD":
                        ClassD.FullText(context, ss.GetColumn(context: context, columnName: "ClassD"), fullText);
                        break;
                    case "ClassE":
                        ClassE.FullText(context, ss.GetColumn(context: context, columnName: "ClassE"), fullText);
                        break;
                    case "ClassF":
                        ClassF.FullText(context, ss.GetColumn(context: context, columnName: "ClassF"), fullText);
                        break;
                    case "ClassG":
                        ClassG.FullText(context, ss.GetColumn(context: context, columnName: "ClassG"), fullText);
                        break;
                    case "ClassH":
                        ClassH.FullText(context, ss.GetColumn(context: context, columnName: "ClassH"), fullText);
                        break;
                    case "ClassI":
                        ClassI.FullText(context, ss.GetColumn(context: context, columnName: "ClassI"), fullText);
                        break;
                    case "ClassJ":
                        ClassJ.FullText(context, ss.GetColumn(context: context, columnName: "ClassJ"), fullText);
                        break;
                    case "ClassK":
                        ClassK.FullText(context, ss.GetColumn(context: context, columnName: "ClassK"), fullText);
                        break;
                    case "ClassL":
                        ClassL.FullText(context, ss.GetColumn(context: context, columnName: "ClassL"), fullText);
                        break;
                    case "ClassM":
                        ClassM.FullText(context, ss.GetColumn(context: context, columnName: "ClassM"), fullText);
                        break;
                    case "ClassN":
                        ClassN.FullText(context, ss.GetColumn(context: context, columnName: "ClassN"), fullText);
                        break;
                    case "ClassO":
                        ClassO.FullText(context, ss.GetColumn(context: context, columnName: "ClassO"), fullText);
                        break;
                    case "ClassP":
                        ClassP.FullText(context, ss.GetColumn(context: context, columnName: "ClassP"), fullText);
                        break;
                    case "ClassQ":
                        ClassQ.FullText(context, ss.GetColumn(context: context, columnName: "ClassQ"), fullText);
                        break;
                    case "ClassR":
                        ClassR.FullText(context, ss.GetColumn(context: context, columnName: "ClassR"), fullText);
                        break;
                    case "ClassS":
                        ClassS.FullText(context, ss.GetColumn(context: context, columnName: "ClassS"), fullText);
                        break;
                    case "ClassT":
                        ClassT.FullText(context, ss.GetColumn(context: context, columnName: "ClassT"), fullText);
                        break;
                    case "ClassU":
                        ClassU.FullText(context, ss.GetColumn(context: context, columnName: "ClassU"), fullText);
                        break;
                    case "ClassV":
                        ClassV.FullText(context, ss.GetColumn(context: context, columnName: "ClassV"), fullText);
                        break;
                    case "ClassW":
                        ClassW.FullText(context, ss.GetColumn(context: context, columnName: "ClassW"), fullText);
                        break;
                    case "ClassX":
                        ClassX.FullText(context, ss.GetColumn(context: context, columnName: "ClassX"), fullText);
                        break;
                    case "ClassY":
                        ClassY.FullText(context, ss.GetColumn(context: context, columnName: "ClassY"), fullText);
                        break;
                    case "ClassZ":
                        ClassZ.FullText(context, ss.GetColumn(context: context, columnName: "ClassZ"), fullText);
                        break;
                    case "NumA":
                        NumA.FullText(context, fullText);
                        break;
                    case "NumB":
                        NumB.FullText(context, fullText);
                        break;
                    case "NumC":
                        NumC.FullText(context, fullText);
                        break;
                    case "NumD":
                        NumD.FullText(context, fullText);
                        break;
                    case "NumE":
                        NumE.FullText(context, fullText);
                        break;
                    case "NumF":
                        NumF.FullText(context, fullText);
                        break;
                    case "NumG":
                        NumG.FullText(context, fullText);
                        break;
                    case "NumH":
                        NumH.FullText(context, fullText);
                        break;
                    case "NumI":
                        NumI.FullText(context, fullText);
                        break;
                    case "NumJ":
                        NumJ.FullText(context, fullText);
                        break;
                    case "NumK":
                        NumK.FullText(context, fullText);
                        break;
                    case "NumL":
                        NumL.FullText(context, fullText);
                        break;
                    case "NumM":
                        NumM.FullText(context, fullText);
                        break;
                    case "NumN":
                        NumN.FullText(context, fullText);
                        break;
                    case "NumO":
                        NumO.FullText(context, fullText);
                        break;
                    case "NumP":
                        NumP.FullText(context, fullText);
                        break;
                    case "NumQ":
                        NumQ.FullText(context, fullText);
                        break;
                    case "NumR":
                        NumR.FullText(context, fullText);
                        break;
                    case "NumS":
                        NumS.FullText(context, fullText);
                        break;
                    case "NumT":
                        NumT.FullText(context, fullText);
                        break;
                    case "NumU":
                        NumU.FullText(context, fullText);
                        break;
                    case "NumV":
                        NumV.FullText(context, fullText);
                        break;
                    case "NumW":
                        NumW.FullText(context, fullText);
                        break;
                    case "NumX":
                        NumX.FullText(context, fullText);
                        break;
                    case "NumY":
                        NumY.FullText(context, fullText);
                        break;
                    case "NumZ":
                        NumZ.FullText(context, fullText);
                        break;
                    case "DateA":
                        DateA.FullText(context, fullText);
                        break;
                    case "DateB":
                        DateB.FullText(context, fullText);
                        break;
                    case "DateC":
                        DateC.FullText(context, fullText);
                        break;
                    case "DateD":
                        DateD.FullText(context, fullText);
                        break;
                    case "DateE":
                        DateE.FullText(context, fullText);
                        break;
                    case "DateF":
                        DateF.FullText(context, fullText);
                        break;
                    case "DateG":
                        DateG.FullText(context, fullText);
                        break;
                    case "DateH":
                        DateH.FullText(context, fullText);
                        break;
                    case "DateI":
                        DateI.FullText(context, fullText);
                        break;
                    case "DateJ":
                        DateJ.FullText(context, fullText);
                        break;
                    case "DateK":
                        DateK.FullText(context, fullText);
                        break;
                    case "DateL":
                        DateL.FullText(context, fullText);
                        break;
                    case "DateM":
                        DateM.FullText(context, fullText);
                        break;
                    case "DateN":
                        DateN.FullText(context, fullText);
                        break;
                    case "DateO":
                        DateO.FullText(context, fullText);
                        break;
                    case "DateP":
                        DateP.FullText(context, fullText);
                        break;
                    case "DateQ":
                        DateQ.FullText(context, fullText);
                        break;
                    case "DateR":
                        DateR.FullText(context, fullText);
                        break;
                    case "DateS":
                        DateS.FullText(context, fullText);
                        break;
                    case "DateT":
                        DateT.FullText(context, fullText);
                        break;
                    case "DateU":
                        DateU.FullText(context, fullText);
                        break;
                    case "DateV":
                        DateV.FullText(context, fullText);
                        break;
                    case "DateW":
                        DateW.FullText(context, fullText);
                        break;
                    case "DateX":
                        DateX.FullText(context, fullText);
                        break;
                    case "DateY":
                        DateY.FullText(context, fullText);
                        break;
                    case "DateZ":
                        DateZ.FullText(context, fullText);
                        break;
                    case "DescriptionA":
                        DescriptionA.FullText(context, fullText);
                        break;
                    case "DescriptionB":
                        DescriptionB.FullText(context, fullText);
                        break;
                    case "DescriptionC":
                        DescriptionC.FullText(context, fullText);
                        break;
                    case "DescriptionD":
                        DescriptionD.FullText(context, fullText);
                        break;
                    case "DescriptionE":
                        DescriptionE.FullText(context, fullText);
                        break;
                    case "DescriptionF":
                        DescriptionF.FullText(context, fullText);
                        break;
                    case "DescriptionG":
                        DescriptionG.FullText(context, fullText);
                        break;
                    case "DescriptionH":
                        DescriptionH.FullText(context, fullText);
                        break;
                    case "DescriptionI":
                        DescriptionI.FullText(context, fullText);
                        break;
                    case "DescriptionJ":
                        DescriptionJ.FullText(context, fullText);
                        break;
                    case "DescriptionK":
                        DescriptionK.FullText(context, fullText);
                        break;
                    case "DescriptionL":
                        DescriptionL.FullText(context, fullText);
                        break;
                    case "DescriptionM":
                        DescriptionM.FullText(context, fullText);
                        break;
                    case "DescriptionN":
                        DescriptionN.FullText(context, fullText);
                        break;
                    case "DescriptionO":
                        DescriptionO.FullText(context, fullText);
                        break;
                    case "DescriptionP":
                        DescriptionP.FullText(context, fullText);
                        break;
                    case "DescriptionQ":
                        DescriptionQ.FullText(context, fullText);
                        break;
                    case "DescriptionR":
                        DescriptionR.FullText(context, fullText);
                        break;
                    case "DescriptionS":
                        DescriptionS.FullText(context, fullText);
                        break;
                    case "DescriptionT":
                        DescriptionT.FullText(context, fullText);
                        break;
                    case "DescriptionU":
                        DescriptionU.FullText(context, fullText);
                        break;
                    case "DescriptionV":
                        DescriptionV.FullText(context, fullText);
                        break;
                    case "DescriptionW":
                        DescriptionW.FullText(context, fullText);
                        break;
                    case "DescriptionX":
                        DescriptionX.FullText(context, fullText);
                        break;
                    case "DescriptionY":
                        DescriptionY.FullText(context, fullText);
                        break;
                    case "DescriptionZ":
                        DescriptionZ.FullText(context, fullText);
                        break;
                    case "AttachmentsA":
                        AttachmentsA.FullText(context, fullText);
                        break;
                    case "AttachmentsB":
                        AttachmentsB.FullText(context, fullText);
                        break;
                    case "AttachmentsC":
                        AttachmentsC.FullText(context, fullText);
                        break;
                    case "AttachmentsD":
                        AttachmentsD.FullText(context, fullText);
                        break;
                    case "AttachmentsE":
                        AttachmentsE.FullText(context, fullText);
                        break;
                    case "AttachmentsF":
                        AttachmentsF.FullText(context, fullText);
                        break;
                    case "AttachmentsG":
                        AttachmentsG.FullText(context, fullText);
                        break;
                    case "AttachmentsH":
                        AttachmentsH.FullText(context, fullText);
                        break;
                    case "AttachmentsI":
                        AttachmentsI.FullText(context, fullText);
                        break;
                    case "AttachmentsJ":
                        AttachmentsJ.FullText(context, fullText);
                        break;
                    case "AttachmentsK":
                        AttachmentsK.FullText(context, fullText);
                        break;
                    case "AttachmentsL":
                        AttachmentsL.FullText(context, fullText);
                        break;
                    case "AttachmentsM":
                        AttachmentsM.FullText(context, fullText);
                        break;
                    case "AttachmentsN":
                        AttachmentsN.FullText(context, fullText);
                        break;
                    case "AttachmentsO":
                        AttachmentsO.FullText(context, fullText);
                        break;
                    case "AttachmentsP":
                        AttachmentsP.FullText(context, fullText);
                        break;
                    case "AttachmentsQ":
                        AttachmentsQ.FullText(context, fullText);
                        break;
                    case "AttachmentsR":
                        AttachmentsR.FullText(context, fullText);
                        break;
                    case "AttachmentsS":
                        AttachmentsS.FullText(context, fullText);
                        break;
                    case "AttachmentsT":
                        AttachmentsT.FullText(context, fullText);
                        break;
                    case "AttachmentsU":
                        AttachmentsU.FullText(context, fullText);
                        break;
                    case "AttachmentsV":
                        AttachmentsV.FullText(context, fullText);
                        break;
                    case "AttachmentsW":
                        AttachmentsW.FullText(context, fullText);
                        break;
                    case "AttachmentsX":
                        AttachmentsX.FullText(context, fullText);
                        break;
                    case "AttachmentsY":
                        AttachmentsY.FullText(context, fullText);
                        break;
                    case "AttachmentsZ":
                        AttachmentsZ.FullText(context, fullText);
                        break;
                    case "Comments":
                        Comments.FullText(context, fullText);
                        break;
                }
            });
            Creator.FullText(context, fullText);
            Updator.FullText(context, fullText);
            CreatedTime.FullText(context, fullText);
            UpdatedTime.FullText(context, fullText);
            if (!onCreating)
            {
                FullTextExtensions.OutgoingMailsFullText(
                    context: context,
                    fullText: fullText,
                    referenceType: "Issues",
                    referenceId: IssueId);
            }
            return fullText
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Distinct()
                .Join(" ");
        }

        public Dictionary<string, int> SearchIndexHash(Context context, SiteSettings ss)
        {
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            else
            {
                var searchIndexHash = new Dictionary<string, int>();
                SiteInfo.TenantCaches.Get(context.TenantId)?
                    .SiteMenu
                    .Breadcrumb(context: context, siteId: SiteId)
                    .SearchIndexes(context, searchIndexHash, 100);
                SiteId.SearchIndexes(context, searchIndexHash, 200);
                UpdatedTime.SearchIndexes(context, searchIndexHash, 200);
                IssueId.SearchIndexes(context, searchIndexHash, 1);
                Title.SearchIndexes(context, searchIndexHash, 4);
                Body.SearchIndexes(context, searchIndexHash, 200);
                StartTime.SearchIndexes(context, searchIndexHash, 200);
                CompletionTime.SearchIndexes(context, searchIndexHash, 200);
                WorkValue.SearchIndexes(context, searchIndexHash, 200);
                ProgressRate.SearchIndexes(context, searchIndexHash, 200);
                Status.SearchIndexes(context, ss.GetColumn(context: context, columnName: "Status"), searchIndexHash, 200);
                Manager.SearchIndexes(context, searchIndexHash, 100);
                Owner.SearchIndexes(context, searchIndexHash, 100);
                ClassA.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassA"), searchIndexHash, 200);
                ClassB.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassB"), searchIndexHash, 200);
                ClassC.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassC"), searchIndexHash, 200);
                ClassD.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassD"), searchIndexHash, 200);
                ClassE.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassE"), searchIndexHash, 200);
                ClassF.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassF"), searchIndexHash, 200);
                ClassG.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassG"), searchIndexHash, 200);
                ClassH.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassH"), searchIndexHash, 200);
                ClassI.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassI"), searchIndexHash, 200);
                ClassJ.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassJ"), searchIndexHash, 200);
                ClassK.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassK"), searchIndexHash, 200);
                ClassL.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassL"), searchIndexHash, 200);
                ClassM.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassM"), searchIndexHash, 200);
                ClassN.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassN"), searchIndexHash, 200);
                ClassO.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassO"), searchIndexHash, 200);
                ClassP.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassP"), searchIndexHash, 200);
                ClassQ.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassQ"), searchIndexHash, 200);
                ClassR.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassR"), searchIndexHash, 200);
                ClassS.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassS"), searchIndexHash, 200);
                ClassT.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassT"), searchIndexHash, 200);
                ClassU.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassU"), searchIndexHash, 200);
                ClassV.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassV"), searchIndexHash, 200);
                ClassW.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassW"), searchIndexHash, 200);
                ClassX.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassX"), searchIndexHash, 200);
                ClassY.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassY"), searchIndexHash, 200);
                ClassZ.SearchIndexes(context, ss.GetColumn(context: context, columnName: "ClassZ"), searchIndexHash, 200);
                NumA.SearchIndexes(context, searchIndexHash, 200);
                NumB.SearchIndexes(context, searchIndexHash, 200);
                NumC.SearchIndexes(context, searchIndexHash, 200);
                NumD.SearchIndexes(context, searchIndexHash, 200);
                NumE.SearchIndexes(context, searchIndexHash, 200);
                NumF.SearchIndexes(context, searchIndexHash, 200);
                NumG.SearchIndexes(context, searchIndexHash, 200);
                NumH.SearchIndexes(context, searchIndexHash, 200);
                NumI.SearchIndexes(context, searchIndexHash, 200);
                NumJ.SearchIndexes(context, searchIndexHash, 200);
                NumK.SearchIndexes(context, searchIndexHash, 200);
                NumL.SearchIndexes(context, searchIndexHash, 200);
                NumM.SearchIndexes(context, searchIndexHash, 200);
                NumN.SearchIndexes(context, searchIndexHash, 200);
                NumO.SearchIndexes(context, searchIndexHash, 200);
                NumP.SearchIndexes(context, searchIndexHash, 200);
                NumQ.SearchIndexes(context, searchIndexHash, 200);
                NumR.SearchIndexes(context, searchIndexHash, 200);
                NumS.SearchIndexes(context, searchIndexHash, 200);
                NumT.SearchIndexes(context, searchIndexHash, 200);
                NumU.SearchIndexes(context, searchIndexHash, 200);
                NumV.SearchIndexes(context, searchIndexHash, 200);
                NumW.SearchIndexes(context, searchIndexHash, 200);
                NumX.SearchIndexes(context, searchIndexHash, 200);
                NumY.SearchIndexes(context, searchIndexHash, 200);
                NumZ.SearchIndexes(context, searchIndexHash, 200);
                DateA.SearchIndexes(context, searchIndexHash, 200);
                DateB.SearchIndexes(context, searchIndexHash, 200);
                DateC.SearchIndexes(context, searchIndexHash, 200);
                DateD.SearchIndexes(context, searchIndexHash, 200);
                DateE.SearchIndexes(context, searchIndexHash, 200);
                DateF.SearchIndexes(context, searchIndexHash, 200);
                DateG.SearchIndexes(context, searchIndexHash, 200);
                DateH.SearchIndexes(context, searchIndexHash, 200);
                DateI.SearchIndexes(context, searchIndexHash, 200);
                DateJ.SearchIndexes(context, searchIndexHash, 200);
                DateK.SearchIndexes(context, searchIndexHash, 200);
                DateL.SearchIndexes(context, searchIndexHash, 200);
                DateM.SearchIndexes(context, searchIndexHash, 200);
                DateN.SearchIndexes(context, searchIndexHash, 200);
                DateO.SearchIndexes(context, searchIndexHash, 200);
                DateP.SearchIndexes(context, searchIndexHash, 200);
                DateQ.SearchIndexes(context, searchIndexHash, 200);
                DateR.SearchIndexes(context, searchIndexHash, 200);
                DateS.SearchIndexes(context, searchIndexHash, 200);
                DateT.SearchIndexes(context, searchIndexHash, 200);
                DateU.SearchIndexes(context, searchIndexHash, 200);
                DateV.SearchIndexes(context, searchIndexHash, 200);
                DateW.SearchIndexes(context, searchIndexHash, 200);
                DateX.SearchIndexes(context, searchIndexHash, 200);
                DateY.SearchIndexes(context, searchIndexHash, 200);
                DateZ.SearchIndexes(context, searchIndexHash, 200);
                DescriptionA.SearchIndexes(context, searchIndexHash, 200);
                DescriptionB.SearchIndexes(context, searchIndexHash, 200);
                DescriptionC.SearchIndexes(context, searchIndexHash, 200);
                DescriptionD.SearchIndexes(context, searchIndexHash, 200);
                DescriptionE.SearchIndexes(context, searchIndexHash, 200);
                DescriptionF.SearchIndexes(context, searchIndexHash, 200);
                DescriptionG.SearchIndexes(context, searchIndexHash, 200);
                DescriptionH.SearchIndexes(context, searchIndexHash, 200);
                DescriptionI.SearchIndexes(context, searchIndexHash, 200);
                DescriptionJ.SearchIndexes(context, searchIndexHash, 200);
                DescriptionK.SearchIndexes(context, searchIndexHash, 200);
                DescriptionL.SearchIndexes(context, searchIndexHash, 200);
                DescriptionM.SearchIndexes(context, searchIndexHash, 200);
                DescriptionN.SearchIndexes(context, searchIndexHash, 200);
                DescriptionO.SearchIndexes(context, searchIndexHash, 200);
                DescriptionP.SearchIndexes(context, searchIndexHash, 200);
                DescriptionQ.SearchIndexes(context, searchIndexHash, 200);
                DescriptionR.SearchIndexes(context, searchIndexHash, 200);
                DescriptionS.SearchIndexes(context, searchIndexHash, 200);
                DescriptionT.SearchIndexes(context, searchIndexHash, 200);
                DescriptionU.SearchIndexes(context, searchIndexHash, 200);
                DescriptionV.SearchIndexes(context, searchIndexHash, 200);
                DescriptionW.SearchIndexes(context, searchIndexHash, 200);
                DescriptionX.SearchIndexes(context, searchIndexHash, 200);
                DescriptionY.SearchIndexes(context, searchIndexHash, 200);
                DescriptionZ.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsA.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsB.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsC.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsD.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsE.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsF.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsG.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsH.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsI.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsJ.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsK.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsL.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsM.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsN.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsO.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsP.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsQ.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsR.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsS.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsT.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsU.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsV.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsW.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsX.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsY.SearchIndexes(context, searchIndexHash, 200);
                AttachmentsZ.SearchIndexes(context, searchIndexHash, 200);
                Comments.SearchIndexes(context, searchIndexHash, 200);
                Creator.SearchIndexes(context, searchIndexHash, 100);
                Updator.SearchIndexes(context, searchIndexHash, 100);
                CreatedTime.SearchIndexes(context, searchIndexHash, 200);
                SearchIndexExtensions.OutgoingMailsSearchIndexes(
                    context: context,
                    searchIndexHash: searchIndexHash,
                    referenceType: "Issues",
                    referenceId: IssueId);
                return searchIndexHash;
            }
        }

        public Error.Types Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool extendedSqls = true,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            IfDuplicatedStatements(ss, statements);
            if (extendedSqls) statements.OnCreatingExtendedSqls(SiteId);
            CreateStatements(context, ss, statements, tableType, param, otherInitValue);
            statements.CreatePermissions(
                context: context,
                ss: ss,
                users: ss.Columns
                    .Where(o => o.UserColumn)
                    .ToDictionary(o =>
                        o.ColumnName,
                        o => SiteInfo.User(
                            context: context,
                            userId: PropertyValue(
                                context: context,
                                name: o.ColumnName).ToInt())));
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            if (response.Event == "Duplicated") return Duplicated(ss, response);
            IssueId = (response.Identity ?? IssueId).ToLong();
            if (synchronizeSummary) SynchronizeSummary(context, ss, forceSynchronizeSourceSummary);
            if (context.ContractSettings.Notice != false && notice)
            {
                SetTitle(context: context, ss: ss);
                CheckNotificationConditions(context: context, ss: ss);
                Notice(context: context, ss: ss, type: "Created");
            }
            if (get) Get(context: context, ss: ss);
            if (ss.PermissionForCreating != null)
            {
                ss.SetPermissions(
                    context: context,
                    referenceId: IssueId);
            }
            var fullText = FullText(context, ss: ss, onCreating: true);
            statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                param: Rds.ItemsParam()
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                where: Rds.ItemsWhere().ReferenceId(IssueId)));
            statements.Add(BinaryUtilities.UpdateReferenceId(
                context: context,
                ss: ss,
                referenceId: IssueId,
                values: fullText));
            if (extendedSqls) statements.OnCreatedExtendedSqls(SiteId, IssueId);
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            Libraries.Search.Indexes.Create(context, ss, this);
            if (get && Rds.ExtendedSqls(SiteId, IssueId)?.Any(o => o.OnCreated) == true)
            {
                Get(context: context, ss: ss);
            }
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertItems(
                    setIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Issues")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.InsertIssues(
                    tableType: tableType,
                    param: param ?? Rds.IssuesParamDefault(
                        context: context,
                        issueModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
                InsertLinks(ss, setIdentity: true),
            });
            if (AttachmentsA_Updated(context: context))
            {
                AttachmentsA.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsB_Updated(context: context))
            {
                AttachmentsB.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsC_Updated(context: context))
            {
                AttachmentsC.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsD_Updated(context: context))
            {
                AttachmentsD.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsE_Updated(context: context))
            {
                AttachmentsE.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsF_Updated(context: context))
            {
                AttachmentsF.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsG_Updated(context: context))
            {
                AttachmentsG.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsH_Updated(context: context))
            {
                AttachmentsH.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsI_Updated(context: context))
            {
                AttachmentsI.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsJ_Updated(context: context))
            {
                AttachmentsJ.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsK_Updated(context: context))
            {
                AttachmentsK.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsL_Updated(context: context))
            {
                AttachmentsL.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsM_Updated(context: context))
            {
                AttachmentsM.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsN_Updated(context: context))
            {
                AttachmentsN.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsO_Updated(context: context))
            {
                AttachmentsO.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsP_Updated(context: context))
            {
                AttachmentsP.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsQ_Updated(context: context))
            {
                AttachmentsQ.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsR_Updated(context: context))
            {
                AttachmentsR.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsS_Updated(context: context))
            {
                AttachmentsS.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsT_Updated(context: context))
            {
                AttachmentsT.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsU_Updated(context: context))
            {
                AttachmentsU.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsV_Updated(context: context))
            {
                AttachmentsV.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsW_Updated(context: context))
            {
                AttachmentsW.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsX_Updated(context: context))
            {
                AttachmentsX.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsY_Updated(context: context))
            {
                AttachmentsY.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsZ_Updated(context: context))
            {
                AttachmentsZ.Write(context: context, statements: statements, referenceId: IssueId);
            }
            return statements;
        }

        public Error.Types Update(
            Context context,
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            bool extendedSqls = true,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (context.ContractSettings.Notice != false && notice)
            {
                CheckNotificationConditions(context: context, ss: ss, before: true);
            }
            if (setBySession) SetBySession(context: context);
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            IfDuplicatedStatements(ss, statements);
            if (extendedSqls) statements.OnUpdatingExtendedSqls(SiteId, IssueId, timestamp);
            UpdateStatements(
                context: context,
                ss: ss,
                statements: statements,
                timestamp: timestamp,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements);
            if (permissionChanged)
            {
                statements.UpdatePermissions(context, ss, IssueId, permissions);
            }
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Duplicated") return Duplicated(ss, response);
            if (response.Count == 0) return Error.Types.UpdateConflicts;
            if (synchronizeSummary) SynchronizeSummary(context, ss, forceSynchronizeSourceSummary);
            if (context.ContractSettings.Notice != false && notice)
            {
                CheckNotificationConditions(context: context, ss: ss);
                Notice(context: context, ss: ss, type: "Updated");
            }
            if (get) Get(context: context, ss: ss);
            UpdateRelatedRecords(context: context, ss: ss, extendedSqls: extendedSqls);
            if (get && Rds.ExtendedSqls(SiteId, IssueId)?.Any(o => o.OnUpdated) == true)
            {
                Get(context: context, ss: ss);
            }
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.IssuesWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateIssues(
                    where: where,
                    param: param ?? Rds.IssuesParamDefault(
                        context: context, issueModel: this, otherInitValue: otherInitValue),
                    countRecord: true)
            });
            if (AttachmentsA_Updated(context: context))
            {
                AttachmentsA.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsB_Updated(context: context))
            {
                AttachmentsB.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsC_Updated(context: context))
            {
                AttachmentsC.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsD_Updated(context: context))
            {
                AttachmentsD.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsE_Updated(context: context))
            {
                AttachmentsE.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsF_Updated(context: context))
            {
                AttachmentsF.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsG_Updated(context: context))
            {
                AttachmentsG.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsH_Updated(context: context))
            {
                AttachmentsH.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsI_Updated(context: context))
            {
                AttachmentsI.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsJ_Updated(context: context))
            {
                AttachmentsJ.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsK_Updated(context: context))
            {
                AttachmentsK.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsL_Updated(context: context))
            {
                AttachmentsL.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsM_Updated(context: context))
            {
                AttachmentsM.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsN_Updated(context: context))
            {
                AttachmentsN.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsO_Updated(context: context))
            {
                AttachmentsO.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsP_Updated(context: context))
            {
                AttachmentsP.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsQ_Updated(context: context))
            {
                AttachmentsQ.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsR_Updated(context: context))
            {
                AttachmentsR.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsS_Updated(context: context))
            {
                AttachmentsS.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsT_Updated(context: context))
            {
                AttachmentsT.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsU_Updated(context: context))
            {
                AttachmentsU.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsV_Updated(context: context))
            {
                AttachmentsV.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsW_Updated(context: context))
            {
                AttachmentsW.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsX_Updated(context: context))
            {
                AttachmentsX.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsY_Updated(context: context))
            {
                AttachmentsY.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (AttachmentsZ_Updated(context: context))
            {
                AttachmentsZ.Write(context: context, statements: statements, referenceId: IssueId);
            }
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.IssuesColumnCollection();
            var param = new Rds.IssuesParamCollection();
            column.SiteId(function: Sqls.Functions.SingleColumn); param.SiteId();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            column.IssueId(function: Sqls.Functions.SingleColumn); param.IssueId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.Title(function: Sqls.Functions.SingleColumn); param.Title();
            column.Body(function: Sqls.Functions.SingleColumn); param.Body();
            column.StartTime(function: Sqls.Functions.SingleColumn); param.StartTime();
            column.CompletionTime(function: Sqls.Functions.SingleColumn); param.CompletionTime();
            column.WorkValue(function: Sqls.Functions.SingleColumn); param.WorkValue();
            column.ProgressRate(function: Sqls.Functions.SingleColumn); param.ProgressRate();
            column.Status(function: Sqls.Functions.SingleColumn); param.Status();
            column.Manager(function: Sqls.Functions.SingleColumn); param.Manager();
            column.Owner(function: Sqls.Functions.SingleColumn); param.Owner();
            column.ClassA(function: Sqls.Functions.SingleColumn); param.ClassA();
            column.ClassB(function: Sqls.Functions.SingleColumn); param.ClassB();
            column.ClassC(function: Sqls.Functions.SingleColumn); param.ClassC();
            column.ClassD(function: Sqls.Functions.SingleColumn); param.ClassD();
            column.ClassE(function: Sqls.Functions.SingleColumn); param.ClassE();
            column.ClassF(function: Sqls.Functions.SingleColumn); param.ClassF();
            column.ClassG(function: Sqls.Functions.SingleColumn); param.ClassG();
            column.ClassH(function: Sqls.Functions.SingleColumn); param.ClassH();
            column.ClassI(function: Sqls.Functions.SingleColumn); param.ClassI();
            column.ClassJ(function: Sqls.Functions.SingleColumn); param.ClassJ();
            column.ClassK(function: Sqls.Functions.SingleColumn); param.ClassK();
            column.ClassL(function: Sqls.Functions.SingleColumn); param.ClassL();
            column.ClassM(function: Sqls.Functions.SingleColumn); param.ClassM();
            column.ClassN(function: Sqls.Functions.SingleColumn); param.ClassN();
            column.ClassO(function: Sqls.Functions.SingleColumn); param.ClassO();
            column.ClassP(function: Sqls.Functions.SingleColumn); param.ClassP();
            column.ClassQ(function: Sqls.Functions.SingleColumn); param.ClassQ();
            column.ClassR(function: Sqls.Functions.SingleColumn); param.ClassR();
            column.ClassS(function: Sqls.Functions.SingleColumn); param.ClassS();
            column.ClassT(function: Sqls.Functions.SingleColumn); param.ClassT();
            column.ClassU(function: Sqls.Functions.SingleColumn); param.ClassU();
            column.ClassV(function: Sqls.Functions.SingleColumn); param.ClassV();
            column.ClassW(function: Sqls.Functions.SingleColumn); param.ClassW();
            column.ClassX(function: Sqls.Functions.SingleColumn); param.ClassX();
            column.ClassY(function: Sqls.Functions.SingleColumn); param.ClassY();
            column.ClassZ(function: Sqls.Functions.SingleColumn); param.ClassZ();
            column.NumA(function: Sqls.Functions.SingleColumn); param.NumA();
            column.NumB(function: Sqls.Functions.SingleColumn); param.NumB();
            column.NumC(function: Sqls.Functions.SingleColumn); param.NumC();
            column.NumD(function: Sqls.Functions.SingleColumn); param.NumD();
            column.NumE(function: Sqls.Functions.SingleColumn); param.NumE();
            column.NumF(function: Sqls.Functions.SingleColumn); param.NumF();
            column.NumG(function: Sqls.Functions.SingleColumn); param.NumG();
            column.NumH(function: Sqls.Functions.SingleColumn); param.NumH();
            column.NumI(function: Sqls.Functions.SingleColumn); param.NumI();
            column.NumJ(function: Sqls.Functions.SingleColumn); param.NumJ();
            column.NumK(function: Sqls.Functions.SingleColumn); param.NumK();
            column.NumL(function: Sqls.Functions.SingleColumn); param.NumL();
            column.NumM(function: Sqls.Functions.SingleColumn); param.NumM();
            column.NumN(function: Sqls.Functions.SingleColumn); param.NumN();
            column.NumO(function: Sqls.Functions.SingleColumn); param.NumO();
            column.NumP(function: Sqls.Functions.SingleColumn); param.NumP();
            column.NumQ(function: Sqls.Functions.SingleColumn); param.NumQ();
            column.NumR(function: Sqls.Functions.SingleColumn); param.NumR();
            column.NumS(function: Sqls.Functions.SingleColumn); param.NumS();
            column.NumT(function: Sqls.Functions.SingleColumn); param.NumT();
            column.NumU(function: Sqls.Functions.SingleColumn); param.NumU();
            column.NumV(function: Sqls.Functions.SingleColumn); param.NumV();
            column.NumW(function: Sqls.Functions.SingleColumn); param.NumW();
            column.NumX(function: Sqls.Functions.SingleColumn); param.NumX();
            column.NumY(function: Sqls.Functions.SingleColumn); param.NumY();
            column.NumZ(function: Sqls.Functions.SingleColumn); param.NumZ();
            column.DateA(function: Sqls.Functions.SingleColumn); param.DateA();
            column.DateB(function: Sqls.Functions.SingleColumn); param.DateB();
            column.DateC(function: Sqls.Functions.SingleColumn); param.DateC();
            column.DateD(function: Sqls.Functions.SingleColumn); param.DateD();
            column.DateE(function: Sqls.Functions.SingleColumn); param.DateE();
            column.DateF(function: Sqls.Functions.SingleColumn); param.DateF();
            column.DateG(function: Sqls.Functions.SingleColumn); param.DateG();
            column.DateH(function: Sqls.Functions.SingleColumn); param.DateH();
            column.DateI(function: Sqls.Functions.SingleColumn); param.DateI();
            column.DateJ(function: Sqls.Functions.SingleColumn); param.DateJ();
            column.DateK(function: Sqls.Functions.SingleColumn); param.DateK();
            column.DateL(function: Sqls.Functions.SingleColumn); param.DateL();
            column.DateM(function: Sqls.Functions.SingleColumn); param.DateM();
            column.DateN(function: Sqls.Functions.SingleColumn); param.DateN();
            column.DateO(function: Sqls.Functions.SingleColumn); param.DateO();
            column.DateP(function: Sqls.Functions.SingleColumn); param.DateP();
            column.DateQ(function: Sqls.Functions.SingleColumn); param.DateQ();
            column.DateR(function: Sqls.Functions.SingleColumn); param.DateR();
            column.DateS(function: Sqls.Functions.SingleColumn); param.DateS();
            column.DateT(function: Sqls.Functions.SingleColumn); param.DateT();
            column.DateU(function: Sqls.Functions.SingleColumn); param.DateU();
            column.DateV(function: Sqls.Functions.SingleColumn); param.DateV();
            column.DateW(function: Sqls.Functions.SingleColumn); param.DateW();
            column.DateX(function: Sqls.Functions.SingleColumn); param.DateX();
            column.DateY(function: Sqls.Functions.SingleColumn); param.DateY();
            column.DateZ(function: Sqls.Functions.SingleColumn); param.DateZ();
            column.DescriptionA(function: Sqls.Functions.SingleColumn); param.DescriptionA();
            column.DescriptionB(function: Sqls.Functions.SingleColumn); param.DescriptionB();
            column.DescriptionC(function: Sqls.Functions.SingleColumn); param.DescriptionC();
            column.DescriptionD(function: Sqls.Functions.SingleColumn); param.DescriptionD();
            column.DescriptionE(function: Sqls.Functions.SingleColumn); param.DescriptionE();
            column.DescriptionF(function: Sqls.Functions.SingleColumn); param.DescriptionF();
            column.DescriptionG(function: Sqls.Functions.SingleColumn); param.DescriptionG();
            column.DescriptionH(function: Sqls.Functions.SingleColumn); param.DescriptionH();
            column.DescriptionI(function: Sqls.Functions.SingleColumn); param.DescriptionI();
            column.DescriptionJ(function: Sqls.Functions.SingleColumn); param.DescriptionJ();
            column.DescriptionK(function: Sqls.Functions.SingleColumn); param.DescriptionK();
            column.DescriptionL(function: Sqls.Functions.SingleColumn); param.DescriptionL();
            column.DescriptionM(function: Sqls.Functions.SingleColumn); param.DescriptionM();
            column.DescriptionN(function: Sqls.Functions.SingleColumn); param.DescriptionN();
            column.DescriptionO(function: Sqls.Functions.SingleColumn); param.DescriptionO();
            column.DescriptionP(function: Sqls.Functions.SingleColumn); param.DescriptionP();
            column.DescriptionQ(function: Sqls.Functions.SingleColumn); param.DescriptionQ();
            column.DescriptionR(function: Sqls.Functions.SingleColumn); param.DescriptionR();
            column.DescriptionS(function: Sqls.Functions.SingleColumn); param.DescriptionS();
            column.DescriptionT(function: Sqls.Functions.SingleColumn); param.DescriptionT();
            column.DescriptionU(function: Sqls.Functions.SingleColumn); param.DescriptionU();
            column.DescriptionV(function: Sqls.Functions.SingleColumn); param.DescriptionV();
            column.DescriptionW(function: Sqls.Functions.SingleColumn); param.DescriptionW();
            column.DescriptionX(function: Sqls.Functions.SingleColumn); param.DescriptionX();
            column.DescriptionY(function: Sqls.Functions.SingleColumn); param.DescriptionY();
            column.DescriptionZ(function: Sqls.Functions.SingleColumn); param.DescriptionZ();
            column.CheckA(function: Sqls.Functions.SingleColumn); param.CheckA();
            column.CheckB(function: Sqls.Functions.SingleColumn); param.CheckB();
            column.CheckC(function: Sqls.Functions.SingleColumn); param.CheckC();
            column.CheckD(function: Sqls.Functions.SingleColumn); param.CheckD();
            column.CheckE(function: Sqls.Functions.SingleColumn); param.CheckE();
            column.CheckF(function: Sqls.Functions.SingleColumn); param.CheckF();
            column.CheckG(function: Sqls.Functions.SingleColumn); param.CheckG();
            column.CheckH(function: Sqls.Functions.SingleColumn); param.CheckH();
            column.CheckI(function: Sqls.Functions.SingleColumn); param.CheckI();
            column.CheckJ(function: Sqls.Functions.SingleColumn); param.CheckJ();
            column.CheckK(function: Sqls.Functions.SingleColumn); param.CheckK();
            column.CheckL(function: Sqls.Functions.SingleColumn); param.CheckL();
            column.CheckM(function: Sqls.Functions.SingleColumn); param.CheckM();
            column.CheckN(function: Sqls.Functions.SingleColumn); param.CheckN();
            column.CheckO(function: Sqls.Functions.SingleColumn); param.CheckO();
            column.CheckP(function: Sqls.Functions.SingleColumn); param.CheckP();
            column.CheckQ(function: Sqls.Functions.SingleColumn); param.CheckQ();
            column.CheckR(function: Sqls.Functions.SingleColumn); param.CheckR();
            column.CheckS(function: Sqls.Functions.SingleColumn); param.CheckS();
            column.CheckT(function: Sqls.Functions.SingleColumn); param.CheckT();
            column.CheckU(function: Sqls.Functions.SingleColumn); param.CheckU();
            column.CheckV(function: Sqls.Functions.SingleColumn); param.CheckV();
            column.CheckW(function: Sqls.Functions.SingleColumn); param.CheckW();
            column.CheckX(function: Sqls.Functions.SingleColumn); param.CheckX();
            column.CheckY(function: Sqls.Functions.SingleColumn); param.CheckY();
            column.CheckZ(function: Sqls.Functions.SingleColumn); param.CheckZ();
            column.AttachmentsA(function: Sqls.Functions.SingleColumn); param.AttachmentsA();
            column.AttachmentsB(function: Sqls.Functions.SingleColumn); param.AttachmentsB();
            column.AttachmentsC(function: Sqls.Functions.SingleColumn); param.AttachmentsC();
            column.AttachmentsD(function: Sqls.Functions.SingleColumn); param.AttachmentsD();
            column.AttachmentsE(function: Sqls.Functions.SingleColumn); param.AttachmentsE();
            column.AttachmentsF(function: Sqls.Functions.SingleColumn); param.AttachmentsF();
            column.AttachmentsG(function: Sqls.Functions.SingleColumn); param.AttachmentsG();
            column.AttachmentsH(function: Sqls.Functions.SingleColumn); param.AttachmentsH();
            column.AttachmentsI(function: Sqls.Functions.SingleColumn); param.AttachmentsI();
            column.AttachmentsJ(function: Sqls.Functions.SingleColumn); param.AttachmentsJ();
            column.AttachmentsK(function: Sqls.Functions.SingleColumn); param.AttachmentsK();
            column.AttachmentsL(function: Sqls.Functions.SingleColumn); param.AttachmentsL();
            column.AttachmentsM(function: Sqls.Functions.SingleColumn); param.AttachmentsM();
            column.AttachmentsN(function: Sqls.Functions.SingleColumn); param.AttachmentsN();
            column.AttachmentsO(function: Sqls.Functions.SingleColumn); param.AttachmentsO();
            column.AttachmentsP(function: Sqls.Functions.SingleColumn); param.AttachmentsP();
            column.AttachmentsQ(function: Sqls.Functions.SingleColumn); param.AttachmentsQ();
            column.AttachmentsR(function: Sqls.Functions.SingleColumn); param.AttachmentsR();
            column.AttachmentsS(function: Sqls.Functions.SingleColumn); param.AttachmentsS();
            column.AttachmentsT(function: Sqls.Functions.SingleColumn); param.AttachmentsT();
            column.AttachmentsU(function: Sqls.Functions.SingleColumn); param.AttachmentsU();
            column.AttachmentsV(function: Sqls.Functions.SingleColumn); param.AttachmentsV();
            column.AttachmentsW(function: Sqls.Functions.SingleColumn); param.AttachmentsW();
            column.AttachmentsX(function: Sqls.Functions.SingleColumn); param.AttachmentsX();
            column.AttachmentsY(function: Sqls.Functions.SingleColumn); param.AttachmentsY();
            column.AttachmentsZ(function: Sqls.Functions.SingleColumn); param.AttachmentsZ();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            return Rds.InsertIssues(
                tableType: tableType,
                param: param,
                select: Rds.SelectIssues(column: column, where: where),
                addUpdatorParam: false);
        }

        public void UpdateRelatedRecords(
            Context context,
            SiteSettings ss,
            bool extendedSqls,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            var fullText = FullText(context, ss: ss);
            var statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                where: Rds.ItemsWhere().ReferenceId(IssueId),
                param: Rds.ItemsParam()
                    .SiteId(SiteId)
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                addUpdatedTimeParam: addUpdatedTimeParam,
                addUpdatorParam: addUpdatorParam,
                _using: updateItems));
            statements.Add(Rds.PhysicalDeleteLinks(
                where: Rds.LinksWhere().SourceId(IssueId)));
            statements.Add(InsertLinks(ss));
            if (extendedSqls) statements.OnUpdatedExtendedSqls(SiteId, IssueId);
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (ss.Sources?.Any() == true)
            {
                ItemUtilities.UpdateTitles(
                    context: context,
                    siteId: SiteId,
                    id: IssueId);
            }
            Libraries.Search.Indexes.Create(context, ss, this);
        }

        private SqlInsert InsertLinks(SiteSettings ss, bool setIdentity = false)
        {
            var link = new Dictionary<long, long>();
            ss.Columns.Where(o => o.Link.ToBool()).ForEach(column =>
            {
                switch (column.Name)
                {
                    case "ClassA":
                        if (ClassA.ToLong() != 0 && !link.ContainsKey(ClassA.ToLong()))
                        {
                            link.Add(ClassA.ToLong(), IssueId);
                        }
                        break;
                    case "ClassB":
                        if (ClassB.ToLong() != 0 && !link.ContainsKey(ClassB.ToLong()))
                        {
                            link.Add(ClassB.ToLong(), IssueId);
                        }
                        break;
                    case "ClassC":
                        if (ClassC.ToLong() != 0 && !link.ContainsKey(ClassC.ToLong()))
                        {
                            link.Add(ClassC.ToLong(), IssueId);
                        }
                        break;
                    case "ClassD":
                        if (ClassD.ToLong() != 0 && !link.ContainsKey(ClassD.ToLong()))
                        {
                            link.Add(ClassD.ToLong(), IssueId);
                        }
                        break;
                    case "ClassE":
                        if (ClassE.ToLong() != 0 && !link.ContainsKey(ClassE.ToLong()))
                        {
                            link.Add(ClassE.ToLong(), IssueId);
                        }
                        break;
                    case "ClassF":
                        if (ClassF.ToLong() != 0 && !link.ContainsKey(ClassF.ToLong()))
                        {
                            link.Add(ClassF.ToLong(), IssueId);
                        }
                        break;
                    case "ClassG":
                        if (ClassG.ToLong() != 0 && !link.ContainsKey(ClassG.ToLong()))
                        {
                            link.Add(ClassG.ToLong(), IssueId);
                        }
                        break;
                    case "ClassH":
                        if (ClassH.ToLong() != 0 && !link.ContainsKey(ClassH.ToLong()))
                        {
                            link.Add(ClassH.ToLong(), IssueId);
                        }
                        break;
                    case "ClassI":
                        if (ClassI.ToLong() != 0 && !link.ContainsKey(ClassI.ToLong()))
                        {
                            link.Add(ClassI.ToLong(), IssueId);
                        }
                        break;
                    case "ClassJ":
                        if (ClassJ.ToLong() != 0 && !link.ContainsKey(ClassJ.ToLong()))
                        {
                            link.Add(ClassJ.ToLong(), IssueId);
                        }
                        break;
                    case "ClassK":
                        if (ClassK.ToLong() != 0 && !link.ContainsKey(ClassK.ToLong()))
                        {
                            link.Add(ClassK.ToLong(), IssueId);
                        }
                        break;
                    case "ClassL":
                        if (ClassL.ToLong() != 0 && !link.ContainsKey(ClassL.ToLong()))
                        {
                            link.Add(ClassL.ToLong(), IssueId);
                        }
                        break;
                    case "ClassM":
                        if (ClassM.ToLong() != 0 && !link.ContainsKey(ClassM.ToLong()))
                        {
                            link.Add(ClassM.ToLong(), IssueId);
                        }
                        break;
                    case "ClassN":
                        if (ClassN.ToLong() != 0 && !link.ContainsKey(ClassN.ToLong()))
                        {
                            link.Add(ClassN.ToLong(), IssueId);
                        }
                        break;
                    case "ClassO":
                        if (ClassO.ToLong() != 0 && !link.ContainsKey(ClassO.ToLong()))
                        {
                            link.Add(ClassO.ToLong(), IssueId);
                        }
                        break;
                    case "ClassP":
                        if (ClassP.ToLong() != 0 && !link.ContainsKey(ClassP.ToLong()))
                        {
                            link.Add(ClassP.ToLong(), IssueId);
                        }
                        break;
                    case "ClassQ":
                        if (ClassQ.ToLong() != 0 && !link.ContainsKey(ClassQ.ToLong()))
                        {
                            link.Add(ClassQ.ToLong(), IssueId);
                        }
                        break;
                    case "ClassR":
                        if (ClassR.ToLong() != 0 && !link.ContainsKey(ClassR.ToLong()))
                        {
                            link.Add(ClassR.ToLong(), IssueId);
                        }
                        break;
                    case "ClassS":
                        if (ClassS.ToLong() != 0 && !link.ContainsKey(ClassS.ToLong()))
                        {
                            link.Add(ClassS.ToLong(), IssueId);
                        }
                        break;
                    case "ClassT":
                        if (ClassT.ToLong() != 0 && !link.ContainsKey(ClassT.ToLong()))
                        {
                            link.Add(ClassT.ToLong(), IssueId);
                        }
                        break;
                    case "ClassU":
                        if (ClassU.ToLong() != 0 && !link.ContainsKey(ClassU.ToLong()))
                        {
                            link.Add(ClassU.ToLong(), IssueId);
                        }
                        break;
                    case "ClassV":
                        if (ClassV.ToLong() != 0 && !link.ContainsKey(ClassV.ToLong()))
                        {
                            link.Add(ClassV.ToLong(), IssueId);
                        }
                        break;
                    case "ClassW":
                        if (ClassW.ToLong() != 0 && !link.ContainsKey(ClassW.ToLong()))
                        {
                            link.Add(ClassW.ToLong(), IssueId);
                        }
                        break;
                    case "ClassX":
                        if (ClassX.ToLong() != 0 && !link.ContainsKey(ClassX.ToLong()))
                        {
                            link.Add(ClassX.ToLong(), IssueId);
                        }
                        break;
                    case "ClassY":
                        if (ClassY.ToLong() != 0 && !link.ContainsKey(ClassY.ToLong()))
                        {
                            link.Add(ClassY.ToLong(), IssueId);
                        }
                        break;
                    case "ClassZ":
                        if (ClassZ.ToLong() != 0 && !link.ContainsKey(ClassZ.ToLong()))
                        {
                            link.Add(ClassZ.ToLong(), IssueId);
                        }
                        break;
                    default: break;
                }
            });
            return LinkUtilities.Insert(link, setIdentity);
        }

        public Error.Types UpdateOrCreate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    setIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Issues")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertIssues(
                    where: where ?? Rds.IssuesWhereDefault(this),
                    param: param ?? Rds.IssuesParamDefault(
                        context: context, issueModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            IssueId = (response.Identity ?? IssueId).ToLong();
            Get(context: context, ss: ss);
            Libraries.Search.Indexes.Create(context, ss, this);
            return Error.Types.None;
        }

        public Error.Types Move(Context context, SiteSettings ss, SiteSettings targetSs)
        {
            SiteId = targetSs.SiteId;
            var statements = new List<SqlStatement>();
            var fullText = FullText(context, targetSs);
            IfDuplicatedStatements(targetSs, statements);
            statements.AddRange(new List<SqlStatement>
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
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Duplicated") return Duplicated(targetSs, response);
            SynchronizeSummary(context: context, ss: ss);
            Get(context: context, ss: targetSs);
            Libraries.Search.Indexes.Create(context, targetSs, this);
            return Error.Types.None;
        }

        public Error.Types Delete(Context context, SiteSettings ss, bool notice = false)
        {
            if (context.ContractSettings.Notice != false && notice)
            {
                CheckNotificationConditions(context: context, ss: ss, before: true);
            }
            var statements = new List<SqlStatement>();
            var where = Rds.IssuesWhere().SiteId(SiteId).IssueId(IssueId);
            statements.OnDeletingExtendedSqls(SiteId, IssueId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteItems(
                    where: Rds.ItemsWhere().ReferenceId(IssueId)),
                Rds.DeleteBinaries(
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(IssueId)),
                Rds.DeleteIssues(where: where)
            });
            statements.OnDeletedExtendedSqls(SiteId, IssueId);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            SynchronizeSummary(context, ss);
            if (context.ContractSettings.Notice != false && notice)
            {
                CheckNotificationConditions(context, ss);
                Notice(context: context, ss: ss, type: "Deleted");
            }
            Libraries.Search.Indexes.Create(context, ss, this);
            return Error.Types.None;
        }

        public Error.Types Restore(Context context, SiteSettings ss,long issueId)
        {
            IssueId = issueId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(IssueId)),
                    Rds.RestoreIssues(
                        where: Rds.IssuesWhere().IssueId(IssueId))
                });
            Libraries.Search.Indexes.Create(context, ss, this);
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteIssues(
                    tableType: tableType,
                    param: Rds.IssuesParam().SiteId(SiteId).IssueId(IssueId)));
            Libraries.Search.Indexes.Create(context, ss, this);
            return Error.Types.None;
        }

        private void IfDuplicatedStatements(SiteSettings ss, List<SqlStatement> statements)
        {
            var param = new Rds.IssuesParamCollection();
            ss.Columns
                .Where(column => column.NoDuplication == true)
                .ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "Title":
                            if (Title.Value != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Title(Title.Value.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "Body":
                            if (Body != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Body(Body), SiteId, IssueId));
                            break;
                        case "StartTime":
                            if (StartTime != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.StartTime(StartTime), SiteId, IssueId));
                            break;
                        case "CompletionTime":
                            if (CompletionTime.Value != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.CompletionTime(CompletionTime.Value), SiteId, IssueId));
                            break;
                        case "WorkValue":
                            if (WorkValue.Value != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.WorkValue(WorkValue.Value), SiteId, IssueId));
                            break;
                        case "ProgressRate":
                            if (ProgressRate.Value != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ProgressRate(ProgressRate.Value), SiteId, IssueId));
                            break;
                        case "Status":
                            if (Status.Value != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Status(Status.Value), SiteId, IssueId));
                            break;
                        case "Manager":
                            if (Manager.Id != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Manager(Manager.Id), SiteId, IssueId));
                            break;
                        case "Owner":
                            if (Owner.Id != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.Owner(Owner.Id), SiteId, IssueId));
                            break;
                        case "ClassA":
                            if (ClassA != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassA(ClassA.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassB":
                            if (ClassB != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassB(ClassB.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassC":
                            if (ClassC != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassC(ClassC.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassD":
                            if (ClassD != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassD(ClassD.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassE":
                            if (ClassE != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassE(ClassE.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassF":
                            if (ClassF != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassF(ClassF.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassG":
                            if (ClassG != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassG(ClassG.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassH":
                            if (ClassH != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassH(ClassH.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassI":
                            if (ClassI != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassI(ClassI.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassJ":
                            if (ClassJ != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassJ(ClassJ.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassK":
                            if (ClassK != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassK(ClassK.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassL":
                            if (ClassL != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassL(ClassL.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassM":
                            if (ClassM != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassM(ClassM.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassN":
                            if (ClassN != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassN(ClassN.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassO":
                            if (ClassO != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassO(ClassO.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassP":
                            if (ClassP != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassP(ClassP.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassQ":
                            if (ClassQ != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassQ(ClassQ.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassR":
                            if (ClassR != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassR(ClassR.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassS":
                            if (ClassS != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassS(ClassS.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassT":
                            if (ClassT != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassT(ClassT.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassU":
                            if (ClassU != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassU(ClassU.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassV":
                            if (ClassV != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassV(ClassV.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassW":
                            if (ClassW != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassW(ClassW.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassX":
                            if (ClassX != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassX(ClassX.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassY":
                            if (ClassY != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassY(ClassY.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "ClassZ":
                            if (ClassZ != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.ClassZ(ClassZ.MaxLength(1024)), SiteId, IssueId));
                            break;
                        case "NumA":
                            if (NumA != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumA(NumA), SiteId, IssueId));
                            break;
                        case "NumB":
                            if (NumB != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumB(NumB), SiteId, IssueId));
                            break;
                        case "NumC":
                            if (NumC != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumC(NumC), SiteId, IssueId));
                            break;
                        case "NumD":
                            if (NumD != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumD(NumD), SiteId, IssueId));
                            break;
                        case "NumE":
                            if (NumE != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumE(NumE), SiteId, IssueId));
                            break;
                        case "NumF":
                            if (NumF != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumF(NumF), SiteId, IssueId));
                            break;
                        case "NumG":
                            if (NumG != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumG(NumG), SiteId, IssueId));
                            break;
                        case "NumH":
                            if (NumH != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumH(NumH), SiteId, IssueId));
                            break;
                        case "NumI":
                            if (NumI != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumI(NumI), SiteId, IssueId));
                            break;
                        case "NumJ":
                            if (NumJ != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumJ(NumJ), SiteId, IssueId));
                            break;
                        case "NumK":
                            if (NumK != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumK(NumK), SiteId, IssueId));
                            break;
                        case "NumL":
                            if (NumL != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumL(NumL), SiteId, IssueId));
                            break;
                        case "NumM":
                            if (NumM != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumM(NumM), SiteId, IssueId));
                            break;
                        case "NumN":
                            if (NumN != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumN(NumN), SiteId, IssueId));
                            break;
                        case "NumO":
                            if (NumO != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumO(NumO), SiteId, IssueId));
                            break;
                        case "NumP":
                            if (NumP != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumP(NumP), SiteId, IssueId));
                            break;
                        case "NumQ":
                            if (NumQ != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumQ(NumQ), SiteId, IssueId));
                            break;
                        case "NumR":
                            if (NumR != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumR(NumR), SiteId, IssueId));
                            break;
                        case "NumS":
                            if (NumS != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumS(NumS), SiteId, IssueId));
                            break;
                        case "NumT":
                            if (NumT != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumT(NumT), SiteId, IssueId));
                            break;
                        case "NumU":
                            if (NumU != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumU(NumU), SiteId, IssueId));
                            break;
                        case "NumV":
                            if (NumV != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumV(NumV), SiteId, IssueId));
                            break;
                        case "NumW":
                            if (NumW != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumW(NumW), SiteId, IssueId));
                            break;
                        case "NumX":
                            if (NumX != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumX(NumX), SiteId, IssueId));
                            break;
                        case "NumY":
                            if (NumY != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumY(NumY), SiteId, IssueId));
                            break;
                        case "NumZ":
                            if (NumZ != 0)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.NumZ(NumZ), SiteId, IssueId));
                            break;
                        case "DateA":
                            if (DateA != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateA(DateA), SiteId, IssueId));
                            break;
                        case "DateB":
                            if (DateB != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateB(DateB), SiteId, IssueId));
                            break;
                        case "DateC":
                            if (DateC != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateC(DateC), SiteId, IssueId));
                            break;
                        case "DateD":
                            if (DateD != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateD(DateD), SiteId, IssueId));
                            break;
                        case "DateE":
                            if (DateE != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateE(DateE), SiteId, IssueId));
                            break;
                        case "DateF":
                            if (DateF != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateF(DateF), SiteId, IssueId));
                            break;
                        case "DateG":
                            if (DateG != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateG(DateG), SiteId, IssueId));
                            break;
                        case "DateH":
                            if (DateH != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateH(DateH), SiteId, IssueId));
                            break;
                        case "DateI":
                            if (DateI != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateI(DateI), SiteId, IssueId));
                            break;
                        case "DateJ":
                            if (DateJ != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateJ(DateJ), SiteId, IssueId));
                            break;
                        case "DateK":
                            if (DateK != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateK(DateK), SiteId, IssueId));
                            break;
                        case "DateL":
                            if (DateL != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateL(DateL), SiteId, IssueId));
                            break;
                        case "DateM":
                            if (DateM != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateM(DateM), SiteId, IssueId));
                            break;
                        case "DateN":
                            if (DateN != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateN(DateN), SiteId, IssueId));
                            break;
                        case "DateO":
                            if (DateO != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateO(DateO), SiteId, IssueId));
                            break;
                        case "DateP":
                            if (DateP != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateP(DateP), SiteId, IssueId));
                            break;
                        case "DateQ":
                            if (DateQ != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateQ(DateQ), SiteId, IssueId));
                            break;
                        case "DateR":
                            if (DateR != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateR(DateR), SiteId, IssueId));
                            break;
                        case "DateS":
                            if (DateS != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateS(DateS), SiteId, IssueId));
                            break;
                        case "DateT":
                            if (DateT != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateT(DateT), SiteId, IssueId));
                            break;
                        case "DateU":
                            if (DateU != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateU(DateU), SiteId, IssueId));
                            break;
                        case "DateV":
                            if (DateV != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateV(DateV), SiteId, IssueId));
                            break;
                        case "DateW":
                            if (DateW != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateW(DateW), SiteId, IssueId));
                            break;
                        case "DateX":
                            if (DateX != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateX(DateX), SiteId, IssueId));
                            break;
                        case "DateY":
                            if (DateY != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateY(DateY), SiteId, IssueId));
                            break;
                        case "DateZ":
                            if (DateZ != 0.ToDateTime())
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DateZ(DateZ), SiteId, IssueId));
                            break;
                        case "DescriptionA":
                            if (DescriptionA != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionA(DescriptionA), SiteId, IssueId));
                            break;
                        case "DescriptionB":
                            if (DescriptionB != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionB(DescriptionB), SiteId, IssueId));
                            break;
                        case "DescriptionC":
                            if (DescriptionC != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionC(DescriptionC), SiteId, IssueId));
                            break;
                        case "DescriptionD":
                            if (DescriptionD != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionD(DescriptionD), SiteId, IssueId));
                            break;
                        case "DescriptionE":
                            if (DescriptionE != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionE(DescriptionE), SiteId, IssueId));
                            break;
                        case "DescriptionF":
                            if (DescriptionF != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionF(DescriptionF), SiteId, IssueId));
                            break;
                        case "DescriptionG":
                            if (DescriptionG != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionG(DescriptionG), SiteId, IssueId));
                            break;
                        case "DescriptionH":
                            if (DescriptionH != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionH(DescriptionH), SiteId, IssueId));
                            break;
                        case "DescriptionI":
                            if (DescriptionI != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionI(DescriptionI), SiteId, IssueId));
                            break;
                        case "DescriptionJ":
                            if (DescriptionJ != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionJ(DescriptionJ), SiteId, IssueId));
                            break;
                        case "DescriptionK":
                            if (DescriptionK != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionK(DescriptionK), SiteId, IssueId));
                            break;
                        case "DescriptionL":
                            if (DescriptionL != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionL(DescriptionL), SiteId, IssueId));
                            break;
                        case "DescriptionM":
                            if (DescriptionM != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionM(DescriptionM), SiteId, IssueId));
                            break;
                        case "DescriptionN":
                            if (DescriptionN != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionN(DescriptionN), SiteId, IssueId));
                            break;
                        case "DescriptionO":
                            if (DescriptionO != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionO(DescriptionO), SiteId, IssueId));
                            break;
                        case "DescriptionP":
                            if (DescriptionP != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionP(DescriptionP), SiteId, IssueId));
                            break;
                        case "DescriptionQ":
                            if (DescriptionQ != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionQ(DescriptionQ), SiteId, IssueId));
                            break;
                        case "DescriptionR":
                            if (DescriptionR != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionR(DescriptionR), SiteId, IssueId));
                            break;
                        case "DescriptionS":
                            if (DescriptionS != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionS(DescriptionS), SiteId, IssueId));
                            break;
                        case "DescriptionT":
                            if (DescriptionT != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionT(DescriptionT), SiteId, IssueId));
                            break;
                        case "DescriptionU":
                            if (DescriptionU != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionU(DescriptionU), SiteId, IssueId));
                            break;
                        case "DescriptionV":
                            if (DescriptionV != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionV(DescriptionV), SiteId, IssueId));
                            break;
                        case "DescriptionW":
                            if (DescriptionW != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionW(DescriptionW), SiteId, IssueId));
                            break;
                        case "DescriptionX":
                            if (DescriptionX != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionX(DescriptionX), SiteId, IssueId));
                            break;
                        case "DescriptionY":
                            if (DescriptionY != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionY(DescriptionY), SiteId, IssueId));
                            break;
                        case "DescriptionZ":
                            if (DescriptionZ != string.Empty)
                                statements.Add(column.IfDuplicatedStatement(
                                    param.DescriptionZ(DescriptionZ), SiteId, IssueId));
                            break;
                    }
                });
        }

        private Error.Types Duplicated(SiteSettings ss, SqlResponse response)
        {
            ss.DuplicatedColumn = response.Data;
            return Error.Types.Duplicated;
        }

        public void SetDefault(Context context, SiteSettings ss)
        {
            ss.Columns
                .Where(o => !o.DefaultInput.IsNullOrEmpty())
                .ForEach(column => SetDefault(context: context, ss: ss, column: column));
        }

        public void SetDefault(Context context, SiteSettings ss, Column column)
        {
            switch (column.ColumnName)
            {
                case "IssueId":
                    IssueId = column.GetDefaultInput(context: context).ToLong();
                    break;
                case "Title":
                    Title.Value = column.GetDefaultInput(context: context).ToString();
                    break;
                case "Body":
                    Body = column.GetDefaultInput(context: context).ToString();
                    break;
                case "WorkValue":
                    WorkValue.Value = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "ProgressRate":
                    ProgressRate.Value = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "Status":
                    Status.Value = column.GetDefaultInput(context: context).ToInt();
                    break;
                case "ClassA":
                    ClassA = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassB":
                    ClassB = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassC":
                    ClassC = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassD":
                    ClassD = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassE":
                    ClassE = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassF":
                    ClassF = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassG":
                    ClassG = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassH":
                    ClassH = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassI":
                    ClassI = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassJ":
                    ClassJ = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassK":
                    ClassK = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassL":
                    ClassL = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassM":
                    ClassM = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassN":
                    ClassN = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassO":
                    ClassO = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassP":
                    ClassP = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassQ":
                    ClassQ = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassR":
                    ClassR = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassS":
                    ClassS = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassT":
                    ClassT = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassU":
                    ClassU = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassV":
                    ClassV = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassW":
                    ClassW = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassX":
                    ClassX = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassY":
                    ClassY = column.GetDefaultInput(context: context).ToString();
                    break;
                case "ClassZ":
                    ClassZ = column.GetDefaultInput(context: context).ToString();
                    break;
                case "NumA":
                    NumA = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumB":
                    NumB = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumC":
                    NumC = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumD":
                    NumD = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumE":
                    NumE = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumF":
                    NumF = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumG":
                    NumG = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumH":
                    NumH = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumI":
                    NumI = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumJ":
                    NumJ = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumK":
                    NumK = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumL":
                    NumL = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumM":
                    NumM = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumN":
                    NumN = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumO":
                    NumO = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumP":
                    NumP = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumQ":
                    NumQ = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumR":
                    NumR = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumS":
                    NumS = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumT":
                    NumT = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumU":
                    NumU = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumV":
                    NumV = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumW":
                    NumW = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumX":
                    NumX = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumY":
                    NumY = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "NumZ":
                    NumZ = column.GetDefaultInput(context: context).ToDecimal();
                    break;
                case "DescriptionA":
                    DescriptionA = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionB":
                    DescriptionB = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionC":
                    DescriptionC = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionD":
                    DescriptionD = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionE":
                    DescriptionE = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionF":
                    DescriptionF = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionG":
                    DescriptionG = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionH":
                    DescriptionH = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionI":
                    DescriptionI = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionJ":
                    DescriptionJ = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionK":
                    DescriptionK = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionL":
                    DescriptionL = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionM":
                    DescriptionM = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionN":
                    DescriptionN = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionO":
                    DescriptionO = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionP":
                    DescriptionP = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionQ":
                    DescriptionQ = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionR":
                    DescriptionR = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionS":
                    DescriptionS = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionT":
                    DescriptionT = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionU":
                    DescriptionU = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionV":
                    DescriptionV = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionW":
                    DescriptionW = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionX":
                    DescriptionX = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionY":
                    DescriptionY = column.GetDefaultInput(context: context).ToString();
                    break;
                case "DescriptionZ":
                    DescriptionZ = column.GetDefaultInput(context: context).ToString();
                    break;
                case "CheckA":
                    CheckA = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckB":
                    CheckB = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckC":
                    CheckC = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckD":
                    CheckD = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckE":
                    CheckE = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckF":
                    CheckF = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckG":
                    CheckG = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckH":
                    CheckH = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckI":
                    CheckI = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckJ":
                    CheckJ = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckK":
                    CheckK = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckL":
                    CheckL = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckM":
                    CheckM = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckN":
                    CheckN = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckO":
                    CheckO = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckP":
                    CheckP = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckQ":
                    CheckQ = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckR":
                    CheckR = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckS":
                    CheckS = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckT":
                    CheckT = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckU":
                    CheckU = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckV":
                    CheckV = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckW":
                    CheckW = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckX":
                    CheckX = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckY":
                    CheckY = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "CheckZ":
                    CheckZ = column.GetDefaultInput(context: context).ToBool();
                    break;
                case "Timestamp":
                    Timestamp = column.GetDefaultInput(context: context).ToString();
                    break;
                case "Manager":
                    Manager = SiteInfo.User(
                        context: context,
                        userId: column.GetDefaultInput(context: context).ToInt());
                    break;
                case "Owner":
                    Owner = SiteInfo.User(
                        context: context,
                        userId: column.GetDefaultInput(context: context).ToInt());
                    break;
                case "AttachmentsA":
                    AttachmentsA = new Attachments();
                    break;
                case "AttachmentsB":
                    AttachmentsB = new Attachments();
                    break;
                case "AttachmentsC":
                    AttachmentsC = new Attachments();
                    break;
                case "AttachmentsD":
                    AttachmentsD = new Attachments();
                    break;
                case "AttachmentsE":
                    AttachmentsE = new Attachments();
                    break;
                case "AttachmentsF":
                    AttachmentsF = new Attachments();
                    break;
                case "AttachmentsG":
                    AttachmentsG = new Attachments();
                    break;
                case "AttachmentsH":
                    AttachmentsH = new Attachments();
                    break;
                case "AttachmentsI":
                    AttachmentsI = new Attachments();
                    break;
                case "AttachmentsJ":
                    AttachmentsJ = new Attachments();
                    break;
                case "AttachmentsK":
                    AttachmentsK = new Attachments();
                    break;
                case "AttachmentsL":
                    AttachmentsL = new Attachments();
                    break;
                case "AttachmentsM":
                    AttachmentsM = new Attachments();
                    break;
                case "AttachmentsN":
                    AttachmentsN = new Attachments();
                    break;
                case "AttachmentsO":
                    AttachmentsO = new Attachments();
                    break;
                case "AttachmentsP":
                    AttachmentsP = new Attachments();
                    break;
                case "AttachmentsQ":
                    AttachmentsQ = new Attachments();
                    break;
                case "AttachmentsR":
                    AttachmentsR = new Attachments();
                    break;
                case "AttachmentsS":
                    AttachmentsS = new Attachments();
                    break;
                case "AttachmentsT":
                    AttachmentsT = new Attachments();
                    break;
                case "AttachmentsU":
                    AttachmentsU = new Attachments();
                    break;
                case "AttachmentsV":
                    AttachmentsV = new Attachments();
                    break;
                case "AttachmentsW":
                    AttachmentsW = new Attachments();
                    break;
                case "AttachmentsX":
                    AttachmentsX = new Attachments();
                    break;
                case "AttachmentsY":
                    AttachmentsY = new Attachments();
                    break;
                case "AttachmentsZ":
                    AttachmentsZ = new Attachments();
                    break;
                case "StartTime":
                    StartTime = column.DefaultTime();
                    break;
                case "DateA":
                    DateA = column.DefaultTime();
                    break;
                case "DateB":
                    DateB = column.DefaultTime();
                    break;
                case "DateC":
                    DateC = column.DefaultTime();
                    break;
                case "DateD":
                    DateD = column.DefaultTime();
                    break;
                case "DateE":
                    DateE = column.DefaultTime();
                    break;
                case "DateF":
                    DateF = column.DefaultTime();
                    break;
                case "DateG":
                    DateG = column.DefaultTime();
                    break;
                case "DateH":
                    DateH = column.DefaultTime();
                    break;
                case "DateI":
                    DateI = column.DefaultTime();
                    break;
                case "DateJ":
                    DateJ = column.DefaultTime();
                    break;
                case "DateK":
                    DateK = column.DefaultTime();
                    break;
                case "DateL":
                    DateL = column.DefaultTime();
                    break;
                case "DateM":
                    DateM = column.DefaultTime();
                    break;
                case "DateN":
                    DateN = column.DefaultTime();
                    break;
                case "DateO":
                    DateO = column.DefaultTime();
                    break;
                case "DateP":
                    DateP = column.DefaultTime();
                    break;
                case "DateQ":
                    DateQ = column.DefaultTime();
                    break;
                case "DateR":
                    DateR = column.DefaultTime();
                    break;
                case "DateS":
                    DateS = column.DefaultTime();
                    break;
                case "DateT":
                    DateT = column.DefaultTime();
                    break;
                case "DateU":
                    DateU = column.DefaultTime();
                    break;
                case "DateV":
                    DateV = column.DefaultTime();
                    break;
                case "DateW":
                    DateW = column.DefaultTime();
                    break;
                case "DateX":
                    DateX = column.DefaultTime();
                    break;
                case "DateY":
                    DateY = column.DefaultTime();
                    break;
                case "DateZ":
                    DateZ = column.DefaultTime();
                    break;
                case "CompletionTime":
                    CompletionTime = new CompletionTime(
                        context: context,
                        ss: ss,
                        value: column.DefaultTime(),
                        status: Status);
                    break;
            }
        }

        public void SetByForm(Context context, SiteSettings ss)
        {
            context.Forms.Keys.ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Issues_Title": Title = new Title(IssueId, context.Forms.Data(controlId)); break;
                    case "Issues_Body": Body = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_StartTime": StartTime = context.Forms.DateTime(controlId).ToUniversal(context: context); ProgressRate.StartTime = StartTime; break;
                    case "Issues_CompletionTime": CompletionTime = new CompletionTime(context: context, ss: ss, value: context.Forms.Data(controlId).ToDateTime(), status: Status, byForm: true); ProgressRate.CompletionTime = CompletionTime.Value; break;
                    case "Issues_WorkValue": WorkValue = new WorkValue(ss.GetColumn(context: context, columnName: "WorkValue").Round(context.Forms.Decimal(context: context, key: controlId)), ProgressRate.Value); break;
                    case "Issues_ProgressRate": ProgressRate = new ProgressRate(CreatedTime, StartTime, CompletionTime, ss.GetColumn(context: context, columnName: "ProgressRate").Round(context.Forms.Decimal(context: context, key: controlId))); WorkValue.ProgressRate = ProgressRate.Value; break;
                    case "Issues_Status": Status = new Status(context.Forms.Int(controlId)); CompletionTime.Status = Status; break;
                    case "Issues_Manager": Manager = SiteInfo.User(context: context, userId: context.Forms.Int(controlId)); break;
                    case "Issues_Owner": Owner = SiteInfo.User(context: context, userId: context.Forms.Int(controlId)); break;
                    case "Issues_ClassA": ClassA = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassB": ClassB = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassC": ClassC = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassD": ClassD = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassE": ClassE = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassF": ClassF = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassG": ClassG = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassH": ClassH = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassI": ClassI = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassJ": ClassJ = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassK": ClassK = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassL": ClassL = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassM": ClassM = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassN": ClassN = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassO": ClassO = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassP": ClassP = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassQ": ClassQ = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassR": ClassR = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassS": ClassS = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassT": ClassT = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassU": ClassU = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassV": ClassV = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassW": ClassW = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassX": ClassX = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassY": ClassY = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_ClassZ": ClassZ = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_NumA": NumA = ss.GetColumn(context: context, columnName: "NumA").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumB": NumB = ss.GetColumn(context: context, columnName: "NumB").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumC": NumC = ss.GetColumn(context: context, columnName: "NumC").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumD": NumD = ss.GetColumn(context: context, columnName: "NumD").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumE": NumE = ss.GetColumn(context: context, columnName: "NumE").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumF": NumF = ss.GetColumn(context: context, columnName: "NumF").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumG": NumG = ss.GetColumn(context: context, columnName: "NumG").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumH": NumH = ss.GetColumn(context: context, columnName: "NumH").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumI": NumI = ss.GetColumn(context: context, columnName: "NumI").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumJ": NumJ = ss.GetColumn(context: context, columnName: "NumJ").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumK": NumK = ss.GetColumn(context: context, columnName: "NumK").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumL": NumL = ss.GetColumn(context: context, columnName: "NumL").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumM": NumM = ss.GetColumn(context: context, columnName: "NumM").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumN": NumN = ss.GetColumn(context: context, columnName: "NumN").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumO": NumO = ss.GetColumn(context: context, columnName: "NumO").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumP": NumP = ss.GetColumn(context: context, columnName: "NumP").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumQ": NumQ = ss.GetColumn(context: context, columnName: "NumQ").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumR": NumR = ss.GetColumn(context: context, columnName: "NumR").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumS": NumS = ss.GetColumn(context: context, columnName: "NumS").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumT": NumT = ss.GetColumn(context: context, columnName: "NumT").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumU": NumU = ss.GetColumn(context: context, columnName: "NumU").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumV": NumV = ss.GetColumn(context: context, columnName: "NumV").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumW": NumW = ss.GetColumn(context: context, columnName: "NumW").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumX": NumX = ss.GetColumn(context: context, columnName: "NumX").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumY": NumY = ss.GetColumn(context: context, columnName: "NumY").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_NumZ": NumZ = ss.GetColumn(context: context, columnName: "NumZ").Round(context.Forms.Decimal(context: context, key: controlId)); break;
                    case "Issues_DateA": DateA = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateB": DateB = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateC": DateC = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateD": DateD = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateE": DateE = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateF": DateF = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateG": DateG = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateH": DateH = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateI": DateI = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateJ": DateJ = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateK": DateK = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateL": DateL = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateM": DateM = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateN": DateN = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateO": DateO = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateP": DateP = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateQ": DateQ = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateR": DateR = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateS": DateS = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateT": DateT = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateU": DateU = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateV": DateV = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateW": DateW = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateX": DateX = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateY": DateY = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DateZ": DateZ = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Issues_DescriptionA": DescriptionA = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionB": DescriptionB = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionC": DescriptionC = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionD": DescriptionD = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionE": DescriptionE = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionF": DescriptionF = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionG": DescriptionG = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionH": DescriptionH = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionI": DescriptionI = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionJ": DescriptionJ = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionK": DescriptionK = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionL": DescriptionL = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionM": DescriptionM = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionN": DescriptionN = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionO": DescriptionO = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionP": DescriptionP = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionQ": DescriptionQ = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionR": DescriptionR = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionS": DescriptionS = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionT": DescriptionT = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionU": DescriptionU = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionV": DescriptionV = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionW": DescriptionW = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionX": DescriptionX = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionY": DescriptionY = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_DescriptionZ": DescriptionZ = context.Forms.Data(controlId).ToString(); break;
                    case "Issues_CheckA": CheckA = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckB": CheckB = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckC": CheckC = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckD": CheckD = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckE": CheckE = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckF": CheckF = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckG": CheckG = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckH": CheckH = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckI": CheckI = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckJ": CheckJ = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckK": CheckK = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckL": CheckL = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckM": CheckM = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckN": CheckN = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckO": CheckO = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckP": CheckP = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckQ": CheckQ = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckR": CheckR = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckS": CheckS = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckT": CheckT = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckU": CheckU = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckV": CheckV = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckW": CheckW = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckX": CheckX = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckY": CheckY = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_CheckZ": CheckZ = context.Forms.Data(controlId).ToBool(); break;
                    case "Issues_AttachmentsA": AttachmentsA = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsB": AttachmentsB = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsC": AttachmentsC = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsD": AttachmentsD = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsE": AttachmentsE = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsF": AttachmentsF = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsG": AttachmentsG = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsH": AttachmentsH = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsI": AttachmentsI = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsJ": AttachmentsJ = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsK": AttachmentsK = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsL": AttachmentsL = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsM": AttachmentsM = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsN": AttachmentsN = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsO": AttachmentsO = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsP": AttachmentsP = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsQ": AttachmentsQ = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsR": AttachmentsR = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsS": AttachmentsS = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsT": AttachmentsT = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsU": AttachmentsU = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsV": AttachmentsV = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsW": AttachmentsW = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsX": AttachmentsX = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsY": AttachmentsY = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_AttachmentsZ": AttachmentsZ = context.Forms.Data(controlId).Deserialize<Attachments>(); break;
                    case "Issues_Timestamp": Timestamp = context.Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(context: context, ss: ss, body: context.Forms.Data("Comments")); break;
                    case "VerUp": VerUp = context.Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: controlId.Substring("Comment".Length).ToInt(),
                                body: context.Forms.Data(controlId));
                        }
                        break;
                }
            });
            var fromSiteId = context.Forms.Long("FromSiteId");
            if (fromSiteId > 0)
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: ss.Links.FirstOrDefault(o => o.SiteId == fromSiteId).ColumnName);
                if (PropertyValue(context: context, name: column?.ColumnName) == context.Forms.Data("LinkId"))
                {
                    column.Linking = true;
                }
            }
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = context.Forms.ControlId().Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        public void SetByModel(IssueModel issueModel)
        {
            SiteId = issueModel.SiteId;
            UpdatedTime = issueModel.UpdatedTime;
            Title = issueModel.Title;
            Body = issueModel.Body;
            StartTime = issueModel.StartTime;
            CompletionTime = issueModel.CompletionTime;
            WorkValue = issueModel.WorkValue;
            ProgressRate = issueModel.ProgressRate;
            RemainingWorkValue = issueModel.RemainingWorkValue;
            Status = issueModel.Status;
            Manager = issueModel.Manager;
            Owner = issueModel.Owner;
            ClassA = issueModel.ClassA;
            ClassB = issueModel.ClassB;
            ClassC = issueModel.ClassC;
            ClassD = issueModel.ClassD;
            ClassE = issueModel.ClassE;
            ClassF = issueModel.ClassF;
            ClassG = issueModel.ClassG;
            ClassH = issueModel.ClassH;
            ClassI = issueModel.ClassI;
            ClassJ = issueModel.ClassJ;
            ClassK = issueModel.ClassK;
            ClassL = issueModel.ClassL;
            ClassM = issueModel.ClassM;
            ClassN = issueModel.ClassN;
            ClassO = issueModel.ClassO;
            ClassP = issueModel.ClassP;
            ClassQ = issueModel.ClassQ;
            ClassR = issueModel.ClassR;
            ClassS = issueModel.ClassS;
            ClassT = issueModel.ClassT;
            ClassU = issueModel.ClassU;
            ClassV = issueModel.ClassV;
            ClassW = issueModel.ClassW;
            ClassX = issueModel.ClassX;
            ClassY = issueModel.ClassY;
            ClassZ = issueModel.ClassZ;
            NumA = issueModel.NumA;
            NumB = issueModel.NumB;
            NumC = issueModel.NumC;
            NumD = issueModel.NumD;
            NumE = issueModel.NumE;
            NumF = issueModel.NumF;
            NumG = issueModel.NumG;
            NumH = issueModel.NumH;
            NumI = issueModel.NumI;
            NumJ = issueModel.NumJ;
            NumK = issueModel.NumK;
            NumL = issueModel.NumL;
            NumM = issueModel.NumM;
            NumN = issueModel.NumN;
            NumO = issueModel.NumO;
            NumP = issueModel.NumP;
            NumQ = issueModel.NumQ;
            NumR = issueModel.NumR;
            NumS = issueModel.NumS;
            NumT = issueModel.NumT;
            NumU = issueModel.NumU;
            NumV = issueModel.NumV;
            NumW = issueModel.NumW;
            NumX = issueModel.NumX;
            NumY = issueModel.NumY;
            NumZ = issueModel.NumZ;
            DateA = issueModel.DateA;
            DateB = issueModel.DateB;
            DateC = issueModel.DateC;
            DateD = issueModel.DateD;
            DateE = issueModel.DateE;
            DateF = issueModel.DateF;
            DateG = issueModel.DateG;
            DateH = issueModel.DateH;
            DateI = issueModel.DateI;
            DateJ = issueModel.DateJ;
            DateK = issueModel.DateK;
            DateL = issueModel.DateL;
            DateM = issueModel.DateM;
            DateN = issueModel.DateN;
            DateO = issueModel.DateO;
            DateP = issueModel.DateP;
            DateQ = issueModel.DateQ;
            DateR = issueModel.DateR;
            DateS = issueModel.DateS;
            DateT = issueModel.DateT;
            DateU = issueModel.DateU;
            DateV = issueModel.DateV;
            DateW = issueModel.DateW;
            DateX = issueModel.DateX;
            DateY = issueModel.DateY;
            DateZ = issueModel.DateZ;
            DescriptionA = issueModel.DescriptionA;
            DescriptionB = issueModel.DescriptionB;
            DescriptionC = issueModel.DescriptionC;
            DescriptionD = issueModel.DescriptionD;
            DescriptionE = issueModel.DescriptionE;
            DescriptionF = issueModel.DescriptionF;
            DescriptionG = issueModel.DescriptionG;
            DescriptionH = issueModel.DescriptionH;
            DescriptionI = issueModel.DescriptionI;
            DescriptionJ = issueModel.DescriptionJ;
            DescriptionK = issueModel.DescriptionK;
            DescriptionL = issueModel.DescriptionL;
            DescriptionM = issueModel.DescriptionM;
            DescriptionN = issueModel.DescriptionN;
            DescriptionO = issueModel.DescriptionO;
            DescriptionP = issueModel.DescriptionP;
            DescriptionQ = issueModel.DescriptionQ;
            DescriptionR = issueModel.DescriptionR;
            DescriptionS = issueModel.DescriptionS;
            DescriptionT = issueModel.DescriptionT;
            DescriptionU = issueModel.DescriptionU;
            DescriptionV = issueModel.DescriptionV;
            DescriptionW = issueModel.DescriptionW;
            DescriptionX = issueModel.DescriptionX;
            DescriptionY = issueModel.DescriptionY;
            DescriptionZ = issueModel.DescriptionZ;
            CheckA = issueModel.CheckA;
            CheckB = issueModel.CheckB;
            CheckC = issueModel.CheckC;
            CheckD = issueModel.CheckD;
            CheckE = issueModel.CheckE;
            CheckF = issueModel.CheckF;
            CheckG = issueModel.CheckG;
            CheckH = issueModel.CheckH;
            CheckI = issueModel.CheckI;
            CheckJ = issueModel.CheckJ;
            CheckK = issueModel.CheckK;
            CheckL = issueModel.CheckL;
            CheckM = issueModel.CheckM;
            CheckN = issueModel.CheckN;
            CheckO = issueModel.CheckO;
            CheckP = issueModel.CheckP;
            CheckQ = issueModel.CheckQ;
            CheckR = issueModel.CheckR;
            CheckS = issueModel.CheckS;
            CheckT = issueModel.CheckT;
            CheckU = issueModel.CheckU;
            CheckV = issueModel.CheckV;
            CheckW = issueModel.CheckW;
            CheckX = issueModel.CheckX;
            CheckY = issueModel.CheckY;
            CheckZ = issueModel.CheckZ;
            AttachmentsA = issueModel.AttachmentsA;
            AttachmentsB = issueModel.AttachmentsB;
            AttachmentsC = issueModel.AttachmentsC;
            AttachmentsD = issueModel.AttachmentsD;
            AttachmentsE = issueModel.AttachmentsE;
            AttachmentsF = issueModel.AttachmentsF;
            AttachmentsG = issueModel.AttachmentsG;
            AttachmentsH = issueModel.AttachmentsH;
            AttachmentsI = issueModel.AttachmentsI;
            AttachmentsJ = issueModel.AttachmentsJ;
            AttachmentsK = issueModel.AttachmentsK;
            AttachmentsL = issueModel.AttachmentsL;
            AttachmentsM = issueModel.AttachmentsM;
            AttachmentsN = issueModel.AttachmentsN;
            AttachmentsO = issueModel.AttachmentsO;
            AttachmentsP = issueModel.AttachmentsP;
            AttachmentsQ = issueModel.AttachmentsQ;
            AttachmentsR = issueModel.AttachmentsR;
            AttachmentsS = issueModel.AttachmentsS;
            AttachmentsT = issueModel.AttachmentsT;
            AttachmentsU = issueModel.AttachmentsU;
            AttachmentsV = issueModel.AttachmentsV;
            AttachmentsW = issueModel.AttachmentsW;
            AttachmentsX = issueModel.AttachmentsX;
            AttachmentsY = issueModel.AttachmentsY;
            AttachmentsZ = issueModel.AttachmentsZ;
            Comments = issueModel.Comments;
            Creator = issueModel.Creator;
            Updator = issueModel.Updator;
            CreatedTime = issueModel.CreatedTime;
            VerUp = issueModel.VerUp;
            Comments = issueModel.Comments;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<IssueApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.Title != null) Title = new Title(data.IssueId.ToLong(), data.Title);
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.StartTime != null) StartTime = data.StartTime.ToDateTime().ToUniversal(context: context); ProgressRate.StartTime = StartTime;
            if (data.CompletionTime != null) CompletionTime = new CompletionTime(context: context, ss: ss, value: data.CompletionTime.ToDateTime(), status: Status, byForm: true); ProgressRate.CompletionTime = CompletionTime.Value;
            if (data.WorkValue != null) WorkValue = new WorkValue(ss.GetColumn(context: context, columnName: "WorkValue").Round(data.WorkValue.ToDecimal()), ProgressRate.Value);
            if (data.ProgressRate != null) ProgressRate = new ProgressRate(CreatedTime, StartTime, CompletionTime, ss.GetColumn(context: context, columnName: "ProgressRate").Round(data.ProgressRate.ToDecimal())); WorkValue.ProgressRate = ProgressRate.Value;
            if (data.Status != null) Status = new Status(data.Status.ToInt()); CompletionTime.Status = Status;
            if (data.Manager != null) Manager = SiteInfo.User(context: context, userId: data.Manager.ToInt());
            if (data.Owner != null) Owner = SiteInfo.User(context: context, userId: data.Owner.ToInt());
            if (data.ClassA != null) ClassA = data.ClassA.ToString().ToString();
            if (data.ClassB != null) ClassB = data.ClassB.ToString().ToString();
            if (data.ClassC != null) ClassC = data.ClassC.ToString().ToString();
            if (data.ClassD != null) ClassD = data.ClassD.ToString().ToString();
            if (data.ClassE != null) ClassE = data.ClassE.ToString().ToString();
            if (data.ClassF != null) ClassF = data.ClassF.ToString().ToString();
            if (data.ClassG != null) ClassG = data.ClassG.ToString().ToString();
            if (data.ClassH != null) ClassH = data.ClassH.ToString().ToString();
            if (data.ClassI != null) ClassI = data.ClassI.ToString().ToString();
            if (data.ClassJ != null) ClassJ = data.ClassJ.ToString().ToString();
            if (data.ClassK != null) ClassK = data.ClassK.ToString().ToString();
            if (data.ClassL != null) ClassL = data.ClassL.ToString().ToString();
            if (data.ClassM != null) ClassM = data.ClassM.ToString().ToString();
            if (data.ClassN != null) ClassN = data.ClassN.ToString().ToString();
            if (data.ClassO != null) ClassO = data.ClassO.ToString().ToString();
            if (data.ClassP != null) ClassP = data.ClassP.ToString().ToString();
            if (data.ClassQ != null) ClassQ = data.ClassQ.ToString().ToString();
            if (data.ClassR != null) ClassR = data.ClassR.ToString().ToString();
            if (data.ClassS != null) ClassS = data.ClassS.ToString().ToString();
            if (data.ClassT != null) ClassT = data.ClassT.ToString().ToString();
            if (data.ClassU != null) ClassU = data.ClassU.ToString().ToString();
            if (data.ClassV != null) ClassV = data.ClassV.ToString().ToString();
            if (data.ClassW != null) ClassW = data.ClassW.ToString().ToString();
            if (data.ClassX != null) ClassX = data.ClassX.ToString().ToString();
            if (data.ClassY != null) ClassY = data.ClassY.ToString().ToString();
            if (data.ClassZ != null) ClassZ = data.ClassZ.ToString().ToString();
            if (data.NumA != null) NumA = ss.GetColumn(context: context, columnName: "NumA").Round(data.NumA.ToDecimal());
            if (data.NumB != null) NumB = ss.GetColumn(context: context, columnName: "NumB").Round(data.NumB.ToDecimal());
            if (data.NumC != null) NumC = ss.GetColumn(context: context, columnName: "NumC").Round(data.NumC.ToDecimal());
            if (data.NumD != null) NumD = ss.GetColumn(context: context, columnName: "NumD").Round(data.NumD.ToDecimal());
            if (data.NumE != null) NumE = ss.GetColumn(context: context, columnName: "NumE").Round(data.NumE.ToDecimal());
            if (data.NumF != null) NumF = ss.GetColumn(context: context, columnName: "NumF").Round(data.NumF.ToDecimal());
            if (data.NumG != null) NumG = ss.GetColumn(context: context, columnName: "NumG").Round(data.NumG.ToDecimal());
            if (data.NumH != null) NumH = ss.GetColumn(context: context, columnName: "NumH").Round(data.NumH.ToDecimal());
            if (data.NumI != null) NumI = ss.GetColumn(context: context, columnName: "NumI").Round(data.NumI.ToDecimal());
            if (data.NumJ != null) NumJ = ss.GetColumn(context: context, columnName: "NumJ").Round(data.NumJ.ToDecimal());
            if (data.NumK != null) NumK = ss.GetColumn(context: context, columnName: "NumK").Round(data.NumK.ToDecimal());
            if (data.NumL != null) NumL = ss.GetColumn(context: context, columnName: "NumL").Round(data.NumL.ToDecimal());
            if (data.NumM != null) NumM = ss.GetColumn(context: context, columnName: "NumM").Round(data.NumM.ToDecimal());
            if (data.NumN != null) NumN = ss.GetColumn(context: context, columnName: "NumN").Round(data.NumN.ToDecimal());
            if (data.NumO != null) NumO = ss.GetColumn(context: context, columnName: "NumO").Round(data.NumO.ToDecimal());
            if (data.NumP != null) NumP = ss.GetColumn(context: context, columnName: "NumP").Round(data.NumP.ToDecimal());
            if (data.NumQ != null) NumQ = ss.GetColumn(context: context, columnName: "NumQ").Round(data.NumQ.ToDecimal());
            if (data.NumR != null) NumR = ss.GetColumn(context: context, columnName: "NumR").Round(data.NumR.ToDecimal());
            if (data.NumS != null) NumS = ss.GetColumn(context: context, columnName: "NumS").Round(data.NumS.ToDecimal());
            if (data.NumT != null) NumT = ss.GetColumn(context: context, columnName: "NumT").Round(data.NumT.ToDecimal());
            if (data.NumU != null) NumU = ss.GetColumn(context: context, columnName: "NumU").Round(data.NumU.ToDecimal());
            if (data.NumV != null) NumV = ss.GetColumn(context: context, columnName: "NumV").Round(data.NumV.ToDecimal());
            if (data.NumW != null) NumW = ss.GetColumn(context: context, columnName: "NumW").Round(data.NumW.ToDecimal());
            if (data.NumX != null) NumX = ss.GetColumn(context: context, columnName: "NumX").Round(data.NumX.ToDecimal());
            if (data.NumY != null) NumY = ss.GetColumn(context: context, columnName: "NumY").Round(data.NumY.ToDecimal());
            if (data.NumZ != null) NumZ = ss.GetColumn(context: context, columnName: "NumZ").Round(data.NumZ.ToDecimal());
            if (data.DateA != null) DateA = data.DateA.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateB != null) DateB = data.DateB.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateC != null) DateC = data.DateC.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateD != null) DateD = data.DateD.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateE != null) DateE = data.DateE.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateF != null) DateF = data.DateF.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateG != null) DateG = data.DateG.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateH != null) DateH = data.DateH.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateI != null) DateI = data.DateI.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateJ != null) DateJ = data.DateJ.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateK != null) DateK = data.DateK.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateL != null) DateL = data.DateL.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateM != null) DateM = data.DateM.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateN != null) DateN = data.DateN.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateO != null) DateO = data.DateO.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateP != null) DateP = data.DateP.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateQ != null) DateQ = data.DateQ.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateR != null) DateR = data.DateR.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateS != null) DateS = data.DateS.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateT != null) DateT = data.DateT.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateU != null) DateU = data.DateU.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateV != null) DateV = data.DateV.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateW != null) DateW = data.DateW.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateX != null) DateX = data.DateX.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateY != null) DateY = data.DateY.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DateZ != null) DateZ = data.DateZ.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DescriptionA != null) DescriptionA = data.DescriptionA.ToString().ToString();
            if (data.DescriptionB != null) DescriptionB = data.DescriptionB.ToString().ToString();
            if (data.DescriptionC != null) DescriptionC = data.DescriptionC.ToString().ToString();
            if (data.DescriptionD != null) DescriptionD = data.DescriptionD.ToString().ToString();
            if (data.DescriptionE != null) DescriptionE = data.DescriptionE.ToString().ToString();
            if (data.DescriptionF != null) DescriptionF = data.DescriptionF.ToString().ToString();
            if (data.DescriptionG != null) DescriptionG = data.DescriptionG.ToString().ToString();
            if (data.DescriptionH != null) DescriptionH = data.DescriptionH.ToString().ToString();
            if (data.DescriptionI != null) DescriptionI = data.DescriptionI.ToString().ToString();
            if (data.DescriptionJ != null) DescriptionJ = data.DescriptionJ.ToString().ToString();
            if (data.DescriptionK != null) DescriptionK = data.DescriptionK.ToString().ToString();
            if (data.DescriptionL != null) DescriptionL = data.DescriptionL.ToString().ToString();
            if (data.DescriptionM != null) DescriptionM = data.DescriptionM.ToString().ToString();
            if (data.DescriptionN != null) DescriptionN = data.DescriptionN.ToString().ToString();
            if (data.DescriptionO != null) DescriptionO = data.DescriptionO.ToString().ToString();
            if (data.DescriptionP != null) DescriptionP = data.DescriptionP.ToString().ToString();
            if (data.DescriptionQ != null) DescriptionQ = data.DescriptionQ.ToString().ToString();
            if (data.DescriptionR != null) DescriptionR = data.DescriptionR.ToString().ToString();
            if (data.DescriptionS != null) DescriptionS = data.DescriptionS.ToString().ToString();
            if (data.DescriptionT != null) DescriptionT = data.DescriptionT.ToString().ToString();
            if (data.DescriptionU != null) DescriptionU = data.DescriptionU.ToString().ToString();
            if (data.DescriptionV != null) DescriptionV = data.DescriptionV.ToString().ToString();
            if (data.DescriptionW != null) DescriptionW = data.DescriptionW.ToString().ToString();
            if (data.DescriptionX != null) DescriptionX = data.DescriptionX.ToString().ToString();
            if (data.DescriptionY != null) DescriptionY = data.DescriptionY.ToString().ToString();
            if (data.DescriptionZ != null) DescriptionZ = data.DescriptionZ.ToString().ToString();
            if (data.CheckA != null) CheckA = data.CheckA.ToBool().ToBool();
            if (data.CheckB != null) CheckB = data.CheckB.ToBool().ToBool();
            if (data.CheckC != null) CheckC = data.CheckC.ToBool().ToBool();
            if (data.CheckD != null) CheckD = data.CheckD.ToBool().ToBool();
            if (data.CheckE != null) CheckE = data.CheckE.ToBool().ToBool();
            if (data.CheckF != null) CheckF = data.CheckF.ToBool().ToBool();
            if (data.CheckG != null) CheckG = data.CheckG.ToBool().ToBool();
            if (data.CheckH != null) CheckH = data.CheckH.ToBool().ToBool();
            if (data.CheckI != null) CheckI = data.CheckI.ToBool().ToBool();
            if (data.CheckJ != null) CheckJ = data.CheckJ.ToBool().ToBool();
            if (data.CheckK != null) CheckK = data.CheckK.ToBool().ToBool();
            if (data.CheckL != null) CheckL = data.CheckL.ToBool().ToBool();
            if (data.CheckM != null) CheckM = data.CheckM.ToBool().ToBool();
            if (data.CheckN != null) CheckN = data.CheckN.ToBool().ToBool();
            if (data.CheckO != null) CheckO = data.CheckO.ToBool().ToBool();
            if (data.CheckP != null) CheckP = data.CheckP.ToBool().ToBool();
            if (data.CheckQ != null) CheckQ = data.CheckQ.ToBool().ToBool();
            if (data.CheckR != null) CheckR = data.CheckR.ToBool().ToBool();
            if (data.CheckS != null) CheckS = data.CheckS.ToBool().ToBool();
            if (data.CheckT != null) CheckT = data.CheckT.ToBool().ToBool();
            if (data.CheckU != null) CheckU = data.CheckU.ToBool().ToBool();
            if (data.CheckV != null) CheckV = data.CheckV.ToBool().ToBool();
            if (data.CheckW != null) CheckW = data.CheckW.ToBool().ToBool();
            if (data.CheckX != null) CheckX = data.CheckX.ToBool().ToBool();
            if (data.CheckY != null) CheckY = data.CheckY.ToBool().ToBool();
            if (data.CheckZ != null) CheckZ = data.CheckZ.ToBool().ToBool();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
        }

        private void SynchronizeSummary(
            Context context, SiteSettings ss, bool forceSynchronizeSourceSummary = false)
        {
            ss.Summaries.ForEach(summary =>
            {
                var id = SynchronizeSummaryDestinationId(summary.LinkColumn);
                var savedId = SynchronizeSummaryDestinationId(summary.LinkColumn, saved: true);
                if (id != 0)
                {
                    SynchronizeSummary(context: context, ss: ss, summary: summary, id: id);
                }
                if (savedId != 0 && id != savedId)
                {
                    SynchronizeSummary(context: context, ss: ss, summary: summary, id: savedId);
                }
            });
            SynchronizeSourceSummary(
                context: context,
                ss: ss,
                force: forceSynchronizeSourceSummary);
        }

        private void SynchronizeSummary(
            Context context, SiteSettings ss, Summary summary, long id)
        {
            var destinationSs = SiteSettingsUtilities.Get(
                context: context, siteId: summary.SiteId);
            if (destinationSs != null)
            {
                Summaries.Synchronize(
                    context: context,
                    ss: ss,
                    destinationSs: destinationSs,
                    destinationSiteId: summary.SiteId,
                    destinationReferenceType: summary.DestinationReferenceType,
                    destinationColumn: summary.DestinationColumn,
                    destinationCondition: destinationSs.Views?.Get(summary.DestinationCondition),
                    setZeroWhenOutOfCondition: summary.SetZeroWhenOutOfCondition == true,
                    sourceSiteId: SiteId,
                    sourceReferenceType: "Issues",
                    linkColumn: summary.LinkColumn,
                    type: summary.Type,
                    sourceColumn: summary.SourceColumn,
                    sourceCondition: ss.Views?.Get(summary.SourceCondition),
                    id: id);
            }
        }

        private void SynchronizeSourceSummary(
            Context context, SiteSettings ss, bool force = false)
        {
            ss.Sources.Values.ForEach(sourceSs =>
                sourceSs.Summaries
                    .Where(o => ss.Views?.Get(o.DestinationCondition) != null || force)
                    .ForEach(summary =>
                        Summaries.Synchronize(
                            context: context,
                            ss: sourceSs,
                            destinationSs: ss,
                            destinationSiteId: summary.SiteId,
                            destinationReferenceType: summary.DestinationReferenceType,
                            destinationColumn: summary.DestinationColumn,
                            destinationCondition: ss.Views?.Get(summary.DestinationCondition),
                            setZeroWhenOutOfCondition: summary.SetZeroWhenOutOfCondition == true,
                            sourceSiteId: sourceSs.SiteId,
                            sourceReferenceType: sourceSs.ReferenceType,
                            linkColumn: summary.LinkColumn,
                            type: summary.Type,
                            sourceColumn: summary.SourceColumn,
                            sourceCondition: sourceSs.Views?.Get(summary.SourceCondition),
                            id: IssueId)));
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

        public void UpdateFormulaColumns(
            Context context, SiteSettings ss, IEnumerable<int> selected = null)
        {
            SetByFormula(context: context, ss: ss);
            var param = Rds.IssuesParam();
            ss.Formulas?
                .Where(o => selected == null || selected.Contains(o.Id))
                .ForEach(formulaSet =>
                {
                    switch (formulaSet.Target)
                    {
                        case "WorkValue": param.WorkValue(WorkValue.Value); break;
                        case "ProgressRate": param.ProgressRate(ProgressRate.Value); break;
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
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateIssues(
                    param: param,
                    where: Rds.IssuesWhereDefault(this),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        public void SetByFormula(Context context, SiteSettings ss)
        {
            ss.Formulas?.ForEach(formulaSet =>
            {
                var columnName = formulaSet.Target;
                var formula = formulaSet.Formula;
                var view = ss.Views?.Get(formulaSet.Condition);
                if (view != null && !Matched(context: context, ss: ss, view: view))
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
                    { "ProgressRate", ProgressRate.Value },
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
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "ProgressRate":
                        ProgressRate.Value = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumA":
                        NumA = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumB":
                        NumB = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumC":
                        NumC = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumD":
                        NumD = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumE":
                        NumE = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumF":
                        NumF = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumG":
                        NumG = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumH":
                        NumH = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumI":
                        NumI = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumJ":
                        NumJ = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumK":
                        NumK = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumL":
                        NumL = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumM":
                        NumM = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumN":
                        NumN = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumO":
                        NumO = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumP":
                        NumP = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumQ":
                        NumQ = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumR":
                        NumR = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumS":
                        NumS = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumT":
                        NumT = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumU":
                        NumU = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumV":
                        NumV = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumW":
                        NumW = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumX":
                        NumX = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumY":
                        NumY = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    case "NumZ":
                        NumZ = formula?.GetResult(
                            data, ss.GetColumn(context: context, columnName: columnName)) ?? 0;
                        break;
                    default: break;
                }
            });
        }

        public void SetTitle(Context context, SiteSettings ss)
        {
            Title = new Title(
                context: context,
                ss: ss,
                id: IssueId,
                data: PropertyValues(
                    context: context,
                    names: ss.TitleColumns));
        }

        private bool Matched(Context context, SiteSettings ss, View view)
        {
            if (view.Incomplete == true && !Status.Incomplete())
            {
                return false;
            }
            var userId = context.UserId;
            if (view.Own == true && !(Manager.Id == userId || Owner.Id == userId))
            {
                return false;
            }
            if (view.NearCompletionTime == true && !CompletionTime.Near(
                context: context,
                ss: ss))
            {
                return false;
            }
            if (view.Delay == true && !ProgressRate.Delay(
                context: context,
                status: Status))
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
                    var column = ss.GetColumn(context: context, columnName: filter.Key);
                    switch (filter.Key)
                    {
                        case "UpdatedTime": match = UpdatedTime.Value.Matched(column, filter.Value); break;
                        case "Title": match = Title.Value.Matched(column, filter.Value); break;
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
                        case "DescriptionA": match = DescriptionA.Matched(column, filter.Value); break;
                        case "DescriptionB": match = DescriptionB.Matched(column, filter.Value); break;
                        case "DescriptionC": match = DescriptionC.Matched(column, filter.Value); break;
                        case "DescriptionD": match = DescriptionD.Matched(column, filter.Value); break;
                        case "DescriptionE": match = DescriptionE.Matched(column, filter.Value); break;
                        case "DescriptionF": match = DescriptionF.Matched(column, filter.Value); break;
                        case "DescriptionG": match = DescriptionG.Matched(column, filter.Value); break;
                        case "DescriptionH": match = DescriptionH.Matched(column, filter.Value); break;
                        case "DescriptionI": match = DescriptionI.Matched(column, filter.Value); break;
                        case "DescriptionJ": match = DescriptionJ.Matched(column, filter.Value); break;
                        case "DescriptionK": match = DescriptionK.Matched(column, filter.Value); break;
                        case "DescriptionL": match = DescriptionL.Matched(column, filter.Value); break;
                        case "DescriptionM": match = DescriptionM.Matched(column, filter.Value); break;
                        case "DescriptionN": match = DescriptionN.Matched(column, filter.Value); break;
                        case "DescriptionO": match = DescriptionO.Matched(column, filter.Value); break;
                        case "DescriptionP": match = DescriptionP.Matched(column, filter.Value); break;
                        case "DescriptionQ": match = DescriptionQ.Matched(column, filter.Value); break;
                        case "DescriptionR": match = DescriptionR.Matched(column, filter.Value); break;
                        case "DescriptionS": match = DescriptionS.Matched(column, filter.Value); break;
                        case "DescriptionT": match = DescriptionT.Matched(column, filter.Value); break;
                        case "DescriptionU": match = DescriptionU.Matched(column, filter.Value); break;
                        case "DescriptionV": match = DescriptionV.Matched(column, filter.Value); break;
                        case "DescriptionW": match = DescriptionW.Matched(column, filter.Value); break;
                        case "DescriptionX": match = DescriptionX.Matched(column, filter.Value); break;
                        case "DescriptionY": match = DescriptionY.Matched(column, filter.Value); break;
                        case "DescriptionZ": match = DescriptionZ.Matched(column, filter.Value); break;
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

        private void CheckNotificationConditions(Context context, SiteSettings ss, bool before = false)
        {
            if (ss.GetNotifications(context: context).Any())
            {
                ss.GetNotifications(context: context)?.CheckConditions(
                    views: ss.Views,
                    before: before,
                    dataSet: Rds.ExecuteDataSet(
                        context: context,
                        statements: ss.GetNotifications(context: context).Select((o, i) =>
                            Rds.SelectIssues(
                                column: Rds.IssuesColumn().IssueId(),
                                where: ss.Views?.Get(before
                                    ? o.BeforeCondition
                                    : o.AfterCondition)?
                                        .Where(
                                            context: context,
                                            ss: ss,
                                            where: Rds.IssuesWhere().IssueId(IssueId)) ??
                                                Rds.IssuesWhere().IssueId(IssueId)))
                                                    .ToArray()));
            }
        }

        private void Notice(Context context, SiteSettings ss, string type)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: IssueId);
            ss.GetNotifications(context: context).ForEach(notification =>
            {
                if (notification.HasRelatedUsers())
                {
                    var users = new List<long>();
                    Rds.ExecuteTable(
                        context: context,
                        statements: Rds.SelectIssues(
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
                    notification.ReplaceRelatedUsers(
                        context: context,
                        users: users);
                }
                switch (type)
                {
                    case "Created":
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Created(
                                context: context,
                                data: Title.DisplayValue).ToString(),
                            url: url,
                            body: NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification));
                        break;
                    case "Updated":
                        var body = NoticeBody(
                            context: context,
                            ss: ss,
                            notification: notification, update: true);
                        if (body.Length > 0)
                        {
                            notification.Send(
                                context: context,
                                ss: ss,
                                title: Displays.Updated(
                                    context: context,
                                    data: Title.DisplayValue).ToString(),
                                url: url,
                                body: body);
                        }
                        break;
                    case "Deleted":
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Deleted(
                                context: context,
                                data: Title.DisplayValue).ToString(),
                            url: url,
                            body: NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification));
                        break;
                }
            });
        }

        private string NoticeBody(
            Context context, SiteSettings ss, Notification notification, bool update = false)
        {
            var body = new System.Text.StringBuilder();
            notification.ColumnCollection(context, ss, update)?.ForEach(column =>
            {
                switch (column.Name)
                {
                    case "Title":
                        body.Append(Title.ToNotice(
                            context: context,
                            saved: SavedTitle,
                            column: column,
                            updated: Title_Updated(context: context),
                            update: update));
                        break;
                    case "Body":
                        body.Append(Body.ToNotice(
                            context: context,
                            saved: SavedBody,
                            column: column,
                            updated: Body_Updated(context: context),
                            update: update));
                        break;
                    case "StartTime":
                        body.Append(StartTime.ToNotice(
                            context: context,
                            saved: SavedStartTime,
                            column: column,
                            updated: StartTime_Updated(context: context),
                            update: update));
                        break;
                    case "CompletionTime":
                        body.Append(CompletionTime.ToNotice(
                            context: context,
                            saved: SavedCompletionTime,
                            column: column,
                            updated: CompletionTime_Updated(context: context),
                            update: update));
                        break;
                    case "WorkValue":
                        body.Append(WorkValue.ToNotice(
                            context: context,
                            saved: SavedWorkValue,
                            column: column,
                            updated: WorkValue_Updated(context: context),
                            update: update));
                        break;
                    case "ProgressRate":
                        body.Append(ProgressRate.ToNotice(
                            context: context,
                            saved: SavedProgressRate,
                            column: column,
                            updated: ProgressRate_Updated(context: context),
                            update: update));
                        break;
                    case "Status":
                        body.Append(Status.ToNotice(
                            context: context,
                            saved: SavedStatus,
                            column: column,
                            updated: Status_Updated(context: context),
                            update: update));
                        break;
                    case "Manager":
                        body.Append(Manager.ToNotice(
                            context: context,
                            saved: SavedManager,
                            column: column,
                            updated: Manager_Updated(context: context),
                            update: update));
                        break;
                    case "Owner":
                        body.Append(Owner.ToNotice(
                            context: context,
                            saved: SavedOwner,
                            column: column,
                            updated: Owner_Updated(context: context),
                            update: update));
                        break;
                    case "ClassA":
                        body.Append(ClassA.ToNotice(
                            context: context,
                            saved: SavedClassA,
                            column: column,
                            updated: ClassA_Updated(context: context),
                            update: update));
                        break;
                    case "ClassB":
                        body.Append(ClassB.ToNotice(
                            context: context,
                            saved: SavedClassB,
                            column: column,
                            updated: ClassB_Updated(context: context),
                            update: update));
                        break;
                    case "ClassC":
                        body.Append(ClassC.ToNotice(
                            context: context,
                            saved: SavedClassC,
                            column: column,
                            updated: ClassC_Updated(context: context),
                            update: update));
                        break;
                    case "ClassD":
                        body.Append(ClassD.ToNotice(
                            context: context,
                            saved: SavedClassD,
                            column: column,
                            updated: ClassD_Updated(context: context),
                            update: update));
                        break;
                    case "ClassE":
                        body.Append(ClassE.ToNotice(
                            context: context,
                            saved: SavedClassE,
                            column: column,
                            updated: ClassE_Updated(context: context),
                            update: update));
                        break;
                    case "ClassF":
                        body.Append(ClassF.ToNotice(
                            context: context,
                            saved: SavedClassF,
                            column: column,
                            updated: ClassF_Updated(context: context),
                            update: update));
                        break;
                    case "ClassG":
                        body.Append(ClassG.ToNotice(
                            context: context,
                            saved: SavedClassG,
                            column: column,
                            updated: ClassG_Updated(context: context),
                            update: update));
                        break;
                    case "ClassH":
                        body.Append(ClassH.ToNotice(
                            context: context,
                            saved: SavedClassH,
                            column: column,
                            updated: ClassH_Updated(context: context),
                            update: update));
                        break;
                    case "ClassI":
                        body.Append(ClassI.ToNotice(
                            context: context,
                            saved: SavedClassI,
                            column: column,
                            updated: ClassI_Updated(context: context),
                            update: update));
                        break;
                    case "ClassJ":
                        body.Append(ClassJ.ToNotice(
                            context: context,
                            saved: SavedClassJ,
                            column: column,
                            updated: ClassJ_Updated(context: context),
                            update: update));
                        break;
                    case "ClassK":
                        body.Append(ClassK.ToNotice(
                            context: context,
                            saved: SavedClassK,
                            column: column,
                            updated: ClassK_Updated(context: context),
                            update: update));
                        break;
                    case "ClassL":
                        body.Append(ClassL.ToNotice(
                            context: context,
                            saved: SavedClassL,
                            column: column,
                            updated: ClassL_Updated(context: context),
                            update: update));
                        break;
                    case "ClassM":
                        body.Append(ClassM.ToNotice(
                            context: context,
                            saved: SavedClassM,
                            column: column,
                            updated: ClassM_Updated(context: context),
                            update: update));
                        break;
                    case "ClassN":
                        body.Append(ClassN.ToNotice(
                            context: context,
                            saved: SavedClassN,
                            column: column,
                            updated: ClassN_Updated(context: context),
                            update: update));
                        break;
                    case "ClassO":
                        body.Append(ClassO.ToNotice(
                            context: context,
                            saved: SavedClassO,
                            column: column,
                            updated: ClassO_Updated(context: context),
                            update: update));
                        break;
                    case "ClassP":
                        body.Append(ClassP.ToNotice(
                            context: context,
                            saved: SavedClassP,
                            column: column,
                            updated: ClassP_Updated(context: context),
                            update: update));
                        break;
                    case "ClassQ":
                        body.Append(ClassQ.ToNotice(
                            context: context,
                            saved: SavedClassQ,
                            column: column,
                            updated: ClassQ_Updated(context: context),
                            update: update));
                        break;
                    case "ClassR":
                        body.Append(ClassR.ToNotice(
                            context: context,
                            saved: SavedClassR,
                            column: column,
                            updated: ClassR_Updated(context: context),
                            update: update));
                        break;
                    case "ClassS":
                        body.Append(ClassS.ToNotice(
                            context: context,
                            saved: SavedClassS,
                            column: column,
                            updated: ClassS_Updated(context: context),
                            update: update));
                        break;
                    case "ClassT":
                        body.Append(ClassT.ToNotice(
                            context: context,
                            saved: SavedClassT,
                            column: column,
                            updated: ClassT_Updated(context: context),
                            update: update));
                        break;
                    case "ClassU":
                        body.Append(ClassU.ToNotice(
                            context: context,
                            saved: SavedClassU,
                            column: column,
                            updated: ClassU_Updated(context: context),
                            update: update));
                        break;
                    case "ClassV":
                        body.Append(ClassV.ToNotice(
                            context: context,
                            saved: SavedClassV,
                            column: column,
                            updated: ClassV_Updated(context: context),
                            update: update));
                        break;
                    case "ClassW":
                        body.Append(ClassW.ToNotice(
                            context: context,
                            saved: SavedClassW,
                            column: column,
                            updated: ClassW_Updated(context: context),
                            update: update));
                        break;
                    case "ClassX":
                        body.Append(ClassX.ToNotice(
                            context: context,
                            saved: SavedClassX,
                            column: column,
                            updated: ClassX_Updated(context: context),
                            update: update));
                        break;
                    case "ClassY":
                        body.Append(ClassY.ToNotice(
                            context: context,
                            saved: SavedClassY,
                            column: column,
                            updated: ClassY_Updated(context: context),
                            update: update));
                        break;
                    case "ClassZ":
                        body.Append(ClassZ.ToNotice(
                            context: context,
                            saved: SavedClassZ,
                            column: column,
                            updated: ClassZ_Updated(context: context),
                            update: update));
                        break;
                    case "NumA":
                        body.Append(NumA.ToNotice(
                            context: context,
                            saved: SavedNumA,
                            column: column,
                            updated: NumA_Updated(context: context),
                            update: update));
                        break;
                    case "NumB":
                        body.Append(NumB.ToNotice(
                            context: context,
                            saved: SavedNumB,
                            column: column,
                            updated: NumB_Updated(context: context),
                            update: update));
                        break;
                    case "NumC":
                        body.Append(NumC.ToNotice(
                            context: context,
                            saved: SavedNumC,
                            column: column,
                            updated: NumC_Updated(context: context),
                            update: update));
                        break;
                    case "NumD":
                        body.Append(NumD.ToNotice(
                            context: context,
                            saved: SavedNumD,
                            column: column,
                            updated: NumD_Updated(context: context),
                            update: update));
                        break;
                    case "NumE":
                        body.Append(NumE.ToNotice(
                            context: context,
                            saved: SavedNumE,
                            column: column,
                            updated: NumE_Updated(context: context),
                            update: update));
                        break;
                    case "NumF":
                        body.Append(NumF.ToNotice(
                            context: context,
                            saved: SavedNumF,
                            column: column,
                            updated: NumF_Updated(context: context),
                            update: update));
                        break;
                    case "NumG":
                        body.Append(NumG.ToNotice(
                            context: context,
                            saved: SavedNumG,
                            column: column,
                            updated: NumG_Updated(context: context),
                            update: update));
                        break;
                    case "NumH":
                        body.Append(NumH.ToNotice(
                            context: context,
                            saved: SavedNumH,
                            column: column,
                            updated: NumH_Updated(context: context),
                            update: update));
                        break;
                    case "NumI":
                        body.Append(NumI.ToNotice(
                            context: context,
                            saved: SavedNumI,
                            column: column,
                            updated: NumI_Updated(context: context),
                            update: update));
                        break;
                    case "NumJ":
                        body.Append(NumJ.ToNotice(
                            context: context,
                            saved: SavedNumJ,
                            column: column,
                            updated: NumJ_Updated(context: context),
                            update: update));
                        break;
                    case "NumK":
                        body.Append(NumK.ToNotice(
                            context: context,
                            saved: SavedNumK,
                            column: column,
                            updated: NumK_Updated(context: context),
                            update: update));
                        break;
                    case "NumL":
                        body.Append(NumL.ToNotice(
                            context: context,
                            saved: SavedNumL,
                            column: column,
                            updated: NumL_Updated(context: context),
                            update: update));
                        break;
                    case "NumM":
                        body.Append(NumM.ToNotice(
                            context: context,
                            saved: SavedNumM,
                            column: column,
                            updated: NumM_Updated(context: context),
                            update: update));
                        break;
                    case "NumN":
                        body.Append(NumN.ToNotice(
                            context: context,
                            saved: SavedNumN,
                            column: column,
                            updated: NumN_Updated(context: context),
                            update: update));
                        break;
                    case "NumO":
                        body.Append(NumO.ToNotice(
                            context: context,
                            saved: SavedNumO,
                            column: column,
                            updated: NumO_Updated(context: context),
                            update: update));
                        break;
                    case "NumP":
                        body.Append(NumP.ToNotice(
                            context: context,
                            saved: SavedNumP,
                            column: column,
                            updated: NumP_Updated(context: context),
                            update: update));
                        break;
                    case "NumQ":
                        body.Append(NumQ.ToNotice(
                            context: context,
                            saved: SavedNumQ,
                            column: column,
                            updated: NumQ_Updated(context: context),
                            update: update));
                        break;
                    case "NumR":
                        body.Append(NumR.ToNotice(
                            context: context,
                            saved: SavedNumR,
                            column: column,
                            updated: NumR_Updated(context: context),
                            update: update));
                        break;
                    case "NumS":
                        body.Append(NumS.ToNotice(
                            context: context,
                            saved: SavedNumS,
                            column: column,
                            updated: NumS_Updated(context: context),
                            update: update));
                        break;
                    case "NumT":
                        body.Append(NumT.ToNotice(
                            context: context,
                            saved: SavedNumT,
                            column: column,
                            updated: NumT_Updated(context: context),
                            update: update));
                        break;
                    case "NumU":
                        body.Append(NumU.ToNotice(
                            context: context,
                            saved: SavedNumU,
                            column: column,
                            updated: NumU_Updated(context: context),
                            update: update));
                        break;
                    case "NumV":
                        body.Append(NumV.ToNotice(
                            context: context,
                            saved: SavedNumV,
                            column: column,
                            updated: NumV_Updated(context: context),
                            update: update));
                        break;
                    case "NumW":
                        body.Append(NumW.ToNotice(
                            context: context,
                            saved: SavedNumW,
                            column: column,
                            updated: NumW_Updated(context: context),
                            update: update));
                        break;
                    case "NumX":
                        body.Append(NumX.ToNotice(
                            context: context,
                            saved: SavedNumX,
                            column: column,
                            updated: NumX_Updated(context: context),
                            update: update));
                        break;
                    case "NumY":
                        body.Append(NumY.ToNotice(
                            context: context,
                            saved: SavedNumY,
                            column: column,
                            updated: NumY_Updated(context: context),
                            update: update));
                        break;
                    case "NumZ":
                        body.Append(NumZ.ToNotice(
                            context: context,
                            saved: SavedNumZ,
                            column: column,
                            updated: NumZ_Updated(context: context),
                            update: update));
                        break;
                    case "DateA":
                        body.Append(DateA.ToNotice(
                            context: context,
                            saved: SavedDateA,
                            column: column,
                            updated: DateA_Updated(context: context),
                            update: update));
                        break;
                    case "DateB":
                        body.Append(DateB.ToNotice(
                            context: context,
                            saved: SavedDateB,
                            column: column,
                            updated: DateB_Updated(context: context),
                            update: update));
                        break;
                    case "DateC":
                        body.Append(DateC.ToNotice(
                            context: context,
                            saved: SavedDateC,
                            column: column,
                            updated: DateC_Updated(context: context),
                            update: update));
                        break;
                    case "DateD":
                        body.Append(DateD.ToNotice(
                            context: context,
                            saved: SavedDateD,
                            column: column,
                            updated: DateD_Updated(context: context),
                            update: update));
                        break;
                    case "DateE":
                        body.Append(DateE.ToNotice(
                            context: context,
                            saved: SavedDateE,
                            column: column,
                            updated: DateE_Updated(context: context),
                            update: update));
                        break;
                    case "DateF":
                        body.Append(DateF.ToNotice(
                            context: context,
                            saved: SavedDateF,
                            column: column,
                            updated: DateF_Updated(context: context),
                            update: update));
                        break;
                    case "DateG":
                        body.Append(DateG.ToNotice(
                            context: context,
                            saved: SavedDateG,
                            column: column,
                            updated: DateG_Updated(context: context),
                            update: update));
                        break;
                    case "DateH":
                        body.Append(DateH.ToNotice(
                            context: context,
                            saved: SavedDateH,
                            column: column,
                            updated: DateH_Updated(context: context),
                            update: update));
                        break;
                    case "DateI":
                        body.Append(DateI.ToNotice(
                            context: context,
                            saved: SavedDateI,
                            column: column,
                            updated: DateI_Updated(context: context),
                            update: update));
                        break;
                    case "DateJ":
                        body.Append(DateJ.ToNotice(
                            context: context,
                            saved: SavedDateJ,
                            column: column,
                            updated: DateJ_Updated(context: context),
                            update: update));
                        break;
                    case "DateK":
                        body.Append(DateK.ToNotice(
                            context: context,
                            saved: SavedDateK,
                            column: column,
                            updated: DateK_Updated(context: context),
                            update: update));
                        break;
                    case "DateL":
                        body.Append(DateL.ToNotice(
                            context: context,
                            saved: SavedDateL,
                            column: column,
                            updated: DateL_Updated(context: context),
                            update: update));
                        break;
                    case "DateM":
                        body.Append(DateM.ToNotice(
                            context: context,
                            saved: SavedDateM,
                            column: column,
                            updated: DateM_Updated(context: context),
                            update: update));
                        break;
                    case "DateN":
                        body.Append(DateN.ToNotice(
                            context: context,
                            saved: SavedDateN,
                            column: column,
                            updated: DateN_Updated(context: context),
                            update: update));
                        break;
                    case "DateO":
                        body.Append(DateO.ToNotice(
                            context: context,
                            saved: SavedDateO,
                            column: column,
                            updated: DateO_Updated(context: context),
                            update: update));
                        break;
                    case "DateP":
                        body.Append(DateP.ToNotice(
                            context: context,
                            saved: SavedDateP,
                            column: column,
                            updated: DateP_Updated(context: context),
                            update: update));
                        break;
                    case "DateQ":
                        body.Append(DateQ.ToNotice(
                            context: context,
                            saved: SavedDateQ,
                            column: column,
                            updated: DateQ_Updated(context: context),
                            update: update));
                        break;
                    case "DateR":
                        body.Append(DateR.ToNotice(
                            context: context,
                            saved: SavedDateR,
                            column: column,
                            updated: DateR_Updated(context: context),
                            update: update));
                        break;
                    case "DateS":
                        body.Append(DateS.ToNotice(
                            context: context,
                            saved: SavedDateS,
                            column: column,
                            updated: DateS_Updated(context: context),
                            update: update));
                        break;
                    case "DateT":
                        body.Append(DateT.ToNotice(
                            context: context,
                            saved: SavedDateT,
                            column: column,
                            updated: DateT_Updated(context: context),
                            update: update));
                        break;
                    case "DateU":
                        body.Append(DateU.ToNotice(
                            context: context,
                            saved: SavedDateU,
                            column: column,
                            updated: DateU_Updated(context: context),
                            update: update));
                        break;
                    case "DateV":
                        body.Append(DateV.ToNotice(
                            context: context,
                            saved: SavedDateV,
                            column: column,
                            updated: DateV_Updated(context: context),
                            update: update));
                        break;
                    case "DateW":
                        body.Append(DateW.ToNotice(
                            context: context,
                            saved: SavedDateW,
                            column: column,
                            updated: DateW_Updated(context: context),
                            update: update));
                        break;
                    case "DateX":
                        body.Append(DateX.ToNotice(
                            context: context,
                            saved: SavedDateX,
                            column: column,
                            updated: DateX_Updated(context: context),
                            update: update));
                        break;
                    case "DateY":
                        body.Append(DateY.ToNotice(
                            context: context,
                            saved: SavedDateY,
                            column: column,
                            updated: DateY_Updated(context: context),
                            update: update));
                        break;
                    case "DateZ":
                        body.Append(DateZ.ToNotice(
                            context: context,
                            saved: SavedDateZ,
                            column: column,
                            updated: DateZ_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionA":
                        body.Append(DescriptionA.ToNotice(
                            context: context,
                            saved: SavedDescriptionA,
                            column: column,
                            updated: DescriptionA_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionB":
                        body.Append(DescriptionB.ToNotice(
                            context: context,
                            saved: SavedDescriptionB,
                            column: column,
                            updated: DescriptionB_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionC":
                        body.Append(DescriptionC.ToNotice(
                            context: context,
                            saved: SavedDescriptionC,
                            column: column,
                            updated: DescriptionC_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionD":
                        body.Append(DescriptionD.ToNotice(
                            context: context,
                            saved: SavedDescriptionD,
                            column: column,
                            updated: DescriptionD_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionE":
                        body.Append(DescriptionE.ToNotice(
                            context: context,
                            saved: SavedDescriptionE,
                            column: column,
                            updated: DescriptionE_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionF":
                        body.Append(DescriptionF.ToNotice(
                            context: context,
                            saved: SavedDescriptionF,
                            column: column,
                            updated: DescriptionF_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionG":
                        body.Append(DescriptionG.ToNotice(
                            context: context,
                            saved: SavedDescriptionG,
                            column: column,
                            updated: DescriptionG_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionH":
                        body.Append(DescriptionH.ToNotice(
                            context: context,
                            saved: SavedDescriptionH,
                            column: column,
                            updated: DescriptionH_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionI":
                        body.Append(DescriptionI.ToNotice(
                            context: context,
                            saved: SavedDescriptionI,
                            column: column,
                            updated: DescriptionI_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionJ":
                        body.Append(DescriptionJ.ToNotice(
                            context: context,
                            saved: SavedDescriptionJ,
                            column: column,
                            updated: DescriptionJ_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionK":
                        body.Append(DescriptionK.ToNotice(
                            context: context,
                            saved: SavedDescriptionK,
                            column: column,
                            updated: DescriptionK_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionL":
                        body.Append(DescriptionL.ToNotice(
                            context: context,
                            saved: SavedDescriptionL,
                            column: column,
                            updated: DescriptionL_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionM":
                        body.Append(DescriptionM.ToNotice(
                            context: context,
                            saved: SavedDescriptionM,
                            column: column,
                            updated: DescriptionM_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionN":
                        body.Append(DescriptionN.ToNotice(
                            context: context,
                            saved: SavedDescriptionN,
                            column: column,
                            updated: DescriptionN_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionO":
                        body.Append(DescriptionO.ToNotice(
                            context: context,
                            saved: SavedDescriptionO,
                            column: column,
                            updated: DescriptionO_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionP":
                        body.Append(DescriptionP.ToNotice(
                            context: context,
                            saved: SavedDescriptionP,
                            column: column,
                            updated: DescriptionP_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionQ":
                        body.Append(DescriptionQ.ToNotice(
                            context: context,
                            saved: SavedDescriptionQ,
                            column: column,
                            updated: DescriptionQ_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionR":
                        body.Append(DescriptionR.ToNotice(
                            context: context,
                            saved: SavedDescriptionR,
                            column: column,
                            updated: DescriptionR_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionS":
                        body.Append(DescriptionS.ToNotice(
                            context: context,
                            saved: SavedDescriptionS,
                            column: column,
                            updated: DescriptionS_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionT":
                        body.Append(DescriptionT.ToNotice(
                            context: context,
                            saved: SavedDescriptionT,
                            column: column,
                            updated: DescriptionT_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionU":
                        body.Append(DescriptionU.ToNotice(
                            context: context,
                            saved: SavedDescriptionU,
                            column: column,
                            updated: DescriptionU_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionV":
                        body.Append(DescriptionV.ToNotice(
                            context: context,
                            saved: SavedDescriptionV,
                            column: column,
                            updated: DescriptionV_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionW":
                        body.Append(DescriptionW.ToNotice(
                            context: context,
                            saved: SavedDescriptionW,
                            column: column,
                            updated: DescriptionW_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionX":
                        body.Append(DescriptionX.ToNotice(
                            context: context,
                            saved: SavedDescriptionX,
                            column: column,
                            updated: DescriptionX_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionY":
                        body.Append(DescriptionY.ToNotice(
                            context: context,
                            saved: SavedDescriptionY,
                            column: column,
                            updated: DescriptionY_Updated(context: context),
                            update: update));
                        break;
                    case "DescriptionZ":
                        body.Append(DescriptionZ.ToNotice(
                            context: context,
                            saved: SavedDescriptionZ,
                            column: column,
                            updated: DescriptionZ_Updated(context: context),
                            update: update));
                        break;
                    case "CheckA":
                        body.Append(CheckA.ToNotice(
                            context: context,
                            saved: SavedCheckA,
                            column: column,
                            updated: CheckA_Updated(context: context),
                            update: update));
                        break;
                    case "CheckB":
                        body.Append(CheckB.ToNotice(
                            context: context,
                            saved: SavedCheckB,
                            column: column,
                            updated: CheckB_Updated(context: context),
                            update: update));
                        break;
                    case "CheckC":
                        body.Append(CheckC.ToNotice(
                            context: context,
                            saved: SavedCheckC,
                            column: column,
                            updated: CheckC_Updated(context: context),
                            update: update));
                        break;
                    case "CheckD":
                        body.Append(CheckD.ToNotice(
                            context: context,
                            saved: SavedCheckD,
                            column: column,
                            updated: CheckD_Updated(context: context),
                            update: update));
                        break;
                    case "CheckE":
                        body.Append(CheckE.ToNotice(
                            context: context,
                            saved: SavedCheckE,
                            column: column,
                            updated: CheckE_Updated(context: context),
                            update: update));
                        break;
                    case "CheckF":
                        body.Append(CheckF.ToNotice(
                            context: context,
                            saved: SavedCheckF,
                            column: column,
                            updated: CheckF_Updated(context: context),
                            update: update));
                        break;
                    case "CheckG":
                        body.Append(CheckG.ToNotice(
                            context: context,
                            saved: SavedCheckG,
                            column: column,
                            updated: CheckG_Updated(context: context),
                            update: update));
                        break;
                    case "CheckH":
                        body.Append(CheckH.ToNotice(
                            context: context,
                            saved: SavedCheckH,
                            column: column,
                            updated: CheckH_Updated(context: context),
                            update: update));
                        break;
                    case "CheckI":
                        body.Append(CheckI.ToNotice(
                            context: context,
                            saved: SavedCheckI,
                            column: column,
                            updated: CheckI_Updated(context: context),
                            update: update));
                        break;
                    case "CheckJ":
                        body.Append(CheckJ.ToNotice(
                            context: context,
                            saved: SavedCheckJ,
                            column: column,
                            updated: CheckJ_Updated(context: context),
                            update: update));
                        break;
                    case "CheckK":
                        body.Append(CheckK.ToNotice(
                            context: context,
                            saved: SavedCheckK,
                            column: column,
                            updated: CheckK_Updated(context: context),
                            update: update));
                        break;
                    case "CheckL":
                        body.Append(CheckL.ToNotice(
                            context: context,
                            saved: SavedCheckL,
                            column: column,
                            updated: CheckL_Updated(context: context),
                            update: update));
                        break;
                    case "CheckM":
                        body.Append(CheckM.ToNotice(
                            context: context,
                            saved: SavedCheckM,
                            column: column,
                            updated: CheckM_Updated(context: context),
                            update: update));
                        break;
                    case "CheckN":
                        body.Append(CheckN.ToNotice(
                            context: context,
                            saved: SavedCheckN,
                            column: column,
                            updated: CheckN_Updated(context: context),
                            update: update));
                        break;
                    case "CheckO":
                        body.Append(CheckO.ToNotice(
                            context: context,
                            saved: SavedCheckO,
                            column: column,
                            updated: CheckO_Updated(context: context),
                            update: update));
                        break;
                    case "CheckP":
                        body.Append(CheckP.ToNotice(
                            context: context,
                            saved: SavedCheckP,
                            column: column,
                            updated: CheckP_Updated(context: context),
                            update: update));
                        break;
                    case "CheckQ":
                        body.Append(CheckQ.ToNotice(
                            context: context,
                            saved: SavedCheckQ,
                            column: column,
                            updated: CheckQ_Updated(context: context),
                            update: update));
                        break;
                    case "CheckR":
                        body.Append(CheckR.ToNotice(
                            context: context,
                            saved: SavedCheckR,
                            column: column,
                            updated: CheckR_Updated(context: context),
                            update: update));
                        break;
                    case "CheckS":
                        body.Append(CheckS.ToNotice(
                            context: context,
                            saved: SavedCheckS,
                            column: column,
                            updated: CheckS_Updated(context: context),
                            update: update));
                        break;
                    case "CheckT":
                        body.Append(CheckT.ToNotice(
                            context: context,
                            saved: SavedCheckT,
                            column: column,
                            updated: CheckT_Updated(context: context),
                            update: update));
                        break;
                    case "CheckU":
                        body.Append(CheckU.ToNotice(
                            context: context,
                            saved: SavedCheckU,
                            column: column,
                            updated: CheckU_Updated(context: context),
                            update: update));
                        break;
                    case "CheckV":
                        body.Append(CheckV.ToNotice(
                            context: context,
                            saved: SavedCheckV,
                            column: column,
                            updated: CheckV_Updated(context: context),
                            update: update));
                        break;
                    case "CheckW":
                        body.Append(CheckW.ToNotice(
                            context: context,
                            saved: SavedCheckW,
                            column: column,
                            updated: CheckW_Updated(context: context),
                            update: update));
                        break;
                    case "CheckX":
                        body.Append(CheckX.ToNotice(
                            context: context,
                            saved: SavedCheckX,
                            column: column,
                            updated: CheckX_Updated(context: context),
                            update: update));
                        break;
                    case "CheckY":
                        body.Append(CheckY.ToNotice(
                            context: context,
                            saved: SavedCheckY,
                            column: column,
                            updated: CheckY_Updated(context: context),
                            update: update));
                        break;
                    case "CheckZ":
                        body.Append(CheckZ.ToNotice(
                            context: context,
                            saved: SavedCheckZ,
                            column: column,
                            updated: CheckZ_Updated(context: context),
                            update: update));
                        break;
                    case "Comments":
                        body.Append(Comments.ToNotice(
                            context: context,
                            saved: SavedComments,
                            column: column,
                            updated: Comments_Updated(context: context),
                            update: update));
                        break;
                    case "Creator":
                        body.Append(Creator.ToNotice(
                            context: context,
                            saved: SavedCreator,
                            column: column,
                            updated: Creator_Updated(context: context),
                            update: update));
                        break;
                    case "Updator":
                        body.Append(Updator.ToNotice(
                            context: context,
                            saved: SavedUpdator,
                            column: column,
                            updated: Updator_Updated(context: context),
                            update: update));
                        break;
                }
            });
            return body.ToString();
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
            SetChoiceHash(context: context, ss: ss);
        }

        public void SetChoiceHash(Context context, SiteSettings ss)
        {
            ss.GetUseSearchLinks(context: context).ForEach(link =>
            {
                var value = PropertyValue(context: context, name: link.ColumnName);
                if (!value.IsNullOrEmpty() &&
                    ss.GetColumn(context: context, columnName: link.ColumnName)?
                        .ChoiceHash.Any(o => o.Value.Value == value) != true)
                {
                    ss.SetChoiceHash(
                        context: context,
                        columnName: link.ColumnName,
                        selectedValues: value.ToSingleList());
                }
            });
            SetTitle(context: context, ss: ss);
        }

        private void Set(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
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
                                UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
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
                            Title = new Title(context: context, ss: ss, dataRow: dataRow, column: column);
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
                            CompletionTime = new CompletionTime(context: context, ss: ss, dataRow: dataRow, column: column);
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
                            Manager = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedManager = Manager.Id;
                            break;
                        case "Owner":
                            Owner = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
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
                            AttachmentsA = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsA = AttachmentsA.RecordingJson();
                            break;
                        case "AttachmentsB":
                            AttachmentsB = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsB = AttachmentsB.RecordingJson();
                            break;
                        case "AttachmentsC":
                            AttachmentsC = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsC = AttachmentsC.RecordingJson();
                            break;
                        case "AttachmentsD":
                            AttachmentsD = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsD = AttachmentsD.RecordingJson();
                            break;
                        case "AttachmentsE":
                            AttachmentsE = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsE = AttachmentsE.RecordingJson();
                            break;
                        case "AttachmentsF":
                            AttachmentsF = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsF = AttachmentsF.RecordingJson();
                            break;
                        case "AttachmentsG":
                            AttachmentsG = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsG = AttachmentsG.RecordingJson();
                            break;
                        case "AttachmentsH":
                            AttachmentsH = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsH = AttachmentsH.RecordingJson();
                            break;
                        case "AttachmentsI":
                            AttachmentsI = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsI = AttachmentsI.RecordingJson();
                            break;
                        case "AttachmentsJ":
                            AttachmentsJ = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsJ = AttachmentsJ.RecordingJson();
                            break;
                        case "AttachmentsK":
                            AttachmentsK = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsK = AttachmentsK.RecordingJson();
                            break;
                        case "AttachmentsL":
                            AttachmentsL = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsL = AttachmentsL.RecordingJson();
                            break;
                        case "AttachmentsM":
                            AttachmentsM = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsM = AttachmentsM.RecordingJson();
                            break;
                        case "AttachmentsN":
                            AttachmentsN = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsN = AttachmentsN.RecordingJson();
                            break;
                        case "AttachmentsO":
                            AttachmentsO = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsO = AttachmentsO.RecordingJson();
                            break;
                        case "AttachmentsP":
                            AttachmentsP = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsP = AttachmentsP.RecordingJson();
                            break;
                        case "AttachmentsQ":
                            AttachmentsQ = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsQ = AttachmentsQ.RecordingJson();
                            break;
                        case "AttachmentsR":
                            AttachmentsR = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsR = AttachmentsR.RecordingJson();
                            break;
                        case "AttachmentsS":
                            AttachmentsS = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsS = AttachmentsS.RecordingJson();
                            break;
                        case "AttachmentsT":
                            AttachmentsT = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsT = AttachmentsT.RecordingJson();
                            break;
                        case "AttachmentsU":
                            AttachmentsU = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsU = AttachmentsU.RecordingJson();
                            break;
                        case "AttachmentsV":
                            AttachmentsV = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsV = AttachmentsV.RecordingJson();
                            break;
                        case "AttachmentsW":
                            AttachmentsW = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsW = AttachmentsW.RecordingJson();
                            break;
                        case "AttachmentsX":
                            AttachmentsX = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsX = AttachmentsX.RecordingJson();
                            break;
                        case "AttachmentsY":
                            AttachmentsY = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsY = AttachmentsY.RecordingJson();
                            break;
                        case "AttachmentsZ":
                            AttachmentsZ = dataRow.String(column.ColumnName).Deserialize<Attachments>() ?? new Attachments();
                            SavedAttachmentsZ = AttachmentsZ.RecordingJson();
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
            SetTitle(context: context, ss: ss);
        }

        public bool Updated(Context context)
        {
            return
                SiteId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                Title_Updated(context: context) ||
                Body_Updated(context: context) ||
                StartTime_Updated(context: context) ||
                CompletionTime_Updated(context: context) ||
                WorkValue_Updated(context: context) ||
                ProgressRate_Updated(context: context) ||
                Status_Updated(context: context) ||
                Manager_Updated(context: context) ||
                Owner_Updated(context: context) ||
                ClassA_Updated(context: context) ||
                ClassB_Updated(context: context) ||
                ClassC_Updated(context: context) ||
                ClassD_Updated(context: context) ||
                ClassE_Updated(context: context) ||
                ClassF_Updated(context: context) ||
                ClassG_Updated(context: context) ||
                ClassH_Updated(context: context) ||
                ClassI_Updated(context: context) ||
                ClassJ_Updated(context: context) ||
                ClassK_Updated(context: context) ||
                ClassL_Updated(context: context) ||
                ClassM_Updated(context: context) ||
                ClassN_Updated(context: context) ||
                ClassO_Updated(context: context) ||
                ClassP_Updated(context: context) ||
                ClassQ_Updated(context: context) ||
                ClassR_Updated(context: context) ||
                ClassS_Updated(context: context) ||
                ClassT_Updated(context: context) ||
                ClassU_Updated(context: context) ||
                ClassV_Updated(context: context) ||
                ClassW_Updated(context: context) ||
                ClassX_Updated(context: context) ||
                ClassY_Updated(context: context) ||
                ClassZ_Updated(context: context) ||
                NumA_Updated(context: context) ||
                NumB_Updated(context: context) ||
                NumC_Updated(context: context) ||
                NumD_Updated(context: context) ||
                NumE_Updated(context: context) ||
                NumF_Updated(context: context) ||
                NumG_Updated(context: context) ||
                NumH_Updated(context: context) ||
                NumI_Updated(context: context) ||
                NumJ_Updated(context: context) ||
                NumK_Updated(context: context) ||
                NumL_Updated(context: context) ||
                NumM_Updated(context: context) ||
                NumN_Updated(context: context) ||
                NumO_Updated(context: context) ||
                NumP_Updated(context: context) ||
                NumQ_Updated(context: context) ||
                NumR_Updated(context: context) ||
                NumS_Updated(context: context) ||
                NumT_Updated(context: context) ||
                NumU_Updated(context: context) ||
                NumV_Updated(context: context) ||
                NumW_Updated(context: context) ||
                NumX_Updated(context: context) ||
                NumY_Updated(context: context) ||
                NumZ_Updated(context: context) ||
                DateA_Updated(context: context) ||
                DateB_Updated(context: context) ||
                DateC_Updated(context: context) ||
                DateD_Updated(context: context) ||
                DateE_Updated(context: context) ||
                DateF_Updated(context: context) ||
                DateG_Updated(context: context) ||
                DateH_Updated(context: context) ||
                DateI_Updated(context: context) ||
                DateJ_Updated(context: context) ||
                DateK_Updated(context: context) ||
                DateL_Updated(context: context) ||
                DateM_Updated(context: context) ||
                DateN_Updated(context: context) ||
                DateO_Updated(context: context) ||
                DateP_Updated(context: context) ||
                DateQ_Updated(context: context) ||
                DateR_Updated(context: context) ||
                DateS_Updated(context: context) ||
                DateT_Updated(context: context) ||
                DateU_Updated(context: context) ||
                DateV_Updated(context: context) ||
                DateW_Updated(context: context) ||
                DateX_Updated(context: context) ||
                DateY_Updated(context: context) ||
                DateZ_Updated(context: context) ||
                DescriptionA_Updated(context: context) ||
                DescriptionB_Updated(context: context) ||
                DescriptionC_Updated(context: context) ||
                DescriptionD_Updated(context: context) ||
                DescriptionE_Updated(context: context) ||
                DescriptionF_Updated(context: context) ||
                DescriptionG_Updated(context: context) ||
                DescriptionH_Updated(context: context) ||
                DescriptionI_Updated(context: context) ||
                DescriptionJ_Updated(context: context) ||
                DescriptionK_Updated(context: context) ||
                DescriptionL_Updated(context: context) ||
                DescriptionM_Updated(context: context) ||
                DescriptionN_Updated(context: context) ||
                DescriptionO_Updated(context: context) ||
                DescriptionP_Updated(context: context) ||
                DescriptionQ_Updated(context: context) ||
                DescriptionR_Updated(context: context) ||
                DescriptionS_Updated(context: context) ||
                DescriptionT_Updated(context: context) ||
                DescriptionU_Updated(context: context) ||
                DescriptionV_Updated(context: context) ||
                DescriptionW_Updated(context: context) ||
                DescriptionX_Updated(context: context) ||
                DescriptionY_Updated(context: context) ||
                DescriptionZ_Updated(context: context) ||
                CheckA_Updated(context: context) ||
                CheckB_Updated(context: context) ||
                CheckC_Updated(context: context) ||
                CheckD_Updated(context: context) ||
                CheckE_Updated(context: context) ||
                CheckF_Updated(context: context) ||
                CheckG_Updated(context: context) ||
                CheckH_Updated(context: context) ||
                CheckI_Updated(context: context) ||
                CheckJ_Updated(context: context) ||
                CheckK_Updated(context: context) ||
                CheckL_Updated(context: context) ||
                CheckM_Updated(context: context) ||
                CheckN_Updated(context: context) ||
                CheckO_Updated(context: context) ||
                CheckP_Updated(context: context) ||
                CheckQ_Updated(context: context) ||
                CheckR_Updated(context: context) ||
                CheckS_Updated(context: context) ||
                CheckT_Updated(context: context) ||
                CheckU_Updated(context: context) ||
                CheckV_Updated(context: context) ||
                CheckW_Updated(context: context) ||
                CheckX_Updated(context: context) ||
                CheckY_Updated(context: context) ||
                CheckZ_Updated(context: context) ||
                AttachmentsA_Updated(context: context) ||
                AttachmentsB_Updated(context: context) ||
                AttachmentsC_Updated(context: context) ||
                AttachmentsD_Updated(context: context) ||
                AttachmentsE_Updated(context: context) ||
                AttachmentsF_Updated(context: context) ||
                AttachmentsG_Updated(context: context) ||
                AttachmentsH_Updated(context: context) ||
                AttachmentsI_Updated(context: context) ||
                AttachmentsJ_Updated(context: context) ||
                AttachmentsK_Updated(context: context) ||
                AttachmentsL_Updated(context: context) ||
                AttachmentsM_Updated(context: context) ||
                AttachmentsN_Updated(context: context) ||
                AttachmentsO_Updated(context: context) ||
                AttachmentsP_Updated(context: context) ||
                AttachmentsQ_Updated(context: context) ||
                AttachmentsR_Updated(context: context) ||
                AttachmentsS_Updated(context: context) ||
                AttachmentsT_Updated(context: context) ||
                AttachmentsU_Updated(context: context) ||
                AttachmentsV_Updated(context: context) ||
                AttachmentsW_Updated(context: context) ||
                AttachmentsX_Updated(context: context) ||
                AttachmentsY_Updated(context: context) ||
                AttachmentsZ_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        public List<string> Mine(Context context)
        {
            var mine = new List<string>();
            var userId = context.UserId;
            if (SavedManager == userId) mine.Add("Manager");
            if (SavedOwner == userId) mine.Add("Owner");
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }
    }
}
