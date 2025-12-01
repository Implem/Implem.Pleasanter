using System;
using System.Diagnostics;

namespace Implem.PleasanterSetup.Utilities
{
    internal static class UrlOpener
    {
        public static bool Open(string url, string message = null,bool isAzure = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                if (!string.IsNullOrEmpty(message)) Console.WriteLine(message);
                return false;
            }
            if (isAzure)
            {
                if (!string.IsNullOrEmpty(message)) Console.WriteLine(message);
                return false;
            }
            try
            {
                if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    };
                    var started = Process.Start(psi);
                    if (started != null)
                    {
                        if (!string.IsNullOrEmpty(message)) Console.WriteLine(message);
                        return true;
                    }
                }
                else
                {
                    if (!HasGuiOnLinux(out _))
                    {
                        if (!string.IsNullOrEmpty(message)) Console.WriteLine(message);
                        return false;
                    }
                    var psi = new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = url,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    var proc = Process.Start(psi);
                    if (proc != null)
                    {
                        if (!string.IsNullOrEmpty(message)) Console.WriteLine(message);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {            
                Console.Error.WriteLine($"UrlOpener.Open failed: {ex.Message}");
            }

            if (!string.IsNullOrEmpty(message)) Console.WriteLine(message);
            return false;
        }
        private static bool HasGuiOnLinux(out string reason)
        {
            try
            {
                var xdgSessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
                if (!string.IsNullOrEmpty(xdgSessionType))
                {
                    var t = xdgSessionType.Trim().ToLowerInvariant();
                    if (t == "x11" || t == "wayland")
                    {
                        reason = $"XDG_SESSION_TYPE='{xdgSessionType}'";
                        return true;
                    }
                    reason = $"XDG_SESSION_TYPE='{xdgSessionType}' (not x11/wayland)";
                    return false;
                }
                reason = "XDG_SESSION_TYPE not set";
            }
            catch
            {
                reason = "Exception reading XDG_SESSION_TYPE";
            }
            return false;
        }
    }
}