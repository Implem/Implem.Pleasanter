using Implem.Libraries.Utilities;
using System;
namespace Implem.DefinitionAccessor
{
    public static class Parameters
    {
        public static string Get(string name)
        {
            var local = "Implem_" + Environments.ServiceName + "_" + name;
            var global = "Implem_" + name;
            if (!Environment.GetEnvironmentVariable(local).IsNullOrEmpty())
            {
                return Environment.GetEnvironmentVariable(local);
            }
            else if (!Environment.GetEnvironmentVariable(global).IsNullOrEmpty())
            {
                return Environment.GetEnvironmentVariable(global);
            }
            else
            {
                return null;
            }
        }

        public static string HtmlTitle
        {
            get
            {
                var environment = Get("HtmlTitle");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.HtmlTitle.String.ToString();
            }
        }

        public static string HtmlHeadKeywords
        {
            get
            {
                var environment = Get("HtmlHeadKeywords");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.HtmlHeadKeywords.String.ToString();
            }
        }

        public static string HtmlHeadDescription
        {
            get
            {
                var environment = Get("HtmlHeadDescription");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.HtmlHeadDescription.String.ToString();
            }
        }

        public static string HtmlHeadAuther
        {
            get
            {
                var environment = Get("HtmlHeadAuther");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.HtmlHeadAuther.String.ToString();
            }
        }

        public static string HtmlHeadViewport
        {
            get
            {
                var environment = Get("HtmlHeadViewport");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.HtmlHeadViewport.String.ToString();
            }
        }

        public static string HtmlLogoText
        {
            get
            {
                var environment = Get("HtmlLogoText");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.HtmlLogoText.String.ToString();
            }
        }

        public static string HtmlCopyright
        {
            get
            {
                var environment = Get("HtmlCopyright");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.HtmlCopyright.String.ToString();
            }
        }

        public static string HtmlCopyrightUrl
        {
            get
            {
                var environment = Get("HtmlCopyrightUrl");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.HtmlCopyrightUrl.String.ToString();
            }
        }

        public static string TimeZoneDefault
        {
            get
            {
                var environment = Get("TimeZoneDefault");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.TimeZoneDefault.String.ToString();
            }
        }

        public static int DataImport
        {
            get
            {
                var environment = Get("DataImport");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.DataImport.Int.ToInt();
            }
        }

        public static int RequireHttps
        {
            get
            {
                var environment = Get("RequireHttps");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.RequireHttps.Int.ToInt();
            }
        }

        public static string AuthenticationProvider
        {
            get
            {
                var environment = Get("AuthenticationProvider");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.AuthenticationProvider.String.ToString();
            }
        }

        public static string LdapSearchRoot
        {
            get
            {
                var environment = Get("LdapSearchRoot");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.LdapSearchRoot.String.ToString();
            }
        }

        public static string LdapSearchProperty
        {
            get
            {
                var environment = Get("LdapSearchProperty");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.LdapSearchProperty.String.ToString();
            }
        }

        public static int LdapTenantId
        {
            get
            {
                var environment = Get("LdapTenantId");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.LdapTenantId.Int.ToInt();
            }
        }

        public static string LdapDeptCode
        {
            get
            {
                var environment = Get("LdapDeptCode");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.LdapDeptCode.String.ToString();
            }
        }

        public static string LdapDeptName
        {
            get
            {
                var environment = Get("LdapDeptName");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.LdapDeptName.String.ToString();
            }
        }

        public static string LdapUserCode
        {
            get
            {
                var environment = Get("LdapUserCode");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.LdapUserCode.String.ToString();
            }
        }

        public static string LdapFirstName
        {
            get
            {
                var environment = Get("LdapFirstName");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.LdapFirstName.String.ToString();
            }
        }

        public static string LdapLastName
        {
            get
            {
                var environment = Get("LdapLastName");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.LdapLastName.String.ToString();
            }
        }

        public static string LdapMailAddress
        {
            get
            {
                var environment = Get("LdapMailAddress");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.LdapMailAddress.String.ToString();
            }
        }

        public static int LimitWarning1
        {
            get
            {
                var environment = Get("LimitWarning1");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.LimitWarning1.Int.ToInt();
            }
        }

