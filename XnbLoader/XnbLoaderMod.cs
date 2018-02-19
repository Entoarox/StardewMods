using System;
using System.IO;
using System.Collections.Generic;
using Entoarox.Framework;
using StardewModdingAPI;

namespace Entoarox.XnbLoader
{
    public class XnbLoaderMod : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The name of the mod folder which contains the files to load.</summary>
        private readonly string ContentFolderName = "ModContent";

        /// <summary>The content paths to create when the mod starts.</summary>
        private readonly string[] PathsToCreate =
        {
            "Animals",
            "Buildings",
            Path.Combine("Characters", "Dialogue"),
            Path.Combine("Characters", "Farmer"),
            Path.Combine("Characters", "Monsters"),
            Path.Combine("Characters", "schedules"),
            Path.Combine("Data", "Events"),
            Path.Combine("Data", "Festivals"),
            Path.Combine("Data", "TV"),
            "Fonts",
            Path.Combine("LooseSprites", "Lighting"),
            "Maps",
            "Mines",
            "Minigames",
            "Portraits",
            "Strings",
            "TerrainFeatures",
            "TileSheets"
        };

        private Dictionary<string, string> Cache = new Dictionary<string, string>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Helper.RequestUpdateCheck("https://raw.githubusercontent.com/Entoarox/StardewMods/master/XnbLoader/update.json");

            // prepare directory structure
            string contentPath = Path.Combine(this.Helper.DirectoryPath, this.ContentFolderName);
            foreach(string path in this.PathsToCreate)
                Directory.CreateDirectory(Path.Combine(contentPath, path));

            // load files
            this.Monitor.Log("Parsing `ModContent` for files to redirect the content manager to...", LogLevel.Info);
            int overrides = this.LoadOverrides(contentPath, contentPath);
            this.PatchFiles();
            this.Monitor.Log($"Parsing complete, found and redirected [{overrides}] files", LogLevel.Info);
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
                string pathSeparator = Path.DirectorySeparatorChar.ToString();
                string relativePath = path.Replace(root + pathSeparator, pathSeparator).Replace(root, pathSeparator);
                this.Monitor.Log($"Looking for files and directories in {relativePath}", LogLevel.Trace);
            }

            // load subfolders
            foreach (string dir in Directory.EnumerateDirectories(path))
                files += this.LoadOverrides(root, Path.Combine(path, dir));

            // load files
            foreach (string file in Directory.EnumerateFiles(path))
            {
                if (Path.GetExtension(file).Equals(".xnb"))
                {
                    string filePath = Path.Combine(path, Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                    string from = filePath.Replace(root + Path.DirectorySeparatorChar, "");
                    if (!this.Cache.ContainsKey(from))
                    {
                        if (!File.Exists(Path.Combine(StardewValley.Game1.content.RootDirectory, from) + ".xnb"))
                            this.Monitor.Log($"Found a file that does not exist in the vanilla content, if this is intentional you can ignore this warning: {from}.xnb", LogLevel.Warn);
                        this.Cache.Add(from, Path.Combine("ModContent", from));
                        files++;
                    }
                    else
                        this.Monitor.Log($"Encountered duplicate file in ModContent, this shouldnt be here: {from}.xnb", LogLevel.Warn);
                }
                else
                    this.Monitor.Log($"Encountered non-xnb file in ModContent, this shouldnt be here: {Path.Combine(path, Path.GetDirectoryName(file), Path.GetFileName(file))}", LogLevel.Warn);

            }
            return files;
        }
        private void PatchFiles()
        {
            this.Monitor.Log("Redirect list:",LogLevel.Trace);
            foreach(KeyValuePair<string, string> entry in this.Cache)
                this.Monitor.Log($"  {entry.Key} ~> {entry.Value}.xnb", LogLevel.Trace);
            foreach (KeyValuePair<string, string> entry in this.Cache)
                this.Helper.Content.RegisterXnbReplacement(entry.Key, entry.Value);
        }
    }
}
