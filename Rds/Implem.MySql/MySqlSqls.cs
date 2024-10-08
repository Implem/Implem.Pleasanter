using Implem.IRds;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.MySql
{
    internal class MySqlSqls : ISqls
    {
        public string TrueString { get; } = "1";

        public string FalseString { get; } = "0";

        public string IsNotTrue { get; } = " <> 1 ";

        public string CurrentDateTime { get; } = " CURRENT_TIMESTAMP(3) ";

        public string Like { get; } = " like ";

        public string NotLike { get; } = " not like ";

        public string LikeWithEscape { get; } = " like {0} escape '|'";

        public string NotLikeWithEscape { get; } = " not like {0} escape '|'";

        public string Escape { get; } = " escape '|'";

        public string EscapeValue(string value)
        {
            return value?
                .Replace("|", "||")
                .Replace("_", "|_")
                .Replace("%", "|%");
        }

        public string IsNull { get; } = "ifnull";

        public string WhereLikeTemplateForward { get; } = "'%' || ";

        public string WhereLikeTemplate { get; } = "#ParamCount#_#CommandCount# || '%'";

        public string GenerateIdentity { get; } = string.Empty;

        public object DateTimeValue(object value)
        {
            return value;
        }

        public string BooleanString(string bit)
        {
            return bit == "1" ? TrueString : FalseString;
        }

        public string IntegerColumnLike(string tableName, string columnName)
        {
            return $@"(cast(""{tableName}"".""{columnName}"" as char) like ";
        }

        public string DateAddDay(int day, string columnBracket)
        {
            return $"date_add({columnBracket},interval {day} day)";
        }

        public string DateAddHour(int hour, string columnBracket)
        {
            return $"date_add({columnBracket},interval {hour} hour)";
        }

        public string DateGroupYearly { get; } = "date_format({0}, '%Y')";

        public string DateGroupMonthly { get; } = "date_format({0}, '%Y/%m')";

        public string DateGroupWeeklyPart { get; } = "case weekday({0}) when 6 then date_add({0},interval -6 day) else date_add({0},interval (0-weekday({0})) day) end";

        public string DateGroupWeekly { get; } = "yearweek({0}, 3)";

        public string DateGroupDaily { get; } = "date_format({0}, '%Y/%m/%d')";

        public string GetPermissions { get; } = @"
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Depts"" on ""Permissions"".""DeptId""=""Depts"".""DeptId""
            where ""Sites"".""TenantId""=@ipT
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""=0
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Sites"".""TenantId""=@ipT
                and ""Groups"".""Disabled""=0
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""=0
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Sites"".""TenantId""=@ipT
                and ""Groups"".""Disabled""=0
                and ""Users"".""UserId""=@ipU
                and ""Users"".""Disabled""=0
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
            where ""Sites"".""TenantId""=@ipT
                and ""Permissions"".""UserId"" > 0
                and ""Permissions"".""UserId""=@ipU
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
            where ""Sites"".""TenantId""=@ipT
                and ""Permissions"".""UserId""=-1";

        public string GetPermissionsById { get; } = @"
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
                inner join ""Depts"" on ""Permissions"".""DeptId""=""Depts"".""DeptId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""=0
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Groups"".""Disabled""=0
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""=0
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Groups"".""Disabled""=0
                and ""Users"".""UserId""=@ipU
                and ""Users"".""Disabled""=0
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Permissions"".""UserId"" > 0
                and ""Permissions"".""UserId""=@ipU
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@ipT
                and ""Permissions"".""UserId""=-1;";

        public string GetGroup { get; } = @"
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Depts"".""TenantId""=@ipT
                and ""Depts"".""DeptId""=@ipD
            union all
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Users"".""TenantId""=@ipT
                and ""Users"".""UserId""=@ipU;";

        public string GetEnabledGroup { get; } = @"
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Groups"".""Disabled""=0
                and ""Depts"".""TenantId""=@ipT
                and ""Depts"".""DeptId""=@ipD
                and ""Depts"".""Disabled""=0
            union all
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Groups"".""Disabled""=0
                and ""Users"".""TenantId""=@ipT
                and ""Users"".""UserId""=@ipU;";

        public string PermissionsWhere { get; } = @"
            (
                exists
                (
                    select ""Depts"".""DeptId"" as ""Id""
                    from ""Depts""
                    where ""Depts"".""TenantId""=@ipT
                        and ""Depts"".""DeptId""=@ipD
                        and ""Depts"".""Disabled""=0
                        and ""Permissions"".""DeptId""=""Depts"".""DeptId""
                        and @ipD<>0
                    union all
                    select ""Groups"".""GroupId"" as ""Id""
                    from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                    where ""Groups"".""TenantId""=@ipT
                        and ""Groups"".""Disabled""=0
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and exists
                        (
                            select ""DeptId""
                            from ""Depts""
                            where ""Depts"".""TenantId""=@ipT
                                and ""Depts"".""DeptId""=@ipD
                                and ""Depts"".""Disabled""=0
                                and ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
                                and @ipD<>0
                        )
                    union all
                    select ""Groups"".""GroupId"" as ""Id""
                    from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                    where ""Groups"".""TenantId""=@ipT
                        and ""Groups"".""Disabled""=0
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and ""GroupMembers"".""UserId""=@ipU
                        and @ipU<>0
                    union all
                    select ""P"".""UserId"" as ""Id""
                    from ""Permissions"" as ""P""
                    where ""P"".""ReferenceId""=""Permissions"".""ReferenceId""
                        and ""P"".""UserId""=""Permissions"".""UserId""
                        and ""P"".""UserId""=@ipU
                        and @ipU<>0
                    union all
                    select ""P"".""UserId"" as ""Id""
                    from ""Permissions"" as ""P""
                    where ""P"".""ReferenceId""=""Permissions"".""ReferenceId""
                        and ""P"".""UserId""=-1
                )
            )";

        public string SiteDeptWhere { get; } = @"
            (
                exists
                (
                    select *
                    from ""Permissions""
                        left outer join ""Depts"" on ""Permissions"".""DeptId""=""Depts"".""DeptId""
                        left outer join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        left outer join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        left outer join ""Depts"" as ""GroupMemberDepts"" on ""GroupMembers"".""DeptId""=""GroupMemberDepts"".""DeptId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and
                        (
                            (
                                ""Depts"".""Disabled""=0
                                and ""Depts"".""DeptId""=""Depts"".""DeptId""
                            )
                            or 
                            (
                                ""Groups"".""Disabled""=0 and 
                                (
                                    (
                                        ""GroupMemberDepts"".""Disabled""=0
                                        and ""GroupMemberDepts"".""DeptId""=""Depts"".""DeptId""
                                    )
                                )
                            )
                        )
                )
            )";

        public string SiteGroupWhere { get; } = @"
            (
                exists
                (
                    select *
                    from ""Permissions""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""Groups"".""Disabled""=0
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and ""Groups"".""GroupId"">0
                )
            )";

        public string SiteUserWhere { get; } = @"
            (
                ""Users"".""UserId"" in
                (
                    select ""Users"".""UserId""
                    from ""Permissions""
                        inner join ""Depts"" as ""PermissionDepts"" on ""Permissions"".""DeptId""=""PermissionDepts"".""DeptId""
                        inner join ""Users"" as ""PermissionUsers"" on ""PermissionDepts"".""DeptId""=""PermissionUsers"".""DeptId""
                        inner join ""Users"" on ""PermissionUsers"".""UserId"" = ""Users"".""UserId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""PermissionDepts"".""Disabled""=0
                        and ""PermissionUsers"".""Disabled""=0
                    union all
                    select ""Users"".""UserId""
                    from ""Permissions""
                        inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        inner join ""Depts"" as ""GroupMemberDepts"" on ""GroupMembers"".""DeptId""=""GroupMemberDepts"".""DeptId""
                        inner join ""Users"" as ""GroupMemberUsers"" on ""GroupMemberDepts"".""DeptId""=""GroupMemberUsers"".""DeptId""
                        inner join ""Users"" on ""GroupMemberUsers"".""UserId"" = ""Users"".""UserId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""Groups"".""Disabled""=0
                        and ""GroupMemberDepts"".""Disabled""=0
                        and ""GroupMemberUsers"".""Disabled""=0
                    union all
                    select ""Users"".""UserId""
                    from ""Permissions""
                        inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        inner join ""Users"" as ""GroupMemberUsers"" on ""GroupMembers"".""UserId""=""GroupMemberUsers"".""UserId""
                        inner join ""Users"" on ""GroupMemberUsers"".""UserId"" = ""Users"".""UserId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""Groups"".""Disabled""=0
                        and ""GroupMemberUsers"".""Disabled""=0
                    union all
                    select ""Users"".""UserId""
                    from ""Permissions""
                        inner join ""Users"" as ""PermissionUsers"" on ""Permissions"".""UserId""=""PermissionUsers"".""UserId""
                        inner join ""Users"" on ""Users"".""Disabled""=0
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""PermissionUsers"".""UserId""=""Users"".""UserId""
                        and ""PermissionUsers"".""Disabled""=0
                )
            )";

        public string SitePermissionsWhere { get; } = @"
            (
                exists
                (
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""Permissions"".""UserId""=-1
                )
            )";

        public string IntegratedSitesPermissionsWhere(string tableName, List<long> sites)
        {
            return $@"
                ""{tableName}_Items"".""SiteId"" in ({sites.Join()})
                and exists(
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                    where ""Permissions"".""ReferenceId""=
                        (
                            select ""Sites"".""InheritPermission""
                            from ""Sites""
                            where ""Sites"".""SiteId""=""{tableName}_Items"".""SiteId""
                        )
                        and ""Permissions"".""PermissionType"" & 1 = 1
                        and {PermissionsWhere}
                    union
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                    where ""Permissions"".""ReferenceId""=""{tableName}_Items"".""ReferenceId""
                        and ""Permissions"".""PermissionType"" & 1 = 1
                        and {PermissionsWhere}
                )";
        }

        public string UpsertBinary { get; } = @"
            update ""Binaries""
            set
                ""Bin"" =
                    (
                    select ""subbin"".""Bin""
                    from
                        (
                        select ""Bin""
                        from ""Binaries""
                        where ""TenantId"" = @ipT
                            and ""Guid"" = @Guid
                            and ""BinaryType"" = @BinaryType
                        )
                        as ""subbin""
                    ) || @Bin
                ,""Updator"" = @ipU
                ,""UpdatedTime"" = current_timestamp(3)
            where ""Binaries"".""TenantId"" = @ipT
                and ""Binaries"".""Guid"" = @Guid
                and ""Binaries"".""BinaryType"" = @BinaryType;
            insert into ""Binaries""
            (
                ""TenantId""
                ,""ReferenceId""
                ,""Guid""
                ,""Ver""
                ,""BinaryType""
                ,""Title""
                ,""Bin""
                ,""FileName""
                ,""Creator""
                ,""Updator""
                ,""CreatedTime""
                ,""UpdatedTime""
            )
            select *
            from
            (
                select
                    @ipT as ""TenantId""
                    ,@ReferenceId as ""ReferenceId""
                    ,@Guid as ""Guid""
                    ,1 as ""Ver""
                    ,@BinaryType as ""BinaryType""
                    ,@Title as ""Title""
                    ,@Bin as ""Bin""
                    ,@FileName as ""FileName""
                    ,@ipU as ""Creator""
                    ,@ipU as ""Updator""
                    ,current_timestamp(3) as ""CreatedTime""
                    ,current_timestamp(3) as ""UpdatedTime""
            )
            as tmp
            where not exists
            (
                select 1
                from ""Binaries""
                where ""Binaries"".""TenantId"" = @ipT
                    and ""Binaries"".""Guid"" = @Guid
                    and ""Binaries"".""BinaryType"" = @BinaryType
            );";

        public string GetBinaryHash(string algorithm)
        {
            switch (algorithm)
            {
                case "md5":
                    return @"
                        select unhex(md5(""Bin""))
                        from ""Binaries""
                        where ""TenantId"" = @ipT
                            and ""Guid"" = @Guid;";
                case "sha256":
                    return @"
                        select unhex(sha2(""Bin"",256))
                        from ""Binaries""
                        where ""TenantId"" = @ipT
                            and ""Guid"" = @Guid;";
                default:
                    return string.Empty;
            }
        }
    }
}
