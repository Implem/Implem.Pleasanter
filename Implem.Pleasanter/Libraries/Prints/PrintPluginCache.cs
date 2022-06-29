using System;
using System.Collections.Generic;
using System.Linq;
using Implem.Libraries.Utilities;
using Implem.Plugins;
using System.Collections.Concurrent;
namespace Implem.Pleasanter.Libraries.Prints
{
    public static class PrintPluginCache
    {
        private static readonly ConcurrentDictionary<string, IPrintPlugin> plugins
            = new ConcurrentDictionary<string, IPrintPlugin>();

        public static IPrintPlugin LoadPrintPlugin(string libraryName)
        {                      
            if(plugins.TryGetValue(libraryName, out IPrintPlugin plugin))
            {
                return plugin;
            }
            var lib = System.IO.Path.Combine(
                Environments.CurrentDirectoryPath,
                "App_Data",
                "Parameters",
                "ExtendedLibraries",
                libraryName);
            var assembly = System.Reflection.Assembly.LoadFrom(lib);
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => !t.IsInterface && typeof(IPrintPlugin).IsAssignableFrom(t));
            if (pluginType == null) { return null; }
            plugin = Activator.CreateInstance(pluginType) as IPrintPlugin;
            plugins.TryAdd(libraryName, plugin);
            return plugin;
        }
    }
}
