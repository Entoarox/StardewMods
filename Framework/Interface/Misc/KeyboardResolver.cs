using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;

namespace Entoarox.Framework.Interface
{
    internal static class KeyboardResolver
    {
        /*********
        ** Fields
        *********/
        private static bool Alt;
        private static bool Caps;
        private static readonly Dictionary<Keys, int[]> Counter = new Dictionary<Keys, int[]>();
        private static KeyboardState Old;
        private static bool Shift;


        /*********
        ** Accessors
        *********/
        /// <summary>This event is triggered when a given key is first pressed. Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account.</summary>
        public static event Action<Keys> KeyDown;

        /// <summary>This event is triggered for a key that is held down long enough to require a repeat firing. Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account. This will not fire every update, as a internal counter is used to keep repeat firing at a acceptable rate.</summary>
        public static event Action<Keys> KeyHeld;

        /// <summary>This event is triggered when a given key is released. Note that this uses the XNA Keys enumeration, thus alternative (shift/caps) values are not taken into account.</summary>
        public static event Action<Keys> KeyUp;

        /// <summary>This event is triggered when a given key counts as having been pressed. This event is essentially like subscribing to both <see cref="KeyDown" /> and <see cref="KeyHeld" /> with a single method.</summary>
        public static event Action<Keys> KeyPress;

        /// <summary>This event is trigger when a given key is first pressed or held. It outputs the char for the intended value rather then the Keys enumeration for the actual key pressed.</summary>
        public static event Action<char> CharReceived;


        /*********
        ** Public methods
        *********/
        /// <summary>Initializer, as there is some init needed.</summary>
        static KeyboardResolver()
        {
            // Shared logic - hook the update
            GameEvents.UpdateTick += KeyboardResolver.Update;
            // MonoGame logic
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                typeof(GameWindow).GetEvent("TextInput").AddEventHandler(Game1.game1.Window, (Action<object, EventArgs>)KeyboardResolver.TextInputHandler);
            else
            {
                // XNA logic
                KeyboardResolver.KeyDown += KeyboardResolver.KeyDownHandler;
                KeyboardResolver.KeyUp += KeyboardResolver.KeyUpHandler;
                KeyboardResolver.KeyHeld += KeyboardResolver.KeyHeldHandler;
            }
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>The method responsible for handling the update.</summary>
        private static void Update(object s, EventArgs e)
        {
            KeyboardState New = Keyboard.GetState();
            Keys[] oldDown = KeyboardResolver.Old.GetPressedKeys();
            Keys[] down = New.GetPressedKeys().Where(a => !KeyboardResolver.Old.IsKeyDown(a)).ToArray();
            Keys[] up = oldDown.Where(a => !New.IsKeyDown(a)).ToArray();
            Keys[] held = oldDown.Where(a => New.IsKeyDown(a)).ToArray();
            foreach (Keys key in down)
            {
                KeyboardResolver.KeyDown?.Invoke(key);
                KeyboardResolver.KeyPress?.Invoke(key);
                // int[]{ticks,tickRate}
                KeyboardResolver.Counter.Add(key, new[] { 30, 30 });
            }

            foreach (Keys key in up)
            {
                KeyboardResolver.Counter.Remove(key);
                KeyboardResolver.KeyUp?.Invoke(key);
            }

            foreach (Keys key in held)
            {
                KeyboardResolver.Counter[key][0]--;
                if (KeyboardResolver.Counter[key][0] > 0)
                    continue;
                KeyboardResolver.Counter[key][0] = KeyboardResolver.Counter[key][1];
                if (KeyboardResolver.Counter[key][1] > 15)
                    KeyboardResolver.Counter[key][1]--;
                KeyboardResolver.KeyHeld?.Invoke(key);
                KeyboardResolver.KeyPress?.Invoke(key);
            }

            KeyboardResolver.Old = New;
        }

        /// <summary>If XNA is being used, we need to do quite a bit of logic to make sure that the correct characters are output.</summary>
        private static void KeyDownHandler(Keys key)
        {
            switch (key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    KeyboardResolver.Shift = true;
                    KeyboardResolver.Alt = true;
                    break;
                case Keys.CapsLock:
                    KeyboardResolver.Caps = true;
                    KeyboardResolver.Alt = true;
                    break;
            }

            KeyboardResolver.CharReceived?.Invoke(KeyboardResolver.ResolveChar(key));
        }

        private static void KeyUpHandler(Keys key)
        {
            switch (key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    KeyboardResolver.Shift = false;
                    KeyboardResolver.Alt = KeyboardResolver.Caps;
                    break;
                case Keys.CapsLock:
                    KeyboardResolver.Caps = false;
                    KeyboardResolver.Alt = KeyboardResolver.Shift;
                    break;
            }
        }

        private static void KeyHeldHandler(Keys key)
        {
            KeyboardResolver.CharReceived?.Invoke(KeyboardResolver.ResolveChar(key));
        }

        private static char ResolveChar(Keys key)
        {
            char @char = (char)key;
            if (!KeyboardResolver.Alt)
                return @char;
            short pre = KeyboardResolver.VkKeyScan(@char);
            uint post = (uint)pre & 0xFF;
            byte[] arr = new byte[256];
            if (KeyboardResolver.Shift)
                arr[0x10] = 0x80;
            if (KeyboardResolver.Caps)
                arr[0x14] = 0x80;
            KeyboardResolver.ToAscii(post, post, arr, out uint @out, 0);
            return (char)@out;
        }

        private static void TextInputHandler(object s, EventArgs e)
        {
            KeyboardResolver.CharReceived?.Invoke((char)e.GetType().GetField("Character").GetValue(e));
        }

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char c);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, out uint lpChar, uint flags); // If MonoGame is in use, we can easily take advantage of its build-in input handler.
    }
}
