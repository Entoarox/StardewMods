using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;

namespace Entoarox.Framework.Core
{
    internal class UpdateHandler
    {
        /*********
        ** Accessors
        *********/
        public static Dictionary<IManifest, string> Map = new Dictionary<IManifest, string>();
        public string Latest;
        public string Recommended;
        public string Minimum;


        /*********
        ** Public methods
        *********/
        public static void DoUpdateChecks()
        {
            try
            {
                string version = typeof(Game1).Assembly.GetName().Version.ToString(2);
                Console.WriteLine("Stardew version:" + version);
                bool connected;
                try
                {
                    using (WebClient client = new WebClient())
                    using (Stream stream = client.OpenRead("http://www.google.com"))
                        connected = true;
                }
                catch
                {
                    connected = false;
                }

                if (connected)
                    Parallel.ForEach(UpdateHandler.Map, pair =>
                    {
                        try
                        {
                            WebClient client = new WebClient();
                            Uri uri = new Uri(pair.Value);
                            SemanticVersion modVersion = (SemanticVersion)pair.Key.Version;
                            try
                            {
                                client.DownloadStringCompleted += (sender, evt) =>
                                {
                                    try
                                    {
                                        if (evt.Error != null)
                                        {
                                            UpdateHandler.HandleError(pair.Key.Name, (WebException)evt.Error);
                                            return;
                                        }

                                        Dictionary<string, UpdateHandler> data = JsonConvert.DeserializeObject<Dictionary<string, UpdateHandler>>(evt.Result);
                                        UpdateHandler info = null;
                                        if (data.ContainsKey(version))
                                            info = data[version];
                                        else if (data.ContainsKey("Default"))
                                            info = data["Default"];
                                        else
                                            EntoaroxFrameworkMod.Logger.ExitGameImmediately($"[UpdateChecker] The {modVersion} version of the `{pair.Key.Name}` mod is not compatible with version {version} of Stardew Valley.");
                                        if (info != null)
                                        {
                                            SemanticVersion min = new SemanticVersion(info.Minimum);
                                            SemanticVersion rec = new SemanticVersion(info.Recommended);
                                            SemanticVersion max = new SemanticVersion(info.Latest);
                                            if (min.IsNewerThan(modVersion))
                                                EntoaroxFrameworkMod.Logger.ExitGameImmediately($"[UpdateChecker] The `{pair.Key.Name}` mod is too old, a newer version is required. Expected {min}, found {modVersion}.");
                                            else if (rec.IsNewerThan(modVersion))
                                                EntoaroxFrameworkMod.Logger.Log($"[UpdateChecker] Version {rec} of the `{pair.Key.Name}` mod is available, it is recommended you update now.", LogLevel.Alert);
                                            else if (max.IsNewerThan(modVersion))
                                                EntoaroxFrameworkMod.Logger.Log($"[UpdateChecker] Version {max} of the `{pair.Key.Name}` mod is available.", LogLevel.Info);
                                        }
                                    }
                                    catch (WebException err)
                                    {
                                        UpdateHandler.HandleError(pair.Key.Name, err);
                                    }
                                    catch (Exception err)
                                    {
                                        EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] The `" + pair.Key.Name + "` mod failed to check for updates, unexpected error occured while reading result, error message follows." + Environment.NewLine + err.ToString(), LogLevel.Error);
                                    }
                                };
                                client.DownloadStringAsync(uri);
                            }
                            catch (WebException err)
                            {
                                UpdateHandler.HandleError(pair.Key.Name, err);
                            }
                        }
                        catch (Exception err)
                        {
                            EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] The `" + pair.Key.Name + "` mod failed to check for updates, unexpected error occured, error message follows." + Environment.NewLine + err.ToString(), LogLevel.Error);
                        }
                    });
                else
                    EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] No internet connection, skipping update checks.", LogLevel.Debug);
            }
            catch (Exception err)
            {
                EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] Unexpected failure, unexpected error occured, error message follows." + Environment.NewLine + err, LogLevel.Error);
            }
        }


        /*********
        ** Protected methods
        *********/
        private static void HandleError(string name, WebException err)
        {
            switch (err.Status)
            {
                case WebExceptionStatus.ConnectFailure:
                    EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] The `" + name + "` mod failed to check for updates, connection failed.", LogLevel.Error);
                    break;

                case WebExceptionStatus.NameResolutionFailure:
                    EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] The `" + name + "` mod failed to check for updates, DNS resolution failed", LogLevel.Error);
                    break;

                case WebExceptionStatus.SecureChannelFailure:
                    EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] The `" + name + "` mod failed to check for updates, SSL handshake failed", LogLevel.Error);
                    break;

                case WebExceptionStatus.Timeout:
                    EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] The `" + name + "` mod failed to check for updates, Connection timed out", LogLevel.Error);
                    break;

                case WebExceptionStatus.TrustFailure:
                    EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] The `" + name + "` mod failed to check for updates, SSL certificate cannot be validated", LogLevel.Error);
                    break;

                case WebExceptionStatus.ProtocolError:
                    HttpWebResponse response = (HttpWebResponse)err.Response;
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            EntoaroxFrameworkMod.Logger.Log($"[UpdateChecker] The `{name}` mod failed to check for updates, The update url provided by the mod does not exist", LogLevel.Warn);
                            break;
                        case HttpStatusCode.RequestTimeout:
                            EntoaroxFrameworkMod.Logger.Log($"[UpdateChecker] The `{name}` mod failed to check for updates, Connection timed out", LogLevel.Warn);
                            break;
                        default:
                            EntoaroxFrameworkMod.Logger.Log($"[UpdateChecker] The `{name}` mod failed to check for updates, Server protocol error.\n\t[{response.StatusCode}]: {response.StatusDescription}", LogLevel.Error);
                            break;
                    }
                    break;

                default:
                    EntoaroxFrameworkMod.Logger.Log("[UpdateChecker] The `" + name + "` mod failed to check for updates, a unknown error occured." + Environment.NewLine + err, LogLevel.Error);
                    break;
            }
        }
    }
}