        public static int LimitWarning2
        {
            get
            {
                var environment = Get("LimitWarning2");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.LimitWarning2.Int.ToInt();
            }
        }

        public static int LimitWarning3
        {
            get
            {
                var environment = Get("LimitWarning3");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.LimitWarning3.Int.ToInt();
            }
        }

        public static int SqlCommandTimeOut
        {
            get
            {
                var environment = Get("SqlCommandTimeOut");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.SqlCommandTimeOut.Int.ToInt();
            }
        }

        public static int SqlAzureRetryCount
        {
            get
            {
                var environment = Get("SqlAzureRetryCount");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.SqlAzureRetryCount.Int.ToInt();
            }
        }

        public static int SqlAzureRetryInterval
        {
            get
            {
                var environment = Get("SqlAzureRetryInterval");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.SqlAzureRetryInterval.Int.ToInt();
            }
        }

        public static int DeleteTempOldThan
        {
            get
            {
                var environment = Get("DeleteTempOldThan");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.DeleteTempOldThan.Int.ToInt();
            }
        }

        public static int DeleteHistoriesOldThan
        {
            get
            {
                var environment = Get("DeleteHistoriesOldThan");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.DeleteHistoriesOldThan.Int.ToInt();
            }
        }

        public static int NearDeadlineBeforeDays
        {
            get
            {
                var environment = Get("NearDeadlineBeforeDays");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.NearDeadlineBeforeDays.Int.ToInt();
            }
        }

        public static int NearDeadlineBeforeDaysMin
        {
            get
            {
                var environment = Get("NearDeadlineBeforeDaysMin");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.NearDeadlineBeforeDaysMin.Int.ToInt();
            }
        }

        public static int NearDeadlineBeforeDaysMax
        {
            get
            {
                var environment = Get("NearDeadlineBeforeDaysMax");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.NearDeadlineBeforeDaysMax.Int.ToInt();
            }
        }

        public static int NearDeadlineAfterDays
        {
            get
            {
                var environment = Get("NearDeadlineAfterDays");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.NearDeadlineAfterDays.Int.ToInt();
            }
        }

        public static int NearDeadlineAfterDaysMin
        {
            get
            {
                var environment = Get("NearDeadlineAfterDaysMin");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.NearDeadlineAfterDaysMin.Int.ToInt();
            }
        }

        public static int NearDeadlineAfterDaysMax
        {
            get
            {
                var environment = Get("NearDeadlineAfterDaysMax");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.NearDeadlineAfterDaysMax.Int.ToInt();
            }
        }

        public static int GridPageSize
        {
            get
            {
                var environment = Get("GridPageSize");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.GridPageSize.Int.ToInt();
            }
        }

        public static int GridPageSizeMin
        {
            get
            {
                var environment = Get("GridPageSizeMin");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.GridPageSizeMin.Int.ToInt();
            }
        }

        public static int GridPageSizeMax
        {
            get
            {
                var environment = Get("GridPageSizeMax");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.GridPageSizeMax.Int.ToInt();
            }
        }

        public static string SolutionBackupPath
        {
            get
            {
                var environment = Get("SolutionBackupPath");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.SolutionBackupPath.String.ToString();
            }
        }

        public static string SolutionBackupExcludeDirectories
        {
            get
            {
                var environment = Get("SolutionBackupExcludeDirectories");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.SolutionBackupExcludeDirectories.String.ToString();
            }
        }

        public static string InternalMailDomain
        {
            get
            {
                var environment = Get("InternalMailDomain");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.InternalMailDomain.String.ToString();
            }
        }

        public static string BinaryStorageProvider
        {
            get
            {
                var environment = Get("BinaryStorageProvider");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.BinaryStorageProvider.String.ToString();
            }
        }

        public static string SmtpProvider
        {
            get
            {
                var environment = Get("SmtpProvider");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.SmtpProvider.String.ToString();
            }
        }

        public static string SmtpHost
        {
            get
            {
                var environment = Get("SmtpHost");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.SmtpHost.String.ToString();
            }
        }

        public static int SmtpPort
        {
            get
            {
                var environment = Get("SmtpPort");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.SmtpPort.Int.ToInt();
            }
        }

