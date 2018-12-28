using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Entoarox.Framework.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;

namespace Entoarox.Framework.UI
{
    internal static class KeyboardInputResolver
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

        /// <summary>This event is trigger when a given key is first pressed or held. It outputs the char for the intended value rather then the Keys enumeration for the actual key pressed.</summary>
        public static event Action<char> CharReceived;


        /*********
        ** Public methods
        *********/
        /// <summary>Initializer, as there is some init needed.</summary>
        static KeyboardInputResolver()
        {
            // Shared logic - hook the update
            EntoaroxFrameworkMod.SHelper.Events.GameLoop.UpdateTicked += KeyboardInputResolver.OnUpdateTicked;
            // MonoGame logic
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // Using reflection to hook the event so that monogame is not required to compile
                // Also makes sure the reference sticks just in case mono rewrite goes heads up on it
                typeof(GameWindow).GetEvent("TextInput").AddEventHandler(Game1.game1.Window, (Action<object, EventArgs>)KeyboardInputResolver.TextInputHandler);
                return;
            }

            // XNA logic
            KeyboardInputResolver.KeyDown += KeyboardInputResolver.KeyDownHandler;
            KeyboardInputResolver.KeyUp += KeyboardInputResolver.KeyUpHandler;
            KeyboardInputResolver.KeyHeld += KeyboardInputResolver.KeyHeldHandler;
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            KeyboardState New = Keyboard.GetState();
            Keys[] oldDown = KeyboardInputResolver.Old.GetPressedKeys();
            Keys[] down = New.GetPressedKeys().Where(a => !KeyboardInputResolver.Old.IsKeyDown(a)).ToArray();
            Keys[] up = oldDown.Where(a => !New.IsKeyDown(a)).ToArray();
            Keys[] held = oldDown.Where(a => New.IsKeyDown(a)).ToArray();
            foreach (Keys key in down)
            {
                KeyboardInputResolver.KeyDown?.Invoke(key);
                // int[]{ticks,tickRate}
                KeyboardInputResolver.Counter.Add(key, new[] { 30, 30 });
            }

            foreach (Keys key in up)
            {
                KeyboardInputResolver.Counter.Remove(key);
                KeyboardInputResolver.KeyUp?.Invoke(key);
            }

            foreach (Keys key in held)
            {
                KeyboardInputResolver.Counter[key][0]--;
                if (KeyboardInputResolver.Counter[key][0] > 0)
                    continue;
                KeyboardInputResolver.Counter[key][0] = KeyboardInputResolver.Counter[key][1];
                if (KeyboardInputResolver.Counter[key][1] > 15)
                    KeyboardInputResolver.Counter[key][1]--;
                KeyboardInputResolver.KeyHeld?.Invoke(key);
            }

            KeyboardInputResolver.Old = New;
        }

        // if XNA is being used, we need to do quite a bit of logic to make sure that the correct characters are output
        private static void KeyDownHandler(Keys key)
        {
            switch (key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    KeyboardInputResolver.Shift = true;
                    KeyboardInputResolver.Alt = true;
                    break;
                case Keys.CapsLock:
                    KeyboardInputResolver.Caps = true;
                    KeyboardInputResolver.Alt = true;
                    break;
            }

            KeyboardInputResolver.CharReceived?.Invoke(KeyboardInputResolver.ResolveChar(key));
        }

        private static void KeyUpHandler(Keys key)
        {
            switch (key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    KeyboardInputResolver.Shift = false;
                    KeyboardInputResolver.Alt = KeyboardInputResolver.Caps;
                    break;
                case Keys.CapsLock:
                    KeyboardInputResolver.Caps = false;
                    KeyboardInputResolver.Alt = KeyboardInputResolver.Shift;
                    break;
            }
        }

        private static void KeyHeldHandler(Keys key)
        {
            KeyboardInputResolver.CharReceived?.Invoke(KeyboardInputResolver.ResolveChar(key));
        }

        private static char ResolveChar(Keys key)
        {
            char @char = (char)key;
            if (!KeyboardInputResolver.Alt)
                return @char;
            short pre = KeyboardInputResolver.VkKeyScan(@char);
            uint post = (uint)pre & 0xFF;
            byte[] arr = new byte[256];
            if (KeyboardInputResolver.Shift)
                arr[0x10] = 0x80;
            if (KeyboardInputResolver.Caps)
                arr[0x14] = 0x80;
            KeyboardInputResolver.ToAscii(post, post, arr, out uint @out, 0);
            return (char)@out;
        }

        private static void TextInputHandler(object s, EventArgs e)
        {
            KeyboardInputResolver.CharReceived?.Invoke((char)e.GetType().GetField("Character").GetValue(e));
        }

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char c);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, out uint lpChar, uint flags); // If MonoGame is in use, we can easily take advantage of its build-in input handler
    }
}
