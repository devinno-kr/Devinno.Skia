using Devinno.Data;
using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace SampleRPi.Pages
{
    public class PageDialogs : DvPage
    {
        #region Member Variable
        private DvTableLayoutPanel tpnl;

        private DvLabel lblMBTitle;
        private DvButton btnMBOK;
        private DvButton btnMBYesNo;
        private DvButton btnMBOKCancel;
        private DvButton btnMBYesNoCancel;
        private DvLabel lblMBResult;

        private DvLabel lblKeyTitle;
        private DvButton btnKeypad;
        private DvButton btnKeypadEx;
        private DvButton btnKeyPass;
        private DvButton btnKeyHex;
        private DvButton btnKeyboard;
        private DvButton btnKeyboardEng;
        private DvLabel lblKeyResult;

        private DvLabel lblEtcTitle;
        private DvColorPicker colorPicker;
        private DvDateTimePicker dtPicker;
        private DvButton btnInputBox;
        private DvComboBox combo;

        private DvLabel lblInputTitle;
        private DvTextInput inText;
        private DvNumberInput<int> inNumber;
        private DvNumberInput<float> inFloat;
        private DvHexInput inHex;
        private DvBoolInput inBool;
        private DvSelectorInput inSel;
        
        private DvLabel lblValueInputTitle;
        private DvTextValueInput viText;
        private DvNumberValueInput<int> viNumber;
        private DvNumberValueInput<float> viFloat;
        private DvHexValueInput viHex;
        private DvBoolValueInput viBool;
        private DvSelectorValueInput viSel;

        private DvLabel lblValueInputButtonTitle;
        private DvTextValueInputButton vibText;
        private DvNumberValueInputButton<int> vibNumber;
        private DvNumberValueInputButton<float> vibFloat;
        private DvHexValueInputButton vibHex;
        private DvBoolValueInputButton vibBool;
        private DvSelectorValueInputButton vibSel;

        private DvMessageBox MessageBox;
        private DvKeypad Keypad;
        private DvKeyboard Keyboard;
        private DvInputBox InputBox;
        #endregion

        #region Constructor
        public PageDialogs()
        {
            UseMasterPage = true;

            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = MainWindow.BaseMargin, Fill = true };

            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 16F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 5F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 15F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 5F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 25F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 5F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 16F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 5F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 28F });

            Controls.Add(tpnl);
            #endregion
            #region New
            MessageBox = new DvMessageBox();
            Keypad = new DvKeypad();
            Keyboard = new DvKeyboard();
            InputBox = new DvInputBox();

            lblMBTitle = new DvLabel { Name = nameof(lblMBTitle), Text = "MessageBox", BackgroundDraw = false };
            btnMBOK = new DvButton { Name = nameof(btnMBOK), Text = "OK" };
            btnMBYesNo = new DvButton { Name = nameof(btnMBYesNo), Text = "Yes / No" };
            btnMBOKCancel = new DvButton { Name = nameof(btnMBOKCancel), Text = "OK / Cancel" };
            btnMBYesNoCancel = new DvButton { Name = nameof(btnMBYesNoCancel), Text = "Yes / No / Cancel" };
            lblMBResult = new DvLabel { Name = nameof(lblMBResult), Text = "" };

            lblKeyTitle = new DvLabel { Name = nameof(lblKeyTitle), Text = "Keyboard", BackgroundDraw = false };
            btnKeypad = new DvButton { Name = nameof(btnKeypad), Text = "Keypad" };
            btnKeypadEx = new DvButton { Name = nameof(btnKeypadEx), Text = "Keypad Ex" };
            btnKeyHex = new DvButton { Name = nameof(btnKeyHex), Text = "Keypad Hex" };
            btnKeyPass = new DvButton { Name = nameof(btnKeyPass), Text = "Password" };
            btnKeyboard = new DvButton { Name = nameof(btnKeyboard), Text = "Keyboard" };
            btnKeyboardEng = new DvButton { Name = nameof(btnKeyboardEng), Text = "Keyboard ENG" };
            lblKeyResult = new DvLabel { Name = nameof(lblKeyResult), Text = "" };

            lblEtcTitle = new DvLabel { Name = nameof(lblEtcTitle), Text = "ETC", BackgroundDraw = false };
            colorPicker = new DvColorPicker { Name = nameof(colorPicker) };
            dtPicker = new DvDateTimePicker { Name = nameof(dtPicker), Type = DateTimePickerType.DateTime };
            btnInputBox = new DvButton { Name = nameof(btnInputBox), Text = "InputBox" };
            combo = new DvComboBox { Name = nameof(combo) };
            for (int i = 1; i <= 12; i++) combo.Items.Add(new TextIconItem { Text = i + "시", Value = i });
            combo.SelectedIndex = 0;

            lblInputTitle = new DvLabel { Name = nameof(lblInputTitle), Text = "Input", BackgroundDraw = false };
            inText = new DvTextInput { Name = nameof(inText) };
            inNumber = new DvNumberInput<int> { Name = nameof(inNumber), Unit = "㎜" };
            inFloat = new DvNumberInput<float> { Name = nameof(inFloat), Unit = "℃" };
            inHex = new DvHexInput { Name = nameof(inHex) };
            inBool = new DvBoolInput { Name = nameof(inBool) };
            inSel = new DvSelectorInput { Name = nameof(inSel) };
            inSel.Items.AddRange(Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(x => new TextIconItem { Text = x.ToString(), Value = x }));
            inSel.SelectedIndex = 0;

            lblValueInputTitle = new DvLabel { Name = nameof(lblValueInputTitle), Text = "Value Input", BackgroundDraw = false };
            viText = new DvTextValueInput { Name = nameof(viText), Title = "명칭", Value = "" };
            viNumber = new DvNumberValueInput<int> { Name = nameof(viNumber), Unit = "㎜", Title = "길이" };
            viFloat = new DvNumberValueInput<float> { Name = nameof(viFloat), Unit = "℃", Title = "온도" };
            viHex = new DvHexValueInput { Name = nameof(viHex), Title = "포트 A" };
            viBool = new DvBoolValueInput { Name = nameof(viBool), Title = "램프" };
            viSel = new DvSelectorValueInput { Name = nameof(viSel), Title = "요일" };
            viSel.Items.AddRange(Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(x => new TextIconItem { Text = x.ToString(), Value = x }));
            viSel.SelectedIndex = 0;

            lblValueInputButtonTitle = new DvLabel { Name = nameof(lblValueInputButtonTitle), Text = "Value Input Button", BackgroundDraw = false };
            vibText = new DvTextValueInputButton { Name = nameof(vibText), Title = "명칭", Value = "", ButtonText = "SET", ButtonWidth = 36 };
            vibNumber = new DvNumberValueInputButton<int> { Name = nameof(vibNumber), Unit = "㎜", Title = "길이", ButtonText = "SET", ButtonWidth = 36 };
            vibFloat = new DvNumberValueInputButton<float> { Name = nameof(vibFloat), Unit = "℃", Title = "온도", ButtonText = "SET", ButtonWidth = 36 };
            vibHex = new DvHexValueInputButton { Name = nameof(vibHex), Title = "포트 A", ButtonText = "SET", ButtonWidth = 36 };
            vibBool = new DvBoolValueInputButton { Name = nameof(vibBool), Title = "램프", ButtonText = "SET", ButtonWidth = 36 };
            vibSel = new DvSelectorValueInputButton { Name = nameof(vibSel), Title = "요일", ButtonText = "SET", ButtonWidth = 36 };
            vibSel.Items.AddRange(Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(x => new TextIconItem { Text = x.ToString(), Value = x }));
            vibSel.SelectedIndex = 0;


            #endregion
            #region Controls.Add
            tpnl.Controls.Add(lblMBTitle, 0, 0);
            tpnl.Controls.Add(btnMBOK, 0, 1);
            tpnl.Controls.Add(btnMBYesNo, 0, 2);
            tpnl.Controls.Add(btnMBOKCancel, 0, 3);
            tpnl.Controls.Add(btnMBYesNoCancel, 0, 4);
            tpnl.Controls.Add(lblMBResult, 0, 5);

            tpnl.Controls.Add(lblKeyTitle, 2, 0);
            tpnl.Controls.Add(btnKeypad, 2, 1);
            tpnl.Controls.Add(btnKeypadEx, 2, 2);
            tpnl.Controls.Add(btnKeyHex, 2, 3);
            tpnl.Controls.Add(btnKeyPass, 2, 4);
            tpnl.Controls.Add(btnKeyboard, 2, 5);
            tpnl.Controls.Add(btnKeyboardEng, 2, 6);
            tpnl.Controls.Add(lblKeyResult, 2, 7);

            tpnl.Controls.Add(lblEtcTitle, 4, 0);
            tpnl.Controls.Add(colorPicker, 4, 1);
            tpnl.Controls.Add(dtPicker, 4, 2);
            tpnl.Controls.Add(btnInputBox, 4, 3);
            tpnl.Controls.Add(combo, 4, 4);

            tpnl.Controls.Add(lblInputTitle, 6, 0);
            tpnl.Controls.Add(inText, 6, 1);
            tpnl.Controls.Add(inNumber, 6, 2);
            tpnl.Controls.Add(inFloat, 6, 3);
            tpnl.Controls.Add(inHex, 6, 4);
            tpnl.Controls.Add(inBool, 6, 5);
            tpnl.Controls.Add(inSel, 6, 6);

            bool v = true;
            if (v)
            {
                tpnl.Controls.Add(lblValueInputTitle, 8, 0);
                tpnl.Controls.Add(viText, 8, 1);
                tpnl.Controls.Add(viNumber, 8, 2);
                tpnl.Controls.Add(viFloat, 8, 3);
                tpnl.Controls.Add(viHex, 8, 4);
                tpnl.Controls.Add(viBool, 8, 5);
                tpnl.Controls.Add(viSel, 8, 6);
            }
            else
            {
                tpnl.Controls.Add(lblValueInputButtonTitle, 8, 0);
                tpnl.Controls.Add(vibText, 8, 1);
                tpnl.Controls.Add(vibNumber, 8, 2);
                tpnl.Controls.Add(vibFloat, 8, 3);
                tpnl.Controls.Add(vibHex, 8, 4);
                tpnl.Controls.Add(vibBool, 8, 5);
                tpnl.Controls.Add(vibSel, 8, 6);
            }
            #endregion
            #region Event
            btnMBOK.MouseClick += (o, s) => MessageBox.ShowMessageBoxOk("애국가", "동해물과 백두산이 마르고 닳도록\r\n하나님이 보우하사 우리나라 만세\r\n무궁화 삼천리 화려강산\r\n대한사람 대한으로 길이 보전하세", (result) => lblMBResult.Text = result.ToString());
            btnMBYesNo.MouseClick += (o, s) => MessageBox.ShowMessageBoxYesNo("진행", "진행 하시겠습니까?", (result) => lblMBResult.Text = result.ToString());
            btnMBOKCancel.MouseClick += (o, s) => MessageBox.ShowMessageBoxOkCancel("확인", "확인 하시겠습니까?", (result) => lblMBResult.Text = result.ToString());
            btnMBYesNoCancel.MouseClick += (o, s) => MessageBox.ShowMessageBoxYesNoCancel("저장", "저장 하시겠습니까?", (result) => lblMBResult.Text = result.ToString());

            btnKeypad.MouseClick += (o, s) => { Keypad.Width = 300; Keypad.ShowKeypad<int>("키패드", (value) => lblKeyResult.Text = value.HasValue ? value.Value.ToString() : ""); };
            btnKeypadEx.MouseClick += (o, s) => { Keypad.Width = 300; Keypad.ShowKeypad<double>("키패드", (value) => lblKeyResult.Text = value.HasValue ? value.Value.ToString() : ""); };
            btnKeyHex.MouseClick += (o, s) => { Keypad.Width = 360; Keypad.ShowHex("키패드", (value) => lblKeyResult.Text = value ?? ""); };
            btnKeyPass.MouseClick += (o, s) => { Keypad.Width = 300; Keypad.ShowPassword("패스워드", (value) => lblKeyResult.Text = value ?? ""); };
            btnKeyboard.MouseClick += (o, s) => Keyboard.ShowKeyboard("키보드", "", (value) => lblKeyResult.Text = value ?? "");
            btnKeyboardEng.MouseClick += (o, s) => Keyboard.ShowKeyboard("키보드", KeyboardMode.EnglishOnly, "", (value) => lblKeyResult.Text = value ?? "");

            btnInputBox.MouseClick += (o, s) =>
            {
                Dictionary<string, InputBoxInfo> infos = new Dictionary<string, InputBoxInfo>();
                infos.Add("Name", new InputBoxInfo { Title = "명칭" });
                infos.Add("Distance", new InputBoxInfo { Title = "길이", Unit = "㎜" });
                infos.Add("Temperature", new InputBoxInfo { Title = "온도", Unit = "℃" });
                infos.Add("DayOfWeek", new InputBoxInfo { Title = "요일" });
                infos.Add("Step", new InputBoxInfo { Title = "단계", Items = new int[] { 0, 10, 20, 30, 40, 50 }.Select(x => new TextIconItem() { Text = x.ToString(), Value = x }).ToList() });
                infos.Add("Use", new InputBoxInfo { Title = "사용여부" });

                InputBox.ItemWidth = 200;
                InputBox.ShowInputBox<Data>("입력", new Data { Name = "테스트", Distance = 100, Temperature = 36.5, DayOfWeek = DayOfWeek.Monday }, infos, (result) =>
                {
                    if (result != null)
                    {
                        var set = new JsonSerializerSettings() { Formatting = Formatting.Indented };
                        var s = Serialize.JsonSerialize(result, set);

                        MessageBox.ShowMessageBoxOk("JSON", s, (result2) => { });
                    }
                });
            };
            #endregion

            combo.MaximumViewCount = 5;

        }
        #endregion
    }

    public class Data
    {
        public string Name { get; set; }
        public int Distance { get; set; }
        public double Temperature { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int Step { get; set; }
        public bool Use { get; set; }
     
        [InputBoxIgnore]
        [JsonIgnore]
        public string Ext { get; set; }

        [JsonIgnore]
        public object Tag { get; set; }
    }
}
