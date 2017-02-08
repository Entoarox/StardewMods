namespace Entoarox.XnbLoader
{
    public class XnbLoaderMod : StardewModdingAPI.Mod
    {
        public override void Entry(StardewModdingAPI.IModHelper helper)
        {
            StardewModdingAPI.Events.GameEvents.LoadContent += GameEvents_LoadContent;
        }
        private string _Path;
        private void GameEvents_LoadContent(object s, object e)
        {
            _Path = System.IO.Path.Combine(Helper.DirectoryPath, "ModContent");
            System.IO.Directory.CreateDirectory(_Path);
            Monitor.Log("Parsing `ModContent` for files to redirect the content manager to...", StardewModdingAPI.LogLevel.Info);
            ParseDir(_Path);
            Monitor.Log("Parsing complete, all files redirected", StardewModdingAPI.LogLevel.Info);
        }
        private void ParseDir(string path)
        {
            Monitor.Log("Scanning for files and directories in " + path, StardewModdingAPI.LogLevel.Trace);
            foreach (string dir in System.IO.Directory.EnumerateDirectories(path))
                ParseDir(System.IO.Path.Combine(path, dir));
            foreach (string file in System.IO.Directory.EnumerateFiles(path))
            {
                string filePath = System.IO.Path.Combine(path, System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file));
                string from = filePath.Replace(_Path + System.IO.Path.PathSeparator, "");
                Monitor.Log($"Redirecting: {from} ~> {filePath}.xnb", StardewModdingAPI.LogLevel.Trace);
                Framework.EntoFramework.GetContentRegistry().RegisterXnb(from, filePath);
            }
        }
    }
}