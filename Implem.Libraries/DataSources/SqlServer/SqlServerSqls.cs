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

        public string SiteUserWhere { get; } = @"
            (
                exists
                (
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
                                    and ""Depts"".""DeptId""=""Users"".""DeptId""
                                )
                                or 
                                (
                                    ""Groups"".""Disabled""='false' and 
                                    (
                                        (
                                            ""GroupMemberDepts"".""Disabled""='false'
                                            and ""GroupMemberDepts"".""DeptId""=""Users"".""DeptId""
                                        )
                                        or
                                        (
                                            ""GroupMembers"".""UserId""=""Users"".""UserId""
                                        )
                                    )
                                )
                                or
                                (
                                    ""Permissions"".""UserId""=""Users"".""UserId""
                                    and ""Permissions"".""UserId"">0
                                )
                                or
                                (
                                    ""Permissions"".""UserId""=-1
                                )
                            )
                    )
                )
            )";
    }
}
