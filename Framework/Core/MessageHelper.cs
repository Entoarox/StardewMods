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
        /*********
        ** Fields
        *********/
        private static readonly int maxMessages = 4;
        private static readonly List<Message> messages = new List<Message>();


        /*********
        ** Public methods
        *********/
        public MessageHelper()
            : base(Game1.viewport.Width / 2 - Game1.tileSize * 12 / 2 - Game1.tileSize, Game1.viewport.Height - Game1.tileSize * 2 - Game1.tileSize / 2, Game1.tileSize * 14, 56, false)
        {
            Game1.onScreenMenus.Add(this);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) { }

        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        public override void performHoverAction(int x, int y) { }

        public override void clickAway() { }

        void IMessageHelper.Add(string message, string name, Color color)
        {
            string text = Game1.parseText(name != null ? $"[{name}[ {message}" : message, Game1.smallFont, 480);
            int size = (int)Game1.smallFont.MeasureString(text).Y;
            Console.WriteLine($"{text} : {size}");
            Message chatMessage = new Message
            {
                Text = text,
                Time = 600,
                Size = size,
                Color = color
            };
            MessageHelper.messages.Add(chatMessage);
            if (MessageHelper.messages.Count > MessageHelper.maxMessages * 5)
                MessageHelper.messages.RemoveAt(0);
        }

        void IMessageHelper.Add(string message, Color color)
        {
            (this as IMessageHelper).Add(message, null, color);
        }

        public override void draw(SpriteBatch b)
        {
            if (!MessageHelper.messages.Any())
                return;

            int width = (int)Game1.smallFont.MeasureString(MessageHelper.messages.OrderByDescending(msg => Game1.smallFont.MeasureString(msg.Text).X).First().Text).X;
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), 6, 6, width + 12, MessageHelper.messages.Sum(t => t.Size + 2) + 10, Color.White * MessageHelper.messages.Last().Alpha);

            int num = 0;
            for (int index = 0; index < MessageHelper.maxMessages; index++)
            {
                if (index > 0)
                    num += MessageHelper.messages[index - 1].Size + 2;
                Utility.drawTextWithShadow(b, MessageHelper.messages[index].Text, Game1.smallFont, new Vector2(16f, 16 + num), MessageHelper.messages[index].Color * MessageHelper.messages[index].Alpha, 1f, 0.99f);
                MessageHelper.messages[index].Time--;
                if (MessageHelper.messages[index].Time < 75)
                    MessageHelper.messages[index].Alpha = MessageHelper.messages[index].Time / 75f;
            }

            MessageHelper.messages.Where(msg => Math.Abs(msg.Alpha) < 0.05f).ToList().ForEach(msg => MessageHelper.messages.Remove(msg));
        }
    }
}
