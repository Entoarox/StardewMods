using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework.Core
{
    internal class MessageHelper : IClickableMenu, IMessageHelper
    {
        private static List<Message> messages = new List<Message>();
        private static int maxMessages = 4;

        internal MessageHelper()
            : base(Game1.viewport.Width / 2 - Game1.tileSize * 12 / 2 - Game1.tileSize, Game1.viewport.Height - Game1.tileSize * 2 - Game1.tileSize / 2, Game1.tileSize * 14, 56, false)
        {
            Game1.onScreenMenus.Add(this);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
        }

        public override void clickAway()
        {
        }

        void IMessageHelper.Add(string message, string name, Color color)
        {
            string text = Game1.parseText(name != null ? $"[{name}[ {message}" : message, Game1.smallFont, 480);
            int size = (int)Game1.smallFont.MeasureString(text).Y;
            Console.WriteLine($"{text} : {size}");
            var chatMessage = new Message()
            {
                Text = text,
                Time = 600,
                Size = size,
                Color=color
            };
            messages.Add(chatMessage);
            if (messages.Count > maxMessages * 5)
                messages.RemoveAt(0);
        }
        void IMessageHelper.Add(string message, Color color) => (this as IMessageHelper).Add(message, null, color);
        public override void draw(SpriteBatch b)
        {
            if (messages.Any())
            {
                int width = (int)Game1.smallFont.MeasureString(messages.OrderByDescending(msg => Game1.smallFont.MeasureString(msg.Text).X).First().Text).X;
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), 6, 6, width + 12, (messages.Sum(t => t.Size + 2) + 10), Color.White * messages.Last().Alpha);

                int num = 0;
                for (int index = 0; index < maxMessages; index++)
                {
                    if (index > 0)
                        num += messages[index - 1].Size + 2;
                    Utility.drawTextWithShadow(b, messages[index].Text, Game1.smallFont, new Vector2(16f, 16 + num), messages[index].Color * messages[index].Alpha, 1f, 0.99f);
                    messages[index].Time--;
                    if (messages[index].Time < 75)
                        messages[index].Alpha = messages[index].Time / 75f;
                }
                messages.Where(msg => Math.Abs(msg.Alpha) < 0.05f).ToList().ForEach(msg => messages.Remove(msg));
            }
        }
    }
}
