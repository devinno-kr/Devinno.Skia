using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvDialogs
    {
        public static DvKeypad Keypad { get; } = new DvKeypad();
        public static DvKeyboard Keyboard { get; } = new DvKeyboard();
        public static DvMessageBox MessageBox { get; } = new DvMessageBox();
        public static DvColorPickerBox ColorPickerBox { get; } = new DvColorPickerBox();
        public static DvDateTimePickerBox DateTimePickerBox { get; } = new DvDateTimePickerBox();
        public static DvInputBox InputBox { get; } = new DvInputBox();
        public static DvSelectorBox SelectorBox { get; } = new DvSelectorBox();
    }
}
