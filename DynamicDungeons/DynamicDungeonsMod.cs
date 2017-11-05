using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Entoarox.DynamicDungeons
{
    public class DynamicDungeonsMod : Mod
    {
        internal static IMonitor SMonitor;
        private DungeonBuilder _Builder;
        private Texture2D _BuilderMinimap;
        public override void Entry(IModHelper helper)
        {
            SMonitor = this.Monitor;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
        }
        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if(Game1.graphics!=null && Game1.graphics.GraphicsDevice!=null)
            {
                this._Builder = new DungeonBuilder();
                this._BuilderMinimap = this._Builder.GetMiniMap();
                GameEvents.UpdateTick -= this.GameEvents_UpdateTick;
                GraphicsEvents.OnPostRenderEvent += this.GraphicsEvents_OnPostRenderEvent;
                InputEvents.ButtonReleased += this.InputEvents_ButtonReleased;
            }
        }
        private void GraphicsEvents_OnPostRenderEvent(object s, EventArgs e)
        {
            int scale = 8;
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(0, 0, this._BuilderMinimap.Bounds.Width * scale + 4 * scale, this._BuilderMinimap.Bounds.Height * scale + 4 * scale), null, Color.Black);
            Game1.spriteBatch.Draw(this._BuilderMinimap, new Vector2(scale * 2, scale * 2), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        }
        private void InputEvents_ButtonReleased(object s, EventArgsInput e)
        {
            if (e.Button == StardewModdingAPI.Utilities.SButton.F5)
            {
                this.Monitor.Log("Generating new dungeon...", LogLevel.Alert);
                this._Builder = new DungeonBuilder();
                this._BuilderMinimap = this._Builder.GetMiniMap();
                this.Monitor.Log("Generation completed!", LogLevel.Alert);
            }
        }
    }
}
