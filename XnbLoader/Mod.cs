using System;
using System.IO;

using StardewModdingAPI;

namespace Entoarox.XnbLoader
{
    using Framework;
    using Framework.Events;
    public class XnbLoaderMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            if (EntoFramework.Version < new Version(1, 6, 5))
                throw new DllNotFoundException("A newer version of EntoaroxFramework.dll is required as the currently installed one is to old for XnbLoader to use.");
            EntoFramework.VersionRequired("XnbLoader", new Version(1, 6, 6));
            VersionChecker.AddCheck("XnbLoader", new Version(ModManifest.Version.MajorVersion, ModManifest.Version.MinorVersion, ModManifest.Version.PatchVersion), "https://raw.githubusercontent.com/Entoarox/StardewMods/master/VersionChecker/XnbLoader.json");
            MoreEvents.SmartManagerReady += MoreEvents_SmartManagerReady;
        }
        private string _Path;
        private int Files = 0;
        private void MoreEvents_SmartManagerReady(object s, object e)
        {
            MoreEvents.SmartManagerReady -= MoreEvents_SmartManagerReady;
            _Path = Path.Combine(Helper.DirectoryPath, "ModContent","");
            Directory.CreateDirectory(_Path);
            Monitor.Log("Parsing `ModContent` for files to redirect the content manager to...", LogLevel.Info);
            ParseDir(_Path);
            Monitor.Log("Reloading static content references...", LogLevel.Trace);
            EntoFramework.GetContentRegistry().ReloadStaticReferences();
            Monitor.Log($"Parsing complete, found and redirected [{Files}] files", LogLevel.Info);
        }
        private void ParseDir(string path)
        {
            string separator = Path.DirectorySeparatorChar.ToString();
            Monitor.Log("Scanning for files and directories in " + path.Replace(_Path + separator, separator).Replace(_Path, separator), LogLevel.Trace);
            foreach (string dir in Directory.EnumerateDirectories(path))
                ParseDir(Path.Combine(path, dir));
            foreach (string file in Directory.EnumerateFiles(path))
            {
                string filePath = Path.Combine(path, Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                string from = filePath.Replace(_Path+separator, "");
                Monitor.Log($"Redirecting: {from} ~> {filePath}.xnb", LogLevel.Trace);
                Files++;
                EntoFramework.GetContentRegistry().RegisterXnb(from, filePath);
            }
        }
    }
}