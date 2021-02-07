using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public class ChoKeyPressEventArgs : KeyPressEventArgs
    {
        public ChoKeyPressEventArgs(char keyChar)
            : base(keyChar)
        { }

        public static ChoKeyPressEventArgs New(KEYBOARDHOOKSTRUCT keyboardHookStruct)
        {
            // Is Control being held down?
            bool control = ((ChoUser32.GetKeyState(ChoUser32.VK_LCONTROL) & 0x80) != 0) ||
                           ((ChoUser32.GetKeyState(ChoUser32.VK_RCONTROL) & 0x80) != 0);

            // Is Shift being held down?
            bool shift = ((ChoUser32.GetKeyState(ChoUser32.VK_LSHIFT) & 0x80) != 0) ||
                         ((ChoUser32.GetKeyState(ChoUser32.VK_RSHIFT) & 0x80) != 0);

            // Is Alt being held down?
            bool alt = ((ChoUser32.GetKeyState(ChoUser32.VK_LALT) & 0x80) != 0) ||
                       ((ChoUser32.GetKeyState(ChoUser32.VK_RALT) & 0x80) != 0);

            // Is CapsLock on?
            bool capslock = (ChoUser32.GetKeyState(ChoUser32.VK_CAPITAL) != 0);

            byte[] keyState = new byte[256];
            byte[] inBuffer = new byte[2];

            ChoUser32.GetKeyboardState(keyState);

            if (ChoUser32.ToAscii(keyboardHookStruct.VKCode,
                      keyboardHookStruct.ScanCode,
                      keyState,
                      inBuffer,
                      keyboardHookStruct.Flags) == 1)
            {

                char key = (char)inBuffer[0];
                if ((capslock ^ shift) && Char.IsLetter(key))
                    key = Char.ToUpper(key);

                return new ChoKeyPressEventArgs(key);
            }

            return null;
        }
    }
}
