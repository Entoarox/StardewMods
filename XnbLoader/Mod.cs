namespace Entoarox.XnbLoader
{
    public class XnbLoaderMod : StardewModdingAPI.Mod
    {
        public override void Entry(StardewModdingAPI.IModHelper helper)
        {
            StardewModdingAPI.Events.GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
        }
        private string _Path;
        private void GameEvents_FirstUpdateTick(object s, object e)
        {
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
        }
        private void GameEvents_UpdateTick(object s, object e)
        {
            StardewModdingAPI.Events.GameEvents.UpdateTick -= GameEvents_UpdateTick;
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
                string from = filePath.Replace(_Path + System.IO.Path.PathSeparator.ToString(), "");
                Monitor.Log($"Redirecting: {from} ~> {filePath}.xnb", StardewModdingAPI.LogLevel.Trace);
                Framework.EntoFramework.GetContentRegistry().RegisterXnb(from, filePath);
            }
        }
    }
}