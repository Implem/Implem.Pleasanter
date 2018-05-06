using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Responses
{
    public class TenantsResponseCollection : ResponseCollection
    {
        public TenantModel TenantModel;

        public TenantsResponseCollection(TenantModel tenantModel)
        {
            TenantModel = tenantModel;
        }

        public TenantsResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public TenantsResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class DemosResponseCollection : ResponseCollection
    {
        public DemoModel DemoModel;

        public DemosResponseCollection(DemoModel demoModel)
        {
            DemoModel = demoModel;
        }

        public DemosResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public DemosResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class StatusesResponseCollection : ResponseCollection
    {
        public StatusModel StatusModel;

        public StatusesResponseCollection(StatusModel statusModel)
        {
            StatusModel = statusModel;
        }

        public StatusesResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public StatusesResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class ReminderSchedulesResponseCollection : ResponseCollection
    {
        public ReminderScheduleModel ReminderScheduleModel;

        public ReminderSchedulesResponseCollection(ReminderScheduleModel reminderScheduleModel)
        {
            ReminderScheduleModel = reminderScheduleModel;
        }

        public ReminderSchedulesResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public ReminderSchedulesResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class HealthsResponseCollection : ResponseCollection
    {
        public HealthModel HealthModel;

        public HealthsResponseCollection(HealthModel healthModel)
        {
            HealthModel = healthModel;
        }

        public HealthsResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public HealthsResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class DeptsResponseCollection : ResponseCollection
    {
        public DeptModel DeptModel;

        public DeptsResponseCollection(DeptModel deptModel)
        {
            DeptModel = deptModel;
        }

        public DeptsResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public DeptsResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class GroupsResponseCollection : ResponseCollection
    {
        public GroupModel GroupModel;

        public GroupsResponseCollection(GroupModel groupModel)
        {
            GroupModel = groupModel;
        }

        public GroupsResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public GroupsResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class GroupMembersResponseCollection : ResponseCollection
    {
        public GroupMemberModel GroupMemberModel;

        public GroupMembersResponseCollection(GroupMemberModel groupMemberModel)
        {
            GroupMemberModel = groupMemberModel;
        }

        public GroupMembersResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public GroupMembersResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class UsersResponseCollection : ResponseCollection
    {
        public UserModel UserModel;

        public UsersResponseCollection(UserModel userModel)
        {
            UserModel = userModel;
        }

        public UsersResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public UsersResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class LoginKeysResponseCollection : ResponseCollection
    {
        public LoginKeyModel LoginKeyModel;

        public LoginKeysResponseCollection(LoginKeyModel loginKeyModel)
        {
            LoginKeyModel = loginKeyModel;
        }

        public LoginKeysResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public LoginKeysResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class MailAddressesResponseCollection : ResponseCollection
    {
        public MailAddressModel MailAddressModel;

        public MailAddressesResponseCollection(MailAddressModel mailAddressModel)
        {
            MailAddressModel = mailAddressModel;
        }

        public MailAddressesResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public MailAddressesResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class OutgoingMailsResponseCollection : ResponseCollection
    {
        public OutgoingMailModel OutgoingMailModel;

        public OutgoingMailsResponseCollection(OutgoingMailModel outgoingMailModel)
        {
            OutgoingMailModel = outgoingMailModel;
        }

        public OutgoingMailsResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public OutgoingMailsResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class SearchIndexesResponseCollection : ResponseCollection
    {
        public SearchIndexModel SearchIndexModel;

        public SearchIndexesResponseCollection(SearchIndexModel searchIndexModel)
        {
            SearchIndexModel = searchIndexModel;
        }

        public SearchIndexesResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public SearchIndexesResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class SitesResponseCollection : ResponseCollection
    {
        public SiteModel SiteModel;

        public SitesResponseCollection(SiteModel siteModel)
        {
            SiteModel = siteModel;
        }

        public SitesResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public SitesResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class OrdersResponseCollection : ResponseCollection
    {
        public OrderModel OrderModel;

        public OrdersResponseCollection(OrderModel orderModel)
        {
            OrderModel = orderModel;
        }

        public OrdersResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public OrdersResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class ExportSettingsResponseCollection : ResponseCollection
    {
        public ExportSettingModel ExportSettingModel;

        public ExportSettingsResponseCollection(ExportSettingModel exportSettingModel)
        {
            ExportSettingModel = exportSettingModel;
        }

        public ExportSettingsResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public ExportSettingsResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class LinksResponseCollection : ResponseCollection
    {
        public LinkModel LinkModel;

        public LinksResponseCollection(LinkModel linkModel)
        {
            LinkModel = linkModel;
        }

        public LinksResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public LinksResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class BinariesResponseCollection : ResponseCollection
    {
        public BinaryModel BinaryModel;

        public BinariesResponseCollection(BinaryModel binaryModel)
        {
            BinaryModel = binaryModel;
        }

        public BinariesResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public BinariesResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class PermissionsResponseCollection : ResponseCollection
    {
        public PermissionModel PermissionModel;

        public PermissionsResponseCollection(PermissionModel permissionModel)
        {
            PermissionModel = permissionModel;
        }

        public PermissionsResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public PermissionsResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class IssuesResponseCollection : ResponseCollection
    {
        public IssueModel IssueModel;

        public IssuesResponseCollection(IssueModel issueModel)
        {
            IssueModel = issueModel;
        }

        public IssuesResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public IssuesResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class ResultsResponseCollection : ResponseCollection
    {
        public ResultModel ResultModel;

        public ResultsResponseCollection(ResultModel resultModel)
        {
            ResultModel = resultModel;
        }

        public ResultsResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public ResultsResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public class WikisResponseCollection : ResponseCollection
    {
        public WikiModel WikiModel;

        public WikisResponseCollection(WikiModel wikiModel)
        {
            WikiModel = wikiModel;
        }

        public WikisResponseCollection Val(string selector, string value)
        {
            base.Val(selector, value);
            return this;
        }

        public WikisResponseCollection ValAndFormData(string selector, string value)
        {
            base.ValAndFormData(selector, value);
            return this;
        }
    }

    public static class ResponseCollectionSpecials
    {
        public static TenantsResponseCollection Ver(this TenantsResponseCollection res) { return res.Val("#Tenants_Ver", res.TenantModel.Ver.ToResponse()); }
        public static TenantsResponseCollection Ver(this TenantsResponseCollection res, string value) { return res.Val("#Tenants_Ver", value); }
        public static TenantsResponseCollection Ver_FormData(this TenantsResponseCollection res) { return res.ValAndFormData("#Tenants_Ver", res.TenantModel.Ver.ToResponse()); }
        public static TenantsResponseCollection Ver_FormData(this TenantsResponseCollection res, string value) { return res.ValAndFormData("#Tenants_Ver", value); }
        public static TenantsResponseCollection Comments(this TenantsResponseCollection res) { return res.Val("#Tenants_Comments", res.TenantModel.Comments.ToResponse()); }
        public static TenantsResponseCollection Comments(this TenantsResponseCollection res, string value) { return res.Val("#Tenants_Comments", value); }
        public static TenantsResponseCollection Comments_FormData(this TenantsResponseCollection res) { return res.ValAndFormData("#Tenants_Comments", res.TenantModel.Comments.ToResponse()); }
        public static TenantsResponseCollection Comments_FormData(this TenantsResponseCollection res, string value) { return res.ValAndFormData("#Tenants_Comments", value); }
        public static TenantsResponseCollection CreatedTime(this TenantsResponseCollection res) { return res.Val("#Tenants_CreatedTime", res.TenantModel.CreatedTime.ToResponse()); }
        public static TenantsResponseCollection CreatedTime(this TenantsResponseCollection res, string value) { return res.Val("#Tenants_CreatedTime", value); }
        public static TenantsResponseCollection CreatedTime_FormData(this TenantsResponseCollection res) { return res.ValAndFormData("#Tenants_CreatedTime", res.TenantModel.CreatedTime.ToResponse()); }
        public static TenantsResponseCollection CreatedTime_FormData(this TenantsResponseCollection res, string value) { return res.ValAndFormData("#Tenants_CreatedTime", value); }
        public static TenantsResponseCollection UpdatedTime(this TenantsResponseCollection res) { return res.Val("#Tenants_UpdatedTime", res.TenantModel.UpdatedTime.ToResponse()); }
        public static TenantsResponseCollection UpdatedTime(this TenantsResponseCollection res, string value) { return res.Val("#Tenants_UpdatedTime", value); }
        public static TenantsResponseCollection UpdatedTime_FormData(this TenantsResponseCollection res) { return res.ValAndFormData("#Tenants_UpdatedTime", res.TenantModel.UpdatedTime.ToResponse()); }
        public static TenantsResponseCollection UpdatedTime_FormData(this TenantsResponseCollection res, string value) { return res.ValAndFormData("#Tenants_UpdatedTime", value); }
        public static TenantsResponseCollection Timestamp(this TenantsResponseCollection res) { return res.Val("#Tenants_Timestamp", res.TenantModel.Timestamp.ToResponse()); }
        public static TenantsResponseCollection Timestamp(this TenantsResponseCollection res, string value) { return res.Val("#Tenants_Timestamp", value); }
        public static TenantsResponseCollection Timestamp_FormData(this TenantsResponseCollection res) { return res.ValAndFormData("#Tenants_Timestamp", res.TenantModel.Timestamp.ToResponse()); }
        public static TenantsResponseCollection Timestamp_FormData(this TenantsResponseCollection res, string value) { return res.ValAndFormData("#Tenants_Timestamp", value); }
        public static DemosResponseCollection Ver(this DemosResponseCollection res) { return res.Val("#Demos_Ver", res.DemoModel.Ver.ToResponse()); }
        public static DemosResponseCollection Ver(this DemosResponseCollection res, string value) { return res.Val("#Demos_Ver", value); }
        public static DemosResponseCollection Ver_FormData(this DemosResponseCollection res) { return res.ValAndFormData("#Demos_Ver", res.DemoModel.Ver.ToResponse()); }
        public static DemosResponseCollection Ver_FormData(this DemosResponseCollection res, string value) { return res.ValAndFormData("#Demos_Ver", value); }
        public static DemosResponseCollection Comments(this DemosResponseCollection res) { return res.Val("#Demos_Comments", res.DemoModel.Comments.ToResponse()); }
        public static DemosResponseCollection Comments(this DemosResponseCollection res, string value) { return res.Val("#Demos_Comments", value); }
        public static DemosResponseCollection Comments_FormData(this DemosResponseCollection res) { return res.ValAndFormData("#Demos_Comments", res.DemoModel.Comments.ToResponse()); }
        public static DemosResponseCollection Comments_FormData(this DemosResponseCollection res, string value) { return res.ValAndFormData("#Demos_Comments", value); }
        public static DemosResponseCollection CreatedTime(this DemosResponseCollection res) { return res.Val("#Demos_CreatedTime", res.DemoModel.CreatedTime.ToResponse()); }
        public static DemosResponseCollection CreatedTime(this DemosResponseCollection res, string value) { return res.Val("#Demos_CreatedTime", value); }
        public static DemosResponseCollection CreatedTime_FormData(this DemosResponseCollection res) { return res.ValAndFormData("#Demos_CreatedTime", res.DemoModel.CreatedTime.ToResponse()); }
        public static DemosResponseCollection CreatedTime_FormData(this DemosResponseCollection res, string value) { return res.ValAndFormData("#Demos_CreatedTime", value); }
        public static DemosResponseCollection UpdatedTime(this DemosResponseCollection res) { return res.Val("#Demos_UpdatedTime", res.DemoModel.UpdatedTime.ToResponse()); }
        public static DemosResponseCollection UpdatedTime(this DemosResponseCollection res, string value) { return res.Val("#Demos_UpdatedTime", value); }
        public static DemosResponseCollection UpdatedTime_FormData(this DemosResponseCollection res) { return res.ValAndFormData("#Demos_UpdatedTime", res.DemoModel.UpdatedTime.ToResponse()); }
        public static DemosResponseCollection UpdatedTime_FormData(this DemosResponseCollection res, string value) { return res.ValAndFormData("#Demos_UpdatedTime", value); }
        public static DemosResponseCollection Timestamp(this DemosResponseCollection res) { return res.Val("#Demos_Timestamp", res.DemoModel.Timestamp.ToResponse()); }
        public static DemosResponseCollection Timestamp(this DemosResponseCollection res, string value) { return res.Val("#Demos_Timestamp", value); }
        public static DemosResponseCollection Timestamp_FormData(this DemosResponseCollection res) { return res.ValAndFormData("#Demos_Timestamp", res.DemoModel.Timestamp.ToResponse()); }
        public static DemosResponseCollection Timestamp_FormData(this DemosResponseCollection res, string value) { return res.ValAndFormData("#Demos_Timestamp", value); }
        public static StatusesResponseCollection Ver(this StatusesResponseCollection res) { return res.Val("#Statuses_Ver", res.StatusModel.Ver.ToResponse()); }
        public static StatusesResponseCollection Ver(this StatusesResponseCollection res, string value) { return res.Val("#Statuses_Ver", value); }
        public static StatusesResponseCollection Ver_FormData(this StatusesResponseCollection res) { return res.ValAndFormData("#Statuses_Ver", res.StatusModel.Ver.ToResponse()); }
        public static StatusesResponseCollection Ver_FormData(this StatusesResponseCollection res, string value) { return res.ValAndFormData("#Statuses_Ver", value); }
        public static StatusesResponseCollection Comments(this StatusesResponseCollection res) { return res.Val("#Statuses_Comments", res.StatusModel.Comments.ToResponse()); }
        public static StatusesResponseCollection Comments(this StatusesResponseCollection res, string value) { return res.Val("#Statuses_Comments", value); }
        public static StatusesResponseCollection Comments_FormData(this StatusesResponseCollection res) { return res.ValAndFormData("#Statuses_Comments", res.StatusModel.Comments.ToResponse()); }
        public static StatusesResponseCollection Comments_FormData(this StatusesResponseCollection res, string value) { return res.ValAndFormData("#Statuses_Comments", value); }
        public static StatusesResponseCollection CreatedTime(this StatusesResponseCollection res) { return res.Val("#Statuses_CreatedTime", res.StatusModel.CreatedTime.ToResponse()); }
        public static StatusesResponseCollection CreatedTime(this StatusesResponseCollection res, string value) { return res.Val("#Statuses_CreatedTime", value); }
        public static StatusesResponseCollection CreatedTime_FormData(this StatusesResponseCollection res) { return res.ValAndFormData("#Statuses_CreatedTime", res.StatusModel.CreatedTime.ToResponse()); }
        public static StatusesResponseCollection CreatedTime_FormData(this StatusesResponseCollection res, string value) { return res.ValAndFormData("#Statuses_CreatedTime", value); }
        public static StatusesResponseCollection UpdatedTime(this StatusesResponseCollection res) { return res.Val("#Statuses_UpdatedTime", res.StatusModel.UpdatedTime.ToResponse()); }
        public static StatusesResponseCollection UpdatedTime(this StatusesResponseCollection res, string value) { return res.Val("#Statuses_UpdatedTime", value); }
        public static StatusesResponseCollection UpdatedTime_FormData(this StatusesResponseCollection res) { return res.ValAndFormData("#Statuses_UpdatedTime", res.StatusModel.UpdatedTime.ToResponse()); }
        public static StatusesResponseCollection UpdatedTime_FormData(this StatusesResponseCollection res, string value) { return res.ValAndFormData("#Statuses_UpdatedTime", value); }
        public static StatusesResponseCollection Timestamp(this StatusesResponseCollection res) { return res.Val("#Statuses_Timestamp", res.StatusModel.Timestamp.ToResponse()); }
        public static StatusesResponseCollection Timestamp(this StatusesResponseCollection res, string value) { return res.Val("#Statuses_Timestamp", value); }
        public static StatusesResponseCollection Timestamp_FormData(this StatusesResponseCollection res) { return res.ValAndFormData("#Statuses_Timestamp", res.StatusModel.Timestamp.ToResponse()); }
        public static StatusesResponseCollection Timestamp_FormData(this StatusesResponseCollection res, string value) { return res.ValAndFormData("#Statuses_Timestamp", value); }
        public static ReminderSchedulesResponseCollection Ver(this ReminderSchedulesResponseCollection res) { return res.Val("#ReminderSchedules_Ver", res.ReminderScheduleModel.Ver.ToResponse()); }
        public static ReminderSchedulesResponseCollection Ver(this ReminderSchedulesResponseCollection res, string value) { return res.Val("#ReminderSchedules_Ver", value); }
        public static ReminderSchedulesResponseCollection Ver_FormData(this ReminderSchedulesResponseCollection res) { return res.ValAndFormData("#ReminderSchedules_Ver", res.ReminderScheduleModel.Ver.ToResponse()); }
        public static ReminderSchedulesResponseCollection Ver_FormData(this ReminderSchedulesResponseCollection res, string value) { return res.ValAndFormData("#ReminderSchedules_Ver", value); }
        public static ReminderSchedulesResponseCollection Comments(this ReminderSchedulesResponseCollection res) { return res.Val("#ReminderSchedules_Comments", res.ReminderScheduleModel.Comments.ToResponse()); }
        public static ReminderSchedulesResponseCollection Comments(this ReminderSchedulesResponseCollection res, string value) { return res.Val("#ReminderSchedules_Comments", value); }
        public static ReminderSchedulesResponseCollection Comments_FormData(this ReminderSchedulesResponseCollection res) { return res.ValAndFormData("#ReminderSchedules_Comments", res.ReminderScheduleModel.Comments.ToResponse()); }
        public static ReminderSchedulesResponseCollection Comments_FormData(this ReminderSchedulesResponseCollection res, string value) { return res.ValAndFormData("#ReminderSchedules_Comments", value); }
        public static ReminderSchedulesResponseCollection CreatedTime(this ReminderSchedulesResponseCollection res) { return res.Val("#ReminderSchedules_CreatedTime", res.ReminderScheduleModel.CreatedTime.ToResponse()); }
        public static ReminderSchedulesResponseCollection CreatedTime(this ReminderSchedulesResponseCollection res, string value) { return res.Val("#ReminderSchedules_CreatedTime", value); }
        public static ReminderSchedulesResponseCollection CreatedTime_FormData(this ReminderSchedulesResponseCollection res) { return res.ValAndFormData("#ReminderSchedules_CreatedTime", res.ReminderScheduleModel.CreatedTime.ToResponse()); }
        public static ReminderSchedulesResponseCollection CreatedTime_FormData(this ReminderSchedulesResponseCollection res, string value) { return res.ValAndFormData("#ReminderSchedules_CreatedTime", value); }
        public static ReminderSchedulesResponseCollection UpdatedTime(this ReminderSchedulesResponseCollection res) { return res.Val("#ReminderSchedules_UpdatedTime", res.ReminderScheduleModel.UpdatedTime.ToResponse()); }
        public static ReminderSchedulesResponseCollection UpdatedTime(this ReminderSchedulesResponseCollection res, string value) { return res.Val("#ReminderSchedules_UpdatedTime", value); }
        public static ReminderSchedulesResponseCollection UpdatedTime_FormData(this ReminderSchedulesResponseCollection res) { return res.ValAndFormData("#ReminderSchedules_UpdatedTime", res.ReminderScheduleModel.UpdatedTime.ToResponse()); }
        public static ReminderSchedulesResponseCollection UpdatedTime_FormData(this ReminderSchedulesResponseCollection res, string value) { return res.ValAndFormData("#ReminderSchedules_UpdatedTime", value); }
        public static ReminderSchedulesResponseCollection Timestamp(this ReminderSchedulesResponseCollection res) { return res.Val("#ReminderSchedules_Timestamp", res.ReminderScheduleModel.Timestamp.ToResponse()); }
        public static ReminderSchedulesResponseCollection Timestamp(this ReminderSchedulesResponseCollection res, string value) { return res.Val("#ReminderSchedules_Timestamp", value); }
        public static ReminderSchedulesResponseCollection Timestamp_FormData(this ReminderSchedulesResponseCollection res) { return res.ValAndFormData("#ReminderSchedules_Timestamp", res.ReminderScheduleModel.Timestamp.ToResponse()); }
        public static ReminderSchedulesResponseCollection Timestamp_FormData(this ReminderSchedulesResponseCollection res, string value) { return res.ValAndFormData("#ReminderSchedules_Timestamp", value); }
        public static HealthsResponseCollection Ver(this HealthsResponseCollection res) { return res.Val("#Healths_Ver", res.HealthModel.Ver.ToResponse()); }
        public static HealthsResponseCollection Ver(this HealthsResponseCollection res, string value) { return res.Val("#Healths_Ver", value); }
        public static HealthsResponseCollection Ver_FormData(this HealthsResponseCollection res) { return res.ValAndFormData("#Healths_Ver", res.HealthModel.Ver.ToResponse()); }
        public static HealthsResponseCollection Ver_FormData(this HealthsResponseCollection res, string value) { return res.ValAndFormData("#Healths_Ver", value); }
        public static HealthsResponseCollection Comments(this HealthsResponseCollection res) { return res.Val("#Healths_Comments", res.HealthModel.Comments.ToResponse()); }
        public static HealthsResponseCollection Comments(this HealthsResponseCollection res, string value) { return res.Val("#Healths_Comments", value); }
        public static HealthsResponseCollection Comments_FormData(this HealthsResponseCollection res) { return res.ValAndFormData("#Healths_Comments", res.HealthModel.Comments.ToResponse()); }
        public static HealthsResponseCollection Comments_FormData(this HealthsResponseCollection res, string value) { return res.ValAndFormData("#Healths_Comments", value); }
        public static HealthsResponseCollection CreatedTime(this HealthsResponseCollection res) { return res.Val("#Healths_CreatedTime", res.HealthModel.CreatedTime.ToResponse()); }
        public static HealthsResponseCollection CreatedTime(this HealthsResponseCollection res, string value) { return res.Val("#Healths_CreatedTime", value); }
        public static HealthsResponseCollection CreatedTime_FormData(this HealthsResponseCollection res) { return res.ValAndFormData("#Healths_CreatedTime", res.HealthModel.CreatedTime.ToResponse()); }
        public static HealthsResponseCollection CreatedTime_FormData(this HealthsResponseCollection res, string value) { return res.ValAndFormData("#Healths_CreatedTime", value); }
        public static HealthsResponseCollection UpdatedTime(this HealthsResponseCollection res) { return res.Val("#Healths_UpdatedTime", res.HealthModel.UpdatedTime.ToResponse()); }
        public static HealthsResponseCollection UpdatedTime(this HealthsResponseCollection res, string value) { return res.Val("#Healths_UpdatedTime", value); }
        public static HealthsResponseCollection UpdatedTime_FormData(this HealthsResponseCollection res) { return res.ValAndFormData("#Healths_UpdatedTime", res.HealthModel.UpdatedTime.ToResponse()); }
        public static HealthsResponseCollection UpdatedTime_FormData(this HealthsResponseCollection res, string value) { return res.ValAndFormData("#Healths_UpdatedTime", value); }
        public static HealthsResponseCollection Timestamp(this HealthsResponseCollection res) { return res.Val("#Healths_Timestamp", res.HealthModel.Timestamp.ToResponse()); }
        public static HealthsResponseCollection Timestamp(this HealthsResponseCollection res, string value) { return res.Val("#Healths_Timestamp", value); }
        public static HealthsResponseCollection Timestamp_FormData(this HealthsResponseCollection res) { return res.ValAndFormData("#Healths_Timestamp", res.HealthModel.Timestamp.ToResponse()); }
        public static HealthsResponseCollection Timestamp_FormData(this HealthsResponseCollection res, string value) { return res.ValAndFormData("#Healths_Timestamp", value); }
        public static DeptsResponseCollection DeptId(this DeptsResponseCollection res) { return res.Val("#Depts_DeptId", res.DeptModel.DeptId.ToResponse()); }
        public static DeptsResponseCollection DeptId(this DeptsResponseCollection res, string value) { return res.Val("#Depts_DeptId", value); }
        public static DeptsResponseCollection DeptId_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_DeptId", res.DeptModel.DeptId.ToResponse()); }
        public static DeptsResponseCollection DeptId_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_DeptId", value); }
        public static DeptsResponseCollection Ver(this DeptsResponseCollection res) { return res.Val("#Depts_Ver", res.DeptModel.Ver.ToResponse()); }
        public static DeptsResponseCollection Ver(this DeptsResponseCollection res, string value) { return res.Val("#Depts_Ver", value); }
        public static DeptsResponseCollection Ver_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_Ver", res.DeptModel.Ver.ToResponse()); }
        public static DeptsResponseCollection Ver_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_Ver", value); }
        public static DeptsResponseCollection DeptCode(this DeptsResponseCollection res) { return res.Val("#Depts_DeptCode", res.DeptModel.DeptCode.ToResponse()); }
        public static DeptsResponseCollection DeptCode(this DeptsResponseCollection res, string value) { return res.Val("#Depts_DeptCode", value); }
        public static DeptsResponseCollection DeptCode_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_DeptCode", res.DeptModel.DeptCode.ToResponse()); }
        public static DeptsResponseCollection DeptCode_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_DeptCode", value); }
        public static DeptsResponseCollection DeptName(this DeptsResponseCollection res) { return res.Val("#Depts_DeptName", res.DeptModel.DeptName.ToResponse()); }
        public static DeptsResponseCollection DeptName(this DeptsResponseCollection res, string value) { return res.Val("#Depts_DeptName", value); }
        public static DeptsResponseCollection DeptName_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_DeptName", res.DeptModel.DeptName.ToResponse()); }
        public static DeptsResponseCollection DeptName_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_DeptName", value); }
        public static DeptsResponseCollection Body(this DeptsResponseCollection res) { return res.Val("#Depts_Body", res.DeptModel.Body.ToResponse()); }
        public static DeptsResponseCollection Body(this DeptsResponseCollection res, string value) { return res.Val("#Depts_Body", value); }
        public static DeptsResponseCollection Body_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_Body", res.DeptModel.Body.ToResponse()); }
        public static DeptsResponseCollection Body_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_Body", value); }
        public static DeptsResponseCollection Comments(this DeptsResponseCollection res) { return res.Val("#Depts_Comments", res.DeptModel.Comments.ToResponse()); }
        public static DeptsResponseCollection Comments(this DeptsResponseCollection res, string value) { return res.Val("#Depts_Comments", value); }
        public static DeptsResponseCollection Comments_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_Comments", res.DeptModel.Comments.ToResponse()); }
        public static DeptsResponseCollection Comments_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_Comments", value); }
        public static DeptsResponseCollection CreatedTime(this DeptsResponseCollection res) { return res.Val("#Depts_CreatedTime", res.DeptModel.CreatedTime.ToResponse()); }
        public static DeptsResponseCollection CreatedTime(this DeptsResponseCollection res, string value) { return res.Val("#Depts_CreatedTime", value); }
        public static DeptsResponseCollection CreatedTime_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_CreatedTime", res.DeptModel.CreatedTime.ToResponse()); }
        public static DeptsResponseCollection CreatedTime_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_CreatedTime", value); }
        public static DeptsResponseCollection UpdatedTime(this DeptsResponseCollection res) { return res.Val("#Depts_UpdatedTime", res.DeptModel.UpdatedTime.ToResponse()); }
        public static DeptsResponseCollection UpdatedTime(this DeptsResponseCollection res, string value) { return res.Val("#Depts_UpdatedTime", value); }
        public static DeptsResponseCollection UpdatedTime_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_UpdatedTime", res.DeptModel.UpdatedTime.ToResponse()); }
        public static DeptsResponseCollection UpdatedTime_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_UpdatedTime", value); }
        public static DeptsResponseCollection Timestamp(this DeptsResponseCollection res) { return res.Val("#Depts_Timestamp", res.DeptModel.Timestamp.ToResponse()); }
        public static DeptsResponseCollection Timestamp(this DeptsResponseCollection res, string value) { return res.Val("#Depts_Timestamp", value); }
        public static DeptsResponseCollection Timestamp_FormData(this DeptsResponseCollection res) { return res.ValAndFormData("#Depts_Timestamp", res.DeptModel.Timestamp.ToResponse()); }
        public static DeptsResponseCollection Timestamp_FormData(this DeptsResponseCollection res, string value) { return res.ValAndFormData("#Depts_Timestamp", value); }
        public static GroupsResponseCollection GroupId(this GroupsResponseCollection res) { return res.Val("#Groups_GroupId", res.GroupModel.GroupId.ToResponse()); }
        public static GroupsResponseCollection GroupId(this GroupsResponseCollection res, string value) { return res.Val("#Groups_GroupId", value); }
        public static GroupsResponseCollection GroupId_FormData(this GroupsResponseCollection res) { return res.ValAndFormData("#Groups_GroupId", res.GroupModel.GroupId.ToResponse()); }
        public static GroupsResponseCollection GroupId_FormData(this GroupsResponseCollection res, string value) { return res.ValAndFormData("#Groups_GroupId", value); }
        public static GroupsResponseCollection Ver(this GroupsResponseCollection res) { return res.Val("#Groups_Ver", res.GroupModel.Ver.ToResponse()); }
        public static GroupsResponseCollection Ver(this GroupsResponseCollection res, string value) { return res.Val("#Groups_Ver", value); }
        public static GroupsResponseCollection Ver_FormData(this GroupsResponseCollection res) { return res.ValAndFormData("#Groups_Ver", res.GroupModel.Ver.ToResponse()); }
        public static GroupsResponseCollection Ver_FormData(this GroupsResponseCollection res, string value) { return res.ValAndFormData("#Groups_Ver", value); }
        public static GroupsResponseCollection GroupName(this GroupsResponseCollection res) { return res.Val("#Groups_GroupName", res.GroupModel.GroupName.ToResponse()); }
        public static GroupsResponseCollection GroupName(this GroupsResponseCollection res, string value) { return res.Val("#Groups_GroupName", value); }
        public static GroupsResponseCollection GroupName_FormData(this GroupsResponseCollection res) { return res.ValAndFormData("#Groups_GroupName", res.GroupModel.GroupName.ToResponse()); }
        public static GroupsResponseCollection GroupName_FormData(this GroupsResponseCollection res, string value) { return res.ValAndFormData("#Groups_GroupName", value); }
        public static GroupsResponseCollection Body(this GroupsResponseCollection res) { return res.Val("#Groups_Body", res.GroupModel.Body.ToResponse()); }
        public static GroupsResponseCollection Body(this GroupsResponseCollection res, string value) { return res.Val("#Groups_Body", value); }
        public static GroupsResponseCollection Body_FormData(this GroupsResponseCollection res) { return res.ValAndFormData("#Groups_Body", res.GroupModel.Body.ToResponse()); }
        public static GroupsResponseCollection Body_FormData(this GroupsResponseCollection res, string value) { return res.ValAndFormData("#Groups_Body", value); }
        public static GroupsResponseCollection Comments(this GroupsResponseCollection res) { return res.Val("#Groups_Comments", res.GroupModel.Comments.ToResponse()); }
        public static GroupsResponseCollection Comments(this GroupsResponseCollection res, string value) { return res.Val("#Groups_Comments", value); }
        public static GroupsResponseCollection Comments_FormData(this GroupsResponseCollection res) { return res.ValAndFormData("#Groups_Comments", res.GroupModel.Comments.ToResponse()); }
        public static GroupsResponseCollection Comments_FormData(this GroupsResponseCollection res, string value) { return res.ValAndFormData("#Groups_Comments", value); }
        public static GroupsResponseCollection CreatedTime(this GroupsResponseCollection res) { return res.Val("#Groups_CreatedTime", res.GroupModel.CreatedTime.ToResponse()); }
        public static GroupsResponseCollection CreatedTime(this GroupsResponseCollection res, string value) { return res.Val("#Groups_CreatedTime", value); }
        public static GroupsResponseCollection CreatedTime_FormData(this GroupsResponseCollection res) { return res.ValAndFormData("#Groups_CreatedTime", res.GroupModel.CreatedTime.ToResponse()); }
        public static GroupsResponseCollection CreatedTime_FormData(this GroupsResponseCollection res, string value) { return res.ValAndFormData("#Groups_CreatedTime", value); }
        public static GroupsResponseCollection UpdatedTime(this GroupsResponseCollection res) { return res.Val("#Groups_UpdatedTime", res.GroupModel.UpdatedTime.ToResponse()); }
        public static GroupsResponseCollection UpdatedTime(this GroupsResponseCollection res, string value) { return res.Val("#Groups_UpdatedTime", value); }
        public static GroupsResponseCollection UpdatedTime_FormData(this GroupsResponseCollection res) { return res.ValAndFormData("#Groups_UpdatedTime", res.GroupModel.UpdatedTime.ToResponse()); }
        public static GroupsResponseCollection UpdatedTime_FormData(this GroupsResponseCollection res, string value) { return res.ValAndFormData("#Groups_UpdatedTime", value); }
        public static GroupsResponseCollection Timestamp(this GroupsResponseCollection res) { return res.Val("#Groups_Timestamp", res.GroupModel.Timestamp.ToResponse()); }
        public static GroupsResponseCollection Timestamp(this GroupsResponseCollection res, string value) { return res.Val("#Groups_Timestamp", value); }
        public static GroupsResponseCollection Timestamp_FormData(this GroupsResponseCollection res) { return res.ValAndFormData("#Groups_Timestamp", res.GroupModel.Timestamp.ToResponse()); }
        public static GroupsResponseCollection Timestamp_FormData(this GroupsResponseCollection res, string value) { return res.ValAndFormData("#Groups_Timestamp", value); }
        public static GroupMembersResponseCollection Ver(this GroupMembersResponseCollection res) { return res.Val("#GroupMembers_Ver", res.GroupMemberModel.Ver.ToResponse()); }
        public static GroupMembersResponseCollection Ver(this GroupMembersResponseCollection res, string value) { return res.Val("#GroupMembers_Ver", value); }
        public static GroupMembersResponseCollection Ver_FormData(this GroupMembersResponseCollection res) { return res.ValAndFormData("#GroupMembers_Ver", res.GroupMemberModel.Ver.ToResponse()); }
        public static GroupMembersResponseCollection Ver_FormData(this GroupMembersResponseCollection res, string value) { return res.ValAndFormData("#GroupMembers_Ver", value); }
        public static GroupMembersResponseCollection Comments(this GroupMembersResponseCollection res) { return res.Val("#GroupMembers_Comments", res.GroupMemberModel.Comments.ToResponse()); }
        public static GroupMembersResponseCollection Comments(this GroupMembersResponseCollection res, string value) { return res.Val("#GroupMembers_Comments", value); }
        public static GroupMembersResponseCollection Comments_FormData(this GroupMembersResponseCollection res) { return res.ValAndFormData("#GroupMembers_Comments", res.GroupMemberModel.Comments.ToResponse()); }
        public static GroupMembersResponseCollection Comments_FormData(this GroupMembersResponseCollection res, string value) { return res.ValAndFormData("#GroupMembers_Comments", value); }
        public static GroupMembersResponseCollection CreatedTime(this GroupMembersResponseCollection res) { return res.Val("#GroupMembers_CreatedTime", res.GroupMemberModel.CreatedTime.ToResponse()); }
        public static GroupMembersResponseCollection CreatedTime(this GroupMembersResponseCollection res, string value) { return res.Val("#GroupMembers_CreatedTime", value); }
        public static GroupMembersResponseCollection CreatedTime_FormData(this GroupMembersResponseCollection res) { return res.ValAndFormData("#GroupMembers_CreatedTime", res.GroupMemberModel.CreatedTime.ToResponse()); }
        public static GroupMembersResponseCollection CreatedTime_FormData(this GroupMembersResponseCollection res, string value) { return res.ValAndFormData("#GroupMembers_CreatedTime", value); }
        public static GroupMembersResponseCollection UpdatedTime(this GroupMembersResponseCollection res) { return res.Val("#GroupMembers_UpdatedTime", res.GroupMemberModel.UpdatedTime.ToResponse()); }
        public static GroupMembersResponseCollection UpdatedTime(this GroupMembersResponseCollection res, string value) { return res.Val("#GroupMembers_UpdatedTime", value); }
        public static GroupMembersResponseCollection UpdatedTime_FormData(this GroupMembersResponseCollection res) { return res.ValAndFormData("#GroupMembers_UpdatedTime", res.GroupMemberModel.UpdatedTime.ToResponse()); }
        public static GroupMembersResponseCollection UpdatedTime_FormData(this GroupMembersResponseCollection res, string value) { return res.ValAndFormData("#GroupMembers_UpdatedTime", value); }
        public static GroupMembersResponseCollection Timestamp(this GroupMembersResponseCollection res) { return res.Val("#GroupMembers_Timestamp", res.GroupMemberModel.Timestamp.ToResponse()); }
        public static GroupMembersResponseCollection Timestamp(this GroupMembersResponseCollection res, string value) { return res.Val("#GroupMembers_Timestamp", value); }
        public static GroupMembersResponseCollection Timestamp_FormData(this GroupMembersResponseCollection res) { return res.ValAndFormData("#GroupMembers_Timestamp", res.GroupMemberModel.Timestamp.ToResponse()); }
        public static GroupMembersResponseCollection Timestamp_FormData(this GroupMembersResponseCollection res, string value) { return res.ValAndFormData("#GroupMembers_Timestamp", value); }
        public static UsersResponseCollection UserId(this UsersResponseCollection res) { return res.Val("#Users_UserId", res.UserModel.UserId.ToResponse()); }
        public static UsersResponseCollection UserId(this UsersResponseCollection res, string value) { return res.Val("#Users_UserId", value); }
        public static UsersResponseCollection UserId_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_UserId", res.UserModel.UserId.ToResponse()); }
        public static UsersResponseCollection UserId_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_UserId", value); }
        public static UsersResponseCollection Ver(this UsersResponseCollection res) { return res.Val("#Users_Ver", res.UserModel.Ver.ToResponse()); }
        public static UsersResponseCollection Ver(this UsersResponseCollection res, string value) { return res.Val("#Users_Ver", value); }
        public static UsersResponseCollection Ver_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Ver", res.UserModel.Ver.ToResponse()); }
        public static UsersResponseCollection Ver_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Ver", value); }
        public static UsersResponseCollection LoginId(this UsersResponseCollection res) { return res.Val("#Users_LoginId", res.UserModel.LoginId.ToResponse()); }
        public static UsersResponseCollection LoginId(this UsersResponseCollection res, string value) { return res.Val("#Users_LoginId", value); }
        public static UsersResponseCollection LoginId_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_LoginId", res.UserModel.LoginId.ToResponse()); }
        public static UsersResponseCollection LoginId_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_LoginId", value); }
        public static UsersResponseCollection Name(this UsersResponseCollection res) { return res.Val("#Users_Name", res.UserModel.Name.ToResponse()); }
        public static UsersResponseCollection Name(this UsersResponseCollection res, string value) { return res.Val("#Users_Name", value); }
        public static UsersResponseCollection Name_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Name", res.UserModel.Name.ToResponse()); }
        public static UsersResponseCollection Name_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Name", value); }
        public static UsersResponseCollection UserCode(this UsersResponseCollection res) { return res.Val("#Users_UserCode", res.UserModel.UserCode.ToResponse()); }
        public static UsersResponseCollection UserCode(this UsersResponseCollection res, string value) { return res.Val("#Users_UserCode", value); }
        public static UsersResponseCollection UserCode_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_UserCode", res.UserModel.UserCode.ToResponse()); }
        public static UsersResponseCollection UserCode_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_UserCode", value); }
        public static UsersResponseCollection Password(this UsersResponseCollection res) { return res.Val("#Users_Password", res.UserModel.Password.ToResponse()); }
        public static UsersResponseCollection Password(this UsersResponseCollection res, string value) { return res.Val("#Users_Password", value); }
        public static UsersResponseCollection Password_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Password", res.UserModel.Password.ToResponse()); }
        public static UsersResponseCollection Password_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Password", value); }
        public static UsersResponseCollection PasswordValidate(this UsersResponseCollection res) { return res.Val("#Users_PasswordValidate", res.UserModel.PasswordValidate.ToResponse()); }
        public static UsersResponseCollection PasswordValidate(this UsersResponseCollection res, string value) { return res.Val("#Users_PasswordValidate", value); }
        public static UsersResponseCollection PasswordValidate_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_PasswordValidate", res.UserModel.PasswordValidate.ToResponse()); }
        public static UsersResponseCollection PasswordValidate_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_PasswordValidate", value); }
        public static UsersResponseCollection PasswordDummy(this UsersResponseCollection res) { return res.Val("#Users_PasswordDummy", res.UserModel.PasswordDummy.ToResponse()); }
        public static UsersResponseCollection PasswordDummy(this UsersResponseCollection res, string value) { return res.Val("#Users_PasswordDummy", value); }
        public static UsersResponseCollection PasswordDummy_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_PasswordDummy", res.UserModel.PasswordDummy.ToResponse()); }
        public static UsersResponseCollection PasswordDummy_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_PasswordDummy", value); }
        public static UsersResponseCollection RememberMe(this UsersResponseCollection res) { return res.Val("#Users_RememberMe", res.UserModel.RememberMe.ToResponse()); }
        public static UsersResponseCollection RememberMe(this UsersResponseCollection res, string value) { return res.Val("#Users_RememberMe", value); }
        public static UsersResponseCollection RememberMe_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_RememberMe", res.UserModel.RememberMe.ToResponse()); }
        public static UsersResponseCollection RememberMe_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_RememberMe", value); }
        public static UsersResponseCollection Birthday(this UsersResponseCollection res) { return res.Val("#Users_Birthday", res.UserModel.Birthday.ToResponse()); }
        public static UsersResponseCollection Birthday(this UsersResponseCollection res, string value) { return res.Val("#Users_Birthday", value); }
        public static UsersResponseCollection Birthday_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Birthday", res.UserModel.Birthday.ToResponse()); }
        public static UsersResponseCollection Birthday_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Birthday", value); }
        public static UsersResponseCollection Gender(this UsersResponseCollection res) { return res.Val("#Users_Gender", res.UserModel.Gender.ToResponse()); }
        public static UsersResponseCollection Gender(this UsersResponseCollection res, string value) { return res.Val("#Users_Gender", value); }
        public static UsersResponseCollection Gender_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Gender", res.UserModel.Gender.ToResponse()); }
        public static UsersResponseCollection Gender_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Gender", value); }
        public static UsersResponseCollection Language(this UsersResponseCollection res) { return res.Val("#Users_Language", res.UserModel.Language.ToResponse()); }
        public static UsersResponseCollection Language(this UsersResponseCollection res, string value) { return res.Val("#Users_Language", value); }
        public static UsersResponseCollection Language_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Language", res.UserModel.Language.ToResponse()); }
        public static UsersResponseCollection Language_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Language", value); }
        public static UsersResponseCollection TimeZone(this UsersResponseCollection res) { return res.Val("#Users_TimeZone", res.UserModel.TimeZone.ToResponse()); }
        public static UsersResponseCollection TimeZone(this UsersResponseCollection res, string value) { return res.Val("#Users_TimeZone", value); }
        public static UsersResponseCollection TimeZone_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_TimeZone", res.UserModel.TimeZone.ToResponse()); }
        public static UsersResponseCollection TimeZone_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_TimeZone", value); }
        public static UsersResponseCollection DeptId(this UsersResponseCollection res) { return res.Val("#Users_DeptId", res.UserModel.DeptId.ToResponse()); }
        public static UsersResponseCollection DeptId(this UsersResponseCollection res, string value) { return res.Val("#Users_DeptId", value); }
        public static UsersResponseCollection DeptId_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_DeptId", res.UserModel.DeptId.ToResponse()); }
        public static UsersResponseCollection DeptId_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_DeptId", value); }
        public static UsersResponseCollection Body(this UsersResponseCollection res) { return res.Val("#Users_Body", res.UserModel.Body.ToResponse()); }
        public static UsersResponseCollection Body(this UsersResponseCollection res, string value) { return res.Val("#Users_Body", value); }
        public static UsersResponseCollection Body_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Body", res.UserModel.Body.ToResponse()); }
        public static UsersResponseCollection Body_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Body", value); }
        public static UsersResponseCollection LastLoginTime(this UsersResponseCollection res) { return res.Val("#Users_LastLoginTime", res.UserModel.LastLoginTime.ToResponse()); }
        public static UsersResponseCollection LastLoginTime(this UsersResponseCollection res, string value) { return res.Val("#Users_LastLoginTime", value); }
        public static UsersResponseCollection LastLoginTime_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_LastLoginTime", res.UserModel.LastLoginTime.ToResponse()); }
        public static UsersResponseCollection LastLoginTime_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_LastLoginTime", value); }
        public static UsersResponseCollection PasswordExpirationTime(this UsersResponseCollection res) { return res.Val("#Users_PasswordExpirationTime", res.UserModel.PasswordExpirationTime.ToResponse()); }
        public static UsersResponseCollection PasswordExpirationTime(this UsersResponseCollection res, string value) { return res.Val("#Users_PasswordExpirationTime", value); }
        public static UsersResponseCollection PasswordExpirationTime_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_PasswordExpirationTime", res.UserModel.PasswordExpirationTime.ToResponse()); }
        public static UsersResponseCollection PasswordExpirationTime_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_PasswordExpirationTime", value); }
        public static UsersResponseCollection PasswordChangeTime(this UsersResponseCollection res) { return res.Val("#Users_PasswordChangeTime", res.UserModel.PasswordChangeTime.ToResponse()); }
        public static UsersResponseCollection PasswordChangeTime(this UsersResponseCollection res, string value) { return res.Val("#Users_PasswordChangeTime", value); }
        public static UsersResponseCollection PasswordChangeTime_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_PasswordChangeTime", res.UserModel.PasswordChangeTime.ToResponse()); }
        public static UsersResponseCollection PasswordChangeTime_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_PasswordChangeTime", value); }
        public static UsersResponseCollection NumberOfLogins(this UsersResponseCollection res) { return res.Val("#Users_NumberOfLogins", res.UserModel.NumberOfLogins.ToResponse()); }
        public static UsersResponseCollection NumberOfLogins(this UsersResponseCollection res, string value) { return res.Val("#Users_NumberOfLogins", value); }
        public static UsersResponseCollection NumberOfLogins_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_NumberOfLogins", res.UserModel.NumberOfLogins.ToResponse()); }
        public static UsersResponseCollection NumberOfLogins_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_NumberOfLogins", value); }
        public static UsersResponseCollection NumberOfDenial(this UsersResponseCollection res) { return res.Val("#Users_NumberOfDenial", res.UserModel.NumberOfDenial.ToResponse()); }
        public static UsersResponseCollection NumberOfDenial(this UsersResponseCollection res, string value) { return res.Val("#Users_NumberOfDenial", value); }
        public static UsersResponseCollection NumberOfDenial_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_NumberOfDenial", res.UserModel.NumberOfDenial.ToResponse()); }
        public static UsersResponseCollection NumberOfDenial_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_NumberOfDenial", value); }
        public static UsersResponseCollection TenantManager(this UsersResponseCollection res) { return res.Val("#Users_TenantManager", res.UserModel.TenantManager.ToResponse()); }
        public static UsersResponseCollection TenantManager(this UsersResponseCollection res, string value) { return res.Val("#Users_TenantManager", value); }
        public static UsersResponseCollection TenantManager_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_TenantManager", res.UserModel.TenantManager.ToResponse()); }
        public static UsersResponseCollection TenantManager_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_TenantManager", value); }
        public static UsersResponseCollection Disabled(this UsersResponseCollection res) { return res.Val("#Users_Disabled", res.UserModel.Disabled.ToResponse()); }
        public static UsersResponseCollection Disabled(this UsersResponseCollection res, string value) { return res.Val("#Users_Disabled", value); }
        public static UsersResponseCollection Disabled_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Disabled", res.UserModel.Disabled.ToResponse()); }
        public static UsersResponseCollection Disabled_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Disabled", value); }
        public static UsersResponseCollection OldPassword(this UsersResponseCollection res) { return res.Val("#Users_OldPassword", res.UserModel.OldPassword.ToResponse()); }
        public static UsersResponseCollection OldPassword(this UsersResponseCollection res, string value) { return res.Val("#Users_OldPassword", value); }
        public static UsersResponseCollection OldPassword_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_OldPassword", res.UserModel.OldPassword.ToResponse()); }
        public static UsersResponseCollection OldPassword_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_OldPassword", value); }
        public static UsersResponseCollection ChangedPassword(this UsersResponseCollection res) { return res.Val("#Users_ChangedPassword", res.UserModel.ChangedPassword.ToResponse()); }
        public static UsersResponseCollection ChangedPassword(this UsersResponseCollection res, string value) { return res.Val("#Users_ChangedPassword", value); }
        public static UsersResponseCollection ChangedPassword_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_ChangedPassword", res.UserModel.ChangedPassword.ToResponse()); }
        public static UsersResponseCollection ChangedPassword_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_ChangedPassword", value); }
        public static UsersResponseCollection ChangedPasswordValidator(this UsersResponseCollection res) { return res.Val("#Users_ChangedPasswordValidator", res.UserModel.ChangedPasswordValidator.ToResponse()); }
        public static UsersResponseCollection ChangedPasswordValidator(this UsersResponseCollection res, string value) { return res.Val("#Users_ChangedPasswordValidator", value); }
        public static UsersResponseCollection ChangedPasswordValidator_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_ChangedPasswordValidator", res.UserModel.ChangedPasswordValidator.ToResponse()); }
        public static UsersResponseCollection ChangedPasswordValidator_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_ChangedPasswordValidator", value); }
        public static UsersResponseCollection AfterResetPassword(this UsersResponseCollection res) { return res.Val("#Users_AfterResetPassword", res.UserModel.AfterResetPassword.ToResponse()); }
        public static UsersResponseCollection AfterResetPassword(this UsersResponseCollection res, string value) { return res.Val("#Users_AfterResetPassword", value); }
        public static UsersResponseCollection AfterResetPassword_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_AfterResetPassword", res.UserModel.AfterResetPassword.ToResponse()); }
        public static UsersResponseCollection AfterResetPassword_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_AfterResetPassword", value); }
        public static UsersResponseCollection AfterResetPasswordValidator(this UsersResponseCollection res) { return res.Val("#Users_AfterResetPasswordValidator", res.UserModel.AfterResetPasswordValidator.ToResponse()); }
        public static UsersResponseCollection AfterResetPasswordValidator(this UsersResponseCollection res, string value) { return res.Val("#Users_AfterResetPasswordValidator", value); }
        public static UsersResponseCollection AfterResetPasswordValidator_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_AfterResetPasswordValidator", res.UserModel.AfterResetPasswordValidator.ToResponse()); }
        public static UsersResponseCollection AfterResetPasswordValidator_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_AfterResetPasswordValidator", value); }
        public static UsersResponseCollection DemoMailAddress(this UsersResponseCollection res) { return res.Val("#Users_DemoMailAddress", res.UserModel.DemoMailAddress.ToResponse()); }
        public static UsersResponseCollection DemoMailAddress(this UsersResponseCollection res, string value) { return res.Val("#Users_DemoMailAddress", value); }
        public static UsersResponseCollection DemoMailAddress_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_DemoMailAddress", res.UserModel.DemoMailAddress.ToResponse()); }
        public static UsersResponseCollection DemoMailAddress_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_DemoMailAddress", value); }
        public static UsersResponseCollection Comments(this UsersResponseCollection res) { return res.Val("#Users_Comments", res.UserModel.Comments.ToResponse()); }
        public static UsersResponseCollection Comments(this UsersResponseCollection res, string value) { return res.Val("#Users_Comments", value); }
        public static UsersResponseCollection Comments_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Comments", res.UserModel.Comments.ToResponse()); }
        public static UsersResponseCollection Comments_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Comments", value); }
        public static UsersResponseCollection CreatedTime(this UsersResponseCollection res) { return res.Val("#Users_CreatedTime", res.UserModel.CreatedTime.ToResponse()); }
        public static UsersResponseCollection CreatedTime(this UsersResponseCollection res, string value) { return res.Val("#Users_CreatedTime", value); }
        public static UsersResponseCollection CreatedTime_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_CreatedTime", res.UserModel.CreatedTime.ToResponse()); }
        public static UsersResponseCollection CreatedTime_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_CreatedTime", value); }
        public static UsersResponseCollection UpdatedTime(this UsersResponseCollection res) { return res.Val("#Users_UpdatedTime", res.UserModel.UpdatedTime.ToResponse()); }
        public static UsersResponseCollection UpdatedTime(this UsersResponseCollection res, string value) { return res.Val("#Users_UpdatedTime", value); }
        public static UsersResponseCollection UpdatedTime_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_UpdatedTime", res.UserModel.UpdatedTime.ToResponse()); }
        public static UsersResponseCollection UpdatedTime_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_UpdatedTime", value); }
        public static UsersResponseCollection Timestamp(this UsersResponseCollection res) { return res.Val("#Users_Timestamp", res.UserModel.Timestamp.ToResponse()); }
        public static UsersResponseCollection Timestamp(this UsersResponseCollection res, string value) { return res.Val("#Users_Timestamp", value); }
        public static UsersResponseCollection Timestamp_FormData(this UsersResponseCollection res) { return res.ValAndFormData("#Users_Timestamp", res.UserModel.Timestamp.ToResponse()); }
        public static UsersResponseCollection Timestamp_FormData(this UsersResponseCollection res, string value) { return res.ValAndFormData("#Users_Timestamp", value); }
        public static LoginKeysResponseCollection Ver(this LoginKeysResponseCollection res) { return res.Val("#LoginKeys_Ver", res.LoginKeyModel.Ver.ToResponse()); }
        public static LoginKeysResponseCollection Ver(this LoginKeysResponseCollection res, string value) { return res.Val("#LoginKeys_Ver", value); }
        public static LoginKeysResponseCollection Ver_FormData(this LoginKeysResponseCollection res) { return res.ValAndFormData("#LoginKeys_Ver", res.LoginKeyModel.Ver.ToResponse()); }
        public static LoginKeysResponseCollection Ver_FormData(this LoginKeysResponseCollection res, string value) { return res.ValAndFormData("#LoginKeys_Ver", value); }
        public static LoginKeysResponseCollection Comments(this LoginKeysResponseCollection res) { return res.Val("#LoginKeys_Comments", res.LoginKeyModel.Comments.ToResponse()); }
        public static LoginKeysResponseCollection Comments(this LoginKeysResponseCollection res, string value) { return res.Val("#LoginKeys_Comments", value); }
        public static LoginKeysResponseCollection Comments_FormData(this LoginKeysResponseCollection res) { return res.ValAndFormData("#LoginKeys_Comments", res.LoginKeyModel.Comments.ToResponse()); }
        public static LoginKeysResponseCollection Comments_FormData(this LoginKeysResponseCollection res, string value) { return res.ValAndFormData("#LoginKeys_Comments", value); }
        public static LoginKeysResponseCollection CreatedTime(this LoginKeysResponseCollection res) { return res.Val("#LoginKeys_CreatedTime", res.LoginKeyModel.CreatedTime.ToResponse()); }
        public static LoginKeysResponseCollection CreatedTime(this LoginKeysResponseCollection res, string value) { return res.Val("#LoginKeys_CreatedTime", value); }
        public static LoginKeysResponseCollection CreatedTime_FormData(this LoginKeysResponseCollection res) { return res.ValAndFormData("#LoginKeys_CreatedTime", res.LoginKeyModel.CreatedTime.ToResponse()); }
        public static LoginKeysResponseCollection CreatedTime_FormData(this LoginKeysResponseCollection res, string value) { return res.ValAndFormData("#LoginKeys_CreatedTime", value); }
        public static LoginKeysResponseCollection UpdatedTime(this LoginKeysResponseCollection res) { return res.Val("#LoginKeys_UpdatedTime", res.LoginKeyModel.UpdatedTime.ToResponse()); }
        public static LoginKeysResponseCollection UpdatedTime(this LoginKeysResponseCollection res, string value) { return res.Val("#LoginKeys_UpdatedTime", value); }
        public static LoginKeysResponseCollection UpdatedTime_FormData(this LoginKeysResponseCollection res) { return res.ValAndFormData("#LoginKeys_UpdatedTime", res.LoginKeyModel.UpdatedTime.ToResponse()); }
        public static LoginKeysResponseCollection UpdatedTime_FormData(this LoginKeysResponseCollection res, string value) { return res.ValAndFormData("#LoginKeys_UpdatedTime", value); }
        public static LoginKeysResponseCollection Timestamp(this LoginKeysResponseCollection res) { return res.Val("#LoginKeys_Timestamp", res.LoginKeyModel.Timestamp.ToResponse()); }
        public static LoginKeysResponseCollection Timestamp(this LoginKeysResponseCollection res, string value) { return res.Val("#LoginKeys_Timestamp", value); }
        public static LoginKeysResponseCollection Timestamp_FormData(this LoginKeysResponseCollection res) { return res.ValAndFormData("#LoginKeys_Timestamp", res.LoginKeyModel.Timestamp.ToResponse()); }
        public static LoginKeysResponseCollection Timestamp_FormData(this LoginKeysResponseCollection res, string value) { return res.ValAndFormData("#LoginKeys_Timestamp", value); }
        public static MailAddressesResponseCollection OwnerId(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_OwnerId", res.MailAddressModel.OwnerId.ToResponse()); }
        public static MailAddressesResponseCollection OwnerId(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_OwnerId", value); }
        public static MailAddressesResponseCollection OwnerId_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_OwnerId", res.MailAddressModel.OwnerId.ToResponse()); }
        public static MailAddressesResponseCollection OwnerId_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_OwnerId", value); }
        public static MailAddressesResponseCollection OwnerType(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_OwnerType", res.MailAddressModel.OwnerType.ToResponse()); }
        public static MailAddressesResponseCollection OwnerType(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_OwnerType", value); }
        public static MailAddressesResponseCollection OwnerType_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_OwnerType", res.MailAddressModel.OwnerType.ToResponse()); }
        public static MailAddressesResponseCollection OwnerType_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_OwnerType", value); }
        public static MailAddressesResponseCollection MailAddressId(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_MailAddressId", res.MailAddressModel.MailAddressId.ToResponse()); }
        public static MailAddressesResponseCollection MailAddressId(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_MailAddressId", value); }
        public static MailAddressesResponseCollection MailAddressId_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_MailAddressId", res.MailAddressModel.MailAddressId.ToResponse()); }
        public static MailAddressesResponseCollection MailAddressId_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_MailAddressId", value); }
        public static MailAddressesResponseCollection Ver(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_Ver", res.MailAddressModel.Ver.ToResponse()); }
        public static MailAddressesResponseCollection Ver(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_Ver", value); }
        public static MailAddressesResponseCollection Ver_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_Ver", res.MailAddressModel.Ver.ToResponse()); }
        public static MailAddressesResponseCollection Ver_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_Ver", value); }
        public static MailAddressesResponseCollection MailAddress(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_MailAddress", res.MailAddressModel.MailAddress.ToResponse()); }
        public static MailAddressesResponseCollection MailAddress(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_MailAddress", value); }
        public static MailAddressesResponseCollection MailAddress_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_MailAddress", res.MailAddressModel.MailAddress.ToResponse()); }
        public static MailAddressesResponseCollection MailAddress_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_MailAddress", value); }
        public static MailAddressesResponseCollection Title(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_Title", res.MailAddressModel.Title.ToResponse()); }
        public static MailAddressesResponseCollection Title(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_Title", value); }
        public static MailAddressesResponseCollection Title_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_Title", res.MailAddressModel.Title.ToResponse()); }
        public static MailAddressesResponseCollection Title_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_Title", value); }
        public static MailAddressesResponseCollection Comments(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_Comments", res.MailAddressModel.Comments.ToResponse()); }
        public static MailAddressesResponseCollection Comments(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_Comments", value); }
        public static MailAddressesResponseCollection Comments_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_Comments", res.MailAddressModel.Comments.ToResponse()); }
        public static MailAddressesResponseCollection Comments_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_Comments", value); }
        public static MailAddressesResponseCollection CreatedTime(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_CreatedTime", res.MailAddressModel.CreatedTime.ToResponse()); }
        public static MailAddressesResponseCollection CreatedTime(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_CreatedTime", value); }
        public static MailAddressesResponseCollection CreatedTime_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_CreatedTime", res.MailAddressModel.CreatedTime.ToResponse()); }
        public static MailAddressesResponseCollection CreatedTime_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_CreatedTime", value); }
        public static MailAddressesResponseCollection UpdatedTime(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_UpdatedTime", res.MailAddressModel.UpdatedTime.ToResponse()); }
        public static MailAddressesResponseCollection UpdatedTime(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_UpdatedTime", value); }
        public static MailAddressesResponseCollection UpdatedTime_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_UpdatedTime", res.MailAddressModel.UpdatedTime.ToResponse()); }
        public static MailAddressesResponseCollection UpdatedTime_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_UpdatedTime", value); }
        public static MailAddressesResponseCollection Timestamp(this MailAddressesResponseCollection res) { return res.Val("#MailAddresses_Timestamp", res.MailAddressModel.Timestamp.ToResponse()); }
        public static MailAddressesResponseCollection Timestamp(this MailAddressesResponseCollection res, string value) { return res.Val("#MailAddresses_Timestamp", value); }
        public static MailAddressesResponseCollection Timestamp_FormData(this MailAddressesResponseCollection res) { return res.ValAndFormData("#MailAddresses_Timestamp", res.MailAddressModel.Timestamp.ToResponse()); }
        public static MailAddressesResponseCollection Timestamp_FormData(this MailAddressesResponseCollection res, string value) { return res.ValAndFormData("#MailAddresses_Timestamp", value); }
        public static OutgoingMailsResponseCollection ReferenceType(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_ReferenceType", res.OutgoingMailModel.ReferenceType.ToResponse()); }
        public static OutgoingMailsResponseCollection ReferenceType(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_ReferenceType", value); }
        public static OutgoingMailsResponseCollection ReferenceType_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_ReferenceType", res.OutgoingMailModel.ReferenceType.ToResponse()); }
        public static OutgoingMailsResponseCollection ReferenceType_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_ReferenceType", value); }
        public static OutgoingMailsResponseCollection ReferenceId(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_ReferenceId", res.OutgoingMailModel.ReferenceId.ToResponse()); }
        public static OutgoingMailsResponseCollection ReferenceId(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_ReferenceId", value); }
        public static OutgoingMailsResponseCollection ReferenceId_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_ReferenceId", res.OutgoingMailModel.ReferenceId.ToResponse()); }
        public static OutgoingMailsResponseCollection ReferenceId_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_ReferenceId", value); }
        public static OutgoingMailsResponseCollection OutgoingMailId(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_OutgoingMailId", res.OutgoingMailModel.OutgoingMailId.ToResponse()); }
        public static OutgoingMailsResponseCollection OutgoingMailId(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_OutgoingMailId", value); }
        public static OutgoingMailsResponseCollection OutgoingMailId_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_OutgoingMailId", res.OutgoingMailModel.OutgoingMailId.ToResponse()); }
        public static OutgoingMailsResponseCollection OutgoingMailId_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_OutgoingMailId", value); }
        public static OutgoingMailsResponseCollection Ver(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_Ver", res.OutgoingMailModel.Ver.ToResponse()); }
        public static OutgoingMailsResponseCollection Ver(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_Ver", value); }
        public static OutgoingMailsResponseCollection Ver_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_Ver", res.OutgoingMailModel.Ver.ToResponse()); }
        public static OutgoingMailsResponseCollection Ver_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_Ver", value); }
        public static OutgoingMailsResponseCollection To(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_To", res.OutgoingMailModel.To.ToResponse()); }
        public static OutgoingMailsResponseCollection To(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_To", value); }
        public static OutgoingMailsResponseCollection To_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_To", res.OutgoingMailModel.To.ToResponse()); }
        public static OutgoingMailsResponseCollection To_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_To", value); }
        public static OutgoingMailsResponseCollection Cc(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_Cc", res.OutgoingMailModel.Cc.ToResponse()); }
        public static OutgoingMailsResponseCollection Cc(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_Cc", value); }
        public static OutgoingMailsResponseCollection Cc_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_Cc", res.OutgoingMailModel.Cc.ToResponse()); }
        public static OutgoingMailsResponseCollection Cc_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_Cc", value); }
        public static OutgoingMailsResponseCollection Bcc(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_Bcc", res.OutgoingMailModel.Bcc.ToResponse()); }
        public static OutgoingMailsResponseCollection Bcc(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_Bcc", value); }
        public static OutgoingMailsResponseCollection Bcc_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_Bcc", res.OutgoingMailModel.Bcc.ToResponse()); }
        public static OutgoingMailsResponseCollection Bcc_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_Bcc", value); }
        public static OutgoingMailsResponseCollection Title(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_Title", res.OutgoingMailModel.Title.ToResponse()); }
        public static OutgoingMailsResponseCollection Title(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_Title", value); }
        public static OutgoingMailsResponseCollection Title_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_Title", res.OutgoingMailModel.Title.ToResponse()); }
        public static OutgoingMailsResponseCollection Title_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_Title", value); }
        public static OutgoingMailsResponseCollection Body(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_Body", res.OutgoingMailModel.Body.ToResponse()); }
        public static OutgoingMailsResponseCollection Body(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_Body", value); }
        public static OutgoingMailsResponseCollection Body_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_Body", res.OutgoingMailModel.Body.ToResponse()); }
        public static OutgoingMailsResponseCollection Body_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_Body", value); }
        public static OutgoingMailsResponseCollection SentTime(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_SentTime", res.OutgoingMailModel.SentTime.ToResponse()); }
        public static OutgoingMailsResponseCollection SentTime(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_SentTime", value); }
        public static OutgoingMailsResponseCollection SentTime_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_SentTime", res.OutgoingMailModel.SentTime.ToResponse()); }
        public static OutgoingMailsResponseCollection SentTime_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_SentTime", value); }
        public static OutgoingMailsResponseCollection DestinationSearchRange(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_DestinationSearchRange", res.OutgoingMailModel.DestinationSearchRange.ToResponse()); }
        public static OutgoingMailsResponseCollection DestinationSearchRange(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_DestinationSearchRange", value); }
        public static OutgoingMailsResponseCollection DestinationSearchRange_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_DestinationSearchRange", res.OutgoingMailModel.DestinationSearchRange.ToResponse()); }
        public static OutgoingMailsResponseCollection DestinationSearchRange_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_DestinationSearchRange", value); }
        public static OutgoingMailsResponseCollection DestinationSearchText(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_DestinationSearchText", res.OutgoingMailModel.DestinationSearchText.ToResponse()); }
        public static OutgoingMailsResponseCollection DestinationSearchText(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_DestinationSearchText", value); }
        public static OutgoingMailsResponseCollection DestinationSearchText_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_DestinationSearchText", res.OutgoingMailModel.DestinationSearchText.ToResponse()); }
        public static OutgoingMailsResponseCollection DestinationSearchText_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_DestinationSearchText", value); }
        public static OutgoingMailsResponseCollection Comments(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_Comments", res.OutgoingMailModel.Comments.ToResponse()); }
        public static OutgoingMailsResponseCollection Comments(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_Comments", value); }
        public static OutgoingMailsResponseCollection Comments_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_Comments", res.OutgoingMailModel.Comments.ToResponse()); }
        public static OutgoingMailsResponseCollection Comments_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_Comments", value); }
        public static OutgoingMailsResponseCollection CreatedTime(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_CreatedTime", res.OutgoingMailModel.CreatedTime.ToResponse()); }
        public static OutgoingMailsResponseCollection CreatedTime(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_CreatedTime", value); }
        public static OutgoingMailsResponseCollection CreatedTime_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_CreatedTime", res.OutgoingMailModel.CreatedTime.ToResponse()); }
        public static OutgoingMailsResponseCollection CreatedTime_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_CreatedTime", value); }
        public static OutgoingMailsResponseCollection UpdatedTime(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_UpdatedTime", res.OutgoingMailModel.UpdatedTime.ToResponse()); }
        public static OutgoingMailsResponseCollection UpdatedTime(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_UpdatedTime", value); }
        public static OutgoingMailsResponseCollection UpdatedTime_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_UpdatedTime", res.OutgoingMailModel.UpdatedTime.ToResponse()); }
        public static OutgoingMailsResponseCollection UpdatedTime_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_UpdatedTime", value); }
        public static OutgoingMailsResponseCollection Timestamp(this OutgoingMailsResponseCollection res) { return res.Val("#OutgoingMails_Timestamp", res.OutgoingMailModel.Timestamp.ToResponse()); }
        public static OutgoingMailsResponseCollection Timestamp(this OutgoingMailsResponseCollection res, string value) { return res.Val("#OutgoingMails_Timestamp", value); }
        public static OutgoingMailsResponseCollection Timestamp_FormData(this OutgoingMailsResponseCollection res) { return res.ValAndFormData("#OutgoingMails_Timestamp", res.OutgoingMailModel.Timestamp.ToResponse()); }
        public static OutgoingMailsResponseCollection Timestamp_FormData(this OutgoingMailsResponseCollection res, string value) { return res.ValAndFormData("#OutgoingMails_Timestamp", value); }
        public static SearchIndexesResponseCollection Ver(this SearchIndexesResponseCollection res) { return res.Val("#SearchIndexes_Ver", res.SearchIndexModel.Ver.ToResponse()); }
        public static SearchIndexesResponseCollection Ver(this SearchIndexesResponseCollection res, string value) { return res.Val("#SearchIndexes_Ver", value); }
        public static SearchIndexesResponseCollection Ver_FormData(this SearchIndexesResponseCollection res) { return res.ValAndFormData("#SearchIndexes_Ver", res.SearchIndexModel.Ver.ToResponse()); }
        public static SearchIndexesResponseCollection Ver_FormData(this SearchIndexesResponseCollection res, string value) { return res.ValAndFormData("#SearchIndexes_Ver", value); }
        public static SearchIndexesResponseCollection Comments(this SearchIndexesResponseCollection res) { return res.Val("#SearchIndexes_Comments", res.SearchIndexModel.Comments.ToResponse()); }
        public static SearchIndexesResponseCollection Comments(this SearchIndexesResponseCollection res, string value) { return res.Val("#SearchIndexes_Comments", value); }
        public static SearchIndexesResponseCollection Comments_FormData(this SearchIndexesResponseCollection res) { return res.ValAndFormData("#SearchIndexes_Comments", res.SearchIndexModel.Comments.ToResponse()); }
        public static SearchIndexesResponseCollection Comments_FormData(this SearchIndexesResponseCollection res, string value) { return res.ValAndFormData("#SearchIndexes_Comments", value); }
        public static SearchIndexesResponseCollection CreatedTime(this SearchIndexesResponseCollection res) { return res.Val("#SearchIndexes_CreatedTime", res.SearchIndexModel.CreatedTime.ToResponse()); }
        public static SearchIndexesResponseCollection CreatedTime(this SearchIndexesResponseCollection res, string value) { return res.Val("#SearchIndexes_CreatedTime", value); }
        public static SearchIndexesResponseCollection CreatedTime_FormData(this SearchIndexesResponseCollection res) { return res.ValAndFormData("#SearchIndexes_CreatedTime", res.SearchIndexModel.CreatedTime.ToResponse()); }
        public static SearchIndexesResponseCollection CreatedTime_FormData(this SearchIndexesResponseCollection res, string value) { return res.ValAndFormData("#SearchIndexes_CreatedTime", value); }
        public static SearchIndexesResponseCollection UpdatedTime(this SearchIndexesResponseCollection res) { return res.Val("#SearchIndexes_UpdatedTime", res.SearchIndexModel.UpdatedTime.ToResponse()); }
        public static SearchIndexesResponseCollection UpdatedTime(this SearchIndexesResponseCollection res, string value) { return res.Val("#SearchIndexes_UpdatedTime", value); }
        public static SearchIndexesResponseCollection UpdatedTime_FormData(this SearchIndexesResponseCollection res) { return res.ValAndFormData("#SearchIndexes_UpdatedTime", res.SearchIndexModel.UpdatedTime.ToResponse()); }
        public static SearchIndexesResponseCollection UpdatedTime_FormData(this SearchIndexesResponseCollection res, string value) { return res.ValAndFormData("#SearchIndexes_UpdatedTime", value); }
        public static SearchIndexesResponseCollection Timestamp(this SearchIndexesResponseCollection res) { return res.Val("#SearchIndexes_Timestamp", res.SearchIndexModel.Timestamp.ToResponse()); }
        public static SearchIndexesResponseCollection Timestamp(this SearchIndexesResponseCollection res, string value) { return res.Val("#SearchIndexes_Timestamp", value); }
        public static SearchIndexesResponseCollection Timestamp_FormData(this SearchIndexesResponseCollection res) { return res.ValAndFormData("#SearchIndexes_Timestamp", res.SearchIndexModel.Timestamp.ToResponse()); }
        public static SearchIndexesResponseCollection Timestamp_FormData(this SearchIndexesResponseCollection res, string value) { return res.ValAndFormData("#SearchIndexes_Timestamp", value); }
        public static SitesResponseCollection SiteId(this SitesResponseCollection res) { return res.Val("#Sites_SiteId", res.SiteModel.SiteId.ToResponse()); }
        public static SitesResponseCollection SiteId(this SitesResponseCollection res, string value) { return res.Val("#Sites_SiteId", value); }
        public static SitesResponseCollection SiteId_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_SiteId", res.SiteModel.SiteId.ToResponse()); }
        public static SitesResponseCollection SiteId_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_SiteId", value); }
        public static SitesResponseCollection UpdatedTime(this SitesResponseCollection res) { return res.Val("#Sites_UpdatedTime", res.SiteModel.UpdatedTime.ToResponse()); }
        public static SitesResponseCollection UpdatedTime(this SitesResponseCollection res, string value) { return res.Val("#Sites_UpdatedTime", value); }
        public static SitesResponseCollection UpdatedTime_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_UpdatedTime", res.SiteModel.UpdatedTime.ToResponse()); }
        public static SitesResponseCollection UpdatedTime_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_UpdatedTime", value); }
        public static SitesResponseCollection Ver(this SitesResponseCollection res) { return res.Val("#Sites_Ver", res.SiteModel.Ver.ToResponse()); }
        public static SitesResponseCollection Ver(this SitesResponseCollection res, string value) { return res.Val("#Sites_Ver", value); }
        public static SitesResponseCollection Ver_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_Ver", res.SiteModel.Ver.ToResponse()); }
        public static SitesResponseCollection Ver_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_Ver", value); }
        public static SitesResponseCollection Title(this SitesResponseCollection res) { return res.Val("#Sites_Title", res.SiteModel.Title.ToResponse()); }
        public static SitesResponseCollection Title(this SitesResponseCollection res, string value) { return res.Val("#Sites_Title", value); }
        public static SitesResponseCollection Title_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_Title", res.SiteModel.Title.ToResponse()); }
        public static SitesResponseCollection Title_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_Title", value); }
        public static SitesResponseCollection Body(this SitesResponseCollection res) { return res.Val("#Sites_Body", res.SiteModel.Body.ToResponse()); }
        public static SitesResponseCollection Body(this SitesResponseCollection res, string value) { return res.Val("#Sites_Body", value); }
        public static SitesResponseCollection Body_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_Body", res.SiteModel.Body.ToResponse()); }
        public static SitesResponseCollection Body_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_Body", value); }
        public static SitesResponseCollection ReferenceType(this SitesResponseCollection res) { return res.Val("#Sites_ReferenceType", res.SiteModel.ReferenceType.ToResponse()); }
        public static SitesResponseCollection ReferenceType(this SitesResponseCollection res, string value) { return res.Val("#Sites_ReferenceType", value); }
        public static SitesResponseCollection ReferenceType_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_ReferenceType", res.SiteModel.ReferenceType.ToResponse()); }
        public static SitesResponseCollection ReferenceType_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_ReferenceType", value); }
        public static SitesResponseCollection InheritPermission(this SitesResponseCollection res) { return res.Val("#Sites_InheritPermission", res.SiteModel.InheritPermission.ToResponse()); }
        public static SitesResponseCollection InheritPermission(this SitesResponseCollection res, string value) { return res.Val("#Sites_InheritPermission", value); }
        public static SitesResponseCollection InheritPermission_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_InheritPermission", res.SiteModel.InheritPermission.ToResponse()); }
        public static SitesResponseCollection InheritPermission_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_InheritPermission", value); }
        public static SitesResponseCollection Comments(this SitesResponseCollection res) { return res.Val("#Sites_Comments", res.SiteModel.Comments.ToResponse()); }
        public static SitesResponseCollection Comments(this SitesResponseCollection res, string value) { return res.Val("#Sites_Comments", value); }
        public static SitesResponseCollection Comments_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_Comments", res.SiteModel.Comments.ToResponse()); }
        public static SitesResponseCollection Comments_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_Comments", value); }
        public static SitesResponseCollection CreatedTime(this SitesResponseCollection res) { return res.Val("#Sites_CreatedTime", res.SiteModel.CreatedTime.ToResponse()); }
        public static SitesResponseCollection CreatedTime(this SitesResponseCollection res, string value) { return res.Val("#Sites_CreatedTime", value); }
        public static SitesResponseCollection CreatedTime_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_CreatedTime", res.SiteModel.CreatedTime.ToResponse()); }
        public static SitesResponseCollection CreatedTime_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_CreatedTime", value); }
        public static SitesResponseCollection Timestamp(this SitesResponseCollection res) { return res.Val("#Sites_Timestamp", res.SiteModel.Timestamp.ToResponse()); }
        public static SitesResponseCollection Timestamp(this SitesResponseCollection res, string value) { return res.Val("#Sites_Timestamp", value); }
        public static SitesResponseCollection Timestamp_FormData(this SitesResponseCollection res) { return res.ValAndFormData("#Sites_Timestamp", res.SiteModel.Timestamp.ToResponse()); }
        public static SitesResponseCollection Timestamp_FormData(this SitesResponseCollection res, string value) { return res.ValAndFormData("#Sites_Timestamp", value); }
        public static OrdersResponseCollection Ver(this OrdersResponseCollection res) { return res.Val("#Orders_Ver", res.OrderModel.Ver.ToResponse()); }
        public static OrdersResponseCollection Ver(this OrdersResponseCollection res, string value) { return res.Val("#Orders_Ver", value); }
        public static OrdersResponseCollection Ver_FormData(this OrdersResponseCollection res) { return res.ValAndFormData("#Orders_Ver", res.OrderModel.Ver.ToResponse()); }
        public static OrdersResponseCollection Ver_FormData(this OrdersResponseCollection res, string value) { return res.ValAndFormData("#Orders_Ver", value); }
        public static OrdersResponseCollection Comments(this OrdersResponseCollection res) { return res.Val("#Orders_Comments", res.OrderModel.Comments.ToResponse()); }
        public static OrdersResponseCollection Comments(this OrdersResponseCollection res, string value) { return res.Val("#Orders_Comments", value); }
        public static OrdersResponseCollection Comments_FormData(this OrdersResponseCollection res) { return res.ValAndFormData("#Orders_Comments", res.OrderModel.Comments.ToResponse()); }
        public static OrdersResponseCollection Comments_FormData(this OrdersResponseCollection res, string value) { return res.ValAndFormData("#Orders_Comments", value); }
        public static OrdersResponseCollection CreatedTime(this OrdersResponseCollection res) { return res.Val("#Orders_CreatedTime", res.OrderModel.CreatedTime.ToResponse()); }
        public static OrdersResponseCollection CreatedTime(this OrdersResponseCollection res, string value) { return res.Val("#Orders_CreatedTime", value); }
        public static OrdersResponseCollection CreatedTime_FormData(this OrdersResponseCollection res) { return res.ValAndFormData("#Orders_CreatedTime", res.OrderModel.CreatedTime.ToResponse()); }
        public static OrdersResponseCollection CreatedTime_FormData(this OrdersResponseCollection res, string value) { return res.ValAndFormData("#Orders_CreatedTime", value); }
        public static OrdersResponseCollection UpdatedTime(this OrdersResponseCollection res) { return res.Val("#Orders_UpdatedTime", res.OrderModel.UpdatedTime.ToResponse()); }
        public static OrdersResponseCollection UpdatedTime(this OrdersResponseCollection res, string value) { return res.Val("#Orders_UpdatedTime", value); }
        public static OrdersResponseCollection UpdatedTime_FormData(this OrdersResponseCollection res) { return res.ValAndFormData("#Orders_UpdatedTime", res.OrderModel.UpdatedTime.ToResponse()); }
        public static OrdersResponseCollection UpdatedTime_FormData(this OrdersResponseCollection res, string value) { return res.ValAndFormData("#Orders_UpdatedTime", value); }
        public static OrdersResponseCollection Timestamp(this OrdersResponseCollection res) { return res.Val("#Orders_Timestamp", res.OrderModel.Timestamp.ToResponse()); }
        public static OrdersResponseCollection Timestamp(this OrdersResponseCollection res, string value) { return res.Val("#Orders_Timestamp", value); }
        public static OrdersResponseCollection Timestamp_FormData(this OrdersResponseCollection res) { return res.ValAndFormData("#Orders_Timestamp", res.OrderModel.Timestamp.ToResponse()); }
        public static OrdersResponseCollection Timestamp_FormData(this OrdersResponseCollection res, string value) { return res.ValAndFormData("#Orders_Timestamp", value); }
        public static ExportSettingsResponseCollection ReferenceType(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_ReferenceType", res.ExportSettingModel.ReferenceType.ToResponse()); }
        public static ExportSettingsResponseCollection ReferenceType(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_ReferenceType", value); }
        public static ExportSettingsResponseCollection ReferenceType_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_ReferenceType", res.ExportSettingModel.ReferenceType.ToResponse()); }
        public static ExportSettingsResponseCollection ReferenceType_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_ReferenceType", value); }
        public static ExportSettingsResponseCollection ReferenceId(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_ReferenceId", res.ExportSettingModel.ReferenceId.ToResponse()); }
        public static ExportSettingsResponseCollection ReferenceId(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_ReferenceId", value); }
        public static ExportSettingsResponseCollection ReferenceId_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_ReferenceId", res.ExportSettingModel.ReferenceId.ToResponse()); }
        public static ExportSettingsResponseCollection ReferenceId_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_ReferenceId", value); }
        public static ExportSettingsResponseCollection Title(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_Title", res.ExportSettingModel.Title.ToResponse()); }
        public static ExportSettingsResponseCollection Title(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_Title", value); }
        public static ExportSettingsResponseCollection Title_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_Title", res.ExportSettingModel.Title.ToResponse()); }
        public static ExportSettingsResponseCollection Title_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_Title", value); }
        public static ExportSettingsResponseCollection ExportSettingId(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_ExportSettingId", res.ExportSettingModel.ExportSettingId.ToResponse()); }
        public static ExportSettingsResponseCollection ExportSettingId(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_ExportSettingId", value); }
        public static ExportSettingsResponseCollection ExportSettingId_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_ExportSettingId", res.ExportSettingModel.ExportSettingId.ToResponse()); }
        public static ExportSettingsResponseCollection ExportSettingId_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_ExportSettingId", value); }
        public static ExportSettingsResponseCollection Ver(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_Ver", res.ExportSettingModel.Ver.ToResponse()); }
        public static ExportSettingsResponseCollection Ver(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_Ver", value); }
        public static ExportSettingsResponseCollection Ver_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_Ver", res.ExportSettingModel.Ver.ToResponse()); }
        public static ExportSettingsResponseCollection Ver_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_Ver", value); }
        public static ExportSettingsResponseCollection AddHeader(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_AddHeader", res.ExportSettingModel.AddHeader.ToResponse()); }
        public static ExportSettingsResponseCollection AddHeader(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_AddHeader", value); }
        public static ExportSettingsResponseCollection AddHeader_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_AddHeader", res.ExportSettingModel.AddHeader.ToResponse()); }
        public static ExportSettingsResponseCollection AddHeader_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_AddHeader", value); }
        public static ExportSettingsResponseCollection Comments(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_Comments", res.ExportSettingModel.Comments.ToResponse()); }
        public static ExportSettingsResponseCollection Comments(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_Comments", value); }
        public static ExportSettingsResponseCollection Comments_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_Comments", res.ExportSettingModel.Comments.ToResponse()); }
        public static ExportSettingsResponseCollection Comments_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_Comments", value); }
        public static ExportSettingsResponseCollection CreatedTime(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_CreatedTime", res.ExportSettingModel.CreatedTime.ToResponse()); }
        public static ExportSettingsResponseCollection CreatedTime(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_CreatedTime", value); }
        public static ExportSettingsResponseCollection CreatedTime_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_CreatedTime", res.ExportSettingModel.CreatedTime.ToResponse()); }
        public static ExportSettingsResponseCollection CreatedTime_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_CreatedTime", value); }
        public static ExportSettingsResponseCollection UpdatedTime(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_UpdatedTime", res.ExportSettingModel.UpdatedTime.ToResponse()); }
        public static ExportSettingsResponseCollection UpdatedTime(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_UpdatedTime", value); }
        public static ExportSettingsResponseCollection UpdatedTime_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_UpdatedTime", res.ExportSettingModel.UpdatedTime.ToResponse()); }
        public static ExportSettingsResponseCollection UpdatedTime_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_UpdatedTime", value); }
        public static ExportSettingsResponseCollection Timestamp(this ExportSettingsResponseCollection res) { return res.Val("#ExportSettings_Timestamp", res.ExportSettingModel.Timestamp.ToResponse()); }
        public static ExportSettingsResponseCollection Timestamp(this ExportSettingsResponseCollection res, string value) { return res.Val("#ExportSettings_Timestamp", value); }
        public static ExportSettingsResponseCollection Timestamp_FormData(this ExportSettingsResponseCollection res) { return res.ValAndFormData("#ExportSettings_Timestamp", res.ExportSettingModel.Timestamp.ToResponse()); }
        public static ExportSettingsResponseCollection Timestamp_FormData(this ExportSettingsResponseCollection res, string value) { return res.ValAndFormData("#ExportSettings_Timestamp", value); }
        public static LinksResponseCollection Ver(this LinksResponseCollection res) { return res.Val("#Links_Ver", res.LinkModel.Ver.ToResponse()); }
        public static LinksResponseCollection Ver(this LinksResponseCollection res, string value) { return res.Val("#Links_Ver", value); }
        public static LinksResponseCollection Ver_FormData(this LinksResponseCollection res) { return res.ValAndFormData("#Links_Ver", res.LinkModel.Ver.ToResponse()); }
        public static LinksResponseCollection Ver_FormData(this LinksResponseCollection res, string value) { return res.ValAndFormData("#Links_Ver", value); }
        public static LinksResponseCollection Comments(this LinksResponseCollection res) { return res.Val("#Links_Comments", res.LinkModel.Comments.ToResponse()); }
        public static LinksResponseCollection Comments(this LinksResponseCollection res, string value) { return res.Val("#Links_Comments", value); }
        public static LinksResponseCollection Comments_FormData(this LinksResponseCollection res) { return res.ValAndFormData("#Links_Comments", res.LinkModel.Comments.ToResponse()); }
        public static LinksResponseCollection Comments_FormData(this LinksResponseCollection res, string value) { return res.ValAndFormData("#Links_Comments", value); }
        public static LinksResponseCollection CreatedTime(this LinksResponseCollection res) { return res.Val("#Links_CreatedTime", res.LinkModel.CreatedTime.ToResponse()); }
        public static LinksResponseCollection CreatedTime(this LinksResponseCollection res, string value) { return res.Val("#Links_CreatedTime", value); }
        public static LinksResponseCollection CreatedTime_FormData(this LinksResponseCollection res) { return res.ValAndFormData("#Links_CreatedTime", res.LinkModel.CreatedTime.ToResponse()); }
        public static LinksResponseCollection CreatedTime_FormData(this LinksResponseCollection res, string value) { return res.ValAndFormData("#Links_CreatedTime", value); }
        public static LinksResponseCollection UpdatedTime(this LinksResponseCollection res) { return res.Val("#Links_UpdatedTime", res.LinkModel.UpdatedTime.ToResponse()); }
        public static LinksResponseCollection UpdatedTime(this LinksResponseCollection res, string value) { return res.Val("#Links_UpdatedTime", value); }
        public static LinksResponseCollection UpdatedTime_FormData(this LinksResponseCollection res) { return res.ValAndFormData("#Links_UpdatedTime", res.LinkModel.UpdatedTime.ToResponse()); }
        public static LinksResponseCollection UpdatedTime_FormData(this LinksResponseCollection res, string value) { return res.ValAndFormData("#Links_UpdatedTime", value); }
        public static LinksResponseCollection Timestamp(this LinksResponseCollection res) { return res.Val("#Links_Timestamp", res.LinkModel.Timestamp.ToResponse()); }
        public static LinksResponseCollection Timestamp(this LinksResponseCollection res, string value) { return res.Val("#Links_Timestamp", value); }
        public static LinksResponseCollection Timestamp_FormData(this LinksResponseCollection res) { return res.ValAndFormData("#Links_Timestamp", res.LinkModel.Timestamp.ToResponse()); }
        public static LinksResponseCollection Timestamp_FormData(this LinksResponseCollection res, string value) { return res.ValAndFormData("#Links_Timestamp", value); }
        public static BinariesResponseCollection Ver(this BinariesResponseCollection res) { return res.Val("#Binaries_Ver", res.BinaryModel.Ver.ToResponse()); }
        public static BinariesResponseCollection Ver(this BinariesResponseCollection res, string value) { return res.Val("#Binaries_Ver", value); }
        public static BinariesResponseCollection Ver_FormData(this BinariesResponseCollection res) { return res.ValAndFormData("#Binaries_Ver", res.BinaryModel.Ver.ToResponse()); }
        public static BinariesResponseCollection Ver_FormData(this BinariesResponseCollection res, string value) { return res.ValAndFormData("#Binaries_Ver", value); }
        public static BinariesResponseCollection Comments(this BinariesResponseCollection res) { return res.Val("#Binaries_Comments", res.BinaryModel.Comments.ToResponse()); }
        public static BinariesResponseCollection Comments(this BinariesResponseCollection res, string value) { return res.Val("#Binaries_Comments", value); }
        public static BinariesResponseCollection Comments_FormData(this BinariesResponseCollection res) { return res.ValAndFormData("#Binaries_Comments", res.BinaryModel.Comments.ToResponse()); }
        public static BinariesResponseCollection Comments_FormData(this BinariesResponseCollection res, string value) { return res.ValAndFormData("#Binaries_Comments", value); }
        public static BinariesResponseCollection CreatedTime(this BinariesResponseCollection res) { return res.Val("#Binaries_CreatedTime", res.BinaryModel.CreatedTime.ToResponse()); }
        public static BinariesResponseCollection CreatedTime(this BinariesResponseCollection res, string value) { return res.Val("#Binaries_CreatedTime", value); }
        public static BinariesResponseCollection CreatedTime_FormData(this BinariesResponseCollection res) { return res.ValAndFormData("#Binaries_CreatedTime", res.BinaryModel.CreatedTime.ToResponse()); }
        public static BinariesResponseCollection CreatedTime_FormData(this BinariesResponseCollection res, string value) { return res.ValAndFormData("#Binaries_CreatedTime", value); }
        public static BinariesResponseCollection UpdatedTime(this BinariesResponseCollection res) { return res.Val("#Binaries_UpdatedTime", res.BinaryModel.UpdatedTime.ToResponse()); }
        public static BinariesResponseCollection UpdatedTime(this BinariesResponseCollection res, string value) { return res.Val("#Binaries_UpdatedTime", value); }
        public static BinariesResponseCollection UpdatedTime_FormData(this BinariesResponseCollection res) { return res.ValAndFormData("#Binaries_UpdatedTime", res.BinaryModel.UpdatedTime.ToResponse()); }
        public static BinariesResponseCollection UpdatedTime_FormData(this BinariesResponseCollection res, string value) { return res.ValAndFormData("#Binaries_UpdatedTime", value); }
        public static BinariesResponseCollection Timestamp(this BinariesResponseCollection res) { return res.Val("#Binaries_Timestamp", res.BinaryModel.Timestamp.ToResponse()); }
        public static BinariesResponseCollection Timestamp(this BinariesResponseCollection res, string value) { return res.Val("#Binaries_Timestamp", value); }
        public static BinariesResponseCollection Timestamp_FormData(this BinariesResponseCollection res) { return res.ValAndFormData("#Binaries_Timestamp", res.BinaryModel.Timestamp.ToResponse()); }
        public static BinariesResponseCollection Timestamp_FormData(this BinariesResponseCollection res, string value) { return res.ValAndFormData("#Binaries_Timestamp", value); }
        public static PermissionsResponseCollection ReferenceId(this PermissionsResponseCollection res) { return res.Val("#Permissions_ReferenceId", res.PermissionModel.ReferenceId.ToResponse()); }
        public static PermissionsResponseCollection ReferenceId(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_ReferenceId", value); }
        public static PermissionsResponseCollection ReferenceId_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_ReferenceId", res.PermissionModel.ReferenceId.ToResponse()); }
        public static PermissionsResponseCollection ReferenceId_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_ReferenceId", value); }
        public static PermissionsResponseCollection DeptId(this PermissionsResponseCollection res) { return res.Val("#Permissions_DeptId", res.PermissionModel.DeptId.ToResponse()); }
        public static PermissionsResponseCollection DeptId(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_DeptId", value); }
        public static PermissionsResponseCollection DeptId_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_DeptId", res.PermissionModel.DeptId.ToResponse()); }
        public static PermissionsResponseCollection DeptId_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_DeptId", value); }
        public static PermissionsResponseCollection GroupId(this PermissionsResponseCollection res) { return res.Val("#Permissions_GroupId", res.PermissionModel.GroupId.ToResponse()); }
        public static PermissionsResponseCollection GroupId(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_GroupId", value); }
        public static PermissionsResponseCollection GroupId_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_GroupId", res.PermissionModel.GroupId.ToResponse()); }
        public static PermissionsResponseCollection GroupId_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_GroupId", value); }
        public static PermissionsResponseCollection UserId(this PermissionsResponseCollection res) { return res.Val("#Permissions_UserId", res.PermissionModel.UserId.ToResponse()); }
        public static PermissionsResponseCollection UserId(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_UserId", value); }
        public static PermissionsResponseCollection UserId_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_UserId", res.PermissionModel.UserId.ToResponse()); }
        public static PermissionsResponseCollection UserId_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_UserId", value); }
        public static PermissionsResponseCollection Ver(this PermissionsResponseCollection res) { return res.Val("#Permissions_Ver", res.PermissionModel.Ver.ToResponse()); }
        public static PermissionsResponseCollection Ver(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_Ver", value); }
        public static PermissionsResponseCollection Ver_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_Ver", res.PermissionModel.Ver.ToResponse()); }
        public static PermissionsResponseCollection Ver_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_Ver", value); }
        public static PermissionsResponseCollection DeptName(this PermissionsResponseCollection res) { return res.Val("#Permissions_DeptName", res.PermissionModel.DeptName.ToResponse()); }
        public static PermissionsResponseCollection DeptName(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_DeptName", value); }
        public static PermissionsResponseCollection DeptName_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_DeptName", res.PermissionModel.DeptName.ToResponse()); }
        public static PermissionsResponseCollection DeptName_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_DeptName", value); }
        public static PermissionsResponseCollection GroupName(this PermissionsResponseCollection res) { return res.Val("#Permissions_GroupName", res.PermissionModel.GroupName.ToResponse()); }
        public static PermissionsResponseCollection GroupName(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_GroupName", value); }
        public static PermissionsResponseCollection GroupName_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_GroupName", res.PermissionModel.GroupName.ToResponse()); }
        public static PermissionsResponseCollection GroupName_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_GroupName", value); }
        public static PermissionsResponseCollection Name(this PermissionsResponseCollection res) { return res.Val("#Permissions_Name", res.PermissionModel.Name.ToResponse()); }
        public static PermissionsResponseCollection Name(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_Name", value); }
        public static PermissionsResponseCollection Name_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_Name", res.PermissionModel.Name.ToResponse()); }
        public static PermissionsResponseCollection Name_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_Name", value); }
        public static PermissionsResponseCollection PermissionType(this PermissionsResponseCollection res) { return res.Val("#Permissions_PermissionType", res.PermissionModel.PermissionType.ToResponse()); }
        public static PermissionsResponseCollection PermissionType(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_PermissionType", value); }
        public static PermissionsResponseCollection PermissionType_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_PermissionType", res.PermissionModel.PermissionType.ToResponse()); }
        public static PermissionsResponseCollection PermissionType_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_PermissionType", value); }
        public static PermissionsResponseCollection Comments(this PermissionsResponseCollection res) { return res.Val("#Permissions_Comments", res.PermissionModel.Comments.ToResponse()); }
        public static PermissionsResponseCollection Comments(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_Comments", value); }
        public static PermissionsResponseCollection Comments_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_Comments", res.PermissionModel.Comments.ToResponse()); }
        public static PermissionsResponseCollection Comments_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_Comments", value); }
        public static PermissionsResponseCollection CreatedTime(this PermissionsResponseCollection res) { return res.Val("#Permissions_CreatedTime", res.PermissionModel.CreatedTime.ToResponse()); }
        public static PermissionsResponseCollection CreatedTime(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_CreatedTime", value); }
        public static PermissionsResponseCollection CreatedTime_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_CreatedTime", res.PermissionModel.CreatedTime.ToResponse()); }
        public static PermissionsResponseCollection CreatedTime_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_CreatedTime", value); }
        public static PermissionsResponseCollection UpdatedTime(this PermissionsResponseCollection res) { return res.Val("#Permissions_UpdatedTime", res.PermissionModel.UpdatedTime.ToResponse()); }
        public static PermissionsResponseCollection UpdatedTime(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_UpdatedTime", value); }
        public static PermissionsResponseCollection UpdatedTime_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_UpdatedTime", res.PermissionModel.UpdatedTime.ToResponse()); }
        public static PermissionsResponseCollection UpdatedTime_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_UpdatedTime", value); }
        public static PermissionsResponseCollection Timestamp(this PermissionsResponseCollection res) { return res.Val("#Permissions_Timestamp", res.PermissionModel.Timestamp.ToResponse()); }
        public static PermissionsResponseCollection Timestamp(this PermissionsResponseCollection res, string value) { return res.Val("#Permissions_Timestamp", value); }
        public static PermissionsResponseCollection Timestamp_FormData(this PermissionsResponseCollection res) { return res.ValAndFormData("#Permissions_Timestamp", res.PermissionModel.Timestamp.ToResponse()); }
        public static PermissionsResponseCollection Timestamp_FormData(this PermissionsResponseCollection res, string value) { return res.ValAndFormData("#Permissions_Timestamp", value); }
        public static IssuesResponseCollection UpdatedTime(this IssuesResponseCollection res) { return res.Val("#Issues_UpdatedTime", res.IssueModel.UpdatedTime.ToResponse()); }
        public static IssuesResponseCollection UpdatedTime(this IssuesResponseCollection res, string value) { return res.Val("#Issues_UpdatedTime", value); }
        public static IssuesResponseCollection UpdatedTime_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_UpdatedTime", res.IssueModel.UpdatedTime.ToResponse()); }
        public static IssuesResponseCollection UpdatedTime_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_UpdatedTime", value); }
        public static IssuesResponseCollection IssueId(this IssuesResponseCollection res) { return res.Val("#Issues_IssueId", res.IssueModel.IssueId.ToResponse()); }
        public static IssuesResponseCollection IssueId(this IssuesResponseCollection res, string value) { return res.Val("#Issues_IssueId", value); }
        public static IssuesResponseCollection IssueId_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_IssueId", res.IssueModel.IssueId.ToResponse()); }
        public static IssuesResponseCollection IssueId_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_IssueId", value); }
        public static IssuesResponseCollection Ver(this IssuesResponseCollection res) { return res.Val("#Issues_Ver", res.IssueModel.Ver.ToResponse()); }
        public static IssuesResponseCollection Ver(this IssuesResponseCollection res, string value) { return res.Val("#Issues_Ver", value); }
        public static IssuesResponseCollection Ver_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_Ver", res.IssueModel.Ver.ToResponse()); }
        public static IssuesResponseCollection Ver_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_Ver", value); }
        public static IssuesResponseCollection Title(this IssuesResponseCollection res) { return res.Val("#Issues_Title", res.IssueModel.Title.ToResponse()); }
        public static IssuesResponseCollection Title(this IssuesResponseCollection res, string value) { return res.Val("#Issues_Title", value); }
        public static IssuesResponseCollection Title_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_Title", res.IssueModel.Title.ToResponse()); }
        public static IssuesResponseCollection Title_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_Title", value); }
        public static IssuesResponseCollection Body(this IssuesResponseCollection res) { return res.Val("#Issues_Body", res.IssueModel.Body.ToResponse()); }
        public static IssuesResponseCollection Body(this IssuesResponseCollection res, string value) { return res.Val("#Issues_Body", value); }
        public static IssuesResponseCollection Body_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_Body", res.IssueModel.Body.ToResponse()); }
        public static IssuesResponseCollection Body_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_Body", value); }
        public static IssuesResponseCollection StartTime(this IssuesResponseCollection res) { return res.Val("#Issues_StartTime", res.IssueModel.StartTime.ToResponse()); }
        public static IssuesResponseCollection StartTime(this IssuesResponseCollection res, string value) { return res.Val("#Issues_StartTime", value); }
        public static IssuesResponseCollection StartTime_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_StartTime", res.IssueModel.StartTime.ToResponse()); }
        public static IssuesResponseCollection StartTime_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_StartTime", value); }
        public static IssuesResponseCollection CompletionTime(this IssuesResponseCollection res) { return res.Val("#Issues_CompletionTime", res.IssueModel.CompletionTime.ToResponse()); }
        public static IssuesResponseCollection CompletionTime(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CompletionTime", value); }
        public static IssuesResponseCollection CompletionTime_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CompletionTime", res.IssueModel.CompletionTime.ToResponse()); }
        public static IssuesResponseCollection CompletionTime_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CompletionTime", value); }
        public static IssuesResponseCollection WorkValue(this IssuesResponseCollection res) { return res.Val("#Issues_WorkValue", res.IssueModel.WorkValue.ToResponse()); }
        public static IssuesResponseCollection WorkValue(this IssuesResponseCollection res, string value) { return res.Val("#Issues_WorkValue", value); }
        public static IssuesResponseCollection WorkValue_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_WorkValue", res.IssueModel.WorkValue.ToResponse()); }
        public static IssuesResponseCollection WorkValue_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_WorkValue", value); }
        public static IssuesResponseCollection ProgressRate(this IssuesResponseCollection res) { return res.Val("#Issues_ProgressRate", res.IssueModel.ProgressRate.ToResponse()); }
        public static IssuesResponseCollection ProgressRate(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ProgressRate", value); }
        public static IssuesResponseCollection ProgressRate_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ProgressRate", res.IssueModel.ProgressRate.ToResponse()); }
        public static IssuesResponseCollection ProgressRate_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ProgressRate", value); }
        public static IssuesResponseCollection RemainingWorkValue(this IssuesResponseCollection res) { return res.Val("#Issues_RemainingWorkValue", res.IssueModel.RemainingWorkValue.ToResponse()); }
        public static IssuesResponseCollection RemainingWorkValue(this IssuesResponseCollection res, string value) { return res.Val("#Issues_RemainingWorkValue", value); }
        public static IssuesResponseCollection RemainingWorkValue_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_RemainingWorkValue", res.IssueModel.RemainingWorkValue.ToResponse()); }
        public static IssuesResponseCollection RemainingWorkValue_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_RemainingWorkValue", value); }
        public static IssuesResponseCollection Status(this IssuesResponseCollection res) { return res.Val("#Issues_Status", res.IssueModel.Status.ToResponse()); }
        public static IssuesResponseCollection Status(this IssuesResponseCollection res, string value) { return res.Val("#Issues_Status", value); }
        public static IssuesResponseCollection Status_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_Status", res.IssueModel.Status.ToResponse()); }
        public static IssuesResponseCollection Status_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_Status", value); }
        public static IssuesResponseCollection Manager(this IssuesResponseCollection res) { return res.Val("#Issues_Manager", res.IssueModel.Manager.ToResponse()); }
        public static IssuesResponseCollection Manager(this IssuesResponseCollection res, string value) { return res.Val("#Issues_Manager", value); }
        public static IssuesResponseCollection Manager_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_Manager", res.IssueModel.Manager.ToResponse()); }
        public static IssuesResponseCollection Manager_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_Manager", value); }
        public static IssuesResponseCollection Owner(this IssuesResponseCollection res) { return res.Val("#Issues_Owner", res.IssueModel.Owner.ToResponse()); }
        public static IssuesResponseCollection Owner(this IssuesResponseCollection res, string value) { return res.Val("#Issues_Owner", value); }
        public static IssuesResponseCollection Owner_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_Owner", res.IssueModel.Owner.ToResponse()); }
        public static IssuesResponseCollection Owner_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_Owner", value); }
        public static IssuesResponseCollection ClassA(this IssuesResponseCollection res) { return res.Val("#Issues_ClassA", res.IssueModel.ClassA.ToResponse()); }
        public static IssuesResponseCollection ClassA(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassA", value); }
        public static IssuesResponseCollection ClassA_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassA", res.IssueModel.ClassA.ToResponse()); }
        public static IssuesResponseCollection ClassA_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassA", value); }
        public static IssuesResponseCollection ClassB(this IssuesResponseCollection res) { return res.Val("#Issues_ClassB", res.IssueModel.ClassB.ToResponse()); }
        public static IssuesResponseCollection ClassB(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassB", value); }
        public static IssuesResponseCollection ClassB_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassB", res.IssueModel.ClassB.ToResponse()); }
        public static IssuesResponseCollection ClassB_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassB", value); }
        public static IssuesResponseCollection ClassC(this IssuesResponseCollection res) { return res.Val("#Issues_ClassC", res.IssueModel.ClassC.ToResponse()); }
        public static IssuesResponseCollection ClassC(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassC", value); }
        public static IssuesResponseCollection ClassC_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassC", res.IssueModel.ClassC.ToResponse()); }
        public static IssuesResponseCollection ClassC_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassC", value); }
        public static IssuesResponseCollection ClassD(this IssuesResponseCollection res) { return res.Val("#Issues_ClassD", res.IssueModel.ClassD.ToResponse()); }
        public static IssuesResponseCollection ClassD(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassD", value); }
        public static IssuesResponseCollection ClassD_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassD", res.IssueModel.ClassD.ToResponse()); }
        public static IssuesResponseCollection ClassD_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassD", value); }
        public static IssuesResponseCollection ClassE(this IssuesResponseCollection res) { return res.Val("#Issues_ClassE", res.IssueModel.ClassE.ToResponse()); }
        public static IssuesResponseCollection ClassE(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassE", value); }
        public static IssuesResponseCollection ClassE_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassE", res.IssueModel.ClassE.ToResponse()); }
        public static IssuesResponseCollection ClassE_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassE", value); }
        public static IssuesResponseCollection ClassF(this IssuesResponseCollection res) { return res.Val("#Issues_ClassF", res.IssueModel.ClassF.ToResponse()); }
        public static IssuesResponseCollection ClassF(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassF", value); }
        public static IssuesResponseCollection ClassF_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassF", res.IssueModel.ClassF.ToResponse()); }
        public static IssuesResponseCollection ClassF_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassF", value); }
        public static IssuesResponseCollection ClassG(this IssuesResponseCollection res) { return res.Val("#Issues_ClassG", res.IssueModel.ClassG.ToResponse()); }
        public static IssuesResponseCollection ClassG(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassG", value); }
        public static IssuesResponseCollection ClassG_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassG", res.IssueModel.ClassG.ToResponse()); }
        public static IssuesResponseCollection ClassG_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassG", value); }
        public static IssuesResponseCollection ClassH(this IssuesResponseCollection res) { return res.Val("#Issues_ClassH", res.IssueModel.ClassH.ToResponse()); }
        public static IssuesResponseCollection ClassH(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassH", value); }
        public static IssuesResponseCollection ClassH_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassH", res.IssueModel.ClassH.ToResponse()); }
        public static IssuesResponseCollection ClassH_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassH", value); }
        public static IssuesResponseCollection ClassI(this IssuesResponseCollection res) { return res.Val("#Issues_ClassI", res.IssueModel.ClassI.ToResponse()); }
        public static IssuesResponseCollection ClassI(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassI", value); }
        public static IssuesResponseCollection ClassI_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassI", res.IssueModel.ClassI.ToResponse()); }
        public static IssuesResponseCollection ClassI_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassI", value); }
        public static IssuesResponseCollection ClassJ(this IssuesResponseCollection res) { return res.Val("#Issues_ClassJ", res.IssueModel.ClassJ.ToResponse()); }
        public static IssuesResponseCollection ClassJ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassJ", value); }
        public static IssuesResponseCollection ClassJ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassJ", res.IssueModel.ClassJ.ToResponse()); }
        public static IssuesResponseCollection ClassJ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassJ", value); }
        public static IssuesResponseCollection ClassK(this IssuesResponseCollection res) { return res.Val("#Issues_ClassK", res.IssueModel.ClassK.ToResponse()); }
        public static IssuesResponseCollection ClassK(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassK", value); }
        public static IssuesResponseCollection ClassK_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassK", res.IssueModel.ClassK.ToResponse()); }
        public static IssuesResponseCollection ClassK_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassK", value); }
        public static IssuesResponseCollection ClassL(this IssuesResponseCollection res) { return res.Val("#Issues_ClassL", res.IssueModel.ClassL.ToResponse()); }
        public static IssuesResponseCollection ClassL(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassL", value); }
        public static IssuesResponseCollection ClassL_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassL", res.IssueModel.ClassL.ToResponse()); }
        public static IssuesResponseCollection ClassL_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassL", value); }
        public static IssuesResponseCollection ClassM(this IssuesResponseCollection res) { return res.Val("#Issues_ClassM", res.IssueModel.ClassM.ToResponse()); }
        public static IssuesResponseCollection ClassM(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassM", value); }
        public static IssuesResponseCollection ClassM_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassM", res.IssueModel.ClassM.ToResponse()); }
        public static IssuesResponseCollection ClassM_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassM", value); }
        public static IssuesResponseCollection ClassN(this IssuesResponseCollection res) { return res.Val("#Issues_ClassN", res.IssueModel.ClassN.ToResponse()); }
        public static IssuesResponseCollection ClassN(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassN", value); }
        public static IssuesResponseCollection ClassN_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassN", res.IssueModel.ClassN.ToResponse()); }
        public static IssuesResponseCollection ClassN_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassN", value); }
        public static IssuesResponseCollection ClassO(this IssuesResponseCollection res) { return res.Val("#Issues_ClassO", res.IssueModel.ClassO.ToResponse()); }
        public static IssuesResponseCollection ClassO(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassO", value); }
        public static IssuesResponseCollection ClassO_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassO", res.IssueModel.ClassO.ToResponse()); }
        public static IssuesResponseCollection ClassO_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassO", value); }
        public static IssuesResponseCollection ClassP(this IssuesResponseCollection res) { return res.Val("#Issues_ClassP", res.IssueModel.ClassP.ToResponse()); }
        public static IssuesResponseCollection ClassP(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassP", value); }
        public static IssuesResponseCollection ClassP_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassP", res.IssueModel.ClassP.ToResponse()); }
        public static IssuesResponseCollection ClassP_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassP", value); }
        public static IssuesResponseCollection ClassQ(this IssuesResponseCollection res) { return res.Val("#Issues_ClassQ", res.IssueModel.ClassQ.ToResponse()); }
        public static IssuesResponseCollection ClassQ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassQ", value); }
        public static IssuesResponseCollection ClassQ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassQ", res.IssueModel.ClassQ.ToResponse()); }
        public static IssuesResponseCollection ClassQ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassQ", value); }
        public static IssuesResponseCollection ClassR(this IssuesResponseCollection res) { return res.Val("#Issues_ClassR", res.IssueModel.ClassR.ToResponse()); }
        public static IssuesResponseCollection ClassR(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassR", value); }
        public static IssuesResponseCollection ClassR_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassR", res.IssueModel.ClassR.ToResponse()); }
        public static IssuesResponseCollection ClassR_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassR", value); }
        public static IssuesResponseCollection ClassS(this IssuesResponseCollection res) { return res.Val("#Issues_ClassS", res.IssueModel.ClassS.ToResponse()); }
        public static IssuesResponseCollection ClassS(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassS", value); }
        public static IssuesResponseCollection ClassS_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassS", res.IssueModel.ClassS.ToResponse()); }
        public static IssuesResponseCollection ClassS_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassS", value); }
        public static IssuesResponseCollection ClassT(this IssuesResponseCollection res) { return res.Val("#Issues_ClassT", res.IssueModel.ClassT.ToResponse()); }
        public static IssuesResponseCollection ClassT(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassT", value); }
        public static IssuesResponseCollection ClassT_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassT", res.IssueModel.ClassT.ToResponse()); }
        public static IssuesResponseCollection ClassT_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassT", value); }
        public static IssuesResponseCollection ClassU(this IssuesResponseCollection res) { return res.Val("#Issues_ClassU", res.IssueModel.ClassU.ToResponse()); }
        public static IssuesResponseCollection ClassU(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassU", value); }
        public static IssuesResponseCollection ClassU_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassU", res.IssueModel.ClassU.ToResponse()); }
        public static IssuesResponseCollection ClassU_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassU", value); }
        public static IssuesResponseCollection ClassV(this IssuesResponseCollection res) { return res.Val("#Issues_ClassV", res.IssueModel.ClassV.ToResponse()); }
        public static IssuesResponseCollection ClassV(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassV", value); }
        public static IssuesResponseCollection ClassV_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassV", res.IssueModel.ClassV.ToResponse()); }
        public static IssuesResponseCollection ClassV_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassV", value); }
        public static IssuesResponseCollection ClassW(this IssuesResponseCollection res) { return res.Val("#Issues_ClassW", res.IssueModel.ClassW.ToResponse()); }
        public static IssuesResponseCollection ClassW(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassW", value); }
        public static IssuesResponseCollection ClassW_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassW", res.IssueModel.ClassW.ToResponse()); }
        public static IssuesResponseCollection ClassW_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassW", value); }
        public static IssuesResponseCollection ClassX(this IssuesResponseCollection res) { return res.Val("#Issues_ClassX", res.IssueModel.ClassX.ToResponse()); }
        public static IssuesResponseCollection ClassX(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassX", value); }
        public static IssuesResponseCollection ClassX_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassX", res.IssueModel.ClassX.ToResponse()); }
        public static IssuesResponseCollection ClassX_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassX", value); }
        public static IssuesResponseCollection ClassY(this IssuesResponseCollection res) { return res.Val("#Issues_ClassY", res.IssueModel.ClassY.ToResponse()); }
        public static IssuesResponseCollection ClassY(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassY", value); }
        public static IssuesResponseCollection ClassY_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassY", res.IssueModel.ClassY.ToResponse()); }
        public static IssuesResponseCollection ClassY_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassY", value); }
        public static IssuesResponseCollection ClassZ(this IssuesResponseCollection res) { return res.Val("#Issues_ClassZ", res.IssueModel.ClassZ.ToResponse()); }
        public static IssuesResponseCollection ClassZ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_ClassZ", value); }
        public static IssuesResponseCollection ClassZ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_ClassZ", res.IssueModel.ClassZ.ToResponse()); }
        public static IssuesResponseCollection ClassZ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_ClassZ", value); }
        public static IssuesResponseCollection NumA(this IssuesResponseCollection res) { return res.Val("#Issues_NumA", res.IssueModel.NumA.ToResponse()); }
        public static IssuesResponseCollection NumA(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumA", value); }
        public static IssuesResponseCollection NumA_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumA", res.IssueModel.NumA.ToResponse()); }
        public static IssuesResponseCollection NumA_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumA", value); }
        public static IssuesResponseCollection NumB(this IssuesResponseCollection res) { return res.Val("#Issues_NumB", res.IssueModel.NumB.ToResponse()); }
        public static IssuesResponseCollection NumB(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumB", value); }
        public static IssuesResponseCollection NumB_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumB", res.IssueModel.NumB.ToResponse()); }
        public static IssuesResponseCollection NumB_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumB", value); }
        public static IssuesResponseCollection NumC(this IssuesResponseCollection res) { return res.Val("#Issues_NumC", res.IssueModel.NumC.ToResponse()); }
        public static IssuesResponseCollection NumC(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumC", value); }
        public static IssuesResponseCollection NumC_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumC", res.IssueModel.NumC.ToResponse()); }
        public static IssuesResponseCollection NumC_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumC", value); }
        public static IssuesResponseCollection NumD(this IssuesResponseCollection res) { return res.Val("#Issues_NumD", res.IssueModel.NumD.ToResponse()); }
        public static IssuesResponseCollection NumD(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumD", value); }
        public static IssuesResponseCollection NumD_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumD", res.IssueModel.NumD.ToResponse()); }
        public static IssuesResponseCollection NumD_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumD", value); }
        public static IssuesResponseCollection NumE(this IssuesResponseCollection res) { return res.Val("#Issues_NumE", res.IssueModel.NumE.ToResponse()); }
        public static IssuesResponseCollection NumE(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumE", value); }
        public static IssuesResponseCollection NumE_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumE", res.IssueModel.NumE.ToResponse()); }
        public static IssuesResponseCollection NumE_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumE", value); }
        public static IssuesResponseCollection NumF(this IssuesResponseCollection res) { return res.Val("#Issues_NumF", res.IssueModel.NumF.ToResponse()); }
        public static IssuesResponseCollection NumF(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumF", value); }
        public static IssuesResponseCollection NumF_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumF", res.IssueModel.NumF.ToResponse()); }
        public static IssuesResponseCollection NumF_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumF", value); }
        public static IssuesResponseCollection NumG(this IssuesResponseCollection res) { return res.Val("#Issues_NumG", res.IssueModel.NumG.ToResponse()); }
        public static IssuesResponseCollection NumG(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumG", value); }
        public static IssuesResponseCollection NumG_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumG", res.IssueModel.NumG.ToResponse()); }
        public static IssuesResponseCollection NumG_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumG", value); }
        public static IssuesResponseCollection NumH(this IssuesResponseCollection res) { return res.Val("#Issues_NumH", res.IssueModel.NumH.ToResponse()); }
        public static IssuesResponseCollection NumH(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumH", value); }
        public static IssuesResponseCollection NumH_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumH", res.IssueModel.NumH.ToResponse()); }
        public static IssuesResponseCollection NumH_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumH", value); }
        public static IssuesResponseCollection NumI(this IssuesResponseCollection res) { return res.Val("#Issues_NumI", res.IssueModel.NumI.ToResponse()); }
        public static IssuesResponseCollection NumI(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumI", value); }
        public static IssuesResponseCollection NumI_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumI", res.IssueModel.NumI.ToResponse()); }
        public static IssuesResponseCollection NumI_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumI", value); }
        public static IssuesResponseCollection NumJ(this IssuesResponseCollection res) { return res.Val("#Issues_NumJ", res.IssueModel.NumJ.ToResponse()); }
        public static IssuesResponseCollection NumJ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumJ", value); }
        public static IssuesResponseCollection NumJ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumJ", res.IssueModel.NumJ.ToResponse()); }
        public static IssuesResponseCollection NumJ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumJ", value); }
        public static IssuesResponseCollection NumK(this IssuesResponseCollection res) { return res.Val("#Issues_NumK", res.IssueModel.NumK.ToResponse()); }
        public static IssuesResponseCollection NumK(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumK", value); }
        public static IssuesResponseCollection NumK_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumK", res.IssueModel.NumK.ToResponse()); }
        public static IssuesResponseCollection NumK_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumK", value); }
        public static IssuesResponseCollection NumL(this IssuesResponseCollection res) { return res.Val("#Issues_NumL", res.IssueModel.NumL.ToResponse()); }
        public static IssuesResponseCollection NumL(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumL", value); }
        public static IssuesResponseCollection NumL_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumL", res.IssueModel.NumL.ToResponse()); }
        public static IssuesResponseCollection NumL_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumL", value); }
        public static IssuesResponseCollection NumM(this IssuesResponseCollection res) { return res.Val("#Issues_NumM", res.IssueModel.NumM.ToResponse()); }
        public static IssuesResponseCollection NumM(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumM", value); }
        public static IssuesResponseCollection NumM_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumM", res.IssueModel.NumM.ToResponse()); }
        public static IssuesResponseCollection NumM_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumM", value); }
        public static IssuesResponseCollection NumN(this IssuesResponseCollection res) { return res.Val("#Issues_NumN", res.IssueModel.NumN.ToResponse()); }
        public static IssuesResponseCollection NumN(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumN", value); }
        public static IssuesResponseCollection NumN_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumN", res.IssueModel.NumN.ToResponse()); }
        public static IssuesResponseCollection NumN_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumN", value); }
        public static IssuesResponseCollection NumO(this IssuesResponseCollection res) { return res.Val("#Issues_NumO", res.IssueModel.NumO.ToResponse()); }
        public static IssuesResponseCollection NumO(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumO", value); }
        public static IssuesResponseCollection NumO_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumO", res.IssueModel.NumO.ToResponse()); }
        public static IssuesResponseCollection NumO_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumO", value); }
        public static IssuesResponseCollection NumP(this IssuesResponseCollection res) { return res.Val("#Issues_NumP", res.IssueModel.NumP.ToResponse()); }
        public static IssuesResponseCollection NumP(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumP", value); }
        public static IssuesResponseCollection NumP_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumP", res.IssueModel.NumP.ToResponse()); }
        public static IssuesResponseCollection NumP_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumP", value); }
        public static IssuesResponseCollection NumQ(this IssuesResponseCollection res) { return res.Val("#Issues_NumQ", res.IssueModel.NumQ.ToResponse()); }
        public static IssuesResponseCollection NumQ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumQ", value); }
        public static IssuesResponseCollection NumQ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumQ", res.IssueModel.NumQ.ToResponse()); }
        public static IssuesResponseCollection NumQ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumQ", value); }
        public static IssuesResponseCollection NumR(this IssuesResponseCollection res) { return res.Val("#Issues_NumR", res.IssueModel.NumR.ToResponse()); }
        public static IssuesResponseCollection NumR(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumR", value); }
        public static IssuesResponseCollection NumR_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumR", res.IssueModel.NumR.ToResponse()); }
        public static IssuesResponseCollection NumR_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumR", value); }
        public static IssuesResponseCollection NumS(this IssuesResponseCollection res) { return res.Val("#Issues_NumS", res.IssueModel.NumS.ToResponse()); }
        public static IssuesResponseCollection NumS(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumS", value); }
        public static IssuesResponseCollection NumS_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumS", res.IssueModel.NumS.ToResponse()); }
        public static IssuesResponseCollection NumS_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumS", value); }
        public static IssuesResponseCollection NumT(this IssuesResponseCollection res) { return res.Val("#Issues_NumT", res.IssueModel.NumT.ToResponse()); }
        public static IssuesResponseCollection NumT(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumT", value); }
        public static IssuesResponseCollection NumT_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumT", res.IssueModel.NumT.ToResponse()); }
        public static IssuesResponseCollection NumT_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumT", value); }
        public static IssuesResponseCollection NumU(this IssuesResponseCollection res) { return res.Val("#Issues_NumU", res.IssueModel.NumU.ToResponse()); }
        public static IssuesResponseCollection NumU(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumU", value); }
        public static IssuesResponseCollection NumU_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumU", res.IssueModel.NumU.ToResponse()); }
        public static IssuesResponseCollection NumU_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumU", value); }
        public static IssuesResponseCollection NumV(this IssuesResponseCollection res) { return res.Val("#Issues_NumV", res.IssueModel.NumV.ToResponse()); }
        public static IssuesResponseCollection NumV(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumV", value); }
        public static IssuesResponseCollection NumV_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumV", res.IssueModel.NumV.ToResponse()); }
        public static IssuesResponseCollection NumV_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumV", value); }
        public static IssuesResponseCollection NumW(this IssuesResponseCollection res) { return res.Val("#Issues_NumW", res.IssueModel.NumW.ToResponse()); }
        public static IssuesResponseCollection NumW(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumW", value); }
        public static IssuesResponseCollection NumW_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumW", res.IssueModel.NumW.ToResponse()); }
        public static IssuesResponseCollection NumW_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumW", value); }
        public static IssuesResponseCollection NumX(this IssuesResponseCollection res) { return res.Val("#Issues_NumX", res.IssueModel.NumX.ToResponse()); }
        public static IssuesResponseCollection NumX(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumX", value); }
        public static IssuesResponseCollection NumX_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumX", res.IssueModel.NumX.ToResponse()); }
        public static IssuesResponseCollection NumX_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumX", value); }
        public static IssuesResponseCollection NumY(this IssuesResponseCollection res) { return res.Val("#Issues_NumY", res.IssueModel.NumY.ToResponse()); }
        public static IssuesResponseCollection NumY(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumY", value); }
        public static IssuesResponseCollection NumY_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumY", res.IssueModel.NumY.ToResponse()); }
        public static IssuesResponseCollection NumY_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumY", value); }
        public static IssuesResponseCollection NumZ(this IssuesResponseCollection res) { return res.Val("#Issues_NumZ", res.IssueModel.NumZ.ToResponse()); }
        public static IssuesResponseCollection NumZ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_NumZ", value); }
        public static IssuesResponseCollection NumZ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_NumZ", res.IssueModel.NumZ.ToResponse()); }
        public static IssuesResponseCollection NumZ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_NumZ", value); }
        public static IssuesResponseCollection DateA(this IssuesResponseCollection res) { return res.Val("#Issues_DateA", res.IssueModel.DateA.ToResponse()); }
        public static IssuesResponseCollection DateA(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateA", value); }
        public static IssuesResponseCollection DateA_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateA", res.IssueModel.DateA.ToResponse()); }
        public static IssuesResponseCollection DateA_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateA", value); }
        public static IssuesResponseCollection DateB(this IssuesResponseCollection res) { return res.Val("#Issues_DateB", res.IssueModel.DateB.ToResponse()); }
        public static IssuesResponseCollection DateB(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateB", value); }
        public static IssuesResponseCollection DateB_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateB", res.IssueModel.DateB.ToResponse()); }
        public static IssuesResponseCollection DateB_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateB", value); }
        public static IssuesResponseCollection DateC(this IssuesResponseCollection res) { return res.Val("#Issues_DateC", res.IssueModel.DateC.ToResponse()); }
        public static IssuesResponseCollection DateC(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateC", value); }
        public static IssuesResponseCollection DateC_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateC", res.IssueModel.DateC.ToResponse()); }
        public static IssuesResponseCollection DateC_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateC", value); }
        public static IssuesResponseCollection DateD(this IssuesResponseCollection res) { return res.Val("#Issues_DateD", res.IssueModel.DateD.ToResponse()); }
        public static IssuesResponseCollection DateD(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateD", value); }
        public static IssuesResponseCollection DateD_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateD", res.IssueModel.DateD.ToResponse()); }
        public static IssuesResponseCollection DateD_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateD", value); }
        public static IssuesResponseCollection DateE(this IssuesResponseCollection res) { return res.Val("#Issues_DateE", res.IssueModel.DateE.ToResponse()); }
        public static IssuesResponseCollection DateE(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateE", value); }
        public static IssuesResponseCollection DateE_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateE", res.IssueModel.DateE.ToResponse()); }
        public static IssuesResponseCollection DateE_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateE", value); }
        public static IssuesResponseCollection DateF(this IssuesResponseCollection res) { return res.Val("#Issues_DateF", res.IssueModel.DateF.ToResponse()); }
        public static IssuesResponseCollection DateF(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateF", value); }
        public static IssuesResponseCollection DateF_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateF", res.IssueModel.DateF.ToResponse()); }
        public static IssuesResponseCollection DateF_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateF", value); }
        public static IssuesResponseCollection DateG(this IssuesResponseCollection res) { return res.Val("#Issues_DateG", res.IssueModel.DateG.ToResponse()); }
        public static IssuesResponseCollection DateG(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateG", value); }
        public static IssuesResponseCollection DateG_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateG", res.IssueModel.DateG.ToResponse()); }
        public static IssuesResponseCollection DateG_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateG", value); }
        public static IssuesResponseCollection DateH(this IssuesResponseCollection res) { return res.Val("#Issues_DateH", res.IssueModel.DateH.ToResponse()); }
        public static IssuesResponseCollection DateH(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateH", value); }
        public static IssuesResponseCollection DateH_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateH", res.IssueModel.DateH.ToResponse()); }
        public static IssuesResponseCollection DateH_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateH", value); }
        public static IssuesResponseCollection DateI(this IssuesResponseCollection res) { return res.Val("#Issues_DateI", res.IssueModel.DateI.ToResponse()); }
        public static IssuesResponseCollection DateI(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateI", value); }
        public static IssuesResponseCollection DateI_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateI", res.IssueModel.DateI.ToResponse()); }
        public static IssuesResponseCollection DateI_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateI", value); }
        public static IssuesResponseCollection DateJ(this IssuesResponseCollection res) { return res.Val("#Issues_DateJ", res.IssueModel.DateJ.ToResponse()); }
        public static IssuesResponseCollection DateJ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateJ", value); }
        public static IssuesResponseCollection DateJ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateJ", res.IssueModel.DateJ.ToResponse()); }
        public static IssuesResponseCollection DateJ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateJ", value); }
        public static IssuesResponseCollection DateK(this IssuesResponseCollection res) { return res.Val("#Issues_DateK", res.IssueModel.DateK.ToResponse()); }
        public static IssuesResponseCollection DateK(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateK", value); }
        public static IssuesResponseCollection DateK_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateK", res.IssueModel.DateK.ToResponse()); }
        public static IssuesResponseCollection DateK_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateK", value); }
        public static IssuesResponseCollection DateL(this IssuesResponseCollection res) { return res.Val("#Issues_DateL", res.IssueModel.DateL.ToResponse()); }
        public static IssuesResponseCollection DateL(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateL", value); }
        public static IssuesResponseCollection DateL_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateL", res.IssueModel.DateL.ToResponse()); }
        public static IssuesResponseCollection DateL_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateL", value); }
        public static IssuesResponseCollection DateM(this IssuesResponseCollection res) { return res.Val("#Issues_DateM", res.IssueModel.DateM.ToResponse()); }
        public static IssuesResponseCollection DateM(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateM", value); }
        public static IssuesResponseCollection DateM_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateM", res.IssueModel.DateM.ToResponse()); }
        public static IssuesResponseCollection DateM_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateM", value); }
        public static IssuesResponseCollection DateN(this IssuesResponseCollection res) { return res.Val("#Issues_DateN", res.IssueModel.DateN.ToResponse()); }
        public static IssuesResponseCollection DateN(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateN", value); }
        public static IssuesResponseCollection DateN_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateN", res.IssueModel.DateN.ToResponse()); }
        public static IssuesResponseCollection DateN_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateN", value); }
        public static IssuesResponseCollection DateO(this IssuesResponseCollection res) { return res.Val("#Issues_DateO", res.IssueModel.DateO.ToResponse()); }
        public static IssuesResponseCollection DateO(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateO", value); }
        public static IssuesResponseCollection DateO_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateO", res.IssueModel.DateO.ToResponse()); }
        public static IssuesResponseCollection DateO_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateO", value); }
        public static IssuesResponseCollection DateP(this IssuesResponseCollection res) { return res.Val("#Issues_DateP", res.IssueModel.DateP.ToResponse()); }
        public static IssuesResponseCollection DateP(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateP", value); }
        public static IssuesResponseCollection DateP_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateP", res.IssueModel.DateP.ToResponse()); }
        public static IssuesResponseCollection DateP_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateP", value); }
        public static IssuesResponseCollection DateQ(this IssuesResponseCollection res) { return res.Val("#Issues_DateQ", res.IssueModel.DateQ.ToResponse()); }
        public static IssuesResponseCollection DateQ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateQ", value); }
        public static IssuesResponseCollection DateQ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateQ", res.IssueModel.DateQ.ToResponse()); }
        public static IssuesResponseCollection DateQ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateQ", value); }
        public static IssuesResponseCollection DateR(this IssuesResponseCollection res) { return res.Val("#Issues_DateR", res.IssueModel.DateR.ToResponse()); }
        public static IssuesResponseCollection DateR(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateR", value); }
        public static IssuesResponseCollection DateR_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateR", res.IssueModel.DateR.ToResponse()); }
        public static IssuesResponseCollection DateR_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateR", value); }
        public static IssuesResponseCollection DateS(this IssuesResponseCollection res) { return res.Val("#Issues_DateS", res.IssueModel.DateS.ToResponse()); }
        public static IssuesResponseCollection DateS(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateS", value); }
        public static IssuesResponseCollection DateS_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateS", res.IssueModel.DateS.ToResponse()); }
        public static IssuesResponseCollection DateS_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateS", value); }
        public static IssuesResponseCollection DateT(this IssuesResponseCollection res) { return res.Val("#Issues_DateT", res.IssueModel.DateT.ToResponse()); }
        public static IssuesResponseCollection DateT(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateT", value); }
        public static IssuesResponseCollection DateT_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateT", res.IssueModel.DateT.ToResponse()); }
        public static IssuesResponseCollection DateT_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateT", value); }
        public static IssuesResponseCollection DateU(this IssuesResponseCollection res) { return res.Val("#Issues_DateU", res.IssueModel.DateU.ToResponse()); }
        public static IssuesResponseCollection DateU(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateU", value); }
        public static IssuesResponseCollection DateU_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateU", res.IssueModel.DateU.ToResponse()); }
        public static IssuesResponseCollection DateU_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateU", value); }
        public static IssuesResponseCollection DateV(this IssuesResponseCollection res) { return res.Val("#Issues_DateV", res.IssueModel.DateV.ToResponse()); }
        public static IssuesResponseCollection DateV(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateV", value); }
        public static IssuesResponseCollection DateV_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateV", res.IssueModel.DateV.ToResponse()); }
        public static IssuesResponseCollection DateV_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateV", value); }
        public static IssuesResponseCollection DateW(this IssuesResponseCollection res) { return res.Val("#Issues_DateW", res.IssueModel.DateW.ToResponse()); }
        public static IssuesResponseCollection DateW(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateW", value); }
        public static IssuesResponseCollection DateW_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateW", res.IssueModel.DateW.ToResponse()); }
        public static IssuesResponseCollection DateW_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateW", value); }
        public static IssuesResponseCollection DateX(this IssuesResponseCollection res) { return res.Val("#Issues_DateX", res.IssueModel.DateX.ToResponse()); }
        public static IssuesResponseCollection DateX(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateX", value); }
        public static IssuesResponseCollection DateX_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateX", res.IssueModel.DateX.ToResponse()); }
        public static IssuesResponseCollection DateX_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateX", value); }
        public static IssuesResponseCollection DateY(this IssuesResponseCollection res) { return res.Val("#Issues_DateY", res.IssueModel.DateY.ToResponse()); }
        public static IssuesResponseCollection DateY(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateY", value); }
        public static IssuesResponseCollection DateY_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateY", res.IssueModel.DateY.ToResponse()); }
        public static IssuesResponseCollection DateY_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateY", value); }
        public static IssuesResponseCollection DateZ(this IssuesResponseCollection res) { return res.Val("#Issues_DateZ", res.IssueModel.DateZ.ToResponse()); }
        public static IssuesResponseCollection DateZ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DateZ", value); }
        public static IssuesResponseCollection DateZ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DateZ", res.IssueModel.DateZ.ToResponse()); }
        public static IssuesResponseCollection DateZ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DateZ", value); }
        public static IssuesResponseCollection DescriptionA(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionA", res.IssueModel.DescriptionA.ToResponse()); }
        public static IssuesResponseCollection DescriptionA(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionA", value); }
        public static IssuesResponseCollection DescriptionA_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionA", res.IssueModel.DescriptionA.ToResponse()); }
        public static IssuesResponseCollection DescriptionA_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionA", value); }
        public static IssuesResponseCollection DescriptionB(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionB", res.IssueModel.DescriptionB.ToResponse()); }
        public static IssuesResponseCollection DescriptionB(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionB", value); }
        public static IssuesResponseCollection DescriptionB_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionB", res.IssueModel.DescriptionB.ToResponse()); }
        public static IssuesResponseCollection DescriptionB_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionB", value); }
        public static IssuesResponseCollection DescriptionC(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionC", res.IssueModel.DescriptionC.ToResponse()); }
        public static IssuesResponseCollection DescriptionC(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionC", value); }
        public static IssuesResponseCollection DescriptionC_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionC", res.IssueModel.DescriptionC.ToResponse()); }
        public static IssuesResponseCollection DescriptionC_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionC", value); }
        public static IssuesResponseCollection DescriptionD(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionD", res.IssueModel.DescriptionD.ToResponse()); }
        public static IssuesResponseCollection DescriptionD(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionD", value); }
        public static IssuesResponseCollection DescriptionD_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionD", res.IssueModel.DescriptionD.ToResponse()); }
        public static IssuesResponseCollection DescriptionD_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionD", value); }
        public static IssuesResponseCollection DescriptionE(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionE", res.IssueModel.DescriptionE.ToResponse()); }
        public static IssuesResponseCollection DescriptionE(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionE", value); }
        public static IssuesResponseCollection DescriptionE_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionE", res.IssueModel.DescriptionE.ToResponse()); }
        public static IssuesResponseCollection DescriptionE_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionE", value); }
        public static IssuesResponseCollection DescriptionF(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionF", res.IssueModel.DescriptionF.ToResponse()); }
        public static IssuesResponseCollection DescriptionF(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionF", value); }
        public static IssuesResponseCollection DescriptionF_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionF", res.IssueModel.DescriptionF.ToResponse()); }
        public static IssuesResponseCollection DescriptionF_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionF", value); }
        public static IssuesResponseCollection DescriptionG(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionG", res.IssueModel.DescriptionG.ToResponse()); }
        public static IssuesResponseCollection DescriptionG(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionG", value); }
        public static IssuesResponseCollection DescriptionG_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionG", res.IssueModel.DescriptionG.ToResponse()); }
        public static IssuesResponseCollection DescriptionG_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionG", value); }
        public static IssuesResponseCollection DescriptionH(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionH", res.IssueModel.DescriptionH.ToResponse()); }
        public static IssuesResponseCollection DescriptionH(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionH", value); }
        public static IssuesResponseCollection DescriptionH_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionH", res.IssueModel.DescriptionH.ToResponse()); }
        public static IssuesResponseCollection DescriptionH_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionH", value); }
        public static IssuesResponseCollection DescriptionI(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionI", res.IssueModel.DescriptionI.ToResponse()); }
        public static IssuesResponseCollection DescriptionI(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionI", value); }
        public static IssuesResponseCollection DescriptionI_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionI", res.IssueModel.DescriptionI.ToResponse()); }
        public static IssuesResponseCollection DescriptionI_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionI", value); }
        public static IssuesResponseCollection DescriptionJ(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionJ", res.IssueModel.DescriptionJ.ToResponse()); }
        public static IssuesResponseCollection DescriptionJ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionJ", value); }
        public static IssuesResponseCollection DescriptionJ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionJ", res.IssueModel.DescriptionJ.ToResponse()); }
        public static IssuesResponseCollection DescriptionJ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionJ", value); }
        public static IssuesResponseCollection DescriptionK(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionK", res.IssueModel.DescriptionK.ToResponse()); }
        public static IssuesResponseCollection DescriptionK(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionK", value); }
        public static IssuesResponseCollection DescriptionK_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionK", res.IssueModel.DescriptionK.ToResponse()); }
        public static IssuesResponseCollection DescriptionK_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionK", value); }
        public static IssuesResponseCollection DescriptionL(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionL", res.IssueModel.DescriptionL.ToResponse()); }
        public static IssuesResponseCollection DescriptionL(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionL", value); }
        public static IssuesResponseCollection DescriptionL_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionL", res.IssueModel.DescriptionL.ToResponse()); }
        public static IssuesResponseCollection DescriptionL_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionL", value); }
        public static IssuesResponseCollection DescriptionM(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionM", res.IssueModel.DescriptionM.ToResponse()); }
        public static IssuesResponseCollection DescriptionM(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionM", value); }
        public static IssuesResponseCollection DescriptionM_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionM", res.IssueModel.DescriptionM.ToResponse()); }
        public static IssuesResponseCollection DescriptionM_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionM", value); }
        public static IssuesResponseCollection DescriptionN(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionN", res.IssueModel.DescriptionN.ToResponse()); }
        public static IssuesResponseCollection DescriptionN(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionN", value); }
        public static IssuesResponseCollection DescriptionN_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionN", res.IssueModel.DescriptionN.ToResponse()); }
        public static IssuesResponseCollection DescriptionN_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionN", value); }
        public static IssuesResponseCollection DescriptionO(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionO", res.IssueModel.DescriptionO.ToResponse()); }
        public static IssuesResponseCollection DescriptionO(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionO", value); }
        public static IssuesResponseCollection DescriptionO_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionO", res.IssueModel.DescriptionO.ToResponse()); }
        public static IssuesResponseCollection DescriptionO_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionO", value); }
        public static IssuesResponseCollection DescriptionP(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionP", res.IssueModel.DescriptionP.ToResponse()); }
        public static IssuesResponseCollection DescriptionP(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionP", value); }
        public static IssuesResponseCollection DescriptionP_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionP", res.IssueModel.DescriptionP.ToResponse()); }
        public static IssuesResponseCollection DescriptionP_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionP", value); }
        public static IssuesResponseCollection DescriptionQ(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionQ", res.IssueModel.DescriptionQ.ToResponse()); }
        public static IssuesResponseCollection DescriptionQ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionQ", value); }
        public static IssuesResponseCollection DescriptionQ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionQ", res.IssueModel.DescriptionQ.ToResponse()); }
        public static IssuesResponseCollection DescriptionQ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionQ", value); }
        public static IssuesResponseCollection DescriptionR(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionR", res.IssueModel.DescriptionR.ToResponse()); }
        public static IssuesResponseCollection DescriptionR(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionR", value); }
        public static IssuesResponseCollection DescriptionR_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionR", res.IssueModel.DescriptionR.ToResponse()); }
        public static IssuesResponseCollection DescriptionR_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionR", value); }
        public static IssuesResponseCollection DescriptionS(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionS", res.IssueModel.DescriptionS.ToResponse()); }
        public static IssuesResponseCollection DescriptionS(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionS", value); }
        public static IssuesResponseCollection DescriptionS_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionS", res.IssueModel.DescriptionS.ToResponse()); }
        public static IssuesResponseCollection DescriptionS_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionS", value); }
        public static IssuesResponseCollection DescriptionT(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionT", res.IssueModel.DescriptionT.ToResponse()); }
        public static IssuesResponseCollection DescriptionT(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionT", value); }
        public static IssuesResponseCollection DescriptionT_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionT", res.IssueModel.DescriptionT.ToResponse()); }
        public static IssuesResponseCollection DescriptionT_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionT", value); }
        public static IssuesResponseCollection DescriptionU(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionU", res.IssueModel.DescriptionU.ToResponse()); }
        public static IssuesResponseCollection DescriptionU(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionU", value); }
        public static IssuesResponseCollection DescriptionU_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionU", res.IssueModel.DescriptionU.ToResponse()); }
        public static IssuesResponseCollection DescriptionU_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionU", value); }
        public static IssuesResponseCollection DescriptionV(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionV", res.IssueModel.DescriptionV.ToResponse()); }
        public static IssuesResponseCollection DescriptionV(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionV", value); }
        public static IssuesResponseCollection DescriptionV_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionV", res.IssueModel.DescriptionV.ToResponse()); }
        public static IssuesResponseCollection DescriptionV_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionV", value); }
        public static IssuesResponseCollection DescriptionW(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionW", res.IssueModel.DescriptionW.ToResponse()); }
        public static IssuesResponseCollection DescriptionW(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionW", value); }
        public static IssuesResponseCollection DescriptionW_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionW", res.IssueModel.DescriptionW.ToResponse()); }
        public static IssuesResponseCollection DescriptionW_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionW", value); }
        public static IssuesResponseCollection DescriptionX(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionX", res.IssueModel.DescriptionX.ToResponse()); }
        public static IssuesResponseCollection DescriptionX(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionX", value); }
        public static IssuesResponseCollection DescriptionX_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionX", res.IssueModel.DescriptionX.ToResponse()); }
        public static IssuesResponseCollection DescriptionX_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionX", value); }
        public static IssuesResponseCollection DescriptionY(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionY", res.IssueModel.DescriptionY.ToResponse()); }
        public static IssuesResponseCollection DescriptionY(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionY", value); }
        public static IssuesResponseCollection DescriptionY_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionY", res.IssueModel.DescriptionY.ToResponse()); }
        public static IssuesResponseCollection DescriptionY_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionY", value); }
        public static IssuesResponseCollection DescriptionZ(this IssuesResponseCollection res) { return res.Val("#Issues_DescriptionZ", res.IssueModel.DescriptionZ.ToResponse()); }
        public static IssuesResponseCollection DescriptionZ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_DescriptionZ", value); }
        public static IssuesResponseCollection DescriptionZ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_DescriptionZ", res.IssueModel.DescriptionZ.ToResponse()); }
        public static IssuesResponseCollection DescriptionZ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_DescriptionZ", value); }
        public static IssuesResponseCollection CheckA(this IssuesResponseCollection res) { return res.Val("#Issues_CheckA", res.IssueModel.CheckA.ToResponse()); }
        public static IssuesResponseCollection CheckA(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckA", value); }
        public static IssuesResponseCollection CheckA_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckA", res.IssueModel.CheckA.ToResponse()); }
        public static IssuesResponseCollection CheckA_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckA", value); }
        public static IssuesResponseCollection CheckB(this IssuesResponseCollection res) { return res.Val("#Issues_CheckB", res.IssueModel.CheckB.ToResponse()); }
        public static IssuesResponseCollection CheckB(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckB", value); }
        public static IssuesResponseCollection CheckB_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckB", res.IssueModel.CheckB.ToResponse()); }
        public static IssuesResponseCollection CheckB_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckB", value); }
        public static IssuesResponseCollection CheckC(this IssuesResponseCollection res) { return res.Val("#Issues_CheckC", res.IssueModel.CheckC.ToResponse()); }
        public static IssuesResponseCollection CheckC(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckC", value); }
        public static IssuesResponseCollection CheckC_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckC", res.IssueModel.CheckC.ToResponse()); }
        public static IssuesResponseCollection CheckC_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckC", value); }
        public static IssuesResponseCollection CheckD(this IssuesResponseCollection res) { return res.Val("#Issues_CheckD", res.IssueModel.CheckD.ToResponse()); }
        public static IssuesResponseCollection CheckD(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckD", value); }
        public static IssuesResponseCollection CheckD_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckD", res.IssueModel.CheckD.ToResponse()); }
        public static IssuesResponseCollection CheckD_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckD", value); }
        public static IssuesResponseCollection CheckE(this IssuesResponseCollection res) { return res.Val("#Issues_CheckE", res.IssueModel.CheckE.ToResponse()); }
        public static IssuesResponseCollection CheckE(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckE", value); }
        public static IssuesResponseCollection CheckE_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckE", res.IssueModel.CheckE.ToResponse()); }
        public static IssuesResponseCollection CheckE_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckE", value); }
        public static IssuesResponseCollection CheckF(this IssuesResponseCollection res) { return res.Val("#Issues_CheckF", res.IssueModel.CheckF.ToResponse()); }
        public static IssuesResponseCollection CheckF(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckF", value); }
        public static IssuesResponseCollection CheckF_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckF", res.IssueModel.CheckF.ToResponse()); }
        public static IssuesResponseCollection CheckF_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckF", value); }
        public static IssuesResponseCollection CheckG(this IssuesResponseCollection res) { return res.Val("#Issues_CheckG", res.IssueModel.CheckG.ToResponse()); }
        public static IssuesResponseCollection CheckG(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckG", value); }
        public static IssuesResponseCollection CheckG_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckG", res.IssueModel.CheckG.ToResponse()); }
        public static IssuesResponseCollection CheckG_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckG", value); }
        public static IssuesResponseCollection CheckH(this IssuesResponseCollection res) { return res.Val("#Issues_CheckH", res.IssueModel.CheckH.ToResponse()); }
        public static IssuesResponseCollection CheckH(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckH", value); }
        public static IssuesResponseCollection CheckH_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckH", res.IssueModel.CheckH.ToResponse()); }
        public static IssuesResponseCollection CheckH_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckH", value); }
        public static IssuesResponseCollection CheckI(this IssuesResponseCollection res) { return res.Val("#Issues_CheckI", res.IssueModel.CheckI.ToResponse()); }
        public static IssuesResponseCollection CheckI(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckI", value); }
        public static IssuesResponseCollection CheckI_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckI", res.IssueModel.CheckI.ToResponse()); }
        public static IssuesResponseCollection CheckI_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckI", value); }
        public static IssuesResponseCollection CheckJ(this IssuesResponseCollection res) { return res.Val("#Issues_CheckJ", res.IssueModel.CheckJ.ToResponse()); }
        public static IssuesResponseCollection CheckJ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckJ", value); }
        public static IssuesResponseCollection CheckJ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckJ", res.IssueModel.CheckJ.ToResponse()); }
        public static IssuesResponseCollection CheckJ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckJ", value); }
        public static IssuesResponseCollection CheckK(this IssuesResponseCollection res) { return res.Val("#Issues_CheckK", res.IssueModel.CheckK.ToResponse()); }
        public static IssuesResponseCollection CheckK(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckK", value); }
        public static IssuesResponseCollection CheckK_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckK", res.IssueModel.CheckK.ToResponse()); }
        public static IssuesResponseCollection CheckK_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckK", value); }
        public static IssuesResponseCollection CheckL(this IssuesResponseCollection res) { return res.Val("#Issues_CheckL", res.IssueModel.CheckL.ToResponse()); }
        public static IssuesResponseCollection CheckL(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckL", value); }
        public static IssuesResponseCollection CheckL_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckL", res.IssueModel.CheckL.ToResponse()); }
        public static IssuesResponseCollection CheckL_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckL", value); }
        public static IssuesResponseCollection CheckM(this IssuesResponseCollection res) { return res.Val("#Issues_CheckM", res.IssueModel.CheckM.ToResponse()); }
        public static IssuesResponseCollection CheckM(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckM", value); }
        public static IssuesResponseCollection CheckM_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckM", res.IssueModel.CheckM.ToResponse()); }
        public static IssuesResponseCollection CheckM_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckM", value); }
        public static IssuesResponseCollection CheckN(this IssuesResponseCollection res) { return res.Val("#Issues_CheckN", res.IssueModel.CheckN.ToResponse()); }
        public static IssuesResponseCollection CheckN(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckN", value); }
        public static IssuesResponseCollection CheckN_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckN", res.IssueModel.CheckN.ToResponse()); }
        public static IssuesResponseCollection CheckN_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckN", value); }
        public static IssuesResponseCollection CheckO(this IssuesResponseCollection res) { return res.Val("#Issues_CheckO", res.IssueModel.CheckO.ToResponse()); }
        public static IssuesResponseCollection CheckO(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckO", value); }
        public static IssuesResponseCollection CheckO_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckO", res.IssueModel.CheckO.ToResponse()); }
        public static IssuesResponseCollection CheckO_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckO", value); }
        public static IssuesResponseCollection CheckP(this IssuesResponseCollection res) { return res.Val("#Issues_CheckP", res.IssueModel.CheckP.ToResponse()); }
        public static IssuesResponseCollection CheckP(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckP", value); }
        public static IssuesResponseCollection CheckP_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckP", res.IssueModel.CheckP.ToResponse()); }
        public static IssuesResponseCollection CheckP_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckP", value); }
        public static IssuesResponseCollection CheckQ(this IssuesResponseCollection res) { return res.Val("#Issues_CheckQ", res.IssueModel.CheckQ.ToResponse()); }
        public static IssuesResponseCollection CheckQ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckQ", value); }
        public static IssuesResponseCollection CheckQ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckQ", res.IssueModel.CheckQ.ToResponse()); }
        public static IssuesResponseCollection CheckQ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckQ", value); }
        public static IssuesResponseCollection CheckR(this IssuesResponseCollection res) { return res.Val("#Issues_CheckR", res.IssueModel.CheckR.ToResponse()); }
        public static IssuesResponseCollection CheckR(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckR", value); }
        public static IssuesResponseCollection CheckR_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckR", res.IssueModel.CheckR.ToResponse()); }
        public static IssuesResponseCollection CheckR_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckR", value); }
        public static IssuesResponseCollection CheckS(this IssuesResponseCollection res) { return res.Val("#Issues_CheckS", res.IssueModel.CheckS.ToResponse()); }
        public static IssuesResponseCollection CheckS(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckS", value); }
        public static IssuesResponseCollection CheckS_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckS", res.IssueModel.CheckS.ToResponse()); }
        public static IssuesResponseCollection CheckS_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckS", value); }
        public static IssuesResponseCollection CheckT(this IssuesResponseCollection res) { return res.Val("#Issues_CheckT", res.IssueModel.CheckT.ToResponse()); }
        public static IssuesResponseCollection CheckT(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckT", value); }
        public static IssuesResponseCollection CheckT_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckT", res.IssueModel.CheckT.ToResponse()); }
        public static IssuesResponseCollection CheckT_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckT", value); }
        public static IssuesResponseCollection CheckU(this IssuesResponseCollection res) { return res.Val("#Issues_CheckU", res.IssueModel.CheckU.ToResponse()); }
        public static IssuesResponseCollection CheckU(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckU", value); }
        public static IssuesResponseCollection CheckU_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckU", res.IssueModel.CheckU.ToResponse()); }
        public static IssuesResponseCollection CheckU_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckU", value); }
        public static IssuesResponseCollection CheckV(this IssuesResponseCollection res) { return res.Val("#Issues_CheckV", res.IssueModel.CheckV.ToResponse()); }
        public static IssuesResponseCollection CheckV(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckV", value); }
        public static IssuesResponseCollection CheckV_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckV", res.IssueModel.CheckV.ToResponse()); }
        public static IssuesResponseCollection CheckV_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckV", value); }
        public static IssuesResponseCollection CheckW(this IssuesResponseCollection res) { return res.Val("#Issues_CheckW", res.IssueModel.CheckW.ToResponse()); }
        public static IssuesResponseCollection CheckW(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckW", value); }
        public static IssuesResponseCollection CheckW_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckW", res.IssueModel.CheckW.ToResponse()); }
        public static IssuesResponseCollection CheckW_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckW", value); }
        public static IssuesResponseCollection CheckX(this IssuesResponseCollection res) { return res.Val("#Issues_CheckX", res.IssueModel.CheckX.ToResponse()); }
        public static IssuesResponseCollection CheckX(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckX", value); }
        public static IssuesResponseCollection CheckX_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckX", res.IssueModel.CheckX.ToResponse()); }
        public static IssuesResponseCollection CheckX_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckX", value); }
        public static IssuesResponseCollection CheckY(this IssuesResponseCollection res) { return res.Val("#Issues_CheckY", res.IssueModel.CheckY.ToResponse()); }
        public static IssuesResponseCollection CheckY(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckY", value); }
        public static IssuesResponseCollection CheckY_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckY", res.IssueModel.CheckY.ToResponse()); }
        public static IssuesResponseCollection CheckY_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckY", value); }
        public static IssuesResponseCollection CheckZ(this IssuesResponseCollection res) { return res.Val("#Issues_CheckZ", res.IssueModel.CheckZ.ToResponse()); }
        public static IssuesResponseCollection CheckZ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CheckZ", value); }
        public static IssuesResponseCollection CheckZ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CheckZ", res.IssueModel.CheckZ.ToResponse()); }
        public static IssuesResponseCollection CheckZ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CheckZ", value); }
        public static IssuesResponseCollection AttachmentsA(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsA", res.IssueModel.AttachmentsA.ToResponse()); }
        public static IssuesResponseCollection AttachmentsA(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsA", value); }
        public static IssuesResponseCollection AttachmentsA_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsA", res.IssueModel.AttachmentsA.ToResponse()); }
        public static IssuesResponseCollection AttachmentsA_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsA", value); }
        public static IssuesResponseCollection AttachmentsB(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsB", res.IssueModel.AttachmentsB.ToResponse()); }
        public static IssuesResponseCollection AttachmentsB(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsB", value); }
        public static IssuesResponseCollection AttachmentsB_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsB", res.IssueModel.AttachmentsB.ToResponse()); }
        public static IssuesResponseCollection AttachmentsB_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsB", value); }
        public static IssuesResponseCollection AttachmentsC(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsC", res.IssueModel.AttachmentsC.ToResponse()); }
        public static IssuesResponseCollection AttachmentsC(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsC", value); }
        public static IssuesResponseCollection AttachmentsC_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsC", res.IssueModel.AttachmentsC.ToResponse()); }
        public static IssuesResponseCollection AttachmentsC_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsC", value); }
        public static IssuesResponseCollection AttachmentsD(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsD", res.IssueModel.AttachmentsD.ToResponse()); }
        public static IssuesResponseCollection AttachmentsD(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsD", value); }
        public static IssuesResponseCollection AttachmentsD_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsD", res.IssueModel.AttachmentsD.ToResponse()); }
        public static IssuesResponseCollection AttachmentsD_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsD", value); }
        public static IssuesResponseCollection AttachmentsE(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsE", res.IssueModel.AttachmentsE.ToResponse()); }
        public static IssuesResponseCollection AttachmentsE(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsE", value); }
        public static IssuesResponseCollection AttachmentsE_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsE", res.IssueModel.AttachmentsE.ToResponse()); }
        public static IssuesResponseCollection AttachmentsE_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsE", value); }
        public static IssuesResponseCollection AttachmentsF(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsF", res.IssueModel.AttachmentsF.ToResponse()); }
        public static IssuesResponseCollection AttachmentsF(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsF", value); }
        public static IssuesResponseCollection AttachmentsF_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsF", res.IssueModel.AttachmentsF.ToResponse()); }
        public static IssuesResponseCollection AttachmentsF_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsF", value); }
        public static IssuesResponseCollection AttachmentsG(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsG", res.IssueModel.AttachmentsG.ToResponse()); }
        public static IssuesResponseCollection AttachmentsG(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsG", value); }
        public static IssuesResponseCollection AttachmentsG_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsG", res.IssueModel.AttachmentsG.ToResponse()); }
        public static IssuesResponseCollection AttachmentsG_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsG", value); }
        public static IssuesResponseCollection AttachmentsH(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsH", res.IssueModel.AttachmentsH.ToResponse()); }
        public static IssuesResponseCollection AttachmentsH(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsH", value); }
        public static IssuesResponseCollection AttachmentsH_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsH", res.IssueModel.AttachmentsH.ToResponse()); }
        public static IssuesResponseCollection AttachmentsH_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsH", value); }
        public static IssuesResponseCollection AttachmentsI(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsI", res.IssueModel.AttachmentsI.ToResponse()); }
        public static IssuesResponseCollection AttachmentsI(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsI", value); }
        public static IssuesResponseCollection AttachmentsI_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsI", res.IssueModel.AttachmentsI.ToResponse()); }
        public static IssuesResponseCollection AttachmentsI_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsI", value); }
        public static IssuesResponseCollection AttachmentsJ(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsJ", res.IssueModel.AttachmentsJ.ToResponse()); }
        public static IssuesResponseCollection AttachmentsJ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsJ", value); }
        public static IssuesResponseCollection AttachmentsJ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsJ", res.IssueModel.AttachmentsJ.ToResponse()); }
        public static IssuesResponseCollection AttachmentsJ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsJ", value); }
        public static IssuesResponseCollection AttachmentsK(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsK", res.IssueModel.AttachmentsK.ToResponse()); }
        public static IssuesResponseCollection AttachmentsK(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsK", value); }
        public static IssuesResponseCollection AttachmentsK_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsK", res.IssueModel.AttachmentsK.ToResponse()); }
        public static IssuesResponseCollection AttachmentsK_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsK", value); }
        public static IssuesResponseCollection AttachmentsL(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsL", res.IssueModel.AttachmentsL.ToResponse()); }
        public static IssuesResponseCollection AttachmentsL(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsL", value); }
        public static IssuesResponseCollection AttachmentsL_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsL", res.IssueModel.AttachmentsL.ToResponse()); }
        public static IssuesResponseCollection AttachmentsL_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsL", value); }
        public static IssuesResponseCollection AttachmentsM(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsM", res.IssueModel.AttachmentsM.ToResponse()); }
        public static IssuesResponseCollection AttachmentsM(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsM", value); }
        public static IssuesResponseCollection AttachmentsM_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsM", res.IssueModel.AttachmentsM.ToResponse()); }
        public static IssuesResponseCollection AttachmentsM_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsM", value); }
        public static IssuesResponseCollection AttachmentsN(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsN", res.IssueModel.AttachmentsN.ToResponse()); }
        public static IssuesResponseCollection AttachmentsN(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsN", value); }
        public static IssuesResponseCollection AttachmentsN_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsN", res.IssueModel.AttachmentsN.ToResponse()); }
        public static IssuesResponseCollection AttachmentsN_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsN", value); }
        public static IssuesResponseCollection AttachmentsO(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsO", res.IssueModel.AttachmentsO.ToResponse()); }
        public static IssuesResponseCollection AttachmentsO(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsO", value); }
        public static IssuesResponseCollection AttachmentsO_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsO", res.IssueModel.AttachmentsO.ToResponse()); }
        public static IssuesResponseCollection AttachmentsO_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsO", value); }
        public static IssuesResponseCollection AttachmentsP(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsP", res.IssueModel.AttachmentsP.ToResponse()); }
        public static IssuesResponseCollection AttachmentsP(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsP", value); }
        public static IssuesResponseCollection AttachmentsP_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsP", res.IssueModel.AttachmentsP.ToResponse()); }
        public static IssuesResponseCollection AttachmentsP_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsP", value); }
        public static IssuesResponseCollection AttachmentsQ(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsQ", res.IssueModel.AttachmentsQ.ToResponse()); }
        public static IssuesResponseCollection AttachmentsQ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsQ", value); }
        public static IssuesResponseCollection AttachmentsQ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsQ", res.IssueModel.AttachmentsQ.ToResponse()); }
        public static IssuesResponseCollection AttachmentsQ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsQ", value); }
        public static IssuesResponseCollection AttachmentsR(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsR", res.IssueModel.AttachmentsR.ToResponse()); }
        public static IssuesResponseCollection AttachmentsR(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsR", value); }
        public static IssuesResponseCollection AttachmentsR_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsR", res.IssueModel.AttachmentsR.ToResponse()); }
        public static IssuesResponseCollection AttachmentsR_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsR", value); }
        public static IssuesResponseCollection AttachmentsS(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsS", res.IssueModel.AttachmentsS.ToResponse()); }
        public static IssuesResponseCollection AttachmentsS(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsS", value); }
        public static IssuesResponseCollection AttachmentsS_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsS", res.IssueModel.AttachmentsS.ToResponse()); }
        public static IssuesResponseCollection AttachmentsS_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsS", value); }
        public static IssuesResponseCollection AttachmentsT(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsT", res.IssueModel.AttachmentsT.ToResponse()); }
        public static IssuesResponseCollection AttachmentsT(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsT", value); }
        public static IssuesResponseCollection AttachmentsT_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsT", res.IssueModel.AttachmentsT.ToResponse()); }
        public static IssuesResponseCollection AttachmentsT_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsT", value); }
        public static IssuesResponseCollection AttachmentsU(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsU", res.IssueModel.AttachmentsU.ToResponse()); }
        public static IssuesResponseCollection AttachmentsU(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsU", value); }
        public static IssuesResponseCollection AttachmentsU_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsU", res.IssueModel.AttachmentsU.ToResponse()); }
        public static IssuesResponseCollection AttachmentsU_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsU", value); }
        public static IssuesResponseCollection AttachmentsV(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsV", res.IssueModel.AttachmentsV.ToResponse()); }
        public static IssuesResponseCollection AttachmentsV(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsV", value); }
        public static IssuesResponseCollection AttachmentsV_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsV", res.IssueModel.AttachmentsV.ToResponse()); }
        public static IssuesResponseCollection AttachmentsV_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsV", value); }
        public static IssuesResponseCollection AttachmentsW(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsW", res.IssueModel.AttachmentsW.ToResponse()); }
        public static IssuesResponseCollection AttachmentsW(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsW", value); }
        public static IssuesResponseCollection AttachmentsW_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsW", res.IssueModel.AttachmentsW.ToResponse()); }
        public static IssuesResponseCollection AttachmentsW_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsW", value); }
        public static IssuesResponseCollection AttachmentsX(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsX", res.IssueModel.AttachmentsX.ToResponse()); }
        public static IssuesResponseCollection AttachmentsX(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsX", value); }
        public static IssuesResponseCollection AttachmentsX_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsX", res.IssueModel.AttachmentsX.ToResponse()); }
        public static IssuesResponseCollection AttachmentsX_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsX", value); }
        public static IssuesResponseCollection AttachmentsY(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsY", res.IssueModel.AttachmentsY.ToResponse()); }
        public static IssuesResponseCollection AttachmentsY(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsY", value); }
        public static IssuesResponseCollection AttachmentsY_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsY", res.IssueModel.AttachmentsY.ToResponse()); }
        public static IssuesResponseCollection AttachmentsY_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsY", value); }
        public static IssuesResponseCollection AttachmentsZ(this IssuesResponseCollection res) { return res.Val("#Issues_AttachmentsZ", res.IssueModel.AttachmentsZ.ToResponse()); }
        public static IssuesResponseCollection AttachmentsZ(this IssuesResponseCollection res, string value) { return res.Val("#Issues_AttachmentsZ", value); }
        public static IssuesResponseCollection AttachmentsZ_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_AttachmentsZ", res.IssueModel.AttachmentsZ.ToResponse()); }
        public static IssuesResponseCollection AttachmentsZ_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_AttachmentsZ", value); }
        public static IssuesResponseCollection Comments(this IssuesResponseCollection res) { return res.Val("#Issues_Comments", res.IssueModel.Comments.ToResponse()); }
        public static IssuesResponseCollection Comments(this IssuesResponseCollection res, string value) { return res.Val("#Issues_Comments", value); }
        public static IssuesResponseCollection Comments_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_Comments", res.IssueModel.Comments.ToResponse()); }
        public static IssuesResponseCollection Comments_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_Comments", value); }
        public static IssuesResponseCollection CreatedTime(this IssuesResponseCollection res) { return res.Val("#Issues_CreatedTime", res.IssueModel.CreatedTime.ToResponse()); }
        public static IssuesResponseCollection CreatedTime(this IssuesResponseCollection res, string value) { return res.Val("#Issues_CreatedTime", value); }
        public static IssuesResponseCollection CreatedTime_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_CreatedTime", res.IssueModel.CreatedTime.ToResponse()); }
        public static IssuesResponseCollection CreatedTime_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_CreatedTime", value); }
        public static IssuesResponseCollection Timestamp(this IssuesResponseCollection res) { return res.Val("#Issues_Timestamp", res.IssueModel.Timestamp.ToResponse()); }
        public static IssuesResponseCollection Timestamp(this IssuesResponseCollection res, string value) { return res.Val("#Issues_Timestamp", value); }
        public static IssuesResponseCollection Timestamp_FormData(this IssuesResponseCollection res) { return res.ValAndFormData("#Issues_Timestamp", res.IssueModel.Timestamp.ToResponse()); }
        public static IssuesResponseCollection Timestamp_FormData(this IssuesResponseCollection res, string value) { return res.ValAndFormData("#Issues_Timestamp", value); }
        public static ResultsResponseCollection UpdatedTime(this ResultsResponseCollection res) { return res.Val("#Results_UpdatedTime", res.ResultModel.UpdatedTime.ToResponse()); }
        public static ResultsResponseCollection UpdatedTime(this ResultsResponseCollection res, string value) { return res.Val("#Results_UpdatedTime", value); }
        public static ResultsResponseCollection UpdatedTime_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_UpdatedTime", res.ResultModel.UpdatedTime.ToResponse()); }
        public static ResultsResponseCollection UpdatedTime_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_UpdatedTime", value); }
        public static ResultsResponseCollection ResultId(this ResultsResponseCollection res) { return res.Val("#Results_ResultId", res.ResultModel.ResultId.ToResponse()); }
        public static ResultsResponseCollection ResultId(this ResultsResponseCollection res, string value) { return res.Val("#Results_ResultId", value); }
        public static ResultsResponseCollection ResultId_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ResultId", res.ResultModel.ResultId.ToResponse()); }
        public static ResultsResponseCollection ResultId_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ResultId", value); }
        public static ResultsResponseCollection Ver(this ResultsResponseCollection res) { return res.Val("#Results_Ver", res.ResultModel.Ver.ToResponse()); }
        public static ResultsResponseCollection Ver(this ResultsResponseCollection res, string value) { return res.Val("#Results_Ver", value); }
        public static ResultsResponseCollection Ver_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_Ver", res.ResultModel.Ver.ToResponse()); }
        public static ResultsResponseCollection Ver_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_Ver", value); }
        public static ResultsResponseCollection Title(this ResultsResponseCollection res) { return res.Val("#Results_Title", res.ResultModel.Title.ToResponse()); }
        public static ResultsResponseCollection Title(this ResultsResponseCollection res, string value) { return res.Val("#Results_Title", value); }
        public static ResultsResponseCollection Title_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_Title", res.ResultModel.Title.ToResponse()); }
        public static ResultsResponseCollection Title_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_Title", value); }
        public static ResultsResponseCollection Body(this ResultsResponseCollection res) { return res.Val("#Results_Body", res.ResultModel.Body.ToResponse()); }
        public static ResultsResponseCollection Body(this ResultsResponseCollection res, string value) { return res.Val("#Results_Body", value); }
        public static ResultsResponseCollection Body_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_Body", res.ResultModel.Body.ToResponse()); }
        public static ResultsResponseCollection Body_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_Body", value); }
        public static ResultsResponseCollection Status(this ResultsResponseCollection res) { return res.Val("#Results_Status", res.ResultModel.Status.ToResponse()); }
        public static ResultsResponseCollection Status(this ResultsResponseCollection res, string value) { return res.Val("#Results_Status", value); }
        public static ResultsResponseCollection Status_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_Status", res.ResultModel.Status.ToResponse()); }
        public static ResultsResponseCollection Status_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_Status", value); }
        public static ResultsResponseCollection Manager(this ResultsResponseCollection res) { return res.Val("#Results_Manager", res.ResultModel.Manager.ToResponse()); }
        public static ResultsResponseCollection Manager(this ResultsResponseCollection res, string value) { return res.Val("#Results_Manager", value); }
        public static ResultsResponseCollection Manager_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_Manager", res.ResultModel.Manager.ToResponse()); }
        public static ResultsResponseCollection Manager_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_Manager", value); }
        public static ResultsResponseCollection Owner(this ResultsResponseCollection res) { return res.Val("#Results_Owner", res.ResultModel.Owner.ToResponse()); }
        public static ResultsResponseCollection Owner(this ResultsResponseCollection res, string value) { return res.Val("#Results_Owner", value); }
        public static ResultsResponseCollection Owner_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_Owner", res.ResultModel.Owner.ToResponse()); }
        public static ResultsResponseCollection Owner_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_Owner", value); }
        public static ResultsResponseCollection ClassA(this ResultsResponseCollection res) { return res.Val("#Results_ClassA", res.ResultModel.ClassA.ToResponse()); }
        public static ResultsResponseCollection ClassA(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassA", value); }
        public static ResultsResponseCollection ClassA_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassA", res.ResultModel.ClassA.ToResponse()); }
        public static ResultsResponseCollection ClassA_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassA", value); }
        public static ResultsResponseCollection ClassB(this ResultsResponseCollection res) { return res.Val("#Results_ClassB", res.ResultModel.ClassB.ToResponse()); }
        public static ResultsResponseCollection ClassB(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassB", value); }
        public static ResultsResponseCollection ClassB_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassB", res.ResultModel.ClassB.ToResponse()); }
        public static ResultsResponseCollection ClassB_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassB", value); }
        public static ResultsResponseCollection ClassC(this ResultsResponseCollection res) { return res.Val("#Results_ClassC", res.ResultModel.ClassC.ToResponse()); }
        public static ResultsResponseCollection ClassC(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassC", value); }
        public static ResultsResponseCollection ClassC_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassC", res.ResultModel.ClassC.ToResponse()); }
        public static ResultsResponseCollection ClassC_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassC", value); }
        public static ResultsResponseCollection ClassD(this ResultsResponseCollection res) { return res.Val("#Results_ClassD", res.ResultModel.ClassD.ToResponse()); }
        public static ResultsResponseCollection ClassD(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassD", value); }
        public static ResultsResponseCollection ClassD_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassD", res.ResultModel.ClassD.ToResponse()); }
        public static ResultsResponseCollection ClassD_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassD", value); }
        public static ResultsResponseCollection ClassE(this ResultsResponseCollection res) { return res.Val("#Results_ClassE", res.ResultModel.ClassE.ToResponse()); }
        public static ResultsResponseCollection ClassE(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassE", value); }
        public static ResultsResponseCollection ClassE_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassE", res.ResultModel.ClassE.ToResponse()); }
        public static ResultsResponseCollection ClassE_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassE", value); }
        public static ResultsResponseCollection ClassF(this ResultsResponseCollection res) { return res.Val("#Results_ClassF", res.ResultModel.ClassF.ToResponse()); }
        public static ResultsResponseCollection ClassF(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassF", value); }
        public static ResultsResponseCollection ClassF_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassF", res.ResultModel.ClassF.ToResponse()); }
        public static ResultsResponseCollection ClassF_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassF", value); }
        public static ResultsResponseCollection ClassG(this ResultsResponseCollection res) { return res.Val("#Results_ClassG", res.ResultModel.ClassG.ToResponse()); }
        public static ResultsResponseCollection ClassG(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassG", value); }
        public static ResultsResponseCollection ClassG_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassG", res.ResultModel.ClassG.ToResponse()); }
        public static ResultsResponseCollection ClassG_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassG", value); }
        public static ResultsResponseCollection ClassH(this ResultsResponseCollection res) { return res.Val("#Results_ClassH", res.ResultModel.ClassH.ToResponse()); }
        public static ResultsResponseCollection ClassH(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassH", value); }
        public static ResultsResponseCollection ClassH_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassH", res.ResultModel.ClassH.ToResponse()); }
        public static ResultsResponseCollection ClassH_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassH", value); }
        public static ResultsResponseCollection ClassI(this ResultsResponseCollection res) { return res.Val("#Results_ClassI", res.ResultModel.ClassI.ToResponse()); }
        public static ResultsResponseCollection ClassI(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassI", value); }
        public static ResultsResponseCollection ClassI_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassI", res.ResultModel.ClassI.ToResponse()); }
        public static ResultsResponseCollection ClassI_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassI", value); }
        public static ResultsResponseCollection ClassJ(this ResultsResponseCollection res) { return res.Val("#Results_ClassJ", res.ResultModel.ClassJ.ToResponse()); }
        public static ResultsResponseCollection ClassJ(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassJ", value); }
        public static ResultsResponseCollection ClassJ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassJ", res.ResultModel.ClassJ.ToResponse()); }
        public static ResultsResponseCollection ClassJ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassJ", value); }
        public static ResultsResponseCollection ClassK(this ResultsResponseCollection res) { return res.Val("#Results_ClassK", res.ResultModel.ClassK.ToResponse()); }
        public static ResultsResponseCollection ClassK(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassK", value); }
        public static ResultsResponseCollection ClassK_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassK", res.ResultModel.ClassK.ToResponse()); }
        public static ResultsResponseCollection ClassK_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassK", value); }
        public static ResultsResponseCollection ClassL(this ResultsResponseCollection res) { return res.Val("#Results_ClassL", res.ResultModel.ClassL.ToResponse()); }
        public static ResultsResponseCollection ClassL(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassL", value); }
        public static ResultsResponseCollection ClassL_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassL", res.ResultModel.ClassL.ToResponse()); }
        public static ResultsResponseCollection ClassL_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassL", value); }
        public static ResultsResponseCollection ClassM(this ResultsResponseCollection res) { return res.Val("#Results_ClassM", res.ResultModel.ClassM.ToResponse()); }
        public static ResultsResponseCollection ClassM(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassM", value); }
        public static ResultsResponseCollection ClassM_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassM", res.ResultModel.ClassM.ToResponse()); }
        public static ResultsResponseCollection ClassM_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassM", value); }
        public static ResultsResponseCollection ClassN(this ResultsResponseCollection res) { return res.Val("#Results_ClassN", res.ResultModel.ClassN.ToResponse()); }
        public static ResultsResponseCollection ClassN(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassN", value); }
        public static ResultsResponseCollection ClassN_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassN", res.ResultModel.ClassN.ToResponse()); }
        public static ResultsResponseCollection ClassN_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassN", value); }
        public static ResultsResponseCollection ClassO(this ResultsResponseCollection res) { return res.Val("#Results_ClassO", res.ResultModel.ClassO.ToResponse()); }
        public static ResultsResponseCollection ClassO(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassO", value); }
        public static ResultsResponseCollection ClassO_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassO", res.ResultModel.ClassO.ToResponse()); }
        public static ResultsResponseCollection ClassO_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassO", value); }
        public static ResultsResponseCollection ClassP(this ResultsResponseCollection res) { return res.Val("#Results_ClassP", res.ResultModel.ClassP.ToResponse()); }
        public static ResultsResponseCollection ClassP(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassP", value); }
        public static ResultsResponseCollection ClassP_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassP", res.ResultModel.ClassP.ToResponse()); }
        public static ResultsResponseCollection ClassP_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassP", value); }
        public static ResultsResponseCollection ClassQ(this ResultsResponseCollection res) { return res.Val("#Results_ClassQ", res.ResultModel.ClassQ.ToResponse()); }
        public static ResultsResponseCollection ClassQ(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassQ", value); }
        public static ResultsResponseCollection ClassQ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassQ", res.ResultModel.ClassQ.ToResponse()); }
        public static ResultsResponseCollection ClassQ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassQ", value); }
        public static ResultsResponseCollection ClassR(this ResultsResponseCollection res) { return res.Val("#Results_ClassR", res.ResultModel.ClassR.ToResponse()); }
        public static ResultsResponseCollection ClassR(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassR", value); }
        public static ResultsResponseCollection ClassR_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassR", res.ResultModel.ClassR.ToResponse()); }
        public static ResultsResponseCollection ClassR_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassR", value); }
        public static ResultsResponseCollection ClassS(this ResultsResponseCollection res) { return res.Val("#Results_ClassS", res.ResultModel.ClassS.ToResponse()); }
        public static ResultsResponseCollection ClassS(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassS", value); }
        public static ResultsResponseCollection ClassS_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassS", res.ResultModel.ClassS.ToResponse()); }
        public static ResultsResponseCollection ClassS_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassS", value); }
        public static ResultsResponseCollection ClassT(this ResultsResponseCollection res) { return res.Val("#Results_ClassT", res.ResultModel.ClassT.ToResponse()); }
        public static ResultsResponseCollection ClassT(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassT", value); }
        public static ResultsResponseCollection ClassT_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassT", res.ResultModel.ClassT.ToResponse()); }
        public static ResultsResponseCollection ClassT_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassT", value); }
        public static ResultsResponseCollection ClassU(this ResultsResponseCollection res) { return res.Val("#Results_ClassU", res.ResultModel.ClassU.ToResponse()); }
        public static ResultsResponseCollection ClassU(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassU", value); }
        public static ResultsResponseCollection ClassU_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassU", res.ResultModel.ClassU.ToResponse()); }
        public static ResultsResponseCollection ClassU_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassU", value); }
        public static ResultsResponseCollection ClassV(this ResultsResponseCollection res) { return res.Val("#Results_ClassV", res.ResultModel.ClassV.ToResponse()); }
        public static ResultsResponseCollection ClassV(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassV", value); }
        public static ResultsResponseCollection ClassV_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassV", res.ResultModel.ClassV.ToResponse()); }
        public static ResultsResponseCollection ClassV_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassV", value); }
        public static ResultsResponseCollection ClassW(this ResultsResponseCollection res) { return res.Val("#Results_ClassW", res.ResultModel.ClassW.ToResponse()); }
        public static ResultsResponseCollection ClassW(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassW", value); }
        public static ResultsResponseCollection ClassW_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassW", res.ResultModel.ClassW.ToResponse()); }
        public static ResultsResponseCollection ClassW_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassW", value); }
        public static ResultsResponseCollection ClassX(this ResultsResponseCollection res) { return res.Val("#Results_ClassX", res.ResultModel.ClassX.ToResponse()); }
        public static ResultsResponseCollection ClassX(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassX", value); }
        public static ResultsResponseCollection ClassX_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassX", res.ResultModel.ClassX.ToResponse()); }
        public static ResultsResponseCollection ClassX_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassX", value); }
        public static ResultsResponseCollection ClassY(this ResultsResponseCollection res) { return res.Val("#Results_ClassY", res.ResultModel.ClassY.ToResponse()); }
        public static ResultsResponseCollection ClassY(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassY", value); }
        public static ResultsResponseCollection ClassY_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassY", res.ResultModel.ClassY.ToResponse()); }
        public static ResultsResponseCollection ClassY_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassY", value); }
        public static ResultsResponseCollection ClassZ(this ResultsResponseCollection res) { return res.Val("#Results_ClassZ", res.ResultModel.ClassZ.ToResponse()); }
        public static ResultsResponseCollection ClassZ(this ResultsResponseCollection res, string value) { return res.Val("#Results_ClassZ", value); }
        public static ResultsResponseCollection ClassZ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_ClassZ", res.ResultModel.ClassZ.ToResponse()); }
        public static ResultsResponseCollection ClassZ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_ClassZ", value); }
        public static ResultsResponseCollection NumA(this ResultsResponseCollection res) { return res.Val("#Results_NumA", res.ResultModel.NumA.ToResponse()); }
        public static ResultsResponseCollection NumA(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumA", value); }
        public static ResultsResponseCollection NumA_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumA", res.ResultModel.NumA.ToResponse()); }
        public static ResultsResponseCollection NumA_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumA", value); }
        public static ResultsResponseCollection NumB(this ResultsResponseCollection res) { return res.Val("#Results_NumB", res.ResultModel.NumB.ToResponse()); }
        public static ResultsResponseCollection NumB(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumB", value); }
        public static ResultsResponseCollection NumB_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumB", res.ResultModel.NumB.ToResponse()); }
        public static ResultsResponseCollection NumB_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumB", value); }
        public static ResultsResponseCollection NumC(this ResultsResponseCollection res) { return res.Val("#Results_NumC", res.ResultModel.NumC.ToResponse()); }
        public static ResultsResponseCollection NumC(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumC", value); }
        public static ResultsResponseCollection NumC_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumC", res.ResultModel.NumC.ToResponse()); }
        public static ResultsResponseCollection NumC_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumC", value); }
        public static ResultsResponseCollection NumD(this ResultsResponseCollection res) { return res.Val("#Results_NumD", res.ResultModel.NumD.ToResponse()); }
        public static ResultsResponseCollection NumD(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumD", value); }
        public static ResultsResponseCollection NumD_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumD", res.ResultModel.NumD.ToResponse()); }
        public static ResultsResponseCollection NumD_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumD", value); }
        public static ResultsResponseCollection NumE(this ResultsResponseCollection res) { return res.Val("#Results_NumE", res.ResultModel.NumE.ToResponse()); }
        public static ResultsResponseCollection NumE(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumE", value); }
        public static ResultsResponseCollection NumE_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumE", res.ResultModel.NumE.ToResponse()); }
        public static ResultsResponseCollection NumE_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumE", value); }
        public static ResultsResponseCollection NumF(this ResultsResponseCollection res) { return res.Val("#Results_NumF", res.ResultModel.NumF.ToResponse()); }
        public static ResultsResponseCollection NumF(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumF", value); }
        public static ResultsResponseCollection NumF_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumF", res.ResultModel.NumF.ToResponse()); }
        public static ResultsResponseCollection NumF_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumF", value); }
        public static ResultsResponseCollection NumG(this ResultsResponseCollection res) { return res.Val("#Results_NumG", res.ResultModel.NumG.ToResponse()); }
        public static ResultsResponseCollection NumG(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumG", value); }
        public static ResultsResponseCollection NumG_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumG", res.ResultModel.NumG.ToResponse()); }
        public static ResultsResponseCollection NumG_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumG", value); }
        public static ResultsResponseCollection NumH(this ResultsResponseCollection res) { return res.Val("#Results_NumH", res.ResultModel.NumH.ToResponse()); }
        public static ResultsResponseCollection NumH(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumH", value); }
        public static ResultsResponseCollection NumH_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumH", res.ResultModel.NumH.ToResponse()); }
        public static ResultsResponseCollection NumH_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumH", value); }
        public static ResultsResponseCollection NumI(this ResultsResponseCollection res) { return res.Val("#Results_NumI", res.ResultModel.NumI.ToResponse()); }
        public static ResultsResponseCollection NumI(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumI", value); }
        public static ResultsResponseCollection NumI_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumI", res.ResultModel.NumI.ToResponse()); }
        public static ResultsResponseCollection NumI_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumI", value); }
        public static ResultsResponseCollection NumJ(this ResultsResponseCollection res) { return res.Val("#Results_NumJ", res.ResultModel.NumJ.ToResponse()); }
        public static ResultsResponseCollection NumJ(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumJ", value); }
        public static ResultsResponseCollection NumJ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumJ", res.ResultModel.NumJ.ToResponse()); }
        public static ResultsResponseCollection NumJ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumJ", value); }
        public static ResultsResponseCollection NumK(this ResultsResponseCollection res) { return res.Val("#Results_NumK", res.ResultModel.NumK.ToResponse()); }
        public static ResultsResponseCollection NumK(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumK", value); }
        public static ResultsResponseCollection NumK_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumK", res.ResultModel.NumK.ToResponse()); }
        public static ResultsResponseCollection NumK_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumK", value); }
        public static ResultsResponseCollection NumL(this ResultsResponseCollection res) { return res.Val("#Results_NumL", res.ResultModel.NumL.ToResponse()); }
        public static ResultsResponseCollection NumL(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumL", value); }
        public static ResultsResponseCollection NumL_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumL", res.ResultModel.NumL.ToResponse()); }
        public static ResultsResponseCollection NumL_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumL", value); }
        public static ResultsResponseCollection NumM(this ResultsResponseCollection res) { return res.Val("#Results_NumM", res.ResultModel.NumM.ToResponse()); }
        public static ResultsResponseCollection NumM(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumM", value); }
        public static ResultsResponseCollection NumM_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumM", res.ResultModel.NumM.ToResponse()); }
        public static ResultsResponseCollection NumM_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumM", value); }
        public static ResultsResponseCollection NumN(this ResultsResponseCollection res) { return res.Val("#Results_NumN", res.ResultModel.NumN.ToResponse()); }
        public static ResultsResponseCollection NumN(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumN", value); }
        public static ResultsResponseCollection NumN_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumN", res.ResultModel.NumN.ToResponse()); }
        public static ResultsResponseCollection NumN_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumN", value); }
        public static ResultsResponseCollection NumO(this ResultsResponseCollection res) { return res.Val("#Results_NumO", res.ResultModel.NumO.ToResponse()); }
        public static ResultsResponseCollection NumO(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumO", value); }
        public static ResultsResponseCollection NumO_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumO", res.ResultModel.NumO.ToResponse()); }
        public static ResultsResponseCollection NumO_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumO", value); }
        public static ResultsResponseCollection NumP(this ResultsResponseCollection res) { return res.Val("#Results_NumP", res.ResultModel.NumP.ToResponse()); }
        public static ResultsResponseCollection NumP(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumP", value); }
        public static ResultsResponseCollection NumP_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumP", res.ResultModel.NumP.ToResponse()); }
        public static ResultsResponseCollection NumP_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumP", value); }
        public static ResultsResponseCollection NumQ(this ResultsResponseCollection res) { return res.Val("#Results_NumQ", res.ResultModel.NumQ.ToResponse()); }
        public static ResultsResponseCollection NumQ(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumQ", value); }
        public static ResultsResponseCollection NumQ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumQ", res.ResultModel.NumQ.ToResponse()); }
        public static ResultsResponseCollection NumQ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumQ", value); }
        public static ResultsResponseCollection NumR(this ResultsResponseCollection res) { return res.Val("#Results_NumR", res.ResultModel.NumR.ToResponse()); }
        public static ResultsResponseCollection NumR(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumR", value); }
        public static ResultsResponseCollection NumR_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumR", res.ResultModel.NumR.ToResponse()); }
        public static ResultsResponseCollection NumR_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumR", value); }
        public static ResultsResponseCollection NumS(this ResultsResponseCollection res) { return res.Val("#Results_NumS", res.ResultModel.NumS.ToResponse()); }
        public static ResultsResponseCollection NumS(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumS", value); }
        public static ResultsResponseCollection NumS_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumS", res.ResultModel.NumS.ToResponse()); }
        public static ResultsResponseCollection NumS_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumS", value); }
        public static ResultsResponseCollection NumT(this ResultsResponseCollection res) { return res.Val("#Results_NumT", res.ResultModel.NumT.ToResponse()); }
        public static ResultsResponseCollection NumT(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumT", value); }
        public static ResultsResponseCollection NumT_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumT", res.ResultModel.NumT.ToResponse()); }
        public static ResultsResponseCollection NumT_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumT", value); }
        public static ResultsResponseCollection NumU(this ResultsResponseCollection res) { return res.Val("#Results_NumU", res.ResultModel.NumU.ToResponse()); }
        public static ResultsResponseCollection NumU(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumU", value); }
        public static ResultsResponseCollection NumU_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumU", res.ResultModel.NumU.ToResponse()); }
        public static ResultsResponseCollection NumU_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumU", value); }
        public static ResultsResponseCollection NumV(this ResultsResponseCollection res) { return res.Val("#Results_NumV", res.ResultModel.NumV.ToResponse()); }
        public static ResultsResponseCollection NumV(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumV", value); }
        public static ResultsResponseCollection NumV_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumV", res.ResultModel.NumV.ToResponse()); }
        public static ResultsResponseCollection NumV_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumV", value); }
        public static ResultsResponseCollection NumW(this ResultsResponseCollection res) { return res.Val("#Results_NumW", res.ResultModel.NumW.ToResponse()); }
        public static ResultsResponseCollection NumW(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumW", value); }
        public static ResultsResponseCollection NumW_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumW", res.ResultModel.NumW.ToResponse()); }
        public static ResultsResponseCollection NumW_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumW", value); }
        public static ResultsResponseCollection NumX(this ResultsResponseCollection res) { return res.Val("#Results_NumX", res.ResultModel.NumX.ToResponse()); }
        public static ResultsResponseCollection NumX(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumX", value); }
        public static ResultsResponseCollection NumX_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumX", res.ResultModel.NumX.ToResponse()); }
        public static ResultsResponseCollection NumX_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumX", value); }
        public static ResultsResponseCollection NumY(this ResultsResponseCollection res) { return res.Val("#Results_NumY", res.ResultModel.NumY.ToResponse()); }
        public static ResultsResponseCollection NumY(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumY", value); }
        public static ResultsResponseCollection NumY_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumY", res.ResultModel.NumY.ToResponse()); }
        public static ResultsResponseCollection NumY_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumY", value); }
        public static ResultsResponseCollection NumZ(this ResultsResponseCollection res) { return res.Val("#Results_NumZ", res.ResultModel.NumZ.ToResponse()); }
        public static ResultsResponseCollection NumZ(this ResultsResponseCollection res, string value) { return res.Val("#Results_NumZ", value); }
        public static ResultsResponseCollection NumZ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_NumZ", res.ResultModel.NumZ.ToResponse()); }
        public static ResultsResponseCollection NumZ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_NumZ", value); }
        public static ResultsResponseCollection DateA(this ResultsResponseCollection res) { return res.Val("#Results_DateA", res.ResultModel.DateA.ToResponse()); }
        public static ResultsResponseCollection DateA(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateA", value); }
        public static ResultsResponseCollection DateA_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateA", res.ResultModel.DateA.ToResponse()); }
        public static ResultsResponseCollection DateA_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateA", value); }
        public static ResultsResponseCollection DateB(this ResultsResponseCollection res) { return res.Val("#Results_DateB", res.ResultModel.DateB.ToResponse()); }
        public static ResultsResponseCollection DateB(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateB", value); }
        public static ResultsResponseCollection DateB_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateB", res.ResultModel.DateB.ToResponse()); }
        public static ResultsResponseCollection DateB_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateB", value); }
        public static ResultsResponseCollection DateC(this ResultsResponseCollection res) { return res.Val("#Results_DateC", res.ResultModel.DateC.ToResponse()); }
        public static ResultsResponseCollection DateC(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateC", value); }
        public static ResultsResponseCollection DateC_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateC", res.ResultModel.DateC.ToResponse()); }
        public static ResultsResponseCollection DateC_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateC", value); }
        public static ResultsResponseCollection DateD(this ResultsResponseCollection res) { return res.Val("#Results_DateD", res.ResultModel.DateD.ToResponse()); }
        public static ResultsResponseCollection DateD(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateD", value); }
        public static ResultsResponseCollection DateD_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateD", res.ResultModel.DateD.ToResponse()); }
        public static ResultsResponseCollection DateD_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateD", value); }
        public static ResultsResponseCollection DateE(this ResultsResponseCollection res) { return res.Val("#Results_DateE", res.ResultModel.DateE.ToResponse()); }
        public static ResultsResponseCollection DateE(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateE", value); }
        public static ResultsResponseCollection DateE_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateE", res.ResultModel.DateE.ToResponse()); }
        public static ResultsResponseCollection DateE_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateE", value); }
        public static ResultsResponseCollection DateF(this ResultsResponseCollection res) { return res.Val("#Results_DateF", res.ResultModel.DateF.ToResponse()); }
        public static ResultsResponseCollection DateF(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateF", value); }
        public static ResultsResponseCollection DateF_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateF", res.ResultModel.DateF.ToResponse()); }
        public static ResultsResponseCollection DateF_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateF", value); }
        public static ResultsResponseCollection DateG(this ResultsResponseCollection res) { return res.Val("#Results_DateG", res.ResultModel.DateG.ToResponse()); }
        public static ResultsResponseCollection DateG(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateG", value); }
        public static ResultsResponseCollection DateG_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateG", res.ResultModel.DateG.ToResponse()); }
        public static ResultsResponseCollection DateG_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateG", value); }
        public static ResultsResponseCollection DateH(this ResultsResponseCollection res) { return res.Val("#Results_DateH", res.ResultModel.DateH.ToResponse()); }
        public static ResultsResponseCollection DateH(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateH", value); }
        public static ResultsResponseCollection DateH_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateH", res.ResultModel.DateH.ToResponse()); }
        public static ResultsResponseCollection DateH_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateH", value); }
        public static ResultsResponseCollection DateI(this ResultsResponseCollection res) { return res.Val("#Results_DateI", res.ResultModel.DateI.ToResponse()); }
        public static ResultsResponseCollection DateI(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateI", value); }
        public static ResultsResponseCollection DateI_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateI", res.ResultModel.DateI.ToResponse()); }
        public static ResultsResponseCollection DateI_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateI", value); }
        public static ResultsResponseCollection DateJ(this ResultsResponseCollection res) { return res.Val("#Results_DateJ", res.ResultModel.DateJ.ToResponse()); }
        public static ResultsResponseCollection DateJ(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateJ", value); }
        public static ResultsResponseCollection DateJ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateJ", res.ResultModel.DateJ.ToResponse()); }
        public static ResultsResponseCollection DateJ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateJ", value); }
        public static ResultsResponseCollection DateK(this ResultsResponseCollection res) { return res.Val("#Results_DateK", res.ResultModel.DateK.ToResponse()); }
        public static ResultsResponseCollection DateK(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateK", value); }
        public static ResultsResponseCollection DateK_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateK", res.ResultModel.DateK.ToResponse()); }
        public static ResultsResponseCollection DateK_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateK", value); }
        public static ResultsResponseCollection DateL(this ResultsResponseCollection res) { return res.Val("#Results_DateL", res.ResultModel.DateL.ToResponse()); }
        public static ResultsResponseCollection DateL(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateL", value); }
        public static ResultsResponseCollection DateL_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateL", res.ResultModel.DateL.ToResponse()); }
        public static ResultsResponseCollection DateL_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateL", value); }
        public static ResultsResponseCollection DateM(this ResultsResponseCollection res) { return res.Val("#Results_DateM", res.ResultModel.DateM.ToResponse()); }
        public static ResultsResponseCollection DateM(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateM", value); }
        public static ResultsResponseCollection DateM_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateM", res.ResultModel.DateM.ToResponse()); }
        public static ResultsResponseCollection DateM_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateM", value); }
        public static ResultsResponseCollection DateN(this ResultsResponseCollection res) { return res.Val("#Results_DateN", res.ResultModel.DateN.ToResponse()); }
        public static ResultsResponseCollection DateN(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateN", value); }
        public static ResultsResponseCollection DateN_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateN", res.ResultModel.DateN.ToResponse()); }
        public static ResultsResponseCollection DateN_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateN", value); }
        public static ResultsResponseCollection DateO(this ResultsResponseCollection res) { return res.Val("#Results_DateO", res.ResultModel.DateO.ToResponse()); }
        public static ResultsResponseCollection DateO(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateO", value); }
        public static ResultsResponseCollection DateO_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateO", res.ResultModel.DateO.ToResponse()); }
        public static ResultsResponseCollection DateO_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateO", value); }
        public static ResultsResponseCollection DateP(this ResultsResponseCollection res) { return res.Val("#Results_DateP", res.ResultModel.DateP.ToResponse()); }
        public static ResultsResponseCollection DateP(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateP", value); }
        public static ResultsResponseCollection DateP_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateP", res.ResultModel.DateP.ToResponse()); }
        public static ResultsResponseCollection DateP_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateP", value); }
        public static ResultsResponseCollection DateQ(this ResultsResponseCollection res) { return res.Val("#Results_DateQ", res.ResultModel.DateQ.ToResponse()); }
        public static ResultsResponseCollection DateQ(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateQ", value); }
        public static ResultsResponseCollection DateQ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateQ", res.ResultModel.DateQ.ToResponse()); }
        public static ResultsResponseCollection DateQ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateQ", value); }
        public static ResultsResponseCollection DateR(this ResultsResponseCollection res) { return res.Val("#Results_DateR", res.ResultModel.DateR.ToResponse()); }
        public static ResultsResponseCollection DateR(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateR", value); }
        public static ResultsResponseCollection DateR_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateR", res.ResultModel.DateR.ToResponse()); }
        public static ResultsResponseCollection DateR_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateR", value); }
        public static ResultsResponseCollection DateS(this ResultsResponseCollection res) { return res.Val("#Results_DateS", res.ResultModel.DateS.ToResponse()); }
        public static ResultsResponseCollection DateS(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateS", value); }
        public static ResultsResponseCollection DateS_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateS", res.ResultModel.DateS.ToResponse()); }
        public static ResultsResponseCollection DateS_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateS", value); }
        public static ResultsResponseCollection DateT(this ResultsResponseCollection res) { return res.Val("#Results_DateT", res.ResultModel.DateT.ToResponse()); }
        public static ResultsResponseCollection DateT(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateT", value); }
        public static ResultsResponseCollection DateT_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateT", res.ResultModel.DateT.ToResponse()); }
        public static ResultsResponseCollection DateT_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateT", value); }
        public static ResultsResponseCollection DateU(this ResultsResponseCollection res) { return res.Val("#Results_DateU", res.ResultModel.DateU.ToResponse()); }
        public static ResultsResponseCollection DateU(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateU", value); }
        public static ResultsResponseCollection DateU_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateU", res.ResultModel.DateU.ToResponse()); }
        public static ResultsResponseCollection DateU_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateU", value); }
        public static ResultsResponseCollection DateV(this ResultsResponseCollection res) { return res.Val("#Results_DateV", res.ResultModel.DateV.ToResponse()); }
        public static ResultsResponseCollection DateV(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateV", value); }
        public static ResultsResponseCollection DateV_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateV", res.ResultModel.DateV.ToResponse()); }
        public static ResultsResponseCollection DateV_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateV", value); }
        public static ResultsResponseCollection DateW(this ResultsResponseCollection res) { return res.Val("#Results_DateW", res.ResultModel.DateW.ToResponse()); }
        public static ResultsResponseCollection DateW(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateW", value); }
        public static ResultsResponseCollection DateW_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateW", res.ResultModel.DateW.ToResponse()); }
        public static ResultsResponseCollection DateW_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateW", value); }
        public static ResultsResponseCollection DateX(this ResultsResponseCollection res) { return res.Val("#Results_DateX", res.ResultModel.DateX.ToResponse()); }
        public static ResultsResponseCollection DateX(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateX", value); }
        public static ResultsResponseCollection DateX_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateX", res.ResultModel.DateX.ToResponse()); }
        public static ResultsResponseCollection DateX_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateX", value); }
        public static ResultsResponseCollection DateY(this ResultsResponseCollection res) { return res.Val("#Results_DateY", res.ResultModel.DateY.ToResponse()); }
        public static ResultsResponseCollection DateY(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateY", value); }
        public static ResultsResponseCollection DateY_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateY", res.ResultModel.DateY.ToResponse()); }
        public static ResultsResponseCollection DateY_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateY", value); }
        public static ResultsResponseCollection DateZ(this ResultsResponseCollection res) { return res.Val("#Results_DateZ", res.ResultModel.DateZ.ToResponse()); }
        public static ResultsResponseCollection DateZ(this ResultsResponseCollection res, string value) { return res.Val("#Results_DateZ", value); }
        public static ResultsResponseCollection DateZ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DateZ", res.ResultModel.DateZ.ToResponse()); }
        public static ResultsResponseCollection DateZ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DateZ", value); }
        public static ResultsResponseCollection DescriptionA(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionA", res.ResultModel.DescriptionA.ToResponse()); }
        public static ResultsResponseCollection DescriptionA(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionA", value); }
        public static ResultsResponseCollection DescriptionA_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionA", res.ResultModel.DescriptionA.ToResponse()); }
        public static ResultsResponseCollection DescriptionA_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionA", value); }
        public static ResultsResponseCollection DescriptionB(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionB", res.ResultModel.DescriptionB.ToResponse()); }
        public static ResultsResponseCollection DescriptionB(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionB", value); }
        public static ResultsResponseCollection DescriptionB_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionB", res.ResultModel.DescriptionB.ToResponse()); }
        public static ResultsResponseCollection DescriptionB_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionB", value); }
        public static ResultsResponseCollection DescriptionC(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionC", res.ResultModel.DescriptionC.ToResponse()); }
        public static ResultsResponseCollection DescriptionC(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionC", value); }
        public static ResultsResponseCollection DescriptionC_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionC", res.ResultModel.DescriptionC.ToResponse()); }
        public static ResultsResponseCollection DescriptionC_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionC", value); }
        public static ResultsResponseCollection DescriptionD(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionD", res.ResultModel.DescriptionD.ToResponse()); }
        public static ResultsResponseCollection DescriptionD(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionD", value); }
        public static ResultsResponseCollection DescriptionD_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionD", res.ResultModel.DescriptionD.ToResponse()); }
        public static ResultsResponseCollection DescriptionD_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionD", value); }
        public static ResultsResponseCollection DescriptionE(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionE", res.ResultModel.DescriptionE.ToResponse()); }
        public static ResultsResponseCollection DescriptionE(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionE", value); }
        public static ResultsResponseCollection DescriptionE_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionE", res.ResultModel.DescriptionE.ToResponse()); }
        public static ResultsResponseCollection DescriptionE_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionE", value); }
        public static ResultsResponseCollection DescriptionF(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionF", res.ResultModel.DescriptionF.ToResponse()); }
        public static ResultsResponseCollection DescriptionF(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionF", value); }
        public static ResultsResponseCollection DescriptionF_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionF", res.ResultModel.DescriptionF.ToResponse()); }
        public static ResultsResponseCollection DescriptionF_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionF", value); }
        public static ResultsResponseCollection DescriptionG(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionG", res.ResultModel.DescriptionG.ToResponse()); }
        public static ResultsResponseCollection DescriptionG(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionG", value); }
        public static ResultsResponseCollection DescriptionG_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionG", res.ResultModel.DescriptionG.ToResponse()); }
        public static ResultsResponseCollection DescriptionG_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionG", value); }
        public static ResultsResponseCollection DescriptionH(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionH", res.ResultModel.DescriptionH.ToResponse()); }
        public static ResultsResponseCollection DescriptionH(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionH", value); }
        public static ResultsResponseCollection DescriptionH_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionH", res.ResultModel.DescriptionH.ToResponse()); }
        public static ResultsResponseCollection DescriptionH_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionH", value); }
        public static ResultsResponseCollection DescriptionI(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionI", res.ResultModel.DescriptionI.ToResponse()); }
        public static ResultsResponseCollection DescriptionI(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionI", value); }
        public static ResultsResponseCollection DescriptionI_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionI", res.ResultModel.DescriptionI.ToResponse()); }
        public static ResultsResponseCollection DescriptionI_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionI", value); }
        public static ResultsResponseCollection DescriptionJ(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionJ", res.ResultModel.DescriptionJ.ToResponse()); }
        public static ResultsResponseCollection DescriptionJ(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionJ", value); }
        public static ResultsResponseCollection DescriptionJ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionJ", res.ResultModel.DescriptionJ.ToResponse()); }
        public static ResultsResponseCollection DescriptionJ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionJ", value); }
        public static ResultsResponseCollection DescriptionK(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionK", res.ResultModel.DescriptionK.ToResponse()); }
        public static ResultsResponseCollection DescriptionK(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionK", value); }
        public static ResultsResponseCollection DescriptionK_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionK", res.ResultModel.DescriptionK.ToResponse()); }
        public static ResultsResponseCollection DescriptionK_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionK", value); }
        public static ResultsResponseCollection DescriptionL(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionL", res.ResultModel.DescriptionL.ToResponse()); }
        public static ResultsResponseCollection DescriptionL(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionL", value); }
        public static ResultsResponseCollection DescriptionL_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionL", res.ResultModel.DescriptionL.ToResponse()); }
        public static ResultsResponseCollection DescriptionL_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionL", value); }
        public static ResultsResponseCollection DescriptionM(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionM", res.ResultModel.DescriptionM.ToResponse()); }
        public static ResultsResponseCollection DescriptionM(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionM", value); }
        public static ResultsResponseCollection DescriptionM_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionM", res.ResultModel.DescriptionM.ToResponse()); }
        public static ResultsResponseCollection DescriptionM_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionM", value); }
        public static ResultsResponseCollection DescriptionN(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionN", res.ResultModel.DescriptionN.ToResponse()); }
        public static ResultsResponseCollection DescriptionN(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionN", value); }
        public static ResultsResponseCollection DescriptionN_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionN", res.ResultModel.DescriptionN.ToResponse()); }
        public static ResultsResponseCollection DescriptionN_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionN", value); }
        public static ResultsResponseCollection DescriptionO(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionO", res.ResultModel.DescriptionO.ToResponse()); }
        public static ResultsResponseCollection DescriptionO(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionO", value); }
        public static ResultsResponseCollection DescriptionO_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionO", res.ResultModel.DescriptionO.ToResponse()); }
        public static ResultsResponseCollection DescriptionO_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionO", value); }
        public static ResultsResponseCollection DescriptionP(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionP", res.ResultModel.DescriptionP.ToResponse()); }
        public static ResultsResponseCollection DescriptionP(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionP", value); }
        public static ResultsResponseCollection DescriptionP_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionP", res.ResultModel.DescriptionP.ToResponse()); }
        public static ResultsResponseCollection DescriptionP_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionP", value); }
        public static ResultsResponseCollection DescriptionQ(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionQ", res.ResultModel.DescriptionQ.ToResponse()); }
        public static ResultsResponseCollection DescriptionQ(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionQ", value); }
        public static ResultsResponseCollection DescriptionQ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionQ", res.ResultModel.DescriptionQ.ToResponse()); }
        public static ResultsResponseCollection DescriptionQ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionQ", value); }
        public static ResultsResponseCollection DescriptionR(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionR", res.ResultModel.DescriptionR.ToResponse()); }
        public static ResultsResponseCollection DescriptionR(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionR", value); }
        public static ResultsResponseCollection DescriptionR_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionR", res.ResultModel.DescriptionR.ToResponse()); }
        public static ResultsResponseCollection DescriptionR_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionR", value); }
        public static ResultsResponseCollection DescriptionS(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionS", res.ResultModel.DescriptionS.ToResponse()); }
        public static ResultsResponseCollection DescriptionS(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionS", value); }
        public static ResultsResponseCollection DescriptionS_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionS", res.ResultModel.DescriptionS.ToResponse()); }
        public static ResultsResponseCollection DescriptionS_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionS", value); }
        public static ResultsResponseCollection DescriptionT(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionT", res.ResultModel.DescriptionT.ToResponse()); }
        public static ResultsResponseCollection DescriptionT(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionT", value); }
        public static ResultsResponseCollection DescriptionT_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionT", res.ResultModel.DescriptionT.ToResponse()); }
        public static ResultsResponseCollection DescriptionT_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionT", value); }
        public static ResultsResponseCollection DescriptionU(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionU", res.ResultModel.DescriptionU.ToResponse()); }
        public static ResultsResponseCollection DescriptionU(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionU", value); }
        public static ResultsResponseCollection DescriptionU_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionU", res.ResultModel.DescriptionU.ToResponse()); }
        public static ResultsResponseCollection DescriptionU_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionU", value); }
        public static ResultsResponseCollection DescriptionV(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionV", res.ResultModel.DescriptionV.ToResponse()); }
        public static ResultsResponseCollection DescriptionV(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionV", value); }
        public static ResultsResponseCollection DescriptionV_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionV", res.ResultModel.DescriptionV.ToResponse()); }
        public static ResultsResponseCollection DescriptionV_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionV", value); }
        public static ResultsResponseCollection DescriptionW(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionW", res.ResultModel.DescriptionW.ToResponse()); }
        public static ResultsResponseCollection DescriptionW(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionW", value); }
        public static ResultsResponseCollection DescriptionW_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionW", res.ResultModel.DescriptionW.ToResponse()); }
        public static ResultsResponseCollection DescriptionW_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionW", value); }
        public static ResultsResponseCollection DescriptionX(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionX", res.ResultModel.DescriptionX.ToResponse()); }
        public static ResultsResponseCollection DescriptionX(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionX", value); }
        public static ResultsResponseCollection DescriptionX_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionX", res.ResultModel.DescriptionX.ToResponse()); }
        public static ResultsResponseCollection DescriptionX_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionX", value); }
        public static ResultsResponseCollection DescriptionY(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionY", res.ResultModel.DescriptionY.ToResponse()); }
        public static ResultsResponseCollection DescriptionY(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionY", value); }
        public static ResultsResponseCollection DescriptionY_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionY", res.ResultModel.DescriptionY.ToResponse()); }
        public static ResultsResponseCollection DescriptionY_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionY", value); }
        public static ResultsResponseCollection DescriptionZ(this ResultsResponseCollection res) { return res.Val("#Results_DescriptionZ", res.ResultModel.DescriptionZ.ToResponse()); }
        public static ResultsResponseCollection DescriptionZ(this ResultsResponseCollection res, string value) { return res.Val("#Results_DescriptionZ", value); }
        public static ResultsResponseCollection DescriptionZ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_DescriptionZ", res.ResultModel.DescriptionZ.ToResponse()); }
        public static ResultsResponseCollection DescriptionZ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_DescriptionZ", value); }
        public static ResultsResponseCollection CheckA(this ResultsResponseCollection res) { return res.Val("#Results_CheckA", res.ResultModel.CheckA.ToResponse()); }
        public static ResultsResponseCollection CheckA(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckA", value); }
        public static ResultsResponseCollection CheckA_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckA", res.ResultModel.CheckA.ToResponse()); }
        public static ResultsResponseCollection CheckA_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckA", value); }
        public static ResultsResponseCollection CheckB(this ResultsResponseCollection res) { return res.Val("#Results_CheckB", res.ResultModel.CheckB.ToResponse()); }
        public static ResultsResponseCollection CheckB(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckB", value); }
        public static ResultsResponseCollection CheckB_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckB", res.ResultModel.CheckB.ToResponse()); }
        public static ResultsResponseCollection CheckB_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckB", value); }
        public static ResultsResponseCollection CheckC(this ResultsResponseCollection res) { return res.Val("#Results_CheckC", res.ResultModel.CheckC.ToResponse()); }
        public static ResultsResponseCollection CheckC(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckC", value); }
        public static ResultsResponseCollection CheckC_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckC", res.ResultModel.CheckC.ToResponse()); }
        public static ResultsResponseCollection CheckC_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckC", value); }
        public static ResultsResponseCollection CheckD(this ResultsResponseCollection res) { return res.Val("#Results_CheckD", res.ResultModel.CheckD.ToResponse()); }
        public static ResultsResponseCollection CheckD(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckD", value); }
        public static ResultsResponseCollection CheckD_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckD", res.ResultModel.CheckD.ToResponse()); }
        public static ResultsResponseCollection CheckD_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckD", value); }
        public static ResultsResponseCollection CheckE(this ResultsResponseCollection res) { return res.Val("#Results_CheckE", res.ResultModel.CheckE.ToResponse()); }
        public static ResultsResponseCollection CheckE(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckE", value); }
        public static ResultsResponseCollection CheckE_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckE", res.ResultModel.CheckE.ToResponse()); }
        public static ResultsResponseCollection CheckE_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckE", value); }
        public static ResultsResponseCollection CheckF(this ResultsResponseCollection res) { return res.Val("#Results_CheckF", res.ResultModel.CheckF.ToResponse()); }
        public static ResultsResponseCollection CheckF(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckF", value); }
        public static ResultsResponseCollection CheckF_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckF", res.ResultModel.CheckF.ToResponse()); }
        public static ResultsResponseCollection CheckF_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckF", value); }
        public static ResultsResponseCollection CheckG(this ResultsResponseCollection res) { return res.Val("#Results_CheckG", res.ResultModel.CheckG.ToResponse()); }
        public static ResultsResponseCollection CheckG(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckG", value); }
        public static ResultsResponseCollection CheckG_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckG", res.ResultModel.CheckG.ToResponse()); }
        public static ResultsResponseCollection CheckG_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckG", value); }
        public static ResultsResponseCollection CheckH(this ResultsResponseCollection res) { return res.Val("#Results_CheckH", res.ResultModel.CheckH.ToResponse()); }
        public static ResultsResponseCollection CheckH(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckH", value); }
        public static ResultsResponseCollection CheckH_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckH", res.ResultModel.CheckH.ToResponse()); }
        public static ResultsResponseCollection CheckH_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckH", value); }
        public static ResultsResponseCollection CheckI(this ResultsResponseCollection res) { return res.Val("#Results_CheckI", res.ResultModel.CheckI.ToResponse()); }
        public static ResultsResponseCollection CheckI(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckI", value); }
        public static ResultsResponseCollection CheckI_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckI", res.ResultModel.CheckI.ToResponse()); }
        public static ResultsResponseCollection CheckI_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckI", value); }
        public static ResultsResponseCollection CheckJ(this ResultsResponseCollection res) { return res.Val("#Results_CheckJ", res.ResultModel.CheckJ.ToResponse()); }
        public static ResultsResponseCollection CheckJ(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckJ", value); }
        public static ResultsResponseCollection CheckJ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckJ", res.ResultModel.CheckJ.ToResponse()); }
        public static ResultsResponseCollection CheckJ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckJ", value); }
        public static ResultsResponseCollection CheckK(this ResultsResponseCollection res) { return res.Val("#Results_CheckK", res.ResultModel.CheckK.ToResponse()); }
        public static ResultsResponseCollection CheckK(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckK", value); }
        public static ResultsResponseCollection CheckK_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckK", res.ResultModel.CheckK.ToResponse()); }
        public static ResultsResponseCollection CheckK_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckK", value); }
        public static ResultsResponseCollection CheckL(this ResultsResponseCollection res) { return res.Val("#Results_CheckL", res.ResultModel.CheckL.ToResponse()); }
        public static ResultsResponseCollection CheckL(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckL", value); }
        public static ResultsResponseCollection CheckL_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckL", res.ResultModel.CheckL.ToResponse()); }
        public static ResultsResponseCollection CheckL_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckL", value); }
        public static ResultsResponseCollection CheckM(this ResultsResponseCollection res) { return res.Val("#Results_CheckM", res.ResultModel.CheckM.ToResponse()); }
        public static ResultsResponseCollection CheckM(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckM", value); }
        public static ResultsResponseCollection CheckM_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckM", res.ResultModel.CheckM.ToResponse()); }
        public static ResultsResponseCollection CheckM_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckM", value); }
        public static ResultsResponseCollection CheckN(this ResultsResponseCollection res) { return res.Val("#Results_CheckN", res.ResultModel.CheckN.ToResponse()); }
        public static ResultsResponseCollection CheckN(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckN", value); }
        public static ResultsResponseCollection CheckN_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckN", res.ResultModel.CheckN.ToResponse()); }
        public static ResultsResponseCollection CheckN_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckN", value); }
        public static ResultsResponseCollection CheckO(this ResultsResponseCollection res) { return res.Val("#Results_CheckO", res.ResultModel.CheckO.ToResponse()); }
        public static ResultsResponseCollection CheckO(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckO", value); }
        public static ResultsResponseCollection CheckO_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckO", res.ResultModel.CheckO.ToResponse()); }
        public static ResultsResponseCollection CheckO_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckO", value); }
        public static ResultsResponseCollection CheckP(this ResultsResponseCollection res) { return res.Val("#Results_CheckP", res.ResultModel.CheckP.ToResponse()); }
        public static ResultsResponseCollection CheckP(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckP", value); }
        public static ResultsResponseCollection CheckP_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckP", res.ResultModel.CheckP.ToResponse()); }
        public static ResultsResponseCollection CheckP_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckP", value); }
        public static ResultsResponseCollection CheckQ(this ResultsResponseCollection res) { return res.Val("#Results_CheckQ", res.ResultModel.CheckQ.ToResponse()); }
        public static ResultsResponseCollection CheckQ(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckQ", value); }
        public static ResultsResponseCollection CheckQ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckQ", res.ResultModel.CheckQ.ToResponse()); }
        public static ResultsResponseCollection CheckQ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckQ", value); }
        public static ResultsResponseCollection CheckR(this ResultsResponseCollection res) { return res.Val("#Results_CheckR", res.ResultModel.CheckR.ToResponse()); }
        public static ResultsResponseCollection CheckR(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckR", value); }
        public static ResultsResponseCollection CheckR_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckR", res.ResultModel.CheckR.ToResponse()); }
        public static ResultsResponseCollection CheckR_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckR", value); }
        public static ResultsResponseCollection CheckS(this ResultsResponseCollection res) { return res.Val("#Results_CheckS", res.ResultModel.CheckS.ToResponse()); }
        public static ResultsResponseCollection CheckS(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckS", value); }
        public static ResultsResponseCollection CheckS_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckS", res.ResultModel.CheckS.ToResponse()); }
        public static ResultsResponseCollection CheckS_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckS", value); }
        public static ResultsResponseCollection CheckT(this ResultsResponseCollection res) { return res.Val("#Results_CheckT", res.ResultModel.CheckT.ToResponse()); }
        public static ResultsResponseCollection CheckT(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckT", value); }
        public static ResultsResponseCollection CheckT_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckT", res.ResultModel.CheckT.ToResponse()); }
        public static ResultsResponseCollection CheckT_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckT", value); }
        public static ResultsResponseCollection CheckU(this ResultsResponseCollection res) { return res.Val("#Results_CheckU", res.ResultModel.CheckU.ToResponse()); }
        public static ResultsResponseCollection CheckU(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckU", value); }
        public static ResultsResponseCollection CheckU_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckU", res.ResultModel.CheckU.ToResponse()); }
        public static ResultsResponseCollection CheckU_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckU", value); }
        public static ResultsResponseCollection CheckV(this ResultsResponseCollection res) { return res.Val("#Results_CheckV", res.ResultModel.CheckV.ToResponse()); }
        public static ResultsResponseCollection CheckV(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckV", value); }
        public static ResultsResponseCollection CheckV_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckV", res.ResultModel.CheckV.ToResponse()); }
        public static ResultsResponseCollection CheckV_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckV", value); }
        public static ResultsResponseCollection CheckW(this ResultsResponseCollection res) { return res.Val("#Results_CheckW", res.ResultModel.CheckW.ToResponse()); }
        public static ResultsResponseCollection CheckW(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckW", value); }
        public static ResultsResponseCollection CheckW_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckW", res.ResultModel.CheckW.ToResponse()); }
        public static ResultsResponseCollection CheckW_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckW", value); }
        public static ResultsResponseCollection CheckX(this ResultsResponseCollection res) { return res.Val("#Results_CheckX", res.ResultModel.CheckX.ToResponse()); }
        public static ResultsResponseCollection CheckX(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckX", value); }
        public static ResultsResponseCollection CheckX_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckX", res.ResultModel.CheckX.ToResponse()); }
        public static ResultsResponseCollection CheckX_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckX", value); }
        public static ResultsResponseCollection CheckY(this ResultsResponseCollection res) { return res.Val("#Results_CheckY", res.ResultModel.CheckY.ToResponse()); }
        public static ResultsResponseCollection CheckY(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckY", value); }
        public static ResultsResponseCollection CheckY_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckY", res.ResultModel.CheckY.ToResponse()); }
        public static ResultsResponseCollection CheckY_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckY", value); }
        public static ResultsResponseCollection CheckZ(this ResultsResponseCollection res) { return res.Val("#Results_CheckZ", res.ResultModel.CheckZ.ToResponse()); }
        public static ResultsResponseCollection CheckZ(this ResultsResponseCollection res, string value) { return res.Val("#Results_CheckZ", value); }
        public static ResultsResponseCollection CheckZ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CheckZ", res.ResultModel.CheckZ.ToResponse()); }
        public static ResultsResponseCollection CheckZ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CheckZ", value); }
        public static ResultsResponseCollection AttachmentsA(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsA", res.ResultModel.AttachmentsA.ToResponse()); }
        public static ResultsResponseCollection AttachmentsA(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsA", value); }
        public static ResultsResponseCollection AttachmentsA_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsA", res.ResultModel.AttachmentsA.ToResponse()); }
        public static ResultsResponseCollection AttachmentsA_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsA", value); }
        public static ResultsResponseCollection AttachmentsB(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsB", res.ResultModel.AttachmentsB.ToResponse()); }
        public static ResultsResponseCollection AttachmentsB(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsB", value); }
        public static ResultsResponseCollection AttachmentsB_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsB", res.ResultModel.AttachmentsB.ToResponse()); }
        public static ResultsResponseCollection AttachmentsB_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsB", value); }
        public static ResultsResponseCollection AttachmentsC(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsC", res.ResultModel.AttachmentsC.ToResponse()); }
        public static ResultsResponseCollection AttachmentsC(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsC", value); }
        public static ResultsResponseCollection AttachmentsC_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsC", res.ResultModel.AttachmentsC.ToResponse()); }
        public static ResultsResponseCollection AttachmentsC_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsC", value); }
        public static ResultsResponseCollection AttachmentsD(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsD", res.ResultModel.AttachmentsD.ToResponse()); }
        public static ResultsResponseCollection AttachmentsD(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsD", value); }
        public static ResultsResponseCollection AttachmentsD_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsD", res.ResultModel.AttachmentsD.ToResponse()); }
        public static ResultsResponseCollection AttachmentsD_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsD", value); }
        public static ResultsResponseCollection AttachmentsE(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsE", res.ResultModel.AttachmentsE.ToResponse()); }
        public static ResultsResponseCollection AttachmentsE(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsE", value); }
        public static ResultsResponseCollection AttachmentsE_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsE", res.ResultModel.AttachmentsE.ToResponse()); }
        public static ResultsResponseCollection AttachmentsE_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsE", value); }
        public static ResultsResponseCollection AttachmentsF(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsF", res.ResultModel.AttachmentsF.ToResponse()); }
        public static ResultsResponseCollection AttachmentsF(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsF", value); }
        public static ResultsResponseCollection AttachmentsF_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsF", res.ResultModel.AttachmentsF.ToResponse()); }
        public static ResultsResponseCollection AttachmentsF_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsF", value); }
        public static ResultsResponseCollection AttachmentsG(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsG", res.ResultModel.AttachmentsG.ToResponse()); }
        public static ResultsResponseCollection AttachmentsG(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsG", value); }
        public static ResultsResponseCollection AttachmentsG_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsG", res.ResultModel.AttachmentsG.ToResponse()); }
        public static ResultsResponseCollection AttachmentsG_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsG", value); }
        public static ResultsResponseCollection AttachmentsH(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsH", res.ResultModel.AttachmentsH.ToResponse()); }
        public static ResultsResponseCollection AttachmentsH(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsH", value); }
        public static ResultsResponseCollection AttachmentsH_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsH", res.ResultModel.AttachmentsH.ToResponse()); }
        public static ResultsResponseCollection AttachmentsH_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsH", value); }
        public static ResultsResponseCollection AttachmentsI(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsI", res.ResultModel.AttachmentsI.ToResponse()); }
        public static ResultsResponseCollection AttachmentsI(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsI", value); }
        public static ResultsResponseCollection AttachmentsI_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsI", res.ResultModel.AttachmentsI.ToResponse()); }
        public static ResultsResponseCollection AttachmentsI_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsI", value); }
        public static ResultsResponseCollection AttachmentsJ(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsJ", res.ResultModel.AttachmentsJ.ToResponse()); }
        public static ResultsResponseCollection AttachmentsJ(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsJ", value); }
        public static ResultsResponseCollection AttachmentsJ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsJ", res.ResultModel.AttachmentsJ.ToResponse()); }
        public static ResultsResponseCollection AttachmentsJ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsJ", value); }
        public static ResultsResponseCollection AttachmentsK(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsK", res.ResultModel.AttachmentsK.ToResponse()); }
        public static ResultsResponseCollection AttachmentsK(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsK", value); }
        public static ResultsResponseCollection AttachmentsK_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsK", res.ResultModel.AttachmentsK.ToResponse()); }
        public static ResultsResponseCollection AttachmentsK_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsK", value); }
        public static ResultsResponseCollection AttachmentsL(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsL", res.ResultModel.AttachmentsL.ToResponse()); }
        public static ResultsResponseCollection AttachmentsL(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsL", value); }
        public static ResultsResponseCollection AttachmentsL_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsL", res.ResultModel.AttachmentsL.ToResponse()); }
        public static ResultsResponseCollection AttachmentsL_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsL", value); }
        public static ResultsResponseCollection AttachmentsM(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsM", res.ResultModel.AttachmentsM.ToResponse()); }
        public static ResultsResponseCollection AttachmentsM(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsM", value); }
        public static ResultsResponseCollection AttachmentsM_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsM", res.ResultModel.AttachmentsM.ToResponse()); }
        public static ResultsResponseCollection AttachmentsM_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsM", value); }
        public static ResultsResponseCollection AttachmentsN(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsN", res.ResultModel.AttachmentsN.ToResponse()); }
        public static ResultsResponseCollection AttachmentsN(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsN", value); }
        public static ResultsResponseCollection AttachmentsN_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsN", res.ResultModel.AttachmentsN.ToResponse()); }
        public static ResultsResponseCollection AttachmentsN_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsN", value); }
        public static ResultsResponseCollection AttachmentsO(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsO", res.ResultModel.AttachmentsO.ToResponse()); }
        public static ResultsResponseCollection AttachmentsO(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsO", value); }
        public static ResultsResponseCollection AttachmentsO_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsO", res.ResultModel.AttachmentsO.ToResponse()); }
        public static ResultsResponseCollection AttachmentsO_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsO", value); }
        public static ResultsResponseCollection AttachmentsP(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsP", res.ResultModel.AttachmentsP.ToResponse()); }
        public static ResultsResponseCollection AttachmentsP(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsP", value); }
        public static ResultsResponseCollection AttachmentsP_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsP", res.ResultModel.AttachmentsP.ToResponse()); }
        public static ResultsResponseCollection AttachmentsP_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsP", value); }
        public static ResultsResponseCollection AttachmentsQ(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsQ", res.ResultModel.AttachmentsQ.ToResponse()); }
        public static ResultsResponseCollection AttachmentsQ(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsQ", value); }
        public static ResultsResponseCollection AttachmentsQ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsQ", res.ResultModel.AttachmentsQ.ToResponse()); }
        public static ResultsResponseCollection AttachmentsQ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsQ", value); }
        public static ResultsResponseCollection AttachmentsR(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsR", res.ResultModel.AttachmentsR.ToResponse()); }
        public static ResultsResponseCollection AttachmentsR(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsR", value); }
        public static ResultsResponseCollection AttachmentsR_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsR", res.ResultModel.AttachmentsR.ToResponse()); }
        public static ResultsResponseCollection AttachmentsR_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsR", value); }
        public static ResultsResponseCollection AttachmentsS(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsS", res.ResultModel.AttachmentsS.ToResponse()); }
        public static ResultsResponseCollection AttachmentsS(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsS", value); }
        public static ResultsResponseCollection AttachmentsS_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsS", res.ResultModel.AttachmentsS.ToResponse()); }
        public static ResultsResponseCollection AttachmentsS_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsS", value); }
        public static ResultsResponseCollection AttachmentsT(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsT", res.ResultModel.AttachmentsT.ToResponse()); }
        public static ResultsResponseCollection AttachmentsT(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsT", value); }
        public static ResultsResponseCollection AttachmentsT_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsT", res.ResultModel.AttachmentsT.ToResponse()); }
        public static ResultsResponseCollection AttachmentsT_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsT", value); }
        public static ResultsResponseCollection AttachmentsU(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsU", res.ResultModel.AttachmentsU.ToResponse()); }
        public static ResultsResponseCollection AttachmentsU(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsU", value); }
        public static ResultsResponseCollection AttachmentsU_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsU", res.ResultModel.AttachmentsU.ToResponse()); }
        public static ResultsResponseCollection AttachmentsU_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsU", value); }
        public static ResultsResponseCollection AttachmentsV(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsV", res.ResultModel.AttachmentsV.ToResponse()); }
        public static ResultsResponseCollection AttachmentsV(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsV", value); }
        public static ResultsResponseCollection AttachmentsV_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsV", res.ResultModel.AttachmentsV.ToResponse()); }
        public static ResultsResponseCollection AttachmentsV_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsV", value); }
        public static ResultsResponseCollection AttachmentsW(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsW", res.ResultModel.AttachmentsW.ToResponse()); }
        public static ResultsResponseCollection AttachmentsW(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsW", value); }
        public static ResultsResponseCollection AttachmentsW_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsW", res.ResultModel.AttachmentsW.ToResponse()); }
        public static ResultsResponseCollection AttachmentsW_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsW", value); }
        public static ResultsResponseCollection AttachmentsX(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsX", res.ResultModel.AttachmentsX.ToResponse()); }
        public static ResultsResponseCollection AttachmentsX(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsX", value); }
        public static ResultsResponseCollection AttachmentsX_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsX", res.ResultModel.AttachmentsX.ToResponse()); }
        public static ResultsResponseCollection AttachmentsX_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsX", value); }
        public static ResultsResponseCollection AttachmentsY(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsY", res.ResultModel.AttachmentsY.ToResponse()); }
        public static ResultsResponseCollection AttachmentsY(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsY", value); }
        public static ResultsResponseCollection AttachmentsY_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsY", res.ResultModel.AttachmentsY.ToResponse()); }
        public static ResultsResponseCollection AttachmentsY_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsY", value); }
        public static ResultsResponseCollection AttachmentsZ(this ResultsResponseCollection res) { return res.Val("#Results_AttachmentsZ", res.ResultModel.AttachmentsZ.ToResponse()); }
        public static ResultsResponseCollection AttachmentsZ(this ResultsResponseCollection res, string value) { return res.Val("#Results_AttachmentsZ", value); }
        public static ResultsResponseCollection AttachmentsZ_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_AttachmentsZ", res.ResultModel.AttachmentsZ.ToResponse()); }
        public static ResultsResponseCollection AttachmentsZ_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_AttachmentsZ", value); }
        public static ResultsResponseCollection Comments(this ResultsResponseCollection res) { return res.Val("#Results_Comments", res.ResultModel.Comments.ToResponse()); }
        public static ResultsResponseCollection Comments(this ResultsResponseCollection res, string value) { return res.Val("#Results_Comments", value); }
        public static ResultsResponseCollection Comments_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_Comments", res.ResultModel.Comments.ToResponse()); }
        public static ResultsResponseCollection Comments_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_Comments", value); }
        public static ResultsResponseCollection CreatedTime(this ResultsResponseCollection res) { return res.Val("#Results_CreatedTime", res.ResultModel.CreatedTime.ToResponse()); }
        public static ResultsResponseCollection CreatedTime(this ResultsResponseCollection res, string value) { return res.Val("#Results_CreatedTime", value); }
        public static ResultsResponseCollection CreatedTime_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_CreatedTime", res.ResultModel.CreatedTime.ToResponse()); }
        public static ResultsResponseCollection CreatedTime_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_CreatedTime", value); }
        public static ResultsResponseCollection Timestamp(this ResultsResponseCollection res) { return res.Val("#Results_Timestamp", res.ResultModel.Timestamp.ToResponse()); }
        public static ResultsResponseCollection Timestamp(this ResultsResponseCollection res, string value) { return res.Val("#Results_Timestamp", value); }
        public static ResultsResponseCollection Timestamp_FormData(this ResultsResponseCollection res) { return res.ValAndFormData("#Results_Timestamp", res.ResultModel.Timestamp.ToResponse()); }
        public static ResultsResponseCollection Timestamp_FormData(this ResultsResponseCollection res, string value) { return res.ValAndFormData("#Results_Timestamp", value); }
        public static WikisResponseCollection UpdatedTime(this WikisResponseCollection res) { return res.Val("#Wikis_UpdatedTime", res.WikiModel.UpdatedTime.ToResponse()); }
        public static WikisResponseCollection UpdatedTime(this WikisResponseCollection res, string value) { return res.Val("#Wikis_UpdatedTime", value); }
        public static WikisResponseCollection UpdatedTime_FormData(this WikisResponseCollection res) { return res.ValAndFormData("#Wikis_UpdatedTime", res.WikiModel.UpdatedTime.ToResponse()); }
        public static WikisResponseCollection UpdatedTime_FormData(this WikisResponseCollection res, string value) { return res.ValAndFormData("#Wikis_UpdatedTime", value); }
        public static WikisResponseCollection WikiId(this WikisResponseCollection res) { return res.Val("#Wikis_WikiId", res.WikiModel.WikiId.ToResponse()); }
        public static WikisResponseCollection WikiId(this WikisResponseCollection res, string value) { return res.Val("#Wikis_WikiId", value); }
        public static WikisResponseCollection WikiId_FormData(this WikisResponseCollection res) { return res.ValAndFormData("#Wikis_WikiId", res.WikiModel.WikiId.ToResponse()); }
        public static WikisResponseCollection WikiId_FormData(this WikisResponseCollection res, string value) { return res.ValAndFormData("#Wikis_WikiId", value); }
        public static WikisResponseCollection Ver(this WikisResponseCollection res) { return res.Val("#Wikis_Ver", res.WikiModel.Ver.ToResponse()); }
        public static WikisResponseCollection Ver(this WikisResponseCollection res, string value) { return res.Val("#Wikis_Ver", value); }
        public static WikisResponseCollection Ver_FormData(this WikisResponseCollection res) { return res.ValAndFormData("#Wikis_Ver", res.WikiModel.Ver.ToResponse()); }
        public static WikisResponseCollection Ver_FormData(this WikisResponseCollection res, string value) { return res.ValAndFormData("#Wikis_Ver", value); }
        public static WikisResponseCollection Title(this WikisResponseCollection res) { return res.Val("#Wikis_Title", res.WikiModel.Title.ToResponse()); }
        public static WikisResponseCollection Title(this WikisResponseCollection res, string value) { return res.Val("#Wikis_Title", value); }
        public static WikisResponseCollection Title_FormData(this WikisResponseCollection res) { return res.ValAndFormData("#Wikis_Title", res.WikiModel.Title.ToResponse()); }
        public static WikisResponseCollection Title_FormData(this WikisResponseCollection res, string value) { return res.ValAndFormData("#Wikis_Title", value); }
        public static WikisResponseCollection Body(this WikisResponseCollection res) { return res.Val("#Wikis_Body", res.WikiModel.Body.ToResponse()); }
        public static WikisResponseCollection Body(this WikisResponseCollection res, string value) { return res.Val("#Wikis_Body", value); }
        public static WikisResponseCollection Body_FormData(this WikisResponseCollection res) { return res.ValAndFormData("#Wikis_Body", res.WikiModel.Body.ToResponse()); }
        public static WikisResponseCollection Body_FormData(this WikisResponseCollection res, string value) { return res.ValAndFormData("#Wikis_Body", value); }
        public static WikisResponseCollection Comments(this WikisResponseCollection res) { return res.Val("#Wikis_Comments", res.WikiModel.Comments.ToResponse()); }
        public static WikisResponseCollection Comments(this WikisResponseCollection res, string value) { return res.Val("#Wikis_Comments", value); }
        public static WikisResponseCollection Comments_FormData(this WikisResponseCollection res) { return res.ValAndFormData("#Wikis_Comments", res.WikiModel.Comments.ToResponse()); }
        public static WikisResponseCollection Comments_FormData(this WikisResponseCollection res, string value) { return res.ValAndFormData("#Wikis_Comments", value); }
        public static WikisResponseCollection CreatedTime(this WikisResponseCollection res) { return res.Val("#Wikis_CreatedTime", res.WikiModel.CreatedTime.ToResponse()); }
        public static WikisResponseCollection CreatedTime(this WikisResponseCollection res, string value) { return res.Val("#Wikis_CreatedTime", value); }
        public static WikisResponseCollection CreatedTime_FormData(this WikisResponseCollection res) { return res.ValAndFormData("#Wikis_CreatedTime", res.WikiModel.CreatedTime.ToResponse()); }
        public static WikisResponseCollection CreatedTime_FormData(this WikisResponseCollection res, string value) { return res.ValAndFormData("#Wikis_CreatedTime", value); }
        public static WikisResponseCollection Timestamp(this WikisResponseCollection res) { return res.Val("#Wikis_Timestamp", res.WikiModel.Timestamp.ToResponse()); }
        public static WikisResponseCollection Timestamp(this WikisResponseCollection res, string value) { return res.Val("#Wikis_Timestamp", value); }
        public static WikisResponseCollection Timestamp_FormData(this WikisResponseCollection res) { return res.ValAndFormData("#Wikis_Timestamp", res.WikiModel.Timestamp.ToResponse()); }
        public static WikisResponseCollection Timestamp_FormData(this WikisResponseCollection res, string value) { return res.ValAndFormData("#Wikis_Timestamp", value); }
    }
}
