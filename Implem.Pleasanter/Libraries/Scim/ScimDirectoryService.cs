using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using GroupCollection = Implem.Pleasanter.Models.GroupCollection;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        internal const int DefaultCount = 100;
        internal const int MaxCount = 200;
        private static readonly Regex EqFilterRegex = new(
            pattern: @"^\s*(?<path>.+)\s+eq\s+""(?<value>[^""]*)""\s*$",
            options: RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex MemberPathRegex = new(
            pattern: @"members\s*\[\s*value\s+eq\s+""(?<value>[^""]+)""\s*\]",
            options: RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly string[] SupportedPatchOps = ["add", "remove", "replace"];

        public ScimServiceResult ListUsers(
            Context context,
            int startIndex,
            int count,
            string filter)
        {
            startIndex = Math.Max(startIndex, 1);
            count = Math.Clamp(count, 0, MaxCount);
            var filterError = TryUserFilterWhere(context, filter, out var where);
            if (filterError != null)
            {
                return filterError;
            }
            var totalResults = Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: where));
            var users = count == 0
                ? []
                : new UserCollection(
                    context: context,
                    ss: null,
                    column: UserColumns(),
                    where: where,
                    orderBy: Rds.UsersOrderBy().UserId(),
                    offset: startIndex - 1,
                    pageSize: count)
                        .Select(row => ToUser(context, row))
                        .ToList();
            return Success(new ScimListResponse
            {
                TotalResults = totalResults,
                Resources = users,
                StartIndex = startIndex,
                ItemsPerPage = users.Count
            });
        }

        public ScimServiceResult GetUser(Context context, string id)
        {
            var model = FindUserModelByScimId(context, id);
            return model == null
                ? Error(404, "User not found")
                : Success(ToUser(context, model));
        }

        public ScimServiceResult CreateUser(Context context, ScimUser request)
        {
            var validationError = ValidateUser(request);
            if (validationError != null)
            {
                return validationError;
            }
            if (request.ExternalId.IsNullOrEmpty())
            {
                return Error(StatusCodes.Status400BadRequest, "externalId is required", "invalidValue");
            }
            var existing = FindUserModelByExternalId(context, request.ExternalId);
            if (existing != null && !existing.ScimId.IsNullOrEmpty())
            {
                return Error(StatusCodes.Status409Conflict, "User already exists", "uniqueness");
            }
            var loginConflict = LoginIdConflict(context, UserLoginId(request), existing?.UserId ?? 0);
            if (loginConflict != null)
            {
                return loginConflict;
            }

            var existingScimId = existing?.ScimId;
            var scimId = existingScimId.IsNullOrEmpty()
                ? Guid.NewGuid().ToString("D")
                : existingScimId;
            var saved = SaveUser(context, scimId, request, existing?.UserId ?? 0);
            if (saved.StatusCode >= StatusCodes.Status400BadRequest)
            {
                return saved;
            }
            saved.StatusCode = existing == null ? StatusCodes.Status201Created : StatusCodes.Status200OK;
            return saved;
        }

        public ScimServiceResult ReplaceUser(
            Context context,
            string id,
            ScimUser request)
        {
            var validationError = ValidateUser(request);
            if (validationError != null)
            {
                return validationError;
            }
            var target = FindUserModelByScimId(context, id);
            if (target == null)
            {
                return Error(StatusCodes.Status404NotFound, "User not found");
            }
            var conflict = LoginIdConflict(context, UserLoginId(request), target.UserId);
            if (conflict != null)
            {
                return conflict;
            }
            return SaveUser(context, id, request);
        }

        public ScimServiceResult PatchUser(
            Context context,
            string id,
            ScimPatchRequest request)
        {
            var row = FindUserModelByScimId(context, id);
            if (row == null)
            {
                return Error(StatusCodes.Status404NotFound, "User not found");
            }
            var user = ToUser(context, row);
            var patchValidationError = ValidatePatchOperations(request);
            if (patchValidationError != null)
            {
                return patchValidationError;
            }
            foreach (var operation in request.Operations)
            {
                var op = operation.Op.ToLowerInvariant();
                var path = NormalizePatchPath(operation.Path);
                if (path.IsNullOrEmpty()
                    && operation.Value is JObject obj)
                {
                    ApplyUserObject(user, obj);
                    continue;
                }
                ApplyUserPatch(user, op, path, operation.Value);
            }
            var validationError = ValidateUser(user);
            if (validationError != null)
            {
                return validationError;
            }
            var conflict = LoginIdConflict(context, UserLoginId(user), row.UserId);
            if (conflict != null)
            {
                return conflict;
            }
            return SaveUser(context, id, user);
        }

        public ScimServiceResult DeleteUser(Context context, string id)
        {
            var row = FindUserModelByScimId(context, id);
            if (row == null)
            {
                return Error(StatusCodes.Status404NotFound, "User not found");
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements:
                [
                    Rds.UpdateUsers(
                        param: Rds.UsersParam()
                            .Disabled(true)
                            .SynchronizedTime(DateTime.Now)
                            .ScimSync(true),
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .UserId(row.UserId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.UsersUpdated)
                ]);
            return new ScimServiceResult { StatusCode = StatusCodes.Status204NoContent };
        }

        private static ScimServiceResult LoginIdConflict(
            Context context,
            string loginId,
            int excludeUserId)
        {
            var existing = FindUserModelByLoginId(context, loginId);
            return existing != null && existing.UserId != excludeUserId
                ? Error(StatusCodes.Status409Conflict, "User already exists", "uniqueness")
                : null;
        }

        public ScimServiceResult ListGroups(
            Context context,
            int startIndex,
            int count,
            string filter,
            string excludedAttributes = null)
        {
            startIndex = Math.Max(startIndex, 1);
            count = Math.Clamp(count, 0, MaxCount);
            var filterError = TryGroupFilterWhere(context, filter, out var where);
            if (filterError != null)
            {
                return filterError;
            }
            var includeMembers = IncludeMembers(excludedAttributes);
            var totalResults = Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectGroups(
                    column: Rds.GroupsColumn().GroupsCount(),
                    where: where));
            var groups = count == 0
                ? []
                : new GroupCollection(
                    context: context,
                    ss: null,
                    column: GroupColumns(),
                        where: where,
                        orderBy: Rds.GroupsOrderBy().GroupId(),
                        offset: startIndex - 1,
                        pageSize: count)
                            .Select(model => ToGroup(context, model, includeMembers: includeMembers))
                            .ToList();
            return Success(new ScimListResponse
            {
                TotalResults = totalResults,
                Resources = groups,
                StartIndex = startIndex,
                ItemsPerPage = groups.Count
            });
        }

        public ScimServiceResult GetGroup(
            Context context,
            string id,
            string excludedAttributes = null)
        {
            var row = FindGroupModelByScimId(context, id);
            return row == null
                ? Error(404, "Group not found")
                : Success(ToGroup(context, row, includeMembers: IncludeMembers(excludedAttributes)));
        }

        public ScimServiceResult CreateGroup(Context context, ScimGroup request)
        {
            var validationError = ValidateGroup(request);
            if (validationError != null)
            {
                return validationError;
            }
            if (request.ExternalId.IsNullOrEmpty())
            {
                return Error(StatusCodes.Status400BadRequest, "externalId is required", "invalidValue");
            }
            var existing = FindGroupModelByExternalId(context, request.ExternalId);
            if (existing != null
                && !existing.ScimId.IsNullOrEmpty()
                && existing.Disabled == false)
            {
                return Error(StatusCodes.Status409Conflict, "Group already exists", "uniqueness");
            }
            var existingScimId = existing?.ScimId;
            var scimId = existingScimId.IsNullOrEmpty()
                ? Guid.NewGuid().ToString("D")
                : existingScimId;
            var saved = SaveGroup(context, scimId, request, updateMembers: true, existingGroupId: existing?.GroupId ?? 0);
            if (saved.StatusCode >= StatusCodes.Status400BadRequest)
            {
                return saved;
            }
            saved.StatusCode = existing == null ? StatusCodes.Status201Created : StatusCodes.Status200OK;
            return saved;
        }

        public ScimServiceResult ReplaceGroup(
            Context context,
            string id,
            ScimGroup request)
        {
            var validationError = ValidateGroup(request);
            if (validationError != null)
            {
                return validationError;
            }
            if (FindGroupModelByScimId(context, id) == null)
            {
                return Error(StatusCodes.Status404NotFound, "Group not found");
            }
            return SaveGroup(context, id, request, updateMembers: true);
        }

        public ScimServiceResult PatchGroup(
            Context context,
            string id,
            ScimPatchRequest request)
        {
            var row = FindGroupModelByScimId(context, id);
            if (row == null)
            {
                return Error(StatusCodes.Status404NotFound, "Group not found");
            }
            var group = ToGroup(context, row, includeMembers: true);
            var updateMembers = false;
            var memberPatches = new List<GroupMemberPatch>();
            var patchValidationError = ValidatePatchOperations(request);
            if (patchValidationError != null)
            {
                return patchValidationError;
            }
            foreach (var operation in request.Operations)
            {
                var op = operation.Op.ToLowerInvariant();
                var path = NormalizePatchPath(operation.Path);
                if (path.IsNullOrEmpty()
                    && operation.Value is JObject obj)
                {
                    ApplyGroupObject(group, obj);
                    updateMembers = updateMembers || obj["members"] != null;
                    continue;
                }
                updateMembers = ApplyGroupPatch(group, op, path, operation.Path, operation.Value, memberPatches)
                    || updateMembers;
            }
            var validationError = ValidateGroup(group);
            if (validationError != null)
            {
                return validationError;
            }
            var saved = SaveGroup(context, id, group, updateMembers: updateMembers);
            if (saved.StatusCode >= StatusCodes.Status400BadRequest)
            {
                return saved;
            }
            if (!updateMembers)
            {
                var memberPatchResult = ApplyGroupMemberPatches(context, row.GroupId, memberPatches);
                if (memberPatchResult != null)
                {
                    return memberPatchResult;
                }
            }
            return new ScimServiceResult { StatusCode = StatusCodes.Status204NoContent };
        }

        private static ScimServiceResult ValidatePatchOperations(ScimPatchRequest request)
        {
            if (request?.Operations == null || request.Operations.Count == 0)
            {
                return Error(
                    StatusCodes.Status400BadRequest,
                    "Patch operations are required",
                    "invalidSyntax");
            }
            foreach (var operation in request.Operations)
            {
                if (operation == null || string.IsNullOrWhiteSpace(operation.Op))
                {
                    return Error(
                        StatusCodes.Status400BadRequest,
                        "Patch operation is required",
                        "invalidSyntax");
                }
                if (!SupportedPatchOps.Contains(operation.Op.ToLowerInvariant()))
                {
                    return Error(
                        StatusCodes.Status400BadRequest,
                        $"Unsupported patch operation: {operation.Op}",
                        "invalidSyntax");
                }
            }
            return null;
        }

        public ScimServiceResult DeleteGroup(Context context, string id)
        {
            var row = FindGroupModelByScimId(context, id);
            if (row == null)
            {
                return Error(StatusCodes.Status404NotFound, "Group not found");
            }
            var groupId = row.GroupId;
            var param = Rds.GroupsParam()
                .Disabled(true)
                .SynchronizedTime(DateTime.Now)
                .ScimSync(true);
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements:
                [
                    Rds.UpdateGroups(
                        param: param,
                        where: Rds.GroupsWhere()
                            .TenantId(context.TenantId)
                            .GroupId(groupId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.GroupsUpdated)
                ]);
            GroupMemberUtilities.SyncGroupMembers(context, groupId);
            return new ScimServiceResult { StatusCode = StatusCodes.Status204NoContent };
        }

        private static ScimServiceResult SaveUser(
            Context context,
            string scimId,
            ScimUser user,
            int existingUserId = 0)
        {
            var loginId = UserLoginId(user);
            var name = UserDisplayName(user);
            if (name.IsNullOrEmpty())
            {
                name = loginId;
            }
            var language = ResolveLanguage(user);
            if (language.IsNullOrEmpty())
            {
                language = Parameters.Service.DefaultLanguage;
            }
            var timeZone = user.Timezone;
            if (timeZone.IsNullOrEmpty())
            {
                timeZone = Parameters.Service.TimeZoneDefault;
            }
            var department = UserDepartment(user);
            var setDepartment = !department.IsNullOrEmpty();
            var clearDepartment = department == string.Empty;
            var userCode = UserCode(user);
            var manager = ResolveManager(
                context: context,
                user: user,
                scimId: scimId,
                existingUserId: existingUserId);
            var now = DateTime.Now;
            var param = Rds.UsersParam()
                .TenantId(context.TenantId)
                .LoginId(loginId.MaxLength(256))
                .Name(name.MaxLength(128))
                .UserCode(
                    userCode.MaxLength(32),
                    _using: userCode != null)
                .FirstName((user.Name?.GivenName ?? string.Empty).MaxLength(32))
                .LastName((user.Name?.FamilyName ?? string.Empty).MaxLength(32))
                .Language(
                    language,
                    _using: !language.IsNullOrEmpty())
                .TimeZone(
                    timeZone,
                    _using: !timeZone.IsNullOrEmpty())
                .Manager(
                    manager.UserId ?? 0,
                    _using: manager.Specified && (manager.Clear || manager.UserId.HasValue))
                .Disabled(user.Active == false)
                .SynchronizedTime(now)
                .ScimId(scimId)
                .ScimExternalId(user.ExternalId.MaxLength(256))
                .ScimSync(true);
            if (setDepartment)
            {
                param.DeptId(sub: Rds.SelectDepts(
                    column: Rds.DeptsColumn().DeptId(),
                    where: Rds.DeptsWhere()
                        .TenantId(context.TenantId)
                        .DeptCode(department)));
            }
            else if (clearDepartment)
            {
                param.DeptId(0);
            }
            AddExtendedAttributeParams(
                param: param,
                extensionData: ExtensionData(user),
                attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                tableName: ScimExtendedAttributeUtilities.UsersTableName);
            var statements = new List<SqlStatement>();
            if (setDepartment)
            {
                statements.Add(Rds.UpdateOrInsertDepts(
                    param: Rds.DeptsParam()
                        .TenantId(context.TenantId)
                        .DeptCode(department)
                        .DeptName(department),
                    where: Rds.DeptsWhere()
                        .TenantId(context.TenantId)
                        .DeptCode(department)));
            }
            statements.Add(Rds.UpdateOrInsertUsers(
                param: param,
                where: existingUserId > 0
                    ? Rds.UsersWhere()
                        .TenantId(context.TenantId)
                        .UserId(existingUserId)
                    : ScimUserWhere(context, scimId),
                addUpdatorParam: true,
                addUpdatedTimeParam: true));
            if (setDepartment)
            {
                statements.Add(StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.DeptsUpdated));
            }
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: context.TenantId,
                type: StatusUtilities.Types.UsersUpdated));
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: [.. statements]);
            var model = FindUserModelByScimId(context, scimId);
            if (model == null)
            {
                return Error(StatusCodes.Status500InternalServerError, "Failed to save user");
            }
            UpdateMailAddresses(context, model.UserId, user.Emails);
            return Success(ToUser(context, model));
        }

        private static ScimServiceResult SaveGroup(
            Context context,
            string scimId,
            ScimGroup group,
            bool updateMembers,
            int existingGroupId = 0)
        {
            var now = DateTime.Now;
            var param = Rds.GroupsParam()
                .TenantId(context.TenantId)
                .GroupName(group.DisplayName.MaxLength(256))
                .Disabled(false)
                .SynchronizedTime(now)
                .ScimId(scimId)
                .ScimExternalId(group.ExternalId.MaxLength(256))
                .ScimSync(true);
            AddExtendedAttributeParams(
                param: param,
                extensionData: ExtensionData(group),
                attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                tableName: ScimExtendedAttributeUtilities.GroupsTableName);
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements:
                [
                    Rds.UpdateOrInsertGroups(
                        param: param,
                        where: existingGroupId > 0
                            ? Rds.GroupsWhere()
                                .TenantId(context.TenantId)
                                .GroupId(existingGroupId)
                            : ScimGroupWhere(context, scimId),
                        addUpdatorParam: true,
                        addUpdatedTimeParam: true),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.GroupsUpdated)
                ]);
            var row = FindGroupModelByScimId(context, scimId);
            if (row == null)
            {
                return Error(StatusCodes.Status500InternalServerError, "Failed to save group");
            }
            var groupId = row.GroupId;
            if (updateMembers)
            {
                var memberResult = UpdateGroupMembers(context, groupId, group.Members);
                if (memberResult != null)
                {
                    return memberResult;
                }
            }
            return Success(ToGroup(context, row, includeMembers: true));
        }

        private static ScimServiceResult Success(object body)
        {
            return new ScimServiceResult
            {
                StatusCode = StatusCodes.Status200OK,
                Body = body
            };
        }

        private static ScimServiceResult Error(
            int statusCode,
            string detail,
            string scimType = null)
        {
            return new ScimServiceResult
            {
                StatusCode = statusCode,
                Body = new ScimError
                {
                    Status = statusCode.ToString(),
                    ScimType = scimType,
                    Detail = detail
                }
            };
        }
    }
}