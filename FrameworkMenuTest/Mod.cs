using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Entoarox.Framework;
using Entoarox.Framework.Events;
using Entoarox.Framework.Menus;

namespace Entoarox.FrameworkMenuTest
{
    public class FrameworkMenuTestMod : Mod
    {
        public override void Entry(params object[] objects)
        {
            MoreEvents.WorldReady += MoreEvents_WorldReady;
        }
        internal static void MoreEvents_WorldReady(object s, EventArgs e)
        {
            FrameworkMenu menu = new FrameworkMenu(new Point(256, 128));
            menu.AddComponent(new CheckboxFormComponent(new Point(0, 0), "Example Checkbox", 0, CheckboxChanged));
            menu.AddComponent(new PlusMinusFormComponent(new Point(0, 10), 0, 100, 0, PlusMinusChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 20), 3, 0, IntSliderChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 30), 12, 1, IntSliderChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 40), 20, 2, IntSliderChanged));
            Game1.activeClickableMenu = menu;
        }
        internal static void CheckboxChanged(int key, bool value)
        {
            Console.WriteLine("CheckBoxChanged: " + key.ToString() + " = " + (value ? "true" : "false"));
        }
        internal static void PlusMinusChanged(int key, int value)
        {
            Console.WriteLine("PlusMinusChanged: " + key.ToString() + " = " + value.ToString());
        }
        internal static void IntSliderChanged(int key, int value)
        {
            Console.WriteLine("IntSliderChanged: " + key.ToString() + " = " + value.ToString());
        }
    }
}
