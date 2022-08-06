﻿using Implem.Libraries.Utilities;
using Implem.Plugins;
using System;
using System.Collections.Concurrent;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Pdf
{
    public static class PdfPluginCache
    {
        private static readonly ConcurrentDictionary<string, IPdfPlugin> plugins = new ConcurrentDictionary<string, IPdfPlugin>();

        public static IPdfPlugin LoadPdfPlugin(string libraryPath)
        {
            if (plugins.TryGetValue(libraryPath, out IPdfPlugin plugin))
            {
                return plugin;
            }
            var lib = System.IO.Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "UserPlugins",
                libraryPath);
            var assembly = System.Reflection.Assembly.LoadFrom(lib);
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => !t.IsInterface && typeof(IPdfPlugin).IsAssignableFrom(t));
            if (pluginType == null) return null;
            plugin = Activator.CreateInstance(pluginType) as IPdfPlugin;
            plugins.TryAdd(libraryPath, plugin);
            return plugin;
        }
    }
}
