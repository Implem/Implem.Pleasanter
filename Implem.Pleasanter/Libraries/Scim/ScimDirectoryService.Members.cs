using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Http;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static ScimServiceResult UpdateGroupMembers(
            Context context,
            int groupId,
            List<ScimMember> members)
        {
            var memberResult = ResolveGroupMembers(context, members, out var userIds, out var childGroupIds);
            if (memberResult != null)
            {
                return memberResult;
            }
            var cycleResult = ValidateGroupChildren(context, groupId, childGroupIds);
            if (cycleResult != null)
            {
                return cycleResult;
            }
            var statements = new List<SqlStatement>
            {
                Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(groupId)
                        .ChildGroup(false)
                        .DeptId(0)),
                Rds.PhysicalDeleteGroupChildren(
                    where: Rds.GroupChildrenWhere()
                        .GroupId(groupId))
            };
            userIds
                .Distinct()
                .ForEach(userId => statements.Add(Rds.InsertGroupMembers(
                    param: Rds.GroupMembersParam()
                        .GroupId(groupId)
                        .DeptId(0)
                        .UserId(userId)
                        .Admin(false))));
            childGroupIds
                .Distinct()
                .ForEach(childId => statements.Add(Rds.InsertGroupChildren(
                    param: Rds.GroupChildrenParam()
                        .GroupId(groupId)
                        .ChildId(childId))));
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: context.TenantId,
                type: StatusUtilities.Types.GroupsUpdated));
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: [.. statements]);
            GroupMemberUtilities.SyncGroupMembers(context, groupId);
            return null;
        }

        private static ScimServiceResult ApplyGroupMemberPatches(
            Context context,
            int groupId,
            List<GroupMemberPatch> patches)
        {
            if (patches.Count == 0)
            {
                return null;
            }
            foreach (var patch in patches)
            {
                if (patch.Operation == GroupMemberPatchOperation.Clear)
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements:
                        [
                            Rds.PhysicalDeleteGroupMembers(
                                where: Rds.GroupMembersWhere()
                                    .GroupId(groupId)
                                    .ChildGroup(false)
                                    .DeptId(0)),
                            Rds.PhysicalDeleteGroupChildren(
                                where: Rds.GroupChildrenWhere()
                                    .GroupId(groupId)),
                            StatusUtilities.UpdateStatus(
                                tenantId: context.TenantId,
                                type: StatusUtilities.Types.GroupsUpdated)
                        ]);
                    GroupMemberUtilities.SyncGroupMembers(context, groupId);
                    continue;
                }
                var memberResult = ResolveGroupMembers(context, patch.Members, out var userIds, out var childGroupIds);
                if (memberResult != null)
                {
                    return memberResult;
                }
                if (patch.Operation == GroupMemberPatchOperation.Add)
                {
                    var cycleResult = ValidateGroupChildren(context, groupId, childGroupIds);
                    if (cycleResult != null)
                    {
                        return cycleResult;
                    }
                    AddGroupMembers(context, groupId, userIds, childGroupIds);
                }
                else
                {
                    RemoveGroupMembers(context, groupId, userIds, childGroupIds);
                }
            }
            return null;
        }

        private static ScimServiceResult ResolveGroupMembers(
            Context context,
            List<ScimMember> members,
            out List<int> userIds,
            out List<int> childGroupIds)
        {
            members ??= [];
            userIds = [];
            childGroupIds = [];
            foreach (var member in members.Where(member => !member.Value.IsNullOrEmpty()))
            {
                switch (MemberType(member))
                {
                    case "user":
                        if (TryAddUserMember(context, member.Value, userIds))
                        {
                            continue;
                        }
                        return Error(StatusCodes.Status400BadRequest, $"User member not found: {member.Value}", "invalidValue");
                    case "group":
                        if (TryAddChildGroupMember(context, member.Value, childGroupIds))
                        {
                            continue;
                        }
                        return Error(StatusCodes.Status400BadRequest, $"Group member not found: {member.Value}", "invalidValue");
                    default:
                        if (TryAddUserMember(context, member.Value, userIds)
                            || TryAddChildGroupMember(context, member.Value, childGroupIds))
                        {
                            continue;
                        }
                        return Error(StatusCodes.Status400BadRequest, $"Member not found: {member.Value}", "invalidValue");
                }
            }
            return null;
        }

        private static ScimServiceResult ValidateGroupChildren(
            Context context,
            int groupId,
            List<int> childGroupIds)
        {
            var childStrings = childGroupIds
                .Distinct()
                .Select(childId => $"Group,{childId}")
                .ToList();
            var cycle = GroupChildUtilities.CheckCircularGroup(
                context: context,
                groupId: groupId,
                disabled: false,
                children: childStrings);
            return cycle != Implem.Pleasanter.Libraries.General.Error.Types.None
                ? Error(StatusCodes.Status400BadRequest, "Circular group or group depth limit was detected", "invalidValue")
                : null;
        }

        private static void AddGroupMembers(
            Context context,
            int groupId,
            List<int> userIds,
            List<int> childGroupIds)
        {
            var statements = new List<SqlStatement>();
            userIds
                .Distinct()
                .ForEach(userId => statements.Add(Rds.UpdateOrInsertGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(groupId)
                        .ChildGroup(false)
                        .DeptId(0)
                        .UserId(userId),
                    param: Rds.GroupMembersParam()
                        .GroupId(groupId)
                        .DeptId(0)
                        .UserId(userId)
                        .ChildGroup(false)
                        .Admin(false))));
            childGroupIds
                .Distinct()
                .ForEach(childId => statements.Add(Rds.UpdateOrInsertGroupChildren(
                    where: Rds.GroupChildrenWhere()
                        .GroupId(groupId)
                        .ChildId(childId),
                    param: Rds.GroupChildrenParam()
                        .GroupId(groupId)
                        .ChildId(childId))));
            if (statements.Count == 0)
            {
                return;
            }
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: context.TenantId,
                type: StatusUtilities.Types.GroupsUpdated));
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: [.. statements]);
            GroupMemberUtilities.SyncGroupMembers(context, groupId);
        }

        private static void RemoveGroupMembers(
            Context context,
            int groupId,
            List<int> userIds,
            List<int> childGroupIds)
        {
            var statements = new List<SqlStatement>();
            userIds
                .Distinct()
                .ForEach(userId => statements.Add(Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(groupId)
                        .ChildGroup(false)
                        .DeptId(0)
                        .UserId(userId))));
            childGroupIds
                .Distinct()
                .ForEach(childId => statements.Add(Rds.PhysicalDeleteGroupChildren(
                    where: Rds.GroupChildrenWhere()
                        .GroupId(groupId)
                        .ChildId(childId))));
            if (statements.Count == 0)
            {
                return;
            }
            statements.Add(StatusUtilities.UpdateStatus(
                tenantId: context.TenantId,
                type: StatusUtilities.Types.GroupsUpdated));
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: [.. statements]);
            GroupMemberUtilities.SyncGroupMembers(context, groupId);
        }

        private static bool TryAddUserMember(
            Context context,
            string scimId,
            List<int> userIds)
        {
            var userModel = FindUserModelByScimId(context, scimId);
            if (userModel == null)
            {
                return false;
            }
            userIds.Add(userModel.UserId);
            return true;
        }

        private static bool TryAddChildGroupMember(
            Context context,
            string scimId,
            List<int> childGroupIds)
        {
            var groupRow = FindGroupModelByScimId(context, scimId);
            if (groupRow == null)
            {
                return false;
            }
            childGroupIds.Add(groupRow.GroupId);
            return true;
        }

        private static string MemberType(ScimMember member)
        {
            var type = member.Type?.Trim();
            if (!type.IsNullOrEmpty())
            {
                return type.ToLowerInvariant();
            }
            var reference = member.Ref?.Trim();
            if (reference.IsNullOrEmpty())
            {
                return null;
            }
            if (reference.Contains("/Groups/", StringComparison.OrdinalIgnoreCase))
            {
                return "group";
            }
            if (reference.Contains("/Users/", StringComparison.OrdinalIgnoreCase))
            {
                return "user";
            }
            return null;
        }

        private static void UpdateMailAddresses(
            Context context,
            int userId,
            List<ScimEmail> emails)
        {
            if (userId <= 0)
            {
                return;
            }
            emails ??= [];
            var statements = new List<SqlStatement>
            {
                Rds.PhysicalDeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerId(userId)
                        .OwnerType("Users"))
            };
            emails
                .Where(email => !email.Value.IsNullOrEmpty())
                .Select(email => email.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ForEach(mailAddress => statements.Add(Rds.InsertMailAddresses(
                    param: Rds.MailAddressesParam()
                        .OwnerId(userId)
                        .OwnerType("Users")
                        .MailAddress(mailAddress))));
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: [.. statements]);
        }

    }
}