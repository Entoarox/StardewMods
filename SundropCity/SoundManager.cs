using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework.Audio;

namespace SundropCity
{
    class SoundManager : IDisposable
    {
        private readonly Dictionary<string, SoundEffectInstance> Library = new Dictionary<string, SoundEffectInstance>();
        private SoundEffectInstance Instance;
        public SoundManager()
        {
            foreach (string file in Directory.EnumerateFiles(Path.Combine(SundropCityMod.SHelper.DirectoryPath, "assets", "Sounds")).Where(_ => Path.GetExtension(_).Equals(".mp3")))
            {
                SundropCityMod.SMonitor.Log("Loading sound file: " + file.Replace(SundropCityMod.SHelper.DirectoryPath + Path.DirectorySeparatorChar, ""));
                string name = Path.GetFileNameWithoutExtension(file);
                if (this.Library.ContainsKey(name))
                    continue;
                try
                {
                    var effect = SoundEffect.FromStream(File.OpenRead(file));
                }
                catch(Exception err)
                {
                    SundropCityMod.SMonitor.Log("Encountered a error loading sound file `" + name + "`, error follows.\n" + err, StardewModdingAPI.LogLevel.Error);
                }
            }
        }
        public void Play(bool looping = false)
        {
            this.Instance.IsLooped = looping;
            this.Instance.Play();
        }
        public void Play(string effect, bool looping = false)
        {
            this.Instance = this.Library[effect];
            this.Play(looping);
        }
        public void Stop(bool immediate = false)
        {
            this.Instance?.Stop(immediate);
        }

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    foreach (var file in this.Library.Values)
                        file.Dispose();
                }

                this.disposedValue = true;
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion
    }
}
