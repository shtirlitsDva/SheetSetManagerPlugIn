using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using static SheetSetManager.Utils;

namespace SheetSetManager
{
    internal class DebugHelper
    {
#if DEBUG
        public static Assembly Debug_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyFolder = @"X:\GitHub\shtirlitsDva\SheetSetManagerPlugIn\ABF_SheetSetManager\bin\Debug";
            prdDbg($"Asked for assembly: {args.Name}!");

            var name = args.Name.Split(',')[0];
            name += ".dll";
            string filePath = Path.Combine(assemblyFolder, name);
            if (File.Exists(filePath)) return Assembly.LoadFrom(filePath);
            else { prdDbg($"File not found: {filePath}!"); return null; }
        }
#endif
    }
}
