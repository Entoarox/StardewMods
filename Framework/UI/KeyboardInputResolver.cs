using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using StardewValley;

namespace Entoarox.Framework.UI
{
    static class KeyboardInputResolver
    {
        /// <summary>
        /// This event is triggered when a given key is first pressed
        /// Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account
        /// </summary>
        public static event Action<Keys> KeyDown;
        /// <summary>
        /// This event is triggered for a key that is held down long enough to require a repeat firing
        /// Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account
        /// This will not fire every update, as a internal counter is used to keep repeat firing at a acceptable rate
        /// </summary>
        public static event Action<Keys> KeyHeld;
        /// <summary>
        /// This event is triggered when a given key is released
        /// Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account
        /// </summary>
        public static event Action<Keys> KeyUp;
        /// <summary>
        /// This event is trigger when a given key is first pressed or held
        /// It outputs the char for the intended value rather then the Keys enumeration for the actual key pressed
        /// </summary>
        public static event Action<char> CharReceived;
        // Private fields
        private static KeyboardState Old;
        private static Dictionary<Keys, int[]> Counter = new Dictionary<Keys, int[]>();
        private static bool Shift = false;
        private static bool Caps = false;
        private static bool Alt = false;
        // Initializer, as there is some init needed
        static KeyboardInputResolver()
        {
            // Shared logic - hook the update
            StardewModdingAPI.Events.GameEvents.UpdateTick += Update;
            // MonoGame logic
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // Using reflection to hook the event so that monogame is not required to compile
                // Also makes sure the reference sticks just in case mono rewrite goes heads up on it
                typeof(GameWindow).GetEvent("TextInput").AddEventHandler(Game1.game1.Window, (Action<object, EventArgs>)TextInputHandler);
                return;
            }
            // XNA logic
            KeyDown += KeyDownHandler;
            KeyUp += KeyUpHandler;
            KeyHeld += KeyHeldHandler;
        }
        // The method responsible for handling the update
        private static void Update(object s, EventArgs e)
        {
            KeyboardState New = Keyboard.GetState();
            Keys[] OldDown = Old.GetPressedKeys();
            Keys[] Down = New.GetPressedKeys().Where(a => !Old.IsKeyDown(a)).ToArray();
            Keys[] Up = OldDown.Where(a => !New.IsKeyDown(a)).ToArray();
            Keys[] Held = OldDown.Where(a => New.IsKeyDown(a)).ToArray();
            foreach (Keys key in Down)
            {
                KeyDown?.Invoke(key);
                // int[]{ticks,tickRate}
                Counter.Add(key, new int[2] { 30, 30 });
            }
            foreach (Keys key in Up)
            {
                Counter.Remove(key);
                KeyUp?.Invoke(key);
            }
            foreach (Keys key in Held)
            {
                Counter[key][0]--;
                if (Counter[key][0] > 0)
                    continue;
                Counter[key][0] = Counter[key][1];
                if (Counter[key][1] > 15)
                    Counter[key][1]--;
                KeyHeld?.Invoke(key);
            }
            Old = New;
        }
        // if XNA is being used, we need to do quite a bit of logic to make sure that the correct characters are output
        private static void KeyDownHandler(Keys key)
        {
            switch (key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    Shift = true;
                    Alt = true;
                    break;
                case Keys.CapsLock:
                    Caps = true;
                    Alt = true;
                    break;
            }
            CharReceived?.Invoke(ResolveChar(@key));
        }
        private static void KeyUpHandler(Keys key)
        {
            switch (key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    Shift = false;
                    Alt = Caps;
                    break;
                case Keys.CapsLock:
                    Caps = false;
                    Alt = Shift;
                    break;
            }
        }
        private static void KeyHeldHandler(Keys key)
        {
            CharReceived?.Invoke(ResolveChar(key));
        }
        private static char ResolveChar(Keys key)
        {
            char Char = (char)key;
            if (!Alt)
                return Char;
            short Pre = VkKeyScan(Char);
            uint Post = (uint)Pre & 0xFF;
            byte[] Arr = new byte[256];
            if (Shift)
                Arr[0x10] = 0x80;
            if (Caps)
                Arr[0x14] = 0x80;
            ToAscii(Post, Post, Arr, out uint Out, 0);
            return (char)Out;
        }
        [DllImport("user32.dll")]
        static extern short VkKeyScan(char c);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int ToAscii(
            uint uVirtKey,
            uint uScanCode,
            byte[] lpKeyState,
            out uint lpChar,
            uint flags
            );
        // If MonoGame is in use, we can easily take advantage of its build-in input handler
        private static void TextInputHandler(object s, EventArgs e)
        {
            CharReceived?.Invoke((char)e.GetType().GetField("Character").GetValue(e));
        }
    }
}
