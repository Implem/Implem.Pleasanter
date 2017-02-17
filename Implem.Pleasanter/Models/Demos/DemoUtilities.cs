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
                SiteSettings = SiteSettingsUtilities.TenantsSiteSettings(),
                TenantName = mailAddress
            };
            tenantModel.Create();
            var demoModel = new DemoModel()
            {
                SiteSettings = SiteSettingsUtilities.DemosSiteSettings(),
                TenantId = tenantModel.TenantId,
                Passphrase = passphrase,
                MailAddress = mailAddress
            };
            demoModel.Create();
            demoModel.Initialize(new OutgoingMailModel()
            {
                SiteSettings = SiteSettingsUtilities.OutgoingMailsSiteSettings(),
                Title = new Title(Displays.DemoMailTitle()),
                Body = Displays.DemoMailBody(Url.Server(), passphrase),
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
            var demoModel = new DemoModel().Get(where: Rds.DemosWhere()
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
            InitializeSites(demoModel, idHash);
            InitializeIssues(demoModel, idHash);
            InitializeResults(demoModel, idHash);
            InitializeLinks(demoModel, idHash);
            InitializePermissions(idHash);
            Rds.ExecuteNonQuery(statements: Rds.UpdateDemos(
                param: Rds.DemosParam().Initialized(true),
                where: Rds.DemosWhere().Passphrase(demoModel.Passphrase)));
            Libraries.Migrators.SiteSettingsMigrator.Migrate();
            SiteInfo.Reflesh();
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
                                .LastName(demoDefinition.Title.Split_1st(' '))
                                .FirstName(demoDefinition.Title.Split_2nd(' '))
                                .DeptId(idHash[demoDefinition.ParentId].ToInt())
                                .FirstAndLastNameOrder(demoDefinition.ClassA == "1"
                                    ? Names.FirstAndLastNameOrders.FirstNameIsFirst
                                    : Names.FirstAndLastNameOrders.LastNameIsFirst)
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
            var topId = Def.DemoDefinitionCollection.First(o =>
                o.Type == "Sites" && o.ParentId == string.Empty).Id;
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .ForEach(demoDefinition =>
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements:
                        new SqlStatement[]
                        {
                            Rds.InsertItems(
                                selectIdentity: true,
                                param: Rds.ItemsParam()
                                    .ReferenceType("Sites")
                                    .Creator(idHash[demoDefinition.Creator])
                                    .Updator(idHash[demoDefinition.Updator])
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
                                        ? idHash[demoDefinition.ParentId]
                                        : 0)
                                    .InheritPermission(idHash, topId, demoDefinition.ParentId)
                                    .SiteSettings(demoDefinition.Body.Replace(idHash))
                                    .Creator(idHash[demoDefinition.Creator])
                                    .Updator(idHash[demoDefinition.Updator])
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                                addUpdatorParam: false)
                        })));
            new SiteCollection(where: Rds.SitesWhere().TenantId(demoModel.TenantId))
                .ForEach(siteModel =>
                {
                    Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                        param: Rds.ItemsParam()
                            .SiteId(siteModel.SiteId)
                            .Title(siteModel.Title.DisplayValue),
                        where: Rds.ItemsWhere().ReferenceId(siteModel.SiteId),
                        addUpdatorParam: false,
                        addUpdatedTimeParam: false));
                });
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
                : self.InheritPermission(idHash[topId]);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeIssues(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Issues")
                .ForEach(demoDefinition =>
                {
                    var issueId = Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam()
                                .ReferenceType("Issues")
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.CreatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false),
                        Rds.InsertIssues(
                            selectIdentity: true,
                            param: Rds.IssuesParam()
                                .SiteId(idHash[demoDefinition.ParentId])
                                .IssueId(raw: Def.Sql.Identity)
                                .Title(demoDefinition.Title)
                                .Body(demoDefinition.Body.Replace(idHash))
                                .StartTime(demoDefinition.StartTime.DemoTime(demoModel))
                                .CompletionTime(demoDefinition.CompletionTime
                                    .AddDays(1).DemoTime(demoModel))
                                .WorkValue(demoDefinition.WorkValue)
                                .ProgressRate(0)
                                .Status(demoDefinition.Status)
                                .Manager(idHash[demoDefinition.Manager])
                                .Owner(idHash[demoDefinition.Owner])
                                .ClassA(demoDefinition.ClassA.Replace(idHash))
                                .ClassB(demoDefinition.ClassB.Replace(idHash))
                                .ClassC(demoDefinition.ClassC.Replace(idHash))
                                .Comments(Comments(demoModel, idHash, demoDefinition.Id))
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.CreatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false)
                    });
                    idHash.Add(demoDefinition.Id, issueId);
                    var siteModel = new SiteModel().Get(
                        where: Rds.SitesWhere().SiteId(idHash[demoDefinition.ParentId]));
                    var ss = siteModel.IssuesSiteSettings();
                    var issueModel = new IssueModel(ss, issueId);
                    Rds.ExecuteNonQuery(statements:
                        Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(issueModel.SiteId)
                                .Title(issueModel.Title.DisplayValue),
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
                            issueModel.Update();
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
        private static void InitializeResults(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Results")
                .ForEach(demoDefinition =>
                {
                    var resultId = Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam()
                                .ReferenceType("Results")
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false),
                        Rds.InsertResults(
                            selectIdentity: true,
                            param: Rds.ResultsParam()
                                .SiteId(idHash[demoDefinition.ParentId])
                                .ResultId(raw: Def.Sql.Identity)
                                .Title(demoDefinition.Title)
                                .Body(demoDefinition.Body.Replace(idHash))
                                .Status(demoDefinition.Status)
                                .Manager(idHash[demoDefinition.Manager])
                                .Owner(idHash[demoDefinition.Owner])
                                .ClassA(demoDefinition.ClassA.Replace(idHash))
                                .ClassB(demoDefinition.ClassB.Replace(idHash))
                                .ClassC(demoDefinition.ClassC.Replace(idHash))
                                .Comments(Comments(demoModel, idHash, demoDefinition.Id))
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)),
                            addUpdatorParam: false)
                    });
                    idHash.Add(demoDefinition.Id, resultId);
                    var siteModel = new SiteModel().Get(
                        where: Rds.SitesWhere().SiteId(idHash[demoDefinition.ParentId]));
                    var ss = siteModel.ResultsSiteSettings();
                    var resultModel = new ResultModel(ss, resultId);
                    Rds.ExecuteNonQuery(statements:
                        Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(resultModel.SiteId)
                                .Title(resultModel.Title.DisplayValue),
                            where: Rds.ItemsWhere().ReferenceId(resultModel.ResultId),
                            addUpdatorParam: false,
                            addUpdatedTimeParam: false));
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
                            .DestinationId(idHash[demoDefinition.ClassB])
                            .SourceId(idHash[demoDefinition.Id]))));
            Def.DemoDefinitionCollection
                .Where(o => o.ClassA.RegexExists("^#[A-Za-z0-9]+?#$"))
                .ForEach(demoDefinition =>
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash[demoDefinition.ClassA
                                .Substring(1, demoDefinition.ClassA.Length - 2)])
                            .SourceId(idHash[demoDefinition.Id]))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializePermissions(Dictionary<string, long> idHash)
        {
            idHash.Where(o => o.Key.StartsWith("Site")).Select(o => o.Value).ForEach(siteId =>
            {
                Rds.ExecuteNonQuery(statements:
                    Rds.InsertPermissions(
                        param: Rds.PermissionsParam()
                            .ReferenceType("Sites")
                            .ReferenceId(siteId)
                            .DeptId(0)
                            .UserId(idHash["User1"])
                            .PermissionType(Permissions.Types.Manager)));
                idHash.Where(o => o.Key.StartsWith("Dept")).Select(o => o.Value).ForEach(deptId =>
                {
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceType("Sites")
                                .ReferenceId(siteId)
                                .DeptId(deptId)
                                .UserId(0)
                                .PermissionType(Permissions.Types.ReadWrite)));
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
                        CommentId = data.Index,
                        CreatedTime = data.DemoDefinition.CreatedTime.DemoTime(demoModel),
                        Creator = idHash[data.DemoDefinition.Creator].ToInt(),
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
                    id, idHash[id.ToString().Substring(1, id.Length - 2)].ToString());
            }
            return self;
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
