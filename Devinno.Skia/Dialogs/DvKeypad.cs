using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvKeypad : DvWindow
    {
        #region Const
        const int MessageTime = 1500;
        #endregion

        #region Properties 
        public float LabelTextSize { get; set; } = 14;
        public float ButtonTextSize { get; set; } = 14;
        public float ButtonIconSize { get; set; } = 16;
        public bool ButtonGradient { get; set; } = false;

        public int? DialogWidth { get; set; } = null;
        public int? DialogHeight { get; set; } = null;
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvButton btn0;
        DvButton btn1;
        DvButton btn2;
        DvButton btn3;
        DvButton btn4;
        DvButton btn5;
        DvButton btn6;
        DvButton btn7;
        DvButton btn8;
        DvButton btn9;
        DvButton btnA;
        DvButton btnB;
        DvButton btnC;
        DvButton btnD;
        DvButton btnE;
        DvButton btnF;
        DvButton btnDot;
        DvButton btnBack;
        DvButton btnClear;
        DvButton btnSign;
        DvButton btnEnter;
        DvLabel lbl;

        DvTableLayoutPanel tpnl;

        string sval = "";
        string svalOrigin = "";
        string svalMessage = "";

        int mode = 0;
        Type valueType;
        bool bOK = false;

        byte? minU8 = null, maxU8 = null;
        ushort? minU16 = null, maxU16 = null;
        uint? minU32 = null, maxU32 = null;
        ulong? minU64 = null, maxU64 = null;
        sbyte? minI8 = null, maxI8 = null;
        short? minI16 = null, maxI16 = null;
        int? minI32 = null, maxI32 = null;
        long? minI64 = null, maxI64 = null;
        float? minF1 = null, maxF1 = null;
        double? minF2 = null, maxF2 = null;
        decimal? minF3 = null, maxF3 = null;
        #endregion

        #region Constructor
        public DvKeypad()
        {
            IconString = "fa-grip-vertical";
            Width = 450;
            Height = 600;

            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = new Padding(10, TitleHeight + 10, 10, 10), Fill = true };

            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));

            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));

            Controls.Add(tpnl);
            #endregion
            #region New
            lbl = new DvLabel() { Name = nameof(lbl), FontSize = LabelTextSize };
            btn0 = new DvButton() { Name = nameof(btn0), Text = "0", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn1 = new DvButton() { Name = nameof(btn1), Text = "1", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn2 = new DvButton() { Name = nameof(btn2), Text = "2", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn3 = new DvButton() { Name = nameof(btn3), Text = "3", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn4 = new DvButton() { Name = nameof(btn4), Text = "4", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn5 = new DvButton() { Name = nameof(btn5), Text = "5", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn6 = new DvButton() { Name = nameof(btn6), Text = "6", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn7 = new DvButton() { Name = nameof(btn7), Text = "7", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn8 = new DvButton() { Name = nameof(btn8), Text = "8", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btn9 = new DvButton() { Name = nameof(btn9), Text = "9", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btnA = new DvButton() { Name = nameof(btnA), Text = "A", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btnB = new DvButton() { Name = nameof(btnB), Text = "B", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btnC = new DvButton() { Name = nameof(btnC), Text = "C", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btnD = new DvButton() { Name = nameof(btnD), Text = "D", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btnE = new DvButton() { Name = nameof(btnE), Text = "E", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btnF = new DvButton() { Name = nameof(btnF), Text = "F", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };

            btnDot = new DvButton { Name = nameof(btnDot), Text = ".", Gradient = ButtonGradient, FontSize = ButtonTextSize, IconSize = ButtonIconSize };
            btnBack = new DvButton { Name = nameof(btnBack), IconString = "fa-delete-left", Text = "", FontSize = ButtonTextSize, IconSize = ButtonIconSize, Gradient = ButtonGradient };
            btnClear = new DvButton { Name = nameof(btnClear), IconString = "fa-eraser", Text = "", FontSize = ButtonTextSize, IconSize = ButtonIconSize, Gradient = ButtonGradient };
            btnSign = new DvButton { Name = nameof(btnSign), IconString = "fa-plus-minus", Text = "", FontSize = ButtonTextSize, IconSize = ButtonIconSize, Gradient = ButtonGradient };
            btnEnter = new DvButton { Name = nameof(btnEnter), IconString = "mt keyboard_return", FontSize = ButtonTextSize, IconSize = ButtonIconSize, Gradient = ButtonGradient };
            #endregion

            #region Enter / Clear
            btnClear.ButtonClick += (o, s) => { sval = ""; SetText(); };
            btnEnter.ButtonClick += (o, s) =>
            {
                if (sval.Length != lbl.Text.Length && sval.Length == 0)
                {
                    if (svalOrigin == lbl.Text)
                    {
                        sval = svalOrigin;
                        bOK = true;
                        Hide();
                    }
                    else
                    {
                        lbl.Text = "";
                    }
                }
                else
                {
                    decimal v1;
                    long v2;
                    ulong v3;

                    if (mode == 0 && ulong.TryParse(sval, out v3))
                    {
                        var valid = true;
                        if (valueType == typeof(byte)) { if (minU8.HasValue && maxU8.HasValue) valid = minU8.Value <= Convert.ToByte(v3) && Convert.ToByte(v3) <= maxU8.Value; }
                        else if (valueType == typeof(ushort)) { if (minU16.HasValue && maxU16.HasValue) valid = minU16.Value <= Convert.ToUInt16(v3) && Convert.ToUInt16(v3) <= maxU16.Value; }
                        else if (valueType == typeof(uint)) { if (minU32.HasValue && maxU32.HasValue) valid = minU32.Value <= Convert.ToUInt32(v3) && Convert.ToUInt32(v3) <= maxU32.Value; }
                        else if (valueType == typeof(ulong)) { if (minU64.HasValue && maxU64.HasValue) valid = minU64.Value <= Convert.ToUInt64(v3) && Convert.ToUInt64(v3) <= maxU64.Value; }

                        if (valid)
                        {
                            Hide(); bOK = true;
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem((o) =>
                            {
                                svalMessage = "범위 초과"; SetText();
                                Thread.Sleep(MessageTime);
                                svalMessage = ""; SetText();
                                sval = "";
                            });
                        }
                    }
                    else if (mode == 1 && long.TryParse(sval, out v2))
                    {
                        var valid = true;
                        if (valueType == typeof(sbyte)) { if (minI8.HasValue && maxI8.HasValue) valid = minI8.Value <= Convert.ToSByte(v2) && Convert.ToSByte(v2) <= maxI8.Value; }
                        else if (valueType == typeof(short)) { if (minI16.HasValue && maxI16.HasValue) valid = minI16.Value <= Convert.ToInt16(v2) && Convert.ToInt16(v2) <= maxI16.Value; }
                        else if (valueType == typeof(int)) { if (minI32.HasValue && maxI32.HasValue) valid = minI32.Value <= Convert.ToInt32(v2) && Convert.ToInt32(v2) <= maxI32.Value; }
                        else if (valueType == typeof(long)) { if (minI64.HasValue && maxI64.HasValue) valid = minI64.Value <= Convert.ToInt64(v2) && Convert.ToInt64(v2) <= maxI64.Value; }

                        if (valid)
                        {
                            Hide(); bOK = true;
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem((o) =>
                            {
                                svalMessage = "범위 초과"; SetText();
                                Thread.Sleep(MessageTime);
                                svalMessage = ""; SetText();
                                sval = "";
                            });
                        }
                    }
                    else if (mode == 2 && decimal.TryParse(sval, out v1))
                    {
                        var valid = true;
                        if (valueType == typeof(float)) { if (minF1.HasValue && maxF1.HasValue) valid = minF1.Value <= Convert.ToSingle(v1) && Convert.ToSingle(v1) <= maxF1.Value; }
                        else if (valueType == typeof(double)) { if (minF2.HasValue && maxF2.HasValue) valid = minF2.Value <= Convert.ToDouble(v1) && Convert.ToDouble(v1) <= maxF2.Value; }
                        else if (valueType == typeof(decimal)) { if (minF3.HasValue && maxF3.HasValue) valid = minF3.Value <= Convert.ToDecimal(v1) && Convert.ToDecimal(v1) <= maxF3.Value; }

                        if (valid)
                        {
                            Hide(); bOK = true;
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem((o) =>
                            {
                                svalMessage = "범위 초과"; SetText();
                                Thread.Sleep(MessageTime);
                                svalMessage = ""; SetText();
                                sval = "";
                            });
                        }
                    }
                    else if (mode == 3 && long.TryParse(sval, out v2))
                    {
                        Hide(); bOK = true;
                    }
                    else if (mode == 4 && long.TryParse(sval, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out v2)) { Hide(); bOK = true; }
                }
            };
            #endregion
            #region 0~9
            btn0.ButtonClick += (o, s) => { sval += "0"; SetText(); };
            btn1.ButtonClick += (o, s) => { sval += "1"; SetText(); };
            btn2.ButtonClick += (o, s) => { sval += "2"; SetText(); };
            btn3.ButtonClick += (o, s) => { sval += "3"; SetText(); };
            btn4.ButtonClick += (o, s) => { sval += "4"; SetText(); };
            btn5.ButtonClick += (o, s) => { sval += "5"; SetText(); };
            btn6.ButtonClick += (o, s) => { sval += "6"; SetText(); };
            btn7.ButtonClick += (o, s) => { sval += "7"; SetText(); };
            btn8.ButtonClick += (o, s) => { sval += "8"; SetText(); };
            btn9.ButtonClick += (o, s) => { sval += "9"; SetText(); };
            #endregion
            #region A~F 
            btnA.ButtonClick += (o, s) => { sval += "A"; SetText(); };
            btnB.ButtonClick += (o, s) => { sval += "B"; SetText(); };
            btnC.ButtonClick += (o, s) => { sval += "C"; SetText(); };
            btnD.ButtonClick += (o, s) => { sval += "D"; SetText(); };
            btnE.ButtonClick += (o, s) => { sval += "E"; SetText(); };
            btnF.ButtonClick += (o, s) => { sval += "F"; SetText(); };
            #endregion
            #region Dot
            btnDot.ButtonClick += (o, s) =>
            {
                if (sval.IndexOf('.') == -1)
                {
                    if (sval.Length == 0) sval += "0";
                    sval += ".";
                }
                SetText();
            };
            #endregion
            #region Sign
            btnSign.ButtonClick += (o, s) =>
            {
                decimal n = 0;
                if (decimal.TryParse(sval, out n))
                {
                    if (n >= 0 && sval.Substring(0, 1) != "-") sval = sval.Insert(0, "-");
                    else if (n <= 0 && sval.Substring(0, 1) == "-") sval = sval.Substring(1);
                }
                SetText();
            };
            #endregion
            #region Back
            btnBack.ButtonClick += (o, s) =>
            {
                if (sval.Length > 0) sval = sval.Substring(0, sval.Length - 1);
                SetText();
            };
            #endregion
        }
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
            if (string.IsNullOrWhiteSpace(svalMessage))
            {
                if (mode == 4)
                {
                    #region Hex
                    long n = 0;
                    if (sval.Length > 0 && long.TryParse(sval, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out n))
                        lbl.Text = sval = n.ToString("X");
                    else
                        lbl.Text = sval = "";
                    #endregion
                }
                else if (mode == 3)
                {
                    #region Password
                    lbl.Text = new string(sval.Select(x => '*').ToArray());
                    #endregion
                }
                else if (mode == 2)
                {
                    #region float
                    decimal n = 0;
                    if (sval.Length > 0 && decimal.TryParse(sval, out n))
                    {
                        if (sval.Last() == '.') sval = lbl.Text = n.ToString() + ".";
                        else sval = lbl.Text = n.ToString();
                    }
                    else sval = lbl.Text = "";
                    #endregion
                }
                else if (mode == 1)
                {
                    #region int
                    long n = 0;
                    if (sval.Length > 0 && long.TryParse(sval, out n))
                    {
                        sval = lbl.Text = n.ToString();
                    }
                    else sval = lbl.Text = "";
                    #endregion
                }
                else if (mode == 0)
                {
                    #region uint
                    ulong n = 0;
                    if (sval.Length > 0 && ulong.TryParse(sval, out n))
                    {
                        sval = lbl.Text = n.ToString();
                    }
                    else sval = lbl.Text = "";
                    #endregion
                }
            }
            else
            {
                lbl.Text = svalMessage;
            }
        }
        #endregion

        #region show
        void show(string Title, Action actLayout, Action actOK)
        {
            #region Control Set
            lbl.FontSize = LabelTextSize;

            foreach (var c in tpnl.Controls.Values)
            {
                var btn = c as DvButton;
                if (c is DvButton) 
                {
                    var vc = c as DvButton;
                    vc.FontSize = ButtonTextSize; 
                    vc.IconSize = ButtonIconSize;
                    vc.Gradient = ButtonGradient;
                }
                if (c is DvLabel)
                {
                    var vc = c as DvLabel;
                    vc.FontSize = ButtonTextSize; 
                    vc.IconSize = ButtonIconSize;
                }
                if (c is DvToggleButton)
                {
                    var vc = c as DvToggleButton; 
                    vc.FontSize = ButtonTextSize; 
                    vc.IconSize = ButtonIconSize;
                    vc.Gradient = ButtonGradient;
                }
            }
            
            if (Design != null)
            {
                var c = Design.Theme.ButtonColor.BrightnessTransmit(Design.Theme.DownBrightness);
                btnEnter.ButtonColor = c;
                btnBack.ButtonColor = c;
                btnClear.ButtonColor = c;
                btnSign.ButtonColor = c;
            }
            #endregion
            #region Title
            var s = "";
            if (valueType == typeof(byte) && minU8.HasValue && maxU8.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minU8.Value, maxU8.Value);
            else if (valueType == typeof(ushort) && minU16.HasValue && maxU16.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minU16.Value, maxU16.Value);
            else if (valueType == typeof(uint) && minU32.HasValue && maxU32.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minU32.Value, maxU32.Value);
            else if (valueType == typeof(ulong) && minU64.HasValue && maxU64.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minU64.Value, maxU64.Value);
            else if (valueType == typeof(sbyte) && minI8.HasValue && maxI8.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minI8.Value, maxI8.Value);
            else if (valueType == typeof(short) && minI16.HasValue && maxI16.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minI16.Value, maxI16.Value);
            else if (valueType == typeof(int) && minI32.HasValue && maxI32.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minI32.Value, maxI32.Value);
            else if (valueType == typeof(long) && minI64.HasValue && maxI64.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minI64.Value, maxI64.Value);
            else if (valueType == typeof(float) && minF1.HasValue && maxF1.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minF1.Value, maxF1.Value);
            else if (valueType == typeof(double) && minF2.HasValue && maxF2.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minF2.Value, maxF2.Value);
            else if (valueType == typeof(decimal) && minF3.HasValue && maxF3.HasValue) s = string.Format(" [ {0} ~ {1} ] ", minF3.Value, maxF3.Value);

            this.Title = Title + s;
            #endregion

            actLayout();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Thread.Sleep(DvDesign.HIDE_TIME);
                    actOK();
                });

            });
        }
        #endregion
        #region ShowKeypad
        public void ShowKeypad<T>(string Title, T? value, T? min, T? max, Action<T?> result) where T : struct
        {
            this.Width = DialogWidth ?? 300;
            this.Height = DialogHeight ?? 400;

            #region min / max
            minI8 = null; minI16 = null; minI32 = null; minI64 = null;
            minU8 = null; minU16 = null; minU32 = null; minU64 = null;
            minF1 = null; minF2 = null; minF3 = null;

            maxI8 = null; maxI16 = null; maxI32 = null; maxI64 = null;
            maxU8 = null; maxU16 = null; maxU32 = null; maxU64 = null;
            maxF1 = null; maxF2 = null; maxF3 = null;
            #endregion
            #region valueType
            var m = -1;
            if (typeof(T) == typeof(sbyte)) { valueType = typeof(sbyte); m = 1; minI8 = (sbyte?)(object)min; maxI8 = (sbyte?)(object)max; }
            else if (typeof(T) == typeof(short)) { valueType = typeof(short); m = 1; minI16 = (short?)(object)min; maxI16 = (short?)(object)max; }
            else if (typeof(T) == typeof(int)) { valueType = typeof(int); m = 1; minI32 = (int?)(object)min; maxI32 = (int?)(object)max; }
            else if (typeof(T) == typeof(long)) { valueType = typeof(long); m = 1; minI64 = (long?)(object)min; maxI64 = (long?)(object)max; }
            else if (typeof(T) == typeof(byte)) { valueType = typeof(byte); m = 0; minU8 = (byte?)(object)min; maxU8 = (byte?)(object)max; }
            else if (typeof(T) == typeof(ushort)) { valueType = typeof(ushort); m = 0; minU16 = (ushort?)(object)min; maxU16 = (ushort?)(object)max; }
            else if (typeof(T) == typeof(uint)) { valueType = typeof(uint); m = 0; minU32 = (uint?)(object)min; maxU32 = (uint?)(object)max; }
            else if (typeof(T) == typeof(ulong)) { valueType = typeof(ulong); m = 0; minU64 = (ulong?)(object)min; maxU64 = (ulong?)(object)max; }
            else if (typeof(T) == typeof(float)) { valueType = typeof(float); m = 2; minF1 = (float?)(object)min; maxF1 = (float?)(object)max; }
            else if (typeof(T) == typeof(double)) { valueType = typeof(double); m = 2; minF2 = (double?)(object)min; maxF2 = (double?)(object)max; }
            else if (typeof(T) == typeof(decimal)) { valueType = typeof(decimal); m = 2; minF3 = (decimal?)(object)min; maxF3 = (decimal?)(object)max; }
            else { valueType = null; m = -1; throw new Exception("숫자 자료형이 아닙니다"); }
            #endregion

            if (m != -1)
            {
                show(Title, () =>
                {
                    if (m == 0)
                    {
                        #region var
                        mode = 0;
                        sval = "";
                        svalOrigin = value.HasValue ? value.Value.ToString() : "";
                        lbl.Text = value.HasValue ? value.Value.ToString() : "";
                        #endregion
                        #region Clear
                        tpnl.Controls.Clear();
                        tpnl.Rows.Clear();
                        tpnl.Columns.Clear();
                        #endregion
                        #region Panel
                        tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                        tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                        tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                        tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                        tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));

                        tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                        tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                        tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                        tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                        #endregion
                        #region Controls
                        tpnl.Controls.Add(lbl, 0, 0, 4, 1);
                        tpnl.Controls.Add(btn7, 0, 1);
                        tpnl.Controls.Add(btn8, 1, 1);
                        tpnl.Controls.Add(btn9, 2, 1);
                        tpnl.Controls.Add(btnBack, 3, 1);
                        tpnl.Controls.Add(btn4, 0, 2);
                        tpnl.Controls.Add(btn5, 1, 2);
                        tpnl.Controls.Add(btn6, 2, 2);
                        tpnl.Controls.Add(btnClear, 3, 2);
                        tpnl.Controls.Add(btn1, 0, 3);
                        tpnl.Controls.Add(btn2, 1, 3);
                        tpnl.Controls.Add(btn3, 2, 3);
                        tpnl.Controls.Add(btnEnter, 3, 3, 1, 2);
                        tpnl.Controls.Add(btn0, 0, 4, 3, 1);
                        #endregion
                    }
                    else if (m == 1)
                    {
                        #region var
                        mode = 1;
                        sval = "";
                        svalOrigin = value.HasValue ? value.Value.ToString() : "";
                        lbl.Text = value.HasValue ? value.Value.ToString() : "";
                        #endregion

                        if (min.HasValue && max.HasValue && Convert.ToInt64((object)min.Value) >= 0 && Convert.ToInt64((object)max.Value) >= 0)
                        {
                            #region Clear
                            tpnl.Controls.Clear();
                            tpnl.Rows.Clear();
                            tpnl.Columns.Clear();
                            #endregion
                            #region Panel
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));

                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            #endregion
                            #region Controls
                            tpnl.Controls.Add(lbl, 0, 0, 4, 1);
                            tpnl.Controls.Add(btn7, 0, 1);
                            tpnl.Controls.Add(btn8, 1, 1);
                            tpnl.Controls.Add(btn9, 2, 1);
                            tpnl.Controls.Add(btnBack, 3, 1);
                            tpnl.Controls.Add(btn4, 0, 2);
                            tpnl.Controls.Add(btn5, 1, 2);
                            tpnl.Controls.Add(btn6, 2, 2);
                            tpnl.Controls.Add(btnClear, 3, 2);
                            tpnl.Controls.Add(btn1, 0, 3);
                            tpnl.Controls.Add(btn2, 1, 3);
                            tpnl.Controls.Add(btn3, 2, 3);
                            tpnl.Controls.Add(btnEnter, 3, 3, 1, 2);
                            tpnl.Controls.Add(btn0, 0, 4, 3, 1);
                            #endregion
                        }
                        else
                        {
                            #region Clear
                            tpnl.Controls.Clear();
                            tpnl.Rows.Clear();
                            tpnl.Columns.Clear();
                            #endregion
                            #region Panel
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));

                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            #endregion
                            #region Controls
                            tpnl.Controls.Add(lbl, 0, 0, 4, 1);
                            tpnl.Controls.Add(btn7, 0, 1);
                            tpnl.Controls.Add(btn8, 1, 1);
                            tpnl.Controls.Add(btn9, 2, 1);
                            tpnl.Controls.Add(btnBack, 3, 1);
                            tpnl.Controls.Add(btn4, 0, 2);
                            tpnl.Controls.Add(btn5, 1, 2);
                            tpnl.Controls.Add(btn6, 2, 2);
                            tpnl.Controls.Add(btnClear, 3, 2);
                            tpnl.Controls.Add(btn1, 0, 3);
                            tpnl.Controls.Add(btn2, 1, 3);
                            tpnl.Controls.Add(btn3, 2, 3);
                            tpnl.Controls.Add(btnSign, 3, 3);
                            tpnl.Controls.Add(btn0, 0, 4, 3, 1);
                            tpnl.Controls.Add(btnEnter, 3, 4);
                            #endregion
                        }
                    }
                    else if (m == 2)
                    {
                        #region var
                        mode = 2;
                        sval = "";
                        svalOrigin = value.HasValue ? value.Value.ToString() : "";
                        lbl.Text = value.HasValue ? value.Value.ToString() : "";
                        #endregion

                        if (min.HasValue && max.HasValue && Convert.ToDecimal((object)min.Value) >= 0 && Convert.ToDecimal((object)max.Value) >= 0)
                        {
                            #region Clear
                            tpnl.Controls.Clear();
                            tpnl.Rows.Clear();
                            tpnl.Columns.Clear();
                            #endregion
                            #region Panel
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));

                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            #endregion
                            #region Controls
                            tpnl.Controls.Add(lbl, 0, 0, 4, 1);
                            tpnl.Controls.Add(btn7, 0, 1);
                            tpnl.Controls.Add(btn8, 1, 1);
                            tpnl.Controls.Add(btn9, 2, 1);
                            tpnl.Controls.Add(btnBack, 3, 1);
                            tpnl.Controls.Add(btn4, 0, 2);
                            tpnl.Controls.Add(btn5, 1, 2);
                            tpnl.Controls.Add(btn6, 2, 2);
                            tpnl.Controls.Add(btnClear, 3, 2);
                            tpnl.Controls.Add(btn1, 0, 3);
                            tpnl.Controls.Add(btn2, 1, 3);
                            tpnl.Controls.Add(btn3, 2, 3);
                            tpnl.Controls.Add(btnEnter, 3, 3, 1, 2);
                            tpnl.Controls.Add(btn0, 0, 4, 2, 1);
                            tpnl.Controls.Add(btnDot, 2, 4);
                            #endregion
                        }
                        else
                        {
                            #region Clear
                            tpnl.Controls.Clear();
                            tpnl.Rows.Clear();
                            tpnl.Columns.Clear();
                            #endregion
                            #region Panel
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));

                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                            #endregion
                            #region Controls
                            tpnl.Controls.Add(lbl, 0, 0, 4, 1);
                            tpnl.Controls.Add(btn7, 0, 1);
                            tpnl.Controls.Add(btn8, 1, 1);
                            tpnl.Controls.Add(btn9, 2, 1);
                            tpnl.Controls.Add(btnBack, 3, 1);
                            tpnl.Controls.Add(btn4, 0, 2);
                            tpnl.Controls.Add(btn5, 1, 2);
                            tpnl.Controls.Add(btn6, 2, 2);
                            tpnl.Controls.Add(btnClear, 3, 2);
                            tpnl.Controls.Add(btn1, 0, 3);
                            tpnl.Controls.Add(btn2, 1, 3);
                            tpnl.Controls.Add(btn3, 2, 3);
                            tpnl.Controls.Add(btnSign, 3, 3);
                            tpnl.Controls.Add(btn0, 0, 4, 2, 1);
                            tpnl.Controls.Add(btnDot, 2, 4);
                            tpnl.Controls.Add(btnEnter, 3, 4);
                            #endregion
                        }
                    }

                }, () =>
                {
                    #region OK
                    if (bOK)
                    {
                        if (valueType == typeof(sbyte)) result((T)(object)Convert.ToSByte(MathTool.Constrain(Convert.ToInt64(sval), minI8 ?? sbyte.MinValue, maxI8 ?? sbyte.MaxValue)));
                        else if (valueType == typeof(short)) result((T)(object)Convert.ToInt16(MathTool.Constrain(Convert.ToInt64(sval), minI16 ?? short.MinValue, maxI16 ?? short.MaxValue)));
                        else if (valueType == typeof(int)) result((T)(object)Convert.ToInt32(MathTool.Constrain(Convert.ToInt64(sval), minI32 ?? int.MinValue, maxI32 ?? int.MaxValue)));
                        else if (valueType == typeof(long)) result((T)(object)Convert.ToInt64(MathTool.Constrain(Convert.ToInt64(sval), minI64 ?? long.MinValue, maxI64 ?? long.MaxValue)));
                        else if (valueType == typeof(byte)) result((T)(object)Convert.ToByte(MathTool.Constrain(Convert.ToUInt64(sval), minU8 ?? byte.MinValue, maxU8 ?? byte.MaxValue)));
                        else if (valueType == typeof(ushort)) result((T)(object)Convert.ToUInt16(MathTool.Constrain(Convert.ToUInt64(sval), minU16 ?? ushort.MinValue, maxU16 ?? ushort.MaxValue)));
                        else if (valueType == typeof(uint)) result((T)(object)Convert.ToUInt32(MathTool.Constrain(Convert.ToUInt64(sval), minU32 ?? uint.MinValue, maxU32 ?? uint.MaxValue)));
                        else if (valueType == typeof(ulong)) result((T)(object)Convert.ToUInt64(MathTool.Constrain(Convert.ToUInt64(sval), minU64 ?? ulong.MinValue, maxU64 ?? ulong.MaxValue)));
                        else if (valueType == typeof(float)) result((T)(object)Convert.ToSingle(MathTool.Constrain(Convert.ToSingle(sval), minF1 ?? float.MinValue, maxF1 ?? float.MaxValue)));
                        else if (valueType == typeof(double)) result((T)(object)Convert.ToDouble(MathTool.Constrain(Convert.ToDouble(sval), minF2 ?? double.MinValue, maxF2 ?? double.MaxValue)));
                        else if (valueType == typeof(decimal)) result((T)(object)Convert.ToDecimal(MathTool.Constrain(Convert.ToDecimal(sval), minF3 ?? decimal.MinValue, maxF3 ?? decimal.MaxValue)));
                    }
                    else result(null);
                    #endregion
                });
            }
        }

        public void ShowKeypad<T>(string Title, Action<T?> result) where T : struct => ShowKeypad<T>(Title, null, null, null, result);
        public void ShowKeypad<T>(string Title, T? value, Action<T?> result) where T : struct => ShowKeypad<T>(Title, value, null, null, result);
        #endregion
        #region ShowPassword
        public void ShowPassword(string Title, string value, Action<string> result)
        {
            this.Width = DialogWidth ?? 300;
            this.Height = DialogHeight ?? 400;

            show(Title, () =>
            {
                #region var
                mode = 3;
                sval = "";
                svalOrigin = value ?? "";
                lbl.Text = value != null ? string.Concat(value.Select(x => "*").ToArray()) : "";
                #endregion
                #region Clear
                tpnl.Controls.Clear();
                tpnl.Rows.Clear();
                tpnl.Columns.Clear();
                #endregion
                #region Panel
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));

                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25));
                #endregion
                #region Controls
                tpnl.Controls.Add(lbl, 0, 0, 4, 1);
                tpnl.Controls.Add(btn7, 0, 1);
                tpnl.Controls.Add(btn8, 1, 1);
                tpnl.Controls.Add(btn9, 2, 1);
                tpnl.Controls.Add(btnBack, 3, 1);
                tpnl.Controls.Add(btn4, 0, 2);
                tpnl.Controls.Add(btn5, 1, 2);
                tpnl.Controls.Add(btn6, 2, 2);
                tpnl.Controls.Add(btnClear, 3, 2);
                tpnl.Controls.Add(btn1, 0, 3);
                tpnl.Controls.Add(btn2, 1, 3);
                tpnl.Controls.Add(btn3, 2, 3);
                tpnl.Controls.Add(btnEnter, 3, 3, 1, 2);
                tpnl.Controls.Add(btn0, 0, 4, 3, 1);
                #endregion

            }, () =>
            {
                #region OK
                if (bOK) result(sval);
                else result(null);
                #endregion
            });
        }

        public void ShowPassword(string Title, Action<string> result) => ShowPassword(Title, null, result);
        #endregion
        #region ShowHex
        public void ShowHex(string Title, string value, Action<string> result)
        {
            this.Width = DialogWidth ?? 480;
            this.Height = DialogHeight ?? 400;


            show(Title, () =>
            {
                #region var
                int n = 0;
                bool b = value != null ? int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out n) : false;

                mode = 4;
                sval = "";
                svalOrigin = b ? n.ToString("X") : "";
                lbl.Text = b ? n.ToString("X") : "";
                #endregion
                #region Clear
                tpnl.Controls.Clear();
                tpnl.Rows.Clear();
                tpnl.Columns.Clear();
                #endregion
                #region Panel
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20));

                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 20));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 20));
                #endregion
                #region Controls
                tpnl.Controls.Add(lbl, 0, 0, 5, 1);
                tpnl.Controls.Add(btnA, 0, 1);
                tpnl.Controls.Add(btn7, 1, 1);
                tpnl.Controls.Add(btn8, 2, 1);
                tpnl.Controls.Add(btn9, 3, 1);
                tpnl.Controls.Add(btnBack, 4, 1);
                tpnl.Controls.Add(btnB, 0, 2);
                tpnl.Controls.Add(btn4, 1, 2);
                tpnl.Controls.Add(btn5, 2, 2);
                tpnl.Controls.Add(btn6, 3, 2);
                tpnl.Controls.Add(btnClear, 4, 2);
                tpnl.Controls.Add(btnC, 0, 3);
                tpnl.Controls.Add(btn1, 1, 3);
                tpnl.Controls.Add(btn2, 2, 3);
                tpnl.Controls.Add(btn3, 3, 3);
                tpnl.Controls.Add(btnEnter, 4, 3, 1, 2);
                tpnl.Controls.Add(btnD, 0, 4);
                tpnl.Controls.Add(btnE, 1, 4);
                tpnl.Controls.Add(btnF, 2, 4);
                tpnl.Controls.Add(btn0, 3, 4);
                #endregion

            }, () =>
            {
                #region OK
                if (bOK) result(sval);
                else result(null);
                #endregion
            });
        }

        public void ShowHex(string Title, Action<string> result) => ShowHex(Title, null, result);
        #endregion
        #endregion
    }
}
