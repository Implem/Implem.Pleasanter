using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
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
        public static string Register(Context context)
        {
            var ss = new SiteSettings();
            var passphrase = Strings.NewGuid();
            var mailAddress = context.Forms.Data("Users_DemoMailAddress");
            var tenantModel = new TenantModel()
            {
                TenantName = mailAddress
            };
            tenantModel.Create(context: context, ss: ss);
            context = context.CreateContext(
                tenantId: tenantModel.TenantId,
                language: context.Language);
            var demoModel = new DemoModel()
            {
                Passphrase = passphrase,
                MailAddress = mailAddress
            };
            demoModel.Create(context: context, ss: ss);
            demoModel.Initialize(context: context, outgoingMailModel: new OutgoingMailModel()
            {
                Title = new Title(Displays.DemoMailTitle(context: context)),
                Body = Displays.DemoMailBody(
                    context: context,
                    data: new string[]
                    {
                        Locations.DemoUri(
                            context: context,
                            passphrase: passphrase),
                        Parameters.Service.DemoUsagePeriod.ToString()
                    }),
                From = new System.Net.Mail.MailAddress(Parameters.Mail.SupportFrom),
                To = mailAddress,
                Bcc = Parameters.Mail.SupportFrom
            });
            return Messages.ResponseSentAcceptanceMail(context: context)
                .Remove("#DemoForm")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool Login(Context context)
        {
            var demoModel = new DemoModel().Get(
                context: context,
                where: Rds.DemosWhere()
                    .Passphrase(context.QueryStrings.Data("passphrase")?.ToUpper())
                    .CreatedTime(
                        DateTime.Now.AddDays(Parameters.Service.DemoUsagePeriod * -1),
                        _operator: ">="));
            if (demoModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                context = context.CreateContext(
                    tenantId: demoModel.TenantId,
                    language: context.Language);
                var password = Strings.NewGuid().Sha512Cng();
                if (!demoModel.Initialized)
                {
                    var idHash = new Dictionary<string, long>();
                    demoModel.Initialize(
                        context: context,
                        idHash: idHash,
                        password: password);
                }
                else
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.UpdateUsers(
                            param: Rds.UsersParam().Password(password),
                            where: Rds.UsersWhere().LoginId(demoModel.LoginId)));
                }
                new UserModel()
                {
                    LoginId = demoModel.LoginId,
                    Password = password
                }.Authenticate(
                    context: context,
                    returnUrl: string.Empty);
                SiteInfo.Reflesh(context: context, force: true);
                return context.Authenticated;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string FirstUser(Context context)
        {
            return Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Users")
                .FirstOrDefault()?
                .Id;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Initialize(
            this DemoModel demoModel, Context context, OutgoingMailModel outgoingMailModel)
        {
            var idHash = new Dictionary<string, long>();
            var password = Strings.NewGuid().Sha512Cng();
            System.Threading.Tasks.Task.Run(() =>
            {
                demoModel.Initialize(context: context, idHash: idHash, password: password);
                outgoingMailModel.Send(context: context, ss: new SiteSettings());
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Initialize(
            this DemoModel demoModel,
            Context context,
            Dictionary<string, long> idHash,
            string password)
        {
            demoModel.InitializeTimeLag();
            InitializeDepts(
                context: context,
                demoModel: demoModel,
                idHash: idHash);
            InitializeUsers(
                context: context,
                demoModel: demoModel,
                idHash: idHash,
                password: password);
            SiteInfo.Reflesh(
                context: context,
                force: true);
            InitializeSites(context: context, demoModel: demoModel, idHash: idHash);
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Sites")
                .OrderBy(o => o.Id)
                .ForEach(o =>
                {
                    InitializeIssues(
                        context: context, demoModel: demoModel, parentId: o.Id, idHash: idHash);
                    InitializeResults(
                        context: context, demoModel: demoModel, parentId: o.Id, idHash: idHash);
                });
            InitializeLinks(
                context: context,
                demoModel: demoModel,
                idHash: idHash);
            InitializePermissions(
                context: context,
                idHash: idHash);
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateDemos(
                    param: Rds.DemosParam().Initialized(true),
                    where: Rds.DemosWhere().Passphrase(demoModel.Passphrase)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeDepts(
            Context context, DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Depts")
                .ForEach(demoDefinition => idHash.Add(
                    demoDefinition.Id, Repository.ExecuteScalar_response(
                        context: context,
                        selectIdentity: true,
                        statements: new SqlStatement[]
                        {
                            Rds.InsertDepts(
                                selectIdentity: true,
                                param: Rds.DeptsParam()
                                    .TenantId(demoModel.TenantId)
                                    .DeptCode(demoDefinition.ClassA)
                                    .DeptName(demoDefinition.Title)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel)))
                        }).Id.ToLong()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeUsers(
            Context context, DemoModel demoModel, Dictionary<string, long> idHash, string password)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Users")
                .ForEach(demoDefinition =>
                {
                    var loginId = LoginId(
                        demoModel: demoModel,
                        userId: demoDefinition.Id);
                    idHash.Add(demoDefinition.Id, Repository.ExecuteScalar_response(
                        context: context,
                        selectIdentity: true,
                        statements: new SqlStatement[]
                        {
                            Rds.InsertUsers(
                                selectIdentity: true,
                                param: Rds.UsersParam()
                                    .TenantId(demoModel.TenantId)
                                    .LoginId(loginId)
                                    .Password(password)
                                    .Name(demoDefinition.Title)
                                    .Language(context.Language)
                                    .DeptId(idHash.Get(demoDefinition.ParentId).ToInt())
                                    .Birthday(demoDefinition.ClassC.ToDateTime())
                                    .Gender(demoDefinition.ClassB)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))),
                            Rds.InsertMailAddresses(
                                param: Rds.MailAddressesParam()
                                    .OwnerId(raw: Def.Sql.Identity)
                                    .OwnerType("Users")
                                    .MailAddress(loginId + "@example.com"))
                        }).Id.ToLong());
                });
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateDemos(
                    where: Rds.DemosWhere().TenantId(demoModel.TenantId),
                    param: Rds.DemosParam()
                        .LoginId(LoginId(
                            demoModel: demoModel,
                            userId: Def.DemoDefinitionCollection
                                .Where(o => o.Language == context.Language)
                                .Where(o => o.Type == "Users")
                                .FirstOrDefault()?
                                .Id))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeSites(
            Context context, DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Sites" && o.ParentId == string.Empty)
                .ForEach(o => InitializeSites(
                    context: context,
                    demoModel: demoModel,
                    idHash: idHash,
                    topId: o.Id));
            new SiteCollection(
                context: context,
                where: Rds.SitesWhere().TenantId(demoModel.TenantId))
                .ForEach(siteModel =>
                    {
                        var fullText = siteModel.FullText(
                            context: context, ss: siteModel.SiteSettings);
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: Rds.UpdateItems(
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
            Context context, DemoModel demoModel, Dictionary<string, long> idHash, string topId)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Sites")
                .Where(o => o.Id == topId || o.ParentId == topId)
                .ForEach(demoDefinition =>
                {
                    var creator = idHash.Get(demoDefinition.Creator);
                    var updator = idHash.Get(demoDefinition.Updator);
                    context = context.CreateContext(
                        tenantId: demoModel.TenantId,
                        userId: updator.ToInt(),
                        language: context.Language);
                    idHash.Add(
                        demoDefinition.Id, Repository.ExecuteScalar_response(
                            context: context,
                            selectIdentity: true,
                            statements: new SqlStatement[]
                            {
                            Rds.InsertItems(
                                selectIdentity: true,
                                param: Rds.ItemsParam()
                                    .ReferenceType("Sites")
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel)),
                                addUpdatorParam: false),
                            Rds.InsertSites(
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
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel)),
                                addUpdatorParam: false)
                            }).Id.ToLong());
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
                : self.InheritPermission(idHash.Get(topId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeIssues(
            Context context,
            DemoModel demoModel,
            string parentId,
            Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.ParentId == parentId)
                .Where(o => o.Type == "Issues")
                .ForEach(demoDefinition =>
                {
                    var creator = idHash.Get(demoDefinition.Creator);
                    var updator = idHash.Get(demoDefinition.Updator);
                    context = context.CreateContext(
                        tenantId: demoModel.TenantId,
                        userId: updator.ToInt(),
                        language: context.Language);
                    var issueId = Repository.ExecuteScalar_response(
                        context: context,
                        selectIdentity: true,
                        statements: new SqlStatement[]
                        {
                            Rds.InsertItems(
                                selectIdentity: true,
                                param: Rds.ItemsParam()
                                    .ReferenceType("Issues")
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel)),
                                addUpdatorParam: false),
                            Rds.InsertIssues(
                                param: Rds.IssuesParam()
                                    .SiteId(idHash.Get(demoDefinition.ParentId))
                                    .IssueId(raw: Def.Sql.Identity)
                                    .Title(demoDefinition.Title)
                                    .Body(demoDefinition.Body.Replace(idHash))
                                    .StartTime(demoDefinition.StartTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .CompletionTime(demoDefinition.CompletionTime
                                        .AddDays(1)
                                        .DemoTime(
                                            context: context,
                                            demoModel: demoModel))
                                    .WorkValue(demoDefinition.WorkValue)
                                    .ProgressRate(0)
                                    .Status(demoDefinition.Status)
                                    .Manager(idHash.Get(demoDefinition.Manager))
                                    .Owner(idHash.Get(demoDefinition.Owner))
                                    .Add(columnBracket: "\"ClassA\"", name: "ClassA", value: demoDefinition.ClassA.Replace(idHash))
                                    .Add(columnBracket: "\"ClassB\"", name: "ClassB", value: demoDefinition.ClassB.Replace(idHash))
                                    .Add(columnBracket: "\"ClassC\"", name: "ClassC", value: demoDefinition.ClassC.Replace(idHash))
                                    .Add(columnBracket: "\"ClassD\"", name: "ClassD", value: demoDefinition.ClassD.Replace(idHash))
                                    .Add(columnBracket: "\"ClassE\"", name: "ClassE", value: demoDefinition.ClassE.Replace(idHash))
                                    .Add(columnBracket: "\"ClassF\"", name: "ClassF", value: demoDefinition.ClassF.Replace(idHash))
                                    .Add(columnBracket: "\"ClassG\"", name: "ClassG", value: demoDefinition.ClassG.Replace(idHash))
                                    .Add(columnBracket: "\"ClassH\"", name: "ClassH", value: demoDefinition.ClassH.Replace(idHash))
                                    .Add(columnBracket: "\"ClassI\"", name: "ClassI", value: demoDefinition.ClassI.Replace(idHash))
                                    .Add(columnBracket: "\"ClassJ\"", name: "ClassJ", value: demoDefinition.ClassJ.Replace(idHash))
                                    .Add(columnBracket: "\"ClassK\"", name: "ClassK", value: demoDefinition.ClassK.Replace(idHash))
                                    .Add(columnBracket: "\"ClassL\"", name: "ClassL", value: demoDefinition.ClassL.Replace(idHash))
                                    .Add(columnBracket: "\"ClassM\"", name: "ClassM", value: demoDefinition.ClassM.Replace(idHash))
                                    .Add(columnBracket: "\"ClassN\"", name: "ClassN", value: demoDefinition.ClassN.Replace(idHash))
                                    .Add(columnBracket: "\"ClassO\"", name: "ClassO", value: demoDefinition.ClassO.Replace(idHash))
                                    .Add(columnBracket: "\"ClassP\"", name: "ClassP", value: demoDefinition.ClassP.Replace(idHash))
                                    .Add(columnBracket: "\"ClassQ\"", name: "ClassQ", value: demoDefinition.ClassQ.Replace(idHash))
                                    .Add(columnBracket: "\"ClassR\"", name: "ClassR", value: demoDefinition.ClassR.Replace(idHash))
                                    .Add(columnBracket: "\"ClassS\"", name: "ClassS", value: demoDefinition.ClassS.Replace(idHash))
                                    .Add(columnBracket: "\"ClassT\"", name: "ClassT", value: demoDefinition.ClassT.Replace(idHash))
                                    .Add(columnBracket: "\"ClassU\"", name: "ClassU", value: demoDefinition.ClassU.Replace(idHash))
                                    .Add(columnBracket: "\"ClassV\"", name: "ClassV", value: demoDefinition.ClassV.Replace(idHash))
                                    .Add(columnBracket: "\"ClassW\"", name: "ClassW", value: demoDefinition.ClassW.Replace(idHash))
                                    .Add(columnBracket: "\"ClassX\"", name: "ClassX", value: demoDefinition.ClassX.Replace(idHash))
                                    .Add(columnBracket: "\"ClassY\"", name: "ClassY", value: demoDefinition.ClassY.Replace(idHash))
                                    .Add(columnBracket: "\"ClassZ\"", name: "ClassZ", value: demoDefinition.ClassZ.Replace(idHash))
                                    .Add(columnBracket: "\"NumA\"", name: "NumA", value: demoDefinition.NumA)
                                    .Add(columnBracket: "\"NumB\"", name: "NumB", value: demoDefinition.NumB)
                                    .Add(columnBracket: "\"NumC\"", name: "NumC", value: demoDefinition.NumC)
                                    .Add(columnBracket: "\"NumD\"", name: "NumD", value: demoDefinition.NumD)
                                    .Add(columnBracket: "\"NumE\"", name: "NumE", value: demoDefinition.NumE)
                                    .Add(columnBracket: "\"NumF\"", name: "NumF", value: demoDefinition.NumF)
                                    .Add(columnBracket: "\"NumG\"", name: "NumG", value: demoDefinition.NumG)
                                    .Add(columnBracket: "\"NumH\"", name: "NumH", value: demoDefinition.NumH)
                                    .Add(columnBracket: "\"NumI\"", name: "NumI", value: demoDefinition.NumI)
                                    .Add(columnBracket: "\"NumJ\"", name: "NumJ", value: demoDefinition.NumJ)
                                    .Add(columnBracket: "\"NumK\"", name: "NumK", value: demoDefinition.NumK)
                                    .Add(columnBracket: "\"NumL\"", name: "NumL", value: demoDefinition.NumL)
                                    .Add(columnBracket: "\"NumM\"", name: "NumM", value: demoDefinition.NumM)
                                    .Add(columnBracket: "\"NumN\"", name: "NumN", value: demoDefinition.NumN)
                                    .Add(columnBracket: "\"NumO\"", name: "NumO", value: demoDefinition.NumO)
                                    .Add(columnBracket: "\"NumP\"", name: "NumP", value: demoDefinition.NumP)
                                    .Add(columnBracket: "\"NumQ\"", name: "NumQ", value: demoDefinition.NumQ)
                                    .Add(columnBracket: "\"NumR\"", name: "NumR", value: demoDefinition.NumR)
                                    .Add(columnBracket: "\"NumS\"", name: "NumS", value: demoDefinition.NumS)
                                    .Add(columnBracket: "\"NumT\"", name: "NumT", value: demoDefinition.NumT)
                                    .Add(columnBracket: "\"NumU\"", name: "NumU", value: demoDefinition.NumU)
                                    .Add(columnBracket: "\"NumV\"", name: "NumV", value: demoDefinition.NumV)
                                    .Add(columnBracket: "\"NumW\"", name: "NumW", value: demoDefinition.NumW)
                                    .Add(columnBracket: "\"NumX\"", name: "NumX", value: demoDefinition.NumX)
                                    .Add(columnBracket: "\"NumY\"", name: "NumY", value: demoDefinition.NumY)
                                    .Add(columnBracket: "\"NumZ\"", name: "NumZ", value: demoDefinition.NumZ)
                                    .Add(columnBracket: "\"DateA\"", name: "DateA", value: demoDefinition.DateA)
                                    .Add(columnBracket: "\"DateB\"", name: "DateB", value: demoDefinition.DateB)
                                    .Add(columnBracket: "\"DateC\"", name: "DateC", value: demoDefinition.DateC)
                                    .Add(columnBracket: "\"DateD\"", name: "DateD", value: demoDefinition.DateD)
                                    .Add(columnBracket: "\"DateE\"", name: "DateE", value: demoDefinition.DateE)
                                    .Add(columnBracket: "\"DateF\"", name: "DateF", value: demoDefinition.DateF)
                                    .Add(columnBracket: "\"DateG\"", name: "DateG", value: demoDefinition.DateG)
                                    .Add(columnBracket: "\"DateH\"", name: "DateH", value: demoDefinition.DateH)
                                    .Add(columnBracket: "\"DateI\"", name: "DateI", value: demoDefinition.DateI)
                                    .Add(columnBracket: "\"DateJ\"", name: "DateJ", value: demoDefinition.DateJ)
                                    .Add(columnBracket: "\"DateK\"", name: "DateK", value: demoDefinition.DateK)
                                    .Add(columnBracket: "\"DateL\"", name: "DateL", value: demoDefinition.DateL)
                                    .Add(columnBracket: "\"DateM\"", name: "DateM", value: demoDefinition.DateM)
                                    .Add(columnBracket: "\"DateN\"", name: "DateN", value: demoDefinition.DateN)
                                    .Add(columnBracket: "\"DateO\"", name: "DateO", value: demoDefinition.DateO)
                                    .Add(columnBracket: "\"DateP\"", name: "DateP", value: demoDefinition.DateP)
                                    .Add(columnBracket: "\"DateQ\"", name: "DateQ", value: demoDefinition.DateQ)
                                    .Add(columnBracket: "\"DateR\"", name: "DateR", value: demoDefinition.DateR)
                                    .Add(columnBracket: "\"DateS\"", name: "DateS", value: demoDefinition.DateS)
                                    .Add(columnBracket: "\"DateT\"", name: "DateT", value: demoDefinition.DateT)
                                    .Add(columnBracket: "\"DateU\"", name: "DateU", value: demoDefinition.DateU)
                                    .Add(columnBracket: "\"DateV\"", name: "DateV", value: demoDefinition.DateV)
                                    .Add(columnBracket: "\"DateW\"", name: "DateW", value: demoDefinition.DateW)
                                    .Add(columnBracket: "\"DateX\"", name: "DateX", value: demoDefinition.DateX)
                                    .Add(columnBracket: "\"DateY\"", name: "DateY", value: demoDefinition.DateY)
                                    .Add(columnBracket: "\"DateZ\"", name: "DateZ", value: demoDefinition.DateZ)
                                    .Add(columnBracket: "\"DescriptionA\"", name: "DescriptionA", value: demoDefinition.DescriptionA)
                                    .Add(columnBracket: "\"DescriptionB\"", name: "DescriptionB", value: demoDefinition.DescriptionB)
                                    .Add(columnBracket: "\"DescriptionC\"", name: "DescriptionC", value: demoDefinition.DescriptionC)
                                    .Add(columnBracket: "\"DescriptionD\"", name: "DescriptionD", value: demoDefinition.DescriptionD)
                                    .Add(columnBracket: "\"DescriptionE\"", name: "DescriptionE", value: demoDefinition.DescriptionE)
                                    .Add(columnBracket: "\"DescriptionF\"", name: "DescriptionF", value: demoDefinition.DescriptionF)
                                    .Add(columnBracket: "\"DescriptionG\"", name: "DescriptionG", value: demoDefinition.DescriptionG)
                                    .Add(columnBracket: "\"DescriptionH\"", name: "DescriptionH", value: demoDefinition.DescriptionH)
                                    .Add(columnBracket: "\"DescriptionI\"", name: "DescriptionI", value: demoDefinition.DescriptionI)
                                    .Add(columnBracket: "\"DescriptionJ\"", name: "DescriptionJ", value: demoDefinition.DescriptionJ)
                                    .Add(columnBracket: "\"DescriptionK\"", name: "DescriptionK", value: demoDefinition.DescriptionK)
                                    .Add(columnBracket: "\"DescriptionL\"", name: "DescriptionL", value: demoDefinition.DescriptionL)
                                    .Add(columnBracket: "\"DescriptionM\"", name: "DescriptionM", value: demoDefinition.DescriptionM)
                                    .Add(columnBracket: "\"DescriptionN\"", name: "DescriptionN", value: demoDefinition.DescriptionN)
                                    .Add(columnBracket: "\"DescriptionO\"", name: "DescriptionO", value: demoDefinition.DescriptionO)
                                    .Add(columnBracket: "\"DescriptionP\"", name: "DescriptionP", value: demoDefinition.DescriptionP)
                                    .Add(columnBracket: "\"DescriptionQ\"", name: "DescriptionQ", value: demoDefinition.DescriptionQ)
                                    .Add(columnBracket: "\"DescriptionR\"", name: "DescriptionR", value: demoDefinition.DescriptionR)
                                    .Add(columnBracket: "\"DescriptionS\"", name: "DescriptionS", value: demoDefinition.DescriptionS)
                                    .Add(columnBracket: "\"DescriptionT\"", name: "DescriptionT", value: demoDefinition.DescriptionT)
                                    .Add(columnBracket: "\"DescriptionU\"", name: "DescriptionU", value: demoDefinition.DescriptionU)
                                    .Add(columnBracket: "\"DescriptionV\"", name: "DescriptionV", value: demoDefinition.DescriptionV)
                                    .Add(columnBracket: "\"DescriptionW\"", name: "DescriptionW", value: demoDefinition.DescriptionW)
                                    .Add(columnBracket: "\"DescriptionX\"", name: "DescriptionX", value: demoDefinition.DescriptionX)
                                    .Add(columnBracket: "\"DescriptionY\"", name: "DescriptionY", value: demoDefinition.DescriptionY)
                                    .Add(columnBracket: "\"DescriptionZ\"", name: "DescriptionZ", value: demoDefinition.DescriptionZ)
                                    .Add(columnBracket: "\"CheckA\"", name: "CheckA", value: demoDefinition.CheckA)
                                    .Add(columnBracket: "\"CheckB\"", name: "CheckB", value: demoDefinition.CheckB)
                                    .Add(columnBracket: "\"CheckC\"", name: "CheckC", value: demoDefinition.CheckC)
                                    .Add(columnBracket: "\"CheckD\"", name: "CheckD", value: demoDefinition.CheckD)
                                    .Add(columnBracket: "\"CheckE\"", name: "CheckE", value: demoDefinition.CheckE)
                                    .Add(columnBracket: "\"CheckF\"", name: "CheckF", value: demoDefinition.CheckF)
                                    .Add(columnBracket: "\"CheckG\"", name: "CheckG", value: demoDefinition.CheckG)
                                    .Add(columnBracket: "\"CheckH\"", name: "CheckH", value: demoDefinition.CheckH)
                                    .Add(columnBracket: "\"CheckI\"", name: "CheckI", value: demoDefinition.CheckI)
                                    .Add(columnBracket: "\"CheckJ\"", name: "CheckJ", value: demoDefinition.CheckJ)
                                    .Add(columnBracket: "\"CheckK\"", name: "CheckK", value: demoDefinition.CheckK)
                                    .Add(columnBracket: "\"CheckL\"", name: "CheckL", value: demoDefinition.CheckL)
                                    .Add(columnBracket: "\"CheckM\"", name: "CheckM", value: demoDefinition.CheckM)
                                    .Add(columnBracket: "\"CheckN\"", name: "CheckN", value: demoDefinition.CheckN)
                                    .Add(columnBracket: "\"CheckO\"", name: "CheckO", value: demoDefinition.CheckO)
                                    .Add(columnBracket: "\"CheckP\"", name: "CheckP", value: demoDefinition.CheckP)
                                    .Add(columnBracket: "\"CheckQ\"", name: "CheckQ", value: demoDefinition.CheckQ)
                                    .Add(columnBracket: "\"CheckR\"", name: "CheckR", value: demoDefinition.CheckR)
                                    .Add(columnBracket: "\"CheckS\"", name: "CheckS", value: demoDefinition.CheckS)
                                    .Add(columnBracket: "\"CheckT\"", name: "CheckT", value: demoDefinition.CheckT)
                                    .Add(columnBracket: "\"CheckU\"", name: "CheckU", value: demoDefinition.CheckU)
                                    .Add(columnBracket: "\"CheckV\"", name: "CheckV", value: demoDefinition.CheckV)
                                    .Add(columnBracket: "\"CheckW\"", name: "CheckW", value: demoDefinition.CheckW)
                                    .Add(columnBracket: "\"CheckX\"", name: "CheckX", value: demoDefinition.CheckX)
                                    .Add(columnBracket: "\"CheckY\"", name: "CheckY", value: demoDefinition.CheckY)
                                    .Add(columnBracket: "\"CheckZ\"", name: "CheckZ", value: demoDefinition.CheckZ)
                                    .Comments(Comments(
                                        context: context,
                                        demoModel: demoModel,
                                        idHash: idHash,
                                        parentId: demoDefinition.Id))
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel)),
                                addUpdatorParam: false)
                        }).Id.ToLong();
                    idHash.Add(demoDefinition.Id, issueId);
                    var siteModel = new SiteModel().Get(
                        context: context,
                        where: Rds.SitesWhere().SiteId(idHash.Get(demoDefinition.ParentId)));
                    var ss = siteModel.IssuesSiteSettings(context: context, referenceId: issueId);
                    var issueModel = new IssueModel(
                        context: context,
                        ss: ss,
                        issueId: issueId);
                    var fullText = issueModel.FullText(context: context, ss: ss);
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.UpdateItems(
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
                        for (var d = 0; d < days -1; d++)
                        {
                            issueModel.VerUp = true;
                            issueModel.Update(context: context, ss: ss);
                            var recordingTime = d > 0
                                ? startTime
                                    .AddDays(d)
                                    .AddHours(-6)
                                    .AddMinutes(new Random().Next(-360, +360))
                                : issueModel.CreatedTime.Value;
                            Repository.ExecuteNonQuery(
                                context: context,
                                statements: Rds.UpdateIssues(
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
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: Rds.UpdateIssues(
                                addUpdatorParam: false,
                                addUpdatedTimeParam: false,
                                param: Rds.IssuesParam()
                                    .ProgressRate(progressRate)
                                    .Status(status)
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel)),
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
            Context context,
            DemoModel demoModel,
            string parentId,
            Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.ParentId == parentId)
                .Where(o => o.Type == "Results")
                .ForEach(demoDefinition =>
                {
                    var creator = idHash.Get(demoDefinition.Creator);
                    var updator = idHash.Get(demoDefinition.Updator);
                    context = context.CreateContext(
                        tenantId: demoModel.TenantId,
                        userId: updator.ToInt(),
                        language: context.Language);
                    var resultId = Repository.ExecuteScalar_response(
                        context: context,
                        selectIdentity: true,
                        statements: new SqlStatement[]
                        {
                            Rds.InsertItems(
                                selectIdentity: true,
                                param: Rds.ItemsParam()
                                    .ReferenceType("Results")
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel)),
                                addUpdatorParam: false),
                            Rds.InsertResults(
                                param: Rds.ResultsParam()
                                    .SiteId(idHash.Get(demoDefinition.ParentId))
                                    .ResultId(raw: Def.Sql.Identity)
                                    .Title(demoDefinition.Title)
                                    .Body(demoDefinition.Body.Replace(idHash))
                                    .Status(demoDefinition.Status)
                                    .Manager(idHash.Get(demoDefinition.Manager))
                                    .Owner(idHash.Get(demoDefinition.Owner))
                                    .Add(columnBracket: "\"ClassA\"", name: "ClassA", value: demoDefinition.ClassA.Replace(idHash))
                                    .Add(columnBracket: "\"ClassB\"", name: "ClassB", value: demoDefinition.ClassB.Replace(idHash))
                                    .Add(columnBracket: "\"ClassC\"", name: "ClassC", value: demoDefinition.ClassC.Replace(idHash))
                                    .Add(columnBracket: "\"ClassD\"", name: "ClassD", value: demoDefinition.ClassD.Replace(idHash))
                                    .Add(columnBracket: "\"ClassE\"", name: "ClassE", value: demoDefinition.ClassE.Replace(idHash))
                                    .Add(columnBracket: "\"ClassF\"", name: "ClassF", value: demoDefinition.ClassF.Replace(idHash))
                                    .Add(columnBracket: "\"ClassG\"", name: "ClassG", value: demoDefinition.ClassG.Replace(idHash))
                                    .Add(columnBracket: "\"ClassH\"", name: "ClassH", value: demoDefinition.ClassH.Replace(idHash))
                                    .Add(columnBracket: "\"ClassI\"", name: "ClassI", value: demoDefinition.ClassI.Replace(idHash))
                                    .Add(columnBracket: "\"ClassJ\"", name: "ClassJ", value: demoDefinition.ClassJ.Replace(idHash))
                                    .Add(columnBracket: "\"ClassK\"", name: "ClassK", value: demoDefinition.ClassK.Replace(idHash))
                                    .Add(columnBracket: "\"ClassL\"", name: "ClassL", value: demoDefinition.ClassL.Replace(idHash))
                                    .Add(columnBracket: "\"ClassM\"", name: "ClassM", value: demoDefinition.ClassM.Replace(idHash))
                                    .Add(columnBracket: "\"ClassN\"", name: "ClassN", value: demoDefinition.ClassN.Replace(idHash))
                                    .Add(columnBracket: "\"ClassO\"", name: "ClassO", value: demoDefinition.ClassO.Replace(idHash))
                                    .Add(columnBracket: "\"ClassP\"", name: "ClassP", value: demoDefinition.ClassP.Replace(idHash))
                                    .Add(columnBracket: "\"ClassQ\"", name: "ClassQ", value: demoDefinition.ClassQ.Replace(idHash))
                                    .Add(columnBracket: "\"ClassR\"", name: "ClassR", value: demoDefinition.ClassR.Replace(idHash))
                                    .Add(columnBracket: "\"ClassS\"", name: "ClassS", value: demoDefinition.ClassS.Replace(idHash))
                                    .Add(columnBracket: "\"ClassT\"", name: "ClassT", value: demoDefinition.ClassT.Replace(idHash))
                                    .Add(columnBracket: "\"ClassU\"", name: "ClassU", value: demoDefinition.ClassU.Replace(idHash))
                                    .Add(columnBracket: "\"ClassV\"", name: "ClassV", value: demoDefinition.ClassV.Replace(idHash))
                                    .Add(columnBracket: "\"ClassW\"", name: "ClassW", value: demoDefinition.ClassW.Replace(idHash))
                                    .Add(columnBracket: "\"ClassX\"", name: "ClassX", value: demoDefinition.ClassX.Replace(idHash))
                                    .Add(columnBracket: "\"ClassY\"", name: "ClassY", value: demoDefinition.ClassY.Replace(idHash))
                                    .Add(columnBracket: "\"ClassZ\"", name: "ClassZ", value: demoDefinition.ClassZ.Replace(idHash))
                                    .Add(columnBracket: "\"NumA\"", name: "NumA", value: demoDefinition.NumA)
                                    .Add(columnBracket: "\"NumB\"", name: "NumB", value: demoDefinition.NumB)
                                    .Add(columnBracket: "\"NumC\"", name: "NumC", value: demoDefinition.NumC)
                                    .Add(columnBracket: "\"NumD\"", name: "NumD", value: demoDefinition.NumD)
                                    .Add(columnBracket: "\"NumE\"", name: "NumE", value: demoDefinition.NumE)
                                    .Add(columnBracket: "\"NumF\"", name: "NumF", value: demoDefinition.NumF)
                                    .Add(columnBracket: "\"NumG\"", name: "NumG", value: demoDefinition.NumG)
                                    .Add(columnBracket: "\"NumH\"", name: "NumH", value: demoDefinition.NumH)
                                    .Add(columnBracket: "\"NumI\"", name: "NumI", value: demoDefinition.NumI)
                                    .Add(columnBracket: "\"NumJ\"", name: "NumJ", value: demoDefinition.NumJ)
                                    .Add(columnBracket: "\"NumK\"", name: "NumK", value: demoDefinition.NumK)
                                    .Add(columnBracket: "\"NumL\"", name: "NumL", value: demoDefinition.NumL)
                                    .Add(columnBracket: "\"NumM\"", name: "NumM", value: demoDefinition.NumM)
                                    .Add(columnBracket: "\"NumN\"", name: "NumN", value: demoDefinition.NumN)
                                    .Add(columnBracket: "\"NumO\"", name: "NumO", value: demoDefinition.NumO)
                                    .Add(columnBracket: "\"NumP\"", name: "NumP", value: demoDefinition.NumP)
                                    .Add(columnBracket: "\"NumQ\"", name: "NumQ", value: demoDefinition.NumQ)
                                    .Add(columnBracket: "\"NumR\"", name: "NumR", value: demoDefinition.NumR)
                                    .Add(columnBracket: "\"NumS\"", name: "NumS", value: demoDefinition.NumS)
                                    .Add(columnBracket: "\"NumT\"", name: "NumT", value: demoDefinition.NumT)
                                    .Add(columnBracket: "\"NumU\"", name: "NumU", value: demoDefinition.NumU)
                                    .Add(columnBracket: "\"NumV\"", name: "NumV", value: demoDefinition.NumV)
                                    .Add(columnBracket: "\"NumW\"", name: "NumW", value: demoDefinition.NumW)
                                    .Add(columnBracket: "\"NumX\"", name: "NumX", value: demoDefinition.NumX)
                                    .Add(columnBracket: "\"NumY\"", name: "NumY", value: demoDefinition.NumY)
                                    .Add(columnBracket: "\"NumZ\"", name: "NumZ", value: demoDefinition.NumZ)
                                    .Add(columnBracket: "\"DateA\"", name: "DateA", value: demoDefinition.DateA)
                                    .Add(columnBracket: "\"DateB\"", name: "DateB", value: demoDefinition.DateB)
                                    .Add(columnBracket: "\"DateC\"", name: "DateC", value: demoDefinition.DateC)
                                    .Add(columnBracket: "\"DateD\"", name: "DateD", value: demoDefinition.DateD)
                                    .Add(columnBracket: "\"DateE\"", name: "DateE", value: demoDefinition.DateE)
                                    .Add(columnBracket: "\"DateF\"", name: "DateF", value: demoDefinition.DateF)
                                    .Add(columnBracket: "\"DateG\"", name: "DateG", value: demoDefinition.DateG)
                                    .Add(columnBracket: "\"DateH\"", name: "DateH", value: demoDefinition.DateH)
                                    .Add(columnBracket: "\"DateI\"", name: "DateI", value: demoDefinition.DateI)
                                    .Add(columnBracket: "\"DateJ\"", name: "DateJ", value: demoDefinition.DateJ)
                                    .Add(columnBracket: "\"DateK\"", name: "DateK", value: demoDefinition.DateK)
                                    .Add(columnBracket: "\"DateL\"", name: "DateL", value: demoDefinition.DateL)
                                    .Add(columnBracket: "\"DateM\"", name: "DateM", value: demoDefinition.DateM)
                                    .Add(columnBracket: "\"DateN\"", name: "DateN", value: demoDefinition.DateN)
                                    .Add(columnBracket: "\"DateO\"", name: "DateO", value: demoDefinition.DateO)
                                    .Add(columnBracket: "\"DateP\"", name: "DateP", value: demoDefinition.DateP)
                                    .Add(columnBracket: "\"DateQ\"", name: "DateQ", value: demoDefinition.DateQ)
                                    .Add(columnBracket: "\"DateR\"", name: "DateR", value: demoDefinition.DateR)
                                    .Add(columnBracket: "\"DateS\"", name: "DateS", value: demoDefinition.DateS)
                                    .Add(columnBracket: "\"DateT\"", name: "DateT", value: demoDefinition.DateT)
                                    .Add(columnBracket: "\"DateU\"", name: "DateU", value: demoDefinition.DateU)
                                    .Add(columnBracket: "\"DateV\"", name: "DateV", value: demoDefinition.DateV)
                                    .Add(columnBracket: "\"DateW\"", name: "DateW", value: demoDefinition.DateW)
                                    .Add(columnBracket: "\"DateX\"", name: "DateX", value: demoDefinition.DateX)
                                    .Add(columnBracket: "\"DateY\"", name: "DateY", value: demoDefinition.DateY)
                                    .Add(columnBracket: "\"DateZ\"", name: "DateZ", value: demoDefinition.DateZ)
                                    .Add(columnBracket: "\"DescriptionA\"", name: "DescriptionA", value: demoDefinition.DescriptionA)
                                    .Add(columnBracket: "\"DescriptionB\"", name: "DescriptionB", value: demoDefinition.DescriptionB)
                                    .Add(columnBracket: "\"DescriptionC\"", name: "DescriptionC", value: demoDefinition.DescriptionC)
                                    .Add(columnBracket: "\"DescriptionD\"", name: "DescriptionD", value: demoDefinition.DescriptionD)
                                    .Add(columnBracket: "\"DescriptionE\"", name: "DescriptionE", value: demoDefinition.DescriptionE)
                                    .Add(columnBracket: "\"DescriptionF\"", name: "DescriptionF", value: demoDefinition.DescriptionF)
                                    .Add(columnBracket: "\"DescriptionG\"", name: "DescriptionG", value: demoDefinition.DescriptionG)
                                    .Add(columnBracket: "\"DescriptionH\"", name: "DescriptionH", value: demoDefinition.DescriptionH)
                                    .Add(columnBracket: "\"DescriptionI\"", name: "DescriptionI", value: demoDefinition.DescriptionI)
                                    .Add(columnBracket: "\"DescriptionJ\"", name: "DescriptionJ", value: demoDefinition.DescriptionJ)
                                    .Add(columnBracket: "\"DescriptionK\"", name: "DescriptionK", value: demoDefinition.DescriptionK)
                                    .Add(columnBracket: "\"DescriptionL\"", name: "DescriptionL", value: demoDefinition.DescriptionL)
                                    .Add(columnBracket: "\"DescriptionM\"", name: "DescriptionM", value: demoDefinition.DescriptionM)
                                    .Add(columnBracket: "\"DescriptionN\"", name: "DescriptionN", value: demoDefinition.DescriptionN)
                                    .Add(columnBracket: "\"DescriptionO\"", name: "DescriptionO", value: demoDefinition.DescriptionO)
                                    .Add(columnBracket: "\"DescriptionP\"", name: "DescriptionP", value: demoDefinition.DescriptionP)
                                    .Add(columnBracket: "\"DescriptionQ\"", name: "DescriptionQ", value: demoDefinition.DescriptionQ)
                                    .Add(columnBracket: "\"DescriptionR\"", name: "DescriptionR", value: demoDefinition.DescriptionR)
                                    .Add(columnBracket: "\"DescriptionS\"", name: "DescriptionS", value: demoDefinition.DescriptionS)
                                    .Add(columnBracket: "\"DescriptionT\"", name: "DescriptionT", value: demoDefinition.DescriptionT)
                                    .Add(columnBracket: "\"DescriptionU\"", name: "DescriptionU", value: demoDefinition.DescriptionU)
                                    .Add(columnBracket: "\"DescriptionV\"", name: "DescriptionV", value: demoDefinition.DescriptionV)
                                    .Add(columnBracket: "\"DescriptionW\"", name: "DescriptionW", value: demoDefinition.DescriptionW)
                                    .Add(columnBracket: "\"DescriptionX\"", name: "DescriptionX", value: demoDefinition.DescriptionX)
                                    .Add(columnBracket: "\"DescriptionY\"", name: "DescriptionY", value: demoDefinition.DescriptionY)
                                    .Add(columnBracket: "\"DescriptionZ\"", name: "DescriptionZ", value: demoDefinition.DescriptionZ)
                                    .Add(columnBracket: "\"CheckA\"", name: "CheckA", value: demoDefinition.CheckA)
                                    .Add(columnBracket: "\"CheckB\"", name: "CheckB", value: demoDefinition.CheckB)
                                    .Add(columnBracket: "\"CheckC\"", name: "CheckC", value: demoDefinition.CheckC)
                                    .Add(columnBracket: "\"CheckD\"", name: "CheckD", value: demoDefinition.CheckD)
                                    .Add(columnBracket: "\"CheckE\"", name: "CheckE", value: demoDefinition.CheckE)
                                    .Add(columnBracket: "\"CheckF\"", name: "CheckF", value: demoDefinition.CheckF)
                                    .Add(columnBracket: "\"CheckG\"", name: "CheckG", value: demoDefinition.CheckG)
                                    .Add(columnBracket: "\"CheckH\"", name: "CheckH", value: demoDefinition.CheckH)
                                    .Add(columnBracket: "\"CheckI\"", name: "CheckI", value: demoDefinition.CheckI)
                                    .Add(columnBracket: "\"CheckJ\"", name: "CheckJ", value: demoDefinition.CheckJ)
                                    .Add(columnBracket: "\"CheckK\"", name: "CheckK", value: demoDefinition.CheckK)
                                    .Add(columnBracket: "\"CheckL\"", name: "CheckL", value: demoDefinition.CheckL)
                                    .Add(columnBracket: "\"CheckM\"", name: "CheckM", value: demoDefinition.CheckM)
                                    .Add(columnBracket: "\"CheckN\"", name: "CheckN", value: demoDefinition.CheckN)
                                    .Add(columnBracket: "\"CheckO\"", name: "CheckO", value: demoDefinition.CheckO)
                                    .Add(columnBracket: "\"CheckP\"", name: "CheckP", value: demoDefinition.CheckP)
                                    .Add(columnBracket: "\"CheckQ\"", name: "CheckQ", value: demoDefinition.CheckQ)
                                    .Add(columnBracket: "\"CheckR\"", name: "CheckR", value: demoDefinition.CheckR)
                                    .Add(columnBracket: "\"CheckS\"", name: "CheckS", value: demoDefinition.CheckS)
                                    .Add(columnBracket: "\"CheckT\"", name: "CheckT", value: demoDefinition.CheckT)
                                    .Add(columnBracket: "\"CheckU\"", name: "CheckU", value: demoDefinition.CheckU)
                                    .Add(columnBracket: "\"CheckV\"", name: "CheckV", value: demoDefinition.CheckV)
                                    .Add(columnBracket: "\"CheckW\"", name: "CheckW", value: demoDefinition.CheckW)
                                    .Add(columnBracket: "\"CheckX\"", name: "CheckX", value: demoDefinition.CheckX)
                                    .Add(columnBracket: "\"CheckY\"", name: "CheckY", value: demoDefinition.CheckY)
                                    .Add(columnBracket: "\"CheckZ\"", name: "CheckZ", value: demoDefinition.CheckZ)
                                    .Comments(Comments(
                                        context: context,      
                                        demoModel: demoModel,
                                        idHash: idHash,
                                        parentId: demoDefinition.Id))
                                    .Creator(creator)
                                    .Updator(updator)
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(
                                        context: context,
                                        demoModel: demoModel)),
                                addUpdatorParam: false)
                        }).Id.ToLong();
                    idHash.Add(demoDefinition.Id, resultId);
                    var siteModel = new SiteModel().Get(
                        context: context,
                        where: Rds.SitesWhere().SiteId(idHash.Get(demoDefinition.ParentId)));
                    var ss = siteModel.ResultsSiteSettings(
                        context: context, referenceId: resultId);
                    var resultModel = new ResultModel(
                        context: context,
                        ss: ss,
                        resultId: resultId);
                    var fullText = resultModel.FullText(context: context, ss: ss);
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(resultModel.SiteId)
                                .Title(resultModel.Title.DisplayValue)
                                .FullText(fullText, _using: fullText != null),
                            where: Rds.ItemsWhere().ReferenceId(resultModel.ResultId),
                            addUpdatorParam: false,
                            addUpdatedTimeParam: false));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeLinks(
            Context context, DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Sites")
                .Where(o => o.ClassB.Trim() != string.Empty)
                .ForEach(demoDefinition =>
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash.Get(demoDefinition.ClassB))
                            .SourceId(idHash.Get(demoDefinition.Id)))));
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Sites")
                .Where(o => o.ClassC.Trim() != string.Empty)
                .ForEach(demoDefinition =>
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash.Get(demoDefinition.ClassC))
                            .SourceId(idHash.Get(demoDefinition.Id)))));
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.ClassA.RegexExists("^#[A-Za-z0-9_]+?#$"))
                .ForEach(demoDefinition =>
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash.Get(demoDefinition.ClassA
                                .Substring(1, demoDefinition.ClassA.Length - 2)))
                            .SourceId(idHash.Get(demoDefinition.Id)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializePermissions(Context context, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Sites")
                .Where(o => o.ParentId == string.Empty)
                .Select(o => o.Id)
                .ForEach(id =>
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.InsertPermissions(
                        param: Rds.PermissionsParam()
                            .ReferenceId(idHash.Get(id))
                            .DeptId(0)
                            .UserId(idHash.Get(FirstUser(context: context)))
                            .PermissionType(Permissions.Manager())));
                idHash.Where(o => o.Key.StartsWith("Dept")).Select(o => o.Value).ForEach(deptId =>
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.InsertPermissions(
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
            Context context,
            DemoModel demoModel,
            Dictionary<string, long> idHash,
            string parentId)
        {
            var comments = new Comments();
            Def.DemoDefinitionCollection
                .Where(o => o.Language == context.Language)
                .Where(o => o.Type == "Comments")
                .Where(o => o.ParentId == parentId)
                .Select((o, i) => new { DemoDefinition = o, Index = i })
                .ForEach(data =>
                    comments.Add(new Comment
                    {
                        CommentId = data.Index + 1,
                        CreatedTime = data.DemoDefinition.CreatedTime.DemoTime(
                            context: context,
                            demoModel: demoModel),
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
            foreach (var id in self.RegexValues("#[A-Za-z0-9_]+?#").Distinct())
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
        private static DateTime DemoTime(this DateTime self, Context context, DemoModel demoModel)
        {
            return self.AddDays(demoModel.TimeLag).ToUniversal(context: context);
        }
    }
}
