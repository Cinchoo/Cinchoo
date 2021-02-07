using Cinchoo.Core.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public class ChoKeyEventArgs : KeyEventArgs
    {
        public ChoKeyEventArgs(Keys keyData)
            : base(keyData)
        { }

        public static ChoKeyEventArgs New(KEYBOARDHOOKSTRUCT keyboardHookStruct)
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

            ChoKeyEventArgs e = new ChoKeyEventArgs(
                (Keys)(
                keyboardHookStruct.VKCode |
                (control ? (int)Keys.Control : 0) |
                (shift ? (int)Keys.Shift : 0) |
                (alt ? (int)Keys.Alt : 0)
                ));

            return e;
        }
    }
}
