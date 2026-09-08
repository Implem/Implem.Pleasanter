using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using GroupCollection = Implem.Pleasanter.Models.GroupCollection;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static UserModel FindUserModelByScimId(Context context, string scimId)
        {
            if (scimId.IsNullOrEmpty())
            {
                return null;
            }
            var collection = new UserCollection(
                context: context,
                ss: null,
                column: UserColumns(),
                where: ScimUserWhere(context, scimId),
                top: 1);
            return collection.FirstOrDefault();
        }

        private static UserModel FindUserModelByExternalId(Context context, string externalId)
        {
            if (externalId.IsNullOrEmpty())
            {
                return null;
            }
            var collection = new UserCollection(
                context: context,
                ss: null,
                column: UserColumns(),
                where: Rds.UsersWhere()
                    .TenantId(context.TenantId)
                    .ScimExternalId(externalId),
                top: 1);
            return collection.FirstOrDefault();
        }

        private static UserModel FindUserModelByLoginId(Context context, string loginId)
        {
            if (loginId.IsNullOrEmpty())
            {
                return null;
            }
            var collection = new UserCollection(
                context: context,
                ss: null,
                column: UserColumns(),
                where: Rds.UsersWhere()
                    .TenantId(context.TenantId)
                    .LoginId(loginId),
                top: 1);
            return collection.FirstOrDefault();
        }

        private static GroupModel FindGroupModelByScimId(Context context, string scimId)
        {
            if (scimId.IsNullOrEmpty())
            {
                return null;
            }
            var collection = new GroupCollection(
                context: context,
                ss: null,
                column: GroupColumns(),
                where: ScimGroupWhere(context, scimId),
                top: 1);
            return collection.FirstOrDefault();
        }

        private static GroupModel FindGroupModelByExternalId(Context context, string externalId)
        {
            if (externalId.IsNullOrEmpty())
            {
                return null;
            }
            var collection = new GroupCollection(
                context: context,
                ss: null,
                column: GroupColumns(),
                where: Rds.GroupsWhere()
                    .TenantId(context.TenantId)
                    .ScimExternalId(externalId),
                top: 1);
            return collection.FirstOrDefault();
        }

        private static ScimUser ToUser(Context context, UserModel model)
        {
            var id = model.ScimId;
            var userId = model.UserId;
            var givenName = model.FirstName;
            var familyName = model.LastName;
            var displayName = model.Name;
            var user = new ScimUser
            {
                Id = id,
                ExternalId = model.ScimExternalId,
                UserName = model.LoginId,
                DisplayName = displayName,
                Active = !model.Disabled,
                Name = new ScimName
                {
                    GivenName = givenName,
                    FamilyName = familyName,
                    Formatted = displayName
                },
                Emails = MailAddresses(context, userId),
                PreferredLanguage = ToScimLanguage(model.Language),
                Timezone = model.TimeZone.IsNullOrEmpty() ? null : model.TimeZone,
                Meta = Meta(context, "User", id, model.CreatedTime.Value, model.UpdatedTime.Value)
            };
            var department = Department(context, model.DeptId);
            var userCode = model.UserCode;
            var manager = ManagerReference(context, userId);
            if (!department.IsNullOrEmpty()
                || !userCode.IsNullOrEmpty()
                || manager != null)
            {
                if (!user.Schemas.Contains(ScimSchemas.EnterpriseUser))
                {
                    user.Schemas.Add(ScimSchemas.EnterpriseUser);
                }
                user.EnterpriseUser = new ScimEnterpriseUser
                {
                    Department = department,
                    EmployeeNumber = userCode,
                    Manager = manager
                };
            }
            AddExtendedAttributeResponse(
                schemas: user.Schemas,
                extensionData: ExtensionData(user),
                model: model,
                attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                tableName: ScimExtendedAttributeUtilities.UsersTableName);
            return user;
        }

        private static ScimGroup ToGroup(Context context, GroupModel model, bool includeMembers)
        {
            var id = model.ScimId;
            var groupId = model.GroupId;
            var group = new ScimGroup
            {
                Id = id,
                ExternalId = model.ScimExternalId,
                DisplayName = model.GroupName,
                Members = includeMembers
                    ? GroupMembers(context, groupId)
                    : null,
                Meta = Meta(context, "Group", id, model.CreatedTime.Value, model.UpdatedTime.Value)
            };
            AddExtendedAttributeResponse(
                schemas: group.Schemas,
                extensionData: ExtensionData(group),
                model: model,
                attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                tableName: ScimExtendedAttributeUtilities.GroupsTableName);
            return group;
        }

        private static bool IncludeMembers(string excludedAttributes)
        {
            return excludedAttributes.IsNullOrEmpty()
                || !excludedAttributes
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(attribute => attribute.Trim())
                    .Any(attribute => string.Equals(attribute, "members", StringComparison.OrdinalIgnoreCase));
        }

        private static List<ScimEmail> MailAddresses(Context context, int userId)
        {
            return [.. new MailAddressCollection(
                context: context,
                column: Rds.MailAddressesColumn().MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId(userId)
                        .OwnerType("Users"))
                            .Select((model, index) => new ScimEmail
                            {
                                Value = model.MailAddress,
                                Type = "work",
                                Primary = index == 0
                            })
                            .Where(email => !email.Value.IsNullOrEmpty())];
        }

        private static string Department(Context context, int deptId)
        {
            if (deptId <= 0)
            {
                return null;
            }
            var dept = new DeptCollection(
                context: context,
                ss: null,
                column: Rds.DeptsColumn()
                    .DeptCode()
                    .DeptName(),
                where: Rds.DeptsWhere()
                    .TenantId(context.TenantId)
                    .DeptId(deptId),
                top: 1)
                    .FirstOrDefault();
            return dept == null
                ? null
                : dept.DeptCode.IsNullOrEmpty()
                    ? dept.DeptName
                    : dept.DeptCode;
        }

        private static List<ScimMember> GroupMembers(Context context, int groupId)
        {
            var users = new GroupMemberCollection(
                context: context,
                column: Rds.GroupMembersColumn()
                    .UserId(),
                where: Rds.GroupMembersWhere()
                    .GroupId(groupId)
                    .ChildGroup(false))
                        .Select(model => model.UserId)
                        .Where(userId => userId > 0)
                        .Distinct()
                        .Select(userId => FindUserRowByUserId(context, userId))
                        .Where(row => row != null)
                        .Select(model => new ScimMember
                        {
                            Value = model.ScimId,
                            Display = model.Name,
                            Type = "User",
                            Ref = ResourceLocation(context, "Users", model.ScimId)
                        });
            var groups = new GroupChildCollection(
                context: context,
                column: Rds.GroupChildrenColumn().ChildId(),
                where: Rds.GroupChildrenWhere().GroupId(groupId))
                    .AsEnumerable()
                    .Select(model => model.ChildId)
                    .Where(childId => childId > 0)
                    .Distinct()
                    .Select(childId => FindGroupRowByGroupId(context, childId))
                    .Where(row => row != null)
                    .Select(model => new ScimMember
                    {
                        Value = model.ScimId,
                        Display = model.GroupName,
                        Type = "Group",
                        Ref = ResourceLocation(context, "Groups", model.ScimId)
                    });
            return [.. users
                .Concat(groups)
                .Where(member => !member.Value.IsNullOrEmpty())];
        }

        private static UserModel FindUserRowByUserId(Context context, int userId)
        {
            return new UserCollection(
                context: context,
                ss: null,
                column: UserColumns(),
                where: Rds.UsersWhere()
                    .TenantId(context.TenantId)
                    .UserId(userId),
                top: 1).FirstOrDefault();
        }

        private static GroupModel FindGroupRowByGroupId(Context context, int groupId)
        {
            return new GroupCollection(
                context: context,
                ss: null,
                column: GroupColumns(),
                where: Rds.GroupsWhere()
                    .TenantId(context.TenantId)
                    .GroupId(groupId),
                top: 1).FirstOrDefault();
        }

        private static Rds.UsersColumnCollection UserColumns()
        {
            var columns = Rds.UsersColumn()
                .UserId()
                .LoginId()
                .Name()
                .UserCode()
                .FirstName()
                .LastName()
                .Language()
                .TimeZone()
                .DeptId()
                .Manager()
                .Disabled()
                .CreatedTime()
                .UpdatedTime()
                .ScimId()
                .ScimExternalId()
                .ScimSync();
            AddExtendedAttributeColumns(
                columns: columns,
                attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                tableName: ScimExtendedAttributeUtilities.UsersTableName);
            return columns;
        }

        private static Rds.GroupsColumnCollection GroupColumns()
        {
            var columns = Rds.GroupsColumn()
                .GroupId()
                .GroupName()
                .Disabled()
                .CreatedTime()
                .UpdatedTime()
                .ScimId()
                .ScimExternalId()
                .ScimSync();
            AddExtendedAttributeColumns(
                columns: columns,
                attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                tableName: ScimExtendedAttributeUtilities.GroupsTableName);
            return columns;
        }

        private static Rds.UsersWhereCollection ScimUserWhere(Context context, string scimId)
        {
            return Rds.UsersWhere()
                .TenantId(context.TenantId)
                .ScimId(scimId);
        }

        private static Rds.GroupsWhereCollection ScimGroupWhere(Context context, string scimId)
        {
            return Rds.GroupsWhere()
                .TenantId(context.TenantId)
                .ScimId(scimId)
                .Disabled(false);
        }

    }
}