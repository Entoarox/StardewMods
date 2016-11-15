using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Entoarox.Framework.Events;
using Entoarox.Framework.Menus;

namespace Entoarox.FrameworkMenuTest
{
    public class FrameworkMenuTestMod : Mod
    {
        internal static ProgressbarComponent prog=new ProgressbarComponent(new Point(105, 20), 0, 40);
        public override void Entry(params object[] objects)
        {
            MoreEvents.WorldReady += MoreEvents_WorldReady;
        }
        internal static void MoreEvents_WorldReady(object s, EventArgs e)
        {
            GameEvents.HalfSecondTick += GameEvents_HalfSecondTick;
            FrameworkMenu menu = new FrameworkMenu(new Point(256, 128+32));
            menu.AddComponent(new LabelComponent(new Point(0, -16), "Component Examples"));
            menu.AddComponent(new CheckboxFormComponent(new Point(0, 2), "Example Checkbox", CheckboxChanged));
            menu.AddComponent(new PlusMinusFormComponent(new Point(0, 12), 0, 100, PlusMinusChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 22), 3, IntSliderChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 32), 12, IntSliderChanged));
            menu.AddComponent(new SliderFormComponent(new Point(0, 42), 20, IntSliderChanged));
            menu.AddComponent(new DropdownFormComponent(new Point(0, 51), new List<string>() {"First","Second","Third","Fourth"}, DropdownChanged));
            menu.AddComponent(new DropdownFormComponent(new Point(0, 65), new List<string>() { "#1", "#2", "#3", "#4", "#5", "#6", "#7", "#8", "#9", "#10","#11","#12" }, DropdownChanged));
            menu.AddComponent(new ButtonFormComponent(new Point(0, 78), "Test Button", ButtonClicked));
            menu.AddComponent(new TextboxFormComponent(new Point(0, 90), TextboxChanged));
            menu.AddComponent(new TextboxFormComponent(new Point(0, 105), TextboxChanged));
            menu.AddComponent(new HeartsComponent(new Point(105, 0), 3, 10));
            menu.AddComponent(new ClickableHeartsComponent(new Point(105, 10), 8, 10, HeartsChanged));
            menu.AddComponent(prog);
            Game1.activeClickableMenu = menu;
        }
        private static int Skipped = 0;
        internal static void GameEvents_HalfSecondTick(object s, EventArgs e)
        {
            if (prog.Value == 40)
            {
                if (Skipped>5)
                    prog.Value = 0;
                Skipped++;
            }
            else if(Skipped>10)
            {
                Skipped = 0;
            }
            else
                prog.Value += 4;
            if (Skipped > 0)
                Skipped++;
        }
        internal static void CheckboxChanged(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, bool value)
        {
            Console.WriteLine("CheckBoxChanged: " + (value ? "true" : "false"));
        }
        internal static void PlusMinusChanged(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, int value)
        {
            Console.WriteLine("PlusMinusChanged: " + value.ToString());
        }
        internal static void IntSliderChanged(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, int value)
        {
            Console.WriteLine("IntSliderChanged: " + value.ToString());
        }
        internal static void HeartsChanged(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, int value)
        {
            Console.WriteLine("HeartsChanged: " + value.ToString());
        }
        internal static void DropdownChanged(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, string value)
        {
            Console.WriteLine("DropdownChanged: " + value);
        }
        internal static void TextboxChanged(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, string value)
        {
            Console.WriteLine("TextboxChanged: " + value);
        }
        internal static void ButtonClicked(IInteractiveMenuComponent component, IComponentCollection collection, FrameworkMenu menu, bool left)
        {
            Console.WriteLine("ButtonClicked");
        }
    }
}
