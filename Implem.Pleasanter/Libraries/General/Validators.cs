using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Libraries.General
{
    public static class Validators
    {
        public static void ValidateMaxLength(
            string columnName,
            decimal? maxLength,
            List<ErrorData> errors,
            string value)
        {
            if (maxLength > 0)
            {
                int length;
                switch (Parameters.Validation.MaxLengthCountType)
                {
                    case "Regex":
                        length = value?.Length + Regex.Replace(
                            value,
                            $"[{Parameters.Validation.SingleSyteCharactorRegexServer}]",
                            string.Empty)?.Length ?? 0;
                        break;
                    default:
                        length = value?.Length ?? 0;
                        break;
                }
                if (length > maxLength)
                {
                    errors.Add(new ErrorData(
                        type: Error.Types.TooLongText,
                        columnName: columnName,
                        data: maxLength?.ToLong().ToStr()));
                }
            }
        }

        public static void ValidateRegex(
            string columnName,
            string serverRegexValidation,
            string regexValidationMessage,
            List<ErrorData> errors,
            string value)
        {
            if (!serverRegexValidation.IsNullOrEmpty())
            {
                try
                {
                    if (!Regex.IsMatch(value, serverRegexValidation))
                    {
                        errors.Add(new ErrorData(
                            type: Error.Types.NotMatchRegex,
                            columnName: columnName,
                            data: regexValidationMessage));
                    }
                }
                catch (ArgumentException)
                {
                    errors.Add(new ErrorData(
                        type: Error.Types.NotMatchRegex,
                        columnName: columnName,
                        data: regexValidationMessage));
                }
            }
        }

        public static ErrorData ValidateApi(
            Context context,
            bool serverScript)
        {
            if (!serverScript
                && (!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.InvalidRequest,
                    sysLogsStatus: 403);
            }
            if (context.InvalidJsonData)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.InvalidJsonData,
                    sysLogsStatus: 400);
            }

            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData ValidateEnvironment(
            Context context,
            bool api,
            IEnumerable<int> deniedEnvironment)
        {
            if (deniedEnvironment.Contains(Parameters.Environment()))
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription()
                );
            }

            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// GUID形式の文字列を検証します（32文字の16進数）
        /// </summary>
        public static bool IsValidGuid(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, "^[0-9A-Fa-f]{32}$");
        }
    }
}