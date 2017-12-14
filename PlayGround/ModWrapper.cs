using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Microsoft.Xna.Framework.Graphics;

using xTile;
using xTile.Layers;
using xTile.Tiles;

using StardewValley;

namespace PlayGround
{
    class ModWrapper : Mod
    {
        public override void Entry(IModHelper helper)
        {
            this.Helper.ConsoleCommands.Add("pg_debug", "pg_debug <action>", (cmd, args) =>
            {
                switch (args[0])
                {
                    case "pooltest":
                        var map = this.Helper.Content.Load<Map>("map.tbin");
                        map.RemoveLayer(map.GetLayer("AlwaysFront"));
                        Texture2D texture = null;
                        try
                        {
                            texture = this.Helper.Content.Load<Texture2D>("steam.png");
                        }
                        catch
                        {
                            texture = null;
                        }
                        Game1.locations.Add(new CustomBathhouse(map, "CustomBathhouse", texture));
                        Game1.warpFarmer("CustomBathhouse", 15, 5, false);
                        break;
                }
            });
        }
    }
}
