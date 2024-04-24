#pragma warning disable CA1416
using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using static Implem.Pleasanter.Models.SysLogModel;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class LdapDs
    {
        public static bool Authenticate(Context context, string loginId, string password)
        {
            foreach (var ldap in Parameters.Authentication.LdapParameters)
            {
                DirectorySearcher searcher;
                try
                {
                    searcher = DirectorySearcher(
                        loginId: ldap.LdapLoginPattern != null
                            ? ldap.LdapLoginPattern.Replace("{loginId}", loginId)
                            : loginId,
                        password: password,
                        ldap: ldap);
                }
                catch (DirectoryServicesCOMException e)
                {
                    new SysLogModel(
                        context: context,
                        extendedErrorMessage: e.ExtendedErrorMessage,
                        e: e);
                    return false;
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                    return false;
                }
                SearchResult result = null;
                searcher.Filter = ldap.LdapSearchPattern != null
                    ? ldap.LdapSearchPattern.Replace("{loginId}", loginId)
                    : $"({ldap.LdapSearchProperty}={loginId})";
                try
                {
                    result = searcher.FindOne();
                }
                catch (DirectoryServicesCOMException e)
                {
                    if (e.ExtendedErrorMessage?.Contains("data 52e") != true)
                    {
                        new SysLogModel(
                            context: context,
                            extendedErrorMessage: e.ExtendedErrorMessage,
                            e: e);
                    }
                    return false;
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                    return false;
                }
                if (result != null)
                {
                    UpdateOrInsert(
                        context: context,
                        result: result,
                        ldap: ldap,
                        synchronizedTime: DateTime.Now);
                    return true;
                }
            }
            return false;
        }

        public static void UpdateOrInsert(Context context, string loginId)
        {
            foreach (var ldap in Parameters.Authentication.LdapParameters)
            {
                var root = new DirectoryEntry(ldap.LdapSearchRoot);
                var searcher = new DirectorySearcher(root);
                searcher.Filter = ldap.LdapSearchPattern != null
                    ? ldap.LdapSearchPattern.Replace("{loginId}", loginId)
                    : $"({ldap.LdapSearchProperty}={loginId.Split_2nd('\\')})";
                try
                {
                    SetPropertiesToLoad(searcher, ldap);
                    SearchResult result = searcher.FindOne();
                    UpdateOrInsert(
                        context: context,
                        result: result,
                        ldap: ldap,
                        synchronizedTime: DateTime.Now);
                }
                catch (DirectoryServicesCOMException e)
                {
                    new SysLogModel(
                        context: context,
                        extendedErrorMessage: e.ExtendedErrorMessage,
                        e: e);
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                }
            }
        }

        private static void UpdateOrInsert(
            Context context,
            SearchResult result,
            ParameterAccessor.Parts.Ldap ldap,
            DateTime synchronizedTime)
        {
            var deptCode = result.Property(
                context: context,
                name: ldap.LdapDeptCode,
                pattern: ldap.LdapDeptCodePattern);
            var deptName = result.Property(
                context: context,
                name: ldap.LdapDeptName,
                pattern: ldap.LdapDeptNamePattern);
            var deptExists = !deptCode.IsNullOrEmpty() && !deptName.IsNullOrEmpty();
            var deptSettings = !ldap.LdapDeptCode.IsNullOrEmpty() && !ldap.LdapDeptName.IsNullOrEmpty();
            var userCode = result.Property(
                context: context,
                name: ldap.LdapUserCode,
                pattern: ldap.LdapUserCodePattern);
            var loginId = LoginId(
                context: context,
                ldap: ldap,
                result: result);
            var name = Name(
                context: context,
                loginId: loginId,
                result: result,
                ldap: ldap);
            var mailAddress = result.Property(
                context: context,
                name: ldap.LdapMailAddress,
                pattern: ldap.LdapMailAddressPattern);
            var statements = new List<SqlStatement>();
            if (deptExists)
            {
                statements.Add(Rds.UpdateOrInsertDepts(
                    param: Rds.DeptsParam()
                        .TenantId(ldap.LdapTenantId)
                        .DeptCode(deptCode)
                        .DeptName(deptName),
                    where: Rds.DeptsWhere().DeptCode(deptCode)));
            }
            var exists = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: Rds.UsersWhere().LoginId(
                        value: context.Sqls.EscapeValue(loginId),
                        _operator: context.Sqls.LikeWithEscape))) == 1;
            var param = Rds.UsersParam()
                .TenantId(ldap.LdapTenantId)
                .LoginId(loginId, _using: !exists)
                .UserCode(userCode)
                .Name(name)
                .DeptId(
                    sub: Rds.SelectDepts(
                        column: Rds.DeptsColumn().DeptId(),
                        where: Rds.DeptsWhere().DeptCode(deptCode)),
                    _using: deptExists)
                .DeptId(0, _using: deptSettings && !deptExists)
                .LdapSearchRoot(ldap.LdapSearchRoot)
                .SynchronizedTime(synchronizedTime);
            ldap.LdapExtendedAttributes?.ForEach(attribute =>
                param.Add(
                    $"\"{attribute.ColumnName}\"",
                    attribute.ColumnName,
                    result.Property(
                        context: context,
                        name: attribute.Name,
                        pattern: attribute.Pattern)));
            statements.Add(Rds.UpdateOrInsertUsers(
                param: param,
                where: Rds.UsersWhere().LoginId(
                    value: context.Sqls.EscapeValue(loginId),
                    _operator: context.Sqls.LikeWithEscape),
                addUpdatorParam: false,
                addUpdatedTimeParam: false));
            if (!mailAddress.IsNullOrEmpty())
            {
                statements.Add(Rds.PhysicalDeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerType("Users")
                        .OwnerId(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere().LoginId(
                                value: context.Sqls.EscapeValue(loginId),
                                _operator: context.Sqls.LikeWithEscape)))));
                statements.Add(Rds.InsertMailAddresses(
                    param: Rds.MailAddressesParam()
                        .OwnerId(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere().LoginId(
                                value: context.Sqls.EscapeValue(loginId),
                                _operator: context.Sqls.LikeWithEscape)))
                        .OwnerType("Users")
                        .MailAddress(mailAddress)));
            }
            var ss = new SiteSettings();
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: ldap.LdapTenantId, type: StatusUtilities.Types.DeptsUpdated));
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: ldap.LdapTenantId, type: StatusUtilities.Types.UsersUpdated));
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: ldap.LdapTenantId, type: StatusUtilities.Types.GroupsUpdated));
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
        }

        private class GroupItem
        {
            public string DisplayName;
            public string ADsPath;
            public string LdapObjectGUID;
            public Implem.ParameterAccessor.Parts.Ldap Ldap;
            public string Pattern;
            public int GraphIdx;
        }

        public static void Sync(Context context)
        {
            // "DateTime.Now"はミリ秒が6桁まで取れる
            // Usersテーブルの"SynchronizedTime"はミリ秒が3桁まで格納されている
            // "DateTime.Now"のものでレコードの絞り込み(where)を行うと条件に合致しないためミリ秒を3桁に揃える必要がある
            var now = DateTime.Now;
            var synchronizedTime = new DateTime(
                year: now.Year,
                month: now.Month,
                day: now.Day,
                hour: now.Hour,
                minute: now.Minute,
                second: now.Second,
                millisecond: now.Millisecond);
            var users = new Dictionary<string,string>();
            var groups = new Dictionary<string, GroupItem>();
            Parameters.Authentication.LdapParameters
                .ForEach(ldap =>
                {
                    ldap.LdapSyncPatterns?
                        .ForEach(pattern =>
                        {
                            // Uers用Ldap同期処理
                            // 既存の振る舞いを変えたくない為にLdapSyncPatternsとLdapSyncGroupPatternsに分け、ここではユーザのみ取得する。
                            Sync(
                                context: context,
                                ldap: ldap,
                                pattern: pattern,
                                synchronizedTime: synchronizedTime,
                                users: users);
                            if (ldap.AutoDisable)
                            {
                                Repository.ExecuteNonQuery(
                                    context: context,
                                    statements: Rds.UpdateUsers(
                                        param: Rds.UsersParam().Disabled(true),
                                        where: Rds.UsersWhere()
                                            .Disabled(false)
                                            .LdapSearchRoot(ldap.LdapSearchRoot)
                                            .SynchronizedTime(_operator: " is not null")
                                            .SynchronizedTime(synchronizedTime, _operator: "<>"),
                                        addUpdatorParam: false,
                                        addUpdatedTimeParam: false));
                            }
                            if (ldap.AutoEnable)
                            {
                                Repository.ExecuteNonQuery(
                                    context: context,
                                    statements: Rds.UpdateUsers(
                                        param: Rds.UsersParam().Disabled(false),
                                        where: Rds.UsersWhere()
                                            .Disabled(true)
                                            .LdapSearchRoot(ldap.LdapSearchRoot)
                                            .SynchronizedTime(_operator: " is not null")
                                            .SynchronizedTime(synchronizedTime),
                                        addUpdatorParam: false,
                                        addUpdatedTimeParam: false));
                            }
                        });
                    ldap.LdapSyncGroupPatterns?
                        .ForEach(pattern =>
                        {
                            // Group用Ldap同期処理
                            Sync(
                                context: context,
                                ldap: ldap,
                                pattern: pattern,
                                synchronizedTime: synchronizedTime,
                                groups: groups);
                        });
                });
            UpdateGroup(
                context: context,
                groups: groups,
                users: users,
                synchronizedTime: synchronizedTime);
        }

        private static void Sync(
            Context context,
            ParameterAccessor.Parts.Ldap ldap,
            string pattern,
            DateTime synchronizedTime,
            Dictionary<string, string> users = null,
            Dictionary<string, GroupItem> groups = null)
        {
            var logs = new Logs()
            {
                new Log("pattern", pattern)
            };
            try
            {
                var directorySearcher = DirectorySearcher(
                    ldap.LdapSyncUser,
                    ldap.LdapSyncPassword,
                    ldap);
                directorySearcher.Filter = pattern;
                if (ldap.LdapSyncPageSize == 0)
                {
                    directorySearcher.PageSize = 1000;
                }
                else if (ldap.LdapSyncPageSize > 0)
                {
                    directorySearcher.PageSize = ldap.LdapSyncPageSize;
                }
                SetPropertiesToLoad(directorySearcher, ldap);
                var results = directorySearcher.FindAll();
                logs.Add("results", results.Count.ToString());
                foreach (SearchResult result in results)
                {
                    if (users != null)
                    {
                        // LdapSyncPatterns用
                        if (Enabled(result, ldap))
                        {
                            logs.Add("result", result.Path);
                            UpdateOrInsert(
                                context: context,
                                result: result,
                                ldap: ldap,
                                synchronizedTime: synchronizedTime);
                            if (!result.IsGroup(context: context) && !users.ContainsKey(result.Path))
                            {
                                users.Add(
                                    result.Path,
                                    LoginId(context: context, ldap: ldap, result: result));
                            }
                        }
                    }
                    else
                    {
                        // LdapSyncGroupPatterns用
                        if (result.IsGroup(context: context))
                        {
                            var groupItem = NewGroupItem(
                                context: context,
                                result: result,
                                ldap: ldap,
                                pattern: pattern,
                                graphIdx: groups.Count);
                            if (groupItem != null && !groups.ContainsKey(groupItem.ADsPath))
                            {
                                groups.Add(groupItem.ADsPath, groupItem);
                            }
                        }
                    }
                }
            }
            catch (DirectoryServicesCOMException e)
            {
                new SysLogModel(
                    context: context,
                    extendedErrorMessage: e.ExtendedErrorMessage,
                    e: e);
            }
            catch (Exception e)
            {
                new SysLogModel(context: context, e: e, logs: logs);
            }
        }

        private static GroupItem NewGroupItem(
            Context context,
            SearchResult result,
            ParameterAccessor.Parts.Ldap ldap,
            string pattern,
            int graphIdx)
        {
            var displayName = result.Property(context: context, name: ldap.LdapGroupName, pattern: ldap.LdapGroupNamePattern);
            if (displayName.IsNullOrEmpty()) return null;
            return new GroupItem()
            {
                DisplayName = displayName,
                ADsPath = result.Path,
                LdapObjectGUID = result.PropertyGUID(context: context, name: "objectGUID"),
                Ldap = ldap,
                Pattern = pattern,
                GraphIdx = graphIdx
            };
        }

        private static void UpdateGroup(
            Context context,
            Dictionary<string ,GroupItem> groups,
            Dictionary<string, string> users,
            DateTime synchronizedTime)
        {
            var logs = new Logs()
            {
                new Log("SyncGroup", "")
            };
            try
            {
                var statements = new List<SqlStatement>();
                var groupGraph = new Dictionary<int, int[]>();
                // グループ作成
                groups.Values
                    .ForEach(group =>
                    {
                        statements.Add(Rds.UpdateOrInsertGroups(
                            param: Rds.GroupsParam()
                                .TenantId(group.Ldap.LdapTenantId)
                                .GroupName(group.DisplayName)
                                .Disabled(false)
                                .LdapSync(true)
                                .LdapGuid(group.LdapObjectGUID)
                                .LdapSearchRoot(group.Ldap.LdapSearchRoot)
                                .SynchronizedTime(synchronizedTime),
                            where: Rds.GroupsWhere().LdapGuid(group.LdapObjectGUID)));
                    });
                // 以前AD連携された 子グループ削除＆グループメンバー削除
                var groupids = Rds.SelectGroups(
                    column: Rds.GroupsColumn().GroupId(),
                    where: Rds.GroupsWhere()
                        .SynchronizedTime(_operator: " is not null")
                        .SynchronizedTime(value: synchronizedTime));
                statements.Add(Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId_In(
                            sub: groupids)));
                statements.Add(Rds.PhysicalDeleteGroupChildren(
                    where: Rds.GroupChildrenWhere()
                        .GroupId_In(
                            sub: groupids)));
                // 削除されたグループのDisableをOnにする。
                statements.Add(new SqlStatement(
                        commandText: Def.Sql.AdGroupDeleteToDisable,
                        param: new SqlParamCollection {
                            { "SynchronizedTime", synchronizedTime }
                        }));
                // 子グループ追加＆グループメンバー追加
                groups.Values
                    .ForEach(groupItem =>
                    {
                        var directorySearcher = DirectorySearcher(
                            loginId: groupItem.Ldap.LdapSyncUser,
                            password: groupItem.Ldap.LdapSyncPassword,
                            ldap: groupItem.Ldap,
                            ldapSearchRoot: groupItem.ADsPath);
                        var ldapUrl = groupItem.ADsPath.Substring(0, groupItem.ADsPath.LastIndexOf('/') + 1);
                        var userLoginIds = new List<string>();
                        var groupGuids = new List<GroupItem>();
                        var groupGuidsAll = new List<GroupItem>();
                        var memberLow = 0;
                        var memberSize = 1000;
                        var firstTime = true;
                        var isLoop = true;
                        while (isLoop)
                        {
                            directorySearcher.PropertiesToLoad.Clear();
                            directorySearcher.PropertiesToLoad.Add($"member;range={memberLow}-{memberLow + memberSize - 1}");
                            var result = directorySearcher.FindOne();
                            logs.Add("result", result?.Path);
                            if (result == null) break;
                            isLoop = false;
                            foreach (string name in result.Properties.PropertyNames)
                            {
                                if (!name.StartsWith("member;range=")) continue;
                                foreach (string item in result.Properties[name])
                                {
                                    var key = ldapUrl + item;
                                    if (users.ContainsKey(key)) userLoginIds.Add(users[key]);
                                    if (groups.ContainsKey(key)) groupGuids.Add(groups[key]);
                                }
                                if (name.IndexOf("*") < 0)
                                {
                                    memberLow += memberSize;
                                    isLoop = true;
                                }
                            }
                            // メンバー全削除＆追加・子グループ全削除＆追加
                            statements.Add(new SqlStatement(
                                commandText: Def.Sql.LdapUpdateGroupMembersAndChildren
                                    .Replace("{{userLoginIds_condition}}",
                                        userLoginIds.Any()
                                            ? $" \"t3\".\"LoginId\" in ({userLoginIds.Select(s => $"'{s}'").Join()}) "
                                            : "(1=0)")
                                    .Replace("{{groupGuids_condition}}",
                                        groupGuids.Any()
                                            ? $" \"t5\".\"LdapGuid\" in ({groupGuids.Select(s => $"'{s.LdapObjectGUID}'").Join()}) "
                                            : "(1=0)"),
                                param: new SqlParamCollection {
                                    { "TenantId", groupItem.Ldap.LdapTenantId },
                                    { "LdapObjectGUID", groupItem.LdapObjectGUID },
                                    { "isFirstTime",  firstTime },
                                    { "isMemberInsert", userLoginIds.Any() },
                                    { "isChildInsert", groupGuids.Any() },
                                }));
                            firstTime = false;
                            groupGuidsAll.AddRange(groupGuids);
                            groupGuids.Clear();
                            userLoginIds.Clear();
                        }
                        groupGraph.Add(groupItem.GraphIdx, groupGuidsAll.Select(v => v.GraphIdx).ToArray());
                    });
                var checkCycle = CheckGroupChildCycle(graph: groupGraph, lvMax: Parameters.General.GroupsDepthMax);
                if (checkCycle < 0)
                {
                    new SysLogModel(
                        context: context,
                        method: "LdapSyncGroup",
                        message: checkCycle == -1
                            ? "Failed to import LDAP group.Groups in LDAP are circular references."
                            : "Failed to import LDAP group.LDAP groups are nested too deeply.",
                        sysLogType: SysLogTypes.UserError);
                    return;
                }
                // テナントの子グループユーザ再構成
                groups
                    .Select(group => group.Value.Ldap.LdapTenantId)
                    .Distinct()
                    .ForEach(tenantId => statements.Add(GroupMemberUtilities.RefreshAllChildMembers(tenantId)));
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: statements.ToArray());
            }
            catch (DirectoryServicesCOMException e)
            {
                new SysLogModel(
                    context: context,
                    extendedErrorMessage: e.ExtendedErrorMessage,
                    e: e);
            }
            catch (Exception e)
            {
                new SysLogModel(context: context, e: e, logs: logs);
            }
        }

        // グループの循環参照・ネストの深さチェック
        private static int CheckGroupChildCycle(Dictionary<int, int[]> graph, int lvMax)
        {
            var errCycle = -1;
            var errLvOver = -2;
            var lv = 0;
            foreach (var k in graph.Keys)
            {
                var paretIds = new Stack<int>();
                var ret = Scanning(graph, k, paretIds, 0, lvMax);
                if (ret < 0)
                {
                    lv = ret;
                    break;
                }
                else if (ret > lv)
                {
                    lv = ret;
                }
            }
            return lv;

            int Scanning(Dictionary<int, int[]> graph, int idx0, Stack<int> paretIds, int lv, int lvMax)
            {
                if (lv >= lvMax) return errLvOver;
                if (!graph.ContainsKey(idx0)) return lv;
                paretIds.Push(idx0);
                var max = lv;
                foreach (var idx1 in graph[idx0])
                {
                    if (paretIds.Contains(idx1)) return errCycle;
                    var val = Scanning(graph, idx1, paretIds, lv + 1, lvMax);
                    if (val < 0) return val;
                    if (val > max) max = val;
                }
                paretIds.Pop();
                return max;
            }
        }

        private static void SetPropertiesToLoad(DirectorySearcher directorySearcher, ParameterAccessor.Parts.Ldap ldap)
        {
            // ActiveDirectoryから取得するプロパティを指定。(指定しないと全部取得される為)
            var list = new List<string>
            {
                "ADsPath",
                "UserAccountControl",
                "sAMAccountName",
                "groupType",
                "objectGUID",
                ldap.LdapDeptCode,
                ldap.LdapDeptName,
                ldap.LdapUserCode,
                ldap.LdapMailAddress,
                ldap.LdapSearchProperty,
                ldap.LdapLastName,
                ldap.LdapFirstName,
                ldap.LdapGroupName,
                ldap.LdapGroupNamePattern,
            };
            ldap.LdapExtendedAttributes?.ForEach(attribute => list.Add(attribute.Name));
            directorySearcher.PropertiesToLoad.AddRange(list.Where(s => !s.IsNullOrEmpty()).ToArray());
        }

        private static bool Enabled(SearchResult result, ParameterAccessor.Parts.Ldap ldap)
        {
            var accountDisabled = 2;
            if (!ldap.LdapExcludeAccountDisabled)
            {
                return true;
            }
            if (result.Properties.Contains("UserAccountControl")
                && result.Properties["UserAccountControl"].Count > 0)
            {
                return (result.Properties["UserAccountControl"][0].ToLong() & accountDisabled) == 0;
            }
            return true;
        }

        private static DirectorySearcher DirectorySearcher(
            string loginId, string password, ParameterAccessor.Parts.Ldap ldap, string ldapSearchRoot = null)
        {
            if (!Enum.TryParse(ldap.LdapAuthenticationType, out AuthenticationTypes type)
                || !Enum.IsDefined(typeof(AuthenticationTypes), type))
            {
                type = AuthenticationTypes.Secure;
            }
            return new DirectorySearcher(loginId == null || password == null
                ? new DirectoryEntry(ldapSearchRoot ?? ldap.LdapSearchRoot)
                : new DirectoryEntry(ldapSearchRoot ?? ldap.LdapSearchRoot, loginId, password, type));
        }

        private static string Property(
            this SearchResult result, Context context, string name, string pattern = null)
        {
            if (!name.IsNullOrEmpty())
            {
                try
                {
                    if (result.Properties.Contains(name))
                    {
                        if (pattern.IsNullOrEmpty())
                        {
                            return result.Properties[name][0].ToString();
                        }
                        foreach (object obj in result.Properties[name])
                        {
                            if (obj.ToString().RegexExists(pattern))
                            {
                                return obj.ToString().RegexFirst(pattern);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    var logs = new Logs()
                    {
                        new Log("result", result.Path),
                        new Log("propertyName", name)
                    };
                    new SysLogModel(context: context, e: e, logs: logs);
                }
            }
            return string.Empty;
        }

        private static string PropertyGUID(
            this SearchResult result, Context context, string name)
        {
            return new Guid((Byte[])result.Properties[name][0]).ToString("N");
        }

        private static bool IsGroup(
            this SearchResult result, Context context)
        {
            return result.Properties.Contains("groupType");
        }

        private static string LoginId(
            Context context,
            ParameterAccessor.Parts.Ldap ldap,
            SearchResult result)
        {
            var loginId = Authentications.Windows(context: context)
                ? NetBiosName(
                    context: context,
                    result: result,
                    ldap: ldap)
                : result.Property(
                    context: context,
                    name: ldap.LdapSearchProperty);
            return loginId;
        }

        private static string Name(
            Context context,
            string loginId,
            SearchResult result,
            ParameterAccessor.Parts.Ldap ldap)
        {
            var name = "{0} {1}".Params(
                result.Property(
                    context: context,
                    name: ldap.LdapLastName,
                    pattern: ldap.LdapLastNamePattern),
                result.Property(
                    context: context,
                    name: ldap.LdapFirstName,
                    pattern: ldap.LdapFirstNamePattern));
            return name != " "
                ? name.Trim()
                : loginId;
        }

        public static string NetBiosName(
            Context context, SearchResult result, ParameterAccessor.Parts.Ldap ldap)
        {
            return ldap.NetBiosDomainName + "\\" + result.Property(
                context: context,
                name: "sAMAccountName");
        }
    }
}