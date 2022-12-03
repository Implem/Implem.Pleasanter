﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class ValidateInput : ISettingListItem
    {
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public bool? Required { get; set; }
        public string ClientRegexValidation { get; set; }
        public string ServerRegexValidation { get; set; }
        public string RegexValidationMessage { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public ValidateInput()
        {
        }

        public ValidateInput GetRecordingData(SiteSettings ss)
        {
            var columnDefinition = ss.ColumnDefinitionHash.Get(ColumnName);
            var processValidateInput = new ValidateInput();
            processValidateInput.Id = Id;
            processValidateInput.ColumnName = ColumnName;
            if (Required == true)
            {
                processValidateInput.Required = Required;
            }
            if (!ClientRegexValidation.IsNullOrEmpty())
            {
                processValidateInput.ClientRegexValidation = ClientRegexValidation;
            }
            if (!ServerRegexValidation.IsNullOrEmpty())
            {
                processValidateInput.ServerRegexValidation = ServerRegexValidation;
            }
            if (!RegexValidationMessage.IsNullOrEmpty())
            {
                processValidateInput.RegexValidationMessage = RegexValidationMessage;
            }
            if (Min != null && Min != ss.DefaultMin(columnDefinition))
            {
                processValidateInput.Min = Min;
            }
            if (Max != null && Max != ss.DefaultMax(columnDefinition))
            {
                processValidateInput.Max = Max;
            }
            return processValidateInput;
        }

        /// <summary>
        /// 入力必須以外の入力検証が指定されていないかチェック
        /// </summary>
        /// <returns></returns>
        public bool HasNotInputValidation()
        {
            return ClientRegexValidation.IsNullOrEmpty()
                && ServerRegexValidation.IsNullOrEmpty()
                && Min == null
                && Max == null;
        }
    }
}