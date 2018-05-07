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
using Implem.Pleasanter.Libraries.Resources;
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
    public static class DemoUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Register()
        {
            var passphrase = Strings.NewGuid();
            var mailAddress = Forms.Data("Users_DemoMailAddress");
            var tenantModel = new TenantModel()
            {
                TenantName = mailAddress
            };
            tenantModel.Create();
            var demoModel = new DemoModel()
            {
                TenantId = tenantModel.TenantId,
                Passphrase = passphrase,
                MailAddress = mailAddress
            };
            demoModel.Create();
            demoModel.Initialize(new OutgoingMailModel()
            {
                Title = new Title(Displays.DemoMailTitle()),
                Body = Displays.DemoMailBody(
                    Locations.DemoUri(passphrase),
                    Parameters.Service.DemoUsagePeriod.ToString()),
                From = new System.Net.Mail.MailAddress(Parameters.Mail.SupportFrom),
                To = mailAddress,
                Bcc = Parameters.Mail.SupportFrom
            });
            return Messages.ResponseSentAcceptanceMail()
                .Remove("#DemoForm")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool Login()
        {
            var demoModel = new DemoModel().Get(
                where: Rds.DemosWhere()
                    .Passphrase(QueryStrings.Data("passphrase"))
                    .CreatedTime(
                        DateTime.Now.AddDays(Parameters.Service.DemoUsagePeriod * -1),
                        _operator: ">="));
            if (demoModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                var loginId = LoginId(demoModel, "User1");
                var password = Strings.NewGuid().Sha512Cng();
                if (!demoModel.Initialized)
                {
                    var idHash = new Dictionary<string, long>();
                    demoModel.Initialize(idHash, password);
                }
                else
                {
                    Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                        param: Rds.UsersParam().Password(password),
                        where: Rds.UsersWhere().LoginId(loginId)));
                }
                new UserModel()
                {
                    LoginId = loginId,
                    Password = password
                }.Authenticate(string.Empty);
                SiteInfo.Reflesh(force: true);
                return Sessions.LoggedIn();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Initialize(
            this DemoModel demoModel, OutgoingMailModel outgoingMailModel)
        {
            var idHash = new Dictionary<string, long>();
            var loginId = LoginId(demoModel, "User1");
            var password = Strings.NewGuid().Sha512Cng();
            System.Threading.Tasks.Task.Run(() =>
            {
                demoModel.Initialize(idHash, password);
                outgoingMailModel.Send();
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Initialize(
            this DemoModel demoModel, Dictionary<string, long> idHash, string password)
        {
            demoModel.InitializeTimeLag();
            InitializeDepts(demoModel, idHash);
            InitializeUsers(demoModel, idHash, password);
            SiteInfo.Reflesh(force: true);
            InitializeSites(demoModel, idHash);
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .OrderBy(o => o.Id)
                .ForEach(o =>
                {
                    InitializeIssues(demoModel, o.Id, idHash);
                    InitializeResults(demoModel, o.Id, idHash);
                });
            InitializeLinks(demoModel, idHash);
            InitializePermissions(idHash);
            Rds.ExecuteNonQuery(statements: Rds.UpdateDemos(
                param: Rds.DemosParam().Initialized(true),
                where: Rds.DemosWhere().Passphrase(demoModel.Passphrase)));
            Libraries.Migrators.SiteSettingsMigrator.Migrate();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeDepts(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Depts")
                .ForEach(demoDefinition =>
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements:
                        Rds.InsertDepts(
                            selectIdentity: true,
                            param: Rds.DeptsParam()
                                .TenantId(demoModel.TenantId)
                                .DeptCode(demoDefinition.ClassA)
                                .DeptName(demoDefinition.Title)
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeUsers(
            DemoModel demoModel, Dictionary<string, long> idHash, string password)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Users")
                .ForEach(demoDefinition =>
                {
                    var loginId = LoginId(demoModel, demoDefinition.Id);
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertUsers(
                            selectIdentity: true,
                            param: Rds.UsersParam()
                                .TenantId(demoModel.TenantId)
                                .LoginId(loginId)
                                .Password(password)
                                .Name(demoDefinition.Title)
                                .DeptId(idHash.Get(demoDefinition.ParentId).ToInt())
                                .Birthday(demoDefinition.ClassC.ToDateTime())
                                .Gender(demoDefinition.ClassB)
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel))),
                        Rds.InsertMailAddresses(
                            param: Rds.MailAddressesParam()
                                .OwnerId(raw: Def.Sql.Identity)
                                .OwnerType("Users")
                                .MailAddress(loginId + "@example.com"))
                    }));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeSites(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites" && o.ParentId == string.Empty)
                .ForEach(o => InitializeSites(demoModel, idHash, o.Id));
            new SiteCollection(where: Rds.SitesWhere().TenantId(demoModel.TenantId))
                .ForEach(siteModel =>
                {
                    var fullText = siteModel.FullText(siteModel.SiteSettings);
                    Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                        param: Rds.ItemsParam()
                            .SiteId(siteModel.SiteId)
                            .Title(siteModel.Title.DisplayValue)
                            .FullText(fullText, _using: fullText != null),
                        where: Rds.ItemsWhere().ReferenceId(siteModel.SiteId),
                        addUpdatorParam: false,
                        addUpdatedTimeParam: false));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeSites(
            DemoModel demoModel, Dictionary<string, long> idHash, string topId)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .Where(o => o.Id == topId || o.ParentId == topId)
                .ForEach(demoDefinition =>
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements:
                        new SqlStatement[]
                        {
                            Rds.InsertItems(
                                selectIdentity: true,
                                param: Rds.ItemsParam()
                                    .ReferenceType("Sites")
                                    .Creator(idHash.Get(demoDefinition.Creator))
                                    .Updator(idHash.Get(demoDefinition.Updator))
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                                addUpdatorParam: false),
                            Rds.InsertSites(
                                selectIdentity: true,
                                param: Rds.SitesParam()
                                    .TenantId(demoModel.TenantId)
                                    .SiteId(raw: Def.Sql.Identity)
                                    .Title(demoDefinition.Title)
                                    .ReferenceType(demoDefinition.ClassA)
                                    .ParentId(idHash.ContainsKey(demoDefinition.ParentId)
                                        ? idHash.Get(demoDefinition.ParentId)
                                        : 0)
                                    .InheritPermission(idHash, topId, demoDefinition.ParentId)
                                    .SiteSettings(demoDefinition.Body.Replace(idHash))
                                    .Creator(idHash.Get(demoDefinition.Creator))
                                    .Updator(idHash.Get(demoDefinition.Updator))
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                                addUpdatorParam: false)
                        })));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Rds.SitesParamCollection InheritPermission(
             this Rds.SitesParamCollection self,
             Dictionary<string, long> idHash,
             string topId,
             string parentId)
        {
            return parentId == string.Empty
                ? self.InheritPermission(raw: Def.Sql.Identity)
                : self.InheritPermission(idHash.Get(topId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeIssues(
            DemoModel demoModel, string parentId, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.ParentId == parentId)
                .Where(o => o.Type == "Issues")
                .ForEach(demoDefinition =>
                {
                    var issueId = Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam()
                                .ReferenceType("Issues")
                                .Creator(idHash.Get(demoDefinition.Creator))
                                .Updator(idHash.Get(demoDefinition.Updator))
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.CreatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false),
                        Rds.InsertIssues(
                            selectIdentity: true,
                            param: Rds.IssuesParam()
                                .SiteId(idHash.Get(demoDefinition.ParentId))
                                .IssueId(raw: Def.Sql.Identity)
                                .Title(demoDefinition.Title)
                                .Body(demoDefinition.Body.Replace(idHash))
                                .StartTime(demoDefinition.StartTime.DemoTime(demoModel))
                                .CompletionTime(demoDefinition.CompletionTime
                                    .AddDays(1).DemoTime(demoModel))
                                .WorkValue(demoDefinition.WorkValue)
                                .ProgressRate(0)
                                .Status(demoDefinition.Status)
                                .Manager(idHash.Get(demoDefinition.Manager))
                                .Owner(idHash.Get(demoDefinition.Owner))
                                .ClassA(demoDefinition.ClassA.Replace(idHash))
                                .ClassB(demoDefinition.ClassB.Replace(idHash))
                                .ClassC(demoDefinition.ClassC.Replace(idHash))
                                .ClassD(demoDefinition.ClassD.Replace(idHash))
                                .ClassE(demoDefinition.ClassE.Replace(idHash))
                                .ClassF(demoDefinition.ClassF.Replace(idHash))
                                .ClassG(demoDefinition.ClassG.Replace(idHash))
                                .ClassH(demoDefinition.ClassH.Replace(idHash))
                                .ClassI(demoDefinition.ClassI.Replace(idHash))
                                .ClassJ(demoDefinition.ClassJ.Replace(idHash))
                                .ClassK(demoDefinition.ClassK.Replace(idHash))
                                .ClassL(demoDefinition.ClassL.Replace(idHash))
                                .ClassM(demoDefinition.ClassM.Replace(idHash))
                                .ClassN(demoDefinition.ClassN.Replace(idHash))
                                .ClassO(demoDefinition.ClassO.Replace(idHash))
                                .ClassP(demoDefinition.ClassP.Replace(idHash))
                                .ClassQ(demoDefinition.ClassQ.Replace(idHash))
                                .ClassR(demoDefinition.ClassR.Replace(idHash))
                                .ClassS(demoDefinition.ClassS.Replace(idHash))
                                .ClassT(demoDefinition.ClassT.Replace(idHash))
                                .ClassU(demoDefinition.ClassU.Replace(idHash))
                                .ClassV(demoDefinition.ClassV.Replace(idHash))
                                .ClassW(demoDefinition.ClassW.Replace(idHash))
                                .ClassX(demoDefinition.ClassX.Replace(idHash))
                                .ClassY(demoDefinition.ClassY.Replace(idHash))
                                .ClassZ(demoDefinition.ClassZ.Replace(idHash))
                                .NumA(demoDefinition.NumA)
                                .NumB(demoDefinition.NumB)
                                .NumC(demoDefinition.NumC)
                                .NumD(demoDefinition.NumD)
                                .NumE(demoDefinition.NumE)
                                .NumF(demoDefinition.NumF)
                                .NumG(demoDefinition.NumG)
                                .NumH(demoDefinition.NumH)
                                .NumI(demoDefinition.NumI)
                                .NumJ(demoDefinition.NumJ)
                                .NumK(demoDefinition.NumK)
                                .NumL(demoDefinition.NumL)
                                .NumM(demoDefinition.NumM)
                                .NumN(demoDefinition.NumN)
                                .NumO(demoDefinition.NumO)
                                .NumP(demoDefinition.NumP)
                                .NumQ(demoDefinition.NumQ)
                                .NumR(demoDefinition.NumR)
                                .NumS(demoDefinition.NumS)
                                .NumT(demoDefinition.NumT)
                                .NumU(demoDefinition.NumU)
                                .NumV(demoDefinition.NumV)
                                .NumW(demoDefinition.NumW)
                                .NumX(demoDefinition.NumX)
                                .NumY(demoDefinition.NumY)
                                .NumZ(demoDefinition.NumZ)
                                .DateA(demoDefinition.DateA)
                                .DateB(demoDefinition.DateB)
                                .DateC(demoDefinition.DateC)
                                .DateD(demoDefinition.DateD)
                                .DateE(demoDefinition.DateE)
                                .DateF(demoDefinition.DateF)
                                .DateG(demoDefinition.DateG)
                                .DateH(demoDefinition.DateH)
                                .DateI(demoDefinition.DateI)
                                .DateJ(demoDefinition.DateJ)
                                .DateK(demoDefinition.DateK)
                                .DateL(demoDefinition.DateL)
                                .DateM(demoDefinition.DateM)
                                .DateN(demoDefinition.DateN)
                                .DateO(demoDefinition.DateO)
                                .DateP(demoDefinition.DateP)
                                .DateQ(demoDefinition.DateQ)
                                .DateR(demoDefinition.DateR)
                                .DateS(demoDefinition.DateS)
                                .DateT(demoDefinition.DateT)
                                .DateU(demoDefinition.DateU)
                                .DateV(demoDefinition.DateV)
                                .DateW(demoDefinition.DateW)
                                .DateX(demoDefinition.DateX)
                                .DateY(demoDefinition.DateY)
                                .DateZ(demoDefinition.DateZ)
                                .DescriptionA(demoDefinition.DescriptionA)
                                .DescriptionB(demoDefinition.DescriptionB)
                                .DescriptionC(demoDefinition.DescriptionC)
                                .DescriptionD(demoDefinition.DescriptionD)
                                .DescriptionE(demoDefinition.DescriptionE)
                                .DescriptionF(demoDefinition.DescriptionF)
                                .DescriptionG(demoDefinition.DescriptionG)
                                .DescriptionH(demoDefinition.DescriptionH)
                                .DescriptionI(demoDefinition.DescriptionI)
                                .DescriptionJ(demoDefinition.DescriptionJ)
                                .DescriptionK(demoDefinition.DescriptionK)
                                .DescriptionL(demoDefinition.DescriptionL)
                                .DescriptionM(demoDefinition.DescriptionM)
                                .DescriptionN(demoDefinition.DescriptionN)
                                .DescriptionO(demoDefinition.DescriptionO)
                                .DescriptionP(demoDefinition.DescriptionP)
                                .DescriptionQ(demoDefinition.DescriptionQ)
                                .DescriptionR(demoDefinition.DescriptionR)
                                .DescriptionS(demoDefinition.DescriptionS)
                                .DescriptionT(demoDefinition.DescriptionT)
                                .DescriptionU(demoDefinition.DescriptionU)
                                .DescriptionV(demoDefinition.DescriptionV)
                                .DescriptionW(demoDefinition.DescriptionW)
                                .DescriptionX(demoDefinition.DescriptionX)
                                .DescriptionY(demoDefinition.DescriptionY)
                                .DescriptionZ(demoDefinition.DescriptionZ)
                                .CheckA(demoDefinition.CheckA)
                                .CheckB(demoDefinition.CheckB)
                                .CheckC(demoDefinition.CheckC)
                                .CheckD(demoDefinition.CheckD)
                                .CheckE(demoDefinition.CheckE)
                                .CheckF(demoDefinition.CheckF)
                                .CheckG(demoDefinition.CheckG)
                                .CheckH(demoDefinition.CheckH)
                                .CheckI(demoDefinition.CheckI)
                                .CheckJ(demoDefinition.CheckJ)
                                .CheckK(demoDefinition.CheckK)
                                .CheckL(demoDefinition.CheckL)
                                .CheckM(demoDefinition.CheckM)
                                .CheckN(demoDefinition.CheckN)
                                .CheckO(demoDefinition.CheckO)
                                .CheckP(demoDefinition.CheckP)
                                .CheckQ(demoDefinition.CheckQ)
                                .CheckR(demoDefinition.CheckR)
                                .CheckS(demoDefinition.CheckS)
                                .CheckT(demoDefinition.CheckT)
                                .CheckU(demoDefinition.CheckU)
                                .CheckV(demoDefinition.CheckV)
                                .CheckW(demoDefinition.CheckW)
                                .CheckX(demoDefinition.CheckX)
                                .CheckY(demoDefinition.CheckY)
                                .CheckZ(demoDefinition.CheckZ)
                                .Comments(Comments(demoModel, idHash, demoDefinition.Id))
                                .Creator(idHash.Get(demoDefinition.Creator))
                                .Updator(idHash.Get(demoDefinition.Updator))
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.CreatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false)
                    });
                    idHash.Add(demoDefinition.Id, issueId);
                    var siteModel = new SiteModel().Get(
                        where: Rds.SitesWhere().SiteId(idHash.Get(demoDefinition.ParentId)));
                    var ss = siteModel.IssuesSiteSettings(issueId);
                    var issueModel = new IssueModel(ss, issueId);
                    var fullText = issueModel.FullText(ss);
                    Rds.ExecuteNonQuery(statements:
                        Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(issueModel.SiteId)
                                .Title(issueModel.Title.DisplayValue)
                                .FullText(fullText, _using: fullText != null),
                            where: Rds.ItemsWhere().ReferenceId(issueModel.IssueId),
                            addUpdatorParam: false,
                            addUpdatedTimeParam: false));
                    var days = issueModel.CompletionTime.Value < DateTime.Now
                        ? (issueModel.CompletionTime.Value - issueModel.StartTime).Days
                        : (DateTime.Now - issueModel.StartTime).Days;
                    if (demoDefinition.ProgressRate > 0)
                    {
                        var startTime = issueModel.StartTime;
                        var progressRate = demoDefinition.ProgressRate;
                        var status = issueModel.Status.Value;
                        var creator = issueModel.Creator.Id;
                        var updator = issueModel.Updator.Id;
                        for (var d = 0; d < days -1; d++)
                        {
                            issueModel.VerUp = true;
                            issueModel.Update(ss);
                            var recordingTime = d > 0
                                ? startTime
                                    .AddDays(d)
                                    .AddHours(-6)
                                    .AddMinutes(new Random().Next(-360, +360))
                                : issueModel.CreatedTime.Value;
                            Rds.ExecuteNonQuery(statements:
                                Rds.UpdateIssues(
                                    tableType: Sqls.TableTypes.History,
                                    addUpdatedTimeParam: false,
                                    addUpdatorParam: false,
                                    param: Rds.IssuesParam()
                                        .ProgressRate(ProgressRate(progressRate, days, d))
                                        .Status(d > 0 ? 200 : 100)
                                        .Creator(creator)
                                        .Updator(updator)
                                        .CreatedTime(recordingTime)
                                        .UpdatedTime(recordingTime),
                                    where: Rds.IssuesWhere()
                                        .IssueId(issueModel.IssueId)
                                        .Ver(sub: Rds.SelectIssues(
                                            tableType: Sqls.TableTypes.HistoryWithoutFlag,
                                            column: Rds.IssuesColumn()
                                                .Ver(function: Sqls.Functions.Max),
                                            where: Rds.IssuesWhere()
                                                .IssueId(issueModel.IssueId)))));
                        }
                        Rds.ExecuteNonQuery(statements:
                            Rds.UpdateIssues(
                                addUpdatorParam: false,
                                addUpdatedTimeParam: false,
                                param: Rds.IssuesParam()
                                    .ProgressRate(progressRate)
                                    .Status(status)
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                                where: Rds.IssuesWhere()
                                    .IssueId(issueModel.IssueId)));
                    }
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static decimal ProgressRate(decimal progressRate, int days, int d)
        {
            return d == 0
                ? 0
                : progressRate / days * (d + (new Random().NextDouble() - 0.4).ToDecimal());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeResults(
            DemoModel demoModel, string parentId, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.ParentId == parentId)
                .Where(o => o.Type == "Results")
                .ForEach(demoDefinition =>
                {
                    var resultId = Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam()
                                .ReferenceType("Results")
                                .Creator(idHash.Get(demoDefinition.Creator))
                                .Updator(idHash.Get(demoDefinition.Updator))
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false),
                        Rds.InsertResults(
                            selectIdentity: true,
                            param: Rds.ResultsParam()
                                .SiteId(idHash.Get(demoDefinition.ParentId))
                                .ResultId(raw: Def.Sql.Identity)
                                .Title(demoDefinition.Title)
                                .Body(demoDefinition.Body.Replace(idHash))
                                .Status(demoDefinition.Status)
                                .Manager(idHash.Get(demoDefinition.Manager))
                                .Owner(idHash.Get(demoDefinition.Owner))
                                .ClassA(demoDefinition.ClassA.Replace(idHash))
                                .ClassB(demoDefinition.ClassB.Replace(idHash))
                                .ClassC(demoDefinition.ClassC.Replace(idHash))
                                .ClassD(demoDefinition.ClassD.Replace(idHash))
                                .ClassE(demoDefinition.ClassE.Replace(idHash))
                                .ClassF(demoDefinition.ClassF.Replace(idHash))
                                .ClassG(demoDefinition.ClassG.Replace(idHash))
                                .ClassH(demoDefinition.ClassH.Replace(idHash))
                                .ClassI(demoDefinition.ClassI.Replace(idHash))
                                .ClassJ(demoDefinition.ClassJ.Replace(idHash))
                                .ClassK(demoDefinition.ClassK.Replace(idHash))
                                .ClassL(demoDefinition.ClassL.Replace(idHash))
                                .ClassM(demoDefinition.ClassM.Replace(idHash))
                                .ClassN(demoDefinition.ClassN.Replace(idHash))
                                .ClassO(demoDefinition.ClassO.Replace(idHash))
                                .ClassP(demoDefinition.ClassP.Replace(idHash))
                                .ClassQ(demoDefinition.ClassQ.Replace(idHash))
                                .ClassR(demoDefinition.ClassR.Replace(idHash))
                                .ClassS(demoDefinition.ClassS.Replace(idHash))
                                .ClassT(demoDefinition.ClassT.Replace(idHash))
                                .ClassU(demoDefinition.ClassU.Replace(idHash))
                                .ClassV(demoDefinition.ClassV.Replace(idHash))
                                .ClassW(demoDefinition.ClassW.Replace(idHash))
                                .ClassX(demoDefinition.ClassX.Replace(idHash))
                                .ClassY(demoDefinition.ClassY.Replace(idHash))
                                .ClassZ(demoDefinition.ClassZ.Replace(idHash))
                                .NumA(demoDefinition.NumA)
                                .NumB(demoDefinition.NumB)
                                .NumC(demoDefinition.NumC)
                                .NumD(demoDefinition.NumD)
                                .NumE(demoDefinition.NumE)
                                .NumF(demoDefinition.NumF)
                                .NumG(demoDefinition.NumG)
                                .NumH(demoDefinition.NumH)
                                .NumI(demoDefinition.NumI)
                                .NumJ(demoDefinition.NumJ)
                                .NumK(demoDefinition.NumK)
                                .NumL(demoDefinition.NumL)
                                .NumM(demoDefinition.NumM)
                                .NumN(demoDefinition.NumN)
                                .NumO(demoDefinition.NumO)
                                .NumP(demoDefinition.NumP)
                                .NumQ(demoDefinition.NumQ)
                                .NumR(demoDefinition.NumR)
                                .NumS(demoDefinition.NumS)
                                .NumT(demoDefinition.NumT)
                                .NumU(demoDefinition.NumU)
                                .NumV(demoDefinition.NumV)
                                .NumW(demoDefinition.NumW)
                                .NumX(demoDefinition.NumX)
                                .NumY(demoDefinition.NumY)
                                .NumZ(demoDefinition.NumZ)
                                .DateA(demoDefinition.DateA)
                                .DateB(demoDefinition.DateB)
                                .DateC(demoDefinition.DateC)
                                .DateD(demoDefinition.DateD)
                                .DateE(demoDefinition.DateE)
                                .DateF(demoDefinition.DateF)
                                .DateG(demoDefinition.DateG)
                                .DateH(demoDefinition.DateH)
                                .DateI(demoDefinition.DateI)
                                .DateJ(demoDefinition.DateJ)
                                .DateK(demoDefinition.DateK)
                                .DateL(demoDefinition.DateL)
                                .DateM(demoDefinition.DateM)
                                .DateN(demoDefinition.DateN)
                                .DateO(demoDefinition.DateO)
                                .DateP(demoDefinition.DateP)
                                .DateQ(demoDefinition.DateQ)
                                .DateR(demoDefinition.DateR)
                                .DateS(demoDefinition.DateS)
                                .DateT(demoDefinition.DateT)
                                .DateU(demoDefinition.DateU)
                                .DateV(demoDefinition.DateV)
                                .DateW(demoDefinition.DateW)
                                .DateX(demoDefinition.DateX)
                                .DateY(demoDefinition.DateY)
                                .DateZ(demoDefinition.DateZ)
                                .DescriptionA(demoDefinition.DescriptionA)
                                .DescriptionB(demoDefinition.DescriptionB)
                                .DescriptionC(demoDefinition.DescriptionC)
                                .DescriptionD(demoDefinition.DescriptionD)
                                .DescriptionE(demoDefinition.DescriptionE)
                                .DescriptionF(demoDefinition.DescriptionF)
                                .DescriptionG(demoDefinition.DescriptionG)
                                .DescriptionH(demoDefinition.DescriptionH)
                                .DescriptionI(demoDefinition.DescriptionI)
                                .DescriptionJ(demoDefinition.DescriptionJ)
                                .DescriptionK(demoDefinition.DescriptionK)
                                .DescriptionL(demoDefinition.DescriptionL)
                                .DescriptionM(demoDefinition.DescriptionM)
                                .DescriptionN(demoDefinition.DescriptionN)
                                .DescriptionO(demoDefinition.DescriptionO)
                                .DescriptionP(demoDefinition.DescriptionP)
                                .DescriptionQ(demoDefinition.DescriptionQ)
                                .DescriptionR(demoDefinition.DescriptionR)
                                .DescriptionS(demoDefinition.DescriptionS)
                                .DescriptionT(demoDefinition.DescriptionT)
                                .DescriptionU(demoDefinition.DescriptionU)
                                .DescriptionV(demoDefinition.DescriptionV)
                                .DescriptionW(demoDefinition.DescriptionW)
                                .DescriptionX(demoDefinition.DescriptionX)
                                .DescriptionY(demoDefinition.DescriptionY)
                                .DescriptionZ(demoDefinition.DescriptionZ)
                                .CheckA(demoDefinition.CheckA)
                                .CheckB(demoDefinition.CheckB)
                                .CheckC(demoDefinition.CheckC)
                                .CheckD(demoDefinition.CheckD)
                                .CheckE(demoDefinition.CheckE)
                                .CheckF(demoDefinition.CheckF)
                                .CheckG(demoDefinition.CheckG)
                                .CheckH(demoDefinition.CheckH)
                                .CheckI(demoDefinition.CheckI)
                                .CheckJ(demoDefinition.CheckJ)
                                .CheckK(demoDefinition.CheckK)
                                .CheckL(demoDefinition.CheckL)
                                .CheckM(demoDefinition.CheckM)
                                .CheckN(demoDefinition.CheckN)
                                .CheckO(demoDefinition.CheckO)
                                .CheckP(demoDefinition.CheckP)
                                .CheckQ(demoDefinition.CheckQ)
                                .CheckR(demoDefinition.CheckR)
                                .CheckS(demoDefinition.CheckS)
                                .CheckT(demoDefinition.CheckT)
                                .CheckU(demoDefinition.CheckU)
                                .CheckV(demoDefinition.CheckV)
                                .CheckW(demoDefinition.CheckW)
                                .CheckX(demoDefinition.CheckX)
                                .CheckY(demoDefinition.CheckY)
                                .CheckZ(demoDefinition.CheckZ)
                                .Comments(Comments(demoModel, idHash, demoDefinition.Id))
                                .Creator(idHash.Get(demoDefinition.Creator))
                                .Updator(idHash.Get(demoDefinition.Updator))
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false)
                    });
                    idHash.Add(demoDefinition.Id, resultId);
                    var siteModel = new SiteModel().Get(
                        where: Rds.SitesWhere().SiteId(idHash.Get(demoDefinition.ParentId)));
                    var ss = siteModel.ResultsSiteSettings(resultId);
                    var resultModel = new ResultModel(ss, resultId);
                    var fullText = resultModel.FullText(ss);
                    Rds.ExecuteNonQuery(statements:
                        Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(resultModel.SiteId)
                                .Title(resultModel.Title.DisplayValue)
                                .FullText(fullText, _using: fullText != null),
                            where: Rds.ItemsWhere().ReferenceId(resultModel.ResultId),
                            addUpdatorParam: false,
                            addUpdatedTimeParam: false));
                    Libraries.Search.Indexes.Create(ss, resultModel);
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeLinks(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .Where(o => o.ClassB.Trim() != string.Empty)
                .ForEach(demoDefinition =>
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash.Get(demoDefinition.ClassB))
                            .SourceId(idHash.Get(demoDefinition.Id)))));
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .Where(o => o.ClassC.Trim() != string.Empty)
                .ForEach(demoDefinition =>
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash.Get(demoDefinition.ClassC))
                            .SourceId(idHash.Get(demoDefinition.Id)))));
            Def.DemoDefinitionCollection
                .Where(o => o.ClassA.RegexExists("^#[A-Za-z0-9]+?#$"))
                .ForEach(demoDefinition =>
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash.Get(demoDefinition.ClassA
                                .Substring(1, demoDefinition.ClassA.Length - 2)))
                            .SourceId(idHash.Get(demoDefinition.Id)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializePermissions(Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .Where(o => o.ParentId == string.Empty)
                .Select(o => o.Id)
                .ForEach(id =>
            {
                Rds.ExecuteNonQuery(statements:
                    Rds.InsertPermissions(
                        param: Rds.PermissionsParam()
                            .ReferenceId(idHash.Get(id))
                            .DeptId(0)
                            .UserId(idHash.Get("User1"))
                            .PermissionType(Permissions.Manager())));
                idHash.Where(o => o.Key.StartsWith("Dept")).Select(o => o.Value).ForEach(deptId =>
                {
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceId(idHash.Get(id))
                                .DeptId(deptId)
                                .UserId(0)
                                .PermissionType(Permissions.General())));
                });
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Comments(
            DemoModel demoModel,
            Dictionary<string, long> idHash,
            string parentId)
        {
            var comments = new Comments();
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Comments")
                .Where(o => o.ParentId == parentId)
                .Select((o, i) => new { DemoDefinition = o, Index = i })
                .ForEach(data =>
                    comments.Add(new Comment
                    {
                        CommentId = data.Index + 1,
                        CreatedTime = data.DemoDefinition.CreatedTime.DemoTime(demoModel),
                        Creator = idHash.Get(data.DemoDefinition.Creator).ToInt(),
                        Body = data.DemoDefinition.Body.Replace(idHash)
                    }));
            return comments.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Replace(this string self, Dictionary<string, long> idHash)
        {
            foreach (var id in self.RegexValues("#[A-Za-z0-9]+?#").Distinct())
            {
                self = self.Replace(
                    id, idHash.Get(id.ToString().Substring(1, id.Length - 2)).ToString());
            }
            return self;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static long Get(this Dictionary<string, long> idHash, string key)
        {
            return idHash.ContainsKey(key)
                ? idHash[key]
                : 0;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string LoginId(DemoModel demoModel, string userId)
        {
            return "Tenant" + demoModel.TenantId + "_" + userId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DateTime DemoTime(this DateTime self, DemoModel demoModel)
        {
            return self.AddDays(demoModel.TimeLag).ToUniversal();
        }
    }
}
