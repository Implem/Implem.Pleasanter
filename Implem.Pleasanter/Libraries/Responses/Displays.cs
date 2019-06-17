using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Displays
    {
        public static Dictionary<string, string> DisplayHash = GetDisplayHash();

        private static Dictionary<string, string> GetDisplayHash()
        {
            var data = new Dictionary<string, string>();
            DisplayAccessor.Displays.DisplayHash.ForEach(display =>
                display.Value.Languages.ForEach(element =>
                    data.Add(
                        display.Key + (!element.Language.IsNullOrEmpty()
                            ? "_" + element.Language
                            : string.Empty),
                        element.Body)));
            Def.ColumnDefinitionCollection
                .Where(o => !o.ExtendedColumnType.IsNullOrEmpty())
                .ForEach(columnDefinition =>
                {
                    data.AddIfNotConainsKey(
                        columnDefinition.Id,
                        columnDefinition.ColumnName);
                    data.AddIfNotConainsKey(
                        columnDefinition.Id + "_ja",
                        columnDefinition.LabelText);
                });
            return data;
        }

        public static string Get(Context context, string id, params string[] data)
        {
            var screen = id;
            var kay = id + "_" + context.Language;
            if (DisplayHash.ContainsKey(kay))
            {
                screen = DisplayHash[kay];
            }
            else if (DisplayHash.ContainsKey(id))
            {
                screen = DisplayHash[id];
            }
            return data?.Any() == true
                ? screen.Params(data)
                : screen;
        }

        public static string Display(this PasswordPolicy policy, Context context)
        {
            return policy.Languages.FirstOrDefault(o => o.Language == context.Language)?.Body
                ?? policy.Languages.FirstOrDefault(o => o.Language.IsNullOrEmpty())?.Body
                ?? policy.Languages.FirstOrDefault()?.Body;
        }

        public static string Add(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Add",
                data: data);
        }

        public static string AddPermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AddPermission",
                data: data);
        }

        public static string Address(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Address",
                data: data);
        }

        public static string AddressBook(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AddressBook",
                data: data);
        }

        public static string Admin(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Admin",
                data: data);
        }

        public static string AdvancedSetting(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AdvancedSetting",
                data: data);
        }

        public static string AfterCondition(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AfterCondition",
                data: data);
        }

        public static string AggregationDetails(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AggregationDetails",
                data: data);
        }

        public static string Aggregations(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Aggregations",
                data: data);
        }

        public static string AggregationSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AggregationSettings",
                data: data);
        }

        public static string AggregationTarget(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AggregationTarget",
                data: data);
        }

        public static string AggregationType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AggregationType",
                data: data);
        }

        public static string AggregationView(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AggregationView",
                data: data);
        }

        public static string AGPL(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AGPL",
                data: data);
        }

        public static string All(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "All",
                data: data);
        }

        public static string AllowEditingComments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AllowEditingComments",
                data: data);
        }

        public static string AllowedUsers(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AllowedUsers",
                data: data);
        }

        public static string AllowImage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AllowImage",
                data: data);
        }

        public static string AllUsers(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AllUsers",
                data: data);
        }

        public static string AlreadyAdded(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AlreadyAdded",
                data: data);
        }

        public static string AlreadyLinked(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "AlreadyLinked",
                data: data);
        }

        public static string And(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "And",
                data: data);
        }

        public static string ApiKey(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApiKey",
                data: data);
        }

        public static string ApiKeyCreated(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApiKeyCreated",
                data: data);
        }

        public static string ApiKeyDeleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApiKeyDeleted",
                data: data);
        }

        public static string ApiSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApiSettings",
                data: data);
        }

        public static string ApplicationError(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApplicationError",
                data: data);
        }

        public static string Approval(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Approval",
                data: data);
        }

        public static string ApprovalMailBody(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalMailBody",
                data: data);
        }

        public static string ApprovalMailTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalMailTitle",
                data: data);
        }

        public static string ApprovalMessage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalMessage",
                data: data);
        }

        public static string ApprovalMessageInvited(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalMessageInvited",
                data: data);
        }

        public static string ApprovalMessageInviting(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalMessageInviting",
                data: data);
        }

        public static string ApprovalReauest(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalReauest",
                data: data);
        }

        public static string ApprovalReauestMailBody(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalReauestMailBody",
                data: data);
        }

        public static string ApprovalReauestMailTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalReauestMailTitle",
                data: data);
        }

        public static string ApprovalRequestMessage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalRequestMessage",
                data: data);
        }

        public static string ApprovalRequestMessageRequesting(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ApprovalRequestMessageRequesting",
                data: data);
        }

        public static string Assembly(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Assembly",
                data: data);
        }

        public static string Attachments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Attachments",
                data: data);
        }

        public static string Authentication(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Authentication",
                data: data);
        }

        public static string Auto(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Auto",
                data: data);
        }

        public static string Average(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Average",
                data: data);
        }

        public static string BadFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BadFormat",
                data: data);
        }

        public static string BadMailAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BadMailAddress",
                data: data);
        }

        public static string BadRequest(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BadRequest",
                data: data);
        }

        public static string Basic(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Basic",
                data: data);
        }

        public static string BeforeCondition(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BeforeCondition",
                data: data);
        }

        public static string Blog(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Blog",
                data: data);
        }

        public static string Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Body",
                data: data);
        }

        public static string BroadMatchOfTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BroadMatchOfTitle",
                data: data);
        }

        public static string BulkDelete(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BulkDelete",
                data: data);
        }

        public static string BulkDeleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BulkDeleted",
                data: data);
        }

        public static string BulkMove(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BulkMove",
                data: data);
        }

        public static string BulkMoved(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BulkMoved",
                data: data);
        }

        public static string BulkRestored(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BulkRestored",
                data: data);
        }

        public static string BulkUpdated(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BulkUpdated",
                data: data);
        }

        public static string BurnDown(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BurnDown",
                data: data);
        }

        public static string BusinessImprovement(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "BusinessImprovement",
                data: data);
        }

        public static string Calendar(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Calendar",
                data: data);
        }

        public static string Camera(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Camera",
                data: data);
        }

        public static string Cancel(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Cancel",
                data: data);
        }

        public static string CanNotChangeInheritance(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CanNotChangeInheritance",
                data: data);
        }

        public static string CanNotConnectCamera(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CanNotConnectCamera",
                data: data);
        }

        public static string CanNotDisabled(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CanNotDisabled",
                data: data);
        }

        public static string CanNotInherit(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CanNotInherit",
                data: data);
        }

        public static string CanNotLink(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CanNotLink",
                data: data);
        }

        public static string CanNotPerformed(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CanNotPerformed",
                data: data);
        }

        public static string CanNotUpdate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CanNotUpdate",
                data: data);
        }

        public static string CantSetAtTopOfSite(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CantSetAtTopOfSite",
                data: data);
        }

        public static string Change(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Change",
                data: data);
        }

        public static string ChangeHistoryList(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ChangeHistoryList",
                data: data);
        }

        public static string ChangePassword(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ChangePassword",
                data: data);
        }

        public static string ChangingPasswordComplete(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ChangingPasswordComplete",
                data: data);
        }

        public static string CharacterCode(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CharacterCode",
                data: data);
        }

        public static string ChatWork(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ChatWork",
                data: data);
        }

        public static string Check(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Check",
                data: data);
        }

        public static string CheckAll(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CheckAll",
                data: data);
        }

        public static string Class(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Class",
                data: data);
        }

        public static string Classification(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Classification",
                data: data);
        }

        public static string Clear(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Clear",
                data: data);
        }

        public static string CodeDefinerBackupCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CodeDefinerBackupCompleted",
                data: data);
        }

        public static string CodeDefinerCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CodeDefinerCompleted",
                data: data);
        }

        public static string CodeDefinerCssCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CodeDefinerCssCompleted",
                data: data);
        }

        public static string CodeDefinerDefCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CodeDefinerDefCompleted",
                data: data);
        }

        public static string CodeDefinerInsertTestDataCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CodeDefinerInsertTestDataCompleted",
                data: data);
        }

        public static string CodeDefinerMvcCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CodeDefinerMvcCompleted",
                data: data);
        }

        public static string CodeDefinerRdsCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CodeDefinerRdsCompleted",
                data: data);
        }

        public static string Column(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Column",
                data: data);
        }

        public static string ColumnAccessControl(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ColumnAccessControl",
                data: data);
        }

        public static string ColumnList(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ColumnList",
                data: data);
        }

        public static string CommentDeleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CommentDeleted",
                data: data);
        }

        public static string Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Comments",
                data: data);
        }

        public static string CommentUpdated(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CommentUpdated",
                data: data);
        }

        public static string CommercialLicense(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CommercialLicense",
                data: data);
        }

        public static string Condition(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Condition",
                data: data);
        }

        public static string ConfirmCreateLink(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmCreateLink",
                data: data);
        }

        public static string ConfirmDelete(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmDelete",
                data: data);
        }

        public static string ConfirmDeleteSite(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmDeleteSite",
                data: data);
        }

        public static string ConfirmPhysicalDelete(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmPhysicalDelete",
                data: data);
        }

        public static string ConfirmRebuildSearchIndex(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmRebuildSearchIndex",
                data: data);
        }

        public static string ConfirmReset(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmReset",
                data: data);
        }

        public static string ConfirmRestore(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmRestore",
                data: data);
        }

        public static string ConfirmSendMail(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmSendMail",
                data: data);
        }

        public static string ConfirmSeparate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmSeparate",
                data: data);
        }

        public static string ConfirmSwitchUser(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmSwitchUser",
                data: data);
        }

        public static string ConfirmSynchronize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmSynchronize",
                data: data);
        }

        public static string ConfirmUnload(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ConfirmUnload",
                data: data);
        }

        public static string Contact(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Contact",
                data: data);
        }

        public static string ControlType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ControlType",
                data: data);
        }

        public static string Copied(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Copied",
                data: data);
        }

        public static string Copy(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Copy",
                data: data);
        }

        public static string CopyByDefault(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CopyByDefault",
                data: data);
        }

        public static string CopySettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CopySettings",
                data: data);
        }

        public static string CopyToNew(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CopyToNew",
                data: data);
        }

        public static string CopyWithComments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CopyWithComments",
                data: data);
        }

        public static string CorporatePlanning(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CorporatePlanning",
                data: data);
        }

        public static string Count(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Count",
                data: data);
        }

        public static string Create(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Create",
                data: data);
        }

        public static string CreateColumnAccessControl(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CreateColumnAccessControl",
                data: data);
        }

        public static string Created(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Created",
                data: data);
        }

        public static string CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CreatedTime",
                data: data);
        }

        public static string Crosstab(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Crosstab",
                data: data);
        }

        public static string Csv(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Csv",
                data: data);
        }

        public static string CsvFile(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CsvFile",
                data: data);
        }

        public static string Currency(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Currency",
                data: data);
        }

        public static string CurrentMembers(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CurrentMembers",
                data: data);
        }

        public static string CurrentPassword(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CurrentPassword",
                data: data);
        }

        public static string CurrentSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CurrentSettings",
                data: data);
        }

        public static string Custom(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Custom",
                data: data);
        }

        public static string CustomDesign(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "CustomDesign",
                data: data);
        }

        public static string Customer(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Customer",
                data: data);
        }

        public static string Daily(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Daily",
                data: data);
        }

        public static string DatabaseSize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DatabaseSize",
                data: data);
        }

        public static string DataStorageDestination(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DataStorageDestination",
                data: data);
        }

        public static string DataView(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DataView",
                data: data);
        }

        public static string Date(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Date",
                data: data);
        }

        public static string DateFilterSetMode(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DateFilterSetMode",
                data: data);
        }

        public static string DateFilterSetModeDefault(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DateFilterSetModeDefault",
                data: data);
        }

        public static string DateFilterSetModeRange(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DateFilterSetModeRange",
                data: data);
        }

        public static string DateRange(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DateRange",
                data: data);
        }

        public static string Day(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Day",
                data: data);
        }

        public static string DayAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DayAgo",
                data: data);
        }

        public static string DayOfWeek(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DayOfWeek",
                data: data);
        }

        public static string DaysAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DaysAgo",
                data: data);
        }

        public static string DecimalPlaces(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DecimalPlaces",
                data: data);
        }

        public static string Default(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Default",
                data: data);
        }

        public static string DefaultAddressBook(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DefaultAddressBook",
                data: data);
        }

        public static string DefaultDestinations(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DefaultDestinations",
                data: data);
        }

        public static string DefaultInput(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DefaultInput",
                data: data);
        }

        public static string DefaultView(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DefaultView",
                data: data);
        }

        public static string DefinitionNotFound(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DefinitionNotFound",
                data: data);
        }

        public static string Delay(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Delay",
                data: data);
        }

        public static string Delete(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Delete",
                data: data);
        }

        public static string DeleteConflicts(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DeleteConflicts",
                data: data);
        }

        public static string Deleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Deleted",
                data: data);
        }

        public static string DeletedImage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DeletedImage",
                data: data);
        }

        public static string DeletedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DeletedTime",
                data: data);
        }

        public static string DeleteFromTrashBox(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DeleteFromTrashBox",
                data: data);
        }

        public static string DeleteHistory(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DeleteHistory",
                data: data);
        }

        public static string DeletePermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DeletePermission",
                data: data);
        }

        public static string Deleter(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Deleter",
                data: data);
        }

        public static string DeleteSite(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DeleteSite",
                data: data);
        }

        public static string DemoMailBody(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DemoMailBody",
                data: data);
        }

        public static string DemoMailTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DemoMailTitle",
                data: data);
        }

        public static string DeptAdmin(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DeptAdmin",
                data: data);
        }

        public static string Description(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Description",
                data: data);
        }

        public static string Destination(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Destination",
                data: data);
        }

        public static string Difference(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Difference",
                data: data);
        }

        public static string DirectUrlCopied(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DirectUrlCopied",
                data: data);
        }

        public static string Disabled(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Disabled",
                data: data);
        }

        public static string SendCompletedInPast(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SendCompletedInPast",
                data: data);
        }

        public static string DisplayName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "DisplayName",
                data: data);
        }

        public static string Duplicated(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Duplicated",
                data: data);
        }

        public static string EarnedValue(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EarnedValue",
                data: data);
        }

        public static string Edit(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Edit",
                data: data);
        }

        public static string EditInDialog(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditInDialog",
                data: data);
        }

        public static string EditInGrid(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditInGrid",
                data: data);
        }

        public static string EditMode(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditMode",
                data: data);
        }

        public static string Editor(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Editor",
                data: data);
        }

        public static string EditorFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditorFormat",
                data: data);
        }

        public static string EditorSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditorSettings",
                data: data);
        }

        public static string EditProfile(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditProfile",
                data: data);
        }

        public static string EditScript(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditScript",
                data: data);
        }

        public static string EditSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditSettings",
                data: data);
        }

        public static string EditStyle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EditStyle",
                data: data);
        }

        public static string Education(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Education",
                data: data);
        }

        public static string EmptyUserName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EmptyUserName",
                data: data);
        }

        public static string Enabled(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Enabled",
                data: data);
        }

        public static string End(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "End",
                data: data);
        }

        public static string EndOfMonth(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EndOfMonth",
                data: data);
        }

        public static string EnterTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "EnterTitle",
                data: data);
        }

        public static string Error(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Error",
                data: data);
        }

        public static string Excel(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Excel",
                data: data);
        }

        public static string Expired(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Expired",
                data: data);
        }

        public static string Export(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Export",
                data: data);
        }

        public static string ExportColumns(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportColumns",
                data: data);
        }

        public static string ExportFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportFormat",
                data: data);
        }

        public static string ExportTypes(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportTypes",
                data: data);
        }

        public static string Expression(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Expression",
                data: data);
        }

        public static string ExternalMailAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExternalMailAddress",
                data: data);
        }

        public static string FailedReadFile(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "FailedReadFile",
                data: data);
        }

        public static string File(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "File",
                data: data);
        }

        public static string FileDeleteCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "FileDeleteCompleted",
                data: data);
        }

        public static string FileDragDrop(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "FileDragDrop",
                data: data);
        }

        public static string FileNotFound(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "FileNotFound",
                data: data);
        }

        public static string FileUpdateCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "FileUpdateCompleted",
                data: data);
        }

        public static string Filters(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Filters",
                data: data);
        }

        public static string FilterSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "FilterSettings",
                data: data);
        }

        public static string FirstDay(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "FirstDay",
                data: data);
        }

        public static string Folder(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Folder",
                data: data);
        }

        public static string ForceUnlockTable(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ForceUnlockTable",
                data: data);
        }

        public static string Format(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Format",
                data: data);
        }

        public static string Formulas(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Formulas",
                data: data);
        }

        public static string Friday(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Friday",
                data: data);
        }

        public static string From(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "From",
                data: data);
        }

        public static string FullText(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "FullText",
                data: data);
        }

        public static string Fy(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Fy",
                data: data);
        }

        public static string Gantt(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Gantt",
                data: data);
        }

        public static string General(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "General",
                data: data);
        }

        public static string GeneralUser(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GeneralUser",
                data: data);
        }

        public static string GoBack(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GoBack",
                data: data);
        }

        public static string Grid(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Grid",
                data: data);
        }

        public static string GridEditorTypes(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GridEditorTypes",
                data: data);
        }

        public static string GridFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GridFormat",
                data: data);
        }

        public static string GridScript(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GridScript",
                data: data);
        }

        public static string GridStyle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GridStyle",
                data: data);
        }

        public static string GroupAdmin(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupAdmin",
                data: data);
        }

        public static string GroupBy(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupBy",
                data: data);
        }

        public static string GroupByX(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupByX",
                data: data);
        }

        public static string GroupByY(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupByY",
                data: data);
        }

        public static string Half1(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Half1",
                data: data);
        }

        public static string Half2(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Half2",
                data: data);
        }

        public static string HasBeenDeleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HasBeenDeleted",
                data: data);
        }

        public static string HasBeenMoved(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HasBeenMoved",
                data: data);
        }

        public static string HasNotPermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HasNotPermission",
                data: data);
        }

        public static string Hidden(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Hidden",
                data: data);
        }

        public static string Hide(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Hide",
                data: data);
        }

        public static string HideList(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HideList",
                data: data);
        }

        public static string Histories(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Histories",
                data: data);
        }

        public static string HistoryDeleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HistoryDeleted",
                data: data);
        }

        public static string HistoryOnGrid(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HistoryOnGrid",
                data: data);
        }

        public static string HourAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HourAgo",
                data: data);
        }

        public static string HoursAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HoursAgo",
                data: data);
        }

        public static string HumanResourcesAndGeneralAffairs(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "HumanResourcesAndGeneralAffairs",
                data: data);
        }

        public static string Hyphen(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Hyphen",
                data: data);
        }

        public static string Icon(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Icon",
                data: data);
        }

        public static string Id(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Id",
                data: data);
        }

        public static string ImageAndText(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ImageAndText",
                data: data);
        }

        public static string ImageLib(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ImageLib",
                data: data);
        }

        public static string ImageOnly(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ImageOnly",
                data: data);
        }

        public static string Import(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Import",
                data: data);
        }

        public static string Imported(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Imported",
                data: data);
        }

        public static string ImportMax(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ImportMax",
                data: data);
        }

        public static string Incomplete(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Incomplete",
                data: data);
        }

        public static string InCompression(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InCompression",
                data: data);
        }

        public static string InCopying(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InCopying",
                data: data);
        }

        public static string IncorrectCurrentPassword(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "IncorrectCurrentPassword",
                data: data);
        }

        public static string IncorrectFileFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "IncorrectFileFormat",
                data: data);
        }

        public static string IncorrectSiteDeleting(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "IncorrectSiteDeleting",
                data: data);
        }

        public static string Index(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Index",
                data: data);
        }

        public static string InformationSystem(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InformationSystem",
                data: data);
        }

        public static string InheritPermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InheritPermission",
                data: data);
        }

        public static string InputMailAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InputMailAddress",
                data: data);
        }

        public static string InternalServerError(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InternalServerError",
                data: data);
        }

        public static string InvalidCsvData(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InvalidCsvData",
                data: data);
        }

        public static string InvalidFormula(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InvalidFormula",
                data: data);
        }

        public static string InvalidIpAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InvalidIpAddress",
                data: data);
        }

        public static string InvalidRequest(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InvalidRequest",
                data: data);
        }

        public static string InvalidSsoCode(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InvalidSsoCode",
                data: data);
        }

        public static string InvitationMailBody(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InvitationMailBody",
                data: data);
        }

        public static string InvitationMailTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InvitationMailTitle",
                data: data);
        }

        public static string Invite(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Invite",
                data: data);
        }

        public static string Invited(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Invited",
                data: data);
        }

        public static string InviteMessage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "InviteMessage",
                data: data);
        }

        public static string Inviting(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Inviting",
                data: data);
        }

        public static string ItemsLimit(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ItemsLimit",
                data: data);
        }

        public static string JoeAccountCheck(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "JoeAccountCheck",
                data: data);
        }

        public static string Json(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Json",
                data: data);
        }

        public static string Kamban(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Kamban",
                data: data);
        }

        public static string Latest(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Latest",
                data: data);
        }

        public static string Leader(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Leader",
                data: data);
        }

        public static string LegalAffairs(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LegalAffairs",
                data: data);
        }

        public static string LessThan(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LessThan",
                data: data);
        }

        public static string License(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "License",
                data: data);
        }

        public static string LicenseDeadline(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LicenseDeadline",
                data: data);
        }

        public static string Licensee(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Licensee",
                data: data);
        }

        public static string LimitAfterDay(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterDay",
                data: data);
        }

        public static string LimitAfterDays(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterDays",
                data: data);
        }

        public static string LimitAfterHour(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterHour",
                data: data);
        }

        public static string LimitAfterHours(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterHours",
                data: data);
        }

        public static string LimitAfterMinute(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterMinute",
                data: data);
        }

        public static string LimitAfterMinutes(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterMinutes",
                data: data);
        }

        public static string LimitAfterMonth(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterMonth",
                data: data);
        }

        public static string LimitAfterMonths(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterMonths",
                data: data);
        }

        public static string LimitAfterSecond(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterSecond",
                data: data);
        }

        public static string LimitAfterSeconds(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterSeconds",
                data: data);
        }

        public static string LimitAfterYear(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterYear",
                data: data);
        }

        public static string LimitAfterYears(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitAfterYears",
                data: data);
        }

        public static string LimitBeforeDay(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeDay",
                data: data);
        }

        public static string LimitBeforeDays(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeDays",
                data: data);
        }

        public static string LimitBeforeHour(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeHour",
                data: data);
        }

        public static string LimitBeforeHours(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeHours",
                data: data);
        }

        public static string LimitBeforeMinute(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeMinute",
                data: data);
        }

        public static string LimitBeforeMinutes(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeMinutes",
                data: data);
        }

        public static string LimitBeforeMonth(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeMonth",
                data: data);
        }

        public static string LimitBeforeMonths(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeMonths",
                data: data);
        }

        public static string LimitBeforeSecond(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeSecond",
                data: data);
        }

        public static string LimitBeforeSeconds(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeSeconds",
                data: data);
        }

        public static string LimitBeforeYear(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeYear",
                data: data);
        }

        public static string LimitBeforeYears(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitBeforeYears",
                data: data);
        }

        public static string LimitJust(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitJust",
                data: data);
        }

        public static string LimitQuantity(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitQuantity",
                data: data);
        }

        public static string LimitSize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitSize",
                data: data);
        }

        public static string LimitTotalSize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LimitTotalSize",
                data: data);
        }

        public static string Line(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Line",
                data: data);
        }

        public static string LineGroup(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LineGroup",
                data: data);
        }

        public static string LinkColumn(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LinkColumn",
                data: data);
        }

        public static string LinkCreated(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LinkCreated",
                data: data);
        }

        public static string LinkCreations(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LinkCreations",
                data: data);
        }

        public static string LinkDestinations(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LinkDestinations",
                data: data);
        }

        public static string Links(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links",
                data: data);
        }

        public static string LinkSources(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LinkSources",
                data: data);
        }

        public static string List(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "List",
                data: data);
        }

        public static string ListMode(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ListMode",
                data: data);
        }

        public static string ListSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ListSettings",
                data: data);
        }

        public static string LockTable(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LockTable",
                data: data);
        }

        public static string LockWarning(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LockWarning",
                data: data);
        }

        public static string Login(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Login",
                data: data);
        }

        public static string LoginIdAlreadyUse(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginIdAlreadyUse",
                data: data);
        }

        public static string LoginIn(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginIn",
                data: data);
        }

        public static string Logistics(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Logistics",
                data: data);
        }

        public static string LogoImage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LogoImage",
                data: data);
        }

        public static string Logout(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Logout",
                data: data);
        }

        public static string Mail(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Mail",
                data: data);
        }

        public static string MailAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddress",
                data: data);
        }

        public static string MailAddressHasNotSet(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddressHasNotSet",
                data: data);
        }

        public static string MailTransmissionCompletion(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailTransmissionCompletion",
                data: data);
        }

        public static string Manage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Manage",
                data: data);
        }

        public static string ManageFolder(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ManageFolder",
                data: data);
        }

        public static string ManagePermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ManagePermission",
                data: data);
        }

        public static string ManagePermissions(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ManagePermissions",
                data: data);
        }

        public static string Manager(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Manager",
                data: data);
        }

        public static string ManageSite(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ManageSite",
                data: data);
        }

        public static string ManageWiki(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ManageWiki",
                data: data);
        }

        public static string Manufacture(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Manufacture",
                data: data);
        }

        public static string MarkDown(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MarkDown",
                data: data);
        }

        public static string Marketing(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Marketing",
                data: data);
        }

        public static string MatchInFrontOfTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MatchInFrontOfTitle",
                data: data);
        }

        public static string Max(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Max",
                data: data);
        }

        public static string MaxColumns(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MaxColumns",
                data: data);
        }

        public static string Md(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Md",
                data: data);
        }

        public static string MdFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MdFormat",
                data: data);
        }

        public static string Members(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Members",
                data: data);
        }

        public static string Menu(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Menu",
                data: data);
        }

        public static string MenuGuide(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MenuGuide",
                data: data);
        }

        public static string Min(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Min",
                data: data);
        }

        public static string MinuteAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MinuteAgo",
                data: data);
        }

        public static string MinutesAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MinutesAgo",
                data: data);
        }

        public static string Monday(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Monday",
                data: data);
        }

        public static string MonitorChangesColumns(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MonitorChangesColumns",
                data: data);
        }

        public static string Month(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Month",
                data: data);
        }

        public static string MonthAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MonthAgo",
                data: data);
        }

        public static string Monthly(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Monthly",
                data: data);
        }

        public static string MonthsAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MonthsAgo",
                data: data);
        }

        public static string Move(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Move",
                data: data);
        }

        public static string Moved(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Moved",
                data: data);
        }

        public static string MoveDown(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MoveDown",
                data: data);
        }

        public static string MoveSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MoveSettings",
                data: data);
        }

        public static string MoveTargetsSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MoveTargetsSettings",
                data: data);
        }

        public static string MoveUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MoveUp",
                data: data);
        }

        public static string Name(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Name",
                data: data);
        }

        public static string NearCompletionTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NearCompletionTime",
                data: data);
        }

        public static string NearCompletionTimeAfterDays(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NearCompletionTimeAfterDays",
                data: data);
        }

        public static string NearCompletionTimeBeforeDays(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NearCompletionTimeBeforeDays",
                data: data);
        }

        public static string New(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "New",
                data: data);
        }

        public static string Newer(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Newer",
                data: data);
        }

        public static string NewScript(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NewScript",
                data: data);
        }

        public static string NewStyle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NewStyle",
                data: data);
        }

        public static string Next(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Next",
                data: data);
        }

        public static string NoClassification(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NoClassification",
                data: data);
        }

        public static string NoData(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NoData",
                data: data);
        }

        public static string NoDuplication(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NoDuplication",
                data: data);
        }

        public static string NoLinks(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NoLinks",
                data: data);
        }

        public static string Normal(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Normal",
                data: data);
        }

        public static string NoTargetRecord(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NoTargetRecord",
                data: data);
        }

        public static string NotFound(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NotFound",
                data: data);
        }

        public static string Notifications(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Notifications",
                data: data);
        }

        public static string NotificationType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NotificationType",
                data: data);
        }

        public static string NotInheritPermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NotInheritPermission",
                data: data);
        }

        public static string NoTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NoTitle",
                data: data);
        }

        public static string NotOutput(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NotOutput",
                data: data);
        }

        public static string NotRequiredColumn(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NotRequiredColumn",
                data: data);
        }

        public static string NotSendIfNotApplicable(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NotSendIfNotApplicable",
                data: data);
        }

        public static string NotSet(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NotSet",
                data: data);
        }

        public static string NoWrap(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NoWrap",
                data: data);
        }

        public static string Num(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Num",
                data: data);
        }

        public static string NumberPerPage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NumberPerPage",
                data: data);
        }

        public static string NumberWeekly(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NumberWeekly",
                data: data);
        }

        public static string NumericColumn(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "NumericColumn",
                data: data);
        }

        public static string Off(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Off",
                data: data);
        }

        public static string OK(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OK",
                data: data);
        }

        public static string Older(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Older",
                data: data);
        }

        public static string On(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "On",
                data: data);
        }

        public static string OnAndOff(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OnAndOff",
                data: data);
        }

        public static string OnOnly(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OnOnly",
                data: data);
        }

        public static string Operations(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Operations",
                data: data);
        }

        public static string OptionList(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OptionList",
                data: data);
        }

        public static string Or(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Or",
                data: data);
        }

        public static string OrderAsc(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OrderAsc",
                data: data);
        }

        public static string OrderDesc(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OrderDesc",
                data: data);
        }

        public static string OrderRelease(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OrderRelease",
                data: data);
        }

        public static string OriginalMessage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OriginalMessage",
                data: data);
        }

        public static string Others(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Others",
                data: data);
        }

        public static string OutgoingMail(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMail",
                data: data);
        }

        public static string OutOfCondition(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutOfCondition",
                data: data);
        }

        public static string Output(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Output",
                data: data);
        }

        public static string OutputDestination(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutputDestination",
                data: data);
        }

        public static string OutputHeader(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutputHeader",
                data: data);
        }

        public static string Over(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Over",
                data: data);
        }

        public static string Overdue(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Overdue",
                data: data);
        }

        public static string OverLimitQuantity(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OverLimitQuantity",
                data: data);
        }

        public static string OverLimitSize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OverLimitSize",
                data: data);
        }

        public static string OverTenantStorageSize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OverTenantStorageSize",
                data: data);
        }

        public static string OverTotalLimitSize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OverTotalLimitSize",
                data: data);
        }

        public static string Own(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Own",
                data: data);
        }

        public static string ParameterSyntaxError(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ParameterSyntaxError",
                data: data);
        }

        public static string PartialMatch(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PartialMatch",
                data: data);
        }

        public static string Password(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Password",
                data: data);
        }

        public static string PasswordNotChanged(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PasswordNotChanged",
                data: data);
        }

        public static string PasswordPolicyViolation(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PasswordPolicyViolation",
                data: data);
        }

        public static string PasswordResetCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PasswordResetCompleted",
                data: data);
        }

        public static string Pattern(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Pattern",
                data: data);
        }

        public static string Period(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Period",
                data: data);
        }

        public static string PeriodType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PeriodType",
                data: data);
        }

        public static string PermissionDestination(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PermissionDestination",
                data: data);
        }

        public static string PermissionForCreating(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PermissionForCreating",
                data: data);
        }

        public static string PermissionNotSelfChange(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PermissionNotSelfChange",
                data: data);
        }

        public static string PermissionSetting(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PermissionSetting",
                data: data);
        }

        public static string PermissionSource(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PermissionSource",
                data: data);
        }

        public static string PhysicalDeleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PhysicalDeleted",
                data: data);
        }

        public static string Plan(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Plan",
                data: data);
        }

        public static string PlannedValue(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PlannedValue",
                data: data);
        }

        public static string Portal(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Portal",
                data: data);
        }

        public static string Prefix(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Prefix",
                data: data);
        }

        public static string Previous(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Previous",
                data: data);
        }

        public static string ProductList(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ProductList",
                data: data);
        }

        public static string ProductName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ProductName",
                data: data);
        }

        public static string Project(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Project",
                data: data);
        }

        public static string Publish(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Publish",
                data: data);
        }

        public static string PublishToAnonymousUsers(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PublishToAnonymousUsers",
                data: data);
        }

        public static string PublishWarning(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "PublishWarning",
                data: data);
        }

        public static string Purchase(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Purchase",
                data: data);
        }

        public static string Quantity(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Quantity",
                data: data);
        }

        public static string Quarter(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Quarter",
                data: data);
        }

        public static string Range(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Range",
                data: data);
        }

        public static string Read(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Read",
                data: data);
        }

        public static string ReadColumnAccessControl(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReadColumnAccessControl",
                data: data);
        }

        public static string ReadOnly(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReadOnly",
                data: data);
        }

        public static string ReadOnlyBecausePreviousVer(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReadOnlyBecausePreviousVer",
                data: data);
        }

        public static string ReadWrite(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReadWrite",
                data: data);
        }

        public static string ReauestingApproval(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReauestingApproval",
                data: data);
        }

        public static string RebuildingCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RebuildingCompleted",
                data: data);
        }

        public static string RebuildSearchIndexes(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RebuildSearchIndexes",
                data: data);
        }

        public static string RecordAccessControl(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RecordAccessControl",
                data: data);
        }

        public static string ReCreate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReCreate",
                data: data);
        }

        public static string Register(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Register",
                data: data);
        }

        public static string RelatingColumn(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RelatingColumn",
                data: data);
        }

        public static string RelatingColumnSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RelatingColumnSettings",
                data: data);
        }

        public static string Reload(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Reload",
                data: data);
        }

        public static string Reminders(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Reminders",
                data: data);
        }

        public static string Reply(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Reply",
                data: data);
        }

        public static string Required(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Required",
                data: data);
        }

        public static string RequiredPermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RequiredPermission",
                data: data);
        }

        public static string RequireMailAddresses(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RequireMailAddresses",
                data: data);
        }

        public static string RequireManagePermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RequireManagePermission",
                data: data);
        }

        public static string RequireTo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RequireTo",
                data: data);
        }

        public static string ResearchAndDevelopment(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ResearchAndDevelopment",
                data: data);
        }

        public static string Reset(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Reset",
                data: data);
        }

        public static string ResetOrder(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ResetOrder",
                data: data);
        }

        public static string ResetPassword(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ResetPassword",
                data: data);
        }

        public static string Restore(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Restore",
                data: data);
        }

        public static string RestoredFromHistory(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "RestoredFromHistory",
                data: data);
        }

        public static string Restricted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Restricted",
                data: data);
        }

        public static string Row(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Row",
                data: data);
        }

        public static string Sales(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sales",
                data: data);
        }

        public static string SamlLoginFailed(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SamlLoginFailed",
                data: data);
        }

        public static string SamplesDisplayed(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SamplesDisplayed",
                data: data);
        }

        public static string Saturday(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Saturday",
                data: data);
        }

        public static string Save(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Save",
                data: data);
        }

        public static string Script(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Script",
                data: data);
        }

        public static string Scripts(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Scripts",
                data: data);
        }

        public static string Search(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Search",
                data: data);
        }

        public static string SearchSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchSettings",
                data: data);
        }

        public static string SearchTypes(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchTypes",
                data: data);
        }

        public static string SecondsAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SecondsAgo",
                data: data);
        }

        public static string Section(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Section",
                data: data);
        }

        public static string Select(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Select",
                data: data);
        }

        public static string SelectableMembers(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SelectableMembers",
                data: data);
        }

        public static string SelectFile(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SelectFile",
                data: data);
        }

        public static string SelectOne(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SelectOne",
                data: data);
        }

        public static string SelectTargets(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SelectTargets",
                data: data);
        }

        public static string SelectTemplate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SelectTemplate",
                data: data);
        }

        public static string Send(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Send",
                data: data);
        }

        public static string SendMail(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SendMail",
                data: data);
        }

        public static string SentAcceptanceMail (
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SentAcceptanceMail ",
                data: data);
        }

        public static string SentMail(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SentMail",
                data: data);
        }

        public static string Separate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Separate",
                data: data);
        }

        public static string Separated(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Separated",
                data: data);
        }

        public static string SeparateNumber(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SeparateNumber",
                data: data);
        }

        public static string SeparateSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SeparateSettings",
                data: data);
        }

        public static string Setting(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Setting",
                data: data);
        }

        public static string SetZeroWhenOutOfCondition(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SetZeroWhenOutOfCondition",
                data: data);
        }

        public static string ShortDisplayName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ShortDisplayName",
                data: data);
        }

        public static string ShowHistory(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ShowHistory",
                data: data);
        }

        public static string ShowProgressRate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ShowProgressRate",
                data: data);
        }

        public static string SiteAccessControl(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SiteAccessControl",
                data: data);
        }

        public static string SiteId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SiteId",
                data: data);
        }

        public static string SiteImageSettingsEditor(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SiteImageSettingsEditor",
                data: data);
        }

        public static string SiteIntegration(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SiteIntegration",
                data: data);
        }

        public static string SiteName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SiteName",
                data: data);
        }

        public static string SitesCreated(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SitesCreated",
                data: data);
        }

        public static string SiteSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SiteSettings",
                data: data);
        }

        public static string SitesLimit(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SitesLimit",
                data: data);
        }

        public static string SiteTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SiteTitle",
                data: data);
        }

        public static string SiteUser(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SiteUser",
                data: data);
        }

        public static string Slack(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Slack",
                data: data);
        }

        public static string SortBy(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SortBy",
                data: data);
        }

        public static string Sorters(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sorters",
                data: data);
        }

        public static string Special(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Special",
                data: data);
        }

        public static string Spinner(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Spinner",
                data: data);
        }

        public static string Standard(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Standard",
                data: data);
        }

        public static string Start(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Start",
                data: data);
        }

        public static string StartDate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "StartDate",
                data: data);
        }

        public static string StartDateTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "StartDateTime",
                data: data);
        }

        public static string Step(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Step",
                data: data);
        }

        public static string Store(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Store",
                data: data);
        }

        public static string Style(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Style",
                data: data);
        }

        public static string Styles(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Styles",
                data: data);
        }

        public static string Subject(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Subject",
                data: data);
        }

        public static string SuffixCopy(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SuffixCopy",
                data: data);
        }

        public static string Summaries(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Summaries",
                data: data);
        }

        public static string SummaryLinkColumn(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SummaryLinkColumn",
                data: data);
        }

        public static string SummarySourceColumn(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SummarySourceColumn",
                data: data);
        }

        public static string SummaryType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SummaryType",
                data: data);
        }

        public static string Sunday(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sunday",
                data: data);
        }

        public static string Support(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Support",
                data: data);
        }

        public static string SwitchRecordWithAjax(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SwitchRecordWithAjax",
                data: data);
        }

        public static string SwitchUser(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SwitchUser",
                data: data);
        }

        public static string SwitchUserInfo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SwitchUserInfo",
                data: data);
        }

        public static string SynchronizationCompleted(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SynchronizationCompleted",
                data: data);
        }

        public static string Synchronize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Synchronize",
                data: data);
        }

        public static string Tables(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tables",
                data: data);
        }

        public static string ManageTable(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ManageTable",
                data: data);
        }

        public static string Target(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Target",
                data: data);
        }

        public static string Teams(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Teams",
                data: data);
        }

        public static string Template(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Template",
                data: data);
        }

        public static string TenantAdmin(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "TenantAdmin",
                data: data);
        }

        public static string TenantImageType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "TenantImageType",
                data: data);
        }

        public static string Test(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Test",
                data: data);
        }

        public static string ThisMonth(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ThisMonth",
                data: data);
        }

        public static string Thursday(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Thursday",
                data: data);
        }

        public static string TimeSeries(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "TimeSeries",
                data: data);
        }

        public static string Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Title",
                data: data);
        }

        public static string TitleSeparator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "TitleSeparator",
                data: data);
        }

        public static string To(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "To",
                data: data);
        }

        public static string Today(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Today",
                data: data);
        }

        public static string ToDisable(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ToDisable",
                data: data);
        }

        public static string ToEnable(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ToEnable",
                data: data);
        }

        public static string Token(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Token",
                data: data);
        }

        public static string TooManyCases(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "TooManyCases",
                data: data);
        }

        public static string TooManyColumnCases(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "TooManyColumnCases",
                data: data);
        }

        public static string TooManyRowCases(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "TooManyRowCases",
                data: data);
        }

        public static string Top(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Top",
                data: data);
        }

        public static string ToParent(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ToParent",
                data: data);
        }

        public static string ToShoot(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ToShoot",
                data: data);
        }

        public static string Total(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Total",
                data: data);
        }

        public static string TrashBox(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "TrashBox",
                data: data);
        }

        public static string Tuesday(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tuesday",
                data: data);
        }

        public static string Unauthorized(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Unauthorized",
                data: data);
        }

        public static string UncheckAll(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UncheckAll",
                data: data);
        }

        public static string Unit(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Unit",
                data: data);
        }

        public static string UnlockTable(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UnlockTable",
                data: data);
        }

        public static string UpdatableImport(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UpdatableImport",
                data: data);
        }

        public static string Update(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Update",
                data: data);
        }

        public static string UpdateColumnAccessControl(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UpdateColumnAccessControl",
                data: data);
        }

        public static string UpdateConflicts(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UpdateConflicts",
                data: data);
        }

        public static string Updated(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Updated",
                data: data);
        }

        public static string UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UpdatedTime",
                data: data);
        }

        public static string Upload(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Upload",
                data: data);
        }

        public static string UsageGuide(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UsageGuide",
                data: data);
        }

        public static string UseCustomDesign(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UseCustomDesign",
                data: data);
        }

        public static string UseFy(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UseFy",
                data: data);
        }

        public static string UseHalf(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UseHalf",
                data: data);
        }

        public static string UseMonth(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UseMonth",
                data: data);
        }

        public static string UseQuarter(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UseQuarter",
                data: data);
        }

        public static string UserAdmin(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UserAdmin",
                data: data);
        }

        public static string UserDisabled(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UserDisabled",
                data: data);
        }

        public static string UserLockout(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UserLockout",
                data: data);
        }

        public static string UserNotSelfDelete(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UserNotSelfDelete",
                data: data);
        }

        public static string UsersLimit(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UsersLimit",
                data: data);
        }

        public static string UserSwitched(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UserSwitched",
                data: data);
        }

        public static string UseSearch(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "UseSearch",
                data: data);
        }

        public static string ValidateDate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidateDate",
                data: data);
        }

        public static string ValidateEmail(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidateEmail",
                data: data);
        }

        public static string ValidateEqualTo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidateEqualTo",
                data: data);
        }

        public static string ValidateMaxLength(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidateMaxLength",
                data: data);
        }

        public static string ValidateMaxNumber(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidateMaxNumber",
                data: data);
        }

        public static string ValidateMinNumber(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidateMinNumber",
                data: data);
        }

        public static string ValidateNumber(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidateNumber",
                data: data);
        }

        public static string ValidateRequired(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidateRequired",
                data: data);
        }

        public static string ValidationError(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ValidationError",
                data: data);
        }

        public static string Value(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Value",
                data: data);
        }

        public static string Version(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Version",
                data: data);
        }

        public static string VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "VerUp",
                data: data);
        }

        public static string View(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "View",
                data: data);
        }

        public static string ViewDemoEnvironment(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ViewDemoEnvironment",
                data: data);
        }

        public static string Wednesday(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wednesday",
                data: data);
        }

        public static string Week(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Week",
                data: data);
        }

        public static string Weekly(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Weekly",
                data: data);
        }

        public static string Wide(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wide",
                data: data);
        }

        public static string WorkValue(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "WorkValue",
                data: data);
        }

        public static string Year(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Year",
                data: data);
        }

        public static string Yearly(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Yearly",
                data: data);
        }

        public static string YearsAgo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YearsAgo",
                data: data);
        }

        public static string Ym(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Ym",
                data: data);
        }

        public static string Ymd(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Ymd",
                data: data);
        }

        public static string Ymda(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Ymda",
                data: data);
        }

        public static string YmdaFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmdaFormat",
                data: data);
        }

        public static string Ymdahm(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Ymdahm",
                data: data);
        }

        public static string YmdahmFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmdahmFormat",
                data: data);
        }

        public static string Ymdahms(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Ymdahms",
                data: data);
        }

        public static string YmdahmsFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmdahmsFormat",
                data: data);
        }

        public static string YmdDatePickerFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmdDatePickerFormat",
                data: data);
        }

        public static string YmdFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmdFormat",
                data: data);
        }

        public static string Ymdhm(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Ymdhm",
                data: data);
        }

        public static string YmdhmDatePickerFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmdhmDatePickerFormat",
                data: data);
        }

        public static string YmdhmFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmdhmFormat",
                data: data);
        }

        public static string Ymdhms(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Ymdhms",
                data: data);
        }

        public static string YmdhmsFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmdhmsFormat",
                data: data);
        }

        public static string YmFormat(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "YmFormat",
                data: data);
        }

        public static string Tenants_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_TenantId",
                data: data);
        }

        public static string Tenants_TenantName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_TenantName",
                data: data);
        }

        public static string Tenants_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_Title",
                data: data);
        }

        public static string Tenants_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_Body",
                data: data);
        }

        public static string Tenants_ContractSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_ContractSettings",
                data: data);
        }

        public static string Tenants_ContractDeadline(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_ContractDeadline",
                data: data);
        }

        public static string Tenants_DisableAllUsersPermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_DisableAllUsersPermission",
                data: data);
        }

        public static string Tenants_LogoType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_LogoType",
                data: data);
        }

        public static string Tenants_HtmlTitleTop(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_HtmlTitleTop",
                data: data);
        }

        public static string Tenants_HtmlTitleSite(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_HtmlTitleSite",
                data: data);
        }

        public static string Tenants_HtmlTitleRecord(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_HtmlTitleRecord",
                data: data);
        }

        public static string Demos_DemoId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_DemoId",
                data: data);
        }

        public static string Demos_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_TenantId",
                data: data);
        }

        public static string Demos_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_Title",
                data: data);
        }

        public static string Demos_LoginId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_LoginId",
                data: data);
        }

        public static string Demos_Passphrase(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_Passphrase",
                data: data);
        }

        public static string Demos_MailAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_MailAddress",
                data: data);
        }

        public static string Demos_Initialized(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_Initialized",
                data: data);
        }

        public static string Demos_TimeLag(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_TimeLag",
                data: data);
        }

        public static string Sessions_SessionGuid(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_SessionGuid",
                data: data);
        }

        public static string Sessions_Key(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_Key",
                data: data);
        }

        public static string Sessions_Page(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_Page",
                data: data);
        }

        public static string Sessions_Value(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_Value",
                data: data);
        }

        public static string Sessions_ReadOnce(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_ReadOnce",
                data: data);
        }

        public static string Sessions_UserArea(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_UserArea",
                data: data);
        }

        public static string SysLogs_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_CreatedTime",
                data: data);
        }

        public static string SysLogs_SysLogId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_SysLogId",
                data: data);
        }

        public static string SysLogs_StartTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_StartTime",
                data: data);
        }

        public static string SysLogs_EndTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_EndTime",
                data: data);
        }

        public static string SysLogs_SysLogType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_SysLogType",
                data: data);
        }

        public static string SysLogs_OnAzure(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_OnAzure",
                data: data);
        }

        public static string SysLogs_MachineName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_MachineName",
                data: data);
        }

        public static string SysLogs_ServiceName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_ServiceName",
                data: data);
        }

        public static string SysLogs_TenantName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_TenantName",
                data: data);
        }

        public static string SysLogs_Application(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Application",
                data: data);
        }

        public static string SysLogs_Class(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Class",
                data: data);
        }

        public static string SysLogs_Method(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Method",
                data: data);
        }

        public static string SysLogs_RequestData(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_RequestData",
                data: data);
        }

        public static string SysLogs_HttpMethod(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_HttpMethod",
                data: data);
        }

        public static string SysLogs_RequestSize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_RequestSize",
                data: data);
        }

        public static string SysLogs_ResponseSize(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_ResponseSize",
                data: data);
        }

        public static string SysLogs_Elapsed(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Elapsed",
                data: data);
        }

        public static string SysLogs_ApplicationAge(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_ApplicationAge",
                data: data);
        }

        public static string SysLogs_ApplicationRequestInterval(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_ApplicationRequestInterval",
                data: data);
        }

        public static string SysLogs_SessionAge(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_SessionAge",
                data: data);
        }

        public static string SysLogs_SessionRequestInterval(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_SessionRequestInterval",
                data: data);
        }

        public static string SysLogs_WorkingSet64(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_WorkingSet64",
                data: data);
        }

        public static string SysLogs_VirtualMemorySize64(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_VirtualMemorySize64",
                data: data);
        }

        public static string SysLogs_ProcessId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_ProcessId",
                data: data);
        }

        public static string SysLogs_ProcessName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_ProcessName",
                data: data);
        }

        public static string SysLogs_BasePriority(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_BasePriority",
                data: data);
        }

        public static string SysLogs_Url(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Url",
                data: data);
        }

        public static string SysLogs_UrlReferer(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_UrlReferer",
                data: data);
        }

        public static string SysLogs_UserHostName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_UserHostName",
                data: data);
        }

        public static string SysLogs_UserHostAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_UserHostAddress",
                data: data);
        }

        public static string SysLogs_UserLanguage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_UserLanguage",
                data: data);
        }

        public static string SysLogs_UserAgent(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_UserAgent",
                data: data);
        }

        public static string SysLogs_SessionGuid(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_SessionGuid",
                data: data);
        }

        public static string SysLogs_ErrMessage(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_ErrMessage",
                data: data);
        }

        public static string SysLogs_ErrStackTrace(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_ErrStackTrace",
                data: data);
        }

        public static string SysLogs_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Title",
                data: data);
        }

        public static string SysLogs_InDebug(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_InDebug",
                data: data);
        }

        public static string SysLogs_AssemblyVersion(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_AssemblyVersion",
                data: data);
        }

        public static string Statuses_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_TenantId",
                data: data);
        }

        public static string Statuses_StatusId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_StatusId",
                data: data);
        }

        public static string Statuses_Value(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_Value",
                data: data);
        }

        public static string ReminderSchedules_SiteId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_SiteId",
                data: data);
        }

        public static string ReminderSchedules_Id(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_Id",
                data: data);
        }

        public static string ReminderSchedules_ScheduledTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_ScheduledTime",
                data: data);
        }

        public static string Depts_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_TenantId",
                data: data);
        }

        public static string Depts_DeptId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_DeptId",
                data: data);
        }

        public static string Depts_DeptCode(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_DeptCode",
                data: data);
        }

        public static string Depts_Dept(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_Dept",
                data: data);
        }

        public static string Depts_DeptName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_DeptName",
                data: data);
        }

        public static string Depts_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_Body",
                data: data);
        }

        public static string Depts_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_Title",
                data: data);
        }

        public static string Groups_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_TenantId",
                data: data);
        }

        public static string Groups_GroupId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_GroupId",
                data: data);
        }

        public static string Groups_GroupName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_GroupName",
                data: data);
        }

        public static string Groups_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_Body",
                data: data);
        }

        public static string Groups_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_Title",
                data: data);
        }

        public static string GroupMembers_GroupId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_GroupId",
                data: data);
        }

        public static string GroupMembers_DeptId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_DeptId",
                data: data);
        }

        public static string GroupMembers_UserId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_UserId",
                data: data);
        }

        public static string GroupMembers_Admin(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_Admin",
                data: data);
        }

        public static string Registrations_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_TenantId",
                data: data);
        }

        public static string Registrations_RegistrationId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_RegistrationId",
                data: data);
        }

        public static string Registrations_MailAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_MailAddress",
                data: data);
        }

        public static string Registrations_Invitee(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Invitee",
                data: data);
        }

        public static string _(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "_",
                data: data);
        }

        public static string Registrations_LoginId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_LoginId",
                data: data);
        }

        public static string Registrations_Name(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Name",
                data: data);
        }

        public static string Registrations_Password(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Password",
                data: data);
        }

        public static string Registrations_PasswordValidate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_PasswordValidate",
                data: data);
        }

        public static string Registrations_Language(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Language",
                data: data);
        }

        public static string Registrations_Passphrase(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Passphrase",
                data: data);
        }

        public static string Registrations_Invitingflg(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Invitingflg",
                data: data);
        }

        public static string Registrations_UserId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_UserId",
                data: data);
        }

        public static string Registrations_DeptId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_DeptId",
                data: data);
        }

        public static string Registrations_GroupId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_GroupId",
                data: data);
        }

        public static string Users_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_TenantId",
                data: data);
        }

        public static string Users_UserId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_UserId",
                data: data);
        }

        public static string Users_LoginId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_LoginId",
                data: data);
        }

        public static string Users_GlobalId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_GlobalId",
                data: data);
        }

        public static string Users_Name(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Name",
                data: data);
        }

        public static string Users_UserCode(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_UserCode",
                data: data);
        }

        public static string Users_Password(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Password",
                data: data);
        }

        public static string Users_PasswordValidate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_PasswordValidate",
                data: data);
        }

        public static string Users_PasswordDummy(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_PasswordDummy",
                data: data);
        }

        public static string Users_RememberMe(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_RememberMe",
                data: data);
        }

        public static string Users_LastName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_LastName",
                data: data);
        }

        public static string Users_FirstName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_FirstName",
                data: data);
        }

        public static string Users_Birthday(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Birthday",
                data: data);
        }

        public static string Users_Gender(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Gender",
                data: data);
        }

        public static string Users_Language(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Language",
                data: data);
        }

        public static string Users_TimeZone(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_TimeZone",
                data: data);
        }

        public static string Users_TimeZoneInfo(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_TimeZoneInfo",
                data: data);
        }

        public static string Users_DeptCode(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_DeptCode",
                data: data);
        }

        public static string Users_DeptId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_DeptId",
                data: data);
        }

        public static string Users_Dept(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Dept",
                data: data);
        }

        public static string Users_FirstAndLastNameOrder(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_FirstAndLastNameOrder",
                data: data);
        }

        public static string Users_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Title",
                data: data);
        }

        public static string Users_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Body",
                data: data);
        }

        public static string Users_LastLoginTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_LastLoginTime",
                data: data);
        }

        public static string Users_PasswordExpirationTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_PasswordExpirationTime",
                data: data);
        }

        public static string Users_PasswordChangeTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_PasswordChangeTime",
                data: data);
        }

        public static string Users_NumberOfLogins(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_NumberOfLogins",
                data: data);
        }

        public static string Users_NumberOfDenial(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_NumberOfDenial",
                data: data);
        }

        public static string Users_TenantManager(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_TenantManager",
                data: data);
        }

        public static string Users_ServiceManager(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_ServiceManager",
                data: data);
        }

        public static string Users_Disabled(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Disabled",
                data: data);
        }

        public static string Users_Lockout(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Lockout",
                data: data);
        }

        public static string Users_LockoutCounter(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_LockoutCounter",
                data: data);
        }

        public static string Users_Developer(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Developer",
                data: data);
        }

        public static string Users_UserSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_UserSettings",
                data: data);
        }

        public static string Users_ApiKey(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_ApiKey",
                data: data);
        }

        public static string Users_OldPassword(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_OldPassword",
                data: data);
        }

        public static string Users_ChangedPassword(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_ChangedPassword",
                data: data);
        }

        public static string Users_ChangedPasswordValidator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_ChangedPasswordValidator",
                data: data);
        }

        public static string Users_AfterResetPassword(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_AfterResetPassword",
                data: data);
        }

        public static string Users_AfterResetPasswordValidator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_AfterResetPasswordValidator",
                data: data);
        }

        public static string Users_MailAddresses(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_MailAddresses",
                data: data);
        }

        public static string Users_DemoMailAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_DemoMailAddress",
                data: data);
        }

        public static string Users_SessionGuid(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_SessionGuid",
                data: data);
        }

        public static string Users_LdapSearchRoot(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_LdapSearchRoot",
                data: data);
        }

        public static string Users_SynchronizedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_SynchronizedTime",
                data: data);
        }

        public static string LoginKeys_LoginId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_LoginId",
                data: data);
        }

        public static string LoginKeys_Key(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_Key",
                data: data);
        }

        public static string LoginKeys_TenantNames(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_TenantNames",
                data: data);
        }

        public static string LoginKeys_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_TenantId",
                data: data);
        }

        public static string LoginKeys_UserId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_UserId",
                data: data);
        }

        public static string MailAddresses_OwnerId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_OwnerId",
                data: data);
        }

        public static string MailAddresses_OwnerType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_OwnerType",
                data: data);
        }

        public static string MailAddresses_MailAddressId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_MailAddressId",
                data: data);
        }

        public static string MailAddresses_MailAddress(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_MailAddress",
                data: data);
        }

        public static string MailAddresses_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_Title",
                data: data);
        }

        public static string Permissions_ReferenceId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_ReferenceId",
                data: data);
        }

        public static string Permissions_DeptId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_DeptId",
                data: data);
        }

        public static string Permissions_GroupId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_GroupId",
                data: data);
        }

        public static string Permissions_UserId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_UserId",
                data: data);
        }

        public static string Permissions_DeptName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_DeptName",
                data: data);
        }

        public static string Permissions_GroupName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_GroupName",
                data: data);
        }

        public static string Permissions_Name(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_Name",
                data: data);
        }

        public static string Permissions_PermissionType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_PermissionType",
                data: data);
        }

        public static string OutgoingMails_ReferenceType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_ReferenceType",
                data: data);
        }

        public static string OutgoingMails_ReferenceId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_ReferenceId",
                data: data);
        }

        public static string OutgoingMails_ReferenceVer(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_ReferenceVer",
                data: data);
        }

        public static string OutgoingMails_OutgoingMailId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_OutgoingMailId",
                data: data);
        }

        public static string OutgoingMails_Host(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Host",
                data: data);
        }

        public static string OutgoingMails_Port(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Port",
                data: data);
        }

        public static string OutgoingMails_From(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_From",
                data: data);
        }

        public static string OutgoingMails_To(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_To",
                data: data);
        }

        public static string OutgoingMails_Cc(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Cc",
                data: data);
        }

        public static string OutgoingMails_Bcc(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Bcc",
                data: data);
        }

        public static string OutgoingMails_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Title",
                data: data);
        }

        public static string OutgoingMails_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Body",
                data: data);
        }

        public static string OutgoingMails_SentTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_SentTime",
                data: data);
        }

        public static string OutgoingMails_DestinationSearchRange(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_DestinationSearchRange",
                data: data);
        }

        public static string OutgoingMails_DestinationSearchText(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_DestinationSearchText",
                data: data);
        }

        public static string SearchIndexes_Word(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Word",
                data: data);
        }

        public static string SearchIndexes_ReferenceId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_ReferenceId",
                data: data);
        }

        public static string SearchIndexes_Priority(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Priority",
                data: data);
        }

        public static string SearchIndexes_ReferenceType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_ReferenceType",
                data: data);
        }

        public static string SearchIndexes_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Title",
                data: data);
        }

        public static string SearchIndexes_Subset(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Subset",
                data: data);
        }

        public static string SearchIndexes_InheritPermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_InheritPermission",
                data: data);
        }

        public static string Items_ReferenceId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_ReferenceId",
                data: data);
        }

        public static string Items_ReferenceType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_ReferenceType",
                data: data);
        }

        public static string Items_SiteId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_SiteId",
                data: data);
        }

        public static string Items_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_Title",
                data: data);
        }

        public static string Items_Site(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_Site",
                data: data);
        }

        public static string Items_FullText(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_FullText",
                data: data);
        }

        public static string Items_SearchIndexCreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_SearchIndexCreatedTime",
                data: data);
        }

        public static string Items_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_UpdatedTime",
                data: data);
        }

        public static string Sites_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_TenantId",
                data: data);
        }

        public static string Sites_SiteId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_SiteId",
                data: data);
        }

        public static string Sites_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Title",
                data: data);
        }

        public static string Sites_GridGuide(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_GridGuide",
                data: data);
        }

        public static string Sites_EditorGuide(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_EditorGuide",
                data: data);
        }

        public static string Sites_ReferenceType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_ReferenceType",
                data: data);
        }

        public static string Sites_ParentId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_ParentId",
                data: data);
        }

        public static string Sites_InheritPermission(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_InheritPermission",
                data: data);
        }

        public static string Sites_SiteSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_SiteSettings",
                data: data);
        }

        public static string Sites_Publish(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Publish",
                data: data);
        }

        public static string Sites_LockedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_LockedTime",
                data: data);
        }

        public static string Sites_LockedUser(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_LockedUser",
                data: data);
        }

        public static string Sites_Ancestors(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Ancestors",
                data: data);
        }

        public static string Sites_SiteMenu(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_SiteMenu",
                data: data);
        }

        public static string Sites_MonitorChangesColumns(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_MonitorChangesColumns",
                data: data);
        }

        public static string Sites_TitleColumns(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_TitleColumns",
                data: data);
        }

        public static string Sites_Export(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Export",
                data: data);
        }

        public static string Orders_ReferenceId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_ReferenceId",
                data: data);
        }

        public static string Orders_ReferenceType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_ReferenceType",
                data: data);
        }

        public static string Orders_OwnerId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_OwnerId",
                data: data);
        }

        public static string Orders_Data(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_Data",
                data: data);
        }

        public static string ExportSettings_ReferenceType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_ReferenceType",
                data: data);
        }

        public static string ExportSettings_ReferenceId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_ReferenceId",
                data: data);
        }

        public static string ExportSettings_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_Title",
                data: data);
        }

        public static string ExportSettings_ExportSettingId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_ExportSettingId",
                data: data);
        }

        public static string ExportSettings_AddHeader(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_AddHeader",
                data: data);
        }

        public static string ExportSettings_ExportColumns(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_ExportColumns",
                data: data);
        }

        public static string Links_DestinationId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_DestinationId",
                data: data);
        }

        public static string Links_SourceId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_SourceId",
                data: data);
        }

        public static string Links_ReferenceType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_ReferenceType",
                data: data);
        }

        public static string Links_SiteId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_SiteId",
                data: data);
        }

        public static string Links_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_Title",
                data: data);
        }

        public static string Links_Subset(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_Subset",
                data: data);
        }

        public static string Links_SiteTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_SiteTitle",
                data: data);
        }

        public static string Binaries_BinaryId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_BinaryId",
                data: data);
        }

        public static string Binaries_TenantId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_TenantId",
                data: data);
        }

        public static string Binaries_ReferenceId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_ReferenceId",
                data: data);
        }

        public static string Binaries_Guid(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Guid",
                data: data);
        }

        public static string Binaries_BinaryType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_BinaryType",
                data: data);
        }

        public static string Binaries_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Title",
                data: data);
        }

        public static string Binaries_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Body",
                data: data);
        }

        public static string Binaries_Bin(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Bin",
                data: data);
        }

        public static string Binaries_Thumbnail(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Thumbnail",
                data: data);
        }

        public static string Binaries_Icon(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Icon",
                data: data);
        }

        public static string Binaries_FileName(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_FileName",
                data: data);
        }

        public static string Binaries_Extension(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Extension",
                data: data);
        }

        public static string Binaries_Size(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Size",
                data: data);
        }

        public static string Binaries_ContentType(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_ContentType",
                data: data);
        }

        public static string Binaries_BinarySettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_BinarySettings",
                data: data);
        }

        public static string Issues_IssueId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_IssueId",
                data: data);
        }

        public static string Issues_StartTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_StartTime",
                data: data);
        }

        public static string Issues_CompletionTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_CompletionTime",
                data: data);
        }

        public static string Issues_WorkValue(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_WorkValue",
                data: data);
        }

        public static string Issues_ProgressRate(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_ProgressRate",
                data: data);
        }

        public static string Issues_RemainingWorkValue(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_RemainingWorkValue",
                data: data);
        }

        public static string Issues_Status(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Status",
                data: data);
        }

        public static string Issues_Manager(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Manager",
                data: data);
        }

        public static string Issues_Owner(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Owner",
                data: data);
        }

        public static string Issues_SiteTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_SiteTitle",
                data: data);
        }

        public static string Results_ResultId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_ResultId",
                data: data);
        }

        public static string Results_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Title",
                data: data);
        }

        public static string Results_Status(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Status",
                data: data);
        }

        public static string Results_Manager(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Manager",
                data: data);
        }

        public static string Results_Owner(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Owner",
                data: data);
        }

        public static string Results_SiteTitle(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_SiteTitle",
                data: data);
        }

        public static string Wikis_WikiId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_WikiId",
                data: data);
        }

        public static string Tenants_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_Ver",
                data: data);
        }

        public static string Tenants_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_Comments",
                data: data);
        }

        public static string Tenants_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_Creator",
                data: data);
        }

        public static string Tenants_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_Updator",
                data: data);
        }

        public static string Tenants_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_CreatedTime",
                data: data);
        }

        public static string Tenants_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_UpdatedTime",
                data: data);
        }

        public static string Tenants_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_VerUp",
                data: data);
        }

        public static string Tenants_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants_Timestamp",
                data: data);
        }

        public static string Demos_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_Ver",
                data: data);
        }

        public static string Demos_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_Comments",
                data: data);
        }

        public static string Demos_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_Creator",
                data: data);
        }

        public static string Demos_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_Updator",
                data: data);
        }

        public static string Demos_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_CreatedTime",
                data: data);
        }

        public static string Demos_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_UpdatedTime",
                data: data);
        }

        public static string Demos_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_VerUp",
                data: data);
        }

        public static string Demos_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos_Timestamp",
                data: data);
        }

        public static string Sessions_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_Ver",
                data: data);
        }

        public static string Sessions_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_Comments",
                data: data);
        }

        public static string Sessions_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_Creator",
                data: data);
        }

        public static string Sessions_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_Updator",
                data: data);
        }

        public static string Sessions_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_CreatedTime",
                data: data);
        }

        public static string Sessions_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_UpdatedTime",
                data: data);
        }

        public static string Sessions_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_VerUp",
                data: data);
        }

        public static string Sessions_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions_Timestamp",
                data: data);
        }

        public static string SysLogs_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Ver",
                data: data);
        }

        public static string SysLogs_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Comments",
                data: data);
        }

        public static string SysLogs_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Creator",
                data: data);
        }

        public static string SysLogs_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Updator",
                data: data);
        }

        public static string SysLogs_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_UpdatedTime",
                data: data);
        }

        public static string SysLogs_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_VerUp",
                data: data);
        }

        public static string SysLogs_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs_Timestamp",
                data: data);
        }

        public static string Statuses_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_Ver",
                data: data);
        }

        public static string Statuses_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_Comments",
                data: data);
        }

        public static string Statuses_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_Creator",
                data: data);
        }

        public static string Statuses_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_Updator",
                data: data);
        }

        public static string Statuses_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_CreatedTime",
                data: data);
        }

        public static string Statuses_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_UpdatedTime",
                data: data);
        }

        public static string Statuses_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_VerUp",
                data: data);
        }

        public static string Statuses_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses_Timestamp",
                data: data);
        }

        public static string ReminderSchedules_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_Ver",
                data: data);
        }

        public static string ReminderSchedules_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_Comments",
                data: data);
        }

        public static string ReminderSchedules_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_Creator",
                data: data);
        }

        public static string ReminderSchedules_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_Updator",
                data: data);
        }

        public static string ReminderSchedules_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_CreatedTime",
                data: data);
        }

        public static string ReminderSchedules_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_UpdatedTime",
                data: data);
        }

        public static string ReminderSchedules_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_VerUp",
                data: data);
        }

        public static string ReminderSchedules_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules_Timestamp",
                data: data);
        }

        public static string Depts_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_Ver",
                data: data);
        }

        public static string Depts_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_Comments",
                data: data);
        }

        public static string Depts_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_Creator",
                data: data);
        }

        public static string Depts_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_Updator",
                data: data);
        }

        public static string Depts_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_CreatedTime",
                data: data);
        }

        public static string Depts_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_UpdatedTime",
                data: data);
        }

        public static string Depts_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_VerUp",
                data: data);
        }

        public static string Depts_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts_Timestamp",
                data: data);
        }

        public static string Groups_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_Ver",
                data: data);
        }

        public static string Groups_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_Comments",
                data: data);
        }

        public static string Groups_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_Creator",
                data: data);
        }

        public static string Groups_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_Updator",
                data: data);
        }

        public static string Groups_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_CreatedTime",
                data: data);
        }

        public static string Groups_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_UpdatedTime",
                data: data);
        }

        public static string Groups_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_VerUp",
                data: data);
        }

        public static string Groups_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups_Timestamp",
                data: data);
        }

        public static string GroupMembers_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_Ver",
                data: data);
        }

        public static string GroupMembers_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_Comments",
                data: data);
        }

        public static string GroupMembers_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_Creator",
                data: data);
        }

        public static string GroupMembers_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_Updator",
                data: data);
        }

        public static string GroupMembers_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_CreatedTime",
                data: data);
        }

        public static string GroupMembers_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_UpdatedTime",
                data: data);
        }

        public static string GroupMembers_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_VerUp",
                data: data);
        }

        public static string GroupMembers_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers_Timestamp",
                data: data);
        }

        public static string Registrations_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Ver",
                data: data);
        }

        public static string Registrations_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Comments",
                data: data);
        }

        public static string Registrations_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Creator",
                data: data);
        }

        public static string Registrations_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Updator",
                data: data);
        }

        public static string Registrations_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_CreatedTime",
                data: data);
        }

        public static string Registrations_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_UpdatedTime",
                data: data);
        }

        public static string Registrations_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_VerUp",
                data: data);
        }

        public static string Registrations_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations_Timestamp",
                data: data);
        }

        public static string Users_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Ver",
                data: data);
        }

        public static string Users_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Comments",
                data: data);
        }

        public static string Users_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Creator",
                data: data);
        }

        public static string Users_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Updator",
                data: data);
        }

        public static string Users_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_CreatedTime",
                data: data);
        }

        public static string Users_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_UpdatedTime",
                data: data);
        }

        public static string Users_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_VerUp",
                data: data);
        }

        public static string Users_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users_Timestamp",
                data: data);
        }

        public static string LoginKeys_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_Ver",
                data: data);
        }

        public static string LoginKeys_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_Comments",
                data: data);
        }

        public static string LoginKeys_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_Creator",
                data: data);
        }

        public static string LoginKeys_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_Updator",
                data: data);
        }

        public static string LoginKeys_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_CreatedTime",
                data: data);
        }

        public static string LoginKeys_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_UpdatedTime",
                data: data);
        }

        public static string LoginKeys_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_VerUp",
                data: data);
        }

        public static string LoginKeys_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys_Timestamp",
                data: data);
        }

        public static string MailAddresses_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_Ver",
                data: data);
        }

        public static string MailAddresses_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_Comments",
                data: data);
        }

        public static string MailAddresses_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_Creator",
                data: data);
        }

        public static string MailAddresses_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_Updator",
                data: data);
        }

        public static string MailAddresses_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_CreatedTime",
                data: data);
        }

        public static string MailAddresses_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_UpdatedTime",
                data: data);
        }

        public static string MailAddresses_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_VerUp",
                data: data);
        }

        public static string MailAddresses_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses_Timestamp",
                data: data);
        }

        public static string Permissions_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_Ver",
                data: data);
        }

        public static string Permissions_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_Comments",
                data: data);
        }

        public static string Permissions_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_Creator",
                data: data);
        }

        public static string Permissions_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_Updator",
                data: data);
        }

        public static string Permissions_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_CreatedTime",
                data: data);
        }

        public static string Permissions_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_UpdatedTime",
                data: data);
        }

        public static string Permissions_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_VerUp",
                data: data);
        }

        public static string Permissions_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions_Timestamp",
                data: data);
        }

        public static string OutgoingMails_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Ver",
                data: data);
        }

        public static string OutgoingMails_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Comments",
                data: data);
        }

        public static string OutgoingMails_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Creator",
                data: data);
        }

        public static string OutgoingMails_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Updator",
                data: data);
        }

        public static string OutgoingMails_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_CreatedTime",
                data: data);
        }

        public static string OutgoingMails_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_UpdatedTime",
                data: data);
        }

        public static string OutgoingMails_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_VerUp",
                data: data);
        }

        public static string OutgoingMails_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails_Timestamp",
                data: data);
        }

        public static string SearchIndexes_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Ver",
                data: data);
        }

        public static string SearchIndexes_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Comments",
                data: data);
        }

        public static string SearchIndexes_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Creator",
                data: data);
        }

        public static string SearchIndexes_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Updator",
                data: data);
        }

        public static string SearchIndexes_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_CreatedTime",
                data: data);
        }

        public static string SearchIndexes_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_UpdatedTime",
                data: data);
        }

        public static string SearchIndexes_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_VerUp",
                data: data);
        }

        public static string SearchIndexes_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes_Timestamp",
                data: data);
        }

        public static string Items_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_Ver",
                data: data);
        }

        public static string Items_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_Comments",
                data: data);
        }

        public static string Items_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_Creator",
                data: data);
        }

        public static string Items_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_Updator",
                data: data);
        }

        public static string Items_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_CreatedTime",
                data: data);
        }

        public static string Items_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_VerUp",
                data: data);
        }

        public static string Items_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items_Timestamp",
                data: data);
        }

        public static string Sites_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_UpdatedTime",
                data: data);
        }

        public static string Sites_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Body",
                data: data);
        }

        public static string Sites_TitleBody(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_TitleBody",
                data: data);
        }

        public static string Sites_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Ver",
                data: data);
        }

        public static string Sites_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Comments",
                data: data);
        }

        public static string Sites_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Creator",
                data: data);
        }

        public static string Sites_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Updator",
                data: data);
        }

        public static string Sites_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_CreatedTime",
                data: data);
        }

        public static string Sites_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_VerUp",
                data: data);
        }

        public static string Sites_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites_Timestamp",
                data: data);
        }

        public static string Orders_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_Ver",
                data: data);
        }

        public static string Orders_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_Comments",
                data: data);
        }

        public static string Orders_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_Creator",
                data: data);
        }

        public static string Orders_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_Updator",
                data: data);
        }

        public static string Orders_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_CreatedTime",
                data: data);
        }

        public static string Orders_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_UpdatedTime",
                data: data);
        }

        public static string Orders_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_VerUp",
                data: data);
        }

        public static string Orders_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders_Timestamp",
                data: data);
        }

        public static string ExportSettings_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_Ver",
                data: data);
        }

        public static string ExportSettings_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_Comments",
                data: data);
        }

        public static string ExportSettings_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_Creator",
                data: data);
        }

        public static string ExportSettings_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_Updator",
                data: data);
        }

        public static string ExportSettings_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_CreatedTime",
                data: data);
        }

        public static string ExportSettings_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_UpdatedTime",
                data: data);
        }

        public static string ExportSettings_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_VerUp",
                data: data);
        }

        public static string ExportSettings_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings_Timestamp",
                data: data);
        }

        public static string Links_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_Ver",
                data: data);
        }

        public static string Links_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_Comments",
                data: data);
        }

        public static string Links_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_Creator",
                data: data);
        }

        public static string Links_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_Updator",
                data: data);
        }

        public static string Links_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_CreatedTime",
                data: data);
        }

        public static string Links_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_UpdatedTime",
                data: data);
        }

        public static string Links_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_VerUp",
                data: data);
        }

        public static string Links_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Links_Timestamp",
                data: data);
        }

        public static string Binaries_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Ver",
                data: data);
        }

        public static string Binaries_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Comments",
                data: data);
        }

        public static string Binaries_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Creator",
                data: data);
        }

        public static string Binaries_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Updator",
                data: data);
        }

        public static string Binaries_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_CreatedTime",
                data: data);
        }

        public static string Binaries_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_UpdatedTime",
                data: data);
        }

        public static string Binaries_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_VerUp",
                data: data);
        }

        public static string Binaries_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries_Timestamp",
                data: data);
        }

        public static string Issues_SiteId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_SiteId",
                data: data);
        }

        public static string Issues_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_UpdatedTime",
                data: data);
        }

        public static string Issues_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Title",
                data: data);
        }

        public static string Issues_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Body",
                data: data);
        }

        public static string Issues_TitleBody(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_TitleBody",
                data: data);
        }

        public static string Issues_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Ver",
                data: data);
        }

        public static string Issues_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Comments",
                data: data);
        }

        public static string Issues_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Creator",
                data: data);
        }

        public static string Issues_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Updator",
                data: data);
        }

        public static string Issues_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_CreatedTime",
                data: data);
        }

        public static string Issues_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_VerUp",
                data: data);
        }

        public static string Issues_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues_Timestamp",
                data: data);
        }

        public static string Results_SiteId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_SiteId",
                data: data);
        }

        public static string Results_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_UpdatedTime",
                data: data);
        }

        public static string Results_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Body",
                data: data);
        }

        public static string Results_TitleBody(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_TitleBody",
                data: data);
        }

        public static string Results_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Ver",
                data: data);
        }

        public static string Results_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Comments",
                data: data);
        }

        public static string Results_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Creator",
                data: data);
        }

        public static string Results_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Updator",
                data: data);
        }

        public static string Results_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_CreatedTime",
                data: data);
        }

        public static string Results_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_VerUp",
                data: data);
        }

        public static string Results_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results_Timestamp",
                data: data);
        }

        public static string Wikis_SiteId(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_SiteId",
                data: data);
        }

        public static string Wikis_UpdatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_UpdatedTime",
                data: data);
        }

        public static string Wikis_Title(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_Title",
                data: data);
        }

        public static string Wikis_Body(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_Body",
                data: data);
        }

        public static string Wikis_TitleBody(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_TitleBody",
                data: data);
        }

        public static string Wikis_Ver(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_Ver",
                data: data);
        }

        public static string Wikis_Comments(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_Comments",
                data: data);
        }

        public static string Wikis_Creator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_Creator",
                data: data);
        }

        public static string Wikis_Updator(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_Updator",
                data: data);
        }

        public static string Wikis_CreatedTime(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_CreatedTime",
                data: data);
        }

        public static string Wikis_VerUp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_VerUp",
                data: data);
        }

        public static string Wikis_Timestamp(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis_Timestamp",
                data: data);
        }

        public static string Tenants(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Tenants",
                data: data);
        }

        public static string Demos(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Demos",
                data: data);
        }

        public static string Sessions(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sessions",
                data: data);
        }

        public static string SysLogs(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SysLogs",
                data: data);
        }

        public static string Statuses(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Statuses",
                data: data);
        }

        public static string ReminderSchedules(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ReminderSchedules",
                data: data);
        }

        public static string Depts(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Depts",
                data: data);
        }

        public static string Groups(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Groups",
                data: data);
        }

        public static string GroupMembers(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "GroupMembers",
                data: data);
        }

        public static string Registrations(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Registrations",
                data: data);
        }

        public static string Users(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Users",
                data: data);
        }

        public static string LoginKeys(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "LoginKeys",
                data: data);
        }

        public static string MailAddresses(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "MailAddresses",
                data: data);
        }

        public static string Permissions(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Permissions",
                data: data);
        }

        public static string OutgoingMails(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "OutgoingMails",
                data: data);
        }

        public static string SearchIndexes(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "SearchIndexes",
                data: data);
        }

        public static string Items(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Items",
                data: data);
        }

        public static string Sites(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Sites",
                data: data);
        }

        public static string Orders(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Orders",
                data: data);
        }

        public static string ExportSettings(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "ExportSettings",
                data: data);
        }

        public static string Binaries(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Binaries",
                data: data);
        }

        public static string Issues(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Issues",
                data: data);
        }

        public static string Results(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Results",
                data: data);
        }

        public static string Wikis(
            Context context,
            params string[] data)
        {
            return Get(
                context: context,
                id: "Wikis",
                data: data);
        }
    }
}
