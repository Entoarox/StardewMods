using System;
using System.IO;

using StardewModdingAPI;

using Entoarox.Framework;

namespace Entoarox.XnbLoader
{
    using Framework;
    public class XnbLoaderMod : Mod
    {
        private string _Path;
        private int Files = 0;
        public override void Entry(IModHelper helper)
        {
            Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/StardewMods/master/XnbLoader/update.json");
            _Path = Path.Combine(Helper.DirectoryPath, "ModContent", "");
            Directory.CreateDirectory(_Path);
            Monitor.Log("Parsing `ModContent` for files to redirect the content manager to...", LogLevel.Info);
            ParseDir(_Path);
            Monitor.Log("Reloading static content references...", LogLevel.Trace);
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
                Helper.Content.RegisterXnbReplacement(from, filePath);
            }
        }
    }
}