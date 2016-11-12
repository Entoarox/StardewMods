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
            menu.AddComponent(new CheckboxFormComponent(new Point(0, 0), "Example Checkbox", CheckboxChanged));
            menu.AddComponent(new PlusMinusFormComponent(new Point(0, 10), 0, 100, PlusMinusChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 20), 3, IntSliderChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 30), 12, IntSliderChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 40), 20, IntSliderChanged));
            menu.AddComponent(new DropdownFormComponent(new Point(0, 50), new List<string>() {"First","Second","Third","Fourth"}, DropdownChanged));
            menu.AddComponent(new DropdownFormComponent(new Point(0, 65), new List<string>() { "#1", "#2", "#3", "#4", "#5", "#6", "#7", "#8", "#9", "#10","#11","#12" }, DropdownChanged));
            menu.AddComponent(new ButtonFormComponent(new Point(0, 78), "Test Button", ButtonClicked));
            menu.AddComponent(new TextboxFormComponent(new Point(0, 90), TextboxChanged));
            Game1.activeClickableMenu = menu;
        }
        internal static void CheckboxChanged(BaseFormComponent component, IComponentCollection collection, FrameworkMenu menu, bool value)
        {
            Console.WriteLine("CheckBoxChanged: " + (value ? "true" : "false"));
        }
        internal static void PlusMinusChanged(BaseFormComponent component, IComponentCollection collection, FrameworkMenu menu, int value)
        {
            Console.WriteLine("PlusMinusChanged: " + value.ToString());
        }
        internal static void IntSliderChanged(BaseFormComponent component, IComponentCollection collection, FrameworkMenu menu, int value)
        {
            Console.WriteLine("IntSliderChanged: " + value.ToString());
        }
        internal static void DropdownChanged(BaseFormComponent component, IComponentCollection collection, FrameworkMenu menu, string value)
        {
            Console.WriteLine("DropdownChanged: " + value);
        }
        internal static void TextboxChanged(BaseFormComponent component, IComponentCollection collection, FrameworkMenu menu, string value)
        {
            Console.WriteLine("TextboxChanged: " + value);
        }
        internal static void ButtonClicked(BaseFormComponent component, IComponentCollection collection, FrameworkMenu menu)
        {
            Console.WriteLine("ButtonClicked");
        }
    }
}
