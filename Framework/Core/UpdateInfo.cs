using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

using StardewModdingAPI;

using Newtonsoft.Json;

using StardewValley;

namespace Entoarox.Framework.Core
{
    internal class UpdateInfo
    {
        public string Latest;
        public string Recommended;
        public string Minimum;

        public static Dictionary<IMod, string> Map = new Dictionary<IMod, string>();

        public static void DoUpdateChecks()
        {
            string version = typeof(Game1).Assembly.GetName().Version.ToString(2);
            bool Connected;
            try
            {
                using (WebClient client = new WebClient())
                using (Stream stream = client.OpenRead("http://www.google.com"))
                    Connected = true;
            }
            catch
            {
                Connected = false;
            }
            Parallel.ForEach(Map, (pair) => {
                if (Connected)
                {
                    WebClient Client = new WebClient();
                    Uri uri = new Uri(pair.Value);
                    SemanticVersion modVersion = (SemanticVersion)pair.Key.ModManifest.Version;
                    Client.DownloadStringCompleted += (sender, evt) =>
                    {
                        Dictionary<string, UpdateInfo> Data = JsonConvert.DeserializeObject<Dictionary<string, UpdateInfo>>(evt.Result);
                        UpdateInfo info = null;
                        if (Data.ContainsKey(version))
                            info = Data[version];
                        else if (Data.ContainsKey("Default"))
                            info = Data["Default"];
                        else
                            pair.Key.Monitor.ExitGameImmediately("Update check failed, the current version of Stardew Valley is not supported");
                        if(info!=null)
                        {
                            SemanticVersion min = new SemanticVersion(info.Minimum);
                            SemanticVersion rec = new SemanticVersion(info.Recommended);
                            SemanticVersion max = new SemanticVersion(info.Latest);
                            if (min.IsNewerThan(modVersion))
                                pair.Key.Monitor.ExitGameImmediately("Update required, the current mod version is below the allowed minimum");
                            if (rec.IsNewerThan(modVersion))
                                pair.Key.Monitor.Log("A new version is available, it is recommended you update now", LogLevel.Alert);
                            if(modVersion.IsBetween(rec,max))
                                pair.Key.Monitor.Log("A new version is available, you can choose to update to this version now", LogLevel.Info);
                        }

                    };
                    try
                    {
                        Client.DownloadStringAsync(uri);
                    }
                    catch (WebException err)
                    {
                        switch (err.Status)
                        {
                            case WebExceptionStatus.ConnectFailure:
                                pair.Key.Monitor.Log("Unable to check for updates, connection failed", LogLevel.Error);
                                break;
                            case WebExceptionStatus.NameResolutionFailure:
                                pair.Key.Monitor.Log("Unable to check for updates, DNS resolution failed", LogLevel.Error);
                                break;
                            case WebExceptionStatus.SecureChannelFailure:
                                pair.Key.Monitor.Log("Unable to check for updates, SSL handshake failed", LogLevel.Error);
                                break;
                            case WebExceptionStatus.Timeout:
                                pair.Key.Monitor.Log("Unable to check for updates, Connection timed out", LogLevel.Error);
                                break;
                            case WebExceptionStatus.TrustFailure:
                                pair.Key.Monitor.Log("Unable to check for updates, SSL certificate cannot be validated", LogLevel.Error);
                                break;
                            case WebExceptionStatus.ProtocolError:
                                HttpWebResponse response = (HttpWebResponse)err.Response;
                                pair.Key.Monitor.Log($"Unable to check for updates, Server protocol error.\n\t[{response.StatusCode}]: {response.StatusDescription}", LogLevel.Error);
                                break;
                            default:
                                pair.Key.Monitor.Log("Unable to check for updates, unknown error occured"+Environment.NewLine+err.ToString(), LogLevel.Error);
                                break;
                        }
                    }
                }
                else
                {
                    pair.Key.Monitor.Log("No internet connection, skipping update check.", LogLevel.Debug);
                }
            });
        }
    }
}