using System;
using System.IO;
using Entoarox.Framework;
using StardewModdingAPI;

namespace Entoarox.XnbLoader
{
    public class XnbLoaderMod : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            VersionChecker.AddCheck("XnbLoader", new Version(1, 0, 6), "https://raw.githubusercontent.com/Entoarox/StardewMods/master/VersionChecker/XnbLoader.json");

            // get content path
            string path = Path.Combine(Helper.DirectoryPath, "ModContent");
            Directory.CreateDirectory(path);

            // load files
            this.Monitor.Log("Parsing `ModContent` for files to redirect the content manager to...", LogLevel.Info);
            int overrides = this.LoadOverrides(path, path);
            this.Monitor.Log($"Parsing complete, found and redirected {overrides} files", LogLevel.Info);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Recursively find XNBs and register them with the content registry.</summary>
        /// <param name="root">The root path being searched.</param>
        /// <param name="path">The path for which to load XNBs.</param>
        private int LoadOverrides(string root, string path)
        {
            int files = 0;

            // log path
            {
                string relativePath = path.Replace(root + Path.DirectorySeparatorChar, Path.DirectorySeparatorChar.ToString()).Replace(root, Path.DirectorySeparatorChar.ToString());
                this.Monitor.Log($"Scanning for files and directories in {relativePath}", LogLevel.Trace);
            }

            // load subfolders
            foreach (string dir in Directory.EnumerateDirectories(path))
                files += this.LoadOverrides(root, Path.Combine(path, dir));

            // load files
            foreach (string file in Directory.EnumerateFiles(path))
            {
                string filePath = Path.Combine(path, Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                string from = filePath.Replace(root + Path.DirectorySeparatorChar, "");
                this.Monitor.Log($"Redirecting: {from} ~> {filePath}.xnb", LogLevel.Trace);
                EntoFramework.GetContentRegistry().RegisterXnb(from, filePath);
                files++;
            }

            return files;
        }
    }
}