using Implem.IRds;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Implem.PostgreSql
{
    class PostgreSqlCommand : ISqlCommand
    {
        NpgsqlCommand _instance;
        internal NpgsqlCommand InnerInstance => _instance;

        public PostgreSqlCommand()
        {
            _instance = new NpgsqlCommand();
        }

        public string CommandText
        {
            get => _instance.CommandText;
            set
            {
                _instance.CommandText = value;


                //TODO CommandText
                var texts = value.Split(';').Select(sql => ConvertCommandTextSQLServerToPostgreSQL(sql, value)).ToArray();
                var r = string.Join("; ", texts.Where(sql => !string.IsNullOrWhiteSpace(sql)));

                _instance.CommandText = r;
            }
        }
        public int CommandTimeout { get => _instance.CommandTimeout; set => _instance.CommandTimeout = value; }
        public CommandType CommandType { get => _instance.CommandType; set => _instance.CommandType = value; }
        public IDbConnection Connection { get => _instance.Connection;
            set
            {
                { if (value is NpgsqlConnection con) _instance.Connection = con; }
                { if (value is PostgreSqlConnection con) _instance.Connection = con.InnerInstance; }
            }
        }

        public IDataParameterCollection Parameters => _instance.Parameters;
        public IDbTransaction Transaction { get => _instance.Transaction; set => _instance.Transaction = (NpgsqlTransaction)value; }
        public UpdateRowSource UpdatedRowSource { get => _instance.UpdatedRowSource; set => _instance.UpdatedRowSource = value; }

        public void Cancel()
        {
            _instance.Cancel();
        }

        public object Clone()
        {
            return _instance.Clone();
        }

        public IDbDataParameter CreateParameter()
        {
            return _instance.CreateParameter();
        }

        public void Dispose()
        {
            _instance.Dispose();
        }

        public int ExecuteNonQuery()
        {
            return _instance.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            return _instance.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return _instance.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            return _instance.ExecuteScalar();
        }

        public void Prepare()
        {
            _instance.Prepare();
        }

        //TODO
        public void Parameters_AddWithValue(string parameterName, object value)
        {
            if (parameterName?.StartsWith("_") == true)
                parameterName = $"ip{parameterName?.Substring(1)}";
            _instance.Parameters.AddWithValue(parameterName, value);
        }

        public void Parameters_Add(ISqlParameter parameter)
        {
            _instance.Parameters.Add(new NpgsqlParameter(parameter.ParameterName, parameter.Value));
        }

        public IEnumerable<ISqlParameter> SqlParameters()
        {
            foreach(NpgsqlParameter parameter in _instance.Parameters)
                yield return new PostgreSqlParameter(parameter);
        }

        //TODO
        private static Regex regexTop = new Regex(" [tT][oO][pP]\\s+[0-9]+ ");


        //TODO
        private string ConvertCommandTextSQLServerToPostgreSQL(string sqlserverCommandText, string allvalue)
        {
            var text = sqlserverCommandText;

            if(text?.Contains(" @@rowcount ") == true)
            {
                text = text;
            }

            text = text?.Replace("[", "\"").Replace("]", "\"");
            text = text?.Replace("set xact_abort on", "");
            text = text?.Replace("@_", "@ip");
            text = text?.Replace(" set @ipI = @@identity", "");

            text = text?.Replace("@PasswordExpirationTime_", "@PasswordExpirationTime_ :: timestamp");
            text = text?.Replace("getdate()", " CURRENT_TIMESTAMP ");
            text = text?.Replace("(\"UserArea\" <> 1)", "(\"UserArea\" is not true)");
            text = text?.Replace("(\"ReadOnce\"=@ReadOnce2)", "(\"ReadOnce\"=@ReadOnce2 :: bool)");

            text = text?.Replace("join Items as", "join \"Items\" as");

            if (text?.ToLower().Contains(" top ") == true) { text = text?.Replace(" TOP 1 ", " ").Replace(" top 1 ", " "); var match = regexTop.Match(text); if (match.Success) { text = text?.Replace(match.Value, ""); var limit = match.Value.ToLower().Replace("top", "").Trim(); text = $"{text} limit {limit} "; } }

            if (text?.Contains("if @@rowcount = 0") == true)
            {
                if(text?.Contains("where (\"TenantId\"=@TenantId) and (\"StatusId\"=@StatusId)") == true)
                    text = string.Join(" ON CONFLICT ON CONSTRAINT \"Statuses_pkey\" DO ", text.Split(new[] { "if @@rowcount = 0" }, System.StringSplitOptions.None).Reverse().Select(s => s.Replace(";", "")))
                        .Replace("update \"Statuses\" set", " update set ")
                        .Replace("where (\"TenantId\"=@TenantId) and (\"StatusId\"=@StatusId)", " ")
                        + ";";

                if (text?.Contains("where (\"SessionGuid\"=@SessionGuid) and (\"Key\"=@Key)") == true)
                    text = string.Join(" ON CONFLICT ON CONSTRAINT \"Sessions_pkey\" DO ", text.Split(new[] { "if @@rowcount = 0" }, System.StringSplitOptions.None).Reverse().Select(s => s.Replace(";", "")))
                        .Replace("update \"Sessions\" set", " update set ")
                        .Replace("where (\"SessionGuid\"=@SessionGuid) and (\"Key\"=@Key) and (\"Page\"=@Page)", " ")
                        .Replace("where (\"SessionGuid\"=@SessionGuid) and (\"Key\"=@Key)", " ")
                        + ";";

                if (text?.Contains("where (\"TenantId\"=@TenantId1) and (\"StatusId\"=@StatusId1)") == true)
                    text = string.Join(" ON CONFLICT ON CONSTRAINT \"Statuses_pkey\" DO ", text.Split(new[] { "if @@rowcount = 0" }, System.StringSplitOptions.None).Reverse().Select(s => s.Replace(";", "")))
                        .Replace("update \"Statuses\" set", " update set ")
                        .Replace("where (\"TenantId\"=@TenantId1) and (\"StatusId\"=@StatusId1)", " ")
                        + ";";

                if (text?.Contains("where (\"TenantId\"=@TenantId2) and (\"StatusId\"=@StatusId2)") == true)
                    text = string.Join(" ON CONFLICT ON CONSTRAINT \"Statuses_pkey\" DO ", text.Split(new[] { "if @@rowcount = 0" }, System.StringSplitOptions.None).Reverse().Select(s => s.Replace(";", "")))
                        .Replace("update \"Statuses\" set", " update set ")
                        .Replace("where (\"TenantId\"=@TenantId2) and (\"StatusId\"=@StatusId2)", " ")
                        + ";";

                if (text?.Contains("where (\"TenantId\"=@TenantId3) and (\"StatusId\"=@StatusId3)") == true)
                    text = string.Join(" ON CONFLICT ON CONSTRAINT \"Statuses_pkey\" DO ", text.Split(new[] { "if @@rowcount = 0" }, System.StringSplitOptions.None).Reverse().Select(s => s.Replace(";", "")))
                        .Replace("update \"Statuses\" set", " update set ")
                        .Replace("where (\"TenantId\"=@TenantId3) and (\"StatusId\"=@StatusId3)", " ")
                        + ";";

                if (text?.Contains("where (\"TenantId\"=@TenantId4) and (\"StatusId\"=@StatusId4)") == true)
                    text = string.Join(" ON CONFLICT ON CONSTRAINT \"Statuses_pkey\" DO ", text.Split(new[] { "if @@rowcount = 0" }, System.StringSplitOptions.None).Reverse().Select(s => s.Replace(";", "")))
                        .Replace("update \"Statuses\" set", " update set ")
                        .Replace("where (\"TenantId\"=@TenantId4) and (\"StatusId\"=@StatusId4)", " ")
                        + ";";

                if (text?.Contains("where (\"TenantId\"=@TenantId5) and (\"StatusId\"=@StatusId5)") == true)
                    text = string.Join(" ON CONFLICT ON CONSTRAINT \"Statuses_pkey\" DO ", text.Split(new[] { "if @@rowcount = 0" }, System.StringSplitOptions.None).Reverse().Select(s => s.Replace(";", "")))
                        .Replace("update \"Statuses\" set", " update set ")
                        .Replace("where (\"TenantId\"=@TenantId5) and (\"StatusId\"=@StatusId5)", " ")
                        + ";";
            }

            return text;
        }

    }
}
