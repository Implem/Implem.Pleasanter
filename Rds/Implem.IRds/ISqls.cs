using System.Collections.Generic;

namespace Implem.IRds
{
    public interface ISqls
    {
        string TrueString { get; }
        string FalseString { get; }
        string IsNotTrue { get; }
        string CurrentDateTime { get; }
        string Like { get; }
        string NotLike { get; }
        string LikeWithEscape { get; }
        string NotLikeWithEscape { get; }
        string Escape { get; }
        string EscapeValue(string value);
        string IsNull { get; }
        string WhereLikeTemplateForward { get; }
        string WhereLikeTemplate { get; }
        string GenerateIdentity { get; }
        object DateTimeValue(object value);
        string BooleanString(string value);
        string IntegerColumnLike(string tableName, string columnName);
        string DateAddDay(int day, string columnBracket);
        string DateAddHour(int hour, string columnBracket);
        string DateGroupYearly { get; }
        string DateGroupMonthly { get; }
        string DateGroupWeeklyPart { get; }
        string DateGroupWeekly { get; }
        string DateGroupDaily { get; }
        string GetPermissions { get; }
        string GetPermissionsById { get; }
        string GetGroup { get; }
        string GetEnabledGroup { get; }
        string PermissionsWhere { get; }
        string SiteDeptWhere { get; }
        string SiteGroupWhere { get; }
        string SiteUserWhere { get; }
        string SitePermissionsWhere { get; }
        public string IntegratedSitesPermissionsWhere(string tableName, List<long> sites);
        string UpsertBinary { get; }
        string GetBinaryHash(string algorithm);
    }
}
