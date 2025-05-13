using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Collections.Generic;

namespace Implem.SqlServer
{
    internal class SqlServerSqls : ISqls
    {
        public string TrueString { get; } = "1";

        public string FalseString { get; } = "0";

        public string IsNotTrue { get; } = " <> 1 ";

        public string CurrentDateTime { get; } = " getdate() ";

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
                .Replace("%", "|%")
                .Replace("[", "|[");
        }

        public string IsNull { get; } = "isnull";

        public string WhereLikeTemplateForward { get; } = "'%' + ";

        public string WhereLikeTemplate { get; } = "#ParamCount#_#CommandCount# + '%'";

        public string GenerateIdentity { get; } = " identity({0}, 1)";

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
            return "(\"" + tableName + "\".\"" + columnName + "\" like ";
        }

        public string DateAddDay(int day, string columnBracket)
        {
            return $"dateadd(day, {day}, {columnBracket})";
        }

        public string DateAddHour(int hour, string columnBracket)
        {
            return $"dateadd(hour,{hour},{columnBracket})";
        }

        public string DateGroupYearly { get; } = "substring(convert(varchar,{0},111),1,4)";

        public string DateGroupMonthly { get; } = "substring(convert(varchar,{0},111),1,7)";

        public string DateGroupWeeklyPart { get; } = "case datepart(weekday,{0}) when 1 then dateadd(day,-6,{0}) else dateadd(day,(2-datepart(weekday,{0})),{0}) end";

        public string DateGroupWeekly { get; } = "datepart(year,{0}) * 100 + datepart(iso_week,{0})";

        public string DateGroupDaily { get; } = "convert(varchar,{0},111)";

        public string GetPermissions { get; } = @"
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Depts"" on ""Permissions"".""DeptId""=""Depts"".""DeptId""
            where ""Sites"".""TenantId""=@_T
                and ""Depts"".""DeptId""=@_D
                and ""Depts"".""Disabled""='false'
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Sites"".""TenantId""=@_T
                and ""Groups"".""Disabled""='false'
                and ""Depts"".""DeptId""=@_D
                and ""Depts"".""Disabled""='false'
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType"" 
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
                inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Sites"".""TenantId""=@_T
                and ""Groups"".""Disabled""='false'
                and ""Users"".""UserId""=@_U
                and ""Users"".""Disabled""='false'
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
            where ""Sites"".""TenantId""=@_T
                and ""Permissions"".""UserId"" > 0
                and ""Permissions"".""UserId""=@_U
            union all
            select distinct
                ""Sites"".""SiteId"" as ""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Sites""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Sites"".""InheritPermission""
            where ""Sites"".""TenantId""=@_T
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
                and ""Sites"".""TenantId""=@_T
                and ""Depts"".""DeptId""=@_D
                and ""Depts"".""Disabled""='false'
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
                and ""Sites"".""TenantId""=@_T
                and ""Groups"".""Disabled""='false'
                and ""Depts"".""DeptId""=@_D
                and ""Depts"".""Disabled""='false'
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
                and ""Sites"".""TenantId""=@_T
                and ""Groups"".""Disabled""='false'
                and ""Users"".""UserId""=@_U
                and ""Users"".""Disabled""='false'
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@_T
                and ""Permissions"".""UserId"" > 0
                and ""Permissions"".""UserId""=@_U
            union all
            select distinct
                ""Items"".""ReferenceId"",
                ""Permissions"".""PermissionType""
            from ""Items""
                inner join ""Sites"" on ""Items"".""SiteId""=""Sites"".""SiteId""
                inner join ""Permissions"" on ""Permissions"".""ReferenceId""=""Items"".""ReferenceId""
            where ""Items"".""ReferenceId""=@ReferenceId
                and ""Sites"".""TenantId""=@_T
                and ""Permissions"".""UserId""=-1;";

        public string GetGroup { get; } = @"
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Depts"".""TenantId""=@_T
                and ""Depts"".""DeptId""=@_D
            union all
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Users"".""TenantId""=@_T
                and ""Users"".""UserId""=@_U;";

        public string GetEnabledGroup { get; } = @"
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Depts"" on ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
            where ""Groups"".""Disabled""='false'
                and ""Depts"".""TenantId""=@_T
                and ""Depts"".""DeptId""=@_D
                and ""Depts"".""Disabled""='false'
            union all
            select ""Groups"".""GroupId"" 
            from ""Groups"" as ""Groups""
                inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                inner join ""Users"" on ""GroupMembers"".""UserId""=""Users"".""UserId""
            where ""Groups"".""Disabled""='false'
                and ""Users"".""TenantId""=@_T
                and ""Users"".""UserId""=@_U;";

        public string PermissionsWhere { get; } = @"
            (
                exists
                (
                    select ""Depts"".""DeptId"" as ""Id""
                    from ""Depts""
                    where ""Depts"".""TenantId""=@_T
                        and ""Depts"".""DeptId""=@_D
                        and ""Depts"".""Disabled""='false'
                        and ""Permissions"".""DeptId""=""Depts"".""DeptId""
                        and @_D<>0
                    union all
                    select ""Groups"".""GroupId"" as ""Id""
                    from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                    where ""Groups"".""TenantId""=@_T
                        and ""Groups"".""Disabled""='false'
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and exists
                        (
                            select ""DeptId""
                            from ""Depts""
                            where ""Depts"".""TenantId""=@_T
                                and ""Depts"".""DeptId""=@_D
                                and ""Depts"".""Disabled""='false'
                                and ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
                                and @_D<>0
                        )
                    union all
                    select ""Groups"".""GroupId"" as ""Id""
                    from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                    where ""Groups"".""TenantId""=@_T
                        and ""Groups"".""Disabled""='false'
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and ""GroupMembers"".""UserId""=@_U
                        and @_U<>0
                    union all
                    select ""P"".""UserId"" as ""Id""
                    from ""Permissions"" as ""P""
                    where ""P"".""ReferenceId""=""Permissions"".""ReferenceId""
                        and ""P"".""UserId""=""Permissions"".""UserId""
                        and ""P"".""UserId""=@_U
                        and @_U<>0
                    union all
                    select ""P"".""UserId"" as ""Id""
                    from ""Permissions"" as ""P""
                    where ""P"".""ReferenceId""=""Permissions"".""ReferenceId""
                        and ""P"".""UserId""=-1
                )
            )";

        public string PermissionsWhereForIntegratedSites { get; } = @"
            (
                exists
                (
                    select ""Depts"".""DeptId"" as ""Id""
                    from ""Depts""
                    where ""Depts"".""TenantId""=@_T
                        and ""Depts"".""DeptId""=@_D
                        and ""Depts"".""Disabled""='false'
                        and ""Permissions"".""DeptId""=""Depts"".""DeptId""
                        and @_D<>0
                    union all
                    select ""Groups"".""GroupId"" as ""Id""
                    from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                    where ""Groups"".""TenantId""=@_T
                        and ""Groups"".""Disabled""='false'
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and exists
                        (
                            select ""DeptId""
                            from ""Depts""
                            where ""Depts"".""TenantId""=@_T
                                and ""Depts"".""DeptId""=@_D
                                and ""Depts"".""Disabled""='false'
                                and ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
                                and @_D<>0
                        )
                    union all
                    select ""Groups"".""GroupId"" as ""Id""
                    from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                    where ""Groups"".""TenantId""=@_T
                        and ""Groups"".""Disabled""='false'
                        and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        and ""GroupMembers"".""UserId""=@_U
                        and @_U<>0
                    union all
                    select ""P"".""UserId"" as ""Id""
                    from ""Permissions"" as ""P""
                    where ""P"".""ReferenceId""=""Permissions"".""ReferenceId""
                        and ""P"".""UserId""=""Permissions"".""UserId""
                        and ""P"".""UserId""=@_U
                        and @_U<>0
                    union all
                    select ""P"".""UserId"" as ""Id""
                    from ""Permissions"" as ""P""
                    where ""P"".""ReferenceId""=""Permissions"".""ReferenceId""
                        and ""P"".""PermissionType"" & 1 = 1
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
                                ""Depts"".""Disabled""='false'
                                and ""Depts"".""DeptId""=""Depts"".""DeptId""
                            )
                            or 
                            (
                                ""Groups"".""Disabled""='false' and 
                                (
                                    (
                                        ""GroupMemberDepts"".""Disabled""='false'
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
                        and ""Groups"".""Disabled""='false'
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
                        and ""PermissionDepts"".""Disabled""='false'
                        and ""PermissionUsers"".""Disabled""='false'
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
                        and ""Groups"".""Disabled""='false'
                        and ""GroupMemberDepts"".""Disabled""='false'
                        and ""GroupMemberUsers"".""Disabled""='false'
                    union all
                    select ""Users"".""UserId""
                    from ""Permissions""
                        inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        inner join ""Users"" as ""GroupMemberUsers"" on ""GroupMembers"".""UserId""=""GroupMemberUsers"".""UserId""
                        inner join ""Users"" on ""GroupMemberUsers"".""UserId"" = ""Users"".""UserId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""Groups"".""Disabled""='false'
                        and ""GroupMemberUsers"".""Disabled""='false'
                    union all
                    select ""Users"".""UserId""
                    from ""Permissions""
                        inner join ""Users"" as ""PermissionUsers"" on ""Permissions"".""UserId""=""PermissionUsers"".""UserId""
                        inner join ""Users"" on ""Users"".""Disabled""='false'
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""PermissionUsers"".""UserId""=""Users"".""UserId""
                        and ""PermissionUsers"".""Disabled""='false'
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
                        and {PermissionsWhereForIntegratedSites}
                    union
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                    where ""Permissions"".""ReferenceId""=""{tableName}_Items"".""ReferenceId""
                        and ""Permissions"".""PermissionType"" & 1 = 1
                        and {PermissionsWhereForIntegratedSites}
                )";
        }

        public string UpsertBinary { get; } = @"
            merge into ""Binaries"" as ""Target""
            using
            (
                select 
                    @_T as ""TenantId""
                    ,@ReferenceId as ""ReferenceId""
                    ,@Guid as ""Guid""
                    ,@BinaryType as ""BinaryType""
                    ,@Title as ""Title""
                    ,@Bin as ""Bin""
                    ,@FileName as ""FileName""
                    ,@_U as ""Creator""
                    ,@_U as ""Updator""
            ) as ""Temp""
            on ""Target"".""TenantId"" = @_T
                and ""Target"".""Guid"" = ""Temp"".""Guid""
                and ""Target"".""BinaryType"" = ""Temp"".""BinaryType""
            when matched
            then
                update set 
                    ""Bin"" = concat(cast(""Target"".""Bin"" as varbinary(max)), cast(""Temp"".""Bin"" as varbinary(max)))
                    ,""Updator"" = @_U
                    ,""UpdatedTime"" = current_timestamp
            when not matched
            then
                insert
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
                values
                (
                    @_T
                    ,@ReferenceId
                    ,@Guid
                    ,1
                    ,@BinaryType
                    ,@Title
                    ,@Bin
                    ,@FileName
                    ,@_U
                    ,@_U
                    ,current_timestamp
                    ,current_timestamp
                );";

        public string GetBinaryHash(string algorithm)
        {
            return @"
                select hashbytes(@Algorithm, cast(""Bin"" as varbinary(max)))
                from ""Binaries""
                where ""TenantId"" = @_T
                    and ""Guid"" = @Guid;";
        }

        public string MigrateDatabaseSelectFrom(string tableName)
        {
            return $@"select * from ""{tableName}"";";
        }

        public string GetChildSiteIdList { get; } = @"
            with cte as (
                select 
                    ""SiteId"",
                    ""ParentId"",
                    ""SiteId"" as ""RootSiteId""
                from ""Sites""
                where ""ParentId"" = @SiteId_
                    and ""TenantId"" = @_T
                union all
                select 
                    c.""SiteId"",
                    c.""ParentId"",
                    p.""RootSiteId""
                from ""Sites"" c
                inner join cte p 
                on p.""SiteId"" = c.""ParentId""
                where c.""TenantId"" = @_T
            )
            select 
                ""RootSiteId"",
                ""SiteId""
            from cte
            order by ""RootSiteId"";";

    }
}
