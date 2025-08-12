using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptFile
    {
        private readonly Context Context;

        public ServerScriptFile(Context context)
        {
            Context = context;
        }

        public string ReadAllText(
            ScriptObject callback,
            string section,
            string path,
            string encode)
        {
            string ret = null;
            CallbackHandler(
                log: $"$ps.file.readAllText(\"*\",\"{path}\",\"{encode}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    if (!File.Exists(fullPath)) return;
                    if (Parameters.Script.ServerScriptFileSizeMax >= 0)
                    {
                        var info = new FileInfo(fullPath);
                        if (info.Length >= Parameters.Script.ServerScriptFileSizeMax * 1024 * 1024)
                        {
                            throw new Exception(
                                Messages.OverLimitSize(
                                    context: Context,
                                    Parameters.Script.ServerScriptFileSizeMax.ToString())
                                .Text);
                        }
                    }
                    ret = File.ReadAllText(fullPath, GetEncoding(encode: encode));
                });
            return ret;
        }

        public bool WriteAllText(
            ScriptObject callback,
            string section,
            string path,
            string data,
            string encode)
        {
            var ret = false;
            CallbackHandler(
                log: $"$ps.file.writeAllText(\"*\",\"{path}\",(data),\"{encode}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    var dir = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.WriteAllText(fullPath, data, GetEncoding(encode: encode));
                    ret = true;
                });
            return ret;
        }

        public dynamic GetFileList(
            ScriptObject callback,
            string section,
            string path)
        {
            var ret = new string[0];
            CallbackHandler(
                log: $"$ps.file.getFileList(\"*\",\"{path}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    if (!Directory.Exists(fullPath)) return;
                    ret = Directory.GetFiles(fullPath).Select(x => Path.GetFileName(x)).ToArray();
                });
            return V8ScriptEngine.Current.Script.Array.from(ret);
        }

        public dynamic GetDirectoryList(
            ScriptObject callback,
            string section,
            string path)
        {
            var ret = new string[0];
            CallbackHandler(
                log: $"$ps.file.getDirectoryList(\"*\",\"{path}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    if (!Directory.Exists(fullPath)) return;
                    ret = Directory.GetDirectories(fullPath).Select(x => Path.GetFileName(x)).ToArray();
                });
            return V8ScriptEngine.Current.Script.Array.from(ret);
        }

        public bool RemoveFile(
            ScriptObject callback,
            string section,
            string path)
        {
            var ret = false;
            CallbackHandler(
                log: $"$ps.file.removeFile(\"*\",\"{path}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    if (!File.Exists(fullPath)) return;
                    File.Delete(fullPath);
                    ret = true;
                });
            return ret;
        }

        public bool MoveFile(
            ScriptObject callback,
            string section,
            string oldPath,
            string newPath)
        {
            var ret = false;
            CallbackHandler(
                log: $"$ps.file.moveFile(\"*\",\"{oldPath}\",\"{newPath}\")",
                callback: callback,
                code: () =>
                {
                    var oldName = NormalizePath(
                        section: section,
                        path: oldPath);
                    var newName = NormalizePath(
                        section: section,
                        path: newPath);
                    if (!File.Exists(oldName) || File.Exists(newName)) return;
                    var basePath = Path.GetDirectoryName(newName);
                    if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                    File.Move(oldName, newName);
                    ret = true;
                });
            return ret;
        }

        public bool CopyFile(
            ScriptObject callback,
            string section,
            string sourcePath,
            string destPath)
        {
            var ret = false;
            CallbackHandler(
                log: $"$ps.file.copyFile(\"*\",\"{sourcePath}\",\"{destPath}\")",
                callback: callback,
                code: () =>
                {
                    var sourceName = NormalizePath(
                        section: section,
                        path: sourcePath);
                    var destName = NormalizePath(
                        section: section,
                        path: destPath);
                    if (!File.Exists(sourceName) || File.Exists(destName)) return;
                    var basePath = Path.GetDirectoryName(destName);
                    if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                    File.Copy(sourceName, destName);
                    ret = true;
                });
            return ret;
        }

        public bool CreateDirectory(
            ScriptObject callback,
            string section,
            string path)
        {
            var ret = false;
            CallbackHandler(
                log: $"$ps.file.createDirectory(\"*\",\"{path}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    ret = true;
                });
            return ret;
        }

        public bool RemoveDirectory(
            ScriptObject callback,
            string section,
            string path)
        {
            var ret = false;
            CallbackHandler(
                log: $"$ps.file.removeDirectory(\"*\",\"{path}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    if (Directory.Exists(fullPath)) Directory.Delete(fullPath, true);
                    ret = true;
                });
            return ret;
        }

        public bool MoveDirectory(
            ScriptObject callback,
            string section,
            string oldPath,
            string newPath)
        {
            var ret = false;
            CallbackHandler(
                log: $"$ps.file.moveDirectory(\"*\",\"{oldPath}\",\"{newPath}\")",
                callback: callback,
                code: () =>
                {
                    var oldName = NormalizePath(
                        section: section,
                        path: oldPath);
                    var newName = NormalizePath(
                        section: section,
                        path: newPath);
                    if (!Directory.Exists(oldName) || Directory.Exists(newName)) return;
                    var basePath = Path.GetDirectoryName(newName);
                    if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                    Directory.Move(oldName, newName);
                    ret = true;
                });
            return ret;
        }

        public bool CreateSection(
            ScriptObject callback,
            string section)
        {
            return CreateDirectory(
                callback: callback,
                section: section,
                path: null);
        }

        public bool RemoveSection(
            ScriptObject callback,
            string section)
        {
            return RemoveDirectory(
                callback: callback,
                section: section,
                path: null);
        }

        public object Import(
            ScriptObject callback,
            string section,
            string path,
            long siteId,
            string json)
        {
            object ret = null;
            CallbackHandler(
                log: $"$ps.file.import(\"*\",\"{path}\",{siteId},\"{json}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    var retJson = ServerScriptUtilities.ImportItem(
                        context: Context,
                        id: siteId,
                        apiRequestBody: json,
                        filePath: fullPath);
                    if (!retJson.IsNullOrEmpty())
                    {
                        ret = V8ScriptEngine.Current.Evaluate($"JSON.parse('{retJson}')");
                    }
                });
            return ret;
        }

        public bool Export(
            ScriptObject callback,
            string section,
            string path,
            long siteId,
            string json)
        {
            var ret = false;
            CallbackHandler(
                log: $"$ps.file.export(\"*\",\"{path}\",{siteId},\"{json}\")",
                callback: callback,
                code: () =>
                {
                    var fullPath = NormalizePath(
                        section: section,
                        path: path);
                    ret = ServerScriptUtilities.ExportItem(
                        context: Context,
                        id: siteId,
                        apiRequestBody: json,
                        filePath: fullPath);
                });
            return ret;
        }

        private static System.Text.Encoding GetEncoding(string encode)
        {
            return System.Text.Encoding.GetEncoding(encode ?? "utf-8");
        }

        private string NormalizePath(
            string section,
            string path)
        {
            var rootPath = Parameters.Script.ServerScriptFilePath;
            if (rootPath == null) throw new ArgumentException(Messages.InvalidPath(Context, "basepath").Text);
            var rootPath1 = rootPath.Replace(Path.DirectorySeparatorChar, '/');
            var rootPath2 = rootPath.Replace('/', Path.DirectorySeparatorChar);
            if (rootPath2 != Path.GetFullPath(rootPath2)) throw new ArgumentException(Messages.InvalidPath(Context, "basepath").Text);
            var regexBase = new Regex("^([a-zA-Z]:/$|//.*|/)$");
            if (regexBase.IsMatch(rootPath1)) throw new ArgumentException(Messages.InvalidPath(Context, "basepath").Text);
            var regexSection = new Regex(@"^[0-9a-zA-Z_\.\-]*$");
            if (!regexSection.IsMatch(section)) throw new ArgumentException(Messages.InvalidPath(Context, "section").Text);
            var regexPath = new Regex(@"^[0-9a-zA-Z_\.\-/]*$");
            if (path != null && !regexPath.IsMatch(path)) throw new ArgumentException(Messages.InvalidPath(Context, "path").Text);
            var sectionPath = Path.Combine(rootPath2, section);
            var fullPath = Path.GetFullPath(Path.Combine(sectionPath, path?.Replace('/', Path.DirectorySeparatorChar) ?? ""));
            if (!fullPath.StartsWith(sectionPath)) throw new ArgumentException(Messages.InvalidPath(Context, "path").Text);
            return fullPath;
        }

        private void CallbackHandler(
            string log,
            ScriptObject callback,
            Action code,
            Func<Exception, bool> skipException = null)
        {
            skipException ??= (e) => false;
            var logMsg = "success:" + log;
            try
            {
                code();
            }
            catch (Exception e)
            {
                var rootPath = Parameters.Script.ServerScriptFilePath.Replace('/', Path.DirectorySeparatorChar); ;
                var errMsg = $"{e.GetType().Name}@{e.Message.Replace(rootPath, "$(base)")}";
                if (!skipException(e))
                {
                    callback.InvokeAsFunction("Exception", errMsg);
                    logMsg = "failure:" + log + "/" + errMsg;
                }
                else
                {
                    logMsg = "skip:" + log + "/" + errMsg;
                }
            }
            new SysLogModel(
                context: Context,
                method: "Files",
                message: logMsg,
                sysLogType: SysLogModel.SysLogTypes.Info);
        }

        public string Script()
        {
            var code = """
                $ps.file =
                {
                    readAllText: function(section, path, encode=null)
                    {
                        return $ps._utils._f0((cb) => _file_cs.ReadAllText(cb, section, path, encode));
                    },
                    writeAllText: function(section, path, data, encode=null)
                    {
                        return $ps._utils._f0((cb) => _file_cs.WriteAllText(cb, section, path, data, encode));
                    },
                    getFileList: function(section, path)
                    {
                        return $ps._utils._f0((cb) => _file_cs.GetFileList(cb, section, path));
                    },
                    getDirectoryList: function(section, path)
                    {
                        return $ps._utils._f0((cb) => _file_cs.GetDirectoryList(cb, section, path));
                    },
                    removeFile: function(section, path)
                    {
                        return $ps._utils._f0((cb) => _file_cs.RemoveFile(cb, section, path));
                    },
                    moveFile: function(section, old_name, new_name)
                    {
                        return $ps._utils._f0((cb) => _file_cs.MoveFile(cb, section, old_name, new_name));
                    },
                    copyFile: function(section, source_name, dest_name)
                    {
                        return $ps._utils._f0((cb) => _file_cs.CopyFile(cb, section, source_name, dest_name));
                    },
                    removeDirectory: function(section, path)
                    {
                        return $ps._utils._f0((cb) => _file_cs.RemoveDirectory(cb, section, path));
                    },
                    moveDirectory: function(section, old_name, new_name)
                    {
                        return $ps._utils._f0((cb) => _file_cs.MoveDirectory(cb, section, old_name, new_name));
                    },
                    createDirectory: function(section, path)
                    {
                        return $ps._utils._f0((cb) => _file_cs.CreateDirectory(cb, section, path));
                    },
                    removeSection: function(section)
                    {
                        return $ps._utils._f0((cb) => _file_cs.RemoveSection(cb, section));
                    },
                    createSection: function(section)
                    {
                        return $ps._utils._f0((cb) => _file_cs.CreateSection(cb, section));
                    },
                    import: function(section, path, siteId, param)
                    {
                        return $ps._utils._f0((cb) => _file_cs.Import(cb, section, path, siteId, param));
                    },
                    export: function(section, path, siteId, param)
                    {
                        return $ps._utils._f0((cb) => _file_cs.Export(cb, section, path, siteId, param));
                    },
                }
            """;
            return code;
        }
    }
}
