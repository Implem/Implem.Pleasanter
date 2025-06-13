using Implem.IRds;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace Implem.Libraries.DataSources.SqlServer
{
    public static class SqlDebugs
    {
        private static readonly object WriteLockObject = new object();
        private const long MaxFileSize = 1 * 1024 * 1024; // 1ファイルの上限：1MB
        private const string Separator = "-----------------------------------------------------------------------------------------------";// 改行文字を定数化
        private const string NewLine = "\r\n";

        // ファイルサイズキャッシュ（効率化のため）
        private static readonly ConcurrentDictionary<string, long> FileSizeCache = new ConcurrentDictionary<string, long>();

        // 共通ファイル名（SQL Server、PostgreSQL、MySQL共通）
        private static readonly string[] LogFiles = {
            "CommandTextForDebugging_1.sql",
            "CommandTextForDebugging_2.sql",
            "CommandTextForDebugging_3.sql"
        };

        [Conditional("DEBUG")]
        public static void WriteSqlLog(string provider, string rdsName, ISqlCommand sqlCommand, string logsPath, SqlLogOptions options = null)
        {
            options ??= GetDefaultOptions(provider);

            try
            {
                string logFilePath;
                lock (WriteLockObject)
                {
                    logFilePath = GetLogFilePath(logsPath, LogFiles);
                }

                var logContent = BuildLogContent(provider, rdsName, sqlCommand, options);
                WriteToFileThreadSafe(logFilePath, logContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL log write failed: {ex.Message}");
            }
        }

        private static SqlLogOptions GetDefaultOptions(string provider)
        {
            return provider switch
            {
                "SQLServer" => new SqlLogOptions
                {
                    IncludeUseStatement = true,
                    ReplaceParameters = false
                },
                "PostgreSQL" or "MySQL" => new SqlLogOptions
                {
                    IncludeUseStatement = false,
                    ReplaceParameters = true
                },
                _ => new SqlLogOptions()
            };
        }

        private static string BuildLogContent(string provider, string rdsName, ISqlCommand sqlCommand, SqlLogOptions options)
        {
            var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
            var parametersText = DeclareParametersText(sqlCommand);
            var formattedSql = FormattedCommandText(sqlCommand);

            if (options.ReplaceParameters)
            {
                formattedSql = ReplaceParameters(formattedSql, sqlCommand);
            }

            var logBuilder = new StringBuilder();

            // タイムスタンプ
            logBuilder.AppendLine($"-- [{timestamp}] -----------------------------------------------------------------------------");

            // USE文（SQL Server用）
            if (options.IncludeUseStatement && !string.IsNullOrEmpty(rdsName))
            {
                logBuilder.AppendLine($"use [{rdsName}];");
            }

            logBuilder.AppendLine(parametersText);
            logBuilder.AppendLine(formattedSql);

            return logBuilder.ToString();
        }

        private static string ReplaceParameters(string sql, ISqlCommand sqlCommand)
        {
            var parameters = sqlCommand.SqlParameters()
                .Select(p => new { Name = "@" + GetParameterName(p), Value = GetParameterValue(p) })
                .OrderByDescending(p => p.Name.Length);

            foreach (var param in parameters)
            {
                sql = sql.Replace(param.Name, param.Value);
            }

            return sql;
        }


        private static string GetParameterValue(ISqlParameter parameter)
        {
            if (parameter.Value == null || parameter.Value == DBNull.Value)
            {
                return "NULL";  // パラメータの値がnullの場合はNULLと表示する
            }

            return parameter.DbType switch
            {
                System.Data.DbType.String
                    or System.Data.DbType.AnsiString
                    or System.Data.DbType.AnsiStringFixedLength
                    or System.Data.DbType.StringFixedLength
                    or System.Data.DbType.Date
                    or System.Data.DbType.DateTime
                    or System.Data.DbType.DateTime2
                    or System.Data.DbType.DateTimeOffset => $"'{parameter.Value.ToStr()}'",
                _ => parameter.Value.ToStr()
            };
        }
        private static readonly string[] Keywords = {
            "begin", "commit", "select", "insert into", "values", "update", "delete from",
            "from", "inner join", "left outer join", "right outer join", "full outer join",
            "where", "group by", "having", "order by", "union", "union all", "if"
        };

        private static readonly Regex SqlKeywordRegex = new(
            string.Join("|", Keywords.Select(keyword => $@"(?<!\r?\n)\b{Regex.Escape(keyword)}\b")),
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled
        );

        private static string FormattedCommandText(ISqlCommand sqlCommand)
        {
            if (string.IsNullOrEmpty(sqlCommand.CommandText))
                return string.Empty;

            // SQLキーワードの前に改行を挿入
            var formattedText = SqlKeywordRegex.Replace(
                sqlCommand.CommandText,
                match => NewLine + match.Value
            );

            var estimatedSize = sqlCommand.CommandText.Length +
                               (sqlCommand.CommandText.Count(c => c == ' ') * 2) +
                               100; // パラメータ部分
            var commandTextFormatted = new StringBuilder(estimatedSize);

            formattedText.SplitReturn()
                .Where(line => !string.IsNullOrWhiteSpace(line))  // 空行を除外
                .ForEach(line =>
                {
                    var cleanLine = line.Trim(); // ← 追加：各行Trim
                    commandTextFormatted.AppendLine(cleanLine.Split(',')
                        .Chunk(3)
                        .Select(o => o.Join(", "))
                        .Join("," + NewLine));
                });
            return commandTextFormatted.ToString();
        }

        private static string DeclareParametersText(ISqlCommand sqlCommand)
        {
            var parameters = sqlCommand.SqlParameters().ToList();
            if (!parameters.Any())
                return string.Empty;

            var estimatedSize = parameters.Count * 80;
            var commandParameters = new StringBuilder(estimatedSize);

            foreach (var parameter in parameters)
            {
                var parameterName = GetParameterName(parameter);
                var declarePart = DeclareParameterText(parameter);
                var parameterValue = GetParameterValue(parameter);

                commandParameters.AppendLine($"{declarePart,-50}set @{parameterName} = {parameterValue};");
            }

            return commandParameters.ToString();
        }

        private static string DeclareParameterText(ISqlParameter parameter)
        {
            return parameter.Size == 0
                ? "declare @{0} {1}; ".Params(
                    GetParameterName(parameter),
                    parameter.SqlDbType.ToString().ToLower())
                : "declare @{0} {1}({2}); ".Params(
                    GetParameterName(parameter),
                    parameter.SqlDbType.ToString().ToLower(),
                    parameter.Size);
        }

        private static string GetParameterName(ISqlParameter parameter)
        {
            return parameter.ParameterName.StartsWith("@")
                ? parameter.ParameterName.Substring(1)
                : parameter.ParameterName;
        }



        /// <summary>
        /// 使用するログファイルを決定する（効率化版：ファイルサイズキャッシュ使用）
        /// </summary>
        /// <param name="logsPath">ログファイルのパス</param>
        /// <param name="logFileNames">ローテーション対象のファイル名配列</param>
        /// <returns>使用するログファイルのフルパス</returns>
        private static string GetLogFilePath(string logsPath, string[] logFileNames)
        {
            Directory.CreateDirectory(logsPath);

            // 1. キャッシュを使ってサイズが上限未満のファイルを探す
            for (int i = 0; i < logFileNames.Length; i++)
            {
                string logFilePath = Path.Combine(logsPath, logFileNames[i]);

                // キャッシュからファイルサイズを取得（存在しない場合は実際のサイズを取得してキャッシュ）
                long fileSize = FileSizeCache.GetOrAdd(logFilePath, filePath =>
                {
                    try
                    {
                        return File.Exists(filePath) ? new FileInfo(filePath).Length : 0;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"File access error for {filePath}: {ex.Message}");
                        return MaxFileSize; // エラーの場合は上限値を返してスキップ
                    }
                });

                if (fileSize < MaxFileSize)
                {
                    return logFilePath; // サイズが上限未満なら使用
                }
            }

            // 2. すべてのファイルが上限を超えた場合、最初のファイルをリセット
            string oldestFilePath = Path.Combine(logsPath, logFileNames[0]);
            try
            {
                File.WriteAllText(oldestFilePath, "");
                // キャッシュをリセット
                FileSizeCache.TryUpdate(oldestFilePath, 0, FileSizeCache.GetOrAdd(oldestFilePath, MaxFileSize));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to reset log file {oldestFilePath}: {ex.Message}");
                // フォールバック: 一時ファイル名を生成
                oldestFilePath = Path.Combine(logsPath, $"CommandTextForDebugging_Fallback_{DateTime.Now:yyyyMMdd_HHmmss}.sql");
                // 新しいファイルのキャッシュエントリを作成
                FileSizeCache.TryAdd(oldestFilePath, 0);
            }

            return oldestFilePath;
        }

        /// <summary>
        /// スレッドセーフにファイルへ書き込む（キャッシュ更新付き）
        /// </summary>
        /// <param name="filePath">書き込み先ファイルパス</param>
        /// <param name="content">書き込む内容</param>
        private static void WriteToFileThreadSafe(string filePath, string content)
        {
            lock (WriteLockObject)
            {
                try
                {
                    long contentLength = Encoding.UTF8.GetByteCount(content);

                    using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (var writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        writer.Write(content);
                    }

                    // キャッシュを更新（書き込み後のファイルサイズ）
                    FileSizeCache.AddOrUpdate(filePath,
                        contentLength, // ファイルが新規の場合
                        (key, oldSize) => oldSize + contentLength); // 既存ファイルの場合は加算
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to write to log file {filePath}: {ex.Message}");

                    // エラーが発生した場合、キャッシュエントリを削除して次回は実ファイルサイズを確認
                    FileSizeCache.TryRemove(filePath, out _);
                }
            }
        }
    }

    public class SqlLogOptions
    {
        public bool IncludeUseStatement { get; set; } = true;
        public bool ReplaceParameters { get; set; } = false;
    }
}
