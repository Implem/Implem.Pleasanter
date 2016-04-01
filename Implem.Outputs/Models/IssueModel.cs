using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
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
        public decimal NumA = 0;
        public decimal NumB = 0;
        public decimal NumC = 0;
        public decimal NumD = 0;
        public decimal NumE = 0;
        public decimal NumF = 0;
        public decimal NumG = 0;
        public decimal NumH = 0;
        public DateTime DateA = 0.ToDateTime();
        public DateTime DateB = 0.ToDateTime();
        public DateTime DateC = 0.ToDateTime();
        public DateTime DateD = 0.ToDateTime();
        public DateTime DateE = 0.ToDateTime();
        public DateTime DateF = 0.ToDateTime();
        public DateTime DateG = 0.ToDateTime();
        public DateTime DateH = 0.ToDateTime();
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
        public decimal SavedNumA = 0;
        public decimal SavedNumB = 0;
        public decimal SavedNumC = 0;
        public decimal SavedNumD = 0;
        public decimal SavedNumE = 0;
        public decimal SavedNumF = 0;
        public decimal SavedNumG = 0;
        public decimal SavedNumH = 0;
        public DateTime SavedDateA = 0.ToDateTime();
        public DateTime SavedDateB = 0.ToDateTime();
        public DateTime SavedDateC = 0.ToDateTime();
        public DateTime SavedDateD = 0.ToDateTime();
        public DateTime SavedDateE = 0.ToDateTime();
        public DateTime SavedDateF = 0.ToDateTime();
        public DateTime SavedDateG = 0.ToDateTime();
        public DateTime SavedDateH = 0.ToDateTime();
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
        public bool NumA_Updated { get { return NumA != SavedNumA; } }
        public bool NumB_Updated { get { return NumB != SavedNumB; } }
        public bool NumC_Updated { get { return NumC != SavedNumC; } }
        public bool NumD_Updated { get { return NumD != SavedNumD; } }
        public bool NumE_Updated { get { return NumE != SavedNumE; } }
        public bool NumF_Updated { get { return NumF != SavedNumF; } }
        public bool NumG_Updated { get { return NumG != SavedNumG; } }
        public bool NumH_Updated { get { return NumH != SavedNumH; } }
        public bool DateA_Updated { get { return DateA != SavedDateA && DateA != null; } }
        public bool DateB_Updated { get { return DateB != SavedDateB && DateB != null; } }
        public bool DateC_Updated { get { return DateC != SavedDateC && DateC != null; } }
        public bool DateD_Updated { get { return DateD != SavedDateD && DateD != null; } }
        public bool DateE_Updated { get { return DateE != SavedDateE && DateE != null; } }
        public bool DateF_Updated { get { return DateF != SavedDateF && DateF != null; } }
        public bool DateG_Updated { get { return DateG != SavedDateG && DateG != null; } }
        public bool DateH_Updated { get { return DateH != SavedDateH && DateH != null; } }
        public List<long> SwitchTargets;

        public IssueModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
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

        public IssueModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            long issueId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            IssueId = issueId;
            PermissionType = permissionType;
            SiteId = SiteSettings.SiteId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public IssueModel(
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
                column: column ?? Rds.IssuesColumnDefault(),
                join: join ??  Rds.IssuesJoinDefault(),
                where: where ?? Rds.IssuesWhereDefault(this),
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
            Get();
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(IssuesUtility.TitleDisplayValue(SiteSettings, this))
                        .Subset(Jsons.ToJson(new IssueSubset(this, SiteSettings))),
                    where: Rds.ItemsWhere().ReferenceId(IssueId)));
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
                    case "Issues_SiteId": if (!SiteSettings.AllColumn("SiteId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_IssueId": if (!SiteSettings.AllColumn("IssueId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Body": if (!SiteSettings.AllColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_TitleBody": if (!SiteSettings.AllColumn("TitleBody").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_StartTime": if (!SiteSettings.AllColumn("StartTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_CompletionTime": if (!SiteSettings.AllColumn("CompletionTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_WorkValue": if (!SiteSettings.AllColumn("WorkValue").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ProgressRate": if (!SiteSettings.AllColumn("ProgressRate").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_RemainingWorkValue": if (!SiteSettings.AllColumn("RemainingWorkValue").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Status": if (!SiteSettings.AllColumn("Status").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Manager": if (!SiteSettings.AllColumn("Manager").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Owner": if (!SiteSettings.AllColumn("Owner").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassA": if (!SiteSettings.AllColumn("ClassA").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassB": if (!SiteSettings.AllColumn("ClassB").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassC": if (!SiteSettings.AllColumn("ClassC").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassD": if (!SiteSettings.AllColumn("ClassD").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassE": if (!SiteSettings.AllColumn("ClassE").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassF": if (!SiteSettings.AllColumn("ClassF").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassG": if (!SiteSettings.AllColumn("ClassG").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassH": if (!SiteSettings.AllColumn("ClassH").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassI": if (!SiteSettings.AllColumn("ClassI").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassJ": if (!SiteSettings.AllColumn("ClassJ").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassK": if (!SiteSettings.AllColumn("ClassK").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassL": if (!SiteSettings.AllColumn("ClassL").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassM": if (!SiteSettings.AllColumn("ClassM").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassN": if (!SiteSettings.AllColumn("ClassN").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassO": if (!SiteSettings.AllColumn("ClassO").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassP": if (!SiteSettings.AllColumn("ClassP").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumA": if (!SiteSettings.AllColumn("NumA").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumB": if (!SiteSettings.AllColumn("NumB").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumC": if (!SiteSettings.AllColumn("NumC").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumD": if (!SiteSettings.AllColumn("NumD").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumE": if (!SiteSettings.AllColumn("NumE").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumF": if (!SiteSettings.AllColumn("NumF").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumG": if (!SiteSettings.AllColumn("NumG").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumH": if (!SiteSettings.AllColumn("NumH").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateA": if (!SiteSettings.AllColumn("DateA").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateB": if (!SiteSettings.AllColumn("DateB").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateC": if (!SiteSettings.AllColumn("DateC").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateD": if (!SiteSettings.AllColumn("DateD").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateE": if (!SiteSettings.AllColumn("DateE").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateF": if (!SiteSettings.AllColumn("DateF").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateG": if (!SiteSettings.AllColumn("DateG").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateH": if (!SiteSettings.AllColumn("DateH").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                    Rds.UpdateIssues(
                        verUp: VerUp,
                        where: Rds.IssuesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.IssuesParamDefault(this, paramAll: paramAll),
                        countRecord: true),
                    Rds.Conditions("@@rowcount = 1"),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(IssueId),
                        param: Rds.ItemsParam()
                            .SiteId(SiteId)
                            .Title(IssuesUtility.TitleDisplayValue(SiteSettings, this))
                            .Subset(Jsons.ToJson(new IssueSubset(this, SiteSettings)))
                            .UpdateTarget(true)),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(IssueId)),
                    InsertLinks(SiteSettings),
                    Rds.End()
                });
            if (count == 0) return ResponseConflicts();
            SynchronizeSummary();
            Get();
            var responseCollection = new IssuesResponseCollection(this);
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
                    default: break;
                }
            });
            return LinksUtility.Insert(link, selectIdentity);
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnUpdated(ref IssuesResponseCollection responseCollection)
        {
            var column = SiteSettings.AllColumn("RemainingWorkValue");
            responseCollection.Val(
                "#Issues_RemainingWorkValue",
                RemainingWorkValue.ToString(column.StringFormat) + column.Unit);
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
                    case "Issues_SiteId": if (!SiteSettings.AllColumn("SiteId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_IssueId": if (!SiteSettings.AllColumn("IssueId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Body": if (!SiteSettings.AllColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_TitleBody": if (!SiteSettings.AllColumn("TitleBody").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_StartTime": if (!SiteSettings.AllColumn("StartTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_CompletionTime": if (!SiteSettings.AllColumn("CompletionTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_WorkValue": if (!SiteSettings.AllColumn("WorkValue").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ProgressRate": if (!SiteSettings.AllColumn("ProgressRate").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_RemainingWorkValue": if (!SiteSettings.AllColumn("RemainingWorkValue").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Status": if (!SiteSettings.AllColumn("Status").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Manager": if (!SiteSettings.AllColumn("Manager").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Owner": if (!SiteSettings.AllColumn("Owner").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassA": if (!SiteSettings.AllColumn("ClassA").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassB": if (!SiteSettings.AllColumn("ClassB").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassC": if (!SiteSettings.AllColumn("ClassC").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassD": if (!SiteSettings.AllColumn("ClassD").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassE": if (!SiteSettings.AllColumn("ClassE").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassF": if (!SiteSettings.AllColumn("ClassF").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassG": if (!SiteSettings.AllColumn("ClassG").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassH": if (!SiteSettings.AllColumn("ClassH").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassI": if (!SiteSettings.AllColumn("ClassI").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassJ": if (!SiteSettings.AllColumn("ClassJ").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassK": if (!SiteSettings.AllColumn("ClassK").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassL": if (!SiteSettings.AllColumn("ClassL").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassM": if (!SiteSettings.AllColumn("ClassM").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassN": if (!SiteSettings.AllColumn("ClassN").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassO": if (!SiteSettings.AllColumn("ClassO").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_ClassP": if (!SiteSettings.AllColumn("ClassP").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumA": if (!SiteSettings.AllColumn("NumA").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumB": if (!SiteSettings.AllColumn("NumB").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumC": if (!SiteSettings.AllColumn("NumC").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumD": if (!SiteSettings.AllColumn("NumD").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumE": if (!SiteSettings.AllColumn("NumE").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumF": if (!SiteSettings.AllColumn("NumF").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumG": if (!SiteSettings.AllColumn("NumG").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_NumH": if (!SiteSettings.AllColumn("NumH").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateA": if (!SiteSettings.AllColumn("DateA").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateB": if (!SiteSettings.AllColumn("DateB").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateC": if (!SiteSettings.AllColumn("DateC").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateD": if (!SiteSettings.AllColumn("DateD").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateE": if (!SiteSettings.AllColumn("DateE").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateF": if (!SiteSettings.AllColumn("DateF").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateG": if (!SiteSettings.AllColumn("DateG").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_DateH": if (!SiteSettings.AllColumn("DateH").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Issues_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(IssuesResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.DisplayValue + " - " + Displays.Edit())
                .Html("#RecordInfo", Html.Builder().RecordInfo(baseModel: this, tableName: "Issues"))
                .Html("#RecordHistories", Html.Builder().RecordHistories(ver: Ver, verType: VerType))
                .Html("#Links", Html.Builder().Links(IssueId))
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
            var responseCollection = new IssuesResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref IssuesResponseCollection responseCollection)
        {
        }

        public string Copy()
        {
            IssueId = 0;
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
                    where: Rds.ItemsWhere().ReferenceId(IssueId),
                    param: Rds.ItemsParam().SiteId(SiteId)),
                Rds.UpdateIssues(
                    where: Rds.IssuesWhere().IssueId(IssueId),
                    param: Rds.IssuesParam().SiteId(SiteId))
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
                        where: Rds.ItemsWhere().ReferenceId(IssueId)),
                    Rds.DeleteIssues(
                        where: Rds.IssuesWhere().SiteId(SiteId).IssueId(IssueId))
                });
            var responseCollection = new IssuesResponseCollection(this);
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

        private void OnDeleted(ref IssuesResponseCollection responseCollection)
        {
        }

        public string Restore(long issueId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            IssueId = issueId;
            Rds.ExecuteNonQuery(
                connectionString: Def.Db.DbOwner,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(IssueId)),
                    Rds.RestoreIssues(
                        where: Rds.IssuesWhere().IssueId(IssueId))
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
                statements: Rds.PhysicalDeleteIssues(
                    tableType: tableType,
                    param: Rds.IssuesParam().SiteId(SiteId).IssueId(IssueId)));
            var responseCollection = new IssuesResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref IssuesResponseCollection responseCollection)
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
                "Issues",
                summary.LinkColumn,
                summary.Type,
                summary.SourceColumn,
                id);
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
                default: return 0;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string EditSeparateSettings()
        {
            return new ResponseCollection()
                .Html(
                    "#Dialog_SeparateSettings",
                    Html.Builder().SeparateSettings(
                        Title.Value,
                        WorkValue.Value,
                        SiteSettings.AllColumn("WorkValue").Unit))
                .Func("separate")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Separate()
        {
            var number = Forms.Int("SeparateNumber");
            if (number >= 2)
            {
                var idHash = new Dictionary<int, long> { { 1, IssueId } };
                var timestampHash = new Dictionary<int, string> { { 1, Timestamp } };
                var comments = Comments.ToJson();
                for (var index = 2; index <= number; index++)
                {
                    IssueId = 0;
                    Create(paramAll: true);
                    idHash.Add(index, IssueId);
                    timestampHash.Add(index, Timestamp);
                }
                var addCommentCollection = new List<string> { Displays.Separated() };
                addCommentCollection.AddRange(idHash.Select(o => "[{0}]({1}{2})".Params(
                    Forms.Data("SeparateTitle_" + o.Key),
                    Url.Server(),
                    Navigations.ItemEdit(o.Value))));
                var addComment = addCommentCollection.Join("\n");
                for (var index = number; index >= 1; index--)
                {
                    var source = index == 1;
                    IssueId = idHash[index];
                    Timestamp = timestampHash[index];
                    Title.Value = Forms.Data("SeparateTitle_" + index);
                    WorkValue.Value = source
                        ? Forms.Decimal("SourceWorkValue")
                        : Forms.Decimal("SeparateWorkValue_" + index);
                    Comments.Clear();
                    if (source || Forms.Bool("SeparateCopyWithComments"))
                    {
                        Comments = comments.Deserialize<Comments>();
                    }
                    Comments.Prepend(addComment);
                    Update(paramAll: true);
                }
                return RecordResponse(this, Messages.Separated());
            }
            else
            {
                return Messages.ResponseInvalidRequest().ToJson();
            }
        }

        public string Histories()
        {
            var hb = Html.Builder();
            hb.Table(
                attributes: Html.Attributes().Class("grid"),
                action: () =>
                {
                    hb.GridHeader(
                        columnCollection: SiteSettings.HistoryGridColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new IssueCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.IssuesWhere().IssueId(IssueId),
                        orderBy: Rds.IssuesOrderBy().UpdatedTime(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(issueModel => hb
                            .Tr(
                                attributes: Html.Attributes()
                                    .Class("grid-row not-link")
                                    .OnClick(Def.JavaScript.HistoryAndCloseDialog
                                        .Params(issueModel.Ver))
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-latest", 1, _using: issueModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryGridColumnCollection().ForEach(column =>
                                        hb.TdValue(column, issueModel))));
                });
            return new IssuesResponseCollection(this).Html("#HistoriesForm", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.IssuesWhere()
                    .IssueId(IssueId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerType(IssueId);
            SwitchTargets = IssuesUtility.GetSwitchTargets(SiteSettings, SiteId);
            return Editor();
        }

        public string PreviousHistory()
        {
            Get(
                where: Rds.IssuesWhere()
                    .IssueId(IssueId)
                    .Ver(Forms.Int("Ver"), _operator: "<"),
                orderBy: Rds.IssuesOrderBy()
                    .Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = IssuesUtility.GetSwitchTargets(SiteSettings, SiteId);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(IssueId, Versions.DirectioTypes.Previous);
                    return Editor();
                default:
                    return new IssuesResponseCollection(this).ToJson();
            }
        }

        public string NextHistory()
        {
            Get(
                where: Rds.IssuesWhere()
                    .IssueId(IssueId)
                    .Ver(Forms.Int("Ver"), _operator: ">"),
                orderBy: Rds.IssuesOrderBy()
                    .Ver(SqlOrderBy.Types.asc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = IssuesUtility.GetSwitchTargets(SiteSettings, SiteId);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(IssueId, Versions.DirectioTypes.Next);
                    return Editor();
                default:
                    return new IssuesResponseCollection(this).ToJson();
            }
        }

        public string Previous()
        {
            var switchTargets = IssuesUtility.GetSwitchTargets(SiteSettings, SiteId);
            var issueModel = new IssueModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                issueId: switchTargets.Previous(IssueId),
                switchTargets: switchTargets);
            return RecordResponse(issueModel);
        }

        public string Next()
        {
            var switchTargets = IssuesUtility.GetSwitchTargets(SiteSettings, SiteId);
            var issueModel = new IssueModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                issueId: switchTargets.Next(IssueId),
                switchTargets: switchTargets);
            return RecordResponse(issueModel);
        }

        public string Reload()
        {
            SwitchTargets = IssuesUtility.GetSwitchTargets(SiteSettings, SiteId);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            IssueModel issueModel, Message message = null, bool pushState = true)
        {
            var siteModel = new SiteModel(SiteId);
            issueModel.MethodType = BaseModel.MethodTypes.Edit;
            return new IssuesResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    issueModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? IssuesUtility.Editor(siteModel, issueModel)
                        : IssuesUtility.Editor(siteModel, this))
                .Message(message)
                .PushState(
                    Navigations.Get("Items", IssueId.ToString(), "Reload"),
                    Navigations.Edit("Items", issueModel.IssueId),
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
                    case "Issues_Title": Title = new Title(IssueId, Forms.Data(controlId)); break;
                    case "Issues_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Issues_StartTime": StartTime = Forms.DateTime(controlId).ToUniversal(); ProgressRate.StartTime = StartTime; break;
                    case "Issues_CompletionTime": CompletionTime = new CompletionTime(Forms.Data(controlId).ToDateTime(), Status, byForm: true); ProgressRate.CompletionTime = CompletionTime.Value; break;
                    case "Issues_WorkValue": WorkValue = new WorkValue(Forms.Decimal(controlId), ProgressRate.Value); break;
                    case "Issues_ProgressRate": ProgressRate = new ProgressRate(CreatedTime, StartTime, CompletionTime, Forms.Decimal(controlId)); WorkValue.ProgressRate = ProgressRate.Value; break;
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
                    case "Issues_NumA": NumA = Forms.Data(controlId).ToDecimal(); break;
                    case "Issues_NumB": NumB = Forms.Data(controlId).ToDecimal(); break;
                    case "Issues_NumC": NumC = Forms.Data(controlId).ToDecimal(); break;
                    case "Issues_NumD": NumD = Forms.Data(controlId).ToDecimal(); break;
                    case "Issues_NumE": NumE = Forms.Data(controlId).ToDecimal(); break;
                    case "Issues_NumF": NumF = Forms.Data(controlId).ToDecimal(); break;
                    case "Issues_NumG": NumG = Forms.Data(controlId).ToDecimal(); break;
                    case "Issues_NumH": NumH = Forms.Data(controlId).ToDecimal(); break;
                    case "Issues_DateA": DateA = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateB": DateB = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateC": DateC = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateD": DateD = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateE": DateE = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateF": DateF = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateG": DateG = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_DateH": DateH = Forms.Data(controlId).ToDateTime().ToUniversal(); break;
                    case "Issues_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                    case "NumA": NumA = dataRow[name].ToDecimal(); SavedNumA = NumA; break;
                    case "NumB": NumB = dataRow[name].ToDecimal(); SavedNumB = NumB; break;
                    case "NumC": NumC = dataRow[name].ToDecimal(); SavedNumC = NumC; break;
                    case "NumD": NumD = dataRow[name].ToDecimal(); SavedNumD = NumD; break;
                    case "NumE": NumE = dataRow[name].ToDecimal(); SavedNumE = NumE; break;
                    case "NumF": NumF = dataRow[name].ToDecimal(); SavedNumF = NumF; break;
                    case "NumG": NumG = dataRow[name].ToDecimal(); SavedNumG = NumG; break;
                    case "NumH": NumH = dataRow[name].ToDecimal(); SavedNumH = NumH; break;
                    case "DateA": DateA = dataRow[name].ToDateTime(); SavedDateA = DateA; break;
                    case "DateB": DateB = dataRow[name].ToDateTime(); SavedDateB = DateB; break;
                    case "DateC": DateC = dataRow[name].ToDateTime(); SavedDateC = DateC; break;
                    case "DateD": DateD = dataRow[name].ToDateTime(); SavedDateD = DateD; break;
                    case "DateE": DateE = dataRow[name].ToDateTime(); SavedDateE = DateE; break;
                    case "DateF": DateF = dataRow[name].ToDateTime(); SavedDateF = DateF; break;
                    case "DateG": DateG = dataRow[name].ToDateTime(); SavedDateG = DateG; break;
                    case "DateH": DateH = dataRow[name].ToDateTime(); SavedDateH = DateH; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                }
            }
            if (SiteSettings != null)
            {
                Title.DisplayValue = IssuesUtility.TitleDisplayValue(SiteSettings, this);
            }
        }

        private string Editor()
        {
            var siteModel = new SiteModel(SiteId);
            return new IssuesResponseCollection(this)
                .Html("#MainContainer", IssuesUtility.Editor(siteModel, this))
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

    public class IssueSubset
    {
        public long SiteId;
        public string SiteId_LabelText;
        public long IssueId;
        public string IssueId_LabelText;
        public int Ver;
        public string Ver_LabelText;
        public Title Title;
        public string Title_LabelText;
        public string Body;
        public string Body_LabelText;
        public TitleBody TitleBody;
        public string TitleBody_LabelText;
        public DateTime StartTime;
        public string StartTime_LabelText;
        public CompletionTime CompletionTime;
        public string CompletionTime_LabelText;
        public WorkValue WorkValue;
        public string WorkValue_LabelText;
        public ProgressRate ProgressRate;
        public string ProgressRate_LabelText;
        public decimal RemainingWorkValue;
        public string RemainingWorkValue_LabelText;
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
        public Comments Comments;
        public string Comments_LabelText;
        public User Creator;
        public string Creator_LabelText;
        public User Updator;
        public string Updator_LabelText;

        public IssueSubset()
        {
        }

        public IssueSubset(IssueModel issueModel, SiteSettings siteSettings)
        {
            SiteId = issueModel.SiteId;
            SiteId_LabelText = siteSettings.EditorColumn("SiteId")?.LabelText;
            IssueId = issueModel.IssueId;
            IssueId_LabelText = siteSettings.EditorColumn("IssueId")?.LabelText;
            Ver = issueModel.Ver;
            Ver_LabelText = siteSettings.EditorColumn("Ver")?.LabelText;
            Title = issueModel.Title;
            Title_LabelText = siteSettings.EditorColumn("Title")?.LabelText;
            Body = issueModel.Body;
            Body_LabelText = siteSettings.EditorColumn("Body")?.LabelText;
            TitleBody = issueModel.TitleBody;
            TitleBody_LabelText = siteSettings.EditorColumn("TitleBody")?.LabelText;
            StartTime = issueModel.StartTime;
            StartTime_LabelText = siteSettings.EditorColumn("StartTime")?.LabelText;
            CompletionTime = issueModel.CompletionTime;
            CompletionTime_LabelText = siteSettings.EditorColumn("CompletionTime")?.LabelText;
            WorkValue = issueModel.WorkValue;
            WorkValue_LabelText = siteSettings.EditorColumn("WorkValue")?.LabelText;
            ProgressRate = issueModel.ProgressRate;
            ProgressRate_LabelText = siteSettings.EditorColumn("ProgressRate")?.LabelText;
            RemainingWorkValue = issueModel.RemainingWorkValue;
            RemainingWorkValue_LabelText = siteSettings.EditorColumn("RemainingWorkValue")?.LabelText;
            Status = issueModel.Status;
            Status_LabelText = siteSettings.EditorColumn("Status")?.LabelText;
            Manager = issueModel.Manager;
            Manager_LabelText = siteSettings.EditorColumn("Manager")?.LabelText;
            Owner = issueModel.Owner;
            Owner_LabelText = siteSettings.EditorColumn("Owner")?.LabelText;
            ClassA = issueModel.ClassA;
            ClassA_LabelText = siteSettings.EditorColumn("ClassA")?.LabelText;
            ClassB = issueModel.ClassB;
            ClassB_LabelText = siteSettings.EditorColumn("ClassB")?.LabelText;
            ClassC = issueModel.ClassC;
            ClassC_LabelText = siteSettings.EditorColumn("ClassC")?.LabelText;
            ClassD = issueModel.ClassD;
            ClassD_LabelText = siteSettings.EditorColumn("ClassD")?.LabelText;
            ClassE = issueModel.ClassE;
            ClassE_LabelText = siteSettings.EditorColumn("ClassE")?.LabelText;
            ClassF = issueModel.ClassF;
            ClassF_LabelText = siteSettings.EditorColumn("ClassF")?.LabelText;
            ClassG = issueModel.ClassG;
            ClassG_LabelText = siteSettings.EditorColumn("ClassG")?.LabelText;
            ClassH = issueModel.ClassH;
            ClassH_LabelText = siteSettings.EditorColumn("ClassH")?.LabelText;
            ClassI = issueModel.ClassI;
            ClassI_LabelText = siteSettings.EditorColumn("ClassI")?.LabelText;
            ClassJ = issueModel.ClassJ;
            ClassJ_LabelText = siteSettings.EditorColumn("ClassJ")?.LabelText;
            ClassK = issueModel.ClassK;
            ClassK_LabelText = siteSettings.EditorColumn("ClassK")?.LabelText;
            ClassL = issueModel.ClassL;
            ClassL_LabelText = siteSettings.EditorColumn("ClassL")?.LabelText;
            ClassM = issueModel.ClassM;
            ClassM_LabelText = siteSettings.EditorColumn("ClassM")?.LabelText;
            ClassN = issueModel.ClassN;
            ClassN_LabelText = siteSettings.EditorColumn("ClassN")?.LabelText;
            ClassO = issueModel.ClassO;
            ClassO_LabelText = siteSettings.EditorColumn("ClassO")?.LabelText;
            ClassP = issueModel.ClassP;
            ClassP_LabelText = siteSettings.EditorColumn("ClassP")?.LabelText;
            NumA = issueModel.NumA;
            NumA_LabelText = siteSettings.EditorColumn("NumA")?.LabelText;
            NumB = issueModel.NumB;
            NumB_LabelText = siteSettings.EditorColumn("NumB")?.LabelText;
            NumC = issueModel.NumC;
            NumC_LabelText = siteSettings.EditorColumn("NumC")?.LabelText;
            NumD = issueModel.NumD;
            NumD_LabelText = siteSettings.EditorColumn("NumD")?.LabelText;
            NumE = issueModel.NumE;
            NumE_LabelText = siteSettings.EditorColumn("NumE")?.LabelText;
            NumF = issueModel.NumF;
            NumF_LabelText = siteSettings.EditorColumn("NumF")?.LabelText;
            NumG = issueModel.NumG;
            NumG_LabelText = siteSettings.EditorColumn("NumG")?.LabelText;
            NumH = issueModel.NumH;
            NumH_LabelText = siteSettings.EditorColumn("NumH")?.LabelText;
            DateA = issueModel.DateA;
            DateA_LabelText = siteSettings.EditorColumn("DateA")?.LabelText;
            DateB = issueModel.DateB;
            DateB_LabelText = siteSettings.EditorColumn("DateB")?.LabelText;
            DateC = issueModel.DateC;
            DateC_LabelText = siteSettings.EditorColumn("DateC")?.LabelText;
            DateD = issueModel.DateD;
            DateD_LabelText = siteSettings.EditorColumn("DateD")?.LabelText;
            DateE = issueModel.DateE;
            DateE_LabelText = siteSettings.EditorColumn("DateE")?.LabelText;
            DateF = issueModel.DateF;
            DateF_LabelText = siteSettings.EditorColumn("DateF")?.LabelText;
            DateG = issueModel.DateG;
            DateG_LabelText = siteSettings.EditorColumn("DateG")?.LabelText;
            DateH = issueModel.DateH;
            DateH_LabelText = siteSettings.EditorColumn("DateH")?.LabelText;
            Comments = issueModel.Comments;
            Comments_LabelText = siteSettings.EditorColumn("Comments")?.LabelText;
            Creator = issueModel.Creator;
            Creator_LabelText = siteSettings.EditorColumn("Creator")?.LabelText;
            Updator = issueModel.Updator;
            Updator_LabelText = siteSettings.EditorColumn("Updator")?.LabelText;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
            IssueId.SearchIndexes(ref searchIndexHash, 1);
            Title.SearchIndexes(ref searchIndexHash, 4);
            Body.SearchIndexes(ref searchIndexHash, 200);
            Manager.SearchIndexes(ref searchIndexHash, 100);
            Owner.SearchIndexes(ref searchIndexHash, 100);
            ClassA.SearchIndexes(ref searchIndexHash, 200);
            ClassB.SearchIndexes(ref searchIndexHash, 200);
            ClassC.SearchIndexes(ref searchIndexHash, 200);
            ClassD.SearchIndexes(ref searchIndexHash, 200);
            ClassE.SearchIndexes(ref searchIndexHash, 200);
            ClassF.SearchIndexes(ref searchIndexHash, 200);
            ClassG.SearchIndexes(ref searchIndexHash, 200);
            ClassH.SearchIndexes(ref searchIndexHash, 200);
            ClassI.SearchIndexes(ref searchIndexHash, 200);
            ClassJ.SearchIndexes(ref searchIndexHash, 200);
            ClassK.SearchIndexes(ref searchIndexHash, 200);
            ClassL.SearchIndexes(ref searchIndexHash, 200);
            ClassM.SearchIndexes(ref searchIndexHash, 200);
            ClassN.SearchIndexes(ref searchIndexHash, 200);
            ClassO.SearchIndexes(ref searchIndexHash, 200);
            ClassP.SearchIndexes(ref searchIndexHash, 200);
            Comments.SearchIndexes(ref searchIndexHash, 200);
            Creator.SearchIndexes(ref searchIndexHash, 100);
            Updator.SearchIndexes(ref searchIndexHash, 100);
            return searchIndexHash;
        }
    }

    public class IssueCollection : List<IssueModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public IssueCollection(
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
            IEnumerable<Aggregation> aggregationCollection = null)
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

        public IssueCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private IssueCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new IssueModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public IssueCollection(
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
                Rds.SelectIssues(
                    dataTableName: "Main",
                    column: column ?? Rds.IssuesColumnDefault(),
                    join: join ??  Rds.IssuesJoinDefault(),
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
                statements.AddRange(Rds.IssuesAggregations(aggregationCollection, where));
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
                statements: Rds.IssuesStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class IssuesUtility
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = Html.Builder();
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var issueCollection = IssueCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                modelName: "Issue",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: IndexScript(
                    issueCollection: issueCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                action: () => hb
                    .Form(
                        attributes: Html.Attributes()
                            .Id_Css("IssuesForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "Issues",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: issueCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    issueCollection: issueCollection,
                                    siteSettings: siteSettings,
                                    permissionType: permissionType,
                                    formData: formData,
                                    dataViewName: dataViewName))
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Issues")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .Dialog_Move("items", siteSettings.SiteId, bulk: true)
                    .Dialog_ImportSettings()
                    .Div(attributes: Html.Attributes()
                        .Id_Css("Dialog_ExportSettings", "dialog")
                        .Title(Displays.ExportSettings()))).ToString();
        }

        private static IssueCollection IssueCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new IssueCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Issues",
                    formData: formData,
                    where: Rds.IssuesWhere().SiteId(siteSettings.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.IssuesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string IndexScript(
            IssueCollection issueCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            if (Routes.Method() == "get")
            {
                switch (dataViewName)
                {
                    case "BurnDown": return Def.JavaScript.DrawBurnDown;
                    case "Gantt": return Def.JavaScript.DrawGantt;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            IssueCollection issueCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                case "BurnDown": return hb.BurnDown(
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    dataRows: BurnDownDataRows(
                        siteSettings: siteSettings,
                        formData: formData),
                    ownerLabelText: siteSettings.AllColumn("Owner").LabelText,
                    column: siteSettings.AllColumn("WorkValue"));
                case "Gantt": return hb.Gantt(
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    dataRows: GanttDataRows(
                        siteSettings: siteSettings,
                        formData: formData),
                    unit: siteSettings.AllColumn("WorkValue").Unit);
                default: return hb.Grid(
                    issueCollection: issueCollection,
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
                case "BurnDown": return BurnDownResponse(siteSettings, permissionType);
                case "Gantt": return GanttResponse(siteSettings, permissionType);
                default: return Grid(siteSettings: siteSettings, permissionType: permissionType);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            IssueCollection issueCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: Html.Attributes()
                        .Id_Css("Grid", "grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            issueCollection: issueCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize.ToString())
                .MainCommands(
                    siteId: siteSettings.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                    bulkMoveButton: true,
                    bulkDeleteButton: true,
                    importButton: true,
                    exportButton: true);
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var issueCollection = IssueCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", issueCollection.Count > 0
                    ? Html.Builder().Grid(
                        siteSettings: siteSettings,
                        issueCollection: issueCollection,
                        permissionType: permissionType,
                        formData: formData)
                    : Html.Builder())
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: issueCollection.Aggregations,
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
            var issueCollection = IssueCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", Html.Builder().GridRows(
                    siteSettings: siteSettings,
                    issueCollection: issueCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: issueCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, issueCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            IssueCollection issueCollection,
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
            issueCollection.ForEach(issueModel => hb
                .Tr(
                    attributes: Html.Attributes()
                        .Class("grid-row")
                        .DataId(issueModel.IssueId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: issueModel.IssueId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    issueModel: issueModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.IssuesColumn()
                .SiteId()
                .IssueId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "UpdatedTime": select.UpdatedTime(); break;
                    case "IssueId": select.IssueId(); break;
                    case "Ver": select.Ver(); break;
                    case "Title": select.Title(); break;
                    case "Body": select.Body(); break;
                    case "TitleBody": select.TitleBody(); break;
                    case "StartTime": select.StartTime(); break;
                    case "CompletionTime": select.CompletionTime(); break;
                    case "WorkValue": select.WorkValue(); break;
                    case "ProgressRate": select.ProgressRate(); break;
                    case "RemainingWorkValue": select.RemainingWorkValue(); break;
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
                    case "NumA": select.NumA(); break;
                    case "NumB": select.NumB(); break;
                    case "NumC": select.NumC(); break;
                    case "NumD": select.NumD(); break;
                    case "NumE": select.NumE(); break;
                    case "NumF": select.NumF(); break;
                    case "NumG": select.NumG(); break;
                    case "NumH": select.NumH(); break;
                    case "DateA": select.DateA(); break;
                    case "DateB": select.DateB(); break;
                    case "DateC": select.DateC(); break;
                    case "DateD": select.DateD(); break;
                    case "DateE": select.DateE(); break;
                    case "DateF": select.DateF(); break;
                    case "DateG": select.DateG(); break;
                    case "DateH": select.DateH(); break;
                    case "Comments": select.Comments(); break;
                    case "Creator": select.Creator(); break;
                    case "Updator": select.Updator(); break;
                    case "CreatedTime": select.CreatedTime(); break;
                }
            });
            return select;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, IssueModel issueModel)
        {
            switch (column.ColumnName)
            {
                case "SiteId": return hb.Td(column: column, value: issueModel.SiteId);
                case "UpdatedTime": return hb.Td(column: column, value: issueModel.UpdatedTime);
                case "IssueId": return hb.Td(column: column, value: issueModel.IssueId);
                case "Ver": return hb.Td(column: column, value: issueModel.Ver);
                case "Title": return hb.Td(column: column, value: issueModel.Title);
                case "Body": return hb.Td(column: column, value: issueModel.Body);
                case "TitleBody": return hb.Td(column: column, value: issueModel.TitleBody);
                case "StartTime": return hb.Td(column: column, value: issueModel.StartTime);
                case "CompletionTime": return hb.Td(column: column, value: issueModel.CompletionTime);
                case "WorkValue": return hb.Td(column: column, value: issueModel.WorkValue);
                case "ProgressRate": return hb.Td(column: column, value: issueModel.ProgressRate);
                case "RemainingWorkValue": return hb.Td(column: column, value: issueModel.RemainingWorkValue);
                case "Status": return hb.Td(column: column, value: issueModel.Status);
                case "Manager": return hb.Td(column: column, value: issueModel.Manager);
                case "Owner": return hb.Td(column: column, value: issueModel.Owner);
                case "ClassA": return hb.Td(column: column, value: issueModel.ClassA);
                case "ClassB": return hb.Td(column: column, value: issueModel.ClassB);
                case "ClassC": return hb.Td(column: column, value: issueModel.ClassC);
                case "ClassD": return hb.Td(column: column, value: issueModel.ClassD);
                case "ClassE": return hb.Td(column: column, value: issueModel.ClassE);
                case "ClassF": return hb.Td(column: column, value: issueModel.ClassF);
                case "ClassG": return hb.Td(column: column, value: issueModel.ClassG);
                case "ClassH": return hb.Td(column: column, value: issueModel.ClassH);
                case "ClassI": return hb.Td(column: column, value: issueModel.ClassI);
                case "ClassJ": return hb.Td(column: column, value: issueModel.ClassJ);
                case "ClassK": return hb.Td(column: column, value: issueModel.ClassK);
                case "ClassL": return hb.Td(column: column, value: issueModel.ClassL);
                case "ClassM": return hb.Td(column: column, value: issueModel.ClassM);
                case "ClassN": return hb.Td(column: column, value: issueModel.ClassN);
                case "ClassO": return hb.Td(column: column, value: issueModel.ClassO);
                case "ClassP": return hb.Td(column: column, value: issueModel.ClassP);
                case "NumA": return hb.Td(column: column, value: issueModel.NumA);
                case "NumB": return hb.Td(column: column, value: issueModel.NumB);
                case "NumC": return hb.Td(column: column, value: issueModel.NumC);
                case "NumD": return hb.Td(column: column, value: issueModel.NumD);
                case "NumE": return hb.Td(column: column, value: issueModel.NumE);
                case "NumF": return hb.Td(column: column, value: issueModel.NumF);
                case "NumG": return hb.Td(column: column, value: issueModel.NumG);
                case "NumH": return hb.Td(column: column, value: issueModel.NumH);
                case "DateA": return hb.Td(column: column, value: issueModel.DateA);
                case "DateB": return hb.Td(column: column, value: issueModel.DateB);
                case "DateC": return hb.Td(column: column, value: issueModel.DateC);
                case "DateD": return hb.Td(column: column, value: issueModel.DateD);
                case "DateE": return hb.Td(column: column, value: issueModel.DateE);
                case "DateF": return hb.Td(column: column, value: issueModel.DateF);
                case "DateG": return hb.Td(column: column, value: issueModel.DateG);
                case "DateH": return hb.Td(column: column, value: issueModel.DateH);
                case "Comments": return hb.Td(column: column, value: issueModel.Comments);
                case "Creator": return hb.Td(column: column, value: issueModel.Creator);
                case "Updator": return hb.Td(column: column, value: issueModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: issueModel.CreatedTime);
                default: return hb;
            }
        }

        public static string EditorNew(SiteModel siteModel, long siteId)
        {
            return Editor(
                siteModel,
                new IssueModel(
                    siteModel.IssuesSiteSettings(),
                    siteModel.PermissionType,
                    methodType: BaseModel.MethodTypes.New)
                {
                    SiteId = siteId
                });
        }

        public static string Editor(SiteModel siteModel, long issueId, bool clearSessions)
        {
            var siteSettings = siteModel.IssuesSiteSettings();
            var issueModel = new IssueModel(
                siteSettings: siteSettings,
                permissionType: siteModel.PermissionType,
                issueId: issueId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            issueModel.SwitchTargets = IssuesUtility.GetSwitchTargets(
                siteSettings, issueModel.SiteId);
            return Editor(siteModel, issueModel);
        }

        public static string Editor(SiteModel siteModel, IssueModel issueModel)
        {
            var hb = Html.Builder();
            issueModel.SiteSettings.SetLinks();
            return hb.Template(
                siteId: siteModel.SiteId,
                modelName: "Issue",
                title: issueModel.MethodType != BaseModel.MethodTypes.New
                    ? issueModel.Title.DisplayValue + " - " + Displays.Edit()
                    : siteModel.Title.DisplayValue + " - " + Displays.New(),
                permissionType: issueModel.PermissionType,
                verType: issueModel.VerType,
                backUrl: Navigations.ItemIndex(issueModel.SiteId),
                methodType: issueModel.MethodType,
                allowAccess:
                    issueModel.PermissionType.CanRead() &&
                    issueModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    hb
                        .Editor(
                            siteSettings: issueModel.SiteSettings,
                            issueModel: issueModel,
                            siteModel: siteModel)
                        .Hidden(controlId: "TableName", value: "Issues")
                        .Hidden(controlId: "Id", value: issueModel.IssueId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteModel siteModel,
            SiteSettings siteSettings,
            IssueModel issueModel)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: Html.Attributes()
                        .Id_Css("IssueForm", "main-form")
                        .Action(Navigations.ItemAction(issueModel.IssueId != 0 
                            ? issueModel.IssueId
                            : siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            id: issueModel.IssueId,
                            baseModel: issueModel,
                            tableName: "Issues",
                            switchTargets: issueModel.SwitchTargets)
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: issueModel.Comments,
                                verType: issueModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(issueModel: issueModel)
                            .Fields(
                                siteSettings: siteSettings,
                                permissionType: siteModel.PermissionType,
                                issueModel: issueModel)
                            .Div(id: "LinkCreations", css: "links", action: () => hb
                                .LinkCreations(
                                    siteSettings: siteSettings,
                                    linkId: issueModel.IssueId,
                                    methodType: issueModel.MethodType))
                            .Div(id: "Links", css: "links", action: () => hb
                                .Links(linkId: issueModel.IssueId))
                            .MainCommands(
                                siteId: siteModel.SiteId,
                                permissionType: siteModel.PermissionType,
                                verType: issueModel.VerType,
                                backUrl: Navigations.ItemIndex(siteSettings.SiteId),
                                referenceType: "items",
                                referenceId: issueModel.IssueId,
                                updateButton: true,
                                copyButton: true,
                                moveButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        issueModel: issueModel,
                                        siteSettings: siteSettings)))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "Issues_Timestamp",
                            css: "must-transport",
                            value: issueModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: issueModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Issues", issueModel.IssueId, issueModel.Ver)
                .Dialog_Copy("items", issueModel.IssueId)
                .Dialog_Move("items", issueModel.IssueId)
                .Dialog_Histories(Navigations.ItemAction(issueModel.IssueId))
                .Dialog_OutgoingMail()
                .EditorExtensions(issueModel: issueModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, IssueModel issueModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic())));
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            IssueModel issueModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.ColumnCollection
                    .Where(o => o.EditorVisible.ToBool())
                    .OrderBy(o => siteSettings.EditorOrder.IndexOf(o.ColumnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "IssueId": hb.Field(siteSettings, column, issueModel.IssueId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Ver": hb.Field(siteSettings, column, issueModel.Ver.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Title": hb.Field(siteSettings, column, issueModel.Title.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Body": hb.Field(siteSettings, column, issueModel.Body.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "StartTime": hb.Field(siteSettings, column, issueModel.StartTime.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "CompletionTime": hb.Field(siteSettings, column, issueModel.CompletionTime.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "WorkValue": hb.Field(siteSettings, column, issueModel.WorkValue.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ProgressRate": hb.Field(siteSettings, column, issueModel.ProgressRate.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "RemainingWorkValue": hb.Field(siteSettings, column, issueModel.RemainingWorkValue.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Status": hb.Field(siteSettings, column, issueModel.Status.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Manager": hb.Field(siteSettings, column, issueModel.Manager.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Owner": hb.Field(siteSettings, column, issueModel.Owner.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassA": hb.Field(siteSettings, column, issueModel.ClassA.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassB": hb.Field(siteSettings, column, issueModel.ClassB.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassC": hb.Field(siteSettings, column, issueModel.ClassC.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassD": hb.Field(siteSettings, column, issueModel.ClassD.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassE": hb.Field(siteSettings, column, issueModel.ClassE.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassF": hb.Field(siteSettings, column, issueModel.ClassF.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassG": hb.Field(siteSettings, column, issueModel.ClassG.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassH": hb.Field(siteSettings, column, issueModel.ClassH.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassI": hb.Field(siteSettings, column, issueModel.ClassI.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassJ": hb.Field(siteSettings, column, issueModel.ClassJ.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassK": hb.Field(siteSettings, column, issueModel.ClassK.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassL": hb.Field(siteSettings, column, issueModel.ClassL.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassM": hb.Field(siteSettings, column, issueModel.ClassM.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassN": hb.Field(siteSettings, column, issueModel.ClassN.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassO": hb.Field(siteSettings, column, issueModel.ClassO.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ClassP": hb.Field(siteSettings, column, issueModel.ClassP.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "NumA": hb.Field(siteSettings, column, issueModel.NumA.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "NumB": hb.Field(siteSettings, column, issueModel.NumB.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "NumC": hb.Field(siteSettings, column, issueModel.NumC.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "NumD": hb.Field(siteSettings, column, issueModel.NumD.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "NumE": hb.Field(siteSettings, column, issueModel.NumE.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "NumF": hb.Field(siteSettings, column, issueModel.NumF.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "NumG": hb.Field(siteSettings, column, issueModel.NumG.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "NumH": hb.Field(siteSettings, column, issueModel.NumH.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DateA": hb.Field(siteSettings, column, issueModel.DateA.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DateB": hb.Field(siteSettings, column, issueModel.DateB.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DateC": hb.Field(siteSettings, column, issueModel.DateC.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DateD": hb.Field(siteSettings, column, issueModel.DateD.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DateE": hb.Field(siteSettings, column, issueModel.DateE.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DateF": hb.Field(siteSettings, column, issueModel.DateF.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DateG": hb.Field(siteSettings, column, issueModel.DateG.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DateH": hb.Field(siteSettings, column, issueModel.DateH.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                        }
                    });
                hb.VerUpCheckBox(issueModel);
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            IssueModel issueModel,
            SiteSettings siteSettings)
        {
            return issueModel.MethodType == BaseModel.MethodTypes.Edit
                ? hb.Button(
                    text: Displays.Separate(),
                    controlCss: "button-separate",
                    onClick: Def.JavaScript.EditSeparateSettings,
                    action: "EditSeparateSettings",
                    method: "post")
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            IssueModel issueModel,
            SiteSettings siteSettings)
        {
            return issueModel.MethodType == BaseModel.MethodTypes.Edit
                ? hb.Dialog_SeparateSettings()
                : hb;
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
                    statements: Rds.SelectIssues(
                        column: Rds.IssuesColumn().IssueId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Issues",
                            formData: formData,
                            where: Rds.IssuesWhere().SiteId(siteId)),
                        orderBy: GridSorters.Get(
                            formData, Rds.IssuesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["IssueId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
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
                    Rds.UpdateIssues(
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Issues",
                            formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                            where: Rds.IssuesWhere()
                                .SiteId(siteSettings.SiteId)
                                .IssueId_In(
                                    value: checkedItems,
                                    negative: negative,
                                    _using: checkedItems.Count() > 0)),
                        param: Rds.IssuesParam().SiteId(siteId),
                        countRecord: true),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectIssues(
                                    column: Rds.IssuesColumn().IssueId(),
                                    where: Rds.IssuesWhere().SiteId(siteId)))
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
                tableName: "Issues",
                formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                where: Rds.IssuesWhere()
                    .SiteId(siteSettings.SiteId)
                    .IssueId_In(
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
                                sub: Rds.SelectIssues(
                                    column: Rds.IssuesColumn().IssueId(),
                                    where: where))),
                    Rds.DeleteIssues(
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
                var siteSettings = siteModel.IssuesSiteSettings();
                var columnHash = new Dictionary<int, Column>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = siteSettings.ColumnCollection
                        .Where(o => o.LabelText == data.Header)
                        .FirstOrDefault();
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var error = Imports.ColumnValidate(siteSettings, columnHash.Values
                    .Select(o => o.ColumnName), "Title", "CompletionTime");
                if (error != null) return error;
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.IssuesParam();
                    param.IssueId(raw: Def.Sql.Identity);
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
                                case "StartTime": param.StartTime(recordingData, _using: recordingData != null); break;
                                case "CompletionTime": param.CompletionTime(recordingData, _using: recordingData != null); break;
                                case "WorkValue": param.WorkValue(recordingData, _using: recordingData != null); break;
                                case "ProgressRate": param.ProgressRate(recordingData, _using: recordingData != null); break;
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
                                case "NumA": param.NumA(recordingData, _using: recordingData != null); break;
                                case "NumB": param.NumB(recordingData, _using: recordingData != null); break;
                                case "NumC": param.NumC(recordingData, _using: recordingData != null); break;
                                case "NumD": param.NumD(recordingData, _using: recordingData != null); break;
                                case "NumE": param.NumE(recordingData, _using: recordingData != null); break;
                                case "NumF": param.NumF(recordingData, _using: recordingData != null); break;
                                case "NumG": param.NumG(recordingData, _using: recordingData != null); break;
                                case "NumH": param.NumH(recordingData, _using: recordingData != null); break;
                                case "DateA": param.DateA(recordingData, _using: recordingData != null); break;
                                case "DateB": param.DateB(recordingData, _using: recordingData != null); break;
                                case "DateC": param.DateC(recordingData, _using: recordingData != null); break;
                                case "DateD": param.DateD(recordingData, _using: recordingData != null); break;
                                case "DateE": param.DateE(recordingData, _using: recordingData != null); break;
                                case "DateF": param.DateF(recordingData, _using: recordingData != null); break;
                                case "DateG": param.DateG(recordingData, _using: recordingData != null); break;
                                case "DateH": param.DateH(recordingData, _using: recordingData != null); break;
                            }
                        }
                    });
                    paramHash.Add(data.Index, param);
                });
                var errorTitle = Imports.Validate(
                    paramHash, siteSettings.AllColumn("Title"));
                if (errorTitle != null) return errorTitle;
                var errorCompletionTime = Imports.Validate(
                    paramHash, siteSettings.AllColumn("CompletionTime"));
                if (errorCompletionTime != null) return errorCompletionTime;
                paramHash.Values.ForEach(param =>
                    new IssueModel(siteSettings, siteModel.PermissionType)
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private static object ImportRecordingData(
            Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(value, inheritPermission);
            switch (column.ColumnName)
            {
                case "CompletionTime":
                    recordingData = recordingData.ToDateTime().AddDays(1);
                    break;
            }
            return recordingData;
        }

        public static ResponseFile Export(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            SiteModel siteModel)
        {
            siteModel.SiteSettings.SetLinks();
            var formData = DataViewFilters.SessionFormData(siteModel.SiteId);
            var issueCollection = new IssueCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                where: DataViewFilters.Get(
                    siteSettings: siteModel.SiteSettings,
                    tableName: "Issues",
                    formData: formData,
                    where: Rds.IssuesWhere().SiteId(siteModel.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.IssuesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)));
            var csv = new StringBuilder();
            var exportColumns = (Sessions.PageSession(
                siteModel.Id, 
                "ExportSettings_ExportColumns").ToString().Deserialize<ExportColumns>());
            var columnHash = exportColumns.ColumnHash(siteModel.IssuesSiteSettings());
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
            issueCollection.ForEach(issueModel =>
            {
                var row = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            issueModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key])));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(
            IssueModel issueModel, string columnName, Column column)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId": value = issueModel.SiteId.ToExport(column); break;
                case "UpdatedTime": value = issueModel.UpdatedTime.ToExport(column); break;
                case "IssueId": value = issueModel.IssueId.ToExport(column); break;
                case "Ver": value = issueModel.Ver.ToExport(column); break;
                case "Title": value = issueModel.Title.ToExport(column); break;
                case "Body": value = issueModel.Body.ToExport(column); break;
                case "TitleBody": value = issueModel.TitleBody.ToExport(column); break;
                case "StartTime": value = issueModel.StartTime.ToExport(column); break;
                case "CompletionTime": value = issueModel.CompletionTime.ToExport(column); break;
                case "WorkValue": value = issueModel.WorkValue.ToExport(column); break;
                case "ProgressRate": value = issueModel.ProgressRate.ToExport(column); break;
                case "RemainingWorkValue": value = issueModel.RemainingWorkValue.ToExport(column); break;
                case "Status": value = issueModel.Status.ToExport(column); break;
                case "Manager": value = issueModel.Manager.ToExport(column); break;
                case "Owner": value = issueModel.Owner.ToExport(column); break;
                case "ClassA": value = issueModel.ClassA.ToExport(column); break;
                case "ClassB": value = issueModel.ClassB.ToExport(column); break;
                case "ClassC": value = issueModel.ClassC.ToExport(column); break;
                case "ClassD": value = issueModel.ClassD.ToExport(column); break;
                case "ClassE": value = issueModel.ClassE.ToExport(column); break;
                case "ClassF": value = issueModel.ClassF.ToExport(column); break;
                case "ClassG": value = issueModel.ClassG.ToExport(column); break;
                case "ClassH": value = issueModel.ClassH.ToExport(column); break;
                case "ClassI": value = issueModel.ClassI.ToExport(column); break;
                case "ClassJ": value = issueModel.ClassJ.ToExport(column); break;
                case "ClassK": value = issueModel.ClassK.ToExport(column); break;
                case "ClassL": value = issueModel.ClassL.ToExport(column); break;
                case "ClassM": value = issueModel.ClassM.ToExport(column); break;
                case "ClassN": value = issueModel.ClassN.ToExport(column); break;
                case "ClassO": value = issueModel.ClassO.ToExport(column); break;
                case "ClassP": value = issueModel.ClassP.ToExport(column); break;
                case "NumA": value = issueModel.NumA.ToExport(column); break;
                case "NumB": value = issueModel.NumB.ToExport(column); break;
                case "NumC": value = issueModel.NumC.ToExport(column); break;
                case "NumD": value = issueModel.NumD.ToExport(column); break;
                case "NumE": value = issueModel.NumE.ToExport(column); break;
                case "NumF": value = issueModel.NumF.ToExport(column); break;
                case "NumG": value = issueModel.NumG.ToExport(column); break;
                case "NumH": value = issueModel.NumH.ToExport(column); break;
                case "DateA": value = issueModel.DateA.ToExport(column); break;
                case "DateB": value = issueModel.DateB.ToExport(column); break;
                case "DateC": value = issueModel.DateC.ToExport(column); break;
                case "DateD": value = issueModel.DateD.ToExport(column); break;
                case "DateE": value = issueModel.DateE.ToExport(column); break;
                case "DateF": value = issueModel.DateF.ToExport(column); break;
                case "DateG": value = issueModel.DateG.ToExport(column); break;
                case "DateH": value = issueModel.DateH.ToExport(column); break;
                case "Comments": value = issueModel.Comments.ToExport(column); break;
                case "Creator": value = issueModel.Creator.ToExport(column); break;
                case "Updator": value = issueModel.Updator.ToExport(column); break;
                case "CreatedTime": value = issueModel.CreatedTime.ToExport(column); break;
                default: return string.Empty;
            }
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, IssueModel issueModel)
        {
            var displayValue = siteSettings.ColumnCollection
                .Where(o => o.TitleVisible.ToBool())
                .OrderBy(o => siteSettings.TitleOrder.IndexOf(o.ColumnName))
                .Select(column => TitleDisplayValue(column, issueModel))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, IssueModel issueModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(issueModel.Title.Value).Text()
                    : issueModel.Title.Value;
                case "ClassA": return column.HasChoices()
                    ? column.Choice(issueModel.ClassA).Text()
                    : issueModel.ClassA;
                case "ClassB": return column.HasChoices()
                    ? column.Choice(issueModel.ClassB).Text()
                    : issueModel.ClassB;
                case "ClassC": return column.HasChoices()
                    ? column.Choice(issueModel.ClassC).Text()
                    : issueModel.ClassC;
                case "ClassD": return column.HasChoices()
                    ? column.Choice(issueModel.ClassD).Text()
                    : issueModel.ClassD;
                case "ClassE": return column.HasChoices()
                    ? column.Choice(issueModel.ClassE).Text()
                    : issueModel.ClassE;
                case "ClassF": return column.HasChoices()
                    ? column.Choice(issueModel.ClassF).Text()
                    : issueModel.ClassF;
                case "ClassG": return column.HasChoices()
                    ? column.Choice(issueModel.ClassG).Text()
                    : issueModel.ClassG;
                case "ClassH": return column.HasChoices()
                    ? column.Choice(issueModel.ClassH).Text()
                    : issueModel.ClassH;
                case "ClassI": return column.HasChoices()
                    ? column.Choice(issueModel.ClassI).Text()
                    : issueModel.ClassI;
                case "ClassJ": return column.HasChoices()
                    ? column.Choice(issueModel.ClassJ).Text()
                    : issueModel.ClassJ;
                case "ClassK": return column.HasChoices()
                    ? column.Choice(issueModel.ClassK).Text()
                    : issueModel.ClassK;
                case "ClassL": return column.HasChoices()
                    ? column.Choice(issueModel.ClassL).Text()
                    : issueModel.ClassL;
                case "ClassM": return column.HasChoices()
                    ? column.Choice(issueModel.ClassM).Text()
                    : issueModel.ClassM;
                case "ClassN": return column.HasChoices()
                    ? column.Choice(issueModel.ClassN).Text()
                    : issueModel.ClassN;
                case "ClassO": return column.HasChoices()
                    ? column.Choice(issueModel.ClassO).Text()
                    : issueModel.ClassO;
                case "ClassP": return column.HasChoices()
                    ? column.Choice(issueModel.ClassP).Text()
                    : issueModel.ClassP;
                default: return string.Empty;
            }
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, DataRow dataRow)
        {
            var displayValue = siteSettings.ColumnCollection
                .Where(o => o.TitleVisible.ToBool())
                .OrderBy(o => siteSettings.TitleOrder.IndexOf(o.ColumnName))
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
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GanttResponse(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var issueCollection = IssueCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html(
                    "#DataViewContainer",
                    Html.Builder().Gantt(
                        siteSettings: siteSettings,
                        permissionType: permissionType,
                        dataRows: GanttDataRows(siteSettings, formData),
                        unit: siteSettings.AllColumn("WorkValue").Unit))
                .Html(
                    "#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: issueCollection.Aggregations,
                    container: false))
                .Func("drawGantt")
                .WindowScrollTop().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> GanttDataRows(
            SiteSettings siteSettings, FormData formData)
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(siteSettings)
                        .IssueId(_as: "Id")
                        .Title()
                        .WorkValue()
                        .StartTime()
                        .CompletionTime()
                        .ProgressRate()
                        .Status()
                        .Owner()
                        .Updator()
                        .CreatedTime()
                        .UpdatedTime(),
                    where: DataViewFilters.Get(
                        siteSettings,
                        "Issues",
                        formData,
                        Rds.IssuesWhere().SiteId(siteSettings.SiteId))))
                            .AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BurnDownResponse(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var issueCollection = IssueCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html(
                    "#DataViewContainer",
                    Html.Builder().BurnDown(
                        siteSettings: siteSettings,
                        permissionType: permissionType,
                        dataRows: BurnDownDataRows(siteSettings, formData),
                        ownerLabelText: siteSettings.AllColumn("Owner").LabelText,
                        column: siteSettings.AllColumn("WorkValue")))
                .Html(
                    "#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: issueCollection.Aggregations,
                    container: false))
                .Func("drawBurnDown")
                .WindowScrollTop().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BurnDownRecordDetails(SiteSettings siteSettings)
        {
            var date = Forms.DateTime("BurnDownDate");
            return new ResponseCollection()
                .After(string.Empty, Html.Builder().BurnDownRecordDetails(
                    elements: new BurnDown(siteSettings, BurnDownDataRows(
                        siteSettings: siteSettings,
                        formData: DataViewFilters.SessionFormData(siteSettings.SiteId)))
                            .Where(o => o.UpdatedTime == date),
                    column: siteSettings.AllColumn("Status"),
                    colspan: Forms.Int("BurnDownColspan"),
                    unit: siteSettings.AllColumn("WorkValue").Unit)).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> BurnDownDataRows(
            SiteSettings siteSettings, FormData formData)
        {
            var where = DataViewFilters.Get(
                siteSettings,
                "Issues",
                formData,
                Rds.IssuesWhere().SiteId(siteSettings.SiteId));
            return Rds.ExecuteTable(
                statements: new SqlStatement[]
                {
                    Rds.SelectIssues(
                        column: Rds.IssuesTitleColumn(siteSettings)
                            .IssueId(_as: "Id")
                            .Ver()
                            .Title()
                            .WorkValue()
                            .StartTime()
                            .CompletionTime()
                            .ProgressRate()
                            .Status()
                            .Updator()
                            .CreatedTime()
                            .UpdatedTime(),
                        where: where,
                        unionType: Sqls.UnionTypes.Union),
                    Rds.SelectIssues(
                        tableType: Sqls.TableTypes.History,
                        column: Rds.IssuesTitleColumn(siteSettings)
                            .IssueId(_as: "Id")
                            .Ver()
                            .Title()
                            .WorkValue()
                            .StartTime()
                            .CompletionTime()
                            .ProgressRate()
                            .Status()
                            .Updator()
                            .CreatedTime()
                            .UpdatedTime(),
                        where: Rds.IssuesWhere()
                            .IssueId_In(sub: Rds.SelectIssues(
                                column: Rds.IssuesColumn().IssueId(),
                                where: where)),
                        orderBy: Rds.IssuesOrderBy()
                            .IssueId()
                            .Ver())
                }).AsEnumerable();
        }
    }
}
