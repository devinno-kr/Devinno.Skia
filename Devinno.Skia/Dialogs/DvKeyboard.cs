using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvKeyboard : DvWindow
    {
        #region Const
        const int HANGUL = 0;
        const int HANGUL_SHIFT = 10;
        const int ENGLISH = 1;
        const int ENGLISH_SHIFT = 11;
        const int NUMBER = 2;
        const int NUMBER_SHIFT = 12;
        #endregion

        #region Properties
        private int PState => (IsNumber ? NUMBER : IsHangul? HANGUL : ENGLISH) + (IsShift ? 10 : 0);
        public bool IsHangul => Han.Checked;
        public bool IsShift => Shift.Checked;
        public bool IsNumber => Num.Checked;

        public int ButtonTextSize { get; set; } = 12;
        public int ButtonIconSize { get; set; } = 16;
        public bool UseShiftHolding { get; set; } = false;
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvLabel lbl;
        DvButton[] Ns = new DvButton[10];
        DvButton[] Us = new DvButton[10];
        DvButton[] Ms = new DvButton[9];
        DvButton[] Ds = new DvButton[7];
        DvButton[] Ss = new DvButton[3];
        DvButton Ent, Back, Clear;
        DvToggleButton Shift, Han, Num;
        DvGridLayoutPanel gpnl;

        bool bOK = false;

        Dictionary<int, Dictionary<string, string>> KeyMap = new Dictionary<int, Dictionary<string, string>>();
        
        Hangul parseHangul = new Hangul();

        string str = "";
        string strOrigin = "";
        #endregion

        #region Constructor
        public DvKeyboard()
        {
            IconString = "fa-keyboard";
            Width = 600;
            Height = 400;

            #region gpnl
            gpnl = new DvGridLayoutPanel() { Name = nameof(gpnl), Margin = new Padding(10), Fill = true };

            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 15 });
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 17 });
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 17 });
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 17 });
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 17 });
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 17 });

            gpnl.Rows[0].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 100 });

            for (int i = 0; i < 10; i++) gpnl.Rows[1].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 10 });
            for (int i = 0; i < 10; i++) gpnl.Rows[2].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 10 });
            for (int i = 0; i < 10; i++) gpnl.Rows[3].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = i == 0 ? 5 : 10 });
            for (int i = 0; i < 9; i++) gpnl.Rows[4].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = (i == 0 || i == 8 ? 15 : 10) });

            gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 15 });
            gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 10 });
            gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 10 });
            gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 40 });
            gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 10 });
            gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 15 });

            Controls.Add(gpnl);
            #endregion
            #region Hangul Init
            parseHangul.InitState();
            #endregion
            #region Make KeyMap
            KeyMap.Add(HANGUL, new Dictionary<string, string>());
            KeyMap.Add(ENGLISH, new Dictionary<string, string>());
            KeyMap.Add(NUMBER, new Dictionary<string, string>());

            KeyMap.Add(HANGUL_SHIFT, new Dictionary<string, string>());
            KeyMap.Add(ENGLISH_SHIFT, new Dictionary<string, string>());
            KeyMap.Add(NUMBER_SHIFT, new Dictionary<string, string>());

            #region HANGUL
            KeyMap[HANGUL].Add("Ns1", "1");
            KeyMap[HANGUL].Add("Ns2", "2");
            KeyMap[HANGUL].Add("Ns3", "3");
            KeyMap[HANGUL].Add("Ns4", "4");
            KeyMap[HANGUL].Add("Ns5", "5");
            KeyMap[HANGUL].Add("Ns6", "6");
            KeyMap[HANGUL].Add("Ns7", "7");
            KeyMap[HANGUL].Add("Ns8", "8");
            KeyMap[HANGUL].Add("Ns9", "9");
            KeyMap[HANGUL].Add("Ns10", "0");

            KeyMap[HANGUL].Add("Us1", "ㅂ");
            KeyMap[HANGUL].Add("Us2", "ㅈ");
            KeyMap[HANGUL].Add("Us3", "ㄷ");
            KeyMap[HANGUL].Add("Us4", "ㄱ");
            KeyMap[HANGUL].Add("Us5", "ㅅ");
            KeyMap[HANGUL].Add("Us6", "ㅛ");
            KeyMap[HANGUL].Add("Us7", "ㅕ");
            KeyMap[HANGUL].Add("Us8", "ㅑ");
            KeyMap[HANGUL].Add("Us9", "ㅐ");
            KeyMap[HANGUL].Add("Us10", "ㅔ");

            KeyMap[HANGUL].Add("Ms1", "ㅁ");
            KeyMap[HANGUL].Add("Ms2", "ㄴ");
            KeyMap[HANGUL].Add("Ms3", "ㅇ");
            KeyMap[HANGUL].Add("Ms4", "ㄹ");
            KeyMap[HANGUL].Add("Ms5", "ㅎ");
            KeyMap[HANGUL].Add("Ms6", "ㅗ");
            KeyMap[HANGUL].Add("Ms7", "ㅓ");
            KeyMap[HANGUL].Add("Ms8", "ㅏ");
            KeyMap[HANGUL].Add("Ms9", "ㅣ");

            KeyMap[HANGUL].Add("Ds1", "ㅋ");
            KeyMap[HANGUL].Add("Ds2", "ㅌ");
            KeyMap[HANGUL].Add("Ds3", "ㅊ");
            KeyMap[HANGUL].Add("Ds4", "ㅍ");
            KeyMap[HANGUL].Add("Ds5", "ㅠ");
            KeyMap[HANGUL].Add("Ds6", "ㅜ");
            KeyMap[HANGUL].Add("Ds7", "ㅡ");

            KeyMap[HANGUL].Add("Ss1", ",");
            KeyMap[HANGUL].Add("Ss2", " ");
            KeyMap[HANGUL].Add("Ss3", ".");
            #endregion
            #region HANGUL_SHIFT
            KeyMap[HANGUL_SHIFT].Add("Ns1", "1");
            KeyMap[HANGUL_SHIFT].Add("Ns2", "2");
            KeyMap[HANGUL_SHIFT].Add("Ns3", "3");
            KeyMap[HANGUL_SHIFT].Add("Ns4", "4");
            KeyMap[HANGUL_SHIFT].Add("Ns5", "5");
            KeyMap[HANGUL_SHIFT].Add("Ns6", "6");
            KeyMap[HANGUL_SHIFT].Add("Ns7", "7");
            KeyMap[HANGUL_SHIFT].Add("Ns8", "8");
            KeyMap[HANGUL_SHIFT].Add("Ns9", "9");
            KeyMap[HANGUL_SHIFT].Add("Ns10", "0");

            KeyMap[HANGUL_SHIFT].Add("Us1", "ㅃ");
            KeyMap[HANGUL_SHIFT].Add("Us2", "ㅉ");
            KeyMap[HANGUL_SHIFT].Add("Us3", "ㄸ");
            KeyMap[HANGUL_SHIFT].Add("Us4", "ㄲ");
            KeyMap[HANGUL_SHIFT].Add("Us5", "ㅆ");
            KeyMap[HANGUL_SHIFT].Add("Us6", "ㅛ");
            KeyMap[HANGUL_SHIFT].Add("Us7", "ㅕ");
            KeyMap[HANGUL_SHIFT].Add("Us8", "ㅑ");
            KeyMap[HANGUL_SHIFT].Add("Us9", "ㅒ");
            KeyMap[HANGUL_SHIFT].Add("Us10", "ㅖ");

            KeyMap[HANGUL_SHIFT].Add("Ms1", "ㅁ");
            KeyMap[HANGUL_SHIFT].Add("Ms2", "ㄴ");
            KeyMap[HANGUL_SHIFT].Add("Ms3", "ㅇ");
            KeyMap[HANGUL_SHIFT].Add("Ms4", "ㄹ");
            KeyMap[HANGUL_SHIFT].Add("Ms5", "ㅎ");
            KeyMap[HANGUL_SHIFT].Add("Ms6", "ㅗ");
            KeyMap[HANGUL_SHIFT].Add("Ms7", "ㅓ");
            KeyMap[HANGUL_SHIFT].Add("Ms8", "ㅏ");
            KeyMap[HANGUL_SHIFT].Add("Ms9", "ㅣ");

            KeyMap[HANGUL_SHIFT].Add("Ds1", "ㅋ");
            KeyMap[HANGUL_SHIFT].Add("Ds2", "ㅌ");
            KeyMap[HANGUL_SHIFT].Add("Ds3", "ㅊ");
            KeyMap[HANGUL_SHIFT].Add("Ds4", "ㅍ");
            KeyMap[HANGUL_SHIFT].Add("Ds5", "ㅠ");
            KeyMap[HANGUL_SHIFT].Add("Ds6", "ㅜ");
            KeyMap[HANGUL_SHIFT].Add("Ds7", "ㅡ");

            KeyMap[HANGUL_SHIFT].Add("Ss1", ",");
            KeyMap[HANGUL_SHIFT].Add("Ss2", " ");
            KeyMap[HANGUL_SHIFT].Add("Ss3", ".");
            #endregion
            #region ENGLISH
            KeyMap[ENGLISH].Add("Ns1", "1");
            KeyMap[ENGLISH].Add("Ns2", "2");
            KeyMap[ENGLISH].Add("Ns3", "3");
            KeyMap[ENGLISH].Add("Ns4", "4");
            KeyMap[ENGLISH].Add("Ns5", "5");
            KeyMap[ENGLISH].Add("Ns6", "6");
            KeyMap[ENGLISH].Add("Ns7", "7");
            KeyMap[ENGLISH].Add("Ns8", "8");
            KeyMap[ENGLISH].Add("Ns9", "9");
            KeyMap[ENGLISH].Add("Ns10", "0");

            KeyMap[ENGLISH].Add("Us1", "q");
            KeyMap[ENGLISH].Add("Us2", "w");
            KeyMap[ENGLISH].Add("Us3", "e");
            KeyMap[ENGLISH].Add("Us4", "r");
            KeyMap[ENGLISH].Add("Us5", "t");
            KeyMap[ENGLISH].Add("Us6", "y");
            KeyMap[ENGLISH].Add("Us7", "u");
            KeyMap[ENGLISH].Add("Us8", "i");
            KeyMap[ENGLISH].Add("Us9", "o");
            KeyMap[ENGLISH].Add("Us10", "p");

            KeyMap[ENGLISH].Add("Ms1", "a");
            KeyMap[ENGLISH].Add("Ms2", "s");
            KeyMap[ENGLISH].Add("Ms3", "d");
            KeyMap[ENGLISH].Add("Ms4", "f");
            KeyMap[ENGLISH].Add("Ms5", "g");
            KeyMap[ENGLISH].Add("Ms6", "h");
            KeyMap[ENGLISH].Add("Ms7", "j");
            KeyMap[ENGLISH].Add("Ms8", "k");
            KeyMap[ENGLISH].Add("Ms9", "l");

            KeyMap[ENGLISH].Add("Ds1", "z");
            KeyMap[ENGLISH].Add("Ds2", "x");
            KeyMap[ENGLISH].Add("Ds3", "c");
            KeyMap[ENGLISH].Add("Ds4", "v");
            KeyMap[ENGLISH].Add("Ds5", "b");
            KeyMap[ENGLISH].Add("Ds6", "n");
            KeyMap[ENGLISH].Add("Ds7", "m");

            KeyMap[ENGLISH].Add("Ss1", ",");
            KeyMap[ENGLISH].Add("Ss2", " ");
            KeyMap[ENGLISH].Add("Ss3", ".");
            #endregion
            #region ENGLISH_SHIFT
            KeyMap[ENGLISH_SHIFT].Add("Ns1", "1");
            KeyMap[ENGLISH_SHIFT].Add("Ns2", "2");
            KeyMap[ENGLISH_SHIFT].Add("Ns3", "3");
            KeyMap[ENGLISH_SHIFT].Add("Ns4", "4");
            KeyMap[ENGLISH_SHIFT].Add("Ns5", "5");
            KeyMap[ENGLISH_SHIFT].Add("Ns6", "6");
            KeyMap[ENGLISH_SHIFT].Add("Ns7", "7");
            KeyMap[ENGLISH_SHIFT].Add("Ns8", "8");
            KeyMap[ENGLISH_SHIFT].Add("Ns9", "9");
            KeyMap[ENGLISH_SHIFT].Add("Ns10", "0");

            KeyMap[ENGLISH_SHIFT].Add("Us1", "Q");
            KeyMap[ENGLISH_SHIFT].Add("Us2", "W");
            KeyMap[ENGLISH_SHIFT].Add("Us3", "E");
            KeyMap[ENGLISH_SHIFT].Add("Us4", "R");
            KeyMap[ENGLISH_SHIFT].Add("Us5", "T");
            KeyMap[ENGLISH_SHIFT].Add("Us6", "Y");
            KeyMap[ENGLISH_SHIFT].Add("Us7", "U");
            KeyMap[ENGLISH_SHIFT].Add("Us8", "I");
            KeyMap[ENGLISH_SHIFT].Add("Us9", "O");
            KeyMap[ENGLISH_SHIFT].Add("Us10", "P");

            KeyMap[ENGLISH_SHIFT].Add("Ms1", "A");
            KeyMap[ENGLISH_SHIFT].Add("Ms2", "S");
            KeyMap[ENGLISH_SHIFT].Add("Ms3", "D");
            KeyMap[ENGLISH_SHIFT].Add("Ms4", "F");
            KeyMap[ENGLISH_SHIFT].Add("Ms5", "G");
            KeyMap[ENGLISH_SHIFT].Add("Ms6", "H");
            KeyMap[ENGLISH_SHIFT].Add("Ms7", "J");
            KeyMap[ENGLISH_SHIFT].Add("Ms8", "K");
            KeyMap[ENGLISH_SHIFT].Add("Ms9", "L");

            KeyMap[ENGLISH_SHIFT].Add("Ds1", "Z");
            KeyMap[ENGLISH_SHIFT].Add("Ds2", "X");
            KeyMap[ENGLISH_SHIFT].Add("Ds3", "C");
            KeyMap[ENGLISH_SHIFT].Add("Ds4", "V");
            KeyMap[ENGLISH_SHIFT].Add("Ds5", "B");
            KeyMap[ENGLISH_SHIFT].Add("Ds6", "N");
            KeyMap[ENGLISH_SHIFT].Add("Ds7", "M");

            KeyMap[ENGLISH_SHIFT].Add("Ss1", ",");
            KeyMap[ENGLISH_SHIFT].Add("Ss2", " ");
            KeyMap[ENGLISH_SHIFT].Add("Ss3", ".");
            #endregion
            #region NUMBER
            KeyMap[NUMBER].Add("Ns1", "1");
            KeyMap[NUMBER].Add("Ns2", "2");
            KeyMap[NUMBER].Add("Ns3", "3");
            KeyMap[NUMBER].Add("Ns4", "4");
            KeyMap[NUMBER].Add("Ns5", "5");
            KeyMap[NUMBER].Add("Ns6", "6");
            KeyMap[NUMBER].Add("Ns7", "7");
            KeyMap[NUMBER].Add("Ns8", "8");
            KeyMap[NUMBER].Add("Ns9", "9");
            KeyMap[NUMBER].Add("Ns10", "0");

            KeyMap[NUMBER].Add("Us1", "!");
            KeyMap[NUMBER].Add("Us2", "@");
            KeyMap[NUMBER].Add("Us3", "#");
            KeyMap[NUMBER].Add("Us4", "~");
            KeyMap[NUMBER].Add("Us5", "%");
            KeyMap[NUMBER].Add("Us6", "^");
            KeyMap[NUMBER].Add("Us7", "&");
            KeyMap[NUMBER].Add("Us8", "*");
            KeyMap[NUMBER].Add("Us9", "(");
            KeyMap[NUMBER].Add("Us10", ")");

            KeyMap[NUMBER].Add("Ms1", "+");
            KeyMap[NUMBER].Add("Ms2", "×");
            KeyMap[NUMBER].Add("Ms3", "÷");
            KeyMap[NUMBER].Add("Ms4", "=");
            KeyMap[NUMBER].Add("Ms5", "/");
            KeyMap[NUMBER].Add("Ms6", "_");
            KeyMap[NUMBER].Add("Ms7", "<");
            KeyMap[NUMBER].Add("Ms8", ">");
            KeyMap[NUMBER].Add("Ms9", "♡");

            KeyMap[NUMBER].Add("Ds1", "-");
            KeyMap[NUMBER].Add("Ds2", "'");
            KeyMap[NUMBER].Add("Ds3", "\"");
            KeyMap[NUMBER].Add("Ds4", ":");
            KeyMap[NUMBER].Add("Ds5", ";");
            KeyMap[NUMBER].Add("Ds6", ",");
            KeyMap[NUMBER].Add("Ds7", "?");

            KeyMap[NUMBER].Add("Ss1", ",");
            KeyMap[NUMBER].Add("Ss2", " ");
            KeyMap[NUMBER].Add("Ss3", ".");
            #endregion
            #region NUMBER_SHIFT
            KeyMap[NUMBER_SHIFT].Add("Ns1", "1");
            KeyMap[NUMBER_SHIFT].Add("Ns2", "2");
            KeyMap[NUMBER_SHIFT].Add("Ns3", "3");
            KeyMap[NUMBER_SHIFT].Add("Ns4", "4");
            KeyMap[NUMBER_SHIFT].Add("Ns5", "5");
            KeyMap[NUMBER_SHIFT].Add("Ns6", "6");
            KeyMap[NUMBER_SHIFT].Add("Ns7", "7");
            KeyMap[NUMBER_SHIFT].Add("Ns8", "8");
            KeyMap[NUMBER_SHIFT].Add("Ns9", "9");
            KeyMap[NUMBER_SHIFT].Add("Ns10", "0");

            KeyMap[NUMBER_SHIFT].Add("Us1", "•");
            KeyMap[NUMBER_SHIFT].Add("Us2", "○");
            KeyMap[NUMBER_SHIFT].Add("Us3", "●");
            KeyMap[NUMBER_SHIFT].Add("Us4", "□");
            KeyMap[NUMBER_SHIFT].Add("Us5", "■");
            KeyMap[NUMBER_SHIFT].Add("Us6", "◇");
            KeyMap[NUMBER_SHIFT].Add("Us7", "$");
            KeyMap[NUMBER_SHIFT].Add("Us8", "€");
            KeyMap[NUMBER_SHIFT].Add("Us9", "₤");
            KeyMap[NUMBER_SHIFT].Add("Us10", "¥");

            KeyMap[NUMBER_SHIFT].Add("Ms1", "'");
            KeyMap[NUMBER_SHIFT].Add("Ms2", "₩");
            KeyMap[NUMBER_SHIFT].Add("Ms3", "\\");
            KeyMap[NUMBER_SHIFT].Add("Ms4", "|");
            KeyMap[NUMBER_SHIFT].Add("Ms5", "☆");
            KeyMap[NUMBER_SHIFT].Add("Ms6", "{");
            KeyMap[NUMBER_SHIFT].Add("Ms7", "}");
            KeyMap[NUMBER_SHIFT].Add("Ms8", "[");
            KeyMap[NUMBER_SHIFT].Add("Ms9", "]");

            KeyMap[NUMBER_SHIFT].Add("Ds1", "°");
            KeyMap[NUMBER_SHIFT].Add("Ds2", "※");
            KeyMap[NUMBER_SHIFT].Add("Ds3", "¤");
            KeyMap[NUMBER_SHIFT].Add("Ds4", "《");
            KeyMap[NUMBER_SHIFT].Add("Ds5", "》");
            KeyMap[NUMBER_SHIFT].Add("Ds6", "¡");
            KeyMap[NUMBER_SHIFT].Add("Ds7", "¿");

            KeyMap[NUMBER_SHIFT].Add("Ss1", ",");
            KeyMap[NUMBER_SHIFT].Add("Ss2", " ");
            KeyMap[NUMBER_SHIFT].Add("Ss3", ".");
            #endregion
            #endregion
            #region New
            var gr = false;
            lbl = new DvLabel() { Name = nameof(lbl) };

            for (int i = 0; i < 10; i++) Ns[i] = new DvButton() { Name = "Ns" + (i + 1), Gradient = gr };
            for (int i = 0; i < 10; i++) Us[i] = new DvButton() { Name = "Us" + (i + 1), Gradient = gr };
            for (int i = 0; i < 9; i++) Ms[i] = new DvButton() { Name = "Ms" + (i + 1), Gradient = gr };
            for (int i = 0; i < 7; i++) Ds[i] = new DvButton() { Name = "Ds" + (i + 1), Gradient = gr };
            for (int i = 0; i < 3; i++) Ss[i] = new DvButton() { Name = "Ss" + (i + 1), Gradient = gr };
            Ent = new DvButton() { Name = nameof(Ent), Text = "", IconString = "", Gradient = gr };
            Back = new DvButton() { Name = nameof(Back), Text = "", IconString = "", Gradient = gr };
            Clear = new DvButton() { Name = nameof(Clear), Text = "", IconString = "", Gradient = gr };
            Han = new DvToggleButton() { Name = nameof(Han), Text = "", IconString = "", Gradient = gr };
            Num = new DvToggleButton() { Name = nameof(Num), Text = "", IconString = "", Gradient = gr };
            Shift = new DvToggleButton() { Name = nameof(Shift), Text = "", IconString = "", Gradient = gr };

            Ent.Drawn += (o, cv) =>
            {
                var design = Design;
                if (design != null && design.Theme != null)
                {
                    var thm = design.Theme;

                    var nsz = Convert.ToInt32(ButtonIconSize * 0.6);
                    var c = Ent;
                    var bounds = Util.FromRect(0, 0, c.Width, c.Height);
                    var sz = new SKSize(Convert.ToInt32(nsz * 1.25F), Convert.ToInt32(nsz * 0.75F));
                    var rtC = Util.MakeRectangleAlign(bounds, sz, DvContentAlignment.MiddleCenter);
                    var cT = c.ForeColor ?? thm.ForeColor;
                    rtC.Offset(0, -Convert.ToInt32(nsz * 0.2));
                    if (c.ButtonDownState)
                    {
                        rtC.Offset(0, 1);
                        cT = cT.BrightnessTransmit(Design.Theme.DownBrightness);
                    }

                    using (var p = new SKPaint() { IsAntialias = true })
                    {
                        using (var mf = SKImageFilter.CreateDropShadow(1, 1, 0, 0, Util.FromArgb(80, SKColors.Black)))
                        {
                            p.IsStroke = true;
                            p.StrokeWidth = 3.5F;
                            p.Color = cT;
                            p.ImageFilter = mf;

                            using (var path = new SKPath() { })
                            {
                                var vc = Convert.ToInt32(nsz * 0.25);
                                var vc2 = Convert.ToInt32(nsz * 0.4);

                                path.MoveTo(rtC.Right, rtC.Top);
                                path.LineTo(rtC.Right, rtC.Bottom);
                                path.LineTo(rtC.Left + vc2, rtC.Bottom);
                                path.LineTo(rtC.Left + vc2, rtC.Bottom - vc);
                                path.LineTo(rtC.Left, rtC.Bottom);
                                path.LineTo(rtC.Left + vc2, rtC.Bottom + vc);
                                path.LineTo(rtC.Left + vc2, rtC.Bottom);

                                cv.DrawPath(path, p);
                            }
                        }

                    }
                }
            };
            #endregion
            #region Event
            for (int i = 0; i < 10; i++) Ns[i].MouseClick += Key_MouseClick;
            for (int i = 0; i < 10; i++) Us[i].MouseClick += Key_MouseClick;
            for (int i = 0; i < 9; i++) Ms[i].MouseClick += Key_MouseClick;
            for (int i = 0; i < 7; i++) Ds[i].MouseClick += Key_MouseClick;
            for (int i = 0; i < 3; i++) Ss[i].MouseClick += Key_MouseClick;
            Ent.MouseClick += Ent_MouseClick;
            Back.MouseClick += Back_MouseClick;
            Clear.MouseClick += Clear_MouseClick;
            Han.CheckedChanged += Han_CheckedChanged;
            Num.CheckedChanged += Num_CheckedChanged;
            Shift.CheckedChanged += Shift_CheckedChanged;
            #endregion
        }
        #endregion

        #region Event
        #region Shift_CheckedChanged
        private void Shift_CheckedChanged(object sender, EventArgs e)
        {
            SetButtonText();
            SetText();
        }
        #endregion
        #region Num_CheckedChanged 
        private void Num_CheckedChanged(object sender, EventArgs e)
        {
            SetButtonText();
            SetText();
        }
        #endregion
        #region Han_CheckedChanged
        private void Han_CheckedChanged(object sender, EventArgs e)
        {
            SetButtonText();
            SetText();
        }
        #endregion
        #region Back_MouseClick
        private void Back_MouseClick(object sender, MouseEventArgs e)
        {
            if (str != null && str.Length > 0)
            {
                str = str.Substring(0, lbl.Text.Length - 1);
                parseHangul.InitState();
            }
            SetText();
            CheckShift();
        }
        #endregion
        #region Ent_MouseClick
        private void Ent_MouseClick(object sender, MouseEventArgs e)
        {
            if (str.Length != lbl.Text.Length && str.Length == 0)
            {
                str = strOrigin;
                bOK = true; Hide();
            }
            else
            {
                bOK = true; Hide();
            }
        }
        #endregion
        #region Key_MouseClick
        private void Key_MouseClick(object sender, MouseEventArgs e)
        {
            DvButton d = (DvButton)sender;
            if (PState == HANGUL || PState == HANGUL_SHIFT)
            {
                string v = str;
                if(parseHangul.Dic.ContainsKey(KeyMap[PState][d.Name])) parseHangul.Input(ref v, parseHangul.Dic[KeyMap[PState][d.Name]]);
                else v += KeyMap[PState][d.Name];
                str = v;
            }
            else
            {
                string v = str;
                v += KeyMap[PState][d.Name];
                str = v;
            }
            SetText();
            CheckShift();
        }
        #endregion
        #region Clear_MouseClick
        private void Clear_MouseClick(object sender, MouseEventArgs e)
        {
            str = "";
            parseHangul.InitState();
            SetText();
        }
        #endregion
        #endregion

        #region Override
        #region OnExitButtonClick
        protected override void OnExitButtonClick()
        {
            bOK = false;
        }
        #endregion

        #region Show
        public override void Show()
        {
            base.Show();

        }
        #endregion
        #region Hide
        public override void Hide()
        {
            base.Hide();

            ev.Set();
        }
        #endregion
        #endregion

        #region Method
        #region SetText
        void SetText()
        {
            lbl.Text = str;
        }
        #endregion
        #region SetButtonText
        void SetButtonText()
        {
            for (int i = 0; i < 10; i++) Ns[i].Text = KeyMap[PState][Ns[i].Name];
            for (int i = 0; i < 10; i++) Us[i].Text = KeyMap[PState][Us[i].Name];
            for (int i = 0; i < 9; i++) Ms[i].Text = KeyMap[PState][Ms[i].Name];
            for (int i = 0; i < 7; i++) Ds[i].Text = KeyMap[PState][Ds[i].Name];
            for (int i = 0; i < 3; i++) Ss[i].Text = KeyMap[PState][Ss[i].Name];

            Ent.Text = "";
            Back.IconString = "fa-delete-left";
            Han.Text = "한/영";
            Num.Text = IsNumber ? (IsHangul ? "가" : "ABC") : "!#1";
            Shift.IconString = "fa-chevron-up";
            Clear.IconString = "fa-eraser";
        }
        #endregion
        #region CheckShift
        void CheckShift()
        {
            if (!UseShiftHolding)
            {
                if (Shift.Checked) Shift.Checked = false;
                SetButtonText();
            }
        }
        #endregion

        #region show
        void show(string Title, Action act1, Action act2)
        {
            lbl.FontSize = ButtonTextSize;
            
            this.Width = Math.Max(480, Width);
            this.Height = Math.Max(300, Height);

            this.Title = Title;

            if (Design != null)
            {
                var c = Design.Theme.ButtonColor.BrightnessTransmit(Design.Theme.KeySpecialButtonBrightness);
                Ent.ButtonColor = c;
                Back.ButtonColor = c;
                Clear.ButtonColor = c;
                Shift.ButtonColor = c;
                Han.ButtonColor = c;
                Num.ButtonColor = c;
            }

            parseHangul.InitState();
            act1();

            foreach (var c in gpnl.Controls.Values)
            {
                var btn = c as DvButton;
                if (c is DvButton) { ((DvButton)c).FontSize = ButtonTextSize; ((DvButton)c).IconSize = ButtonIconSize; }
                if (c is DvLabel) { ((DvLabel)c).FontSize = ButtonTextSize; ((DvLabel)c).IconSize = ButtonIconSize; }
                if (c is DvToggleButton) { ((DvToggleButton)c).FontSize = ButtonTextSize; ((DvToggleButton)c).IconSize = ButtonIconSize; }
            }

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) => { Thread.Sleep(DvDesign.HIDE_TIME); act2(); });

            });
        }
        #endregion
        #region ShowKeyboard
        public void ShowKeyboard(string Title, KeyboardMode Mode, string value, Action<string> result)
        {
            Han.Checked = Mode == KeyboardMode.Korea;
            Num.Checked = Mode == KeyboardMode.Number;
            Shift.Checked = false;

            show(Title, () =>
            {
                str = "";
                strOrigin = value ?? "";
                lbl.Text = value ?? "";

                #region Clear
                gpnl.Controls.Clear();
                gpnl.Rows[5].Columns.Clear();
                #endregion
                #region Panel
                if (Mode != KeyboardMode.EnglishOnly)
                {
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 15 });
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 10 });
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 10 });
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 40 });
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 10 });
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 15 });
                }
                else
                {
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 20 });
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 60 });
                    gpnl.Rows[5].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 20 });
                }
                #endregion
                #region Control
                if (Mode != KeyboardMode.EnglishOnly)
                {
                    gpnl.Controls.Add(lbl, 0, 0);
                    for (int i = 0; i < 10; i++) gpnl.Controls.Add(Ns[i], i, 1);
                    for (int i = 0; i < 10; i++) gpnl.Controls.Add(Us[i], i, 2);
                    for (int i = 0; i < 9; i++) gpnl.Controls.Add(Ms[i], i + 1, 3);
                    for (int i = 0; i < 7; i++) gpnl.Controls.Add(Ds[i], i + 1, 4);
                    for (int i = 0; i < 3; i++) gpnl.Controls.Add(Ss[i], i + 2, 5);
                    gpnl.Controls.Add(Shift, 0, 4);
                    gpnl.Controls.Add(Back, 8, 4);
                    gpnl.Controls.Add(Num, 0, 5);
                    gpnl.Controls.Add(Han, 1, 5);
                    gpnl.Controls.Add(Ent, 5, 5);
                }
                else
                {
                    gpnl.Controls.Add(lbl, 0, 0);
                    for (int i = 0; i < 10; i++) gpnl.Controls.Add(Ns[i], i, 1);
                    for (int i = 0; i < 10; i++) gpnl.Controls.Add(Us[i], i, 2);
                    for (int i = 0; i < 9; i++) gpnl.Controls.Add(Ms[i], i + 1, 3);
                    for (int i = 0; i < 7; i++) gpnl.Controls.Add(Ds[i], i + 1, 4);
                    gpnl.Controls.Add(Ss[1], 1, 5);
                    gpnl.Controls.Add(Shift, 0, 4);
                    gpnl.Controls.Add(Back, 8, 4);
                    gpnl.Controls.Add(Ent, 2, 5);
                    gpnl.Controls.Add(Clear, 0, 5);
                }
                #endregion

                SetButtonText();

            }, () =>
            {
                if (bOK) result(str);
                else result(null);
            });
        }

        public void ShowKeyboard(string Title, Action<string> result) => ShowKeyboard(Title, KeyboardMode.English, "", result);
        public void ShowKeyboard(string Title, string value, Action<string> result) => ShowKeyboard(Title, KeyboardMode.English, value, result);
        public void ShowKeyboard(string Title, KeyboardMode Mode, Action<string> result) => ShowKeyboard(Title, Mode, "", result);
        #endregion
        #endregion
    }

    #region Enum : KeyboardMode
    public enum KeyboardMode { Korea, English, Number, EnglishOnly }
    #endregion
    #region Class : Hangul
    #region enum : State
    enum State
    {
        Cho,
        Jung,
        Jong
    }
    #endregion
    #region enum : Cho
    enum Cho
    {
        None = -1,
        r = 0,  //ㄱ
        R = 1,  //ㄲ
        s = 2,  //ㄴ
        e = 3,  //ㄷ
        E = 4,  //ㄸ
        f = 5,  //ㄹ
        a = 6,  //ㅁ
        q = 7,  //ㅂ
        Q = 8,  //ㅃ
        t = 9,  //ㅅ
        T = 10, //ㅆ
        d = 11, //ㅇ
        w = 12, //ㅈ
        W = 13, //ㅉ
        c = 14, //ㅊ
        z = 15, //ㅋ
        x = 16, //ㅌ
        v = 17, //ㅍ
        g = 18  //ㅎ
    }
    #endregion
    #region enum : Jung
    enum Jung
    {
        None = -1,
        k = 0,  //ㅏ
        o = 1,  //ㅐ
        i = 2,  //ㅑ
        O = 3,  //ㅒ
        j = 4,  //ㅓ
        p = 5,  //ㅔ
        u = 6,  //ㅕ
        P = 7,  //ㅖ
        h = 8,  //ㅗ
        hk = 9, //ㅘ
        ho = 10,//ㅙ
        hl = 11,//ㅚ
        y = 12, //ㅛ
        n = 13, //ㅜ
        nj = 14,//ㅝ
        np = 15,//ㅞ
        nl = 16,//ㅟ
        b = 17, //ㅠ
        m = 18, //ㅡ
        ml = 19,//ㅢ
        l = 20 //ㅣ
    }
    #endregion
    #region enum : Jong
    enum Jong
    {
        None = -1,
        r = 1,    //ㄱ
        R = 2,    //ㄲ
        rt = 3,    //ㄳ
        s = 4,    //ㄴ
        sw = 5,    //ㄵ
        sg = 6,    //ㄶ
        e = 7,    //ㄷ
        f = 8,    //ㄹ
        fr = 9,    //ㄺ
        fa = 10,   //ㄻ
        fq = 11,   //ㄼ
        ft = 12,   //ㄽ
        fx = 13,   //ㄾ
        fv = 14,   //ㄿ
        fg = 15,   //ㅀ
        a = 16,   //ㅁ
        q = 17,   //ㅂ
        qt = 18,   //ㅄ
        t = 19,   //ㅅ
        T = 20,   //ㅆ
        d = 21,    //ㅇ
        w = 22,   //ㅈ
        c = 23,   //ㅊ
        z = 24,   //ㅋ
        x = 25,   //ㅌ
        v = 26,   //ㅍ
        g = 27    //ㅎ
    }
    #endregion
    #region Class : Hangul
    class Hangul
    {
        private const int KIYEOK = 0x1100;
        private const int A = 0x1161;
        private const int GA = 0xac00;

        private const int CHO_COUNT = 0x0013;
        private const int JUNG_COUNT = 0x0015;
        private const int JONG_COUNT = 0x001c;

        private const char JUNG_INIT_CHAR = '.';
        private const char JONG_INIT_CHAR = '.';

        private string _currentChar;
        private string _result;
        private State _state;

        private int _cho;
        private int _jung;
        private char _jungFirst;
        private bool _jungPossible;
        private int _jong;
        private char _jongFirst;
        private char _jongLast;
        private bool _jongPossible;

        public Dictionary<string, char> Dic = new Dictionary<string, char>();
        public Hangul()
        {
            Dic.Add("ㅂ", 'q');
            Dic.Add("ㅃ", 'Q');
            Dic.Add("ㅈ", 'w');
            Dic.Add("ㅉ", 'W');
            Dic.Add("ㄷ", 'e');
            Dic.Add("ㄸ", 'E');
            Dic.Add("ㄱ", 'r');
            Dic.Add("ㄲ", 'R');
            Dic.Add("ㅅ", 't');
            Dic.Add("ㅆ", 'T');
            Dic.Add("ㅛ", 'y');
            Dic.Add("ㅕ", 'u');
            Dic.Add("ㅑ", 'i');
            Dic.Add("ㅐ", 'o');
            Dic.Add("ㅒ", 'O');
            Dic.Add("ㅔ", 'p');
            Dic.Add("ㅖ", 'P');

            Dic.Add("ㅁ", 'a');
            Dic.Add("ㄴ", 's');
            Dic.Add("ㅇ", 'd');
            Dic.Add("ㄹ", 'f');
            Dic.Add("ㅎ", 'g');
            Dic.Add("ㅗ", 'h');
            Dic.Add("ㅓ", 'j');
            Dic.Add("ㅏ", 'k');
            Dic.Add("ㅣ", 'l');

            Dic.Add("ㅋ", 'z');
            Dic.Add("ㅌ", 'x');
            Dic.Add("ㅊ", 'c');
            Dic.Add("ㅍ", 'v');
            Dic.Add("ㅠ", 'b');
            Dic.Add("ㅜ", 'n');
            Dic.Add("ㅡ", 'm');


            _currentChar = string.Empty;
            _result = string.Empty;
            _state = State.Cho;

            _cho = -1;
            _jung = -1;
            _jungFirst = JUNG_INIT_CHAR;
            _jong = -1;
            _jongFirst = JONG_INIT_CHAR;
            _jongLast = JONG_INIT_CHAR;
        }

        private char GetSingleJa(int value)
        {
            byte[] bytes = BitConverter.GetBytes((short)(0x1100 + value));
            return Convert.ToChar(Encoding.Unicode.GetString(bytes, 0, bytes.Length));

        }


        private char GetSingleMo(int value) //하나의 모음으로만 구성된 완성형 글자를 반환
        {
            byte[] bytes = BitConverter.GetBytes((short)(0x1161 + value));
            return Convert.ToChar(Encoding.Unicode.GetString(bytes, 0, bytes.Length));
        }

        private char GetCompleteChar()
        {
            int tempJong = 0;
            if (_jong < 0)
                tempJong = 0;
            else
                tempJong = _jong;
            int completeChar = (_cho * (JUNG_COUNT * JONG_COUNT)) + (_jung * JONG_COUNT) + tempJong + GA;
            byte[] naeBytes = BitConverter.GetBytes((short)(completeChar));
            return Convert.ToChar(Encoding.Unicode.GetString(naeBytes, 0, naeBytes.Length));
        }

        private char Filter(char ch)
        {
            if (ch == 'A') ch = 'a';
            if (ch == 'B') ch = 'b';
            if (ch == 'C') ch = 'c';
            if (ch == 'D') ch = 'd';
            if (ch == 'F') ch = 'f';
            if (ch == 'G') ch = 'g';
            if (ch == 'H') ch = 'h';
            if (ch == 'I') ch = 'i';
            if (ch == 'J') ch = 'j';
            if (ch == 'K') ch = 'k';
            if (ch == 'L') ch = 'l';
            if (ch == 'M') ch = 'm';
            if (ch == 'N') ch = 'n';
            if (ch == 'S') ch = 's';
            if (ch == 'U') ch = 'u';
            if (ch == 'V') ch = 'v';
            if (ch == 'X') ch = 'x';
            if (ch == 'Y') ch = 'y';
            if (ch == 'Z') ch = 'z';

            return ch;
        }

        public void Input(ref string source, char ch)
        {
            ch = Filter(ch);

            int code = (int)ch;
            if (code == 8)
            {
                if (source.Length <= 0)
                    return;

                if (_state == State.Cho)
                {
                    source = source.Substring(0, source.Length - 1);
                }
                else if (_state == State.Jung && _jungFirst.Equals(JUNG_INIT_CHAR))
                {
                    _state = State.Cho;
                    source = source.Substring(0, source.Length - 1);
                }
                else if (_jungPossible && (_jung != 8 && _jung != 13 && _jung != 18) && _jongFirst.Equals(JONG_INIT_CHAR) && _jongLast.Equals(JONG_INIT_CHAR))
                {
                    _state = State.Jung;
                    source = source.Substring(0, source.Length - 1);
                    _jung = CheckJung(_jungFirst.ToString());
                    _jong = -1;
                    source += GetCompleteChar();
                    _jungPossible = true;
                }
                else if ((_state == State.Jong || _state == State.Jung) && !_jungFirst.Equals(JUNG_INIT_CHAR) && _jongFirst.Equals(JONG_INIT_CHAR) && _jongLast.Equals(JONG_INIT_CHAR))
                {
                    _state = State.Jung;
                    _jungFirst = JUNG_INIT_CHAR;
                    _jungPossible = false;
                    _jung = -1;
                    source = source.Substring(0, source.Length - 1);
                    source += GetSingleJa(_cho);
                }
                else if (_state == State.Jong && !_jongFirst.Equals(JONG_INIT_CHAR) && !_jongLast.Equals(JONG_INIT_CHAR))
                {
                    _state = State.Jong;
                    source = source.Substring(0, source.Length - 1);
                    _jongLast = JONG_INIT_CHAR;
                    _jong = CheckJong(_jongFirst.ToString());
                    source += GetCompleteChar();
                    _jongPossible = true;
                }
                else if (_state == State.Jong && !_jongFirst.Equals(JONG_INIT_CHAR))
                {
                    int temp = CheckJung(_jungFirst.ToString());
                    if (temp == 8 || temp == 13 || temp == 18)
                    {
                        _jungPossible = true;
                        _state = State.Jung;
                    }
                    else
                    {
                        _state = State.Jong;
                    }
                    source = source.Substring(0, source.Length - 1);
                    _jong = -1;
                    _jongFirst = JONG_INIT_CHAR;
                    source += GetCompleteChar();
                }
                return;
            }

            if (!((code >= 97 && code <= 122) || (code >= 65 && code <= 90)))
            {
                _cho = -1;
                _jung = -1;
                _jong = -1;
                _jungFirst = JUNG_INIT_CHAR;
                _jongFirst = JONG_INIT_CHAR;
                _jongLast = JONG_INIT_CHAR;
                _state = State.Cho;
                source += ch;
                return;
            }

            if (_state == State.Cho)
            {
                _cho = CheckCho(ch);

                if (_cho >= 0)
                {
                    _state = State.Jung;
                    source += GetSingleJa(_cho);
                }
                else
                {
                    _state = State.Jung;
                    Input(ref source, ch);
                }
            }
            else if (_state == State.Jung)
            {

                if (_jung < 0)
                {
                    _jung = CheckJung(ch.ToString());
                    if (_jung < 0)
                    {
                        _state = State.Cho;
                        Input(ref source, ch);
                        return;
                    }

                    if (_cho < 0)
                    {
                        source += GetSingleMo(CheckJung(ch.ToString()));
                        _state = State.Cho;
                        _jung = -1;
                        return;
                    }
                    else
                    {
                        if (_jung == 8 || _jung == 13 || _jung == 18)
                        {
                            _jungPossible = true;
                            _state = State.Jung;
                        }
                        else
                        {
                            _state = State.Jong;
                        }
                        _jungFirst = ch;
                        source = source.Substring(0, source.Length - 1);
                        source += GetCompleteChar();
                    }
                }
                else
                {
                    string jung = string.Empty;
                    jung += _jungFirst;
                    jung += ch;

                    int temp = CheckJung(jung);
                    if (temp > 0)
                    {
                        _jung = temp;
                        source = source.Substring(0, source.Length - 1);
                        source += GetCompleteChar();
                        _state = State.Jong;
                    }
                    else
                    {
                        _state = State.Jong;
                        Input(ref source, ch);
                    }
                }
            }
            else if (_state == State.Jong)
            {
                if (_jong < 0)
                {
                    _jong = CheckJong(ch.ToString());

                    if (_jong > 0)
                    {
                        source = source.Substring(0, source.Length - 1);
                        source += GetCompleteChar();

                        _jongFirst = ch;
                        if (_jong == 1 || _jong == 4 || _jong == 8 || _jong == 17)
                        {
                            _jongPossible = true;
                        }
                    }
                    else if (CheckJung(ch.ToString()) >= 0)
                    {
                        _state = State.Jung;
                        _cho = -1;
                        _jung = -1;
                        Input(ref source, ch);
                        return;
                    }
                    else if (CheckCho(ch) >= 0)
                    {
                        _jongPossible = false;
                        _jong = 0;
                        Input(ref source, ch);
                    }
                }
                else
                {
                    if (_jongPossible)
                    {
                        _jongPossible = false;
                        string jong = string.Empty;
                        jong += _jongFirst;
                        jong += ch;

                        int temp = CheckJong(jong);

                        if (temp > 0)
                        {
                            _jongLast = ch;
                            _jong = temp;
                            source = source.Substring(0, source.Length - 1);
                            source += GetCompleteChar();
                        }
                        else
                        {
                            Input(ref source, ch);
                        }
                    }
                    else
                    {
                        if (CheckCho(ch) >= 0)
                        {
                            _jongFirst = JONG_INIT_CHAR;
                            _jongLast = JONG_INIT_CHAR;

                            _state = State.Cho;
                            _jung = -1;
                            _jong = -1;
                            _jungFirst = JUNG_INIT_CHAR;
                            _jungPossible = false;
                            Input(ref source, ch);
                        }
                        else
                        {
                            if (_jongLast.Equals(JONG_INIT_CHAR))
                            {
                                source = source.Substring(0, source.Length - 1);
                                _jong = 0;
                                source += GetCompleteChar();

                                _cho = CheckCho(_jongFirst);
                            }
                            else
                            {
                                source = source.Substring(0, source.Length - 1);
                                _jong = CheckJong(_jongFirst.ToString());
                                source += GetCompleteChar();

                                _cho = CheckCho(_jongLast);
                            }
                            source += GetSingleJa(_cho);

                            _jongFirst = JONG_INIT_CHAR;
                            _jongLast = JONG_INIT_CHAR;
                            _jungPossible = false;
                            _jung = -1;
                            _jong = -1;
                            _state = State.Jung;
                            Input(ref source, ch);
                        }
                    }
                }
            }
        }

        private int CheckCho(char ch)
        {
            string[] ar = { "r", "R", "s", "e", "E", "f", "a", "q", "Q", "t", "T", "d", "w", "W", "c", "z", "x", "v", "g" };

            for (int i = 0; i < ar.Length; i++)
            {
                if (ar[i].ToString().Equals(ch.ToString()))
                {
                    return i;
                }
            }
            return -1;
        }

        private int CheckJung(string ch)
        {
            string[] ar = { "k", "o", "i", "O", "j", "p", "u", "P", "h", "hk", "ho", "hl", "y", "n", "nj", "np", "nl", "b", "m", "ml", "l" };

            for (int i = 0; i < ar.Length; i++)
            {
                if (ar[i].ToString().Equals(ch.ToString()))
                {
                    return i;
                }
            }
            return -1;
        }

        private int CheckJong(string ch)
        {
            string[] ar = { "r", "R", "rt", "s", "sw", "sg", "e", "f", "fr", "fa", "fq", "ft", "fx", "fv", "fg", "a", "q", "qt", "t", "T", "d", "w", "c", "z", "x", "v", "g" };

            for (int i = 0; i < ar.Length; i++)
            {
                if (ar[i].ToString().Equals(ch.ToString()))
                {
                    return i + 1;
                }
            }
            return -1;
        }

        public void InitState()
        {
            _cho = -1;
            _jung = -1;
            _jong = -1;
            _state = State.Cho;
            _jungPossible = false;
            _jongPossible = false;
            _jungFirst = JUNG_INIT_CHAR;
            _jongFirst = JONG_INIT_CHAR;
            _jongLast = JONG_INIT_CHAR;
        }
    }
    #endregion
    #endregion
}
