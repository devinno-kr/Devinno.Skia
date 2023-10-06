using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;

namespace SampleRPi.Pages
{
    public partial class PageDialogs : DvPage
    {
        #region Constructor
        public PageDialogs()
        {
            InitializeComponent();

            #region Event
            #region ls
            var ls = new List<DvTextIcon>();
            for (int i = 1; i <= 5; i++) ls.Add(new DvTextIcon { Text = $"Item{i}" });

            var lss = new List<SelectorItem>();
            for (int i = 1; i <= 5; i++) lss.Add(new SelectorItem { Text = $"Item{i}" });
            #endregion

            #region btnKeypadInt.ButtonClick
            btnKeypadInt.ButtonClick += (o, s) =>
            {
                DvDialogs.Keypad.ShowKeypad<int>("Int", (result) => { if (result.HasValue) lblKeypad.Text = result.Value.ToString(); });
            };
            #endregion
            #region btnKeypadFloat.ButtonClick
            btnKeypadFloat.ButtonClick += (o, s) =>
            {
                DvDialogs.Keypad.ShowKeypad<float>("Float", (result) => { if (result.HasValue) lblKeypad.Text = result.Value.ToString(); });
            };
            #endregion
            #region btnKeypadHex.ButtonClick
            btnKeypadHex.ButtonClick += (o, s) =>
            {
                DvDialogs.Keypad.ShowHex("Hex", (result) => { if (result != null) lblKeypad.Text = result; });
            };
            #endregion
            #region btnKeypadPasswd.ButtonClick
            btnKeypadPasswd.ButtonClick += (o, s) =>
            {
                DvDialogs.Keypad.ShowPassword("Password", (result) => { if (result != null) lblKeypad.Text = result; });
            };
            #endregion

            #region btnKeyboardEng.ButtonClick
            btnKeyboardEng.ButtonClick += (o, s) =>
            {
                DvDialogs.Keyboard.ShowKeyboard("English", KeyboardMode.English, (result) => { if (result != null) lblKeyboard.Text = result; });
            };
            #endregion
            #region btnKeyboardKor.ButtonClick
            btnKeyboardKor.ButtonClick += (o, s) =>
            {
                DvDialogs.Keyboard.ShowKeyboard("Korea", KeyboardMode.Korea, (result) => { if (result != null) lblKeyboard.Text = result; });
            };
            #endregion
            #region btnKeyboardNum.ButtonClick
            btnKeyboardNum.ButtonClick += (o, s) =>
            {
                DvDialogs.Keyboard.ShowKeyboard("Number", KeyboardMode.Number, (result) => { if (result != null) lblKeyboard.Text = result; });
            };
            #endregion
            #region btnKeyboardEngOnly.ButtonClick
            btnKeyboardEngOnly.ButtonClick += (o, s) =>
            {
                DvDialogs.Keyboard.ShowKeyboard("English Only", KeyboardMode.EnglishOnly, (result) => { if (result != null) lblKeyboard.Text = result; });
            };
            #endregion

            #region btnSelSelector.ButtonClick
            btnSelSelector.ButtonClick += (o, s) =>
            {
                DvDialogs.SelectorBox.ShowSelector("Selector", null, lss, (result) => { if (result != null) lblSelector.Text = result.Text; });
            };
            #endregion
            #region btnSelWheel.ButtonClick
            btnSelWheel.ButtonClick += (o, s) =>
            {
                DvDialogs.SelectorBox.ShowWheel("Wheel", null, ls, (result) => { if (result != null) lblSelector.Text = result.Text; });
            };
            #endregion
            #region btnSelRadio.ButtonClick
            btnSelRadio.ButtonClick += (o, s) =>
            {
                DvDialogs.SelectorBox.ColumnCount = 2;
                DvDialogs.SelectorBox.ShowRadio("Radio", null, ls, (result) => { if (result != null) lblSelector.Text = result.Text; });
                DvDialogs.SelectorBox.ColumnCount = 1;
            };
            #endregion
            #region btnSelCheck.ButtonClick
            btnSelCheck.ButtonClick += (o, s) =>
            {
                DvDialogs.SelectorBox.ColumnCount = 2;
                DvDialogs.SelectorBox.ShowCheck("Check", new List<DvTextIcon>(), ls, (result) =>
                {
                    if (result != null)
                    {
                        var r = string.Concat(result.Select(x => x.Text.Substring(4) + " "));
                        lblSelector.Text = r;
                    }
                });
                DvDialogs.SelectorBox.ColumnCount = 1;
            };
            #endregion

            #region btnMessageOK.ButtonClick
            btnMessageOK.ButtonClick += (o, s) => {

                DvDialogs.MessageBox.ShowMessageBoxOk("OK", "확인", (result) => lblMessage.Text = result.ToString());
            };
            #endregion
            #region btnMessageOC.ButtonClick
            btnMessageOC.ButtonClick += (o, s) => {

                DvDialogs.MessageBox.ShowMessageBoxOkCancel("OK/Cancel", "확인 or 취소 ?", (result) => lblMessage.Text = result.ToString());
            };
            #endregion
            #region btnMessageYN.ButtonClick
            btnMessageYN.ButtonClick += (o, s) => {

                DvDialogs.MessageBox.ShowMessageBoxYesNo("Yes/No", "예 or 아니요 ?", (result) => lblMessage.Text = result.ToString());
            };
            #endregion
            #region btnMessageYNC.ButtonClick
            btnMessageYNC.ButtonClick += (o, s) => {

                DvDialogs.MessageBox.ShowMessageBoxYesNoCancel("Yes/No/Cancel", "예 or 아니요 or 취소 ?", (result) => lblMessage.Text = result.ToString());
            };
            #endregion

            #region btnInputBox.ButtonClick
            btnInputBox.ButtonClick += (o, s) =>
            {
                DvDialogs.InputBox.ShowInputBox<People>("InputBox", null, null, (result) =>
                {
                    if (result != null) lblOther.Text = $"{result.Name} ({result.Age})";
                });
            };
            #endregion
            #region btnColorPicker.ButtonClick
            btnColorPicker.ButtonClick += (o, s) =>
            {
                DvDialogs.ColorPickerBox.ShowColorPicker("ColorPicker", null, (result) =>
                {
                    if(result.HasValue)
                    {
                        lblOther.Text = result.Value.ToString();
                    }
                });
            };
            #endregion
            #region btnTimePicker.ButtonClick
            btnTimePicker.ButtonClick += (o, s) =>
            {
                DvDialogs.DateTimePickerBox.ShowDateTimePicker("DateTimePicker", null, (result) =>
                {
                    if (result.HasValue)
                    {
                        lblOther.Text = result.Value.ToString("yy.MM.dd HH:mm;ss");
                    }
                });
            };
            #endregion
            #region btnCustomWindow.ButtonClick
            btnCustomWindow.ButtonClick += (o, s) =>
            {
                Program.MainWindow.CustomWindow.ShowCustomWindow(null, (result) =>
                {
                    if (result.HasValue) lblOther.Text = result.Value.ToString();
                });
            };
            #endregion
            #endregion
        }
        #endregion
    }

    #region class : People
    class People
    {
        public byte Age { get; set; }
        public string Name { get; set; }
    }
    #endregion
}
