using Implem.Pleasanter.Libraries.Converts;
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
        public static TenantsResponseCollection Ver(this TenantsResponseCollection responseCollection) { return responseCollection.Val("#Tenants_Ver", responseCollection.TenantModel.Ver.ToResponse()); }
        public static TenantsResponseCollection Ver(this TenantsResponseCollection responseCollection, string value) { return responseCollection.Val("#Tenants_Ver", value); }
        public static TenantsResponseCollection Ver_FormData(this TenantsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Tenants_Ver", responseCollection.TenantModel.Ver.ToResponse()); }
        public static TenantsResponseCollection Ver_FormData(this TenantsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Tenants_Ver", value); }
        public static TenantsResponseCollection Title(this TenantsResponseCollection responseCollection) { return responseCollection.Val("#Tenants_Title", responseCollection.TenantModel.Title.ToResponse()); }
        public static TenantsResponseCollection Title(this TenantsResponseCollection responseCollection, string value) { return responseCollection.Val("#Tenants_Title", value); }
        public static TenantsResponseCollection Title_FormData(this TenantsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Tenants_Title", responseCollection.TenantModel.Title.ToResponse()); }
        public static TenantsResponseCollection Title_FormData(this TenantsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Tenants_Title", value); }
        public static TenantsResponseCollection Body(this TenantsResponseCollection responseCollection) { return responseCollection.Val("#Tenants_Body", responseCollection.TenantModel.Body.ToResponse()); }
        public static TenantsResponseCollection Body(this TenantsResponseCollection responseCollection, string value) { return responseCollection.Val("#Tenants_Body", value); }
        public static TenantsResponseCollection Body_FormData(this TenantsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Tenants_Body", responseCollection.TenantModel.Body.ToResponse()); }
        public static TenantsResponseCollection Body_FormData(this TenantsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Tenants_Body", value); }
        public static TenantsResponseCollection CreatedTime(this TenantsResponseCollection responseCollection) { return responseCollection.Val("#Tenants_CreatedTime", responseCollection.TenantModel.CreatedTime.ToResponse()); }
        public static TenantsResponseCollection CreatedTime(this TenantsResponseCollection responseCollection, string value) { return responseCollection.Val("#Tenants_CreatedTime", value); }
        public static TenantsResponseCollection CreatedTime_FormData(this TenantsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Tenants_CreatedTime", responseCollection.TenantModel.CreatedTime.ToResponse()); }
        public static TenantsResponseCollection CreatedTime_FormData(this TenantsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Tenants_CreatedTime", value); }
        public static TenantsResponseCollection UpdatedTime(this TenantsResponseCollection responseCollection) { return responseCollection.Val("#Tenants_UpdatedTime", responseCollection.TenantModel.UpdatedTime.ToResponse()); }
        public static TenantsResponseCollection UpdatedTime(this TenantsResponseCollection responseCollection, string value) { return responseCollection.Val("#Tenants_UpdatedTime", value); }
        public static TenantsResponseCollection UpdatedTime_FormData(this TenantsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Tenants_UpdatedTime", responseCollection.TenantModel.UpdatedTime.ToResponse()); }
        public static TenantsResponseCollection UpdatedTime_FormData(this TenantsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Tenants_UpdatedTime", value); }
        public static TenantsResponseCollection Timestamp(this TenantsResponseCollection responseCollection) { return responseCollection.Val("#Tenants_Timestamp", responseCollection.TenantModel.Timestamp.ToResponse()); }
        public static TenantsResponseCollection Timestamp(this TenantsResponseCollection responseCollection, string value) { return responseCollection.Val("#Tenants_Timestamp", value); }
        public static TenantsResponseCollection Timestamp_FormData(this TenantsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Tenants_Timestamp", responseCollection.TenantModel.Timestamp.ToResponse()); }
        public static TenantsResponseCollection Timestamp_FormData(this TenantsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Tenants_Timestamp", value); }
        public static DemosResponseCollection Ver(this DemosResponseCollection responseCollection) { return responseCollection.Val("#Demos_Ver", responseCollection.DemoModel.Ver.ToResponse()); }
        public static DemosResponseCollection Ver(this DemosResponseCollection responseCollection, string value) { return responseCollection.Val("#Demos_Ver", value); }
        public static DemosResponseCollection Ver_FormData(this DemosResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Demos_Ver", responseCollection.DemoModel.Ver.ToResponse()); }
        public static DemosResponseCollection Ver_FormData(this DemosResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Demos_Ver", value); }
        public static DemosResponseCollection CreatedTime(this DemosResponseCollection responseCollection) { return responseCollection.Val("#Demos_CreatedTime", responseCollection.DemoModel.CreatedTime.ToResponse()); }
        public static DemosResponseCollection CreatedTime(this DemosResponseCollection responseCollection, string value) { return responseCollection.Val("#Demos_CreatedTime", value); }
        public static DemosResponseCollection CreatedTime_FormData(this DemosResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Demos_CreatedTime", responseCollection.DemoModel.CreatedTime.ToResponse()); }
        public static DemosResponseCollection CreatedTime_FormData(this DemosResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Demos_CreatedTime", value); }
        public static DemosResponseCollection UpdatedTime(this DemosResponseCollection responseCollection) { return responseCollection.Val("#Demos_UpdatedTime", responseCollection.DemoModel.UpdatedTime.ToResponse()); }
        public static DemosResponseCollection UpdatedTime(this DemosResponseCollection responseCollection, string value) { return responseCollection.Val("#Demos_UpdatedTime", value); }
        public static DemosResponseCollection UpdatedTime_FormData(this DemosResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Demos_UpdatedTime", responseCollection.DemoModel.UpdatedTime.ToResponse()); }
        public static DemosResponseCollection UpdatedTime_FormData(this DemosResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Demos_UpdatedTime", value); }
        public static DemosResponseCollection Timestamp(this DemosResponseCollection responseCollection) { return responseCollection.Val("#Demos_Timestamp", responseCollection.DemoModel.Timestamp.ToResponse()); }
        public static DemosResponseCollection Timestamp(this DemosResponseCollection responseCollection, string value) { return responseCollection.Val("#Demos_Timestamp", value); }
        public static DemosResponseCollection Timestamp_FormData(this DemosResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Demos_Timestamp", responseCollection.DemoModel.Timestamp.ToResponse()); }
        public static DemosResponseCollection Timestamp_FormData(this DemosResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Demos_Timestamp", value); }
        public static DeptsResponseCollection TenantId(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_TenantId", responseCollection.DeptModel.TenantId.ToResponse()); }
        public static DeptsResponseCollection TenantId(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_TenantId", value); }
        public static DeptsResponseCollection TenantId_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_TenantId", responseCollection.DeptModel.TenantId.ToResponse()); }
        public static DeptsResponseCollection TenantId_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_TenantId", value); }
        public static DeptsResponseCollection DeptId(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_DeptId", responseCollection.DeptModel.DeptId.ToResponse()); }
        public static DeptsResponseCollection DeptId(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_DeptId", value); }
        public static DeptsResponseCollection DeptId_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_DeptId", responseCollection.DeptModel.DeptId.ToResponse()); }
        public static DeptsResponseCollection DeptId_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_DeptId", value); }
        public static DeptsResponseCollection Ver(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_Ver", responseCollection.DeptModel.Ver.ToResponse()); }
        public static DeptsResponseCollection Ver(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_Ver", value); }
        public static DeptsResponseCollection Ver_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_Ver", responseCollection.DeptModel.Ver.ToResponse()); }
        public static DeptsResponseCollection Ver_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_Ver", value); }
        public static DeptsResponseCollection ParentDeptId(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_ParentDeptId", responseCollection.DeptModel.ParentDeptId.ToResponse()); }
        public static DeptsResponseCollection ParentDeptId(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_ParentDeptId", value); }
        public static DeptsResponseCollection ParentDeptId_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_ParentDeptId", responseCollection.DeptModel.ParentDeptId.ToResponse()); }
        public static DeptsResponseCollection ParentDeptId_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_ParentDeptId", value); }
        public static DeptsResponseCollection DeptCode(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_DeptCode", responseCollection.DeptModel.DeptCode.ToResponse()); }
        public static DeptsResponseCollection DeptCode(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_DeptCode", value); }
        public static DeptsResponseCollection DeptCode_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_DeptCode", responseCollection.DeptModel.DeptCode.ToResponse()); }
        public static DeptsResponseCollection DeptCode_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_DeptCode", value); }
        public static DeptsResponseCollection DeptName(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_DeptName", responseCollection.DeptModel.DeptName.ToResponse()); }
        public static DeptsResponseCollection DeptName(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_DeptName", value); }
        public static DeptsResponseCollection DeptName_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_DeptName", responseCollection.DeptModel.DeptName.ToResponse()); }
        public static DeptsResponseCollection DeptName_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_DeptName", value); }
        public static DeptsResponseCollection Body(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_Body", responseCollection.DeptModel.Body.ToResponse()); }
        public static DeptsResponseCollection Body(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_Body", value); }
        public static DeptsResponseCollection Body_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_Body", responseCollection.DeptModel.Body.ToResponse()); }
        public static DeptsResponseCollection Body_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_Body", value); }
        public static DeptsResponseCollection CreatedTime(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_CreatedTime", responseCollection.DeptModel.CreatedTime.ToResponse()); }
        public static DeptsResponseCollection CreatedTime(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_CreatedTime", value); }
        public static DeptsResponseCollection CreatedTime_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_CreatedTime", responseCollection.DeptModel.CreatedTime.ToResponse()); }
        public static DeptsResponseCollection CreatedTime_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_CreatedTime", value); }
        public static DeptsResponseCollection UpdatedTime(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_UpdatedTime", responseCollection.DeptModel.UpdatedTime.ToResponse()); }
        public static DeptsResponseCollection UpdatedTime(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_UpdatedTime", value); }
        public static DeptsResponseCollection UpdatedTime_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_UpdatedTime", responseCollection.DeptModel.UpdatedTime.ToResponse()); }
        public static DeptsResponseCollection UpdatedTime_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_UpdatedTime", value); }
        public static DeptsResponseCollection Timestamp(this DeptsResponseCollection responseCollection) { return responseCollection.Val("#Depts_Timestamp", responseCollection.DeptModel.Timestamp.ToResponse()); }
        public static DeptsResponseCollection Timestamp(this DeptsResponseCollection responseCollection, string value) { return responseCollection.Val("#Depts_Timestamp", value); }
        public static DeptsResponseCollection Timestamp_FormData(this DeptsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Depts_Timestamp", responseCollection.DeptModel.Timestamp.ToResponse()); }
        public static DeptsResponseCollection Timestamp_FormData(this DeptsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Depts_Timestamp", value); }
        public static UsersResponseCollection UserId(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_UserId", responseCollection.UserModel.UserId.ToResponse()); }
        public static UsersResponseCollection UserId(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_UserId", value); }
        public static UsersResponseCollection UserId_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_UserId", responseCollection.UserModel.UserId.ToResponse()); }
        public static UsersResponseCollection UserId_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_UserId", value); }
        public static UsersResponseCollection Ver(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_Ver", responseCollection.UserModel.Ver.ToResponse()); }
        public static UsersResponseCollection Ver(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_Ver", value); }
        public static UsersResponseCollection Ver_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_Ver", responseCollection.UserModel.Ver.ToResponse()); }
        public static UsersResponseCollection Ver_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_Ver", value); }
        public static UsersResponseCollection LoginId(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_LoginId", responseCollection.UserModel.LoginId.ToResponse()); }
        public static UsersResponseCollection LoginId(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_LoginId", value); }
        public static UsersResponseCollection LoginId_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_LoginId", responseCollection.UserModel.LoginId.ToResponse()); }
        public static UsersResponseCollection LoginId_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_LoginId", value); }
        public static UsersResponseCollection Disabled(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_Disabled", responseCollection.UserModel.Disabled.ToResponse()); }
        public static UsersResponseCollection Disabled(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_Disabled", value); }
        public static UsersResponseCollection Disabled_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_Disabled", responseCollection.UserModel.Disabled.ToResponse()); }
        public static UsersResponseCollection Disabled_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_Disabled", value); }
        public static UsersResponseCollection Password(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_Password", responseCollection.UserModel.Password.ToResponse()); }
        public static UsersResponseCollection Password(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_Password", value); }
        public static UsersResponseCollection Password_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_Password", responseCollection.UserModel.Password.ToResponse()); }
        public static UsersResponseCollection Password_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_Password", value); }
        public static UsersResponseCollection PasswordValidate(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_PasswordValidate", responseCollection.UserModel.PasswordValidate.ToResponse()); }
        public static UsersResponseCollection PasswordValidate(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_PasswordValidate", value); }
        public static UsersResponseCollection PasswordValidate_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_PasswordValidate", responseCollection.UserModel.PasswordValidate.ToResponse()); }
        public static UsersResponseCollection PasswordValidate_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_PasswordValidate", value); }
        public static UsersResponseCollection PasswordDummy(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_PasswordDummy", responseCollection.UserModel.PasswordDummy.ToResponse()); }
        public static UsersResponseCollection PasswordDummy(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_PasswordDummy", value); }
        public static UsersResponseCollection PasswordDummy_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_PasswordDummy", responseCollection.UserModel.PasswordDummy.ToResponse()); }
        public static UsersResponseCollection PasswordDummy_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_PasswordDummy", value); }
        public static UsersResponseCollection RememberMe(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_RememberMe", responseCollection.UserModel.RememberMe.ToResponse()); }
        public static UsersResponseCollection RememberMe(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_RememberMe", value); }
        public static UsersResponseCollection RememberMe_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_RememberMe", responseCollection.UserModel.RememberMe.ToResponse()); }
        public static UsersResponseCollection RememberMe_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_RememberMe", value); }
        public static UsersResponseCollection LastName(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_LastName", responseCollection.UserModel.LastName.ToResponse()); }
        public static UsersResponseCollection LastName(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_LastName", value); }
        public static UsersResponseCollection LastName_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_LastName", responseCollection.UserModel.LastName.ToResponse()); }
        public static UsersResponseCollection LastName_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_LastName", value); }
        public static UsersResponseCollection FirstName(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_FirstName", responseCollection.UserModel.FirstName.ToResponse()); }
        public static UsersResponseCollection FirstName(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_FirstName", value); }
        public static UsersResponseCollection FirstName_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_FirstName", responseCollection.UserModel.FirstName.ToResponse()); }
        public static UsersResponseCollection FirstName_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_FirstName", value); }
        public static UsersResponseCollection Birthday(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_Birthday", responseCollection.UserModel.Birthday.ToResponse()); }
        public static UsersResponseCollection Birthday(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_Birthday", value); }
        public static UsersResponseCollection Birthday_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_Birthday", responseCollection.UserModel.Birthday.ToResponse()); }
        public static UsersResponseCollection Birthday_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_Birthday", value); }
        public static UsersResponseCollection Sex(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_Sex", responseCollection.UserModel.Sex.ToResponse()); }
        public static UsersResponseCollection Sex(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_Sex", value); }
        public static UsersResponseCollection Sex_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_Sex", responseCollection.UserModel.Sex.ToResponse()); }
        public static UsersResponseCollection Sex_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_Sex", value); }
        public static UsersResponseCollection Language(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_Language", responseCollection.UserModel.Language.ToResponse()); }
        public static UsersResponseCollection Language(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_Language", value); }
        public static UsersResponseCollection Language_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_Language", responseCollection.UserModel.Language.ToResponse()); }
        public static UsersResponseCollection Language_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_Language", value); }
        public static UsersResponseCollection TimeZone(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_TimeZone", responseCollection.UserModel.TimeZone.ToResponse()); }
        public static UsersResponseCollection TimeZone(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_TimeZone", value); }
        public static UsersResponseCollection TimeZone_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_TimeZone", responseCollection.UserModel.TimeZone.ToResponse()); }
        public static UsersResponseCollection TimeZone_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_TimeZone", value); }
        public static UsersResponseCollection DeptId(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_DeptId", responseCollection.UserModel.DeptId.ToResponse()); }
        public static UsersResponseCollection DeptId(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_DeptId", value); }
        public static UsersResponseCollection DeptId_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_DeptId", responseCollection.UserModel.DeptId.ToResponse()); }
        public static UsersResponseCollection DeptId_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_DeptId", value); }
        public static UsersResponseCollection FirstAndLastNameOrder(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_FirstAndLastNameOrder", responseCollection.UserModel.FirstAndLastNameOrder.ToResponse()); }
        public static UsersResponseCollection FirstAndLastNameOrder(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_FirstAndLastNameOrder", value); }
        public static UsersResponseCollection FirstAndLastNameOrder_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_FirstAndLastNameOrder", responseCollection.UserModel.FirstAndLastNameOrder.ToResponse()); }
        public static UsersResponseCollection FirstAndLastNameOrder_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_FirstAndLastNameOrder", value); }
        public static UsersResponseCollection LastLoginTime(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_LastLoginTime", responseCollection.UserModel.LastLoginTime.ToResponse()); }
        public static UsersResponseCollection LastLoginTime(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_LastLoginTime", value); }
        public static UsersResponseCollection LastLoginTime_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_LastLoginTime", responseCollection.UserModel.LastLoginTime.ToResponse()); }
        public static UsersResponseCollection LastLoginTime_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_LastLoginTime", value); }
        public static UsersResponseCollection PasswordExpirationTime(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_PasswordExpirationTime", responseCollection.UserModel.PasswordExpirationTime.ToResponse()); }
        public static UsersResponseCollection PasswordExpirationTime(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_PasswordExpirationTime", value); }
        public static UsersResponseCollection PasswordExpirationTime_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_PasswordExpirationTime", responseCollection.UserModel.PasswordExpirationTime.ToResponse()); }
        public static UsersResponseCollection PasswordExpirationTime_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_PasswordExpirationTime", value); }
        public static UsersResponseCollection PasswordChangeTime(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_PasswordChangeTime", responseCollection.UserModel.PasswordChangeTime.ToResponse()); }
        public static UsersResponseCollection PasswordChangeTime(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_PasswordChangeTime", value); }
        public static UsersResponseCollection PasswordChangeTime_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_PasswordChangeTime", responseCollection.UserModel.PasswordChangeTime.ToResponse()); }
        public static UsersResponseCollection PasswordChangeTime_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_PasswordChangeTime", value); }
        public static UsersResponseCollection NumberOfLogins(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_NumberOfLogins", responseCollection.UserModel.NumberOfLogins.ToResponse()); }
        public static UsersResponseCollection NumberOfLogins(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_NumberOfLogins", value); }
        public static UsersResponseCollection NumberOfLogins_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_NumberOfLogins", responseCollection.UserModel.NumberOfLogins.ToResponse()); }
        public static UsersResponseCollection NumberOfLogins_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_NumberOfLogins", value); }
        public static UsersResponseCollection NumberOfDenial(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_NumberOfDenial", responseCollection.UserModel.NumberOfDenial.ToResponse()); }
        public static UsersResponseCollection NumberOfDenial(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_NumberOfDenial", value); }
        public static UsersResponseCollection NumberOfDenial_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_NumberOfDenial", responseCollection.UserModel.NumberOfDenial.ToResponse()); }
        public static UsersResponseCollection NumberOfDenial_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_NumberOfDenial", value); }
        public static UsersResponseCollection TenantAdmin(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_TenantAdmin", responseCollection.UserModel.TenantAdmin.ToResponse()); }
        public static UsersResponseCollection TenantAdmin(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_TenantAdmin", value); }
        public static UsersResponseCollection TenantAdmin_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_TenantAdmin", responseCollection.UserModel.TenantAdmin.ToResponse()); }
        public static UsersResponseCollection TenantAdmin_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_TenantAdmin", value); }
        public static UsersResponseCollection OldPassword(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_OldPassword", responseCollection.UserModel.OldPassword.ToResponse()); }
        public static UsersResponseCollection OldPassword(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_OldPassword", value); }
        public static UsersResponseCollection OldPassword_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_OldPassword", responseCollection.UserModel.OldPassword.ToResponse()); }
        public static UsersResponseCollection OldPassword_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_OldPassword", value); }
        public static UsersResponseCollection ChangedPassword(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_ChangedPassword", responseCollection.UserModel.ChangedPassword.ToResponse()); }
        public static UsersResponseCollection ChangedPassword(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_ChangedPassword", value); }
        public static UsersResponseCollection ChangedPassword_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_ChangedPassword", responseCollection.UserModel.ChangedPassword.ToResponse()); }
        public static UsersResponseCollection ChangedPassword_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_ChangedPassword", value); }
        public static UsersResponseCollection ChangedPasswordValidator(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_ChangedPasswordValidator", responseCollection.UserModel.ChangedPasswordValidator.ToResponse()); }
        public static UsersResponseCollection ChangedPasswordValidator(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_ChangedPasswordValidator", value); }
        public static UsersResponseCollection ChangedPasswordValidator_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_ChangedPasswordValidator", responseCollection.UserModel.ChangedPasswordValidator.ToResponse()); }
        public static UsersResponseCollection ChangedPasswordValidator_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_ChangedPasswordValidator", value); }
        public static UsersResponseCollection AfterResetPassword(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_AfterResetPassword", responseCollection.UserModel.AfterResetPassword.ToResponse()); }
        public static UsersResponseCollection AfterResetPassword(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_AfterResetPassword", value); }
        public static UsersResponseCollection AfterResetPassword_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_AfterResetPassword", responseCollection.UserModel.AfterResetPassword.ToResponse()); }
        public static UsersResponseCollection AfterResetPassword_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_AfterResetPassword", value); }
        public static UsersResponseCollection AfterResetPasswordValidator(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_AfterResetPasswordValidator", responseCollection.UserModel.AfterResetPasswordValidator.ToResponse()); }
        public static UsersResponseCollection AfterResetPasswordValidator(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_AfterResetPasswordValidator", value); }
        public static UsersResponseCollection AfterResetPasswordValidator_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_AfterResetPasswordValidator", responseCollection.UserModel.AfterResetPasswordValidator.ToResponse()); }
        public static UsersResponseCollection AfterResetPasswordValidator_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_AfterResetPasswordValidator", value); }
        public static UsersResponseCollection DemoMailAddress(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_DemoMailAddress", responseCollection.UserModel.DemoMailAddress.ToResponse()); }
        public static UsersResponseCollection DemoMailAddress(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_DemoMailAddress", value); }
        public static UsersResponseCollection DemoMailAddress_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_DemoMailAddress", responseCollection.UserModel.DemoMailAddress.ToResponse()); }
        public static UsersResponseCollection DemoMailAddress_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_DemoMailAddress", value); }
        public static UsersResponseCollection CreatedTime(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_CreatedTime", responseCollection.UserModel.CreatedTime.ToResponse()); }
        public static UsersResponseCollection CreatedTime(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_CreatedTime", value); }
        public static UsersResponseCollection CreatedTime_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_CreatedTime", responseCollection.UserModel.CreatedTime.ToResponse()); }
        public static UsersResponseCollection CreatedTime_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_CreatedTime", value); }
        public static UsersResponseCollection UpdatedTime(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_UpdatedTime", responseCollection.UserModel.UpdatedTime.ToResponse()); }
        public static UsersResponseCollection UpdatedTime(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_UpdatedTime", value); }
        public static UsersResponseCollection UpdatedTime_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_UpdatedTime", responseCollection.UserModel.UpdatedTime.ToResponse()); }
        public static UsersResponseCollection UpdatedTime_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_UpdatedTime", value); }
        public static UsersResponseCollection Timestamp(this UsersResponseCollection responseCollection) { return responseCollection.Val("#Users_Timestamp", responseCollection.UserModel.Timestamp.ToResponse()); }
        public static UsersResponseCollection Timestamp(this UsersResponseCollection responseCollection, string value) { return responseCollection.Val("#Users_Timestamp", value); }
        public static UsersResponseCollection Timestamp_FormData(this UsersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Users_Timestamp", responseCollection.UserModel.Timestamp.ToResponse()); }
        public static UsersResponseCollection Timestamp_FormData(this UsersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Users_Timestamp", value); }
        public static MailAddressesResponseCollection OwnerId(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_OwnerId", responseCollection.MailAddressModel.OwnerId.ToResponse()); }
        public static MailAddressesResponseCollection OwnerId(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_OwnerId", value); }
        public static MailAddressesResponseCollection OwnerId_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_OwnerId", responseCollection.MailAddressModel.OwnerId.ToResponse()); }
        public static MailAddressesResponseCollection OwnerId_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_OwnerId", value); }
        public static MailAddressesResponseCollection OwnerType(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_OwnerType", responseCollection.MailAddressModel.OwnerType.ToResponse()); }
        public static MailAddressesResponseCollection OwnerType(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_OwnerType", value); }
        public static MailAddressesResponseCollection OwnerType_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_OwnerType", responseCollection.MailAddressModel.OwnerType.ToResponse()); }
        public static MailAddressesResponseCollection OwnerType_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_OwnerType", value); }
        public static MailAddressesResponseCollection MailAddressId(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_MailAddressId", responseCollection.MailAddressModel.MailAddressId.ToResponse()); }
        public static MailAddressesResponseCollection MailAddressId(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_MailAddressId", value); }
        public static MailAddressesResponseCollection MailAddressId_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_MailAddressId", responseCollection.MailAddressModel.MailAddressId.ToResponse()); }
        public static MailAddressesResponseCollection MailAddressId_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_MailAddressId", value); }
        public static MailAddressesResponseCollection Ver(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_Ver", responseCollection.MailAddressModel.Ver.ToResponse()); }
        public static MailAddressesResponseCollection Ver(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_Ver", value); }
        public static MailAddressesResponseCollection Ver_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_Ver", responseCollection.MailAddressModel.Ver.ToResponse()); }
        public static MailAddressesResponseCollection Ver_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_Ver", value); }
        public static MailAddressesResponseCollection MailAddress(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_MailAddress", responseCollection.MailAddressModel.MailAddress.ToResponse()); }
        public static MailAddressesResponseCollection MailAddress(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_MailAddress", value); }
        public static MailAddressesResponseCollection MailAddress_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_MailAddress", responseCollection.MailAddressModel.MailAddress.ToResponse()); }
        public static MailAddressesResponseCollection MailAddress_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_MailAddress", value); }
        public static MailAddressesResponseCollection Title(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_Title", responseCollection.MailAddressModel.Title.ToResponse()); }
        public static MailAddressesResponseCollection Title(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_Title", value); }
        public static MailAddressesResponseCollection Title_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_Title", responseCollection.MailAddressModel.Title.ToResponse()); }
        public static MailAddressesResponseCollection Title_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_Title", value); }
        public static MailAddressesResponseCollection CreatedTime(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_CreatedTime", responseCollection.MailAddressModel.CreatedTime.ToResponse()); }
        public static MailAddressesResponseCollection CreatedTime(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_CreatedTime", value); }
        public static MailAddressesResponseCollection CreatedTime_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_CreatedTime", responseCollection.MailAddressModel.CreatedTime.ToResponse()); }
        public static MailAddressesResponseCollection CreatedTime_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_CreatedTime", value); }
        public static MailAddressesResponseCollection UpdatedTime(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_UpdatedTime", responseCollection.MailAddressModel.UpdatedTime.ToResponse()); }
        public static MailAddressesResponseCollection UpdatedTime(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_UpdatedTime", value); }
        public static MailAddressesResponseCollection UpdatedTime_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_UpdatedTime", responseCollection.MailAddressModel.UpdatedTime.ToResponse()); }
        public static MailAddressesResponseCollection UpdatedTime_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_UpdatedTime", value); }
        public static MailAddressesResponseCollection Timestamp(this MailAddressesResponseCollection responseCollection) { return responseCollection.Val("#MailAddresses_Timestamp", responseCollection.MailAddressModel.Timestamp.ToResponse()); }
        public static MailAddressesResponseCollection Timestamp(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.Val("#MailAddresses_Timestamp", value); }
        public static MailAddressesResponseCollection Timestamp_FormData(this MailAddressesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#MailAddresses_Timestamp", responseCollection.MailAddressModel.Timestamp.ToResponse()); }
        public static MailAddressesResponseCollection Timestamp_FormData(this MailAddressesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#MailAddresses_Timestamp", value); }
        public static PermissionsResponseCollection ReferenceType(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_ReferenceType", responseCollection.PermissionModel.ReferenceType.ToResponse()); }
        public static PermissionsResponseCollection ReferenceType(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_ReferenceType", value); }
        public static PermissionsResponseCollection ReferenceType_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_ReferenceType", responseCollection.PermissionModel.ReferenceType.ToResponse()); }
        public static PermissionsResponseCollection ReferenceType_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_ReferenceType", value); }
        public static PermissionsResponseCollection ReferenceId(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_ReferenceId", responseCollection.PermissionModel.ReferenceId.ToResponse()); }
        public static PermissionsResponseCollection ReferenceId(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_ReferenceId", value); }
        public static PermissionsResponseCollection ReferenceId_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_ReferenceId", responseCollection.PermissionModel.ReferenceId.ToResponse()); }
        public static PermissionsResponseCollection ReferenceId_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_ReferenceId", value); }
        public static PermissionsResponseCollection DeptId(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_DeptId", responseCollection.PermissionModel.DeptId.ToResponse()); }
        public static PermissionsResponseCollection DeptId(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_DeptId", value); }
        public static PermissionsResponseCollection DeptId_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_DeptId", responseCollection.PermissionModel.DeptId.ToResponse()); }
        public static PermissionsResponseCollection DeptId_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_DeptId", value); }
        public static PermissionsResponseCollection UserId(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_UserId", responseCollection.PermissionModel.UserId.ToResponse()); }
        public static PermissionsResponseCollection UserId(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_UserId", value); }
        public static PermissionsResponseCollection UserId_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_UserId", responseCollection.PermissionModel.UserId.ToResponse()); }
        public static PermissionsResponseCollection UserId_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_UserId", value); }
        public static PermissionsResponseCollection Ver(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_Ver", responseCollection.PermissionModel.Ver.ToResponse()); }
        public static PermissionsResponseCollection Ver(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_Ver", value); }
        public static PermissionsResponseCollection Ver_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_Ver", responseCollection.PermissionModel.Ver.ToResponse()); }
        public static PermissionsResponseCollection Ver_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_Ver", value); }
        public static PermissionsResponseCollection DeptName(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_DeptName", responseCollection.PermissionModel.DeptName.ToResponse()); }
        public static PermissionsResponseCollection DeptName(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_DeptName", value); }
        public static PermissionsResponseCollection DeptName_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_DeptName", responseCollection.PermissionModel.DeptName.ToResponse()); }
        public static PermissionsResponseCollection DeptName_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_DeptName", value); }
        public static PermissionsResponseCollection FullName1(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_FullName1", responseCollection.PermissionModel.FullName1.ToResponse()); }
        public static PermissionsResponseCollection FullName1(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_FullName1", value); }
        public static PermissionsResponseCollection FullName1_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_FullName1", responseCollection.PermissionModel.FullName1.ToResponse()); }
        public static PermissionsResponseCollection FullName1_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_FullName1", value); }
        public static PermissionsResponseCollection FullName2(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_FullName2", responseCollection.PermissionModel.FullName2.ToResponse()); }
        public static PermissionsResponseCollection FullName2(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_FullName2", value); }
        public static PermissionsResponseCollection FullName2_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_FullName2", responseCollection.PermissionModel.FullName2.ToResponse()); }
        public static PermissionsResponseCollection FullName2_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_FullName2", value); }
        public static PermissionsResponseCollection FirstAndLastNameOrder(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_FirstAndLastNameOrder", responseCollection.PermissionModel.FirstAndLastNameOrder.ToResponse()); }
        public static PermissionsResponseCollection FirstAndLastNameOrder(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_FirstAndLastNameOrder", value); }
        public static PermissionsResponseCollection FirstAndLastNameOrder_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_FirstAndLastNameOrder", responseCollection.PermissionModel.FirstAndLastNameOrder.ToResponse()); }
        public static PermissionsResponseCollection FirstAndLastNameOrder_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_FirstAndLastNameOrder", value); }
        public static PermissionsResponseCollection PermissionType(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_PermissionType", responseCollection.PermissionModel.PermissionType.ToResponse()); }
        public static PermissionsResponseCollection PermissionType(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_PermissionType", value); }
        public static PermissionsResponseCollection PermissionType_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_PermissionType", responseCollection.PermissionModel.PermissionType.ToResponse()); }
        public static PermissionsResponseCollection PermissionType_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_PermissionType", value); }
        public static PermissionsResponseCollection CreatedTime(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_CreatedTime", responseCollection.PermissionModel.CreatedTime.ToResponse()); }
        public static PermissionsResponseCollection CreatedTime(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_CreatedTime", value); }
        public static PermissionsResponseCollection CreatedTime_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_CreatedTime", responseCollection.PermissionModel.CreatedTime.ToResponse()); }
        public static PermissionsResponseCollection CreatedTime_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_CreatedTime", value); }
        public static PermissionsResponseCollection UpdatedTime(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_UpdatedTime", responseCollection.PermissionModel.UpdatedTime.ToResponse()); }
        public static PermissionsResponseCollection UpdatedTime(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_UpdatedTime", value); }
        public static PermissionsResponseCollection UpdatedTime_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_UpdatedTime", responseCollection.PermissionModel.UpdatedTime.ToResponse()); }
        public static PermissionsResponseCollection UpdatedTime_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_UpdatedTime", value); }
        public static PermissionsResponseCollection Timestamp(this PermissionsResponseCollection responseCollection) { return responseCollection.Val("#Permissions_Timestamp", responseCollection.PermissionModel.Timestamp.ToResponse()); }
        public static PermissionsResponseCollection Timestamp(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.Val("#Permissions_Timestamp", value); }
        public static PermissionsResponseCollection Timestamp_FormData(this PermissionsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Permissions_Timestamp", responseCollection.PermissionModel.Timestamp.ToResponse()); }
        public static PermissionsResponseCollection Timestamp_FormData(this PermissionsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Permissions_Timestamp", value); }
        public static OutgoingMailsResponseCollection ReferenceType(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_ReferenceType", responseCollection.OutgoingMailModel.ReferenceType.ToResponse()); }
        public static OutgoingMailsResponseCollection ReferenceType(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_ReferenceType", value); }
        public static OutgoingMailsResponseCollection ReferenceType_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_ReferenceType", responseCollection.OutgoingMailModel.ReferenceType.ToResponse()); }
        public static OutgoingMailsResponseCollection ReferenceType_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_ReferenceType", value); }
        public static OutgoingMailsResponseCollection ReferenceId(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_ReferenceId", responseCollection.OutgoingMailModel.ReferenceId.ToResponse()); }
        public static OutgoingMailsResponseCollection ReferenceId(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_ReferenceId", value); }
        public static OutgoingMailsResponseCollection ReferenceId_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_ReferenceId", responseCollection.OutgoingMailModel.ReferenceId.ToResponse()); }
        public static OutgoingMailsResponseCollection ReferenceId_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_ReferenceId", value); }
        public static OutgoingMailsResponseCollection OutgoingMailId(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_OutgoingMailId", responseCollection.OutgoingMailModel.OutgoingMailId.ToResponse()); }
        public static OutgoingMailsResponseCollection OutgoingMailId(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_OutgoingMailId", value); }
        public static OutgoingMailsResponseCollection OutgoingMailId_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_OutgoingMailId", responseCollection.OutgoingMailModel.OutgoingMailId.ToResponse()); }
        public static OutgoingMailsResponseCollection OutgoingMailId_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_OutgoingMailId", value); }
        public static OutgoingMailsResponseCollection Ver(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_Ver", responseCollection.OutgoingMailModel.Ver.ToResponse()); }
        public static OutgoingMailsResponseCollection Ver(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_Ver", value); }
        public static OutgoingMailsResponseCollection Ver_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_Ver", responseCollection.OutgoingMailModel.Ver.ToResponse()); }
        public static OutgoingMailsResponseCollection Ver_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_Ver", value); }
        public static OutgoingMailsResponseCollection To(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_To", responseCollection.OutgoingMailModel.To.ToResponse()); }
        public static OutgoingMailsResponseCollection To(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_To", value); }
        public static OutgoingMailsResponseCollection To_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_To", responseCollection.OutgoingMailModel.To.ToResponse()); }
        public static OutgoingMailsResponseCollection To_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_To", value); }
        public static OutgoingMailsResponseCollection Cc(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_Cc", responseCollection.OutgoingMailModel.Cc.ToResponse()); }
        public static OutgoingMailsResponseCollection Cc(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_Cc", value); }
        public static OutgoingMailsResponseCollection Cc_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_Cc", responseCollection.OutgoingMailModel.Cc.ToResponse()); }
        public static OutgoingMailsResponseCollection Cc_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_Cc", value); }
        public static OutgoingMailsResponseCollection Bcc(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_Bcc", responseCollection.OutgoingMailModel.Bcc.ToResponse()); }
        public static OutgoingMailsResponseCollection Bcc(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_Bcc", value); }
        public static OutgoingMailsResponseCollection Bcc_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_Bcc", responseCollection.OutgoingMailModel.Bcc.ToResponse()); }
        public static OutgoingMailsResponseCollection Bcc_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_Bcc", value); }
        public static OutgoingMailsResponseCollection Title(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_Title", responseCollection.OutgoingMailModel.Title.ToResponse()); }
        public static OutgoingMailsResponseCollection Title(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_Title", value); }
        public static OutgoingMailsResponseCollection Title_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_Title", responseCollection.OutgoingMailModel.Title.ToResponse()); }
        public static OutgoingMailsResponseCollection Title_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_Title", value); }
        public static OutgoingMailsResponseCollection Body(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_Body", responseCollection.OutgoingMailModel.Body.ToResponse()); }
        public static OutgoingMailsResponseCollection Body(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_Body", value); }
        public static OutgoingMailsResponseCollection Body_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_Body", responseCollection.OutgoingMailModel.Body.ToResponse()); }
        public static OutgoingMailsResponseCollection Body_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_Body", value); }
        public static OutgoingMailsResponseCollection SentTime(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_SentTime", responseCollection.OutgoingMailModel.SentTime.ToResponse()); }
        public static OutgoingMailsResponseCollection SentTime(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_SentTime", value); }
        public static OutgoingMailsResponseCollection SentTime_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_SentTime", responseCollection.OutgoingMailModel.SentTime.ToResponse()); }
        public static OutgoingMailsResponseCollection SentTime_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_SentTime", value); }
        public static OutgoingMailsResponseCollection DestinationSearchRange(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_DestinationSearchRange", responseCollection.OutgoingMailModel.DestinationSearchRange.ToResponse()); }
        public static OutgoingMailsResponseCollection DestinationSearchRange(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_DestinationSearchRange", value); }
        public static OutgoingMailsResponseCollection DestinationSearchRange_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_DestinationSearchRange", responseCollection.OutgoingMailModel.DestinationSearchRange.ToResponse()); }
        public static OutgoingMailsResponseCollection DestinationSearchRange_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_DestinationSearchRange", value); }
        public static OutgoingMailsResponseCollection DestinationSearchText(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_DestinationSearchText", responseCollection.OutgoingMailModel.DestinationSearchText.ToResponse()); }
        public static OutgoingMailsResponseCollection DestinationSearchText(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_DestinationSearchText", value); }
        public static OutgoingMailsResponseCollection DestinationSearchText_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_DestinationSearchText", responseCollection.OutgoingMailModel.DestinationSearchText.ToResponse()); }
        public static OutgoingMailsResponseCollection DestinationSearchText_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_DestinationSearchText", value); }
        public static OutgoingMailsResponseCollection CreatedTime(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_CreatedTime", responseCollection.OutgoingMailModel.CreatedTime.ToResponse()); }
        public static OutgoingMailsResponseCollection CreatedTime(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_CreatedTime", value); }
        public static OutgoingMailsResponseCollection CreatedTime_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_CreatedTime", responseCollection.OutgoingMailModel.CreatedTime.ToResponse()); }
        public static OutgoingMailsResponseCollection CreatedTime_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_CreatedTime", value); }
        public static OutgoingMailsResponseCollection UpdatedTime(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_UpdatedTime", responseCollection.OutgoingMailModel.UpdatedTime.ToResponse()); }
        public static OutgoingMailsResponseCollection UpdatedTime(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_UpdatedTime", value); }
        public static OutgoingMailsResponseCollection UpdatedTime_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_UpdatedTime", responseCollection.OutgoingMailModel.UpdatedTime.ToResponse()); }
        public static OutgoingMailsResponseCollection UpdatedTime_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_UpdatedTime", value); }
        public static OutgoingMailsResponseCollection Timestamp(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.Val("#OutgoingMails_Timestamp", responseCollection.OutgoingMailModel.Timestamp.ToResponse()); }
        public static OutgoingMailsResponseCollection Timestamp(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.Val("#OutgoingMails_Timestamp", value); }
        public static OutgoingMailsResponseCollection Timestamp_FormData(this OutgoingMailsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#OutgoingMails_Timestamp", responseCollection.OutgoingMailModel.Timestamp.ToResponse()); }
        public static OutgoingMailsResponseCollection Timestamp_FormData(this OutgoingMailsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#OutgoingMails_Timestamp", value); }
        public static SearchIndexesResponseCollection Ver(this SearchIndexesResponseCollection responseCollection) { return responseCollection.Val("#SearchIndexes_Ver", responseCollection.SearchIndexModel.Ver.ToResponse()); }
        public static SearchIndexesResponseCollection Ver(this SearchIndexesResponseCollection responseCollection, string value) { return responseCollection.Val("#SearchIndexes_Ver", value); }
        public static SearchIndexesResponseCollection Ver_FormData(this SearchIndexesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#SearchIndexes_Ver", responseCollection.SearchIndexModel.Ver.ToResponse()); }
        public static SearchIndexesResponseCollection Ver_FormData(this SearchIndexesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#SearchIndexes_Ver", value); }
        public static SearchIndexesResponseCollection CreatedTime(this SearchIndexesResponseCollection responseCollection) { return responseCollection.Val("#SearchIndexes_CreatedTime", responseCollection.SearchIndexModel.CreatedTime.ToResponse()); }
        public static SearchIndexesResponseCollection CreatedTime(this SearchIndexesResponseCollection responseCollection, string value) { return responseCollection.Val("#SearchIndexes_CreatedTime", value); }
        public static SearchIndexesResponseCollection CreatedTime_FormData(this SearchIndexesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#SearchIndexes_CreatedTime", responseCollection.SearchIndexModel.CreatedTime.ToResponse()); }
        public static SearchIndexesResponseCollection CreatedTime_FormData(this SearchIndexesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#SearchIndexes_CreatedTime", value); }
        public static SearchIndexesResponseCollection UpdatedTime(this SearchIndexesResponseCollection responseCollection) { return responseCollection.Val("#SearchIndexes_UpdatedTime", responseCollection.SearchIndexModel.UpdatedTime.ToResponse()); }
        public static SearchIndexesResponseCollection UpdatedTime(this SearchIndexesResponseCollection responseCollection, string value) { return responseCollection.Val("#SearchIndexes_UpdatedTime", value); }
        public static SearchIndexesResponseCollection UpdatedTime_FormData(this SearchIndexesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#SearchIndexes_UpdatedTime", responseCollection.SearchIndexModel.UpdatedTime.ToResponse()); }
        public static SearchIndexesResponseCollection UpdatedTime_FormData(this SearchIndexesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#SearchIndexes_UpdatedTime", value); }
        public static SearchIndexesResponseCollection Timestamp(this SearchIndexesResponseCollection responseCollection) { return responseCollection.Val("#SearchIndexes_Timestamp", responseCollection.SearchIndexModel.Timestamp.ToResponse()); }
        public static SearchIndexesResponseCollection Timestamp(this SearchIndexesResponseCollection responseCollection, string value) { return responseCollection.Val("#SearchIndexes_Timestamp", value); }
        public static SearchIndexesResponseCollection Timestamp_FormData(this SearchIndexesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#SearchIndexes_Timestamp", responseCollection.SearchIndexModel.Timestamp.ToResponse()); }
        public static SearchIndexesResponseCollection Timestamp_FormData(this SearchIndexesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#SearchIndexes_Timestamp", value); }
        public static SitesResponseCollection SiteId(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_SiteId", responseCollection.SiteModel.SiteId.ToResponse()); }
        public static SitesResponseCollection SiteId(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_SiteId", value); }
        public static SitesResponseCollection SiteId_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_SiteId", responseCollection.SiteModel.SiteId.ToResponse()); }
        public static SitesResponseCollection SiteId_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_SiteId", value); }
        public static SitesResponseCollection UpdatedTime(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_UpdatedTime", responseCollection.SiteModel.UpdatedTime.ToResponse()); }
        public static SitesResponseCollection UpdatedTime(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_UpdatedTime", value); }
        public static SitesResponseCollection UpdatedTime_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_UpdatedTime", responseCollection.SiteModel.UpdatedTime.ToResponse()); }
        public static SitesResponseCollection UpdatedTime_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_UpdatedTime", value); }
        public static SitesResponseCollection Ver(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_Ver", responseCollection.SiteModel.Ver.ToResponse()); }
        public static SitesResponseCollection Ver(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_Ver", value); }
        public static SitesResponseCollection Ver_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_Ver", responseCollection.SiteModel.Ver.ToResponse()); }
        public static SitesResponseCollection Ver_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_Ver", value); }
        public static SitesResponseCollection Title(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_Title", responseCollection.SiteModel.Title.ToResponse()); }
        public static SitesResponseCollection Title(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_Title", value); }
        public static SitesResponseCollection Title_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_Title", responseCollection.SiteModel.Title.ToResponse()); }
        public static SitesResponseCollection Title_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_Title", value); }
        public static SitesResponseCollection Body(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_Body", responseCollection.SiteModel.Body.ToResponse()); }
        public static SitesResponseCollection Body(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_Body", value); }
        public static SitesResponseCollection Body_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_Body", responseCollection.SiteModel.Body.ToResponse()); }
        public static SitesResponseCollection Body_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_Body", value); }
        public static SitesResponseCollection ReferenceType(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_ReferenceType", responseCollection.SiteModel.ReferenceType.ToResponse()); }
        public static SitesResponseCollection ReferenceType(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_ReferenceType", value); }
        public static SitesResponseCollection ReferenceType_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_ReferenceType", responseCollection.SiteModel.ReferenceType.ToResponse()); }
        public static SitesResponseCollection ReferenceType_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_ReferenceType", value); }
        public static SitesResponseCollection InheritPermission(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_InheritPermission", responseCollection.SiteModel.InheritPermission.ToResponse()); }
        public static SitesResponseCollection InheritPermission(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_InheritPermission", value); }
        public static SitesResponseCollection InheritPermission_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_InheritPermission", responseCollection.SiteModel.InheritPermission.ToResponse()); }
        public static SitesResponseCollection InheritPermission_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_InheritPermission", value); }
        public static SitesResponseCollection CreatedTime(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_CreatedTime", responseCollection.SiteModel.CreatedTime.ToResponse()); }
        public static SitesResponseCollection CreatedTime(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_CreatedTime", value); }
        public static SitesResponseCollection CreatedTime_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_CreatedTime", responseCollection.SiteModel.CreatedTime.ToResponse()); }
        public static SitesResponseCollection CreatedTime_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_CreatedTime", value); }
        public static SitesResponseCollection Timestamp(this SitesResponseCollection responseCollection) { return responseCollection.Val("#Sites_Timestamp", responseCollection.SiteModel.Timestamp.ToResponse()); }
        public static SitesResponseCollection Timestamp(this SitesResponseCollection responseCollection, string value) { return responseCollection.Val("#Sites_Timestamp", value); }
        public static SitesResponseCollection Timestamp_FormData(this SitesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Sites_Timestamp", responseCollection.SiteModel.Timestamp.ToResponse()); }
        public static SitesResponseCollection Timestamp_FormData(this SitesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Sites_Timestamp", value); }
        public static OrdersResponseCollection Ver(this OrdersResponseCollection responseCollection) { return responseCollection.Val("#Orders_Ver", responseCollection.OrderModel.Ver.ToResponse()); }
        public static OrdersResponseCollection Ver(this OrdersResponseCollection responseCollection, string value) { return responseCollection.Val("#Orders_Ver", value); }
        public static OrdersResponseCollection Ver_FormData(this OrdersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Orders_Ver", responseCollection.OrderModel.Ver.ToResponse()); }
        public static OrdersResponseCollection Ver_FormData(this OrdersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Orders_Ver", value); }
        public static OrdersResponseCollection CreatedTime(this OrdersResponseCollection responseCollection) { return responseCollection.Val("#Orders_CreatedTime", responseCollection.OrderModel.CreatedTime.ToResponse()); }
        public static OrdersResponseCollection CreatedTime(this OrdersResponseCollection responseCollection, string value) { return responseCollection.Val("#Orders_CreatedTime", value); }
        public static OrdersResponseCollection CreatedTime_FormData(this OrdersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Orders_CreatedTime", responseCollection.OrderModel.CreatedTime.ToResponse()); }
        public static OrdersResponseCollection CreatedTime_FormData(this OrdersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Orders_CreatedTime", value); }
        public static OrdersResponseCollection UpdatedTime(this OrdersResponseCollection responseCollection) { return responseCollection.Val("#Orders_UpdatedTime", responseCollection.OrderModel.UpdatedTime.ToResponse()); }
        public static OrdersResponseCollection UpdatedTime(this OrdersResponseCollection responseCollection, string value) { return responseCollection.Val("#Orders_UpdatedTime", value); }
        public static OrdersResponseCollection UpdatedTime_FormData(this OrdersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Orders_UpdatedTime", responseCollection.OrderModel.UpdatedTime.ToResponse()); }
        public static OrdersResponseCollection UpdatedTime_FormData(this OrdersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Orders_UpdatedTime", value); }
        public static OrdersResponseCollection Timestamp(this OrdersResponseCollection responseCollection) { return responseCollection.Val("#Orders_Timestamp", responseCollection.OrderModel.Timestamp.ToResponse()); }
        public static OrdersResponseCollection Timestamp(this OrdersResponseCollection responseCollection, string value) { return responseCollection.Val("#Orders_Timestamp", value); }
        public static OrdersResponseCollection Timestamp_FormData(this OrdersResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Orders_Timestamp", responseCollection.OrderModel.Timestamp.ToResponse()); }
        public static OrdersResponseCollection Timestamp_FormData(this OrdersResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Orders_Timestamp", value); }
        public static ExportSettingsResponseCollection ReferenceType(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_ReferenceType", responseCollection.ExportSettingModel.ReferenceType.ToResponse()); }
        public static ExportSettingsResponseCollection ReferenceType(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_ReferenceType", value); }
        public static ExportSettingsResponseCollection ReferenceType_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_ReferenceType", responseCollection.ExportSettingModel.ReferenceType.ToResponse()); }
        public static ExportSettingsResponseCollection ReferenceType_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_ReferenceType", value); }
        public static ExportSettingsResponseCollection ReferenceId(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_ReferenceId", responseCollection.ExportSettingModel.ReferenceId.ToResponse()); }
        public static ExportSettingsResponseCollection ReferenceId(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_ReferenceId", value); }
        public static ExportSettingsResponseCollection ReferenceId_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_ReferenceId", responseCollection.ExportSettingModel.ReferenceId.ToResponse()); }
        public static ExportSettingsResponseCollection ReferenceId_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_ReferenceId", value); }
        public static ExportSettingsResponseCollection Title(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_Title", responseCollection.ExportSettingModel.Title.ToResponse()); }
        public static ExportSettingsResponseCollection Title(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_Title", value); }
        public static ExportSettingsResponseCollection Title_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_Title", responseCollection.ExportSettingModel.Title.ToResponse()); }
        public static ExportSettingsResponseCollection Title_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_Title", value); }
        public static ExportSettingsResponseCollection ExportSettingId(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_ExportSettingId", responseCollection.ExportSettingModel.ExportSettingId.ToResponse()); }
        public static ExportSettingsResponseCollection ExportSettingId(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_ExportSettingId", value); }
        public static ExportSettingsResponseCollection ExportSettingId_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_ExportSettingId", responseCollection.ExportSettingModel.ExportSettingId.ToResponse()); }
        public static ExportSettingsResponseCollection ExportSettingId_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_ExportSettingId", value); }
        public static ExportSettingsResponseCollection Ver(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_Ver", responseCollection.ExportSettingModel.Ver.ToResponse()); }
        public static ExportSettingsResponseCollection Ver(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_Ver", value); }
        public static ExportSettingsResponseCollection Ver_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_Ver", responseCollection.ExportSettingModel.Ver.ToResponse()); }
        public static ExportSettingsResponseCollection Ver_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_Ver", value); }
        public static ExportSettingsResponseCollection AddHeader(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_AddHeader", responseCollection.ExportSettingModel.AddHeader.ToResponse()); }
        public static ExportSettingsResponseCollection AddHeader(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_AddHeader", value); }
        public static ExportSettingsResponseCollection AddHeader_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_AddHeader", responseCollection.ExportSettingModel.AddHeader.ToResponse()); }
        public static ExportSettingsResponseCollection AddHeader_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_AddHeader", value); }
        public static ExportSettingsResponseCollection CreatedTime(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_CreatedTime", responseCollection.ExportSettingModel.CreatedTime.ToResponse()); }
        public static ExportSettingsResponseCollection CreatedTime(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_CreatedTime", value); }
        public static ExportSettingsResponseCollection CreatedTime_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_CreatedTime", responseCollection.ExportSettingModel.CreatedTime.ToResponse()); }
        public static ExportSettingsResponseCollection CreatedTime_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_CreatedTime", value); }
        public static ExportSettingsResponseCollection UpdatedTime(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_UpdatedTime", responseCollection.ExportSettingModel.UpdatedTime.ToResponse()); }
        public static ExportSettingsResponseCollection UpdatedTime(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_UpdatedTime", value); }
        public static ExportSettingsResponseCollection UpdatedTime_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_UpdatedTime", responseCollection.ExportSettingModel.UpdatedTime.ToResponse()); }
        public static ExportSettingsResponseCollection UpdatedTime_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_UpdatedTime", value); }
        public static ExportSettingsResponseCollection Timestamp(this ExportSettingsResponseCollection responseCollection) { return responseCollection.Val("#ExportSettings_Timestamp", responseCollection.ExportSettingModel.Timestamp.ToResponse()); }
        public static ExportSettingsResponseCollection Timestamp(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.Val("#ExportSettings_Timestamp", value); }
        public static ExportSettingsResponseCollection Timestamp_FormData(this ExportSettingsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#ExportSettings_Timestamp", responseCollection.ExportSettingModel.Timestamp.ToResponse()); }
        public static ExportSettingsResponseCollection Timestamp_FormData(this ExportSettingsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#ExportSettings_Timestamp", value); }
        public static LinksResponseCollection Ver(this LinksResponseCollection responseCollection) { return responseCollection.Val("#Links_Ver", responseCollection.LinkModel.Ver.ToResponse()); }
        public static LinksResponseCollection Ver(this LinksResponseCollection responseCollection, string value) { return responseCollection.Val("#Links_Ver", value); }
        public static LinksResponseCollection Ver_FormData(this LinksResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Links_Ver", responseCollection.LinkModel.Ver.ToResponse()); }
        public static LinksResponseCollection Ver_FormData(this LinksResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Links_Ver", value); }
        public static LinksResponseCollection CreatedTime(this LinksResponseCollection responseCollection) { return responseCollection.Val("#Links_CreatedTime", responseCollection.LinkModel.CreatedTime.ToResponse()); }
        public static LinksResponseCollection CreatedTime(this LinksResponseCollection responseCollection, string value) { return responseCollection.Val("#Links_CreatedTime", value); }
        public static LinksResponseCollection CreatedTime_FormData(this LinksResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Links_CreatedTime", responseCollection.LinkModel.CreatedTime.ToResponse()); }
        public static LinksResponseCollection CreatedTime_FormData(this LinksResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Links_CreatedTime", value); }
        public static LinksResponseCollection UpdatedTime(this LinksResponseCollection responseCollection) { return responseCollection.Val("#Links_UpdatedTime", responseCollection.LinkModel.UpdatedTime.ToResponse()); }
        public static LinksResponseCollection UpdatedTime(this LinksResponseCollection responseCollection, string value) { return responseCollection.Val("#Links_UpdatedTime", value); }
        public static LinksResponseCollection UpdatedTime_FormData(this LinksResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Links_UpdatedTime", responseCollection.LinkModel.UpdatedTime.ToResponse()); }
        public static LinksResponseCollection UpdatedTime_FormData(this LinksResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Links_UpdatedTime", value); }
        public static LinksResponseCollection Timestamp(this LinksResponseCollection responseCollection) { return responseCollection.Val("#Links_Timestamp", responseCollection.LinkModel.Timestamp.ToResponse()); }
        public static LinksResponseCollection Timestamp(this LinksResponseCollection responseCollection, string value) { return responseCollection.Val("#Links_Timestamp", value); }
        public static LinksResponseCollection Timestamp_FormData(this LinksResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Links_Timestamp", responseCollection.LinkModel.Timestamp.ToResponse()); }
        public static LinksResponseCollection Timestamp_FormData(this LinksResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Links_Timestamp", value); }
        public static BinariesResponseCollection ReferenceId(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_ReferenceId", responseCollection.BinaryModel.ReferenceId.ToResponse()); }
        public static BinariesResponseCollection ReferenceId(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_ReferenceId", value); }
        public static BinariesResponseCollection ReferenceId_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_ReferenceId", responseCollection.BinaryModel.ReferenceId.ToResponse()); }
        public static BinariesResponseCollection ReferenceId_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_ReferenceId", value); }
        public static BinariesResponseCollection BinaryId(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_BinaryId", responseCollection.BinaryModel.BinaryId.ToResponse()); }
        public static BinariesResponseCollection BinaryId(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_BinaryId", value); }
        public static BinariesResponseCollection BinaryId_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_BinaryId", responseCollection.BinaryModel.BinaryId.ToResponse()); }
        public static BinariesResponseCollection BinaryId_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_BinaryId", value); }
        public static BinariesResponseCollection Ver(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_Ver", responseCollection.BinaryModel.Ver.ToResponse()); }
        public static BinariesResponseCollection Ver(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_Ver", value); }
        public static BinariesResponseCollection Ver_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_Ver", responseCollection.BinaryModel.Ver.ToResponse()); }
        public static BinariesResponseCollection Ver_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_Ver", value); }
        public static BinariesResponseCollection BinaryType(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_BinaryType", responseCollection.BinaryModel.BinaryType.ToResponse()); }
        public static BinariesResponseCollection BinaryType(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_BinaryType", value); }
        public static BinariesResponseCollection BinaryType_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_BinaryType", responseCollection.BinaryModel.BinaryType.ToResponse()); }
        public static BinariesResponseCollection BinaryType_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_BinaryType", value); }
        public static BinariesResponseCollection Title(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_Title", responseCollection.BinaryModel.Title.ToResponse()); }
        public static BinariesResponseCollection Title(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_Title", value); }
        public static BinariesResponseCollection Title_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_Title", responseCollection.BinaryModel.Title.ToResponse()); }
        public static BinariesResponseCollection Title_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_Title", value); }
        public static BinariesResponseCollection Body(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_Body", responseCollection.BinaryModel.Body.ToResponse()); }
        public static BinariesResponseCollection Body(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_Body", value); }
        public static BinariesResponseCollection Body_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_Body", responseCollection.BinaryModel.Body.ToResponse()); }
        public static BinariesResponseCollection Body_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_Body", value); }
        public static BinariesResponseCollection FileName(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_FileName", responseCollection.BinaryModel.FileName.ToResponse()); }
        public static BinariesResponseCollection FileName(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_FileName", value); }
        public static BinariesResponseCollection FileName_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_FileName", responseCollection.BinaryModel.FileName.ToResponse()); }
        public static BinariesResponseCollection FileName_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_FileName", value); }
        public static BinariesResponseCollection Extension(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_Extension", responseCollection.BinaryModel.Extension.ToResponse()); }
        public static BinariesResponseCollection Extension(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_Extension", value); }
        public static BinariesResponseCollection Extension_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_Extension", responseCollection.BinaryModel.Extension.ToResponse()); }
        public static BinariesResponseCollection Extension_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_Extension", value); }
        public static BinariesResponseCollection Size(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_Size", responseCollection.BinaryModel.Size.ToResponse()); }
        public static BinariesResponseCollection Size(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_Size", value); }
        public static BinariesResponseCollection Size_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_Size", responseCollection.BinaryModel.Size.ToResponse()); }
        public static BinariesResponseCollection Size_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_Size", value); }
        public static BinariesResponseCollection CreatedTime(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_CreatedTime", responseCollection.BinaryModel.CreatedTime.ToResponse()); }
        public static BinariesResponseCollection CreatedTime(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_CreatedTime", value); }
        public static BinariesResponseCollection CreatedTime_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_CreatedTime", responseCollection.BinaryModel.CreatedTime.ToResponse()); }
        public static BinariesResponseCollection CreatedTime_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_CreatedTime", value); }
        public static BinariesResponseCollection UpdatedTime(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_UpdatedTime", responseCollection.BinaryModel.UpdatedTime.ToResponse()); }
        public static BinariesResponseCollection UpdatedTime(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_UpdatedTime", value); }
        public static BinariesResponseCollection UpdatedTime_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_UpdatedTime", responseCollection.BinaryModel.UpdatedTime.ToResponse()); }
        public static BinariesResponseCollection UpdatedTime_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_UpdatedTime", value); }
        public static BinariesResponseCollection Timestamp(this BinariesResponseCollection responseCollection) { return responseCollection.Val("#Binaries_Timestamp", responseCollection.BinaryModel.Timestamp.ToResponse()); }
        public static BinariesResponseCollection Timestamp(this BinariesResponseCollection responseCollection, string value) { return responseCollection.Val("#Binaries_Timestamp", value); }
        public static BinariesResponseCollection Timestamp_FormData(this BinariesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Binaries_Timestamp", responseCollection.BinaryModel.Timestamp.ToResponse()); }
        public static BinariesResponseCollection Timestamp_FormData(this BinariesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Binaries_Timestamp", value); }
        public static IssuesResponseCollection UpdatedTime(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_UpdatedTime", responseCollection.IssueModel.UpdatedTime.ToResponse()); }
        public static IssuesResponseCollection UpdatedTime(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_UpdatedTime", value); }
        public static IssuesResponseCollection UpdatedTime_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_UpdatedTime", responseCollection.IssueModel.UpdatedTime.ToResponse()); }
        public static IssuesResponseCollection UpdatedTime_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_UpdatedTime", value); }
        public static IssuesResponseCollection IssueId(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_IssueId", responseCollection.IssueModel.IssueId.ToResponse()); }
        public static IssuesResponseCollection IssueId(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_IssueId", value); }
        public static IssuesResponseCollection IssueId_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_IssueId", responseCollection.IssueModel.IssueId.ToResponse()); }
        public static IssuesResponseCollection IssueId_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_IssueId", value); }
        public static IssuesResponseCollection Ver(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_Ver", responseCollection.IssueModel.Ver.ToResponse()); }
        public static IssuesResponseCollection Ver(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_Ver", value); }
        public static IssuesResponseCollection Ver_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_Ver", responseCollection.IssueModel.Ver.ToResponse()); }
        public static IssuesResponseCollection Ver_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_Ver", value); }
        public static IssuesResponseCollection Title(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_Title", responseCollection.IssueModel.Title.ToResponse()); }
        public static IssuesResponseCollection Title(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_Title", value); }
        public static IssuesResponseCollection Title_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_Title", responseCollection.IssueModel.Title.ToResponse()); }
        public static IssuesResponseCollection Title_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_Title", value); }
        public static IssuesResponseCollection Body(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_Body", responseCollection.IssueModel.Body.ToResponse()); }
        public static IssuesResponseCollection Body(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_Body", value); }
        public static IssuesResponseCollection Body_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_Body", responseCollection.IssueModel.Body.ToResponse()); }
        public static IssuesResponseCollection Body_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_Body", value); }
        public static IssuesResponseCollection StartTime(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_StartTime", responseCollection.IssueModel.StartTime.ToResponse()); }
        public static IssuesResponseCollection StartTime(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_StartTime", value); }
        public static IssuesResponseCollection StartTime_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_StartTime", responseCollection.IssueModel.StartTime.ToResponse()); }
        public static IssuesResponseCollection StartTime_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_StartTime", value); }
        public static IssuesResponseCollection CompletionTime(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CompletionTime", responseCollection.IssueModel.CompletionTime.ToResponse()); }
        public static IssuesResponseCollection CompletionTime(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CompletionTime", value); }
        public static IssuesResponseCollection CompletionTime_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CompletionTime", responseCollection.IssueModel.CompletionTime.ToResponse()); }
        public static IssuesResponseCollection CompletionTime_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CompletionTime", value); }
        public static IssuesResponseCollection WorkValue(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_WorkValue", responseCollection.IssueModel.WorkValue.ToResponse()); }
        public static IssuesResponseCollection WorkValue(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_WorkValue", value); }
        public static IssuesResponseCollection WorkValue_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_WorkValue", responseCollection.IssueModel.WorkValue.ToResponse()); }
        public static IssuesResponseCollection WorkValue_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_WorkValue", value); }
        public static IssuesResponseCollection ProgressRate(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ProgressRate", responseCollection.IssueModel.ProgressRate.ToResponse()); }
        public static IssuesResponseCollection ProgressRate(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ProgressRate", value); }
        public static IssuesResponseCollection ProgressRate_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ProgressRate", responseCollection.IssueModel.ProgressRate.ToResponse()); }
        public static IssuesResponseCollection ProgressRate_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ProgressRate", value); }
        public static IssuesResponseCollection RemainingWorkValue(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_RemainingWorkValue", responseCollection.IssueModel.RemainingWorkValue.ToResponse()); }
        public static IssuesResponseCollection RemainingWorkValue(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_RemainingWorkValue", value); }
        public static IssuesResponseCollection RemainingWorkValue_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_RemainingWorkValue", responseCollection.IssueModel.RemainingWorkValue.ToResponse()); }
        public static IssuesResponseCollection RemainingWorkValue_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_RemainingWorkValue", value); }
        public static IssuesResponseCollection Status(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_Status", responseCollection.IssueModel.Status.ToResponse()); }
        public static IssuesResponseCollection Status(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_Status", value); }
        public static IssuesResponseCollection Status_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_Status", responseCollection.IssueModel.Status.ToResponse()); }
        public static IssuesResponseCollection Status_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_Status", value); }
        public static IssuesResponseCollection Manager(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_Manager", responseCollection.IssueModel.Manager.ToResponse()); }
        public static IssuesResponseCollection Manager(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_Manager", value); }
        public static IssuesResponseCollection Manager_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_Manager", responseCollection.IssueModel.Manager.ToResponse()); }
        public static IssuesResponseCollection Manager_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_Manager", value); }
        public static IssuesResponseCollection Owner(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_Owner", responseCollection.IssueModel.Owner.ToResponse()); }
        public static IssuesResponseCollection Owner(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_Owner", value); }
        public static IssuesResponseCollection Owner_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_Owner", responseCollection.IssueModel.Owner.ToResponse()); }
        public static IssuesResponseCollection Owner_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_Owner", value); }
        public static IssuesResponseCollection ClassA(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassA", responseCollection.IssueModel.ClassA.ToResponse()); }
        public static IssuesResponseCollection ClassA(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassA", value); }
        public static IssuesResponseCollection ClassA_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassA", responseCollection.IssueModel.ClassA.ToResponse()); }
        public static IssuesResponseCollection ClassA_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassA", value); }
        public static IssuesResponseCollection ClassB(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassB", responseCollection.IssueModel.ClassB.ToResponse()); }
        public static IssuesResponseCollection ClassB(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassB", value); }
        public static IssuesResponseCollection ClassB_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassB", responseCollection.IssueModel.ClassB.ToResponse()); }
        public static IssuesResponseCollection ClassB_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassB", value); }
        public static IssuesResponseCollection ClassC(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassC", responseCollection.IssueModel.ClassC.ToResponse()); }
        public static IssuesResponseCollection ClassC(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassC", value); }
        public static IssuesResponseCollection ClassC_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassC", responseCollection.IssueModel.ClassC.ToResponse()); }
        public static IssuesResponseCollection ClassC_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassC", value); }
        public static IssuesResponseCollection ClassD(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassD", responseCollection.IssueModel.ClassD.ToResponse()); }
        public static IssuesResponseCollection ClassD(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassD", value); }
        public static IssuesResponseCollection ClassD_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassD", responseCollection.IssueModel.ClassD.ToResponse()); }
        public static IssuesResponseCollection ClassD_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassD", value); }
        public static IssuesResponseCollection ClassE(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassE", responseCollection.IssueModel.ClassE.ToResponse()); }
        public static IssuesResponseCollection ClassE(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassE", value); }
        public static IssuesResponseCollection ClassE_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassE", responseCollection.IssueModel.ClassE.ToResponse()); }
        public static IssuesResponseCollection ClassE_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassE", value); }
        public static IssuesResponseCollection ClassF(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassF", responseCollection.IssueModel.ClassF.ToResponse()); }
        public static IssuesResponseCollection ClassF(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassF", value); }
        public static IssuesResponseCollection ClassF_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassF", responseCollection.IssueModel.ClassF.ToResponse()); }
        public static IssuesResponseCollection ClassF_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassF", value); }
        public static IssuesResponseCollection ClassG(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassG", responseCollection.IssueModel.ClassG.ToResponse()); }
        public static IssuesResponseCollection ClassG(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassG", value); }
        public static IssuesResponseCollection ClassG_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassG", responseCollection.IssueModel.ClassG.ToResponse()); }
        public static IssuesResponseCollection ClassG_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassG", value); }
        public static IssuesResponseCollection ClassH(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassH", responseCollection.IssueModel.ClassH.ToResponse()); }
        public static IssuesResponseCollection ClassH(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassH", value); }
        public static IssuesResponseCollection ClassH_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassH", responseCollection.IssueModel.ClassH.ToResponse()); }
        public static IssuesResponseCollection ClassH_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassH", value); }
        public static IssuesResponseCollection ClassI(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassI", responseCollection.IssueModel.ClassI.ToResponse()); }
        public static IssuesResponseCollection ClassI(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassI", value); }
        public static IssuesResponseCollection ClassI_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassI", responseCollection.IssueModel.ClassI.ToResponse()); }
        public static IssuesResponseCollection ClassI_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassI", value); }
        public static IssuesResponseCollection ClassJ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassJ", responseCollection.IssueModel.ClassJ.ToResponse()); }
        public static IssuesResponseCollection ClassJ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassJ", value); }
        public static IssuesResponseCollection ClassJ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassJ", responseCollection.IssueModel.ClassJ.ToResponse()); }
        public static IssuesResponseCollection ClassJ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassJ", value); }
        public static IssuesResponseCollection ClassK(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassK", responseCollection.IssueModel.ClassK.ToResponse()); }
        public static IssuesResponseCollection ClassK(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassK", value); }
        public static IssuesResponseCollection ClassK_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassK", responseCollection.IssueModel.ClassK.ToResponse()); }
        public static IssuesResponseCollection ClassK_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassK", value); }
        public static IssuesResponseCollection ClassL(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassL", responseCollection.IssueModel.ClassL.ToResponse()); }
        public static IssuesResponseCollection ClassL(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassL", value); }
        public static IssuesResponseCollection ClassL_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassL", responseCollection.IssueModel.ClassL.ToResponse()); }
        public static IssuesResponseCollection ClassL_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassL", value); }
        public static IssuesResponseCollection ClassM(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassM", responseCollection.IssueModel.ClassM.ToResponse()); }
        public static IssuesResponseCollection ClassM(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassM", value); }
        public static IssuesResponseCollection ClassM_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassM", responseCollection.IssueModel.ClassM.ToResponse()); }
        public static IssuesResponseCollection ClassM_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassM", value); }
        public static IssuesResponseCollection ClassN(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassN", responseCollection.IssueModel.ClassN.ToResponse()); }
        public static IssuesResponseCollection ClassN(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassN", value); }
        public static IssuesResponseCollection ClassN_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassN", responseCollection.IssueModel.ClassN.ToResponse()); }
        public static IssuesResponseCollection ClassN_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassN", value); }
        public static IssuesResponseCollection ClassO(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassO", responseCollection.IssueModel.ClassO.ToResponse()); }
        public static IssuesResponseCollection ClassO(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassO", value); }
        public static IssuesResponseCollection ClassO_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassO", responseCollection.IssueModel.ClassO.ToResponse()); }
        public static IssuesResponseCollection ClassO_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassO", value); }
        public static IssuesResponseCollection ClassP(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassP", responseCollection.IssueModel.ClassP.ToResponse()); }
        public static IssuesResponseCollection ClassP(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassP", value); }
        public static IssuesResponseCollection ClassP_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassP", responseCollection.IssueModel.ClassP.ToResponse()); }
        public static IssuesResponseCollection ClassP_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassP", value); }
        public static IssuesResponseCollection ClassQ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassQ", responseCollection.IssueModel.ClassQ.ToResponse()); }
        public static IssuesResponseCollection ClassQ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassQ", value); }
        public static IssuesResponseCollection ClassQ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassQ", responseCollection.IssueModel.ClassQ.ToResponse()); }
        public static IssuesResponseCollection ClassQ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassQ", value); }
        public static IssuesResponseCollection ClassR(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassR", responseCollection.IssueModel.ClassR.ToResponse()); }
        public static IssuesResponseCollection ClassR(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassR", value); }
        public static IssuesResponseCollection ClassR_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassR", responseCollection.IssueModel.ClassR.ToResponse()); }
        public static IssuesResponseCollection ClassR_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassR", value); }
        public static IssuesResponseCollection ClassS(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassS", responseCollection.IssueModel.ClassS.ToResponse()); }
        public static IssuesResponseCollection ClassS(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassS", value); }
        public static IssuesResponseCollection ClassS_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassS", responseCollection.IssueModel.ClassS.ToResponse()); }
        public static IssuesResponseCollection ClassS_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassS", value); }
        public static IssuesResponseCollection ClassT(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassT", responseCollection.IssueModel.ClassT.ToResponse()); }
        public static IssuesResponseCollection ClassT(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassT", value); }
        public static IssuesResponseCollection ClassT_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassT", responseCollection.IssueModel.ClassT.ToResponse()); }
        public static IssuesResponseCollection ClassT_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassT", value); }
        public static IssuesResponseCollection ClassU(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassU", responseCollection.IssueModel.ClassU.ToResponse()); }
        public static IssuesResponseCollection ClassU(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassU", value); }
        public static IssuesResponseCollection ClassU_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassU", responseCollection.IssueModel.ClassU.ToResponse()); }
        public static IssuesResponseCollection ClassU_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassU", value); }
        public static IssuesResponseCollection ClassV(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassV", responseCollection.IssueModel.ClassV.ToResponse()); }
        public static IssuesResponseCollection ClassV(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassV", value); }
        public static IssuesResponseCollection ClassV_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassV", responseCollection.IssueModel.ClassV.ToResponse()); }
        public static IssuesResponseCollection ClassV_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassV", value); }
        public static IssuesResponseCollection ClassW(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassW", responseCollection.IssueModel.ClassW.ToResponse()); }
        public static IssuesResponseCollection ClassW(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassW", value); }
        public static IssuesResponseCollection ClassW_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassW", responseCollection.IssueModel.ClassW.ToResponse()); }
        public static IssuesResponseCollection ClassW_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassW", value); }
        public static IssuesResponseCollection ClassX(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassX", responseCollection.IssueModel.ClassX.ToResponse()); }
        public static IssuesResponseCollection ClassX(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassX", value); }
        public static IssuesResponseCollection ClassX_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassX", responseCollection.IssueModel.ClassX.ToResponse()); }
        public static IssuesResponseCollection ClassX_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassX", value); }
        public static IssuesResponseCollection ClassY(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassY", responseCollection.IssueModel.ClassY.ToResponse()); }
        public static IssuesResponseCollection ClassY(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassY", value); }
        public static IssuesResponseCollection ClassY_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassY", responseCollection.IssueModel.ClassY.ToResponse()); }
        public static IssuesResponseCollection ClassY_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassY", value); }
        public static IssuesResponseCollection ClassZ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_ClassZ", responseCollection.IssueModel.ClassZ.ToResponse()); }
        public static IssuesResponseCollection ClassZ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_ClassZ", value); }
        public static IssuesResponseCollection ClassZ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_ClassZ", responseCollection.IssueModel.ClassZ.ToResponse()); }
        public static IssuesResponseCollection ClassZ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_ClassZ", value); }
        public static IssuesResponseCollection NumA(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumA", responseCollection.IssueModel.NumA.ToResponse()); }
        public static IssuesResponseCollection NumA(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumA", value); }
        public static IssuesResponseCollection NumA_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumA", responseCollection.IssueModel.NumA.ToResponse()); }
        public static IssuesResponseCollection NumA_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumA", value); }
        public static IssuesResponseCollection NumB(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumB", responseCollection.IssueModel.NumB.ToResponse()); }
        public static IssuesResponseCollection NumB(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumB", value); }
        public static IssuesResponseCollection NumB_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumB", responseCollection.IssueModel.NumB.ToResponse()); }
        public static IssuesResponseCollection NumB_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumB", value); }
        public static IssuesResponseCollection NumC(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumC", responseCollection.IssueModel.NumC.ToResponse()); }
        public static IssuesResponseCollection NumC(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumC", value); }
        public static IssuesResponseCollection NumC_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumC", responseCollection.IssueModel.NumC.ToResponse()); }
        public static IssuesResponseCollection NumC_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumC", value); }
        public static IssuesResponseCollection NumD(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumD", responseCollection.IssueModel.NumD.ToResponse()); }
        public static IssuesResponseCollection NumD(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumD", value); }
        public static IssuesResponseCollection NumD_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumD", responseCollection.IssueModel.NumD.ToResponse()); }
        public static IssuesResponseCollection NumD_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumD", value); }
        public static IssuesResponseCollection NumE(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumE", responseCollection.IssueModel.NumE.ToResponse()); }
        public static IssuesResponseCollection NumE(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumE", value); }
        public static IssuesResponseCollection NumE_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumE", responseCollection.IssueModel.NumE.ToResponse()); }
        public static IssuesResponseCollection NumE_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumE", value); }
        public static IssuesResponseCollection NumF(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumF", responseCollection.IssueModel.NumF.ToResponse()); }
        public static IssuesResponseCollection NumF(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumF", value); }
        public static IssuesResponseCollection NumF_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumF", responseCollection.IssueModel.NumF.ToResponse()); }
        public static IssuesResponseCollection NumF_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumF", value); }
        public static IssuesResponseCollection NumG(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumG", responseCollection.IssueModel.NumG.ToResponse()); }
        public static IssuesResponseCollection NumG(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumG", value); }
        public static IssuesResponseCollection NumG_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumG", responseCollection.IssueModel.NumG.ToResponse()); }
        public static IssuesResponseCollection NumG_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumG", value); }
        public static IssuesResponseCollection NumH(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumH", responseCollection.IssueModel.NumH.ToResponse()); }
        public static IssuesResponseCollection NumH(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumH", value); }
        public static IssuesResponseCollection NumH_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumH", responseCollection.IssueModel.NumH.ToResponse()); }
        public static IssuesResponseCollection NumH_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumH", value); }
        public static IssuesResponseCollection NumI(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumI", responseCollection.IssueModel.NumI.ToResponse()); }
        public static IssuesResponseCollection NumI(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumI", value); }
        public static IssuesResponseCollection NumI_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumI", responseCollection.IssueModel.NumI.ToResponse()); }
        public static IssuesResponseCollection NumI_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumI", value); }
        public static IssuesResponseCollection NumJ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumJ", responseCollection.IssueModel.NumJ.ToResponse()); }
        public static IssuesResponseCollection NumJ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumJ", value); }
        public static IssuesResponseCollection NumJ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumJ", responseCollection.IssueModel.NumJ.ToResponse()); }
        public static IssuesResponseCollection NumJ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumJ", value); }
        public static IssuesResponseCollection NumK(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumK", responseCollection.IssueModel.NumK.ToResponse()); }
        public static IssuesResponseCollection NumK(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumK", value); }
        public static IssuesResponseCollection NumK_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumK", responseCollection.IssueModel.NumK.ToResponse()); }
        public static IssuesResponseCollection NumK_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumK", value); }
        public static IssuesResponseCollection NumL(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumL", responseCollection.IssueModel.NumL.ToResponse()); }
        public static IssuesResponseCollection NumL(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumL", value); }
        public static IssuesResponseCollection NumL_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumL", responseCollection.IssueModel.NumL.ToResponse()); }
        public static IssuesResponseCollection NumL_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumL", value); }
        public static IssuesResponseCollection NumM(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumM", responseCollection.IssueModel.NumM.ToResponse()); }
        public static IssuesResponseCollection NumM(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumM", value); }
        public static IssuesResponseCollection NumM_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumM", responseCollection.IssueModel.NumM.ToResponse()); }
        public static IssuesResponseCollection NumM_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumM", value); }
        public static IssuesResponseCollection NumN(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumN", responseCollection.IssueModel.NumN.ToResponse()); }
        public static IssuesResponseCollection NumN(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumN", value); }
        public static IssuesResponseCollection NumN_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumN", responseCollection.IssueModel.NumN.ToResponse()); }
        public static IssuesResponseCollection NumN_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumN", value); }
        public static IssuesResponseCollection NumO(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumO", responseCollection.IssueModel.NumO.ToResponse()); }
        public static IssuesResponseCollection NumO(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumO", value); }
        public static IssuesResponseCollection NumO_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumO", responseCollection.IssueModel.NumO.ToResponse()); }
        public static IssuesResponseCollection NumO_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumO", value); }
        public static IssuesResponseCollection NumP(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumP", responseCollection.IssueModel.NumP.ToResponse()); }
        public static IssuesResponseCollection NumP(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumP", value); }
        public static IssuesResponseCollection NumP_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumP", responseCollection.IssueModel.NumP.ToResponse()); }
        public static IssuesResponseCollection NumP_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumP", value); }
        public static IssuesResponseCollection NumQ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumQ", responseCollection.IssueModel.NumQ.ToResponse()); }
        public static IssuesResponseCollection NumQ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumQ", value); }
        public static IssuesResponseCollection NumQ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumQ", responseCollection.IssueModel.NumQ.ToResponse()); }
        public static IssuesResponseCollection NumQ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumQ", value); }
        public static IssuesResponseCollection NumR(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumR", responseCollection.IssueModel.NumR.ToResponse()); }
        public static IssuesResponseCollection NumR(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumR", value); }
        public static IssuesResponseCollection NumR_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumR", responseCollection.IssueModel.NumR.ToResponse()); }
        public static IssuesResponseCollection NumR_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumR", value); }
        public static IssuesResponseCollection NumS(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumS", responseCollection.IssueModel.NumS.ToResponse()); }
        public static IssuesResponseCollection NumS(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumS", value); }
        public static IssuesResponseCollection NumS_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumS", responseCollection.IssueModel.NumS.ToResponse()); }
        public static IssuesResponseCollection NumS_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumS", value); }
        public static IssuesResponseCollection NumT(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumT", responseCollection.IssueModel.NumT.ToResponse()); }
        public static IssuesResponseCollection NumT(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumT", value); }
        public static IssuesResponseCollection NumT_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumT", responseCollection.IssueModel.NumT.ToResponse()); }
        public static IssuesResponseCollection NumT_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumT", value); }
        public static IssuesResponseCollection NumU(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumU", responseCollection.IssueModel.NumU.ToResponse()); }
        public static IssuesResponseCollection NumU(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumU", value); }
        public static IssuesResponseCollection NumU_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumU", responseCollection.IssueModel.NumU.ToResponse()); }
        public static IssuesResponseCollection NumU_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumU", value); }
        public static IssuesResponseCollection NumV(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumV", responseCollection.IssueModel.NumV.ToResponse()); }
        public static IssuesResponseCollection NumV(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumV", value); }
        public static IssuesResponseCollection NumV_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumV", responseCollection.IssueModel.NumV.ToResponse()); }
        public static IssuesResponseCollection NumV_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumV", value); }
        public static IssuesResponseCollection NumW(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumW", responseCollection.IssueModel.NumW.ToResponse()); }
        public static IssuesResponseCollection NumW(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumW", value); }
        public static IssuesResponseCollection NumW_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumW", responseCollection.IssueModel.NumW.ToResponse()); }
        public static IssuesResponseCollection NumW_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumW", value); }
        public static IssuesResponseCollection NumX(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumX", responseCollection.IssueModel.NumX.ToResponse()); }
        public static IssuesResponseCollection NumX(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumX", value); }
        public static IssuesResponseCollection NumX_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumX", responseCollection.IssueModel.NumX.ToResponse()); }
        public static IssuesResponseCollection NumX_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumX", value); }
        public static IssuesResponseCollection NumY(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumY", responseCollection.IssueModel.NumY.ToResponse()); }
        public static IssuesResponseCollection NumY(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumY", value); }
        public static IssuesResponseCollection NumY_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumY", responseCollection.IssueModel.NumY.ToResponse()); }
        public static IssuesResponseCollection NumY_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumY", value); }
        public static IssuesResponseCollection NumZ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_NumZ", responseCollection.IssueModel.NumZ.ToResponse()); }
        public static IssuesResponseCollection NumZ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_NumZ", value); }
        public static IssuesResponseCollection NumZ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_NumZ", responseCollection.IssueModel.NumZ.ToResponse()); }
        public static IssuesResponseCollection NumZ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_NumZ", value); }
        public static IssuesResponseCollection DateA(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateA", responseCollection.IssueModel.DateA.ToResponse()); }
        public static IssuesResponseCollection DateA(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateA", value); }
        public static IssuesResponseCollection DateA_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateA", responseCollection.IssueModel.DateA.ToResponse()); }
        public static IssuesResponseCollection DateA_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateA", value); }
        public static IssuesResponseCollection DateB(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateB", responseCollection.IssueModel.DateB.ToResponse()); }
        public static IssuesResponseCollection DateB(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateB", value); }
        public static IssuesResponseCollection DateB_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateB", responseCollection.IssueModel.DateB.ToResponse()); }
        public static IssuesResponseCollection DateB_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateB", value); }
        public static IssuesResponseCollection DateC(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateC", responseCollection.IssueModel.DateC.ToResponse()); }
        public static IssuesResponseCollection DateC(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateC", value); }
        public static IssuesResponseCollection DateC_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateC", responseCollection.IssueModel.DateC.ToResponse()); }
        public static IssuesResponseCollection DateC_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateC", value); }
        public static IssuesResponseCollection DateD(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateD", responseCollection.IssueModel.DateD.ToResponse()); }
        public static IssuesResponseCollection DateD(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateD", value); }
        public static IssuesResponseCollection DateD_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateD", responseCollection.IssueModel.DateD.ToResponse()); }
        public static IssuesResponseCollection DateD_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateD", value); }
        public static IssuesResponseCollection DateE(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateE", responseCollection.IssueModel.DateE.ToResponse()); }
        public static IssuesResponseCollection DateE(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateE", value); }
        public static IssuesResponseCollection DateE_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateE", responseCollection.IssueModel.DateE.ToResponse()); }
        public static IssuesResponseCollection DateE_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateE", value); }
        public static IssuesResponseCollection DateF(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateF", responseCollection.IssueModel.DateF.ToResponse()); }
        public static IssuesResponseCollection DateF(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateF", value); }
        public static IssuesResponseCollection DateF_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateF", responseCollection.IssueModel.DateF.ToResponse()); }
        public static IssuesResponseCollection DateF_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateF", value); }
        public static IssuesResponseCollection DateG(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateG", responseCollection.IssueModel.DateG.ToResponse()); }
        public static IssuesResponseCollection DateG(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateG", value); }
        public static IssuesResponseCollection DateG_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateG", responseCollection.IssueModel.DateG.ToResponse()); }
        public static IssuesResponseCollection DateG_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateG", value); }
        public static IssuesResponseCollection DateH(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateH", responseCollection.IssueModel.DateH.ToResponse()); }
        public static IssuesResponseCollection DateH(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateH", value); }
        public static IssuesResponseCollection DateH_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateH", responseCollection.IssueModel.DateH.ToResponse()); }
        public static IssuesResponseCollection DateH_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateH", value); }
        public static IssuesResponseCollection DateI(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateI", responseCollection.IssueModel.DateI.ToResponse()); }
        public static IssuesResponseCollection DateI(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateI", value); }
        public static IssuesResponseCollection DateI_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateI", responseCollection.IssueModel.DateI.ToResponse()); }
        public static IssuesResponseCollection DateI_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateI", value); }
        public static IssuesResponseCollection DateJ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateJ", responseCollection.IssueModel.DateJ.ToResponse()); }
        public static IssuesResponseCollection DateJ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateJ", value); }
        public static IssuesResponseCollection DateJ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateJ", responseCollection.IssueModel.DateJ.ToResponse()); }
        public static IssuesResponseCollection DateJ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateJ", value); }
        public static IssuesResponseCollection DateK(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateK", responseCollection.IssueModel.DateK.ToResponse()); }
        public static IssuesResponseCollection DateK(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateK", value); }
        public static IssuesResponseCollection DateK_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateK", responseCollection.IssueModel.DateK.ToResponse()); }
        public static IssuesResponseCollection DateK_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateK", value); }
        public static IssuesResponseCollection DateL(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateL", responseCollection.IssueModel.DateL.ToResponse()); }
        public static IssuesResponseCollection DateL(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateL", value); }
        public static IssuesResponseCollection DateL_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateL", responseCollection.IssueModel.DateL.ToResponse()); }
        public static IssuesResponseCollection DateL_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateL", value); }
        public static IssuesResponseCollection DateM(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateM", responseCollection.IssueModel.DateM.ToResponse()); }
        public static IssuesResponseCollection DateM(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateM", value); }
        public static IssuesResponseCollection DateM_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateM", responseCollection.IssueModel.DateM.ToResponse()); }
        public static IssuesResponseCollection DateM_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateM", value); }
        public static IssuesResponseCollection DateN(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateN", responseCollection.IssueModel.DateN.ToResponse()); }
        public static IssuesResponseCollection DateN(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateN", value); }
        public static IssuesResponseCollection DateN_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateN", responseCollection.IssueModel.DateN.ToResponse()); }
        public static IssuesResponseCollection DateN_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateN", value); }
        public static IssuesResponseCollection DateO(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateO", responseCollection.IssueModel.DateO.ToResponse()); }
        public static IssuesResponseCollection DateO(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateO", value); }
        public static IssuesResponseCollection DateO_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateO", responseCollection.IssueModel.DateO.ToResponse()); }
        public static IssuesResponseCollection DateO_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateO", value); }
        public static IssuesResponseCollection DateP(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateP", responseCollection.IssueModel.DateP.ToResponse()); }
        public static IssuesResponseCollection DateP(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateP", value); }
        public static IssuesResponseCollection DateP_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateP", responseCollection.IssueModel.DateP.ToResponse()); }
        public static IssuesResponseCollection DateP_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateP", value); }
        public static IssuesResponseCollection DateQ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateQ", responseCollection.IssueModel.DateQ.ToResponse()); }
        public static IssuesResponseCollection DateQ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateQ", value); }
        public static IssuesResponseCollection DateQ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateQ", responseCollection.IssueModel.DateQ.ToResponse()); }
        public static IssuesResponseCollection DateQ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateQ", value); }
        public static IssuesResponseCollection DateR(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateR", responseCollection.IssueModel.DateR.ToResponse()); }
        public static IssuesResponseCollection DateR(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateR", value); }
        public static IssuesResponseCollection DateR_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateR", responseCollection.IssueModel.DateR.ToResponse()); }
        public static IssuesResponseCollection DateR_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateR", value); }
        public static IssuesResponseCollection DateS(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateS", responseCollection.IssueModel.DateS.ToResponse()); }
        public static IssuesResponseCollection DateS(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateS", value); }
        public static IssuesResponseCollection DateS_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateS", responseCollection.IssueModel.DateS.ToResponse()); }
        public static IssuesResponseCollection DateS_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateS", value); }
        public static IssuesResponseCollection DateT(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateT", responseCollection.IssueModel.DateT.ToResponse()); }
        public static IssuesResponseCollection DateT(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateT", value); }
        public static IssuesResponseCollection DateT_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateT", responseCollection.IssueModel.DateT.ToResponse()); }
        public static IssuesResponseCollection DateT_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateT", value); }
        public static IssuesResponseCollection DateU(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateU", responseCollection.IssueModel.DateU.ToResponse()); }
        public static IssuesResponseCollection DateU(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateU", value); }
        public static IssuesResponseCollection DateU_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateU", responseCollection.IssueModel.DateU.ToResponse()); }
        public static IssuesResponseCollection DateU_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateU", value); }
        public static IssuesResponseCollection DateV(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateV", responseCollection.IssueModel.DateV.ToResponse()); }
        public static IssuesResponseCollection DateV(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateV", value); }
        public static IssuesResponseCollection DateV_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateV", responseCollection.IssueModel.DateV.ToResponse()); }
        public static IssuesResponseCollection DateV_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateV", value); }
        public static IssuesResponseCollection DateW(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateW", responseCollection.IssueModel.DateW.ToResponse()); }
        public static IssuesResponseCollection DateW(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateW", value); }
        public static IssuesResponseCollection DateW_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateW", responseCollection.IssueModel.DateW.ToResponse()); }
        public static IssuesResponseCollection DateW_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateW", value); }
        public static IssuesResponseCollection DateX(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateX", responseCollection.IssueModel.DateX.ToResponse()); }
        public static IssuesResponseCollection DateX(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateX", value); }
        public static IssuesResponseCollection DateX_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateX", responseCollection.IssueModel.DateX.ToResponse()); }
        public static IssuesResponseCollection DateX_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateX", value); }
        public static IssuesResponseCollection DateY(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateY", responseCollection.IssueModel.DateY.ToResponse()); }
        public static IssuesResponseCollection DateY(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateY", value); }
        public static IssuesResponseCollection DateY_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateY", responseCollection.IssueModel.DateY.ToResponse()); }
        public static IssuesResponseCollection DateY_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateY", value); }
        public static IssuesResponseCollection DateZ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DateZ", responseCollection.IssueModel.DateZ.ToResponse()); }
        public static IssuesResponseCollection DateZ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DateZ", value); }
        public static IssuesResponseCollection DateZ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DateZ", responseCollection.IssueModel.DateZ.ToResponse()); }
        public static IssuesResponseCollection DateZ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DateZ", value); }
        public static IssuesResponseCollection DescriptionA(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionA", responseCollection.IssueModel.DescriptionA.ToResponse()); }
        public static IssuesResponseCollection DescriptionA(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionA", value); }
        public static IssuesResponseCollection DescriptionA_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionA", responseCollection.IssueModel.DescriptionA.ToResponse()); }
        public static IssuesResponseCollection DescriptionA_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionA", value); }
        public static IssuesResponseCollection DescriptionB(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionB", responseCollection.IssueModel.DescriptionB.ToResponse()); }
        public static IssuesResponseCollection DescriptionB(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionB", value); }
        public static IssuesResponseCollection DescriptionB_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionB", responseCollection.IssueModel.DescriptionB.ToResponse()); }
        public static IssuesResponseCollection DescriptionB_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionB", value); }
        public static IssuesResponseCollection DescriptionC(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionC", responseCollection.IssueModel.DescriptionC.ToResponse()); }
        public static IssuesResponseCollection DescriptionC(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionC", value); }
        public static IssuesResponseCollection DescriptionC_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionC", responseCollection.IssueModel.DescriptionC.ToResponse()); }
        public static IssuesResponseCollection DescriptionC_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionC", value); }
        public static IssuesResponseCollection DescriptionD(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionD", responseCollection.IssueModel.DescriptionD.ToResponse()); }
        public static IssuesResponseCollection DescriptionD(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionD", value); }
        public static IssuesResponseCollection DescriptionD_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionD", responseCollection.IssueModel.DescriptionD.ToResponse()); }
        public static IssuesResponseCollection DescriptionD_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionD", value); }
        public static IssuesResponseCollection DescriptionE(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionE", responseCollection.IssueModel.DescriptionE.ToResponse()); }
        public static IssuesResponseCollection DescriptionE(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionE", value); }
        public static IssuesResponseCollection DescriptionE_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionE", responseCollection.IssueModel.DescriptionE.ToResponse()); }
        public static IssuesResponseCollection DescriptionE_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionE", value); }
        public static IssuesResponseCollection DescriptionF(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionF", responseCollection.IssueModel.DescriptionF.ToResponse()); }
        public static IssuesResponseCollection DescriptionF(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionF", value); }
        public static IssuesResponseCollection DescriptionF_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionF", responseCollection.IssueModel.DescriptionF.ToResponse()); }
        public static IssuesResponseCollection DescriptionF_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionF", value); }
        public static IssuesResponseCollection DescriptionG(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionG", responseCollection.IssueModel.DescriptionG.ToResponse()); }
        public static IssuesResponseCollection DescriptionG(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionG", value); }
        public static IssuesResponseCollection DescriptionG_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionG", responseCollection.IssueModel.DescriptionG.ToResponse()); }
        public static IssuesResponseCollection DescriptionG_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionG", value); }
        public static IssuesResponseCollection DescriptionH(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionH", responseCollection.IssueModel.DescriptionH.ToResponse()); }
        public static IssuesResponseCollection DescriptionH(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionH", value); }
        public static IssuesResponseCollection DescriptionH_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionH", responseCollection.IssueModel.DescriptionH.ToResponse()); }
        public static IssuesResponseCollection DescriptionH_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionH", value); }
        public static IssuesResponseCollection DescriptionI(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionI", responseCollection.IssueModel.DescriptionI.ToResponse()); }
        public static IssuesResponseCollection DescriptionI(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionI", value); }
        public static IssuesResponseCollection DescriptionI_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionI", responseCollection.IssueModel.DescriptionI.ToResponse()); }
        public static IssuesResponseCollection DescriptionI_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionI", value); }
        public static IssuesResponseCollection DescriptionJ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionJ", responseCollection.IssueModel.DescriptionJ.ToResponse()); }
        public static IssuesResponseCollection DescriptionJ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionJ", value); }
        public static IssuesResponseCollection DescriptionJ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionJ", responseCollection.IssueModel.DescriptionJ.ToResponse()); }
        public static IssuesResponseCollection DescriptionJ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionJ", value); }
        public static IssuesResponseCollection DescriptionK(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionK", responseCollection.IssueModel.DescriptionK.ToResponse()); }
        public static IssuesResponseCollection DescriptionK(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionK", value); }
        public static IssuesResponseCollection DescriptionK_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionK", responseCollection.IssueModel.DescriptionK.ToResponse()); }
        public static IssuesResponseCollection DescriptionK_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionK", value); }
        public static IssuesResponseCollection DescriptionL(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionL", responseCollection.IssueModel.DescriptionL.ToResponse()); }
        public static IssuesResponseCollection DescriptionL(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionL", value); }
        public static IssuesResponseCollection DescriptionL_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionL", responseCollection.IssueModel.DescriptionL.ToResponse()); }
        public static IssuesResponseCollection DescriptionL_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionL", value); }
        public static IssuesResponseCollection DescriptionM(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionM", responseCollection.IssueModel.DescriptionM.ToResponse()); }
        public static IssuesResponseCollection DescriptionM(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionM", value); }
        public static IssuesResponseCollection DescriptionM_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionM", responseCollection.IssueModel.DescriptionM.ToResponse()); }
        public static IssuesResponseCollection DescriptionM_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionM", value); }
        public static IssuesResponseCollection DescriptionN(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionN", responseCollection.IssueModel.DescriptionN.ToResponse()); }
        public static IssuesResponseCollection DescriptionN(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionN", value); }
        public static IssuesResponseCollection DescriptionN_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionN", responseCollection.IssueModel.DescriptionN.ToResponse()); }
        public static IssuesResponseCollection DescriptionN_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionN", value); }
        public static IssuesResponseCollection DescriptionO(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionO", responseCollection.IssueModel.DescriptionO.ToResponse()); }
        public static IssuesResponseCollection DescriptionO(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionO", value); }
        public static IssuesResponseCollection DescriptionO_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionO", responseCollection.IssueModel.DescriptionO.ToResponse()); }
        public static IssuesResponseCollection DescriptionO_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionO", value); }
        public static IssuesResponseCollection DescriptionP(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionP", responseCollection.IssueModel.DescriptionP.ToResponse()); }
        public static IssuesResponseCollection DescriptionP(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionP", value); }
        public static IssuesResponseCollection DescriptionP_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionP", responseCollection.IssueModel.DescriptionP.ToResponse()); }
        public static IssuesResponseCollection DescriptionP_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionP", value); }
        public static IssuesResponseCollection DescriptionQ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionQ", responseCollection.IssueModel.DescriptionQ.ToResponse()); }
        public static IssuesResponseCollection DescriptionQ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionQ", value); }
        public static IssuesResponseCollection DescriptionQ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionQ", responseCollection.IssueModel.DescriptionQ.ToResponse()); }
        public static IssuesResponseCollection DescriptionQ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionQ", value); }
        public static IssuesResponseCollection DescriptionR(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionR", responseCollection.IssueModel.DescriptionR.ToResponse()); }
        public static IssuesResponseCollection DescriptionR(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionR", value); }
        public static IssuesResponseCollection DescriptionR_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionR", responseCollection.IssueModel.DescriptionR.ToResponse()); }
        public static IssuesResponseCollection DescriptionR_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionR", value); }
        public static IssuesResponseCollection DescriptionS(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionS", responseCollection.IssueModel.DescriptionS.ToResponse()); }
        public static IssuesResponseCollection DescriptionS(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionS", value); }
        public static IssuesResponseCollection DescriptionS_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionS", responseCollection.IssueModel.DescriptionS.ToResponse()); }
        public static IssuesResponseCollection DescriptionS_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionS", value); }
        public static IssuesResponseCollection DescriptionT(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionT", responseCollection.IssueModel.DescriptionT.ToResponse()); }
        public static IssuesResponseCollection DescriptionT(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionT", value); }
        public static IssuesResponseCollection DescriptionT_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionT", responseCollection.IssueModel.DescriptionT.ToResponse()); }
        public static IssuesResponseCollection DescriptionT_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionT", value); }
        public static IssuesResponseCollection DescriptionU(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionU", responseCollection.IssueModel.DescriptionU.ToResponse()); }
        public static IssuesResponseCollection DescriptionU(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionU", value); }
        public static IssuesResponseCollection DescriptionU_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionU", responseCollection.IssueModel.DescriptionU.ToResponse()); }
        public static IssuesResponseCollection DescriptionU_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionU", value); }
        public static IssuesResponseCollection DescriptionV(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionV", responseCollection.IssueModel.DescriptionV.ToResponse()); }
        public static IssuesResponseCollection DescriptionV(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionV", value); }
        public static IssuesResponseCollection DescriptionV_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionV", responseCollection.IssueModel.DescriptionV.ToResponse()); }
        public static IssuesResponseCollection DescriptionV_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionV", value); }
        public static IssuesResponseCollection DescriptionW(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionW", responseCollection.IssueModel.DescriptionW.ToResponse()); }
        public static IssuesResponseCollection DescriptionW(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionW", value); }
        public static IssuesResponseCollection DescriptionW_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionW", responseCollection.IssueModel.DescriptionW.ToResponse()); }
        public static IssuesResponseCollection DescriptionW_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionW", value); }
        public static IssuesResponseCollection DescriptionX(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionX", responseCollection.IssueModel.DescriptionX.ToResponse()); }
        public static IssuesResponseCollection DescriptionX(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionX", value); }
        public static IssuesResponseCollection DescriptionX_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionX", responseCollection.IssueModel.DescriptionX.ToResponse()); }
        public static IssuesResponseCollection DescriptionX_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionX", value); }
        public static IssuesResponseCollection DescriptionY(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionY", responseCollection.IssueModel.DescriptionY.ToResponse()); }
        public static IssuesResponseCollection DescriptionY(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionY", value); }
        public static IssuesResponseCollection DescriptionY_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionY", responseCollection.IssueModel.DescriptionY.ToResponse()); }
        public static IssuesResponseCollection DescriptionY_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionY", value); }
        public static IssuesResponseCollection DescriptionZ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_DescriptionZ", responseCollection.IssueModel.DescriptionZ.ToResponse()); }
        public static IssuesResponseCollection DescriptionZ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_DescriptionZ", value); }
        public static IssuesResponseCollection DescriptionZ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_DescriptionZ", responseCollection.IssueModel.DescriptionZ.ToResponse()); }
        public static IssuesResponseCollection DescriptionZ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_DescriptionZ", value); }
        public static IssuesResponseCollection CheckA(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckA", responseCollection.IssueModel.CheckA.ToResponse()); }
        public static IssuesResponseCollection CheckA(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckA", value); }
        public static IssuesResponseCollection CheckA_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckA", responseCollection.IssueModel.CheckA.ToResponse()); }
        public static IssuesResponseCollection CheckA_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckA", value); }
        public static IssuesResponseCollection CheckB(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckB", responseCollection.IssueModel.CheckB.ToResponse()); }
        public static IssuesResponseCollection CheckB(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckB", value); }
        public static IssuesResponseCollection CheckB_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckB", responseCollection.IssueModel.CheckB.ToResponse()); }
        public static IssuesResponseCollection CheckB_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckB", value); }
        public static IssuesResponseCollection CheckC(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckC", responseCollection.IssueModel.CheckC.ToResponse()); }
        public static IssuesResponseCollection CheckC(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckC", value); }
        public static IssuesResponseCollection CheckC_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckC", responseCollection.IssueModel.CheckC.ToResponse()); }
        public static IssuesResponseCollection CheckC_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckC", value); }
        public static IssuesResponseCollection CheckD(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckD", responseCollection.IssueModel.CheckD.ToResponse()); }
        public static IssuesResponseCollection CheckD(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckD", value); }
        public static IssuesResponseCollection CheckD_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckD", responseCollection.IssueModel.CheckD.ToResponse()); }
        public static IssuesResponseCollection CheckD_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckD", value); }
        public static IssuesResponseCollection CheckE(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckE", responseCollection.IssueModel.CheckE.ToResponse()); }
        public static IssuesResponseCollection CheckE(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckE", value); }
        public static IssuesResponseCollection CheckE_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckE", responseCollection.IssueModel.CheckE.ToResponse()); }
        public static IssuesResponseCollection CheckE_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckE", value); }
        public static IssuesResponseCollection CheckF(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckF", responseCollection.IssueModel.CheckF.ToResponse()); }
        public static IssuesResponseCollection CheckF(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckF", value); }
        public static IssuesResponseCollection CheckF_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckF", responseCollection.IssueModel.CheckF.ToResponse()); }
        public static IssuesResponseCollection CheckF_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckF", value); }
        public static IssuesResponseCollection CheckG(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckG", responseCollection.IssueModel.CheckG.ToResponse()); }
        public static IssuesResponseCollection CheckG(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckG", value); }
        public static IssuesResponseCollection CheckG_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckG", responseCollection.IssueModel.CheckG.ToResponse()); }
        public static IssuesResponseCollection CheckG_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckG", value); }
        public static IssuesResponseCollection CheckH(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckH", responseCollection.IssueModel.CheckH.ToResponse()); }
        public static IssuesResponseCollection CheckH(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckH", value); }
        public static IssuesResponseCollection CheckH_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckH", responseCollection.IssueModel.CheckH.ToResponse()); }
        public static IssuesResponseCollection CheckH_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckH", value); }
        public static IssuesResponseCollection CheckI(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckI", responseCollection.IssueModel.CheckI.ToResponse()); }
        public static IssuesResponseCollection CheckI(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckI", value); }
        public static IssuesResponseCollection CheckI_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckI", responseCollection.IssueModel.CheckI.ToResponse()); }
        public static IssuesResponseCollection CheckI_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckI", value); }
        public static IssuesResponseCollection CheckJ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckJ", responseCollection.IssueModel.CheckJ.ToResponse()); }
        public static IssuesResponseCollection CheckJ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckJ", value); }
        public static IssuesResponseCollection CheckJ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckJ", responseCollection.IssueModel.CheckJ.ToResponse()); }
        public static IssuesResponseCollection CheckJ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckJ", value); }
        public static IssuesResponseCollection CheckK(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckK", responseCollection.IssueModel.CheckK.ToResponse()); }
        public static IssuesResponseCollection CheckK(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckK", value); }
        public static IssuesResponseCollection CheckK_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckK", responseCollection.IssueModel.CheckK.ToResponse()); }
        public static IssuesResponseCollection CheckK_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckK", value); }
        public static IssuesResponseCollection CheckL(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckL", responseCollection.IssueModel.CheckL.ToResponse()); }
        public static IssuesResponseCollection CheckL(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckL", value); }
        public static IssuesResponseCollection CheckL_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckL", responseCollection.IssueModel.CheckL.ToResponse()); }
        public static IssuesResponseCollection CheckL_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckL", value); }
        public static IssuesResponseCollection CheckM(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckM", responseCollection.IssueModel.CheckM.ToResponse()); }
        public static IssuesResponseCollection CheckM(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckM", value); }
        public static IssuesResponseCollection CheckM_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckM", responseCollection.IssueModel.CheckM.ToResponse()); }
        public static IssuesResponseCollection CheckM_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckM", value); }
        public static IssuesResponseCollection CheckN(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckN", responseCollection.IssueModel.CheckN.ToResponse()); }
        public static IssuesResponseCollection CheckN(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckN", value); }
        public static IssuesResponseCollection CheckN_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckN", responseCollection.IssueModel.CheckN.ToResponse()); }
        public static IssuesResponseCollection CheckN_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckN", value); }
        public static IssuesResponseCollection CheckO(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckO", responseCollection.IssueModel.CheckO.ToResponse()); }
        public static IssuesResponseCollection CheckO(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckO", value); }
        public static IssuesResponseCollection CheckO_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckO", responseCollection.IssueModel.CheckO.ToResponse()); }
        public static IssuesResponseCollection CheckO_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckO", value); }
        public static IssuesResponseCollection CheckP(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckP", responseCollection.IssueModel.CheckP.ToResponse()); }
        public static IssuesResponseCollection CheckP(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckP", value); }
        public static IssuesResponseCollection CheckP_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckP", responseCollection.IssueModel.CheckP.ToResponse()); }
        public static IssuesResponseCollection CheckP_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckP", value); }
        public static IssuesResponseCollection CheckQ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckQ", responseCollection.IssueModel.CheckQ.ToResponse()); }
        public static IssuesResponseCollection CheckQ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckQ", value); }
        public static IssuesResponseCollection CheckQ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckQ", responseCollection.IssueModel.CheckQ.ToResponse()); }
        public static IssuesResponseCollection CheckQ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckQ", value); }
        public static IssuesResponseCollection CheckR(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckR", responseCollection.IssueModel.CheckR.ToResponse()); }
        public static IssuesResponseCollection CheckR(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckR", value); }
        public static IssuesResponseCollection CheckR_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckR", responseCollection.IssueModel.CheckR.ToResponse()); }
        public static IssuesResponseCollection CheckR_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckR", value); }
        public static IssuesResponseCollection CheckS(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckS", responseCollection.IssueModel.CheckS.ToResponse()); }
        public static IssuesResponseCollection CheckS(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckS", value); }
        public static IssuesResponseCollection CheckS_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckS", responseCollection.IssueModel.CheckS.ToResponse()); }
        public static IssuesResponseCollection CheckS_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckS", value); }
        public static IssuesResponseCollection CheckT(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckT", responseCollection.IssueModel.CheckT.ToResponse()); }
        public static IssuesResponseCollection CheckT(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckT", value); }
        public static IssuesResponseCollection CheckT_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckT", responseCollection.IssueModel.CheckT.ToResponse()); }
        public static IssuesResponseCollection CheckT_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckT", value); }
        public static IssuesResponseCollection CheckU(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckU", responseCollection.IssueModel.CheckU.ToResponse()); }
        public static IssuesResponseCollection CheckU(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckU", value); }
        public static IssuesResponseCollection CheckU_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckU", responseCollection.IssueModel.CheckU.ToResponse()); }
        public static IssuesResponseCollection CheckU_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckU", value); }
        public static IssuesResponseCollection CheckV(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckV", responseCollection.IssueModel.CheckV.ToResponse()); }
        public static IssuesResponseCollection CheckV(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckV", value); }
        public static IssuesResponseCollection CheckV_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckV", responseCollection.IssueModel.CheckV.ToResponse()); }
        public static IssuesResponseCollection CheckV_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckV", value); }
        public static IssuesResponseCollection CheckW(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckW", responseCollection.IssueModel.CheckW.ToResponse()); }
        public static IssuesResponseCollection CheckW(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckW", value); }
        public static IssuesResponseCollection CheckW_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckW", responseCollection.IssueModel.CheckW.ToResponse()); }
        public static IssuesResponseCollection CheckW_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckW", value); }
        public static IssuesResponseCollection CheckX(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckX", responseCollection.IssueModel.CheckX.ToResponse()); }
        public static IssuesResponseCollection CheckX(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckX", value); }
        public static IssuesResponseCollection CheckX_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckX", responseCollection.IssueModel.CheckX.ToResponse()); }
        public static IssuesResponseCollection CheckX_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckX", value); }
        public static IssuesResponseCollection CheckY(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckY", responseCollection.IssueModel.CheckY.ToResponse()); }
        public static IssuesResponseCollection CheckY(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckY", value); }
        public static IssuesResponseCollection CheckY_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckY", responseCollection.IssueModel.CheckY.ToResponse()); }
        public static IssuesResponseCollection CheckY_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckY", value); }
        public static IssuesResponseCollection CheckZ(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CheckZ", responseCollection.IssueModel.CheckZ.ToResponse()); }
        public static IssuesResponseCollection CheckZ(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CheckZ", value); }
        public static IssuesResponseCollection CheckZ_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CheckZ", responseCollection.IssueModel.CheckZ.ToResponse()); }
        public static IssuesResponseCollection CheckZ_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CheckZ", value); }
        public static IssuesResponseCollection CreatedTime(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_CreatedTime", responseCollection.IssueModel.CreatedTime.ToResponse()); }
        public static IssuesResponseCollection CreatedTime(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_CreatedTime", value); }
        public static IssuesResponseCollection CreatedTime_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_CreatedTime", responseCollection.IssueModel.CreatedTime.ToResponse()); }
        public static IssuesResponseCollection CreatedTime_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_CreatedTime", value); }
        public static IssuesResponseCollection Timestamp(this IssuesResponseCollection responseCollection) { return responseCollection.Val("#Issues_Timestamp", responseCollection.IssueModel.Timestamp.ToResponse()); }
        public static IssuesResponseCollection Timestamp(this IssuesResponseCollection responseCollection, string value) { return responseCollection.Val("#Issues_Timestamp", value); }
        public static IssuesResponseCollection Timestamp_FormData(this IssuesResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Issues_Timestamp", responseCollection.IssueModel.Timestamp.ToResponse()); }
        public static IssuesResponseCollection Timestamp_FormData(this IssuesResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Issues_Timestamp", value); }
        public static ResultsResponseCollection UpdatedTime(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_UpdatedTime", responseCollection.ResultModel.UpdatedTime.ToResponse()); }
        public static ResultsResponseCollection UpdatedTime(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_UpdatedTime", value); }
        public static ResultsResponseCollection UpdatedTime_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_UpdatedTime", responseCollection.ResultModel.UpdatedTime.ToResponse()); }
        public static ResultsResponseCollection UpdatedTime_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_UpdatedTime", value); }
        public static ResultsResponseCollection ResultId(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ResultId", responseCollection.ResultModel.ResultId.ToResponse()); }
        public static ResultsResponseCollection ResultId(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ResultId", value); }
        public static ResultsResponseCollection ResultId_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ResultId", responseCollection.ResultModel.ResultId.ToResponse()); }
        public static ResultsResponseCollection ResultId_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ResultId", value); }
        public static ResultsResponseCollection Ver(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_Ver", responseCollection.ResultModel.Ver.ToResponse()); }
        public static ResultsResponseCollection Ver(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_Ver", value); }
        public static ResultsResponseCollection Ver_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_Ver", responseCollection.ResultModel.Ver.ToResponse()); }
        public static ResultsResponseCollection Ver_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_Ver", value); }
        public static ResultsResponseCollection Title(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_Title", responseCollection.ResultModel.Title.ToResponse()); }
        public static ResultsResponseCollection Title(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_Title", value); }
        public static ResultsResponseCollection Title_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_Title", responseCollection.ResultModel.Title.ToResponse()); }
        public static ResultsResponseCollection Title_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_Title", value); }
        public static ResultsResponseCollection Body(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_Body", responseCollection.ResultModel.Body.ToResponse()); }
        public static ResultsResponseCollection Body(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_Body", value); }
        public static ResultsResponseCollection Body_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_Body", responseCollection.ResultModel.Body.ToResponse()); }
        public static ResultsResponseCollection Body_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_Body", value); }
        public static ResultsResponseCollection Status(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_Status", responseCollection.ResultModel.Status.ToResponse()); }
        public static ResultsResponseCollection Status(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_Status", value); }
        public static ResultsResponseCollection Status_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_Status", responseCollection.ResultModel.Status.ToResponse()); }
        public static ResultsResponseCollection Status_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_Status", value); }
        public static ResultsResponseCollection Manager(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_Manager", responseCollection.ResultModel.Manager.ToResponse()); }
        public static ResultsResponseCollection Manager(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_Manager", value); }
        public static ResultsResponseCollection Manager_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_Manager", responseCollection.ResultModel.Manager.ToResponse()); }
        public static ResultsResponseCollection Manager_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_Manager", value); }
        public static ResultsResponseCollection Owner(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_Owner", responseCollection.ResultModel.Owner.ToResponse()); }
        public static ResultsResponseCollection Owner(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_Owner", value); }
        public static ResultsResponseCollection Owner_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_Owner", responseCollection.ResultModel.Owner.ToResponse()); }
        public static ResultsResponseCollection Owner_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_Owner", value); }
        public static ResultsResponseCollection ClassA(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassA", responseCollection.ResultModel.ClassA.ToResponse()); }
        public static ResultsResponseCollection ClassA(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassA", value); }
        public static ResultsResponseCollection ClassA_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassA", responseCollection.ResultModel.ClassA.ToResponse()); }
        public static ResultsResponseCollection ClassA_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassA", value); }
        public static ResultsResponseCollection ClassB(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassB", responseCollection.ResultModel.ClassB.ToResponse()); }
        public static ResultsResponseCollection ClassB(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassB", value); }
        public static ResultsResponseCollection ClassB_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassB", responseCollection.ResultModel.ClassB.ToResponse()); }
        public static ResultsResponseCollection ClassB_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassB", value); }
        public static ResultsResponseCollection ClassC(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassC", responseCollection.ResultModel.ClassC.ToResponse()); }
        public static ResultsResponseCollection ClassC(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassC", value); }
        public static ResultsResponseCollection ClassC_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassC", responseCollection.ResultModel.ClassC.ToResponse()); }
        public static ResultsResponseCollection ClassC_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassC", value); }
        public static ResultsResponseCollection ClassD(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassD", responseCollection.ResultModel.ClassD.ToResponse()); }
        public static ResultsResponseCollection ClassD(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassD", value); }
        public static ResultsResponseCollection ClassD_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassD", responseCollection.ResultModel.ClassD.ToResponse()); }
        public static ResultsResponseCollection ClassD_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassD", value); }
        public static ResultsResponseCollection ClassE(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassE", responseCollection.ResultModel.ClassE.ToResponse()); }
        public static ResultsResponseCollection ClassE(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassE", value); }
        public static ResultsResponseCollection ClassE_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassE", responseCollection.ResultModel.ClassE.ToResponse()); }
        public static ResultsResponseCollection ClassE_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassE", value); }
        public static ResultsResponseCollection ClassF(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassF", responseCollection.ResultModel.ClassF.ToResponse()); }
        public static ResultsResponseCollection ClassF(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassF", value); }
        public static ResultsResponseCollection ClassF_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassF", responseCollection.ResultModel.ClassF.ToResponse()); }
        public static ResultsResponseCollection ClassF_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassF", value); }
        public static ResultsResponseCollection ClassG(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassG", responseCollection.ResultModel.ClassG.ToResponse()); }
        public static ResultsResponseCollection ClassG(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassG", value); }
        public static ResultsResponseCollection ClassG_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassG", responseCollection.ResultModel.ClassG.ToResponse()); }
        public static ResultsResponseCollection ClassG_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassG", value); }
        public static ResultsResponseCollection ClassH(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassH", responseCollection.ResultModel.ClassH.ToResponse()); }
        public static ResultsResponseCollection ClassH(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassH", value); }
        public static ResultsResponseCollection ClassH_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassH", responseCollection.ResultModel.ClassH.ToResponse()); }
        public static ResultsResponseCollection ClassH_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassH", value); }
        public static ResultsResponseCollection ClassI(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassI", responseCollection.ResultModel.ClassI.ToResponse()); }
        public static ResultsResponseCollection ClassI(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassI", value); }
        public static ResultsResponseCollection ClassI_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassI", responseCollection.ResultModel.ClassI.ToResponse()); }
        public static ResultsResponseCollection ClassI_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassI", value); }
        public static ResultsResponseCollection ClassJ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassJ", responseCollection.ResultModel.ClassJ.ToResponse()); }
        public static ResultsResponseCollection ClassJ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassJ", value); }
        public static ResultsResponseCollection ClassJ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassJ", responseCollection.ResultModel.ClassJ.ToResponse()); }
        public static ResultsResponseCollection ClassJ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassJ", value); }
        public static ResultsResponseCollection ClassK(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassK", responseCollection.ResultModel.ClassK.ToResponse()); }
        public static ResultsResponseCollection ClassK(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassK", value); }
        public static ResultsResponseCollection ClassK_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassK", responseCollection.ResultModel.ClassK.ToResponse()); }
        public static ResultsResponseCollection ClassK_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassK", value); }
        public static ResultsResponseCollection ClassL(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassL", responseCollection.ResultModel.ClassL.ToResponse()); }
        public static ResultsResponseCollection ClassL(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassL", value); }
        public static ResultsResponseCollection ClassL_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassL", responseCollection.ResultModel.ClassL.ToResponse()); }
        public static ResultsResponseCollection ClassL_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassL", value); }
        public static ResultsResponseCollection ClassM(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassM", responseCollection.ResultModel.ClassM.ToResponse()); }
        public static ResultsResponseCollection ClassM(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassM", value); }
        public static ResultsResponseCollection ClassM_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassM", responseCollection.ResultModel.ClassM.ToResponse()); }
        public static ResultsResponseCollection ClassM_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassM", value); }
        public static ResultsResponseCollection ClassN(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassN", responseCollection.ResultModel.ClassN.ToResponse()); }
        public static ResultsResponseCollection ClassN(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassN", value); }
        public static ResultsResponseCollection ClassN_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassN", responseCollection.ResultModel.ClassN.ToResponse()); }
        public static ResultsResponseCollection ClassN_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassN", value); }
        public static ResultsResponseCollection ClassO(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassO", responseCollection.ResultModel.ClassO.ToResponse()); }
        public static ResultsResponseCollection ClassO(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassO", value); }
        public static ResultsResponseCollection ClassO_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassO", responseCollection.ResultModel.ClassO.ToResponse()); }
        public static ResultsResponseCollection ClassO_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassO", value); }
        public static ResultsResponseCollection ClassP(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassP", responseCollection.ResultModel.ClassP.ToResponse()); }
        public static ResultsResponseCollection ClassP(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassP", value); }
        public static ResultsResponseCollection ClassP_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassP", responseCollection.ResultModel.ClassP.ToResponse()); }
        public static ResultsResponseCollection ClassP_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassP", value); }
        public static ResultsResponseCollection ClassQ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassQ", responseCollection.ResultModel.ClassQ.ToResponse()); }
        public static ResultsResponseCollection ClassQ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassQ", value); }
        public static ResultsResponseCollection ClassQ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassQ", responseCollection.ResultModel.ClassQ.ToResponse()); }
        public static ResultsResponseCollection ClassQ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassQ", value); }
        public static ResultsResponseCollection ClassR(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassR", responseCollection.ResultModel.ClassR.ToResponse()); }
        public static ResultsResponseCollection ClassR(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassR", value); }
        public static ResultsResponseCollection ClassR_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassR", responseCollection.ResultModel.ClassR.ToResponse()); }
        public static ResultsResponseCollection ClassR_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassR", value); }
        public static ResultsResponseCollection ClassS(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassS", responseCollection.ResultModel.ClassS.ToResponse()); }
        public static ResultsResponseCollection ClassS(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassS", value); }
        public static ResultsResponseCollection ClassS_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassS", responseCollection.ResultModel.ClassS.ToResponse()); }
        public static ResultsResponseCollection ClassS_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassS", value); }
        public static ResultsResponseCollection ClassT(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassT", responseCollection.ResultModel.ClassT.ToResponse()); }
        public static ResultsResponseCollection ClassT(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassT", value); }
        public static ResultsResponseCollection ClassT_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassT", responseCollection.ResultModel.ClassT.ToResponse()); }
        public static ResultsResponseCollection ClassT_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassT", value); }
        public static ResultsResponseCollection ClassU(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassU", responseCollection.ResultModel.ClassU.ToResponse()); }
        public static ResultsResponseCollection ClassU(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassU", value); }
        public static ResultsResponseCollection ClassU_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassU", responseCollection.ResultModel.ClassU.ToResponse()); }
        public static ResultsResponseCollection ClassU_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassU", value); }
        public static ResultsResponseCollection ClassV(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassV", responseCollection.ResultModel.ClassV.ToResponse()); }
        public static ResultsResponseCollection ClassV(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassV", value); }
        public static ResultsResponseCollection ClassV_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassV", responseCollection.ResultModel.ClassV.ToResponse()); }
        public static ResultsResponseCollection ClassV_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassV", value); }
        public static ResultsResponseCollection ClassW(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassW", responseCollection.ResultModel.ClassW.ToResponse()); }
        public static ResultsResponseCollection ClassW(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassW", value); }
        public static ResultsResponseCollection ClassW_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassW", responseCollection.ResultModel.ClassW.ToResponse()); }
        public static ResultsResponseCollection ClassW_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassW", value); }
        public static ResultsResponseCollection ClassX(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassX", responseCollection.ResultModel.ClassX.ToResponse()); }
        public static ResultsResponseCollection ClassX(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassX", value); }
        public static ResultsResponseCollection ClassX_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassX", responseCollection.ResultModel.ClassX.ToResponse()); }
        public static ResultsResponseCollection ClassX_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassX", value); }
        public static ResultsResponseCollection ClassY(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassY", responseCollection.ResultModel.ClassY.ToResponse()); }
        public static ResultsResponseCollection ClassY(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassY", value); }
        public static ResultsResponseCollection ClassY_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassY", responseCollection.ResultModel.ClassY.ToResponse()); }
        public static ResultsResponseCollection ClassY_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassY", value); }
        public static ResultsResponseCollection ClassZ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_ClassZ", responseCollection.ResultModel.ClassZ.ToResponse()); }
        public static ResultsResponseCollection ClassZ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_ClassZ", value); }
        public static ResultsResponseCollection ClassZ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_ClassZ", responseCollection.ResultModel.ClassZ.ToResponse()); }
        public static ResultsResponseCollection ClassZ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_ClassZ", value); }
        public static ResultsResponseCollection NumA(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumA", responseCollection.ResultModel.NumA.ToResponse()); }
        public static ResultsResponseCollection NumA(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumA", value); }
        public static ResultsResponseCollection NumA_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumA", responseCollection.ResultModel.NumA.ToResponse()); }
        public static ResultsResponseCollection NumA_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumA", value); }
        public static ResultsResponseCollection NumB(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumB", responseCollection.ResultModel.NumB.ToResponse()); }
        public static ResultsResponseCollection NumB(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumB", value); }
        public static ResultsResponseCollection NumB_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumB", responseCollection.ResultModel.NumB.ToResponse()); }
        public static ResultsResponseCollection NumB_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumB", value); }
        public static ResultsResponseCollection NumC(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumC", responseCollection.ResultModel.NumC.ToResponse()); }
        public static ResultsResponseCollection NumC(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumC", value); }
        public static ResultsResponseCollection NumC_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumC", responseCollection.ResultModel.NumC.ToResponse()); }
        public static ResultsResponseCollection NumC_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumC", value); }
        public static ResultsResponseCollection NumD(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumD", responseCollection.ResultModel.NumD.ToResponse()); }
        public static ResultsResponseCollection NumD(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumD", value); }
        public static ResultsResponseCollection NumD_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumD", responseCollection.ResultModel.NumD.ToResponse()); }
        public static ResultsResponseCollection NumD_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumD", value); }
        public static ResultsResponseCollection NumE(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumE", responseCollection.ResultModel.NumE.ToResponse()); }
        public static ResultsResponseCollection NumE(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumE", value); }
        public static ResultsResponseCollection NumE_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumE", responseCollection.ResultModel.NumE.ToResponse()); }
        public static ResultsResponseCollection NumE_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumE", value); }
        public static ResultsResponseCollection NumF(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumF", responseCollection.ResultModel.NumF.ToResponse()); }
        public static ResultsResponseCollection NumF(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumF", value); }
        public static ResultsResponseCollection NumF_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumF", responseCollection.ResultModel.NumF.ToResponse()); }
        public static ResultsResponseCollection NumF_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumF", value); }
        public static ResultsResponseCollection NumG(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumG", responseCollection.ResultModel.NumG.ToResponse()); }
        public static ResultsResponseCollection NumG(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumG", value); }
        public static ResultsResponseCollection NumG_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumG", responseCollection.ResultModel.NumG.ToResponse()); }
        public static ResultsResponseCollection NumG_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumG", value); }
        public static ResultsResponseCollection NumH(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumH", responseCollection.ResultModel.NumH.ToResponse()); }
        public static ResultsResponseCollection NumH(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumH", value); }
        public static ResultsResponseCollection NumH_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumH", responseCollection.ResultModel.NumH.ToResponse()); }
        public static ResultsResponseCollection NumH_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumH", value); }
        public static ResultsResponseCollection NumI(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumI", responseCollection.ResultModel.NumI.ToResponse()); }
        public static ResultsResponseCollection NumI(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumI", value); }
        public static ResultsResponseCollection NumI_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumI", responseCollection.ResultModel.NumI.ToResponse()); }
        public static ResultsResponseCollection NumI_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumI", value); }
        public static ResultsResponseCollection NumJ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumJ", responseCollection.ResultModel.NumJ.ToResponse()); }
        public static ResultsResponseCollection NumJ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumJ", value); }
        public static ResultsResponseCollection NumJ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumJ", responseCollection.ResultModel.NumJ.ToResponse()); }
        public static ResultsResponseCollection NumJ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumJ", value); }
        public static ResultsResponseCollection NumK(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumK", responseCollection.ResultModel.NumK.ToResponse()); }
        public static ResultsResponseCollection NumK(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumK", value); }
        public static ResultsResponseCollection NumK_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumK", responseCollection.ResultModel.NumK.ToResponse()); }
        public static ResultsResponseCollection NumK_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumK", value); }
        public static ResultsResponseCollection NumL(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumL", responseCollection.ResultModel.NumL.ToResponse()); }
        public static ResultsResponseCollection NumL(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumL", value); }
        public static ResultsResponseCollection NumL_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumL", responseCollection.ResultModel.NumL.ToResponse()); }
        public static ResultsResponseCollection NumL_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumL", value); }
        public static ResultsResponseCollection NumM(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumM", responseCollection.ResultModel.NumM.ToResponse()); }
        public static ResultsResponseCollection NumM(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumM", value); }
        public static ResultsResponseCollection NumM_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumM", responseCollection.ResultModel.NumM.ToResponse()); }
        public static ResultsResponseCollection NumM_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumM", value); }
        public static ResultsResponseCollection NumN(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumN", responseCollection.ResultModel.NumN.ToResponse()); }
        public static ResultsResponseCollection NumN(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumN", value); }
        public static ResultsResponseCollection NumN_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumN", responseCollection.ResultModel.NumN.ToResponse()); }
        public static ResultsResponseCollection NumN_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumN", value); }
        public static ResultsResponseCollection NumO(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumO", responseCollection.ResultModel.NumO.ToResponse()); }
        public static ResultsResponseCollection NumO(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumO", value); }
        public static ResultsResponseCollection NumO_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumO", responseCollection.ResultModel.NumO.ToResponse()); }
        public static ResultsResponseCollection NumO_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumO", value); }
        public static ResultsResponseCollection NumP(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumP", responseCollection.ResultModel.NumP.ToResponse()); }
        public static ResultsResponseCollection NumP(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumP", value); }
        public static ResultsResponseCollection NumP_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumP", responseCollection.ResultModel.NumP.ToResponse()); }
        public static ResultsResponseCollection NumP_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumP", value); }
        public static ResultsResponseCollection NumQ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumQ", responseCollection.ResultModel.NumQ.ToResponse()); }
        public static ResultsResponseCollection NumQ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumQ", value); }
        public static ResultsResponseCollection NumQ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumQ", responseCollection.ResultModel.NumQ.ToResponse()); }
        public static ResultsResponseCollection NumQ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumQ", value); }
        public static ResultsResponseCollection NumR(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumR", responseCollection.ResultModel.NumR.ToResponse()); }
        public static ResultsResponseCollection NumR(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumR", value); }
        public static ResultsResponseCollection NumR_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumR", responseCollection.ResultModel.NumR.ToResponse()); }
        public static ResultsResponseCollection NumR_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumR", value); }
        public static ResultsResponseCollection NumS(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumS", responseCollection.ResultModel.NumS.ToResponse()); }
        public static ResultsResponseCollection NumS(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumS", value); }
        public static ResultsResponseCollection NumS_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumS", responseCollection.ResultModel.NumS.ToResponse()); }
        public static ResultsResponseCollection NumS_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumS", value); }
        public static ResultsResponseCollection NumT(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumT", responseCollection.ResultModel.NumT.ToResponse()); }
        public static ResultsResponseCollection NumT(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumT", value); }
        public static ResultsResponseCollection NumT_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumT", responseCollection.ResultModel.NumT.ToResponse()); }
        public static ResultsResponseCollection NumT_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumT", value); }
        public static ResultsResponseCollection NumU(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumU", responseCollection.ResultModel.NumU.ToResponse()); }
        public static ResultsResponseCollection NumU(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumU", value); }
        public static ResultsResponseCollection NumU_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumU", responseCollection.ResultModel.NumU.ToResponse()); }
        public static ResultsResponseCollection NumU_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumU", value); }
        public static ResultsResponseCollection NumV(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumV", responseCollection.ResultModel.NumV.ToResponse()); }
        public static ResultsResponseCollection NumV(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumV", value); }
        public static ResultsResponseCollection NumV_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumV", responseCollection.ResultModel.NumV.ToResponse()); }
        public static ResultsResponseCollection NumV_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumV", value); }
        public static ResultsResponseCollection NumW(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumW", responseCollection.ResultModel.NumW.ToResponse()); }
        public static ResultsResponseCollection NumW(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumW", value); }
        public static ResultsResponseCollection NumW_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumW", responseCollection.ResultModel.NumW.ToResponse()); }
        public static ResultsResponseCollection NumW_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumW", value); }
        public static ResultsResponseCollection NumX(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumX", responseCollection.ResultModel.NumX.ToResponse()); }
        public static ResultsResponseCollection NumX(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumX", value); }
        public static ResultsResponseCollection NumX_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumX", responseCollection.ResultModel.NumX.ToResponse()); }
        public static ResultsResponseCollection NumX_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumX", value); }
        public static ResultsResponseCollection NumY(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumY", responseCollection.ResultModel.NumY.ToResponse()); }
        public static ResultsResponseCollection NumY(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumY", value); }
        public static ResultsResponseCollection NumY_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumY", responseCollection.ResultModel.NumY.ToResponse()); }
        public static ResultsResponseCollection NumY_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumY", value); }
        public static ResultsResponseCollection NumZ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_NumZ", responseCollection.ResultModel.NumZ.ToResponse()); }
        public static ResultsResponseCollection NumZ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_NumZ", value); }
        public static ResultsResponseCollection NumZ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_NumZ", responseCollection.ResultModel.NumZ.ToResponse()); }
        public static ResultsResponseCollection NumZ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_NumZ", value); }
        public static ResultsResponseCollection DateA(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateA", responseCollection.ResultModel.DateA.ToResponse()); }
        public static ResultsResponseCollection DateA(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateA", value); }
        public static ResultsResponseCollection DateA_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateA", responseCollection.ResultModel.DateA.ToResponse()); }
        public static ResultsResponseCollection DateA_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateA", value); }
        public static ResultsResponseCollection DateB(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateB", responseCollection.ResultModel.DateB.ToResponse()); }
        public static ResultsResponseCollection DateB(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateB", value); }
        public static ResultsResponseCollection DateB_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateB", responseCollection.ResultModel.DateB.ToResponse()); }
        public static ResultsResponseCollection DateB_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateB", value); }
        public static ResultsResponseCollection DateC(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateC", responseCollection.ResultModel.DateC.ToResponse()); }
        public static ResultsResponseCollection DateC(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateC", value); }
        public static ResultsResponseCollection DateC_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateC", responseCollection.ResultModel.DateC.ToResponse()); }
        public static ResultsResponseCollection DateC_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateC", value); }
        public static ResultsResponseCollection DateD(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateD", responseCollection.ResultModel.DateD.ToResponse()); }
        public static ResultsResponseCollection DateD(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateD", value); }
        public static ResultsResponseCollection DateD_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateD", responseCollection.ResultModel.DateD.ToResponse()); }
        public static ResultsResponseCollection DateD_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateD", value); }
        public static ResultsResponseCollection DateE(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateE", responseCollection.ResultModel.DateE.ToResponse()); }
        public static ResultsResponseCollection DateE(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateE", value); }
        public static ResultsResponseCollection DateE_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateE", responseCollection.ResultModel.DateE.ToResponse()); }
        public static ResultsResponseCollection DateE_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateE", value); }
        public static ResultsResponseCollection DateF(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateF", responseCollection.ResultModel.DateF.ToResponse()); }
        public static ResultsResponseCollection DateF(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateF", value); }
        public static ResultsResponseCollection DateF_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateF", responseCollection.ResultModel.DateF.ToResponse()); }
        public static ResultsResponseCollection DateF_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateF", value); }
        public static ResultsResponseCollection DateG(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateG", responseCollection.ResultModel.DateG.ToResponse()); }
        public static ResultsResponseCollection DateG(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateG", value); }
        public static ResultsResponseCollection DateG_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateG", responseCollection.ResultModel.DateG.ToResponse()); }
        public static ResultsResponseCollection DateG_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateG", value); }
        public static ResultsResponseCollection DateH(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateH", responseCollection.ResultModel.DateH.ToResponse()); }
        public static ResultsResponseCollection DateH(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateH", value); }
        public static ResultsResponseCollection DateH_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateH", responseCollection.ResultModel.DateH.ToResponse()); }
        public static ResultsResponseCollection DateH_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateH", value); }
        public static ResultsResponseCollection DateI(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateI", responseCollection.ResultModel.DateI.ToResponse()); }
        public static ResultsResponseCollection DateI(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateI", value); }
        public static ResultsResponseCollection DateI_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateI", responseCollection.ResultModel.DateI.ToResponse()); }
        public static ResultsResponseCollection DateI_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateI", value); }
        public static ResultsResponseCollection DateJ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateJ", responseCollection.ResultModel.DateJ.ToResponse()); }
        public static ResultsResponseCollection DateJ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateJ", value); }
        public static ResultsResponseCollection DateJ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateJ", responseCollection.ResultModel.DateJ.ToResponse()); }
        public static ResultsResponseCollection DateJ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateJ", value); }
        public static ResultsResponseCollection DateK(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateK", responseCollection.ResultModel.DateK.ToResponse()); }
        public static ResultsResponseCollection DateK(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateK", value); }
        public static ResultsResponseCollection DateK_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateK", responseCollection.ResultModel.DateK.ToResponse()); }
        public static ResultsResponseCollection DateK_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateK", value); }
        public static ResultsResponseCollection DateL(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateL", responseCollection.ResultModel.DateL.ToResponse()); }
        public static ResultsResponseCollection DateL(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateL", value); }
        public static ResultsResponseCollection DateL_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateL", responseCollection.ResultModel.DateL.ToResponse()); }
        public static ResultsResponseCollection DateL_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateL", value); }
        public static ResultsResponseCollection DateM(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateM", responseCollection.ResultModel.DateM.ToResponse()); }
        public static ResultsResponseCollection DateM(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateM", value); }
        public static ResultsResponseCollection DateM_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateM", responseCollection.ResultModel.DateM.ToResponse()); }
        public static ResultsResponseCollection DateM_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateM", value); }
        public static ResultsResponseCollection DateN(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateN", responseCollection.ResultModel.DateN.ToResponse()); }
        public static ResultsResponseCollection DateN(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateN", value); }
        public static ResultsResponseCollection DateN_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateN", responseCollection.ResultModel.DateN.ToResponse()); }
        public static ResultsResponseCollection DateN_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateN", value); }
        public static ResultsResponseCollection DateO(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateO", responseCollection.ResultModel.DateO.ToResponse()); }
        public static ResultsResponseCollection DateO(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateO", value); }
        public static ResultsResponseCollection DateO_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateO", responseCollection.ResultModel.DateO.ToResponse()); }
        public static ResultsResponseCollection DateO_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateO", value); }
        public static ResultsResponseCollection DateP(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateP", responseCollection.ResultModel.DateP.ToResponse()); }
        public static ResultsResponseCollection DateP(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateP", value); }
        public static ResultsResponseCollection DateP_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateP", responseCollection.ResultModel.DateP.ToResponse()); }
        public static ResultsResponseCollection DateP_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateP", value); }
        public static ResultsResponseCollection DateQ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateQ", responseCollection.ResultModel.DateQ.ToResponse()); }
        public static ResultsResponseCollection DateQ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateQ", value); }
        public static ResultsResponseCollection DateQ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateQ", responseCollection.ResultModel.DateQ.ToResponse()); }
        public static ResultsResponseCollection DateQ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateQ", value); }
        public static ResultsResponseCollection DateR(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateR", responseCollection.ResultModel.DateR.ToResponse()); }
        public static ResultsResponseCollection DateR(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateR", value); }
        public static ResultsResponseCollection DateR_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateR", responseCollection.ResultModel.DateR.ToResponse()); }
        public static ResultsResponseCollection DateR_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateR", value); }
        public static ResultsResponseCollection DateS(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateS", responseCollection.ResultModel.DateS.ToResponse()); }
        public static ResultsResponseCollection DateS(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateS", value); }
        public static ResultsResponseCollection DateS_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateS", responseCollection.ResultModel.DateS.ToResponse()); }
        public static ResultsResponseCollection DateS_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateS", value); }
        public static ResultsResponseCollection DateT(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateT", responseCollection.ResultModel.DateT.ToResponse()); }
        public static ResultsResponseCollection DateT(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateT", value); }
        public static ResultsResponseCollection DateT_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateT", responseCollection.ResultModel.DateT.ToResponse()); }
        public static ResultsResponseCollection DateT_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateT", value); }
        public static ResultsResponseCollection DateU(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateU", responseCollection.ResultModel.DateU.ToResponse()); }
        public static ResultsResponseCollection DateU(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateU", value); }
        public static ResultsResponseCollection DateU_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateU", responseCollection.ResultModel.DateU.ToResponse()); }
        public static ResultsResponseCollection DateU_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateU", value); }
        public static ResultsResponseCollection DateV(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateV", responseCollection.ResultModel.DateV.ToResponse()); }
        public static ResultsResponseCollection DateV(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateV", value); }
        public static ResultsResponseCollection DateV_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateV", responseCollection.ResultModel.DateV.ToResponse()); }
        public static ResultsResponseCollection DateV_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateV", value); }
        public static ResultsResponseCollection DateW(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateW", responseCollection.ResultModel.DateW.ToResponse()); }
        public static ResultsResponseCollection DateW(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateW", value); }
        public static ResultsResponseCollection DateW_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateW", responseCollection.ResultModel.DateW.ToResponse()); }
        public static ResultsResponseCollection DateW_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateW", value); }
        public static ResultsResponseCollection DateX(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateX", responseCollection.ResultModel.DateX.ToResponse()); }
        public static ResultsResponseCollection DateX(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateX", value); }
        public static ResultsResponseCollection DateX_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateX", responseCollection.ResultModel.DateX.ToResponse()); }
        public static ResultsResponseCollection DateX_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateX", value); }
        public static ResultsResponseCollection DateY(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateY", responseCollection.ResultModel.DateY.ToResponse()); }
        public static ResultsResponseCollection DateY(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateY", value); }
        public static ResultsResponseCollection DateY_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateY", responseCollection.ResultModel.DateY.ToResponse()); }
        public static ResultsResponseCollection DateY_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateY", value); }
        public static ResultsResponseCollection DateZ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DateZ", responseCollection.ResultModel.DateZ.ToResponse()); }
        public static ResultsResponseCollection DateZ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DateZ", value); }
        public static ResultsResponseCollection DateZ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DateZ", responseCollection.ResultModel.DateZ.ToResponse()); }
        public static ResultsResponseCollection DateZ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DateZ", value); }
        public static ResultsResponseCollection DescriptionA(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionA", responseCollection.ResultModel.DescriptionA.ToResponse()); }
        public static ResultsResponseCollection DescriptionA(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionA", value); }
        public static ResultsResponseCollection DescriptionA_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionA", responseCollection.ResultModel.DescriptionA.ToResponse()); }
        public static ResultsResponseCollection DescriptionA_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionA", value); }
        public static ResultsResponseCollection DescriptionB(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionB", responseCollection.ResultModel.DescriptionB.ToResponse()); }
        public static ResultsResponseCollection DescriptionB(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionB", value); }
        public static ResultsResponseCollection DescriptionB_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionB", responseCollection.ResultModel.DescriptionB.ToResponse()); }
        public static ResultsResponseCollection DescriptionB_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionB", value); }
        public static ResultsResponseCollection DescriptionC(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionC", responseCollection.ResultModel.DescriptionC.ToResponse()); }
        public static ResultsResponseCollection DescriptionC(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionC", value); }
        public static ResultsResponseCollection DescriptionC_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionC", responseCollection.ResultModel.DescriptionC.ToResponse()); }
        public static ResultsResponseCollection DescriptionC_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionC", value); }
        public static ResultsResponseCollection DescriptionD(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionD", responseCollection.ResultModel.DescriptionD.ToResponse()); }
        public static ResultsResponseCollection DescriptionD(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionD", value); }
        public static ResultsResponseCollection DescriptionD_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionD", responseCollection.ResultModel.DescriptionD.ToResponse()); }
        public static ResultsResponseCollection DescriptionD_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionD", value); }
        public static ResultsResponseCollection DescriptionE(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionE", responseCollection.ResultModel.DescriptionE.ToResponse()); }
        public static ResultsResponseCollection DescriptionE(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionE", value); }
        public static ResultsResponseCollection DescriptionE_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionE", responseCollection.ResultModel.DescriptionE.ToResponse()); }
        public static ResultsResponseCollection DescriptionE_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionE", value); }
        public static ResultsResponseCollection DescriptionF(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionF", responseCollection.ResultModel.DescriptionF.ToResponse()); }
        public static ResultsResponseCollection DescriptionF(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionF", value); }
        public static ResultsResponseCollection DescriptionF_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionF", responseCollection.ResultModel.DescriptionF.ToResponse()); }
        public static ResultsResponseCollection DescriptionF_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionF", value); }
        public static ResultsResponseCollection DescriptionG(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionG", responseCollection.ResultModel.DescriptionG.ToResponse()); }
        public static ResultsResponseCollection DescriptionG(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionG", value); }
        public static ResultsResponseCollection DescriptionG_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionG", responseCollection.ResultModel.DescriptionG.ToResponse()); }
        public static ResultsResponseCollection DescriptionG_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionG", value); }
        public static ResultsResponseCollection DescriptionH(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionH", responseCollection.ResultModel.DescriptionH.ToResponse()); }
        public static ResultsResponseCollection DescriptionH(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionH", value); }
        public static ResultsResponseCollection DescriptionH_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionH", responseCollection.ResultModel.DescriptionH.ToResponse()); }
        public static ResultsResponseCollection DescriptionH_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionH", value); }
        public static ResultsResponseCollection DescriptionI(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionI", responseCollection.ResultModel.DescriptionI.ToResponse()); }
        public static ResultsResponseCollection DescriptionI(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionI", value); }
        public static ResultsResponseCollection DescriptionI_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionI", responseCollection.ResultModel.DescriptionI.ToResponse()); }
        public static ResultsResponseCollection DescriptionI_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionI", value); }
        public static ResultsResponseCollection DescriptionJ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionJ", responseCollection.ResultModel.DescriptionJ.ToResponse()); }
        public static ResultsResponseCollection DescriptionJ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionJ", value); }
        public static ResultsResponseCollection DescriptionJ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionJ", responseCollection.ResultModel.DescriptionJ.ToResponse()); }
        public static ResultsResponseCollection DescriptionJ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionJ", value); }
        public static ResultsResponseCollection DescriptionK(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionK", responseCollection.ResultModel.DescriptionK.ToResponse()); }
        public static ResultsResponseCollection DescriptionK(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionK", value); }
        public static ResultsResponseCollection DescriptionK_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionK", responseCollection.ResultModel.DescriptionK.ToResponse()); }
        public static ResultsResponseCollection DescriptionK_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionK", value); }
        public static ResultsResponseCollection DescriptionL(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionL", responseCollection.ResultModel.DescriptionL.ToResponse()); }
        public static ResultsResponseCollection DescriptionL(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionL", value); }
        public static ResultsResponseCollection DescriptionL_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionL", responseCollection.ResultModel.DescriptionL.ToResponse()); }
        public static ResultsResponseCollection DescriptionL_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionL", value); }
        public static ResultsResponseCollection DescriptionM(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionM", responseCollection.ResultModel.DescriptionM.ToResponse()); }
        public static ResultsResponseCollection DescriptionM(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionM", value); }
        public static ResultsResponseCollection DescriptionM_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionM", responseCollection.ResultModel.DescriptionM.ToResponse()); }
        public static ResultsResponseCollection DescriptionM_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionM", value); }
        public static ResultsResponseCollection DescriptionN(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionN", responseCollection.ResultModel.DescriptionN.ToResponse()); }
        public static ResultsResponseCollection DescriptionN(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionN", value); }
        public static ResultsResponseCollection DescriptionN_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionN", responseCollection.ResultModel.DescriptionN.ToResponse()); }
        public static ResultsResponseCollection DescriptionN_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionN", value); }
        public static ResultsResponseCollection DescriptionO(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionO", responseCollection.ResultModel.DescriptionO.ToResponse()); }
        public static ResultsResponseCollection DescriptionO(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionO", value); }
        public static ResultsResponseCollection DescriptionO_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionO", responseCollection.ResultModel.DescriptionO.ToResponse()); }
        public static ResultsResponseCollection DescriptionO_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionO", value); }
        public static ResultsResponseCollection DescriptionP(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionP", responseCollection.ResultModel.DescriptionP.ToResponse()); }
        public static ResultsResponseCollection DescriptionP(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionP", value); }
        public static ResultsResponseCollection DescriptionP_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionP", responseCollection.ResultModel.DescriptionP.ToResponse()); }
        public static ResultsResponseCollection DescriptionP_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionP", value); }
        public static ResultsResponseCollection DescriptionQ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionQ", responseCollection.ResultModel.DescriptionQ.ToResponse()); }
        public static ResultsResponseCollection DescriptionQ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionQ", value); }
        public static ResultsResponseCollection DescriptionQ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionQ", responseCollection.ResultModel.DescriptionQ.ToResponse()); }
        public static ResultsResponseCollection DescriptionQ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionQ", value); }
        public static ResultsResponseCollection DescriptionR(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionR", responseCollection.ResultModel.DescriptionR.ToResponse()); }
        public static ResultsResponseCollection DescriptionR(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionR", value); }
        public static ResultsResponseCollection DescriptionR_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionR", responseCollection.ResultModel.DescriptionR.ToResponse()); }
        public static ResultsResponseCollection DescriptionR_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionR", value); }
        public static ResultsResponseCollection DescriptionS(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionS", responseCollection.ResultModel.DescriptionS.ToResponse()); }
        public static ResultsResponseCollection DescriptionS(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionS", value); }
        public static ResultsResponseCollection DescriptionS_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionS", responseCollection.ResultModel.DescriptionS.ToResponse()); }
        public static ResultsResponseCollection DescriptionS_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionS", value); }
        public static ResultsResponseCollection DescriptionT(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionT", responseCollection.ResultModel.DescriptionT.ToResponse()); }
        public static ResultsResponseCollection DescriptionT(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionT", value); }
        public static ResultsResponseCollection DescriptionT_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionT", responseCollection.ResultModel.DescriptionT.ToResponse()); }
        public static ResultsResponseCollection DescriptionT_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionT", value); }
        public static ResultsResponseCollection DescriptionU(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionU", responseCollection.ResultModel.DescriptionU.ToResponse()); }
        public static ResultsResponseCollection DescriptionU(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionU", value); }
        public static ResultsResponseCollection DescriptionU_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionU", responseCollection.ResultModel.DescriptionU.ToResponse()); }
        public static ResultsResponseCollection DescriptionU_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionU", value); }
        public static ResultsResponseCollection DescriptionV(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionV", responseCollection.ResultModel.DescriptionV.ToResponse()); }
        public static ResultsResponseCollection DescriptionV(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionV", value); }
        public static ResultsResponseCollection DescriptionV_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionV", responseCollection.ResultModel.DescriptionV.ToResponse()); }
        public static ResultsResponseCollection DescriptionV_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionV", value); }
        public static ResultsResponseCollection DescriptionW(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionW", responseCollection.ResultModel.DescriptionW.ToResponse()); }
        public static ResultsResponseCollection DescriptionW(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionW", value); }
        public static ResultsResponseCollection DescriptionW_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionW", responseCollection.ResultModel.DescriptionW.ToResponse()); }
        public static ResultsResponseCollection DescriptionW_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionW", value); }
        public static ResultsResponseCollection DescriptionX(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionX", responseCollection.ResultModel.DescriptionX.ToResponse()); }
        public static ResultsResponseCollection DescriptionX(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionX", value); }
        public static ResultsResponseCollection DescriptionX_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionX", responseCollection.ResultModel.DescriptionX.ToResponse()); }
        public static ResultsResponseCollection DescriptionX_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionX", value); }
        public static ResultsResponseCollection DescriptionY(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionY", responseCollection.ResultModel.DescriptionY.ToResponse()); }
        public static ResultsResponseCollection DescriptionY(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionY", value); }
        public static ResultsResponseCollection DescriptionY_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionY", responseCollection.ResultModel.DescriptionY.ToResponse()); }
        public static ResultsResponseCollection DescriptionY_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionY", value); }
        public static ResultsResponseCollection DescriptionZ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_DescriptionZ", responseCollection.ResultModel.DescriptionZ.ToResponse()); }
        public static ResultsResponseCollection DescriptionZ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_DescriptionZ", value); }
        public static ResultsResponseCollection DescriptionZ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_DescriptionZ", responseCollection.ResultModel.DescriptionZ.ToResponse()); }
        public static ResultsResponseCollection DescriptionZ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_DescriptionZ", value); }
        public static ResultsResponseCollection CheckA(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckA", responseCollection.ResultModel.CheckA.ToResponse()); }
        public static ResultsResponseCollection CheckA(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckA", value); }
        public static ResultsResponseCollection CheckA_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckA", responseCollection.ResultModel.CheckA.ToResponse()); }
        public static ResultsResponseCollection CheckA_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckA", value); }
        public static ResultsResponseCollection CheckB(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckB", responseCollection.ResultModel.CheckB.ToResponse()); }
        public static ResultsResponseCollection CheckB(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckB", value); }
        public static ResultsResponseCollection CheckB_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckB", responseCollection.ResultModel.CheckB.ToResponse()); }
        public static ResultsResponseCollection CheckB_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckB", value); }
        public static ResultsResponseCollection CheckC(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckC", responseCollection.ResultModel.CheckC.ToResponse()); }
        public static ResultsResponseCollection CheckC(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckC", value); }
        public static ResultsResponseCollection CheckC_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckC", responseCollection.ResultModel.CheckC.ToResponse()); }
        public static ResultsResponseCollection CheckC_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckC", value); }
        public static ResultsResponseCollection CheckD(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckD", responseCollection.ResultModel.CheckD.ToResponse()); }
        public static ResultsResponseCollection CheckD(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckD", value); }
        public static ResultsResponseCollection CheckD_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckD", responseCollection.ResultModel.CheckD.ToResponse()); }
        public static ResultsResponseCollection CheckD_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckD", value); }
        public static ResultsResponseCollection CheckE(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckE", responseCollection.ResultModel.CheckE.ToResponse()); }
        public static ResultsResponseCollection CheckE(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckE", value); }
        public static ResultsResponseCollection CheckE_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckE", responseCollection.ResultModel.CheckE.ToResponse()); }
        public static ResultsResponseCollection CheckE_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckE", value); }
        public static ResultsResponseCollection CheckF(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckF", responseCollection.ResultModel.CheckF.ToResponse()); }
        public static ResultsResponseCollection CheckF(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckF", value); }
        public static ResultsResponseCollection CheckF_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckF", responseCollection.ResultModel.CheckF.ToResponse()); }
        public static ResultsResponseCollection CheckF_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckF", value); }
        public static ResultsResponseCollection CheckG(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckG", responseCollection.ResultModel.CheckG.ToResponse()); }
        public static ResultsResponseCollection CheckG(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckG", value); }
        public static ResultsResponseCollection CheckG_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckG", responseCollection.ResultModel.CheckG.ToResponse()); }
        public static ResultsResponseCollection CheckG_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckG", value); }
        public static ResultsResponseCollection CheckH(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckH", responseCollection.ResultModel.CheckH.ToResponse()); }
        public static ResultsResponseCollection CheckH(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckH", value); }
        public static ResultsResponseCollection CheckH_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckH", responseCollection.ResultModel.CheckH.ToResponse()); }
        public static ResultsResponseCollection CheckH_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckH", value); }
        public static ResultsResponseCollection CheckI(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckI", responseCollection.ResultModel.CheckI.ToResponse()); }
        public static ResultsResponseCollection CheckI(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckI", value); }
        public static ResultsResponseCollection CheckI_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckI", responseCollection.ResultModel.CheckI.ToResponse()); }
        public static ResultsResponseCollection CheckI_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckI", value); }
        public static ResultsResponseCollection CheckJ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckJ", responseCollection.ResultModel.CheckJ.ToResponse()); }
        public static ResultsResponseCollection CheckJ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckJ", value); }
        public static ResultsResponseCollection CheckJ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckJ", responseCollection.ResultModel.CheckJ.ToResponse()); }
        public static ResultsResponseCollection CheckJ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckJ", value); }
        public static ResultsResponseCollection CheckK(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckK", responseCollection.ResultModel.CheckK.ToResponse()); }
        public static ResultsResponseCollection CheckK(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckK", value); }
        public static ResultsResponseCollection CheckK_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckK", responseCollection.ResultModel.CheckK.ToResponse()); }
        public static ResultsResponseCollection CheckK_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckK", value); }
        public static ResultsResponseCollection CheckL(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckL", responseCollection.ResultModel.CheckL.ToResponse()); }
        public static ResultsResponseCollection CheckL(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckL", value); }
        public static ResultsResponseCollection CheckL_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckL", responseCollection.ResultModel.CheckL.ToResponse()); }
        public static ResultsResponseCollection CheckL_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckL", value); }
        public static ResultsResponseCollection CheckM(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckM", responseCollection.ResultModel.CheckM.ToResponse()); }
        public static ResultsResponseCollection CheckM(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckM", value); }
        public static ResultsResponseCollection CheckM_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckM", responseCollection.ResultModel.CheckM.ToResponse()); }
        public static ResultsResponseCollection CheckM_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckM", value); }
        public static ResultsResponseCollection CheckN(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckN", responseCollection.ResultModel.CheckN.ToResponse()); }
        public static ResultsResponseCollection CheckN(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckN", value); }
        public static ResultsResponseCollection CheckN_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckN", responseCollection.ResultModel.CheckN.ToResponse()); }
        public static ResultsResponseCollection CheckN_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckN", value); }
        public static ResultsResponseCollection CheckO(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckO", responseCollection.ResultModel.CheckO.ToResponse()); }
        public static ResultsResponseCollection CheckO(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckO", value); }
        public static ResultsResponseCollection CheckO_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckO", responseCollection.ResultModel.CheckO.ToResponse()); }
        public static ResultsResponseCollection CheckO_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckO", value); }
        public static ResultsResponseCollection CheckP(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckP", responseCollection.ResultModel.CheckP.ToResponse()); }
        public static ResultsResponseCollection CheckP(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckP", value); }
        public static ResultsResponseCollection CheckP_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckP", responseCollection.ResultModel.CheckP.ToResponse()); }
        public static ResultsResponseCollection CheckP_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckP", value); }
        public static ResultsResponseCollection CheckQ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckQ", responseCollection.ResultModel.CheckQ.ToResponse()); }
        public static ResultsResponseCollection CheckQ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckQ", value); }
        public static ResultsResponseCollection CheckQ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckQ", responseCollection.ResultModel.CheckQ.ToResponse()); }
        public static ResultsResponseCollection CheckQ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckQ", value); }
        public static ResultsResponseCollection CheckR(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckR", responseCollection.ResultModel.CheckR.ToResponse()); }
        public static ResultsResponseCollection CheckR(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckR", value); }
        public static ResultsResponseCollection CheckR_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckR", responseCollection.ResultModel.CheckR.ToResponse()); }
        public static ResultsResponseCollection CheckR_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckR", value); }
        public static ResultsResponseCollection CheckS(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckS", responseCollection.ResultModel.CheckS.ToResponse()); }
        public static ResultsResponseCollection CheckS(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckS", value); }
        public static ResultsResponseCollection CheckS_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckS", responseCollection.ResultModel.CheckS.ToResponse()); }
        public static ResultsResponseCollection CheckS_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckS", value); }
        public static ResultsResponseCollection CheckT(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckT", responseCollection.ResultModel.CheckT.ToResponse()); }
        public static ResultsResponseCollection CheckT(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckT", value); }
        public static ResultsResponseCollection CheckT_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckT", responseCollection.ResultModel.CheckT.ToResponse()); }
        public static ResultsResponseCollection CheckT_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckT", value); }
        public static ResultsResponseCollection CheckU(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckU", responseCollection.ResultModel.CheckU.ToResponse()); }
        public static ResultsResponseCollection CheckU(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckU", value); }
        public static ResultsResponseCollection CheckU_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckU", responseCollection.ResultModel.CheckU.ToResponse()); }
        public static ResultsResponseCollection CheckU_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckU", value); }
        public static ResultsResponseCollection CheckV(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckV", responseCollection.ResultModel.CheckV.ToResponse()); }
        public static ResultsResponseCollection CheckV(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckV", value); }
        public static ResultsResponseCollection CheckV_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckV", responseCollection.ResultModel.CheckV.ToResponse()); }
        public static ResultsResponseCollection CheckV_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckV", value); }
        public static ResultsResponseCollection CheckW(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckW", responseCollection.ResultModel.CheckW.ToResponse()); }
        public static ResultsResponseCollection CheckW(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckW", value); }
        public static ResultsResponseCollection CheckW_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckW", responseCollection.ResultModel.CheckW.ToResponse()); }
        public static ResultsResponseCollection CheckW_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckW", value); }
        public static ResultsResponseCollection CheckX(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckX", responseCollection.ResultModel.CheckX.ToResponse()); }
        public static ResultsResponseCollection CheckX(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckX", value); }
        public static ResultsResponseCollection CheckX_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckX", responseCollection.ResultModel.CheckX.ToResponse()); }
        public static ResultsResponseCollection CheckX_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckX", value); }
        public static ResultsResponseCollection CheckY(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckY", responseCollection.ResultModel.CheckY.ToResponse()); }
        public static ResultsResponseCollection CheckY(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckY", value); }
        public static ResultsResponseCollection CheckY_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckY", responseCollection.ResultModel.CheckY.ToResponse()); }
        public static ResultsResponseCollection CheckY_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckY", value); }
        public static ResultsResponseCollection CheckZ(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CheckZ", responseCollection.ResultModel.CheckZ.ToResponse()); }
        public static ResultsResponseCollection CheckZ(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CheckZ", value); }
        public static ResultsResponseCollection CheckZ_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CheckZ", responseCollection.ResultModel.CheckZ.ToResponse()); }
        public static ResultsResponseCollection CheckZ_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CheckZ", value); }
        public static ResultsResponseCollection CreatedTime(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_CreatedTime", responseCollection.ResultModel.CreatedTime.ToResponse()); }
        public static ResultsResponseCollection CreatedTime(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_CreatedTime", value); }
        public static ResultsResponseCollection CreatedTime_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_CreatedTime", responseCollection.ResultModel.CreatedTime.ToResponse()); }
        public static ResultsResponseCollection CreatedTime_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_CreatedTime", value); }
        public static ResultsResponseCollection Timestamp(this ResultsResponseCollection responseCollection) { return responseCollection.Val("#Results_Timestamp", responseCollection.ResultModel.Timestamp.ToResponse()); }
        public static ResultsResponseCollection Timestamp(this ResultsResponseCollection responseCollection, string value) { return responseCollection.Val("#Results_Timestamp", value); }
        public static ResultsResponseCollection Timestamp_FormData(this ResultsResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Results_Timestamp", responseCollection.ResultModel.Timestamp.ToResponse()); }
        public static ResultsResponseCollection Timestamp_FormData(this ResultsResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Results_Timestamp", value); }
        public static WikisResponseCollection UpdatedTime(this WikisResponseCollection responseCollection) { return responseCollection.Val("#Wikis_UpdatedTime", responseCollection.WikiModel.UpdatedTime.ToResponse()); }
        public static WikisResponseCollection UpdatedTime(this WikisResponseCollection responseCollection, string value) { return responseCollection.Val("#Wikis_UpdatedTime", value); }
        public static WikisResponseCollection UpdatedTime_FormData(this WikisResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Wikis_UpdatedTime", responseCollection.WikiModel.UpdatedTime.ToResponse()); }
        public static WikisResponseCollection UpdatedTime_FormData(this WikisResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Wikis_UpdatedTime", value); }
        public static WikisResponseCollection WikiId(this WikisResponseCollection responseCollection) { return responseCollection.Val("#Wikis_WikiId", responseCollection.WikiModel.WikiId.ToResponse()); }
        public static WikisResponseCollection WikiId(this WikisResponseCollection responseCollection, string value) { return responseCollection.Val("#Wikis_WikiId", value); }
        public static WikisResponseCollection WikiId_FormData(this WikisResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Wikis_WikiId", responseCollection.WikiModel.WikiId.ToResponse()); }
        public static WikisResponseCollection WikiId_FormData(this WikisResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Wikis_WikiId", value); }
        public static WikisResponseCollection Ver(this WikisResponseCollection responseCollection) { return responseCollection.Val("#Wikis_Ver", responseCollection.WikiModel.Ver.ToResponse()); }
        public static WikisResponseCollection Ver(this WikisResponseCollection responseCollection, string value) { return responseCollection.Val("#Wikis_Ver", value); }
        public static WikisResponseCollection Ver_FormData(this WikisResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Wikis_Ver", responseCollection.WikiModel.Ver.ToResponse()); }
        public static WikisResponseCollection Ver_FormData(this WikisResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Wikis_Ver", value); }
        public static WikisResponseCollection Title(this WikisResponseCollection responseCollection) { return responseCollection.Val("#Wikis_Title", responseCollection.WikiModel.Title.ToResponse()); }
        public static WikisResponseCollection Title(this WikisResponseCollection responseCollection, string value) { return responseCollection.Val("#Wikis_Title", value); }
        public static WikisResponseCollection Title_FormData(this WikisResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Wikis_Title", responseCollection.WikiModel.Title.ToResponse()); }
        public static WikisResponseCollection Title_FormData(this WikisResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Wikis_Title", value); }
        public static WikisResponseCollection Body(this WikisResponseCollection responseCollection) { return responseCollection.Val("#Wikis_Body", responseCollection.WikiModel.Body.ToResponse()); }
        public static WikisResponseCollection Body(this WikisResponseCollection responseCollection, string value) { return responseCollection.Val("#Wikis_Body", value); }
        public static WikisResponseCollection Body_FormData(this WikisResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Wikis_Body", responseCollection.WikiModel.Body.ToResponse()); }
        public static WikisResponseCollection Body_FormData(this WikisResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Wikis_Body", value); }
        public static WikisResponseCollection CreatedTime(this WikisResponseCollection responseCollection) { return responseCollection.Val("#Wikis_CreatedTime", responseCollection.WikiModel.CreatedTime.ToResponse()); }
        public static WikisResponseCollection CreatedTime(this WikisResponseCollection responseCollection, string value) { return responseCollection.Val("#Wikis_CreatedTime", value); }
        public static WikisResponseCollection CreatedTime_FormData(this WikisResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Wikis_CreatedTime", responseCollection.WikiModel.CreatedTime.ToResponse()); }
        public static WikisResponseCollection CreatedTime_FormData(this WikisResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Wikis_CreatedTime", value); }
        public static WikisResponseCollection Timestamp(this WikisResponseCollection responseCollection) { return responseCollection.Val("#Wikis_Timestamp", responseCollection.WikiModel.Timestamp.ToResponse()); }
        public static WikisResponseCollection Timestamp(this WikisResponseCollection responseCollection, string value) { return responseCollection.Val("#Wikis_Timestamp", value); }
        public static WikisResponseCollection Timestamp_FormData(this WikisResponseCollection responseCollection) { return responseCollection.ValAndFormData("#Wikis_Timestamp", responseCollection.WikiModel.Timestamp.ToResponse()); }
        public static WikisResponseCollection Timestamp_FormData(this WikisResponseCollection responseCollection, string value) { return responseCollection.ValAndFormData("#Wikis_Timestamp", value); }
    }
}