        public static string SendGridSmtpUser
        {
            get
            {
                var environment = Get("SendGridSmtpUser");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.SendGridSmtpUser.String.ToString();
            }
        }

        public static string SendGridSmtpPassword
        {
            get
            {
                var environment = Get("SendGridSmtpPassword");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.SendGridSmtpPassword.String.ToString();
            }
        }

        public static int SizeToUseTextArea
        {
            get
            {
                var environment = Get("SizeToUseTextArea");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.SizeToUseTextArea.Int.ToInt();
            }
        }

        public static string ProjectModelRequire
        {
            get
            {
                var environment = Get("ProjectModelRequire");
                return !environment.IsNullOrEmpty()
                    ? environment.ToString()
                    : Def.ParameterTable.ProjectModelRequire.String.ToString();
            }
        }

        public static int CompletionCode
        {
            get
            {
                var environment = Get("CompletionCode");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.CompletionCode.Int.ToInt();
            }
        }

        public static int WorkValueHeight
        {
            get
            {
                var environment = Get("WorkValueHeight");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.WorkValueHeight.Int.ToInt();
            }
        }

        public static int WorkValueTextTop
        {
            get
            {
                var environment = Get("WorkValueTextTop");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.WorkValueTextTop.Int.ToInt();
            }
        }

        public static int ProgressRateWidth
        {
            get
            {
                var environment = Get("ProgressRateWidth");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.ProgressRateWidth.Int.ToInt();
            }
        }

        public static int ProgressRateHeight
        {
            get
            {
                var environment = Get("ProgressRateHeight");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.ProgressRateHeight.Int.ToInt();
            }
        }

        public static int ProgressRateItemHeight
        {
            get
            {
                var environment = Get("ProgressRateItemHeight");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.ProgressRateItemHeight.Int.ToInt();
            }
        }

        public static int ProgressRateTextTop
        {
            get
            {
                var environment = Get("ProgressRateTextTop");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.ProgressRateTextTop.Int.ToInt();
            }
        }

        public static int GanttItemMaxHeight
        {
            get
            {
                var environment = Get("GanttItemMaxHeight");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.GanttItemMaxHeight.Int.ToInt();
            }
        }

        public static int GanttItemMinHeight
        {
            get
            {
                var environment = Get("GanttItemMinHeight");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.GanttItemMinHeight.Int.ToInt();
            }
        }

        public static int ImageSizeRegular
        {
            get
            {
                var environment = Get("ImageSizeRegular");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.ImageSizeRegular.Int.ToInt();
            }
        }

        public static int ImageSizeThumbnail
        {
            get
            {
                var environment = Get("ImageSizeThumbnail");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.ImageSizeThumbnail.Int.ToInt();
            }
        }

        public static int ImageSizeIcon
        {
            get
            {
                var environment = Get("ImageSizeIcon");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.ImageSizeIcon.Int.ToInt();
            }
        }

        public static decimal SearchConcordanceRate
        {
            get
            {
                var environment = Get("SearchConcordanceRate");
                return !environment.IsNullOrEmpty()
                    ? environment.ToDecimal()
                    : Def.ParameterTable.SearchConcordanceRate.Decimal.ToDecimal();
            }
        }

        public static int SearchPageSize
        {
            get
            {
                var environment = Get("SearchPageSize");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.SearchPageSize.Int.ToInt();
            }
        }

        public static int AdminTasksDoSpan
        {
            get
            {
                var environment = Get("AdminTasksDoSpan");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.AdminTasksDoSpan.Int.ToInt();
            }
        }

        public static int SeparateMax
        {
            get
            {
                var environment = Get("SeparateMax");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.SeparateMax.Int.ToInt();
            }
        }

        public static int SeparateMin
        {
            get
            {
                var environment = Get("SeparateMin");
                return !environment.IsNullOrEmpty()
                    ? environment.ToInt()
                    : Def.ParameterTable.SeparateMin.Int.ToInt();
            }
        }

        public static DateTime MinTime
        {
            get
            {
                var environment = Get("MinTime");
                return !environment.IsNullOrEmpty()
                    ? environment.ToDateTime()
                    : Def.ParameterTable.MinTime.DateTime.ToDateTime();
            }
        }
    }
}
