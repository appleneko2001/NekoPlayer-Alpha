using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NekoPlayer.Core.Resources
{
    public static class ResourceManager
    {
        public static Stream ExtractData(string path)
        {
            var assembly = Assembly.GetAssembly(typeof(ResourceManager));
            var list = assembly.GetManifestResourceNames();
            var name = list.Single(str => str.EndsWith(path, StringComparison.InvariantCulture));//new AssemblyName(assembly.FullName);
            return assembly.GetManifestResourceStream(name);
        }
    }
}
