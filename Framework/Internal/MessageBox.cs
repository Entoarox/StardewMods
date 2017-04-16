using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.Framework
{
    internal class MessageBox : IClickableMenu, IMessageBox
    {
        private static List<Message> messages = new List<Message>();
        private static int maxMessages = 4;
        internal static IMessageBox Singleton;

        internal MessageBox()
            : base(Game1.viewport.Width / 2 - Game1.tileSize * 12 / 2 - Game1.tileSize, Game1.viewport.Height - Game1.tileSize * 2 - Game1.tileSize / 2, Game1.tileSize * 14, 56, false)
        {
        }

        internal static void Setup()
        {
            Singleton = new MessageBox();
            Game1.onScreenMenus.Add(Singleton as MessageBox);
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

        void IMessageBox.receiveMessage(string message, string name, Color color)
        {
            var chatMessage = new Message();
            var msg = name != null ? $"[{name}[ {message}" : message;
            chatMessage.message = Game1.parseText(msg, Game1.smallFont, 480);
            chatMessage.timeLeftToDisplay = 600;
            Console.WriteLine($"{chatMessage.message} : {(int)Game1.smallFont.MeasureString(chatMessage.message).Y}");
            chatMessage.verticalSize = (int)Game1.smallFont.MeasureString(chatMessage.message).Y;
            chatMessage.textColor = color;

            messages.Add(chatMessage);
            if (messages.Count < maxMessages)
                return;
            messages.RemoveAt(0);
        }
        void IMessageBox.receiveMessage(string message, Color color)
        {
            (this as IMessageBox).receiveMessage(message, null, color);
        }

        internal void update()
        {
            foreach (Message msg in messages)
            {
                if (msg.timeLeftToDisplay > 0)
                    --msg.timeLeftToDisplay;
                if (msg.timeLeftToDisplay < 75)
                    msg.alpha = msg.timeLeftToDisplay / 75f;
            }
            messages.Where(msg => Math.Abs(msg.alpha) < 0.05f).ToList().ForEach(msg => messages.Remove(msg));
        }

        public override void draw(SpriteBatch b)
        {
            if (messages.Any())
            {
                int width = (int)Game1.smallFont.MeasureString(messages.OrderByDescending(msg => Game1.smallFont.MeasureString(msg.message).X).First().message).X;
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), 6, 6, width + 12, (messages.Sum(t => t.verticalSize + 2) + 10), Color.White * messages.Last().alpha);

                int num = 0;
                for (int index = 0; index < messages.Count; index++)
                {
                    if (index > 0)
                        num += messages[index - 1].verticalSize + 2;
                    Utility.drawTextWithShadow(b, messages[index].message, Game1.smallFont, new Vector2(16f, 16 + num), messages[index].textColor * messages[index].alpha, 1f, 0.99f);
                }
                update();
            }
        }
    }
}
