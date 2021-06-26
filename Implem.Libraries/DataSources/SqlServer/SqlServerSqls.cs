namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlServerSqls
    {
        public SqlServerSqls()
        {
        }

        public string TrueString { get; } = "1";

        public string FalseString { get; } = "0";

        public string IsNotTrue { get; } = " <> 1 ";

        public string CurrentDateTime { get; } = " getdate() ";

        public string Like { get; } = " like ";

        public string WhereLikeTemplateForward { get; } = "'%' + ";

        public string WhereLikeTemplate { get; } = "#ParamCount#_#CommandCount# + '%')";

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

        public string DateAddHour(int hour, string columnBracket)
        {
            return $"dateadd(hour,{hour},{columnBracket})";
        }

        public string DateGroupYearly { get; } = "substring(convert(varchar,{0},111),1,4)";

        public string DateGroupMonthly { get; } = "substring(convert(varchar,{0},111),1,7)";

        public string DateGroupWeeklyPart { get; } = "case datepart(weekday,{0}) when 1 then dateadd(day,-6,{0}) else dateadd(day,(2-datepart(weekday,{0})),{0}) end";

        public string DateGroupWeekly { get; } = "datepart(year,{0}) * 100 + datepart(week,{0})";

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

        public string PermissionsWhere { get; } = @"
            (
                exists
                (
                    select* from ""Depts""
                    where ""Depts"".""TenantId""=@_T
                        and ""Depts"".""DeptId""=@_D
                        and ""Depts"".""Disabled""='false'
                        and ""Permissions"".""DeptId""=""Depts"".""DeptId""
                        and @_D<>0
                )
                or
                (
                    exists
                    (
                        select*
                        from ""Groups"" inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        where ""Groups"".""TenantId""=@_T
                            and ""Groups"".""Disabled""='false'
                            and ""Permissions"".""GroupId""=""Groups"".""GroupId""
                            and
                            (
                                exists
                                (
                                    select* from ""Depts""
                                    where ""Depts"".""TenantId""=@_T
                                        and ""Depts"".""DeptId""=@_D
                                        and ""Depts"".""Disabled""='false'
                                        and ""GroupMembers"".""DeptId""=""Depts"".""DeptId""
                                        and @_D<>0
                                )
                                or
                                (
                                    ""GroupMembers"".""UserId""=@_U
                                    and @_U<>0
                                )
                            )
                    )
                )
                or
                (
                    ""Permissions"".""UserId""=@_U
                    and @_U<>0
                )
                or
                (
                    ""Permissions"".""UserId""=-1
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
                exists
                (
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                        inner join ""Depts"" as ""PermissionDepts"" on ""Permissions"".""DeptId""=""PermissionDepts"".""DeptId""
                        inner join ""Users"" as ""PermissionUsers"" on ""PermissionDepts"".""DeptId""=""PermissionUsers"".""DeptId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""PermissionUsers"".""UserId""=""Users"".""UserId""
                        and ""PermissionDepts"".""Disabled""='false'
                        and ""PermissionUsers"".""Disabled""='false'
                    union all
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                        inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        inner join ""Depts"" as ""GroupMemberDepts"" on ""GroupMembers"".""DeptId""=""GroupMemberDepts"".""DeptId""
                        inner join ""Users"" as ""GroupMemberUsers"" on ""GroupMemberDepts"".""DeptId""=""GroupMemberUsers"".""DeptId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""GroupMemberUsers"".""UserId""=""Users"".""UserId""
                        and ""Groups"".""Disabled""='false'
                        and ""GroupMemberDepts"".""Disabled""='false'
                        and ""GroupMemberUsers"".""Disabled""='false'
                    union all
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                        inner join ""Groups"" on ""Permissions"".""GroupId""=""Groups"".""GroupId""
                        inner join ""GroupMembers"" on ""Groups"".""GroupId""=""GroupMembers"".""GroupId""
                        inner join ""Users"" as ""GroupMemberUsers"" on ""GroupMembers"".""UserId""=""GroupMemberUsers"".""UserId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""GroupMemberUsers"".""UserId""=""Users"".""UserId""
                        and ""Groups"".""Disabled""='false'
                        and ""GroupMemberUsers"".""Disabled""='false'
                    union all
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                        inner join ""Users"" as ""PermissionUsers"" on ""Permissions"".""UserId""=""PermissionUsers"".""UserId""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""PermissionUsers"".""UserId""=""Users"".""UserId""
                        and ""Users"".""Disabled""='false'
                        and ""PermissionUsers"".""Disabled""='false'
                    union all
                    select ""Permissions"".""ReferenceId""
                    from ""Permissions""
                    where
                        ""Permissions"".""ReferenceId""={0}
                        and ""Permissions"".""UserId""=-1
                )
            )";
    }
}
