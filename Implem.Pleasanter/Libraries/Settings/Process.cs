﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class Process : ISettingListItem
    {
        public enum ScreenTypes
        {
            New = 10,
            Edit = 20
        }

        public enum ActionTypes
        {
            Save = 0,
            PostBack = 10,
            None = 90
        }

        public enum ValidationTypes
        {
            Merge = 0,
            Replacement = 10,
            None = 90
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ScreenTypes? ScreenType { get; set; }
        public int CurrentStatus { get; set; }
        public int ChangedStatus { get; set; }
        public string Description { get; set; }
        public string Tooltip { get; set; }
        public string ConfirmationMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string OnClick { get; set; }
        public ActionTypes? ActionType { get; set; }
        public ValidationTypes? ValidationType { get; set; }
        public SettingList<ValidateInput> ValidateInputs { get; set; }
        public List<int> Depts { get; set; }
        public List<int> Groups { get; set; }
        public List<int> Users { get; set; }
        public View View { get; set; }
        public string ErrorMessage { get; set; }
        public SettingList<DataChange> DataChanges { get; set; }
        public SettingList<Notification> Notifications { get; set; }
        [NonSerialized]
        public bool MatchConditions;

        public Process()
        {
        }

        public Process (
            int id,
            string name,
            string displayName,
            ScreenTypes? screenType,
            int currentStatus,
            int changedStatus,
            string description,
            string tooltip,
            string confirmationMessage,
            string successMessage,
            string onClick,
            ActionTypes? actionType,
            ValidationTypes? validationType,
            SettingList<ValidateInput> validateInputs,
            List<Permission> permissions,
            View view,
            string errorMessage,
            SettingList<DataChange> dataChanges,
            SettingList<Notification> notifications)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
            ScreenType = screenType;
            CurrentStatus = currentStatus;
            ChangedStatus = changedStatus;
            Description = description;
            Tooltip = tooltip;
            ConfirmationMessage = confirmationMessage;
            SuccessMessage = successMessage;
            OnClick = onClick;
            ActionType = actionType;
            ValidationType = validationType;
            ValidateInputs = validateInputs;
            SetPermissions(permissions: permissions);
            View = view;
            ErrorMessage = errorMessage;
            DataChanges = dataChanges;
            Notifications = notifications;
        }

        public void Update(
            string name,
            string displayName,
            ScreenTypes? screenType,
            int currentStatus,
            int changedStatus,
            string description,
            string tooltip,
            string confirmationMessage,
            string successMessage,
            string onClick,
            ActionTypes? actionType,
            ValidationTypes? validationType,
            SettingList<ValidateInput> validateInputs,
            List<Permission> permissions,
            View view,
            string errorMessage,
            SettingList<DataChange> dataChanges,
            SettingList<Notification> notifications)
        {
            Name = name;
            DisplayName = displayName;
            ScreenType = screenType;
            CurrentStatus = currentStatus;
            ChangedStatus = changedStatus;
            Description = description;
            Tooltip = tooltip;
            ConfirmationMessage = confirmationMessage;
            SuccessMessage = successMessage;
            OnClick = onClick;
            ActionType = actionType;
            ValidationType = validationType;
            ValidateInputs = validateInputs;
            SetPermissions(permissions: permissions);
            View = view;
            ErrorMessage = errorMessage;
            DataChanges = dataChanges;
            Notifications = notifications;
        }

        private void SetPermissions(List<Permission> permissions)
        {
            Depts?.Clear();
            Groups?.Clear();
            Users?.Clear();
            foreach (var permission in permissions)
            {
                switch (permission.Name)
                {
                    case "Dept":
                        if (Depts == null)
                        {
                            Depts = new List<int>();
                        }
                        if (!Depts.Contains(permission.Id))
                        {
                            Depts.Add(permission.Id);
                        }
                        break;
                    case "Group":
                        if (Groups == null)
                        {
                            Groups = new List<int>();
                        }
                        if (!Groups.Contains(permission.Id))
                        {
                            Groups.Add(permission.Id);
                        }
                        break;
                    case "User":
                        if (Users == null)
                        {
                            Users = new List<int>();
                        }
                        if (!Users.Contains(permission.Id))
                        {
                            Users.Add(permission.Id);
                        }
                        break;
                }
            }
        }

        public List<Permission> GetPermissions(SiteSettings ss)
        {
            var permissions = new List<Permission>();
            Depts?.ForEach(deptId => permissions.Add(new Permission(
                ss: ss,
                name: "Dept",
                id: deptId)));
            Groups?.ForEach(groupId => permissions.Add(new Permission(
                ss: ss,
                name: "Group",
                id: groupId)));
            Users?.ForEach(userId => permissions.Add(new Permission(
                ss: ss,
                name: "User",
                id: userId)));
            return permissions;
        }

        public Process GetRecordingData(
            Context context,
            SiteSettings ss)
        {
            var process = new Process();
            process.Id = Id;
            process.Name = Name;
            if (!DisplayName.IsNullOrEmpty())
            {
                process.DisplayName = DisplayName;
            }
            if (ScreenType != ScreenTypes.Edit)
            {
                process.ScreenType = ScreenType;
            }
            process.CurrentStatus = CurrentStatus;
            process.ChangedStatus = ChangedStatus;
            if (!Description.IsNullOrEmpty())
            {
                process.Description = Description;
            }
            if (!Tooltip.IsNullOrEmpty())
            {
                process.Tooltip = Tooltip;
            }
            if (!ConfirmationMessage.IsNullOrEmpty())
            {
                process.ConfirmationMessage = ConfirmationMessage;
            }
            if (!SuccessMessage.IsNullOrEmpty())
            {
                process.SuccessMessage = SuccessMessage;
            }
            if (!OnClick.IsNullOrEmpty())
            {
                process.OnClick = OnClick;
            }
            if (ActionType != ActionTypes.Save)
            {
                process.ActionType = ActionType;
            }
            if (ValidationType != ValidationTypes.Merge)
            {
                process.ValidationType = ValidationType;
            }
            ValidateInputs?.ForEach(validateInput =>
            {
                if (process.ValidateInputs == null)
                {
                    process.ValidateInputs = new SettingList<ValidateInput>();
                }
                process.ValidateInputs.Add(validateInput.GetRecordingData(ss: ss));
            });
            if (Depts?.Any() == true)
            {
                process.Depts = Depts;
            }
            if (Groups?.Any() == true)
            {
                process.Groups = Groups;
            }
            if (Users?.Any() == true)
            {
                process.Users = Users;
            }
            process.View = View?.GetRecordingData(
                context: context,
                ss: ss);
            if (!ErrorMessage.IsNullOrEmpty())
            {
                process.ErrorMessage = ErrorMessage;
            }
            DataChanges?.ForEach(dataChange =>
            {
                if (process.DataChanges == null)
                {
                    process.DataChanges = new SettingList<DataChange>();
                }
                process.DataChanges.Add(dataChange.GetRecordingData(
                    context: context,
                    ss: ss));
            });
            Notifications?.ForEach(notification =>
            {
                if (process.Notifications == null)
                {
                    process.Notifications = new SettingList<Notification>();
                }
                process.Notifications.Add(notification.GetRecordingData(
                    context: context,
                    ss: ss));
            });
            return process;
        }

        public string GetDisplayName()
        {
            var displayName = Strings.CoalesceEmpty(
                DisplayName,
                Name);
            return displayName;
        }

        public bool Accessable(Context context)
        {
            if (context.HasPrivilege)
            {
                return true;
            }
            if (Depts?.Any() != true
                && Groups?.Any() != true
                && Users?.Any() != true)
            {
                return true;
            }
            if (Depts?.Contains(context.DeptId) == true)
            {
                return true;
            }
            if (Groups?.Any(groupId => context.Groups.Contains(groupId)) == true)
            {
                return true;
            }
            if (Users?.Contains(context.UserId) == true)
            {
                return true;
            }
            return false;
        }

        public Message GetSuccessMessage(Context context)
        {
            var displayName = GetDisplayName();
            var text = Strings.CoalesceEmpty(
                SuccessMessage,
                Displays.ProcessExecuted(
                    context: context,
                    data: new string[] { displayName }));
            var message = new Message()
            {
                Text = text,
                Css = "alert-success"
            };
            return message;
        }

        public Message GetErrorMessage(Context context)
        {
            var text = Strings.CoalesceEmpty(
                ErrorMessage,
                Displays.Incorrect(context: context));
            var message = new Message()
            {
                Text = text,
                Css = "alert-error"
            };
            return message;
        }
    }
}